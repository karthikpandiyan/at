//-----------------------------------------------------------------------
// <copyright file= "ProfileHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//---------------------------------------------------------------------

namespace JCI.CAM.AutoTaggingAppWeb
{    
    using System;
    using System.Collections.Generic;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.UserProfiles;

    /// <summary>
    /// Helper Class to Retrieve User Profile Properties
    /// </summary>
    public static class ProfileHelper
    {
        /// <summary>
        /// Gets a user profile property Value for the specified user.
        /// </summary>
        /// <param name="ctx">An Authenticated ClientContext</param>
        /// <param name="userName">The name of the target user.</param>
        /// <param name="propertyName">The value of the property to get.</param>
        /// <returns><see cref="System.String"/>The specified profile property for the specified user. Will return an Empty String if the property is not available.</returns>
        public static string GetProfilePropertyFor(ClientContext ctx, string userName, string propertyName)
        {
            string result = string.Empty;
            if (ctx != null)
            {
                try
                {
                    //// PeopleManager class provides the methods for operations related to people
                    PeopleManager peopleManager = new PeopleManager(ctx);
                    //// GetUserProfilePropertyFor method is used to get a specific user profile property for a user
                    ClientResult<string> profileProperty = peopleManager.GetUserProfilePropertyFor(userName, propertyName);
                    ctx.ExecuteQuery();
                    result = profileProperty.Value;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// GetUserProfilePropertyFor method is used to get a specific user profile property for a user
        /// </summary>
        /// <param name="ctx">The authenticate context.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="profilePropertyNames">The profile property names.</param>
        /// <returns>Collection profile properties values</returns>
        public static IEnumerable<string> GetProfilePropertiesFor(ClientContext ctx, string userName, string[] profilePropertyNames)
        {
            if (ctx != null)
            {
                try
                {
                    //// PeopleManager class provides the methods for operations related to people
                    PeopleManager peopleManager = new PeopleManager(ctx);

                    //// GetUserProfilePropertyFor method is used to get a specific user profile property for a user
                    UserProfilePropertiesForUser profilePropertiesForUser = new UserProfilePropertiesForUser(ctx, userName, profilePropertyNames);
                    IEnumerable<string> profilePropertyValues = peopleManager.GetUserProfilePropertiesFor(profilePropertiesForUser);

                    // Load the request and run it on the server.
                    ctx.Load(profilePropertiesForUser);
                    ctx.ExecuteQuery();

                    return profilePropertyValues;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                    throw;
                }
            }

            return null;
        }
    }
}