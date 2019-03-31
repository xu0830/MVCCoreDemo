using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    /// <summary>
    /// Jsonp回调函数Response
    /// </summary>
    public class CallBackResponse
    {
        public string Exp { get; set; }

        public string Dfp { get; set; }
    }
}
