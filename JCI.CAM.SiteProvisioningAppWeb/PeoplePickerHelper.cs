//-----------------------------------------------------------------------
// <copyright file="PeoplePickerHelper.cs" company="Microsoft">
//     Microsoft &amp; JCI .
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.SiteProvisioningAppWeb
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI.WebControls;
    using JCI.CAM.SiteProvisioningAppWeb.Models;
    using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    
    /// <summary>
    /// Helper class that provides method to call SP service for people picker control
    /// </summary>
    public class PeoplePickerHelper
    {
        /// <summary>
        /// Gets search results
        /// </summary>
        /// <returns>Search Results</returns>
        public static string GetPeoplePickerSearchData()
        {
            var sharePointContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
            using (var context = sharePointContext.CreateUserClientContextForSPHost())
            {
                return GetPeoplePickerSearchData(context);
            }
        }

        /// <summary>
        /// Gets search results
        /// </summary>
        /// <param name="context">The SP Client Context Object</param>
        /// <returns>Search Results</returns>
        public static string GetPeoplePickerSearchData(ClientContext context)
        {
            // get searchstring and other variables
            var searchString = (string)HttpContext.Current.Request["SearchString"];
            int principalType = Convert.ToInt32(HttpContext.Current.Request["PrincipalType"]);

            ClientPeoplePickerQueryParameters querryParams = new ClientPeoplePickerQueryParameters
            {
                AllowMultipleEntities = false,
                MaximumEntitySuggestions = 2000,
                PrincipalSource = PrincipalSource.All,
                PrincipalType = (PrincipalType)principalType,
                QueryString = searchString
            };

            // execute query to Sharepoint
            ClientResult<string> clientResult = Microsoft.SharePoint.ApplicationPages.ClientPickerQuery.ClientPeoplePickerWebServiceInterface.ClientPeoplePickerSearchUser(context, querryParams);
            context.ExecuteQuery();
            return clientResult.Value;
        }

        /// <summary>
        /// Adds user to the List and stores it in the Hidden field in JSON serialized form
        /// </summary>
        /// <param name="peoplePickerHiddenField">The hidden field to store the value</param>
        /// <param name="user">The User to add to the list</param>
        public static void FillPeoplePickerValue(HiddenField peoplePickerHiddenField, Microsoft.SharePoint.Client.User user)
        {
            List<PeoplePickerUser> peoplePickerUsers = new List<PeoplePickerUser>(1)
            {
                new PeoplePickerUser() { Name = user.Title, Email = user.Email, Login = user.LoginName }
            };
            peoplePickerHiddenField.Value = JsonHelper.Serialize<List<PeoplePickerUser>>(peoplePickerUsers);
        }

        /// <summary>
        /// Adds users to the List and stores it in the Hidden field in JSON serialized form
        /// </summary>
        /// <param name="peoplePickerHiddenField">The hidden field to store the value</param>
        /// <param name="users"> Users to add to the list</param>
        public static void FillPeoplePickerValue(HiddenField peoplePickerHiddenField, Microsoft.SharePoint.Client.User[] users)
        {
            List<PeoplePickerUser> peoplePickerUsers = new List<PeoplePickerUser>();
            foreach (var user in users)
            {
                peoplePickerUsers.Add(new PeoplePickerUser() { Name = user.Title, Email = user.Email, Login = user.LoginName });
            }

            peoplePickerHiddenField.Value = JsonHelper.Serialize<List<PeoplePickerUser>>(peoplePickerUsers);
        }

        /// <summary>
        /// Retrieves People list from the hidden field
        /// </summary>
        /// <param name="peoplePickerHiddenField">The hidden field that stores the serialized JSON</param>
        /// <returns>List of users</returns>
        public static List<PeoplePickerUser> GetValuesFromPeoplePicker(HiddenField peoplePickerHiddenField)
        {
            return JsonHelper.Deserialize<List<PeoplePickerUser>>(peoplePickerHiddenField.Value);
        }
    }
}