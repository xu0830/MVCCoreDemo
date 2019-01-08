﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CJ.Infrastructure;
using CJ.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCCoreDemo.Models;
using Newtonsoft.Json;

namespace MVCCoreDemo.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            //var user = UserService.GetUserById(1);
            //ViewBag.CurrentUser = user.UserName;

            //JsonModel obj = JsonConvert.DeserializeObject<JsonModel>(response);

            //ViewBag.Msg = obj.Msg;
            //new WebService().LoginApi("13428108149","xucanjie88!?");
            //new WebService().RecommendDaily();
            
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

   
}
