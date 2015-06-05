// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProvisioningResponseMessage.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Response Message to send to Custom Application Incoming Queue that represents the response
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.Azure.Framework.Provisioning
{
    /// <summary>
    /// Response Message to send to Custom Application Incoming Queue that represents the response from the 
    /// site provisioning engine.
    /// </summary>
    public class ProvisioningResponseMessage
    {
        /// <summary>
        /// Gets or sets a value indicating whether if the site request has error or not
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is faulted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFaulted
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the Fault Message
        /// </summary>
        /// <value>
        /// The fault message.
        /// </value>
        public string FaultMessage
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
