// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{   
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JCI.CAM.Common.Models;
    using JCI.CAM.Provisioning.Core.TemplateEntites;

    /// <summary>
    /// Class for working with Provisioning Templates
    /// </summary>
    public class TemplateManager
    {
        /// <summary>
        /// Gets or sets the template configuration.
        /// </summary>
        /// <value>
        /// The template configuration.
        /// </value>
        public TemplateConfiguration TemplateConfig { get; set; }

        /// <summary>
        /// Gets or sets the configuration containers.
        /// </summary>
        /// <value>
        /// The configuration containers.
        /// </value>
        public ConfigurationContainers ConfigurationContainers { get; set; }

        /// <summary>
        /// Gets the name of the template by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Template Xml</returns>        
        public Template GetTemplateByTitle(string name)
        {
            if (string.IsNullOrEmpty(name))
            { 
                throw new ArgumentException(name); 
            }

            var templateResult = this.TemplateConfig.Templates.FirstOrDefault(t => t.Title == name);
            return templateResult;
        }

        /// <summary>
        /// Returns a Template object by ID.
        /// Will return Null if the Template is not found
        /// </summary>
        /// <param name="id">The ID of the template</param>
        /// <returns>
        /// Template instance
        /// </returns>        
        public Template GetTemplateByID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(id);
            }

            return this.TemplateConfig.Templates.FirstOrDefault(t => t.ID == id);
        }

        /// <summary>
        /// Returns the collection of Templates that are available for creating Web sites within the site collection.
        /// </summary>
        /// <returns>List of Template</returns>
        public List<Template> GetAvailableTemplates()
        {
            var template = this.TemplateConfig.Templates.FindAll(t => t.Enabled);
            return template;
        }       

        /// <summary>
        /// Returns a <seealso cref="ThemePackage"/> that is used for applying branding to a Web site.
        /// </summary>
        /// <param name="name">Name of theme</param>
        /// <returns>ThemePackage instance</returns>
        public ThemePackage GetThemePackageByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(name);
            }

            var templateThemePackage = this.TemplateConfig.ThemePackages.Find(p => p.Name == name);
            return templateThemePackage;
        }

        /// <summary>
        /// Returns a collection of <seealso cref="CustomAction"/>. These Custom Actions should be applied to the Web site.
        /// </summary>
        /// <returns>CustomActions List</returns>
        public List<CustomAction> GetCustomActions()
        {
            return this.TemplateConfig.CustomActions.FindAll(c => c.Enabled);
        }

        /// <summary>
        /// Gets the List Definitions.
        /// </summary>
        /// <returns>List of ListDefinition</returns>
        public List<ListDefinition> GetListDefinitions()
        {
            return this.TemplateConfig.ListDefinitions;
        }

        /// <summary>
        /// Gets the name of the configuration container by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// ConfigurationContainer instance
        /// </returns>
        /// <exception cref="System.ArgumentException">Argument exception</exception>
        public ConfigurationContainer GetConfigurationContainerByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(name);
            }

            ConfigurationContainer configurationContainer = null;

            if (this.ConfigurationContainers != null)
            {
                configurationContainer = this.ConfigurationContainers.ConfigurationContainerItems.FirstOrDefault(t => t.Name == name);
            }

            return configurationContainer;
        }
    }
}
