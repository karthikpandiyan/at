// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppSettingsManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Used to return an Instance of AppSettings
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.Provisioning.Core.Configuration
{
    /// <summary>
    /// Used to return an Instance of AppSettings
    /// </summary>
    public interface IAppSettingsManager
    {
        /// <summary>
        /// Returns an Instance of AppSettings
        /// </summary>
        /// <returns>App Settings</returns>
        AppSettings GetAppSettings();
    }
}
