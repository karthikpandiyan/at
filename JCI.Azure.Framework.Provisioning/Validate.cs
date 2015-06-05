// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Validate.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Validate Azure inputs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.Azure.Framework.Provisioning
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Validate Azure inputs
    /// </summary>
    public class Validate
    {
        /// <summary>
        /// Nulls the specified parameter value.
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException">Parameter must not be null.</exception>
        public static void Null(object paramValue, string paramName)
        {
            if (paramValue == null)
            {
                throw new ArgumentException("Parameter must not be null.", paramName ?? string.Empty);
            }
        }

        /// <summary>
        /// Strings the specified parameter value.
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException">Parameter must have length greater than zero.</exception>
        public static void String(string paramValue, string paramName)
        {
            Null(paramValue, paramName);
            if (paramValue.Length == 0)
            {
                throw new ArgumentException("Parameter must have length greater than zero.", paramName ?? string.Empty);
            }
        }        

        /// <summary>
        /// BLOBs the name of the container.
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException">Error invalid format</exception>
        public static void BlobContainerName(string paramValue, string paramName)
        {
            Null(paramValue, paramName);

            var regex = new Regex("^(?-i)(?:[a-z0-9]|(?<=[0-9a-z])-(?=[0-9a-z])){3,63}$", RegexOptions.Compiled);
            if (!regex.IsMatch(paramValue))
            {
                throw new ArgumentException(
                    "Blob container names must conform to these rules: " +
                    "Must start with a letter or number, and can contain only letters, numbers, and the dash (-) character. " +
                    "Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names. " +
                    "All letters in a container name must be lowercase. " +
                    "Must be from 3 to 63 characters long.",
                    paramName ?? string.Empty);
            }
        }

        /// <summary>
        /// BLOBs the name.
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException">Blob names must conform to these rules:
        /// Must be from 1 to 1024 characters long.</exception>
        public static void BlobName(string paramValue, string paramName)
        {
            String(paramValue, paramName);

            if (paramValue.Length > 1024)
            {
                throw new ArgumentException(
                    "Blob names must conform to these rules: " +
                    "Must be from 1 to 1024 characters long.", 
                    paramName ?? string.Empty);
            }
        }
    }
}
