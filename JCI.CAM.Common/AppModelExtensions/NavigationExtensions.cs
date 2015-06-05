// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationExtensions.cs" company="Microsoft">
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
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Navigation node extension
    /// </summary>
    public static partial class NavigationExtensions
    {
        #region Custom actions
        /// <summary>
        /// Adds custom action to a web. If the CustomAction exists the item will be updated.
        /// Setting CustomActionEntity.Remove == true will delete the CustomAction.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="customAction">Information about the custom action be added or deleted</param>
        /// <returns>
        /// True if action was success
        /// </returns>        
        public static bool AddCustomAction(this Web web, CustomActionEntity customAction)
        {
            LogHelper.LogInformation("Add custom actions.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
            var existingActions = web.UserCustomActions;
            web.Context.Load(existingActions);
            web.Context.ExecuteQuery();

            var targetAction = web.UserCustomActions.FirstOrDefault(uca => uca.Name == customAction.Name);

            if (customAction.Remove)
            {
                targetAction.DeleteObject();
                web.Context.ExecuteQuery();                
            }

            if (targetAction == null)
            {
                targetAction = existingActions.Add();
            }
            
            targetAction.Name = customAction.Name;
            targetAction.Description = customAction.Description;
            targetAction.Location = customAction.Location;

            if (customAction.Location == JavaScriptExtensions.SCRIPTLOCATION)
            {
                customAction.ScriptBlock = ListExtensions.ReplaceUrlTokens(customAction.ScriptBlock);
                targetAction.ScriptBlock = customAction.ScriptBlock;
                customAction.ScriptSrc = ListExtensions.ReplaceUrlTokens(customAction.ScriptSrc);
                targetAction.ScriptSrc = customAction.ScriptSrc;
            }
            else
            {
                targetAction.Sequence = customAction.Sequence;
                customAction.Url = ListExtensions.ReplaceUrlTokens(customAction.Url);
                targetAction.Url = customAction.Url;
                targetAction.Group = customAction.Group;
                targetAction.Title = customAction.Title;
                customAction.ImageUrl = ListExtensions.ReplaceUrlTokens(customAction.ImageUrl);
                targetAction.ImageUrl = customAction.ImageUrl;
                targetAction.RegistrationId = customAction.RegistrationId;
                targetAction.CommandUIExtension = customAction.CommandUIExtension;

                if (customAction.RightsPermissions != null)
                {
                    targetAction.Rights = customAction.RightsPermissions;
                }

                if (customAction.RegistrationType.HasValue)
                {
                    targetAction.RegistrationType = customAction.RegistrationType.Value;
                }
            }

            targetAction.Update();
            web.Context.Load(web, w => w.UserCustomActions);
            web.Context.ExecuteQuery();
            LogHelper.LogInformation("User custom action added.", JCI.CAM.Common.Logging.LogEventID.InformationWrite);
                    return true;
                }

        #endregion
    }
}
