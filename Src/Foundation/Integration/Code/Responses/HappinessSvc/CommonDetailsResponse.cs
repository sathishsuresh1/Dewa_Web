using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.HappinessSvc
{
    public class CommonDetailsResponse
    {
        [XmlElement(ElementName = "survey")]
        public string SurveyName { get; set; }

        [XmlElement(ElementName = "type")]
        public string ControlType { get; set; }

        [XmlElement(ElementName = "english")]
        public string EnglishText { get; set; }

        [XmlElement(ElementName = "arabic")]
        public string ArabicText { get; set; }
    }
}
