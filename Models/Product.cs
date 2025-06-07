using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    public class Product
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductType { get; set; }
        public string? ProductUnit { get; set; }
        public float ProductQuantity { get; set; }
        public float ProductPrice { get; set; }
    }
}
