using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class EmailVerificationModel
    {
        public string param { get; set; }
        public string applicationvisibility { get; set; }

        public string errorcode { get; set; }

        public string errormessage { get; set; }

        public string newverificationlinkenable { get; set; }

        public string success { get; set; }

        public string verificationstatus { get; set; }
    }
}