// <copyright file="OnPremisePersonalSitesTransformationJobHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Personal Sites Migration job 
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PersonalSitesTransformationJob.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization; 
    using JCI.CAM.Common;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;    
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Migration.Common.Authentication;
    using JCI.CAM.Migration.Common.Entity;
    using JCI.CAM.Migration.Common.Helpers;    
    using JCI.CAM.Provisioning.Core;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;     

    /// <summary>
    /// On Premise Site Migration Job Helper Class
    /// </summary>
    public class OnPremisePersonalSitesTransformationJobHelper : PersonalSitesTransformationJobHelper
    {
    }
}
