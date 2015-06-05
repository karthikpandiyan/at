// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteRequestInformation.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Domain Object for working with SharePoint Site Requests
    /// </summary>
    [DataContract]
    public class SiteRequestInformation
    {
        #region instance members
        /// <summary>
        /// The time zone identifier
        /// </summary>
        private int timeZoneId = 13;

        /// <summary>
        /// Language Id
        /// </summary>
        private uint lcid = 1033;

        /// <summary>
        /// The additional admins
        /// </summary>
        private List<SharePointUser> additionalAdmins = new List<SharePointUser>();

        /// <summary>
        /// The external sharing enabled
        /// </summary>
        private bool externalSharingEnabled = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Site Collection URL
        /// </summary>
        [DataMember]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Site Collection Title
        /// </summary>
        [DataMember]
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Site Collection Description
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Site Template
        /// </summary>
        [DataMember]
        public string Template
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the template title.
        /// </summary>
        /// <value>
        /// The template title.
        /// </value>
        public string TemplateTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the business unit.
        /// </summary>
        /// <value>
        /// The business unit.
        /// </value>
        public string BusinessUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public string Region
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is test site.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is test site; otherwise, <c>false</c>.
        /// </value>
        public bool IsTestSite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the business unit admin.
        /// </summary>
        /// <value>
        /// The business unit admin.
        /// </value>
        public SharePointUser BusinessUnitAdmin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the business unit admin email.
        /// </summary>
        /// <value>
        /// The business unit admin email.
        /// </value>
        public SharePointUser BusinessUnitAdminEmail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tertiary admin user.
        /// </summary>
        /// <value>
        /// The tertiary admin user.
        /// </value>
        public SharePointUser TertiaryAdminUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the secondary admin user.
        /// </summary>
        /// <value>
        /// The secondary admin user.
        /// </value>
        public SharePointUser SecondaryAdminUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer terms collection.
        /// </summary>
        /// <value>
        /// The customer terms collection.
        /// </value>
        public string CustomerTermsCollection
        {
            get;
            set;
        }
       
        /// <summary>
        /// Gets or sets Owner of the Site Collection
        /// </summary>
        [DataMember]
        public SharePointUser SiteOwner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Addition Site Administrators
        /// </summary>
        [XmlArray(ElementName = "AdditionalAdministrators")]
        [XmlArrayItem("SharePointUser", typeof(SharePointUser))]
        public List<SharePointUser> AdditionalAdministrators
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Addition Site Administrators
        /// </summary>
        [XmlArray(ElementName = "InitialSiteUsers")]
        [XmlArrayItem("SharePointUser", typeof(SharePointUser))]
        public List<SharePointUser> InitialSiteUsers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Site Policy to Apply
        /// </summary>
        [DataMember]
        public string SitePolicy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the community site.
        /// </summary>
        /// <value>
        /// The type of the community site.
        /// </value>
        public string CommunitySiteType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list item identifier.
        /// </summary>
        /// <value>
        /// The list item identifier.
        /// </value>
        public int ListItemId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the requestor.
        /// </summary>
        /// <value>
        /// The name of the requestor.
        /// </value>
        public string RequestorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the requestor email.
        /// </summary>
        /// <value>
        /// The requestor email.
        /// </value>
        public string RequestorEmail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets string that represents the status of the request
        /// </summary>
        public string RequestStatus
        {
            get { return this.EnumStatus.ToString(); }
            set { this.EnumStatus = (SiteRequestStatus)Enum.Parse(typeof(SiteRequestStatus), value); }
        }

        /// <summary>
        /// Gets or setsStatus of the Request
        /// </summary>
        [XmlIgnore]
        public Enum EnumStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets site locale.
        /// The default value is 1033 for en-us
        /// </summary>
        /// <value>
        /// Site locale Id.
        /// </value>
        [DataMember]
        public uint Lcid
        {
            get
            {
                return this.lcid;
            }

            set
            {
                this.lcid = value;
            }
        }

        /// <summary>
        /// Gets or sets the time zone of the site collection.
        /// The default value is 13 (GMT-08:00) Pacific Time (US and Canada)
        /// </summary>
        [DataMember]
        public int TimeZoneId
        {
            get
            {
                return this.timeZoneId;
            }

            set
            {
                this.timeZoneId = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if External Sharing should be enabled. This option is not available in on-premises build.
        /// Default value is True
        /// </summary>
        [DataMember]
        public bool EnableExternalSharing
        {
            get { return this.externalSharingEnabled; }
            set { this.externalSharingEnabled = value; }
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        public string Country { get; set; }
        
        /// <summary>
        /// Gets the type of the site.
        /// </summary>
        /// <value>
        /// The type of the site.
        /// </value>
        public string SiteType
        {
            get
            {
                return this.Template;
            }
        }

        /// <summary>
        /// Gets or sets the moderator.
        /// </summary>
        /// <value>
        /// The moderator.
        /// </value>
        public SharePointUser Moderator { get; set; }

        /// <summary>
        /// Gets or sets the confidentiality level.
        /// </summary>
        /// <value>
        /// The confidentiality level.
        /// </value>
        public string ConfidentialityLevel { get; set; }

        /// <summary>
        /// Gets or sets the customers terms.
        /// </summary>
        /// <value>
        /// The customers terms.
        /// </value>
        public string CustomersTerms { get; set; }

        /// <summary>
        /// Gets or sets the project start date.
        /// </summary>
        /// <value>
        /// The project start date.
        /// </value>
        public string ProjectStartDate { get; set; }

        /// <summary>
        /// Gets or sets the project end date.
        /// </summary>
        /// <value>
        /// The project end date.
        /// </value>
        public string ProjectEndDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the name of the partner.
        /// </summary>
        /// <value>
        /// The name of the partner.
        /// </value>
        public string PartnerName { get; set; }
        #endregion
    }
}
