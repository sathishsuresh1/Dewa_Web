using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.TaskModels
{
    public class SitemapInfo
    {
        public SitemapInfo()
        {
            children = new List<SitemapInfo>();
        }
        public string page_id { get; set; }
        public string page_type { get; set; }
        public string page_type_id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string name_ar { get; set; }
        public string url_ar { get; set; }
        public bool is_label_only { get; set; }
        public int level { get; set; }
        public List<SitemapInfo> children { get; set; }
    }


}