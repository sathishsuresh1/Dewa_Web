using DEWAXP.Feature.HR.Extensions;
using DEWAXP.Feature.HR.Models.Scholarship;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.ScholarshipService;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CNST = DEWAXP.Feature.HR.Models.Scholarship.Constants.AttachmentType;

namespace DEWAXP.Feature.HR.Controllers
{
    public class ScholarshipController : BaseController
    {
        #region Local Constants

        public const string LOGIN_VIEW = "~/Views/Feature/HR/Scholarship/ScholarshipLogin.cshtml";
        public const string PROFILE_UPDATE_VIEW = "~/Views/Feature/HR/Scholarship/ProfileUpdate.cshtml";

        #endregion Local Constants

        private scholarshipHelpValues _helpValues;

        #region [Actions]

        #region Login

        public ActionResult Login()
        {
            if (IsUserLoggedIn()) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PERSONAL_INFORMATION_PAGE); }
            return View(LOGIN_VIEW, new Login());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                string responseMessage = string.Empty;
                //DEWAXP.Foundation.Integration.ScholarshipService.LoginScholarship
                if (TryLogin(model, out responseMessage))
                {
                    if (model.InitialLogin)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SET_NEW_PASSWORD, QueryString.Parse(string.Format("?userid={0}", model.UserName)));
                    }

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PREAPPLY_PAGE);
                }
                model.IsValidationError = true;

                ModelState.AddModelError(string.Empty, responseMessage);

                return PartialView(LOGIN_VIEW, model);
            }

            return View(LOGIN_VIEW, model);
        }

        #endregion Login

        #region ForgotPassword

        public ActionResult ForgotPassword()
        {
            if (IsUserLoggedIn()) { RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PERSONAL_INFORMATION_PAGE); }
            return View("~/Views/Feature/HR/Scholarship/ForgotPassword.cshtml", new ForgotPassword());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = ScholarshipServiceClient.ForgotPassword(model.UserName, model.EmailAddress, RequestLanguage, Request.Segment());
                    //if (response.Succeeded)
                    //{
                    //}
                    //ModelState.AddModelError(string.Empty, response.Message);
                    var recoveryEmailModel = new RecoveryEmail
                    {
                        EmailAddress = model.EmailAddress,
                        Context = RecoveryContext.Password
                    };

                    //CacheProvider.Store(CacheKeys.SCHOLARSHIP_PASSWORD_RECOVERY_STATE, new CacheItem<RecoveryEmail>(recoveryEmailModel));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_RECOVERY_EMAIL_SENT);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            return View("~/Views/Feature/HR/Scholarship/ForgotPassword.cshtml",model);
        }

        [HttpGet]
        public ActionResult SetNewPassword(string userid)
        {
            return View("~/Views/Feature/HR/Scholarship/SetNewPassword.cshtml",new SetNewPasswordModel() { Username = userid });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(SetNewPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Password != model.ConfirmPassword) { ModelState.AddModelError("", "Scholarship Password Not Match Error"); return View("~/Views/Feature/HR/Scholarship/SetNewPassword.cshtml", model); }

                    var response = ScholarshipServiceClient.SetNewPassword(model.Username, model.CurrentPassword, model.Password, "",
                        RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        model.PasswordResetSuccessful = true;
                    }
                    else
                    {
                    }
                    ModelState.AddModelError("", response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }
            return View("~/Views/Feature/HR/Scholarship/SetNewPassword.cshtml",model);
        }

        private void ClearSessionAndSignOut()
        {
            if (!string.IsNullOrEmpty(CurrentPrincipal.UserId))
            {
                DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
                //Clear any Scholarship related cache/session
            }
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
        }

        #endregion ForgotPassword

        #region ForgotUsername

        public ActionResult ForgotUsername()
        {
            if (IsUserLoggedIn()) { RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PERSONAL_INFORMATION_PAGE); }
            return View("~/Views/Feature/HR/Scholarship/ForgotUsername.cshtml",new ForgotUsername());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotUsername(ForgotUsername model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = ScholarshipServiceClient.RequestUsername(model.Email, RequestLanguage, Request.Segment());
                    var recoveryEmailModel = new RecoveryEmail
                    {
                        EmailAddress = model.Email,
                        Context = RecoveryContext.Username
                    };

                    //CacheProvider.Store(CacheKeys.SCHOLARSHIP_PASSWORD_RECOVERY_STATE, new CacheItem<RecoveryEmail>(recoveryEmailModel));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_RECOVERY_EMAIL_SENT);
                    //if (response.Succeeded)
                    //{
                    //}
                    //ModelState.AddModelError(string.Empty, Translate.Text("Scholarship try again"));
                }
                catch (System.Exception ex)
                {
                    //Add log to Sitecore log
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            return View("~/Views/Feature/HR/Scholarship/ForgotUsername.cshtml",model);
        }

        #endregion ForgotUsername

        #region Register

        public ActionResult Register()
        {
            if (IsUserLoggedIn()) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PERSONAL_INFORMATION_PAGE); }

            var model = new Registration();

            model.Programs = GetHelpValues(HelpValueType.Programs);

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            return View("~/Views/Feature/HR/Scholarship/Register.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(Registration model)
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

            if (IsUserLoggedIn()) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PERSONAL_INFORMATION_PAGE); }

            model.Programs = GetHelpValues(HelpValueType.Programs);

            if (ModelState.IsValid && status)
            {
                var saveResponse = ScholarshipServiceClient.NewRegistration(model.Program, model.FirstName, model.MiddleName, model.LastName,
                       model.MobileNumber, model.UserName, model.Password, model.Password1, model.Email, model.Email1,
                       model.EmiratesID, model.HaveScored80Plus == "1" ? true : false, RequestLanguage, Request.Segment());

                if (saveResponse.Succeeded)
                {
                    //RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_DASHBOARD);
                    return View("~/Views/Feature/HR/Scholarship/RegistrationSuccess.cshtml");
                }
                else
                {
                    ModelState.AddModelError("", saveResponse.Message);
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

            return View("~/Views/Feature/HR/Scholarship/Register.cshtml",model);
        }

        #endregion Register

        [HttpGet]
        public ActionResult PersonalInformation()
        {
            ScholarshipPreApplyPageModel _preApplyDetal = null;
            bool preApplyDetailOk = CacheProvider.TryGet("ScholarshipPreApplyConfirm", out _preApplyDetal) && _preApplyDetal.IsGPAOk && _preApplyDetal.IsUniversityYearOk;
            if (!preApplyDetailOk)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PREAPPLY_PAGE);
            }

            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel model = new ProfileUpdateModel(); //GetProfileUpdateStage();

            //if (model.Stage != UpdateStage.PersonalInformation)
            //{
            var res = ScholarshipServiceClient.SignIn(user.UserName, user.UserPassword, RequestLanguage, Request.Segment(), user.WebServiceCredentials);
            if (res.Succeeded)
            {
                model = res.Payload.@return.candidateDetails.MapToModel(UpdateStage.PersonalInformation);
                SaveProfileUpdateStage(model);
            }
            else
            {
                LogService.Error(new System.Exception(res.Message), this);
                model.Stage = UpdateStage.Invalid;
                goto JumpHere;
            }
            //}

            model.PersonalInformation.Nationalities = GetHelpValues(HelpValueType.Countries);
            model.PersonalInformation.ApplicationSources = GetHelpValues(HelpValueType.ApplicationSources);

        JumpHere:
            return View(PROFILE_UPDATE_VIEW, model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalInformation(PersonalDetail model)
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel existingModel = GetProfileUpdateStage();

            if (ModelState.IsValid & ValidatePersonalInformation(model, existingModel.PersonalInformation))
            {
                try
                {
                    candidateDetails details = new candidateDetails();
                    details.studentFormData = model.MapToModel();
                    details.attachmentData = GetAttachmentsForPersonalInformation(model);

                    //details.attachmentData
                    var res = ScholarshipServiceClient.UpdateCandidateDetails(details, existingModel.StepNumber,
                        user.WebServiceCredentials, user.UserName, RequestLanguage, Request.Segment());

                    if (res.Succeeded)
                    {
                        ProfileUpdateModel newStateModel = res.Payload.MapToModel(UpdateStage.ContactDetails);

                        /* existingModel.Stage = UpdateStage.ContactDetails;
                         existingModel.PersonalInformation = res.Payload.studentFormDat;

                        existingModel.PersonalInformation.EmirateIDUpload = null;
                         existingModel.PersonalInformation.Photo = null;
                         existingModel.PersonalInformation.PassportUpload = null;
                         existingModel.PersonalInformation.FamilyBookUpload = null;*/

                        SaveProfileUpdateStage(newStateModel);
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_CONTACT_DETAIL_PAGE);
                    }
                    else
                    {
                        ModelState.AddModelError("", res.Message);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            model.ExistingPhoto = existingModel.PersonalInformation.ExistingPhoto;
            model.ExistingPassport = existingModel.PersonalInformation.ExistingPassport;
            model.ExistingFamilyBook = existingModel.PersonalInformation.ExistingFamilyBook;
            model.ExistingEmiratesID = existingModel.PersonalInformation.ExistingEmiratesID;

            existingModel.PersonalInformation = model;

            existingModel.PersonalInformation.Nationalities = GetHelpValues(HelpValueType.Countries);
            existingModel.PersonalInformation.ApplicationSources = GetHelpValues(HelpValueType.ApplicationSources);

            return View(PROFILE_UPDATE_VIEW, existingModel);
        }

        public ActionResult ContactDetail()
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel model = new ProfileUpdateModel();

            var res = ScholarshipServiceClient.SignIn(user.UserName, user.UserPassword, RequestLanguage, Request.Segment(), user.WebServiceCredentials);
            if (res.Succeeded)
            {
                model = res.Payload.@return.candidateDetails.MapToModel(UpdateStage.ContactDetails);
                SaveProfileUpdateStage(model);
            }
            else
            {
                LogService.Error(new System.Exception(res.Message), this);
                model.Stage = UpdateStage.Invalid;
                goto JumpHere;
            }

            model.ContactInformation.Emirates = GetHelpValues(HelpValueType.Emirates, "AE");
            model.ContactInformation.Areas = GetHelpValues(HelpValueType.Areas, model.ContactInformation.Emirate);

        JumpHere:
            return View(PROFILE_UPDATE_VIEW, model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactDetail(ContactDetail model)
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel existingModel = GetProfileUpdateStage();

            if (ModelState.IsValid)
            {
                try
                {
                    candidateDetails details = new candidateDetails();
                    details.studentFormData = model.MapToModel();

                    var res = ScholarshipServiceClient.UpdateCandidateDetails(details, existingModel.StepNumber, user.WebServiceCredentials, user.UserName, RequestLanguage, Request.Segment());

                    if (res.Succeeded)
                    {
                        ProfileUpdateModel newStateModel = res.Payload.MapToModel(UpdateStage.AcademicInformation);
                        //existingModel.Stage = UpdateStage.AcademicInformation;
                        //existingModel.ContactInformation = model;
                        SaveProfileUpdateStage(newStateModel);

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_ACADEMIC_INFORMATION_PAGE);
                    }
                    else
                    {
                        ModelState.AddModelError("", res.Message);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            existingModel.ContactInformation = model;

            existingModel.ContactInformation.Emirates = GetHelpValues(HelpValueType.Emirates, "AE");
            existingModel.ContactInformation.Areas = GetHelpValues(HelpValueType.Areas, model.Emirate);

            return View(PROFILE_UPDATE_VIEW, existingModel);
        }

        public ActionResult AcademicInformation()
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel model = new ProfileUpdateModel();

            var res = ScholarshipServiceClient.SignIn(user.UserName, user.UserPassword, RequestLanguage, Request.Segment(), user.WebServiceCredentials);
            if (res.Succeeded)
            {
                model = res.Payload.@return.candidateDetails.MapToModel(UpdateStage.AcademicInformation);
                model.AcademicInformation.Programs = GetHelpValues(HelpValueType.Programs);
                model.AcademicInformation.Program = model.CandidateProgram;
                SaveProfileUpdateStage(model);
            }
            else
            {
                LogService.Error(new System.Exception(res.Message), this);
                model.Stage = UpdateStage.Invalid;
                ModelState.AddModelError("", res.Message);
                goto JumpHere;
            }

            model.AcademicInformation.Countries = GetHelpValues(HelpValueType.Countries);
            model.AcademicInformation.Grades = GetHelpValues(HelpValueType.Grades);
            model.AcademicInformation.Levels = GetHelpValues(HelpValueType.Levels);
            model.AcademicInformation.Majors = new List<SelectListItem>();
            model.AcademicInformation.Majors1 = GetHelpValues(HelpValueType.Majors);
            model.AcademicInformation.Programs = GetHelpValues(HelpValueType.Programs);

            if (!string.IsNullOrEmpty(model.AcademicInformation.University))
            {
                model.AcademicInformation.Universities = GetHelpValues(HelpValueType.Universities, model.AcademicInformation.Program);
                model.AcademicInformation.Majors = GetHelpValues(HelpValueType.Majors, model.AcademicInformation.University);
            }

        JumpHere:

            return View(PROFILE_UPDATE_VIEW, model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AcademicInformation(AcademicDetail model)
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel existingModel = GetProfileUpdateStage();

            if (ModelState.IsValid && ValidateAcademicDetails(model, existingModel.AcademicInformation) == true)
            {
                try
                {
                    candidateDetails details = new candidateDetails();
                    details.studentFormData = model.MapToModel();

                    var grades = GetHelpValues(HelpValueType.Grades);
                    foreach (var i in model.Eduction)
                    {
                        var grd = grades.Where(x => x.Text.Equals(i.Level)).FirstOrDefault();

                        if (grd == null) { ModelState.AddModelError("", Translate.Text("Scholarship Invalid Grade Level Provided")); goto JumpHere; }

                        i.Level = grd.Value;
                    }

                    details.educationDataList = model.MapToEducationData();

                    bool isEduListValid = details.educationDataList.Count() >= 3 ?
                        (details.educationDataList.Any(i => i.level.Equals("12"))
                        && details.educationDataList.Any(i => i.level.Equals("13"))
                        && details.educationDataList.Any(i => i.level.Equals("14"))) : false;

                    if (!isEduListValid)
                    {
                        ModelState.AddModelError("", Translate.Text("Scholarship Please Provide Education History"));
                        goto JumpHere;
                    }

                    details.attachmentData = GetAttachmentsForAcademicInformation(model);

                    var res = ScholarshipServiceClient.UpdateCandidateDetails(details, existingModel.StepNumber,
                        user.WebServiceCredentials, user.UserName, RequestLanguage, Request.Segment());

                    if (res.Succeeded)
                    {
                        ProfileUpdateModel newStateModel = res.Payload.MapToModel(UpdateStage.Questionair);
                        SaveProfileUpdateStage(newStateModel);

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_QUESTIONNAIRE_PAGE);
                    }
                    else
                    {
                        ModelState.AddModelError("", res.Message);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

        JumpHere:

            model.ExistingAcademicCertificate = existingModel.AcademicInformation.ExistingAcademicCertificate;
            model.ExistingCertificateOfGoodConduct = existingModel.AcademicInformation.ExistingCertificateOfGoodConduct;
            model.ExistingGrade12FinalCertificate = existingModel.AcademicInformation.ExistingGrade12FinalCertificate;
            model.ExistingPODCard = existingModel.AcademicInformation.ExistingPODCard;
            model.ExistingPoliceCertificate = existingModel.AcademicInformation.ExistingPoliceCertificate;
            model.ExistingUniversityTranscript = existingModel.AcademicInformation.ExistingUniversityTranscript;

            model.Eduction = string.IsNullOrEmpty(model.EducationJson) ? new List<Eduction>() : CustomJsonConvertor.DeserializeObject<List<Eduction>>(model.EducationJson);
            existingModel.AcademicInformation = model;
            existingModel.AcademicInformation.Programs = GetHelpValues(HelpValueType.Programs);
            existingModel.AcademicInformation.Universities = GetHelpValues(HelpValueType.Universities, model.Program);
            existingModel.AcademicInformation.Countries = GetHelpValues(HelpValueType.Countries);
            existingModel.AcademicInformation.Grades = GetHelpValues(HelpValueType.Grades);
            //existingModel.AcademicInformation.Levels = GetHelpValues(HelpValueType.Levels);
            existingModel.AcademicInformation.Majors = GetHelpValues(HelpValueType.Majors);
            existingModel.AcademicInformation.Programs = GetHelpValues(HelpValueType.Programs);
            existingModel.AcademicInformation.Majors1 = GetHelpValues(HelpValueType.Majors);
            if (!string.IsNullOrEmpty(model.University))
            {
                existingModel.AcademicInformation.Universities = GetHelpValues(HelpValueType.Universities, model.Program);
                existingModel.AcademicInformation.Majors = GetHelpValues(HelpValueType.Majors, model.University);
            }

            existingModel.Stage = UpdateStage.AcademicInformation;

            return View(PROFILE_UPDATE_VIEW, existingModel);
        }

        public ActionResult Questionnaire()
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel existingModel = GetProfileUpdateStage();

            ProfileUpdateModel model = new ProfileUpdateModel();

            if (existingModel.Stage == UpdateStage.Completed)
            {
                return View(PROFILE_UPDATE_VIEW, existingModel);
            }

            var res = ScholarshipServiceClient.SignIn(user.UserName, user.UserPassword, RequestLanguage, Request.Segment(), user.WebServiceCredentials);
            if (res.Succeeded)
            {
                model = res.Payload.@return.candidateDetails.MapToModel(UpdateStage.Questionair);
                SaveProfileUpdateStage(model);
            }
            else
            {
                LogService.Error(new System.Exception(res.Message), this);
                model.Stage = UpdateStage.Invalid;
                goto JumpHere;
            }

        //other code

        JumpHere:

            return View(PROFILE_UPDATE_VIEW, model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Questionnaire(Questionnaire model)
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }

            ProfileUpdateModel existingModel = GetProfileUpdateStage();

            if (ModelState.IsValid)
            {
                try
                {
                    //Validation, if candidate is Male, military certificate is mandatory

                    /*if (existingModel.CandidateGender.Equals(Translate.Text("Scholarship Gender Male")))
                    {
                        if (model.MilitaryServiceUpload == null && existingModel.Questionnaire.ExistingMilitaryServiceUpload == null)
                        {
                            ModelState.AddModelError("MilitaryServiceUpload", Translate.Text("Scholarship Field Required"));

                            goto JumpHere;
                        }
                    }*/

                    candidateDetails details = new candidateDetails();
                    details.studentFormData = model.MapToModel();

                    if (model.MilitaryServiceUpload != null && model.MilitaryServiceUpload.ContentLength > 100)
                    {
                        details.attachmentData = new studentAttachmentData[1];

                        byte[] item = new byte[model.MilitaryServiceUpload.ContentLength];
                        model.MilitaryServiceUpload.InputStream.Read(item, 0, model.MilitaryServiceUpload.ContentLength);
                        model.MilitaryServiceUpload.InputStream.Close();

                        details.attachmentData[0] = new studentAttachmentData()
                        {
                            data = item,
                            mimetype = CNST.AT_MILITARY__SERVICE_STATUS,
                            filename = Models.Scholarship.Questionnaire.MILITARY_STATUS_FILE_NAME + "." + model.MilitaryServiceUpload.GetTrimmedFileExtension()
                        };
                    }

                    var res = ScholarshipServiceClient.UpdateCandidateDetails(details, existingModel.StepNumber,
                        user.WebServiceCredentials, user.UserName, RequestLanguage, Request.Segment());

                    if (res.Succeeded)
                    {
                        existingModel.Stage = UpdateStage.Completed;
                        existingModel.Questionnaire = model;

                        existingModel.Questionnaire.MilitaryServiceUpload = null;

                        SaveProfileUpdateStage(existingModel);

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PROFILE_COMPLETED_PAGE);
                    }
                    else
                    {
                        ModelState.AddModelError("", res.Message);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

        JumpHere:

            model.ExistingMilitaryServiceUpload = existingModel.Questionnaire.ExistingMilitaryServiceUpload;
            existingModel.Questionnaire = model;
            existingModel.Stage = UpdateStage.Questionair;

            return View(PROFILE_UPDATE_VIEW, existingModel);
        }

        public ActionResult ScholarshipPreApplyPage()
        {
            Login user;
            if (!IsUserLoggedIn(out user)) { return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE); }
            CacheProvider.Remove("ScholarshipPreApplyConfirm");
            ScholarshipPreApplyPageModel model = new ScholarshipPreApplyPageModel();
            return View("~/Views/Feature/HR/Scholarship/ScholarshipPreApplyPage.cshtml",model);
        }

        [HttpPost]
        public ActionResult ScholarshipPreApplyPage(ScholarshipPreApplyPageModel model)
        {
            if (model.IsUniversityYearOk && model.IsGPAOk)
            {
                CacheProvider.Store("ScholarshipPreApplyConfirm", new AccessCountingCacheItem<ScholarshipPreApplyPageModel>(model, Times.Once));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_PERSONAL_INFORMATION_PAGE);
            }
            return View("~/Views/Feature/HR/Scholarship/ScholarshipPreApplyPage.cshtml",model);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            if (IsLoggedIn)
            {
                ClearSessionAndSignOut();
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SCHOLARSHIP_SIGNIN_PAGE);
        }

        [HttpGet]
        public ActionResult EmailVerification()
        {
            string param = Request.QueryString["t"];
            string resendFlag = "";
            EmailVerificationModel model = new EmailVerificationModel();
            if (param != null)
            {
                model.param = param;
                if (!string.IsNullOrWhiteSpace(Request.QueryString["rf"]))
                    resendFlag = "X";
                var r = DewaScholarshipRestClient.EmailVerification(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.DewaScholarship.EmailVerificationRequest()
                {
                    param = param,
                    resendflag = resendFlag,
                }, Request.Segment(), RequestLanguage);

                if (r.Succeeded && r.Payload != null)
                {
                    model.errorcode = r.Payload.errorcode;
                    model.errormessage = r.Payload.errormessage;
                    model.newverificationlinkenable = r.Payload.newverificationlinkenable;
                    model.success = r.Payload.success;
                    if (resendFlag == "X")
                    {
                        model.verificationstatus = "3";
                    }
                    else
                    {
                        model.verificationstatus = r.Payload.verificationstatus;
                    }

                    model.applicationvisibility = r.Payload.applicationvisibility;
                }
                else
                {
                    model.verificationstatus = "-1";
                    model.errormessage = r.Message;
                    ModelState.AddModelError(string.Empty, r.Message);
                }
            }
            else
            {
                model.verificationstatus = "-1";
                model.errormessage = Translate.Text("Invalid URL");
                ModelState.AddModelError(string.Empty, model.errormessage);
            }
            return PartialView("~/Views/Feature/HR/Scholarship/EmailVerification.cshtml", model);
        }

        #endregion [Actions]

        #region Validation

        private bool ValidatePersonalInformation(PersonalDetail model, PersonalDetail existingModel)
        {
            bool retVal = true;

            /*if (model.Photo == null && existingModel.ExistingPhoto == null)
            {
                ModelState.AddModelError("Photo", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }*/
            if (model.EmirateIDUpload == null && existingModel.ExistingEmiratesID == null)
            {
                ModelState.AddModelError("EmirateIDUpload", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }
            if (model.PassportUpload == null && existingModel.ExistingPassport == null)
            {
                ModelState.AddModelError("PassportUpload", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }
            if (model.FamilyBookUpload == null && existingModel.ExistingFamilyBook == null)
            {
                ModelState.AddModelError("FamilyBookUpload", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }
            return retVal;
        }

        private bool ValidateAcademicDetails(AcademicDetail model, AcademicDetail existingModel)
        {
            bool retVal = true;

            if (model.University.ToLower().Equals("10") && string.IsNullOrEmpty(model.OtherUniversity))
            {
                ModelState.AddModelError("OtherUniversity", Translate.Text(Models.Scholarship.Constants.DictionaryKeys.FIELD_REQUIRED)); retVal = false;
            }
            if (model.Major.ToLower().Equals("1018") && string.IsNullOrEmpty(model.OtherMajor))
            {
                ModelState.AddModelError("OtherMajor", Translate.Text(Models.Scholarship.Constants.DictionaryKeys.FIELD_REQUIRED)); retVal = false;
            }

            if (model.AcademicCertificates == null && existingModel.ExistingAcademicCertificate == null)
            {
                ModelState.AddModelError("AcademicCertificates", Translate.Text(Models.Scholarship.Constants.DictionaryKeys.FIELD_REQUIRED));
                retVal = false;
            }
            /*if (model.Grade12FinalCertificate == null && existingModel.ExistingGrade12FinalCertificate == null)
            {
                ModelState.AddModelError("Grade12FinalCertificate", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }
            if (model.CertificateOfGoodConduct == null && existingModel.ExistingCertificateOfGoodConduct == null)
            {
                ModelState.AddModelError("CertificateOfGoodConduct", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }*/
            if (model.PoliceCertificate == null && existingModel.ExistingPoliceCertificate == null)
            {
                ModelState.AddModelError("PoliceCertificate", Translate.Text(Models.Scholarship.Constants.DictionaryKeys.FIELD_REQUIRED));
                retVal = false;
            }
            /*if (model.PeopleOfDeterminationCard == null && existingModel.ExistingPODCard == null)
            {
                ModelState.AddModelError("PeopleOfDeterminationCard", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }
            if (model.UniversityTranscript == null && existingModel.ExistingUniversityTranscript == null)
            {
                ModelState.AddModelError("UniversityTranscript", Translate.Text(PersonalDetail.FIELD_REQUIRED_MESSAGE_KEY));
                retVal = false;
            }*/
            return retVal;
        }

        #endregion Validation

        #region private methods

        private bool TryLogin(Login model, out string responseMessage)
        {
            responseMessage = string.Empty;
            try
            {
                var response = ScholarshipServiceClient.SignIn(model.UserName, model.UserPassword, RequestLanguage, Request.Segment(), string.Empty);

                if (response.Succeeded)
                {
                    model.InitialLogin = response.Payload.@return.initPassword.ToUpper().Equals("X");

                    if (model.InitialLogin) return true;

                    model.WebServiceCredentials = response.Payload.@return.credentials;

                    CacheProvider.Store(CacheKeys.SCHOLARSHIP_LOGIN_MODEL, new CacheItem<Login>(model, TimeSpan.FromMinutes(60)));

                    ProfileUpdateModel updateModel = response.Payload.@return.candidateDetails.MapToModel(UpdateStage.PersonalInformation);

                    updateModel.Stage = UpdateStage.PersonalInformation;

                    SaveProfileUpdateStage(updateModel);

                    AuthStateService.Save(new DewaProfile(model.UserName, "", Roles.scholarship)
                    {
                        IsContactUpdated = true
                    });
                    return true;
                }
                else
                {
                    responseMessage = response.Message;
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                responseMessage = ex.Message;
                return false;
            }
        }

        private studentAttachmentData[] GetAttachmentsForPersonalInformation(PersonalDetail model)
        {
            List<studentAttachmentData> attachments = new List<studentAttachmentData>();
            if (model.Photo != null && model.Photo.ContentLength > 100)
            {
                byte[] item = new byte[model.Photo.ContentLength];
                model.Photo.InputStream.Read(item, 0, model.Photo.ContentLength);
                model.Photo.InputStream.Close();

                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_PHOTO,
                    filename = PersonalDetail.CANDIDATE_PHOTO_FILE_NAME + "." + model.Photo.GetTrimmedFileExtension()
                });
            }
            if (model.EmirateIDUpload != null && model.EmirateIDUpload.ContentLength > 100)
            {
                byte[] item = new byte[model.EmirateIDUpload.ContentLength];
                model.EmirateIDUpload.InputStream.Read(item, 0, model.EmirateIDUpload.ContentLength);
                model.EmirateIDUpload.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_EMIRATES_ID,
                    filename = PersonalDetail.CANDIDATE_EMIRATES_ID_FILE_NAME + "." + model.EmirateIDUpload.GetTrimmedFileExtension()
                });
            }
            if (model.PassportUpload != null && model.PassportUpload.ContentLength > 100)
            {
                byte[] item = new byte[model.PassportUpload.ContentLength];
                model.PassportUpload.InputStream.Read(item, 0, model.PassportUpload.ContentLength);
                model.PassportUpload.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_PASSPORT,
                    filename = PersonalDetail.CANDIDATE_PASSPORT_FILE_NAME + "." + model.PassportUpload.GetTrimmedFileExtension()
                });
            }
            if (model.FamilyBookUpload != null && model.FamilyBookUpload.ContentLength > 100)
            {
                byte[] item = new byte[model.FamilyBookUpload.ContentLength];
                model.FamilyBookUpload.InputStream.Read(item, 0, model.FamilyBookUpload.ContentLength);
                model.FamilyBookUpload.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_FAMILY_BOOK,
                    filename = PersonalDetail.CANDIDATE_FAMILYBOOK_FILE_NAME + "." + model.FamilyBookUpload.GetTrimmedFileExtension()
                });
            }
            return attachments.ToArray();
        }

        private studentAttachmentData[] GetAttachmentsForAcademicInformation(AcademicDetail model)
        {
            List<studentAttachmentData> attachments = new List<studentAttachmentData>();
            if (model.AcademicCertificates != null && model.AcademicCertificates.ContentLength > 100)
            {
                byte[] item = new byte[model.AcademicCertificates.ContentLength];
                model.AcademicCertificates.InputStream.Read(item, 0, model.AcademicCertificates.ContentLength);
                model.AcademicCertificates.InputStream.Close();

                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_ACADEMIC_CERTIFICATE,
                    filename = AcademicDetail.ACADEMIC_CERTIFICATES_FILE_NAME + "." + model.AcademicCertificates.GetTrimmedFileExtension()
                });
            }
            if (model.Grade12FinalCertificate != null && model.Grade12FinalCertificate.ContentLength > 100)
            {
                byte[] item = new byte[model.Grade12FinalCertificate.ContentLength];
                model.Grade12FinalCertificate.InputStream.Read(item, 0, model.Grade12FinalCertificate.ContentLength);
                model.Grade12FinalCertificate.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_GRADE12_FINAL_CERTIFICATE,
                    filename = AcademicDetail.ACADEMIC_GRADE12_FINAL_CERTIFICATES_FILE_NAME + "." + model.Grade12FinalCertificate.GetTrimmedFileExtension()
                });
            }
            if (model.CertificateOfGoodConduct != null && model.CertificateOfGoodConduct.ContentLength > 100)
            {
                byte[] item = new byte[model.CertificateOfGoodConduct.ContentLength];
                model.CertificateOfGoodConduct.InputStream.Read(item, 0, model.CertificateOfGoodConduct.ContentLength);
                model.CertificateOfGoodConduct.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_CO_GOOD_CONDUCT,
                    filename = AcademicDetail.ACADEMIC_CERTIFICATE_OF_GOOD_CONDUCT_FILE_NAME + "." + model.CertificateOfGoodConduct.GetTrimmedFileExtension()
                });
            }
            if (model.PoliceCertificate != null && model.PoliceCertificate.ContentLength > 100)
            {
                byte[] item = new byte[model.PoliceCertificate.ContentLength];
                model.PoliceCertificate.InputStream.Read(item, 0, model.PoliceCertificate.ContentLength);
                model.PoliceCertificate.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_POLICE_CLEARANCE,
                    filename = AcademicDetail.ACADEMIC_POLICE_CLEARANCE_FILE_NAME + "." + model.PoliceCertificate.GetTrimmedFileExtension()
                });
            }
            if (model.PeopleOfDeterminationCard != null && model.PeopleOfDeterminationCard.ContentLength > 100)
            {
                byte[] item = new byte[model.PeopleOfDeterminationCard.ContentLength];
                model.PeopleOfDeterminationCard.InputStream.Read(item, 0, model.PeopleOfDeterminationCard.ContentLength);
                model.PeopleOfDeterminationCard.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_PEOPLE_OF_DETERMINATION,
                    filename = AcademicDetail.ACADEMIC_POD_CARD_FILE_NAME + "." + model.PeopleOfDeterminationCard.GetTrimmedFileExtension()
                });
            }
            if (model.UniversityTranscript != null && model.UniversityTranscript.ContentLength > 100)
            {
                byte[] item = new byte[model.UniversityTranscript.ContentLength];
                model.UniversityTranscript.InputStream.Read(item, 0, model.UniversityTranscript.ContentLength);
                model.UniversityTranscript.InputStream.Close();
                attachments.Add(new studentAttachmentData()
                {
                    data = item,
                    mimetype = CNST.AT_UNIVERSITY_TRANSCRIPT,
                    filename = AcademicDetail.ACADEMIC_UNIVERSITY_TRANSCRIPT_FILE_NAME + "." + model.UniversityTranscript.GetTrimmedFileExtension()
                });
            }
            return attachments.ToArray();
        }

        private List<SelectListItem> GetHelpValues(HelpValueType type, string parent = "")
        {
            parent = string.IsNullOrEmpty(parent) ? string.Empty : ":" + parent;
            try
            {
                if (this._helpValues == null) this._helpValues = GetHelpValuesFromCache();

                switch (type)
                {
                    case HelpValueType.ApplicationSources:
                        return this._helpValues.source.Select(x => new SelectListItem() { Text = x.value.Trim(), Value = x.key.Trim() }).ToList();

                    case HelpValueType.Areas:
                        if (!string.IsNullOrEmpty(parent))
                        {
                            var uvs = new List<SelectListItem>();
                            foreach (var i in this._helpValues.area)
                            {
                                if (i.key.Contains(parent))
                                {
                                    string[] varr = i.key.Split(':');
                                    uvs.Add(new SelectListItem()
                                    {
                                        Value = varr.Length == 2 ? varr[0].Trim() : "",
                                        Text = i.value.Trim()
                                    }); continue;
                                }
                            }
                            return uvs;
                        }
                        return this._helpValues.area.Select(x => new SelectListItem() { Text = x.value.Trim(), Value = x.key.Trim() }).ToList();

                    case HelpValueType.Countries:
                        return this._helpValues.country.Select(x => new SelectListItem() { Text = x.value.Trim(), Value = x.key.Trim() }).ToList();

                    case HelpValueType.Emirates:
                        if (!string.IsNullOrEmpty(parent))
                        {
                            var uvs = new List<SelectListItem>();
                            foreach (var i in this._helpValues.emirate)
                            {
                                if (i.key.Contains(parent))
                                {
                                    string[] varr = i.key.Split(':');
                                    uvs.Add(new SelectListItem()
                                    {
                                        Value = varr.Length == 2 ? varr[0].Trim() : "",
                                        Text = i.value.Trim()
                                    }); continue;
                                }
                            }
                            return uvs;
                        }
                        else
                        {
                            var uvs = new List<SelectListItem>();
                            foreach (var i in this._helpValues.emirate)
                            {
                                string[] varr = i.key.Split(':');
                                uvs.Add(new SelectListItem()
                                {
                                    Value = varr.Length == 2 ? varr[0].Trim() : "",
                                    Text = i.value.Trim()
                                }); continue;
                            }
                            return uvs;
                        }
                    //return this._helpValues.emirate.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                    case HelpValueType.Grades:
                        return this._helpValues.grade.Select(x => new SelectListItem() { Text = x.value.Trim(), Value = x.key.Trim() }).ToList();

                    case HelpValueType.Levels:
                        if (!string.IsNullOrEmpty(parent))
                        {
                            var uvs = new List<SelectListItem>();
                            foreach (var i in this._helpValues.level)
                            {
                                if (i.key.Contains(parent))
                                {
                                    string[] varr = i.key.Split(':');
                                    uvs.Add(new SelectListItem()
                                    {
                                        Value = varr.Length == 2 ? varr[0].Trim() : "",
                                        Text = i.value.Trim()
                                    }); continue;
                                }
                            }
                            return uvs;
                        }
                        return this._helpValues.level.Select(x => new SelectListItem() { Text = x.value.Trim(), Value = x.key.Trim() }).ToList();

                    case HelpValueType.Programs:
                        return this._helpValues.program.Select(x => new SelectListItem() { Text = x.value.Trim(), Value = x.key.Trim() }).ToList();

                    case HelpValueType.Universities:
                        if (!string.IsNullOrEmpty(parent))
                        {
                            var uvs = new List<SelectListItem>();
                            foreach (var i in this._helpValues.university)
                            {
                                if (i.key.Contains(parent))
                                {
                                    string[] varr = i.key.Split(':');
                                    uvs.Add(new SelectListItem()
                                    {
                                        Value = varr.Length == 2 ? varr[0].Trim() : "",
                                        Text = i.value.Trim()
                                    }); continue;
                                }
                            }
                            return uvs;
                        }
                        return this._helpValues.university.Select(x => new SelectListItem() { Text = x.value.Trim(), Value = x.key.Trim() }).ToList();

                    case HelpValueType.Majors:
                        if (!string.IsNullOrEmpty(parent))
                        {
                            var uvs = new List<SelectListItem>();
                            foreach (var i in this._helpValues.major)
                            {
                                if (i.key.Contains(parent))
                                {
                                    string[] varr = i.key.Split(':');
                                    uvs.Add(new SelectListItem()
                                    {
                                        Value = varr.Length == 2 ? varr[0].Trim() : "",
                                        Text = i.value.Trim()
                                    }); continue;
                                }
                            }
                            return uvs;
                        }
                        else
                        {
                            var uvs = new List<SelectListItem>();
                            foreach (var i in this._helpValues.major.Distinct())
                            {
                                if (uvs.Any(x => x.Value.Equals(i.value.Trim()))) { continue; }
                                string[] varr = i.key.Split(':');
                                if (varr.Length == 2 && !uvs.Any(x => x.Value.Equals(varr[0].Trim())))
                                    uvs.Add(new SelectListItem()
                                    {
                                        Value = varr.Length == 2 ? varr[0].Trim() : "",
                                        Text = i.value
                                    }); continue;
                            }
                            return uvs;
                        }
                        //return this._helpValues.major.Select(x => new SelectListItem() { Text = x.value, Value = x.key }).ToList();
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return new List<SelectListItem>();
        }

        private scholarshipHelpValues GetHelpValuesFromCache()
        {
            var hv = new scholarshipHelpValues();

            switch (RequestLanguage)
            {
                case SupportedLanguage.English:
                    //if (!ApplicationCacheProvider.TryGet(CacheKeys.SCHOLARSHIP_HELP_VALUES_ENGLISH_CACHE_KEY, out hv))
                    //{
                    hv = ScholarshipServiceClient.GetHelpValuesInEnglish().Payload;
                    //ApplicationCacheProvider.Store(CacheKeys.SCHOLARSHIP_HELP_VALUES_ENGLISH_CACHE_KEY, hv);
                    //}
                    break;

                case SupportedLanguage.Arabic:
                    //if (!ApplicationCacheProvider.TryGet(CacheKeys.SCHOLARSHIP_HELP_VALUES_ARABIC_CACHE_KEY, out hv))
                    //{
                    hv = ScholarshipServiceClient.GetHelpValuesInArabic().Payload;
                    //ApplicationCacheProvider.Store(CacheKeys.SCHOLARSHIP_HELP_VALUES_ARABIC_CACHE_KEY, hv);
                    //}
                    break;
            }

            return hv;
        }

        private bool IsUserLoggedIn(out Login model)
        {
            //Login model = new Login();
            if (CacheProvider.TryGet<Login>(CacheKeys.SCHOLARSHIP_LOGIN_MODEL, out model))
            {
                return true;
            }
            return false;
        }

        private bool IsUserLoggedIn()
        {
            Login model = new Login();
            if (CacheProvider.TryGet<Login>(CacheKeys.SCHOLARSHIP_LOGIN_MODEL, out model))
            {
                return true;
            }
            return false;
        }

        private Login GetLoggedInCandidate()
        {
            Login model = new Login();
            if (!CacheProvider.TryGet<Login>(CacheKeys.SCHOLARSHIP_LOGIN_MODEL, out model))
            {
                return null;
            }
            return model;
        }

        private ProfileUpdateModel GetProfileUpdateStage()
        {
            ProfileUpdateModel model = new ProfileUpdateModel();

            if (!CacheProvider.TryGet<ProfileUpdateModel>(CacheKeys.SCHOLARSHIP_PROFILE_UPDATE_STATE_CACHE_KEY, out model))
            {
                //var user = GetLoggedInCandidate();

                //var response = ScholarshipServiceClient.
                model = new ProfileUpdateModel() { Stage = UpdateStage.PersonalInformation, PersonalInformation = new PersonalDetail() };
            }

            return model;
        }

        private void SaveProfileUpdateStage(ProfileUpdateModel model)
        {
            CacheProvider.Store(CacheKeys.SCHOLARSHIP_PROFILE_UPDATE_STATE_CACHE_KEY,
                new CacheItem<ProfileUpdateModel>(model, TimeSpan.FromMinutes(30)));
        }

        private enum HelpValueType
        {
            Programs, Countries, Emirates, Areas, Universities, Majors, Grades, Levels, ApplicationSources
        }

        #endregion private methods

        #region LookupAjax Methods

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetAreas(string emid)
        {
            if (!string.IsNullOrWhiteSpace(emid) && emid.Length == 3)
            {
                IEnumerable<SelectListItem> areas = GetHelpValues(HelpValueType.Areas, emid);
                //return Json(areas, JsonRequestBehavior.AllowGet);
                return Json(areas.Select(x => new { Value = x.Value, Text = x.Text }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetRegions(string emid)
        {
            if (!string.IsNullOrWhiteSpace(emid))
            {
                IEnumerable<SelectListItem> rgns = GetHelpValues(HelpValueType.Emirates, emid);
                //return Json(areas, JsonRequestBehavior.AllowGet);
                return Json(rgns.Select(x => new { Value = x.Value, Text = x.Text }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetUniversities(string emid)
        {
            if (!string.IsNullOrWhiteSpace(emid))
            {
                IEnumerable<SelectListItem> uni = GetHelpValues(HelpValueType.Universities, emid);
                //return Json(areas, JsonRequestBehavior.AllowGet);
                return Json(uni.Select(x => new { Value = x.Value, Text = x.Text }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetMajors(string emid)
        {
            if (!string.IsNullOrWhiteSpace(emid))
            {
                IEnumerable<SelectListItem> mjrs = GetHelpValues(HelpValueType.Majors, emid);
                //return Json(areas, JsonRequestBehavior.AllowGet);
                return Json(mjrs.Select(x => new { Value = x.Value, Text = x.Text }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetLevels(string emid)
        {
            if (!string.IsNullOrWhiteSpace(emid))
            {
                IEnumerable<SelectListItem> lvls = GetHelpValues(HelpValueType.Levels, emid);

                return Json(lvls.Select(x => new { Value = x.Value, Text = x.Text }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #endregion LookupAjax Methods
    }
}