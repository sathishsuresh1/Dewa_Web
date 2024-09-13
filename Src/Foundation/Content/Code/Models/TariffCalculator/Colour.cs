using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models.TariffCalculator
{
    [SitecoreType(TemplateId = "{6E78C716-1369-442C-BB3D-C5B41B9F1E14}", AutoMap = true)]
    public class Colour : GlassBase
    {
        public virtual string CssClass { get; set; }
    }
}