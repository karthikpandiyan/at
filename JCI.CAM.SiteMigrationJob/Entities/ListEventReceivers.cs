//-----------------------------------------------------------------------
// <copyright file= "ListEventReceivers.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationJob.Entities
{   
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;

    /// <summary>
    /// List event receivers class
    /// </summary>
    public class ListEventReceivers
    {
        /// <summary>
        /// Event Receiver class
        /// </summary>
        public class Receiver
        {
            /// <summary>
            /// List receiver URL
            /// </summary>
            private string listReceiverUrl = string.Empty;

            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>
            /// The title.
            /// </value>
            [XmlAttribute("ListTitle")]
            public string ListTitle
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the list template type.
            /// </summary>
            /// <value>
            /// List template type.
            /// </value>
            [XmlAttribute("TemplateType")]
            public int TemplateType
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            public string Type
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the sequence number.
            /// </summary>
            /// <value>
            /// The sequence number.
            /// </value>
            public ushort SequenceNumber
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the URL.
            /// </summary>
            /// <value>
            /// The URL.
            /// </value>
            public string Url
            {
                get
                {
                    return ListExtensions.ReplaceUrlTokens(this.listReceiverUrl);
                }

                set
                {
                    this.listReceiverUrl = value;
                }
            }

            /// <summary>
            /// Gets or sets the synchronization.
            /// </summary>
            /// <value>
            /// The synchronization.
            /// </value>
            public string Synchronization { get; set; }
        }        
    }    
}