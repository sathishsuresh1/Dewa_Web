using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.TenderSvc;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.TenderSvc
{
    [Service(typeof(ITenderServiceClient), Lifetime = Lifetime.Transient)]
    public class TenderSoapClient : BaseDewaGateway, ITenderServiceClient
    {
        public ServiceResponse<GetOpenTenderList> GetOpenTenderList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {

                try
                {
                    var response = client.GetOpenTenderList(AppVersion, string.Empty, segment.Identifier(), language.Code());

                    var typedResponse = response.DeserializeAs<GetOpenTenderList>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<GetOpenTenderList>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<GetOpenTenderList>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetOpenTenderList>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetTenderOpeningListResponse> GetTenderOpeningList(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var response = client.GetTenderOpeningList(AppVersion, string.Empty, segment.Identifier(), language.Code());

                    var typedResponse = response.DeserializeAs<GetTenderOpeningListResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<GetTenderOpeningListResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<GetTenderOpeningListResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetTenderOpeningListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<TenderItemResultResponse> GetTenderOpeningResult(string fid, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {

                try
                {
                    var response = client.GetTenderOpeningResult(fid, AppVersion, string.Empty, segment.Identifier(), language.Code());

                    var typedResponse = response.DeserializeAs<TenderItemResultResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<TenderItemResultResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<TenderItemResultResponse>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<TenderItemResultResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #region Service Proxy methods

        private Integration.TenderSvc.TenderServicesSoapClient CreateProxy()
        {
            return new Integration.TenderSvc.TenderServicesSoapClient("TenderServicesSoap", GetEndpointAddress("TenderServicesSoap"));
        }

        #endregion
    }
}
