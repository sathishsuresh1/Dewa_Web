// <copyright file="ShamsDubaiTrainingController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>
using DEWAXP.Feature.ShamsDubai.Models.ShamsDubaiTraining;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.SmartConsultant;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Sitecorex = global::Sitecore.Context;

namespace DEWAXP.Feature.ShamsDubai.Controllers
{
    /// <summary>
    /// Defines the <see cref="ShamsDubaiTrainingController" />.
    /// </summary>
    public class ShamsDubaiTrainingController : BaseController
    {
        /// <summary>
        /// The DisplayTraining.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult DisplayTraining()
        {
            List<DisplayTraining> displayTrainings = GetDisplayTrainingList();
            return View("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/DisplayTraining.cshtml", displayTrainings);
        }

        #region BookTraining

        [HttpGet]
        public ActionResult BookTraining(long? t = 0)
        {
            BookTrainingModel model = new BookTrainingModel();

            // Captcha
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            if (t.HasValue && t.Value > 0)
            {
                var trainingdetail = GetDisplayTrainingList().FirstOrDefault(x => x.TrainingId == t);
                if (trainingdetail != null)
                {
                    model.TrainingName = trainingdetail.TrainingDescription;
                    model.TrainingDuration = trainingdetail.TrainingDate;
                    model.TraningStartDate = DateTime.Parse(trainingdetail.TraningStartDate);
                    model.TraningEndDate = DateTime.Parse(trainingdetail.TraningEndDate);

                    model.TrainingId = t.Value;
                    model.IsSuccess = true;

                    model.DesignationList = GetLstDataSource(DataSources.SHAMS_DUBAI_DESIGNATION).ToList();
                    model.ReasonforEnrollmentList = GetLstDataSource(DataSources.SHAMS_DUBAI_ENROLLMENT).ToList();
                    model.ElectricalSystemList = GetLstDataSource(DataSources.SHAMS_DUBAI_ELECTRICALSYSTEM).ToList();
                    model.PVDesignList = GetLstDataSource(DataSources.SHAMS_DUBAI_PVDESIGN).ToList();
                    model.SolarPVExpertList = GetLstDataSource(DataSources.SHAMS_DUBAI_SOLARPVEXPERT).ToList();
                    return View("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/BookTraining.cshtml",model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_SOLAR_PV_DISPLAYING);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BookTraining(BookTrainingModel bookTrainingModel)
        {
            try
            {
                // Captcha
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
                    var trainingdetail = GetDisplayTrainingList().FirstOrDefault(x => x.TrainingId == bookTrainingModel.TrainingId);
                    if (trainingdetail == null)
                    {
                        status = false;
                        ModelState.AddModelError(string.Empty, Translate.Text("PVCC training detail absent"));
                    }

                    if (status)
                    {
                        bookTrainingModel.TrainingName = trainingdetail.TrainingDescription;
                        bookTrainingModel.TrainingDuration = trainingdetail.TrainingDate;
                        bookTrainingModel.TraningStartDate = DateTime.Parse(trainingdetail.TraningStartDate);
                        bookTrainingModel.TraningEndDate = DateTime.Parse(trainingdetail.TraningEndDate);

                        UpdateConsultantTrainingRequest updateTraning = new UpdateConsultantTrainingRequest();
                        UpdateConsultantTrainingAttachRequest attachTraining = new UpdateConsultantTrainingAttachRequest();
                        bookTrainingModel.IsSuccess = true;
                        updateTraning.trainingdetails = new TrainingDetails()
                        {
                            // Applicant details
                            //-- step 1 --//
                            trainingid = bookTrainingModel.TrainingId,
                            trainingname = bookTrainingModel.TrainingName,
                            trainingduration = 1,
                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                            applicantname = bookTrainingModel.ApplicationName,
                            emailaddress = bookTrainingModel.Email,
                            mobilenumber = Convert.ToInt64(bookTrainingModel.Mobile),
                            certificatenumber = bookTrainingModel.PVCCCertificateNumber,
                            reasonforenroll = bookTrainingModel.ReasonforEnrollment,
                            countryname = bookTrainingModel.Nationality,
                            departmenttext = bookTrainingModel.Department,
                            designation = bookTrainingModel.Designation,
                            solarpvexpert = bookTrainingModel.SolarPVExpert.ToUpper(),

                            //-- step 2 --//
                            visanumber = bookTrainingModel.VisaNumber,
                            passportnumber = bookTrainingModel.PassportNumber,
                            designesexp = bookTrainingModel.ElectricalSystem,
                            designpvexp = bookTrainingModel.PVDesign,
                            shamsexp = bookTrainingModel.DescforShamsProject,
                            additionalnote = bookTrainingModel.DescforAdditional,

                            // Company details
                            //-- step 3 --//
                            companyname = bookTrainingModel.companyDetails.CompanyName,
                            tradelicense = bookTrainingModel.companyDetails.CompanyTradeLicenceNumber,     //Convert.ToInt64(bookTrainingModel.companyDetails.CompanyTradeLicenceNumber),
                            vatnumber = bookTrainingModel.companyDetails.VATRegistrationNumber,          //Convert.ToInt64(bookTrainingModel.companyDetails.VATRegistrationNumber),
                            companydescription = bookTrainingModel.companyDetails.CompanyActivityDescription,
                            companyemail = bookTrainingModel.companyDetails.CompanyEmail,
                            companymobile = Convert.ToInt64(bookTrainingModel.companyDetails.CompanyMobile),
                            companycontactperson = bookTrainingModel.companyDetails.ContactPersonName,
                        };

                        // date fields
                        if (bookTrainingModel.TraningStartDate.HasValue)
                        {
                            updateTraning.trainingdetails.startdate = Convert.ToInt64(bookTrainingModel.TraningStartDate.Value.ToString("yyyyMMdd"));
                        }
                        if (bookTrainingModel.TraningEndDate.HasValue)
                        {
                            updateTraning.trainingdetails.enddate = Convert.ToInt64(bookTrainingModel.TraningEndDate.Value.ToString("yyyyMMdd"));
                        }
                        if (bookTrainingModel.VisaIssueDate != null)
                        {
                            updateTraning.trainingdetails.visaissuedate = Convert.ToInt64(bookTrainingModel.VisaIssueDate.ToString("yyyyMMdd"));
                        }
                        if (bookTrainingModel.VisaExpiryDate != null)
                        {
                            updateTraning.trainingdetails.visavaliditydate = Convert.ToInt64(bookTrainingModel.VisaExpiryDate.ToString("yyyyMMdd"));
                        }
                        if (bookTrainingModel.PassportIssueDate != null)
                        {
                            updateTraning.trainingdetails.passportissuedate = Convert.ToInt64(bookTrainingModel.PassportIssueDate.ToString("yyyyMMdd"));
                        }
                        if (bookTrainingModel.PassportExpiryDate != null)
                        {
                            updateTraning.trainingdetails.passportexpirydate = Convert.ToInt64(bookTrainingModel.PassportExpiryDate.ToString("yyyyMMdd"));
                        }
                        if (bookTrainingModel.companyDetails.LicenseIssueDate != null)
                        {
                            updateTraning.trainingdetails.licenseissuedate = Convert.ToInt64(bookTrainingModel.companyDetails.LicenseIssueDate.ToString("yyyyMMdd"));
                        }
                        if (bookTrainingModel.companyDetails.LicenseExpiryDate != null)
                        {
                            updateTraning.trainingdetails.licenseexpirydate = Convert.ToInt64(bookTrainingModel.companyDetails.LicenseExpiryDate.ToString("yyyyMMdd"));
                        }

                        var response = SmartConsultantClient.BookingPVCCTraining(updateTraning, RequestLanguage, Request.Segment());

                        if (response.Succeeded && response.Payload != null)
                        {
                            bookTrainingModel.RequestNumber = response.Payload.Requestnumber ?? string.Empty;
                            // Attachment
                            List<TrainingAttachmentDetails> trainingAttachmentDetails = new List<TrainingAttachmentDetails>();
                            string error = string.Empty;

                            // Applicant’s Experience Certificates (attested)
                            if (bookTrainingModel.PV1_ExperienceCertificate != null && bookTrainingModel.PV1_ExperienceCertificate.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV1_ExperienceCertificate, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_1 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV1_ExperienceCertificate.InputStream.CopyTo(memoryStream_1);
                                        bookTrainingModel.PV1_ExperienceCertificate_FileBinary = memoryStream_1.ToArray();
                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV1",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV1_ExperienceCertificate_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV1_ExperienceCertificate.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV1_ExperienceCertificate.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV1_ExperienceCertificate.ContentType // bookTrainingModel.PV1_ExperienceCertificate.FileName.GetFileExtensionTrimmed(),
                                        });
                                    }
                                }
                            }

                            // PVCC Enrollment form with Company Seal
                            if (bookTrainingModel.PV2_CompanySeal != null && bookTrainingModel.PV2_CompanySeal.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV2_CompanySeal, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_2 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV2_CompanySeal.InputStream.CopyTo(memoryStream_2);
                                        bookTrainingModel.PV2_CompanySeal_FileBinary = memoryStream_2.ToArray();

                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV2",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV2_CompanySeal_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV2_CompanySeal.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV2_CompanySeal.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV2_CompanySeal.ContentType,
                                        });
                                    }
                                }
                            }

                            // Company Trade License
                            if (bookTrainingModel.PV3_TradeLicense != null && bookTrainingModel.PV3_TradeLicense.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV3_TradeLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_3 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV3_TradeLicense.InputStream.CopyTo(memoryStream_3);
                                        bookTrainingModel.PV3_TradeLicense_FileBinary = memoryStream_3.ToArray();

                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV3",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV3_TradeLicense_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV3_TradeLicense.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV3_TradeLicense.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV3_TradeLicense.ContentType,
                                        });
                                    }
                                }
                            }

                            // Applicant’s degree (attested)
                            if (bookTrainingModel.PV4_ApplicantDegree != null && bookTrainingModel.PV4_ApplicantDegree.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV4_ApplicantDegree, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_4 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV4_ApplicantDegree.InputStream.CopyTo(memoryStream_4);
                                        bookTrainingModel.PV4_ApplicantDegree_FileBinary = memoryStream_4.ToArray();

                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV4",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV4_ApplicantDegree_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV4_ApplicantDegree.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV4_ApplicantDegree.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV4_ApplicantDegree.ContentType,
                                        });
                                    }
                                }
                            }

                            // Applicant’s Passport (attested)
                            if (bookTrainingModel.PV5_ApplicantPassport != null && bookTrainingModel.PV5_ApplicantPassport.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV5_ApplicantPassport, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_5 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV5_ApplicantPassport.InputStream.CopyTo(memoryStream_5);
                                        bookTrainingModel.PV5_ApplicantPassport_FileBinary = memoryStream_5.ToArray();

                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV5",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV5_ApplicantPassport_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV5_ApplicantPassport.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV5_ApplicantPassport.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV5_ApplicantPassport.ContentType,
                                        });
                                    }
                                }
                            }

                            // Applicant’s Visa (attested)
                            if (bookTrainingModel.PV6_ApplicantEmiratesID != null && bookTrainingModel.PV6_ApplicantEmiratesID.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV6_ApplicantEmiratesID, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_6 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV6_ApplicantEmiratesID.InputStream.CopyTo(memoryStream_6);
                                        bookTrainingModel.PV6_ApplicantEmiratesID_FileBinary = memoryStream_6.ToArray();

                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV6",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV6_ApplicantEmiratesID_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV6_ApplicantEmiratesID.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV6_ApplicantEmiratesID.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV6_ApplicantEmiratesID.ContentType,
                                        });
                                    }
                                }
                            }

                            // Others (if any)
                            if (bookTrainingModel.PV7_Others != null && bookTrainingModel.PV7_Others.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV7_Others, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_7 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV7_Others.InputStream.CopyTo(memoryStream_7);
                                        bookTrainingModel.PV7_Others_FileBinary = memoryStream_7.ToArray();

                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV7",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV7_Others_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV7_Others.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV7_Others.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV7_Others.ContentType,
                                        });
                                    }
                                }
                            }

                            // Applicant's photo
                            if (bookTrainingModel.PV8_ApplicantPhoto != null && bookTrainingModel.PV8_ApplicantPhoto.ContentLength > 0)
                            {
                                if (!AttachmentIsValid(bookTrainingModel.PV8_ApplicantPhoto, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                                {
                                    bookTrainingModel.IsSuccess = false;
                                    ModelState.AddModelError(string.Empty, error);
                                }
                                else
                                {
                                    using (var memoryStream_8 = new MemoryStream())
                                    {
                                        bookTrainingModel.PV8_ApplicantPhoto.InputStream.CopyTo(memoryStream_8);
                                        bookTrainingModel.PV8_ApplicantPhoto_FileBinary = memoryStream_8.ToArray();

                                        trainingAttachmentDetails.Add(new TrainingAttachmentDetails()
                                        {
                                            documenttype = "PV8",
                                            emiratesid = Convert.ToInt64(bookTrainingModel.EmiratesID),
                                            filecontent = Convert.ToBase64String(bookTrainingModel.PV8_ApplicantPhoto_FileBinary ?? new byte[0]),
                                            filesize = Convert.ToInt64(bookTrainingModel.PV8_ApplicantPhoto.ContentLength),
                                            trainingid = Convert.ToInt64(bookTrainingModel.TrainingId),
                                            reqnumber = bookTrainingModel.RequestNumber,
                                            filename = bookTrainingModel.PV8_ApplicantPhoto.FileName?.ToUpper(),
                                            mimetype = bookTrainingModel.PV8_ApplicantPhoto.ContentType,
                                        });
                                    }
                                }
                            }
                            attachTraining.trainingattachmentDetails = trainingAttachmentDetails.ToList();

                            var attachresponse = SmartConsultantClient.BookingPVCCTrainingAttach(attachTraining, RequestLanguage, Request.Segment());
                            if (attachresponse != null && attachresponse.Succeeded && attachresponse.Payload != null)
                            {
                                CacheProvider.Store("BookTrainingModel", new CacheItem<BookTrainingModel>(bookTrainingModel));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_BOOK_TRAINING_SUCCESS);
                            }
                            else
                            {
                                bookTrainingModel.IsSuccess = false;
                                ModelState.AddModelError(string.Empty, response.Message);
                            }
                        }
                        else
                        {
                            bookTrainingModel.IsSuccess = false;
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                bookTrainingModel.IsSuccess = false;
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            bookTrainingModel.DesignationList = GetLstDataSource(DataSources.SHAMS_DUBAI_DESIGNATION).ToList();
            bookTrainingModel.ReasonforEnrollmentList = GetLstDataSource(DataSources.SHAMS_DUBAI_ENROLLMENT).ToList();
            bookTrainingModel.ElectricalSystemList = GetLstDataSource(DataSources.SHAMS_DUBAI_ELECTRICALSYSTEM).ToList();
            bookTrainingModel.PVDesignList = GetLstDataSource(DataSources.SHAMS_DUBAI_PVDESIGN).ToList();
            bookTrainingModel.SolarPVExpertList = GetLstDataSource(DataSources.SHAMS_DUBAI_SOLARPVEXPERT).ToList();

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }

            return View("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/BookTraining.cshtml",bookTrainingModel);
        }

        #endregion BookTraining

        #region BookingSuccess

        [HttpGet]
        public ActionResult BookingSuccess()
        {
            BookTrainingModel bookTrainingModel = null;
            if (CacheProvider.TryGet("BookTrainingModel", out bookTrainingModel))
            {
                CacheProvider.Remove("SolarPVCCModel");
                CacheProvider.Remove("BookTrainingModel");
                return PartialView("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/BookingSuccess.cshtml", new BookTrainingModel()
                {
                    TrainingId = bookTrainingModel.TrainingId,
                    TrainingName = bookTrainingModel.TrainingName,
                    RequestNumber = bookTrainingModel.RequestNumber,
                    TraningStartDate = bookTrainingModel.TraningStartDate,
                    TraningEndDate = bookTrainingModel.TraningEndDate,
                    IsSuccess = true
                });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SHAMS_BOOK_TRAINING);
        }

        #endregion BookingSuccess

        #region ApplicationTracking

        [HttpGet]
        public ActionResult ApplicationTracking()
        {
            ApplicationTrackingModel model = new ApplicationTrackingModel();
            return View("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/ApplicationTracking.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplicationTracking(ApplicationTrackingModel model)
        {
            List<ApplicationTrackResult> _trackResult = new List<ApplicationTrackResult>();
            try
            {
                TrainingBookingDetailsRequest trainingBookingDetailsRequest = new TrainingBookingDetailsRequest()
                {
                    requestnumber = model.RequestNumber ?? string.Empty,
                    emiratesid = model.EmiratesID ?? string.Empty
                };

                var response = SmartConsultantClient.TrainingBookingDetails(trainingBookingDetailsRequest, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    _trackResult.Add(new ApplicationTrackResult()
                    {
                        Applicationname = response.Payload.applicantname ?? string.Empty,
                        Certificate = response.Payload.certificate ?? string.Empty,
                        Certificatenumber = response.Payload.certificatenumber ?? string.Empty,
                        CompanycontactPerson = response.Payload.companycontactperson ?? string.Empty,
                        Companydescription = response.Payload.companydescription ?? string.Empty,
                        Companyemail = response.Payload.companyemail ?? string.Empty,
                        Companymobile = response.Payload.companymobile ?? string.Empty,
                        Companyname = response.Payload.companyname ?? string.Empty,
                        Countryname = response.Payload.countryname ?? string.Empty,
                        Departmenttext = response.Payload.departmenttext ?? string.Empty,
                        Description = response.Payload.description ?? string.Empty,
                        Designation = response.Payload.designation ?? string.Empty,
                        Designesexp = response.Payload.designesexp ?? string.Empty,
                        Designpvexp = response.Payload.designpvexp ?? string.Empty,
                        Emailaddress = response.Payload.emailaddress ?? string.Empty,
                        EmiratesId = response.Payload.emiratesid ?? string.Empty,
                        Enddate = !string.IsNullOrWhiteSpace(response.Payload.enddate) && !response.Payload.enddate.Equals("0000-00-00") ? DateTime.ParseExact(response.Payload.enddate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) : string.Empty,
                        Licenseexpirydate = "",//DateTime.ParseExact(response.Payload.licenseexpirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) ?? string.Empty,
                        Licenseissuedate = "", // DateTime.ParseExact(response.Payload.licenseissuedate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) ?? string.Empty,
                        Mobilenumber = response.Payload.mobilenumber ?? string.Empty,
                        Passportexpirydate = !string.IsNullOrWhiteSpace(response.Payload.passportexpirydate) && !response.Payload.passportexpirydate.Equals("0000-00-00") ? DateTime.ParseExact(response.Payload.passportexpirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) : string.Empty,
                        Passportissuedate = !string.IsNullOrWhiteSpace(response.Payload.passportissuedate) && !response.Payload.passportissuedate.Equals("0000-00-00") ? DateTime.ParseExact(response.Payload.passportissuedate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) : string.Empty,
                        Passportnumber = response.Payload.passportnumber ?? string.Empty,
                        Reasonforenroll = response.Payload.reasonforenroll ?? string.Empty,
                        Responsecode = response.Payload.responsecode ?? string.Empty,
                        Shamsexp = response.Payload.shamsexp ?? string.Empty,
                        Startdate = !string.IsNullOrWhiteSpace(response.Payload.startdate) && !response.Payload.startdate.Equals("0000-00-00") ? DateTime.ParseExact(response.Payload.startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) : string.Empty,
                        Tradelicense = response.Payload.tradelicense ?? string.Empty,
                        Trainingduration = response.Payload.trainingduration ?? string.Empty,
                        TrainingId = response.Payload.trainingid ?? string.Empty,
                        Trainingname = response.Payload.trainingname ?? string.Empty,
                        Vatnumber = response.Payload.vatnumber ?? string.Empty,
                        Visaissuedate = !string.IsNullOrWhiteSpace(response.Payload.visaissuedate) && !response.Payload.visaissuedate.Equals("0000-00-00") ? DateTime.ParseExact(response.Payload.visaissuedate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) : string.Empty,
                        Visanumber = response.Payload.visanumber ?? string.Empty,
                        Visavaliditydate = !string.IsNullOrWhiteSpace(response.Payload.visavaliditydate) && !response.Payload.visavaliditydate.Equals("0000-00-00") ? DateTime.ParseExact(response.Payload.visavaliditydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) : string.Empty,
                        Applicationstatus = response.Payload.applicationstatus ?? string.Empty
                    });
                    model.AppTrackResultList = _trackResult.ToList();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/ApplicationTrackingList.cshtml", model);
        }

        #endregion ApplicationTracking

        #region CertificateVerification

        [HttpGet]
        public ActionResult CertificateVerification(string ceno, string eino)
        {
            string _ceNo = string.Empty;
            string _eiNo = string.Empty;

            CertificateVerifyingModel verifyingModel = new CertificateVerifyingModel();

            if (!string.IsNullOrWhiteSpace(ceno))
            {
                _ceNo = Base64Decode(ceno);
            }
            if (!string.IsNullOrWhiteSpace(eino))
            {
                _eiNo = Base64Decode(eino);
            }
            verifyingModel.CertificateNumber = _ceNo;
            verifyingModel.EmiratesID = _eiNo;

            return View("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/CertificateVerification.cshtml",verifyingModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CertificateVerification(CertificateVerifyingModel model)
        {
            List<CertificateVerifyResult> _verifyResult = new List<CertificateVerifyResult>();
            try
            {
                PVCCCertificateDetailsRequest pvccCertificateDetailsRequest = new PVCCCertificateDetailsRequest()
                {
                    requestnumber = string.Empty,
                    certificatenumber = model.CertificateNumber ?? string.Empty,
                    emiratesid = model.EmiratesID ?? string.Empty
                };

                var response = SmartConsultantClient.PVCCCertificateDetails(pvccCertificateDetailsRequest, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    _verifyResult.Add(new CertificateVerifyResult()
                    {
                        Applicantname = response.Payload.applicantname ?? string.Empty,
                        Companyname = response.Payload.companyname ?? string.Empty,
                        Certificatecategory = response.Payload.certificatecategory ?? string.Empty,
                        Certificatecontent = response.Payload.certificatecontent ?? string.Empty,
                        Certificateexpirydate = DateTime.ParseExact(response.Payload.certificateexpirydate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) ?? string.Empty,
                        Certificateissued = response.Payload.certificateissued ?? string.Empty,
                        Certificateissuedate = DateTime.ParseExact(response.Payload.certificateissuedate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) ?? string.Empty,
                        Certificatenumber = response.Payload.certificatenumber ?? string.Empty,
                        Certificatestatus = response.Payload.certificatestatus ?? string.Empty,
                        Description = response.Payload.description ?? string.Empty,
                        Emiratesid = response.Payload.emiratesid ?? string.Empty,
                        Enddate = DateTime.ParseExact(response.Payload.enddate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) ?? string.Empty,
                        Requestnumber = response.Payload.requestnumber ?? string.Empty,
                        Responsecode = response.Payload.responsecode ?? string.Empty,
                        Startdate = DateTime.ParseExact(response.Payload.startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) ?? string.Empty,
                        Trainingid = response.Payload.trainingid ?? string.Empty
                    });
                    model.CertiVerifyResultList = _verifyResult.ToList();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/ShamsDubai/ShamsDubaiTraining/CertificateVerificationList.cshtml", model);
        }

        #endregion CertificateVerification

        #region Base64Decode

        //public static string Base64Decode(string base64EncodedData)
        //{
        //    try
        //    {
        //        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        //        return Encoding.UTF8.GetString(base64EncodedBytes);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Error(ex, ex.Message);
        //        return string.Empty;
        //    }
        //}

        #endregion Base64Decode

        #region GetDetails

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetDetails(string eidnumber, string otpnumber)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(eidnumber))
                {
                    PVCCEmiratesidVerifyRequest pvccEmiratesidVerifyRequest = new PVCCEmiratesidVerifyRequest()
                    {
                        emiratesid = eidnumber ?? string.Empty,
                        OTP = otpnumber ?? string.Empty
                    };

                    var response = SmartConsultantClient.PVCCEmiratesidVerify(pvccEmiratesidVerifyRequest, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        return Json(new { Message = string.Empty, JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        return Json(new { Message = response.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return null;
        }

        #endregion GetDetails

        #region [function]

        private List<DisplayTraining> GetDisplayTrainingList()
        {
            List<DisplayTraining> displayTrainings = null;
            if (!CacheProvider.TryGet("SolarPVCCModel" + Translate.Text("lang"), out displayTrainings))
            {
                displayTrainings = new List<DisplayTraining>();
                ServiceResponse<List<TrainingDetail>> response = SmartConsultantClient.DisplayPVCCTraining(RequestLanguage);
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.Count > 0)
                {
                    response.Payload.Where(y => !string.IsNullOrWhiteSpace(y.Startdate)).ToList().ForEach(x => displayTrainings.Add(new DisplayTraining
                    {
                        TrainingDescription = x.Shorttextname,
                        TrainingId = x.Objectid,
                        TrainingYear = DateTime.ParseExact(x.Startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).Year,
                        TrainingDate = !string.IsNullOrWhiteSpace(x.Startdate) && !string.IsNullOrWhiteSpace(x.Enddate) ? (x.Startdate.Equals(x.Enddate) ?
                         DateTime.ParseExact(x.Startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture)
                         : DateTime.ParseExact(x.Startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture) + " - " + DateTime.ParseExact(x.Enddate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd MMM yyyy", Sitecorex.Culture)) : string.Empty,
                        TraningStartDate = x.Startdate,
                        TraningEndDate = x.Enddate
                    }));
                    CacheProvider.Store("SolarPVCCModel" + Translate.Text("lang"), new CacheItem<List<DisplayTraining>>(displayTrainings, TimeSpan.FromDays(1)));
                }
            }
            return displayTrainings;
        }

        #endregion [function]
    }
}