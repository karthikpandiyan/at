// <copyright file= "SiteMigrationResponseMessage.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Migration.Common.Entity
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Entity class for Site Migration Response
    /// </summary>
    [Serializable]
    [DataContract]
    public class SiteMigrationResponseMessage
    {
        /// <summary>
        /// Gets or sets a value indicating whether if the site request has error or not
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is faulted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFaulted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Fault Message
        /// </summary>
        /// <value>
        /// The fault message.
        /// </value>
        public string FaultMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the site request payload. The incoming request is sent back to the caller.
        /// This is represents the SiteMigrationRequest object as an string in XML string. You must Serialize
        /// the SiteMigrationRequest object.
        /// </summary>
        /// <value>
        /// The site migration request.
        /// </value>
        public string SiteMigrationRequest
        {
            get;
            set;
        }
    }
}