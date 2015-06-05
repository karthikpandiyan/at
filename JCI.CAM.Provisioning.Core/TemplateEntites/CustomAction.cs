// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAction.cs" company="Microsoft">
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
    /// CustomAction xml schema
    /// </summary>
    public partial class CustomAction
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
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        [XmlAttribute]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [XmlAttribute]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [XmlAttribute]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        [XmlAttribute]
        public int Sequence { get; set; }        

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [XmlAttribute]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CustomAction"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the script source.
        /// </summary>
        /// <value>
        /// The script source.
        /// </value>
        [XmlAttribute]
        public string ScriptSrc { get; set; }

        /// <summary>
        /// Gets or sets the script block.
        /// </summary>
        /// <value>
        /// The script block.
        /// </value>
        [XmlElement]
        public string ScriptBlock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [apply only to test site].
        /// </summary>
        /// <value>
        /// <c>true</c> if [apply only to test site]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool ApplyOnlyToTestSite { get; set; }
    }
}
