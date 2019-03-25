using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
        public static T GetSession<T>(string key)
        {
            string value = MyHttpContext.Current.Session.GetString(key);
            return value == null ? default(T) :
                                 JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetSession(string key, string value)
        {
            MyHttpContext.Current.Session.SetString(key, value);
        }

        public static void SetSession(string key, object value)
        {
            MyHttpContext.Current.Session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static void RemoveSession(string key)
        {
            MyHttpContext.Current.Session.Remove(key);
        }
    }
}
