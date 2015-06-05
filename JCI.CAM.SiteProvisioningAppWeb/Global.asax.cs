﻿// <auto-generated />
namespace JCI.CAM.SiteProvisioningAppWeb
{
    using System;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using JCI.CAM.Common.Logging;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = false;

        }

        private void Application_Error(object sender, EventArgs e)
        {
            //// Code that runs when an unhandled error occurs
            //// Get the exception object.
            Exception exc = Server.GetLastError();

            //// Log the exception and notify system operators
            LogHelper.LogError(exc);

            //// TODO: Redirect to error page.
            
            //// Clear the error from the server
            Server.ClearError();
        }
    }
}
