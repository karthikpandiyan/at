//-----------------------------------------------------------------------
// <copyright file="ListProvisionHelper.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.SPHelpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Xml;
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Models;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// List provision helper
    /// </summary>
    public static class ListProvisionHelper
    {
        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>
        /// Return new List
        /// </returns>
        public static List CreateList(Web web, Web rootWeb, ListDefinition definition)
        {
            int listTemplateType = GetListTemplateTypeByBaseType(definition.BaseType);
            List newList = null;
            newList = web.CreateList(listTemplateType, definition.Title, definition.Description, definition.VersioningEnabled.ToBoolean(), definition.EnableMinorVersions.ToBoolean(), definition.EnableContentTypes.ToBoolean());

            if (newList != null)
            {
                AddFieldsToList(definition.Fields, newList);

                AddViewsToList(definition.Views, newList);

                web.RemoveAllContentTypesFromList(newList);

                AddContentTypesToList(web, rootWeb, newList, definition.ContentTypes);

                AddListEventReceiver(newList, definition.Receivers);
            }

            return newList;
        }

        /// <summary>
        /// Adds the list event receiver.
        /// </summary>
        /// <param name="newList">The new list.</param>
        /// <param name="listReceivers">The list receivers.</param>
        private static void AddListEventReceiver(List newList, List<ListReceiver> listReceivers)
        {
            if (listReceivers == null || listReceivers.Count == 0)
            {
                return;
            }

            foreach (ListReceiver listReceiver in listReceivers)
            {
                newList.AddRemoteEventReceiver(listReceiver.Name, listReceiver.Url, (EventReceiverType)Enum.Parse(typeof(EventReceiverType), listReceiver.Type, true), (EventReceiverSynchronization)Enum.Parse(typeof(EventReceiverSynchronization), listReceiver.Synchronization, true), listReceiver.SequenceNumber, true);
            }
        }

        /// <summary>
        /// Adds the content types to list.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="contentTypes">The content types.</param>
        private static void AddContentTypesToList(Web web, Web rootWeb, List newList, ContentTypeDefinitions contentTypes)
        {
            if (contentTypes == null)
            {
                return;
            }

            //// Create Contenttype
            if (contentTypes.ContentTypeRef != null)
            {
                foreach (var contentTypeRef in contentTypes.ContentTypeRef)
                {
                    newList.AddSiteContentTypeToListById(rootWeb, contentTypeRef.ID);
                }
            }

            if (contentTypes.ContentType != null)
            {
                ContentTypeDefinition contentTypeDefinition = contentTypes.ContentType;

                ContentType webContentType = web.CreateContentType(contentTypeDefinition.Name, string.Empty, contentTypeDefinition.ID, contentTypeDefinition.Group);

                //// Add contenttype to List
                ContentType listContentType = newList.AddContentTypeToList(webContentType);

                //// Add fieldRefs to ContentType
                List<ContentTypeFieldRef> cntFields = contentTypeDefinition.FieldRefs;
                foreach (var contentTypeFieldRef in cntFields)
                {
                    newList.AddListFieldToContentTypeById(listContentType.Id.StringValue, contentTypeFieldRef.ID, false, false);
                }
            }
        }

        /// <summary>
        /// Adds the views to list.
        /// </summary>
        /// <param name="views">The views.</param>
        /// <param name="newList">The new list.</param>
        private static void AddViewsToList(List<ListView> views, List newList)
        {
            //// Create View to List
            foreach (var view in views)
            {
                if (!view.Hidden.ToBoolean() || !view.ReadOnly.ToBoolean() || !string.IsNullOrEmpty(view.DisplayName))
                {
                    newList.DeleteViewByName(view.DisplayName);

                    List<string> fieldArry = new List<string>();

                    foreach (var field in view.ViewFields)
                    {
                        fieldArry.Add(field.Name);
                    }

                    Microsoft.SharePoint.Client.ViewType type = (Microsoft.SharePoint.Client.ViewType)Enum.Parse(typeof(Microsoft.SharePoint.Client.ViewType), view.Type, true);
                    newList.CreateView(view.DisplayName, type, fieldArry.ToArray(), view.RowLimit, view.DefaultView.ToBoolean(), view.Query);
                }
            }
        }

        /// <summary>
        /// Adds the fields to list.
        /// </summary>
        /// <param name="listfields">The list fields.</param>
        /// <param name="newList">The new list.</param>
        private static void AddFieldsToList(ListFields listfields, List newList)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(CreateFieldsXML(listfields));
            XmlNodeList fields = xmlDoc.SelectNodes("//Field");
            foreach (XmlNode field in fields)
            {
                newList.CreateField(field.OuterXml);
            }
        }

        /// <summary>
        /// Gets the type of the list template type by base.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <returns>
        /// Return list template type
        /// </returns>
        private static int GetListTemplateTypeByBaseType(int baseType)
        {
            int listTemplateType;
            switch (baseType)
            {
                case 0:
                    listTemplateType = (int)ListTemplateType.GenericList;
                    break;
                case 1:
                    listTemplateType = (int)ListTemplateType.DocumentLibrary;
                    break;
                case 4:
                    listTemplateType = (int)ListTemplateType.Survey;
                    break;
                case 5:
                    listTemplateType = (int)ListTemplateType.IssueTracking;
                    break;
                default:
                    listTemplateType = (int)ListTemplateType.GenericList;
                    break;
            }

            return listTemplateType;
        }

        /// <summary>
        /// Creates the XML.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// Return xml string
        /// </returns>
        private static string CreateFieldsXML(ListFields entity)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(entity.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, entity);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return WebUtility.HtmlDecode(xmlDoc.InnerXml.Replace("<FieldBody>", string.Empty).Replace("</FieldBody>", string.Empty));
            }
        }
    }
}