using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models.NN
{
    public class NeuralNetworkList
    {
        public ObjectId IdMongoList { get; set; }
        public int IdNNList { get; set; }
    }
}
