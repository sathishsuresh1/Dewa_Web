using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Models.MoveOut;
using DEWAXP.Foundation.Content.Models.UpdateIBAN;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using _modelrestapi = DEWAXP.Foundation.Integration.APIHandler.Models;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    public class IBANListController : BaseApiController
    {
        //[HttpGet]
        //public HttpResponseMessage bp(string accountnumber, string bpNumber)
        //{
        //    return MoveOutV2(accountnumber, bpNumber);
        //}
        [HttpGet]
        public HttpResponseMessage Getaccount(string accountnumber, string bpNumber)
        {
            UpdateIBANModel model = null;

            MoveOutResult moveoutResult = new MoveOutResult();

            string[] selectedaccount = null;

            if (accountnumber.Contains(','))
                selectedaccount = accountnumber.Split(',');
            else
                selectedaccount = new string[] { accountnumber };

            //var state = new MoveOutWorkflowState();
            List<_modelrestapi.Request.MoveOut.Moveout_AccountsIn_Request> accountlist = new List<_modelrestapi.Request.MoveOut.Moveout_AccountsIn_Request>();
            Array.ForEach(selectedaccount, x => accountlist.Add(new _modelrestapi.Request.MoveOut.Moveout_AccountsIn_Request { contractaccountnumber = x.ToString() }));
            ServiceResponse<AccountDetails[]> cacheresponse;
            _modelrestapi.Request.MoveOut.MoveoutRequest moveoutParam = null; _modelrestapi.Request.MoveOut.MoveoutRequest moveoutParamUpdate = null;
            if (!CacheProvider.TryGet("RF", out cacheresponse) || cacheresponse.Payload.Length < 1)
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, "timeout error message");
            }
            if (cacheresponse.Payload.Where(x => FormatContractAccount(x.AccountNumber).Equals(FormatContractAccount(accountnumber))).FirstOrDefault().BillingClass.Equals(BillingClassification.ElectricVehicle))
            {
                moveoutParam = new _modelrestapi.Request.MoveOut.MoveoutRequest
                {
                    accountlist = accountlist,
                    channel = "M",
                    executionflag = "R",
                    applicationflag = "R",
                    notificationtype = "EV"
                };

                moveoutParamUpdate = new _modelrestapi.Request.MoveOut.MoveoutRequest
                {
                    accountlist = accountlist,
                    channel = "M",
                    executionflag = "R",
                    applicationflag = "U",
                    notificationtype = "EV"
                };
            }
            else
            {
                moveoutParam = new _modelrestapi.Request.MoveOut.MoveoutRequest
                {
                    accountlist = accountlist,
                    channel = "M",
                    executionflag = "R",
                    applicationflag = "M",
                    notificationtype = "RF"
                };

                moveoutParamUpdate = new _modelrestapi.Request.MoveOut.MoveoutRequest
                {
                    accountlist = accountlist,
                    channel = "M",
                    executionflag = "R",
                    applicationflag = "U",
                    notificationtype = "RF"
                };
            }

            if (moveoutParam != null)
            {
                moveoutParam.userid = CurrentPrincipal.UserId;
                moveoutParam.sessionid = CurrentPrincipal.SessionToken;
            }

            if (moveoutParamUpdate != null)
            {
                moveoutParamUpdate.userid = CurrentPrincipal.UserId;
                moveoutParamUpdate.sessionid = CurrentPrincipal.SessionToken;
            }

            var response = MoveOutClient.SetMoveOutRequestV2(moveoutParam, Request.Segment(), RequestLanguage);

            var updateIban = MoveOutClient.SetMoveOutRequestV2(moveoutParamUpdate, Request.Segment(), RequestLanguage);

            if (response.Succeeded && updateIban.Succeeded && updateIban.Payload != null && response.Payload != null && response.Payload != null && response.Payload.accountslist != null)
            {
                model = new UpdateIBANModel();
                model.IsRequestChequeAllowed = response.Payload.accountslist[0].okcheque == "Y" ? true : false;
                model.IsRequestIbanAllowed = response.Payload.accountslist[0].okiban == "Y" ? true : false;
                model.IsRequestTransferAllowed = response.Payload.accountslist[0].okaccounttransfer == "Y" ? true : false;
                model.IsUpdateChequeAllowed = updateIban.Payload.accountslist[0].okcheque == "Y" ? true : false;
                model.IsUpdateIbanAllowed = updateIban.Payload.accountslist[0].okiban == "Y" ? true : false;
                model.IsWesternUnionAllowed = response.Payload.accountslist[0].okwesternunion == "Y" ? true : false;
                model.AccountSelected = accountnumber;
                model.SelectedBusinessPartnerNumber = bpNumber;
                model.BusinessPartnerType = response.Payload.accountslist[0].businesspartnercategory;
                model.CustomerName = response.Payload.accountslist[0].contractaccountname;
                model.CustomerFirstName = response.Payload.accountslist[0].customerfirstname;
                model.CustomerLastName = response.Payload.accountslist[0].customerlastname;
                model.CustomerAccountnumber = response.Payload.accountslist[0].contractaccountnumber;
                model.CreditBalance = response.Payload.accountslist[0].creditbalance;
                model.Emailid = response.Payload.accountslist[0].email;
                model.MaskedEmailid = response.Payload.accountslist[0].maskedemail;
                model.nomoveoutpayflag = response.Payload.accountslist[0].nomoveoutpayflag;
                model.MobileNumber = response.Payload.accountslist[0].mobile;
                model.MaskedMobileNumber = response.Payload.accountslist[0].maskedmobile;

                List<_modelrestapi.Response.MoveOut.MoveOutTransferAccountsResponse> tList = null;
                if (response.Payload.trnsferlist != null)
                {
                    tList = response.Payload.trnsferlist.ToList();
                    tList.Insert(0, new _modelrestapi.Response.MoveOut.MoveOutTransferAccountsResponse() { contractaccountname = Translate.Text("refund.selectaccount"), contractaccountnumber = "0" });
                }
                model.lsttranferaccount = tList;
                model.ValidBankCodes = (response.Payload.banklist != null && response.Payload.banklist.Any()) ? new Tuple<string, string>(string.Join(",", response.Payload.banklist.Where(y => y.bptype == response.Payload.accountslist[0].businesspartnercategory && !y.bankkey.Equals("*")).Select(y => y.bankkey)), string.Join(", ", response.Payload.banklist.Where(y => y.bptype == response.Payload.accountslist[0].businesspartnercategory && !y.bankkey.Equals("*")).Select(y => y.bankname))) : new Tuple<string, string>("", "");
                model.divisionlist = response.Payload.divisionlist;
                if (model.divisionlist != null && model.divisionlist.Count() > 0)
                {
                    model.TotalPendingBalance = Convert.ToString(model.divisionlist.Where(x => !string.IsNullOrWhiteSpace(x.totalamount))?.Sum(x => Convert.ToDecimal(x.totalamount)));

                    model.PaymentAccountList = string.Join(",", model.divisionlist.Select(x => x.contractaccountnumber));
                    model.PaymentAmountList = string.Join(",", model.divisionlist.Select(x => x.totalamount));
                    model.PaymentBP_List = string.Join(",", model.divisionlist.Select(x => x.businesspartner));
                }
                if (model.IsRequestIbanAllowed || model.IsUpdateIbanAllowed)
                {
                    var bpresponse = DewaApiClient.IBANList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, "", bpNumber, RequestLanguage, Request.Segment());
                    if (bpresponse.Succeeded)
                    {
                        model.IbanValues = bpresponse.Payload.IBAN;
                    }
                }
                CacheProvider.Store($"IBANDetail_ac{accountnumber}_bp{bpNumber}", new CacheItem<UpdateIBANModel>(model, TimeSpan.FromHours(1)));

                return Request.CreateResponse(HttpStatusCode.OK, model);
            }

            return Request.CreateErrorResponse(HttpStatusCode.OK, response?.Payload?.notificationlist?[0]?.message ?? response.Message);
        }
        [HttpGet]
        public HttpResponseMessage GetMasterDataWUlist()
        {
            List<SelectListItem> countrylist = new List<SelectListItem>();
            try
            {
                var response = DewaApiClient.GetMasterDataWUlist(string.Empty, string.Empty, "A", RequestLanguage, Request.Segment());

                if (response != null && response.Payload != null)
                {
                    GetMasterDataWUResponse _response;
                    _response = response.Payload;
                    CacheProvider.Store(CacheKeys.Western_Union_Response, new CacheItem<GetMasterDataWUResponse>(_response));

                    var countrygrouped = _response.@return.masterdatadetails.GroupBy(x => new
                    {
                        x.country,
                        x.countryname
                    });
                    var selectgroup = countrygrouped.Select(x => x);

                    foreach (var item in selectgroup)
                    {
                        countrylist.Add(new SelectListItem { Text = item.Key.countryname, Value = item.Key.country });
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, countrylist);
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return Request.CreateResponse(HttpStatusCode.OK, countrylist);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetWUStateList(string SelectedCountry)
        {
            List<SelectListItem> statelist = new List<SelectListItem>();
            List<SelectListItem> currencylist = new List<SelectListItem>();
            List<SelectListItem> citylist = new List<SelectListItem>();
            try
            {
                GetMasterDataWUResponse _resp;
                if (!CacheProvider.TryGet(CacheKeys.Western_Union_Response, out _resp))
                {
                    var response = DewaApiClient.GetMasterDataWUlist(string.Empty, string.Empty, "A", RequestLanguage, Request.Segment());
                    _resp = response.Payload;
                }

                // Currency check
                var currencygrouped = _resp.@return.masterdatadetails.Where(x => x.country.Equals(SelectedCountry)).GroupBy(x => new
                {
                    x.currencykey
                });
                var currencygroup = currencygrouped.Select(x => x);
                foreach (var item in currencygroup)
                {
                    currencylist.Add(new SelectListItem { Text = item.Key.currencykey, Value = item.Key.currencykey });
                }

                // State & City check
                var selectedcountry = _resp.@return.masterdatadetails.Where(x => x.country.Equals(SelectedCountry)).FirstOrDefault();

                if (!selectedcountry.nostateflag.Equals("X"))
                {
                    var stategrouped = _resp.@return.masterdatadetails.Where(x => x.country.Equals(SelectedCountry)).GroupBy(x => new
                    {
                        x.state,
                        x.statename
                    });
                    var selectgroup = stategrouped.Select(x => x);
                    foreach (var item in selectgroup)
                    {
                        statelist.Add(new SelectListItem { Text = item.Key.statename, Value = item.Key.state });
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { stateflag = true, cityflag = false, currencylst = currencylist, statelst = statelist });
                }
                else if (!selectedcountry.nocityflag.Equals("X"))
                {
                    var citygrouped = _resp.@return.masterdatadetails.Where(x => x.country.Equals(SelectedCountry)).GroupBy(x => new
                    {
                        x.city
                    });
                    var selectgroup = citygrouped.Select(x => x);
                    foreach (var item in selectgroup)
                    {
                        citylist.Add(new SelectListItem { Text = item.Key.city, Value = item.Key.city });
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { stateflag = false, cityflag = true, currencylst = currencylist, citylst = citylist });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { stateflag = false, cityflag = false, currencylst = currencylist });
                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return Request.CreateResponse(HttpStatusCode.OK, statelist);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetWUCityList(string SelectedState)
        {
            List<SelectListItem> citylist = new List<SelectListItem>();
            try
            {
                GetMasterDataWUResponse _resp;
                if (!CacheProvider.TryGet(CacheKeys.Western_Union_Response, out _resp))
                {
                    var response = DewaApiClient.GetMasterDataWUlist(string.Empty, string.Empty, "A", RequestLanguage, Request.Segment());
                    _resp = response.Payload;
                }

                var selectedstate = _resp.@return.masterdatadetails.Where(x => x.state.Equals(SelectedState)).FirstOrDefault();

                if (!selectedstate.nocityflag.Equals("X"))
                {
                    var citygrouped = _resp.@return.masterdatadetails.Where(x => x.state.Equals(SelectedState)).GroupBy(x => new
                    {
                        x.city
                    });
                    var selectgroup = citygrouped.Select(x => x);
                    foreach (var item in selectgroup)
                    {
                        citylist.Add(new SelectListItem { Text = item.Key.city, Value = item.Key.city });
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { cityflag = true, citylst = citylist });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { cityflag = false });
                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return Request.CreateResponse(HttpStatusCode.OK, citylist);
            }
        }
    }
}