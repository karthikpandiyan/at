//-----------------------------------------------------------------------
// <copyright file= "ListView.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
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
    /// List View
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListView")]
    public class ListView
    {
        /// <summary>
        /// Gets or sets the view fields.
        /// </summary>
        /// <value>
        /// The view fields.
        /// </value>
        [XmlArrayItemAttribute("FieldRef", IsNullable = false)]
        public List<ListViewFieldRef> ViewFields
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public string Query
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the base view identifier.
        /// </summary>
        /// <value>
        /// The base view identifier.
        /// </value>
        [XmlAttributeAttribute]
        public byte BaseViewID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [XmlAttributeAttribute]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mobile view.
        /// </summary>
        /// <value>
        /// The mobile view.
        /// </value>
        [XmlAttributeAttribute]
        public string MobileView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tabular view.
        /// </summary>
        /// <value>
        /// The tabular view.
        /// </value>
        [XmlAttributeAttribute]
        public string TabularView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the free form.
        /// </summary>
        /// <value>
        /// The free form.
        /// </value>
        [XmlAttributeAttribute]
        public string FreeForm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the row limit.
        /// </summary>
        /// <value>
        /// The row limit.
        /// </value>
        [XmlAttributeAttribute]
        public byte RowLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [row limit specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [row limit specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnoreAttribute]
        public bool RowLimitSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [XmlAttributeAttribute]
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default view.
        /// </summary>
        /// <value>
        /// The default view.
        /// </value>
        [XmlAttributeAttribute]
        public string DefaultView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mobile default view.
        /// </summary>
        /// <value>
        /// The mobile default view.
        /// </value>
        [XmlAttributeAttribute]
        public string MobileDefaultView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the setup path.
        /// </summary>
        /// <value>
        /// The setup path.
        /// </value>
        [XmlAttributeAttribute]
        public string SetupPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        [XmlAttributeAttribute]
        public string ImageUrl
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
        /// Gets or sets the file dialog.
        /// </summary>
        /// <value>
        /// The file dialog.
        /// </value>
        [XmlAttributeAttribute]
        public string FileDialog
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hidden.
        /// </summary>
        /// <value>
        /// The hidden.
        /// </value>
        [XmlAttributeAttribute]
        public string Hidden
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        [XmlAttributeAttribute]
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the moderation.
        /// </summary>
        /// <value>
        /// The type of the moderation.
        /// </value>
        [XmlAttributeAttribute]
        public string ModerationType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the web part zone identifier.
        /// </summary>
        /// <value>
        /// The web part zone identifier.
        /// </value>
        [XmlAttributeAttribute]
        public string WebPartZoneID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the requires client integration.
        /// </summary>
        /// <value>
        /// The requires client integration.
        /// </value>
        [XmlAttributeAttribute]
        public string RequiresClientIntegration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the read only.
        /// </summary>
        /// <value>
        /// The read only.
        /// </value>
        [XmlAttributeAttribute]
        public string ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the aggregate view.
        /// </summary>
        /// <value>
        /// The aggregate view.
        /// </value>
        [XmlAttributeAttribute]
        public string AggregateView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the toolbar template.
        /// </summary>
        /// <value>
        /// The toolbar template.
        /// </value>
        [XmlAttributeAttribute]
        public string ToolbarTemplate
        {
            get;
            set;
        }
    }
}