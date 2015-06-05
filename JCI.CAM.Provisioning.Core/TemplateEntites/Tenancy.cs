// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tenancy.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Tenancy Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Xml.Serialization;

    /// <summary>
    /// Tenancy xml schema
    /// </summary>
    public partial class Tenancy
    {
        /// <summary>
        /// Gets or sets the web application url.
        /// </summary>
        /// <value>
        /// The web application URL.
        /// </value>
        [XmlAttribute]
        public string WebApplicationUrl { get; set; }

        /// <summary>
        /// Gets or sets the tenant admin url.
        /// </summary>
        /// <value>
        /// The tenant admin URL.
        /// </value>
        [XmlAttribute]
        public string TenantAdminUrl { get; set; }       
    }
}
