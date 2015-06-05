// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListInstance.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SharePointDataController retrieves sharepoint document metadata
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// List Instance
    /// </summary>
    public class ListInstance
    {
        /// <summary>
        /// The content type bindings
        /// </summary>
        private List<ContentTypeBinding> contentTypeBindings = new List<ContentTypeBinding>();

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [XmlAttribute]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the document template.
        /// </summary>
        /// <value>
        /// The document template.
        /// </value>
        [XmlAttribute]
        public string ListTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the name of the definition.
        /// </summary>
        /// <value>
        /// The name of the definition.
        /// </value>
        [XmlAttribute]
        public string CustomListDefinitionName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [on quick launch].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [on quick launch]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool OnQuickLaunch { get; set; }  

        /// <summary>
        /// Gets or sets a value indicating whether [enable versioning].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable versioning]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool EnableVersioning { get; set; }

        /// <summary>
        /// Gets or sets the get content type bindings.
        /// </summary>
        /// <value>
        /// The get content type bindings.
        /// </value>
        [XmlArray(ElementName = "ContentTypeBindings")]
        [XmlArrayItem("ContentTypeBinding", typeof(ContentTypeBinding))]
        public List<ContentTypeBinding> GetContentTypeBindings
        {
            get { return this.contentTypeBindings; }
            set { this.contentTypeBindings = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is custom; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsCustom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [remove default content types].
        /// </summary>
        /// <value>
        /// <c>true</c> if [remove default content types]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool RemoveDefaultContentTypes { get; set; }
    }
}
