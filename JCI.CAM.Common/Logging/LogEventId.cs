// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEventID.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   The LogEvent ID class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.Logging
{
    /// <summary>
    /// Log Event ID
    /// Assign ranges of Event IDs (like 1001-2000) to a type of task or action
    /// </summary>
    public static class LogEventID
    {
        /// <summary>
        /// The exception handling
        /// Information Event
        /// </summary>
        public const int InformationWrite = 1001;

        /// <summary>
        /// The exception handling
        /// Exception Handling
        /// </summary>
        public const int ExceptionHandling = 5002;

        /// <summary>
        /// The exception handling
        /// Exception Handling
        /// </summary>
        public const int LoggingInterceptionBehavior = 5003;

        /// <summary>
        /// The service helper
        /// </summary>
        public const int ServiceHelper = 5004;

        /// <summary>
        /// The field exits
        /// </summary>
        public const int FieldAlreadyExists = 4501;

        /// <summary>
        /// The content type exits
        /// </summary>
        public const int ContentTypeAlreadyExists = 4502;
    }
}
