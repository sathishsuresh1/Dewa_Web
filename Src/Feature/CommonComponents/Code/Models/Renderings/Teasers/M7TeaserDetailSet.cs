using DEWAXP.Foundation.ORM.Models;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    public class M7TeaserDetailSet : GlassBase
    {
        public virtual string Header { get; set; }
        public virtual IEnumerable<M7TeaserDetail> Children { get; set; }
    }
}