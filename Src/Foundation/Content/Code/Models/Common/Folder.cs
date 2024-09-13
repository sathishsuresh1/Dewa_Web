using System.Collections.Generic;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Content.Models.Common
{
	[SitecoreType(TemplateName = "Folder", TemplateId = TemplateIdString, AutoMap = true)]
	public class Folder : GlassBase
	{
        public const string TemplateIdString = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}";

        [SitecoreChildren(InferType = true)]
		public virtual IEnumerable<ContentBase> Children { get; set; }
	}
}