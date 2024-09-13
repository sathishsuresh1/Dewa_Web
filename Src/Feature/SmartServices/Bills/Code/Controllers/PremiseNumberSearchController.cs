// <copyright file="PremiseNumberSearchController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\mayur.prajapati</author>
using DEWAXP.Feature.Bills.Models.PremiseNumberSearch;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.PremiseNumberSearch;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    /// <summary>
    /// Defines the <see cref="PremiseNumberSearchController" />.
    /// </summary>
    public class PremiseNumberSearchController : BaseController
    {
        #region PremiseNoTracking

        [HttpGet]
        public ActionResult PremiseNoTracking()
        {
            PremiseNumSearchModel searchModel = new PremiseNumSearchModel();
            try
            {
                // Captcha
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
                searchModel.PaginationInfo = new PaginationModel(null, null, 0, 0);
                searchModel.SearchOptions = GetAccountTypeList();
                searchModel.IsLoggedIn = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? true : false;
                searchModel.columnsCount = new PremiseNumSearchCountData();
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View("~/Views/Feature/Bills/PremiseNumberSearch/PremiseNoTracking.cshtml", searchModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PremiseNoTracking(PremiseNumSearchModel model)
        {
            PremiseNumberSearchRequest premiseMasterValueRequest = new PremiseNumberSearchRequest();

            try
            {
                // Captcha
                bool status = false;
                model.IsLoggedIn = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? true : false;
                model.PaginationInfo = new PaginationModel(null, null, 0, 0);
                model.columnsCount = new PremiseNumSearchCountData();

                // Request for Search results

                #region [Request for Search results]

                premiseMasterValueRequest.logintype = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? "L" : "A";
                premiseMasterValueRequest.searchflag = "R";
                premiseMasterValueRequest.sessionid = Convert.ToString(CurrentPrincipal?.SessionToken);

                premiseMasterValueRequest.searchinput = new SearchInput
                {
                    key = model.selectedSearchType,
                    value = (model.selectedSearchType == "GP") ? model.gisxyValue : model.SearchText
                };
                premiseMasterValueRequest.downloadFlag = string.Empty;

                string cacheResultKey = "PremiseNumSearchResultList" + premiseMasterValueRequest.searchinput.key + premiseMasterValueRequest.searchinput.value;
                List<PremiseNumSearchResult> returnResult = null;

                bool isLoadmore = (!string.IsNullOrWhiteSpace(model.cachedKey) && model.cachedKey == cacheResultKey);

                #endregion [Request for Search results]

                // Not execute in loggedin case
                if (!model.IsLoggedIn && !isLoadmore)
                {
                    string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

                    if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
                    {
                        status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
                    }
                    else if (!ReCaptchaHelper.Recaptchasetting())
                    {
                        status = true;
                    }
                }

                if (status || model.IsLoggedIn || isLoadmore)
                {
                    if (!(CacheProvider.TryGet(cacheResultKey, out returnResult) && returnResult != null && returnResult.Count > 0))
                    {
                        var response = PremiseNumberSearchClient.PremiseNumberSearch(premiseMasterValueRequest, RequestLanguage, Request.Segment());

                        if (response != null && response.Succeeded && response.Payload != null)
                        {
                            model.PremiseNumSearchResultList = response.Payload.searchResultList.Select(y => new PremiseNumSearchResult
                            {
                                BusinessPartner = y.businessPartner ?? string.Empty,
                                CommunityName = y.communityName ?? string.Empty,
                                ContractAccount = y.contractAccount ?? string.Empty,
                                ElectricityApplication = y.electricityApplication ?? string.Empty,
                                GisX = y.gisX ?? string.Empty,
                                GisY = y.gisY ?? string.Empty,
                                LegacyNumber = y.legacyNumber ?? string.Empty,
                                Load = y.load ?? string.Empty,
                                MakaniNumber = y.makaniNumber ?? string.Empty,
                                Name = y.name ?? string.Empty,
                                PremiseType = y.premiseType ?? string.Empty,
                                PtypeText = y.ptypeText ?? string.Empty,
                                RoomNumber = y.roomNumber ?? string.Empty,
                                UnitNumber = y.unitNumber ?? string.Empty,
                                WaterApplication = y.waterApplication ?? string.Empty
                            }).ToList();

                            // store result list
                            CacheProvider.Store(cacheResultKey, new CacheItem<List<PremiseNumSearchResult>>(model.PremiseNumSearchResultList, TimeSpan.FromMinutes(40)));
                        }
                        else
                        {
                            model.resDescription = response.Message;
                        }
                    }
                    else
                    {
                        model.PremiseNumSearchResultList = returnResult;
                    }

                    if (model.PremiseNumSearchResultList != null && model.PremiseNumSearchResultList.Count > 0)
                    {
                        model.columnsCount.BusinessPartnerCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.BusinessPartner)).Count();
                        model.columnsCount.CommunityNameCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.CommunityName)).Count();
                        model.columnsCount.ContractAccountCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.ContractAccount)).Count();
                        model.columnsCount.ElectricityApplicationCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.ElectricityApplication)).Count();
                        model.columnsCount.GisXCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.GisX)).Count();
                        model.columnsCount.GisYCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.GisY)).Count();
                        model.columnsCount.LegacyNumberCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.LegacyNumber)).Count();
                        model.columnsCount.LoadCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.Load)).Count();
                        model.columnsCount.MakaniNumberCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.MakaniNumber)).Count();
                        model.columnsCount.NameCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Count();
                        model.columnsCount.PremiseTypeCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.PremiseType)).Count();
                        model.columnsCount.PtypeTextCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.PtypeText)).Count();
                        model.columnsCount.RoomNumberCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.RoomNumber)).Count();
                        model.columnsCount.UnitNumberCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.UnitNumber)).Count();
                        model.columnsCount.WaterApplicationCount = model.PremiseNumSearchResultList.Where(x => !string.IsNullOrWhiteSpace(x.WaterApplication)).Count();
                    }

                    #region [Pagination]

                    int pageItem = Convert.ToInt32(Translate.Text("PNS_PageItem"));
                    int totalPage = 0;
                    model.cachedKey = cacheResultKey;
                    model.currentPage = model.currentPage + 1;
                    model.PremiseFilterSeachResultList = model.PremiseNumSearchResultList?.ApplyPaging(model.currentPage, pageItem)?.ToList();
                    totalPage = Pager.CalculateTotalPages(model.PremiseNumSearchResultList.Count(), pageItem);
                    model.PaginationInfo = new PaginationModel("", "", model.currentPage, totalPage);

                    #endregion [Pagination]

                    if (model.currentPage > 1)
                    {
                        return View("~/Views/Feature/Bills/PremiseNumberSearch/PremiseNoTrackingList.cshtml", model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            // Captcha
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            model.SearchOptions = GetAccountTypeList();
            model.IsLoggedIn = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? true : false;
            return View("~/Views/Feature/Bills/PremiseNumberSearch/PremiseNoTracking.cshtml", model);
        }

        #endregion PremiseNoTracking

        #region GetAccountTypeList

        private List<PremiseNumSearchOptions> GetAccountTypeList()
        {
            PremiseNumberSearchRequest premiseMasterValueRequest = new PremiseNumberSearchRequest();
            List<PremiseNumSearchOptions> returnData = null;

            if (!CacheProvider.TryGet(CacheKeys.PREMISE_ACCOUNT_SEARCHOPTIONS + RequestLanguage.ToString(), out returnData))
            {
                try
                {
                    // Request for Master value
                    premiseMasterValueRequest.logintype = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? "L" : "A";
                    premiseMasterValueRequest.searchflag = "F";
                    premiseMasterValueRequest.sessionid = string.Empty; //!string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? CurrentPrincipal.SessionToken : null;

                    premiseMasterValueRequest.searchinput = new SearchInput
                    {
                        key = "",
                        value = ""
                    };
                    premiseMasterValueRequest.downloadFlag = string.Empty;

                    var response = PremiseNumberSearchClient.PremiseNumberSearch(premiseMasterValueRequest, RequestLanguage, Request.Segment());

                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        returnData = response.Payload.masterValueList.Select(x => new PremiseNumSearchOptions
                        {
                            key = x.key,
                            value = x.value,
                            dummy = x.dummy
                        }).OrderBy(x => x.key).ToList();
                        CacheProvider.Store(CacheKeys.PREMISE_ACCOUNT_SEARCHOPTIONS + RequestLanguage.ToString(), new CacheItem<List<PremiseNumSearchOptions>>(returnData, TimeSpan.FromMinutes(40)));
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            return returnData;
        }

        #endregion GetAccountTypeList

        #region DownloadSearchResultList

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DownloadSearchResultList(string pnstno, string pnsstno)
        {
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.xls";
            string fileMimeType = "application/vnd.ms-excel";  //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet

            PremiseNumberSearchRequest premiseNumSearchRequest = new PremiseNumberSearchRequest();
            try
            {
                premiseNumSearchRequest.logintype = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? "L" : "A";
                premiseNumSearchRequest.searchflag = "R";
                premiseNumSearchRequest.sessionid = string.Empty;
                premiseNumSearchRequest.searchinput = new SearchInput
                {
                    key = pnsstno,
                    value = pnstno
                };
                premiseNumSearchRequest.downloadFlag = "Y";

                var response = PremiseNumberSearchClient.PremiseNumberSearch(premiseNumSearchRequest, RequestLanguage, Request.Segment());

                if (response != null && response.Payload != null && response.Succeeded)
                {
                    downloadFile = Convert.FromBase64String(response.Payload.content);
                    fileName = getClearstr(string.Format(Translate.Text("PNS_FileDownloadName"), Convert.ToString(pnstno ?? Guid.NewGuid().ToString()))) + Translate.Text("PNS_FileExtension");
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return File(downloadFile, fileMimeType, fileName);
        }

        #endregion DownloadSearchResultList
    }
}