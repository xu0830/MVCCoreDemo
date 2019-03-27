using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CJ.Infrastructure.Cache;
using CJ.Services.Stations;
using CJ.Services.Stations.Dtos;
using Microsoft.AspNetCore.Mvc;
using MVCCoreDemo.Controllers;
using MVCCoreDemo.Models;
using MVCCoreDemo.Models.Station;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCCoreDemo.ApiControllers
{
    [Route("api/[controller]")]
    public class StationController : BaseController
    {
        // GET: api/<controller>
        private readonly IStationService stationService;

        public StationController(IStationService _stationService)
        {
            stationService = _stationService;
        }

        /// <summary>
        /// 获取图片验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost("getValidateImage")]
        public OutputModel GetValidateImage()
        {
            OutputModel response = new OutputModel();
            response.Code = 200;
            response.Result = "success";
            response.Data = stationService.GetValidatePicUrl();
            
            return response;
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("validateLogin")]
        public OutputModel ValidateLogin([FromBody]TicketQueryInput input)
        {
            OutputModel response = new OutputModel();

            var loginServiceDto = stationService.ValidateLogin(new StationServiceInput()
            {
                UserName = input.UserName,
                Password = input.Password,
                PointsData = input.PointsData,
                Token = input.Token
            });

            return new OutputModel {
                Code = loginServiceDto.LoginStatus ? 200 : 204,
                Result = loginServiceDto.Result,
                Data = loginServiceDto.passenger
            };
        }

        // GET api/<controller>/5
        [HttpGet("{userName}")]
        public PassengerResponse Get(string userName)
        {
            return (PassengerResponse)CacheHelper.GetCache("passenger_" + userName);
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {

        }

        /// <summary>
        /// 车票查询接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ticketQuery")]
        public OutputModel TicketQuery([FromBody]TicketQueryInput input)
        {
            OutputModel response = new OutputModel();
            response.Code = 200;
            response.Result = "success";
            response.Data = stationService.TicketQuery(new StationServiceInput()
            {
                From_station_code = input.From_station_code,
                To_station_code = input.To_station_code,
                Purpose_codes = "ADULT",
                Train_date = input.Train_date
            });
            return response;
        }

        //public OutputModel TicketInfo([FromBody]TicketQueryInput input)
        //{
        //    OutputModel response = new OutputModel();
        //    response.Code = 200;
        //    response.Result = "success";
        //    response.Data = stationService.TicketQuery(new StationServiceInput()
        //    {
        //        From_station_code = input.From_station_code,
        //        To_station_code = input.To_station_code,
        //        Purpose_codes = "ADULT",
        //        Train_date = input.Train_date
        //    });
        //    return response;
        //}

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


    }
}
