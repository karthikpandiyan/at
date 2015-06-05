//-----------------------------------------------------------------------
// <copyright file= "AutoTaggingHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.AutoTaggingAppWeb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JCI.CAM.Common;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.EventReceivers;

    /// <summary>
    /// Auto tagging helper
    /// </summary>
    public class AutoTaggingHelper
    {
        /// <summary>
        /// Query to find the item in the Taxonomy List to get the WSS ID
        /// </summary>
        private const string TaxonomyCamlQuery =
            "<View><Query><Where><Eq><FieldRef Name='Title'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>";

        /// <summary>
        /// Taxonomy formatted string
        /// </summary>
        private const string TaxonomyFormattedString = "{0};#{1}|{2}";

        /// <summary>
        /// Taxonomy hidden list
        /// </summary>
        private const string TaxonomyHiddenList = "TaxonomyHiddenList";

        /// <summary>
        /// Term Id
        /// </summary>
        private const string TaxonomyFieldsIdForTerm = "IdForTerm";

        /// <summary>
        /// The exception message format
        /// </summary>
        private const string ExceptionMessageInvalidArg = "The arguement {0}, is invalid or not supplied.";

        /// <summary>
        /// Assigns the metadata.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="userLoginName">Name of the user login.</param>
        /// <param name="afterProperties">The after properties.</param>
        /// <param name="result">The item to update.</param>
        public static void AssignMetadata(ClientContext ctx, string userLoginName, Dictionary<string, object> afterProperties, SPRemoteEventResult result)
        {
            string[] profileFields =
            {
                UserProfilePropertyHelper.AutoTagBusinessUnitField,
                UserProfilePropertyHelper.AutoTagLocationField,
                UserProfilePropertyHelper.AutoTagLanguageField,
                UserProfilePropertyHelper.AutoTagDataClassificationLevelField
            };
            IEnumerable<string> profileFieldValues = ProfileHelper.GetProfilePropertiesFor(ctx, userLoginName, profileFields);
            var valuesList = profileFieldValues.ToList();
            if (valuesList.Any())
            {
                if (!afterProperties.ContainsKey(Constants.BusinessUnitColumnName))
                {
                    SetBusinessUnit(ctx, result, valuesList[0]);
                }

                if (!afterProperties.ContainsKey(Constants.JciLocationColumnName))
                {
                    SetLocation(ctx, result, valuesList[1]);
                }

                if (!afterProperties.ContainsKey(Constants.JciLanguageColumnName))
                {
                    SetLanguage(ctx, result, valuesList[2]);
                }

                if (!afterProperties.ContainsKey(Constants.DataClassificationLevelColumnName))
                {
                    SetDataClassificationLevel(ctx, result, valuesList[3]);
                }
            }
        }

        /// <summary>
        /// This will return the format of a Taxonomy value.
        /// If the Term is not found this method will return an <see cref="string.Empty" /><example>"2;#MYTERNNAME|74972ac9-3183-4775-b232-cd6de3569c65"</example>
        /// </summary>
        /// <param name="ctx">The Authenticated ClientContext</param>
        /// <param name="term">The term.</param>
        /// <returns>
        /// Taxonomy Formatted string
        /// </returns>
        /// <exception cref="System.ArgumentException">Exception if term is empty</exception>
        public static string GetTaxonomyFormat(ClientContext ctx, string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, ExceptionMessageInvalidArg, "term"));
            }

            string result = string.Empty;

            var rootWeb = ctx.Site.RootWeb;
            ctx.Load(rootWeb);
            ctx.ExecuteQuery();

            // TaxonomyHiddenList is the hidden list on root web
            var list = rootWeb.Lists.GetByTitle(TaxonomyHiddenList);
            CamlQuery camlQuery = new CamlQuery { ViewXml = string.Format(TaxonomyCamlQuery, term) };

            var listItemCollection = list.GetItems(camlQuery);

            ctx.Load(
                listItemCollection,
                eachItem => eachItem.Include(
                    item => item,
                    item => item.Id,
                    item => item[TaxonomyFieldsIdForTerm]));
            ctx.ExecuteQuery();

            if (listItemCollection.Count > 0)
            {
                var item = listItemCollection.FirstOrDefault();
                var wssId = item.Id;
                var termId = item[TaxonomyFieldsIdForTerm].ToString();
                result = string.Format(TaxonomyFormattedString, wssId, term, termId);
            }

            return result;
        }

        /// <summary>
        /// Sets the business unit.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="result">Remote event result.</param>
        /// <param name="profilePropertyValue">The profile property value.</param>
        private static void SetBusinessUnit(ClientContext ctx, SPRemoteEventResult result, string profilePropertyValue)
        {
            if (!string.IsNullOrEmpty(profilePropertyValue))
            {
                string[] profilePropertyValues = profilePropertyValue.Split('|');

                if (profilePropertyValues.Length > 0)
                {
                    profilePropertyValue = profilePropertyValues[0];
                }

                var formatTaxonomy = AutoTaggingHelper.GetTaxonomyFormat(ctx, profilePropertyValue);
                result.ChangedItemProperties.Add(Constants.BusinessUnitColumnName, formatTaxonomy);
                LogHelper.LogInformation(string.Format("Add taxonomy term value {0} to list on changedItemProperties of column {1} to get taxonomy format.", formatTaxonomy, Constants.BusinessUnitColumnName), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Sets the location.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="result">The Remote Event result.</param>
        /// <param name="profilePropertyValue">The profile property value.</param>
        private static void SetLocation(ClientContext ctx, SPRemoteEventResult result, string profilePropertyValue)
        {
            if (!string.IsNullOrEmpty(profilePropertyValue))
            {
                string[] profilePropertyValues = profilePropertyValue.Split('|');

                if (profilePropertyValues.Length > 0)
                {
                    profilePropertyValue = profilePropertyValues[0];
                }

                var formatTaxonomy = AutoTaggingHelper.GetTaxonomyFormat(ctx, profilePropertyValue);
                result.ChangedItemProperties.Add(Constants.JciLocationColumnName, formatTaxonomy);
                LogHelper.LogInformation(string.Format("Add taxonomy term value {0} to list on changedItemProperties of column {1} to get taxonomy format.", formatTaxonomy, Constants.JciLocationColumnName), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Sets the language.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="result">The Remote Event result.</param>
        /// <param name="profilePropertyValue">The profile property value.</param>
        private static void SetLanguage(ClientContext ctx, SPRemoteEventResult result, string profilePropertyValue)
        {
            if (!string.IsNullOrEmpty(profilePropertyValue))
            {
                string[] profilePropertyValues = profilePropertyValue.Split('|');
                if (profilePropertyValues.Length > 0)
                {
                    profilePropertyValue = profilePropertyValues[0];
                }

                var formatTaxonomy = AutoTaggingHelper.GetTaxonomyFormat(ctx, profilePropertyValue);
                result.ChangedItemProperties.Add(Constants.JciLanguageColumnName, formatTaxonomy);
                LogHelper.LogInformation(string.Format("Add taxonomy term value {0} to list on changedItemProperties of column {1} to get taxonomy format.", formatTaxonomy, Constants.JciLanguageColumnName), LogEventID.InformationWrite);
            }
        }

        /// <summary>
        /// Sets the data classification level.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="result">The item to update.</param>
        /// <param name="profilePropertyValue">The profile property value.</param>
        private static void SetDataClassificationLevel(ClientContext ctx, SPRemoteEventResult result, string profilePropertyValue)
        {
            if (!string.IsNullOrEmpty(profilePropertyValue))
            {
                string[] profilePropertyValues = profilePropertyValue.Split('|');
                if (profilePropertyValues.Length > 0)
                {
                    profilePropertyValue = profilePropertyValues[0];
                }

                var formatTaxonomy = AutoTaggingHelper.GetTaxonomyFormat(ctx, profilePropertyValue);
                result.ChangedItemProperties.Add(Constants.DataClassificationLevelColumnName, formatTaxonomy);
                LogHelper.LogInformation(string.Format("Add taxonomy term value {0} to list on changedItemProperties of column {1} to get taxonomy format.", formatTaxonomy, Constants.DataClassificationLevel), LogEventID.InformationWrite);
            }
        }
    }
}