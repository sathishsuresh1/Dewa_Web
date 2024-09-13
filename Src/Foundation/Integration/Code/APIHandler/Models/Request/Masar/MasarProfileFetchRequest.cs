using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarProfileFetchRequest
    {
        public Profileinputs profileinputs { get; set; }
    }

    public class Profileinputs
    {
        public string userid { get; set; }
        public string sessionid { get; set; }
    }
}
