// <copyright file= "Features.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationJob.Entities
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Entity class for Features xml
    /// </summary>
    [Serializable]
    [DataContract]
    public class Features
    {
        /// <summary>
        /// Entity class for Feature info xml
        /// </summary>
        public class Feature
        {
            /// <summary>
            /// Gets or sets the feature name
            /// </summary>
            [XmlAttribute("Name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the feature ID 
            /// </summary>
            [XmlAttribute("GUID")]
            public Guid ID { get; set; }

            /// <summary>
            /// Gets or sets the feature scope
            /// </summary>
            [XmlAttribute("Scope")]
            public string Scope { get; set; }
        }
    }
}