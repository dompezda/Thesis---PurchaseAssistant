using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class ProductList
    {

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ListId { get; set; }
        public List List { get; set; }
        //public List<Product> Products { get; set; }

        //public int PersonID { get; set; }
        //public string ListName { get; set; }

        // public ICollection<Relation> Relations { get; set; }

        public ProductList()
        {
            // ProductListID = 
            //Products = new List<Product>();
            // Relations = new HashSet<Relation>();
        }

    }
}
