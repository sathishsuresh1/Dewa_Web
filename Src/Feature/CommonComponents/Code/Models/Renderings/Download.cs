using Glass.Mapper.Sc.Fields;
using Sitecore.Data;
using Sitecore.Data.Items;
using Context = Sitecore.Context;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class Download
	{
		public virtual string Text { get; set; }
		public virtual string Icon { get; set; }
		public virtual File File { get; set; }
		public virtual MediaItem FileMediaItem
		{
			get
			{
				if (File == null) return null;
				var mediaId = File.Id;
				var item = Context.Database.GetItem(new ID(mediaId));
				return item;
			}
		}
	}
}