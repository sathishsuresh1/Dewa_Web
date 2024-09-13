using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Models.ConsumptionComplaint
{
    public class SurveyInfoDetail
    {
        public SurveyInfoDetail()
        {
            PreRadioQuestions = new List<QuestionAndAnsItem>();
            CustomerEmotion = new List<QuestionAndAnsItem>();
            FinalQuestionList = new List<QuestionAndAnsItem>();
            BottomQuestionList = new List<QuestionAndAnsItem>();
        }

        public string IntroText { get; set; }
        public string SurveyType { get; set; }

        public string SurveyNo { get; set; }

        public List<QuestionAndAnsItem> PreRadioQuestions { get; set; }

        public List<QuestionAndAnsItem> CustomerEmotion { get; set; }

        public List<QuestionAndAnsItem> FinalQuestionList { get; set; }
        public List<QuestionAndAnsItem> BottomQuestionList { get; set; }

        public bool ShowError { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class QuestionAndAnsItem
    {

        public QuestionAndAnsItem()
        {
            AnsList = new List<QuestionAndAnsItem>();

        }
        public string Id { get; set; }
        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }

        public string SubQuestionId { get; set; }
        /// <summary>
        /// Subquestion
        /// </summary>
        public List<QuestionAndAnsItem> SubQuestion { get; set; }
        /// <summary>
        /// Text & Value
        /// </summary>
        public List<QuestionAndAnsItem> AnsList { get; set; }

        public string PreSectionTitle { get; set; }

        public string ValueId { get; set; }

        public string Text { get; set; }

        public string Category { get; set; }
    }
}