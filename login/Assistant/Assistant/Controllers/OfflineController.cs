using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Assistant.Data;
using Assistant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Assistant.Controllers
{

    [Route("[controller]/[action]")]
    public class OfflineController : Controller
    {

        public MongoDbContext db = new MongoDbContext();
        public static string SelectedId;
        public IActionResult Select_list_to_save_offline()
        {
            var userId = ObjectId.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            foreach (var item in db.MongoLists.AsQueryable().ToList())
            {
                if (item.ProductList.Count() == 0)
                {

                    var ListToRemove = db.MongoLists.AsQueryable().Where(x => x.Id == item.Id).FirstOrDefault();
                    db.MongoLists.DeleteOne(a => a.Id == ListToRemove.Id);
                }
            }

            List<MongoDBProdList> Lists = db.MongoLists.AsQueryable().Where(x => x.UserId == userId).ToList();

            return View("~/Views/Offline/Select_list_to_save_offline.cshtml", Lists);
        }

        [HttpGet("{id}")]

        [Route("[controller]/[action]/{id?}")]

        public JsonResult GetList(string Id)
        {
            List<string> Products = new List<string>();
            List<Product> currentList = new List<Product>();

            currentList = db.MongoLists.AsQueryable().Where(w => w.Id == ObjectId.Parse(SelectedId)).Select(x => x.ProductList).FirstOrDefault();

            List<OfflineProduct> ListToCache = new List<OfflineProduct>();
            JsonSerializer serializer = new JsonSerializer();
            for (int i = 0; i < currentList.Count; i++)
            {
                var Prod = new OfflineProduct();
                Prod.ID = i;
                Prod.Name = currentList[i].Name;
                Prod.Done = false;
                ListToCache.Add(Prod);
            }
            string json = JsonConvert.SerializeObject(ListToCache);

            return Json(json);
        }

        [HttpPost]

        public IActionResult DisplayOfflineList(string Id)
        {
            SelectedId = Id;
            var JsonData = GetList(Id);

            return View(JsonData);
        }



    }
}