using DEWAXP.Feature.SupplyManagement.Models.Complaints;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.ContactDetails;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DEWAXP.Feature.SupplyManagement.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class TrackComplaintsController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Account(string id, string o = null)
        {
            // Fetch contact details for the selected account
            var response = DewaApiClient.GetContactDetails(CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                var contactDetails = ContactDetails.From(response.Payload);

                // Fetch the selected account details
                //var accountListResponse = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
                var accountListResponse = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

                if (accountListResponse!=null && accountListResponse.Succeeded && accountListResponse.Payload!=null)
                {
                    var selectedAccount = accountListResponse.Payload.FirstOrDefault(al => al.AccountNumber == id);

                    // Fetch and map the complaint track requests
                    var bpNumber = selectedAccount.BusinessPartnerNumber;
                    var accountNumber = selectedAccount.AccountNumber;
                    var enquiriesResponse = DewaApiClient.GetCustomerEnquiries(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, bpNumber, accountNumber, RequestLanguage, Request.Segment(), o);

                    if (enquiriesResponse!=null && enquiriesResponse.Succeeded && enquiriesResponse.Payload!=null)
                    {
                        var enquiries = enquiriesResponse.Payload;
                        var complaintRequests = enquiries.Select(ComplaintRequest.From).ToList();
                        var trackRequest = new TrackComplaintRequests(complaintRequests, contactDetails);
                        return Request.CreateResponse(HttpStatusCode.OK, trackRequest);
                    }
                }
            }

            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, response.Message);
        }
    }
}