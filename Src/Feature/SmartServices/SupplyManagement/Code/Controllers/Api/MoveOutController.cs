using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Models.MoveOut;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using _modelrestapi = DEWAXP.Foundation.Integration.APIHandler.Models;

namespace DEWAXP.Feature.SupplyManagement.Controllers.Api
{
    public class MoveOutController : BaseApiController
    {
        public HttpResponseMessage Get(string accounts, bool evflag = false, string cardnumber = "")
        {
            MoveOutResult moveoutResult = new MoveOutResult();
            if (!string.IsNullOrWhiteSpace(accounts))
            {
                string[] selectedaccount = null;

                if (accounts.Contains(','))
                    selectedaccount = accounts.Split(',');
                else
                    selectedaccount = new string[] { accounts };
                _modelrestapi.Request.MoveOut.MoveoutRequest moveoutParam = null;
                var state = new MoveOutWorkflowState();
                List<_modelrestapi.Request.MoveOut.Moveout_AccountsIn_Request> accountlist = new List<_modelrestapi.Request.MoveOut.Moveout_AccountsIn_Request>();

                if (evflag)
                {
                    CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(accounts.TrimStart(new char[] { '0' }), Times.Once));
                    Array.ForEach(selectedaccount, x => accountlist.Add(new _modelrestapi.Request.MoveOut.Moveout_AccountsIn_Request { contractaccountnumber = x.ToString(), additionalinput1 = cardnumber }));
                }
                else
                {
                    Array.ForEach(selectedaccount, x => accountlist.Add(new _modelrestapi.Request.MoveOut.Moveout_AccountsIn_Request { contractaccountnumber = x.ToString() }));
                }

                //List<accountsIn> accountlist = new List<accountsIn>();
                //Array.ForEach(selectedaccount, x => accountlist.Add(new accountsIn { contractaccountnumber = x.ToString() }));

                //moveOutParams moveoutParam = new moveOutParams
                //{
                //    accountlist = accountlist.ToArray(),
                //    channel = "W",
                //    executionflag = "R",
                //    applicationflag = "M"
                //};
                moveoutParam = new _modelrestapi.Request.MoveOut.MoveoutRequest
                {
                    accountlist = accountlist,
                    channel = "W",
                    executionflag = "R",
                    applicationflag = "M",
                    userid = CurrentPrincipal.UserId,
                    sessionid = CurrentPrincipal.SessionToken
                };
                if (evflag)
                {
                    moveoutParam.notificationtype = "EV";
                }
                var response = MoveOutClient.SetMoveOutRequestV2(moveoutParam, Request.Segment(), RequestLanguage);
                //var response = DewaApiClient.SetMoveOutRequest(moveoutParam, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null && response.Payload != null && response.Payload.accountslist != null)
                {
                    moveoutResult.totalamounttopay = response.Payload.accountslist.ToList().Select(x => x.okpaymenttocollect.Equals("Y")).Any() ? response.Payload.accountslist.ToList().Where(y => y.okpaymenttocollect.Equals("Y")).Select(x => Double.Parse(x.amounttocollect)).Sum() : 0.0;
                    moveoutResult.iscustomer = response.Payload.accountslist.ToList().Where(x => x.businesspartnercategory.Equals("1")).HasAny();
                    moveoutResult.proceed = moveoutResult.totalamounttopay > 0.0 ? false : true;
                    moveoutResult.issuccess = true;
                    moveoutResult.details = response.Payload.accountslist;
                    moveoutResult.divisionlist = response.Payload.divisionlist;
                    if (moveoutResult.divisionlist != null && moveoutResult.divisionlist.Count() > 0)
                    {
                        moveoutResult.TotalPendingBalance = Convert.ToString(moveoutResult.divisionlist.Where(x => !string.IsNullOrWhiteSpace(x.totalamount))?.Sum(x => Convert.ToDecimal(x.totalamount)));
                        if (!string.IsNullOrWhiteSpace(moveoutResult.TotalPendingBalance) && moveoutResult.TotalPendingBalance != "0")
                            moveoutResult.proceed = false;

                        moveoutResult.PaymentAccountList = string.Join(",", moveoutResult.divisionlist.Select(x => x.contractaccountnumber));
                        moveoutResult.PaymentAmountList = string.Join(",", moveoutResult.divisionlist.Select(x => x.totalamount));
                        moveoutResult.PaymentBP_List = string.Join(",", moveoutResult.divisionlist.Select(x => x.businesspartner));
                    }
                    if (evflag)
                    {
                        moveoutResult.evCardnumber = cardnumber;
                        CacheProvider.Store(CacheKeys.EV_DEACTIVATE_SELECTEDACCOUNT, new CacheItem<string>(accounts, TimeSpan.FromMinutes(20)));
                        CacheProvider.Store(CacheKeys.EV_DEACTIVATE_RESULT, new CacheItem<MoveOutState>(new MoveOutState { moveoutdetails = response.Payload, moveoutresult = moveoutResult }, TimeSpan.FromMinutes(20)));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.MOVE_OUT_RESULT, new CacheItem<MoveOutState>(new MoveOutState { moveoutdetails = response.Payload, moveoutresult = moveoutResult }, TimeSpan.FromMinutes(20)));
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, moveoutResult);
                }

                if (!response.Succeeded && response.Payload != null && response.Payload != null
                    && response.Payload.notificationlist != null && response.Payload.notificationlist.Any())
                {
                    var duplicateRequests = new List<string>();
                    foreach (var duplicateRequest in response.Payload.notificationlist)
                    {
                        duplicateRequests.Add(duplicateRequest.message + " for Account: " + duplicateRequest.contractaccountnumber);
                    }
                    moveoutResult.duplicaterequests = duplicateRequests;
                }
                moveoutResult.errormessage = response.Message;
            }
            moveoutResult.issuccess = false;
            moveoutResult.errormessage = !string.IsNullOrEmpty(moveoutResult.errormessage) && !string.IsNullOrWhiteSpace(moveoutResult.errormessage) ? moveoutResult.errormessage : "Select account";
            return Request.CreateResponse(HttpStatusCode.OK, moveoutResult);
        }
    }
}