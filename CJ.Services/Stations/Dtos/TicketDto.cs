using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    public class TicketDto
    {
        /// <summary>
        /// 车票密文
        /// </summary>
        public string SecretStr { get; set; }

        /// <summary>
        /// 按钮提示文字
        /// </summary>
        public string ButtonTextInfo { get; set; }

        /// <summary>
        /// 列车编号
        /// </summary>
        public string Train_no { get; set; }

        /// <summary>
        /// 车次
        /// </summary>
        public string Station_train_code { get; set; }

        /// <summary>
        /// 始发站编码
        /// </summary>
        public string Start_station_telecode { get; set; }

        /// <summary>
        /// 终点站编码
        /// </summary>
        public string End_station_telecode { get; set; }

        /// <summary>
        /// 出发站编码
        /// </summary>
        public string From_station_telecode { get; set; }

        /// <summary>
        /// 到达站编码
        /// </summary>
        public string To_station_telecode { get; set; }

        /// <summary>
        /// 出发时间
        /// </summary>
        public string Start_time { get; set; }

        /// <summary>
        /// 到达时间
        /// </summary>
        public string Arrive_time { get; set; }

        /// <summary>
        /// 历时
        /// </summary>
        public string Lishi { get; set; }

        /// <summary>
        /// 是否可以网上购票   "Y" -> true
        /// </summary> 
        public string CanWebBuy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Yp_info { get; set; }

        /// <summary>
        /// 出发日期
        /// </summary>
        public string Start_train_data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Train_seat_feature { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Location_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string From_station_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string To_station_no { get; set; }

        /// <summary>
        /// 是否支持二代身份证进出站    !=0 -> true
        /// </summary>
        public string Is_support_card { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Controlled_train_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Gg_num { get; set; }

        /// <summary>
        /// 高级软卧
        /// </summary>
        public string Gr_num { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public string Qt_num { get; set; }

        /// <summary>
        /// 软卧一等座
        /// </summary>
        public string Rw_num { get; set; }

        /// <summary>
        /// 软座
        /// </summary>
        public string Rz_num { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Tz_num { get; set; }

        /// <summary>
        /// 无座数量
        /// </summary>
        public string Wz_num { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Yb_num { get; set; }

        /// <summary>
        /// 硬卧二等卧
        /// </summary>
        public string Yw_num { get; set; }

        /// <summary>
        /// 硬座
        /// </summary>
        public string Yz_num { get; set; }

        /// <summary>
        /// 二等座数量
        /// </summary>
        public string Ze_num { get; set; }

        /// <summary>
        /// 一等座数量
        /// </summary>
        public string Zy_num { get; set; }

        /// <summary>
        /// 商务座数量
        /// </summary>
        public string Swz_num { get; set; }

        /// <summary>
        /// 动卧
        /// </summary>
        public string Srrb_num { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Yp_ex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Seat_types { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Exchange_train_flag { get; set; }

        /// <summary>
        /// 候补
        /// </summary>
        public string Houbu_train_flag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Houbu_seat_limit { get; set; }

        /// <summary>
        /// 出发站名称
        /// </summary>
        public string From_station_name { get; set; }

        /// <summary>
        /// 到达站名称
        /// </summary>
        public string To_station_name { get; set; }
    }
}
