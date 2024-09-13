using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;

namespace DEWAXP.Feature.CommonComponents.Models.Header
{
    public class NotificationModel
    {
        [SitecoreField("Icon")]
        public virtual NotificationIcon Icon { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
       public virtual DateTime StartTime { get; set; }
       public virtual DateTime EndTime { get; set; }

	
		[SitecoreField(FieldName = "Display Globally")]
		public virtual bool Enabled { get; set; }

        [SitecoreField("Time Period")]
        public virtual string TimePeriod { get; set; }

        [SitecoreField("Recurring Time")]
        public virtual string RecurringTime { get; set; }

        [SitecoreField(FieldName = "NotificationLink")]
       public virtual Link NotificationLink { get; set; }

		[SitecoreField(FieldName = "NotificationType")]
		public virtual string NotificationType { get; set; }

		[SitecoreField(FieldName = "Show Permenant")]
		public virtual bool ShowPermenant { get; set; }

		[SitecoreField(FieldName = "Show Close Button")]
		public virtual bool ShowCloseButton { get; set; }
	}
    [SitecoreType(TemplateId = "{F0BCCAA8-8B8F-435C-AF43-9452CC814AAC}", AutoMap = true)]
    public class NotificationIcon
    {
        [SitecoreField("Value")]
        public virtual string Value { get; set; }
    }
}