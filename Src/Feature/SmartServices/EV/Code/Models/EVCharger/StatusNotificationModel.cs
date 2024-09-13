using System;
using System.Collections.Generic;
using System.Linq;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.Tr;
using System.Web.Mvc;
using Glass.Mapper.Sc;
using System.Globalization;
using SitecoreX = Sitecore;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    public class StatusNotificationModel
    {
        public string Date { get; set; }
        public string RequestType { get; set; }
        public string NotificationNumber { get; set; }
        public string ShortStatusText { get; set; }
        public string BPNumber { get; set; }
        public string strnotificationdate { get; set; }
        public string strnotificationtime { get; set; }
        public DateTime Dtnotificationdate => DateTime.ParseExact(strnotificationdate, "dd MMM yyyy", CultureInfo.InvariantCulture).Add(TimeSpan.Parse(strnotificationtime));

    }

    public class EVNotificationPageModel
    {
        public EVNotificationPageModel()
        {
            StatusList = new List<StatusNotificationModel>();
        }
        public List<StatusNotificationModel> StatusList { get; private set; }

        public int totalpage { get; set; }
        public int page { get; set; }
        public bool pagination { get; set; }
        public string sortby { get; set; }
        public IEnumerable<int> pagenumbers { get; set; }

        public static EVNotificationPageModel MapFrom(EVTrackResponse response, int page, string sortby, DEWAXP.Foundation.Integration.Enums.SupportedLanguage requestlang)
        {
            var model = new EVNotificationPageModel();
            List<Evnotificationlist> slist = new List<Evnotificationlist>();
            if (!string.IsNullOrEmpty(sortby))
            {
                
                model.sortby = sortby;
                var splitedsortby = sortby.Split(',');
                if(splitedsortby!= null && splitedsortby.Count()>0)
                {
                    foreach (var unsplitedsortbycolon in splitedsortby)
                    {
                        IEnumerable<Evnotificationlist> sortedlist = null;
                        var splitedsortbycolon = unsplitedsortbycolon.Split(':');
                        if(splitedsortbycolon!= null && splitedsortbycolon.Count()>0)
                        {
                            sortedlist = response.Envelope.Body.GetEVTrackRequestResponse.@return.evnotificationlist.Where(x => x.codegrouppartobject.Equals(splitedsortbycolon[0]) && x.partobject.Equals(splitedsortbycolon[1]));
                            //if(slist != null && slist.Count() > 0)
                            //{
                            //    break;
                            //}
                        }
                        if(sortedlist != null && sortedlist.Count() > 0)
                        {
                            slist.AddRange(sortedlist);
                        }
                    }
                }
                
            }
            else
            {
                slist = response.Envelope.Body.GetEVTrackRequestResponse.@return.evnotificationlist?.ToList();
            }
            if(requestlang.Equals(DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic))
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ar-AE");
            }
            else
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            }

            foreach (var evl in slist)
            {
                model.StatusList.Add(new StatusNotificationModel()
                {
                    BPNumber = evl.businesspartnernumber,
                    NotificationNumber = evl.notificationnumber,
                    RequestType = evl.requesttype,
                    ShortStatusText = evl.shortStatustext,
                    Date = DateTime.Parse(evl.notificationdate, CultureInfo.DefaultThreadCurrentCulture).ToString("dd MMM yyy"),
                    strnotificationdate = DateTime.ParseExact(evl.notificationdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
                    strnotificationtime = evl.notificationtime
                });
            }
            if(model.StatusList != null && model.StatusList.Count >0)
            {
                model.StatusList = model.StatusList.OrderByDescending(x => x.Dtnotificationdate).ToList();
            }
            model.page = page;
            int count = NotificationsPerPage();

            model.totalpage = Pager.CalculateTotalPages(model.StatusList.Count, count);
            model.pagination = model.totalpage > 1 ? true : false;
            model.pagenumbers = model.totalpage > 1 ? GetPaginationRange(page, model.totalpage) : new List<int>();


            model.StatusList = model.StatusList.Skip((page - 1) * count).Take(count).ToList();

            /*if (string.IsNullOrEmpty(sortby))
                slist = response.Envelope.Body.GetEVTrackRequestResponse.@return.evnotificationlist.OrderBy(x => x.).Skip((page - 1) * 20).Take(20);
            else slist = response.Envelope.Body.GetEVTrackRequestResponse.@return.evnotificationlist.Skip((page - 1) * 20).Take(20);*/

            return model;
        }

        private static IEnumerable<int> GetPaginationRange(int currentPage, int totalPages)
        {
            const int desiredCount = 5;
            var returnint = new List<int>();

            var start = currentPage - 1;
            var projectedEnd = start + desiredCount;
            if (projectedEnd > totalPages)
            {
                start = start - (projectedEnd - totalPages);
                projectedEnd = totalPages;
            }

            int p = start;
            while (p++ < projectedEnd)
            {
                if (p > 0)
                {
                    returnint.Add(p);
                }
            }
            return returnint;
        }

        #region
        //public static IEnumerable<SelectListItem> GetLstDataSource(string datasource)
        //{
        //    try
        //    {
        //        ISitecoreContext sitecorecontext = new SitecoreContext();
        //        var dataSource = sitecorecontext.GetItem<ListDataSources>(datasource, false);
        //        var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
        //        return convertedItems;
        //    }
        //    catch (System.Exception)
        //    {
        //        throw new System.Exception("Error in getting Datasource");
        //    }
        //}

        public static int NotificationsPerPage()
        {
            SitecoreX.Data.Items.Item configItem = SitecoreX.Context.Database.GetItem(SitecoreItemIdentifiers.EV_Notifications_Config);
            return configItem != null && configItem.Fields["Notifications Per Page"] != null &&
                configItem.Fields["Notifications Per Page"].Value != null ? int.Parse(configItem.Fields["Notifications Per Page"].Value.ToString()) : 5;
        }
        #endregion
    }
}