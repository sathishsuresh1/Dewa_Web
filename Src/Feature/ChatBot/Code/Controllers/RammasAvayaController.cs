using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web;
using System.Web.Configuration;

using Sitecore.Globalization;
using DEWAXP.Foundation.Logger;
using DEWAXP.Feature.Rammas.Extensions;
using DEWAXP.Feature.Rammas.Pipelines.Providers;
using DEWAXP.Feature.Rammas.Models.RammasLogin;
using DEWAXP.Feature.Rammas.Models.Account;
using RestSharp;
using DEWAXP.Feature.Rammas.Models.DirectLine;
using DEWAXP.Foundation.Content.Controllers;

namespace DEWAXP.Feature.Rammas.Controllers
{
    public class RammasAvayaController : BaseController
    {

        private const string VT_AUTHENTICATION = "authentication";
        private const string RAMMAS_Hi_KEY = "Rammas Hi";
        private const string RAMMAS_LoggedIn_KEY = "Rammas you are logged in";


        //[ValidateAntiForgeryToken]
        public ActionResult InitializeChat()
        {
            RammasAvayaClientModel model = new RammasAvayaClientModel() { UserProfile = CurrentPrincipal };

            try
            {
                var req = Newtonsoft.Json.JsonConvert.SerializeObject(getReqJson());
                               
                RestClient client = new RestClient(RammasAvayaClientModel.chatInitUrl);

                RestRequest request = new RestRequest(Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };
                const string jsontype = "application/json";
                request.Parameters.Clear();
                request.AlwaysMultipartFormData = false;

                request.AddHeader("Accept", jsontype);
                request.AddHeader("Content-Type", jsontype);
                //request.AddHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(RESTClientUserName + ":" + RESTClientPassword)));
                request.AddQueryParameter("journeyElement", "ChatService");
                request.AddParameter(jsontype, Newtonsoft.Json.Linq.JValue.Parse(req), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    InitResponseRoot robj = Newtonsoft.Json.JsonConvert.DeserializeObject<InitResponseRoot>(response.Content);
                    if (robj != null && !string.IsNullOrEmpty(robj.data?.contextId))
                    {
                        model.RequestId = robj.data.contextId;
                    }
                }
                else
                {
                    model.IsServerError = true;
                    LogService.Error(new Exception("Rammas Avaya Chat Initializatin Failed") { Source = response.ToString() }, this);
                }

                /*using (HttpClient client1 = GetHttpClient())
                {
                    HttpResponseMessage response = client1.PostAsync(new Uri(RammasAvayaClientModel.chatInitUrl), req, new System.Net.Http.Formatting.JsonMediaTypeFormatter()).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        InitResponseRoot robj = Newtonsoft.Json.JsonConvert.DeserializeObject<InitResponseRoot>(response.Content.ReadAsStringAsync().Result);
                        if (robj != null && !string.IsNullOrEmpty(robj.data?.contextId))
                        {
                            model.RequestId = robj.data.contextId;
                        }
                    }
                    else
                    {
                        model.IsServerError = true;
                        LogService.Error(new Exception("Rammas Avaya Chat Initializatin Failed") { Source = response.ToString() }, this);
                    }
                }*/
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this); model.IsServerError = true;
            }

            return View("~/Views/RammasAvaya/RammasAvaya.cshtml", model);
        }

        #region private objects
        private void SyncLoggedInUser(ref BotService bs)
        {
            if (IsLoggedIn)
            {
                if (CurrentPrincipal.Role == Roles.User || CurrentPrincipal.Role == Roles.Jobseeker)
                {
                    string cid = "";
                    bool rl = false;
                    if (CacheProvider.TryGet(CacheKeys.RAMMAS_DIRECTLINE_LOGIN, out rl)
                        && CacheProvider.TryGet(CacheKeys.RAMMAS_CONVERSATION_ID, out cid)
                        && cid.Equals(bs.Conversation.Id))
                    {
                        return;
                    }
                    else
                    {
                        string error;

                        //if(CurrentPrincipal.Role.Equals(Roles.Jobseeker)
                        LoginToRammas(CurrentPrincipal.Role.Equals(Roles.User) ? VT_AUTHENTICATION : "JS", new LoginModel() { Username = CurrentPrincipal.Username }, CurrentPrincipal.SessionToken, ref bs, out error);
                    }
                }
                else
                {
                    ClearSessionAndSignOut();
                }
            }
        }

        private bool LoginToRammas(string loginType, LoginModel model, string sessionId, ref BotService bs, out string error)
        {
            int magicNumber = RammasParameter.GenerateRandomNumber();

            string qsCID = Request.QueryString["conversationId"]?.ToString();
            //ServiceResponse<LoginResponse> customer_response;
            //ServiceResponse<userLoginValidation> jseeker_response;

            bool loginstatus = false; string msg = string.Empty; error = "";
            string rammasSessionId;

            if (loginType.Equals(VT_AUTHENTICATION, StringComparison.InvariantCultureIgnoreCase))
            {
                loginstatus = TryLoginWithSmartCustAuthentication(model.Username, sessionId, out rammasSessionId);
            }
            else
            {
                rammasSessionId = CurrentPrincipal.SessionToken;
                loginstatus = string.IsNullOrEmpty(rammasSessionId);
                //loginstatus = TryLoginWithJobSeeker(model, out jseeker_response, out rammasSessionId); msg = jseeker_response.Message;
                //CacheProvider.Store(jseeker, new CacheItem<string>(rammasSessionId));
            }

            if (loginstatus == true && !string.IsNullOrEmpty(rammasSessionId))
            {
                DirectLineValidationModel vmodel = new DirectLineValidationModel()
                {
                    ConversationId = bs.Conversation.Id,
                    DisplayName = model.Username,
                    LoginType = loginType.Equals(VT_AUTHENTICATION) ? "CS" : "JS",
                    SessionId = rammasSessionId,
                    UserName = model.Username,
                    MagicNumber = magicNumber,
                    LanguageCode = RequestLanguage == Webservices.Enums.SupportedLanguage.English ? "en-US" : "ar-AE"
                };
                bool validateDirectline = false;

                if (IsWebServiceLogin(bs) || (!string.IsNullOrEmpty(qsCID) && bs.Conversation.Id.Equals(qsCID)))
                {
                    validateDirectline = BotService.ValidateDirectLineAfterChatInit(vmodel, bs.Conversation);
                }
                else
                {
                    validateDirectline = BotService.ValidateDirectLine(vmodel, bs.Conversation);
                }

                if (validateDirectline)
                {
                    CacheProvider.Store(CacheKeys.RAMMAS_DIRECTLINE_LOGIN, new CacheItem<bool>(true));
                    CacheProvider.Store(CacheKeys.RAMMAS_CONVERSATION_ID, new CacheItem<string>(bs.Conversation.Id));

                    //bs.Conversation.Activity.From.Id = CurrentPrincipal.UserId;
                    //bs.Conversation.Activity.From.Name = CurrentPrincipal.UserId;

                    bs.UpdateConversationState();

                    return true;
                }
            }
            else
            {
                LogService.Error(new System.Exception("rammas Smart Authentication Failed"), this);
            }
            return false;
        }

        private bool TryLoginWithSmartCustAuthentication(string userId, string sessionId, out string rammasSessionId)
        {
            //Log.Info("auth session", this);

            rammasSessionId = null;
            var response = SmartCustAuthenticationServiceClient.GetLoginSessionCustomerAuthentication(userId, sessionId, RequestLanguage, Request.Segment());
            //Log.Info("auth response", response);

            if (response.Succeeded)
            {
                rammasSessionId = response.Payload.SessionNumber;
                return true;
            }
            //error = response.Message;
            return false;
        }

        private bool IsWebServiceLogin(BotService bs)
        {
            var lr = bs.Conversation.History?.LastOrDefault()?.Responses?.Where(r => r.Attachments?.Count > 0)?.ToList()?.LastOrDefault();
            if (lr != null && lr.Attachments?.Count > 0 &&
                lr.Attachments.LastOrDefault().Content?.buttons != null &&
                lr.Attachments.LastOrDefault().Content.buttons.Count > 0 &&
                !string.IsNullOrEmpty(lr.Attachments.LastOrDefault().Content.buttons.LastOrDefault().value))
            {
                return lr.Attachments.LastOrDefault().Content.buttons.LastOrDefault().value.Contains("azurewebsites.net/api/Authentication");
            }
            return false;
        }

        private void ClearSessionAndSignOut()
        {
            DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
        }

        public static HttpClient GetHttpClient()
        {

            ICredentials credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"], WebConfigurationManager.AppSettings["PROXYDOMAIN"]);
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"], true, null, credentials),
                UseProxy = true
            };

            HttpClient httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.Accept.Clear();

            return httpClient;
        }
        #endregion

        private InitReqRoot getReqJson()
        {
            long cid = DateTime.Now.Ticks;
            InitReqRoot ro = new InitReqRoot()
            {
                groupId = cid.ToString(),
                persistToEDM = true,
                data = new Data1() { AccountNumber = "", CustomerName = "", EmailAddress = "" },
                schema = new Schema()
                {
                    CustomerId = cid.ToString(),
                    Locale = Request.UserLanguages.FirstOrDefault(),
                    ServiceMap = new ServiceMap()
                    {
                        SM1 = new SM()
                        {
                            attributes = new Attributes()
                            {
                                Channel = new List<string>() { "Chat" },
                                Language = new List<string>() { "English" },
                                ServiceType = new List<string>() { "DewaChat" }
                            },
                            priority = 5
                        }
                    },
                    Strategy = "Most Idle"
                }
            };

            return ro;
        }
    }

}
