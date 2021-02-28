using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.Models
{
    public class BD_Category : TableEntity
    {
        public string Name { get; set; }
    }
}
