using CJ.Services.Stations.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations
{
    public interface IStationService
    {
        /// <summary>
        /// 获取验证码图片URL
        /// </summary>
        /// <returns></returns>
        object GetValidatePicUrl();

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <returns></returns>
        LoginServiceDto ValidateLogin(StationServiceInput input);

        /// <summary>
        /// 车票查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<TicketDto> TicketQuery(StationServiceInput input);

        void GetPassengerDto();
    }
}
