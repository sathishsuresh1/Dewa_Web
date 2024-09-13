using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Extensions
{
    public static class GenericExtension
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        public static List<SelectListItem> GetGccCountriesList()
        {
            try
            {
                ListDataSources dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.SHAMS_DUBAI_GCCCOUNTRIES));
                IEnumerable<SelectListItem> convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems.ToList();

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            List<SelectListItem> lists = new List<SelectListItem>
            {
                new SelectListItem { Text = "Bahrain", Value = "Bahrain" },
                new SelectListItem { Text = "Kuwait", Value = "Kuwait" },
                new SelectListItem { Text = "Oman", Value = "Oman" },
                new SelectListItem { Text = "Qatar", Value = "Qatar" },
                new SelectListItem { Text = "Saudi Arabia", Value = "Saudi Arabia" },
                new SelectListItem { Text = "United Arab Emirates", Value = "United Arab Emirates" }
            };
            return lists;
        }

        public static List<SelectListItem> GetLstDataSource(string datasource)
        {
            try
            {
                ListDataSources dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(datasource));
                IEnumerable<SelectListItem> convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems.ToList();
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, null);
                return new List<SelectListItem>();
            }
        }
    }
}