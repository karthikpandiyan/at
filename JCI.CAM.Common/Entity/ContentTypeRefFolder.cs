//-----------------------------------------------------------------------
// <copyright file= "ContentTypeRefFolder.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Content Type Ref Folder
    /// </summary>
    [SerializableAttribute]    
    [XmlRootAttribute("ListsListContentTypesContentTypeRefFolder")]
    public class ContentTypeRefFolder
    {
        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        /// <value>
        /// The name of the target.
        /// </value>
        [XmlAttributeAttribute]
        public string TargetName
        {
            get;
            set;
        }
    }
}