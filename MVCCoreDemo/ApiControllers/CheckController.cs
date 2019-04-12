using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CJ.Infrastructure;
using CJ.Infrastructure.Cache;
using CJ.Infrastructure.EmailHelper;
using CJ.Infrastructure.Log;
using CJ.Services.Roles;
using CJ.Services.Users;
using CJ.Services.Users.Dtos;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MVCCoreDemo.Controllers;
using MVCCoreDemo.Models;
using Newtonsoft.Json;

namespace MVCCoreDemo.ApiControllers
{
    [Route("api/[controller]")]
    public class CheckController : Controller
    {
        private IUserService userService;

        private IRoleService roleService;

        public CheckController(IUserService _userService, IRoleService _roleService)
        {
            roleService = _roleService;
            userService = _userService;
        }

        /// <summary>
        /// 网站验证数据
        /// </summary>
        /// <returns></returns>
        // GET api/<controller>/5
        [HttpGet]
        public OutputModel Get()
        {
            OutputModel outputModel = new OutputModel();
            var origins = WebConfig.CorsOrigins;

            int entireX = 240;
            int entireY = 140;
            int radius = 7;
            int side = 14;
            Random rd = new Random();
            int sideLength = (int)(2 * side + 0.5 * radius * (4 + 1.8));
            int maxX = entireX - sideLength + 1;
            int minX = 2 * sideLength + 1;

            int maxY = entireY - sideLength + 1;
            int minY = (int)(radius + 0.5 * 1.8 * radius) + 1;

            int x = rd.Next(minX, maxX);
            int y = rd.Next(minY, maxY);

            outputModel.Code = 200;
            outputModel.Result = "success";
            string picGuid = Guid.NewGuid().ToString();

            outputModel.Data = new
            {
                Token = picGuid,
                Point = new
                {
                    x,
                    y
                },
                Msg = WebConfig.CorsOrigins
            };

            CacheHelper.SetCache(picGuid, x, DateTime.Now + TimeSpan.FromMinutes(1));

            return outputModel;
        }

        /// <summary>
        /// 验证码校验接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // POST api/<controller>
        [HttpPost]
        public OutputModel Post([FromBody]InputModel input)
        {
            OutputModel response = new OutputModel();
           
            string inputData = RSAHelper.Decrypt(WebConfig.PrivateKey, input.Params);

            PointModel userPoint = JsonConvert.DeserializeObject<PointModel>(inputData);

            //  参数错误
            if (userPoint == null)
            {
                response.Result = "params invalid";
                response.Code = 403;
                return response;
            }

            //  验证码 cache丢失
            object currentX = CacheHelper.GetCache(userPoint.Token);

            if (currentX == null)
            {
                response.Result = "Cache lost";
                response.Code = 404;
                return response;
            }

            if (userPoint.x <= (int)currentX + 1 && userPoint.x >= (int)currentX - 1)
            {
                response.Code = 200;
                response.Result = "success";
            }

            return response;
        }


        [HttpPost("getUserSession")]
        public OutputModel GetUserSession()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString() ?? "";
            var userDto = userService.IsLogin(token);

            if (userDto != null)
            {
                return new OutputModel()
                {
                    Code = 200,
                    Result = "success",
                    Data = new
                    {
                        userId = userDto.Id,
                        userName = userDto.NickName
                    }
                };
            }

            return new OutputModel()
            {
                Code = 204,
                Result = "fail",
                Data = new
                {
                    
                }
            };

        }

        /// <summary>
        /// 登录请求接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public OutputModel Login([FromBody]InputModel input)
        {
            OutputModel response = new OutputModel();

            string inputData = RSAHelper.Decrypt(WebConfig.PrivateKey, input.Params);

            LoginInputModel user = JsonConvert.DeserializeObject<LoginInputModel>(inputData);

            //  参数错误
            if (user == null)
            {
                response.Result = "params invalid";
                response.Code = 403;
                return response;
            }

            UserDto userDto = new UserDto() {
                UserName = user.UserName,
                Password = MD5Encrypt.Getmd5(user.Password)
            };

            var output = userService.Login(userDto);
            
            response.Code = output.Flag? 200 : 204;
            response.Result = output.Msg;
            response.Data = output.Token;

            return response;
        }

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        [HttpPost("addUser")]
        public bool AddUser()
        {
            return true;
        }
    }
}