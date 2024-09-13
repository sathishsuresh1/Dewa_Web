using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.General
{
    public class MaiDubaiRequest
    {
        public Subscriptioninput subscriptioninput { get; set; }
    }

    public class Subscriptioninput
    {
        public string appidentifier { get; set; }
        public string appversion { get; set; }
        public string contractaccount { get; set; }
        public string email { get; set; }
        public string emailsubscribeflag { get; set; }
        public string lang { get; set; }
        public string mobile { get; set; }
        public string mobileosversion { get; set; }
        public string mobilesubscribeflag { get; set; }
        public string mode { get; set; }
        public string process { get; set; }
        public string referencenumber { get; set; }
        public string vendorid { get; set; }
    }

}
