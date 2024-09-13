using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Persona
{
    [SitecoreType(TemplateName = "Persona Category", TemplateId = "{DE1796C9-BDF1-42CF-BB09-7E7862954F20}", AutoMap = true)]
    public class PersonaCategory: GlassBase
    {
        //public PersonaCategory() {
        //    PersonaSectionMenuList = new List<PersonaSubItem>();
        //}
        /// <summary>
        /// Title
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        [SitecoreField("Description")]
        public virtual string Description { get; set; }
        /// <summary>
        /// Icon
        /// </summary>
        [SitecoreField("Icon")]
        public virtual string Icon { get; set; }

        /// <summary>
        /// Icon Image
        /// </summary>
        [SitecoreField("Icon Image")]
        public virtual Image IconImage { get; set; }

        /// <summary>
        /// Persona Section MenuList
        /// </summary>
        [SitecoreField("Persona Section MenuList")]
        public virtual IEnumerable<PersonaSubItem> PersonaSectionMenuList { get; set; }

        /// <summary>
        /// Page Link
        /// </summary>
        [SitecoreField("Page Link")]
       public virtual Link PageLink { get; set; }

        #region [Setting Detail]
        /// <summary>
        /// Is Hidden
        /// </summary>
        [SitecoreField("Is Hidden")]
        public virtual string IsHidden { get; set; }
        #endregion
    }
}