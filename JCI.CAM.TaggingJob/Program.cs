// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SharePointDataController retrieves sharepoint document metadata
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.TaggingJob
{
    using System;
    using JCI.CAM.Common.Logging;
    using Microsoft.Azure.WebJobs;

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
            var host = new JobHost();

            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();            
        }
    }
}
