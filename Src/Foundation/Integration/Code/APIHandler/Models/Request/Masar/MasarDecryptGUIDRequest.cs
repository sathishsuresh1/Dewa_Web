using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarDecryptGUIDRequest
    {
        public Decryptdetails decryptdetails { get; set; }
    }

    public class Decryptdetails
    {
        public string key { get; set; }
        public string lang { get; set; }
        public string processtype { get; set; }
    }

}
