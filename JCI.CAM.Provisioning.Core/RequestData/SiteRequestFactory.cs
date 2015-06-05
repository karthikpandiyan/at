// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteRequestFactory.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Factory for working with the Site Request Repository
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Data
{    
    using System;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core.Configuration;

    /// <summary>
    /// Factory for working with the Site Request Repository
    /// <example>
    /// ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
    /// </example>
    /// </summary>
    public sealed class SiteRequestFactory : ISiteRequestFactory
    {
        #region Private Instance Members
        /// <summary>
        /// SiteRequestFactory instance
        /// </summary>
        private static readonly SiteRequestFactory Instance = new SiteRequestFactory();
        #endregion

        /// <summary>
        /// Returns an interface for working with the SiteRequest Factory
        /// </summary>
        /// <returns>ISiteRequestFactory instance</returns>
        public static ISiteRequestFactory GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Returns the the Site Request Manager Interface
        /// </summary>
        /// <returns>
        /// SiteRequestManager object
        /// </returns>
        /// <exception cref="DataStoreException">Exception occurred while Creating Instance</exception>
        public ISiteRequestManager GetSiteRequestManager()
        {
            LogHelper.LogInformation("JCI.CAM.Provisioning.Core.Data.SiteRequestFactory.GetSiteRequestManager - Loading Site Request Manager assemply to return SiteRequestManager instance", LogEventID.InformationWrite);
            var configFactory = ConfigurationFactoryManager.GetInstance();
            var manager = configFactory.GetAppSetingsManager();
            var settings = manager.GetAppSettings();
            var managerTypeString = settings.RepositoryManager;

            try
            {
                var type = managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (ISiteRequestManager)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                return instance;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                throw new DataStoreException("Exception Occured while Creating Instance", ex);
            }
        }
    }
}
