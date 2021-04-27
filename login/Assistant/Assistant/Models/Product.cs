using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class Product
    {

        public ObjectId Id { get; set; }
        public string Name { get; set; }

        public List<ProductList> ProductList { get; set; }
        
        public Product()
        {
            
            Id = ObjectId.GenerateNewId();
            
        }
    }
}
