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
using Newtonsoft.Json.Converters;

namespace CJ.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 定时器
            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.Enabled = true;
            //timer.Interval = 1000;
            //int second = 1;
            //timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, eventArg) =>
            //{

            //});
            #endregion

            #region 获取火车站编码Json格式数据
            //string response = HttpHelper.SendGetRequest("https://kyfw.12306.cn/otn/resources/js/framework/station_name.js?station_version=1.9094", new CookieContainer());

            //string jsonData = response.Substring(response.IndexOf("@") + 1, response.LastIndexOf("'") - response.IndexOf("@"));

            //var StationArr = jsonData.Split("@");

            //StationJson stationJson = new StationJson();
            //stationJson.Stations = new List<Station>();

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
            //    stationJson.Stations.Add(station);

            //}

            //string path = Directory.GetCurrentDirectory();

            //string directoryPath = path + "\\json";

            //string FilePath = directoryPath + "\\station.json";

            //if (!Directory.Exists(directoryPath))
            //{
            //    // Create the directory it does not exist.
            //    Directory.CreateDirectory(directoryPath);
            //}

            //if (!File.Exists(FilePath))  // 判断是否已有相同文件 
            //{
            //    FileStream fs1 = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);

            //    fs1.Close();
            //}

            //using (StreamWriter sw = new StreamWriter(FilePath))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    serializer.Converters.Add(new JavaScriptDateTimeConverter());
            //    serializer.NullValueHandling = NullValueHandling.Ignore;

            //    //构建Json.net的写入流
            //    JsonWriter writer = new JsonTextWriter(sw)
            //    {
            //        Formatting = Formatting.Indented,//格式化缩进
            //        Indentation = 4,  //缩进四个字符
            //        IndentChar = ' '  //缩进的字符是空格
            //    };

            //    //把模型数据序列化并写入Json.net的JsonWriter流中
            //    serializer.Serialize(writer, stationJson);
            //    //ser.Serialize(writer, ht);
            //    writer.Close();
            //    sw.Close();
            //}

            //Console.WriteLine("文件输出成功");
            #endregion

            Console.ReadKey();
        }
    }

    public class StationJson
    {
        public List<Station> Stations { get; set; }
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
