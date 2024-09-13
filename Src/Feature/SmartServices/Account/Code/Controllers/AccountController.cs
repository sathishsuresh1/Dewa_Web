using DEWAXP.Feature.Account.Filters;
using DEWAXP.Feature.Account.Models;
using DEWAXP.Feature.Account.Models.CustomerUpdate;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.AccountModel;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.RammasLogin;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Newtonsoft.Json;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;

//using Sitecore.Security.Accounts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using X.PagedList;
using dewaSVC = DEWAXP.Foundation.Integration.DewaSvc;
using Roles = DEWAXP.Foundation.Content.Roles;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.Account.Controllers
{
    public class AccountController : BaseController
    {
        //private AccountController _contentRepository;
        //private ICacheProvider CacheProvider;
        //private DewaProfile CurrentPrincipal;
        //private IDewaAuthStateService AuthStateService;

        //public AccountController(ICacheProvider cacheProvider,
        //    DewaProfile dewaProfile,
        //    IDewaAuthStateService dewaAuthStateService)
        //{
        //    CacheProvider = cacheProvider;
        //    CurrentPrincipal = dewaProfile;
        //    AuthStateService = dewaAuthStateService;
        //}
        [HttpPost, AntiForgeryHandleError, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error, lockerror;
                    if (TryLogin(model, out error, out lockerror))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                    }
                    if (!string.IsNullOrWhiteSpace(lockerror))
                    {
                        CacheProvider.Store(CacheKeys.ACCOUNTLOCK_ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                    }
                }
                catch (System.Exception)
                {
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult LoginMain(string returnUrl)
        {
            LoginModel model = new LoginModel();
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
            }

            string error, lockerror;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            if (CacheProvider.TryGet(CacheKeys.ACCOUNTLOCK_ERROR_MESSAGE, out lockerror))
            {
                ModelState.AddModelError("accountlock", lockerror);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                ClearSessionAndSignOut();
            }
            string userid;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_USERID, out userid))
            {
                model.Username = userid;
                ViewBag.movein = true;
            }

            ViewBag.ReturnUrl = returnUrl;

            return PartialView("~/Views/Feature/Account/Forms/_LoginFormMain.cshtml", model);
        }

        [HttpPost, AntiForgeryHandleError, ValidateAntiForgeryToken]
        public ActionResult LoginMain(LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error, lockerror;
                    if (TryLogin(model, out error, out lockerror))
                    {
                        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                    }
                    if (!string.IsNullOrWhiteSpace(lockerror))
                    {
                        ModelState.AddModelError("accountlock", lockerror);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("~/Views/Feature/Account/Forms/_LoginFormMain.cshtml", model);
        }

        [HttpGet]
        public ActionResult GovernmentLogin(string returnUrl)
        {
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.Government))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J91_GOVT_OBS_REQUEST);
            }
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("~/Views/Feature/Account/Forms/_GovernmentLoginForm.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GovernmentLogin(LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error;
                    if (TryLoginAsGovernmentUser(model, out error))
                    {
                        if (!string.IsNullOrWhiteSpace(returnUrl))
                        {
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J91_GOVT_OBS_REQUEST);
                            }
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J91_GOVT_OBS_REQUEST);
                    }

                    ModelState.AddModelError(string.Empty, error);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("~/Views/Feature/Account/Forms/_GovernmentLoginForm.cshtml", model);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            if (IsLoggedIn)
            {
                bool myid = CurrentPrincipal.IsMyIdUser, governmental = CurrentPrincipal.Role == Roles.Government, academy = CurrentPrincipal.Role == Roles.DewaAcademy;
                string username = CurrentPrincipal.Username, samlSessionIndex = myid ? (Session["SamlSessionIndex"] != null ? Session["SamlSessionIndex"].ToString() : string.Empty) : string.Empty;

                ClearSessionAndSignOut();

                if (!string.IsNullOrWhiteSpace(samlSessionIndex) && CurrentPrincipal.IsMyIdUser)
                {
                    //SendMyIdLogoutRequest(username, samlSessionIndex);
                    //Fixed for MyID Logout
                    //return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                    return new EmptyResult();
                }
                else if (CurrentPrincipal.IsMyIdUser)
                {
                    if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["UAEPASS_Logout"]))
                    {
                        var link = string.Format(WebConfigurationManager.AppSettings["UAEPASS_Logout"], LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE, false));
                        if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                        {
                            Response.Redirect(link);
                        }
                    }
                }

                if (governmental)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J8_GOVT_LOGIN_PAGE);
                }
                if (academy)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEWAACADEMY_LOGIN);
                }

                if (CurrentPrincipal.Role == Roles.ScrapeSale)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
        }

        [HttpPost, AntiForgeryHandleError, ValidateAntiForgeryToken]
        public ActionResult ExpoLogin(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error, lockerror;
                    if (TryLogin(model, out error, out lockerror))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                    }
                    if (!string.IsNullOrWhiteSpace(lockerror))
                    {
                        CacheProvider.Store(CacheKeys.ACCOUNTLOCK_ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                    }
                }
                catch (System.Exception)
                {
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EXPO_LOGIN_PAGE);
        }

        [HttpGet]
        public ActionResult ExpoLogin(string returnUrl)
        {
            LoginModel model = new LoginModel();
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
            }

            string error, lockerror;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            if (CacheProvider.TryGet(CacheKeys.ACCOUNTLOCK_ERROR_MESSAGE, out lockerror))
            {
                ModelState.AddModelError("accountlock", lockerror);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                ClearSessionAndSignOut();
            }

            string userid;
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_USERID, out userid))
            {
                model.Username = userid;
                ViewBag.movein = true;
            }
            //This next line is added so that on each login the auth cookie is recreated,this change is done to fix the issue # WEBR-217 (Defect 74)
            //ClearSessionAndSignOut();
            ViewBag.ReturnUrl = returnUrl;
            ClearAccountCache();
            return PartialView("~/Views/Feature/Account/Forms/_LoginExpoForm.cshtml", model);
        }

        public void LogoutUser(LoginModel loginmodel)
        {
            //CacheProvider.Store(CacheKeys.FIX_LOGINMODEL, new CacheItem<LoginModel>(loginmodel));
            if (IsLoggedIn)
            {
                bool myid = CurrentPrincipal.IsMyIdUser, governmental = CurrentPrincipal.Role == Roles.Government;
                string username = CurrentPrincipal.Username, samlSessionIndex = myid ? (Session["SamlSessionIndex"] != null ? Session["SamlSessionIndex"].ToString() : string.Empty) : string.Empty;

                ClearSessionAndSignOut();

                if (!string.IsNullOrWhiteSpace(samlSessionIndex) && CurrentPrincipal.IsMyIdUser)
                {
                    //SendMyIdLogoutRequest(username, samlSessionIndex);
                }
                else if (CurrentPrincipal.IsMyIdUser)
                {
                    if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["UAEPASS_Logout"]))
                    {
                        if (Uri.IsWellFormedUriString(WebConfigurationManager.AppSettings["UAEPASS_Logout"].ToString(), UriKind.Absolute))
                        {
                            Response.Redirect(WebConfigurationManager.AppSettings["UAEPASS_Logout"].ToString());
                        }
                    }
                }
            }
        }

        //private void SendMyIdLogoutRequest(string username, string samlSessionIndex)
        //{
        //    var logoutRequest = new LogoutRequest
        //    {
        //        //issuer name should be match with wso2 defination
        //        Issuer = new Issuer(MyIdConfig.IssuerUrl),
        //        NameID = new NameID(username)
        //    };

        //    logoutRequest.SessionIndexes.Add(new SessionIndex(samlSessionIndex));

        //    //Serialize the logout request to XML for transmission.
        //    var logoutRequestXml = logoutRequest.ToXml();

        //    //Send the logout request to the IdP over HTTP redirect.
        //    var logoutRedirect = MyIdConfig.LogoutUrl;

        //    var x509Certificate = (X509Certificate2)HttpContext.ApplicationInstance.Application[MyIdConfig.ServiceProviderCertKey];

        //    SingleLogoutService.SendLogoutRequestByHTTPRedirect(Response, logoutRedirect, logoutRequestXml, null, x509Certificate.PrivateKey);
        //}

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return PartialView("~/Views/Feature/Account/Forms/_ForgotPassword.cshtml", new ForgotPasswordModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = DewaApiClient.ResetPassword(model.Username, model.Email, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        var recoveryEmailModel = new RecoveryEmailSentModel
                        {
                            EmailAddress = model.Email,
                            Context = RecoveryContext.Password
                        };

                        CacheProvider.Store(CacheKeys.RECOVERY_EMAIL_STATE, new CacheItem<RecoveryEmailSentModel>(recoveryEmailModel));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_RECOVERY_EMAIL_SENT);
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            return PartialView("~/Views/Feature/Account/Forms/_ForgotPassword.cshtml", model);
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult ForgotUsername()
        {
            return PartialView("~/Views/Feature/Account/Forms/_ForgotUserName.cshtml", new ForgotUsernameModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotUsername(ForgotUsernameModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = DewaApiClient.RequestUserIdentifierReminder(model.BusinessPartnerNumber, model.Email, RequestLanguage, Request.Segment());

                    if (response.Succeeded)
                    {
                        var recoveryEmailModel = new RecoveryEmailSentModel
                        {
                            EmailAddress = model.Email,
                            Context = RecoveryContext.Username
                        };

                        CacheProvider.Store(CacheKeys.RECOVERY_EMAIL_STATE, new CacheItem<RecoveryEmailSentModel>(recoveryEmailModel));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_RECOVERY_EMAIL_SENT);
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            ViewBag.Modal = ContentRepository.GetItem<ModalOverlay>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{6DFBF80F-1B61-4332-9E35-EB5860434E6B}")));

            return PartialView("~/Views/Feature/Account/Forms/_ForgotUserName.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult ChangePassword()
        {
            //if (CurrentPrincipal.IsMyIdUser)
            //{
            //    ViewBag.ChangePassword = "invalid";
            //    //return RedirectToSitecoreItem(SitecoreItemIdentifiers.J60_CANNOT_CHANGE_PASSWORD);
            //}

            //return View("~/Views/Feature/Account/Forms/_ChangePassword.cshtml", new ChangePasswordModel
            //{
            //    UserId = CurrentPrincipal.Username
            //});

            if (CurrentPrincipal.IsMyIdUser)
            {
                ViewBag.ChangePassword = "invalid";
            }
            else
            {
                string errorMessage;
                string CHANGE_PWD;
                if (CacheProvider.TryGet(CacheKeys.PROFILE_CHANGE_PWD_ERROR, out errorMessage))
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    ViewBag.ChangePassword = null;
                }
                else if (CacheProvider.TryGet(CacheKeys.PROFILE_CHANGE_PWD, out CHANGE_PWD))
                {
                    ViewBag.ChangePassword = CHANGE_PWD;
                }
                else
                {
                    ViewBag.success = null;
                }

                CacheProvider.Remove(CacheKeys.PROFILE_CHANGE_PWD);
                CacheProvider.Remove(CacheKeys.PROFILE_CHANGE_PWD_ERROR);
            }
            return View("~/Views/Feature/Account/Forms/_ChangePassword.cshtml", new ChangePasswordModel
            {
                UserId = CurrentPrincipal.Username
            });
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            ViewBag.ChangePassword = null;
            if (ModelState.IsValid)
            {
                try
                {
                    if (String.CompareOrdinal(model.OldPassword, model.NewPassword) == 0)
                    {
                        CacheProvider.Store(CacheKeys.PROFILE_CHANGE_PWD, new CacheItem<string>("success"));
                        return RedirectToSitecoreItem(ScPageItemId.ChangePassword);
                    }
                    var userId = CurrentPrincipal.UserId;
                    var newSessionId = CurrentPrincipal.SessionToken;
                    //var loginResponse = DewaApiClient.Authenticate(CurrentPrincipal.UserId, model.OldPassword, language: RequestLanguage, segment: Request.Segment());
                    var loginResponse = SmartCustomerClient.LoginUser(
                            new LoginRequest
                            {
                                getloginsessionrequest = new Getloginsessionrequest
                                {
                                    userid = CurrentPrincipal.UserId,
                                    password = Base64Encode(model.OldPassword),
                                }
                            }, RequestLanguage, Request.Segment());
                    if (loginResponse.Succeeded && loginResponse.Payload != null)
                    {
                        newSessionId = loginResponse.Payload.SessionNumber;
                    }
                    else if (!loginResponse.Succeeded && loginResponse.Payload != null && loginResponse.Payload.ResponseCode.Equals("116"))
                    {
                        ModelState.AddModelError("accountlock", loginResponse.Message);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, loginResponse.Message);
                    }
                    var response = DewaApiClient.ChangePassword(userId, newSessionId, model.OldPassword, model.NewPassword, model.ConfirmPassword, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        CacheProvider.Store(CacheKeys.PROFILE_CHANGE_PWD, new CacheItem<string>("success"));
                        return RedirectToSitecoreItem(ScPageItemId.ChangePassword);
                    }
                    CacheProvider.Store(CacheKeys.PROFILE_CHANGE_PWD_ERROR, new CacheItem<string>(response.Message));
                    //ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    CacheProvider.Store(CacheKeys.PROFILE_CHANGE_PWD_ERROR, new CacheItem<string>("Unexpected error"));
                    //ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            return RedirectToSitecoreItem(ScPageItemId.ChangePassword);
            //// Reset password fields
            //var emptyValue = new ValueProviderResult(string.Empty, string.Empty, CultureInfo.CurrentCulture);
            //ModelState.SetModelValue("OldPassword", emptyValue);
            //model.OldPassword = string.Empty;

            //ModelState.SetModelValue("NewPassword", emptyValue);
            //model.NewPassword = string.Empty;

            //ModelState.SetModelValue("ConfirmPassword", emptyValue);
            //model.ConfirmPassword = string.Empty;

            //return PartialView("~/Views/Feature/Account/Forms/_ChangePassword.cshtml", model);
        }

        [HttpGet]
        public ActionResult ChangePasswordSuccessful()
        {
            return View("~/Views/Feature/Account/Forms/_ChangePasswordSuccessful.cshtml");
        }

        [HttpGet]
        public ActionResult CannotChangePassword()
        {
            return View("~/Views/Feature/Account/Forms/_CannotChangePassword.cshtml");
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult RecoveryEmailSent()
        {
            RecoveryEmailSentModel model;
            if (!CacheProvider.TryGet(CacheKeys.RECOVERY_EMAIL_STATE, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            return View("~/Views/Feature/Account/Forms/_RecoveryEmailSent.cshtml", model);
        }

        [HttpGet]
        public ActionResult SetNewPassword(string userid, string dynli)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(dynli))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }

            try
            {
                if (IsLoggedIn)
                {
                    ClearSessionAndSignOut();
                    var url = Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped).ToString();
                    if (Url.IsLocalUrl(url))
                    {
                        return Redirect(url);
                    }
                }
                var response = DewaApiClient.SetNewPassword(userid, dynli, "V", string.Empty, string.Empty, RequestLanguage, Request.Segment());
                if (!response.Succeeded)
                {
                    ViewBag.reseterror = true;
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/Account/Forms/_SetNewPassword.cshtml", new SetNewPasswordModel
            {
                SessionToken = dynli,
                Username = userid
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(SetNewPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = DewaApiClient.SetNewPassword(model.Username, model.SessionToken, "D", model.Password, model.ConfirmPassword, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        return PartialView("~/Views/Feature/Account/Forms/_SetPasswordSuccessful.cshtml");
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            return PartialView("~/Views/Feature/Account/Forms/_SetNewPassword.cshtml", model);
        }

        [AcceptVerbs("GET", "HEAD")]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult Avatar()
        {
            var d = AuthStateService.GetActiveProfile();
            var model = new LoggedInAvatarDetail();
            model.UserName = !string.IsNullOrWhiteSpace(d.Name) ? d.Name : d.Username;
            model.BPNumber = d.BusinessPartner;
            model.ProfileImage = new ProfilePhotoModel();
            ProfilePhotoModel dd;
            if (CacheProvider.TryGet(CacheKeys.PROFILE_PHOTO, out dd))
            {
                model.ProfileImage = dd;
            }
            return PartialView("~/Views/Feature/Account/Account/_Avatar.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpPost]
        public ActionResult Avatar(string p)
        {
            var d = AuthStateService.GetActiveProfile();
            var model = new LoggedInAvatarDetail();
            model.UserName = !string.IsNullOrWhiteSpace(d.Name) ? d.Name : d.Username;
            model.BPNumber = d.BusinessPartner;
            model.ProfileImage = new ProfilePhotoModel();
            ProfilePhotoModel dd;
            if (CacheProvider.TryGet(CacheKeys.PROFILE_PHOTO, out dd))
            {
                model.ProfileImage = dd;
            }
            return PartialView("~/Views/Feature/Account/Account/_Avatar.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpPost]
        public ActionResult UpdateAvtar(FormCollection fileCollection)
        {
            string error = string.Empty;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (!AttachmentIsValid(file, General.MaxAttachmentSize, out error, General.AcceptedImageFileTypes))
                {
                    CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_ERROR, new CacheItem<string>(error));
                }
                if (Request.Files != null)
                {
                    if (!string.IsNullOrWhiteSpace(file.FileName))
                    {
                        var imageBytes = ReadImage(Request.Files[0]);
                        var extension = file.GetTrimmedFileExtension();

                        var response = DewaApiClient.UploadProfilePhoto(imageBytes, extension, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        if (response != null && response.Succeeded)
                        {
                            CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_PIC_SUCCESS, new CacheItem<string>(response.Message));
                            StoreProfilePhoto(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
                        }
                        else
                            CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_ERROR, new CacheItem<string>(response.Message));
                    }
                }
            }
            return RedirectToSitecoreItem(ScPageItemId.CustomerProfilePage);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult UpdateContactInfo()
        {
            UpdateContactInfoSuccessModel model = new UpdateContactInfoSuccessModel();
            ViewBag.Emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates).ToDictionary(x => x.DisplayName, x => Translate.Text(x.DisplayName));

            string errorMessage = "";
            string PROFILE_CONTACT_SUCCESS = "";

            if (CacheProvider.TryGet(CacheKeys.PROFILE_CONTACT_SUCCESS, out PROFILE_CONTACT_SUCCESS) && CacheProvider.TryGet(CacheKeys.UPDATE_CONTACT_INFO_STATE, out model))
            {
                ViewBag.UCI = PROFILE_CONTACT_SUCCESS;
            }
            else if (CacheProvider.TryGet(CacheKeys.PROFILE_CONTACT_ERROR, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                ViewBag.UCI = null;
            }
            else
            {
                ViewBag.UCI = null;
            }

            CacheProvider.Remove(CacheKeys.PROFILE_CONTACT_ERROR);
            CacheProvider.Remove(CacheKeys.PROFILE_CONTACT_SUCCESS);
            CacheProvider.Remove(CacheKeys.UPDATE_CONTACT_INFO_STATE);

            return PartialView("~/Views/Feature/Account/Account/_UpdateContactInfo.cshtml", new UpdateContactInfoModel() { UpdateContactInfoSuccessModel = model });
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateContactInfo(UpdateContactInfoModel model)
        {
            string error = string.Empty;
            CacheProvider.Remove(CacheKeys.PROFILE_CONTACT_SUCCESS);
            CacheProvider.Remove(CacheKeys.PROFILE_CONTACT_ERROR);
            if (model.ProfilePictureUploader != null)
            {
                if (!AttachmentIsValid(model.ProfilePictureUploader, General.MaxAttachmentSize, out error, General.AcceptedImageFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var changeResponse = DewaApiClient.ChangeContactDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,
                        new ChangeContactDetails
                        {
                            ContractAccountNumber = model.AccountNumberSelected,
                            BusinessPartnerNumber = model.SelectedBusinessPartnerNumber,
                            EmailAddress = model.EmailAddress,
                            Emirate = model.SelectedEmirateKey,
                            FaxNumber = !(string.IsNullOrEmpty(model.FaxNumber)) ? model.FaxNumber.AddMobileNumberZeroPrefix() : string.Empty,
                            MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                            NickName = model.NickName ?? string.Empty,
                            PoBox = model.PoBox,
                            PreferredLanguage = model.PreferredLanguage,
                            TelephoneNumber = !(string.IsNullOrEmpty(model.TelephoneNumber)) ? model.TelephoneNumber.AddMobileNumberZeroPrefix() : string.Empty
                        }, RequestLanguage, Request.Segment());

                    if (model.ProfilePictureUploader != null)
                    {
                        if (!string.IsNullOrWhiteSpace(model.ProfilePictureUploader.FileName))
                        {
                            var imageBytes = ReadImage(model.ProfilePictureUploader);
                            var extension = model.ProfilePictureUploader.GetTrimmedFileExtension();

                            DewaApiClient.UploadProfilePhoto(imageBytes, extension, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        }
                    }

                    if (changeResponse.Succeeded)
                    {
                        var state = new UpdateContactInfoSuccessModel()
                        {
                            AccountNumberSelected = model.AccountNumberSelected,
                            EmailAddress = model.EmailAddress,
                            FaxNumber = model.FaxNumber.AddMobileNumberZeroPrefix(),
                            MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                            NickName = model.NickName,
                            PoBox = model.PoBox,
                            PreferredLanguage = model.PreferredLanguage,
                            SelectedAccountName = model.SelectedAccountName,
                            SelectedBusinessPartnerNumber = model.SelectedBusinessPartnerNumber,
                            SelectedCategory = model.SelectedCategory,
                            SelectedEmirateValue = model.SelectedEmirateValue,
                            SelectedPremiseNumber = model.SelectedPremiseNumber,
                            IsAccountActive = model.IsAccountActive,
                            TelephoneNumber = model.TelephoneNumber.AddMobileNumberZeroPrefix()
                        };

                        // We clear the cached account list in order to ensure that the changes are immediately visible
                        CacheProvider.Remove(CacheKeys.ACCOUNT_LIST);

                        CacheProvider.Store(CacheKeys.UPDATE_CONTACT_INFO_STATE, new AccessCountingCacheItem<UpdateContactInfoSuccessModel>(state, Times.Once));
                        CacheProvider.Store(CacheKeys.PROFILE_CONTACT_SUCCESS, new CacheItem<string>("success"));

                        StoreProfilePhoto(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
                        //var path = Context.Database.GetItem(SitecoreItemIdentifiers.J21_UPDATE_CONTACT_DETAILS_CONFIRMATION).Paths.FullPath;
                        //return RedirectToSitecoreItem(SitecoreItemIdentifiers.J21_UPDATE_CONTACT_DETAILS_CONFIRMATION);
                    }
                    else
                    {
                        error = changeResponse.Message;
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    error = Translate.Text("Unexpected error");
                    //ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                CacheProvider.Store(CacheKeys.PROFILE_CONTACT_ERROR, new CacheItem<string>(error));
            }

            return RedirectToSitecoreItem(ScPageItemId.ConsumerProfilePage);

            //ViewBag.Emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates).ToDictionary(x => x.DisplayName, x => Translate.Text(x.DisplayName));

            //return PartialView("_UpdateContactInfo", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult UpdateContactInfoSuccess()
        {
            UpdateContactInfoSuccessModel model;
            if (!CacheProvider.TryGet(CacheKeys.UPDATE_CONTACT_INFO_STATE, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J21_UPDATE_CONTACT_DETAILS);
            }

            CurrentPrincipal.PrimaryAccount = model.AccountNumberSelected;
            CurrentPrincipal.Name = model.NickName;
            CurrentPrincipal.EmailAddress = model.EmailAddress;
            CurrentPrincipal.MobileNumber = model.MobileNumber;

            AuthStateService.Save(CurrentPrincipal);

            return PartialView("~/Views/Feature/Account/Account/_UpdateContactInfoSuccess.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult UpdateCustomerInfo(string id)
        {
            UpdateCustomerInfoModel model = new UpdateCustomerInfoModel();
            UpdateCustomerInfoSuccessModel successModel = new UpdateCustomerInfoSuccessModel();
            ServiceResponse<AccountDetails[]> accountdetails;
            try
            {
                ViewBag.Emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates).ToDictionary(x => x.DisplayName, x => Translate.Text(x.DisplayName));
                ViewBag.UCI = null;
                string errorMessage = "";
                string PROFILE_CUSTOMER_SUCCESS = "";

                if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_PIC_SUCCESS, out PROFILE_CUSTOMER_SUCCESS))
                {
                    ViewBag.ProfileAvatar = PROFILE_CUSTOMER_SUCCESS;
                }
                if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_SUCCESS, out PROFILE_CUSTOMER_SUCCESS) && CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_STATE, out successModel))
                {
                    ViewBag.UCI = PROFILE_CUSTOMER_SUCCESS;
                    model.UpdateCustomerInfoSuccessModel = successModel;
                }
                else if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_SUCCESS, out PROFILE_CUSTOMER_SUCCESS) && CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_SUCCESS_OTP, out successModel))
                {
                    ViewBag.UCI = PROFILE_CUSTOMER_SUCCESS;
                    model.UpdateCustomerInfoSuccessModel = successModel;
                }
                else if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_ERROR, out errorMessage))
                {
                    if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_RESPONSE, out model))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                        id = model.SelectedBusinessPartner;
                    }
                }
                var accountResponse = GetBillingAccounts(false, true, string.Empty, string.Empty);
                if (accountResponse != null && accountResponse.Succeeded && accountResponse.Payload != null)
                {
                    accountdetails = accountResponse;
                    if (string.IsNullOrWhiteSpace(id))
                        model.SelectedBusinessPartner = CurrentPrincipal.BusinessPartner;//accountdetails.Payload.ToList().FirstOrDefault().BusinessPartnerNumber.TrimStart(new char[] { '0' });
                    else
                        model.SelectedBusinessPartner = id;
                    if (string.IsNullOrWhiteSpace(model.SelectedBusinessPartner))
                        model.SelectedBusinessPartner = string.Empty;

                    var selectedbp = model.SelectedBusinessPartner.TrimStart(new char[] { '0' });
                    model.BusinessPartnerList = accountdetails.Payload.ToList().GroupBy(t => t.BusinessPartnerNumber)
                                              .Select(g => g.First()).Select(x => new SelectListItem
                                              {
                                                  Value = x.BusinessPartnerNumber,
                                                  Text = x.BPName + " - " + x.BusinessPartnerNumber
                                              }).Distinct().ToList();
                    model.Accounts = accountdetails.Payload.ToList().Where(x => x.BusinessPartnerNumber.Equals(selectedbp)).ToArray();
                    model.CustomerAccountList = accountdetails.Payload.Where(x => x.BusinessPartnerNumber.Equals(selectedbp)).Select(x => new SelectListItem
                    {
                        Text = (string.IsNullOrWhiteSpace(x.NickName) ? x.AccountName : x.NickName) + " - " + x.BillingClass + " | " + Translate.Text("cust.account.number") + " " + x.AccountNumber,
                        Value = x.AccountNumber
                    }).ToList();
                }
                if (ViewBag.UCI == null && string.IsNullOrWhiteSpace(errorMessage))
                {
                    var response = DewaApiClient.GetProfileDetails(FormatBusinessPartner(model.SelectedBusinessPartner), CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                    if (response != null && response.Payload != null)
                    {
                        CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_MASTERDATA, new CacheItem<dewaSVC.masterData[]>(response.Payload.masterDatas, TimeSpan.FromMinutes(40)));
                        if (response.Payload.masterDatas != null)
                        {
                            model.MaritalStatusList = GetMasterData(response.Payload.masterDatas, "MARST");
                            model.DisabilityTypeList = GetMasterData(response.Payload.masterDatas, "DIS_TYPE");
                            model.PreferredMediaList = GetMasterData(response.Payload.masterDatas, "MCHANNEL");
                            model.MonthlyIncomeList = GetMasterData(response.Payload.masterDatas, "MINCOME");
                            model.NationalityList = GetMasterData(response.Payload.masterDatas, "NATIO");
                            model.MedicalSituationTypeList = GetMasterData(response.Payload.masterDatas, "MADICAL_TYPE");
                            model.TechnologyUsage = GetMasterData(response.Payload.masterDatas, "TECH");
                            model.PODTypesList = GetMasterData(response.Payload.masterDatas, "POD_TYPE");
                            model.PODCategoryList = GetMasterData(response.Payload.masterDatas, "POD_CAT");
                            model.EquiExistList = GetMasterData(response.Payload.masterDatas, "EQUI_EXIST");
                            model.RelationList = GetMasterData(response.Payload.masterDatas, "RELATION");
                            model.PrefferedLanguageList = GetLstDataSource(DataSources.CUSTOMER_PROFILE_LANGUAGE).ToList();
                            model.IssuingAuthorityList = GetLstDataSource(DataSources.CUSTOMER_PROFILE_TRADE_LICENSE_ISSUING_AUTHOR).ToList();
                        }
                        if (response.Payload.communications != null)
                        {
                            model.BPCommunications = response.Payload.communications.Select(x => new BPCommunication
                            {
                                CommunicationType = x.communicationType,
                                SubscribeEmail = !string.IsNullOrWhiteSpace(x.email) && x.email.ToLower().Equals("y") ? true : false,
                                SubscribeSMS = !string.IsNullOrWhiteSpace(x.sms) && x.sms.ToLower().Equals("y") ? true : false,
                            }).ToList();
                        }
                        if (response.Payload.profileMain != null)
                        {
                            model.NameUpdateExist = response.Payload.profileMain.nameupdateExists;
                            model.EmirateIdUpdateExist = response.Payload.profileMain.emiratesidupdateexist;
                            model.PassportUpdateExist = response.Payload.profileMain.passportupdateexist;
                            model.TradelicenseUpdateExist = response.Payload.profileMain.tradelicenseupdateexist;
                            model.EmirateIdExpired = response.Payload.profileMain.emiratesidexpired;
                            model.PassportExpired = response.Payload.profileMain.passportidexpired;
                            model.TradelicenseExpired = response.Payload.profileMain.tradelicenseidexpired;
                            model.BusinessPartnerType = response.Payload.profileMain.businessPartnerType;
                            model.BusinessPartnerNumber = response.Payload.profileMain.businessPartner;
                            model.CustomerName = response.Payload.profileMain.name;
                            model.EmailAddress = response.Payload.profileMain.emailAddress;
                            model.MobileNumber = response.Payload.profileMain.mobileNumber.RemoveMobileNumberZeroPrefix();
                            model.TelephoneNumber = response.Payload.profileMain.telephoneNumber;
                            model.HiddenEmailAddress = response.Payload.profileMain.emailAddress;
                            model.HiddenMobileNumber = response.Payload.profileMain.mobileNumber.RemoveMobileNumberZeroPrefix();
                            model.POBox = response.Payload.profileMain.pobox;
                            model.SelectedEmirateKey = response.Payload.profileMain.emirate;
                            model.EmirateId = response.Payload.profileMain.emirates_id;
                            if (model.EmirateId != null)
                                model.EmirateId = model.EmirateId.Replace("-", "");
                            //model.EmirateExpDate = (!string.IsNullOrWhiteSpace(response.Payload.profileMain.emiratesid_expiry) && response.Payload.profileMain.emiratesid_expiry != "0000-00-00") ? DateHelper.getCultureDate(response.Payload.profileMain.emiratesid_expiry).ToString("dd MMM yyyy") : string.Empty;
                            model.EmirateExpDate = !string.IsNullOrWhiteSpace(response.Payload.profileMain.emiratesid_expiry) && !response.Payload.profileMain.emiratesid_expiry.Equals("0000-00-00") ?
                                                 DateTime.ParseExact(response.Payload.profileMain.emiratesid_expiry, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMMM yyyy", SitecoreX.Culture) : string.Empty;

                            model.PassportNo = response.Payload.profileMain.passport_no;
                            //model.PassportExpDate = (!string.IsNullOrWhiteSpace(response.Payload.profileMain.passport_expiry) && response.Payload.profileMain.passport_expiry != "0000-00-00") ? DateHelper.getCultureDate(response.Payload.profileMain.passport_expiry).ToString("dd MMM yyyy") : string.Empty;
                            model.PassportExpDate = !string.IsNullOrWhiteSpace(response.Payload.profileMain.passport_expiry) && !response.Payload.profileMain.passport_expiry.Equals("0000-00-00") ?
                                                 DateTime.ParseExact(response.Payload.profileMain.passport_expiry, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMMM yyyy", SitecoreX.Culture) : string.Empty;

                            model.SelectedNationality = response.Payload.profileMain.nationality;
                            model.TrandeLicenseNo = response.Payload.profileMain.tradelicense_no;
                            //model.TrandeLicenseExpDate = (!string.IsNullOrWhiteSpace(response.Payload.profileMain.tradelicense_expiry) && response.Payload.profileMain.tradelicense_expiry != "0000-00-00") ? DateHelper.getCultureDate(response.Payload.profileMain.tradelicense_expiry).ToString("dd MMM yyyy") : string.Empty;
                            model.TrandeLicenseExpDate = !string.IsNullOrWhiteSpace(response.Payload.profileMain.tradelicense_expiry) && !response.Payload.profileMain.tradelicense_expiry.Equals("0000-00-00") ?
                                                 DateTime.ParseExact(response.Payload.profileMain.tradelicense_expiry, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMMM yyyy", SitecoreX.Culture) : string.Empty;

                            model.SelectedIssuingAuthority = response.Payload.profileMain.institute;
                            model.SelectedPrefferedLanguage = response.Payload.profileMain.language;
                            //model.GeneralCommSMS = response.Payload.sms;
                            model.SelectedMaritalStatus = response.Payload.profileMain.marital_status;
                            model.SelectedMonthlyIncome = response.Payload.profileMain.monthly_income;
                            model.SelectedTechnologyRating = response.Payload.profileMain.technology_rating;
                            model.SelectedPreferredMedia = response.Payload.profileMain.preferred_media;
                            model.mailVerifyPending = response.Payload.profileMain.mailVerifyPending;
                            model.OtherMediaChannel = response.Payload.profileMain.otherchannel;
                            model.DateOfBirth = !string.IsNullOrWhiteSpace(response.Payload.profileMain.dateofbirth) && !response.Payload.profileMain.dateofbirth.Equals("0000-00-00") ?
                                                 DateTime.ParseExact(response.Payload.profileMain.dateofbirth, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMMM yyyy", SitecoreX.Culture) : string.Empty;

                            //CultureInfo culture;

                            //culture = SitecoreX.Culture;
                            //if (culture.ToString().Equals("ar-AE"))
                            //{
                            //    model.EmirateExpDate = model.EmirateExpDate.Replace("Jan", "يناير").Replace("Feb", "فبراير").Replace("Mar", "مارس").Replace("Apr", "أبريل").Replace("May", "مايو").Replace("Jun", "يونيو").Replace("Jul", "يوليو").Replace("Aug", "أغسطس").Replace("Sep", "سبتمبر").Replace("Oct", "أكتوبر").Replace("Nov", "نوفمبر").Replace("Dec", "ديسمبر");
                            //    model.PassportExpDate = model.PassportExpDate.Replace("Jan", "يناير").Replace("Feb", "فبراير").Replace("Mar", "مارس").Replace("Apr", "أبريل").Replace("May", "مايو").Replace("Jun", "يونيو").Replace("Jul", "يوليو").Replace("Aug", "أغسطس").Replace("Sep", "سبتمبر").Replace("Oct", "أكتوبر").Replace("Nov", "نوفمبر").Replace("Dec", "ديسمبر");
                            //    model.TrandeLicenseExpDate = model.TrandeLicenseExpDate.Replace("Jan", "يناير").Replace("Feb", "فبراير").Replace("Mar", "مارس").Replace("Apr", "أبريل").Replace("May", "مايو").Replace("Jun", "يونيو").Replace("Jul", "يوليو").Replace("Aug", "أغسطس").Replace("Sep", "سبتمبر").Replace("Oct", "أكتوبر").Replace("Nov", "نوفمبر").Replace("Dec", "ديسمبر");
                            //}

                            //POD data
                            CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_MAIN, new CacheItem<dewaSVC.profileMain>(response.Payload.profileMain, TimeSpan.FromMinutes(20)));
                        }

                        if (response.Payload.podProfileMain != null)
                        {
                            var myselfPOD = response.Payload.podProfileMain.Where(x => !string.IsNullOrEmpty(x.type) && x.type == "M").ToList();
                            if (myselfPOD != null && myselfPOD.Count() > 0)
                            {
                                model.SelectedAccountNumber = myselfPOD[0].contractAccountNumber;
                                model.SelectedPODCategory = myselfPOD[0].podCategory;
                                ///model.SelectedDisabilityType = myselfPOD[0].disabilityType;
                                if (!string.IsNullOrWhiteSpace(myselfPOD[0].disabilityType))
                                {
                                    model.MultipleDisableTypes = myselfPOD[0].disabilityType.Split(';').ToList();
                                }
                                if (!string.IsNullOrWhiteSpace(myselfPOD[0].medicalSituationExist))
                                {
                                    model.MultipleMedicalSituationTypes = myselfPOD[0].medicalSituationExist.Split(';').ToList();
                                }
                                //model.SelectedMedicalSituationType = myselfPOD[0].medicalSituationExist;
                                model.OtherDisabilityType = myselfPOD[0].otherdisease;
                                model.OtherMedicalSituationType = myselfPOD[0].othermedicaltype;
                                model.SelectedEquipment = myselfPOD[0].medicalEquipmentType;
                                model.DeterminationID = myselfPOD[0].podidcardNumber;
                                model.IsMyselfPOD = true;
                                var fItem = model.PODTypesList.Where(x => x.Value == "M").FirstOrDefault();
                                fItem.Selected = true;
                            }
                            model.FamilyMembers = response.Payload.podProfileMain.Where(x => x.type == "F" && !string.IsNullOrEmpty(x.type)).Select(x => new PODMembers
                            {
                                Action = "U",
                                AccountNumber = x.contractAccountNumber,
                                PODNumber = x.podidcardNumber,
                                //DisabilityType = x.disabilityType,
                                MultipleDisableTypes = x.disabilityType != null ? x.disabilityType.Split(';').ToList() : null,
                                OtherMedicalSituationType = x.othermedicaltype,
                                OtherDisabilityType = x.otherdisease,
                                FamilyMemberName = x.nameofFamilyMember,
                                Relationship = x.relationship,
                                PODCategory = x.podCategory,
                                OtherRelationship = x.otherrelationship,
                                //MedicalSituationType = x.medicalSituationExist,
                                MultipleMedicalSituationTypes = x.medicalSituationExist != null ? x.medicalSituationExist.Split(';').ToList() : null,
                                MedicalEquipmentType = x.medicalEquipmentType,
                                PODType = x.type,
                            }).ToList();
                            if (model.FamilyMembers != null && model.FamilyMembers.Count > 0)
                            {
                                var fItem = model.PODTypesList.Where(x => x.Value == "F").FirstOrDefault();
                                fItem.Selected = true;
                            }
                        }
                    }
                }
                CacheProvider.Remove(CacheKeys.CUSTOMER_PROFILE_ERROR);
                CacheProvider.Remove(CacheKeys.CUSTOMER_PROFILE_SUCCESS);
                CacheProvider.Remove(CacheKeys.CUSTOMER_PROFILE_STATE);
                CacheProvider.Remove(CacheKeys.CUSTOMER_PROFILE_RESPONSE);
                CacheProvider.Remove(CacheKeys.CUSTOMER_PROFILE_PIC_SUCCESS);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return PartialView("~/Views/Feature/Account/Account/_UpdateCustomerInfo.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateCustomerInfo(UpdateCustomerInfoModel model)
        {
            string error = string.Empty;
            try
            {
                ViewBag.Emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates).ToDictionary(x => x.DisplayName, x => Translate.Text(x.DisplayName));
                dewaSVC.masterData[] masterData = null;
                if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_MASTERDATA, out masterData))
                {
                    if (masterData != null)
                    {
                        model.MaritalStatusList = GetMasterData(masterData, "MARST");
                        model.DisabilityTypeList = GetMasterData(masterData, "DIS_TYPE");
                        model.PreferredMediaList = GetMasterData(masterData, "MCHANNEL");
                        model.MonthlyIncomeList = GetMasterData(masterData, "MINCOME");
                        model.NationalityList = GetMasterData(masterData, "NATIO");
                        model.MedicalSituationTypeList = GetMasterData(masterData, "MADICAL_TYPE");
                        model.TechnologyUsage = GetMasterData(masterData, "TECH");
                        model.PODTypesList = GetMasterData(masterData, "POD_TYPE");
                        model.PODCategoryList = GetMasterData(masterData, "POD_CAT");
                        model.EquiExistList = GetMasterData(masterData, "EQUI_EXIST");
                        model.RelationList = GetMasterData(masterData, "RELATION");
                        model.PrefferedLanguageList = GetLstDataSource(DataSources.CUSTOMER_PROFILE_LANGUAGE).ToList();
                        model.IssuingAuthorityList = GetLstDataSource(DataSources.CUSTOMER_PROFILE_TRADE_LICENSE_ISSUING_AUTHOR).ToList();
                    }
                }
                if (ModelState.IsValid)
                {
                    profileSaveInput profileSaveInput = null;
                    List<bpCommunicationDetails> communicationDetails = new List<bpCommunicationDetails>();

                    communicationDetails = model.BPCommunications != null && model.BPCommunications.HasAny() ?
                             model.BPCommunications.Select(x => new bpCommunicationDetails
                             {
                                 communicationtype = x.CommunicationType,
                                 email = x.SubscribeEmail ? "Y" : string.Empty,
                                 sms = x.SubscribeSMS ? "Y" : string.Empty,
                             }).ToList()
                            : null;
                    model.MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix();
                    List<dewaSVC.profilePODDetails> podmembers = new List<dewaSVC.profilePODDetails>();
                    if (model.postedPOD != null)
                    {
                        podmembers = JsonConvert.DeserializeObject<List<dewaSVC.profilePODDetails>>(model.postedPOD);
                    }

                    foreach (var fkey in Request.Files.AllKeys)
                    {
                        if (fkey.Contains("tlupload"))
                        {
                            HttpPostedFileBase file = Request.Files[fkey];

                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                var item1 = podmembers.Where(x => x.filename1 == fkey).FirstOrDefault();
                                if (item1 != null)
                                    item1.attachment1 = binaryReader.ReadBytes(file.ContentLength);

                                var item2 = podmembers.Where(x => x.filename2 == fkey).FirstOrDefault();
                                if (item2 != null)
                                    item2.attachment2 = binaryReader.ReadBytes(file.ContentLength);

                                var item3 = podmembers.Where(x => x.filename3 == fkey).FirstOrDefault();
                                if (item3 != null)
                                    item3.attachment3 = binaryReader.ReadBytes(file.ContentLength);
                            }
                        }
                    }

                    byte[] EmirateIdByteData = null;
                    if (model.EmirateIdUploader != null)
                    {
                        using (var binaryReader = new BinaryReader(model.EmirateIdUploader.InputStream))
                        {
                            EmirateIdByteData = binaryReader.ReadBytes(model.EmirateIdUploader.ContentLength);
                        }
                    }
                    byte[] PassportByteData = null;
                    if (model.PassportUploader != null)
                    {
                        using (var binaryReader = new BinaryReader(model.PassportUploader.InputStream))
                        {
                            PassportByteData = binaryReader.ReadBytes(model.PassportUploader.ContentLength);
                        }
                    }
                    byte[] TrandeLicenseByteData = null;
                    if (model.TrandeLicenseUploader != null)
                    {
                        using (var binaryReader = new BinaryReader(model.TrandeLicenseUploader.InputStream))
                        {
                            TrandeLicenseByteData = binaryReader.ReadBytes(model.TrandeLicenseUploader.ContentLength);
                        }
                    }

                    byte[] AccountHolderEmirateIDByte = null;
                    if (model.AccountHolderEmirateID != null)
                    {
                        using (var binaryReader = new BinaryReader(model.AccountHolderEmirateID.InputStream))
                        {
                            AccountHolderEmirateIDByte = binaryReader.ReadBytes(model.AccountHolderEmirateID.ContentLength);
                        }
                    }
                    byte[] DeterminationIDCopyByte = null;
                    if (model.DeterminationIDCopy != null)
                    {
                        using (var binaryReader = new BinaryReader(model.DeterminationIDCopy.InputStream))
                        {
                            DeterminationIDCopyByte = binaryReader.ReadBytes(model.DeterminationIDCopy.ContentLength);
                        }
                    }

                    if (model.SelectedPODType != null && model.SelectedPODType.Where(x => x.Contains("M")).ToList().Count > 0)
                    {
                        podmembers.Add(new dewaSVC.profilePODDetails
                        {
                            contractaccountnumber = model.SelectedAccountNumber,
                            podcategory = model.SelectedPODCategory,
                            disabilitytype = model.MultipleDisableTypes != null ? string.Join(";", model.MultipleDisableTypes) : string.Empty,
                            medicalSituationexist = model.MultipleMedicalSituationTypes != null ? string.Join(";", model.MultipleMedicalSituationTypes) : string.Empty,
                            medicalequipmenttype = model.SelectedEquipment,
                            podidcardnumber = model.DeterminationID,
                            type = "M",
                            attachment1 = AccountHolderEmirateIDByte,
                            attachment2 = DeterminationIDCopyByte,
                            othermedicaltype = model.OtherMedicalSituationType,
                            otherdisease = model.OtherDisabilityType
                        });
                    }
                    else
                    {
                        if (model.IsMyselfPOD)
                        {
                            podmembers.Add(new dewaSVC.profilePODDetails
                            {
                                contractaccountnumber = model.SelectedAccountNumber,
                                podcategory = model.SelectedPODCategory,
                                disabilitytype = model.SelectedDisabilityType,
                                otherdisease = model.OtherDisabilityType,
                                medicalSituationexist = model.SelectedMedicalSituationType,
                                othermedicaltype = model.OtherMedicalSituationType,
                                medicalequipmenttype = model.SelectedEquipment,
                                podidcardnumber = model.DeterminationID,
                                type = "M",
                                recorddeletionindicator = "X",
                                attachment1 = AccountHolderEmirateIDByte,
                                attachment2 = DeterminationIDCopyByte,
                            });
                        }
                    }
                    if (podmembers != null && podmembers.Count > 0)
                    {
                        var myselfPOD = podmembers.Where(x => !string.IsNullOrEmpty(x.type) && x.type == "M").ToList();
                        if (myselfPOD != null && myselfPOD.Count() > 0)
                        {
                            model.SelectedAccountNumber = myselfPOD[0].contractaccountnumber;
                            model.SelectedPODCategory = myselfPOD[0].podcategory;
                            model.SelectedDisabilityType = myselfPOD[0].disabilitytype;
                            model.SelectedMedicalSituationType = myselfPOD[0].medicalSituationexist;
                            model.SelectedEquipment = myselfPOD[0].medicalequipmenttype;
                            model.DeterminationID = myselfPOD[0].podidcardnumber;
                            model.OtherDisabilityType = myselfPOD[0].otherdisease;
                            model.OtherMedicalSituationType = myselfPOD[0].othermedicaltype;
                            var fItem = model.PODTypesList.Where(x => x.Value == "M").FirstOrDefault();
                            fItem.Selected = true;
                        }
                        model.FamilyMembers = podmembers.Where(x => x.type == "F" && !string.IsNullOrEmpty(x.type)).Select(x => new PODMembers
                        {
                            AccountNumber = x.contractaccountnumber,
                            PODNumber = x.podidcardnumber,
                            OtherDisabilityType = x.otherdisease,
                            OtherMedicalSituationType = x.othermedicaltype,
                            MultipleDisableTypes = x.disabilitytype != null ? x.disabilitytype.Split(';').ToList() : null,
                            FamilyMemberName = x.nameoffamilymember,
                            Relationship = x.relationship,
                            OtherRelationship = x.otherrelationship,
                            PODCategory = x.podcategory,
                            //MedicalSituationType = x.medicalSituationexist,
                            MultipleMedicalSituationTypes = x.medicalSituationexist != null ? x.medicalSituationexist.Split(';').ToList() : null,
                            MedicalEquipmentType = x.medicalequipmenttype,
                            Action = x.recorddeletionindicator,
                            PODType = x.type,
                        }).ToList();
                        if (model.FamilyMembers != null && model.FamilyMembers.Count > 0)
                        {
                            var fItem = model.PODTypesList.Where(x => x.Value == "F").FirstOrDefault();
                            fItem.Selected = true;
                        }
                    }
                    model.SelectedPreferredMedia = model.MultiplePreferredMedia != null ? string.Join(";", model.MultiplePreferredMedia) : string.Empty;
                    profileSaveInput = new profileSaveInput
                    {
                        profileheader = new profileHeaderDetails
                        {
                            businesspartner = model.SelectedBusinessPartner, //CurrentPrincipal.BusinessPartner,
                            businesspartnertype = model.BusinessPartnerType,
                            name = model.CustomerName,
                            email = model.EmailAddress,
                            mobile = model.MobileNumber.AddMobileNumberZeroPrefix(),
                            telephone = model.TelephoneNumber.AddMobileNumberZeroPrefix(),
                            pobox = model.POBox,
                            emirate = model.SelectedEmirateKey,
                            emiratesid = model.EmirateId,
                            emiratesidexpiry = model.EmirateExpDate != null ? DateHelper.getCultureDate(model.EmirateExpDate).ToString("yyyyMMdd") : string.Empty,
                            passportnumber = model.PassportNo,
                            passportexpiry = model.PassportExpDate != null ? DateHelper.getCultureDate(model.PassportExpDate).ToString("yyyyMMdd") : string.Empty,
                            nationality = model.SelectedNationality,
                            passportattachment = PassportByteData,
                            tradelicensenumber = model.TrandeLicenseNo,
                            tradelicenseexpiry = model.TrandeLicenseExpDate != null ? DateHelper.getCultureDate(model.TrandeLicenseExpDate).ToString("yyyyMMdd") : string.Empty,
                            tradelicenseattachment = TrandeLicenseByteData,
                            language = model.SelectedPrefferedLanguage,
                            institute = model.SelectedIssuingAuthority,
                            // SelectedIssuingAuthority
                            // subcribe Email / SMS
                            maritalstatus = model.SelectedMaritalStatus,
                            monthlyincome = model.SelectedMonthlyIncome,
                            technologyrating = model.SelectedTechnologyRating,
                            //preferredmedia = model.SelectedPreferredMedia,
                            preferredmedia = model.MultiplePreferredMedia != null ? string.Join(";", model.MultiplePreferredMedia) : string.Empty,
                            otherchannel = model.OtherMediaChannel,
                            emiratesidattachment = EmirateIdByteData,
                            dateofbirth = model.DateOfBirth != null ? DateHelper.getCultureDate(model.DateOfBirth).ToString("yyyyMMdd") : string.Empty,
                            otprequestid = model.OtpRequestId
                        },
                        bpCommunicationlist = communicationDetails != null ? communicationDetails.ToArray() : null,
                        podlist = podmembers.ToArray(),
                        contractaccount = model.AccountNumberSelected != null ? model.AccountNumberSelected.Split(',') : null
                    };

                    var response = DewaApiClient.SetCustomerProfileSave(profileSaveInput, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null && response.Payload.responsecode != "0")
                    {
                        model.Success = "1";
                        var successModel = new UpdateCustomerInfoSuccessModel()
                        {
                            BusinessPartnerNumber = model.SelectedBusinessPartner,
                            AccountNumberSelected = model.AccountNumberSelected,// model.SelectedAccountNumber != null ? model.SelectedAccountNumber : "N/A",
                            EmailAddress = model.EmailAddress,
                            MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                            NickName = model.CustomerName,
                            PoBox = model.POBox,
                            PreferredLanguage = model.PrefferedLanguageList.Where(x => x.Value == model.SelectedPrefferedLanguage).Select(x => x.Text).FirstOrDefault(),
                        };
                        ClearAccountCache();
                        CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_SUCCESS, new CacheItem<string>("success"));
                        if (response.Payload.requestnumber.Equals("00000000000000000000"))
                        {
                            CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_STATE, new AccessCountingCacheItem<UpdateCustomerInfoSuccessModel>(successModel, Times.Once));
                        }
                        else
                        {
                            VerifyMobileNumber verifyMobile = new VerifyMobileNumber
                            {
                                RequestType = model.RequestType,
                                BusinessPartner = model.SelectedBusinessPartner,
                                Requestnumber = response.Payload.requestnumber,
                                EmailAddress = !string.IsNullOrWhiteSpace(model.EmailAddress) ? model.EmailAddress : string.Empty,
                                MobileNumber = !string.IsNullOrWhiteSpace(model.MobileNumber) ? model.MobileNumber.AddMobileNumberZeroPrefix() : string.Empty,
                                URL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.customerProfile),
                                SuccessURL = CustomerProfileSuccessURL.Customer_Profile
                            };
                            CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_SUCCESS_OTP2, new AccessCountingCacheItem<UpdateCustomerInfoSuccessModel>(successModel, Times.Once));
                            CacheProvider.Store(CacheKeys.AccountInformation_Request, new CacheItem<VerifyMobileNumber>(verifyMobile));
                            CacheProvider.Store(CacheKeys.AccountInformation_Request_Onetime, new AccessCountingCacheItem<VerifyMobileNumber>(verifyMobile, Times.Max));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.verifymobilenumber_page);
                        }
                    }
                    else
                    {
                        error = response.Message;
                        model.Success = "0";
                        CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_RESPONSE, new CacheItem<UpdateCustomerInfoModel>(model, TimeSpan.FromMinutes(20)));
                    }
                }
                if (!string.IsNullOrEmpty(error))
                {
                    CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_ERROR, new CacheItem<string>(error));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return RedirectToSitecoreItem(ScPageItemId.CustomerProfilePage);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CheckProfileValidation(string businesspartner, string ivtype, string idnumber, string name, string tradelicense, string passport, string dob, string nationality, string institute)
        {
            string responseMessage = string.Empty;
            string responseCode = string.Empty;
            if (!string.IsNullOrEmpty(businesspartner) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(ivtype))
            {
                var response = DewaApiClient.GetCustomerProfileValidate(new profileCustomerValidateInput
                {
                    customerdetails = new profileCustomerDetails
                    {
                        name = name,
                        emiratesid = idnumber,
                        tradelicensenumber = tradelicense,
                        passportnumber = passport,
                        passportdateofbirth = !string.IsNullOrWhiteSpace(dob) ? DateHelper.getCultureDate(dob).ToString("yyyyMMdd") : string.Empty,
                        passportnationality = nationality,
                        institute = institute,
                        businesspartner = businesspartner
                    },
                }, ivtype, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                responseCode = "399";
                responseMessage = response.Message; //Translate.Text("cust.require.ivtype");

                if (response != null && response.Payload != null)
                {
                    responseMessage = response.Message;
                    responseCode = response.Payload.responsecode;
                    return Json(new { Message = responseMessage, errorCode = responseCode });
                }
            }
            else
            {
                responseCode = "399";
                responseMessage = Translate.Text("cust.require.ivtype");
            }
            return Json(new { Message = responseMessage, errorCode = responseCode });
        }

        #region Manage Account information V1

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult ManageAccountV1()
        {
            string errorMessage;
            UpdateContactInfoSuccessModel successModel;
            ManageAccountInfo model = new ManageAccountInfo();
            ViewBag.success = false;
            if (CacheProvider.TryGet(CacheKeys.AccountInformation_Error, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            if (CacheProvider.TryGet(CacheKeys.AccountInformation_Success, out successModel))
            {
                model.successModel = successModel;
                ViewBag.success = true;
            }
            else if (CacheProvider.TryGet(CacheKeys.AccountInformation_Success_OTP, out successModel))
            {
                model.successModel = successModel;
                ViewBag.success = true;
            }
            return View("~/Views/Feature/Account/Account/v1/ManageAccount.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), ValidateAntiForgeryToken, HttpPost]
        public ActionResult ManageAccountV1(ManageAccountInfo model)
        {
            string error = string.Empty;
            byte[] fileupload = new byte[0];
            if (model.ProfilePictureUploader != null)
            {
                if (!AttachmentIsValid(model.ProfilePictureUploader, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.ProfilePictureUploader.InputStream.CopyTo(memoryStream);
                        fileupload = memoryStream.ToArray();
                    }
                }
            }
            if (ModelState.IsValid)
            {
                var response = DewaApiClient.SetContractAccountSave(
                        new contractAccountSaveInput
                        {
                            profilecaheader = new profileContractAccount
                            {
                                contractAccount = model.AccountNumberSelected,
                                businessPartner = model.SelectedBusinessPartnerNumber,
                                emailAddres = model.EmailAddress,
                                mobile = !string.IsNullOrWhiteSpace(model.MobileNumber) ? model.MobileNumber.AddMobileNumberZeroPrefix() : string.Empty,
                                nickName = model.NickName,
                                primaryAccount = model.IsPrimaryAccount ? "X" : string.Empty,
                                otprequestid = model.OtpRequestId
                            },

                            contractaccount = model.MultipleAccountNumberSelected != null ? model.MultipleAccountNumberSelected.Split(',') : new string[] { model.AccountNumberSelected },
                            bpCommunicationlist = model.CommunicationList != null && model.CommunicationList.HasAny() ?
                             model.CommunicationList.Select(x => new bpCommunicationDetails
                             {
                                 communicationtype = x.CommunicationType,
                                 email = x.IsEmail ? "Y" : string.Empty,
                                 sms = x.IsSMS ? "Y" : string.Empty,
                             }).ToArray()
                            : null
                        }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && !string.IsNullOrWhiteSpace(response.Payload))
                {
                    if (model.ProfilePictureUploader != null)
                    {
                        var profilepicresponse = DewaApiClient.SetImageContractAccount(new SetImageContractAccount
                        {
                            contractaccountnumber = model.AccountNumberSelected,
                            imagedata = fileupload,
                            sessionid = CurrentPrincipal.SessionToken,
                            userid = CurrentPrincipal.UserId
                        }, RequestLanguage, Request.Segment());
                        var projectedFilename = string.Format("{0}_{1}x{2}", model.AccountNumberSelected.TrimStart(new char[] { '0' }), 50, 50);
                        var projectedPath = GetAccountPicsPhysicalUploadPath(projectedFilename);
                        RemoveCachedImage(projectedPath);
                    }
                    string fullname = string.Empty;
                    var accountsresponse = GetAccounts(false);
                    if (accountsresponse != null && accountsresponse.Payload != null && accountsresponse.Payload.Count() > 0 && accountsresponse.Payload.Where(x => x.AccountNumber.Equals(model.AccountNumberSelected.TrimStart(new char[] { '0' }))).Any())
                    {
                        var selectedaccountList = accountsresponse.Payload.Where(x => x.AccountNumber.Equals(model.AccountNumberSelected.TrimStart(new char[] { '0' })));

                        if (selectedaccountList != null && selectedaccountList.Count() > 0)
                        {
                            var selectedaccountdetails = selectedaccountList.FirstOrDefault();
                            fullname = selectedaccountdetails.AccountName;
                        }
                    }
                    CacheProvider.Remove(CacheKeys.ACCOUNT_LIST);
                    UpdateContactInfoSuccessModel successModel = new UpdateContactInfoSuccessModel
                    {
                        MultipleAccountNumberSelected = model.MultipleAccountNumberSelected != null ? model.MultipleAccountNumberSelected : string.Empty,
                        AccountNumberSelected = model.AccountNumberSelected,
                        SelectedBusinessPartnerNumber = model.SelectedBusinessPartnerNumber,
                        EmailAddress = model.EmailAddress,
                        MobileNumber = !string.IsNullOrWhiteSpace(model.MobileNumber) ? model.MobileNumber.AddMobileNumberZeroPrefix() : string.Empty,
                        NickName = model.NickName,
                        SelectedAccountName = fullname
                    };
                    if (response.Payload.Equals("00000000000000000000"))
                    {
                        CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(model.AccountNumberSelected.TrimStart(new char[] { '0' }), Times.Once));
                        CacheProvider.Store(CacheKeys.AccountInformation_Success, new AccessCountingCacheItem<UpdateContactInfoSuccessModel>(successModel, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.myprofile);
                    }
                    else
                    {
                        VerifyMobileNumber verifyMobile = new VerifyMobileNumber
                        {
                            BusinessPartner = model.SelectedBusinessPartnerNumber,
                            Requestnumber = response.Payload,
                            RequestType = model.RequestType,
                            MobileNumber = !string.IsNullOrWhiteSpace(model.MobileNumber) ? model.MobileNumber.AddMobileNumberZeroPrefix() : string.Empty,
                            EmailAddress = !string.IsNullOrWhiteSpace(model.EmailAddress) ? model.EmailAddress : string.Empty,
                            URL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.myprofile),
                            SuccessURL = CustomerProfileSuccessURL.Manage_Account_Information
                        };
                        CacheProvider.Store(CacheKeys.AccountInformation_Success_OTP2, new AccessCountingCacheItem<UpdateContactInfoSuccessModel>(successModel, Times.Once));
                        CacheProvider.Store(CacheKeys.AccountInformation_Request, new CacheItem<VerifyMobileNumber>(verifyMobile));
                        CacheProvider.Store(CacheKeys.AccountInformation_Request_Onetime, new AccessCountingCacheItem<VerifyMobileNumber>(verifyMobile, Times.Max));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.verifymobilenumber_page);
                    }
                }
                else
                {
                    CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(model.AccountNumberSelected.TrimStart(new char[] { '0' }), Times.Once));
                    CacheProvider.Store(CacheKeys.AccountInformation_Error, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.myprofile);
                }
            }
            CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(model.AccountNumberSelected.TrimStart(new char[] { '0' }), Times.Once));
            CacheProvider.Store(CacheKeys.AccountInformation_Error, new AccessCountingCacheItem<string>(error, Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.myprofile);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageAccountV1Ajax(string account)
        {
            ManageAccountInfo model = new ManageAccountInfo();
            if (!string.IsNullOrWhiteSpace(account))
            {
                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(account.TrimStart(new char[] { '0' }), Times.Once));
                var response = DewaApiClient.GetProfileContractAccountDetails(
                    new profileContractAccountInput
                    {
                        contractAccount = FormatContractAccount(account),
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId
                    }, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null && response.Payload.profileContractAccount != null)
                {
                    var accountdetail = response.Payload.profileContractAccount;
                    model = new ManageAccountInfo
                    {
                        AccountNumberSelected = FormatContractAccount(accountdetail.contractAccount),
                        SelectedBusinessPartnerNumber = FormatBusinessPartner(accountdetail.businessPartner),
                        NickName = accountdetail.nickName,
                        EmailAddress = accountdetail.emailAddres,
                        MobileNumber = !string.IsNullOrWhiteSpace(accountdetail.mobile) ? accountdetail.mobile.RemoveMobileNumberZeroPrefix() : string.Empty,
                        IsPrimaryAccount = !string.IsNullOrWhiteSpace(accountdetail.primaryAccount) ? accountdetail.primaryAccount.Equals("X") : false,
                        IsVerifyEmail = !string.IsNullOrWhiteSpace(accountdetail.verifyEmail) ? accountdetail.verifyEmail.Equals("X") : false
                    };

                    model.ProfilePictureBytes = new byte[0];
                    var profilepicResponse = DewaApiClient.GetAccountImage(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, account, RequestLanguage, Request.Segment());
                    if (profilepicResponse != null && profilepicResponse.Succeeded && profilepicResponse.Payload != null)
                    {
                        model.ProfilePictureBytes = profilepicResponse.Payload;
                    }
                    if (response.Payload.communications != null && response.Payload.communications.HasAny())
                    {
                        model.CommunicationList = new System.Collections.Generic.List<ManageAccountCommunication>();
                        Array.ForEach(response.Payload.communications, x => model.CommunicationList.Add(new ManageAccountCommunication
                        {
                            CommunicationType = x.communicationType,
                            IsEmail = !string.IsNullOrWhiteSpace(x.email) && x.email.ToLower().Equals("y") ? true : false,
                            IsSMS = !string.IsNullOrWhiteSpace(x.sms) && x.sms.ToLower().Equals("y") ? true : false,
                        }));
                    }
                    string selectedaccount = model.AccountNumberSelected.TrimStart(new char[] { '0' });
                    string selectedbp = model.SelectedBusinessPartnerNumber.TrimStart(new char[] { '0' });
                    var accountsresponse = GetAccounts(false);
                    if (accountsresponse != null && accountsresponse.Payload != null && accountsresponse.Payload.Count() > 0 && accountsresponse.Payload.Where(x => x.AccountNumber.Equals(account)).Any())
                    {
                        var selectedbp_accountlist = accountsresponse.Payload.ToList().Where(x => x.BusinessPartnerNumber.Equals(selectedbp));
                        var selectedaccountList = accountsresponse.Payload.Where(x => x.AccountNumber.Equals(selectedaccount));
                        if (selectedbp_accountlist != null && selectedbp_accountlist.Count() > 0)
                        {
                            model.Accounts = selectedbp_accountlist.ToList().Where(x => !x.AccountNumber.Equals(selectedaccount)).ToArray();
                        }

                        if (selectedaccountList != null && selectedaccountList.Count() > 0)
                        {
                            var selectedaccountdetails = selectedaccountList.FirstOrDefault();
                            model.Location = selectedaccountdetails.Location;
                            model.Street = selectedaccountdetails.Street;
                            model.PremiseNumber = selectedaccountdetails.PremiseNumber;
                            model.XCordinate = selectedaccountdetails.XCordinate;
                            model.YCordinate = selectedaccountdetails.YCordinate;
                        }
                    }
                    CacheProvider.Store(CacheKeys.AccountInformation_profileContractAccount, new CacheItem<profileContractAccount>(response.Payload.profileContractAccount, TimeSpan.FromMinutes(20)));
                }
            }
            return View("~/Views/Feature/Account/Account/v1/_ManageAccountForm.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult VerifyMobilenumber()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.verifymobilenumber_error, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            VerifyMobileNumber verifyMobile;
            if (CacheProvider.TryGet(CacheKeys.AccountInformation_Request_Onetime, out verifyMobile))
            {
                if (CacheProvider.TryGet(CacheKeys.AccountInformation_Request, out verifyMobile))
                {
                    ViewBag.mobile = verifyMobile.MobileNumber;
                    ViewBag.URL = verifyMobile.URL;
                    return View("~/Views/Feature/Account/Account/v1/VerifyMobileNumber.cshtml", verifyMobile);
                }
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.myprofile);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpPost]
        public ActionResult VerifyMobilenumber(string otp)
        {
            VerifyMobileNumber verifyMobile;
            if (CacheProvider.TryGet(CacheKeys.AccountInformation_Request, out verifyMobile))
            {
                var response = DewaApiClient.SetProfileMobileConfirmation(
                   new profileMobileInput
                   {
                       requestnumber = verifyMobile.Requestnumber,
                       otp = otp,
                       sessionid = CurrentPrincipal.SessionToken,
                       userid = CurrentPrincipal.UserId
                   }, RequestLanguage, Request.Segment());


                if (response != null && response.Succeeded)
                {
                    if (verifyMobile.SuccessURL.Equals(CustomerProfileSuccessURL.Manage_Account_Information))
                    {
                        UpdateContactInfoSuccessModel updateContactInfoSuccessModel;
                        if (CacheProvider.TryGet(CacheKeys.AccountInformation_Success_OTP2, out updateContactInfoSuccessModel))
                        {
                            CacheProvider.Store(CacheKeys.AccountInformation_Success_OTP, new AccessCountingCacheItem<UpdateContactInfoSuccessModel>(updateContactInfoSuccessModel, Times.Once));
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.myprofile);
                    }
                    else if (verifyMobile.SuccessURL.Equals(CustomerProfileSuccessURL.Customer_Profile))
                    {
                        UpdateCustomerInfoSuccessModel updateCustomerInfoSuccessModel;
                        if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_SUCCESS_OTP2, out updateCustomerInfoSuccessModel))
                        {
                            CacheProvider.Store(CacheKeys.CUSTOMER_PROFILE_SUCCESS_OTP, new AccessCountingCacheItem<UpdateCustomerInfoSuccessModel>(updateCustomerInfoSuccessModel, Times.Once));
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.customerProfile);
                    }
                }
                else
                {
                    CacheProvider.Store(CacheKeys.AccountInformation_Request, new CacheItem<VerifyMobileNumber>(verifyMobile));
                    CacheProvider.Store(CacheKeys.AccountInformation_Request_Onetime, new AccessCountingCacheItem<VerifyMobileNumber>(verifyMobile, Times.Max));
                    CacheProvider.Store(CacheKeys.verifymobilenumber_error, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.verifymobilenumber_page);
                }
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.myprofile);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public JsonResult ResendOTP()
        {
            VerifyMobileNumber verifyMobileNumber;
            if (CacheProvider.TryGet(CacheKeys.AccountInformation_Request, out verifyMobileNumber))
            {
                var response = DewaApiClient.SetResendOTP(
                    new otpRequestInput
                    {
                        businesspartner = FormatBusinessPartner(verifyMobileNumber.BusinessPartner),
                        flag = "R",
                        requestnumber = verifyMobileNumber.Requestnumber,
                        sessionid = CurrentPrincipal.SessionToken
                    });
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.responsecode != "0")
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult VerifyEmail(string code)
        {
            ViewBag.success = false;
            if (!string.IsNullOrWhiteSpace(code))
            {
                var response = DewaApiClient.SetProfileEmailConfirmation(
                   new profileEmailInput
                   {
                       code = code,
                       sessionid = CurrentPrincipal.SessionToken,
                       userid = CurrentPrincipal.UserId
                   }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded)
                {
                    ViewBag.success = true;
                    ViewBag.message = response.Message;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "check the URL");
            }
            return View("~/Views/Feature/Account/Account/v1/VerifyEmail.cshtml");
        }

        #endregion Manage Account information V1

        public PartialViewResult AccountSelectorV1(int? page, string q, string currentFilter, string contextid, string checkMoveOut, string ServiceFlag, [System.Web.Http.FromUri] string[] selectedaccounts, string selected_tab = "0")
        {
            int totalPages = 0;
            int[] totalPages2 = { 0, 0 };

            #region Context from sitecore if not then setting default

            ///
            if (string.IsNullOrWhiteSpace(contextid))
            {
                contextid = RenderingContext.Current.Rendering.DataSource;
            }
            var context = ContentRepository.GetItem<AccountSelector>(new GetItemByIdOptions(Guid.Parse(contextid ?? "{A442D390-1266-49C9-858A-D872B07D2320}")));
            var pageNumber = page ?? 1;

            var qs = QueryString.Parse(Request.RawUrl);
            string[] requested = qs.ContainsKey("a") ? qs["a"].Split(',') : new string[0];
            if (selectedaccounts != null && selectedaccounts.Length > 0 && selectedaccounts.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                requested = selectedaccounts;
            }
            else if (!string.IsNullOrEmpty(context.SelectedAccountCacheKey))
            {
                if (requested != null && requested.Length > 0)
                { }
                else
                {
                    string selectedacc = string.Empty;
                    if (CacheProvider.TryGet(context.SelectedAccountCacheKey, out selectedacc))
                    {
                        requested = new string[] { selectedacc };
                    }
                }
            }
            ViewBag.requestedaccounts = requested;
            Foundation.Content.Models.Account[] mappedAccounts = null;
            if (q != null)
            {
                page = 1;
            }
            else
            {
                q = currentFilter;
            }
            ViewBag.CurrentFilter = q;
            //var context = SitecoreContext.GetItem<AccountSelector>(RenderingContext.Current.Rendering.DataSource);

            ViewBag.contextid = contextid;
            int pageSize = context.PageSize != 0 ? context.PageSize : 20;

            #endregion Context from sitecore if not then setting default

            ///For Clearance Certificate Account Select to filter the contract account
            if (!string.IsNullOrEmpty(context.NotificationCode))
            {
                checkMoveOut = context.NotificationCode;
            }

            if (!string.IsNullOrEmpty(context.ServiceFlag))
            {
                ServiceFlag = context.ServiceFlag;
            }

            //if (RenderingContext.CurrentOrNull != null && !string.IsNullOrWhiteSpace(RenderingContext.Current.Rendering.Parameters["moveout"]))
            //{
            //    checkMoveOut = RenderingContext.Current.Rendering.Parameters["moveout"];
            //}
            //This is the dynamic key which will determine from which key the account will be loaded
            string cacheKey = checkMoveOut;
            if (!string.IsNullOrWhiteSpace(ServiceFlag))
            {
                cacheKey = ServiceFlag;
            }
            ViewBag.checkMoveOut = checkMoveOut;

            var accountsresponse = GetBillingAccounts(context.SecondaryDatasource, true, checkMoveOut, cacheKey, ServiceFlag);
            AccountDetails[] accounts = new AccountDetails[0];
            if (accountsresponse.Succeeded)
            {
                if (context.ResidentialAccount && !string.IsNullOrEmpty(contextid) && contextid.Equals("{16735F4F-D84A-402B-BE46-55EA4EFBEEE7}"))
                {
                    accounts = accountsresponse.Payload.Where(x => x.BillingClass == BillingClassification.Residential).ToArray();
                }
                else
                { accounts = accountsresponse.Payload; }
            }

            AccountList model;

            if (accounts != null && accounts.Any())
            {
                ViewBag.isexpocustomer = accounts.FirstOrDefault()?.Isexpocustomer;
                if (context.SecondaryDatasource)
                {
                    if (context.ElectricVehicleAccount)
                    {
                        mappedAccounts = accounts.Select(Foundation.Content.Models.Account.From).ToArray();
                    }
                    else
                    {
                        mappedAccounts = accounts.Where(acc => acc.BillingClass != BillingClassification.ElectricVehicle).Select(Foundation.Content.Models.Account.From).ToArray();
                    }
                }
                else if (context.ElectricVehicleAccount && context.ResidentialAccount && context.NonResidentialAccount)
                {
                    mappedAccounts = accounts.Where(acc => acc.IsActive).Select(Foundation.Content.Models.Account.From).ToArray();
                }
                else if (context.ElectricVehicleAccount)
                {
                    mappedAccounts = accounts.Where(acc => acc.IsActive && acc.BillingClass == BillingClassification.ElectricVehicle).Select(Foundation.Content.Models.Account.From).ToArray();
                }
                else
                {
                    mappedAccounts = accounts.Where(acc => acc.IsActive && acc.BillingClass != BillingClassification.ElectricVehicle).Select(Foundation.Content.Models.Account.From).ToArray();
                }

                // if condition for check accounts is Owner or Tenant
                if (context.Owner == true && context.Tenant == false)
                {
                    mappedAccounts = mappedAccounts.Where(acc => acc.CustomerType == Translate.Text("moveout.owner")).ToArray();
                }
                else if (context.Owner == false && context.Tenant == true)
                {
                    mappedAccounts = mappedAccounts.Where(acc => acc.CustomerType == Translate.Text("moveout.tenant")).ToArray();
                }
                // End

                var _authService = DependencyResolver.Current.GetService<IDewaAuthStateService>();
                var profile = _authService.GetActiveProfile();
                if (profile != null)
                {
                    profile.HasActiveAccounts = mappedAccounts.Any();
                }
                _authService.Save(profile);

                DewaProfile _userProfile = AuthStateService.GetActiveProfile();
                if (_userProfile != null)
                {
                    #region Commented Code -- By Adeel - to fix the performance issue

                    //var mappedPrimary = mappedAccounts.FirstOrDefault(m => m.AccountNumber.Equals(primary.Payload.AccountNumber));

                    #endregion Commented Code -- By Adeel - to fix the performance issue

                    var mappedPrimary = mappedAccounts.FirstOrDefault(m => m.AccountNumber.Equals(_userProfile.PrimaryAccount));
                    if (mappedPrimary != null)
                    {
                        mappedPrimary.Primary = true;
                        mappedPrimary.Selected = requested.Length == 0;
                    }
                }

                foreach (var account in mappedAccounts.OrderByDescending(a => a.Primary))
                {
                    if (account.Selected && !context.MultiSelect) break;

                    account.Selected = false;
                }

                if (mappedAccounts.Any() && !mappedAccounts.Any(a => a.Selected) && !context.MultiSelect)
                {
                    mappedAccounts[0].Selected = true;
                }

                if (mappedAccounts.Any())
                {
                    var accountsarraylist = mappedAccounts.OrderByDescending(a => a.Primary).ToArray();
                    if (requested != null && requested.Length > 0)
                    {
                        var bps = mappedAccounts.Where(x => requested.Contains(x.AccountNumber)).Select(y => y.BusinessPartnerNumber).ToArray();
                        accountsarraylist = mappedAccounts.OrderByDescending(a => bps.Contains(a.BusinessPartnerNumber)).OrderByDescending(a => requested.Contains(a.AccountNumber)).ToArray();
                        accountsarraylist.ToList().ForEach(x => x.Primary = false);
                        accountsarraylist.FirstOrDefault(s => s.Selected = true);
                        accountsarraylist.FirstOrDefault(s => s.Primary = true);
                    }
                    model = context.With(accountsarraylist);
                }
                else
                {
                    model = context.With(Foundation.Content.Models.Account.Null);
                }
            }
            else
            {
                model = context.With(Foundation.Content.Models.Account.Null);
            }

            #region Testing for applying Paging

            IPagedList<Foundation.Content.Models.Account> pagedList = null;
            IPagedList<Foundation.Content.Models.Account>[] pagedList2 = new IPagedList<Foundation.Content.Models.Account>[2];
            //search
            if (!String.IsNullOrEmpty(q))
            {
                q = q.ToLower();
                IEnumerable<Foundation.Content.Models.Account> acc = model.Accounts.Where(s => s.AccountNumber.Contains(q)
                                        || (!string.IsNullOrWhiteSpace(s.Name) && s.Name.ToLower().Contains(q)) || (!string.IsNullOrWhiteSpace(s.NickName) && s.NickName.ToLower().Contains(q)));
                totalPages = Pager.CalculateTotalPages(acc.Count(), pageSize);
                pagedList = acc.ToPagedList<Foundation.Content.Models.Account>(pageNumber, pageSize);

                if (context.MultiSelect)
                {
                    ViewBag.selected_tab = selected_tab;
                    int k = 0;

                    var classGroups = model.Accounts.Where(s => s.AccountNumber.Contains(q)
                                        || (!string.IsNullOrWhiteSpace(s.Name) && s.Name.ToLower().Contains(q)) || (!string.IsNullOrWhiteSpace(s.NickName) && s.NickName.ToLower().Contains(q))).GroupBy(a => a.BillingClassTemp).OrderByDescending(group => group.Key);

                    if (classGroups.Count() == 1 && classGroups.FirstOrDefault().Key != Translate.Text("Residential"))
                    {
                        pagedList2[k] = new PagedList<Foundation.Content.Models.Account>(new List<Foundation.Content.Models.Account>() { Foundation.Content.Models.Account.Null }, pageNumber, pageSize);

                        totalPages2[k] = Pager.CalculateTotalPages(classGroups.FirstOrDefault().Count(), pageSize);
                        k++;
                    }

                    foreach (var classGroup in classGroups)
                    {
                        if (k.ToString() == selected_tab)
                        {
                            pagedList2[k] = new PagedList<Foundation.Content.Models.Account>(classGroup.ToList(), pageNumber, pageSize);

                            totalPages2[k] = Pager.CalculateTotalPages(classGroup.Count(), pageSize);
                        }
                        else
                        {
                            pagedList2[k] = new PagedList<Foundation.Content.Models.Account>(classGroup.ToList(), 1, pageSize);

                            totalPages2[k] = Pager.CalculateTotalPages(classGroup.Count(), pageSize);
                        }
                        k++;
                    }
                }
            }
            else
            {
                totalPages = Pager.CalculateTotalPages(model.Accounts.Count, pageSize);
                pagedList = model.Accounts.ToPagedList(pageNumber, pageSize);
                if (context.MultiSelect)
                {
                    ViewBag.selected_tab = selected_tab;
                    int k = 0;
                    foreach (var classGroup in model.Accounts.GroupBy(acc => acc.BillingClassTemp).OrderByDescending(group => group.Key))
                    {
                        if (k.ToString() == selected_tab)
                        {
                            pagedList2[k] = new PagedList<Foundation.Content.Models.Account>(classGroup.ToList(), pageNumber, pageSize);

                            totalPages2[k] = Pager.CalculateTotalPages(classGroup.Count(), pageSize);
                        }
                        else
                        {
                            pagedList2[k] = new PagedList<Foundation.Content.Models.Account>(classGroup.ToList(), 1, pageSize);

                            totalPages2[k] = Pager.CalculateTotalPages(classGroup.Count(), pageSize);
                        }
                        k++;
                    }
                }
            }

            model.PagedAccounts = pagedList;
            model.PagedAccountsArray = pagedList2;

            #endregion Testing for applying Paging

            var currentItem = ContextRepository.GetCurrentItem<Item>();
            ViewBag.IsRTL = currentItem != null && currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;
            if (page != null)
            {
                if (context.MultiSelect)
                {
                    return PartialView("~/Views/Feature/Account/Selector/M43 Multi Selection Account List.cshtml", (AccountSelector)model);
                }
                else
                {
                    return PartialView("~/Views/Feature/Account/Selector/M43 Single Selection Account List.cshtml", (AccountSelector)model);
                }
            }
            if (context.DashboardPage)
            {
                return PartialView("~/Views/Feature/Dashboard/Dashboard/MainDashboard.cshtml", (AccountSelector)model);
            }
            return PartialView("~/Views/Feature/Account/Selector/M43 Account Selector.cshtml", (AccountSelector)model);
        }

        [HttpGet]
        public PartialViewResult subAccountSelector(int? page, string q, string currentFilter, string contextid)
        {
            int totalPages = 0;
            int pageSize = 20;
            var pageNumber = page ?? 1;
            var qs = QueryString.Parse(Request.RawUrl);
            var requested = qs.ContainsKey("a") ? qs["a"].Split(',') : new string[0];
            if (q != null)
            {
                page = 1;
            }
            else
            {
                q = currentFilter;
            }
            ViewBag.CurrentFilter = q;
            //var context = SitecoreContext.GetItem<AccountSelector>(RenderingContext.Current.Rendering.DataSource);

            #region Testing for applying paging

            ///

            var context = ContentRepository.GetItem<AccountSelector>(new GetItemByIdOptions(Guid.Parse(contextid ?? "{A442D390-1266-49C9-858A-D872B07D2320}")));
            ViewBag.contextid = contextid;

            #endregion Testing for applying paging

            var accountsresponse = GetAccounts(false);
            AccountDetails[] accounts = new AccountDetails[0];
            if (accountsresponse.Succeeded)
            {
                accounts = accountsresponse.Payload;
            }
            AccountList model;
            if (accounts != null && accounts.Any())
            {
                var mappedAccounts = accounts
                    .Where(acc => acc.IsActive)
                    .Select(Foundation.Content.Models.Account.From)
                    .ToArray();
                var _authService = DependencyResolver.Current.GetService<IDewaAuthStateService>();
                var profile = _authService.GetActiveProfile();
                if (profile != null)
                {
                    profile.HasActiveAccounts = mappedAccounts.Any();
                }
                _authService.Save(profile);

                #region Commented Code -- By Adeel - to fix the performance issue

                //var primary = DewaApiClient.GetPrimaryAccount(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                //if (primary.Succeeded && primary.Payload != null)

                #endregion Commented Code -- By Adeel - to fix the performance issue

                DewaProfile _userProfile = AuthStateService.GetActiveProfile();
                if (_userProfile != null)
                {
                    #region Commented Code -- By Adeel - to fix the performance issue

                    //var mappedPrimary = mappedAccounts.FirstOrDefault(m => m.AccountNumber.Equals(primary.Payload.AccountNumber));

                    #endregion Commented Code -- By Adeel - to fix the performance issue

                    var mappedPrimary = mappedAccounts.FirstOrDefault(m => m.AccountNumber.Equals(_userProfile.PrimaryAccount));
                    if (mappedPrimary != null)
                    {
                        mappedPrimary.Primary = true;
                        mappedPrimary.Selected = requested.Length == 0;
                    }
                }

                foreach (var account in mappedAccounts.OrderByDescending(a => a.Primary))
                {
                    if (account.Selected && !context.MultiSelect) break;

                    account.Selected = requested.Contains(account.AccountNumber);
                }

                if (!mappedAccounts.Any(a => a.Selected))
                {
                    mappedAccounts[0].Selected = true;
                }

                model = context.With(mappedAccounts.OrderByDescending(a => a.Primary).ToArray());
            }
            else
            {
                model = context.With(Foundation.Content.Models.Account.Null);
            }

            #region Testing for applying Paging

            IPagedList<Foundation.Content.Models.Account> pagedList = null;
            //search
            if (!String.IsNullOrEmpty(q))
            {
                IEnumerable<Foundation.Content.Models.Account> acc = model.Accounts.Where(s => s.AccountNumber.Contains(q)
                                       || s.Name.Contains(q));
                totalPages = Pager.CalculateTotalPages(acc.Count(), pageSize);
                pagedList = acc.ToPagedList(pageNumber, pageSize);
            }
            else
            {
                totalPages = Pager.CalculateTotalPages(model.Accounts.Count, pageSize);
                pagedList = model.Accounts.ToPagedList(pageNumber, pageSize);
            }
            //

            // IPagedList<Account> pagedList = model.Accounts.ToPagedList<Account>(pageNumber, pageSize);
            model.PagedAccounts = pagedList;

            #endregion Testing for applying Paging

            var currentItem = ContextRepository.GetCurrentItem<Item>();
            ViewBag.IsRTL = currentItem != null && currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;
            return PartialView("~/Views/Feature/Account/Selector/M43 Single Selection Account List.cshtml", (AccountSelector)model);
        }

        public PartialViewResult BillSelector(int? page, string q, string currentFilter, string contextid)
        {
            int totalPages = 0;
            //int pageSize = 20;
            var pageNumber = page ?? 1;
            var qs = QueryString.Parse(Request.RawUrl);
            var requested = qs.ContainsKey("a") ? qs["a"].Split(',') : new string[0];
            if (q != null)
            {
                page = 1;
            }
            else
            {
                q = currentFilter;
            }
            ViewBag.CurrentFilter = q;
            var accountsresponse = GetBillingAccounts(true, true, string.Empty);
            AccountDetails[] accounts = new AccountDetails[0];
            if (accountsresponse.Succeeded)
            {
                accounts = accountsresponse.Payload;
            }
            if (contextid == null)
            {
                contextid = RenderingContext.Current.Rendering.DataSource;
            }
            var context = ContentRepository.GetItem<BillSelector>(new GetItemByIdOptions(Guid.Parse(contextid != null ? contextid : "{A442D390-1266-49C9-858A-D872B07D2320}")));
            ViewBag.contextid = contextid;
            int pageSize = context.PageSize != 0 ? context.PageSize : 20;
            IPagedList<Foundation.Content.Models.Account> pagedList = null;
            string[] selection;
            if (CacheProvider.TryGet(CacheKeys.SELECTED_BILL_LIST, out selection))
            {
                if (!context.AllowSelection)
                {
                    accounts = accounts.RemoveWhere(acc => !selection.Contains(acc.AccountNumber)).ToArray();
                }
            }
            selection = selection ?? new string[0];

            var mapped = accounts
                //.Where(acc => acc.IsActive)
                //.Where(acc => acc.IsActive || (acc.Balance > 0 && !acc.IsActive))
                .Select(Foundation.Content.Models.Account.From)
                .ToArray();

            var model = context.With(mapped);
            if (context.AllowSelection || context.Editable)
            {
                foreach (var account in model)
                {
                    account.Selected = (!selection.Any() || selection.Contains(account.AccountNumber));
                }
            }
            if (!String.IsNullOrEmpty(q))
            {
                q = q.ToLower();
                IEnumerable<Foundation.Content.Models.Account> acc = model.Accounts.Where(s => s.AccountNumber.Contains(q)
                                        || (!string.IsNullOrWhiteSpace(s.Name) && s.Name.ToLower().Contains(q)) || (!string.IsNullOrWhiteSpace(s.NickName) && s.NickName.ToLower().Contains(q)));
                totalPages = Pager.CalculateTotalPages(acc.Count(), pageSize);
                pagedList = acc.ToPagedList(pageNumber, pageSize);
            }
            else
            {
                totalPages = Pager.CalculateTotalPages(model.Accounts.Count, pageSize);
                pagedList = model.Accounts.ToPagedList(pageNumber, pageSize);
            }
            model.PagedAccounts = pagedList;

            if (page != null)
            {
                return PartialView("~/Views/Feature/Account/Selector/M44 Bill Selection List.cshtml", (BillSelector)model);
            }
            return PartialView("~/Views/Feature/Account/Selector/M44 Bill Selector.cshtml", (BillSelector)model);
        }

        #region Method(s)

        private List<SelectListItem> GetMasterData(dewaSVC.masterData[] data, string keyName)
        {
            return data.ToList().Where(x => x.componentName == keyName).Select(y => new SelectListItem { Text = y.valueName, Value = y.keyName }).ToList();
        }

        private bool TryLogin(LoginModel model, out string error, out string lockerror)
        {
            error = null;
            lockerror = string.Empty;
            var _fc = FetchFutureCenterValues();

            //var response = DewaApiClient.AuthenticateNew(model.Username, model.Password, RequestLanguage, Request.Segment(), _fc.Branch);
            var response = SmartCustomerClient.LoginUser(
                new LoginRequest
                {
                    getloginsessionrequest = new Getloginsessionrequest
                    {
                        userid = model.Username,
                        password = Base64Encode(model.Password),
                        center = _fc.Branch
                    }
                }, RequestLanguage, Request.Segment());

            if (response.Succeeded)
            {
                var primaryAccountNumber = string.Empty;
                var emailAddress = string.Empty;
                var mobileNumber = string.Empty;
                var fullName = string.Empty;
                var termsAndConditions = string.Empty;
                var isContactUpdated = false;
                var BusinessPartner = string.Empty;
                var PopupFlag = false;

                if (response.Payload != null && !String.IsNullOrEmpty(response.Payload.AccountNumber))
                {
                    primaryAccountNumber = response.Payload.AccountNumber;

                    #region Removing Primary account Call - Commented by Adeel

                    //var contactResponse = DewaApiClient.GetContactDetails(response.Payload, primaryAccountNumber, RequestLanguage);
                    //if (contactResponse.Succeeded)
                    //{

                    #endregion Removing Primary account Call - Commented by Adeel

                    emailAddress = response.Payload.Email;
                    mobileNumber = response.Payload.Mobile;
                    BusinessPartner = response.Payload.BusinessPartner;
                    fullName = response.Payload.FullName;
                    PopupFlag = response.Payload.PopupFlag;
                }

                termsAndConditions = response.Payload.AcceptedTerms ? "X" : string.Empty;
                isContactUpdated = response.Payload.IsUpdateContact;
                string userid;
                if (CacheProvider.TryGet(CacheKeys.MOVEIN_USERID, out userid))
                {
                    isContactUpdated = true;
                }
                var dewaprofile = new DewaProfile(model.Username, response.Payload.SessionNumber)
                {
                    BusinessPartner = BusinessPartner,
                    PrimaryAccount = primaryAccountNumber,
                    EmailAddress = emailAddress,
                    FullName = fullName,
                    MobileNumber = mobileNumber,
                    HasActiveAccounts = true,
                    TermsAndConditions = termsAndConditions,
                    IsContactUpdated = isContactUpdated,
                    PopupFlag = PopupFlag
                };

                if (response.Payload != null && String.IsNullOrWhiteSpace(response.Payload.AccountNumber))
                {
                    var accountsresponse = SmartCustomerClient.GetCAList(dewaprofile.UserId, dewaprofile.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());
                    if (accountsresponse != null && accountsresponse.Succeeded && accountsresponse.Payload != null)
                    {
                        CacheProvider.Store(CacheKeys.ACCOUNT_LIST, new CacheItem<ServiceResponse<AccountDetails[]>>(accountsresponse, TimeSpan.FromHours(1)));
                        if (!accountsresponse.Payload.Where(acc => acc.BillingClass != BillingClassification.ElectricVehicle).HasAny()
                                && accountsresponse.Payload.Where(acc => acc.BillingClass == BillingClassification.ElectricVehicle).HasAny())
                        {
                            //primaryAccountNumber = accountsresponse.Payload.Where(acc => acc.BillingClass == BillingClassification.ElectricVehicle).FirstOrDefault().AccountNumber;
                            dewaprofile = new DewaProfile(model.Username, response.Payload.SessionNumber)
                            {
                                BusinessPartner = BusinessPartner,
                                PrimaryAccount = primaryAccountNumber,
                                EmailAddress = emailAddress,
                                FullName = fullName,
                                MobileNumber = mobileNumber,
                                HasActiveAccounts = true,
                                TermsAndConditions = termsAndConditions,
                                IsContactUpdated = isContactUpdated,
                                PopupFlag = PopupFlag,
                                IsEVUser = true
                            };
                        }
                    }
                }
                AuthStateService.Save(dewaprofile);
                //StoreProfilePhoto(model.Username, response.Payload);

                return true;
            }
            else if (!response.Succeeded && response.Payload != null && response.Payload.ResponseCode.Equals("116"))
            {
                lockerror = response.Message;
                CacheProvider.Store(CacheKeys.ForgotPassword_Step + ("unlock"), new AccessCountingCacheItem<string>("5", Times.Once));
                CacheProvider.Store(CacheKeys.ForgotPassword_Username, new AccessCountingCacheItem<string>(model.Username, Times.Once));
            }
            error = response.Message;
            return false;
        }

        private bool TryLoginAsGovernmentUser(LoginModel model, out string error)
        {
            error = null;

            var response = DewaApiClient.Authenticate(model.Username, model.Password, true, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                AuthStateService.Save(new DewaProfile(model.Username, response.Payload, Roles.Government)
                {
                    TermsAndConditions = "X"
                });
                StoreProfilePhoto(model.Username, response.Payload);

                return true;
            }
            error = response.Message;
            return false;
        }

        //TODO: need to move to generic function
        private byte[] ReadImage(HttpPostedFileBase upload)
        {
            byte[] imgData;

            using (Stream inputStream = upload.InputStream)
            {
                var memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }

                imgData = memoryStream.ToArray();
            }

            return imgData;
        }

        //private string GetImageMimeType(byte[] bytes)
        //{
        //    using (var ms = new MemoryStream(bytes))
        //    {
        //        var image = Image.FromStream(ms);

        //        if (image.RawFormat.Equals(ImageFormat.Bmp))
        //        {
        //            return "image/bmp";
        //        }
        //        if (image.RawFormat.Equals(ImageFormat.Jpeg))
        //        {
        //            return "image/jpeg";
        //        }
        //        if (image.RawFormat.Equals(ImageFormat.Png))
        //        {
        //            return "image/png";
        //        }
        //        if (image.RawFormat.Equals(ImageFormat.Tiff))
        //        {
        //            return "image/tiff";
        //        }
        //        if (image.RawFormat.Equals(ImageFormat.Icon))
        //        {
        //            return "image/ico";
        //        }
        //    }
        //    return string.Empty;
        //}
        private void ClearSessionAndSignOut()
        {
            DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
        }

        private void ClearAccountCache()
        {
            CacheProvider.Remove(CacheKeys.ACCOUNT_LIST_WITH_BILLING);
            CacheProvider.Remove(CacheKeys.ACCOUNT_LIST);
        }

        private string GetAccountPicsPhysicalUploadPath(string fileName = null)
        {
            const string UPLOAD_PATH = "~/upload/account_thumbs";
            var path = Server.MapPath(UPLOAD_PATH);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return Path.Combine(path, fileName);
            }
            return path;
        }

        private void RemoveCachedImage(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public bool PostLoginCredentialsToBot(string userName, string sessionId, string channelId, string conversationId, string surl, string activityId, string fromId, string fromName, string toId, string toName, string languageCode, out string error, int magicNumber, string loginType = "authentication")
        {
            try
            {
                bool IsSuccess = false;
                ICredentials credentials = new NetworkCredential(WebConfigurationManager.AppSettings["PROXYUSER"], WebConfigurationManager.AppSettings["PROXYPASSWORD"]);
                HttpClientHandler handler = new HttpClientHandler()
                {
                    Proxy = new WebProxy(WebConfigurationManager.AppSettings["PROXYURL"], true, null, credentials),
                    UseProxy = true
                };
                //After login sucess need to post call for bot controller i.e; below code
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(handler))
                {
                    var baseAddress = WebConfigurationManager.AppSettings["BotUrl"]; //TODO: Set main bot url here
                    httpClient.BaseAddress = new Uri(baseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //var jwtToken = new JwtManager().GenerateToken(userName, sessionId, channelId, conversationId, surl, activityId, fromId, fromName, toId, toName, languageCode,magicNumber, loginType, 60);
                    var jwtToken = new JwtManager().GenerateToken(userName, sessionId, channelId, conversationId, languageCode, magicNumber, loginType, 60);
                    HttpResponseMessage response = httpClient.PostAsJsonAsync(new Uri(baseAddress), jwtToken).Result;

                    IsSuccess = response.IsSuccessStatusCode;
                    error = response.ReasonPhrase;
                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    ViewBag.Message = "Error: Something went wrong.";
                    //}
                    //else
                    //{
                    //    ViewBag.Message = "You have been successfully authenticated.";
                    //}
                }
                return IsSuccess;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                error = ex.InnerException.ToString();
                return false;
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtp(CustomerInfoSendOtp request)
        {
            string errVerf = "";
            try
            {
                dewaSVC.profileMain profileMainData = null;
                if (CacheProvider.TryGet(CacheKeys.CUSTOMER_PROFILE_MAIN, out profileMainData))
                {
                    if (profileMainData != null && request != null && !string.IsNullOrEmpty(request.type))
                    {
                        //Send or Verify otp
                        var response = SmartCommunicationClient.CustomerVerifyOtp(new SmartCommunicationVerifyOtpRequest()
                        {
                            mode = request.mode,
                            sessionid = CurrentPrincipal.SessionToken,
                            reference = (!string.IsNullOrEmpty(request.mode) && request.mode.Equals("V") && !string.IsNullOrEmpty(request.reqId)) ? request.reqId : string.Empty,
                            prtype = request.prtype,
                            mobile = (!string.IsNullOrEmpty(request.type) && request.type.Equals("email")) ? profileMainData.mobileNumber : string.Empty,
                            email = (!string.IsNullOrEmpty(request.type) && request.type.Equals("mobile")) ? profileMainData.emailAddress : string.Empty,
                            contractaccountnumber = string.Empty,
                            businesspartner = request.bpno,
                            otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : null
                        }, Request.Segment(), RequestLanguage); ;
                        if (response != null && response.Succeeded)
                        {
                            return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            errVerf = response.Message;
                        }
                        return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = ex.Message;
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtpAccount(CustomerInfoSendOtp request)
        {
            string errVerf = "";
            try
            {
                dewaSVC.profileContractAccount profileContractAccount = null;
                if (CacheProvider.TryGet(CacheKeys.AccountInformation_profileContractAccount, out profileContractAccount))
                {
                    if (profileContractAccount != null && request != null && !string.IsNullOrEmpty(request.type))
                    {
                        //Send or Verify otp
                        var response = SmartCommunicationClient.CustomerVerifyOtp(new SmartCommunicationVerifyOtpRequest()
                        {
                            mode = request.mode,
                            sessionid = CurrentPrincipal.SessionToken,
                            reference = (!string.IsNullOrEmpty(request.mode) && request.mode.Equals("V") && !string.IsNullOrEmpty(request.reqId)) ? request.reqId : string.Empty,
                            prtype = request.prtype,
                            mobile = (!string.IsNullOrEmpty(request.type) && request.type.Equals("email")) ? profileContractAccount.mobile : string.Empty,
                            email = (!string.IsNullOrEmpty(request.type) && request.type.Equals("mobile")) ? profileContractAccount.emailAddres : string.Empty,
                            contractaccountnumber = FormatContractAccount(profileContractAccount.contractAccount),
                            businesspartner = FormatBusinessPartner(profileContractAccount.businessPartner),
                            otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : null
                        }, Request.Segment(), RequestLanguage); ;
                        if (response != null && response.Succeeded)
                        {
                            return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            errVerf = response.Message;
                        }
                        return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = ex.Message;
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }

        #endregion Method(s)
    }
}