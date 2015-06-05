// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineProvisioningService.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Provisioning Implementation class for Office 365 Site Collection
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core.Data
{   
    using System;
    using System.Globalization;    
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.TemplateEntites;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using CommonConstants = JCI.CAM.Common;

    /// <summary>
    /// Site Provisioning Implementation class for Office 365 Site Collection
    /// </summary>
    public class OnlineProvisioningService : AbstractProvisioningService
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
        /// Initializes a new instance of the <see cref="OnlineProvisioningService"/> class.
        /// </summary>
        public OnlineProvisioningService()
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
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.ProvisionSite", LogEventID.InformationWrite);
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

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.ProvisionSite completed", LogEventID.InformationWrite);
            return true;
        }

        /// <summary>
        /// Determines if the Default SharePoint Groups should be created or not. With on premises builds
        /// default groups are not created during the site provisioning.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="template">The template.</param>
        protected override void HandleDefaultGroups(SiteRequestInformation properties, Template template)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.HandleDefaultGroups", LogEventID.InformationWrite);

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return;
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.HandleDefaultGroups - Default Groups for site {0} creation started for on premise", LogEventID.InformationWrite);

            string ownerGroupFormat = "{0} Owners";
            string memberGroupFormat = "{0} Members";
            string visitorGroupFormat = "{0} Visitors";
            string ownerGroupDisplayName = string.Format(ownerGroupFormat, properties.Title);
            string memberGroupDisplayName = string.Format(memberGroupFormat, properties.Title);
            string vistorGroupDisplayName = string.Format(visitorGroupFormat, properties.Title);
            string moderatorsGroupDisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", properties.Title, CommonConstants.Constants.ModeratorsGroupName);
            bool isAdminAdded = false;
            bool isMemberAdded = false;
            bool isVisitorAdded = false;
            bool isModeratorAdded = false;

            this.UsingContext(
                ctx =>
                {
                    Tenant tenant = new Tenant(ctx);
                    var site = tenant.GetSiteByUrl(properties.Url);

                    var web = site.RootWeb;

                    ctx.Load(web.AssociatedOwnerGroup);
                    ctx.Load(web.AssociatedMemberGroup);
                    ctx.Load(web.AssociatedVisitorGroup);
                    ctx.ExecuteQuery();
                    Group ownerGroup;
                    Group memberGroup;
                    Group visitorGroup;

                    if (web.AssociatedOwnerGroup == null || web.AssociatedOwnerGroup.ServerObjectIsNull == true)
                    {
                        ownerGroup = web.AddGroup(ownerGroupDisplayName, string.Format("Use this group to grant people full control permissions to the SharePoint site: {0} ", properties.Title), true, false);
                        web.AssociatedOwnerGroup = ownerGroup;
                        web.AssociatedOwnerGroup.Update();
                        web.Update();
                        ctx.ExecuteQuery();
                        isAdminAdded = true;
                        web.AssociatedOwnerGroup.Owner = ownerGroup;
                        web.AssociatedOwnerGroup.Update();
                        ctx.ExecuteQuery();
                    }
                    else
                    {
                        ownerGroup = web.AssociatedOwnerGroup;
                    }

                    if (web.AssociatedMemberGroup == null || web.AssociatedMemberGroup.ServerObjectIsNull == true)
                    {
                        memberGroup = web.AddGroup(memberGroupDisplayName, string.Format("Use this group to grant people contribute permissions to the SharePoint site: {0}", properties.Title), false, false);
                        web.AssociatedMemberGroup = memberGroup;
                        web.AssociatedMemberGroup.Update();
                        web.Update();
                        ctx.ExecuteQuery();
                        isMemberAdded = true;
                        web.AssociatedMemberGroup.Owner = web.AssociatedOwnerGroup;
                        web.AssociatedMemberGroup.Update();
                        ctx.ExecuteQuery();
                    }
                    else
                    {
                        memberGroup = web.AssociatedMemberGroup;
                    }

                    if (web.AssociatedVisitorGroup == null || web.AssociatedVisitorGroup.ServerObjectIsNull == true)
                    {
                        visitorGroup = web.AddGroup(vistorGroupDisplayName, string.Format("Use this group to grant people read permissions to the SharePoint site: {0}", properties.Title), false, false);
                        web.AssociatedVisitorGroup = visitorGroup;
                        web.AssociatedVisitorGroup.Update();
                        web.Update();
                        ctx.ExecuteQuery();
                        isVisitorAdded = true;
                        web.AssociatedVisitorGroup.Owner = web.AssociatedOwnerGroup;
                        web.AssociatedVisitorGroup.Update();
                        ctx.ExecuteQuery();
                    }
                    else
                    {
                        visitorGroup = web.AssociatedVisitorGroup;
                    }

                    if (template.RootTemplate == JCI.CAM.Common.Constants.SiteTemplateIdCommunitySite || template.RootTemplate == JCI.CAM.Common.Constants.PartnerSiteTemplateIdCommunitySite)
                    {
                        Group moderatorGroup = null;
                        try
                        {
                            moderatorGroup = web.SiteGroups.GetByName(moderatorsGroupDisplayName);
                            ctx.Load(moderatorGroup);
                            ctx.ExecuteQuery();
                        }
                        catch (ServerException ex)
                        {
                            moderatorGroup = null;
                            LogHelper.LogError(ex, LogEventID.ExceptionHandling, string.Format("Group - {0} does not exist and will get created.", moderatorsGroupDisplayName));
                        }

                        if (moderatorGroup == null || moderatorGroup.ServerObjectIsNull == true)
                        {
                            moderatorGroup = web.AddGroup(moderatorsGroupDisplayName, string.Format("Use this group to grant people moderate permissions to the SharePoint site: {0}", properties.Title), false, false);
                            ctx.ExecuteQuery();
                            isModeratorAdded = true;
                            moderatorGroup.Owner = web.AssociatedOwnerGroup;
                            moderatorGroup.Update();
                            ctx.ExecuteQuery();
                        }
                    }

                    ConfigureGroupPermissions(ownerGroupDisplayName, memberGroupDisplayName, vistorGroupDisplayName, moderatorsGroupDisplayName, isAdminAdded, isMemberAdded, isVisitorAdded, isModeratorAdded, ctx, web);

                    LogHelper.LogInformation(string.Format("Setting group Security Permissions for {0}, {1}, {2} completed.", ownerGroupDisplayName, memberGroupDisplayName, vistorGroupDisplayName), LogEventID.InformationWrite);
                },
            1200000,
            properties.Url);
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
                    LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.CheckSiteExist - Context authenticated", LogEventID.InformationWrite);
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
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.CreateSiteCollection - Creating site collection", LogEventID.InformationWrite);
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
                        LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.CreateSiteCollection - Context authenticated", LogEventID.InformationWrite);

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

                        // Create and Wait
                        SpoOperation spo = tenantSite.CreateSite(newSite);
                        ctx.Load(spo, i => i.IsComplete);
                        ctx.ExecuteQuery();

                        while (!spo.IsComplete)
                        {
                            // Wait and then try again
                            System.Threading.Thread.Sleep(300);
                            ctx.Load(spo, i => i.IsComplete);
                            ctx.ExecuteQuery();
                        }

                        creationStatus = true;
                        LogHelper.LogInformation(string.Format("Site Collection {0} created", properties.Url), LogEventID.InformationWrite);
                    },
                1200000,
                properties.Url);
            }

            return creationStatus;
        }
    }
}