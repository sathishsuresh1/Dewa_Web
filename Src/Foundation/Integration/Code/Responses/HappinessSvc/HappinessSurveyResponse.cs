using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DEWAXP.Foundation.Integration.Responses.HappinessSvc
{
    [Serializable]
    [XmlRoot(ElementName = "GetCustHappinessSurveyQuestionsResponse")]
    public class HappinessSurveyResponse
    {
        [XmlElement(ElementName = "responseCode")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlArray(ElementName = "surveyQuestionList")]
        public List<QuestionsResponse> Questions { get; set; }

        [XmlArray(ElementName = "surveyAnswerList")]
        public List<AnswersResponse> Answers { get; set; }

        [XmlArray(ElementName = "surveyRelationList")]
        public List<RelationsResponse> Relations { get; set; }
    }
}
