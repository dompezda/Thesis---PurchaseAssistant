using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class Relation
    {
        public Guid ID { get; set; }
        public Guid ListID { get; set; }
        public ProductList ProductList { get; set; }
        public Guid ProductID { get; set; }
        public Product Product { get; set; }
        public Relation()
        {
            ID = new Guid();
        }
    }
}
