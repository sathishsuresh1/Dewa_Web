using DEWAXP.Feature.Bills.Models.BusinessPartnerSelectionDetails;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class BusinessPartnerSelectionController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Account(string id)
        {
            //var accountDetailsResponse = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
            var accountDetailsResponse = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());
            if (accountDetailsResponse.Succeeded)
            {
                var accountDetails = accountDetailsResponse.Payload.FirstOrDefault(x => x.AccountNumber == id);
                if (accountDetails == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The requested account could not be found.");
                }
                return Request.CreateResponse(HttpStatusCode.OK, BusinessPartnerDetails.From(accountDetails));
            }
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, accountDetailsResponse.Message);
        }
    }
}