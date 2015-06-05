//-----------------------------------------------------------------------
// <copyright file= "HomeController.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.SubSiteProvisioningAppWeb.Controllers
{   
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Web.Mvc;
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.SubSiteProvisioningAppWeb.Models;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Controller actions
    /// </summary>
    [SharePointContextFilter]
    public class HomeController : Controller
    {
        /// <summary>
        /// Index action instance.
        /// </summary>
        /// <returns>return view</returns>
        public ActionResult Index()
        {
            try
            {
                LogHelper.LogInformation("Subsite provisioning app loading.", LogEventID.InformationWrite);
                SiteViewModel siteViewModel = this.GetSiteViewModel();
                LogHelper.LogInformation("Subsite provisioning app loaded.", LogEventID.InformationWrite);
                return this.View(siteViewModel);
            }
            catch (Exception ex)
            {
                SiteViewModel siteViewModel = this.GetSiteViewModel();
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                return this.View("Index", siteViewModel);
            }
        }       

        /// <summary>
        /// Creates the specified site view model.
        /// </summary>
        /// <param name="siteViewModel">The site view model.</param>
        /// <returns>Return view</returns>
        [HttpPost]
        public ActionResult Create(SiteViewModel siteViewModel)
        {
            string message = string.Empty;
            try
            {
                LogHelper.LogInformation("Create sub site.", LogEventID.InformationWrite);

                if (!ModelState.IsValid)
                {
                    return this.LoadIndex(siteViewModel, "Site view model is invalid.");
                }

                return this.SubSiteProvision(siteViewModel);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                return this.LoadIndex(siteViewModel, "Exception occured in sub site creation.");
            }
        }

        /// <summary>
        /// Subs the site provision.
        /// </summary>
        /// <param name="siteViewModel">The site view model.</param>
        /// <returns>Return result view</returns>
        private ActionResult SubSiteProvision(SiteViewModel siteViewModel)
        {
            var spcontext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var context = spcontext.CreateUserClientContextForSPHost())
            {
                context.Load(context.Web);
                context.ExecuteQuery();

                Web parentWeb = context.Web;

                if (!parentWeb.CheckCurrentUserAuthorization(PermissionKind.ManageSubwebs))
                {
                    return this.LoadIndex(siteViewModel, "You do not have enough rights to create site.");
                }

                if (parentWeb.WebExists(siteViewModel.URLName))
                {
                    return this.LoadIndex(siteViewModel, "This sub site is already existed. Please try with different name.");
                }
                else
                {
                    Web newWeb = this.SubSiteProvision(siteViewModel, context, parentWeb);

                    // All done, let's return the newly created site
                    // Redirect to just created site
                    return this.Redirect(newWeb.Url);
                }
            }
        }

        /// <summary>
        /// Loads the index.
        /// </summary>
        /// <param name="siteViewModel">The site view model.</param>
        /// <param name="message">The message.</param>
        /// <returns>Returns view index</returns>
        private ActionResult LoadIndex(SiteViewModel siteViewModel, string message = "")
        {
            if (!string.IsNullOrEmpty(message))
            {
                ModelState.AddModelError("error", message);
            }

            siteViewModel.SiteTemplates = this.GetSiteTemplates();
            siteViewModel.Languages = this.GetWebSupportedUiLanguages();
            return this.View("Index", siteViewModel);
        }

        /// <summary>
        /// Subs the site provision.
        /// </summary>
        /// <param name="siteViewModel">The site view model.</param>
        /// <param name="context">The context.</param>
        /// <param name="parentWeb">The parent web.</param>
        /// <returns>Return web</returns>
        private Web SubSiteProvision(SiteViewModel siteViewModel, ClientContext context, Web parentWeb)
        {
            Web newWeb = parentWeb.CreateWeb(siteViewModel.Title, siteViewModel.URLName, siteViewModel.Description, siteViewModel.SiteTemplate, Convert.ToInt32(siteViewModel.LCID), siteViewModel.IsParentSitePermission);

            TemplateConfiguration config = this.GetTemplateConfiguration();

            this.ActivateFeatures(newWeb, config);

            context.Load(context.Site.RootWeb);
            context.ExecuteQuery();

            Web rootWeb = context.Site.RootWeb;

            this.SetTheme(rootWeb, newWeb, config);

            this.ApplySitePolicy(newWeb, config);

            this.AddCustomActions(newWeb, config);

            context.Load(newWeb);
            context.ExecuteQuery();
            LogHelper.LogInformation("Subsite created.", LogEventID.InformationWrite);
            return newWeb;
        }

        /// <summary>
        /// Adds the custom actions.
        /// </summary>
        /// <param name="newWeb">The new web.</param>
        /// <param name="config">The configuration.</param>
        private void AddCustomActions(Web newWeb, TemplateConfiguration config)
        {
            List<CustomActionEntity> customActions = config.CustomActions;
            foreach (var customAction in customActions)
            {
                if (!string.IsNullOrEmpty(customAction.Rights))
                {
                    BasePermissions rightsPermissions = new BasePermissions();
                    rightsPermissions.Set((PermissionKind)Enum.ToObject(typeof(PermissionKind), Convert.ToInt32(customAction.Rights)));
                    customAction.RightsPermissions = rightsPermissions;
                }

                // Add user custom action to the just created site
                newWeb.AddCustomAction(customAction);
            }
        }

        /// <summary>
        /// Applies the site policy.
        /// </summary>
        /// <param name="newWeb">The new web.</param>
        /// <param name="config">The configuration.</param>
        private void ApplySitePolicy(Web newWeb, TemplateConfiguration config)
        {
            List<PolicyEntity> policies = config.Policies;
            foreach (var policy in policies)
            {
                // apply policy to the just created site
                newWeb.ApplySitePolicy(policy.Name);
            }
        }

        /// <summary>
        /// Sets the theme.
        /// </summary>
        /// <param name="parentWeb">The parent web.</param>
        /// <param name="newWeb">The new web.</param>
        /// <param name="config">The configuration.</param>
        private void SetTheme(Web parentWeb, Web newWeb, TemplateConfiguration config)
        {
            // Set oob theme to the just created site
            ThemeEntity themeEntity = config.ThemePackage;
            newWeb.SetThemeBasedOnName(parentWeb, themeEntity.Name);
        }

        /// <summary>
        /// Activates the features.
        /// </summary>
        /// <param name="newWeb">The new web.</param>
        /// <param name="config">The configuration.</param>
        private void ActivateFeatures(Web newWeb, TemplateConfiguration config)
        {
            List<FeatureEntity> features = config.Features;
            foreach (var feature in features)
            {
                // Activate site feature to the just created site
                FeatureDefinitionScope scope = (FeatureDefinitionScope)Enum.Parse(typeof(FeatureDefinitionScope), feature.Scope, true);
                newWeb.ActivateFeature(new Guid(feature.ID), feature.Activate, scope);
            }
        }

        /// <summary>
        /// Gets the languages.
        /// </summary>
        /// <returns>Return languages</returns>
        private List<SelectListItem> GetWebSupportedUiLanguages()
        {
            List<SelectListItem> languages = new List<SelectListItem>();

            var spcontext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var context = spcontext.CreateUserClientContextForSPHost())
            {
                context.Load(context.Web, w => w.SupportedUILanguageIds);
                context.ExecuteQuery();

                foreach (var lcid in context.Web.SupportedUILanguageIds)
                {
                    string language = CultureInfo.GetCultureInfo(lcid).DisplayName;
                    languages.Add(new SelectListItem { Text = language, Value = lcid.ToString(), Selected = lcid == 1033 ? true : false });
                }
            }

            return languages;
        }

        /// <summary>
        /// Gets the site templates.
        /// </summary>
        /// <returns>Return templates</returns>
        private List<SelectListItem> GetSiteTemplates()
        {            
            List<SelectListItem> templates = new List<SelectListItem>();
            TemplateConfiguration config = this.GetTemplateConfiguration();
            if (config == null || config.SiteTemplates == null)
            {
                return templates;
            }
            
            List<SiteTemplateEntity> siteTemplates = config.SiteTemplates;

            foreach (var webTemplate in siteTemplates)
            {
                templates.Add(new SelectListItem { Text = webTemplate.Title, Value = webTemplate.Name, Selected = webTemplate.Name.Equals("STS#0", StringComparison.OrdinalIgnoreCase) });
            }

            return templates;
        }

        /// <summary>
        /// Gets the template configuration.
        /// </summary>
        /// <returns>Return template</returns>
        private TemplateConfiguration GetTemplateConfiguration()
        {
            LogHelper.LogInformation("Serialize template configuration xml to oject.", LogEventID.InformationWrite);

            TemplateConfiguration templateConfiguration = new TemplateConfiguration();

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TemplateConfigurationXmlLocation"]))
            {
                return templateConfiguration;
            }

            string xmlLocation = ConfigurationManager.AppSettings["TemplateConfigurationXmlLocation"];
            string path = HttpContext.Server.MapPath(xmlLocation);
            XmlSerializer deserializer = new XmlSerializer(typeof(TemplateConfiguration));
            TextReader textReader = new System.IO.StreamReader(path);
            templateConfiguration = (TemplateConfiguration)deserializer.Deserialize(textReader);
            textReader.Close();
            LogHelper.LogInformation("Serialized template configuration xml to object.", LogEventID.InformationWrite);
            return templateConfiguration;
        }

        /// <summary>
        /// Gets the site view model.
        /// </summary>
        /// <returns>Return site view model</returns>
        private SiteViewModel GetSiteViewModel()
        {
            SiteViewModel siteViewModel = new SiteViewModel
            {
                IsParentSitePermission = true,
                Languages = this.GetWebSupportedUiLanguages(),
                SiteTemplates = this.GetSiteTemplates()
            };
            return siteViewModel;
        }
    }
}