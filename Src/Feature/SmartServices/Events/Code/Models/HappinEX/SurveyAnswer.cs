using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    [SitecoreType(TemplateName = "Survey Response", TemplateId = "{0379D918-21CA-43B2-81D7-22BF33B0DE33}", AutoMap = true)]
    public class SurveyAnswer : GlassBase
    {
        [SitecoreField(FieldName = "Response Json")]
        public virtual string ResponseJson { get; set; }
    }

    [SitecoreType(TemplateName = "Folder", TemplateId = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}", AutoMap = true)]
    public class ResponseFolder : GlassBase
    {

    }
}