
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class ApplicationUser : MongoUser
    {


        public string Home { get; set; }
        public string Gender { get; set; }
        public string Salary { get; set; }


    }
}
