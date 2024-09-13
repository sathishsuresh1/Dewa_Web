using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarDisplayBankResponse
    {
        public List<BankmasterdetailApp> bankmasterdetails { get; set; }
        public string description { get; set; }
        public string responsecode { get; set; }
    }

    public class BankmasterdetailApp
    {
        public string bankaccount { get; set; }
        public string bankkey { get; set; }
        public string bankname { get; set; }
        public string holdername { get; set; }
        public string iban { get; set; }
        public string swift { get; set; }
    }
}
