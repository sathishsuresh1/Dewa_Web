using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Controllers.Api
{
    public class MoveInUserController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(string userId, string password, string confirmPass)
        {
            var response = DewaApiClient.MoveIn(userId,
                password,
                confirmPass,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                string.Empty,
                string.Empty,
                null,
                string.Empty,
                string.Empty,
                RequestLanguage,
                Request.Segment());

            if (response.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response.Payload);
            }
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, response.Message);
        }
    }
}