//-----------------------------------------------------------------------
// <copyright file= "ListDefinitions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
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
    /// List Definitions
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("Lists")]
    public class ListDefinitions
    {
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        [XmlElementAttribute("List")]
        public List<ListDefinition> List
        {
            get;
            set;
        }
    }
}