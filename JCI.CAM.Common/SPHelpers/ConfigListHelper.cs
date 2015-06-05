//-----------------------------------------------------------------------
// <copyright file="ConfigListHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.SPHelpers
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    
    /// <summary>
    /// Config List Helper
    /// </summary>
    public static class ConfigListHelper
    {
        /// <summary>
        /// The site policy name
        /// </summary>
        public const string SitePolicyName = "SitePolicyName";

        /// <summary>
        /// The property bag XML
        /// </summary>
        public const string PropertyBagXml = "SiteDirectoryListMetadataColumnsXMLKey";

        /// <summary>
        /// The configuration list name
        /// </summary>
        public const string ConfigurationListNameKey = "GlobalConfigurationListName";

        /// <summary>
        /// The configuration list key field
        /// </summary>
        public const string ConfigurationListKeyField = "Title";

        /// <summary>
        /// The configuration list value field
        /// </summary>
        public const string ConfigurationListValueField = "Value";

        /// <summary>
        /// The configuration list query
        /// </summary>
        public const string ConfigurationListQuery = "<View><Query><Where><Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>";

        /// <summary>
        /// XPath XML configuration value
        /// </summary>
        public const string SiteMetadataColumnXPath = "/Columns/Column";

        /// <summary>
        /// The site approver list name
        /// </summary>
        public const string SiteApproverListName = "SiteApprover";

        /// <summary>
        /// The approver list region field
        /// </summary>
        public const string ApproverListRegionField = "Region1";

        /// <summary>
        /// The approver list business unit field
        /// </summary>
        public const string ApproverListBusinessUnitField = "BusinessUnit1";

         /// <summary>
        /// The approver list site template field
        /// </summary>
        public const string ApproverListSiteTemplateUnitField = "SiteTemplate";

        /// <summary>
        /// The business unit admin field value
        /// </summary>
        public const string BusinessUnitAdminFieldValue = "BusinessUnitAdminFieldValue";

        /// <summary>
        /// The business unit admin email field value
        /// </summary>
        public const string BusinessUnitAdminEmailFieldValue = "BusinessUnitAdminEmailFieldValue";

        /// <summary>
        /// The get BU Admin CAML query of the site request
        /// </summary>
        public const string GetBuAdminQuery = "<View><Query><Where><And><And><Eq><FieldRef Name='{0}' /><Value Type='Choice'>{1}</Value></Eq><Eq><FieldRef Name='{2}' /><Value Type='Choice'>{3}</Value></Eq></And><Eq><FieldRef Name='{4}' /><Value Type='Choice'>{5}</Value></Eq></And></Where></Query></View>";

        /// <summary>
        /// Gets the configuration list value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="listTitle">The list title.</param>
        /// <returns>
        /// Config value
        /// </returns>
        public static string GetConfigurationListValue(ClientContext context, Web web, string keyName, string listTitle)
        {
            string fieldValue = string.Empty;
            var query = new CamlQuery();
            query.ViewXml = string.Format(ConfigurationListQuery, ConfigurationListKeyField, keyName);
            context.Load(web, w => w.Lists);
            var list = web.Lists.GetByTitle(listTitle);
            context.Load(list);
            var listItemCollection = list.GetItems(query);
            context.Load(listItemCollection, eachItem => eachItem.Include(item => item, item => item[ConfigurationListKeyField], item => item[ConfigurationListValueField]));
            context.ExecuteQuery();

            if (listItemCollection.Count > 0)
            {
                var listItem = listItemCollection.FirstOrDefault();

                if (listItem[ConfigurationListValueField] != null)
                {
                    fieldValue = listItem[ConfigurationListValueField].ToString();
                }
            }
            else
            {
                LogHelper.LogInformation(string.Format("GetConfigurationListValue - Could not get Config value for key {0}", keyName), LogEventID.InformationWrite);
            }

            return fieldValue;
        }

        /// <summary>
        /// Gets the business admin group.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="web">The web.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="businessUnit">The business unit.</param>
        /// <param name="templateType">Type of the template.</param>
        /// <returns>
        /// Business unit owner lookup value
        /// </returns>
        public static Dictionary<string, string> GetBusinessAdminGroup(ClientContext context, Web web, string regionName, string businessUnit, string templateType)
        {           
            Dictionary<string, string> fieldsValue = new Dictionary<string, string>();
            var query = new CamlQuery();            
            query.ViewXml = string.Format(GetBuAdminQuery, ApproverListRegionField, regionName, ApproverListBusinessUnitField, businessUnit, ApproverListSiteTemplateUnitField, templateType);
            context.Load(web, w => w.Lists);
            var list = web.Lists.GetByTitle(SiteApproverListName);
            context.Load(list);
            var listItemCollection = list.GetItems(query);
            context.Load(listItemCollection, eachItem => eachItem.Include(item => item, item => item[Constants.SiteTemplateField], item => item[Constants.BusinessUnitAdminField], item => item[ApproverListRegionField], item => item[ApproverListBusinessUnitField], item => item[Constants.BusinessUnitAdminEmailField]));
            context.ExecuteQuery();

            if (listItemCollection.Count > 0)
            {
                foreach (var item in listItemCollection)
                {
                    if (item[Constants.SiteTemplateField] != null && item[Constants.SiteTemplateField].ToString() == templateType)
                    {
                        FieldUserValue fieldUser = null;

                        if (item[Constants.BusinessUnitAdminField] is FieldUserValue[])
                        {
                            FieldUserValue[] fieldUsers = item[Constants.BusinessUnitAdminField] as FieldUserValue[];

                            if (fieldUsers.Length > 0)
                            {
                                fieldUser = fieldUsers.FirstOrDefault();
                            }
                        }
                        else
                        {
                            fieldUser = (FieldUserValue)item[Constants.BusinessUnitAdminField];
                        }

                        fieldsValue.Add(BusinessUnitAdminFieldValue, fieldUser.LookupValue);
                        if (item[Constants.BusinessUnitAdminEmailField] != null)
                        {
                            fieldsValue.Add(BusinessUnitAdminEmailFieldValue, item[Constants.BusinessUnitAdminEmailField] as string);
                        }

                        break;
                    }
                }                
            }
            else
            {
                LogHelper.LogInformation("GetBusinessAdminGroup - Could not get Bu Amins", LogEventID.InformationWrite);
            }

            return fieldsValue;
        }
    }
}
