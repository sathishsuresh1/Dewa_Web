using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.LocationDataPoints
{
    public class DataPointsResponse
    {
        public List<Chargerlist> chargerlist { get; set; }
        public string description { get; set; }
        public List<Happinesscenterlist> happinesscenterlist { get; set; }
        public List<Paymentlocationlist> paymentlocationlist { get; set; }
        public string responsecode { get; set; }
        public List<Watersupplylist> watersupplylist { get; set; }
    }

    public class Chargerlist
    {
        [JsonProperty("additionalImage")]
        public string additionalImage { get; set; }

        [JsonProperty("additionalText1")]
        public string additionalText1 { get; set; }

        [JsonProperty("chargetype")]
        public string chargetype { get; set; }

        [JsonProperty("classification")]
        public object classification { get; set; }

        [JsonProperty("connectors")]
        public string connectors { get; set; }

        [JsonProperty("country")]
        public string ccode { get; set; }

        [JsonProperty("devicestatus")]
        public string status { get; set; }

        [JsonProperty("dstatus")]
        public string dstatus { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("evphone")]
        public string evphone { get; set; }

        [JsonProperty("fimage")]
        public string fimage { get; set; }

        [JsonProperty("hubeleonid")]
        public string hubeleonid { get; set; }

        [JsonProperty("idno")]
        public string id { get; set; }

        [JsonProperty("iimage")]
        public string iimage { get; set; }

        [JsonProperty("latitude")]
        public string latitude { get; set; }

        [JsonProperty("locationAddress")]
        public string address { get; set; }

        [JsonProperty("locationName")]
        public string name { get; set; }

        [JsonProperty("longitude")]
        public string longitude { get; set; }

        [JsonProperty("pimage")]
        public string pimage { get; set; }

        [JsonProperty("region")]
        public string city { get; set; }

        [JsonProperty("telephoneno")]
        public string phone { get; set; }

        [JsonProperty("website")]
        public string web { get; set; }

        [JsonProperty("workinghours")]
        public string whrs { get; set; }
    }
    public class Happinesscenterlist
    {
        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("addressdetails")]
        public string addressdetails { get; set; }

        [JsonProperty("addressline1")]
        public string addressline1 { get; set; }

        [JsonProperty("businesscard")]
        public string businesscardlink { get; set; }

        [JsonProperty("callcenternumber")]
        public string callcenternumber { get; set; }

        [JsonProperty("city")]
        public string city { get; set; }

        [JsonProperty("country")]
        public string countrycode { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("emergencynumber")]
        public string emergencynumber { get; set; }

        [JsonProperty("idno")]
        public string id { get; set; }

        [JsonProperty("imagelink")]
        public string iimage { get; set; }

        [JsonProperty("landmark")]
        public string landmark { get; set; }

        [JsonProperty("lang")]
        public string lang { get; set; }

        [JsonProperty("latitude")]
        public string latitude { get; set; }

        [JsonProperty("longitude")]
        public string longitude { get; set; }

        [JsonProperty("makaninumber")]
        public string makaninumber { get; set; }

        [JsonProperty("mapimage")]
        public string pimage { get; set; }

        [JsonProperty("nearestmetro")]
        public string nearestmetro { get; set; }

        [JsonProperty("office")]
        public string code { get; set; }

        [JsonProperty("officeno")]
        public string phone { get; set; }

        [JsonProperty("pobox")]
        public string zipcode { get; set; }

        [JsonProperty("servicelist")]
        public List<Servicelist> servicelist { get; set; }

        [JsonProperty("title")]
        public string name { get; set; }

        [JsonProperty("website")]
        public string website { get; set; }

        [JsonProperty("workinghours")]
        public string whrs { get; set; }
    }
    public class Paymentlocationlist
    {
        [JsonProperty("address")]
        public string name { get; set; }

        [JsonProperty("category")]
        public string catg { get; set; }

        [JsonProperty("ccno")]
        public string phone { get; set; }

        [JsonProperty("city")]
        public string city { get; set; }

        [JsonProperty("country")]
        public string ccode { get; set; }

        [JsonProperty("fimage")]
        public string fimage { get; set; }

        [JsonProperty("idno")]
        public string id { get; set; }

        [JsonProperty("iimage")]
        public string iimage { get; set; }

        [JsonProperty("lang")]
        public string lang { get; set; }

        [JsonProperty("latitude")]
        public string latitude { get; set; }

        [JsonProperty("longitude")]
        public string longitude { get; set; }

        [JsonProperty("pimage")]
        public string pimage { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("website")]
        public string web { get; set; }

        [JsonProperty("wname")]
        public string wname { get; set; }

        [JsonProperty("workhours")]
        public string whrs { get; set; }
    }
    public class Servicelist
    {
        [JsonProperty("lang")]
        public string lang { get; set; }

        [JsonProperty("service")]
        public string service { get; set; }

        [JsonProperty("slno")]
        public string slno { get; set; }
    }
    public class Watersupplylist
    {
        [JsonProperty("address")]
        public string name { get; set; }

        [JsonProperty("category")]
        public string catg { get; set; }

        [JsonProperty("ccno")]
        public string phone { get; set; }

        [JsonProperty("city")]
        public string city { get; set; }

        [JsonProperty("country")]
        public string ccode { get; set; }

        [JsonProperty("fimage")]
        public string fimage { get; set; }

        [JsonProperty("idno")]
        public string id { get; set; }

        [JsonProperty("iimage")]
        public string iimage { get; set; }

        [JsonProperty("lang")]
        public string lang { get; set; }

        [JsonProperty("latitude")]
        public string latitude { get; set; }

        [JsonProperty("longitude")]
        public string longitude { get; set; }

        [JsonProperty("pimage")]
        public string pimage { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("website")]
        public string web { get; set; }

        [JsonProperty("wname")]
        public string wname { get; set; }

        [JsonProperty("workhours")]
        public string whrs { get; set; }
    }

    public class MapDataPoints
    {
        public List<Chargerlist> FastEVCharge { get; set; }

        public List<Chargerlist> NormalEVCharge { get; set; }

        public List<Paymentlocationlist> PaymentLocation { get; set; }

        public List<Watersupplylist> WaterSupply { get; set; }

        public List<Happinesscenterlist> CustomerService { get; set; }
    }
}
