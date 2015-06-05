//-----------------------------------------------------------------------
// <copyright file= "SecurityExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace JCI.CAM.Common
{
    /// <summary>
    /// Security Extensions
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Creates a Secure String
        /// </summary>
        /// <param name="data">string to be converted</param>
        /// <returns>secure string instance</returns>
        public static System.Security.SecureString CreateSecureString(string data)
        {
            if (data == null || string.IsNullOrEmpty(data))
            {
                return null;
            }

            System.Security.SecureString secureString = new System.Security.SecureString();

            char[] charArray = data.ToCharArray();

            foreach (char ch in charArray)
            {
                secureString.AppendChar(ch);
            }

            return secureString;
        }
    }
}
