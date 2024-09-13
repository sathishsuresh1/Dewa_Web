using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.IdealHomeConsumer
{
    [SitecoreType(TemplateId = "{CA1260AC-91D4-41BF-AF22-19126918B0A8}", AutoMap = true)]
    public class VideoModelSet : GlassBase
    {
        [SitecoreField("Header")]
        public virtual string Header { get; set; }
        public virtual IEnumerable<VideoModelItem> Children { get; set; }
    }
}