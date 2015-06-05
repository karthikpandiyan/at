// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenancyConfiguration.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   TenancyConfiguration Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using JCI.CAM.Common.Models;

    /// <summary>
    /// TemplateConfiguration xml schema
    /// </summary>
    [XmlRoot(ElementName = "TenancyConfiguration")]
    public partial class TenancyConfiguration
    {
        #region private members      

        /// <summary>
        /// The list definition
        /// </summary>
        private List<Tenancy> tenancies;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the tenancies.
        /// </summary>
        /// <value>
        /// The tenancies.
        /// </value>
        [XmlArray(ElementName = "Tenancies")]
        [XmlArrayItem("Tenancy", typeof(Tenancy))]
        public List<Tenancy> Tenancies
        {
            get
            {
                return this.tenancies ?? (this.tenancies = new List<Tenancy>());
            }

            set
            {
                this.tenancies = value;
            }
        }
      
        #endregion
    }
}
