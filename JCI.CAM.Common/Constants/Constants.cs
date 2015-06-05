//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common
{
    /// <summary>
    /// Site Provisioning Tool Constants Class
    /// </summary>
    public static class Constants
    {
        #region Site Provision Timer Job

        /// <summary>
        /// The site provision tool site url.
        /// </summary>
        public const string SiteProvisionToolSiteUrlKey = "SiteProvisionToolSiteUrlKey";

        /// <summary>
        /// The web app url.
        /// </summary>
        public const string WebAppUrlKey = "SiteProvisionToolWepAppUrlKey";

        /// <summary>
        /// The web app url.
        /// </summary>
        public const string WebApplicationURL = "WebApplication";

        /// <summary>
        /// The site request list name
        /// </summary>
        public const string SiteRequestListName = "SiteRequest";

        /// <summary>
        /// The Approver list name
        /// </summary>
        public const string ApproverListName = "SiteApprover";

        /// <summary>
        /// The Business Unit Admin field
        /// </summary>
        public const string BusinessUnitAdminField = "BusinessUnitAdmin";

        /// <summary>
        /// The business unit admin email field
        /// </summary>
        public const string BusinessUnitAdminEmailField = "BusinessUnitAdminEmail";

        /// <summary>
        /// The site configuration list name
        /// </summary>
        public const string SiteConfigurationListName = "SiteFeaturesConfiguration";

        /// <summary>
        /// The value of site template with Hash
        /// </summary>
        public const string SiteTemplateIdWithHashFieldName = "SiteTemplateIdWithHash";

        /// <summary>
        /// The site provision job title
        /// </summary>
        public const string SiteProvisionJobTitle = "Site Provisioning Job";

        /// <summary>
        /// The site provision job description
        /// </summary>
        public const string SiteProvisionJobDescription = "Job to provision collaboration sites with JCI branding. This job will also send email for rejected and more information required request";

        /// <summary>
        /// The site provision request raised message
        /// </summary>
        public const string RequestRaised = "Site request already raised.";

        /// <summary>
        /// The site provision request raised message color attribute
        /// </summary>
        public const string Color = "color";

        /// <summary>
        /// The site provision request raised message in red
        /// </summary>
        public const string Red = "red";

        /// <summary>
        /// The ListHybridMetadataProvider
        /// </summary>
        public const string ListItemSiteMetadataProvider = "ListItemSiteMetadataProvider";

        /// <summary>
        /// The List Meta data
        /// </summary>
        public const string ListMetaData = "List";

        /// <summary>
        /// The sites managed path
        /// </summary>
        public const string SitesManagedPath = "sites/";

        /// <summary>
        /// The confidentiality level  - Public
        /// </summary>
        public const string ConfidentialityLevelPublic = "Public";

        /// <summary>
        /// The confidentiality level  - Internal
        /// </summary>
        public const string ConfidentialityLevelInternal = "Internal";

        /// <summary>
        /// The confidentiality level  - Confidential
        /// </summary>
        public const string ConfidentialityLevelConfidential = "Confidential";

        /// <summary>
        /// The confidentiality level  - Internal
        /// </summary>
        public const string ConfidentialityLevelRestricted = "Restricted";

        /// <summary>
        /// The everyone
        /// </summary>
        public const string EveryoneUsers = "Everyone";

        /// <summary>
        /// The region site property
        /// </summary>
        public const string RegionSiteProperty = "Region";

        /// <summary>
        /// The Cost Center property
        /// </summary>
        public const string CostCenterProperty = "CostCenter";

        /// <summary>
        /// The Project Start Date property
        /// </summary>
        public const string ProjectStartDate = "ProjectStartDate";

        /// <summary>
        /// The Project End date property
        /// </summary>
        public const string AnticipatedProjectEndDate = "AnticipatedProjectEndDate";

        /// <summary>
        /// The business unit site property
        /// </summary>
        public const string BusinessUnitSiteProperty = "BusinessUnit";

        /// <summary>
        /// The business opportunity site property
        /// </summary>
        public const string BusinessOpportunitySiteProperty = "BusinessOpportunity";

        /// <summary>
        /// The initial site type site property
        /// </summary>
        public const string InitialSiteTypeSiteProperty = "InitialSiteType";

        /// <summary>
        /// The country site property
        /// </summary>
        public const string CountrySiteProperty = "Country";

        /// <summary>
        /// The language site property
        /// </summary>
        public const string LanguageSiteProperty = "SiteLanguage";

        /// <summary>
        /// The visitors group name
        /// </summary>
        public const string VisitorsGroupName = "Visitors";

        /// <summary>
        /// The owners group name
        /// </summary>
        public const string OwnersGroupName = "Owners";

        /// <summary>
        /// The members group name
        /// </summary>
        public const string MembersGroupName = "Members";

        /// <summary>
        /// The approvers group name
        /// </summary>
        public const string ApproversGroupName = "Approvers";

        /// <summary>
        /// The designers group name
        /// </summary>
        public const string DesignersGroupName = "Designers";

        /// <summary>
        /// The moderators group name
        /// </summary>
        public const string ModeratorsGroupName = "Moderators";

        /// <summary>
        /// The publishing feature GUID
        /// </summary>
        public const string PublishingFeatureGuid = "f6924d36-2fa8-4f0b-b16d-06b7250180fa";

        /// <summary>
        /// The Web Level publishing feature GUID
        /// </summary>
        public const string PublishingWebFeatureGuid = "94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb";

        /// <summary>
        /// Community members
        /// </summary>
        public const string CommunityMembers = "Community Members";

        /// <summary>
        /// The request status success
        /// </summary>
        public const string RequestStatusSuccess = "Successful";

        /// <summary>
        /// The request status exception
        /// </summary>
        public const string RequestStatusException = "Exception";

        /// <summary>
        /// The request status exception
        /// </summary>
        public const string AlreadyActivated = "already activated";

        /// <summary>
        /// The constant which gets added when the automatic groups are created
        /// </summary>
        public const string AutomaticNumberConstant = "1";

        /// <summary>
        /// Request Status Rejected
        /// </summary>
        public const string RequestStatusRejected = "Rejected";

        /// <summary>
        /// The community site type public
        /// </summary>
        public const string CommunitySiteTypePublic = "Public";

        /// <summary>
        /// The community site type private
        /// </summary>
        public const string CommunitySiteTypePrivate = "Private";

        /// <summary>
        /// The community site type secured
        /// </summary>
        public const string CommunitySiteTypeSecured = "Secured";

        /// <summary>
        /// The community site auto approval property
        /// </summary>
        public const string CommunitySiteEnableAutoApproval = "vti_CommunityEnableAutoApproval";

        /// <summary>
        /// The batch format
        /// </summary>
        public const string BatchFormat =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ows:Batch OnError=\"Return\">{0}</ows:Batch>";

        /// <summary>
        /// The batch format
        /// </summary>
        public const string MethodFormat =
            "<Method ID=\"{0}\"><SetList>{1}</SetList><SetVar Name=\"Cmd\">Save</SetVar><SetVar Name=\"ID\">New</SetVar><SetVar Name=\"urn:schemas-microsoft-com:office:office#Member\">{2}</SetVar></Method>";

        /// <summary>
        /// This is the user id of the user who would be used as the primary site collection admin, for the site collection creation API
        /// This user should not be same as a Business Unit Approver.
        /// </summary>
        public const string DummySiteCollectionAdminUserGlobalID = "DummySiteCollectionAdminUserGlobalID";
        #endregion Site Provision Timer Job

        #region View State Fields

        /// <summary>
        /// Primary View State
        /// </summary>
        //// public const string PrimaryViewState = "Primary";

        /// <summary>
        /// Time Zone View State
        /// </summary>
        public const string TimeZoneProfileField = "TimeZone";

        /// <summary>
        /// Secondary View State
        /// </summary>
        //// public const string SecondaryViewState = "Secondary";

        /// <summary>
        /// Tertiary View State
        /// </summary>
        //// public const string TertiaryViewState = "Ternary";

        /// <summary>
        /// Moderator View State
        /// </summary>
        //// public const string ModeratorViewState = "Moderator";

        /// <summary>
        /// Manager View State
        /// </summary>
        public const string ManagerViewState = "Manager";

        /// <summary>
        /// Initial Members to Provision View State
        /// </summary>
        //// public const string InitialMemberstoProvisionViewState = "InitialMemberstoProvision";
        #endregion

        #region Button Text
        /// <summary>
        /// Button start step next control
        /// </summary>
        public const string StartStepNext = "btnStartStepNext";

        /// <summary>
        /// JCI site provisioning resource file
        /// </summary>
        public const string JciSiteProvisioning = "JciSiteProvisioning";

        /// <summary>
        /// Managed metadata binding error
        /// </summary>
        public const string ErrorBindingManagedMetadataText = "ErrorBindingManagedMetadata_Text";

        /// <summary>
        /// Site feature configuration List does not contain information
        /// </summary>
        public const string ErrorSiteFeatureConfigurationListText = "ErrorSiteFeatureConfigurationList_Text";

        /// <summary>
        /// User properties not defined
        /// </summary>
        public const string ErrorBindingUserProfileText = "ErrorBindingUserProfile_Text";

        /// <summary>
        /// User properties not defined for System Account
        /// </summary>
        public const string ErrorSystemAccountUserProfileText = "ErrorSystemAccountUserProfile_Text";

        /// <summary>
        /// Site Request Web part Next Button resource text
        /// </summary>
        public const string SiteRequestWebPartNextButtonText = "SiteRequestWebPart_NextButton_Text";

        /// <summary>
        /// Site Request Web part Previous Button resource text
        /// </summary>
        public const string SiteRequestWebPartPreviousButtonText = "SiteRequestWebPart_PreviousButton_Text";

        /// <summary>
        /// Site Request Web part Finish Button resource text
        /// </summary>
        public const string SiteRequestWebPartFinishButtonText = "SiteRequestWebPart_FinishButton_Text";

        /// <summary>
        /// Site Request Web part Update Button resource text
        /// </summary>
        public const string SiteRequestWebPartUpdateButtonText = "SiteRequestWebPart_UpdateButton_Text";

        /// <summary>
        /// Reject Web part cancel button
        /// </summary>
        public const string RejectLabelCancelMessage = "RejectRequestWebPart_Cancel_Text";

        /// <summary>
        /// Reject Web part cancel button
        /// </summary>
        public const string RejectLabelNoticeMessage = "RejectRequestWebPart_Notice_Text";

        /// <summary>
        /// Reject Web part cancel button
        /// </summary>
        public const string EditLabelSuccessMessage = "EditRequestWebPart_Success_Text";

        /// <summary>
        /// Site Request Web part Step next button
        /// </summary>
        public const string StepNext = "btnStepNext";

        /// <summary>
        /// Site Request Web part Step next button
        /// </summary>
        public const string StepPrevious = "btnStepPrevious";
        #endregion

        #region Site Request List Fields
        /// <summary>
        /// The Site request list path
        /// </summary>
        public const string SiteRequestListPath = "/Lists/SiteRequest/AllItems.aspx";

        /// <summary>
        /// The title field
        /// </summary>
        public const string TitleField = "Title";

        /// <summary>
        /// The site type field
        /// </summary>
        public const string SiteTypeField = "SiteType";

        /// <summary>
        /// The site Region field
        /// </summary>
        public const string RegionField = "Region";

        /// <summary>
        /// The site BusinessUnit field
        /// </summary>
        public const string BusinessUnitField = "BusinessUnit";

        /// <summary>
        /// The site description field
        /// </summary>
        public const string SiteDescriptionField = "SiteDescription";

        /// <summary>
        /// The site URL field
        /// </summary>
        public const string SiteUrlField = "SiteUrl";

        /// <summary>
        /// The primary owner field
        /// </summary>
        public const string PrimaryOwnerField = "PrimarySiteOwner";

        /// <summary>
        /// The secondary owner field
        /// </summary>
        public const string SecondaryOwnerField = "SecondarySiteOwner";

        /// <summary>
        /// The tertiary owner field
        /// </summary>
        public const string TertiaryOwnerField = "TertiarySiteOwner";

        /// <summary>
        /// The confidentiality level field
        /// </summary>
        public const string ConfidentialityLevelField = "ConfidentialityLevel";

        /// <summary>
        /// The site language field
        /// </summary>
        public const string SiteLanguageField = "SiteLanguage";

        /// <summary>
        /// The site additional field
        /// </summary>
        public const string AdditionalInfoField = "AdditionalInfo";

        /// <summary>
        /// The site additional field
        /// </summary>
        public const string ClarityProjectNumberField = "ClarityProjectNumber";

        /// <summary>
        /// The site InitialMembersToProvision field
        /// </summary>
        public const string InitialMembersToProvisionField = "InitialMembersToProvision";

        /// <summary>
        /// The site IsTestSite field
        /// </summary>
        public const string IsTestSite = "IsTestSite";

        /// <summary>
        /// The community site type field
        /// </summary>
        public const string CommunitySiteTypeField = "CommunitySiteType";

        /// <summary>
        /// The site contact SitePurpose field
        /// </summary>
        public const string SitePurposeField = "SitePurpose";

        /// <summary>
        /// The site customers field
        /// </summary>
        public const string SiteCustomersField = "JCICustomer";

        /// <summary>
        /// The site contact WorkflowStatus field
        /// </summary>
        public const string WorkflowStatusField = "WorkflowStatus";

        /// <summary>
        /// The site contact Language field
        /// </summary>
        public const string LanguageField = "Language";

        /// <summary>
        /// The site contact TimeZone field
        /// </summary>
        public const string TimeZoneField = "SiteTimeZone";

        /// <summary>
        /// The site contact Request Status field
        /// </summary>
        public const string RequestStatusField = "RequestStatus";

        /// <summary>
        /// The site contact Request Status field Pending
        /// </summary>
        public const string RequestStatusFieldPending = "Pending";

        /// <summary>
        /// The moderator field
        /// </summary>
        public const string ModeratorField = "Moderator";

        /// <summary>
        /// The DueDate Field
        /// </summary>
        public const string DueDateField = "DueDate";

        /// <summary>
        /// The Created By Field
        /// </summary>
        public const string CreatedByField = "Author";

        /// <summary>
        /// The list ID
        /// </summary>
        public const string ListId = "ListID";

        #endregion

        #region Contact details

        /// <summary>
        /// The site contact name field
        /// </summary>
        public const string ContactName = "RequestorName";

        /// <summary>
        /// The site contact Email field
        /// </summary>
        public const string ContactEmail = "RequestorEmail";

        /// <summary>
        /// The site contact Phone field
        /// </summary>
        public const string ContactPhone = "RequesterPhone";

        /// <summary>
        /// The site contact Manager field
        /// </summary>
        public const string ManagerName = "ManagerName";

        /// <summary>
        /// The site contact RequestorDepartment field
        /// </summary>
        public const string RequestorDepartmentField = "RequestorDepartment";

        /// <summary>
        /// The site contact RequestorFacility field
        /// </summary>
        public const string RequestorFacilityField = "RequestorFacility";

        /// <summary>
        /// The site contact RequestorGlobalId field
        /// </summary>
        public const string RequestorGlobalIdField = "RequestorGlobalId";

        /// <summary>
        /// The site request status field
        /// </summary>
        public const string SiteRequestStatusField = "RequestStatus";

        /// <summary>
        /// The site request status timestamp field
        /// </summary>
        public const string SiteRequestStatusTimestampField = "RequestStatusTimestamp";

        /// <summary>
        /// The site workflow status field
        /// </summary>
        public const string SiteWorkflowStatusField = "WorkflowStatus";

        #endregion

        #region Site Request Webpart

        /// <summary>
        /// Layout Images folder 
        /// </summary>
        public const string LayoutImageProvisioningFolder = "~/_layouts/15/JCI.Foundation.SiteProvisioning/Images/";

        /// <summary>
        /// Configurable list details
        /// </summary>
        public const string ConfigurableList = "SiteApproverList";

        /// <summary>
        /// The site type name project site
        /// </summary>
        public const string SiteTemplateIdProjectSiteWithHash = "PROJECTSITE#0";

        /// <summary>
        /// The site type name community site
        /// </summary>
        public const string SiteTemplateIdCommunitySiteWithHash = "COMMUNITY#0";

        /// <summary>
        /// The site type name team site
        /// </summary>
        public const string SiteTemplateIdTeamSiteWithHash = "STS#0";

        /// <summary>
        /// Request processed
        /// </summary>
        public const string Finished = "Finished";

        /// <summary>
        /// Request raised
        /// </summary>
        public const string Started = "Started";

        /// <summary>
        /// Request raised
        /// </summary>
        public const string SystemError = "SystemError";

        /// <summary>
        /// Gets the url for image for each site template
        /// </summary>
        public const string SiteTemplateImageSource = "SiteTemplateImage";

        /// <summary>
        /// Defines site type for each site template
        /// </summary>
        public const string SiteType = "SiteType";

        /// <summary>
        /// Describes each site template
        /// </summary>
        public const string SiteTemplateDescription = "SiteTemplateDescription";

        /// <summary>
        /// Gets the site template id for each template
        /// </summary>
        public const string SiteTemplateId = "SiteTemplateId";
        #endregion

        #region Configurable List

        /// <summary>
        /// Configurable list details
        /// </summary>
        public const string SiteTemplateField = "SiteTemplate";

        /// <summary>
        /// Configurable list details
        /// </summary>
        public const string CountryField = "Country";

        /// <summary>
        /// Configurable StatusFieldNew details
        /// </summary>
        public const string StatusFieldNew = "New";

        /// <summary>
        /// Test site Yes
        /// </summary>
        public const string TestSiteYes = "true";

        /// <summary>
        /// Test site No
        /// </summary>
        public const string TestSiteNo = "false";

        /// <summary>
        /// Test site Yes
        /// </summary>
        public const string TestYes = "Yes";

        /// <summary>
        /// Configurable WorkflowStatusFieldRejected details
        /// </summary>
        public const string WorkflowStatusFieldRejected = "Rejected";

        /// <summary>
        /// Site Url already exists
        /// </summary>
        public const string SiteUrlExistsText = "SiteUrlExistsText";

        /// <summary>
        /// Page check in status
        /// </summary>
        public const string CheckInStatus = "Page checked in";

        /// <summary>
        /// Page publish status
        /// </summary>
        public const string PublishStatus = "Page published";

        /// <summary>
        /// Team site
        /// </summary>
        public const string SiteTemplateIdTeamSite = "STS0";

        /// <summary>
        /// Team site feature XML
        /// </summary>
        public const string TeamSiteFeatureXml = @"<SiteProvisionSitefeature><SiteLeveLFeature>
<FeatureId>afbe0759-62db-4e7a-bf45-66756f0ad1cf</FeatureId><Ranking>1</Ranking>
<FeatureId>00ce7360-6b31-4dd0-8080-7c684a7911b3</FeatureId><Ranking>2</Ranking>
<FeatureId>f4c77600-2297-4c97-9059-570c52101a50</FeatureId><Ranking>3</Ranking>
<FeatureId>414e6d23-379b-4da2-8895-3ca5eade2b4f</FeatureId><Ranking>4</Ranking>
<FeatureId>8599557d-cb3e-4185-a1f5-cf09c9445b70</FeatureId><Ranking>5</Ranking>
<FeatureId>e32afde8-7b9f-4c59-9de2-d64f2514fcdd</FeatureId><Ranking>6</Ranking>
<FeatureId>37ca0e87-bb09-4e32-b06d-25c2812626f1</FeatureId><Ranking>7</Ranking>
<FeatureId>056ee638-9c10-4eb6-83f7-a34dcb4c1de8</FeatureId><Ranking>8</Ranking>
<FeatureId>b1894cf6-f138-452d-be02-7785c158bb01</FeatureId><Ranking>9</Ranking>
<FeatureId>0af5989a-3aea-4519-8ab0-85d91abe39ff</FeatureId><Ranking>10</Ranking>
</SiteLeveLFeature>
</SiteProvisionSitefeature>";

        /// <summary>
        /// Collaboration site site
        /// </summary>
        public const string SiteTemplateIdCommunitySite = "COMMUNITY#0";

        /// <summary>
        /// Community site feature XML
        /// </summary>
        public const string CommunitySiteFeatureXml = @"<SiteProvisionSitefeature><SiteLeveLFeature>
<FeatureId>ad2b52fa-0217-49da-b9cd-1bf192bd6773</FeatureId><Ranking>1</Ranking>
<FeatureId>1eb21665-4f04-4310-8980-104b96887e4b</FeatureId><Ranking>2</Ranking>
<FeatureId>4017ca37-0e47-4a09-82e9-10eb15a6852e</FeatureId><Ranking>3</Ranking>
<FeatureId>062007ce-b336-462c-9c26-832251925063</FeatureId><Ranking>4</Ranking>
<FeatureId>fa6c4821-28a5-4dd0-a1a2-eed33de5bfa4</FeatureId><Ranking>5</Ranking>
<FeatureId>a6dfd15f-4f74-45b0-8d24-d55531d1c454</FeatureId><Ranking>6</Ranking>
<FeatureId>74500fd8-1d1c-4c02-81d6-d95af647f70d</FeatureId><Ranking>7</Ranking>
<FeatureId>0489189a-f82d-43ac-be31-b738f5f9aa15</FeatureId><Ranking>8</Ranking>
<FeatureId>1fe8bebf-b0cc-4330-9078-7e4fd42e2b75</FeatureId><Ranking>9</Ranking>
<FeatureId>37ca0e87-bb09-4e32-b06d-25c2812626f1</FeatureId><Ranking>10</Ranking>
<FeatureId>056ee638-9c10-4eb6-83f7-a34dcb4c1de8</FeatureId><Ranking>11</Ranking>
<FeatureId>b1894cf6-f138-452d-be02-7785c158bb01</FeatureId><Ranking>12</Ranking>
<FeatureId>4AEC7207-0D02-4f4f-AA07-B370199CD0C7</FeatureId><Ranking>13</Ranking>
<FeatureId>0af5989a-3aea-4519-8ab0-85d91abe39ff</FeatureId><Ranking>14</Ranking>
</SiteLeveLFeature>
</SiteProvisionSitefeature>";

        /// <summary>
        /// Project site
        /// </summary>
        public const string SiteTemplateIdProjectSite = "PROJECTSITE0";

        /// <summary>
        /// Project site feature XML
        /// </summary>
        public const string ProjectSiteFeatureXml = @"<SiteProvisionSitefeature><SiteLeveLFeature>
<FeatureId>afbe0759-62db-4e7a-bf45-66756f0ad1cf</FeatureId><Ranking>1</Ranking>
<FeatureId>e6e51ed8-5ee4-4e7c-ad34-0bf4a57d2d74</FeatureId><Ranking>2</Ranking>
<FeatureId>6dcd6744-dd62-4b97-a8af-ca207d1d4439</FeatureId><Ranking>3</Ranking>
<FeatureId>a259ae71-771c-4554-b83b-ce8c2b4d2b5b</FeatureId><Ranking>4</Ranking>
<FeatureId>52c043fc-4335-4c82-8385-ca1fca938e97</FeatureId><Ranking>5</Ranking>
<FeatureId>37ca0e87-bb09-4e32-b06d-25c2812626f1</FeatureId><Ranking>6</Ranking>
<FeatureId>056ee638-9c10-4eb6-83f7-a34dcb4c1de8</FeatureId><Ranking>7</Ranking>
<FeatureId>b1894cf6-f138-452d-be02-7785c158bb01</FeatureId><Ranking>8</Ranking>
<FeatureId>0af5989a-3aea-4519-8ab0-85d91abe39ff</FeatureId><Ranking>9</Ranking>
<FeatureId>e30f826f-0fc4-473a-87d6-9bc886a37158</FeatureId><Ranking>10</Ranking>
</SiteLeveLFeature>
</SiteProvisionSitefeature>";
        #endregion

        /// <summary>
        /// Partner site starting text for IDs
        /// </summary>
        public const string PartnerSiteTemplateIdStartsWith = "PARTNER";

        /// <summary>
        /// Partner team site
        /// </summary>
        public const string PartnerSiteTemplateIdTeamSite = "PARTNERTEAMSITE";

        /// <summary>
        /// Partner community site
        /// </summary>
        public const string PartnerSiteTemplateIdCommunitySite = "PARTNERCOMMUNITYSITE";

        /// <summary>
        /// Partner project site
        /// </summary>
        public const string PartnerSiteTemplateIdProjectSite = "PARTNERPROJECTSITE";

        /// <summary>
        /// Key for storing JCI terms and condition page link
        /// </summary>
        public const string JCITermsPageLinkKey = "JCITermsPageLink";

        /// <summary>
        /// Key for storing JCI terms and condition page link for partner sites
        /// </summary>
        public const string JCIPartnerTermsPageLinkKey = "JCIPartnerTermsPageLink";

        /// <summary>
        /// The JCI CAM branding property bag key
        /// </summary>
        public const string JCICAMBrandingPropertyBagKey = "JCICAMBrandingPropertyBag";

        #region Taxonomy Constants

        /// <summary>
        /// The metadata group name
        /// </summary>
        public const string MetadataGroupName = "JCI.Foundation.Terms";

        /// <summary>
        /// The business unit term set name
        /// </summary>
        public const string TermsetNameBusinessUnit = "JCIBusinessUnit";

        /// <summary>
        /// The language term set name
        /// </summary>
        public const string TermsetNameLanguage = "JCILanguage";

        /// <summary>
        /// The region term set name
        /// </summary>
        public const string TermsetNameRegion = "JCILocation";

        /// <summary>
        /// The country term set name
        /// </summary>
        public const string TermsetNameCountry = "JCICountry";

        /// <summary>
        /// Customer term set name
        /// </summary>
        public const string TermsetNameCustomers = "JCICustomers";

        #endregion Taxonomy Constants

        #region SiteMetadata

        /// <summary>
        /// SiteURL Metadata configuration value
        /// </summary>
        public const string SiteMetadataSiteURL = "SiteDirectoryListMetadataSiteURL";

        /// <summary>
        /// list id Metadata configuration value
        /// </summary>
        public const string SiteMetadatalistid = "SiteDirectoryListMetadatalistid";

        /// <summary>
        /// Property key value for storing customer names
        /// </summary>
        public const string SiteCustomersPropertyBagKey = "JCICustomer";

        #endregion

        #region Site WebPart

        /// <summary>
        /// Script Path
        /// </summary>
        public const string FeatureIdxPath = "/SiteProvisionSitefeature/SiteLeveLFeature";

        /// <summary>
        /// date time control frame
        /// </summary>
        public const string DateTimeControlFrame = "/{0}/iframe.aspx";

        /// <summary>
        /// System account login name
        /// </summary>
        public const string SystemAccount = "SHAREPOINT\\system";
        #endregion

        #region Site provisioning JOB properties

        /// <summary>
        /// Start Hour property
        /// </summary>
        public const string StartHour = "StartHour";

        /// <summary>
        /// Start Minute property
        /// </summary>
        public const string StartMinute = "StartMinute";

        /// <summary>
        /// Start Second property
        /// </summary>
        public const string StartSecond = "StartSecond";

        /// <summary>
        /// Finish Hour property
        /// </summary>
        public const string FinishHour = "FinishHour";

        /// <summary>
        /// Finish Hour property
        /// </summary>
        public const string FinishMinute = "FinishMinute";

        /// <summary>
        /// Finish Second property
        /// </summary>
        public const string FinishSecond = "FinishSecond";

        #endregion

        #region Workflow Status Field
        /// <summary>
        /// Approved Workflow status
        /// </summary>
        public const string WorkflowStatusApproved = "Approved";

        /// <summary>
        /// Rejected Workflow Status
        /// </summary>
        public const string WorkflowStatusRejected = "Rejected";

        /// <summary>
        /// MoreInfoRequired workflow Status
        /// </summary>
        public const string WorkflowStatusMoreInfoRequired = "MoreInfoRequired";

        /// <summary>
        /// MoreInfoRequired workflow Status
        /// </summary>
        public const string WorkflowStatusPending = "Pending";

        /// <summary>
        /// InformationProvided workflow Status
        /// </summary>
        public const string WorkflowStatusInformationProvided = "InformationProvided";
        #endregion

        #region Workflow Task Fields

        /// <summary>
        /// Workflow Task Comment field name
        /// </summary>
        public const string WorkflowTaskCommentsField = "_Comments";

        /// <summary>
        /// Workflow Task Related Items Field Name
        /// </summary>
        public const string WorkflowTaskRelatedItemField = "RelatedItems";

        /// <summary>
        /// Workflow Task List Name
        /// </summary>
        public const string WorkflowTaskListName = "JCIWorkflowTaskList";

        /// <summary>
        /// No Comments provided
        /// </summary>
        public const string NoCommentsProvided = "--No comments provided--";

        /// <summary>
        /// Edit site request
        /// </summary>
        public const string EditRequestUrl = "/SitePages/Edit Request.aspx?jciitemid=";

        /// <summary>
        /// Edit site request
        /// </summary>
        public const string CancelRequestUrl = "/SitePages/Cancel Request.aspx?jciitemid=";

        /// <summary>
        /// Replace more information body
        /// </summary>
        public const string ReplaceMoreInfoBody = "<br/>\n<br/>\nPlease provide the following information, so that we can proceed with processing your request: \n<br/>\n \n";

        /// <summary>
        /// Replace more information body
        /// </summary>
        public const string ReplaceRejectedBody = "because ";
        #endregion

        #region Test Site

        /// <summary>
        /// Test site feature id
        /// </summary>
        public const string TestSiteFeatureGuid = "0925c121-6f17-4120-b443-2e88f5160941";

        #endregion

        #region Community Constants

        /// <summary>
        /// Member Status Active
        /// </summary>
        public const int MemberStatusIntActive = 1;

        /// <summary>
        /// Member Field Name
        /// </summary>
        public const string MemberFieldName = "Member";

        /// <summary>
        /// Community Member ContentType Id
        /// </summary>
        public const string CommunityMemberContentTypeId = "0x010027FC2137D8DE4b00A40E14346D070D5201";

        /// <summary>
        /// Status Field Name
        /// </summary>
        public const string StatusIntFieldName = "MemberStatusInt";

        /// <summary>
        /// Member List Name
        /// </summary>
        public const string MemberListName = "Community Members";

        /// <summary>
        /// Community Members Count
        /// </summary>
        public const string CommunityMembersCount = "Community_MembersCount";

        #endregion

        #region Site Policy
        /// <summary>
        /// Test site policy
        /// </summary>
        public const string SitePolicyName = "SitePolicykey";

        /// <summary>
        /// Test site policy id
        /// </summary>
        public const string ProjectPolicyId = "projectpolicyid";

        /// <summary>
        /// Test site policy has expired property
        /// </summary>
        public const string DlcSiteHasExpirationPolicy = "dlc_siteHasExpirationPolicy";

        /// <summary>
        /// Test site policy exception
        /// </summary>
        public const string SitePolicyException = "SitePolicyException";

        #endregion

        /// <summary>
        /// Characters which are not allowed in business unit, region, country
        /// </summary>
        public const string ValidationCharacters = @"[\u0000-\uFFFF-[^""~#$%&*+|}{:?><=\'/;]]";

        /// <summary>
        /// Site title validator
        /// </summary>
        public const string SiteTitleValidation = "#%&*:<>?\\//\"{}~|+";

        /// <summary>
        /// view state value
        /// </summary>
        public const string SiteTitleValue = "SiteTitleValue";

        /// <summary>
        /// The theme directory
        /// </summary>
        public const string THEMESDIRECTORY = "/_catalogs/theme/15/{0}";

        /// <summary>
        /// The master page directory
        /// </summary>
        public const string MASTERPAGEDIRECTORY = "/_catalogs/masterpage/{0}";

        /// <summary>
        /// The master page seattle
        /// </summary>
        public const string MASTERPAGESEATTLE = "/_catalogs/masterpage/seattle.master";

        #region Site Features Configuration List
        
        /// <summary>
        /// Name of the column containing the path of the Site Template Icon
        /// </summary>
        public const string SiteTemplateImageColumnName = "SiteTemplateImage";

        /// <summary>
        /// Contains Description 
        /// </summary>
        public const string SiteTemplateDescriptionColumnName = "SiteTemplateDescription";

        /// <summary>
        /// Managed path for each type of site
        /// </summary>
        public const string SiteManagedPathColumnName = "ManagedPath";

        /// <summary>
        /// FeatureId xml column name
        /// </summary>
        public const string FeatureIdColumnName = "FeatureId";

        /// <summary>
        /// Site Name display value
        /// </summary>
        public const string SiteTypeColumnName = "SiteType";

        /// <summary>
        /// Path of Site Icon Team
        /// </summary>
        public const string SiteTemplateImageNameTeamSite = "TeamSiteIcon.jpg";
        
        /// <summary>
        /// Path of Site Icon Project
        /// </summary>
        public const string SiteTemplateImageNameProjectSite = "ProjectSiteIcon.jpg";
        
        /// <summary>
        /// Path of Site Icon Community
        /// </summary>
        public const string SiteTemplateImageNameCommunitySite = "CommunitySiteIcon.jpg";

        /// <summary>
        /// Team Managed Path
        /// </summary>
        public const string SiteTemplateManagedPathTeamSite = "sites";
       
        /// <summary>
        /// Project Managed Path
        /// </summary>
        public const string SiteTemplateManagedPathProjectSite = "sites";

        /// <summary>
        /// Community Managed Path
        /// </summary>
        public const string SiteTemplateManagedPathCommunitySite = "sites";

        /// <summary>
        /// Default is true
        /// </summary>
        public const bool SiteTemplateAvailableForSelection = true;
        #endregion

        #region Content Type

        /// <summary>
        /// base Metadata content type
        /// </summary>
        public const string BaseMetaDataContentType = "JCI Document";

        /// <summary>
        /// JCI Document Excel content type
        /// </summary>
        public const string JCIDocumentExcelContentType = "JCI Document Excel";

        /// <summary>
        /// JCI Document PowerPoint content type
        /// </summary>
        public const string JCIDocumentPowerPointContentType = "JCI Document PowerPoint";

        /// <summary>
        /// JCI General Project Document content type
        /// </summary>
        public const string JCIGeneralProjDocumentExcel = "JCI General Project Document Excel";

        /// <summary>
        /// JCI General Project Document PowerPoint content type
        /// </summary>
        public const string JCIGeneralProjDocumentPowerPoint = "JCI General Project Document PowerPoint";

        /// <summary>
        /// JCI IT Project Document Excel content type
        /// </summary>
        public const string JCIITProjDocumentExcel = "JCI IT Project Document Excel";

        /// <summary>
        /// JCI IT Project Document PowerPoint content type
        /// </summary>
        public const string JCIITProjDocumentPowerPoint = "JCI IT Project Document PowerPoint";

        /// <summary>
        /// Xml to show DIP for Content Type
        /// </summary>
        public const string ContentTypeShowDIPXml = "<customXsn xmlns=\"http://schemas.microsoft.com/office/2006/metadata/customXsn\"><xsnLocation></xsnLocation><cached>True</cached><openByDefault>True</openByDefault><xsnScope></xsnScope></customXsn>";

        /// <summary>
        /// Content Type DIP Schema Xml
        /// </summary>
        public const string ContentTypeDIPSchemaXml = "http://schemas.microsoft.com/office/2006/metadata/customXsn";

        #endregion

        #region Content Type Constants

        /// <summary>
        /// Name of the after property ContentTypeId
        /// </summary>
        public const string ContentTypeIdAfterPropertyName = "ContentTypeId";

        /// <summary>
        /// Base Metadata Content Type Id
        /// </summary>
        public const string BaseMetadataContentTypeId = "0x010100EA254AAEB50D427F87E6DD3EC4BD2437";

        /// <summary>
        /// General Project Document Content Type 
        /// </summary>
        public const string GeneralProjectDocument = "JCI General Project Document";

        /// <summary>
        /// IT Project Document Content Type 
        /// </summary>
        public const string ItProjectDocument = "JCI IT Project Document";

        /// <summary>
        /// General Project Document Content Type Id
        /// </summary>
        public const string GeneralProjectDocumentContentTypeId =
            "0x010100EA254AAEB50D427F87E6DD3EC4BD2437001AA0C7DF0AF14AE6B3FCF07D05373CB4";

        /// <summary>
        /// IT Project Document Content Type Id
        /// </summary>
        public const string ItProjectDocumentContentTypeId =
            "0x010100EA254AAEB50D427F87E6DD3EC4BD243700DEB8C8D5795E4CCF916F3DECC091F51C";

        /// <summary>
        /// BusinessUnit column name in BaseMetadata Content Type
        /// </summary>
        public const string BusinessUnitColumnName = "JCIBusinessUnit";

        /// <summary>
        /// JCILocation column name in BaseMetadata Content Type
        /// </summary>
        public const string JciLocationColumnName = "JCILocation";

        /// <summary>
        /// JCILanguage column name in BaseMetadata Content Type
        /// </summary>
        public const string JciLanguageColumnName = "JCILanguage";

        /// <summary>
        /// Data Classification Level column name in BaseMetadata Content Type
        /// </summary>
        public const string DataClassificationLevelColumnName = "DataClassificationLevel";

        /// <summary>
        /// Data Classification Level term set name 
        /// </summary>
        public const string DataClassificationLevel = "DataClassificationLevel";
        #endregion

        /// <summary>
        /// Data Classification Level term set name 
        /// </summary>
        public const string SiteClassificationLevel = "SiteClassificationLevel";

        #region Auto Tagging

        /// <summary>
        /// Taxonomy Hidden List Name
        /// </summary>
        public const string TaxonomyHiddenListName = "TaxonomyHiddenList";

        /// <summary>
        /// The taxonomy fields identifier for term
        /// </summary>
        public const string TaxonomyFieldsIDForTerm = "IdForTerm";

        /// <summary>
        /// Name of Auto Tagging Class
        /// </summary>
        public const string AutoTaggingClass = "JCI.Foundation.Collaboration.EventReceiver.AutoTaggingEventReceiver.AutoTaggingEventReceiver";
        #endregion

        #region Config List GCL
        /// <summary>
        /// The Site request Email Subject
        /// </summary>
        public const string SiteRequestEmailSubjectKey = "SiteRequestEmailSubjectKey";

        /// <summary>
        /// The Site request Email Body
        /// </summary>
        public const string SiteApproveEmailBodyKey = "SiteApproveEmailBodyKey";        

        /// <summary>
        /// The Rejected Site Request Email Subject Key
        /// </summary>
        public const string SiteRejectEmailSubjectKey = "SiteRejectEmailSubjectKey";

        /// <summary>
        /// The Rejected Site Request Email Body Key
        /// </summary>
        public const string SiteRejectEmailBodyKey = "SiteRejectEmailBodyKey";

        /// <summary>
        /// The More info Required Site Request Email Subject Key
        /// </summary>
        public const string SiteMoreInfoRequiredSubjectKey = "SiteMoreInfoRequiredSubjectKey";

        /// <summary>
        /// The More info Required Site Request Email Body Key
        /// </summary>
        public const string SiteMoreInfoRequiredEmailBodyKey = "SiteMoreInfoRequiredEmailBodyKey";

        /// <summary>
        /// The site request failed email subject key
        /// </summary>
        public const string SiteRequestFailedEmailSubjectKey = "SiteRequestFailedEmailSubjectKey";

        /// <summary>
        /// The site request failed email body key
        /// </summary>
        public const string SiteRequestFailedEmailBodyKey = "SiteRequestFailedEmailBodyKey";
        #endregion
    }
}
