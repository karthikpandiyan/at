// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalData.cs" company="Microsoft Corporation &amp; Toyota">
//   Copyright (c) Microsoft Corporation and Toyota
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationJob
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents Global information
    /// </summary>
    public static class GlobalData
    {
        /// <summary>
        /// Gets the environment
        /// </summary>
        public static string Environment
        {
            get
            {
                if (ConfigurationManager.AppSettings["Environment"] != null)
                {
                    return ConfigurationManager.AppSettings["Environment"].ToUpperInvariant();
                }
                else
                {
                    return "ONPREMISE";
                }
            }
        }

        /// <summary>
        /// Gets or sets the domain provided by user
        /// </summary>
        public static string ProvidedDomain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password provided by user
        /// </summary>
        public static System.Security.SecureString ProvidedPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user name provided by user
        /// </summary>
        public static string ProvidedUserName
        {
            get;
            set;
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
        /// Gets a value indicating whether to send the email notification regarding site migration status or not.
        /// </summary>
        public static bool SendEmailNotificationForSiteMigrationStatus
        {
            get
            {
                if (ConfigurationManager.AppSettings["SendEmailNotificationForSiteMigrationStatus"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailNotificationForSiteMigrationStatus"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run the test site banner injection operation or not.
        /// </summary>
        public static bool RunTestSiteBannerInjectionOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["RunTestSiteBannerInjectionOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["RunTestSiteBannerInjectionOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the test site banner feature name.
        /// </summary>
        public static string TestSiteBannerFeatureName
        {
            get
            {
                if (ConfigurationManager.AppSettings["TestSiteBannerFeatureName"] != null)
                {
                    return ConfigurationManager.AppSettings["TestSiteBannerFeatureName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the test site banner feature ID.
        /// </summary>
        public static string TestSiteBannerFeatureID
        {
            get
            {
                if (ConfigurationManager.AppSettings["TestSiteBannerFeatureID"] != null)
                {
                    return ConfigurationManager.AppSettings["TestSiteBannerFeatureID"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run the feature deactivation operation or not.
        /// </summary>
        public static bool RunDeactiveFeaturesOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["RunDeactiveFeaturesOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["RunDeactiveFeaturesOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run the header footer injection operation or not.
        /// </summary>
        public static bool RunHeaderFooterInjectionOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["RunHeaderFooterInjectionOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["RunHeaderFooterInjectionOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run the sub site injection operation or not.
        /// </summary>
        public static bool RunSubSiteInjectionOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["RunSubSiteInjectionOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["RunSubSiteInjectionOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run the SharePoint designer operation or not.
        /// </summary>
        public static bool RunSPDOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["RunSPDOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["RunSPDOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run applying themes operation or not.
        /// </summary>
        public static bool RunApplyingThemesOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["RunApplyingThemesOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["RunApplyingThemesOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run the adding event receiver to list operation or not.
        /// </summary>
        public static bool RunAddingEventReceiverToListOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["RunAddingEventReceiverToListOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["RunAddingEventReceiverToListOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the value to determine whether to run the adding event receiver to Shared Document Library operation or not.
        /// </summary>
        public static string AddEventReceiversToSharedDocumentLibrary
        {
            get
            {
                if (ConfigurationManager.AppSettings["AddEventReceiversToSharedDocumentLibrary"] != null)
                {
                    return ConfigurationManager.AppSettings["AddEventReceiversToSharedDocumentLibrary"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to run the adding sandbox solution to the site operation or not.
        /// </summary>
        public static bool AddSandBoxSolutionOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["AddSandBoxSolutionOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["AddSandBoxSolutionOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the sandbox solution file path.
        /// </summary>
        public static string SandBoxSolutionFilePath
        {
            get
            {
                if (ConfigurationManager.AppSettings["SandBoxSolutionFilePath"] != null)
                {
                    return ConfigurationManager.AppSettings["SandBoxSolutionFilePath"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to update the existing custom publishing page layout to default blank layout or not.
        /// </summary>
        public static bool UpdatePageLayoutOperation
        {
            get
            {
                if (ConfigurationManager.AppSettings["UpdatePageLayoutOperation"] != null)
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["UpdatePageLayoutOperation"]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the SPD script block.
        /// </summary>
        public static string SPDScriptBlock
        {
            get
            {
                if (ConfigurationManager.AppSettings["SPDScriptBlock"] != null)
                {
                    return ConfigurationManager.AppSettings["SPDScriptBlock"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the test banner script block.
        /// </summary>
        public static string TestSiteBannerScriptBlock
        {
            get
            {
                if (ConfigurationManager.AppSettings["TestSiteBannerScriptBlock"] != null)
                {
                    return ConfigurationManager.AppSettings["TestSiteBannerScriptBlock"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the header and footer script block.
        /// </summary>
        public static string HeaderFooterScriptBlock
        {
            get
            {
                if (ConfigurationManager.AppSettings["HeaderFooterScriptBlock"] != null)
                {
                    return ConfigurationManager.AppSettings["HeaderFooterScriptBlock"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the feature details xml schema file path.
        /// </summary>
        public static string FeatureDetailsXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["FeatureDetailsXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["FeatureDetailsXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the themes package xml schema file path.
        /// </summary>
        public static string ThemesPackageXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["ThemesPackageXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["ThemesPackageXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the list event receiver xml schema file path.
        /// </summary>
        public static string ListEventReceiversXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["ListEventReceiversXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["ListEventReceiversXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the page layout details xml schema file path.
        /// </summary>
        public static string PageLayoutDetailsXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["PageLayoutDetailsXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["PageLayoutDetailsXmlLocation"];
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
        /// Gets the Success status value for migration.
        /// </summary>
        public static string MigrationSuccessStatus
        {
            get
            {
                if (ConfigurationManager.AppSettings["MigrationSuccessStatus"] != null)
                {
                    return ConfigurationManager.AppSettings["MigrationSuccessStatus"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the Failed status value for migration.
        /// </summary>
        public static string MigrationFailedStatus
        {
            get
            {
                if (ConfigurationManager.AppSettings["MigrationFailedStatus"] != null)
                {
                    return ConfigurationManager.AppSettings["MigrationFailedStatus"];
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