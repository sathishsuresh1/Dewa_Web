using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using System;

namespace DEWAXP.Feature.Search.Models
{
    public class SearchResult : SearchResultItem
    {
        [IndexField("__Display name")]
        public virtual string DisplayName { get; set; }

        [IndexField("Browser Title")]
        public virtual string BrowserTitle { get; set; }

        [IndexField("Header")]
        public virtual string Header { get; set; }

        [IndexField("Summary")]
        public virtual string Summary { get; set; }

        [IndexField("Description")]
        public virtual string Description { get; set; }

        [IndexField("Exclude from search results")]
        public virtual bool ShouldBeExcluded { get; set; }

        [IndexField("Menu Label")]
        public virtual string MenuLabel { get; set; }

        [IndexField("Search Category")]
        public virtual string categories { get; set; }

        [IndexField("Meta Keywords")]
        public virtual string MetaKeywords { get; set; }

        [IndexField("Meta Description")]
        public virtual string MetaDescription { get; set; }

        [IndexField("Publish Date")]
        public virtual DateTime PublishDate { get; set; }

        //[IgnoreIndexField]
        //public System.Collections.Generic.List<BreadCrumbItemViewModel> Breadcrumb { get; set; }
    }
}