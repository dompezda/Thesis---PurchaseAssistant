using AspNetCore.Identity.MongoDbCore.Models;
using AspNetCore.IdentityProvider.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Home { get; set; }
        public string Gender { get; set; }
        public string Salary { get; set; }


    }
}
