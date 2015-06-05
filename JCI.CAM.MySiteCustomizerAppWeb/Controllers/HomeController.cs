//-----------------------------------------------------------------------
// <copyright file= "HomeController.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.MySiteCustomizerAppWeb.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Home controller
    /// </summary>
    [SharePointContextFilter]
    public class HomeController : Controller
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Return view</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}