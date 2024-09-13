using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    [SitecoreType(TemplateName = "QuickImageLinksSection", TemplateId = "{49A35245-6D72-4753-9383-C49EABA5E694}", AutoMap = true)]
    public class QuickImageLinksSection : ContentBase
    {
        #region [Content Data]

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
        /// Quick Image Link
        /// </summary>
        [SitecoreField("Quick Image Link")]
        public virtual IEnumerable<M7Teaser> QuickM7Teaser { get; set; }

        #endregion [Content Data]

        #region [Common]

        /// <summary>
        /// Background Image
        /// </summary>
        [SitecoreField("Background Image")]
        public virtual Image BackgroundImage { get; set; }

        #endregion [Common]
    }
}