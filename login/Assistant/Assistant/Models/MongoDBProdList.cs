using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class MongoDBProdList
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<Product> ProductList { get; set; }
        public DateTime CreateDate { get; set; }
        public ObjectId UserId { get; set; }

        public MongoDBProdList()
        {
            //Id = ObjectId.GenerateNewId();
            CreateDate = DateTime.Now;
            ProductList = new List<Product>();

        }
    }
}
