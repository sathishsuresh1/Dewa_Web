using Sitecore.ContentSearch.ComputedFields;
using Sitecore.ContentSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Search.DynamicFields
{
    public class CategoryField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            string category = null;
            var indexItem = indexable as SitecoreIndexableItem;
            var item = (global::Sitecore.Data.Items.Item)indexItem?.Item;

            if (item != null)
            {
                category = item.Fields["Search Category"].Value;
                if (item.TemplateName == "NewsArticle")
                {
                    category = "News";
                }
                if (category == "" || category == null)
                {
                    category = "Content";
                }
            }
            return category;
        }
    }
}