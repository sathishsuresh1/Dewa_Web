using DEWAXP.Feature.ChatBot.Models.DirectLine;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.AccountModel;
using DEWAXP.Foundation.Content.Models.RammasLogin;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.JobSeekerSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json.Linq;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DEWAXP.Feature.ChatBot.Controllers
{
    public class RammasDirectLineController : BaseController
    {
        //private const string jseeker = "JobSeekerLoggedIn";
        private const string VT_AUTHENTICATION = "authentication";

        private const string RAMMAS_Hi_KEY = "Rammas Hi";
        private const string RAMMAS_LoggedIn_KEY = "Rammas you are logged in";
        private bool addUserInputToHistory = true;

        #region Login

        private bool LoginToRammas(string loginType, LoginModel model, string sessionId, ref BotService bs, out string error)
        {
            int magicNumber = RammasParameter.GenerateRandomNumber();

            string qsCID = Request.Params.Get("conversationId") ?? string.Empty;
            //ServiceResponse<LoginResponse> customer_response;
            //ServiceResponse<DEWAXP.Foundation.Integration.JobSeekerSvc.userLoginValidation> jseeker_response;

            bool loginstatus = false; string msg = string.Empty; error = ""; //bool isJobseekerLogin = false;
            string rammasSessionId;

            if (loginType.Equals(VT_AUTHENTICATION, StringComparison.InvariantCultureIgnoreCase))
            {
                loginstatus = TryLoginWithSmartCustAuthentication(model.Username, sessionId, out rammasSessionId);
            }
            else
            {
                //rammasSessionId = CurrentPrincipal.SessionToken;
                //loginstatus = string.IsNullOrEmpty(rammasSessionId);
                //loginstatus = TryLoginWithJobSeeker(model, out jseeker_response, out rammasSessionId); msg = jseeker_response.Message;
                //CacheProvider.Store(jseeker, new CacheItem<string>(rammasSessionId));
                rammasSessionId = string.Empty;
                //loginstatus = Session["R01"] == null ? false : true;
                if (!string.IsNullOrEmpty(sessionId))
                {
                    loginstatus = true;
                    rammasSessionId = sessionId; //isJobseekerLogin = true;
                }
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
                    LanguageCode = RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.English ? "en-US" : "ar-AE"
                };
                bool validateDirectline = false;

                if (IsWebServiceLogin(bs) || (!string.IsNullOrEmpty(qsCID) && bs.Conversation.Id.Equals(qsCID)) || Session["R01"] != null)
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

        private bool TryLoginWithJobSeeker(LoginModel model, out ServiceResponse<userLoginValidation> response, out string sessionId)
        {
            //Log.Info("TryLoginWithJobSeeker", "");

            sessionId = null;

            response = JobSeekerClient.GetValidateCandidateLogin(model.Username, Base64Encode(model.Password),true, RequestLanguage, Request.Segment());
            // Log.Info("TryLoginWithJobSeeker response", response);

            if (response.Succeeded)
            {
                sessionId = response.Payload.credential;
                return true;
            }
            //error = response.Message;
            return false;
        }

        #endregion Login

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult InitializeChat()
        {
            ChatModel model = new ChatModel() { IsLoggedInUser = IsLoggedIn };

            StringBuilder sb = new StringBuilder("\r\nRammas Page Init started at: " + DateTime.Now.ToLongTimeString());

            BotService s = new BotService(RequestLanguage.ToString());
            if (s == null || s.Conversation == null || s.ConversationStarted == false) { goto JumpHere; }
            SyncLoggedInUser(ref s);
            string conid;
            bool autoInitialize = true;
            if (!s.Conversation.UserAuthenticated && CacheProvider.TryGet(CacheKeys.RAMMAS_CONVERSATION_ID, out conid) && conid.Equals(s.Conversation.Id))
            {
                s.Conversation.UserAuthenticated = true;

                if (s.Conversation.History?.Count > 0)
                {
                    autoInitialize = false;

                    JsonResponse jr = new JsonResponse() { Order = 0, RenderType = 1, Text = Translate.Text(RAMMAS_LoggedIn_KEY) };
                    List<JsonResponse> jrl = new List<JsonResponse>();
                    jrl.Add(jr);

                    s.Conversation.UpdateHistory(jrl, "");
                    var res = s.ReadConversationResponse(BotService.GetHttpClient(false, true, s.Conversation.Token), true);
                    if (res.Count > 0)
                    {
                        //int ord = 1;
                        s.Conversation.UpdateHistory(res, "");
                    }

                    model.InitialJson = Newtonsoft.Json.JsonConvert.SerializeObject(s.Conversation.History);
                }
            }
            if (autoInitialize)
            {
                //model.InitialJson = Newtonsoft.Json.JsonConvert.SerializeObject(TempGetData());

                if (s.Conversation.History.Count > 0)
                {
                    var res = s.ReadConversationResponse(BotService.GetHttpClient(false, true, s.Conversation.Token), true);
                    if (res.Count > 0)
                    {
                        s.Conversation.UpdateHistory(res, "");
                    }
                    //s.Conversation.History.ForEach(x => x.Responses.ForEach(y => y.Attachments.RemoveAll(z => z.ContentType == CardType.Audio)));
                    model.InitialJson = Newtonsoft.Json.JsonConvert.SerializeObject(s.Conversation.History);
                }
                else
                {
                    var res = PostToBot(Translate.Text(RAMMAS_Hi_KEY), ref s, true);
                    List<Journey> jr = new List<Journey>();
                    jr.Add(new Journey() { Order = 0, Responses = res, UserAction = "", ActivityDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    ////DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz")
                    model.InitialJson = Newtonsoft.Json.JsonConvert.SerializeObject(jr);
                    foreach (var j in jr)
                    {
                        foreach (var r in j.Responses)
                        {
                            r.Attachments.RemoveAll(x => x.ContentType == CardType.Audio);
                        }
                    }

                    s.Conversation.History = jr;
                }
            }

            s.UpdateConversationState();
        JumpHere:
            sb.AppendLine(Environment.NewLine + "Rammas Page Init Ended at " + DateTime.Now.ToLongTimeString()); LogService.Info(new System.Exception(sb.ToString()));

            return View("~/Views/Feature/Rammas/RammasDirectLine/RammasLandingPage.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostMessage(string msg)
        {
            BotService s = new BotService(RequestLanguage.ToString());

            if (s.IsNewSession)
            {
                ClearSessionAndSignOut();
                Response.StatusCode = (int)System.Net.HttpStatusCode.Gone;
                return Json("Session End", JsonRequestBehavior.AllowGet);
            }
            var res = PostToBot(msg, ref s);
            if (s.Conversation.IsError)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.ExpectationFailed;
                ClearSessionAndSignOut();
                return Json(s.Conversation.ErrorMessage, JsonRequestBehavior.AllowGet);
            }

            return Json(res, JsonRequestBehavior.AllowGet);  //Ok(s.PostConversation(msg));
        }

        private List<JsonResponse> PostToBot(string msg, ref BotService s, bool isPageLoad = false)
        {
            //BotService s = new BotService(GetUserIdOrName, RequestLanguage.ToString(), isPageLoad);

            List<JsonResponse> res = new List<JsonResponse>();

            if (!s.ConversationStarted) { return res; }

            ProcessUserResponse(ref res, isPageLoad, ref s, msg);

            s.Conversation.PreviousUserAction = s.Conversation.CurrentUserAction;

            s.Conversation.Activity.Type = "message";
            s.Conversation.Activity.Text = "";
            s.Conversation.Activity.Name = "";
            s.Conversation.Activity.Value = null;

            s.Conversation.UserAuthenticated = IsLoggedIn;

            s.Conversation.UpdateHistory(res, this.addUserInputToHistory ? msg : string.Empty);
            if (IsStarMenu(res)) { s.Conversation.CurrentUserAction = s.Conversation.PreviousUserAction = 0; }
            s.UpdateConversationState();

            return res;
        }

        private void ProcessUserResponse(ref List<JsonResponse> res, bool isPageLoad, ref BotService s, string msg)
        {
            if (isPageLoad)
            {
                s.Conversation.Activity.Text = msg;
                res = s.PostConversation();
                return;
            }

            byte b = 0;
            if (msg.IsRootLevel(out b))
            {
                s.Conversation.CurrentUserAction = b;
                if (b == 16)
                {
                    s.Conversation.Activity.Type = "event";
                    s.Conversation.Activity.Name = msg;
                    res = s.PostConversation();
                    return;
                }
                if (s.Conversation.CurrentUserAction == s.Conversation.PreviousUserAction) { s.Conversation.PreviousUserAction = b.GetPreviousLevel(); goto subOption; }
                switch (s.Conversation.PreviousUserAction)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 14:
                        s.Conversation.PreviousUserAction = 0;
                        s.Conversation.Activity.Text = msg;
                        res = s.PostConversation();
                        return;

                    case 13:
                    case 131:
                    case 15:
                    case 151:
                    case 16:
                    case 161:
                        s.Conversation.CurrentUserAction = 200;
                        s.Conversation.TempUserAction = b;
                        s.Conversation.MasterUserAction = s.Conversation.PreviousUserAction;
                        s.Conversation.Activity.Text = msg;
                        res = s.PostConversation();
                        return;

                    default:
                        s.Conversation.Activity.Text = msg;
                        res = s.PostConversation();
                        return;
                }
            }

            if (s.Conversation.PreviousUserAction == 132 || s.Conversation.PreviousUserAction == 131)
            {
                if (msg.StartsWith("SR_"))
                {
                    s.Conversation.PreviousUserAction = 16;
                }
                //else if (s.Conversation.IsSurveyExpected) {  s.Conversation.PreviousUserAction = 0; s.Conversation.CurrentUserAction = 0; s.Conversation.IsSurveyExpected = false; }
            }

        subOption:
            switch (s.Conversation.PreviousUserAction)
            {
                case 13:
                    if (msg.IsValidEPayNo())
                    {
                        s.Conversation.CurrentUserAction = 131;
                        s.Conversation.Activity.Text = msg;
                        s.Conversation.LastUserAction = msg;
                    }
                    else
                    {
                        LogService.Debug("Rammas: Invalid Easy Pay No: " + msg);
                        ProcessUnknownInput(ref res, ref s, msg);
                    }

                    break;

                case 131:
                    if (IsLoggedIn && msg.StartsWith("EP_"))
                    {
                        string[] vls = msg.ToValArray();
                        if (vls != null && vls.Length == 2)
                        {
                            //s.Conversation.Activity.Text = string.Empty;
                            s.Conversation.Activity.Value = JValue.Parse(CreateJson("Amount", vls[1]));
                            s.Conversation.CurrentUserAction = 132; this.addUserInputToHistory = false;
                            s.Conversation.IsSurveyExpected = true;
                            break;
                        }
                    }
                    if (msg.IsValidEPayNo())
                    {
                        s.Conversation.CurrentUserAction = 131;
                        s.Conversation.Activity.Text = msg;
                        s.Conversation.LastUserAction = msg;
                    }
                    else
                    {
                        ProcessUnknownInput(ref res, ref s, msg);
                    }

                    break;

                case 132://after the easy pay is done, two cases, survey or main menu
                    if (msg.StartsWith("EP_"))
                    {
                        string[] vls = msg.ToValArray();
                        if (vls != null && vls.Length == 2)
                        {
                            s.Conversation.Activity.Value = JValue.Parse(CreateJson("Amount", vls[1]));
                            s.Conversation.CurrentUserAction = 132; this.addUserInputToHistory = false;
                            s.Conversation.IsSurveyExpected = true;
                            break;
                        }
                    }
                    if (msg.StartsWith("SR_") && s.Conversation.IsSurveyExpected)
                    {
                        string[] srv = msg.ToValArray();
                        if (srv != null && srv.Length == 2)
                        {
                            this.addUserInputToHistory = false;
                            s.Conversation.CurrentUserAction = 161;
                            //s.Conversation.Activity.Text = string.Empty;
                            s.Conversation.Activity.Value = JValue.Parse(CreateJson("selected", srv[1]));
                            s.Conversation.IsSurveyExpected = false;
                        }
                        else
                        {
                            ProcessUnknownInput(ref res, ref s, msg);
                        }
                    }
                    s.Conversation.CurrentUserAction = 132;
                    s.Conversation.Activity.Text = msg;
                    break;

                case 15: //dewa store
                    s.Conversation.CurrentUserAction = 151;
                    s.Conversation.Activity.Text = msg;
                    break;

                case 151:
                    string cmd = !string.IsNullOrEmpty(msg) && msg.Length > 3 ? msg.Substring(0, 3) : msg;
                    s.Conversation.CurrentUserAction = 151;
                    this.addUserInputToHistory = false;
                    switch (cmd)
                    {
                        case "DT_":
                            string[] vls0 = msg.ToValArray();
                            if (vls0 != null && vls0.Length == 4)
                            {
                                s.Conversation.Activity.Value = CreateJson("action", vls0[2], "number", int.Parse(vls0[1]), "category", int.Parse(vls0[3]));
                                //JValue.Parse("{'action': '" + vls0[2] + "', 'number': " + vls0[1] + ",'category':" + vls0[3] + "}"); //.ToString
                                s.Conversation.CurrentUserAction = 152;
                                res = s.PostConversation();
                                s.Conversation.CurrentUserAction = 151;
                                return;
                            }
                            break;

                        case "NI_":
                            string[] vls1 = msg.ToValArray();
                            if (vls1 != null && vls1.Length == 4)
                            {
                                //s.Conversation.Activity.Text = string.Empty;
                                s.Conversation.Activity.Value = CreateJson("action", vls1[2], "number", int.Parse(vls1[1]), "category", int.Parse(vls1[3]));
                                //JValue.Parse("{'action': '" + vls1[2] + "', 'number': " + vls1[1] + ",'category':" + vls1[3] + "}");
                                //s.Conversation.CurrentUserAction = 152;
                            }
                            break;

                        case "AF_":
                            string[] afa = msg.ToValArray();
                            if (afa != null && afa.Length == 4)
                            {
                                s.Conversation.Activity.Value = CreateJson("action", afa[2], "number", int.Parse(afa[1]), "category", int.Parse(afa[3]));
                                // JValue.Parse("{'action': '" + afa[2] + "', 'number': " + afa[1] + ",'category':" + afa[3] + "}");
                                //s.Conversation.CurrentUserAction = 152;
                            }
                            break;
                        /*case "Wha": //whats' new
                        case "My ": //My Favorites
                        case "Tel": //Telecome
                        case "Tra": //Travel
                        case "Hom": //Home
                        case "Ban": //Bank
                        case "Oth": //Others
                        case "Sho": //Show More
                            this.addUserInputToHistory = true;
                            s.Conversation.Activity.Text = msg;
                            break;*/

                        default:
                            switch (msg.ToLower())
                            {
                                /*case "DubaiNow Account":
                                case "حساب دبي الآن":
                                    this.addUserInputToHistory = true;
                                    s.Conversation.Activity.Text = msg;
                                    break;*/
                                case "exit":
                                case "bye":
                                case "quit":
                                case "hello":
                                case "hi":
                                case "استقال":
                                case "خروج":
                                case "وداعا":
                                    ProcessUnknownInput(ref res, ref s, msg);
                                    break;

                                default:
                                    //arabic cases
                                    byte ua = 0;
                                    if (msg.IsRootLevel(out ua) == true) { ProcessUnknownInput(ref res, ref s, msg); return; }
                                    this.addUserInputToHistory = true;
                                    s.Conversation.Activity.Text = msg;
                                    break;
                            }
                            break;
                    }

                    break;

                case 18:
                case 16: //survey root
                    string[] sr2 = msg.ToValArray();
                    if (sr2 != null && sr2.Length == 2)
                    {
                        this.addUserInputToHistory = false;
                        s.Conversation.CurrentUserAction = 161;
                        //s.Conversation.Activity.Text = string.Empty;
                        s.Conversation.Activity.Value = JValue.Parse(CreateJson("selected", sr2[1]));
                    }
                    else
                    {
                        ProcessUnknownInput(ref res, ref s, msg);
                    }
                    break;

                case 161:
                    string[] vls2 = msg.Trim('_').ToValArray();
                    if (vls2 != null && vls2.Length > 0)
                    {
                        s.Conversation.Activity.Value = JValue.Parse(CreateJson(vls2));
                        s.Conversation.CurrentUserAction = 162;
                        this.addUserInputToHistory = false;
                    }
                    else
                    {
                        ProcessUnknownInput(ref res, ref s, msg);
                    }
                    break;

                case 200: // special case when context changes
                    //this.addUserInputToHistory = true;
                    if (msg.ToLower().Equals("yes") || msg.Equals("نعم"))
                    {
                        s.Conversation.PreviousUserAction = s.Conversation.TempUserAction < 19 ? (byte)0 : s.Conversation.PreviousUserAction;
                        s.Conversation.CurrentUserAction = s.Conversation.TempUserAction;
                    }
                    else if (msg.IsValidEPayNo())
                    {
                        s.Conversation.CurrentUserAction = 131;
                        s.Conversation.Activity.Text = msg;
                        s.Conversation.LastUserAction = msg;
                    }
                    else
                    {
                        s.Conversation.CurrentUserAction = s.Conversation.MasterUserAction;
                    }
                    s.Conversation.Activity.Text = msg;
                    break;

                default:
                    //ProcessUnknownInput(ref res, ref s, msg);
                    s.Conversation.Activity.Text = msg;
                    break;
            }

            //s.Conversation.Activity.Text = msg;
            res = s.PostConversation();
        }

        private void ProcessUnknownInput(ref List<JsonResponse> res, ref BotService s, string msg)
        {
            s.Conversation.CurrentUserAction = 200; //just to ignore special rendering
            s.Conversation.MasterUserAction = s.Conversation.PreviousUserAction;
            byte ua = 0;
            if (msg.IsRootLevel(out ua) == true)
            {
                s.Conversation.TempUserAction = ua;
                switch (ua)
                {
                    case 16:
                        s.Conversation.Activity.Type = "event";
                        s.Conversation.Activity.Name = msg;
                        break;

                    default:
                        s.Conversation.Activity.Text = msg;
                        break;
                }
            }
            else
            {
                s.Conversation.TempUserAction = 17;
                s.Conversation.Activity.Text = msg;
            }

            //res = s.PostConversation();
        }

        [HttpGet]
        public JsonResult CheckState()
        {
            bool status = false; string key;
            if (CacheProvider.TryGet(CacheKeys.RAMMAS_TRANSACTION_SUCCESS, out key))
            {
                try
                {
                    BotService s = new BotService(RequestLanguage.ToString());
                    //byte counter = 0;
                    //jumpHere:
                    var res = s.ReadConversationResponse(BotService.GetHttpClient(false, true, s.Conversation.Token), true, true);
                    /*if (res.Count < 1 && counter < 4)
                    {
                        counter++;
                        System.Threading.Thread.Sleep(1000);
                        goto jumpHere;
                    }*/

                    if (res.Count > 0)
                    {
                        s.Conversation.UpdateHistory(res, ""); status = true; CacheProvider.Remove(CacheKeys.RAMMAS_TRANSACTION_SUCCESS);
                        s.Conversation.UpdateHistory(res, "");
                        s.UpdateConversationState();
                    }

                    return Json(new { status = status ? 1 : 0, data = res }, JsonRequestBehavior.AllowGet);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    status = false;
                }
            }
            return Json(new { status = status ? 1 : 0 }, JsonRequestBehavior.AllowGet);
        }

        private void SyncLoggedInUser(ref BotService bs)
        {
            if (IsLoggedIn || Session["R01"] != null)
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
                        string authType = Session["R01"] == null ? VT_AUTHENTICATION : "JS";
                        LoginToRammas(authType, new LoginModel() { Username = CurrentPrincipal.Username }, CurrentPrincipal.SessionToken, ref bs, out error);
                    }
                }
                else
                {
                    ClearSessionAndSignOut();
                }
            }
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

        private string CreateJson(string p, string v)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);

            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();

                writer.WritePropertyName(p);
                writer.WriteValue(v);

                writer.WriteEndObject();
            }
            return sb.ToString();
        }

        private JToken CreateJson(string p1, string v1, string p2, int v2, string p3, int v3)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);

            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();

                writer.WritePropertyName(p1);
                writer.WriteValue(v1);

                writer.WritePropertyName(p2);
                writer.WriteValue(v2);

                writer.WritePropertyName(p3);
                writer.WriteValue(v3);

                writer.WriteEndObject();
            }
            return JValue.Parse(sb.ToString());
        }

        private string CreateJson(string[] vls2)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);

            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;

                writer.WriteStartObject();

                //string jsn = "{";
                foreach (var ar in vls2)
                {
                    string[] ta = ar.ToValArray(':');
                    if (ta?.Length == 2)
                    {
                        //jsn += string.Format("'{0}':'{1}',", ta[0], ta[1]);
                        writer.WritePropertyName(ta[0]);
                        writer.WriteValue(ta[1]);
                    }
                }

                //jsn += "'selected':'Submit'}";

                writer.WritePropertyName("selected");
                writer.WriteValue("Submit");

                writer.WriteEndObject();
            }
            return sb.ToString();
        }

        private bool IsStarMenu(List<JsonResponse> jrl)
        {
            byte o; int counter = 0;
            foreach (var jr in jrl)
            {
                foreach (var a in jr.Attachments)
                {
                    switch (a.ContentType)
                    {
                        case CardType.Button:
                            {
                                foreach (var b in a.Content.buttons)
                                {
                                    if (b.value.IsRootLevel(out o)) counter++;
                                }
                            }
                            break;
                    }
                }
            }

            return counter > 1 ? true : false;
        }
    }
}