using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    public class TkResponseModel
    {
        public string Result_message { get; set; }

        public string Result_code { get; set; }

        public string Apptk { get; set; }

        public string Newapptk { get; set; }
    }
}
