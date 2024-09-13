using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Header
{
	public class Notification
	{
		public virtual string Text { get; set; }
        [SitecoreField(FieldName = "NotificationLink")]
	   public virtual Link NotificationLink { get; set; }
	}
}