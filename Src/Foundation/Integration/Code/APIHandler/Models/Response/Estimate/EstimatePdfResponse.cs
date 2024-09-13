using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate
{
   public class EstimatePdfResponse
    {
        public string content_type { get; set; }
        public string description { get; set; }
        public string file_size { get; set; }
        public string filecontent { get; set; }
        public string responsecode { get; set; }

    }
}
