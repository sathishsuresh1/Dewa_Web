using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models.TariffCalculator
{
    [SitecoreType(TemplateId = "{23FEE9AB-F1FA-46A7-991C-12BD04C6ADCD}", AutoMap = true)]
    public class Consumption : GlassBase
    {
        public virtual string From { get; set; }
        public virtual string To { get; set; }
        public virtual string Tariff { get; set; }
    }
}