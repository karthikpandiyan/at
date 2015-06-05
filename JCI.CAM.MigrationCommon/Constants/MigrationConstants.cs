//-----------------------------------------------------------------------
// <copyright file= "MigrationConstants.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Migration.Common
{
    /// <summary>
    /// Migration code related constants
    /// </summary>
    public static class MigrationConstants
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
        /// Feature details xml node-list
        /// </summary>
        public const string FeatureDetailsXMLNodeList = "/Features/Feature";

        /// <summary>
        /// Feature definition xml Name attribute
        /// </summary>
        public const string FeatureDefinitionXMLTitleAttribute = "Name";

        /// <summary>
        /// Feature definition xml GUID attribute
        /// </summary>
        public const string FeatureDefinitionXMLGUIDAttribute = "GUID";

        /// <summary>
        /// Feature definition xml Scope attribute
        /// </summary>
        public const string FeatureDefinitionXMLSCOPEAttribute = "Scope";

        /// <summary>
        /// Site scope feature
        /// </summary>
        public const string SiteScopeFeature = "site";

        /// <summary>
        /// Web scope feature
        /// </summary>
        public const string WebScopeFeature = "web";

        /// <summary>
        /// List ID column
        /// </summary>
        public const string IDColumn = "ID";

        /// <summary>
        /// Site Title column
        /// </summary>
        public const string SiteTitleColumn = "Title";

        /// <summary>
        /// Site URL column
        /// </summary>
        public const string SiteURLColumn = "JCISiteUrl";

        /// <summary>
        /// Site migration status column
        /// </summary>
        public const string SiteMigrationStatusColumn = "JCISiteMigrationStatus";        

        /// <summary>
        /// Site migration error data column
        /// </summary>
        public const string SiteMigrationErrorDataColumn = "JCISiteMigrationLog";

        /// <summary>
        /// Site owners column
        /// </summary>
        public const string SiteOwnersColumn = "JCISiteOwners";

        /// <summary>
        /// Site schedule start date column
        /// </summary>
        public const string SiteScheduleStartDateColumn = "JCISiteScheduleStartDate";

        /// <summary>
        /// Duplicate content type error string
        /// </summary>
        public const string DuplicateContentTypeErrorString = "duplicate";        

        /// <summary>
        /// Query to fetch the site details to run the migration job
        /// </summary>
        public const string SiteMigrationRequestsJobCamlQuery = @"<View><Query><Where><And><Eq><FieldRef Name='" + SiteMigrationStatusColumn + "' /><Value Type='Choice'>Not Started</Value></Eq><Or><IsNull><FieldRef Name='" + SiteScheduleStartDateColumn + "' /></IsNull><Leq><FieldRef Name='" + SiteScheduleStartDateColumn + "' /><Value IncludeTimeValue='FALSE' Type='DateTime'>{0}</Value></Leq></Or></And></Where></Query><RowLimit>1000</RowLimit></View>";

        /// <summary>
        /// Query to fetch the site migration request item
        /// </summary>
        public const string SiteMigrationRequestItemByIdQuery = "<View><Query><Where><Eq><FieldRef Name='ID'/><Value Type='Integer'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>";

        /// <summary>
        /// Specifying the Feature deactivation operation whether to run or not
        /// </summary>
        public const string RunSiteMigrationJobOperation = "true";
        
        /// <summary>
        /// Specifying the sub site injection operation whether to run or not
        /// </summary>
        public const string RunSubSiteInjectionOperation = "true";
                        
        /// <summary>
        /// Custom action location
        /// </summary>
        public const string CustomActionLocation = "ScriptLink";

        /// <summary>
        /// Feature xml root element
        /// </summary>
        public const string FeatureXMLRootElement = "Features";

        /// <summary>
        /// List event receiver xml root element
        /// </summary>
        public const string ListEventReceiverXMLRootElement = "Receivers";

        /// <summary>
        /// Page layout xml root element
        /// </summary>
        public const string PageLayoutsXMLRootElement = "PageLayouts";

        /// <summary>
        /// Custom action xml root element
        /// </summary>
        public const string CustomActionsXMLRootElement = "CustomActions";

        /// <summary>
        /// Azure Connection Key
        /// </summary>
        public const string AzureConnectionKey = "ServiceBus.SiteMigrationConnection";

        /// <summary>
        /// Request Name Key
        /// </summary>
        public const string RequestNameKey = "ServiceBus.SiteMigrationRequestQueue";

        /// <summary>
        /// Solution Gallery library name
        /// </summary>
        public const string SolutionGallery = "Solution Gallery";

        /// <summary>
        /// Solution Gallery library name
        /// </summary>
        public const string MasterPageGallery = "Master Page Gallery";

        /// <summary>
        /// Publishing feature property
        /// </summary>
        public const string PublishingFeatureProperty = "__PublishingFeatureActivated";

        /// <summary>
        /// Pages library title
        /// </summary>
        public const string PagesLibraryTitle = "Pages";

        /// <summary>
        /// Header Footer injection custom action name
        /// </summary>
        public const string HeaderFooterCustomActionName = "JCI_CAM_HeaderFooterInjection";

        /// <summary>
        /// Test site banner injection custom action name
        /// </summary>
        public const string TestSiteBannerCustomActionName = "JCI_CAM_TestSiteBannerInjection";

        /// <summary>
        /// SPD settings injection custom action name
        /// </summary>
        public const string SPDSettingsCustomActionName = "JCI_CAM_SPDSettings";

        /// <summary>
        /// Sub site link injection custom action name
        /// </summary>
        public const string SubsiteLinkCustomActionName = "JCI_CAM_SubSite_OverrideCreateNewSite";

        /// <summary>
        /// Get pages from library query
        /// </summary>
        public const string GetPagesLibraryItemsQuery = "<View><Query><Where><IsNotNull><FieldRef Name='ID'/></IsNotNull></Where></Query><RowLimit>100</RowLimit></View>";

        /// <summary>
        /// The Site Migration Success Email Subject
        /// </summary>
        public const string SiteMigrationSuccessEmailSubjectKey = "SiteMigrationSuccessEmailSubjectKey";

        /// <summary>
        /// The Site Migration Success Email Body
        /// </summary>
        public const string SiteMigrationSuccessEmailBodyKey = "SiteMigrationSuccessEmailBodyKey";

        /// <summary>
        /// The Site Migration failed Email Subject
        /// </summary>
        public const string SiteMigrationFailedEmailSubjectKey = "SiteMigrationFailedEmailSubjectKey";

        /// <summary>
        /// The Site migration failed Email Body
        /// </summary>
        public const string SiteMigrationFailedEmailBodyKey = "SiteMigrationFailedEmailBodyKey";

        /// <summary>
        /// The Site migration pending Subject Body
        /// </summary>
        public const string SiteMigrationPendingEmailSubjectKey = "SiteMigrationPendingEmailSubjectKey";

        /// <summary>
        /// The Site migration pending Email Body
        /// </summary>
        public const string SiteMigrationPendingEmailBodyKey = "SiteMigrationPendingEmailBodyKey";

        /// <summary>
        /// Query to fetch the personal site details to run the personal site migration job
        /// </summary>
        public const string PersonalSiteMigrationJobCamlQuery = @"<View><Query><Where><Eq><FieldRef Name='" + SiteMigrationStatusColumn + "' /><Value Type='Choice'>Not Started</Value></Eq></Where></Query><RowLimit>500</RowLimit></View>";

        /// <summary>
        /// Query to check whether the personal site is already exists or not in the list.
        /// </summary>
        public const string PersonalSiteMigrationRequestCheckCamlQuery = @"<View><Query><Where><Contains><FieldRef Name='" + SiteURLColumn + "' /><Value Type='URL'>{0}</Value></Contains></Where></Query><RowLimit>1</RowLimit></View>";

        /// <summary>
        /// The personal space property
        /// </summary>
        public const string PersonalSpaceProperty = "personalspace";

        /// <summary>
        /// The account name property
        /// </summary>
        public const string AccountNameProperty = "accountname";

        /// <summary>
        /// The first name property
        /// </summary>
        public const string FirstNameProperty = "firstname";

        /// <summary>
        /// The last name property
        /// </summary>
        public const string LastNameProperty = "lastname";

        /// <summary>
        /// The work email property
        /// </summary>
        public const string WorkEmailProperty = "workemail";

        /// <summary>
        /// Gets the value to determine whether to send the email notification regarding site migration request or not.
        /// </summary>
        public const string SendEmailNotificationForSiteMigrationRequest = "true";

        /// <summary>
        /// Gets the value to determine whether to send the email notification regarding site migration status or not.
        /// </summary>
        public const string SendEmailNotificationForSiteMigrationStatus = "true";

        /// <summary>
        /// The master page URL
        /// </summary>
        public const string MasterPageUrl = "/_catalogs/masterpage/seattle.master";

        /// <summary>
        /// One drive transformation property bag key
        /// </summary>
        public const string JCICAMTransformationPropertyBagKey = "vNextTransformation";

        /// <summary>
        /// One drive transformation property bag value
        /// </summary>
        public const string JCICAMTransformationPropertyBagValue = "Yes";

        /// <summary>
        /// The publishing welcome page content type
        /// </summary>
        public const string PublishingWelcomePageContentType = "Welcome Page";

        /// <summary>
        /// The web part page title bar web part title
        /// </summary>
        public const string WebPartPageTitleBarWebpartTitle = "Web Part Page Title Bar";

        /// <summary>
        /// Query to fetch the successfully site migration sites
        /// </summary>
        public const string SiteMigrationSuccessCamlQuery = @"<View><Query><Where><Eq><FieldRef Name='" + SiteMigrationStatusColumn + "' /><Value Type='Choice'>Success</Value></Eq></Where></Query><RowLimit>1000</RowLimit></View>";

        /// <summary>
        /// Query to fetch the sandbox solution
        /// </summary>
        public const string SandboxSolutionQuery = @"<View><Query><Where><Eq><FieldRef Name='FileLeafRef' /><Value Type='File'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>";
    }
}
