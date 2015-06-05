// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteRequestManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.Provisioning.Core.Configuration;
    using JCI.CAM.Provisioning.Core.Constants;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using Microsoft.SharePoint.Client.WorkflowServices;

    /// <summary>
    /// Implementation class for the Site Request Repository that leverages SharePoint as the data source.
    /// </summary>
    internal class SiteRequestManager : ISiteRequestManager, ISharePointService
    {
        #region Private Instance Members
        /// <summary>
        /// The approved site requests CAML query
        /// </summary>
        public const string ApprovedSiteRequestsCamlQueryNotValid = "<View><Query><Where><Eq><FieldRef Name='RequestStatus'/><Value Type='Text'>New</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";

        /// <summary>
        /// The get site requests CAML query if the Site Request is approved
        /// </summary>
        public const string ApprovedSiteRequestsCamlQuery = "<View><Query><Where><And><Eq><FieldRef Name='WorkflowStatus' /><Value Type='Choice'>Approved</Value></Eq><Eq><FieldRef Name='RequestStatus' /><Value Type='Choice'>New</Value></Eq></And></Where></Query><RowLimit>100</RowLimit></View>";

        /// <summary>
        /// The site request item by identifier query
        /// </summary>
        public const string SiteRequestItemByIdQuery = "<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Integer'>{0}</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteRequestManager" /> class.
        /// </summary>
        public SiteRequestManager()
        {
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets the implementation for AppOnlyAuthentication
        /// </summary>
        public IAuthentication Authentication
        {
            get
            {
                return new AppOnlyAuthenticationSite();
            }
        }
        #endregion

        #region ISharePointService Members
        /// <summary>
        /// Class used for working with the ClientContext
        /// </summary>
        /// <param name="action">The action.</param>
        public virtual void UsingContext(Action<ClientContext> action)
        {
            this.UsingContext(action, Timeout.Infinite);
        }

        /// <summary>
        /// Class used for working with the ClientContext
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="csomTimeout">The CSOM timeout.</param>
        public virtual void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext ctx = this.Authentication.GetAuthenticatedContext())
            {
                ctx.RequestTimeout = csomTimeout;
                action(ctx);
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
            using (ClientContext ctx = this.Authentication.GetAuthenticatedContextForGivenUrl(siteUrl))
            {
                ctx.RequestTimeout = csomTimeout;
                action(ctx);
            }
        }
        #endregion

        #region ISiteRequestManager Members
        /// <summary>
        /// Returns a collection of all Approved Site Requests
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// Will return a collection of new SiteRequests or an empty collection will be returned
        /// </returns>
        public ICollection<SiteRequestInformation> GetApprovedRequests(AppSettings settings)
        {
            return this.GetSiteRequestsByCaml(ApprovedSiteRequestsCamlQuery, settings);
        }

        /// <summary>
        /// Updates the status of a site request in the site repository
        /// </summary>
        /// <param name="listItemId">The list item identifier.</param>
        /// <param name="status">Request Status</param>
        public void UpdateRequestStatus(int listItemId, SiteRequestStatus status)
        {
            this.UpdateRequestStatus(listItemId, status, string.Empty, string.Empty);
        }

        /// <summary>
        /// Updates the status of a site request in the site repository
        /// </summary>
        /// <param name="listItemId">The list item identifier.</param>
        /// <param name="status">Request Status</param>
        /// <param name="statusMessage">Status Message</param>
        /// <param name="siteRequestListName">Site Request List</param>
        public void UpdateRequestStatus(int listItemId, SiteRequestStatus status, string statusMessage, string siteRequestListName)
        {
            // bool isWorkFlowException
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.UpdateRequestStatus - Getting authenticated context", LogEventID.InformationWrite);

            this.UsingContext(ctx =>
            {
                LogHelper.LogInformation(string.Format("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.UpdateRequestStatus - Updating site request {0}", listItemId), LogEventID.InformationWrite);

                var web = ctx.Web;
                var list = web.Lists.GetByTitle(siteRequestListName);
                var query = new CamlQuery
                {
                    ViewXml = string.Format(SiteRequestItemByIdQuery, listItemId.ToString(CultureInfo.InvariantCulture))
                };

                ListItemCollection itemCollection = list.GetItems(query);
                ctx.Load(itemCollection);
                ctx.ExecuteQuery();

                if (itemCollection != null && itemCollection.Count != 0)
                {
                    ListItem item = itemCollection.FirstOrDefault();
                    item[SiteRequestListFields.RequestStatusFieldInternalName] = status.ToString();

                    if (!string.IsNullOrEmpty(statusMessage))
                    {
                        item[SiteRequestListFields.RequestStatusTimestampFieldInternalName] = statusMessage;
                    }

                    item.Update();
                    ctx.ExecuteQuery();
                    LogHelper.LogInformation(string.Format("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.UpdateRequestStatus - Updated site request {0} with status and message {1}", listItemId, statusMessage), LogEventID.InformationWrite);
                }
            });

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.UpdateRequestStatus - completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Sends the email notification.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="settings">The settings.</param>
        public void SendEmailNotification(SiteRequestInformation siteRequest, SiteRequestStatus status, AppSettings settings)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.SendEmailNotification", LogEventID.InformationWrite);
            string subject = string.Empty;
            string body = string.Empty;

            try
            {
                LogHelper.LogInformation("Getting Tenant context", LogEventID.InformationWrite);
                this.UsingContext(
                    tenantCtx =>
                    {
                        Tenant tenant = new Tenant(tenantCtx);
                        var site = tenant.GetSiteByUrl(settings.SPHostUrl);

                        using (var ctx = site.Context.Clone(settings.SPHostUrl))
                        {
                            // Get root web
                            var configWeb = ctx.Web;

                            LogHelper.LogInformation("Config Site acquired", LogEventID.InformationWrite);

                            if (status == SiteRequestStatus.Successful)
                            {
                                // Get values from Config list
                                subject = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteRequestEmailSubjectKey, settings.WorkflowConfigurationListName);
                                body = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteApproveEmailBodyKey, settings.WorkflowConfigurationListName);
                            }

                            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
                            {
                                LogHelper.LogInformation("Email message is set and context changed", LogEventID.InformationWrite);

                                // Email properties
                                LogHelper.LogInformation("Email message processing", LogEventID.InformationWrite);
                                string templateTitle = siteRequest.Template;
                                if (!string.IsNullOrEmpty(siteRequest.TemplateTitle))
                                {
                                    templateTitle = siteRequest.TemplateTitle;
                                }

                                EmailProperties properties = new EmailProperties
                                {
                                    To = new string[] { siteRequest.RequestorEmail },
                                    Subject = subject,
                                    Body = string.Format(body, siteRequest.RequestorName, templateTitle, siteRequest.Title, siteRequest.Url, siteRequest.BusinessUnit)
                                };
                                Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(ctx, properties);
                                ctx.ExecuteQuery();
                                LogHelper.LogInformation("Email message sent", LogEventID.InformationWrite);
                            }
                        }
                    },
            1200000,
            settings.SPHostUrl);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "SendEmailNotification method.");
            }
            finally
            {
                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.SendEmailNotification - Completed", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Sends the email notification on exception.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="traceDetails">The trace details.</param>
        public void SendEmailNotificationOnException(SiteRequestInformation siteRequest, SiteRequestStatus status, AppSettings settings, string exceptionMessage, string traceDetails)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.SendEmailNotification", LogEventID.InformationWrite);
            string subject = string.Empty;
            string body = string.Empty;

            try
            {
                LogHelper.LogInformation("Getting Tenant context", LogEventID.InformationWrite);
                this.UsingContext(
                    tenantCtx =>
                    {
                        Tenant tenant = new Tenant(tenantCtx);
                        var site = tenant.GetSiteByUrl(settings.SPHostUrl);

                        using (var ctx = site.Context.Clone(settings.SPHostUrl))
                        {
                            // Get root web
                            var configWeb = ctx.Web;

                            LogHelper.LogInformation("Config Site acquired", LogEventID.InformationWrite);

                            if (status == SiteRequestStatus.Exception)
                            {
                                // Get values from Config list
                                subject = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteRequestFailedEmailSubjectKey, settings.WorkflowConfigurationListName);
                                body = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteRequestFailedEmailBodyKey, settings.WorkflowConfigurationListName);
                            }

                            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
                            {
                                LogHelper.LogInformation("Email message is set and context changed", LogEventID.InformationWrite);

                                // Email properties
                                LogHelper.LogInformation("Email message processing", LogEventID.InformationWrite);
                                string templateTitle = siteRequest.Template;
                                if (!string.IsNullOrEmpty(siteRequest.TemplateTitle))
                                {
                                    templateTitle = siteRequest.TemplateTitle;
                                }

                                EmailProperties properties = new EmailProperties
                                {
                                    To = new string[] { siteRequest.BusinessUnitAdmin.Email },
                                    Subject = subject,
                                    Body = string.Format(body, siteRequest.BusinessUnit, siteRequest.Url, siteRequest.Title, siteRequest.ListItemId.ToString(), exceptionMessage, traceDetails)
                                };
                                Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(ctx, properties);
                                ctx.ExecuteQuery();
                                LogHelper.LogInformation("Email message sent", LogEventID.InformationWrite);
                            }
                        }
                    },
            1200000,
            settings.SPHostUrl);
            }
            catch (Exception errorDetails)
            {
                LogHelper.LogError(errorDetails, LogEventID.ExceptionHandling, "SendEmailNotification method.");
            }
            finally
            {
                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.SendEmailNotification - Completed", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Sends the email notification on exception.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="ex">Exception object</param>
        public void SendEmailNotificationOnException(SiteRequestInformation siteRequest, SiteRequestStatus status, AppSettings settings, Exception ex)
        {
            this.SendEmailNotificationOnException(siteRequest, status, settings, ex != null ? ex.Message : string.Empty, ex != null ? ex.StackTrace : string.Empty);           
        }

        /// <summary>
        /// Sends the notification for sites.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="templateManager">The template manager.</param>
        public void SendNotificationForSites(AppSettings settings, TemplateManager templateManager)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.SendNotificationForSites", LogEventID.InformationWrite);
            List<NotificationMessageParameters> parametersCollectgion = new List<NotificationMessageParameters>();

            this.UsingContext(
                tenantCtx =>
                {
                    Tenant tenant = new Tenant(tenantCtx);
                    var site = tenant.GetSiteByUrl(settings.SPHostUrl);

                    using (var ctx = site.Context.Clone(settings.SPHostUrl))
                    {
                        var web = ctx.Web;
                        var list = web.Lists.GetByTitle(settings.SiteRequestListName);
                        var query = new CamlQuery { ViewXml = CamlQueryHelper.GetNotificationSiteQuery };
                        ListItemCollection itemCollection = list.GetItems(query);
                        ctx.Load(web);
                        ctx.Load(itemCollection);
                        ctx.ExecuteQuery();

                        // Method call to send notification for items whose request status is Pending and Workflow status is either Rejected or MoreInfoRequired.
                        if (itemCollection != null && itemCollection.Count != 0)
                        {
                            foreach (var item in itemCollection)
                            {
                                NotificationMessageParameters parameters = this.ProcessNotificationForSites(ctx, web, item, settings, templateManager);

                                if (parameters != null)
                                {
                                    parametersCollectgion.Add(parameters);
                                }
                            }
                        }

                        if (parametersCollectgion.Count > 0)
                        {
                            this.SendNotificationForSites(ctx, web, parametersCollectgion, settings);
                        }
                        else
                        {
                            LogHelper.LogInformation("NotificationMessageParameters is null, no notification email sent.", LogEventID.InformationWrite);
                        }
                    }
                },
           1200000,
           settings.SPHostUrl);
           
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.SendNotificationForSites - Completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Closes the expired requests.
        /// </summary>
        /// <param name="settings">App Settings</param>
        public void CloseExpiredRequests(AppSettings settings)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.CloseExpiredRequests", LogEventID.InformationWrite);

             this.UsingContext(
                 tenantCtx =>
            {
                Tenant tenant = new Tenant(tenantCtx);
                var site = tenant.GetSiteByUrl(settings.SPHostUrl);

                using (var ctx = site.Context.Clone(settings.SPHostUrl))
                {
                    var web = ctx.Web;
                    var list = web.Lists.GetByTitle(settings.SiteRequestListName);
                    var query = new CamlQuery { ViewXml = CamlQueryHelper.GetInformationWaitingRequests };
                    ListItemCollection itemCollection = list.GetItems(query);
                    ctx.Load(itemCollection);
                    ctx.ExecuteQuery();

                    // Method call to send notification for items whose request status is New and Workflow status is MoreInfoRequired.
                    if (itemCollection != null && itemCollection.Count != 0)
                    {
                        foreach (var item in itemCollection)
                        {
                            CloseExpiredRequest(ctx, item);
                        }
                    }
                }
            },
            1200000,
            settings.SPHostUrl);

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.CloseExpiredRequests - Completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Updates the work flow history list.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="eventCode">The event code.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <param name="workFlowHistoryListName">Name of the work flow history list.</param>        
        public void UpdateWorkFlowHistoryList(SiteRequestInformation siteRequest, int eventCode, Exception ex, string message, string workFlowHistoryListName)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.UpdateWorkFlowHistoryList - Getting authenticated context", LogEventID.InformationWrite);
            this.UsingContext(
                ctx =>
                {
                    LogHelper.LogInformation(string.Format("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.UpdateWorkFlowHistoryList - WorkFlowHistoryList for item {0}", siteRequest.ListItemId), LogEventID.InformationWrite);
                    var web = ctx.Web;
                    ctx.Load(web.CurrentUser);
                    ctx.ExecuteQuery();
                    string itemId = string.Empty;

                    if (string.IsNullOrEmpty(workFlowHistoryListName))
                    {
                        LogHelper.LogInformation("Workflow History list name is empty. Could not update the list", LogEventID.InformationWrite);
                        return;
                    }

                    var list = web.Lists.GetByTitle(workFlowHistoryListName);

                    // Create item and update list
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem workflowItem = list.AddItem(itemCreateInfo);
                    workflowItem[SiteRequestListFields.WorkflowHistoryTitleField] = eventCode.ToString();

                    // SPWorkflowHistoryEventType.WorkflowError = 10
                    workflowItem[SiteRequestListFields.WorkflowHistoryEventTypeField] = 10;
                    workflowItem[SiteRequestListFields.WorkflowHistoryParentInstanceField] = null;
                    workflowItem[SiteRequestListFields.WorkflowHistoryAssociationIDField] = SiteRequestListFields.WorkflowName;
                    workflowItem[SiteRequestListFields.WorkflowHistoryListIDField] = null;
                    workflowItem[SiteRequestListFields.WorkflowHistoryUserID] = web.CurrentUser;
                    workflowItem[SiteRequestListFields.WorkflowHistoryDateOccurred] = DateTime.Now;

                    if (siteRequest != null)
                    {
                        itemId = siteRequest.ListItemId.ToString();

                        if (siteRequest.Url.Length > 200)
                        {
                            workflowItem[SiteRequestListFields.WorkflowHistoryDescription] = siteRequest.Url.Substring(0, 200);
                        }
                        else
                        {
                            workflowItem[SiteRequestListFields.WorkflowHistoryDescription] = siteRequest.Url;
                        }
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        workflowItem[SiteRequestListFields.WorkflowHistoryData] = message + "with Error: " + ex.StackTrace;
                    }
                    else
                    {
                        workflowItem[SiteRequestListFields.WorkflowHistoryData] = ex.StackTrace;
                    }

                    workflowItem.Update();
                    ctx.ExecuteQuery();
                    LogHelper.LogInformation(string.Format("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.UpdateWorkFlowHistoryList - Updated WorkFlowHistoryList for item {0}", itemId), LogEventID.InformationWrite);
                });
        }

        /// <summary>
        /// Deletes the site collection.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        public void DeleteSiteCollection(SiteRequestInformation siteRequest)
        {
            LogHelper.LogInformation(string.Format("Deleting Site Collection {0}", siteRequest.Url), LogEventID.InformationWrite);
            AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

            try
            {
                using (ClientContext ctx = tenantAuthentication.GetAuthenticatedContextForGivenUrl(siteRequest.Url))
                {
                    Tenant tenantSite = new Tenant(ctx);
                    var site = tenantSite.GetSiteByUrl(siteRequest.Url);
                    ctx.Load(site);
                    ctx.ExecuteQuery();

                    if (site != null)
                    {
                        SpoOperation spo = tenantSite.RemoveSite(siteRequest.Url);
                        ctx.Load(spo, i => i.IsComplete);
                        ctx.ExecuteQuery();

                        while (!spo.IsComplete)
                        {
                            // Wait and then try again
                            System.Threading.Thread.Sleep(300);
                            ctx.Load(spo, i => i.IsComplete);
                            ctx.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "DeleteSiteCollection method.");
            }
            finally
            {
                LogHelper.LogInformation(string.Format("Site Collection {0} Deleted", siteRequest.Url), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Closes the workflow failures.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void CloseWorkflowFailures(AppSettings settings)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.CloseExpiredRequests", LogEventID.InformationWrite);

            this.UsingContext(
                  tenantCtx =>
                  {
                      Tenant tenant = new Tenant(tenantCtx);
                      var site = tenant.GetSiteByUrl(settings.SPHostUrl);

                      using (var ctx = site.Context.Clone(settings.SPHostUrl))
                      {
                          var web = ctx.Web;
                          var list = web.Lists.GetByTitle(settings.SiteRequestListName);
                          var query = new CamlQuery { ViewXml = CamlQueryHelper.GetPendingSiteQuery };
                          ListItemCollection itemCollection = list.GetItems(query);
                          ctx.Load(itemCollection);
                          ctx.Load(list);
                          ctx.Load(web);
                          ctx.ExecuteQuery();

                          string approvalWorkflowTitle = settings.ApprovalWorkFlow;
                          string updateWorkflowTitle = settings.UpdateWorkFlow;

                          // Method call to send notification for items whose request status is New and Workflow status is MoreInfoRequired.
                          if (itemCollection != null && itemCollection.Count != 0)
                          {
                              LogHelper.LogInformation("Retrieving Workflow subscriptions.", LogEventID.InformationWrite);
                              WorkflowSubscription approvalSubscription = WorkflowExtensions.GetWorkflowSubscription(web, approvalWorkflowTitle);
                              WorkflowSubscription updateSubscription = WorkflowExtensions.GetWorkflowSubscription(web, updateWorkflowTitle);

                              foreach (var item in itemCollection)
                              {
                                  ctx.Load(item);
                                  ctx.ExecuteQuery();
                                  LogHelper.LogInformation("Retrieving Workflow instances for list item.", LogEventID.InformationWrite);
                                  WorkflowInstanceCollection instanceCollection = WorkflowExtensions.GetWorkflowInstances(web, list.Id, item);
                                  LogHelper.LogInformation("Retrieved Workflow instances for list item.", LogEventID.InformationWrite);

                                  if (approvalSubscription != null && approvalSubscription.ServerObjectIsNull == false && approvalSubscription.ServerObjectIsNull == false && instanceCollection != null && instanceCollection.ServerObjectIsNull == false && instanceCollection.Count > 0)
                                  {
                                      var hasWorkflowFailed = HasWorkFlowExpired(approvalSubscription, updateSubscription, instanceCollection);

                                      if (hasWorkflowFailed)
                                      {
                                          LogHelper.LogInformation(string.Format("Workflow expired. Updating item {0}.", item.Id), LogEventID.InformationWrite);

                                          // Mark the request Exception    
                                          string statusDateStamp = Constants.RequestStatusException + " - " + DateTime.Now.ToString();
                                          item[SiteRequestListFields.RequestStatusFieldInternalName] = Constants.RequestStatusException;
                                          item[SiteRequestListFields.WorkflowStatusFieldInternalName] = Constants.RequestStatusException;
                                          item[Constants.SiteRequestStatusTimestampField] = statusDateStamp;
                                          item.Update();
                                          ctx.ExecuteQuery();

                                          // Workflowhistroy update.
                                          LogHelper.LogInformation(string.Format("A workflow has failed for a site request. Site Request item id: {0}.", item.Id), LogEventID.InformationWrite);
                                      }
                                  }
                              }
                          }
                      }
                  },
              1200000,
              settings.SPHostUrl);

            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.CloseExpiredRequests - Completed", LogEventID.InformationWrite);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Determines whether [has work flow expired] [the specified approval subscription].
        /// </summary>
        /// <param name="approvalSubscription">The approval subscription.</param>
        /// <param name="updateSubscription">The update subscription.</param>
        /// <param name="instanceCollection">The instance collection.</param>
        /// <returns>Workflow expiry status.</returns>
        private static bool HasWorkFlowExpired(WorkflowSubscription approvalSubscription, WorkflowSubscription updateSubscription, WorkflowInstanceCollection instanceCollection)
        {
            LogHelper.LogInformation("Check workflow expiry.", LogEventID.InformationWrite);
            bool hasWorkflowFailed = false;
            WorkflowInstance latestApprovalWorkFlow = null;
            WorkflowInstance latestUpdatedWorkFlow = null;

            foreach (var workFlow in instanceCollection)
            {
                if (workFlow.WorkflowSubscriptionId == approvalSubscription.Id)
                {
                    if (latestApprovalWorkFlow == null)
                    {
                        latestApprovalWorkFlow = workFlow;
                    }
                    else
                    {
                        if (latestApprovalWorkFlow.LastUpdated < workFlow.LastUpdated)
                        {
                            latestApprovalWorkFlow = workFlow;
                        }
                    }
                }
                else if (workFlow.WorkflowSubscriptionId == updateSubscription.Id)
                {
                    if (latestUpdatedWorkFlow == null)
                    {
                        latestUpdatedWorkFlow = workFlow;
                    }
                    else
                    {
                        if (latestUpdatedWorkFlow.LastUpdated < workFlow.LastUpdated)
                        {
                            latestUpdatedWorkFlow = workFlow;
                        }
                    }
                }
            }

            if (latestApprovalWorkFlow != null)
            {
                if ((latestApprovalWorkFlow.Status == WorkflowStatus.Canceled)
                    || (latestApprovalWorkFlow.Status == WorkflowStatus.Terminated)
                    || (latestApprovalWorkFlow.Status == WorkflowStatus.Suspended))
                {
                    hasWorkflowFailed = true;
                }
            }

            if (latestUpdatedWorkFlow != null)
            {
                if ((latestUpdatedWorkFlow.Status == WorkflowStatus.Canceled)
                   || (latestUpdatedWorkFlow.Status == WorkflowStatus.Terminated)
                   || (latestUpdatedWorkFlow.Status == WorkflowStatus.Suspended))
                {
                    hasWorkflowFailed = true;
                }
            }

            return hasWorkflowFailed;
        }

        /// <summary>
        /// Closes the expired request.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="item">The item.</param>
        private static void CloseExpiredRequest(ClientContext ctx, ListItem item)
        {
            LogHelper.LogInformation("CloseExpiredRequest", LogEventID.InformationWrite);
            DateTime dueDate = DateTime.MinValue;

            if (item["JCIDueDate"] != null)
            {
                if (DateTime.TryParse(item["JCIDueDate"].ToString(), out dueDate))
                {
                    if ((dueDate != DateTime.MinValue) && (dueDate < DateTime.Today))
                    {
                        LogHelper.LogInformation(string.Format("Marking request expired ID : {0}", item.Id), LogEventID.InformationWrite);
                        MarkRequestExpired(ctx, item, Constants.WorkflowStatusFieldRejected);
                    }
                }
            }

            LogHelper.LogInformation("CloseExpiredRequest - Completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Marks the request expired.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="item">The item.</param>
        /// <param name="workFlowStatus">The work flow status.</param>
        private static void MarkRequestExpired(ClientContext ctx, ListItem item, string workFlowStatus)
        {
            string statusDateStamp = workFlowStatus + " - " + DateTime.Now.ToString();
            item[Constants.SiteWorkflowStatusField] = workFlowStatus;
            item[Constants.SiteRequestStatusField] = Constants.RequestStatusFieldPending;
            item[Constants.SiteRequestStatusTimestampField] = statusDateStamp;
            item.Update();
            ctx.ExecuteQuery();
        }

        /// <summary>
        /// Method used to check the related task for the site request  item
        /// </summary>
        /// <param name="currentItem">Current SiteRequest List ITem</param>
        /// <param name="taskItem">Task List Item</param>
        /// <returns>True if task found for </returns>
        private static bool IsTaksRelatedToItem(ListItem currentItem, ListItem taskItem)
        {
            LogHelper.LogInformation("IsTaksRelatedToItem - completed", LogEventID.InformationWrite);
            bool isTaksRelatedToItem = false;

            try
            {
                string taskRelatedItem = Convert.ToString(taskItem[Constants.WorkflowTaskRelatedItemField]);
                if (!string.IsNullOrEmpty(taskRelatedItem))
                {
                    string taskItemId = taskRelatedItem.Split(',')[0].Split(':')[1];

                    if (currentItem.Id.ToString().Equals(taskItemId))
                    {
                        isTaksRelatedToItem = true;
                    }
                }
            }
            catch (Exception exception)
            {
                LogHelper.LogError(exception, LogEventID.ExceptionHandling, "DeleteSiteCollection method.");
            }
            finally
            {
                LogHelper.LogInformation("IsTaksRelatedToItem - completed", LogEventID.InformationWrite);
            }

            return isTaksRelatedToItem;
        }

        /// <summary>
        /// Sends the email notification.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="toUsers">To users.</param>
        private static void SendEmailNotification(ClientContext clientContext, string subject, string body, List<string> toUsers)
        {
            EmailProperties properties = new EmailProperties { To = toUsers, Subject = subject, Body = body };
            Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(clientContext, properties);
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Sends the notification for sites.
        /// </summary>
        /// <param name="ctx">Authenticated client context.</param>
        /// <param name="configWeb">The configuration web.</param>
        /// <param name="parametersCollection">The parameters collection.</param>
        /// <param name="settings">The settings.</param>
        private void SendNotificationForSites(ClientContext ctx, Web configWeb, List<NotificationMessageParameters> parametersCollection, AppSettings settings)
        {
            LogHelper.LogInformation("Processing email message body in SendNotificationForSites.", LogEventID.InformationWrite);
            string emailSubjectRejected = string.Empty;
            string emailBodyRejected = string.Empty;
            string emailSubjectMoreInfo = string.Empty;
            string emailBodyMoreInfo = string.Empty;
            string htmlBody = string.Empty;

            try
            {
                // Creating email structure
                emailSubjectRejected = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteRejectEmailSubjectKey, settings.WorkflowConfigurationListName);
                emailBodyRejected = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteRejectEmailBodyKey, settings.WorkflowConfigurationListName);
                emailSubjectMoreInfo = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteMoreInfoRequiredSubjectKey, settings.WorkflowConfigurationListName);
                emailBodyMoreInfo = ConfigListHelper.GetConfigurationListValue(ctx, configWeb, Common.Constants.SiteMoreInfoRequiredEmailBodyKey, settings.WorkflowConfigurationListName);
                
                if (!string.IsNullOrEmpty(emailSubjectRejected) && !string.IsNullOrEmpty(emailBodyRejected) && !string.IsNullOrEmpty(emailSubjectMoreInfo) && !string.IsNullOrEmpty(emailBodyMoreInfo))
                {
                    LogHelper.LogInformation("Email message is set and context changed", LogEventID.InformationWrite);

                    // Email properties
                    LogHelper.LogInformation("Email message processing", LogEventID.InformationWrite);

                    foreach (var parameters in parametersCollection)
                    {
                        List<string> requestorEmailId = new List<string>();
                        requestorEmailId.Add(parameters.RequestorEmail);

                        switch (parameters.WorkFlowStatus)
                        {
                            case Constants.WorkflowStatusRejected:

                                if (!string.IsNullOrEmpty(emailBodyRejected))
                                {
                                    htmlBody = string.Format(emailBodyRejected, parameters.RequestorName, parameters.SiteType, parameters.ApproverComments, parameters.SiteTitle, parameters.BusinessUnit);

                                    if (string.IsNullOrEmpty(parameters.ApproverComments))
                                    {
                                        htmlBody = htmlBody.Replace(Constants.ReplaceRejectedBody, string.Empty);
                                    }

                                    LogHelper.LogInformation(string.Format("Sending email notification for {0}", Constants.WorkflowStatusRejected), LogEventID.InformationWrite);
                                    SendEmailNotification(ctx, emailSubjectRejected, htmlBody, requestorEmailId);
                                }

                                break;

                            case Constants.WorkflowStatusMoreInfoRequired:

                                if (!string.IsNullOrEmpty(emailBodyMoreInfo))
                                {
                                    htmlBody = string.Format(emailBodyMoreInfo, parameters.RequestorName, parameters.SiteType, parameters.ApproverComments, parameters.SiteUrl, parameters.SiteId, parameters.BusinessUnit);

                                    htmlBody = htmlBody.Replace("<#", "{").Replace("#>", "}");

                                    if (string.IsNullOrEmpty(parameters.ApproverComments))
                                    {
                                        htmlBody = htmlBody.Replace(Constants.ReplaceMoreInfoBody, string.Empty);
                                    }

                                    LogHelper.LogInformation(string.Format("Sending email notification for {0}", Constants.WorkflowStatusMoreInfoRequired), LogEventID.InformationWrite);
                                    SendEmailNotification(ctx, emailSubjectMoreInfo, htmlBody, requestorEmailId);
                                }

                                break;
                        }
                    }

                    LogHelper.LogInformation("Email message sent", LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "SendEmailNotification method.");
            }
            finally
            {
                LogHelper.LogInformation("Processing email message body in SendNotificationForSites - completed.", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Sends the notification for sites.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="templateManager">The template manager.</param>
        /// <returns>NotificationMessageParameters object properties for email notifications</returns>
        private NotificationMessageParameters ProcessNotificationForSites(ClientContext ctx, Web web, ListItem item, AppSettings settings, TemplateManager templateManager)
        {
            LogHelper.LogInformation("Processing request item for constructing email message body and request list update in ProcessNotificationForSites.", LogEventID.InformationWrite);

            // Get LIst item properties
            string workflowStatus = this.BaseSet(item, SiteRequestListFields.WorkflowStatusFieldInternalName);
            string sitetype = this.BaseSet(item, SiteRequestListFields.SiteTypeFieldInternalName);

            // Getting site template from configured templates to get template code. In FTC implementation is was queried form SiteFeaturesConfiguration list.
            var template = templateManager.GetTemplateByID(sitetype);

            if (template != null)
            {
                sitetype = template.Title;           
            }

            string approverComment = this.GetApproverComment(ctx, web, item, workflowStatus);
            string siteTitle = this.BaseSet(item, SiteRequestListFields.TitleFieldInternalName);
            string siteUrlField = this.BaseSetUrl(item, SiteRequestListFields.SiteUrlFieldInternalName);
            string requesterName = this.BaseSet(item, SiteRequestListFields.RequestorNameFieldInternalName);
            string requesterEmail = this.BaseSet(item, SiteRequestListFields.RequestorEmailFieldInternalName);
            string businessUnit = this.BaseSet(item, SiteRequestListFields.BusinessUnitFieldInternalName);

            // Setting NotificationMessageParameters object properties for email notifications
            NotificationMessageParameters parameters = new NotificationMessageParameters
            {
                RequestorName = requesterName,
                RequestorEmail = requesterEmail,
                SiteType = sitetype,
                ApproverComments = approverComment,
                SiteTitle = siteTitle,
                BusinessUnit = businessUnit,
                SiteUrl = web.Url,
                SiteId = item.Id,
                WorkFlowStatus = workflowStatus
            };

            switch (workflowStatus)
            {
                case Constants.WorkflowStatusRejected:
                    {
                        LogHelper.LogInformation(string.Format("Updating Site request list for {0}", Constants.WorkflowStatusRejected), LogEventID.InformationWrite);
                        this.UpdateRequestStatus(item.Id, SiteRequestStatus.Rejected, string.Empty, settings.SiteRequestListName);
                        break;
                    }

                case Constants.WorkflowStatusMoreInfoRequired:
                    {
                        LogHelper.LogInformation(string.Format("Updating Site request list for {0}", Constants.WorkflowStatusMoreInfoRequired), LogEventID.InformationWrite);
                        this.UpdateRequestStatus(item.Id, SiteRequestStatus.New, string.Empty, settings.SiteRequestListName);
                        break;
                    }
            }

            LogHelper.LogInformation("Processing request item for constructing email message body and request list update in ProcessNotificationForSites completed.", LogEventID.InformationWrite);
            return parameters;
        }

        /// <summary>
        /// Gets the approver comment.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="web">The web.</param>
        /// <param name="item">The item.</param>
        /// <param name="workflowStatus">The workflow status.</param>
        /// <returns>Approver comments</returns>
        private string GetApproverComment(ClientContext ctx, Web web, ListItem item, string workflowStatus)
        {
            LogHelper.LogInformation("Getting ApproverComment", LogEventID.InformationWrite);
            string approverComment = string.Empty;

            try
            {
                List taskList = web.Lists.GetByTitle(Constants.WorkflowTaskListName);
                ctx.Load(taskList);
                CamlQuery query = new CamlQuery
                {
                    ViewXml = string.Format(CamlQueryHelper.GetSiteRequestTaskListQuery, workflowStatus)
                };
                ListItemCollection itemCollection = taskList.GetItems(query);
                ctx.Load(itemCollection);
                ctx.ExecuteQuery();

                if (itemCollection != null && itemCollection.Count > 0)
                {
                    foreach (var taskItem in itemCollection)
                    {
                        if (IsTaksRelatedToItem(item, taskItem))
                        {
                            // approverComment = taskItem.Fields.ContainsField(Constants.WorkflowTaskCommentsField) ? Convert.ToString(taskitem[Constants.WorkflowTaskCommentsField]) : string.Empty;
                            approverComment = this.BaseSet(taskItem, Constants.WorkflowTaskCommentsField);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                // WorkflowHistoryLogger.LogErrorToHistory(web, EventCodes.SiteProvisioning, exception.Message, exception);
                // this.UpdateWorkFlowHistoryList(null, EventCodes.SiteProvisioningCreateSiteError, ex, errormesage, this.settings.WorkFlowHistoryListName);
                LogHelper.LogError(exception, LogEventID.ExceptionHandling, "DeleteSiteCollection method.");
            }
            finally
            {
                LogHelper.LogInformation("Getting ApproverComment - completed", LogEventID.InformationWrite);
            }

            return approverComment;
        }

        /// <summary>
        /// Used to get a value from a list
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Item Value</returns>
        private string BaseSet(ListItem item, string fieldName)
        {
            return item[fieldName] == null ? string.Empty : item[fieldName].ToString();
        }

        /// <summary>
        /// Bases the set URL.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Web url string</returns>
        private string BaseSetUrl(ListItem item, string fieldName)
        {
            string siteUrl = string.Empty;

            // return item[fieldName] == null ? string.Empty : item[fieldName].ToString();
            if (item[fieldName] != null)
            {
                if (item[fieldName] is FieldUrlValue)
                {
                    // FieldUrl webUrl = item[fieldName] as FieldUrl;
                    // siteUrl = webUrl.DefaultValue;
                    FieldUrlValue webUrl = item[fieldName] as FieldUrlValue;
                    siteUrl = webUrl.Url;
                }
            }

            return siteUrl;
        }

        /// <summary>
        /// Bases the get.
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Item Value</returns>
        private T BaseGet<T>(ListItem item, string fieldName)
        {
            var value = item[fieldName];
            return (T)value;
        }

        /// <summary>
        /// Bases the set user.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="item">The item.</param>
        /// <param name="field">The field.</param>
        /// <returns>SharePointUser object</returns>
        private SharePointUser BaseSetUser(ClientContext ctx, ListItem item, string field)
        {
            // SharePointUser owner = new SharePointUser();
            SharePointUser owner = null;

            if (item[field] != null)
            {
                FieldUserValue fieldUser = null;

                if (item[field] is FieldUserValue[])
                {
                    FieldUserValue[] fieldUsers = item[field] as FieldUserValue[];

                    if (fieldUsers.Length > 0)
                    {
                        fieldUser = fieldUsers.FirstOrDefault();
                    }
                }
                else
                {
                    fieldUser = (FieldUserValue)item[field];
                }

                if (fieldUser != null)
                {
                    // var fieldUser = (FieldUserValue)item[field];   
                    owner = new SharePointUser();
                    User user = ctx.Web.EnsureUser(fieldUser.LookupValue);
                    ctx.Load(user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                    ctx.ExecuteQuery();

                    owner.Email = user.Email;
                    owner.LoginName = user.LoginName;
                    owner.Name = user.Title;

                    if (owner.LoginName.Contains("|"))
                    {
                        // trim string till membership - i:0#.f|membership|kishore@spoazure.onmicrosoft.com
                        owner.LoginName = owner.LoginName.Substring(owner.LoginName.LastIndexOf("|") + 1);
                    }
                }
            }

            return owner;
        }

        /// <summary>
        /// Bases the set integer.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Field Value in Integer</returns>
        private int BaseSetInt(ListItem item, string fieldName)
        {
            return Convert.ToInt32(item[fieldName]);
        }

        /// <summary>
        /// Helper to return a unit integer from a string field
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Unit integer value</returns>
        private uint BaseSetUint(ListItem item, string fieldName)
        {
            object temp = item[fieldName];
            uint result = new uint();
            if (temp != null)
            {
                uint.TryParse(item[fieldName].ToString(), out result);
                return result;
            }

            return result;
        }

        /// <summary>
        /// Bases the set users.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>List of Users</returns>
        private List<SharePointUser> BaseSetUsers(ClientContext ctx, ListItem item, string fieldName)
        {
            List<SharePointUser> users = new List<SharePointUser>();
            if (item[fieldName] != null && (item[fieldName] is FieldUserValue[]))
            {
                foreach (FieldUserValue userValue in item[fieldName] as FieldUserValue[])
                {
                    if (userValue != null)
                    {
                        User user = ctx.Web.EnsureUser(userValue.LookupValue);
                        ctx.Load(user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                        ctx.ExecuteQuery();

                        var sharePointUser = new SharePointUser()
                        {
                            Email = user.Email,
                            LoginName = user.LoginName,
                            Name = user.Title
                        };

                        if (sharePointUser.LoginName.Contains("|"))
                        {
                            // trim string till membership - i:0#.f|membership|kishore@spoazure.onmicrosoft.com
                            sharePointUser.LoginName = sharePointUser.LoginName.Substring(sharePointUser.LoginName.LastIndexOf("|") + 1);
                        }

                        users.Add(sharePointUser);
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Helper Member to return SiteRequest from the SharePoint SiteRequest Repository
        /// </summary>
        /// <param name="camlQuery">The CAML query.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// List of Site Requests
        /// </returns>
        private ICollection<SiteRequestInformation> GetSiteRequestsByCaml(string camlQuery, AppSettings settings)
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.GetSiteRequestsByCaml - Getting site request details", LogEventID.InformationWrite);
            List<SiteRequestInformation> siteRequests = new List<SiteRequestInformation>();
            this.UsingContext(ctx =>
                {
                    // query
                    var query = new CamlQuery { ViewXml = camlQuery };
                    var web = ctx.Web;
                    ctx.Load(web, w => w.Lists);
                    var list = web.Lists.GetByTitle(settings.SiteRequestListName);
                    ctx.Load(list);
                    var listItemCollection = list.GetItems(query);
                    ctx.Load(
                        listItemCollection,
                         eachItem => eachItem.Include(
                         item => item,
                          item => item[SiteRequestListFields.ListItemIdFieldInternalName],
                         item => item[SiteRequestListFields.TitleFieldInternalName],
                         item => item[SiteRequestListFields.SiteDescriptionFieldInternalName],
                         item => item[SiteRequestListFields.SiteTypeFieldInternalName],
                         item => item[SiteRequestListFields.BusinessUnitFieldInternalName],
                         item => item[SiteRequestListFields.CommunitySiteTypeFieldInternalName],
                         item => item[SiteRequestListFields.ConfidentialityLevelFieldInternalName],
                         item => item[SiteRequestListFields.IsTestSiteFieldInternalName],
                         item => item[SiteRequestListFields.PrimarySiteOwnerFieldInternalName],
                         item => item[SiteRequestListFields.RegionFieldInternalName],
                         item => item[SiteRequestListFields.RequestorEmailFieldInternalName],
                         item => item[SiteRequestListFields.RequestorNameFieldInternalName],
                         item => item[SiteRequestListFields.SecondarySiteOwnerFieldInternalName],
                         item => item[SiteRequestListFields.SiteUrlFieldInternalName],
                         item => item[SiteRequestListFields.TertiarySiteOwnerFieldInternalName],
                         item => item[SiteRequestListFields.RequestStatusFieldInternalName],
                         item => item[SiteRequestListFields.WorkflowStatusFieldInternalName],
                         item => item[SiteRequestListFields.ModeratorFieldInternalName],
                         item => item[SiteRequestListFields.InitialMembersToProvisionFieldInternalName],
                         item => item[SiteRequestListFields.CountryFieldInternalName],
                         item => item[SiteRequestListFields.PartnerNameFieldInternalName],
                         item => item[SiteRequestListFields.ProjectNameFieldInternalName],
                         item => item[SiteRequestListFields.ProjectStartDateFieldInternalName],
                         item => item[SiteRequestListFields.ProjectEndDateFieldInternalName]));

                    LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestManager.GetSiteRequestsByCaml - Loading site request list items", LogEventID.InformationWrite);
                    ctx.ExecuteQuery();

                    foreach (ListItem item in listItemCollection)
                    {
                        var siteInfo = new SiteRequestInformation()
                        {
                            ListItemId = this.BaseSetInt(item, SiteRequestListFields.ListItemIdFieldInternalName),
                            Title = this.BaseSet(item, SiteRequestListFields.TitleFieldInternalName),
                            Description = this.BaseSet(item, SiteRequestListFields.SiteDescriptionFieldInternalName),
                            Template = this.BaseSet(item, SiteRequestListFields.SiteTypeFieldInternalName),
                            Url = this.BaseSetUrl(item, SiteRequestListFields.SiteUrlFieldInternalName),
                            SiteOwner = this.BaseSetUser(ctx, item, SiteRequestListFields.PrimarySiteOwnerFieldInternalName),
                            InitialSiteUsers = this.BaseSetUsers(ctx, item, SiteRequestListFields.InitialMembersToProvisionFieldInternalName),
                            SecondaryAdminUser = this.BaseSetUser(ctx, item, SiteRequestListFields.SecondarySiteOwnerFieldInternalName),
                            RequestStatus = this.BaseSet(item, SiteRequestListFields.RequestStatusFieldInternalName),
                            BusinessUnit = this.BaseSet(item, SiteRequestListFields.BusinessUnitFieldInternalName),
                            CommunitySiteType = this.BaseSet(item, SiteRequestListFields.CommunitySiteTypeFieldInternalName),
                            IsTestSite = this.BaseGet<bool>(item, SiteRequestListFields.IsTestSiteFieldInternalName),
                            TertiaryAdminUser = this.BaseSetUser(ctx, item, SiteRequestListFields.TertiarySiteOwnerFieldInternalName),
                            Region = this.BaseSet(item, SiteRequestListFields.RegionFieldInternalName),
                            RequestorEmail = this.BaseSet(item, SiteRequestListFields.RequestorEmailFieldInternalName),
                            RequestorName = this.BaseSet(item, SiteRequestListFields.RequestorNameFieldInternalName),
                            Country = this.BaseSet(item, SiteRequestListFields.CountryFieldInternalName),
                            Moderator = this.BaseSetUser(ctx, item, SiteRequestListFields.ModeratorFieldInternalName),
                            ConfidentialityLevel = this.BaseSet(item, SiteRequestListFields.ConfidentialityLevelFieldInternalName),
                            PartnerName = this.BaseSet(item, SiteRequestListFields.PartnerNameFieldInternalName),
                            ProjectName = this.BaseSet(item, SiteRequestListFields.ProjectNameFieldInternalName),
                            ProjectStartDate = this.BaseSet(item, SiteRequestListFields.ProjectStartDateFieldInternalName),
                            ProjectEndDate = this.BaseSet(item, SiteRequestListFields.ProjectEndDateFieldInternalName)
                        };

                        siteRequests.Add(siteInfo);
                    }
                });

            return siteRequests;
        }
        #endregion
    }
}