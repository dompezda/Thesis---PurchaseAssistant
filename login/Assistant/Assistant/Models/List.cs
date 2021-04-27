using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class List
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<ProductList> ProductList { get; set; }
        public DateTime CreateDate { get; set; }
        public ObjectId UserId { get; set; }

        public List()
        {
            CreateDate = DateTime.Now;
            Id = new ObjectId();

        }

    }
}
