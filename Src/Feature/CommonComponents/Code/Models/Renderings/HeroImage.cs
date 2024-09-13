using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class HeroImage : ContentBase
	{
		public virtual string Header { get; set; }
		public virtual string SubHeader { get; set; }
		public virtual Image Image { get; set; }
	}
}