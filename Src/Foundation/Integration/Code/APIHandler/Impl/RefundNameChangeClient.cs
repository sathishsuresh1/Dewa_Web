using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundNameChange;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.RefundNameChange;
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
    [Service(typeof(IRefundNameChangeClient), Lifetime = Lifetime.Transient)]
    public class RefundNameChangeClient : BaseApiDewaGateway, IRefundNameChangeClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";
        public ServiceResponse<RefundNameChangeResponse> RefundNameChange(RefundNameChangeRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.vendorid = GetVendorId(segment);
                request.appversion = AppVersion;
                request.appidentifier = segment.Identifier();
                request.lang = language.Code();


                var ApiRequest = new { customerdetailsinput = request };
                IRestResponse response = DewaApiExecute(BaseApiUrl, "refundnamechange", ApiRequest, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    RefundNameChangeResponse _Response = CustomJsonConvertor.DeserializeObject<RefundNameChangeResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<RefundNameChangeResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<RefundNameChangeResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<RefundNameChangeResponse>(null, false, $"response value: '{response}'");
                }

                //return  ManageawaymodeAsync(request).Result;
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<RefundNameChangeResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
