using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.IdealHome.Models.IdealHome
{
    [SitecoreType(TemplateId = "{E9379546-CE50-4F70-B73E-9A875FC9BACD}", AutoMap = true)]
    public class SurveyQuestions : GlassBase
    {
        [SitecoreField(FieldName = "Main Criteria")]
        public virtual string MainCriteria { get; set; }

        [SitecoreField(FieldName = "Sub Criteria")]
        public virtual string SubCriteria { get; set; }

        [SitecoreField(FieldName = "Sub Criteria Code")]
        public virtual string SubCriteriaCode { get; set; }

        public virtual List<Surveyoptions> OptionList { get; set; }

        public virtual int QuestionNo { get; set; }
    }
}