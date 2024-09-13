using System.Collections.Generic;
using System.Web;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Linq;
using Sitecore.Globalization;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    public class YoutubeVideoPlaylists : SectionTitle
    {
        public virtual IEnumerable<YoutubeVideoPlaylist> Playlists { get; set; }
    }

    [SitecoreType(TemplateId = "{3633B1F2-77E1-425A-B72D-339E74FF285A}", AutoMap = true)]
    public class YoutubeVideoPlaylist : SectionTitle
    {
        [SitecoreChildren]
        public virtual IEnumerable<YoutubeVideo> Videos { get; set; }

        [SitecoreField(FieldName = "Image Link")]
        public virtual string ImageLink { get; set; }

        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }

        [SitecoreField(FieldName = "Playlist Id")]
        public virtual string PlaylistId { get; set; }

        public virtual string VideoPlaylistPageLink { get; set; }

        [SitecoreField(FieldName = "Deactivate")]
        public virtual bool Deactivate { get; set; }

        public virtual string ToJson()
        {
            var listObj = new VideoListJson();
            listObj.list = this.Videos.Select(x => new List()
            {
                desc = x.Description,
                DoP = x.FormattedPublishedDate,
                id = x.VideoId,
                img = x.ImageLink,
                len = string.Format("{0} {1}", x.VideoLength, string.IsNullOrEmpty(x.VideoLength) ? "" : Translate.Text("YT min")),
                title = x.Title
            }).ToList();

            return Newtonsoft.Json.JsonConvert.SerializeObject(listObj);

        }

        [SitecoreField("Page Size")]
        public virtual int PageSize { get; set; }

        public virtual PaginationModel Pagination { get; set; }
    }

    [SitecoreType(TemplateId = "{DF23C99A-FC5A-455E-BB8F-0DA9DC596786}", AutoMap = true)]
    public class YoutubeVideo
    {
        [SitecoreField(FieldName = "Image Link")]
        public virtual string ImageLink { get; set; }

        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }

        [SitecoreField(FieldName = "Video Id")]
        public virtual string VideoId { get; set; }

        [SitecoreField(FieldName = "Deactivate")]
        public virtual bool Deactivate { get; set; }

        [SitecoreField(FieldName = "Description")]
        public virtual string Description { get; set; }

        [SitecoreField(FieldName = "Published")]
        public virtual string Published { get; set; }

        [SitecoreField(FieldName = "VideoLength")]
        public virtual string VideoLength { get; set; }

        public virtual string FormattedPublishedDate
        {
            get
            {
                if (string.IsNullOrEmpty(this.Published)) { return string.Empty; }
                System.DateTime dt = System.DateTime.Now;

                if (System.DateTime.TryParse(this.Published, out dt))
                {
                    return dt.ToString("MMM d, yyyy");
                }
                return string.Empty;
            }
        }

        #region [Additional Info]
        [SitecoreField("Album Category")]
        public virtual BaseDataSourceValue AlbumCategory { get; set; }
        #endregion
    }

    public class List
    {
        public virtual string id { get; set; }
        public virtual string img { get; set; }
        public virtual string title { get; set; }
        public virtual string desc { get; set; }
        public virtual string DoP { get; set; }
        public virtual string len { get; set; }
    }
    public class VideoListJson
    {
       public virtual List<List> list { get; set; }
    }

    
}