using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Breadcrumbs
{
    public class BreadCrumbModel : GlassBase
    {
        [SitecoreField("Menu Label")]
        public virtual string MenuLabel { get; set; }
        [SitecoreParent(InferType = false)]
        public virtual BreadCrumbModel Parent { get; set; }

    }
}