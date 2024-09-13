using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Links;
using Sitecore.Publishing;
using Sitecore.Publishing.Pipelines.Publish;
namespace DEWAXP.Foundation.Content.Pipelines
{
    public class AddRelatedItemsToQueue : PublishProcessor
    {
        private Database _sourceDatabase;

        public override void Process(PublishContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            Log.Info(GetType().FullName + ": Adding items to publish queue - START", this);
            _sourceDatabase = context.PublishOptions.SourceDatabase;
            IEnumerable<ID> itemIDs = GetRelatedItemIDs(context.PublishOptions);
            IEnumerable<PublishingCandidate> publishingCandidates = itemIDs.Select(itemId => new PublishingCandidate(itemId, context.PublishOptions)).ToArray();
            
            context.Queue.Add(publishingCandidates);
            Log.Info(GetType().FullName + ": Adding items to publish queue - END", this);
        }

        private IEnumerable<ID> GetRelatedItemIDs(PublishOptions options)
        {
            IEnumerable<ID> publishQueueItems = GetPublishQueueItems(options);
            IEnumerable<ID> publishQueueReferencedItems = publishQueueItems.SelectMany(GetReferences).ToArray();
            // In later versions of Sitecore the "referencedItems" collection
            // will contain data sources as well (Sitecore 6.6 Update-4 if I'm not mistaken).
            IEnumerable<ID> publishQueueDataSourceItems = publishQueueItems.SelectMany(GetDataSources).ToArray();
            HashSet<ID> allIDs = new HashSet<ID>();
            allIDs.UnionWith(publishQueueReferencedItems);
            allIDs.UnionWith(publishQueueDataSourceItems);

            //Adding Publishing Statistics item in the queue so it can be published as well.
            ID publishingStatisticsID = new ID(SitecoreItemIdentifiers.PUBLISHING_STATISTICS);
            allIDs.Add(publishingStatisticsID);
            return allIDs;
        }

        private IEnumerable<ID> GetPublishQueueItems(PublishOptions options)
        {
            if (options.Mode == PublishMode.Incremental)
                return PublishQueue.GetPublishQueue(options).Select(candidate => candidate.ItemId).ToArray();
            return PublishQueue.GetContentBranch(options).Select(candidate => candidate.ItemId).ToArray();
        }

        private IEnumerable<ID> GetReferences(ID itemID)
        {
            Item item = _sourceDatabase.GetItem(itemID);
            if (item == null)
                return Enumerable.Empty<ID>();
            ItemLink[] references = Globals.LinkDatabase.GetItemReferences(item, true);
            return references.Select(reference => reference.TargetItemID).ToArray();
        }

        private IEnumerable<ID> GetDataSources(ID itemID)
        {
            Item item = _sourceDatabase.GetItem(itemID);
            if (item == null)
                return Enumerable.Empty<ID>();
            string layoutXml = item[FieldIDs.LayoutField];
            if (string.IsNullOrEmpty(layoutXml))
                return Enumerable.Empty<ID>();
            IEnumerable<DeviceDefinition> devices = LayoutDefinition.Parse(layoutXml).Devices.Cast<DeviceDefinition>();
            IEnumerable<RenderingDefinition> renderings = devices.SelectMany(device => device.Renderings.Cast<RenderingDefinition>());
            var renderingReferences = item.Visualization.GetRenderings(this.GetDeviceItem(_sourceDatabase,"default"), true);
            IEnumerable<string> pathOrIDs = renderingReferences.Select(rendering => rendering.Settings.DataSource).Where(pathOrID => !string.IsNullOrEmpty(pathOrID)).ToArray();
            HashSet<ID> allIDs = new HashSet<ID>();
            IEnumerable<ID> ParentDatasourceIds = pathOrIDs.Select(_sourceDatabase.GetItem).Where(dataSourceItem => dataSourceItem != null).Select(dataSourceItem => dataSourceItem.ID).ToArray();
            allIDs.UnionWith(ParentDatasourceIds);
            foreach (var ParId in ParentDatasourceIds)
            {
                Item pitem = _sourceDatabase.GetItem(ParId);
                if (pitem.HasChildren)
                {
                    var pChildIds = pitem.Children.Select(c => c.ID);
                    allIDs.UnionWith(pChildIds);
                }
            }
            return allIDs;
        }
        private DeviceItem GetDeviceItem(Database db, string deviceName)
        {
            return db.Resources.Devices.GetAll().Where(d => d.Name.ToLower() == deviceName.ToLower()).First();
        }
    }


}