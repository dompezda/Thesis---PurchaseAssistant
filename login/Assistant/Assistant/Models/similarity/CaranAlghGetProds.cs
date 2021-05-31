using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models.similarity
{
    public class CaranAlghGetProds
    {
        public ObjectId ProdId { get; set; }

        public int Value { get; set; }
    }
}
