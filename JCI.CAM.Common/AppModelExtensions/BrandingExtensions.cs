// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrandingExtensions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  List Extensions
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{    
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.Utilities;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Branding extension
    /// </summary>
    public static partial class BrandingExtensions
    {
        /// <summary>
        /// The regex invalid file name characters
        /// </summary>
        private const string REGEXINVALIDFILENAMECHARS = @"[<>:;*?/\\|""&%\t\r\n]";

        /// <summary>
        /// The inherit master
        /// </summary>
       private const string InheritMaster = "__InheritMasterUrl";

        /// <summary>
        /// The inherit theme
        /// </summary>
       private const string InheritTheme = "__InheritsThemedCssFolderUrl";

        /// <summary>
        /// The current look name
        /// </summary>
       private const string CurrentLookName = "Current";

        /// <summary>
        /// The CAML query find by filename
        /// </summary>
       private const string CAMLQUERYFINDBYFILENAME = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Name' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                     </Query>
                </View>";

       /// <summary>
       /// This method will disable SharePoint designer you must be a site collection administrator to perform this action
       /// A UnauthorizedAccessException is thrown when attempting to set the property if either the user is not a Site Collection administrator or the setting is disabled at the 
       /// web application level.
       /// Site Collection Administrators will always be able to edit sites. 
       /// </summary>
       /// <param name="site">Site instance</param>
       public static void DisableDesigner(Site site)
       {
           try
           {
               ////Allow Site Owners and Designers to use SharePoint Designer in this Site Collection 
               site.AllowDesigner = false;
               ////Allow Site Owners and Designers to Customize Master Pages and Page Layouts 
               site.AllowMasterPageEditing = false;
               ////Allow Site Owners and Designers to Detach Pages from the Site Definition 
               site.AllowRevertFromTemplate = false;
               ////Allow Site Owners and Designers to See the Hidden URL structure of their Web Site 
               site.ShowUrlStructure = false;
               site.Context.ExecuteQuery();
           }
           catch (Exception ex)
           {
               LogHelper.LogError(ex, LogEventID.ExceptionHandling);
           }
       }

       /// <summary>
       ///  Adding custom action
       /// </summary>
       /// <param name="site">Site Instance</param>
       /// <param name="web">Web Instance</param>
       /// <param name="actionDescription">Custom action description</param>
       /// <param name="actionLocation">Custom action location</param>
       /// <param name="scriptBlock">Custom action script block</param>
       public static void AddCustomActionJSLinks(Site site, Web web, string actionDescription, string actionLocation, string scriptBlock)
       {
               var existingActions = web.UserCustomActions;
               site.Context.Load(existingActions);
               site.Context.ExecuteQuery();
               var actions = existingActions.ToArray();

               // Deleting existing custom action
               foreach (var action in actions)
               {
                   if (action.Description == actionDescription && action.Location == actionLocation)
                   {
                       action.DeleteObject();
                       site.Context.ExecuteQuery();
                   }
               }

               var newAction = existingActions.Add();
               newAction.Description = actionDescription;
               newAction.Location = actionLocation;
               scriptBlock = ListExtensions.ReplaceUrlTokens(scriptBlock);
               newAction.ScriptBlock = scriptBlock;
               newAction.Update();
               site.Context.Load(web, s => s.UserCustomActions);
               site.Context.ExecuteQuery();
       }

        /// <summary>
        /// Checks if a composed look exists.
        /// </summary>
        /// <param name="web">Web to check</param>
        /// <param name="composedLookName">Name of the composed look</param>
        /// <returns>
        /// true if it exists; otherwise false
        /// </returns>
        public static bool ComposedLookExists(this Web web, string composedLookName)
        {
            var found = GetComposedLook(web, composedLookName);
            return found != null;
        }

        /// <summary>
        /// Returns the named composed look from the web gallery
        /// </summary>
        /// <param name="web">Web to check</param>
        /// <param name="composedLookName">Name of the composed look to retrieve</param>
        /// <returns>
        /// Entity with the attributes of the composed look, or null if it does not exist
        /// </returns>
        public static ThemeEntity GetComposedLook(this Web web, string composedLookName)
        {
            ThemeEntity theme = null;

            List designCatalog = web.GetCatalog((int)ListTemplateType.DesignCatalog);
            string camlString = @"
            <View>  
                <Query> 
                    <Where><Eq><FieldRef Name='Name' /><Value Type='Text'>{0}</Value></Eq></Where> 
                </Query> 
                <ViewFields>
                    <FieldRef Name='ImageUrl' />
                    <FieldRef Name='MasterPageUrl' />
                    <FieldRef Name='FontSchemeUrl' />
                    <FieldRef Name='ThemeUrl' />
                </ViewFields> 
            </View>";

            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(camlString, composedLookName);

            ListItemCollection themes = designCatalog.GetItems(camlQuery);
            web.Context.Load(themes);
            web.Context.ExecuteQuery();
            if (themes.Count > 0)
            {
                var themeItem = themes[0];
                theme = new ThemeEntity();
                if (themeItem["ThemeUrl"] != null && themeItem["ThemeUrl"].ToString().Length > 0)
                {
                    theme.Theme = (themeItem["ThemeUrl"] as FieldUrlValue).Url;
                }

                if (themeItem["MasterPageUrl"] != null && themeItem["MasterPageUrl"].ToString().Length > 0)
                {
                    theme.MasterPage = (themeItem["MasterPageUrl"] as FieldUrlValue).Url;
                }

                if (themeItem["FontSchemeUrl"] != null && themeItem["FontSchemeUrl"].ToString().Length > 0)
                {
                    theme.Font = (themeItem["FontSchemeUrl"] as FieldUrlValue).Url;
                }

                if (themeItem["ImageUrl"] != null && themeItem["ImageUrl"].ToString().Length > 0)
                {
                    theme.BackgroundImage = (themeItem["ImageUrl"] as FieldUrlValue).Url;
                }
            }

            return theme;
        }

        /// <summary>
        /// Creates (or updates) a composed look in the web site; usually this is done in the root site of the collection.
        /// </summary>
        /// <param name="web">Web to create the composed look in</param>
        /// <param name="lookName">Name of the theme</param>
        /// <param name="paletteFileName">File name of the palette file in the theme catalog of the site collection; path component ignored.</param>
        /// <param name="fontFileName">File name of the font file in the theme catalog of the site collection; path component ignored.</param>
        /// <param name="backgroundFileName">File name of the background image file in the theme catalog of the site collection; path component ignored.</param>
        /// <param name="masterFileName">File name of the master page in the master page catalog of the web site; path component ignored.</param>
        /// <param name="displayOrder">Display order of the composed look</param>
        /// <param name="replaceContent">Replace composed look if it already exists (default true)</param>
        public static void CreateComposedLookByName(this Web web, string lookName, string paletteFileName, string fontFileName, string backgroundFileName, string masterFileName, int displayOrder = 1, bool replaceContent = true)
        {
            var paletteUrl = default(string);
            var fontUrl = default(string);
            var backgroundUrl = default(string);
            var masterUrl = default(string);

            // using (var innerContext = new ClientContext(web.Context.Url) { Credentials = web.Context.Credentials })
            // {
            // var rootWeb = innerContext.Site.RootWeb;
            var rootWeb = web;
            Utility.EnsureWeb(web.Context, rootWeb, "ServerRelativeUrl");

            if (!string.IsNullOrEmpty(paletteFileName))
            {
                paletteUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMESDIRECTORY, Path.GetFileName(paletteFileName)));
            }

            if (!string.IsNullOrEmpty(fontFileName))
            {
                fontUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMESDIRECTORY, Path.GetFileName(fontFileName)));
            }

            if (!string.IsNullOrEmpty(backgroundFileName))
            {
                backgroundUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMESDIRECTORY, Path.GetFileName(backgroundFileName)));
            }

            // }
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }

            if (!string.IsNullOrEmpty(masterFileName))
            {
                masterUrl = UrlUtility.Combine(web.ServerRelativeUrl, string.Format(Constants.MASTERPAGEDIRECTORY, Path.GetFileName(masterFileName)));
            }

            CreateComposedLookByUrl(web, lookName, paletteUrl, fontUrl, backgroundUrl, masterUrl, displayOrder, replaceContent);
        }

        /// <summary>
        /// Creates the name of the composed look for one drive by.
        /// </summary>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="lookName">Name of the look.</param>
        /// <param name="paletteFileName">Name of the palette file.</param>
        /// <param name="fontFileName">Name of the font file.</param>
        /// <param name="backgroundFileName">Name of the background file.</param>
        /// <param name="masterFileName">Name of the master file.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <param name="replaceContent">if set to <c>true</c> [replace content].</param>
        public static void CreateComposedLookForOneDriveByName(this Web rootWeb, string lookName, string paletteFileName, string fontFileName, string backgroundFileName, string masterFileName, int displayOrder = 1, bool replaceContent = true)
        {
            var paletteUrl = default(string);
            var fontUrl = default(string);
            var backgroundUrl = default(string);
            var masterUrl = default(string);

            Utility.EnsureWeb(rootWeb.Context, rootWeb, "ServerRelativeUrl");

            if (!string.IsNullOrEmpty(paletteFileName))
            {
                paletteUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMESDIRECTORY, Path.GetFileName(paletteFileName)));
            }

            if (!string.IsNullOrEmpty(fontFileName))
            {
                fontUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMESDIRECTORY, Path.GetFileName(fontFileName)));
            }

            if (!string.IsNullOrEmpty(backgroundFileName))
            {
                backgroundUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.THEMESDIRECTORY, Path.GetFileName(backgroundFileName)));
            }

            if (!rootWeb.IsPropertyAvailable("ServerRelativeUrl"))
            {
                rootWeb.Context.Load(rootWeb, w => w.ServerRelativeUrl);
                rootWeb.Context.ExecuteQuery();
            }

            if (!string.IsNullOrEmpty(masterFileName))
            {
                masterUrl = UrlUtility.Combine(rootWeb.ServerRelativeUrl, string.Format(Constants.MASTERPAGEDIRECTORY, Path.GetFileName(masterFileName)));
            }

            CreateComposedLookByUrl(rootWeb, lookName, paletteUrl, fontUrl, backgroundUrl, masterUrl, displayOrder, replaceContent);
        }

        /// <summary>
        /// Creates (or updates) a composed look in the web site; usually this is done in the root site of the collection.
        /// </summary>
        /// <param name="web">Web to create the composed look in</param>
        /// <param name="lookName">Name of the theme</param>
        /// <param name="paletteServerRelativeUrl">URL of the palette file, usually in the theme catalog of the site collection</param>
        /// <param name="fontServerRelativeUrl">URL of the font file, usually in the theme catalog of the site collection</param>
        /// <param name="backgroundServerRelativeUrl">URL of the background image file, usually in /_layouts/15/images</param>
        /// <param name="masterServerRelativeUrl">URL of the master page, usually in the master page catalog of the web site</param>
        /// <param name="displayOrder">Display order of the composed look</param>
        /// <param name="replaceContent">Replace composed look if it already exists (default true)</param>
        /// <exception cref="System.Exception">Composed look already exists, replace contents needs to be specified.</exception>
        public static void CreateComposedLookByUrl(this Web web, string lookName, string paletteServerRelativeUrl, string fontServerRelativeUrl, string backgroundServerRelativeUrl, string masterServerRelativeUrl, int displayOrder = 1, bool replaceContent = true)
        {
            Utility.EnsureWeb(web.Context, web, "ServerRelativeUrl");
            var composedLooksList = web.GetCatalog((int)ListTemplateType.DesignCatalog);

            // Check for existing, by name
            CamlQuery query = new CamlQuery();
            query.ViewXml = string.Format(CAMLQUERYFINDBYFILENAME, lookName);
            var existingCollection = composedLooksList.GetItems(query);
            web.Context.Load(existingCollection);
            web.Context.ExecuteQuery();
            ListItem item = existingCollection.FirstOrDefault();

            if (item == null)
            {
                //// LogHelper.LogInformation(( int )EventId.CreateComposedLook, CoreResources.BrandingExtension_CreateComposedLook, lookName, web.ServerRelativeUrl);
                ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                item = composedLooksList.AddItem(itemInfo);
                item["Name"] = lookName;
                item["Title"] = lookName;
            }
            else
            {
                if (!replaceContent)
                {
                    throw new System.Exception("Composed look already exists, replace contents needs to be specified.");
                }
                //// LoggingUtility.Internal.TraceInformation(( int )EventId.UpdateComposedLook, CoreResources.BrandingExtension_UpdateComposedLook, lookName, web.ServerRelativeUrl);
            }

            if (!string.IsNullOrEmpty(paletteServerRelativeUrl))
            {
                item["ThemeUrl"] = paletteServerRelativeUrl;
            }

            if (!string.IsNullOrEmpty(fontServerRelativeUrl))
            {
                item["FontSchemeUrl"] = fontServerRelativeUrl;
            }

            if (!string.IsNullOrEmpty(backgroundServerRelativeUrl))
            {
                item["ImageUrl"] = backgroundServerRelativeUrl;
            }

            // we use seattle master if anything else is not set
            if (string.IsNullOrEmpty(masterServerRelativeUrl))
            {
                item["MasterPageUrl"] = UrlUtility.Combine(web.ServerRelativeUrl, Constants.MASTERPAGESEATTLE);
            }
            else
            {
                item["MasterPageUrl"] = masterServerRelativeUrl;
            }

            item["DisplayOrder"] = displayOrder;
            item.Update();
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Retrieves the named composed look, overrides with specified palette, font, background and master page, and then recursively sets the specified values.
        /// </summary>
        /// <param name="web">Web to apply composed look to</param>
        /// <param name="lookName">Name of the composed look to apply; null will apply the override values only</param>
        /// <param name="paletteServerRelativeUrl">Override palette file URL to use</param>
        /// <param name="fontServerRelativeUrl">Override font file URL to use</param>
        /// <param name="backgroundServerRelativeUrl">Override background image file URL to use</param>
        /// <param name="masterServerRelativeUrl">Override master page file URL to use</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting sub sites only; true to force all sub sites to inherit</param>
        /// <exception cref="System.Exception">Throw exception</exception>
        public static void SetComposedLookByUrl(this Web web, string lookName, string paletteServerRelativeUrl = null, string fontServerRelativeUrl = null, string backgroundServerRelativeUrl = null, string masterServerRelativeUrl = null, bool resetSubsitesToInherit = false)
        {
            var paletteUrl = default(string);
            var fontUrl = default(string);
            var backgroundUrl = default(string);
            var masterUrl = default(string);

            if (!string.IsNullOrWhiteSpace(lookName))
            {
                var composedLooksList = web.GetCatalog((int)ListTemplateType.DesignCatalog);

                // Check for existing, by name
                CamlQuery query = new CamlQuery();
                query.ViewXml = string.Format(CAMLQUERYFINDBYFILENAME, lookName);
                var existingCollection = composedLooksList.GetItems(query);
                web.Context.Load(existingCollection);
                web.Context.ExecuteQuery();
                var item = existingCollection.FirstOrDefault();

                if (item != null)
                {
                    var lookPaletteUrl = item["ThemeUrl"] as FieldUrlValue;
                    if (lookPaletteUrl != null)
                    {
                        paletteUrl = HttpUtility.UrlDecode(new Uri(lookPaletteUrl.Url).AbsolutePath);
                    }

                    var lookFontUrl = item["FontSchemeUrl"] as FieldUrlValue;
                    if (lookFontUrl != null)
                    {
                        fontUrl = HttpUtility.UrlDecode(new Uri(lookFontUrl.Url).AbsolutePath);
                    }

                    var lookBackgroundUrl = item["ImageUrl"] as FieldUrlValue;
                    if (lookBackgroundUrl != null)
                    {
                        backgroundUrl = HttpUtility.UrlDecode(new Uri(lookBackgroundUrl.Url).AbsolutePath);
                    }

                    var lookMasterUrl = item["MasterPageUrl"] as FieldUrlValue;
                    if (lookMasterUrl != null)
                    {
                        masterUrl = HttpUtility.UrlDecode(new Uri(lookMasterUrl.Url).AbsolutePath);
                    }
                }
                else
                {
                    //// LoggingUtility.Internal.TraceError(( int )EventId.ThemeMissing, CoreResources.BrandingExtension_ComposedLookMissing, lookName);
                    throw new System.Exception(string.Format("Composed look '{0}' can not be found; pass null or empty to set look directly (not based on an existing entry)", lookName));
                }
            }

            if (!string.IsNullOrEmpty(paletteServerRelativeUrl))
            {
                paletteUrl = paletteServerRelativeUrl;
            }

            if (!string.IsNullOrEmpty(fontServerRelativeUrl))
            {
                fontUrl = fontServerRelativeUrl;
            }

            if (!string.IsNullOrEmpty(backgroundServerRelativeUrl))
            {
                backgroundUrl = backgroundServerRelativeUrl;
            }

            if (!string.IsNullOrEmpty(masterServerRelativeUrl))
            {
                masterUrl = masterServerRelativeUrl;
            }

            // Save as 'current'
            web.CreateComposedLookByUrl(CurrentLookName, paletteUrl, fontUrl, backgroundUrl, masterUrl, displayOrder: 0);
            web.SetMasterPageByUrl(masterUrl, resetSubsitesToInherit);
            web.SetCustomMasterPageByUrl(masterUrl, resetSubsitesToInherit);
            web.SetThemeByUrl(paletteUrl, fontUrl, backgroundUrl, resetSubsitesToInherit);
        }

        /// <summary>
        /// Recursively applies the specified palette, font, and background image.
        /// </summary>
        /// <param name="web">Web to apply to</param>
        /// <param name="paletteServerRelativeUrl">URL of palette file to apply</param>
        /// <param name="fontServerRelativeUrl">URL of font file to apply</param>
        /// <param name="backgroundServerRelativeUrl">URL of background image to apply</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting sub sites only; true to force all sub sites to inherit</param>
        /// <param name="updateRootOnly">false (default) to apply to sub sites; true to only apply to specified site</param>
        public static void SetThemeByUrl(this Web web, string paletteServerRelativeUrl, string fontServerRelativeUrl, string backgroundServerRelativeUrl, bool resetSubsitesToInherit = false, bool updateRootOnly = false)
        {
            var websToUpdate = new List<Web>();
            web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            //// LoggingUtility.Internal.TraceInformation(( int )EventId.SetTheme, CoreResources.BrandingExtension_ApplyTheme, paletteServerRelativeUrl, web.ServerRelativeUrl);
            web.AllProperties[InheritTheme] = "False";
            web.Update();
            web.ApplyTheme(paletteServerRelativeUrl, fontServerRelativeUrl, backgroundServerRelativeUrl, shareGenerated: true);
            web.Context.ExecuteQuery();            
            websToUpdate.Add(web);

            if (!updateRootOnly)
            {
                var index = 0;
                while (index < websToUpdate.Count)
                {
                    var currentWeb = websToUpdate[index];
                    var websCollection = currentWeb.Webs;
                    web.Context.Load(websCollection, wc => wc.Include(w => w.AllProperties, w => w.ServerRelativeUrl));
                    web.Context.ExecuteQuery();
                    foreach (var childWeb in websCollection)
                    {
                        var inheritThemeProperty = childWeb.GetPropertyBagValueString(InheritTheme, string.Empty);
                        bool inheritTheme = false;
                        if (!string.IsNullOrEmpty(inheritThemeProperty))
                        {
                            inheritTheme = string.Equals(childWeb.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase);
                        }

                        if (resetSubsitesToInherit || inheritTheme)
                        {                            
                            childWeb.AllProperties[InheritTheme] = "True";                           
                            childWeb.Update();
                            //// TODO: CSOM does not support the ThemedCssFolderUrl property yet (Nov 2014), so must call ApplyTheme at each level.
                            //// This is very slow, so replace with simply setting the ThemedCssFolderUrl property instead once available.
                            childWeb.ApplyTheme(paletteServerRelativeUrl, fontServerRelativeUrl, backgroundServerRelativeUrl, shareGenerated: true);
                            web.Context.ExecuteQuery();
                            websToUpdate.Add(childWeb);
                        }
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// Set master page by using given URL as parameter. Suitable for example in cases where you want sub sites to reference root site master page gallery. This is typical with publishing sites.
        /// </summary>
        /// <param name="web">Context web</param>
        /// <param name="masterPageServerRelativeUrl">URL to the master page.</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting sub sites only; true to force all sub sites to inherit</param>
        /// <param name="updateRootOnly">false (default) to apply to sub sites; true to only apply to specified site</param>
        /// <exception cref="System.ArgumentNullException">master Page Url</exception>
        public static void SetMasterPageByUrl(this Web web, string masterPageServerRelativeUrl, bool resetSubsitesToInherit = false, bool updateRootOnly = false)
        {
            if (string.IsNullOrEmpty(masterPageServerRelativeUrl)) 
            { 
                throw new ArgumentNullException("masterPageUrl"); 
            }

            var websToUpdate = new List<Web>();
            web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            // LoggingUtility.Internal.TraceInformation(( int )EventId.SetMasterUrl, CoreResources.BrandingExtension_SetMasterUrl, masterPageServerRelativeUrl, web.ServerRelativeUrl);
            web.AllProperties[InheritMaster] = "False";
            web.MasterUrl = masterPageServerRelativeUrl;
            web.Update();
            web.Context.ExecuteQuery();
            websToUpdate.Add(web);

            if (!updateRootOnly)
            {
                var index = 0;
                while (index < websToUpdate.Count)
                {
                    var currentWeb = websToUpdate[index];
                    var websCollection = currentWeb.Webs;
                    web.Context.Load(websCollection, wc => wc.Include(w => w.AllProperties, w => w.ServerRelativeUrl));
                    web.Context.ExecuteQuery();
                    foreach (var childWeb in websCollection)
                    {
                        var inheritThemeProperty = childWeb.GetPropertyBagValueString(InheritTheme, string.Empty);
                        bool inheritTheme = false;
                        if (!string.IsNullOrEmpty(inheritThemeProperty))
                        {
                            inheritTheme = string.Equals(childWeb.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase);
                        }

                        if (resetSubsitesToInherit || inheritTheme)
                        {
                            //// LoggingUtility.Internal.TraceVerbose("Inherited: " + CoreResources.BrandingExtension_SetMasterUrl, masterPageServerRelativeUrl, childWeb.ServerRelativeUrl);
                            childWeb.AllProperties[InheritMaster] = "True";
                            childWeb.MasterUrl = masterPageServerRelativeUrl;
                            childWeb.Update();
                            web.Context.ExecuteQuery();
                            websToUpdate.Add(childWeb);
                        }
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// Set Custom master page by using given URL as parameter. Suitable for example in cases where you want sub sites to reference root site master page gallery. This is typical with publishing sites.
        /// </summary>
        /// <param name="web">Context web</param>
        /// <param name="masterPageServerRelativeUrl">The master page server relative URL.</param>
        /// <param name="resetSubsitesToInherit">false (default) to apply to currently inheriting sub sites only; true to force all sub sites to inherit</param>
        /// <param name="updateRootOnly">false (default) to apply to sub sites; true to only apply to specified site</param>
        /// <exception cref="System.ArgumentNullException">master Page Url</exception>
        public static void SetCustomMasterPageByUrl(this Web web, string masterPageServerRelativeUrl, bool resetSubsitesToInherit = false, bool updateRootOnly = false)
        {
            if (string.IsNullOrEmpty(masterPageServerRelativeUrl))
            {
                throw new ArgumentNullException("masterPageUrl");
            }

            var websToUpdate = new List<Web>();
            web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            //// LoggingUtility.Internal.TraceInformation(( int )EventId.SetCustomMasterUrl, CoreResources.BrandingExtension_SetCustomMasterUrl, masterPageServerRelativeUrl, web.ServerRelativeUrl);
            web.AllProperties[InheritMaster] = "False";
            web.CustomMasterUrl = masterPageServerRelativeUrl;
            web.Update();
            web.Context.ExecuteQuery();
            websToUpdate.Add(web);

            if (!updateRootOnly)
            {
                var index = 0;
                while (index < websToUpdate.Count)
                {
                    var currentWeb = websToUpdate[index];
                    var websCollection = currentWeb.Webs;
                    web.Context.Load(websCollection, wc => wc.Include(w => w.AllProperties, w => w.ServerRelativeUrl));
                    web.Context.ExecuteQuery();
                    foreach (var childWeb in websCollection)
                    {
                        var inheritThemeProperty = childWeb.GetPropertyBagValueString(InheritTheme, string.Empty);
                        bool inheritTheme = false;
                        if (!string.IsNullOrEmpty(inheritThemeProperty))
                        {
                            inheritTheme = string.Equals(childWeb.AllProperties[InheritTheme].ToString(), "True", StringComparison.InvariantCultureIgnoreCase);
                        }

                        if (resetSubsitesToInherit || inheritTheme)
                        {
                            //// LoggingUtility.Internal.TraceVerbose("Inherited: " + CoreResources.BrandingExtension_SetCustomMasterUrl, masterPageServerRelativeUrl, childWeb.ServerRelativeUrl);
                            childWeb.AllProperties[InheritMaster] = "True";
                            childWeb.CustomMasterUrl = masterPageServerRelativeUrl;
                            childWeb.Update();
                            web.Context.ExecuteQuery();
                            websToUpdate.Add(childWeb);
                        }
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// Sets the name of the theme based on.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="themeName">Name of the theme.</param>
        public static void SetThemeBasedOnName(this Web web, Web rootWeb, string themeName)
        {
            LogHelper.LogInformation("Set theme based on name.", LogEventID.InformationWrite);

            // Let's get instance to the composite look gallery
            List themeList = rootWeb.GetCatalog((int)ListTemplateType.DesignCatalog);
            web.Context.Load(themeList);
            web.Context.ExecuteQuery();

            CamlQuery query = new CamlQuery();
            query.ViewXml = string.Format(CAMLQUERYFINDBYFILENAME, themeName);
            var found = themeList.GetItems(query);
            web.Context.Load(found);
            web.Context.ExecuteQuery();
            if (found.Count > 0)
            {
                ListItem themeEntry = found[0];
                ////Set the properties for applying custom theme which was jus uplaoded
                string spcolorURL = null;
                if (themeEntry["ThemeUrl"] != null && themeEntry["ThemeUrl"].ToString().Length > 0)
                {
                    spcolorURL = MakeAsRelativeUrl((themeEntry["ThemeUrl"] as FieldUrlValue).Url);
                }

                string spfontURL = null;
                if (themeEntry["FontSchemeUrl"] != null && themeEntry["FontSchemeUrl"].ToString().Length > 0)
                {
                    spfontURL = MakeAsRelativeUrl((themeEntry["FontSchemeUrl"] as FieldUrlValue).Url);
                }

                string backGroundImage = null;
                if (themeEntry["ImageUrl"] != null && themeEntry["ImageUrl"].ToString().Length > 0)
                {
                    backGroundImage = MakeAsRelativeUrl((themeEntry["ImageUrl"] as FieldUrlValue).Url);
                }

                // Set theme
                web.ApplyTheme(spcolorURL, spfontURL, backGroundImage, true);

                // Let's also update master page, if needed
                if (themeEntry["MasterPageUrl"] != null && themeEntry["MasterPageUrl"].ToString().Length > 0)
                {
                    web.MasterUrl = MakeAsRelativeUrl((themeEntry["MasterPageUrl"] as FieldUrlValue).Url);
                }

                web.Context.ExecuteQuery();
                LogHelper.LogInformation("Added theme based on name.", LogEventID.InformationWrite);
            }
        }       

        #region File upload sections

        /// <summary>
        /// Uploads the theme file.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="localFilePath">The local file path.</param>
        /// <param name="themeFolderVersion">The theme folder version.</param>
        /// <returns>Share point file</returns>
        /// <exception cref="ArgumentNullException">local File Path</exception>
        /// <exception cref="ArgumentException">Source file path is required.;localFilePath</exception>
        public static Microsoft.SharePoint.Client.File UploadThemeFile(this Web web, string localFilePath, string themeFolderVersion = "15")
        {
            if (localFilePath == null)
            {
                throw new ArgumentNullException("localFilePath");
            }

            if (string.IsNullOrWhiteSpace(localFilePath))
            {
                throw new ArgumentException("Source file path is required.", "localFilePath");
            }

            var fileName = System.IO.Path.GetFileName(localFilePath);
           
            using (var localStream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open))
            {
                return UploadThemeFile(web, fileName, localStream, themeFolderVersion);
            }
        }

        /// <summary>
        /// Uploads the theme file.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="localStream">The local stream.</param>
        /// <param name="themeFolderVersion">The theme folder version.</param>
        /// <returns>Share point file</returns>
        /// <exception cref="System.ArgumentNullException">
        /// fileName
        /// or
        /// localStream
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Destination file name is required.;fileName
        /// or
        /// The argument must be a single file name and cannot contain path characters.;fileName
        /// </exception>
        public static Microsoft.SharePoint.Client.File UploadThemeFile(this Web web, string fileName, System.IO.Stream localStream, string themeFolderVersion = "15")
        {
            if (fileName == null) 
            { 
                throw new ArgumentNullException("fileName");
            }

            if (localStream == null)
            {
                throw new ArgumentNullException("localStream");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            { 
                throw new ArgumentException("Destination file name is required.", "fileName");
            }            
            
            if (fileName.Contains('/') || fileName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single file name and cannot contain path characters.", "fileName");
            }

            // Theme catalog only exists at site collection root
            var themesList = web.GetCatalog((int)ListTemplateType.ThemeCatalog);
            var themesFolder = themesList.RootFolder.EnsureFolder(themeFolderVersion);
            return themesFolder.UploadFile(fileName, localStream, true);            
        }

        /// <summary>
        /// Checks if the subfolder exists, and if it does not exist creates it.
        /// </summary>
        /// <param name="parentFolder">Parent folder to create under</param>
        /// <param name="folderName">Folder name to retrieve or create</param>
        /// <returns>
        /// The existing or newly created folder
        /// </returns>
        /// <exception cref="System.ArgumentException">The argument must be a single folder name and cannot contain path characters.;folderName</exception>
        /// <remarks>
        /// Note that this only checks one level of folder (the Folders collection) and cannot accept a name with path characters.
        /// </remarks>
        public static Folder EnsureFolder(this Folder parentFolder, string folderName)
        {           
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            var folderCollection = parentFolder.Folders;
            var folder = EnsureFolderImplementation(folderCollection, folderName);
            return folder;
        }        

        /// <summary>
        /// Uploads a file to the specified folder.
        /// </summary>
        /// <param name="folder">Folder to upload file to.</param>
        /// <param name="fileName">Location of the file to be uploaded.</param>
        /// <param name="stream">file stream</param>
        /// <param name="overwriteIfExists">true (default) to overwrite existing files</param>
        /// <returns>The uploaded File, so that additional operations (such as setting properties) can be done.</returns>
        public static Microsoft.SharePoint.Client.File UploadFile(this Folder folder, string fileName, Stream stream, bool overwriteIfExists)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Destination file name is required.", "fileName");
            }

            if (Regex.IsMatch(fileName, REGEXINVALIDFILENAMECHARS))
            {
                throw new ArgumentException("The argument must be a single file name and cannot contain path characters.", "fileName");
            }

            // Create the file
            var newFileInfo = new FileCreationInformation()
            {
                ContentStream = stream,
                Url = fileName,
                Overwrite = overwriteIfExists
            };

            // LoggingUtility.Internal.TraceVerbose("Creating file info with Url '{0}'", newFileInfo.Url);
            var file = folder.Files.Add(newFileInfo);
            folder.Context.Load(file);
            folder.Context.ExecuteQueryRetry();

            return file;
        }

        /// <summary>
        /// Ensures the folder implementation.
        /// </summary>
        /// <param name="folderCollection">The folder collection.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>Share point folder</returns>
        private static Folder EnsureFolderImplementation(FolderCollection folderCollection, string folderName)
        {
            Folder folder = null;
            folderCollection.Context.Load(folderCollection);
            folderCollection.Context.ExecuteQueryRetry();

            foreach (Folder existingFolder in folderCollection)
            {
                if (string.Equals(existingFolder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    folder = existingFolder;
                    break;
                }
            }

            if (folder == null)
            {
                folder = CreateFolderImplementation(folderCollection, folderName);
            }

            return folder;
        }

        /// <summary>
        /// Creates the folder implementation.
        /// </summary>
        /// <param name="folderCollection">The folder collection.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>Share point Folder</returns>
        private static Folder CreateFolderImplementation(FolderCollection folderCollection, string folderName)
        {
            var newFolder = folderCollection.Add(folderName);
            folderCollection.Context.Load(newFolder);
            folderCollection.Context.ExecuteQueryRetry();
            return newFolder;
        }

        /// <summary>
        /// Makes as relative URL.
        /// </summary>
        /// <param name="urlToProcess">The URL to process.</param>
        /// <returns>return url</returns>
        private static string MakeAsRelativeUrl(string urlToProcess)
        {
            Uri uri = new Uri(urlToProcess);
            return uri.AbsolutePath;
        }

        #endregion
    }
}
