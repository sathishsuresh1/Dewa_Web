using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using DEWAXP.Foundation.Integration.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IEstimateClient),Lifetime =Lifetime.Transient)]
    public class EstimateClient : BaseApiDewaGateway, IEstimateClient
    {
      
        public ServiceResponse<NewConnectionRefundResponse> SmartCommunicationSubmit(NewConnectionRefundRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;              
                var apiRequest = new  { NewConnectionRefund = request};
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "newconnectionrefund", apiRequest, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    NewConnectionRefundResponse _Response = CustomJsonConvertor.DeserializeObject<NewConnectionRefundResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<NewConnectionRefundResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<NewConnectionRefundResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<NewConnectionRefundResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<NewConnectionRefundResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<EstimateCustomerListResponse> EstimateCustomerlist(EstimateCustomerListResquest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "estimatecustomerlist", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EstimateCustomerListResponse _Response = CustomJsonConvertor.DeserializeObject<EstimateCustomerListResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EstimateCustomerListResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<EstimateCustomerListResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EstimateCustomerListResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EstimateCustomerListResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<EstimateDetailsResponse> EstimateDetails(EstimateDetailsRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "estimatedetails", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EstimateDetailsResponse _Response = CustomJsonConvertor.DeserializeObject<EstimateDetailsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EstimateDetailsResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<EstimateDetailsResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EstimateDetailsResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EstimateDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<EstimatePaymentHistoryResponse> EstimateHistory(EstimatePaymentHistoryRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "estimatehistory", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EstimatePaymentHistoryResponse _Response = CustomJsonConvertor.DeserializeObject<EstimatePaymentHistoryResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EstimatePaymentHistoryResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<EstimatePaymentHistoryResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EstimatePaymentHistoryResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EstimatePaymentHistoryResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<EstimatePdfResponse> EstimatePDF(EstimatePdfRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "estimatepdf", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EstimatePdfResponse _Response = CustomJsonConvertor.DeserializeObject<EstimatePdfResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EstimatePdfResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<EstimatePdfResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EstimatePdfResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EstimatePdfResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<EstimateAmountDisplayResponse> EstimateAmountDisplay(EstimateAmountDisplayRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.EstimateDetailsRetrieve.lang = language.Code();
                request.EstimateDetailsRetrieve.vendorid = GetVendorId(segment);
                request.EstimateDetailsRetrieve.appversion = AppVersion;
                request.EstimateDetailsRetrieve.appidentifier = segment.Identifier();
                request.EstimateDetailsRetrieve.mobileosversion = AppVersion;
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "estimateamountdisplay", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EstimateAmountDisplayResponse _Response = CustomJsonConvertor.DeserializeObject<EstimateAmountDisplayResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<EstimateAmountDisplayResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<EstimateAmountDisplayResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EstimateAmountDisplayResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EstimateAmountDisplayResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<NewConnectionTaxInvoicePdfResponse> NewConnectionTaxInvoicePdf(NewConnectionTaxInvoicePdfRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "newconnectiontaxinvoicepdf", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    NewConnectionTaxInvoicePdfResponse _Response = CustomJsonConvertor.DeserializeObject<NewConnectionTaxInvoicePdfResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<NewConnectionTaxInvoicePdfResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<NewConnectionTaxInvoicePdfResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<NewConnectionTaxInvoicePdfResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<NewConnectionTaxInvoicePdfResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<PaymentReceiptDetailsResponse> OnlinePaymentReceipt(PaymentReceiptDetailsRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.lang = language.Code();
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.mobileosversion = AppVersion;
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "onlinepaymentreceipt", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PaymentReceiptDetailsResponse _Response = CustomJsonConvertor.DeserializeObject<PaymentReceiptDetailsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<PaymentReceiptDetailsResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<PaymentReceiptDetailsResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<PaymentReceiptDetailsResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<PaymentReceiptDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
