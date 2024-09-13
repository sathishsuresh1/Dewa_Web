// <copyright file="uaepasscontroller.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Account.Controllers
{
    using DEWAXP.Feature.Account.Models.UAEPass;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Content.Services;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.APIHandler.Config;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Requests;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Diagnostics;
    using global::Sitecore.Globalization;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Configuration;
    using System.Net;
    using System.Web.Configuration;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="UAEPassController" />
    /// </summary>
    public class UAEPassController : BaseController
    {
        /// <summary>
        /// The Login
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet]
        public ActionResult Login(string returnUrl, string isFrame = "", SiteEnumHelper.UAEPassuser? uType = SiteEnumHelper.UAEPassuser.consumer)
        {
            if (!string.IsNullOrWhiteSpace(isFrame))
            {
                CacheProvider.Store(CacheKeys.UAEPASS_returnUrl, new CacheItem<string>(returnUrl + "?isFrame=true"));
                CacheProvider.Store(CacheKeys.UAEPASS_USC_returnUrl, new CacheItem<bool>(true));
            }
            else
                CacheProvider.Store(CacheKeys.UAEPASS_returnUrl, new CacheItem<string>(returnUrl));

            if (IsLoggedIn && CurrentPrincipal.IsMyIdUser)
            {
                string _returnUrl;
                CacheProvider.TryGet(CacheKeys.UAEPASS_returnUrl, out _returnUrl);
                if (!string.IsNullOrWhiteSpace(_returnUrl))
                {
                    if (Url.IsLocalUrl(_returnUrl))
                    {
                        return Redirect(_returnUrl);
                    }
                    else
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                    }
                }
                else
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                }
            }
            string lang = RequestLanguage.Code();
            CacheProvider.Store(CacheKeys.UAEPASS_USER_TYPE, new AccessCountingCacheItem<SiteEnumHelper.UAEPassuser?>(uType, Times.Once));
            var loginurl = string.Format(UAEPassConfig.UAEPASS_OPENID_ACCESSCODE, UAEPassConfig.UAEPASS_RETURNOAUTHURL, UAEPassConfig.UAEPASS_CLIENT_ID, UAEPassConfig.UAEPASS_STATE, lang.ToLower());
            return Redirect(loginurl);
        }

        /// <summary>
        /// The oauthCallback
        /// </summary>
        /// <param name="code">The code<see cref="string"/></param>
        /// <param name="state">The state<see cref="string"/></param>
        /// <param name="error">The error<see cref="string"/></param>
        /// <param name="errordescription">The errordescription<see cref="string"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet]
        public ActionResult oauthCallback(string code = "", string state = "", string error = "", string errordescription = "")
        {
            if (!string.IsNullOrWhiteSpace(state) && state.Equals(UAEPassConfig.UAEPASS_STATE))
            {
                SiteEnumHelper.UAEPassuser? uType = null;
                if (!string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(error))
                {
                    CacheProvider.TryGet(CacheKeys.UAEPASS_USER_TYPE, out uType);
                    switch (uType)
                    {
                        case SiteEnumHelper.UAEPassuser.careerportal:

                            #region [code]

                            var uaePassLoginData = UAEPassServiceClient.UAEPASSDubaiIdLogin(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.UAEPassService.UAEPassDubaiIdLoginRequest()
                            {
                                code = code,
                            }, Request.Segment(), RequestLanguage);
                            if (uaePassLoginData.Succeeded && uaePassLoginData.Payload != null)
                            {
                                var _cUserData = uaePassLoginData.Payload;
                                AuthStateService.Save(new DewaProfile(_cUserData.aliasname, _cUserData.sessionid, Roles.Jobseeker)
                                {
                                    FullName = RequestLanguage == SupportedLanguage.English ? _cUserData.fullname_en : _cUserData.fullname_ar,
                                    EmailAddress = _cUserData.email,
                                    MobileNumber = _cUserData.mobile,
                                    EmiratesIdentifier = _cUserData.idn,
                                    IsRegister = _cUserData.registered,
                                    IsContactUpdated = true
                                });
                                CacheProvider.Store("careerusepass", new AccessCountingCacheItem<bool>(true, Times.Once));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                            }

                            return HandleAuthenticationJobseekerError(uaePassLoginData.Message);

                        #endregion [code]

                        default:

                            #region [code]
                            var uaePassCustomerAuthentication = UAEPassServiceClient.UAEPASSCustomerAuthentication(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.UAEPassService.UAEPassDubaiIdLoginRequest()
                            {
                                code = code,
                            });

                            if (uaePassCustomerAuthentication != null && uaePassCustomerAuthentication.Succeeded && uaePassCustomerAuthentication.Payload !=null)
                            {
                                if (uaePassCustomerAuthentication.Payload.access_token != null && uaePassCustomerAuthentication.Payload.token_type != null)
                                {
                                    var uaePassCustomerData = UAEPassServiceClient.UAEPASSCustomerData((string)uaePassCustomerAuthentication.Payload.access_token,
                                        (string)uaePassCustomerAuthentication.Payload.token_type);
                                    if (uaePassCustomerData != null && uaePassCustomerData.Succeeded &&!string.IsNullOrWhiteSpace(uaePassCustomerData.Payload))
                                    {
                                        var result = UAEPassInfo.FromJson(uaePassCustomerData.Payload);
                                        if (!string.IsNullOrEmpty(result.Idn) && result.UserType != "SOP1")
                                        {
                                            string _myIdEmirate = string.Format("MYID{0}", result.Idn);
                                            var termsAndConditions = string.Empty;
                                            var fullName = string.Empty;
                                            var emailAddress = string.Empty;
                                            var mobileNumber = string.Empty;
                                            var BusinessPartner = string.Empty;
                                            var isContactUpdated = false;
                                            string username = !string.IsNullOrWhiteSpace(result.FirstnameEn) ? result.FirstnameEn : !string.IsNullOrWhiteSpace(result.FirstnameAr) ? result.FirstnameAr : string.Empty;
                                            try
                                            {
                                                var setDubaiIDDetailsResponse = DewaApiClient.SetDubaiIDDetails(new DEWAXP.Foundation.Integration.DewaSvc.loginHelper
                                                {
                                                    dob = result.Dob,
                                                    firstname_AR = result.FirstnameAr,
                                                    firstname_EN = result.FirstnameEn,
                                                    fullname = result.FullnameEn,
                                                    fullname_AR = result.FullnameAr,
                                                    gender = result.Gender,
                                                    idcardexpirydate = result.IdCardExpiryDate,
                                                    idcardissuedate = result.IdCardIssueDate,
                                                    idcardnumber = "",
                                                    idn = result.Idn,
                                                    lastname_AR = result.LastnameAr,
                                                    lastname_EN = result.LastnameEn,
                                                    mail = result.Email,
                                                    mobile = result.Mobile,
                                                    nationality = result.NationalityEn,
                                                    password = "",
                                                    photo = result.Photo,
                                                    tfn = "",
                                                    timestamp = "",
                                                    userid = result.Email,
                                                    uuid = result.Uuid
                                                }, RequestLanguage, Request.Segment());
                                            }
                                            catch (Exception)
                                            {
                                                Log.Info("UAE Pass -> Logging service failed.", this);
                                            }
                                            var authResponse = DewaApiClient.Authenticate(result.Idn, RequestLanguage, Request.Segment());
                                            if (authResponse.Succeeded && authResponse.Payload != null)
                                            {
                                                Log.Info("UAE Pass -> DEWA authentication succeeded.", this);

                                                if (authResponse.Payload.CredentialIsKnown)
                                                {
                                                    var primaryAccountResponse = DewaApiClient.GetPrimaryAccount(_myIdEmirate, authResponse.Payload.Credential, RequestLanguage, Request.Segment());
                                                    string primaryAccountNo = string.Empty;
                                                    if (primaryAccountResponse.Succeeded && primaryAccountResponse.Payload != null)
                                                    {
                                                        primaryAccountNo = primaryAccountResponse.Payload.AccountNumber;
                                                        termsAndConditions = primaryAccountResponse.Payload.AcceptedTerms;
                                                        isContactUpdated = primaryAccountResponse.Payload.IsUpdateContact;
                                                        fullName = primaryAccountResponse.Payload.FullName;
                                                        BusinessPartner = primaryAccountResponse.Payload.BusinessPartner;
                                                        emailAddress = primaryAccountResponse.Payload.Email;
                                                        mobileNumber = primaryAccountResponse.Payload.Mobile;
                                                    }
                                                    var profile = new DewaProfile(username.Trim(), authResponse.Payload.Credential)
                                                    {
                                                        PrimaryAccount = primaryAccountNo,
                                                        EmiratesIdentifier = result.Idn,
                                                        IsMyIdUser = true,
                                                        TermsAndConditions = termsAndConditions,
                                                        IsContactUpdated = isContactUpdated,
                                                        FullName = fullName,
                                                        BusinessPartner = BusinessPartner,
                                                        EmailAddress = emailAddress,
                                                        MobileNumber = mobileNumber
                                                    };

                                                    AuthStateService.Save(profile);
                                                    string _returnUrl;
                                                    CacheProvider.TryGet(CacheKeys.UAEPASS_returnUrl, out _returnUrl);
                                                    if (!string.IsNullOrWhiteSpace(_returnUrl))
                                                    {
                                                        if (Url.IsLocalUrl(_returnUrl))
                                                        {
                                                            return Redirect(_returnUrl);
                                                        }
                                                        else
                                                        {
                                                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                                                    }
                                                }
                                                CacheProvider.Store(CacheKeys.UAEPASSINFO, new CacheItem<UAEPassInfo>(result));

                                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LINK_UAEPASS_TO_BP);
                                            }

                                            Log.Warn(string.Format("UAE Pass-> DEWA authentication FAILED: [{0}]", authResponse.Message), this);

                                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(authResponse.Message, Times.Once));

                                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                                        }
                                        else
                                        {
                                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("UAE_NO_EmiratesID"), Times.Once));
                                            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["UAEPASS_Logout"]))
                                            {
                                                if (Uri.IsWellFormedUriString(WebConfigurationManager.AppSettings["UAEPASS_Logout"].ToString(), UriKind.Absolute))
                                                {
                                                    return Redirect(WebConfigurationManager.AppSettings["UAEPASS_Logout"].ToString());
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return HandleAuthenticationError(uaePassCustomerData.Message);
                                    }
                                }
                            }
                            else
                            {
                                    return HandleAuthenticationError(uaePassCustomerAuthentication.Message);
                            }

                            #endregion [code]

                            break;
                    }
                }
                return HandleAuthenticationError(Translate.Text(error));
            }
            return HandleAuthenticationError(Translate.Text("Unexpected Error"));
        }

        [HttpGet]
        public ActionResult Link()
        {
            UAEPassInfo info;
            if (!CacheProvider.TryGet(CacheKeys.UAEPASSINFO, out info))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            string username = !string.IsNullOrWhiteSpace(info.FirstnameEn) ? info.FirstnameEn : !string.IsNullOrWhiteSpace(info.FirstnameAr) ? info.FirstnameAr : string.Empty;
            string email = !string.IsNullOrWhiteSpace(info.Email) ? info.Email : "";
            string mobile = !string.IsNullOrWhiteSpace(info.Mobile) ? info.Mobile : "";
            string[] input_str = { "971", "+971", "0" };
            foreach (var item in input_str)
            {
                if (mobile.Length > item.Length && mobile.StartsWith(item))
                {
                    mobile = mobile.Remove(0, item.Length);
                }
            }

            return PartialView("~/Views/Feature/Account/UAEPass/_Link.cshtml", new LinkBusinessPartnerToMyIdModel()
            {
                EmiratesIdentifier = info.Idn,
                MyId = username,
                EmailAddress = email,
                MobileNumber = mobile
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Link(LinkBusinessPartnerToMyIdModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Feature/Account/UAEPass/_Link.cshtml", model);
            }

            var request = new LinkBusinessPartnerToMyId
            {
                BusinessPartnerNumber = model.BusinessPartnerNumber,
                EmiratesIdentifier = model.EmiratesIdentifier,
                EmailAddress = model.EmailAddress,
                MobileNumber = model.MobileNumber,
                PoBox = model.PoBox,
                MyIdUsername = model.MyId
            };

            var response = CustomerServiceClient.LinkBusinessPartnerToMyId(request, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                var profile = new DewaProfile(model.MyId, response.Payload)
                {
                    EmiratesIdentifier = model.EmiratesIdentifier,
                    IsMyIdUser = true
                };

                AuthStateService.Save(profile);

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
            }

            ModelState.AddModelError(string.Empty, response.Message);

            return PartialView("~/Views/Feature/Account/UAEPass/_Link.cshtml", model);
        }

        /// <summary>
        /// The HandleAuthenticationError
        /// </summary>
        /// <param name="error">The error<see cref="string"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        private ActionResult HandleAuthenticationError(string error)
        {
            Log.Warn(string.Format("UAE Pass authentication error response: [{0}]", error), this);
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
        }

        private ActionResult HandleAuthenticationJobseekerError(string error)
        {
            Log.Warn(string.Format("UAE Pass authentication error response: [{0}]", error), this);
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
        }
    }
}