using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate
{
    public class NewConnectionTaxInvoicePdfResponse
    {
        public byte[] content { get; set; }
        public string contenttype { get; set; }
        public string description { get; set; }
        public object filedescription { get; set; }
        public string filesize { get; set; }
        public string responsecode { get; set; }
    }
}
