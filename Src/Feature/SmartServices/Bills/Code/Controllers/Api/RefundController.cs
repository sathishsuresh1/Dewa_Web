using DEWAXP.Feature.Bills.Models.Refund;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using Sitecore.Globalization;
using System;
using System.Net;
using System.Net.Http;
namespace DEWAXP.Feature.Bills.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class RefundController : BaseApiController
    {
        public HttpResponseMessage GetRefundHistory(string id, string bp, int page = 1, string sortby = "")
        {
            string transactionaccount;
            string refundlang;
            refundHistory refundHistory;
            RefundHistoryModel returnData = null;
            ibanListchild[] IbanValues = null;
            if (CacheProvider.TryGet(CacheKeys.SELECTED_REFUNDACCOUNT, out transactionaccount))
            {
                if (CacheProvider.TryGet(CacheKeys.SELECTED_REFUNDACCOUNT_LANGUAGE, out refundlang))
                {
                    if (transactionaccount.Equals(id) && refundlang.Equals(RequestLanguage.ToString()))
                    {
                        if (CacheProvider.TryGet(CacheKeys.SELECTED_REFUND, out refundHistory))
                        {
                            returnData = RefundHistoryModel.From(refundHistory, page, sortby);
                        }
                    }
                }
            }

            if (returnData == null)
            {
                var response = DewaApiClient.GetRefundHistory(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, bp, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    if (response.Payload != null && response.Payload.responsecode != "105")
                    {
                        CacheProvider.Store(CacheKeys.SELECTED_REFUND, new CacheItem<refundHistory>(response.Payload, TimeSpan.FromMinutes(20)));
                        CacheProvider.Store(CacheKeys.SELECTED_REFUNDACCOUNT, new CacheItem<string>(id, TimeSpan.FromMinutes(20)));
                        CacheProvider.Store(CacheKeys.SELECTED_REFUNDACCOUNT_LANGUAGE, new CacheItem<string>(RequestLanguage.ToString(), TimeSpan.FromMinutes(20)));
                        returnData = RefundHistoryModel.From(response.Payload, 1, string.Empty);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = false, Message = response.Message });
                    }
                }
            }

            if (returnData != null)
            {
                if (!CacheProvider.TryGet("IBANLIST" + id, out IbanValues))
                {
                    returnData.IbanValues = DewaApiClient.IBANList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, bp, RequestLanguage, Request.Segment())?.Payload?.IBAN;
                    if (returnData.IbanValues != null)
                    {
                        CacheProvider.Store("IBANLIST" + id, new CacheItem<ibanListchild[]>(returnData.IbanValues, TimeSpan.FromMinutes(20)));
                    }
                }
                else
                {
                    returnData.IbanValues = IbanValues;
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, Result = returnData });
            }

            return Request.CreateErrorResponse(HttpStatusCode.GatewayTimeout, Translate.Text("timeout error message"));
        }
    }
}