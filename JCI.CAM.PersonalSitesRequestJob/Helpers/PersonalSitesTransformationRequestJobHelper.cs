// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonalSitesTransformationRequestJobHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Timer Job implementation to store the requested items in azure table by executing the personal sites migration request Job
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.PersonalSitesRequestJob.Helpers
{    
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Migration.Common.Helpers;
    using Microsoft.SharePoint.Client;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Timer Job implementation to store the blog transformation requests to the Azure Queue.
    /// </summary>
    public class PersonalSitesTransformationRequestJobHelper
    {
        /// <summary>
        /// The personal site transformation request error data
        /// </summary>
        private string personalSiteTransformationRequestErrorData = string.Empty;

        /// <summary>
        /// Getting the site migration requests
        /// </summary>
        public void ProcessTransformationRequests()
        {
            try
            {
                List<SiteEntity> userPersonalSiteDetails = this.GetUserProfileDetails();

                if (userPersonalSiteDetails.Any())
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "There are {0} personal sites.", userPersonalSiteDetails.Count), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                    this.AddPersonalSitesToAzureTableStorage(userPersonalSiteDetails);
                }
                else
                {
                    LogHelper.LogInformation("JCI.CAM.PersonalSitesRequestJob.Helpers.PersonalSitesTransformationRequestJobHelper() - There are 0 personal sites.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }                
            }
            catch (Exception ex)
            {
                string errorData = "JCI.CAM.PersonalSitesRequestJob.Helpers.PersonalSitesTransformationRequestJobHelper.ProcessTransformationRequests() - Error occured while accessing the user profiles.";
                this.personalSiteTransformationRequestErrorData = this.personalSiteTransformationRequestErrorData + errorData;
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Gets the user profile details.
        /// </summary>
        /// <returns>Returns user details</returns>
        private List<SiteEntity> GetUserProfileDetails()
        {
            try
            {
                LogHelper.LogInformation("Creating site context", LogEventID.InformationWrite);
                using (var context = new ClientContext(GlobalData.SiteProvisioningSiteUrl))
                {
                    context.Credentials = new System.Net.NetworkCredential(GlobalData.UserName, GlobalData.Password, GlobalData.Domain);
                    context.Load(context.Web);
                    context.ExecuteQuery();

                    Web web = context.Web;
                    LogHelper.LogInformation("Site context created", LogEventID.InformationWrite);

                    LogHelper.LogInformation("Querying for user profiles", LogEventID.InformationWrite);
                    List<SiteEntity> sites = web.MySiteSearch();
                    return sites;
                }
            }
            catch (Exception ex)
            {
                string errorData = "Error occured while accessing the user profiles.";
                this.personalSiteTransformationRequestErrorData = this.personalSiteTransformationRequestErrorData + errorData;
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }

            return null;
        }

        /// <summary>
        /// Adds the personal sites to azure table.
        /// </summary>
        /// <param name="personalSitesInfo">The personal sites information.</param>
        private void AddPersonalSitesToAzureTableStorage(List<SiteEntity> personalSitesInfo)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Retrieving the storage account from the connection string {0}", ConfigurationManager.ConnectionStrings["PersonalSitesTransformationTableStorage"].ConnectionString), LogEventID.InformationWrite);

                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["PersonalSitesTransformationTableStorage"].ConnectionString);

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the personal site migration table. And if the specified table not exists then created a table
                CloudTable table = tableClient.GetTableReference(GlobalData.PersonalSitesTransformationTableName);
                table.CreateIfNotExists();

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Accessed table {0}.", GlobalData.PersonalSitesTransformationTableName), LogEventID.InformationWrite);

                foreach (var personalSiteInfo in personalSitesInfo)
                {
                    try
                    {                        
                        string personalSiteURL = personalSiteInfo.Url.ToLower();
                        string status = MigrationCommonHelper.GetEnumFriendlyName(SiteMigrationRequestStatus.NotStarted);
                        string rowKey = personalSiteURL.Replace(@"/", string.Empty);

                        // Construct the query operation for all customer entities where PartitionKey="Smith".
                        TableQuery<PersonalSitesMigrationRequest> query = new TableQuery<PersonalSitesMigrationRequest>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));
                        
                        IEnumerable<PersonalSitesMigrationRequest> personalSiteRequests = table.ExecuteQuery(query);
                        PersonalSitesMigrationRequest personalSiteRequest = personalSiteRequests.FirstOrDefault();

                        // Checking whether the site already exists in the table storage.
                        if (personalSiteRequest != null)
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} site already exists in the table storage.", personalSiteURL), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        }
                        else
                        {                          
                            string loginName = personalSiteInfo.SiteOwnerLogin;
                            string title = personalSiteInfo.Title;
                            string email = string.Empty;
                            
                            string siteType = MigrationCommonHelper.GetEnumFriendlyName(SiteMigrationType.SiteCollection);

                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Adding personal site transformation request to the table storage. Details: Title - {0}, Type - {1}, Migration Status - {2}, url - {3}, user - {4}, user email - {5}", title, siteType, status, personalSiteURL, loginName, email), LogEventID.InformationWrite);

                            // Adding request to table storage
                            PersonalSitesMigrationRequest personalSiteMigrationRequest = new PersonalSitesMigrationRequest();
                            personalSiteMigrationRequest.PartitionKey = title;
                            personalSiteMigrationRequest.RowKey = rowKey;
                            personalSiteMigrationRequest.SiteTitle = title;
                            personalSiteMigrationRequest.SiteType = siteType;
                            personalSiteMigrationRequest.SiteOwner = email;
                            personalSiteMigrationRequest.SiteURL = personalSiteURL;
                            personalSiteMigrationRequest.SiteMigrationStatus = status;
                            personalSiteMigrationRequest.Log = this.personalSiteTransformationRequestErrorData;                            

                            TableOperation tableOperation = TableOperation.Insert(personalSiteMigrationRequest);
                            TableResult result = table.Execute(tableOperation);

                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Added personal site transformation request to the table storage."), LogEventID.InformationWrite);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesRequestJob.Helpers.AddPersonalSitesToAzureTableStorage() - Error occured while adding the request to table storage.");
                        MigrationCommonHelper.ExceptionLogging(ex, errorData);
                    }
                }      
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesRequestJob.Helpers.AddPersonalSitesToList() - Error occured while adding the transformation request to table storage.");
                this.personalSiteTransformationRequestErrorData = this.personalSiteTransformationRequestErrorData + errorData + string.Empty;
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }        
    }
}
