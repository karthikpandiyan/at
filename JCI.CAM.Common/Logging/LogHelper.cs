// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogHelper.cs"  company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   The LogHelper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.Logging
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Logging class contains methods for logging the exceptions and events
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Name of the trace source
        /// </summary>
        private static readonly string TraceSourceName = "JCICAMTraceSource";

        /// <summary>
        /// Trace Source instance for Logging
        /// </summary>
        private static readonly TraceSource TraceLogSource = new TraceSource(TraceSourceName, SourceLevels.All);

        /// <summary>
        /// Method to Log Information
        /// </summary>
        /// <param name="message">Logging message</param>
        /// <param name="eventId">event id of application</param>
        public static void LogInformation(string message, int eventId = LogEventID.InformationWrite)
        {
            try
            {
                // message = DateTime.Now.ToString() + " - " + message; // Add Timestamp
                Write(TraceEventType.Information, eventId, message);
            }
            catch
            {
                // suppress if logging is failing
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="logException">The log exception.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="additionalInfo">The additional information.</param>
        public static void LogError(Exception logException, int eventId = 0, params string[] additionalInfo)
        {
            try
            {
                if (logException != null)
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception Message: {0}, Additional Info:{1}, Stack Trace : {2}",
                        logException.Message,
                        string.Join(":", additionalInfo),
                        logException.StackTrace);
                    Write(TraceEventType.Error, eventId, message);
                }
            }
            catch
            {
                // suppress if logging is failing
            }
        }

        /// <summary>
        /// Method to add logs
        /// </summary>
        /// <param name="traceType">Error Or warning</param>
        /// <param name="eventId">Event is to be logged</param>
        /// <param name="message">The Message to be logged</param>
        private static void Write(TraceEventType traceType, int eventId, string message)
        {
            if (traceType == TraceEventType.Error)
            {
                System.Diagnostics.Trace.TraceError(message);
            }
            else if (traceType == TraceEventType.Warning)
            {
                System.Diagnostics.Trace.TraceWarning(message);
            }
            else
            {
                System.Diagnostics.Trace.TraceInformation(message);
            }

            TraceLogSource.TraceEvent(traceType, eventId, message);
        }
    }
}
