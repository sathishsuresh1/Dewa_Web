using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
	public class Teaser : ContentBase
	{
		public virtual string Header { get; set; }
		public virtual string Subheader { get; set; }
        public virtual string Description { get; set; }
		public virtual Link Link { get; set; }
        public virtual string LinkText { get; set; }
        public virtual bool IsHayakTeaser { get; set; }
        public virtual string Icontext { get; set; }
        public virtual Image Image { get; set; }       
        public virtual Image BackgroundImage { get; set; }
        public virtual string ClassName { get; set; }
        public virtual bool HidebyDefault { get; set; }
        public virtual bool OpenInNewTab { get; set; }
    }
}