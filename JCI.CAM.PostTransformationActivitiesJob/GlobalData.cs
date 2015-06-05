// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalData.cs" company="Microsoft Corporation &amp; Toyota">
//   Copyright (c) Microsoft Corporation and Toyota
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.PostTransformationActivitiesJob
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents Global information
    /// </summary>
    public static class GlobalData
    {
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
        /// Gets the site migration request list title.
        /// </summary>
        /// <value>
        /// The site migration request list title.
        /// </value>
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
    }
}