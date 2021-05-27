using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models.similarity
{
    public class JaccardListObject
    {
        public ObjectId userId { get; set; }
        public double similarityValue { get; set; }
    }
}
