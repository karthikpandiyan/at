// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InformationManagementExtensions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  List Extensions
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{
    using System.Linq;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.InformationPolicy;

    /// <summary>
    /// Information Management Extensions
    /// </summary>
    public static partial class InformationManagementExtensions
    {
        /// <summary>
        /// Apply a policy to a site
        /// </summary>
        /// <param name="web">Web to operate on</param>
        /// <param name="sitePolicy">Policy to apply</param>
        /// <returns>True if applied, false otherwise</returns>
        public static bool ApplySitePolicy(this Web web, string sitePolicy)
        {
            LogHelper.LogInformation("Apply site policies.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);

            bool result = false;

            ClientObjectList<ProjectPolicy> sitePolicies = ProjectPolicy.GetProjectPolicies(web.Context, web);
            web.Context.Load(sitePolicies);
            web.Context.ExecuteQuery();

            if (sitePolicies != null && sitePolicies.Count > 0)
            {
                ProjectPolicy policyToApply = sitePolicies.Where(p => p.Name == sitePolicy).FirstOrDefault();

                if (policyToApply != null)
                {
                    ProjectPolicy.ApplyProjectPolicy(web.Context, web, policyToApply);
                    web.Context.ExecuteQuery();
                    result = true;
                    LogHelper.LogInformation("Site policy applied.", LogEventID.InformationWrite);
                }
                else
                {
                    LogHelper.LogInformation(string.Format("Site does not contain Site policy {0} to be applied.", sitePolicy), LogEventID.InformationWrite);
                }
            }

            return result;
        }
    }
}
