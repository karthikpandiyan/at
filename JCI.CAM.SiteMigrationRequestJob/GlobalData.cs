// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalData.cs" company="Microsoft Corporation &amp; Toyota">
//   Copyright (c) Microsoft Corporation and Toyota
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationRequestJob
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents Global information
    /// </summary>
    public static class GlobalData
    {
        /// <summary>
        /// Gets a value indicating whether to send the email notification regarding site migration request or not.
        /// </summary>
        public static bool SendEmailNotificationForSiteMigrationRequest
        {
            get
            {
                if (ConfigurationManager.AppSettings["SendEmailNotificationForSiteMigrationRequest"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailNotificationForSiteMigrationRequest"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the url of site on which create/delete operations to be carried
        /// </summary>
        public static string ContentTypeHubSiteUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["ContentTypeHubSiteUrl"] != null)
                {
                    return ConfigurationManager.AppSettings["ContentTypeHubSiteUrl"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the global configuration list title
        /// </summary>
        public static string GlobalConfigurationListName
        {
            get
            {
                if (ConfigurationManager.AppSettings["GlobalConfigurationListName"] != null)
                {
                    return ConfigurationManager.AppSettings["GlobalConfigurationListName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the Migration Request Site Url
        /// </summary>
        public static string MigrationRequestSiteUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["MigrationRequestSiteUrl"] != null)
                {
                    return ConfigurationManager.AppSettings["MigrationRequestSiteUrl"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the migration list definition title.
        /// </summary>
        public static string MigrationRequestListTitle
        {
            get
            {
                if (ConfigurationManager.AppSettings["MigrationRequestListTitle"] != null)
                {
                    return ConfigurationManager.AppSettings["MigrationRequestListTitle"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the site provisioning site URL.
        /// </summary>
        /// <value>
        /// The site provisioning site URL.
        /// </value>
        public static string SiteProvisioningSiteUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteProvisioningSiteUrl"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteProvisioningSiteUrl"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the workflow configuration list.
        /// </summary>
        /// <value>
        /// The name of the workflow configuration list.
        /// </value>
        public static string WorkflowConfigurationListName
        {
            get
            {
                if (ConfigurationManager.AppSettings["WorkflowConfigurationListName"] != null)
                {
                    return ConfigurationManager.AppSettings["WorkflowConfigurationListName"];
                }

                return null;
            }
        }
    }
}