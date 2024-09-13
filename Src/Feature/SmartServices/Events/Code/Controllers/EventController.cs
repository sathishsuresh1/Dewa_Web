using DEWAXP.Feature.Events.Models.Events;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.CustomDB.DataModel;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Logger;
using global::Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links.UrlBuilders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sitecorex = global::Sitecore;

namespace DEWAXP.Feature.Events.Controllers
{
    public class EventController : BaseController
    {
        /// <summary>
        /// Defines the _lock
        /// </summary>
        private static readonly object _lock = new object();

        private static string eventCodePrefix = "EVT";

        // GET: Event
        [HttpGet]
        public ActionResult Registration()
        {
            RegistrationModel model = new RegistrationModel();
            ListDataSources sessionDataSource = null;
            try
            {
                sessionDataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.AGILE_GOVERNANCE_GLOBAL_SUMMIT_SESSION_OPTIONS));
                if (sessionDataSource != null)
                {
                    model.SessionList = sessionDataSource.Items.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();
                    CacheProvider.Store(CacheKeys.AGILE_GOVERNANCE_SESSION, new CacheItem<List<SelectListItem>>(model.SessionList, TimeSpan.FromMinutes(40)));
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

            return View("~/Views/Feature/Events/Event/Registration.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Registration(RegistrationModel model)
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

                List<SelectListItem> sessionlist = new List<SelectListItem>();
                if (CacheProvider.TryGet(CacheKeys.AGILE_GOVERNANCE_SESSION, out sessionlist))
                {
                    model.SessionList = sessionlist;
                }
                if (status)
                {
                    if (ModelState.IsValid)
                    {
                        using (var context = new Entities())
                        {
                            var result = context.EventRegistrations.Where(x => x.Email.Equals(model.Email)).ToList();
                            if (result.Count < 1)
                            {
                                string eventCode = string.Format("{0}{1}{2}", eventCodePrefix, DateTime.Now.ToString("MMdd"), GetEventCode().ToString());
                                model.LinkUrl = GetEncryptedLinkExpiryURL(eventCode);
                                string strSessionName = string.Join(",", model.MultipleSessionName);

                                var response = context.SP_EventRegistration(model.FullName, model.CompanyName, model.Country, model.Designation, model.Mobile, model.Email, eventCode, strSessionName, model.Institution, model.LinkUrl);
                                if (response != null)
                                {
                                    var returnData = response.FirstOrDefault();
                                    if (returnData != null && returnData.Description == "True")
                                    {
                                        SendVerificationEmail(model);
                                        model.Message = Translate.Text("Event.SuccessfullyRegistered");
                                        CacheProvider.Store(CacheKeys.AGILE_GOVERNANCE_RESULT, new CacheItem<RegistrationModel>(model, TimeSpan.FromMinutes(40)));
                                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.AgileGovtGlobalSummitRegistrationSuccess);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, Translate.Text("Event_UserAlreadyRegistered."));
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Event_UnabledToSubmit"));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Event_UnsubscribeCaptchaNotValid"));
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
            return View("~/Views/Feature/Events/Event/Registration.cshtml",model);
        }

        public ActionResult Verify(string code)
        {
            RegistrationModel model = null;
            string EventCode = string.Empty;
            string errormessage = Translate.Text("Event_InvalidURL");

            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    code = HttpUtility.UrlDecode(code);
                    code = code.Trim().Replace(" ", "+");
                    if (GetDecryptedValues(code, out EventCode, out errormessage))
                    {
                        using (var context = new Entities())
                        {
                            var result = context.EventRegistrations.Where(x => x.LinkUrl.Equals(code)).SingleOrDefault();
                            if (!string.IsNullOrWhiteSpace(result.ID.ToString()))
                            {
                                DateTimeStyles styles;
                                DateTime dateResult;
                                string currentDate = string.Empty;
                                styles = DateTimeStyles.None;
                                bool IsVerified = false;
                                IsVerified = (bool)result.Verified;
                                if (DateTime.TryParse(DateTime.Now.ToLongDateString(), CultureInfo.InvariantCulture, styles, out dateResult))
                                    currentDate = dateResult.ToString("yyyy-MM-dd hh:mm:ss");
                                result.Verified = true;
                                result.UpdatedDate = DateTime.Now;
                                //result.UpdatedDate = DateTime.TryParse(currentDate);
                                context.SaveChanges();
                                model = new RegistrationModel
                                {
                                    FullName = result.FullName,
                                    CompanyName = result.CompanyName,
                                    Country = result.Country,
                                    Mobile = result.Mobile,
                                    Email = result.Email,
                                    Designation = result.Designation,
                                    Institution = result.Institution,
                                    LinkUrl = result.LinkUrl,
                                    EventCode = result.EventCode
                                };
                                if (IsVerified.Equals(false))
                                {
                                    SendConfirmationEmail(model);
                                    model.Message = Translate.Text("Event.SuccessfullyVerified");
                                }
                                else
                                {
                                    model.Message = Translate.Text("Event_AlreadyVerified");
                                }
                            }
                        }
                    }
                    else
                    {
                        model.Message = errormessage;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            CacheProvider.Store(CacheKeys.AGILE_GOVERNANCE_RESULT, new CacheItem<RegistrationModel>(model, TimeSpan.FromMinutes(40)));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.AgileGovtGlobalSummitRegistrationSuccess);
        }

        [HttpGet]
        public ActionResult Success()
        {
            RegistrationModel model = null;
            if (CacheProvider.TryGet(CacheKeys.AGILE_GOVERNANCE_RESULT, out model))
            {
                if (model != null)
                {
                    CacheProvider.Remove(CacheKeys.AGILE_GOVERNANCE_RESULT);
                    return View("~/Views/Feature/Events/Event/Success.cshtml",model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.AgileGovtGlobalSummit);
        }

        [HttpGet]
        public ActionResult ViewSession()
        {
            SessionModel model = new SessionModel();
            return View("~/Views/Feature/Events/Event/ViewSession.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ViewSession(SessionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.EventCode))
                    {
                        using (var context = new Entities())
                        {
                            var result = context.EventRegistrations.Where(x => x.Email.ToLower().Equals(model.Email.ToLower())).SingleOrDefault();
                            if (result != null && !string.IsNullOrWhiteSpace(result.ID.ToString()))
                            {
                                string eventCode = result.EventCode;
                                if (result.Attendance != null && (bool)result.Attendance)
                                {
                                    eventCode = string.Format("{0}{1}{2}", eventCodePrefix, DateTime.Now.ToString("MMdd"), GetEventCode().ToString());
                                    result.EventCode = eventCode;
                                    result.UpdatedDate = DateTime.Now;
                                    context.SaveChanges();
                                }

                                model.Email = result.Email;
                                model.IsValidEmail = true;
                                model.Message = Translate.Text("Event_SendEventCode");
                                SendEventCodeEmail(model, eventCode);
                            }
                            else
                            {
                                //ModelState.AddModelError(string.Empty, Translate.Text("Event_RegisteredEmail"));
                                model.IsValidEmail = true;
                                model.Message = Translate.Text("Event_SendEventCode");
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(model.EventCode) && !string.IsNullOrWhiteSpace(model.Email))
                    {
                        using (var context = new Entities())
                        {
                            var result = context.EventRegistrations.Where(x => x.EventCode.ToLower().Equals(model.EventCode.ToLower())).SingleOrDefault();
                            if (result != null && !string.IsNullOrWhiteSpace(result.ID.ToString()))
                            {
                                result.Attendance = true;
                                result.UpdatedDate = DateTime.Now;
                                context.SaveChanges();

                                model.Email = result.Email;
                                model.IsValidEmail = true;
                                model.IsValidCode = true;
                                model.Message = Translate.Text("Event_SuccessfullyVerified");
                            }
                            else
                            {
                                model.IsValidEmail = true;
                                ModelState.AddModelError(string.Empty, Translate.Text("Event_InvalidEventCode"));
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Event_UnabledToSubmit"));
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View("~/Views/Feature/Events/Event/ViewSession.cshtml",model);
        }

        /// <summary>
        /// Return a string of random hexadecimal values which is 6 characters long and relatively unique.
        /// </summary>
        /// <returns></returns>
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
            System.Text.UTF8Encoding UTF8 = new UTF8Encoding();
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

        private bool GetDecryptedValues(string url, out string eventcode, out string errormessage)
        {
            bool valid = false;
            eventcode = string.Empty;
            errormessage = Translate.Text("Please check the URL");
            try
            {
                string passphrase = "GlobalEvent";
                byte[] Results;
                UTF8Encoding UTF8 = new UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.Zeros
                };
                byte[] DataToDecrypt = Convert.FromBase64String(url);
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                var resulttext = UTF8.GetString(Results);
                string[] result = resulttext.Split('|');
                if (result.Length > 1 && !string.IsNullOrWhiteSpace(result[1]))
                {
                    if (DateTime.TryParse(result[1], out DateTime parsetime))
                    {
                        if (DateTime.Now.CompareTo(parsetime.AddDays(3)) < 0)
                        {
                            valid = true;
                            eventcode = result[0];
                        }
                        else
                        {
                            errormessage = Translate.Text("Link has been expired");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return valid;
        }

        private void SendVerificationEmail(RegistrationModel model)
        {
            Item VerificationEmail = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.AgileGovtGlobalSummit_VerifyEmail);
            string fromEmail = "no-reply@dewa.gov.ae";
            string toEmail = model.Email;
            string body = string.Empty;
            Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.AgileGovtGlobalSummit_Verify));
            string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always }) + "?code=" + HttpUtility.UrlEncode(model.LinkUrl);
            UniqueURL = UniqueURL.Replace(":443", "");
            if (VerificationEmail != null)
            {
                body = VerificationEmail["Rich Text"];
                body = body.Replace("{VerificationLink}", UniqueURL);
                EmailServiceClient.SendEmail(fromEmail, toEmail, VerificationEmail["Description"].ToString(), body);
            }
        }

        private void SendConfirmationEmail(RegistrationModel model)
        {
            Item VerificationEmail = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.AgileGovtGlobalSummit_ConfirmEmail);
            string fromEmail = "no-reply@dewa.gov.ae";
            string toEmail = model.Email;
            string body = string.Empty;

            if (VerificationEmail != null)
            {
                body = VerificationEmail["Rich Text"];
                EmailServiceClient.SendEmail(fromEmail, toEmail, VerificationEmail["Description"].ToString(), body);
            }
        }

        private void SendEventCodeEmail(SessionModel model, string eventCode)
        {
            Item EventCodeEmail = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.AgileGovtGlobalSummit_EventCodeEmail);
            string fromEmail = "no-reply@dewa.gov.ae";
            string toEmail = model.Email;
            string body = string.Empty;
            string subject = string.Empty;
            if (EventCodeEmail != null)
            {
                body = EventCodeEmail["Rich Text"];
                body = body.Replace("{EventCode}", eventCode);
                EmailServiceClient.SendEmail(fromEmail, toEmail, EventCodeEmail["Description"].ToString(), body);
            }
        }
    }
}