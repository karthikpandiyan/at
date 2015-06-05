// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteSecurity.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Internal class that is a facade for getting values from the web config
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// SiteSecurity xml schema
    /// </summary>
    [XmlRoot(ElementName = "Security")]
    public partial class SiteSecurity
    {
        #region Private
        /// <summary>
        /// The additional administrators
        /// </summary>
        private List<AdditionalAdministrator> additionalAdministrators;

        /// <summary>
        /// The additional owners
        /// </summary>
        private List<Owner> additionalOwners;

        /// <summary>
        /// The additional members
        /// </summary>
        private List<Member> additionalMembers;

        /// <summary>
        /// The additional visitors
        /// </summary>
        private List<Vistor> additionalVisitors;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the additional administrators.
        /// </summary>
        /// <value>
        /// The additional administrators.
        /// </value>
        [XmlArray(ElementName = "AdditionalAdministrators")]
        [XmlArrayItem("User", typeof(AdditionalAdministrator))]
        public List<AdditionalAdministrator> AdditionalAdministrators
        {
            get
            {
                return this.additionalAdministrators ?? (this.additionalAdministrators = new List<AdditionalAdministrator>());
            }

            set 
            {
                this.additionalAdministrators = value; 
            }
        }

        /// <summary>
        /// Gets or sets the additional owners.
        /// </summary>
        /// <value>
        /// The additional owners.
        /// </value>
        [XmlArray(ElementName = "AdditionalOwners")]
        [XmlArrayItem("User", typeof(Owner))]
        public List<Owner> AdditionalOwners
        {
            get
            {
                return this.additionalOwners ?? (this.additionalOwners = new List<Owner>());
            }

            set 
            {
                this.additionalOwners = value; 
            }
        }

        /// <summary>
        /// Gets or sets the additional members.
        /// </summary>
        /// <value>
        /// The additional members.
        /// </value>
        [XmlArray(ElementName = "AdditionalMembers")]
        [XmlArrayItem("User", typeof(Member))]
        public List<Member> AdditionalMembers
        {
            get
            {
                return this.additionalMembers ?? (this.additionalMembers = new List<Member>());
            }

            set 
            {
                this.additionalMembers = value;
            }
        }

        /// <summary>
        /// Gets or sets the additional visitors.
        /// </summary>
        /// <value>
        /// The additional visitors.
        /// </value>
        [XmlArray(ElementName = "AdditionalVistors")]
        [XmlArrayItem("User", typeof(Vistor))]
        public List<Vistor> AdditionalVisitors
        {
            get
            {
                return this.additionalVisitors ?? (this.additionalVisitors = new List<Vistor>());
            }

            set 
            {
                this.additionalVisitors = value; 
            }
        }
        #endregion
    }
}
