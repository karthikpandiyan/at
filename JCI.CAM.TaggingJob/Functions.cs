//-----------------------------------------------------------------------
// <copyright file= "Functions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.TaggingJob
{
    using System;
    using System.IO;
    using JCI.CAM.Common.Logging;
    using Microsoft.Azure.WebJobs;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Function Class
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// Processes the queue message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="log">The log.</param>
        public static void ProcessQueueMessage([QueueTrigger("autotagrequest")] CloudQueueMessage message, TextWriter log)
        {
            LogHelper.LogInformation("JCI.CAM.TaggingJob.Functions.ProcessQueueMessage -  Auto Tag Job execution started", LogEventID.InformationWrite);
            try
            {
                var tagJob = new TagRequestHandler();
                tagJob.ProcessRequestQueue(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }

            LogHelper.LogInformation("JCI.CAM.TaggingJob.Functions.ProcessQueueMessage -  Auto Tag Job execution completed", LogEventID.InformationWrite);
        }
    }
}
