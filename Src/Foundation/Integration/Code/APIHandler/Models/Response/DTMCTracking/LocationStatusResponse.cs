using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCTracking
{
    public class LocationStatusResponse
    {
        public string description { get; set; }
        public string responsecode { get; set; }
        public LocationDetail notiflocation { get; set; }
    }

    public class LocationDetail
    {
        public string geolatitude { get; set; }
        public string geolongitude { get; set; }
        public string guid { get; set; }
        public string timestamp { get; set; }
    }
}
