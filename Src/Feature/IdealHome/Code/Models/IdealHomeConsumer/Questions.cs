using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.IdealHome.Models.IdealHomeConsumer
{

    [SitecoreType(TemplateId = "{D0280B94-C4A8-4DC1-824D-E19BA7E1A2C2}", AutoMap = true)]
    public class Questions:GlassBase
    {
        [SitecoreField(FieldName = "Question")]
        public virtual string Question { get; set; }

        [SitecoreField(FieldName = "CorrectAnswer")]
        public virtual Answers CorrectAnswer { get; set; }

        [SitecoreChildren]
        public virtual IEnumerable<Answers> AnswersList { get; set; }
        public virtual Answers AnswerChoosen { get; set; }
    }
}