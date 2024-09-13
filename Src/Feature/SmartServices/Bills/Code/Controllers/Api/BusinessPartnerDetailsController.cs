using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DEWAXP.Feature.Bills.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class BusinessPartnerDetailsController : BaseApiController
    {
        public HttpResponseMessage Get(string id)
        {
            BusinessPartner bpmodel = new BusinessPartner();
            string emiratesid = string.Empty;
            string emiratesidexpiry = string.Empty;
            var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (UserDetails != null)
            {
                bpmodel = UserDetails.Payload.BusinessPartners.Where(x => x.businesspartnernumber == "00" + id).FirstOrDefault();
                if (bpmodel != null)
                {
                    if (!string.IsNullOrWhiteSpace(bpmodel.mobilenumber))
                    {
                        bpmodel.mobilenumber = bpmodel.mobilenumber.RemoveMobileNumberZeroPrefix();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, bpmodel);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.OK, string.Empty);
        }
    }
}