using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static System.Net.WebRequestMethods;

namespace DEWAXP.Feature.Dashboard.Controllers.Api
{
    [TwoPhaseAuthorize]
    public class BesController : BaseApiController
    {
        private string URL = ConfigurationManager.AppSettings[ConfigKeys.BES_BASE_URL];// "http://10.12.244.57:8888/acp_dewa/html/";

        [AcceptVerbs(Http.Get, Http.Post)]
        [TwoPhaseAuthorize]
        public async Task<HttpResponseMessage> Proxy()
        {
            //var consumptionResponse = DewaApiClient.GetContractAccountUserIDCheck(request.BehaviourIndicator, CurrentPrincipal.SessionToken, request.AccountNumber, request.PremiseNumber, RequestLanguage, Request.Segment());

            try
            {
                string account_id = "";
                if (this.Request.Method == HttpMethod.Post)
                {
                    var contentType = Request.Content.Headers.ContentType.MediaType;
                    if (contentType == "application/x-www-form-urlencoded")
                    {
                        Request.Content.LoadIntoBufferAsync().Wait();
                        NameValueCollection formData = Request.Content.ReadAsFormDataAsync().Result;
                        account_id = formData["account_id"];
                    }
                }
                else if (this.Request.Method == HttpMethod.Get)
                {
                    account_id = Request.RequestUri.ParseQueryString()["account_id"];
                }
                account_id = FormatContractAccount(account_id);
                if (string.IsNullOrEmpty(account_id))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User Id required");
                }
                else
                {
                    if(account_id.Equals("000000000000"))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User Id Not Valid");
                    }
                }
                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(account_id.TrimStart(new char[] { '0' }), Times.Once));
                var checkResponse = DewaApiClient.GetContractAccountUserIDCheck(CurrentPrincipal.UserId, account_id, RequestLanguage, Request.Segment());

                if (checkResponse.Succeeded)
                {
                    string certhashkey = HttpContext.Current.Application["beshashstring"] as string;
                    using (var handler = new WebRequestHandler())
                    {
                        ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, error) =>
                        {
                            if (cert.GetCertHashString() == certhashkey)
                            {
                                return true;
                            }
                            else
                            {
                                return error == SslPolicyErrors.None;
                            }
                        };

                        using (HttpClient http = new HttpClient(handler))
                        {
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                            this.Request.RequestUri = new Uri(URL + this.Request.RequestUri.Query);

                            if (this.Request.Method == HttpMethod.Get)
                            {
                                this.Request.Content = null;
                            }

                            return await http.SendAsync(this.Request);
                        }
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "User Not loggedin or Not mapped");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}