using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CJ.Infrastructure;
using CJ.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVCCoreDemo.Models;

namespace MVCCoreDemo.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        /// <summary>
        /// 登录UI页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            int a = 0;
            return View();
        }

        /// <summary>
        /// 获取Rsa公钥
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetRsaPublicKey()
        {
            return Json(new
            {
                code = 200,
                RsaPublicKey = WebConfig.PublicKey,
            });
        }

        /// <summary>
        /// 验证码生成接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetVerifyPicPosition()
        {
            int entireX = 250;
            int entireY = 140; 
            int radius = 8;
            int side = 16;
            Random rd = new Random();
            int sideLength = (int)(2 * side + 0.5 * radius * (4 + 1.8));
            int maxX = entireX - sideLength;
            int minX = 2 * sideLength;

            int maxY = entireY - sideLength;
            int minY = (int)(radius + 0.5 * 1.8 * radius);

            int PointX = rd.Next(minX+1, maxX+1);
            int PointY = rd.Next(minY+1, maxY+1);

            string picGuid = new Guid().ToString();

            SessionHelper.SetSession("picPosition", PointX + "," +PointY);

            return Json(new {
                PointX,
                PointY
            });
        }

        [HttpPost]
        /// <summary>
        /// 验证码校验接口
        /// </summary>
        /// <param name="picPositionModelInput"></param>
        /// <returns></returns>
        public IActionResult CheckVerifyPic([FromBody]PicPositionModel picPositionModelInput)
        {
            string[] points = SessionHelper.GetSession<string>("picPosition").Split(',');
            int currentX = 0;
            int currentY = 0;
            if (!int.TryParse(points[0], out currentX) || !int.TryParse(points[1], out currentY)) {
                return Json(new
                {
                    code = "203",
                    msg = "validation error"
                });
            }
            else
            {
                if (picPositionModelInput.PointX <= currentX + 1 && picPositionModelInput.PointX  >= currentX - 1)
                {
                    return Json(new
                    {
                        code = "200",
                        msg = "validation success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        code = "203",
                        msg = "validation error"
                    });
                }
            }
            
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login([FromBody]LoginInputModel input)
        {
            //if(_service.CheckUser(input.UserName, input.Password))
            //{
            //    if (input.Remember)
            //    {
            //        HttpContext.Response.Cookies.Append("DemoUser", input.UserName);
            //    }
            //    return Json(new {
            //        code = 200,
            //        msg = "success"
            //    });
            //}
            //else
            //{
            //    return Json(new LoginModelOutput()
            //    {
            //        Msg = "fail",
            //        Code = 401
            //    });
            //}
            return Json(new LoginOutputModel() {
                Msg = "fail",
                Code = 401
            });
        }

        [HttpPost]
        /// <summary>
        /// 退出登录接口
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            //if (_service.Logout())
            //{
            //    return Json(new LoginModelOutput()
            //    {
            //        Msg = "success",
            //        Code = 200,
            //        Url = "/Login/Index"
            //    });
            //}
            //else
            //{
            //    return Json(new LoginModelOutput()
            //    {
            //        Msg = "退出异常",
            //        Code = 401,
            //        Url = "/Login/Index"
            //    });
            //}
            return Json(new LoginOutputModel() {
                Msg = "登出异常"
            });
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
    }
}