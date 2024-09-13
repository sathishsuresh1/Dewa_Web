using System;
using System.Collections.Generic;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using _scData = Sitecore.Data;
using _scDataItem = Sitecore.Data.Items;
using Context = Sitecore.Context;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    public class CarouselSet : SectionTitle
    {
        public virtual IEnumerable<CarouselSlide> Children { get; set; }

        [SitecoreField("Page Size")]
        public virtual int PageSize { get; set; }

        public PaginationModel Pagination { get; set; }
    }

    public class CarouselSlide : ContentBase
    {
        public virtual string Header { get; set; }
        public virtual string Subheader { get; set; }
        public virtual Image Image { get; set; }

        [SitecoreField("Button Text")]
        public virtual string ButtonText { get; set; }

        [SitecoreField("Button Link")]
       public virtual Link ButtonLink { get; set; }
        [SitecoreField("Download File")]
       public virtual File DownloadFile { get; set; }
        [SitecoreField("Download Button Text")]
        public virtual string DownloadButtonText { get; set; }
        [SitecoreField("Download File AR")]
       public virtual File DownloadFileAR { get; set; }
        
        [SitecoreField("Teaser Icon Name")]
        public virtual string TeaserIconName { get; set; }

        public _scDataItem.MediaItem FileMediaItem
        {
            get
            {
                if (DownloadFile == null) return null;
                var mediaId = DownloadFile.Id;
                _scDataItem.Item item = Context.Database.GetItem(new _scData.ID(mediaId));
                return item;
            }
        }
        public _scDataItem.MediaItem FileMediaItemAR
        {
            get
            {
                if (DownloadFileAR == null) return null;
                var mediaId = DownloadFileAR.Id;
                _scDataItem.Item item = Context.Database.GetItem(new _scData.ID(mediaId));
                return item;
            }
        }

        //Hide File Date
        [SitecoreField("Hide File Date")]
        public virtual bool HideFileDate { get; set; }

        [SitecoreField("Custom Published Date")]
        public virtual DateTime CustomPublishedDate { get; set; }
    }
}