// <copyright file="OnlinePersonalSitesTransformationJobHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
// Online Personal Sites Migration job 
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PersonalSitesTransformationJob.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization; 
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;    
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Authentication;
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Migration.Common.Helpers;    
    using JCI.CAM.Provisioning.Core;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;     

    /// <summary>
    /// Site Migration Job Helper Class
    /// </summary>
    public class OnlinePersonalSitesTransformationJobHelper : PersonalSitesTransformationJobHelper
    {   
        /// <summary>
        /// Transforms the personal sites.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="personalSiteUrl">The personal site URL.</param>
        /// <param name="personalSiteTitle">The personal site title.</param>
        /// <param name="themeEntity">The theme entity.</param>
        /// <param name="customActionEntity">The custom action entity.</param>
        /// <param name="featureInfo">The feature information.</param>
        public override void TransformingPersonalSites(ClientContext context, string personalSiteUrl, string personalSiteTitle, ThemeEntity themeEntity, CustomActionEntity customActionEntity, List<Features.Feature> featureInfo)
        {
            try
            {
                // Business site custom branding.
                Tenant tenantSite = new Tenant(context);
                var personalSite = tenantSite.GetSiteByUrl(personalSiteUrl);
                context.Load(personalSite, s => s.Url, s => s.ServerRelativeUrl);
                Web rootWeb = personalSite.RootWeb;
                context.Load(rootWeb, w => w.Title, w => w.Url, w => w.ServerRelativeUrl, w => w.CustomMasterUrl, w => w.MasterUrl);

                // Getting relative url
                Uri siteURL = new Uri(personalSiteUrl);
                string relativeSiteURL = Uri.UnescapeDataString(siteURL.AbsolutePath);

                Web web = personalSite.OpenWeb(relativeSiteURL);
                context.Load(web, w => w.Title, w => w.Url, w => w.ServerRelativeUrl, w => w.CustomMasterUrl, w => w.MasterUrl);
                context.ExecuteQuery();

                PersonalSitesTransformationJobHelper.DeactivateFeatures(personalSite, web, featureInfo);

                PersonalSitesTransformationJobHelper.ResetMasterPage(web, rootWeb);

                // Let's first upload the contoso theme to host web, if it does not exist there
                if (web.Url.Equals(rootWeb.Url))
                {
                    LogHelper.LogInformation(string.Format("Applying custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                    PersonalSitesTransformationJobHelper.DeployThemeToWeb(web, themeEntity);
                    LogHelper.LogInformation(string.Format("Applied custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format("Not uploading the theme for site {0} because it is not a root site.", web.Url), LogEventID.InformationWrite);
                    LogHelper.LogInformation(string.Format("Applying custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                    web.SetThemeBasedOnName(rootWeb, themeEntity.Name);
                    LogHelper.LogInformation(string.Format("Applied custom branding for site {0}...", web.Url), LogEventID.InformationWrite);
                }

                try
                {
                    PersonalSitesTransformationJobHelper.ApplyCss(web, themeEntity, "Style Library");
                    LogHelper.LogInformation(string.Format("Applied css for site {0}", web.Url), LogEventID.InformationWrite);
                    PersonalSitesTransformationJobHelper.ApplySiteLogo(web, themeEntity, "Style Library");
                    LogHelper.LogInformation(string.Format("Applied site logo for site {0}", web.Url), LogEventID.InformationWrite);
                }
                catch (Exception ex)
                {
                    // Ignoring exception
                    LogHelper.LogInformation("Due to API limitations, AlternateCSS and SiteLogo options may not work in dedicated environment. This options can be used in vNext.", LogEventID.InformationWrite);
                    LogHelper.LogError(ex);
                }

                // apply header and footer css.
                web.AddCustomAction(customActionEntity);
                LogHelper.LogInformation(string.Format("Applied header and footer css for site {0}", web.Url), LogEventID.InformationWrite);

                web.SetPropertyBagValue(MigrationConstants.JCICAMTransformationPropertyBagKey, MigrationConstants.JCICAMTransformationPropertyBagValue);
                
                LogHelper.LogInformation("Checking for subsites...", LogEventID.InformationWrite);

                WebCollection webs = web.Webs;
                context.Load(webs);
                context.ExecuteQuery();

                if (webs.Count > 0)
                {
                    foreach (var subWeb in webs)
                    {
                        // Checking whether the site template is app or not.
                        if (!subWeb.WebTemplate.ToLower().Equals(GlobalData.AppSiteTemplate.ToLower()))
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Transforming the subsite {0}...", subWeb.Url), LogEventID.InformationWrite);
                            this.TransformingPersonalSites(context, subWeb.Url, subWeb.Title, themeEntity, customActionEntity, featureInfo);
                        }
                        else
                        {
                            LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Not transforming the site because this is an app site. Subsite url: {0}, Template: {1}.", subWeb.Url, subWeb.WebTemplate), LogEventID.InformationWrite);
                        }
                    }
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "There are no subsites."), LogEventID.InformationWrite);
                }

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Tranformation is completed for site {0}({1}).", personalSiteTitle, personalSiteUrl), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                string errorData = string.Format(CultureInfo.InvariantCulture, "JCI.CAM.PersonalSitesTransformationJob.Helpers.TransformingOneDriveSites() - Error occurred while performomg the transformation for one drive site {0}({1}).", personalSiteTitle, personalSiteUrl);
                PersonalSitesTransformationJobHelper.PersonalSiteTransformationJobErrorData = PersonalSitesTransformationJobHelper.PersonalSiteTransformationJobErrorData + errorData + " Exception Details: " + ex.Message + " ";
                MigrationCommonHelper.ExceptionLogging(ex, errorData);
            }
        }
    }
}
