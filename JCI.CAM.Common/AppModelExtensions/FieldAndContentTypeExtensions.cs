//-----------------------------------------------------------------------
// <copyright file= "FieldAndContentTypeExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using JCI.CAM.Common.Logging;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This class provides extension methods that will help you work with fields and content types.
    /// </summary>
    public static partial class FieldAndContentTypeExtensions
    {
        /// <summary>
        /// Create a content type based on the classic feature framework structure.
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="absolutePathToFile">Absolute path to the xml location</param>
        public static void CreateContentTypeFromXMLFile(this Web web, string absolutePathToFile)
        {
            LogHelper.LogInformation("Loading content types definition file ...", LogEventID.InformationWrite);
            XmlDocument xd = new XmlDocument();
            xd.Load(absolutePathToFile);
            CreateContentTypeFromXML(web, xd);
        }

        /// <summary>
        /// Create a content type based on the classic feature framework structure.
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="xmlDoc">Actual XML document</param>
        public static void CreateContentTypeFromXML(this Web web, XmlDocument xmlDoc)
        {
            LogHelper.LogInformation("Provisioning all content types by reading definition file ...", LogEventID.InformationWrite);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("namespace", "http://schemas.microsoft.com/sharepoint/");

            XmlNodeList contentTypes = xmlDoc.SelectNodes("//namespace:ContentType", nsmgr);
            foreach (XmlNode ct in contentTypes)
            {
                string ctid = ct.Attributes["ID"].Value;
                string name = ct.Attributes["Name"].Value;

                var description = string.Empty;
                if (((XmlElement)ct).HasAttribute("Description"))
                {
                    description = ((XmlElement)ct).GetAttribute("Description");
                }

                var group = string.Empty;
                if (((XmlElement)ct).HasAttribute("Group"))
                {
                    group = ((XmlElement)ct).GetAttribute("Group");
                }

                web.CreateContentTypeForCTHub(name, description, ctid, group);

                XmlNodeList fieldRefs = ct.SelectNodes(".//namespace:FieldRef", nsmgr);
                XmlAttribute attr = null;
                foreach (XmlNode fieldRef in fieldRefs)
                {
                    bool required = false;
                    bool hidden = false;
                    string fieldId = fieldRef.Attributes["ID"].Value;
                    string fieldName = fieldRef.Attributes["Name"].Value;
                    attr = fieldRef.Attributes["Required"];
                    if (attr != null)
                    {
                        required = attr.Value.ToBoolean();
                    }

                    attr = fieldRef.Attributes["Hidden"];
                    if (attr != null)
                    {
                        hidden = attr.Value.ToBoolean();
                    }

                    web.AddFieldToContentTypeById(ctid, fieldName, required, hidden);
                }
            }

            LogHelper.LogInformation("Content Types created successfully.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Creates the content type for ct hub.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="contentTypeName">Name of the content type.</param>
        /// <param name="description">The description.</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <param name="group">The group.</param>
        /// <param name="parentContentType">Type of the parent content.</param>
        /// <returns>Content Type reference</returns>
        /// <exception cref="System.ArgumentNullException">
        /// name
        /// or
        /// contentTypeId
        /// </exception>
        public static ContentType CreateContentTypeForCTHub(this Web web, string contentTypeName, string description, string contentTypeId, string group = "Custom", ContentType parentContentType = null)
        {
            LogHelper.LogInformation(string.Format("Creating {0} ContentType.", contentTypeName), LogEventID.InformationWrite);

            if (string.IsNullOrEmpty(contentTypeName))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentNullException("contentTypeId");
            }

            if (web.ContentTypeExistsByName(contentTypeName))
            {
                return web.GetContentTypeById(contentTypeId);
            }

            // Load the current collection of content types
            ContentTypeCollection contentTypes = web.ContentTypes;
            web.Context.Load(contentTypes);
            web.Context.ExecuteQuery();
            ContentTypeCreationInformation newContentType = new ContentTypeCreationInformation();

            // Set the properties for the content type
            newContentType.Name = contentTypeName;
            newContentType.Id = contentTypeId;
            newContentType.Description = description;
            newContentType.Group = group;
            newContentType.ParentContentType = parentContentType;
            ContentType siteContentType = contentTypes.Add(newContentType);
            web.Context.Load(siteContentType);
            web.Context.ExecuteQuery();

            LogHelper.LogInformation(string.Format("{0} ContentType created successfully.", contentTypeName), LogEventID.InformationWrite);

            ////Return the content type object
            return siteContentType;
        }

        /// <summary>
        /// Create new content type to web
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="contentTypeName">Name of the content type</param>
        /// <param name="description">Description for the content type</param>
        /// <param name="contentTypeId">Complete ID for the content type</param>
        /// <param name="group">Group for the content type</param>
        /// <param name="parentContentType">Parent Content Type</param>
        /// <returns>
        /// The created content type
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// name
        /// or
        /// contentTypeId
        /// </exception>
        public static ContentType CreateContentType(this Web web, string contentTypeName, string description, string contentTypeId, string group = "Custom", ContentType parentContentType = null)
        {
            LogHelper.LogInformation(string.Format("Creating {0} ContentType.", contentTypeName), LogEventID.InformationWrite);

            if (string.IsNullOrEmpty(contentTypeName))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentNullException("contentTypeId");
            }

            if (web.ContentTypeExistsById(contentTypeId))
            {
                return web.GetContentTypeById(contentTypeId);
            }

            // Load the current collection of content types
            ContentTypeCollection contentTypes = web.ContentTypes;
            web.Context.Load(contentTypes);
            web.Context.ExecuteQuery();
            ContentTypeCreationInformation newContentType = new ContentTypeCreationInformation();

            // Set the properties for the content type
            newContentType.Name = contentTypeName;
            newContentType.Id = contentTypeId;
            newContentType.Description = description;
            newContentType.Group = group;
            newContentType.ParentContentType = parentContentType;
            ContentType siteContentType = contentTypes.Add(newContentType);
            web.Context.Load(siteContentType);
            web.Context.ExecuteQuery();

            LogHelper.LogInformation(string.Format("{0} ContentType created successfully.", contentTypeName), LogEventID.InformationWrite);

            ////Return the content type object
            return siteContentType;
        }

        /// <summary>
        /// Associates field to content type
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="contentTypeID">The content type identifier.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        public static void AddFieldToContentTypeById(this Web web, string contentTypeID, string fieldName, bool required = false, bool hidden = false)
        {
            LogHelper.LogInformation("Getting field reference to add it to the content type", LogEventID.InformationWrite);

            // Get content type
            ContentType ct = web.GetContentTypeById(contentTypeID);
            web.Context.Load(ct);
            web.Context.Load(ct.FieldLinks);
            web.Context.ExecuteQuery();

            // Get field
            Field fld = web.Fields.GetByInternalNameOrTitle(fieldName);

            // Add field association to content type
            AddFieldToContentType(web, ct, fld, required, hidden);
        }

        /// <summary>
        /// Associates field to content type
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="contentTypeID">The content type identifier.</param>
        /// <param name="fieldID">String representation of the field ID</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        public static void AddListFieldToContentTypeById(this List list, string contentTypeID, string fieldID, bool required = false, bool hidden = false)
        {
            LogHelper.LogInformation("Adding List Field to ContentType", LogEventID.InformationWrite);

            // Get content type
            ContentType ct = list.GetContentTypeById(contentTypeID);
            list.Context.Load(ct);
            list.Context.Load(ct.FieldLinks);
            list.Context.ExecuteQuery();

            foreach (FieldLink item in ct.FieldLinks)
            {
                if (item.Id.Equals(new Guid(fieldID)))
                {
                    return;
                }
            }

            Field fld = list.Fields.GetById(new Guid(fieldID));

            AddListFieldToContentType(list, ct, fld, required, hidden);
            LogHelper.LogInformation("List field added to ContentType successfully", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Return content type by Id
        /// </summary>
        /// <param name="web">Web to be processed</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <returns>
        /// Content type object or null if was not found
        /// </returns>
        /// <exception cref="System.ArgumentNullException">throws exception if contentType Id is null</exception>
        public static ContentType GetContentTypeById(this Web web, string contentTypeId)
        {
            LogHelper.LogInformation("Finding site contenttype", LogEventID.InformationWrite);

            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentNullException("contentTypeId");
            }

            ContentTypeCollection contentTypeCollection = web.ContentTypes;
            web.Context.Load(contentTypeCollection);
            web.Context.ExecuteQuery();
            foreach (var item in contentTypeCollection)
            {
                if (item.Id.StringValue.Equals(contentTypeId, StringComparison.OrdinalIgnoreCase))
                {
                    LogHelper.LogInformation(string.Format("Site contenttype {0} is available", item.Name), LogEventID.InformationWrite);
                    return item;
                }
            }

            LogHelper.LogInformation("Site contenttype is not available", LogEventID.InformationWrite);
            return null;
        }

        /// <summary>
        /// Return content type by Id
        /// </summary>
        /// <param name="list">Web to be processed</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <returns>
        /// Content type object or null if was not found
        /// </returns>
        /// <exception cref="System.ArgumentNullException">throws exception if contentType Id is null</exception>
        public static ContentType GetContentTypeById(this List list, string contentTypeId)
        {
            LogHelper.LogInformation("Finding list contenttype.", LogEventID.InformationWrite);

            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentNullException("contentTypeId");
            }

            ContentTypeCollection contentTypeCollection = list.ContentTypes;
            list.Context.Load(contentTypeCollection);
            list.Context.ExecuteQuery();
            foreach (var item in contentTypeCollection)
            {
                if (item.Id.StringValue.StartsWith(contentTypeId, StringComparison.OrdinalIgnoreCase))
                {
                    LogHelper.LogInformation(string.Format("List contenttype {0} is available", item.Name), LogEventID.InformationWrite);
                    return item;
                }
            }

            LogHelper.LogInformation("List contenttype is not available", LogEventID.InformationWrite);
            return null;
        }

        /// <summary>
        /// Associates field to content type
        /// </summary>
        /// <param name="list">Site to be processed - can be root web or sub site</param>
        /// <param name="contentType">Content type to associate field to</param>
        /// <param name="field">Field to associate to the content type</param>
        /// <param name="required">Optionally make this a required field</param>
        /// <param name="hidden">Optionally make this a hidden field</param>
        public static void AddListFieldToContentType(this List list, ContentType contentType, Field field, bool required = false, bool hidden = false)
        {
            LogHelper.LogInformation("Adding List Field to List ContentType", LogEventID.InformationWrite);

            if (!contentType.IsPropertyAvailable("Id"))
            {
                list.Context.Load(contentType, ct => ct.Id);
                list.Context.ExecuteQuery();
            }

            if (!field.IsPropertyAvailable("Id"))
            {
                list.Context.Load(field, f => f.Id);
                list.Context.ExecuteQuery();
            }

            FieldLinkCreationInformation fldInfo = new FieldLinkCreationInformation();
            fldInfo.Field = field;
            contentType.FieldLinks.Add(fldInfo);
            contentType.Update(false);
            list.Context.ExecuteQuery();

            list.Context.Load(field);
            list.Context.ExecuteQuery();

            if (required || hidden)
            {
                ////Update FieldLink
                FieldLink flink = contentType.FieldLinks.GetById(field.Id);
                flink.Required = required;
                flink.Hidden = hidden;
                contentType.Update(false);
                list.Context.ExecuteQuery();
            }

            LogHelper.LogInformation("Added List Field to List ContentType", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Associates field to content type
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="contentType">Content type to associate field to</param>
        /// <param name="field">Field to associate to the content type</param>
        /// <param name="required">Optionally make this a required field</param>
        /// <param name="hidden">Optionally make this a hidden field</param>
        public static void AddFieldToContentType(this Web web, ContentType contentType, Field field, bool required = false, bool hidden = false)
        {
            LogHelper.LogInformation("Adding site column to site ContentType", LogEventID.InformationWrite);

            if (!contentType.IsPropertyAvailable("Id"))
            {
                web.Context.Load(contentType, ct => ct.Id);
                web.Context.ExecuteQuery();
            }

            if (!field.IsPropertyAvailable("Id"))
            {
                web.Context.Load(field, f => f.Id);
                web.Context.ExecuteQuery();
            }

            FieldLink linkRef = contentType.FieldLinks.GetById(field.Id);
            web.Context.Load(linkRef, f => f.Id);
            web.Context.ExecuteQuery();
            if (linkRef != null && linkRef.IsPropertyAvailable("Id"))
            {
                return;
            }

            try
            {
                FieldLinkCreationInformation fldInfo = new FieldLinkCreationInformation();
                fldInfo.Field = field;
                contentType.FieldLinks.Add(fldInfo);
                contentType.Update(true);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("is read only"))
                {
                    throw;
                }

                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
            }

            web.Context.Load(field);
            web.Context.ExecuteQuery();

            if (required || hidden)
            {
                try
                {
                    ////Update FieldLink
                    FieldLink flink = contentType.FieldLinks.GetById(field.Id);
                    flink.Required = required;
                    flink.Hidden = hidden;
                    contentType.Update(true);
                    web.Context.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("is read only"))
                    {
                        throw;
                    }

                    LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                }
            }

            LogHelper.LogInformation("Added site column to Site ContentType", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Does content type exists in the web
        /// </summary>
        /// <param name="web">Web to be processed</param>
        /// <param name="contentTypeName">Name of the content type</param>
        /// <returns>
        /// True if the content type exists, false otherwise
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Content Type Name</exception>
        public static bool ContentTypeExistsByName(this Web web, string contentTypeName)
        {
            LogHelper.LogInformation("Checking site content type is available or not.", LogEventID.InformationWrite);

            if (string.IsNullOrEmpty(contentTypeName))
            {
                throw new ArgumentNullException("contentTypeName");
            }

            ContentTypeCollection contentTypeCollection = web.ContentTypes;
            IEnumerable<ContentType> results = web.Context.LoadQuery<ContentType>(contentTypeCollection.Where(item => item.Name == contentTypeName));
            web.Context.ExecuteQuery();
            ContentType ct = results.FirstOrDefault();
            if (ct != null)
            {
                LogHelper.LogInformation(string.Format("Site Content Type {0} is available", contentTypeName), LogEventID.ContentTypeAlreadyExists);
                return true;
            }

            LogHelper.LogInformation("Checked site content type is not available.", LogEventID.InformationWrite);

            return false;
        }

        /// <summary>
        /// Creates fields from feature element xml file schema. XML file can contain one or many field definitions created using classic feature framework structure.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlFilePath">Absolute path to the xml location</param>
        public static void CreateFieldsFromXMLFile(this Web web, string xmlFilePath)
        {
            LogHelper.LogInformation("Loading field definition file ...", LogEventID.InformationWrite);

            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);

            // Perform the action field creation
            CreateFieldsFromXML(web, xd);
        }

        /// <summary>
        /// Creates field from xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlDoc">Actual XML document</param>
        public static void CreateFieldsFromXML(this Web web, XmlDocument xmlDoc)
        {
            LogHelper.LogInformation("Reading field definition file.", LogEventID.InformationWrite);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("namespace", "http://schemas.microsoft.com/sharepoint/");

            XmlNodeList fields = xmlDoc.SelectNodes("//namespace:Field", nsmgr);
            int count = fields.Count;
            foreach (XmlNode field in fields)
            {
                string id = field.Attributes["ID"].Value;
                string name = field.Attributes["Name"].Value;

                // IF field already existed, let's move on
                if (web.FieldExistsByName(name))
                {
                    LogHelper.LogInformation(string.Format("Site column {0} already exist.", name), LogEventID.InformationWrite);
                }
                else
                {
                    string xml = field.OuterXml.Replace("xmlns=\"http://schemas.microsoft.com/sharepoint/\"", string.Empty);
                    web.CreateField(xml);
                }
            }
        }

        /// <summary>
        /// Create field to web remotely
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="fieldAsXml">The XML declaration of SiteColumn definition</param>
        /// <param name="executeQuery">Execute query</param>
        /// <returns>
        /// The newly created field or existing field.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">field xml</exception>
        public static Field CreateField(this Web web, string fieldAsXml, bool executeQuery = true)
        {
            LogHelper.LogInformation("Creating new site column", LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(fieldAsXml))
            {
                throw new ArgumentNullException("fieldAsXml");
            }

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(fieldAsXml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xd.NameTable);
            nsmgr.AddNamespace("namespace", "http://schemas.microsoft.com/sharepoint/");
            XmlNode fieldNode = xd.SelectSingleNode("//Field", nsmgr);
            string id = fieldNode.Attributes["ID"].Value;
            string name = fieldNode.Attributes["Name"].Value;

            FieldCollection fields = web.Fields;
            web.Context.Load(fields);
            web.Context.ExecuteQuery();

            Field field = fields.AddFieldAsXml(fieldAsXml, false, AddFieldOptions.AddFieldInternalNameHint);
            web.Update();

            if (executeQuery)
            {
                web.Context.ExecuteQuery();
            }

            LogHelper.LogInformation(string.Format("Created site column {0} successfully.", name), LogEventID.InformationWrite);

            return field;
        }

        /// <summary>
        /// Adds a field to a list
        /// </summary>
        /// <param name="list">List to process</param>
        /// <param name="fieldAsXml">The XML declaration of SiteColumn definition</param>
        /// <returns>
        /// The newly created field or existing field.
        /// </returns>
        public static Field CreateField(this List list, string fieldAsXml)
        {
            LogHelper.LogInformation("Creating List column from fields xml.", LogEventID.InformationWrite);
            FieldCollection fields = list.Fields;
            list.Context.Load(fields);
            list.Context.ExecuteQuery();

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(fieldAsXml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xd.NameTable);
            nsmgr.AddNamespace("namespace", "http://schemas.microsoft.com/sharepoint/");
            XmlNode fieldNode = xd.SelectSingleNode("//Field", nsmgr);
            string id = fieldNode.Attributes["ID"].Value;
            string name = fieldNode.Attributes["Name"].Value;

            if (list.FieldExistsById(new Guid(id)))
            {
                Field field = list.Fields.GetById(new Guid(id));
                list.Context.Load(field);
                list.Context.ExecuteQuery();

                LogHelper.LogInformation(string.Format("List column {0} existed.", name), LogEventID.InformationWrite);
                return field;
            }
            else
            {
                Field field = fields.AddFieldAsXml(fieldAsXml, false, AddFieldOptions.AddFieldInternalNameHint);
                list.Update();
                list.Context.ExecuteQuery();

                LogHelper.LogInformation(string.Format("Created list column {0}.", name), LogEventID.InformationWrite);
                return field;
            }
        }

        /// <summary>
        /// Returns if the field is found
        /// </summary>
        /// <param name="list">List to process</param>
        /// <param name="fieldId">The field ID</param>
        /// <returns>
        /// True if the fields exists, false otherwise
        /// </returns>
        public static bool FieldExistsById(this List list, Guid fieldId)
        {
            LogHelper.LogInformation("Cheking list column available or not.", LogEventID.InformationWrite);
            FieldCollection fields = list.Fields;
            list.Context.Load(fields);
            list.Context.ExecuteQuery();
            foreach (var item in fields)
            {
                if (item.Id == fieldId)
                {
                    LogHelper.LogInformation(string.Format("List column {0} available.", item.Title), LogEventID.InformationWrite);
                    return true;
                }
            }

            LogHelper.LogInformation("List column not available.", LogEventID.InformationWrite);
            return false;
        }

        /// <summary>
        /// Returns if the field is found
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="fieldName">String for the field internal name to be used as query criteria</param>
        /// <returns>
        /// True or false depending on the field existence
        /// </returns>
        /// <exception cref="System.ArgumentNullException">field name</exception>
        public static bool FieldExistsByName(this List list, string fieldName)
        {
            LogHelper.LogInformation(string.Format("Checking list column {0} exist or not.", fieldName), LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            FieldCollection fields = list.Fields;
            IEnumerable<Field> results = list.Context.LoadQuery<Field>(fields.Where(item => item.InternalName == fieldName || item.Title == fieldName));
            list.Context.ExecuteQuery();
            if (results.FirstOrDefault() != null)
            {
                LogHelper.LogInformation(string.Format("Found list column {0}.", fieldName), LogEventID.InformationWrite);
                return true;
            }

            LogHelper.LogInformation(string.Format("List column {0} not available.", fieldName), LogEventID.InformationWrite);
            return false;
        }

        /// <summary>
        /// Returns if the field is found
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="fieldName">String for the field internal name to be used as query criteria</param>
        /// <returns>
        /// True or false depending on the field existence
        /// </returns>
        /// <exception cref="System.ArgumentNullException">field name</exception>
        public static bool FieldExistsByName(this Web web, string fieldName)
        {
            LogHelper.LogInformation(string.Format("Checking site column {0} exist or not.", fieldName), LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            FieldCollection fields = web.Fields;
            IEnumerable<Field> results = web.Context.LoadQuery<Field>(fields.Where(item => item.InternalName == fieldName));
            web.Context.ExecuteQuery();
            if (results.FirstOrDefault() != null)
            {
                LogHelper.LogInformation(string.Format("Found site column {0}.", fieldName), LogEventID.InformationWrite);
                return true;
            }

            LogHelper.LogInformation(string.Format("Site column {0} not available.", fieldName), LogEventID.InformationWrite);
            return false;
        }

        /// <summary>
        /// Gets the name of the field by.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Return Field</returns>
        /// <exception cref="System.ArgumentNullException">field name</exception>
        public static Field GetFieldByName(this Web web, string fieldName)
        {
            LogHelper.LogInformation(string.Format("Checking site column {0} exist or not.", fieldName), LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            Field field = web.Fields.GetByInternalNameOrTitle(fieldName);
            web.Context.Load(field);
            web.Context.ExecuteQuery();
            LogHelper.LogInformation(string.Format("Site column {0} found.", fieldName), LogEventID.InformationWrite);
            return field;
        }

        /// <summary>
        /// Adds the field to list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="field">The field.</param>
        public static void AddFieldToList(this List list, Field field)
        {
            LogHelper.LogInformation(string.Format("Adding field {0} to list.", field.Title), LogEventID.InformationWrite);
            list.Fields.Add(field);
            list.Update();
            list.Context.ExecuteQuery();
            LogHelper.LogInformation(string.Format("Added field {0} to list.", field.Title), LogEventID.InformationWrite);
        }

        /// <summary>
        /// Sets the index of the list field.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="isIndex">if set to <c>true</c> [is index].</param>
        /// <exception cref="System.ArgumentNullException">field name</exception>
        public static void SetListFieldIndex(this List list, string fieldName, bool isIndex = false)
        {
            LogHelper.LogInformation(string.Format("Setting index on list field {0}.", fieldName), LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            Field field = list.Fields.GetByInternalNameOrTitle(fieldName);
            field.Indexed = isIndex;
            field.Update();
            list.Update();
            list.Context.ExecuteQuery();
            LogHelper.LogInformation(string.Format("List field {0} indexed.", fieldName), LogEventID.InformationWrite);
        }

        /// <summary>
        /// Creates field from xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlFilePath">Absolute path to the xml location</param>
        /// <param name="documentTemplatesPath">Absolute path to the document templates location</param>
        public static void CreateDocumentContentTypeFromXML(this Web web, string xmlFilePath, string documentTemplatesPath)
        {
            LogHelper.LogInformation("Loading document template association definition file ...", LogEventID.InformationWrite);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            XmlNodeList documentTemplates = xmlDoc.SelectNodes("//DocumentTemplate");
            foreach (XmlNode documentTemplate in documentTemplates)
            {
                string contentTypeId = documentTemplate.Attributes["ContentTypeId"].Value;
                string path = documentTemplate.Attributes["Path"].Value;
                string documentTemlateFilepath = documentTemplatesPath + "\\" + path;
                SetDocumentAsTemplate(web, contentTypeId, documentTemlateFilepath);
            }

            LogHelper.LogInformation("Document Template association to respective content types completed successfully.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Set Document Template to Content Type
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="contentTypeId">String for the ContentType Id to be used as query criteria.</param>
        /// <param name="documentTemplatePath">String for the Document template file to set to content type.</param>
        public static void SetDocumentAsTemplate(this Web web, string contentTypeId, string documentTemplatePath)
        {
            var templateName = documentTemplatePath.Substring(documentTemplatePath.LastIndexOf("\\") + 1);
            LogHelper.LogInformation(string.Format("Associating {0} document template to contentype.", templateName), LogEventID.InformationWrite);
            ContentType contentType = web.ContentTypes.GetById(contentTypeId);
            web.Context.Load(contentType);
            web.Context.ExecuteQuery();

            // Get instance to the _cts folder created for the each of the content types
            string folderServerRelativeURL = "_cts/" + contentType.Name;
            Folder contentFolder = web.GetFolderByServerRelativeUrl(folderServerRelativeURL);
            web.Context.Load(contentFolder);
            web.Context.ExecuteQuery();

            // Load the local template document
            string path = documentTemplatePath;
            string fileName = System.IO.Path.GetFileName(path);
            byte[] filecontent = System.IO.File.ReadAllBytes(path);

            // Uplaod file to the Office365
            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Overwrite = true;
                newFile.Content = filecontent;
                newFile.Url = folderServerRelativeURL + "/" + fileName;

                Microsoft.SharePoint.Client.File uploadedFile = contentFolder.Files.Add(newFile);
                web.Context.Load(uploadedFile);
                web.Context.ExecuteQuery();
            }

            contentType.DocumentTemplate = fileName;
            contentType.Update(true);
            web.Context.ExecuteQuery();
            LogHelper.LogInformation(string.Format("Added {0} document template to contentype.", templateName), LogEventID.InformationWrite);
        }

        /// <summary>
        /// Binds a field to a term set based on an xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="absolutePathToFile">Absolute path to the xml location</param>
        public static void BindFieldsToTermSetsFromXMLFile(this Web web, string absolutePathToFile)
        {
            LogHelper.LogInformation("Loading Taxonomy fields mapping definition file ...", LogEventID.InformationWrite);
            XmlDocument xd = new XmlDocument();
            xd.Load(absolutePathToFile);
            BindFieldsToTermSetsFromXML(web, xd);
        }

        /// <summary>
        /// Binds a field to a term set based on an xml structure which follows the classic feature framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlDoc">Actual XML document</param>
        public static void BindFieldsToTermSetsFromXML(this Web web, XmlDocument xmlDoc)
        {
            LogHelper.LogInformation("Updating fields and its term set binding...", LogEventID.InformationWrite);
            XmlNodeList fields = xmlDoc.SelectNodes("//MMSField");
            foreach (XmlNode mmsfield in fields)
            {
                string fieldGuid = mmsfield.Attributes["FieldGuid"].Value;
                string groupName = mmsfield.Attributes["MMSGroupName"].Value;
                string termSet = mmsfield.Attributes["TermSet"].Value;

                TaxonomyExtensions.WireUpTaxonomyField(web, new Guid(fieldGuid), groupName, termSet);
            }

            LogHelper.LogInformation("Updated fields and  its term set mapping.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Adds the site content type to list by content type Id.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="rootWeb">The root web.</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <param name="defaultContent">if set to <c>true</c> [default content].</param>
        public static void AddSiteContentTypeToListById(this List list, Web rootWeb, string contentTypeId, bool defaultContent = false)
        {
            ContentType contentType = GetContentTypeById(rootWeb, contentTypeId);
            if (contentType != null)
            {
                AddContentTypeToList(list, contentType, defaultContent);
            }
        }

        /// <summary>
        /// Add content type to list
        /// </summary>
        /// <param name="list">List to add content type to</param>
        /// <param name="contentTypeID">Complete ID for the content type</param>
        /// <param name="defaultContent">If set true, content type is updated to be default content type for the list</param>
        public static void AddContentTypeToListById(this List list, string contentTypeID, bool defaultContent = false)
        {
            Web web = list.ParentWeb;
            ContentType contentType = GetContentTypeById(web, contentTypeID);
            if (contentType != null)
            {
                AddContentTypeToList(list, contentType, defaultContent);
            }
        }

        /// <summary>
        /// Add content type to list
        /// </summary>
        /// <param name="list">List to add content type to</param>
        /// <param name="contentType">Content type to add to the list</param>
        /// <param name="defaultContent">If set true, content type is updated to be default content type for the list</param>
        /// <returns>Return content type</returns>
        /// <exception cref="System.ArgumentNullException">content type</exception>
        public static ContentType AddContentTypeToList(this List list, ContentType contentType, bool defaultContent = false)
        {
            LogHelper.LogInformation("Adding content type to list.", LogEventID.InformationWrite);
            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            if (list.ContentTypeExistsById(contentType.Id.StringValue))
            {
                return GetContentTypeById(list, contentType.Id.StringValue);
            }

            list.ContentTypesEnabled = true;
            list.Update();
            list.Context.ExecuteQuery();

            ContentType listContentType = list.ContentTypes.AddExistingContentType(contentType);
            list.Context.Load(listContentType);
            list.Context.ExecuteQuery();
            if (defaultContent)
            {
                SetDefaultContentTypeToList(list, contentType);
            }

            LogHelper.LogInformation("Added content type to list.", LogEventID.InformationWrite);
            return listContentType;
        }

        /// <summary>
        /// Does content type exist in list
        /// </summary>
        /// <param name="list">List to update</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <returns>
        /// True if the content type exists, false otherwise
        /// </returns>
        /// <exception cref="System.ArgumentNullException">content Type Id</exception>
        public static bool ContentTypeExistsById(this List list, string contentTypeId)
        {
            LogHelper.LogInformation("Checking list content type is available or not.", LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentNullException("contentTypeId");
            }

            if (!list.ContentTypesEnabled)
            {
                return false;
            }

            ContentTypeCollection contentTypeCollection = list.ContentTypes;
            list.Context.Load(contentTypeCollection);
            list.Context.ExecuteQuery();

            foreach (var item in contentTypeCollection)
            {
                if (item.Id.StringValue.StartsWith(contentTypeId, StringComparison.OrdinalIgnoreCase))
                {
                    LogHelper.LogInformation("Checked list content type is available.", LogEventID.InformationWrite);
                    return true;
                }
            }

            LogHelper.LogInformation("Checked list content type is not available.", LogEventID.InformationWrite);
            return false;
        }

        /// <summary>
        /// Does content type exists in the web
        /// </summary>
        /// <param name="web">Web to be processed</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        /// <returns>
        /// True if the content type exists, false otherwise
        /// </returns>
        /// <exception cref="System.ArgumentNullException">content Type Id</exception>
        public static bool ContentTypeExistsById(this Web web, string contentTypeId)
        {
            LogHelper.LogInformation("Checking site content type is available or not.", LogEventID.InformationWrite);
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentNullException("contentTypeId");
            }

            ContentTypeCollection contentTypeCollection = web.ContentTypes;
            web.Context.Load(contentTypeCollection);
            web.Context.ExecuteQuery();
            foreach (var item in contentTypeCollection)
            {
                if (item.Id.StringValue.StartsWith(contentTypeId, StringComparison.OrdinalIgnoreCase))
                {
                    LogHelper.LogInformation("Checked site content type is available.", LogEventID.InformationWrite);
                    return true;
                }
            }

            LogHelper.LogInformation("Checked site content type is not available.", LogEventID.InformationWrite);
            return false;
        }

        /// <summary>
        /// Sets the default content type to list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="contentTypeId">The content type identifier.</param>
        public static void SetDefaultContentTypeToList(this List list, string contentTypeId)
        {
            LogHelper.LogInformation("Adding default contenttype to list.", LogEventID.InformationWrite);
            ContentTypeCollection contentTypeCollection = list.ContentTypes;
            list.Context.Load(contentTypeCollection);
            list.Context.ExecuteQuery();

            var contentTypeIds = new List<ContentTypeId>();
            foreach (ContentType ct in contentTypeCollection)
            {
                contentTypeIds.Add(ct.Id);
            }

            var newOrder = contentTypeIds.Except(contentTypeIds.Where(id => id.StringValue.StartsWith("0x012000")))
                                 .OrderBy(x => !x.StringValue.StartsWith(contentTypeId, StringComparison.OrdinalIgnoreCase))
                                 .ToArray();
            list.RootFolder.UniqueContentTypeOrder = newOrder;

            list.RootFolder.Update();
            list.Update();
            list.Context.ExecuteQuery();
            LogHelper.LogInformation("Added default contenttype to list.", LogEventID.InformationWrite);
        }

        /// <summary>
        /// Set default content type to list
        /// </summary>
        /// <param name="list">List to update</param>
        /// <param name="contentType">Content type to make default</param>
        public static void SetDefaultContentTypeToList(this List list, ContentType contentType)
        {
            SetDefaultContentTypeToList(list, contentType.Id.ToString());
        }
    }
}
