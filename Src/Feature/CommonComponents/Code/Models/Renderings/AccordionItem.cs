using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.IdealHomeConsumer
{
    [SitecoreType(TemplateId = "{F14D0C9A-D263-4DB5-9880-F5ABCE1C65F5}", AutoMap = true)]
     public class AccordionItem : GlassBase
    {
        #region AccordionList
        [SitecoreField("Accordion Title")]
        public virtual string Title { get; set; }
        [SitecoreField("Accordion Content")]
        public virtual string Content { get; set; }
        [SitecoreField("Accordion Image")]
        public virtual Image Image { get; set; }
        [SitecoreField("Module")]
        public virtual bool Module { get; set; }
        [SitecoreField("Module Placeholder")]
        public virtual string ModulePlaceholder  { get; set; }
        #endregion
    }
}