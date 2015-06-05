//-----------------------------------------------------------------------
// <copyright file="TaxonomyHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.SPHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;

    /// <summary>
    /// Helper class to retrieve Term sets from Term store
    /// </summary>
    public class TaxonomyHelper
    {
        /// <summary>
        /// Get Terms from Term sets
        /// </summary>
        /// <param name="ctx">The SP client context</param>
        /// <param name="groupName">The group name in the term store</param>
        /// <param name="termSetName">The Term set name</param>
        /// <returns>Collection of terms</returns>
        public static Collection<string> GetManagedMetadataItems(ClientContext ctx, string groupName, string termSetName)
        {
            Collection<string> items = new Collection<string>();
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(ctx);

            // Get the term store by name
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();

            // Get the term group by Name
            TermGroup termGroup = termStore.Groups.GetByName(groupName);

            // Get the term set by Name
            TermSet termSet = termGroup.TermSets.GetByName(termSetName);

            // Get all the terms 
            TermCollection termColl = termSet.Terms;

            ctx.Load(termColl);

            // Execute the query to the server
            ctx.ExecuteQuery();
            if (termColl != null)
            {
                foreach (Term tempTerm in termColl)
                {
                    items.Add(tempTerm.Name);
                }
            }

            return items;
        }
    }
}
