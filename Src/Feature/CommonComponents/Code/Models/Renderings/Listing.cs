using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class Listing : PageBase
	{
		[SitecoreField("Page Size")]
		public virtual int PageSize { get; set; }

		public virtual PaginationModel Pagination { get; set; }
	}
}