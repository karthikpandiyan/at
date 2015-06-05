// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  omain Model for Application Settings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core
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
        /// Gets the content type hub URL.
        /// </summary>
        /// <value>
        /// The content type hub URL.
        /// </value>
        public string ContentTypeHubUrl { get; internal set; }

        /// <summary>
        /// Gets the name of the global configuration list.
        /// </summary>
        /// <value>
        /// The name of the global configuration list.
        /// </value>
        public string GlobalConfigurationListName { get; internal set; }
                
        /// <summary>
        /// Gets the name of the workflow configuration list.
        /// </summary>
        /// <value>
        /// The name of the workflow configuration list.
        /// </value>
        public string WorkflowConfigurationListName { get; internal set; }
        
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
        /// Gets the repository manager.
        /// </summary>
        /// <value>
        /// The repository manager.
        /// </value>
        public string RepositoryManager { get; internal set; }

        /// <summary>
        /// Gets the name of the site request list.
        /// </summary>
        /// <value>
        /// The name of the site request list.
        /// </value>
        public string SiteRequestListName { get; internal set; }

        /// <summary>
        /// Gets the name of the definition file.
        /// </summary>
        /// <value>
        /// The name of the configuration file.
        /// </value>
        public string TemplateDefinitionFileName { get; internal set; }

        /// <summary>
        /// Gets the name of the configuration file.
        /// </summary>
        /// <value>
        /// The name of the configuration file.
        /// </value>
        public string TemplateConfigurationFileName { get; internal set; }

        /// <summary>
        /// Gets the name of the work flow history list.
        /// </summary>
        /// <value>
        /// The name of the work flow history list.
        /// </value>
        public string WorkFlowHistoryListName { get; internal set; }

        /// <summary>
        /// Gets or sets the approval work flow.
        /// </summary>
        /// <value>
        /// The approval work flow.
        /// </value>
        public string ApprovalWorkFlow { get; set; }

        /// <summary>
        /// Gets or sets the update work flow.
        /// </summary>
        /// <value>
        /// The update work flow.
        /// </value>
        public string UpdateWorkFlow { get; set; }
    }
}
