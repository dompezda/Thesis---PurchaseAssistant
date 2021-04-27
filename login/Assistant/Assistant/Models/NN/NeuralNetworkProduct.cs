using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class NeuralNetworkProduct
    {
        public ObjectId IdMongo { get; set; }
        public int IdNN { get; set; }
    }
}
