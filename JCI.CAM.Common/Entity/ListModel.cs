//-----------------------------------------------------------------------
// <copyright file= "ListModel.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// List view model
    /// </summary>
    public class ListModel
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required(ErrorMessage = "Please enter list title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the template title.
        /// </summary>
        /// <value>
        /// The template title.
        /// </value>
        [Required(ErrorMessage = "Please select a template")]
        public string TemplateTitle { get; set; }

        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>
        /// The templates.
        /// </value>
        public List<string> Templates { get; set; }
    }
}