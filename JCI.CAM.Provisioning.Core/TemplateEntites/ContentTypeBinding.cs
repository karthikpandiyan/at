// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentTypeBinding.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SharePointDataController retrieves sharepoint document metadata
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Xml.Serialization;

    /// <summary>
    /// ContentTypeBinding xml schema
    /// </summary>
    public class ContentTypeBinding
    {
        /// <summary>
        /// Gets or sets the content type identifier.
        /// </summary>
        /// <value>
        /// The content type identifier.
        /// </value>
        [XmlAttribute]
        public string ContentTypeID { get; set; }

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
