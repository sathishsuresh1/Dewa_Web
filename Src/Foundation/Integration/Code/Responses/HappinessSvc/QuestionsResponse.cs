using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.HappinessSvc
{
    [XmlType("question")]
    public class QuestionsResponse : CommonDetailsResponse
    {
        [XmlElement(ElementName = "questionNumber")]
        public string QuestionNumber { get; set; }
    }
}
