//-----------------------------------------------------------------------
// <copyright file= "MigrationConstants.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.ConsoleApp
{
    /// <summary>
    /// Migration code related constants
    /// </summary>
    public static class MigrationConstants
    {       
        /// <summary>
        /// Solution Gallery library name
        /// </summary>
        public const string SolutionGallery = "Solution Gallery";

        /// <summary>
        /// List definition xml node-list
        /// </summary>
        public const string ListDefinitionXMLNodeList = "/ListDefinition/ListTemplate";

        /// <summary>
        /// List definition xml Type attribute
        /// </summary>
        public const string ListDefinitionXMLSourceTemplateAttribute = "SourceType";

        /// <summary>
        /// List definition xml Type attribute
        /// </summary>
        public const string ListDefinitionXMLTargetTemplateAttribute = "TargetType";

        /// <summary>
        /// Site Title column
        /// </summary>
        public const string SiteTitleColumn = "Title";

        /// <summary>
        /// Site URL column
        /// </summary>
        public const string SiteURLColumn = "JCISiteUrl";

        /// <summary>
        /// Site migration status column
        /// </summary>
        public const string SiteMigrationStatusColumn = "JCISiteMigrationStatus";

        /// <summary>
        /// Query to fetch the successfully site migration sites
        /// </summary>
        public const string SiteMigrationSuccessCamlQuery = @"<View><Query><Where><Eq><FieldRef Name='" + SiteMigrationStatusColumn + "' /><Value Type='Choice'>Success</Value></Eq></Where></Query><RowLimit>1000</RowLimit></View>";

        /// <summary>
        /// Query to fetch the sandbox solution
        /// </summary>
        public const string SandboxSolutionQuery = @"<View><Query><Where><Eq><FieldRef Name='FileLeafRef' /><Value Type='File'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>";

        /// <summary>
        /// Ready Status column
        /// </summary>
        public const string SiteMigrationReadyStatusColumn = "Ready_x0020_for_x0020_List_x00200";        

        /// <summary>
        /// Provision status column
        /// </summary>
        public const string ProvisionStatusColumn = "List_x0020_Definition_x0020_Migr";

        /// <summary>
        /// Error data column
        /// </summary>
        public const string ErrorDataColumn = "List_x0020_Definition_x0020_Migr0";

        /// <summary>
        /// Query to fetch the site details to provision the list instance
        /// </summary>
        public const string ProvisionCamlQuery = @"<View><Query><Where><And><Eq><FieldRef Name='" + SiteMigrationReadyStatusColumn + "' /><Value Type='Boolean'>1</Value></Eq><Or><Eq><FieldRef Name='" + ProvisionStatusColumn + "' /><Value Type='Choice'>Not Started</Value></Eq><Eq><FieldRef Name='" + ProvisionStatusColumn + "' /><Value Type='Choice'>Error</Value></Eq></Or></And></Where></Query><RowLimit>100</RowLimit></View>";

        /// <summary>
        /// Duplicate content type error string
        /// </summary>
        public const string DuplicateContentTypeErrorString = "duplicate";
    }
}
