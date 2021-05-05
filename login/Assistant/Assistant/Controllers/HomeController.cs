﻿using System;
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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Assistant.Controllers
{
    
    public class HomeController : Controller
    {


        public static ListViewModel listViewModel = new ListViewModel();
        public int IdList = 0;
        public string listName;
        public static string isPrivateList;
        public static ObjectId currentlyEditedListId;
        public static ObjectId IdToShare = ObjectId.Empty;

        //protected ApplicationDbContext ApplicationDbContext { get; set; }
        public MongoDbContext db = new MongoDbContext();
        //public MongoDbContext MongoDbContext = new MongoDbContext();


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
        public IActionResult Get_lists()
        {

            return View();
        }
        [HttpGet]
        public IActionResult Main_menu()
        {
            
            List<Product> frequentList = new List<Product>();

            //var Prod = new Product()
            //{
            //    Id = ObjectId.GenerateNewId(),
            //    Name = "Mleko drugie"
            //};
            //db.Products.InsertOne(Prod);
            var prodIds = db.Products.AsQueryable().ToList();
            int amount = prodIds.Count();
            if (amount / 2 > 5)
            {
                amount = 5;
            }

            //var groupedProd = db.ProductList
            //                   .GroupBy(q => q.ProductId)
            //                   .OrderByDescending(gp => gp.Count())
            //                   .Take(amount / 2)
            //                   .Select(g => g.Key).ToList();
            Product prod = new Product();
            var groupedProd = db.ProductList.AsQueryable().GroupBy(x => x.ProductId).OrderByDescending(gp => gp.Count()).Take(amount).Select(g => g.Key).ToList();
            List<Product> sendListToFrequentList = new List<Product>();
            foreach (var item in groupedProd)
            {
               
                prod = db.Products.Find(x => x.Id == item).FirstOrDefault();
                sendListToFrequentList.Add(prod);
            }
            frequentList = sendListToFrequentList;
           
            return View("~/Views/Utility/Main_menu.cshtml", frequentList);
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
                return View("~/View/Utility/Connection_Error.cshtml");
            }

        }

        [HttpGet]
        public IActionResult Create_list(ObjectId? id)
        {

            if (id != null)
            {

                var getIds=db.ProductList.AsQueryable().Where(x => x.ListId == id).Select(y => y.ProductId).ToList();
                var getListFromDB = new List<Product>();
                foreach (var item in getIds)
                {
                    getListFromDB.Add(db.Products.Find(x => x.Id == item).FirstOrDefault());
                }

            }
            else
            {
                
                var GetIds = db.ProductList.AsQueryable().Where(x => x.ListId == currentlyEditedListId).Select(x => x.ProductId).ToList();
                foreach (var item in GetIds)
                {
                    var ProdToAdd = db.Products.AsQueryable().Where(x => x.Id == item).FirstOrDefault();
                    listViewModel.productsToPartial.Add(ProdToAdd);
                }

            }
            listViewModel.productList = new ProductList();
            listViewModel.productList.List = new List();
            var currentList = new List();
                if (id != null)
                {
                    currentList = db.List.AsQueryable().Where(x => x.Id == id).FirstOrDefault();
                }
                else
                {
                    currentList = db.List.AsQueryable().Where(x => x.Id == currentlyEditedListId).FirstOrDefault();
                }

                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != null && currentList != null)
                {
                    listViewModel.productList.List.UserId = currentList.UserId;
                }
                if (currentList != null)
                    listViewModel.productList.List.Id = currentList.Id;
           
            
            return View("~/Views/Home/Create_list.cshtml", listViewModel);
        }
        [HttpPost]
        public IActionResult Send_Name(string Name, string IsPrivate)
        {
            listName = Name;
            isPrivateList = IsPrivate;

                List currentlyEditedList;

                currentlyEditedList = new List { Name = listName };
                db.List.InsertOne(currentlyEditedList);
                if (IsPrivate == "on")
                {
                    var userId = ObjectId.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    currentlyEditedList.UserId = userId;

                }
                currentlyEditedList.ProductList = new List<ProductList>();
                listViewModel.productsToPartial = new List<Product>();
                listViewModel.productList = new ProductList();
                listViewModel.productList.List = new List();
                listViewModel.productList.List.Id = currentlyEditedList.Id;
                currentlyEditedListId = currentlyEditedList.Id;


            
            return View("~/Views/Home/Create_list.cshtml", listViewModel);
        }

        [HttpGet]
        public IActionResult List_name()
        {


            return View("~/Views/Utility/List_name.cshtml");
        }

        [HttpGet]
        public IActionResult SelectList()
        {

            listViewModel.productLists = new List<ProductList>();

                listViewModel.productLists = db.ProductList.AsQueryable().ToList();
            

                ViewBag.Lists = db.List.AsQueryable().Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();

            


            return View();
        }




        [HttpGet]
        public IActionResult Private_list_load()
        {
            var userId = ObjectId.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (var item in db.List.AsQueryable())
                {
                    var listToCheck = db.ProductList.AsQueryable().Where(n => n.ListId == item.Id).Count();

                    if (listToCheck == 0)
                    {
                        var ListToRemove = db.List.AsQueryable().Where(n => n.Id == item.Id).FirstOrDefault();
                        db.List.DeleteOne(x => x.Id == ListToRemove.Id);
                    }
                }
            

                listViewModel.productLists = db.ProductList.AsQueryable().ToList();
            

                ViewBag.Lists = db.List.AsQueryable().Where(x => x.UserId == userId).Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            

            return View("~/Views/ListDisplay/Private_list_load.cshtml", listViewModel.productLists);
        }
        [HttpGet]
        public IActionResult Public_list_load()
        {
      
                foreach (var item in db.List.AsQueryable())
                {
                    var listToCheck = db.ProductList.AsQueryable().Where(n => n.ListId == item.Id).Count();

                    if (listToCheck == 0)
                    {
                        var ListToRemove = db.List.AsQueryable().Where(n => n.Id == item.Id).FirstOrDefault();
                        db.List.DeleteOne(x => x.Id == ListToRemove.Id);  
                    }
                }
            

                listViewModel.productLists = db.ProductList.AsQueryable().ToList();
            

                ViewBag.Lists = db.List.AsQueryable().Where(x => x.UserId == null).Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            

            return View("~/Views/ListDisplay/Public_list_load.cshtml", listViewModel.productLists);
        }




        [HttpGet]
        public IActionResult Load_List()
        {
            
                foreach (var item in db.List.AsQueryable())
                {
                    var listToCheck = db.ProductList.AsQueryable().Where(n => n.ListId == item.Id).Count();

                    if (listToCheck == 0)
                    {
                        var ListToRemove = db.List.AsQueryable().Where(n => n.Id == item.Id).FirstOrDefault();
                    db.List.DeleteOne(x => x.Id == ListToRemove.Id);

                }
                }
            
            
                listViewModel.productLists = db.ProductList.AsQueryable().ToList();
            
            
                ViewBag.Lists = db.List.AsQueryable().Where(x => x.UserId == null).Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            

            return View("~/Views/ListDisplay/Load_list.cshtml", listViewModel.productLists);
        }

        [HttpPost]
        public IActionResult Edit_List(List list)
        {
            List listToEdit = new List();
            
            
                listToEdit = db.List.AsQueryable().Include(x => x.ProductList).Where(n => n.Id == list.Id).FirstOrDefault();
            
           
                foreach (var item in listToEdit.ProductList)
                {
                    var tempItem = db.Products.AsQueryable().Where(x => x.Id == item.ProductId);
                    item.Product = tempItem.FirstOrDefault();
                }
            
            currentlyEditedListId = listToEdit.Id;
            var amount = listToEdit.ProductList.Count();
            return RedirectToAction(nameof(Create_list), listToEdit);

        }


        [HttpPost]
        public IActionResult ChangeType(ObjectId ListId, ObjectId UserId)
        {
            
                var listToChange = db.List.AsQueryable().Where(x => x.UserId == UserId).Where(p => p.Id == ListId).FirstOrDefault();
                if (listToChange.UserId == null)
                {
                    listToChange.UserId = UserId;
                    return RedirectToAction(nameof(Public_list_load));
                }
                if (listToChange.UserId != null)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    listToChange.UserId = ObjectId.Empty;
                    return RedirectToAction(nameof(Private_list_load));
                }
            


            return RedirectToAction(nameof(Create_list));
        }







        [HttpPost]
        public IActionResult AddProduct(Product product)
        {

            var productList = new ProductList();
            Product NewProd = new Product();
            if (product.Name != null)
            {

                
                    var ProdList = db.Products.AsQueryable().Select(p => p.Name).ToList();
                    if (!ProdList.Contains(product.Name))
                    {
                    NewProd.Id = product.Id;
                    NewProd.Name = product.Name;
                    db.Products.InsertOne(NewProd);
                    }
                    else
                    {
                    NewProd = db.Products.AsQueryable().Where(p => p.Name == product.Name).FirstOrDefault();
                    }

                    var currentlyEditedList = db.List.AsQueryable().Single(x => x.Id == currentlyEditedListId);
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    productList.List = currentlyEditedList;
                    productList.ListId = currentlyEditedList.Id;
                    productList.Product = NewProd;
                    productList.ProductId = NewProd.Id;
                    currentlyEditedList.UserId = ObjectId.Parse(userId);
                    db.ProductList.InsertOne(productList);
                    
                

            }
            return RedirectToAction(nameof(Create_list),currentlyEditedListId);
        }

        [HttpPost]
        public IActionResult FinishList(ObjectId? id)
        {
            List ToCheck = new List();
            string userId = null;
            string checkPrivate = null;
            
                if (id == null)
                {
                    id = currentlyEditedListId;
                }
                

                if (currentlyEditedListId != null && isPrivateList == "on")
                {
                    ToCheck = db.List.AsQueryable().Where(x => x.Id == currentlyEditedListId).FirstOrDefault();
                    checkPrivate = db.List.AsQueryable().Where(x => x.Id == currentlyEditedListId).Select(w => w.UserId).ToString();

                }
                
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //if(db.Lists.Where(x => x.Id == id).Select(w => w.UserId).FirstOrDefault().ToString()!=null)
                //{ 
                //check = db.Lists.Where(x => x.Id == id).Select(w => w.UserId).FirstOrDefault().ToString();
                //}
                //else
                //{
                //    check = null;
                //}
            

            if (ToCheck.UserId != null)
            {
                return RedirectToAction(nameof(Private_list_load));
            }
            else
            {
                return RedirectToAction(nameof(Public_list_load));
            }
            
        }



        [HttpGet]
        public IActionResult Share_list()
        {
            var userId = ObjectId.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (var item in db.List.AsQueryable().ToList())
                {
                    var listToCheck = db.ProductList.AsQueryable().Where(n => n.ListId == item.Id).Count();

                    if (listToCheck == 0)
                    {
                        var ListToRemove = db.List.AsQueryable().Where(n => n.Id == item.Id).FirstOrDefault();
                        db.List.DeleteOne(x => x.Id == ListToRemove.Id);
                    }
                }
            
          
                listViewModel.productLists = db.ProductList.AsQueryable().ToList();
            
            
                ViewBag.Lists = db.List.AsQueryable().Where(x => x.UserId == userId).Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            

            return View("~/Views/Utility/Share_list.cshtml", listViewModel.productLists);
        }

        [HttpPost]
        public IActionResult Get_share_email(ObjectId ListId)
        {
            IdToShare = ListId;
            return View();
        }

        //[HttpPost]
        //public IActionResult Send_email(string mail)
        //{
           
        //    List ListToShare = new List();
        //    List<string> usersEmails = new List<string>();
        //    bool Exist = false;
            
        //        usersEmails = db.Users.AsQueryable().Select(x => x.Email).ToList();
            
        //    foreach (var item in usersEmails)
        //    {
        //        if (item == mail)
        //        {
        //            Exist = true;
        //        }
        //    }

        //    if (Exist == true)
        //    {
        //        ProductList newProdToSave = new ProductList();
        //        ObjectId NewUserId;
               
        //            var friendMail = User.FindFirstValue(ClaimTypes.Name);
        //            NewUserId = db.Users.AsQueryable().Where(x => x.Email == mail).Select(y => y.Id).FirstOrDefault();
        //            List newList = new List();
        //            newList.UserId = NewUserId;

        //            newList.Name = "Lista udostepniona przez " + friendMail;

        //            db.List.InsertOne(newList);
                    
        //            var getProd = db.ProductList.AsQueryable().Where(x => x.ListId == IdToShare).Select(w => w.ProductId);
        //            foreach (var item in getProd)
        //            {

        //                var newProd = db.Products.AsQueryable().Where(x => x.Id == item).FirstOrDefault();
        //                newProdToSave.ListId = newList.Id;
        //                newProdToSave.List = newList;
        //                newProdToSave.ProductId = newProd.Id;
        //                newProdToSave.Product = newProd;
        //                db.ProductList.InsertOne(newProdToSave);

        //            }



                

        //    }
        //    else
        //    {

        //        ViewData["ErrorMessage"] = "Adres E-mail niepoprawny lub nie isnieje w bazie";
        //    }

        //        var userId = ObjectId.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //        ViewBag.Lists = db.List.AsQueryable().Where(x => x.UserId == userId).Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            
        //    return View("~/Views/ListDisplay/Private_list_load.cshtml");
        //}

        [HttpPost]
        public IActionResult DeleteProduct(ObjectId IdToRemove)
        {
            
                var ProductToCheck = db.Products.AsQueryable().Where(n => n.Id == IdToRemove).FirstOrDefault();
                var amount = db.ProductList.AsQueryable().Where(p => p.ProductId == ProductToCheck.Id).Count();
                if (amount == 1)
                {
                    db.Products.DeleteOne(x=>x.Id== IdToRemove);
                }
                else
                {
                    
                    var ToRemove = db.ProductList.AsQueryable().Where(n => n.ListId == currentlyEditedListId).Where(p => p.ProductId == ProductToCheck.Id).FirstOrDefault();
                    db.ProductList.DeleteOne(x=>x.ListId==currentlyEditedListId && x.ProductId==IdToRemove);
                }

            
            return RedirectToAction(nameof(Create_list));
        }


        [HttpPost]
        public IActionResult DeleteList(List list)
        {
            List ToRemove = new List();


            ToRemove = db.List.AsQueryable().Include(x => x.ProductList).Where(n => n.Id == list.Id).FirstOrDefault();
 
            db.List.DeleteOne(x => x.Id == ToRemove.Id);

            
            if (ToRemove.UserId != null)
            {
                return RedirectToAction(nameof(Private_list_load));
            }
            else
            {
                return RedirectToAction(nameof(Public_list_load));
            }

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

        [HttpGet("{id}")]

        [Route("[controller]/[action]/{id?}")]
       
        public JsonResult ListToShop(ObjectId id)
        {

            List<string> Products = new List<string>();
            List<string> currentList = new List<string>();

                currentList = db.ProductList.AsQueryable().Where(w => w.ListId == id).Select(p => p.Product.Name).ToList();
                foreach (var item in currentList)
                {
                    Products.Add(item);
                }
            
            List<OfflineProduct> ListToCache = new List<OfflineProduct>();
            JsonSerializer serializer = new JsonSerializer();
            for (int i = 0; i < currentList.Count; i++)
            {
                var Prod = new OfflineProduct();
                Prod.ID = i;
                Prod.Name = currentList[i];
                Prod.Done = false;
                ListToCache.Add(Prod);
            }
            string json = JsonConvert.SerializeObject(ListToCache);

            return Json(json);
        }


        [HttpPost]
        public IActionResult ShoppingView(ObjectId id)
        {
            var JsonData = ListToShop(id);

            return View(JsonData);
        }
    }
}