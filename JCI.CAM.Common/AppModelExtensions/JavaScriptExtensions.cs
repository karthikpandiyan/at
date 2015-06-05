// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JavaScriptExtensions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  List Extensions
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Java script extension
    /// </summary>
    public static partial class JavaScriptExtensions
    {
        /// <summary>
        /// The script location
        /// </summary>
        public const string SCRIPTLOCATION = "ScriptLink";

        /// <summary>
        /// Injects links to java script files via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">semi colon delimited list of links to java script files</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Web web, string key, string scriptLinks)
        {
            return web.AddJsLink(key, new List<string>(scriptLinks.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)));
        }

        /// <summary>
        /// Injects links to java script files via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptLinks">IEnumerable list of links to java script files</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsLink(this Web web, string key, IEnumerable<string> scriptLinks)
        {
            if (scriptLinks.Count() == 0)
            {
                throw new ArgumentException("Parameter scriptLinks can't be empty");
            }

            StringBuilder scripts = new StringBuilder(@" var headID = document.getElementsByTagName('head')[0]; var ");
            foreach (var link in scriptLinks)
            {
                if (!string.IsNullOrEmpty(link))
                {
                    scripts.AppendFormat(@"newScript = document.createElement('script');newScript.type = 'text/javascript';newScript.src = '{0}';headID.appendChild(newScript);", link);
                }
            }

            bool ret = web.AddJsBlock(key, scripts.ToString());
            return ret;
        }       

        /// <summary>
        /// Injects java script via a adding a custom action to the site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Identifier (key) for the custom action that will be created</param>
        /// <param name="scriptBlock">Java script to be injected</param>
        /// <returns>True if action was ok</returns>
        public static bool AddJsBlock(this Web web, string key, string scriptBlock)
        {
            LogHelper.LogInformation("Add js block to site.", LogEventID.InformationWrite);
            scriptBlock = ListExtensions.ReplaceUrlTokens(scriptBlock);
            var jsaction = new CustomActionEntity()
            {
                Name = key,
                Location = SCRIPTLOCATION,
                ScriptBlock = scriptBlock,
            };
            bool ret = web.AddCustomAction(jsaction);
            LogHelper.LogInformation("Added js block to site.", LogEventID.InformationWrite);
            return ret;
        }
    }
}
