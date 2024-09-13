using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.HappinessSvc
{
    [XmlType("answer")]
    public class AnswersResponse : CommonDetailsResponse
    {
        [XmlElement(ElementName = "answerNumber")]
        public string AnswerNumber { get; set; }

        [XmlElement(ElementName = "questionNumber")]
        public string QuestionNumber { get; set; }

      
    }
}
