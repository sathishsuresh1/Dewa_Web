using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Models.ConsumptionStatistics;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Enums;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class NeighbourhoodConsumptionStatisticsController : BaseApiController
    {
        public HttpResponseMessage Get([FromUri] NeighbourHoodComparisonRequest request)
        {
            var consumptionResponse = DewaApiClient.GetComparativeConsumption(request.BehaviourIndicator, CurrentPrincipal.SessionToken, request.AccountNumber, request.PremiseNumber, RequestLanguage, Request.Segment());
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
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No consumption data available");
        }

        [DataContract]
        public class NeighbourHoodComparisonRequest
        {
            [DataMember]
            public string AccountNumber { get; set; }

            [DataMember]
            public string PremiseNumber { get; set; }

            [DataMember]
            public string BehaviourIndicator { get; set; }
        }
    }
}