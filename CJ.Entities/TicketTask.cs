using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Entities
{
    public class TicketTask: IEntity
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string LeftStation { get; set; }

        public string ArriveStation { get; set; }

        public string LeftDate { get; set; }

        public string TrainCode { get; set; }

        public string SeatType { get; set; }

        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 任务状态
        ///     0: 进行中
        ///     1: 完成
        ///     2: 失败
        /// </summary>
        public int Status { get; set; }

        public string NoticeEmail { get; set; }
    }
}
