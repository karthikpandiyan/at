// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharePointUser.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SharePoint User Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Domain Object for SharePoint Users
    /// </summary>
    [DataContract]
    public class SharePointUser
    {
        /// <summary>
        /// Gets or sets the name of the login.
        /// </summary>
        /// <value>
        /// The name of the login.
        /// </value>
        [DataMember]
        public string LoginName
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
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataMember]
        public string Email
        {
            get;
            set;
        }
    }
}
