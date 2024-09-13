using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.CommonComponents.Models.Gallery
{
    public class MediaVidoeGalleryModel
    {
       public virtual List<string> FilterDataList { get; set; }
        public virtual string SearchText { get; set; }
        public virtual string SelectedFilter { get; set; }
        public virtual string DatasourceId { get; set; }

        public virtual string pageNo { get; set; }
       public virtual List<Renderings.YoutubeVideo> Slides { get; set; }

        public PaginationModel PaginationInfo { get; set; }

        public ContentBase CurrentPage { get; set; }

       public virtual List<BaseMediaVideoListItem> AlbumPageList { get; set; }
    }

    public class BaseMediaVideoListItem
    {
        public virtual string Key { get; set; }
       public virtual List<Renderings.YoutubeVideo> MediaItems { get; set; }
    }


}