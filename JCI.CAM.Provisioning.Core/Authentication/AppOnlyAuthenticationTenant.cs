// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppOnlyAuthenticationTenant.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Authenticated Class for using the Tenant API
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Authentication
{    
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml.Linq;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.TemplateEntites;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Authenticated Class for using the Tenant API
    /// </summary>
    public class AppOnlyAuthenticationTenant : IAuthentication
    {
        #region Instance Members
        /// <summary>
        /// The configuration manager
        /// </summary>
        private static readonly IConfigurationFactory ConfigManager = ConfigurationFactoryManager.GetInstance();

        /// <summary>
        /// The application settings manager
        /// </summary>
        private static readonly IAppSettingsManager AppSettingsManager = ConfigManager.GetAppSetingsManager();

        /// <summary>
        /// The available tenancies
        /// </summary>
        private static List<Tenancy> availableTenancies;
                
        /// <summary>
        /// The application identifier
        /// </summary>
        private string appID;

        /// <summary>
        /// The application secret
        /// </summary>
        private string appSecret;

        /// <summary>
        /// The tenant admin URL
        /// </summary>
        private string tenantAdminUrl;

        /// <summary>
        /// The realm
        /// </summary>
        private string realm;

        /// <summary>
        /// The site URL
        /// </summary>
        private string siteUrl;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the available tenancies.
        /// </summary>
        /// <value>
        /// The available tenancies.
        /// </value>
        public static List<Tenancy> AvailableTenancies
        {
            get
            {
                try
                {
                    if (availableTenancies == null || availableTenancies.Any() == false)
                    {
                        LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Authentication.AppOnlyAuthenticationTenant.AvailableTenancies -  Loading tenancies configuration details", LogEventID.InformationWrite);
                        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings.Get("TenancyConfigurationFileLocation"));
                        var tenancyConfiguration = new TenancyConfiguration();
                        using (var result = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                        {
                            XDocument xmlDocument = XDocument.Load(result);
                            tenancyConfiguration = XmlSerializerHelper.Deserialize<TenancyConfiguration>(xmlDocument);
                        }

                        if (tenancyConfiguration.Tenancies != null && tenancyConfiguration.Tenancies.Count > 0)
                        {
                            availableTenancies = tenancyConfiguration.Tenancies;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex);
                }

                return availableTenancies;
            }
        }

        /// <summary>
        /// Gets or sets Site Url
        /// </summary>
        public string SiteUrl
        {
            get
            {
                this.siteUrl = string.IsNullOrEmpty(this.siteUrl) ? AppSettingsManager.GetAppSettings().TenantAdminUrl : this.siteUrl;
                return this.siteUrl;
            }

            set
            {
                this.siteUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets SharePoint Realm
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        public string Realm
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.realm))
                {
                    if (!string.IsNullOrWhiteSpace(this.TenantAdminUrl))
                    {
                        this.realm = TokenHelper.GetRealmFromTargetUrl(new Uri(this.TenantAdminUrl));
                    }
                }

                return this.realm;
            }

            set
            {
                this.realm = value;
            }
        }

        /// <summary>
        /// Gets or sets AppID that is registered
        /// By Default this will read from the ClientId in your config file of your solution.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        public string AppId
        {
            get
            {
                this.appID = string.IsNullOrEmpty(this.appID) ? AppSettingsManager.GetAppSettings().ClientID : this.appID;
                return this.appID;
            }

            set
            {
                this.appID = value;
            }
        }

        /// <summary>
        /// Gets or sets the application secret.
        /// </summary>
        /// <value>
        /// The application secret.
        /// </value>
        public string AppSecret
        {
            get
            {
                this.appSecret = string.IsNullOrEmpty(this.appSecret) ? AppSettingsManager.GetAppSettings().ClientSecret : this.appSecret;
                return this.appSecret;
            }

            set
            {
                this.appSecret = value;
            }
        }

        /// <summary>
        /// Gets or sets tenant admin Url for the environment.
        /// By Default this will read from the TenantAdminUrl in your config file of your solution.
        /// </summary>
        public string TenantAdminUrl
        {
            get
            {
                this.tenantAdminUrl = string.IsNullOrEmpty(this.tenantAdminUrl) ? AppSettingsManager.GetAppSettings().TenantAdminUrl : this.tenantAdminUrl;
                return this.tenantAdminUrl;
            }

            set
            {
                this.tenantAdminUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets Client secret or app secret that has been registered.
        /// By Default this will read from the ClientSecret in your config file of your solution.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        private string AccessToken
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Returns am Authenticated ClientContext
        /// </summary>
        /// <returns>
        /// Authenticated ClientContext
        /// </returns>
        public ClientContext GetAuthenticatedContext()
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Authentication.AppOnlyAuthenticationTenant.GetAuthenticatedContext -  Initializing AppOnly Authentication context", LogEventID.InformationWrite);
            this.EnsureToken();
            var ctx = TokenHelper.GetClientContextWithAccessToken(this.TenantAdminUrl, this.AccessToken);
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Authentication.AppOnlyAuthenticationTenant.GetAuthenticatedContext - Initialized AppOnly Authentication context", LogEventID.InformationWrite);
            return ctx;
        }

        /// <summary>
        /// Gets the authenticated context for given URL.
        /// </summary>
        /// <param name="siteUrl">The site URL.</param>
        /// <returns>Authenticated Client context</returns>
        /// <exception cref="System.Exception">Invalid tenant admin url or not found.
        /// or
        /// Invalid tenant admin url or not found.</exception>
        public ClientContext GetAuthenticatedContextForGivenUrl(string siteUrl)
        {
            List<Tenancy> tenancies = AppOnlyAuthenticationTenant.AvailableTenancies;
            string tenantAdminUrl = string.Empty;
            if (tenancies != null)
            {
                Tenancy tenancy = tenancies.FirstOrDefault(t => siteUrl.StartsWith(t.WebApplicationUrl, StringComparison.OrdinalIgnoreCase));
                if (tenancy == null || string.IsNullOrEmpty(tenancy.TenantAdminUrl))
                {
                    throw new Exception("Invalid tenant admin url or not found.");
                }

                tenantAdminUrl = tenancy.TenantAdminUrl;
            }
            else
            {
                throw new Exception("Invalid tenant admin url or not found.");
            }

            LogHelper.LogInformation(string.Format("Tenant Admin Url:{0}", tenantAdminUrl), LogEventID.InformationWrite);
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Authentication.AppOnlyAuthenticationTenant.GetAuthenticatedContext -  Initializing AppOnly Authentication context", LogEventID.InformationWrite);
            this.EnsureToken(tenantAdminUrl);
            var ctx = TokenHelper.GetClientContextWithAccessToken(tenantAdminUrl, this.AccessToken);
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Authentication.AppOnlyAuthenticationTenant.GetAuthenticatedContext - Initialized AppOnly Authentication context", LogEventID.InformationWrite);
            return ctx;
        }

        /// <summary>
        /// Gets the specific tenant authenticated context.
        /// </summary>
        /// <param name="tenantAdminUrl">The tenant admin URL.</param>
        /// <returns>Authenticated Client Context</returns>
        public ClientContext GetSpecificTenantAuthenticatedContext(string tenantAdminUrl)
        {
            LogHelper.LogInformation("Initializing AppOnly Authentication context...", LogEventID.InformationWrite);
            this.EnsureToken(tenantAdminUrl);
            var ctx = TokenHelper.GetClientContextWithAccessToken(tenantAdminUrl, this.AccessToken);
            LogHelper.LogInformation("Initialized AppOnly Authentication context...", LogEventID.InformationWrite);
            return ctx;
        }

        /// <summary>
        /// Method to Ensure that an OAUTH token is valid
        /// </summary>
        public void EnsureToken()
        {
            LogHelper.LogInformation("Valdiating Access token", LogEventID.InformationWrite);
            if (string.IsNullOrWhiteSpace(this.AccessToken))
            {
                var oauthResponse = TokenHelper.GetAppOnlyAccessToken(
                    TokenHelper.SharePointPrincipal,
                    new Uri(this.TenantAdminUrl).Authority,
                    this.Realm);

                this.AccessToken = oauthResponse.AccessToken;
            }
        }

        /// <summary>
        /// Ensures the token.
        /// </summary>
        /// <param name="siteUrl">The site URL.</param>
        public void EnsureToken(string siteUrl)
        {
            if (string.IsNullOrWhiteSpace(this.AccessToken))
            {
                var authResponse = TokenHelper.GetAppOnlyAccessToken(
                    TokenHelper.SharePointPrincipal,
                    new Uri(siteUrl).Authority,
                    this.Realm);

                this.AccessToken = authResponse.AccessToken;
            }
        }

        /// <summary>
        /// Gets the authenticated web request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// Http Web Request
        /// </returns>
        /// <exception cref="System.ArgumentException">site url</exception>
        public HttpWebRequest GetAuthenticatedWebRequest(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException(PCResources.Exception_Message_EmptyString_Arg, "url");
            }

            this.EnsureToken();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add("Authorization", "Bearer " + this.AccessToken);
            return request;
        }

        /// <summary>
        /// Gets the authenticated context.
        /// </summary>
        /// <param name="siteUrl">The site URL.</param>
        /// <returns>
        /// Client context
        /// </returns>
        public ClientContext GetAuthenticatedContext(string siteUrl)
        {
            this.EnsureToken(siteUrl);
            var context = TokenHelper.GetClientContextWithAccessToken(siteUrl.ToString(), this.AccessToken);
            return context;
        }
    }
}
