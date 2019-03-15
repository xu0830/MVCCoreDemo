using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations
{
    interface IStationService
    {
        string GetCode(string StationName);
    }
}
