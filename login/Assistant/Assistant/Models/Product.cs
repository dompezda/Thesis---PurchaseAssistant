using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class Product
    {


        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }

        
        public Product()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
