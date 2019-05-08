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
using System.Collections.Specialized;
using Quartz.Impl;
using Quartz;
using System.Threading.Tasks;
using CJ.Infrastructure.Encode;
using CJ.Services.Stations.Dtos;
using MimeKit;
using MailKit.Net.Smtp;

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
            StartScheduler("first");

            StartScheduler("second");

            Console.ReadLine();

        }

        private static async Task StartScheduler(string str)
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                var schedulerFactory = new StdSchedulerFactory(props);
                IScheduler scheduler = await schedulerFactory.GetScheduler();

                // and start it off
                await scheduler.Start();

                var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever())//每60秒执行一次
                            .WithIdentity(str)
                            .Build();

                //4、创建任务
                var jobDetail = JobBuilder.Create<HelloJob>()
                                .UsingJobData("taskRunNum", 0)  //通过在Trigger中添加参数值
                                .UsingJobData("ticketTask", str)
                                .WithIdentity(new JobKey(str))
                                .Build();

                //5、将触发器和任务器绑定到调度器中
                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (SchedulerException ex)
            {

            }
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

                TicketTaskDto dto = new TicketTaskDto();

                string dtoStr = JsonConvert.SerializeObject(dto);

                var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())//每两秒执行一次
                            .UsingJobData("key1", dtoStr)
                            .WithIdentity("trigger", "group")
                            .Build();
                //4、创建任务
                var jobDetail = JobBuilder.Create<HelloJob>()
                                .UsingJobData("key1", 321)  //通过在Trigger中添加参数值
                                .UsingJobData("key2", "123")
                                .WithIdentity("job", "group")
                                .Build();
                //5、将触发器和任务器绑定到调度器中
                await scheduler.ScheduleJob(jobDetail, trigger);

                //// some sleep to show what's happening
                //await Task.Delay(TimeSpan.FromSeconds(3));

                //// and last shut down the scheduler when you are ready to close your program
                //await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }
       
    }

    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HelloJob : IJob//创建IJob的实现类，并实现Excute方法。
    {
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;//获取Job中的参数

            var triggerData = context.Trigger.JobDataMap;//获取Trigger中的参数

            var data = context.MergedJobDataMap;//获取Job和Trigger中合并的参数

            var taskRunNum = jobData.GetInt("taskRunNum");
            var ticketTask = jobData.GetString("ticketTask");

            jobData["taskRunNum"] = ++taskRunNum;

            return Task.Run(() =>
            {
                //using (StreamWriter sw = new StreamWriter(@"C:\Users\Administrator\Desktop\error.log", true, Encoding.UTF8))
                //{
                //    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                //}
                Console.WriteLine($"taskRunNum: {taskRunNum}");
                Console.WriteLine($"ticketTask: {ticketTask}");
                if (taskRunNum == 4)
                {
                    context.Scheduler.DeleteJob(new JobKey("first"));
                }
            });
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
    }
}
