//-----------------------------------------------------------------------
// <copyright file= "DeploymentModules.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a collection of SharePoint modules
    /// </summary>
    [XmlRoot("Modules")]
    public class DeploymentModules
    {
        /// <summary>
        /// Backing variable for Elements property
        /// </summary>
        private List<DeploymentModule> elements = new List<DeploymentModule>();

        /// <summary>
        /// Gets or sets the Elements collection
        /// </summary>
        [XmlArray("Elements")]
        [XmlArrayItem(ElementName = "Module", Type = typeof(DeploymentModule))]
        public List<DeploymentModule> Elements
        {
            get
            {
                return this.elements;
            }

            set
            {
                this.elements = value;
            }
        }
    }
}
