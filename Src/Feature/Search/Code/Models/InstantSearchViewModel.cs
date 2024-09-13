using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.Integration.Enums;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DEWAXP.Feature.Search.Models
{
    public class InstantSearchViewModel
    {
        public InstantSearchViewModel()
        {
            Services = new List<SearchItem>(); News = new List<SearchItem>(); Content = new List<SearchItem>();
        }
        public List<SearchItem> Services { get; set; }
        public int ServicesCount { get; set; }
        public int totalSkip { get; set; }
        public int PageCount { get; set; }
        public List<SearchItem> Content { get; set; }
        public int ContentCount { get; set; }
        public List<SearchItem> News { get; set; }
        public int NewsCount { get; set; }
        public string Term { get; set; }
        public bool IfHitsFound { get { return this.Services.Count > 0 || this.Content.Count > 0 || this.News.Count > 0; } }
        public int TotalRecordsFound { get; set; }
        public Categories CurrentCategory { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SearchResultPageUrl { get; set; }
        public LinkContainer PopularSearchLinks { get; set; }
        public double SearchTime { get; set; }
        public double SolrSearchTime { get; set; }
        public SupportedLanguage Language { get; set; }
        public int ServicesPageCount { get; set; }
        public int NewsPageCount { get; set; }
        public int ContentPageCount { get; set; }
        public static int GetPageCount(List<SearchItem> list, int pageSize)
        {
            if (list != null && list.Count > 0)
            {
                if (list.Count <= pageSize) return 1;
                int rem;
                int pages = Math.DivRem(list.Count, pageSize, out rem);
                pages = pages + (rem > 0 ? 1 : 0);
                return pages;
            }
            return 1;
        }

        public static int GetPageCountInt(int __count, int pageSize)
        {
            if (__count > 0)
            {
                if (__count <= pageSize) return 1;
                int rem;
                int pages = Math.DivRem(__count, pageSize, out rem);
                pages = pages + (rem > 0 ? 1 : 0);
                return pages;
            }
            return 1;
        }

        public static int GetCategoryPageCount(InstantSearchViewModel model, Categories category)
        {
            switch (category)
            {
                case Categories.Services:
                    return model.ServicesPageCount;
                case Categories.News:
                    return model.NewsPageCount;
                case Categories.Content:
                    return model.ContentPageCount;
                default:
                    return 1;
            }
        }
    }

    public class SearchItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }

    public enum Categories
    {
        Services, News, Content, All
    }

    [SitecoreType(TemplateId = "{28291DA0-E161-490C-B39C-1D9B96121EC9}", AutoMap = true)]
    public class SearchConfigItem
    {
        [SitecoreField(FieldName = "Search Suggestion List Size")]
        public virtual int SuggestionListSize { get; set; }

        [SitecoreField(FieldName = "Search Resutls Page Size")]
        public virtual int SearchResultsPageSize { get; set; }

    }
}