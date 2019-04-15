using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CJ.Infrastructure.Cache
{
    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public class CacheHelper
    {
        public static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static object GetCache(string key)
        {
            object value;
            _cache.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// 通过键名获取缓存值
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns></returns>
        public static T GetCache<T>(string key)
        {
            T value;
            _cache.TryGetValue(key, out value);
            
            return value;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            try
            {
                _cache.Remove(key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置不过期的缓存
        /// </summary>
        /// <param name="CacheKey">缓存键名</param>
        /// <param name="value">缓存值</param>
        public static void SetCache(object CacheKey, object value)
        {
            _cache.Set(CacheKey, value);
        }

        /// <summary>
        /// 设置相对时间（相对上次访问所过去的时间）的缓存
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="value"></param>
        /// <param name="slidingExpiration"></param>
        public static void SetCache(string CacheKey, object value, TimeSpan slidingExpiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(slidingExpiration);
            _cache.Set(CacheKey, value, cacheEntryOptions);
        }

        /// <summary>
        /// 设置绝对过期时间的缓存
        /// </summary>
        /// <param name="CacheKey">缓存键名</param>
        /// <param name="value">缓存值</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        public static void SetCache(string CacheKey, object value, DateTime absoluteExpiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
           .SetAbsoluteExpiration(new DateTimeOffset(absoluteExpiration));
            _cache.Set(CacheKey, value, cacheEntryOptions);
            
        }

        /// <summary>
        /// 设置含文件依赖项的缓存
        /// </summary>
        /// <param name="CacheKey">缓存键名</param>
        /// <param name="value">缓存值</param>
        /// <param name="filePath">文件依赖项路径</param>
        /// <param name="fileName">文件依赖项名</param>
        public static void SetCache(string CacheKey, object value , string filePath, string fileName)
        {
            var fileProvider = new PhysicalFileProvider(filePath);
            var fileToken = fileProvider.Watch(fileName);
            _cache.Set(CacheKey, value, fileToken);

        }
    }
}
