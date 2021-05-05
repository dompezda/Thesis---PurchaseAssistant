using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class ProductList
    {
        public ObjectId _id { get; set; }
        public ObjectId ProductId { get; set; }
        public Product Product { get; set; }
        public ObjectId ListId { get; set; }
        public List List { get; set; }


        public ProductList()
        {

        }

    }
}
