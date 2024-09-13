using DEWAXP.Foundation.Integration.APIHandler.Models.Response.LocationDataPoints;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DEWAXP.Foundation.Integration.APIHandler.Models.Request.LocationDataPoints.DataPointsRequest;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface ILocationDataPointsClient
    {
        ServiceResponse<DataPointsResponse> GetMapDataPoints(Root request);
    }
}
