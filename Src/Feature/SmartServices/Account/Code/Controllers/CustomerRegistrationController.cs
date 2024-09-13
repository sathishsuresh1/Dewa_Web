using DEWAXP.Feature.Account.Models.CustomerRegistration;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using Sitecore.Globalization;
using System;
using System.Web.Mvc;

namespace DEWAXP.Feature.Account.Controllers
{
    public class CustomerRegistrationController : BaseController
    {
        [AcceptVerbs("GET", "HEAD")]
        public ActionResult Register()
        {
            if (IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
            }
            var queryvalues = this.GetAccountValues("user");
            var businesspartnermodel = new BusinessPartnerNumberModel
            {
                BusinessPartnerNumber = queryvalues != null ? queryvalues.BusinessPartner : string.Empty
            };

            return View("~/Views/Feature/Account/CustomerRegistration/RegisterCustomer.cshtml", businesspartnermodel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(BusinessPartnerNumberModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = DewaApiClient.GetBusinessPartnerDetailsForRegistration(model.BusinessPartnerNumber, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        if (string.IsNullOrWhiteSpace(response.Payload.Mobile) && string.IsNullOrWhiteSpace(response.Payload.Email))
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_ERROR_VERIFICATION_METHOD);
                        }

                        var state = new RegistrationWorkflowState
                        {
                            BusinessPartnerNumber = response.Payload.BusinessPartnerNumber,
                            Email = response.Payload.Email,
                            Mobile = response.Payload.Mobile
                        };

                        if (!string.IsNullOrWhiteSpace(response.Payload.Email))
                        {
                            state.AvailableMethods |= VerificationMethods.Email;
                        }

                        if (!string.IsNullOrWhiteSpace(response.Payload.Mobile))
                        {
                            state.AvailableMethods |= VerificationMethods.Mobile;
                        }

                        CacheProvider.Store(CacheKeys.REGISTRATION_WORKFLOW_STATE, new CacheItem<RegistrationWorkflowState>(state));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CHOOSE_VERIFICATION_METHOD);
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            return View("~/Views/Feature/Account/CustomerRegistration/RegisterCustomer.cshtml", model);
        }

        [HttpGet]
        public ActionResult ChooseVerificationMethod()
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            return View("~/Views/Feature/Account/CustomerRegistration/ChooseVerificationMethod.cshtml", new ConfirmVerificationMethodModel
            {
                Email = state.Email,
                Mobile = state.Mobile,
                SelectedMethod = !string.IsNullOrWhiteSpace(state.Email) ? VerificationMethods.Email : VerificationMethods.Mobile
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChooseVerificationMethod(ConfirmVerificationMethodModel model)
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            state.SelectedMethod = model.SelectedMethod;
            if (state.EmailVerificationPreferred || state.MobileVerificationPreferred)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CONFIRM_VERIFICATION_METHOD);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_ERROR_VERIFICATION_METHOD);
        }

        [HttpGet]
        public ActionResult ErrorVerificationMethod()
        {
            return View("~/Views/Feature/Account/CustomerRegistration/ErrorVerificationMethod.cshtml");
        }

        [HttpGet]
        public ActionResult ConfirmVerificationMethod()
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            return View("~/Views/Feature/Account/CustomerRegistration/ConfirmVerificationMethod.cshtml", new ConfirmVerificationMethodModel
            {
                Email = state.Email,
                Mobile = state.Mobile,
                SelectedMethod = state.SelectedMethod
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConfirmVerificationMethod(ConfirmVerificationMethodModel model)
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            var verificationModel = new ConfirmVerificationMethodModel
            {
                Email = state.Email,
                Mobile = state.Mobile,
                SelectedMethod = state.SelectedMethod
            };

            try
            {
                var response = DewaApiClient.SendVerificationCodeForRegistration(state.BusinessPartnerNumber, state.SelectedMethod == VerificationMethods.Mobile, state.SelectedMethod == VerificationMethods.Email, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_VERIFY);
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/Account/CustomerRegistration/ConfirmVerificationMethod.cshtml", verificationModel);
        }

        [HttpGet]
        public ActionResult ResendVerification()
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            var verifyModel = new VerifyModel
            {
                Email = state.Email,
                Mobile = state.Mobile,
                SelectedMethod = state.SelectedMethod
            };

            try
            {
                var response = DewaApiClient.SendVerificationCodeForRegistration(state.BusinessPartnerNumber, state.SelectedMethod == VerificationMethods.Mobile, state.SelectedMethod == VerificationMethods.Email, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_VERIFY);
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/Account/CustomerRegistration/Verify.cshtml", verifyModel);
        }

        [HttpGet]
        public ActionResult Verify()
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            return View("~/Views/Feature/Account/CustomerRegistration/Verify.cshtml", new VerifyModel
            {
                Email = state.Email,
                Mobile = state.Mobile,
                SelectedMethod = state.SelectedMethod
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Verify(VerifyModel model)
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            var verifyModel = new VerifyModel
            {
                Email = state.Email,
                Mobile = state.Mobile,
                SelectedMethod = state.SelectedMethod
            };

            if (ModelState.IsValid)
            {
                try
                {
                    var response = DewaApiClient.ConfirmVerificationCodeForRegistration(state.BusinessPartnerNumber, state.SelectedMethod == VerificationMethods.Mobile, state.SelectedMethod == VerificationMethods.Email, model.VerificationCode, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        state.VerificationCode = model.VerificationCode;

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_SET_USERNAME_PASSWORD);
                    }
                    verifyModel.maxAttempt = response.Payload.Maxattempt;
                    if (verifyModel.maxAttempt != null && verifyModel.maxAttempt == "X")
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text(DictionaryKeys.Registration.InvalidVerificationCode));
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            return View("~/Views/Feature/Account/CustomerRegistration/Verify.cshtml", verifyModel);
        }

        [HttpGet]
        public ActionResult SetUsernameAndPassword()
        {
            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            var model = new SetUsernamePasswordModel
            {
                BusinessPartnerNumber = state.BusinessPartnerNumber,
                Email = state.Email,
                Mobile = state.Mobile
            };

            if (state.AvailableMethods == VerificationMethods.Both)
            {
                model.CanUpdateEmailAddress = state.SelectedMethod != VerificationMethods.Email;
                model.CanUpdateMobileNumber = state.SelectedMethod != VerificationMethods.Mobile;
            }
            else
            {
                model.CanUpdateMobileNumber = string.IsNullOrWhiteSpace(state.Mobile) && state.SelectedMethod != VerificationMethods.Mobile;
                model.CanUpdateEmailAddress = string.IsNullOrWhiteSpace(state.Email) && state.SelectedMethod != VerificationMethods.Email;
            }

            return View("~/Views/Feature/Account/CustomerRegistration/SetUsernameAndPassword.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetUsernameAndPassword(SetUsernamePasswordModel model)
        {
            CacheProvider.Store(CacheKeys.REGISTRATION_SUCCESSFUL_STATE, new CacheItem<bool>(false));
            string returnURL;

            RegistrationWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REGISTRATION_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
            }

            if (state.MobileVerificationPreferred)
            {
                ModelState.Remove("Mobile");
            }
            if (state.EmailVerificationPreferred)
            {
                ModelState.Remove("Email");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // We only submit contact details which have actually changed
                    var mobile = (!string.Equals(state.Mobile, model.Mobile) && !string.IsNullOrWhiteSpace(model.Mobile)) ? model.Mobile : null;
                    var email = (!string.Equals(state.Email, model.Email) && !string.IsNullOrWhiteSpace(model.Email)) ? model.Email : null;

                    var response = DewaApiClient.RegisterCustomer(state.BusinessPartnerNumber, mobile, email, model.Username, model.Password, model.ConfirmPassword, state.VerificationCode, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        state.Username = model.Username;
                        state.Password = model.Password;
                        state.ConfirmPassword = model.ConfirmPassword;

                        //var loginResponse = DewaApiClient.AuthenticateNew(model.Username, model.Password, language: RequestLanguage, segment: Request.Segment());
                        var loginResponse = SmartCustomerClient.LoginUser(
                            new LoginRequest
                            {
                                getloginsessionrequest = new Getloginsessionrequest
                                {
                                    userid = model.Username,
                                    password = Base64Encode(model.Password),
                                }
                            }, RequestLanguage, Request.Segment());
                        if (loginResponse.Succeeded && response.Payload != null)
                        {
                            AuthStateService.Save(new DewaProfile(model.Username, loginResponse.Payload.SessionNumber)
                            {
                                PrimaryAccount = loginResponse.Payload.AccountNumber,
                                EmailAddress = loginResponse.Payload.Email,
                                MobileNumber = loginResponse.Payload.Mobile,
                                FullName = loginResponse.Payload.FullName,
                                HasActiveAccounts = true,
                                TermsAndConditions = loginResponse.Payload.AcceptedTerms ? "X" : string.Empty,
                                IsFirstRegistration = !string.IsNullOrWhiteSpace(loginResponse.Payload.AccountNumber)
                            });
                        }
                        else if (!loginResponse.Succeeded && loginResponse.Payload != null && loginResponse.Payload.ResponseCode.Equals("116"))
                        {
                            ModelState.AddModelError("accountlock", loginResponse.Message);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, loginResponse.Message);
                        }

                        CacheProvider.Store(CacheKeys.REGISTRATION_SUCCESSFUL_STATE, new CacheItem<bool>(true));

                        if (CacheProvider.TryGet(CacheKeys.RegistrationReturnURL, out returnURL) && !string.IsNullOrEmpty(returnURL))
                        {
                            CacheProvider.Remove(CacheKeys.RegistrationReturnURL);
                            return RedirectToSitecoreItem(returnURL);
                        }
                        else
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J98_START);
                        }
                    }
                    ModelState.AddModelError(string.Empty, Translate.Text(response.Message));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            model.Email = state.Email;
            model.Mobile = state.Mobile;

            return View("~/Views/Feature/Account/CustomerRegistration/SetUsernameAndPassword.cshtml", model);
        }

        [Serializable]
        public class RegistrationWorkflowState
        {
            public string BusinessPartnerNumber { get; set; }

            public string Email { get; set; }

            public string Mobile { get; set; }

            public VerificationMethods SelectedMethod { get; set; }

            public VerificationMethods AvailableMethods { get; set; }

            public string VerificationCode { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }

            public string ConfirmPassword { get; set; }

            public bool EmailVerificationPreferred
            {
                get { return SelectedMethod == VerificationMethods.Email && !string.IsNullOrWhiteSpace(Email); }
            }

            public bool MobileVerificationPreferred
            {
                get { return SelectedMethod == VerificationMethods.Mobile && !string.IsNullOrWhiteSpace(Mobile); }
            }
        }
    }
}