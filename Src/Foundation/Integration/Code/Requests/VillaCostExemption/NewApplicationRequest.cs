
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Requests.VillaCostExemption
{
    public class NewApplicationRequest
    {        
        public villarequest villarequest { get; set; }                
    }

    public class ownerdetails
    {
        public string idtype { get; set; }
        public string email { get; set; }
        public string emiratesid { get; set; }
        public string itemnumber { get; set; }
        public string marsoom { get; set; }
        public string mobile { get; set; }
        public string mobile2 { get; set; }
        public string name { get; set; }
        public string ownerapplicationreferencenumber { get; set; }
        public string passport { get; set; }
        public string passportexpiry { get; set; }
        public string passportissue_authority { get; set; }
        public string relation { get; set; }
        public string remarks { get; set; }
    }

    public class villarequest:BaseRequest
    {
        public string applicationnumber { get; set; }
        public string applicationreferencenumber { get; set; }
        public string applicationsequencenumber { get; set; }
        public string applicationstatus { get; set; }
        public string applicationstatusdesc { get; set; }
        public string customernumber { get; set; }
        public string dateofrecordcreation { get; set; }
        public string estimate { get; set; }        
        public string notificationnumber { get; set; }
        public List<ownerdetails> ownerdetails { get; set; }
        public string ownertype { get; set; }
        public string ownertypedescription { get; set; }
        public string processcode { get; set; }
        public string remarks { get; set; }        
        public string timeofrecordcreation { get; set; }        
        public string version { get; set; }
    }



}
