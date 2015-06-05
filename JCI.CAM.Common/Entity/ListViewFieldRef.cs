//-----------------------------------------------------------------------
// <copyright file= "ListViewFieldRef.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// List view field ref
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListViewFieldRef")]
    public class ListViewFieldRef
    {
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

        /// <summary>
        /// Gets or sets the text only.
        /// </summary>
        /// <value>
        /// The text only.
        /// </value>
        [XmlAttributeAttribute]
        public string TextOnly
        {
            get;
            set;
        }
    }
}