using System.Collections.Generic;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Header
{
	[SitecoreType(TemplateId = "{889737FA-BB57-49CF-BFBB-A12FCB1EB03B}", AutoMap = true)]
    public class SublandingA : SublandingBase
	{
	}

	[SitecoreType(TemplateId = "{0A88F6DB-BEA5-413D-A7AE-C52B710E1300}", AutoMap = true)]
    public class SublandingB : SublandingBase
	{
	}

	[SitecoreType(TemplateId = "{D76D1566-230A-4DCE-8F50-1B87657B7E25}", AutoMap = true)]
    public class SublandingC : SublandingBase
	{
	}

    [SitecoreType(TemplateId = "{939234A8-2467-4237-BBA5-6813E7788C34}", AutoMap = true)]
    public class SublandingPage : SublandingBase
    {
    }
	
	[SitecoreType(TemplateId = "{5BA1EBDF-5F97-4A4A-A23D-4D6B1ACB5319}", AutoMap = true)]
	public class SublandingBase : PageBase
	{
		[SitecoreChildren(InferType = true)]
		public virtual IEnumerable<ContentBase> Children { get; set; }
	}

    [SitecoreType(TemplateId = "{3F4E3AAF-8E72-419A-92B6-666F716BE928}", AutoMap = true)]
    public class SublandingFolder : ContentBase
    {
        //[SitecoreField("Name")]
        //public virtual string Name { get; set; }

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<ContentBase> Children { get; set; }
    }
}
