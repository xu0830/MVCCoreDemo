using CJ.Infrastructure;
using CJ.Infrastructure.Cache;
using CJ.Infrastructure.Json;
using CJ.Services.Stations.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CJ.Services.Stations
{
    public class StationService : IStationService
    {
        /// <summary>
        /// 获取图片验证码的URL
        /// </summary>
        /// <returns></returns>
        public object GetValidatePicUrl()
        {
            var client = new RestClient("https://kyfw.12306.cn/passport/captcha/captcha-image64");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "328540e8-54d7-4061-9e1b-ec284dbd0352");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            IRestResponse response = client.Execute(request);
            CaptchaImageDto captchaImage = JsonConvert.DeserializeObject<CaptchaImageDto>(response.Content);

            StringBuilder sb = new StringBuilder();

            sb.Append("data:image/jpg;base64,");

            sb.Append(captchaImage.Image);

            string token = Guid.NewGuid().ToString();
            CacheHelper.SetCache(token, response.Cookies, DateTime.Now.AddMinutes(3));

            return new {
                token,
                ImgUrl = sb.ToString()
            };
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

            IList< RestResponseCookie> cookieContainer = (IList<RestResponseCookie>) CacheHelper.GetCache(input.Token);

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
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Connection", "true");

            if (cookieContainer.Count > 0)
            {
                foreach (var cookie in cookieContainer)
                {
                    request.AddParameter(cookie.Name, cookie.Value, ParameterType.Cookie);
                }
            }

            IRestResponse response = client.Execute(request);
            #endregion

            #region 登录接口
            var loginAnswer = answer.Replace(",", "%2C");
            var client_2 = new RestClient("https://kyfw.12306.cn/passport/web/login");
            var request_2 = new RestRequest(Method.POST);
            request_2.AddHeader("Accept", "application/json");
            request_2.AddHeader("cache-control", "no-cache");
            request_2.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            request_2.AddHeader("Connection", "true");
            request_2.AddParameter("", $"username={input.UserName}&password={input.Password}&answer={answer}&appid=otn",
                ParameterType.RequestBody);

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
            catch (Exception ex)
            {

            }

            if (loginResponseDto == null)
            {
                return new LoginServiceDto
                {
                    LoginStatus = false,
                    Result = "密码错误"
                };
            }
            else if(loginResponseDto.Result_code != 0)
            {
                return new LoginServiceDto
                {
                    LoginStatus = false,
                    Result = "验证码校验失败"
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

            return new LoginServiceDto
            {
                LoginStatus = true,
                Result = "登录成功"
            };
        } 

        /// <summary>
        /// 车票查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<TicketDto> TicketQuery(StationServiceInput input)
        {
            string queryUrl = $"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={input.Train_date}&leftTicketDTO.from_station={input.From_station_code}&leftTicketDTO.to_station={input.To_station_code}&purpose_codes={input.Purpose_codes}";
            var client_8 = new RestClient($"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={input.Train_date}&leftTicketDTO.from_station={input.From_station_code}&leftTicketDTO.to_station={input.To_station_code}&purpose_codes={input.Purpose_codes}");
            var request_8 = new RestRequest(Method.GET);
            request_8.AddHeader("cache-control", "no-cache");
            IRestResponse response_8 = client_8.Execute(request_8);

            TicketsQueryResponse ticketsResponse = JsonConvert.DeserializeObject<TicketsQueryResponse>(response_8.Content);

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
                //To_station_name = props[];
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
