//-----------------------------------------------------------------------
// <copyright file= "SiteEntity.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Entity
{
    using System;

    /// <summary>
    /// SiteEntity class describes the information for a SharePoint site (collection)
    /// </summary>
    public class SiteEntity
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets site owner
        /// </summary>
        /// <value>
        /// The site owner login.
        /// </value>
        public string SiteOwnerLogin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current resource usage.
        /// </summary>
        /// <value>
        /// The current resource usage.
        /// </value>
        public double CurrentResourceUsage
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the LCID.
        /// </summary>
        /// <value>
        /// The LCID.
        /// </value>
        public uint Lcid
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the storage maximum level.
        /// </summary>
        /// <value>
        /// The storage maximum level.
        /// </value>
        public long StorageMaximumLevel
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the storage usage.
        /// </summary>
        /// <value>
        /// The storage usage.
        /// </value>
        public long StorageUsage
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the storage warning level.
        /// </summary>
        /// <value>
        /// The storage warning level.
        /// </value>
        public long StorageWarningLevel
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the last content modified date.
        /// </summary>
        /// <value>
        /// The last content modified date.
        /// </value>
        public DateTime LastContentModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        public string Template
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time zone identifier.
        /// </summary>
        /// <value>
        /// The time zone identifier.
        /// </value>
        public int TimeZoneId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user code quota in points
        /// </summary>
        public double UserCodeMaximumLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user code quota warning level in points
        /// </summary>
        /// <value>
        /// The user code warning level.
        /// </value>
        public double UserCodeWarningLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets count of the SPWeb objects in the site collection
        /// </summary>
        /// <value>
        /// The webs count.
        /// </value>
        public int WebsCount
        {
            get;
            set;
        }
    }
}
