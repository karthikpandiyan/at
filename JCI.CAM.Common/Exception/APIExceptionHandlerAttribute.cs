// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiExceptionHandlerAttribute.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   The Exception Hanlder class for API Controllers.
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.APIException
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.Resources;

    /// <summary>
    /// Handler attribute for API Controllers to handle exceptions
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = true)]
    public class ApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Handles exceptions from Controller Actions and Filters. 
        /// HttpResponseExceptions are skipped by this.
        /// </summary>
        /// <param name="actionExecutedContext">ExceptionContext object</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            LogHelper.LogInformation("Enter inside ApiExceptionHandlerAttribute.Onexception method", LogEventID.ExceptionHandling);

            Exception exception = null;

            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
                // Get the inner exception if one exists. 
                exception = (actionExecutedContext.Exception.InnerException == null) ? actionExecutedContext.Exception : actionExecutedContext.Exception.InnerException;
                LogHelper.LogError(exception, LogEventID.ExceptionHandling);

                var exceptionType = exception.GetType();

                var responseMsg = new HttpResponseMessage();

                // Controllers would throw ObjectNotFoundException, if the requested record doesn't exist.
                // Return HTTP 404 
                if (exceptionType == typeof(UnauthorizedAccessException))
                {
                    //// Return HTTP 401 
                    responseMsg.StatusCode = HttpStatusCode.Unauthorized;
                    responseMsg.ReasonPhrase = CommonResources.UserDoesNotHaveAccess;
                }
                else if (exceptionType == typeof(SqlException))
                {
                    // Status code is HTTP 500
                    // But change the message
                    responseMsg.StatusCode = HttpStatusCode.InternalServerError;
                    responseMsg.ReasonPhrase = CommonResources.ErrorInDataBaseOperation;
                }
                else
                {
                    // Status code is HTTP 500
                    // Unknown error
                    responseMsg.StatusCode = HttpStatusCode.InternalServerError;
                    responseMsg.ReasonPhrase = CommonResources.UnknownErrorInPerformingOperation;
                }

                actionExecutedContext.Response = responseMsg;
                responseMsg.Content = new StringContent(exception.Message);
            }

            LogHelper.LogInformation("Exiting ApiExceptionHandlerAttribute.Onexception method", LogEventID.ExceptionHandling);
        }
    }
}
