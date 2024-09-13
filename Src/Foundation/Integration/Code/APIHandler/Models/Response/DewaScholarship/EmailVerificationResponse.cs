using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.DewaScholarship
{
    public class EmailVerificationResponse
    {
        public string applicationvisibility { get; set; }
        public string errorcode { get; set; }
        public string errormessage { get; set; }
        public string newverificationlinkenable { get; set; }
        public string success { get; set; }
        public string verificationstatus { get; set; }                             
    }
}
