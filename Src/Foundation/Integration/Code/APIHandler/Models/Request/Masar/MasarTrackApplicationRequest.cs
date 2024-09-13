using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarTrackApplicationRequest
    {
        public Trackinputs trackinputs { get; set; }
    }

    public class Trackinputs
    {
        public string referencenumber { get; set; }
        public string lang { get; set; }
    }
}
