// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplateFactory.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{
    using JCI.CAM.Provisioning.Core.TemplateEntites;    

    /// <summary>
    /// Interface for Creating the TemplateManager 
    /// </summary>
    public interface ITemplateFactory
    {
        /// <summary>
        /// Returns an TemplateManager for working with Site Templates
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="templateConfigurations">The template configurations.</param>
        /// <returns>
        /// TemplateManager Object
        /// </returns>
        TemplateManager GetTemplateManager(TemplateConfiguration config, ConfigurationContainers templateConfigurations);
    }
}
