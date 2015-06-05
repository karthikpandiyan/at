//-----------------------------------------------------------------------
// <copyright file= "ClientContextExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;    

    /// <summary>
    /// ClientContext Extensions methods
    /// </summary>
    public static class ClientContextExtensions
    {
        /// <summary>
        /// Executes the query retry.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="retryCount">Number of times to retry the request</param>
        /// <param name="delay">Milliseconds to wait before retrying the request. The delay will be increased (doubled) every retry</param>
        public static void ExecuteQueryRetry(this ClientRuntimeContext clientContext, int retryCount = 1, int delay = 500)
        {
            ExecuteQueryImplementation(clientContext, retryCount, delay);
        }       

        /// <summary>
        /// Clones a ClientContext object while "taking over" the security context of the existing ClientContext instance
        /// </summary>
        /// <param name="clientContext">ClientContext to be cloned</param>
        /// <param name="siteUrl">Site url to be used for cloned ClientContext</param>
        /// <returns>A ClientContext object created for the passed site url</returns>
        public static ClientContext Clone(this ClientRuntimeContext clientContext, string siteUrl)
        {
            if (string.IsNullOrWhiteSpace(siteUrl))
            {
                throw new ArgumentException("Url of the site is required.", "siteUrl");
            }

            return clientContext.Clone(new Uri(siteUrl));
        }

        /// <summary>
        /// Clones a ClientContext object while "taking over" the security context of the existing ClientContext instance
        /// </summary>
        /// <param name="clientContext">ClientContext to be cloned</param>
        /// <param name="siteUrl">Site url to be used for cloned ClientContext</param>
        /// <returns>A ClientContext object created for the passed site url</returns>
        public static ClientContext Clone(this ClientRuntimeContext clientContext, Uri siteUrl)
        {
            if (siteUrl == null)
            {
                throw new ArgumentException("Url of the site is required.", "siteUrl");
            }

            ClientContext clonedClientContext = new ClientContext(siteUrl);
            clonedClientContext.AuthenticationMode = clientContext.AuthenticationMode;

            // In case of using networkcredentials in on premises or SharePointOnlineCredentials in Office 365
            if (clientContext.Credentials != null)
            {
                clonedClientContext.Credentials = clientContext.Credentials;
            }
            else
            {
                // Take over the form digest handling setting
                clonedClientContext.FormDigestHandlingEnabled = (clientContext as ClientContext).FormDigestHandlingEnabled;

                // In case of app only or SAML
                clonedClientContext.ExecutingWebRequest += delegate(object objectSender, WebRequestEventArgs webRequestEventArgs)
                {
                    // Call the ExecutingWebRequest delegate method from the original ClientContext object, but pass along the webRequestEventArgs of 
                    // the new delegate method
                    MethodInfo methodInfo = clientContext.GetType().GetMethod("OnExecutingWebRequest", BindingFlags.Instance | BindingFlags.NonPublic);
                    object[] parametersArray = new object[] { webRequestEventArgs };
                    methodInfo.Invoke(clientContext, parametersArray);
                };
            }

            return clonedClientContext;
        }

        /// <summary>
        /// Executes the query implementation.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="delay">The delay.</param>
        /// <exception cref="System.ArgumentException">
        /// Provide a retry count greater than zero.
        /// or
        /// Provide a delay greater than zero.
        /// </exception>
        /// <exception cref="JCI.CAM.Common.AppModelExtensions.ClientContextExtensions.MaximumRetryAttemptedException"></exception>
        private static void ExecuteQueryImplementation(ClientRuntimeContext clientContext, int retryCount = 1, int delay = 500)
        {
            int retryAttempts = 0;
            int backoffInterval = delay;

            if (retryCount <= 0)
            {
                throw new ArgumentException("Provide a retry count greater than zero.");
            }

            if (delay <= 0)
            {
                throw new ArgumentException("Provide a delay greater than zero.");
            }

            // Do while retry attempt is less than retry count
            while (retryAttempts < retryCount)
            {
                try
                {
                    clientContext.ExecuteQuery();
                    return;
                }
                catch (WebException wex)
                {
                    var response = wex.Response as HttpWebResponse;

                    // Check if request was throttled - http status code 429
                    // Check is request failed due to server unavailable - http status code 503
                    if (response != null && (response.StatusCode == (HttpStatusCode)429 || response.StatusCode == (HttpStatusCode)503))
                    {
                        Debug.WriteLine("CSOM request frequency exceeded usage limits. Sleeping for {0} seconds before retrying.", backoffInterval);

                        // Add delay for retry
                        Thread.Sleep(backoffInterval);

                        // Add to retry count and increase delay.
                        retryAttempts++;
                        backoffInterval = backoffInterval * 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new MaximumRetryAttemptedException(string.Format("Maximum retry attempts {0}, has be attempted.", retryCount));
        }        

        /// <summary>
        /// MaximumRetryAttemptedException exception class
        /// </summary>
        [Serializable]
        public class MaximumRetryAttemptedException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MaximumRetryAttemptedException"/> class.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public MaximumRetryAttemptedException(string message)
                : base(message)
            {
            }
        }
    }
}
