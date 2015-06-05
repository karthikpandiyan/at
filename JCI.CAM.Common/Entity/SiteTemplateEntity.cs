//-----------------------------------------------------------------------
// <copyright file= "SiteTemplateEntity.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Site Template entity
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("SiteTemplate")]
    public class SiteTemplateEntity
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
         [XmlAttribute]
        public string Title { get; set; }

         /// <summary>
         /// Gets or sets the name.
         /// </summary>
         /// <value>
         /// The name.
         /// </value>
         [XmlAttribute]
         public string Name { get; set; }
    }
}
