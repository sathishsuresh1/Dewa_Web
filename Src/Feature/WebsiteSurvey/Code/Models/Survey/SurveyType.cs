// <copyright file="SurveyType.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\hansrajsinh.rathva</author>

namespace DEWAXP.Feature.WebsiteSurvey.Models.Survey
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using System.Collections.Generic;
    /// <summary>
    /// Defines the <see cref="SurveyType" />
    /// </summary>
    [SitecoreType(TemplateId = "{92918200-8DCD-4C43-BE3F-E093EEC0847B}", AutoMap = true)]
    public class SurveyType : GlassBase
    {
        /// <summary>
        /// Gets or sets the InquiryTypeTitle
        /// </summary>
        [SitecoreField("Survey Type Title")]
        public virtual string SurveyTypeTitle { get; set; }

        /// <summary>
        /// Gets or sets the Surveyid
        /// </summary>
        [SitecoreField("Survey id")]
        public virtual string Surveyid { get; set; }

        /// <summary>
        /// Gets or sets the Bottom text
        /// </summary>
        [SitecoreField("Bottom text")]
        public virtual string Bottomtext { get; set; }
        /// <summary>
        /// Gets or sets the QuestionsList
        /// </summary>
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<SurveyStep> SurveySteps { get; set; }

    }
    /// <summary>
    /// Defines the <see cref="SurveyStep" />
    /// </summary>
    [SitecoreType(TemplateId = "{A5A90EDF-AAB3-466F-9558-E5B2A3B3D9FC}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class SurveyStep : GlassBase
    {
        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        [SitecoreField("Text")]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [SitecoreField("Value")]
        public virtual string Value { get; set; }
        /// <summary>
        /// Gets or sets the QuestionsList
        /// </summary>
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<QuestionGroup> QuestionGroupList { get; set; }

    }
    /// <summary>
    /// Defines the <see cref="SurveyStep" />
    /// </summary>
    [SitecoreType(TemplateId = "{C3F00C7A-3FE9-40D5-A925-74E4EEA71D51}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class QuestionGroup : GlassBase
    {
        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        [SitecoreField("Text")]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [SitecoreField("Value")]
        public virtual string Value { get; set; }
        /// <summary>
        /// Gets or sets the QuestionsList
        /// </summary>
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<Question> QuestionList { get; set; }
    }
    /// <summary>
    /// Defines the <see cref="J110Services" />
    /// </summary>
    [SitecoreType(TemplateId = "{828C6757-245E-4B4F-8A59-1FB4155AA56A}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class Question : GlassBase
    {
        /// <summary>
        /// Gets or sets the DetailedQuestion
        /// </summary>
        [SitecoreField("Question Details")]
        public virtual string QuestionDetails { get; set; }

        /// <summary>
        /// Gets or sets the DetailedQuestion
        /// </summary>
        [SitecoreField("Question Type")]
        public virtual QuestionTypes QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the AnswerList
        /// </summary>
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<Answer> AnswerList { get; set; }
    }
    /// <summary>
    /// Defines the <see cref="QuestionTypes" />
    /// </summary>
    [SitecoreType(TemplateId = "{62533F68-5E9A-4813-B2A5-412295D9D90E}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class QuestionTypes : GlassBase
    {
        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        [SitecoreField("Text")]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [SitecoreField("Value")]
        public virtual string Value { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Answers" />
    /// </summary>
    [SitecoreType(TemplateId = "{CC706C82-D414-465F-90DA-B46449D991CE}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class Answer : GlassBase
    {
        /// <summary>
        /// Gets or sets the Text
        /// </summary>
        [SitecoreField("Text")]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [SitecoreField("Value")]
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        [SitecoreField("TextBox")]
        public virtual bool TextBox { get; set; }
    }
}