//-----------------------------------------------------------------------
// <copyright file="UserProfile.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// User Profile Model for profile properties
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Gets or sets contact name
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// Gets or sets phone
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets Manager
        /// </summary>
        public string ManangerName { get; set; }

        /// <summary>
        /// Gets or sets Department
        /// </summary>
        public string RequestorDepartmentField { get; set; }

        /// <summary>
        /// Gets or sets Facility Location
        /// </summary>
        public string RequestorFacilityField { get; set; }

        /// <summary>
        /// Gets or sets GlobalId
        /// </summary>
        public string RequestorGlobalIdField { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        /// Gets or sets Business Unit
        /// </summary>
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets Region
        /// </summary>
        public string Region { get; set; }
    }
}