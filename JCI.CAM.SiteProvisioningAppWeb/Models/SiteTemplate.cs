//-----------------------------------------------------------------------
// <copyright file="SiteTemplate.cs" company="Microsoft">
//     Microsoft &amp; JCI .
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.SiteProvisioningAppWeb.Models
{
    /// <summary>
    /// Model for SiteTemplate
    /// </summary>
    public class SiteTemplate
    {
        /// <summary>
        /// Gets or sets Name of the Site Type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Description of the Site Type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Image icon for the site type
        /// </summary>
        public string TypeIcon { get; set; }

        /// <summary>
        /// Gets or sets Managed path for the sites
        /// </summary>
        public string ManagedPath { get; set; }

        /// <summary>
        /// Gets or sets Root app web for the site type
        /// </summary>
        public string WebappUrl { get; set; }

        /// <summary>
        /// Gets or sets Base Id
        /// </summary>
        public string BaseId { get; set; }

        /// <summary>
        /// Gets or sets Unique Short Name for the site type
        /// </summary>
        public string Id { get; set; }
    }
}