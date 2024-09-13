using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType(TemplateName = "Hero Slide", TemplateId = "{E1D4C8FF-FD95-4413-92EE-F8A9F573BC34}", AutoMap = true)]
    public class HeroSlidewithCTA : GlassBase
    {
        #region [Content Data]

        /// <summary>
        /// Image
        /// </summary>
        [SitecoreField("Image")]
        public virtual Image Image { get; set; }

        /// <summary>
        /// Header
        /// </summary>
        [SitecoreField("Header")]
        public virtual string Header { get; set; }
        /// <summary>
        /// Sub Header
        /// </summary>
        [SitecoreField("Sub Header")]
        public virtual string SubHeader { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        [SitecoreField("Description")]
        public virtual string Description { get; set; }
        /// <summary>
        /// IsDisabled
        /// </summary>
        [SitecoreField("IsDisabled")]
        public virtual string IsDisabled { get; set; }
        #endregion

        #region [Content CTA Data]
        /// <summary>
        /// CTA 1 Text
        /// </summary>
        [SitecoreField("CTA 1 Text")]
        public virtual string CTA1Text { get; set; }
        /// <summary>
        /// CTA 1 Link
        /// </summary>
        [SitecoreField("CTA 1 Link")]
       public virtual Link CTA1Link { get; set; }
        /// <summary>
        /// CTA 1 Type
        /// </summary>
        [SitecoreField("CTA 1 Type")]
        public virtual BaseDataSourceValue CTA1Type { get; set; }
        /// <summary>
        /// CTA 2 Text
        /// </summary>
        [SitecoreField("CTA 2 Text")]
        public virtual string CTA2Text { get; set; }
        /// <summary>
        /// CTA 2 Link
        /// </summary>
        [SitecoreField("CTA 2 Link")]
       public virtual Link CTA2Link { get; set; }
        /// <summary>
        /// CTA 2 Type
        /// </summary>
        [SitecoreField("CTA 2 Type")]
        public virtual BaseDataSourceValue CTA2Type { get; set; }
        #endregion

    }
}