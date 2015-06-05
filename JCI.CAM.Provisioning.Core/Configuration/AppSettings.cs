// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  omain Model for Application Settings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{
    /// <summary>
    /// Domain Model for Application Settings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets Tenant Administration Site.
        /// </summary>
        public string TenantAdminUrl { get; internal set; }

        /// <summary>
        /// Gets SharePoint Site that is hosting the Application
        /// </summary>
        public string SPHostUrl { get; internal set; }

        /// <summary>
        /// Gets the Client ID
        /// </summary>
        public string ClientID { get; internal set; }

        /// <summary>
        /// Gets Client Secret
        /// </summary>
        public string ClientSecret { get; internal set; }    
  
        /// <summary>
        /// Gets Support Team Email used for notifications
        /// </summary>
        public string SupportEmailNotification { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether [automatic approve]. Configuration option to Auto Approve Site Requests. If you use workflows to approve site creation this should be set to false
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic approve]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoApprove { get; internal set; }
        
        /// <summary>
        /// Gets a value indicating whether [SharePoint on premises].
        /// </summary>
        /// <value>
        /// <c>true</c> if [SharePoint on premises]; otherwise, <c>false</c>.
        /// </value>
        public bool SharePointOnPremises { get; internal set; }
        
        /// <summary>
        /// Gets the tenant admin account.
        /// </summary>
        /// <value>
        /// The tenant admin account.
        /// </value>
        public string TenantAdminAccount { get; internal set; }
        
        /// <summary>
        /// Gets the tenant admin account password. TODO- to be encrypted
        /// </summary>
        /// <value>
        /// The tenant admin account password.
        /// </value>
        public string TenantAdminAccountPwd { get; internal set; }

        /// <summary>
        /// Gets or sets the my site tenant admin URL.
        /// </summary>
        /// <value>
        /// The My Site tenant admin URL.
        /// </value>
        public string MySiteTenantAdminUrl { get; set; }
        
        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        /// <value>
        /// The repository manager.
        /// </value>
        public string RepositoryManager { get; internal set; }
    }
}
