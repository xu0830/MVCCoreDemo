using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CJ.Infrastructure;
using CJ.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCCoreDemo.Models;

namespace MVCCoreDemo.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if (UserService.IsLogin())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModelInput input)
        {
            if(UserService.CheckUser(input.UserName, input.Password))
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return Json(new LoginModelOutput()
                {
                    Msg = "fail",
                    Code = 401
                });
            }
        }

        public IActionResult Logout()
        {
            if (UserService.Logout())
            {
                return Json(new LoginModelOutput()
                {
                    Msg = "success",
                    Code = 200,
                    Url = "/Login/Index"
                });
            }
            else
            {
                return Json(new LoginModelOutput()
                {
                    Msg = "退出异常",
                    Code = 401,
                    Url = "/Login/Index"
                });
            }
        }
    }
}