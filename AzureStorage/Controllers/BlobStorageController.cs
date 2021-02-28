using AzureStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlobStorageController : ControllerBase
    {
        private readonly BlobStorage blobStorage;
        private readonly string containerName = "demo";
        public BlobStorageController(BlobStorage blobStorage)
        {
            this.blobStorage = blobStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string blobName) 
        {
            return Ok(await blobStorage.Get(containerName, blobName));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await blobStorage.GetAll(containerName));
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file)
        {
            return Ok(await blobStorage.Add(containerName, file));
        }

        [HttpPost]
        public async Task<IActionResult> AddBase64(FileClass file)
        {
            return Ok(await blobStorage.Add(containerName, file.Base64, file.Extension));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string blobName)
        {
            return Ok(await blobStorage.Delete(containerName, blobName));
        }


        public class FileClass 
        {
            public string Base64 { get; set; }
            public string Extension { get; set; }
        }

    }
}
