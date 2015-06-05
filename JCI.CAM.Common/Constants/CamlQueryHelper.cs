//-----------------------------------------------------------------------
// <copyright file="CamlQueryHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common
{
    /// <summary>
    /// Query helper
    /// </summary>
    public static class CamlQueryHelper
    {
        /// <summary>
        /// Text type column in Where Clause,where {0} stands for column name and {1} stands for column value
        /// </summary>
        public const string TextTypeColumnInWhereClause = @"<Where>
                                                             <Eq>
                                                                <FieldRef Name='{0}'/>
                                                                <Value Type='Text'>{1}</Value>
                                                              </Eq>
                                                           </Where>";

        /// <summary>
        /// The get site requests CAML query if the Site Request is approved
        /// </summary>
        public const string GetSiteRequestsCamlQuery = "<Where><And><Eq><FieldRef Name='WorkflowStatus' /><Value Type='Choice'>Approved</Value></Eq><Eq><FieldRef Name='RequestStatus' /><Value Type='Choice'>New</Value></Eq></And></Where>";

        /// <summary>
        /// The get BU Admin CAML query of the site request
        /// </summary>
        public const string GetBuAdminQuery = "<Where><And><Eq><FieldRef Name='Region1' /><Value Type='Choice'>{0}</Value></Eq><Eq><FieldRef Name='BusinessUnit1' /><Value Type='Choice'>{1}</Value></Eq></And></Where>";

        /// <summary>
        /// The get features CAML query of the site request
        /// </summary>
        public const string GetFeaturesCamlQuery = "<Where><Eq><FieldRef Name='SiteTemplateId' /><Value Type='Text'>{0}</Value></Eq></Where>";

        /// <summary>
        /// The get features CAML query which returns the Configuration Keys
        /// </summary>
        public const string GetGlobalConfigKeyCamlQuery = "<View><Query<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>";

        /// <summary>
        /// The get Site Url CAML query which return the site url
        /// </summary>
        public const string GetSiteUrlCamlQuery = "<Where><And><Contains><FieldRef Name='SiteUrl' /><Value Type='URL'>{0}</Value></Contains><Neq><FieldRef Name='WorkflowStatus' /><Value Type='Choice'>Rejected</Value></Neq></And></Where>";

        /// <summary>
        /// Get Notification Sites which returns all pending items of a site request
        /// </summary>
        public const string GetNotificationSiteQuery = "<View><Query><Where><And><Eq><FieldRef Name='RequestStatus' /><Value Type='Choice'>Pending</Value></Eq><Or><Eq><FieldRef Name='WorkflowStatus' /><Value Type='Choice'>Rejected</Value></Eq><Eq><FieldRef Name='WorkflowStatus' /><Value Type='Choice'>MoreInfoRequired</Value></Eq></Or></And></Where></Query><RowLimit>1000</RowLimit></View>";

        /// <summary>
        /// Get expired site requests
        /// </summary>
        public const string GetInformationWaitingRequests = "<View><Query><Where><And><Eq><FieldRef Name='RequestStatus' /><Value Type='Choice'>New</Value></Eq><Eq><FieldRef Name='WorkflowStatus' /><Value Type='Choice'>MoreInfoRequired</Value></Eq></And></Where></Query><RowLimit>1000</RowLimit></View>";

        /// <summary>
        /// Get Pending Sites which returns all pending site requests
        /// </summary>
        public const string GetPendingSiteQuery = "<View><Query><Where><And><Eq><FieldRef Name='RequestStatus' /><Value Type='Choice'>New</Value></Eq><Eq><FieldRef Name='WorkflowStatus' /><Value Type='Choice'>Pending</Value></Eq></And></Where></Query><RowLimit>1000</RowLimit></View>";

        /// <summary>
        /// Use to return the Workflow Task List Query 
        /// </summary>
        public const string GetSiteRequestTaskListQuery = "<View><Query><Where><Eq><FieldRef Name='JCITaskOutcome' /><Value Type='OutcomeChoice'>{0}</Value></Eq></Where></Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy><RowLimit>1000</RowLimit></View>";

        /// <summary>
        /// Use to return the Community Member User Query
        /// </summary>
        public const string CommunityMemberUserQuery = "<Where><Eq><FieldRef Name='{0}' LookupId='TRUE'/><Value Type='Lookup'>{1}</Value></Eq></Where>";

        /// <summary>
        /// Use to return the TaxonomyHiddenList
        /// </summary>
        public const string GetTermHiddenIdQuery = "<Where><Eq><FieldRef Name='IdForTerm' /><Value Type='Text'>{0}</Value></Eq></Where>";

        /// <summary>
        /// Use to return the TaxonomyHiddenList
        /// </summary>
        public const string GetTermHiddenTitleQuery = "<View><Query><Where><Eq><FieldRef Name='Title'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>";

        /// <summary>
        /// Use to return the TeamAnnouncementsListName Query
        /// </summary>
        public const string TeamAnnouncementsListNameQuery =
            "<Where><Or><IsNull><FieldRef Name='Expires' /></IsNull><Geq><FieldRef Name='Expires' /><Value Type='DateTime'><Today /></Value></Geq></Or></Where><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy><RowLimit Paged='TRUE'>5</RowLimit>";

        /// <summary>
        /// use to return the Team Project Assignments Query
        /// </summary>
        public const string TeamProjectAssignmentsListNameQuery =
            "<OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy><RowLimit Paged='TRUE'>5</RowLimit>";

        /// <summary>
        /// use to return the ProjectAnnouncementsListName Query
        /// </summary>
        public const string ProjectAnnouncementsListNameQuery =
            "<Where><Or><IsNull><FieldRef Name='Expires' /></IsNull><Geq><FieldRef Name='Expires' /><Value Type='DateTime'><Today /></Value></Geq></Or></Where><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy><RowLimit Paged='TRUE'>5</RowLimit>";

        /// <summary>
        /// Use to return the ProjectLinksListName Query
        /// </summary>
        public const string ProjectLinksListNameQuery =
            "<Where><Gt><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Gt></Where><OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy><RowLimit Paged='TRUE'>5</RowLimit>";

        /// <summary>
        /// Use to return the CommunityLinksListName Query
        /// </summary>
        public const string CommunityLinksListNameQuery =
            "<Where><Gt><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Gt></Where><OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy><RowLimit Paged='TRUE'>5</RowLimit>";

        /// <summary>
        /// Use to return the Documents Query
        /// </summary>
        public const string DocumentsQuery =
        "<Where><Gt><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Gt></Where><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy><RowLimit Paged='TRUE'>5</RowLimit>";

        /// <summary>
        /// Use to return the CommunityAnnouncementsListName Query
        /// </summary>
        public const string CommunityAnnouncementsListNameQuery =
            "<Where><Or><IsNull><FieldRef Name='Expires' /></IsNull><Geq><FieldRef Name='Expires' /><Value Type='DateTime'><Today /></Value></Geq></Or></Where><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy><RowLimit Paged='TRUE'>5</RowLimit>";

        /// <summary>
        /// Use to return all the items in the list
        /// </summary>
        public const string AllListItems = @"<Where><IsNotNull><FieldRef Name='ID' /></IsNotNull></Where>";
    }
}
