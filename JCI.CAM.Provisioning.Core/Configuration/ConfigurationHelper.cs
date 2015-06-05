// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Interface that is used by the factory that is responsible for creating objects for IAppSettingsManager and ITemplateFactory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.Configuration
{
    /// <summary>
    /// Helper class to read from the Config files
    /// </summary>
    internal static class ConfigurationHelper
    {
        /// <summary>
        /// Logging Source
        /// </summary>
        public const string LoggingSource = "ConfigurationHelper";

        #region Public Static Members

        /// <summary>
        /// Helper method to return the a value define in the config file.
        /// </summary>
        /// <param name="key">The key of the value to return</param>
        /// <returns>Value of the key requested</returns>
        public static string Get(string key)
        {
            ////string _returnValue = string.Empty;

            ////if (string.IsNullOrEmpty(key))
            ////    throw new ArgumentException(PCResources.Exception_Message_EmptyString_Arg, "key");

            ////try
            ////{
            ////    Log.Debug(LOGGING_SOURCE, PCResources.AppSettings_GetKey, key);
            ////    if(SC.ConfigurationManager.AppSettings.AllKeys.Contains(key))
            ////    { 
            ////        _returnValue = SC.ConfigurationManager.AppSettings.Get(key);
            ////    }
            ////    else
            ////    {
            ////        Log.Warning(LOGGING_SOURCE, PCResources.AppSettings_KeyNotFound, key);
            ////    }
            ////    return _returnValue;
            ////}
            ////catch(SC.ConfigurationErrorsException)
            ////{
            ////    throw;
            ////}

            return string.Empty;
        }
        #endregion
    }
}
