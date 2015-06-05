//-----------------------------------------------------------------------
// <copyright file= "ListFieldRefs.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// List Field Refs
    /// </summary>
    [SerializableAttribute]   
    [XmlRootAttribute("ListsListFieldsFieldFieldRefs")]
    public class ListFieldRefs
    {
        /// <summary>
        /// Gets or sets the field reference.
        /// </summary>
        /// <value>
        /// The field reference.
        /// </value>
        public ListFieldRef FieldRef
        {
            get;
            set;
        }
    }
}