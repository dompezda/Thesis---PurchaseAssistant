using Assistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.ViewModels
{
    public class ListViewModel
    {
        public Product product { get; set; }
        public ProductList productList { get; set; }
        public List<Product> productsToPartial { get; set; }

        public List<ProductList> productLists { get; set; }
    }
}
