using DEWAXP.Feature.CommonComponents.Models.Renderings;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore.Common;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.CommonComponents.Helpers
{
    public static class NewsHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(SitecoreX.Database)));

        //public NewsHelper(IContentRepository contentRepository)
        //{
        //    _contentRepository = contentRepository;
        //}
        public static IEnumerable<Article> GetNewsListing(Guid itemId, out int count)
        {
            var listing = _contentRepository.GetItem<Item>(new GetItemByIdOptions(itemId));

            var children = listing.Axes.GetDescendants()
                .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA")))
                .ToList();

            IEnumerable<Article> filteredSet = children.OrderByDescending(c => c.PublishDate);
            count = children.Count;
            return filteredSet;
        }

        public static IEnumerable<Article> GetNewsListingNew(Guid itemId, int? month, int? year, out int count)//not in use
        {
            var listing = _contentRepository.GetItem<Item>(new GetItemByIdOptions(itemId));
            IEnumerable<Article> filteredSet = null;

            if (month == null && year == null)
            {
                var children = listing.Axes.GetDescendants()
                    .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                    && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                    || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA")))
                    .ToList();

                filteredSet = children.OrderByDescending(c => c.PublishDate);
                count = children.Count;
            }
            else if (month == null && year != null)
            {
                var children = listing.Axes.GetDescendants()
                    .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                    && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                    || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA"))
                    && (c.PublishDate.Year == year))
                    .ToList();

                filteredSet = children.OrderByDescending(c => c.PublishDate);
                count = children.Count;
            }
            else if (month != null && year == null)
            {
                var children = listing.Axes.GetDescendants()
                .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA"))
                && (c.PublishDate.Month == month))
                .ToList();

                filteredSet = children.OrderByDescending(c => c.PublishDate);
                count = children.Count;
            }
            else
            {
                var children = listing.Axes.GetDescendants()
                .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA"))
                && (c.PublishDate.Month == month && c.PublishDate.Year == year))
                .ToList();

                filteredSet = children.OrderByDescending(c => c.PublishDate);
                count = children.Count;
            }
            return filteredSet;
        }

        public static IEnumerable<Article> GetNewsListingIndex(Guid itemId, int month, int year, int skip, int take, out int TotalSkipCount, bool takeall = false)
        {
            var listing = _contentRepository.GetItem<Item>(new GetItemByIdOptions(itemId));

            string ArticleTemplateTypeId = "{960E0516-38BC-495D-A49B-57A9EB0CE1CA}";
            const string indexName = "custom_web_news_index";
            var index = ContentSearchManager.GetIndex(indexName);
            int defaultCount = 100;

            if (take > defaultCount)
            {
                take = defaultCount;
            }

            using (var context = index.CreateSearchContext())
            {
                //string[] Format = new string[] { "M/d/yyyy h:mm:ss tt", "yyyyMMdd'T'HHmmss'Z'", "yyyyMMdd'T'HHmmss" };

                IEnumerable<Article> results = null;
                List<SearchResult> solarSearchResults = null;
                if (month == int.MinValue && year == int.MinValue)
                {
                    if (takeall)
                    {

                        solarSearchResults = context.GetQueryable<SearchResult>(new CultureExecutionContext(SitecoreX.Language.CultureInfo))
                       .Where(x => x.Path.StartsWith(listing.Paths.Path) && x.TemplateId == ID.Parse(ArticleTemplateTypeId)).OrderByDescending(x => x.PublishDate).Take(defaultCount)?.ToList();

                    }
                    else
                    {
                        solarSearchResults = context.GetQueryable<SearchResult>(new CultureExecutionContext(SitecoreX.Language.CultureInfo))
                        .Where(x => x.Path.StartsWith(listing.Paths.Path) && x.TemplateId == ID.Parse(ArticleTemplateTypeId)).OrderByDescending(x => x.PublishDate).Skip(skip).Take(take)?.ToList();
                    }
                }

                else
                {


                    var lastDayOfMonth = DateTime.DaysInMonth(year, month);
                    //get data by month & year 
                    DateTime _toDate = new DateTime(year, month, lastDayOfMonth).AddHours(23).AddMinutes(59).AddSeconds(59); //get the last date of the month
                    DateTime _fromDate = new DateTime(year, month, 1);//get the start date of the month

                    var query = PredicateBuilder.True<SearchResult>()
                              .And(sr => sr.PublishDate > _fromDate)
                              .And(sr => sr.PublishDate < _toDate)
                              .And(sr => sr.Path.StartsWith(listing.Paths.Path))
                              .And(sr => sr.TemplateId == ID.Parse(ArticleTemplateTypeId));

                    if (takeall)
                    {
                        solarSearchResults = context.GetQueryable<SearchResult>(new CultureExecutionContext(SitecoreX.Language.CultureInfo))
                       .Where(query).OrderByDescending(x => x.PublishDate).Take(defaultCount)?.ToList();
                    }
                    else
                    {
                        solarSearchResults = context.GetQueryable<SearchResult>(new CultureExecutionContext(SitecoreX.Language.CultureInfo))
                       .Where(query).OrderByDescending(x => x.PublishDate)?.Skip(skip)?.Take(take)?.ToList();
                    }

                }


                if (solarSearchResults != null && solarSearchResults.Any() && solarSearchResults.Count() > 0)
                {
                    results = solarSearchResults?.Select(y => y.GetItem())?.Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c)));
                }
                else
                {
                    results = new List<Article>();
                }
                TotalSkipCount = skip + results.Count();
                return results;
            }
        }

        public static IEnumerable<Article> GetNewsListing(Guid itemId, out int count, int month, int year)
        {
            var listing = _contentRepository.GetItem<Item>(new GetItemByIdOptions(itemId));
            IEnumerable<Article> filteredSet = null;

            if (month == int.MinValue && year == int.MinValue)
            {
                var children = listing.Axes.GetDescendants()
                    .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                    && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                    || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA")))
                    .ToList();
                filteredSet = children.OrderByDescending(c => c.PublishDate);
                count = children.Count;
            }
            else
            {
                var children = listing.Axes.GetDescendants()
                    .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                    && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                    || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA")) && (c.PublishDate.Month == month && c.PublishDate.Year == year))
                    .ToList();
                filteredSet = children.OrderByDescending(c => c.PublishDate);
                count = children.Count;
            }
            return filteredSet;
        }

        public static IEnumerable<LatestUpdate> GetLatestUpdates(Guid itemId, out int count)//not in use
        {
            var listing = _contentRepository.GetItem<Item>(new GetItemByIdOptions(itemId));
            if (listing == null)
            {
                count = 0;
                return null;
            }
            var children = listing.Axes.GetDescendants()
                .Select(c => _contentRepository.GetItem<LatestUpdate>(new GetItemByItemOptions(c))).Where(c => c != null && (c.TemplateId == Guid.Parse("{D4E2BD63-CC54-4FEB-A821-7FF36425CEB2}")))
                .ToList();

            IEnumerable<LatestUpdate> filteredSet = children.OrderByDescending(c => c.PublishDate);

            count = children.Count;
            return filteredSet;
        }

        public static IEnumerable<SelectListItem> GetMonthsList()
        {
            var listing = _contentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE)));
            var children = listing.Axes.GetDescendants()
                .Select(c => _contentRepository.GetItem<Article>(new GetItemByItemOptions(c))).Where(c => c != null
                && (c.TemplateId == Guid.Parse("960E0516-38BC-495D-A49B-57A9EB0CE1CA")
                || c.TemplateId == Guid.Parse("A72A1FCC-CDB9-4949-842F-A8001075A7EA")))
                .ToList().OrderByDescending(c => c.PublishDate).FirstOrDefault();

            var months = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.MONTHS_LIST)).Items;
            var convertedItems = months.Select(m => new SelectListItem { Text = m.Text, Value = m.Value, Selected = m.Value == children.PublishDate.Month.ToString() ? true : false });
            return convertedItems;
        }

        public static IEnumerable<SelectListItem> GetStaticMonthList()
        {
            var months = _contentRepository.GetItem<ListDataSources>(new GetItemByIdOptions(Guid.Parse(DataSources.SHORT_WORD_MONTHS_LIST))).Items;
            var convertedItems = months.Select(m => new SelectListItem { Text = m.Text, Value = m.Value });
            return convertedItems;
        }
    }
}