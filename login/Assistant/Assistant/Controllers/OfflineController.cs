using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assistant.Data;
using Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assistant.Controllers
{
    
    public class OfflineController : Controller
    {
    
        [HttpPost]
        public async Task<IActionResult> GetList(int GetListId)
        {
            
            List<string> Products = new List<string>();
            List<string> currentList = new List<string>();
            using (var db = new ApplicationDbContext())
            {
                currentList = db.ProductLists.Where(w => w.ListId == GetListId).Select(p => p.Product.Name).ToList();
                foreach (var item in currentList)
                {
                    Products.Add(item);
                    //Products.Add(db.Products.Where(x => x.Id == ProdId).Select(w => w.Name).Take(1).ToString());
                }
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
            string path = "./wwwroot/OfflineList.json";

            if(System.IO.File.Exists(path))
            {
                
                await System.IO.File.WriteAllTextAsync(path, string.Empty);
                
                await System.IO.File.WriteAllTextAsync(path, json);
                
                //await FileWriteAsync(path, json);
            }

            return View(Products);
        }

        public async Task FileWriteAsync(string filePath, string messaage, bool append = true)
        {
            using (FileStream stream = new FileStream(filePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                await sw.WriteLineAsync(messaage);
            }
        }


    }
}