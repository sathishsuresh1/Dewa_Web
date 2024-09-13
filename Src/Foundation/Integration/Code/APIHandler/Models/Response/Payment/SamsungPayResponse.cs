using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Payment
{
    public class SamsungPayResponse
    {
        [JsonProperty("errormessage")]
        public string Errormessage;

        [JsonProperty("responsecode")]
        public string Responsecode;

        [JsonProperty("amount")]
        public string Amount;

        [JsonProperty("statusdescription")]
        public string Statusdescription;

        [JsonProperty("dewatransactionid")]
        public string Dewatransactionid;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("mobilepaymentstatus")]
        public object Mobilepaymentstatus;

        [JsonProperty("pgtransactionid")]
        public string Pgtransactionid;

        [JsonProperty("statuscode")]
        public string Statuscode;

        [JsonProperty("approvalcode")]
        public object Approvalcode;

        [JsonProperty("sdgreferencenumber")]
        public string Sdgreferencenumber;

        [JsonProperty("vendor")]
        public object Vendor;

        [JsonProperty("dateandtime")]
        public string Dateandtime;

        [JsonProperty("paymentmethod")]
        public object Paymentmethod;
    }


}
