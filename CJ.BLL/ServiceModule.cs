using CJ.Models;
using CJ.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CJ.Services
{
    public static class ServiceModule
    {
        /// <summary>
        /// 批量注入IOC容器
        /// </summary>
        /// <param name="service"></param>
        public static void Startup(IServiceCollection services)
        {
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //List<Type> ts = assembly.GetTypes().ToList();

            //var result = new Dictionary<Type, Type[]>();

            //foreach (var item in ts.Where(s => !s.IsInterface))
            //{
            //    var interfaceTypes = item.GetInterfaces();
            //    foreach (var interfaceType in interfaceTypes)
            //    {
            //        service.AddScoped(interfaceType, item);
            //    }
            //}


            //foreach (var item in GetClassName("CJ.Services"))
            //{
            //    foreach (var typeArray in item.Value)
            //    {
            //        services.AddScoped(typeArray, item.Key);
            //    }
            //}
            //return services;
            var classNames = GetClassName("CJ.Services");
            services.AddScoped<IRepository<User>, Repository<User>>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> ts = assembly.GetTypes().ToList();
        }

        public static Dictionary<Type, Type[]> GetClassName(string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();

                var result = new Dictionary<Type, Type[]>();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    result.Add(item, interfaceType);
                }
                return result;
            }
            return new Dictionary<Type, Type[]>();
        }
    }
}
