using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    [SitecoreType(TemplateName = "Survey Section", TemplateId = "{D0CD51D2-EF23-4F82-BA20-F603172593B2}", AutoMap = true)]
    public class SurveySection : GlassBase
    {
        [SitecoreField(FieldName = "Main Question")]
        public virtual SurveyQuestion MainQuestion { get; set; }

        [SitecoreField(FieldName = "Sub Questions", Setting = SitecoreFieldSettings.InferType)]
        public virtual IEnumerable<SurveyQuestion> SubQuestions { get; set; }

        [SitecoreField(FieldName = "Sub Heading", Setting = SitecoreFieldSettings.InferType)]
        public virtual string SubHeading { get; set; }

        [SitecoreField(FieldName = "Maximum Selections", Setting = SitecoreFieldSettings.InferType)]
        public virtual int MaximumSelections { get; set; }
        //

        [SitecoreField(FieldName = "Show When Technical Discussion is done", Setting = SitecoreFieldSettings.InferType)]
        public virtual bool ShowWhenTechnicalDiscussionIsDone { get; set; }

        [SitecoreField(FieldName = "Show When Technical Discussion Is Done Offline", Setting = SitecoreFieldSettings.InferType)]
        public virtual bool ShowWhenTechnicalDiscussionIsDoneOffline { get; set; }

        private const string wrapper_div = "j120-smart-response--happinEX_improve--suggestTD";
        public string IsTD
        {
            get
            {
                if (this.ShowWhenTechnicalDiscussionIsDone && this.MainQuestion.DiscussionChannels.Count() > 0)
                {
                    return wrapper_div;
                }

                return this.ShowWhenTechnicalDiscussionIsDone ? (this.ShowWhenTechnicalDiscussionIsDoneOffline ? wrapper_div : string.Empty) : string.Empty;
            }
        }
        public string DiscussionChannels
        {
            get
            {
                return this.MainQuestion.ShowWhenDisscussionChannelIs != null ? string.Join(",", this.MainQuestion.ShowWhenDisscussionChannelIs.Select(x => x.Channel)) : string.Empty;
            }
        }
        public string GetHeading
        {
            get
            {
                return string.Format(this.SubHeading, "<strong>" + this.MainQuestion.Title + "</strong>");
            }
        }
    }
}