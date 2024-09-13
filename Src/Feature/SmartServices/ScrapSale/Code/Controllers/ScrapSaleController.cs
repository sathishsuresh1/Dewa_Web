using DEWAXP.Foundation.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecorex = Sitecore.Context;
using ScData = Sitecore.Data;

namespace DEWAXP.Feature.ScrapSale.Controllers
{
    using DEWAXP.Feature.ScrapSale.Filters;
    using DEWAXP.Feature.ScrapSale.Helpers;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Filters.Mvc;
    using DEWAXP.Foundation.Content.Models.MoveOut;
    using DEWAXP.Foundation.Content.Models.Payment;
    using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
    using DEWAXP.Foundation.Content.Models.ScrapSale;
    using DEWAXP.Foundation.Content.Models.SupplyManagement.MoveOut;
    using DEWAXP.Foundation.Content.Models.TariffCalculator;
    using DEWAXP.Foundation.Content.Models.UpdateIBAN;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Content.Services;
    using DEWAXP.Foundation.Content.Utils;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.APIHandler.Clients;
    using DEWAXP.Foundation.Integration.APIHandler.Impl;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Masar;
    using DEWAXP.Foundation.Integration.CorporatePortal;
    using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.JobSeekerSvc;
    using DEWAXP.Foundation.Integration.KhadamatechDEWASvc;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.GraphSvc;
    using DEWAXP.Foundation.Integration.Responses.QmsSvc;
    using DEWAXP.Foundation.Integration.Responses.VendorSvc;
    using DEWAXP.Foundation.Integration.Responses.VillaCostExemption;
    using DEWAXP.Foundation.Logger;
    using Models.ScrapSale;
    using Sitecore.ContentSearch.ComputedFields;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Publishing.Explanations;
    using System.DirectoryServices.ActiveDirectory;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using WebGrease.Activities;
    using static DEWAXP.Foundation.Helpers.SystemEnum;
    using static Sitecore.Configuration.State;
    using static Sitecore.ContentSearch.Linq.Extensions.ReflectionExtensions;
 

    //using System.Web.Security;
    using _AccountSModel = Foundation.Content.Models.AccountModel;

    public class ScrapSaleController : BaseController
    {
        // GET: ScrapSale

        #region [Actions Controller]

        #region [Accounts]

        #region [PreLogin]
        public string _ReturnURL
        {
            get
            {
                string _responseData = string.Empty;
                CacheProvider.TryGet("return_URI", out _responseData);
                return _responseData;
            }
            set
            {
                CacheProvider.Store("return_URI", new CacheItem<string>(value, TimeSpan.FromDays(30)));
            }
        }

        public string _sType
        {
            get
            {
                string _responseData = string.Empty;
                CacheProvider.TryGet("return_sType", out _responseData);
                return _responseData;
            }
            set
            {
                CacheProvider.Store("return_sType", new CacheItem<string>(value, TimeSpan.FromDays(30)));
            }
        }

        public ActionResult Login(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                _ReturnURL = returnUrl.ToLower();
            else
                _ReturnURL = string.Empty;//MasarConfig.ScrapSaleDashboardURL;


            LoginModel model = new LoginModel();
            if (!string.IsNullOrEmpty(_ReturnURL))
                model.LoginType = _ReturnURL.Contains(MasarConfig.ScrapSaleDashboardURL) ? MasarConfig.ScrapSales : MasarConfig.Miscellaneous;
            else
                model.LoginType = string.Empty;

            model.AfterLoginURL = _ReturnURL;

            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = Translate.Text("defaultSelect"), Value = "" });
            data.AddRange(GetDataSource(SitecoreItemIdentifiers.MISCELLANEOUS_SERVICE_LIST));
            model.MiscellaneousServices = data;


            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.ScrapeSale) && _ReturnURL.ToLower().Contains(MasarConfig.ScrapSaleDashboardURL))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
            }
            else if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.Miscellaneous) && _ReturnURL.ToLower().Contains(MasarConfig.ElectricMeterTesting))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_NEW_CONNECTION);
            }
            else if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.Miscellaneous) && _ReturnURL.ToLower().Contains(MasarConfig.ElectricMeterTestingDEWAProjects))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_PROJECT);
            }
            else if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.Miscellaneous) && _ReturnURL.ToLower().Contains(MasarConfig.TransformerOilTesting))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.OIL_TESTING);
            }
            else if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.Miscellaneous) && _ReturnURL.ToLower().Contains(MasarConfig.JointerCertificationFormURL))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING);
            }


            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            //if (string.IsNullOrEmpty(returnUrl))
            //{
            //    ClearSessionAndSignOut();
            //}
            // if (string.IsNullOrEmpty(_ReturnURL))
            //   return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);


            return View("~/Views/Feature/ScrapSale/ScrapSale/Login.cshtml", model);


        }

        [HttpPost, AntiForgeryHandleError, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string error = string.Empty, sessionId;
                    ServiceResponse<MasarLoginResponse> response;

                    if (!string.IsNullOrEmpty(model.MiscellaneousServicesKey))
                        model.LoginType = model.MiscellaneousServicesKey == MasarConfig.ScrapSales ? MasarConfig.ScrapSales : MasarConfig.Miscellaneous;

                    _sType = model.LoginType;

                    if (!string.IsNullOrEmpty(model.MiscellaneousServicesKey) && model.MiscellaneousServicesKey == MasarConfig.ScrapSales)
                        _ReturnURL = MasarConfig.ScrapSaleDashboardURL;
                    else if (!string.IsNullOrEmpty(model.MiscellaneousServicesKey) && (model.MiscellaneousServicesKey == MasarConfig.TestingServices || model.MiscellaneousServicesKey == MasarConfig.JointerCertification))
                        _ReturnURL = MasarConfig.Miscellaneous;

                    if (TryLoginWithScrapeSale(model, out response, out sessionId))
                    {
                        returnUrl = HttpUtility.UrlDecode(returnUrl);
                        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                    }



                    if (response.Payload != null && response.Payload.responsecode != "000")
                    {
                        if (response.Payload.responsecode == "115")
                            ModelState.AddModelError(string.Empty, Translate.Text("SC_DoesNotHaveRole"));
                        else
                            ModelState.AddModelError(string.Empty, response.Message?.Replace(Translate.Text("InvalidLogin"), Translate.Text("SC_InvalidLogin")));

                    }
                    else if (response.Payload == null)
                        ModelState.AddModelError(string.Empty, response.Message?.Replace(Translate.Text("InvalidLogin"), Translate.Text("SC_InvalidLogin")));
                    else
                    {
                        if (!string.IsNullOrEmpty(model.MiscellaneousServicesKey))
                        {
                            if (model.MiscellaneousServicesKey == MasarConfig.ScrapSales)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_INTROPAGE);
                            }
                            else if (model.MiscellaneousServicesKey == MasarConfig.TestingServices)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.TESTING_SERVICES);
                            }
                            else if (model.MiscellaneousServicesKey == MasarConfig.JointerCertification)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_CERTIFICATION);
                            }
                        }
                        else
                        {
                            if (model.AfterLoginURL.ToLower().Contains(MasarConfig.ScrapSaleDashboardURL))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
                            }
                            else if (model.AfterLoginURL.ToLower().Contains(MasarConfig.ElectricMeterTesting))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_NEW_CONNECTION);
                            }
                            else if (model.AfterLoginURL.ToLower().Contains(MasarConfig.ElectricMeterTestingDEWAProjects))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_PROJECT);
                            }
                            else if (model.AfterLoginURL.ToLower().Contains(MasarConfig.TransformerOilTesting))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.OIL_TESTING);
                            }
                            else if (model.AfterLoginURL.ToLower().Contains(MasarConfig.JointerCertificationFormURL))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING);
                            }
                        }

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_DASHBOARD);
                    }
                }

                List<SelectListItem> data = new List<SelectListItem>();
                data.Add(new SelectListItem() { Text = Translate.Text("defaultSelect"), Value = "" });
                data.AddRange(GetDataSource(SitecoreItemIdentifiers.MISCELLANEOUS_SERVICE_LIST));
                model.MiscellaneousServices = data;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Login.cshtml", model);
        }

        [HttpGet]
        public ActionResult ForgotUsername()
        {
            string _custType = string.Empty;

            if (Request.QueryString["t"] != null)
                _custType = Request.QueryString["t"].ToString() == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(MasarConfig.ScrapType)) ? MasarConfig.ScrapSales : Request.QueryString["t"].ToString() == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(MasarConfig.MiscellaneousType)) ? MasarConfig.Miscellaneous : string.Empty;
            else
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);

            ForgotUsernameModel model = new ForgotUsernameModel();
            model.CustomerType = _custType;

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/ForgotUsername.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotUsername(ForgotUsernameModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Forgotiddetails resetCred = new Forgotiddetails()
                    {
                        businesspartnernumber = model.CustomerNumber,
                        email = model.Email,
                        customertype = model.CustomerType
                    };



                    var response = MasarClient.MasarForgotUserName(new MasarForgotUserNameRequest()
                    {
                        forgotiddetails = resetCred
                    }, RequestLanguage, Request.Segment());



                    if (response != null && response.Succeeded)
                    {
                        var recoveryEmailModel = new _AccountSModel.RecoveryEmailSentModel
                        {
                            EmailAddress = model.Email,
                            Context = _AccountSModel.RecoveryContext.Username
                        };

                        CacheProvider.Store(CacheKeys.RECOVERY_EMAIL_STATE, new CacheItem<_AccountSModel.RecoveryEmailSentModel>(recoveryEmailModel));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_RECOVERY_EMAIL_SENT);
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    //ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            //ViewBag.Modal = SitecoreContext.GetItem<ModalOverlay>(Guid.Parse("{6DFBF80F-1B61-4332-9E35-EB5860434E6B}"));

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/ForgotUsername.cshtml", model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {


            ForgotPasswordModel model = new ForgotPasswordModel();

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/ForgotPassword.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //model.Username = StringExtensions.GetSanitizePlainText(model.Username);
                    //model.Email = StringExtensions.GetSanitizePlainText(model.Email);
                    //model.OtpRequestId = StringExtensions.GetSanitizePlainText(model.OtpRequestId);
                    //model.ReferenceId = StringExtensions.GetSanitizePlainText(model.ReferenceId);
                    //model.Password = StringExtensions.GetSanitizePlainText(model.Password);

                    Resetcredentialdetails resetCred = new Resetcredentialdetails()
                    {
                        //otp = model.OtpRequestId,
                        reference = model.ReferenceId,
                        password = model.Password,
                        userid = model.Username
                    };



                    var response = MasarClient.MasarForgotPassword(new MasarForgetPasswordRequest()
                    {
                        resetcredentialdetails = resetCred
                    }, Request.Segment(), RequestLanguage);

                    if (response != null && response.Succeeded)
                    {
                        if (response.Payload != null && response.Payload.responsecode == "000")
                        {
                            model.isPasswordSetSuccessful = true;
                            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/SetPasswordSuccessful.cshtml");
                        }
                        else
                            //ModelState.AddModelError(string.Empty, response.Message);
                            ModelState.AddModelError(string.Empty, response.Message);
                    }
                    else
                        //ModelState.AddModelError(string.Empty, response.Message);
                        ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/ForgotPassword.cshtml", model);
        }


        [HttpGet]
        public ActionResult SetNewPassword(string d)
        {
            try
            {
                Decryptdetails decryptCred = new Decryptdetails()
                {
                    key = d,
                    processtype = MasarConfig.NonBilling
                };



                var response = MasarClient.MasarCreateUserDecryptGUID(new MasarDecryptGUIDRequest()
                {
                    decryptdetails = decryptCred
                }, Request.Segment(), RequestLanguage);

                if (response != null && response.Succeeded)
                {
                    if (response.Payload != null)
                    {
                        CacheProvider.Store("SecEmail", new CacheItem<string>(response.Payload.email, TimeSpan.FromHours(1)));
                        CacheProvider.Store("SecMobile", new CacheItem<string>(response.Payload.mobile, TimeSpan.FromHours(1)));

                        return PartialView("~/Views/Feature/ScrapSale/ScrapSale/SetNewPassword.cshtml", new SetNewPasswordModel
                        {
                            Key = d,
                            Email = response.Payload.maskedemail,
                            Mobile = response.Payload.maskedmobile,
                            MaskedEmail = response.Payload.maskedemail,
                            MaskedMobile = response.Payload.maskedmobile,
                            OtpRequestId = response.Payload.referencenumber,
                            MaxAttemptflag = response.Payload.maxattemptflag,
                            SMSDuration = response.Payload.smsduration,
                            EmailDuration = response.Payload.emailduration,
                            isSucess = true

                        });
                    }
                    else
                        //ModelState.AddModelError(string.Empty, response.Message);
                        ModelState.AddModelError(string.Empty, response.Message);
                }
                else
                    //ModelState.AddModelError(string.Empty, response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/SetNewPassword.cshtml", new SetNewPasswordModel
            {

            });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(SetNewPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.isSucess = false;
                    UserCredentialRequest credReq = new UserCredentialRequest()
                    {
                        linkid = model.Key,
                        otp = model.Otp,
                        reference = model.OtpRequestId,
                        userid = model.Username,
                        password = model.Password,
                        processtype = MasarConfig.NonBilling
                    };



                    var response = MasarClient.MasarCreateUserCredential(new MasarCreateUserCredentialRequest()
                    {
                        newuserinputs = credReq
                    }, RequestLanguage, Request.Segment());

                    if (response != null && response.Succeeded)
                    {
                        return PartialView("~/Views/Feature/ScrapSale/ScrapSale/SetUserNamePasswordSuccessful.cshtml");
                    }

                    //ModelState.AddModelError(string.Empty, response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/SetNewPassword.cshtml", model);
        }

        #endregion [PreLogin]

        #region [Account Registration]

        [HttpGet]
        public ActionResult TrackApplication(string AppNo)
        {
            try
            {
                if (!string.IsNullOrEmpty(AppNo))
                {
                    Trackinputs credReq = new Trackinputs()
                    {
                        referencenumber = AppNo
                    };

                    var response = MasarClient.MasarTrackApplication(new MasarTrackApplicationRequest()
                    {
                        trackinputs = credReq
                    }, RequestLanguage, Request.Segment());

                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        string _tptype = string.Empty;

                        if (!string.IsNullOrEmpty(response.Payload.workflowstatus) && response.Payload.workflowstatus.ToUpper() == "INPR")
                            _tptype = "1";
                        else if (!string.IsNullOrEmpty(response.Payload.workflowstatus) && response.Payload.workflowstatus.ToUpper() == "CLAR")
                            _tptype = "2";
                        else
                            _tptype = "1";

                        EncryptDecrypt _encryption = new EncryptDecrypt();
                        string _encryptedLink = _encryption.EncryptTextURL(_tptype + "|" + AppNo, MasarConfig._zStr);
                        string linkquerystring = !string.IsNullOrEmpty(_tptype) ? LinkHelper.GetItemUrl(SitecoreItemIdentifiers.MISCELLANEOUS_USER_REGISTRATION) + "?rn=" + _encryptedLink : LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SCRAPESALE_USER_REGISTRATION);

                        var model = new UserRegistrationModel()
                        {
                            ApplicationNO = response.Payload.requestid,
                            Status = response.Payload.workflowstatus,
                            RejectionReason = response.Payload.reason,
                            AppSubmittedDate = !string.IsNullOrEmpty(response.Payload.createdat)
                                ? DateTime.ParseExact(response.Payload.createdat, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)
                                : DateTime.MinValue,
                            CurrentWorkflowDate = !string.IsNullOrEmpty(response.Payload.workflowat)
                                ? DateTime.ParseExact(response.Payload.workflowat, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)
                                : DateTime.MinValue,
                            EncryptedLink = linkquerystring,
                            WorkingDays = response.Payload.sla,
                            EstimatedServiceCompletionDate = response.Payload.sladate

                        };
                        return PartialView("~/Views/Feature/ScrapSale/ScrapSale/TrackApplication.cshtml", model);
                    }

                    //ModelState.AddModelError(string.Empty, response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                else
                {
                    return PartialView("~/Views/Feature/ScrapSale/ScrapSale/TrackApplication.cshtml", new UserRegistrationModel());
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }


            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/TrackApplication.cshtml", new UserRegistrationModel());
        }


        [HttpGet]
        public ActionResult UserRegistration()
        {
            string _custType = string.Empty;
            string defaultPhoneCore = "AE";
            string _appNo = string.Empty;
            string _tp = string.Empty;
            if (Request.QueryString["t"] != null)
                _custType = Request.QueryString["t"].ToString() == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(MasarConfig.ScrapType)) ? MasarConfig.ScrapSales : Request.QueryString["t"].ToString() == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(MasarConfig.MiscellaneousType)) ? MasarConfig.Miscellaneous : string.Empty;
            else if (Request.QueryString["rn"] != null)
            {
                EncryptDecrypt _encryption = new EncryptDecrypt();
                string _decryptedLink = _encryption.DecryptText(Request.QueryString["rn"].ToString(), MasarConfig._zStr);

                if (!string.IsNullOrEmpty(_decryptedLink))
                {
                    _tp = _decryptedLink.Split('|')[0];
                    _appNo = _decryptedLink.Split('|')[1];
                }
                else
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);
            }
            else
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);

            var model = new UserRegistrationModel()
            {
                CountryList = GetCountry(),
                RegionList = GetRegion("AE"),
                Countrykey = "AE",
                MobileCode = defaultPhoneCore,
                TelephoneCode = defaultPhoneCore,
                CompanyTelephoneCode = defaultPhoneCore,
                CompanyMobileCode = defaultPhoneCore,
                AreaList = GetArea(""),
                CompanyTelephoneCodeList = GetTelephoneCountryCode(),
                CustomerType = _custType,
                //TradeLicenseIssuingAuthority = GetLstDataSource(DataSources.CUSTOMER_PROFILE_TRADE_LICENSE_ISSUING_AUTHOR).ToList()
                TradeLicenseIssuingAuthority = GetIssuedBy().ToList(),
                RequestNumber = _appNo,
                ViewResubmit = _tp

            };

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/UserRegistration.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserRegistration(UserRegistrationModel model)
        {
            if (string.IsNullOrEmpty(model.CustomerType))
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);

            List<Attachmentlist> registrationAttachments = new List<Attachmentlist>();
            bool isAttachmentError = false;

            string error = string.Empty;

            if (model.EIDAttachment != null)
            {
                string _ext = !string.IsNullOrEmpty(model.EIDAttachment.FileName) ? Path.GetExtension(model.EIDAttachment.FileName).ToLower() : string.Empty;

                if (!AttachmentIsValid(model.EIDAttachment, General.MaxAttachmentSize, out error, General.AcceptedMasarFileTypes))
                {
                    isAttachmentError = true;
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    registrationAttachments.Add(new Attachmentlist()
                    {
                        filename = model.EIDAttachment.FileName,
                        filedata = Convert.ToBase64String(model.EIDAttachment.ToArray()),
                        mimetype = _ext == ".pdf" ? "application/pdf" : _ext == ".jpg" ? "image/jpg" : _ext == ".jpeg" ? "image/jpg" : _ext == ".png" ? "image/png" : string.Empty,
                        attachmenttype = "EM",
                    });
                }
            }


            if (model.TradeLicenseAttachment != null)
            {
                string _ext = !string.IsNullOrEmpty(model.TradeLicenseAttachment.FileName) ? Path.GetExtension(model.TradeLicenseAttachment.FileName).ToLower() : string.Empty;

                if (!AttachmentIsValid(model.TradeLicenseAttachment, General.MaxAttachmentSize, out error, General.AcceptedMasarFileTypes))
                {
                    isAttachmentError = true;
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    registrationAttachments.Add(new Attachmentlist()
                    {
                        filename = model.TradeLicenseAttachment.FileName,
                        filedata = Convert.ToBase64String(model.TradeLicenseAttachment.ToArray()),
                        mimetype = _ext == ".pdf" ? "application/pdf" : _ext == ".jpg" ? "image/jpg" : _ext == ".jpeg" ? "image/jpg" : _ext == ".png" ? "image/png" : string.Empty,
                        attachmenttype = "TL",
                    });
                }
            }


            if (registrationAttachments.Count() < 1 && !model.isEIDAFetch && !model.isTradeLicenseFetch && string.IsNullOrEmpty(model.RequestNumber))
            {
                ModelState.AddModelError("0001", Translate.Text("SS_UR_AttachmentRequired"));
                ModelState.AddModelError("", Translate.Text("SS_UR_AttachmentRequired"));
                isAttachmentError = true;
            }

            if (!string.IsNullOrEmpty(model.TradeLicenseIssueDate))
                model.TradeLicenseIssueDate = model.TradeLicenseIssueDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            if (!string.IsNullOrEmpty(model.TradeLicenseExpiryDate))
                model.TradeLicenseExpiryDate = model.TradeLicenseExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            if (!string.IsNullOrEmpty(model.ExpiryDate))
                model.ExpiryDate = model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");


            if (ModelState.IsValid && !isAttachmentError)
            {
                var _mobNumber = model.UserType == "O" ? model.CompanyMobileNumber : model.MobileNumber;
                if (_mobNumber[0] != '0')
                    _mobNumber = "0" + _mobNumber;

                var _telephoneCode = model.UserType == "O" ? model.CompanyTelephoneCode : model.TelephoneCode;
                if (string.IsNullOrEmpty(model.IndividualCompanyTelephone) && string.IsNullOrEmpty(model.CompanyTelephone))
                    _telephoneCode = string.Empty;

                model.Countrykey = model.UserType == "I" ? "AE" : model.Countrykey;

                var registrationInfoRequest = new RegistrationInfo()
                {
                    country = model.UserType == "I" ? "AE" : model.Countrykey,
                    citycode = model.Countrykey == "AE" ? model.City : model.ActualCity,
                    email = model.UserType == "O" ? model.CompanyEmail : model.EmailAddress,
                    emiratesid = model.UserType == "I" ? model.EmiratesID : string.Empty,
                    firstname = model.UserType == "O" ? string.Empty : model.FirstName,
                    companyname = model.UserType == "O" ? model.CompanyName : string.Empty,
                    lastname = model.LastName,
                    mobile = _mobNumber, //!string.IsNullOrWhiteSpace(model.MobileNumber) ? Convert.ToInt64(model.MobileNumber).ToString("0000000000") : "",
                    pobox = model.POBox,
                    region = model.Region,
                    street = model.Street,
                    telephone = model.UserType == "O" ? model.CompanyTelephone : model.IndividualCompanyTelephone,
                    extension = model.UserType == "O" ? model.CompanyTelephoneExtension : model.IndividualCompanyTelephoneExtension,
                    title = model.UserType == "O" ? MasarConfig.Company : MasarConfig.Mr,
                    tradelicense = model.UserType == "O" ? model.TradelicenseNumber : string.Empty,
                    tradelicensevalidfrom = model.UserType == "O" ? !string.IsNullOrEmpty(model.TradeLicenseIssueDate) ? DateTime.ParseExact(model.TradeLicenseIssueDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                    tradelicensevalidto = model.UserType == "O" ? !string.IsNullOrEmpty(model.TradeLicenseExpiryDate) ? DateTime.ParseExact(model.TradeLicenseExpiryDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                    issueauthoritytext = model.UserType == "O" ? model.IssuingAuthorityDescription : string.Empty,
                    //customer category 2 for Organization and 1 for Inidividual
                    customercategory = model.UserType == "O" ? MasarConfig.Organization : MasarConfig.Individual,
                    customertype = model.CustomerType,//_ReturnURL == MasarConfig.ScrapSaleDashboardURL ? MasarConfig.ScrapSales : MasarConfig.Miscellaneous,
                    issueauthority = model.UserType == "O" ? model.TradeLicenseIssuingAuthorityKey : string.Empty,
                    requesttype = string.IsNullOrEmpty(model.RequestNumber) ? MasarConfig.CreateProfile : MasarConfig.ResubmitProfile,
                    eidexpirydate = model.UserType == "I" ? !string.IsNullOrEmpty(model.ExpiryDate) ? DateTime.ParseExact(model.ExpiryDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                    mobiledialcode = model.UserType == "O" ? model.CompanyMobileCode : model.MobileCode,
                    telephonedialcode = _telephoneCode,
                    vatregistrationnumber = model.UserType == "O" ? model.VatRegistrationNo : string.Empty,
                    sessionid = "",
                    userid = "",
                    testflag = string.Empty,
                    noattachflag = registrationAttachments.Count() == 0 ? "X" : string.Empty,
                    applicationnumber = !string.IsNullOrEmpty(model.RequestNumber) ? model.RequestNumber : string.Empty

                };

                var response = MasarClient.ScrapRegistration(new MasarUserRegistrationRequest()
                {
                    customerinputs = registrationInfoRequest

                }, RequestLanguage, Request.Segment());



                if (response != null && response.Succeeded && response.Payload.responsecode != null)
                {
                    if (registrationAttachments.Count() > 0)
                    {
                        var AttachmentDetailsRequest = new Attachmentinputs()
                        {
                            appno = response.Payload.requestnumber,
                            lastdoc = "X",
                            userid = "",
                            sessionid = "",
                            requesttype = string.IsNullOrEmpty(model.RequestNumber) ? MasarConfig.CreateProfile : MasarConfig.ResubmitProfile,
                            reference = model.OtpRequestId,
                            attachmentlist = registrationAttachments
                        };


                        var attachmentResponse = MasarClient.AddMasarAttachments(new MasarAttachmentRequest()
                        {
                            attachmentinputs = AttachmentDetailsRequest

                        }, RequestLanguage, Request.Segment());

                        if (attachmentResponse != null && attachmentResponse.Succeeded && attachmentResponse.Payload.responsecode != null)
                        {
                            model.ApplicationNO = response.Payload.requestnumber;
                            model.IsSucceed = true;
                        }
                        else
                        {
                            model.IsValidationPassed = true;
                            // ModelState.AddModelError("", Translate.Text("SS_S1_GeneralError"));
                            //ModelState.AddModelError("", attachmentResponse.Message);
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                    else
                    {
                        model.ApplicationNO = response.Payload.requestnumber;
                        model.IsSucceed = true;
                    }

                }
                else if (response.Payload != null)
                {
                    model.IsValidationPassed = true;
                    ModelState.AddModelError("", response.Payload.description);

                }
                else if (response.Message != null)
                {
                    model.IsValidationPassed = true;
                    // ModelState.AddModelError("", response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            model.CountryList = GetCountry();
            model.RegionList = GetRegion(model.Countrykey);
            model.AreaList = GetArea("");
            model.CompanyTelephoneCodeList = GetTelephoneCountryCode();
            model.TradeLicenseIssuingAuthority = GetIssuedBy().ToList();

            if (!string.IsNullOrEmpty(model.Countrykey) && model.CountryList != null)
            {
                model.CountryList.Where(x => x.Value == "AE").FirstOrDefault().Selected = true;
            }
            model.RegionList = GetRegion(model.Countrykey);
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/UserRegistration.cshtml", model);
        }

        [HttpGet]
        public ActionResult AccountRegistrationStep2(string cno, bool d = true)
        {
            AccountRegistrationStep2Model model = new AccountRegistrationStep2Model();

            string eCno = "";
            if (d)
            {
                //decryption
                eCno = cno;
                cno = Base64Decode(cno);
                model.ConsumerNo = cno;
            }

            var response = CustomerSmartSaleClient.GetRegistrationScrapCustomerDetails(new GetRegistrationScrapCustomerDetails()
            {
                customernumber = cno,
            }, RequestLanguage, Request.Segment());

            if (response.Succeeded && response.Payload != null)
            {
                var data = response.Payload.@return.scrapcustomerdetails;

                if (!string.IsNullOrWhiteSpace(data.maskedemail) || !string.IsNullOrWhiteSpace(data.maskedmobile))
                {
                    model.MaskedEmail = data.maskedemail;
                    model.MaskedMobile = data.maskedmobile;

                    CacheProvider.Store("dewa_account_step", new CacheItem<AccountRegistrationStep2Model>(model));
                }
                else
                {
                    ModelState.AddModelError("", "No Masked Data available");
                }
            }
            else
            {
                //ModelState.AddModelError("", response.Message);
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/AccountRegistrationStep2.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AccountRegistrationStep2(AccountRegistrationStep2Model model)
        {
            if (!string.IsNullOrWhiteSpace(model.SendBY))
            {
                var response = CustomerSmartSaleClient.SendVerifyScrapRegistrationCode(new SendVerifyScrapRegistrationCode()
                {
                    businesspartnernumber = model.ConsumerNo,
                    emailflag = model.SendBY == "e" ? "X" : null,
                    mobileflag = model.SendBY == "m" ? "X" : null,
                    verifyflag = "S"
                }, RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null)
                {
                    //model.OtpTrigger = true;
                    CacheProvider.Store("dewa_account_step", new CacheItem<AccountRegistrationStep2Model>(model));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_AccountRegistrationStep3);
                }
                else
                {
                    //ModelState.AddModelError("", response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", Translate.Text("SS_S1_GeneralError"));
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/AccountRegistrationStep2.cshtml", model);
        }

        public ActionResult AccountRegistrationStep3()
        {
            AccountRegistrationStep2Model model = null;
            if (CacheProvider.TryGet("dewa_account_step", out model))
            {
                return PartialView("~/Views/Feature/ScrapSale/ScrapSale/AccountRegistrationStep3.cshtml", model);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_AccountRegistrationStep2);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AccountRegistrationStep3(AccountRegistrationStep2Model model)
        {
            if (model.ActionType == "otp")
            {
                var response = CustomerSmartSaleClient.SendVerifyScrapRegistrationCode(new SendVerifyScrapRegistrationCode()
                {
                    businesspartnernumber = model.ConsumerNo,
                    emailflag = model.SendBY == "e" ? "X" : null,
                    mobileflag = model.SendBY == "m" ? "X" : null,
                    verifyflag = "S"
                }, RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null)
                {
                    return PartialView("~/Views/Feature/ScrapSale/ScrapSale/AccountRegistrationStep3.cshtml", model);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.OtpNumber))
            {
                CacheProvider.Store("dewa_account_step", new CacheItem<AccountRegistrationStep2Model>(model));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_AccountRegistrationStep4);
            }
            ModelState.AddModelError("OtpNumber", Translate.Text("OTP is required"));
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/AccountRegistrationStep3.cshtml", model);
        }

        [HttpGet]
        public ActionResult AccountRegistrationStep4()
        {
            AccountRegistrationStep2Model _stepData = null;
            if (CacheProvider.TryGet("dewa_account_step", out _stepData))
            {
                /*get value from prevous steps*/
                SetDewaScrapeAccountRegistrationModel model = new SetDewaScrapeAccountRegistrationModel()
                {
                    OtpNumber = _stepData.OtpNumber,
                    MaskedEmail = _stepData.MaskedEmail,
                    MaskedMobile = _stepData.MaskedMobile,
                    ConsumerNo = _stepData.ConsumerNo,
                    //OtpTrigger = _stepData.OtpTrigger,
                    SendBY = _stepData.SendBY,
                };

                return PartialView("~/Views/Feature/ScrapSale/ScrapSale/AccountRegistrationStep4.cshtml", model);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_AccountRegistrationStep2);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AccountRegistrationStep4(SetDewaScrapeAccountRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var userIDCheckResponse = CustomerSmartSaleClient.GetUserIDCheck(new GetUserIDCheck()
                {
                    userid = model.UserName,
                }, RequestLanguage, Request.Segment());

                if (userIDCheckResponse.Succeeded)
                {
                    var response = CustomerSmartSaleClient.SetScrapCustomerAccountRegistration(new SetScrapCustomerAccountRegistration()
                    {
                        confirmpassword = model.ConfirmedPassword,
                        password = model.Password,
                        newuserid = model.UserName,
                        customernumber = model.ConsumerNo,
                        verificationcode = model.OtpNumber
                    }, RequestLanguage, Request.Segment());

                    if (response.Succeeded && response.Payload != null)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_AccountRegistrationSuccess);
                    }
                    else
                    {
                        //ModelState.AddModelError("", response.Message);
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
                else
                {
                    //ModelState.AddModelError("", userIDCheckResponse.Message);
                    ModelState.AddModelError(string.Empty, userIDCheckResponse.Message);
                }
            }

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/AccountRegistrationStep4.cshtml", model);
        }

        #endregion [Account Registration]

        #endregion [Accounts]

        [HttpGet]
        public ActionResult OpenTenderDocument()
        {
            List<openTender> openTendersList = null;

            if (!CacheProvider.TryGet(CacheKeys.SCRAP_SALE_OPEN_TENDER_LIST, out openTendersList))
            {
                var reponseData = CustomerSmartSaleClient.GetOpenTenderList(new GetOpenTenderList()
                {
                    //date = "20200310"
                }, RequestLanguage, Request.Segment());

                if (reponseData != null &&
                    reponseData.Succeeded &&
                    reponseData.Payload?.@return.opentender != null &&
                    reponseData.Payload?.@return.opentender.Count() > 0)
                {
                    openTendersList = reponseData.Payload?.@return.opentender.ToList();
                    CacheProvider.Store(CacheKeys.SCRAP_SALE_OPEN_TENDER_LIST, new CacheItem<List<openTender>>(openTendersList));
                }
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/OpenTenderDocument.cshtml", openTendersList);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadTenderAdvertisement(string tno)
        {
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";

            var response = CustomerSmartSaleClient.GetTenderAdvertisement(new GetTenderAdvertisement()
            {
                tendernumber = tno
            }, RequestLanguage, Request.Segment());

            if (response != null && response.Payload != null && response.Succeeded
                && response.Payload.@return.document != null && response.Payload.@return.document.Length > 0)
            {
                downloadFile = response.Payload.@return.document;
                fileName = "TenderAd_{0}.pdf";
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(tno ?? Guid.NewGuid().ToString())));
            return File(downloadFile, "application/pdf", _fileName);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadTenderDocument(string tno)
        {
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";
            string fileMimeType = "application/pdf";

            var file = GetTenderDocFiles(tno);
            if (file != null)
            {
                fileName = file.FileNameWithExtension;
                downloadFile = file.FileBytes;
                fileMimeType = file.MimeTypeExtension;
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(tno ?? Guid.NewGuid().ToString())));
            return File(downloadFile, fileMimeType, _fileName);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ViewTenderReciept(string tno)
        {
            var response = CustomerSmartSaleClient.GetTenderReceipt(new GetTenderReceipt()
            {
                referencenumber = tno,
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken,
            }, RequestLanguage, Request.Segment());

            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";

            if (response != null && response.Payload != null && response.Succeeded
                && response.Payload.@return.receipt != null && response.Payload.@return.receipt.Length > 0)
            {
                downloadFile = response.Payload.@return.receipt;
                fileName = "TenderReciept_{0}.pdf";
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(tno ?? Guid.NewGuid().ToString())));
            return File(downloadFile, "application/pdf", _fileName);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderPurchaseHistory()
        {
            //var response1 = CustomerSmartSaleClient.GetTenderReferenceNumber(new GetTenderReferenceNumber()
            //{
            //    tendernumber = "000000003012000024",
            //    consumernumber = CurrentPrincipal.BusinessPartner,
            //    amount = "500.00",
            //    transactiontype = "TF"
            //}, RequestLanguage, Request.Segment());

            openTenderList model = new openTenderList();

            var response = CustomerSmartSaleClient.GetTenderListPurchasedHistory(new GetTenderListPurchasedHistory()
            {
                consumernumber = CurrentPrincipal.PrimaryAccount,
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken
            }, RequestLanguage, Request.Segment());

            if (response != null && response.Succeeded && response.Payload != null)
            {
                model = response.Payload.@return;
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/TenderPurchaseHistory.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult PostLogInOpenTenderList()
        {
            openTenderList model = new openTenderList();

            var response = CustomerSmartSaleClient.GetLoginOpenTenderList(new GetLoginOpenTenderList()
            {
                consumernumber = CurrentPrincipal.PrimaryAccount,
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken
            }, RequestLanguage, Request.Segment());

            if (response != null && response.Succeeded && response.Payload != null)
            {
                model = response.Payload.@return;
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/PostLogInOpenTenderList.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderPurchase(string t = "", string r = "")
        {
            ScrapeTenderPaymentModel model = null;

            #region [Payment Detail]

            if (!string.IsNullOrWhiteSpace(r))
            {
                if (CacheProvider.TryGet("TenderOnlinePurchaseDetail", out model))
                {
                    if (model != null && model.IsSuccess)
                    {
                        if (!string.IsNullOrWhiteSpace(model.referencenumber))
                        {
                            var responseD = CustomerSmartSaleClient.GetSSTenderPayStatus(new GetSSTenderPayStatus()
                            {
                                consumernumber = CurrentPrincipal.PrimaryAccount,
                                tendernumber = model.transactionNumber,
                                userid = CurrentPrincipal.UserId,
                                sessionid = CurrentPrincipal.SessionToken,
                            }, RequestLanguage, Request.Segment());

                            if (responseD.Succeeded && responseD.Payload != null && responseD.Payload.@return.paymentstatus == "X")
                            {
                                model.referencenumber = responseD.Payload.@return.referencenumber;
                            }
                        }

                        return View("~/Views/Feature/ScrapSale/ScrapSale/TenderPurchaseAlready.cshtml", model);
                    }
                    else
                    {
                        model.IsError = true;
                        ModelState.AddModelError("", model.Message);
                    }
                }

                if (model == null)
                {
                    //model.IsError = true;
                    ModelState.AddModelError("", "Session Out for TransactionId" + r);
                }
                return View("~/Views/Feature/ScrapSale/ScrapSale/TenderPurchase.cshtml", model);
            }

            #endregion [Payment Detail]

            model = new ScrapeTenderPaymentModel();
            string _msg = "No Tender Number found.";
            openTender openTender = null;

            if (!string.IsNullOrWhiteSpace(t))
            {
                var response = CustomerSmartSaleClient.GetOpenTenderListRequested(new GetOpenTenderListRequested()
                {
                    consumernumber = CurrentPrincipal.PrimaryAccount,
                    tendernumber = t,
                    //date = _date,
                    userid = CurrentPrincipal.UserId,
                    sessionid = CurrentPrincipal.SessionToken,
                }, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded)
                {
                    openTender = response.Payload.@return.opentender.FirstOrDefault();
                }
                else
                {
                    _msg = response.Message;
                }
            }

            if (openTender != null)
            {
                model.closingDate = openTender.closingDate;
                model.floatingDate = openTender.floatingDate;
                model.gotoLink = openTender.gotoLink;
                model.referencenumber = openTender.referencenumber;
                model.rfxStatus = openTender.rfxStatus;
                model.serialNumber = openTender.serialNumber;
                model.tenderFeeAmount = openTender.tenderFeeAmount;
                model.transactionDescription = openTender.transactionDescription;
                model.transactionNumber = openTender.transactionNumber;
                model.type = openTender.type;

                //check its paid or Not

                var response = CustomerSmartSaleClient.GetSSTenderPayStatus(new GetSSTenderPayStatus()
                {
                    consumernumber = CurrentPrincipal.PrimaryAccount,
                    tendernumber = openTender.transactionNumber,
                    userid = CurrentPrincipal.UserId,
                    sessionid = CurrentPrincipal.SessionToken,
                }, RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null)
                {
                    var _d = response.Payload.@return;

                    model.PaymentStatus = Convert.ToBoolean(_d.paymentstatus == "X");
                    model.referencenumber = _d.referencenumber;

                    if (model.PaymentStatus)
                    {
                        return View("~/Views/Feature/ScrapSale/ScrapSale/TenderPurchaseAlready.cshtml", model);
                    }

                    CacheProvider.Store("TenderPurchaseDetail", new CacheItem<ScrapeTenderPaymentModel>(model, TimeSpan.FromHours(1)));
                }

                //if not Asked for Payment
            }
            else
            {
                model.IsError = true;
                ModelState.AddModelError("", _msg);
            }

            //if(ModelState.IsValid)
            //{
            //    string PaymentOptUrl = string.Format("{0}#m25-tab-1", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SCRAPESALE_OPEN_TENDER_PUCHASE));
            //    return Redirect(PaymentOptUrl);
            //}

            return View("~/Views/Feature/ScrapSale/ScrapSale/TenderPurchase.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderPurchase(ScrapeTenderPaymentModel model)
        {
            ScrapeTenderPaymentModel _exitigData = null;

            bool doPayment = true;
            if (!CacheProvider.TryGet("TenderPurchaseDetail", out _exitigData))
            {
                doPayment = false;
            }

            if (model.transactionNumber != _exitigData.transactionNumber)
            {
                doPayment = false;
            }

            if (doPayment)
            {
                _exitigData.IsOnline = model.IsOnline;
                //generate Reference no
                var response = CustomerSmartSaleClient.GetTenderReferenceNumber(new GetTenderReferenceNumber()
                {
                    consumernumber = CurrentPrincipal.PrimaryAccount,
                    tendernumber = _exitigData.transactionNumber,
                    transactiondescription = _exitigData.transactionDescription,
                    transactiontype = "TF",
                    amount = _exitigData.tenderFeeAmount,
                    email = CurrentPrincipal.EmailAddress,
                    paymenttype = _exitigData.IsOnline ? "O" : "F",
                }, RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null)
                {
                    _exitigData.IsJustPaid = true;
                    _exitigData.referencenumber = response.Payload.@return.referencenumber;

                    if (_exitigData.IsOnline)
                    {
                        #region [MIM Payment Implementation]

                        var payRequest = new CipherPaymentModel();
                        payRequest.PaymentData.amounts = _exitigData.tenderFeeAmount;
                        payRequest.PaymentData.contractaccounts = _exitigData.referencenumber;
                        payRequest.PaymentData.businesspartner = CurrentPrincipal.PrimaryAccount;
                        payRequest.PaymentData.email = CurrentPrincipal.EmailAddress;
                        payRequest.PaymentData.mobile = CurrentPrincipal.MobileNumber;
                        payRequest.PaymentData.easypaynumber = _exitigData.referencenumber;
                        payRequest.PaymentData.easypayflag = "T";
                        payRequest.PaymentData.notificationnumber = _exitigData.referencenumber;
                        payRequest.ServiceType = ServiceType.Miscellaneous;
                        payRequest.IsThirdPartytransaction = false;
                        payRequest.PaymentMethod = model.paymentMethod;
                        payRequest.BankKey = model.bankkey;
                        payRequest.SuqiaValue = model.SuqiaDonation;
                        payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                        var payResponse = ExecutePaymentGateway(payRequest);
                        if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                        {
                            CacheProvider.Store("TenderOnlinePurchaseDetail", new CacheItem<ScrapeTenderPaymentModel>(_exitigData, TimeSpan.FromHours(1)));
                            return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                        }
                        ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                        #endregion [MIM Payment Implementation]
                    }
                    else
                    {
                        CacheProvider.Store("TenderPurchaseDetail", new CacheItem<ScrapeTenderPaymentModel>(_exitigData, TimeSpan.FromMinutes(20)));
                        return View("~/Views/Feature/ScrapSale/ScrapSale/TenderPurchaseAlready.cshtml", _exitigData);
                    }
                }
                //ModelState.AddModelError("", response.Message);
                ModelState.AddModelError(string.Empty, Translate.Text("Webservice Error"));
            }
            else
            {
                model.IsError = true;
                ModelState.AddModelError("", "Please try again");
            }

            return View("~/Views/Feature/ScrapSale/ScrapSale/TenderPurchase.cshtml", _exitigData);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetRegionByCKey(string ckey)
        {
            return Json(new { data = GetRegion(ckey) }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult RenderSaleOrderDownload(string rid)
        {
            SalesOrderDownloadOTPModel model = new SalesOrderDownloadOTPModel();
            if (!string.IsNullOrWhiteSpace(rid))
            {
                //1.Send – OTP SMS
                //2.Verify – OTP Verification
                //3.Update – Download Tax invoice

                var returnData = CustomerSmartSaleClient.GetSalesOrderDownloadOTP(new GetSalesOrderDownloadOTP()
                {
                    action = Translate.Text("SOD_ACT_Send"),
                    referenceid = rid
                }, RequestLanguage, Request.Segment());

                if (returnData.Succeeded)
                {
                    model.ActionType = Translate.Text("SOD_ACT_Verify");
                    model.RefId = rid;
                }
                else
                {
                    ModelState.AddModelError("", returnData.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", Translate.Text("SOD_InvalidAction"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/RenderSaleOrderDownload.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RenderSaleOrderDownload(SalesOrderDownloadOTPModel model)
        {
            var returnData = CustomerSmartSaleClient.GetSalesOrderDownloadOTP(new GetSalesOrderDownloadOTP()
            {
                action = model.ActionType,
                referenceid = model.RefId,
                OTP = model.OTP
            }, RequestLanguage, Request.Segment());
            if (returnData.Succeeded)
            {
                if (model.ActionType == Translate.Text("SOD_ACT_Verify"))
                {
                    model.ActionType = Translate.Text("SOD_ACT_update");
                }
            }
            else
            {
                ModelState.AddModelError("", returnData.Message);
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/RenderSaleOrderDownload.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DownloadSaleOrderDownload(SalesOrderDownloadOTPModel model)
        {
            string fileName = "Error_{0}.pdf";
            byte[] downloadFile = new byte[0];
            string ctype = "application/pdf";
            try
            {
                var returnData = CustomerSmartSaleClient.GetSalesOrderDownloadOTP(new GetSalesOrderDownloadOTP()
                {
                    action = model.ActionType,
                    referenceid = model.RefId,
                    OTP = model.OTP
                }, RequestLanguage, Request.Segment());
                if (returnData.Succeeded)
                {
                    var _file = returnData.Payload.@return.attachment.FirstOrDefault();
                    if (_file != null)
                    {
                        downloadFile = _file.content;
                        fileName = _file.filename;
                        ctype = _file.contenttype;
                    }
                }
                else
                {
                    ModelState.AddModelError("", returnData.Message);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(model.RefId ?? Guid.NewGuid().ToString())))?.ToLower();
            return File(downloadFile, ctype, _fileName);
        }

        #endregion [Actions Controller]

        #region [Functions]

        private bool TryLoginWithScrapeSale(LoginModel model, out ServiceResponse<MasarLoginResponse> response, out string sessionId)
        {
            sessionId = null;

            #region Future Center Cookie functionality added by Shujaat

            var _fc = FetchFutureCenterValues();

            #endregion Future Center Cookie functionality added by Shujaat

            var loginRequest = new Foundation.Integration.APIHandler.Models.Request.Masar.Getloginsessionrequest()
            {
                type = model.LoginType,
                userid = model.Username,
                password = model.Password

            };


            response = MasarClient.MasarLogin(new MasarLoginRequest()
            {
                getloginsessionrequest = loginRequest
            }, RequestLanguage, Request.Segment());

            if (response != null && response.Succeeded && response.Payload.responsecode == "000")
            {
                //var data = response.Payload.;
                string _roles = model.LoginType == MasarConfig.ScrapSales ? Roles.ScrapeSale : Roles.Miscellaneous;

                sessionId = response.Payload.sessionid;
                //response.Payload.businesspartner = "8000000225";
                AuthStateService.Save(new DewaProfile(model.Username, sessionId, _roles)
                {
                    PrimaryAccount = response.Payload.primarycontractaccount,
                    EmailAddress = response.Payload.email,
                    BusinessPartner = response.Payload.businesspartner,
                    MobileNumber = response.Payload.mobile,
                    Name = response.Payload.fullname,
                    TermsAndConditions = response.Payload.termsandcondition.ToString(),
                    IsContactUpdated = response.Payload.updatemobileemail
                    //VatNumber = response.Payload.va
                });
                return true;
            }
            //error = response.Message;
            return false;
        }

        //private static string Base64Decode(string base64EncodedData)
        //{
        //    try
        //    {
        //        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        //        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogService.Info(ex);
        //    }
        //    return string.Empty;
        //}
        #region OLD Dropdown load
        //private List<SelectListItem> GetCountry()
        //{
        //    List<SelectListItem> data = new List<SelectListItem>();
        //    List<SelectListItem> reponseData = null;
        //    if (!CacheProvider.TryGet("scrap_countrylist" + RequestLanguage, out reponseData))
        //    {
        //        var response = CustomerSmartSaleClient.GetCountryHelpValues(new GetCountryHelpValues(), RequestLanguage, Request.Segment());

        //        if (response.Succeeded && response.Payload.@return.countrylist != null)
        //        {
        //            reponseData = response.Payload.@return.countrylist.Select(x => new SelectListItem() { Text = x.countrynamefulltext, Value = x.countrykey }).ToList();
        //            CacheProvider.Store("scrap_countrylist" + RequestLanguage, new CacheItem<List<SelectListItem>>(reponseData, TimeSpan.FromHours(1)));
        //        }
        //    }
        //    if (reponseData != null)
        //    {
        //        data.AddRange(reponseData);
        //    }
        //    return data;
        //}

        //private List<SelectListItem> GetRegion(string ckey)
        //{
        //    List<SelectListItem> data = new List<SelectListItem>();
        //    data.Add(new SelectListItem() { Text = Translate.Text("defaultSelect") });

        //    List<SelectListItem> reponseData = null;
        //    var response = CustomerSmartSaleClient.GetCountryHelpValues(new GetCountryHelpValues()
        //    {
        //        CountryKey = ckey
        //    }, RequestLanguage, Request.Segment());

        //    if (response.Succeeded &&
        //        response.Payload.@return != null &&
        //        response.Payload.@return.regionlist != null)
        //    {
        //        reponseData = response.Payload.@return.regionlist.Select(x => new SelectListItem() { Text = x.regiondescription, Value = x.region1 }).ToList();
        //    }
        //    if (reponseData != null)
        //    {
        //        data.AddRange(reponseData);
        //    }
        //    return data;
        //}

        #endregion

        private List<SelectListItem> GetCountry()
        {
            List<SelectListItem> data = new List<SelectListItem>();
            List<SelectListItem> reponseData = null;

            var response = GetAllDropdownValues();

            if (response.Succeeded && response.Payload.dropdownlist != null)
            {
                var issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "COUNTRY");
                if (issuedBy != null)
                {
                    reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
                else
                {
                    CacheProvider.Remove("scrap_alldropdowns");
                    response = GetAllDropdownValues();
                    issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "COUNTRY");
                    if (issuedBy != null)
                        reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
            }
            if (reponseData != null)
            {
                data.AddRange(reponseData);
            }
            return data;
        }

        private List<SelectListItem> GetTelephoneCountryCode()
        {
            List<SelectListItem> data = new List<SelectListItem>();
            List<SelectListItem> reponseData = null;

            var response = GetAllDropdownValues();

            if (response.Succeeded && response.Payload.dropdownlist != null)
            {
                var issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "TEL_EXT");
                if (issuedBy != null)
                {
                    reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
                else
                {
                    CacheProvider.Remove("scrap_alldropdowns");
                    response = GetAllDropdownValues();
                    issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "TEL_EXT");
                    if (issuedBy != null)
                        reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
            }
            if (reponseData != null)
            {
                data.AddRange(reponseData);
            }
            return data;
        }


        private List<SelectListItem> GetIssuedBy()
        {
            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = Translate.Text("defaultSelect"), Value = "" });
            List<SelectListItem> reponseData = null;

            var response = GetAllDropdownValues();


            if (response.Succeeded && response.Payload.dropdownlist != null)
            {

                var issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "ZISSUE_AUTH");
                if (issuedBy != null)
                {
                    reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
                else
                {
                    CacheProvider.Remove("scrap_alldropdowns");
                    response = GetAllDropdownValues();
                    issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "ZISSUE_AUTH");
                    if (issuedBy != null)
                        reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
            }

            if (reponseData != null)
            {
                data.AddRange(reponseData);
            }
            return data;
        }

        public ServiceResponse<MasarDropDownBaseResponse> GetAllDropdownValues()
        {
            ServiceResponse<MasarDropDownBaseResponse> reponseData = null;
            if (!CacheProvider.TryGet("scrap_alldropdowns" + RequestLanguage, out reponseData))
            {
                MasarDropdownRequest req = new MasarDropdownRequest();
                MasarDropdownBaseRequest bsReq = new MasarDropdownBaseRequest()
                {
                    dropdowninputs = req
                };

                reponseData = MasarClient.GetMasarDropDownData(bsReq, RequestLanguage, Request.Segment());
                CacheProvider.Store("scrap_alldropdowns" + RequestLanguage, new CacheItem<ServiceResponse<MasarDropDownBaseResponse>>(reponseData, TimeSpan.FromHours(1)));
            }

            return reponseData;
        }



        private List<SelectListItem> GetRegion(string ckey)
        {
            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = Translate.Text("defaultSelect"), Value = "" });

            List<SelectListItem> reponseData = null;
            var response = GetAllDropdownValues();


            if (response.Succeeded && response.Payload.dropdownlist != null)
            {

                var issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "EMIRATESCUST");
                if (issuedBy != null)
                {
                    reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
                else
                {
                    CacheProvider.Remove("scrap_alldropdowns");
                    response = GetAllDropdownValues();
                    issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "EMIRATESCUST");
                    if (issuedBy != null)
                        reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
            }

            if (reponseData != null)
            {
                data.AddRange(reponseData);
            }
            return data;
        }

        private List<SelectListItem> GetArea(string ckey)
        {
            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = Translate.Text("defaultSelect"), Value = "" });

            List<SelectListItem> reponseData = null;
            var response = GetAllDropdownValues();


            if (response.Succeeded && response.Payload.dropdownlist != null)
            {

                var issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "OFFICE_LOCATION");
                if (issuedBy != null)
                {
                    reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
                else
                {
                    CacheProvider.Remove("scrap_alldropdowns");
                    response = GetAllDropdownValues();
                    issuedBy = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "OFFICE_LOCATION");
                    if (issuedBy != null)
                        reponseData = issuedBy.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
            }

            if (reponseData != null)
            {
                data.AddRange(reponseData);
            }
            return data;
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ValidateUserRegistration(UserRegistrationModel model)
        {
            string errVerf = "";
            string receivedDate = DateTime.Now.ToString();
            try
            {
                if (!string.IsNullOrEmpty(model.TradeLicenseIssueDate))
                    model.TradeLicenseIssueDate = model.TradeLicenseIssueDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                if (!string.IsNullOrEmpty(model.TradeLicenseExpiryDate))
                    model.TradeLicenseExpiryDate = model.TradeLicenseExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                if (!string.IsNullOrEmpty(model.ExpiryDate))
                    model.ExpiryDate = model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");


                if (ModelState.IsValid)
                {
                    var _mobNumber = model.UserType == "O" ? model.CompanyMobileNumber : model.MobileNumber;
                    if (_mobNumber[0] != '0')
                        _mobNumber = "0" + _mobNumber;

                    var _telephoneCode = model.UserType == "O" ? model.CompanyTelephoneCode : model.TelephoneCode;
                    if (string.IsNullOrEmpty(model.IndividualCompanyTelephone) && string.IsNullOrEmpty(model.CompanyTelephone))
                        _telephoneCode = string.Empty;

                    var registrationInfoRequest = new RegistrationInfo()
                    {
                        country = model.UserType == "I" ? "AE" : model.Countrykey,
                        citycode = model.Countrykey == "AE" ? model.City : model.ActualCity,
                        email = model.UserType == "O" ? model.CompanyEmail : model.EmailAddress,
                        emiratesid = model.UserType == "I" ? model.EmiratesID : string.Empty,
                        firstname = model.UserType == "O" ? string.Empty : model.FirstName,
                        companyname = model.UserType == "O" ? model.CompanyName : string.Empty,
                        lastname = model.LastName,
                        mobile = _mobNumber, //!string.IsNullOrWhiteSpace(model.MobileNumber) ? Convert.ToInt64(model.MobileNumber).ToString("0000000000") : "",
                        pobox = model.POBox,
                        region = model.Region,
                        street = model.Street,
                        telephone = model.UserType == "O" ? model.CompanyTelephone : model.IndividualCompanyTelephone,
                        extension = model.UserType == "O" ? model.CompanyTelephoneExtension : model.IndividualCompanyTelephoneExtension,
                        title = model.UserType == "O" ? MasarConfig.Company : MasarConfig.Mr,
                        tradelicense = model.UserType == "O" ? model.TradelicenseNumber : string.Empty,
                        tradelicensevalidfrom = model.UserType == "O" ? !string.IsNullOrEmpty(model.TradeLicenseIssueDate) ? DateTime.ParseExact(model.TradeLicenseIssueDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                        tradelicensevalidto = model.UserType == "O" ? !string.IsNullOrEmpty(model.TradeLicenseExpiryDate) ? DateTime.ParseExact(model.TradeLicenseExpiryDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                        issueauthoritytext = model.UserType == "O" ? model.IssuingAuthorityDescription : string.Empty,
                        //customer category 2 for Organization and 1 for Inidividual
                        customercategory = model.UserType == "O" ? MasarConfig.Organization : MasarConfig.Individual,
                        customertype = model.CustomerType,//_ReturnURL == MasarConfig.ScrapSaleDashboardURL ? MasarConfig.ScrapSales : MasarConfig.Miscellaneous,
                        issueauthority = model.UserType == "O" ? model.TradeLicenseIssuingAuthorityKey : string.Empty,
                        requesttype = model.customeraccountgroup,
                        eidexpirydate = model.UserType == "I" ? !string.IsNullOrEmpty(model.ExpiryDate) ? DateTime.ParseExact(model.ExpiryDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                        mobiledialcode = model.UserType == "O" ? model.CompanyMobileCode : model.MobileCode,
                        telephonedialcode = _telephoneCode,
                        vatregistrationnumber = model.UserType == "O" ? model.VatRegistrationNo : string.Empty,
                        sessionid = "",
                        userid = "",
                        testflag = "X",
                        noattachflag = "X"

                    };
                    string apiCallStart = DateTime.Now.ToString();
                    var response = MasarClient.ScrapRegistration(new MasarUserRegistrationRequest()
                    {
                        customerinputs = registrationInfoRequest

                    }, RequestLanguage, Request.Segment());

                    string apiCallEnd = DateTime.Now.ToString();
                    if (response != null && response.Succeeded)
                    {

                        return Json(new { status = true, desc = errVerf, mvcDate = receivedDate, apiStart = apiCallStart, apiEnd = apiCallEnd }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        errVerf = response.Message;
                    }
                    return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }



        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FetchEIDA(SendICARequest request)
        {
            string errVerf = "";

            if (!string.IsNullOrEmpty(request.emiratesexpirydate))
                request.emiratesexpirydate = request.emiratesexpirydate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");

            DateTime _eidaDate = !string.IsNullOrEmpty(request.emiratesexpirydate)
                            ? DateTime.ParseExact(request.emiratesexpirydate, "dd MMMM yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
            try
            {
                Icainputs icaIn = new Icainputs()
                {
                    emiratesid = request.emiratesid,
                    emiratesexpirydate = _eidaDate.ToString("ddMMyyyy"),
                    userid = string.Empty,
                    sessionid = string.Empty

                };

                var response = MasarClient.MasarFetchICA(new MasarFetchICARequest()
                {
                    icainputs = icaIn

                }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded)
                {
                    if (response.Payload != null && !string.IsNullOrEmpty(response.Payload.mobile))
                    {
                        if (response.Payload.mobile[0] == '0')
                            response.Payload.mobile = response.Payload.mobile.Substring(1);
                    }

                    return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errVerf = response.Message;
                }
                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FetchTradeLicense(SendDEDRequest request)
        {
            string errVerf = "";


            if (!string.IsNullOrEmpty(request.tlexpirydate))
                request.tlexpirydate = request.tlexpirydate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");




            DateTime _tlExpiryDate = !string.IsNullOrEmpty(request.tlexpirydate)
                            ? DateTime.ParseExact(request.tlexpirydate, "dd MMMM yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
            try
            {
                Tradelicenseinput dedIn = new Tradelicenseinput()
                {
                    tradelicensenumber = request.tradelicensenumber,
                    expirydate = _tlExpiryDate == DateTime.MinValue ? string.Empty : _tlExpiryDate.ToString("ddMMyyyy"),
                    userid = string.Empty,
                    sessionid = string.Empty,
                    issuingauthority = request.issuingauthority,
                    type = request.type,
                    mode = request.mode

                };

                var response = MasarClient.MasarFetchDED(new MasarFetchDEDRequest()
                {
                    tradelicenseinput = dedIn

                }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded)
                {
                    if (response.Payload != null)
                    {
                        DateTime _tlIssueDate = !string.IsNullOrEmpty(response.Payload.tradelicensedetails.issuedate)
                           ? DateTime.ParseExact(response.Payload.tradelicensedetails.issuedate, "ddMMyyyy",
                                      System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
                        response.Payload.tradelicensedetails.issuedate = _tlIssueDate == DateTime.MinValue ? string.Empty : _tlIssueDate.ToString("dd MMMM yyyy");

                        return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, desc = response.Message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errVerf = response.Message;
                }
                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FetchApplicationData(Trackinput request)
        {
            string errVerf = "";

            try
            {
                Trackinput trackappReq = new Trackinput()
                {
                    referencenumber = !string.IsNullOrEmpty(request.referencenumber) ? request.referencenumber : string.Empty,
                    applicationnumber = request.applicationnumber

                };

                var response = MasarClient.FetchTrackApplicationData(new MasarTrackInputRequest()
                {
                    trackinput = trackappReq

                }, RequestLanguage, Request.Segment());



                if (response != null && response.Succeeded)
                {
                    if (response.Payload != null)
                    {
                        var returnResponse = response.Payload.trackdetailslist.FirstOrDefault();

                        bool isDocAvailable = false;
                        if (response.Payload.trackdetailattach != null && response.Payload.trackdetailattach.Count > 0)
                            isDocAvailable = true;

                        if (returnResponse != null)
                        {
                            if (returnResponse.businesspartnercategory == "2")
                            {
                                DateTime _tlIssueDate = !string.IsNullOrEmpty(returnResponse.tradelicensevalidfrom)
                                   ? DateTime.ParseExact(returnResponse.tradelicensevalidfrom, "ddMMyyyy",
                                              System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
                                returnResponse.tradelicensevalidfrom = _tlIssueDate == DateTime.MinValue ? string.Empty : _tlIssueDate.ToString("dd MMMM yyyy");

                                if (RequestLanguage.Code().ToLower() != "en")
                                    returnResponse.tradelicensevalidfrom = returnResponse.tradelicensevalidfrom.Replace("January", "يناير").Replace("February", "فبراير").Replace("March", "مارس").Replace("April", "أبريل").Replace("May", "مايو").Replace("June", "يونيو").Replace("July", "يوليو").Replace("August", "أغسطس").Replace("September", "سبتمبر").Replace("October", "أكتوبر").Replace("November", "نوفمبر").Replace("December", "ديسمبر");

                                DateTime _tlExpiryDate = !string.IsNullOrEmpty(returnResponse.tradelicensevalidto)
                                  ? DateTime.ParseExact(returnResponse.tradelicensevalidto, "ddMMyyyy",
                                             System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
                                returnResponse.tradelicensevalidto = _tlExpiryDate == DateTime.MinValue ? string.Empty : _tlExpiryDate.ToString("dd MMMM yyyy");

                                if (RequestLanguage.Code().ToLower() != "en")
                                    returnResponse.tradelicensevalidto = returnResponse.tradelicensevalidto.Replace("January", "يناير").Replace("February", "فبراير").Replace("March", "مارس").Replace("April", "أبريل").Replace("May", "مايو").Replace("June", "يونيو").Replace("July", "يوليو").Replace("August", "أغسطس").Replace("September", "سبتمبر").Replace("October", "أكتوبر").Replace("November", "نوفمبر").Replace("December", "ديسمبر");

                                returnResponse.emiratesidvalidfrom = string.Empty;
                                returnResponse.emiratesidvalidto = string.Empty;

                            }
                            else if (returnResponse.businesspartnercategory == "1")
                            {
                                DateTime _EidIssueDate = !string.IsNullOrEmpty(returnResponse.emiratesidvalidfrom)
                                  ? DateTime.ParseExact(returnResponse.emiratesidvalidfrom, "ddMMyyyy",
                                             System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
                                returnResponse.emiratesidvalidfrom = _EidIssueDate == DateTime.MinValue ? string.Empty : _EidIssueDate.ToString("dd MMMM yyyy");

                                if (RequestLanguage.Code().ToLower() != "en")
                                    returnResponse.emiratesidvalidfrom = returnResponse.emiratesidvalidfrom.Replace("January", "يناير").Replace("February", "فبراير").Replace("March", "مارس").Replace("April", "أبريل").Replace("May", "مايو").Replace("June", "يونيو").Replace("July", "يوليو").Replace("August", "أغسطس").Replace("September", "سبتمبر").Replace("October", "أكتوبر").Replace("November", "نوفمبر").Replace("December", "ديسمبر");

                                DateTime _EidExpiryDate = !string.IsNullOrEmpty(returnResponse.emiratesidvalidto)
                                  ? DateTime.ParseExact(returnResponse.emiratesidvalidto, "ddMMyyyy",
                                             System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
                                returnResponse.emiratesidvalidto = _EidExpiryDate == DateTime.MinValue ? string.Empty : _EidExpiryDate.ToString("dd MMMM yyyy");

                                if (RequestLanguage.Code().ToLower() != "en")
                                    returnResponse.emiratesidvalidto = returnResponse.emiratesidvalidto.Replace("January", "يناير").Replace("February", "فبراير").Replace("March", "مارس").Replace("April", "أبريل").Replace("May", "مايو").Replace("June", "يونيو").Replace("July", "يوليو").Replace("August", "أغسطس").Replace("September", "سبتمبر").Replace("October", "أكتوبر").Replace("November", "نوفمبر").Replace("December", "ديسمبر");

                                returnResponse.tradelicensevalidfrom = string.Empty;
                                returnResponse.tradelicensevalidto = string.Empty;
                            }

                            return Json(new { status = true, docAvailable = isDocAvailable, desc = response.Message, data = returnResponse }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new { status = false, desc = response.Message }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { status = false, desc = response.Message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errVerf = response.Message;
                }
                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult DownloadAttachmentForTrackEnrollment(string requestnumber)
        {
            string errVerf = string.Empty;

            try
            {

                Trackinput trackappReq = new Trackinput()
                {
                    referencenumber = string.Empty,
                    applicationnumber = requestnumber

                };

                var response = MasarClient.FetchTrackApplicationData(new MasarTrackInputRequest()
                {
                    trackinput = trackappReq

                }, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null)
                {
                    if (response.Payload.trackdetailattach != null && response.Payload.trackdetailattach.Count() > 0)
                    {
                        string _ext = !string.IsNullOrEmpty(response.Payload.trackdetailattach.FirstOrDefault().filename) ? Path.GetExtension(response.Payload.trackdetailattach.FirstOrDefault().filename).ToLower() : string.Empty;
                        return File(response.Payload.trackdetailattach.FirstOrDefault().content, response.Payload.trackdetailattach.FirstOrDefault().mimetype, requestnumber + "." + _ext);

                    }
                }




            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return null;

        }




        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale + "," + Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult DownloadAttachment(string requestnumber)
        {
            string errVerf = string.Empty;

            try
            {

                Bankdisplayreq _getBank = new Bankdisplayreq()
                {
                    requestnumber = requestnumber,
                    sessionid = CurrentPrincipal.SessionToken,
                    userid = CurrentPrincipal.UserId
                };

                var response = MasarClient.GetBankList(_getBank.userid, _getBank.sessionid, _getBank.requestnumber, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null)
                {
                    if (response.Payload.bankattachment[0] != null)
                    {
                        // byte[] bytes = System.Convert.FromBase64String(response.Payload.bankattachment[0].content);
                        string _ext = !string.IsNullOrEmpty(response.Payload.bankattachment.FirstOrDefault().filename) ? Path.GetExtension(response.Payload.bankattachment.FirstOrDefault().filename).ToLower() : string.Empty;
                        return File(response.Payload.bankattachment.FirstOrDefault().content, response.Payload.bankattachment.FirstOrDefault().mimetype, requestnumber + "." + _ext);
                        //return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, requestnumber + "." + _ext);
                    }
                }




            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return null;

        }



        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ViewBankRequest(Bankdisplayreq request)
        {
            string errVerf = string.Empty;
            try
            {
                Bankdisplayreq _getBank = new Bankdisplayreq()
                {
                    requestnumber = request.requestnumber,
                    sessionid = CurrentPrincipal.SessionToken,
                    userid = CurrentPrincipal.UserId
                };

                var response = MasarClient.GetBankList(_getBank.userid, _getBank.sessionid, _getBank.requestnumber, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {

                    return Json(new { status = true, desc = response.Message, data = response.Payload.bankheaderdetails }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errVerf = response.Message;
                }

                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CancelBankRequest(Bankdeletereq request)
        {
            string errVerf = string.Empty;
            try
            {
                Bankdeletereq _cancelBank = new Bankdeletereq()
                {
                    requestnumber = request.requestnumber,
                    sessionid = CurrentPrincipal.SessionToken,
                    userid = CurrentPrincipal.UserId
                };



                var response = MasarClient.CancelBankRequest(new MasarCancelBankRequest()
                {
                    bankdeletereq = _cancelBank

                }, RequestLanguage, Request.Segment());
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
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShouldDisplayIban(VerifyIbanRequest request)
        {
            string errVerf = string.Empty;
            try
            {
                bool isCountryIban = false;
                var response = GetAllDropdownValues();

                if (response.Succeeded && response.Payload.dropdownlist != null)
                {
                    var IsIban = response.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == "IBAN_MANDAT");
                    if (IsIban != null)
                    {
                        var reponseData = IsIban.values.Where(x => x.key == request.countrycode);
                        if (reponseData.Any())
                        {
                            if (reponseData.FirstOrDefault().value.ToUpper() == "X")
                                isCountryIban = true;
                        }
                    }
                }

                return Json(new { status = isCountryIban }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }



        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtp(MasarSendOtpRequest request)
        {
            string errVerf = "";
            string EmailId = string.Empty;
            string MobileNum = string.Empty;
            if (string.IsNullOrEmpty(request.email) && string.IsNullOrEmpty(request.mobile))
            {
                CacheProvider.TryGet("SecEmail", out EmailId);
                CacheProvider.TryGet("SecMobile", out MobileNum);
            }
            else
            {
                EmailId = request.email;
                MobileNum = request.mobile;
            }

            try
            {
                bool doCallAPIwithEmiratesId = false;
                //MasarReadOtpNB
                if (!string.IsNullOrEmpty(EmailId) && EmailId.IndexOf("*") > -1)
                    doCallAPIwithEmiratesId = true;

                if (!string.IsNullOrEmpty(MobileNum) && MobileNum.IndexOf("*") > -1)
                    doCallAPIwithEmiratesId = true;

                #region commentedcode
                //if (doCallAPIwithEmiratesId)
                //{
                //    ReadotpinputsEID getData = new ReadotpinputsEID()
                //    {
                //        otpmode = "R",
                //        processtype = request.prtype,
                //        inputid = request.emiratesId,
                //        idtype = "E"

                //    };

                //    var response = MasarClient.MasarReadOtpNB(new ReadOtpNbRequest()
                //    {
                //        readotpinputs = getData

                //    }, RequestLanguage, Request.Segment());
                //    if (response != null && response.Succeeded)
                //    {
                //        MasarOtpInput otpIn = new MasarOtpInput()
                //        {
                //            mode = request.mode,
                //            sessionid = CurrentPrincipal.SessionToken,
                //            reference = !string.IsNullOrEmpty(request.reqId) ? request.reqId : string.Empty,
                //            prtype = request.prtype,
                //            email = !string.IsNullOrEmpty(EmailId) ? response.Payload.email : string.Empty,
                //            mobile = !string.IsNullOrEmpty(MobileNum) ? response.Payload.mobile : string.Empty,
                //            otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : null,
                //            inputid = CurrentPrincipal.UserId,
                //            idtype = "U"

                //        };

                //        var responseOTP = MasarClient.MasarSendOtp(new MasarOTPRequest()
                //        {
                //            otpinput = otpIn

                //        }, Request.Segment(), RequestLanguage);
                //        if (responseOTP != null && responseOTP.Succeeded)
                //        {
                //            return Json(new { status = true, desc = responseOTP.Message, data = responseOTP.Payload }, JsonRequestBehavior.AllowGet);
                //        }
                //        else
                //        {
                //            errVerf = responseOTP.Message;
                //        }
                //    }
                //    else
                //    {
                //        errVerf = response.Message;
                //    }

                //}
                //else
                //{
                #endregion

                MasarOtpInput otpIn = new MasarOtpInput()
                {
                    mode = request.mode,
                    sessionid = CurrentPrincipal.SessionToken,
                    reference = !string.IsNullOrEmpty(request.reqId) ? request.reqId : string.Empty,
                    prtype = request.prtype,
                    email = EmailId,
                    mobile = MobileNum,
                    otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : string.Empty,
                    inputid = !string.IsNullOrEmpty(request.inputId) ? request.inputId : doCallAPIwithEmiratesId ? request.emiratesId : !string.IsNullOrEmpty(CurrentPrincipal.UserId) ? CurrentPrincipal.UserId : string.Empty,
                    idtype = !string.IsNullOrEmpty(request.idType) ? request.idType : doCallAPIwithEmiratesId ? "E" : !string.IsNullOrEmpty(CurrentPrincipal.UserId) ? "U" : string.Empty

                };

                var response = MasarClient.MasarSendOtp(new MasarOTPRequest()
                {
                    otpinput = otpIn

                }, Request.Segment(), RequestLanguage);
                if (response != null && response.Succeeded)
                {

                    response.Payload.email = EmailId;
                    response.Payload.mobile = MobileNum;

                    return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errVerf = response.Message;
                }
                //}
                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtpDecryptGUID(SendOtpRequest request)
        {
            string errVerf = string.Empty;
            string EmailId = string.Empty;
            string MobileNum = string.Empty;

            CacheProvider.TryGet("SecEmail", out EmailId);
            CacheProvider.TryGet("SecMobile", out MobileNum);


            try
            {
                //Send or Verify otp
                MasarOtpInput otpIn = new MasarOtpInput()
                {
                    mode = request.mode,
                    //sessionid = CurrentPrincipal.SessionToken,
                    reference = !string.IsNullOrEmpty(request.reqId) ? request.reqId : string.Empty,
                    prtype = request.prtype,
                    email = EmailId,
                    mobile = MobileNum,
                    otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : null,
                    inputid = !string.IsNullOrEmpty(CurrentPrincipal.UserId) ? CurrentPrincipal.UserId : string.Empty,
                    idtype = !string.IsNullOrEmpty(CurrentPrincipal.UserId) ? "U" : string.Empty

                };

                if (request.mode == "S")
                {
                    MasarOTPResponse _mResponse = new MasarOTPResponse();
                    _mResponse.referencenumber = request.reqId;

                    return Json(new { status = true, desc = string.Empty, data = _mResponse }, JsonRequestBehavior.AllowGet);
                }

                else
                {

                    var response = MasarClient.MasarSendOtp(new MasarOTPRequest()
                    {
                        otpinput = otpIn

                    }, Request.Segment(), RequestLanguage);




                    if (response != null && response.Succeeded)
                    {
                        response.Payload.email = EmailId;
                        response.Payload.mobile = MobileNum;

                        return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        errVerf = response.Message;
                    }
                }
                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetEmailNMobileTrack(Readotpinputs request)
        {
            string errVerf = "";
            try
            {
                Readotpinputs otpIn = new Readotpinputs()
                {
                    idtype = request.idtype,
                    processtype = MasarConfig.NonBilling,
                    inputid = request.inputid,
                    otpmode = "S"
                };

                var response = MasarClient.MasarGetMaskedEmailNPhone(new MasarGetMaskedEmailNMobileRequest()
                {
                    readotpinputs = otpIn

                }, Request.Segment(), RequestLanguage);
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
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetMaskedEmailNMobile(inputWrapperMaskedEmailNPhone request)
        {
            string errVerf = "";
            try
            {
                request.dN = request.dN;

                if (!string.IsNullOrEmpty(request.dN))
                    request.dN = "0";
                Readotpinputs otpIn = new Readotpinputs()
                {
                    idtype = MasarConfig.IdTYpe,
                    processtype = MasarConfig.NonBilling,
                    inputid = request.userName
                };

                var response = MasarClient.MasarGetMaskedEmailNPhone(new MasarGetMaskedEmailNMobileRequest()
                {
                    readotpinputs = otpIn

                }, Request.Segment(), RequestLanguage);
                if (response != null && response.Succeeded)
                {
                    response.Payload.email = response.Payload.maskedemail;
                    response.Payload.mobile = response.Payload.maskedmobile;

                    if (request.dN != "1010")
                    {
                        response.Payload.email = "";
                        response.Payload.mobile = "";
                    }


                    return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errVerf = response.Message;
                }
                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtpForgetPassword(inputWrapperMaskedEmailNPhone request)
        {
            string errVerf = "";
            string cemail = string.Empty;
            string cmobile = string.Empty;
            try
            {
                Readotpinputs otpMasked = new Readotpinputs()
                {
                    idtype = MasarConfig.IdTYpe,
                    processtype = MasarConfig.NonBilling,
                    inputid = request.userName
                };

                var response = MasarClient.MasarGetMaskedEmailNPhone(new MasarGetMaskedEmailNMobileRequest()
                {
                    readotpinputs = otpMasked

                }, Request.Segment(), RequestLanguage);

                if (response != null && response.Succeeded && response.Payload != null)
                {
                    response.Payload.email = response.Payload.maskedemail;
                    response.Payload.mobile = response.Payload.maskedmobile;

                    if (request.dN == "1")
                        cemail = response.Payload.email;
                    else if (request.dN == "2")
                        cmobile = response.Payload.mobile;

                    MasarOtpInput otpIn = new MasarOtpInput()
                    {
                        mode = string.IsNullOrEmpty(request.referenceNumber) ? MasarConfig.Mode : MasarConfig.ModeVerify,
                        sessionid = CurrentPrincipal.SessionToken,
                        reference = string.IsNullOrEmpty(request.referenceNumber) ? string.Empty : request.referenceNumber,
                        prtype = MasarConfig.NonBilling,
                        email = (!string.IsNullOrEmpty(cemail)) ? cemail : string.Empty,
                        mobile = (!string.IsNullOrEmpty(cmobile)) ? cmobile : string.Empty,
                        otp = string.IsNullOrEmpty(request.referenceNumber) ? string.Empty : request.actualOTP,
                        inputid = request.userName,
                        idtype = "U"

                    };

                    var responseOTP = MasarClient.MasarSendOtp(new MasarOTPRequest()
                    {
                        otpinput = otpIn

                    }, Request.Segment(), RequestLanguage);

                    if (responseOTP != null && responseOTP.Succeeded)
                    {
                        responseOTP.Payload.email = response.Payload.maskedemail;
                        responseOTP.Payload.mobile = response.Payload.maskedmobile;

                        return Json(new { status = true, desc = responseOTP.Message, data = responseOTP.Payload }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        errVerf = responseOTP.Message;
                    }

                }

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
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = Translate.Text("Unexpected error");
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }



        private CustomFileDetail GetTenderDocFiles(string tenderNo)
        {
            //doc info
            var _tenderDocRequest = new GetTenderDocumentDownload()
            {
                tendernumber = tenderNo,
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken,
                documentnumber = "",
                flag = ""
            };

            //trigger Service to get info
            var responseMultipleInfoFIle = CustomerSmartSaleClient.GetTenderDocumentDownload(_tenderDocRequest, RequestLanguage, Request.Segment());

            if (responseMultipleInfoFIle != null && responseMultipleInfoFIle.Payload != null && responseMultipleInfoFIle.Succeeded
                && responseMultipleInfoFIle.Payload.@return.document != null && responseMultipleInfoFIle.Payload.@return.document.Length > 0)
            {
                //getMultiple file details.

                #region [Get multiple + single file]

                if (responseMultipleInfoFIle.Payload.@return.document.Length == 0)
                {
                    #region [GET Single file]

                    var d = responseMultipleInfoFIle.Payload.@return.document.FirstOrDefault();

                    ///requesting for specific file by doc number.
                    _tenderDocRequest.documentnumber = d.documentnumber;
                    _tenderDocRequest.flag = "X";
                    var responseMultipleData = CustomerSmartSaleClient.GetTenderDocumentDownload(_tenderDocRequest, RequestLanguage, Request.Segment());

                    if (responseMultipleData.Succeeded && responseMultipleData.Payload != null &&
                             responseMultipleData.Payload.@return.document != null)
                    {
                        var filedata = responseMultipleData.Payload.@return.document.Where(x => x.documentnumber == d.documentnumber).FirstOrDefault();

                        if (filedata != null)
                        {
                            return new CustomFileDetail()
                            {
                                FileNameWithExtension = filedata.filename + ".pdf",
                                FileBytes = filedata.document,
                                MimeTypeExtension = filedata.contenttype
                            };
                        }
                    }

                    #endregion [GET Single file]
                }
                else
                {
                    #region [Get Multiple File ]

                    List<CustomFileDetail> fileList = new List<CustomFileDetail>();
                    foreach (var item in responseMultipleInfoFIle.Payload.@return.document)
                    {
                        ///requesting for specific file by doc number.
                        _tenderDocRequest.documentnumber = item.documentnumber;
                        _tenderDocRequest.flag = "X";
                        var responseMultipleData = CustomerSmartSaleClient.GetTenderDocumentDownload(_tenderDocRequest, RequestLanguage, Request.Segment());

                        if (responseMultipleData.Succeeded && responseMultipleData.Payload != null &&
                             responseMultipleData.Payload.@return.document != null)
                        {
                            var filedata = responseMultipleData.Payload.@return.document.Where(x => x.documentnumber == item.documentnumber).FirstOrDefault();

                            if (filedata != null)
                            {
                                fileList.Add(new CustomFileDetail()
                                {
                                    FileNameWithExtension = $"{filedata.filename}",
                                    MimeTypeExtension = filedata.contenttype,
                                    FileBytes = filedata.document
                                });
                            }
                        }
                    }

                    return CustomFileUtility.DownloadMultipleFiles("TenderDocZip_{0}", fileList);

                    #endregion [Get Multiple File ]
                }

                #endregion [Get multiple + single file]
            }

            return null;
        }

        private string getClearstri(string input)
        {
            string cleanStr = "";
            try
            {
                Regex regex = new Regex(Translate.Text("SS_Regex_Pattern"));
                cleanStr = regex.Replace(input, "");
            }
            catch (System.Exception)
            {
                cleanStr = "";
            }
            return cleanStr;
        }

        #endregion [Functions]

        #region Phase-2

        /// <summary>
        /// The AjaxWorkLogs
        /// </summary>
        /// <param name="currentPage">The currentPage<see cref="int"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost]
        public ActionResult AjaxSearchBidItems(int currentPage, int pageSize)
        {
            int maxRows = pageSize;
            BidNowModel bidModel = null;
            TenderBidingStep2Model modelStep2 = new TenderBidingStep2Model();

            if (CacheProvider.TryGet("TenderBidModel", out bidModel))
            {
                var bidItemList = bidModel.bidStep2.BidItemList;
                double pageCount = (double)(bidItemList.Count() / Convert.ToDecimal(maxRows));
                modelStep2.BidItemList = bidItemList.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

                modelStep2.TotalPage = Pager.CalculateTotalPages(bidItemList.Count, maxRows);
                modelStep2.Pagination = modelStep2.TotalPage > 1 ? true : false;
                modelStep2.Pagenumbers = modelStep2.TotalPage > 1 ? CommonUtility.GetPaginationRange(currentPage, modelStep2.TotalPage) : new List<int>();
                modelStep2.Page = currentPage;
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/_bidItemList.cshtml", modelStep2);
        }

        #region DownloadBOMDocument

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadBOMDocument(string tno)
        {
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";
            string fileMimeType = "application/pdf";

            var response = CustomerSmartSaleClient.GetBOQDownload(new GetBOQDownload()
            {
                tendernumber = tno
            }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            if (response.Succeeded && response.Payload != null &&
                             response.Payload.@return.attachment != null && response.Payload.@return.attachment.Length > 0)
            {
                var attachementList = response.Payload.@return.attachment;

                if (attachementList != null && attachementList.Count() == 1)
                {
                    var fileData = attachementList.FirstOrDefault();
                    downloadFile = fileData.content;
                    fileName = fileData.filename;
                    fileMimeType = fileData.contenttype;
                }
                else if (attachementList != null && attachementList.Count() > 1)
                {
                    #region [Get Multiple File ]

                    List<CustomFileDetail> fileList = new List<CustomFileDetail>();
                    foreach (var filedataitem in attachementList)
                    {
                        if (filedataitem != null)
                        {
                            fileList.Add(new CustomFileDetail()
                            {
                                FileBytes = filedataitem.content,
                                FileNameWithExtension = $"{filedataitem.filename}",
                                MimeTypeExtension = filedataitem.contenttype,
                            });
                        }
                    }

                    var mFiles = CustomFileUtility.DownloadMultipleFiles("BOMStruDocZip_{0}", fileList);

                    if (mFiles != null)
                    {
                        downloadFile = mFiles.FileBytes;
                        fileName = mFiles.FileNameWithExtension;
                        fileMimeType = mFiles.MimeTypeExtension;
                    }

                    #endregion [Get Multiple File ]
                }
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(tno ?? Guid.NewGuid().ToString())));
            return File(downloadFile, fileMimeType, _fileName);
        }

        #endregion DownloadBOMDocument

        //Tender Biding step-1

        #region TenderBidItemStep1

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderBidItemStep1(string t, string m)
        {
            BidNowModel bidnowModel = new BidNowModel();
            bidnowModel.bidStep1 = new TenderBidingStep1Model();
            bidnowModel.bidMode = m;
            try
            {
                var _bodDisplay = new GetBOQDisplay()
                {
                    tendernumber = t,
                };

                //trigger Service to get info
                var response = CustomerSmartSaleClient.GetBOQDisplay(_bodDisplay, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (response != null && response.Payload != null && response.Succeeded && response.Payload?.@return != null)
                {
                    var step1 = response.Payload.@return.header.FirstOrDefault();
                    if (step1 != null)
                    {
                        if (!string.IsNullOrEmpty(step1.tenderclosingdate))
                        {
                            long dif = DateTime.Now.Subtract(Convert.ToDateTime(step1.tenderclosingdate + " 23:59:59")).Ticks;
                            if (dif > 0)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
                            }
                        }
                        bidnowModel.bidStep1 = new TenderBidingStep1Model()
                        {
                            TenderNumber = step1.tendernumber,
                            TenderBondAmount = Convert.ToDecimal(step1.bidamount).ToString("F"),
                            TenderVatPayble = Convert.ToDecimal(step1.vatamount).ToString("F"),
                            TenderStatus = step1.tenderstatus,
                            TenderBidStatus = step1.bidstatus,
                            TenderSubmissiondate = step1.tenderclosingdate,
                            TenderSubmissionDeadline = step1.deadline,
                            TenderBidRefNumber = step1.bidreferencenumber,
                            TenderARDescription = step1.ardescription,
                            TenderBidAmount = Convert.ToDecimal(step1.tenderbondamount).ToString("F"),
                            TenderEndDescription = step1.enddescription,
                            TenderStatusValue = step1.status,
                            TenderBondPercentage = step1.tenderbondpercentage,
                            bidMode = bidnowModel.bidMode
                        };
                    }

                    var step2ItemList = response.Payload.@return.items;
                    bool isExpiredAndDisplay = string.IsNullOrWhiteSpace(step1.deadline) && (m.ToLower() == "display");
                    if ((m.ToLower() == "edit" || isExpiredAndDisplay) && (step1.status == "1" || step1.status == "2" || step1.status == "3")) // Check submit also
                    {
                        SetBIDUpdate _setBidUpdate = new SetBIDUpdate();
                        _setBidUpdate.bidmode = (step1.status == "1" || step1.status == "2" || step1.status == "3") ? "E" : "D";
                        if (isExpiredAndDisplay)
                        {
                            _setBidUpdate.bidmode = "D";
                        }
                        _setBidUpdate.bidreferencenumber = step1.bidreferencenumber;

                        var bidupdresponse = CustomerSmartSaleClient.SetBIDUpdate(_setBidUpdate, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                        if (bidupdresponse != null && bidupdresponse.Payload != null && bidupdresponse.Succeeded && bidupdresponse.Payload?.@return != null)
                        {
                            bidnowModel.bidStep2 = new TenderBidingStep2Model();
                            if (_setBidUpdate.bidmode == "E")
                            {
                                bidnowModel.bidStep2.TenderBidRefNumber = step1.bidreferencenumber;
                                bidnowModel.bidStep2.BidStatus = step1.bidstatus;
                                if (bidupdresponse.Payload.@return != null && bidupdresponse.Payload.@return.itemsedit != null)
                                {
                                    foreach (var item in bidupdresponse.Payload.@return.itemsedit)
                                    {
                                        bidnowModel.bidStep2.BidItemList.Add(new BidItem()
                                        {
                                            bidBond = item.bidbond,
                                            bomComponent = item.bomcomponent,
                                            bomItemnumber = item.bomitemnumber,
                                            componentQuantity = item.componentquantity,
                                            componentUnit = item.componentunit,
                                            materialDescription = item.materialdescription,
                                            serialNumber = item.sortstring,
                                            netPrice = Convert.ToDecimal(item.netprice).ToString("F"),
                                            netValue = Convert.ToDecimal(item.netvalue).ToString("F"),
                                            storageLocation = item.storagelocation,
                                            totalValue = Convert.ToDecimal(item.totalvalue).ToString("F"),
                                            vatAmount = Convert.ToDecimal(item.vatamount).ToString("F"),
                                        });
                                    }
                                }
                            }

                            if (isExpiredAndDisplay)
                            {
                                bidnowModel.bidStep2.TenderBidRefNumber = step1.bidreferencenumber;
                                bidnowModel.bidStep2.BidStatus = step1.bidstatus;
                                if (bidupdresponse.Payload.@return != null && bidupdresponse.Payload.@return.itemsdisplay != null)
                                {
                                    foreach (var item in bidupdresponse.Payload.@return.itemsdisplay)
                                    {
                                        bidnowModel.bidStep2.BidItemList.Add(new BidItem()
                                        {
                                            bidBond = item.bidbond,
                                            bomComponent = item.bomcomponent,
                                            bomItemnumber = item.bomitemnumber,
                                            componentQuantity = item.componentquantity,
                                            componentUnit = item.componentunit,
                                            //materialDescription = item.materialdescription,
                                            serialNumber = item.sortstring,
                                            netPrice = Convert.ToDecimal(item.netprice).ToString("F"),
                                            netValue = Convert.ToDecimal(item.netvalue).ToString("F"),
                                            //storageLocation = item.storagelocation,
                                            totalValue = Convert.ToDecimal(item.totalvalue).ToString("F"),
                                            vatAmount = Convert.ToDecimal(item.vatamount).ToString("F"),
                                        });
                                    }
                                }
                            }

                            if (_setBidUpdate.bidmode == "D" & !isExpiredAndDisplay)
                            {
                                if (bidupdresponse.Payload.@return != null && bidupdresponse.Payload.@return.itemsdisplay != null)
                                {
                                    foreach (var item in bidupdresponse.Payload.@return.itemsdisplay)
                                    {
                                        bidnowModel.bidStep2.BidItemList.Add(new BidItem()
                                        {
                                            bidBond = item.bidbond,
                                            bomComponent = item.bomcomponent,
                                            bomItemnumber = item.bomitemnumber,
                                            componentQuantity = item.componentquantity,
                                            componentUnit = item.componentunit,
                                            serialNumber = item.sortstring,
                                            //materialDescription = item.materialdescription,
                                            netPrice = Convert.ToDecimal(item.netprice).ToString("F"),
                                            netValue = Convert.ToDecimal(item.netvalue).ToString("F"),
                                            //storageLocation = item.storagelocation,
                                            totalValue = Convert.ToDecimal(item.totalvalue).ToString("F"),
                                            vatAmount = Convert.ToDecimal(item.vatamount).ToString("F")
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (step2ItemList != null)
                        {
                            bidnowModel.bidStep2 = new TenderBidingStep2Model();
                            foreach (var item in step2ItemList)
                            {
                                bidnowModel.bidStep2.BidItemList.Add(new BidItem()
                                {
                                    bidBond = item.bidbond,
                                    bomComponent = item.bomcomponent,
                                    bomItemnumber = item.bomitemnumber,
                                    componentQuantity = item.componentquantity,
                                    componentUnit = item.componentunit,
                                    materialDescription = item.materialdescription,
                                    serialNumber = item.sortstring,
                                    netPrice = Convert.ToDecimal(item.netprice).ToString("F"),
                                    netValue = Convert.ToDecimal(item.netvalue).ToString("F"),
                                    storageLocation = item.storagelocation,
                                    totalValue = Convert.ToDecimal(item.totalvalue).ToString("F"),
                                    vatAmount = Convert.ToDecimal(item.vatamount).ToString("F")
                                });
                            }
                        }
                    }

                    CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidnowModel));
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep1.cshtml", bidnowModel.bidStep1);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBidItemStep1(TenderBidingStep1Model model)
        {
            BidNowModel bidNowModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidNowModel))
                {
                    if (bidNowModel != null)
                    {
                        //   CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                    }
                }
                if (model.SubmitType != null && model.SubmitType.ToLower() == "stpwithdraw")
                {
                    SetBIDWithdraw _setBidWithdraw = new SetBIDWithdraw();
                    _setBidWithdraw.bidmode = "W";
                    _setBidWithdraw.bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber;

                    var response = CustomerSmartSaleClient.SetBIDWithdraw(_setBidWithdraw, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                    if (response != null && response.Payload != null && response.Succeeded && response.Payload?.@return != null)
                    {
                        CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, response.Message);
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
                if (model.SubmitType != null && model.SubmitType.ToLower() == "stpnext")
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP2);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        #endregion TenderBidItemStep1

        //Tender Biding step-2

        #region TenderBidItemStep2

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderBidItemStep2()
        {
            BidNowModel bidModel = null;
            int currentPage = 1;
            int maxRows = 2000; // 5 do later.
            TenderBidingStep2Model modelStep2 = new TenderBidingStep2Model();
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidModel))
                {
                    ViewBag.IsExpiredbiditemDisplay = Convert.ToBoolean(bidModel.bidStep1.bidMode == "DISPLAY" && string.IsNullOrWhiteSpace(bidModel.bidStep1.TenderSubmissionDeadline));
                    modelStep2.BidStatus = bidModel.bidStep1.TenderStatusValue;
                    modelStep2.TenderNumber = bidModel.bidStep1.TenderNumber;
                    modelStep2.bidMode = bidModel.bidMode;
                    modelStep2.TenderBidRefNumber = bidModel.bidStep1.TenderBidRefNumber;
                    modelStep2.TenderEndDescription = bidModel.bidStep1.TenderEndDescription;
                    modelStep2.TenderARDescription = bidModel.bidStep1.TenderARDescription;

                    //Pagination
                    var bidItemList = bidModel.bidStep2.BidItemList;
                    modelStep2.TotalRecords = bidItemList.Count();
                    modelStep2.BidItemList = bidItemList.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();
                    double pageCount = (double)(bidItemList.Count() / Convert.ToDecimal(maxRows));
                    modelStep2.TotalPage = Pager.CalculateTotalPages(bidItemList.Count, maxRows);
                    modelStep2.Pagination = modelStep2.TotalPage > 1 ? true : false;
                    modelStep2.Pagenumbers = modelStep2.TotalPage > 1 ? CommonUtility.GetPaginationRange(1, modelStep2.TotalPage) : new List<int>();
                    modelStep2.Page = 1;
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep2.cshtml", modelStep2);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            //return Redirect(string.Format("{0}?tno={1}&m={2}", @LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP1), bidModel.bidStep1.TenderNumber, bidModel.bidMode));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBidItemStep2(TenderBidingStep2Model model)
        {
            BidNowModel bidNowModel = null;
            bidCreation bidCreate = new bidCreation();
            SetBIDCreate _setBidCreate = new SetBIDCreate();
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidNowModel))
                {
                    if (bidNowModel != null)
                    {
                        foreach (var bidItem in model.BidItemList)
                        {
                            var item = bidNowModel.bidStep2.BidItemList.Where(x => x.bomItemnumber == bidItem.bomItemnumber).FirstOrDefault();
                            item.netPrice = bidItem.netPrice;
                            item.netValue = bidItem.netValue;
                            item.totalValue = bidItem.totalValue;
                        }
                        CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                    }
                }
                if (model.SubmitType != null && model.SubmitType.ToLower() == "savebid")
                {
                    // Re-withdraw service trigger
                    if (bidNowModel.bidStep1.TenderStatusValue == "3")
                    {
                        SetBIDWithdraw _setBidWithdraw = new SetBIDWithdraw();
                        _setBidWithdraw.bidmode = "R";
                        _setBidWithdraw.bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber;

                        var resWithdraw = CustomerSmartSaleClient.SetBIDWithdraw(_setBidWithdraw, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                        if (resWithdraw != null && resWithdraw.Payload != null && resWithdraw.Succeeded && resWithdraw.Payload?.@return != null)
                        {
                            CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                            //return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, resWithdraw.Message);
                        }
                    }

                    List<biditemDetails> list = new List<biditemDetails>();
                    foreach (var item in model.BidItemList)
                    {
                        list.Add(new biditemDetails { bomcomponent = item.bomComponent, netprice = item.netPrice, bomitemnumber = item.bomItemnumber });
                    }

                    bidCreate.bidmode = "S";
                    bidCreate.items = list.ToArray();
                    bidCreate.tenderno = bidNowModel.bidStep1.TenderNumber;

                    _setBidCreate.bidcreation = bidCreate;

                    var response = CustomerSmartSaleClient.SetBIDCreate(_setBidCreate, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                    if (response != null && response.Payload != null && response.Succeeded && response.Payload?.@return != null)
                    {
                        bidNowModel.bidStep2.TenderBidRefNumber = response.Payload.@return.bidreferencenumber;
                        bidNowModel.bidStep1.TenderBidRefNumber = response.Payload.@return.bidreferencenumber;
                        bidNowModel.bidStep2.successDesc = response.Payload.@return.description;
                        bidNowModel.bidStep2.Success = true;
                        bidNowModel.bidStep2.TenderNumber = bidNowModel.bidStep1.TenderNumber;
                        bidNowModel.bidStep2.bidMode = bidNowModel.bidMode;
                        bidNowModel.bidStep2.BidStatus = bidNowModel.bidStep1.TenderStatusValue;
                        CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                    }
                    else
                    {
                        bidNowModel.bidStep2.TenderNumber = bidNowModel.bidStep1.TenderNumber;
                        bidNowModel.bidStep2.bidMode = bidNowModel.bidMode;
                        //ModelState.AddModelError(string.Empty, response.Message);
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep2.cshtml", bidNowModel.bidStep2);
                }
                if (model.SubmitType != null && model.SubmitType.ToLower() == "nextbid")
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP3);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            //return Redirect(string.Format("{0}?tno={1}&m={2}", @LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP1), bidNowModel.bidStep1.TenderNumber, bidNowModel.bidMode));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        #endregion TenderBidItemStep2

        //Tender Biding step-3

        #region TenderBidItemStep3

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderBidItemStep3()
        {
            BidNowModel bidModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidModel))
                {
                    bidModel.bidStep3 = new TenderBidingStep3Model()
                    {
                        TenderNumber = bidModel.bidStep1.TenderNumber,
                        TenderBidRefNumber = bidModel.bidStep1.TenderBidRefNumber,
                        TenderEndDescription = bidModel.bidStep1.TenderEndDescription,
                        TenderARDescription = bidModel.bidStep1.TenderARDescription
                    };
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep3.cshtml", bidModel.bidStep3);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBidItemStep3(TenderBidingStep3Model model)
        {
            BidNowModel bidNowModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidNowModel))
                {
                    if (bidNowModel != null)
                    {
                        bidNowModel.bidStep3 = model;
                        if (model.SubmitType.ToLower() == "nextbid")
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP5);
                        }
                        if (model.SubmitType.ToLower() == "paybid")
                        {
                            TenderBiddingPaymentModel tenderBiddingPaymentModel = new TenderBiddingPaymentModel()
                            {
                                tenderFeeAmount = model.EarnestDepositAmount,
                                TenderNumber = bidNowModel.bidStep1.TenderNumber,
                                TenderBidRefNumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                TenderARDescription = bidNowModel.bidStep1.TenderARDescription,
                                TenderEndDescription = bidNowModel.bidStep1.TenderEndDescription
                            };
                            CacheProvider.Store("TenderBidPaymentModel", new CacheItem<TenderBiddingPaymentModel>(tenderBiddingPaymentModel));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_BIDDING_PUCHASE);
                        }

                        CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                    }
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP5);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        #endregion TenderBidItemStep3

        //Tender Biding step-5

        #region TenderBidItemStep5

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderBidItemStep5()
        {
            BidNowModel bidModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidModel))
                {
                    bidModel.bidStep5 = new TenderBidingStep5Model()
                    {
                        TenderNumber = bidModel.bidStep1.TenderNumber,
                        TenderEndDescription = bidModel.bidStep1.TenderEndDescription,
                        TenderARDescription = bidModel.bidStep1.TenderARDescription,
                        TenderBidRefNumber = bidModel.bidStep1.TenderBidRefNumber,
                        BidStatus = bidModel.bidStep1.TenderStatusValue
                    };
                    bidModel.bidStep6 = new TenderBidingStep6Model();
                    bidModel.bidStep7 = new TenderBidingStep7Model();

                    if (!string.IsNullOrWhiteSpace(bidModel.bidStep1.TenderBidRefNumber))
                    {
                        List<CustomFileDetail> customFileDetails = new List<CustomFileDetail>();
                        customFileDetails = GetBIDItemAttachment(bidModel.bidStep1.TenderBidRefNumber);
                        if (customFileDetails != null)
                        {
                            for (int i = 0; i < customFileDetails.Count; i++)
                            {
                                if (customFileDetails[i].AttachmentNumber == "1")
                                {
                                    bidModel.bidStep5.TenderBond_AttachmentFileBinary1 = customFileDetails[i].FileBytes;
                                    bidModel.bidStep5.TenderBond_AttachmentFileName1 = customFileDetails[i].FileNameWithExtension;
                                    bidModel.bidStep5.TenderBond_AttachmentMimeType1 = customFileDetails[i].MimeTypeExtension;
                                    bidModel.bidStep5.DocumentNumber = customFileDetails[i].DocumentNumber;
                                }
                                if (customFileDetails[i].AttachmentNumber == "2")
                                {
                                    bidModel.bidStep5.TenderBond_AttachmentFileBinary2 = customFileDetails[i].FileBytes;
                                    bidModel.bidStep5.TenderBond_AttachmentFileName2 = customFileDetails[i].FileNameWithExtension;
                                    bidModel.bidStep5.TenderBond_AttachmentMimeType2 = customFileDetails[i].MimeTypeExtension;
                                    bidModel.bidStep5.DocumentNumber = customFileDetails[i].DocumentNumber;
                                }
                                if (customFileDetails[i].AttachmentNumber == "3")
                                {
                                    bidModel.bidStep5.TenderBond_AttachmentFileBinary3 = customFileDetails[i].FileBytes;
                                    bidModel.bidStep5.TenderBond_AttachmentFileName3 = customFileDetails[i].FileNameWithExtension;
                                    bidModel.bidStep5.TenderBond_AttachmentMimeType3 = customFileDetails[i].MimeTypeExtension;
                                    bidModel.bidStep5.DocumentNumber = customFileDetails[i].DocumentNumber;
                                }
                                if (customFileDetails[i].AttachmentNumber == "4")
                                {
                                    bidModel.bidStep6.TenderForm_AttachmentFileBinary1 = customFileDetails[i].FileBytes;
                                    bidModel.bidStep6.TenderForm_AttachmentFileName1 = customFileDetails[i].FileNameWithExtension;
                                    bidModel.bidStep6.TenderForm_AttachmentMimeType1 = customFileDetails[i].MimeTypeExtension;
                                    bidModel.bidStep6.DocumentNumber = customFileDetails[i].DocumentNumber;
                                }
                                if (customFileDetails[i].AttachmentNumber == "5")
                                {
                                    bidModel.bidStep6.TenderForm_AttachmentFileBinary2 = customFileDetails[i].FileBytes;
                                    bidModel.bidStep6.TenderForm_AttachmentFileName2 = customFileDetails[i].FileNameWithExtension;
                                    bidModel.bidStep6.TenderForm_AttachmentMimeType2 = customFileDetails[i].MimeTypeExtension;
                                    bidModel.bidStep6.DocumentNumber = customFileDetails[i].DocumentNumber;
                                }
                                if (customFileDetails[i].AttachmentNumber == "6")
                                {
                                    bidModel.bidStep7.TenderOther_AttachmentFileBinary1 = customFileDetails[i].FileBytes;
                                    bidModel.bidStep7.TenderOther_AttachmentFileName1 = customFileDetails[i].FileNameWithExtension;
                                    bidModel.bidStep7.TenderOther_AttachmentMimeType1 = customFileDetails[i].MimeTypeExtension;
                                    bidModel.bidStep7.DocumentNumber = customFileDetails[i].DocumentNumber;
                                }
                            }
                        }
                        CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidModel));
                    }
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep5.cshtml", bidModel.bidStep5);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBidItemStep5(TenderBidingStep5Model model)
        {
            BidNowModel bidNowModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidNowModel))
                {
                    if (bidNowModel != null)
                    {
                        // Attachement
                        bidNowModel.bidtenderAttachments = new List<bidAttachment>();

                        // Attachment 1
                        string actMode1 = "C";
                        if (bidNowModel.bidStep5.TenderBond_AttachmentFileBinary1 != null)
                        {
                            if (model.TenderBond_AttachedDocument1 != null)
                            {
                                actMode1 = "U";
                                bidNowModel.bidStep5.TenderBond_AttachmentFileBinary1 = model.TenderBond_AttachedDocument1.ToArray();
                            }
                            else if (model.TenderBond_AttachmentRemove1)
                            {
                                bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                                {
                                    action = "D",
                                    bidattachmenttype = "1",
                                    bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                    content = new byte[0],
                                    filename = "",
                                    mimetype = "",
                                    documentnumber = bidNowModel.bidStep5.DocumentNumber
                                });
                            }
                        }
                        else
                        {
                            bidNowModel.bidStep5.TenderBond_AttachmentFileBinary1 = model.TenderBond_AttachedDocument1.ToArray();
                        }

                        if (model.TenderBond_AttachedDocument1 != null)
                        {
                            bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                            {
                                action = actMode1,
                                bidattachmenttype = "1",
                                bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                content = bidNowModel.bidStep5.TenderBond_AttachmentFileBinary1.ToArray(),
                                filename = model.TenderBond_AttachedDocument1.FileName?.ToUpper(),
                                mimetype = model.TenderBond_AttachedDocument1.FileName.GetFileExtensionTrimmed(),
                                documentnumber = (actMode1 == "U") ? bidNowModel.bidStep5.DocumentNumber : string.Empty
                            });
                        }

                        // Attachment 2
                        string actMode2 = "C";
                        if (bidNowModel.bidStep5.TenderBond_AttachmentFileBinary2 != null)
                        {
                            if (model.TenderBond_AttachedDocument2 != null)
                            {
                                actMode2 = "U";
                                bidNowModel.bidStep5.TenderBond_AttachmentFileBinary2 = model.TenderBond_AttachedDocument2.ToArray();
                            }
                            else if (model.TenderBond_AttachmentRemove2)
                            {
                                bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                                {
                                    action = "D",
                                    bidattachmenttype = "2",
                                    bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                    content = new byte[0],
                                    filename = "",
                                    mimetype = "",
                                    documentnumber = bidNowModel.bidStep5.DocumentNumber
                                });
                            }
                        }
                        else
                        {
                            bidNowModel.bidStep5.TenderBond_AttachmentFileBinary2 = model.TenderBond_AttachedDocument2.ToArray();
                        }

                        if (model.TenderBond_AttachedDocument2 != null)
                        {
                            bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                            {
                                action = actMode2,
                                bidattachmenttype = "2",
                                bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                content = bidNowModel.bidStep5.TenderBond_AttachmentFileBinary2.ToArray(),
                                filename = model.TenderBond_AttachedDocument2.FileName?.ToUpper(),
                                mimetype = model.TenderBond_AttachedDocument2.FileName.GetFileExtensionTrimmed(),
                                documentnumber = (actMode2 == "U") ? bidNowModel.bidStep5.DocumentNumber : string.Empty
                            });
                        }

                        // Attachment 3
                        string actMode3 = "C";
                        if (bidNowModel.bidStep5.TenderBond_AttachmentFileBinary3 != null)
                        {
                            if (model.TenderBond_AttachedDocument3 != null)
                            {
                                actMode3 = "U";
                                bidNowModel.bidStep5.TenderBond_AttachmentFileBinary3 = model.TenderBond_AttachedDocument3.ToArray();
                            }
                            else if (model.TenderBond_AttachmentRemove3)
                            {
                                bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                                {
                                    action = "D",
                                    bidattachmenttype = "3",
                                    bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                    content = new byte[0],
                                    filename = "",
                                    mimetype = "",
                                    documentnumber = bidNowModel.bidStep5.DocumentNumber
                                });
                            }
                        }
                        else
                        {
                            bidNowModel.bidStep5.TenderBond_AttachmentFileBinary3 = model.TenderBond_AttachedDocument3.ToArray();
                        }

                        if (model.TenderBond_AttachedDocument3 != null)
                        {
                            bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                            {
                                action = actMode3,
                                bidattachmenttype = "3",
                                bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                content = bidNowModel.bidStep5.TenderBond_AttachmentFileBinary3.ToArray(),
                                filename = model.TenderBond_AttachedDocument3.FileName?.ToUpper(),
                                mimetype = model.TenderBond_AttachedDocument3.FileName.GetFileExtensionTrimmed(),
                                documentnumber = (actMode3 == "U") ? bidNowModel.bidStep5.DocumentNumber : string.Empty
                            });
                        }

                        CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                    }
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP6);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        #endregion TenderBidItemStep5

        //Tender Biding step-6

        #region TenderBidItemStep6

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderBidItemStep6()
        {
            BidNowModel bidModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidModel))
                {
                    bidModel.bidStep6 = new TenderBidingStep6Model()
                    {
                        TenderNumber = bidModel.bidStep1.TenderNumber,
                        TenderEndDescription = bidModel.bidStep1.TenderEndDescription,
                        TenderARDescription = bidModel.bidStep1.TenderARDescription,

                        // Attachment 1
                        TenderForm_AttachedDocument1 = bidModel.bidStep6.TenderForm_AttachedDocument1,
                        TenderForm_AttachmentFileBinary1 = bidModel.bidStep6.TenderForm_AttachmentFileBinary1,
                        TenderForm_AttachmentFileName1 = bidModel.bidStep6.TenderForm_AttachmentFileName1,
                        TenderForm_AttachmentMimeType1 = bidModel.bidStep6.TenderForm_AttachmentMimeType1,
                        TenderForm__AttachmentFileType1 = bidModel.bidStep6.TenderForm__AttachmentFileType1,

                        // Attachment 2
                        TenderForm_AttachedDocument2 = bidModel.bidStep6.TenderForm_AttachedDocument2,
                        TenderForm_AttachmentFileBinary2 = bidModel.bidStep6.TenderForm_AttachmentFileBinary2,
                        TenderForm_AttachmentFileName2 = bidModel.bidStep6.TenderForm_AttachmentFileName2,
                        TenderForm_AttachmentMimeType2 = bidModel.bidStep6.TenderForm_AttachmentMimeType2,
                        TenderForm__AttachmentFileType2 = bidModel.bidStep6.TenderForm__AttachmentFileType2,

                        DocumentNumber = bidModel.bidStep6.DocumentNumber
                    };
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep6.cshtml", bidModel.bidStep6);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBidItemStep6(TenderBidingStep6Model model)
        {
            BidNowModel bidNowModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidNowModel))
                {
                    if (bidNowModel != null)
                    {
                        string actMode4 = "C";
                        if (bidNowModel.bidStep6.TenderForm_AttachmentFileBinary1 != null)
                        {
                            if (model.TenderForm_AttachedDocument1 != null)
                            {
                                actMode4 = "U";
                                bidNowModel.bidStep6.TenderForm_AttachmentFileBinary1 = model.TenderForm_AttachedDocument1.ToArray();
                            }
                            else if (model.TenderForm_AttachmentRemove1)
                            {
                                bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                                {
                                    action = "D",
                                    bidattachmenttype = "4",
                                    bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                    content = new byte[0],
                                    filename = "",
                                    mimetype = "",
                                    documentnumber = bidNowModel.bidStep6.DocumentNumber
                                });
                            }
                        }
                        else
                        {
                            bidNowModel.bidStep6.TenderForm_AttachmentFileBinary1 = model.TenderForm_AttachedDocument1.ToArray();
                        }
                        if (model.TenderForm_AttachedDocument1 != null)
                        {
                            bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                            {
                                action = actMode4,
                                bidattachmenttype = "4",
                                bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                content = bidNowModel.bidStep6.TenderForm_AttachmentFileBinary1.ToArray(),
                                filename = model.TenderForm_AttachedDocument1.FileName?.ToUpper(),
                                mimetype = model.TenderForm_AttachedDocument1.FileName.GetFileExtensionTrimmed(),
                                documentnumber = (actMode4 == "U") ? bidNowModel.bidStep6.DocumentNumber : string.Empty
                            });
                        }

                        string actMode5 = "C";
                        if (bidNowModel.bidStep6.TenderForm_AttachmentFileBinary2 != null)
                        {
                            if (model.TenderForm_AttachedDocument2 != null)
                            {
                                actMode5 = "U";
                                bidNowModel.bidStep6.TenderForm_AttachmentFileBinary2 = model.TenderForm_AttachedDocument2.ToArray();
                            }
                            else if (model.TenderForm_AttachmentRemove2)
                            {
                                bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                                {
                                    action = "D",
                                    bidattachmenttype = "5",
                                    bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                    content = new byte[0],
                                    filename = "",
                                    mimetype = "",
                                    documentnumber = bidNowModel.bidStep6.DocumentNumber
                                });
                            }
                        }
                        else
                        {
                            bidNowModel.bidStep6.TenderForm_AttachmentFileBinary2 = model.TenderForm_AttachedDocument2.ToArray();
                        }
                        if (model.TenderForm_AttachedDocument2 != null)
                        {
                            bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                            {
                                action = actMode5,
                                bidattachmenttype = "5",
                                bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                content = bidNowModel.bidStep6.TenderForm_AttachmentFileBinary2.ToArray(),
                                filename = model.TenderForm_AttachedDocument2.FileName?.ToUpper(),
                                mimetype = model.TenderForm_AttachedDocument2.FileName.GetFileExtensionTrimmed(),
                                documentnumber = (actMode5 == "U") ? bidNowModel.bidStep6.DocumentNumber : string.Empty
                            });
                        }

                        CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));
                    }
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE_BIDITEM_STEP7);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        #endregion TenderBidItemStep6

        //Tender Biding step-7

        #region TenderBidItemStep7

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderBidItemStep7()
        {
            BidNowModel bidModel = null;
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidModel))
                {
                    bidModel.bidStep7 = new TenderBidingStep7Model()
                    {
                        TenderNumber = bidModel.bidStep1.TenderNumber,
                        TenderEndDescription = bidModel.bidStep1.TenderEndDescription,
                        TenderARDescription = bidModel.bidStep1.TenderARDescription,

                        // Attachment 1
                        TenderOther_AttachedDocument1 = bidModel.bidStep7.TenderOther_AttachedDocument1,
                        TenderOther_AttachmentFileBinary1 = bidModel.bidStep7.TenderOther_AttachmentFileBinary1,
                        TenderOther_AttachmentFileName1 = bidModel.bidStep7.TenderOther_AttachmentFileName1,
                        TenderOther_AttachmentMimeType1 = bidModel.bidStep7.TenderOther_AttachmentMimeType1,
                        TenderOther__AttachmentFileType1 = bidModel.bidStep7.TenderOther__AttachmentFileType1,

                        DocumentNumber = bidModel.bidStep7.DocumentNumber
                    };
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep7.cshtml", bidModel.bidStep7);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderBidItemStep7(TenderBidingStep7Model model)
        {
            BidNowModel bidNowModel = null;
            bidCreation bidCreate = new bidCreation();
            SetBIDCreate _setBidCreate = new SetBIDCreate();
            try
            {
                if (CacheProvider.TryGet("TenderBidModel", out bidNowModel))
                {
                    if (bidNowModel != null)
                    {
                        string actMode7 = "C";
                        if (bidNowModel.bidStep7.TenderOther_AttachmentFileBinary1 != null)
                        {
                            if (model.TenderOther_AttachedDocument1 != null)
                            {
                                actMode7 = "U";
                                bidNowModel.bidStep7.TenderOther_AttachmentFileBinary1 = model.TenderOther_AttachedDocument1.ToArray();
                            }
                            else if (model.TenderOther_AttachmentRemove1)
                            {
                                bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                                {
                                    action = "D",
                                    bidattachmenttype = "6",
                                    bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                    content = new byte[0],
                                    filename = "",
                                    mimetype = "",
                                    documentnumber = bidNowModel.bidStep7.DocumentNumber
                                });
                            }
                        }
                        else
                        {
                            bidNowModel.bidStep7.TenderOther_AttachmentFileBinary1 = model.TenderOther_AttachedDocument1.ToArray();
                        }

                        if (model.TenderOther_AttachedDocument1 != null)
                        {
                            bidNowModel.bidtenderAttachments.Add(new bidAttachment()
                            {
                                action = actMode7,
                                bidattachmenttype = "6",
                                bidreferencenumber = bidNowModel.bidStep1.TenderBidRefNumber,
                                content = bidNowModel.bidStep7.TenderOther_AttachmentFileBinary1.ToArray(),
                                filename = model.TenderOther_AttachedDocument1.FileName?.ToUpper(),
                                mimetype = model.TenderOther_AttachedDocument1.FileName.GetFileExtensionTrimmed(),
                                documentnumber = (actMode7 == "U") ? bidNowModel.bidStep7.DocumentNumber : string.Empty
                            });
                        }

                        List<biditemDetails> list = new List<biditemDetails>();
                        foreach (var item in bidNowModel.bidStep2.BidItemList)
                        {
                            list.Add(new biditemDetails { bomcomponent = item.bomComponent, netprice = item.netPrice, bomitemnumber = item.bomItemnumber });
                        }

                        bidCreate.bidmode = "C";
                        bidCreate.items = list.ToArray();
                        bidCreate.tenderno = bidNowModel.bidStep1.TenderNumber;
                        bidCreate.attachment = bidNowModel.bidtenderAttachments.ToArray();

                        _setBidCreate.bidcreation = bidCreate;

                        var response = CustomerSmartSaleClient.SetBIDCreate(_setBidCreate, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                        if (response != null && response.Payload != null && response.Succeeded && response.Payload?.@return != null)
                        {
                            bidNowModel.successReferenceNumber = response.Payload.@return.bidreferencenumber;

                            CacheProvider.Store("TenderBidModel", new CacheItem<BidNowModel>(bidNowModel));

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_TENDER_BIDDING_SUCCESS);
                        }
                        else
                        {
                            //ModelState.AddModelError(string.Empty, response.Message);
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                }
                return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidActionStep7.cshtml", bidNowModel.bidStep7);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        #endregion TenderBidItemStep7

        #region TenderPurchaseBiddingSuccess

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderPurchaseBiddingSuccess()
        {
            BidNowModel bidNowModel = null;
            if (CacheProvider.TryGet("TenderBidModel", out bidNowModel))
            {
                CacheProvider.Remove("TenderBidModel");
                return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidSubmit.cshtml", new TenderBidingConfirm()
                {
                    confirmBidReferenceNumber = bidNowModel.bidStep1.TenderBidRefNumber,
                    confirmReferenceNumber = bidNowModel.successReferenceNumber,
                    confirmSuccess = true,
                    confirmTenderNumber = bidNowModel.bidStep1.TenderNumber
                });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PORTAL_DASHBOARD);
        }

        #endregion TenderPurchaseBiddingSuccess

        #region DownloadBIDItemDocument

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadBIDItemDocument(string bidrefno)
        {
            // Bid Item download
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";
            string fileMimeType = "application/pdf";

            var response = CustomerSmartSaleClient.GetBIDMainDownload(new GetBIDMainDownload()
            {
                bidreferencenumber = bidrefno
            }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            if (response.Succeeded && response.Payload != null &&
                             response.Payload.@return.bidmainattachment != null && response.Payload.@return.bidmainattachment.Length > 0)
            {
                var attachementList = response.Payload.@return.bidmainattachment;

                if (attachementList != null && attachementList.Count() == 1)
                {
                    var fileData = attachementList.FirstOrDefault();
                    downloadFile = fileData.content;
                    fileName = fileData.filename;
                    fileMimeType = fileData.contenttype;
                }
                else if (attachementList != null && attachementList.Count() > 1)
                {
                    #region [Get Multiple File ]

                    List<CustomFileDetail> fileList = new List<CustomFileDetail>();
                    foreach (var filedataitem in attachementList)
                    {
                        if (filedataitem != null)
                        {
                            fileList.Add(new CustomFileDetail()
                            {
                                FileBytes = filedataitem.content,
                                FileNameWithExtension = $"{filedataitem.filename}",
                                MimeTypeExtension = filedataitem.contenttype,
                            });
                        }
                    }

                    var mFiles = CustomFileUtility.DownloadMultipleFiles("BidItemDocZip_{0}", fileList);

                    if (mFiles != null)
                    {
                        downloadFile = mFiles.FileBytes;
                        fileName = mFiles.FileNameWithExtension;
                        fileMimeType = mFiles.MimeTypeExtension;
                    }

                    #endregion [Get Multiple File ]
                }
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(bidrefno ?? Guid.NewGuid().ToString())));
            return File(downloadFile, fileMimeType, _fileName);
        }

        #endregion DownloadBIDItemDocument

        #region DownloadBIDItemAttachment

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadBIDItemAttachment(string bidrefno)
        {
            // Bid Item Download
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";
            string fileMimeType = "application/pdf";

            var file = GetBIDItembtnAttachment(bidrefno);
            if (file != null)
            {
                fileName = file.FileNameWithExtension;
                downloadFile = file.FileBytes;
                fileMimeType = file.MimeTypeExtension;
            }

            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(bidrefno ?? Guid.NewGuid().ToString())));
            return File(downloadFile, fileMimeType, _fileName);
        }

        #endregion DownloadBIDItemAttachment

        #region GetBIDItembtnAttachment

        private CustomFileDetail GetBIDItembtnAttachment(string bidrefNo)
        {
            //doc info
            var _bidItemDocRequest = new GetBIDDownload()
            {
                bidreferencenumber = bidrefNo,
                documentnumber = "",
                flag = ""
            };

            List<CustomFileDetail> fileList = new List<CustomFileDetail>();

            //trigger Service to get info
            var responseMultipleInfoFIle = CustomerSmartSaleClient.GetBIDDownload(_bidItemDocRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            if (responseMultipleInfoFIle != null && responseMultipleInfoFIle.Payload != null && responseMultipleInfoFIle.Succeeded
                && responseMultipleInfoFIle.Payload.@return.document != null && responseMultipleInfoFIle.Payload.@return.document.Length > 0)
            {
                //getMultiple file details.

                #region [Get multiple + single file]

                if (responseMultipleInfoFIle.Payload.@return.document.Length == 0)
                {
                    #region [GET Single file]

                    var d = responseMultipleInfoFIle.Payload.@return.document.FirstOrDefault();

                    ///requesting for specific file by doc number.
                    _bidItemDocRequest.documentnumber = d.documentnumber;
                    _bidItemDocRequest.flag = "X";
                    _bidItemDocRequest.bidreferencenumber = bidrefNo;
                    var responseMultipleData = CustomerSmartSaleClient.GetBIDDownload(_bidItemDocRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                    if (responseMultipleData.Succeeded && responseMultipleData.Payload != null &&
                             responseMultipleData.Payload.@return.document != null)
                    {
                        var filedata = responseMultipleData.Payload.@return.document.Where(x => x.documentnumber == d.documentnumber).FirstOrDefault();

                        if (filedata != null)
                        {
                            return new CustomFileDetail()
                            {
                                FileNameWithExtension = filedata.filename,
                                FileBytes = filedata.document,
                                MimeTypeExtension = filedata.contenttype,
                                AttachmentNumber = d.contenttype
                            };
                        }
                    }

                    #endregion [GET Single file]
                }
                else
                {
                    #region [Get Multiple File ]

                    foreach (var item in responseMultipleInfoFIle.Payload.@return.document)
                    {
                        ///requesting for specific file by doc number.
                        _bidItemDocRequest.documentnumber = item.documentnumber;
                        _bidItemDocRequest.flag = "X";
                        _bidItemDocRequest.bidreferencenumber = bidrefNo;
                        var responseMultipleData = CustomerSmartSaleClient.GetBIDDownload(_bidItemDocRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                        if (responseMultipleData.Succeeded && responseMultipleData.Payload != null &&
                             responseMultipleData.Payload.@return.document != null)
                        {
                            var filedata = responseMultipleData.Payload.@return.document.Where(x => x.documentnumber == item.documentnumber).FirstOrDefault();

                            if (filedata != null)
                            {
                                fileList.Add(new CustomFileDetail()
                                {
                                    FileNameWithExtension = $"{filedata.filename}",
                                    MimeTypeExtension = filedata.contenttype,
                                    FileBytes = filedata.document,
                                    AttachmentNumber = item.contenttype
                                });
                            }
                        }
                    }

                    return CustomFileUtility.DownloadMultipleFiles("BidAttachDocZip_{0}", fileList);

                    #endregion [Get Multiple File ]
                }

                #endregion [Get multiple + single file]
            }

            return null;
        }

        #endregion GetBIDItembtnAttachment

        #region GetBIDItemAttachment

        private List<CustomFileDetail> GetBIDItemAttachment(string bidrefNo)
        {
            //doc info
            var _bidItemDocRequest = new GetBIDDownload()
            {
                bidreferencenumber = bidrefNo,
                documentnumber = "",
                flag = ""
            };
            List<CustomFileDetail> fileList = new List<CustomFileDetail>();

            //trigger Service to get info
            var responseMultipleInfoFIle = CustomerSmartSaleClient.GetBIDDownload(_bidItemDocRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            if (responseMultipleInfoFIle != null && responseMultipleInfoFIle.Payload != null && responseMultipleInfoFIle.Succeeded
                && responseMultipleInfoFIle.Payload.@return.document != null && responseMultipleInfoFIle.Payload.@return.document.Length > 0)
            {
                //getMultiple file details.

                #region [Get multiple + single file]

                if (responseMultipleInfoFIle.Payload.@return.document.Length == 0)
                {
                    #region [GET Single file]

                    var d = responseMultipleInfoFIle.Payload.@return.document.FirstOrDefault();

                    ///requesting for specific file by doc number.
                    _bidItemDocRequest.documentnumber = d.documentnumber;
                    _bidItemDocRequest.flag = "X";
                    _bidItemDocRequest.bidreferencenumber = bidrefNo;
                    var responseMultipleData = CustomerSmartSaleClient.GetBIDDownload(_bidItemDocRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                    if (responseMultipleData.Succeeded && responseMultipleData.Payload != null &&
                             responseMultipleData.Payload.@return.document != null)
                    {
                        var filedata = responseMultipleData.Payload.@return.document.Where(x => x.documentnumber == d.documentnumber).FirstOrDefault();

                        if (filedata != null)
                        {
                            fileList.Add(new CustomFileDetail()
                            {
                                FileNameWithExtension = filedata.filename,
                                FileBytes = filedata.document,
                                MimeTypeExtension = filedata.contenttype,
                                AttachmentNumber = d.contenttype,
                                DocumentNumber = d.documentnumber
                            });
                            return fileList;
                        }
                    }

                    #endregion [GET Single file]
                }
                else
                {
                    #region [Get Multiple File ]

                    foreach (var item in responseMultipleInfoFIle.Payload.@return.document)
                    {
                        ///requesting for specific file by doc number.
                        _bidItemDocRequest.documentnumber = item.documentnumber;
                        _bidItemDocRequest.flag = "X";
                        _bidItemDocRequest.bidreferencenumber = bidrefNo;
                        var responseMultipleData = CustomerSmartSaleClient.GetBIDDownload(_bidItemDocRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                        if (responseMultipleData.Succeeded && responseMultipleData.Payload != null &&
                             responseMultipleData.Payload.@return.document != null)
                        {
                            var filedata = responseMultipleData.Payload.@return.document.Where(x => x.documentnumber == item.documentnumber).FirstOrDefault();

                            if (filedata != null)
                            {
                                fileList.Add(new CustomFileDetail()
                                {
                                    FileNameWithExtension = $"{filedata.filename}",
                                    MimeTypeExtension = filedata.contenttype,
                                    FileBytes = filedata.document,
                                    AttachmentNumber = item.contenttype,
                                    DocumentNumber = item.documentnumber
                                });
                            }
                        }
                    }

                    return fileList;

                    #endregion [Get Multiple File ]
                }

                #endregion [Get multiple + single file]
            }
            return null;
        }

        #endregion GetBIDItemAttachment

        #region TenderPurchaseBidding

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult TenderPurchaseBidding(string r = "")
        {
            TenderBiddingPaymentModel model = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(r))
                {
                    if (CacheProvider.TryGet("TenderBidOnlinePurchaseDetail", out model))
                    {
                        if (model != null && model.IsSuccess)
                        {
                            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBiddingAlready.cshtml", model);
                        }
                        else
                        {
                            model.IsError = true;
                            ModelState.AddModelError("", model.Message);
                        }
                    }
                    if (model == null)
                    {
                        //model.IsError = true;
                        ModelState.AddModelError("", "Session Out for TransactionId" + r);
                    }
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidding.cshtml", model);
                }

                if (!CacheProvider.TryGet("TenderBidPaymentModel", out model))
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("TenderBidPaymentModel is empty."));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidding.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TenderPurchaseBidding(TenderBiddingPaymentModel model)
        {
            try
            {
                //generate Reference no
                if (!string.IsNullOrWhiteSpace(model.TenderBidRefNumber) && !string.IsNullOrWhiteSpace(model.tenderFeeAmount))
                {
                    var response = CustomerSmartSaleClient.GetTenderReferenceNumber(new GetTenderReferenceNumber()
                    {
                        bidreferencenumber = model.TenderBidRefNumber,
                        consumernumber = CurrentPrincipal.PrimaryAccount,
                        tendernumber = model.TenderNumber,
                        transactiondescription = string.Empty,
                        transactiontype = "EM",
                        amount = model.tenderFeeAmount,
                        email = CurrentPrincipal.EmailAddress,
                        paymenttype = model.IsOnline ? "O" : "F",
                    }, RequestLanguage, Request.Segment());

                    if (response.Succeeded && response.Payload != null)
                    {
                        model.IsJustPaid = true;
                        model.ReferenceNo = response.Payload.@return.referencenumber;

                        if (model.IsOnline)
                        {
                            #region [MIM Payment Implementation]

                            var payRequest = new CipherPaymentModel();
                            payRequest.PaymentData.amounts = model.tenderFeeAmount;
                            payRequest.PaymentData.contractaccounts = model.ReferenceNo;
                            payRequest.PaymentData.businesspartner = CurrentPrincipal.PrimaryAccount;
                            payRequest.PaymentData.email = CurrentPrincipal.EmailAddress;
                            payRequest.PaymentData.mobile = CurrentPrincipal.MobileNumber;
                            payRequest.PaymentData.easypaynumber = model.ReferenceNo;
                            payRequest.PaymentData.easypayflag = "BT";
                            payRequest.PaymentData.notificationnumber = model.ReferenceNo;
                            payRequest.ServiceType = ServiceType.Miscellaneous;
                            payRequest.IsThirdPartytransaction = false;
                            payRequest.PaymentMethod = model.paymentMethod;
                            payRequest.BankKey = model.bankkey;
                            payRequest.SuqiaValue = model.SuqiaDonation;
                            payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                            var payResponse = ExecutePaymentGateway(payRequest);
                            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                            {
                                CacheProvider.Store("TenderBidOnlinePurchaseDetail", new CacheItem<TenderBiddingPaymentModel>(model, TimeSpan.FromHours(1)));
                                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                            }
                            ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                            #endregion [MIM Payment Implementation]
                        }
                        else
                        {
                            CacheProvider.Store("TenderBidPurchaseDetail", new CacheItem<TenderBiddingPaymentModel>(model, TimeSpan.FromMinutes(20)));
                            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBiddingAlready.cshtml", model);
                        }
                    }
                    //ModelState.AddModelError("", response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                else
                {
                    model.IsError = true;
                    ModelState.AddModelError("", "Please try again");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/TenderPurchaseBidding.cshtml", model);
        }

        #endregion TenderPurchaseBidding

        protected List<SelectListItem> GetDataSource(string sourceID)
        {
            var item = Sitecorex.Database.GetItem(new ScData.ID(sourceID));
            if (item != null && item.Children != null)
            {
                return item.Children?.Select(c => new SelectListItem
                {
                    Text = c.Fields["Text"].ToString(),
                    Value = c.Fields["Value"].ToString()
                })?.ToList();
            }
            return new List<SelectListItem>();
        }



        #region LogOut

        [HttpGet]
        public ActionResult LogOut()
        {
            string isScrap = _ReturnURL;

            if (IsLoggedIn)
            {
                ClearSessionAndSignOut();
            }

            //  return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN, new QueryString(false)
            // .With("returnUrl", isScrap));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);
        }

        #endregion LogOut

        // Sales Order Tab

        #region SalesOrderTab

        #region SalesOrderHistory

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult SalesOrderHistory()
        {
            scrapOrdersList model = new scrapOrdersList();
            try
            {
                var response = CustomerSmartSaleClient.GetScrapOrders(new GetScrapOrders()
                {
                    customernumber = CurrentPrincipal.PrimaryAccount,
                    materialnumber = "",//"3012000073",
                    salesdocumentnumber = "", //"3011000162"
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null)
                {
                    model = response.Payload.@return;
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, response.Message);
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SalesOrderHistory.cshtml", model);
        }

        #endregion SalesOrderHistory

        #region DownloadSalesOrderDocument

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadSalesOrderDocument(string ssono, string tno)
        {
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";
            string fileMimeType = "application/pdf";

            try
            {
                var response = CustomerSmartSaleClient.GetSalesOrderDownload(new GetSalesOrderDownload()
                {
                    salesdocumentnumber = ssono,
                    tendernumber = tno
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (response != null && response.Payload != null && response.Succeeded
                     && response.Payload.@return.attachment != null && response.Payload.@return.attachment.Length > 0)
                {
                    var fileData = response.Payload.@return.attachment.FirstOrDefault();
                    if (fileData != null)
                    {
                        downloadFile = fileData.content;
                        fileName = fileData.filename;
                        fileMimeType = fileData.contenttype;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(ssono ?? Guid.NewGuid().ToString())));
            return File(downloadFile, fileMimeType, _fileName);
        }

        #endregion DownloadSalesOrderDocument

        #region SalesOrderPaymentMethod

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SalesOrderPaymentMethod(SalesOrderPaymentModel model)
        {
            CacheProvider.Store("SalesOrderPaymentModel", new CacheItem<SalesOrderPaymentModel>(model));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_SALESORDER_PAYMENT);
        }

        #endregion SalesOrderPaymentMethod

        #region SalesOrderPayment

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult SalesOrderPayment(string r = "")
        {
            SalesOrderPaymentModel model = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(r))
                {
                    if (CacheProvider.TryGet("SalesOrderOnlinePurchaseDetail", out model))
                    {
                        if (model != null && model.IsSuccess)
                        {
                            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SalesOrderPaymentAlready.cshtml", model);
                        }
                        else
                        {
                            model.IsError = true;
                            ModelState.AddModelError("", model.Message);
                        }
                    }
                    if (model == null)
                    {
                        //model.IsError = true;
                        ModelState.AddModelError("", "Session Out for TransactionId" + r);
                    }
                    return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SalesOrderPayment.cshtml", model);
                }

                if (!CacheProvider.TryGet("SalesOrderPaymentModel", out model))
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("SalesOrderPaymentModel is empty."));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SalesOrderPayment.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SalesOrderPayment(SalesOrderPaymentModel model)
        {
            try
            {
                //generate Reference no
                if (!string.IsNullOrWhiteSpace(model.salesDocumentNo) && !string.IsNullOrWhiteSpace(model.SalesPaymentAmt))
                {
                    if (model.IsOnline)
                    {
                        #region [MIM Payment Implementation]

                        var payRequest = new CipherPaymentModel();
                        payRequest.PaymentData.amounts = model.SalesPaymentAmt;
                        payRequest.PaymentData.contractaccounts = model.salesDocumentNo;
                        payRequest.PaymentData.businesspartner = CurrentPrincipal.PrimaryAccount;
                        payRequest.PaymentData.email = CurrentPrincipal.EmailAddress;
                        payRequest.PaymentData.mobile = CurrentPrincipal.MobileNumber;
                        payRequest.PaymentData.easypaynumber = model.salesDocumentNo;
                        payRequest.PaymentData.easypayflag = "SO";
                        payRequest.PaymentData.notificationnumber = model.salesDocumentNo;
                        payRequest.ServiceType = ServiceType.Miscellaneous;
                        payRequest.IsThirdPartytransaction = false;
                        payRequest.PaymentMethod = model.paymentMethod;
                        payRequest.BankKey = model.bankkey;
                        payRequest.SuqiaValue = model.SuqiaDonation;
                        payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                        var payResponse = ExecutePaymentGateway(payRequest);
                        if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                        {
                            CacheProvider.Store("SalesOrderOnlinePurchaseDetail", new CacheItem<SalesOrderPaymentModel>(model, TimeSpan.FromHours(1)));
                            return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                        }
                        ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                        #endregion [MIM Payment Implementation]
                    }
                    else
                    {
                        var response = CustomerSmartSaleClient.GetTenderReferenceNumber(new GetTenderReferenceNumber()
                        {
                            bidreferencenumber = model.salesDocumentNo,
                            consumernumber = CurrentPrincipal.PrimaryAccount,
                            tendernumber = model.SalesTenderNo,
                            transactiondescription = string.Empty,
                            transactiontype = "SO",
                            amount = model.SalesPaymentAmt,
                            email = CurrentPrincipal.EmailAddress,
                            paymenttype = "F",
                        }, RequestLanguage, Request.Segment());

                        if (response.Succeeded && response.Payload != null)
                        {
                            // model.IsJustPaid = true;
                            model.ReferenceNo = response.Payload.@return.referencenumber;
                        }
                        else
                        {
                            //ModelState.AddModelError("", response.Message);
                            ModelState.AddModelError(string.Empty, response.Message);
                        }

                        CacheProvider.Store("SalesOrderPurchaseDetail", new CacheItem<SalesOrderPaymentModel>(model, TimeSpan.FromMinutes(20)));
                        return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SalesOrderPaymentAlready.cshtml", model);
                    }
                }
                else
                {
                    model.IsError = true;
                    ModelState.AddModelError("", "Please try again");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SalesOrderPayment.cshtml", model);
        }

        #endregion SalesOrderPayment

        #region SalesOrderPaymentHistory

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult SalesOrderPaymentHistory()
        {
            SalesOrderPaymentHistoryModel model = new SalesOrderPaymentHistoryModel();
            try
            {
                ViewBag.ReceiptsFilter = GetLstDataSource(DataSources.SCRAP_SALE_SALESORDER_RECEIPTS).ToList();

                // EMD payment history
                var emdResponse = CustomerSmartSaleClient.GetEMDList(new GetEMDList()
                {
                    consumernumber = CurrentPrincipal.PrimaryAccount,
                    bidreferencenumber = "",
                    tendernumber = "",
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (emdResponse != null && emdResponse.Succeeded && emdResponse.Payload != null)
                {
                    model.emdList = emdResponse.Payload.@return;

                    if (model.emdList.EMDlistdetails != null)
                    {
                        model.emdList.EMDlistdetails = model.emdList.EMDlistdetails.ToList().OrderByDescending(x => x.postingdate).ToArray();
                    }
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, emdResponse.Message);
                    ModelState.AddModelError(string.Empty, emdResponse.Message);
                }

                // Sales order payment history
                var orderResponse = CustomerSmartSaleClient.GetScrapOrderPayments(new GetScrapOrderPayments()
                {
                    customernumber = CurrentPrincipal.PrimaryAccount,
                    materialnumber = "",
                    salesdocumentnumber = "",
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (orderResponse != null && orderResponse.Succeeded && orderResponse.Payload != null)
                {
                    model.scrapOrdersDetailsList = orderResponse.Payload.@return;
                    if (model.scrapOrdersDetailsList.SOPaymentDetails != null)
                    {
                        model.scrapOrdersDetailsList.SOPaymentDetails = model.scrapOrdersDetailsList.SOPaymentDetails.ToList().OrderByDescending(x => x.postingdate).ToArray();
                    }
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, orderResponse.Message);
                    ModelState.AddModelError(string.Empty, orderResponse.Message);
                    //ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SalesOrderPaymentHistory.cshtml", model);
        }

        #endregion SalesOrderPaymentHistory

        #region DownloadSalesOrderReceipt

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadSalesOrderReceipt(SalesOrderPaymentHistoryModel model)
        {
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";
            string fileMimeType = "application/pdf";

            try
            {
                var response = CustomerSmartSaleClient.GetScrapAccountDocumentDownload(new GetScrapAccountDocumentDownload()
                {
                    customernumber = CurrentPrincipal.PrimaryAccount,
                    salesdocumentnumber = model.salesDocNumber,
                    tendernumber = "",
                    paymentdata = new scrapSOPaymentData
                    {
                        accountingdocumentnumber = model.accDocNumber,
                        amountlocalcurrency = "",
                        assignmentnumber = "",
                        companycode = model.companyCode,
                        currencykey = "",
                        customernumber = "",
                        documentdate = "",
                        documenttype = "",
                        entrydate = "",
                        fiscalyear = model.fiscalYear,
                        postingdate = "",
                        referencedocumentnumber = ""
                    }
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (response != null && response.Payload != null && response.Succeeded
                     && response.Payload.@return.attachment != null && response.Payload.@return.attachment.Length > 0)
                {
                    var fileData = response.Payload.@return.attachment.FirstOrDefault();
                    if (fileData != null)
                    {
                        downloadFile = fileData.content;
                        fileName = fileData.filename;
                        fileMimeType = fileData.contenttype;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(model.salesDocNumber ?? Guid.NewGuid().ToString())));
            return File(downloadFile, fileMimeType, _fileName);
        }

        #endregion DownloadSalesOrderReceipt

        #region DownloadEMDPaymentReceipt

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadEMDPaymentReceipt(SalesOrderPaymentHistoryModel model)
        {
            byte[] downloadFile = new byte[0];
            string fileName = "Error_{0}.pdf";
            string fileMimeType = "application/pdf";

            try
            {
                var response = CustomerSmartSaleClient.GetTenderReceipt(new GetTenderReceipt()
                {
                    accountingdocumentnumber = model.emdAccDocNumber,
                    referencenumber = model.tenderRefNumber
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (response != null && response.Payload != null && response.Succeeded
                     && response.Payload.@return.receipt != null && response.Payload.@return.receipt.Length > 0)
                {
                    var fileData = response.Payload.@return.receipt;
                    if (fileData != null)
                    {
                        downloadFile = fileData;
                        fileName = "{0}.pdf";
                        //fileMimeType = fileData.contenttype;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            string _fileName = getClearstri(string.Format(fileName, Convert.ToString(model.emdAccDocNumber ?? Guid.NewGuid().ToString())));
            return File(downloadFile, fileMimeType, _fileName);
        }

        #endregion DownloadEMDPaymentReceipt

        #endregion SalesOrderTab

        #region Tender Opening results

        [HttpGet]
        public ActionResult TenderOpeningResults()
        {
            List<openTender> openTenderResultList = null;
            try
            {
                if (!CacheProvider.TryGet(CacheKeys.SCRAP_SALE_OPEN_TENDER_RESULT, out openTenderResultList))
                {
                    var reponseData = CustomerSmartSaleClient.GetScrapsalesTenderResultList(new GetScrapsalesTenderResultList()
                    {
                        date = ""//"20181216"
                    }, RequestLanguage, Request.Segment());

                    if (reponseData != null &&
                        reponseData.Succeeded &&
                        reponseData.Payload?.@return.opentender != null &&
                        reponseData.Payload?.@return.opentender.Count() > 0)
                    {
                        openTenderResultList = reponseData.Payload?.@return.opentender.ToList();
                        CacheProvider.Store(CacheKeys.SCRAP_SALE_OPEN_TENDER_RESULT, new CacheItem<List<openTender>>(openTenderResultList));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, reponseData.Message);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/TenderOpeningResults.cshtml", openTenderResultList);
        }

        #endregion Tender Opening results

        #region GetTenderOpeningResultList

        [HttpGet]
        public PartialViewResult GetTenderOpeningResultList(string t)
        {
            GetTenderResultDisplayDataResponse model = new GetTenderResultDisplayDataResponse();

            try
            {
                var tender = VendorServiceClient.GetTenderResultDisplay(t, RequestLanguage, Request.Segment());

                var Fdate = tender.Payload.FloatingDate.ToString().FormatDate("dd.MM.yyyy");
                var Cdate = tender.Payload.ClosingDate.ToString().FormatDate("dd.MM.yyyy");

                if (tender != null && tender.Succeeded && tender.Payload != null)
                {
                    model = tender.Payload;
                    model.TenderNumber = t;
                    model.FloatingDate = Fdate.HasValue ? Fdate.Value.ToString("dd-MMM-yyyy", Sitecorex.Culture) : string.Empty;
                    model.ClosingDate = Cdate.HasValue ? Cdate.Value.ToString("dd-MMM-yyyy", Sitecorex.Culture) : string.Empty;
                    model.TenderDescription = Server.HtmlDecode(tender.Payload.TenderDescription);
                    model.TenderType = tender.Payload.TenderType;
                    ViewBag.HasResult = true;
                }
                else
                {
                    ViewBag.HasResult = false;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/_TenderOpeningResultsList.cshtml", model);
        }

        #endregion GetTenderOpeningResultList

        // Profile update tab

        #region Profile Update

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale + "," + Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult SetProfileUpdate()
        {
            string errVerf = "";
            //Send or Verify otp

            string defaultPhoneCode = "AE";

            var model = new ProfileUpdateModel();



            Profileinputs fetchUserInput = new Profileinputs()
            {
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId
            };

            var response = MasarClient.MasarProfileFetch(new MasarProfileFetchRequest()
            {
                profileinputs = fetchUserInput

            }, RequestLanguage, Request.Segment());

            model.CountryList = GetCountry();
            model.RegionList = GetRegion("AE");
            model.AreaList = GetArea("");
            model.TradeLicenseIssuingAuthority = GetIssuedBy().ToList();
            model.CompanyTelephoneCodeList = GetTelephoneCountryCode();

            model.TradeLicenseIssueDate = string.Empty;
            model.TradeLicenseExpiryDate = string.Empty;

            try
            {

                if (response != null && response.Succeeded)
                {
                    if (response.Payload.profilelist != null && response.Payload.profilelist.Count > 0)
                    {
                        var profileDetails = response.Payload.profilelist.FirstOrDefault();

                        if (profileDetails.bpcategory == null)
                            profileDetails.bpcategory = MasarConfig.Organization;



                        //for Organization bpcategory is 2 and for individual its 1
                        if (profileDetails.bpcategory == MasarConfig.Organization)
                        {
                            model.TradenumberFlag = profileDetails.tradenumberflag;
                            model.IssuedateFlag = profileDetails.issuedateflag;
                            model.ExpirydateFlag = profileDetails.expirydateflag;
                            model.AttachmentFlag = profileDetails.attachmentflag;
                            model.IssueauthorityFlag = profileDetails.issueauthorityflag;



                            model.CompanyEmail = profileDetails.email;
                            model.CompanyMobileNumber = !string.IsNullOrEmpty(profileDetails.mobile) ? profileDetails.mobile.RemoveMobileNumberZeroPrefix().Replace("+971", string.Empty) : string.Empty;

                            if (!string.IsNullOrEmpty(model.CompanyMobileNumber) && model.CompanyMobileNumber[0] == '0')
                                model.CompanyMobileNumber = model.CompanyMobileNumber.Substring(1);

                            model.UserType = "O";
                            model.TradeLicenseIssuingAuthorityKey = profileDetails.issuedby;
                            model.IssuingAuthorityDescription = profileDetails.issueauthoritytext;
                            model.TradelicenseNumber = profileDetails.tradelicense;
                            model.TradeLicenseIssueDate = !string.IsNullOrEmpty(profileDetails.tradelicensevalidfrom) && profileDetails.tradelicensevalidfrom != "00000000"
                                ? DateTime.ParseExact(profileDetails.tradelicensevalidfrom, "ddMMyyyy",
                                           System.Globalization.CultureInfo.InvariantCulture).ToString("dd MMMM yyyy") : string.Empty;

                            model.TradeLicenseExpiryDate = !string.IsNullOrEmpty(profileDetails.tradelicensevalidto) && profileDetails.tradelicensevalidto != "00000000"
                               ? DateTime.ParseExact(profileDetails.tradelicensevalidto, "ddMMyyyy",
                                          System.Globalization.CultureInfo.InvariantCulture).ToString("dd MMMM yyyy") : string.Empty;
                            model.CompanyName = profileDetails.companyname;
                            model.CompanyTelephone = !string.IsNullOrEmpty(profileDetails.telephone) ? profileDetails.telephone.RemoveMobileNumberZeroPrefix().Replace("+971", string.Empty) : string.Empty;

                            model.CompanyTelephoneExtension = profileDetails.extension;

                            model.VatRegistrationNo = profileDetails.vatregistrationnumber;
                            model.CompanyTelephoneCode = !string.IsNullOrEmpty(profileDetails.telephonedialcode) ? profileDetails.telephonedialcode : defaultPhoneCode;
                            model.CompanyMobileCode = !string.IsNullOrEmpty(profileDetails.mobiledialcode) ? profileDetails.mobiledialcode : defaultPhoneCode;


                        }
                        else if (profileDetails.bpcategory == MasarConfig.Individual)
                        {
                            model.EmailAddress = profileDetails.email;
                            model.MobileNumber = !string.IsNullOrEmpty(profileDetails.mobile) ? profileDetails.mobile.RemoveMobileNumberZeroPrefix().Replace("+971", string.Empty) : string.Empty;

                            if (!string.IsNullOrEmpty(model.MobileNumber) && model.MobileNumber[0] == '0')
                                model.MobileNumber = model.MobileNumber.Substring(1);

                            model.UserType = "I";
                            model.EmiratesID = profileDetails.emiratesid;
                            model.IndividualCompanyTelephone = profileDetails.telephone;
                            model.IndividualCompanyTelephoneExtension = profileDetails.extension;
                            model.Title = profileDetails.title;
                            model.FirstName = profileDetails.name1;
                            model.LastName = profileDetails.name2;
                            model.ExpiryDate = !string.IsNullOrEmpty(profileDetails.emiratevalidto) && profileDetails.emiratevalidto != "00000000"
                                ? DateTime.ParseExact(profileDetails.emiratevalidto, "ddMMyyyy",
                                           System.Globalization.CultureInfo.InvariantCulture).ToString("dd MMMM yyyy") : string.Empty;

                            model.MobileCode = !string.IsNullOrEmpty(profileDetails.mobiledialcode) ? profileDetails.mobiledialcode : defaultPhoneCode;
                            model.TelephoneCode = !string.IsNullOrEmpty(profileDetails.telephonedialcode) ? profileDetails.telephonedialcode : defaultPhoneCode;


                        }

                        model.Street = profileDetails.street;
                        model.POBox = profileDetails.pobox;
                        model.Countrykey = string.IsNullOrEmpty(profileDetails.country) ? "AE" : profileDetails.country;
                        model.City = profileDetails.country == "AE" ? profileDetails.city : string.Empty;
                        model.ActualCity = profileDetails.country == "AE" ? string.Empty : profileDetails.city;
                        model.Region = profileDetails.region;



                        return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SetProfileUpdate.cshtml", model);
                    }
                }
                else
                {
                    errVerf = response.Message;
                }

            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SetProfileUpdate.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale + "," + Roles.Miscellaneous)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetProfileUpdate(ProfileUpdateModel model)
        {

            // profileDetails.flag = model.SubmitType; //SND generate OTP ,  VER verify OTP , UPD Update backend after verification
            if (!string.IsNullOrEmpty(model.TradeLicenseIssueDate))
                model.TradeLicenseIssueDate = model.TradeLicenseIssueDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            if (!string.IsNullOrEmpty(model.TradeLicenseExpiryDate))
                model.TradeLicenseExpiryDate = model.TradeLicenseExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            if (!string.IsNullOrEmpty(model.ExpiryDate))
                model.ExpiryDate = model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");



            Profileinputs fetchUserInput = new Profileinputs()
            {
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId
            };

            var responseFetch = MasarClient.MasarProfileFetch(new MasarProfileFetchRequest()
            {
                profileinputs = fetchUserInput

            }, RequestLanguage, Request.Segment());

            if (responseFetch != null && responseFetch.Succeeded)
            {
                if (responseFetch.Payload.profilelist != null && responseFetch.Payload.profilelist.Count > 0)
                {
                    int _errors = 0;
                    string error = string.Empty;
                    bool isAttachmentError = false;

                    var profileDetails = responseFetch.Payload.profilelist.FirstOrDefault();

                    string _expirydate = !string.IsNullOrEmpty(profileDetails.tradelicensevalidto) && profileDetails.tradelicensevalidto != "00000000"
                          ? DateTime.ParseExact(profileDetails.tradelicensevalidto, "ddMMyyyy",
                                     System.Globalization.CultureInfo.InvariantCulture).ToString("dd MMMM yyyy") : string.Empty;


                    model.TradeLicenseIssuingAuthorityKey = model.IssueauthorityFlag == "E" ? model.TradeLicenseIssuingAuthorityKey : profileDetails.issuedby;
                    model.IssuingAuthorityDescription = model.TradeLicenseIssuingAuthorityKey == "OT" ? (model.IssueauthorityFlag == "E" ? model.IssuingAuthorityDescription : profileDetails.issueauthoritytext) : string.Empty;
                    model.CompanyName = profileDetails.companyname;
                    model.TradelicenseNumber = model.TradenumberFlag == "E" ? model.TradelicenseNumber : profileDetails.tradelicense;
                    model.Title = profileDetails.bpcategory == MasarConfig.Organization ? MasarConfig.Company : MasarConfig.Mr;
                    model.FirstName = string.IsNullOrEmpty(model.FirstName) ? profileDetails.name1 : model.FirstName;
                    model.LastName = string.IsNullOrEmpty(model.LastName) ? profileDetails.name2 : model.LastName;
                    model.EmiratesID = string.IsNullOrEmpty(model.EmiratesID) ? profileDetails.emiratesid : model.EmiratesID;
                    model.Countrykey = profileDetails.country;
                    model.UserType = profileDetails.bpcategory == MasarConfig.Organization ? "O" : "I";

                    if (_expirydate != model.TradeLicenseExpiryDate && model.UserType == "O" && model.AttachmentFlag == "M")
                        model.IsTradeLicenseDocRequired = true;

                    List<Attachmentlist> registrationAttachments = new List<Attachmentlist>();


                    if (model.EIDAttachment != null)
                    {
                        string _ext = !string.IsNullOrEmpty(model.EIDAttachment.FileName) ? Path.GetExtension(model.EIDAttachment.FileName).ToLower() : string.Empty;

                        if (!AttachmentIsValid(model.EIDAttachment, General.MaxAttachmentSize, out error, General.AcceptedMasarFileTypes))
                        {
                            isAttachmentError = true;
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {

                            registrationAttachments.Add(new Attachmentlist()
                            {
                                filename = model.EIDAttachment.FileName,
                                filedata = Convert.ToBase64String(model.EIDAttachment.ToArray()),
                                mimetype = _ext == "pdf" ? "application/pdf" : _ext == "jpg" ? "image/jpg" : _ext == "jpeg" ? "image/jpg" : _ext == "png" ? "image/png" : string.Empty,
                                attachmenttype = "EM",
                            });
                        }
                    }


                    if (model.TradeLicenseAttachment != null)
                    {
                        string _ext = !string.IsNullOrEmpty(model.TradeLicenseAttachment.FileName) ? Path.GetExtension(model.TradeLicenseAttachment.FileName).ToLower() : string.Empty;

                        if (!AttachmentIsValid(model.TradeLicenseAttachment, General.MaxAttachmentSize, out error, General.AcceptedMasarFileTypes))
                        {
                            isAttachmentError = true;
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            registrationAttachments.Add(new Attachmentlist()
                            {
                                filename = model.TradeLicenseAttachment.FileName,
                                filedata = Convert.ToBase64String(model.TradeLicenseAttachment.ToArray()),
                                mimetype = _ext == "pdf" ? "application/pdf" : _ext == "jpg" ? "image/jpg" : _ext == "jpeg" ? "image/jpg" : _ext == "png" ? "image/png" : string.Empty,
                                attachmenttype = "TL",
                            });
                        }
                    }



                    if (ModelState.IsValid && !isAttachmentError)
                    {
                        var _mobNumber = model.UserType == "O" ? model.CompanyMobileNumber : model.MobileNumber;
                        if (_mobNumber[0] != '0')
                            _mobNumber = "0" + _mobNumber;


                        var _telephoneCode = model.UserType == "O" ? model.CompanyTelephoneCode : model.TelephoneCode;
                        if (string.IsNullOrEmpty(model.IndividualCompanyTelephone) && string.IsNullOrEmpty(model.CompanyTelephone))
                            _telephoneCode = string.Empty;

                        model.Countrykey = model.UserType == "I" ? "AE" : model.Countrykey;

                        var registrationInfoRequest = new RegistrationInfo()
                        {
                            country = model.Countrykey,
                            citycode = model.Countrykey == "AE" ? model.City : model.ActualCity,
                            email = model.UserType == "O" ? model.CompanyEmail : model.EmailAddress,
                            emiratesid = model.UserType == "I" ? model.EmiratesID : string.Empty,
                            firstname = model.UserType == "O" ? string.Empty : model.FirstName,
                            companyname = model.UserType == "O" ? model.CompanyName : string.Empty,
                            lastname = model.UserType == "O" ? string.Empty : model.LastName,
                            mobile = _mobNumber, //!string.IsNullOrWhiteSpace(model.MobileNumber) ? Convert.ToInt64(model.MobileNumber).ToString("0000000000") : "",
                            pobox = model.POBox,
                            region = model.Region,
                            street = model.Street,
                            telephone = model.UserType == "O" ? model.CompanyTelephone : model.IndividualCompanyTelephone,
                            extension = model.UserType == "O" ? model.CompanyTelephoneExtension : model.IndividualCompanyTelephoneExtension,
                            title = model.UserType == "O" ? MasarConfig.Company : MasarConfig.Mr,
                            tradelicense = model.UserType == "O" ? model.TradelicenseNumber : string.Empty,
                            tradelicensevalidfrom = model.UserType == "O" ? !string.IsNullOrEmpty(model.TradeLicenseIssueDate) ? DateTime.ParseExact(model.TradeLicenseIssueDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                            tradelicensevalidto = model.UserType == "O" ? !string.IsNullOrEmpty(model.TradeLicenseExpiryDate) ? DateTime.ParseExact(model.TradeLicenseExpiryDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                            eidexpirydate = model.UserType == "I" ? !string.IsNullOrEmpty(model.ExpiryDate) ? DateTime.ParseExact(model.ExpiryDate, "dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("ddMMyyyy") : string.Empty : string.Empty,
                            customercategory = model.UserType == "O" ? MasarConfig.Organization : MasarConfig.Individual,
                            customertype = _sType,//_ReturnURL == MasarConfig.ScrapSaleDashboardURL ? MasarConfig.ScrapSales : MasarConfig.Miscellaneous,
                            issueauthority = model.UserType == "O" ? model.TradeLicenseIssuingAuthorityKey : string.Empty,
                            mobiledialcode = model.UserType == "O" ? model.CompanyMobileCode : model.MobileCode,
                            telephonedialcode = _telephoneCode,
                            vatregistrationnumber = model.UserType == "O" ? model.VatRegistrationNo : string.Empty,
                            requesttype = MasarConfig.UpdateProfile,
                            sessionid = CurrentPrincipal.SessionToken,
                            userid = CurrentPrincipal.UserId,
                            noattachflag = registrationAttachments.Count() == 0 ? "X" : string.Empty

                        };

                        if (model.IsTradeLicenseDocRequired && model.TradeLicenseAttachment == null)
                        {
                            ModelState.AddModelError("", Translate.Text("Upload_TradeLicense"));
                            _errors++;


                        }
                        else
                        {

                            var response = MasarClient.ScrapRegistration(new MasarUserRegistrationRequest()
                            {
                                customerinputs = registrationInfoRequest

                            }, RequestLanguage, Request.Segment());


                            if (response != null && response.Succeeded && response.Payload.responsecode != null)
                            {
                                model.ApplicationNO = response.Payload.requestnumber;
                                if (registrationAttachments.Count() > 0)
                                {
                                    var AttachmentDetailsRequest = new Attachmentinputs()
                                    {
                                        appno = response.Payload.requestnumber,
                                        lastdoc = "X",
                                        userid = CurrentPrincipal.UserId,
                                        sessionid = CurrentPrincipal.SessionToken,
                                        requesttype = MasarConfig.UpdateProfile,
                                        reference = model.OtpRequestId,
                                        attachmentlist = registrationAttachments
                                    };


                                    var attachmentResponse = MasarClient.AddMasarAttachments(new MasarAttachmentRequest()
                                    {
                                        attachmentinputs = AttachmentDetailsRequest

                                    }, RequestLanguage, Request.Segment());

                                    if (attachmentResponse != null && attachmentResponse.Succeeded && attachmentResponse.Payload.responsecode != null)
                                    {
                                        model.IsSuccess = true;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", attachmentResponse.Payload.description);
                                        _errors++;
                                    }
                                }

                            }
                            else if (response.Payload != null)
                            {
                                ModelState.AddModelError("", response.Payload.description);
                                _errors++;
                            }
                            else if (response.Message != null)
                            {
                                ModelState.AddModelError("", response.Message);
                                _errors++;
                            }
                        }

                        if (_errors == 0)
                        {
                            model.IsSuccess = true;

                            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SetProfileUpdateConfirm.cshtml", model);

                            //return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SetProfileUpdate.cshtml", model);

                        }

                    }



                }
            }

            model.CountryList = GetCountry();
            model.RegionList = GetRegion(model.Countrykey);
            model.AreaList = GetArea("");
            model.TradeLicenseIssuingAuthority = GetIssuedBy().ToList();
            model.CompanyTelephoneCodeList = GetTelephoneCountryCode();

            if (!string.IsNullOrEmpty(model.Countrykey) && model.CountryList != null)
            {
                model.CountryList.Where(x => x.Value == model.Countrykey).FirstOrDefault().Selected = true;
            }

            if (!string.IsNullOrEmpty(model.Region) && model.AreaList != null)
            {
                model.AreaList.Where(x => x.Value == model.Region).FirstOrDefault().Selected = true;
            }

            if (!string.IsNullOrEmpty(model.City) && model.RegionList != null)
            {
                model.RegionList.Where(x => x.Value == model.City).FirstOrDefault().Selected = true;
            }

            if (!string.IsNullOrEmpty(model.TradeLicenseIssuingAuthorityKey) && model.TradeLicenseIssuingAuthority != null)
            {
                model.TradeLicenseIssuingAuthority.Where(x => x.Value == model.TradeLicenseIssuingAuthorityKey).FirstOrDefault().Selected = true;
            }

            return View("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SetProfileUpdate.cshtml", model);
        }

        private ServiceResponse<SetProfileUpdateResponse> UpdateProfileOTP(profileRequestDetails profileDetails)
        {
            var responseData = CustomerSmartSaleClient.SetProfileUpdate(new SetProfileUpdate()
            {
                profilerequest = profileDetails
            }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            return responseData;
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale)]
        [HttpGet]
        public ActionResult ProfileUpdateSuccess()
        {
            ProfileUpdateModel profileUpdateModel = null;
            if (CacheProvider.TryGet("ProfileUpdateModel", out profileUpdateModel))
            {
                CacheProvider.Remove("ProfileUpdateModel");
                return PartialView("~/Views/Feature/ScrapSale/ScrapSale/Phase-2/SetProfileUpdateConfirm.cshtml", new ProfileUpdateModel()
                {
                    //emailNotificationNumber = profileUpdateModel.emailNotificationNumber,
                    //mobileNotificationNumber = profileUpdateModel.mobileNotificationNumber,
                    //IsSuccess = true
                });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCRAPESALE_PROFILEUPDATE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ResendMobileEmailOTP()
        {
            ProfileUpdateModel otpModel = null;
            if (CacheProvider.TryGet("ProfileUpdateModel", out otpModel))
            {
                var response = CustomerSmartSaleClient.SetProfileUpdate(new SetProfileUpdate()
                {
                    profilerequest = new profileRequestDetails
                    {
                        //mobileotpflag = otpModel.mobileotpflag,
                        //customernumber = CurrentPrincipal.PrimaryAccount,
                        //flag = "SND",
                        //emailotpflag = otpModel.emailotpflag,
                        //emailaddress = !(otpModel.emailAddress.Equals(CurrentPrincipal.EmailAddress)) ? otpModel.emailAddress : "",
                        //newmobile = !(otpModel.mobileNo.Equals(CurrentPrincipal.MobileNumber)) ? otpModel.mobileNo : "",
                    }
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }

        #endregion Profile Update

        #region ClearSessionAndSignOut

        private void ClearSessionAndSignOut()
        {
            DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
        }

        #endregion ClearSessionAndSignOut

        #endregion Phase-2

        #region Masar-CVI

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale + "," + Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult BankList()
        {
            try
            {
                var response = MasarClient.GetBankList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, RequestLanguage, Request.Segment());

                ViewBag.BankNames = GetSelectItemsByKey("BANK", true);
                ViewBag.City = GetSelectItemsByKey("CITY", true);
                ViewBag.Currency = GetSelectItemsByKey("CURRENCY", true);
                ViewBag.Country = GetSelectItemsByKey("COUNTRY", true);
                ViewBag.Nationality = GetSelectItemsByKey("NATIONALITY", true);

                if (response != null && response.Succeeded && response.Payload != null && response.Payload.bankmasterdetails != null)
                {
                    return PartialView("~/Views/Feature/ScrapSale/ScrapSale/_BankList.cshtml", response.Payload);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/_BankList.cshtml", new MasarBankResponse());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale + "," + Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult UpdatePassword()
        {
            try
            {

                if (IsLoggedIn)
                {
                    ChangePasswordModel model = new ChangePasswordModel();
                    model.Type = _sType;
                    return PartialView("~/Views/Feature/ScrapSale/ScrapSale/ChangePassword.cshtml", model);
                }
                else
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);
                }

            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/ScrapSale/ScrapSale/ChangePassword.cshtml", null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale + "," + Roles.Miscellaneous)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //model.Type = StringExtensions.GetSanitizePlainText(model.Type);
                    //model.OldPassword = StringExtensions.GetSanitizePlainText(model.OldPassword);
                    //model.Password = StringExtensions.GetSanitizePlainText(model.Password);
                    //model.ConfirmPassword = StringExtensions.GetSanitizePlainText(model.ConfirmPassword);
                    //model.IsSuccess = StringExtensions.GetSanitizePlainText(model.IsSuccess);

                    MasarChangePasswordRequest req = new MasarChangePasswordRequest();
                    req.changepwdinput.userid = CurrentPrincipal.UserId;
                    req.changepwdinput.sessionid = CurrentPrincipal.SessionToken;
                    req.changepwdinput.oldpassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.OldPassword));
                    req.changepwdinput.newpassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.Password));
                    req.changepwdinput.type = model.Type;

                    var response = MasarClient.UpdatePassword(req, RequestLanguage, Request.Segment());

                    if (response != null && response.Succeeded)
                    {
                        return Json(new { status = true, desc = response.Message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, desc = response.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    //ViewData.ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.ScrapeSale + "," + Roles.Miscellaneous)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddBank(AddBankModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //model.BankName = StringExtensions.GetSanitizePlainText(model.BankName);
                    //model.Country = StringExtensions.GetSanitizePlainText(model.Country);
                    //model.AccountNumber = StringExtensions.GetSanitizePlainText(model.AccountNumber);
                    //model.AccountHolderName = StringExtensions.GetSanitizePlainText(model.AccountHolderName);
                    //model.Currency = StringExtensions.GetSanitizePlainText(model.Currency);
                    //model.SwiftCode = StringExtensions.GetSanitizePlainText(model.SwiftCode);
                    //model.IFSCCode = StringExtensions.GetSanitizePlainText(model.IFSCCode);
                    //model.IsCorrespondent = StringExtensions.GetSanitizePlainText(model.IsCorrespondent);
                    //model.BankNameCorrespondent = StringExtensions.GetSanitizePlainText(model.BankNameCorrespondent);
                    //model.CountryCorrespondent = StringExtensions.GetSanitizePlainText(model.CountryCorrespondent);
                    //model.CityCorrespondent = StringExtensions.GetSanitizePlainText(model.CityCorrespondent);
                    //model.Address = StringExtensions.GetSanitizePlainText(model.Address);
                    //model.City = StringExtensions.GetSanitizePlainText(model.City);

                    MasarAddBankRequest req = new MasarAddBankRequest();
                    req.addbankinputs.userid = CurrentPrincipal.UserId;
                    req.addbankinputs.sessionid = CurrentPrincipal.SessionToken;

                    if (model.Attachment != null)
                    {
                        string _ext = !string.IsNullOrEmpty(model.Attachment.FileName) ? Path.GetExtension(model.Attachment.FileName).ToLower() : string.Empty;
                        req.addbankinputs.attachmentlist.Add(new Attachmentlist()
                        {
                            filename = model.Attachment.FileName,
                            filedata = Convert.ToBase64String(model.Attachment.ToArray()),
                            mimetype = _ext == "pdf" ? "application/pdf" : _ext == "jpg" ? "image/jpg" : _ext == "jpeg" ? "image/jpg" : _ext == "png" ? "image/png" : string.Empty,
                        });
                    }

                    if (!string.IsNullOrEmpty(model.IsCorrespondent) && model.IsCorrespondent == "1")
                    {
                        req.addbankinputs.correspondence = "x";
                        req.addbankinputs.correspondentbankregion = model.CountryCorrespondent;
                        req.addbankinputs.correspondentbankname = model.BankNameCorrespondent;
                        req.addbankinputs.correspondentbankaddress = model.Address;
                        //req.addbankinputs.city = model.CityCorrespondent;
                        req.addbankinputs.correspondentswift = model.SwiftCodeCorespondent;
                    }
                    else
                    {
                        req.addbankinputs.correspondence = "";
                    }
                    if (model.Country == "AE")
                        req.addbankinputs.iban = model.IBAN;
                    else
                        req.addbankinputs.route = model.IFSCCode;


                    req.addbankinputs.currency = model.Currency;
                    req.addbankinputs.swift = model.SwiftCode;
                    req.addbankinputs.accountholder = model.AccountHolderName;
                    req.addbankinputs.accountnumber = model.AccountNumber;
                    req.addbankinputs.region = model.Country;
                    req.addbankinputs.bankname = model.BankName;
                    req.addbankinputs.address = model.AddressBank;

                    var response = MasarClient.AddBank(req, RequestLanguage, Request.Segment());

                    if (response != null && response.Succeeded)
                    {
                        return Json(new { status = true, desc = response.Message, appnum = response.Payload != null ? response.Payload.referencenumber : string.Empty }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, desc = response.Message, appnum = string.Empty }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    TempData["IsSuccess"] = "";
                    return Json(new { status = false, desc = Translate.Text("Unexpected error"), appnum = string.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false, desc = Translate.Text("Unexpected error") }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PrivateMethods
        private IEnumerable<SelectListItem> GetSelectItemsByKey(string key, bool setDefault = false)
        {
            List<SelectListItem> ddrValues = null;
            List<SelectListItem> data = new List<SelectListItem>();
            if (setDefault)
            {
                data.Add(new SelectListItem() { Text = Translate.Text("defaultSelect"), Value = "" });
            }
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    var dropdownValues = GetAllDropdownValues();

                    if (dropdownValues.Succeeded && dropdownValues.Payload.dropdownlist != null)
                    {
                        var SelectItems = dropdownValues.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == key.Trim());

                        if (SelectItems != null)
                        {
                            ddrValues = SelectItems.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                        }

                        else
                        {
                            CacheProvider.Remove("scrap_alldropdowns");

                            dropdownValues = GetAllDropdownValues();

                            SelectItems = dropdownValues.Payload.dropdownlist.FirstOrDefault(x => x.fieldname == key.Trim());

                            ddrValues = SelectItems.values.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                        }

                        data.AddRange(ddrValues);

                        return data;
                    }
                }
            }
            catch (System.Exception ex)
            {
                CacheProvider.Remove("scrap_alldropdowns");
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            finally
            {
                ddrValues = null;
            }
            return Enumerable.Empty<SelectListItem>();
        }
        #endregion
    }
}