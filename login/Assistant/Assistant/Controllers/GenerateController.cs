using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Assistant.Models;
using Assistant.Data;
using System.Security.Claims;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Lma;
using Encog.Util;
using Encog.Util.Arrayutil;

namespace Assistant.Controllers
{
    public class GenerateController : Controller
    {
       
        public static int? currentlyEditedListId = null;
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Select_mode()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Generate()
        {
            using (var db = new ApplicationDbContext())
            {
                int amount = db.ProductLists.Count();


            }



            return View();
        }



        [HttpPost]
        public IActionResult NeuralNetwork(int Days, string alghoritm)
        {
            //[wiersze][kolumny]

            var network = new BasicNetwork();
            CreateNetwork(network);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int[] GetLists;
            DateTime Today = DateTime.Now;

            using (var db = new ApplicationDbContext())
            {
                GetLists = db.Lists.Where(x => x.CreateDate >= Today.AddDays(-Days)).Where(o=>o.UserId==userId).OrderBy(w => w.CreateDate).Select(p => p.Id).ToArray();
            }






                return RedirectToAction("Create_list","Home");
        }



        public List<double> GetAllProducts(List<int> input1,List<int> input2, List<int> output)
        {
            List<double> ListToReturn = new List<double>();
            for (int i = 0; i < input1.Count; i++)
            {
                var number=Convert.ToDouble(input1[i]);
                ListToReturn.Add(number);
            }
            for (int i = 0; i < input2.Count; i++)
            {
                var number = Convert.ToDouble(input2[i]);
                ListToReturn.Add(number);
            }
            for (int i = 0; i < output.Count; i++)
            {
                var number = Convert.ToDouble(output[i]);
                ListToReturn.Add(number);
            }

            return ListToReturn;
        }
        public double[] GetNormalizedArray(List<double> ArrayToNormalize)
        {
            var hi = 1;
            var lo = 0;
            var norm = new NormalizeArray { NormalizedHigh = hi, NormalizedLow = lo };
            var Normalized = norm.Process(ArrayToNormalize.ToArray());
            return Normalized;

        }
        public double[] GetDeNormalizerArray(double min,double max, double[] Array)
        {
            var difference = max - min;
            double[] deNormalizedArray = new double[Array.Length];
            for (int i = 0; i < Array.Length; i++)
            {
                deNormalizedArray[i] = (Array[i] * 10) + 1;
            }
            return deNormalizedArray;
        }

        public BasicNetwork CreateNetwork(BasicNetwork Network)
        {
            Network.AddLayer(new BasicLayer(null, true, 3));
            Network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 15));
            Network.AddLayer(new BasicLayer(new ActivationCompetitive(), true, 10));
            Network.Structure.FinalizeStructure();
            Network.Reset();

            return Network;
        }







        public IActionResult Neural_Network_Settings()
        {
            return View();
        }

        public IActionResult Interpolation()
        {
            List<Product> products = new List<Product>();
            ProductList helperList = new ProductList();
            List currentlyEditedList = new List();
            var productList = new ProductList();



            using (var db = new ApplicationDbContext())
            {
                currentlyEditedList = new List { Name = "Lista z " + DateTime.Now.ToString() };
                db.Lists.Add(currentlyEditedList);
                db.SaveChanges();
                currentlyEditedListId = currentlyEditedList.Id;

                int newId = db.Lists.OrderBy(x => x.Id).Select(p => p.Id).Last();

                var dateCheck = DateTime.Now.AddDays(-21);
                var Lists = db.Lists.Where(x => x.CreateDate > dateCheck).Select(c => c.Id);


                foreach (var item in Lists)
                {
                    var prodId = db.ProductLists.Where(x => x.ListId == item).Select(c => c.ProductId);
                    foreach (var q in prodId)
                    {
                        var productToCheck = db.Products.Where(p => p.Id == q).FirstOrDefault();
                        int amount = db.ProductLists.Where(p => p.ProductId == q).Count();
                        if (!products.Contains(productToCheck) && amount >= 2)
                        {
                            products.Add(productToCheck);
                        }
                    }

                }


                foreach (var itemToAdd in products)
                {
                    var listsWithTheProduct = db.ProductLists.Where(x => x.ProductId == itemToAdd.Id).Select(w => w.List.CreateDate);
                    var twoLastUses = listsWithTheProduct.OrderByDescending(x => x.Date).Take(2);
                    var EndDate = twoLastUses.First();
                    var StartDate = twoLastUses.Last();
                    var interval = (EndDate - StartDate).TotalDays;
                    var TodayPlus = DateTime.Now.AddDays(3);
                    var TodayMinus = DateTime.Now.AddDays(-3);
                    var dateToCheck = EndDate.AddDays(interval);
                    if (dateToCheck >= TodayMinus && dateToCheck <= TodayPlus)
                    {
                        //var prodId = db.Products.Where(x => x.Name == itemToAdd.Name).Select(t=>t.Id).FirstOrDefault();
                        //var prodName= db.Products.Where(x => x.Name == itemToAdd.Name).Select(w => w.Name).FirstOrDefault();
                        currentlyEditedList = db.Lists.Single(x => x.Id == currentlyEditedListId);
                        if (User.FindFirstValue(ClaimTypes.NameIdentifier) != null)
                        {
                            currentlyEditedList.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        }
                        productList.List = currentlyEditedList;
                        productList.Product = itemToAdd;
                        db.ProductLists.Add(productList);
                        db.SaveChanges();
                    }


                }


            }

            if (currentlyEditedList.UserId == null)
            {
                return RedirectToAction("Public_list_load", "Home");//productList }

            }
            else
            {
                return RedirectToAction("Private_list_load", "Home");//productList }
            }
        }
    }
}
