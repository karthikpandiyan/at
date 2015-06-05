// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Program Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteMigrationJob
{    
    using System;
    using JCI.CAM.Common.Logging;

    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Executes site migration job
        /// </summary>
        /// <param name="args">Args instance</param>
        public static void Main(string[] args)
        {
            try
            {
                LogHelper.LogInformation("Starting site migration job execution...", LogEventID.InformationWrite);
                SiteMigrationJobHandler siteMigrationRequestHandler = new SiteMigrationJobHandler();
                siteMigrationRequestHandler.ProcessRequestQueue();
                LogHelper.LogInformation("Completed site migration job execution.", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured while running the site migration request queue job execution.", LogEventID.InformationWrite);
                LogHelper.LogError(ex);
            }            
        }
    }
}
