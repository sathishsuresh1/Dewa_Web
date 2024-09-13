using DEWAXP.Foundation.Content;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;

namespace DEWAXP.Feature.Search.Services
{
    public class SiteSearch
    {
        public static void TrackSiteSearch(Item pageEventItem, string query)
        {
            Assert.ArgumentNotNull(pageEventItem, nameof(pageEventItem));
            Assert.IsNotNull(pageEventItem, $"Cannot find page event: {pageEventItem}");

            if (Tracker.IsActive)
            {
                var pageEventData = new PageEventData("Search", Guid.Parse(SitecoreItemIdentifiers.SEARCH_PAGE_EVENT))
                {
                    ItemId = pageEventItem.ID.ToGuid(),
                    Data = query,
                    DataKey = query,
                    Text = query,
                };
                var interaction = Tracker.Current.Session.Interaction;
                if (interaction != null)
                {
                    interaction.CurrentPage.Register(pageEventData);
                }
            }
        }
    }
}