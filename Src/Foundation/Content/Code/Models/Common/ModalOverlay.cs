using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models.Common
{
	public class ModalOverlay : ContentBase
	{
		[SitecoreField("Trigger Text")]
		public virtual string TriggerText { get; set; }

		public virtual string Header { get; set; }
		[SitecoreField("Close Text")]
		public virtual string CloseText { get; set; }
		public virtual string Content { get; set; }
	}
}