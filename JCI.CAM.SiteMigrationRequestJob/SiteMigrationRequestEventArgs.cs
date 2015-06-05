// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteMigrationRequestEventArgs.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   site migration request event args
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteMigrationRequestJob
{
    using System;
    using JCI.CAM.Migration.Common.Entity;

    /// <summary>
    /// SiteRequest Event Class
    /// </summary>
    public class SiteMigrationRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMigrationRequestEventArgs" /> class.
        /// </summary>
        /// <param name="siteMigrationRequest">site migration request information.</param>
        public SiteMigrationRequestEventArgs(SiteMigrationRequest siteMigrationRequest)
        {
            this.SiteMigrationRequest = siteMigrationRequest;
        }

        /// <summary>
        /// Gets or sets site migration request Information
        /// </summary>
        public SiteMigrationRequest SiteMigrationRequest { get; set; }
    }
}
