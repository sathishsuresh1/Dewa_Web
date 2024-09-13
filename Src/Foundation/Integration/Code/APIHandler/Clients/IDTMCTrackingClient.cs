using DEWAXP.Foundation.Integration.APIHandler.Models.Request.DTMCTracking;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCTracking;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IDTMCTrackingClient
    {
        ServiceResponse<LocationStatusResponse> GetLocationStatus(LocationStatusRequest request, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<NotificationStatusResponse> GetNotificationStatus(NotificationStatusRequest request, RequestSegment segment = RequestSegment.Desktop);
    }
}
