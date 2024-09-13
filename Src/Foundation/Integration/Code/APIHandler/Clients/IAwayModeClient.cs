using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.AwayMode;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IAwayModeClient
    {
         ServiceResponse<ManageAwayModeResponse> Manageawaymode(ManageAwayModeRequest request, RequestSegment segment = RequestSegment.Desktop);

        //#region [Async]
        //Task<ServiceResponse<ManageAwayModeResponse>> ManageawaymodeAsync(ManageAwayModeRequest request, RequestSegment segment = RequestSegment.Desktop);
        //#endregion


        ServiceResponse<ConsumptionDataResponse> GetConsumptionData(string  code, RequestSegment segment = RequestSegment.Desktop);

    }
}
