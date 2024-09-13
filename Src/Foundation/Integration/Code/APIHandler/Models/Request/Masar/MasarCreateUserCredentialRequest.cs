using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarCreateUserCredentialRequest
    {
        public UserCredentialRequest newuserinputs { get; set; }
    }

    public class UserCredentialRequest
    {
        public string userid { get; set; }
        public string password { get; set; }
        public string lang { get; set; }
        public string linkid { get; set; }
        public string otp { get; set; }
        public string reference { get; set; }
        public string processtype { get; set; }
    }
}
