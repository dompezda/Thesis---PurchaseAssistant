using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models.similarity
{
    public class PropositionList
    {
        public List<Product> JaccardProducts   { get; set; }
        public List<Product> EuclideanProducts { get; set; }
        public List<Product> CaranProducts { get; set; }
        public List<Product> AssociationProducts { get; set; }

        //public PropositionList()
        //{
        //    JaccardProducts = new List<Product>();
        //    EuclideanProducts = new List<Product>();
        //    CaranProducts = new List<Product>();
        //    AssociationProducts = new List<Product>();
        //}
    }
}
