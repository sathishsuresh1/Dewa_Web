using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.IdealHomeConsumer
{
    [SitecoreType(TemplateId = "{42E3EE47-D577-4E47-84C6-9297DF996328}", AutoMap = true)]
    public class AccordionSet : GlassBase
    {
        [SitecoreField("Header")]
        public virtual string Header { get; set; }
        public virtual IEnumerable<AccordionItem> Children { get; set; }
    }
}