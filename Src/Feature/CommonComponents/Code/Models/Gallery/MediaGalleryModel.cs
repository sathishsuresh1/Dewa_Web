using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.CommonComponents.Models.Gallery
{
    public class MediaGalleryModel
    {
        public virtual List<string> FilterDataList { get; set; }
        public virtual List<BaseMediaListItem> AlbumPageList { get; set; }
    }


    public class BaseMediaListItem
    {
        public virtual string Key { get; set; }
        public virtual List<Renderings.Teasers.M9Teaser> MediaItems { get; set; }
    }
}