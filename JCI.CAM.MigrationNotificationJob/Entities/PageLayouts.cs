//-----------------------------------------------------------------------
// <copyright file= "PageLayouts.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationJob.Entities
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using JCI.CAM.Provisioning.Core.TemplateEntites;

    /// <summary>
    /// Page layout details
    /// </summary>    
    public class PageLayouts
    {
        /// <summary>
        /// Event Receiver class
        /// </summary>
        public class PageLayout
        {
            /// <summary>
            /// Gets or sets the home page name of the page layout.
            /// </summary>
            [XmlAttribute("HomePage")]
            public string HomePage { get; set; }

            /// <summary>
            /// Gets or sets the site template.
            /// </summary>
            [XmlAttribute("WebTemplate")]
            public string WebTemplate { get; set; }

            /// <summary>
            /// Gets or sets the name of the page layout.
            /// </summary>
            [XmlAttribute("Name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the library title of page layout.
            /// </summary>
            [XmlAttribute("Url")]
            public string Url { get; set; }

            /// <summary>
            /// Gets or sets the old page layout.
            /// </summary>
            [XmlAttribute("OldPageLayout")]
            public string OldPageLayout { get; set; }

            /// <summary>
            /// Gets or sets the new page layout.
            /// </summary>
            [XmlAttribute("NewPageLayout")]
            public string NewPageLayout { get; set; }

            /// <summary>
            /// Gets or sets the List View WebPart.
            /// </summary>
            [XmlElement("ListViewWebPart")]
            public string ListViewWebPart { get; set; }

            /// <summary>
            /// Gets or sets the Files to deployed
            /// </summary>
            [XmlElement("File")]
            public List<DeploymentFile> Files
            {
                get;
                set;
            }
        }        
    }    
}