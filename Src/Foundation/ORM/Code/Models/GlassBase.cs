using DEWAXP.Foundation.DI;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections;

namespace DEWAXP.Foundation.ORM.Models
{
    [Service(typeof(IGlassBase), Lifetime = Lifetime.Transient)]
    public class GlassBase : IGlassBase
    {
        [SitecoreId]
        public virtual Guid Id { get; set; }

        [SitecoreInfo(SitecoreInfoType.TemplateId)]
        public virtual Guid TemplateId { get; set; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        public virtual string Name { get; set; }

        [SitecoreInfo(SitecoreInfoType.DisplayName)]
        public virtual string DisplayName { get; set; }

        [SitecoreInfo(SitecoreInfoType.TemplateName)]
        public virtual string TemplateName { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url)]
        public virtual string Url { get; set; }

        [SitecoreInfo(SitecoreInfoType.ContentPath)]
        public virtual string ContentPath { get; set; }

        [SitecoreInfo(SitecoreInfoType.Language)]
        public virtual Language Language { get; set; }

        [SitecoreInfo(SitecoreInfoType.Version)]
        public virtual int Version { get; set; }

        [SitecoreItem]
        public virtual Item Item { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url, UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual string FullUrl { get; set; }
        [SitecoreInfo(SitecoreInfoType.BaseTemplateIds)]
        public virtual IEnumerable BaseTemplateIds { get; set; }
    }
}