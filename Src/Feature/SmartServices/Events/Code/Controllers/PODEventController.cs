using DEWAXP.Feature.Events.Models.Events;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.CustomDB.DataModel;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Logger;
using global::Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Sitecorex = global::Sitecore;

namespace DEWAXP.Feature.Events.Controllers
{
    public class PODEventController : BaseController
    {
        /// <summary>
        /// Defines the _lock
        /// </summary>
        private static readonly object _lock = new object();

        private static string eventCodePrefix = "PODEVT";

        #region PODEventRegistration

        [HttpGet]
        public ActionResult PODEventRegistration()
        {
            PODRegistrationModel model = new PODRegistrationModel();
            try
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
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return View("~/Views/Feature/Events/PODEvent/PODEventRegistration.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PODEventRegistration(PODRegistrationModel model)
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
                //List<SelectListItem> sessionlist = new List<SelectListItem>();
                //if (CacheProvider.TryGet(CacheKeys.AGILE_GOVERNANCE_SESSION, out sessionlist))
                //{
                //    model.SessionList = sessionlist;
                //}
                if (status)
                {
                    if (ModelState.IsValid)
                    {
                        using (var context = new Entities())
                        {
                            var result = context.PODEventRegistrations.Where(x => x.Email.Equals(model.Email)).ToList();
                            if (result.Count < 1)
                            {
                                string eventCode = string.Format("{0}{1}{2}", eventCodePrefix, DateTime.Now.ToString("MMdd"), GetEventCode().ToString());
                                model.LinkUrl = GetEncryptedLinkExpiryURL(eventCode);
                                //string strSessionName = string.Join(",", model.MultipleSessionName);

                                var passCode = context.PODEventCodes.Where(x => x.IsSend == false).ToList().FirstOrDefault();
                                model.Passcode = passCode.Code;

                                var response = context.SP_PODEventRegistration(model.FullName, model.Email, model.CompanyName, model.Passcode, model.LinkUrl).ToList();
                                if (response != null)
                                {
                                    var returnData = response.FirstOrDefault();
                                    if (returnData != null && returnData.Description == "True")
                                    {
                                        passCode.IsSend = true;
                                        context.SaveChanges();

                                        SendVerificationEmail(model);
                                        model.Message = Translate.Text("POD_Event_SuccessfullyRegistered");
                                        CacheProvider.Store(CacheKeys.POD_EVENT_REGISTRATION_RESULT, new CacheItem<PODRegistrationModel>(model, TimeSpan.FromMinutes(40)));
                                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.PODEventRegistration_Success);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, Translate.Text("POD_Event_UserAlreadyRegistered"));
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("POD_Event_UnabledToSubmit"));
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
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            return View("~/Views/Feature/Events/PODEvent/PODEventRegistration.cshtml",model);
        }

        [HttpGet]
        public ActionResult Success()
        {
            PODRegistrationModel model = null;
            if (CacheProvider.TryGet(CacheKeys.POD_EVENT_REGISTRATION_RESULT, out model))
            {
                if (model != null)
                {
                    CacheProvider.Remove(CacheKeys.POD_EVENT_REGISTRATION_RESULT);
                    return View("~/Views/Feature/Events/PODEvent/Success.cshtml",model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.PODEventRegistration);
        }

        private string GetEventCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return al.GetRandomUppercaseAlphaNumericValue(8);
            }
        }

        private string GetEncryptedLinkExpiryURL(string eventcode)
        {
            string passphrase = "GlobalEvent";
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.Zeros;
            byte[] DataToEncrypt = UTF8.GetBytes(string.Concat(eventcode, "|", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            var newlinkurl = Convert.ToBase64String(Results);
            return newlinkurl;
        }

        private void SendVerificationEmail(PODRegistrationModel model)
        {
            Item VerificationEmail = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.PODEventRegistration_VerifyEmail);
            string fromEmail = "no-reply@dewa.gov.ae";
            string toEmail = model.Email;
            string body = string.Empty;
            //Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.AgileGovtGlobalSummit_Verify));
            //string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always }) + "?code=" + HttpUtility.UrlEncode(model.LinkUrl);
            //UniqueURL = UniqueURL.Replace(":443", "");
            if (VerificationEmail != null)
            {
                body = VerificationEmail["Rich Text"];
                body = body.Replace("{Passcode}", model.Passcode);
                //body = body.Replace("{VerificationLink}", UniqueURL);
                EmailServiceClient.SendEmail(fromEmail, toEmail, VerificationEmail["Description"].ToString(), body);
            }
        }

        #endregion PODEventRegistration
    }
}