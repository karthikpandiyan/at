//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//     Microsoft &amp; JCI .
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.SiteProvisioningAppWeb.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using JCI.CAM.Common;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.SiteProvisioningAppWeb.Models;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// The controller that handles the root page calls
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The Index Action
        /// </summary>
        /// <returns>Returns Index Page</returns>
        [SharePointContextFilter]
        public ActionResult Index()
        {
            // HttpContext.Response.Write("<!-- Index: Page request -->");
            LogHelper.LogInformation("Index: Page request");

            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            try
            {
                using (var clientContext = context.CreateAppOnlyClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        User currentUser = clientContext.Web.CurrentUser;
                        clientContext.Load(currentUser, user => user.Title);
                        clientContext.ExecuteQuery();
                        ViewBag.UserName = currentUser.Title;
                    }
                    else
                    {
                        HttpContext.Response.Write("Unable to create SharePoint Context");
                        LogHelper.LogInformation("Unable to create client context");
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Response.Write("Unable to create SharePoint Context");
                LogHelper.LogError(ex);

                // throw ex;
            }

            return this.View();
        }

        /// <summary>
        /// Creates a Site Request
        /// </summary>
        /// <param name="request">A Request object containing site request data</param>
        /// <returns>Returns True or False based on the success-failure of the request</returns>
        [SharePointContextFilter]
        [System.Web.Mvc.HttpPost]
        public ActionResult Create(Request request)
        {
            try
            {
                LogHelper.LogInformation("Create: Request for new site received, site name: " + request.siteURL, JCI.CAM.Common.Logging.LogEventID.InformationWrite);

                // ToDo: Validate site create request parameters
                LogHelper.LogInformation("Validating request", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                if (request.siteType == null || request.managedPath == null || request.webAppUrl == null)
                {
                    LogHelper.LogInformation("Site templates were not loaded correctly, refresh your browser and try again");
                    return
                        this.Json(
                            new
                            {
                                result = false,
                                message = "Site templates were not loaded correctly, refresh your browser and try again"
                            });
                }

                if (request.startDate == null || request.startDate.Year < 1980)
                {
                    return this.Json(
                            new
                            {
                                result = false,
                                message = "Start date cannot be blank or have invalid value , refresh your browser and try again"
                            });
                }

                LogHelper.LogInformation("Checing primary owners");

                // Parse primary owner and secondary owners
                System.Collections.Generic.List<PeoplePickerUser> primaryOwners =
                    new System.Collections.Generic.List<PeoplePickerUser>();
                System.Collections.Generic.List<PeoplePickerUser> secondaryOwners =
                    new System.Collections.Generic.List<PeoplePickerUser>();

                if (request.primaryOwner == null)
                {
                    LogHelper.LogInformation("No primary owners found, rejecting request", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    return this.Json(new { result = false, message = "Primary owner is required" });
                }

                // Primary Owners
                LogHelper.LogInformation("Reading ownership details from request ", LogEventID.InformationWrite);
                dynamic owners = System.Web.Helpers.Json.Decode(request.primaryOwner);
                foreach (var o in owners)
                {
                    PeoplePickerUser people = new PeoplePickerUser()
                    {
                        Email = o.Email,
                        Name = o.Name,
                        LookupId = o.LookupId,
                        Login = o.Login
                    };

                    primaryOwners.Add(people);
                }

                if (request.secondaryOwner != null)
                {
                    // Secondary Owners
                    owners = System.Web.Helpers.Json.Decode(request.secondaryOwner);
                    foreach (var o in owners)
                    {
                        PeoplePickerUser people = new PeoplePickerUser()
                        {
                            Email = o.Email,
                            Name = o.Name,
                            LookupId = o.LookupId,
                            Login = o.Login
                        };

                        secondaryOwners.Add(people);
                    }
                }

                var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                LogHelper.LogInformation("Creating Client context", LogEventID.InformationWrite);
                using (var clientContext = context.CreateUserClientContextForSPHost())
                {
                    clientContext.RequestTimeout = Timeout.Infinite;

                    User currentUser = null;
                    User mgr = null;
                    JCI.CAM.Common.Models.UserProfile profile;
                    currentUser = clientContext.Web.CurrentUser;

                    try
                    {
                        LogHelper.LogInformation("Loading current user information", LogEventID.InformationWrite);
                        clientContext.Load(currentUser, user => user.Title, user => user.Email);
                        clientContext.ExecuteQuery();
                        LogHelper.LogInformation("Loading current user profile", LogEventID.InformationWrite);
                        profile = UserProfileHelper.GetUserProfile(clientContext);
                    }
                    catch
                    {
                        return this.Json(new { result = false, message = "Failed to load profile of the requestor" });
                    }

                    var web = clientContext.Web;
                    clientContext.ExecuteQuery();

                    // Load the manager information
                    LogHelper.LogInformation("Loading manager information");
                    if (string.IsNullOrEmpty(profile.ManangerName))
                    {
                        try
                        {
                            mgr = web.EnsureUser(profile.ManangerName);
                            clientContext.Load(mgr, user => user.Id);
                            clientContext.ExecuteQuery();
                            if (mgr != null)
                            {
                                LogHelper.LogInformation("manager id: " + mgr.Id);
                            }
                            else
                            {
                                LogHelper.LogInformation("manager not found");
                            }
                        }
                        catch
                        {
                            LogHelper.LogInformation("Error during Manager assignment");
                        }
                    }
                    else
                    {
                        LogHelper.LogInformation("Manager not assigned");
                    }

                    LogHelper.LogInformation("Validating web availability", LogEventID.InformationWrite);
                    var siteUrl = request.webAppUrl + request.managedPath + "/" + request.siteURL;
                    bool isExist = false;

                    try
                    {
                        isExist = this.CheckSiteExists(siteUrl);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex);
                    }

                    if (isExist)
                    {
                        // site found
                        LogHelper.LogInformation("Requested site already exists, rejecting request", LogEventID.InformationWrite);

                        // ToDo: Also look whether any request with same url is already present in the list
                        return this.Json(new { result = false, message = "The site already exists" });
                    }

                    LogHelper.LogInformation("Getting primary owners", LogEventID.InformationWrite);
                    List<FieldUserValue> usersList = new List<FieldUserValue>();
                    foreach (PeoplePickerUser p in primaryOwners)
                    {
                        User newUser = web.EnsureUser(p.Login);

                        try
                        {
                            clientContext.Load(newUser, user => user.Id);
                            clientContext.ExecuteQuery();
                            FieldUserValue userValue = new FieldUserValue { LookupId = newUser.Id };
                            usersList.Add(userValue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError(ex);
                            return this.Json(new { result = false, message = "Unable to resolve primary users list" });
                        }
                    }

                    LogHelper.LogInformation("Assigning additional owners", LogEventID.InformationWrite);
                    var secOwners = new List<FieldUserValue>();
                    if (secondaryOwners.Count > 0)
                    {
                        foreach (PeoplePickerUser p in secondaryOwners)
                        {
                            User newUser = web.EnsureUser(p.Login);
                            try
                            {
                                clientContext.Load(newUser);
                                clientContext.ExecuteQuery();
                                FieldUserValue userValue = new FieldUserValue { LookupId = newUser.Id };
                                secOwners.Add(userValue);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.LogError(ex);
                                LogHelper.LogInformation("Unable to resolve additional owners list", LogEventID.InformationWrite);
                                return
                                    this.Json(new { result = false, message = "Unable to resolve additional owners list" });
                            }
                        }
                    }

                    // ToDo: Check whether the given site exists in the provided managed path
                    var lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();
                    LogHelper.LogInformation("Fetching site request list...", LogEventID.InformationWrite);
                    var siteReq =
                        (from s in lists where s.Title.ToLower() == Constants.SiteRequestListName.ToLower() select s)
                            .FirstOrDefault();
                    if (siteReq != null)
                    {
                        ListItemCreationInformation info = new ListItemCreationInformation();
                        ListItem item = siteReq.AddItem(info);
                        LogHelper.LogInformation("Assigning values to item attributes", LogEventID.InformationWrite);

                        item[Constants.TitleField] = request.siteName;
                        item[Constants.SiteTypeField] = request.siteType;
                        item[Constants.SiteDescriptionField] = request.description;
                        LogHelper.LogInformation(Constants.TitleField + ": " + request.siteName);
                        LogHelper.LogInformation(Constants.SiteTypeField + ": " + request.siteType);
                        LogHelper.LogInformation(Constants.SiteDescriptionField + ": " + request.description);

                        // ToDo: See if SiteTemplate has to be saved
                        // item[Constants.SiteTemplateId] = request.siteTemplate;
                        if (request.siteTemplate == Constants.SiteTemplateIdProjectSite ||
                            request.siteTemplate == Constants.PartnerSiteTemplateIdProjectSite)
                        {
                            if (request.startDate != null && request.startDate.Year > 1980)
                            {
                                item[Constants.ProjectStartDate] = request.startDate;
                            }
                            else
                            {
                                item[Constants.ProjectStartDate] = DateTime.Now;
                            }

                            if (request.endDate != null && request.endDate.Year > 1980)
                            {
                                item[Constants.AnticipatedProjectEndDate] = request.endDate;
                            }
                            else
                            {
                                item[Constants.AnticipatedProjectEndDate] = null;
                            }
                        }

                        LogHelper.LogInformation(Constants.BusinessUnitSiteProperty + ": " + request.siteBU);
                        LogHelper.LogInformation(Constants.RegionSiteProperty + ": " + request.siteRegion);
                        LogHelper.LogInformation(Constants.CountryField + ": " + request.siteCountry);
                        LogHelper.LogInformation(Constants.ClarityProjectNumberField + ": " + request.projectNumber);

                        item[Constants.BusinessUnitSiteProperty] = request.siteBU;
                        if (request.jciCustomers != null && request.jciCustomers != string.Empty)
                        {
                            item[Constants.SiteCustomersField] = request.jciCustomers;
                        }

                        item[Constants.RegionSiteProperty] = request.siteRegion;
                        item[Constants.CountryField] = request.siteCountry;
                        if (request.projectNumber != null)
                        {
                            item[Constants.ClarityProjectNumberField] = request.projectNumber;
                        }

                        LogHelper.LogInformation(
                            "Requested site URL is: " + request.webAppUrl + request.managedPath + "/" + request.siteURL,
                            LogEventID.InformationWrite);
                        item[Constants.SiteUrlField] = request.webAppUrl + request.managedPath + "/" + request.siteURL;

                        // Pull this from user profile
                        LogHelper.LogInformation("Filling requestor profile attributes", LogEventID.InformationWrite);
                        if (profile != null)
                        {
                            if (profile.ContactName != null)
                            {
                                item[Constants.ContactName] = currentUser.Title;
                                LogHelper.LogInformation(Constants.ContactName + ": " + profile.ContactName);
                            }

                            if (profile.ContactPhone != null)
                            {
                                item[Constants.ContactPhone] = profile.ContactPhone;
                                LogHelper.LogInformation(Constants.ContactPhone + ": " + profile.ContactPhone);
                            }

                            if (!string.IsNullOrEmpty(profile.ManangerName))
                            {
                                try
                                {
                                    FieldUserValue manager = new FieldUserValue { LookupId = mgr.Id };
                                    item[Constants.ManagerName] = manager;
                                    LogHelper.LogInformation(Constants.ManagerName + ": " + profile.ManangerName);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.LogError(ex);
                                    LogHelper.LogInformation("Unable to set manager");
                                }
                            }

                            if (profile.RequestorDepartmentField != null)
                            {
                                item[Constants.RequestorDepartmentField] = profile.RequestorDepartmentField;
                                LogHelper.LogInformation(Constants.RequestorDepartmentField + ": " +
                                                         profile.RequestorDepartmentField);
                            }

                            if (profile.RequestorFacilityField != null)
                            {
                                item[Constants.RequestorFacilityField] = profile.RequestorFacilityField;
                                LogHelper.LogInformation(Constants.RequestorFacilityField + ": " +
                                                         profile.RequestorFacilityField);
                            }

                            if (profile.RequestorGlobalIdField != null)
                            {
                                item[Constants.RequestorGlobalIdField] = profile.RequestorGlobalIdField;
                                LogHelper.LogInformation(Constants.RequestorGlobalIdField + ": " +
                                                         profile.RequestorGlobalIdField);
                            }

                            if (profile.ContactEmail != null)
                            {
                                item[Constants.ContactEmail] = currentUser.Email;
                                LogHelper.LogInformation(Constants.ContactEmail + ": " + currentUser.Email);
                            }
                        }

                        LogHelper.LogInformation("Assigning workflow values", LogEventID.InformationWrite);

                        // Assign Workflow values
                        item[Constants.WorkflowStatusField] = Constants.StatusFieldNew;
                        item[Constants.RequestStatusField] = Constants.StatusFieldNew;

                        // ToDo: Implement Taxonomy Picker
                        // Partner & Community site based logic
                        if (request.siteTemplate.StartsWith(Constants.PartnerSiteTemplateIdStartsWith))
                        {
                            // Microsoft.SharePoint.Taxonomy.TaxonomyFieldValueCollection customerValueCollection = new Microsoft.SharePoint.Taxonomy.TaxonomyFieldValueCollection(string.Empty);
                            // customerValueCollection.PopulateFromLabelGuidPairs(TaxonomyWebTaggingControlCustomer.Text);
                            // item[Constants.SiteCustomersField] = customerValueCollection;
                        }

                        if (request.siteType == "COMMUNITY0")
                        {
                            item[Constants.CommunitySiteTypeField] = request.communitySiteType;
                        }
                        else
                        {
                            item[Constants.ConfidentialityLevelField] = request.confidentiality;
                        }

                        if (request.IsTestSite == true)
                        {
                            item[Constants.IsTestSite] = Constants.TestSiteYes;
                        }
                        else
                        {
                            item[Constants.IsTestSite] = Constants.TestSiteNo;
                        }

                        item[Constants.PrimaryOwnerField] = usersList;
                        if (secOwners.Count != 0)
                        {
                            item[Constants.SecondaryOwnerField] = secOwners;
                        }

                        LogHelper.LogInformation("Attemting to save request", LogEventID.InformationWrite);
                        item.Update();
                        try
                        {
                            clientContext.ExecuteQuery();
                            LogHelper.LogInformation("Request created", LogEventID.InformationWrite);
                            return this.Json(new { result = true, message = "The request has been created successfully" });
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError(ex);
                            return this.Json(new { result = false, message = "The request cannot be saved" });
                        }
                    }
                }

                return this.Json(new { result = false, message = "The request got terminated abruptly" });
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                if (ex.InnerException != null)
                {
                    LogHelper.LogError(ex.InnerException);
                }

                return
                    this.Json(
                        new
                        {
                            result = false,
                            message = "Failed to creates request, please refresh the page and try again"
                        });
            }
        }

        /// <summary>
        /// Validates whether the Site already exists or not
        /// </summary>
        /// <param name="name">Requested Site Title</param>
        /// <returns>Returns True or False based on the success-failure of the request</returns>
        [SharePointContextFilter]
        [System.Web.Mvc.HttpPost]
        public ActionResult CheckIfExists([System.Web.Http.FromBody] string name)
        {
            LogHelper.LogInformation("CheckIfExists: Checking if site by title exists, requested site title: " + name, JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(name))
            {
                return this.Json(new { exists = true });
            }

            try
            {
                if (this.CheckSiteExists(name))
                {
                    return this.Json(new { exists = true });
                }
                else
                {
                    return this.Json(new { exists = false });
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Unable to validate URL");
                LogHelper.LogError(ex);
                return this.Json(new { exists = false });
            }
        }

        /// <summary>
        ///     Checks if the site already exists or not
        /// </summary>
        /// <param name="siteUrl">The full URL of the requested site</param>
        /// <returns>True if site exists, else false</returns>
        public bool CheckSiteExists(string siteUrl)
        {
            string tenantAdminSiteUrl = (string)ConfigurationManager.AppSettings["TenantAdminUrl"];
            var tenantAdminUri = new Uri(tenantAdminSiteUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                var tenant = new Tenant(adminContext);
                try
                {
                    Site s = tenant.GetSiteByUrl(siteUrl);
                    adminContext.Load(s);
                    adminContext.ExecuteQuery();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Deletes a site provision request
        /// </summary>
        /// <param name="requestId">The ID of the SP SiteRequest List entry</param>
        /// <returns>Marks the site request as Rejected</returns>
        [SharePointContextFilter]
        [System.Web.Mvc.HttpPost]
        public ActionResult Delete([System.Web.Http.FromBody] int requestId)
        {
            LogHelper.LogInformation("Delete: Delete request received for Request ID: " + requestId, JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = context.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();
                    var siteReq =
                        (from s in lists where s.Title.ToLower() == Constants.SiteRequestListName.ToLower() select s)
                            .FirstOrDefault();
                    if (siteReq != null)
                    {
                        ListItem req = siteReq.GetItemById(requestId);
                        req[Common.Constants.WorkflowStatusField] = Common.Constants.WorkflowStatusFieldRejected;
                        req.Update();
                        siteReq.Update();
                    }
                    else
                    {
                        return this.Json(new { success = false, message = "The site request was not found" });
                    }

                    try
                    {
                        clientContext.Web.Update();
                        clientContext.ExecuteQuery();

                        LogHelper.LogInformation("Request updated", LogEventID.InformationWrite);
                        return this.Json(new { success = true, message = "The site request has been cancelled" });
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex);
                        return this.Json(new { success = false, message = "The request got terminated abruptly" });
                    }
                }

                return this.Json(new { success = false, message = "The request got terminated abruptly" });
            }
        }

        /// <summary>
        /// Provides request data for matching SP SiteRequest List entry Id
        /// </summary>
        /// <param name="requestId">The SP List Id of the request to be edited</param>
        /// <returns>Data for Editing</returns>
        [SharePointContextFilter]
        [System.Web.Mvc.HttpGet]
        public ActionResult GetRequestData([System.Web.Http.FromUri] int requestId)
        {
            LogHelper.LogInformation("GetRequestData: Requested data for request Id: " + requestId, JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            JCI.CAM.SiteProvisioningAppWeb.Models.Request request = new JCI.CAM.SiteProvisioningAppWeb.Models.Request();

            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = context.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();
                    var siteReq =
                        (from s in lists where s.Title.ToLower() == Constants.SiteRequestListName.ToLower() select s)
                            .FirstOrDefault();
                    ListItem req = siteReq.GetItemById(requestId);
                    LogHelper.LogInformation("Fetching request details");
                    clientContext.Load(req);
                    clientContext.ExecuteQuery();

                    request.requestId = req.Id;
                    request.siteType = (string)req[Constants.SiteTypeField];
                    request.siteName = (string)req[Constants.TitleField];
                    request.description = (string)req[Constants.SiteDescriptionField];
                    Microsoft.SharePoint.Client.FieldUrlValue siteURL =
                        (Microsoft.SharePoint.Client.FieldUrlValue)req[Constants.SiteUrlField];

                    LogHelper.LogInformation("Request URL is: " + siteURL.Url);
                    request.siteURL = siteURL.Url.Split(new char[] { '/' }).Last();

                    // ToDo: Get thie from site templates
                    // request.siteTemplate = (string)req[Constants.SiteTemplateId];
                    request.managedPath = siteURL.Url.Split(new char[] { '/' }).Reverse().Take(2).Last();
                    var webAppUrlSB = new StringBuilder();
                    var webAppUrlChunks = siteURL.Url.Split(new char[] { '/' }).Reverse().Skip(2).Reverse();

                    foreach (var chunks in webAppUrlChunks)
                    {
                        webAppUrlSB.Append(chunks + "/");
                    }

                    request.webAppUrl = webAppUrlSB.ToString();
                    request.confidentiality = (string)req[Constants.ConfidentialityLevelField];
                    request.siteBU = (string)req[Constants.BusinessUnitSiteProperty];
                    request.siteCountry = (string)req[Constants.CountryField];
                    request.siteRegion = (string)req[Constants.RegionSiteProperty];

                    if (req[Constants.ProjectStartDate] != null && request.startDate.Year > 1980)
                    {
                        request.startDate = (DateTime)req[Constants.ProjectStartDate];
                    }
                    else
                    {
                        request.startDate = DateTime.Now;
                    }

                    if (req[Constants.AnticipatedProjectEndDate] != null)
                    {
                        request.endDate = (DateTime)req[Constants.AnticipatedProjectEndDate];
                    }
                    else
                    {
                        request.endDate = DateTime.Now;
                    }

                    request.isAgreed = true;
                    if (req[Constants.CommunitySiteTypeField] != null)
                    {
                        request.communitySiteType = (string)req[Constants.CommunitySiteTypeField];
                    }

                    request.projectNumber = (string)req[Constants.ClarityProjectNumberField];

                    if (req[Constants.IsTestSite].ToString() == Constants.TestSiteYes)
                    {
                        request.IsTestSite = true;
                    }
                    else
                    {
                        request.IsTestSite = false;
                    }

                    List<PeoplePickerUser> primaryOwners = new List<PeoplePickerUser>();
                    List<Microsoft.SharePoint.Client.FieldUserValue> owners = new List<FieldUserValue>();

                    try
                    {
                        owners = (List<Microsoft.SharePoint.Client.FieldUserValue>)req[Constants.PrimaryOwnerField];
                    }
                    catch
                    {
                        owners.Add((Microsoft.SharePoint.Client.FieldUserValue)req[Constants.PrimaryOwnerField]);
                    }

                    foreach (var owner in owners)
                    {
                        PeoplePickerUser user = new PeoplePickerUser()
                        {
                            Login = owner.LookupValue.ToString(),
                            Email = owner.Email,
                            Name = owner.LookupValue,
                            LookupId = owner.LookupId
                        };
                        primaryOwners.Add(user);
                    }

                    request.primaryOwner = JsonConvert.SerializeObject(primaryOwners);
                    var secondaryOwner = req[Constants.SecondaryOwnerField];
                    request.secondaryOwner = JsonConvert.SerializeObject(secondaryOwner);
                }
            }

            return this.Json(request, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Provides Form data for the request form
        /// </summary>
        /// <returns>Data for Form</returns>
        [SharePointContextFilter]
        [System.Web.Mvc.HttpGet]
        public ActionResult GetFormData()
        {
            LogHelper.LogInformation("GetFormData: Form data requested", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            try
            {
                var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                using (var clientContext = context.CreateUserClientContextForSPHost())
                {
                    User currentUser = clientContext.Web.CurrentUser;

                    clientContext.Load(currentUser, user => user.Title, user => user.Email, user => user.UserId, user => user.LoginName, user => user.Id);
                    clientContext.ExecuteQuery();

                    PeoplePickerUser admin = new PeoplePickerUser()
                    {
                        Email = currentUser.Email,
                        Login = currentUser.LoginName,
                        LookupId = currentUser.Id,
                        Name = currentUser.Title
                    };

                    Collection<PeoplePickerUser> users = new Collection<PeoplePickerUser> { admin };
                    Common.Models.UserProfile userProfile = null;
                    try
                    {
                        userProfile = Common.SPHelpers.UserProfileHelper.GetUserProfile(clientContext);
                    }
                    catch (Exception ex)
                    {
                        userProfile = new Common.Models.UserProfile()
                        {
                            BusinessUnit = string.Empty,
                            ContactEmail = string.Empty,
                            ContactName = string.Empty,
                            ContactPhone = string.Empty,
                            Region = string.Empty,
                            Country = string.Empty,
                        };
                        LogHelper.LogInformation("Unable to fetch user profile");
                        LogHelper.LogError(ex);
                    }

                    Collection<string> businessUnitList = TaxonomyHelper.GetManagedMetadataItems(clientContext, Constants.MetadataGroupName, Constants.TermsetNameBusinessUnit);
                    Collection<string> regionList = TaxonomyHelper.GetManagedMetadataItems(clientContext, Constants.MetadataGroupName, Constants.TermsetNameRegion);
                    Collection<string> countryList = TaxonomyHelper.GetManagedMetadataItems(clientContext, Constants.MetadataGroupName, Constants.TermsetNameCountry);
                    Collection<string> classificationLevels = TaxonomyHelper.GetManagedMetadataItems(clientContext, Constants.MetadataGroupName, Constants.SiteClassificationLevel);

                    return this.Json(
                        new
                        {
                            businessUnits = businessUnitList,
                            countries = countryList,
                            regions = regionList,
                            classificationLevels = classificationLevels,
                            siteTemplates = this.GetSiteTemplates(),
                            user = users,
                            currentBu = userProfile.BusinessUnit,
                            currentCountry = userProfile.Country,
                            currentRegion = userProfile.Region
                        },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                return this.Json(new { result = false, message = "Failed to get form data" });
            }
        }

        /// <summary>
        /// Returns available site templates and related data
        /// </summary>
        /// <returns>site templates</returns>
        public IEnumerable<SiteTemplate> GetSiteTemplates()
        {
            LogHelper.LogInformation("GetSiteTemplates: Getting Site Templates", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            try
            {
                List<SiteTemplate> templates = new List<SiteTemplate>();
                var xmlFile = ConfigurationManager.AppSettings["SiteTemplates"]; // Set it to "~/SiteTemplates/Templates.config"
                XDocument doc = XDocument.Load(Server.MapPath(xmlFile));
                IEnumerable<XElement> temps = from ts in doc.Descendants("Template") select ts;
                foreach (var t in temps)
                {
                    if (t.Attribute("Enabled").Value == "true")
                    {
                        SiteTemplate template = new SiteTemplate()
                        {
                            Name = t.Attribute("Title").Value,
                            Description = t.Value,
                            BaseId = t.Attribute("BaseId").Value,
                            TypeIcon = t.Attribute("TypeIcon").Value,
                            ManagedPath = t.Attribute("ManagedPath").Value,
                            WebappUrl = t.Attribute("WebAppPath").Value,
                            Id = t.Attribute("ID").Value
                        };
                        templates.Add(template);
                    }
                }

                return templates;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Saves an edited site request
        /// </summary>
        /// <param name="request">the request list id</param>
        /// <param name="info">Additional information text</param>
        /// <returns>Returns True or False based on the success-failure of the request</returns>
        [SharePointContextFilter]
        [System.Web.Mvc.HttpPost]
        public ActionResult EditRequest(int request, string info)
        {
            LogHelper.LogInformation("SaveRequest: Edited request received for Request ID: " + request, JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            ListCollection lists = null;
            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = context.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();
                    var siteReq =
                        (from s in lists where s.Title.ToLower() == Constants.SiteRequestListName.ToLower() select s)
                            .FirstOrDefault();

                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.Load(web.Webs);
                    clientContext.ExecuteQuery();

                    ListItem req = siteReq.GetItemById(request);
                    clientContext.Load(req);
                    clientContext.ExecuteQuery();

                    LogHelper.LogInformation("updating  information", LogEventID.InformationWrite);
                    req[Constants.AdditionalInfoField] = info;

                    LogHelper.LogInformation("updating  workflow status", LogEventID.InformationWrite);
                    req[Constants.WorkflowStatusField] = Constants.WorkflowStatusInformationProvided;

                    LogHelper.LogInformation("Attemting to save request", LogEventID.InformationWrite);
                    req.Update();
                    try
                    {
                        clientContext.ExecuteQuery();
                        LogHelper.LogInformation("Request updated", LogEventID.InformationWrite);
                        return this.Json(new { success = true, message = "The request has been updated successfully" });
                    }
                    catch
                    {
                        return this.Json(new { success = false, message = "The request cannot be saved" });
                    }
                }

                return this.Json(new { success = false, message = "The request got terminated abruptly" });
            }
        }

        /// <summary>
        /// Saves an edited site request
        /// </summary>
        /// <param name="request">A Request object containing edited site request data</param>
        /// <returns>Returns True or False based on the success-failure of the request</returns>
        ////[SharePointContextFilter]
        ////[System.Web.Mvc.HttpPost]
        ////public ActionResult SaveRequest(Request request)
        ////{
        ////    LogHelper.LogInformation("SaveRequest: Edited request received for Request ID: " + request.requestId, JCI.CAM.Common.Logging.LogEventID.InformationWrite);

        ////    // TODO: Ask for confirmation on the page to the user with site details
        ////    ListCollection lists = null;

        ////    // Parse primary owner and secondary owners
        ////    System.Collections.Generic.List<PeoplePickerUser> primaryOwners =
        ////        new System.Collections.Generic.List<PeoplePickerUser>();
        ////    System.Collections.Generic.List<PeoplePickerUser> secondaryOwners =
        ////        new System.Collections.Generic.List<PeoplePickerUser>();

        ////    // Primary Owners
        ////    dynamic owners = System.Web.Helpers.Json.Decode(request.primaryOwner);
        ////    foreach (var o in owners)
        ////    {
        ////        PeoplePickerUser people = new PeoplePickerUser()
        ////        {
        ////            Email = o.Email,
        ////            Name = o.Name,
        ////            LookupId = o.LookupId,
        ////            Login = o.Login
        ////        };
        ////        primaryOwners.Add(people);
        ////    }

        ////    // Secondary Owners
        ////    owners = System.Web.Helpers.Json.Decode(request.secondaryOwner);
        ////    foreach (var o in owners)
        ////    {
        ////        PeoplePickerUser people = new PeoplePickerUser()
        ////        {
        ////            Email = o.Email,
        ////            Name = o.Name,
        ////            LookupId = o.LookupId,
        ////            Login = o.Login
        ////        };
        ////        secondaryOwners.Add(people);
        ////    }

        ////    var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
        ////    using (var clientContext = context.CreateUserClientContextForSPHost())
        ////    {
        ////        if (clientContext != null)
        ////        {
        ////            lists = clientContext.Web.Lists;
        ////            clientContext.Load(lists);
        ////            clientContext.ExecuteQuery();
        ////            var siteReq =
        ////                (from s in lists where s.Title.ToLower() == Constants.SiteRequestListName.ToLower() select s)
        ////                    .FirstOrDefault();

        ////            var web = clientContext.Web;
        ////            clientContext.Load(web);
        ////            clientContext.Load(web.Webs);
        ////            clientContext.ExecuteQuery();
        ////            var subWeb =
        ////                (from w in web.Webs where w.Title.ToLower() == request.siteName.ToLower() select w)
        ////                    .SingleOrDefault();
        ////            if (subWeb != null)
        ////            {
        ////                // site found
        ////                LogHelper.LogInformation("Requested site already exists, saving changed request failed", LogEventID.InformationWrite);
        ////                return this.Json(new { result = false, message = "The site already exists" });
        ////            }

        ////            ListItem req = siteReq.GetItemById(request.requestId);
        ////            clientContext.Load(req);
        ////            clientContext.ExecuteQuery();

        ////            User currentUser = null;
        ////            JCI.CAM.Common.Models.UserProfile profile;
        ////            try
        ////            {
        ////                LogHelper.LogInformation("Loading current user information", LogEventID.InformationWrite);
        ////                clientContext.Load(currentUser, user => user.Title, user => user.Email);
        ////                clientContext.ExecuteQuery();
        ////                LogHelper.LogInformation("Loading current user profile", LogEventID.InformationWrite);
        ////                profile = UserProfileHelper.GetUserProfile(clientContext);
        ////            }
        ////            catch
        ////            {
        ////                return this.Json(new { result = false, message = "Failed to load profile of the requestor" });
        ////            }

        ////            List<FieldUserValue> usersList = new List<FieldUserValue>();
        ////            foreach (PeoplePickerUser p in primaryOwners)
        ////            {
        ////                User newUser = web.EnsureUser(p.Login);

        ////                try
        ////                {
        ////                    clientContext.Load(newUser, user => user.Id);
        ////                    clientContext.ExecuteQuery();
        ////                    FieldUserValue userValue = new FieldUserValue();
        ////                    userValue.LookupId = newUser.Id;
        ////                    usersList.Add(userValue);
        ////                }
        ////                catch (Exception ex)
        ////                {
        ////                    LogHelper.LogError(ex);
        ////                    return this.Json(new { result = false, message = "Unable to resolve primary users list" });
        ////                }
        ////            }

        ////            List<FieldUserValue> secOwners = new List<FieldUserValue>();
        ////            if (secondaryOwners.Count > 0)
        ////            {
        ////                foreach (PeoplePickerUser p in secondaryOwners)
        ////                {
        ////                    User newUser = web.EnsureUser(p.Login);
        ////                    try
        ////                    {
        ////                        clientContext.Load(newUser);
        ////                        clientContext.ExecuteQuery();
        ////                        FieldUserValue userValue = new FieldUserValue { LookupId = newUser.Id };
        ////                        secOwners.Add(userValue);
        ////                    }
        ////                    catch (Exception ex)
        ////                    {
        ////                        LogHelper.LogError(ex);
        ////                        LogHelper.LogInformation("Unable to resolve additional owners list", LogEventID.InformationWrite);
        ////                        return this.Json(new { result = false, message = "Unable to resolve additional owners list" });
        ////                    }
        ////                }
        ////            }

        ////            req[Constants.TitleField] = request.siteName;
        ////            req[Constants.SiteTypeField] = request.siteType;
        ////            req[Constants.SiteDescriptionField] = request.description;
        ////            req[Constants.AnticipatedProjectEndDate] = request.endDate;
        ////            req[Constants.ProjectStartDate] = request.startDate;
        ////            req[Constants.ProjectStartDate] = request.startDate;
        ////            req[Constants.AnticipatedProjectEndDate] = request.endDate;
        ////            req[Constants.BusinessUnitSiteProperty] = request.siteBU;
        ////            req[Constants.ClarityProjectNumberField] = request.projectNumber;
        ////            req[Constants.ContactName] = currentUser.Title;
        ////            try
        ////            {
        ////                if (currentUser.Email == null)
        ////                {
        ////                    clientContext.Load(currentUser);
        ////                    clientContext.ExecuteQuery();
        ////                    req[Constants.ContactEmail] = currentUser.Email ?? string.Empty;
        ////                }
        ////            }
        ////            catch (Exception ex)
        ////            {
        ////                LogHelper.LogError(ex);
        ////            }

        ////            req[Constants.PrimaryOwnerField] = usersList;
        ////            req[Constants.SecondaryOwnerField] = usersList;
        ////            req[Constants.ContactPhone] = string.Empty;
        ////            req[Constants.ManagerName] = string.Empty;
        ////            req[Constants.RegionSiteProperty] = request.siteRegion;
        ////            req[Constants.SiteUrlField] = request.webAppUrl + request.managedPath + "/" + request.siteURL;
        ////            req[Constants.CountryField] = request.siteCountry;

        ////            if (request.siteTemplate == Constants.SiteTemplateIdCommunitySite ||
        ////                request.siteTemplate == Constants.PartnerSiteTemplateIdCommunitySite)
        ////            {
        ////                req[Constants.CommunitySiteTypeField] = request.communitySiteType;
        ////            }
        ////            else
        ////            {
        ////                req[Constants.ConfidentialityLevelField] = request.confidentiality;
        ////            }

        ////            if (request.IsTestSite == true)
        ////            {
        ////                req[Constants.IsTestSite] = Constants.TestSiteYes;
        ////            }
        ////            else
        ////            {
        ////                req[Constants.IsTestSite] = Constants.TestSiteNo;
        ////            }

        ////            // Pull this from user profile
        ////            LogHelper.LogInformation("Filling requestor profile attributes", LogEventID.InformationWrite);
        ////            if (profile != null)
        ////            {
        ////                if (profile.ContactName != null)
        ////                {
        ////                    req[Constants.ContactName] = currentUser.Title;
        ////                }

        ////                if (profile.ContactPhone != null)
        ////                {
        ////                    req[Constants.ContactPhone] = profile.ContactPhone;
        ////                }

        ////                if (profile.ManangerName != null)
        ////                {
        ////                    req[Constants.ManagerName] = profile.ManangerName;
        ////                }

        ////                if (profile.RequestorDepartmentField != null)
        ////                {
        ////                    req[Constants.RequestorDepartmentField] = profile.RequestorDepartmentField;
        ////                }

        ////                if (profile.RequestorFacilityField != null)
        ////                {
        ////                    req[Constants.RequestorFacilityField] = profile.RequestorFacilityField;
        ////                }

        ////                if (profile.RequestorGlobalIdField != null)
        ////                {
        ////                    req[Constants.RequestorGlobalIdField] = profile.RequestorGlobalIdField;
        ////                }

        ////                if (profile.ContactEmail != null)
        ////                {
        ////                    req[Constants.ContactEmail] = currentUser.Email;
        ////                }
        ////            }

        ////            LogHelper.LogInformation("Assigning workflow values", LogEventID.InformationWrite);
        ////            req[Constants.WorkflowStatusField] = Constants.StatusFieldNew;
        ////            req[Constants.RequestStatusField] = Constants.StatusFieldNew;

        ////            // ToDo: Implement Taxonomy Picker
        ////            // Partner & Community site based logic
        ////            if (request.siteTemplate.StartsWith(Constants.PartnerSiteTemplateIdStartsWith))
        ////            {
        ////                // Microsoft.SharePoint.Taxonomy.TaxonomyFieldValueCollection customerValueCollection = new Microsoft.SharePoint.Taxonomy.TaxonomyFieldValueCollection(string.Empty);
        ////                // customerValueCollection.PopulateFromLabelGuidPairs(TaxonomyWebTaggingControlCustomer.Text);
        ////                // req[Constants.SiteCustomersField] = customerValueCollection;
        ////            }

        ////            LogHelper.LogInformation("Attemting to save request", LogEventID.InformationWrite);
        ////            req.Update();
        ////            try
        ////            {
        ////                clientContext.ExecuteQuery();
        ////                LogHelper.LogInformation("Request updated", LogEventID.InformationWrite);
        ////                return this.Json(new { result = true, message = "The request has been updated successfully" });
        ////            }
        ////            catch
        ////            {
        ////                return this.Json(new { result = false, message = "The request cannot be saved" });
        ////            }
        ////        }

        ////        return this.Json(new { result = false, message = "The request got terminated abruptly" });
        ////    }
        ////}

        /// <summary>
        /// Saves an edited site request
        /// </summary>
        /// <param name="request">A Request object containing edited site request data</param>
        /// <returns>Returns True or False based on the success-failure of the request</returns>
        ////[System.Web.Mvc.HttpGet]
        ////public ActionResult SaveRequest2(Request request)
        ////{
        ////if(context == null)
        ////{
        ////    Uri sharePointSiteUrl = new Uri("https://inullable.sharepoint.com");

        ////    if (Request.QueryString["code"] != null)
        ////    {
        ////        TokenCache.UpdateCacheWithCode(HttpContext.Request, HttpContext.Response, sharePointSiteUrl);
        ////    }

        ////    if (!TokenCache.IsTokenInCache(Request.Cookies))
        ////    {
        ////        Response.Redirect(TokenHelper.GetAuthorizationUrl(sharePointSiteUrl.ToString(),
        ////                                                          "Web.Write"));
        ////    }
        ////    else
        ////    {
        ////        string refreshToken = TokenCache.GetCachedRefreshToken(Request.Cookies);
        ////        string accessToken =
        ////        TokenHelper.GetAccessToken(
        ////                   refreshToken,
        ////                   "00000003-0000-0ff1-ce00-000000000000",
        ////                   sharePointSiteUrl.Authority,
        ////                   TokenHelper.GetRealmFromTargetUrl(sharePointSiteUrl)).AccessToken;

        ////        using (ClientContext context2 =
        ////               TokenHelper.GetClientContextWithAccessToken(sharePointSiteUrl.ToString(),
        ////                                                           accessToken))
        ////        {
        ////            context2.Load(context2.Web);
        ////            context2.ExecuteQuery();

        ////            Response.Write("<p>" + context2.Web.Title + "</p>");
        ////        }
        ////    }
        ////    return this.View();

        ////}
        ////}

        /// <summary>
        /// Puts a status to confirm that site is running
        /// </summary>
        /// <param name="request">Dummy Object</param>
        /// <returns>string - Site Running</returns>
        [System.Web.Mvc.HttpGet]
        public ActionResult GetStatus(Request request)
        {
            HttpContext.Response.Write("Site Running");
            return this.View();
        }
    }
}