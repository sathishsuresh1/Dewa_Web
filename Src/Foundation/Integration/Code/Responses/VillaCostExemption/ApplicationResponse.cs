using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.VillaCostExemption
{
    public class ApplicationResponse
    {
        [JsonProperty("applicationreference")]
        public string Applicationreference { get; set; }

        [JsonProperty("attachmenttypes")]
        public List<Attachmenttype> Attachmenttypes { get; set; }

        [JsonProperty("bpdetails")]
        public List<Bpdetail> Bpdetails { get; set; }

        [JsonProperty("customerdetails")]
        public List<Customerdetail> CustomerDetails { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("notificationnumber")]
        public object Notificationnumber { get; set; }

        [JsonProperty("ownerdetails")]
        public List<Ownerdetail> OwnerDetails { get; set; }

        [JsonProperty("ownertypes")]
        public List<Ownertype> Ownertypes { get; set; }

        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        [JsonProperty("statustypes")]
        public List<Statustype> Statustypes { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class Bpdetail
    {
        [JsonProperty("bpapplicationreferencenumber")]
        public string Bpapplicationreferencenumber { get; set; }

        [JsonProperty("bpemiratesid")]
        public string Bpemiratesid { get; set; }

        [JsonProperty("bpestimate")]
        public string Bpestimate { get; set; }

        [JsonProperty("bpname")]
        public string Bpname { get; set; }

        [JsonProperty("bpnumber")]
        public string Bpnumber { get; set; }

        [JsonProperty("bpversion")]
        public string Bpversion { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("relationship")]
        public string Relationship { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("valid")]
        public string Valid { get; set; }
    }

}
