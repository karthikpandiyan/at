// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Helper class to read from the Config files
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core.Configuration
{
    using System;
    using System.Configuration;
    using JCI.CAM.Common.Logging;

    /// <summary>
    /// Helper class to read from the Config files
    /// </summary>
    internal static class ConfigurationHelper
    {
        #region Public Static Members

        /// <summary>
        /// Helper method to return the a value define in the config file.
        /// </summary>
        /// <param name="key">The key of the value to return</param>
        /// <returns>Value of the key requested</returns>
        public static string Get(string key)
        {
            string returnValue = string.Empty;

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(PCResources.Exception_Message_EmptyString_Arg, "key");
            }

            try
            {
                if (ConfigurationManager.AppSettings[key] != null)
                {
                    returnValue = ConfigurationManager.AppSettings.Get(key);
                }

                return returnValue;
            }
            catch (ConfigurationErrorsException ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                throw;
            }
        }
        #endregion
    }
}