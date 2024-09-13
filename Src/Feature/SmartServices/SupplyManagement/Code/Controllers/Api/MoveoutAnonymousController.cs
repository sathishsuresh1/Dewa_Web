using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Models.MoveOut.v3;
using DEWAXP.Foundation.DataAnnotations;
using DEWAXP.Foundation.Helpers.Extensions;
using System.Net;
using System.Net.Http;

namespace DEWAXP.Feature.SupplyManagement.Controllers.Api
{
    public class MoveoutAnonymousController : BaseApiController
    {
        public HttpResponseMessage Get()
        {
            //Modified by Syed Shujaat Ali for Future center
            var _fc = FetchFutureCenterValues();

            string mobile = null;
            string email = null;

            MoveOutAnonymous model = new MoveOutAnonymous();
            CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out model);

            var _emailAdressFormat = new EmailAddressAttribute();

            if (_emailAdressFormat.IsValid(model.SelectedOptions))
            {
                email = model.SelectedOptions;
            }
            else
            {
                mobile = model.SelectedOptions;
            }

            var _response = DewaApiClient.SetMoveOutwithOTP(model.AccountNumber, model.PremiseNumber, model.OTPflag, _fc.Branch, RequestLanguage, Request.Segment(), mobile, email);

            if (_response.Payload.@return.responsecode.Equals("000"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { description = _response.Payload.@return.description, message = _response.Message });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { description = _response.Payload.@return.description, message = _response.Message });
        }
    }
}