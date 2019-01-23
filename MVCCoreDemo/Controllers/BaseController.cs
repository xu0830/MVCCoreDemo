using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using CJ.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCCoreDemo.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 登录状态验证
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var user = SessionHelper.GetSession("user");
            if (String.IsNullOrEmpty(user) || "".Equals(user))
            {
                context.Result = new RedirectResult("/Login/Index");
                return;
            }
        }
    }
}
