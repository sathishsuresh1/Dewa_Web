using System.Collections.Generic;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class ThumbnailGallery : PageBase
	{
		public virtual IEnumerable<Image> Images { get; set; }
	}
}