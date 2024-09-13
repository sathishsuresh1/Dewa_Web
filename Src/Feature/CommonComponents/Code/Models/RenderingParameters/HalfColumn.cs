using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.RenderingParameters
{
    [SitecoreType(TemplateId = "{B4FAAE9A-C9D6-4259-A368-0729B0565DF0}", AutoMap = true)]
    public class FormattedTextSettings : GlassBase
    {
        public virtual bool DisplayInHalfColumn { get; set; }
        public virtual bool DisplayRawNoGrid { get; set; }
        public virtual bool DisplayRaw { get; set; }
    }
}