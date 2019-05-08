using CJ.Entities;
using CJ.Infrastructure;
using CJ.Infrastructure.Cache;
using CJ.Infrastructure.EmailHelper;
using CJ.Infrastructure.Encode;
using CJ.Infrastructure.Log;
using CJ.Infrastructure.Repositories;
using CJ.Services.Stations.Dtos;
using CJ.Services.Stations.Dtos.response;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJ.Services.Quartz.Jobs
{
    /// <summary>
    /// 执行订票任务
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class TicketOrderJob : IJob
    {
        private readonly IRepository<TicketTask> ticketTaskRepository = new Repository<TicketTask>(new DefaultDbContext());

        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;//获取Job中的参数

            var taskStr = jobData.GetString("ticketTask");

            var taskRunNum = jobData.GetInt("taskRunNum");

            jobData["taskRunNum"] = ++taskRunNum;

            var input = JsonConvert.DeserializeObject<TicketTaskDto>(taskStr);

            return Task.Run(() =>
            {
                var currentModel = ticketTaskRepository.Get(input.Id);
                if (currentModel.Status == 3)
                {
                    context.Scheduler.DeleteJob(new JobKey(input.UserName));
                    LogHelper.Info($"任务{input.UserName}手动停止");
                }
                else
                {
                    LogHelper.Info($"任务{input.UserName}的第{taskRunNum}次执行");
                    try
                    {
                        var cookieContainer = CacheHelper.GetCache<IList<RestResponseCookie>>(input.UserName + "_loginStatus");

                        if (cookieContainer == null)
                        {
                            throw new Exception("登录状态失效");
                        }

                        foreach (var trainCode in input.TrainCodes)
                        {
                            TicketDto orderTicket = new TicketDto();

                            #region Query
                            var client = new RestClient($"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={input.LeftDate}&leftTicketDTO.from_station={input.LeftStation.Code}&leftTicketDTO.to_station={input.ArriveStation.Code}&purpose_codes=ADULT");
                            var request = new RestRequest(Method.GET);
                            request.AddHeader("cache-control", "no-cache");
                            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

                            IList<RestResponseCookie> cookies = new List<RestResponseCookie>();

                            foreach (var item in cookieContainer)
                            {
                                cookies.Add(new RestResponseCookie()
                                {
                                    Name = item.Name,
                                    Value = item.Value
                                });
                            }

                            cookies.Add(new RestResponseCookie()
                            {
                                Name = "_jc_save_fromDate",
                                Value = input.LeftDate
                            });

                            cookies.Add(new RestResponseCookie()
                            {
                                Name = "_jc_save_fromStation",
                                Value = UnicodeHelper.String2Unicode(input.LeftStation.Name).Replace("\\", "%") + "%2C" + orderTicket.From_station_telecode
                            });

                            cookies.Add(new RestResponseCookie()
                            {
                                Name = "_jc_save_toDate",
                                Value = input.LeftDate
                            });

                            cookies.Add(new RestResponseCookie()
                            {
                                Name = "_jc_save_toStation",
                                Value = UnicodeHelper.String2Unicode(input.ArriveStation.Name).Replace("\\", "%") + "%2C" + input.ArriveStation.Code
                            });

                            cookies.Add(new RestResponseCookie()
                            {
                                Name = "_jc_save_wfdc_flag",
                                Value = "dc"
                            });

                            foreach (var item in cookies)
                            {
                                request.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                            }

                            IRestResponse response = client.Execute(request);

                            TicketsQueryResponse ticketsResponse = null;

                            try
                            {
                                ticketsResponse = JsonConvert.DeserializeObject<TicketsQueryResponse>(response.Content);
                            }
                            catch (Exception)
                            {

                            }


                            if (ticketsResponse == null)
                            {
                                throw new Exception("https://kyfw.12306.cn/otn/leftTicket/query 接口异常" + response.Content);
                            }

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

                                if (ticket.Station_train_code == trainCode)
                                {
                                    orderTicket = ticket;
                                }
                            }
                            #endregion


                            if (orderTicket.CanWebBuy == "Y")
                            {
                                #region submitOrderRequest
                                var client_1 = new RestClient("https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest");
                                var request_1 = new RestRequest(Method.POST);
                                request_1.AddHeader("cache-control", "no-cache");
                                request_1.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                request_1.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

                                

                                foreach (var item in cookies)
                                {
                                    request_1.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                                }

                                request_1.AddParameter("application/x-www-form-urlencoded",
                                    "secretStr=" +
                                    orderTicket.SecretStr +
                                    "&train_date=" + input.LeftDate + "&back_train_date=" + input.LeftDate + "&tour_flag=dc&purpose_codes=ADULT" +
                                    "&query_from_station_name=" + orderTicket.From_station_name + "&query_to_station_name=" + orderTicket.From_station_name,
                                    ParameterType.RequestBody);
                                IRestResponse response_1 = client_1.Execute(request_1);

                                CommonResponse<object> submitOrderRequestResponse = null;
                                try
                                {
                                    submitOrderRequestResponse = JsonConvert.DeserializeObject<CommonResponse<object>>(response_1.Content);
                                }
                                catch (Exception)
                                {
                                }

                                if (submitOrderRequestResponse == null)
                                {
                                    throw new Exception("https://kyfw.12306.cn/otn/leftTicket/submitOrderRequest 接口异常" + response_1.Content);
                                }
                                if (!submitOrderRequestResponse.Status)
                                {
                                    if (submitOrderRequestResponse.Messages[0].ToString().IndexOf("您还有未处理的订单") > -1)
                                    {
                                        EmailHelper.Send(currentModel.NoticeEmail, "订票成功", "订票成功,登录你的12306完成支付");
                                        var model = ticketTaskRepository.Get(input.Id);
                                        model.Status = 1;
                                        ticketTaskRepository.Update(model);
                                        context.Scheduler.Shutdown();
                                        LogHelper.Info("任务执行完成");
                                        break;
                                    }
                                    else
                                    {
                                        LogHelper.Info($"任务{input.UserName}submitOrderRequest提交失败" + response_1.Content);
                                        continue;
                                    }
                                }
                                #endregion

                                #region https://kyfw.12306.cn/otn/confirmPassenger/initDc

                                var client_2 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/initDc");

                                var request_2 = new RestRequest(Method.POST);
                                request_2.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

                                if (cookies != null && cookies.Count > 0)
                                {
                                    foreach (var item in cookies)
                                    {
                                        request_2.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                                    }
                                }

                                IRestResponse response_2 = client_2.Execute(request_2);

                                int init_seatTypes_index = response_2.Content.IndexOf("init_seatTypes");

                                int init_seatTypes_lastIndex = response_2.Content.IndexOf("defaultTicketTypes");

                                int FormIndex = response_2.Content.IndexOf("ticketInfoForPassengerForm");

                                int FormLastIndex = response_2.Content.LastIndexOf("orderRequestDTO");

                                int limit_ticket_num_index = response_2.Content.LastIndexOf("init_limit_ticket_num");

                                int SubmitTokenIndex = response_2.Content.IndexOf("globalRepeatSubmitToken");

                                int global_lang_index = response_2.Content.IndexOf("global_lang");

                                if (init_seatTypes_lastIndex < 0 || init_seatTypes_index < 0 || FormIndex < 0 || FormLastIndex < 0 || limit_ticket_num_index < 0 || SubmitTokenIndex < 0 || global_lang_index < 0)
                                {
                                    throw new Exception("api https://kyfw.12306.cn/otn/confirmPassenger/initDc 接口异常" + response_1.Content + response_2.Content);
                                }

                                string init_seatTypesTemp = response_2.Content.Substring(init_seatTypes_index, init_seatTypes_lastIndex - init_seatTypes_index);

                                string init_seatTypesStr = init_seatTypesTemp.Substring(init_seatTypesTemp.IndexOf("=") + 1,
                                    init_seatTypesTemp.LastIndexOf("]") - init_seatTypesTemp.IndexOf("="));

                                List<Init_seatType> init_SeatTypes = JsonConvert.DeserializeObject<List<Init_seatType>>(init_seatTypesStr);

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

                                if (cookies != null && cookies.Count > 0)
                                {
                                    foreach (var item in cookies)
                                    {
                                        request_3.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                                    }
                                }
                                IRestResponse response_3 = client_3.Execute(request_3);

                                CommonResponse<PassengerData> passengerDTOResponse = null;

                                try
                                {
                                    passengerDTOResponse = JsonConvert.DeserializeObject<CommonResponse<PassengerData>>(response_3.Content);
                                }
                                catch (Exception)
                                {
                                }

                                if (passengerDTOResponse == null || !passengerDTOResponse.Data.IsExist)
                                {
                                    throw new Exception("https://kyfw.12306.cn/otn/confirmPassenger/getPassengerDTOs 接口异常" + response_3.Content);
                                }

                                #endregion

                                #region checkOrderInfo
                                var client_4 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo");
                                var request_4 = new RestRequest(Method.POST);
                                request_4.AddHeader("Cache-Control", "no-cache");
                                request_4.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                                request_4.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

                                if (cookies != null && cookies.Count > 0)
                                {
                                    foreach (var item in cookies)
                                    {
                                        request_4.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                                    }
                                }

                                request_4.AddParameter("application/x-www-form-urlencoded; charset=UTF-8",
                                    $"cancel_flag=2&bed_level_order_num=000000000000000000000000000000&passengerTicketStr={input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N&oldPassengerStr={passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_&tour_flag=dc&randCode=&whatsSelect=1", ParameterType.RequestBody);

                                IRestResponse response_4 = client_4.Execute(request_4);

                                CommonResponse<CheckOrderResponseData> checkOrderResponse = null;

                                try
                                {
                                    checkOrderResponse = JsonConvert.DeserializeObject<CommonResponse<CheckOrderResponseData>>(response_4.Content);
                                }
                                catch (Exception)
                                {

                                }

                                if (checkOrderResponse == null)
                                {
                                    throw new Exception("https://kyfw.12306.cn/otn/confirmPassenger/checkOrderInfo 接口异常" + response_4.Content);
                                }
                                #endregion

                                #region getQueueCount
                                var client_5 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount");
                                var request_5 = new RestRequest(Method.POST);
                                request_5.AddHeader("Cache-Control", "no-cache");
                                request_5.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                                request_5.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");
                                if (cookies != null && cookies.Count > 0)
                                {
                                    foreach (var item in cookies)
                                    {
                                        request_5.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                                    }
                                }

                                string dateStr = input.LeftDateJs.Replace(":", "%3A").Replace("+", "%2B");

                                //  "Tue%20Apr%2030%202019%2000%3A00%3A00%20GMT%2B0800%20(%E4%B8%AD%E5%9B%BD%E6%A0%87%E5%87%86%E6%97%B6%E9%97%B4)" 
                                request_5.AddParameter("application/x-www-form-urlencoded; charset=UTF-8 ",
                                    "train_date=" + dateStr +
                                    "&train_no=" + orderTicket.Train_no +
                                    "&stationTrainCode=" + orderTicket.Station_train_code +
                                    "&seatType=" + input.SeatType +
                                    "&fromStationTelecode=" + orderTicket.From_station_telecode
                                    + "&toStationTelecode=" + orderTicket.To_station_telecode +
                                    "&leftTicket=" + ticketInfoForPassengerForm.QueryLeftTicketRequestDTO.YpInfoDetail +
                                    "&purpose_codes=00&train_location=" + ticketInfoForPassengerForm.Train_location + "&undefined=", ParameterType.RequestBody);


                                IRestResponse response_5 = client_5.Execute(request_5);


                                CommonResponse<QueueCountResponseData> queueCountResponseData = null;
                                try
                                {
                                    queueCountResponseData = JsonConvert.DeserializeObject<CommonResponse<QueueCountResponseData>>(response_5.Content);
                                }
                                catch (Exception)
                                {

                                }

                                if (queueCountResponseData == null)
                                {
                                    throw new Exception("https://kyfw.12306.cn/otn/confirmPassenger/getQueueCount 接口异常" + response_5.Content);
                                }

                                #endregion

                                #region confirmSingleForQueue
                                var client_6 = new RestClient("https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue");
                                var request_6 = new RestRequest(Method.POST);
                                request_6.AddHeader("Accept", "application/json");
                                request_6.AddHeader("Cache-Control", "no-cache");
                                request_6.AddHeader("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                                request_6.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0");

                                if (cookies.Count > 0)
                                {
                                    foreach (var item in cookies)
                                    {
                                        request_6.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                                    }
                                }

                                string passengerTicketStrEncode = Uri.EscapeDataString($"{input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N");
                                string oldPassengerStrEncode = Uri.EscapeDataString($"{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_");

                                var submitForm = $"passengerTicketStr={input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N" +
                                $"&oldPassengerStr={passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_" +
                                $"&randCode=&purpose_codes=00&" +
                                $"key_check_isChange={ticketInfoForPassengerForm.Key_check_isChange}" +
                                $"&leftTicketStr={ticketInfoForPassengerForm.LeftTicketStr}&train_location={ticketInfoForPassengerForm.Train_location}" +
                                $"&choose_seats=&seatDetailType=000&whatsSelect=1&roomType=00&dwAll=N";

                                request_6.AddParameter("application/x-www-form-urlencoded; charset=UTF-8 ",
                                $"passengerTicketStr={input.SeatType},0,1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},{passengerDTOResponse.Data.Normal_passengers[0].Mobile_no},N" +
                                $"&oldPassengerStr={passengerDTOResponse.Data.Normal_passengers[0].Passenger_name},1,{passengerDTOResponse.Data.Normal_passengers[0].Passenger_id_no},1_" +
                                $"&randCode=&purpose_codes=00&" +
                                $"key_check_isChange={ticketInfoForPassengerForm.Key_check_isChange}" +
                                $"&leftTicketStr={ticketInfoForPassengerForm.LeftTicketStr}&train_location={ticketInfoForPassengerForm.Train_location}" +
                                $"&choose_seats=&seatDetailType=000&whatsSelect=1&roomType=00&dwAll=N", ParameterType.RequestBody);

                                IRestResponse response_6 = client_6.Execute(request_6);
                                try
                                {
                                    CommonResponse<ConfirmSingleResponseData> confirmSingleForQueueResponse = JsonConvert.DeserializeObject<CommonResponse<ConfirmSingleResponseData>>(response_6.Content);

                                    if (confirmSingleForQueueResponse.Data.SubmitStatus)
                                    {
                                        EmailHelper.Send(currentModel.NoticeEmail, "订票成功", "订票成功,登录你的12306完成支付");
                                        var model = ticketTaskRepository.Get(input.Id);
                                        model.Status = 1;
                                        ticketTaskRepository.Update(model);
                                        context.Scheduler.DeleteJob(new JobKey(input.UserName));
                                        LogHelper.Info("任务执行完成");
                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        CommonResponse<string> submitStatus = JsonConvert.DeserializeObject<CommonResponse<string>>(response_6.Content);
                                    }
                                    catch (Exception)
                                    {
                                        throw new Exception("https://kyfw.12306.cn/otn/confirmPassenger/confirmSingleForQueue 接口异常" + response_6.Content + submitForm);
                                    }
                                }

                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //var model = ticketTaskRepository.Get(input.Id);
                        //model.Status = 2;
                        //ticketTaskRepository.Update(model);
                        //context.Scheduler.Shutdown();
                        LogHelper.Error($"任务{input.UserName}执行异常 异常信息: " + ex.Message);
                    }
                }
            });
        }
    }
}
