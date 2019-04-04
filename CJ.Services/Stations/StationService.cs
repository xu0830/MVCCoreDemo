using AutoMapper;
using CJ.Entities;
using CJ.Infrastructure;
using CJ.Infrastructure.Cache;
using CJ.Infrastructure.EmailHelper;
using CJ.Infrastructure.Encode;
using CJ.Infrastructure.Json;
using CJ.Infrastructure.Repositories;
using CJ.Services.Stations.Dtos;
using CJ.Services.Stations.Dtos.response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CJ.Services.Stations
{
    public class StationService : IStationService
    {
        private IRepository<TicketTask> stationTaskRepository;

        private IMapper mapper;

        public StationService(IRepository<TicketTask> _stationTaskRepository,
            IMapper _mapper)
        {
            stationTaskRepository = _stationTaskRepository;
            mapper = _mapper;
        }

        /// <summary>
        /// 获取图片验证码的URL
        /// </summary>
        /// <returns></returns>
        public ValidatePicOutput GetValidatePicUrl()
        {
            try
            {
                var client = new RestClient("https://kyfw.12306.cn/passport/captcha/captcha-image64");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");


                IRestResponse response = client.Execute(request);
                CaptchaImageDto captchaImage = JsonConvert.DeserializeObject<CaptchaImageDto>(response.Content);

                StringBuilder sb = new StringBuilder();

                sb.Append("data:image/jpg;base64,");

                sb.Append(captchaImage.Image);

                string token = Guid.NewGuid().ToString();
                CacheHelper.SetCache(token, response.Cookies);

                return new ValidatePicOutput
                {
                    Flag = true,
                    Token = token,
                    ImgUrl = sb.ToString(),
                    Msg = "success"
                };

            }
            catch (Exception)
            {
                return new ValidatePicOutput
                {
                    Flag = false,
                    Msg = "异常错误"
                };
            }
        }

        public bool SubmitOrder(TicketTaskDto input)
        {
            TicketTask ticketTask = mapper.Map<TicketTask>(input);
            ticketTask.CreatedTime = DateTime.Now;
            ticketTask.ArriveStation = input.ArriveStation.Name;
            ticketTask.LeftStation = input.LeftStation.Name;
            stationTaskRepository.InsertAsync(ticketTask);

            #region Query
            var client = new RestClient($"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={input.LeftDate}&leftTicketDTO.from_station={input.LeftStation.Code}&leftTicketDTO.to_station={input.ArriveStation.Code}&purpose_codes=ADULT");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");
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
            var orderTicket = tickets.Where(c => c.Station_train_code == input.TrainCode).FirstOrDefault();
            #endregion

            #region submitOrderRequest
            var client_1 = new RestClient("https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest");
            var request_1 = new RestRequest(Method.POST);
            request_1.AddHeader("cache-control", "no-cache");
            request_1.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request_1.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            var passenger = GetPassengerDto(input.UserName);

            var cookieContainer = CacheHelper.GetCache<IList<RestResponseCookie>>(input.UserName + "_loginStatus");

            if (cookieContainer != null && cookieContainer.Count > 0)
            {
                cookieContainer.Add(new RestResponseCookie()
                {
                    Name = "_jc_save_fromDate",
                    Value = input.LeftDate
                });

                cookieContainer.Add(new RestResponseCookie()
                {
                    Name = "_jc_save_fromStation",
                    Value = UnicodeHelper.String2Unicode(input.LeftStation.Name).Replace("\\", "%") + "%2C" + input.LeftStation.Code
                });

                cookieContainer.Add(new RestResponseCookie()
                {
                    Name = "_jc_save_toDate",
                    Value = input.LeftDate
                });

                cookieContainer.Add(new RestResponseCookie()
                {
                    Name = "_jc_save_toStation",
                    Value = UnicodeHelper.String2Unicode(input.ArriveStation.Name).Replace("\\", "%") + "%2C" + input.ArriveStation.Code
                });

                cookieContainer.Add(new RestResponseCookie()
                {
                    Name = "_jc_save_wfdc_flag",
                    Value = "dc"
                });

                CacheHelper.SetCache(input.UserName+ "loginStatus", cookieContainer);

                foreach (var item in cookieContainer)
                {
                    request_1.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }


            }

            request_1.AddParameter("application/x-www-form-urlencoded",
                "secretStr=" +
                orderTicket.SecretStr +
                "&train_date=" + input.LeftDate + "&back_train_date=" + input.LeftDate + "&tour_flag=dc&purpose_codes=ADULT" +
                "&query_from_station_name=" + input.LeftStation.Name + "&query_to_station_name=" + input.ArriveStation.Name,
                ParameterType.RequestBody);
            IRestResponse response_1 = client_1.Execute(request_1);
            #endregion

            #region https://kyfw.12306.cn/otn/confirmPassenger/initDc

            var client_2 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/initDc");

            var request_2 = new RestRequest(Method.POST);
            request_2.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            if (cookieContainer != null && cookieContainer.Count > 0)
            {
                foreach (var item in cookieContainer)
                {
                    request_2.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }

            IRestResponse response_2 = client_2.Execute(request_2);

            int FormIndex = response_2.Content.IndexOf("ticketInfoForPassengerForm");

            int FormLastIndex = response_2.Content.LastIndexOf("orderRequestDTO");

            int limit_ticket_num_index = response_2.Content.LastIndexOf("init_limit_ticket_num");

            int SubmitTokenIndex = response_2.Content.IndexOf("globalRepeatSubmitToken");

            int global_lang_index = response_2.Content.IndexOf("global_lang");

            if (FormIndex<0 || FormLastIndex<0 || limit_ticket_num_index<0 || SubmitTokenIndex<0 || global_lang_index<0)
            {
                return false;
            }

            StringBuilder passengerForm = new StringBuilder();

            passengerForm.Append(response_2.Content.Substring(FormIndex, FormLastIndex - FormIndex));

            string passengerFormStr = passengerForm.ToString();

            TicketInfoForPassengerForm ticketInfoForPassengerForm = JsonConvert.DeserializeObject<TicketInfoForPassengerForm>
                    (passengerFormStr.Substring(passengerFormStr.IndexOf("=") + 1,
                    passengerFormStr.LastIndexOf("}") - passengerFormStr.IndexOf("=")));

            string OrderRequestDTOStr = response_2.Content.Substring(FormLastIndex, limit_ticket_num_index - FormLastIndex);

            OrderQuestDto orderQuestDTO = JsonConvert.DeserializeObject<OrderQuestDto>(
                OrderRequestDTOStr.Substring(OrderRequestDTOStr.IndexOf("=") + 1,
                    OrderRequestDTOStr.LastIndexOf("}") - OrderRequestDTOStr.IndexOf("=")));

            #endregion

            #region GetPassengerDto

            var client_3 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs");
            var request_3 = new RestRequest(Method.POST);
            request_3.AddHeader("cache-control", "no-cache");
            request_3.AddHeader("Host", "kyfw.12306.cn");
            request_3.AddHeader("Origin", "https://kyfw.12306.cn");
            request_3.AddHeader("Referer", "https://kyfw.12306.cn/otn/confirmPassenger/initDc");
            request_3.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            if (cookieContainer != null && cookieContainer.Count > 0)
            {
                foreach (var item in cookieContainer)
                {
                    request_3.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }
            IRestResponse response_3 = client_3.Execute(request_3);

            CommonResponse<PassengerData> passengerDTOResponse = JsonConvert.DeserializeObject<CommonResponse<PassengerData>>(response_3.Content);

            #endregion

            #region checkOrderInfo
            var client_4 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo");
            var request_4 = new RestRequest(Method.POST);
            request_4.AddHeader("Cache-Control", "no-cache");
            request_4.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request_4.AddHeader("Connection", "true");
            request_4.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            if (cookieContainer != null && cookieContainer.Count > 0)
            {
                foreach (var item in cookieContainer)
                {
                    request_4.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }

            request_4.AddParameter("application/x-www-form-urlencoded; charset=UTF-8",
                $"cancel_flag=2&bed_level_order_num=000000000000000000000000000000&passengerTicketStr={input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N&oldPassengerStr={passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_&tour_flag=dc&randCode=&whatsSelect=1", ParameterType.RequestBody);

            IRestResponse response_4 = client_4.Execute(request_4);

            CommonResponse<CheckOrderResponseData> checkOrderResponse = JsonConvert.DeserializeObject<CommonResponse<CheckOrderResponseData>>(response_4.Content);
            #endregion

            #region getQueueCount
            var client_5 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount");
            var request_5 = new RestRequest(Method.POST);
            request_5.AddHeader("Cache-Control", "no-cache");
            request_5.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request_5.AddHeader("Connection", "true");
            request_5.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");
            if (cookieContainer != null && cookieContainer.Count > 0)
            {
                foreach (var item in cookieContainer)
                {
                    request_5.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }

            string dateStr = input.LeftDateJs.Replace(":", "%3A").Replace("+", "%2B");

            //  "Tue%20Apr%2030%202019%2000%3A00%3A00%20GMT%2B0800%20(%E4%B8%AD%E5%9B%BD%E6%A0%87%E5%87%86%E6%97%B6%E9%97%B4)" 
            request_5.AddParameter("application/x-www-form-urlencoded; charset=UTF-8 ",
                "train_date=" + dateStr +
                "&train_no=" + orderQuestDTO.Train_no +
                "&stationTrainCode=" + orderQuestDTO.Station_train_code + 
                "&seatType=O" +
                "&fromStationTelecode=" + orderQuestDTO.From_station_telecode
                + "&toStationTelecode=" + orderQuestDTO.To_station_telecode +
                "&leftTicket=" + ticketInfoForPassengerForm.QueryLeftTicketRequestDTO.YpInfoDetail +
                "&purpose_codes=00&train_location=" + ticketInfoForPassengerForm.Train_location + "&undefined=", ParameterType.RequestBody);


            IRestResponse response_5 = client_5.Execute(request_5);

            CommonResponse<QueueCountResponseData> queueCountResponseData = JsonConvert.DeserializeObject<CommonResponse<QueueCountResponseData>>
                (response_5.Content);

            #endregion

            #region confirmSingleForQueue
            var client_6 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue");
            var request_6 = new RestRequest(Method.POST);
            request_6.AddHeader("Cache-Control", "no-cache");
            request_6.AddHeader("Connection", "true");
            request_6.AddHeader("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            request_6.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

            if (cookieContainer.Count > 0)
            {
                foreach (var item in cookieContainer)
                {
                    request_6.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }


            if (checkOrderResponse.Data.CanChooseSeats=="Y" && !checkOrderResponse.Data.Choose_Seats.Contains(input.SeatType))
            {
                input.SeatType = checkOrderResponse.Data.Choose_Seats[0].ToString();
            }

            request_6.AddParameter("application/x-www-form-urlencoded; charset=UTF-8",
            $"passengerTicketStr={input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N&oldPassengerStr={passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_&randCode=&purpose_codes=00&key_check_isChange={ticketInfoForPassengerForm.Key_check_isChange}&leftTicketStr={ticketInfoForPassengerForm.LeftTicketStr}&train_location={ticketInfoForPassengerForm.Train_location}&choose_seats=&seatDetailType=000&whatsSelect=1&roomType=00&dwAll=N", ParameterType.RequestBody);

            IRestResponse response_6 = client_6.Execute(request_6);

            CommonResponse<ConfirmSingleResponseData> confirmSingleForQueueResponse = JsonConvert.DeserializeObject<CommonResponse<ConfirmSingleResponseData>>(response_6.Content);

            if (confirmSingleForQueueResponse.Data.SubmitStatus)
            {
                EmailHelper.Send("1126818689@qq.com", "订票成功", "恭喜订票成功,登录你的12306完成支付");
            }

            #endregion

            return false;
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

            #region logdevice回调函数
            var client_0 = new RestClient("https://kyfw.12306.cn/otn/HttpZF/logdevice?algID=ozy7Gbfya4&hashCode=WfH7dJnFd1fsVPyp7w5waSpXKQX_Mz9Eg7kEMgvMQ6I&FMQw=0&q4f3=zh-CN&VySQ=FGEbgvFhJ2TiuUR5kdZvKllDSsfJHQJZ&VPIf=1&custID=133&VEek=unknown&dzuS=0&yD16=0&EOQP=4902a61a235fbb59700072139347967d&lEnu=3232235642&jp76=52d67b2a5aa5e031084733d5006cc664&hAqN=Win32&platform=WEB&ks0Q=d22ca0b81584fbea62237b14bd04c866&TeRS=1040x1920&tOHY=24xx1080x1920&Fvje=i1l1o1s1&q5aJ=-8&wNLf=99115dfb07133750ba677d055874de87&0aew=Mozilla/5.0%20(Windows%20NT%2010.0;%20Win64;%20x64)%20AppleWebKit/537.36%20(KHTML,%20like%20Gecko)%20Chrome/71.0.3578.98%20Safari/537.36&E3gR=556abc357c181c7e407b7183cd421c5c&timestamp=" + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            var request_0 = new RestRequest(Method.GET);
            request_0.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response_0 = client_0.Execute(request_0);

            string resJsonStr = response_0.Content.Substring(response_0.Content.IndexOf("'") + 1, response_0.Content.LastIndexOf("'") - response_0.Content.IndexOf("'") - 1);

            CallBackResponse callBackFunction = JsonConvert.DeserializeObject<CallBackResponse>(resJsonStr);
            cookieContainer.Add(new RestResponseCookie {
                Name = "RAIL_DEVICEID",
                Value = callBackFunction.Dfp
            });
            cookieContainer.Add(new RestResponseCookie
            {
                Name = "RAIL_EXPIRATION",
                Value = callBackFunction.Exp
            });

            #endregion

            #region 验证码校验
            var client = new RestClient("https://kyfw.12306.cn/passport/captcha/captcha-check?answer=" + answer + "&rand=sjran&login_site=E");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");
            request.AddHeader("Connection", "true");
            if (cookieContainer.Count > 0)
            {
                foreach (var cookie in cookieContainer)
                {
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
            request_2.AddHeader("Accept", "application/json");
            request_2.AddHeader("cache-control", "no-cache");
            request_2.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request_2.AddHeader("Connection", "true");
            request_2.AddParameter("application/x-www-form-urlencoded", "username=" + input.UserName + "&password=" + input.Password + "&answer=" + loginAnswer + "&appid=otn&undefined=",
                ParameterType.RequestBody);
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0");

            if (cookieContainer.Count > 0)
            {
                foreach (var cookie in cookieContainer)
                {
                    request_2.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                }
            }

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

            CacheHelper.SetCache(input.UserName + "_loginStatus", cookieContainer);

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
