// <copyright file="InquiryType.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GeneralServices.Models.CCSurvey
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="InquiryType" />
    /// </summary>
    [SitecoreType(TemplateId = "{19F5EDDB-10BB-4288-A734-0E12893812A2}", AutoMap = true)]
    public class InquiryType : GlassBase
    {
        /// <summary>
        /// Gets or sets the InquiryTypeTitle
        /// </summary>
        [SitecoreField("Inquiry Type Title")]
        public virtual string InquiryTypeTitle { get; set; }

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
        public virtual IEnumerable<Question> QuestionList { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="J110Services" />
    /// </summary>
    [SitecoreType(TemplateId = "{2D92732A-920F-42D6-B33A-D5FADFE41C20}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class Question : GlassBase
    {
        /// <summary>
        /// Gets or sets the DetailedQuestion
        /// </summary>
        [SitecoreField("Detailed Question")]
        public virtual string DetailedQuestion { get; set; }

        /// <summary>
        /// Gets or sets the DetailedQuestion
        /// </summary>
        [SitecoreField("Question Type")]
        public virtual QuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the AnswerList
        /// </summary>
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<Answer> AnswerList { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="QuestionType" />
    /// </summary>
    [SitecoreType(TemplateId = "{611EFFF2-6B2D-435C-925A-CBCD0F8164D6}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class QuestionType : GlassBase
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
    /// Defines the <see cref="Answer" />
    /// </summary>
    [SitecoreType(TemplateId = "{33D86C33-9F30-49CA-A7CA-8085C08D9E9B}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
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
    }
}
