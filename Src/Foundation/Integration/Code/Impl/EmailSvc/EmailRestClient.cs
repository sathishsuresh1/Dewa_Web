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

namespace DEWAXP.Foundation.Integration.Impl.EmailSvc
{
    [Service(typeof(IEmailServiceClient), Lifetime = Lifetime.Transient)]
    public class EmailRestClient : BaseDewaGateway, IEmailServiceClient
    {
        public ServiceResponse<string> SendEmail(string from, string to, string subject, string body)
        {
            return Send(from, to, subject, body, new List<Tuple<string, byte[]>>());
        }

        /// <summary>
        /// Item1: filename
        /// Item2: bytes of the attachment
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public ServiceResponse<string> SendEmail(string from, string to, string cc, string bcc, string subject, string body, List<Tuple<string, byte[]>> attachments)
        {
            return Send(from, to, subject, body, attachments,cc,bcc);
        }

        private ServiceResponse<string> Send(string from, string to, string subject, string body, List<Tuple<string, byte[]>> attachments,string cc="",string bcc="")
        {
            try
            {
                RestRequest request = null;

                RestClient client = CreateRESTClient();

                request = new RestRequest("utility.api/sendmail/", Method.POST);

                request = CreateHeader(request);

                var model = new SendEmailModel() { From = from, To = to, Subject = subject, Message = body, CC = cc, BCC = bcc };

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var att in attachments)
                    { model.Attachments.Add(new Attachment() { FileName = att.Item1, FileBase64String = Convert.ToBase64String(att.Item2) }); }
                }               

                request.AddBody(model);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WebApiRestResponseBase _emailResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WebApiRestResponseBase>(response.Content);

                    return new ServiceResponse<string>(_emailResponse.ResponseData);
                }
                else
                {
                    return new ServiceResponse<string>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {

                LogService.Fatal(ex, this);
                return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
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
