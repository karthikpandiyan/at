//-----------------------------------------------------------------------
// <copyright file= "SiteViewModel.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.SubSiteProvisioningAppWeb.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Sub site creation view model
    /// </summary>
    public class SiteViewModel
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required(ErrorMessage = "Please enter site title")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Invalid title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the URL.
        /// </summary>
        /// <value>
        /// The name of the URL.
        /// </value>
        [Required(ErrorMessage = "Please enter site URL")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Site URL can contain alphabetic and numeric characters only. No spaces allowed.")]
        public string URLName { get; set; }

        /// <summary>
        /// Gets or sets the LCID.
        /// </summary>
        /// <value>
        /// The LCID.
        /// </value>
        [Required(ErrorMessage = "Please select language")]
        public string LCID { get; set; }

        /// <summary>
        /// Gets or sets the site template.
        /// </summary>
        /// <value>
        /// The site template.
        /// </value>
        [Required(ErrorMessage = "Please select site template")]
        public string SiteTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user permission].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user permission]; otherwise, <c>false</c>.
        /// </value>
        public bool IsParentSitePermission { get; set; }

        /// <summary>
        /// Gets or sets the site templates.
        /// </summary>
        /// <value>
        /// The site templates.
        /// </value>
        public List<System.Web.Mvc.SelectListItem> SiteTemplates { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public List<System.Web.Mvc.SelectListItem> Languages { get; set; }
    }
}