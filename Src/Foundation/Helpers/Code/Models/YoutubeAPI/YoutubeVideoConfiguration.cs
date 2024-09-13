using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.Helpers.Models.YoutubeAPI
{
    [SitecoreType(TemplateId = "{DF01D268-9CB2-46D8-9EAA-46D5546EEFF2}", AutoMap = true)]
    public class YoutubeVideoConfiguration
    {
        [SitecoreField(FieldName = "APIKey")]
        public virtual string APIKey { get; set; }

        [SitecoreField(FieldName = "DEWA Channel English")]
        public virtual string DEWAChannelEnglish { get; set; }

        [SitecoreField(FieldName = "DEWA Channel Arabic")]
        public virtual string DEWAChannelArabic { get; set; }

        [SitecoreField(FieldName = "APIUrl")]
        public virtual string APIUrl { get; set; }

        [SitecoreField(FieldName = "Unlisted Playlist")]
        public virtual string UnlistedPlaylist { get; set; }
    }
    #region Enum Youtube Video Language 
    public enum YoutubeVideoLanguage
    {
        Arabic = 0,
        English = 1
    }
    #endregion
}