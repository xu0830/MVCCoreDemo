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

        public string Token { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证码点数据
        /// </summary>
        public List<int> PointsData { get; set; }
    }
}
