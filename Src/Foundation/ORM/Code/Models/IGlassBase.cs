using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections;

namespace DEWAXP.Foundation.ORM.Models
{
    public interface IGlassBase
    {
        Guid Id { get; set; }
        Language Language { get; set; }
        int Version { get; set; }
        IEnumerable BaseTemplateIds { get; set; }
        string TemplateName { get; set; }
        Guid TemplateId { get; set; }
        string Name { get; set; }

        [SitecoreInfo(SitecoreInfoType.DisplayName)]
        string DisplayName { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url)]
        string Url { get; set; }

        [SitecoreInfo(SitecoreInfoType.ContentPath)]
        string ContentPath { get; set; }

        [SitecoreItem]
        Item Item { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url, UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        string FullUrl { get; set; }
    }
}