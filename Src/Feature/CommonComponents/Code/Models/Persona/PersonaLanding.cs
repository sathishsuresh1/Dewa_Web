using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;
using DEWAXP.Feature.CommonComponents.Models.Renderings;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Persona
{
    /// <summary>
    /// PersonaLanding : {5A16A792-DFD8-45E2-A6A4-9401F6CF213C}
    /// </summary>
	[SitecoreType(TemplateId = "{5A16A792-DFD8-45E2-A6A4-9401F6CF213C}", AutoMap = true)]
	public class PersonaLanding : PageBase
	{
		[SitecoreChildren(InferType = true)]
		public virtual IEnumerable<ContentBase> Children { get; set; }
	}
}