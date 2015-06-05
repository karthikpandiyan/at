// <copyright file="OnPremisePostTranformationJobActivities.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Post Tranformation Job Activities
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PostTransformationActivitiesJob.Helpers
{ 
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Migration.Common;
    using JCI.CAM.Provisioning.Core.Authentication;
    using Microsoft.Online.SharePoint.TenantAdministration;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Publishing;

    /// <summary>
    /// Post Transformation Job Activities
    /// </summary>
    public class OnPremisePostTranformationJobActivities : PostTranformationJobActivities
    {
    }
}
