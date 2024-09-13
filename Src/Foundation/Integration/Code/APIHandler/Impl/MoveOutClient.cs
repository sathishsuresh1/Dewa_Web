using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.DewaSvc;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IMoveOutClient), Lifetime = Lifetime.Transient)]
    public class MoveOutClient : BaseApiDewaGateway, IMoveOutClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";
        public ServiceResponse<MoveOutResponse> SetMoveOutRequestV2(MoveoutRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                request.lang = language.Code();
                request.moveoutversion = "03";

                var apiRequest = new
                {
                    moveoutparams = request
                };
                IRestResponse response = DewaApiExecute(BaseApiUrl, "moveout", apiRequest, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MoveOutResponse _Response = CustomJsonConvertor.DeserializeObject<MoveOutResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<MoveOutResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<MoveOutResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<MoveOutResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<MoveOutResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
