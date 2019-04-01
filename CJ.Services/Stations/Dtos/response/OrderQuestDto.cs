using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos.response
{
    public class OrderQuestDto
    {
        public int Adult_num { get; set; }

        public object Apply_order_no { get; set; }

        public object Bed_level_order_num { get; set; }

        public object Bureau_code { get; set; }

        public object Cancel_flag { get; set; }

        public object Card_num { get; set; }

        public object Channel { get; set; }

        public int Child_num { get; set; }

        public object Choose_seat { get; set; }

        public int Disability_num { get; set; }

        public Train_dateModel End_time { get; set; }

        public int Exchange_train_flag { get; set; }

        public string From_station_name { get; set; }

        public string From_station_telecode { get; set; }

        public object Get_ticket_pass { get; set; }

        public string Id_mode { get; set; }

        public object IsShowPassCode { get; set; }

        public object LeftTicketGenTime { get; set; }

        public object Order_date { get; set; }

        public object PassengerFlag { get; set; }

        public object RealleftTicket { get; set; }

        public object ReqIpAddress { get; set; }

        public object ReqTimeLeftStr { get; set; }

        public string Reserve_flag { get; set; }

        public object Seat_detail_type_code { get; set; }

        public object Seat_type_code { get; set; }

        public object Sequence_no { get; set; }

        public Train_dateModel Start_time { get; set; }

        public object Start_time_str { get; set; }

        public string Station_train_code { get; set; }

        public int Student_num { get; set; }

        public int Ticket_num { get; set; }

        public object Ticket_type_order_num { get; set; }

        public string To_station_name { get; set; }

        public string To_station_telecode { get; set; }

        public string Tour_flag { get; set; }

        public object TrainCodeText { get; set; }

        public Train_dateModel Train_date { get; set; }

        public object Train_date_str { get; set; }

        public object Train_location { get; set; }

        public string Train_no { get; set; }

        public object Train_order { get; set; }

        public object VarStr { get; set; }
    }

    public class Train_dateModel
    {
        public int Date { get; set; }

        public int Day { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public int Month { get; set; }

        public int Seconds { get; set; }

        public string Time { get; set; }

        public int TimezoneOffset { get; set; }

        public int Year { get; set; }
    }


}
