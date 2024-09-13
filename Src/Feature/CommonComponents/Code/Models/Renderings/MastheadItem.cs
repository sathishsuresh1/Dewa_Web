using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    public class MastheadItem : ContentBase
    {
        //public MastheadItem() {
        //    this.QuickMenuLinks = new List<ContentBase>();
        //    this.ParsonaMenuList = new List<Persona.PersonaCategory>();
        //}
        #region [Content Logo Data]

        /// <summary>
        ///Top Left Logo Image 
        /// </summary>
        [SitecoreField("Top Left Logo Image")]
        public virtual Image TopLeftLogoImage { get; set; }
        [SitecoreField("Top Left Logo Image Dark")]
        public virtual Image TopLeftLogoImageDark { get; set; }
        /// <summary>
        /// Top Left Logo PageUrl
        /// </summary>
        [SitecoreField("Top Left Logo PageUrl")]
       public virtual Link TopLeftLogoPageUrl { get; set; }
        /// <summary>
        /// Top Center Logo Image
        /// </summary>
        [SitecoreField("Top Center Logo Image")]
        public virtual Image TopCenterLogoImage { get; set; }
        /// <summary>
        /// Top Center Logo PageUrl
        /// </summary>
        [SitecoreField("Top Center Logo PageUrl")]
       public virtual Link TopCenterLogoPageUrl { get; set; }
        /// <summary>
        /// Top Right Logo Image
        /// </summary>
        [SitecoreField("Top Right Logo Image")]
        public virtual Image TopRightLogoImage { get; set; }
        [SitecoreField("Top Right Logo Image Dark")]
        public virtual Image TopRightLogoImageDark { get; set; }
        /// <summary>
        /// Top Right Logo PageUrl
        /// </summary>
        [SitecoreField("Top Right Logo PageUrl")]
       public virtual Link TopRightLogoPageUrl { get; set; }
        #endregion

        #region [Content Menu Data]
        //Quick Menu Links
        [SitecoreField("Quick Menu Links")]
        public virtual IEnumerable<ContentBase> QuickMenuLinks { get; set; }
        //Parsona MenuList
        [SitecoreField("Parsona MenuList")]
        public virtual IEnumerable<Persona.PersonaCategory> ParsonaMenuList { get; set; }
        //Other MenuList
        /// <summary>
        /// Other MenuList
        /// </summary>
        [SitecoreField("Other MenuList")]
        public virtual IEnumerable<OtherMenu> OtherMenuList { get; set; }
        #endregion
        #region [Rammas Content Data]
        /// <summary>
        ///Rammas Image 
        /// </summary>
        [SitecoreField("Rammas Image")]
        public virtual Image RammasImage { get; set; }
        /// <summary>
        /// Rammas Title
        /// </summary>
        [SitecoreField("Rammas Title")]
        public virtual string RammasTitle { get; set; }
        /// <summary>
        /// Rammas  PageUrl
        /// </summary>
        [SitecoreField("Rammas PageUrl")]
       public virtual Link RammasPageUrl { get; set; }

        #endregion
        #region [Other Content Data]
        /// <summary>
        /// CTA Text
        /// </summary>
        [SitecoreField("CTA Text")]
        public virtual string CTAText { get; set; }
        /// <summary>
        /// CTA URL
        /// </summary>
        [SitecoreField("CTA URL")]
       public virtual Link CTAURL { get; set; }
        #endregion
    }
}