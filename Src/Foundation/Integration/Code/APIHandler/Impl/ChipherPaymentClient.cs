using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SecuredPayment;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SecuredPayment;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IChipherPaymentClient), Lifetime = Lifetime.Transient)]
    public class ChipherPaymentClient : BaseApiDewaGateway, Clients.IChipherPaymentClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";
        public ServiceResponse<CipherPaymentDetailResponse> GenerateEpayToken(CipherPaymentDetailRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {

                request.paymentparams.vendorid = GetVendorId(segment);
                //request.paymentparams.appversion = AppVersion;
                //request.paymentparams.appidentifier = segment.Identifier();
                //request.paymentparams.mobileosversion = AppVersion;
                request.paymentparams.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "epaytoken", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CipherPaymentDetailResponse _Response = CustomJsonConvertor.DeserializeObject<CipherPaymentDetailResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<CipherPaymentDetailResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<CipherPaymentDetailResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<CipherPaymentDetailResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<CipherPaymentDetailResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
