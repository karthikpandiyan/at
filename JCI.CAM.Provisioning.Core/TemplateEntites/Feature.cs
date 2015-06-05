// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feature.cs" company="Microsoft">
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
    /// Feature xml schema
    /// </summary>
    public partial class Feature
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttribute]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        [XmlAttribute]
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature"/> is activate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if activate; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool Activate { get; set; }
    }
}
