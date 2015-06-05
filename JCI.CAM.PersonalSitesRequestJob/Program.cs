// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Program Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PersonalSitesRequestJob
{
    using System;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.PersonalSitesRequestJob.Helpers;

    /// <summary>
    /// Program Class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Executes blog transformation request queue job
        /// </summary>
        public static void Main()
        {
            try
            {
                LogHelper.LogInformation("Starting personal sites transformation request queue job execution...", LogEventID.InformationWrite);
                PersonalSitesTransformationRequestJobHelper personalSitesTransformationRequestJobHelper = new PersonalSitesTransformationRequestJobHelper();
                personalSitesTransformationRequestJobHelper.ProcessTransformationRequests();
                LogHelper.LogInformation("Completed personal sites transformation request job execution.", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Main() - Error occured while running the blog transformation request queue job execution.", LogEventID.InformationWrite);
                LogHelper.LogError(ex);
            }
        }
    }
}
