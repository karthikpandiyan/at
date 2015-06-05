// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractProvisioningService.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Abstract Site Provisioning Service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.Models;
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.Entity;
    using JCI.CAM.Provisioning.Core.TemplateEntites;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using CommonConstants = JCI.CAM.Common;

    /// <summary>
    /// Abstract Site Provisioning Service
    /// </summary>
    public abstract class AbstractProvisioningService : IProvisioningService, ISharePointService
    {
        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        /// <value>
        /// The authentication.
        /// </value>
        public IAuthentication Authentication
        {
            get;
            set;
        }

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
        public abstract bool? ProvisionSite(SiteRequestInformation properties, Template template, TemplateManager manager, AppSettings settings);

        /// <summary>
        /// Delegate that is used to handle creation of ClientContext that is authenticated
        /// </summary>
        /// <param name="action">Authenticated ClientContext.</param>
        public void UsingContext(Action<ClientContext> action)
        {
            this.UsingContext(action, Timeout.Infinite);
        }

        /// <summary>
        /// Delegate that is used to handle creation of ClientContext that is authenticated
        /// </summary>
        /// <param name="action">Authenticated ClientContext.</param>
        /// <param name="csomTimeout">The CSOM timeout.</param>
        public void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext context = this.Authentication.GetAuthenticatedContext())
            {
                context.RequestTimeout = csomTimeout;
                action(context);
            }
        }

        /// <summary>
        /// Usings the context.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="csomTimeout">The CSOM timeout.</param>
        /// <param name="siteUrl">The site URL.</param>     
        public virtual void UsingContext(Action<ClientContext> action, int csomTimeout, string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                using (ClientContext ctx = this.Authentication.GetAuthenticatedContextForGivenUrl(siteUrl))
                {
                    ctx.RequestTimeout = csomTimeout;
                    action(ctx);
                }
            }
            else
            {
                this.UsingContext(action, csomTimeout);
            }
        }

        /// <summary>
        /// Configures the group permissions.
        /// </summary>
        /// <param name="ownerGroupDisplayName">Display name of the owner group.</param>
        /// <param name="memberGroupDisplayName">Display name of the member group.</param>
        /// <param name="visitorGroupDisplayName">Display name of the visitor group.</param>
        /// <param name="moderatorsGroupDisplayName">Display name of the moderators group.</param>
        /// <param name="isAdminAdded">if set to <c>true</c> [is admin added].</param>
        /// <param name="isMemberAdded">if set to <c>true</c> [is member added].</param>
        /// <param name="isVisitorAdded">if set to <c>true</c> [is visitor added].</param>
        /// <param name="isModeratorAdded">if set to <c>true</c> [is moderator added].</param>
        /// <param name="ctx">The CTX.</param>
        /// <param name="web">The web.</param>
        protected static void ConfigureGroupPermissions(string ownerGroupDisplayName, string memberGroupDisplayName, string visitorGroupDisplayName, string moderatorsGroupDisplayName, bool isAdminAdded, bool isMemberAdded, bool isVisitorAdded, bool isModeratorAdded, ClientContext ctx, Web web)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.ConfigureGroupPermissions - Default Groups for site {0} created:", LogEventID.InformationWrite);

            bool executeQuery = false;

            if (isAdminAdded)
            {
                web.AddPermissionLevelToGroup(ownerGroupDisplayName, RoleType.Administrator);
                executeQuery = true;
            }

            if (isMemberAdded)
            {
                web.AddPermissionLevelToGroup(memberGroupDisplayName, RoleType.Editor);
                executeQuery = true;
            }

            if (isVisitorAdded)
            {
                web.AddPermissionLevelToGroup(visitorGroupDisplayName, RoleType.Reader);
                executeQuery = true;
            }

            if (isModeratorAdded)
            {
                web.AddPermissionLevelToGroup(moderatorsGroupDisplayName, RoleType.WebDesigner);
                executeQuery = true;
            }

            if (executeQuery)
            {
                ctx.ExecuteQuery();
            }

            LogHelper.LogInformation(string.Format("Setting group Security Permissions for {0}, {1}, {2} completed in ConfigureGroupPermissions.", ownerGroupDisplayName, memberGroupDisplayName, visitorGroupDisplayName), LogEventID.InformationWrite);
        }      

        /// <summary>
        /// Determines if the Default SharePoint Groups should be created or not. With on premises builds
        /// default groups are not created during the site provisioning.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="template">The template.</param>
        protected virtual void HandleDefaultGroups(SiteRequestInformation properties, Template template)
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
                tenantCtx =>
            {
                Tenant tenant = new Tenant(tenantCtx);
                var site = tenant.GetSiteByUrl(properties.Url);

                using (var ctx = site.Context.Clone(properties.Url))
                {
                    var web = ctx.Web;

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
                }

                LogHelper.LogInformation(string.Format("Setting group Security Permissions for {0}, {1}, {2} completed.", ownerGroupDisplayName, memberGroupDisplayName, vistorGroupDisplayName), LogEventID.InformationWrite);
            }, 
            1200000,
            properties.Url);
        }

        /// <summary>
        /// Disables the SPD settings.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected virtual void DisableSPDSettings(SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.DisableSPDSettings", LogEventID.InformationWrite);

            try
            {
                if (properties == null)
                {
                    LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                    return;
                }

                this.UsingContext(
                    tenantCtx =>
                    {
                        Tenant tenant = new Tenant(tenantCtx);
                        var site = tenant.GetSiteByUrl(properties.Url);

                        using (var ctx = site.Context.Clone(properties.Url))
                        {
                            var currentSite = ctx.Site;
                            BrandingExtensions.DisableDesigner(currentSite);
                            ctx.ExecuteQuery();
                        }
                    },
              1200000,
              properties.Url);

                LogHelper.LogInformation("Settings to Disable SPDS ettings completed", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }
        }

        /// <summary>
        /// Sets the Description of the Site Collection.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected virtual void SetSiteDescription(SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.SetSiteDescription", LogEventID.InformationWrite);

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return;
            }

            this.UsingContext(
                tenantCtx =>
          {
              Tenant tenant = new Tenant(tenantCtx);
              var site = tenant.GetSiteByUrl(properties.Url);

              using (var ctx = site.Context.Clone(properties.Url))
              {
                  var web = ctx.Web;
                  web.Description = properties.Description;
                  web.Update();
                  ctx.ExecuteQuery();
              }
                },
          1200000,
          properties.Url);

            LogHelper.LogInformation(string.Format("Setting Site Description {0}: for site {1} completed", properties.Description, properties.Url), LogEventID.InformationWrite);
        }

        /// <summary>
        /// Sets the Sit Administrators
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="template">The template.</param>
        /// <param name="settings">The settings.</param>
        protected virtual void SetAdministrators(SiteRequestInformation properties, Template template, AppSettings settings)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.SetAdministrators", LogEventID.InformationWrite);

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return;
            }

            this.UsingContext(
                tenantCtx =>
                {
                    Tenant tenant = new Tenant(tenantCtx);
                    var site = tenant.GetSiteByUrl(properties.Url);

                    using (var ctx = site.Context.Clone(properties.Url))
                    {
                        var web = ctx.Web;
                        var spOwner = web.EnsureUser(properties.SiteOwner.LoginName);
                        web.AssociatedOwnerGroup.Users.AddUser(spOwner);
                        web.AssociatedOwnerGroup.Update();
                       
                        ctx.ExecuteQuery();

                        // RequestAccessEmail cannot be set as it is not supported in CSOM
                        // Additional admins
                        Microsoft.SharePoint.Client.User owner = null;

                        if (properties.SecondaryAdminUser != null && !string.IsNullOrEmpty(properties.SecondaryAdminUser.LoginName))
                        {
                            owner = web.EnsureUser(properties.SecondaryAdminUser.LoginName);

                            if (owner != null)
                            {
                                Group userGroup = web.AssociatedOwnerGroup;
                                AddUsersToGroup(ctx, owner, userGroup);
                            }
                        }

                        owner = null;

                        if (properties.TertiaryAdminUser != null && !string.IsNullOrEmpty(properties.TertiaryAdminUser.LoginName))
                        {
                            owner = web.EnsureUser(properties.TertiaryAdminUser.LoginName);

                            if (owner != null)
                            {
                                Group userGroup = web.AssociatedOwnerGroup;
                                AddUsersToGroup(ctx, owner, userGroup);
                            }
                        }
                    }
                },
           1200000,
           properties.Url);

            this.SetBusinessUnitAdmins(properties);

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.SetAdministrators complete", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Active OOB features
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="template">The template.</param>
        protected void ActiveFeatures(SiteRequestInformation info, Template template)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.ActiveFeatures - Activating features for site collection", LogEventID.InformationWrite);

            if (info == null || template == null)
            {
                LogHelper.LogInformation("Method not executed as info object is null", LogEventID.InformationWrite);
                return;
            }

            if (template.ConfigurationContainer != null && template.ConfigurationContainer.Features != null && template.ConfigurationContainer.Features.Count > 0)
            {
                this.UsingContext(
                    tenantCtx =>
           {
               Tenant tenant = new Tenant(tenantCtx);
               var site = tenant.GetSiteByUrl(info.Url);

               using (var ctx = site.Context.Clone(info.Url))
               {
                   var web = ctx.Web;
                   ctx.ExecuteQuery();

                   foreach (var feature in template.ConfigurationContainer.Features)
                   {
                       if (feature.Activate)
                       {
                           var featureGUID = new Guid(feature.ID);
                           LogHelper.LogInformation(string.Format("Activating Feature Id {0}", feature.ID), LogEventID.InformationWrite);
                           if (feature.Scope.Equals("Site", StringComparison.OrdinalIgnoreCase))
                           {
                               this.ActivateSiteFeature(site, info.Url, featureGUID);
                           }

                           if (feature.Scope.Equals("Web", StringComparison.OrdinalIgnoreCase))
                           {
                               this.ActivateWebFeature(web, info.Url, featureGUID);
                           }
                       }
                   }
               }
                    },
           1200000,
           info.Url);
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.ActiveFeatures - Activating features for site collection completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Activates the site feature.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="url">The URL.</param>
        /// <param name="featureID">The feature identifier.</param>
        protected virtual void ActivateSiteFeature(Site site, string url, Guid featureID)
        {
            if (!site.IsFeatureActive(featureID))
            {
                site.ActivateFeature(featureID);
                LogHelper.LogInformation(string.Format("Site features {0} activated.", featureID), LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format("Site features {0} already activated.", featureID), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Activate web features.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="url">The URL.</param>
        /// <param name="featureID">The feature identifier.</param>
        protected virtual void ActivateWebFeature(Web web, string url, Guid featureID)
        {
            if (!web.IsFeatureActive(featureID))
            {
                web.ActivateFeature(featureID);
                LogHelper.LogInformation(string.Format("Web features {0} activated.", featureID), LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format("Web features {0} already activated.", featureID), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Deploys the custom actions.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="template">The template.</param>
        protected void DeployCustomActions(SiteRequestInformation info, TemplateManager manager, Template template)
        {
            LogHelper.LogInformation(string.Format("Applying Custom Actions to Web {0}", info.Url), LogEventID.InformationWrite);

            if (template.ConfigurationContainer.CustomActions != null && template.ConfigurationContainer.CustomActions.Count > 0)
            {
                var customActions = manager.GetCustomActions();

                if (customActions != null && customActions.Count > 0)
                {
                    List<CustomActionEntity> actionEntities = new List<CustomActionEntity>();

                    foreach (var action in template.ConfigurationContainer.CustomActions)
                    {
                        CustomAction customAction = customActions.FirstOrDefault(t => t.Name == action.Name);

                        if (customAction != null)
                        {
                            var actionEntity = new CustomActionEntity()
                            {
                                Name = customAction.Name,
                                Title = customAction.Title,
                                Description = customAction.Description,
                                Group = customAction.Group,
                                Location = customAction.Location,
                                Sequence = customAction.Sequence,
                                Url = string.Format(customAction.Url, info.Url),
                                ScriptSrc = customAction.ScriptSrc,
                                ScriptBlock = string.IsNullOrEmpty(customAction.ScriptBlock) ? string.Empty : WebUtility.HtmlDecode(customAction.ScriptBlock)
                            };

                            // PNP extension calls execute query 
                            if (!action.ApplyOnlyToTestSite)
                            {
                                actionEntities.Add(actionEntity);
                            }
                            else
                            {
                                if (info.IsTestSite)
                                {
                                    actionEntities.Add(actionEntity);
                                }
                            }
                        }
                    }

                    this.DeployCustomActions(info.Url, actionEntities);
                }
            }

            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Custom Actions deployment completed {0}", info.Url), LogEventID.InformationWrite);
        }

        /// <summary>
        /// Deploys the custom actions.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="customActions">The custom actions.</param>
        protected virtual void DeployCustomActions(string url, List<CustomActionEntity> customActions)
        {
            if (customActions.Count > 0)
            {
                this.UsingContext(
                    tenantCtx =>
           {
               Tenant tenant = new Tenant(tenantCtx);
               var site = tenant.GetSiteByUrl(url);

               using (var ctx = site.Context.Clone(url))
               {
                   var web = ctx.Web;

                   foreach (var customAction in customActions)
                   {
                       // PNP extension calls execute query 
                       web.AddCustomAction(customAction);
                   }
               }
           },
           1200000,
           url);
            }
        }

        /// <summary>
        /// Applies the branding to web.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="template">The template.</param>
        /// <param name="manager">The manager.</param>
        protected void ApplyBrandingToWeb(SiteRequestInformation info, Template template, TemplateManager manager)
        {
            LogHelper.LogInformation(string.Format("Applying Branding to Web {0}", info.Url), LogEventID.InformationWrite);

            if (template == null)
            {
                LogHelper.LogInformation("Method not executed as info and template objects are null", LogEventID.InformationWrite);
                return;
            }

            var themePack = manager.GetThemePackageByName(template.ThemePackageName);

            if (themePack != null)
            {
                LogHelper.LogInformation(string.Format("Retreived ThemePackage {0} for site {1}", template.ThemePackageName, info.Url), LogEventID.InformationWrite);
                this.DeployTheme(info.Url, themePack);
            }
            else
            {
                LogHelper.LogInformation(string.Format("There is no theme package defined for template {0}", template.Title), LogEventID.InformationWrite);
            }

            LogHelper.LogInformation("Applying Branding to Web completed", LogEventID.InformationWrite);
        }       

        #region Branding

        /// <summary>
        /// Deploys the theme.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="theme">The theme.</param>
        protected virtual void DeployTheme(string url, ThemePackage theme)
        {
            LogHelper.LogInformation("Deploying Theme files", LogEventID.InformationWrite);

            this.UsingContext(
                tenantCtx =>
                {
                    Tenant tenant = new Tenant(tenantCtx);
                    var site = tenant.GetSiteByUrl(url);

                    using (var ctx = site.Context.Clone(url))
                    {
                        // Get root web
                        var siteWeb = ctx.Web;
                        ctx.Load(siteWeb);
                        ctx.ExecuteQuery();

                        if (!string.IsNullOrEmpty(theme.ColorFile))
                        {
                            siteWeb.UploadThemeFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, theme.ColorFile));
                            LogHelper.LogInformation(string.Format("Completed Uploading Color File {0}: for site {1}", theme.ColorFile, url), LogEventID.InformationWrite);
                        }

                        if (!string.IsNullOrEmpty(theme.FontFile))
                        {
                            siteWeb.UploadThemeFile(AppDomain.CurrentDomain.BaseDirectory, theme.FontFile);
                            LogHelper.LogInformation(string.Format("Completed Uploading Font File {0}: for site {1}", theme.FontFile, url), LogEventID.InformationWrite);
                        }

                        if (!string.IsNullOrEmpty(theme.BackgroundFile))
                        {
                            siteWeb.UploadThemeFile(AppDomain.CurrentDomain.BaseDirectory, theme.BackgroundFile);
                            LogHelper.LogInformation(string.Format("Completed Uploading BackgroundFile {0}: for site {1}", theme.BackgroundFile, url), LogEventID.InformationWrite);
                        }

                        // bool isRootWeb = siteWeb.ServerRelativeUrl == site.ServerRelativeUrl;
                        siteWeb.CreateComposedLookByName(theme.Name, theme.ColorFile, theme.FontFile, theme.BackgroundFile, theme.MasterPage);
                        LogHelper.LogInformation(string.Format("Created Composed look {0}: for site {1}", theme.Name, url), LogEventID.InformationWrite);
                        siteWeb.SetComposedLookByUrl(theme.Name);
                        LogHelper.LogInformation(string.Format("Setting theme {0}: for site {1}", theme.Name, url), LogEventID.InformationWrite);
                        try
                        {
                            this.ApplyCss(siteWeb, theme);
                            this.ApplySiteLogo(siteWeb, theme);                            
                        }
                        catch (Exception ex)
                        {
                            // Ignoring exception
                            LogHelper.LogInformation("Due to API limitations, AlternateCSS and SiteLogo options may not work in dedicated environment. This options can be used in vNext.", LogEventID.InformationWrite);
                            LogHelper.LogError(ex);
                        }

                        this.SetSitePropertyBag(siteWeb, "_sp_branding_version", theme.Name);
                        this.SetSitePropertyBag(siteWeb, "_sp_branding_themename", theme.Version);
                    }
                },
           1200000,
           url);
        }

        /// <summary>
        /// Applies the CSS.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="theme">The theme.</param>
        protected virtual void ApplyCss(Web web, ThemePackage theme)
        {
            LogHelper.LogInformation("Deploying and configuring  CSS files", LogEventID.InformationWrite);

            if (!string.IsNullOrEmpty(theme.AlternateCSS))
            {
                List assetLibrary = web.GetListByTitle("Site Assets");
                web.Context.Load(assetLibrary, l => l.RootFolder);

                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, theme.AlternateCSS);

                // Get File info
                System.IO.FileInfo cssFile = new System.IO.FileInfo(filePath);
                FileCreationInformation newFile = new FileCreationInformation
                {
                    Content = System.IO.File.ReadAllBytes(filePath),
                    Url = cssFile.Name,
                    Overwrite = true
                };
                Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
                web.Context.Load(uploadFile);
                web.Context.ExecuteQuery();
                string cssUrl = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, "SiteAssets", cssFile.Name);
                web.AlternateCssUrl = cssUrl;
                web.Update();
                web.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Applies the site logo.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="theme">The theme.</param>
        protected virtual void ApplySiteLogo(Web web, ThemePackage theme)
        {
            LogHelper.LogInformation("Deploying and configuring  Site Logo", LogEventID.InformationWrite);

            if (!string.IsNullOrEmpty(theme.SiteLogo))
            {
                List assetLibrary = web.GetListByTitle("Site Assets");
                web.Context.Load(assetLibrary, l => l.RootFolder);

                // Get File Info
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, theme.SiteLogo);

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                FileCreationInformation newFile = new FileCreationInformation
                {
                    Content = System.IO.File.ReadAllBytes(filePath),
                    Url = fileInfo.Name,
                    Overwrite = true
                };
                Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
                web.Context.Load(uploadFile);
                web.Context.ExecuteQuery();
                LogHelper.LogInformation(string.Format("Uploaded Site Logo {0} to list {1} for site {2} ", fileInfo.Name, "SiteAssets", web.Url), LogEventID.InformationWrite);
                string siteLogoUrl = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, "SiteAssets", fileInfo.Name);
                web.SiteLogoUrl = siteLogoUrl;
                web.Update();
                web.Context.ExecuteQuery();
                LogHelper.LogInformation(string.Format("Setting Site Logo {0}: for site {1}", fileInfo.Name, web.Url), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Gets the business unit admin user.
        /// </summary>
        /// <param name="properties">Site Request properties.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="templateName">Name of the template.</param>
        protected void GetBusinessUnitAdminUser(SiteRequestInformation properties, AppSettings settings, string templateName)
        {
            try
            {
                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.GetBusinessUnitAdminUser - Getting BU Admin details", LogEventID.InformationWrite);

                string adminLookUpValue = string.Empty;
                string businessUnitAdminEmail = string.Empty;
                Dictionary<string, string> fieldsValue = null;

                AppOnlyAuthenticationSite siteAuthentication = new AppOnlyAuthenticationSite();
                using (ClientContext ctx = siteAuthentication.GetAuthenticatedContext(settings.SPHostUrl))
                {
                    var configWeb = ctx.Web;
                    fieldsValue = ConfigListHelper.GetBusinessAdminGroup(ctx, configWeb, properties.Region, properties.BusinessUnit, templateName);
                    if (fieldsValue != null)
                    {
                        fieldsValue.TryGetValue(ConfigListHelper.BusinessUnitAdminFieldValue, out adminLookUpValue);
                        fieldsValue.TryGetValue(ConfigListHelper.BusinessUnitAdminEmailFieldValue, out businessUnitAdminEmail);
                    }

                    if (!string.IsNullOrEmpty(adminLookUpValue))
                    {
                        var web = ctx.Web;
                        Microsoft.SharePoint.Client.User user = web.EnsureUser(adminLookUpValue);
                        ctx.Load(user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                        ctx.ExecuteQuery();

                        if (user != null)
                        {
                            SharePointUser adminUser = new SharePointUser
                            {
                                LoginName = user.Title.Contains("\\") ? user.Title : user.LoginName,
                                Email = string.IsNullOrEmpty(user.Email) ? businessUnitAdminEmail : user.Email,
                                Name = user.Title
                            };

                            properties.BusinessUnitAdmin = adminUser;
                        }
                    }
                }

                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.AbstractProvisioningService.GetBusinessUnitAdminUser - Completed", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        #endregion

        #region List
        /// <summary>
        /// Provisions the list instances.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="template">The template.</param>
        /// <param name="templateManager">The template manager.</param>
        protected void ProvisionListInstances(SiteRequestInformation properties, Template template, TemplateManager templateManager)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.ProvisionListInstances", LogEventID.InformationWrite);

            if (properties == null || template == null)
            {
                LogHelper.LogInformation("Method not executed as properties or template objects are null", LogEventID.InformationWrite);
                return;
            }

            var listDefinitions = templateManager.GetListDefinitions();

            if (template.ConfigurationContainer != null && template.ConfigurationContainer.ListInstances != null && template.ConfigurationContainer.ListInstances.Count > 0)
            {
                this.UsingContext(
                    tenantCtx =>
           {
               Tenant tenant = new Tenant(tenantCtx);
               var site = tenant.GetSiteByUrl(properties.Url);

               using (var ctx = site.Context.Clone(properties.Url))
               {
                   var siteWeb = ctx.Web;
                   ctx.Load(siteWeb);
                   ctx.ExecuteQuery();

                   foreach (var listInstance in template.ConfigurationContainer.ListInstances)
                   {
                       var listDefinition = listDefinitions.FindAll(c => c.Title == listInstance.CustomListDefinitionName).FirstOrDefault();
                       this.ProvisionListInstance(ctx, siteWeb, listDefinition, listInstance);
                   }
               }
           },
           1200000,
           properties.Url);
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.ProvisionListInstances Completed.", LogEventID.InformationWrite);
        }
        #endregion

        #region webpart

        /// <summary>
        /// Adds the web parts to landing page.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="template">The template.</param>
        /// <param name="manager">The manager.</param>
        protected void AddListWebPartsToLandingPage(SiteRequestInformation info, Template template, TemplateManager manager)
        {
            LogHelper.LogInformation(string.Format("Adding webparts to Web {0}", info.Url), LogEventID.InformationWrite);

            if (template.ConfigurationContainer.DeploymentModules != null && template.ConfigurationContainer.DeploymentModules.Count > 0)
            {
                this.UsingContext(
                    tenantCtx =>
            {
                Tenant tenant = new Tenant(tenantCtx);
                var site = tenant.GetSiteByUrl(info.Url);

                using (var ctx = site.Context.Clone(info.Url))
                {
                    var web = ctx.Web;
                    ctx.Load(web, r => r.ServerRelativeUrl);
                    ctx.ExecuteQuery();

                    foreach (var deployementModule in template.ConfigurationContainer.DeploymentModules)
                    {
                        foreach (var deploymentFile in deployementModule.Files)
                        {
                            WebPartEntity webPartEntity = new WebPartEntity();
                            string listTitle = string.Empty;
                            foreach (var property in deploymentFile.Properties)
                            {
                                if (string.Compare(property.Name, "ZoneId", StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    webPartEntity.WebPartZone = property.Value;
                                }

                                if (string.Compare(property.Name, "ZoneIndex", StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    webPartEntity.WebPartIndex = Convert.ToInt32(property.Value, CultureInfo.InvariantCulture);
                                }

                                if (string.Compare(property.Name, "Title", StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    webPartEntity.WebPartTitle = property.Value;
                                }

                                if (string.Compare(property.Name, "ListTitle", StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    listTitle = property.Value;
                                }

                                if (string.Compare(property.Name, "Row", StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    webPartEntity.Row = Convert.ToInt32(property.Value, CultureInfo.InvariantCulture);
                                }

                                if (string.Compare(property.Name, "Column", StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    webPartEntity.Column = Convert.ToInt32(property.Value, CultureInfo.InvariantCulture);
                                }
                            }

                            try
                            {
                                // Get List from web
                                if (web.ListExists(listTitle))
                                {
                                    var list = web.GetListByTitle(listTitle);

                                    webPartEntity.WebPartXml = string.Format(deployementModule.ListViewWebPart, list.Title, list.Id);
                                    string landingPageUrl = string.Empty;
                                    if (!string.IsNullOrEmpty(deployementModule.Url))
                                    {
                                        landingPageUrl = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, deployementModule.Url, deploymentFile.Url);
                                    }
                                    else
                                    {
                                        landingPageUrl = string.Format("{0}/{1}", web.ServerRelativeUrl, deploymentFile.Url);
                                    }

                                    web.AddWebPartToWebPartPage(landingPageUrl, webPartEntity);
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Successfully added List WebPart {0} at web site:  {1}", listTitle, info.Url), LogEventID.InformationWrite);
                                }
                                else
                                {
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List {0} not found at web site:  {1}", listTitle, info.Url), LogEventID.InformationWrite);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.LogError(ex, LogEventID.InformationWrite);
                            }
                        }
                    }
                }
                    },
            1200000,
            info.Url);
            }

            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Adding webparts completed {0}", info.Url), LogEventID.InformationWrite);
        }
        #endregion

        #region sitepolicy

        /// <summary>
        /// Applies the site policy.
        /// </summary>
        /// <param name="siteUrl">The site URL.</param>
        /// <param name="policyName">Name of the policy.</param>
        protected virtual void ApplySitePolicy(string siteUrl, string policyName)
        {
            if (!string.IsNullOrEmpty(siteUrl) && !string.IsNullOrEmpty(policyName))
            {
                try
                {
                    this.SetSitePolicy(siteUrl, policyName);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling, string.Format("Exception while applying Site policy - {0}", policyName));
                }
            }
            else
            {
                LogHelper.LogInformation("Site Policy cannot be applied no policy name specified.", LogEventID.InformationWrite);
            }
        }
        #endregion

        #region Others
        /// <summary>
        /// Adds the everyone to visitors.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="ctx">Client Context</param>
        protected virtual void AddEveryoneToVisitors(SiteRequestInformation properties, ClientContext ctx)
        {
            var web = ctx.Web;
            this.AddEveryoneToVisitors(ctx, web, properties);
        }

        /// <summary>
        /// Adds the everyone to visitors.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="web">The web.</param>
        /// <param name="properties">The properties.</param>
        protected virtual void AddEveryoneToVisitors(ClientContext clientContext, Web web, SiteRequestInformation properties)
        {
            // Constants.EveryoneExceptExternalUsers = Everyone except external users            
            var allUsersGroup = web.EnsureUser(Constants.EveryoneUsers);
            web.AssociatedVisitorGroup.Users.AddUser(allUsersGroup);
            web.AssociatedVisitorGroup.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Configures the partner site collection.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected void ConfigurePartnerSiteCollection(SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService ConfigurePartnerSiteCollection", LogEventID.InformationWrite);

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return;
            }

            if (!string.IsNullOrEmpty(properties.CustomersTerms))
            {
                this.UsingContext(
                    tenantCtx =>
            {
                Tenant tenant = new Tenant(tenantCtx);
                var site = tenant.GetSiteByUrl(properties.Url);

                using (var ctx = site.Context.Clone(properties.Url))
                {
                    var web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQuery();

                    // Set partners site property bag
                    SetSitePropertyBag(web, Constants.SiteCustomersPropertyBagKey, properties.CustomersTerms);
                }
                    },
            1200000,
            properties.Url);
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService ConfigurePartnerSiteCollection completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Performs the community site configurations.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected void PerformCommunitySiteConfigurations(SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService PerformCommunitySiteConfigurations", LogEventID.InformationWrite);

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return;
            }

            try
            {
                this.UsingContext(
                    tenantCtx =>
           {
               Tenant tenant = new Tenant(tenantCtx);
               var site = tenant.GetSiteByUrl(properties.Url);

               using (var ctx = site.Context.Clone(properties.Url))
               {
                   if (properties.Moderator != null && !string.IsNullOrEmpty(properties.Moderator.LoginName))
                   {
                       string moderatorsGroupName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", properties.Title, CommonConstants.Constants.ModeratorsGroupName);
                       this.ConfigureCommunityModerators(moderatorsGroupName, properties, ctx);
                   }

                   this.ConfigureCommunitySiteTypeConfiguration(properties, ctx);
                   this.JoinCommunity(properties, ctx);

                   if (properties.InitialSiteUsers.Count > 0)
                   {
                       this.AddInitialCommunityMembers(properties, ctx);
                   }
               }
                    },
           1200000,
           properties.Url);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "Exception configuring Community.");
            }
            finally
            {
                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService PerformCommunitySiteConfigurations completed", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Adds the initial site members.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected void AddInitialSiteMembers(SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService AddInitialSiteMembers", LogEventID.InformationWrite);

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return;
            }

            if (properties.InitialSiteUsers.Count > 0)
            {
                this.UsingContext(
                    tenantCtx =>
          {
              Tenant tenant = new Tenant(tenantCtx);
              var site = tenant.GetSiteByUrl(properties.Url);

              using (var ctx = site.Context.Clone(properties.Url))
              {
                  var web = ctx.Web;

                  foreach (var initialMember in properties.InitialSiteUsers)
                  {
                      web.RefreshLoad();
                      ctx.ExecuteQuery();

                      if (initialMember != null && !string.IsNullOrEmpty(initialMember.LoginName))
                      {
                          Microsoft.SharePoint.Client.User initialUser = web.EnsureUser(initialMember.LoginName);
                          web.AssociatedVisitorGroup.Users.AddUser(initialUser);
                          web.AssociatedVisitorGroup.Update();
                          ctx.ExecuteQuery();
                      }
                  }
              }
                    },
          1200000,
          properties.Url);
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService AddInitialSiteMembers completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Sets the site security.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected void SetSiteSecurity(SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService SetSiteSecurity completed", LogEventID.InformationWrite);

            if (properties == null)
            {
                LogHelper.LogInformation("Method not executed as properties object is null", LogEventID.InformationWrite);
                return;
            }

            if (properties.ConfidentialityLevel == Constants.ConfidentialityLevelPublic || properties.ConfidentialityLevel == Constants.ConfidentialityLevelInternal)
            {
                this.UsingContext(
                    tenantCtx =>
          {
              Tenant tenant = new Tenant(tenantCtx);
              var site = tenant.GetSiteByUrl(properties.Url);
              using (var ctx = site.Context.Clone(properties.Url))
              {
                  this.AddEveryoneToVisitors(properties, ctx);
              }
                    },
          1200000,
          properties.Url);
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService SetSiteSecurity completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Sets the site property bag.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="template">The template.</param>
        protected void SetSitePropertyBag(SiteRequestInformation info, Template template)
        {
            LogHelper.LogInformation("Property bag settings configuration Xml processing", LogEventID.InformationWrite);
            if (template != null && template.ConfigurationContainer != null && template.ConfigurationContainer.SiteProperties != null && template.ConfigurationContainer.SiteProperties.Any())
            {
                var siteProperties = template.ConfigurationContainer.SiteProperties;

                SiteMetadata metaData = new SiteMetadata { MetaData = new Dictionary<string, string>() };

                foreach (SiteProperty siteProperty in siteProperties)
                {
                    // this.columns.Add(columnNode.InnerText);
                    var property = info.GetType().GetProperty(siteProperty.Name);
                    if (property != null)
                    {
                        var propertyInfo = property.GetValue(info);

                        if (propertyInfo != null)
                        {
                            if (propertyInfo is string || propertyInfo is int || propertyInfo is bool)
                            {
                                string propertyValue = propertyInfo.ToString();

                                if (!string.IsNullOrEmpty(propertyValue))
                                {
                                    metaData.MetaData.Add(siteProperty.Name, propertyValue);
                                }
                            }
                        }
                    }
                }

                if (metaData.MetaData.Keys.Count > 0)
                {
                    this.UsingContext(
                        tenantCtx =>
                    {
                        Tenant tenant = new Tenant(tenantCtx);
                        var site = tenant.GetSiteByUrl(info.Url);

                        using (var ctx = site.Context.Clone(info.Url))
                        {
                            var web = ctx.Web;
                            foreach (var key in metaData.MetaData.Keys)
                            {
                                LogHelper.LogInformation(string.Format("Adding property {0} and its value {1} to site property bag", key, metaData.MetaData[key]), LogEventID.InformationWrite);
                                this.SetSitePropertyBag(web, key, metaData.MetaData[key]);
                            }
                        }
                        },
                    1200000,
                    info.Url);
                }

                LogHelper.LogInformation("Property bag updated successfully", LogEventID.InformationWrite);
            }
        }
        #endregion

        #region Users
        /// <summary>
        /// Adds the users to group.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="userGroup">The user group.</param>
        private static void AddUsersToGroup(ClientContext ctx, Microsoft.SharePoint.Client.User owner, Group userGroup)
        {
            userGroup.Users.AddUser(owner);
            userGroup.Update();
            ctx.ExecuteQuery();
        }

        /// <summary>
        /// Adds the community list item.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="communityList">The community list.</param>
        /// <param name="communityUser">The community user.</param>
        /// <param name="communityItem">The community item.</param>
        /// <returns>Community list item</returns>
        private static ListItem AddCommunityListItem(ClientContext clientContext, List communityList, Microsoft.SharePoint.Client.User communityUser, ListItem communityItem)
        {
            try
            {
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                communityItem = communityList.AddItem(itemCreateInfo);
                communityItem["Title"] = communityUser.Title;
                communityItem[Constants.MemberFieldName] = communityUser;
                communityItem[Constants.StatusIntFieldName] = Constants.MemberStatusIntActive;
                communityItem.Update();
                clientContext.ExecuteQuery();
            }
            catch (ServerException ex)
            {
                communityItem = null;
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "Exception while adding members to community list.");
            }

            return communityItem;
        }

        /// <summary>
        /// Gets the business admin group.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <returns>String Business admin lookup value</returns>
        private string GetBusinessAdminGroup(SiteRequestInformation properties, AppSettings settings, string templateName)
        {
            string adminLookUpValue = string.Empty;
            Dictionary<string, string> fieldsValue;

            AppOnlyAuthenticationSite siteAuthentication = new AppOnlyAuthenticationSite();
            using (ClientContext ctx = siteAuthentication.GetAuthenticatedContext(settings.SPHostUrl))
            {
                var configWeb = ctx.Web;
                fieldsValue = ConfigListHelper.GetBusinessAdminGroup(ctx, configWeb, properties.Region, properties.BusinessUnit, templateName);
            }

            if (fieldsValue != null)
            {
                fieldsValue.TryGetValue(ConfigListHelper.BusinessUnitAdminFieldValue, out adminLookUpValue);
            }

            return adminLookUpValue;
        }

        /// <summary>
        /// Sets the business unit admins.
        /// </summary>
        /// <param name="properties">The properties.</param>
        private void SetBusinessUnitAdmins(SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService SetBusinessUnitAdmins", LogEventID.InformationWrite);

            if (properties.BusinessUnitAdmin != null && !string.IsNullOrEmpty(properties.BusinessUnitAdmin.LoginName))
            {
                this.UsingContext(
                tenantCtx =>
                {
                    Tenant tenant = new Tenant(tenantCtx);
                    var site = tenant.GetSiteByUrl(properties.Url);

                    using (var ctx = site.Context.Clone(properties.Url))
                    {
                        var web = ctx.Web;
                        Microsoft.SharePoint.Client.User user = web.EnsureUser(properties.BusinessUnitAdmin.LoginName);
                        ctx.Load(user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                        ctx.ExecuteQuery();
                        user.IsSiteAdmin = true;
                        user.Update();
                        ctx.ExecuteQuery();
                        Microsoft.SharePoint.Client.User requestor = web.EnsureUser(properties.RequestorName);
                        ctx.Load(requestor, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                        ctx.ExecuteQuery();
                        requestor.IsSiteAdmin = false;
                        requestor.Update();
                        ctx.ExecuteQuery();
                    }
                },
             1200000,
             properties.Url);
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService SetBusinessUnitAdmins Completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Adds the initial community members.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="ctx">Client Context</param>
        private void AddInitialCommunityMembers(SiteRequestInformation properties, ClientContext ctx)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService AddInitialCommunityMembers", LogEventID.InformationWrite);
            
            var web = ctx.Web;
            ctx.Load(web);
            ctx.ExecuteQuery();
            var communityList = web.Lists.GetByTitle(CommonConstants.Constants.MemberListName);
            ctx.ExecuteQuery();

            if (communityList != null)
            {
                foreach (var initialMember in properties.InitialSiteUsers)
                {
                    web.RefreshLoad();
                    ctx.ExecuteQuery();

                    // web = ctx.Site.RootWeb;
                    if (initialMember != null && !string.IsNullOrEmpty(initialMember.LoginName))
                    {
                        Microsoft.SharePoint.Client.User itemUser = web.EnsureUser(initialMember.LoginName);
                        ctx.Load(itemUser);
                        ctx.ExecuteQuery();

                        if (itemUser != null)
                        {
                            var query = new CamlQuery
                            {
                                ViewXml = string.Format(CamlQueryHelper.CommunityMemberUserQuery, Constants.MemberFieldName, itemUser.Id)
                            };
                            var items = communityList.GetItems(query);
                            ctx.Load(items, eachItem => eachItem.Include(item => item, item => item[Constants.MemberFieldName], item => item[Constants.StatusIntFieldName]));
                            ctx.ExecuteQuery();

                            if (items.Count > 0)
                            {
                                continue;
                            }

                            AddCommunityListItem(ctx, communityList, itemUser, null);
                        }
                    }
                }
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService AddInitialCommunityMembers Completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Joins the community.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="ctx">Client Context</param>
        private void JoinCommunity(SiteRequestInformation properties, ClientContext ctx)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService JoinCommunity", LogEventID.InformationWrite);

            var web = ctx.Web;
            ctx.Load(web);
            ctx.ExecuteQuery();
            this.EnsureUserInCommunityMembersList(ctx, web, properties);

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService JoinCommunity Completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Ensures the user in community members list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="web">The web.</param>
        /// <param name="properties">The properties.</param>
        private void EnsureUserInCommunityMembersList(ClientContext clientContext, Web web, SiteRequestInformation properties)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService EnsureUserInCommunityMembersList", LogEventID.InformationWrite);

            try
            {
                var communityList = web.Lists.GetByTitle(CommonConstants.Constants.MemberListName);
                var primaryAdmin = web.EnsureUser(properties.SiteOwner.LoginName);
                Microsoft.SharePoint.Client.User secondaryAdmin = null;
                Microsoft.SharePoint.Client.User tertaryAdmin = null;
                Microsoft.SharePoint.Client.User moderator = null;

                if (properties.SecondaryAdminUser != null && !string.IsNullOrEmpty(properties.SecondaryAdminUser.LoginName))
                {
                    secondaryAdmin = web.EnsureUser(properties.SecondaryAdminUser.LoginName);
                    clientContext.Load(secondaryAdmin);
                }

                if (properties.TertiaryAdminUser != null && !string.IsNullOrEmpty(properties.TertiaryAdminUser.LoginName))
                {
                    tertaryAdmin = web.EnsureUser(properties.TertiaryAdminUser.LoginName);
                    clientContext.Load(tertaryAdmin);
                }

                if (properties.Moderator != null && !string.IsNullOrEmpty(properties.Moderator.LoginName))
                {
                    moderator = web.EnsureUser(properties.Moderator.LoginName);
                    clientContext.Load(moderator);
                }

                clientContext.Load(communityList);
                clientContext.Load(primaryAdmin);
                clientContext.ExecuteQuery();
                this.AddMemberToCommunityList(clientContext, web, communityList, primaryAdmin);

                if (secondaryAdmin != null)
                {
                    this.AddMemberToCommunityList(clientContext, web, communityList, secondaryAdmin);
                }

                if (tertaryAdmin != null)
                {
                    this.AddMemberToCommunityList(clientContext, web, communityList, tertaryAdmin);
                }

                if (moderator != null)
                {
                    this.AddMemberToCommunityList(clientContext, web, communityList, moderator);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "Exception Joining users to Community.");
            }
            finally
            {
                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService EnsureUserInCommunityMembersList - completed", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Adds the member to community list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="web">The web.</param>
        /// <param name="communityList">The community list.</param>
        /// <param name="communityUser">The community user.</param>
        private void AddMemberToCommunityList(ClientContext clientContext, Web web, List communityList, Microsoft.SharePoint.Client.User communityUser)
        {
            var query = new CamlQuery
            {
                ViewXml =
                    string.Format(CamlQueryHelper.CommunityMemberUserQuery, Constants.MemberFieldName, communityUser.Id)
            };
            var items = communityList.GetItems(query);
            clientContext.Load(items, eachItem => eachItem.Include(item => item, item => item[Constants.MemberFieldName], item => item[Constants.StatusIntFieldName]));
            clientContext.ExecuteQuery();

            ListItem communityItem = null;

            if (items.Count > 0)
            {
                communityItem = items[0];
            }

            if (communityItem != null)
            {
                if (communityItem[Constants.StatusIntFieldName] != null)
                {
                    if (communityItem[Constants.StatusIntFieldName].ToString() != Constants.MemberStatusIntActive.ToString())
                    {
                        communityItem[Constants.StatusIntFieldName] = Constants.MemberStatusIntActive;
                        communityItem.Update();
                        clientContext.ExecuteQuery();
                    }
                }
            }
            else
            {
                communityItem = AddCommunityListItem(clientContext, communityList, communityUser, null);
                this.SetSitePropertyBag(web, Constants.CommunityMembersCount, communityList.ItemCount + 1);
            }
        }

        /// <summary>
        /// Configures the community moderators.
        /// </summary>
        /// <param name="moderatorsGroupName">Name of the moderators group.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="ctx">Client Context</param>
        private void ConfigureCommunityModerators(string moderatorsGroupName, SiteRequestInformation properties, ClientContext ctx)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService ConfigureCommunityModerators", LogEventID.InformationWrite);

            var web = ctx.Web;
            var moderator = web.EnsureUser(properties.Moderator.LoginName);
            GroupCollection groups = web.SiteGroups;
            ctx.Load(web);
            ctx.Load(groups);
            ctx.Load(moderator);
            ctx.ExecuteQuery();

            // Add users to group
            Group moderatorGroup = web.SiteGroups.GetByName(moderatorsGroupName);
            web.AssociatedOwnerGroup.Users.AddUser(moderator);
            web.AssociatedOwnerGroup.Update();

            if (moderatorGroup != null)
            {
                moderatorGroup.Users.AddUser(moderator);
                moderatorGroup.Update();
                LogHelper.LogInformation("Added moderator to Admin group", LogEventID.InformationWrite);
            }

            ctx.ExecuteQuery();

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService ConfigureCommunityModerators completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Configures the community site type configuration.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="ctx">Client Context</param>
        private void ConfigureCommunitySiteTypeConfiguration(SiteRequestInformation properties, ClientContext ctx)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService ConfigureCommunitySiteTypeConfiguration", LogEventID.InformationWrite);

            switch (properties.CommunitySiteType)
            {
                case CommonConstants.Constants.CommunitySiteTypePublic:

                    var web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQuery();
                    web.SetPropertyBagValue(CommonConstants.Constants.CommunitySiteEnableAutoApproval, "true");

                    this.AddEveryoneToVisitors(ctx, web, properties);
                    break;

                case CommonConstants.Constants.CommunitySiteTypePrivate:
                    // RequestAccessEmail code cannot be applied                   
                    this.AddEveryoneToVisitors(properties, ctx);
                    break;
            }

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.OnlineProvisioningService.AbstractProvisioningService ConfigureCommunitySiteTypeConfiguration completed", LogEventID.InformationWrite);
        }

         #endregion

        #region PropertyBag
        /// <summary>
        /// Sets the site property bag.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        private void SetSitePropertyBag(Web web, string propertyName, string propertyValue)
        {
            web.SetPropertyBagValue(propertyName, propertyValue);
            web.AddIndexedPropertyBagKey(propertyName);
        }

        /// <summary>
        /// Sets the site property bag.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        private void SetSitePropertyBag(Web web, string propertyName, int propertyValue)
        {
            web.SetPropertyBagValue(propertyName, propertyValue);
            web.AddIndexedPropertyBagKey(propertyName);
        }

        /// <summary>
        /// Sets the site policy.
        /// </summary>
        /// <param name="siteUrl">The site URL.</param>
        /// <param name="policyName">Name of the policy.</param>
        private void SetSitePolicy(string siteUrl, string policyName)
        {
            LogHelper.LogInformation(string.Format("Applying Site Policy {0}", policyName), LogEventID.InformationWrite);

            this.UsingContext(
                tenantCtx =>
          {
              Tenant tenant = new Tenant(tenantCtx);
              var site = tenant.GetSiteByUrl(siteUrl);

              using (var context = site.Context.Clone(siteUrl))
              {
                  var web = context.Web;
                  context.Load(web);
                  context.ExecuteQuery();
                  web.ApplySitePolicy(policyName);
              }
                },
          1200000,
          siteUrl);
        }
        #endregion

        #region list
        /// <summary>
        /// Provisions the list instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="instance">The instance.</param>
        private void ProvisionListInstance(ClientContext context, Web web, ListDefinition definition, ListInstance instance)
        {
            int listTemplateType = -1;

            if (definition != null)
            {
                listTemplateType = this.GetListTemplateTypeByBaseType(definition.BaseType);
            }

            List newList = null;

            if (!web.ListExists(instance.Title))
            {
                if (instance.IsCustom && listTemplateType != -1 && definition != null)
                {
                    newList = web.CreateList(listTemplateType, instance.Title, instance.Description, definition.VersioningEnabled.ToBoolean(), definition.EnableMinorVersions.ToBoolean(), definition.EnableContentTypes.ToBoolean(), true, instance.OnQuickLaunch);
                }
                else
                {
                    newList = web.CreateList(instance.ListTemplateId, instance.Title, instance.EnableVersioning, false, true, true);
                }
            }
            else
            {
                newList = web.Lists.GetByTitle(instance.Title);
                context.Load(newList);
                context.ExecuteQuery();
            }

            if (newList != null && newList.ServerObjectIsNull != null && newList.ServerObjectIsNull != true)
            {
                if (definition != null)
                {
                    this.AddFieldsToList(definition.Fields, newList);
                    this.AddViewsToList(definition.Views, newList);

                    if (definition.ContentTypes != null)
                    {
                        web.RemoveAllContentTypesFromList(newList);
                        this.AddContentTypesToList(web, newList, definition.ContentTypes);
                    }

                    if (definition.Receivers != null && definition.Receivers.Count > 0)
                    {
                        this.AddListEventReceiver(newList, definition.Receivers);
                    }
                }
                else if (!instance.IsCustom && !string.IsNullOrEmpty(instance.ListTemplateId))
                {
                    if (instance.GetContentTypeBindings != null && instance.GetContentTypeBindings.Count > 0)
                    {
                        if (instance.RemoveDefaultContentTypes)
                        {
                            web.RemoveAllContentTypesFromList(newList);
                        }

                        foreach (var contentTypeBinding in instance.GetContentTypeBindings)
                        {
                            newList.AddContentTypeToListById(contentTypeBinding.ContentTypeID, contentTypeBinding.Default);
                        }
                    }
                }

                context.Load(newList, l => l.DefaultViewUrl);
                context.ExecuteQuery();
                LogHelper.LogInformation(string.Format("List Instance created {0}", instance.Title), LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format("Falied to create list {0}", instance.Title), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Gets the type of the list template type by base.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <returns>Return list template type</returns>
        private int GetListTemplateTypeByBaseType(int baseType)
        {
            int listTemplateType;

            switch (baseType)
            {
                case 0:
                    listTemplateType = (int)ListTemplateType.GenericList;
                    break;
                case 1:
                    listTemplateType = (int)ListTemplateType.DocumentLibrary;
                    break;
                case 4:
                    listTemplateType = (int)ListTemplateType.Survey;
                    break;
                case 5:
                    listTemplateType = (int)ListTemplateType.IssueTracking;
                    break;
                default:
                    listTemplateType = (int)ListTemplateType.GenericList;
                    break;
            }

            return listTemplateType;
        }

        /// <summary>
        /// Adds the fields to list.
        /// </summary>
        /// <param name="listfields">The list fields.</param>
        /// <param name="newList">The new list.</param>
        private void AddFieldsToList(ListFields listfields, List newList)
        {
            if (listfields != null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(this.CreateXML(listfields));
                XmlNodeList fields = xmlDoc.SelectNodes("//Field");

                if (fields != null)
                {
                    foreach (XmlNode field in fields)
                    {
                        newList.CreateField(field.OuterXml);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the XML.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// Return xml string
        /// </returns>
        private string CreateXML(ListFields entity)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(entity.GetType());

            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, entity);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return WebUtility.HtmlDecode(xmlDoc.InnerXml.Replace("<FieldBody>", string.Empty).Replace("</FieldBody>", string.Empty));
            }
        }

        /// <summary>
        /// Adds the views to list.
        /// </summary>
        /// <param name="views">The views.</param>
        /// <param name="newList">The new list.</param>
        private void AddViewsToList(List<ListView> views, List newList)
        {
            if (views != null && views.Count > 0)
            {
                //// Create View to List
                foreach (var view in views)
                {
                    if (!view.Hidden.ToBoolean() || !view.ReadOnly.ToBoolean() || !string.IsNullOrEmpty(view.DisplayName))
                    {
                        newList.DeleteViewByName(view.DisplayName);

                        List<string> fieldArry = new List<string>();

                        foreach (var field in view.ViewFields)
                        {
                            fieldArry.Add(field.Name);
                        }

                        Microsoft.SharePoint.Client.ViewType type = (Microsoft.SharePoint.Client.ViewType)Enum.Parse(typeof(Microsoft.SharePoint.Client.ViewType), view.Type, true);
                        newList.CreateView(view.DisplayName, type, fieldArry.ToArray(), view.RowLimit, view.DefaultView.ToBoolean(), view.Query);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the content types to list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="contentTypes">The content types.</param>
        private void AddContentTypesToList(Web web, List newList, ContentTypeDefinitions contentTypes)
        {
            if (contentTypes == null)
            {
                return;
            }

            //// Create Contenttype
            if (contentTypes.ContentTypeRef != null)
            {
                foreach (var contentTypeRef in contentTypes.ContentTypeRef)
                {
                    newList.AddContentTypeToListById(contentTypeRef.ID);
                }
            }

            if (contentTypes.ContentType != null)
            {
                ContentTypeDefinition contentType = contentTypes.ContentType;

                Microsoft.SharePoint.Client.ContentType siteContentType = web.CreateContentType(contentType.Name, string.Empty, contentType.ID, contentType.Group);

                //// Add contenttype to List
                Microsoft.SharePoint.Client.ContentType listContentType = newList.AddContentTypeToList(siteContentType);

                //// Add fieldRefs to ContentType
                List<ContentTypeFieldRef> cntFields = contentType.FieldRefs;
                foreach (var contentTypeFieldRef in cntFields)
                {
                    newList.AddListFieldToContentTypeById(listContentType.Id.StringValue, contentTypeFieldRef.ID, false, false);
                }
            }
        }

        /// <summary>
        /// Adds the list event receiver.
        /// </summary>
        /// <param name="newList">The new list.</param>
        /// <param name="listReceivers">The list receivers.</param>
        private void AddListEventReceiver(List newList, List<ListReceiver> listReceivers)
        {
            if (listReceivers == null || listReceivers.Count == 0)
            {
                return;
            }

            foreach (ListReceiver listReceiver in listReceivers)
            {
                newList.AddRemoteEventReceiver(listReceiver.Name, listReceiver.Url, (EventReceiverType)Enum.Parse(typeof(EventReceiverType), listReceiver.Type, true), (EventReceiverSynchronization)Enum.Parse(typeof(EventReceiverSynchronization), listReceiver.Synchronization, true), listReceiver.SequenceNumber, true);
            }
        }
        #endregion
    }
}