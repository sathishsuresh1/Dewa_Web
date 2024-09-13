using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Footer
{
	public class M74ExpanderSet
	{
        [SitecoreField("Title")]
        public virtual string Title { get; set; }
        [SitecoreField("SubTitle")]
        public virtual string SubTitle { get; set; }
        [SitecoreField("MainImage")]
        public virtual Image MainImage { get; set; }
        [SitecoreField("Link")]
        public virtual Link Link { get; set; }
        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<M74ExpanderItem> ExpanderList { get; set; }
    }
    public class M74ExpanderItem
    {
        [SitecoreField("Title")]
        public virtual string Title { get; set; }
        [SitecoreField("Link")]
        public virtual Link Link { get; set; }
        [SitecoreField("Icon class")]
        public virtual string Iconclass { get; set; }
    }
}