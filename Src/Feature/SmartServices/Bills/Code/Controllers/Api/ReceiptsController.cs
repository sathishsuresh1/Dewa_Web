using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.Bills;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class ReceiptsController : BaseApiController
    {
        public HttpResponseMessage Account(string id)
        {
            var response = DewaApiClient.GetReceipts(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id,null, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                var receipts = response.Payload.Distinct(new ReceiptEqualityComparer()).Select(ReceiptModel.From);

                return Request.CreateResponse(HttpStatusCode.OK, receipts);
            }
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, response.Message);
        }

        private class ReceiptEqualityComparer : IEqualityComparer<Receipt>
        {
            public bool Equals(Receipt x, Receipt y)
            {
                return string.Equals(x.PaymentGatewayTransactionReference, y.PaymentGatewayTransactionReference);
            }

            public int GetHashCode(Receipt obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}