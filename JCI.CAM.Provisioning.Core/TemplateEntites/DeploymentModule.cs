//-----------------------------------------------------------------------
// <copyright file= "DeploymentModule.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a SharePoint module 
    /// </summary>
    [XmlRoot(ElementName = "Module")]
    public class DeploymentModule
    {
        /// <summary>
        /// Gets or sets the Files to deployed
        /// </summary>
       [XmlElement("File")]
        public List<DeploymentFile> Files
        {
            get;
            set;
        }

       /// <summary>
       /// Gets or sets the ListView web part.
       /// </summary>
       /// <value>
       /// The ListView web part.
       /// </value>
        [XmlElement("ListViewWebPart")]
       public string ListViewWebPart { get; set; }

        /// <summary>
        /// Gets or sets the name of the module
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the url where the files will be deployed
        /// </summary>
        [XmlAttribute]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of source files
        /// </summary>
        [XmlAttribute]
        public string Path
        {
            get;
            set;
        }
    }
}
