using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword
{
    public class ForgotUserNameResponse
    {     
        public string emailid { get; set; }
        public string errorcode { get; set; }
        public string errormessage { get; set; }
        public string success { get; set; }
    }
}
