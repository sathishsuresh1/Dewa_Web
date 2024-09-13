using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.LocationDataPoints
{
    public class DataPointsRequest
    {
        public class Root
        {
            public onlinemapinputs onlinemapinputs { get; set; }
        }

        public class onlinemapinputs
        {
            public string vendorid { get; set; }
            public string lang { get; set; }
            public string happinesscenter { get; set; }
            public string paymentlocation { get; set; }
            public string watersupplypoint { get; set; }
            public string evcharger { get; set; }
        }
    }
}
