using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    public class CaptchaImageDto
    {
        public string Result_message { get; set; }
        public string Result_code { get; set; }
        public string Image { get; set; }
    }
}
