using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    public class ValidatePicOutput
    {
        public bool Flag { get; set; }

        public string Token { get; set; }

        public string ImgUrl { get; set; }

        public string Msg { get; set; }
    }
}
