using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    public class OtherMenu : ContentBase
    {
        /// <summary>
        /// Title
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }
        /// <summary>
        /// Icon
        /// </summary>
        [SitecoreField("Icon")]
        public virtual string Icon { get; set; }
        //Other MenuList/// <summary>
        /// Menu List
        /// </summary>
        [SitecoreField("Other MenuList")]
        public virtual IEnumerable<OtherSubMenu> OtherMenuList { get; set; }
    }
}