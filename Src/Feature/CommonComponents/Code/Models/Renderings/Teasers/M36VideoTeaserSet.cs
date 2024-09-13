using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data.Items;
using Glass.Mapper.Sc.Fields;
using System;
using Sitecore.Data.Fields;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    [SitecoreType(TemplateId = "{1F3E97CB-158E-423E-AD06-11DB943F2C92}", AutoMap = true)]
    public class M36VideoTeaserSet : ContentBase
    {
        //public M36VideoTeaserSet()
        //{
        //    Children = new List<VideoTeaserItem>();
        //}
        [SitecoreField("Header")]
        public virtual string Header { get; set; }

        [SitecoreField("Subheader")]
        public virtual string Subheader { get; set; }
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<VideoTeaserItem> Children { get; set; }
    }
    [SitecoreType(TemplateId = "{4A1836B0-96EA-40DD-A17E-6062038DBA5F}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class VideoTeaserItem : ContentBase
    {

        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        [SitecoreField("Video Source")]
        public virtual string VideoSource { get; set; }

        [SitecoreField("Main Image")]
        public virtual Image MainImage { get; set; }

        [SitecoreField("Video Background Image")]
        public virtual Image VideoBackgroundImage { get; set; }

        [SitecoreField("Video Name")]
        public virtual string VideoName { get; set; }

        [SitecoreField("Video Date")]
       public virtual DateTime VideoDate { get; set; }

        [SitecoreField("Description")]
        public virtual string Description { get; set; }

        [SitecoreField("Redirect URL")]
        public virtual string RedirectURL { get; set; }

        [SitecoreField("Redirect URL Text")]
        public virtual string RedirectURLText { get; set; }
    }
}