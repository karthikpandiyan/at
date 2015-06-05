// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldsFile.cs" company="Microsoft">
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
    /// Fields File
    /// </summary>
    public class FieldsFile
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
        /// Returns a collection of Fields to provision
        /// </summary>
        /// <returns>Fields list</returns>
        public List<Field> GetFields()
        { 
            List<Field> listFields = new List<Field>();
           
            if (!string.IsNullOrEmpty(this.FileName))
            {
                var folderPath = PathHelper.GetAssemblyDirectory();
                var fileName = this.FileName;
                string filePath = string.Format("{0}{1}{2}", folderPath, @"\", fileName);

                XDocument doc = XDocument.Load(filePath);

                XElement fields = doc.Element("Fields");
                if (fields != null)
                {
                    foreach (var fieldElement in fields.Elements())
                    {
                        Field field = new Field { SchemaXml = fieldElement.ToString() };
                        fields.Add(field);
                    }
                }
            }

            return listFields;
        }
    }
}
