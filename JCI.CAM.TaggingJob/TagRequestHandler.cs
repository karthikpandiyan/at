// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagRequesthandler.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Handler to provision request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.TaggingJob
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using JCI.Azure.Framework.Provisioning;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core;
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.Provisioning.Core.Data;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Tag request handler
    /// </summary>
    public class TagRequestHandler
    {
        #region Instance Members

        /// <summary>
        /// The authentication
        /// </summary>
        private AppOnlyAuthenticationTenant authentication = new AppOnlyAuthenticationTenant();

        #endregion

        #region Public Members

        /// <summary>
        /// Processes the request queue.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ProcessRequestQueue(CloudQueueMessage message)
        {
            try
            {
                LogHelper.LogInformation("JCI.CAM.TaggingJob.TagRequestHandler.ProcessRequestQueue - Getting message from queue", LogEventID.InformationWrite);

                if (message != null && !string.IsNullOrEmpty(message.AsString))
                {
                    AutoTagRequestInformation autoTagRequestInfo = XmlSerializerHelper.Deserialize<AutoTagRequestInformation>(message.AsString);
                    this.ProcessRequest(message, autoTagRequestInfo);
                    AzureQueueStorage azureQueueStorage = InitializAzureStorage();
                    azureQueueStorage.DeleteMessageFromQueue(message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                throw;
            }
        }

        /// <summary>
        /// Initialize the azure storage.
        /// </summary>
        /// <returns>Return Azure Storage</returns>
        /// <exception cref="System.ArgumentException">Argument Exception</exception>
        private static AzureQueueStorage InitializAzureStorage()
        {
            AzureQueueStorage azureQueueStorage = new AzureQueueStorage();
            azureQueueStorage.AzureConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString();
            azureQueueStorage.RequestQueueName = ConfigurationManager.AppSettings["AutoTagRequestQueue"];

            if (string.IsNullOrEmpty(azureQueueStorage.AzureConnectionString))
            {
                throw new ArgumentException(string.Format("Azure Configuration {0} is missing in the config file", "AzureWebJobsStorage"));
            }
            else
            {
                LogHelper.LogInformation(string.Format("JCI.CAM.TaggingJob.TagRequestHandler.InitializAzureStorage - Getting connectionstring for queue is {0}", azureQueueStorage.AzureConnectionString), LogEventID.InformationWrite);
            }

            if (string.IsNullOrEmpty(azureQueueStorage.RequestQueueName))
            {
                throw new ArgumentException(string.Format("Azure Configuration {0} is missing in the config file", "AutoTagRequestQueue"));
            }

            return azureQueueStorage;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="autoTagRequestInfo">The automatic tag request information.</param>
        private void ProcessRequest(CloudQueueMessage message, AutoTagRequestInformation autoTagRequestInfo)
        {
            AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

            using (ClientContext tenantContext = tenantAuthentication.GetAuthenticatedContextForGivenUrl(autoTagRequestInfo.SiteURL))
            {
                tenantContext.RequestTimeout = 120000;
                Tenant tenantSite = new Tenant(tenantContext);
                var siteContext = tenantSite.GetSiteByUrl(autoTagRequestInfo.SiteURL);
                using (var context = siteContext.Context.Clone(autoTagRequestInfo.SiteURL))
                {
                    try
                    {
                        string userLoginName = autoTagRequestInfo.UserLoginName;
                        List library = context.Web.Lists.GetById(new Guid(autoTagRequestInfo.ListID));
                        var itemToUpdate = library.GetItemById(autoTagRequestInfo.ListItemID);
                        context.Load(itemToUpdate);
                        context.Load(itemToUpdate.File);
                        context.ExecuteQuery();

                        // verify item checkout
                        if (itemToUpdate.File.CheckOutType == CheckOutType.None)
                        {
                            AutoTaggingHelper.SetMetadataOnItemAdded(context, userLoginName, itemToUpdate, autoTagRequestInfo.CurrentUserId);
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format("List item Id : {0} is checkedout by {1} .", autoTagRequestInfo.ListItemID, autoTagRequestInfo.UserDisplayName), LogEventID.InformationWrite);
                            AzureQueueStorage azureQueueStorage = InitializAzureStorage();
                            azureQueueStorage.UpdateMessage(message, autoTagRequestInfo, TimeSpan.FromHours(2.0));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                        if (ex.Message.Equals("Item does not exist. It may have been deleted by another user."))
                        {
                            LogHelper.LogInformation(string.Format("List item Id : {0} is not available. Will try in next execution cycle.", autoTagRequestInfo.ListItemID), LogEventID.InformationWrite);
                        }
                        else
                        {
                            AzureQueueStorage azureQueueStorage = InitializAzureStorage();
                            azureQueueStorage.UpdateMessage(message, autoTagRequestInfo, TimeSpan.FromHours(2.0));
                        }
                    }
                }
            }
        }
        #endregion
    }
}
