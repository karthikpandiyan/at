// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteRequestStatus.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core
{
    /// <summary>
    /// Enumeration for SiteRequestStatus
    /// </summary>
    public enum SiteRequestStatus
    {
        /// <summary>
        /// Indicate complete state
        /// </summary>
        Complete,

        /// <summary>
        /// Indicate exception state
        /// </summary>
        Exception,

        /// <summary>
        /// Indicate new state
        /// </summary>
        New,

        /// <summary>
        /// Indicate processing state
        /// </summary>
        Processing,

        /// <summary>
        /// Indicate pending state
        /// </summary>
        Pending,

        /// <summary>
        /// Indicate approved state
        /// </summary>
        Approved,

        /// <summary>
        /// The successful
        /// </summary>
        Successful,

        /// <summary>
        /// The rejected
        /// </summary>
        Rejected
    }
}
