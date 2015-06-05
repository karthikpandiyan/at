// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBlobStorage.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//    Helper class for Windows Azure storage blobs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.Azure.Framework.Provisioning.Interfaces
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Helper class for Windows Azure storage blobs
    /// </summary>
    public interface IBlobStorage
    {
        /// <summary>
        /// Creates the block BLOB.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="data">The data.</param>
        /// <returns>Blob reference</returns>
        string CreateBlockBlob(string blobId, string contentType, Stream data);

        /// <summary>
        /// Creates the block BLOB.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="data">The data.</param>
        /// <returns>Blob reference</returns>
        string CreateBlockBlob(string blobId, string contentType, byte[] data);

        /// <summary>
        /// Creates the block BLOB.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="data">The data.</param>
        /// <returns>Blob reference</returns>
        string CreateBlockBlob(string blobId, string contentType, string data);

        /// <summary>
        /// Creates the block BLOB.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Blob reference</returns>
        string CreateBlockBlob(string blobId, string filePath);

        /// <summary>
        /// Gets the block BLOB reference.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <returns>Blob reference</returns>
        CloudBlockBlob GetBlockBlobReference(string blobId);

        /// <summary>
        /// Gets the block BLOB data as stream.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <returns>Blob file stream</returns>
        Stream GetBlockBlobDataAsStream(string blobId);

        /// <summary>
        /// Gets the block BLOB data as string.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <returns>Blob file string</returns>
        string GetBlockBlobDataAsString(string blobId);

        /// <summary>
        /// Lists the blobs in container.
        /// </summary>
        /// <returns>IListBlobItem list</returns>
        IEnumerable<IListBlobItem> ListBlobsInContainer();

        /// <summary>
        /// Deletes the BLOB container.
        /// </summary>
        void DeleteBlobContainer();

        /// <summary>
        /// Deletes the BLOB.
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        void DeleteBlob(string blobId);
    }
}
