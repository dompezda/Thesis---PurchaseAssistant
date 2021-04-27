//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using Assistant.Data;
//using Assistant.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Memory;
//using Newtonsoft.Json;

//namespace Assistant.Controllers
//{
//    [Route("[controller]/[action]")]
//    public class OfflineController : Controller
//    {
//        public static int SavedList = 0;
//        public IActionResult Select_list_to_save_offline()
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            using (var db = new ApplicationDbContext())
//            {
//                foreach (var item in db.Lists)
//                {
//                    var listToCheck = db.ProductLists.Where(n => n.ListId == item.Id).Count();

//                    if (listToCheck == 0)
//                    {
//                        var ListToRemove = db.Lists.Where(n => n.Id == item.Id).FirstOrDefault();
//                        db.Remove(ListToRemove);
//                        db.SaveChanges();
//                    }
//                }
//            }
//            List<List> Lists = new List<List>();
//            using (var db = new ApplicationDbContext())
//            {
//                Lists = db.Lists.Where(x => x.UserId == userId).Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
//            }

//            return View("~/Views/Offline/Select_list_to_save_offline.cshtml", Lists);
//        }

//        [HttpGet("{id}")]
        
//        public JsonResult GetList(int id)
//        {
//            SavedList = id;
//            List<string> Products = new List<string>();
//            List<string> currentList = new List<string>();
//            using (var db = new ApplicationDbContext())
//            {
//                currentList = db.ProductLists.Where(w => w.ListId == id).Select(p => p.Product.Name).ToList();
//                foreach (var item in currentList)
//                {
//                    Products.Add(item);
//                }
//            }
//            List<OfflineProduct> ListToCache = new List<OfflineProduct>();
//            JsonSerializer serializer = new JsonSerializer();
//            for (int i = 0; i < currentList.Count; i++)
//            {
//                var Prod = new OfflineProduct();
//                Prod.ID = i;
//                Prod.Name = currentList[i];
//                Prod.Done = false;
//                ListToCache.Add(Prod);
//            }
//            string json = JsonConvert.SerializeObject(ListToCache);

//            return Json(json);
//        }

//        [HttpPost]

//        public IActionResult DisplayOfflineList(int id)
//        {
//            var JsonData = GetList(id);

//            return View(JsonData);
//        }
        
        

//    }
//}