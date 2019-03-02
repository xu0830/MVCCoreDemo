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
            string response_1 = HttpHelper.SendGetRequest("https://kyfw.12306.cn/passport/captcha/captcha-image64?login_site=E&module=login&rand=sjrand&1551194559244&callback=jQuery191018701156729533341_1551193310268&_=1551193310271");
            string response_1_1 = HttpHelper.SendGetRequest("https://kyfw.12306.cn/passport/captcha/captcha-image64?login_site=E&module=login&rand=sjrand&1551194559244&callback=jQuery191018701156729533341_1551193310268&_=1551193310271");

            Console.WriteLine(response_1);
            Console.WriteLine();
            Console.WriteLine(response_1_1);

            Console.ReadKey();
            
        }
    }
}
