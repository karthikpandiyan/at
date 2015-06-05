// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobStorage.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//    Helper class for Windows Azure storage blobs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.Azure.Framework.Provisioning
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using JCI.Azure.Framework.Provisioning.Interfaces;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Helper class for Windows Azure storage blobs
    /// </summary>
    public class BlobStorage : IBlobStorage
    {
        /// <summary>
        /// The cloud BLOB container
        /// </summary>
        private readonly CloudBlobContainer cloudBlobContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorage"/> class.
        /// </summary>
        /// <param name="blobContainerName">Name of the BLOB container.</param>
        /// <param name="storageConnectionString">The storage connection string.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        public BlobStorage(string blobContainerName, string storageConnectionString, bool isPublic = true)
        {
            Validate.BlobContainerName(blobContainerName, "blobContainerName");
            Validate.String(storageConnectionString, "storageConnectionString");

            var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            this.cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
            this.cloudBlobContainer.CreateIfNotExists();

            if (!isPublic)
            {
                return;
            }

            var permission = this.cloudBlobContainer.GetPermissions();
            permission.PublicAccess = BlobContainerPublicAccessType.Container;
            this.cloudBlobContainer.SetPermissions(permission);
        }

        /// <summary>
        /// Creates a new block blob and populates it from a stream
        /// </summary>
        /// <param name="blobId">The blobId for the block blob</param>
        /// <param name="contentType">The content type for the block blob</param>
        /// <param name="data">The data to store in the block blob</param>
        /// <returns>
        /// The URI to the created block blob
        /// </returns>
        public string CreateBlockBlob(string blobId, string contentType, Stream data)
        {
            Validate.BlobName(blobId, "blobId");
            Validate.String(contentType, "contentType");
            Validate.Null(data, "data");

            var cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(blobId);
            cloudBlockBlob.Properties.ContentType = contentType;
            cloudBlockBlob.UploadFromStream(data);

            return cloudBlockBlob.Uri.ToString();
        }

        /// <summary>
        /// Creates a new block blob and populates it from a byte array
        /// </summary>
        /// <param name="blobId">The blobId for the block blob</param>
        /// <param name="contentType">The content type for the block blob</param>
        /// <param name="data">The data to store in the block blob</param>
        /// <returns>
        /// The URI to the created block blob
        /// </returns>
        public string CreateBlockBlob(string blobId, string contentType, byte[] data)
        {
            Validate.BlobName(blobId, "blobId");
            Validate.String(contentType, "contentType");
            Validate.Null(data, "data");

            var cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(blobId);
            cloudBlockBlob.Properties.ContentType = contentType;
            cloudBlockBlob.UploadFromByteArray(data, 0, data.Length);

            return cloudBlockBlob.Uri.ToString();
        }

        /// <summary>
        /// Creates a new block blob and populates it from a string
        /// </summary>
        /// <param name="blobId">The blobId for the block blob</param>
        /// <param name="contentType">The content type for the block blob</param>
        /// <param name="data">The data to store in the block blob</param>
        /// <returns>
        /// The URI to the created block blob
        /// </returns>
        public string CreateBlockBlob(string blobId, string contentType, string data)
        {
            Validate.BlobName(blobId, "blobId");
            Validate.String(contentType, "contentType");
            Validate.String(data, "data");

            var cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(blobId);
            cloudBlockBlob.Properties.ContentType = contentType;
            cloudBlockBlob.UploadText(data);

            return cloudBlockBlob.Uri.ToString();
        }

        /// <summary>
        /// Creates a new block blob and populates it from a file
        /// </summary>
        /// <param name="blobId">The blobId for the block blob</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// The URI to the created block blob
        /// </returns>
        public string CreateBlockBlob(string blobId, string filePath)
        {
            Validate.BlobName(blobId, "blobId");
            Validate.String(filePath, "contentType");

            var cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(blobId);            
            cloudBlockBlob.UploadFromFile(filePath, FileMode.OpenOrCreate);

            return cloudBlockBlob.Uri.ToString();
        }

        /// <summary>
        /// Gets a reference to a block blob with the given unique blob name
        /// </summary>
        /// <param name="blobId">The unique block blob identifier</param>
        /// <returns>
        /// A reference to the block blob
        /// </returns>
        public CloudBlockBlob GetBlockBlobReference(string blobId)
        {
            Validate.BlobName(blobId, "blobId");

            return this.cloudBlobContainer.GetBlockBlobReference(blobId);
        }

        /// <summary>
        /// Returns as stream with the contents of a block blob
        /// with the given blob name
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <returns>
        /// Stream blob file
        /// </returns>
        public Stream GetBlockBlobDataAsStream(string blobId)
        {
            Validate.BlobName(blobId, "blobId");

            var blob = this.cloudBlobContainer.GetBlockBlobReference(blobId);
            var stream = new MemoryStream();
            blob.DownloadToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        /// Returns as string with the contents of a block blob
        /// with the given blob name
        /// </summary>
        /// <param name="blobId">The BLOB identifier.</param>
        /// <returns>
        /// blob string
        /// </returns>
        public string GetBlockBlobDataAsString(string blobId)
        {
            Validate.BlobName(blobId, "blobId");

            var blob = this.cloudBlobContainer.GetBlockBlobReference(blobId);
            return blob.DownloadText();
        }

        /// <summary>
        /// Returns a list of all the blobs in a container
        /// </summary>
        /// <returns>IListBlobItem list</returns>
        public IEnumerable<IListBlobItem> ListBlobsInContainer()
        {
            return this.cloudBlobContainer.ListBlobs(null, true).ToList();
        }

        /// <summary>
        /// Deletes the blob container
        /// </summary>
        public void DeleteBlobContainer()
        {
            this.cloudBlobContainer.DeleteIfExists();
        }

        /// <summary>
        /// Deletes the block blob with the given unique blob name
        /// </summary>
        /// <param name="blobId">The unique block blob identifier</param>
        public void DeleteBlob(string blobId)
        {
            var blob = this.cloudBlobContainer.GetBlockBlobReference(blobId);
            blob.DeleteIfExists();
        }
    }
}