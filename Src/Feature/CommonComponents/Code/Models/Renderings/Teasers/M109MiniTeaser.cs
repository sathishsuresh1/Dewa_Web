using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    [SitecoreType(TemplateName = "M109MiniTeaserData", TemplateId = "{34EFB76A-F766-444D-9B5E-E046C30A3D5E}", AutoMap = true)]
    public class M109MiniTeaser
    {
        [SitecoreField("Teaser Header")]
        public virtual string TeaserHeader { get; set; }

        [SitecoreField("Teaser Sub Header")]
        public virtual string TeaserSubHeader { get; set; }

        [SitecoreChildren]
        public virtual IEnumerable<M109MiniTeaserItem> Children { get; set; }
    }

    [SitecoreType(TemplateName = "M109MiniTeaserItem", TemplateId = "{28F17C30-F4A6-4E89-8EA1-9B1CA116EE27}", AutoMap = true)]
    public class M109MiniTeaserItem
    {
        [SitecoreField("Teaser Title")]
        public virtual string TeaserTitle { get; set; }

        [SitecoreField("Icon Type")]
        public virtual string IconType { get; set; }

        [SitecoreField("Teaser Link")]
       public virtual Link TeaserLink { get; set; }
    }
}