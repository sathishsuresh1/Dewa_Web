using DEWAXP.Foundation.Integration.APIHandler.Models.Request.DTMCInsightsReport;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.meterreading;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCInsightsReport;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.meterreading;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IDTMCInsightsReportClient
    {
        ServiceResponse<GetWaterAIResponse> GetWaterAI(GetWaterAIRequest request, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<GetWaterAIConsumptionResponse> GetWaterAIConsumption(GetWaterAIConsumptionRequest request, RequestSegment segment = RequestSegment.Desktop);

        /// <summary>
        /// UnBilled Consumption
        /// </summary>
        /// <param name="request"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<MeterreadingResponse> GetMeterReading(MeterreadingRequest request, RequestSegment segment = RequestSegment.Desktop);
    }
}
