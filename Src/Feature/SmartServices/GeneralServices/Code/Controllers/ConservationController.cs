using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Models.Common;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.DewaSvc;
    using DEWAXP.Foundation.Integration.LectureBookingSvc;
    using Models.Conservation;
    using System.IO;

    public class ConservationController : BaseController
    {
        #region [variable]
        private const string EducationFormCode = "02";
        private const string LeaderFormCode = "03";
        private const string TeamFormCode = "04";
        #endregion [variable]
        // GET: DEWAForms
        #region [Actions]

        /// <summary>
        /// 1. EducationalInstitutionForm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FormEducationalInstitution()
        {
            FormEducationalInstitutionModel model = new FormEducationalInstitutionModel();

            // ReCaptcha
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            var preValuesDataList = GetPreValuesData(EducationFormCode);
            if (preValuesDataList != null && preValuesDataList.Count() > 0)
            {
                model.conservationFormParameters.CoFormTypeofInstitution = GetList(preValuesDataList, "TYPE OF INSTITUTION");
                model.conservationFormParameters.CoFormAcedamicLevel = GetList(preValuesDataList, "ACADEMIC LEVEL");
                model.conservationFormParameters.CoFormAirconditioning = GetList(preValuesDataList, "AIRCONDITIONING (AC)");
                model.conservationFormParameters.CoFormLighting = GetList(preValuesDataList, "LIGHTING");
                model.conservationFormParameters.CoFormOfficeEquipment = GetList(preValuesDataList, "OFFICE EQUIPMENT");
                model.conservationFormParameters.CoFormWater = GetList(preValuesDataList, "WATER");
                model.conservationFormParameters.CoFormOther = GetList(preValuesDataList, "OTHER");
                model.conservationFormParameters.CoFormNone = GetList(preValuesDataList, "NONE");
                model.conservationFormParameters.CoFormEnvironmentalActivites = GetList(preValuesDataList, "ENVIRONMENTAL ACTIVITIES");
                model.conservationFormParameters.CoFormLecturesWorkshops = GetList(preValuesDataList, "LECTURES & WORKSHOPS");
                model.conservationFormParameters.CoFormMethodOfEducation = GetList(preValuesDataList, "METHOD EDUCATION");
            }
            return View("~/Views/Feature/GeneralServices/Conservation/FormEducationalInstitution.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FormEducationalInstitution(FormEducationalInstitutionModel model)
        {
            try
            {
                bool status = false;
                bool validattach = true;
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
                        var requestForm = new formBookingRequest()
                        {
                            // Sector section
                            sector = model.Sector,

                            // Institute Details
                            nameofinstitute = model.InstitutionDetails.Name,
                            institutelandline = model.InstitutionDetails.ContactNo,
                            instituteemailaddress = model.InstitutionDetails.EmailAddress,
                            totalfaculty = model.InstitutionDetails.TotalFaculityNo,
                            totalstudents = model.InstitutionDetails.TotalStudentNo,
                            electricityaccountnumber = model.InstitutionDetails.EletricityAccountNo,
                            wateraccountnumber = model.InstitutionDetails.WaterAccountNo,

                            // Headmaster/Headmistress Details
                            coordinatorname = model.HeadmasterOrHeadmistressDetails.Name,
                            coordinatormobilenumber = model.HeadmasterOrHeadmistressDetails.ContactNo,
                            coordinatoremailaddress = model.HeadmasterOrHeadmistressDetails.EmailAddress,

                            // Award Coordinator Details
                            nominatedcoordinatorname = model.AwardCoordinatorDetails.FullName,
                            nominatedcoordinatormobile = model.AwardCoordinatorDetails.ContactNo,
                            nominatedcoordinatoremail = model.AwardCoordinatorDetails.EmailAddress,

                            // Technical Parameters
                            saveewtargets = model.conservationTechnicalParameter.SavingObjective,
                            saveewchange = model.conservationTechnicalParameter.SavingBehaviour,

                            // Awareness Parameters
                            socialmediachannels = model.conservationAwarenessParameter.SocialMediaChannel
                        };

                        // Academic Level details
                        requestForm = SubmitAcademicInEduRequest(model.SelectedAcademics, requestForm, EducationFormCode);

                        //Method of Education
                        requestForm = SubmitMethodofEducationInEduRequest(model.MethodofEducation, requestForm, EducationFormCode);

                        // Technical & Awareness Parameters
                        var conservationAwardRequest = new conservationAwardRequest()
                        {
                            // Airconditioning (AC)
                            acfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),
                            acfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),
                            acfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),
                            acfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),

                            // Lightings
                            lgfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),

                            // Office Equipment
                            oefield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),

                            // Water
                            wtfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),

                            // Other
                            otfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),

                            // None
                            nnfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechNoneSelectedValue, model.conservationTechnicalParameter.TechNoneSelectedDetail),

                            // Environmental Activities
                            eafield1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield4 = GetcheckDesciption("4", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield5 = GetcheckDesciption("5", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield6 = GetcheckDesciption("6", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield7 = GetcheckDesciption("7", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield8 = GetcheckDesciption("8", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield9 = GetcheckDesciption("9", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),

                            eanstd1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd4 = GetcheckDesciption("4", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd5 = GetcheckDesciption("5", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd6 = GetcheckDesciption("6", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd7 = GetcheckDesciption("7", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd8 = GetcheckDesciption("8", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd9 = GetcheckDesciption("9", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),

                            // Lectures & Workshops
                            lwfield1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),
                            lwfield2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),
                            lwfield3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),

                            lwstd1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                            lwstd2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                            lwstd3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                        };

                        // Attachment logic
                        byte[] attachmentBytes = new byte[0];
                        var attachmentType = string.Empty;
                        string error = string.Empty;
                        if (model.AttachedDocument != null && model.AttachedDocument.ContentLength > 0)
                        {
                            if (!AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                            {
                                ModelState.AddModelError(string.Empty, error);
                                validattach = false;
                            }
                            else
                            {
                                try
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        model.AttachedDocument.InputStream.CopyTo(memoryStream);
                                        attachmentBytes = memoryStream.ToArray();
                                        attachmentType = model.AttachedDocument.FileName;
                                        model.AttachmentFileBinary = attachmentBytes;
                                        model.AttachmentFileType = attachmentType;
                                    }
                                }
                                catch (System.Exception)
                                {
                                    ModelState.AddModelError(string.Empty, "Unexpected error");
                                    // model.RegistrationStage = Step.STEP_ONE;
                                    return View("~/Views/Feature/GeneralServices/Conservation/FormEducationalInstitution.cshtml",model);
                                }
                            }
                        }
                        if (validattach)
                        {
                            PutLectureBookingAwardSubmission requestBooking = new PutLectureBookingAwardSubmission()
                            {
                                formrequest = requestForm,
                                conservationaward = conservationAwardRequest,
                                channel = "W",
                                form = Convert.ToString((int)ConservationFromType.FormEducationalInstitution),
                                updatemode = "W",
                                filecontent = model.AttachmentFileBinary != null ? model.AttachmentFileBinary.ToArray() : new byte[0],// Document data
                                filename = model.AttachedDocument != null ? model.AttachmentFileType : string.Empty, // Document
                            };

                            var submittedResponse = LectureBookingClient.PutLectureBookingAwardSubmission(requestBooking, Convert.ToString(AuthStateService?.GetActiveProfile()?.UserId), RequestLanguage, Request.Segment());

                            if (submittedResponse != null && submittedResponse.Succeeded && submittedResponse.Payload != null && submittedResponse.Payload.@return != null)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CONSERVATION_EDUCATIONALINSTITUTION_SUCCESSPAGE);
                            }
                            else
                            {
                                ModelState.AddModelError("", submittedResponse.Message);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
                //model.SectorList = GetSectorsList();
                // model.AcademicList = GetAcademyList(ConservationFromType.FormEducationalInstitution);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            var preValuesDataList = GetPreValuesData(EducationFormCode);
            if (preValuesDataList != null && preValuesDataList.Count() > 0)
            {
                model.conservationFormParameters.CoFormTypeofInstitution = GetList(preValuesDataList, "TYPE OF INSTITUTION");
                model.conservationFormParameters.CoFormAcedamicLevel = GetList(preValuesDataList, "ACADEMIC LEVEL");
                model.conservationFormParameters.CoFormAirconditioning = GetList(preValuesDataList, "AIRCONDITIONING (AC)");
                model.conservationFormParameters.CoFormLighting = GetList(preValuesDataList, "LIGHTING");
                model.conservationFormParameters.CoFormOfficeEquipment = GetList(preValuesDataList, "OFFICE EQUIPMENT");
                model.conservationFormParameters.CoFormWater = GetList(preValuesDataList, "WATER");
                model.conservationFormParameters.CoFormOther = GetList(preValuesDataList, "OTHER");
                model.conservationFormParameters.CoFormNone = GetList(preValuesDataList, "NONE");
                model.conservationFormParameters.CoFormEnvironmentalActivites = GetList(preValuesDataList, "ENVIRONMENTAL ACTIVITIES");
                model.conservationFormParameters.CoFormLecturesWorkshops = GetList(preValuesDataList, "LECTURES & WORKSHOPS");
                model.conservationFormParameters.CoFormMethodOfEducation = GetList(preValuesDataList, "METHOD EDUCATION");
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
            return View("~/Views/Feature/GeneralServices/Conservation/FormEducationalInstitution.cshtml",model);
        }

        /// <summary>
        /// 2. Leader Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FormLeader()
        {
            FormLeaderModel model = new FormLeaderModel();
            //model.SectorList = GetSectorsList();

            // ReCaptcha
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            var preValuesDataList = GetPreValuesData(LeaderFormCode);
            if (preValuesDataList != null && preValuesDataList.Count() > 0)
            {
                model.conservationFormParameters.CoFormTypeofInstitution = GetList(preValuesDataList, "TYPE OF INSTITUTION");
                model.conservationFormParameters.CoFormAirconditioning = GetList(preValuesDataList, "AIRCONDITIONING (AC)");
                model.conservationFormParameters.CoFormLighting = GetList(preValuesDataList, "LIGHTING");
                model.conservationFormParameters.CoFormOfficeEquipment = GetList(preValuesDataList, "OFFICE EQUIPMENT");
                model.conservationFormParameters.CoFormWater = GetList(preValuesDataList, "WATER");
                model.conservationFormParameters.CoFormOther = GetList(preValuesDataList, "OTHER");
                model.conservationFormParameters.CoFormNone = GetList(preValuesDataList, "NONE");
                model.conservationFormParameters.CoFormEnvironmentalActivites = GetList(preValuesDataList, "ENVIRONMENTAL ACTIVITIES");
                model.conservationFormParameters.CoFormLecturesWorkshops = GetList(preValuesDataList, "LECTURES & WORKSHOPS");
            }

            return View("~/Views/Feature/GeneralServices/Conservation/FormLeader.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FormLeader(FormLeaderModel model)
        {
            try
            {
                bool status = false;
                bool validattach = true;
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
                        // form Booking request
                        var requestForm = new formBookingRequest()
                        {
                            // Sector section
                            sector = model.Sector,

                            //Institute Details
                            nameofinstitute = model.InstitutionDetails.Name,
                            institutelandline = model.InstitutionDetails.ContactNo,
                            instituteemailaddress = model.InstitutionDetails.EmailAddress,

                            //Nominated Distinguished Conservation
                            coordinatorname = model.NominatedDistinguishedLeaderDetails.Name,
                            coordinatormobilenumber = model.NominatedDistinguishedLeaderDetails.ContactNo,
                            coordinatoremailaddress = model.NominatedDistinguishedLeaderDetails.EmailAddress,

                            //Conservation Leader Assistance Details
                            conservationname = model.ConservationLeaderAssistanceDetails.FullName,
                            conservationmobilenumber = model.ConservationLeaderAssistanceDetails.ContactNo,
                            conservationemailaddress = model.ConservationLeaderAssistanceDetails.EmailAddress,

                            //Nominated Award Coordinator Details
                            nominatedcoordinatorname = model.NominatedAwardCoordinatorDetails.FullName,
                            nominatedcoordinatormobile = model.NominatedAwardCoordinatorDetails.ContactNo,
                            nominatedcoordinatoremail = model.NominatedAwardCoordinatorDetails.EmailAddress,

                            //Nominated Parent Details
                            nominatedparentname = model.NominatedParentDetails.FullName,
                            nominatedparentmobile = model.NominatedParentDetails.ContactNo,
                            nominatedparentemail = model.NominatedParentDetails.EmailAddress,

                            // Technical Parameters
                            saveewtargets = model.conservationTechnicalParameter.SavingObjective,
                            saveewchange = model.conservationTechnicalParameter.SavingBehaviour,

                            // Awareness Parameters
                            socialmediachannels = model.conservationAwarenessParameter.SocialMediaChannel,
                            distinguishparenttargets = model.conservationAwarenessParameter.ParentTarget,
                            distinguishparentachie = model.conservationAwarenessParameter.ParentAchievement
                        };

                        // Technical & Awareness Parameters
                        var conservationAwardRequest = new conservationAwardRequest()
                        {
                            // Airconditioning (AC)
                            acfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),
                            acfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),
                            acfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),
                            acfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechAirconditioningSelectedValue, model.conservationTechnicalParameter.TechAirconditioningSelectedDetail),

                            // Lightings
                            lgfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),
                            lgfield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechLightingSelectedValue, model.conservationTechnicalParameter.TechLightingSelectedDetail),

                            // Office Equipment
                            oefield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),
                            oefield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechOfficeEquipmentSelectedValue, model.conservationTechnicalParameter.TechOfficeEquipmentSelectedDetail),

                            // Water
                            wtfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),
                            wtfield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechWaterSelectedValue, model.conservationTechnicalParameter.TechWaterSelectedDetail),

                            // Other
                            otfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield2 = GetcheckDesciption("2", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield3 = GetcheckDesciption("3", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield4 = GetcheckDesciption("4", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),
                            otfield5 = GetcheckDesciption("5", model.conservationTechnicalParameter.TechOtherSelectedValue, model.conservationTechnicalParameter.TechOtherSelectedDetail),

                            // None
                            nnfield1 = GetcheckDesciption("1", model.conservationTechnicalParameter.TechNoneSelectedValue, model.conservationTechnicalParameter.TechNoneSelectedDetail),

                            // Environmental Activities
                            eafield1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield4 = GetcheckDesciption("4", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield5 = GetcheckDesciption("5", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield6 = GetcheckDesciption("6", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield7 = GetcheckDesciption("7", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield8 = GetcheckDesciption("8", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield9 = GetcheckDesciption("9", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),

                            eanstd1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd4 = GetcheckDesciption("4", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd5 = GetcheckDesciption("5", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd6 = GetcheckDesciption("6", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd7 = GetcheckDesciption("7", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd8 = GetcheckDesciption("8", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd9 = GetcheckDesciption("9", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),

                            // Lectures & Workshops
                            lwfield1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),
                            lwfield2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),
                            lwfield3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),

                            lwstd1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                            lwstd2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                            lwstd3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                        };

                        // Attachment logic
                        byte[] attachmentBytes = new byte[0];
                        var attachmentType = string.Empty;
                        string error = string.Empty;
                        if (model.AttachedDocument != null && model.AttachedDocument.ContentLength > 0)
                        {
                            if (!AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                            {
                                ModelState.AddModelError(string.Empty, error);
                                validattach = false;
                            }
                            else
                            {
                                try
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        model.AttachedDocument.InputStream.CopyTo(memoryStream);
                                        attachmentBytes = memoryStream.ToArray();
                                        attachmentType = model.AttachedDocument.FileName;
                                        model.AttachmentFileBinary = attachmentBytes;
                                        model.AttachmentFileType = attachmentType;
                                    }
                                }
                                catch (System.Exception)
                                {
                                    ModelState.AddModelError(string.Empty, "Unexpected error");
                                    // model.RegistrationStage = Step.STEP_ONE;
                                    return View("~/Views/Feature/GeneralServices/Conservation/FormLeader.cshtml",model);
                                }
                            }
                        }
                        if (validattach)
                        {
                            var submittedResponse = LectureBookingClient.PutLectureBookingAwardSubmission(new PutLectureBookingAwardSubmission()
                            {
                                channel = "W", //WebService
                                form = Convert.ToString((int)ConservationFromType.FormLeader),
                                formrequest = requestForm,
                                conservationaward = conservationAwardRequest,
                                updatemode = "W", //write
                                filecontent = model.AttachmentFileBinary != null ? model.AttachmentFileBinary.ToArray() : new byte[0],// Document data
                                filename = model.AttachedDocument != null ? model.AttachmentFileType : string.Empty, // Document
                            }, Convert.ToString(AuthStateService?.GetActiveProfile()?.UserId), RequestLanguage, Request.Segment());

                            if (submittedResponse != null && submittedResponse.Payload != null && submittedResponse.Payload.@return != null && submittedResponse.Succeeded)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CONSERVATION_LEADER_SUCCESSPAGE);
                            }
                            else
                            {
                                ModelState.AddModelError("", submittedResponse.Message);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
                // model.SectorList = GetSectorsList();
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            var preValuesDataList = GetPreValuesData(LeaderFormCode);
            if (preValuesDataList != null && preValuesDataList.Count() > 0)
            {
                model.conservationFormParameters.CoFormTypeofInstitution = GetList(preValuesDataList, "TYPE OF INSTITUTION");
                model.conservationFormParameters.CoFormAirconditioning = GetList(preValuesDataList, "AIRCONDITIONING (AC)");
                model.conservationFormParameters.CoFormLighting = GetList(preValuesDataList, "LIGHTING");
                model.conservationFormParameters.CoFormOfficeEquipment = GetList(preValuesDataList, "OFFICE EQUIPMENT");
                model.conservationFormParameters.CoFormWater = GetList(preValuesDataList, "WATER");
                model.conservationFormParameters.CoFormOther = GetList(preValuesDataList, "OTHER");
                model.conservationFormParameters.CoFormNone = GetList(preValuesDataList, "NONE");
                model.conservationFormParameters.CoFormEnvironmentalActivites = GetList(preValuesDataList, "ENVIRONMENTAL ACTIVITIES");
                model.conservationFormParameters.CoFormLecturesWorkshops = GetList(preValuesDataList, "LECTURES & WORKSHOPS");
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
            return View("~/Views/Feature/GeneralServices/Conservation/FormLeader.cshtml",model);
        }

        /// <summary>
        /// 3. Team Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FormTeam()
        {
            FormTeamModel model = new FormTeamModel();
            // ReCaptcha
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            var preValuesDataList = GetPreValuesData(TeamFormCode);
            if (preValuesDataList != null && preValuesDataList.Count() > 0)
            {
                model.conservationFormParameters.CoFormTypeofInstitution = GetList(preValuesDataList, "TYPE OF INSTITUTION");
                model.conservationFormParameters.CoFormAcedamicLevel = GetList(preValuesDataList, "ACADEMIC LEVEL");
                model.conservationFormParameters.CoFormEnvironmentalActivites = GetList(preValuesDataList, "ENVIRONMENTAL ACTIVITIES");
                model.conservationFormParameters.CoFormLecturesWorkshops = GetList(preValuesDataList, "LECTURES & WORKSHOPS");
            }

            return View("~/Views/Feature/GeneralServices/Conservation/FormTeam.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FormTeam(FormTeamModel model)
        {
            try
            {
                bool status = false;
                bool validattach = true;
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
                        // form Booking request
                        var requestForm = new formBookingRequest()
                        {
                            //Sector Section
                            sector = model.Sector,
                            //nursery = model.AcademicLevel,

                            //Institute Details
                            nameofinstitute = model.InstitutionDetails.Name,

                            //Team Leader Details
                            coordinatorname = model.TeamLeaderDetails.Name,
                            coordinatormobilenumber = model.TeamLeaderDetails.ContactNo,
                            coordinatoremailaddress = model.TeamLeaderDetails.EmailAddress,

                            //Team member’s names
                            nominatedteamname1 = model.TeamMembers.NAME1,
                            nominatedteamname2 = model.TeamMembers.NAME2,
                            nominatedteamname3 = model.TeamMembers.NAME3,
                            nominatedteamname4 = model.TeamMembers.NAME4,
                            nominatedteamname5 = model.TeamMembers.NAME5,

                            // Technical Parameters
                            saveewtargets = model.conservationTechnicalParameter.SavingObjective,
                            saveewchange = model.conservationTechnicalParameter.SavingBehaviour,
                            innovationcreativity = model.conservationTechnicalParameter.SavingInnovation,

                            // Awareness Parameters
                            socialmediachannels = model.conservationAwarenessParameter.SocialMediaChannel
                        };

                        // Academic Level details
                        requestForm = SubmitAcademicInEduRequest(model.SelectedAcademics, requestForm, TeamFormCode);

                        // Technical & Awareness Parameters
                        var conservationAwardRequest = new conservationAwardRequest()
                        {
                            // Environmental Activities
                            eafield1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield4 = GetcheckDesciption("4", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield5 = GetcheckDesciption("5", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield6 = GetcheckDesciption("6", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield7 = GetcheckDesciption("7", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield8 = GetcheckDesciption("8", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),
                            eafield9 = GetcheckDesciption("9", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedDetail),

                            eanstd1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd4 = GetcheckDesciption("4", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd5 = GetcheckDesciption("5", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd6 = GetcheckDesciption("6", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd7 = GetcheckDesciption("7", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd8 = GetcheckDesciption("8", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),
                            eanstd9 = GetcheckDesciption("9", model.conservationAwarenessParameter.AwarEnvironmentalSelectedValue, model.conservationAwarenessParameter.AwarEnvironmentalSelectedStdno),

                            // Lectures & Workshops
                            lwfield1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),
                            lwfield2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),
                            lwfield3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedDetail),

                            lwstd1 = GetcheckDesciption("1", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                            lwstd2 = GetcheckDesciption("2", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                            lwstd3 = GetcheckDesciption("3", model.conservationAwarenessParameter.AwarLecturesSelectedValue, model.conservationAwarenessParameter.AwarLecturesSelectedStdno),
                        };

                        // Attachment logic
                        byte[] attachmentBytes = new byte[0];
                        var attachmentType = string.Empty;
                        string error = string.Empty;
                        if (model.AttachedDocument != null && model.AttachedDocument.ContentLength > 0)
                        {
                            if (!AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                            {
                                ModelState.AddModelError(string.Empty, error);
                                validattach = false;
                            }
                            else
                            {
                                try
                                {
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        model.AttachedDocument.InputStream.CopyTo(memoryStream);
                                        attachmentBytes = memoryStream.ToArray();
                                        attachmentType = model.AttachedDocument.FileName;
                                        model.AttachmentFileBinary = attachmentBytes;
                                        model.AttachmentFileType = attachmentType;
                                    }
                                }
                                catch (System.Exception)
                                {
                                    ModelState.AddModelError(string.Empty, "Unexpected error");
                                    // model.RegistrationStage = Step.STEP_ONE;
                                    return View("~/Views/Feature/GeneralServices/Conservation/FormTeam.cshtml",model);
                                }
                            }
                        }
                        if (validattach)
                        {
                            var submittedResponse = LectureBookingClient.PutLectureBookingAwardSubmission(new PutLectureBookingAwardSubmission()
                            {
                                channel = "W", //WebService
                                form = Convert.ToString((int)ConservationFromType.FormTeam),
                                formrequest = requestForm,
                                conservationaward = conservationAwardRequest,
                                updatemode = "W", //write
                                filecontent = model.AttachmentFileBinary != null ? model.AttachmentFileBinary.ToArray() : new byte[0],// Document data
                                filename = model.AttachedDocument != null ? model.AttachmentFileType : string.Empty, // Document
                            }, Convert.ToString(AuthStateService?.GetActiveProfile()?.UserId), RequestLanguage, Request.Segment());

                            if (submittedResponse != null && submittedResponse.Succeeded)
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CONSERVATION_TEAM_SUCCESSPAGE);
                            }
                            else
                            {
                                ModelState.AddModelError("", submittedResponse.Message);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
                //model.SectorList = GetSectorsList();
                //model.AcademicList = GetAcademyList(ConservationFromType.FormTeam);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var preValuesDataList = GetPreValuesData(TeamFormCode);
            if (preValuesDataList != null && preValuesDataList.Count() > 0)
            {
                model.conservationFormParameters.CoFormTypeofInstitution = GetList(preValuesDataList, "TYPE OF INSTITUTION");
                model.conservationFormParameters.CoFormAcedamicLevel = GetList(preValuesDataList, "ACADEMIC LEVEL");
                model.conservationFormParameters.CoFormEnvironmentalActivites = GetList(preValuesDataList, "ENVIRONMENTAL ACTIVITIES");
                model.conservationFormParameters.CoFormLecturesWorkshops = GetList(preValuesDataList, "LECTURES & WORKSHOPS");
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
            return View("~/Views/Feature/GeneralServices/Conservation/FormTeam.cshtml",model);
        }

        /// <summary>
        /// 4. Project Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FormProject()
        {
            FormProjectModel model = new FormProjectModel();
            model.SectorList = GetSectorsList();
            return View("~/Views/Feature/GeneralServices/Conservation/FormProject.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FormProject(FormProjectModel model)
        {
            if (ModelState.IsValid)
            {
                var submittedResponse = LectureBookingClient.PutLectureBookingAwardSubmission(new PutLectureBookingAwardSubmission()
                {
                    channel = "W", //WebService
                    form = Convert.ToString((int)ConservationFromType.FormProject),
                    formrequest = new formBookingRequest()
                    {
                        sector = model.Sector,

                        //instute detail
                        nameofinstitute = model.InstitutionDetails.Name,
                        institutelandline = model.InstitutionDetails.ContactNo,
                        instituteemailaddress = model.InstitutionDetails.EmailAddress,

                        //Institution Faculty detail
                        coordinatorname = model.InstitutionFacultyDetail.Name,
                        coordinatormobilenumber = model.InstitutionFacultyDetail.ContactNo,
                        coordinatoremailaddress = model.InstitutionFacultyDetail.EmailAddress,

                        //Nominated Team member's
                        nominatedteamname1 = model.NominatedTeamMembers.NAME1,
                        nominatedteamname2 = model.NominatedTeamMembers.NAME2,
                        nominatedteamname3 = model.NominatedTeamMembers.NAME3,
                    },
                    conservationaward = new conservationAwardRequest(),
                    updatemode = "W" //write
                }, Convert.ToString(AuthStateService?.GetActiveProfile()?.UserId), RequestLanguage, Request.Segment());

                if (submittedResponse != null && submittedResponse.Succeeded)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CONSERVATION_PROJECT_SUCCESSPAGE);
                }
                else
                {
                    ModelState.AddModelError("", submittedResponse.Message);
                }
            }

            model.SectorList = GetSectorsList();
            return View("~/Views/Feature/GeneralServices/Conservation/FormProject.cshtml",model);
        }

        /// <summary>
        /// 5.Lecture Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FormLecture()
        {
            FormLectureModel model = new FormLectureModel();
            model.FromHelpers = BindFormsPrefilledValues(ConservationFromType.FormLecture);
            // ReCaptcha
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            return View("~/Views/Feature/GeneralServices/Conservation/FormLecture.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FormLecture(FormLectureModel model)
        {
            try
            {
                model.FromHelpers = BindFormsPrefilledValues(ConservationFromType.FormLecture);
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
                        var submittedResponse = LectureBookingClient.PutLectureBookingAwardSubmission(new PutLectureBookingAwardSubmission()
                        {
                            channel = "W", //WebService
                            form = Convert.ToString((int)ConservationFromType.FormLecture),
                            formrequest = new formBookingRequest()
                            {
                                lecture = model.LectureDetails,
                                nursery = model.AcademiLevel,
                                typeform = model.LectureTypeDetails,
                                numberofattendencees = model.NumberOfAttendees,
                                lecturedate = model.DateOfLecture,
                                suitabletime = model.SuitableTime,
                                coordinatorname = model.CoordinateDetails.Name,
                                coordinatoremailaddress = model.CoordinateDetails.EmailAddress,
                                telephonenumber = model.CoordinateDetails.TelePhone,
                                coordinatormobilenumber = model.CoordinateDetails.ContactNo,
                                address = model.CoordinateDetails.Address,
                                organisationname = model.CoordinateDetails.Organization
                            },
                            conservationaward = new conservationAwardRequest(),
                            updatemode = "W" //write
                        }, Convert.ToString(AuthStateService?.GetActiveProfile()?.UserId), RequestLanguage, Request.Segment());

                        if (submittedResponse != null && submittedResponse.Succeeded)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CONSERVATION_LECTURE_SUCCESSPAGE);
                        }
                        else
                        {
                            ModelState.AddModelError("", submittedResponse.Message);
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
                ModelState.AddModelError(string.Empty, ex.Message);
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
            return View("~/Views/Feature/GeneralServices/Conservation/FormLecture.cshtml",model);
        }

        #endregion [Actions]

        #region [function]

        private ConservationFromHelpers BindFormsPrefilledValues(ConservationFromType fromType)
        {
            ConservationFromHelpers conservationFromHelpers = new ConservationFromHelpers();
            PutLectureBookingAwardSubmissionResponse data = null;

            if (!CacheProvider.TryGet($"cv.fromhelper{RequestLanguage}", out data))
            {
                var d = LectureBookingClient.PutLectureBookingAwardSubmission(new PutLectureBookingAwardSubmission()
                {
                    channel = "W", //WebService
                    form = Convert.ToString((int)ConservationFromType.FormLecture),
                    formrequest = new formBookingRequest(),
                    conservationaward = new conservationAwardRequest(),
                    updatemode = "R" //Read
                }, Convert.ToString(AuthStateService?.GetActiveProfile()?.UserId), RequestLanguage, Request.Segment());

                if (d != null && d.Succeeded)
                {
                    data = d.Payload;
                    CacheProvider.Store($"cv.fromhelper{RequestLanguage}", new CacheItem<PutLectureBookingAwardSubmissionResponse>(data, TimeSpan.FromHours(1)));
                }
            }

            //before
            switch (fromType)
            {
                case ConservationFromType.FormEducationalInstitution: break;
                case ConservationFromType.FormLeader: break;
                case ConservationFromType.FormLecture:
                    conservationFromHelpers.LecturList.Add(new SelectListItem() { Text = Translate.Text("FG_DDlDefault"), Value = "" });
                    conservationFromHelpers.AcademicList.Add(new SelectListItem() { Text = Translate.Text("FG_DDlDefault"), Value = "" });
                    conservationFromHelpers.LectureTypeList.Add(new SelectListItem() { Text = Translate.Text("FG_DDlDefault"), Value = "" });
                    break;

                case ConservationFromType.FormProject: break;
                case ConservationFromType.FormTeam: break;
            }
            if (data != null)
            {
                conservationFromHelpers.LecturList.AddRange(data.@return.formhelpvalues?.Where(x => x.fieldname == "LECTURE").Select(x => new SelectListItem() { Text = x.value, Value = x.valuekey })?.ToList().OrderBy(x => x.Text));
                conservationFromHelpers.AcademicList.AddRange(data.@return.formhelpvalues?.Where(x => x.fieldname == "NURSERY").Select(x => new SelectListItem() { Text = x.value, Value = x.valuekey })?.ToList().OrderBy(x => x.Text));
                conservationFromHelpers.LectureTypeList.AddRange(data.@return.formhelpvalues?.Where(x => x.fieldname == "FORMTYPE").Select(x => new SelectListItem() { Text = x.value, Value = x.valuekey })?.ToList().OrderBy(x => x.Text));
            }

            return conservationFromHelpers;
        }

        //{26405B07-B1DD-4AC9-81DC-968013DA6F1B}

        private ListDataSources GetSectorsList()
        {
            ListDataSources listDataSources = null;
            if (!CacheProvider.TryGet($"cv.sectionist{RequestLanguage}", out listDataSources))
            {
                listDataSources = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.Conservation_SectionList));
                if (listDataSources != null && Convert.ToBoolean(listDataSources?.Items?.Count() > 0))
                {
                    CacheProvider.Store($"cv.sectionist{RequestLanguage}", new CacheItem<ListDataSources>(listDataSources, TimeSpan.FromHours(1)));
                }
            }
            return listDataSources;
        }

        private ListDataSources GetAcademiclist(ConservationFromType conservationFromType)
        {
            ListDataSources listDataSources = null;
            if (!CacheProvider.TryGet($"cv.academiclist{RequestLanguage}", out listDataSources))
            {
                listDataSources = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.Conservation_AcademicList));
                if (listDataSources != null && Convert.ToBoolean(listDataSources?.Items?.Count() > 0))
                {
                    CacheProvider.Store($"cv.academiclist{RequestLanguage}", new CacheItem<ListDataSources>(listDataSources, TimeSpan.FromHours(1)));
                }
            }
            return listDataSources;
        }

        private formBookingRequest SubmitAcademicInFromRequest(List<string> selectedAcademic, formBookingRequest formRequest)
        {
            if (formRequest != null)
            {
                //var list = Enum.GetValues(typeof(ConservationAcademics))
                //.Cast<ConservationAcademics>()
                //.Select(v => v.ToString())
                //.ToList();
                foreach (string academic in selectedAcademic)
                {
                    ConservationAcademics academicVal = (ConservationAcademics)Convert.ToInt32(academic);
                    switch (academicVal)
                    {
                        case ConservationAcademics.Nursery:
                            formRequest.nursery = "X";
                            break;

                        case ConservationAcademics.Kindergarten:
                            formRequest.kindergarten = "X";
                            break;

                        case ConservationAcademics.Primary:
                            formRequest.primaryschool = "X";
                            break;

                        case ConservationAcademics.Elementary:
                            formRequest.elementary = "X";
                            break;

                        case ConservationAcademics.Intermediate:
                            formRequest.intermediate = "X";
                            break;

                        case ConservationAcademics.Secondary:
                            formRequest.secondary = "X";
                            break;

                        case ConservationAcademics.HighSchool:
                            formRequest.higherschool = "X";
                            break;

                        case ConservationAcademics.UniversityCollege:
                            formRequest.college_university = "X";
                            break;

                        case ConservationAcademics.SpecialNeedsCenter:
                            formRequest.pod_specialneeds = "X";
                            break;

                        case ConservationAcademics.AdultEducationCenters:
                            formRequest.educationcenter = "X";
                            break;
                    }
                }
            }

            return formRequest;
        }

        private conservationAwardRequest submitconservationAwardRequest(List<string> selectedParameters, conservationAwardRequest conservationAwardRequest)
        {
            if (conservationAwardRequest != null)
            {
                foreach (string item in selectedParameters)
                {
                    ConservationFormParameters formParamVal = new ConservationFormParameters();
                }
            }
            return conservationAwardRequest;
        }

        private List<CheckBoxListItem> GetAcademyList(ConservationFromType conservationFromType)
        {
            List<CheckBoxListItem> returnData = new List<CheckBoxListItem>();
            var d = GetAcademiclist(ConservationFromType.FormTeam).Items?.Select(x => new CheckBoxListItem()
            {
                Text = x.Text,
                Value = x.Value,
            }).ToList();

            if (d != null)
            {
                List<string> filter = new List<string>();
                filter.Add(Convert.ToString((int)ConservationAcademics.Kindergarten));
                filter.Add(Convert.ToString((int)ConservationAcademics.Elementary));
                filter.Add(Convert.ToString((int)ConservationAcademics.SpecialNeedsCenter));
                switch (conservationFromType)
                {
                    case ConservationFromType.FormEducationalInstitution:
                        filter.Add(Convert.ToString((int)ConservationAcademics.Primary));
                        filter.Add(Convert.ToString((int)ConservationAcademics.HighSchool));
                        filter.Add(Convert.ToString((int)ConservationAcademics.UniversityCollege));

                        break;

                    case ConservationFromType.FormLeader:
                        break;

                    case ConservationFromType.FormTeam:
                        filter.Add(Convert.ToString((int)ConservationAcademics.Primary));
                        break;

                    case ConservationFromType.FormProject:
                        break;

                    case ConservationFromType.FormLecture:
                        break;
                }
                returnData = d.Where(x => filter.Contains(x.Value)).ToList();
            }

            return returnData;
        }

        #endregion [function]

        #region GetList

        private IEnumerable<clearanceMaster> GetList(clearanceMaster[] list, string key)
        {
            return list.Where(x => !string.IsNullOrWhiteSpace(x.fieldname) && x.fieldname.ToLower().Equals(key.ToLower()));
        }

        #endregion GetList

        #region GetPreValuesData

        private clearanceMaster[] GetPreValuesData(string fromCode)
        {
            clearanceMaster[] responseData = null;
            if (!CacheProvider.TryGet($"cv.PreValuesData{fromCode}{RequestLanguage}", out responseData))
            {
                var response = DewaApiClient.GetConservationFieldMaster(string.Empty, fromCode, "00", RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null && response.Payload.clearancemasterlist != null && response.Payload.clearancemasterlist.Count() > 0)
                {
                    responseData = response.Payload.clearancemasterlist;
                    CacheProvider.Store($"cv.PreValuesData{fromCode}{RequestLanguage}", new CacheItem<clearanceMaster[]>(responseData, TimeSpan.FromHours(1)));
                }
            }

            return responseData;
        }

        #endregion GetPreValuesData

        #region GetcheckDesciption   // Technical and Awareness Parameters section value

        private string GetcheckDesciption(string key, List<string> keyList, List<string> descriptionList)
        {
            int i = 0;
            string desc = "";
            if (keyList != null && keyList.Any() && keyList.Count > 0)
            {
                foreach (var item in keyList)
                {
                    if (item == key)
                    {
                        desc = descriptionList[i];
                    }
                    i++;
                }
            }
            return desc;
        }

        #endregion GetcheckDesciption   // Technical and Awareness Parameters section value

        #region

        private formBookingRequest SubmitAcademicInEduRequest(List<string> selectedAcademic, formBookingRequest formRequest, string formCode)
        {
            if (formRequest != null)
            {
                foreach (string academic in selectedAcademic)
                {
                    if (formCode == "02")
                    {
                        ConservationEduAcademics academiceduVal = (ConservationEduAcademics)Convert.ToInt32(academic);
                        switch (academiceduVal)
                        {
                            case ConservationEduAcademics.Kindergarten:
                                formRequest.kindergarten = "X";
                                break;

                            case ConservationEduAcademics.Primary:
                                formRequest.primaryschool = "X";
                                break;

                            case ConservationEduAcademics.Elementary:
                                formRequest.elementary = "X";
                                break;

                            case ConservationEduAcademics.HighSchool:
                                formRequest.higherschool = "X";
                                break;

                            case ConservationEduAcademics.UniversityCollege:
                                formRequest.college_university = "X";
                                break;

                            case ConservationEduAcademics.SpecialNeedsCenter:
                                formRequest.pod_specialneeds = "X";
                                break;
                        }
                    }
                    else if (formCode == "04")
                    {
                        ConservationTeamAcademics academicteamVal = (ConservationTeamAcademics)Convert.ToInt32(academic);
                        switch (academicteamVal)
                        {
                            case ConservationTeamAcademics.Kindergarten:
                                formRequest.kindergarten = "X";
                                break;

                            case ConservationTeamAcademics.Primary:
                                formRequest.primaryschool = "X";
                                break;

                            case ConservationTeamAcademics.Elementary:
                                formRequest.elementary = "X";
                                break;

                            case ConservationTeamAcademics.SpecialNeedsCenter:
                                formRequest.pod_specialneeds = "X";
                                break;
                        }
                    }
                }
            }
            return formRequest;
        }

        private formBookingRequest SubmitMethodofEducationInEduRequest(List<string> selectedMethodofEducation, formBookingRequest formRequest, string formCode)
        {
            if (formRequest != null)
            {
                if (formCode == "02")
                {
                    foreach (string methodofeducation in selectedMethodofEducation)
                    {
                        ConservationEduMethodOfEducation methodofeducationVal = (ConservationEduMethodOfEducation)Convert.ToInt32(methodofeducation);
                        switch (methodofeducationVal)
                        {
                            case ConservationEduMethodOfEducation.VirutalLearning100:
                                formRequest.virtuallearning = "X";
                                break;

                            case ConservationEduMethodOfEducation.VirutalLearning50:
                                formRequest.hybridlearning = "X";
                                break;
                        }
                    }
                }
            }
            return formRequest;
        }

        #endregion
    }
}

public class CheckBoxListItem
{
    public string Text { get; set; }
    public string Value { get; set; }
    public bool Selected { get; set; }
}