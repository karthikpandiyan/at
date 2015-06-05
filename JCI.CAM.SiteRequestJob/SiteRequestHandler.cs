// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteRequestHandler.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SiteRequestHandler that is the subscriber to handle saving SiteRequest to the Azure Queue
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteRequest.Job
{
    using System;
    using System.Configuration;
    using JCI.Azure.Framework.Provisioning;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core;

    /// <summary>
    /// SiteRequestHandler that is the subscriber to handle saving SiteRequest to the Azure Queue
    /// </summary>
    public class SiteRequestHandler
    {
        /// <summary>
        /// Azure Connection Key
        /// </summary>
        public const string AzureConnectionKey = "ServiceBus.Connection";

        /// <summary>
        /// Request Name Key
        /// </summary>
        public const string RequestNameKey = "ServiceBus.RequestQueue";

        /// <summary>
        /// The azure service manager
        /// </summary>
        private readonly ServiceBusManager azureServiceManager = new ServiceBusManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteRequestHandler"/> class.
        /// </summary>
        /// <param name="publisher">Site Request Job</param>
        public SiteRequestHandler(SiteRequestJob publisher)
        {
            publisher.ApprovedSiteRequest += this.HandleNewSiteRequest;
        }

        /// <summary>
        /// Handles the new site request.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The <see cref="SiteRequestEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.ArgumentException">Throws exception if AzureConnectionKey or RequestQueueName are invalid
        /// </exception>
        public void HandleNewSiteRequest(object sender, SiteRequestEventArgs e)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.SiteRequestHandler.HandleNewSiteRequest - Processing site request item", LogEventID.InformationWrite);
            var siteRequest = e.SiteRequest;
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.SiteRequestHandler.HandleNewSiteRequest - Serializing SiteRequestInformation object", LogEventID.InformationWrite);
            var payload = XmlSerializerHelper.Serialize<SiteRequestInformation>(siteRequest);
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.SiteRequestHandler.HandleNewSiteRequest -Initializing Service Manager and getting Azure connection string and Queue details", LogEventID.InformationWrite);

            this.azureServiceManager.AzureConnectionString = ConfigurationManager.AppSettings[AzureConnectionKey];
            this.azureServiceManager.RequestQueueName = ConfigurationManager.AppSettings[RequestNameKey];

            if (string.IsNullOrEmpty(this.azureServiceManager.AzureConnectionString))
            {
                throw new ArgumentException(
                    string.Format("Azure Configuration {0} is missing in the config file", AzureConnectionKey));
            }

            if (string.IsNullOrEmpty(this.azureServiceManager.RequestQueueName))
            {
                throw new ArgumentException(
                   string.Format("Azure Configuration {0} is missing in the config file", RequestNameKey));
            }

            try
            {
                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.SiteRequestHandler.HandleNewSiteRequest - Save message to queue", LogEventID.InformationWrite);
                ProvisioningRequestMessage message = new ProvisioningRequestMessage { SiteRequest = payload };
                this.azureServiceManager.SendProvisioningRequest(message);
                var requestManager = e.SiteRequestManager;
                AppSettings appSettings = e.AppSettings;
                requestManager.UpdateRequestStatus(siteRequest.ListItemId, SiteRequestStatus.Pending, string.Empty, appSettings.SiteRequestListName);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                throw;
            }
        }
    }
}
