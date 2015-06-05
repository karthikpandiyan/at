// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnPremProvisioningService.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Provisioning Service Implementation for On-premises and Office 365 SPO-D Legacy
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Data
{  
    using System;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.TemplateEntites;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    
    /// <summary>
    /// Site Provisioning Service Implementation for On-premises and Office 365 SPO-D Legacy
    /// </summary>
    public class OnPremProvisioningService : AbstractProvisioningService, ISharePointService
    {
        #region Instance Members
        /// <summary>
        /// Site request factory instance
        /// </summary>
        private readonly ISiteRequestFactory requestFactory;

        /// <summary>
        /// Configuration factory manager instance
        /// </summary>
        private IConfigurationFactory configFactory = ConfigurationFactoryManager.GetInstance();

        /// <summary>
        /// App settings instance
        /// </summary>
        private AppSettings settings = null;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OnPremProvisioningService"/> class.
        /// </summary>
        public OnPremProvisioningService()
            : base()
        {
            this.requestFactory = SiteRequestFactory.GetInstance();
        }
        #endregion

        /// <summary>
        /// Provisions the site.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="template">The template.</param>
        /// <param name="templateManager">The template manager.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// Site collection creation status
        /// </returns>
        public override bool? ProvisionSite(SiteRequestInformation properties, Template template, TemplateManager templateManager, AppSettings settings)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnPremProvisioningService.ProvisionSite", LogEventID.InformationWrite);
            bool creationStatus = false;
            this.settings = settings;

            this.GetBusinessUnitAdminUser(properties, settings, template.ID);
            if (properties.BusinessUnitAdmin == null || string.IsNullOrEmpty(properties.BusinessUnitAdmin.LoginName))
            {
                LogHelper.LogInformation(string.Format("Invalid Business Unit Admin or No Business Unit Admin found for the site request - {0}", properties.Url), LogEventID.InformationWrite);
                string statusMessage = string.Format("{0} - {1}", "Invalid BU Admin or No BU Admin found.", DateTime.Now.ToString());
                var requestManager = this.requestFactory.GetSiteRequestManager();
                requestManager.UpdateRequestStatus(properties.ListItemId, SiteRequestStatus.Exception, statusMessage, this.settings.SiteRequestListName);
                return false;
            }
            
            creationStatus = this.CreateSiteCollection(properties, template, templateManager);

            if (!creationStatus)
            {
                return false;
            }

            this.HandleDefaultGroups(properties, template);
            this.SetAdministrators(properties, template, settings);
            this.SetSiteDescription(properties);
            this.DisableSPDSettings(properties);           
            this.AddInitialSiteMembers(properties);
            this.ActiveFeatures(properties, template);
            this.SetSitePropertyBag(properties, template);
            this.ApplyBrandingToWeb(properties, template, templateManager);
            this.DeployCustomActions(properties, templateManager, template);
            this.ProvisionListInstances(properties, template, templateManager);
            this.AddListWebPartsToLandingPage(properties, template, templateManager);

            if (template.RootTemplate == JCI.CAM.Common.Constants.SiteTemplateIdCommunitySite || template.RootTemplate == JCI.CAM.Common.Constants.PartnerSiteTemplateIdCommunitySite)
            {
                this.PerformCommunitySiteConfigurations(properties);
            }
            else
            {
                this.SetSiteSecurity(properties);
            }

            if (template.RootTemplate.StartsWith(JCI.CAM.Common.Constants.PartnerSiteTemplateIdStartsWith))
            {
                this.ConfigurePartnerSiteCollection(properties);
            }

            if (properties.IsTestSite)
            {
                this.ApplySitePolicy(properties.Url, template.TestSitePolicy);
            }
            else
            {
                this.ApplySitePolicy(properties.Url, template.SitePolicy);
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnPremProvisioningService.ProvisionSite completed", LogEventID.InformationWrite);
            return true;
        }

        /// <summary>
        /// Checks the site exist.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Return true/false on site status</returns>
        private bool CheckSiteExist(SiteRequestInformation properties)
        {
            bool siteExist = false;
            this.UsingContext(
                ctx =>
                    {
                        LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnPremProvisioningService.CheckSiteExist - Context authenticated", LogEventID.InformationWrite);
                        Tenant tenantSite = new Tenant(ctx);

                        try
                        {
                            var site = tenantSite.GetSiteByUrl(properties.Url);
                            ctx.ExecuteQuery();

                            if (site != null && site.ServerObjectIsNull == false)
                            {
                                LogHelper.LogInformation(string.Format("Site already exists and will not be provisioned - {0}", properties.Url), LogEventID.InformationWrite);
                                siteExist = true;
                                string statusMessage = string.Format("{0} - {1}", "AlreadyExists", DateTime.Now.ToString());
                                var requestManager = this.requestFactory.GetSiteRequestManager();
                                requestManager.UpdateRequestStatus(properties.ListItemId, SiteRequestStatus.Exception, statusMessage, this.settings.SiteRequestListName);
                            }
                        }
                        catch (ServerException ex)
                        {
                            if (ex.ServerErrorTypeName == "Microsoft.Online.SharePoint.Common.SpoNoSiteException")
                            {
                                LogHelper.LogInformation(string.Format("Site Existance Check - Does not exist, Creating - {0}", properties.Url), LogEventID.InformationWrite);
                            }
                            else if (ex.Message.StartsWith("Unable to access site:"))
                            {
                                LogHelper.LogInformation(string.Format("Site already exists in Recycle bin and will not be provisioned - {0}", properties.Url), LogEventID.InformationWrite);
                                siteExist = true;
                                string statusMessage = string.Format("{0} - {1}", "AlreadyExists (RecycleBin)", DateTime.Now.ToString());
                                var requestManager = this.requestFactory.GetSiteRequestManager();
                                requestManager.UpdateRequestStatus(properties.ListItemId, SiteRequestStatus.Exception, statusMessage, this.settings.SiteRequestListName);
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format("Site Existance Check Issue details, site - {0}, Message - {1}, Stacktrace - {2}", properties.Url, ex.Message, ex.StackTrace), LogEventID.InformationWrite);
                                LogHelper.LogInformation(string.Format("Site Existance Check - Site Unavailable and issue unknown. Refer above stack trace for further invistigation. Will try creating site - {0}", properties.Url), LogEventID.InformationWrite);
                            }
                        }
                    },
                    1200000,
                    properties.Url);

            return siteExist;
        }

        /// <summary>
        /// Creates the site collection.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="template">The template.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>
        /// Site Creation Status
        /// </returns>
        private bool CreateSiteCollection(SiteRequestInformation properties, Template template, TemplateManager manager)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnPremProvisioningService.CreateSiteCollection - Creating site collection", LogEventID.InformationWrite);
            bool creationStatus = false;

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return false;
            }

            bool siteExist = this.CheckSiteExist(properties);
            if (!siteExist)
            {
                this.UsingContext(
                    ctx =>
                         {
                             LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnPremProvisioningService.CreateSiteCollection - Context authenticated", LogEventID.InformationWrite);

                             Tenant tenantSite = new Tenant(ctx);

                             var newSite = new SiteCreationProperties
                             {
                                 Title = properties.Title,
                                 Url = properties.Url,
                                 Owner = properties.RequestorName,
                                 Template = template.RootTemplate,
                                 Lcid = properties.Lcid,
                                 TimeZoneId = properties.TimeZoneId,
                                 StorageMaximumLevel = template.StorageMaximumLevel,
                                 StorageWarningLevel = template.StorageWarningLevel,
                                 UserCodeMaximumLevel = template.UserCodeMaximumLevel,
                                 UserCodeWarningLevel = template.UserCodeWarningLevel
                             };

                             tenantSite.CreateSite(newSite);
                             ctx.ExecuteQuery();
                             try
                             {
                                 tenantSite.GetSiteByUrl(properties.Url);
                                 LogHelper.LogInformation(string.Format("Site Collection {0} created", properties.Url), LogEventID.InformationWrite);
                             }
                             catch (Exception ex)
                             {
                                 LogHelper.LogError(ex);
                                 LogHelper.LogInformation("Kept wait time to complete site creation finalization process", LogEventID.InformationWrite);
                                 System.Threading.Thread.Sleep(60000);
                             }

                             creationStatus = true;
                         },
                         1200000,
                         properties.Url);
            }

            return creationStatus;
        }
    }
}
