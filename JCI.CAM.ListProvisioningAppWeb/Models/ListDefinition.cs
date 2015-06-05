//-----------------------------------------------------------------------
// <copyright file= "ListDefinition.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.ListProvisioningAppWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// List Definition
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsList")]
    public class ListDefinition
    {
        /// <summary>
        /// Gets or sets the content types.
        /// </summary>
        /// <value>
        /// The content types.
        /// </value>
        public ContentTypeDefinitions ContentTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public ListFields Fields
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the views.
        /// </summary>
        /// <value>
        /// The views.
        /// </value>
        [XmlArrayItemAttribute("View", IsNullable = false)]
        public List<ListView> Views
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the receivers.
        /// </summary>
        /// <value>
        /// The receivers.
        /// </value>
        [XmlArrayItemAttribute("Receiver", IsNullable = false)]
        public List<ListReceiver> Receivers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [XmlAttributeAttribute]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [XmlAttributeAttribute]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the base.
        /// </summary>
        /// <value>
        /// The type of the base.
        /// </value>
        [XmlAttributeAttribute]
        public byte BaseType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the enable content types.
        /// </summary>
        /// <value>
        /// The enable content types.
        /// </value>
        [XmlAttributeAttribute]
        public string EnableContentTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the versioning enabled.
        /// </summary>
        /// <value>
        /// The versioning enabled.
        /// </value>
        [XmlAttributeAttribute]
        public string VersioningEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the major version limit.
        /// </summary>
        /// <value>
        /// The major version limit.
        /// </value>
        [XmlAttributeAttribute]
        public byte MajorVersionLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [major version limit specified].
        /// </summary>
        /// <value>
        /// <c>true</c> if [major version limit specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnoreAttribute]
        public bool MajorVersionLimitSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the enable minor versions.
        /// </summary>
        /// <value>
        /// The enable minor versions.
        /// </value>
        [XmlAttributeAttribute]
        public string EnableMinorVersions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the major with minor versions limit.
        /// </summary>
        /// <value>
        /// The major with minor versions limit.
        /// </value>
        [XmlAttributeAttribute]
        public byte MajorWithMinorVersionsLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [major with minor versions limit specified].
        /// </summary>
        /// <value>
        /// <c>true</c> if [major with minor versions limit specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnoreAttribute]
        public bool MajorWithMinorVersionsLimitSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        [XmlAttributeAttribute]
        public string Direction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the folder creation.
        /// </summary>
        /// <value>
        /// The folder creation.
        /// </value>
        [XmlAttributeAttribute]
        public string FolderCreation
        {
            get;
            set;
        }
    }
}