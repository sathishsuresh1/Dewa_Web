using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    [SitecoreType(TemplateName = "Question", TemplateId = "{B666EEB1-0394-4C97-BAE0-C6A22F5C2992}", AutoMap = true)]
    public class SurveyQuestion : GlassBase
    {
        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }


        [SitecoreField(FieldName = "Show When Technical Discussion is done", Setting = SitecoreFieldSettings.InferType)]
        public virtual bool ShowWhenTechnicalDiscussionIsDone { get; set; }


        [SitecoreField(FieldName = "Show When Discussion Channel Is", Setting = SitecoreFieldSettings.InferType)]
        public virtual IEnumerable<SurveyChannel> ShowWhenDisscussionChannelIs { get; set; }

        public string DiscussionChannels
        {
            get
            {
                return this.ShowWhenDisscussionChannelIs != null ? string.Join(",", this.ShowWhenDisscussionChannelIs.Select(x => x.Channel)) : string.Empty;
            }
        }

        private const string wrapper_div = "j120-smart-response--happinEX_improve--suggestTD";
        public string IsTD
        {
            get { return this.ShowWhenTechnicalDiscussionIsDone ? wrapper_div : string.Empty; }
        }
    }

}