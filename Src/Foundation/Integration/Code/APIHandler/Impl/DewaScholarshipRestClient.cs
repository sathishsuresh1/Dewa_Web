using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.DewaScholarship;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.DewaScholarship;
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
    [Service(typeof(IDewaScholarshipRestClient), Lifetime = Lifetime.Transient)]
    public class DewaScholarshipRestClient : BaseApiDewaGateway, IDewaScholarshipRestClient
    {
        public ServiceResponse<EmailVerificationResponse> EmailVerification(EmailVerificationRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                request.vendorid = GetVendorId(segment);
                request.lang =  language.Code();
                IRestResponse response = DewaApiExecute($"{Config.ApiBaseConfig.DEWAScholarship_ApiUrl}", "emailverification", request, Method.POST, null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EmailVerificationResponse _Response = CustomJsonConvertor.DeserializeObject<EmailVerificationResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.errorcode) && _Response.errorcode.Equals("0"))
                    {
                        return new ServiceResponse<EmailVerificationResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<EmailVerificationResponse>(null, false, _Response.errormessage);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<EmailVerificationResponse>(null, false, $"response value: '{response}'");
                }

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<EmailVerificationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
