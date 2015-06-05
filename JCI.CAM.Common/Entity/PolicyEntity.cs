//-----------------------------------------------------------------------
// <copyright file= "PolicyEntity.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Policy entity
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("Policy")]
    public class PolicyEntity
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
