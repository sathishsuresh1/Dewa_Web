using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.IdealHomeConsumer
{
    [SitecoreType(TemplateId = "{031C88C5-02CB-4137-B79C-53F705E2004E}", AutoMap = true)]
     public class VideoModelItem : GlassBase
    {
        #region VideoModelItem
        [SitecoreField("Video Title")]
        public virtual string VTitle { get; set; }
        [SitecoreField("Video URL")]
        public virtual string VURL { get; set; }
        [SitecoreField("Video Length")]
        public virtual string VLength { get; set; }
        [SitecoreField("Video Back Image")]
        public virtual Image VBackImage { get; set; }
        #endregion
    }
}