using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.UAEPassService
{
    public class UAEPassDubaiIdLoginResponse
    {
        public string aliasname { get; set; }
        public string description { get; set; }
        public bool dewasession { get; set; }
        public string email { get; set; }
        public string fullname_ar { get; set; }
        public string fullname_en { get; set; }
        public string idn { get; set; }
        public bool internalerror { get; set; }
        public string lastlogin { get; set; }
        public string mobile { get; set; }
        public bool registered { get; set; }
        public string responsecode { get; set; }
        public string sessionid { get; set; }
        public string userid { get; set; }
    }
}
