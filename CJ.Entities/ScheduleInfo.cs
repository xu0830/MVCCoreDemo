using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Entities
{
    public class ScheduleInfo: IEntity
    {
        public int Id { get; set; }

        public string JobGroup { get; set; }

        public string JobName { get; set; }

        public int RunStatus { get; set; }

        public string CromExpress { get; set; }

        public DateTime StartRunTime { get; set; }

        public DateTime EndRunTime { get; set; }

        public DateTime NextRunTime { get; set; }

        public string Token { get; set; }

        public string AppID { get; set; }
    }
}
