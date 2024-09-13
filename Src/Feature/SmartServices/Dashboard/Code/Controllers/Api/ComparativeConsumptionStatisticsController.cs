using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.ConsumptionStatistics;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Enums;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class ComparativeConsumptionStatisticsController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Accounts([FromUri] string[] accounts)
        {
            var consumptionResponse = DewaApiClient.GetComparativeConsumption(CurrentPrincipal.SessionToken, accounts, RequestLanguage, Request.Segment());
            if (consumptionResponse.Succeeded)
            {
                if (consumptionResponse.Payload != null)
                {
                    var @return = DataSeries.Create(consumptionResponse.Payload);
                    if (!@return.Any(series => series.Utility.Equals(MunicipalService.Electricity)))
                    {
                        @return.Add(DataSeries.Null(MunicipalService.Electricity));
                    }

                    if (!@return.Any(series => series.Utility.Equals(MunicipalService.Water)))
                    {
                        @return.Add(DataSeries.Null(MunicipalService.Water));
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { series = @return });
                }
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No consumption data available");
            }
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, consumptionResponse.Message);
        }
    }
}