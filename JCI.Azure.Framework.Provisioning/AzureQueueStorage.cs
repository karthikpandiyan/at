// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureQueueStorage.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Request Message Entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.Azure.Framework.Provisioning
{   
    using System;
    using System.Configuration;
    using System.Globalization;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Provisioning.Core;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling.Configuration;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;    
    using Microsoft.WindowsAzure.Storage.RetryPolicies;    

    /// <summary>
    /// Implements operations for azure queue storage
    /// </summary>
    public class AzureQueueStorage
    {
        #region instance Members
        /// <summary>
        /// The azure connection
        /// </summary>
        private string azureConnection;

        /// <summary>
        /// The request queue name
        /// </summary>
        private string requestQueueName;

        /// <summary>
        /// The time span
        /// </summary>
        private double timeSPan;

        /// <summary>
        /// The queue request options
        /// </summary>
        private QueueRequestOptions queueRequestOptions = new QueueRequestOptions();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueueStorage"/> class.
        /// </summary>
        public AzureQueueStorage()
        {
            IRetryPolicy retryPolicy = new LinearRetry(TimeSpan.FromSeconds(2), 15);
            this.queueRequestOptions.RetryPolicy = retryPolicy;
        }

        #region Properties
        /// <summary>
        /// Gets or sets Azure Connection String Property for working with the service bus
        /// </summary>
        public string AzureConnectionString
        {
            get
            {
                return this.azureConnection;
            }

            set
            {
                this.azureConnection = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the request queue.
        /// </summary>
        /// <value>
        /// The name of the request queue.
        /// </value>
        public string RequestQueueName
        {
            get
            {
                return this.requestQueueName;
            }

            set
            {
                this.requestQueueName = value;
            }
        }

        /// <summary>
        /// Gets or sets time span
        /// </summary>
        public double TimeSPan
        {
            get
            {
                return this.timeSPan;
            }

            set
            {
                this.timeSPan = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets the queue.
        /// </summary>
        /// <returns>Returns the queue</returns>
        public CloudQueue GetQueue()
        {
            CloudQueue queue = null;

            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.GetQueue() - Retrieving the storage account from the connection string {0}", this.AzureConnectionString), LogEventID.InformationWrite);
                
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.AzureConnectionString);

                // Create the queue client
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue
                queue = queueClient.GetQueueReference(this.RequestQueueName);

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.GetQueue() - Accessed queue {0}.", queue.Name), LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured at JCI.Azure.Framework.Provisioning.AzureQueueStorage.GetQueue().");
                LogHelper.LogError(ex);
            }
            
            return queue;
        }

        /// <summary>
        /// Sends the message to queue.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        public void SendMessageToQueue(string requestMessage)
        {
            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.SendMessageToQueue() - Sending the message {0} to the queue...", requestMessage), LogEventID.InformationWrite);
                CloudQueue queue = this.GetQueue();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage(requestMessage);
                queue.AddMessage(message, null, TimeSpan.FromMinutes(1.0), this.queueRequestOptions);

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.SendMessageToQueue() - Sent the message {0} to the queue.", requestMessage), LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured at JCI.Azure.Framework.Provisioning.AzureQueueStorage.SendMessageToQueue().");
                LogHelper.LogError(ex);
            }
        }

        /// <summary>
        /// Reads the message from queue.
        /// </summary>
        /// <returns>Returns the message</returns>
        public CloudQueueMessage ReadMessageFromQueue()
        {
            CloudQueueMessage queueMessage = null;
            string message = string.Empty;

            try
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.ReadMessageFromQueue() - Reading the message from queue..."), LogEventID.InformationWrite);

                CloudQueue queue = this.GetQueue();
                queueMessage = queue.GetMessage(TimeSpan.FromMinutes(1.0), this.queueRequestOptions);
                message = queueMessage.AsString;
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.ReadMessageFromQueue() - Message details: {0}.", message), LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured at JCI.Azure.Framework.Provisioning.AzureQueueStorage.ReadMessageFromQueue().");
                LogHelper.LogError(ex);
            }

            return queueMessage;
        }

        /// <summary>
        /// Updates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="autoTagRequestInformation">The automatic tag request information.</param>
        /// <param name="timeToLive">The time to live.</param>
        public void UpdateMessage(CloudQueueMessage message, AutoTagRequestInformation autoTagRequestInformation, TimeSpan timeToLive)
        {
            try
            {
                CloudQueue queue = this.GetQueue();
                if (autoTagRequestInformation.RetryCount <= 2)
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.UpdateMessage() - Updating the message..."), LogEventID.InformationWrite);
                    autoTagRequestInformation.RetryCount = autoTagRequestInformation.RetryCount++;
                    string autoTagRequestMessage = XmlSerializerHelper.Serialize<AutoTagRequestInformation>(autoTagRequestInformation);
                    message.SetMessageContent(autoTagRequestMessage);
                    queue.UpdateMessage(message, timeToLive, MessageUpdateFields.Content | MessageUpdateFields.Visibility, this.queueRequestOptions);
                }
                else
                {
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.UpdateMessage() - Deleting the message because retry count is {0}...", autoTagRequestInformation.RetryCount), LogEventID.InformationWrite);
                    queue.DeleteMessage(message, this.queueRequestOptions);
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.UpdateMessage() - Deleted message because retry count is {0}.", autoTagRequestInformation.RetryCount), LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured at JCI.Azure.Framework.Provisioning.AzureQueueStorage.UpdateMessage().");
                LogHelper.LogError(ex);
            }
        }

        /// <summary>
        /// Deletes the message from queue.
        /// </summary>
        /// <param name="queueMessage">The queue message.</param>
        public void DeleteMessageFromQueue(CloudQueueMessage queueMessage)
        {
            try
            {
                CloudQueue queue = this.GetQueue();

                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.DeleteMessageFormQueue() - Deleting the message {0}...", queueMessage.AsString), LogEventID.InformationWrite);
                queue.DeleteMessage(queueMessage, this.queueRequestOptions);
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.Azure.Framework.Provisioning.AzureQueueStorage.DeleteMessageFormQueue() - Deleted message {0}.", queueMessage.AsString), LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured at JCI.Azure.Framework.Provisioning.AzureQueueStorage.DeleteMessageFormQueue().");
                LogHelper.LogError(ex);
            }
        }
    }
}
