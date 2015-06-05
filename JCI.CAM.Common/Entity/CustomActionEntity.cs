//-----------------------------------------------------------------------
// <copyright file= "CustomActionEntity.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Xml.Serialization;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Custom action entity
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("CustomAction")]
    public class CustomActionEntity
    {
        /// <summary>
        /// Gets or sets a value that specifies an implementation specific XML fragment that determines user interface properties of the custom action
        /// </summary>
        /// <value>
        /// The command UI extension.
        /// </value>
        public string CommandUIExtension { get; set; }

        /// <summary>
        /// Gets or sets the value that specifies the identifier of the object associated with the custom action.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        public string RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the type of the registration.
        /// </summary>
        /// <value>
        /// The type of the registration.
        /// </value>
        public UserCustomActionRegistrationType? RegistrationType { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom action.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttributeAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [XmlAttributeAttribute]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [XmlAttributeAttribute]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [XmlAttributeAttribute]
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the script block.
        /// </summary>
        /// <value>
        /// The script block.
        /// </value>
        public string ScriptBlock
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        [XmlAttributeAttribute]
        public int Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        [XmlAttributeAttribute]
        public string Group
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
        [XmlAttributeAttribute]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value that specifies the permissions needed for the custom action.
        /// </summary>
        /// <value>
        /// The rights.
        /// </value>
        [XmlAttributeAttribute]
        public string Rights
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value that specifies the permissions needed for the custom action.
        /// </summary>
        /// <value>
        /// The rights.
        /// </value>
        public BasePermissions RightsPermissions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CustomActionEntity"/> is remove.
        /// </summary>
        /// <value>
        ///   <c>true</c> if remove; otherwise, <c>false</c>.
        /// </value>
        public bool Remove
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that specifies the URI of a file which contains the ECMAScript to execute on the page
        /// </summary>
        /// <value>
        /// The script source.
        /// </value>
        public string ScriptSrc { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the URI of a file which contains the ECMAScript to execute on the page
        /// </summary>
        /// <value>
        /// The script source.
        /// </value>
        [XmlAttributeAttribute]
        public string Enabled { get; set; }
    }
}
