//-----------------------------------------------------------------------
// <copyright file= "Global.asax.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.AutoTaggingAppWeb
{    
    using System;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using JCI.CAM.Common.Logging;

    /// <summary>
    /// MVC Application
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application_s the start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
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
