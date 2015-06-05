//-----------------------------------------------------------------------
// <copyright file= "ContentTypeBinding.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Content Type Binding
    /// </summary>
    [XmlRoot(ElementName = "ContentTypeBinding")]
    public class ContentTypeBinding
    {
        /// <summary>
        /// Gets or sets the content type identifier.
        /// </summary>
        /// <value>
        /// The content type identifier.
        /// </value>
        [XmlAttribute]
        public string ContentTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ContentTypeBinding"/> is default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if default; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool Default { get; set; }
    }
}
