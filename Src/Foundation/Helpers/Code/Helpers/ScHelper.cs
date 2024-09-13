using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Helpers
{
    public static class ScHelper
    {
        public static bool IsPageExistedInCurrentPageOrDescendent(Guid ParentPageId, Guid currentPageId)
        {
            try
            {
                if (ParentPageId != null && currentPageId != null)
                {
                    //checking page is current page
                    if (ParentPageId == currentPageId)
                    {
                        return true;
                    }
                    //checking page is child page
                    var parentPage = new RequestContext(new SitecoreService(SitecoreX.Database)).SitecoreService.GetItem<Item>(ParentPageId);
                    if (parentPage != null)
                    {
                        var filterPage = parentPage.Axes.GetChild(new ID(currentPageId));
                        return filterPage != null;
                    }
                }
            }
            catch (Exception ex)
            {
                Foundation.Logger.LogService.Info(ex);
            }

            return false;
        }

        public static List<SelectListItem> GetDataSourceItemList(string sourceID)
        {
            var item = SitecoreX.Database.GetItem(new ID(sourceID));
            if (item != null)
            {
                return item.Children.Select(c => new SelectListItem
                {
                    Text = c.Fields["Text"].ToString(),
                    Value = c.Fields["Value"].ToString()
                }).ToList();
            }
            return new List<SelectListItem>();
        }
    }
}