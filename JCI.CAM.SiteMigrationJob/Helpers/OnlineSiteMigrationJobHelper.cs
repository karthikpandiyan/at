// <copyright file="OnlineSiteMigrationJobHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Migration job 
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteMigrationJob.Helpers
{   
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Helpers;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.SiteMigrationJob.Entities;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Site Migration Job Helper Class
    /// </summary>
    public class OnlineSiteMigrationJobHelper : SiteMigrationJobHelper
    {
        /// <summary>
        /// Site Migration Job
        /// </summary>
        /// <param name="siteURL">Site URL</param>
        /// <returns>Returns the error message during site migration job</returns>
        public override string SiteMigrationJob(string siteURL)
        {
            AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

            using (ClientContext context = tenantAuthentication.GetAuthenticatedContextForGivenUrl(siteURL))
            {
                context.RequestTimeout = 120000;

                // Tenant admin context
                Tenant tenant = new Tenant(context);

                // Site Collection context 
                var site = tenant.GetSiteByUrl(siteURL);
                context.Load(site, s => s.Url, s => s.ServerRelativeUrl, s => s.RootWeb.Url);
                context.ExecuteQuery();

                // Getting relative url
                Uri siteAbsoluteUrl = new Uri(siteURL);
                string webUrl = Uri.UnescapeDataString(siteAbsoluteUrl.AbsolutePath);

                // Getting the site
                var web = site.OpenWeb(webUrl);
                site.Context.Load(web, w => w.Url, w => w.Title, w => w.ServerRelativeUrl, w => w.MasterUrl, w => w.CustomMasterUrl, w => w.RootFolder.WelcomePage, w => w.WebTemplate);
                site.Context.ExecuteQuery();
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Site Migration Job is started for site {0} ({1}).", web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                SiteMigrationJobHelper.SiteMigrationJobOperations(site, web);
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Site Migration Job is completed for site {0} ({1}).", web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

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
                             bool isAppWebSite = false;
                                if (ConfigurationManager.AppSettings["SharePointHostedAppDomains"] != null)
                                {
                                    string[] appDomains = ConfigurationManager.AppSettings["SharePointHostedAppDomains"].Split(',').ToArray();                                   
                                    foreach (string appDomain in appDomains)
                                    {
                                        if (subSite.Url.ToUpperInvariant().Contains(appDomain.ToUpperInvariant()))
                                        {
                                            LogHelper.LogInformation(string.Format("Skipping AppWeb site from transformation-{0}", subSite.Url), LogEventID.InformationWrite);
                                            isAppWebSite = true;
                                            break;
                                        }
                                    }
                                }

                                if (!isAppWebSite)
                                {
                                    this.SiteMigrationJob(subSite.Url);
                                }
                        }
                        catch (Exception ex)
                        {
                            string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while runing the site migration job for sub-site {0}).", siteURL);
                            MigrationCommonHelper.ExceptionLogging(ex, errorData);
                            SiteMigrationJobHelper.SiteMigrationErrorData = SiteMigrationJobHelper.SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                        }
                    }
                }
            }

            return SiteMigrationJobHelper.SiteMigrationErrorData;
        }

        /// <summary>
        /// Deletes the page layouts.
        /// </summary>
        /// <param name="siteURL">Site Url</param>
        /// <param name="siteMigrationErrorData">Error Data</param>
        /// <param name="pageLayoutsInfo">Page layouts Details</param>
        /// <returns>Returns error data</returns>
        public override string DeletePageLayouts(string siteURL, string siteMigrationErrorData, List<PageLayouts.PageLayout> pageLayoutsInfo)
        {
            string rootSiteUrl = string.Empty;

            try
            {
                AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                using (ClientContext context = tenantAuthentication.GetAuthenticatedContextForGivenUrl(siteURL))
                {
                    // Tenant admin context
                    Tenant tenant = new Tenant(context);

                    // Site Collection context 
                    var site = tenant.GetSiteByUrl(siteURL);
                    context.Load(site, s => s.Url, s => s.ServerRelativeUrl, s => s.RootWeb.Url);
                    context.ExecuteQuery();

                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started deleting page layouts..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                    // Root web
                    Web rootWeb = site.RootWeb;
                    rootSiteUrl = rootWeb.Url;
                    List publishingLayouts = rootWeb.Lists.GetByTitle(MigrationConstants.MasterPageGallery);

                    for (int count = 0; count < pageLayoutsInfo.Count; count++)
                    {
                        try
                        {
                            string pageLayoutInfoUrl = pageLayoutsInfo[count].OldPageLayout;
                            string fileName = Path.GetFileName(pageLayoutInfoUrl);

                            CamlQuery pageLayoutQuery = new CamlQuery
                            {
                                ViewXml = @"<View><Query><Where><Eq><FieldRef Name='FileLeafRef' /><Value Type='File'>" + fileName + "</Value></Eq></Where></Query></View>"
                            };
                            ListItemCollection pagelayouts = publishingLayouts.GetItems(pageLayoutQuery);
                            rootWeb.Context.Load(pagelayouts);
                            rootWeb.Context.ExecuteQuery();

                            if (pagelayouts.Count > 0)
                            {
                                ListItem layout = pagelayouts.FirstOrDefault();
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Deleting {0} page layout.", fileName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                                layout.DeleteObject();
                                rootWeb.Context.ExecuteQuery();
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Deleted {0} page layout.", fileName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not able to delete {0} page layout because the page is not identified in master page gallery.", fileName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                        catch (Exception ex)
                        {
                            string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while deleting the {0} pagelayout for site {1}.", Path.GetFileName(pageLayoutsInfo[count].OldPageLayout), rootWeb.Url);
                            MigrationCommonHelper.ExceptionLogging(ex, errorData);
                            siteMigrationErrorData = siteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while accesing the list {0} for site {1}.", MigrationConstants.MasterPageGallery, rootSiteUrl);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                siteMigrationErrorData = siteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }

            return siteMigrationErrorData;
        }
    }
}
