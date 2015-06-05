// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Helper Class for working with Paths for the Templates
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core
{
    using System;
    using System.IO;
    using System.Reflection;
    using JCI.CAM.Common.Logging;

    /// <summary>
    /// Helper Class for working with Paths for the Templates
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Gets the Path to where the DLLs are stored
        /// </summary>
        /// <returns>Assembly Directory Name</returns>
        public static string GetAssemblyDirectory()
        {
            try
            {
                LogHelper.LogInformation("JCI.CAM.Provisioning.Core.PathHelper.GetAssemblyDirectory - Getting current execution assembly directory path", LogEventID.InformationWrite);
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uriBuilder = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uriBuilder.Path);
                return Path.GetDirectoryName(path);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                throw;
            }
        }
    }
}
