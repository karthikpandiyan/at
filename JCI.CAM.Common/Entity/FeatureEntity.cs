//-----------------------------------------------------------------------
// <copyright file= "FeatureEntity.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Feature entity
    /// </summary>
    [SerializableAttribute]
    [XmlRootAttribute("Feature")]
    public class FeatureEntity
    {
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
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        [XmlAttributeAttribute]
        public string Scope
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FeatureEntity"/> is activate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if activate; otherwise, <c>false</c>.
        /// </value>
        [XmlAttributeAttribute]
        public bool Activate
        {
            get;
            set;
        }
    }
}
