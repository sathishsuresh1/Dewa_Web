// <copyright file="AccountRecoveryController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Account.Controllers
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory;
    using global::Sitecore.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Feature.Account.Models;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Content.Models.AccountModel;
    using System;

    /// <summary>
    /// Defines the <see cref="AccountRecoveryController" />.
    /// </summary>
    public class AccountRecoveryController : BaseController
    {
        /// <summary>
        /// The ForgotPassword.
        /// </summary>
        /// <param name="s">The s<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ForgotPassword(string s)
        {
            ViewBag.Title = Translate.Text("Forgot password");
            return GetAccountRecovery(s);
        }

        /// <summary>
        /// The Accountunlock.
        /// </summary>
        /// <param name="s">The s<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Accountunlock(string s)
        {
            ViewBag.Title = Translate.Text("Account Unlock");
            ViewBag.accountunlock = true;
            return GetAccountRecovery(s, true);
        }

        /// <summary>
        /// The GetAccountRecovery.
        /// </summary>
        /// <param name="s">The s<see cref="string"/>.</param>
        /// <param name="unlock">The unlock<see cref="bool"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        private ActionResult GetAccountRecovery(string s, bool unlock = false)
        {
            //bool accountunlock = false;
            ViewBag.BackLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out string errormessage))
            {
                ModelState.AddModelError(string.Empty, errormessage);
            }
            if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Step + (unlock ? "unlock" : string.Empty), out string steps) || !string.IsNullOrWhiteSpace(s))
            {
                if (!string.IsNullOrWhiteSpace(s) && s.Equals("1"))
                {
                    steps = s;
                }

                switch (steps)
                {
                    case "1":
                        VerifyEmailandMobileModel selectedModel;
                        if (CacheProvider.TryGet(CacheKeys.ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), out selectedModel))
                        {
                            ViewBag.BackLink = unlock ? LinkHelper.GetItemUrl(SitecoreItemIdentifiers.ACCOUNT_UNLOCK) : LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_FORGOT_PASSWORD);
                            return View("~/Views/Feature/Account/AccountRecovery/_VerifyEmailandMobile.cshtml", selectedModel);
                        }
                        break;
                    case "2":
                        ForgotPasswordViewModel forgotPasswordViewModel;
                        if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), out forgotPasswordViewModel))
                        {
                            CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                            ViewBag.Email = forgotPasswordViewModel.EmailAddess;
                            ViewBag.subtitleText = forgotPasswordViewModel.SelectedOption.Equals("1") ?
                               string.Format(Translate.Text("Verifyemail.SubTitle"), forgotPasswordViewModel.MaskedEmailAddess) :
                               string.Format(Translate.Text("Verifymobile.SubTitle"), forgotPasswordViewModel.MaskedMobile);
                            ViewBag.BackLink = unlock ? LinkHelper.GetItemUrl(SitecoreItemIdentifiers.ACCOUNT_UNLOCK) + "?s=1" : LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_FORGOT_PASSWORD) + "?s=1";

                            return View("~/Views/Feature/Account/AccountRecovery/_VerifyOTP.cshtml");
                        }
                        break;
                    case "3":
                        //ForgotPasswordViewModel forgotPasswordViewModel;
                        if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), out forgotPasswordViewModel))
                        {
                            if (!unlock)
                            {
                                CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                ViewBag.BackLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_FORGOT_PASSWORD);
                                return View("~/Views/Feature/Account/AccountRecovery/_SetNewPassword.cshtml", new SetNewPasswordV1Model { Username = forgotPasswordViewModel.Username });
                            }
                        }
                        break;
                    case "4":
                        CacheProvider.Remove(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty));
                        if (!unlock)
                        {
                            ViewBag.SuccessTitle = Translate.Text("AR.Password_reset_successful");
                            ViewBag.Subtitle = Translate.Text("AR.Password_reset_successful_Success");
                        }
                        else
                        {
                            ViewBag.SuccessTitle = Translate.Text("AR.Account_unlocked");
                            ViewBag.Subtitle = Translate.Text("AR.Account_unlocked_Success");
                        }
                        return View("~/Views/Feature/Account/AccountRecovery/_SetNewPasswordSuccess.cshtml");
                    case "5":
                        string username;
                        if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Username, out username))
                        {
                            CacheProvider.Store(CacheKeys.ForgotPassword_Username, new AccessCountingCacheItem<string>(username, Times.Once));
                            SendOTPMethod(new ForgotPasswordV1Model { Username=username}, true, out bool success);
                            if(success)
                            {
                                VerifyEmailandMobileModel selectedModel1;
                                if (CacheProvider.TryGet(CacheKeys.ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), out selectedModel1))
                                {
                                    return View("~/Views/Feature/Account/AccountRecovery/_VerifyEmailandMobile.cshtml", selectedModel1);
                                }
                            }
                        }
                        break;
                }
            }
            if(unlock)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
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
            return View("~/Views/Feature/Account/AccountRecovery/_ForgotPassword.cshtml", new ForgotPasswordV1Model());
        }

        /// <summary>
        /// The ForgotPasswordSubmit.
        /// </summary>
        /// <param name="model">The model<see cref="ForgotPasswordV1Model"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordSubmit(ForgotPasswordV1Model model)
        {
            return ForgotPasswordPost(model);
        }

        /// <summary>
        /// The AccountUnlockSubmit.
        /// </summary>
        /// <param name="model">The model<see cref="ForgotPasswordV1Model"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AccountUnlockSubmit(ForgotPasswordV1Model model)
        {
            return ForgotPasswordPost(model, true);
        }

        /// <summary>
        /// The ForgotPasswordPost.
        /// </summary>
        /// <param name="model">The model<see cref="ForgotPasswordV1Model"/>.</param>
        /// <param name="unlock">The unlock<see cref="bool"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        private ActionResult ForgotPasswordPost(ForgotPasswordV1Model model, bool unlock = false)
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
                    try
                    {
                        SendOTPMethod(model, unlock,out bool success);
                    }
                    catch (System.Exception ex)
                    {
                        LogService.Error(ex, this);
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                    }
                }
            }

            if (unlock)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.ACCOUNT_UNLOCK);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_FORGOT_PASSWORD);
        }

        private void SendOTPMethod(ForgotPasswordV1Model model, bool unlock, out bool success)
        {
            success = false;
            RefundHistoryVerifyOtpRequest verifyRequest = new RefundHistoryVerifyOtpRequest()
            {
                mode = "R",
                lang = RequestLanguageCode,
                sessionid = string.Empty,
                reference = model.Username,
                prtype = "USID",
            };

            Foundation.Integration.Responses.ServiceResponse<Foundation.Integration.APIHandler.Models.Response.RefundHistory.RefundHistoryResponse> returnData = RefundHistoryClient.VerifyOtp(verifyRequest);
            bool IsSuccessful = returnData != null && returnData.Succeeded && returnData.Payload != null &&
            (System.Convert.ToInt32(returnData.Payload.emaillist?.Count ?? 0) > 0 || System.Convert.ToInt32(returnData.Payload.mobilelist?.Count ?? 0) > 0);
            if (IsSuccessful)
            {
                VerifyEmailandMobileModel verifyEmailandMobileModel = new VerifyEmailandMobileModel { Username = model.Username };
                ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel
                {
                    Businesspartnernumber = returnData.Payload.businesspartnernumber,
                    Username = model.Username
                };
                Foundation.Integration.APIHandler.Models.Response.RefundHistory.RefundHistoryEmailResponse email = returnData.Payload.emaillist?.FirstOrDefault() ?? null;
                Foundation.Integration.APIHandler.Models.Response.RefundHistory.RefundHistoryMobileResponse mobile = returnData.Payload.mobilelist?.FirstOrDefault() ?? null;

                if (email != null && !string.IsNullOrWhiteSpace(email.unmaskedemail))
                {
                    forgotPasswordViewModel.EmailAddess = email.unmaskedemail;
                    forgotPasswordViewModel.MaskedEmailAddess = email.maskedemail;
                    verifyEmailandMobileModel.EmailAddess = email.maskedemail;
                }
                if (mobile != null && !string.IsNullOrWhiteSpace(mobile.unmaskedmobile))
                {
                    forgotPasswordViewModel.Mobile = mobile.unmaskedmobile;
                    forgotPasswordViewModel.MaskedMobile = mobile.maskedmobile;
                    verifyEmailandMobileModel.Mobile = mobile.maskedmobile;
                }
                success = true;
                CacheProvider.Store(CacheKeys.ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<VerifyEmailandMobileModel>(verifyEmailandMobileModel, Times.Once));
                CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                CacheProvider.Store(CacheKeys.ForgotPassword_Step + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<string>("1", Times.Once));
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
            }
        }

        /// <summary>
        /// The ForgotPasswordVerifyOTP.
        /// </summary>
        /// <param name="model">The model<see cref="VerifyEmailandMobileModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordVerifyOTP(VerifyEmailandMobileModel model)
        {
            return VerifyOTP(model);
        }

        /// <summary>
        /// The AccountUnlockVerifyOTP.
        /// </summary>
        /// <param name="model">The model<see cref="VerifyEmailandMobileModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AccountUnlockVerifyOTP(VerifyEmailandMobileModel model)
        {
            return VerifyOTP(model, true);
        }

        /// <summary>
        /// The VerifyOTP.
        /// </summary>
        /// <param name="model">The model<see cref="VerifyEmailandMobileModel"/>.</param>
        /// <param name="unlock">The unlock<see cref="bool"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        private ActionResult VerifyOTP(VerifyEmailandMobileModel model, bool unlock = false)
        {
            try
            {
                if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), out ForgotPasswordViewModel forgotPasswordViewModel))
                {
                    if (forgotPasswordViewModel != null && model != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) &&
                        !string.IsNullOrWhiteSpace(model.Username) && forgotPasswordViewModel.Username.Equals(model.Username)
                        && !string.IsNullOrWhiteSpace(model.SelectedOption))
                    {
                        RefundHistoryVerifyOtpRequest verifyRequest = new RefundHistoryVerifyOtpRequest()
                        {
                            mode = "S",
                            lang = RequestLanguageCode,
                            sessionid = string.Empty,
                            reference = forgotPasswordViewModel.Username,
                            prtype = "USID",
                            email = model.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                            mobile = model.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                            businesspartner = forgotPasswordViewModel.Businesspartnernumber
                        };

                        Foundation.Integration.Responses.ServiceResponse<Foundation.Integration.APIHandler.Models.Response.RefundHistory.RefundHistoryResponse> returnData = RefundHistoryClient.VerifyOtp(verifyRequest);
                        bool IsSuccessful = returnData != null && returnData.Succeeded;
                        if (IsSuccessful)
                        {
                            forgotPasswordViewModel.SelectedOption = model.SelectedOption;
                            CacheProvider.Store(CacheKeys.ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                            {
                                EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                Mobile = forgotPasswordViewModel.MaskedMobile,
                                Username = forgotPasswordViewModel.Username,
                                SelectedOption = forgotPasswordViewModel.SelectedOption
                            }, Times.Once));
                            CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                            CacheProvider.Store(CacheKeys.ForgotPassword_Step + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<string>("2", Times.Once));
                        }
                        else
                        {
                            if (!unlock)
                            {
                                CacheProvider.Remove(CacheKeys.ForgotPassword_OTP + (unlock ? "unlock" : string.Empty));
                            }
                            else
                            {
                                CacheProvider.Store(CacheKeys.ForgotPassword_Step + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<string>("5", Times.Once));
                            }
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
            }
            if (unlock)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.ACCOUNT_UNLOCK);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_FORGOT_PASSWORD);
        }

        /// <summary>
        /// The ForgotPasswordSubmitOTP.
        /// </summary>
        /// <param name="otp">The otp<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordSubmitOTP(string otp)
        {
            if (!string.IsNullOrWhiteSpace(otp))
            {
                try
                {
                    if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata, out ForgotPasswordViewModel forgotPasswordViewModel))
                    {
                        if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.SelectedOption))
                        {
                            RefundHistoryVerifyOtpRequest verifyRequest = new RefundHistoryVerifyOtpRequest()
                            {
                                mode = "V",
                                lang = RequestLanguageCode,
                                sessionid = string.Empty,
                                reference = forgotPasswordViewModel.Username,
                                prtype = "USID",
                                email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                                mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                                businesspartner = forgotPasswordViewModel.Businesspartnernumber,
                                otp = otp
                            };

                            Foundation.Integration.Responses.ServiceResponse<Foundation.Integration.APIHandler.Models.Response.RefundHistory.RefundHistoryResponse> returnData = RefundHistoryClient.VerifyOtp(verifyRequest);
                            bool IsSuccessful = returnData != null && returnData.Succeeded && ((string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)) ||
                                (!string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)
                                && !returnData.Payload.maxattempts.Equals("X")));
                            if (IsSuccessful)
                            {
                                forgotPasswordViewModel.OTP = otp;
                                CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata, new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                CacheProvider.Store(CacheKeys.ForgotPassword_Step, new AccessCountingCacheItem<string>("3", Times.Once));
                            }
                            else if (returnData != null && !returnData.Succeeded && returnData.Payload != null && !string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)
                                && returnData.Payload.maxattempts.Equals("X"))
                            {
                                CacheProvider.Remove(CacheKeys.ForgotPassword_OTP);
                                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                            }
                            else
                            {
                                CacheProvider.Store(CacheKeys.ForgotPassword_OTP, new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                                {
                                    EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                    Mobile = forgotPasswordViewModel.MaskedMobile,
                                    Username = forgotPasswordViewModel.Username,
                                    SelectedOption = forgotPasswordViewModel.SelectedOption
                                }, Times.Once));
                                CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata, new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                CacheProvider.Store(CacheKeys.ForgotPassword_Step, new AccessCountingCacheItem<string>("2", Times.Once));
                                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_FORGOT_PASSWORD);
        }

        /// <summary>
        /// The AccountUnlockSubmitOTP.
        /// </summary>
        /// <param name="otp">The otp<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AccountUnlockSubmitOTP(string otp)
        {
            if (!string.IsNullOrWhiteSpace(otp))
            {
                try
                {
                    if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata + "unlock", out ForgotPasswordViewModel forgotPasswordViewModel))
                    {
                        if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.SelectedOption))
                        {
                            RefundHistoryVerifyOtpRequest verifyRequest = new RefundHistoryVerifyOtpRequest()
                            {
                                mode = "V",
                                lang = RequestLanguageCode,
                                sessionid = string.Empty,
                                reference = forgotPasswordViewModel.Username,
                                prtype = "USID",
                                email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                                mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                                businesspartner = forgotPasswordViewModel.Businesspartnernumber,
                                otp = otp
                            };

                            Foundation.Integration.Responses.ServiceResponse<Foundation.Integration.APIHandler.Models.Response.RefundHistory.RefundHistoryResponse> returnData = RefundHistoryClient.VerifyOtp(verifyRequest);
                            bool IsSuccessful = returnData != null && returnData.Succeeded && ((string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)) ||
                                (!string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)
                                && !returnData.Payload.maxattempts.Equals("X")));
                            if (IsSuccessful)
                            {
                                UnlockAccountRequest forgotPasswordRequest = new UnlockAccountRequest
                                {
                                    passwordinput = new unlockaccountinput()
                                    {
                                        lang = RequestLanguageCode,
                                        sessionid = string.Empty,
                                        email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                                        mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                                        businesspartner = forgotPasswordViewModel.Businesspartnernumber,
                                        otp = otp,
                                        userid = forgotPasswordViewModel.Username,
                                        password = string.Empty,
                                        confirmpassword = string.Empty
                                    }
                                };

                                Foundation.Integration.Responses.ServiceResponse<Foundation.Integration.APIHandler.Models.Response.Common.CommonResponse> response = SmartCustomerClient.UnlockAccount(forgotPasswordRequest);
                                if (response != null && response.Succeeded)
                                {
                                    CacheProvider.Store(CacheKeys.ForgotPassword_Step + "unlock", new AccessCountingCacheItem<string>("4", Times.Once));
                                }
                                else
                                {
                                    CacheProvider.Store(CacheKeys.ForgotPassword_OTP + "unlock", new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                                    {
                                        EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                        Mobile = forgotPasswordViewModel.MaskedMobile,
                                        Username = forgotPasswordViewModel.Username,
                                        SelectedOption = forgotPasswordViewModel.SelectedOption
                                    }, Times.Once));
                                    CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata + "unlock", new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                    CacheProvider.Store(CacheKeys.ForgotPassword_Step + "unlock", new AccessCountingCacheItem<string>("2", Times.Once));
                                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                                }
                            }
                            else if (returnData != null && !returnData.Succeeded && returnData.Payload != null && !string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)
                                && returnData.Payload.maxattempts.Equals("X"))
                            {
                                CacheProvider.Remove(CacheKeys.ForgotPassword_OTP);
                                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                            }
                            else
                            {
                                CacheProvider.Store(CacheKeys.ForgotPassword_OTP + "unlock", new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                                {
                                    EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                    Mobile = forgotPasswordViewModel.MaskedMobile,
                                    Username = forgotPasswordViewModel.Username,
                                    SelectedOption = forgotPasswordViewModel.SelectedOption
                                }, Times.Once));
                                CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata + "unlock", new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                CacheProvider.Store(CacheKeys.ForgotPassword_Step + "unlock", new AccessCountingCacheItem<string>("2", Times.Once));
                                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ACCOUNT_UNLOCK);
        }

        /// <summary>
        /// The SetnewPasswordSubmit.
        /// </summary>
        /// <param name="model">The model<see cref="SetNewPasswordV1Model"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetnewPasswordSubmit(SetNewPasswordV1Model model)
        {
            try
            {
                if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata, out ForgotPasswordViewModel forgotPasswordViewModel))
                {
                    if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.SelectedOption))
                    {
                        ForgotPasswordRequest forgotPasswordRequest = new ForgotPasswordRequest
                        {
                            passwordinput = new passwordinput()
                            {
                                mode = "V",
                                lang = RequestLanguageCode,
                                sessionid = string.Empty,
                                reference = forgotPasswordViewModel.Username,
                                prtype = "USID",
                                email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                                mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                                businesspartner = forgotPasswordViewModel.Businesspartnernumber,
                                otp = forgotPasswordViewModel.OTP,
                                userid = forgotPasswordViewModel.Username,
                                password = Base64Encode(model.Password),
                                confirmpassword = Base64Encode(model.ConfirmPassword)
                            }
                        };

                        Foundation.Integration.Responses.ServiceResponse<Foundation.Integration.APIHandler.Models.Response.Common.CommonResponse> returnData = SmartCustomerClient.ForgotUseridPwd(forgotPasswordRequest);
                        bool IsSuccessful = returnData != null && returnData.Succeeded;
                        if (IsSuccessful)
                        {
                            CacheProvider.Store(CacheKeys.ForgotPassword_Step, new AccessCountingCacheItem<string>("4", Times.Once));
                        }
                        else
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_FORGOT_PASSWORD);
        }

        /// <summary>
        /// The ForgotPasswordResendOTP.
        /// </summary>
        /// <returns>The <see cref="JsonResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ForgotPasswordResendOTP()
        {
            string error = Translate.Text("Select the value");
            try
            {
                if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata + "unlock", out ForgotPasswordViewModel forgotPasswordViewModel))
                {
                    CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata + "unlock", new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                }
                else if (CacheProvider.TryGet(CacheKeys.ForgotPassword_Saveddata, out forgotPasswordViewModel))
                {
                    CacheProvider.Store(CacheKeys.ForgotPassword_Saveddata, new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));

                }
                if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username))
                {
                    RefundHistoryVerifyOtpRequest verifyRequest = new RefundHistoryVerifyOtpRequest()
                    {
                        mode = "S",
                        lang = RequestLanguageCode,
                        sessionid = string.Empty,
                        reference = forgotPasswordViewModel.Username,
                        prtype = "USID",
                        email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                        mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                        businesspartner = forgotPasswordViewModel.Businesspartnernumber
                    };

                    Foundation.Integration.Responses.ServiceResponse<Foundation.Integration.APIHandler.Models.Response.RefundHistory.RefundHistoryResponse> returnData = RefundHistoryClient.VerifyOtp(verifyRequest);
                    bool IsSuccessful = returnData != null && returnData.Succeeded;
                    if (IsSuccessful)
                    {
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                        error = returnData.Message;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                error = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, Error = error }, JsonRequestBehavior.AllowGet);
        }
    }
}
