using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarLoginResponse
    {
        public string businesspartner { get; set; }
        public string customertype { get; set; }
        public string description { get; set; }
        public string developerMessage { get; set; }
        public string developerStatus { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string lastlogin { get; set; }
        public string mobile { get; set; }
        public bool popup { get; set; }
        public string primarycontractaccount { get; set; }
        public string responsecode { get; set; }
        public string sessionid { get; set; }
        public bool termsandcondition { get; set; }
        public bool updatemobileemail { get; set; }
        public string userCode { get; set; }
        public string userMessage { get; set; }
        public string userid { get; set; }
    }
}
