using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.ComplaintSurvey
{
	[Serializable]
	public class ComplaintSurveyModel
	{
		public string[] QuestionList { get; set; }
		public string[] QuestionNoList { get; set; }
		public string[] AnswerChoice { get; set; }
	}
	public class Questions
	{
		public string Question { get; set; }
		public string QuestionNo { get; set; }
		public string QuestionGroup { get; set; }
		
	}

    public class Survey
    {
        public List<SurveyQuestion> questions { get; set; }
    }


    public class SurveyQuestion {
        public string lang { get; set; }
        public string questionname { get; set; }
        public string questionnumber { get; set; }
        public string questiontype { get; set; }
        public string SrNo { get; set; }

        public type type {
            get
            {
                if (!string.IsNullOrEmpty(questiontype))
                {
                    return (type)int.Parse(questiontype);
                }
                return type.RB;
            }
        }
        public string subquestionnumber { get; set; }
        public string surveytype { get; set; }
        public List<SurveyOption> options{get;set;}

        public List<SurveyQuestion> subquestion { get; set; }
    }

    public class SurveyOption
    {
        public string lang { get; set; }
        public string optionnumber { get; set; }
        public string optiontext { get; set; }
        public string questionnumber { get; set; }
        public string subquestionnumber { get; set; }
        public string surveytype { get; set; }
    }
    public enum type
    {
        CB= 0, RB=1,TB=2
    };
}