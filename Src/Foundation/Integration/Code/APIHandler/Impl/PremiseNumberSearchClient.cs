using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.PremiseNumberSearch;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.PremiseNumberSearch;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;

using RestSharp;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IPremiseNumberSearchClient), Lifetime = Lifetime.Transient)]
    public class PremiseNumberSearchClient : BaseApiDewaGateway, Clients.IPremiseNumberSearchClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.SmartCustomerV3_ApiUrl}";

        public ServiceResponse<PremiseNumberSearchResponse> PremiseNumberSearch(PremiseNumberSearchRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {

                request.lang = language.Code();
                request.vendor = GetVendorId(segment);

                IRestResponse response = DewaApiExecute(BaseApiUrl, "displaypremisenumber", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PremiseNumberSearchResponse _Response = CustomJsonConvertor.DeserializeObject<PremiseNumberSearchResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responseCode) && _Response.responseCode.Equals("000"))
                    {
                        return new ServiceResponse<PremiseNumberSearchResponse>(_Response);
                    }
                    else
                    {
                        //LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                        return new ServiceResponse<PremiseNumberSearchResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<PremiseNumberSearchResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<PremiseNumberSearchResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

    }
}
