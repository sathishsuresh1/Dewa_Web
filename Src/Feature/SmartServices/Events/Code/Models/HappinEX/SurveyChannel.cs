using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    [SitecoreType(TemplateName = "Channel", TemplateId = "{DD66A3FF-68DB-4CAF-B3A0-A61773631491}", AutoMap = true)]
    public class SurveyChannel:GlassBase
    {
        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }

        [SitecoreField(FieldName = "Id")]
        public virtual string Channel { get; set; }
    }
}