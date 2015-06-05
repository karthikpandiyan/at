// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationContainer.cs" company="Microsoft">
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
    /// Configuration Container
    /// </summary>
    [XmlRoot(ElementName = "ConfigurationContainer")]
    public partial class ConfigurationContainer
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
        /// Gets or sets the security.
        /// </summary>
        /// <value>
        /// The security.
        /// </value>
        [XmlElement]
        public SiteSecurity Security { get; set; }

        /// <summary>
        /// Gets or sets the list instances.
        /// </summary>
        /// <value>
        /// The list instances.
        /// </value>
        [XmlArray(ElementName = "Lists")]
        [XmlArrayItem("ListInstance", typeof(ListInstance))]
        public List<ListInstance> ListInstances { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        [XmlArray(ElementName = "FieldsConfig")]
        [XmlArrayItem("Fields", typeof(FieldsFile))]
        public List<FieldsFile> Fields { get; set; }

        /// <summary>
        /// Gets or sets the content type configuration file.
        /// </summary>
        /// <value>
        /// The content type configuration file.
        /// </value>
        [XmlArray(ElementName = "ContentTypeConfig")]
        [XmlArrayItem("ContentType", typeof(ContentTypeFile))]
        public List<ContentTypeFile> ContentTypeConfigFile { get; set; }

        /// <summary>
        /// Gets or sets the features.
        /// </summary>
        /// <value>
        /// The features.
        /// </value>
        [XmlArray(ElementName = "Features")]
        [XmlArrayItem("Feature", typeof(Feature))]
        public List<Feature> Features { get; set; }

        /// <summary>
        /// Gets or sets the custom actions.
        /// </summary>
        /// <value>
        /// The custom actions.
        /// </value>
        [XmlArray(ElementName = "CustomActions")]
        [XmlArrayItem("CustomAction", typeof(CustomAction))]
        public List<CustomAction> CustomActions { get; set; }

        /// <summary>
        /// Gets or sets the deployment modules.
        /// </summary>
        /// <value>
        /// The deployment modules.
        /// </value>
        [XmlArray(ElementName = "Modules")]
        [XmlArrayItem("Module", typeof(DeploymentModule))]
        public List<DeploymentModule> DeploymentModules { get; set; }

        /// <summary>
        /// Gets or sets the site properties.
        /// </summary>
        /// <value>
        /// The site properties.
        /// </value>
        [XmlArray(ElementName = "SiteProperties")]
        [XmlArrayItem("SiteProperty", typeof(SiteProperty))]
        public List<SiteProperty> SiteProperties { get; set; }        
    }
}
