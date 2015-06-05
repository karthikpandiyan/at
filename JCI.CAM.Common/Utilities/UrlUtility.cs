//-----------------------------------------------------------------------
// <copyright file= "UrlUtility.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.Utilities
{
    using System;

    /// <summary>
    /// URL Utility
    /// </summary>
    public static class UrlUtility
    {
        /// <summary>
        /// The path delimiter
        /// </summary>
        private const char PATHDELIMITER = '/';

        /// <summary>
        /// The invalid character regex
        /// </summary>
        private const string INVALIDCHARSREGEX = @"[\\~#%&*{}/:<>?+|\""]";

        #region [ Combine ]
        /// <summary>
        /// Combines a path and a relative path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="relativePaths">The relative paths.</param>
        /// <returns>return path</returns>
        public static string Combine(string path, params string[] relativePaths)
        {
            string pathBuilder = path ?? string.Empty;

            if (relativePaths == null)
            {
                return pathBuilder;
            }

            foreach (string relPath in relativePaths)
            {
                pathBuilder = Combine(pathBuilder, relPath);
            }

            return pathBuilder;
        }

        /// <summary>
        /// Combines a path and a relative path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="relative">The relative.</param>
        /// <returns>return path</returns>
        public static string Combine(string path, string relative)
        {
            if (relative == null)
            {
                relative = string.Empty;
            }

            if (path == null)
            {
                path = string.Empty;
            }

            if (relative.Length == 0 && path.Length == 0)
            {
                return string.Empty;
            }

            if (relative.Length == 0)
            {
                return path;
            }

            if (path.Length == 0)
            {
                return relative;
            }

            path = path.Replace('\\', PATHDELIMITER);
            relative = relative.Replace('\\', PATHDELIMITER);

            return path.TrimEnd(PATHDELIMITER) + PATHDELIMITER + relative.TrimStart(PATHDELIMITER);
        }
        #endregion
    }
}
