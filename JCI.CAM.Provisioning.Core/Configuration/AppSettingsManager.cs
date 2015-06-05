// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Internal class that is a facade for getting values from the web config
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{  
    using JCI.CAM.Common.Logging;

    /// <summary>
    /// Internal class that is a facade for getting values from the web config. 
    /// Used for returning settings for the application.
    /// </summary>
    internal class AppSettingsManager : IAppSettingsManager
    {
        #region Instance Members
        /// <summary>
        /// The tenant admin URL key
        /// </summary>
        public const string TenantAdminUrlKey = "TenantAdminUrl";

        /// <summary>
        /// The tenant URL key
        /// </summary>
        public const string TenantUrlKey = "TenantUrl";

        /// <summary>
        /// The SP host URL key
        /// </summary>
        public const string SPHostUrlKey = "SPHost";

        /// <summary>
        /// The client identifier key
        /// </summary>
        public const string ClientIdKey = "ClientId";

        /// <summary>
        /// The client secret key
        /// </summary>
        public const string ClientSecretKey = "ClientSecret";

        /// <summary>
        /// The content type hub URL
        /// </summary>
        public const string ContentTypeHubUrl = "ContentTypeHubUrl";

        /// <summary>
        /// The global configuration list name
        /// </summary>
        public const string GlobalConfigurationListName = "GlobalConfigurationListName";

        /// <summary>
        /// The support team notification key
        /// </summary>
        public const string SupportTeamNotificationKey = "SupportTeamNotificationEmail";

        /// <summary>
        /// The automatic approve sites key
        /// </summary>
        public const string AutoApproveSitesKey = "AutoApproveSites";

        /// <summary>
        /// The repository manager type key
        /// </summary>
        public const string RepositoryManagerTypeKey = "RepositoryManagerType";

        /// <summary>
        /// The share point on premise key
        /// </summary>
        public const string SharePointOnPremKey = "SharePointOnPremises";

        /// <summary>
        /// The tenant admin account key
        /// </summary>
        public const string TenantAdminAccountKey = "TenantAdminAccount";

        /// <summary>
        /// The tenant admin account password key
        /// </summary>
        public const string TenantAdminAccountPasswordKey = "TenantAdminAccountPWD";

        /// <summary>
        /// My site tenant admin URL key
        /// </summary>
        public const string MySiteTenantAdminUrlKey = "MysiteTenantAdminUrl";

        /// <summary>
        /// The site request list name key
        /// </summary>
        public const string SiteRequestListNameKey = "SiteRequestListName";

        /// <summary>
        /// The configuration file name key
        /// </summary>
        public const string TemplateDefinitionFileName = "TemplateDefinitionFileName";

        /// <summary>
        /// The configuration file name key
        /// </summary>
        public const string TemplateConfigurationFileName = "TemplateConfigurationFileName";

        /// <summary>
        /// The work flow history list name key
        /// </summary>
        public const string WorkFlowHistoryListNameKey = "WorkFlowHistoryListName";

        /// <summary>
        /// The approval work flow
        /// </summary>
        public const string ApprovalWorkFlow = "ApprovalWorkFlow";

        /// <summary>
        /// The update work flow
        /// </summary>
        public const string UpdateWorkFlow = "UpdateWorkFlow";

        /// <summary>
        /// The workflow configuration list name key
        /// </summary>
        public const string WorkflowConfigurationListNameKey = "WorkflowConfigurationListName";

        /// <summary>
        /// AppSettingsManager instance
        /// </summary>
        private static readonly AppSettingsManager Instance = new AppSettingsManager();        
        #endregion
              
        #region IAppSettingsManager Members
        /// <summary>
        /// Returns an Instance of AppSettings
        /// </summary>
        /// <returns>
        /// App Settings
        /// </returns>
        public AppSettings GetAppSettings()
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Configuration.AppSettingsManager.GetAppSettings - getting app configuration values", LogEventID.InformationWrite);
            var appConfig = new AppSettings
            {
                TenantAdminUrl = ConfigurationHelper.Get(TenantAdminUrlKey),
                SPHostUrl = ConfigurationHelper.Get(SPHostUrlKey),
                ClientID = ConfigurationHelper.Get(ClientIdKey),
                ClientSecret = ConfigurationHelper.Get(ClientSecretKey),
                SupportEmailNotification = ConfigurationHelper.Get(SupportTeamNotificationKey),
                RepositoryManager = ConfigurationHelper.Get(RepositoryManagerTypeKey),
                SiteRequestListName = ConfigurationHelper.Get(SiteRequestListNameKey),
                TemplateDefinitionFileName = ConfigurationHelper.Get(TemplateDefinitionFileName),
                TemplateConfigurationFileName = ConfigurationHelper.Get(TemplateConfigurationFileName),
                WorkFlowHistoryListName = ConfigurationHelper.Get(WorkFlowHistoryListNameKey),
                ContentTypeHubUrl = ConfigurationHelper.Get(ContentTypeHubUrl),
                GlobalConfigurationListName = ConfigurationHelper.Get(GlobalConfigurationListName),
                ApprovalWorkFlow = ConfigurationHelper.Get(ApprovalWorkFlow),
                UpdateWorkFlow = ConfigurationHelper.Get(UpdateWorkFlow),
                WorkflowConfigurationListName = ConfigurationHelper.Get(WorkflowConfigurationListNameKey)
            };

            var autoApprove = ConfigurationHelper.Get(AutoApproveSitesKey);
            var sharePointOnPremises = ConfigurationHelper.Get(SharePointOnPremKey);

            bool result = false;
            if (bool.TryParse(autoApprove, out result))
            {
                appConfig.AutoApprove = result;
            }

            if (bool.TryParse(sharePointOnPremises, out result))
            {
                appConfig.SharePointOnPremises = result;
            }

            return appConfig;
        }
        #endregion

        #region Static Members
        /// <summary>
        /// Gets IAppSettingsManager instance.
        /// </summary>
        /// <returns>IAppSettingsManager object</returns>
        internal static IAppSettingsManager GetInstance()
        {
            return Instance;
        }
        #endregion
    }
}
