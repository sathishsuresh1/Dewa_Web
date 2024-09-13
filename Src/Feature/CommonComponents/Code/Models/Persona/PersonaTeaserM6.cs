using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;

namespace DEWAXP.Feature.CommonComponents.Models.Persona
{
    [SitecoreType(TemplateId = "{D692725D-0228-4F67-B76B-513D3611749B}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class PersonaTeaserM6 : GlassBase
    {
        [SitecoreField("Header")]
        public virtual string Header { get; set; }
        [SitecoreField("Subheader")]
        public virtual string Subheader { get; set; }

        [SitecoreField("Link")]
        public virtual string Link { get; set; }

        [SitecoreField("Image")]
        public virtual Image Image { get; set; }

        [SitecoreField("BackgroundImage")]
        public virtual Image BackgroundImage { get; set; }
        
    }
}