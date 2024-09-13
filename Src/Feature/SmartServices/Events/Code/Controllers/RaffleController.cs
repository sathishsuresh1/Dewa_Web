using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.CustomDB.DataModel;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Controllers
{
    public class RaffleController : BaseController
    {
        // GET: Raffle/Create
        public ActionResult Create()
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
            return View("~/Views/Feature/Events/Raffle/Create.cshtml");
        }

        // POST: Raffle/Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(raffel model)
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
                            var result = context.raffels.Where(x => x.email.ToLower().Equals(model.email) || x.mobile.Equals(model.mobile)).ToList();
                            if (result.Count < 1)
                            {
                                var r = context.sp_insert_raffel(model.name, model.email, model.mobile);
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.RaffleSubmissionpage);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, Translate.Text("User already Present"));
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Unable to submit Raffle"));
                        return View(model);
                    }
                    // TODO: Add insert logic here
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
            return View("~/Views/Feature/Events/Raffle/Create.cshtml",model);
        }
    }
}