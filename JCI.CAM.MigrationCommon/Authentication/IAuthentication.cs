// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthentication.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Interface that is used to implement Authentication Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Migration.Common.Authentication
{    
    using System.Net;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Interface that is used to implement Authentication Class
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Gets tenant admin Url for the environment.
        /// </summary>
        string TenantAdminUrl
        {
            get;
        }

        /// <summary>
        /// Gets or sets Site Url
        /// </summary>
        string SiteUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Returns am Authenticated ClientContext
        /// </summary>
        /// <returns>Authenticated ClientContext</returns>
        ClientContext GetAuthenticatedContext();

        /// <summary>
        /// Gets the authenticated context.
        /// </summary>
        /// <param name="siteUrl">The site URL.</param>
        /// <returns>Client context</returns>
        ClientContext GetAuthenticatedContext(string siteUrl);

        /// <summary>
        /// Gets the specific tenant authenticated context.
        /// </summary>
        /// <param name="tenantAdminUrl">The tenant admin URL.</param>
        /// <returns>Client Context</returns>
        ClientContext GetSpecificTenantAuthenticatedContext(string tenantAdminUrl);

        /// <summary>
        /// Gets the authenticated web request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Http Web Request</returns>
        HttpWebRequest GetAuthenticatedWebRequest(string url);
    }
}
