// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProvisioningRequestHandler.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Handler to provision request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteProvisioningJob
{    
    using System;
    using System.Configuration;
    using System.IO;
    using System.Xml.Linq;
    using JCI.Azure.Framework.Provisioning;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.Data;
    using JCI.CAM.Provisioning.Core.Entity;
    using JCI.CAM.Provisioning.Core.TemplateEntites;

    /// <summary>
    /// Handler to provision request
    /// </summary>
    public class ProvisioningRequestHandler
    {
        #region Instance Members
        /// <summary>
        /// The azure connection key
        /// </summary>
        public const string AzureConnectionKey = "ServiceBus.Connection";

        /// <summary>
        /// The request queue name key
        /// </summary>
        public const string RequestQueueNameKey = "ServiceBus.RequestQueue";

        /// <summary>
        /// Site request factory instance
        /// </summary>
        private readonly ISiteRequestFactory requestFactory;

        /// <summary>
        /// Configuration factory instance
        /// </summary>
        private readonly IConfigurationFactory configFactory;

        /// <summary>
        /// Template factory instance
        /// </summary>
        private readonly ITemplateFactory templateFactory;

        /// <summary>
        /// Application manager instance
        /// </summary>
        private readonly IAppSettingsManager appManager;

        /// <summary>
        /// App settings
        /// </summary>
        private readonly AppSettings settings;

        /// <summary>
        /// The azure service manager
        /// </summary>
        private readonly ServiceBusManager azureServiceManager = new ServiceBusManager();

        /// <summary>
        /// The template manager
        /// </summary>
        private readonly TemplateManager templateManager;

        /// <summary>
        /// The authentication
        /// </summary>
        private AppOnlyAuthenticationTenant authentication = new AppOnlyAuthenticationTenant();
        
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProvisioningRequestHandler"/> class.
        /// </summary>
        public ProvisioningRequestHandler()
        {
            ConfigurationContainers containers = null;
            TemplateConfiguration templateConfig = null;
            this.requestFactory = SiteRequestFactory.GetInstance();
            this.configFactory = ConfigurationFactoryManager.GetInstance();
            this.appManager = this.configFactory.GetAppSetingsManager();
            this.settings = this.appManager.GetAppSettings();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.settings.TemplateDefinitionFileName);            
            using (var result = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {
                XDocument xmlDocument = XDocument.Load(result);
                templateConfig = XmlSerializerHelper.Deserialize<TemplateConfiguration>(xmlDocument);

                this.templateFactory = this.configFactory.GetTemplateFactory(templateConfig);               
            }

            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.settings.TemplateConfigurationFileName);            
            using (var configFileStream = new System.IO.FileStream(configFilePath, System.IO.FileMode.Open))
            {
                XDocument xmlConfigDocument = XDocument.Load(configFileStream);
                containers = XmlSerializerHelper.Deserialize<ConfigurationContainers>(xmlConfigDocument);
            }

            this.templateManager = this.templateFactory.GetTemplateManager(templateConfig, containers);
        }
        #endregion

        #region Public Members

        /// <summary>
        /// Processes the request queue.
        /// </summary>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Check for Azure Connection key and Request Queue Name key
        /// </exception>
        public void ProcessRequestQueue()
        {
            LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.ProcessRequestQueue - Connecting to Azure Service Queue", LogEventID.InformationWrite);
            this.azureServiceManager.AzureConnectionString = ConfigurationManager.AppSettings[AzureConnectionKey];
            this.azureServiceManager.RequestQueueName = ConfigurationManager.AppSettings[RequestQueueNameKey];
            if (string.IsNullOrEmpty(this.azureServiceManager.AzureConnectionString))
            {
                throw new ConfigurationErrorsException(
                    string.Format("Azure Configuration {0} is missing in the config file", AzureConnectionKey));
            }

            if (string.IsNullOrEmpty(this.azureServiceManager.RequestQueueName))
            {
                throw new ConfigurationErrorsException(
                   string.Format("Azure Configuration {0} is missing in the config file", RequestQueueNameKey));
            }

            LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.ProcessRequestQueue - Getting message from Queue", LogEventID.InformationWrite);
            var message = this.azureServiceManager.GetMessage();
                       
            while (message != null)
            {
                this.ProcessRequest(message);
                message = this.azureServiceManager.GetMessage();
            }

            LogHelper.LogInformation("Comleted Processing Queue. Starting Site Request cleanup tasks.", LogEventID.InformationWrite);

            try
            {
                // Site Request cleanup tasks
                var requestManager = this.requestFactory.GetSiteRequestManager();
                requestManager.SendNotificationForSites(this.settings, this.templateManager);
                requestManager.CloseExpiredRequests(this.settings);
                requestManager.CloseWorkflowFailures(this.settings);
                LogHelper.LogInformation("Comleted Processing Queue. Starting Site Request cleanup tasks - Completed.", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.InformationWrite, ex.Message);
            }
            finally
            {
                LogHelper.LogInformation(string.Format("CI.CAM.SiteProvisioningJob.ProcessRequestQueue - There is no Site Request Messages pending in the queue {0}.", this.azureServiceManager.RequestQueueName), LogEventID.InformationWrite);
            }
        }
           
        #endregion

        #region private members

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        private void ProcessRequest(ProvisioningRequestMessage requestMessage)
        {           
            LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.ProcessRequest - Deserializing queue message", LogEventID.InformationWrite);
            var siteRequestPayload = requestMessage.SiteRequest;
            if (!string.IsNullOrEmpty(siteRequestPayload))
            {
                var siteRequest = XmlSerializerHelper.Deserialize<SiteRequestInformation>(siteRequestPayload);
                
                if (siteRequest != null)
                {
                    try
                    {
                        SiteProvisioningManager manager = new SiteProvisioningManager(this.settings);
                        bool? creationStatus = manager.ProcessNewSiteRequests(siteRequest, this.templateManager, this.settings);

                        if (creationStatus == true)
                        {
                            this.HandleSuccessResponseMessage(requestMessage, siteRequest);
                        }
                        else
                        {                            
                            if (siteRequest.BusinessUnitAdmin != null && !string.IsNullOrEmpty(siteRequest.BusinessUnitAdmin.Email))
                            {
                                var requestManager = this.requestFactory.GetSiteRequestManager();
                                requestManager.SendEmailNotificationOnException(siteRequest, SiteRequestStatus.Exception, this.settings, "A duplicate site request with same site Url. This site is already exist.", string.Empty);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogInformation(ex.Message, LogEventID.InformationWrite);           
                        LogHelper.LogError(ex);
                        this.HandleFaultResponseMessage(requestMessage, ex, siteRequest);
                    }
                }
                else
                {
                    LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.ProcessRequest - Invalid queue message", LogEventID.InformationWrite);
                }
            }
        }

        /// <summary>
        /// Handles the success response message.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="siteRequest">The site request.</param>
        private void HandleSuccessResponseMessage(ProvisioningRequestMessage requestMessage, SiteRequestInformation siteRequest)
        {
            if (siteRequest.ListItemId > 0)
            {
                try
                {                   
                    LogHelper.LogInformation(string.Format("Sending Success Email for list item - ID: {0}", siteRequest.ListItemId), LogEventID.InformationWrite);
                    var requestManager = this.requestFactory.GetSiteRequestManager();
                    requestManager.SendEmailNotification(siteRequest, SiteRequestStatus.Successful, this.settings);

                    // Update site request list.
                    LogHelper.LogInformation(string.Format("Success status update for list item - ID: {0}", siteRequest.ListItemId), LogEventID.InformationWrite);
                    string statusMessage = string.Format("{0} - {1}", SiteRequestStatus.Successful, DateTime.Now.ToString());
                    requestManager.UpdateRequestStatus(siteRequest.ListItemId, SiteRequestStatus.Successful, statusMessage, this.settings.SiteRequestListName);                                      
                }
                catch (Exception error)
                {
                    LogHelper.LogError(error, LogEventID.InformationWrite, null);
                }
            }

            if (!string.IsNullOrEmpty(requestMessage.ReplyTo))
            {
                try
                {
                    LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.HandleSuccessResponseMessage - Constructing ReplyTo message", LogEventID.InformationWrite);
                    ProvisioningResponseMessage responseMessage = new ProvisioningResponseMessage
                    {
                        SiteRequest = requestMessage.SiteRequest
                    };
                    this.azureServiceManager.SendReplyMessage(responseMessage, requestMessage.ReplyTo);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.InformationWrite, null);
                }
            }
        }

        /// <summary>
        /// Handles the fault response message.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="ex">Exception instance.</param>
        /// <param name="siteRequest">The site request.</param>
        private void HandleFaultResponseMessage(ProvisioningRequestMessage requestMessage, Exception ex, SiteRequestInformation siteRequest)
        {
            LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.HandleFaultResponseMessage - Constructing fault response message", LogEventID.InformationWrite);
            
            if (siteRequest.ListItemId > 0)
            {
                LogHelper.LogInformation(string.Format("Fault exception for Site request list item - ID: {0}", siteRequest.ListItemId), LogEventID.InformationWrite);

                try
                {  
                    var requestManager = this.requestFactory.GetSiteRequestManager();

                    // Update site request list.
                    LogHelper.LogInformation(string.Format("Fault status update for list item - ID: {0}", siteRequest.ListItemId), LogEventID.InformationWrite);
                    string statusMessage = string.Format("{0} - {1}", SiteRequestStatus.Exception, DateTime.Now.ToString());
                    requestManager.UpdateRequestStatus(siteRequest.ListItemId, SiteRequestStatus.Exception, statusMessage, this.settings.SiteRequestListName);

                    // Update WorkflowHistory list.
                    string errormesage = "Error occured in the Site Provisioning Job ProvisionSite Method for item id:" + siteRequest.ListItemId + "\n" + ex.Message.ToString();
                    LogHelper.LogInformation("Fault status update for WorkflowHistory list", LogEventID.InformationWrite);
                    requestManager.UpdateWorkFlowHistoryList(siteRequest, EventCodes.SiteProvisioningCreateSiteError, ex, errormesage, this.settings.WorkFlowHistoryListName);

                    if (siteRequest.BusinessUnitAdmin != null && !string.IsNullOrEmpty(siteRequest.BusinessUnitAdmin.Email))
                    {
                        requestManager.SendEmailNotificationOnException(siteRequest, SiteRequestStatus.Exception, this.settings, ex);
                    }
                }
                catch (Exception error)
                {
                    LogHelper.LogError(error, LogEventID.InformationWrite, ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(requestMessage.ReplyTo))
            {
                try
                {
                    ProvisioningResponseMessage responseMessage = new ProvisioningResponseMessage
                    {
                        SiteRequest = requestMessage.SiteRequest,
                        IsFaulted = true,
                        FaultMessage = ex.Message
                    };
                    this.azureServiceManager.SendReplyMessage(responseMessage, requestMessage.ReplyTo);
                }
                catch (Exception exception)
                {
                    LogHelper.LogError(exception, LogEventID.InformationWrite, null);
                }
            }
        }
        #endregion
    }
}
