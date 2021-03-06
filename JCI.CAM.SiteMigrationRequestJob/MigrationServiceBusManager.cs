﻿// <copyright file="MigrationServiceBusManager.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
// Site migration request service bus manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.SiteMigrationRequestJob
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using JCI.Azure.Framework.Provisioning;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common.Entity; 
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;       

    /// <summary>
    /// Implementation class for working with Azure Service Bus and working with the Site Migration Request
    /// </summary>
    public class MigrationServiceBusManager
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
        #endregion

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
        /// Gets or sets Azure Request Queue Name that is used to send messages to the Request Queue for the Migration engine.
        /// </summary>
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

        #endregion

        /// <summary>
        /// Member to Send a Site Migration Request Message to the Migration Engine.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Azure Configuration missing in the config file
        /// </exception>
        /// <exception cref="ConfigurationErrorsException">Azure Configuration missing in the config file</exception>
        public void SendMigrationRequest(SiteMigrationRequestMessage payload)
        {
            LogHelper.LogInformation("Started sending message request to queue", LogEventID.InformationWrite);
            var nameSpaceManager = this.GetNameSpaceManager();

            if (string.IsNullOrEmpty(this.AzureConnectionString))
            {
                throw new ConfigurationErrorsException(
                    string.Format("Azure Configuration - AzureConnectionString is missing in the config file"));
            }

            if (string.IsNullOrEmpty(this.RequestQueueName))
            {
                throw new ConfigurationErrorsException(
                   string.Format("Azure Configuration - RequestQueueName is missing in the config file"));
            }

            try
            {                
                QueueClient client = this.GetQueueClient(nameSpaceManager, this.RequestQueueName);
                using (BrokeredMessage message = new BrokeredMessage(payload))
                {
                    client.Send(message);

                    // LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.ServiceBusManager.SendMessage - Successfully Sent Messages {0} to Queue {1} ", payload.SiteRequest, this.RequestQueueName), LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, new string[] { payload.SiteMigrationRequest, this.RequestQueueName });
                throw;
            }
        }

        /// <summary>
        /// Used to Send Response Messages from the Site Migration Engine.
        /// If replyTo is null or whitespace an ArgumentException will be thrown
        /// </summary>
        /// <param name="message">The Response Message</param>
        /// <param name="replyTo">The Queue Name to send the response</param>
        /// <exception cref="System.ArgumentException">reply To</exception>
        /// <exception cref="ArgumentException">Occurs if a passed argument is invalid</exception>
        public void SendReplyMessage(ProvisioningResponseMessage message, string replyTo)
        {
            LogHelper.LogInformation("JCI.CAM.SiteMigrationRequestJob.Helpers.ServiceBusManager.SendReplyMessage - Started sending message request to replyto queue", LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(replyTo))
            {
                throw new ArgumentException("replyTo");
            }

            var nameSpaceManager = this.GetNameSpaceManager();

            try
            {
                QueueClient client = this.GetQueueClient(nameSpaceManager, replyTo);
                using (BrokeredMessage brokeredMessage = new BrokeredMessage(message))
                {
                    client.Send(brokeredMessage);
                    LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.ServiceBusManager.SendReplyMessage - Successfully Sent Messages {0} to Queue {1} ", message.ToString(), replyTo), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling, new string[] { message.SiteRequest, replyTo });
            }
        }

        /// <summary>
        /// Returns a Request Message from the Queue. The method will return null if no message exists
        /// or is not in a valid format.
        /// </summary>
        /// <returns>
        /// A Message object containing the RequestMessage.
        /// </returns>
        public SiteMigrationRequestMessage GetMessage()
        {
            LogHelper.LogInformation("JCI.CAM.SiteMigrationRequestJob.Helpers.ServiceBusManager.GetMessage - Started reading message from queue queue", LogEventID.InformationWrite);
            var nameSpaceManager = this.GetNameSpaceManager();
            QueueClient client = this.GetRequestQueueClientForRead(nameSpaceManager, ReceiveMode.ReceiveAndDelete);

            var message = client.Receive(TimeSpan.FromSeconds(10));
            if (message != null)
            {
                try
                {
                    LogHelper.LogInformation("Deserializing message to MigrationRequestMessage object", LogEventID.InformationWrite);
                    var requestMessage = message.GetBody<SiteMigrationRequestMessage>();

                    // This will return null if its not our message we just ignore it
                    if (requestMessage != null)
                    {
                        return requestMessage;
                    }                  
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex, LogEventID.ExceptionHandling, null);
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Private Member to Return the NameSpaceManager. AzureConnectionString Property is used to create
        /// the NamespaceManager
        /// </summary>
        /// <returns>Namespace Manager</returns>
        private NamespaceManager GetNameSpaceManager()
        {
            LogHelper.LogInformation(string.Format("Getting Namespce manager for Azure Connection {0}", this.AzureConnectionString), LogEventID.InformationWrite);
            return NamespaceManager.CreateFromConnectionString(this.AzureConnectionString);
        }

        /// <summary>
        /// Returns a Azure QueueClient. If the Queue does not exist the queue will be created.
        /// </summary>
        /// <param name="nameSpaceManager">Azure NameSpaceManager</param>
        /// <param name="queueName">Queue name of the Azure Queue.</param>
        /// <returns>Azure QueueClient</returns>
        private QueueClient GetQueueClient(NamespaceManager nameSpaceManager, string queueName)
        {
            if (!nameSpaceManager.QueueExists(this.RequestQueueName))
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.ServiceBusManager.GetQueueClient - Queue {0} doesn't exist. Creating it", this.RequestQueueName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                nameSpaceManager.CreateQueue(this.RequestQueueName);
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.ServiceBusManager.GetQueueClient - Successfully created Queue {0} ", this.RequestQueueName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }

            return QueueClient.CreateFromConnectionString(this.AzureConnectionString, queueName);
        }

        /// <summary>
        /// Gets the QueueClient for Reading operations. Uses RequestQueueName property to connect to the Queue.
        /// If the Queue does not exist the queue will be created.
        /// </summary>
        /// <param name="nameSpaceManager">Azure NameSpaceManager</param>
        /// <param name="mode">Azure ReceiveMode</param>
        /// <returns>Azure QueueClient</returns>
        private QueueClient GetRequestQueueClientForRead(NamespaceManager nameSpaceManager, ReceiveMode mode)
        {
            if (!nameSpaceManager.QueueExists(this.RequestQueueName))
            {
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.ServiceBusManager.GetQueueClient - Queue {0} doesn't exist creating it", this.RequestQueueName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                nameSpaceManager.CreateQueue(this.RequestQueueName);
                LogHelper.LogInformation(string.Format(CultureInfo.InvariantCulture, "JCI.CAM.SiteMigrationRequestJob.Helpers.GetQueueClient - Successfully created Queue {0} ", this.RequestQueueName), JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            }

            return QueueClient.CreateFromConnectionString(this.AzureConnectionString, this.RequestQueueName, mode);
        }
    }
}
