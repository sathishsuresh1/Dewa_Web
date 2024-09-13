using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType(TemplateName = "Image Carousel List", TemplateId = "{70D8B870-C916-4C4D-B79D-44FBC786108B}", AutoMap = true)]
    public class ImageCarouselList : GlassBase
    {
        #region [Content Data]

        /// <summary>
        /// IsDisabled
        /// </summary>
        [SitecoreField("LinkedImage")]
        public virtual IEnumerable<LinkedImage> linkedImage { get; set; }

        #endregion [Content Data]
    }
}