// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemePackage.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Xml.Serialization;

    /// <summary>
    /// ThemePackage xml schema
    /// </summary>
    public partial class ThemePackage
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
        /// Gets or sets the color file.
        /// </summary>
        /// <value>
        /// The color file.
        /// </value>
        [XmlAttribute]
        public string ColorFile { get; set; }

        /// <summary>
        /// Gets or sets the font file.
        /// </summary>
        /// <value>
        /// The font file.
        /// </value>
        [XmlAttribute]
        public string FontFile { get; set; }

        /// <summary>
        /// Gets or sets the background file.
        /// </summary>
        /// <value>
        /// The background file.
        /// </value>
        [XmlAttribute]
        public string BackgroundFile { get; set; }

        /// <summary>
        /// Gets or sets the master page.
        /// </summary>
        /// <value>
        /// The master page.
        /// </value>
        [XmlAttribute]
        public string MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the site logo.
        /// </summary>
        /// <value>
        /// The site logo.
        /// </value>
        [XmlAttribute]
        public string SiteLogo { get; set; }

        /// <summary>
        /// Gets or sets the alternate CSS.
        /// </summary>
        /// <value>
        /// The alternate CSS.
        /// </value>
        [XmlAttribute]
        public string AlternateCSS { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [XmlAttribute]
        public int Version { get; set; }
    }
}
