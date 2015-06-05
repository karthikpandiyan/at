// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Functions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SharePointDataController retrieves sharepoint document metadata
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteProvisioningJob
{
    using System.IO;
    using Microsoft.Azure.WebJobs;

    /// <summary>
    /// Commonly used functions
    /// </summary>
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.

        /// <summary>
        /// Processes the queue message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="log">The log.</param>
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }
    }
}
