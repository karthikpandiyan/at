//-----------------------------------------------------------------------
// <copyright file= "ContentTypeDefinitions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Content Type Definitions
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListContentTypes")]
    public class ContentTypeDefinitions
    {
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public ContentTypeDefinition ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content type reference.
        /// </summary>
        /// <value>
        /// The content type reference.
        /// </value>
        [XmlElementAttribute("ContentTypeRef")]
        public List<ContentTypeRef> ContentTypeRef
        {
            get;
            set;
        }
    }
}