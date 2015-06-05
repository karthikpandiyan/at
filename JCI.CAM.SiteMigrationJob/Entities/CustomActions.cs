//-----------------------------------------------------------------------
// <copyright file= "CustomActions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationJob.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// Custom actions details
    /// </summary>    
    public class CustomActions
    {
        /// <summary>
        /// Custom action class
        /// </summary>
        public class CustomAction
        {
            /// <summary>
            /// Gets or sets the name of custom action.
            /// </summary>
            [XmlAttribute("Name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the script block of custom action.
            /// </summary>
            [XmlElement("ScriptBlock")]
            public string ScriptBlock { get; set; }
        }        
    }    
}