using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productos.Models
{
    public class BD_Product : TableEntity
    {
        public BD_Category Category { get; set; }
        public List<BD_Images> Images { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
    }
}
