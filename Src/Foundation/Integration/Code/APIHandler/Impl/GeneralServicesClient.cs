using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.General;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.General;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    using @GeneralResponse = Models.Response.General;

    [Service(typeof(IGeneralServicesClient), Lifetime = Lifetime.Transient)]
    public class GeneralServicesClient : BaseApiDewaGateway, IGeneralServicesClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        public ServiceResponse<MaiDubaiResponse> DecryptURL(MaiDubaiRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.subscriptioninput.lang = language.Code();

                IRestResponse response = DewaApiExecute(BaseApiUrl, "setsubsciptionpreferences", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    @GeneralResponse.MaiDubaiResponse _Response = CustomJsonConvertor.DeserializeObject<@GeneralResponse.MaiDubaiResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<@GeneralResponse.MaiDubaiResponse>(_Response);
                    }
                    else
                    {
                        LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<@GeneralResponse.MaiDubaiResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<@GeneralResponse.MaiDubaiResponse>(null, false, $"response value:  '{JsonConvert.SerializeObject(response)}'");
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<@GeneralResponse.MaiDubaiResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);

        }


    }
}
