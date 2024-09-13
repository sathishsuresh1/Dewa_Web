using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.ContactDetails;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DEWAXP.Feature.Account.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class ContactDetailsController : BaseApiController
    {
        public HttpResponseMessage Get(string id)
        {
            var contactDetailsResponse = DewaApiClient.GetContactDetails(CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
            //var accountDetailsResponse = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
            var accountDetailsResponse = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

            if (contactDetailsResponse.Succeeded && accountDetailsResponse.Succeeded)
            {
                var accountDetails = accountDetailsResponse.Payload.First(x => x.AccountNumber == id);

                return Request.CreateResponse(HttpStatusCode.OK, ContactDetails.From(contactDetailsResponse.Payload, accountDetails));
            }

            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, string.Concat(contactDetailsResponse.Message, accountDetailsResponse.Message));
        }
    }
}