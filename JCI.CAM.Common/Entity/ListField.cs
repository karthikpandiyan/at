//-----------------------------------------------------------------------
// <copyright file= "ListField.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// List Field
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("ListsListFieldsField")]
    public class ListField
    {
        /// <summary>
        /// Gets or sets the field refs.
        /// </summary>
        /// <value>
        /// The field refs.
        /// </value>
        public ListFieldRefs FieldRefs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public string Default
        {
            get;
            set;
        }        

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        /// <value>
        /// The choices.
        /// </value>
        public string FieldBody
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttributeAttribute]
        public string ID
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
        [XmlAttributeAttribute]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the multiple user picker.
        /// </summary>
        /// <value>
        /// The multiple user picker.
        /// </value>
        [XmlAttributeAttribute]
        public string Mult
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
        [XmlAttributeAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [XmlAttributeAttribute]
        public string DisplayName
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
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        [XmlAttributeAttribute]
        public string SourceID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        [XmlAttributeAttribute]
        public byte MaxLength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [maximum length specified].
        /// </summary>
        /// <value>
        /// <c>true</c> if [maximum length specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnoreAttribute]
        public bool MaxLengthSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the allow deletion.
        /// </summary>
        /// <value>
        /// The allow deletion.
        /// </value>
        [XmlAttributeAttribute]
        public string AllowDeletion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the static.
        /// </summary>
        /// <value>
        /// The name of the static.
        /// </value>
        [XmlAttributeAttribute]
        public string StaticName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the required.
        /// </summary>
        /// <value>
        /// The required.
        /// </value>
        [XmlAttributeAttribute]
        public string Required
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hidden.
        /// </summary>
        /// <value>
        /// The hidden.
        /// </value>
        [XmlAttributeAttribute]
        public string Hidden
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        [XmlAttributeAttribute]
        public string ResultType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the read only.
        /// </summary>
        /// <value>
        /// The read only.
        /// </value>
        [XmlAttributeAttribute]
        public string ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the enforce unique values.
        /// </summary>
        /// <value>
        /// The enforce unique values.
        /// </value>
        [XmlAttributeAttribute]
        public string EnforceUniqueValues
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the indexed.
        /// </summary>
        /// <value>
        /// The indexed.
        /// </value>
        [XmlAttributeAttribute]
        public string Indexed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        [XmlAttributeAttribute]
        public string Format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the language id.
        /// </summary>
        /// <value>
        /// The language id.
        /// </value>
        [XmlAttributeAttribute]
        public ushort LCID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [language id specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [language id specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnoreAttribute]
        public bool LCIDSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customization.
        /// </summary>
        /// <value>
        /// The customization.
        /// </value>
        [XmlAttributeAttribute]
        public string Customization
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the show in edit form.
        /// </summary>
        /// <value>
        /// The show in edit form.
        /// </value>
        [XmlAttributeAttribute]
        public string ShowInEditForm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the show in view forms.
        /// </summary>
        /// <value>
        /// The show in view forms.
        /// </value>
        [XmlAttributeAttribute]
        public string ShowInViewForms
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the show in list settings.
        /// </summary>
        /// <value>
        /// The show in list settings.
        /// </value>
        [XmlAttributeAttribute]
        public string ShowInListSettings
        {
            get;
            set;
        }
    }
}