// <copyright file="OnlinePostTranformationJobActivities.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Post Tranformation Job Activities
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PostTransformationActivitiesJob.Helpers
{ 
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Provisioning.Core.Authentication;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Publishing;

    /// <summary>
    /// Post Transformation Job Activities
    /// </summary>
    public class OnlinePostTranformationJobActivities : PostTranformationJobActivities
    {
        /// <summary>
        /// Installs SandBox Solution
        /// </summary>
        /// <param name="succesfullyMigratedSites">Contains successfully migrated sites</param>
        protected override void InstallSandBoxSolutionAndUploadTheme(List<string> succesfullyMigratedSites)
        {
            string solutionGalleryListTitle = MigrationConstants.SolutionGallery;
            string fileName = Path.GetFileNameWithoutExtension(GlobalData.SandboxedListDefinitionWSP);

            PostTranformationJobActivities.GetThemeDetailsFromXML();

            for (int count = 0; count < succesfullyMigratedSites.Count; count++)
            {
                try
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Creating tenant context for site {0}", succesfullyMigratedSites[count]), LogEventID.InformationWrite);

                    AppOnlyAuthenticationTenant tenantAuthentication = new AppOnlyAuthenticationTenant();

                    using (ClientContext context = tenantAuthentication.GetAuthenticatedContextForGivenUrl(succesfullyMigratedSites[count]))
                    {
                        // Tenant admin context
                        Tenant tenant = new Tenant(context);

                        // Site Collection context 
                        var site = tenant.GetSiteByUrl(succesfullyMigratedSites[count]);
                        context.Load(site, s => s.Url, s => s.ServerRelativeUrl, s => s.RootWeb.Url);
                        context.ExecuteQuery();

                        LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "Tenant context created for site {0}", succesfullyMigratedSites[count]), LogEventID.InformationWrite);

                        // Root site
                        Web rootWeb = site.RootWeb;
                        context.Load(rootWeb, w => w.Url);
                        ListCollection lists = rootWeb.Lists;
                        List solutionGalleryList = null;
                        IEnumerable<List> existingLists = rootWeb.Context.LoadQuery(lists.Where(list => list.Title == solutionGalleryListTitle));
                        rootWeb.Context.ExecuteQuery();
                        solutionGalleryList = existingLists.FirstOrDefault();

                        PostTranformationJobActivities.InstallDesignPackage(context, site, rootWeb, solutionGalleryList, solutionGalleryListTitle, fileName);

                        PostTranformationJobActivities.AddThemeToSites(context, site, rootWeb, succesfullyMigratedSites[count]);
                    }
                }
                catch (Exception ex)
                {
                    string errorData = string.Format(CultureInfo.InvariantCulture, "Error occured while activating {0} sandbox solution in site {1}.", GlobalData.SandboxedListDefinitionWSP, succesfullyMigratedSites[count]);
                    PostTranformationJobActivities.ExceptionLogging(ex, errorData);
                }
            }
        }
    }
}
