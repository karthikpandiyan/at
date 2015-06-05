// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowExtensions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{   
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.WorkflowServices;

    /// <summary>
    /// Work Flow extension methods
    /// </summary>
    public static class WorkflowExtensions
    {
        /// <summary>
        /// Deploys the workflows
        /// </summary>
        /// <param name="web">web instance</param>
        /// <param name="clientContext">client context instance</param>
        /// <param name="workflowName">Name of the workflow.</param>
        /// <param name="filePath">The file path.</param>
        public static void DeployWorkflow(Web web, ClientRuntimeContext clientContext, string workflowName, string filePath)
        {
            if (web == null || clientContext == null || string.IsNullOrEmpty(filePath))
            {
                LogHelper.LogInformation("Invalid input data", LogEventID.InformationWrite);
                return;
            }

            LogHelper.LogInformation(string.Format("Deploying workflow {0}", workflowName), LogEventID.InformationWrite);
            try
            {
                var workflowServicesManager = new WorkflowServicesManager(clientContext, web);
                clientContext.Load(workflowServicesManager);
                clientContext.ExecuteQuery();

                if (workflowServicesManager.IsConnected)
                {
                    var workflowDeploymentService = workflowServicesManager.GetWorkflowDeploymentService();
                    var publishedWorkflowDefinitions = workflowDeploymentService.EnumerateDefinitions(true);
                    clientContext.Load(publishedWorkflowDefinitions);
                    clientContext.ExecuteQuery();

                    try
                    {
                        var existingWorkFlow = publishedWorkflowDefinitions.FirstOrDefault((wf) => (string.Compare(wf.DisplayName, workflowName, StringComparison.CurrentCultureIgnoreCase) == 0));
                        if (existingWorkFlow == null)
                        {
                            WorkflowDefinition workflowDefinition = new WorkflowDefinition(clientContext);
                            workflowDefinition.DisplayName = workflowName;
                            string contents = string.Empty;

                            using (FileStream fs = new FileStream(filePath, FileMode.Open))
                            {
                                var sr = new StreamReader(fs);
                                contents = sr.ReadToEnd();
                                workflowDefinition.Xaml = contents;
                                clientContext.Load(workflowDefinition);
                                var result = workflowDeploymentService.SaveDefinition(workflowDefinition);
                                clientContext.ExecuteQuery();

                                workflowDeploymentService.PublishDefinition(result.Value);
                                clientContext.ExecuteQuery();
                            }
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format("The WorkFlow definition already exists in the web. Name : {0} ", workflowName), LogEventID.InformationWrite);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogInformation(string.Format("Error deploying WorkFlow : {0}", workflowName), LogEventID.ExceptionHandling);
                        LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                        throw;                       
                    }
                }
                else
                {
                    LogHelper.LogInformation("Error deploying WorkFlow - WorkFlow Service Manager is not Connected", LogEventID.ExceptionHandling);
                }
            }
            catch (Exception exc)
            {
                LogHelper.LogInformation("WorkFlow Service Manager is not Connected", LogEventID.InformationWrite);
                LogHelper.LogError(exc, LogEventID.ExceptionHandling, null);
                throw;
            }
        }

        /// <summary>
        /// Returns a workflow subscription for a site.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="name">The name.</param>
        /// <returns>Workflow Subscription</returns>
        public static WorkflowSubscription GetWorkflowSubscription(this Web web, string name)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
            var subscriptions = subscriptionService.EnumerateSubscriptions();
            var subscriptionQuery = from sub in subscriptions where sub.Name == name select sub;
            var subscriptionsResults = web.Context.LoadQuery(subscriptionQuery);
            web.Context.ExecuteQueryRetry();
            var subscription = subscriptionsResults.FirstOrDefault();
            return subscription;
        }

        /// <summary>
        /// Returns a workflow subscription
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Workflow Subscription</returns>
        public static WorkflowSubscription GetWorkflowSubscription(this Web web, Guid id)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
            var subscription = subscriptionService.GetSubscription(id);
            web.Context.Load(subscription);
            web.Context.ExecuteQueryRetry();
            return subscription;
        }

        /// <summary>
        /// Returns a workflow subscription (associations) for a list
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="name">The name.</param>
        /// <returns>Workflow Subscription</returns>
        public static WorkflowSubscription GetWorkflowSubscription(this List list, string name)
        {
            var servicesManager = new WorkflowServicesManager(list.Context, list.ParentWeb);
            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
            var subscriptions = subscriptionService.EnumerateSubscriptionsByList(list.Id);
            var subscriptionQuery = from sub in subscriptions where sub.Name == name select sub;
            var subscriptionResults = list.Context.LoadQuery(subscriptionQuery);
            list.Context.ExecuteQueryRetry();
            var subscription = subscriptionResults.FirstOrDefault();
            return subscription;
        }

        /// <summary>
        /// Returns all workflow instances for a list item
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="listGuid">The list unique identifier.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        /// Workflow Instance Collection
        /// </returns>
        public static WorkflowInstanceCollection GetWorkflowInstances(this Web web, Guid listGuid, ListItem item)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var workflowInstanceService = servicesManager.GetWorkflowInstanceService();
            var instances = workflowInstanceService.EnumerateInstancesForListItem(listGuid, item.Id);
            web.Context.Load(instances);
            web.Context.ExecuteQueryRetry();
            return instances;
        }

        /// <summary>
        /// Adds a workflow subscription to a list
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="workflowDefinition">The workflow definition. <seealso><cref>WorkflowExtensions.GetWorkflowDefinition</cref></seealso></param>
        /// <param name="subscriptionName">The name of the workflow subscription to create</param>
        /// <param name="startManually">if True the workflow can be started manually</param>
        /// <param name="startOnCreate">if True the workflow will be started on item creation</param>
        /// <param name="startOnChange">if True the workflow will be started on item change</param>
        /// <param name="historyListName">the name of the history list. If not available it will be created</param>
        /// <param name="taskListName">the name of the task list. If not available it will be created</param>
        /// <param name="associationValues">The association values.</param>
        /// <returns>
        /// Workflow subscription Id
        /// </returns>
        public static Guid AddWorkflowSubscription(this List list, WorkflowDefinition workflowDefinition, string subscriptionName, bool startManually, bool startOnCreate, bool startOnChange, string historyListName, string taskListName, Dictionary<string, string> associationValues = null)
        {
            // parameter validation
            subscriptionName.ValidateNotNullOrEmpty("subscriptionName");
            historyListName.ValidateNotNullOrEmpty("historyListName");
            taskListName.ValidateNotNullOrEmpty("taskListName");

            var historyList = list.ParentWeb.GetListByTitle(historyListName);
            if (historyList == null)
            {
                historyList = list.ParentWeb.CreateList(ListTemplateType.WorkflowHistory, historyListName, false);
            }

            var taskList = list.ParentWeb.GetListByTitle(taskListName);
            if (taskList == null)
            {
                taskList = list.ParentWeb.CreateList(ListTemplateType.Tasks, taskListName, false);
            }

            var sub = new WorkflowSubscription(list.Context);

            sub.DefinitionId = workflowDefinition.Id;
            sub.Enabled = true;
            sub.Name = subscriptionName;

            var eventTypes = new List<string>();
            if (startManually)
            {
                eventTypes.Add("WorkflowStart");
            }

            if (startOnCreate)
            {
                eventTypes.Add("ItemAdded");
            }

            if (startOnChange)
            {
                eventTypes.Add("ItemUpdated");
            }

            sub.EventTypes = eventTypes;

            sub.SetProperty("HistoryListId", historyList.Id.ToString());
            sub.SetProperty("TaskListId", taskList.Id.ToString());

            if (associationValues != null)
            {
                foreach (var key in associationValues.Keys)
                {
                    sub.SetProperty(key, associationValues[key]);
                }
            }

            var servicesManager = new WorkflowServicesManager(list.Context, list.ParentWeb);

            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();

            var subscriptionResult = subscriptionService.PublishSubscriptionForList(sub, list.Id);

            list.Context.ExecuteQueryRetry();

            return subscriptionResult.Value;
        }

        /// <summary>
        /// Returns a workflow definition for a site
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="displayName">Display Name</param>
        /// <param name="publishedOnly">Is Published Only</param>
        /// <returns>
        /// Workflow Definition
        /// </returns>
        public static WorkflowDefinition GetWorkflowDefinition(this Web web, string displayName, bool publishedOnly = true)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var deploymentService = servicesManager.GetWorkflowDeploymentService();
            var definitions = deploymentService.EnumerateDefinitions(publishedOnly);
            var definitionQuery = from def in definitions where def.DisplayName == displayName select def;
            var definitionResults = web.Context.LoadQuery(definitionQuery);
            web.Context.ExecuteQueryRetry();
            var definition = definitionResults.FirstOrDefault();
            return definition;
        }
    }
}
