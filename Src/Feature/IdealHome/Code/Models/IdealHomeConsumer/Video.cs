using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;
using System.Web.Mvc;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.IdealHome.Models.IdealHomeConsumer
{

    [SitecoreType(TemplateId = "{194227E7-6861-4DB2-82F1-7E662A674E34}", AutoMap = true)]
    public class VideoList : GlassBase
    {
        [SitecoreField(FieldName = "Video List")]
        public virtual IEnumerable<Video> VideoGallery { get; set; }

        public virtual Video SelectedVideo { get; set; }
        public virtual bool completedVideoSection { get; set; }
    }


    [SitecoreType(TemplateId = "{394DD36A-5A3E-4826-888B-FEF749E48735}", AutoMap = true)]
    public class Video : GlassBase
    {

        [SitecoreField(FieldName = "VideoTitle")]
        public virtual string VideoTitle { get; set; }

        [SitecoreField(FieldName = "VideoDescription")]
        public virtual string VideoDescription { get; set; }

        [SitecoreField(FieldName = "VideoURL")]
        public virtual string VideoURL { get; set; }
        [SitecoreField(FieldName = "VideoLength")]
        public virtual string VideoLength { get; set; }
        [SitecoreField(FieldName = "VideoBackImage")]
        public virtual Image VideoBackImage { get; set; }

        public virtual bool Watched { get; set; }

    }

    [SitecoreType(TemplateId = "{87124854-1533-4D95-A70F-FCE690A70C57}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class VideoResponse : GlassBase
    {
        [SitecoreField(FieldName = "VideoWatched")]
        public virtual System.Collections.Specialized.NameValueCollection NameValue { get; set; }


        [SitecoreField(FieldName = "isCompleted")]
        public virtual bool IsCompleted { get; set; }
    }

    #region Phase 1

    [SitecoreType(TemplateId = "{1DAF4012-46EF-4FEC-9B02-D58E6EE63346}", AutoMap = true)]
    public class VideoGroupList : GlassBase
    {
        [SitecoreField(FieldName = "Video Category List")]
        public virtual IEnumerable<VideoCategoryDataList> VideoCategoryList { get; set; }
    }

    [SitecoreType(TemplateId = "{D73D4C25-DC70-4FAA-9A0F-035C5B2FAD62}", AutoMap = true)]
    public class VideoCategoryDataList : VideoList
    {
        [SitecoreField(FieldName = "Header")]
        public virtual string Header { get; set; }

    }
    #endregion
}