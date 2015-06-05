// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteMigrationRequestHandler.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SiteMigrationRequestHandler that is the subscriber to handle saving SiteMigrationRequest to the Azure Queue
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationRequestJob
{
    using System;
    using System.Configuration;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common; 
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Provisioning.Core;
    using JCI.CAM.SiteMigrationRequestJob.Helpers;

    /// <summary>
    /// SiteRequestHandler that is the subscriber to handle saving SiteRequest to the Azure Queue
    /// </summary>
    public class SiteMigrationRequestHandler
    {
        /// <summary>
        /// The azure service manager
        /// </summary>
        private readonly MigrationServiceBusManager manager = new MigrationServiceBusManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMigrationRequestHandler"/> class.
        /// </summary>
        /// <param name="publisher">site migration request Job</param>
        public SiteMigrationRequestHandler(SiteMigrationRequestJobHelper publisher)
        {
            publisher.ApprovedSiteMigrationRequest += this.HandleNewSiteRequest;
        }

        /// <summary>
        /// Handles the new site migration request.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The <see cref="SiteRequestEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.ArgumentException">Throws exception if AzureConnectionKey or RequestQueueName are invalid
        /// </exception>
        public void HandleNewSiteRequest(object sender, SiteMigrationRequestEventArgs e)
        {
            LogHelper.LogInformation("Processing site migration request item", LogEventID.InformationWrite);
            var siteMigrationRequest = e.SiteMigrationRequest;
            LogHelper.LogInformation("Serializing SiteMigrationRequest object", LogEventID.InformationWrite);
            var payload = XmlSerializerHelper.Serialize<SiteMigrationRequest>(siteMigrationRequest);
            LogHelper.LogInformation("JCI.CAM.SiteMigrationRequestJob.SiteRequestHandler.HandleNewSiteRequest -Initializing Service Manager and getting Azure connection string and Queue details", LogEventID.InformationWrite);

            this.manager.AzureConnectionString = ConfigurationManager.AppSettings[MigrationConstants.AzureConnectionKey];
            this.manager.RequestQueueName = ConfigurationManager.AppSettings[MigrationConstants.RequestNameKey];

            if (string.IsNullOrEmpty(this.manager.AzureConnectionString))
            {
                throw new ArgumentException(
                    string.Format("Azure Configuration {0} is missing in the config file", MigrationConstants.AzureConnectionKey));
            }

            if (string.IsNullOrEmpty(this.manager.RequestQueueName))
            {
                throw new ArgumentException(
                   string.Format("Azure Configuration {0} is missing in the config file", MigrationConstants.RequestNameKey));
            }

            try
            {
                LogHelper.LogInformation("JCI.CAM.SiteMigrationRequestJob.SiteRequestHandler.HandleNewSiteRequest - Save message to queue", LogEventID.InformationWrite);
                SiteMigrationRequestMessage message = new SiteMigrationRequestMessage { SiteMigrationRequest = payload };
                this.manager.SendMigrationRequest(message);

                // Updating status
                SiteMigrationRequestJobHelper siteMigrationRequestJob = new SiteMigrationRequestJobHelper();
                siteMigrationRequestJob.UpdateRequestStatus(siteMigrationRequest.ListItemId, SiteMigrationRequestStatus.Pending, string.Empty, GlobalData.MigrationRequestListTitle.ToString());
                siteMigrationRequestJob.SendEmailNotification(siteMigrationRequest, Convert.ToString(SiteMigrationRequestStatus.Pending), MigrationConstants.SiteMigrationPendingEmailSubjectKey, MigrationConstants.SiteMigrationPendingEmailBodyKey);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                throw;
            }
        }
    }
}
