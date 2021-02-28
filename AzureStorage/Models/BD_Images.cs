using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.Models
{
    public class BD_Images : TableEntity
    {
        public string NameFile { get; set; }
        //public IFormFile File { get; set; }
    }
}
