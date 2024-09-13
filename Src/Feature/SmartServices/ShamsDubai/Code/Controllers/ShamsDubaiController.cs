using DEWAXP.Feature.ShamsDubai.Models.ShamsDubai;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.Kofax;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Logger;
using ShamsDubaiEnum = DEWAXP.Feature.ShamsDubai.Models.ShamsDubai.ShamsDubai;
using Newtonsoft.Json;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Attribute = DEWAXP.Foundation.Content.Models.Kofax.Attribute;

namespace DEWAXP.Feature.ShamsDubai.Controllers
{
    public class ShamsDubaiController : BaseController
    {
        #region Properties

        public string folderId = string.Empty;

        public string FolderId
        {
            get
            {
                if (Request.QueryString.Count > 0 && !string.IsNullOrEmpty(Request.QueryString["Folder_ID"]))
                {
                    folderId = Convert.ToString(Request.QueryString["Folder_ID"]);
                }
                return folderId;
            }
        }
        public string action = string.Empty;

        public string Action
        {
            get
            {
                if (Request.QueryString.Count > 0 && !String.IsNullOrEmpty(Request.QueryString["q"]))
                {
                    action = Convert.ToString(Request.QueryString["q"]);
                }
                return action;
            }
        }
        #endregion Properties

        #region MVC Actions

        /// <summary>
        /// Action Method Called by the Sitecore to Post the Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SubmitForm(SubscribeModel model)
        {
            bool status = false;
            string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

            if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
            {
                status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
            }
            else if (!ReCaptchaHelper.Recaptchasetting())
            {
                status = true;
            }
            CacheProvider.Store(CacheKeys.ShamsDubaisubscriptionmodel, new AccessCountingCacheItem<SubscribeModel>(model, Times.Once));
            if (status)
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.UnsubscribeEmail) && model.q.ToUpper() == "US")
                    {
                        model.ProcessingType = "3";
                        var isSaved = SaveSubscribeUser(model, ShamsDubaiEnum.Unsubscribe);
                        if (isSaved.Item1 == true)
                        {
                            CacheProvider.Store(CacheKeys.ShamsDubaisubscription, new AccessCountingCacheItem<string>("usd", Times.Once));
                            
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSDONE);
                        }
                        else
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(isSaved.Item2, Times.Once));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSCRIPTION);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.Email))
                    {
                        model.ProcessingType = "1";
                        var isSaved = SaveSubscribeUser(model, ShamsDubaiEnum.Subscribe);
                        if (isSaved.Item1 == true)
                        {
                            CacheProvider.Store(CacheKeys.ShamsDubaisubscription, new AccessCountingCacheItem<string>("sd", Times.Once));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSDONE);
                        }
                        else
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(isSaved.Item2, Times.Once));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSCRIPTION);
                        }
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Shams-Msg-Exists-Email"), Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSCRIPTION);
                    }
                }
                else
                {
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Shams-Msg-Exists-Email"), Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSCRIPTION);
                }
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Shams-Captcha-Not-Valid"), Times.Once));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSCRIPTION);
            }
        }

        [HttpGet]
        public ActionResult SubmitForm()
        {
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            ViewBag.QueryParamter = Request.QueryString["q"];

            SubscribeModel model = new SubscribeModel();
            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            if (!CacheProvider.TryGet(CacheKeys.ShamsDubaisubscriptionmodel, out model))
            {
                model = new SubscribeModel();
            }
            return PartialView("~/Views/Feature/ShamsDubai/ShamsDubai/_NewRequest.cshtml", GetDropDownList(model));
        }

        /// <summary>
        /// Subsciption action calls the subscription method to confirm the user subscription
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Subscription()
        {
            SubscribeModel model = new SubscribeModel();
            string action;
            if (CacheProvider.TryGet(CacheKeys.ShamsDubaisubscription, out action))
            {
                if (action.ToUpper() == "USD")
                {
                    model.Message = Translate.Text("Shams-Msg-US");
                }
                else if (action.ToUpper() == "SD")
                {
                    model.Message = Translate.Text("Shams-Msg-SD");
                }
                return PartialView("~/Views/Feature/ShamsDubai/ShamsDubai/_SubmissionSuccess.cshtml", model);
            }
            if (!string.IsNullOrEmpty(FolderId) && !string.IsNullOrEmpty(Action) && Action.ToUpper() == "SU")
            {
                model.ProcessingType = "2";
                model.UniqueID = FolderId;
                var isSaved = SaveSubscribeUser(model, ShamsDubaiEnum.Confirm);
                if (isSaved.Item1 == true)
                {
                    model.Message = Translate.Text("Shams-Msg-SC");
                }
                else
                {
                    model.Message = Translate.Text("Shams-Msg-Error-Conf-Sub");
                }
                return PartialView("~/Views/Feature/ShamsDubai/ShamsDubai/_SubmissionSuccess.cshtml", model);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_DUBAI_SUBSCRIPTION);
        }

        #endregion MVC Actions

        #region Methods

        /// <summary>
        /// GetDropDown List populates the list of subscription type from sitecore datasource
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SubscribeModel</returns>
        private SubscribeModel GetDropDownList(SubscribeModel model)
        {
            var customerType = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.SHAMSDUBAICUSTOMERTYPE)).Items;
            var customerTypeItems = customerType.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
            model.CustomerTypeList = customerTypeItems.ToList();

            return model;
        }

        #endregion Methods

        #region Subcribe User

        private Tuple<bool, string, string> SaveSubscribeUser(SubscribeModel model, ShamsDubaiEnum value)
        {
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                KofaxBaseViewModel kofaxBaseViewModel = new KofaxBaseViewModel();
                switch (value)
                {
                    case ShamsDubaiEnum.Subscribe:
                        kofaxBaseViewModel.Parameters.Add(new Parameter(KofaxConstants.ShamsDubaiName) { Attribute = GetSubscribeUserAttribute(model) });
                        break;

                    case ShamsDubaiEnum.Unsubscribe:
                        kofaxBaseViewModel.Parameters.Add(new Parameter(KofaxConstants.ShamsDubaiName) { Attribute = GetUnSubscribeUserAttribute(model) });
                        break;

                    case ShamsDubaiEnum.Confirm:
                        kofaxBaseViewModel.Parameters.Add(new Parameter(KofaxConstants.ShamsDubaiName) { Attribute = GetConfirmSubscribeUserAttribute(model) });
                        break;
                }
                var res = KofaxRESTService.SubmitKofax(KofaxConstants.ShamsDubaiEmailSubscription, JsonConvert.SerializeObject(kofaxBaseViewModel, Converter.Settings));
                LogService.Info(new Exception("Shams Dubai Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    string newid = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("UniqueID") && x.Type.Equals("text")).FirstOrDefault().Value;
                    string message = string.Empty;
                    if (res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Run_Status") && x.Type.Equals("text")).FirstOrDefault().Value.Equals("Failed"))
                    {
                        message = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Error_Message") && x.Type.Equals("text")).FirstOrDefault().Value;
                        retval = new Tuple<bool, string, string>(false, message, newid);
                    }
                    else
                    {
                        retval = new Tuple<bool, string, string>(true, message, newid);
                    }
                }
                else
                {
                    retval = new Tuple<bool, string, string>(false, res.Message, "");
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return retval;
        }

        #endregion Subcribe User

        #region Get Subcriber User Attributes

        private List<Attribute> GetSubscribeUserAttribute(SubscribeModel model)
        {
            List<Attribute> retval = new List<Attribute>();
            string lang = string.Empty;
            lang = Sitecore.Globalization.Language.Current.ToString() == "en" ? "en" : "ar";

            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "ProcessingType", Value = model.ProcessingType.ToString() ?? string.Empty });
            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "Name", Value = model.Name.Trim() ?? string.Empty });
            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "Email", Value = model.Email.Trim() ?? string.Empty });
            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "Language", Value = lang.ToUpper() ?? string.Empty });

            if (!String.IsNullOrEmpty(model.Others))
            {
                retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "UserType", Value = model.Others ?? string.Empty });
            }
            else
            {
                retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "UserType", Value = model.SubscriberType.ToString() ?? string.Empty });
            }
            return retval;
        }

        #endregion Get Subcriber User Attributes

        #region Confirm User Subscribtion Attributes

        private List<Attribute> GetConfirmSubscribeUserAttribute(SubscribeModel model)
        {
            List<Attribute> retval = new List<Attribute>();
            string lang = string.Empty;
            lang = Sitecore.Globalization.Language.Current.ToString() == "en" ? "en" : "ar";

            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "ProcessingType", Value = model.ProcessingType.ToString() ?? string.Empty });
            retval.Add(new Attribute { Type = kofaxTypeEnum.Integer, Name = "UniqueID", Value = model.UniqueID ?? string.Empty });
            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "Language", Value = lang.ToUpper() ?? string.Empty });

            return retval;
        }

        #endregion Confirm User Subscribtion Attributes

        #region Confirm User UnSubscribtion Attributes

        private List<Attribute> GetUnSubscribeUserAttribute(SubscribeModel model)
        {
            List<Attribute> retval = new List<Attribute>();
            string lang = string.Empty;
            lang = Sitecore.Globalization.Language.Current.ToString() == "en" ? "en" : "ar";

            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "ProcessingType", Value = model.ProcessingType.ToString() ?? string.Empty });
            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "Email", Value = model.UnsubscribeEmail ?? string.Empty });
            retval.Add(new Attribute { Type = kofaxTypeEnum.Text, Name = "Language", Value = lang.ToUpper() ?? string.Empty });

            return retval;
        }

        #endregion Confirm User UnSubscribtion Attributes
    }
}