using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    
    public class SocialMediaConfigModel
    {
        [SitecoreField(FieldName = "Twitter Account Name")]
        public string TwitterAccountName { get; set; }

        [SitecoreField(FieldName = "Tweets By")]
        public string TweetsBy { get; set; }

        [SitecoreField(FieldName = "widgetid")]
        public string Widgetid { get; set; }

        [SitecoreField(FieldName = "Youtube Account Name")]
        public string YoutubeAccountName { get; set; }

        [SitecoreField(FieldName = "UserId")]
        public string InstagramUserId { get; set; }

        [SitecoreField(FieldName = "Access Token")]
        public string AccessToken { get; set; }

        
    }
   
}