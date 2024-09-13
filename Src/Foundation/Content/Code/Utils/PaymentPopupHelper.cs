using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Content.Utils
{
    public static class PaymentPopupHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(SitecoreX.Database)));

        public static List<SelectListItem> GetSuqiaDonationList()
        {
            try
            {
                var item = _contentRepository.GetItem<ListDataSources>(new GetItemByIdOptions(Guid.Parse(DataSources.SuqiaDonationDatasource)));
                if (item != null && item.Items != null && item.Items.Count() > 0)
                {
                    var convertedItems = item.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            return null;
        }

        public static decimal GetSuqiaAmount(string SuqiaDonationType, string SuqiaDonationAmt)
        {

            decimal suqiaPaidAmount = 0;
            if (!string.IsNullOrWhiteSpace(SuqiaDonationType))
            {
                var squiaType = GetSuqiaDonationList().FirstOrDefault(x => x.Value == (SuqiaDonationType));
                if (squiaType != null)
                {
                    if (squiaType.Value == "other")
                    {
                        suqiaPaidAmount = Convert.ToDecimal(SuqiaDonationAmt);
                    }
                    else
                    {
                        suqiaPaidAmount = Convert.ToDecimal(squiaType.Text.Replace(Translate.Text("AED"), ""));
                    }
                }

            }

            return suqiaPaidAmount;
        }
    }
}