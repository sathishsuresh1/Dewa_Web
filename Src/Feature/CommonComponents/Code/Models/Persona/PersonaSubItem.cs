using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Persona
{
    public class PersonaSubItem : GlassBase
    {
        //public PersonaSubItem()
        //{
        //    MenuList = new List<ContentBase>();
        //}
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
        /// <summary>
        /// Menu List
        /// </summary>
        [SitecoreField("Menu List")]
        public virtual IEnumerable<ContentBase> MenuList { get; set; }

        #region [Other]
        /// <summary>
        /// Teaser Header
        /// </summary>
        [SitecoreField("Teaser Header")]
        public virtual string TeaserHeader { get; set; }
        /// <summary>
        /// Teaser Subheader
        /// </summary>
        [SitecoreField("Teaser Subheader")]
        public virtual string TeaserSubheader { get; set; }
        /// <summary>
        /// Teaser Link
        /// </summary>
        [SitecoreField("Teaser Link")]
       public virtual Link TeaserLink { get; set; }
        /// <summary>
        /// Teaser Image
        /// </summary>
        [SitecoreField("Teaser Image")]
        public virtual Image TeaserImage { get; set; }
        /// <summary>
        /// Teaser BackgroundImage
        /// </summary>
        [SitecoreField("Teaser BackgroundImage")]
        public virtual Image TeaserBackgroundImage { get; set; }
        /// <summary>
        /// Text Alighment
        /// </summary>
        [SitecoreField("Text Alignment")]
        public virtual string TextAlignment { get; set; }
        /// <summary>
        /// Activate Teaser
        /// </summary>
        [SitecoreField("Activate Teaser")]
        public virtual string ActivateTeaser { get; set; }

        /// <summary>
        /// Teaser Subfooter
        /// </summary>
        [SitecoreField("Teaser SubFooter")]
        public virtual string TeaserSubfooter { get; set; }

        /// <summary>
        /// Show live data for water, smart response will not show any data
        /// </summary>
        [SitecoreField("Show Live Data")]
        public virtual string ShowLiveData { get; set; }
        #endregion

        #region [Setting Detail]
        /// <summary>
        /// Is Hidden
        /// </summary>
        [SitecoreField("Is Hidden")]
        public virtual string IsHidden { get; set; }

        /// <summary>
        /// Is Default Active
        /// </summary>
        [SitecoreField("Is Default Active")]
        public virtual string IsDefaultAct { get; set; }
        #endregion

        #region [Thumnail Detail]
        [SitecoreField("Thumbnail Image")]
        public virtual Image ThumbnailImage { get; set; }
        #endregion
    }
}