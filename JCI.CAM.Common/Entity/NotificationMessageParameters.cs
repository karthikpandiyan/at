//-----------------------------------------------------------------------
// <copyright file= "NotificationMessageParameters.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Container for propagating email message parameters
    /// </summary>
    public class NotificationMessageParameters
    {
        /// <summary>
        /// Gets or sets the name of the requestor.
        /// </summary>
        /// <value>
        /// The name of the requestor.
        /// </value>
        public string RequestorName { get; set; }

        /// <summary>
        /// Gets or sets the type of the site.
        /// </summary>
        /// <value>
        /// The type of the site.
        /// </value>
        public string SiteType { get; set; }

        /// <summary>
        /// Gets or sets the approver comments.
        /// </summary>
        /// <value>
        /// The approver comments.
        /// </value>
        public string ApproverComments { get; set; }

        /// <summary>
        /// Gets or sets the site title.
        /// </summary>
        /// <value>
        /// The site title.
        /// </value>
        public string SiteTitle { get; set; }

        /// <summary>
        /// Gets or sets the business unit.
        /// </summary>
        /// <value>
        /// The business unit.
        /// </value>
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets the edit request URL.
        /// </summary>
        /// <value>
        /// The edit request URL.
        /// </value>
        public string EditRequestUrl { get; set; }

        /// <summary>
        /// Gets or sets the cancel request URL.
        /// </summary>
        /// <value>
        /// The cancel request URL.
        /// </value>
        public string CancelRequestUrl { get; set; }

        /// <summary>
        /// Gets or sets the work flow status.
        /// </summary>
        /// <value>
        /// The work flow status.
        /// </value>
        public string WorkFlowStatus { get; set; }

        /// <summary>
        /// Gets or sets the requestor email.
        /// </summary>
        /// <value>
        /// The requestor email.
        /// </value>
        public string RequestorEmail { get; set; }

        /// <summary>
        /// Gets or sets the site URL.
        /// </summary>
        /// <value>
        /// The site URL.
        /// </value>
        public string SiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the site identifier.
        /// </summary>
        /// <value>
        /// The site identifier.
        /// </value>
        public int SiteId { get; set; }
    }
}
