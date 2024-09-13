using Newtonsoft.Json;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.Responses.EVGreenCard.Trd
{
    public class Evstatuslist
    {
        public string description { get; set; }
        public string status { get; set; }
        public string statuscode { get; set; }
    }

    public class Return
    {
        public string description { get; set; }
        public object evcardnumber { get; set; }
        public IList<Evstatuslist> evstatuslist { get; set; }
        public string notificationnumber { get; set; }
        public string responsecode { get; set; }
        public string statuscode { get; set; }
        public string statusdescription { get; set; }
    }

    public class GetEVNotificationDetailsResponse
    {
        [JsonProperty("return")]
        public Return @return { get; set; }
    }

    public class Body
    {
        public GetEVNotificationDetailsResponse GetEVNotificationDetailsResponse { get; set; }
    }

    public class Envelope
    {
        public Body Body { get; set; }
    }

   public class EVTrackResponseDetail
    {
        public Envelope Envelope { get; set; }
    }
}
