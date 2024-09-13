using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Sitecore.ContentSearch.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Search.Models
{
    public class SearchResults : PageBase
    {
        public SearchResults(IEnumerable<SearchResult> results, PaginationModel pagination, string searchTerm, int totalResults, FacetResults facetResult, string facets)
        {
            TotalResults = totalResults;
            SearchTerm = searchTerm;
            Pagination = pagination;
            Results = results ?? new List<SearchResult>();
            FacetResult = facetResult;
            Facets = facets;
        }


        public FacetResults FacetResult { get; set; }
        public IEnumerable<SearchResult> Results { get; private set; }
        public PaginationModel Pagination { get; private set; }
        public string SearchTerm { get; private set; }
        public int TotalResults { get; private set; }

        public string Facets { get; set; }
        public int PageSize { get; set; }
        public bool IsPageNoAltered { get; set; }
        public IEnumerable<SelectListItem> PageSizeList { get; set; }
    }
}