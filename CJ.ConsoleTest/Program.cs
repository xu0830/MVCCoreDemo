using CJ.Infrastructure;
using CJ.Infrastructure.Cache;
using CJ.Infrastructure.Log;
using CJ.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CJ.Infrastructure.Json;
using System.Net;
using RestSharp;
using Newtonsoft.Json;

namespace CJ.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.Enabled = true;
            //timer.Interval = 1000;
            //int second = 1;
            //timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, eventArg) =>
            //{

            //});

            //Console.ReadKey();

            //string response = HttpHelper.SendGetRequest("https://kyfw.12306.cn/otn/resources/js/framework/station_name.js?station_version=1.9094");

            //string jsonData = response.Substring(response.IndexOf("@") + 1, response.LastIndexOf("'") - response.IndexOf("@"));

            //var StationArr = jsonData.Split("@");

            //foreach (var item in StationArr)
            //{
            //    var StationItem = item.Split("|");



            //    Station station = new Station()
            //    {
            //        CNAbbr = StationItem[0],
            //        CNName = StationItem[1],
            //        Code = StationItem[2],
            //        CNPhoneticAlpha = StationItem[3],
            //        Index = StationItem[5]
            //    };
            //    Console.WriteLine(station);

            //}

            List<RestResponseCookie> cookieContainer = new List<RestResponseCookie>();

            //var client = new RestClient("https://kyfw.12306.cn/passport/captcha/captcha-image64?login_site=E&module=login&rand=sjrand&1551239247870&callback=jQuery191008629789637497698_1551237847685&_=1551237847688");
            //var request = new RestRequest(Method.GET);

            //var response = client.Execute<CaptchaImage>(request);

            var response = RestHelper.SendGetRequest("https://kyfw.12306.cn/passport/captcha/captcha-image64?login_site=E&module=login&rand=sjrand&1551239247870&callback=jQuery191008629789637497698_1551237847685&_=1551237847688");

            //cookieContainer.AddRange(response.Cookies);
            int leftBracketIndex = response.Content.IndexOf("(");
            int rightBracketIndex = response.Content.IndexOf(")");

            var data = response.Content.Substring(leftBracketIndex + 1, rightBracketIndex-leftBracketIndex - 1);

            CaptchaImage captchaImage = JsonConvert.DeserializeObject<CaptchaImage>(data);


            StringBuilder sb = new StringBuilder();

            sb.Append("data:image/jpg;base64,");

            sb.Append(captchaImage.Image);

            Console.WriteLine(sb.ToString());

            Console.ReadKey();
        }
    }

    public class CaptchaImage
    {
        public string Result_message { get; set; }    
        public string Result_code { get; set; }    
        public string Image { get; set; }    
    }

    public class Station
    {
        public string CNAbbr { get; set; }
        public string CNName { get; set; }
        public string Code { get; set; }
        public string CNPhoneticAlpha { get; set; }
        public string Index { get; set; }

        public override string ToString()
        {
            return this.CNAbbr + " " + this.CNName
                + " " + this.Code + " " + this.CNPhoneticAlpha + " " + this.Index;
        }
    }
}
