using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductList> ProductList { get; set; }

    }
}
