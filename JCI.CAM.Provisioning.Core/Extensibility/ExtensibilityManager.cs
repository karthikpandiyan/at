// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensibilityManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Class to handle Extensibility call out
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core.Extensibility
{
    using System;

    /// <summary>
    /// Class to handle Extensibility call out.
    /// </summary>
    public class ExtensibilityManager
    {
        /// <summary>
        /// Member to call out to the Extensibility Provider. The Assembly must implement <see cref="ISharePointExtensibility" /> and the
        /// assembly must be located in the same directory.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Assembly type.</param>
        /// <param name="siteInfo">Site request information.</param>
        public void Execute(string assemblyName, string typeName, SiteRequestInformation siteInfo)
        {
            try
            {
                var instance = (ISharePointExtensibility)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                instance.ProcessRequest(siteInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
