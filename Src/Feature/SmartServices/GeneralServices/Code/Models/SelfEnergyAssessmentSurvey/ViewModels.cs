using DEWAXP.Foundation.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Feature.GeneralServices.Models.SelfEnergyAssessmentSurvey
{
    public class MessageViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class SurveyPageViewModel
    {
        public Section FirstSection { get; set; }
    }
    public class LandingPageViewModel
    {
        public bool IsStarted { get; set; }
        public int PercentageDone { get; set; }
        public List<KeyValuePair<string, string>> Downloads { get; set; }
    }

    [Serializable]
    public class Survey
    {
        public Stage Stage { get; set; }
        public List<Section> Sections { get; set; }
        public List<DEWAXP.Foundation.Integration.APIHandler.Models.Request.SelfEnergyAssessmentSurvey.Answerlist> Answers { get; set; }
        public Stage GetNextStage()
        {
            switch (this.Stage)
            {
                case Stage.One:
                    return Stage.Two;
                case Stage.Two:
                    return Stage.Three;
                case Stage.Three:
                    return Stage.Four;
                case Stage.Four:
                    return Stage.Five;
                case Stage.Five:
                    return Stage.Six;
                case Stage.Six:
                    return Stage.Seven;
                case Stage.Seven:
                    return Stage.Eight;
                default:
                    return Stage.One;

            }
        }
        public Stage GetPreviousStage()
        {
            switch (this.Stage)
            {
                case Stage.Three:
                    return Stage.Two;
                case Stage.Four:
                    return Stage.Three;
                case Stage.Five:
                    return Stage.Four;
                case Stage.Six:
                    return Stage.Five;
                case Stage.Seven:
                    return Stage.Six;
                case Stage.Eight:
                    return Stage.Seven;
                default:
                    return Stage.One;

            }
        }
        public int GetStageNumber()
        {
            int index = (int)this.Stage + 1;
            return index;
        }
        //public int QuestionFactor { get; set; }
        public int TotalQuestions { get; set; }
        public int QuestionSaved { get; set; }
        public int LastPercentageCompleted { get; set; }
        public SectionType JourneyType { get; set; }
    }

    public enum Stage
    {
        One, Two, Three, Four, Five, Six, Seven, Eight
    }
    public enum QuestionType
    {
        TextBox, RadioButton, CheckBox, SelectList, Slider, Rating
    }

    public enum SectionType { Both, Apartment, Villa }
    public class Section
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SectionType Type { get; set; }
        public int PercentageCompleted { get; set; }

        public int GetSectionNumber()
        {
            switch (this.Name.Trim().ToUpper())
            {
                case "GIN":
                    return 1;
                case "GIA":
                case "GIV":
                    return 2;
                case "BVA":
                    return 3;
                case "BTA":
                case "BTV":
                    return 4;
                case "LVA":
                    return 5;
                case "KVA":
                    return 6;
                case "SID":
                    return 7;
                case "EXV":
                    return 8;
                default:
                    return 1;
            }
        }
        public List<SurveyQuestionViewModel> Questions { get; set; }
        public SectionType JourneyType { get; set; }
    }

    public class SurveyQuestionViewModel
    {
        public int Counter { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public string DQDisplayNumber { get; set; }
        //public int MaximumSelectCount { get; set; }
        public bool IsDynamic { get; set; }
        public QuestionType Type { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public int MaxSelection { get; set; }
        public bool IsIconImage { get { return this.Answers != null && this.Answers.Any(x => !string.IsNullOrEmpty(x.ImageUrl)) ? true : false; } }
        public string GetNumberWithLeadingZeros()
        {
            if (this.Number < 10) return string.Format("000{0}", this.Number);
            return string.Format("00{0}", this.Number);
        }
        public List<SurveyAnswerViewModel> Answers { get; set; }
        public string SelectedAnswer
        {
            get { return this.Answers.Where(x => x.Selected).FirstOrDefault()?.AnswerInputText ?? ""; }
        }
        public string SelectedAnswerDisplay
        {
            get { return this.Answers.Where(x => x.Selected).FirstOrDefault()?.Description ?? ""; }
        }
        public string GetKey()
        {
            if (this.IsDynamic)
                return string.Format("{0}{1}_{2}_{3}", this.ParentSection.ToUpper(), this.ParentQuestionNumber, this.Number, this.ParentAnswer.Number);
            else
                return string.Format("{0}_{1}", this.ParentSection.ToUpper(), this.Number);
        }
        public string ParentSection { get; set; }
        public string GetDynamicSectionId()
        {
            return string.Format("{0}{1}{2}", ParentSection.ToLower(), ParentQuestionNumber.ToString(), ParentAnswer == null ? Number.ToString() : ParentAnswer.Number);
        }

        public int ParentQuestionNumber { get; set; }
        public SurveyAnswerViewModel ParentAnswer { get; set; }

        public void SortAnswers()
        {
            if (this.Answers == null) return;
            try
            {
                this.Answers = this.Answers.OrderBy(x => x.Number).ToList();
            }
            catch (Exception ex) { LogService.Error(ex, new object()); }
        }
    }


    public class SurveyAnswerViewModel
    {
        public string Description { get; set; }
        public string Number { get; set; }
        public bool Selected { get; set; }
        public string ImageUrl { get; set; }
        public string AnswerInputText { get; set; }
        public string GetKey(string parent)
        {
            return string.Format("{0}_{1}", parent, this.Number);
        }
        public string GetToggleAttribute(SurveyQuestionViewModel mainQ)
        {
            return this.DynamicQuestions?.Count > 0 ? string.Format("toggle-target=\".{0}\"", mainQ.ParentSection.ToLower() + mainQ.Number.ToString() + this.Number) : "";
        }
        public List<SurveyQuestionViewModel> DynamicQuestions { get; set; }
    }

    public static class Extensions
    {
        public static QuestionType QuestionTypeFromString(this string type)
        {
            switch (type.Trim().ToUpper())
            {
                case "RB":
                    return QuestionType.RadioButton;
                case "CB":
                    return QuestionType.CheckBox;
                case "IN":
                    return QuestionType.TextBox;
                case "SL":
                    return QuestionType.Slider;
                case "RT":
                    return QuestionType.Rating;
                default:
                    return QuestionType.SelectList;
            }
        }

        public static SectionType SectionTypeFromString(this string type)
        {
            switch (type.Trim().ToUpper())
            {
                case "A":
                    return SectionType.Apartment;
                case "V":
                    return SectionType.Villa;
                default: // B both
                    return SectionType.Both;
            }
        }
    }

    public class ReportPageViewModel
    {
        public string ContentKey { get; set; }
        public string ElectricityDataSeriesJson { get; set; }
        public string WaterDataSeriesJson { get; set; }
        public string Version { get; set; }
        public List<Tips> Tips { get; set; }
    }

    public class Tips
    {
        public string Title { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string ImgUrl { get; set; }
    }

}
