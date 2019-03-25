using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreDemo.Models.Station
{
    public class TicketQueryInput
    {
        /// <summary>
        /// 出发日期
        /// </summary>
        public string Train_date { get; set; }

        /// <summary>
        /// 出发车站
        /// </summary>
        public string From_station_code { get; set; }

        /// <summary>
        /// 到达车站
        /// </summary>
        public string To_station_code { get; set; }

        /// <summary>
        /// 车票类型 
        ///     成人票、学生票
        /// </summary>
        public string Purpose_codes { get; set; }
    }
}
