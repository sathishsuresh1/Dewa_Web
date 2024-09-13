using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.RefundHistory;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IRefundHistoryClient), Lifetime = Lifetime.Transient)]
    public class RefundHistoryClient : BaseApiDewaGateway, IRefundHistoryClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        public ServiceResponse<IBANNumberV2Response> IbanNumberv2(IBANNumberV2Request request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {

                request.vendorid = GetVendorId(segment); 
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                request.lang = language.Code();
                IRestResponse response = DewaApiExecute(BaseApiUrl, "ibannumberv2", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IBANNumberV2Response _Response = CustomJsonConvertor.DeserializeObject<IBANNumberV2Response>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<IBANNumberV2Response>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<IBANNumberV2Response>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<IBANNumberV2Response>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<IBANNumberV2Response>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<RefundHistoryResponse> VerifyOtp(RefundHistoryVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop)
        {

            try
            {
                IRestResponse response = DewaApiExecute(BaseApiUrl, "verifyotp", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    RefundHistoryResponse _Response = CustomJsonConvertor.DeserializeObject<RefundHistoryResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<RefundHistoryResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<RefundHistoryResponse>(_Response, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<RefundHistoryResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<RefundHistoryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
