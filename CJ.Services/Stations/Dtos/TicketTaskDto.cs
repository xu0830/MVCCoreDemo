using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    public class TicketTaskDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public Station LeftStation { get; set; }

        public Station ArriveStation { get; set; }

        public string LeftDate { get; set; }

        public string LeftDateJs { get; set; }

        public List<string> TrainCodes { get; set; }

        public string SeatType { get; set; }

        public DateTime CreatedTime { get; set; }

        public string PassengerToken { get; set; }

        public int Status { get; set; }

        public string NoticeEmail { get; set; }
    }

    public class Station
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
