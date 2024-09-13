using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.VillaCostExemption
{
    public class DownloadFileRequest
    {
        public Attachrequest attachrequest { get; set; }
    }
    public class Attachrequest : BaseRequest
    {
        public string applicationreferencenumber { get; set; }
        public string documentnumber { get; set; }
        public string flag { get; set; }
        public string attachmenttype { get; set; }
        public string content { get; set; }
        public string filename { get; set; }
        public string mimetype { get; set; }
        public string action { get; set; }
        public string itemnumber { get; set; }
    }
}
