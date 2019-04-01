using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos.response
{
    public class CommonResponse<T>
    {
        public string ValidateMessageShowId { get; set; }

        public bool Status { get; set; }

        public int Httpstatus { get; set; }

        public T Data { get; set; }

        public List<Object> Messages { get; set; }

        public object ValidateMessages { get; set; }
    }

    public class CheckOrderResponseData
    {
        public string IfShowPassCode { get; set; }

        public string CanChooseBeds { get; set; }

        public string CanChooseSeats { get; set; }

        public string Choose_Seats { get; set; }

        public string IsCanChooseMid { get; set; }

        public int IfShowPassCodeTime { get; set; }

        public bool SubmitStatus { get; set; }

        public string SmokeStr { get; set; }
    }

    public class QueueCountResponseData
    {
        public int Count { get; set; }

        public string Ticket { get; set; }

        public bool Op_2 { get; set; }

        public int CountT { get; set; }

        public bool Op_1 { get; set; }
    }

    public class ConfirmSingleResponseData
    {
        public bool SubmitStatus { get; set; }
    }
}
