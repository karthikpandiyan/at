// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  List Extensions
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.Utilities;
    using Microsoft.SharePoint.Client;
    using Microsoft.WindowsAzure;    

    /// <summary>
    /// Class that provides generic list creation and manipulation methods
    /// </summary>
    public static partial class ListExtensions
    {
        /// <summary>
        /// Checks if list exists on the particular site based on the list Title property.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list to be checked.</param>
        /// <returns>True if the list exists</returns>
        public static bool ListExists(this Web web, string listTitle)
        {
            List existingList = null;
            try
            {
                if (string.IsNullOrEmpty(listTitle))
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List title is null at site {0} ({1}).", web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }

                ListCollection lists = web.Lists;
                IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
                web.Context.ExecuteQuery();
                existingList = results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Checking whether list {0} exists or not.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }

            if (existingList != null)
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "The list with title {0} existed.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                return true;
            }

            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "The list with title {0} not existed.", listTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            return false;            
        }

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="listType">Type of the list.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="enableVersioning">if set to <c>true</c> [enable versioning].</param>
        /// <param name="updateAndExecuteQuery">if set to <c>true</c> [update and execute query].</param>
        /// <param name="urlPath">The URL path.</param>
        /// <param name="enableContentTypes">if set to <c>true</c> [enable content types].</param>
        /// <returns>List Instance</returns>
        public static List CreateList(this Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "", bool enableContentTypes = false)
        {
            return CreateListInternal(web, null, (int)listType, listName, enableVersioning, updateAndExecuteQuery, urlPath, enableContentTypes);
        }

        /// <summary>
        /// Adds a default list to a site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="templateType">Built in list template type</param>
        /// <param name="newListName">Name of the list</param>
        /// <param name="description">The description.</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        /// <param name="enableMinorVersions">Enable minor versions on the list</param>
        /// <param name="enableContentTypes">(Optional) Enable content type management</param>
        /// <param name="updateAndExecuteQuery">(Optional) Perform list update and execute query, defaults to true</param>
        /// <param name="quickLaunch">if set to <c>true</c> [quick launch].</param>
        /// <returns>
        /// The newly created list
        /// </returns>
        public static List CreateList(this Web web, int templateType, string newListName, string description, bool enableVersioning, bool enableMinorVersions, bool enableContentTypes = false, bool updateAndExecuteQuery = true, bool quickLaunch = false)
        {
            List newList = null;

            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Creating {0} list or library.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                ListCollection listCol = web.Lists;
                ListCreationInformation lci = new ListCreationInformation();
                lci.Title = newListName;
                lci.Description = description;
                lci.TemplateType = templateType;
                newList = listCol.Add(lci);

                // sets version propety
                newList.EnableVersioning = enableVersioning;
                newList.EnableMinorVersions = enableMinorVersions;

                newList.OnQuickLaunch = quickLaunch;

                // Enables or Disables content type
                newList.ContentTypesEnabled = enableContentTypes;

                // Updates the context
                if (updateAndExecuteQuery)
                {
                    newList.Update();
                    web.Context.Load(listCol);
                    web.Context.ExecuteQuery();
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} list or library created successfully.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }
            
            return newList;
        }

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="templateType">Type of the template.</param>
        /// <param name="newListName">New name of the list.</param>
        /// <param name="enableVersioning">if set to <c>true</c> [enable versioning].</param>
        /// <param name="enableMinorVersions">if set to <c>true</c> [enable minor versions].</param>
        /// <param name="enableContentTypes">if set to <c>true</c> [enable content types].</param>
        /// <param name="updateAndExecuteQuery">if set to <c>true</c> [update and execute query].</param>
        /// <returns>List instance</returns>
        public static List CreateList(this Web web, string templateType, string newListName, bool enableVersioning, bool enableMinorVersions, bool enableContentTypes = false, bool updateAndExecuteQuery = true)
        {
            List newList = null;

            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Creating {0} list or library.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                // Validate Template type input
                ListTemplateType listTemplate = (ListTemplateType)Enum.Parse(typeof(ListTemplateType), templateType);

                if (Enum.IsDefined(typeof(ListTemplateType), listTemplate))
                {                    
                    ListCollection listCol = web.Lists;
                    ListCreationInformation lci = new ListCreationInformation();
                    lci.Title = newListName;
                    lci.TemplateType = (int)listTemplate;
                    newList = listCol.Add(lci);

                    // sets version propety
                    if (lci.TemplateType != (int)ListTemplateType.Survey)
                    {
                        newList.EnableVersioning = enableVersioning;

                        if (lci.TemplateType != (int)ListTemplateType.GenericList)
                        {
                            newList.EnableMinorVersions = enableMinorVersions;
                        }

                        // Enables or Disables content type
                        newList.ContentTypesEnabled = enableContentTypes;
                    }                 
                    
                    // Updates the context
                    if (updateAndExecuteQuery)
                    {
                        newList.Update();
                        web.Context.Load(listCol);
                        web.Context.ExecuteQuery();
                    }

                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} list or library created successfully.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} list or library is not created as template type in not valid.", newListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }                
            }
            catch (ArgumentException aex)
            {
                LogHelper.LogError(aex, LogEventID.ExceptionHandling, "Might be due to invalid list template value");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }

            return newList;
        }

        /// <summary>
        /// Removes all content type from a list/library
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="list">List Instance</param>
        /// <exception cref="System.ArgumentNullException">list name</exception>
        public static void RemoveAllContentTypesFromList(this Web web, List list)
        {
            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Removing the content types from the list {0}", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            if (list == null)
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} list is null.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            else
            {
                try
                {
                    ContentType listContentType = null;
                    ContentTypeCollection contentTypes = list.ContentTypes;
                    web.Context.Load(contentTypes);
                    web.Context.ExecuteQuery();

                    // Deleting content types from the new list
                    for (int contentTypeCount = 0; contentTypeCount < contentTypes.Count; contentTypeCount++)
                    {
                        try
                        {
                            listContentType = contentTypes[contentTypeCount];

                            if (listContentType.Hidden || listContentType.Sealed || listContentType.ReadOnly)
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not able to delete {0} content type from the {1} list . Content type properties: Hidden-{2}, Sealed-{3}, ReadOnly-{4}", listContentType.Name, list.Title, listContentType.Hidden, listContentType.Sealed, listContentType.ReadOnly), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                            else
                            {
                                listContentType.DeleteObject();
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} content type is deleted.", listContentType.Name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Error occured while deleting the {0} content type from the {1} list for {2} site ({3}). Message: {4}.", listContentType.Name, list.Title, web.Title, web.Url, ex.Message), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        }
                    }

                    list.Update();
                    list.Context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List: {0}, Site: {1} ({2}). Message: {3}.", list.Title, web.Title, web.Url, ex.Message), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                }
            }
        }

        /// <summary>
        /// Removes a content type from a list/library by name
        /// </summary>
        /// <param name="list">The list</param>
        /// <param name="contentTypeName">The content type name to remove from the list</param>
        public static void RemoveContentTypeByName(List list, string contentTypeName)
        {
            try
            {
                if (string.IsNullOrEmpty(contentTypeName))
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Content Type is null at list {0}.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    ContentTypeCollection cts = list.ContentTypes;
                    list.Context.Load(cts);

                    IEnumerable<ContentType> results = list.Context.LoadQuery<ContentType>(cts.Where(item => item.Name == contentTypeName));
                    list.Context.ExecuteQuery();

                    ContentType ct = results.FirstOrDefault();

                    if (ct != null)
                    {
                        ct.DeleteObject();
                        list.Update();
                        list.Context.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }
        }

        /// <summary>
        /// Adding a content type to the list/library
        /// </summary>
        /// <param name="list">The list</param>
        /// <param name="contentType">The content type instance to add to the list</param>
        /// <exception cref="System.ArgumentNullException">Content Type</exception>
        public static void AddingContentTypetoList(List list, ContentType contentType)
        {
            try
            {
                if (contentType != null)
                {
                    list.ContentTypes.AddExistingContentType(contentType);
                    list.Context.ExecuteQuery();
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} content type is added.", contentType.Name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Content Type object is null at list {0}.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }
        }

        /// <summary>
        /// Removes a view from a list/library.
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="list">List Instance</param>
        /// <param name="listViews">List View Collection Instance</param>
        /// <param name="listView">View to be remove from the list</param>
        /// <exception cref="System.ArgumentNullException">list view</exception>
        public static void RemoveListView(Web web, List list, ViewCollection listViews, View listView)
        {
            try
            {
                if (listView != null)
                {
                    IEnumerable<View> results = list.Context.LoadQuery<View>(listViews.Where(item => item.Title == listView.Title));
                    list.Context.ExecuteQuery();

                    View deleteView = results.FirstOrDefault();
                    if (deleteView != null)
                    {
                        deleteView.DeleteObject();
                        list.Update();
                        list.Context.ExecuteQuery();
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "List View object is null at list {0}, site {1} ({2}).", list.Title, web.Title, web.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }
        }

        /// <summary>
        /// Gets all the fields which are to be displayed in the list view.
        /// </summary>
        /// <param name="web">Web Instance</param>
        /// <param name="view">View Instance</param>
        /// <returns>
        /// Returns view fields
        /// </returns>
        public static string[] GetViewFields(Web web, View view)
        {
            web.Context.Load(view.ViewFields);
            web.Context.ExecuteQuery();
            return view.ViewFields.ToArray();
        }

        /// <summary>
        /// Create view to existing list
        /// </summary>
        /// <param name="list">List Instance</param>
        /// <param name="viewName">View Name</param>
        /// <param name="viewType">View Type</param>
        /// <param name="viewFields">Fields to be displayed.</param>
        /// <param name="rowLimit">No. of rows to be displayed per page.</param>
        /// <param name="setAsDefault">View to be shown as default.</param>
        /// <param name="query">view query</param>
        /// <param name="personal">Personal View</param>
        /// <returns>
        /// Returns the created view
        /// </returns>
        /// <exception cref="System.ArgumentNullException">view name</exception>
        public static View CreateView(this List list, string viewName, ViewType viewType, string[] viewFields, uint rowLimit, bool setAsDefault, string query = null, bool personal = false)
        {
            View view = null;

            if (string.IsNullOrEmpty(viewName))
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "View name is null at list {0}.", list.Title), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }

            ViewCreationInformation viewCreationInformation = new ViewCreationInformation();
            viewCreationInformation.Title = viewName;
            viewCreationInformation.ViewTypeKind = viewType;
            viewCreationInformation.RowLimit = rowLimit;
            viewCreationInformation.ViewFields = viewFields;
            viewCreationInformation.PersonalView = personal;
            viewCreationInformation.SetAsDefaultView = setAsDefault;

            if (!string.IsNullOrEmpty(query))
            {
                viewCreationInformation.Query = query;
            }

            view = list.Views.Add(viewCreationInformation);
            list.Context.Load(view);
            list.Context.ExecuteQuery();
            list.Update();
            list.Context.ExecuteQuery();
            return view;
        }

        /// <summary>
        /// Gets a view by Name
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>
        /// returns null if not found
        /// </returns>
        public static View GetViewByName(this List list, string viewName)
        {
            LogHelper.LogInformation(string.Format("Getting view by title. {0}.", viewName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            viewName.ValidateNotNullOrEmpty("viewName");

            try
            {
                var view = list.Views.GetByTitle(viewName);

                list.Context.Load(view);
                list.Context.ExecuteQuery();
                LogHelper.LogInformation(string.Format("{0} view found.", viewName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                return view;
            }
            catch (ServerException)
            {
                LogHelper.LogInformation(string.Format("{0} view not found.", viewName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                return null;
            }
        }

        /// <summary>
        /// Gets a view by Name
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="viewName">Name of the view.</param>
        public static void DeleteViewByName(this List list, string viewName)
        {
            View view = list.GetViewByName(viewName);
            if (view != null)
            {
                list.Context.Load(view);
                view.DeleteObject(); 
                list.Update();
                list.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Adds the remote event receiver.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="name">The name.</param>
        /// <param name="url">The URL.</param>
        /// <param name="eventReceiverType">Type of the event receiver.</param>
        /// <param name="synchronization">The synchronization.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <returns>return event receiver</returns>
        public static EventReceiverDefinition AddRemoteEventReceiver(this List list, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, int sequenceNumber, bool force)
        {
            LogHelper.LogInformation(string.Format("Adding {0} remote event receiver to list.", name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            var query = from receiver
                     in list.EventReceivers
                        where receiver.ReceiverName == name
                        select receiver;
            var receivers = list.Context.LoadQuery(query);
            list.Context.ExecuteQuery();

            var receiverExists = receivers.Any();
            if (receiverExists && force)
            {
                var receiver = receivers.FirstOrDefault();
                receiver.DeleteObject();
                list.Context.ExecuteQuery();
                receiverExists = false;
            }

            EventReceiverDefinition def = null;

            if (!receiverExists)
            {
                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.EventType = eventReceiverType;
                receiver.ReceiverUrl = url;
                receiver.ReceiverName = name;
                receiver.SequenceNumber = sequenceNumber;
                receiver.Synchronization = synchronization;                
                def = list.EventReceivers.Add(receiver);
                list.Context.Load(def);
                list.Context.ExecuteQuery();
                LogHelper.LogInformation(string.Format("Added {0} remote event receiver to list successfully.", name), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }

            return def;
        }

        /// <summary>
        /// Gets the list by title.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="listTitle">The list title.</param>
        /// <returns>List object</returns>
        /// <exception cref="System.ArgumentNullException">List Title is null</exception>
        /// <exception cref="System.ArgumentException">List title is null</exception>
        public static List GetListByTitle(this Web web, string listTitle)
        {
            if (string.IsNullOrEmpty(listTitle))
            {
                throw (listTitle == null)
                  ? new ArgumentNullException("listTitle")
                  : new ArgumentException("List title is null");
            }

            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
            web.Context.ExecuteQueryRetry();
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Gets the list by URL.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="webRelativeUrl">The web relative URL.</param>
        /// <returns>Share point list</returns>
        /// <exception cref="System.ArgumentNullException">web Relative Url</exception>
        public static List GetListByUrl(this Web web, string webRelativeUrl)
        {
            if (string.IsNullOrEmpty(webRelativeUrl))
            {
                throw new ArgumentNullException("webRelativeUrl");
            }

            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }

            var listServerRelativeUrl = UrlUtility.Combine(web.ServerRelativeUrl, webRelativeUrl);
            var foundList = web.GetList(listServerRelativeUrl);
            web.Context.Load(foundList, l => l.DefaultViewUrl, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden, l => l.RootFolder);
           
            try
            {
                web.Context.ExecuteQueryRetry();
            }
            catch (ServerException se)
            {
                if (se.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    foundList = null;
                }
                else
                {
                    throw;
                }
            }

            return foundList;
        }

        /// <summary>
        /// Replaces the URL tokens.
        /// </summary>
        /// <param name="listReceiverUrl">The list receiver URL.</param>
        /// <returns>Replaced tokens string</returns>
        public static string ReplaceUrlTokens(string listReceiverUrl)
        {
            if (string.IsNullOrEmpty(listReceiverUrl))
            {
                return listReceiverUrl;
            }

            string webSiteHostName = CloudConfigurationManager.GetSetting("WebSiteHostName");
            if (webSiteHostName != null)
            {
                string receiverUrl = listReceiverUrl.Replace("~webSiteHostName", webSiteHostName);
                return receiverUrl;
            }

            return listReceiverUrl;
        }

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="templateFeatureId">The template feature identifier.</param>
        /// <param name="templateType">Type of the template.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="enableVersioning">if set to <c>true</c> [enable versioning].</param>
        /// <param name="updateAndExecuteQuery">if set to <c>true</c> [update and execute query].</param>
        /// <param name="urlPath">The URL path.</param>
        /// <param name="enableContentTypes">if set to <c>true</c> [enable content types].</param>
        /// <returns>List Instance</returns>
        private static List CreateListInternal(this Web web, Guid? templateFeatureId, int templateType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "", bool enableContentTypes = false)
        {
            LogHelper.LogInformation(string.Format("Creating list '{0}' from template {1}{2}.", listName, templateType, templateFeatureId.HasValue ? " (feature " + templateFeatureId.Value.ToString() + ")" : string.Empty));

            ListCollection listCollection = web.Lists;
            ListCreationInformation listCreationInfo = new ListCreationInformation();
            listCreationInfo.Title = listName;
            listCreationInfo.TemplateType = templateType;
            if (templateFeatureId.HasValue)
            {
                listCreationInfo.TemplateFeatureId = templateFeatureId.Value;
            }

            if (!string.IsNullOrEmpty(urlPath))
            {
                listCreationInfo.Url = urlPath;
            }

            List newList = listCollection.Add(listCreationInfo);

            if (enableVersioning)
            {
                newList.EnableVersioning = true;
                newList.EnableMinorVersions = true;
            }

            if (enableContentTypes)
            {
                newList.ContentTypesEnabled = true;
            }

            if (updateAndExecuteQuery)
            {
                newList.Update();
                web.Context.Load(listCollection);
                web.Context.ExecuteQueryRetry();
            }

            return newList;
        }
    }
}
