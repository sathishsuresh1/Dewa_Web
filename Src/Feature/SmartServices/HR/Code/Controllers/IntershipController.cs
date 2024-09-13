using DEWAXP.Feature.HR.Models.Internship;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartSurvey;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartSurvey;
using DEWAXP.Foundation.Integration.InternshipSvc;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace DEWAXP.Feature.HR.Controllers
{
    public class InternshipController : BaseController
    {
        #region Properties and Varibles

        private enum TrainingType
        {
            PRJ = 0,
            INT = 1,
            SUM = 2
        }

        private const string channel = "WEB";

        private IEFormServiceClient EFormsServiceClient
        {
            get { return DependencyResolver.Current.GetService<IEFormServiceClient>(); }
        }

        #endregion Properties and Varibles

        #region Actions

        [HttpGet]
        public ActionResult Research()
        {
            Session.Add("Load", "isLoad");

            var model = new Internship();

            var requestHelpValues = new GetInternshipHelpValues();

            requestHelpValues.countrycodes = "X";

            var responseModel = GetHelperValues(model, requestHelpValues);
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            return PartialView("~/Views/Feature/HR/Internship/_Research.cshtml", responseModel);
        }

        [HttpGet]
        public ActionResult ApplyInternship()
        {
            var model = new Internship();
            var requestHelpValues = new GetInternshipHelpValues();

            model.EmirateList = GetLstDataSource(DataSources.EmiratesList);// GetEmirates();

            requestHelpValues.countrycodes = "X";

            var responseModel = GetHelperValues(model, requestHelpValues);

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            return PartialView("~/Views/Feature/HR/Internship/_ApplyInternship.cshtml", responseModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyInternship(Internship model)
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

                if (status)
                {
                    MemoryStream TranscriptMemoryStream = new MemoryStream();
                    model.File_Transcript.InputStream.CopyTo(TranscriptMemoryStream);

                    MemoryStream OfficialLetterMemoryStream = new MemoryStream();
                    model.File_Official_Letter.InputStream.CopyTo(OfficialLetterMemoryStream);

                    MemoryStream CVMemoryStream = new MemoryStream();
                    model.File_CV.InputStream.CopyTo(CVMemoryStream);

                    //MemoryStream PassportMemoryStream = new MemoryStream();
                    //model.File_Passport.InputStream.CopyTo(PassportMemoryStream);

                    var request = new SetInternshipRegistration
                    {
                        InternshipUserDetails = new internshipUserDetails()
                        {
                            applicantname = model.Name,
                            startdate = FormatDate(DateTime.Now.ToString()),
                            emailaddress = model.Email_Address,
                            mobilenumber = model.CountryCode + model.Mobile_Number,
                            major = model.Major,
                            universityname = model.University,
                            internproject = TrainingType.INT.ToString(),
                            address = model.Address,
                            age = model.Age,
                            country = model.Nationality,
                            emirate = model.Emirates,
                            loginchannel = channel,
                            //gradepointaverage = model.GradePointAverage
                        },
                        InternshipDetails = new internshipDetails()
                        {
                            coordinatoremailaddress = model.WorkPlacement_Coordinator_Email,
                            workplacecoordinator = model.WorkPlacement_Coordinator_Name,
                            fromdate = FormatDate(model.WorkPlacement_From_Date),
                            todate = FormatDate(model.WorkPlacement_To_Date),
                            dewascholarship = model.DEWA_Scholarship,
                            scholarshipID = model.DEWA_Scholarship_ID
                        },

                        InternshipTrainingattachments = new internshipTrainingattachments()
                        {
                            officialletter = OfficialLetterMemoryStream.ToArray(),
                            officialletterfilename = model.File_Official_Letter.FileName,
                            officialletterfiletype = model.File_Official_Letter.ContentType,

                            cv = CVMemoryStream.ToArray(),
                            cvfilename = model.File_CV.FileName,
                            cvfiletype = model.File_CV.ContentType,

                            transcriptcopy = TranscriptMemoryStream.ToArray(),
                            transcriptfilename = model.File_Transcript.FileName,
                            transcriptfiletype = model.File_Transcript.ContentType,

                            //passportcopy = PassportMemoryStream.ToArray(),
                            //passportcopyfilename = model.File_Passport.FileName,
                            //passportcopyfiletype = model.File_Passport.ContentType
                        }
                    };

                    var response = IntershipServiceClient.SetInternshipRegistration(request);

                    TranscriptMemoryStream.Flush();
                    OfficialLetterMemoryStream.Flush();
                    CVMemoryStream.Flush();
                    //PassportMemoryStream.Flush();

                    if (response != null)
                    {
                        if (response.Succeeded)
                        {
                            QueryString p = new QueryString(true);
                            p.With("p", TrainingType.INT.ToString(), false);

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_APPLY_SUCCESS, p);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            var requestHelpValues = new GetInternshipHelpValues();

            model.EmirateList = GetLstDataSource(DataSources.EmiratesList);// GetEmirates();

            requestHelpValues.countrycodes = "X";

            var responseModel = GetHelperValues(model, requestHelpValues);

            ViewBag.queryString = Session["q"];

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            return PartialView("~/Views/Feature/HR/Internship/_ApplyInternship.cshtml", responseModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Research(Internship model)
        {
            try
            {
                var isLoad = Session["Load"];

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
                    projectResearchattachments prj = new projectResearchattachments();

                    if (model.SQ == "1")
                    {
                        MemoryStream SurveyQuestionMemoryStream = new MemoryStream();
                        model.File_Survey_Questions.InputStream.CopyTo(SurveyQuestionMemoryStream);

                        prj.surveyquestions = SurveyQuestionMemoryStream.ToArray();
                        prj.surveyquestionsfilename = model.File_Survey_Questions.FileName;
                        prj.surveyquestionsfiletype = model.File_Survey_Questions.ContentType;

                        SurveyQuestionMemoryStream.Flush();
                    }
                    else if (model.SQ == "2")
                    {
                        MemoryStream InterQuestionMemoryStream = new MemoryStream();
                        model.File_Interview_Questions.InputStream.CopyTo(InterQuestionMemoryStream);

                        prj.interviewquestions = InterQuestionMemoryStream.ToArray();
                        prj.interviewquestionsfilename = model.File_Interview_Questions.FileName;
                        prj.interviewquestionsfiletype = model.File_Interview_Questions.ContentType;

                        InterQuestionMemoryStream.Flush();
                    }

                    MemoryStream OfficialLetterMemoryStream = new MemoryStream();
                    model.File_Official_Letter.InputStream.CopyTo(OfficialLetterMemoryStream);

                    MemoryStream ProjectDescriptionMemoryStream = new MemoryStream();
                    model.File_Project_Description.InputStream.CopyTo(ProjectDescriptionMemoryStream);

                    prj.officialletter = OfficialLetterMemoryStream.ToArray();
                    prj.officialletterfilename = model.File_Official_Letter.FileName;
                    prj.officialletterfiletype = model.File_Official_Letter.ContentType;

                    prj.projectdescription = ProjectDescriptionMemoryStream.ToArray();
                    prj.projectdescriptionfilename = model.File_Project_Description.FileName;
                    prj.projectdescriptionfiletype = model.File_Project_Description.ContentType;

                    OfficialLetterMemoryStream.Flush();
                    ProjectDescriptionMemoryStream.Flush();

                    var request = new SetInternshipRegistration
                    {
                        InternshipUserDetails = new internshipUserDetails()
                        {
                            applicantname = model.Name,
                            startdate = FormatDate(DateTime.Now.ToString()),
                            emailaddress = model.Email_Address,
                            mobilenumber = model.CountryCode + model.Mobile_Number,
                            major = model.Major,
                            universityname = model.University,
                            internproject = TrainingType.PRJ.ToString(),
                            loginchannel = channel
                        },
                        ProjectDetails = new projectDetails()
                        {
                            purposeofprojectrequest = model.Project,
                            subjectofproject = model.Subject
                        },

                        ProjectResearchattachments = prj
                    };

                    var response = IntershipServiceClient.SetInternshipRegistration(request);

                    if (response != null)
                    {
                        if (response.Succeeded)
                        {
                            QueryString p = new QueryString(true);
                            p.With("p", TrainingType.PRJ.ToString(), false);

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_APPLY_SUCCESS, p);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (System.Web.Mvc.HttpAntiForgeryException ex)
            {
                //Add log to Sitecore log
                LogService.Fatal(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            catch (System.Exception ex)
            {
                //Add log to Sitecore log
                LogService.Fatal(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            var requestHelpValues = new GetInternshipHelpValues();

            model.EmirateList = GetLstDataSource(DataSources.EmiratesList);// GetEmirates();

            requestHelpValues.countrycodes = "X";

            var responseModel = GetHelperValues(model, requestHelpValues);

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            ViewBag.queryString = Session["q"];

            return PartialView("~/Views/Feature/HR/Internship/_Research.cshtml", responseModel);
        }

        [HttpGet]
        public ActionResult SummerTraining()
        {
            var model = new Internship();

            var requestHelpValues = new GetInternshipHelpValues();

            requestHelpValues.Relation = "X";
            requestHelpValues.countrycodes = "X";
            requestHelpValues.departmentdivision = "X";

            var responseModel = GetHelperValues(model, requestHelpValues);

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            return PartialView("~/Views/Feature/HR/Internship/_SummerTraining.cshtml", responseModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SummerTraining(Internship model)
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

                if (status)
                {
                    MemoryStream TranscriptMemoryStream = new MemoryStream();
                    model.File_Transcript.InputStream.CopyTo(TranscriptMemoryStream);

                    MemoryStream PictureMemoryStream = new MemoryStream();
                    model.File_Picture.InputStream.CopyTo(PictureMemoryStream);

                    //MemoryStream FamilyBookMemoryStream = new MemoryStream();
                    //model.File_FamilyBook.InputStream.CopyTo(FamilyBookMemoryStream);

                    //MemoryStream PassportMemoryStream = new MemoryStream();
                    //model.File_Passport.InputStream.CopyTo(PassportMemoryStream);

                    var request = new SetInternshipRegistration
                    {
                        InternshipUserDetails = new internshipUserDetails()
                        {
                            applicantname = model.Name,
                            startdate = FormatDate(DateTime.Now.ToString()),
                            emailaddress = model.Email_Address,
                            mobilenumber = model.CountryCode + model.Mobile_Number,
                            major = model.Major,
                            universityname = model.University,
                            internproject = TrainingType.SUM.ToString(),
                            address = model.Address,
                            age = model.Age,
                            passportnumber = model.PassportNumber,
                            emirate = model.Emirates,
                            country = model.Nationality,
                            loginchannel = channel
                        },

                        SummerDetails = new summerDetails()
                        {
                            familybooknumber = model.FamilyBookNumber,
                            workindewa = model.Work_In_DEWA,
                            fromdate = FormatDate(model.Work_Dewa_From_Date),
                            todate = FormatDate(model.Work_Dewa_To_Date),
                            departmenttext = model.Department,
                            division = model.Division,
                            educationlevel = model.EducationLevel_High_School,
                            educationsection = model.Section,
                            parentname = model.Parent_Name,
                            parentrelationship = model.Parent_RelationShip,
                            homephonenumber = model.Parent_Home_Phone_Country_Code + model.Parent_Home_Phone,
                            relationshipmobilenumber = model.Parent_Mobile_Number_Country_Code + model.Parent_Mobile_Number,
                            relationshipwithrelative = model.Relative_Relationship,
                            relativeindewa = model.Relative_In_DEWA,
                            relativemobilenumber = model.Relative_Mobile_Country_Code + model.Relative_Mobile_Number,
                            relativename = model.Relative_Name,
                            relativeworkingdepartment = model.Relative_Department,
                            dateofbirth = FormatDate(model.DateOfBirth),
                            educationleveluniversity = model.EducationLevel_University,
                            semester = model.Semester,
                            relativeworkingdivision = model.Relative_Division,
                            yearworkedindewa = model.Work_Dewa_Year
                        },

                        SummerTrainingattachments = new summerTrainingattachments()
                        {
                            //passportcopy = PassportMemoryStream.ToArray(),
                            //passportcopyfilename = model.File_Passport.FileName,
                            //passportcopyfiletype = model.File_Passport.ContentType,

                            //familybook = FamilyBookMemoryStream.ToArray(),
                            //familybookfilename = model.File_FamilyBook.FileName,
                            //familybookfiletype = model.File_FamilyBook.ContentType,

                            personalphoto = PictureMemoryStream.ToArray(),
                            personalphotofilename = model.File_Picture.FileName,
                            personalphotofiletype = model.File_Picture.ContentType,

                            certificate = TranscriptMemoryStream.ToArray(),
                            certificatefilename = model.File_Transcript.FileName,
                            certificatefiletype = model.File_Transcript.ContentType,
                        }
                    };

                    var response = IntershipServiceClient.SetInternshipRegistration(request);

                    TranscriptMemoryStream.Flush();
                    PictureMemoryStream.Flush();
                    //PassportMemoryStream.Flush();
                    //FamilyBookMemoryStream.Flush();

                    if (response != null)
                    {
                        if (response.Succeeded)
                        {
                            QueryString p = new QueryString(true);
                            p.With("p", TrainingType.SUM.ToString(), false);

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_APPLY_SUCCESS, p);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (System.Exception ex)
            {
                //Add log to Sitecore log
                LogService.Fatal(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            var requestHelpValues = new GetInternshipHelpValues();

            model.EmirateList = GetLstDataSource(DataSources.EmiratesList);// GetEmirates();

            requestHelpValues.Relation = "X";
            requestHelpValues.countrycodes = "X";
            requestHelpValues.departmentdivision = "X";

            var responseModel = GetHelperValues(model, requestHelpValues);

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            ViewBag.queryString = Session["q"];

            return PartialView("~/Views/Feature/HR/Internship/_SummerTraining.cshtml", responseModel);
        }

        [HttpGet]
        public ActionResult Landing()
        {
            return PartialView("~/Views/Feature/HR/Internship/_Landing.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Landing(Internship model)
        {
            QueryString q = new QueryString(true);
            q.With("q", model.National, false);

            Session["q"] = model.National;

            if (model.Program == "1")
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_APPLY_INTERNSHIP, q);
            }
            else if (model.Program == "3")
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_RESEARCH, q);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SUMMER_TRAINING, q);
            }
        }

        [HttpGet]
        public ActionResult Success(Internship model)
        {
            return PartialView("~/Views/Feature/HR/Internship/_Success.cshtml", model);
        }

        #region Work Placement Evaluation
        [HttpGet]
        public ActionResult WorkPlacementEvaluation(string d)
        {
            d = Foundation.Content.Utils.CommonUtility.GetSanitizePlainText(d);
            string viewpath = "~/Views/Feature/HR/Internship/Survey/WorkPlacementEvaluation.cshtml";
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            if (!string.IsNullOrWhiteSpace(d))
            {
                var response = SmartSurveyClient.SurveyData(new SurveyDataInput { surveyinput = new Surveyinput { surveyidentifier = d, surveymode = "R" } }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    if (!string.IsNullOrWhiteSpace(response.Payload.Status) && response.Payload.Status.Equals("Y"))
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Survey_already_submitted"));
                        return View(viewpath, response.Payload);
                    }
                    if (!string.IsNullOrWhiteSpace(errorMessage))
                    {
                        return View(viewpath, response.Payload);
                    }
                    ViewBag.surveyid = d;
                    var CountryCoderesponse = IntershipServiceClient.GetHelpValues(new GetInternshipHelpValues { countrycodes = "X" }, RequestLanguage);
                    if (CountryCoderesponse != null && CountryCoderesponse.Succeeded && CountryCoderesponse.Payload != null && CountryCoderesponse.Payload.@return != null && CountryCoderesponse.Payload.@return.countrycodes != null)
                    {
                        var countrycodelist = CountryCoderesponse.Payload.@return.countrycodes
                       .Select(p => new SelectListItem()
                       {
                           Text = "+" + p.countrytelephonecode,
                           Value = p.countrytelephonecode,

                       }
                       ).ToList();
                        countrycodelist.Where(X => X.Value.Equals("971")).ToList().ForEach(y => y.Selected = true);
                        ViewBag.CountryCallingCodesList = countrycodelist;
                        CacheProvider.Store(CacheKeys.Internship_Survey_key, new CacheItem<string>(d));
                        CacheProvider.Store(CacheKeys.Internship_Survey_Questions, new CacheItem<Surveydataoutput>(response.Payload));

                        if ((!string.IsNullOrWhiteSpace(response.Payload.Status) && response.Payload.Status.Equals("D")) || !string.IsNullOrWhiteSpace(response.Payload.Authenticate) && response.Payload.Authenticate.ToLower().Equals("false"))
                            return View(viewpath, response.Payload);
                        else
                        {
                            SurveyOTPVerify surveyOTPVerify;
                            if (CacheProvider.TryGet(CacheKeys.Internship_Survey_OTPSend, out surveyOTPVerify))
                            {
                                if (surveyOTPVerify.OTPVerified == true)
                                {
                                    CacheProvider.Remove(CacheKeys.Internship_Survey_OTPSend);
                                    return View(viewpath, response.Payload);
                                }
                            }
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_OTPSEND_PAGE);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Something went wrong"));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Check the intern survey Link"));
            }
            return View(viewpath, null);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WorkPlacementEvaluation(string form, string d)
        {
            string surveykey = string.Empty;
            Surveydataoutput surveydataoutput;
            if (!CacheProvider.TryGet(CacheKeys.Internship_Survey_key, out surveykey))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE, new QueryString(false).With("d", d, false));
            }
            if (!surveykey.Equals(d))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE, new QueryString(false).With("d", d, false));
            }
            if (!CacheProvider.TryGet(CacheKeys.Internship_Survey_Questions, out surveydataoutput))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE, new QueryString(false).With("d", d, false));
            }
            //string viewpath = "~/Views/Feature/HR/Internship/Survey/WorkPlacementEvaluation.cshtml";
            if (!string.IsNullOrWhiteSpace(surveykey) && surveydataoutput != null)
            {
                string error = string.Empty;
                Surveyinput surveyinput = GenerateSurveyInput(Request.Form, surveykey, surveydataoutput, out error);
                if (string.IsNullOrWhiteSpace(error))
                {
                    var response = SmartSurveyClient.SurveyData(new SurveyDataInput { surveyinput = surveyinput }, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        @ViewBag.Title = surveydataoutput.Introductionheader;
                        return View("~/Views/Feature/HR/Internship/Survey/_Success.cshtml");
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                    }
                }
                else
                {
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                }
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Check the intern survey Link"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE, new QueryString(false).With("d", d, false));
        }
        [HttpGet]
        public ActionResult DownloadFile(string id)
        {
            string surveykey = string.Empty;
            Surveydataoutput surveydataoutput;

            if (CacheProvider.TryGet(CacheKeys.Internship_Survey_Questions, out surveydataoutput))
            {

                if (surveydataoutput != null)
                {
                    List<Foundation.Integration.APIHandler.Models.Response.SmartSurvey.Grouplist> Grouplist = surveydataoutput.Grouplist;

                    var Questiontypeslist = Grouplist?.Select(x => x.Questiontypeslist?.Where(q => q.Questiontype == id)).FirstOrDefault();

                    if (Questiontypeslist != null && Questiontypeslist.Count() > 0)
                    {
                        byte[] bytes = Convert.FromBase64String(Questiontypeslist?.FirstOrDefault().Surveyfeedback);
                        string type = Questiontypeslist?.FirstOrDefault().Filetype;
                        if (bytes != null && type != null)
                            return File(bytes, type);
                    }
                }
            }
            return null;
        }
        #endregion

        #region OTP Verification

        [HttpGet]
        public ActionResult SurveySendOTP()
        {
            string viewpath = "~/Views/Feature/HR/Internship/Survey/OTPSend.cshtml";
            string surveykey = string.Empty;
            Surveydataoutput surveydataoutput;
            ViewBag.IsValidKey = true;
            SurveyOTPVerify model = new SurveyOTPVerify();
            CacheProvider.Remove(CacheKeys.Internship_Survey_OTPSend);
            if (!CacheProvider.TryGet(CacheKeys.Internship_Survey_key, out surveykey))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE);
            }

            if (CacheProvider.TryGet(CacheKeys.Internship_Survey_Questions, out surveydataoutput))
            {
                model.EmailAddress = surveydataoutput.Emailid;
                model.MobileNumber = surveydataoutput.Phone;
                model.MaskedEmailAddress = (!string.IsNullOrWhiteSpace(surveydataoutput.MaskedEmailid)) ? surveydataoutput.MaskedEmailid : (!string.IsNullOrWhiteSpace(surveydataoutput.Emailid) ? MaskEmail(surveydataoutput.Emailid) : string.Empty);
                model.MaskedMobileNumber = (!string.IsNullOrWhiteSpace(surveydataoutput.MaskedPhone)) ? surveydataoutput.MaskedPhone : (!string.IsNullOrWhiteSpace(surveydataoutput.Phone) ? MaskMobile(surveydataoutput.Phone) : string.Empty);
            }
            else
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE);

            if (surveydataoutput != null && !string.IsNullOrWhiteSpace(surveydataoutput.Authenticate) && surveydataoutput.Authenticate.ToLower().Equals("false"))
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE);

            return View(viewpath, model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SurveySendOTP(SurveyOTPVerify surveyOTPVerify)
        {
            string viewpath = "~/Views/Feature/HR/Internship/Survey/OTPSend.cshtml";
            string surveykey = string.Empty;
            Surveydataoutput surveydataoutput;
            if (CacheProvider.TryGet(CacheKeys.Internship_Survey_Questions, out surveydataoutput))
            {
                surveyOTPVerify.EmailAddress = surveydataoutput.Emailid;
                surveyOTPVerify.MobileNumber = surveydataoutput.Phone;
                surveyOTPVerify.MaskedEmailAddress = (!string.IsNullOrWhiteSpace(surveydataoutput.MaskedEmailid)) ? surveydataoutput.MaskedEmailid : (!string.IsNullOrWhiteSpace(surveydataoutput.Emailid) ? MaskEmail(surveydataoutput.Emailid) : string.Empty);
                surveyOTPVerify.MaskedMobileNumber = (!string.IsNullOrWhiteSpace(surveydataoutput.MaskedPhone)) ? surveydataoutput.MaskedPhone : (!string.IsNullOrWhiteSpace(surveydataoutput.Phone) ? MaskMobile(surveydataoutput.Phone) : string.Empty);
            }

            if (CacheProvider.TryGet(CacheKeys.Internship_Survey_key, out surveykey))
            {
                var response = SmartSurveyClient.SurveyOtp(new SurveyOTPInput
                {
                    surveyotpinput = new Surveyotpinput
                    {
                        email = (!string.IsNullOrWhiteSpace(surveyOTPVerify.SelectedOptions) && surveyOTPVerify.SelectedOptions.ToLower().Equals("email")) ? surveydataoutput.Emailid : string.Empty,
                        mobile = (!string.IsNullOrWhiteSpace(surveyOTPVerify.SelectedOptions) && surveyOTPVerify.SelectedOptions.ToLower().Equals("mobile")) ? surveydataoutput.Phone : string.Empty,
                        mode = "S",
                        surveyidentifier = surveykey
                    }
                }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.Responsecode == "000")
                {
                    surveyOTPVerify.OTPSend = true;
                    CacheProvider.Store(CacheKeys.Internship_Survey_OTPSend, new CacheItem<SurveyOTPVerify>(surveyOTPVerify));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_OTPVERIFICATION_PAGE);
                }
                else
                {
                    ModelState.AddModelError("", response.Message);
                }
            }
            return View(viewpath, surveyOTPVerify);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ResendOTP()
        {
            string surveykey = string.Empty;
            SurveyOTPVerify surveyOTPVerify;
            CacheProvider.TryGet(CacheKeys.Internship_Survey_key, out surveykey);
            if (CacheProvider.TryGet(CacheKeys.Internship_Survey_OTPSend, out surveyOTPVerify))
            {
                var response = SmartSurveyClient.SurveyOtp(new SurveyOTPInput
                {
                    surveyotpinput = new Surveyotpinput
                    {
                        email = (!string.IsNullOrWhiteSpace(surveyOTPVerify.SelectedOptions) && surveyOTPVerify.SelectedOptions.ToLower().Equals("email")) ? surveyOTPVerify.EmailAddress : string.Empty,
                        mobile = (!string.IsNullOrWhiteSpace(surveyOTPVerify.SelectedOptions) && surveyOTPVerify.SelectedOptions.ToLower().Equals("mobile")) ? surveyOTPVerify.MobileNumber : string.Empty,
                        mode = "S",
                        surveyidentifier = surveykey
                    }
                }, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null && response.Payload.Responsecode == "000")
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SurveyOTPVerfication()
        {
            string viewpath = "~/Views/Feature/HR/Internship/Survey/OTPVerification.cshtml";
            string surveykey = string.Empty;
            Surveydataoutput surveydataoutput;
            SurveyOTPVerify surveyOTPVerify;
            if (!CacheProvider.TryGet(CacheKeys.Internship_Survey_OTPSend, out surveyOTPVerify) ||
                !CacheProvider.TryGet(CacheKeys.Internship_Survey_key, out surveykey) ||
                !CacheProvider.TryGet(CacheKeys.Internship_Survey_Questions, out surveydataoutput))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE);
            }

            if (surveyOTPVerify != null && surveyOTPVerify.OTPSend == false)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE);
            if (surveyOTPVerify != null && !string.IsNullOrWhiteSpace(surveyOTPVerify.TimerTime))
                surveyOTPVerify.TimerTime = string.Empty;
            return View(viewpath, surveyOTPVerify);
        }
        [HttpPost]
        public ActionResult SurveyOTPVerfication(string otp, string timertime)
        {
            otp = Foundation.Content.Utils.CommonUtility.GetSanitizePlainText(otp);
            string viewpath = "~/Views/Feature/HR/Internship/Survey/OTPVerification.cshtml";
            string surveykey = string.Empty;
            CacheProvider.TryGet(CacheKeys.Internship_Survey_key, out surveykey);
            SurveyOTPVerify surveyOTPVerify;
            if (CacheProvider.TryGet(CacheKeys.Internship_Survey_OTPSend, out surveyOTPVerify))
            {
                var response = SmartSurveyClient.SurveyOtp(new SurveyOTPInput
                {
                    surveyotpinput = new Surveyotpinput
                    {
                        email = (!string.IsNullOrWhiteSpace(surveyOTPVerify.SelectedOptions) && surveyOTPVerify.SelectedOptions.ToLower().Equals("email")) ? surveyOTPVerify.EmailAddress : string.Empty,
                        mobile = (!string.IsNullOrWhiteSpace(surveyOTPVerify.SelectedOptions) && surveyOTPVerify.SelectedOptions.ToLower().Equals("mobile")) ? surveyOTPVerify.MobileNumber : string.Empty,
                        mode = "V",
                        OTP = otp,
                        surveyidentifier = surveykey
                    }
                }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.Responsecode == "000")
                {
                    surveyOTPVerify.OTPSend = false;
                    surveyOTPVerify.OTPVerified = true;
                    CacheProvider.Store(CacheKeys.Internship_Survey_OTPSend, new CacheItem<SurveyOTPVerify>(surveyOTPVerify));
                    if (CacheProvider.TryGet(CacheKeys.Internship_Survey_key, out surveykey))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.INTERNSHIP_SURVEY_PAGE, new QueryString(false).With("d", surveykey, false));
                    }
                }
                else
                {
                    surveyOTPVerify.TimerTime = timertime;
                    surveyOTPVerify.Message = response.Message;
                    ModelState.AddModelError("", response.Message);
                }
            }
            return View(viewpath, surveyOTPVerify);
        }


        #endregion

        #endregion

        #region Methods and Implementation 
        /// <summary>
        /// Format date to specific format
        /// </summary>
        /// <param name="_modelDate"></param>
        /// <returns></returns>
        private string FormatDate(string _modelDate)
        {
            string _date = string.Empty;

            if (_modelDate != null)
            {
                _date = Convert.ToDateTime(_modelDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December")).ToString("yyyyMMdd");
            }
            return _date;
        }

        private Internship GetHelperValues(Internship model, GetInternshipHelpValues requestHelpValues)
        {
            model.EmirateList = GetLstDataSource(DataSources.EmiratesList);//GetEmirates();
            model.SectionList = GetLstDataSource(DataSources.HighSchoolSectionsList);
            model.GradeList = GetLstDataSource(DataSources.GradeList);

            var response = IntershipServiceClient.GetHelpValues(requestHelpValues, RequestLanguage);

            if (response.Payload.@return != null)
            {
                if (response.Payload.@return.relationship != null)
                {
                    model.RelationList = response.Payload.@return.relationship
                        .Select(p => new SelectListItem() { Text = p.value, Value = p.value }).ToList();
                }
                if (response.Payload.@return.orgdepartment != null)
                {
                    model.DepartmentList = response.Payload.@return.orgdepartment
                    .Select(p => new SelectListItem()
                    {
                        Text = p.value,
                        Value = p.value
                    }
                    ).ToList();
                }
                if (response.Payload.@return.orgdivision != null)
                {
                    model.DivisionList = response.Payload.@return.orgdivision
                    .Select(p => new SelectListItem()
                    {
                        Text = p.value,
                        Value = p.value
                    }
                    ).ToList();
                }
                if (response.Payload.@return.countrycodes != null)
                {
                    model.CountryCallingCodesList = response.Payload.@return.countrycodes
                .Select(p => new SelectListItem()
                {
                    Text = p.countryname + "(+" + p.countrytelephonecode + ")",
                    Value = p.countrytelephonecode,
                }
                ).ToList();

                    if (Request.QueryString["q"] == "yes")
                    {
                        model.CountryCallingCodesList.Where(X => X.Value.Equals("971")).ToList().ForEach(y => y.Selected = true);
                    }
                }
            }
            return model;
        }

        private Surveyinput GenerateSurveyInput(NameValueCollection form, string surveykey, Surveydataoutput surveydataoutput, out string error)
        {
            error = string.Empty;
            string errormessage = Translate.Text("check the value");
            Surveyinput surveyinput = new Surveyinput { surveyidentifier = surveykey, surveymode = "U", surveydatainput = new Surveydatainput { grouplist = new List<Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Grouplist>() } };
            if (surveydataoutput != null && surveydataoutput.Grouplist != null && surveydataoutput.Grouplist.Count > 0)
            {
                foreach (Foundation.Integration.APIHandler.Models.Response.SmartSurvey.Grouplist group in surveydataoutput.Grouplist)
                {
                    Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Grouplist grouplist = new Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Grouplist
                    {
                        group = group.Group,
                        groupnumber = group.Groupnumber,
                        questiontypeslist = new List<Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Questiontypeslist>()
                    };
                    foreach (Foundation.Integration.APIHandler.Models.Response.SmartSurvey.Questiontypeslist question in group.Questiontypeslist)
                    {
                        if (!string.IsNullOrWhiteSpace(question.Displayonly) && question.Displayonly.Equals("X"))
                        {
                        }
                        else
                        {
                            Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Questiontypeslist questiontypeslist = new Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Questiontypeslist
                            {
                                question = question.Question,
                                questionnumber = question.Questionnumber,
                                questiontype = question.Questiontype,
                            };
                            switch (question.Questiontype)
                            {
                                case "CM":
                                case "SR":
                                case "ML":
                                    if (form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                    {
                                        questiontypeslist.surveyfeedback = form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)];
                                    }
                                    else
                                    {
                                        error = errormessage;
                                    }
                                    break;
                                case "MB":
                                    if (form[string.Format("form-field-group{0}_question{1}_code", group.Groupnumber, question.Questionnumber)] != null &&
                                        form[string.Format("form-field-group{0}_question{1}_number", group.Groupnumber, question.Questionnumber)] != null)
                                    {
                                        questiontypeslist.surveyfeedback = form[string.Format("form-field-group{0}_question{1}_code", group.Groupnumber, question.Questionnumber)] +
                                            form[string.Format("form-field-group{0}_question{1}_number", group.Groupnumber, question.Questionnumber)];
                                    }
                                    else
                                    {
                                        error = errormessage;
                                    }
                                    break;
                                case "DD":
                                case "DG":
                                    if (form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                    {
                                        questiontypeslist.optionslist = new List<Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist>();
                                        var optionvalues = form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)].Split(',');
                                        foreach (var option in optionvalues)
                                        {
                                            Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist optionslist = new Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist
                                            {
                                                questionoption = option,
                                                questionoptiondescription = question.Optionslist.FirstOrDefault(x => x.Questionoption.Equals(option)).Questionoptiondescription,
                                                selectedflag = "X"
                                            };
                                            questiontypeslist.optionslist.Add(optionslist);
                                        }

                                    }
                                    else
                                    {
                                        error = errormessage;
                                    }
                                    break;
                                case "DT":
                                    if (!string.IsNullOrWhiteSpace(question.Alignment) && question.Alignment.Equals("R"))
                                    {
                                        if (form[string.Format("form-field-group{0}_question{1}From", group.Groupnumber, question.Questionnumber)] != null &&
                                            form[string.Format("form-field-group{0}_question{1}To", group.Groupnumber, question.Questionnumber)] != null)
                                        {
                                            try
                                            {
                                                DateTime fromTime;
                                                DateTime ToTime;
                                                DateTime.TryParse(form[string.Format("form-field-group{0}_question{1}From", group.Groupnumber, question.Questionnumber)], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromTime);
                                                DateTime.TryParse(form[string.Format("form-field-group{0}_question{1}To", group.Groupnumber, question.Questionnumber)], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ToTime);
                                                if (fromTime != null && ToTime != null && fromTime.Ticks > 0 && ToTime.Ticks > 0)
                                                {
                                                    questiontypeslist.surveyfeedback = string.Format("{0}-{1}", fromTime.ToString("yyyyMMdd"), ToTime.ToString("yyyyMMdd"));
                                                }
                                                else
                                                {
                                                    error = errormessage;
                                                }
                                            }
                                            catch (System.Exception ex)
                                            {
                                                LogService.Error(ex, this);
                                                error = errormessage;
                                            }
                                        }
                                        else
                                        {
                                            error = errormessage;
                                        }
                                    }
                                    else
                                    {
                                        if (form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                        {
                                            try
                                            {
                                                DateTime fromTime;
                                                DateTime.TryParse(form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromTime);
                                                if (fromTime != null && fromTime.Ticks > 0)
                                                {
                                                    questiontypeslist.surveyfeedback = fromTime.ToString("yyyyMMdd");
                                                }
                                                else
                                                {
                                                    error = errormessage;
                                                }
                                            }
                                            catch (System.Exception ex)
                                            {
                                                LogService.Error(ex, this);
                                                error = errormessage;
                                            }
                                        }
                                        else
                                        {
                                            error = errormessage;
                                        }
                                    }
                                    break;
                                case "RB":
                                case "SM":
                                    if (form[string.Format("radio_group{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                    {
                                        questiontypeslist.optionslist = new List<Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist>();
                                        Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist optionslist = new Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist
                                        {
                                            questionoption = form[string.Format("radio_group{0}_question{1}", group.Groupnumber, question.Questionnumber)],
                                            questionoptiondescription = question.Optionslist.FirstOrDefault(x => x.Questionoption.Equals(form[string.Format("radio_group{0}_question{1}", group.Groupnumber, question.Questionnumber)])).Questionoptiondescription,
                                            selectedflag = "X"
                                        };
                                        questiontypeslist.optionslist.Add(optionslist);
                                        if (!string.IsNullOrWhiteSpace(question.Othermandatoryoptionno) && !question.Othermandatoryoptionno.Equals("0") &&
                                            form[string.Format("form-field-group{0}_question{1}_textarea", group.Groupnumber, question.Questionnumber)] != null)
                                        {
                                            questiontypeslist.surveyfeedback = form[string.Format("form-field-group{0}_question{1}_textarea", group.Groupnumber, question.Questionnumber)];
                                        }
                                    }
                                    else
                                    {
                                        error = errormessage;
                                    }
                                    break;
                                case "CH":
                                case "SW":
                                    if (form[string.Format("checkbox_group{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                    {
                                        questiontypeslist.optionslist = new List<Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist>();
                                        var optionvalues = form[string.Format("checkbox_group{0}_question{1}", group.Groupnumber, question.Questionnumber)].Split(',');
                                        foreach (var option in optionvalues)
                                        {
                                            Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist optionslist = new Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist
                                            {
                                                questionoption = option,
                                                questionoptiondescription = question.Optionslist.FirstOrDefault(x => x.Questionoption.Equals(option)).Questionoptiondescription,
                                                selectedflag = "X"
                                            };
                                            questiontypeslist.optionslist.Add(optionslist);
                                        }
                                        if (!string.IsNullOrWhiteSpace(question.Othermandatoryoptionno) && !question.Othermandatoryoptionno.Equals("0") &&
                                            form[string.Format("form-field-group{0}_question{1}_textarea", group.Groupnumber, question.Questionnumber)] != null)
                                        {
                                            questiontypeslist.surveyfeedback = form[string.Format("form-field-group{0}_question{1}_textarea", group.Groupnumber, question.Questionnumber)];
                                        }
                                    }
                                    else
                                    {
                                        error = errormessage;
                                    }
                                    break;

                                case "ST":
                                    if (form[string.Format("form-field-rangeslider{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                    {
                                        questiontypeslist.optionslist = new List<Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist>();
                                        Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist optionslist = new Foundation.Integration.APIHandler.Models.Request.SmartSurvey.Optionslist
                                        {
                                            questionoption = form[string.Format("form-field-rangeslider{0}_question{1}", group.Groupnumber, question.Questionnumber)],
                                            questionoptiondescription = question.Optionslist.FirstOrDefault(x => x.Questionoption.Equals(form[string.Format("form-field-rangeslider{0}_question{1}", group.Groupnumber, question.Questionnumber)])).Questionoptiondescription,
                                            selectedflag = "X"
                                        };
                                        questiontypeslist.optionslist.Add(optionslist);
                                    }
                                    else
                                    {
                                        error = errormessage;
                                    }
                                    break;
                                case "TM":
                                    if (!string.IsNullOrWhiteSpace(question.Alignment) && question.Alignment.Equals("R"))
                                    {
                                        if (form[string.Format("form-field-group{0}_question{1}From", group.Groupnumber, question.Questionnumber)] != null &&
                                            form[string.Format("form-field-group{0}_question{1}To", group.Groupnumber, question.Questionnumber)] != null)
                                        {
                                            try
                                            {
                                                DateTime fromTime;
                                                DateTime ToTime;
                                                DateTime.TryParseExact(form[string.Format("form-field-group{0}_question{1}From", group.Groupnumber, question.Questionnumber)].Replace("صباحاً", "AM").Replace("مساءً", "PM"), "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromTime);
                                                DateTime.TryParseExact(form[string.Format("form-field-group{0}_question{1}To", group.Groupnumber, question.Questionnumber)].Replace("صباحاً", "AM").Replace("مساءً", "PM"), "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ToTime);
                                                if (fromTime != null && ToTime != null && fromTime.Ticks > 0 && ToTime.Ticks > 0)
                                                {
                                                    questiontypeslist.surveyfeedback = string.Format("{0}-{1}", fromTime.ToString("hh:mm:ss"), ToTime.ToString("hh:mm:ss"));
                                                }
                                                else
                                                {
                                                    error = errormessage;
                                                }
                                            }
                                            catch (System.Exception ex)
                                            {
                                                LogService.Error(ex, this);
                                                error = errormessage;
                                            }
                                        }
                                        else
                                        {
                                            error = errormessage;
                                        }
                                    }
                                    else
                                    {
                                        if (form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                        {
                                            try
                                            {
                                                DateTime fromTime;
                                                DateTime.TryParseExact(form[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)].Replace("صباحاً", "AM").Replace("مساءً", "PM"), "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromTime);
                                                if (fromTime != null && fromTime.Ticks > 0)
                                                {
                                                    questiontypeslist.surveyfeedback = fromTime.ToString("hh:mm:ss");
                                                }
                                                else
                                                {
                                                    error = errormessage;
                                                }
                                            }
                                            catch (System.Exception ex)
                                            {
                                                LogService.Error(ex, this);
                                                error = errormessage;
                                            }
                                        }
                                        else
                                        {
                                            error = errormessage;
                                        }
                                    }
                                    break;
                                case "FL":
                                case "SG":
                                case "PH":
                                    if (Request.Files != null && Request.Files[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)] != null)
                                    {
                                        try
                                        {
                                            HttpPostedFileBase file = Request.Files[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)]; //Uploaded file
                                                                                                                                                                                   //Use the following properties to get file's name, size and MIMEType
                                            int fileSize = file.ContentLength;
                                            string fileName = file.FileName;
                                            string ext = file.GetTrimmedFileExtension();
                                            long FileSizeLimit = !string.IsNullOrWhiteSpace(question.Allowedfilesize) ? Convert.ToInt64(question.Allowedfilesize) : 2048000;
                                            var supportedTypes = !string.IsNullOrWhiteSpace(question.Allowedfiletypes) ? question.Allowedfiletypes.Split(',').Select(x => "." + x).ToArray() : new[] { ".jpg", ".png", ".jpeg", ".PNG", ".JPG", ".JPEG" };
                                            if (!CustomeAttachmentIsValid(file, FileSizeLimit, out error, supportedTypes))
                                            {
                                                ModelState.AddModelError(string.Empty, error);
                                                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                                            }
                                            else
                                            {
                                                string mimeType = file.ContentType;
                                                Stream fileContent = file.InputStream;
                                                byte[] fileData = null;
                                                using (var binaryReader = new BinaryReader(Request.Files[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)].InputStream))
                                                {
                                                    fileData = binaryReader.ReadBytes(Request.Files[string.Format("form-field-group{0}_question{1}", group.Groupnumber, question.Questionnumber)].ContentLength);
                                                    questiontypeslist.surveyfeedback = Convert.ToBase64String(fileData);
                                                    questiontypeslist.filetype = file.ContentType;
                                                    questiontypeslist.filename = file.FileName;
                                                }
                                            }
                                        }
                                        catch (System.Exception ex)
                                        {
                                            LogService.Error(ex, this);
                                            error = errormessage;
                                        }
                                    }
                                    else
                                    {
                                        error = errormessage;
                                    }

                                    break;
                                default:
                                    break;
                            }
                            grouplist.questiontypeslist.Add(questiontypeslist);
                        }
                    }
                    surveyinput.surveydatainput.grouplist.Add(grouplist);
                }
            }
            return surveyinput;
        }

        private string MaskMobile(string number)
        {
            if (!string.IsNullOrWhiteSpace(number))
            {
                return string.Format("XXXXXXX{0}", number.Trim().Substring(7, 3));
            }
            return string.Empty;
        }
        private string MaskEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
                return Regex.Replace(email, pattern, m => new string('*', m.Length));
            }
            return string.Empty;
        }
        #endregion
    }
}