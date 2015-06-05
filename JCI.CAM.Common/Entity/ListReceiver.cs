//-----------------------------------------------------------------------
// <copyright file= "ListReceiver.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{    
    using System;
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;

    /// <summary>
    /// List receiver
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListReceiver")]
    public partial class ListReceiver
    {
        /// <summary>
        /// The list receiver URL
        /// </summary>
        private string listReceiverUrl = string.Empty;

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