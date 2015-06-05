// <copyright file="PostTranformationJobActivities.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Post Tranformation Job Activities
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PostTransformationActivitiesJob.Helpers
{ 
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Provisioning.Core.Authentication;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Publishing;

    /// <summary>
    /// Post Transformation Job Activities
    /// </summary>
    public abstract class PostTranformationJobActivities
    {
        /// <summary>
        /// Themes info
        /// </summary>
        private static ThemeEntity themeInfo;

        /// <summary>
        /// Post transformation activities job.
        /// </summary>
        public void PostTransformationActivitiesJob()
        {
            AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

            using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContextForGivenUrl(GlobalData.MigrationRequestSiteUrl))
            {
                Tenant tenantSite = new Tenant(tenantContext);
                var site = tenantSite.GetSiteByUrl(GlobalData.MigrationRequestSiteUrl);
                using (var context = site.Context.Clone(GlobalData.MigrationRequestSiteUrl))
                {
                    this.ActivateSandBoxSolution(context);
                }
            }
        }

        /// <summary>
        /// Installs the design package.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="site">The site.</param>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="solutionGalleryList">The solution gallery list.</param>
        /// <param name="solutionGalleryListTitle">The solution gallery list title.</param>
        /// <param name="fileName">Name of the file.</param>
        protected static void InstallDesignPackage(ClientContext context, Site site, Web rootWeb, List solutionGalleryList, string solutionGalleryListTitle, string fileName)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Checking whether {0} library exists or not...", solutionGalleryListTitle), LogEventID.InformationWrite);

                // Checking whether the list exists or not.
                if (solutionGalleryList != null)
                {
                    rootWeb.Context.Load(solutionGalleryList, s => s.RootFolder.ServerRelativeUrl);
                    rootWeb.Context.ExecuteQuery();

                    // Getting the root folder of site
                    var rootFolder = rootWeb.RootFolder;
                    rootWeb.Context.Load(rootFolder, f => f.ServerRelativeUrl);

                    // Query to check whether the sandbox solution exists or not.
                    CamlQuery sandboxSolutionQuery = new CamlQuery
                    {
                        ViewXml = string.Format(CultureInfo.InvariantCulture, MigrationConstants.SandboxSolutionQuery, GlobalData.SandboxedListDefinitionWSP)
                    };
                    ListItemCollection solutionGalleryListItems = solutionGalleryList.GetItems(sandboxSolutionQuery);
                    rootWeb.Context.Load(solutionGalleryListItems);
                    rootWeb.Context.ExecuteQuery();

                    if (solutionGalleryListItems.Count == 1)
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} library exists.", solutionGalleryListTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                        Guid guid = new Guid();

                        if (GlobalData.SandboxedListDefinitionWSPGUID != null)
                        {
                            guid = new Guid(GlobalData.SandboxedListDefinitionWSPGUID);
                        }

                        // Get the DesignPackageInfo
                        var packageInfo = new DesignPackageInfo()
                        {
                            PackageName = fileName,
                            PackageGuid = guid,
                            MajorVersion = GlobalData.SandboxedSolutionMajorVersion,
                            MinorVersion = GlobalData.SandboxedSolutionMinorVersion
                        };

                        // Install the solution from the file url
                        var filerelativeurl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", solutionGalleryList.RootFolder.ServerRelativeUrl, GlobalData.SandboxedListDefinitionWSP);

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Installing design package in site {0}...", rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        DesignPackage.Install(context, site, packageInfo, filerelativeurl);
                        context.ExecuteQueryRetry();

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Applying design package in site {0}...", rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        DesignPackage.Apply(context, site, packageInfo);
                        context.ExecuteQueryRetry();

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Successfully activated {0} sandboxed solution in site {1}.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} sandboxed solution is not exists in site {1}.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not activaing sandboxed solution in site {0} because list {1} doesn't exists.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PostTransformationActivitiesJob.Helpers.PostTranformationJobActivities.InstallDesignPackage() - Error occured while installing {0} sandbox solution design package in site {1}.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url);
                ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Gets the theme details from XML.
        /// </summary>
        protected static void GetThemeDetailsFromXML()
        {
            LogHelper.LogInformation("Serialize themes package xml to object.", LogEventID.InformationWrite);

            string xmlPath = Path.GetFullPath(GlobalData.ThemeXmlLocation);

            if (!string.IsNullOrEmpty(xmlPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ThemeEntity));
                using (TextReader textReader = new System.IO.StreamReader(xmlPath))
                {
                    themeInfo = (ThemeEntity)deserializer.Deserialize(textReader);
                }

                LogHelper.LogInformation("Serialized theme xml to object.", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Adds the theme to sites.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="site">The site.</param>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="siteURL">The site URL.</param>
        protected static void AddThemeToSites(ClientContext context, Site site, Web rootWeb, string siteURL)
        {
            // Getting relative url
            Uri siteAbsoluteUrl = new Uri(siteURL);
            string webUrl = Uri.UnescapeDataString(siteAbsoluteUrl.AbsolutePath);

            // Getting the site
            Web web = site.OpenWeb(webUrl);
            context.Load(web, w => w.Url, w => w.Title, w => w.ServerRelativeUrl);
            context.ExecuteQuery();

            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Client context created for site {0}", siteURL), LogEventID.InformationWrite);

            UploadAndSetTheme(rootWeb, web);

            WebCollection subSites = web.Webs;
            context.Load(subSites);
            context.ExecuteQuery();

            // Sub-Sites
            if (subSites.Count > 0)
            {
                foreach (Web subSite in subSites)
                {
                    try
                    {
                        AddThemeToSites(context, site, rootWeb, subSite.Url);
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while runing the site migration job for sub-site {0}).", subSite.Url);
                        ExceptionLogging(ex, errorData);
                    }
                }
            }
        }

        /// <summary>
        /// Exception logging
        /// </summary>
        /// <param name="ex">Exception Object</param>
        /// <param name="errorData">Error message</param>
        protected static void ExceptionLogging(Exception ex, string errorData)
        {
            LogHelper.LogInformation(errorData, JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            LogHelper.LogError(ex, LogEventID.ExceptionHandling);
        }

        /// <summary>
        /// Installs SandBox Solution
        /// </summary>
        /// <param name="succesfullyMigratedSites">Contains successfully migrated sites</param>
        protected virtual void InstallSandBoxSolutionAndUploadTheme(List<string> succesfullyMigratedSites)
        {
            string solutionGalleryListTitle = MigrationConstants.SolutionGallery;
            string fileName = Path.GetFileNameWithoutExtension(GlobalData.SandboxedListDefinitionWSP);

            GetThemeDetailsFromXML();

            for (int count = 0; count < succesfullyMigratedSites.Count; count++)
            {
                try
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Creating tenant context for site {0}", succesfullyMigratedSites[count]), LogEventID.InformationWrite);

                    AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                    using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContextForGivenUrl(succesfullyMigratedSites[count]))
                    {
                        Tenant tenantSite = new Tenant(tenantContext);
                        var site = tenantSite.GetSiteByUrl(GlobalData.MigrationRequestSiteUrl);
                        using (var context = site.Context.Clone(succesfullyMigratedSites[count]))
                        {
                            context.Load(context.Site.RootWeb);
                            context.ExecuteQuery();

                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Tenant context created for site {0}", succesfullyMigratedSites[count]), LogEventID.InformationWrite);

                            // Root site
                            Web rootWeb = context.Site.RootWeb;
                            context.Load(rootWeb, w => w.Url);
                            ListCollection lists = rootWeb.Lists;
                            List solutionGalleryList = null;
                            IEnumerable<List> existingLists = rootWeb.Context.LoadQuery(lists.Where(list => list.Title == solutionGalleryListTitle));
                            rootWeb.Context.ExecuteQuery();
                            solutionGalleryList = existingLists.FirstOrDefault();

                            InstallDesignPackage(context, context.Site, rootWeb, solutionGalleryList, solutionGalleryListTitle, fileName);

                            AddThemeToSites(context, site, rootWeb, succesfullyMigratedSites[count]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while activating {0} sandbox solution in site {1}.", GlobalData.SandboxedListDefinitionWSP, succesfullyMigratedSites[count]);
                    ExceptionLogging(ex, errorData);
                }
            }
        }

        /// <summary>
        /// Uploads the and set theme.
        /// </summary>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="web">The web.</param>
        private static void UploadAndSetTheme(Web rootWeb, Web web)
        {
            try
            {
                // Check whether to run the operation or not.
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Running themes operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                if (!string.IsNullOrEmpty(themeInfo.ColorFile))
                {
                    rootWeb.UploadThemeFile(string.Format(CultureInfo.InvariantCulture, "{0}", Path.GetFullPath(themeInfo.ColorFile)));
                    LogHelper.LogInformation(string.Format("Completed Uploading Color File {0}.", themeInfo.ColorFile), LogEventID.InformationWrite);
                }

                rootWeb.CreateComposedLookByName(themeInfo.Name, themeInfo.ColorFile, themeInfo.FontFile, themeInfo.BackgroundFile, themeInfo.MasterPage);
                LogHelper.LogInformation(string.Format("Created Composed look {0}: for site {1}", themeInfo.Name, web.Url), LogEventID.InformationWrite);

                web.SetThemeBasedOnName(rootWeb, themeInfo.Name);
                LogHelper.LogInformation(string.Format("Seting theme {0}: for site {1}", themeInfo.Name, web.Url), LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while applying the themes for site {0}.", web.Url);
                ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Traversing through SiteMigrationRequest list to get the successfully migrated sites
        /// </summary>
        /// <param name="context">ClientContext instance</param>
        /// <param name="web">Web instance</param>
        /// <param name="siteMigrationRequestList">Site Migration Request list title</param>
        /// <returns>
        /// Returns site migration requests
        /// </returns>
        private List<string> TraverseSiteMigrationList(ClientContext context, Web web, List siteMigrationRequestList)
        {
            LogHelper.LogInformation("Loading site migration request list items...", LogEventID.InformationWrite);

            List<string> siteMigrationSuccessItems = new List<string>();

            while (true)
            {
                CamlQuery queryToGetSiteMigrationSuccessItems = new CamlQuery
                {
                    ViewXml = MigrationConstants.SiteMigrationSuccessCamlQuery
                };
                ListItemCollection siteMigrationRequests = siteMigrationRequestList.GetItems(queryToGetSiteMigrationSuccessItems);
                web.Context.Load(siteMigrationRequests);
                web.Context.ExecuteQuery();

                // Item postion
                var itemPosition = siteMigrationRequests.ListItemCollectionPosition;

                if (siteMigrationRequests.Count > 0)
                {
                    foreach (ListItem siteMigrationRequest in siteMigrationRequests)
                    {
                        try
                        {
                            FieldUrlValue siteUrlField = (FieldUrlValue)siteMigrationRequest[MigrationConstants.SiteURLColumn];

                            if (!string.IsNullOrEmpty(siteUrlField.Url) && siteUrlField != null)
                            {
                                siteMigrationSuccessItems.Add(siteUrlField.Url);
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PostTransformationActivitiesJob.Helpers.PostTranformationJobActivities.TraverseSiteMigrationList() - Site url field is null or empty. Item: {0}.", siteUrlField), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging(ex, string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PostTransformationActivitiesJob.Helpers.PostTranformationJobActivities.TraverseSiteMigrationList() - Error occured while accesing the succesful migrated sites list items."));
                        }
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PostTransformationActivitiesJob.Helpers.PostTranformationJobActivities.TraverseSiteMigrationList() - There are no successfully migrated sites present in the {0} list.", GlobalData.MigrationRequestListTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }

                // If item position is null then break the while loop.
                if (itemPosition == null)
                {
                    LogHelper.LogInformation("Loaded site migration request list items...", LogEventID.InformationWrite);
                    break;
                }
            }

            return siteMigrationSuccessItems;
        }
        
        /// <summary>
        /// Activates sandbox solution
        /// </summary>
        /// <param name="context">The context.</param>
        private void ActivateSandBoxSolution(ClientContext context)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PostTransformationActivitiesJob.Helpers.PostTranformationJobActivities.ActivateSandBoxSolution() - Getting the sites which are successfully migrated from list {0}...", GlobalData.MigrationRequestListTitle), LogEventID.InformationWrite);

                // Site Collection context 
                var site = context.Site;
                context.Load(site, s => s.Url, s => s.ServerRelativeUrl, s => s.RootWeb.Url);

                // Root site
                Web rootWeb = site.RootWeb;
                context.Load(rootWeb, w => w.Url, w => w.Lists);
                List siteMigrationRequestList = rootWeb.Lists.GetByTitle(GlobalData.MigrationRequestListTitle);
                context.Load(siteMigrationRequestList);
                context.ExecuteQuery();

                // Getting the sites which are succesfully migrated
                List<string> successfullyMigratedSites = this.TraverseSiteMigrationList(context, rootWeb, siteMigrationRequestList);

                OnPremisePostTranformationJobActivities onPremisePostTransformationJobActivities = new OnPremisePostTranformationJobActivities();
                OnlinePostTranformationJobActivities onlinePostTransformationJobActivities = new OnlinePostTranformationJobActivities();

                if (GlobalData.SharePointOnPremKey)
                {
                    onPremisePostTransformationJobActivities.InstallSandBoxSolutionAndUploadTheme(successfullyMigratedSites);
                }
                else
                {
                    onlinePostTransformationJobActivities.InstallSandBoxSolutionAndUploadTheme(successfullyMigratedSites);
                }

                LogHelper.LogInformation("Completed activating sandbox solutions.", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PostTransformationActivitiesJob.Helpers.PostTranformationJobActivities.ActivateSandBoxSolution() - Error occured while accessing the context.");
                ExceptionLogging(ex, errorData);
            }
        }
    }
}
