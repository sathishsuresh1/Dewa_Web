using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarTrackInputRequest
    {
        public Trackinput trackinput { get; set; }
    }

    public class Trackinput
    {
        public string referencenumber { get; set; }
        public string applicationnumber { get; set; }
        public string lang { get; set; }
    }


}
