//-----------------------------------------------------------------------
// <copyright file= "MySiteHostBrandingController.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.MySiteCustomizerAppWeb.Controllers
{    
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web.Mvc;
    using System.Xml.Serialization;
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.UserProfiles;

    /// <summary>
    /// My site host branding controller
    /// </summary> 
    [SharePointContextFilter]
    public class MySiteHostBrandingController : Controller
    {
        /// <summary>
        /// Index action this instance.
        /// </summary>
        /// <returns>Return view</returns>
        public ActionResult Index()
        {
            try
            {
                var spcontext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                using (ClientContext clientContext = spcontext.CreateUserClientContextForSPHost())
                {
                    LogHelper.LogInformation("Loading personal site", LogEventID.InformationWrite);

                    // Get user profile
                    ProfileLoader loader = Microsoft.SharePoint.Client.UserProfiles.ProfileLoader.GetProfileLoader(clientContext);
                    UserProfile profile = loader.GetUserProfile();
                    Microsoft.SharePoint.Client.Site personalSite = profile.PersonalSite;

                    clientContext.Load(personalSite);
                    clientContext.ExecuteQuery();

                    if (personalSite != null)
                    {
                        LogHelper.LogInformation("Loaded personal site", LogEventID.InformationWrite);
                    }
                    else
                    {
                        LogHelper.LogInformation("Personal site does not exist", LogEventID.InformationWrite);
                    }

                    BrandingEntity brandingEntity = this.GetBrandingEntity();

                    ThemeEntity themeEntity = brandingEntity.ThemePackage;
                    CustomActionEntity customActionEntity = brandingEntity.CustomAction;

                    // Let's check if the site already exists
                    if (personalSite.ServerObjectIsNull.Value == false)
                    {
                        LogHelper.LogInformation("Accessing OneDrive site", LogEventID.InformationWrite);

                        using (var context = personalSite.Context.Clone(personalSite.Url))
                        {
                            // OneDrive for Business site custom branding.
                            Web personalRootWeb = context.Web;
                            context.Load(personalRootWeb);
                            context.ExecuteQuery();

                            if (personalRootWeb.GetPropertyBagValueInt(Constants.JCICAMBrandingPropertyBagKey, 0) != themeEntity.Version)
                            {
                                // Let's first upload the contoso theme to host web, if it does not exist there
                                LogHelper.LogInformation(string.Format("OneDrive for Business site existed at {0}. Custom branding applying.", personalRootWeb.Url), LogEventID.InformationWrite);

                                this.DeployThemeToWeb(personalRootWeb, themeEntity);
                                LogHelper.LogInformation(string.Format("OneDrive for Business site existed at {0}. Custom branding applied.", personalRootWeb.Url), LogEventID.InformationWrite);
                                try
                                {
                                    this.ApplyCss(personalRootWeb, themeEntity, "Style Library");
                                    LogHelper.LogInformation(string.Format("Applied css for site {0}", personalRootWeb.Url), LogEventID.InformationWrite);
                                    this.ApplySiteLogo(personalRootWeb, themeEntity, "Style Library");
                                    LogHelper.LogInformation(string.Format("Applied site logo for site {0}", personalRootWeb.Url), LogEventID.InformationWrite);
                                }
                                catch (Exception ex)
                                {
                                    // Ignoring exception
                                    LogHelper.LogInformation("Due to API limitations, AlternateCSS and SiteLogo options may not work in dedicated environment. This options can be used in vNext.", LogEventID.InformationWrite);
                                    LogHelper.LogError(ex);
                                }

                                // apply header and footer css.
                                personalRootWeb.AddCustomAction(customActionEntity);
                                LogHelper.LogInformation(string.Format("Applied header and footer css for site {0}", personalRootWeb.Url), LogEventID.InformationWrite);

                                // Let's set the site processed, so that we don't update that all the time. Currently set as theme version.
                                personalRootWeb.SetPropertyBagValue(Constants.JCICAMBrandingPropertyBagKey, themeEntity.Version);
                            }

                            LogHelper.LogInformation("Accessing blog or sub sites", LogEventID.InformationWrite);
                            WebCollection webs = personalRootWeb.Webs;
                            context.Load(webs);
                            context.ExecuteQuery();

                            foreach (var subWeb in webs)
                            {
                                if (subWeb.GetPropertyBagValueInt(Constants.JCICAMBrandingPropertyBagKey, 0) != themeEntity.Version)
                                {
                                    LogHelper.LogInformation(string.Format("Sub site existed at {0}. Custom branding applying.", subWeb.Url), LogEventID.InformationWrite);

                                    subWeb.SetThemeBasedOnName(personalRootWeb, themeEntity.Name);

                                    LogHelper.LogInformation(string.Format("Sub site existed at {0}. Custom branding applied.", subWeb.Url), LogEventID.InformationWrite);
                                    try
                                    {
                                        this.ApplyCss(subWeb, themeEntity, "SiteAssets");
                                        LogHelper.LogInformation(string.Format("Applied css for site {0}", subWeb.Url), LogEventID.InformationWrite);
                                        this.ApplySiteLogo(subWeb, themeEntity, "SiteAssets");
                                        LogHelper.LogInformation(string.Format("Applied site logo for site {0}", subWeb.Url), LogEventID.InformationWrite);
                                    }
                                    catch (Exception ex)
                                    {
                                        // Ignoring exception
                                        LogHelper.LogInformation("Due to API limitations, AlternateCSS and SiteLogo options may not work in dedicated environment. This options can be used in vNext.", LogEventID.InformationWrite);
                                        LogHelper.LogError(ex);
                                    }

                                    // apply header and footer css.
                                    subWeb.AddCustomAction(customActionEntity);
                                    LogHelper.LogInformation(string.Format("Applied header and footer css for site {0}", subWeb.Url), LogEventID.InformationWrite);

                                    // Let's set the site processed, so that we don't update that all the time. Currently set as theme version.
                                    subWeb.SetPropertyBagValue(Constants.JCICAMBrandingPropertyBagKey, themeEntity.Version);
                                }
                            }
                        }
                    }

                    LogHelper.LogInformation("Accessing MySitesHost site", LogEventID.InformationWrite);

                    Site site = clientContext.Site;
                    Web mySiteRootWeb = site.RootWeb;
                    clientContext.Load(mySiteRootWeb);
                    clientContext.ExecuteQuery();

                    if (mySiteRootWeb.GetPropertyBagValueInt(Constants.JCICAMBrandingPropertyBagKey, 0) != themeEntity.Version)
                    {
                        // Let's first upload the contoso theme to host web, if it does not exist there
                        LogHelper.LogInformation(string.Format("Mysite host site existed at {0}. Custom branding applying.", mySiteRootWeb.Url), LogEventID.InformationWrite);

                        this.DeployThemeToWeb(mySiteRootWeb, themeEntity);

                        LogHelper.LogInformation(string.Format("Mysite host site existed at {0}. Custom branding applied.", mySiteRootWeb.Url), LogEventID.InformationWrite);
                        try
                        {
                            this.ApplyCss(mySiteRootWeb, themeEntity, "Site Assets");
                            LogHelper.LogInformation(string.Format("Applied css for site {0}", mySiteRootWeb.Url), LogEventID.InformationWrite);
                            this.ApplySiteLogo(mySiteRootWeb, themeEntity, "Site Assets");
                            LogHelper.LogInformation(string.Format("Applied site logo for site {0}", mySiteRootWeb.Url), LogEventID.InformationWrite);
                        }
                        catch (Exception ex)
                        {
                            // Ignoring exception
                            LogHelper.LogInformation("Due to API limitations, AlternateCSS and SiteLogo options may not work in dedicated environment. This options can be used in vNext.", LogEventID.InformationWrite);
                            LogHelper.LogError(ex);
                        }

                        // apply header and footer css.
                        mySiteRootWeb.AddCustomAction(customActionEntity);
                        LogHelper.LogInformation(string.Format("Applied header and footer css for site {0}", mySiteRootWeb.Url), LogEventID.InformationWrite);

                        // Let's set the site processed, so that we don't update that all the time. Currently set as "version" 1 of branding
                        mySiteRootWeb.SetPropertyBagValue(Constants.JCICAMBrandingPropertyBagKey, themeEntity.Version);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "Error in applying Custom branding for My site host.");
            }

            return this.View();
        }

        #region Branding

        /// <summary>
        /// Deploys the theme.
        /// </summary>
        /// <param name="web">The root web.</param>
        /// <param name="theme">The theme.</param>
        public void DeployThemeToWeb(Web web, ThemeEntity theme)
        {
            try
            {
                if (!string.IsNullOrEmpty(theme.ColorFile))
                {
                    web.UploadThemeFile(HttpContext.Server.MapPath(string.Format("~/{0}", theme.ColorFile)));
                    LogHelper.LogInformation(string.Format("Completed Uploading Color File {0}: for site {1}", theme.ColorFile, web.Url), LogEventID.InformationWrite);
                }

                if (!string.IsNullOrEmpty(theme.FontFile))
                {
                    web.UploadThemeFile(HttpContext.Server.MapPath(string.Format("~/{0}", theme.FontFile)));
                    LogHelper.LogInformation(string.Format("Completed Uploading Font File {0}: for site {1}", theme.FontFile, web.Url), LogEventID.InformationWrite);
                }

                if (!string.IsNullOrEmpty(theme.BackgroundFile))
                {
                    web.UploadThemeFile(HttpContext.Server.MapPath(string.Format("~/{0}", theme.BackgroundFile)));
                    LogHelper.LogInformation(string.Format("Completed Uploading BackgroundFile {0}: for site {1}", theme.BackgroundFile, web.Url), LogEventID.InformationWrite);
                }

                web.CreateComposedLookForOneDriveByName(theme.Name, theme.ColorFile.Substring(theme.ColorFile.LastIndexOf('/') + 1), theme.FontFile.Substring(theme.FontFile.LastIndexOf('/') + 1), theme.BackgroundFile.Substring(theme.BackgroundFile.LastIndexOf('/') + 1), string.Empty);
                LogHelper.LogInformation(string.Format("Created Composed look {0}: for site {1}", theme.Name, web.Url), LogEventID.InformationWrite);
                web.SetComposedLookByUrl(theme.Name);
                LogHelper.LogInformation(string.Format("Seting theme {0}: for site {1}", theme.Name, web.Url), LogEventID.InformationWrite);
            }
            catch (ServerException svex)
            {
                LogHelper.LogError(svex, LogEventID.ExceptionHandling, "DeployTheme");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "DeployTheme");
            }
        }

        /// <summary>
        /// Applies the CSS.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="theme">The theme.</param>
        /// <param name="listTitle">The list title.</param>
        public void ApplyCss(Web web, ThemeEntity theme, string listTitle)
        {
            if (!string.IsNullOrEmpty(theme.AlternateCSS))
            {
                try
                {
                    string cssFilePath = HttpContext.Server.MapPath(string.Format("~/{0}", theme.AlternateCSS));
                    List assetLibrary = web.GetListByTitle(listTitle);
                    web.Context.Load(assetLibrary, l => l.RootFolder);
                    System.IO.FileInfo cssFile = new System.IO.FileInfo(cssFilePath);
                    FileCreationInformation newFile = new FileCreationInformation
                    {
                        Content = System.IO.File.ReadAllBytes(cssFilePath),
                        Url = cssFile.Name,
                        Overwrite = true
                    };
                    Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
                    web.Context.Load(uploadFile);
                    web.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format("Uploaded Site Logo {0} to list {1} for site {2} ", cssFile.Name, listTitle, web.Url), LogEventID.InformationWrite);

                    string cssUrl = string.Empty;
                    if (web.ServerRelativeUrl.Equals("/"))
                    {
                        cssUrl = string.Format("{0}{1}/{2}", web.ServerRelativeUrl, listTitle, cssFile.Name);
                    }
                    else
                    {
                        cssUrl = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, listTitle, cssFile.Name);
                    }

                    web.AlternateCssUrl = cssUrl;
                    web.Update();
                    web.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format("Setting {0} css : for site {1}", cssFile.Name, web.Url), LogEventID.InformationWrite);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling, "ApplyCSS");
                }
            }
        }

        /// <summary>
        /// Applies the site logo.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="theme">The theme.</param>
        /// <param name="listTitle">The list title.</param>
        public virtual void ApplySiteLogo(Web web, ThemeEntity theme, string listTitle)
        {
            if (!string.IsNullOrEmpty(theme.SiteLogo))
            {
                try
                {
                    string siteLogoFilePath = HttpContext.Server.MapPath(string.Format("~/{0}", theme.SiteLogo));
                    List assetLibrary = web.GetListByTitle(listTitle);
                    web.Context.Load(assetLibrary, l => l.RootFolder);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(siteLogoFilePath);
                    FileCreationInformation newFile = new FileCreationInformation
                    {
                        Content = System.IO.File.ReadAllBytes(siteLogoFilePath),
                        Url = fileInfo.Name,
                        Overwrite = true
                    };
                    Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
                    web.Context.Load(uploadFile);
                    web.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format("Uploaded Site Logo {0} to list {1} for site {2} ", fileInfo.Name, listTitle, web.Url), LogEventID.InformationWrite);

                    string siteLogoUrl = string.Empty;
                    if (web.ServerRelativeUrl.Equals("/"))
                    {
                        siteLogoUrl = string.Format("{0}{1}/{2}", web.ServerRelativeUrl, listTitle, fileInfo.Name);
                    }
                    else
                    {
                        siteLogoUrl = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, listTitle, fileInfo.Name);
                    }

                    web.SiteLogoUrl = siteLogoUrl;
                    web.Update();
                    web.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format("Setting Site Logo {0}: for site {1}", fileInfo.Name, web.Url), LogEventID.InformationWrite);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling, "ApplySiteLogo");
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets the theme entity.
        /// </summary>
        /// <returns>Return theme</returns>
        private BrandingEntity GetBrandingEntity()
        {
            LogHelper.LogInformation("Loading theme package xml file to serialize", LogEventID.InformationWrite);

            BrandingEntity brandingEntity = new BrandingEntity();

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["ThemePackageXmlLocation"]))
            {
                LogHelper.LogInformation("Theme package xml file location not found", LogEventID.InformationWrite);
                return brandingEntity;
            }

            string xmlLocation = ConfigurationManager.AppSettings["ThemePackageXmlLocation"];
            string path = HttpContext.Server.MapPath(xmlLocation);
            XmlSerializer deserializer = new XmlSerializer(typeof(BrandingEntity));
            TextReader textReader = new System.IO.StreamReader(path);
            brandingEntity = (BrandingEntity)deserializer.Deserialize(textReader);
            textReader.Close();
            LogHelper.LogInformation("Theme package xml file is serialized", LogEventID.InformationWrite);
            return brandingEntity;
        }
    }
}
