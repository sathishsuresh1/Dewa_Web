using DEWAXP.Feature.Dashboard.Models.Enquiries;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Helpers.Extensions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class EnquiriesController : BaseApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Account(string id, string bpNumber)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(bpNumber))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to find account.");
                    }

                    var response = DewaApiClient.GetCustomerEnquiries(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, bpNumber, id, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        var enquires = response.Payload.Select(EnquiryModel.From);

                        return Request.CreateResponse(HttpStatusCode.OK, enquires);
                    }
                    ErrorMessage = response.Message;
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage);
            }))());
        }
    }
}