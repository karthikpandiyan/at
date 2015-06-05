// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureExtensions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  List Extensions
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Common.AppModelExtensions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;    

    /// <summary>
    /// Feature Extensions
    /// </summary>
    public static partial class FeatureExtensions
    {
        /// <summary>
        /// Deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="features">Feature collection instance</param>
        /// <param name="featureTitle">Feature Title</param>
        /// <param name="featureID">Feature ID</param>
        public static void DeactivateFeature(FeatureCollection features, string featureTitle, Guid featureID)
        {
            // Check whether feature is activated or not. If activated then deactivate it.
            if (IsFeatureActiveInternal(features, featureID))
            {
                ProcessFeatureInternal(features, featureID, false);
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} feature deactivated.", featureTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
            else
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "{0} feature is not activated.", featureTitle), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Activates a site collection or site scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to activate</param>
        /// <param name="activate">if set to <c>true</c> [activate].</param>
        /// <param name="scope">The scope.</param>
        public static void ActivateFeature(this Web web, Guid featureID, bool activate = true, FeatureDefinitionScope scope = FeatureDefinitionScope.Farm)
        {
            web.ProcessFeature(featureID, activate, scope);
        }

        /// <summary>
        /// Activates a site collection or site scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to activate</param>
        public static void ActivateFeature(this Site site, Guid featureID)
        {            
            site.ProcessFeature(featureID, true);
        }       

        /// <summary>
        /// Checks if a feature is active
        /// </summary>
        /// <param name="site">Site to operate against</param>
        /// <param name="featureID">ID of the feature to check</param>
        /// <returns>True if active, false otherwise</returns>
        public static bool IsFeatureActive(this Site site, Guid featureID)
        {
            return IsFeatureActiveInternal(site.Features, featureID);
        }

        /// <summary>
        /// Checks if a feature is active
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="featureID">ID of the feature to check</param>
        /// <returns>True if active, false otherwise</returns>
        public static bool IsFeatureActive(this Web web, Guid featureID)
        {
            return IsFeatureActiveInternal(web.Features, featureID);
        }

        /// <summary>
        /// Activates or deactivates a site collection or web scoped feature
        /// </summary>
        /// <param name="features">Feature Collection which contains the feature</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        /// <param name="scope">The scope.</param>
        public static void ProcessFeatureInternal(FeatureCollection features, Guid featureID, bool activate, FeatureDefinitionScope scope = FeatureDefinitionScope.Farm)
        {
            try
            {
                LogHelper.LogInformation("Process feature activation or deactivation.", LogEventID.InformationWrite);

                features.Context.Load(features);
                features.Context.ExecuteQuery();

                // The original number of active features...use this to track if the feature activation went OK
                int oldCount = features.Count();

                if (activate)
                {
                    // GetById does not seem to work for site scoped features...if (clientSiteFeatures.GetById(featureID) == null)

                    // FeatureDefinitionScope defines how the features have been deployed. All OOB features are farm deployed
                    features.Add(featureID, true, scope);
                    features.Context.ExecuteQuery();

                    // retry logic needed to make this more bulletproof :-(
                    features.Context.Load(features);
                    features.Context.ExecuteQuery();

                    int tries = 0;
                    int currentCount = features.Count();
                    while (currentCount <= oldCount && tries < 5)
                    {
                        tries++;
                        features.Add(featureID, true, scope);
                        features.Context.ExecuteQuery();
                        features.Context.Load(features);
                        features.Context.ExecuteQuery();
                        currentCount = features.Count();
                    }

                    LogHelper.LogInformation("Feature activation processed.", LogEventID.InformationWrite);
                }
                else
                {
                    try
                    {
                        features.Remove(featureID, true);
                        features.Context.ExecuteQuery();
                        LogHelper.LogInformation("Feature removed.", LogEventID.InformationWrite);
                    }
                    catch (System.Exception ex)
                    {
                        LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }
        }

        /// <summary>
        /// Checks if a feature is active in the given FeatureCollection.
        /// </summary>
        /// <param name="features">FeatureCollection to check in</param>
        /// <param name="featureID">ID of the feature to check</param>
        /// <returns>True if active, false otherwise</returns>
        private static bool IsFeatureActiveInternal(FeatureCollection features, Guid featureID)
        {
            bool featureIsActive = false;

            features.Context.Load(features);
            features.Context.ExecuteQueryRetry();
            Feature iprFeature = features.GetById(featureID);
            features.Context.Load(iprFeature, f => f.DefinitionId);
            features.Context.ExecuteQueryRetry();

            if (iprFeature != null && iprFeature.IsPropertyAvailable("DefinitionId") && !iprFeature.ServerObjectIsNull.Value && iprFeature.DefinitionId.Equals(featureID))
            {
                featureIsActive = true;
            }
            else
            {
                featureIsActive = false;
            }

            return featureIsActive;
        }

        /// <summary>
        /// Activates or deactivates a site collection scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        private static void ProcessFeature(this Site site, Guid featureID, bool activate)
        {
            ProcessFeatureInternal(site.Features, featureID, activate);
        }

        /// <summary>
        /// Activates or deactivates a web scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        /// <param name="scope">The scope.</param>
        private static void ProcessFeature(this Web web, Guid featureID, bool activate, FeatureDefinitionScope scope)
        {
            ProcessFeatureInternal(web.Features, featureID, activate, scope);
        }        
    }
}
