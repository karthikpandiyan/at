// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateConfiguration.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{    
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using JCI.CAM.Common.Models;

    /// <summary>
    /// TemplateConfiguration xml schema
    /// </summary>
    [XmlRoot(ElementName = "TemplateConfiguration")]
    public partial class TemplateConfiguration
    {
        #region private members
        /// <summary>
        /// The templates
        /// </summary>
        private List<Template> templates;

        /// <summary>
        /// The theme packages
        /// </summary>
        private List<ThemePackage> themePackages;

        /// <summary>
        /// The custom actions
        /// </summary>
        private List<CustomAction> customActions;

        /// <summary>
        /// The list definition
        /// </summary>
        private List<ListDefinition> listDefinitions;
       
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>
        /// The templates.
        /// </value>
        [XmlArray(ElementName = "Templates")]
        [XmlArrayItem("Template", typeof(Template))]
        public List<Template> Templates 
        {
            get 
            {
                if (this.templates == null)
                {
                    this.templates = new List<Template>();
                }

                return this.templates;
            }

            set 
            { 
                this.templates = value;
            }
        }

        /// <summary>
        /// Gets or sets the theme packages.
        /// </summary>
        /// <value>
        /// The theme packages.
        /// </value>
        [XmlArray(ElementName = "ThemePackages")]
        [XmlArrayItem("ThemePackage", typeof(ThemePackage))]
        public List<ThemePackage> ThemePackages
        {
            get
            {
                return this.themePackages ?? (this.themePackages = new List<ThemePackage>());
            }

            set 
            { 
                this.themePackages = value; 
            }
        }

        /// <summary>
        /// Gets or sets the custom actions.
        /// </summary>
        /// <value>
        /// The custom actions.
        /// </value>
        [XmlArray(ElementName = "CustomActions")]
        [XmlArrayItem("CustomAction", typeof(CustomAction))]
        public List<CustomAction> CustomActions
        {
            get
            {
                return this.customActions ?? (this.customActions = new List<CustomAction>());
            }

            set 
            { 
                this.customActions = value; 
            }
        }

        /// <summary>
        /// Gets or sets the list definitions.
        /// </summary>
        /// <value>
        /// The list definitions.
        /// </value>
        [XmlArray(ElementName = "Lists")]
        [XmlArrayItem("List", typeof(ListDefinition))]
        public List<ListDefinition> ListDefinitions
        {
            get
            {
                return this.listDefinitions ?? (this.listDefinitions = new List<ListDefinition>());
            }

            set
            {
                this.listDefinitions = value;
            }
        }
        #endregion
    }
}
