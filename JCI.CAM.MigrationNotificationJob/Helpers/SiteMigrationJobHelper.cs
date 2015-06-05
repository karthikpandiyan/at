// <copyright file="SiteMigrationJobHelper.cs" company="Microsoft">
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
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Migration.Common.Helpers;
    using JCI.CAM.Provisioning.Core;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.SiteMigrationJob.Entities;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using Microsoft.SharePoint.Client.WebParts;   

    /// <summary>
    /// Site Migration Job Helper Class
    /// </summary>
    public abstract class SiteMigrationJobHelper
    {     
         /// <summary>
        /// Features info
        /// </summary>
        private static List<Features.Feature> featureInfo;

        /// <summary>
        /// Themes info
        /// </summary>
        private static ThemeEntity themeInfo;

        /// <summary>
        /// List event receivers info
        /// </summary>
        private static List<ListEventReceivers.Receiver> listEventReceiverInfo;

        /// <summary>
        /// Page layouts info
        /// </summary>
        private static List<PageLayouts.PageLayout> pageLayoutsInfo;

        /// <summary>
        /// The web part details
        /// </summary>
        private static List<string> webpartsDetails;

        /// <summary>
        /// Schema common file path
        /// </summary>
        private static string commonFilePath;

        /// <summary>
        /// Header Footer script block
        /// </summary>
        private static string headerFooterScriptBlock;

        /// <summary>
        /// Test site banner script block
        /// </summary>
        private static string testSiteBannerScriptBlock;

        /// <summary>
        /// SPD settings injection script block
        /// </summary>
        private static string sharePointDesignerScriptBlock;

        /// <summary>
        /// sub site link injection script block
        /// </summary>
        private static string subsiteLinkScriptBlock;

        /// <summary>
        /// The migration requested site URL 
        /// </summary>
        private static string requestedSiteURL;

        /// <summary>
        /// Gets or sets the error data while running the site migration job.
        /// </summary>
        protected static string SiteMigrationErrorData { get; set; }

        /// <summary>
        /// Updates the site migration status in Site Migration list
        /// </summary>
        /// <param name="siteMigrationRequest">SiteMigrationRequest object</param>
        /// <param name="status">Site migration job status</param>
        /// <param name="siteMigrationErrorData">Error details</param>        
        /// <param name="listName">Site migration request list name</param>
        public static void UpdateSiteMigrationRequestStatus(SiteMigrationRequest siteMigrationRequest, string status, string siteMigrationErrorData, string listName)
        {
            try
            {
                AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContextForGivenUrl(GlobalData.MigrationRequestSiteUrl))
                {
                    Tenant tenantSite = new Tenant(tenantContext);
                    var site = tenantSite.GetSiteByUrl(GlobalData.MigrationRequestSiteUrl);
                    using (var context = site.Context.Clone(GlobalData.MigrationRequestSiteUrl))
                    {
                        var web = context.Web;
                    context.Load(web, w => w.Url, w => w.Title);
                    List siteMigrationRequestList = web.Lists.GetByTitle(listName);
                    context.Load(siteMigrationRequestList);
                    ListItem siteMigrationRequestListItem = siteMigrationRequestList.GetItemById(siteMigrationRequest.ListItemId);
                    context.Load(siteMigrationRequestListItem);
                    context.ExecuteQuery();

                    if (!string.IsNullOrEmpty(siteMigrationErrorData.Trim()) || status.Equals(SiteMigrationRequestStatus.Failed.ToString()))
                    {
                        string statusMessage = string.Format("{0} - {1}.", siteMigrationErrorData, DateTime.Now.ToString());

                        siteMigrationRequestListItem[MigrationConstants.SiteMigrationStatusColumn] = GlobalData.MigrationFailedStatus;
                        siteMigrationRequestListItem[MigrationConstants.SiteMigrationErrorDataColumn] = siteMigrationRequestListItem[MigrationConstants.SiteMigrationErrorDataColumn] + " " + statusMessage;
                    }
                    else
                    {
                        siteMigrationRequestListItem[MigrationConstants.SiteMigrationStatusColumn] = GlobalData.MigrationSuccessStatus;
                        siteMigrationRequestListItem[MigrationConstants.SiteMigrationErrorDataColumn] = string.Empty;
                    }

                    siteMigrationRequestListItem.Update();
                    web.Context.ExecuteQuery();
                }
            }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating site migration request.");
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Gets feature details from xml file
        /// </summary>
        /// <param name="xmlPath">Feature xml file path</param>
        public static void GetFeatureDetailsFromXML(string xmlPath)
        {
            LogHelper.LogInformation("Serializing feature xml to object.", LogEventID.InformationWrite);

            if (!string.IsNullOrEmpty(xmlPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<Features.Feature>), new XmlRootAttribute(MigrationConstants.FeatureXMLRootElement));
                using (TextReader textReader = new System.IO.StreamReader(xmlPath))
                {
                    featureInfo = (List<Features.Feature>)deserializer.Deserialize(textReader);
                }

                LogHelper.LogInformation("Serialized feature xml to object.", LogEventID.InformationWrite);
            }            
        }

        /// <summary>
        /// Gets the template configuration.
        /// </summary>
        /// <param name="xmlPath">Theme xml file path</param>
        public static void GetThemeDetailsFromXML(string xmlPath)
        {
            LogHelper.LogInformation("Serialize themes package xml to object.", LogEventID.InformationWrite);
            
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
        /// Gets the list event receiver details from xml file.
        /// </summary>
        /// <param name="xmlPath">List event receiver xml file path</param>
        public static void GetListEventReceiverDetailsFromXML(string xmlPath)
        {
            LogHelper.LogInformation("Serializing list event receiver xml to object.", LogEventID.InformationWrite);

            if (!string.IsNullOrEmpty(xmlPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<ListEventReceivers.Receiver>), new XmlRootAttribute(MigrationConstants.ListEventReceiverXMLRootElement));
                using (TextReader textReader = new System.IO.StreamReader(xmlPath))
                {
                    listEventReceiverInfo = (List<ListEventReceivers.Receiver>)deserializer.Deserialize(textReader);
                }

                LogHelper.LogInformation("Serialized list event receiver xml to object.", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Gets the page layout details from xml file
        /// </summary>
        /// <param name="xmlPath">Page layout xml file path</param>
        /// <returns>Returns Page layout details</returns>
        public static List<PageLayouts.PageLayout> GetPageLayoutDetailsFromXML(string xmlPath)
        {
            LogHelper.LogInformation("Serializing page layout xml to object.", LogEventID.InformationWrite);

            if (!string.IsNullOrEmpty(xmlPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<PageLayouts.PageLayout>), new XmlRootAttribute(MigrationConstants.PageLayoutsXMLRootElement));
                using (TextReader textReader = new System.IO.StreamReader(xmlPath))
                {
                    pageLayoutsInfo = (List<PageLayouts.PageLayout>)deserializer.Deserialize(textReader);
                }

                LogHelper.LogInformation("Serialized page layout xml to object.", LogEventID.InformationWrite);
            }

            return pageLayoutsInfo;
        }

        /// <summary>
        /// Get custom actions script block from XML file
        /// </summary>
        /// <param name="xmlPath">Custom actions xml file path</param>
        public static void GetCustomActionsScriptBlockFromXML(string xmlPath)
        {
            List<CustomActions.CustomAction> customActionsInfo = new List<CustomActions.CustomAction>();

            LogHelper.LogInformation("Serializing custom actions xml to object.", LogEventID.InformationWrite);

            if (!string.IsNullOrEmpty(xmlPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<CustomActions.CustomAction>), new XmlRootAttribute(MigrationConstants.CustomActionsXMLRootElement));
                using (TextReader textReader = new System.IO.StreamReader(xmlPath))
                {
                    customActionsInfo = (List<CustomActions.CustomAction>)deserializer.Deserialize(textReader);
                }

                LogHelper.LogInformation("Serialized custom actions xml to object.", LogEventID.InformationWrite);

                // Getting script block of custom action
                for (int count = 0; count < customActionsInfo.Count; count++)
                {
                    try
                    {
                        string customActionName = customActionsInfo[count].Name;

                        switch (customActionName)
                        {
                            case MigrationConstants.HeaderFooterCustomActionName:
                                headerFooterScriptBlock = customActionsInfo[count].ScriptBlock;
                                break;
                            case MigrationConstants.TestSiteBannerCustomActionName:
                                testSiteBannerScriptBlock = customActionsInfo[count].ScriptBlock;
                                break;
                            case MigrationConstants.SPDSettingsCustomActionName:
                                sharePointDesignerScriptBlock = customActionsInfo[count].ScriptBlock;
                                break;
                            case MigrationConstants.SubsiteLinkCustomActionName:
                                subsiteLinkScriptBlock = customActionsInfo[count].ScriptBlock;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationJob.Helpers.SiteMigrationJobHelper.GetCustomActionsScriptBlockFromXML() - Error occurred while getting the script block for custom actions.");
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    }
                }
            }
        }

        /// <summary>
        /// Gets feature details from xml file
        /// </summary>
        /// <param name="path">Common schema file path</param>
        public static void GetCommonFilePath(string path)
        {
            commonFilePath = path;            
        }        

        /// <summary>
        /// Empty the site migration error data 
        /// </summary>
        public static void EmptySiteMigrationErrorData()
        {
            SiteMigrationErrorData = string.Empty;
        }

        /// <summary>
        /// Requested migration site URL.
        /// </summary>
        /// <param name="siteURL">The site URL.</param>
        public static void MigrationRequestedSiteURL(string siteURL)
        {
            requestedSiteURL = siteURL;
        }

        /// <summary>
        /// Site Migration Job
        /// </summary>
        /// <param name="siteURL">Site URL</param>
        /// <returns>Returns the error message during site migration job</returns>
        public virtual string SiteMigrationJob(string siteURL)
        {
            LogHelper.LogInformation(string.Format("Starting transformation for site -{0}", siteURL), LogEventID.InformationWrite);
            AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

            using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContextForGivenUrl(siteURL))
            {
                tenantContext.RequestTimeout = 120000;
                Tenant tenantSite = new Tenant(tenantContext);
                var siteContext = tenantSite.GetSiteByUrl(siteURL);
                using (var context = siteContext.Context.Clone(siteURL))
                {
                    // Site Collection context 
                    var site = context.Site;
                    context.Load(site, s => s.Url, s => s.ServerRelativeUrl, s => s.RootWeb.Url);
                    context.ExecuteQuery();

                    // Getting relative url
                    Uri siteAbsoluteUrl = new Uri(siteURL);
                    string webUrl = Uri.UnescapeDataString(siteAbsoluteUrl.AbsolutePath);

                    // Getting the site
                    var web = context.Web;
                    site.Context.Load(web, w => w.Url, w => w.Title, w => w.ServerRelativeUrl, w => w.MasterUrl, w => w.CustomMasterUrl, w => w.RootFolder.WelcomePage, w => w.WebTemplate);
                    site.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Site Migration Job is started for site {0} ({1}).", web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    SiteMigrationJobOperations(site, web);
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
                                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while runing the site migration job for sub-site {0}).", subSite.Url);
                                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                            }
                        }
                    }
                }
            }

            return SiteMigrationErrorData;
        }        

        /// <summary>
        /// Sends the email notification.
        /// </summary>
        /// <param name="siteMigrationRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="emailSubject">Email Subject</param>
        /// <param name="emailBody">Email body</param>
        public virtual void SendEmailNotification(SiteMigrationRequest siteMigrationRequest, string status, string emailSubject, string emailBody)
        {
            if (GlobalData.SendEmailNotificationForSiteMigrationStatus)
            {
                LogHelper.LogInformation("Sending email notification regarding the status of site migration job...", LogEventID.InformationWrite);

                string subject = string.Empty;
                string body = string.Empty;

                AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContextForGivenUrl(GlobalData.SiteProvisioningSiteUrl))
                {
                    LogHelper.LogInformation("Tenant context acquired", LogEventID.InformationWrite);

                    try
                    {
                        Tenant tenantSite = new Tenant(tenantContext);
                        var site = tenantSite.GetSiteByUrl(GlobalData.SiteProvisioningSiteUrl);
                        using (var ctx = site.Context.Clone(GlobalData.SiteProvisioningSiteUrl))
                        {
                            var configWeb = ctx.Web;

                            // Get values from Config list
                            subject = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, emailSubject, GlobalData.WorkflowConfigurationListName);
                            body = System.Net.WebUtility.HtmlDecode(ConfigListHelper.GetConfigurationListValue(ctx, configWeb, emailBody, GlobalData.WorkflowConfigurationListName));

                            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
                            {
                                LogHelper.LogInformation("Email message is set and context changed", LogEventID.InformationWrite);

                                List<SharePointUser> siteOwners = siteMigrationRequest.SiteOwners;

                                List<string> emailIDs = new List<string>();

                                for (int count = 0; count < siteOwners.Count; count++)
                                {
                                    if (!string.IsNullOrEmpty(siteOwners[count].Email))
                                    {
                                        try
                                        {
                                            emailIDs.Add(siteOwners[count].Email);
                                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Added {0} receipient to send the mail.", siteOwners[count].Email), LogEventID.InformationWrite);
                                        }
                                        catch (Exception ex)
                                        {
                                            string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationJob.Helpers.SendEmailNotification() - Error occurred while assigning the email collection. Email ID: {0}.", siteOwners[count].Email);
                                            MigrationCommonHelper.ExceptionLogging(ex, errorData);
                                        }
                                    }
                                    else
                                    {
                                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationJob.Helpers.SendEmailNotification() - Email ID is null or empty for user {0}. Email ID: {1}.", siteOwners[count].LoginName, siteOwners[count].Email), LogEventID.InformationWrite);
                                    }
                                }

                                if (emailIDs.Count > 0)
                                {
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Total receipients : {0}", emailIDs), LogEventID.InformationWrite);

                                    // Email properties
                                    LogHelper.LogInformation("Email message processing - site migration job...", LogEventID.InformationWrite);
                                    EmailProperties properties = new EmailProperties
                                    {
                                        To = emailIDs,
                                        Subject = subject,
                                        Body = string.Format(CultureInfo.InvariantCulture, body, siteMigrationRequest.SiteTitle, siteMigrationRequest.SiteURL)
                                    };
                                    Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(ctx, properties);
                                    ctx.ExecuteQuery();
                                    LogHelper.LogInformation("Email message sent regarding site migration job.", LogEventID.InformationWrite);
                                }
                                else
                                {
                                    LogHelper.LogInformation("Not sending email because there are no receipients.", LogEventID.InformationWrite);
                                }
                            }
                            else
                            {
                                LogHelper.LogInformation("Site migration job - subject and body are empty.", LogEventID.InformationWrite);
                            }

                            LogHelper.LogInformation("Sending email notification Completed.", LogEventID.InformationWrite);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationJob.Helpers.SendEmailNotification() - Error occurred while sending email.");
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    }
                }
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not sending email notification regarding site migrtaion status for site {0}.", siteMigrationRequest.SiteURL), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Deletes the page layouts.
        /// </summary>
        /// <param name="siteURL">Site Url</param>
        /// <param name="siteMigrationErrorData">Error Data</param>
        /// <param name="pageLayoutsInfo">Page layouts Details</param>
        /// <returns>Returns error data</returns>
        public virtual string DeletePageLayouts(string siteURL, string siteMigrationErrorData, List<PageLayouts.PageLayout> pageLayoutsInfo)
        {
            string rootSiteUrl = string.Empty;
            try
            {
                AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContextForGivenUrl(siteURL))
                {
                    Tenant tenantSite = new Tenant(tenantContext);
                    var siteContext = tenantSite.GetSiteByUrl(siteURL);
                    using (var context = siteContext.Context.Clone(siteURL))
                    {
                        // Site Collection context 
                        var site = context.Site;
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
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while accesing the list {0} for site {1}.", MigrationConstants.MasterPageGallery, rootSiteUrl);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                siteMigrationErrorData = siteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }

            return siteMigrationErrorData;
        }

        /// <summary>
        /// Site Migration Job Operations
        /// </summary>
        /// <param name="site">Site Instance</param>
        /// <param name="web">Web Instance</param>
        protected static void SiteMigrationJobOperations(Site site, Web web)
        {
            // Root web
            Web rootWeb = GetRootWeb(site);

            // Operations
            InjectTestSiteBannerOperation(site, web);
            GetWebpartsDetailsFromHomePageWebpartexists(web);
            DeactivateFeaturesOperation(site, web);
            ResetMasterPage(web, rootWeb);
            InjectHeaderFooterOperation(site, web);
            InjectSubSiteLinkOperation(site, web);
            SPDOperation(site, web, rootWeb);
            ApplyThemesOperation(web, rootWeb);
            AddEventReceiversToListOperation(web);
            UpdatingPageLayoutOperation(rootWeb, web);
            AddSandBoxSolutionOperation(rootWeb, web);

            // Set property bag value indicates that the site is got transformed.
            web.SetPropertyBagValue(MigrationConstants.JCICAMTransformationPropertyBagKey, MigrationConstants.JCICAMTransformationPropertyBagValue);
        }

        /// <summary>
        /// Gets the root web.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>Returns the root site</returns>
        protected static Web GetRootWeb(Site site)
        {
            Web rootWeb = site.RootWeb;
            site.Context.Load(rootWeb, w => w.Url, w => w.Title, w => w.ServerRelativeUrl, w => w.MasterUrl, w => w.CustomMasterUrl, w => w.RootFolder.WelcomePage);
            site.Context.ExecuteQuery();

            return rootWeb;
        }

        /// <summary>
        /// Injecting test site banner
        /// </summary>
        /// <param name="site">Site instance</param>
        /// <param name="web">Web instance</param>
        private static void InjectTestSiteBannerOperation(Site site, Web web)
        {
            // Check whether to run the operation or not.
            if (GlobalData.RunTestSiteBannerInjectionOperation)
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started test site banner injection operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                string testSiteBannerFeatureName = GlobalData.TestSiteBannerFeatureName;
                Guid testSiteBannerFeatureID = new Guid(GlobalData.TestSiteBannerFeatureID);

                // Check whether test site banner feature is activated or not. If activated then deactivate it and inject the banner
                if (FeatureExtensions.IsFeatureActive(web, testSiteBannerFeatureID))
                {
                    FeatureExtensions.ProcessFeatureInternal(web.Features, testSiteBannerFeatureID, false);
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} feature deactivated.", testSiteBannerFeatureName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                    string errorMessage = "Error occurred while injecting test site banner for site {0}.";

                    // Inject Test site banner
                    try
                    {
                        BrandingExtensions.AddCustomActionJSLinks(site, web, "TestSiteBannerInjection.js", "ScriptLink", testSiteBannerScriptBlock);
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, errorMessage, web.Url);
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    }

                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Injected test site banner."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} feature is not activated.", testSiteBannerFeatureName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not running test site banner injection operation."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Gets the web parts details from home page web part exists.
        /// </summary>
        /// <param name="web">The web.</param>
        private static void GetWebpartsDetailsFromHomePageWebpartexists(Web web)
        {
            try
            {
                bool isPublishingSite = false;
                string pageName = string.Empty;

                isPublishingSite = IsPublishingWeb(web);

                if (isPublishingSite)
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started updating the publishing page operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                    if (pageLayoutsInfo.Count > 0)
                    {
                        for (int count = 0; count < pageLayoutsInfo.Count; count++)
                        {
                            if (web.WebTemplate.ToLower().Equals(pageLayoutsInfo[count].WebTemplate.ToLower()))
                            {
                                pageName = pageLayoutsInfo[count].HomePage;
                                break;
                            }
                        }

                        if (pageName != string.Empty)
                        {
                            webpartsDetails = new List<string>();
                            string pageURL = web.ServerRelativeUrl + "/Pages/" + pageName;
                            pageURL = pageURL.Replace("//", "/");
                            Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(pageURL);
                            web.Context.Load(file);

                            try
                            {                                
                                web.Context.ExecuteQuery();
                            }
                            catch (Exception)
                            {                                
                                file = null;                               
                            }

                            if (file != null)
                            {                                
                                LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                                web.Context.Load(wpm.WebParts, wps => wps.Include(wp => wp.WebPart.Title));
                                web.Context.ExecuteQuery();

                                foreach (WebPartDefinition wpd in wpm.WebParts)
                                {
                                    try
                                    {
                                        WebPart wp = wpd.WebPart;

                                        if (!wp.Title.ToLower().Equals(MigrationConstants.WebPartPageTitleBarWebpartTitle.ToLower()))
                                        {
                                            webpartsDetails.Add(wp.Title);
                                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} webpart exists in the {1} welcome page.", wp.Title, file.Name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while adding the web part title to an object for site {0}.", web.Url);
                                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                                    }
                                }
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationJob.Helpers.SiteMigrationJobHelper.GetWebpartsDetailsFromHomePageWebpartexists() - {0} page doesn't exists in site {1}.", pageName, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationJob.Helpers.SiteMigrationJobHelper.GetWebpartsDetailsFromHomePageWebpartexists() - site template is not matching. Current site template is {0}.", web.WebTemplate), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while getting the webpart details for site {0}.", web.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }
        }

        /// <summary>
        /// Deactivate Feature operation
        /// </summary>
        /// <param name="site">Site instance</param>
        /// <param name="web">Web Instance</param>
        private static void DeactivateFeaturesOperation(Site site, Web web)
        {
            // Check whether to run the operation or not.
            if (GlobalData.RunDeactiveFeaturesOperation)
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started feature deactivation operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                string rootSiteURL = site.RootWeb.Url;

                for (int count = 0; count < featureInfo.Count; count++)
                {
                    try
                    {
                        // Check whether the scope of the feature is site or web.
                        if (featureInfo[count].Scope.ToLower().Equals(MigrationConstants.SiteScopeFeature) && rootSiteURL.Equals(web.Url))
                        {
                            FeatureExtensions.DeactivateFeature(site.Features, featureInfo[count].Name, featureInfo[count].ID);
                        }
                        else if (featureInfo[count].Scope.ToLower().Equals(MigrationConstants.WebScopeFeature))
                        {
                            FeatureExtensions.DeactivateFeature(web.Features, featureInfo[count].Name, featureInfo[count].ID);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while deactivating the feature {0}, {1} site.", featureInfo[count].Name, web.Url);
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    }
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Completed feature deactivation operation."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not running feature deactivation operation."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Resets the master page.
        /// </summary>
        /// <param name="currentWeb">Current web.</param>
        /// <param name="rootWeb">Root web.</param>
        private static void ResetMasterPage(Web currentWeb, Web rootWeb)
        {
            string masterPageUrl = string.Empty;

            try
            {
                masterPageUrl = string.Concat(rootWeb.ServerRelativeUrl, MigrationConstants.MasterPageUrl).Replace("//", "/");

                // Checking the master page url
                if (masterPageUrl.ToLower().Equals(currentWeb.MasterUrl.ToLower()) && masterPageUrl.ToLower().Equals(currentWeb.CustomMasterUrl.ToLower()))
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Master page already reset to {0} for site {1}.", rootWeb.MasterUrl, rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Changing master page url from {0} to {1} and custom master page url from {2} to {3} for site {4}.", currentWeb.MasterUrl, masterPageUrl, currentWeb.CustomMasterUrl, masterPageUrl, currentWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    currentWeb.SetCustomMasterPageByUrl(masterPageUrl, true);
                    currentWeb.SetMasterPageByUrl(masterPageUrl, true);
                    currentWeb.Update();
                    currentWeb.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Changed master page url from {0} to {1} and custom master page url from {2} to {3} for site {4}.", currentWeb.MasterUrl, masterPageUrl, currentWeb.CustomMasterUrl, masterPageUrl, currentWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while changing the master page from {0} to {1} for site {2}.", masterPageUrl, rootWeb.MasterUrl, currentWeb.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Injecting Header, footer and other JS, CSS files
        /// </summary>
        /// <param name="site">Site instance</param>
        /// <param name="web">Web instance</param>
        private static void InjectHeaderFooterOperation(Site site, Web web)
        {
            // Check whether to run the operation or not.
            if (GlobalData.RunHeaderFooterInjectionOperation)
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started header and footer injection operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                string errorMessage = "Error occurred injecting header and footer for site {0}.";

                // Inject branding injection
                try
                {
                    BrandingExtensions.AddCustomActionJSLinks(site, web, "Branding Injection", MigrationConstants.CustomActionLocation, headerFooterScriptBlock);
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, errorMessage, web.Url);
                    MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Injected header and footer."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not running header and footer injection operation."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Changing the sub site link to create custom sites
        /// </summary>
        /// <param name="site">Site instance</param>
        /// <param name="web">Web instance</param>
        private static void InjectSubSiteLinkOperation(Site site, Web web)
        {
            string errorMessage = "Error occurred while injecting the subsite custom action for site {0}.";

            try
            {
                // Check whether to run the operation or not.
                if (GlobalData.RunSubSiteInjectionOperation)
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started subsite injection operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                    // Inject subsite link injection
                    try
                    {
                        BrandingExtensions.AddCustomActionJSLinks(site, web, "Subsite Injection", MigrationConstants.CustomActionLocation, subsiteLinkScriptBlock);
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, errorMessage, web.Url);
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    }

                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Injected subsite link."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not running subsite link injection operation."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, errorMessage, web.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }
        }

        /// <summary>
        /// Disabling SharePoint Designer button and hiding SharePoint Designer settings operation
        /// </summary>
        /// <param name="site">Site instance</param>
        /// <param name="web">Web instance</param>
        /// <param name="rootWeb">Root site</param>
        private static void SPDOperation(Site site, Web web, Web rootWeb)
        {
            // Check whether to run the operation or not.
            if (GlobalData.RunSPDOperation)
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Running SharePoint designer operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                // Disabling SharePoint Designer button
                try
                {
                     BrandingExtensions.DisableDesigner(site);
                     LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Disabled SharePoint Designer button for site {0}.", web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);                  
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while disabling the SharePoint designer button for site {0}.", web.Url);
                    MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                }
                
                string errorMessage = "Error occurred while hiding the SharePoint designer settings for site {0}.";

                // Hiding SharePoint Designer settings option
                try
                {
                    // checking whether the current site is root or not.
                    if (rootWeb.ServerRelativeUrl.ToLower().Equals(web.ServerRelativeUrl.ToLower()))
                    {
                        BrandingExtensions.AddCustomActionJSLinks(site, site.RootWeb, "settings.js", MigrationConstants.CustomActionLocation, sharePointDesignerScriptBlock);
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Injected js to hide the SharePoint Designer settings option for site {0}.", web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not Injecting js to hide the SharePoint Designer settings option for site {0} because this is not root site.", web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }  
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, errorMessage, web.Url);
                    MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Injected script to hide SharePoint Designer settings link in settings page."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not disabling and injecting the SharePoint designer settings."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }
        
        /// <summary>
        /// Applying themes operation
        /// </summary>
        /// <param name="web">Web instance</param>
        /// <param name="rootWeb">Root site</param>
        private static void ApplyThemesOperation(Web web, Web rootWeb)
        {
            try
            {
                // Check whether to run the operation or not.
                if (GlobalData.RunApplyingThemesOperation)
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Running themes operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    
                    if (!string.IsNullOrEmpty(themeInfo.ColorFile))
                    {
                        rootWeb.UploadThemeFile(string.Format(CultureInfo.InvariantCulture, "{0}{1}", commonFilePath, themeInfo.ColorFile));
                        LogHelper.LogInformation(string.Format("Completed Uploading Color File {0}.", themeInfo.ColorFile), LogEventID.InformationWrite);
                    }

                    rootWeb.CreateComposedLookByName(themeInfo.Name, themeInfo.ColorFile, themeInfo.FontFile, themeInfo.BackgroundFile, themeInfo.MasterPage);
                    LogHelper.LogInformation(string.Format("Created Composed look {0}: for site {1}", themeInfo.Name, web.Url), LogEventID.InformationWrite);

                    web.SetThemeBasedOnName(rootWeb, themeInfo.Name);
                    LogHelper.LogInformation(string.Format("Seting theme {0}: for site {1}", themeInfo.Name, web.Url), LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not applying the themes."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while applying the themes for site {0}.", web.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }
        }
        
        /// <summary>
        /// Adding event receiver to the list
        /// </summary>
        /// <param name="web">Web instance</param>
        private static void AddEventReceiversToListOperation(Web web)
        {
            // Check whether to run the operation or not.
            if (GlobalData.RunAddingEventReceiverToListOperation)
            {
                string listTitle = string.Empty;

                ListCollection lists = web.Lists;
                web.Context.Load(lists);
                web.Context.ExecuteQuery();

                if (listEventReceiverInfo.Count > 0)
                {
                    string libraryName = GlobalData.AddEventReceiversToSharedDocumentLibrary;

                    foreach (ListEventReceivers.Receiver listEventReceiver in listEventReceiverInfo)
                    {
                        listTitle = listEventReceiver.ListTitle;
                        int listTemplateType = listEventReceiver.TemplateType;

                        foreach (List list in lists)
                        {
                            try
                            {
                                // Checking the template type
                                if (listTemplateType == list.BaseTemplate)
                                {
                                    // Checking whether the list title is empty or not.
                                    if (listTitle == null)
                                    {
                                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List details: List title - {0}.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                                        list.AddRemoteEventReceiver(listEventReceiver.Name, listEventReceiver.Url, (EventReceiverType)Enum.Parse(typeof(EventReceiverType), listEventReceiver.Type, true), (EventReceiverSynchronization)Enum.Parse(typeof(EventReceiverSynchronization), listEventReceiver.Synchronization, true), listEventReceiver.SequenceNumber, true);
                                    }
                                    else
                                    {
                                        if (listTitle.Equals(list.EntityTypeName))
                                        {
                                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List details: List title - {0}.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                                            list.AddRemoteEventReceiver(listEventReceiver.Name, listEventReceiver.Url, (EventReceiverType)Enum.Parse(typeof(EventReceiverType), listEventReceiver.Type, true), (EventReceiverSynchronization)Enum.Parse(typeof(EventReceiverSynchronization), listEventReceiver.Synchronization, true), listEventReceiver.SequenceNumber, true);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while adding the event receiver to the list {0}.", listTitle);
                                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                            }
                        }
                    }

                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Adding event receiver to lists operation completed."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation("List event receiver xml is invalid or empty.", LogEventID.InformationWrite);
                }
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not adding event reciever to the list."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Adding sandbox solution to the site operation
        /// </summary>
        /// <param name="rootWeb">Root site</param>
        /// <param name="web">Current site</param>
        private static void AddSandBoxSolutionOperation(Web rootWeb, Web web)
        {
            // Check whether to run the operation or not.
            if (GlobalData.AddSandBoxSolutionOperation)
            {
                // checking whether the current site is root or not.
                if (rootWeb.ServerRelativeUrl.ToLower().Equals(web.ServerRelativeUrl.ToLower()))
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Adding SandBox solution..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                    try
                    {
                        string listTitle = MigrationConstants.SolutionGallery;

                        ListCollection lists = rootWeb.Lists;
                        List solutionGalleryList = null;
                        IEnumerable<List> existingLists = rootWeb.Context.LoadQuery(lists.Where(list => list.Title == listTitle));
                        rootWeb.Context.ExecuteQuery();
                        solutionGalleryList = existingLists.FirstOrDefault();

                        // Checking whether the list exists or not.
                        if (solutionGalleryList != null)
                        {
                            string solutionFullPath = Path.Combine(commonFilePath, GlobalData.SandBoxSolutionFilePath);
                            FileInfo fileInfo = new FileInfo(solutionFullPath);

                            // Uploading the sandbox solution WSP file
                            FileCreationInformation newFile = new FileCreationInformation
                            {
                                Content = System.IO.File.ReadAllBytes(solutionFullPath),
                                Url = fileInfo.Name,
                                Overwrite = true
                            };
                            Microsoft.SharePoint.Client.File uploadFile = solutionGalleryList.RootFolder.Files.Add(newFile);
                            rootWeb.Context.Load(uploadFile);
                            rootWeb.Context.ExecuteQuery();

                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Successfully added sandboxed solution to site {0}. File Path : {1}.", rootWeb.Url, solutionFullPath), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not adding sandboxed solution to the site {0} because list {1} doesn't exists.", rootWeb.Url, listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while adding the sand box solution to site {0}.", rootWeb.Url);
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not adding sandbox solution because site {0}({1}) is not a root site.", web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                } 
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not adding sand box solution."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }        

        /// <summary>
        /// Update the existing custom publishing page layout to default blank layout
        /// </summary>
        /// <param name="rootWeb">Root site</param>
        /// <param name="web">Web instance</param>
        private static void UpdatingPageLayoutOperation(Web rootWeb, Web web)
        {
            // Check whether to run the operation or not.
            if (GlobalData.UpdatePageLayoutOperation)
            {
                bool isPublishingSite = false;
                isPublishingSite = IsPublishingWeb(web);

                try
                {
                    if (isPublishingSite)
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started updating the publishing page operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        
                        if (pageLayoutsInfo.Count > 0)
                        {
                            string listTitle = MigrationConstants.PagesLibraryTitle;
                            ListCollection lists = web.Lists;
                            List pageLibrary = null;
                            IEnumerable<List> existingLists = web.Context.LoadQuery(lists.Where(list => list.Title == listTitle));
                            web.Context.ExecuteQuery();
                            pageLibrary = existingLists.FirstOrDefault();

                            // Checking whether the library is null or not.
                            if (pageLibrary != null)
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} library exists.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                                PublishPageLayout(rootWeb, web, pageLibrary);
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not updating the Page layout for the site {0} because list {1} doesn't exists.", web.Url, listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Page layout xml file is either invalid or empty."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        }
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not updating the publishing page for site {0} because publishing feature is not activated.", web.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating the publishing page for site {0}.", web.Url);
                    MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                }
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not updating the Page layout."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Adding WebParts to page layout
        /// </summary>
        /// <param name="web">Web instance</param>
        /// <param name="file">List Item File</param>
        /// <param name="pageLayoutInfo">Page layout details</param>
        /// <param name="fileName">Page name</param>
        private static void AddWebpartsToPageLayout(Web web, Microsoft.SharePoint.Client.File file, PageLayouts.PageLayout pageLayoutInfo, string fileName)
        {
            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started adding webparts to page..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            foreach (var deploymentFile in pageLayoutInfo.Files)
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
                }

                for (int count = 0; count < webpartsDetails.Count; count++)
                {
                    if (webpartsDetails[count].ToLower().Equals(listTitle.ToLower()))
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} WebPart exists in the existing page.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                        try
                        {
                            bool isWebpartExists = CheckIfWebpartexists(web, file, listTitle);

                            if (!isWebpartExists)
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} WebPart is not exists.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                                // Get List from web
                                if (web.ListExists(listTitle))
                                {
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List {0} exists.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                                    var list = web.GetListByTitle(listTitle);

                                    webPartEntity.WebPartXml = string.Format(pageLayoutInfo.ListViewWebPart, list.Title, list.Id);
                                    string landingPageUrl = string.Empty;
                                    if (!string.IsNullOrEmpty(pageLayoutInfo.Url))
                                    {
                                        landingPageUrl = string.Format("{0}/{1}/{2}", web.ServerRelativeUrl, pageLayoutInfo.Url, fileName);
                                    }
                                    else
                                    {
                                        landingPageUrl = string.Format("{0}/{1}", web.ServerRelativeUrl, fileName);
                                    }

                                    web.AddWebPartToWebPartPage(landingPageUrl, webPartEntity);
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Successfully added List WebPart {0} in page {1} at web site: {2}", listTitle, fileName, web.Url), LogEventID.InformationWrite);
                                }
                                else
                                {
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List {0} not found at web site:  {1}", listTitle, web.Url), LogEventID.InformationWrite);
                                }
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not adding the {0} WebPart because it is already existed.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                        catch (Exception ex)
                        {
                            string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while checking whether the list {0} exists and adding webpart to page for site {1}.", listTitle, web.Url);
                            MigrationCommonHelper.ExceptionLogging(ex, errorData);
                            SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Publishing the page layout
        /// </summary>        
        /// <param name="rootWeb">Root site</param>
        /// <param name="web">Web instance</param>
        /// <param name="pageLibrary">Page library list</param>
        private static void PublishPageLayout(Web rootWeb, Web web, List pageLibrary)
        {
            CamlQuery query = new CamlQuery { ViewXml = MigrationConstants.GetPagesLibraryItemsQuery };
            ListItemCollection publishingPages = pageLibrary.GetItems(query);
            web.Context.Load(publishingPages, items => items.IncludeWithDefaultProperties(item => item.File));
            web.Context.ExecuteQuery();

            // getting list item to update additional properties
            foreach (ListItem publishingPage in publishingPages)
            {
                try
                {
                    FieldUrlValue oldPageLayout = (FieldUrlValue)publishingPage["PublishingPageLayout"];

                    // Check whether page layout property is null or not.
                    if (oldPageLayout != null)
                    {
                        string publishingPageLayout = oldPageLayout.Url;

                        for (int count = 0; count < pageLayoutsInfo.Count; count++)
                        {
                            string pageLayoutInfoUrl = pageLayoutsInfo[count].OldPageLayout;

                            if (publishingPageLayout.ToLower().EndsWith(pageLayoutInfoUrl.ToLower()))
                            {
                                // Checking whether the file is checked-out or not.
                                if (publishingPage.File.CheckOutType == CheckOutType.None)
                                {
                                    publishingPage.File.CheckOut();
                                }
                                else
                                {
                                    publishingPage.File.CheckIn("Checked-in by migration job.", CheckinType.MajorCheckIn);
                                    publishingPage.File.Publish("Published by migration job.");
                                    pageLibrary.Context.Load(publishingPage);
                                    pageLibrary.Context.ExecuteQuery();
                                    publishingPage.File.CheckOut();
                                }

                                SetContentType(pageLibrary, publishingPage, web.Url, publishingPage.File.Name);

                                DeleteWebpart(web, publishingPage.File);

                                string fileName = publishingPage.File.Name.ToLower();

                                // Checking whether the page is home page or not
                                if (pageLayoutsInfo[count].HomePage.ToLower().Equals(fileName))
                                {
                                    AddWebpartsToPageLayout(web, publishingPage.File, pageLayoutsInfo[count], fileName);
                                }
                                else
                                {
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not adding webparts to {0} page because it is not home page.", fileName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                                }

                                string newPageLayoutUrl = pageLayoutsInfo[count].NewPageLayout;
                                string[] newPageLayoutUrlDescription = newPageLayoutUrl.Split(',');
                                string newPageLayoutFullUrl = rootWeb.Url + newPageLayoutUrlDescription[0];
                                newPageLayoutFullUrl = newPageLayoutFullUrl.Replace("//_catalogs", "/_catalogs");

                                publishingPage["PublishingPageLayout"] = new FieldUrlValue() { Url = newPageLayoutFullUrl, Description = newPageLayoutUrlDescription[1] };
                                publishingPage.Update(); 
                                publishingPage.File.CheckIn("Checked-in by migration job.", CheckinType.MajorCheckIn);
                                publishingPage.File.Publish("Published by migration job.");
                                pageLibrary.Context.Load(publishingPage);
                                pageLibrary.Context.ExecuteQuery();

                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Updated the Page layout from {0} to {1} for file {2}.", publishingPageLayout, pageLayoutsInfo[count].NewPageLayout, publishingPage.File.Name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Page layout for file {0} is null.", publishingPage.File.Name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating the publishing page {0} for site {1}.", web.Url, publishingPage.File.Name);
                    MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                }
            }

            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Updating Page layout operation is completed."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
        }

        /// <summary>
        /// Sets the type of the content.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="publishingPageItem">The publishing page item.</param>
        /// <param name="webUrl">The web URL.</param>
        /// <param name="fileName">Name of the file.</param>
        private static void SetContentType(List list, ListItem publishingPageItem, string webUrl, string fileName)
        {
            try
            {
                var ctx = publishingPageItem.Context;
                var result = ctx.LoadQuery(list.ContentTypes.Where(ct => ct.Name == MigrationConstants.PublishingWelcomePageContentType));
                ctx.ExecuteQuery();
                ContentType listContentType = result.FirstOrDefault();
                if (listContentType == null)
                {
                    list.ContentTypes.AddExistingContentType(listContentType);
                    list.Update();
                    ctx.ExecuteQuery();
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Updated the content type ({0}) for {1} page.", MigrationConstants.PublishingWelcomePageContentType, fileName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }

                publishingPageItem["ContentTypeId"] = listContentType.Id;
                publishingPageItem.Update();
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating the content type for publishing page {0} for site {1}.", fileName, webUrl);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }
        }

        /// <summary>
        /// Deletes the web part.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="file">The file.</param>
        private static void DeleteWebpart(Web web, Microsoft.SharePoint.Client.File file)
        {
            try
            {
                LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                web.Context.Load(wpm.WebParts, wps => wps.Include(wp => wp.WebPart.Title));
                web.Context.ExecuteQuery();                

                foreach (WebPartDefinition wpd in wpm.WebParts)
                {
                    try
                    {
                        WebPart wp = wpd.WebPart;
                        if (wp.Title.ToLower() == MigrationConstants.WebPartPageTitleBarWebpartTitle.ToLower())
                        {
                            wpd.DeleteWebPart();
                            web.Context.ExecuteQuery();
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Deleting the {0} webpart in {1} page.", MigrationConstants.WebPartPageTitleBarWebpartTitle, file.Name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating deleting the webpart in page {0} for site {1}.", file.Name, web.Url);
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    }
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating deleting the webpart in page {0} for site {1}.", file.Name, web.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }
        }

        /// <summary>
        /// Checks if web part exists.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="file">The file.</param>
        /// <param name="webPartTitle">The web part title.</param>
        /// <returns>Returns the value to determine whether web part exists or not.</returns>
        private static bool CheckIfWebpartexists(Web web, Microsoft.SharePoint.Client.File file, string webPartTitle)
        {
            bool isWebPartExists = false;
            try
            {
                LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                web.Context.Load(wpm.WebParts, wps => wps.Include(wp => wp.WebPart.Title));
                web.Context.ExecuteQuery();
                foreach (WebPartDefinition wpd in wpm.WebParts)
                {
                    try
                    {
                        WebPart wp = wpd.WebPart;
                        if (wp.Title.ToLower() == webPartTitle.ToLower())
                        {
                            isWebPartExists = true;
                            return isWebPartExists;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating deleting the webpart in page {0} for site {1}.", file.Name, web.Url);
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    }
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating deleting the webpart in page {0} for site {1}.", file.Name, web.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }

            return isWebPartExists;
        }

        /// <summary>
        /// Checking whether site template is publishing or not
        /// </summary>
        /// <param name="web">Web instance</param>
        /// <returns>Returns the value specifying whether site template is publishing or not</returns>
        private static bool IsPublishingWeb(Web web)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Checking whether the site template is publishing or not..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                var ctx = web.Context;
                var propName = "__PublishingFeatureActivated";
                
                // Ensure web properties are loaded
                if (!web.IsObjectPropertyInstantiated("AllProperties"))
                {
                    ctx.Load(web, w => w.AllProperties);
                    ctx.ExecuteQuery();
                }
                
                // Verify whether publishing feature is activated 
                if (web.AllProperties.FieldValues.ContainsKey(propName))
                {
                    bool propVal;
                    bool.TryParse((string)web.AllProperties[propName], out propVal);
                    return propVal;
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while checking whether the site is publishing or not, site url: {0}.", web.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
                SiteMigrationErrorData = SiteMigrationErrorData + errorData + " Exception Details: " + ex.Message + " ";
            }

            return false;
        }        
    }
}
