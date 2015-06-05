//-----------------------------------------------------------------------
// <copyright file= "SiteMigrationRequest.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Migration.Common.Entity
{    
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using JCI.CAM.Provisioning.Core;

    /// <summary>
    /// Entity class for Site Migration Request
    /// </summary>
    [Serializable]
    [DataContract]
    public class SiteMigrationRequest
    {
        /// <summary>
        /// Gets or sets the site title
        /// </summary>
        [DataMember]
        public string SiteTitle { get; set; }

        /// <summary>
        /// Gets or sets the site owners
        /// </summary>
        [DataMember]
        public List<SharePointUser> SiteOwners { get; set; }

        /// <summary>
        /// Gets or sets the site URL 
        /// </summary>
        [DataMember]
        public string SiteURL { get; set; }

        /// <summary>
        /// Gets or sets the site type
        /// </summary>
        [DataMember]
        public string SiteType { get; set; }

        /// <summary>
        /// Gets or sets the list item identifier.
        /// </summary>
        /// <value>
        /// The list item identifier.
        /// </value>
        public int ListItemId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the site migration request message.
        /// </summary>
        public string SiteMigrationRequestMessage
        {
            get;
            set;
        }
    }
}