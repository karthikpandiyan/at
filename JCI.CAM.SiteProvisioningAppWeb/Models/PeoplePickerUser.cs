//-----------------------------------------------------------------------
// <copyright file="PeoplePickerUser.cs" company="Microsoft">
//     Microsoft &amp; JCI .
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.SiteProvisioningAppWeb.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Representative model of People to be used in the People Picker control
    /// </summary>
    [DataContract]
    public class PeoplePickerUser
    {
        /// <summary>
        /// Gets or sets Lookup Id
        /// </summary>
        [DataMember]
        public int? LookupId { get; set; }    // Nullable

        /// <summary>
        /// Gets or sets Login
        /// </summary>
        [DataMember]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        [DataMember]
        public string Email { get; set; }
    }
}