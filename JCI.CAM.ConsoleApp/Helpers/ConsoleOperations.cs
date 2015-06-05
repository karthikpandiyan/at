// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleOperations.cs" company="Microsoft Corporation &amp; Toyota">
//   Copyright (c) Microsoft Corporation and Toyota
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.Models;
    using JCI.CAM.Common.SPHelpers;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Publishing;
    using Microsoft.SharePoint.Client.WebParts;

    /// <summary>
    /// Console Operation class
    /// </summary>
    public static class ConsoleOperations
    {
        /// <summary>
        /// The JCI customer field title
        /// </summary>
        private static readonly string JciCustomerFieldTitle = "JCICustomer";

        /// <summary>
        /// The JCI customer field
        /// </summary>
        private static readonly string JciCustomerFieldXml = "<Field Name='JCICustomer' ID='{03574B2B-6923-40BE-83C6-0694BD73DE27}' StaticName='JCICustomer' DisplayName='JCI Customer' Group='JCI.Foundation.Columns' Type='Text' Required='FALSE' MaxLength='255' AllowDeletion='TRUE' />";

        /// <summary>
        /// Path for Site Columns XML file
        /// </summary>
        private static string fieldsXmlPath;

        /// <summary>
        /// Path for content types XML file
        /// </summary>
        private static string contentTypeXmlPath;

        /// <summary>
        /// Path for document template XML file
        /// </summary>
        private static string documentTemplatesXmlPath;

        /// <summary>
        /// Path for document templates folder path
        /// </summary>
        private static string documentTemplatesLocationPath;

        /// <summary>
        /// Path for taxonomy fields xml path
        /// </summary>
        private static string taxonomyFieldsXmlPath;

        /// <summary>
        /// The site migration request list path
        /// </summary>
        private static string siteMigrationRequestListPath;

        /// <summary>
        /// The site provisioning lists XML location
        /// </summary>
        private static string siteProvisioningListsXmlLocation;

        /// <summary>
        /// The list templates XML location
        /// </summary>
        private static string listTemplatesXmlLocation;

        /// <summary>
        /// Themes info
        /// </summary>
        private static ThemeEntity themeInfo;

        /// <summary>
        /// Deploys the fields and content types.
        /// </summary>
        /// <param name="web">The web.</param>
        public static void DeployFieldsAndContentTypes(Web web)
        {
            LogHelper.LogInformation("Started provisioning fields ...", LogEventID.InformationWrite);
            fieldsXmlPath = Path.GetFullPath(GlobalData.FieldsXmlLocation);
            web.CreateFieldsFromXMLFile(fieldsXmlPath);
            LogHelper.LogInformation("Completed fields provisioning.", LogEventID.InformationWrite);

            LogHelper.LogInformation("Started provisioning content types ...", LogEventID.InformationWrite);
            contentTypeXmlPath = Path.GetFullPath(GlobalData.ContentTypeXmlLocation);
            web.CreateContentTypeFromXMLFile(contentTypeXmlPath);
            LogHelper.LogInformation("Completed content types provisioning.", LogEventID.InformationWrite);

            LogHelper.LogInformation("Started associating document templates to respective content types ...", LogEventID.InformationWrite);
            documentTemplatesXmlPath = Path.GetFullPath(GlobalData.DocumentTemplatesXmlLocation);
            documentTemplatesLocationPath = Path.GetFullPath(GlobalData.DocumentTemplatesLocation);
            web.CreateDocumentContentTypeFromXML(documentTemplatesXmlPath, documentTemplatesLocationPath);
            LogHelper.LogInformation("Completed document templates association to content types.", LogEventID.InformationWrite);

            LogHelper.LogInformation("Started mapping Taxonomy fields to respective terms...", LogEventID.InformationWrite);
            taxonomyFieldsXmlPath = Path.GetFullPath(GlobalData.TaxonomyFieldsXmlLocation);
            web.BindFieldsToTermSetsFromXMLFile(taxonomyFieldsXmlPath);
            LogHelper.LogInformation("Completed Taxonomy field mappings.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Configures the site migration request list.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void ConfigureSiteMigrationRequestList(ClientContext context)
        {
            LogHelper.LogInformation("Started provisioning lists ...", LogEventID.InformationWrite);
            siteMigrationRequestListPath = Path.GetFullPath(GlobalData.SiteMigrationRequestListXmlLocation);
            ListDefinitions listDefinitions = GetListDefinition(siteMigrationRequestListPath);
            if (listDefinitions != null)
            {
                context.Load(context.Site.RootWeb);
                context.ExecuteQuery();
                Web rootWeb = context.Site.RootWeb;

                context.Load(context.Web);
                context.ExecuteQuery();
                Web web = context.Web;

                List<ListDefinition> lists = listDefinitions.List;
                foreach (var listDefinition in lists)
                {
                    if (web.ListExists(listDefinition.Title))
                    {
                        LogHelper.LogInformation(string.Format("{0} list already existed. Please try with different list title.", listDefinition.Title), LogEventID.InformationWrite);
                    }
                    else
                    {
                        ListProvisionHelper.CreateList(web, rootWeb, listDefinition);
                    }
                }
            }

            LogHelper.LogInformation("Completed list provisioning.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Gets the site migration request list definition.
        /// </summary>
        /// <param name="siteMigrationRequestListPath">The site migration request list path.</param>
        /// <returns>
        /// Return List definitions
        /// </returns>
        public static ListDefinitions GetListDefinition(string siteMigrationRequestListPath)
        {
            ListDefinitions listDefinition = new ListDefinitions();

            if (string.IsNullOrEmpty(siteMigrationRequestListPath))
            {
                return listDefinition;
            }

            XmlSerializer deserializer = new XmlSerializer(typeof(ListDefinitions));
            TextReader textReader = new System.IO.StreamReader(siteMigrationRequestListPath);
            listDefinition = (ListDefinitions)deserializer.Deserialize(textReader);
            textReader.Close();
            return listDefinition;
        }

        /// <summary>
        /// Gets the list templates.
        /// </summary>
        /// <param name="siteMigrationRequestListPath">The site migration request list path.</param>
        /// <returns>Returns list templates</returns>
        public static ListTemplates GetListTemplates(string siteMigrationRequestListPath)
        {
            ListTemplates listDefinition = new ListTemplates();

            if (string.IsNullOrEmpty(siteMigrationRequestListPath))
            {
                return listDefinition;
            }

            XmlSerializer deserializer = new XmlSerializer(typeof(ListTemplates));
            TextReader textReader = new System.IO.StreamReader(siteMigrationRequestListPath);
            listDefinition = (ListTemplates)deserializer.Deserialize(textReader);
            textReader.Close();
            return listDefinition;
        }

        /// <summary>
        /// Deploys the workflows.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void DeployWorkflows(ClientContext context)
        {
            LogHelper.LogInformation("Loading site provisioning site ...", LogEventID.InformationWrite);
            Web web = context.Web;
            context.Load(web);
            context.ExecuteQuery();

            string filePath = Path.GetFullPath(GlobalData.SiteRequestApprovalWorkflowFilePath);
            WorkflowExtensions.DeployWorkflow(web, context, GlobalData.SiteRequestApprovalWorkflowDefinitionName, filePath);
            List siteRequestList = web.GetListByTitle(GlobalData.SiteRequestListTitle);

            if (siteRequestList == null)
            {
                LogHelper.LogInformation(string.Format("SiteRequest List not found at web site : {0}", web.Url), LogEventID.InformationWrite);
                return;
            }

            LogHelper.LogInformation("Getting Site Request Approval workflow definition reference", LogEventID.InformationWrite);
            var approvalWorkflowDefinition = web.GetWorkflowDefinition(GlobalData.SiteRequestApprovalWorkflowDefinitionName);
            LogHelper.LogInformation("Associating Approval workflow to the list", LogEventID.InformationWrite);
            var approvalWorkflowSubscription = siteRequestList.GetWorkflowSubscription(GlobalData.SiteRequestApprovalWorkflowSubscriptionName);
            if (approvalWorkflowSubscription == null)
            {
                siteRequestList.AddWorkflowSubscription(approvalWorkflowDefinition, GlobalData.SiteRequestApprovalWorkflowSubscriptionName, true, true, false, GlobalData.HistoryListName, GlobalData.WorkflowTaskListName);
            }

            filePath = Path.GetFullPath(GlobalData.SiteRequestUpdateWorkflowFilePath);
            WorkflowExtensions.DeployWorkflow(web, context, GlobalData.SiteRequestUpdateWorkflowDefinitionName, filePath);
            LogHelper.LogInformation("Getting Site Request Update workflow definition reference", LogEventID.InformationWrite);
            var updateWorkflowDefinition = web.GetWorkflowDefinition(GlobalData.SiteRequestUpdateWorkflowDefinitionName);
            LogHelper.LogInformation("Associating Update workflow to the list", LogEventID.InformationWrite);
            var updateWorkflowSubscription = siteRequestList.GetWorkflowSubscription(GlobalData.SiteRequestUpdateWorkflowSubscriptionName);
            if (updateWorkflowSubscription == null)
            {
                siteRequestList.AddWorkflowSubscription(updateWorkflowDefinition, GlobalData.SiteRequestUpdateWorkflowSubscriptionName, true, false, true, GlobalData.HistoryListName, GlobalData.WorkflowTaskListName);
            }

            LogHelper.LogInformation("Workflow deployment operation completed", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Adds the fields to site request list.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void AddFieldsToSiteRequestList(ClientContext context)
        {
            LogHelper.LogInformation("Add fields to SiteRequest List", LogEventID.InformationWrite);

            string siteRequestListTitle = GlobalData.SiteRequestListTitle;
            if (string.IsNullOrEmpty(siteRequestListTitle))
            {
                LogHelper.LogInformation("SiteRequest List is null or empty", LogEventID.InformationWrite);
                return;
            }

            Web web = context.Web;

            List siteRequestList = web.GetListByTitle(siteRequestListTitle);

            if (siteRequestList == null)
            {
                LogHelper.LogInformation(string.Format("SiteRequest List not found at web site : {0}", web.Url), LogEventID.InformationWrite);
                return;
            }

            if (!siteRequestList.FieldExistsByName(JciCustomerFieldTitle))
            {
                siteRequestList.CreateField(JciCustomerFieldXml);
                LogHelper.LogInformation(string.Format("Added field JCICustomer to list {0}.", siteRequestList.Title), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Configures the site request list indexed columns.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void ConfigureSiteRequestListIndexedColumns(ClientContext context)
        {
            LogHelper.LogInformation("Started SiteRequest List columns indexing...", LogEventID.InformationWrite);

            string siteRequestListTitle = GlobalData.SiteRequestListTitle;
            if (string.IsNullOrEmpty(siteRequestListTitle))
            {
                LogHelper.LogInformation("SiteRequest List is null or empty", LogEventID.InformationWrite);
                return;
            }

            string siteRequestListColumns = GlobalData.SiteRequestListIndexbleColumns;
            if (string.IsNullOrEmpty(siteRequestListColumns))
            {
                LogHelper.LogInformation("SiteRequest List indexble columns are null or empty", LogEventID.InformationWrite);
                return;
            }

            List<string> indexbleColumns = siteRequestListColumns.Split(',').ToList();

            Web web = context.Web;

            List siteRequestList = web.GetListByTitle(siteRequestListTitle);

            if (siteRequestList == null)
            {
                LogHelper.LogInformation(string.Format("SiteRequest List not found at web site : {0}", web.Url), LogEventID.InformationWrite);
                return;
            }

            foreach (string fieldName in indexbleColumns)
            {
                try
                {
                    if (siteRequestList.FieldExistsByName(fieldName))
                    {
                        siteRequestList.SetListFieldIndex(fieldName, true);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling, string.Format("Index for the field {0} not seted.", fieldName));
                }
            }

            LogHelper.LogInformation("Completed SiteRequest list columns indexed.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Gets the theme details from XML.
        /// </summary>
        public static void GetThemeDetailsFromXML()
        {
            LogHelper.LogInformation("Serialize themes package xml to object.", LogEventID.InformationWrite);

            string xmlPath = Path.GetFullPath(GlobalData.ThemeXmlLocation);

            if (!string.IsNullOrEmpty(xmlPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ThemeEntity));
                using (TextReader textReader = new System.IO.StreamReader(xmlPath))
                {
                    themeInfo = (ThemeEntity)deserializer.Deserialize(textReader);
                }

                LogHelper.LogInformation("Serialized theme xml to object.", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Activates sandbox solution
        /// </summary>
        /// <param name="context">The context.</param>
        public static void ActivateSandBoxSolution(ClientContext context)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.ConsoleApp.ConsoleApp.ActivateSandBoxSolution() - Getting the sites which are successfully migrated from list {0}...", GlobalData.SiteMigrationRequestListName), LogEventID.InformationWrite);
                context.Load(context.Site.RootWeb);
                context.ExecuteQuery();

                // Root site
                Web rootWeb = context.Site.RootWeb;
                context.Load(rootWeb, w => w.Url);
                List siteMigrationRequestList = rootWeb.Lists.GetByTitle(GlobalData.SiteMigrationRequestListName);
                context.Load(siteMigrationRequestList);
                context.ExecuteQuery();

                // Getting the sites which are succesfully migrated
                List<string> successfullyMigratedSites = TraverseSiteMigrationList(context, rootWeb, siteMigrationRequestList);

                InstallSandBoxSolutionAndUploadTheme(successfullyMigratedSites);

                LogHelper.LogInformation("Completed activating sandbox solutions.", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.ConsoleApp.ConsoleApp.ActivateSandBoxSolution() - Error occured while accessing the site.");
                ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Configures the site provisioning lists.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void ConfigureSiteProvisioningLists(ClientContext context)
        {
            LogHelper.LogInformation("Started provisioning lists ...", LogEventID.InformationWrite);
            siteProvisioningListsXmlLocation = Path.GetFullPath(GlobalData.SiteProvisioningListsXmlLocation);
            listTemplatesXmlLocation = Path.GetFullPath(GlobalData.ListTemplatesXmlLocation);
            ListTemplates listInstances = GetListTemplates(listTemplatesXmlLocation);
            ListDefinitions listDefinitions = GetListDefinition(siteProvisioningListsXmlLocation);
            if (listInstances != null && listDefinitions != null)
            {
                context.Load(context.Site.RootWeb);
                context.ExecuteQuery();
                Web web = context.Site.RootWeb;

                if (listDefinitions.List != null)
                {
                    List<ListDefinition> lists = listDefinitions.List;
                    foreach (var listInstance in listInstances.ListInstances)
                    {
                        try
                        {
                            var listDefinition = lists.FindAll(c => c.Title == listInstance.CustomListDefinitionName).FirstOrDefault();
                            ProvisionListInstance(context, web, listDefinition, listInstance);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                        }
                    }
                }
            }

            LogHelper.LogInformation("Completed list provisioning.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Adds the work flow configuration.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        public static void AddWorkFlowConfiguration(ClientContext context, Web web)
        {
            LogHelper.LogInformation("Started WorkFlow Configuration ...", LogEventID.InformationWrite);

            if (web.ListExists(Constants.WorkflowConfiguration))
            {
                List workFlowConfiguration = web.Lists.GetByTitle(Constants.WorkflowConfiguration);
                web.Context.Load(workFlowConfiguration);
                web.Context.ExecuteQuery();

                if (workFlowConfiguration != null)
                {
                    // Enter values for DueDateCountKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.DueDateCountKey))
                    {
                        AddItem(workFlowConfiguration, Constants.DueDateCountKey, Constants.DueDateCountKeyValue, null);
                    }

                    // Enter values for TaskAssignedEmailKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.TaskAssignedEmailKey))
                    {
                        AddItem(workFlowConfiguration, Constants.TaskAssignedEmailKey, Constants.TaskAssignedEmailKeyValue, null);
                    }

                    // Enter values for TaskCanceledEmailKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.TaskCanceledEmailKey))
                    {
                        AddItem(workFlowConfiguration, Constants.TaskCanceledEmailKey, Constants.TaskCanceledEmailKeyValue, null);
                    }

                    // Enter values for TaskDueEmailKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.TaskDueEmailKey))
                    {
                        AddItem(workFlowConfiguration, Constants.TaskDueEmailKey, Constants.TaskDueEmailKeyValue, null);
                    }

                    // Enter values for ApproverListQueryKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.ApproverListQueryKey))
                    {
                        AddItem(workFlowConfiguration, Constants.ApproverListQueryKey, Constants.ApproverListQueryKeyValue, null);
                    }

                    // Enter values for SiteMoreInfoRequiredEmailBodyKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMoreInfoRequiredEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMoreInfoRequiredEmailBodyKey, Constants.SiteMoreInfoRequiredEmailBodyKeyValue, null);
                    }

                    // Enter values for SiteMigrationPendingEmailBodyKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationPendingEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationPendingEmailBodyKey, Constants.SiteMigrationPendingEmailBodyKeyValue, null);
                    }

                    // Enter values for SiteMigrationPendingEmailSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationPendingEmailSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationPendingEmailSubjectKey, Constants.SiteMigrationPendingEmailSubjectKeyValue, null);
                    }

                    // Enter values for SiteMigrationFailedEmailBodyKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationFailedEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationFailedEmailBodyKey, Constants.SiteMigrationFailedEmailBodyKeyValue, null);
                    }

                    // Enter values for SiteMigrationSuccessEmailBodyKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationSuccessEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationSuccessEmailBodyKey, Constants.SiteMigrationSuccessEmailBodyKeyValue, null);
                    }

                    // Enter values for SiteMigrationFailedEmailSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationFailedEmailSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationFailedEmailSubjectKey, Constants.SiteMigrationFailedEmailSubjectKeyValue, null);
                    }

                    // Enter values for SiteMigrationSuccessEmailSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationSuccessEmailSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationSuccessEmailSubjectKey, Constants.SiteMigrationSuccessEmailSubjectKeyValue, null);
                    }

                    // Enter values for SiteMigrationRequestEmailBodyKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationRequestEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationRequestEmailBodyKey, Constants.SiteMigrationRequestEmailBodyKeyValue, null);
                    }

                    // Enter values for SiteMigrationRequestEmailSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMigrationRequestEmailSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMigrationRequestEmailSubjectKey, Constants.SiteMigrationRequestEmailSubjectKeyValue, null);
                    }

                    // Enter values for SiteRequestEmailSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteRequestEmailSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteRequestEmailSubjectKey, Constants.SiteRequestEmailSubjectKeyValue, null);
                    }

                    // Enter values for SiteRequestEmailSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteApproveEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteApproveEmailBodyKey, Constants.SiteApproveEmailBodyKeyValue, null);
                    }

                    // Enter values for SiteRejectEmailSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteRejectEmailSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteRejectEmailSubjectKey, Constants.SiteRejectEmailSubjectKeyValue, null);
                    }

                    // Enter values for SiteRejectEmailBodyKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteRejectEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteRejectEmailBodyKey, Constants.SiteRejectEmailBodyKeyValue, null);
                    }

                    // Enter values for SiteMoreInfoRequiredSubjectKey configuration list
                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteMoreInfoRequiredSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteMoreInfoRequiredSubjectKey, Constants.SiteMoreInfoRequiredSubjectKeyValue, null);
                    }

                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteRequestFailedEmailBodyKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteRequestFailedEmailBodyKey, Constants.SiteRequestFailedEmailBodyKeyValue, null);
                    }

                    if (!CheckEntry(context, workFlowConfiguration, Constants.SiteRequestFailedEmailSubjectKey))
                    {
                        AddItem(workFlowConfiguration, Constants.SiteRequestFailedEmailSubjectKey, Constants.SiteRequestFailedEmailSubjectKeyValue, null);
                    }
                }
            }

            LogHelper.LogInformation("Completed WorkFlow Configuration.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="workFlowConfiguration">The work flow configuration.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="description">The description.</param>
        public static void AddItem(List workFlowConfiguration, string key, string value, string description)
        {
            LogHelper.LogInformation(string.Format("Started adding WorkFlow Configuration list item : {0} WorkFlow Configuration", key), LogEventID.InformationWrite);

            ListItemCreationInformation itemInfo = new ListItemCreationInformation();
            ListItem item = workFlowConfiguration.AddItem(itemInfo);

            // Add team Site default data
            item["Title"] = key;

            if (value != null)
            {
                item["Value"] = value;
            }

            if (description != null)
            {
                item["Description"] = description;
            }

            item.Update();
            workFlowConfiguration.Update();
            workFlowConfiguration.Context.ExecuteQuery();

            LogHelper.LogInformation(string.Format("Completed adding WorkFlow Configuration list item : {0} WorkFlow Configuration", key), LogEventID.InformationWrite);
        }

        /// <summary>
        /// Checks the entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="workFlowConfiguration">The work flow configuration.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Return true or false
        /// </returns>
        public static bool CheckEntry(ClientContext context, List workFlowConfiguration, string key)
        {
            bool entryPresent = false;

            if (workFlowConfiguration != null)
            {
                var query = new CamlQuery();
                query.ViewXml = string.Format(CultureInfo.InvariantCulture, CamlQueryHelper.GetGlobalConfigKeyCamlQuery, key);
                var itemCollection = workFlowConfiguration.GetItems(query);
                context.Load(itemCollection);
                context.ExecuteQuery();
                foreach (ListItem listItem in itemCollection)
                {
                    if (listItem["Title"] != null && listItem["Title"].ToString() == key)
                    {
                        entryPresent = true;
                        break;
                    }
                }
            }

            return entryPresent;
        }

        /// <summary>
        /// Configures the site provisioning lists access.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void ConfigureSiteProvisioningListsAccess(ClientContext context)
        {
            LogHelper.LogInformation("Started Assign Contribute Access ...", LogEventID.InformationWrite);
            context.Load(context.Site.RootWeb);
            context.ExecuteQuery();
            Web web = context.Site.RootWeb;

            User allUsersWindowsUser = web.EnsureUser(Constants.AllUsersWindows);
            context.Load(allUsersWindowsUser);
            context.ExecuteQuery();

            List siteRequest = web.Lists.GetByTitle(Constants.SiteRequestListName);
            List workflowHistoryListName = web.Lists.GetByTitle(Constants.WorkflowHistoryListName);
            List workflowTaskList = web.Lists.GetByTitle(Constants.WorkflowTaskListName);

            context.Load(siteRequest);
            context.Load(workflowHistoryListName);
            context.Load(workflowTaskList);
            context.ExecuteQuery();

            if (siteRequest != null)
            {
                // Add All Users to Site request list
                AssignContributeAccess(context, web, siteRequest, allUsersWindowsUser, RoleType.Contributor);
            }

            if (workflowHistoryListName != null)
            {
                // Add All Users to Work flow history list
                AssignContributeAccess(context, web, workflowHistoryListName, allUsersWindowsUser, RoleType.Contributor);
            }

            AddWorkFlowConfiguration(context, web);

            // Create custom group
            CreateCustomGroup(web);

            // Create custom permission role
            CreateCustomRole(web);

            // Add user to the group
            web.AddUserToGroup(Constants.JCIAddPermissionGroup, allUsersWindowsUser);

            if (workflowTaskList != null)
            {
                // Add custom group created above
                AssignCustomAccessToList(context, web, workflowHistoryListName);
            }

            LogHelper.LogInformation("Completed Assign Contribute Access.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Assigns the contribute access.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        /// <param name="user">The user.</param>
        /// <param name="roleType">Type of the role.</param>
        public static void AssignContributeAccess(ClientContext context, Web web, List list, User user, RoleType roleType)
        {
            try
            {
                LogHelper.LogInformation("Entering Method - AssignContributeAccess", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                list.BreakRoleInheritance(false, false);
                RoleDefinitionBindingCollection roleDefinitionCollection = new RoleDefinitionBindingCollection(context);
                RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(roleType);
                roleDefinitionCollection.Add(roleDefinition);
                list.RoleAssignments.Add(user, roleDefinitionCollection);
                list.Context.ExecuteQuery();
            }
            catch (Exception exception)
            {
                LogHelper.LogError(exception, LogEventID.ExceptionHandling);
            }
            finally
            {
                LogHelper.LogInformation("Exiting Method - AssignContributeAccess", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Assigns the custom access to list.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="web">The web.</param>
        /// <param name="list">The list.</param>
        public static void AssignCustomAccessToList(ClientContext ctx, Web web, List list)
        {
            try
            {
                LogHelper.LogInformation("Entering Method - SharePointHelper.AssignAccess", LogEventID.InformationWrite);

                ctx.Load(list, l => l.HasUniqueRoleAssignments);
                ctx.ExecuteQuery();

                if (!list.HasUniqueRoleAssignments)
                {
                    list.BreakRoleInheritance(true, false); // ensure we don't inherit permissions from parent
                }

                RoleDefinitionBindingCollection roleDefinitionCollection = new RoleDefinitionBindingCollection(ctx);
                RoleDefinition roleDefinition = web.RoleDefinitions.GetByName(Constants.JCIAddPermission);
                ctx.Load(roleDefinition);
                ctx.ExecuteQuery();

                Group jciGroup = web.SiteGroups.GetByName(Constants.JCIAddPermissionGroup);
                ctx.Load(jciGroup);
                ctx.ExecuteQuery();

                roleDefinitionCollection.Add(roleDefinition);
                list.RoleAssignments.Add(jciGroup, roleDefinitionCollection);
                list.Update();
                ctx.ExecuteQuery();
            }
            catch (Exception exception)
            {
                LogHelper.LogError(exception, LogEventID.ExceptionHandling);
            }
            finally
            {
                LogHelper.LogInformation("Exiting Method - SharePointHelper.AssignAccess", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Creates the custom group.
        /// </summary>
        /// <param name="web">The web.</param>
        public static void CreateCustomGroup(Web web)
        {
            if (!GroupExists(web, Constants.JCIAddPermissionGroup))
            {
                LogHelper.LogInformation("Started Create Custom Group ...", LogEventID.InformationWrite);

                //// Create custom group
                Group group = web.AddGroup(Constants.JCIAddPermissionGroup, null, true);
                User currentuser = web.CurrentUser;
                web.Context.Load(currentuser);
                web.Context.ExecuteQuery();

                group.Users.AddUser(currentuser);
                group.Update();
                web.Context.ExecuteQuery();
                LogHelper.LogInformation("Completed Create Custom Group ...", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Creates the custom role.
        /// </summary>
        /// <param name="web">The web.</param>
        public static void CreateCustomRole(Web web)
        {
            LogHelper.LogInformation("Started Create Custom Role ...", LogEventID.InformationWrite);

            if (!RoleExists(web, Constants.JCIAddPermission))
            {
                BasePermissions permissions = new BasePermissions();
                permissions.Set(PermissionKind.AddListItems);
                RoleDefinitionCreationInformation roleCreationInfo = new RoleDefinitionCreationInformation();
                roleCreationInfo.BasePermissions = permissions;
                roleCreationInfo.Name = Constants.JCIAddPermission;
                roleCreationInfo.Description = Constants.JCIAddPermissionDesc;
                RoleDefinition roleDefinition = web.RoleDefinitions.Add(roleCreationInfo);
                web.Context.ExecuteQuery();
                LogHelper.LogInformation("Completed Create Custom Role ...", LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Add script editor web part
        /// </summary>
        /// <param name="ctx">The SharePoint Context</param>
        public static void AddWebPartToDispForm(ClientContext ctx)
        {
            ctx.Load(ctx.Web);
            ctx.ExecuteQuery();           

            Microsoft.SharePoint.Client.File dispForm = ctx.Web.GetFileByServerRelativeUrl(GlobalData.DispFormPageRelativePath);
            ctx.Load(dispForm);
            ctx.ExecuteQuery();

            string zoneId = "Main";
            int zoneIndex = 0;

            string webpartXML = System.IO.File.ReadAllText(GlobalData.WebPartFile);

            LimitedWebPartManager wm = dispForm.GetLimitedWebPartManager(PersonalizationScope.Shared);
            ctx.Load(wm);
            ctx.Load(wm.WebParts);
            ctx.ExecuteQuery();

            LogHelper.LogInformation("Checking existing webparts and see if the webpart is already added");
            bool webpartPresent = false;
            foreach (var webpart in wm.WebParts)
            {
                ctx.Load(webpart);
                ctx.Load(webpart.WebPart);
                ctx.Load(webpart.WebPart.Properties);
                ctx.ExecuteQuery();
                
                LogHelper.LogInformation("Id: " + ((Guid)webpart.Id).ToString());
                if ((string)webpart.WebPart.Properties["Title"] == GlobalData.WebPartTitle)
                {
                    webpartPresent = true;
                    LogHelper.LogInformation("The webpart is already added to the page, skipping webpart addition");
                }
            }

            if (!webpartPresent)
            {
                try
                {
                    WebPartDefinition webPartDefinition = wm.ImportWebPart(webpartXML);
                    WebPartDefinition newWebPartDefinition = wm.AddWebPart(webPartDefinition.WebPart, zoneId, zoneIndex);
                    ctx.Load(dispForm);
                    ctx.ExecuteQuery();
                    dispForm.Publish("Published changes");

                    LogHelper.LogInformation("Webpart added to the page");
                }
                catch (Exception ex)
                {
                    LogHelper.LogInformation("Error while adding webpart:");
                    LogHelper.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Roles the exists.
        /// </summary>
        /// <param name="currentWeb">The current web.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>Return true or false</returns>
        private static bool RoleExists(Web currentWeb, string roleName)
        {
            RoleDefinitionCollection roles = currentWeb.RoleDefinitions;
            currentWeb.Context.Load(roles);
            currentWeb.Context.ExecuteQuery();
            if (roles == null)
            {
                return false;
            }

            return roles.Cast<RoleDefinition>().Any(role => role.Name == roleName);
        }

        /// <summary>
        /// Groups the exists.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>Return true or false</returns>
        private static bool GroupExists(Web web, string groupName)
        {
            var groups = web.SiteGroups;
            web.Context.Load(groups);
            web.Context.ExecuteQuery();
            if (groups == null)
            {
                return false;
            }

            return groups.Cast<Group>().Any(role => role.Title == groupName);
        }

        #region list
        /// <summary>
        /// Provisions the list instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="instance">The instance.</param>
        private static void ProvisionListInstance(ClientContext context, Web web, ListDefinition definition, ListInstanceTemplate instance)
        {
            int listTemplateType = -1;

            if (definition != null)
            {
                listTemplateType = GetListTemplateTypeByBaseType(definition.BaseType);
            }

            List newList = null;

            if (!web.ListExists(instance.Title))
            {
                if (instance.IsCustom && listTemplateType != -1 && definition != null)
                {
                    newList = web.CreateList(listTemplateType, instance.Title, instance.Description, definition.VersioningEnabled.ToBoolean(), definition.EnableMinorVersions.ToBoolean(), definition.EnableContentTypes.ToBoolean(), true, instance.OnQuickLaunch);
                }
                else
                {
                    newList = web.CreateList(instance.ListTemplateId, instance.Title, instance.EnableVersioning, false, true, true);
                }
            }
            else
            {
                newList = web.Lists.GetByTitle(instance.Title);
                context.Load(newList);
                context.ExecuteQuery();
            }

            if (newList != null && newList.ServerObjectIsNull != null && newList.ServerObjectIsNull != true)
            {
                if (definition != null)
                {
                    AddFieldsToList(definition.Fields, newList);

                    if (definition.ContentTypes != null)
                    {
                        web.RemoveAllContentTypesFromList(newList);
                        AddContentTypesToList(web, newList, definition.ContentTypes);
                    }

                    AddViewsToList(definition.Views, newList);

                    if (definition.Receivers != null && definition.Receivers.Count > 0)
                    {
                        AddListEventReceiver(newList, definition.Receivers);
                    }
                }
                else if (!instance.IsCustom && !string.IsNullOrEmpty(instance.ListTemplateId))
                {
                    if (instance.ContentTypeBinding != null)
                    {
                        if (instance.RemoveDefaultContentTypes)
                        {
                            web.RemoveAllContentTypesFromList(newList);
                        }

                        newList.AddContentTypeToListById(instance.ContentTypeBinding.ContentTypeId, instance.ContentTypeBinding.Default);
                    }
                }

                context.Load(newList, l => l.DefaultViewUrl);
                context.ExecuteQuery();
                LogHelper.LogInformation(string.Format("List Instance created {0}", instance.Title), LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format("Falied to create list {0}", instance.Title), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Gets the type of the list template type by base.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <returns>
        /// Return list template type
        /// </returns>
        private static int GetListTemplateTypeByBaseType(int baseType)
        {
            int listTemplateType;

            switch (baseType)
            {
                case 0:
                    listTemplateType = (int)ListTemplateType.GenericList;
                    break;
                case 1:
                    listTemplateType = (int)ListTemplateType.DocumentLibrary;
                    break;
                case 4:
                    listTemplateType = (int)ListTemplateType.Survey;
                    break;
                case 5:
                    listTemplateType = (int)ListTemplateType.IssueTracking;
                    break;
                default:
                    listTemplateType = (int)ListTemplateType.GenericList;
                    break;
            }

            return listTemplateType;
        }

        /// <summary>
        /// Adds the fields to list.
        /// </summary>
        /// <param name="listfields">The list fields.</param>
        /// <param name="newList">The new list.</param>
        private static void AddFieldsToList(ListFields listfields, List newList)
        {
            if (listfields != null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(CreateXML(listfields));
                XmlNodeList fields = xmlDoc.SelectNodes("//Field");

                if (fields != null)
                {
                    foreach (XmlNode field in fields)
                    {
                        newList.CreateField(field.OuterXml);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the XML.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// Return xml string
        /// </returns>
        private static string CreateXML(ListFields entity)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(entity.GetType());

            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, entity);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return System.Net.WebUtility.HtmlDecode(xmlDoc.InnerXml.Replace("<FieldBody>", string.Empty).Replace("</FieldBody>", string.Empty));
            }
        }

        /// <summary>
        /// Adds the views to list.
        /// </summary>
        /// <param name="views">The views.</param>
        /// <param name="newList">The new list.</param>
        private static void AddViewsToList(List<ListView> views, List newList)
        {
            if (views != null && views.Count > 0)
            {
                //// Create View to List
                foreach (var view in views)
                {
                    if (!view.Hidden.ToBoolean() || !view.ReadOnly.ToBoolean() || !string.IsNullOrEmpty(view.DisplayName))
                    {
                        newList.DeleteViewByName(view.DisplayName);

                        List<string> fieldArry = new List<string>();

                        foreach (var field in view.ViewFields)
                        {
                            fieldArry.Add(field.Name);
                        }

                        Microsoft.SharePoint.Client.ViewType type = (Microsoft.SharePoint.Client.ViewType)Enum.Parse(typeof(Microsoft.SharePoint.Client.ViewType), view.Type, true);
                        newList.CreateView(view.DisplayName, type, fieldArry.ToArray(), view.RowLimit, view.DefaultView.ToBoolean(), view.Query);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the content types to list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="contentTypes">The content types.</param>
        private static void AddContentTypesToList(Web web, List newList, ContentTypeDefinitions contentTypes)
        {
            if (contentTypes == null)
            {
                return;
            }

            //// Create Contenttype
            if (contentTypes.ContentTypeRef != null)
            {
                foreach (var contentTypeRef in contentTypes.ContentTypeRef)
                {
                    newList.AddContentTypeToListById(contentTypeRef.ID);
                }
            }

            if (contentTypes.ContentType != null)
            {
                ContentTypeDefinition contentType = contentTypes.ContentType;

                Microsoft.SharePoint.Client.ContentType siteContentType = web.CreateContentType(contentType.Name, string.Empty, contentType.ID, contentType.Group);

                //// Add contenttype to List
                Microsoft.SharePoint.Client.ContentType listContentType = newList.AddContentTypeToList(siteContentType);

                //// Add fieldRefs to ContentType
                List<ContentTypeFieldRef> cntFields = contentType.FieldRefs;
                foreach (var contentTypeFieldRef in cntFields)
                {
                    newList.AddListFieldToContentTypeById(listContentType.Id.StringValue, contentTypeFieldRef.ID, false, false);
                }
            }
        }

        /// <summary>
        /// Adds the list event receiver.
        /// </summary>
        /// <param name="newList">The new list.</param>
        /// <param name="listReceivers">The list receivers.</param>
        private static void AddListEventReceiver(List newList, List<ListReceiver> listReceivers)
        {
            if (listReceivers == null || listReceivers.Count == 0)
            {
                return;
            }

            foreach (ListReceiver listReceiver in listReceivers)
            {
                newList.AddRemoteEventReceiver(listReceiver.Name, listReceiver.Url, (EventReceiverType)Enum.Parse(typeof(EventReceiverType), listReceiver.Type, true), (EventReceiverSynchronization)Enum.Parse(typeof(EventReceiverSynchronization), listReceiver.Synchronization, true), listReceiver.SequenceNumber, true);
            }
        }
        #endregion

        /// <summary>
        /// Traversing through SiteMigrationRequest list to get the successfully migrated sites
        /// </summary>
        /// <param name="context">ClientContext instance</param>
        /// <param name="web">Web instance</param>
        /// <param name="siteMigrationRequestList">Site Migration Request list title</param>
        /// <returns>
        /// Returns site migration requests
        /// </returns>
        private static List<string> TraverseSiteMigrationList(ClientContext context, Web web, List siteMigrationRequestList)
        {
            LogHelper.LogInformation("Loading site migration request list items...", LogEventID.InformationWrite);

            List<string> siteMigrationSuccessItems = new List<string>();

            while (true)
            {
                CamlQuery queryToGetSiteMigrationSuccessItems = new CamlQuery
                {
                    ViewXml = MigrationConstants.SiteMigrationSuccessCamlQuery
                };
                ListItemCollection siteMigrationRequests = siteMigrationRequestList.GetItems(queryToGetSiteMigrationSuccessItems);
                web.Context.Load(siteMigrationRequests);
                web.Context.ExecuteQuery();

                // Item postion
                var itemPosition = siteMigrationRequests.ListItemCollectionPosition;

                if (siteMigrationRequests.Count > 0)
                {
                    foreach (ListItem siteMigrationRequest in siteMigrationRequests)
                    {
                        try
                        {
                            FieldUrlValue siteUrlField = (FieldUrlValue)siteMigrationRequest[MigrationConstants.SiteURLColumn];

                            if (!string.IsNullOrEmpty(siteUrlField.Url) && siteUrlField != null)
                            {
                                siteMigrationSuccessItems.Add(siteUrlField.Url);
                            }
                            else
                            {
                                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.ConsoleApp.ConsoleApp.TraverseSiteMigrationList() - Site url field is null or empty. Item: {0}.", siteUrlField), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging(ex, string.Format(CultureInfo.InvariantCulture, "JCI.CAM.ConsoleApp.ConsoleApp.TraverseSiteMigrationList() - Error occured while accesing the succesful migrated sites list items."));
                        }
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.ConsoleApp.ConsoleApp.TraverseSiteMigrationList() - There are no successfully migrated sites present in the {0} list.", GlobalData.SiteMigrationRequestListName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }

                // If item position is null then break the while loop.
                if (itemPosition == null)
                {
                    LogHelper.LogInformation("Loaded site migration request list items...", LogEventID.InformationWrite);
                    break;
                }
            }

            return siteMigrationSuccessItems;
        }

        /// <summary>
        /// Installs SandBox Solution
        /// </summary>
        /// <param name="succesfullyMigratedSites">Contains successfully migrated sites</param>
        private static void InstallSandBoxSolutionAndUploadTheme(List<string> succesfullyMigratedSites)
        {
            string solutionGalleryListTitle = MigrationConstants.SolutionGallery;
            string fileName = Path.GetFileNameWithoutExtension(GlobalData.SandboxedListDefinitionWSP);

            GetThemeDetailsFromXML();

            for (int count = 0; count < succesfullyMigratedSites.Count; count++)
            {
                try
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Creating client context for site {0}", succesfullyMigratedSites[count]), LogEventID.InformationWrite);

                    using (var context = new ClientContext(succesfullyMigratedSites[count]))
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

                        context.Load(context.Site.RootWeb);
                        context.ExecuteQuery();

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Client context created for site {0}", succesfullyMigratedSites[count]), LogEventID.InformationWrite);

                        // Root site
                        Web rootWeb = context.Site.RootWeb;
                        context.Load(rootWeb, w => w.Url);
                        ListCollection lists = rootWeb.Lists;
                        List solutionGalleryList = null;
                        IEnumerable<List> existingLists = rootWeb.Context.LoadQuery(lists.Where(list => list.Title == solutionGalleryListTitle));
                        rootWeb.Context.ExecuteQuery();
                        solutionGalleryList = existingLists.FirstOrDefault();

                        InstallDesignPackage(context, rootWeb, solutionGalleryList, solutionGalleryListTitle, fileName);

                        AddThemeToSites(context, rootWeb, succesfullyMigratedSites[count]);
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while activating {0} sandbox solution in site {1}.", GlobalData.SandboxedListDefinitionWSP, succesfullyMigratedSites[count]);
                    ExceptionLogging(ex, errorData);
                }
            }
        }

        /// <summary>
        /// Adds the theme to sites.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="siteURL">The site URL.</param>
        private static void AddThemeToSites(ClientContext context, Web rootWeb, string siteURL)
        {
            // Getting relative url
            Uri siteAbsoluteUrl = new Uri(siteURL);
            string webUrl = Uri.UnescapeDataString(siteAbsoluteUrl.AbsolutePath);

            // Getting the site
            Web web = context.Site.OpenWeb(webUrl);
            context.Load(web, w => w.Url, w => w.Title, w => w.ServerRelativeUrl);
            context.ExecuteQuery();

            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Client context created for site {0}", siteURL), LogEventID.InformationWrite);

            UploadAndSetTheme(rootWeb, web);

            WebCollection subSites = web.Webs;
            context.Load(subSites);
            context.ExecuteQuery();

            // Sub-Sites
            if (subSites.Count > 0)
            {
                foreach (Web subSite in subSites)
                {
                    try
                    {
                        AddThemeToSites(context, rootWeb, subSite.Url);
                    }
                    catch (Exception ex)
                    {
                        string errorData = string.Format(CultureInfo.InvariantCulture, "Error occurred while runing the site migration job for sub-site {0}).", siteURL);
                        ExceptionLogging(ex, errorData);
                    }
                }
            }
        }

        /// <summary>
        /// Uploads the and set theme.
        /// </summary>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="web">The web.</param>
        private static void UploadAndSetTheme(Web rootWeb, Web web)
        {
            try
            {
                // Check whether to run the operation or not.
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Running themes operation..."), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                if (!string.IsNullOrEmpty(themeInfo.ColorFile))
                {
                    rootWeb.UploadThemeFile(string.Format(CultureInfo.InvariantCulture, "{0}", Path.GetFullPath(themeInfo.ColorFile)));
                    LogHelper.LogInformation(string.Format("Completed Uploading Color File {0}.", themeInfo.ColorFile), LogEventID.InformationWrite);
                }

                rootWeb.CreateComposedLookByName(themeInfo.Name, themeInfo.ColorFile, themeInfo.FontFile, themeInfo.BackgroundFile, themeInfo.MasterPage);
                LogHelper.LogInformation(string.Format("Created Composed look {0}: for site {1}", themeInfo.Name, web.Url), LogEventID.InformationWrite);

                web.SetThemeBasedOnName(rootWeb, themeInfo.Name);
                LogHelper.LogInformation(string.Format("Seting theme {0}: for site {1}", themeInfo.Name, web.Url), LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while applying the themes for site {0}.", web.Url);
                ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Installing applying design package
        /// </summary>
        /// <param name="context">ClientContext object instance</param>
        /// <param name="rootWeb">Web object instance</param>
        /// <param name="solutionGalleryList">Solution Gallery List</param>
        /// <param name="solutionGalleryListTitle">Solution Gallery List Title</param>
        /// <param name="fileName">WSP name</param>
        private static void InstallDesignPackage(ClientContext context, Web rootWeb, List solutionGalleryList, string solutionGalleryListTitle, string fileName)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Checking whether {0} library exists or not...", solutionGalleryListTitle), LogEventID.InformationWrite);

                // Checking whether the list exists or not.
                if (solutionGalleryList != null)
                {
                    rootWeb.Context.Load(solutionGalleryList, s => s.RootFolder.ServerRelativeUrl);
                    rootWeb.Context.ExecuteQuery();

                    // Getting the root folder of site
                    var rootFolder = rootWeb.RootFolder;
                    rootWeb.Context.Load(rootFolder, f => f.ServerRelativeUrl);

                    // Query to check whether the sandbox solution exists or not.
                    CamlQuery sandboxSolutionQuery = new CamlQuery
                    {
                        ViewXml = string.Format(CultureInfo.InvariantCulture, MigrationConstants.SandboxSolutionQuery, GlobalData.SandboxedListDefinitionWSP)
                    };
                    ListItemCollection solutionGalleryListItems = solutionGalleryList.GetItems(sandboxSolutionQuery);
                    rootWeb.Context.Load(solutionGalleryListItems);
                    rootWeb.Context.ExecuteQuery();

                    if (solutionGalleryListItems.Count == 1)
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} library exists.", solutionGalleryListTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                        Guid guid = new Guid();

                        if (GlobalData.SandboxedListDefinitionWSPGUID != null)
                        {
                            guid = new Guid(GlobalData.SandboxedListDefinitionWSPGUID);
                        }

                        // Get the DesignPackageInfo
                        var packageInfo = new DesignPackageInfo()
                        {
                            PackageName = fileName,
                            PackageGuid = guid,
                            MajorVersion = GlobalData.SandboxedSolutionMajorVersion,
                            MinorVersion = GlobalData.SandboxedSolutionMinorVersion
                        };

                        // Install the solution from the file url
                        var filerelativeurl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", solutionGalleryList.RootFolder.ServerRelativeUrl, GlobalData.SandboxedListDefinitionWSP);

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Installing design package in site {0}...", rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        DesignPackage.Install(context, context.Site, packageInfo, filerelativeurl);
                        context.ExecuteQueryRetry();

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Applying design package in site {0}...", rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                        DesignPackage.Apply(context, context.Site, packageInfo);
                        context.ExecuteQueryRetry();

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Successfully activated {0} sandboxed solution in site {1}.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                    else
                    {
                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} sandboxed solution is not exists in site {1}.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not activaing sandboxed solution in site {0} because list {1} doesn't exists.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.ConsoleApp.ConsoleOperations.InstallDesignPackage() - Error occured while installing {0} sandbox solution design package in site {1}.", GlobalData.SandboxedListDefinitionWSP, rootWeb.Url);
                ExceptionLogging(ex, errorData);
            }
        }

        /// <summary>
        /// Exception logging
        /// </summary>
        /// <param name="ex">Exception Object</param>
        /// <param name="errorData">Error message</param>
        private static void ExceptionLogging(Exception ex, string errorData)
        {
            LogHelper.LogInformation(errorData, JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            LogHelper.LogError(ex, LogEventID.ExceptionHandling);
        }
    }
}
