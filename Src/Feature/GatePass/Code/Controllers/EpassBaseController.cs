// <copyright file="EpassBaseController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Controllers
{
    using DEWAXP.Feature.GatePass.Models.ePass;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartVendor.WorkPermit;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.SmartVendor.WorkPermit;
    using DEWAXP.Foundation.Integration.SmartVendorSvc;
    using DEWAXP.Foundation.Logger;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using X.PagedList;
    using Sitecorex = global::Sitecore;

    /// <summary>
    /// Defines the <see cref="EpassBaseController" />.
    /// </summary>
    public class EpassBaseController : BaseController
    {
        protected GroupPassPemitResponse GetWorkpermitPasses(string workpermitnumber = "")
        {
            string grouppassid = string.Empty;
            if (!string.IsNullOrWhiteSpace(workpermitnumber) && (workpermitnumber.ToLower().StartsWith("gp") || workpermitnumber.ToLower().StartsWith("sp")))
            {
                grouppassid = workpermitnumber;
                workpermitnumber = string.Empty;
            }
            var response = SmartVendorClient.SubmitWorkPermitPass(
                        new GroupPemitPassRequest
                        {
                            grouppassinput = new Grouppassinput
                            {
                                processcode = "PDT",
                                lang = RequestLanguage.ToString(),
                                sessionid = CurrentPrincipal.SessionToken,
                                userid = CurrentPrincipal.UserId,
                                permitpass = workpermitnumber,
                                groupid = grouppassid
                            }
                        }, RequestLanguage, Request.Segment());

            if (response != null && response.Succeeded && response.Payload != null)
            {
                return response.Payload;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The assignWPStatus.
        /// </summary>
        /// <param name="status">The status<see cref="string"/>.</param>
        /// <param name="pass_expiry_date">The pass_expiry_date<see cref="string"/>.</param>
        /// <returns>The <see cref="SecurityPassStatus"/>.</returns>
        protected SecurityPassStatus assignWPStatus(List<Grouppasslocationlistres> grouppasslocationlistres, string grouppassid, string pass_expiry_date)
        {
            SecurityPassStatus assingedstatus = SecurityPassStatus.Notapplicable;
            DateTime? expirydate = !string.IsNullOrWhiteSpace(pass_expiry_date) ? DateTime.Parse(pass_expiry_date) : (DateTime?)null;

            if (grouppasslocationlistres != null && grouppasslocationlistres.Count() > 0 && grouppasslocationlistres.Where(y => y.Grouppassid.Equals(grouppassid)).Any())
            {
                var locationliststatus = grouppasslocationlistres.Where(y => y.Grouppassid.Equals(grouppassid)).Select(z => z.Status).ToList();
                if (locationliststatus.All(x => !string.IsNullOrWhiteSpace(x) && x.Equals("05")))
                {
                    assingedstatus = SecurityPassStatus.Cancelled;
                }
                else if (locationliststatus.All(x => !string.IsNullOrWhiteSpace(x) && x.Equals("02")))
                {
                    assingedstatus = SecurityPassStatus.Rejected;
                }
                else if (locationliststatus.Any(x => !string.IsNullOrWhiteSpace(x) && (x.Equals("03") || x.Equals("04"))))
                {
                    if (expirydate != null && expirydate.HasValue && expirydate.Value.Ticks > 0)
                    {
                        if (expirydate.Value.Date < DateTime.Now.Date)
                        {
                            assingedstatus = SecurityPassStatus.Expired;
                        }
                        else if (expirydate.Value.Date < DateTime.Now.Date.AddDays(14))
                        {
                            assingedstatus = SecurityPassStatus.SoontoExpire;
                        }
                        else
                        {
                            assingedstatus = SecurityPassStatus.Active;
                        }
                    }
                }
                else if (locationliststatus.All(x => !string.IsNullOrWhiteSpace(x) && x.Equals("01")))
                {
                    assingedstatus = SecurityPassStatus.UnderApprovalinWorkPermit;
                }
                else
                {
                    assingedstatus = SecurityPassStatus.Notapplicable;
                }
            }
            //if (!string.IsNullOrWhiteSpace(status))
            //{
            //    if (status.ToLower().Equals("05"))
            //    {
            //        assingedstatus = SecurityPassStatus.Cancelled;
            //    }
            //    else if (status.ToLower().Equals("02"))
            //    {
            //        assingedstatus = SecurityPassStatus.Rejected;
            //    }
            //    else if (status.ToLower().Equals("03") || status.ToLower().Equals("04"))
            //    {
            //        if (expirydate != null && expirydate.HasValue && expirydate.Value.Ticks > 0)
            //        {
            //            if (expirydate.Value.Date < DateTime.Now.Date)
            //            {
            //                assingedstatus = SecurityPassStatus.Expired;
            //            }
            //            else if (expirydate.Value.Date < DateTime.Now.Date.AddDays(14))
            //            {
            //                assingedstatus = SecurityPassStatus.SoontoExpire;
            //            }
            //            else
            //            {
            //                assingedstatus = SecurityPassStatus.Active;
            //            }
            //        }
            //    }
            //    else if (status.ToLower().Equals("01"))
            //    {
            //        assingedstatus = SecurityPassStatus.UnderApprovalinWorkPermit;
            //    }
            //    else
            //    {
            //        assingedstatus = SecurityPassStatus.Notapplicable;
            //    }
            //}

            return assingedstatus;
        }

        /// <summary>
        /// The GetWPCountryList.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{SelectListItem}"/>.</returns>
        protected IEnumerable<SelectListItem> GetWPCountryList()
        {
            ServiceResponse<countryListResponse> response = VendorServiceClient.GetCountryList(new GetCountryList { sessionid = CurrentPrincipal.SessionToken }, RequestLanguage, Request.Segment());
            if (response != null && response.Succeeded && response.Payload != null && response.Payload.countryList != null && response.Payload.countryList.Count() > 0)
            {
                List<SelectListItem> countrylist = response.Payload.countryList.Select(c => new SelectListItem
                {
                    Text = c.countryName,
                    Value = c.countryKey
                }).ToList();

                CacheProvider.Store(CacheKeys.WORK_PERMIT_COUNTRYLIST, new CacheItem<List<SelectListItem>>(countrylist, TimeSpan.FromMinutes(40)));
                return countrylist;
            }
            else
            {
                return Enumerable.Empty<SelectListItem>();
            }
        }

        public IEnumerable<SelectListItem> GetLocationList()
        {
            var response = SmartVendorClient.SubmitWorkPermitPass(
                        new GroupPemitPassRequest
                        {
                            grouppassinput = new Grouppassinput
                            {
                                processcode = "INT",
                                lang = RequestLanguage.ToString(),
                                sessionid = CurrentPrincipal.SessionToken,
                                userid = CurrentPrincipal.UserId,
                            }
                        }, RequestLanguage, Request.Segment());
            if (response != null && response.Succeeded && response.Payload != null && response.Payload.Grouppasslocationlist != null && response.Payload.Grouppasslocationlist.Count() > 0)
            {
                List<SelectListItem> countrylist = response.Payload.Grouppasslocationlist.Select(c => new SelectListItem
                {
                    Text = c.Location,
                    Value = c.Locationcode
                }).ToList();
                return countrylist;
            }
            else
            {
                return Enumerable.Empty<SelectListItem>();
            }
        }

        /// <summary>
        /// The ConvertViewToString.
        /// </summary>
        /// <param name="viewName">The viewName<see cref="string"/>.</param>
        /// <param name="model">The model<see cref="object"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        protected string FormatVehiclePlateNumber(string Emirates, string platecode, string platenumber) => (!string.IsNullOrWhiteSpace(Emirates) ? Emirates.Trim() : string.Empty) + "-" + (!string.IsNullOrWhiteSpace(platecode) ? platecode.Trim() : string.Empty) + "-" + (!string.IsNullOrWhiteSpace(platenumber) ? platenumber.Trim() : string.Empty);

        protected List<SelectListItem> GetDetailForCatOrCode(string catCode = "", bool isPlateCode = true, string region = "DXB")
        {
            List<SelectListItem> data = new List<SelectListItem>();
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.EvPlateDetailsResponse d = null;

            if (!CacheProvider.TryGet("ev.plate.detail", out d))
            {
                var returnData = EVCardApiHandler.GetEvPlateDetails(new DEWAXP.Foundation.Integration.APIHandler.Models.ApiBaseRequest(), RequestLanguage, Request.Segment());
                if (returnData != null && returnData.Succeeded)
                {
                    d = returnData.Payload;
                    CacheProvider.Store("ev.plate.detail", new CacheItem<DEWAXP.Foundation.Integration.APIHandler.Models.Response.EvPlateDetailsResponse>(d, TimeSpan.FromHours(1)));
                }
            }

            if (d != null)
            {
                bool IsArabic = RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic;

                if (isPlateCode)
                {
                    data = d.EVPlateCodeList.Where(x => x.region == region && (x.categoryCode == catCode || string.IsNullOrWhiteSpace(catCode))).GroupBy(x => new { x.codeAR, x.codeEN, x.plateCode })?.Select(xx => new SelectListItem() { Text = (IsArabic ? xx.FirstOrDefault()?.codeAR : xx.FirstOrDefault()?.codeEN), Value = xx.FirstOrDefault()?.plateCode })?.Distinct()?.ToList();
                }
                else
                {
                    data = d.EVPlateCodeList.Where(x => x.region == region).GroupBy(x => new { x.categoryEN, x.categoryAR, x.categoryCode })?.Select(xx => new SelectListItem() { Text = (IsArabic ? xx.FirstOrDefault()?.categoryAR : xx.FirstOrDefault()?.categoryEN), Value = xx.FirstOrDefault()?.categoryCode })?.Distinct()?.ToList();
                }
            }

            return data;
        }

        protected DateTime FormatEpassDate(string datetime) => Convert.ToDateTime(datetime.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December"));

        protected string FormatEpassstrDate(string datetime) => (Sitecorex.Context.Culture.Name.Equals("ar-AE") ? FormatEpassDate(datetime).ToString("MMMM dd، yyyy", Sitecorex.Context.Culture) : FormatEpassDate(datetime).ToString("MMMM dd, yyyy", Sitecorex.Context.Culture));

        protected string FormatKofaxDate(string datetime)
        {
            string kofaxdatestr = string.Empty;
            try
            {
                var kofaxdate = Convert.ToDateTime(datetime.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December"));
                if (kofaxdate != null && kofaxdate.Ticks > 0)
                {
                    kofaxdatestr = kofaxdate.ToString("dd MMM yyyy");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return kofaxdatestr;
        }

        protected string FormatKofaxDatewithCulture(string datetime)
        {
            string kofaxdatestr = string.Empty;
            try
            {
                var kofaxdate = Convert.ToDateTime(datetime.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December"));
                if (kofaxdate != null && kofaxdate.Ticks > 0)
                {
                    kofaxdatestr = kofaxdate.ToString("dd MMM yyyy", Sitecorex.Context.Culture);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return kofaxdatestr;
        }

        protected List<string> lstselectedlocations(string ePassLocation)
        {
            if (!string.IsNullOrWhiteSpace(ePassLocation))
            {
                var list = new List<string>(ePassLocation.ToString().Replace("\0", "").Replace("||", "|").Split(';'));
                var locations = GetLocation();
                var selectListItems = locations.Where(x => list.Any(y => y.Equals(x.Value)));
                if (selectListItems != null && selectListItems.Count() > 0)
                {
                    return selectListItems.Select(x => x.Text).ToList();
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// The GetLocation.
        /// </summary>
        /// <returns>The <see cref="List{string}"/>.</returns>
        protected List<SelectListItem> GetLocation(string datasource = "")
        {
            if (string.IsNullOrWhiteSpace(datasource))
            {
                datasource = DataSources.EpassgenerationLocations;
            }
            List<SelectListItem> lstLocation = EpassHelper.GetLocations(datasource);
            return lstLocation;
        }
    }
}