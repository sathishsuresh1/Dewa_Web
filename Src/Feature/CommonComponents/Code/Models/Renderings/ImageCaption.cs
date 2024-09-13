using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType(TemplateId = "{CF8B25C1-795D-4E21-8072-5D08DF1F3C0F}", AutoMap = true)]
    public class ImageCaption : GlassBase
    {
		public virtual Image Image { get; set; }
		public virtual string Caption { get; set; }
	}
}