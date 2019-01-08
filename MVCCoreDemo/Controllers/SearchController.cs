using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CJ.Services;
using MVCCoreDemo.Models;

namespace MVCCoreDemo.Controllers
{
    public class SearchController : BaseController
    {
        [HttpPost]
        public IActionResult SearchApi([FromBody]SearchModelInput input)
        {
            NeteaseService service = new NeteaseService();
            return Json(service.SearchApi(input.searchword, input.searchtype));
        }
    }
}