using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    public class MasarDropdownRequest
    {
        public string lang { get; set; }

    }
  
    public class MasarDropdownBaseRequest
    {
        public MasarDropdownRequest dropdowninputs { get; set; }
    }

}
