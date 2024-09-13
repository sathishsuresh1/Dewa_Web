using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.ORM.Models
{
    public class ContentBase : GlassBase
    {
        [SitecoreField("Menu Label")]
        public virtual string MenuLabel { get; set; }

        [SitecoreField("Display In Menu")]
        public virtual bool DisplayInMenu { get; set; }

        #region [Persona]

        [SitecoreField("Persona Tag")]
        public virtual BaseDataSourceValue PersonaTag { get; set; }

        #endregion [Persona]

        #region [Additional Info]

        [SitecoreField("Album Category")]
        public virtual BaseDataSourceValue AlbumCategory { get; set; }

        #endregion [Additional Info]
    }
}