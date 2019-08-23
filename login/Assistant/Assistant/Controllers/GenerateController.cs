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
    public class GenerateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Select_list()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Generate()
        {
            using (var db = new ApplicationDbContext())
            {
                int amount=db.ProductLists.Count();


            }



                return View();
        }
    }
}