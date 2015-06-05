// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLTemplateManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   XMLTemplateManager instance
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core.TemplateEntites;
    using System;    

    /// <summary>
    /// XMLTemplateManager instance
    /// </summary>
    internal sealed class XMLTemplateManager : ITemplateFactory
    {
        #region Instance Members
        /// <summary>
        /// The configuration factory
        /// </summary>
        private static readonly IConfigurationFactory ConfigFactory = ConfigurationFactoryManager.GetInstance();

        /// <summary>
        /// The lock object
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// The is initialized
        /// </summary>
        private static bool isInitialized = false;

        /// <summary>
        /// The template manager
        /// </summary>
        private TemplateManager templateManager = null;        
        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="XMLTemplateManager"/> class from being created.
        /// </summary>
        private XMLTemplateManager()
        {
        }
        #endregion

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        internal static XMLTemplateManager Instance
        {
            get { return XMLManager.Instance; }
        }

        #region public Members
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Init(TemplateConfiguration  config)
        {
            lock (LockObject)
            {
                if (!isInitialized)
                {
                    Instance.LoadXML(config);
                }
            }           
        }

        /// <summary>
        /// Returns an TemplateManager for working with Site Templates
        /// </summary>
        /// <returns>
        /// TemplateManager Object
        /// </returns>
        public TemplateManager GetTemplateManager(TemplateConfiguration config, ConfigurationContainers templateConfigurations)
        {
            this.templateManager = new TemplateManager
            {
                TemplateConfig = config,
                ConfigurationContainers = templateConfigurations
            };
            return this.templateManager;
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Loads the XML.
        /// </summary>
        private void LoadXML(TemplateConfiguration templateConfig)
        {
            try
            {
                this.templateManager = new TemplateManager { TemplateConfig = templateConfig };
                isInitialized = true;
                LogHelper.LogInformation(string.Format("JCI.CAM.Provisioning.Core.Configuration.XMLTemplateManager  - Loaded Configuration File for templates"), LogEventID.InformationWrite);                
            }
            catch (Exception ex)
            {
                // LogHelper.LogError("JCI.CAM.Provisioning.Core.Configuration.XMLTemplateManager", PCResources.XMLTemplateManager_Error, _ex.Message, _ex.StackTrace);
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, "JCI.CAM.Provisioning.Core.Configuration.XMLTemplateManager");
                isInitialized = false;
                throw;
            }
        }
        #endregion

        /// <summary>
        /// Private class for Nested Singleton to support initialization
        /// </summary>
        private class XMLManager
        {
            /// <summary>
            /// The instance
            /// </summary>
            internal static readonly XMLTemplateManager Instance = new XMLTemplateManager();           

            /// <summary>
            /// Prevents a default instance of the <see cref="XMLManager"/> class from being created.
            /// </summary>
            private XMLManager()
            {
            }
        }        
    }
}
