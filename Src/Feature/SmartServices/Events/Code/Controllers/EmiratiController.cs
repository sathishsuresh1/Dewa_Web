using DEWAXP.Feature.Events.Models.Emirati;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.CustomDB.DataModel;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Controllers
{
    public class EmiratiController : BaseController
    {
        public int MaxAllowed = 110;

        // GET: Emirati
        [HttpGet]
        public ActionResult Registration()
        {
            EmiratiRegModel model = new EmiratiRegModel();
            try
            {
                using (var context = new Entities())
                {
                    var result = context.SP_EmirateCheckOfflineAllow(MaxAllowed);
                    if (result != null)
                    {
                        var returnData = result.FirstOrDefault();
                        if (returnData != null && returnData.success == 0)
                        {
                            ViewBag.isVirtualEvent = true;
                            model.Entrytype = "Online";
                        }
                        else
                        {
                            ViewBag.isVirtualEvent = false;
                        }
                    }
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
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return View("~/Views/Feature/Events/Emirati/Registration.cshtml",model);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Registration(EmiratiRegModel model)
        {
            try
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
                    if (ModelState.IsValid)
                    {
                        using (var context = new Entities())
                        {
                            var result = context.EmiratiEventRegistrations.Where(x => x.emailid.ToLower().Equals(model.EmailAddress.ToLower()) || x.prno.ToLower().Equals(model.PrNo.ToLower())).ToList();
                            if (result.Count < 1)
                            {
                                var response = context.SP_EmirateEventRegistration(MaxAllowed, model.Name, model.EmailAddress, model.Designation, model.Entrytype, model.PrNo, model.Tc, model.IsVaccinated, model.Will_provide_rtpcr, null, null).ToList();
                                if (response != null)
                                {
                                    var returnData = response.FirstOrDefault();
                                    if (returnData != null && returnData.success == 1)
                                    {
                                        CacheProvider.Store(CacheKeys.POD_EVENT_REGISTRATION_RESULT, new CacheItem<EmiratiRegModel>(model, TimeSpan.FromMinutes(40)));
                                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.EW_SUCCESS_PAGE);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, Translate.Text("EW_Event_UserAlreadyRegistered"));
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("EW_Event_UnabledToSubmit"));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            using (var context = new Entities())
            {
                var result = context.SP_EmirateCheckOfflineAllow(MaxAllowed);
                if (result != null)
                {
                    var returnData = result.FirstOrDefault();
                    if (returnData != null && returnData.success == 0)
                    {
                        ViewBag.isVirtualEvent = true;
                        model.Entrytype = "Online";
                    }
                    else
                    {
                        ViewBag.isVirtualEvent = false;
                    }
                }
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
            return View("~/Views/Feature/Events/Emirati/Registration.cshtml",model);
        }

        // GET: Emirati
        [HttpGet]
        public ActionResult Event()
        {
            VerficationModel model = new VerficationModel();
            try
            {
                ViewBag.Succes = false;
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return View("~/Views/Feature/Events/Emirati/Event.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Event(VerficationModel model)
        {
            try
            {
                ViewBag.Succes = false;
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
                    if (ModelState.IsValid)
                    {
                        using (var context = new Entities())
                        {
                            var result = context.EmiratiEventRegistrations.Where(x => x.emailid.ToLower() == model.EmailAddress.ToLower()).ToList();
                            if (result.Count == 1)
                            {
                                ViewBag.Succes = true;
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, Translate.Text("EW_Event_UserNotAvailable"));
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("EW_Event_UnabledToSubmit"));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
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
            return View("~/Views/Feature/Events/Emirati/Event.cshtml", model);
        }
    }
}