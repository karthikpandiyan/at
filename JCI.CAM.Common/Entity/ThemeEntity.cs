//-----------------------------------------------------------------------
// <copyright file= "ThemeEntity.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Theme entity
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ThemePackage")]
    public class ThemeEntity
    {
        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        public string Theme { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttributeAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        /// <value>
        /// The background image.
        /// </value>
        public string BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public string Font { get; set; }

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
        /// Gets or sets the custom master page.
        /// </summary>
        /// <value>
        /// The custom master page.
        /// </value>
        public string CustomMasterPage { get; set; }

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
