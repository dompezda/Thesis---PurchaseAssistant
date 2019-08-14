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
        public IActionResult Create_list()
        {

            // listViewModel.productsToPartial = new List<Product>();
            using (var db = new ApplicationDbContext(null))
            {
                listViewModel.productsToPartial =
                    db.ProductLists.Where(x => x.List.Id == currentlyEditedListId).Select(x => x.Product).ToList();
            }


            return View(listViewModel);
        }


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

            return View();
        }

        [HttpGet]
        public IActionResult SelectList()
        {
            listViewModel.productLists = new List<ProductList>();
            using (var db = new ApplicationDbContext(null))
            {
                listViewModel.productLists = db.ProductLists.ToList();
            }
            using (var db = new ApplicationDbContext(null))
            {
                ViewBag.Lists = db.Lists.Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();

            }

            return View();
        }

        [HttpGet]
        public IActionResult Load_List()
        {
            using (var db = new ApplicationDbContext(null))
            {
                listViewModel.productLists = db.ProductLists.ToList();
            }
            using (var db = new ApplicationDbContext(null))
            {
                ViewBag.Lists = db.Lists.Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            }

            return View(listViewModel.productLists);
        }

        [HttpPost]
        public IActionResult Edit_List(List list)
        {
            List listToEdit = new List();
            using (var db = new ApplicationDbContext(null))
            {
                listToEdit = db.Lists.Include(x => x.ProductList)
                 .Where(n => n.Id == list.Id).FirstOrDefault();

            }





            return RedirectToAction(nameof(Create_list), listToEdit);
        }


        [HttpPost]
        public IActionResult AddProduct(ListViewModel recvListViewModel)
        {

            var productList = new ProductList();
            if (recvListViewModel.product.Name != null)
            {
                using (var db = new ApplicationDbContext(null))
                {

                    List currentlyEditedList;

                    // Jeśli nie edytujemy teraz listy to utwórz nową
                    // i ustaw jej Id jako currentlyEditedListId
                    if (currentlyEditedListId == null)
                    {
                        currentlyEditedList = new List { Name = "Lista z " + DateTime.Now.ToString() };
                        db.Lists.Add(currentlyEditedList);
                        db.SaveChanges();
                        currentlyEditedListId = currentlyEditedList.Id;
                    }

                    // Dodaj nowy produkt do listy produktów, jeśli nie istnieje
                    if (!db.Products.Contains(recvListViewModel.product))
                    {
                        db.Products.Add(recvListViewModel.product);
                    }
                    // Weź listę, która ma id równe temu co edytujemy
                    currentlyEditedList = db.Lists.Single(x => x.Id == currentlyEditedListId);

                    // Dodaj powiązanie wiele-do-wielu z listą
                    productList.List = currentlyEditedList;
                    // Dodaj powiązanie wiele-do-wielu z produktem                    
                    productList.Product = recvListViewModel.product;
                    // Dodaj powyższe do bazy i zapisz
                    db.ProductLists.Add(productList);
                    db.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Create_list));
        }

        [HttpPost]
        public IActionResult FinishList()
        {
            using (var db = new ApplicationDbContext(null))
            {
                // Policz ile jest produktów w liście którą teraz edytujemy
                var count = db.ProductLists.Where(x => x.ListId == currentlyEditedListId)
                .Select(x => x.Product).Count();
                System.Console.WriteLine($"count: {count}, currentid: {currentlyEditedListId}");

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

            using (var db = new ApplicationDbContext(null))
            {
                Product ToRemove = db.Products.Include(x => x.ProductList)
                    .Where(n => n.Name == product.Name).FirstOrDefault();
                db.Products.Remove(ToRemove);
                db.SaveChanges();
            }

            return RedirectToAction(nameof(Create_list));
        }

        [HttpPost]
        public IActionResult DeleteList(List list)
        {
            using (var db = new ApplicationDbContext(null))
            {

                List ToRemove = db.Lists.Include(x => x.ProductList)
                    .Where(n => n.Id == list.Id).FirstOrDefault();
                db.Lists.Remove(ToRemove);
                db.SaveChanges();

            }

            return RedirectToAction(nameof(Load_List));
        }

        public ProductList productList = new ProductList();




        [HttpGet]
        public IActionResult ProductList(List<Product> list)
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
