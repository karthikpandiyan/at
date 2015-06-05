// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProvisioningRequestMessage.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.Azure.Framework.Provisioning
{
    /// <summary>
    /// Site Request Message that is used as the Data Contract for the Site Provisioning Engine.
    /// </summary>
    public class ProvisioningRequestMessage
    {
        /// <summary>
        /// Gets or sets the address of the queue to reply to
        /// </summary>
        public string ReplyTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the site request payload. The incoming request is sent back to the caller.
        /// This is represents the SiteRequestInformation object as an string in XML string. You must Serialize
        /// the SiteRequestInformation object.
        /// <see cref="Framework.Provisioning.Core.SiteRequestInformation" /><see cref="Framework.Provisioning.Core.Utilities.XmlSerializerHelper" />
        /// </summary>
        /// <value>
        /// The site request.
        /// </value>
        public string SiteRequest
        {
            get;
            set;
        }      
    }
}
