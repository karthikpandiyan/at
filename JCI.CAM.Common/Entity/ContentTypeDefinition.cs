//-----------------------------------------------------------------------
// <copyright file= "ContentTypeDefinition.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
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
    /// Content Type Definition
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListContentTypesContentType")]
    public class ContentTypeDefinition
    {
        /// <summary>
        /// Gets or sets the field refs.
        /// </summary>
        /// <value>
        /// The field refs.
        /// </value>
        [XmlArrayItemAttribute("FieldRef", IsNullable = false)]
        public List<ContentTypeFieldRef> FieldRefs
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

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttributeAttribute]
        public string Group
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