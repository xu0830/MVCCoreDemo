using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreDemo.Models
{
    /// <summary>
    /// api控制器接口输入Model
    /// </summary>
    public class InputModel
    {
        public string Token { get; set; }
        public string Params { get; set; }
    }
}
