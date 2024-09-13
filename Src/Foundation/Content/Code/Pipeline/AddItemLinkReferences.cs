using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Comparers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Links;
using Sitecore.Publishing;
using Sitecore.Publishing.Pipelines.GetItemReferences;
using Sitecore.Publishing.Pipelines.Publish;
using Sitecore.Publishing.Pipelines.PublishItem;

namespace DEWAXP.Foundation.Content.Pipelines
{
    public class AddItemLinkReferences : GetItemReferencesProcessor 
    {
        protected override List<Item> GetItemReferences(PublishItemContext context)
        {
            Item target = context.PublishOptions.SourceDatabase.GetItem(
    context.ItemId,
    context.PublishOptions.Language);
            return this.GetReferences(target, true).ToList();
        }
        private System.Collections.Generic.IEnumerable<Item> GetReferences(Item item, bool sharedOnly)
        {
            Assert.ArgumentNotNull(item, "item");
            System.Collections.Generic.List<Item> list = new System.Collections.Generic.List<Item>();
            ItemLink[] source = item.Links.GetValidLinks();
            source = (from link in source
                where item.Database.Name.Equals(link.TargetDatabaseName, System.StringComparison.OrdinalIgnoreCase)
                select link).ToArray<ItemLink>();
            if (sharedOnly)
            {
                source = source.Where(delegate(ItemLink link)
                {
                Item sourceItem = link.GetSourceItem();
                return sourceItem != null && (ID.IsNullOrEmpty(link.SourceFieldID) || sourceItem.Fields[link.SourceFieldID].Shared);
                }).ToArray<ItemLink>();
            }
            System.Collections.Generic.List<Item> list2 = (from link in source
            select link.GetTargetItem() into relatedItem
            where relatedItem != null
            select relatedItem).ToList<Item>();
            System.Collections.Generic.List<Item> list3 = new List<Item>();
            foreach (var itemS in list2)
            {
                list3.Add(itemS);
                if (itemS.HasChildren)
                {
                    list3.AddRange(itemS.Children);
                }
            }
            foreach (Item current in list3)
            {
                list.AddRange(PublishQueue.GetParents(current));
                list.Add(current);
            }
            return list.Distinct(new ItemIdComparer());
        }
    }


}