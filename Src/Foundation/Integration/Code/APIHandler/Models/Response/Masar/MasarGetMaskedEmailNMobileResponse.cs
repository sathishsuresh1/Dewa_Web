using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarGetMaskedEmailNMobileResponse
    {
        public string description { get; set; }
        public string email { get; set; }
        public string maskedemail { get; set; }
        public string maskedmobile { get; set; }
        public string mobile { get; set; }
        public string responsecode { get; set; }
        public string referencenumber { get; set; }
    }
}
