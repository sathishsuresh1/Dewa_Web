using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Media
{
    [SitecoreType(TemplateId = "{74C6B3DB-3B90-4878-BE64-2147B5F4EF1B}", AutoMap = true)]
    public class VideoFolder
    {
        [SitecoreChildren]
        public virtual IEnumerable<Video> Children { get; set; }
    }

    [SitecoreType(TemplateId = "{F3163199-BFD3-4115-9A98-63471863B658}", AutoMap = true)]
    public class Video
    {
        public virtual string Source { get; set; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        public virtual string DisplayName { get; set; }

        [SitecoreField("Video Description", FieldType = SitecoreFieldType.SingleLineText)]
        public virtual string Description { get; set; }
    }
}