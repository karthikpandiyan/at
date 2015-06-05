// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SharePointDataController retrieves sharepoint document metadata
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteProvisioningJob
{
    using System;
    using JCI.CAM.Common.Logging;

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976

    /// <summary>
    /// Site provisioning start method or starting point
    /// </summary>
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.Program.Main -  Job execution started", LogEventID.InformationWrite);
            try
            {                
                var provisioningJob = new ProvisioningRequestHandler();
                provisioningJob.ProcessRequestQueue();
            }catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceInformation(ex.Message + ex.StackTrace);
            }

            LogHelper.LogInformation("JCI.CAM.SiteProvisioningJob.Program.Main -  Job execution completed", LogEventID.InformationWrite);
        }
    }
}
