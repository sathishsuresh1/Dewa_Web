using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.DewaScholarship
{
    public class EmailVerificationRequest
    {
        public string vendorid { get; set; }
        public string param { get; set; }
        public string resendflag { get; set; }
        public string lang { get; set; }
    }
}
