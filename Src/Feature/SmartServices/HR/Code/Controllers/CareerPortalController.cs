using DEWAXP.Feature.Account.Filters;
using DEWAXP.Feature.HR.Helpers.CareerPortal;
using DEWAXP.Feature.HR.Models.CareerPortal;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.AccountModel;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Content.Utils;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.JobSeekerSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Roles = DEWAXP.Foundation.Content.Roles;
using SitecoreX = Sitecore.Context;

//using global::Sitecore.Globalization;

namespace DEWAXP.Feature.HR.Controllers
{
    public class CareerPortalController : BaseController
    {
        //conversationId=7sjzLEeImUb8GichBrXux4-in&loginType=jobseeker&languageCode=en-US
        private const string conversationId = "conversationId", loginType = "loginType";

        #region Actions

        // GET: CareerPortal
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            LoginModel model = new LoginModel();
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }

            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                //ClearSessionAndSignOut();
            }
            if (!string.IsNullOrEmpty(Request.QueryString[conversationId]))
            {
                returnUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.RammasLandingPage);
            }
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("~/Views/Feature/HR/CareerPortal/_Login.cshtml", model);
        }

        [HttpPost, AntiForgeryHandleError, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                /* try
                 {
                     string error = string.Empty, sessionId;
                     ServiceResponse<userLoginValidation> response;
                     // if(true)
                     if (TryLoginWithJobSeeker(model, out response, out sessionId))
                     {
                         returnUrl = HttpUtility.UrlDecode(returnUrl);
                         if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                         {
                             if (Session[Models.DirectLine.BotService._conversation_key] != null)
                             {
                                 //Rammas chat already initiated
                                 Session.Add("R01", sessionId);
                             }

                             return Redirect(returnUrl);
                         }
                         return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                     }

                     if (response.Payload != null && response.Payload.errorcode == "10212")
                     {
                         ModelState.AddModelError(string.Empty, response.Message);
                         //ViewBag.ReturnUrl = returnUrl;
                         return PartialView("~/Views/Feature/HR/CareerPortal/_Login.cshtml", model);
                     }
                     else if (!response.Succeeded && response.Payload != null && response.Payload.errorcode.Equals("116"))
                     {
                         ModelState.AddModelError("jobseeker_accountlock", response.Message);

                         //lockerror = response.Message;
                         CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + ("unlock"), new AccessCountingCacheItem<string>("5", Times.Once));
                         CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Username, new AccessCountingCacheItem<string>(model.Username, Times.Once));

                         return PartialView("~/Views/Feature/HR/CareerPortal/_Login.cshtml", model);
                     }
                 }
                 catch (System.Exception ex)
                 {
                     ModelState.AddModelError(string.Empty, Translate.Text(Convert.ToString(ex.InnerException ?? null)));
                 }*/

                try
                {
                    string error, lockerror, sessionId;
                    if (TryLogin(model, out error, out lockerror, out sessionId))
                    {
                        returnUrl = HttpUtility.UrlDecode(returnUrl);
                        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            if (Session[GenericConstants._conversation_key] != null)
                            {
                                //Rammas chat already initiated
                                Session.Add("R01", sessionId);
                            }

                            return Redirect(returnUrl);
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                    }
                    if (!string.IsNullOrWhiteSpace(lockerror))
                    {
                        ModelState.AddModelError("jobseeker_accountlock", lockerror);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    //ModelState.AddModelError(string.Empty, Translate.Text(Convert.ToString(ex.InnerException ?? null)));
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            //ViewBag.ReturnUrl = returnUrl;
            return PartialView("~/Views/Feature/HR/CareerPortal/_Login.cshtml", model);
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
            }
            myAppDashboardRequest request = new myAppDashboardRequest();
            Dashboard model = new Dashboard();
            var responseDashboard = JobSeekerClient.PutCandidateMyAppdashboard(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (responseDashboard.Succeeded && responseDashboard.Payload != null)
            {
                if (responseDashboard.Payload.jobdetails != null)
                {
                    var applicationDetails = responseDashboard.Payload.jobdetails.Select(x => new ApplicationDetail
                    {
                        ApplicationDate = x.applicationdate,
                        ApplicationStausID = x.applicationstatus,
                        ApplicationStausText = x.applicationstatustext,
                        PostingGuid = x.postingguid,
                        PostingGuidStatus = x.postingguidstatus,
                        JobDetails = x.jobapplicationdetails.Select(j => new JobDetail
                        {
                            JobId = j.objectid,
                            JobTitle = x.postingheader,
                            ObjectId = j.objectid,
                            ObjectType = j.objecttype,
                            PlanVersion = j.planversion
                        }).First()
                    }).ToList();
                    model.applicationDetails = applicationDetails;
                }
            }
            else
            {
                if (responseDashboard.Payload.errorcode == "10510")
                {
                    ClearSessionAndSignOut();
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
                }
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/Dashboard.cshtml", model);
        }

        [HttpPost]
        public ActionResult Dashboard(string postGUID)
        {
            return View("~/Views/Feature/HR/CareerPortal/Dashboard.cshtml");
        }

        [HttpPost]
        public ActionResult ChangePicture(FormCollection fileCollection, string id)
        {
            long FileSizeLimit = 2048000;
            string error = null;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                var supportedTypes = new[] { ".jpg", ".png", ".jpeg", ".PNG", ".JPG", ".JPEG" };
                if (!CustomeAttachmentIsValid(file, FileSizeLimit, out error, supportedTypes))
                {
                    ModelState.AddModelError(string.Empty, "Invalid file format");
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>("Invalid file format", Times.Once));
                }
                else
                {
                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                    {
                        fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                        attachmentsRequest request = null;
                        if (id != null)
                        {
                            string[] strArray = id.Split('#');
                            request = new attachmentsRequest
                            {
                                attachmentheader = CareerPortalHelper.CandidateAttachmentType,
                                attachmenttype = CareerPortalHelper.CandidateAttachmentType,
                                attachmenttypetext = fileName,
                                filename = fileName,
                                content = fileData,
                                contenttype = mimeType,
                                objectid = strArray[0],
                                objecttype = strArray[1],
                                planversion = strArray[2],
                                sequencenumber = strArray[3],
                                attachment = strArray[4],
                                updatemode = "U",
                            };
                        }
                        else
                        {
                            request = new attachmentsRequest
                            {
                                attachmentheader = CareerPortalHelper.CandidateAttachmentType,
                                attachmenttype = CareerPortalHelper.CandidateAttachmentType,
                                attachmenttypetext = fileName,
                                filename = fileName,
                                content = fileData,
                                contenttype = mimeType,
                                updatemode = "I",
                            };
                        }
                        var response = JobSeekerClient.GetCandidateAttachements(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        if (response.Succeeded && response.Payload != null)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                            CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                        }
                    }
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
        }

        [HttpGet]
        public ActionResult CandidateProfile()
        {
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
            }
            CandidateProfile model = new CandidateProfile();
            model = GetCandidateAttachements();
            if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).ToList().Count > 0)
            {
                var picList = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).ToList();
                foreach (var item in picList)
                {
                    model.ProfilePicId = item.objectid + "#" + item.objecttype + "#" + item.planversion + "#" + item.sequencenumber + "#" + item.attachment;
                    break;
                }
                model.ProfilePic = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).First();
            }//0012

            string sectioncount = string.Empty;
            ViewBag.CandidateName = CurrentPrincipal.Name;
            model.LastLogin = CurrentPrincipal.LastLogin;
            //model.ProfileProgress = ProfileProgress();

            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_POSTED_SECTION, out sectioncount))
            {
                model.Section = sectioncount;
            }

            string errorMsg = string.Empty;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_ErrorMessage, out errorMsg))
            {
                ModelState.AddModelError(string.Empty, errorMsg);
                CacheProvider.Remove(CacheKeys.CAREERPORTAL_ErrorMessage);
            }

            List<string> lsterrorMsg = new List<string>();
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_ListErrorMessage, out lsterrorMsg))
            {
                foreach (var error in lsterrorMsg)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                CacheProvider.Remove(CacheKeys.CAREERPORTAL_ListErrorMessage);
            }
            if (string.IsNullOrWhiteSpace(sectioncount))
                model.Section = "1";

            var response = JobSeekerClient.GetProfileHelpValues(RequestLanguage, Request.Segment());
            CacheProvider.Store(CacheKeys.CAREERPORTAL_HELP_VALUES, new CacheItem<profileHelpValues>(response.Payload, TimeSpan.FromMinutes(40)));

            if (!response.Succeeded)
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/_CandidateProfile.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CandidateProfile(CandidateProfile model)
        {
            //return View("~/Views/Feature/HR/CareerPortal/_CandidateProfile.cshtml", model);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
        }

        [HttpGet]
        public ActionResult PersonOfDetermination()
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
            return PartialView("~/Views/Feature/HR/CareerPortal/PersonOfDetermination.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonOfDetermination(POD model)
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
                if (!status)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }

                if (ModelState.IsValid && status)
                {
                    var request = new PutPODCandidateRegistration
                    {
                        name = model.FullName,
                        emailaddress = model.EmailAddress,
                        mobilenumber = model.CountryCode + model.MobileNumber,
                        podid = model.PodID
                    };
                    var response = JobSeekerClient.PODCandidateRegistration(request, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                        return PartialView("~/Views/Feature/HR/CareerPortal/_PODSuccess.cshtml");
                    else
                        ModelState.AddModelError(string.Empty, response.Message);
                }
                else
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
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, Translate.Text(ex.InnerException.ToString()));
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/PersonOfDetermination.cshtml", model);
        }

        [HttpGet]
        public ActionResult JobDetails(string q)
        {
            JobDetails model = new JobDetails();
            var jobId = q;
            model.JobId = jobId;
            List<Jobs> jobList = new List<Jobs>();
            if (IsLoggedIn || CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                ViewBag.IsLoggedIn = IsLoggedIn.ToString().ToLower();
            }
            else
                ViewBag.IsLoggedIn = "false";

            string jobPostingGUID = string.Empty;
            if (!string.IsNullOrWhiteSpace(jobId))
            {
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_SEARCH_JOBS, out jobList))
                {
                    if (jobList != null)
                        jobPostingGUID = jobList.Where(x => x.Jobid == jobId).Select(x => x.JobPostingKey).FirstOrDefault();
                }
                var response = JobSeekerClient.GetCandidatePostingdisplay(jobId, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    model.JobTitle = response.Payload.jobpostingheaderdescription;
                    model.Department = response.Payload.jobpostingdepartmentdescription;
                    model.EndDate = response.Payload.jobpostingdate;
                    model.Project = response.Payload.jobpostingprojectdescription;
                    model.Requirement = response.Payload.jobpostingrequirementdescription;
                    model.Task = response.Payload.jobpostingtaskdescription;
                    model.Company = response.Payload.jobpostingcompanydescription;
                    model.PostingGUID = jobPostingGUID;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    if (response.Payload != null && (response.Payload.errorcode == "10510" || response.Payload.userid == null))
                    {
                        ClearSessionAndSignOut();
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
                    }
                }
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            return View("~/Views/Feature/HR/CareerPortal/JobDetails.cshtml",model);
        }

        [HttpGet]
        public ActionResult TellFriend(string q)
        {
            ViewBag.JobId = q;
            //var response = JobSeekerClient.GetCandidateProfile(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
            }
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            return View("~/Views/Feature/HR/CareerPortal/TellFriend.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TellFriend(TellAFriend model, string JobId)
        {
            //var response = JobSeekerClient.GetCandidateProfile(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
            }
            if (ModelState.IsValid)
            {
                PutTellAFriend request = new PutTellAFriend
                {
                    recipentname = model.RecipentName,
                    recipentemailaddress = model.RecipentEmailAddress,
                    sendername = model.SenderName,
                    sendermessage = model.SenderMessage,
                    postingguid = model.JobId
                };

                var response = JobSeekerClient.PutTellAFriend(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "")
                {
                    return PartialView("~/Views/Feature/HR/CareerPortal/_TellAFriendSuccess.cshtml");
                }
                else
                    ModelState.AddModelError(string.Empty, response.Message);
            }

            return View("~/Views/Feature/HR/CareerPortal/TellFriend.cshtml");
        }

        [HttpPost]
        public ActionResult ApplyJob(string jobDesc, string jobId)
        {
            ViewBag.JobDescription = jobDesc;
            ViewBag.jobId = jobId;

            //if (IsLoggedIn || CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            //{
            //    Helpers.QueryString p = new Helpers.QueryString(true);
            //    p.With("jobId", jobId, false);
            //    // string redirectUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD) + "?jobId=" + jobId;
            //    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
            //}

            return PartialView("~/Views/Feature/HR/CareerPortal/_ApplyJob.cshtml");
        }

        [HttpGet]
        public ActionResult ApplicationWizard(string q)
        {
            var jobId = q;
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
            }
            if (string.IsNullOrWhiteSpace(jobId))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }

            CandidateProfile model = new CandidateProfile();
            model = GetCandidateAttachements();
            if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).ToList().Count > 0)
            {
                var picList = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).ToList();
                foreach (var item in picList)
                {
                    model.ProfilePicId = item.objectid + "#" + item.objecttype + "#" + item.planversion + "#" + item.sequencenumber + "#" + item.attachment;
                    break;
                }
                model.ProfilePic = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).First();
            }
            model.JobId = jobId;
            string sectioncount = string.Empty;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_POSTED_SECTION, out sectioncount))
            {
                model.Section = sectioncount;
            }
            string errorMsg = string.Empty;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_ErrorMessage, out errorMsg))
            {
                ModelState.AddModelError(string.Empty, errorMsg);
            }

            if (string.IsNullOrWhiteSpace(sectioncount))
                model.Section = "6";

            var response = JobSeekerClient.GetProfileHelpValues(RequestLanguage, Request.Segment());
            CacheProvider.Store(CacheKeys.CAREERPORTAL_HELP_VALUES, new CacheItem<profileHelpValues>(response.Payload, TimeSpan.FromMinutes(40)));
            if (!response.Succeeded)
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/ApplicationWizard.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalInformation(CandidateProfile model)
        {
            if (ModelState.IsValid)
            {
                model.Section = "1";
                // Call web method to update the personal information

                DateTime dateResult = getCultureDate(model.DOB);
                additionaldataRequest[] additionaldatalist = new additionaldataRequest[]
                {
                        new additionaldataRequest
                        {
                            disabilityID = model.DisabilityId,
                            emiratesID = model.EmiratesID,
                            experience = model.YearsOfExperience,
                            highestqualificationlevel = model.HighestQualificationLevel,
                            passportnumber = model.PassportNo,
                            religion = model.Religion,
                            UAEresident = model.IsUAEResident
                        }
                };

                var updateRequest = new profileUpdateRequest
                {
                    updatemode = "U",
                    firstname = model.FirstName,
                    lastname = model.LastName,
                    gender = model.Gender,
                    maritalstatus = model.MaritalStatus,
                    birthdate = dateResult != null ? dateResult.ToString("yyyyMMdd") : string.Empty,
                    nationality = model.Nationality,
                    additionaldatalist = additionaldatalist,
                    mobile = model.MobileNo
                };
                var responseProfile = JobSeekerClient.GetCandidateProfile(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (responseProfile.Succeeded && responseProfile.Payload != null)
                {
                    model.Section = "2";
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_PERSONAL_INFO, new CacheItem<profileUpdate>(responseProfile.Payload, TimeSpan.FromMinutes(60)));
                    if (!model.SaveandContinueButtonClicked)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
                    }
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    if (model.JobId != null)
                    {
                        QueryString p = new QueryString(true);
                        p.With("q", model.JobId, false);
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
                    }
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                }
                else
                {
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(responseProfile.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContactAddress(CandidateProfile model)
        {
            if (ModelState.IsValid)
            {
                model.Section = "2";
                personalDataAddressRequest[] addressdatalist = new personalDataAddressRequest[]
                {
                        new personalDataAddressRequest
                        {
                            country = model.Country,
                            city = model.City,
                            region = model.Emirates,
                            street = model.permantAddress,
                            communicationchannel = "01",
                            postalcode = model.PostalCode // It's mandatory as suggested by USHA
                        }
                };
                CultureInfo culture;
                DateTimeStyles styles;

                culture = SitecoreX.Culture;
                if (culture.ToString().Equals("ar-AE"))
                {
                    model.DOB = model.DOB.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                }
                styles = DateTimeStyles.None;
                DateTime dateResult;
                DateTime.TryParse(model.DOB, culture, styles, out dateResult);

                additionaldataRequest[] additionaldatalist = new additionaldataRequest[]
                {
                        new additionaldataRequest
                        {
                            disabilityID = model.DisabilityId,
                            emiratesID = model.EmiratesID,
                            experience = model.YearsOfExperience,
                            highestqualificationlevel = model.HighestQualificationLevel,
                            passportnumber = model.PassportNo,
                            religion = model.Religion,
                            UAEresident = model.IsUAEResident
                        }
                };

                var updateRequest = new profileUpdateRequest
                {
                    updatemode = "U",
                    firstname = model.FirstName,
                    lastname = model.LastName,
                    gender = model.Gender,
                    maritalstatus = model.MaritalStatus,
                    birthdate = dateResult != null ? dateResult.ToString("yyyyMMdd") : string.Empty,
                    nationality = model.Nationality,
                    additionaldatalist = additionaldatalist,
                    mobile = model.MobileNo,
                    personaldatalist = addressdatalist
                };

                var responseProfile = JobSeekerClient.GetCandidateProfile(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (responseProfile.Succeeded && responseProfile.Payload != null && responseProfile.Payload.errorcode != "10555")
                {
                    model.Section = "3";
                    ViewBag.FullName = model.FirstName + " " + model.LastName;
                    if (!model.SaveandContinueButtonClicked)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
                    }
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    if (model.JobId != null)
                    {
                        QueryString p = new QueryString(true);
                        p.With("q", model.JobId, false);
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
                    }
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                }
                else
                {
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(responseProfile.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EducationProfession(CandidateProfile model)
        {
            model.Section = "4";
            if (!model.SaveandContinueButtonClicked)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WorkExperience(CandidateProfile model)
        {
            model.Section = "5";
            if (!model.SaveandContinueButtonClicked)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AttachmentInfo(CandidateProfile model)
        {
            if (ModelState.IsValid)
            {
                model.Section = "6";
            }
            if (!model.SaveandContinueButtonClicked)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            var isAttachmnet = CheckAttachment();
            if (!isAttachmnet)
                model.Section = "5";
            else
                model.Section = "6";

            CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
            if (model.JobId != null)
            {
                QueryString p = new QueryString(true);
                p.With("q", model.JobId, false);
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
        }

        [HttpGet]
        public ActionResult ProfileSuccess()
        {
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
            }
            string sectioncount = string.Empty;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_POSTED_SECTION, out sectioncount))
            {
                if (sectioncount != "6")
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/_ProfileSuccess.cshtml");
        }

        [HttpGet]
        public ActionResult ApplicationSuccess()
        {
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Jobseeker))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
            }
            string sectioncount = string.Empty;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_POSTED_SECTION, out sectioncount))
            {
                if (sectioncount != "8")
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            string message = string.Empty;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_ErrorMessage, out message))
            {
                ViewBag.SuccessMessage = message;
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/_ApplicationSuccess.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReivewSubmit(CandidateProfile model)
        {
            if (ModelState.IsValid)
            {
                model = GetCandidateAttachements();
                bool isAttached = CheckAttachment();
                model.Section = "5";
                if (isAttached)
                {
                    var response = JobSeekerClient.GetCandidateProfileRelease("I", "X", "X", CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        model.Section = "6";
                        CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE_SUCCESS);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                        CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                    }
                }
            }
            CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Preferences(CandidateProfile model)
        {
            if (ModelState.IsValid)
            {
                model.Section = "6";
                QueryString p = new QueryString(true);
                p.With("q", model.JobId, false);
                List<preferenceRequestDetails> preferenceRequestDetail = new List<preferenceRequestDetails> {
                    new preferenceRequestDetails
                    {
                        functionalarea = model.FunctionalAreas,
                        hierarchylevel = model.HierarachyLevel,
                        objectid = model.objectid,
                        objecttype = model.objecttype,
                        planversion = model.planversion,
                        sequencenumber = model.sequenceNo,
                    }
                };

                var request = new preferenceRequest
                {
                    interestgroup = model.InterestGroup,
                    preferencedetails = preferenceRequestDetail.ToArray(),
                    updatemode = "I", // model.sequenceNo != null ? "U" : "I",
                };
                var responseProfile = JobSeekerClient.GetCandidatePreferenceUpdate(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (responseProfile.Succeeded && responseProfile.Payload != null)
                {
                    model.Section = "8";
                    if (!model.SaveandContinueButtonClicked)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD);
                    }
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    //QueryString q = new QueryString(true);
                    //q.CombineWith("?jobId=" + model.JobId);
                    //return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, q);

                    // string redirectUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD) + "?jobId=" + jobId;
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
                }
                else
                {
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(responseProfile.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CoverLetter(CandidateProfile model)
        {
            QueryString p = new QueryString(true);
            p.With("q", model.JobId, false);
            if (ModelState.IsValid)
            {
                List<applSource> applSources = new List<applSource> {
                    new applSource
                    {
                        applicationsource = model.ApplicationSource,
                        applicationsourcetype =  model.ApplicationSourceType
                    }
                };
                List<coverLetterDetails> coverLetterDetails = new List<coverLetterDetails> {
                    new coverLetterDetails
                    {
                        description = model.CoverLetterDescription
                    }
                };
                var request = new coverLetterUpdateRequest
                {
                    appdatalist = applSources.ToArray(),
                    jobid = model.JobId,
                    updatemode = "I",
                    coverletterlist = coverLetterDetails.ToArray()
                };
                var response = JobSeekerClient.GetCandidateCoverLetterUpdate(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    var applyJobResponse = JobSeekerClient.PutCandidateJobApply(model.JobId, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                    if (applyJobResponse.Succeeded && applyJobResponse.Payload != null)
                    {
                        CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                        CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(applyJobResponse.Payload.errormessage, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_SUCCESS);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, applyJobResponse.Message);
                        CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                        CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(applyJobResponse.Message, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_APPLICATION_WIZARD, p);
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult CandidateRegistration()
        {
            if (IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_DASHBOARD);
            }
            RegistrationModel model = new RegistrationModel();
            //model.RegistrationTypes = GetLstDataSource(DataSources.CAREER_PORTAL_REGISTRATION_TYPES).ToList();
            model.ExperienceTypes = GetLstDataSource(DataSources.CAREER_PORTAL_Experience_Types).ToList();
            model.NatureOfWorkTypes = GetLstDataSource(DataSources.CAREER_PORTAL_Nature_Types).ToList();

            var response = JobSeekerClient.GetProfileHelpValues(RequestLanguage, Request.Segment());
            CacheProvider.Store(CacheKeys.CAREERPORTAL_HELP_VALUES, new CacheItem<profileHelpValues>(response.Payload, TimeSpan.FromMinutes(40)));

            if (!response.Succeeded)
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }

            if (response.Payload != null)
            {
                model.NationalityList = response.Payload.nationlist.Select(x => new SelectListItem
                {
                    Text = x.countrynamefull,
                    Value = x.countrykey
                }).ToList();
            }

            return PartialView("~/Views/Feature/HR/CareerPortal/_CandidateRegistration.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CandidateRegistration(RegistrationModel model)
        {
            model.ExperienceTypes = GetLstDataSource(DataSources.CAREER_PORTAL_Experience_Types).ToList();
            model.NatureOfWorkTypes = GetLstDataSource(DataSources.CAREER_PORTAL_Nature_Types).ToList();

            profileHelpValues profileHelpValues = null;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
            {
                if (profileHelpValues != null)
                {
                    model.NationalityList = profileHelpValues.nationlist.Select(x => new SelectListItem
                    {
                        Text = x.countrynamefull,
                        Value = x.countrykey
                    }).ToList();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = JobSeekerClient.CandidateRegistration(model.Username, model.Password, model.POD, model.SelectedExperienceType, model.SelectedNatureOfWorkType, model.FirstName, model.LastName, model.EmailAddress, model.Nationality, model.TermsCondition.ToString(), model.PassportNumber, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                        return PartialView("~/Views/Feature/HR/CareerPortal/_RegistrationSuccess.cshtml");
                    else
                        ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text(ex.InnerException.ToString()));
                }
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/_CandidateRegistration.cshtml", model);
        }

        [HttpGet]
        public ActionResult CandidateEmailVerification()
        {
            string param = Request.QueryString["PARAM"];
            string resendFlag = "";
            ServiceResponse<emailVerification> response;
            CandidateEmailVerification model = new CandidateEmailVerification();
            if (param != null)
            {
                model.param = param;
                if (!string.IsNullOrWhiteSpace(Request.QueryString["resendFlag"]))
                    resendFlag = "X";
                response = JobSeekerClient.verifyRegistrationEmail(param, resendFlag, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    model.errorcode = response.Payload.errorcode;
                    model.errormessage = response.Payload.errormessage;
                    model.newverificationlinkenable = response.Payload.newverificationlinkenable;
                    model.success = response.Payload.success;
                    if (resendFlag == "X")
                        model.verificationstatus = "3";
                    else
                        model.verificationstatus = response.Payload.verificationstatus;

                    model.applicationvisibility = response.Payload.applicationvisibility;
                    //return PartialView("~/Views/Feature/HR/CareerPortal/_CandidateEmailVerification.cshtml", model);
                }
                else
                {
                    model.verificationstatus = "-1";
                    model.errormessage = response.Message;
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/_CandidateEmailVerification.cshtml", model);
        }

        

        private bool TryLoginWithJobSeeker(LoginModel model, out ServiceResponse<userLoginValidation> response, out string sessionId)
        {
            sessionId = null;

            #region Future Center Cookie functionality added by Shujaat

            var _fc = FetchFutureCenterValues();

            #endregion Future Center Cookie functionality added by Shujaat

            response = JobSeekerClient.GetValidateCandidateLogin(model.Username, Base64Encode(model.Password),language: RequestLanguage,segment: Request.Segment());
            if (response.Succeeded)
            {
                sessionId = response.Payload.credential;
                AuthStateService.Save(new DewaProfile(model.Username, response.Payload.credential, Roles.Jobseeker)
                {
                    IsContactUpdated = true
                });
                return true;
            }
            //error = response.Message;
            return false;
        }

        private bool TryLogin(LoginModel model, out string error, out string lockerror, out string sessionId)
        {
            error = null;
            lockerror = string.Empty;
            var _fc = FetchFutureCenterValues();

            var response = JobseekerRestClient.LoginUser(
                new LoginRequest
                {
                    getloginsessionrequest = new Getloginsessionrequest
                    {
                        userid = model.Username,
                        password = Base64Encode(model.Password),
                        center = _fc.Branch
                    }
                }, RequestLanguage, Request.Segment());

            sessionId = null;
            if (response.Succeeded)
            {
                sessionId = response.Payload.SessionNumber;
                AuthStateService.Save(new DewaProfile(model.Username, response.Payload.SessionNumber, Roles.Jobseeker)
                {
                    IsContactUpdated = true,
                    LastLogin = response.Payload.Lastlogin
                });
                return true;
            }
            else if (!response.Succeeded && response.Payload != null && response.Payload.ResponseCode.Equals("116"))
            {
                lockerror = response.Message;
                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + ("unlock"), new AccessCountingCacheItem<string>("5", Times.Once));
                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Username, new AccessCountingCacheItem<string>(model.Username, Times.Once));
            }
            error = response.Message;
            return false;
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (IsLoggedIn)
            {
                bool isuaepass = false;
                CacheProvider.TryGet("careerusepass", out isuaepass);
                ClearSessionAndSignOut();
                if (isuaepass)
                {
                    if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["UAEPASS_Logout"]))
                    {
                        var link = string.Format(WebConfigurationManager.AppSettings["UAEPASS_Logout"], LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN, false));
                        if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                        {
                            Response.Redirect(link);
                        }
                    }
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
        }

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

        [HttpGet]
        public ActionResult JobSearch()
        {
            SearchJobModel model = new SearchJobModel();
            List<JobFilters> lstfuntionalAreas = new List<JobFilters>();
            List<JobFilters> lsthierarcy = new List<JobFilters>();

            var responseJobFilter = JobSeekerClient.GetJobFilter(RequestLanguage);

            if (responseJobFilter.Succeeded && responseJobFilter.Payload != null && responseJobFilter.Payload.functionalarea != null && responseJobFilter.Payload.functionalarea.Count() > 0)
            {
                lstfuntionalAreas = responseJobFilter.Payload.functionalarea.Select(x => new JobFilters
                {
                    Code = x.functionalarea,
                    Description = x.functionalareadescription
                }).ToList();
                lsthierarcy = responseJobFilter.Payload.hierarcy.Select(x => new JobFilters
                {
                    Code = x.hierarcytype,
                    Description = x.hierarcydescription
                }).ToList();
            }
            else
            {
                ModelState.AddModelError(string.Empty, responseJobFilter.Message);
            }
            model.Hierarcylevel = lsthierarcy;
            model.Functionalarea = lstfuntionalAreas;
            return PartialView("~/Views/Feature/HR/CareerPortal/_JobSearch.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JobSearch(SearchJobModel model)
        {
            if (ModelState.IsValid)
            {
                int currentPage = 1;
                int maxRows = 5;
                List<JobFilters> lstfuntionalAreas = new List<JobFilters>();
                List<JobFilters> lsthierarcy = new List<JobFilters>();

                if (IsLoggedIn || CurrentPrincipal.Role.Equals(Roles.Jobseeker))
                {
                    ViewBag.IsLoggedIn = IsLoggedIn.ToString().ToLower();
                }
                else
                    ViewBag.IsLoggedIn = "false";

                List<Jobs> jobList = new List<Jobs>();
                var response = JobSeekerClient.GetSearchJobs(model.Keyword, string.Empty, model.Hierarcy, string.Empty, string.Empty, string.Empty, model.Functionalareacode, RequestLanguage);
                if (response.Succeeded && response.Payload != null)
                {
                    if (response.Payload.joblistDetails != null)
                    {
                        var responseJobFilter = JobSeekerClient.GetJobFilter(RequestLanguage);
                        if (responseJobFilter.Succeeded && responseJobFilter.Payload != null)
                        {
                            lstfuntionalAreas = responseJobFilter.Payload.functionalarea.Select(x => new JobFilters
                            {
                                Code = x.functionalarea,
                                Description = x.functionalareadescription
                            }).ToList();
                            lsthierarcy = responseJobFilter.Payload.hierarcy.Select(x => new JobFilters
                            {
                                Code = x.hierarcytype,
                                Description = x.hierarcydescription
                            }).ToList();
                            model.Hierarcylevel = lsthierarcy;
                            model.Functionalarea = lstfuntionalAreas;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, responseJobFilter.Message);
                        }
                        jobList = response.Payload.joblistDetails.Select(x => new Jobs
                        {
                            functionalArea = x.functionalarea,
                            Jobid = x.hrjobid,
                            JobPostingKey = x.jobpostingkey,
                            Jobdescription = x.jobdescription,
                            Publishdate = x.publishstartdate != null ? getCultureDate(x.publishstartdate).ToString("dd MMM yyyy") : string.Empty,
                            Referencecode = x.referencecode,
                            EmploymentEndDate = x.employmentenddate != null ? getCultureDate(x.employmentenddate).ToString("dd MMM yyyy") : string.Empty,
                        }).ToList();
                    }
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_SEARCH_JOBS, new CacheItem<List<Jobs>>(jobList, TimeSpan.FromMinutes(20)));
                    //model.Functionalarea = lstfuntionalAreas;
                    model.totalRecords = jobList.Count();
                    model.Joblist = jobList.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList(); ;
                    double pageCount = (double)(jobList.Count() / Convert.ToDecimal(maxRows));
                    model.totalpage = Pager.CalculateTotalPages(jobList.Count, maxRows);
                    model.pagination = model.totalpage > 1 ? true : false;
                    model.pagenumbers = model.totalpage > 1 ? CommonUtility.GetPaginationRange(1, model.totalpage) : new List<int>();
                    model.page = 1;
                    return PartialView("~/Views/Feature/HR/CareerPortal/_SearchResult.cshtml", model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/_JobSearch.cshtml", model);
        }

        /// <summary>
        /// The AjaxWorkLogs
        /// </summary>
        /// <param name="currentPage">The currentPage<see cref="int"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost]
        public ActionResult AjaxSearchJob(int currentPage, int pageSize)
        {
            int maxRows = pageSize;
            SearchJobModel filterJobs = new SearchJobModel();
            List<Jobs> jobList = new List<Jobs>();
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_SEARCH_JOBS, out jobList))
            {
                double pageCount = (double)(jobList.Count() / Convert.ToDecimal(maxRows));
                filterJobs.Joblist = jobList.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

                filterJobs.totalpage = Pager.CalculateTotalPages(jobList.Count, maxRows);
                filterJobs.pagination = filterJobs.totalpage > 1 ? true : false;
                filterJobs.pagenumbers = filterJobs.totalpage > 1 ? CommonUtility.GetPaginationRange(currentPage, filterJobs.totalpage) : new List<int>();
                filterJobs.page = currentPage;
            }
            return PartialView("~/Views/Feature/HR/CareerPortal/_JobsList.cshtml", filterJobs);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AjaxCandidateProfile(string section)
        {
            CandidateProfile model = new CandidateProfile();
            profileHelpValues profileHelpValues = null;
            string regType = string.Empty;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_REGISTRATION_TYPE, out regType))
            {
                model.ExperienceType = regType;
            }

            profileUpdate personalInfo = null;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_PERSONAL_INFO, out personalInfo))
            {
                if (personalInfo != null)
                {
                    ViewBag.FullName = personalInfo.firstname + " " + personalInfo.lastname;
                }
            }

            if (section == "1")
            {
                model = GetPersoanlInfoAddress();
                if (!model.IsSessionCheck)
                    ViewBag.IsSessionTimeOut = "true";
                else
                    ViewBag.IsSessionTimeOut = "false";

                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.ReligionList = profileHelpValues.religion.Select(x => new SelectListItem
                        {
                            Text = x.religiousdenominationdescription,
                            Value = x.religiousdenominationkey
                        }).ToList();
                        model.NationalityList = profileHelpValues.nationlist.Select(x => new SelectListItem
                        {
                            Text = x.countrynamefull,
                            Value = x.countrykey
                        }).ToList();
                        model.EducationLevel = profileHelpValues.educationlevellist.Select(x => new SelectListItem
                        {
                            Text = x.nameofdegree,
                            Value = x.degreelevel
                        }).ToList();
                    }
                }
                model.Section = "1";
                //model.ProfileProgress = ProfileProgress();

                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_PersonalInformation.cshtml", model);
            }
            else if (section == "2")
            {
                model = GetPersoanlInfoAddress();
                if (!model.IsSessionCheck)
                    ViewBag.IsSessionTimeOut = "true";
                else
                    ViewBag.IsSessionTimeOut = "false";

                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.CountryList = profileHelpValues.countrylist.Select(x => new SelectListItem
                        {
                            Text = x.value,
                            Value = x.key
                        }).ToList();
                        if (profileHelpValues.regionlist != null)
                        {
                            model.EmiratesList = profileHelpValues.regionlist.Select(x => new SelectListItem
                            {
                                Text = x.key,
                                Value = x.value
                            }).ToList();
                        }
                    }
                }
                model.Section = "2";
                //model.ProfileProgress = ProfileProgress();

                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_AddressContact.cshtml", model);
            }
            else if (section == "3")
            {
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.QualificationGroup = profileHelpValues.qualificationlist.Select(x => new { x.qualificationgrouptext, x.qualificationgroup }).Distinct().ToList().Select(o => new SelectListItem
                        {
                            Text = o.qualificationgrouptext,
                            Value = o.qualificationgroup
                        }).ToList();
                        model.ProficiencyList = profileHelpValues.proficiencylist.Select(x => new { x.proficiencyrating, x.proficiencyscaletext }).Distinct().ToList().Select(o => new SelectListItem
                        {
                            Text = o.proficiencyscaletext,
                            Value = o.proficiencyrating
                        }).ToList();
                        model.EducationLevel = profileHelpValues.educationlevellist.Select(x => new SelectListItem
                        {
                            Text = x.nameofdegree,
                            Value = x.degreelevel
                        }).ToList();
                        model.FieldOfStudy = profileHelpValues.educationfieldlist.Select(x => new SelectListItem
                        {
                            Text = x.value,
                            Value = x.key
                        }).ToList();

                        model.CertificationList = profileHelpValues.qualificationlist.Where(w => w.qualificationgroup == CareerPortalHelper.CertificatesId).Select(x => new SelectListItem
                        {
                            Text = x.qualificationdescription,
                            Value = x.qualificationid
                        }).ToList();
                    }
                }
                model.CandidateEducation = GetEducationDetails(model);
                model.CandidateQualification = GetQualificationDetails(model);
                if (model.CandidateQualification != null)
                {
                    model.CandidateCertification = model.CandidateQualification.Where(w => w.QualificationGrpId == CareerPortalHelper.CertificatesId).Select(x => new Qualification
                    {
                        objectid = x.objectid,
                        objectversion = x.objectversion,
                        planversion = x.planversion,
                        sequenceNo = x.sequenceNo,
                        QualificationGrp = x.QualificationGrp,
                        QualificationGrpId = model.QualificationGroup.Where(t => t.Text.ToLower() == x.QualificationGrp.ToLower()).Select(t => t.Value).FirstOrDefault(),
                        QualificationId = x.QualificationId,
                        QualificationName = x.QualificationName,
                        ProficiencyName = x.ProficiencyName,
                        ProficiencyId = x.ProficiencyId,
                        IssuingOrganization = x.IssuingOrganization
                    }).ToList();
                    model.CandidateQualification = model.CandidateQualification.Where(w => w.QualificationGrpId != CareerPortalHelper.CertificatesId).ToList();
                }

                model.Section = "3";

                bool IsSubmit;
                model.ProfileProgress = ProfileProgress(out IsSubmit);
                model.IsSubmit = IsSubmit;

                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_EducationProfessional.cshtml", model);
            }
            else if (section == "4")
            {
                model.Section = "4";
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.DesignationList = profileHelpValues.functionalarea.Select(x => new SelectListItem
                        {
                            Text = x.functionalareadescription,
                            Value = x.functionalarea
                        }).ToList();
                        model.CountryList = profileHelpValues.countrylist.Select(x => new SelectListItem
                        {
                            Text = x.value,
                            Value = x.key
                        }).ToList();
                    }
                }
                model.CandidateWorkExperience = GetWorkExpDetails(model);

                bool IsSubmit;
                model.ProfileProgress = ProfileProgress(out IsSubmit);
                model.IsSubmit = IsSubmit;

                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_WorkExperience.cshtml", model);
            }
            else if (section == "5")
            {
                model = GetCandidateAttachements();
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        if (IsUAE_Redisdent())
                        {
                            //remove Passport form list.
                            model.AttachementTypes = profileHelpValues.attachmentlist.Select(x => new SelectListItem
                            {
                                Text = x.attachmenttypedescription,
                                Value = x.attachmenttype
                            }).Where(x => x.Value != "0016").ToList();
                        }
                        else
                        {
                            model.AttachementTypes = profileHelpValues.attachmentlist.Select(x => new SelectListItem
                            {
                                Text = x.attachmenttypedescription,
                                Value = x.attachmenttype
                            }).ToList();
                        }
                    }
                }
                model.Section = "5";

                bool IsSubmit;
                model.ProfileProgress = ProfileProgress(out IsSubmit);
                model.IsSubmit = IsSubmit;

                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_AttachmentsInfo.cshtml", model);
            }
            else if (section == "6")
            {
                model.Section = "6";
                model.ProfileRelease = GetReviewSubmit();
                if (!model.ProfileRelease.Success)
                {
                    ModelState.AddModelError(string.Empty, model.ProfileRelease.strMessage);
                    model.Success = model.ProfileRelease.Success;
                    model.errorCode = model.ProfileRelease.errorCode;
                }

                bool IsSubmit;
                model.ProfileProgress = ProfileProgress(out IsSubmit);
                model.IsSubmit = IsSubmit;

                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_ReviewSubmit.cshtml", model);
            }
            return new EmptyResult();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AjaxApplicationWizard(string section, string jobId)
        {
            CandidateProfile model = new CandidateProfile();
            //var DeserializedModel = JsonConvert.DeserializeObject<CandidateProfile>(modelParameter);
            profileUpdate personalInfo = null;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_PERSONAL_INFO, out personalInfo))
            {
                if (personalInfo != null)
                {
                    ViewBag.FullName = personalInfo.firstname + " " + personalInfo.lastname;
                }
            }
            profileHelpValues profileHelpValues = null;
            if (section == "1")
            {
                model = GetPersoanlInfoAddress();

                if (!model.IsSessionCheck)
                    ViewBag.IsSessionTimeOut = "true";
                else
                    ViewBag.IsSessionTimeOut = "false";

                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.ReligionList = profileHelpValues.religion.Select(x => new SelectListItem
                        {
                            Text = x.religiousdenominationdescription,
                            Value = x.religiousdenominationkey
                        }).ToList();
                        model.NationalityList = profileHelpValues.nationlist.Select(x => new SelectListItem
                        {
                            Text = x.countrynamefull,
                            Value = x.countrykey
                        }).ToList();
                        model.EducationLevel = profileHelpValues.educationlevellist.Select(x => new SelectListItem
                        {
                            Text = x.nameofdegree,
                            Value = x.degreelevel
                        }).ToList();
                    }
                }
                model.Section = "1";
                model.JobId = jobId;
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_PersonalInformation.cshtml", model);
            }
            else if (section == "2")
            {
                model = GetPersoanlInfoAddress();

                if (!model.IsSessionCheck)
                    ViewBag.IsSessionTimeOut = "true";
                else
                    ViewBag.IsSessionTimeOut = "false";

                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.CountryList = profileHelpValues.countrylist.Select(x => new SelectListItem
                        {
                            Text = x.value,
                            Value = x.key
                        }).ToList();
                        if (profileHelpValues.regionlist != null)
                        {
                            model.EmiratesList = profileHelpValues.regionlist.Select(x => new SelectListItem
                            {
                                Text = x.key,
                                Value = x.value
                            }).ToList();
                        }
                    }
                }
                model.Section = "2";
                model.JobId = jobId;
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_AddressContact.cshtml", model);
            }
            else if (section == "3")
            {
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.QualificationGroup = profileHelpValues.qualificationlist.Select(x => new { x.qualificationgrouptext, x.qualificationgroup }).Distinct().ToList().Select(o => new SelectListItem
                        {
                            Text = o.qualificationgrouptext,
                            Value = o.qualificationgroup
                        }).ToList();
                        model.ProficiencyList = profileHelpValues.proficiencylist.Select(x => new { x.proficiencyrating, x.proficiencyscaletext }).Distinct().ToList().Select(o => new SelectListItem
                        {
                            Text = o.proficiencyscaletext,
                            Value = o.proficiencyrating
                        }).ToList();
                        model.EducationLevel = profileHelpValues.educationlevellist.Select(x => new SelectListItem
                        {
                            Text = x.nameofdegree,
                            Value = x.degreelevel
                        }).ToList();
                        model.FieldOfStudy = profileHelpValues.educationfieldlist.Select(x => new SelectListItem
                        {
                            Text = x.value,
                            Value = x.key
                        }).ToList();
                        model.CertificationList = profileHelpValues.qualificationlist.Where(w => w.qualificationgroup == CareerPortalHelper.CertificatesId).Select(x => new SelectListItem
                        {
                            Text = x.qualificationdescription,
                            Value = x.qualificationid
                        }).ToList();
                    }
                }
                model.CandidateEducation = GetEducationDetails(model);
                model.CandidateQualification = GetQualificationDetails(model);
                if (model.CandidateQualification != null)
                {
                    model.CandidateCertification = model.CandidateQualification.Where(w => w.QualificationGrpId == CareerPortalHelper.CertificatesId).Select(x => new Qualification
                    {
                        objectid = x.objectid,
                        objectversion = x.objectversion,
                        planversion = x.planversion,
                        sequenceNo = x.sequenceNo,
                        QualificationGrp = x.QualificationGrp,
                        QualificationGrpId = model.QualificationGroup.Where(t => t.Text.ToLower() == x.QualificationGrp.ToLower()).Select(t => t.Value).FirstOrDefault(),
                        QualificationId = x.QualificationId,
                        QualificationName = x.QualificationName,
                        ProficiencyName = x.ProficiencyName,
                        ProficiencyId = x.ProficiencyName
                    }).ToList();
                    model.CandidateQualification = model.CandidateQualification.Where(w => w.QualificationGrpId != CareerPortalHelper.CertificatesId).ToList();
                    if (!ModelState.IsValid)
                    {
                        ClearSessionAndSignOut();
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
                    }
                }
                model.Section = "3";
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_EducationProfessional.cshtml", model);
            }
            else if (section == "4")
            {
                model.Section = "4";
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.DesignationList = profileHelpValues.functionalarea.Select(x => new SelectListItem
                        {
                            Text = x.functionalareadescription,
                            Value = x.functionalarea
                        }).ToList();
                        model.CountryList = profileHelpValues.countrylist.Select(x => new SelectListItem
                        {
                            Text = x.value,
                            Value = x.key
                        }).ToList();
                    }
                }
                model.CandidateWorkExperience = GetWorkExpDetails(model);
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_WorkExperience.cshtml", model);
            }
            else if (section == "5")
            {
                model = GetCandidateAttachements();
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.AttachementTypes = profileHelpValues.attachmentlist.Select(x => new SelectListItem
                        {
                            Text = x.attachmenttypedescription,
                            Value = x.attachmenttype
                        }).ToList();
                    }
                }
                model.Section = "5";
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_AttachmentsInfo.cshtml", model);
            }
            else if (section == "6")
            {
                model.Section = "6";
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        if (profileHelpValues.interestgroups != null)
                        {
                            model.InterestGroupsList = profileHelpValues.interestgroups.Select(x => new SelectListItem
                            {
                                Text = x.interestgroupdescription,
                                Value = x.interestgroup
                            }).ToList();
                        }
                        if (profileHelpValues.functionalarea != null)
                        {
                            model.DesignationList = profileHelpValues.functionalarea.Select(x => new SelectListItem
                            {
                                Text = x.functionalareadescription,
                                Value = x.functionalarea
                            }).ToList();
                        }
                        if (profileHelpValues.hierarachylevel != null)
                        {
                            model.HierarachyLevelList = profileHelpValues.hierarachylevel.Select(x => new SelectListItem
                            {
                                Text = x.hierarchyleveldescription,
                                Value = x.hierarchylevel
                            }).ToList();
                        }
                    }
                }
                var request = new preferenceRequest
                {
                    updatemode = "R",
                };
                var response = JobSeekerClient.GetCandidatePreferenceUpdate(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    preferenceDetails[] preferenceDetails = response.Payload.preferencedetails;
                    if (preferenceDetails != null && preferenceDetails.Count() > 0)
                    {
                        model.HierarachyLevel = preferenceDetails[0].hierarchylevel;
                        model.FunctionalAreas = preferenceDetails[0].functionalarea;
                        model.sequenceNo = preferenceDetails[0].sequencenumber;
                        model.objectid = preferenceDetails[0].objectid;
                        model.objecttype = preferenceDetails[0].objecttype;
                        model.planversion = preferenceDetails[0].planversion;
                    }
                    model.InterestGroup = response.Payload.interestgroup;
                }
                else
                {
                    model.strMessage = response.Message;
                    ModelState.AddModelError(string.Empty, response.Message);
                    if (response.Payload != null && response.Payload.errorcode == "10510")
                    {
                        model.errorCode = response.Payload.errorcode;
                        ClearSessionAndSignOut();
                    }
                }
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_Preferences.cshtml", model);
            }
            else if (section == "7")
            {
                model.Section = "7";
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_GeneralQuestionnaire.cshtml", model);
            }
            else if (section == "8")
            {
                model.Section = "8";
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        if (profileHelpValues.applicationlist != null)
                        {
                            //model.ApplicationSourceTypeList = profileHelpValues.applicationlist.Select(x => new SelectListItem
                            //{
                            //    Text = x.sourcetype,
                            //    Value = x.sourcetypecode
                            //}).ToList();

                            model.ApplicationSourceTypeList = profileHelpValues.applicationlist
                                                   .GroupBy(c => new { c.sourcetype, c.sourcetypecode }).ToList()
                                                   .Select(x => new applicationSource
                                                   {
                                                       sourcetype = x.FirstOrDefault().sourcetype,
                                                       sourcetypecode = x.FirstOrDefault().sourcetypecode
                                                   }).ToList();
                        }

                        if (profileHelpValues.applicationlist != null)
                        {
                            //model.ApplicationSourceList = profileHelpValues.applicationlist.Select(x => new SelectListItem
                            //{
                            //    Text = x.sourcetext,
                            //    Value = x.sourcecode
                            //}).ToList();

                            model.ApplicationSourceList = profileHelpValues.applicationlist.ToList();
                        }
                    }
                }
                var request = new coverLetterUpdateRequest
                {
                    updatemode = "R",
                };
                var response = JobSeekerClient.GetCandidateCoverLetterUpdate(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null)
                {
                    applSource[] applSourceDetails = response.Payload.applicationsourcelist;
                    if (applSourceDetails.Count() > 0)
                    {
                        model.ApplicationSource = applSourceDetails[0].applicationsource;
                        model.FunctionalAreas = applSourceDetails[0].applicationsourcetype;
                    }
                }
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_CoverLetter.cshtml", model);
            }
            return new EmptyResult();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AjaxGetEmiratesIdDetails(string emiratesId)
        {
            var model = new CandidateProfile();
            var response = JobSeekerClient.GetEmiratesIdDetails(CurrentPrincipal.UserId, emiratesId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null)
            {
                DateTime dateResult = getCultureDate(response.Payload.birthdate);
                model.DOB = (dateResult != null && dateResult.Ticks > 0) ? dateResult.ToString("dd MMM yyyy") : string.Empty;
                model.Gender = response.Payload.gender;
                model.MobileNo = response.Payload.mobile;
                model.EmiratesID = emiratesId;
                CultureInfo culture;
                culture = SitecoreX.Culture;
                if (culture.ToString().Equals("ar-AE"))
                    model.FirstName = response.Payload.name_in_arabic;
                else
                    model.FirstName = response.Payload.name_in_enlish;
                model.Nationality = response.Payload.nationality;
                model.PassportNo = response.Payload.passport;
                model.Emirates = response.Payload.region;
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                model.strMessage = response.Message;
                model.Success = false;
                if (response.Payload != null && response.Payload.errorcode == "10510")
                {
                    ClearSessionAndSignOut();
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AjaxSaveEducationDetails(string educationData, string IsDelete)
        {
            CandidateProfile model = new CandidateProfile();
            var educationlist = CustomJsonConvertor.DeserializeObject<List<Educations>>(educationData);
            profileHelpValues profileHelpValues = null;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
            {
                if (profileHelpValues != null)
                {
                    model.EducationLevel = profileHelpValues.educationlevellist.Select(x => new SelectListItem
                    {
                        Text = x.nameofdegree,
                        Value = x.degreelevel
                    }).ToList();
                    model.FieldOfStudy = profileHelpValues.educationfieldlist.Select(x => new SelectListItem
                    {
                        Text = x.value,
                        Value = x.key
                    }).ToList();
                }
            }
            if (educationlist.Count() > 0)
            {
                educationRequest updateRequest = null;
                if (IsDelete.ToLower() == "true")
                {
                    updateRequest = new educationRequest
                    {
                        updatemode = "D",
                        objectid = educationlist[0].objectid,
                        objectversion = educationlist[0].objectversion,
                        planversion = educationlist[0].planversion,
                        sequencenumber = educationlist[0].sequenceNo,
                    };
                }
                else
                {
                    DateTime dateStartResult = getCultureDate(educationlist[0].strStartDate);
                    DateTime dateEndResult = getCultureDate(educationlist[0].strEndDate);
                    updateRequest = new educationRequest
                    {
                        updatemode = educationlist[0].sequenceNo != null ? "U" : "I",
                        //updatemode = "U",
                        objectid = educationlist[0].objectid,
                        objectversion = educationlist[0].objectversion,
                        planversion = educationlist[0].planversion,
                        educationlevel = educationlist[0].EducationLevel,
                        universityname = educationlist[0].UniversityName,
                        fieldofstudy = educationlist[0].FieldOfStudy,
                        startdate = dateStartResult != null ? dateStartResult.ToString("yyyyMMdd") : string.Empty,
                        enddate = dateEndResult != null ? dateEndResult.ToString("yyyyMMdd") : string.Empty,
                        sequencenumber = educationlist[0].sequenceNo,
                    };
                }
                var response = JobSeekerClient.GetCandidateEducation(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
                {
                    //model.EducationLevel = DeserializedModel.EducationLevel;
                    //model.FieldOfStudy = DeserializedModel.FieldOfStudy;
                    if (response.Payload.workexp != null)
                    {
                        model.CandidateEducation = response.Payload.workexp.Select(x => new Educations
                        {
                            objectid = x.objectid,
                            objectversion = x.objectversion,
                            planversion = x.planversion,
                            EducationLevel = model.EducationLevel.Where(t => t.Value.ToLower() == x.educationlevel.ToLower()).Select(t => t.Text).FirstOrDefault(),//getEducationtext(x.educationlevel, model),
                            UniversityName = x.universityname,
                            FieldOfStudy = model.FieldOfStudy.Where(t => t.Value.ToLower() == x.fieldofstudy.ToLower()).Select(t => t.Text).FirstOrDefault(),//getStudytext(x.fieldofstudy, model),
                            strStartDate = x.startdate != null ? getCultureDate(x.startdate).ToString("dd MMM yyyy") : string.Empty,//x.startdate,
                            strEndDate = x.enddate != null ? getCultureDate(x.enddate).ToString("dd MMM yyyy") : string.Empty,
                            sequenceNo = x.sequencenumber
                        }).ToList();
                    }
                    model.Success = true;
                    model.Section = "3";
                    model.strMessage = response.Message;
                }
                else
                {
                    model.Success = false;
                    model.strMessage = response.Message;
                    model.Section = "3";
                    ModelState.AddModelError(string.Empty, response.Message);
                    if (response.Payload != null && response.Payload.errorcode == "10510")
                    {
                        model.errorCode = response.Payload.errorcode;
                        ClearSessionAndSignOut();
                    }
                }
            }

            bool IsSubmit;
            model.ProfileProgress = ProfileProgress(out IsSubmit);
            model.IsSubmit = IsSubmit;

            return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_EducationList.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AjaxSaveQualification(string qualificationData, string IsDelete, string IsCert)
        {
            CandidateProfile model = new CandidateProfile();
            profileHelpValues profileHelpValues = null;
            List<qualificationDetails> qualificationDetails = new List<qualificationDetails>();
            var qualificationlist = CustomJsonConvertor.DeserializeObject<List<Qualification>>(qualificationData);
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
            {
                if (profileHelpValues != null)
                {
                    model.QualificationList = profileHelpValues.qualificationlist.Select(x => new SelectListItem
                    {
                        Text = x.qualificationdescription,
                        Value = x.qualificationid
                    }).ToList();

                    model.QualificationGroup = profileHelpValues.qualificationlist.Select(x => new { x.qualificationgrouptext, x.qualificationgroup }).Distinct().ToList().Select(o => new SelectListItem
                    {
                        Text = o.qualificationgrouptext,
                        Value = o.qualificationgroup
                    }).ToList();
                }
            }

            if (qualificationlist.Count > 0)
            {
                qualificationRequest qualificationRequest = null;
                if (IsDelete.ToLower() == "true")
                {
                    qualificationRequest = new qualificationRequest
                    {
                        updatemode = "D",
                        objectid = qualificationlist[0].objectid,
                        objectversion = qualificationlist[0].objectversion,
                        planversion = qualificationlist[0].planversion,
                        sequencenumber = qualificationlist[0].sequenceNo,
                    };
                }
                else
                {
                    qualificationRequest = new qualificationRequest
                    {
                        updatemode = qualificationlist[0].sequenceNo != null ? "U" : "I",
                        objectid = qualificationlist[0].objectid,
                        objectversion = qualificationlist[0].objectversion,
                        planversion = qualificationlist[0].planversion,
                        sequencenumber = qualificationlist[0].sequenceNo,
                        qualificationobjecttype = "Q",
                        qualificationtext = qualificationlist[0].QualificationName,
                        proficiency = qualificationlist[0].ProficiencyId,
                        proficiencytext = qualificationlist[0].ProficiencyName,
                        qualificationgrouptext = qualificationlist[0].QualificationGrp,
                        qualificationobjectid = qualificationlist[0].QualificationId,
                        additionalqualificationtext = qualificationlist[0].IssuingOrganization
                    };
                }
                var response = JobSeekerClient.GetCandidateQualification(qualificationRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
                {
                    if (response.Payload.qualificationdetails != null)
                    {
                        model.CandidateQualification = response.Payload.qualificationdetails.Select(x => new Qualification
                        {
                            objectid = x.objectid,
                            objectversion = x.objectversion,
                            planversion = x.planversion,
                            sequenceNo = x.sequencenumber,
                            QualificationGrp = x.qualificationgrouptext,
                            QualificationGrpId = model.QualificationGroup.Where(t => t.Text.ToLower() == x.qualificationgrouptext.ToLower()).Select(t => t.Value).FirstOrDefault(),
                            QualificationId = x.qualificationobjecttype,
                            QualificationName = x.qualificationtext,
                            ProficiencyName = x.proficiencytext,
                            ProficiencyId = x.proficiency,
                            IssuingOrganization = x.additionalqualificationtext
                        }).ToList();
                        model.CandidateCertification = model.CandidateQualification.Where(w => w.QualificationGrpId == CareerPortalHelper.CertificatesId).Select(x => new Qualification
                        {
                            objectid = x.objectid,
                            objectversion = x.objectversion,
                            planversion = x.planversion,
                            sequenceNo = x.sequenceNo,
                            QualificationGrp = x.QualificationGrp,
                            QualificationGrpId = model.QualificationGroup.Where(t => t.Text.ToLower() == x.QualificationGrp.ToLower()).Select(t => t.Value).FirstOrDefault(),
                            QualificationId = x.QualificationId,
                            QualificationName = x.QualificationName,
                            ProficiencyName = x.ProficiencyName,
                            ProficiencyId = x.ProficiencyId,
                            IssuingOrganization = x.IssuingOrganization
                        }).ToList();
                        model.CandidateQualification = model.CandidateQualification.Where(w => w.QualificationGrpId != CareerPortalHelper.CertificatesId).ToList();
                    }
                    model.Section = "3";
                    model.strMessage = response.Message;
                }
                else
                {
                    model.Section = "3";
                    model.Success = false;
                    model.strMessage = response.Message;
                    ModelState.AddModelError(string.Empty, response.Message);
                    if (response.Payload != null && response.Payload.errorcode == "10510")
                    {
                        model.errorCode = response.Payload.errorcode;
                        ClearSessionAndSignOut();
                    }
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }

            bool IsSubmit;
            model.ProfileProgress = ProfileProgress(out IsSubmit);
            model.IsSubmit = IsSubmit;

            if (IsCert.ToLower() == "true")
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_CertificationList.cshtml", model);
            else
                return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_QualificationList.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AjaxGetQualifications(string groupId)
        {
            profileHelpValues profileHelpValues = null;
            List<SelectListItem> qualifications = new List<SelectListItem>();
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
            {
                qualifications = profileHelpValues.qualificationlist.Where(w => w.qualificationgroup == groupId).Select(x => new SelectListItem
                {
                    Text = x.qualificationdescription,
                    Value = x.qualificationid
                }).ToList();
            }

            return Json(qualifications, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AjaxSaveWorkExpDetails(string workExpData, string IsDelete)
        {
            var workExplist = CustomJsonConvertor.DeserializeObject<List<WorkExperience>>(workExpData);
            CandidateProfile model = new CandidateProfile();
            profileHelpValues profileHelpValues = null;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
            {
                if (profileHelpValues != null)
                {
                    model.DesignationList = profileHelpValues.functionalarea.Select(x => new SelectListItem
                    {
                        Text = x.functionalareadescription,
                        Value = x.functionalarea
                    }).ToList();
                    model.JobLocation = profileHelpValues.countrylist.Select(x => new SelectListItem
                    {
                        Text = x.value,
                        Value = x.key
                    }).ToList();

                    model.DesignationList = profileHelpValues.functionalarea.Select(x => new SelectListItem
                    {
                        Text = x.functionalareadescription,
                        Value = x.functionalarea
                    }).ToList();
                    model.CountryList = profileHelpValues.countrylist.Select(x => new SelectListItem
                    {
                        Text = x.value,
                        Value = x.key
                    }).ToList();
                }
            }
            if (workExplist.Count() > 0)
            {
                workEXPRequest updateRequest = null;
                if (IsDelete.ToLower() == "true")
                {
                    updateRequest = new workEXPRequest
                    {
                        updatemode = "D",
                        objectid = workExplist[0].objectid,
                        objecttype = workExplist[0].objectversion,
                        planversion = workExplist[0].planversion,
                        sequencenumber = workExplist[0].sequenceNo,
                    };
                }
                else
                {
                    DateTime dateStartResult = getCultureDate(workExplist[0].StartDate);
                    //DateTime dateEndResult = getCultureDate(workExplist[0].EndDate);zzz
                    if (workExplist[0].CurrentEmployer)
                    {
                        List<WorkExperience> workList = null;
                        if (!CacheProvider.TryGet(CacheKeys.CAREERPORTAL_WORK_EXP_LIST, out workList))
                        {
                            workList = GetWorkExpDetails(model);
                        }

                        if (workList != null)
                        {
                            var currEmpCount = workList.Where(x => x.CurrentEmployer == true).Count();
                            var UpdCurrEmpCount = workList.Where(x => x.CurrentEmployer == true && x.sequenceNo != workExplist[0].sequenceNo).Count();
                            if (currEmpCount > 0 && workExplist[0].sequenceNo == null || UpdCurrEmpCount > 0)
                            {
                                model.Section = "4";
                                model.Success = false;
                                model.strMessage = Translate.Text("already exist current employer");
                                ModelState.AddModelError(string.Empty, model.strMessage);
                                return Json(model, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    updateRequest = new workEXPRequest
                    {
                        updatemode = workExplist[0].sequenceNo != null ? "U" : "I",
                        sequencenumber = workExplist[0].sequenceNo,
                        objectid = workExplist[0].objectid,
                        objecttype = workExplist[0].objectversion,
                        planversion = workExplist[0].planversion,
                        currentemployer = workExplist[0].EmployerName,
                        jobtitletext = workExplist[0].Designation,
                        countryname = workExplist[0].JobLocation,
                        startdate = dateStartResult != null ? dateStartResult.ToString("yyyyMMdd") : string.Empty,
                        enddate = !string.IsNullOrEmpty(workExplist[0].EndDate) ? getCultureDate(workExplist[0].EndDate).ToString("yyyyMMdd") : string.Empty,
                        currentemployerflag = workExplist[0].CurrentEmployer ? "X" : string.Empty
                    };
                }

                var response = JobSeekerClient.GetCandidateWorkExp(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
                {
                    if (response.Payload.workexp != null)
                    {
                        model.CandidateWorkExperience = response.Payload.workexp.Select(x => new WorkExperience
                        {
                            EmployerName = x.currentemployer,
                            Designation = x.jobtitletext,//model.DesignationList.Where(t => t.Value.ToLower() == x.jobtitle.ToLower()).Select(t => t.Text).FirstOrDefault(), //getJobTitletext(x.jobtitle, DeserializedModel),
                            JobLocation = model.JobLocation.Where(t => t.Value.ToLower() == x.countryname.ToLower()).Select(t => t.Text).FirstOrDefault(),//getCountrytext(x.countryname, DeserializedModel),
                            StartDate = x.startdate != null ? getCultureDate(x.startdate).ToString("dd MMM yyyy") : string.Empty,
                            EndDate = x.enddate != null ? getCultureDate(x.enddate).ToString("dd MMM yyyy") : string.Empty,
                            CurrentEmployer = !string.IsNullOrEmpty(x.currentemployerflag) && x.currentemployerflag == "X" ? true : false,
                            objectid = x.objectid,
                            objectversion = x.objecttype,
                            planversion = x.planversion,
                            sequenceNo = x.sequencenumber
                        }).ToList();

                        CacheProvider.Store(CacheKeys.CAREERPORTAL_WORK_EXP_LIST, new AccessCountingCacheItem<List<WorkExperience>>(model.CandidateWorkExperience, Times.Once));
                    }
                    else
                    {
                        CacheProvider.Remove(CacheKeys.CAREERPORTAL_WORK_EXP_LIST);
                    }

                    model.Section = "4";
                    model.strMessage = response.Message;
                }
                else
                {
                    CacheProvider.Remove(CacheKeys.CAREERPORTAL_WORK_EXP_LIST);

                    model.Section = "4";
                    model.Success = false;
                    model.strMessage = response.Message;
                    ModelState.AddModelError(string.Empty, response.Message);
                    if (response.Payload != null && response.Payload.errorcode == "10510")
                    {
                        model.errorCode = response.Payload.errorcode;
                        ClearSessionAndSignOut();
                    }
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }

            bool IsSubmit;
            model.ProfileProgress = ProfileProgress(out IsSubmit);
            model.IsSubmit = IsSubmit;

            return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_WorkExpList.cshtml", model);
        }

        [HttpPost]
        public ActionResult AjaxUploadFiles(FormCollection attachmentData)
        {
            if (attachmentData != null)
            {
                string attachmentHeader = attachmentData["attachmentHeader"];
                string attachmentType = attachmentData["attachmentType"];
                var updateParams = (attachmentData["updateParameter"] != null && attachmentData["updateParameter"] != "") ? attachmentData["updateParameter"].Split('#') : null;
                profileHelpValues profileHelpValues = null;
                CandidateProfile model = new CandidateProfile();
                if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_HELP_VALUES, out profileHelpValues))
                {
                    if (profileHelpValues != null)
                    {
                        model.AttachementTypes = profileHelpValues.attachmentlist.Select(x => new SelectListItem
                        {
                            Text = x.attachmenttypedescription,
                            Value = x.attachmenttype
                        }).ToList();
                    }
                }
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                                //Use the following properties to get file's name, size and MIMEType
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                    {
                        fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                        attachmentsRequest request = new attachmentsRequest
                        {
                            attachmentheader = attachmentHeader,
                            attachmenttype = attachmentType,
                            attachmenttypetext = fileName,
                            filename = fileName,
                            content = fileData,
                            contenttype = mimeType,
                            updatemode = "I"
                        };
                        if (updateParams != null && updateParams.Count() > 0)
                        {
                            request.updatemode = "U";
                            request.objectid = updateParams[0];
                            request.objecttype = updateParams[1];
                            request.planversion = updateParams[2];
                            request.sequencenumber = updateParams[3];
                            request.attachment = updateParams[4];
                            request.attachmenttype = updateParams[5];
                        }
                        var response = JobSeekerClient.GetCandidateAttachements(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        if (response.Succeeded && response.Payload != null)
                        {
                            if (response.Payload.workexp != null)
                            {
                                model.CandidateAttchments = response.Payload.workexp.Select(x => new UploadedDocuments
                                {
                                    attachmenttype = x.attachmenttype,
                                    attachmentheader = x.attachmentheader,
                                    attachmenttypetext = x.attachmenttypetext,
                                    attachmenturl = x.attachmenturl,
                                    contenttype = x.contenttype,
                                    objectid = x.objectid,
                                    planversion = x.planversion,
                                    sequencenumber = x.sequencenumber,
                                    attachment = x.attachment,
                                    content = x.filecontent,
                                    ContentBase64 = Convert.ToBase64String(x.filecontent ?? new byte[0])
                                }).ToList();
                            }
                            model.Section = "6";
                            if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).ToList().Count > 0)
                            {
                                var picList = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).ToList();
                                foreach (var item in picList)
                                {
                                    model.ProfilePicId = item.objectid + "#" + item.objecttype + "#" + item.planversion + "#" + item.sequencenumber + "#" + item.attachment;
                                    break;
                                }
                                model.ProfilePic = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).First();
                            }
                            model.strMessage = response.Message;

                            bool IsSubmit;
                            model.ProfileProgress = ProfileProgress(out IsSubmit);
                            model.IsSubmit = IsSubmit;

                            return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_AttachmentList.cshtml", model);
                        }
                        else
                        {
                            model.Section = "6";
                            model.strMessage = response.Message;
                            ModelState.AddModelError(string.Empty, response.Message);
                            if (response.Payload != null && response.Payload.errorcode == "10510")
                            {
                                model.errorCode = response.Payload.errorcode;
                                ClearSessionAndSignOut();
                            }
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxDeleteFile(string removeFiles)
        {
            CandidateProfile model = new CandidateProfile();

            if (removeFiles != null)
            {
                var removeFile = removeFiles.Split('#');

                attachmentsRequest request = new attachmentsRequest
                {
                    updatemode = "D",
                    objectid = removeFile[0],
                    objecttype = removeFile[1],
                    planversion = removeFile[2],
                    sequencenumber = removeFile[3],
                    attachment = removeFile[4],
                    attachmenttype = removeFile[5],
                };

                var response = JobSeekerClient.GetCandidateAttachements(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
                {
                    if (response.Payload.workexp != null)
                    {
                        model.CandidateAttchments = response.Payload.workexp.Select(x => new UploadedDocuments
                        {
                            attachmenttype = x.attachmenttype,
                            attachmentheader = x.attachmentheader,
                            attachmenttypetext = x.attachmenttypetext,
                            attachmenturl = x.attachmenturl,
                            contenttype = x.contenttype,
                            objecttype = x.objecttype,
                            objectid = x.objectid,
                            planversion = x.planversion,
                            sequencenumber = x.sequencenumber,
                            attachment = x.attachment,
                            content = x.filecontent,
                            ContentBase64 = Convert.ToBase64String(x.filecontent ?? new byte[0])
                        }).ToList();
                    }
                    model.Section = "6";
                    if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).ToList().Count > 0)
                    {
                        var picList = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).ToList();
                        foreach (var item in picList)
                        {
                            model.ProfilePicId = item.objectid + "#" + item.objecttype + "#" + item.planversion + "#" + item.sequencenumber + "#" + item.attachment;
                            break;
                        }
                        model.ProfilePic = model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).Select(x => x.ContentBase64).First();
                    }
                    model.strMessage = response.Message;

                    bool IsSubmit;
                    model.ProfileProgress = ProfileProgress(out IsSubmit);
                    model.IsSubmit = IsSubmit;

                    return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_AttachmentList.cshtml", model);
                }
                else
                {
                    model.strMessage = response.Message;
                    ModelState.AddModelError(string.Empty, response.Message);
                    if (response.Payload != null && response.Payload.errorcode == "10510")
                    {
                        model.errorCode = response.Payload.errorcode;
                        ClearSessionAndSignOut();
                    }
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_POSTED_SECTION, new AccessCountingCacheItem<string>(model.Section, Times.Once));
                    CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxWithdrawApp(string applicationData)
        {
            Dashboard model = new Dashboard();
            if (applicationData != null)
            {
                var appdata = CustomJsonConvertor.DeserializeObject<List<ApplicationDetail>>(applicationData);
                var request = new myAppDashboardRequest
                {
                    withdrawn = "X",
                    postingguid = appdata[0].PostingGuid,
                    objectid = appdata[0].JobDetails.ObjectId,
                    objecttype = appdata[0].JobDetails.ObjectType,
                    planversion = appdata[0].JobDetails.PlanVersion,
                };
                var responseDashboard = JobSeekerClient.PutCandidateMyAppdashboard(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (responseDashboard.Succeeded && responseDashboard.Payload != null)
                {
                    var applicationDetails = responseDashboard.Payload.jobdetails.Select(x => new ApplicationDetail
                    {
                        ApplicationDate = x.applicationdate,
                        ApplicationStausID = x.applicationstatus,
                        ApplicationStausText = x.applicationstatustext,
                        PostingGuid = x.postingguid,
                        PostingGuidStatus = x.postingguidstatus,
                        JobDetails = x.jobapplicationdetails.Select(j => new JobDetail
                        {
                            JobId = j.objectid,
                            JobTitle = x.postingheader,
                            ObjectId = j.objectid,
                            ObjectType = j.objecttype,
                            PlanVersion = j.planversion
                        }).First()
                    }).ToList();
                    model.applicationDetails = applicationDetails;
                    model.Success = true;
                    model.Message = responseDashboard.Message;
                    return PartialView("~/Views/Feature/HR/CareerPortal/Modules/_ApplicationList.cshtml", model);
                }
                else
                {
                    model.Success = false;
                    model.Message = responseDashboard.Message;
                    ModelState.AddModelError(string.Empty, responseDashboard.Message);
                    if (responseDashboard.Payload != null && responseDashboard.Payload.errorcode == "10510")
                    {
                        model.errorCode = responseDashboard.Payload.errorcode;
                        ClearSessionAndSignOut();
                    }
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Account Unlock

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
            ViewBag.BackLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);

            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out string errormessage))
            {
                ModelState.AddModelError(string.Empty, errormessage);
            }
            if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Step + (unlock ? "unlock" : string.Empty), out string steps) || !string.IsNullOrWhiteSpace(s))
            {
                if (!string.IsNullOrWhiteSpace(s) && s.Equals("1"))
                {
                    steps = s;
                }

                switch (steps)
                {
                    case "1":
                        VerifyEmailandMobileModel selectedModel;
                        if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), out selectedModel))
                        {
                            ViewBag.BackLink = unlock ? LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_PORTAL_ACCOUNT_UNLOCK) : LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_POTAL_FORGOTPASSWORD);
                            return View("~/Views/Feature/HR/CareerPortal/_VerifyEmailandMobile.cshtml", selectedModel);
                        }
                        break;

                    case "2":
                        ForgotPasswordViewModel forgotPasswordViewModel;
                        if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), out forgotPasswordViewModel))
                        {
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                            ViewBag.Email = forgotPasswordViewModel.EmailAddess;
                            ViewBag.subtitleText = forgotPasswordViewModel.SelectedOption.Equals("1") ?
                               string.Format(Translate.Text("Verifyemail.SubTitle"), forgotPasswordViewModel.MaskedEmailAddess) :
                               string.Format(Translate.Text("Verifymobile.SubTitle"), forgotPasswordViewModel.MaskedMobile);
                            ViewBag.BackLink = unlock ? LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_PORTAL_ACCOUNT_UNLOCK) + "?s=1" : LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_POTAL_FORGOTPASSWORD) + "?s=1";

                            ViewBag.ValidityMinutes = Convert.ToInt32(forgotPasswordViewModel.ValidityMinutes);
                            ViewBag.ValiditySeconds = Convert.ToInt32(forgotPasswordViewModel.ValiditySeconds);

                            return View("~/Views/Feature/HR/CareerPortal/_VerifyOTP.cshtml");
                        }
                        break;

                    case "3":
                        //ForgotPasswordViewModel forgotPasswordViewModel;
                        if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), out forgotPasswordViewModel))
                        {
                            if (!unlock)
                            {
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                ViewBag.BackLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_POTAL_FORGOTPASSWORD);
                                return View("~/Views/Feature/HR/CareerPortal/_SetNewPassword.cshtml", new SetNewPasswordV1Model { Username = forgotPasswordViewModel.Username });
                            }
                        }
                        break;

                    case "4":
                        CacheProvider.Remove(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty));
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
                        return View("~/Views/Feature/HR/CareerPortal/_SetNewPasswordSuccess.cshtml");

                    case "5":
                        string username;
                        if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Username, out username))
                        {
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Username, new AccessCountingCacheItem<string>(username, Times.Once));
                            SendOTPMethod(new ForgotPasswordV1Model { Username = username }, true, out bool success);
                            if (success)
                            {
                                VerifyEmailandMobileModel selectedModel1;
                                if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), out selectedModel1))
                                {
                                    return View("~/Views/Feature/HR/CareerPortal/_VerifyEmailandMobile.cshtml", selectedModel1);
                                }
                            }
                        }
                        break;
                }
            }
            if (unlock)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_LOGIN);
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
            return View("~/Views/Feature/HR/CareerPortal/_ForgotPassword.cshtml", new ForgotPasswordV1Model());
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
                        SendOTPMethod(model, unlock, out bool success);
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
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_ACCOUNT_UNLOCK);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_FORGOTPASSWORD);
        }

        private void SendOTPMethod(ForgotPasswordV1Model model, bool unlock, out bool success)
        {
            success = false;
            OTPRequest verifyRequest = new OTPRequest()
            {
                mode = "R",
                userid = model.Username,
                lang = RequestLanguageCode,
                sessionid = string.Empty
                //,
                //reference = model.Username,
                //prtype = "USID",
            };

            DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.OTPResponse> returnData = JobseekerRestClient.VerifyOtp(verifyRequest);
            bool IsSuccessful = returnData != null && returnData.Succeeded && returnData.Payload != null &&
                ((returnData.Payload.emaillist != null && returnData.Payload.emaillist.Count() > 0 && !string.IsNullOrEmpty(returnData.Payload.emaillist.FirstOrDefault().maskedemail)) ||
                (returnData.Payload.mobilelist != null && returnData.Payload.mobilelist.Count() > 0 && !string.IsNullOrEmpty(returnData.Payload.mobilelist.FirstOrDefault().maskedmobile)));
            //(System.Convert.ToInt32(returnData.Payload.emaillist?.Count ?? 0) > 0 || System.Convert.ToInt32(returnData.Payload.mobilelist?.Count ?? 0) > 0);
            if (IsSuccessful)
            {
                VerifyEmailandMobileModel verifyEmailandMobileModel = new VerifyEmailandMobileModel { Username = model.Username };
                ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel
                {
                    //Businesspartnernumber = returnData.Payload.businesspartnernumber,
                    Username = model.Username
                };
                DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.Emaillist email = returnData.Payload.emaillist.FirstOrDefault();
                DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.Mobilelist mobile = returnData.Payload.mobilelist.FirstOrDefault();

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
                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<VerifyEmailandMobileModel>(verifyEmailandMobileModel, Times.Once));
                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<string>("1", Times.Once));
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
                if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), out ForgotPasswordViewModel forgotPasswordViewModel))
                {
                    if (forgotPasswordViewModel != null && model != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) &&
                        !string.IsNullOrWhiteSpace(model.Username) && forgotPasswordViewModel.Username.Equals(model.Username)
                        && !string.IsNullOrWhiteSpace(model.SelectedOption))
                    {
                        OTPRequest verifyRequest = new OTPRequest()
                        {
                            mode = "S",
                            lang = RequestLanguageCode,
                            sessionid = string.Empty,
                            reference = forgotPasswordViewModel.Username,
                            userid = forgotPasswordViewModel.Username,
                            prtype = "USID",
                            email = model.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                            mobile = model.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty
                        };

                        DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.OTPResponse> returnData = JobseekerRestClient.VerifyOtp(verifyRequest);
                        bool IsSuccessful = returnData != null && returnData.Succeeded;
                        if (IsSuccessful)
                        {
                            forgotPasswordViewModel.SelectedOption = model.SelectedOption;
                            forgotPasswordViewModel.ValidityMinutes = returnData.Payload.validityminutes;
                            forgotPasswordViewModel.ValiditySeconds = returnData.Payload.validityseconds;
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_OTP + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                            {
                                EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                Mobile = forgotPasswordViewModel.MaskedMobile,
                                Username = forgotPasswordViewModel.Username,
                                SelectedOption = forgotPasswordViewModel.SelectedOption
                            }, Times.Once));
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<string>("2", Times.Once));
                        }
                        else
                        {
                            if (!unlock)
                            {
                                CacheProvider.Remove(CacheKeys.Jobseeker_ForgotPassword_OTP + (unlock ? "unlock" : string.Empty));
                            }
                            else
                            {
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + (unlock ? "unlock" : string.Empty), new AccessCountingCacheItem<string>("5", Times.Once));
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
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_ACCOUNT_UNLOCK);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_FORGOTPASSWORD);
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
                    if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata, out ForgotPasswordViewModel forgotPasswordViewModel))
                    {
                        if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.SelectedOption))
                        {
                            OTPRequest verifyRequest = new OTPRequest()
                            {
                                mode = "V",
                                lang = RequestLanguageCode,
                                sessionid = string.Empty,
                                reference = forgotPasswordViewModel.Username,
                                prtype = "USID",
                                email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                                mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                                userid = forgotPasswordViewModel.Username,
                                //businesspartner = forgotPasswordViewModel.Businesspartnernumber,
                                otp = otp
                            };

                            DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.OTPResponse> returnData = JobseekerRestClient.VerifyOtp(verifyRequest);
                            bool IsSuccessful = returnData != null && returnData.Payload != null && returnData.Succeeded && ((string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)) ||
                                (!string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)
                                && !returnData.Payload.maxattempts.Equals("X")));
                            if (IsSuccessful)
                            {
                                forgotPasswordViewModel.OTP = otp;
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata, new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step, new AccessCountingCacheItem<string>("3", Times.Once));
                            }
                            else if (returnData != null && !returnData.Succeeded && returnData.Payload != null && !string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)
                                && returnData.Payload.maxattempts.Equals("X"))
                            {
                                CacheProvider.Remove(CacheKeys.Jobseeker_ForgotPassword_OTP);
                                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                            }
                            else
                            {
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_OTP, new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                                {
                                    EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                    Mobile = forgotPasswordViewModel.MaskedMobile,
                                    Username = forgotPasswordViewModel.Username,
                                    SelectedOption = forgotPasswordViewModel.SelectedOption
                                }, Times.Once));

                                //forgotPasswordViewModel.ValidityMinutes = returnData.Payload.@return.validityminutes;
                                forgotPasswordViewModel.ValiditySeconds = returnData.Payload.validityseconds;

                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata, new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step, new AccessCountingCacheItem<string>("2", Times.Once));
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

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_FORGOTPASSWORD);
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
                    if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata + "unlock", out ForgotPasswordViewModel forgotPasswordViewModel))
                    {
                        if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.SelectedOption))
                        {
                            OTPRequest verifyRequest = new OTPRequest()
                            {
                                mode = "V",
                                lang = RequestLanguageCode,
                                sessionid = string.Empty,
                                reference = forgotPasswordViewModel.Username,
                                prtype = "USID",
                                email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                                mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                                userid = forgotPasswordViewModel.Username,
                                //businesspartner = forgotPasswordViewModel.Businesspartnernumber,
                                otp = otp
                            };

                            DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.OTPResponse> returnData = JobseekerRestClient.VerifyOtp(verifyRequest);
                            bool IsSuccessful = returnData != null && returnData.Payload != null && returnData.Succeeded && ((string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)) ||
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

                                DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common.CommonResponse> response = JobseekerRestClient.UnlockAccount(forgotPasswordRequest);
                                if (response != null && response.Succeeded)
                                {
                                    CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + "unlock", new AccessCountingCacheItem<string>("4", Times.Once));
                                }
                                else
                                {
                                    CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_OTP + "unlock", new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                                    {
                                        EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                        Mobile = forgotPasswordViewModel.MaskedMobile,
                                        Username = forgotPasswordViewModel.Username,
                                        SelectedOption = forgotPasswordViewModel.SelectedOption
                                    }, Times.Once));

                                    //forgotPasswordViewModel.ValidityMinutes = returnData.Payload.@return.validityminutes;
                                    forgotPasswordViewModel.ValiditySeconds = returnData.Payload.validityseconds;

                                    CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata + "unlock", new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                    CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + "unlock", new AccessCountingCacheItem<string>("2", Times.Once));
                                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                                }
                            }
                            else if (returnData != null && !returnData.Succeeded && returnData.Payload != null && !string.IsNullOrWhiteSpace(returnData.Payload.maxattempts)
                                && returnData.Payload.maxattempts.Equals("X"))
                            {
                                CacheProvider.Remove(CacheKeys.Jobseeker_ForgotPassword_OTP);
                                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(returnData.Message, Times.Once));
                            }
                            else
                            {
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_OTP + "unlock", new AccessCountingCacheItem<VerifyEmailandMobileModel>(new VerifyEmailandMobileModel
                                {
                                    EmailAddess = forgotPasswordViewModel.MaskedEmailAddess,
                                    Mobile = forgotPasswordViewModel.MaskedMobile,
                                    Username = forgotPasswordViewModel.Username,
                                    SelectedOption = forgotPasswordViewModel.SelectedOption
                                }, Times.Once));

                                //forgotPasswordViewModel.ValidityMinutes = returnData.Payload.@return.validityminutes;
                                forgotPasswordViewModel.ValiditySeconds = returnData.Payload.validityseconds;

                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata + "unlock", new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                                CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step + "unlock", new AccessCountingCacheItem<string>("2", Times.Once));
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
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_ACCOUNT_UNLOCK);
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
                if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata, out ForgotPasswordViewModel forgotPasswordViewModel))
                {
                    if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username) && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.SelectedOption))
                    {
                        ForgotPasswordRequest forgotPasswordRequest = new ForgotPasswordRequest
                        {
                            passwordinput = new passwordinput()
                            {
                                //mode = "V",
                                lang = RequestLanguageCode,
                                sessionid = string.Empty,
                                reference = forgotPasswordViewModel.Username,
                                prtype = "USID",
                                email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                                mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty,
                                // businesspartner = forgotPasswordViewModel.Businesspartnernumber,
                                otp = forgotPasswordViewModel.OTP,
                                userid = forgotPasswordViewModel.Username,
                                password = Base64Encode(model.Password),
                                confirmpassword = Base64Encode(model.ConfirmPassword)
                            }
                        };

                        DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common.CommonResponse> returnData = JobseekerRestClient.ForgotUseridPwd(forgotPasswordRequest);
                        bool IsSuccessful = returnData != null && returnData.Succeeded;
                        if (IsSuccessful)
                        {
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step, new AccessCountingCacheItem<string>("4", Times.Once));
                        }
                        else
                        {
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata, new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                            CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Step, new AccessCountingCacheItem<string>("3", Times.Once));
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
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_FORGOTPASSWORD);
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
                if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata + "unlock", out ForgotPasswordViewModel forgotPasswordViewModel))
                {
                    CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata + "unlock", new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                }
                else if (CacheProvider.TryGet(CacheKeys.Jobseeker_ForgotPassword_Saveddata, out forgotPasswordViewModel))
                {
                    CacheProvider.Store(CacheKeys.Jobseeker_ForgotPassword_Saveddata, new AccessCountingCacheItem<ForgotPasswordViewModel>(forgotPasswordViewModel, Times.Once));
                }
                if (forgotPasswordViewModel != null && !string.IsNullOrWhiteSpace(forgotPasswordViewModel.Username))
                {
                    OTPRequest verifyRequest = new OTPRequest()
                    {
                        mode = "S",
                        lang = RequestLanguageCode,
                        sessionid = string.Empty,
                        reference = forgotPasswordViewModel.Username,
                        userid = forgotPasswordViewModel.Username,
                        prtype = "USID",
                        email = forgotPasswordViewModel.SelectedOption.Equals("1") ? forgotPasswordViewModel.EmailAddess : string.Empty,
                        mobile = forgotPasswordViewModel.SelectedOption.Equals("2") ? forgotPasswordViewModel.Mobile : string.Empty
                    };

                    DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.OTPResponse> returnData = JobseekerRestClient.VerifyOtp(verifyRequest);
                    bool IsSuccessful = returnData != null && returnData.Succeeded;
                    if (IsSuccessful)
                    {
                        forgotPasswordViewModel.ValidityMinutes = returnData.Payload.validityminutes;
                        forgotPasswordViewModel.ValiditySeconds = returnData.Payload.validityseconds;
                        return Json(new { status = true, validitySeconds = Convert.ToInt32(forgotPasswordViewModel.ValiditySeconds) }, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult ForgotUserName()
        {
            return PartialView("~/Views/Feature/HR/CareerPortal/_ForgotUserName.cshtml", new ForgotUsernameV1Model());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotUserName(ForgotUsernameV1Model model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ForgotUserNameRequest forgotUserNameRequest = new ForgotUserNameRequest
                    {
                        emailid = model.Email,
                        emiratesid = model.SelectedOption == "2" ? model.EmiratesID : "",
                        passportnumber = model.SelectedOption == "1" ? model.PassportNo : "",
                        lang = RequestLanguageCode
                    };
                    DEWAXP.Foundation.Integration.Responses.ServiceResponse<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword.ForgotUserNameResponse> returnData = JobseekerRestClient.ForgotUserName(forgotUserNameRequest);
                    var recoveryEmailModel = new RecoveryEmailSentModel
                    {
                        EmailAddress = model.Email,
                        Context = RecoveryContext.CandidateProfileUsername
                    };

                    CacheProvider.Store(CacheKeys.RECOVERY_EMAIL_STATE, new CacheItem<RecoveryEmailSentModel>(recoveryEmailModel));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_POTAL_RECOVERY_EMAIL_SENT);
                    //bool IsSuccessful = returnData != null && returnData.Succeeded;
                    //if (IsSuccessful)
                    //{
                    //}
                    //else if (!returnData.Succeeded && returnData.Payload != null && (returnData.Payload.errorcode.Equals("10214")))
                    //{
                    //    ModelState.AddModelError(string.Empty, string.Format(Translate.Text("Forgot_Username_EmiratesID_ErrorMessage"), DEWAXP.Foundation.Helpers.LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_POTAL_CANDIDATEREGISTRATION)));
                    //}
                    //else if (!returnData.Succeeded && returnData.Payload != null && (returnData.Payload.errorcode.Equals("10213")))
                    //{
                    //    ModelState.AddModelError(string.Empty, string.Format(Translate.Text("Forgot_Username_Passport_ErrorMessage"), DEWAXP.Foundation.Helpers.LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CAREER_POTAL_CANDIDATEREGISTRATION)));
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError(string.Empty, returnData.Message);
                    //}
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            ViewBag.Modal = ContentRepository.GetItem<ModalOverlay>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{6DFBF80F-1B61-4332-9E35-EB5860434E6B}")));

            return PartialView("~/Views/Feature/HR/CareerPortal/_ForgotUserName.cshtml", model);
        }

        #endregion Account Unlock

        #region DownloadProfile

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadProfile()
        {
            try
            {
                // Bid Item Download
                byte[] downloadFile = new byte[0];
                string fileName = "CareerProfile_{0}.pdf";
                string fileMimeType = "application/pdf";
                var response = JobSeekerClient.GetCandidateProfileDownload(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.@return.pdffile != null)
                {
                    downloadFile = response.Payload.@return.pdffile;
                    string _fileName = getClearstri(string.Format(fileName, Guid.NewGuid().ToString()));
                    return File(downloadFile, fileMimeType, _fileName);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_PORTAL_PROFILE);
        }

        #endregion DownloadProfile

        #endregion Actions

        #region Methods

        private CandidateProfile GetPersoanlInfoAddress()
        {
            CandidateProfile model = new CandidateProfile();
            var updateRequest = new profileUpdateRequest
            {
                updatemode = "R",
            };
            var responseProfile = JobSeekerClient.GetCandidateProfile(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            //model.NationalityList = FormExtensions.GetNationalities(null, false);
            additionaldataResponse[] additionaldatalist = null;
            personalDataAddressRequest[] personaldatalist = null;
            if (responseProfile.Succeeded && responseProfile.Payload != null && responseProfile.Payload.errorcode != "10555")
            {
                personaldatalist = responseProfile.Payload.personaldatalist;
                additionaldatalist = responseProfile.Payload.additionaldatareslist;
                DateTime dateResult = getCultureDate(responseProfile.Payload.birthdate);
                model.FirstName = responseProfile.Payload.firstname;
                model.LastName = responseProfile.Payload.lastname;
                model.Gender = responseProfile.Payload.gender;
                model.MaritalStatus = responseProfile.Payload.maritalstatus;
                model.DOB = dateResult != null ? dateResult.ToString("dd MMM yyyy") : string.Empty;
                model.Nationality = responseProfile.Payload.nation;
                model.MobileNo = responseProfile.Payload.mobile;
                model.LastLogin = CurrentPrincipal.LastLogin;

                bool IsSubmit;
                model.ProfileProgress = ProfileProgress(out IsSubmit);
                model.IsSubmit = IsSubmit;

                if (personaldatalist != null && personaldatalist.Count() > 0)
                {
                    model.Emirates = personaldatalist[0].region;
                    model.permantAddress = personaldatalist[0].street;
                    model.Country = personaldatalist[0].country;
                    model.City = personaldatalist[0].city;
                    model.PostalCode = personaldatalist[0].postalcode;
                }
                if (additionaldatalist != null && additionaldatalist.Count() > 0)
                {
                    model.Religion = additionaldatalist[0].religion;
                    model.PassportNo = additionaldatalist[0].passportnumber;
                    model.IsUAEResident = additionaldatalist[0].UAEresident;
                    model.EmiratesID = additionaldatalist[0].emiratesID;
                    model.DisabilityId = additionaldatalist[0].disabilityID;
                    model.YearsOfExperience = additionaldatalist[0].experience;
                    model.HighestQualificationLevel = additionaldatalist[0].highestqualificationlevel;
                    //model.RegistrationType = additionaldatalist[0].registrationtype;
                    model.PeopleOfDetermination = additionaldatalist[0].peopleofdetermination;
                    model.NatureOfWorkType = additionaldatalist[0].natureofwork;
                    model.ExperienceType = additionaldatalist[0].experiencetype;
                }
                CacheProvider.Store(CacheKeys.CAREERPORTAL_REGISTRATION_TYPE, new CacheItem<string>(model.ExperienceType, TimeSpan.FromMinutes(60)));
                CacheProvider.Store(CacheKeys.CAREERPORTAL_PERSONAL_INFO, new CacheItem<profileUpdate>(responseProfile.Payload, TimeSpan.FromMinutes(60)));
            }
            else
            {
                ModelState.AddModelError(string.Empty, responseProfile.Message);
                model.Success = false;
                model.strMessage = responseProfile.Message;
                if (responseProfile.Payload.errorcode == "10510")
                {
                    model.errorCode = responseProfile.Payload.errorcode;
                    model.IsSessionCheck = false;
                    ClearSessionAndSignOut();
                }
            }
            return model;
        }

        private List<Educations> GetEducationDetails(CandidateProfile model)
        {
            CandidateProfile newModel = new CandidateProfile();
            var updateRequest = new educationRequest
            {
                updatemode = "R"
            };
            educationDetails[] educationDetails = null;
            var response = JobSeekerClient.GetCandidateEducation(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
            {
                educationDetails = response.Payload.workexp;
                if (response.Payload.workexp != null)
                {
                    newModel.CandidateEducation = response.Payload.workexp.Select(x => new Educations
                    {
                        objectid = x.objectid,
                        objectversion = x.objectversion,
                        planversion = x.planversion,
                        EducationLevel = model.EducationLevel.Where(t => t.Value.ToLower() == x.educationlevel.ToLower()).Select(t => t.Text).FirstOrDefault(),//getEducationtext(x.educationlevel, model),
                        UniversityName = x.universityname,
                        FieldOfStudy = model.FieldOfStudy.Where(t => t.Value.ToLower() == x.fieldofstudy.ToLower()).Select(t => t.Text).FirstOrDefault(),
                        strStartDate = x.startdate != null ? getCultureDate(x.startdate).ToString("dd MMM yyyy") : string.Empty,//x.startdate,
                        strEndDate = x.enddate != null ? getCultureDate(x.enddate).ToString("dd MMM yyyy") : string.Empty,
                        sequenceNo = x.sequencenumber
                    }).ToList();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                model.Success = false;
                model.strMessage = response.Message;
                if (response.Payload != null && response.Payload.errorcode == "10510")
                {
                    model.errorCode = response.Payload.errorcode;
                    ClearSessionAndSignOut();
                }
            }
            return newModel.CandidateEducation;
        }

        private List<Qualification> GetQualificationDetails(CandidateProfile model)
        {
            CandidateProfile newModel = new CandidateProfile();
            var updateRequest = new qualificationRequest
            {
                updatemode = "R"
            };
            var response = JobSeekerClient.GetCandidateQualification(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
            {
                if (response.Payload.qualificationdetails != null)
                {
                    newModel.CandidateQualification = response.Payload.qualificationdetails.Select(x => new Qualification
                    {
                        objectid = x.objectid,
                        objectversion = x.objectversion,
                        planversion = x.planversion,
                        sequenceNo = x.sequencenumber,
                        QualificationGrp = x.qualificationgrouptext,
                        QualificationGrpId = model.QualificationGroup.Where(t => t.Text.ToLower() == x.qualificationgrouptext.ToLower()).Select(t => t.Value).FirstOrDefault(),
                        QualificationId = x.qualificationobjecttype,
                        QualificationName = x.qualificationtext,
                        ProficiencyName = x.proficiencytext,
                        ProficiencyId = x.proficiency,
                        IssuingOrganization = x.additionalqualificationtext
                    }).ToList();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                model.Success = false;
                model.strMessage = response.Message;
                if (response.Payload != null && response.Payload.errorcode == "10510")
                {
                    model.errorCode = response.Payload.errorcode;
                    ClearSessionAndSignOut();
                }
            }
            return newModel.CandidateQualification;
        }

        private List<WorkExperience> GetWorkExpDetails(CandidateProfile model)
        {
            var updateRequest = new workEXPRequest
            {
                updatemode = "R"
            };

            workEXPDetails[] workExpDetails = null;
            var response = JobSeekerClient.GetCandidateWorkExp(updateRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
            {
                workExpDetails = response.Payload.workexp;
                if (response.Payload.workexp != null)
                {
                    model.CandidateWorkExperience = response.Payload.workexp.Select(x => new WorkExperience
                    {
                        EmployerName = x.currentemployer,
                        Designation = x.jobtitletext, //model.DesignationList.Where(t => t.Value.ToLower() == x.jobtitle.ToLower()).Select(t => t.Text).FirstOrDefault(), //getJobTitletext(x.jobtitle, DeserializedModel),
                        JobLocation = model.CountryList.Where(t => t.Value.ToLower() == x.countryname.ToLower()).Select(t => t.Text).FirstOrDefault(),
                        StartDate = x.startdate != null ? getCultureDate(x.startdate).ToString("dd MMM yyyy") : string.Empty,//x.startdate,
                        EndDate = x.enddate != null ? getCultureDate(x.enddate).ToString("dd MMM yyyy") : string.Empty,
                        CurrentEmployer = !string.IsNullOrEmpty(x.currentemployerflag) && x.currentemployerflag == "X" ? true : false,
                        objectid = x.objectid,
                        objectversion = x.objecttype,
                        planversion = x.planversion,
                        sequenceNo = x.sequencenumber
                    }).ToList();

                    CacheProvider.Store(CacheKeys.CAREERPORTAL_WORK_EXP_LIST, new AccessCountingCacheItem<List<WorkExperience>>(model.CandidateWorkExperience, Times.Once));
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                model.Success = false;
                model.strMessage = response.Message;
                if (response.Payload != null && response.Payload.errorcode == "10510")
                {
                    model.errorCode = response.Payload.errorcode;
                    ClearSessionAndSignOut();
                }
            }
            return model.CandidateWorkExperience;
        }

        private CandidateProfile GetCandidateAttachements()
        {
            CandidateProfile model = new CandidateProfile();
            var attachementRequest = new attachmentsRequest
            {
                updatemode = "R"
            };
            var response = JobSeekerClient.GetCandidateAttachements(attachementRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null && response.Payload.errorcode != "10555")
            {
                if (response.Payload.workexp != null)
                {
                    model.CandidateAttchments = response.Payload.workexp.Select(x => new UploadedDocuments
                    {
                        attachmenttype = x.attachmenttype,
                        attachmentheader = x.attachmentheader,
                        attachmenttypetext = x.attachmenttypetext,
                        attachmenturl = x.attachmenturl,
                        contenttype = x.contenttype,
                        objecttype = x.objecttype,
                        objectid = x.objectid,
                        planversion = x.planversion,
                        sequencenumber = x.sequencenumber,
                        attachment = x.attachment,
                        content = x.filecontent,
                        ContentBase64 = Convert.ToBase64String(x.filecontent ?? new byte[0])
                    }).ToList();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                model.Success = false;
                model.strMessage = response.Message;
                if (response.Payload != null && response.Payload.errorcode == "10510")
                {
                    model.errorCode = response.Payload.errorcode;
                    ClearSessionAndSignOut();
                }
            }
            return model;
        }

        private ProfielRelease GetReviewSubmit()
        {
            ProfielRelease model = new ProfielRelease();
            var response = JobSeekerClient.GetCandidateProfileRelease("R", string.Empty, string.Empty, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null)
            {
                model.PrivacyAccepted = response.Payload.privacyaccepted;
                model.PrivacyUrl = response.Payload.privacyurl;
                model.ProfileStatus = response.Payload.profilestatus;
                model.ProfileStatustext = response.Payload.profilestatustext;
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
                model.Success = false;
                model.strMessage = response.Message;
                if (response.Payload != null && response.Payload.errorcode == "10510")
                {
                    model.errorCode = response.Payload.errorcode;
                    ClearSessionAndSignOut();
                }
            }
            return model;
        }

        private DateTime getCultureDate(string strDate)
        {
            CultureInfo culture;
            DateTimeStyles styles;

            culture = SitecoreX.Culture;
            if (culture.ToString().Equals("ar-AE"))
            {
                strDate = strDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }
            styles = DateTimeStyles.None;
            DateTime dateResult;
            DateTime.TryParse(strDate, culture, styles, out dateResult);
            return dateResult;
        }

        private bool IsUAE_Redisdent()
        {
            profileUpdate personalInfo = null;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_PERSONAL_INFO, out personalInfo))
            {
                var _addDetail = personalInfo.additionaldatareslist.FirstOrDefault();
                if (_addDetail != null)
                {
                    return _addDetail.UAEresident == "1" || personalInfo.nation == "AE";
                }
            }

            return false;
        }

        private bool CheckAttachment()
        {
            CandidateProfile model = new CandidateProfile();
            bool isAttachment = true;
            model = GetCandidateAttachements();
            //if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CandidateAttachmentType).ToList().Count == 0)
            //{
            //    isAttachment = false;
            //    CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(Translate.Text("upload photo"), Times.Once));
            //}
            if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.HighestEducationalAttachmentType).ToList().Count == 0)
            {
                isAttachment = false;
                CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(Translate.Text("upload highest educational"), Times.Once));
            }
            if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.CVAttachmentType).ToList().Count == 0)
            {
                isAttachment = false;
                CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(Translate.Text("upload cv resume"), Times.Once));
            }
            if (model.CandidateAttchments == null)
            {
                isAttachment = false;
                CacheProvider.Store(CacheKeys.CAREERPORTAL_ListErrorMessage, new AccessCountingCacheItem<List<string>>(new List<string> { Translate.Text("upload cv resume"), Translate.Text("upload highest educational") }.ToList(), Times.Once));
            }
            profileUpdate personalInfo = null;
            additionaldataResponse[] additionaldatalist = null;
            personalDataAddressRequest[] personaldatalist = null;
            if (CacheProvider.TryGet(CacheKeys.CAREERPORTAL_PERSONAL_INFO, out personalInfo))
            {
                if (personalInfo != null)
                {
                    personaldatalist = personalInfo.personaldatalist;
                    additionaldatalist = personalInfo.additionaldatareslist;
                    model.FirstName = personalInfo.firstname;
                    model.LastName = personalInfo.lastname;
                    model.Nationality = personalInfo.nation;

                    if (personaldatalist != null && personaldatalist.Count() > 0)
                    {
                        model.Emirates = personaldatalist[0].region;
                        model.permantAddress = personaldatalist[0].street;
                        model.Country = personaldatalist[0].country;
                        model.City = personaldatalist[0].city;
                        model.PostalCode = personaldatalist[0].postalcode;
                    }
                    if (additionaldatalist != null && additionaldatalist.Count() > 0)
                    {
                        model.Religion = additionaldatalist[0].religion;
                        model.PassportNo = additionaldatalist[0].passportnumber;
                        model.IsUAEResident = additionaldatalist[0].UAEresident;
                        model.EmiratesID = additionaldatalist[0].emiratesID;
                        model.DisabilityId = additionaldatalist[0].disabilityID;
                        model.YearsOfExperience = additionaldatalist[0].experience;
                        model.HighestQualificationLevel = additionaldatalist[0].highestqualificationlevel;
                        //model.RegistrationType = additionaldatalist[0].registrationtype;
                        model.ExperienceType = additionaldatalist[0].experiencetype;
                    }

                    //if (model.IsUAEResident != "1")
                    if (!IsUAE_Redisdent())
                    {
                        if (model.CandidateAttchments != null && model.CandidateAttchments.Where(x => x.attachmenttype == CareerPortalHelper.PassportAttachmentType).ToList().Count == 0)
                        {
                            isAttachment = false;
                            CacheProvider.Store(CacheKeys.CAREERPORTAL_ErrorMessage, new AccessCountingCacheItem<string>(Translate.Text("upload passport"), Times.Once));
                        }
                    }
                }
            }
            return isAttachment;
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

        private int ProfileProgress(out bool IsSubmit)
        {
            IsSubmit = false;

            try
            {
                var response = JobSeekerClient.GetCandidateProfileStatus(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.@return.profilecompletion != null)
                {
                    if (response.Payload.@return.profilecomplete != null && response.Payload.@return.profilecomplete.Length > 0)
                    {
                        IsSubmit = Convert.ToDouble(response.Payload.@return.profilecomplete[0].submit) > 0 ? true : false;
                    }

                    var profileCompletion = response.Payload.@return.profilecompletion;
                    if (!string.IsNullOrWhiteSpace(profileCompletion))
                    {
                        return Convert.ToInt32(Convert.ToDouble(profileCompletion));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
            }

            return 0;
        }

        #endregion Methods
    }
}