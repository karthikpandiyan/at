//-----------------------------------------------------------------------
// <copyright file= "AutoTaggingService.svc.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.AutoTaggingAppWeb.Services
{
    using System;
    using JCI.CAM.Common.Logging;
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
            throw new NotImplementedException();
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
                string realm = TokenHelper.GetRealmFromTargetUrl(webUri);
                string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, webUri.Authority, realm).AccessToken;
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))                               
                {
                    if (ctx != null)
                    {
                        List _list = ctx.Web.Lists.GetByTitle("targer");

                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                        ListItem newItem = _list.AddItem(itemCreateInfo);
                        
                        newItem["Title"] = "test";
                        newItem.Update();
                        ctx.ExecuteQuery();
                        //var itemProperties = properties.ItemEventProperties;
                        //var userLoginName = properties.ItemEventProperties.UserLoginName;
                        //var afterProperites = itemProperties.AfterProperties;
                      
                        //AutoTaggingHelper.AssignMetadata(ctx, userLoginName, afterProperites, result);
                    }
                }
            }
            catch (Exception exception)
            {
                LogHelper.LogError(exception, LogEventID.ExceptionHandling);
                throw;
            }
        }
    }
}
