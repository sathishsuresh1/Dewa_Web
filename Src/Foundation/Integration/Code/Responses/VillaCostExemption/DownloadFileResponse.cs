using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.VillaCostExemption
{
    public class DownloadFileResponse
    {
        public List<Attachlist> attachlist { get; set; }
        public string description { get; set; }
        public string responsecode { get; set; }
    }
    public class Attachlist
    {
        public string attachmenttype { get; set; }
        public string documentnumber { get; set; }
        public byte[] content { get; set; }
        public string filename { get; set; }
        public string filesize { get; set; }
    }
}
