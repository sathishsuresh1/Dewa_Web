using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.HappinessSvc
{
    [Serializable]
    [XmlRoot(ElementName = "SetCustHappinessSurveyAnswersResponse")]
    public class HappinessSurveySubmitResponse
    {
        [XmlElement(ElementName = "responseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
    }
}
