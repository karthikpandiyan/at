// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingInterceptionBehavior.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   The Logging Interception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Logging interception. Gets called before invoking the Methods for Types registered in Unity.Config
    /// Both Invocation and Return are caught
    /// </summary>
    public class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        /// <summary>
        /// Formatted string for logging.
        /// </summary>
        private const string MessageInvokingMethod = "Invoking method {0} on Object {1}";

        /// <summary>
        /// Formatted string for logging.
        /// </summary>
        private const string MessageReturn = "Completed method {0} on Object {1}";

        /// <summary>
        /// Formatted string for logging.
        /// </summary>
        private const string MessageThrowEx = "Method {0} on Object {1} threw exception";

        /// <summary>
        /// Gets a value indicating whether to execute the interceptor.
        /// </summary>
        public bool WillExecute
        {
            get { return true; }
        }

        /// <summary>
        /// Executed when a method in the proxy class is invoked
        /// </summary>
        /// <param name="input">Method details</param>
        /// <param name="getNext">Next interceptor</param>
        /// <returns>Method return arguments</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (getNext == null)
            {
                throw new ArgumentNullException("getNext");
            }

            // Before invoking the method on the original target.
            // Log the method name, and the Object it is part of
            LogHelper.LogInformation(string.Format(MessageInvokingMethod, input.MethodBase.Name, input.Target.ToString()), LogEventID.LoggingInterceptionBehavior);

            // Invoke the next behavior in the chain.
            var result = getNext()(input, getNext);

            // After invoking the method on the original target.
            if (result.Exception != null)
            {
                LogHelper.LogInformation(string.Format(MessageThrowEx, input.MethodBase.Name, input.Target.ToString()), LogEventID.LoggingInterceptionBehavior);
            }
            else
            {
                LogHelper.LogInformation(string.Format(MessageReturn, input.MethodBase.Name, input.Target.ToString()), LogEventID.LoggingInterceptionBehavior);
            }

            return result;
        }

        /// <summary>
        /// Gets a collection of the required interceptor types.
        /// </summary>
        /// <returns>Collection of the interception types.</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }
    }
}
