// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISiteRequestManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Interface Site Request Manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Data
{
    using System;
    using System.Collections.Generic;
    using JCI.CAM.Provisioning.Core.Configuration;

    /// <summary>
    /// Interface Site Request Manager
    /// </summary>
    public interface ISiteRequestManager
    {
        /// <summary>
        /// Returns a collection of all Approved Site Requests
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// Will return a collection of new SiteRequests or an empty collection will be returned
        /// </returns>
        ICollection<SiteRequestInformation> GetApprovedRequests(AppSettings settings);

        /// <summary>
        /// Updates the status of a site request in the site repository
        /// </summary>
        /// <param name="listItemId">The list item identifier.</param>
        /// <param name="status">Request Status</param>
        void UpdateRequestStatus(int listItemId, SiteRequestStatus status);

        /// <summary>
        /// Updates the status of a site request in the site repository
        /// </summary>
        /// <param name="listItemId">The list item identifier.</param>
        /// <param name="status">Request Status</param>
        /// <param name="statusMessage">Status Message</param>
        /// <param name="siteRequestListName">Name of the site request list.</param>
        void UpdateRequestStatus(int listItemId, SiteRequestStatus status, string statusMessage, string siteRequestListName);

        /// <summary>
        /// Sends the email notification.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="settings">The settings.</param>
        void SendEmailNotification(SiteRequestInformation siteRequest, SiteRequestStatus status, AppSettings settings);

        /// <summary>
        /// Sends the email notification on exception.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="traceDetails">The trace details.</param>
        void SendEmailNotificationOnException(SiteRequestInformation siteRequest, SiteRequestStatus status, AppSettings settings, string exceptionMessage, string traceDetails);

        /// <summary>
        /// Sends the email notification on exception.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="status">The status.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="ex">Exception details.</param>
        void SendEmailNotificationOnException(SiteRequestInformation siteRequest, SiteRequestStatus status, AppSettings settings, Exception ex);

        /// <summary>
        /// Updates the work flow history list.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        /// <param name="eventCode">The event code.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <param name="workFlowHistoryListName">Name of the work flow history list.</param>        
        void UpdateWorkFlowHistoryList(SiteRequestInformation siteRequest, int eventCode, Exception ex, string message, string workFlowHistoryListName);

        /// <summary>
        /// Deletes the site collection.
        /// </summary>
        /// <param name="siteRequest">The site request.</param>
        void DeleteSiteCollection(SiteRequestInformation siteRequest);

        /// <summary>
        /// Sends the notification for sites.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="templateManager">The template manager.</param>
        void SendNotificationForSites(AppSettings settings, TemplateManager templateManager);

        /// <summary>
        /// Closes the expired requests.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void CloseExpiredRequests(AppSettings settings);

        /// <summary>
        /// Closes the workflow failures.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void CloseWorkflowFailures(AppSettings settings);
    }
}
