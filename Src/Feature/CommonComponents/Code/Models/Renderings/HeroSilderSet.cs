using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType(TemplateName = "Hero Silder Set", TemplateId = "{B0BB66FB-4C4F-4BA9-B365-E3EFE990A0EF}", AutoMap = true)]
    public class HeroSilderSet : GlassBase
    {
        public HeroSilderSet()
        {
        }

        /// <summary>
        /// Slide Count Limit
        /// </summary>
        [SitecoreField("Slide Limit Count", FieldId = "{1803129B-BB7C-477F-88C2-DFCBA9E9CF52}")]
        public virtual BaseDataSourceValue SlideLimitCount { get; set; }

        /// <summary>
        /// Hero Slides Children
        /// </summary>
        public virtual IEnumerable<HeroSlidewithCTA> Children { get; set; }
    }
}