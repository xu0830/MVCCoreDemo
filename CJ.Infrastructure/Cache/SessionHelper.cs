using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Infrastructure
{
    public static class MyHttpContext
    {
        public static IServiceProvider ServiceProvider;
        static MyHttpContext()
        { }
        public static HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
    }

    public class SessionHelper
    {
        public static string GetSession(string key)
        {
            string value = MyHttpContext.Current.Session.GetString(key);
            return value ?? "";
        }

        public static void SetSession(string key, string value)
        {
            MyHttpContext.Current.Session.SetString(key, value);
        }

        public static void RemoveSession(string key)
        {
            MyHttpContext.Current.Session.Remove(key);
        }
    }
}
