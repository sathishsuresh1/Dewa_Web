// <copyright file="WebsiteSurveyQuestionAnswers.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\hansrajsinh.rathva</author>

using DEWAXP.Foundation.Integration.Impl.WebsiteSurveySvc;

namespace DEWAXP.Feature.WebsiteSurvey.Models.Survey
{
    public class WebsiteSurveyQuestionAnswers
    {
        public SurveyType SurveyType { get; set; }
        public SurveyData[] surveyInputs { get; set; }
        public string suggestionText { get; set; }
        public string datasource { get; set; }
    }

    public class surveyInput
    {
        public string choosen_value { get; set; }

        public string question { get; set; }
    }
}