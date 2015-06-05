//-----------------------------------------------------------------------
// <copyright file= "Default.aspx.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.SPDSettingsAppWeb
{
    using System;

    /// <summary>
    /// SPD settings
    /// </summary>
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Page pre-initialization event
        /// </summary>
        /// <param name="sender">Object instance</param>
        /// <param name="e">Event args instance</param>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(this.Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender">Object instance</param>
        /// <param name="e">Event args instance</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var context = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = context.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                Response.Write(clientContext.Web.Title);
            }
        }
    }
}