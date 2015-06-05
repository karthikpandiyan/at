// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteMetadata.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Entity
{
    using System.Collections.Generic;

    /// <summary>
    /// This Class is used to have to Site Metadata that is used in Site Navigation
    /// </summary>
    public class SiteMetadata
    {
        #region Member Variables

        /// <summary>
        /// Site Metadata Dictionary
        /// </summary>
        private Dictionary<string, string> metaData;

        /// <summary>
        /// Gets or sets the meta data dictionary.
        /// </summary>
        public Dictionary<string, string> MetaData
        {
            get
            {
                return this.metaData;
            }

            set
            {
                this.metaData = value;
            }
        }
        #endregion
    }
}
