using DEWAXP.Feature.HR.Models.DewaAcademy;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.HR.Controllers
{
    public class DewaAcademyController : BaseController
    {
        private string errorKey = "errorMsg";

        #region Actions

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var x = FetchFutureCenterValues();

            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.DewaAcademy))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEWAACADEMY_DASHBOARD);
            }
            string error;

            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(errorKey, error);
            }

            ViewBag.ReturnUrl = returnUrl;

            return PartialView("~/Views/Feature/HR/DewaAcademy/_Login.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error, status;
                    if (TryLogin(model, out error, out status))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEWAACADEMY_DASHBOARD);
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEWAACADEMY_LOGIN);
        }

        [HttpGet]
        public ActionResult PersonalDetails()
        {
            User model = new User();

            //var result = DewaApiClient.GetServiceComplaintCriteria(RequestLanguage, Request.Segment());
            //if (result.Succeeded)
            //{
            //    model.CityList = DropdownHelper.CityDropdown(result.Payload.CityList);
            //}
            model.NationalityList = GetNationalities(DropDownnatioNalitiesValues);
            model.AcademicYearList = FormExtensions.GetYears(50, 0);
            model.EmirateList = GetEmirates();
            return View("~/Views/Feature/HR/DewaAcademy/PersonalDetails.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PersonalDetails(User model)
        {
            byte[] StudentPhotoUpload = new byte[0],
             PassportUpload = new byte[0],
             EmiratesIdUpload = new byte[0],
             FamilyBookUpload = new byte[0],
             AcademicCertificateUpload = new byte[0],
             CourcesTrainingCertificateUpload = new byte[0],
             CertificateIssuedAbroadUpload = new byte[0],
             BirthcertificateUpload = new byte[0];

            //byte[] PassportUpload = new byte[0];

            string StudentPhotoUploadname = "", StudentPhotoUploadtype = "",
                PassportUploadname = "", PassportUploadtype = "",
                EmiratesIdUploadname = "", EmiratesIdUploadtype = "",
                FamilyBookUploadname = "", FamilyBookUploadtype = "",
                AcademicCertificateUploadname = "", AcademicCertificateUploadtype = "",
                CourcesTrainingCertificateUploadname = "", CourcesTrainingCertificateUploadtype = "",
                CertificateIssuedAbroadUploadname = "", CertificateIssuedAbroadUploadtype = "",
            BirthcertificateUploadname = "", BirthcertificateUploadtype = "";
            string error = string.Empty;

            if (model.StudentPhotoUpload != null && model.StudentPhotoUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.StudentPhotoUpload, General.MaxAttachmentSize, out error, General.AcceptedImageFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.StudentPhotoUpload.InputStream.CopyTo(memoryStream);
                        StudentPhotoUpload = memoryStream.ToArray();
                        StudentPhotoUploadname = model.StudentPhotoUpload.FileName;
                        StudentPhotoUploadtype = AttachmentHelper.GetMimeType(model.StudentPhotoUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (model.PassportUpload != null && model.PassportUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.PassportUpload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.PassportUpload.InputStream.CopyTo(memoryStream);
                        PassportUpload = memoryStream.ToArray();
                        PassportUploadname = model.PassportUpload.FileName;
                        PassportUploadtype = AttachmentHelper.GetMimeType(model.PassportUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (model.EmiratesIdUpload != null && model.EmiratesIdUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.EmiratesIdUpload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.EmiratesIdUpload.InputStream.CopyTo(memoryStream);
                        EmiratesIdUpload = memoryStream.ToArray();
                        EmiratesIdUploadname = model.EmiratesIdUpload.FileName;
                        EmiratesIdUploadtype = AttachmentHelper.GetMimeType(model.EmiratesIdUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (model.FamilyBookUpload != null && model.FamilyBookUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.FamilyBookUpload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.FamilyBookUpload.InputStream.CopyTo(memoryStream);
                        FamilyBookUpload = memoryStream.ToArray();
                        FamilyBookUploadname = model.FamilyBookUpload.FileName;
                        FamilyBookUploadtype = AttachmentHelper.GetMimeType(model.FamilyBookUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (model.AcademicCertificateUpload != null && model.AcademicCertificateUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.AcademicCertificateUpload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.AcademicCertificateUpload.InputStream.CopyTo(memoryStream);
                        AcademicCertificateUpload = memoryStream.ToArray();
                        AcademicCertificateUploadname = model.AcademicCertificateUpload.FileName;
                        AcademicCertificateUploadtype = AttachmentHelper.GetMimeType(model.AcademicCertificateUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (model.CourcesTrainingCertificateUpload != null && model.CourcesTrainingCertificateUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.CourcesTrainingCertificateUpload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.CourcesTrainingCertificateUpload.InputStream.CopyTo(memoryStream);
                        CourcesTrainingCertificateUpload = memoryStream.ToArray();
                        CourcesTrainingCertificateUploadname = model.CourcesTrainingCertificateUpload.FileName;
                        CourcesTrainingCertificateUploadtype = AttachmentHelper.GetMimeType(model.CourcesTrainingCertificateUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (model.CertificateIssuedAbroadUpload != null && model.CertificateIssuedAbroadUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.CertificateIssuedAbroadUpload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.CertificateIssuedAbroadUpload.InputStream.CopyTo(memoryStream);
                        CertificateIssuedAbroadUpload = memoryStream.ToArray();
                        CertificateIssuedAbroadUploadname = model.CertificateIssuedAbroadUpload.FileName;
                        CertificateIssuedAbroadUploadtype = AttachmentHelper.GetMimeType(model.CertificateIssuedAbroadUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (model.BirthcertificateUpload != null && model.BirthcertificateUpload.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.BirthcertificateUpload, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(errorKey, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.BirthcertificateUpload.InputStream.CopyTo(memoryStream);
                        BirthcertificateUpload = memoryStream.ToArray();
                        BirthcertificateUploadname = model.BirthcertificateUpload.FileName;
                        BirthcertificateUploadtype = AttachmentHelper.GetMimeType(model.BirthcertificateUpload.GetTrimmedFileExtension());
                    }
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime dateTime;
                    DateTime familybookissueddate;
                    DateTime emiratesIDexpiredate, emiratesIDissueddate, passportexpirydate, passportissueddate;

                    CultureInfo culture = SitecoreX.Culture;
                    if (culture.ToString().Equals("ar-AE"))
                    {
                        model.DateOfBirth = model.DateOfBirth.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                        model.familybookissueddate = model.familybookissueddate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                        model.EmiratesIDexpiredate = model.EmiratesIDexpiredate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                        model.EmiratesIDissueddate = model.EmiratesIDissueddate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                        model.Passportexpirydate = model.Passportexpirydate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                        model.Passportissueddate = model.Passportissueddate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                    }
                    DateTime.TryParse(model.DateOfBirth, out dateTime);
                    DateTime.TryParse(model.familybookissueddate, out familybookissueddate);
                    DateTime.TryParse(model.EmiratesIDexpiredate, out emiratesIDexpiredate);
                    DateTime.TryParse(model.EmiratesIDissueddate, out emiratesIDissueddate);
                    DateTime.TryParse(model.Passportexpirydate, out passportexpirydate);
                    DateTime.TryParse(model.Passportissueddate, out passportissueddate);

                    var response = JobSeekerClient.SetAcademyRegistration(
                        model.ApplicantFirstName,
                        model.ApplicantLastName,
                        model.ApplicantMiddleName,
                        //model.FullNamear,
                        model.Nationality,
                        model.PassportNumber,
                        model.EmiratesIdNumber,
                        dateTime,
                        model.Birthplace,
                        StudentPhotoUpload,
                        StudentPhotoUploadname,
                        StudentPhotoUploadtype,
                        PassportUpload,
                        PassportUploadname,
                        PassportUploadtype,
                        EmiratesIdUpload,
                        EmiratesIdUploadname,
                        EmiratesIdUploadtype,
                        FamilyBookUpload,
                        FamilyBookUploadname,
                        FamilyBookUploadtype,
                        BirthcertificateUpload,
                        BirthcertificateUploadname,
                        BirthcertificateUploadtype,
                        model.EmailAddress,
                        model.MobileNumber,
                        model.AddressRoad,
                        model.Landmark, model.City,
                        model.NumberOfSiblings.ToString(),
                        model.SchoolName,
                        model.FinalPercentageGrade,
                        model.AcademicYear,
                       AcademicCertificateUpload,
                       AcademicCertificateUploadname,
                       AcademicCertificateUploadtype,
                       CourcesTrainingCertificateUpload,
                       CourcesTrainingCertificateUploadname,
                       CourcesTrainingCertificateUploadtype,
                        CertificateIssuedAbroadUpload,
                        CertificateIssuedAbroadUploadname,
                        CertificateIssuedAbroadUploadtype,
                        model.Password,
                        familybookissueddate,
                        model.familybooknumber,
                        model.familynumber,
                        model.fatherfirstname,
                        model.fatherlastname,
                        model.fathermiddlename,
                        model.fathermobilenumber,
                        model.motherfirstname,
                        model.motherlastname,
                        model.mothermiddlename,
                        model.mothermobilenumber,
                        model.postalcode,
                        emiratesIDexpiredate,
                        emiratesIDissueddate,
                        model.Idbaranumber,
                        passportexpirydate,
                        passportissueddate,
                        model.Placeofidentificationissued,
                        model.Secondarymobilenumber,
                        model.Telephonenumber,
                        model.Unifiednumber,
                        RequestLanguage, Request.Segment());

                    if (response.Succeeded)
                    {
                        CacheProvider.Store<string>(CacheKeys.Academy_Success, new CacheItem<string>(response.Message, TimeSpan.FromMinutes(100)));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEWAACADEMY_SUCCESS);
                    }
                    ModelState.AddModelError(errorKey, response.Message);
                    //return View(model);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    //return View(model);
                    ModelState.AddModelError(errorKey, Translate.Text("Unexpected error"));
                }
            }
            //var result = DewaApiClient.GetServiceComplaintCriteria(RequestLanguage, Request.Segment());
            //if (result.Succeeded)
            //{
            //    model.CityList = DropdownHelper.CityDropdown(result.Payload.CityList);
            //}
            model.NationalityList = GetNationalities(DropDownnatioNalitiesValues);
            model.AcademicYearList = FormExtensions.GetYears(50, 0);
            model.EmirateList = GetEmirates();
            return View("~/Views/Feature/HR/DewaAcademy/PersonalDetails.cshtml",model);
        }

        [HttpGet]
        public ActionResult Success()
        {
            string selection;
            if (CacheProvider.TryGet(CacheKeys.Academy_Success, out selection))
            {
                User Model = new User
                {
                    SuccessMessage = selection
                };
                return View("~/Views/Feature/HR/DewaAcademy/Success.cshtml", Model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEWAACADEMY_REGISTRATIONS);
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.DewaAcademy))
            {
                var status = CurrentPrincipal.SessionToken;
                User Model = new User
                {
                    Status = status
                };
                return View("~/Views/Feature/HR/DewaAcademy/Dashboard.cshtml", Model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEWAACADEMY_LOGIN);
        }

        #endregion Actions

        #region Methods

        protected List<SelectListItem> GetEmirates()
        {
            try
            {
                var emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates);

                var result = from itm in emirates.ToList()
                             select new SelectListItem()
                             {
                                 Text = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownEmirateValues),
                                 Value = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownEmirateValues)
                             };

                return result.ToList();
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                //ModelState.AddModelError(errorKey, Translate.Text("Unexpected error"));
                return null;
            }
        }

        public static List<SelectListItem> GetNationalities(Dictionary<string, string> dropDownTermValues, bool useShortValues = true, bool usedefault = false)
        {
            var source = "en".Equals(SitecoreX.Language.CultureInfo.TwoLetterISOLanguageName)
                ? ConfigurationManager.AppSettings["ENNationalitiesXML"] : ConfigurationManager.AppSettings["ARNationalitiesXML"];

            var list = new List<SelectListItem>();

            using (var ds = new DataSet())
            {
                ds.ReadXml(source);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Text = row[2].ToString().AddToTermDictionary(row[1].ToString(), dropDownTermValues),
                        Value = row[2].ToString().AddToTermDictionary(row[1].ToString(), dropDownTermValues),
                        Selected = !usedefault ? (row[1].ToString() == "AE" ? true : false) : false
                    });
                }
            }
            return list;
        }

        public Dictionary<string, string> DropDownEmirateValues
        {
            get
            {
                Dictionary<string, string> dictionary;
                if (!CacheProvider.TryGet("Emirates", out dictionary))
                {
                    dictionary = new Dictionary<string, string>();

                    CacheProvider.Store("Emirates", new CacheItem<Dictionary<string, string>>(dictionary));
                }
                return dictionary;
            }
        }

        private bool TryLogin(Login model, out string error, out string status)
        {
            error = null;
            status = null;
            var response = JobSeekerClient.GetValidateAcademyLogin(model.EmailAddress, model.Password, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                status = response.Payload.status;
                AuthStateService.Save(new DewaProfile(model.EmailAddress, status, Roles.DewaAcademy)
                {
                    TermsAndConditions = "X",
                    IsContactUpdated = true
                });
                return true;
            }
            error = response.Message;
            return false;
        }

        public Dictionary<string, string> DropDownnatioNalitiesValues
        {
            get
            {
                Dictionary<string, string> dictionary;
                if (!CacheProvider.TryGet(CacheKeys.TERMS, out dictionary))
                {
                    dictionary = new Dictionary<string, string>();

                    CacheProvider.Store(CacheKeys.TERMS, new CacheItem<Dictionary<string, string>>(dictionary));
                }
                return dictionary;
            }
        }

        //unused method
        //private byte[] ReadImage(HttpPostedFileBase upload)
        //{
        //    byte[] imgData;

        //    using (Stream inputStream = upload.InputStream)
        //    {
        //        var memoryStream = inputStream as MemoryStream;
        //        if (memoryStream == null)
        //        {
        //            memoryStream = new MemoryStream();
        //            inputStream.CopyTo(memoryStream);
        //        }

        //        imgData = memoryStream.ToArray();
        //    }

        //    return imgData;
        //}

        #endregion Methods
    }
}