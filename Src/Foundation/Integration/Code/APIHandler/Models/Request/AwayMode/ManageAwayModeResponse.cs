using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.AwayMode
{
    public class ManageAwayModeResponse : ApiBaseResponse
    {
        public RequestDetail requestDetails { get; set; }
        public string requestNumber { get; set; }
    }
    public class RequestDetail
    {

        public string beginDate { get; set; }
        public string contractAccount { get; set; }
        public string email { get; set; }
        public string endDate { get; set; }
        public string frequency { get; set; }
        public string request { get; set; }

    }
}
