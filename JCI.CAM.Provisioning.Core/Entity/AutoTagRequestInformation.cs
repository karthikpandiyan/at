// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoTagRequestInformation.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Domain Object for working with SharePoint Auto tag Requests
    /// </summary>
    [DataContract]
    public class AutoTagRequestInformation
    {
        /// <summary>
        /// Gets or sets the site URL.
        /// </summary>
        /// <value>
        /// The site URL.
        /// </value>
        [DataMember]
        public string SiteURL { get; set; }

        /// <summary>
        /// Gets or sets the list identifier.
        /// </summary>
        /// <value>
        /// The list identifier.
        /// </value>
        [DataMember]
        public string ListID { get; set; }

        /// <summary>
        /// Gets or sets the list item identifier.
        /// </summary>
        /// <value>
        /// The list item identifier.
        /// </value>
        [DataMember]
        public int ListItemID { get; set; }

        /// <summary>
        /// Gets or sets the users email.
        /// </summary>
        /// <value>
        /// The users email.
        /// </value>
        [DataMember]
        public string UserLoginName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the user.
        /// </summary>
        /// <value>
        /// The display name of the user.
        /// </value>
        [DataMember]
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the current user identifier.
        /// </summary>
        /// <value>
        /// The current user identifier.
        /// </value>
        [DataMember]
        public int CurrentUserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [DataMember]
        public SharePointUser User
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>
        /// The retry count.
        /// </value>
        public int RetryCount { get; set; }
    }
}
