using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword
{
    public class ForgotUserNameRequest
    {
        public string emiratesid { get; set; }
        public string emailid { get; set; }
        public string passportnumber { get; set; }
        public string vendorid { get; set; }
        public string lang { get; set; }
    }
}
