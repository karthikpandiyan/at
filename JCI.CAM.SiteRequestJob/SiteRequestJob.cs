// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteRequestJob.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   imer Job implementation to store the Site Request Job to the Azure Queue for processing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteRequest.Job
{
    using System;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.Data;

    /// <summary>
    /// Timer Job implementation to store the Site Request Job to the Azure Queue for processing by the 
    /// Framework.Provisioning.Job Timer Job.
    /// TODO Docs on V2 which will use Timer Job Framework
    /// </summary>
    public class SiteRequestJob
    {
        #region Instance Members
        /// <summary>
        /// Site request factory instance
        /// </summary>
        private ISiteRequestFactory requestFactory;

        /// <summary>
        /// Configuration factory instance
        /// </summary>
        private IConfigurationFactory configFactory;

        /// <summary>
        /// Approved site request
        /// </summary>
        private EventHandler<SiteRequestEventArgs> approvedSiteRequest;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteRequestJob"/> class.
        /// </summary>
        public SiteRequestJob()
        {
            this.requestFactory = SiteRequestFactory.GetInstance();

            this.configFactory = ConfigurationFactoryManager.GetInstance();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the approved site request.
        /// </summary>
        /// <value>
        /// The approved site request.
        /// </value>
        public EventHandler<SiteRequestEventArgs> ApprovedSiteRequest
        {
            get
            {
                return this.approvedSiteRequest;
            }

            set
            {
                this.approvedSiteRequest = value;
            }
        }

        /// <summary>
        /// Gets or sets ISiteRequestFactory instance.
        /// </summary>
        /// <value>
        /// The request factory.
        /// </value>
        public ISiteRequestFactory RequestFactory
        {
            get
            {
                return this.requestFactory;
            }

            set
            {
                this.requestFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets IConfigurationFactory instance.
        /// </summary>
        /// <value>
        /// The configuration factory.
        /// </value>
        public IConfigurationFactory ConfigFactory
        {
            get
            {
                return this.configFactory;
            }

            set
            {
                this.configFactory = value;
            }
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Used to Process Approved Site Requests in the Site Repository Data store.
        /// </summary>
        public void ProcessSiteRequests()
        {
            LogHelper.LogInformation("JCI.CAM.SiteRequest.Job.SiteRequestJob.ProcessSiteRequests - Checking for Approved site requests", LogEventID.InformationWrite);
            var siteRequestManager = this.requestFactory.GetSiteRequestManager();            
            var appSettingsManager = this.configFactory.GetAppSetingsManager();
            var appSettings = appSettingsManager.GetAppSettings();
            var siteRequests = siteRequestManager.GetApprovedRequests(appSettings);
            SiteRequestHandler handler = new SiteRequestHandler(this);
            foreach (var siteRequest in siteRequests)
            {
                this.OnNewApprovedRequest(new SiteRequestEventArgs(siteRequest, siteRequestManager, appSettings));
            }

            LogHelper.LogInformation(string.Format("JCI.CAM.SiteRequest.Job.SiteRequestJob.ProcessSiteRequests -There are {0} site requests approved.", siteRequests.Count), LogEventID.InformationWrite);
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// Event Handler
        /// </summary>
        /// <param name="e">The <see cref="SiteRequestEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNewApprovedRequest(SiteRequestEventArgs e)
        {
            EventHandler<SiteRequestEventArgs> handler = this.ApprovedSiteRequest;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
