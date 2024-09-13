using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.RenderingParameters
{
    [SitecoreType(TemplateId = "{FA219CAA-8847-4F85-A6DA-983BA2A0EF37}", AutoMap = true)]
    public class TeaserRenderingParameter : GlassBase
    {
        [SitecoreField("Enable News Teaser Style")]
        public virtual bool EnableNewsTeaserStyle { get; set; }

        [SitecoreField("Enable Board Teaser Style")]
        public virtual bool EnableBoardTeaserStyle { get; set; }
    }
}