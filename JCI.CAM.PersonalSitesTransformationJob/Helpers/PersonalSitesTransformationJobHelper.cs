// <copyright file="PersonalSitesTransformationJobHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Personal Sites Migration job 
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PersonalSitesTransformationJob.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization; 
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;    
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Authentication;
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Migration.Common.Helpers;    
    using JCI.CAM.Provisioning.Core;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;     

    /// <summary>
    /// Site Migration Job Helper Class
    /// </summary>
    public abstract class PersonalSitesTransformationJobHelper
    {      
        /// <summary>
        /// Schema common file path
        /// </summary>
        private static string commonFilePath = AppDomain.CurrentDomain.BaseDirectory;              

        /// <summary>
        /// Gets or sets the personal site transformation job error data
        /// </summary>
        protected static string PersonalSiteTransformationJobErrorData { get; set; }        

        /// <summary>
        /// Updates the personal site migration status in azure table
        /// </summary>
        /// <param name="personalSiteMigrationRequest">SiteMigrationRequest object</param>
        /// <param name="personalSiteMigrationStatus">Site migration job status</param>
        /// <param name="siteMigrationErrorData">Error details</param>        
        /// <param name="personalSiteMigrationTable">Personal Site Migration table</param>
        public static void UpdateSiteMigrationRequestStatus(PersonalSitesMigrationRequest personalSiteMigrationRequest, string personalSiteMigrationStatus, string siteMigrationErrorData, CloudTable personalSiteMigrationTable)
        {
            try
            {
                string notStartedStatus = MigrationCommonHelper.GetEnumFriendlyName(SiteMigrationRequestStatus.NotStarted);

                // Construct the query operation for all customer entities where PartitionKey="Smith".
                TableQuery<PersonalSitesMigrationRequest> query = new TableQuery<PersonalSitesMigrationRequest>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, personalSiteMigrationRequest.RowKey));

                IEnumerable<PersonalSitesMigrationRequest> personalSitesMigrationUpdateRequest = personalSiteMigrationTable.ExecuteQuery(query).ToList();

                // Assign the result to a PersonalSitesMigrationRequest object.
                PersonalSitesMigrationRequest updateEntity = personalSitesMigrationUpdateRequest.FirstOrDefault();

                if (updateEntity != null)
                {
                    // Change the status.
                    updateEntity.SiteMigrationStatus = personalSiteMigrationStatus;

                    // log
                    updateEntity.Log = siteMigrationErrorData;

                    // Create the InsertOrReplace TableOperation
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);

                    // Execute the operation.
                    personalSiteMigrationTable.Execute(updateOperation);

                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Updated the status from {0} to {1} for site {2}.", personalSiteMigrationRequest.SiteMigrationStatus, personalSiteMigrationStatus, personalSiteMigrationRequest.SiteURL), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                    else
                    {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Entity could not be retrieved for site {0}.", personalSiteMigrationRequest.SiteURL), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while updating site migration request.");
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Getting the site migration requests
        /// </summary>
        public void ProcessTransformationJob()
        {
            try
            {
                List<PersonalSitesMigrationRequest> personalSitesMigrationRequests = new List<PersonalSitesMigrationRequest>();
                string siteMigrationStatus = MigrationCommonHelper.GetEnumFriendlyName(SiteMigrationRequestStatus.NotStarted);

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Retrieving the storage account from the connection string {0}", ConfigurationManager.ConnectionStrings["PersonalSitesTransformationTableStorage"].ConnectionString), LogEventID.InformationWrite);

                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["PersonalSitesTransformationTableStorage"].ConnectionString);

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the personal site migration table. And if the specified table not exists then created a table
                CloudTable personalSiteMigrationTable = tableClient.GetTableReference(GlobalData.PersonalSitesTransformationTableName);
                personalSiteMigrationTable.CreateIfNotExists();

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Accessed table {0}.", GlobalData.PersonalSitesTransformationTableName), LogEventID.InformationWrite);

                // Construct the query operation for all customer entities where PartitionKey="Smith".
                TableQuery<PersonalSitesMigrationRequest> query = new TableQuery<PersonalSitesMigrationRequest>().Where(TableQuery.GenerateFilterCondition("SiteMigrationStatus", QueryComparisons.Equal, siteMigrationStatus));

                personalSitesMigrationRequests = personalSiteMigrationTable.ExecuteQuery(query).ToList();

                // Checking whether there are any personal site migration requests
                if (personalSitesMigrationRequests.Count > 0)
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "There are {0} requests present in the {1} table.", personalSitesMigrationRequests.Count, GlobalData.PersonalSitesTransformationTableName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    this.PerformTransformation(personalSitesMigrationRequests, personalSiteMigrationTable);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "There are {0} requests present in the {1} table.", personalSitesMigrationRequests.Count, GlobalData.PersonalSitesTransformationTableName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while accessing the azure table {0}.", GlobalData.PersonalSitesTransformationTableName);
                PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData;
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Performs the transformation.
        /// </summary>
        /// <param name="personalSites">The personal sites.</param>
        /// <param name="personalSiteMigrationTable">Personal Site Migration table</param>
        public void PerformTransformation(List<PersonalSitesMigrationRequest> personalSites, CloudTable personalSiteMigrationTable)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Processing the personal sites tranformation requests..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                BrandingEntity brandingEntity = GetBrandingEntity();
                ThemeEntity themeEntity = brandingEntity.ThemePackage;
                CustomActionEntity customActionEntity = brandingEntity.CustomAction;
                List<Features.Feature> featureInfo = GetFeatureDetailsFromXML();
                
                OnPremisePersonalSitesTransformationJobHelper onPremisePersonalSitesTransformationJobHelper = new OnPremisePersonalSitesTransformationJobHelper();
                OnlinePersonalSitesTransformationJobHelper onlinePersonalSitesTransformationJobHelper = new OnlinePersonalSitesTransformationJobHelper();

                LogHelper.LogInformation("Getting tenant context...", LogEventID.InformationWrite);

                AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                using (ClientContext context = tenantAuthentication.GetSpecificTenantAuthenticatedContext(GlobalData.MySiteTenantAdminUrl))
                {
                    context.RequestTimeout = 120000;
                    LogHelper.LogInformation("Tenant context acquired", LogEventID.InformationWrite);

                    foreach (var personalSite in personalSites)
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Performing tranformation for site {0}({1})...", personalSite.SiteTitle, personalSite.SiteURL), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                        try
                        {
                            PersonalSiteTransformationJobErrorData = string.Empty;

                            if (GlobalData.SharePointOnPremKey)
                            {
                                onPremisePersonalSitesTransformationJobHelper.TransformingPersonalSites(context, personalSite.SiteURL, personalSite.SiteTitle, themeEntity, customActionEntity, featureInfo);
                            }
                            else
                            {
                                onlinePersonalSitesTransformationJobHelper.TransformingPersonalSites(context, personalSite.SiteURL, personalSite.SiteTitle, themeEntity, customActionEntity, featureInfo);
                            }

                            try
                            {
                                LogHelper.LogInformation(string.Format("Sending email for successfully transforming the site: {0}", personalSite.SiteURL), LogEventID.InformationWrite);

                                if (string.IsNullOrEmpty(PersonalSiteTransformationJobErrorData))
                                {
                                    LogHelper.LogInformation(string.Format("Updating the personal site migration status to success for site {0}", personalSite.SiteURL), LogEventID.InformationWrite);
                                    UpdateSiteMigrationRequestStatus(personalSite, SiteMigrationRequestStatus.Success.ToString(), PersonalSiteTransformationJobErrorData, personalSiteMigrationTable);

                                    if (GlobalData.SharePointOnPremKey)
                                    {
                                        onPremisePersonalSitesTransformationJobHelper.SendEmailNotification(personalSite, SiteMigrationRequestStatus.Success.ToString(), MigrationConstants.SiteMigrationSuccessEmailSubjectKey, MigrationConstants.SiteMigrationSuccessEmailBodyKey);
                                    }
                                    else
                                    {
                                        onlinePersonalSitesTransformationJobHelper.SendEmailNotification(personalSite, SiteMigrationRequestStatus.Success.ToString(), MigrationConstants.SiteMigrationSuccessEmailSubjectKey, MigrationConstants.SiteMigrationSuccessEmailBodyKey);
                                    }
                                }
                                else
                                {
                                    LogHelper.LogInformation(string.Format("Updating the personal site migration status to failed for site {0}", personalSite.SiteURL), LogEventID.InformationWrite);
                                    string statusMessage = string.Format("{0} - {1}", PersonalSiteTransformationJobErrorData, DateTime.Now.ToString());
                                    UpdateSiteMigrationRequestStatus(personalSite, SiteMigrationRequestStatus.Failed.ToString(), statusMessage, personalSiteMigrationTable);

                                    if (GlobalData.SharePointOnPremKey)
                                    {
                                        onPremisePersonalSitesTransformationJobHelper.SendEmailNotification(personalSite, SiteMigrationRequestStatus.Failed.ToString(), MigrationConstants.SiteMigrationFailedEmailSubjectKey, MigrationConstants.SiteMigrationFailedEmailBodyKey);
                                    }
                                    else
                                    {
                                        onlinePersonalSitesTransformationJobHelper.SendEmailNotification(personalSite, SiteMigrationRequestStatus.Failed.ToString(), MigrationConstants.SiteMigrationFailedEmailSubjectKey, MigrationConstants.SiteMigrationFailedEmailBodyKey);
                                    }
                                }
                            }
                            catch (Exception error)
                            {
                                MigrationCommonHelper.ExceptionLogging(error, "JCI.CAM.PersonalSitesTransformationJob.Helpers.performTranformation() - Error occurred while updating the status and sending mail notification.");
                            }
                        }
                        catch (Exception ex)
                        {
                            string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.performTranformation() - Error occurred while performomg the transformation for site {0}({1}).", personalSite.SiteTitle, personalSite.SiteURL);
                            PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                            MigrationCommonHelper.ExceptionLogging(ex, errorData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.performTranformation() - Error occurred while deserializing the xml.");
                PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Sends the email notification.
        /// </summary>
        /// <param name="siteMigrationRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="emailSubject">Email Subject</param>
        /// <param name="emailBody">Email body</param>
        public virtual void SendEmailNotification(PersonalSitesMigrationRequest siteMigrationRequest, string status, string emailSubject, string emailBody)
        {
            if (GlobalData.SendEmailNotification)
            {
                LogHelper.LogInformation("Sending email notification regarding the status of personal site migration job...", LogEventID.InformationWrite);

                string subject = string.Empty;
                string body = string.Empty;

                AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContext())
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

                                string emailID = siteMigrationRequest.SiteOwner;

                                List<string> emailIDs = new List<string>();

                                if (!string.IsNullOrEmpty(emailID))
                                {
                                    try
                                    {
                                        emailIDs.Add(emailID);
                                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Added {0} receipient to send the mail.", emailID), LogEventID.InformationWrite);
                                    }
                                    catch (Exception ex)
                                    {
                                        string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.SendEmailNotification() - Error occurred while assigning the email collection. Email ID: {0}.", emailID);
                                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                                    }
                                }
                                else
                                {
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationJob.Helpers.SendEmailNotification() - Email ID is null or empty. Email ID: {0}.", emailID), LogEventID.InformationWrite);
                                }

                                if (emailIDs.Count > 0)
                                {
                                    // Email properties
                                    LogHelper.LogInformation("Email message processing - personal sites migration job...", LogEventID.InformationWrite);
                                    EmailProperties properties = new EmailProperties
                                    {
                                        To = emailIDs,
                                        Subject = subject,
                                        Body = string.Format(CultureInfo.InvariantCulture, body, siteMigrationRequest.SiteTitle, siteMigrationRequest.SiteURL)
                                    };
                                    Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(ctx, properties);
                                    ctx.ExecuteQuery();
                                    LogHelper.LogInformation("Email message sent regarding personal sites migration job.", LogEventID.InformationWrite);
                                }
                                else
                                {
                                    LogHelper.LogInformation("Not sending email because there are no receipients.", LogEventID.InformationWrite);
                                }
                            }
                            else
                            {
                                LogHelper.LogInformation("Personal sites migration job - subject and body are empty.", LogEventID.InformationWrite);
                            }

                            LogHelper.LogInformation("Sending email notification Completed.", LogEventID.InformationWrite);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.SendEmailNotification() - Error occurred while sending email.");
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    }
                }
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not sending email notification regarding personal site migrtaion status for site {0}.", siteMigrationRequest.SiteURL), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }            
        }

        /// <summary>
        /// Transforms the personal sites.
        /// </summary>
        /// <param name="tenantContext">The tenant context.</param>
        /// <param name="personalSiteUrl">The personal site URL.</param>
        /// <param name="personalSiteTitle">The personal site title.</param>
        /// <param name="themeEntity">The theme entity.</param>
        /// <param name="customActionEntity">The custom action entity.</param>
        /// <param name="featureInfo">The feature information.</param>
        public virtual void TransformingPersonalSites(ClientContext tenantContext, string personalSiteUrl, string personalSiteTitle, ThemeEntity themeEntity, CustomActionEntity customActionEntity, List<Features.Feature> featureInfo)
        {
            try
            {
                // OneDrive for Business site custom branding.
                Tenant tenantSite = new Tenant(tenantContext);
                var site = tenantSite.GetSiteByUrl(personalSiteUrl);
                using (var context = site.Context.Clone(personalSiteUrl))
                {
                    Site personalSite = context.Site;
                    Web rootWeb = personalSite.RootWeb;
                    context.Load(rootWeb, w => w.Title, w => w.Url, w => w.ServerRelativeUrl, w => w.CustomMasterUrl, w => w.MasterUrl);

                    // Getting relative url
                    Uri siteURL = new Uri(personalSiteUrl);
                    string relativeSiteURL = Uri.UnescapeDataString(siteURL.AbsolutePath);

                    Web web = context.Web;                    
                    context.Load(web, w => w.Title, w => w.Url, w => w.ServerRelativeUrl, w => w.CustomMasterUrl, w => w.MasterUrl);
                    context.ExecuteQuery();

                    DeactivateFeatures(personalSite, web, featureInfo);

                    ResetMasterPage(web, rootWeb);

                    web.AddCustomAction(customActionEntity);

                    // Let's first upload the contoso theme to host web, if it does not exist there
                    if (web.Url.Equals(rootWeb.Url))
                    {
                        LogHelper.LogInformation(string.Format("Applying custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                        DeployThemeToWeb(web, themeEntity);
                        LogHelper.LogInformation(string.Format("Applied custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format("Not uploading the theme for site {0} because it is not a root site.", web.Url), LogEventID.InformationWrite);
                        LogHelper.LogInformation(string.Format("Applying custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                        web.SetThemeBasedOnName(rootWeb, themeEntity.Name);
                        LogHelper.LogInformation(string.Format("Applied custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                    }

                    try
                    {
                        ApplyCss(web, themeEntity, "Style Library");
                        LogHelper.LogInformation(string.Format("Applied css for site {0}", web.Url), LogEventID.InformationWrite);
                        ApplySiteLogo(web, themeEntity, "Style Library");
                        LogHelper.LogInformation(string.Format("Applied site logo for site {0}", web.Url), LogEventID.InformationWrite);
                    }
                    catch (Exception ex)
                    {
                        // Ignoring exception
                        LogHelper.LogInformation("Due to API limitations, AlternateCSS and SiteLogo options may not work in dedicated environment. This options can be used in vNext.", LogEventID.InformationWrite);
                        LogHelper.LogError(ex);
                    }

                    web.SetPropertyBagValue(MigrationConstants.JCICAMTransformationPropertyBagKey, MigrationConstants.JCICAMTransformationPropertyBagValue);
                    
                    LogHelper.LogInformation("Checking for subsites...", LogEventID.InformationWrite);

                    WebCollection webs = web.Webs;
                    context.Load(webs);
                    context.ExecuteQuery();

                    if (webs.Count > 0)
                    {
                        foreach (var subWeb in webs)
                        {
                            // Checking whether the site template is app or not.
                            if (!subWeb.WebTemplate.ToLower().Equals(GlobalData.AppSiteTemplate.ToLower()))
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Transforming the subsite {0}...", subWeb.Url), LogEventID.InformationWrite);
                                this.TransformingPersonalSites(context, subWeb.Url, subWeb.Title, themeEntity, customActionEntity, featureInfo);
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not transforming the site because this is an app site. Subsite url: {0}, Template: {1}.", subWeb.Url, subWeb.WebTemplate), LogEventID.InformationWrite);
                            }
                        }
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "There are no subsites."), LogEventID.InformationWrite);
                    }

                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Tranformation is completed for site {0}({1}).", personalSiteTitle, personalSiteUrl), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.TransformingOneDriveSites() - Error occurred while performomg the transformation for one drive site {0}({1}).", personalSiteTitle, personalSiteUrl);
                PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Gets the feature details from XML.
        /// </summary>
        /// <returns>Returns feature details</returns>
        protected static List<Features.Feature> GetFeatureDetailsFromXML()
        {
            LogHelper.LogInformation("Serializing feature xml to object.", LogEventID.InformationWrite);

            string xmlPath = Path.Combine(commonFilePath, @"Resources\SchemaFiles\JCIFeatureDetails.xml");
            List<Features.Feature> featureInfo = new List<Features.Feature>();

            if (!string.IsNullOrEmpty(xmlPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<Features.Feature>), new XmlRootAttribute(MigrationConstants.FeatureXMLRootElement));
                using (TextReader textReader = new System.IO.StreamReader(xmlPath))
                {
                    featureInfo = (List<Features.Feature>)deserializer.Deserialize(textReader);
                }

                LogHelper.LogInformation("Serialized feature xml to object.", LogEventID.InformationWrite);
            }

            return featureInfo;
        }

        /// <summary>
        /// Gets the theme entity.
        /// </summary>
        /// <returns>Return theme</returns>
        protected static BrandingEntity GetBrandingEntity()
        {
            LogHelper.LogInformation("Loading theme package xml file to serialize", LogEventID.InformationWrite);

            BrandingEntity brandingEntity = new BrandingEntity();
            string path = Path.Combine(commonFilePath, @"Resources\SchemaFiles\ThemePackage.xml");
            XmlSerializer deserializer = new XmlSerializer(typeof(BrandingEntity));
            TextReader textReader = new System.IO.StreamReader(path);
            brandingEntity = (BrandingEntity)deserializer.Deserialize(textReader);
            textReader.Close();
            LogHelper.LogInformation("Theme package xml file is serialized", LogEventID.InformationWrite);
            return brandingEntity;
        }        

        /// <summary>
        /// Resets the master page.
        /// </summary>
        /// <param name="currentWeb">Current web.</param>
        /// <param name="rootWeb">Root web.</param>
        protected static void ResetMasterPage(Web currentWeb, Web rootWeb)
        {
            string masterPageUrl = string.Empty;

            try
            {
                masterPageUrl = string.Concat(rootWeb.ServerRelativeUrl, MigrationConstants.MasterPageUrl);

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
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while changing the master page from {0} to {1} for site {2}.", masterPageUrl, rootWeb.MasterUrl, rootWeb.Url);
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Deploys the theme.
        /// </summary>
        /// <param name="web">The root web.</param>
        /// <param name="theme">The theme.</param>
        protected static void DeployThemeToWeb(Web web, ThemeEntity theme)
        {
            try
            {
                if (!string.IsNullOrEmpty(theme.ColorFile))
                {
                    web.UploadThemeFile(string.Format(CultureInfo.InvariantCulture, "{0}{1}", commonFilePath, theme.ColorFile));
                    LogHelper.LogInformation(string.Format("Completed Uploading Color File {0}: for site {1}", theme.ColorFile, web.Url), LogEventID.InformationWrite);
                }

                if (!string.IsNullOrEmpty(theme.FontFile))
                {
                    web.UploadThemeFile(string.Format(CultureInfo.InvariantCulture, "{0}{1}", commonFilePath, theme.FontFile));
                    LogHelper.LogInformation(string.Format("Completed Uploading Font File {0}: for site {1}", theme.FontFile, web.Url), LogEventID.InformationWrite);
                }

                if (!string.IsNullOrEmpty(theme.BackgroundFile))
                {
                    web.UploadThemeFile(string.Format(CultureInfo.InvariantCulture, "{0}{1}", commonFilePath, theme.BackgroundFile));
                    LogHelper.LogInformation(string.Format("Completed Uploading BackgroundFile {0}: for site {1}", theme.BackgroundFile, web.Url), LogEventID.InformationWrite);
                }

                web.CreateComposedLookForOneDriveByName(theme.Name, theme.ColorFile.Substring(theme.ColorFile.LastIndexOf('/') + 1), theme.FontFile.Substring(theme.FontFile.LastIndexOf('/') + 1), theme.BackgroundFile.Substring(theme.BackgroundFile.LastIndexOf('/') + 1), string.Empty);
                LogHelper.LogInformation(string.Format("Created Composed look {0}: for site {1}", theme.Name, web.Url), LogEventID.InformationWrite);
                web.SetComposedLookByUrl(theme.Name);
                LogHelper.LogInformation(string.Format("Seting theme {0}: for site {1}", theme.Name, web.Url), LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.DeployThemeToWeb() - Error occurred while deploying theme to site {0}.", web.Url);
                PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Applies the CSS.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="theme">The theme.</param>
        /// <param name="listTitle">The list title.</param>
        protected static void ApplyCss(Web web, ThemeEntity theme, string listTitle)
        {
            if (!string.IsNullOrEmpty(theme.AlternateCSS))
            {
                try
                {
                    string cssFilePath = string.Format(CultureInfo.InvariantCulture, "{0}{1}", commonFilePath, theme.AlternateCSS);
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
                    string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.ApplyCss() - Error occurred while applying css to site {0}.", web.Url);
                    PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    MigrationCommonHelper.ExceptionLogging(ex, errorData);
                }
            }
        }

        /// <summary>
        /// Applies the site logo.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="theme">The theme.</param>
        /// <param name="listTitle">The list title.</param>
        protected static void ApplySiteLogo(Web web, ThemeEntity theme, string listTitle)
        {
            if (!string.IsNullOrEmpty(theme.SiteLogo))
            {
                try
                {
                    string siteLogoFilePath = string.Format(CultureInfo.InvariantCulture, "{0}{1}", commonFilePath, theme.SiteLogo);
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
                    string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.ApplySiteLogo() - Error occurred while applying the logo to site {0}.", web.Url);
                    PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                    MigrationCommonHelper.ExceptionLogging(ex, errorData);
                }
            }
        }

        /// <summary>
        /// Deactivates the features.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="web">The web.</param>
        /// <param name="featureInfo">The feature information.</param>
        protected static void DeactivateFeatures(Site site, Web web, List<Features.Feature> featureInfo)
        {
            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Started deactivating features..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            for (int count = 0; count < featureInfo.Count; count++)
            {
                try
                {
                    // Check whether the scope of the feature is site or web.
                    if (featureInfo[count].Scope.ToLower().Equals(MigrationConstants.SiteScopeFeature))
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
                    PersonalSiteTransformationJobErrorData = PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                }
            }

            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Completed deactivating features."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
        }
    }
}
