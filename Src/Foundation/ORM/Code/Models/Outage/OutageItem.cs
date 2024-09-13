using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;

namespace DEWAXP.Foundation.ORM.Models.Outage
{
    [SitecoreType(TemplateId = "{F2AD8AE3-F22D-4E7C-BD20-04276CC01DE1}", AutoMap = true)]
    public class OutageItem : GlassBase
    {
        [SitecoreField(FieldName = "StartDate")]
        public virtual DateTime? StartDate { get; set; }

        [SitecoreField(FieldName = "EndDate")]
        public virtual DateTime? EndDate { get; set; }

        [SitecoreField(FieldName = "OutageURL")]
        public virtual Link OutageURL { get; set; }
    }
}