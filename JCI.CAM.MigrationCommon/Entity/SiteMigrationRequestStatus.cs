// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteMigrationRequestStatus.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   site migration request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Migration.Common.Entity
{
    using System.ComponentModel;

    /// <summary>
    /// Enumeration for SiteMigrationRequestStatus
    /// </summary>
    public enum SiteMigrationRequestStatus
    {
        /// <summary>
        /// Indicate new state
        /// </summary>
        [Description("Not Started")]
        NotStarted,

        /// <summary>
        /// Indicate pending state
        /// </summary>
        Pending,

        /// <summary>
        /// Indicate exception state
        /// </summary>
        Failed,

        /// <summary>
        /// Indicate new state
        /// </summary>
        Ignore,

        /// <summary>
        /// Indicate success state
        /// </summary>
        Success
    }
}
