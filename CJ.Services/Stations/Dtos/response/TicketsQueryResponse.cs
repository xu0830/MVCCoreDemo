using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    /// <summary>
    /// 车票查询接口 Response Model
    /// </summary>
    public class TicketsQueryResponse
    {
        public int Httpstatus { get; set; }

        public string Messages { get; set; }
        
        public bool Status { get; set; }

        public TicketModel Data { get; set; }
    }

    public class TicketModel
    {
        public int Flag { get; set; }

        public JObject Map { get; set; }

        public List<string> Result { get; set; }
    }

    public class TrainInfo
    {
        public string Start_station_name { get; set; }

        public string Arrive_time { get; set; }

        public string Station_train_code { get; set; }

        public string Station_name { get; set; }

        public string Train_class_name { get; set; }

        public int Service_type { get; set; }

        public string Start_time { get; set; }

        public string Stopover_time { get; set; }

        public string End_station_name { get; set; }

        public string Station_no { get; set; }

        public bool IsEnabled { get; set; }
    }
}
