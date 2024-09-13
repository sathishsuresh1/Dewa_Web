using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    [Serializable]
    public class MasarForgotUserNameRequest
    {
        public Forgotiddetails forgotiddetails { get; set; }
    }

    [Serializable]
    public class Forgotiddetails
    {
        public string appidentifier { get; set; }
        public string appversion { get; set; }
        public string businesspartnernumber { get; set; }
        public string email { get; set; }
        public string lang { get; set; }
        public string mobileosversion { get; set; }
        public string customertype { get; set; }
    }

}
