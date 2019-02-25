using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCoreDemo.Models
{
    /// <summary>
    /// api控制器接口输出Model
    /// </summary>
    public class OutputModel
    {
        public string Result { get; set; }
        public object Data { get; set; }
        public int Code { get; set; }
    }
}
