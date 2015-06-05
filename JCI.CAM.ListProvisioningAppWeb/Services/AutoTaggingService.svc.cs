//-----------------------------------------------------------------------
// <copyright file= "AutoTaggingService.svc.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.AutoTaggingAppWeb.Services
{
    using System;
    using System.Configuration;
    using JCI.Azure.Framework.Provisioning;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core;
    using JCI.CAM.Provisioning.Core.Data;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.EventReceivers;

    /// <summary>
    /// Auto tagging service
    /// </summary>
    public class AutoTaggingService : IRemoteEventService
    {
        /// <summary>
        /// Handles events that occur before an action occurs, such as when a user adds or deletes a list item.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <returns>Holds information returned from the remote event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            try
            {
                switch (properties.EventType)
                {
                    case SPRemoteEventType.ItemAdding:
                        this.HandleAutoTaggingItemAdding(properties, result);
                        break;
                    case SPRemoteEventType.ItemAdded:
                        this.HandleAutoTaggingItemAdded(properties);
                        break;
                }

                result.Status = SPRemoteEventServiceStatus.Continue;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Handles events that occur after an action occurs, such as after a user adds an item to a list or deletes an item from a list.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <exception cref="System.NotImplementedException">not implemented</exception>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            try
            {
                switch (properties.EventType)
                {
                    case SPRemoteEventType.ItemAdded:
                        this.HandleAutoTaggingItemAdded(properties);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                throw;
            }
        }

        /// <summary>
        /// Used to Handle the ItemAdding Event
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="result">The result.</param>
        public void HandleAutoTaggingItemAdding(SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            try
            {
                string webUrl = properties.ItemEventProperties.WebUrl;
                Uri webUri = new Uri(webUrl);
                string realm = JCI.CAM.ListProvisioningAppWeb.TokenHelper.GetRealmFromTargetUrl(webUri);
                string accessToken = JCI.CAM.ListProvisioningAppWeb.TokenHelper.GetAppOnlyAccessToken(JCI.CAM.ListProvisioningAppWeb.TokenHelper.SharePointPrincipal, webUri.Authority, realm).AccessToken;
                using (ClientContext ctx = JCI.CAM.ListProvisioningAppWeb.TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))
                {
                    if (ctx != null)
                    {
                        var itemProperties = properties.ItemEventProperties;
                        var userLoginName = properties.ItemEventProperties.UserLoginName;
                        var afterProperites = itemProperties.AfterProperties;

                        AutoTaggingHelper.AssignMetadata(ctx, userLoginName, afterProperites, result);
                    }
                }
            }
            catch (Exception exception)
            {
                LogHelper.LogError(exception, LogEventID.ExceptionHandling);
                throw;
            }
        }

        /// <summary>
        /// Used to handle the ItemAdded event.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void HandleAutoTaggingItemAdded(SPRemoteEventProperties properties)
        {
            try
            {
                AutoTagRequestInformation autoTagRequest = new AutoTagRequestInformation();
                autoTagRequest.SiteURL = properties.ItemEventProperties.WebUrl;
                autoTagRequest.ListID = properties.ItemEventProperties.ListId.ToString();
                autoTagRequest.ListItemID = properties.ItemEventProperties.ListItemId;
                autoTagRequest.UserLoginName = properties.ItemEventProperties.UserLoginName;
                autoTagRequest.UserDisplayName = properties.ItemEventProperties.UserDisplayName;
                autoTagRequest.CurrentUserId = properties.ItemEventProperties.CurrentUserId;
            
                this.AutoTagQueue(autoTagRequest);
            }
            catch (Exception exception)
            {
                LogHelper.LogError(exception, LogEventID.ExceptionHandling);
            }
        }

        /// <summary>
        /// Automatics the tag queue.
        /// </summary>
        /// <param name="autoTagRequest">The automatic tag request.</param>
        /// <exception cref="System.ArgumentException">
        /// The Exception
        /// </exception>
        private void AutoTagQueue(AutoTagRequestInformation autoTagRequest)
        {
            AzureQueueStorage azureQueueStorage = new AzureQueueStorage();

            LogHelper.LogInformation("JCI.CAM.AutoTaggingAppWeb.Services.AutoTaggingServic.AutoTagQueue - Processing autotag request item", LogEventID.InformationWrite);
            LogHelper.LogInformation("JCI.CAM.AutoTaggingAppWeb.Services.AutoTaggingService.AutoTagQueue - Serializing AutoTagRequestInformation object", LogEventID.InformationWrite);
            var payload = XmlSerializerHelper.Serialize<AutoTagRequestInformation>(autoTagRequest);
            LogHelper.LogInformation("JCI.CAM.AutoTaggingAppWeb.Services.AutoTaggingService.AutoTagQueue -Initializing Service Manager and getting Azure connection string and Queue details", LogEventID.InformationWrite);

            azureQueueStorage.AzureConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString();
            azureQueueStorage.RequestQueueName = ConfigurationManager.AppSettings["AutoTagRequestQueue"];

            if (string.IsNullOrEmpty(azureQueueStorage.AzureConnectionString))
            {
                throw new ArgumentException(string.Format("Azure Configuration {0} is missing in the config file", "AzureWebJobsStorage"));
            }

            if (string.IsNullOrEmpty(azureQueueStorage.RequestQueueName))
            {
                throw new ArgumentException(string.Format("Azure Configuration {0} is missing in the config file", "AutoTagRequestQueue"));
            }

            try
            {
                LogHelper.LogInformation("JCI.CAM.AutoTaggingAppWeb.Services.AutoTaggingService.AutoTagQueue - Save message to queue", LogEventID.InformationWrite);
                azureQueueStorage.SendMessageToQueue(payload);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                throw;
            }
        }
    }
}
