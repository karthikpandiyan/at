//-----------------------------------------------------------------------
// <copyright file= "TaxonomyExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common.AppModelExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;

    /// <summary>
    /// Extension methods for Taxonomy related
    /// </summary>
    public static class TaxonomyExtensions
    {
        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Field ID to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">If true, create a multi value field</param>
        public static void WireUpTaxonomyField(this Web web, Guid id, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            var field = web.Fields.GetById(id);
            web.Context.Load(field);
            web.WireUpTaxonomyField(field, mmsGroupName, mmsTermSetName, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <param name="multiValue">If true, create a multi value field</param>
        public static void WireUpTaxonomyField(this Web web, Field field, string mmsGroupName, string mmsTermSetName, bool multiValue = false)
        {
            TermStore termStore = GetDefaultTermStore(web);

            if (termStore == null)
            {
                throw new NullReferenceException("The default term store is not available.");
            }

            if (string.IsNullOrEmpty(mmsTermSetName))
            {
                throw new ArgumentNullException("mmsTermSetName", "The MMS term set is not specified.");
            }

            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            web.Context.Load(termStore);
            web.Context.Load(termSet);
            web.Context.ExecuteQuery();

            WireUpTaxonomyField(web, field, termSet, multiValue);
        }

       /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="termSet">Taxonomy TermSet</param>
        /// <param name="multiValue">If true, create a multi value field</param>
        public static void WireUpTaxonomyField(this Web web, Field field, TermSet termSet, bool multiValue = false)
        {
            WireUpTaxonomyFieldInternal(field, termSet, multiValue);
        }

        /// <summary>
        /// Wires up MMS field to the specified term set or term.
        /// </summary>
        /// <param name="field">Field to be wired up</param>
        /// <param name="taxonomyItem">Taxonomy TermSet or Term</param>
        /// <param name="multiValue">Allow multiple selection</param>
        /// <exception cref="System.ArgumentException">Bound TaxonomyItem must be either a TermSet or a Term</exception>
        private static void WireUpTaxonomyFieldInternal(Field field, TaxonomyItem taxonomyItem, bool multiValue)
        {
            var clientContext = field.Context as ClientContext;

            taxonomyItem.ValidateNotNullOrEmpty("taxonomyItem");

            var anchorTerm = taxonomyItem as Term;

            if (anchorTerm != default(Term) && !anchorTerm.IsPropertyAvailable("TermSet"))
            {
                clientContext.Load(anchorTerm.TermSet);
                clientContext.ExecuteQuery();
            }

            var termSet = taxonomyItem is Term ? anchorTerm.TermSet : taxonomyItem as TermSet;

            if (termSet == default(TermSet))
            {
                throw new ArgumentException("Bound TaxonomyItem must be either a TermSet or a Term");
            }

            if (!termSet.IsPropertyAvailable("TermStore"))
            {
                clientContext.Load(termSet.TermStore);
                clientContext.ExecuteQuery();
            }

            // set the SSP ID and Term Set ID on the taxonomy field
            var taxField = clientContext.CastTo<TaxonomyField>(field);
            taxField.SspId = termSet.TermStore.Id;
            taxField.TermSetId = termSet.Id;

            if (anchorTerm != default(Term))
            {
                taxField.AnchorId = anchorTerm.Id;
            }

            taxField.AllowMultipleValues = multiValue;
            taxField.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Private method used for resolving taxonomy term set for taxonomy field
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns>Term store</returns>
        private static TermStore GetDefaultTermStore(Web web)
        {
            TermStore termStore = null;
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(web.Context);
            web.Context.Load(
                taxonomySession,
                ts => ts.TermStores.Include(
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name)));
            web.Context.ExecuteQuery();
            if (taxonomySession != null)
            {
                termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            }

            return termStore;
        }
    }
}
