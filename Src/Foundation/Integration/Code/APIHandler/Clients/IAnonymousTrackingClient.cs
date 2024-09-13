using DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IAnonymousTrackingClient
    {
        ServiceResponse<GeneralTrackResponse> GetGeneralTrackDetail(GeneralTrackRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<TrackListResponse> GetNotificationList(TrackListRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<TrackVerifyOtpResponse> VerifyOtp(TrackVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
