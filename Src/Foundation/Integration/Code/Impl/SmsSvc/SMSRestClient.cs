using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.WebApi;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Impl.SmartVendor;

namespace DEWAXP.Foundation.Integration.Impl.SmsSvc
{
    [Service(typeof(ISmsServiceClient), Lifetime = Lifetime.Transient)]
    public class SMSRestClient : BaseDewaGateway, ISmsServiceClient
    {
        public ServiceResponse<bool> Send_DEWA_SMS(string Moblie_Number, string Text_Message, string Application_Name, string Sender_Name, string SMS_Priority)
        {
            return Send(Moblie_Number, Text_Message, Application_Name, Sender_Name, SMS_Priority);
        }

        public ServiceResponse<bool> Send_DEWA_SMSAr(string Moblie_Number, string Text_Message, string Application_Name, string Sender_Name, string SMS_Priority)
        {
            return Send(Moblie_Number, Text_Message, Application_Name, Sender_Name, SMS_Priority, "ar");
        }

        private ServiceResponse<bool> Send(string mobile, string message, string application, string senderName, string priority, string language = "en")
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateRESTClient();

                request = new RestRequest("utility.api/senddewasms", Method.POST);

                request = CreateHeader(request);

                var model = new SendSMSModel() { MoblieNumber = mobile, TextMessage = message, ApplicationName = application, SenderName = senderName, SMSPriority = priority, Lang = language };

                request.AddBody(model);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WebApiRestResponseBase _smsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WebApiRestResponseBase>(response.Content);

                    return new ServiceResponse<bool>(_smsResponse.IsSuccess);
                }
                else
                {
                    return new ServiceResponse<bool>(false, false, $"response value: '{response.Content}'");
                }
            }
            catch (System.Exception ex)
            {

                LogService.Fatal(ex, this);
                return new ServiceResponse<bool>(false, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        private RestClient CreateRESTClient()
        {
            return new RestClient(DewaWebApi.RESTClientURL);
        }

        private RestRequest CreateHeader(RestRequest request)
        {
            request.AddHeader("password", DewaWebApi.RESTClientPassword);
            request.AddHeader("username", DewaWebApi.RESTClientUserName);
            request.RequestFormat = DataFormat.Json;

            return request;
        }
    }
}
