//-----------------------------------------------------------------------
// <copyright file= "BrandingEntity.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Branding Entity
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("Branding")]
    public class BrandingEntity
    {
        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        public ThemeEntity ThemePackage { get; set; }

        /// <summary>
        /// Gets or sets the custom action.
        /// </summary>
        /// <value>
        /// The custom action.
        /// </value>
        public CustomActionEntity CustomAction { get; set; }
    }
}
