//-----------------------------------------------------------------------
// <copyright file="SessionSharePointContext.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) 2013 Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common
{
    using System;

    /// <summary>
    /// Encapsulates all the information from SharePoint.
    /// </summary>
    [Serializable]
    public class SessionSharePointContext
    {
        /// <summary>
        /// Gets or sets The context token.
        /// </summary>
        public string ContextTokenString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets The context token's "CacheKey" claim.
        /// </summary>
        public string CacheKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets The context token's "refresh token" claim.
        /// </summary>
        public string RefreshToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets The SP Host Url
        /// </summary>
        public Uri SPHostUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets The SP App Web Url
        /// </summary>
        public Uri SharePointAppWebUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets The SharePoint Language
        /// </summary>
        public string SharePointLanguage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets The SharePoint Client Tag
        /// </summary>
        public string SharePointClientTag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets The SharePoint Product Number
        /// </summary>
        public string SharePointProductNumber
        {
            get;
            set;
        }
    }
}