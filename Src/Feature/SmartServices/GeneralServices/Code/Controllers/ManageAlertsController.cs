using DEWAXP.Feature.GeneralServices.Models.ManageAlerts;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    public class ManageAlertsController : BaseServiceActivationController
    {
        [HttpGet]
        public ActionResult UnsubscribeEmail()
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
            ViewBag.Reasons = PopulateReasons();
            return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/Unsubscribe.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnsubscribeEmail(Unsubscribe Model)
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
            if (status)
            {
                var subscribedresponse = DewaApiClient.SetSubscription(string.Empty, Model.selectedreason, Model.emailid, "2", string.Empty, string.Empty, RequestLanguage, Request.Segment());
                if (subscribedresponse.Succeeded)
                {
                    CacheProvider.Store(CacheKeys.UnsubscribeConfirmation, new CacheItem<string>(subscribedresponse.Payload.@return.description, TimeSpan.FromMinutes(100)));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.UnsubscribeThankyou);
                }
                ModelState.AddModelError(string.Empty, subscribedresponse.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
            }
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            ViewBag.Reasons = PopulateReasons();
            return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/Unsubscribe.cshtml", Model);
        }

        [HttpGet]
        public ActionResult UnsubscribeConfirmation()
        {
            string selection;
            if (CacheProvider.TryGet(CacheKeys.UnsubscribeConfirmation, out selection))
            {
                Unsubscribe Model = new Unsubscribe
                {
                    description = selection
                };
                return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/Confirmation.cshtml", Model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ManageSubscriptionerror);
        }

        [HttpGet]
        public ActionResult ManagePreference(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var subscribedresponse = DewaApiClient.SetSubscription(id, "R", string.Empty, string.Empty, string.Empty, string.Empty, RequestLanguage, Request.Segment());
                if (subscribedresponse.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(subscribedresponse.Payload.@return.subscribedEmail) || !string.IsNullOrWhiteSpace(subscribedresponse.Payload.@return.subscribedMobile))
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
                        Unsubscribe Model = new Unsubscribe
                        {
                            emailid = subscribedresponse.Payload.@return.subscribedEmail,
                            emailflag = subscribedresponse.Payload.@return.emailSubscribedFlag,
                            mobile = subscribedresponse.Payload.@return.subscribedMobile,
                            mobileflag = subscribedresponse.Payload.@return.mobileSubscribedFlag,
                            contractaccount = id,
                            description = subscribedresponse.Payload.@return.description
                        };
                        CacheProvider.Store(CacheKeys.ManagePreference, new CacheItem<Unsubscribe>(Model, TimeSpan.FromMinutes(100)));
                        return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/Managepreference.cshtml", Model);
                    }
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ManageSubscriptionerror);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManagePreference(string description, string contractaccount, string emailflag, string emailid, string mobile, string mobileflag)
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
            if (!string.IsNullOrWhiteSpace(contractaccount) && status)
            {
                var subscribedresponse = DewaApiClient.SetSubscription(contractaccount, "W", emailid, emailflag, mobile, mobileflag, RequestLanguage, Request.Segment());
                if (subscribedresponse.Succeeded)
                {
                    if (emailflag.Equals("1"))
                    {
                        CacheProvider.Store(CacheKeys.ManagePreferenceConfirmationtitle, new CacheItem<string>(Translate.Text("Subscribe success title"), TimeSpan.FromMinutes(100)));
                        CacheProvider.Store(CacheKeys.ManagePreferenceConfirmation, new CacheItem<string>(Translate.Text("Managepreference success"), TimeSpan.FromMinutes(100)));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ManagePreferenceConfirmationtitle, new CacheItem<string>(Translate.Text("UnSubscribe success title"), TimeSpan.FromMinutes(100)));
                        CacheProvider.Store(CacheKeys.ManagePreferenceConfirmation, new CacheItem<string>(Translate.Text("Managepreference unsuccess"), TimeSpan.FromMinutes(100)));
                    }
                    //CacheProvider.Store(CacheKeys.ManagePreferenceConfirmation, new CacheItem<string>(subscribedresponse.Payload.@return.description, TimeSpan.FromMinutes(100)));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.ManageSubscriptionconfirmation);
                }
                ModelState.AddModelError(string.Empty, subscribedresponse.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
            }
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            Unsubscribe Model = new Unsubscribe();
            if (CacheProvider.TryGet(CacheKeys.ManagePreference, out Model))
            {
                return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/Managepreference.cshtml", Model);
            }
            //return PartialView("Managepreference", Model);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ManageSubscriptionerror);
        }

        [HttpGet]
        public ActionResult ManagePreferenceConfirmation()
        {
            string selection, title;
            if (CacheProvider.TryGet(CacheKeys.ManagePreferenceConfirmation, out selection) && CacheProvider.TryGet(CacheKeys.ManagePreferenceConfirmationtitle, out title))
            {
                Unsubscribe Model = new Unsubscribe
                {
                    description = selection,
                    Title = title
                };
                return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/ManagepreferenceConfirmation.cshtml", Model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ManageSubscriptionerror);
        }

        [HttpGet]
        public ActionResult Verify(string id, string subscribe = "2")
        {
            Unsubscribe errorModel = new Unsubscribe
            {
                description = Translate.Text("Please Enter the valid link")
            };
            if (!string.IsNullOrWhiteSpace(id))
            {
                string readwritemode = "V";
                if (!string.IsNullOrWhiteSpace(subscribe) && !subscribe.Equals("2"))
                {
                    readwritemode = "S";
                }
                var subscribedresponse = DewaApiClient.SetSubscription(id, readwritemode, string.Empty, subscribe, string.Empty, string.Empty, RequestLanguage, Request.Segment());
                if (subscribedresponse.Succeeded)
                {
                    Unsubscribe Model = new Unsubscribe
                    {
                        contractaccount = id,
                        success = true
                    };
                    if (!string.IsNullOrWhiteSpace(subscribe) && !subscribe.Equals("2"))
                    {
                        Model.Title = Translate.Text("Subscribe success title");
                        Model.description = Translate.Text("Subscribe success");
                        Model.verifysuccess = false;
                    }
                    else
                    {
                        Model.Title = Translate.Text("UnSubscribe success title");
                        Model.description = Translate.Text("UnSubscribe success");
                        Model.verifysuccess = true;
                        Model.subscribelink = HttpContext.Request.Url + "&subscribe=1";
                    }
                    return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/Verify.cshtml", Model);
                }
                errorModel.Title = Translate.Text("UnSubscribe failure title");
                errorModel.description = Translate.Text("UnSubscribe failure");
                errorModel.success = false;
                errorModel.verifysuccess = false;
            }
            return PartialView("~/Views/Feature/GeneralServices/ManageAlerts/Verify.cshtml", errorModel);
        }

        private ListDataSources GetReasons()
        {
            return this.ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.UNSUBSCRIBE_REASON));
        }

        private IEnumerable<SelectListItem> PopulateReasons()
        {
            var reasons = GetReasons().Items;
            var convertedItems = reasons.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
            return convertedItems;
        }
    }
}