using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    public class LocationModel
    {
        public string MainLocation { get; set; }
        public List<string> SelectedLocation { get; set; }
        //public DewaOfficeLocation OfficeLocations { get; set; }

        public string ToJson()
        {
            string json = "";

            if (this.SelectedLocation != null && this.SelectedLocation.Count > 0)
            {
                json = string.Join(",", this.SelectedLocation.Select(x => string.Format("'{0}'", x)).ToList());
            }
            return "[" + json + "]";
        }
    }
}