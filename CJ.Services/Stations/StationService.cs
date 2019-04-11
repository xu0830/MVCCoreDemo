using AutoMapper;
using CJ.Entities;
using CJ.Infrastructure;
using CJ.Infrastructure.Cache;
using CJ.Infrastructure.EmailHelper;
using CJ.Infrastructure.Encode;
using CJ.Infrastructure.Json;
using CJ.Infrastructure.Log;
using CJ.Infrastructure.Repositories;
using CJ.Services.Quartz.Jobs;
using CJ.Services.Stations.Dtos;
using CJ.Services.Stations.Dtos.response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CJ.Services.Stations
{
    public class StationService : IStationService
    {
        private readonly IRepository<TicketTask> ticketTaskRepository;

        private readonly ISchedulerFactory schedulerFactory;

        private readonly IMapper mapper;

        public StationService(IRepository<TicketTask> _ticketTaskRepository,
            ISchedulerFactory _schedulerFactory,
            IMapper _mapper)
        {
            ticketTaskRepository = _ticketTaskRepository;
            schedulerFactory = _schedulerFactory;
            mapper = _mapper;
        }

        /// <summary>
        /// 获取图片验证码
        /// </summary>
        /// <returns></returns>
        public ValidatePicOutput GetValidatePicUrl()
        {
            try
            {
                string token = Guid.NewGuid().ToString();

                IList<RestResponseCookie> cookieContainer = new List<RestResponseCookie>();

                #region GetJs

                var client_getjs = new RestClient("https://kyfw.12306.cn/otn/HttpZF/GetJS");
                var request_getjs = new RestRequest(Method.GET);
                request_getjs.AddHeader("Accept", "*/*");
                request_getjs.AddHeader("Referer", "https://www.12306.cn/index/");
                request_getjs.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

                IRestResponse response_getjs = client_getjs.Execute(request_getjs);
                int startIndex = response_getjs.Content.IndexOf("logdevice");
                int endIndex = response_getjs.Content.IndexOf("x26hashCode");

                string algIDStr = response_getjs.Content.Substring(startIndex, endIndex - startIndex);
                startIndex = algIDStr.IndexOf("x3d") + 3;
                endIndex = algIDStr.LastIndexOf("\\");
                string algID = algIDStr.Substring(startIndex, endIndex - startIndex);

                #endregion

                #region https://kyfw.12306.cn/otn/HttpZF/logdevice

                var client_0 = new RestClient("https://kyfw.12306.cn/otn/HttpZF/logdevice?algID="+ algID + "&hashCode=Gkw-Y_y6wBSc16GWzx-e02qAfIa_5EG4L0AuRZB6Crg&FMQw=0&q4f3=zh-CN&VySQ=FGFLDAChjTm6conxJL29tPitliq2W5Be&VPIf=1&custID=133&VEek=unspecified&dzuS=0&yD16=0&EOQP=485390435c136bdc557f204512e80047&lEnu=3232235642&jp76=d41d8cd98f00b204e9800998ecf8427e&hAqN=Win32&platform=WEB&ks0Q=d41d8cd98f00b204e9800998ecf8427e&TeRS=1040x1920&tOHY=24xx1080x1920&Fvje=i1l1s1&q5aJ=-8&wNLf=99115dfb07133750ba677d055874de87&0aew=Mozilla/5.0%20(Windows%20NT%2010.0;%20Win64;%20x64;%20rv:66.0)%20Gecko/20100101%20Firefox/66.0&E3gR=5d0f2ccc799dc71ec8dff936c65ac4d4&timestamp=" + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                var request_0 = new RestRequest(Method.GET);
                request_0.AddHeader("Referer", "https://www.12306.cn/index/");
                request_0.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

                request_0.AddParameter("BIGipServerotn", response_getjs.Cookies.Where(c=> c.Name == "BIGipServerotn").FirstOrDefault().Value, ParameterType.Cookie);

                IRestResponse response_0 = client_0.Execute(request_0);

                string resJsonStr = response_0.Content.Substring(response_0.Content.IndexOf("'") + 1, response_0.Content.LastIndexOf("'") - response_0.Content.IndexOf("'") - 1);

                CallBackResponse callBackFunction = JsonConvert.DeserializeObject<CallBackResponse>(resJsonStr);

                cookieContainer.Add(new RestResponseCookie
                {
                    Name = "RAIL_DEVICEID",
                    Value = callBackFunction.Dfp
                });
                cookieContainer.Add(new RestResponseCookie
                {
                    Name = "RAIL_EXPIRATION",
                    Value = callBackFunction.Exp
                });

                #endregion


                #region https://kyfw.12306.cn/otn/resources/login.html

                var client_0_1 = new RestClient("https://kyfw.12306.cn/otn/resources/login.html");
                var request_0_1 = new RestRequest(Method.GET);
                request_0_1.AddHeader("cache-control", "no-cache");
                request_0_1.AddHeader("Connection", "true");
                request_0_1.AddHeader("Host", "kyfw.12306.cn");
                request_0_1.AddHeader("Referer", "https://kyfw.12306.cn/otn/resources/login.html");
                request_0_1.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

                if (cookieContainer.Count > 0)
                {
                    foreach (var cookie in cookieContainer)
                    {
                        request_0_1.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                    }
                }
                /*
                    MomBPBwgfrZqn2o0S1UmP5nTy4Gogq378uLfa4CMBey1VIYSASzf_SaTa8svTgOwbGiwbPQuqkVKUotLdoyl6k8Jdht7KChp9UDK0nx-V2er9i7SqmT38BWZmu9aGvdimgnoriLSDaXeZWvHag3_E8SYlObbDVCf
                    1555255119707
                 */
                IRestResponse response_0_1 = client_0_1.Execute(request_0_1);

                #endregion
               
                

                #region https://kyfw.12306.cn/otn/login/conf
                var client_1 = new RestClient("https://kyfw.12306.cn/otn/login/conf");
                var request_1 = new RestRequest(Method.POST);
                request_1.AddHeader("cache-control", "no-cache");
                request_1.AddHeader("Connection", "true");
                request_1.AddHeader("Host", "kyfw.12306.cn");
                request_1.AddHeader("Referer", "https://kyfw.12306.cn/otn/resources/login.html");
                request_1.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

                if (cookieContainer.Count > 0)
                {
                    foreach (var cookie in cookieContainer)
                    {
                        if (cookie.Name != "JSESSIONID")
                        {
                            request_1.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                        }
                    }
                }

                IRestResponse response_1 = client_1.Execute(request_1);

              
                //if (response_1.Cookies != null && response_1.Cookies.Count > 0)
                //{
                //    var cookie = response_1.Cookies.Where(c => c.Name == "route").FirstOrDefault();
                //    cookieContainer.Add(new RestResponseCookie
                //    {
                //        Name = cookie.Name,
                //        Value = cookie.Value
                //    });

                //}

                #endregion

                #region https://kyfw.12306.cn/otn/index12306/getLoginBanner

                var client_2 = new RestClient("https://kyfw.12306.cn/otn/index12306/getLoginBanner");
                var request_2 = new RestRequest(Method.GET);
                request_2.AddHeader("cache-control", "no-cache");
                request_2.AddHeader("Connection", "true");
                request_2.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

                request_2.AddParameter("RAIL_DEVICEID", callBackFunction.Dfp, ParameterType.Cookie);
                request_2.AddParameter("RAIL_EXPIRATION", callBackFunction.Exp, ParameterType.Cookie);

                IRestResponse response_2 = client_2.Execute(request_2);

                #endregion

                cookieContainer.Add(new RestResponseCookie
                {
                    Name = "route",
                    Value = response_1.Cookies.Where(c => c.Name == "route").FirstOrDefault().Value
                });

                cookieContainer.Add(new RestResponseCookie
                {
                    Name = "JSESSIONID",
                    Value = response_2.Cookies.Where(c => c.Name == "JSESSIONID").FirstOrDefault().Value
                });

                cookieContainer.Add(new RestResponseCookie
                {
                    Name = "BIGipServerotn",
                    Value = response_2.Cookies.Where(c => c.Name == "BIGipServerotn").FirstOrDefault().Value
                });


                #region https://kyfw.12306.cn/passport/web/auth/uamtk-static

                var client_3 = new RestClient("https://kyfw.12306.cn/passport/web/auth/uamtk-static");
                var request_3 = new RestRequest(Method.POST);
                request_3.AddHeader("cache-control", "no-cache");
                request_3.AddHeader("Connection", "true");
                request_3.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

                if (cookieContainer.Count > 0)
                {
                    foreach (var cookie in cookieContainer)
                    {
                        if (cookie.Name != "JSESSIONID")
                        {
                            request_3.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                        }
                    }
                }

                if (response_1.Cookies != null && response_1.Cookies.Count > 0)
                {
                    foreach (var cookie in response_1.Cookies)
                    {
                        if (cookie.Name != "JSESSIONID")
                        {
                            request_3.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                        }
                    }
                }


                request_3.AddParameter("application/x-www-form-urlencoded", "appid=otn", ParameterType.RequestBody);

               
                IRestResponse response_3 = client_3.Execute(request_3);

                cookieContainer.Add(new RestResponseCookie
                {
                    Name = "_passport_session",
                    Value = response_3.Cookies.Where(c => c.Name == "_passport_session").FirstOrDefault().Value
                });

                //cookieContainer.Add(new RestResponseCookie
                //{
                //    Name = "BIGipServerpassport",
                //    Value = response_3.Cookies.Where(c => c.Name == "BIGipServerpassport").FirstOrDefault().Value
                //});

                if (response_3.Cookies.Where(c => c.Name == "BIGipServerpassport").FirstOrDefault() != null)
                {
                    cookieContainer.Add(new RestResponseCookie
                    {
                        Name = "BIGipServerpassport",
                        Value = response_3.Cookies.Where(c => c.Name == "BIGipServerpassport").FirstOrDefault().Value
                    });
                }

                if (response_3.Cookies.Where(c => c.Name == "BIGipServerpassport").FirstOrDefault() != null)
                {
                    cookieContainer.Add(new RestResponseCookie
                    {
                        Name = "BIGipServerpassport",
                        Value = response_3.Cookies.Where(c => c.Name == "BIGipServerpassport").FirstOrDefault().Value
                    });
                }
                if (response_3.Cookies.Where(c => c.Name == "BIGipServerpool_passport").FirstOrDefault() != null)
                {
                    cookieContainer.Add(new RestResponseCookie
                    {
                        Name = "BIGipServerpool_passport",
                        Value = response_3.Cookies.Where(c => c.Name == "BIGipServerpool_passport").FirstOrDefault().Value
                    });
                }

                #endregion


                #region 获取验证码 https://kyfw.12306.cn/passport/captcha/captcha-image64?login_site=E&module=login&rand=sjrand

                var client = new RestClient("https://kyfw.12306.cn/passport/captcha/captcha-image64?login_site=E&module=login&rand=sjrand");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                if (cookieContainer.Count > 0)
                {
                    foreach (var cookie in cookieContainer)
                    {
                        if (cookie.Name == "JSESSIONID")
                        {
                            continue;
                        }
                        request.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                    }
                }

                IRestResponse response = client.Execute(request);


                cookieContainer.Add(new RestResponseCookie
                {
                    Name = "_passport_ct",
                    Value = response.Cookies.Where(c => c.Name == "_passport_ct").FirstOrDefault().Value
                });


                #endregion
                CaptchaImageDto captchaImage = JsonConvert.DeserializeObject<CaptchaImageDto>(response.Content);

                StringBuilder sb = new StringBuilder();

                sb.Append("data:image/jpg;base64,");

                sb.Append(captchaImage.Image);

                
                CacheHelper.SetCache(token, cookieContainer);

                return new ValidatePicOutput
                {
                    Flag = true,
                    Token = token,
                    ImgUrl = sb.ToString(),
                    Msg = "success"
                };

            }
            catch (Exception ex)
            {
                return new ValidatePicOutput
                {
                    Flag = false,
                    Msg = "异常错误"
                };
            }
        }

        /// <summary>
        /// 开启定时任务
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async Task StartScheduler(TicketTaskDto dto)
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
               
                IScheduler scheduler = await schedulerFactory.GetScheduler();

                // and start it off
                await scheduler.Start();

                string dtoStr = JsonConvert.SerializeObject(dto);

                var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(60).RepeatForever())//每两秒执行一次
                            .WithIdentity("trigger", "group")
                            .Build();
                //4、创建任务
                var jobDetail = JobBuilder.Create<TicketOrderJob>()
                                .UsingJobData("taskRunNum", 0)  //通过在Trigger中添加参数值
                                .UsingJobData("ticketTask", dtoStr)
                                .WithIdentity(dto.UserName, "ticketTask")
                                .Build();
                //5、将触发器和任务器绑定到调度器中
                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (SchedulerException ex)
            {
                TicketTask model = ticketTaskRepository.Get(dto.Id);
                model.Status = 2;
                ticketTaskRepository.Update(model);
                LogHelper.Error("启动任务失败，StartScheduler()方法出错 具体异常: " + ex.Message);
            }
        }

        public OrderOutput SubmitOrder(TicketTaskDto input)
        {
            var ticketTask = ticketTaskRepository.GetAll().
                Where(c => c.UserName == input.UserName && c.Status == 0).FirstOrDefault();

            if (ticketTask != null)
            {
                return new OrderOutput()
                {
                    Flag = false,
                    Msg = "任务执行中！"
                };
            }
            else
            {
                TicketTask ticketTaskObj = mapper.Map<TicketTask>(input);
                ticketTaskObj.CreatedTime = DateTime.Now;
                ticketTaskObj.ArriveStation = input.ArriveStation.Name;
                ticketTaskObj.LeftStation = input.LeftStation.Name;
                ticketTaskObj.Status = 0;
                ticketTaskObj = ticketTaskRepository.Insert(ticketTaskObj);

                input.Id = ticketTaskObj.Id;

                StartScheduler(input);

                return new OrderOutput()
                {
                    Flag = true,
                    Msg = "任务提交完成， 请耐心等待结果"
                };
            }

            #region taskExecute
            //#region Query
            //var client = new RestClient($"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={input.LeftDate}&leftTicketDTO.from_station={input.LeftStation.Code}&leftTicketDTO.to_station={input.ArriveStation.Code}&purpose_codes=ADULT");
            //var request = new RestRequest(Method.GET);
            //request.AddHeader("cache-control", "no-cache");
            //request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");
            //IRestResponse response = client.Execute(request);

            //TicketsQueryResponse ticketsResponse = JsonConvert.DeserializeObject<TicketsQueryResponse>(response.Content);

            //List<TicketDto> tickets = new List<TicketDto>();
            //foreach (var item in ticketsResponse.Data.Result)
            //{
            //    var props = item.Split('|');
            //    TicketDto ticket = new TicketDto()
            //    {
            //        SecretStr = props[0],
            //        ButtonTextInfo = props[1],
            //        Train_no = props[2],
            //        Station_train_code = props[3],
            //        Start_station_telecode = props[4],
            //        End_station_telecode = props[5],
            //        From_station_telecode = props[6],
            //        To_station_telecode = props[7],
            //        Start_time = props[8],
            //        Arrive_time = props[9],
            //        Lishi = props[10],
            //        CanWebBuy = props[11],
            //        Yp_info = props[12],
            //        Start_train_data = props[13],
            //        Train_seat_feature = props[14],
            //        Location_code = props[15],
            //        From_station_no = props[16],
            //        To_station_no = props[17],
            //        Is_support_card = props[18],
            //        Controlled_train_flag = props[19],
            //        Gg_num = !string.IsNullOrEmpty(props[20]) ? props[20] : "--",
            //        Gr_num = !string.IsNullOrEmpty(props[21]) ? props[21] : "--",
            //        Qt_num = !string.IsNullOrEmpty(props[22]) ? props[22] : "--",
            //        Rw_num = !string.IsNullOrEmpty(props[23]) ? props[23] : "--",
            //        Rz_num = !string.IsNullOrEmpty(props[24]) ? props[24] : "--",
            //        Tz_num = !string.IsNullOrEmpty(props[25]) ? props[25] : "--",
            //        Wz_num = !string.IsNullOrEmpty(props[26]) ? props[26] : "--",
            //        Yb_num = !string.IsNullOrEmpty(props[27]) ? props[27] : "--",
            //        Yw_num = !string.IsNullOrEmpty(props[28]) ? props[28] : "--",
            //        Yz_num = !string.IsNullOrEmpty(props[29]) ? props[29] : "--",
            //        Ze_num = !string.IsNullOrEmpty(props[30]) ? props[30] : "--",
            //        Zy_num = !string.IsNullOrEmpty(props[31]) ? props[31] : "--",
            //        Swz_num = !string.IsNullOrEmpty(props[32]) ? props[32] : "--",
            //        Srrb_num = !string.IsNullOrEmpty(props[33]) ? props[33] : "--",
            //        Yp_ex = props[34],
            //        Seat_types = props[35],
            //        Exchange_train_flag = props[36],
            //        Houbu_train_flag = props[37],
            //        From_station_name = ticketsResponse.Data.Map.GetValue(props[6]).ToString(),
            //        To_station_name = ticketsResponse.Data.Map.GetValue(props[7]).ToString()
            //    };
            //    if (props.Length > 38)
            //    {
            //        ticket.Houbu_seat_limit = props[38];
            //    }
            //    tickets.Add(ticket);
            //}
            //var orderTicket = tickets.Where(c => c.Station_train_code == input.TrainCode).FirstOrDefault();
            //#endregion

            //#region submitOrderRequest
            //var client_1 = new RestClient("https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest");
            //var request_1 = new RestRequest(Method.POST);
            //request_1.AddHeader("cache-control", "no-cache");
            //request_1.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request_1.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            //var passenger = GetPassengerDto(input.UserName);

            //var cookieContainer = CacheHelper.GetCache<IList<RestResponseCookie>>(input.UserName + "_loginStatus");

            //if (cookieContainer != null && cookieContainer.Count > 0)
            //{
            //    cookieContainer.Add(new RestResponseCookie()
            //    {
            //        Name = "_jc_save_fromDate",
            //        Value = input.LeftDate
            //    });

            //    cookieContainer.Add(new RestResponseCookie()
            //    {
            //        Name = "_jc_save_fromStation",
            //        Value = UnicodeHelper.String2Unicode(input.LeftStation.Name).Replace("\\", "%") + "%2C" + input.LeftStation.Code
            //    });

            //    cookieContainer.Add(new RestResponseCookie()
            //    {
            //        Name = "_jc_save_toDate",
            //        Value = input.LeftDate
            //    });

            //    cookieContainer.Add(new RestResponseCookie()
            //    {
            //        Name = "_jc_save_toStation",
            //        Value = UnicodeHelper.String2Unicode(input.ArriveStation.Name).Replace("\\", "%") + "%2C" + input.ArriveStation.Code
            //    });

            //    cookieContainer.Add(new RestResponseCookie()
            //    {
            //        Name = "_jc_save_wfdc_flag",
            //        Value = "dc"
            //    });

            //    CacheHelper.SetCache(input.UserName+ "loginStatus", cookieContainer);

            //    foreach (var item in cookieContainer)
            //    {
            //        request_1.AddParameter(item.Name, item.Value, ParameterType.Cookie);
            //    }


            //}

            //request_1.AddParameter("application/x-www-form-urlencoded",
            //    "secretStr=" +
            //    orderTicket.SecretStr +
            //    "&train_date=" + input.LeftDate + "&back_train_date=" + input.LeftDate + "&tour_flag=dc&purpose_codes=ADULT" +
            //    "&query_from_station_name=" + input.LeftStation.Name + "&query_to_station_name=" + input.ArriveStation.Name,
            //    ParameterType.RequestBody);
            //IRestResponse response_1 = client_1.Execute(request_1);
            //#endregion

            //#region https://kyfw.12306.cn/otn/confirmPassenger/initDc

            //var client_2 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/initDc");

            //var request_2 = new RestRequest(Method.POST);
            //request_2.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            //if (cookieContainer != null && cookieContainer.Count > 0)
            //{
            //    foreach (var item in cookieContainer)
            //    {
            //        request_2.AddParameter(item.Name, item.Value, ParameterType.Cookie);
            //    }
            //}

            //IRestResponse response_2 = client_2.Execute(request_2);

            //int init_seatTypes_index = response_2.Content.IndexOf("init_seatTypes");

            //int init_seatTypes_lastIndex = response_2.Content.IndexOf("defaultTicketTypes");

            //int FormIndex = response_2.Content.IndexOf("ticketInfoForPassengerForm");

            //int FormLastIndex = response_2.Content.LastIndexOf("orderRequestDTO");

            //int limit_ticket_num_index = response_2.Content.LastIndexOf("init_limit_ticket_num");

            //int SubmitTokenIndex = response_2.Content.IndexOf("globalRepeatSubmitToken");

            //int global_lang_index = response_2.Content.IndexOf("global_lang");

            //if (init_seatTypes_lastIndex < 0 || init_seatTypes_index < 0 || FormIndex < 0 || FormLastIndex<0 || limit_ticket_num_index<0 || SubmitTokenIndex<0 || global_lang_index<0)
            //{
            //    return false;
            //}

            //string init_seatTypesTemp = response_2.Content.Substring(init_seatTypes_index, init_seatTypes_lastIndex-init_seatTypes_index);

            //string init_seatTypesStr = init_seatTypesTemp.Substring(init_seatTypesTemp.IndexOf("=") + 1,
            //    init_seatTypesTemp.LastIndexOf("]") - init_seatTypesTemp.IndexOf("="));

            //List<Init_seatType> init_SeatTypes = JsonConvert.DeserializeObject<List<Init_seatType>>(init_seatTypesStr);

            //StringBuilder passengerForm = new StringBuilder();

            //passengerForm.Append(response_2.Content.Substring(FormIndex, FormLastIndex - FormIndex));

            //string passengerFormStr = passengerForm.ToString();

            //TicketInfoForPassengerForm ticketInfoForPassengerForm = JsonConvert.DeserializeObject<TicketInfoForPassengerForm>
            //        (passengerFormStr.Substring(passengerFormStr.IndexOf("=") + 1,
            //        passengerFormStr.LastIndexOf("}") - passengerFormStr.IndexOf("=")));

            //string OrderRequestDTOStr = response_2.Content.Substring(FormLastIndex, limit_ticket_num_index - FormLastIndex);

            //OrderQuestDto orderQuestDTO = JsonConvert.DeserializeObject<OrderQuestDto>(
            //    OrderRequestDTOStr.Substring(OrderRequestDTOStr.IndexOf("=") + 1,
            //        OrderRequestDTOStr.LastIndexOf("}") - OrderRequestDTOStr.IndexOf("=")));

            //#endregion

            //#region GetPassengerDto

            //var client_3 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs");
            //var request_3 = new RestRequest(Method.POST);
            //request_3.AddHeader("cache-control", "no-cache");
            //request_3.AddHeader("Host", "kyfw.12306.cn");
            //request_3.AddHeader("Origin", "https://kyfw.12306.cn");
            //request_3.AddHeader("Referer", "https://kyfw.12306.cn/otn/confirmPassenger/initDc");
            //request_3.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            //if (cookieContainer != null && cookieContainer.Count > 0)
            //{
            //    foreach (var item in cookieContainer)
            //    {
            //        request_3.AddParameter(item.Name, item.Value, ParameterType.Cookie);
            //    }
            //}
            //IRestResponse response_3 = client_3.Execute(request_3);

            //CommonResponse<PassengerData> passengerDTOResponse = JsonConvert.DeserializeObject<CommonResponse<PassengerData>>(response_3.Content);

            //#endregion

            //#region checkOrderInfo
            //var client_4 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo");
            //var request_4 = new RestRequest(Method.POST);
            //request_4.AddHeader("Cache-Control", "no-cache");
            //request_4.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            //request_4.AddHeader("Connection", "true");
            //request_4.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            //if (cookieContainer != null && cookieContainer.Count > 0)
            //{
            //    foreach (var item in cookieContainer)
            //    {
            //        request_4.AddParameter(item.Name, item.Value, ParameterType.Cookie);
            //    }
            //}

            //request_4.AddParameter("application/x-www-form-urlencoded; charset=UTF-8",
            //    $"cancel_flag=2&bed_level_order_num=000000000000000000000000000000&passengerTicketStr={input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N&oldPassengerStr={passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_&tour_flag=dc&randCode=&whatsSelect=1", ParameterType.RequestBody);

            //IRestResponse response_4 = client_4.Execute(request_4);

            //CommonResponse<CheckOrderResponseData> checkOrderResponse = JsonConvert.DeserializeObject<CommonResponse<CheckOrderResponseData>>(response_4.Content);
            //#endregion

            //#region getQueueCount
            //var client_5 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount");
            //var request_5 = new RestRequest(Method.POST);
            //request_5.AddHeader("Cache-Control", "no-cache");
            //request_5.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            //request_5.AddHeader("Connection", "true");
            //request_5.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");
            //if (cookieContainer != null && cookieContainer.Count > 0)
            //{
            //    foreach (var item in cookieContainer)
            //    {
            //        request_5.AddParameter(item.Name, item.Value, ParameterType.Cookie);
            //    }
            //}

            //string dateStr = input.LeftDateJs.Replace(":", "%3A").Replace("+", "%2B");

            ////  "Tue%20Apr%2030%202019%2000%3A00%3A00%20GMT%2B0800%20(%E4%B8%AD%E5%9B%BD%E6%A0%87%E5%87%86%E6%97%B6%E9%97%B4)" 
            //request_5.AddParameter("application/x-www-form-urlencoded; charset=UTF-8 ",
            //    "train_date=" + dateStr +
            //    "&train_no=" + orderQuestDTO.Train_no +
            //    "&stationTrainCode=" + orderQuestDTO.Station_train_code + 
            //    "&seatType=" + input.SeatType + 
            //    "&fromStationTelecode=" + orderQuestDTO.From_station_telecode
            //    + "&toStationTelecode=" + orderQuestDTO.To_station_telecode +
            //    "&leftTicket=" + ticketInfoForPassengerForm.QueryLeftTicketRequestDTO.YpInfoDetail +
            //    "&purpose_codes=00&train_location=" + ticketInfoForPassengerForm.Train_location + "&undefined=", ParameterType.RequestBody);


            //IRestResponse response_5 = client_5.Execute(request_5);

            //CommonResponse<QueueCountResponseData> queueCountResponseData = JsonConvert.DeserializeObject<CommonResponse<QueueCountResponseData>>
            //    (response_5.Content);

            //#endregion

            //#region confirmSingleForQueue
            //var client_6 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue");
            //var request_6 = new RestRequest(Method.POST);
            //request_6.AddHeader("Cache-Control", "no-cache");
            //request_6.AddHeader("Connection", "true");
            //request_6.AddHeader("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            //request_6.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            //if (cookieContainer.Count > 0)
            //{
            //    foreach (var item in cookieContainer)
            //    {
            //        request_6.AddParameter(item.Name, item.Value, ParameterType.Cookie);
            //    }
            //}

            //string passengerTicketStrEncode = Uri.EscapeDataString($"{input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N");
            //string oldPassengerStrEncode = Uri.EscapeDataString($"{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_");

            //request_6.AddParameter("undefined",
            //$"passengerTicketStr={input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N" +
            //$"&oldPassengerStr={passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_" +
            //$"&randCode=&purpose_codes=00&" +
            //$"key_check_isChange={ticketInfoForPassengerForm.Key_check_isChange}" +
            //$"&leftTicketStr={ticketInfoForPassengerForm.LeftTicketStr}&train_location={ticketInfoForPassengerForm.Train_location}" +
            //$"&choose_seats=&seatDetailType=000&whatsSelect=1&roomType=00&dwAll=N", ParameterType.RequestBody);

            //IRestResponse response_6 = client_6.Execute(request_6);

            //CommonResponse<ConfirmSingleResponseData> confirmSingleForQueueResponse = JsonConvert.DeserializeObject<CommonResponse<ConfirmSingleResponseData>>(response_6.Content);

            //if (confirmSingleForQueueResponse.Data.SubmitStatus)
            //{
            //    EmailHelper.Send("1126818689@qq.com", "订票成功", "恭喜订票成功,登录你的12306完成支付");
            //}

            //#endregion

            #endregion
        }

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <returns></returns>
        public LoginServiceDto ValidateLogin(StationServiceInput input)
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sb_2 = new StringBuilder();

            string answer = string.Join(",", input.PointsData.ToArray());

            IList<RestResponseCookie> cookieContainer = (IList<RestResponseCookie>) CacheHelper.GetCache(input.Token);

            if (cookieContainer == null)
            {
                return new LoginServiceDto
                {
                    LoginStatus = false,
                    Result = "验证码失效"
                };
            }

            #region 验证码校验
            var client = new RestClient("https://kyfw.12306.cn/passport/captcha/captcha-check?answer=" + answer + "&rand=sjran&login_site=E");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "true");
            request.AddHeader("Host", "kyfw.12306.cn");
            request.AddHeader("Referer", "https://kyfw.12306.cn/otn/resources/login.html");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");
            if (cookieContainer.Count > 0)
            {
                foreach (var cookie in cookieContainer)
                {
                    if (cookie.Name == "JSESSIONID")
                    {
                        continue;
                    }
                    request.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                }
            }

            IRestResponse response = client.Execute(request);

            ImgValidateResponse imgValidateResponse = JsonConvert.DeserializeObject<ImgValidateResponse>(response.Content);
            if (imgValidateResponse.Result_code != 4)
            {
                return new LoginServiceDto
                {
                    LoginStatus = false,
                    Result = "验证码错误"
                };
            }
            #endregion

           

            #region 登录接口
            var loginAnswer = answer.Replace(",", "%2C");
            var client_2 = new RestClient("https://kyfw.12306.cn/passport/web/login");
            var request_2 = new RestRequest(Method.POST);
            request_2.AddHeader("Connection", "true");
            request_2.AddHeader("Accept", "application/json");
            request_2.AddHeader("cache-control", "no-cache");
            request_2.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request_2.AddHeader("Host", "kyfw.12306.cn");
            request_2.AddHeader("Referer", "https://kyfw.12306.cn/otn/resources/login.html");
            request_2.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

            if (cookieContainer.Count > 0)
            {
                foreach (var cookie in cookieContainer)
                {
                    if (cookie.Name == "JSESSIONID")
                    {
                        continue;
                    }
                    request_2.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                }
            }

            request_2.AddParameter("application/x-www-form-urlencoded", "&username="+ input.UserName +"&password="+ input.Password +"&answer="+ loginAnswer +"&appid=otn", ParameterType.RequestBody);

            
          

            LoginResponseDto loginResponseDto = null;

            IRestResponse response_2 = client_2.Execute(request_2);

            try
            {
                loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(response_2.Content);
            }
            catch (Exception)
            {
                return new LoginServiceDto
                {
                    LoginStatus = false,
                    Result = "登录异常"
                };
            }

            if (loginResponseDto.Result_code != 0)
            {
                return new LoginServiceDto
                {
                    LoginStatus = false,
                    Result = "密码错误"
                };
            }

            #endregion

            #region 获取标识登录状态的cookie
            var client_3 = new RestClient("https://kyfw.12306.cn/passport/web/auth/uamtk");
            var request_3 = new RestRequest(Method.POST);
            request_3.AddHeader("cache-control", "no-cache");
            request_3.AddParameter("application/x-www-form-urlencoded", "appid=otn", ParameterType.RequestBody);

            if (response_2.Cookies.Count > 0)
            {
                foreach (var item in response_2.Cookies)
                {
                    request_3.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }

            IRestResponse response_3 = client_3.Execute(request_3);

            TkResponseModel responseModel = JsonConvert.DeserializeObject<TkResponseModel>(response_3.Content);

            var client_4 = new RestClient("https://kyfw.12306.cn/otn/uamauthclient");
            var request_4 = new RestRequest(Method.POST);
            request_4.AddHeader("cache-control", "no-cache");
            request_4.AddParameter("application/x-www-form-urlencoded", "tk=" + responseModel.Newapptk, ParameterType.RequestBody);

            IRestResponse response_4 = client_4.Execute(request_4);

            //  保存登录状态

            cookieContainer.Clear();
            foreach (var item in response_4.Cookies)
            {
                cookieContainer.Add(item);
            }
          
            CacheHelper.SetCache(input.UserName + "_loginStatus", cookieContainer, new TimeSpan(0, 30, 0));

            #endregion

            #region 校验登陆状态
            var client_5 = new RestClient("https://kyfw.12306.cn/otn/login/checkUser");
            var request_5 = new RestRequest(Method.POST);
            request_5.AddHeader("cache-control", "no-cache");

            if (response_4.Cookies.Count > 0)
            {
                foreach (var item in response_4.Cookies)
                {
                    request_5.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }
            IRestResponse response_5 = client_5.Execute(request_5);
            #endregion

            PassengerData passengerResponse = GetPassengerDto(input.UserName);

            return new LoginServiceDto
            {
                LoginStatus = true,
                Result = "登录成功",
                Passenger = new PassengerOutput(){
                    Name = passengerResponse.Normal_passengers[0].Passenger_name,
                    Account = input.UserName
                }
            };
        } 

        /// <summary>
        /// 获取用户信息
        /// </summary>
        public PassengerData GetPassengerDto(string userName)
        {
            #region Query
            var client_1 = new RestClient($"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date=2019-04-30&leftTicketDTO.from_station=IOQ&leftTicketDTO.to_station=PEQ&purpose_codes=ADULT");
            var request_1 = new RestRequest(Method.GET);
            request_1.AddHeader("cache-control", "no-cache");
            request_1.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");
            IRestResponse response_1 = client_1.Execute(request_1);
            #endregion

            var client = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Host", "kyfw.12306.cn");
            request.AddHeader("Origin", "https://kyfw.12306.cn");
            request.AddHeader("Referer", "https://kyfw.12306.cn/otn/confirmPassenger/initDc");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            request.AddParameter("_jc_save_fromDate", "2019-04-30", ParameterType.Cookie);
            request.AddParameter("_jc_save_fromStation", "%u6DF1%u5733%u5317%2CIOQ", ParameterType.Cookie);
            request.AddParameter("_jc_save_toDate", "2019-04-30", ParameterType.Cookie);
            request.AddParameter("_jc_save_toStation", "%u666E%u5B81%2CPEQ", ParameterType.Cookie);
            request.AddParameter("_jc_save_wfdc_flag", "dc", ParameterType.Cookie);

            IList<RestResponseCookie> cookieContainer = (IList<RestResponseCookie>)CacheHelper.GetCache( userName + "_loginStatus");

            if (cookieContainer != null && cookieContainer.Count > 0)
            {
                foreach (var cookie in cookieContainer)
                {
                    request.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                }
            }

            IRestResponse response = client.Execute(request);
            try
            {
                CommonResponse<PassengerData> passengerResponse = JsonConvert.DeserializeObject<CommonResponse<PassengerData>>(response.Content);
                CacheHelper.SetCache("passenger_" + userName, passengerResponse);
                return passengerResponse.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 车票查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<TicketDto> TicketQuery(StationServiceInput input)
        {
            var client = new RestClient($"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={input.Train_date}&leftTicketDTO.from_station={input.From_station_code}&leftTicketDTO.to_station={input.To_station_code}&purpose_codes={input.Purpose_codes}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);

            TicketsQueryResponse ticketsResponse = JsonConvert.DeserializeObject<TicketsQueryResponse>(response.Content);

            List<TicketDto> tickets = new List<TicketDto>();
            foreach (var item in ticketsResponse.Data.Result)
            {
                var props = item.Split('|');
                TicketDto ticket = new TicketDto()
                {
                    SecretStr = props[0],
                    ButtonTextInfo = props[1],
                    Train_no = props[2],
                    Station_train_code = props[3],
                    Start_station_telecode = props[4],
                    End_station_telecode = props[5],
                    From_station_telecode = props[6],
                    To_station_telecode = props[7],
                    Start_time = props[8],
                    Arrive_time = props[9],
                    Lishi = props[10],
                    CanWebBuy = props[11],
                    Yp_info = props[12],
                    Start_train_data = props[13],
                    Train_seat_feature = props[14],
                    Location_code = props[15],
                    From_station_no = props[16],
                    To_station_no = props[17],
                    Is_support_card = props[18],
                    Controlled_train_flag = props[19],
                    Gg_num = !string.IsNullOrEmpty(props[20]) ? props[20] : "--",
                    Gr_num = !string.IsNullOrEmpty(props[21]) ? props[21] : "--",
                    Qt_num = !string.IsNullOrEmpty(props[22]) ? props[22] : "--",
                    Rw_num = !string.IsNullOrEmpty(props[23]) ? props[23] : "--",
                    Rz_num = !string.IsNullOrEmpty(props[24]) ? props[24] : "--",
                    Tz_num = !string.IsNullOrEmpty(props[25]) ? props[25] : "--",
                    Wz_num = !string.IsNullOrEmpty(props[26]) ? props[26] : "--",
                    Yb_num = !string.IsNullOrEmpty(props[27]) ? props[27] : "--",
                    Yw_num = !string.IsNullOrEmpty(props[28]) ? props[28] : "--",
                    Yz_num = !string.IsNullOrEmpty(props[29]) ? props[29] : "--",
                    Ze_num = !string.IsNullOrEmpty(props[30]) ? props[30] : "--",
                    Zy_num = !string.IsNullOrEmpty(props[31]) ? props[31] : "--",
                    Swz_num = !string.IsNullOrEmpty(props[32]) ? props[32] : "--",
                    Srrb_num = !string.IsNullOrEmpty(props[33]) ? props[33] : "--",
                    Yp_ex = props[34],
                    Seat_types = props[35],
                    Exchange_train_flag = props[36],
                    Houbu_train_flag = props[37],
                    From_station_name = ticketsResponse.Data.Map.GetValue(props[6]).ToString(),
                    To_station_name = ticketsResponse.Data.Map.GetValue(props[7]).ToString()
                };
                if (props.Length > 38)
                {
                    ticket.Houbu_seat_limit = props[38];
                }
                tickets.Add(ticket);
            }
            return tickets;
        }

    }
}
