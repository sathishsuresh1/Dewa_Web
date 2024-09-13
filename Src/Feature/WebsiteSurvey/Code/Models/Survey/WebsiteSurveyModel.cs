using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;

namespace DEWAXP.Feature.WebsiteSurvey.Models.Survey
{

    public class WebsiteSurveyModel
    {
        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }
        [SitecoreField(FieldName = "BackgroundImage")]
        public virtual Image BackgroundImage { get; set; }
        [SitecoreField(FieldName = "Description")]
        public virtual string Description { get; set; }
        [SitecoreField(FieldName = "CloseText")]
        public virtual string CloseText { get; set; }
        [SitecoreField(FieldName = "LinkText")]
        public virtual string LinkText { get; set; }
        [SitecoreField(FieldName = "SurveyLink")]
       public virtual Link SurveyLink { get; set; }
        [SitecoreField(FieldName = "IsActive")]
        public virtual bool IsActive { get; set; }

        public virtual string ExpiryDays { get; set; }
    }
}