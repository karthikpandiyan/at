//-----------------------------------------------------------------------
// <copyright file= "DeploymentFile.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a File
    /// </summary>
    [XmlRoot(ElementName = "File")]
    public class DeploymentFile
    {
       /// <summary>
        /// Gets or sets the path of the file
        /// </summary>
        [XmlAttribute]
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the properties for the file
        /// </summary>
        [XmlElement("Property")]
        public List<DeploymentFileProperty> Properties
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Url of the file
        /// </summary>
        [XmlAttribute]
        public string Url
        {
            get;
            set;
        }
    }
}
