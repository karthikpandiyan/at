//-----------------------------------------------------------------------
// <copyright file= "ContentTypeFieldRef.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Content Type Field Ref
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListContentTypesContentTypeFieldRef")]
    public class ContentTypeFieldRef
    {
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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttributeAttribute]
        public string Name
        {
            get;
            set;
        }
    }
}