﻿// <auto-generated/>
using System;
using JCI.CAM.Common.Logging;

namespace JCI.CAM.BrandingCustomizationAppWeb
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Application_Error(object sender, EventArgs e)
        {
            //// Code that runs when an unhandled error occurs
            //// Get the exception object.
            Exception exc = Server.GetLastError();

            //// Log the exception and notify system operators
            LogHelper.LogError(exc);

            //// Clear the error from the server
            Server.ClearError();
        }
    }
}