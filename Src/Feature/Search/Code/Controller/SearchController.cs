using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content;
using Glass.Mapper.Sc;
using DEWAXP.Foundation.Helpers;
using Sitecore.Mvc.Configuration;
using Sitecore.Data.Items;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Feature.Search.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using SearchResult = DEWAXP.Feature.Search.Models.SearchResult;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SolrNetExtension;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Links;
using Sitecore;
using static Sitecore.Configuration.Settings;
using DEWAXP.Foundation.Logger;
using Categories = DEWAXP.Feature.Search.Models.Categories;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.CustomDB.DataModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DEWAXP.Feature.Search.Services;


namespace DEWAXP.Feature.Search.Controllers
{
    public class SearchController : BaseController
    {
        #region [variable]
        const int DefaultPageSize = 20; const string indexName = "custom_web_search_index"; const string searchCategoryFieldName = "search_category";
        const string searchCacheKey = "j32search";

        private SearchConfigItem ConfigItem => ContentRepository.GetItem<SearchConfigItem>(new GetItemByPathOptions("/sitecore/content/Global Config/Search Config")) ?? new SearchConfigItem() { SearchResultsPageSize = 20, SuggestionListSize = 5 };
        #endregion
        public ActionResult Submit(string term, int page = 1, int pageSize = DefaultPageSize, string filter = "All")
        {
            var searchPage = ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.SITE_SEARCH_RESULTS)));
            var url = LinkManager.GetItemUrl(searchPage, LinkOptions.Url);

            return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = url.TrimStart(new[] { '/' }), term, page, filter, pageSize });
        }

        public PartialViewResult Results(string term, int category, int page)
        {

            term = System.Web.HttpUtility.HtmlEncode(term);
            InstantSearchViewModel storedval = GetResults(term.Trim(), category, page);
            SiteSearch.TrackSiteSearch(ContextRepository.GetCurrentItem<Item>(), term);
            InstantSearchViewModel retval = new InstantSearchViewModel()
            {
                ContentPageCount = storedval.ContentPageCount,
                CurrentPage = page,
                Term = term,
                PageSize = ConfigItem.SearchResultsPageSize,
                SearchResultPageUrl = LinkHelper.GetItemUrl(Context.Item) + $"?term={term}",
                PopularSearchLinks = ContentRepository.GetItem<LinkContainer>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.GLOBAL_Top_Search))),
                NewsPageCount = storedval.NewsPageCount,
                ServicesPageCount = storedval.ServicesPageCount,
                SearchTime = storedval.SearchTime,
                TotalRecordsFound = storedval.TotalRecordsFound,
                Services = storedval.Services,//storedval.Services.Take(ConfigItem.SearchResultsPageSize).ToList(),
                News = storedval.News,//storedval.News.Take(ConfigItem.SearchResultsPageSize).ToList(),
                Content = storedval.Content,//storedval.Content.Take(ConfigItem.SearchResultsPageSize).ToList(),
                ServicesCount = storedval.ServicesCount, //storedval.Services != null ? storedval.Services.Count : 0,
                ContentCount = storedval.ContentCount, //storedval.Content != null ? storedval.Content.Count : 0,
                NewsCount = storedval.NewsCount, //storedval.News != null ? storedval.News.Count : 0
                totalSkip = storedval.totalSkip,
                PageCount = page
            };

            switch (category)
            {
                case 1:
                    retval.CurrentCategory = Categories.Services;
                    //if (page > 1) { retval.Services = storedval.Services.Skip(ConfigItem.SearchResultsPageSize * (page - 1)).Take(ConfigItem.SearchResultsPageSize).ToList(); }
                    break;
                case 2:
                    retval.CurrentCategory = Categories.News;
                    // if (page > 1) { retval.News = storedval.News.Skip(ConfigItem.SearchResultsPageSize * (page - 1)).Take(ConfigItem.SearchResultsPageSize).ToList(); }
                    break;
                case 3:
                    retval.CurrentCategory = Categories.Content;
                    // if (page > 1) { retval.Content = storedval.Content.Skip(ConfigItem.SearchResultsPageSize * (page - 1)).Take(ConfigItem.SearchResultsPageSize).ToList(); }
                    break;
                default:
                    retval.CurrentCategory = Categories.All;
                    break;
            }

            return PartialView("~/Views/Feature/Search/Listing/_SearchResults.cshtml", retval);
        }


        [ValidateAntiForgeryToken]
        public ActionResult SearchSuggestion(string term, string item)
        {
            InstantSearchViewModel retval = GetResultsSuggestion(term.Trim(), -1, 1);
            var config = ConfigItem;
            
            var services = retval.Services.Take(config.SuggestionListSize).Select(s => new SearchItem() { Title = s.Title, Url = s.Url, Description = s.Description }).ToList();
            var news = retval.News.Take(config.SuggestionListSize).Select(s => new SearchItem() { Title = s.Title, Url = s.Url, Description = s.Description }).ToList();
            var contents = retval.Content.Take(config.SuggestionListSize).Select(s => new SearchItem() { Title = s.Title, Url = s.Url, Description = s.Description }).ToList();

            var itemid = !string.IsNullOrWhiteSpace(item) && Guid.TryParse(item, out Guid result) ? ContentRepository.GetItem<Item>(new GetItemByIdOptions(result)) : ContentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.SITE_SEARCH_RESULTS)));
            SiteSearch.TrackSiteSearch(itemid, term);

            return Json(new { RecordsFound = retval.TotalRecordsFound, Services = services ?? new List<SearchItem>(), News = news ?? new List<SearchItem>(), Content = contents ?? new List<SearchItem>(), MoreServices = retval.Services.Count > config.SuggestionListSize, MoreNews = retval.News.Count > config.SuggestionListSize, MoreContent = retval.Content.Count > config.SuggestionListSize }, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveFeedback(string term, bool didUFind, string feedback)
        {
            try
            {
                using (var db = new Entities())
                {
                    db.WebsiteSearchFeedbacks.Add(new WebsiteSearchFeedback() { DidYouFind = didUFind, SearchDate = DateTime.Now, SearchTerm = System.Web.HttpUtility.HtmlEncode(term), Feedback = string.IsNullOrEmpty(feedback) ? string.Empty : System.Web.HttpUtility.HtmlEncode(feedback) });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        #region Search Suggestions


        private InstantSearchViewModel GetResults(string term, int categoryNo, int pageNo)
        {
            int totalTake = 20;
            int totalSkip = (pageNo - 1) * totalTake;


            InstantSearchViewModel retval; // = new InstantSearchViewModel();
            if (CacheProvider.TryGet<InstantSearchViewModel>(searchCacheKey, out retval))
            {
                if (!string.IsNullOrEmpty(retval.Term) && retval.Term.Equals(term) && pageNo == retval.totalSkip && retval.Language.Equals(RequestLanguage)) { return retval; }
            }

            retval = new InstantSearchViewModel() { Language = RequestLanguage };
            retval.totalSkip = pageNo;
            try
            {


                ISearchIndex index = ContentSearchManager.GetIndex(indexName);
                using (IProviderSearchContext context = index.CreateSearchContext())
                {
                    //FacetResults facets;
                    //FacetResults fr = new FacetResults();
                    //Dictionary<string, int> dictionary = new Dictionary<string, int>();
                    //Dictionary<string, int> dictionary2 = new Dictionary<string, int>();                    
                    DateTime startTime = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(term))
                    {
                        term = term.Trim();
                        var query = PredicateBuilder.True<SearchResult>()
                            .And(sr => sr.Header.Contains(term))
                            .Or(sr => sr.BrowserTitle.Contains(term))
                            .Or(sr => sr.DisplayName.Contains(term))
                            .Or(sr => sr.Description.Contains(term))
                            .Or(sr => sr.Content.Contains(term))
                            .Or(sr => sr.Summary.Contains(term))
                            .Or(sr => sr.MetaKeywords.Contains(term))
                            .Or(sr => sr.MetaDescription.Contains(term));


                        IQueryable<SearchResult> result = context.GetQueryable<SearchResult>()
                            .Where(sr => sr.Language == global::Sitecore.Context.Language.Name)
                            .Where(sr => !sr.ShouldBeExcluded)
                            .Where(query).FacetOn(f => f.categories, 0);

                        List<SearchResult> countResult = result.ToList();

                        if (countResult != null && countResult.Count > 0)
                        {

                            retval.NewsCount = countResult.Where(x => x.categories == "News").Count() > 0 ? countResult.Where(x => x.categories == "News").Count() : 0;
                            retval.ServicesCount = countResult.Where(x => x.categories == "Service").Count() > 0 ? countResult.Where(x => x.categories == "Service").Count() : 0;
                            retval.ContentCount = countResult.Where(x => x.categories != "Service" && x.categories != "News").Count() > 0 ? countResult.Where(x => x.categories != "Service" && x.categories != "News").Count() : 0;

                            retval.TotalRecordsFound = retval.NewsCount + retval.ServicesCount + retval.ContentCount;


                            retval.ServicesPageCount = InstantSearchViewModel.GetPageCountInt(retval.ServicesCount, ConfigItem.SearchResultsPageSize);
                            retval.NewsPageCount = InstantSearchViewModel.GetPageCountInt(retval.NewsCount, ConfigItem.SearchResultsPageSize);
                            retval.ContentPageCount = InstantSearchViewModel.GetPageCountInt(retval.ContentCount, ConfigItem.SearchResultsPageSize);
                        }

                        List<SearchResult> filteredResult = new List<SearchResult>();


                        List<SearchResult> resultNews = result.Where(x => x.categories == "News").OrderByDescending(y => y.PublishDate).Skip(totalSkip).Take(totalTake).ToList();

                        List<SearchResult> resultService = result.Where(x => x.categories == "Service").Skip(totalSkip).Take(totalTake).ToList();

                        List<SearchResult> resultContent = result.Where(x => x.categories != "Service" && x.categories != "News").Skip(totalSkip).Take(totalTake).ToList();

                        if (resultNews != null)
                        {
                            filteredResult.AddRange(resultNews);
                        }

                        if (resultService != null)
                        {
                            filteredResult.AddRange(resultService);
                        }

                        if (resultContent != null)
                        {
                            filteredResult.AddRange(resultContent);
                        }

                        retval.SolrSearchTime = (DateTime.Now - startTime).TotalSeconds;

                        foreach (var item in filteredResult)
                        {
                            if (string.IsNullOrEmpty(item.categories)) continue;



                            SearchItem si = new SearchItem();

                            si.Title = item.Header ?? item.MenuLabel ?? item.BrowserTitle;

                            //if (string.IsNullOrEmpty(si.Title)) continue;

                            si.Url = LinkHelper.GetItemUrl(item.ItemId.Guid.ToString(), false);

                            si.Date = item.PublishDate.ToString("dd MMM yyyy", global::Sitecore.Context.Language.CultureInfo).Replace("يوليه", "يوليو"); ;
                            si.Description = string.IsNullOrEmpty(item.Summary) ? (string.IsNullOrEmpty(item.Description) ? (item.MetaDescription) : item.Description) : item.Summary;

                            switch (item.categories.Trim().ToLower())
                            {
                                case "service":
                                    retval.Services.Add(si);
                                    //retval.TotalRecordsFound++;
                                    break;
                                case "news":
                                    retval.News.Add(si);
                                    // retval.TotalRecordsFound++;
                                    break;
                                default:    //content and initiatives
                                    retval.Content.Add(si);
                                    // retval.TotalRecordsFound++;
                                    break;
                            }
                        }

                        //retval.News = retval.News.OrderByDescending(x => x.Date).ToList();

                        retval.Term = term; retval.SearchTime = Math.Round((DateTime.Now - startTime).TotalSeconds, 2);
                        // retval.ServicesPageCount = InstantSearchViewModel.GetPageCount(retval.Services, ConfigItem.SearchResultsPageSize);
                        // retval.NewsPageCount = InstantSearchViewModel.GetPageCount(retval.News, ConfigItem.SearchResultsPageSize);
                        // retval.ContentPageCount = InstantSearchViewModel.GetPageCount(retval.Content, ConfigItem.SearchResultsPageSize);

                        CacheProvider.Store(searchCacheKey, new CacheItem<InstantSearchViewModel>(retval, TimeSpan.FromMinutes(5)));
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                throw ex;
            }

            return retval ?? new InstantSearchViewModel();
        }

        private InstantSearchViewModel GetResultsSuggestion(string term, int categoryNo, int pageNo)
        {
            int totalTake = 10;
            int totalSkip = (pageNo - 1) * totalTake;


            InstantSearchViewModel retval; // = new InstantSearchViewModel();
            if (CacheProvider.TryGet<InstantSearchViewModel>(searchCacheKey, out retval))
            {
                if (!string.IsNullOrEmpty(retval.Term) && retval.Term.Equals(term) && retval.Language.Equals(RequestLanguage)) { return retval; }
            }

            retval = new InstantSearchViewModel() { Language = RequestLanguage };
            retval.totalSkip = 0;
            try
            {


                ISearchIndex index = ContentSearchManager.GetIndex(indexName);
                using (IProviderSearchContext context = index.CreateSearchContext())
                {
                    //FacetResults facets;
                    //FacetResults fr = new FacetResults();
                    //Dictionary<string, int> dictionary = new Dictionary<string, int>();
                    //Dictionary<string, int> dictionary2 = new Dictionary<string, int>();                    
                    DateTime startTime = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(term))
                    {
                        term = term.Trim();
                        var query = PredicateBuilder.True<SearchResult>()
                            .And(sr => sr.Header.Contains(term))
                            .Or(sr => sr.BrowserTitle.Contains(term))
                            .Or(sr => sr.DisplayName.Contains(term))
                            .Or(sr => sr.Description.Contains(term))
                            .Or(sr => sr.Content.Contains(term))
                            .Or(sr => sr.Summary.Contains(term))
                            .Or(sr => sr.MetaKeywords.Contains(term))
                            .Or(sr => sr.MetaDescription.Contains(term));


                        IQueryable<SearchResult> result = context.GetQueryable<SearchResult>()
                            .Where(sr => sr.Language == global::Sitecore.Context.Language.Name)
                            .Where(sr => !sr.ShouldBeExcluded)
                            .Where(query).FacetOn(f => f.categories, 0);


                        List<SearchResult> countResult = result.ToList();

                        if (countResult != null && countResult.Count > 0)
                        {

                            retval.NewsCount = countResult.Where(x => x.categories == "News").Count() > 0 ? countResult.Where(x => x.categories == "News").Count() : 0;
                            retval.ServicesCount = countResult.Where(x => x.categories == "Service").Count() > 0 ? countResult.Where(x => x.categories == "Service").Count() : 0;
                            retval.ContentCount = countResult.Where(x => x.categories != "Service" && x.categories != "News").Count() > 0 ? countResult.Where(x => x.categories != "Service" && x.categories != "News").Count() : 0;

                            retval.TotalRecordsFound = retval.NewsCount + retval.ServicesCount + retval.ContentCount;

                        }

                        List<SearchResult> filteredResult = new List<SearchResult>();


                        List<SearchResult> resultNews = result.Where(x => x.categories == "News").OrderByDescending(y => y.PublishDate).Skip(totalSkip).Take(totalTake).ToList();

                        List<SearchResult> resultService = result.Where(x => x.categories == "Service").Skip(totalSkip).Take(totalTake).ToList();

                        List<SearchResult> resultContent = result.Where(x => x.categories != "Service" && x.categories != "News").Skip(totalSkip).Take(totalTake).ToList();

                        if (resultNews != null)
                        {
                            filteredResult.AddRange(resultNews);
                        }

                        if (resultService != null)
                        {
                            filteredResult.AddRange(resultService);
                        }

                        if (resultContent != null)
                        {
                            filteredResult.AddRange(resultContent);
                        }

                        retval.SolrSearchTime = (DateTime.Now - startTime).TotalSeconds;

                        foreach (var item in filteredResult)
                        {
                            if (string.IsNullOrEmpty(item.categories)) continue;



                            SearchItem si = new SearchItem();

                            si.Title = item.Header ?? item.MenuLabel ?? item.BrowserTitle;

                            //if (string.IsNullOrEmpty(si.Title)) continue;

                            si.Url = LinkHelper.GetItemUrl(item.ItemId.Guid.ToString(), false);

                            si.Date = item.PublishDate.ToString("dd MMM yyyy", global::Sitecore.Context.Language.CultureInfo).Replace("يوليه", "يوليو"); ;
                            si.Description = string.IsNullOrEmpty(item.Summary) ? (string.IsNullOrEmpty(item.Description) ? (item.MetaDescription) : item.Description) : item.Summary;

                            switch (item.categories.Trim().ToLower())
                            {
                                case "service":
                                    retval.Services.Add(si);
                                    //retval.TotalRecordsFound++;
                                    break;
                                case "news":
                                    retval.News.Add(si);
                                    // retval.TotalRecordsFound++;
                                    break;
                                default:    //content and initiatives
                                    retval.Content.Add(si);
                                    // retval.TotalRecordsFound++;
                                    break;
                            }
                        }

                        //retval.News = retval.News.OrderByDescending(x => x.Date).ToList();

                        retval.Term = term; retval.SearchTime = Math.Round((DateTime.Now - startTime).TotalSeconds, 2);
                        // retval.ServicesPageCount = InstantSearchViewModel.GetPageCount(retval.Services, ConfigItem.SearchResultsPageSize);
                        // retval.NewsPageCount = InstantSearchViewModel.GetPageCount(retval.News, ConfigItem.SearchResultsPageSize);
                        // retval.ContentPageCount = InstantSearchViewModel.GetPageCount(retval.Content, ConfigItem.SearchResultsPageSize);

                        CacheProvider.Store(searchCacheKey, new CacheItem<InstantSearchViewModel>(retval, TimeSpan.FromMinutes(5)));
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                throw ex;
            }

            return retval ?? new InstantSearchViewModel();
        }


        #endregion

    }
}