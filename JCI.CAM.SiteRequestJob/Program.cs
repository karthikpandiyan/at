// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Program Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteRequestJob
{
    using JCI.CAM.Common.Logging;
    using JCI.CAM.SiteRequest.Job;

    /// <summary>
    /// Program Class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            LogHelper.LogInformation("Starting site request job execution", LogEventID.InformationWrite);
            var siteRequestJob = new SiteRequestJob();
            siteRequestJob.ProcessSiteRequests();
            LogHelper.LogInformation("Completed site request job execution", LogEventID.InformationWrite);
        }
    }
}
