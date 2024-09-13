using DEWAXP.Feature.EV.Models.EVCharger;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using Sitecore.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace DEWAXP.Feature.EV.Controllers.Api
{
    public class EVGreenCardChargerController : BaseApiController
    {
        private readonly IEVServiceClient _iEVServiceClient;

        public EVGreenCardChargerController()
        {
            this._iEVServiceClient = DependencyResolver.Current.GetService<IEVServiceClient>();
        }

        private IEVServiceClient ServiceClient
        { get { return this._iEVServiceClient; } }
        [HttpGet]
        public HttpResponseMessage GetNotifications(string id, string bp, int page = 1, string sortby = "")
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
            }
            var response = ServiceClient.GetNotifications(bp, CurrentPrincipal.SessionToken, CurrentPrincipal.UserId, "", id, "", RequestLanguage, Request.Segment());
            //DewaApiClient.GetRefundHistory(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, bp, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                if (response.Payload != null && response.Payload.Envelope.Body.GetEVTrackRequestResponse.@return.responsecode == "000")
                {
                    var result = EVNotificationPageModel.MapFrom(response.Payload, page, sortby, RequestLanguage);
                    if (result != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { success = true, Result = result });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, Message = response.Message });
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.GatewayTimeout, Translate.Text("timeout error message"));
        }
        [HttpGet]
        public HttpResponseMessage GetNotificationDetails(string accnumber, string trnumber, string bpnumber)
        {
            if (!string.IsNullOrWhiteSpace(accnumber))
            {
                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(accnumber.TrimStart(new char[] { '0' }), Times.Once));
            }
            var response = ServiceClient.GetNotificationDetail(bpnumber, CurrentPrincipal.SessionToken, CurrentPrincipal.UserId, trnumber, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                var nDetail = response?.Payload?.Envelope?.Body?.GetEVNotificationDetailsResponse?.@return;

                if (nDetail != null && nDetail.responsecode == "000")
                {
                    var result = StatusNotificationDetailModel.MapToModel(nDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, Result = result });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, Message = response.Message });
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.GatewayTimeout, Translate.Text("timeout error message"));
        }
    }
}