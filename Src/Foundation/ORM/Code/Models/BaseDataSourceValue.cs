using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.ORM.Models
{
    [SitecoreType(TemplateName = "Base Data Source Value", TemplateId = "{48F30F4B-12DD-49C1-B9B6-FAEF3236ACFD}", AutoMap = true)]
    public class BaseDataSourceValue : GlassBase
    {
        [SitecoreField("Data Value")]
        public virtual string DataValue { get; set; }
    }
}