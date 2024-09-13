using Sitecore.ContentSearch.ComputedFields;
using Sitecore.ContentSearch.Utilities;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Search.DynamicFields
{
    public class AllTemplatesField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            var templates = new List<string>();
            var indexItem = indexable as SitecoreIndexableItem;
            if (indexItem != null)
            {
                var item = (global::Sitecore.Data.Items.Item)indexItem.Item;
                this.GetAllTemplates(item.Template, templates);
            }
            return templates;
        }

        public void GetAllTemplates(TemplateItem baseTemplate, List<string> list)
        {
            if (baseTemplate.ID != global::Sitecore.TemplateIDs.StandardTemplate)
            {
                string str = IdHelper.NormalizeGuid(baseTemplate.ID);
                list.Add(str);
                foreach (TemplateItem item in baseTemplate.BaseTemplates)
                {
                    this.GetAllTemplates(item, list);
                }
            }
        }

    }
}