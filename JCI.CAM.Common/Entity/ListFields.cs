//-----------------------------------------------------------------------
// <copyright file= "ListFields.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
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
    /// List Fields
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListFields")]
    public class ListFields
    {
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [XmlElementAttribute("Field")]
        public List<ListField> Field
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [XmlTextAttribute]
        public List<string> Text
        {
            get;
            set;
        }
    }
}