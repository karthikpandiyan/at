// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationCommonHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Migration common helper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Migration.Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.SPHelpers;  
    using JCI.CAM.Migration.Common;    
    using JCI.CAM.Migration.Common.Authentication;    
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Provisioning.Core; 
    using Microsoft.SharePoint.Client;    
    using Microsoft.SharePoint.Client.Utilities;    

    /// <summary>
    /// Migration common helper
    /// </summary>
    public static class MigrationCommonHelper
    {
        /// <summary>
        /// Get the site owner details
        /// </summary>
        /// <param name="context">ClientContext instance</param>
        /// <param name="siteMigrationRequest">Gets the site migration request list item</param>
        /// <param name="listTitle">Site migration request list title</param>
        /// <returns>Returns the site owner details</returns>
        public static List<SharePointUser> GetSiteOwners(ClientContext context, ListItem siteMigrationRequest, string listTitle)
        {
            List<SharePointUser> users = new List<SharePointUser>();

            try
            {
                string siteOwnersColumnName = MigrationConstants.SiteOwnersColumn;
                FieldUserValue[] siteOwners = (FieldUserValue[])siteMigrationRequest[siteOwnersColumnName];

                if (siteOwners != null)
                {
                    foreach (FieldUserValue siteOwner in siteOwners)
                    {
                        try
                        {
                            if (siteOwner != null)
                            {
                                User user = context.Web.EnsureUser(siteOwner.LookupValue);
                                context.Load(user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                                context.ExecuteQuery();

                                var sharePointUser = new SharePointUser()
                                {
                                    Email = user.Email,
                                    LoginName = user.LoginName,
                                    Name = user.Title
                                };

                                if (sharePointUser.LoginName.Contains("|"))
                                {
                                    sharePointUser.LoginName = sharePointUser.LoginName.Substring(sharePointUser.LoginName.LastIndexOf("|") + 1);
                                }

                                users.Add(sharePointUser);
                            }
                            else
                            {
                                LogHelper.LogInformation("Site owner is invalid or null.", LogEventID.InformationWrite);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging(ex, string.Format(CultureInfo.InvariantCulture, "Error occured while accessing the site owners column from list {0}", listTitle));
                        }
                    }
                }
                else
                {
                    LogHelper.LogInformation("There are no site owners.", LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging(ex, string.Format(CultureInfo.InvariantCulture, "Error occured while accessing the site owners column from list {0}", listTitle));
            }

            return users;
        }

        /// <summary>
        /// Retrieve the description on the enumeration
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>Returns status</returns>
        public static string GetEnumFriendlyName(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        /// <summary>
        /// Exception logging
        /// </summary>
        /// <param name="ex">Exception Object</param>
        /// <param name="errorData">Error message</param>
        public static void ExceptionLogging(Exception ex, string errorData)
        {
            LogHelper.LogInformation(errorData, JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            LogHelper.LogError(ex, LogEventID.ExceptionHandling);
        }
    }
}
