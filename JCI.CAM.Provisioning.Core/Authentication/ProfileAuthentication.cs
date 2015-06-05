// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileAuthentication.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Authentication Class used for working with UserProfiles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Authentication
{    
    using System;
    using System.Net;
    using System.Security;
    using JCI.CAM.Provisioning.Core.Configuration;
    using Microsoft.SharePoint.Client;
    using JCI.CAM.Common.Logging;

    /// <summary>
    /// Authentication Class used for working with UserProfiles.
    /// The Class should use a Service Account
    /// </summary>
    public class ProfileAuthentication : IAuthentication
    {   
        #region Instance Members
       
        /// <summary>
        /// Configuration manager instance
        /// </summary>
        private static readonly IConfigurationFactory ConfigManager = ConfigurationFactoryManager.GetInstance();

        /// <summary>
        /// Application settings manager Instance
        /// </summary>
        private static readonly IAppSettingsManager AppManager = ConfigManager.GetAppSetingsManager();

        /// <summary>
        /// App settings
        /// </summary>
        private AppSettings settings;

        /// <summary>
        /// Tenant admin account
        /// </summary>
        private string tenantAdminAccount;

        /// <summary>
        /// Tenant admin account password
        /// </summary>
        private string tenantAdminAccountPassword;

        /// <summary>
        /// Tenant admin URL
        /// </summary>
        private string tenantAdminUrl;

        /// <summary>
        /// The my site admin URL
        /// </summary>
        private string mysiteAdminUrl;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileAuthentication"/> class.
        /// </summary>
        public ProfileAuthentication()
        {
            this.settings = AppManager.GetAppSettings();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Account with Tenant Admin Permissions
        /// By Default this will read from the TenantAdminAccount in your config file of your solution.
        /// </summary>
        /// <value>
        /// The tenant admin account.
        /// </value>
        public string TenantAdminAccount
        {
            get
            {
                this.tenantAdminAccount = string.IsNullOrEmpty(this.tenantAdminAccount) ? this.settings.TenantAdminAccount : this.tenantAdminAccount;
                return this.tenantAdminAccount;
            }

            set
            {
                this.tenantAdminAccount = value;
            }
        }

        /// <summary>
        /// Gets or sets Account with Tenant Admin Permissions
        /// By Default this will read from the TenantAdminAccountPassword in your config file of your solution.
        /// </summary>
        /// <value>
        /// The tenant admin account password.
        /// </value>
        public string TenantAdminAccountPassword
        {
            get
            {
                this.tenantAdminAccountPassword = string.IsNullOrEmpty(this.tenantAdminAccountPassword) ? this.settings.TenantAdminAccountPwd : this.tenantAdminAccountPassword;
                return this.tenantAdminAccountPassword;
            }

            set
            {
                this.tenantAdminAccountPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets Tenant Admin Url for your environment
        /// </summary>
        public string TenantAdminUrl
        {
            get
            {
                this.tenantAdminUrl = string.IsNullOrEmpty(this.tenantAdminUrl) ? this.settings.TenantAdminUrl : this.tenantAdminUrl;
                return this.tenantAdminUrl;
            }

            set
            {
                this.mysiteAdminUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets My Site Tenant Url
        /// </summary>
        public string MysiteTenantAdminUrl
        {
            get
            {
                this.mysiteAdminUrl = string.IsNullOrEmpty(this.mysiteAdminUrl) ? this.settings.MySiteTenantAdminUrl : this.mysiteAdminUrl;
                return this.mysiteAdminUrl;
            }

            set
            {
                this.tenantAdminUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets Credentials for working with the My Site Tenant Url
        /// </summary>
        public ICredentials Credentials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Site Url
        /// </summary>
        public string SiteUrl
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Returns an Authenticated ClientContext
        /// </summary>
        /// <returns>
        /// Authenticated ClientContext
        /// </returns>
        public ClientContext GetAuthenticatedContext()
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Authentication.ProfileAuthentication.GetAuthenticatedContext -  Initializing Authentication context with provided credentials", LogEventID.InformationWrite);
            this.EnsureCredentials();
            var ctx = new ClientContext(this.MysiteTenantAdminUrl);
            ctx.Credentials = this.Credentials;
            return ctx;
        }

        /// <summary>
        /// Ensure that the Credentials are set.
        /// By default this will read the SharePointOnPremises section in the application config.
        /// If SharePointOnPremises is True NetworkCredential is used, if SharePointOnPremises is false, this method will use
        /// SharePointOnlineCredentials.
        /// </summary>
        private void EnsureCredentials()
        {
            if (this.Credentials == null)
            {
                // check if Onprem if so use networkcreds of not use SharePoint Online Creds
                if (this.settings.SharePointOnPremises)
                {
                    NetworkCredential credentials = new NetworkCredential(this.TenantAdminAccount, this.TenantAdminAccountPassword);
                    this.Credentials = credentials;
                }
                else
                {
                    SecureString passWord = new SecureString();
                    foreach (char c in this.TenantAdminAccountPassword.ToCharArray())
                    {
                        passWord.AppendChar(c);
                    }

                    SharePointOnlineCredentials credentials = new SharePointOnlineCredentials(this.TenantAdminAccount, passWord);
                    this.Credentials = credentials;
                }
            }
        }      
    }
}
