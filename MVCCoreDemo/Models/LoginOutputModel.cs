using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MVCCoreDemo.Models
{
    /// <summary>
    /// 登录接口输入model
    /// </summary>
    public class LoginInputModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }

    /// <summary>
    /// 登录接口输出model
    /// </summary>
    public class LoginOutputModel
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public string Url { get; set; }
    }
}
