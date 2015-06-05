//-----------------------------------------------------------------------
// <copyright file= "Program.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.PersonalSitesTransformationJob
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.PersonalSitesTransformationJob.Helpers;
    using Microsoft.Azure.WebJobs;    

    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            LogHelper.LogInformation("Starting personal sites transformation job execution...", LogEventID.InformationWrite);

            OnlinePersonalSitesTransformationJobHelper onlinePersonalSitesTransformationJobHelper = new OnlinePersonalSitesTransformationJobHelper();
            OnPremisePersonalSitesTransformationJobHelper onPremisePersonalSitesTransformationJobHelper = new OnPremisePersonalSitesTransformationJobHelper();

            if (GlobalData.SharePointOnPremKey)
            {
                onPremisePersonalSitesTransformationJobHelper.ProcessTransformationJob();                
            }
            else
            {
                onlinePersonalSitesTransformationJobHelper.ProcessTransformationJob();
            }

            LogHelper.LogInformation("Completed personal sites transformation job execution.", LogEventID.InformationWrite);
        }
    }
}
