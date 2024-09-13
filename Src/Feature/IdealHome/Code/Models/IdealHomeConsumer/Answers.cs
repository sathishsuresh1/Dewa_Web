using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.IdealHome.Models.IdealHomeConsumer
{

    [SitecoreType(TemplateId = "{C89D2673-B840-4414-AF25-C90D65641EB7}", AutoMap = true)]
    public class Answers:GlassBase
    {
        [SitecoreField(FieldName = "Text")]
        public virtual string Text { get; set; }

        [SitecoreField(FieldName = "Value")]
        public virtual string Value { get; set; }

    }
}