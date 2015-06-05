//-----------------------------------------------------------------------
// <copyright file= "TemplateConfiguration.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Template configuration
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("TemplateConfiguration")]
    public class TemplateConfiguration
    {
        /// <summary>
        /// Gets or sets the features.
        /// </summary>
        /// <value>
        /// The features.
        /// </value>
        [XmlArrayItemAttribute("Feature", IsNullable = false)]
        public List<FeatureEntity> Features
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policies.
        /// </summary>
        /// <value>
        /// The policies.
        /// </value>
        [XmlArrayItemAttribute("Policy", IsNullable = false)]
        public List<PolicyEntity> Policies
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the theme package.
        /// </summary>
        /// <value>
        /// The theme package.
        /// </value>
        public ThemeEntity ThemePackage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the custom actions.
        /// </summary>
        /// <value>
        /// The custom actions.
        /// </value>
        [XmlArrayItemAttribute("CustomAction", IsNullable = false)]
        public List<CustomActionEntity> CustomActions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the custom actions.
        /// </summary>
        /// <value>
        /// The custom actions.
        /// </value>
        [XmlArrayItemAttribute("SiteTemplate", IsNullable = false)]
        public List<SiteTemplateEntity> SiteTemplates
        {
            get;
            set;
        }
    }
}
