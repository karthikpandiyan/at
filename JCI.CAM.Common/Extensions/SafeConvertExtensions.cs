//-----------------------------------------------------------------------
// <copyright file= "SafeConvertExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace System
{
    /// <summary>
    /// Convert Extensions
    /// </summary>
    public static class SafeConvertExtensions
    {
        #region [ ToBoolean ]
        /// <summary>
        /// Converts the input string to a boolean and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="defaultValue">A default value to return for a null input value.</param>
        /// <returns>Returns true or false</returns>
        public static bool ToBoolean(this string input, bool defaultValue)
        {
            try
            {
                return Convert.ToBoolean(input);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Converts the input string to a boolean and if null, it returns the default value.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Returns true or false</returns>
        public static bool ToBoolean(this string input)
        {
            return ToBoolean(input, false);
        }
        #endregion
    }
}
