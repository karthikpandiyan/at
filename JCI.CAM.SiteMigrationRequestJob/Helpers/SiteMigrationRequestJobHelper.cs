// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteMigrationRequestJobHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Timer Job implementation to store the site migration request Job to the Azure Queue for processing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationRequestJob.Helpers
{   
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using JCI.CAM.Common.AppModelExtensions;
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

    /// <summary>
    /// Timer Job implementation to store the site migration requests to the Azure Queue.
    /// </summary>
    public class SiteMigrationRequestJobHelper
    {
        /// <summary>
        /// Approved site migration request
        /// </summary>
        private EventHandler<SiteMigrationRequestEventArgs> approvedSiteMigrationRequest;

        #region Properties
        /// <summary>
        /// Gets or sets the approved site migration request.
        /// </summary>
        /// <value>
        /// The approved site migration request.
        /// </value>
        public EventHandler<SiteMigrationRequestEventArgs> ApprovedSiteMigrationRequest
        {
            get
            {
                return this.approvedSiteMigrationRequest;
            }

            set
            {
                this.approvedSiteMigrationRequest = value;
            }
        }

        /// <summary>
        /// Gets the implementation for AppOnlyAuthentication
        /// </summary>
        private static IAuthentication Authentication
        {
            get
            {
                return new AppOnlyAuthenticationSite();
            }
        }
        #endregion  

        /// <summary>
        /// Retrieve the description on the enumeration
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>Returns status</returns>
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        /// <summary>
        /// Updates the status of a site request in the site repository
        /// </summary>
        /// <param name="listItemId">The list item identifier.</param>
        /// <param name="status">Request Status</param>
        /// <param name="statusMessage">Status Message</param>
        /// <param name="siteMigrationRequestListName">Site Request List</param>
        public void UpdateRequestStatus(int listItemId, SiteMigrationRequestStatus status, string statusMessage, string siteMigrationRequestListName)
        {
            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Updating site migration request item {0}...", listItemId), LogEventID.InformationWrite);

            AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

            using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContext())
            {
                LogHelper.LogInformation("Tenant context acquired", LogEventID.InformationWrite);

                // Get Content type hub url specified in SPHostUrl - app config entry
                Tenant tenantSite = new Tenant(tenantContext);
                var site = tenantSite.GetSiteByUrl(GlobalData.MigrationRequestSiteUrl);
                using (var context = site.Context.Clone(GlobalData.MigrationRequestSiteUrl))
                {
                    var web = context.Web;
                    context.Load(web, w => w.Url, w => w.Title);
                    List siteMigrationRequestList = web.Lists.GetByTitle(GlobalData.MigrationRequestListTitle);
                    context.Load(siteMigrationRequestList);

                    ListItem item = siteMigrationRequestList.GetItemById(listItemId);
                    context.Load(item);
                    web.Context.ExecuteQuery();

                    if (item != null)
                    {
                        string requestStatus = GetDescription(status);

                        item[MigrationConstants.SiteMigrationStatusColumn] = requestStatus;
                        item[MigrationConstants.SiteMigrationErrorDataColumn] = item[MigrationConstants.SiteMigrationErrorDataColumn] + statusMessage;
                        item.Update();
                        context.ExecuteQuery();
                        LogHelper.LogInformation(string.Format("Updated site migration request - item ID: {0} with status and message {1}", listItemId, statusMessage), LogEventID.InformationWrite);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the email notification.
        /// </summary>
        /// <param name="siteMigrationRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="emailSubject">Email Subject</param>
        /// <param name="emailBody">Email body</param>
        public void SendEmailNotification(SiteMigrationRequest siteMigrationRequest, string status, string emailSubject, string emailBody)
        {
            if (GlobalData.SendEmailNotificationForSiteMigrationRequest)
            {
                LogHelper.LogInformation("Sending email notification regarding the status of site migration request job...", LogEventID.InformationWrite);

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
                                            string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.SendEmailNotification() - Error occurred while assigning the email collection. Email ID: {0}.", siteOwners[count].Email);
                                            this.ExceptionLogging(ex, errorData);
                                        }
                                    }
                                    else
                                    {
                                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.SendEmailNotification() - Email ID is null or empty for user {0}. Email ID: {1}.", siteOwners[count].LoginName, siteOwners[count].Email), LogEventID.InformationWrite);
                                    }
                                }

                                if (emailIDs.Count > 0)
                                {
                                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Total receipients : {0}", emailIDs.Count), LogEventID.InformationWrite);

                                    // Email properties
                                    LogHelper.LogInformation("Email message processing - site migration request job...", LogEventID.InformationWrite);
                                    EmailProperties properties = new EmailProperties
                                    {
                                        To = emailIDs,
                                        Subject = subject,
                                        Body = string.Format(CultureInfo.InvariantCulture, body, siteMigrationRequest.SiteTitle, siteMigrationRequest.SiteURL)
                                    };
                                    Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(ctx, properties);
                                    ctx.ExecuteQuery();
                                    LogHelper.LogInformation("Email message sent regarding site migration request job.", LogEventID.InformationWrite);
                                }
                                else
                                {
                                    LogHelper.LogInformation("Not sending email because there are no receipients.", LogEventID.InformationWrite);
                                }
                            }
                            else
                            {
                                LogHelper.LogInformation("Site migration request job - subject and body are empty.", LogEventID.InformationWrite);
                            }

                            LogHelper.LogInformation("Sending email notification Completed.", LogEventID.InformationWrite);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.SendEmailNotification() - Error occurred while sending email.");
                        this.ExceptionLogging(ex, errorData);
                    }
                }
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not sending email notification regarding site migrtaion request for site {0}.", siteMigrationRequest.SiteURL), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }        

        /// <summary>
        /// Used to Process Approved site migration requests in the Site Repository Data store.
        /// </summary>
        public void ProcessSiteMigrationRequestJob()
        {
            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Checking for site migration requests in list {0}, site {1}...", GlobalData.MigrationRequestListTitle, GlobalData.MigrationRequestSiteUrl), LogEventID.InformationWrite);

            List<SiteMigrationRequest> siteMigrationRequests = this.GetSiteMigrationRequests();
            SiteMigrationRequestHandler handler = new SiteMigrationRequestHandler(this);

            foreach (var siteMigrationRequest in siteMigrationRequests)
            {
                this.OnNewApprovedRequest(new SiteMigrationRequestEventArgs(siteMigrationRequest));
            }

            LogHelper.LogInformation(string.Format("{0} site migration requests are queued.", siteMigrationRequests.Count), LogEventID.InformationWrite);
        }

        /// <summary>
        /// Event Handler
        /// </summary>
        /// <param name="e">The <see cref="SiteRequestEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNewApprovedRequest(SiteMigrationRequestEventArgs e)
        {
            EventHandler<SiteMigrationRequestEventArgs> handler = this.ApprovedSiteMigrationRequest;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Exception logging
        /// </summary>
        /// <param name="ex">Exception Object</param>
        /// <param name="errorData">Error message</param>
        private void ExceptionLogging(Exception ex, string errorData)
        {
            LogHelper.LogInformation(errorData, JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            LogHelper.LogError(ex, LogEventID.ExceptionHandling);
        }

        /// <summary>
        /// Getting the site migration requests
        /// </summary>
        /// <returns>Returns site migration requests</returns>
        private List<SiteMigrationRequest> GetSiteMigrationRequests()
        {
            List<SiteMigrationRequest> siteMigrationRequests = new List<SiteMigrationRequest>();

            try
            {
                LogHelper.LogInformation("Getting tenant context...", LogEventID.InformationWrite);

                AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContext())
                {
                    LogHelper.LogInformation("Tenant context acquired", LogEventID.InformationWrite);

                    // Get Content type hub url specified in SPHostUrl - app config entry
                    Tenant tenantSite = new Tenant(tenantContext);
                    var site = tenantSite.GetSiteByUrl(GlobalData.MigrationRequestSiteUrl);

                    using (var ctx = site.Context.Clone(GlobalData.MigrationRequestSiteUrl))
                    {
                        var web = ctx.Web;

                        ctx.Load(web, w => w.Title, w => w.Url);
                        List siteMigrationRequestList = web.Lists.GetByTitle(GlobalData.MigrationRequestListTitle);
                        ctx.Load(siteMigrationRequestList);
                        ctx.ExecuteQuery();
                        LogHelper.LogInformation("Config Site acquired", LogEventID.InformationWrite);

                        if (siteMigrationRequestList != null)
                        {
                            siteMigrationRequests = this.TraverseSiteMigrationList(ctx, web, siteMigrationRequestList);
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} list is null.", GlobalData.MigrationRequestListTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ExceptionLogging(ex, "Error occured while accessing the site migration request list");
            }

            return siteMigrationRequests;
        }

        /// <summary>
        /// Traversing through SiteMigrationRequest list
        /// </summary>
        /// <param name="context">ClientContext instance</param>
        /// <param name="web">Web instance</param>
        /// <param name="siteMigrationRequestList">Site Migration Request list title</param>
        /// <returns>Returns site migration requests</returns>
        private List<SiteMigrationRequest> TraverseSiteMigrationList(ClientContext context, Web web, List siteMigrationRequestList)
        {
            LogHelper.LogInformation("Loading site migration request list items...", LogEventID.InformationWrite);

            ListItemCollectionPosition itemPosition = null;
            List<SiteMigrationRequest> siteMigrationRequestQueueItems = new List<SiteMigrationRequest>();
            
            DateTime scheduleStartDate = DateTime.Now;
            string scheduleStartDateString = scheduleStartDate.ToString("s");
            string siteMigrationRequestsJobCamlQuery = string.Format(CultureInfo.InvariantCulture, MigrationConstants.SiteMigrationRequestsJobCamlQuery, scheduleStartDateString);

            while (true)
            {
                CamlQuery queryToGetSiteMigrationRequest = new CamlQuery
                {
                    ListItemCollectionPosition = itemPosition,
                    ViewXml = siteMigrationRequestsJobCamlQuery
                };                

                ListItemCollection siteMigrationRequests = siteMigrationRequestList.GetItems(queryToGetSiteMigrationRequest);
                web.Context.Load(siteMigrationRequests);
                web.Context.ExecuteQuery();

                // Item postion
                itemPosition = siteMigrationRequests.ListItemCollectionPosition;

                if (siteMigrationRequests.Count > 0)
                {
                    foreach (ListItem siteMigrationRequest in siteMigrationRequests)
                    {
                        try
                        {
                            string requestedSiteTitle = siteMigrationRequest[MigrationConstants.SiteTitleColumn].ToString();
                            FieldUrlValue requestedSiteUrlField = (FieldUrlValue)siteMigrationRequest[MigrationConstants.SiteURLColumn];
                            string requestedSiteUrl = requestedSiteUrlField.Url;

                            SiteMigrationRequest siteMigrationRequestEntity = new SiteMigrationRequest
                            {
                                ListItemId = Convert.ToInt32(siteMigrationRequest[MigrationConstants.IDColumn]),
                                SiteTitle = requestedSiteTitle,
                                SiteURL = requestedSiteUrl,
                                SiteOwners = MigrationCommonHelper.GetSiteOwners(context, siteMigrationRequest, siteMigrationRequestList.Title)
                            };
                            siteMigrationRequestQueueItems.Add(siteMigrationRequestEntity);
                        }
                        catch (Exception ex)
                        {
                            this.ExceptionLogging(ex, string.Format(CultureInfo.InvariantCulture, "Error occured while accesing the items for queue at {0} ({1}).", siteMigrationRequest[MigrationConstants.SiteTitleColumn], siteMigrationRequest[MigrationConstants.SiteURLColumn]));
                        }
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "There are no items present in the queue, {0} list.", GlobalData.MigrationRequestListTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }

                // If item position is null then break the while loop.
                if (itemPosition == null)
                {
                    break;
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Paging Info: {0}", itemPosition.PagingInfo), LogEventID.InformationWrite);
                }
            }

            return siteMigrationRequestQueueItems;
        }
    }
}
