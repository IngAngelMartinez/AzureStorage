using AzureStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QueueStorageController : ControllerBase
    {
        private readonly QueueStorage queueStorage;
        private readonly string queueName = "demo";

        public QueueStorageController(QueueStorage queueStorage)
        {
            this.queueStorage = queueStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            return Ok(await queueStorage.Peek(queueName));
        }

        [HttpPost]
        public async Task<IActionResult> Add(string message) 
        {
            return Ok(await queueStorage.Add(queueName, message));
        }

        [HttpPost]
        public async Task<IActionResult> Update(string messageText) 
        {
            return Ok(await queueStorage.Update(queueName, messageText));
        }

        [HttpPost]
        public async Task<IActionResult> Delete() 
        {
            return Ok(await queueStorage.Delete(queueName));
        }

    }
}
