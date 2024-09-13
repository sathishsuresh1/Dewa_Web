using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarFetchDEDResponse
    {
        public string description { get; set; }
        public string responsecode { get; set; }
        public Tradelicensedetails tradelicensedetails { get; set; }
    }

    public class Tradelicensedetails
    {
        public string attachmentflag { get; set; }
        public string expirydate { get; set; }
        public string expirydateflag { get; set; }
        public object issueauthority { get; set; }
        public string issueauthorityflag { get; set; }
        public string issuedate { get; set; }
        public string issuedateflag { get; set; }
        public object message { get; set; }
        public string tradelicensenumber { get; set; }
        public string tradenumberflag { get; set; }
    }

}
