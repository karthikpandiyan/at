//-----------------------------------------------------------------------
// <copyright file= "Constants.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.ConsoleApp
{
    /// <summary>
    /// Constants class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Dedicated environment
        /// </summary>
        public const string DedicatedEnvironment = "DEDICATED";

        /// <summary>
        /// Standard environment
        /// </summary>
        public const string StandardEnvironment = "STANDARD";

        /// <summary>
        /// OnPremise environment
        /// </summary>
        public const string OnPremiseEnvironment = "ONPREMISE";

        /// <summary>
        /// All users windows
        /// </summary>
        public const string AllUsersWindows = "c:0!.s|windows";

        /// <summary>
        /// Site request List Name
        /// </summary>
        public const string SiteRequestListName = "SiteRequest";

        /// <summary>
        /// Workflow History List Name
        /// </summary>
        public const string WorkflowHistoryListName = "JCIWorkflowHistoryList";

        /// <summary>
        /// Workflow Task List Name
        /// </summary>
        public const string WorkflowTaskListName = "JCIWorkflowTaskList";

        /// <summary>
        /// Workflow Custom List Name
        /// </summary>
        public const string WorkflowConfiguration = "WorkflowConfiguration";

        #region Workflow Configuration
        /// <summary>
        /// The due date count.
        /// </summary>
        public const string DueDateCountKey = "DueDateCountKey";

        /// <summary>
        /// The due date count value.
        /// </summary>
        public const string DueDateCountKeyValue = "4";

        /// <summary>
        /// The task assigned Email key.
        /// </summary>
        public const string TaskAssignedEmailKey = "TaskAssignedEmailKey";

        /// <summary>
        /// The task assigned Email value.
        /// </summary>
        public const string TaskAssignedEmailKeyValue = @"Action Required: New Site request for review#
                                                        Hi,

                                                        New Site has been requested for review. A task has been assigned against your name. 

                                                        To review the task, click on the following Task Url: 

                                                        Title : %Task: Title%

                                                        Task Url: <a href='%TaskSpecial: TaskUrl%' >%TaskSpecial: TaskUrl%</a>

                                                        Regards,
                                                        JCI Support Team";

        /// <summary>
        /// The task cancelled Email key.
        /// </summary>
        public const string TaskCanceledEmailKey = "TaskCanceledEmailKey";

        /// <summary>
        /// The task cancelled Email value.
        /// </summary>
        public const string TaskCanceledEmailKeyValue = @"Canceled: New Site request for review#
                                                        Hi,

                                                        Assigned task has been rejected by approver.

                                                        To review the task, click on the following Task Url: 

                                                        Title : %Task: Title%

                                                        Task Url: <a href='%TaskSpecial: TaskUrl%' >%TaskSpecial: TaskUrl%</a>

                                                        Thanks,
                                                        JCI Support Team";

        /// <summary>
        /// The task due date Email key.
        /// </summary>
        public const string TaskDueEmailKey = "TaskDueEmailKey";

        /// <summary>
        /// The task due date Email value.
        /// </summary>
        public const string TaskDueEmailKeyValue = @"Reminder: Site request for review#

                                                        Hi, 

                                                        A task assigned to you is due and requires you to take action. 

                                                        To review the task, click on the following Task Url: 

                                                        Title : %Task: Title% 

                                                        Task Url: <a href='%TaskSpecial: TaskUrl%' >%TaskSpecial: TaskUrl%</a> 

                                                        Regards, 
                                                        SharePoint CoE Administration Team";

        /// <summary>
        /// The Approve list query key.
        /// </summary>
        public const string ApproverListQueryKey = "ApproverListQueryKey";

        /// <summary>
        /// The Approve list query value.
        /// </summary>
        public const string ApproverListQueryKeyValue = "{0}_vti_bin/Listdata.svc/SiteApprover()?$filter=BusinessUnitValue eq '{1}' and RegionValue eq '{2}' and SiteTemplateValue eq '{3}'&$select=BusinessUnitAdminId";

        /// <summary>
        /// The site more information required email body key
        /// </summary>
        public const string SiteMoreInfoRequiredEmailBodyKey = "SiteMoreInfoRequiredEmailBodyKey";

        /// <summary>
        /// The site more information required email body key value
        /// </summary>
        public const string SiteMoreInfoRequiredEmailBodyKeyValue = @"Dear {0}, <br/><br/>The ""{1}"""
                                                                   + " that you requested is in process. However, there is additional information that is needed before the site can be approved and created.<br/><br/>"
                                                                   + " Please provide the following information, so that we can proceed with processing your request: "
                                                                   + " <br/>{2} <br/><br/>"
                                                                   + " To Provide additional comment please click on this url: <a href='{3}/_layouts/15/appredirect.aspx?instance_id=<#AppInstanceId#>&edit= {4}' >{3}/_layouts/15/appredirect.aspx?instance_id=<# AppInstanceId#>&edit={4}</a>.  " 
                                                                   + " <br/><br/>"
                                                                   + " To cancel this request, please click here: <a href='{3}/_layouts/15/appredirect.aspx?instance_id=<#AppInstanceId#>&requestId={4}' >{3}/_layouts/15/appredirect.aspx?instance_id=<#AppInstanceId#>&requestId={4}</a>."
                                                                   + " <br/><br/>Thank you, <br/>{5} Admin team";

        /// <summary>
        /// The site migration pending email body key
        /// </summary>
        public const string SiteMigrationPendingEmailBodyKey = "SiteMigrationPendingEmailBodyKey";

        /// <summary>
        /// The site migration pending email body key value
        /// </summary>
        public const string SiteMigrationPendingEmailBodyKeyValue = 
                                                                @"Dear, <br/><br/>The ""{0}"" has been queued to apply updates.<br/><br/>"
                                                                + "Please wait for the next update notification in this regard.<br/><br/>"
                                                                + "If you require further assistance please contact your local Help Desk to find additional information concerning features/functionality of the technology."
                                                                + "<br/><br/>Thank you, <br/>Admin team";

        /// <summary>
        /// The site migration pending email subject key
        /// </summary>
        public const string SiteMigrationPendingEmailSubjectKey = "SiteMigrationPendingEmailSubjectKey";

        /// <summary>
        /// The site migration pending email subject key value
        /// </summary>
        public const string SiteMigrationPendingEmailSubjectKeyValue = "Your site has been queued to apply updates";

        /// <summary>
        /// The site migration failed email body key
        /// </summary>
        public const string SiteMigrationFailedEmailBodyKey = "SiteMigrationFailedEmailBodyKey";

        /// <summary>
        /// The site migration failed email body key value
        /// </summary>
        public const string SiteMigrationFailedEmailBodyKeyValue = @"Dear, <br/><br/>Updates to your site ""{0}"" has been failed. Admin team is looking into those issues. <br/><br/> If you require further assistance, please contact your local Help Desk.<br/>Thank you, <br/>Admin team";

        /// <summary>
        /// The site migration success email body key
        /// </summary>
        public const string SiteMigrationSuccessEmailBodyKey = "SiteMigrationSuccessEmailBodyKey";

        /// <summary>
        /// The site migration success email body key value
        /// </summary>
        public const string SiteMigrationSuccessEmailBodyKeyValue = @"Dear, <br/><br/>The ""{0}"" has been update successfully. <br/><br/>Please proceed to the following URL to access the ""{0}"" site:<br/><br/><a href=""{1}"" > {0} </a><br/><br/>"
                                                                    + "If you require further assistance, please contact your local Help Desk."
                                                                    + "<br/><br/>Thank you, <br/>Admin team";

        /// <summary>
        /// The site migration failed email subject key
        /// </summary>
        public const string SiteMigrationFailedEmailSubjectKey = "SiteMigrationFailedEmailSubjectKey";

        /// <summary>
        /// The site migration failed email subject key value
        /// </summary>
        public const string SiteMigrationFailedEmailSubjectKeyValue = "Failed to update your site. Admin team is working on it.";

        /// <summary>
        /// The site migration success email subject key
        /// </summary>
        public const string SiteMigrationSuccessEmailSubjectKey = "SiteMigrationSuccessEmailSubjectKey";

        /// <summary>
        /// The site migration success email subject key value
        /// </summary>
        public const string SiteMigrationSuccessEmailSubjectKeyValue = "You site has been updated Successfully.";

        /// <summary>
        /// The site migration request email body key
        /// </summary>
        public const string SiteMigrationRequestEmailBodyKey = "SiteMigrationRequestEmailBodyKey";

        /// <summary>
        /// The site migration request email body key value
        /// </summary>
        public const string SiteMigrationRequestEmailBodyKeyValue = @"Dear, <br/><br/>The ""{0}"" is being updated. <br/><br/>Please wait for the next update notification in this regard.<br/><br/><a href='{1}' > {0} </a><br/><br/>If you require further information, please contact your local Help Desk.<br/><br/>Thank you, <br/>Admin team";

        /// <summary>
        /// The site migration request email subject key
        /// </summary>
        public const string SiteMigrationRequestEmailSubjectKey = "SiteMigrationRequestEmailSubjectKey";

        /// <summary>
        /// The site migration request email subject key value
        /// </summary>
        public const string SiteMigrationRequestEmailSubjectKeyValue = "Making planned changes in your site.";

        /// <summary>
        /// The site request email subject key
        /// </summary>
        public const string SiteRequestEmailSubjectKey = "SiteRequestEmailSubjectKey";

        /// <summary>
        /// The site request email subject key value
        /// </summary>
        public const string SiteRequestEmailSubjectKeyValue = "Site created successfully";

        /// <summary>
        /// The site approve email body key
        /// </summary>
        public const string SiteApproveEmailBodyKey = "SiteApproveEmailBodyKey";

        /// <summary>
        /// The site approve email body key value
        /// </summary>
        public const string SiteApproveEmailBodyKeyValue = @"Dear {0}, <br/><br/>The ""{1}"" that you requested has been approved and created. <br/><br/>Please proceed to the following URL to access the ""{2}"" site:<br/><br/><a href='{3}' > {3} </a><br/><br/>"
                                                        + " Training materials, documentation, discussion forums and other helpful information is available on the  <a href='https://my.jci.com/sites/O365Support' target=\"_blank\">O365 Support Community</a> to help you get started with your new site."
                                                        + "<br/><br/>"
                                                        + "If you require further assistance please contact your local Help Desk to find additional information concerning features/functionality of the technology."
                                                        + "<br/><br/>Thank you, <br/>{4} Admin team";

        /// <summary>
        /// The site request failed email subject key
        /// </summary>
        public const string SiteRequestFailedEmailSubjectKey = "SiteRequestFailedEmailSubjectKey";

        /// <summary>
        /// The site request failed email subject key value
        /// </summary>
        public const string SiteRequestFailedEmailSubjectKeyValue = "Site creation failure notice";

        /// <summary>
        /// The site request failed email body key
        /// </summary>
        public const string SiteRequestFailedEmailBodyKey = "SiteRequestFailedEmailBodyKey";

        /// <summary>
        /// The site request failed email body key value
        /// </summary>
        public const string SiteRequestFailedEmailBodyKeyValue = @"Dear {0} Admin,"
+ "<br/><br/>The site request for <a href='{1}' > {2} </a>, has been unsuccessful with the following reason :<br/><br/>{4}<br/><br/>"
+ "Please contact your local Help Desk for further help with the following troubleshooting details."
+ "<br/><br/>Site Request Id:{3}"
+ "<br/><br/>Exception Message:{4}"
+ "<br/>Exception Trace:{5}"
+ "<br/><br/>Thank you, <br/>{0} Admin team";

        /// <summary>
        /// The site reject email subject key
        /// </summary>
        public const string SiteRejectEmailSubjectKey = "SiteRejectEmailSubjectKey";

        /// <summary>
        /// The site reject email subject key value
        /// </summary>
        public const string SiteRejectEmailSubjectKeyValue = "Site Request denied";

        /// <summary>
        /// The site reject email body key
        /// </summary>
        public const string SiteRejectEmailBodyKey = "SiteRejectEmailBodyKey";

        /// <summary>
        /// The site reject email body key value
        /// </summary>
        public const string SiteRejectEmailBodyKeyValue = @"Dear {0}, <br/><br/>The ""{1}"" that you requested, ""{3}"" site, has been denied because {2}. <br/><br/>If you require further assistance please contact your local Help Desk to find additional information concerning features/functionality of the technology.<br/><br/>Thank you, <br/>{4} Admin team";

        /// <summary>
        /// The site more information required subject key
        /// </summary>
        public const string SiteMoreInfoRequiredSubjectKey = "SiteMoreInfoRequiredSubjectKey";

        /// <summary>
        /// The site more information required subject key value
        /// </summary>
        public const string SiteMoreInfoRequiredSubjectKeyValue = "More info requested by BU admin from site requestor";
        #endregion

        #region Custom Permission

        /// <summary>
        /// JCI Custom Permission
        /// </summary>
        public const string JCIAddPermission = "JCIAddPermissions";

        /// <summary>
        /// JCI Custom Permission description
        /// </summary>
        public const string JCIAddPermissionDesc = "JCI Custom permission level to add an item";

        /// <summary>
        /// JCI Custom Group
        /// </summary>
        public const string JCIAddPermissionGroup = "JCIAddPermissionsGroup";

        #endregion
    }
}