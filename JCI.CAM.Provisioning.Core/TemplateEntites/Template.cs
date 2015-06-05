// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Template.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Internal class that is a facade for getting values from the web config
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Template xml schema
    /// </summary>
    [XmlRoot(ElementName = "Template")]
    public partial class Template
    {
        #region Private Members       
        /// <summary>
        /// The list instances
        /// </summary>
        private List<ListInstance> listInstances = new List<ListInstance>();
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttribute]
        public string ID { get; set; }       

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [XmlAttribute]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Template"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the root template.
        /// </summary>
        /// <value>
        /// The root template.
        /// </value>
        [XmlAttribute]
        public string RootTemplate { get; set; }       

        /// <summary>
        /// Gets or sets the name of the theme package.
        /// </summary>
        /// <value>
        /// The name of the theme package.
        /// </value>
        [XmlAttribute]
        public string ThemePackageName { get; set; }       

        /// <summary>
        /// Gets or sets the storage maximum level.
        /// </summary>
        /// <value>
        /// The storage maximum level.
        /// </value>
        [XmlAttribute]
        public long StorageMaximumLevel { get; set; }

        /// <summary>
        /// Gets or sets the storage warning level.
        /// </summary>
        /// <value>
        /// The storage warning level.
        /// </value>
        [XmlAttribute]
        public long StorageWarningLevel { get; set; }

        /// <summary>
        /// Gets or sets the user code maximum level.
        /// </summary>
        /// <value>
        /// The user code maximum level.
        /// </value>
        [XmlAttribute]
        public long UserCodeMaximumLevel { get; set; }

        /// <summary>
        /// Gets or sets the user code warning level.
        /// </summary>
        /// <value>
        /// The user code warning level.
        /// </value>
        [XmlAttribute]
        public long UserCodeWarningLevel { get; set; }

        /// <summary>
        /// Gets or sets the name of the configuration.
        /// </summary>
        /// <value>
        /// The name of the configuration.
        /// </value>
        [XmlAttribute]
        public string ConfigurationName { get; set; }

        /// <summary>
        /// Gets or sets the configuration container.
        /// </summary>
        /// <value>
        /// The configuration container.
        /// </value>
        public ConfigurationContainer ConfigurationContainer { get; set; }       

        /// <summary>
        /// Gets or sets the site policy.
        /// </summary>
        /// <value>
        /// The site policy.
        /// </value>        
        [XmlAttribute]
        public string SitePolicy { get; set; }

        /// <summary>
        /// Gets or sets the test site policy.
        /// </summary>
        /// <value>
        /// The test site policy.
        /// </value>
        [XmlAttribute]
        public string TestSitePolicy { get; set; }
        #endregion
    }
}
