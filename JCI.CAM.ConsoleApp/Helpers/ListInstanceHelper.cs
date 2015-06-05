// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListInstanceHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Provisions List Instance
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// List Instance Helper Class
    /// </summary>
    public static class ListInstanceHelper
    {
        /// <summary>
        /// Contains error data while provisioning lists.
        /// </summary>
        private static string listErrorData = string.Empty;

        /// <summary>
        /// Gets template type of list definition from xml file
        /// </summary>
        /// <returns>List definition template types</returns>
        public static Dictionary<int, int> GetListDefinitionTypeFromXML()
        {
            Dictionary<int, int> listDefinitionTemplateTypes = new Dictionary<int, int>();

            if (GlobalData.ListDefinitionMigrationXmlLocation != null)
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(XmlReader.Create(GlobalData.ListDefinitionMigrationXmlLocation));
                    XmlNodeList templateTypes = xmlDoc.SelectNodes(MigrationConstants.ListDefinitionXMLNodeList);

                    foreach (XmlNode templateType in templateTypes)
                    {
                        try
                        {
                            int sourceType = Convert.ToInt32(templateType.Attributes[MigrationConstants.ListDefinitionXMLSourceTemplateAttribute].Value);
                            int targetType = Convert.ToInt32(templateType.Attributes[MigrationConstants.ListDefinitionXMLTargetTemplateAttribute].Value);

                            listDefinitionTemplateTypes.Add(sourceType, targetType);
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging(ex, string.Format(CultureInfo.InvariantCulture, "Error occured while adding the list definition template type to the dictionary."));
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while reading the list definition xml file.");
                    ExceptionLogging(ex, errorData);
                }
            }

            return listDefinitionTemplateTypes;
        }

        /// <summary>
        /// Exception logging
        /// </summary>
        /// <param name="ex">Exception Object</param>
        /// <param name="errorData">Error message</param>
        public static void ExceptionLogging(Exception ex, string errorData)
        {
            LogHelper.LogInformation(errorData, JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            LogHelper.LogError(ex, LogEventID.ExceptionHandling);
        }

        /// <summary>
        /// Creates new list instance.
        /// </summary>
        /// <param name="siteURL">Site url</param>
        /// <param name="listDefinitionTemplateTypes">List definition template types</param>
        /// <returns>Returns the error data.</returns>
        public static string ProcessListMigration(Uri siteURL, Dictionary<int, int> listDefinitionTemplateTypes)
        {
            listErrorData = string.Empty;

            using (ClientContext context = new ClientContext(siteURL))
            {
                if (GlobalData.Environment == Constants.OnPremiseEnvironment || GlobalData.Environment == Constants.DedicatedEnvironment)
                {
                    context.Credentials = new System.Net.NetworkCredential(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword, GlobalData.ProvidedDomain);
                }
                else
                {
                    SharePointOnlineCredentials sharepointOnlineCredentials = new SharePointOnlineCredentials(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword);
                    context.Credentials = sharepointOnlineCredentials;
                }

                Web web = context.Web;
                context.Load(web, w => w.Url, w => w.Title);
                ListCollection listCollection = web.Lists;
                context.Load(listCollection);
                context.ExecuteQuery();

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List instance operation is started for site {0} ({1}).", web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                Console.WriteLine();

                foreach (List list in listCollection.ToList())
                {
                    MigrateList(web, list, listDefinitionTemplateTypes);
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List instance operation is completed for site {0} ({1}).", web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                WebCollection subSites = web.Webs;
                context.Load(subSites);

                context.ExecuteQuery();

                // Sub-Sites
                if (subSites.Count > 0)
                {
                    foreach (Web subSite in subSites)
                    {
                        context.Load(subSite, w => w.Url, w => w.Title);
                        context.ExecuteQuery();
                        ProcessListMigration(new Uri(subSite.Url), listDefinitionTemplateTypes);
                    }
                }
            }

            return listErrorData;
        }

        /// <summary>
        /// Migrates the list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        /// <param name="listDefinitionTemplateTypes">List definition template types</param>
        /// <param name="deleteIfEmpty">if set to <c>true</c> [delete if empty].</param>
        public static void MigrateList(Web web, List list, Dictionary<int, int> listDefinitionTemplateTypes, bool deleteIfEmpty = true)
        {
            if (IsListBasedOnCustomTemplate(list, listDefinitionTemplateTypes))
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Creating list instance copy for {0}.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                LogHelper.LogInformation("Checking list empty or not", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                if (IsListEmpty(list) && deleteIfEmpty)
                {
                    // Delete the list
                    list.DeleteObject();
                    web.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Deleted empty list {0}.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    CreateList(web, list, listDefinitionTemplateTypes);
                }
            }
        }

        /// <summary>
        /// Checking whether the list is custom or not
        /// </summary>
        /// <param name="list">List Instance</param>
        /// <param name="listDefinitionTemplateTypes">List definition template types</param>
        /// <returns>Whether the list is custom template or not.</returns>
        private static bool IsListBasedOnCustomTemplate(List list, Dictionary<int, int> listDefinitionTemplateTypes)
        {
            if (listDefinitionTemplateTypes.ContainsKey(list.BaseTemplate))
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List details: Title - {0}, Template Type - {1}", list.Title, list.BaseTemplate), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is list empty] [the specified list].
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>Whether the list contains rows or not.</returns>
        private static bool IsListEmpty(List list)
        {
            if (list.ItemCount > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a list or library to a web.
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="sourceList">List Instance</param>
        /// <param name="listDefinitionTemplateTypes">List definition template types</param>
        private static void CreateList(Web web, List sourceList, Dictionary<int, int> listDefinitionTemplateTypes)
        {
            List newList = null;

            try
            {
                string listName = sourceList.Title;
                string newListName = string.Concat(listName, GlobalData.MigrationRequestListTitleExtension);
                bool listContentTypesEnabled = sourceList.ContentTypesEnabled;

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Checking list instance copy is already exist {0}.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                // Checking whether the new list is exists or not.
                bool newListExists = ListExtensions.ListExists(web, newListName);

                // Provisioning list
                if (!newListExists)
                {
                    int targetTemplateType = listDefinitionTemplateTypes[sourceList.BaseTemplate];

                    if (targetTemplateType.ToString().Length > 0)
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Creating list instance copy {0}", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        newList = ListExtensions.CreateList(web, targetTemplateType, newListName, string.Empty, sourceList.EnableVersioning, sourceList.EnableMinorVersions, listContentTypesEnabled);
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Target list template type is null."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} list is already exists.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                    // Accessing the new list
                    newList = web.Lists.GetByTitle(newListName);
                    web.Context.Load(newList, t => t.ItemCount);
                    web.Context.ExecuteQuery();
                }

                if (newList != null && newList.ItemCount == 0)
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Configuring list instance copy {0}", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    newList = web.Lists.GetByTitle(newListName);
                    web.Context.Load(newList);
                    web.Context.ExecuteQuery();

                    ProvisioningContentTypesforList(web, sourceList, newList, listContentTypesEnabled, newListName);
                    ProvisioningListViewsforList(web, sourceList, newList, newListName);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} list is null or being used.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while provisioning the {0} list.", string.Concat(sourceList.Title, GlobalData.MigrationRequestListTitleExtension));
                listErrorData = listErrorData + errorData + ", ";
                ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Adds content type to the new list
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="sourceList">Existing list instance</param>
        /// <param name="newList">New list instance</param>
        /// <param name="listContentTypesEnabled">Holds the value to specify whether the content types is enabled for the list or not</param>
        /// <param name="newListName">New list name.</param>
        private static void ProvisioningContentTypesforList(Web web, List sourceList, List newList, bool listContentTypesEnabled, string newListName)
        {
            if (listContentTypesEnabled)
            {
                try
                {
                    ContentTypeCollection contentTypes = sourceList.ContentTypes;
                    web.Context.Load(contentTypes);
                    web.Context.ExecuteQuery();

                    ListExtensions.RemoveAllContentTypesFromList(web, newList);
                    AddingContentTypesToList(contentTypes, newList);
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while provisioning the content type to the list {0}.", newListName);
                    listErrorData = listErrorData + errorData + ", ";
                    ExceptionLogging(ex, errorData);
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Content Types operation completed for the list {0}.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Content Types not enabled for the list {0}.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Adding content types to the new list from the source list
        /// </summary>
        /// <param name="contentTypes">ContentType Collection</param>
        /// <param name="newList">New list</param>
        private static void AddingContentTypesToList(ContentTypeCollection contentTypes, List newList)
        {
            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Adding content types to {0} list.", newList.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            foreach (ContentType contentType in contentTypes)
            {
                try
                {
                    if (contentType.Hidden || contentType.Sealed)
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not adding {0} content type to the list. Content type properties: Hidden-{1}, Sealed-{2}, ReadOnly-{3}.", contentType.Name, contentType.Hidden, contentType.Sealed, contentType.ReadOnly), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                    else
                    {
                        ListExtensions.AddingContentTypetoList(newList, contentType);
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while adding the {0} content type to the {1} list.", contentType.Name, newList.Title);

                    // Handles the duplicate error
                    if (!errorData.ToLower().Contains(MigrationConstants.DuplicateContentTypeErrorString))
                    {
                        listErrorData = listErrorData + errorData + ", ";
                    }

                    ExceptionLogging(ex, errorData);
                }
            }
        }

        /// <summary>
        /// Adds view to the new list
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="sourceList">Existing list instance</param>
        /// <param name="newList">New list instance</param>
        /// <param name="newListName">New list name</param>
        private static void ProvisioningListViewsforList(Web web, List sourceList, List newList, string newListName)
        {
            try
            {
                ViewCollection listViews = sourceList.Views;
                ViewCollection newListViews = newList.Views;
                web.Context.Load(listViews);
                web.Context.Load(newListViews);
                web.Context.ExecuteQuery();

                int listViewCount = sourceList.Views.Count;

                if (listViewCount > 0)
                {
                    RemoveViewsFromNewList(web, newList, newListViews);
                    AddViewsToNewList(web, newList, listViews);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Views doesn't exists in the list {0}.", sourceList.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "View operation completed for the list {0}.", newList.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while provisioning the content type to the list {0} at site {1}.", newListName, web.Title);
                listErrorData = listErrorData + errorData + ", ";
                ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Deleting views from the new list
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="newList">New List</param>
        /// <param name="newListViews">View Collection</param>
        private static void RemoveViewsFromNewList(Web web, List newList, ViewCollection newListViews)
        {
            foreach (View newListView in newListViews)
            {
                try
                {
                    if (!newListView.Hidden)
                    {
                        ListExtensions.RemoveListView(web, newList, newListViews, newListView);
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} view is deleted.", newListView.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while deleting the {0} view from the {1} list.", newListView.Title, newList.Title);
                    listErrorData = listErrorData + errorData + ", ";
                    ExceptionLogging(ex, errorData);
                }
            }
        }

        /// <summary>
        /// Adding views to the new list from the source list
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="newList">New List</param>
        /// <param name="listViews">View Collection</param>
        private static void AddViewsToNewList(Web web, List newList, ViewCollection listViews)
        {
            foreach (View listView in listViews)
            {
                try
                {
                    if (!listView.Hidden)
                    {
                        string[] viewFields = ListExtensions.GetViewFields(web, listView);
                        ViewType viewType = (ViewType)Enum.Parse(typeof(ViewType), listView.ViewType, true);
                        View newListView = ListExtensions.CreateView(newList, listView.Title, viewType, viewFields, listView.RowLimit, listView.DefaultView, listView.ViewQuery, listView.PersonalView);
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} view is added.", listView.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while adding the {0} view to the {1} list.", listView.Title, newList.Title);
                    listErrorData = listErrorData + errorData + ", ";
                    ExceptionLogging(ex, errorData);
                }
            }
        }
    }
}
