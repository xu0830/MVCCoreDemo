using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    /// <summary>
    /// 图片验证码接口Response
    /// </summary>
    public class ImgValidateResponse
    {
        public string Result_message { get; set; }

        public int Result_code { get; set; }
    }
}
