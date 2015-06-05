// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalData.cs" company="Microsoft Corporation &amp; Toyota">
//   Copyright (c) Microsoft Corporation and Toyota
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.PersonalSitesTransformationJob
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents Global information
    /// </summary>
    public static class GlobalData
    {
        /// <summary>
        /// Gets my site tenant admin URL.
        /// </summary>
        /// <value>
        /// My site tenant admin URL.
        /// </value>
        public static string MySiteTenantAdminUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["MySiteTenantAdminUrl"] != null)
                {
                    return ConfigurationManager.AppSettings["MySiteTenantAdminUrl"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to send the email notification regarding personal site transformation status or not.
        /// </summary>
        public static bool SendEmailNotification
        {
            get
            {
                if (ConfigurationManager.AppSettings["SendEmailNotification"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailNotification"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the connection string for personal sites transformation table name
        /// </summary>
        public static string PersonalSitesTransformationTableName
        {
            get
            {
                if (ConfigurationManager.AppSettings["PersonalSitesTransformationTableName"] != null)
                {
                    return ConfigurationManager.AppSettings["PersonalSitesTransformationTableName"];
                }

                return null;
            }
        }        

        /// <summary>
        /// Gets a value indicating whether [share point on premise key].
        /// </summary>
        /// <value>
        /// <c>true</c> if [share point on premise key]; otherwise, <c>false</c>.
        /// </value>
        public static bool SharePointOnPremKey
        {
            get
            {
                if (ConfigurationManager.AppSettings["SharePointOnPremises"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["SharePointOnPremises"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the app site template
        /// </summary>
        public static string AppSiteTemplate
        {
            get
            {
                if (ConfigurationManager.AppSettings["AppSiteTemplate"] != null)
                {
                    return ConfigurationManager.AppSettings["AppSiteTemplate"];
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