// <copyright file="SiteMigrationJobHandler.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Handler to site migration request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationJob
{    
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Migration.Common.Helpers;
    using JCI.CAM.Provisioning.Core;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.SiteMigrationJob.Entities;    
    using JCI.CAM.SiteMigrationJob.Helpers;

    /// <summary>
    /// Handler to site migration request
    /// </summary>
    public class SiteMigrationJobHandler
    {
        #region Instance Members
        /// <summary>
        /// Contains error data while deserialize the schema files.
        /// </summary>
        private readonly string deserializationErrorData = string.Empty;

        /// <summary>
        /// The azure service manager
        /// </summary>
        private readonly MigrationServiceBusManager azureServiceManager = new MigrationServiceBusManager();

        /// <summary>
        /// The on premise site migration job helper
        /// </summary>
        private OnPremiseSiteMigrationJobHelper onPremiseSiteMigrationJobHelper = new OnPremiseSiteMigrationJobHelper();

        /// <summary>
        /// The online site migration job helper
        /// </summary>
        private OnlineSiteMigrationJobHelper onlineSiteMigrationJobHelper = new OnlineSiteMigrationJobHelper();
        
        /// <summary>
        /// The authentication
        /// </summary>
        private AppOnlyAuthenticationTenant authentication = new AppOnlyAuthenticationTenant();

        /// <summary>
        /// The page layouts information
        /// </summary>
        private List<PageLayouts.PageLayout> pageLayoutsInfo = new List<PageLayouts.PageLayout>();

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMigrationJobHandler"/> class.
        /// </summary>
        public SiteMigrationJobHandler()
        {
            var commonFilePath = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                // Path for schema files               
                var featuresInfoFilePath = Path.Combine(commonFilePath, @"Resources\SchemaFiles\JCIFeatureDetails.xml");
                var themesInfoFilePath = Path.Combine(commonFilePath, @"Resources\SchemaFiles\JCIThemePackage.xml");
                var listEventReceiversInfoFilePath = Path.Combine(commonFilePath, @"Resources\SchemaFiles\JCIListEventReceivers.xml");
                var pageLayoutsInfoFilePath = Path.Combine(commonFilePath, @"Resources\SchemaFiles\JCIPageLayoutDetails.xml");
                var customActionsInfoFilePath = Path.Combine(commonFilePath, @"Resources\SchemaFiles\JCICustomActions.xml");

                // De-Serializing the xml schema files of feature, themes, list evenet receiver and page layout
                SiteMigrationJobHelper.GetFeatureDetailsFromXML(featuresInfoFilePath);
                SiteMigrationJobHelper.GetThemeDetailsFromXML(themesInfoFilePath);
                SiteMigrationJobHelper.GetListEventReceiverDetailsFromXML(listEventReceiversInfoFilePath);
                this.pageLayoutsInfo = SiteMigrationJobHelper.GetPageLayoutDetailsFromXML(pageLayoutsInfoFilePath);
                SiteMigrationJobHelper.GetCustomActionsScriptBlockFromXML(customActionsInfoFilePath);
                SiteMigrationJobHelper.GetCommonFilePath(commonFilePath);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Exception occured while calling the methods to de-serialize the xml schema. {0}", commonFilePath);
                this.deserializationErrorData = errorData;
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
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
            LogHelper.LogInformation("Connecting to Azure Service Queue.", LogEventID.InformationWrite);
            this.azureServiceManager.AzureConnectionString = ConfigurationManager.AppSettings[MigrationConstants.AzureConnectionKey];
            this.azureServiceManager.RequestQueueName = ConfigurationManager.AppSettings[MigrationConstants.RequestNameKey];
            if (string.IsNullOrEmpty(this.azureServiceManager.AzureConnectionString))
            {
                throw new ConfigurationErrorsException(
                    string.Format("Azure Configuration {0} is missing in the config file", MigrationConstants.AzureConnectionKey));
            }

            if (string.IsNullOrEmpty(this.azureServiceManager.RequestQueueName))
            {
                throw new ConfigurationErrorsException(
                   string.Format("Azure Configuration {0} is missing in the config file", MigrationConstants.RequestNameKey));
            }

            LogHelper.LogInformation("Getting message from Queue to process site migration job...", LogEventID.InformationWrite);
            var message = this.azureServiceManager.GetMessage();
            int count = 0;
                       
            while (message != null)
            {
                count++;
                LogHelper.LogInformation(string.Format("Site Migration Request {0}, message: {1}", count, message.SiteMigrationRequest), LogEventID.InformationWrite);
                this.ProcessRequest(message);
                message = this.azureServiceManager.GetMessage();
            }

            LogHelper.LogInformation(string.Format("There are no Site Migration Request Messages pending in the queue {0}.", this.azureServiceManager.RequestQueueName), LogEventID.InformationWrite);
        }
           
        #endregion

        #region private members

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        private void ProcessRequest(SiteMigrationRequestMessage requestMessage)
        {
            LogHelper.LogInformation("Site migration request - Deserializing queue message.", LogEventID.InformationWrite);
            var siteRequestPayload = requestMessage.SiteMigrationRequest;
            if (!string.IsNullOrEmpty(siteRequestPayload))
            {
                var siteMigrationRequest = XmlSerializerHelper.Deserialize<SiteMigrationRequest>(siteRequestPayload);
                string siteMigrationJobErrorData = string.Empty;
                SiteMigrationJobHelper.EmptySiteMigrationErrorData();
                SiteMigrationJobHelper.MigrationRequestedSiteURL(siteMigrationRequest.SiteURL);

                if (siteMigrationRequest != null)
                {                   
                    try
                    {
                        // Checking whether error occured while deserializing the xml schema
                        if (string.IsNullOrEmpty(this.deserializationErrorData.Trim()))
                        {
                            if (GlobalData.SharePointOnPremKey)
                            {
                                siteMigrationJobErrorData = this.onPremiseSiteMigrationJobHelper.SiteMigrationJob(siteMigrationRequest.SiteURL);
                                siteMigrationJobErrorData = this.onPremiseSiteMigrationJobHelper.DeletePageLayouts(siteMigrationRequest.SiteURL, siteMigrationJobErrorData, this.pageLayoutsInfo);
                            }
                            else
                            {
                                siteMigrationJobErrorData = this.onlineSiteMigrationJobHelper.SiteMigrationJob(siteMigrationRequest.SiteURL);
                                siteMigrationJobErrorData = this.onlineSiteMigrationJobHelper.DeletePageLayouts(siteMigrationRequest.SiteURL, siteMigrationJobErrorData, this.pageLayoutsInfo);
                            }

                            this.HandleSuccessResponseMessage(requestMessage, siteMigrationRequest, siteMigrationJobErrorData);
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Error occurred while deserializing the schema files. Site details: {0}).", siteMigrationRequest.SiteURL));
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = string.Format(CultureInfo.InvariantCulture, "Error occurred while runing the site migration job for site {0}).", siteMigrationRequest.SiteURL);
                        LogHelper.LogInformation(errorMessage);                        
                        LogHelper.LogError(ex);
                        siteMigrationJobErrorData = siteMigrationJobErrorData + ex.Message + errorMessage;
                        this.HandleFaultResponseMessage(requestMessage, ex, siteMigrationRequest, siteMigrationJobErrorData);
                    }
                }
                else
                {
                    LogHelper.LogInformation("Site migration request - Invalid queue message.", LogEventID.InformationWrite);
                }
            }
        }

        /// <summary>
        /// Handles the success response message.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="siteMigrationRequest">SiteMigrationRequest object instance.</param>
        /// <param name="siteMigrationJobErrorData">Site Migration Job error Details.</param>
        private void HandleSuccessResponseMessage(SiteMigrationRequestMessage requestMessage, SiteMigrationRequest siteMigrationRequest, string siteMigrationJobErrorData)
        {
            if (siteMigrationRequest.ListItemId > 0)
            {
                try
                {
                    LogHelper.LogInformation(string.Format("Sending Success Email for list item - ID: {0}", siteMigrationRequest.ListItemId), LogEventID.InformationWrite);

                    if (string.IsNullOrEmpty(siteMigrationJobErrorData))
                    {
                        LogHelper.LogInformation(string.Format("Success status update for list item - ID: {0}", siteMigrationRequest.ListItemId), LogEventID.InformationWrite);

                        this.UpdateRequestStatusAndSendEmailNotification(siteMigrationRequest, SiteMigrationRequestStatus.Success.ToString(), siteMigrationJobErrorData, MigrationConstants.SiteMigrationSuccessEmailSubjectKey, MigrationConstants.SiteMigrationSuccessEmailBodyKey);                      
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format("Failed status update for list item - ID: {0}", siteMigrationRequest.ListItemId), LogEventID.InformationWrite);

                        this.UpdateRequestStatusAndSendEmailNotification(siteMigrationRequest, SiteMigrationRequestStatus.Failed.ToString(), siteMigrationJobErrorData, MigrationConstants.SiteMigrationFailedEmailSubjectKey, MigrationConstants.SiteMigrationFailedEmailBodyKey);
                    }
                }
                catch (Exception error)
                {
                    LogHelper.LogError(error, LogEventID.InformationWrite, null);
                }
            }
        }

        /// <summary>
        /// Updates the request status and send email notification.
        /// </summary>
        /// <param name="siteMigrationRequest">The site migration request.</param>
        /// <param name="status">The status.</param>
        /// <param name="siteMigrationJobErrorData">The site migration job error data.</param>
        /// <param name="emailSubject">The email subject.</param>
        /// <param name="emailBody">The email body.</param>
        private void UpdateRequestStatusAndSendEmailNotification(SiteMigrationRequest siteMigrationRequest, string status, string siteMigrationJobErrorData, string emailSubject, string emailBody)
        {
            SiteMigrationJobHelper.UpdateSiteMigrationRequestStatus(siteMigrationRequest, status, siteMigrationJobErrorData, GlobalData.MigrationRequestListTitle);

            if (GlobalData.SharePointOnPremKey)
            {
                this.onPremiseSiteMigrationJobHelper.SendEmailNotification(siteMigrationRequest, status, emailSubject, emailBody);
            }
            else
            {
                this.onlineSiteMigrationJobHelper.SendEmailNotification(siteMigrationRequest, status, emailSubject, emailBody);
            }
        }

        /// <summary>
        /// Handles the fault response message.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="ex">Exception instance.</param>
        /// <param name="siteMigrationRequest">SiteMigrationRequest object instance.</param>
        /// <param name="siteMigrationJobErrorData">Site Migration Job error Details.</param>
        private void HandleFaultResponseMessage(SiteMigrationRequestMessage requestMessage, Exception ex, SiteMigrationRequest siteMigrationRequest, string siteMigrationJobErrorData)
        {
            LogHelper.LogInformation("Constructing fault response message...", LogEventID.InformationWrite);
            
            if (siteMigrationRequest.ListItemId > 0)
            {
                LogHelper.LogInformation(string.Format("Fault exception for Site migration request list item - ID: {0}", siteMigrationRequest.ListItemId), LogEventID.InformationWrite);

                try
                {
                    LogHelper.LogInformation(string.Format("Fault status update for list item - ID: {0}", siteMigrationRequest.ListItemId), LogEventID.InformationWrite);

                    this.UpdateRequestStatusAndSendEmailNotification(siteMigrationRequest, SiteMigrationRequestStatus.Failed.ToString(), siteMigrationJobErrorData, MigrationConstants.SiteMigrationFailedEmailSubjectKey, MigrationConstants.SiteMigrationFailedEmailBodyKey);
                }
                catch (Exception error)
                {
                    LogHelper.LogError(error, LogEventID.InformationWrite, ex.Message);
                }
            }
        }
        #endregion
    }
}
