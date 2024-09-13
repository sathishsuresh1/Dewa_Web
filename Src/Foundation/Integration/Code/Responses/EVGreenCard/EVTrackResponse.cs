using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.EVGreenCard.Tr
{
    public class Evnotificationlist
    {
        public string businesspartnernumber { get; set; }
        public string codegroup { get; set; }
        public string codegrouppartobject { get; set; }
        public string coding { get; set; }
        public string notificationdate { get; set; }
        public string notificationnumber { get; set; }
        public string notificationtime { get; set; }
        public string notificationtype { get; set; }
        public string notificationtypetext { get; set; }
        public string partobject { get; set; }
        public string requesttype { get; set; }
        public string shortStatustext { get; set; }
        public object status { get; set; }
    }

    public class Return
    {
        public string description { get; set; }
        public IList<Evnotificationlist> evnotificationlist { get; set; }
        public string responsecode { get; set; }
    }

    public class GetEVTrackRequestResponse
    {
        [JsonProperty("return")]
        public Return @return { get; set; }
    }

    public class Body
    {

        public GetEVTrackRequestResponse GetEVTrackRequestResponse { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }
    }

    public class EVTrackResponse
    {
        public Envelope Envelope { get; set; }
    }


}
