//-----------------------------------------------------------------------
// <copyright file= "SiteMigrationRequestMessage.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Migration.Common.Entity
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Site Migration Request Message that is used as the Data Contract for the Site Migration Engine.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SiteMigrationRequestMessage
    {
        /// <summary>
        /// Gets or sets the site migration request payload. The incoming request is sent back to the caller.
        /// This is represents the SiteMigrationRequest object as an string in XML string. You must Serialize
        /// the SiteMigrationRequest object.
        /// </summary>
        /// <value>
        /// The site migration request.
        /// </value>
        [DataMember]
        public string SiteMigrationRequest
        {
            get;
            set;
        }
    }
}