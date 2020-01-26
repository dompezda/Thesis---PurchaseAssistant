using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class Product
    {

        public int Id { get; set; }
        public string Name { get; set; }
        //public double Price { get; set; }


        public List<ProductList> ProductList { get; set; }

        public Product()
        {
            
        }
    }
}
