using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using CJ.Infrastructure;
using Microsoft.Net.Http.Headers;
using MVCCoreDemo.Models;
using CJ.Services.Users.Dtos;
using CJ.Infrastructure.Cache;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCCoreDemo.Controllers
{
    /// <summary>
    /// 控制器父类
    ///     判断referer是否合法
    ///         以及是否登录
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 判断referer 是否合法
        /// </summary>
        /// <returns></returns>
        private bool IsRefererValid(ActionExecutingContext context)
        {
            if (WebConfig.RefererVerify)
            {
                string refererContent = context.HttpContext.Request.Headers[HeaderNames.Referer].ToString();
                if (!refererContent.StartsWith(WebConfig.ClientRootAddress) && !refererContent.StartsWith(WebConfig.ServerRootAddress))
                {
                    return false;
                }
            }
            
            return true;
        }

        ///// <summary>
        ///// 登录状态验证 && referer  
        ///// </summary>
        ///// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            string token = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token) || CacheHelper.GetCache(token) == null)
            {
                context.Result = Json(new OutputModel
                {
                    Code = 204,
                    Result = "用户未登录"
                });
            }

            //if (String.IsNullOrEmpty(user) || "".Equals(user))
            //{
            //    context.Result = new RedirectResult("/Login/Index");
            //    return;
            //}
            //if (!IsRefererValid(context)) {
            //    context.Result = Json(new OutputModel
            //    {
            //        Code = 202
            //    });
            //}
        } 
    }
}
