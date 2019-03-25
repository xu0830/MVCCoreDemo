using CJ.Infrastructure.Json;
using CJ.Services.Stations.Dtos;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations
{
    public class StationService : IStationService
    {
        /// <summary>
        /// 车票查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<TicketDto> TicketQuery(StationServiceInput input)
        {
            var client_8 = new RestClient($"https://kyfw.12306.cn/otn/leftTicket/query?leftTicketDTO.train_date={input.Train_date}&leftTicketDTO.from_station={input.From_station_code}&leftTicketDTO.to_station=${input.To_station_code}&purpose_codes={input.Purpose_codes}");
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
                    Gg_num = props[20],
                    Gr_num = props[21],
                    Qt_num = props[22],
                    Rw_num = props[23],
                    Rz_num = props[24],
                    Tz_num = props[25],
                    Wz_num = props[26],
                    Yb_num = props[27],
                    Yw_num = props[28],
                    Yz_num = props[29],
                    Ze_num = props[30],
                    Zy_num = props[31],
                    Swz_num = props[32],
                    Srrb_num = props[33],
                    Yp_ex = props[34],
                    Seat_types = props[35],
                    Exchange_train_flag = props[36],
                    Houbu_train_flag = props[37],
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
