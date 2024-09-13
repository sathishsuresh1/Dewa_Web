// <copyright file="EpassHelper.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Models.Common;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Logger;
    using Glass.Mapper.Sc;
    using Glass.Mapper.Sc.Web;
    using global::Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using SitecoreX = global::Sitecore.Context;

    /// <summary>
    /// Defines the <see cref="EpassHelper" />.
    /// </summary>
    public static class EpassHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(SitecoreX.Database)));
        /// <summary>
        /// The GetPageSize.
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/>.</returns>
        public static List<SelectListItem> GetPageSize()
        {
            try
            {
                global::Sitecore.Data.Items.Item pagesizeitem = SitecoreX.Database.GetItem(DataSources.EpassPageSize);
                if (pagesizeitem != null)
                {
                    ListDataSources mylistdatasource = _contentRepository.GetItem<ListDataSources>(new GetItemByItemOptions(pagesizeitem));
                    IEnumerable<SelectListItem> convertedItems = mylistdatasource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            List<SelectListItem> lists = new List<SelectListItem>
            {
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "20", Value = "20" }
            };
            return lists;
        }

        /// <summary>
        /// The GetGccCountries.
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/>.</returns>
        public static List<SelectListItem> GetLocations(string datasource)
        {
            try
            {
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = DataSources.EpassgenerationLocations;
                }
                ListDataSources dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(datasource));
                IEnumerable<SelectListItem> convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems.ToList();
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            List<SelectListItem> lists = new List<SelectListItem>
            {
                new SelectListItem { Text = "Al Quoz Sustainable Building", Value = "1" },
                new SelectListItem { Text = "Al Wasl Office", Value = "2" },
                new SelectListItem { Text = "Burj Nahar Office", Value = "3" },
                new SelectListItem { Text = "Connection / Disconnection Al Quoz", Value = "4" },
                new SelectListItem { Text = "DEWA Academy", Value = "5" },
                new SelectListItem { Text = "H Station", Value = "6" },
                new SelectListItem { Text = "Solar Park", Value = "7" },
                new SelectListItem { Text = "Warsan Store", Value = "8" }
            };
            return lists;
        }

        /// <summary>
        /// The GetGccCountries.
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/>.</returns>
        public static List<SelectListItem> GetGccCountries()
        {
            try
            {
                ListDataSources dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.GccCountries));
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

        public static List<SelectListItem> GetPageStatus()
        {
            try
            {
                global::Sitecore.Data.Items.Item pagestatusitem = SitecoreX.Database.GetItem(DataSources.EpassPageStatus);
                if (pagestatusitem != null)
                {
                    ListDataSources mylistdatasource = _contentRepository.GetItem<ListDataSources>(new GetItemByItemOptions(pagestatusitem));
                    IEnumerable<SelectListItem> convertedItems = mylistdatasource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            List<SelectListItem> lists = new List<SelectListItem>
            {
                new SelectListItem { Text = "all", Value = "all" },
            };
            return lists;
        }

        public static List<SelectListItem> GetPendingApprovalPageStatus()
        {
            try
            {
                global::Sitecore.Data.Items.Item pagestatusitem = SitecoreX.Database.GetItem(DataSources.EpassapprovalPageStatus);
                if (pagestatusitem != null)
                {
                    ListDataSources mylistdatasource = _contentRepository.GetItem<ListDataSources>(new GetItemByItemOptions(pagestatusitem));
                    IEnumerable<SelectListItem> convertedItems = mylistdatasource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            List<SelectListItem> lists = new List<SelectListItem>
            {
                new SelectListItem { Text = "all", Value = "all" },
            };
            return lists;
        }

        public static List<SelectListItem> GetSelectListItems(string datasource)
        {
            try
            {
                global::Sitecore.Data.Items.Item pagestatusitem = SitecoreX.Database.GetItem(datasource);
                if (pagestatusitem != null)
                {
                    ListDataSources mylistdatasource = _contentRepository.GetItem<ListDataSources>(new GetItemByItemOptions(pagestatusitem));
                    IEnumerable<SelectListItem> convertedItems = mylistdatasource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, new object());
            }
            List<SelectListItem> lists = new List<SelectListItem>
            {
                new SelectListItem { Text = "none", Value = "none" },
            };
            return lists;
        }

        /// <summary>
        /// The GetFromToTime.
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/>.</returns>
        public static List<SelectListItem> GetFromToTime(string strstarttime = "", string strendtime = "", bool nighttime = false)
        {
            if (string.IsNullOrWhiteSpace(strstarttime))
            {
                strstarttime = "00:00";
            }
            if (string.IsNullOrWhiteSpace(strendtime))
            {
                strendtime = "23:30";
            }
            // Set the start time (00:00 means 12:00 AM)
            DateTime StartTime = DateTime.ParseExact(strstarttime, "HH:mm", null);
            // Set the end time (23:55 means 11:55 PM)
            DateTime EndTime = DateTime.ParseExact(strendtime, "HH:mm", CultureInfo.InvariantCulture);
            //Set 5 minutes interval
            TimeSpan Interval = new TimeSpan(0, 30, 0);
            var currentdate = DateTime.Today;
            //To set 1 hour interval
            //TimeSpan Interval = new TimeSpan(1, 0, 0);
            List<SelectListItem> lists = new List<SelectListItem>();
            if (!nighttime)
            {
                while (StartTime <= EndTime)
                {
                    lists.Add(new SelectListItem { Text = StartTime.ToString("hh:mm tt", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = StartTime.ToString("HH:mm", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً") });
                    StartTime = StartTime.Add(Interval);
                }
            }
            else
            {
                while (StartTime <= DateTime.Today.AddDays(1) && StartTime > currentdate)
                {
                    lists.Add(new SelectListItem { Text = StartTime.ToString("hh:mm tt", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = StartTime.ToString("HH:mm", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً") });
                    StartTime = StartTime.Add(Interval);
                }
                while (EndTime <= DateTime.Today.AddDays(1) && EndTime > currentdate)
                {
                    lists.Add(new SelectListItem { Text = currentdate.ToString("hh:mm tt", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = currentdate.ToString("HH:mm", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً") });
                    currentdate = currentdate.Add(Interval);
                }
            }
            return lists;
        }

        /// <summary>
        /// Get CurrentTime
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="interval"></param>
        /// <param name="timeformat"></param>
        /// <param name="allOption"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetCurrentFromToTime()
        {
            List<SelectListItem> lists = new List<SelectListItem>();
            DateTime startTime = DateTime.ParseExact("00:00", "HH:mm", null);
            DateTime endTime = DateTime.ParseExact("23:30", "HH:mm", CultureInfo.InvariantCulture);
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan Interval = new TimeSpan(0, 30, 0);
            while (startTime <= endTime)
            {
                if (TimeSpan.Parse(startTime.ToString("HH:mm")) >= currentTime)
                    lists.Add(new SelectListItem { Text = startTime.ToString("hh:mm tt", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = startTime.ToString("HH:mm", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً") });

                startTime = startTime.Add(Interval);
            }
            return lists;
        }

        /// <summary>
        /// The WPGetFromToTime.
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/>.</returns>
        public static List<SelectListItem> WPGetFromToTime()
        {
            DateTime StartTime = DateTime.ParseExact("00:00", "HH:mm", null);
            DateTime EndTime = DateTime.ParseExact("23:30", "HH:mm", CultureInfo.InvariantCulture);
            TimeSpan Interval = new TimeSpan(0, 30, 0);
            List<SelectListItem> lists = new List<SelectListItem>();
            StartTime = TimeList(StartTime, EndTime, Interval, lists);
            return lists;
        }

        /// <summary>
        /// The TimeList.
        /// </summary>
        /// <param name="StartTime">The StartTime<see cref="DateTime"/>.</param>
        /// <param name="EndTime">The EndTime<see cref="DateTime"/>.</param>
        /// <param name="Interval">The Interval<see cref="TimeSpan"/>.</param>
        /// <param name="lists">The lists<see cref="List{SelectListItem}"/>.</param>
        /// <returns>The <see cref="DateTime"/>.</returns>
        private static DateTime TimeList(DateTime StartTime, DateTime EndTime, TimeSpan Interval, List<SelectListItem> lists)
        {
            while (StartTime <= EndTime)
            {
                lists.Add(new SelectListItem { Text = StartTime.ToString("hh:mm tt", SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = StartTime.ToString("HHmmss", SitecoreX.Culture) });
                StartTime = StartTime.Add(Interval);
            }

            return StartTime;
        }

        public static List<SelectListItem> WPGetGccCountries()
        {
            try
            {
                ListDataSources dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.wpGccCountries));
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


        public static string ToDisplayDate(this string datevar)
        {
            DateTime dt = DateTime.Now;
            if (DateTime.TryParse(datevar, out dt)) { return dt.ToString("MMM dd, yyyy HH:mm"); }
            return string.Empty;
        }

        public static string GetDisplayName(Enum val)
        {
            return val.GetType()
                  .GetMember(val.ToString())
                  .FirstOrDefault()
                  ?.GetCustomAttribute<DisplayAttribute>(false)
                  ?.Name
                  ?? val.ToString();
        }

        public static string getWPLocationStatus(string status, SecurityPassStatus passStatus)
        {
            SecurityPassStatus securityPassStatus = SecurityPassStatus.Notapplicable;
            if (passStatus.Equals(SecurityPassStatus.Expired))
            {
                securityPassStatus = SecurityPassStatus.Expired;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status.ToLower().Equals("05"))
                    {
                        securityPassStatus = SecurityPassStatus.Cancelled;
                    }
                    else if (status.ToLower().Equals("02"))
                    {
                        securityPassStatus = SecurityPassStatus.Rejected;
                    }
                    else if (status.ToLower().Equals("03") || status.ToLower().Equals("04"))
                    {
                        securityPassStatus = SecurityPassStatus.Active;
                    }
                    else if (status.ToLower().Equals("01"))
                    {
                        securityPassStatus = SecurityPassStatus.UnderApprovalinWorkPermit;
                    }
                    else
                    {
                        securityPassStatus = SecurityPassStatus.Notapplicable;
                    }
                }
            }
            return Translate.Text("epassstatus." + securityPassStatus.ToString().ToLower());
        }
    }
}