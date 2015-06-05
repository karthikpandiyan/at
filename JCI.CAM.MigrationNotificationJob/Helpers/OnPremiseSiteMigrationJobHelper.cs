// <copyright file="OnPremiseSiteMigrationJobHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Site Migration job 
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.SiteMigrationJob.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;    
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Migration.Common.Helpers;
    using JCI.CAM.Provisioning.Core; 
    using JCI.CAM.Provisioning.Core.Authentication;
    using JCI.CAM.SiteMigrationJob.Entities;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;       
    using Microsoft.SharePoint.Client.WebParts;       

    /// <summary>
    /// Site Migration Job Helper Class
    /// </summary>
    public class OnPremiseSiteMigrationJobHelper : SiteMigrationJobHelper
    {        
    }
}
