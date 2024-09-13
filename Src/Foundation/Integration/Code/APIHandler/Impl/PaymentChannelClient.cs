using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Payment;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Payment;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json;
using RestSharp;
using Sitecore.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System;
using static Sitecore.Configuration.State;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using Sitecore.Web.UI.HtmlControls;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IPaymentChannelClient), Lifetime = Lifetime.Transient)]
    public class PaymentChannelClient : BaseApiDewaGateway,IPaymentChannelClient
    {
        private string BaseApiUrl => $"{ApiBaseConfig.NBDPay_ApiUrl}";
        public ServiceResponse<SamsungPayResponse> PaymentChannel(SamsungPayRequest Request, string channel, SupportedLanguage requestLanguage, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                Request.walletin.lang = requestLanguage.Code();
                Request.walletin.appidentifier = segment.Identifier();
                Request.walletin.vendorid = GetVendorId(segment);
                Request.walletin.appversion = AppVersion;
                Request.walletin.device = DeviceType;
                //LogService.Info(JsonConvert.SerializeObject(Request));
                IRestResponse response = DewaApiExecute(BaseApiUrl, Paymentchannelmethodname(channel), Request, Method.POST, null);
               
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //LogService.Info(JsonConvert.SerializeObject(response.Content));
                    SamsungPayResponse _Response = CustomJsonConvertor.DeserializeObject<SamsungPayResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<SamsungPayResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<SamsungPayResponse>(_Response, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<SamsungPayResponse>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<SamsungPayResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public async Task<string> GetMerchantSessionAsync(string request1)
        {

            try
            {
                var handler = new HttpClientHandler();
                //handler.Proxy = new WebProxy("http://myinternet.dewa.gov.ae/");
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;

                // PEM certificates support requires .NET 5 and higher
                // Export to PFX is needed because of this bug https://github.com/dotnet/runtime/issues/23749#issuecomment-747407051                 //@"E:/cert/merchant_id_sandbox.pem", @"E:/cert/merchant_id_sandbox.key"
                handler.ClientCertificates.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(@"E:/www/stagingnew.dewa.gov.ae/merchant_id_sandbox.pem").Export(X509ContentType.Pfx)));

                // In production code, don't destroy the HttpClient through using, but better use IHttpClientFactory factory or at least reuse an existing HttpClient instance
                // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests
                // https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
                using (var httpClient = new HttpClient(handler))
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://apple-pay-gateway-cert.apple.com/paymentservices/paymentSession"))
                    {
                        request.Content = new StringContent(request1);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");



                        return await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();
                    }
                }
                
                
            }
            catch (Exception ex)
            {
                LogService.Info("response Result Error" + JsonConvert.SerializeObject(ex));
                LogService.Info(ex);
                LogService.Info(new Exception(ex.Message + ex.InnerException.Message));
            }

            return await Task.FromResult(string.Empty);
        }

        public string Paymentchannelmethodname(string channel)
        {
            switch(channel)
            {
                case "samsung":
                    return "samsungpay";
                case "apple":
                    return "applepay";
                default:
                    return string.Empty;
            }
        }
    }
}
