//-----------------------------------------------------------------------
// <copyright file="UserProfilePropertyHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common
{
    /// <summary>
    /// The user profile property.
    /// </summary>
    public static class UserProfilePropertyHelper
    {
        /// <summary>
        /// Business Unit user profile property
        /// </summary>
        public const string BusinessUnitProfileField = "JCI-BusinessOrg";

        /// <summary>
        /// Region user profile property
        /// </summary>
        public const string RegionProfileField = "SPS-Location";

        /// <summary>
        /// Contact Name user profile property
        /// </summary>
        public const string ContactNameProfileField = "PreferredName";

        /// <summary>
        /// Contact Email user profile property
        /// </summary>
        public const string ContactEmailProfileField = "WorkEmail";

        /// <summary>
        /// Contact Phone user profile property
        /// </summary>
        public const string ContactPhoneProfileField = "CellPhone";

        /// <summary>
        /// Requester Department user profile property
        /// </summary>
        public const string RequestorDepartmentProfileField = "Department";

        /// <summary>
        /// Requester Facility user profile property
        /// </summary>
        public const string RequestorFacilityProfileField = "Office";

        /// <summary>
        /// Requester groupID user profile property
        /// </summary>
        public const string RequestorGlobalIdProfileField = "AccountName";

        /// <summary>
        /// Manager user profile property
        /// </summary>
        public const string ManagerProfileField = "Manager";

        /// <summary>
        /// TimeZone user profile property
        /// </summary>
        public const string TimeZoneProfileField = "SPS-TimeZone";

        /// <summary>
        /// Country user profile property
        /// </summary>
        public const string CountryProfileField = "JCI-Country";

        /// <summary>
        /// Auto Tagging Business Unit User Profile Property
        /// </summary>
        public const string AutoTagBusinessUnitField = "JCI-AutoTagBU";

        /// <summary>
        /// Auto Tagging Location User Profile Property
        /// </summary>
        public const string AutoTagLocationField = "JCI-AutoTagLocation";

        /// <summary>
        /// Auto Tagging DataClassificationLevel User Profile Property
        /// </summary>
        public const string AutoTagDataClassificationLevelField = "JCI-AutoTagDataClassificationLevel";

        /// <summary>
        /// Auto Tagging Language User Profile Property
        /// </summary>
        public const string AutoTagLanguageField = "JCI-AutoTagLanguage";

        /// <summary>
        /// User Profile String Multi Value Delimiter
        /// </summary>
        public const string UserProfileStringMultiValueDelimiter = ",";
    }
}
