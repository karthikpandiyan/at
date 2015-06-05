// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteProvisioningManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Provisioning Job
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core.Data
{
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.TemplateEntites;

    /// <summary>
    /// Site Provisioning Manager
    /// </summary>
    public class SiteProvisioningManager
    {
        #region Variables

        /// <summary>
        /// The online provisioning service
        /// </summary>
        private readonly OnlineProvisioningService onlineProvisioningService = new OnlineProvisioningService();

        /// <summary>
        /// The OnPremise provisioning service
        /// </summary>
        private readonly OnPremProvisioningService onPremProvisioningService = new OnPremProvisioningService();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteProvisioningManager" /> class.
        /// </summary>
        public SiteProvisioningManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteProvisioningManager"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SiteProvisioningManager(AppSettings settings)
        {
            if (settings.SharePointOnPremises)
            {
                this.onPremProvisioningService.Authentication = new AppOnlyAuthenticationTenant();
            }
            else
            {
                this.onlineProvisioningService.Authentication = new AppOnlyAuthenticationTenant();
            }
        }

        /// <summary>
        /// Processes the new site requests.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="templateManager">The template manager.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// Site creation Status
        /// </returns>
        public bool? ProcessNewSiteRequests(SiteRequestInformation request, TemplateManager templateManager, AppSettings settings)
        {
            bool? creationStatus = false;
            var template = templateManager.GetTemplateByID(request.Template);

            if (template != null)
            {
                string templateConfigurationName = template.ConfigurationName;
                ConfigurationContainer container = templateManager.GetConfigurationContainerByName(templateConfigurationName);
                request.TemplateTitle = template.Title;
                if (container != null)
                {
                    template.ConfigurationContainer = container;
                }

                if (settings.SharePointOnPremises)
                {                    
                    creationStatus = this.onPremProvisioningService.ProvisionSite(request, template, templateManager, settings);
                }
                else
                {
                    creationStatus = this.onlineProvisioningService.ProvisionSite(request, template, templateManager, settings);
                }
            }

            LogHelper.LogInformation("End processing new site requests", LogEventID.InformationWrite);
            return creationStatus;
        }
    }
}
