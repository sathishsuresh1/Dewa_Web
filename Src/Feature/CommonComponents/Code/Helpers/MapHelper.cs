using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.CommonComponents.Helpers
{
    public static class MapHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(SitecoreX.Database)));

        public static List<SelectListItem> GetMapList()
        {
            try
            {
                var mapitem = _contentRepository.GetItem<ListDataSources>(new GetItemByIdOptions(Guid.Parse(DataSources.M32MapDatasource)));
                //var mapitem = SitecoreX.Database.GetItem(new global::Sitecore.Data.ID(DataSources.M32MapDatasource));
                if (mapitem != null && mapitem.Items!= null && mapitem.Items.Count()>0)
                {
                    //ISitecoreContext context = new SitecoreContext();
                    //var mylistdatasource = context.Cast<ListDataSources>(mapitem);
                    var convertedItems = mapitem.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            return null;
        }
    }
}