using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response
{
    public class EvPlateDetailsResponse : ApiBaseResponse
    {
        public List<EVPlateDetail> EVPlateCodeList { get; set; }
    }


    public class EVPlateDetail
    {
        public string categoryAR { get; set; }
        public string categoryCode { get; set; }
        public string categoryEN { get; set; }
        public string codeAR { get; set; }
        public string codeEN { get; set; }
        public string plateCode { get; set; }
        public string region { get; set; }
    }
}
