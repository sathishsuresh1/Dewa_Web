using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarLoginRequest
    {
        public Getloginsessionrequest getloginsessionrequest { get; set; }
    }

    public class Getloginsessionrequest
    {
        public string password { get; set; }
        public string merchantid { get; set; }
        public string merchantpassword { get; set; }
        public string lang { get; set; }
        public string userid { get; set; }
        public string type { get; set; }
    }

}
