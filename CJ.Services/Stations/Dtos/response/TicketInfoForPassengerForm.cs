using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos.response
{
    public class TicketInfoForPassengerForm
    {
        public List<CardType> CardTypes { get; set; }

        public int IsAsync { get; set; }

        public string Key_check_isChange { get; set; }

        public List<string> LeftDetails { get; set; }

        public string LeftTicketStr { get; set; }

        public LimitBuySeatTicketModel LimitBuySeatTicketDTO { get; set; }

        public int MaxTicketNum { get; set; }

        public OrderRequestModel OrderRequestDTO { get; set; }

        public string Purpose_codes { get; set; }

        public QueryLeftTicketRequestModel QueryLeftTicketRequestDTO { get; set; }

        public string Tour_flag { get; set; }

        public string Train_location { get; set; }
    }

    public class LimitBuySeatTicketModel
    {

    }

    public class OrderRequestModel
    {

    }

    public class CardType
    {
        public object End_station_time { get; set; }

        public object End_time { get; set; }

        public string Id { get; set; }

        public object Start_station_name { get; set; }

        public object Start_time { get; set; }

        public string Value { get; set; }
    }

    public class QueryLeftTicketRequestModel
    {
        public string Arrive_time { get; set; }

        public string Bigger20 { get; set; }

        public int Exchange_train_flag { get; set; }

        public string From_station { get; set; }

        public string From_station_name { get; set; }

        public string From_staion_no { get; set; }

        public string Lishi { get; set; }

        public object Login_id { get; set; }

        public object Login_mode { get; set; }

        public object Login_site { get; set; }

        public string Purpose_code { get; set; }

        public object Query_type { get; set; }

        public object SeatTypeAndNum { get; set; }

        public string Seat_Types { get; set; }

        public string Start_time { get; set; }

        public object Start_time_begin { get; set; }

        public object Start_time_end { get; set; }

        public string Station_train_code { get; set; }

        public object Ticket_type { get; set; }

        public string To_station { get; set; }

        public string To_station_name { get; set; }

        public string To_station_no { get; set; }

        public string Train_date { get; set; }

        public object Train_flag { get; set; }

        public object Train_headers { get; set; }

        public string Train_no { get; set; }

        public bool UseMasterPool { get; set; }

        public bool UseWB10LimitTime { get; set; }

        public bool UsingGemfireCache { get; set; }

        public string YpInfoDetail { get; set; }
    }
}
