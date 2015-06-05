// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationFactoryManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Factory that is responsible for creating instances of IAppSettingsManager and ITemplateFactory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{
    using JCI.CAM.Provisioning.Core.TemplateEntites;   

    /// <summary>
    /// Factory that is responsible for creating instances of IAppSettingsManager
    /// and ITemplateFactory
    /// </summary>
    public sealed class ConfigurationFactoryManager : IConfigurationFactory
    {
        #region Private Instance Members
        /// <summary>
        /// Configuration Manager instance
        /// </summary>
        private static readonly ConfigurationFactoryManager Instance = new ConfigurationFactoryManager();
        #endregion

        #region IConfigurationFactory Members

        /// <summary>
        /// Returns an Instance of IConfigurationFactory
        /// </summary>
        /// <returns>IConfigurationFactory object</returns>
        public static IConfigurationFactory GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Returns an instance of IAppSettingsManager that is responsible for reading from the config files.
        /// </summary>
        /// <returns>
        /// IAppSettingsManager object
        /// </returns>
        public IAppSettingsManager GetAppSetingsManager()
        {
            return AppSettingsManager.GetInstance();
        }

        #endregion

        /// <summary>
        /// Returns an Instance of ITemplateFactory that is responsible for working
        /// Site Templates
        /// </summary>
        /// <param name="config">TemplateConfiguration instance</param>
        /// <returns>
        /// ITemplateFactory object
        /// </returns>
        public ITemplateFactory GetTemplateFactory(TemplateConfiguration config)
        {
            XMLTemplateManager.Init(config);
            return XMLTemplateManager.Instance;            
        }
    }
}
