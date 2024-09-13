using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.VillaCostExemption
{
    public class BaseRequest
    {        
        public string vendorid { get; set; }        
        public string mobileosversion { get; set; }        
        public string appversion { get; set; }        
        public string appidentifier { get; set; }        
        public string lang { get; set; }        
        public string sessionid { get; set; }        
        public string userid { get; set; }
    }
}
