using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CJ.Infrastructure
{
    public class InfrastructureModule
    {
        public static void Startup(IServiceCollection services)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> ts = assembly.GetTypes().ToList();

            var result = new Dictionary<Type, Type[]>();

            foreach (var item in ts.Where(s => !s.IsInterface))
            {
                var interfaceTypes = item.GetInterfaces();
                foreach (var interfaceType in interfaceTypes)
                {
                    services.AddScoped(interfaceType, item);
                }
            }
        }
    }
    
}
