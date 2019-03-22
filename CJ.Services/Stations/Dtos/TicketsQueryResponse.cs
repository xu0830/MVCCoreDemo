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

        public object Map { get; set; }

        public List<string> Result { get; set; }
    }
}
