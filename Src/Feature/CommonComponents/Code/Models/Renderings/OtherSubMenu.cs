using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    public class OtherSubMenu : ContentBase
    {
        //public OtherSubMenu()
        //{
        //    MenuList = new List<ContentBase>();
        //}

        /// <summary>
        /// Title
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Menu List
        /// </summary>
        [SitecoreField("Menu List")]
        public virtual IEnumerable<ContentBase> MenuList { get; set; }
    }
}