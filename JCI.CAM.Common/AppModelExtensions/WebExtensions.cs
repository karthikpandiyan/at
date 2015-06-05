//-----------------------------------------------------------------------
// <copyright file= "WebExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{    
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.Utilities;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Search.Query;

    /// <summary>
    /// Web extensions
    /// </summary>
    public static partial class WebExtensions
    {
        /// <summary>
        /// The index property key
        /// </summary>
        private const string INDEXEDPROPERTYKEY = "vti_indexedpropertykeys";

        #region Web (site) query, creation and deletion

        /// <summary>
        /// Adds a new child Web (site) to a parent Web.
        /// </summary>
        /// <param name="parentWeb">The parent Web (site) to create under</param>
        /// <param name="title">The title of the new site.</param>
        /// <param name="leafUrl">A string that represents the URL leaf name.</param>
        /// <param name="description">The description of the new site.</param>
        /// <param name="template">The name of the site template to be used for creating the new site.</param>
        /// <param name="language">The locale ID that specifies the language of the new site.</param>
        /// <param name="inheritPermissions">Specifies whether the new site will inherit permissions from its parent site.</param>
        /// <param name="inheritNavigation">Specifies whether the site inherits navigation.</param>
        /// <returns>return web</returns>
        /// <exception cref="System.ArgumentException">The argument must be a single web URL and cannot contain path characters.;leafUrl</exception>
        public static Web CreateWeb(this Web parentWeb, string title, string leafUrl, string description, string template, int language, bool inheritPermissions = true, bool inheritNavigation = true)
        {
            LogHelper.LogInformation("Create web.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
           
            if (leafUrl.Contains('/') || leafUrl.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single web URL and cannot contain path characters.", "leafUrl");
            }

            WebCreationInformation creationInfo = new WebCreationInformation()
            {
                Url = leafUrl,
                Title = title,
                Description = description,
                UseSamePermissionsAsParentSite = true,
                WebTemplate = template,
                Language = language
            };

            Web newWeb = parentWeb.Webs.Add(creationInfo);

            if (!inheritPermissions)
            {
                LogHelper.LogInformation("Breaking the role inhertance from parent...", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                
                // Breaking inheritance and inheriting the current role assignments from the parent site 
                newWeb.BreakRoleInheritance(true, false);
                LogHelper.LogInformation("Subsite has unique permissions-.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }

            newWeb.Navigation.UseShared = inheritNavigation;
            newWeb.Update();

            parentWeb.Context.ExecuteQuery();
            LogHelper.LogInformation("Web created.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            return newWeb;
        }

        /// <summary>
        /// Determines if a child Web site with the specified leaf URL exists.
        /// </summary>
        /// <param name="parentWeb">The Web site to check under</param>
        /// <param name="leafUrl">A string that represents the URL leaf name.</param>
        /// <returns>
        /// true if the Web (site) exists; otherwise false
        /// </returns>
        /// <exception cref="System.ArgumentException">The argument must be a single web URL and cannot contain path characters.;leafUrl</exception>
        public static bool WebExists(this Web parentWeb, string leafUrl)
        {
            LogHelper.LogInformation("Check web is exited or not", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
           
            if (leafUrl.Contains('/') || leafUrl.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single web URL and cannot contain path characters.", "leafUrl");
            }

            Utility.EnsureWeb(parentWeb.Context, parentWeb, "ServerRelativeUrl");
            var serverRelativeUrl = UrlUtility.Combine(parentWeb.ServerRelativeUrl, leafUrl);
            var webs = parentWeb.Webs;
            //// NOTE: Predicate does not take into account a required case-insensitive comparison
            //// var results = parentWeb.Context.LoadQuery<Web>(webs.Where(item => item.ServerRelativeUrl == serverRelativeUrl));
            parentWeb.Context.Load(webs, wc => wc.Include(w => w.ServerRelativeUrl));
            parentWeb.Context.ExecuteQuery();
            var exists = webs.Any(item => string.Equals(item.ServerRelativeUrl, serverRelativeUrl, StringComparison.OrdinalIgnoreCase));
            LogHelper.LogInformation("Checked web exists.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            return exists;
        }

        /// <summary>
        /// Get string typed property bag value. If does not contain, returns given default value.
        /// </summary>
        /// <param name="web">Web to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Value of the property bag entry as string
        /// </returns>
        public static string GetPropertyBagValueString(this Web web, string key, string defaultValue)
        {
            object value = GetPropertyBagValueInternal(web, key);
            if (value != null)
            {
                return (string)value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Checks the user authorization.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="permissionToCheck">The permission to check.</param>
        /// <returns>
        /// True / False based on user has permission or not
        /// </returns>
        public static bool CheckCurrentUserAuthorization(this Web web, PermissionKind permissionToCheck)
        {
            User user = web.CurrentUser;
            web.Context.Load(user);
            web.Context.ExecuteQueryRetry();
            
            var permission = web.GetUserEffectivePermissions(user.LoginName);
            web.Context.ExecuteQueryRetry();
            return permission.Value.Has(permissionToCheck);
        }

        #endregion

        #region Property Bag extensions
        /// <summary>
        /// Sets a key/value pair in the web property bag
        /// </summary>
        /// <param name="web">Web that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">String value for the property bag entry</param>
        public static void SetPropertyBagValue(this Web web, string key, string value)
        {
            SetPropertyBagValueInternal(web, key, value);
        }

        /// <summary>
        /// Sets a key/value pair in the web property bag
        /// </summary>
        /// <param name="web">Web that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">Integer value for the property bag entry</param>
        public static void SetPropertyBagValue(this Web web, string key, int value)
        {
            SetPropertyBagValueInternal(web, key, value);
        }       

        /// <summary>
        /// Marks a property bag key for indexing
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="key">The key to mark for indexing</param>
        /// <returns>Returns True if succeeded</returns>
        public static bool AddIndexedPropertyBagKey(this Web web, string key)
        {
            bool result = false;
            var keys = GetIndexedPropertyBagKeys(web).ToList();
            
            if (!keys.Contains(key))
            {
                keys.Add(key);
                web.SetPropertyBagValue(INDEXEDPROPERTYKEY, GetEncodedValueForSearchIndexProperty(keys));
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Returns all keys in the property bag that have been marked for indexing
        /// </summary>
        /// <param name="web">The site to process</param>
        /// <returns>property collection</returns>
        public static IEnumerable<string> GetIndexedPropertyBagKeys(this Web web)
        {
            List<string> keys = new List<string>();

            if (web.PropertyBagContainsKey(INDEXEDPROPERTYKEY))
            {
                foreach (string key in web.GetPropertyBagValueString(INDEXEDPROPERTYKEY, string.Empty).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    byte[] bytes = Convert.FromBase64String(key);
                    keys.Add(Encoding.Unicode.GetString(bytes));
                }
            }

            return keys;
        }

        /// <summary>
        /// Checks if the given property bag entry exists
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Key of the property bag entry to check</param>
        /// <returns>True if the entry exists, false otherwise</returns>
        public static bool PropertyBagContainsKey(this Web web, string key)
        {
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQueryRetry();
            
            if (props.FieldValues.ContainsKey(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get integer typed property bag value. If does not contain, returns default value.
        /// </summary>
        /// <param name="web">Web to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Value of the property bag entry as integer
        /// </returns>
        public static int? GetPropertyBagValueInt(this Web web, string key, int defaultValue)
        {
            object value = GetPropertyBagValueInternal(web, key);
            if (value != null)
            {
                return (int)value;
            }
            else
            {
                return defaultValue;
            }
        }

        #endregion

        #region Site retrieval via search
        /// <summary>
        /// Returns all my site site collections
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns>All my site site collections</returns>
        [SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods",
            Justification = "Search Query code")]
        public static List<SiteEntity> MySiteSearch(this Web web)
        {
            string keywordQuery = string.Format("contentclass:\"STS_Site\" AND WebTemplate:SPSPERS", web.Context.Url);
            return web.SiteSearch(keywordQuery);
        }

        /// <summary>
        /// Returns the site collections that comply with the passed keyword query
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="keywordQueryValue">Keyword query</param>
        /// <param name="trimDublicates">Indicates if duplicates should be trimmed or not</param>
        /// <returns>
        /// All found site collections
        /// </returns>
        public static List<SiteEntity> SiteSearch(this Web web, string keywordQueryValue, bool trimDublicates = true)
        {
            try
            {
                LogHelper.LogInformation(string.Format("Site search '{0}'", keywordQueryValue), LogEventID.InformationWrite);

                List<SiteEntity> sites = new List<SiteEntity>();

                KeywordQuery keywordQuery = new KeywordQuery(web.Context);
                keywordQuery.TrimDuplicates = false;

                if (keywordQueryValue.Length == 0)
                {
                    keywordQueryValue = "contentclass:\"STS_Site\"";
                }

                int startRow = 0;
                int totalRows = 0;

                totalRows = web.ProcessQuery(keywordQueryValue, sites, keywordQuery, startRow);

                if (totalRows > 0)
                {
                    while (totalRows >= sites.Count)
                    {
                        startRow += 500;
                        totalRows = web.ProcessQuery(keywordQueryValue, sites, keywordQuery, startRow);
                    }
                }

                return sites;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);

                // rethrow does lose one line of stack trace, but we want to log the error at the component boundary
                throw;
            }
        }       

        // private methods

        /// <summary>
        /// Runs a query
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="keywordQueryValue">keyword query</param>
        /// <param name="sites">sites variable that hold the resulting sites</param>
        /// <param name="keywordQuery">KeywordQuery object</param>
        /// <param name="startRow">Start row of the result set to be returned</param>
        /// <returns>
        /// Total number of rows for the query
        /// </returns>
        private static int ProcessQuery(this Web web, string keywordQueryValue, List<SiteEntity> sites, KeywordQuery keywordQuery, int startRow)
        {
            int totalRows = 0;

            keywordQuery.QueryText = keywordQueryValue;
            keywordQuery.RowLimit = 500;
            keywordQuery.StartRow = startRow;
            keywordQuery.SelectProperties.Add("Title");
            keywordQuery.SelectProperties.Add("SPSiteUrl");
            keywordQuery.SelectProperties.Add("Description");
            keywordQuery.SelectProperties.Add("WebTemplate");
            keywordQuery.SortList.Add("SPSiteUrl", SortDirection.Ascending);
            SearchExecutor searchExec = new SearchExecutor(web.Context);
            ClientResult<ResultTableCollection> results = searchExec.ExecuteQuery(keywordQuery);
            web.Context.ExecuteQueryRetry();

            if (results != null)
            {
                if (results.Value[0].RowCount > 0)
                {
                    totalRows = results.Value[0].TotalRows;

                    foreach (var row in results.Value[0].ResultRows)
                    {
                        sites.Add(new SiteEntity
                        {
                            Title = row["Title"] != null ? row["Title"].ToString() : string.Empty,
                            Url = row["SPSiteUrl"] != null ? row["SPSiteUrl"].ToString() : string.Empty,
                            Description = row["Description"] != null ? row["Description"].ToString() : string.Empty,
                            Template = row["WebTemplate"] != null ? row["WebTemplate"].ToString() : string.Empty,
                        });
                    }
                }
            }

            return totalRows;
        }
        #endregion

        /// <summary>
        /// Used to convert the list of property keys is required format for listing keys to be index
        /// </summary>
        /// <param name="keys">list of keys to set to be searchable</param>
        /// <returns>string formatted list of keys in proper format</returns>
        private static string GetEncodedValueForSearchIndexProperty(IEnumerable<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string current in keys)
            {
                stringBuilder.Append(Convert.ToBase64String(Encoding.Unicode.GetBytes(current)));
                stringBuilder.Append('|');
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Type independent implementation of the property getter.
        /// </summary>
        /// <param name="web">Web to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <returns>Value of the property bag entry</returns>
        private static object GetPropertyBagValueInternal(Web web, string key)
        {
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQuery();
            if (props.FieldValues.ContainsKey(key))
            {
                return props.FieldValues[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets a key/value pair in the web property bag
        /// </summary>
        /// <param name="web">Web that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">Value for the property bag entry</param>
        private static void SetPropertyBagValueInternal(Web web, string key, object value)
        {
            var props = web.AllProperties;

            // Get the value, if the web properties are already loaded
            if (props.FieldValues.Count > 0)
            {
                props[key] = value;
            }
            else
            {
                // Load the web properties
                web.Context.Load(props);
                web.Context.ExecuteQueryRetry();

                props[key] = value;
            }

            web.Update();
            web.Context.ExecuteQueryRetry();
        }
    }
}
