//-----------------------------------------------------------------------
// <copyright file= "Program.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.ConsoleApp
{    
    using System;
    using JCI.CAM.Common;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Main Class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                string input = string.Empty;

                do
                {
                    Console.WriteLine();
                    Console.WriteLine("1 - Deploy Fields and Content Types in CT Hub");
                    Console.WriteLine("2 - Configure Site Provisioning Site");
                    Console.WriteLine("3 - Provision site migration related lists");
                    Console.WriteLine("4 - Activate Sandbox Solution");
                    Console.WriteLine("5 - Add script to Site Request approval task display form");
                    Console.WriteLine("Q - Quit");
                    Console.Write("Please enter an option: ");
                    input = Console.ReadLine();

                    if (!input.Equals("Q"))
                    {
                        GetCredentials();
                    }

                    switch (input.ToUpper(System.Globalization.CultureInfo.CurrentCulture))
                    {
                        case "1":
                            CallDeployFieldsAndContentTypes();
                            break;
                        case "2":
                            CallConfigureSiteProvisioningSite();                           
                            break;
                        case "3":
                            CallConfigureSiteMigrationRequestList();
                            break;
                        case "4":
                            ////CallActivatingSandBoxSolution();
                            Console.WriteLine("This functionality is now moved to Azure Web Job.");
                           break;
                        case "5":
                            CallConfigureSiteDispFormPage();
                            break;
                        case "Q":
                            break;
                        default:
                            Console.WriteLine("Invalid input provided");
                            break;
                    }
                    
                    Console.WriteLine("Please press enter to go back to menu ...");
                    Console.ReadLine();
                }
                while (input.ToUpper(System.Globalization.CultureInfo.CurrentCulture) != "Q");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }
            finally
            {
                LogHelper.LogInformation("Press Key to Close Window", LogEventID.InformationWrite);
                Console.WriteLine("**************");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Starts SiteRequest Display Form Page configuration
        /// </summary>
        private static void CallConfigureSiteDispFormPage()
        {
            Console.WriteLine(string.Empty);
            LogHelper.LogInformation("Creating Client Context ...", LogEventID.InformationWrite);
            
            using (var context = new ClientContext(GlobalData.MigrationRequestSiteUrl))
            {
                if (GlobalData.Environment == Constants.OnPremiseEnvironment || GlobalData.Environment == Constants.DedicatedEnvironment)
                {
                    context.Credentials = new System.Net.NetworkCredential(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword, GlobalData.ProvidedDomain);
                }
                else
                {
                    SharePointOnlineCredentials sharepointOnlineCredentials = new SharePointOnlineCredentials(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword);
                    context.Credentials = sharepointOnlineCredentials;
                }

                ConsoleOperations.AddWebPartToDispForm(context);
            }
        }

        /// <summary>
        /// Calls to configure site migration request list.
        /// </summary>
        private static void CallConfigureSiteMigrationRequestList()
        {
            Console.WriteLine(string.Empty);
            LogHelper.LogInformation("Creating Client Context ...", LogEventID.InformationWrite);

            using (var context = new ClientContext(GlobalData.MigrationRequestSiteUrl))
            {
                if (GlobalData.Environment == Constants.OnPremiseEnvironment || GlobalData.Environment == Constants.DedicatedEnvironment)
                {
                    context.Credentials = new System.Net.NetworkCredential(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword, GlobalData.ProvidedDomain);
                }
                else
                {
                    SharePointOnlineCredentials sharepointOnlineCredentials = new SharePointOnlineCredentials(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword);
                    context.Credentials = sharepointOnlineCredentials;
                }

                ConsoleOperations.ConfigureSiteMigrationRequestList(context);
            }
        }

        /// <summary>
        /// Calls to configure site provisioning site.
        /// </summary>
        private static void CallConfigureSiteProvisioningSite()
        {
            LogHelper.LogInformation("Creating Client Context ...", LogEventID.InformationWrite);

            using (var context = new ClientContext(GlobalData.SiteProvisioningSiteUrl))
            {
                if (GlobalData.Environment == Constants.OnPremiseEnvironment || GlobalData.Environment == Constants.DedicatedEnvironment)
                {
                    context.Credentials = new System.Net.NetworkCredential(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword, GlobalData.ProvidedDomain);
                }
                else
                {
                    SharePointOnlineCredentials sharepointOnlineCredentials = new SharePointOnlineCredentials(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword);
                    context.Credentials = sharepointOnlineCredentials;
                }

                ConsoleOperations.ConfigureSiteProvisioningLists(context);
                ConsoleOperations.ConfigureSiteProvisioningListsAccess(context);
                ConsoleOperations.AddFieldsToSiteRequestList(context);
                ConsoleOperations.ConfigureSiteRequestListIndexedColumns(context);
                ConsoleOperations.DeployWorkflows(context);
            }
        }

        /// <summary>
        /// Calls the deploy fields and content types.
        /// </summary>
        private static void CallDeployFieldsAndContentTypes()
        {
            Console.WriteLine(string.Empty);
            LogHelper.LogInformation("Creating Client Context ...", LogEventID.InformationWrite);

            using (var context = new ClientContext(GlobalData.ContentTypeHubSiteUrl))
            {
                if (GlobalData.Environment == Constants.OnPremiseEnvironment || GlobalData.Environment == Constants.DedicatedEnvironment)
                {
                    context.Credentials = new System.Net.NetworkCredential(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword, GlobalData.ProvidedDomain);
                }
                else
                {
                    SharePointOnlineCredentials sharepointOnlineCredentials = new SharePointOnlineCredentials(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword);
                    context.Credentials = sharepointOnlineCredentials;
                }

                context.Load(context.Web);
                context.ExecuteQuery();

                Web web = context.Web;

                ConsoleOperations.DeployFieldsAndContentTypes(web);
            }
        }

        /// <summary>
        /// Calls to configure site migration request list.
        /// </summary>
        private static void CallActivatingSandBoxSolution()
        {
            Console.WriteLine(string.Empty);
            
            LogHelper.LogInformation("Creating Client Context ...", LogEventID.InformationWrite);

            using (var context = new ClientContext(GlobalData.MigrationRequestSiteUrl))
            {
                if (GlobalData.Environment == Constants.OnPremiseEnvironment || GlobalData.Environment == Constants.DedicatedEnvironment)
                {
                    context.Credentials = new System.Net.NetworkCredential(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword, GlobalData.ProvidedDomain);
                }
                else
                {
                    SharePointOnlineCredentials sharepointOnlineCredentials = new SharePointOnlineCredentials(GlobalData.ProvidedUserName, GlobalData.ProvidedPassword);
                    context.Credentials = sharepointOnlineCredentials;
                }

                ConsoleOperations.ActivateSandBoxSolution(context);
            }
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        private static void GetCredentials()
        {
            ConsoleKeyInfo key;
            bool retryUserNameInput = false;
            string domainUserName = string.Empty;
            string password = string.Empty;

            do
            {
                Console.WriteLine(@"Please provide username (Ex: <<Domain>>\<<User Name>>)");

                domainUserName = Console.ReadLine();

                string createType = GlobalData.Environment;
                if (createType.Equals(Constants.OnPremiseEnvironment, StringComparison.OrdinalIgnoreCase) || createType.Equals(Constants.DedicatedEnvironment, StringComparison.OrdinalIgnoreCase))
                {
                    if (domainUserName.Contains("\\"))
                    {
                        string[] userName = domainUserName.Split('\\');
                        GlobalData.ProvidedDomain = userName[0];
                        GlobalData.ProvidedUserName = userName[1];
                        retryUserNameInput = false;
                    }
                    else
                    {
                        Console.WriteLine(@"User name not provided in format <<Domain>>\<<User Name>>");
                        retryUserNameInput = true;
                    }
                }
                else
                {
                    GlobalData.ProvidedUserName = domainUserName;
                }
            }
            while (retryUserNameInput);

            Console.WriteLine("Please provide password");

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.CursorLeft--;
                        Console.Write('\0');
                        Console.CursorLeft--;
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);

            GlobalData.ProvidedPassword = SecurityExtensions.CreateSecureString(password.TrimEnd('\r'));
        }
    }
}
