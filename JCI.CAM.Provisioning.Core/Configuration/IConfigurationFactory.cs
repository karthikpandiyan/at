// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationFactory.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Interface that is used by the factory that is responsible for creating objects for IAppSettingsManager and ITemplateFactory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{
    using JCI.CAM.Provisioning.Core.TemplateEntites;

    /// <summary>
    /// Interface that is used by the factory that is responsible for creating objects for IAppSettingsManager and ITemplateFactory
    /// </summary>
    public interface IConfigurationFactory
    {
        /// <summary>
        /// Gets the Object that is responsible for returning the Settings of the Applications
        /// </summary>
        /// <returns>IAppSettingsManager object</returns>
        IAppSettingsManager GetAppSetingsManager();

        /// <summary>
        /// Returns an ITemplateFactory for working with Site Templates
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// ITemplateFactory object
        /// </returns>
        ITemplateFactory GetTemplateFactory(TemplateConfiguration config);
    }
}
