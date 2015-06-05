﻿//-----------------------------------------------------------------------
// <copyright file= "DeploymentFileProperty.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represent a file property
    /// </summary>
    [XmlRoot(ElementName = "Property")]
    public class DeploymentFileProperty
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the property value
        /// </summary>
        [XmlAttribute]
        public string Value
        {
            get;
            set;
        }
    }
}
