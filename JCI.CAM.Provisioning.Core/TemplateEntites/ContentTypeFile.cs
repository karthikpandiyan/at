// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentTypeFile.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   SharePointDataController retrieves sharepoint document metadata
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core.TemplateEntites
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// ContentTypeFile xml schema
    /// </summary>
    public class ContentTypeFile
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [XmlAttribute]
        public string FileName { get; set; }

        /// <summary>
        /// Gets the content type declaration.
        /// </summary>
        /// <returns>Content types list</returns>
        public List<ContentType> GetContentTypeDeclaration()
        {
            List<ContentType> contentTypes = new List<ContentType>();

            if (!string.IsNullOrEmpty(this.FileName))
            {
                var directoryPath = PathHelper.GetAssemblyDirectory();
                var fileName = this.FileName;
                string filePath = string.Format("{0}{1}{2}", directoryPath, @"\", fileName);

                XDocument doc = XDocument.Load(filePath);
                XElement fields = doc.Element("ContentTypes");
                if (fields != null)
                {
                    foreach (var element in fields.Elements())
                    {
                        ContentType field = new ContentType { SchemaXml = element.ToString() };
                        contentTypes.Add(field);
                    }
                }
            }

            return contentTypes;
        }
    }
}
