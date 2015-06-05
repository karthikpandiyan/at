//-----------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Microsoft">
//     Microsoft &amp; JCI .
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.SiteProvisioningAppWeb
{
    using System;
    using System.IO;
    using System.Text;
    
    /// <summary>
    /// JSON helper to serialize-deserialize data
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// Serializes object
        /// </summary>
        /// <typeparam name="T">Type of Object</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>Serialized JSON string</returns>
        public static string Serialize<T>(T obj)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }

        /// <summary>
        /// Deserializes object
        /// </summary>
        /// <typeparam name="T">Type of Object</typeparam>
        /// <param name="json">serialized JSON</param>
        /// <returns>Object of type T</returns>
        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
            }

            // ms.Close();
            // ms.Dispose();
            return obj;
        }
    }
}