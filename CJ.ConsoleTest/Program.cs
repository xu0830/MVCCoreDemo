using CJ.Infrastructure;
using CJ.Infrastructure.Cache;
using CJ.Infrastructure.Log;
using CJ.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CJ.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().ToList();


            DateTime now = DateTime.Now ;


            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            int second = 1;
            timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, eventArg) =>
            {
                
            });

            Console.ReadKey();
            
        }
    }
}
