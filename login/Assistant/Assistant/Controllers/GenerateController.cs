using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assistant.Models;
using System.Net;
using Assistant.Data;
using Assistant.ViewModels;
using Microsoft.EntityFrameworkCore;
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
                        if(User.FindFirstValue(ClaimTypes.NameIdentifier)!=null)
                        {
                            currentlyEditedList.UserId= User.FindFirstValue(ClaimTypes.NameIdentifier);
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

        public IActionResult NeuralNetwork()
        {
            //[wiersze][kolumny]

            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IQueryable<int> GetLists;
            
            List<int> inputIdealList=new List<int>();
            List<int> inputSecondList = new List<int>();
            List<int> inputFirstList = new List<int>();
            double[][] inputNetwork;
            double[][] idealOutput;

            using (var db = new ApplicationDbContext())
            {
                GetLists = db.Lists.Where(p => p.UserId == userId).Where(w => w.CreateDate >= DateTime.Now.AddDays(-60)).Select(w=>w.Id);
                var ListsToUse=GetLists.ToArray();
            
            for (int i = 0; i < ListsToUse.Count() ; i++)
            {
                    int firstListLen;
                    int secondListLen;
                    int idealListLen;
                    if (i+2<ListsToUse.Count())
                    { 
                        var firstList = ListsToUse[i];
                        var secondList = ListsToUse[i + 1];
                        var idealList = ListsToUse[i + 2];
                        inputFirstList = db.ProductLists.Where(p => p.ListId == firstList).Select(w => w.ProductId).ToList();
                        inputSecondList = db.ProductLists.Where(p => p.ListId == secondList).Select(w => w.ProductId).ToList();
                        inputIdealList = db.ProductLists.Where(p => p.ListId == idealList).Select(w => w.ProductId).ToList();
                        firstListLen = inputFirstList.Count();
                        secondListLen = inputSecondList.Count();
                        idealListLen = inputIdealList.Count();
                    }
                    else
                    {
                        break;
                    }
                    int maxLength;
                    if(firstListLen>secondListLen)
                    {
                        maxLength = firstListLen;
                    }
                    else
                    {
                        maxLength = secondListLen;
                    }
                    inputNetwork=new double[maxLength][];
                    idealOutput = new double[idealListLen][];
                    int count = 0;
                    for (int w = 0; w < maxLength; w++)
                    {                      
                        if(inputFirstList[count]==null)
                        {
                            inputNetwork[w] = new double[] { 0, inputSecondList[count] };
                            
                        }
                        if (inputSecondList[count] == null)
                        {
                            inputNetwork[w] = new double[] { inputFirstList[count], 0 };
                        }
                        else
                        {
                            inputNetwork[w] = new double[] { inputFirstList[count], inputSecondList[count] };
                        }
                       
                        count++;
                        
                    }
                    for (int ideal = 0; ideal < idealListLen; ideal++)
                    {
                        idealOutput[ideal] = new double[] { inputIdealList[ideal] };
                    }

                    IMLDataSet trainingSet = new BasicMLDataSet(inputNetwork, idealOutput);
                    IMLTrain train = new ResilientPropagation(network, trainingSet);

                    int epoch = 1;
                    int counterLevenberg = 0;
                    int counterResilient = 0;
                    double ErrorResilient = 20;
                    double ErrorLavenberg = 20;
                    do
                    {
                        var StartError = train.Error;
                        train.Iteration();
                        System.Diagnostics.Debug.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                        System.Diagnostics.Debug.WriteLine(@"Current Error: " + ErrorResilient);
                        
                        
                        
                        epoch++;
                        var EndError = train.Error;
                        if (StartError == EndError)
                        {
                            counterResilient++;
                        }
                        if (counterResilient >= 10)
                        {
                            ErrorResilient++;
                            counterResilient = 0;
                        }
                        System.Diagnostics.Debug.WriteLine(@"Current iteration: " + counterResilient);
                        System.Diagnostics.Debug.WriteLine(@"Current error start: " + StartError);
                        System.Diagnostics.Debug.WriteLine(@"Current error end: " + EndError);
                    } while (train.Error > ErrorResilient);


                    train = new LevenbergMarquardtTraining(network, trainingSet);
                    do
                    {
                        
                        var StartError = train.Error;
                        train.Iteration();
                        System.Diagnostics.Debug.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                        System.Diagnostics.Debug.WriteLine(@"Current Error: " + ErrorLavenberg);
                        epoch++;
                        var EndError = train.Error;
                        if (EndError+ 0.0000000099999>StartError)
                        {
                            counterLevenberg++;
                        }
                        if (counterLevenberg >= 10)
                        {
                            ErrorLavenberg++;
                            counterLevenberg = 0;
                        }
                        System.Diagnostics.Debug.WriteLine(@"Current iteration: " + counterLevenberg);
                        System.Diagnostics.Debug.WriteLine(@"Current error start: " + StartError);
                        System.Diagnostics.Debug.WriteLine(@"Current error end: " + EndError);

                    } while (train.Error > ErrorLavenberg);


                    System.Diagnostics.Debug.WriteLine(@"Neural Network Results:");
                    foreach (IMLDataPair pair in trainingSet)
                    {
                        IMLData output = network.Compute(pair.Input);
                        System.Diagnostics.Debug.WriteLine(pair.Input[0] + @"," + pair.Input[1]
                                          + @", actual=" + output[0] + @",ideal=" + pair.Ideal[0]);
                    }

                }


                
            }



            SerializeObject.Save(userId + ".ser", network);
            

            int Id = 5;
            return RedirectToAction("Create_list","Home",Id);
        }
    }
}
