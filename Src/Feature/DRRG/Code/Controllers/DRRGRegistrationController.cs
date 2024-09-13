// <copyright file="DRRGRegistrationController.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Controllers
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.CustomDB.DRRGDataModel;
    using DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG;
    using DEWAXP.Feature.DRRG.Models;
    using Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Data.Entity.Core.Objects;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Feature.DRRG.Filters.Mvc;
    using System.Globalization;
    using System.Data;
    using Exception = System.Exception;
    using EntityFrameworkExtras.EF6;
    using DEWAXP.Foundation.Content.Models.AccountModel;
    using System.Web;
    using System.IO;
    using DEWAXP.Foundation.Helpers.Extensions;
    using LoginModel = DEWAXP.Feature.DRRG.Models.LoginModel;
    using DEWAXP.Foundation.Content.Services;
    using System.Security.Cryptography;
    using DEWAXP.Foundation.Content.Repositories;
    using System.Threading;

    /// <summary>
    /// Defines the <see cref="DRRGRegistrationController" />.
    /// </summary>
    public class DRRGRegistrationController : DRRGBaseController
    {
        public long FileSizeLimit = 2048000;
        public string[] supportedTypes = new[] { ".jpg", ".png", ".jpeg", ".pdf", ".doc", ".PDF", ".PNG", ".JPG", ".JPEG", ".DOCX" };
        public string[] AuthorizationLetterSupportedTypes = new[] { ".PDF" };
        public string[] supportedLogoTypes = new[] { ".jpg", ".png", ".jpeg", ".PNG", ".JPG", ".JPEG" };
        /// <summary>
        /// The Registration.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Registration()
        {
            RegistrationViewModel model = new RegistrationViewModel { CountryMobileList = GetCountryMobilelist(), NationalityList = GetCountrylist() };
            model.UserLocalRepresentative = true;
            return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", model);
        }

        /// <summary>
        /// The Registration.
        /// </summary>
        /// <param name="model">The model<see cref="RegistrationViewModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Registration(RegistrationViewModel model)
        {
            string error = string.Empty;

            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            model.ManufacturerCode = ManufacturerCode();

            Addfile(model.TradeMarkLogo, FileType.TrademarkLogo, FileEntity.Manufacturer, model.ManufacturerCode, error, FileSizeLimit, supportedLogoTypes, DRRGStandardValues.Registration, dRRG_Files_TYlist);
            if (model.UserLocalRepresentative)
            {
                Addfile(model.TradeLicenseDocument, FileType.TradeLicenseDoc, FileEntity.Manufacturer, model.ManufacturerCode, error, FileSizeLimit, supportedTypes, DRRGStandardValues.Registration, dRRG_Files_TYlist);
            }

            if (model.SupportingDocument != null && model.SupportingDocument.Count > 0)
            {
                foreach (System.Web.HttpPostedFileBase supportingdocument in model.SupportingDocument)
                {
                    Addfile(supportingdocument, FileType.SupportingDocument, FileEntity.Manufacturer, model.ManufacturerCode, error, FileSizeLimit, supportedTypes, DRRGStandardValues.Registration, dRRG_Files_TYlist);
                }

            }
            //if (!string.IsNullOrWhiteSpace(model.signatureCopy))
            //{
            //    var sizeInBytes = Math.Ceiling((double)model.signatureCopy.Length / 4) * 3;
            //    //var sizeInKb = sizeInBytes / 1000;
            //    dRRG_Files_TYlist.Add(new DRRG_Files_TY
            //    {
            //        Name = model.ManufacturerCode + "_Signature.png",
            //        ContentType = "image/png",
            //        Extension = "png",
            //        File_Type = FileType.SignatureCopy,
            //        Entity = FileEntity.Manufacturer,
            //        Content = Convert.FromBase64String(model.signatureCopy),
            //        Size = Convert.ToString(sizeInBytes),
            //        Reference_ID = model.ManufacturerCode,
            //        Manufacturercode = DRRGStandardValues.Registration
            //    });
            //}
            if (model.factory != null && model.factory.Length > 0)
            {
                model.factory.ToList().ForEach(x => x.FactoryCode = FactoryCode());
                foreach (Factory factory in model.factory)
                {
                    Addfile(factory.EnvironmentalFile, FileType.EnvironmentalManagementSupport, FileEntity.Factory, factory.FactoryCode, error, FileSizeLimit, supportedTypes, DRRGStandardValues.Registration, dRRG_Files_TYlist);
                    Addfile(factory.EOLFile, FileType.EOLPVModule, FileEntity.Factory, factory.FactoryCode, error, FileSizeLimit, supportedTypes, DRRGStandardValues.Registration, dRRG_Files_TYlist);
                    Addfile(factory.QMSFile, FileType.QualityManagementSupport, FileEntity.Factory, factory.FactoryCode, error, FileSizeLimit, supportedTypes, DRRGStandardValues.Registration, dRRG_Files_TYlist);
                    if (string.IsNullOrWhiteSpace(factory.FactoryFullName))
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Add Factory: Please enter factory name"));
                    }
                    if (string.IsNullOrWhiteSpace(factory.FactoryCountry))
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Add Factory: Please select country"));
                    }
                    if (string.IsNullOrWhiteSpace(factory.FactoryAddress))
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Add Factory: Please enter factory address"));
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Add Factory"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<DRRG_Factory_Details_TY> dRRG_Factory_Details_TY = new List<DRRG_Factory_Details_TY>();
                    model.factory.ToList().ForEach(x =>
                    dRRG_Factory_Details_TY.Add(
                        new DRRG_Factory_Details_TY
                        {
                            Address = x.FactoryAddress,
                            Country = !string.IsNullOrWhiteSpace(x.FactoryCountry) ? x.FactoryCountry : "United Arab Emirates",
                            EOL_PV_Module = x.EOLPVModule,
                            Factory_Name = x.FactoryFullName,
                            Factory_Code = x.FactoryCode,
                            Manufacturer_Code = model.ManufacturerCode
                        }
                    ));
                    List<DRRG_Manufacturer_Details_TY> dRRG_Manufacturer_Details_TY = new List<DRRG_Manufacturer_Details_TY>() {
                        new DRRG_Manufacturer_Details_TY
                        {
                            Brand_Name = (!string.IsNullOrWhiteSpace(model.BrandName) ? model.BrandName.Trim() : string.Empty),
                            Manufacturer_Name = (!string.IsNullOrWhiteSpace(model.ManufacturerFullName) ? model.ManufacturerFullName.Trim() : string.Empty),
                            Manufacturer_Country =model.Manufacturercountry,
                            Corporate_Email = (!string.IsNullOrWhiteSpace(model.ManufacturerEmailAddress) ? model.ManufacturerEmailAddress.Trim(): string.Empty),
                            Corporate_Phone_Number = (!string.IsNullOrWhiteSpace(model.ManufacturerPhoneNumber) ? model.ManufacturerPhoneNumber.Trim() : string.Empty),
                            Corporate_Phone_Code = (!string.IsNullOrWhiteSpace(model.ManufacturerPhonecode) ? model.ManufacturerPhonecode : string.Empty),
                            Corporate_Fax_Number = (!string.IsNullOrWhiteSpace(model.ManufacturerFaxNumber) ? model.ManufacturerFaxNumber.Trim() : string.Empty),
                            Corporate_Fax_Code = (!string.IsNullOrWhiteSpace(model.ManufacturerFaxcode) ? model.ManufacturerFaxcode : string.Empty),
                            Local_Representative =  model.UserLocalRepresentative,
                            User_Designation = (!string.IsNullOrWhiteSpace(model.UserDesignation) ? model.UserDesignation.Trim() : string.Empty),
                            User_Email_Address = (!string.IsNullOrWhiteSpace(model.UserEmail) ? model.UserEmail.Trim() : string.Empty),
                            User_First_Name = (!string.IsNullOrWhiteSpace(model.Userfirstname) ? model.Userfirstname.Trim() : string.Empty ),
                            User_Last_Name = (!string.IsNullOrWhiteSpace(model.UserLastName) ? model.UserLastName.Trim() : string.Empty),
                            User_Gender = model.UserGender?UserGender.Male:UserGender.Female,
                            User_Mobile_Number = (!string.IsNullOrWhiteSpace(model.UserPhoneNumber) ? model.UserPhoneNumber.Trim() : string.Empty),
                            User_Mobile_Code = model.UserPhonecode,
                            User_Nationality = model.UserCountry,
                            Company_Full_Name = model.UserLocalRepresentative ? model.Userrepresentativename.Trim() : string.Empty,
                            Website = model.Website,
                            Manufacturer_Code = model.ManufacturerCode
                        } };
                    Proc_DRRG_InsertManufacturer procedure = new Proc_DRRG_InsertManufacturer()
                    {
                        useremail = model.UserEmail,
                        dRRG_Manufacturer_Details_TY = dRRG_Manufacturer_Details_TY,
                        dRRG_Factory_Details_TY = dRRG_Factory_Details_TY,
                        dRRG_Files_TY = dRRG_Files_TYlist
                    };
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        context.Database.ExecuteStoredProcedure(procedure);
                        string errormessage = procedure.error;
                        if (!string.IsNullOrWhiteSpace(procedure.error))
                        {
                            ModelState.AddModelError(string.Empty, GetDRRGErrormessage(procedure.error));
                        }
                        else
                        {
                            ViewBag.Email = model.UserEmail;
                            string linktext = GetEncryptedLinkExpiryURL(model.UserEmail, DRRGStandardValues.Registration);
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                            context.SP_DRRG_Insertpasswordlink(model.UserEmail, linktext, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.UserExists))
                            {
                                SendDRRGRegistrationEmail(model.Userfirstname + " " + model.UserLastName, model.UserEmail, model.UserLocalRepresentative, linktext, DRRGStandardValues.Registration, model.ManufacturerFullName, model.ManufacturerCode);
                                return View("~/Views/Feature/DRRG/Registration/RegistrationSuccess.cshtml");
                            }
                            else
                            {
                                LogService.Error(new Exception(myString), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            model.CountryMobileList = GetCountryMobilelist();
            model.NationalityList = GetCountrylist();
            return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.DRRG))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
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
            return View("~/Views/Feature/DRRG/Account/_ForgotPassword.cshtml", new ForgotPasswordV1Model());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordV1Model model)
        {
            bool status = false;
            try
            {
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    status = ReCaptchaHelper.RecaptchaResponse(Request["g-recaptcha-response"]);
                }
                if (status)
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.Username))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            string linktext = GetEncryptedLinkExpiryURL(model.Username, DRRGStandardValues.ForgotPassword);
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                            context.SP_DRRG_Insertpasswordlink(model.Username, linktext, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.UserExists))
                            {
                                ObjectParameter myOutputParamName = new ObjectParameter("fullname", typeof(string));
                                context.DRRG_GetUserDetail_ByEmail(model.Username, myOutputParamName);
                                SendDRRGForgotpasswordEmail(myOutputParamName.Value != null ? myOutputParamName.Value.ToString() : "User", model.Username, linktext);
                                return View("~/Views/Feature/DRRG/Account/_ForgotPasswordSuccess.cshtml", model);
                            }
                            else
                            {
                                LogService.Error(new Exception(myString), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("DRRG_Invalid_Captcha"));
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
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                status = ReCaptchaHelper.RecaptchaResponse(Request["g-recaptcha-response"]);
            }

            return View("~/Views/Feature/DRRG/Account/_ForgotPassword.cshtml", new ForgotPasswordV1Model());
        }


        [HttpGet]
        public ActionResult SetPassword(string code)
        {
            SetNewPassword model = new SetNewPassword { Success = false };
            if (!string.IsNullOrWhiteSpace(code))
            {
                string codeunchanged = code;
                code = code.Trim().Replace(" ", "+");
                string userid, error, module;
                if (GetDecryptedValues(code, out userid, out error, out module))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                        context.SP_DRRG_UserCheck(userid, code, myOutputParamresponse);
                        string myString = Convert.ToString(myOutputParamresponse.Value);
                        if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.UserExists))
                        {
                            model.Username = code;
                            model.CodeUnchanged = code;
                            model.Success = true;

                        }
                        else
                        {
                            LogService.Error(new Exception(myString), this);
                            ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
            }
            return View("~/Views/Feature/DRRG/Account/SetNewPassword.cshtml", model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetPassword(SetNewPassword model)
        {
            string userid, error, module;
            model.Success = false;
            if (GetDecryptedValues(model.Username, out userid, out error, out module))
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                    context.SP_DRRG_UserCheck(userid, model.CodeUnchanged, myOutputParamresponse);
                    string myString = Convert.ToString(myOutputParamresponse.Value);
                    if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.UserExists))
                    {
                        ObjectParameter myOutputParamresponse1 = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                        context.SP_DRRG_SetNewPassword(userid, Convert.ToString(model.Password), myOutputParamresponse1);
                        string myString1 = Convert.ToString(myOutputParamresponse1.Value);
                        if (!string.IsNullOrWhiteSpace(myString1) && myString1.Equals(DRRGStandardValues.Success))
                        {
                            ViewBag.SuccessTitle = Translate.Text("DRRG.PasswordResetSuccessful");
                            ViewBag.Subtitle = GetDRRGSetPasswordmessage(module);
                            model.Success = true;
                            ObjectParameter myOutputParamName = new ObjectParameter("fullname", typeof(string));
                            context.DRRG_GetUserDetail_ByEmail(userid, myOutputParamName);
                            SendDRRGSetpasswordEmail(myOutputParamName.Value != null ? myOutputParamName.Value.ToString() : "User", userid, module);
                            return View("~/Views/Feature/DRRG/Account/SetNewPasswordSuccess.cshtml", model);
                        }
                    }
                    else
                    {
                        LogService.Error(new Exception(myString), this);
                    }
                }
            }
            ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
            return View("~/Views/Feature/DRRG/Account/SetNewPassword.cshtml", model);
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult Login(string returnUrl)
        {
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.DRRG) && !string.IsNullOrWhiteSpace(CurrentPrincipal.LastLogin))
            {
                if (CurrentPrincipal.LastLogin.Equals(DRRGStandardValues.Success))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
                }
                else if (CurrentPrincipal.LastLogin.Equals(DRRGStandardValues.Rejected))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_REJECTED);
                }
                else if (CurrentPrincipal.LastLogin.Equals(DRRGStandardValues.Usernotevaluated))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_AUTHORIZED_LETTER);
                }
                else if (CurrentPrincipal.LastLogin.Equals(DRRGStandardValues.UserPendingEvaualtion))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_PENDING_EVALUATION);
                }
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ClearCookiesSignOut();
                ViewBag.ReturnUrl = returnUrl;
            }

            return View("~/Views/Feature/DRRG/Account/Login.cshtml");
        }

        [HttpPost, AntiForgeryHandleErrorAttributeDRRG, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                            ObjectParameter SessionParamresponse = new ObjectParameter(DRRGStandardValues.sessionout, typeof(string));
                            ObjectParameter NameParamresponse = new ObjectParameter(DRRGStandardValues.name, typeof(string));
                            ObjectParameter ManufacturecodeParamresponse = new ObjectParameter(DRRGStandardValues.manufactercode, typeof(string));
                            context.SP_DRRG_Login(model.Username, model.Password, myOutputParamresponse, SessionParamresponse, NameParamresponse, ManufacturecodeParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            string session = Convert.ToString(SessionParamresponse.Value);
                            string name = Convert.ToString(NameParamresponse.Value);
                            string manucode = Convert.ToString(ManufacturecodeParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && !string.IsNullOrWhiteSpace(session) && !string.IsNullOrWhiteSpace(manucode) &&
                                (myString.Equals(DRRGStandardValues.Success) || myString.Equals(DRRGStandardValues.Rejected) || myString.Equals(DRRGStandardValues.Usernotevaluated) || myString.Equals(DRRGStandardValues.UserPendingEvaualtion)))
                            {
                                AuthStateService.Save(new DewaProfile(model.Username, session, Roles.DRRG) { BusinessPartner = manucode, Name = name, LastLogin = myString });
                                returnUrl = HttpUtility.UrlDecode(returnUrl);
                                if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
                                {
                                    Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddMinutes(60);
                                }
                                System.Web.HttpContext.Current.Session.Timeout = 60;
                                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }
                                if (myString.Equals(DRRGStandardValues.Success))
                                {
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
                                }
                                else if (myString.Equals(DRRGStandardValues.Rejected))
                                {
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_REJECTED);
                                }
                                else if (myString.Equals(DRRGStandardValues.Usernotevaluated))
                                {
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_AUTHORIZED_LETTER);
                                }
                                else if (myString.Equals(DRRGStandardValues.UserPendingEvaualtion))
                                {
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_PENDING_EVALUATION);
                                }
                                else if (myString.Equals(DRRGStandardValues.Incorrectpassword))
                                {
                                    ModelState.AddModelError(string.Empty, GetDRRGErrormessage(DRRGStandardValues.Incorrectpassword));
                                }
                            }
                            else
                            {
                                //LogService.Error(new Exception(myString), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }

            }
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Feature/DRRG/Account/Login.cshtml", model);
        }

        [TwoPhaseDRRGAuthorize(NotEvaluated = true), HttpGet]
        public ActionResult AuthorizationLetter()
        {
            return View("~/Views/Feature/DRRG/Registration/AuthorizationLetter.cshtml");
        }
        [TwoPhaseDRRGAuthorize(NotEvaluated = true), HttpPost, ValidateAntiForgeryToken]
        public ActionResult AuthorizationLetter(AuthorizationLetterModel model)
        {
            if (model != null && model.AuthorizationLetter != null)
            {
                string error = string.Empty;
                List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();

                Addfile(model.AuthorizationLetter, FileType.AuthorizationLetter, FileEntity.Manufacturer, CurrentPrincipal.BusinessPartner, error, FileSizeLimit, AuthorizationLetterSupportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                if (string.IsNullOrWhiteSpace(error))
                {
                    Proc_DRRG_InsertAuthorizationLetter procedure = new Proc_DRRG_InsertAuthorizationLetter()
                    {
                        useremail = CurrentPrincipal.Username,
                        manufacturecode = CurrentPrincipal.BusinessPartner,
                        dRRG_Files_TY = dRRG_Files_TYlist
                    };
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        context.Database.ExecuteStoredProcedure(procedure);
                        string errormessage = procedure.error;
                        if (!string.IsNullOrWhiteSpace(procedure.error) && procedure.error.Equals(DRRGStandardValues.Success))
                        {
                            SendDRRGModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, CurrentPrincipal.BusinessPartner, DRRGStandardValues.AuthorizedLetterSubmitted);
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                            context.SP_DRRG_Logout(CurrentPrincipal.Username, CurrentPrincipal.SessionToken, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);

                            QueryString q1 = new QueryString(true);
                            q1.With("code", CurrentPrincipal.BusinessPartner, true);

                            ClearCookiesSignOut();

                            CacheProvider.Store("authorizationlettersuccess", new AccessCountingCacheItem<bool>(true, Times.Exactly(1)));

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_AUTHORIZATIONLETTERSUCCESS, q1);

                            //return View("~/Views/Feature/DRRG/Registration/AuthorizationLetterSuccess.cshtml");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, GetDRRGErrormessage(procedure.error));
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "attach the file");
            }
            return View("~/Views/Feature/DRRG/Registration/AuthorizationLetter.cshtml");
        }

        [HttpGet]
        public ActionResult AuthorizationLetterSuccess()
        {
            try
            {
                bool isSuccess = false;
                if (Request.QueryString["code"] != null && !string.IsNullOrEmpty(Request.QueryString["code"].ToString()))
                {
                    ViewBag.manufacturecode = Request.QueryString["code"].ToString();

                    if (CacheProvider.TryGet("authorizationlettersuccess", out isSuccess) && isSuccess)
                    {

                        return View("~/Views/Feature/DRRG/Registration/AuthorizationLetterSuccess.cshtml");
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
        }

        [TwoPhaseDRRGAuthorize(NotEvaluated = true), HttpGet]
        public ActionResult AuthorizationSucess()
        {
            return View("~/Views/Feature/DRRG/Registration/AuthorizationLetterSuccess.cshtml");
        }
        [TwoPhaseDRRGAuthorize(EvaluatedPending = true), HttpGet]
        public ActionResult PendingEvaluation()
        {
            ViewBag.RefNo = CurrentPrincipal.BusinessPartner;
            return View("~/Views/Feature/DRRG/Registration/PendingEvaulation.cshtml");
        }
        [TwoPhaseDRRGAuthorize(EvaluatedRejected = true), HttpGet]
        public ActionResult Rejection()
        {
            ViewBag.RefNo = CurrentPrincipal.BusinessPartner;
            return View("~/Views/Feature/DRRG/Registration/Rejection.cshtml");
        }
        [TwoPhaseDRRGAuthorize(EvaluatedRejected = true), HttpGet]
        public ActionResult RejectedApplication()
        {
            RegistrationViewModel model = new RegistrationViewModel { CountryMobileList = GetCountryMobilelist(), NationalityList = GetCountrylist() };
            using (DRRGEntities context = new DRRGEntities())
            {
                var result = context.SP_DRRG_GETManufacturerdetails(CurrentPrincipal.BusinessPartner, CurrentPrincipal.UserId).ToList();
                if (result != null && result.Count() > 0)
                {
                    var resultitem = result.FirstOrDefault();
                    var factorydetails = context.SP_DRRG_GETFactorydetails(CurrentPrincipal.BusinessPartner, CurrentPrincipal.UserId).ToList();
                    var resultfiles = context.SP_DRRG_GETFilesbyIDAdmin(CurrentPrincipal.BusinessPartner).ToList();
                    if (resultfiles != null)
                    {
                        foreach (var item in resultfiles)
                        {
                            if (!model.FileList.ContainsKey(item.File_ID.ToString()) && item.File_Type == FileType.SupportingDocument)
                                model.FileList.Add(item.File_ID.ToString(), new fileResult { fileId = item.File_ID, fileName = item.Name, content = item.Content });
                        }
                    }
                    model.ManufacturerId = resultitem.Manufacturer_ID;
                    model.ManufacturerCode = resultitem.Manufacturer_Code;
                    model.ManufacturerFullName = resultitem.Manufacturer_Name;
                    model.BrandName = resultitem.Brand_Name;
                    model.Manufacturercountry = resultitem.Manufacturer_Country;
                    model.ManufacturerEmailAddress = resultitem.Corporate_Email;
                    model.ManufacturerPhoneNumber = resultitem.Corporate_Phone_Number;
                    model.ManufacturerPhonecode = resultitem.Corporate_Phone_Code;
                    model.ManufacturerFaxcode = resultitem.Corporate_Fax_Code;
                    model.ManufacturerFaxNumber = resultitem.Corporate_Fax_Number;
                    model.Website = resultitem.Website;
                    model.UserLocalRepresentative = resultitem.Local_Representative;
                    model.UserDesignation = resultitem.User_Designation;
                    model.UserEmail = resultitem.User_Email_Address;
                    model.Userfirstname = resultitem.User_First_Name;
                    model.UserLastName = resultitem.User_Last_Name;
                    model.UserGender = !string.IsNullOrWhiteSpace(resultitem.User_Gender) && resultitem.User_Gender.Equals(UserGender.Male); //= model.UserGender ? UserGender.Male : UserGender.Female,
                    model.UserPhoneNumber = resultitem.User_Mobile_Number;
                    model.UserPhonecode = resultitem.User_Mobile_Code;
                    model.UserCountry = resultitem.User_Nationality;
                    model.Userrepresentativename = resultitem.Company_Full_Name;
                    model.updateprofile = true;
                    if (factorydetails != null && factorydetails.Count() > 0)
                    {
                        List<Factory> factories = new List<Factory>();
                        factorydetails.ToList().ForEach(x =>
                    factories.Add(
                        new Factory
                        {
                            FactoryAddress = x.Address,
                            FactoryCountry = x.Country,
                            EOLPVModule = x.EOL_PV_Module,
                            FactoryFullName = x.Factory_Name,
                            FactoryCode = x.Factory_Code,
                            EOLFileBinary = resultfiles.Where(f => f.File_Type == FileType.EOLPVModule)?.Select(f => f.Content)?.FirstOrDefault(),
                            EOLFileName = resultfiles.Where(f => f.File_Type == FileType.EOLPVModule)?.Select(f => f.Name)?.FirstOrDefault(),
                            QMSFileBinary = resultfiles.Where(f => f.File_Type == FileType.QualityManagementSupport)?.Select(f => f.Content)?.FirstOrDefault(),
                            QMSFileName = resultfiles.Where(f => f.File_Type == FileType.QualityManagementSupport)?.Select(f => f.Name)?.FirstOrDefault(),
                            EnvironmentalFileBinary = resultfiles.Where(f => f.File_Type == FileType.EnvironmentalManagementSupport)?.Select(f => f.Content)?.FirstOrDefault(),
                            EnvironmentalFileName = resultfiles.Where(f => f.File_Type == FileType.EnvironmentalManagementSupport)?.Select(f => f.Name)?.FirstOrDefault(),
                        }));
                        model.factory = factories.ToArray();
                    }

                    model.TradeMarkLogoBinary = resultfiles.Where(x => x.File_Type == FileType.TrademarkLogo)?.Select(x => x.Content)?.FirstOrDefault();
                    model.TradeLicenseDocumentBinary = resultfiles.Where(x => x.File_Type == FileType.TradeLicenseDoc)?.Select(x => x.Content)?.FirstOrDefault();
                    model.TradeMarkLogo_FileName = resultfiles.Where(x => x.File_Type == FileType.TrademarkLogo)?.Select(x => x.Name)?.FirstOrDefault();
                    model.TradeLicenseDocument_FileName = resultfiles.Where(x => x.File_Type == FileType.TradeLicenseDoc)?.Select(x => x.Name)?.FirstOrDefault();

                    return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                }
            }
            return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", null);
        }

        [HttpPost, TwoPhaseDRRGAuthorize(EvaluatedRejected = true)]
        public ActionResult RejectedApplication(RegistrationViewModel model)
        {
            string error = string.Empty;

            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            model.ManufacturerCode = CurrentPrincipal.BusinessPartner;

            Addfile(model.TradeMarkLogo, FileType.TrademarkLogo, FileEntity.Manufacturer, model.ManufacturerCode, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);

            if (model.SupportingDocument != null && model.SupportingDocument.Count > 0)
            {
                foreach (HttpPostedFileBase supportingdocument in model.SupportingDocument)
                {
                    Addfile(supportingdocument, FileType.SupportingDocument, FileEntity.Manufacturer, model.ManufacturerCode, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                }
            }

            if (model.factory != null && model.factory.Length > 0)
            {
                foreach (Factory factory in model.factory)
                {
                    Addfile(factory.EnvironmentalFile, FileType.EnvironmentalManagementSupport, FileEntity.Factory, factory.FactoryCode, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(factory.EOLFile, FileType.EOLPVModule, FileEntity.Factory, factory.FactoryCode, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(factory.QMSFile, FileType.QualityManagementSupport, FileEntity.Factory, factory.FactoryCode, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Please add factory"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                #region [Update Signature]
                                if (!string.IsNullOrWhiteSpace(model.signatureCopy))
                                {
                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == model.ManufacturerCode.ToLower() && x.File_Type == FileType.SignatureCopy).ToList();
                                    foreach (var item in rRG_Files)
                                    {
                                        context.DRRG_Files.Remove(item);
                                        context.SaveChanges();
                                    }
                                    var sizeInBytes = Math.Ceiling((double)model.signatureCopy.Length / 4) * 3;
                                    context.DRRG_Files.Add(new DRRG_Files
                                    {
                                        Name = model.ManufacturerCode + "_Signature.png",
                                        ContentType = "image/png",
                                        Extension = "png",
                                        File_Type = FileType.SignatureCopy,
                                        Entity = FileEntity.PVmodule,
                                        Content = Convert.FromBase64String(model.signatureCopy),
                                        Size = Convert.ToString(sizeInBytes),
                                        Reference_ID = model.ManufacturerCode,
                                        Manufacturer_Code = DRRGStandardValues.Registration
                                    });
                                    context.SaveChanges();
                                }
                                #endregion

                                #region [Update Trade Mark Logo]
                                if (model.TradeMarkLogo != null && model.TradeMarkLogo.ContentLength > 0)
                                {
                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == model.ManufacturerCode.ToLower() && x.File_Type == FileType.TrademarkLogo).ToList();
                                    foreach (var item in rRG_Files)
                                    {
                                        context.DRRG_Files.Remove(item);
                                        context.SaveChanges();
                                    }
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.TradeMarkLogo.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.TradeMarkLogo.FileName,
                                            ContentType = model.TradeMarkLogo.ContentType,
                                            Extension = model.TradeMarkLogo.GetTrimmedFileExtension(),
                                            File_Type = FileType.TrademarkLogo,
                                            Entity = FileEntity.Manufacturer,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.TradeMarkLogo.ContentLength.ToString(),
                                            Reference_ID = model.ManufacturerCode,
                                            Manufacturer_Code = DRRGStandardValues.Registration
                                        });
                                        context.SaveChanges();
                                    }
                                }
                                #endregion

                                #region [Update Trade License Document]
                                if (model.TradeLicenseDocument != null && model.TradeLicenseDocument.ContentLength > 0)
                                {
                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == model.ManufacturerCode.ToLower() && x.File_Type == FileType.TradeLicenseDoc).ToList();
                                    foreach (var item in rRG_Files)
                                    {
                                        context.DRRG_Files.Remove(item);
                                        context.SaveChanges();
                                    }
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.TradeLicenseDocument.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.TradeLicenseDocument.FileName,
                                            ContentType = model.TradeLicenseDocument.ContentType,
                                            Extension = model.TradeLicenseDocument.GetTrimmedFileExtension(),
                                            File_Type = FileType.TradeLicenseDoc,
                                            Entity = FileEntity.Manufacturer,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.TradeLicenseDocument.ContentLength.ToString(),
                                            Reference_ID = model.ManufacturerCode,
                                            Manufacturer_Code = DRRGStandardValues.Registration
                                        });
                                        context.SaveChanges();
                                    }

                                }
                                #endregion

                                #region [Update Supporting Documents]
                                if (model.SupportingDocument != null && model.SupportingDocument.Count > 0)
                                {
                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == model.ManufacturerCode.ToLower() && x.File_Type == FileType.SupportingDocument).ToList();
                                    foreach (var item in rRG_Files)
                                    {
                                        context.DRRG_Files.Remove(item);
                                        context.SaveChanges();
                                    }
                                    foreach (HttpPostedFileBase supportingdocument in model.SupportingDocument)
                                    {
                                        if (supportingdocument != null)
                                        {
                                            using (MemoryStream memoryStream_8 = new MemoryStream())
                                            {
                                                supportingdocument.InputStream.CopyTo(memoryStream_8);
                                                context.DRRG_Files.Add(new DRRG_Files
                                                {
                                                    Name = supportingdocument.FileName,
                                                    ContentType = supportingdocument.ContentType,
                                                    Extension = supportingdocument.GetTrimmedFileExtension(),
                                                    File_Type = FileType.SupportingDocument,
                                                    Entity = FileEntity.Manufacturer,
                                                    Content = memoryStream_8.ToArray() ?? new byte[0],
                                                    Size = supportingdocument.ContentLength.ToString(),
                                                    Reference_ID = model.ManufacturerCode,
                                                    Manufacturer_Code = DRRGStandardValues.Registration
                                                });
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region [Update Factory]
                                if (model.factory != null && model.factory.Length > 0)
                                {
                                    model.factory.ToList().ForEach(x => { if (string.IsNullOrWhiteSpace(x.FactoryCode)) { x.FactoryCode = FactoryCode(); } });
                                    foreach (Factory factory in model.factory)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrWhiteSpace(factory.FactoryFullName))
                                            {
                                                DRRG_Factory_Details rRG_Factory_Details = context.DRRG_Factory_Details.Where(x => x.Factory_Code.ToLower().Equals(factory.FactoryCode.ToLower())).FirstOrDefault();
                                                if (rRG_Factory_Details != null)
                                                {
                                                    rRG_Factory_Details.Address = factory.FactoryAddress;
                                                    rRG_Factory_Details.Country = factory.FactoryCountry;
                                                    rRG_Factory_Details.EOL_PV_Module = factory.EOLPVModule;
                                                    rRG_Factory_Details.Factory_Name = factory.FactoryFullName;
                                                    rRG_Factory_Details.Factory_Code = factory.FactoryCode;
                                                    rRG_Factory_Details.Manufacturer_Code = model.ManufacturerCode;
                                                    context.SaveChanges();
                                                }
                                                else
                                                {
                                                    context.DRRG_Factory_Details.Add(new DRRG_Factory_Details
                                                    {
                                                        Address = factory.FactoryAddress,
                                                        Country = factory.FactoryCountry,
                                                        EOL_PV_Module = factory.EOLPVModule,
                                                        Factory_Name = factory.FactoryFullName,
                                                        Factory_Code = factory.FactoryCode,
                                                        Manufacturer_Code = model.ManufacturerCode,
                                                        Manufacturer_ID = model.ManufacturerId
                                                    });
                                                    context.SaveChanges();
                                                }
                                                if (factory.EnvironmentalFile != null && factory.EnvironmentalFile.ContentLength > 0)
                                                {
                                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == factory.FactoryCode.ToLower() && x.File_Type == FileType.EnvironmentalManagementSupport).ToList();
                                                    foreach (var item in rRG_Files)
                                                    {
                                                        context.DRRG_Files.Remove(item);
                                                        context.SaveChanges();
                                                    }
                                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                                    {
                                                        factory.EnvironmentalFile.InputStream.CopyTo(memoryStream_8);
                                                        context.DRRG_Files.Add(new DRRG_Files
                                                        {
                                                            Name = factory.EnvironmentalFile.FileName,
                                                            ContentType = factory.EnvironmentalFile.ContentType,
                                                            Extension = factory.EnvironmentalFile.GetTrimmedFileExtension(),
                                                            File_Type = FileType.EnvironmentalManagementSupport,
                                                            Entity = FileEntity.Factory,
                                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                                            Size = factory.EnvironmentalFile.ContentLength.ToString(),
                                                            Reference_ID = factory.FactoryCode,
                                                            Manufacturer_Code = model.ManufacturerCode
                                                        });
                                                        context.SaveChanges();
                                                    }
                                                }
                                                if (factory.EOLFile != null && factory.EOLFile.ContentLength > 0)
                                                {
                                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == factory.FactoryCode.ToLower() && x.File_Type == FileType.EOLPVModule).ToList();
                                                    foreach (var item in rRG_Files)
                                                    {
                                                        context.DRRG_Files.Remove(item);
                                                        context.SaveChanges();
                                                    }
                                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                                    {
                                                        factory.EOLFile.InputStream.CopyTo(memoryStream_8);
                                                        context.DRRG_Files.Add(new DRRG_Files
                                                        {
                                                            Name = factory.EOLFile.FileName,
                                                            ContentType = factory.EOLFile.ContentType,
                                                            Extension = factory.EOLFile.GetTrimmedFileExtension(),
                                                            File_Type = FileType.EOLPVModule,
                                                            Entity = FileEntity.Factory,
                                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                                            Size = factory.EOLFile.ContentLength.ToString(),
                                                            Reference_ID = factory.FactoryCode,
                                                            Manufacturer_Code = model.ManufacturerCode
                                                        });
                                                        context.SaveChanges();
                                                    }
                                                }
                                                if (factory.QMSFile != null && factory.QMSFile.ContentLength > 0)
                                                {
                                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == factory.FactoryCode.ToLower() && x.File_Type == FileType.QualityManagementSupport).ToList();
                                                    foreach (var item in rRG_Files)
                                                    {
                                                        context.DRRG_Files.Remove(item);
                                                        context.SaveChanges();
                                                    }
                                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                                    {
                                                        factory.QMSFile.InputStream.CopyTo(memoryStream_8);
                                                        context.DRRG_Files.Add(new DRRG_Files
                                                        {
                                                            Name = factory.QMSFile.FileName,
                                                            ContentType = factory.QMSFile.ContentType,
                                                            Extension = factory.QMSFile.GetTrimmedFileExtension(),
                                                            File_Type = FileType.QualityManagementSupport,
                                                            Entity = FileEntity.Factory,
                                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                                            Size = factory.QMSFile.ContentLength.ToString(),
                                                            Reference_ID = factory.FactoryCode,
                                                            Manufacturer_Code = model.ManufacturerCode
                                                        });
                                                        context.SaveChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                DRRG_Factory_Details rRG_Factory_Details = context.DRRG_Factory_Details.Where(x => x.Factory_Code.ToLower().Equals(factory.FactoryCode.ToLower())).FirstOrDefault();
                                                if (rRG_Factory_Details != null)
                                                {
                                                    context.DRRG_Factory_Details.Remove(rRG_Factory_Details);
                                                    context.SaveChanges();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LogService.Error(ex, this);
                                        }
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, Translate.Text("Add Factory"));
                                }
                                #endregion

                                #region [Update Manufacturer Details]
                                DRRG_Manufacturer_Details dRRG_Manufacturer_Details = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code.ToLower().Equals(model.ManufacturerCode)).FirstOrDefault();
                                if (dRRG_Manufacturer_Details != null)
                                {
                                    dRRG_Manufacturer_Details.Brand_Name = model.BrandName;
                                    dRRG_Manufacturer_Details.Manufacturer_Name = model.ManufacturerFullName;
                                    dRRG_Manufacturer_Details.Manufacturer_Country = model.Manufacturercountry;
                                    dRRG_Manufacturer_Details.Corporate_Email = model.ManufacturerEmailAddress;
                                    dRRG_Manufacturer_Details.Corporate_Phone_Number = model.ManufacturerPhoneNumber;
                                    dRRG_Manufacturer_Details.Corporate_Phone_Code = model.ManufacturerPhonecode;
                                    dRRG_Manufacturer_Details.Corporate_Fax_Number = model.ManufacturerFaxNumber;
                                    dRRG_Manufacturer_Details.Corporate_Fax_Code = model.ManufacturerFaxcode;
                                    dRRG_Manufacturer_Details.Local_Representative = model.UserLocalRepresentative;
                                    dRRG_Manufacturer_Details.User_Designation = model.UserDesignation;
                                    dRRG_Manufacturer_Details.User_Email_Address = dRRG_Manufacturer_Details.User_Email_Address;
                                    dRRG_Manufacturer_Details.User_First_Name = model.Userfirstname;
                                    dRRG_Manufacturer_Details.User_Last_Name = model.UserLastName;
                                    dRRG_Manufacturer_Details.User_Gender = model.UserGender ? UserGender.Male : UserGender.Female;
                                    dRRG_Manufacturer_Details.User_Mobile_Number = model.UserPhoneNumber;
                                    dRRG_Manufacturer_Details.User_Mobile_Code = model.UserPhonecode;
                                    dRRG_Manufacturer_Details.User_Nationality = model.UserCountry;
                                    dRRG_Manufacturer_Details.Company_Full_Name = model.Userrepresentativename;
                                    dRRG_Manufacturer_Details.Website = model.Website;
                                    dRRG_Manufacturer_Details.Status = "Submitted";
                                    dRRG_Manufacturer_Details.UpdatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture);
                                    dRRG_Manufacturer_Details.Manufacturer_Code = model.ManufacturerCode;
                                    context.SaveChanges();

                                }
                                #endregion

                                #region [Update Login Details]
                                DRRG_UserLogin dRRG_UserLogin = context.DRRG_UserLogin.Where(x => x.Login_username.ToLower().Equals(CurrentPrincipal.Username.ToString())).FirstOrDefault();
                                if (dRRG_UserLogin != null)
                                {
                                    dRRG_UserLogin.Status = "Updated";
                                    dRRG_UserLogin.Name = model.Userfirstname + " " + model.UserLastName;
                                    dRRG_UserLogin.UpdatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture);
                                    context.SaveChanges();
                                }
                                #endregion

                                transaction.Commit();

                                ViewBag.Email = model.UserEmail;
                                ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                                context.SP_DRRG_Logout(CurrentPrincipal.Username, CurrentPrincipal.SessionToken, myOutputParamresponse);
                                string myString = Convert.ToString(myOutputParamresponse.Value);
                                ClearCookiesSignOut();
                                SendDRRGRegistrationEmail(model.Userfirstname, model.UserEmail, model.UserLocalRepresentative, string.Empty, DRRGStandardValues.UpdateProfile, model.ManufacturerFullName, model.ManufacturerCode);
                                ViewBag.SuccessText = Translate.Text("DRRG.rejected profile description");
                                return View("~/Views/Feature/DRRG/Dashboard/UpdateProfileSuccess.cshtml");
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            model.CountryMobileList = GetCountryMobilelist();
            model.NationalityList = GetCountrylist();
            return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            using (DRRGEntities context = new DRRGEntities())
            {
                var applicationSession = context.DRRG_ApplicationSession.Where(x => x.SessionId == CurrentPrincipal.SessionToken && x.UserId == CurrentPrincipal.UserId).ToList();
                if (applicationSession != null)
                {
                    foreach (var item in applicationSession)
                    {
                        context.DRRG_ApplicationSession.Remove(item);
                        context.SaveChanges();
                    }
                }
                ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                context.SP_DRRG_Logout(CurrentPrincipal.Username, CurrentPrincipal.SessionToken, myOutputParamresponse);
                string myString = Convert.ToString(myOutputParamresponse.Value);
                ClearCookiesSignOut();
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_HOME);
        }
        public void LogoutUser(LoginModel loginmodel)
        {
            ClearCookiesSignOut();
        }
        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult DRRGAuthentication()
        {
            return new EmptyResult();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CheckManufacturerValidation(string fieldtype, string manufValue, string brandValue, string manufCode = "")
        {
            string responseMessage = string.Empty;
            string responseCode = string.Empty;
            if (!string.IsNullOrEmpty(fieldtype) && (!string.IsNullOrEmpty(manufValue) || !string.IsNullOrEmpty(brandValue)))
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    DRRG_Manufacturer_Details manufacturerDetail = null;
                    string oldManufactturerName = string.Empty;
                    string oldBrandName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(manufCode))
                    {
                        var existManfcturerDetail = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code == manufCode).FirstOrDefault();
                        oldManufactturerName = existManfcturerDetail.Manufacturer_Name;
                        oldBrandName = existManfcturerDetail.Brand_Name;
                    }
                    responseCode = "000";
                    responseMessage = "Success";

                    if (fieldtype == "M" && !string.IsNullOrWhiteSpace(manufValue) && string.IsNullOrWhiteSpace(manufCode))
                    {
                        manufacturerDetail = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Name.ToLower().Equals(manufValue.ToLower()) &&
                        (x.Status.Equals("Approved") ||
                         x.Status.Equals("Submitted") ||
                         x.Status.Equals("Updated") ||
                         x.Status.Equals("AuthorizedLetterUpdated") ||
                         x.Status.Equals("AuthorizedLetterSubmitted"))).FirstOrDefault();
                    }
                    else if (fieldtype == "M" && !string.IsNullOrWhiteSpace(manufCode) && !string.IsNullOrEmpty(manufValue))
                    {
                        if (manufValue != oldManufactturerName)
                        {
                            manufacturerDetail = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Name.ToLower().Equals(manufValue.ToLower()) &&
                                                    (x.Status.Equals("Approved") ||
                                                     x.Status.Equals("Submitted") ||
                                                     x.Status.Equals("Updated") ||
                                                     x.Status.Equals("AuthorizedLetterUpdated") ||
                                                     x.Status.Equals("AuthorizedLetterSubmitted"))).FirstOrDefault();
                        }
                    }

                    if (fieldtype == "B" && !string.IsNullOrWhiteSpace(brandValue) && string.IsNullOrWhiteSpace(manufCode))
                    {
                        manufacturerDetail = context.DRRG_Manufacturer_Details.Where(x => x.Brand_Name.ToLower().Equals(brandValue.ToLower()) &&
                                            (x.Status.Equals("Approved") ||
                                                x.Status.Equals("Submitted") ||
                                                x.Status.Equals("Updated") ||
                                                x.Status.Equals("AuthorizedLetterUpdated") ||
                                                x.Status.Equals("AuthorizedLetterSubmitted"))).FirstOrDefault();
                    }
                    else if (fieldtype == "B" && !string.IsNullOrWhiteSpace(manufCode) && !string.IsNullOrEmpty(brandValue))
                    {
                        if (brandValue != oldBrandName)
                        {
                            manufacturerDetail = context.DRRG_Manufacturer_Details.Where(x => x.Brand_Name.ToLower().Equals(brandValue.ToLower()) &&
                                                (x.Status.Equals("Approved") ||
                                                    x.Status.Equals("Submitted") ||
                                                    x.Status.Equals("Updated") ||
                                                    x.Status.Equals("AuthorizedLetterUpdated") ||
                                                    x.Status.Equals("AuthorizedLetterSubmitted"))).FirstOrDefault();
                        }
                    }

                    if (fieldtype == "M" && manufacturerDetail != null && !string.IsNullOrWhiteSpace(manufacturerDetail.Manufacturer_Name))
                    {
                        responseMessage = Translate.Text("DRRG.ExistManufacturername");
                        responseCode = "399";
                        return Json(new { Message = responseMessage, errorCode = responseCode });
                    }
                    if (fieldtype == "B" && manufacturerDetail != null && !string.IsNullOrWhiteSpace(manufacturerDetail.Brand_Name))
                    {
                        responseMessage = Translate.Text("DRRG.ExistBrandname");
                        responseCode = "399";
                        return Json(new { Message = responseMessage, errorCode = responseCode });
                    }
                }
            }
            else
            {
                responseCode = "399";
                responseMessage = Translate.Text("DRRG_FieldIsRequired");
            }
            return Json(new { Message = responseMessage, errorCode = responseCode });
        }
        public ActionResult EligibleEquipmentList()
        {
            List<DRRG_Manufacturer_Details> lst = null;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    lst = context.DRRG_Manufacturer_Details.Where(x => x.Status.ToLower().Equals("approved")).OrderBy(x => x.Manufacturer_Name).ToList();

                    ViewBag.CurrentRole = CurrentPrincipal.Role;
                    CacheProvider.Store(CacheKeys.DRRG__ELIGIBLEEQUIPMENT_LIST, new CacheItem<List<DRRG_Manufacturer_Details>>(lst, TimeSpan.FromMinutes(40)));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/EligibleEquipmentList.cshtml", lst);
        }
        public ActionResult GetManufacturers(string manuCode, string type)
        {
            DRRGModuleList model = new DRRGModuleList();
            model.isAdmin = false;
            using (DRRGEntities context = new DRRGEntities())
            {
                if (type.ToLower().Trim() == "pv")
                {
                    int index = 0;
                    var pv = context.DRRG_PVMODULE.Where(x => x.Status.ToLower().Equals("approved") && x.Manufacturer_Code.ToLower().Equals(manuCode.ToLower())).ToList();
                    pv.Where(y => y != null && y.Status.ToLower().Equals("approved")).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        pvId = x.PV_ID,
                        modelName = x.Model_Name,
                        nominalpower = GetNominalPowerDetails(x),
                        celltechnology = x.Cell_Technology,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Cell_Technology,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Nominal_Power,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method,
                        extraCompliance = getExtraCompliance(x.PV_ID, "pv", context)
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_PvModuleListing.cshtml", model);
                }
                else if (type.ToLower().Trim() == "inv" || type.ToLower().Trim() == "iv")
                {
                    int index = 0;
                    var iv = context.DRRG_InverterModule.Where(x => x.Status.ToLower().Equals("approved") && x.Manufacturer_Code.ToLower().Equals(manuCode.ToLower())).ToList();
                    iv.Where(y => y != null).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        inverterId = x.Inverter_ID,
                        modelName = x.Model_Name,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Model_Name,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = Convert.ToString(x.Protection_Degree),
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method,
                        ratedpower = GetRatedPower(x),
                        acparentpower = GetACParentPower(x),
                        usageCategory = x.Function_String,

                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0,
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_InverterModuleListing.cshtml", model);
                }
                else if (type.ToLower().Trim() == "ip")
                {
                    int index = 0;
                    var ip = context.DRRG_InterfaceModule.Where(x => x.Status.ToLower().Equals("approved") && x.Manufacturer_Code.ToLower().Equals(manuCode.ToLower())).ToList();
                    ip.Where(y => y != null && y.Status.ToLower().Equals("approved")).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        interfaceId = x.Interface_ID,
                        modelName = x.Model_Name,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Model_Name,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Compliance,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method,
                        extraCompliance = getExtraCompliance(x.Interface_ID, "ip", context)
                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_InterfaceModule.cshtml", model);
                }
                else
                {
                    int index = 0;
                    var pv = context.SP_DRRG_GETPVModuleByManuId(manuCode.Trim()).ToList();
                    pv.Where(y => y != null && y.Status.ToLower().Equals("approved")).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        modelName = x.PV_ID,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Cell_Technology,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Nominal_Power,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method
                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_PvModuleListing.cshtml", model);
                }
            }

        }
        [HttpGet]
        public ActionResult mdFile(string id, string manu)
        {
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var pvresultfiles = context.SP_DRRG_GETFilesbyID(id, manu).ToList();
                    if (pvresultfiles != null && pvresultfiles.Count > 0)
                    {
                        byte[] bytes = pvresultfiles.Where(x => x.File_Type.Equals(FileType.ModelDataSheet)).OrderByDescending(x => x.File_ID).FirstOrDefault().Content;
                        string type = pvresultfiles.FirstOrDefault().ContentType;
                        return File(bytes, type);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return null;
        }
    }
}
