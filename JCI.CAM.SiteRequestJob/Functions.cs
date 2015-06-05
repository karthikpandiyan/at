// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Functions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Program Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteRequestJob
{
    using System.IO;
    using Microsoft.Azure.WebJobs;

    /// <summary>
    /// Azure Queue Processing class
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// This function will get triggered/executed when a new message is written on an Azure Queue called queue.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="log">The log.</param>
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }
    }
}
