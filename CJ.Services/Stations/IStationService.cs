using CJ.Services.Stations.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations
{
    public interface IStationService
    {
        /// <summary>
        /// 车票查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<TicketDto> TicketQuery(StationServiceInput input);
    }
}
