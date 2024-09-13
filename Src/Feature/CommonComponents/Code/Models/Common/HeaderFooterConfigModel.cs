using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Common
{
    [SitecoreType(TemplateId = "{470CEAE9-9FAA-4EF4-B14C-991AA194C87C}", AutoMap = true)]
    public class HeaderFooterConfigModel
    {
        [SitecoreField("Querystring Parameter")]
        public virtual string QuerystrParam { get; set; }

        [SitecoreField(FieldName = "Querystring Parameter Value")]
        public virtual string QuerystrParamVal { get; set; }

        [SitecoreField(FieldName = "QuerystringChecked")]
        public virtual bool QuerystrParamChecked { get; set; }
    }
}