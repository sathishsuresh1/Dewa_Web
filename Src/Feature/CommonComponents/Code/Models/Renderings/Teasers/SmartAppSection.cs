using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    [SitecoreType(TemplateName = "SmartAppSection", TemplateId = "{CE3E42A4-4867-4E95-9AA7-2360C6674E51}", AutoMap = true)]
    public class SmartAppSection : ContentBase
    {
        /// <summary>
        /// Header
        /// </summary>
        [SitecoreField("Header")]
        public virtual string Header { get; set; }

        /// <summary>
        /// SubHeader
        /// </summary>
        [SitecoreField("SubHeader")]
        public virtual string SubHeader { get; set; }

        /// <summary>
        /// Main Image
        /// </summary>
        [SitecoreField("Main Image")]
        public virtual Image MainImage { get; set; }

        /// <summary>
        /// App Link Image
        /// </summary>
        [SitecoreField("App Link Image")]
        public virtual IEnumerable<LinkedImage> AppLinkImage { get; set; }
    }
}