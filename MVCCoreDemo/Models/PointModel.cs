using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreDemo.Models
{
    /// <summary>
    /// 拼图验证输入Model
    /// </summary>
    public class PointModel
    {
        public string Token { get; set; }
        public int x { get; set; }
    }
}
