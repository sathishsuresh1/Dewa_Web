using DEWAXP.Feature.GeneralServices.Models;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    [TwoPhaseAuthorize]
    public class RoadworksController : BaseController
    {
        [HttpGet]
        public PartialViewResult Roadworks()
        {
            var response = DewaApiClient.GetRoadWorksList(CurrentPrincipal.SessionToken, CurrentPrincipal.UserId, RequestLanguage, Request.Segment());
            if (!response.Succeeded)
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }

            var viewModel = new RoadWorks
            {
                DataLocations = SerializeRoadworksLocations(response),
                Message = response.Message,
                Succeeded = response.Succeeded
            };

            return PartialView("~/Views/Feature/GeneralServices/Roadworks/_Roadworks.cshtml", viewModel);
        }

        private static string SerializeRoadworksLocations(ServiceResponse<RoadWorksListResponse> getRoadWorksListResponse)
        {
            if (getRoadWorksListResponse.Succeeded && getRoadWorksListResponse.Payload != null)
            {
                return new JavaScriptSerializer().Serialize(getRoadWorksListResponse.Payload.RoadWorks);
            }
            return new JavaScriptSerializer().Serialize(new List<RoadWork>());
        }
    }
}