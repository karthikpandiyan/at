// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationContainers.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Interface that is implemented for Site Provisioning for both On premise and Office 365
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration Containers
    /// </summary>
    [XmlRoot(ElementName = "ConfigurationContainers")]    
    public partial class ConfigurationContainers
    {
        /// <summary>
        /// Gets or sets the configuration container items.
        /// </summary>
        /// <value>
        /// The configuration container items.
        /// </value>
         [XmlArray(ElementName = "Containers")]
        [XmlArrayItem("ConfigurationContainer", typeof(ConfigurationContainer))]
        public List<ConfigurationContainer> ConfigurationContainerItems { get; set; }
    }
}
