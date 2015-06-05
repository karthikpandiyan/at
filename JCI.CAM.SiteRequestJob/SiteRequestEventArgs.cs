// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteRequestEventArgs.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteRequest.Job
{   
    using System;
    using JCI.CAM.Provisioning.Core;
    using JCI.CAM.Provisioning.Core.Data;

    /// <summary>
    /// SiteRequest Event Class
    /// </summary>
    public class SiteRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteRequestEventArgs" /> class.
        /// </summary>
        /// <param name="siteRequest">Site request information.</param>
        /// <param name="siteRequestManager">The site request manager.</param>
        /// <param name="appSettings">The application settings.</param>
        public SiteRequestEventArgs(SiteRequestInformation siteRequest, ISiteRequestManager siteRequestManager, AppSettings appSettings)
        {
            this.SiteRequest = siteRequest;
            this.SiteRequestManager = siteRequestManager;
            this.AppSettings = appSettings;
        }

        /// <summary>
        /// Gets or sets Site Request Information
        /// </summary>
        public SiteRequestInformation SiteRequest { get; set; }

        /// <summary>
        /// Gets or sets the site request manager.
        /// </summary>
        /// <value>
        /// The site request manager.
        /// </value>
        public ISiteRequestManager SiteRequestManager { get; set; }

        /// <summary>
        /// Gets or sets the application settings.
        /// </summary>
        /// <value>
        /// The application settings.
        /// </value>
        public AppSettings AppSettings { get; set; }
    }
}
