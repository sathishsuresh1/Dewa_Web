using DEWAXP.Feature.ChatBot.Models.HayakChat;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.ChatBot.Controllers
{
    public class HayakChatController : BaseController
    {
        private readonly IHayakClient _hayakClient;
        protected IHayakClient HayakClientReq => _hayakClient;

        public HayakChatController() : base()
        {
            _hayakClient = DependencyResolver.Current.GetService<IHayakClient>();
        }

        public ActionResult InitializeChatWithBot()
        {
            var model = new HayakChatModel() { UserProfile = CurrentPrincipal };
            var acid = GetContextId(new BaseContactDetailsModel() { ChatBotRequired = true }, "N", "WEB", "Rammas");
            model.RequestId = acid.Item1;
            model.RequestLang = acid.Item2;

            return View("~/Views/Feature/Rammas/RammasDirectLine/RammasAvayaLandingPage.cshtml", model);
        }

        public ActionResult InitializeChat()
        {
            return View("~/Views/Feature/Rammas/HayakChat/HayakChatLandingPage.cshtml", new HayakChatModel() { UserProfile = CurrentPrincipal });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InitializeChat(BaseContactDetailsModel model)
        {
            var acid = GetContextId(model);

            return Json(new { requestId = acid.Item1 ?? "", language = acid.Item2 ?? "" }, JsonRequestBehavior.AllowGet);
        }

        private Tuple<string, string> GetContextId(BaseContactDetailsModel model, string authenticated = "N", string originType = "WEB", string serviceType = "Hayak")
        {
            Data jmodel = new Data()
            {
                Authenticated = authenticated,
                ChatbotRequired = model.ChatBotRequired ? "Y" : "N",
                Language = (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic ? "Arabic" : "English"),
                OriginType = originType,
                ServiceType = serviceType
            };
            if (User.Identity.IsAuthenticated)
            {
                //base.RequestLanguage
                jmodel.Authenticated = "Y";
                jmodel.FullName = CurrentPrincipal.FullName;
                jmodel.EmailAddress = CurrentPrincipal.EmailAddress;
                jmodel.MobileNumber = CurrentPrincipal.MobileNumber;
                jmodel.AccountNumber = model.AccountNumber;
            }
            else
            {
                jmodel.FullName = model.FullName;
                jmodel.EmailAddress = model.EmailAddress;
                jmodel.MobileNumber = model.MobileNumber;
                jmodel.AccountNumber = model.AccountNumber;
            }
            string requestId = string.Empty;

            try
            {
                var req = Newtonsoft.Json.JsonConvert.SerializeObject(getReqJson(jmodel));

#if DEBUG
                global::Sitecore.Diagnostics.Log.Info(req, typeof(HayakChatController));
#endif
                var response = HayakClientReq.GetContextId(req);
                if (response != null && response.Succeeded)
                {
                    requestId = response.Payload;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this); //model.IsServerError = true;
            }

            return new Tuple<string, string>(requestId, jmodel.Language);
        }

        private InitReqRoot getReqJson(Data data)
        {
            //long cid = DateTime.Now.Ticks;
            InitReqRoot ro = new InitReqRoot()
            {
                groupId = data.AccountNumber,
                persistToEDM = true,
                //data = new Data() { AccountNumber = "", CustomerName = "", EmailAddress = "" },
                data = data,
                schema = new Schema()
                {
                    CustomerId = data.AccountNumber,
                    Locale = Request.UserLanguages.FirstOrDefault(),
                    ServiceMap = new ServiceMap()
                    {
                        SM1 = new SM()
                        {
                            attributes = new Attributes()
                            {
                                Channel = new List<string>() { "Chat" },
                                Language = new List<string>() { data.Language },
                                ServiceType = new List<string>() { data.ServiceType }
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