using DEWAXP.Feature.SupplyManagement.Models.MoveIn;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DEWAXP.Feature.SupplyManagement.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class BusinessPartnerEmiratesSelectionController : BaseApiController
    {
        public HttpResponseMessage Get(string id)
        {
            BPandVATViewModel bpmodel = new BPandVATViewModel();
            string emiratesid = string.Empty;
            string emiratesidexpiry = string.Empty;
            var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (UserDetails != null)
            {
                bpmodel.bp = UserDetails.Payload.BusinessPartners.Where(x => x.businesspartnernumber == id).FirstOrDefault();
                var VatDetails = DewaApiClient.GetVatNumber(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (VatDetails.Succeeded)
                {
                    bpmodel.vatdetails = VatDetails.Payload.@return.bpVatList.ToList().Where(x => x.bpnumber == id).FirstOrDefault();
                }
                return Request.CreateResponse(HttpStatusCode.OK, bpmodel);
            }
            return Request.CreateErrorResponse(HttpStatusCode.OK, string.Empty);
        }
    }
}