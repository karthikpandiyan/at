//-----------------------------------------------------------------------
// <copyright file= "ListTemplates.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// List templates
    /// </summary>
    [XmlRoot(ElementName = "ListTemplatesConfiguration")]
    public class ListTemplates
    {
        /// <summary>
        /// Gets or sets the list instances.
        /// </summary>
        /// <value>
        /// The list instances.
        /// </value>
        [XmlArray(ElementName = "ListTemplates")]
        [XmlArrayItem("ListInstance", typeof(ListInstanceTemplate))]
        public List<ListInstanceTemplate> ListInstances
        {
            get;
            set;
        }
    }
}
