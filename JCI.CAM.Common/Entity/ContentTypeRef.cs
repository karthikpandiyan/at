//-----------------------------------------------------------------------
// <copyright file= "ContentTypeRef.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Content Type Ref
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListContentTypesContentTypeRef")]
    public class ContentTypeRef
    {
        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        /// <value>
        /// The folder.
        /// </value>
        public ContentTypeRefFolder Folder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttributeAttribute]
        public string ID
        {
            get;
            set;
        }
    }
}