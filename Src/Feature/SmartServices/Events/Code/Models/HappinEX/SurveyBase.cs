using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    [SitecoreType(TemplateName = "Basic Survey", TemplateId = "{9AB614B7-C3D6-40DA-8AE3-771B9582F5EF}", AutoMap = true)]
    public class BasicSurvey : GlassBase
    {
        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }


        [SitecoreField(FieldName = "Product or Service")]
        public virtual string ProductOrService { get; set; }

        [SitecoreField(FieldName = "When Happy", Setting = SitecoreFieldSettings.InferType)]
        public virtual IEnumerable<SurveySection> QuestionsWhenHappy { get; set; }

        [SitecoreField(FieldName = "When Neutral", Setting = SitecoreFieldSettings.InferType)]
        public virtual IEnumerable<SurveySection> QuestionsWhenNeutral { get; set; }

        [SitecoreField(FieldName = "When Sad", Setting = SitecoreFieldSettings.InferType)]
        public virtual IEnumerable<SurveySection> QuestionsWhenSad { get; set; }

        [SitecoreField(FieldName = "Channels", Setting = SitecoreFieldSettings.InferType)]
        public virtual IEnumerable<SurveyChannel> Channels { get; set; }

        [SitecoreField(FieldName = "Data Storage Location", FieldType = SitecoreFieldType.DropTree, Setting = SitecoreFieldSettings.InferType)]
        public virtual string DataStorageLocation { get; set; }

        [SitecoreField(FieldName = "Post Service Survey", Setting = SitecoreFieldSettings.InferType)]
        public virtual bool IsPostServiceSurvey { get; set; }
        
        [SitecoreField(FieldName = "IsPre2020Survey", Setting = SitecoreFieldSettings.InferType)]
        public virtual bool IsPre2020Survey { get; set; }
    }

}