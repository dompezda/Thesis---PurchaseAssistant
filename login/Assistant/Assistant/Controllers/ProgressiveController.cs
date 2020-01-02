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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IO;


namespace Assistant.Controllers
{
    public class ProgressiveController : Controller
    {
        

        public IActionResult Select_list_to_save_offline()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            List<List> Lists = new List<List>();
            using (var db = new ApplicationDbContext())
            {
                Lists = db.Lists.Where(x => x.UserId == userId).Include(x => x.ProductList).ThenInclude(x => x.Product).ToList();
            }

            return View("~/Views/Progressive/Select_list_to_save_offline.cshtml", Lists);
        }
    }
}