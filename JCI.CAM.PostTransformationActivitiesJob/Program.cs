// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Program Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace JCI.CAM.PostTransformationActivitiesJob
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.PostTransformationActivitiesJob.Helpers;
    using Microsoft.Azure.WebJobs;

    /// <summary>
    /// Executes site migration job
    /// </summary>
    /// <param name="args">Args instance</param>
    public class Program
    {
        /// <summary>
        /// Executes site migration job
        /// </summary>
        /// <param name="args">Args instance</param>
        public static void Main(string[] args)
        {
            try
            {
                LogHelper.LogInformation("Starting post transformation activities job...", LogEventID.InformationWrite);

                OnPremisePostTranformationJobActivities onPremisePostTranformationJobActivities = new OnPremisePostTranformationJobActivities();
                OnlinePostTranformationJobActivities onlinePostTranformationJobActivities = new OnlinePostTranformationJobActivities();

                if (GlobalData.SharePointOnPremKey)
                {
                    onPremisePostTranformationJobActivities.PostTransformationActivitiesJob();
                }
                else
                {
                    onlinePostTranformationJobActivities.PostTransformationActivitiesJob();
                } 

                LogHelper.LogInformation("Completed post transformation activities job.", LogEventID.InformationWrite);
            }
            catch (Exception ex)
            {
                LogHelper.LogInformation("Error occured while running the post transformation activities job.", LogEventID.InformationWrite);
                LogHelper.LogError(ex);
            }
        }
    }
}
