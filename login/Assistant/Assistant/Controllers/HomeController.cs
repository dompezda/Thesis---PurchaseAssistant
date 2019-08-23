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

namespace Assistant.Controllers
{
    public class HomeController : Controller
    {


        public static ListViewModel listViewModel = new ListViewModel();
        public int IdList = 0;

        public static int? currentlyEditedListId = null;



        [HttpGet]
        public IActionResult Connection_Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Starting_screen()
        {

            return View();
        }

        [HttpGet]
        public IActionResult Main_menu()
        {

            List<Product> frequentList = new List<Product>();
            using (var db = new ApplicationDbContext())
            {
                var prodIds = db.Products.Select(x => x.Id);
                int amount = db.Products.Count();

                var groupedProd = db.ProductLists
                                   .GroupBy(q => q.ProductId)
                                   .OrderByDescending(gp => gp.Count())
                                   .Take(amount/2)
                                   .Select(g => g.Key).ToList();

                foreach (var item in groupedProd)
                {
                    frequentList.Add(db.Products.Where(n => n.Id == item).FirstOrDefault());
                }

            }



            return View(frequentList);
        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            bool Check = CheckForInternetConnection();

            if (Check == true)
            {
                return RedirectToAction(nameof(Main_menu));
            }
            else
            {
                return View("Connection_Error");
            }

        }

        [HttpGet]
        public IActionResult Create_list(int? id)
        {
            
           
           if(id!=null)
            {
                using (var db = new ApplicationDbContext())
                {
                    var listFromDB = db.ProductLists.Where(x => x.ListId == id).Select(v => v.Product).ToList();
                    listViewModel.productsToPartial = listFromDB;

                }
            }
           else
            {
                using (var db = new ApplicationDbContext())
                {
                    listViewModel.productsToPartial =
                        db.ProductLists.Where(x => x.List.Id == currentlyEditedListId).Select(x => x.Product).ToList();


                }
            }
           
           



            return View(listViewModel);
        }
  
        [HttpGet]
        public IActionResult SelectList()
        {
            
            listViewModel.productLists = new List<ProductList>();
            using (var db = new ApplicationDbContext())
            {
                listViewModel.productLists = db.ProductLists.ToList();
            }
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Lists = db.Lists.Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();

            }


            return View();
        }

        [HttpGet]
        public IActionResult Load_List()
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var item in db.Lists)
                {
                    var listToCheck = db.ProductLists.Where(n => n.ListId == item.Id).Count();

                    if (listToCheck == 0)
                    {
                        var ListToRemove = db.Lists.Where(n => n.Id == item.Id).FirstOrDefault();
                        db.Remove(ListToRemove);
                        db.SaveChanges();
                    }
                }
            }
            using (var db = new ApplicationDbContext())
            {
                listViewModel.productLists = db.ProductLists.ToList();
            }
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Lists = db.Lists.Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            }

            return View(listViewModel.productLists);
        }

        [HttpPost]
        public IActionResult Edit_List(List list)
        {
            List listToEdit = new List();
            using (var db = new ApplicationDbContext())
            {
                listToEdit = db.Lists.Include(x => x.ProductList)
                 .Where(n => n.Id == list.Id).FirstOrDefault();
            }
            using (var db = new ApplicationDbContext())
            {
                foreach (var item in listToEdit.ProductList)
                {
                    var tempItem = db.Products.Where(x => x.Id == item.ProductId);
                    item.Product = tempItem.FirstOrDefault();
                }
            }
            currentlyEditedListId = listToEdit.Id;
            var amount = listToEdit.ProductList.Count();
            return RedirectToAction(nameof(Create_list), listToEdit);
            
        }


        [HttpPost]
        public IActionResult AddProduct(ListViewModel recvListViewModel)
        {

            var productList = new ProductList();
            if (recvListViewModel.product.Name != null)
            {
                using (var db = new ApplicationDbContext())
                {

                    List currentlyEditedList;
                    if (currentlyEditedListId == null)
                    {
                        currentlyEditedList = new List { Name = "Lista z " + DateTime.Now.ToString() };
                        db.Lists.Add(currentlyEditedList);
                        db.SaveChanges();
                        currentlyEditedListId = currentlyEditedList.Id;
                    }
                    else
                    {
                        currentlyEditedList = db.Lists.Where(p => p.Id == currentlyEditedListId).FirstOrDefault();
                    }
                    var ProdList = db.Products.Select(p=>p.Name);
                    if (!ProdList.Contains(recvListViewModel.product.Name))
                    {
                        db.Products.Add(recvListViewModel.product);
                    }
                    else
                    {
                        recvListViewModel.product = db.Products.Where(p => p.Name == recvListViewModel.product.Name).FirstOrDefault();
                    }

                    currentlyEditedList = db.Lists.Single(x => x.Id == currentlyEditedListId);

                    productList.List = currentlyEditedList;                 
                    productList.Product = recvListViewModel.product;
                    db.ProductLists.Add(productList);
                    db.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Create_list));
        }

        [HttpPost]
        public IActionResult FinishList()
        {
            using (var db = new ApplicationDbContext())
            {
                
                var count = db.ProductLists.Where(x => x.ListId == currentlyEditedListId)
                .Select(x => x.Product).Count();
               

                if (count != 0)
                {
                    currentlyEditedListId = null;
                }

            }

            return RedirectToAction(nameof(Load_List));
        }

        [HttpPost]
        public IActionResult DeleteProduct(Product product)
        {
            using (var db = new ApplicationDbContext())
            {
                var ProductToCheck = db.Products.Where(n => n.Name == product.Name).FirstOrDefault();
                var amount = db.ProductLists.Where(p => p.ProductId == ProductToCheck.Id).Count();
                if (amount == 1)
                {
                    Product ToRemove = db.Products.Include(x => x.ProductList)
                                       .Where(n => n.Name == product.Name).FirstOrDefault();
                    db.Products.Remove(ToRemove);
                    db.SaveChanges();
                }
                else
                {
                    var newListId = db.Lists.Last();
                    var ToRemove = db.ProductLists.Where(n => n.ListId == newListId.Id).Where(p => p.ProductId == ProductToCheck.Id).FirstOrDefault();
                    db.Remove(ToRemove);
                    db.SaveChanges();
                }

            }
            return RedirectToAction(nameof(Create_list));
        }
   

        [HttpPost]
        public IActionResult DeleteList(List list)
        {
            using (var db = new ApplicationDbContext())
            {

                List ToRemove = db.Lists.Include(x => x.ProductList)
                    .Where(n => n.Id == list.Id).FirstOrDefault();
                db.Lists.Remove(ToRemove);
                db.SaveChanges();

            }

            return RedirectToAction(nameof(Load_List));
        }



        [HttpGet]
        public IActionResult ProductList(List<Product> list)
        {

            return PartialView(list);
        }


        [HttpGet]
        public IActionResult FrequentList(List<Product> list)
        {

            return PartialView(list);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }






    }
}
