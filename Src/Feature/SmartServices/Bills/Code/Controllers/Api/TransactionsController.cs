using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.Bills;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using Sitecore.Globalization;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class TransactionsController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Account(string id, int page = 1, string sortby = "")
        {
            AccountCache transactionaccount;
            BillHistoryResponse paymentHistoryDetail;
            if (CacheProvider.TryGet(CacheKeys.SELECTED_TRANSACTIONACCOUNT, out transactionaccount))
            {
                if (transactionaccount != null && transactionaccount.accountnumber.Equals(id) && transactionaccount.RequestLanguage.Equals(RequestLanguage))
                {
                    if (CacheProvider.TryGet(CacheKeys.SELECTED_TRANSACTION, out paymentHistoryDetail))
                    {
                        CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
                        return Request.CreateResponse(HttpStatusCode.OK, TransactionHistoryModel.From(paymentHistoryDetail, page, sortby));
                    }
                }
            }
            CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
            var response = SmartCustomerClient.GetBillPaymentHistory(
                        new BillHistoryRequest
                        {
                            contractaccountnumber = id,
                            sessionid = CurrentPrincipal.SessionToken,
                            userid = CurrentPrincipal.UserId
                        }, RequestLanguage, Request.Segment());

            //DewaApiClient.GetPaymentHistory(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
            //var response = DewaApiClient.GetTransactionHistory(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                if (response.Payload != null && response.Payload.Responsecode != "105")
                {
                    CacheProvider.Store(CacheKeys.SELECTED_TRANSACTION, new CacheItem<BillHistoryResponse>(response.Payload, TimeSpan.FromMinutes(20)));
                    CacheProvider.Store(CacheKeys.SELECTED_TRANSACTIONACCOUNT, new CacheItem<AccountCache>(new AccountCache { accountnumber = id, RequestLanguage = RequestLanguage }, TimeSpan.FromMinutes(20)));
                    return Request.CreateResponse(HttpStatusCode.OK, TransactionHistoryModel.From(response.Payload, 1, string.Empty));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new TransactionHistoryModel());
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, response.Message);
        }
    }
}