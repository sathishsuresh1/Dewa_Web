using Sitecore.ContentSearch.Pipelines.GetDependencies;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace DEWAXP.Feature.Search
{
    public class GetDatasourceDependencies : BaseProcessor
    {
        public override void Process(GetDependenciesArgs context)
        {
            Func<ItemUri, bool> func = null;
            Assert.IsNotNull(context.IndexedItem, "indexed item");
            Assert.IsNotNull(context.Dependencies, "dependencies");
            Item item = (Item)(context.IndexedItem as SitecoreIndexableItem);
            if (item != null)
            {
                if (func == null)
                {
                    func = uri => (bool)((uri != null) && ((bool)(uri != item.Uri)));
                }
                System.Collections.Generic.IEnumerable<ItemUri> source = Enumerable.Where<ItemUri>(from l in Globals.LinkDatabase.GetReferrers(item, FieldIDs.LayoutField) select l.GetSourceItem().Uri, func).Distinct<ItemUri>();
                context.Dependencies.AddRange(source.Select(x => (SitecoreItemUniqueId)x));
            }
        }

    }
}