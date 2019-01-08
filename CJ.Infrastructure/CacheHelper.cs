using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Infrastructure
{
    public static class CacheHelper
    {
        private static Dictionary<string, object> cookiesContainer = new Dictionary<string, object>();

        public static Object GetKey(string keyName)
        {
            Object obj = new object();
            cookiesContainer.TryGetValue(keyName, out obj);
            return obj;
        }

        public static void SetKey(string keyName, object obj)
        {
            cookiesContainer.Add(keyName, obj);
        }
    }
}
