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
using Encog.ML.Data.Versatile;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Neural.Networks.Training.Propagation.Quick;
using Encog.Neural.Networks.Training.Propagation.SCG;

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
        public IActionResult NeuralNetwork(int Days, int alghoritm)
        {
            


            List<int> firstList = new List<int>();
            List<int> secondList = new List<int>();
            List<int> idealList = new List<int>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int[] GetLists;
            DateTime Today = DateTime.Now;
            using (var db = new ApplicationDbContext())
            {
                GetLists = db.Lists.Where(x => x.CreateDate >= Today.AddDays(-Days)).Where(o => o.UserId == userId).OrderBy(w => w.CreateDate).Select(p => p.Id).ToArray();
             
            }
            List<int> ProductsAmount = new List<int>();
            for (int i = 0; i < GetLists.Length; i++)
            {
                using (var db = new ApplicationDbContext())
                {
                    var testorino = db.ProductLists.Where(x => x.ListId == GetLists[i]).Select(w => w.ProductId).ToArray();
                    for (int w = 0; w < testorino.Length; w++)
                    {
                        ProductsAmount.Add(testorino[w]);
                    }
                }

            }
            var OutPutIDs = ProductsAmount.Select(x => x).Distinct().ToList();
            var network = new BasicNetwork();
            CreateNetwork(network, OutPutIDs);



           
            for (int i = 0; i < GetLists.Length - 2; i++)
            {
               //GetData from DB
                using (var db = new ApplicationDbContext())
                {
                    firstList = db.ProductLists.Where(x => x.ListId == GetLists[i]).Select(o => o.ProductId).ToList();
                    secondList = db.ProductLists.Where(x => x.ListId == GetLists[i + 1]).Select(o => o.ProductId).ToList();
                    idealList = db.ProductLists.Where(x => x.ListId == GetLists[i + 2]).Select(o => o.ProductId).ToList();
                }

                List<int> GetAllIds = new List<int>();
                List<int> InputIDs = new List<int>();
                List<int> IdealIDs = new List<int>();
                IdealIDs = idealList.ToList();
                InputIDs = firstList.Concat(secondList).Select(x=>x).Distinct().ToList();
                

                //Products for iteration
                //OutPutIDs
                double[][] InputArray = new double[2][];
                double[][] OutputArray = new double[2][];

                for (int k = 0; k < InputArray.Length; k++)
                {
                    InputArray[k] = new double[OutPutIDs.Count()];
                    OutputArray[k] = new double[OutPutIDs.Count()];

                }

                for (int j = 0; j < OutPutIDs.Count; j++)
                {
                    InputArray[0][j] = OutPutIDs[j];
                    OutputArray[0][j] = OutPutIDs[j];
                }
                for (int l = 0; l < OutPutIDs.Count; l++)
                {
                    InputArray[1][l] = 0;
                    OutputArray[1][l] = 0;
                }
                for (int k = 0; k < OutPutIDs.Count; k++)
                {
                   var jeden= InputArray[0][k];
                    for (int l = 0; l < InputIDs.Count; l++)
                    {
                        if (InputIDs[l] == InputArray[0][k])
                        {
                            var dwa = InputIDs[l];
                            InputArray[1][k] = 1;
                        }
                    }
                   
                }


                for (int k = 0; k < OutPutIDs.Count; k++)
                {
                    var jeden = OutputArray[0][k];
                    for (int l = 0; l < IdealIDs.Count; l++)
                    {
                        if (IdealIDs[l] == OutputArray[0][k])
                        {
                            var dwa = InputIDs[l];
                            OutputArray[1][k] = 1;
                        }
                    }

                }



                var testowowoow = InputArray;
                var wowoowwtest = OutputArray;



                var trainingSet = new BasicMLDataSet(InputArray,OutputArray);


                //train 
                var train = new Backpropagation(network, trainingSet);


                int epoch = 1;
                double trainError = 0.005;
                do
                {
                    train.Iteration();
                    if (epoch % 1000 == 0)
                    {
                        Console.WriteLine($"Epoch #{epoch} Error: {train.Error}");
                    }
                    epoch++;
                }
                while (train.Error > trainError);

                Console.WriteLine($"Epoch #{epoch}");
                train.FinishTraining();


            }


            //Generate new List
            List<int> FirstOfLastsList = new List<int>();
            List<int> LastList = new List<int>();
            using (var db = new ApplicationDbContext())
            {
                FirstOfLastsList = db.ProductLists.Where(x => x.ListId == GetLists[GetLists.Length - 2]).Select(o => o.ProductId).ToList();
                LastList = db.ProductLists.Where(x => x.ListId == GetLists[GetLists.Length - 1]).Select(o => o.ProductId).ToList();

            }
            
            var FinalCheck = LastList.Concat(FirstOfLastsList);

            var FinalIntInput = FinalCheck.Select(x => x).Distinct();
            double[] FinalInput = new double[FinalIntInput.Count()];
            for (int i = 0; i < FinalIntInput.Count(); i++)
            {
                FinalInput[i] = Convert.ToDouble(FinalIntInput.ElementAt(i));
            }


            BasicMLData FinalMLData = new BasicMLData(FinalInput);
            var test0001=network.Compute(FinalMLData);

            double[,] GetOutPutIdsPhaseOne = new double[2, test0001.Count];
            for (int i = 0; i < test0001.Count; i++)
            {

                GetOutPutIdsPhaseOne[0, i] = OutPutIDs.ElementAt(i);
                GetOutPutIdsPhaseOne[1, i] = test0001[i];

            }
            double RequiredWeight = 0.43;
            List<int> OutPutProd = new List<int>();
            for (int i = 0; i < OutPutIDs.Count(); i++)
            {
                if (GetOutPutIdsPhaseOne[1, i] >= RequiredWeight)
                {
                    OutPutProd.Add((int)GetOutPutIdsPhaseOne[0, i]);
                }
            }
            var test00NN = OutPutProd;
            List<string> GeneratedList = new List<string>();
            using (var db = new ApplicationDbContext())
            {

                for (int i = 0; i < test00NN.Count; i++)
                {
                    var Prod = db.Products.Where(x => x.Id == test00NN[i]).Select(w => w.Name).FirstOrDefault().ToString();
                    GeneratedList.Add(Prod);
                    
                }
            }
           
            return RedirectToAction("Create_list","Home");
        }


        public BasicNetwork CreateNetwork(BasicNetwork Network, List<int> OutputNeurons)
        {
            Network.AddLayer(new BasicLayer(null, true, OutputNeurons.Count));
            Network.AddLayer(new BasicLayer(new ActivationSoftMax(), true, OutputNeurons.Count));
            Network.AddLayer(new BasicLayer(new ActivationTANH(), false, OutputNeurons.Count));
            Network.Structure.FinalizeStructure();
            Network.Reset();

            return Network;
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




//BasicMLData inputSecond = new BasicMLData(inputSecondList);

//BasicMLDataSet dataSet = new BasicMLDataSet(InputArray,OutputArray);               //switch (alghoritm)
// {

//     case 1:
//         var trainBackPropagation = new Backpropagation(network, dataSet, 0.3, 0.2);
//         break;
//     case 2: //XX
//         var trainManhattan = new ManhattanPropagation(network, dataSet, 0.00001);
//         break;
//     case 3:
//         var trainQuickProp = new QuickPropagation(network, dataSet);
//         break;
//     case 4:
//         var trainResilientProp = new ResilientPropagation(network, dataSet);
//         break;
//     case 5:
//         var trainSCG = new ScaledConjugateGradient(network, dataSet);
//         break;
//     case 6:
//         var trainLavenberg = new LevenbergMarquardtTraining(network, dataSet);
//         break;
//     case 7:
//         var trainCompetetive = new QuickPropagation(network, dataSet);
//         break;
// }
