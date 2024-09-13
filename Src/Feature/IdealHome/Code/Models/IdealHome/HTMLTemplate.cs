using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using Sitecore.Data.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.IdealHome.Models.Configurations.Html_Template
{

    [SitecoreType(TemplateId = "{D696C81B-95B2-4479-AC36-659FE2A8F9C5}", AutoMap = true)]
    public class HtmlTemplateConfigurations : GlassBase
    {
        
        [SitecoreField(FieldName = "Html Text")]
        public virtual string HtmlText { get; set; }

        [SitecoreField(FieldName = "Html Image")]
        public virtual Image HtmlImage { get; set; }

        [SitecoreField(FieldName = "Html Email")]
        public virtual string  HtmlEmail { get; set; }
    }
}