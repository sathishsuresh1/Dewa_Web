using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses
{
    public class Data
    {
        public int IntResult { get; set; }
        public string StringResult { get; set; }
        public string Result { get; set; }
    }

    public class WebsiteSurveyResponse
    {
        public string Description { get; set; }
        public int ResponseCode { get; set; }
        public Data Data { get; set; }
    }

}
