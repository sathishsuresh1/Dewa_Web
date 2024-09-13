using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Premise;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Requests.JoinOwnership;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.JoinOwnership;
using DEWAXP.Foundation.Logger;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IPremiseClient), Lifetime = Lifetime.Transient)]
    public class PremiseHandler : BaseApiDewaGateway, Clients.IPremiseClient
    {
        /// <summary>
        /// 
        /// </summary>
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        public ServiceResponse<PremiseDetailsResponse> GetDetails(PremiseDetailsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.PremiseDetailsIN.lang = language.Code();
                request.PremiseDetailsIN.vendorid = GetVendorId(segment);
                request.PremiseDetailsIN.appidentifier = segment.Identifier();
                request.PremiseDetailsIN.appversion = AppVersion;
                request.PremiseDetailsIN.mobileosver = AppVersion;

                IRestResponse response = DewaApiExecute(BaseApiUrl, "premisedetailsv3", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PremiseDetailsResponse _Response = CustomJsonConvertor.DeserializeObject<PremiseDetailsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<PremiseDetailsResponse>(_Response);
                    }
                    else
                    {
                       //LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<PremiseDetailsResponse>(null, false, _Response.responseMessage);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<PremiseDetailsResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<PremiseDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<JointOwnerResponse> PostJointOwnershipRequest(JointOwnerRequest request, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.jointownerinputs.lang = language.Code();

                var response = DewaApiExecute(BaseApiUrl, "submitjointowner", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JointOwnerResponse _Response = CustomJsonConvertor.DeserializeObject<JointOwnerResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.ResponseCode) && _Response.ResponseCode.Equals("000"))
                    {
                        return new ServiceResponse<JointOwnerResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"PostJointOwnershipRequest value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<JointOwnerResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"PostJointOwnershipRequest value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<JointOwnerResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<JointOwnerResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<CommonResponse> SaveJointOwnershipAttachment(JointOwnerAttachmentRequest request)
        {
            try
            {
                var response = DewaApiExecute(BaseApiUrl, "saveattachment", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    CommonResponse _Response = CustomJsonConvertor.DeserializeObject<CommonResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<CommonResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<CommonResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<CommonResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<CommonResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
