using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models.Common
{
	[SitecoreType(TemplateId = "{59F89E22-0868-4682-A0F9-7CB3AA5097DB}", AutoMap = true)]
	public class SmartResponseWaterModel
	{
		[SitecoreField("Water Usage Count")]
		public virtual string WaterUsageCount { get; set; }

        [SitecoreField(FieldName = "IsCheckedActive")]
        public virtual bool IsCheckedActive { get; set; }

    }
}