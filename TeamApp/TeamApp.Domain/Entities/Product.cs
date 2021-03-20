using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Product
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
    }
}
