using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Globalization;
using SitecoreX = Sitecore.Context;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content;

namespace DEWAXP.Feature.HR.Controllers
{
    public class CareerFairController : BaseController
    {

        // GET: CareerFair
        #region [Action]
        [HttpGet]
        public ActionResult NormalRegistration()
        {
            Models.CareerFair.NormalRegistrationDetail model = new Models.CareerFair.NormalRegistrationDetail();
            model.QualificationList = GetLstDataSource(DataSources.QUALIFICATIONS).ToList();
            model.SpecialisationList = GetLstDataSource(DataSources.SPECIALISATIONS).ToList();
            return View("~/Views/Feature/HR/CareerFair/NormalRegistration.cshtml",model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NormalRegistration(Models.CareerFair.NormalRegistrationDetail model)
        {
            #region [variable]
            string error = string.Empty;
            byte[] profileAttachment = new byte[0];
            #endregion

            if (ModelState.IsValid)
            {
                #region [file attachment handling]
                if (model.UploadedResume != null &&
               model.UploadedResume.ContentLength > 0 &&
               model.UploadedResume.ContentType.ToLower() == "application/pdf")
                {
                    if (!AttachmentIsValid(model.UploadedResume, General.MaxAttachmentSize, out error, General.AcceptedImageFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.UploadedResume.InputStream.CopyTo(memoryStream);
                            profileAttachment = memoryStream.ToArray();
                            //UploadedResumename = model.StudentPhotoUpload.FileName;
                            //UploadedResumetype = AttachmentHelper.GetMimeType(model.StudentPhotoUpload.GetTrimmedFileExtension());
                        }
                    }
                }
                #endregion

                #region [Date Formate & casting]
                if (!string.IsNullOrEmpty(model.DateOfBirth))
                {
                    CultureInfo culture = SitecoreX.Culture;
                    if (culture.ToString().Equals("ar-AE"))
                    {
                        model.DateOfBirth = model.DateOfBirth.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                    }
                    model.DateOfBirth = System.Convert.ToDateTime(model.DateOfBirth).ToString("yyyyMMdd");
                }
                #endregion
                var response = JobSeekerClient.SetCareerFairRegistration(new DEWAXP.Foundation.Integration.JobSeekerSvc.careerFairRegistrationDetails()
                {
                    applicantFirstName = model.ApplicantFirstName,
                    applicantMiddleName = model.ApplicantMiddleName,
                    applicantLastName = model.ApplicantLastName,
                    gender = model.Gender,
                    dateOfBirth = model.DateOfBirth,
                    emailaddress = model.Emailaddress,
                    emiratesID = model.EmiratesID,
                    mobileNumber = model.MobileNumber,
                    qualification = model.Qualification,
                    specialisation = model.Specialisation,
                    profile = profileAttachment,
                    others = model.Other,
                    university = model.University,
                    yearsofexperience = model.YearsOfExperience
                    
                });

                if (response != null && response.Succeeded)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CAREER_FAIR_NORMAL_REGISTRATIOON_SUCCESS);
                }

                TempData["ErrorMessage"] = response.Message;


            }
            model.QualificationList = GetLstDataSource(DataSources.QUALIFICATIONS).ToList();
            model.SpecialisationList = GetLstDataSource(DataSources.SPECIALISATIONS).ToList();

            return View("~/Views/Feature/HR/CareerFair/NormalRegistration.cshtml",model);
        }
        #endregion
    }
}