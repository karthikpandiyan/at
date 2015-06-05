// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISharePointExtensibility.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core.Extensibility
{
    /// <summary>
    /// Interface SharePoint Extensibility
    /// </summary>
    public interface ISharePointExtensibility
    {
        /// <summary>
        /// Processes site request.
        /// </summary>
        /// <param name="request">Site request information.</param>
        void ProcessRequest(SiteRequestInformation request);
    }
}
