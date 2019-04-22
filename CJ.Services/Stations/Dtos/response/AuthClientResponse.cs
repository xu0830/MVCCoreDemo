using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos.response
{
    public class AuthClientResponse
    {
        public string Apptk { get; set; }
        
        public int Result_code { get; set; }

        public string Result_message { get; set; }

        public string Username { get; set; }
    }
}
