using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar
{
    public class MasarDropDownBaseResponse
    {
        public string responsecode { get; set; }
        public string description { get; set; }

        public List<MasarDropdownResponse> dropdownlist { get; set; }
    }

    public class MasarDropdownResponse
    {
        public string fieldname { get; set; }
        public List<DropDownValue> values { get; set; }
        
    }

    public class DropDownValue
    {
        public string key { get; set; }
        public string lang { get; set; }
        public string value { get; set; }
    }
}
