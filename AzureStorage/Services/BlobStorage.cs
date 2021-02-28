using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorage.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage.Services
{
    public class BlobStorage
    {
        private readonly string connectionStringStorage;
        
        public BlobStorage(IConfiguration configuration)
        {
            connectionStringStorage = configuration.GetSection("ConnectionStrings:AzureStorage").Value;
        }


        public async Task<Response<List<BlobItem>>> GetAll(string containerName)
        {
            List<BlobItem> blobItems = new List<BlobItem>();

            BlobContainerClient containerClient = await ContainerClient(containerName);

            await foreach (var item in containerClient.GetBlobsAsync())
            {
                blobItems.Add(item);
            }

            return new Response<List<BlobItem>>(blobItems);
        }

        public async Task<Response<byte[]>> Get(string containerName, string blobName)
        {
            byte[] file = Array.Empty<byte>();

            BlobContainerClient containerClient = await ContainerClient(containerName);

            //BlobDownloadInfo blobDownload = await containerClient.GetBlobClient(blobName).DownloadAsync();

            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    await blobDownload.Content.CopyToAsync(memoryStream);
            //    file = memoryStream.ToArray();
            //}

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await containerClient.GetBlobClient(blobName).DownloadToAsync(memoryStream);
                file = memoryStream.ToArray();
            }

            return new Response<byte[]>(file);


        }

        public async Task<Response<string>> Add(string containerName, IFormFile file)
        {

            string nameBlob = Guid.NewGuid().ToString() + "." + file.FileName.Split('.').LastOrDefault();

            BlobContainerClient containerClient = await ContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(nameBlob);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                await blobClient.UploadAsync(memoryStream);
            }

            return new Response<string>(data: nameBlob);

        }

        public async Task<Response<string>> Add(string containerName, byte[] file, string extension)
        {

            string nameBlob = Guid.NewGuid().ToString() + "." + extension;

            BlobContainerClient containerClient = await ContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(nameBlob);
            
            using (MemoryStream memoryStream = new MemoryStream(file, false))
            {
                BlobContentInfo blobContentInfo = await blobClient.UploadAsync(memoryStream);    
            }
            
            return new Response<string>(data: nameBlob);

        }

        public async Task<Response<string>> Add(string containerName, string file, string extension)
        {

            byte[] fileByte = Convert.FromBase64String(file);

            return await Add(containerName, fileByte, extension);

        }

        public async Task<Response<bool>> Delete(string containerName, string blobName) 
        {

            BlobContainerClient containerClient = await ContainerClient(containerName);

            await containerClient.GetBlobClient(blobName).DeleteIfExistsAsync();

            return new Response<bool>(true);
        }

        
        private async Task<BlobContainerClient> ContainerClient(string containerName)
        {

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionStringStorage);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }

    }
}
