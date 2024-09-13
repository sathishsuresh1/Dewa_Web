using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut
{
    public class MoveoutRequest
    {
        public List<Moveout_AccountsIn_Request> accountlist { get; set; }
        public string appidentifier { get; set; }
        public string applicationflag { get; set; }
        public string appversion { get; set; }
        public string attachment { get; set; }
        public string attachment2 { get; set; }
        public string attachment2type { get; set; }
        public string attachment3 { get; set; }
        public string attachment3type { get; set; }
        public string attachmenttype { get; set; }
        public string channel { get; set; }
        public string disconnectiondate { get; set; }
        public string disconnectiontime { get; set; }
        public string email { get; set; }
        public string executionflag { get; set; }
        public string ibannumber { get; set; }
        public string lang { get; set; }
        public string mobile { get; set; }
        public string mobileosversion { get; set; }
        public string moveoutversion { get; set; }
        public string notificationtype { get; set; }
        public string plotnumber { get; set; }
        public string premisenumber { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
        public string vendorid { get; set; }
    }

    public class Moveout_AccountsIn_Request
    {
        public string additionalinput { get; set; }
        public string additionalinput1 { get; set; }
        public string additionalinput2 { get; set; }
        public string additionalinput3 { get; set; }
        public string additionalinput4 { get; set; }
        public string additionalinput5 { get; set; }
        public string additionalinput6 { get; set; }
        public string additionalinput7 { get; set; }
        public string additionalinput8 { get; set; }
        public string businesspartnernumber { get; set; }
        public string city { get; set; }
        public string contractaccountname { get; set; }
        public string contractaccountnumber { get; set; }
        public string countrykey { get; set; }
        public string currencykey { get; set; }
        public string dateofbirth { get; set; }
        public string demolishplotnumber { get; set; }
        public string demolishpremise { get; set; }
        public string disconnectiondate { get; set; }
        public string disconnectiontime { get; set; }
        public string email { get; set; }
        public string emiratesid { get; set; }
        public string firstname { get; set; }
        public string fullname { get; set; }
        public string ibannumber { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public string passportnumber { get; set; }
        public string premise { get; set; }
        public string refundmode { get; set; }
        public string region { get; set; }
        public string transferaccountnumber { get; set; }
        public string usertype { get; set; }

    }
}
