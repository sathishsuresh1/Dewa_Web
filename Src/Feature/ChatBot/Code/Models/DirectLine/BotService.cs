using DEWAXP.Foundation.Logger;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;
using SitecoreX = Sitecore;

namespace DEWAXP.Feature.ChatBot.Models.DirectLine
{
    public class BotService
    {
        public const string _conversation_key = "C111";
        private const string ACTION_SUBMIT = "Action.Submit";
        private const string _activity_type_message = "message";
        private const string _user_id_guest = "guest";
        public const string CONTENT_TYPE_HERO = "application/vnd.microsoft.card.hero";
        public const string CONTENT_TYPE_AUDIO = "application/vnd.microsoft.card.audio";
        public const string CONTENT_TYPE_ADAPTIVE = "application/vnd.microsoft.card.adaptive";
        public const string BUTTON_IM_BACK = "imBack";
        public const string AMOUNT_REGEX = @"^-?([0-9]\d{0,5})([\.]\d{1,2})?$";
        public const string MISSING_TOKEN_ERROR = "Missing token or secret";
        public const string BADREQUEST_ERROR = "BadArgument";
        private string startupErrors;
        public ConversationModel Conversation;

        public bool ConversationStarted { get; private set; }
        public bool IsNewSession { get; set; }

        public static string Activity_type_message
        { get { return _activity_type_message; } }

        public static string User_id_guest
        { get { return _user_id_guest; } }

        public BotService(string requestLanguage, bool fullPageReload = false)
        {
            //string err;
            //bool conversation_status = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (fullPageReload == true || HttpContext.Current.Session[_conversation_key] == null)
            {
                this.ConversationStarted = this.StartConversation(out this.startupErrors, requestLanguage);
                this.IsNewSession = true;
            }
            else
            {
                this.Conversation = HttpContext.Current.Session[_conversation_key] == null ? null : (ConversationModel)HttpContext.Current.Session[_conversation_key];

                if (Conversation == null || Conversation.HasExpired || !this.Conversation.Language.Equals(requestLanguage))
                {
                    this.ConversationStarted = this.StartConversation(out this.startupErrors, requestLanguage); //  this.StartConversationRest();
                    this.Conversation.ShowPreviousChat = false; this.IsNewSession = true;
                }
                else if ((this.Conversation.Expires - DateTime.Now).TotalMinutes < 10)
                {
                    if (RefreshToken(requestLanguage)) { this.UpdateConversationState(); }
                }
                /*else
                {
                    Conversation.ShowPreviousChat = fullPageReload ? true : false;
                }*/
            }
            this.ConversationStarted = this.Conversation?.Token != null ? true : false;
        }

        public bool RefreshToken(string language)
        {
            try
            {
                using (HttpClient httpClient = GetHttpClient(false, true, this.Conversation.Token))
                {
                    HttpResponseMessage response = httpClient.PostAsync(new Uri(WebConfigurationManager.AppSettings["refreshTokenUri"]), new StringContent("")).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        LogService.Error(new System.Exception("Rammas Refresh token failed: " + response.ReasonPhrase), this);
                        return false;
                    }
                    else
                    {
                        var readString = response.Content.ReadAsStringAsync();
                        var res = Newtonsoft.Json.JsonConvert.DeserializeObject<Conversation>(readString.Result);
                        this.Conversation.Token = res.Token; this.Conversation.Expires = DateTime.Now.AddSeconds(res.ExpiresIn.Value);
                        //= new ConversationModel() { Id = res.ConversationId, Token = "Bearer " + res.Token, Expires = DateTime.Now.AddSeconds(res.ExpiresIn.Value), Language = language };

                        //HttpContext.Current.Session.Add(_conversation_key, this.Conversation);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                //err = ex.Message;
            }

            return false;
        }

        public bool StartConversation(out string err, string language)
        {
            err = string.Empty;
            try
            {
                using (HttpClient httpClient = GetHttpClient(true, true, string.Empty))
                {
                    HttpResponseMessage response = httpClient.PostAsync(new Uri(WebConfigurationManager.AppSettings["ConversationUri"]), new StringContent("")).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        err = response.ReasonPhrase;
                        return false;
                    }
                    else
                    {
                        var readString = response.Content.ReadAsStringAsync();
                        var res = Newtonsoft.Json.JsonConvert.DeserializeObject<Conversation>(readString.Result);
                        this.Conversation = new ConversationModel() { Id = res.ConversationId, Token = "Bearer " + res.Token, Expires = DateTime.Now.AddSeconds(res.ExpiresIn.Value), Language = language };
                        this.Conversation.History = new List<Journey>();

                        this.Conversation.Activity = new Activity()
                        {
                            Type = Activity_type_message,
                            From = new ChannelAccount()
                            {
                                Id = DateTime.Now.Ticks.ToString(),
                                Name = DateTime.Now.Ticks.ToString()
                            },
                            ChannelData = new { channelType = "Web-Directline" }
                        };

                        //LogService.Warn(new Exception(readString.Result.ToString(), null), this);

                        HttpContext.Current.Session.Add(_conversation_key, this.Conversation);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                err = ex.Message;
            }

            return false;
        }

        public void UpdateConversationState()
        {
            HttpContext.Current.Session.Remove(_conversation_key);

            HttpContext.Current.Session.Add(_conversation_key, this.Conversation);
        }

        public List<JsonResponse> PostConversation()
        {
            if (this.Conversation == null) goto jumpHere;
            //if (this.Conversation.CurrentUserAction == 15 || this.Conversation.CurrentUserAction == 151) return ReadConversationResponse(null, DateTime.Now);
            try
            {
                // var messageTime = DateTime.Now;
                bool tryonce = false;
            retryHere:
                using (HttpClient client = GetHttpClient(false, true, this.Conversation.Token))
                {
                    HttpResponseMessage response = client.PostAsync(new Uri(string.Format(WebConfigurationManager.AppSettings["activityUri"], this.Conversation.Id, string.Empty)), this.Conversation.Activity, new System.Net.Http.Formatting.JsonMediaTypeFormatter()).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return ReadConversationResponse(client);
                    }
                    else
                    {
                        if (IsBadToken(response) && !tryonce)
                        {
                            RefreshToken(this.Conversation.Language);
                            tryonce = true;
                            goto retryHere;
                        }
                        LogService.Error(new Exception(response.StatusCode.ToString() + ", " + this.Conversation.ErrorMessage), this);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                this.Conversation.IsError = true;
                this.Conversation.ErrorMessage = ex.Message;
            }

        jumpHere:
            return new List<JsonResponse>();
        }

        public List<JsonResponse> ReadConversationResponse(HttpClient httpClient, bool doNotReTry = false, bool postPayment = false)
        {
            List<JsonResponse> jr = new List<JsonResponse>();

            try
            {
                var retryCount = 0; ActivitySet actRS = null; bool tryonce = false;
            JumpHere:
                //if (httpClient == null) goto tempHere;

                if (!doNotReTry) System.Threading.Thread.Sleep(2000);

                var activityresponse = httpClient.GetAsync(string.Format(WebConfigurationManager.AppSettings["activityUri"], this.Conversation.Id, string.IsNullOrEmpty(this.Conversation.Watermark) ? "" : "?watermark=" + this.Conversation.Watermark)).Result;

                string resp = activityresponse.Content.ReadAsStringAsync().Result;

                if (!activityresponse.IsSuccessStatusCode)
                {
                    if (IsBadToken(activityresponse) && !tryonce)
                    {
                        RefreshToken(this.Conversation.Language);
                        tryonce = true;
                        httpClient = GetHttpClient(false, true, this.Conversation.Token);
                        goto JumpHere;
                    }
                    LogService.Error(new Exception(activityresponse.StatusCode.ToString() + ", " + this.Conversation.ErrorMessage), this);
                    return new List<JsonResponse>();
                }

#if DEBUG
                //System.Diagnostics.Debug.WriteLine(System.Text.RegularExpressions.Regex.Unescape(resp));
                LogService.Info(new Exception(System.Text.RegularExpressions.Regex.Unescape(resp), null));
#endif

                actRS = JsonConvert.DeserializeObject<ActivitySet>(resp);

                var latestActivity = actRS?.Activities?.Where(x => (x.From.Id != this.Conversation.Activity.From.Id) && (!x.Text.Equals("SSOEVENT")))?.ToList();

                if (latestActivity?.Count == 0 && retryCount < 30 && !doNotReTry)
                {
                    retryCount++;
                    goto JumpHere;
                }

                if (latestActivity == null || latestActivity.Count == 0) return jr;
                if (!string.IsNullOrEmpty(actRS?.Watermark)) { this.Conversation.Watermark = actRS.Watermark; }
                int order = 0;
                foreach (var a in latestActivity.OrderBy(x => x.Id))
                {
                    JsonResponse ajr = new JsonResponse() { Text = a.Text, Attachments = new List<Attachment>(), Order = order };
                    order++;

                    switch (a.AttachmentLayout)
                    {
                        case "carousel":
                            switch (this.Conversation.CurrentUserAction)
                            {
                                case 151:
                                    ajr.RenderType = 8;
                                    break;

                                case 152:
                                    ajr.RenderType = 81;
                                    break;

                                default:
                                    ajr.RenderType = string.IsNullOrEmpty(a.Text) ? 3 : 2;
                                    break;
                            }

                            break;

                        case "list":
                            switch (this.Conversation.CurrentUserAction)
                            {
                                case 13:
                                    ajr.RenderType = 12;
                                    break;

                                case 131:
                                case 132:
                                    ajr.RenderType = 13;
                                    break;

                                default:
                                    ajr.RenderType = string.IsNullOrEmpty(a.Text) ? 5 : 4;
                                    break;
                            }
                            break;

                        default: //null or empty
                            switch (this.Conversation.CurrentUserAction)
                            {
                                case 13:
                                    ajr.RenderType = 12;
                                    break;

                                case 131:
                                case 132:
                                    ajr.RenderType = 13;
                                    break;

                                case 15:
                                    ajr.RenderType = 7;
                                    break;

                                case 151:
                                    ajr.RenderType = 8;
                                    break;

                                case 152:
                                    ajr.RenderType = 81;
                                    break;

                                case 16:
                                case 18:
                                    ajr.RenderType = 9;
                                    break;

                                case 161:
                                    ajr.RenderType = 10;
                                    break;

                                default:
                                    ajr.RenderType = 5;
                                    break;
                            }
                            break;
                    }

                    if (a.Attachments == null || a.Attachments.Count == 0) { jr.Add(ajr); continue; }

                    foreach (var att in a.Attachments)
                    {
                        Attachment ja = new Attachment();
                        switch (att.ContentType)
                        {
                            case CONTENT_TYPE_ADAPTIVE:
                                switch (ajr.RenderType)
                                {
                                    case 8:
                                    case 81:
                                        var si = JsonConvert.DeserializeObject<StoreBody>(att.Content.ToString());
                                        if (si != null && si.body != null && si.body.Count > 0)
                                        {
                                            if (ajr.RenderType == 8)
                                            { RenderActivitySet(ja, ajr, att, si); }
                                            else { RenderStoreItemDetail(ja, ajr, att, si); }
                                        }
                                        if (si?.actions?.Count > 0 && si.actions?.FirstOrDefault()?.data?.Count() > 0)
                                        {
                                            var i = si.actions?.FirstOrDefault()?.type?.Equals(ACTION_SUBMIT);
                                            i = i == true && si.actions.FirstOrDefault()?.data?.Length > 0 ? true : false;

                                            if (i == true)
                                            {
                                                ja.ContentType = CardType.ActivitySet;
                                                UIStoreItem sd = new UIStoreItem();
                                                sd.showMore = true;
                                                sd.title = si.actions.FirstOrDefault().title;
                                                sd.detail = si.actions.FirstOrDefault().data;

                                                ja.Content.storeitems.Add(sd);
                                                ajr.Attachments.Add(ja);
                                            }
                                        }
                                        break;

                                    case 9:
                                        var sr = JsonConvert.DeserializeObject<SurveyStage1Body>(att.Content.ToString());
                                        RenderSurveyStage1(ja, ajr, att, sr ?? new SurveyStage1Body());
                                        break;

                                    case 10:
                                        var sr2 = JsonConvert.DeserializeObject<SurveyStage2Body>(att.Content.ToString());
                                        RenderSurveyStage2(ja, ajr, att, sr2 ?? new SurveyStage2Body());
                                        break;

                                    case 12:
                                    case 13:
                                        if ((this.Conversation.IsUserAuthenticated && att.Content.ToString().Contains("Container")) || postPayment)
                                        {
                                            var sr1 = JsonConvert.DeserializeObject<SurveyStage1Body>(att.Content.ToString());
                                            if (sr1.body.Count > 0 && sr1.body.LastOrDefault().items?.Count > 0)
                                            {
                                                RenderSurveyStage1(ja, ajr, att, sr1 ?? new SurveyStage1Body());
                                                ajr.RenderType = 9; this.Conversation.CurrentUserAction = 132; this.Conversation.IsSurveyExpected = true;
                                            }
                                            break;
                                        }

                                        /*if (postPayment)
                                        {
                                            var sr1 = JsonConvert.DeserializeObject<SurveyStage1Body>(att.Content.ToString());
                                            RenderSurveyStage1(ja, ajr, att, sr1 ?? new SurveyStage1Body());
                                            ajr.RenderType = 9;
                                            break;
                                        }
                                        else
                                        {*/
                                        AdaptiveCard ac = JsonConvert.DeserializeObject<AdaptiveCard>(att.Content.ToString());
                                        if (ac != null && ac.body?.Count > 0) //its easy pay TextBlock
                                        {
                                            RenderTextBlock(ja, ajr, att, ac); this.Conversation.CurrentUserAction = 131;
                                        }
                                        //}
                                        break;

                                    default:
                                        if (att.Content.ToString().Contains("Container"))
                                        {
                                            var sr1 = JsonConvert.DeserializeObject<SurveyStage1Body>(att.Content.ToString());
                                            if (sr1.body.Count > 0 && sr1.body.LastOrDefault().items?.Count > 0)
                                            {
                                                RenderSurveyStage1(ja, ajr, att, sr1 ?? new SurveyStage1Body());
                                                ajr.RenderType = 9; this.Conversation.CurrentUserAction = 132; this.Conversation.IsSurveyExpected = true;
                                            }
                                            break;
                                        }
                                        if (att.Content.ToString().Contains("consumer/billing/easypay"))
                                        {
                                            AdaptiveCard ep = JsonConvert.DeserializeObject<AdaptiveCard>(att.Content.ToString());
                                            if (ep != null && ep.body?.Count > 0) //its easy pay TextBlock
                                            {
                                                RenderTextBlock(ja, ajr, att, ep);
                                                this.Conversation.CurrentUserAction = 13; ajr.RenderType = 12;
                                                break;
                                            }
                                        }

                                        LogService.Error(new Exception("Rammas Invalid adaptive card detected." + Environment.NewLine + att.Content.ToString()), this);
                                        break;
                                }
                                break;

                            case CONTENT_TYPE_HERO:
                                ajr.RenderType = ajr.RenderType > 5 ? 5 : ajr.RenderType;

                                var heroContent = JsonConvert.DeserializeObject<HeroTypeContent>(att.Content.ToString());

                                ja.ContentType = CardType.Button; ja.Content.buttons = heroContent?.buttons;

                                ja.Content.text = string.IsNullOrEmpty(heroContent.text) ? string.Empty : heroContent.text;
                                //if (contents.buttons?.Count > 0) { ja.Content.buttons = contents.buttons; }

                                ajr.Attachments.Add(ja);
                                break;

                            case CONTENT_TYPE_AUDIO:
                                var conts = JsonConvert.DeserializeObject<Content>(att.Content.ToString());
                                ja.ContentType = CardType.Audio;
                                //ja.Content = new Content() { media = new List<Media>() };

                                if (!string.IsNullOrEmpty(conts.text)) { ja.Content.text = conts.text; }
                                if (conts.media?.Count > 0) { ja.Content.media = conts.media; }

                                ajr.Attachments.Add(ja);
                                break;
                        }
                    }

                    jr.Add(ajr);
                }
                /*var ssoevent = jr.Where(x => x.Text.Equals("SSOEVENT")).FirstOrDefault();
                if (ssoevent != null)
                {
                    jr.Remove(ssoevent);
                    if (jr.Count == 0 && retryCount < 7) { goto JumpHere; }
                }*/
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                this.Conversation.IsError = true;
                this.Conversation.ErrorMessage = ex.Message;
            }

            return jr;
        }

        #region Helper Methods

        private void RenderSurveyStage2(Attachment ja, JsonResponse ajr, Microsoft.Bot.Connector.DirectLine.Attachment att, SurveyStage2Body so)
        {
            try
            {
                ja.ContentType = CardType.ActivitySet;

                //var b = so.body[0];

                UISurveyStage2 s2 = new UISurveyStage2() { items = new List<Button>() };
                foreach (var b in so.body)
                {
                    switch (b.type)
                    {
                        case "TextBlock":
                            string ttext = b.text.TrimEnd('\r', '\n').Replace("\r\n", "|");
                            string[] ca = ttext.ToValArray('|');
                            s2.text1 = ca.Length > 0 ? ca[0] : ttext;
                            s2.text2 = ca.Length == 2 ? ca[1] : string.Empty;
                            break;
                        //case "Input.Toggle":
                        default:
                            s2.items.Add(new Button() { title = b.title.TrimEnd('\r', '\n'), value = b.id });
                            break;
                    }
                }

                s2.sectionLabel = so.actions[0]?.title;

                s2.textBoxLabel = so.actions[0]?.card?.body[0]?.text;
                s2.textBoxPH = so.actions[0]?.card?.body[1]?.placeholder;
                s2.buttonLabel = so.actions[1]?.title; //finish button text

                ja.Content.surveyStage2.Add(s2);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            ajr.Attachments.Add(ja);
        }

        private void RenderSurveyStage1(Attachment ja, JsonResponse ajr, Microsoft.Bot.Connector.DirectLine.Attachment att, SurveyStage1Body so)
        {
            try
            {
                ja.ContentType = CardType.ActivitySet;

                ja.Content.surveyStage1.Add(new UISurveyStage1()
                {
                    text = so.body[0].text,
                    happyTitle = so.body[1]?.items[0]?.columns[0]?.items[1]?.text,
                    happyImageUrl = so.body[1]?.items[0]?.columns[0]?.items[0]?.url,
                    happyValue = so.body[1]?.items[0]?.columns[0]?.items[0]?.selectAction?.data?.selected,

                    neutralTitle = so.body[1]?.items[0]?.columns[1]?.items[1]?.text,
                    neutralImageUrl = so.body[1]?.items[0]?.columns[1]?.items[0]?.url,
                    neutralValue = so.body[1]?.items[0]?.columns[1]?.items[0]?.selectAction?.data?.selected,

                    sadTitle = so.body[1]?.items[0]?.columns[2]?.items[1]?.text,
                    sadImageUrl = so.body[1]?.items[0]?.columns[2]?.items[0]?.url,
                    sadValue = so.body[1]?.items[0]?.columns[2]?.items[0]?.selectAction?.data?.selected
                });
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            ajr.Attachments.Add(ja);
        }

        private void RenderStoreItemDetail(Attachment ja, JsonResponse ajr, Microsoft.Bot.Connector.DirectLine.Attachment att, StoreBody si)
        {
            var t1_cols = si.body[1].columns[0];
            var t2_cols = si.body[2].columns[0];
            var u_cols = si.body[0].columns[0];

            try
            {
                ja.ContentType = CardType.ActivitySet;

                //ja.Content.itemdetail
                var sd = new UIStoreItemDetail();

                sd.title1 = t1_cols?.items?.First()?.text;
                sd.body1 = t2_cols?.items?.First()?.text;
                sd.title2 = si.body[3].columns[0]?.items?.First()?.text;
                sd.body2 = si.body[3].columns[0]?.items?.Last()?.text;
                sd.title3 = si.body[4].columns[0]?.items?.First()?.text;
                sd.body3 = si.body[4].columns[0]?.items?.Last()?.text;
                sd.title4 = si.body[5].columns[0]?.items?.First()?.text;
                sd.body4 = si.body[5].columns[1]?.items?.First()?.text;
                sd.url = u_cols?.items?.First()?.url;
                ja.Content.itemdetail.Add(sd);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            ajr.Attachments.Add(ja);
        }

        private void RenderActivitySet(Attachment ja, JsonResponse ajr, Microsoft.Bot.Connector.DirectLine.Attachment att, StoreBody si)
        {
            ja.ContentType = CardType.ActivitySet;

            var sd = new UIStoreItem();

            foreach (var b in si.body)
            {
                if (b?.columns?.Count > 0)
                {
                    foreach (var c in b.columns)
                    {
                        if (c.items?.Count > 0)
                        {
                            foreach (var i in c.items)
                            {
                                switch (i.type)
                                {
                                    case "TextBlock":
                                        switch (i.weight)
                                        {
                                            case "bolder": //its the main title
                                                sd.title = i.text;

                                                break;

                                            default:
                                                if (i.text != null && !string.IsNullOrEmpty(i.text)) { sd.detail = i.text; }
                                                break;
                                        }

                                        break;

                                    case "Image":
                                        if (!string.IsNullOrEmpty(i.url)) { sd.url = i.url; }
                                        //if(!string.IsNullOrEmpty(i.))
                                        break;

                                    case "ActionSet":
                                        if (i.actions != null && i.actions.Count > 0)
                                        {
                                            var action = i.actions.FirstOrDefault();
                                            //sd.id = action?.data?.number ?? 0;
                                            //sd.actionTitle = action?.title ?? string.Empty;
                                            //sd.actionValue = action?.data?.action ?? string.Empty;
                                            if (action.data == null) break;

                                            switch (action.data.action)
                                            {
                                                /*case "غير مهتم":
                                                case "Not Interested":
                                                    sd.NITitle = action.title;
                                                    sd.NIValue = action.data.action;
                                                    sd.id = action.data.number;
                                                    sd.category = action.data.category;
                                                    break;*/
                                                case "إزالة من المفضلة":
                                                case "إضافة إلى المفضلة":
                                                case "Add To Favourites":
                                                case "Remove From Favourite":
                                                    sd.AFImageUrl = action.iconUrl;
                                                    sd.AFValue = action.data.action;
                                                    sd.id = action.data.number;
                                                    sd.category = action.data.category;
                                                    break;

                                                case "عرض التفاصيل":
                                                case "View Details":
                                                    sd.VDTitle = action.title;
                                                    sd.VDValue = action.data.action;
                                                    sd.id = action.data.number;
                                                    sd.category = action.data.category;
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            sd.showAF = this.Conversation.IsUserAuthenticated;
            //sd.showNI = sd.showAF;

            ja.Content.storeitems.Add(sd);
            ajr.Attachments.Add(ja);
        }

        private void RenderTextBlock(Attachment ja, JsonResponse ajr, Microsoft.Bot.Connector.DirectLine.Attachment att, AdaptiveCard ac)
        {
            //AdaptiveCard ac = JsonConvert.DeserializeObject<AdaptiveCard>(att.Content.ToString());
            //if (ac != null)
            //{
            //if (ac.version.Equals("0.0")) {
            ja.ContentType = ac.version.Equals("0.0") ? CardType.Adaptive0 : CardType.Adaptive1;
            //ja.Content.buttons = new List<Button>();
            int counter = 1; int total = 0; bool isAmountEditable = true;
            var rx = new System.Text.RegularExpressions.Regex(AMOUNT_REGEX, System.Text.RegularExpressions.RegexOptions.Compiled);

            if (ac?.body?.Count > 0)
            {
                var txtblocks = ac.body.ToList();
                var blank = ac.body.Where(x => !string.IsNullOrEmpty(x.text) && x.text.Equals("\r\n")).FirstOrDefault();
                if (blank != null) { txtblocks.Remove(blank); }

                Button btn = new Button();
                switch (ac.version)
                {
                    case "0.0":
                        //ja.Content.buttons.Add(new Button() { title = txtblocks.FirstOrDefault()?.text, value = "", type = txtblocks.FirstOrDefault().type, first = true });
                        ja.Content.text = txtblocks.FirstOrDefault()?.text;
                        break;

                    case "1.0":

                        var amtObj = txtblocks.Where(x => !string.IsNullOrEmpty(x.text) && rx.IsMatch(x.text)).FirstOrDefault();
                        var inText = txtblocks.Where(x => x.type.Equals("Input.Text")).FirstOrDefault();
                        if (inText != null)
                        {
                            txtblocks.Remove(inText);
                            var last = txtblocks.LastOrDefault();
                            txtblocks.Remove(last);
                            AddButtons(txtblocks, ja);

                            ja.Content.buttons.Add(new Button() { title = last.text.Replace("*", ""), ph = inText.placeholder, value = amtObj?.text ?? "", type = inText.type });
                        }
                        else
                        {
                            AddButtons(txtblocks, ja); isAmountEditable = false;
                            //ja.Content.buttons.Add(new Button() { title = "<br>", value = last.text, type = last.type });
                        }
                        break;
                }
                total = ja.Content.buttons.Count + (ac.actions != null ? ac.actions.Count : 0);

                if (total == txtblocks.Count && ja.Content.buttons?.Count > 0) { ja.Content.buttons.LastOrDefault().last = true; }
                counter = txtblocks.Count;
            }
            //ja.Content.buttons = ac?.body?.Count > 0 ? ac.body.Where(x => !string.IsNullOrEmpty(x.text) && !x.text.Equals("\r\n")).Select(x => new Button() { title = x.text, type = x.type, value = x.text }).ToList() : new List<Button>();
            var action = ac?.actions?.FirstOrDefault();
            if (action == null && ja.Content.buttons?.Count > 0)
            {
                ja.Content.buttons.LastOrDefault().last = true;
            }
            else
            {
                var pbtn = new Button() { title = action?.title, type = action?.type, value = action?.url, last = true };

                if (!isAmountEditable && this.Conversation.IsUserAuthenticated)
                {
                    var amtObj = ac.body.Where(x => !string.IsNullOrEmpty(x.text) && rx.IsMatch(x.text)).FirstOrDefault();
                    if (amtObj != null && !string.IsNullOrEmpty(amtObj.text))
                    {
                        pbtn.value = amtObj.text;
                    }
                    //pbtn.value
                }
                ja.Content.buttons.Add(pbtn);
            }

            ajr.Attachments.Add(ja);
            //}
        }

        private void AddButtons(List<TextBlock> txtblocks, Attachment ja)
        {
            Button btn = new Button(); bool isFirst = true;

            for (int i = 0; i < txtblocks.Count; i++)
            {
                btn.first = isFirst; btn.type = txtblocks[i].type;

                if (i == 0 || (i % 2) == 0)
                {
                    btn.title = txtblocks[i].text.EncodeAstarics();
                    if ((i + 1) == txtblocks.Count)
                    {
                        ja.Content.buttons.Add(new Button() { title = btn.title, value = "-", type = btn.type });
                    }
                }
                else
                {
                    btn.value = txtblocks[i].text.EncodeAstarics();
                    ja.Content.buttons.Add(new Button() { title = btn.title.EncodeAstarics(), value = btn.value, type = btn.type });
                }
                isFirst = false;
            }
        }

        #endregion Helper Methods

        #region Static Methods

        public static bool ValidateDirectLine(DirectLineValidationModel model, ConversationModel con)
        {
            try
            {
                con.Activity.Text = "SSOEVENT";
                con.Activity.Type = "event";
                con.Activity.Name = "SSOEVENT";
                con.Activity.Value = Newtonsoft.Json.Linq.JValue.Parse(model.ToJson(false));
                //Newtonsoft.Json.Linq.JValue.Parse(@"{'sessionId':'" + model.SessionId + "','userName':'" + model.UserName + "','loginType':'" + model.LoginType + "','displayName':'" + model.DisplayName + "','magicNumber':'" + model.MagicNumber + "','languageCode':'" + model.LanguageCode + "'}");

                using (HttpClient client = GetHttpClient(false, true, con.Token))
                {
                    HttpResponseMessage response = client.PostAsync(new Uri(string.Format(WebConfigurationManager.AppSettings["activityUri"],
                        con.Id, string.Empty)), con.Activity, new System.Net.Http.Formatting.JsonMediaTypeFormatter()).Result;
#if DEBUG
                    SitecoreX.Diagnostics.Log.Info("Rammas SSO Login \r\n " + JsonConvert.SerializeObject(con.Activity) + Environment.NewLine, typeof(BotService));
                    SitecoreX.Diagnostics.Log.Info("Rammas SSO Response \r\n " + JsonConvert.SerializeObject(response), typeof(BotService));
#endif
                    con.Activity.Text = con.Activity.Name = string.Empty; con.Activity.Value = null; con.Activity.Type = "message";
                    if (!response.IsSuccessStatusCode)
                    {
                        LogService.Error(new Exception(string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)), typeof(BotService));
                    }
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, typeof(BotService));
            }

            return false;
        }

        public static bool ValidateDirectLineAfterChatInit(DirectLineValidationModel model, ConversationModel con)
        {
            try
            {
                //var messageTime = DateTime.Now;

                var json = Newtonsoft.Json.Linq.JValue.Parse(model.ToJson(true));
                //@"{'sessionId':'" + model.SessionId + "','userName':'" + model.UserName + "','loginType':'" + model.LoginType + "','displayName':'" + model.DisplayName + "','magicNumber':'" + model.MagicNumber + "','languageCode':'" + model.LanguageCode + "','conversationId':'" + con.Id + "'}");

                using (HttpClient client = GetHttpClient(false, true, con.Token))
                {
                    HttpResponseMessage response = client.PostAsJsonAsync(new Uri(WebConfigurationManager.AppSettings["validateDirectLineUri"]), json).Result;
#if DEBUG
                    SitecoreX.Diagnostics.Log.Info("Rammas AfterChatInit Login Response \r\n " + JsonConvert.SerializeObject(response), typeof(BotService));
#endif
                    if (!response.IsSuccessStatusCode)
                    {
                        LogService.Error(new Exception(string.Format("{0} {1}", response.StatusCode, response.ReasonPhrase)), typeof(BotService));
                    }
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, typeof(BotService));
            }

            return false;
        }

        /// <summary>
        /// provide token for already started conversations, can be empty in case of new Conversation.
        /// </summary>
        /// <param name="isNew"></param>
        /// <param name="addAuthorizationHeader"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static HttpClient GetHttpClient(bool isNew = false, bool addAuthorizationHeader = true, string token = null)
        {
            //ICredentials credentials = new NetworkCredential(_proxyUser, _proxyPassword);
            ICredentials credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"], WebConfigurationManager.AppSettings["PROXYDOMAIN"]);
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"], true, null, credentials),
                UseProxy = true
            };

            HttpClient httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (addAuthorizationHeader)
            { httpClient.DefaultRequestHeaders.Add("Authorization", isNew ? WebConfigurationManager.AppSettings["directline_secret"] : token); }
            /*#if DEBUG
                        System.Diagnostics.Debug.WriteLine("isNew:{0}  addAuthHeader: {1}  token: {2}", isNew, addAuthorizationHeader, token);
            #endif*/
            return httpClient;
        }

        private bool IsBadToken(HttpResponseMessage res)
        {
            string resp = res.Content.ReadAsStringAsync().Result;
            this.Conversation.IsError = true;
            this.Conversation.ErrorMessage = "Token Error: " + resp;
            if (resp.Contains(BADREQUEST_ERROR) || resp.Contains(MISSING_TOKEN_ERROR))
            {
                return true;
            }

            return false;
        }

        #endregion Static Methods
    }

    [Serializable]
    public class ConversationModel
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }

        public Activity Activity { get; set; }
        public string Watermark { get; set; }

        public bool HasExpired
        {
            get { return this.Expires == null ? true : (this.Expires <= DateTime.Now ? true : false); }
        }

        public bool ShowPreviousChat { get; set; }

        public string Language { get; set; }
        /*public void UpdateWatermark(string watermark, string cacheKey)
        {
            this.Watermark = watermark; this.Activity.Text = string.Empty; this.Activity.Value = null;
            HttpContext.Current.Session[cacheKey] = this;
        }*/
        public byte PreviousUserAction { get; set; }
        public byte CurrentUserAction { get; set; }
        public byte MasterUserAction { get; set; }
        public byte TempUserAction { get; set; }
        public bool UserAuthenticated { get; set; }
        public bool IsUserAuthenticated
        { get { return (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated; } }
        public List<Journey> History { get; set; }
        public string LastUserAction { get; set; }
        public bool IsSurveyExpected { get; set; }

        [NonSerialized]
        public bool IsError;

        [NonSerialized]
        public string ErrorMessage;

        public void UpdateHistory(List<JsonResponse> cl, string userAction)
        {
            if (!string.IsNullOrEmpty(userAction) && userAction.Contains(",")) { userAction = userAction.Substring(0, userAction.IndexOf(",")); }
            int orderH = this.History.Count > 0 ? this.History.Max(x => x.Order) + 1 : 0;
            Journey jrn = new Journey() { UserAction = userAction, Order = orderH, Responses = new List<JsonResponse>() };

            foreach (var jr in cl)
            {
                JsonResponse res = new JsonResponse() { Order = jr.Order, RenderType = jr.RenderType, Text = jr.Text, UserAction = jr.UserAction, Attachments = new List<Attachment>() };
                if (jr.Attachments == null || jr.Attachments.Count == 0) { jrn.Responses.Add(res); continue; }
                foreach (var att in jr.Attachments)
                {
                    if (att.ContentType != CardType.Audio) { res.Attachments.Add(att); }
                }
                jrn.Responses.Add(res);
            }
            jrn.ActivityDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.History.Add(jrn);
        }
    }

    public class Media
    {
        public string url { get; set; }
    }

    public class Button
    {
        public string type { get; set; }
        public string title { get; set; }
        public string value { get; set; }
        public bool first { get; set; }
        public bool last { get; set; }

        /// <summary>
        /// Placeholder text
        /// </summary>
        public string ph { get; set; }
    }

    public class UIStoreItem
    {
        public string title { get; set; }
        public string detail { get; set; }
        public string url { get; set; }
        public int id { get; set; }

        /// <summary>
        /// Show Not Interested Button
        /// </summary>
        //public bool showNI { get; set; }
        /// <summary>
        /// Show Add to Favorites
        /// </summary>
        public bool showAF { get; set; }

        public string AFImageUrl { get; set; }
        public bool showMore { get; set; }

        //public string showMoreText { get; set; }
        public string AFValue { get; set; }

        //public string NITitle { get; set; }
        //public string NIValue { get; set; }
        public string VDTitle { get; set; }

        public string VDValue { get; set; }
        public int category { get; set; }
    }

    public class UIStoreItemDetail
    {
        public string url { get; set; }
        public string title1 { get; set; }
        public string body1 { get; set; }
        public string title2 { get; set; }
        public string body2 { get; set; }
        public string title3 { get; set; }
        public string body3 { get; set; }
        public string title4 { get; set; }
        public string body4 { get; set; }
    }

    public class UISurveyStage1
    {
        public string text { get; set; }
        public string happyTitle { get; set; }
        public string happyValue { get; set; }
        public string happyImageUrl { get; set; }
        public string neutralTitle { get; set; }
        public string neutralValue { get; set; }
        public string neutralImageUrl { get; set; }
        public string sadTitle { get; set; }
        public string sadValue { get; set; }
        public string sadImageUrl { get; set; }
    }

    public class UISurveyStage2
    {
        public string text1 { get; set; }
        public string text2 { get; set; }
        public List<Button> items { get; set; }
        public string textBoxLabel { get; set; }
        public string textBoxPH { get; set; }
        public string buttonLabel { get; set; }
        public string sectionLabel { get; set; }
    }

    public class Content
    {
        public string text { get; set; }
        public List<Button> buttons { get; set; }
        public List<Media> media { get; set; }
        public List<UIStoreItem> storeitems { get; set; }
        public List<UIStoreItemDetail> itemdetail { get; set; }
        public List<UISurveyStage1> surveyStage1 { get; set; }
        public List<UISurveyStage2> surveyStage2 { get; set; }
    }

    public class JsonResponse
    {
        /// <summary>
        /// how to render card
        /// 1 > Text only
        /// 2 > Text with Carousel
        /// 3 > Carousel - Buttons
        /// 4 > Text with List
        /// 5 > list - Buttons
        /// 6 > adaptive card easy pay
        /// 7 > adaptive card dewa store carosel
        /// </summary>
        public int RenderType { get; set; }

        public string Text { get; set; }

        //public bool IsCarousel { get; set; }
        public List<Attachment> Attachments { get; set; }

        public int Order { get; set; }
        public string UserAction { get; set; }
    }

    public enum CardType
    {
        Adaptive0, Adaptive1, Button, Audio, Url, ActivitySet
    }

    public class Attachment
    {
        public Attachment()
        {
            this.Content = new Content()
            {
                buttons = new List<Button>(),
                media = new List<Media>(),
                storeitems = new List<UIStoreItem>(),
                itemdetail = new List<UIStoreItemDetail>(),
                surveyStage1 = new List<UISurveyStage1>(),
                surveyStage2 = new List<UISurveyStage2>()
            };
            //this.ColumnSet = new List<StoreColumnSet>();
        }

        public CardType ContentType { get; set; }
        public Content Content { get; set; }
        //public List<StoreColumnSet> ColumnSet { get; set; }
    }

    //Start new code
    public class Images
    {
    }

    public class MobileApp
    {
        public string type { get; set; }
        public string title { get; set; }
        public string value { get; set; }
    }

    public class Web
    {
        public string type { get; set; }
        public string title { get; set; }
        public string value { get; set; }
    }

    public class Buttons
    {
        public List<MobileApp> mobileApp { get; set; }
        public List<Web> web { get; set; }
    }

    public class ChannelData
    {
        public string displayformat { get; set; }
        public Images images { get; set; }
        public Buttons buttons { get; set; }
    }

    //End new code

    #region Adaptive Cards - Easy Pay

    public class BaseBlock
    {
        public string type { get; set; }
    }

    public class TextBlock : BaseBlock
    {
        public string text { get; set; }
        public string id { get; set; }
        public string placeholder { get; set; }
    }

    public class Action
    {
        public string type { get; set; }
        public string url { get; set; }
        public string title { get; set; }
    }

    public class AdaptiveCard
    {
        //public string type { get; set; }
        //public string version { get; set; }
        public string version { get; set; }

        public List<TextBlock> body { get; set; }
        public List<Action> actions { get; set; }
    }

    #endregion Adaptive Cards - Easy Pay

    #region Hero Cards

    /*public class HeroCard
    {
        public List<Button> buttons { get; set; }
    }*/

    public class HeroTypeContent
    {
        public string contentType { get; set; }

        //public object contentUrl { get; set; }
        public List<Button> buttons { get; set; }

        public string text { get; set; }
        //public object name { get; set; }
        //public object thumbnailUrl { get; set; }
    }

    #endregion Hero Cards

    #region Adaptive Cards - Store Carosel

    public class ActionData
    {
        public string action { get; set; }
        public int number { get; set; }
        public int category { get; set; }
    }

    public class ShowMore
    {
        public string type { get; set; }
        public string data { get; set; }
        public string title { get; set; }
    }

    public class ColumnAction
    {
        public string type { get; set; }
        public string id { get; set; }
        public ActionData data { get; set; }
        public string title { get; set; }
        public string iconUrl { get; set; }
    }

    public class Item
    {
        public string type { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public string weight { get; set; }
        public bool wrap { get; set; }
        public int maxLines { get; set; }
        public string horizontalAlignment { get; set; }
        public string altText { get; set; }
        public List<ColumnAction> actions { get; set; }
    }

    public class Column
    {
        public string type { get; set; }
        public string width { get; set; }
        public List<Item> items { get; set; }
    }

    public class StoreColumnSet
    {
        public string type { get; set; }
        public List<Column> columns { get; set; }
        public string style { get; set; }
        public bool bleed { get; set; }
        public string height { get; set; }
    }

    public class StoreBody
    {
        public string type { get; set; }
        public string version { get; set; }
        public List<StoreColumnSet> body { get; set; }
        public List<ShowMore> actions { get; set; }
        public string minHeight { get; set; }
    }

    public class DEWAStoreItem
    {
        public string contentType { get; set; }

        //public object contentUrl { get; set; }
        public StoreBody content { get; set; }

        //public object name { get; set; }
        //public object thumbnailUrl { get; set; }
    }

    #endregion Adaptive Cards - Store Carosel

    #region SurveyCard

    public class Data
    {
        public string selected { get; set; }
    }

    public class SelectAction
    {
        public string type { get; set; }
        public Data data { get; set; }
        public string title { get; set; }
    }

    public class SurveyItemSub
    {
        public string type { get; set; }
        public string size { get; set; }
        public string url { get; set; }
        public SelectAction selectAction { get; set; }
        public string weight { get; set; }
        public string text { get; set; }
        public string horizontalAlignment { get; set; }
    }

    public class SurveyColumn
    {
        public string type { get; set; }
        public string width { get; set; }
        public List<SurveyItemSub> items { get; set; }
    }

    public class SurveyItemMain
    {
        public string type { get; set; }
        public List<SurveyColumn> columns { get; set; }
        public string horizontalAlignment { get; set; }
    }

    public class SurveyObject
    {
        public string type { get; set; }
        public string size { get; set; }
        public string text { get; set; }
        public string horizontalAlignment { get; set; }
        public List<SurveyItemMain> items { get; set; }
    }

    public class SurveyStage1Body
    {
        public string type { get; set; }
        public string version { get; set; }
        public List<SurveyObject> body { get; set; }
        public string minHeight { get; set; }
    }

    #endregion SurveyCard

    #region SurveyOptions

    public class Stage2Body
    {
        public string type { get; set; }
        public string size { get; set; }
        public string weight { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string valueOn { get; set; }
        public string valueOff { get; set; }
        public bool? wrap { get; set; }
        public string value { get; set; }
    }

    public class Stage2ActionCardBody
    {
        public string type { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public string placeholder { get; set; }
        public bool? isMultiline { get; set; }
        public int? maxLength { get; set; }
    }

    public class Stage2Card
    {
        public string type { get; set; }
        public List<Stage2ActionCardBody> body { get; set; }
    }

    public class Stage2Data
    {
        public string selected { get; set; }
    }

    public class Stage2Action
    {
        public string type { get; set; }
        public Stage2Card card { get; set; }
        public string title { get; set; }
        public Stage2Data data { get; set; }
    }

    public class SurveyStage2Body
    {
        public string type { get; set; }
        public string version { get; set; }
        public List<Stage2Body> body { get; set; }
        public List<Stage2Action> actions { get; set; }
    }

    #endregion SurveyOptions

    public class Journey
    {
        public int Order { get; set; }
        public string UserAction { get; set; }
        public List<JsonResponse> Responses { get; set; }
        public string ActivityDate { get; set; }
    }
}