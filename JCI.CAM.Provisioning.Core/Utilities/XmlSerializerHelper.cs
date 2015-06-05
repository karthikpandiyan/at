// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializerHelper.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  Helper class to Serialize and Deserialize objects to and from XML
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Helper class to Serialize and Deserialize objects to and from XML
    /// </summary>
    public static class XmlSerializerHelper
    {
        #region Private Instance Members
        /// <summary>
        /// XML formatter
        /// </summary>
        private static readonly Dictionary<Type, XmlSerializer> XmlFormatter;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="XmlSerializerHelper"/> class.
        /// </summary>
        static XmlSerializerHelper()
        {
            XmlFormatter = new Dictionary<Type, XmlSerializer>();
        }
        #endregion

       #region Public Members

        /// <summary>
        /// Deserialize an XDocument to instance of an object T
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="xdoc">Document XML.</param>
        /// <returns>Deserialized object</returns>
        public static T Deserialize<T>(XDocument xdoc)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (var reader = xdoc.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Serializes an object instance to an XML represented string.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <returns>
        /// An string that represents the serialized object.
        /// </returns>
        public static string Serialize<T>(T objectToSerialize) where T : new()
        {
            using (StringWriter writer = new StringWriter())
            {
                GetFormatter(objectToSerialize.GetType()).Serialize(writer, objectToSerialize);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Deserializes an XML string to an object instance
        /// </summary>
        /// <typeparam name="T">The Object Type to Deserialize</typeparam>
        /// <param name="xmlString">A string in XML format that representing the serialized object</param>
        /// <returns>
        /// An object instance of T
        /// </returns>
        public static T Deserialize<T>(string xmlString) where T : new()
        {
            if (!string.IsNullOrEmpty(xmlString))
            {
                using (StringReader reader = new StringReader(xmlString))
                {
                    return (T)GetFormatter(typeof(T)).Deserialize(reader);
                }
            }

            return default(T);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the formatter for the specified type. If the formatter is not provided one will be created.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>Serialized object</returns>
        private static XmlSerializer GetFormatter(Type objectType)
        {
            if (!XmlFormatter.ContainsKey(objectType))
            {
                XmlFormatter.Add(objectType, new XmlSerializer(objectType));
            }

            return XmlFormatter[objectType];
        }
        #endregion
    }
}
