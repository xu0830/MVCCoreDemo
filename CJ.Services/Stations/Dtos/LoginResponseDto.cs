using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    public class LoginResponseDto
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Result_message { get; set; }

        /// <summary>
        /// 0: 登录成功
        /// 5: 验证码校验失败
        /// </summary>
        public int Result_code { get; set; }


    }
}
