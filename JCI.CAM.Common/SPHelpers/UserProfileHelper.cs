//-----------------------------------------------------------------------
// <copyright file="UserProfileHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.SPHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JCI.CAM.Common.Models;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using Microsoft.SharePoint.Client.UserProfiles;

    /// <summary>
    /// Helper class to retrieve Term sets from Term store
    /// </summary>
    public class UserProfileHelper
    {
        /// <summary>
        /// Get User Profile properties
        /// </summary>
        /// <param name="ctx">SP Client Context</param>
        /// <returns>User Profile object</returns>
        public static JCI.CAM.Common.Models.UserProfile GetUserProfile(ClientContext ctx)
        {
            Logging.LogHelper.LogInformation("Getting user profile");
            PeopleManager peopleManager = new PeopleManager(ctx);
            Logging.LogHelper.LogInformation("Getting profile properties");
            PersonProperties personProperties = peopleManager.GetMyProperties();
            
            // Load the request and run it on the server.
            ctx.Load(personProperties);
            ctx.ExecuteQuery();

            Logging.LogHelper.LogInformation("retrieving specific properties");
            var myContactEmail = string.Empty;
            try
            {
                myContactEmail = personProperties.UserProfileProperties[UserProfilePropertyHelper.ContactEmailProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Contact email not found");
            }

            var myContactName = string.Empty;
            try
            {
                myContactName = personProperties.UserProfileProperties[UserProfilePropertyHelper.ContactNameProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Contact name not found");
            }

            var myContactPhone = string.Empty;
            try
            {
                myContactPhone = personProperties.UserProfileProperties[UserProfilePropertyHelper.ContactPhoneProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Contact Phone not found");
            }

            var myManangerName = string.Empty;
            try
            {
                myManangerName = personProperties.UserProfileProperties[UserProfilePropertyHelper.ManagerProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Manager name not found");
            }

            var myRequestorDepartmentField = string.Empty;
            try
            {
                myRequestorDepartmentField = personProperties.UserProfileProperties[UserProfilePropertyHelper.RequestorDepartmentProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Department not found");
            }

            var myRequestorFacilityField = string.Empty;
            try
            {
                myRequestorFacilityField = personProperties.UserProfileProperties[UserProfilePropertyHelper.RequestorFacilityProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Requestor facility not found");
            }

            var myRequestorGlobalIdField = string.Empty;
            try
            {
                myRequestorGlobalIdField = personProperties.UserProfileProperties[UserProfilePropertyHelper.RequestorGlobalIdProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Global ID not found");
            }

            var myBusinessUnit = string.Empty;
            try
            {
                myBusinessUnit = personProperties.UserProfileProperties[UserProfilePropertyHelper.BusinessUnitProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Business Unit not found");
            }

            var myCountry = string.Empty;
            try
            {
                myCountry = personProperties.UserProfileProperties[UserProfilePropertyHelper.CountryProfileField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Country value not found");
            }

            var myRegion = string.Empty;
            try
            {
                myRegion = personProperties.UserProfileProperties[UserProfilePropertyHelper.AutoTagLocationField];
            }
            catch
            {
                Logging.LogHelper.LogInformation("Region value not found");
            }

            // Retrieve specific properties 
            JCI.CAM.Common.Models.UserProfile profile = new Common.Models.UserProfile()
            {
                ContactEmail = myContactEmail,
                ContactName = myContactName,
                ContactPhone = myContactPhone,
                ManangerName = myManangerName,
                RequestorDepartmentField = myRequestorDepartmentField,
                RequestorFacilityField = myRequestorFacilityField,
                RequestorGlobalIdField = myRequestorGlobalIdField,
                BusinessUnit = myBusinessUnit,
                Country = myCountry,
                Region = myRegion,
            };

            return profile;
        }
    }
}
