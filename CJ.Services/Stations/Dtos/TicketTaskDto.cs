using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    public class TicketTaskDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string LeftStation { get; set; }

        public string ArriveStation { get; set; }

        public string LeftDate { get; set; }

        public string TrainCode { get; set; }

        public string SeatType { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
