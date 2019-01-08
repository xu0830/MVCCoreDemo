using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MVCCoreDemo.Models
{
    public class LoginModelInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginModelOutput
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public string Url { get; set; }
    }
}
