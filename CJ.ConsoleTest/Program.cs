﻿using CJ.Infrastructure;
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
using System.Collections.Specialized;
using Quartz.Impl;
using Quartz;
using System.Threading.Tasks;
using CJ.Infrastructure.Encode;

namespace CJ.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 定时器

            //System.Timers.Timer timer = new System.Timers.Timer();
            //timer.Enabled = true;
            //timer.Interval = 60000;//执行间隔时间,单位为毫秒;此时时间间隔为1分钟  
            //timer.Start();
            //timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, eventArg) =>
            //{
            //    //Console.WriteLine(DateTime.Now.Second);
            //    //if (DateTime.Now.Hour == 16 && DateTime.Now.Minute == 19 && DateTime.Now.DayOfWeek != 0
            //    //&& (int)(DateTime.Now.DayOfWeek) != 6)
            //    //{
            //    //    Console.WriteLine(DateTime.Now);
            //    //}
            //});

            //Console.WriteLine((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);

            #endregion


            #region 获取火车站编码Json格式数据
            //string response = HttpHelper.SendGetRequest("https://kyfw.12306.cn/otn/resources/js/framework/favorite_name.js", new CookieContainer());

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
            //        Index = StationItem[3]
            //    };
            //    stationJson.Stations.Add(station);

            //}

            //string path = Directory.GetCurrentDirectory();

            //string directoryPath = path + "\\json";

            //string FilePath = directoryPath + "\\favoritestation.json";

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



            // trigger async evaluation
            string encodeStr = Uri.EscapeDataString("O,0,1,许灿杰,1,445281199508301071,13428108149,N");

            Console.WriteLine(encodeStr);


        }

        private static async Task RunProgram()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // some sleep to show what's happening
                await Task.Delay(TimeSpan.FromSeconds(3));

                // and last shut down the scheduler when you are ready to close your program
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }

        //public async static void QuickTask()
        //{
        //    // construct a scheduler factory
        //    NameValueCollection props = new NameValueCollection
        //    {
        //        { "quartz.serializer.type", "binary" }
        //    };
        //    StdSchedulerFactory factory = new StdSchedulerFactory(props);

        //    // get a scheduler
        //    IScheduler sched = await factory.GetScheduler();
        //    await sched.Start();

        //    // define the job and tie it to our HelloJob class
        //    IJobDetail job = JobBuilder.Create<HelloJob>()
        //        .WithIdentity("myJob", "group1")
        //        .Build();

        //    // Trigger the job to run now, and then every 40 seconds
        //    ITrigger trigger = TriggerBuilder.Create()
        //        .WithIdentity("myTrigger", "group1")
        //        .StartNow()
        //        .WithSimpleSchedule(x => x
        //            .WithIntervalInSeconds(1)
        //            .RepeatForever())
        //    .Build();

        //    await sched.ScheduleJob(job, trigger);
        //}
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
    }
}
