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
        ValidatePicOutput GetValidatePicUrl();

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <returns></returns>
        LoginServiceDto ValidateLogin(StationServiceInput input);

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        OrderOutput SubmitOrder(TicketTaskDto dto);

        bool StopTask(string userName);

        /// <summary>
        /// 车票查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<TicketDto> TicketQuery(StationServiceInput input);

        PassengerData GetPassengerDto(string userName);
    }
}
