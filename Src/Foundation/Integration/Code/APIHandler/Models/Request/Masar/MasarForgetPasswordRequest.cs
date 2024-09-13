using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarForgetPasswordRequest
    {
        public Resetcredentialdetails resetcredentialdetails { get; set; }
    }

    public class Resetcredentialdetails
    {
        //public string appidentifier { get; set; }
        //public string appversion { get; set; }
        public string lang { get; set; }
        //public string mobileosversion { get; set; }
      //  public string mode { get; set; }
       // public string otp { get; set; }
        public string password { get; set; }
        //public string processtype { get; set; }
        public string userid { get; set; }
        public string reference { get; set; }
        public string vendorid { get; set; }
    }
}
