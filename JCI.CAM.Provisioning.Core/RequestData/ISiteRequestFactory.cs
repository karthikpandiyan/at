// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISiteRequestFactory.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Data
{
    /// <summary>
    /// Site Request Factory Interface
    /// </summary>
    public interface ISiteRequestFactory
    {
        /// <summary>
        /// Returns an interface for working with the Site Request Repository
        /// </summary>
        /// <returns>SiteRequestManager object</returns>
        /// <exception cref="Framework.Provisioning.Core.Data.DataStoreException">Exception that occurs when interacting with the Site Request Repository</exception>
        ISiteRequestManager GetSiteRequestManager();
    }      
}
