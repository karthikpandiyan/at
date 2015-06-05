//-----------------------------------------------------------------------
// <copyright file= "ValidationExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System.Collections.Generic;
    using JCI.CAM.Common.Resources;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Validation Extensions
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates an object for not being null or not being the default value
        /// </summary>
        /// <typeparam name="T">Input value</typeparam>
        /// <param name="input">The object to check</param>
        /// <param name="variableName">The name of the variable name to report in the error</param>
        /// <exception cref="System.ArgumentException">Thrown when variable is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when variable is null</exception>
        public static void ValidateNotNullOrEmpty<T>(this T input, string variableName)
        {
            if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrEmpty(input as string))
                {
                    throw (input == null)
                      ? new ArgumentNullException(variableName)
                      : new ArgumentException(CommonResources.Exception_Message_EmptyString_Arg, variableName);
                }
            }
            else if (typeof(T).IsSubclassOf(typeof(ClientObject)))
            {
                if (input == null || (input as ClientObject).ServerObjectIsNull == true)
                {
                    throw new ArgumentNullException(variableName);
                }
            }
            else
            {
                if (EqualityComparer<T>.Default.Equals(input, default(T)))
                {
                    throw new ArgumentException(variableName);
                }
            }
        }
    }
}
