// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalData.cs" company="Microsoft Corporation &amp; Toyota">
//   Copyright (c) Microsoft Corporation and Toyota
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.ConsoleApp
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
                    return "OMPREMISE";
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
        /// Gets the name of Sandboxed List Definition WSP
        /// </summary>
        public static string SandboxedListDefinitionWSP
        {
            get
            {
                if (ConfigurationManager.AppSettings["SandboxedListDefinitionWSP"] != null)
                {
                    return ConfigurationManager.AppSettings["SandboxedListDefinitionWSP"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of Sandboxed List Definition WSP
        /// </summary>
        public static string SandboxedListDefinitionWSPGUID
        {
            get
            {
                if (ConfigurationManager.AppSettings["SandboxedListDefinitionWSPGUID"] != null)
                {
                    return ConfigurationManager.AppSettings["SandboxedListDefinitionWSPGUID"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the major version of Sandboxed List Definition WSP
        /// </summary>
        public static int SandboxedSolutionMajorVersion
        {
            get
            {
                if (ConfigurationManager.AppSettings["SandboxedSolutionMajorVersion"] != null)
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["SandboxedSolutionMajorVersion"]);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the minor version of Sandboxed List Definition WSP
        /// </summary>
        public static int SandboxedSolutionMinorVersion
        {
            get
            {
                if (ConfigurationManager.AppSettings["SandboxedSolutionMinorVersion"] != null)
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["SandboxedSolutionMinorVersion"]);
                }

                return 0;
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
        /// Gets the fields schema files path.
        /// </summary>
        public static string FieldsXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["FieldsXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["FieldsXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the theme schema files path.
        /// </summary>
        public static string ThemeXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["ThemeXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["ThemeXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the content types schema files path.
        /// </summary>
        public static string ContentTypeXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["ContentTypeXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["ContentTypeXmlLocation"];
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
        /// Gets the document templates files path.
        /// </summary>
        public static string DocumentTemplatesLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["DocumentTemplatesLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["DocumentTemplatesLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the document templates schema files path.
        /// </summary>
        public static string DocumentTemplatesXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["DocumentTemplatesXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["DocumentTemplatesXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the taxonomy fields mapping schema files path.
        /// </summary>
        public static string TaxonomyFieldsXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["TaxonomyFieldsXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["TaxonomyFieldsXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the site migration request list XML location.
        /// </summary>
        /// <value>
        /// The site migration request list XML location.
        /// </value>
        public static string SiteMigrationRequestListXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteMigrationRequestListXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteMigrationRequestListXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the site request list title.
        /// </summary>
        /// <value>
        /// The site request list title.
        /// </value>
        public static string SiteRequestListTitle
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestListTitle"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestListTitle"];
                }

                return null;
            }
        }        

        /// <summary>
        /// Gets the site migration request list title.
        /// </summary>
        /// <value>
        /// The site migration request list title.
        /// </value>
        public static string SiteMigrationRequestListName
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteMigrationRequestListName"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteMigrationRequestListName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the site request list indexed columns.
        /// </summary>
        /// <value>
        /// The site request list indexed columns.
        /// </value>
        public static string SiteRequestListIndexbleColumns
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestListIndexbleColumns"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestListIndexbleColumns"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the site request approval workflow file path.
        /// </summary>
        /// <value>
        /// The site request approval workflow file path.
        /// </value>
        public static string SiteRequestApprovalWorkflowFilePath
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestApprovalWorkflowFilePath"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestApprovalWorkflowFilePath"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the site request approval workflow definition.
        /// </summary>
        /// <value>
        /// The name of the site request approval workflow definition.
        /// </value>
        public static string SiteRequestApprovalWorkflowDefinitionName
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestApprovalWorkflowDefinitionName"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestApprovalWorkflowDefinitionName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the site request approval workflow subscription.
        /// </summary>
        /// <value>
        /// The name of the site request approval workflow subscription.
        /// </value>
        public static string SiteRequestApprovalWorkflowSubscriptionName
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestApprovalWorkflowSubscriptionName"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestApprovalWorkflowSubscriptionName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the site request update workflow file path.
        /// </summary>
        /// <value>
        /// The site request update workflow file path.
        /// </value>
        public static string SiteRequestUpdateWorkflowFilePath
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestUpdateWorkflowFilePath"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestUpdateWorkflowFilePath"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the site request update workflow definition.
        /// </summary>
        /// <value>
        /// The name of the site request update workflow definition.
        /// </value>
        public static string SiteRequestUpdateWorkflowDefinitionName
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestUpdateWorkflowDefinitionName"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestUpdateWorkflowDefinitionName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the site request update workflow subscription.
        /// </summary>
        /// <value>
        /// The name of the site request update workflow subscription.
        /// </value>
        public static string SiteRequestUpdateWorkflowSubscriptionName
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteRequestUpdateWorkflowSubscriptionName"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteRequestUpdateWorkflowSubscriptionName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the history list.
        /// </summary>
        /// <value>
        /// The name of the history list.
        /// </value>
        public static string HistoryListName
        {
            get
            {
                if (ConfigurationManager.AppSettings["HistoryListName"] != null)
                {
                    return ConfigurationManager.AppSettings["HistoryListName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the workflow task list.
        /// </summary>
        /// <value>
        /// The name of the workflow task list.
        /// </value>
        public static string WorkflowTaskListName
        {
            get
            {
                if (ConfigurationManager.AppSettings["WorkflowTaskListName"] != null)
                {
                    return ConfigurationManager.AppSettings["WorkflowTaskListName"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the site provisioning lists XML location.
        /// </summary>
        /// <value>
        /// The site provisioning lists XML location.
        /// </value>
        public static string SiteProvisioningListsXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteProvisioningListsXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["SiteProvisioningListsXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the list templates XML location.
        /// </summary>
        /// <value>
        /// The list templates XML location.
        /// </value>
        public static string ListTemplatesXmlLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings["ListTemplatesXmlLocation"] != null)
                {
                    return ConfigurationManager.AppSettings["ListTemplatesXmlLocation"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets WebPartFile
        /// </summary>
        public static string WebPartFile
        {
            get
            {
                if (ConfigurationManager.AppSettings["WebPartFile"] != null)
                {
                    return ConfigurationManager.AppSettings["WebPartFile"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets Display Form Page Relative Path
        /// </summary>
        public static string DispFormPageRelativePath
        {
            get
            {
                if (ConfigurationManager.AppSettings["DispFormPageRelativePath"] != null)
                {
                    return ConfigurationManager.AppSettings["DispFormPageRelativePath"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets WebPartTitle
        /// </summary>
        public static string WebPartTitle
        {
            get
            {
                if (ConfigurationManager.AppSettings["WebPartTitle"] != null)
                {
                    return ConfigurationManager.AppSettings["WebPartTitle"];
                }

                return null;
            }
        }
    }
}