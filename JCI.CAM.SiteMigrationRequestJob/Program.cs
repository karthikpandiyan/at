// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Program Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteMigrationRequestJob
{
    using System;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.SiteMigrationRequestJob.Helpers;

    /// <summary>
    /// Program Class
    /// </summary>
    public class Program
    {
      /// <summary>
      /// Executes site migration request queue job
      /// </summary>
      public static void Main()
        {
            try
            {
                LogHelper.LogInformation("Starting site migration request queue job execution...", LogEventID.InformationWrite);
                SiteMigrationRequestJobHelper siteMigrationRequestJob = new SiteMigrationRequestJobHelper();
                siteMigrationRequestJob.ProcessSiteMigrationRequestJob();
                LogHelper.LogInformation("Completed site migration request job execution.", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured while running the site migration request queue job execution.", LogEventID.InformationWrite);
                LogHelper.LogError(ex);
            }
        }
    }
}
