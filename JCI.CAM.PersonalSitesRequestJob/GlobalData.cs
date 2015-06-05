// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalData.cs" company="Microsoft Corporation &amp; Toyota">
//   Copyright (c) Microsoft Corporation and Toyota
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.PersonalSitesRequestJob
{
    using System.Configuration;

    /// <summary>
    /// Represents Global information
    /// </summary>
    public static class GlobalData
    {
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
        /// Gets the user profile web application Url
        /// </summary>
        public static string UserProfilesWebApplicationUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["UserProfilesWebApplicationUrl"] != null)
                {
                    return ConfigurationManager.AppSettings["UserProfilesWebApplicationUrl"];
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
        /// Gets the domain
        /// </summary>
        public static string Domain
        {
            get
            {
                if (ConfigurationManager.AppSettings["Domain"] != null)
                {
                    return ConfigurationManager.AppSettings["Domain"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the password
        /// </summary>
        public static string Password
        {
            get
            {
                if (ConfigurationManager.AppSettings["Password"] != null)
                {
                    return ConfigurationManager.AppSettings["Password"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the user name
        /// </summary>
        public static string UserName
        {
            get
            {
                if (ConfigurationManager.AppSettings["UserName"] != null)
                {
                    return ConfigurationManager.AppSettings["UserName"];
                }

                return null;
            }
        }
    }
}