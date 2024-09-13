using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory
{
    public class IBANNumberV2Request
    {
        public string contractaccountnumber { get; set; }
        public string businesspartner { get; set; }
        public string ibannumber { get; set; }
        public string chequeiban { get; set; }
        public string address { get; set; }
        public string notificationnumber { get; set; }
        public string skipcaflag { get; set; }
        public string bankdetailsid { get; set; }
        public string vendorid { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string appversion { get; set; }
        public string appidentifier { get; set; }
        public string mobileosversion { get; set; }
        public string validateibanflag { get; set; }
        public string lang { get; set; }
    }
}
