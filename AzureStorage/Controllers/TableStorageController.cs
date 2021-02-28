using AzureStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Productos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TableStorageController : ControllerBase
    {
        private readonly TableStorage tableStorage;
        private readonly string tableName = "demo";

        public TableStorageController(TableStorage tableStorage)
        {
            this.tableStorage = tableStorage;
        }

        [HttpPost]
        public async Task<IActionResult> Create(BD_Product product)
        {
            return Ok(await tableStorage.AddOrMerge(product, "Products", "partitionKeeey"));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string partitionKey, string rowKey) 
        {
            return Ok(await tableStorage.Get<BD_Product>(tableName, partitionKey, rowKey));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await tableStorage.GetAll<BD_Product>(tableName));
        }

        [HttpGet]
        public async Task<IActionResult> GetFilter()
        {
            return Ok(await tableStorage.GetFilter<BD_Product>(tableName, T => T.PartitionKey == "2"));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteByEntry(BD_Product product)
        {
            return Ok(await tableStorage.Delete(product, tableName));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteByKeys(string partitionKey, string rowKey)
        {
            var entry = await tableStorage.Get<BD_Product>(tableName, partitionKey, rowKey);
            return Ok(await tableStorage.Delete(entry.Data, tableName));
        }








        [HttpPost]
        public async Task<IActionResult> InsertALot() 
        {
            return Ok(await tableStorage.AddALot());
        }

    }
}
