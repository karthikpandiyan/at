//-----------------------------------------------------------------------
// <copyright file= "ListFieldRef.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// List Field Ref
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListFieldsFieldFieldRefsFieldRef")]
    public class ListFieldRef
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
    }
}