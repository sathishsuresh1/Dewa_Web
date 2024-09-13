using DEWAXP.Foundation.Content.Models.Common;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.CollectiveStatement
{
    public class CollectiveStatementModel : GenericPageWithIntro
    {
        public IEnumerable<SelectListItem> AccountsList { get; set; }
        public IEnumerable<SelectListItem> DownloadOptions { get; set; }
        public string SelectedAccount { get; set; }
        public string SelectedDownloadOption { get; set; }
        public string DownloadLink { get; set; }
    }
}