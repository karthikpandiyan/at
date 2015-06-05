// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProvisioningService.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Interface that is implemented for Site Provisioning for both On premise and Office 365
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Data
{
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.TemplateEntites;       

    /// <summary>
    /// Interface that is implemented for Site Provisioning for both On premise and Office 365
    /// </summary>
    public interface IProvisioningService
    {
        /// <summary>
        /// Provisions the site.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="template">The template.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// Site collection creation status
        /// </returns>
        bool? ProvisionSite(SiteRequestInformation properties, Template template, TemplateManager manager, AppSettings settings);        
    }
}
