//-----------------------------------------------------------------------
// <copyright file= "PersonalSitesMigrationRequest.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
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
    using Microsoft.SharePoint.Client;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Entity class for Site Migration Request
    /// </summary>
    [Serializable]
    [DataContract]
    public class PersonalSitesMigrationRequest : TableEntity
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
        public string SiteOwner { get; set; }

        /// <summary>
        /// Gets or sets the site URL 
        /// </summary>
        [DataMember]
        public string SiteURL { get; set; }

        /// <summary>
        /// Gets or sets the status of site migration
        /// </summary>
        [DataMember]
        public string SiteMigrationStatus { get; set; }

        /// <summary>
        /// Gets or sets the site type
        /// </summary>
        [DataMember]
        public string SiteType { get; set; }

        /// <summary>
        /// Gets or sets the log
        /// </summary>
        [DataMember]
        public string Log { get; set; }
    }
}