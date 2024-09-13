// <copyright file="WorkPermitController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Controllers.Epass
{
    using DEWAXP.Feature.GatePass.Models.ePass;
    using DEWAXP.Feature.GatePass.Models.WorkPermit;
    using DEWAXP.Feature.GatePass.Utils;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Requests.SmartVendor.WorkPermit;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.SmartVendor.WorkPermit;
    using DEWAXP.Foundation.Integration.Responses.VendorSvc;
    using DEWAXP.Foundation.Integration.SmartVendorSvc;
    using DEWAXP.Foundation.Logger;
    using global::Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using X.PagedList;
    using Sitecorex = global::Sitecore;

    /// <summary>
    /// Defines the <see cref="WorkPermitController" />.
    /// </summary>
    public class WorkPermitController : EpassBaseController
    {
        /// <summary>
        /// The ApplyWorkPermit.
        /// </summary>
        /// <param name="passNo">The passNo<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ApplyWorkPermit(string passNo = null)
        {
            WorkPermitPass model = new WorkPermitPass();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out string error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            if (CacheProvider.TryGet(CacheKeys.WORKPERMIT_PASSREQUEST, out WorkPermitPass model1))
            {
                model = model1;
            }
            WPDefaultValues(model);

            return PartialView("~/Views/Feature/GatePass/WorkPermit/ApplyWorkPermit.cshtml", model);
        }

        private void WPDefaultValues(WorkPermitPass model)
        {
            model.LocationList = GetLocationList();
            if (!CacheProvider.TryGet(CacheKeys.WORK_PERMIT_COUNTRYLIST, out IEnumerable<SelectListItem> countrylist))
            {
                countrylist = GetWPCountryList();
            }
            model.NationalityList = countrylist;
            ServiceResponse<subContractorResponse> contractorlistresponse = GetWPSubContractorList();
            if (contractorlistresponse != null && contractorlistresponse.Succeeded && contractorlistresponse.Payload != null && contractorlistresponse.Payload.subcontractordetails != null && contractorlistresponse.Payload.subcontractordetails.Count() > 0)
            {
                model.SubcontractorList = contractorlistresponse.Payload.subcontractordetails.ToList();
            }
            model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
            model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
            model.EmirateOrCountry = "DXB";
            model.PlateCategory = GetDetailForCatOrCode("", false);
        }

        /// <summary>
        /// The ApplyWorkPermit.
        /// </summary>
        /// <param name="model">The model<see cref="WorkPermitPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyWorkPermit(WorkPermitPass model)
        {
            try
            {
                if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
                }
                if (model.PassSubmitType.Equals("single"))
                {
                    string error;
                    List<Attachmentlist> lstattachmentDetails = new List<Attachmentlist>();
                    List<Grouppasslocationlist> lstlocPassDetails = new List<Grouppasslocationlist>();
                    if (model.SinglePass_Photo != null && model.SinglePass_Photo.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Photo, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsApplicantphoto = true;
                                model.SinglePass_Photo.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Photo.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Photo.ContentType,
                                    folderid = AttachmentTypes.Applicant_Photo
                                });
                            }
                        }
                    }
                    if (model.SinglePass_EmiratesID != null && model.SinglePass_EmiratesID.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_EmiratesID, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsEmiratesid = true;
                                model.SinglePass_EmiratesID.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_EmiratesID.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_EmiratesID.ContentType,
                                    folderid = AttachmentTypes.EmiratesID
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Visa != null && model.SinglePass_Visa.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Visa, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Isvisadocument = true;
                                model.SinglePass_Visa.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Visa.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Visa.ContentType,
                                    folderid = AttachmentTypes.VisaDocument
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Passport != null && model.SinglePass_Passport.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Passport, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Ispassportdocument = true;
                                model.SinglePass_Passport.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Passport.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Passport.ContentType,
                                    folderid = AttachmentTypes.PassportDocument
                                });
                            }
                        }
                    }
                    if (model.withcar && model.DrivingLicense != null && model.DrivingLicense.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.DrivingLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsDrivingLicense = true;
                                model.DrivingLicense.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.DrivingLicense.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.DrivingLicense.ContentType,
                                    folderid = AttachmentTypes.DrivingLicense
                                });
                            }
                        }
                    }
                    if (model.SelectedLocation != null && model.SelectedLocation.Count() > 0)
                    {
                        Array.ForEach(model.SelectedLocation.ToArray(), x => lstlocPassDetails.Add(new Grouppasslocationlist { locationcode = x.ToString() }));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>("Select Location", Times.Once));
                        ModelState.AddModelError("SelectedLocation", Translate.Text("wp.select the location"));
                    }
                    if (model.EmiratesIDExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.EmiratesIDExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"));
                    }
                    //Single/Group Epass Validation for Passport Expiry  and Visa date 
                    if (model.PassportExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.PassportExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Passport expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Passport expiry date"));
                    }
                    if (model.VisaExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.VisaExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Visa expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Visa expiry date"));
                    }
                    if (!model.PassExpiryDate.HasValue || !model.PassIssueDate.HasValue)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date and Pass Issue date should not be empty"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date and Pass Issue date should not be empty"));
                    }
                    if (ModelState.IsValid)
                    {
                        var response = SmartVendorClient.SubmitWorkPermitPass(
                            new GroupPemitPassRequest
                            {
                                grouppassinput = new Grouppassinput
                                {
                                    processcode = "SAV",
                                    save = "S",
                                    type = "S",
                                    lang = RequestLanguage.ToString(),
                                    sessionid = CurrentPrincipal.SessionToken,
                                    userid = CurrentPrincipal.UserId,
                                    attachmentlist = lstattachmentDetails.ToArray(),
                                    grouppasslocationlist = lstlocPassDetails.ToArray(),
                                    projectcoordinatordetails = new Projectcoordinatordetails
                                    {
                                        fullname = model.Projectcoordinatorname,
                                        emailid = model.ProjectCoordinatorEmailaddress,
                                        mobile = !string.IsNullOrWhiteSpace(model.ProjectCoordinatorMobile) ? model.ProjectCoordinatorMobile.AddMobileNumberZeroPrefix() : string.Empty
                                    },
                                    workpermitpassdetailsip = new Workpermitpassdetailsip
                                    {
                                        fullname = model.FullName,
                                        emailid = model.Emailaddress,
                                        profession = model.ProfessionLevel,
                                        emiratesid = model.EmiratesID,
                                        emiratesidenddate = model.EmiratesIDExpiryDate.HasValue ? model.EmiratesIDExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        countrykey = model.Nationality,
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                        passportnumber = model.PassportNumber,
                                        passportenddate = model.PassportExpiryDate.HasValue ? model.PassportExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        visanumber = model.VisaNumber,
                                        visaendate = model.VisaExpiryDate.HasValue ? model.VisaExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        remarks = model.Remarks,
                                        platenumber = model.withcar ? FormatVehiclePlateNumber(model.EmirateOrCountry, model.PlateCode, model.PlateNumber) : string.Empty,
                                        vehicleavailableflag = model.withcar ? "X" : string.Empty,
                                        photoflag = model.IsApplicantphoto ? "X" : string.Empty,
                                        emiratesidflag = model.IsEmiratesid ? "X" : string.Empty,
                                        passportflag = model.Ispassportdocument ? "X" : string.Empty,
                                        visaflag = model.Isvisadocument ? "X" : string.Empty,
                                        drivinglicenseflag = model.IsDrivingLicense ? "X" : string.Empty,
                                    },
                                    workpermitpassrequestip = new Workpermitpassrequestip
                                    {
                                        userid = CurrentPrincipal.UserId,
                                        countrykey = model.Nationality,
                                        passissuedate = model.PassIssueDate.HasValue ? model.PassIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        passexpirydate = model.PassExpiryDate.HasValue ? model.PassExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        fromtime = model.FromTime,
                                        totime = model.ToTime,
                                        ponumber = model.PONumber,
                                        projectname = model.POName,
                                        purposeofvisit = model.PurposeofVisit,
                                        remarks = model.Remarks,
                                        emailid = model.Emailaddress,
                                        lang = RequestLanguage.ToString(),
                                        permitsubreference = model.SubContractorID,
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                    },
                                }
                            }, RequestLanguage, Request.Segment());

                        if (response != null && response.Succeeded && response.Payload.Permitpassdetails != null)
                        {
                            model.ReferenceNumber = response.Payload.Permitpassdetails.Grouppassid.ToString();
                            return PartialView("~/Views/Feature/GatePass/WorkPermit/_Success.cshtml", model);
                        }
                        else
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                            // ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                }
                else
                {
                    if (model.GroupPass_Applicants != null)
                    {
                        CSVfileimportwitherror result = CSVfileimport.wpfileread(model.GroupPass_Applicants);
                        if (result.errorlist.Count <= 0 && result.wppassedrecords.Count <= 0)
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Excel Sheet is Empty"), Times.Once));
                            ModelState.AddModelError("GroupPass_Applicants", Translate.Text("Excel Sheet is Empty"));
                        }

                        if (result.errorlist != null && result.errorlist.Count > 0)
                        {
                            model.errorlist = result.errorlist;
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Epass.Grpinfomissing"), Times.Once));
                            ModelState.AddModelError("GroupPass_Applicants", Translate.Text("Epass.Grpinfomissing"));
                        }
                        else
                        {
                            model.errorlist = null;
                        }
                        List<WorkPermitCSVfileformat> records = result.wppassedrecords.ToList();
                        if (!model.GroupPassExpiryDate.HasValue || !model.GroupPassIssueDate.HasValue)
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date and Pass Issue date should not be empty"), Times.Once));
                            ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date and Pass Issue date should not be empty"));
                        }
                        if (ModelState.IsValid)
                        {
                            model.PageNo = 3;
                            if (records != null && records.Count > 0)
                            {
                                CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(model, TimeSpan.FromMinutes(40)));
                                CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, new CacheItem<List<WorkPermitCSVfileformat>>(result.wppassedrecords, TimeSpan.FromMinutes(40)));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.WORKPERMIT_REVIEW_PAGE);
                            }
                        }
                        model.PageNo = 3;
                        WPDefaultValues(model);
                        return PartialView("~/Views/Feature/GatePass/WorkPermit/ApplyWorkPermit.cshtml", model);
                    }
                }
            }
            catch (System.Exception ex)
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>("Unable to process the request, Kindly resubmit the request again", Times.Once));
                //ModelState.AddModelError(string.Empty, "Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            CacheProvider.Store(CacheKeys.WORKPERMIT_PASSREQUEST, new AccessCountingCacheItem<WorkPermitPass>(model, Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.WORKPERMIT_PAGE);
        }

        [HttpGet]
        public ActionResult WorkPermitReview()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<WorkPermitCSVfileformat> csvlist = new List<WorkPermitCSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out csvlist))
            {
                WorkPermitPass model = new WorkPermitPass();
                if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, out model))
                {
                    CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(model, TimeSpan.FromMinutes(40)));
                    WPDefaultValues(model);
                    model.allpasssubmitted = !csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                    return PartialView("~/Views/Feature/GatePass/WorkPermit/ApplyWorkPermitReview.cshtml", model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.WORKPERMIT_PAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProcessMultipass(WorkPermitPass model)
        {
            try
            {
                WorkPermitPass passmodel = new WorkPermitPass();
                if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, out passmodel))
                {
                    CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(passmodel, TimeSpan.FromMinutes(40)));
                    string error;
                    List<Attachmentlist> lstattachmentDetails = new List<Attachmentlist>();
                    List<Grouppasslocationlist> lstlocPassDetails = new List<Grouppasslocationlist>();
                    if (model.SinglePass_Photo != null && model.SinglePass_Photo.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Photo, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsApplicantphoto = true;
                                model.SinglePass_Photo.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Photo.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Photo.ContentType,
                                    folderid = AttachmentTypes.Applicant_Photo
                                });
                            }
                        }
                    }
                    if (model.SinglePass_EmiratesID != null && model.SinglePass_EmiratesID.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_EmiratesID, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsEmiratesid = true;
                                model.SinglePass_EmiratesID.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_EmiratesID.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_EmiratesID.ContentType,
                                    folderid = AttachmentTypes.EmiratesID
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Visa != null && model.SinglePass_Visa.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Visa, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Isvisadocument = true;
                                model.SinglePass_Visa.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Visa.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Visa.ContentType,
                                    folderid = AttachmentTypes.VisaDocument
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Passport != null && model.SinglePass_Passport.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Passport, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Ispassportdocument = true;
                                model.SinglePass_Passport.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Passport.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Passport.ContentType,
                                    folderid = AttachmentTypes.PassportDocument
                                });
                            }
                        }
                    }
                    if (model.withcar && model.DrivingLicense != null && model.DrivingLicense.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.DrivingLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsDrivingLicense = true;
                                model.DrivingLicense.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.DrivingLicense.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.DrivingLicense.ContentType,
                                    folderid = AttachmentTypes.DrivingLicense
                                });
                            }
                        }
                    }
                    if (passmodel.GroupSelectedLocation != null && passmodel.GroupSelectedLocation.Count() > 0)
                    {
                        Array.ForEach(passmodel.GroupSelectedLocation.ToArray(), x => lstlocPassDetails.Add(new Grouppasslocationlist { locationcode = x.ToString() }));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>("Select Location", Times.Once));
                        ModelState.AddModelError("SelectedLocation", Translate.Text("wp.select the location"));
                    }

                    model.PassIssue = passmodel.GroupPassIssue;
                    model.SubContractorID = passmodel.GroupSubcontractorID;
                    model.PassExpiry = passmodel.GroupPassExpiry;
                    model.FromTime = passmodel.GroupFromTime;
                    model.ToTime = passmodel.GroupToTime;
                    model.Projectcoordinatorname = passmodel.GroupProjectcoordinatorname;
                    model.ProjectCoordinatorMobile = passmodel.GroupProjectCoordinatorMobile;
                    model.ProjectCoordinatorEmailaddress = passmodel.GroupProjectCoordinatorEmailaddress;
                    model.PONumber = passmodel.PONumber;
                    model.POName = passmodel.POName;
                    model.PurposeofVisit = passmodel.GroupPurposeofVisit;
                    if (model.EmiratesIDExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.EmiratesIDExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"));
                        return Json(new { status = false, Message = Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date") }, JsonRequestBehavior.AllowGet);
                    }
                    if (model.PassportExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.PassportExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Passport expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Passport expiry date"));
                        return Json(new { status = false, Message = Translate.Text("Pass expiry date should not be greater than Passport expiry date") }, JsonRequestBehavior.AllowGet);
                    }
                    if (model.VisaExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.VisaExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Visa expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Visa expiry date"));
                        return Json(new { status = false, Message = Translate.Text("Pass expiry date should not be greater than Visa expiry date") }, JsonRequestBehavior.AllowGet);
                    }
                    if (ModelState.IsValid)
                    {
                        List<WorkPermitCSVfileformat> csvlist = new List<WorkPermitCSVfileformat>();
                        if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out csvlist))
                        {
                            if (string.IsNullOrWhiteSpace(passmodel.GroupPassid))
                            {
                                var grouppassresponse = SmartVendorClient.SubmitWorkPermitPass(
                                new GroupPemitPassRequest
                                {
                                    grouppassinput = new Grouppassinput
                                    {
                                        processcode = "SVD",
                                        save = "S",
                                        type = "H",
                                        groupid = passmodel.GroupPassid,
                                        lang = RequestLanguage.ToString(),
                                        sessionid = CurrentPrincipal.SessionToken,
                                        userid = CurrentPrincipal.UserId,
                                        grouppasslocationlist = lstlocPassDetails.ToArray(),
                                        projectcoordinatordetails = new Projectcoordinatordetails
                                        {
                                            fullname = model.Projectcoordinatorname,
                                            emailid = model.ProjectCoordinatorEmailaddress,
                                            mobile = !string.IsNullOrWhiteSpace(model.ProjectCoordinatorMobile) ? model.ProjectCoordinatorMobile.AddMobileNumberZeroPrefix() : string.Empty
                                        },
                                        workpermitpassrequestip = new Workpermitpassrequestip
                                        {
                                            userid = CurrentPrincipal.UserId,
                                            passissuedate = model.PassIssueDate.HasValue ? model.PassIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                                            passexpirydate = model.PassExpiryDate.HasValue ? model.PassExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                            fromtime = model.FromTime,
                                            totime = model.ToTime,
                                            ponumber = model.PONumber,
                                            projectname = model.POName,
                                            emailid = CurrentPrincipal.EmailAddress,
                                            lang = RequestLanguage.ToString(),
                                            permitsubreference = model.SubContractorID,
                                            purposeofvisit = model.PurposeofVisit
                                        }
                                    }
                                }, RequestLanguage, Request.Segment());
                                if (grouppassresponse != null && grouppassresponse.Succeeded && grouppassresponse.Payload.Permitpassdetails != null)
                                {
                                    var grouppassid = grouppassresponse.Payload.Permitpassdetails.Grouppassid;
                                    passmodel.GroupPassid = grouppassid;
                                    CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(passmodel, TimeSpan.FromMinutes(40)));
                                }
                                else
                                {
                                    return Json(new { status = false, Message = grouppassresponse.Message }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            var response = SmartVendorClient.SubmitWorkPermitPass(
                            new GroupPemitPassRequest
                            {
                                grouppassinput = new Grouppassinput
                                {
                                    processcode = "SVD",
                                    save = "S",
                                    type = "I",
                                    groupid = passmodel.GroupPassid,
                                    lang = RequestLanguage.ToString(),
                                    sessionid = CurrentPrincipal.SessionToken,
                                    userid = CurrentPrincipal.UserId,
                                    attachmentlist = lstattachmentDetails.ToArray(),
                                    workpermitpassdetailsip = new Workpermitpassdetailsip
                                    {
                                        grouppassid = passmodel.GroupPassid,
                                        fullname = model.FullName,
                                        emailid = model.Emailaddress,
                                        profession = model.ProfessionLevel,
                                        emiratesid = model.EmiratesID,
                                        emiratesidenddate = model.EmiratesIDExpiryDate.HasValue ? model.EmiratesIDExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        countrykey = model.Nationality,
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                        passportnumber = model.PassportNumber,
                                        passportenddate = model.PassportExpiryDate.HasValue ? model.PassportExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        visanumber = model.VisaNumber,
                                        visaendate = model.VisaExpiryDate.HasValue ? model.VisaExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        remarks = model.Remarks,
                                        platenumber = model.withcar ? FormatVehiclePlateNumber(model.EmirateOrCountry, model.PlateCode, model.PlateNumber) : string.Empty,
                                        vehicleavailableflag = model.withcar ? "X" : string.Empty,
                                        photoflag = model.IsApplicantphoto ? "X" : string.Empty,
                                        emiratesidflag = model.IsEmiratesid ? "X" : string.Empty,
                                        passportflag = model.Ispassportdocument ? "X" : string.Empty,
                                        visaflag = model.Isvisadocument ? "X" : string.Empty,
                                        drivinglicenseflag = model.IsDrivingLicense ? "X" : string.Empty,
                                    },
                                    workpermitpassrequestip = new Workpermitpassrequestip
                                    {
                                        userid = CurrentPrincipal.UserId,
                                        countrykey = model.Nationality,
                                        passissuedate = model.PassIssueDate.HasValue ? model.PassIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        passexpirydate = model.PassExpiryDate.HasValue ? model.PassExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        fromtime = model.FromTime,
                                        totime = model.ToTime,
                                        ponumber = model.PONumber,
                                        projectname = model.POName,
                                        purposeofvisit = model.PurposeofVisit,
                                        remarks = model.Remarks,
                                        emailid = model.Emailaddress,
                                        lang = RequestLanguage.ToString(),
                                        permitsubreference = model.SubContractorID,
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                    },
                                }
                            }, RequestLanguage, Request.Segment());

                            string strerror = string.Empty;
                            if (response != null && response.Succeeded && response.Payload.Permitpassdetails != null)
                            {
                                model.ReferenceNumber = response.Payload.Permitpassdetails.Permitpass;
                                var grouppassid = response.Payload.Permitpassdetails.Grouppassid;

                                csvlist.Where(x => x.ID.ToString().Equals(model.Serialnumber)).ToList().ForEach(
                                x =>
                                {
                                    x.registeredefolderid = model.ReferenceNumber;
                                    x.CustomerName = model.FullName;
                                    x.grouppassid = grouppassid;
                                    x.Emailid = model.Emailaddress;
                                    x.Phonenumber = model.Mobilenumber;
                                    x.EmiratesID = model.EmiratesID;
                                    x.EidDate = model.EmiratesIDExpiryDate;
                                    x.Passportnumber = model.PassportNumber;
                                    x.PassportexpDate = model.PassportExpiryDate;
                                    x.Profession = model.ProfessionLevel;
                                    x.Nationality = model.Nationality;
                                    x.Visanumber = model.VisaNumber;
                                    x.VisaexpDate = model.VisaExpiryDate;
                                });

                                bool notcompleted = csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                                CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, new CacheItem<List<WorkPermitCSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                                WPDefaultValues(passmodel);
                                string viewContent = ConvertViewToString("/Views/Feature/GatePass/WorkPermit/_ReviewForm.cshtml", passmodel);
                                if (!notcompleted)
                                {
                                    viewContent = ConvertViewToString("/Views/Feature/GatePass/WorkPermit/_ReviewFormAllSubmitted.cshtml", passmodel);
                                }

                                return Json(new { status = true, completed = !notcompleted, result = viewContent });
                            }
                            else
                            {
                                return Json(new { status = false, Message = response.Message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                return Json(new { status = false, Message = "Error while submitting the request" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProcessMultipassSubmit()
        {
            try
            {
                WorkPermitPass passmodel = new WorkPermitPass();
                if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, out passmodel))
                {
                    if (!string.IsNullOrWhiteSpace(passmodel.GroupPassid))
                    {
                        List<WorkPermitCSVfileformat> csvlist = new List<WorkPermitCSVfileformat>();
                        if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out csvlist))
                        {
                            bool notcompleted = csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                            CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, new CacheItem<List<WorkPermitCSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                            if (!notcompleted)
                            {
                                var response = SmartVendorClient.SubmitWorkPermitPass(
                                new GroupPemitPassRequest
                                {
                                    grouppassinput = new Grouppassinput
                                    {
                                        processcode = "SAV",
                                        save = "S",
                                        type = "",
                                        groupid = passmodel.GroupPassid,
                                        lang = RequestLanguage.ToString(),
                                        sessionid = CurrentPrincipal.SessionToken,
                                        userid = CurrentPrincipal.UserId,
                                    }
                                }, RequestLanguage, Request.Segment());
                                if (response != null && response.Succeeded)
                                {
                                    CacheProvider.Remove(CacheKeys.WORK_PERMIT_MULTIPASS_LIST);
                                    CacheProvider.Remove(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST);
                                    passmodel.ReferenceNumber = passmodel.GroupPassid;
                                    string viewContent = ConvertViewToString("/Views/Feature/GatePass/WorkPermit/_Success.cshtml", passmodel);
                                    return Json(new { status = true, completed = !notcompleted, result = viewContent });
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, Message = "check the input" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RemoveEpassentry(string id, string eid)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<WorkPermitCSVfileformat> csvlist = new List<WorkPermitCSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out csvlist))
            {
                WorkPermitCSVfileformat itemToRemove = csvlist.SingleOrDefault(r => r.ID.ToString().Equals(id) && r.EmiratesID.Equals(eid));
                if (itemToRemove != null)
                {
                    if (!string.IsNullOrWhiteSpace(itemToRemove.registeredefolderid))
                    {
                        var response = SmartVendorClient.SubmitWorkPermitPass(
                            new GroupPemitPassRequest
                            {
                                grouppassinput = new Grouppassinput
                                {
                                    processcode = "REM",
                                    permitpass = itemToRemove.registeredefolderid,
                                    lang = RequestLanguage.ToString(),
                                    sessionid = CurrentPrincipal.SessionToken,
                                    userid = CurrentPrincipal.UserId,
                                }
                            }, RequestLanguage, Request.Segment());
                    }
                    csvlist.Remove(itemToRemove);
                    CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, new CacheItem<List<WorkPermitCSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                }
                return Json(new { Message = true });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.WORKPERMIT_PAGE);
        }

        /// <summary>
        /// The DisplayPOAccountSelector.
        /// </summary>
        /// <param name="model">The model<see cref="PoDetailsListModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult DisplayPOAccountSelector(PoDetailsListModel model)
        {
            try
            {
                bool initial = model.PageNo < 1;
                int pageNo = model.PageNo <= 1 ? 1 : model.PageNo;
                int pageSize = 10;
                ServiceResponse<POListResponse> response = VendorServiceClient.GetPOList(new DEWAXP.Foundation.Integration.SmartVendorSvc.GetPOList { UserId = CurrentPrincipal.UserId, sessionid = CurrentPrincipal.SessionToken }, RequestLanguage, Request.Segment());
                //var response = VendorServiceClient.GetPOList(CurrentPrincipal.BusinessPartner, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.PO_DISPLAY != null && response.Payload.PO_DISPLAY.POITEMS != null && response.Payload.PO_DISPLAY.POITEMS.Count > 0)
                {
                    List<POITEM> polist = response.Payload.PO_DISPLAY.POITEMS;
                    if (!string.IsNullOrEmpty(model.Search))
                    {
                        polist = polist.Where(x => x != null && !string.IsNullOrEmpty(x.PO_desc) && !string.IsNullOrEmpty(x.Po_No) && (x.Po_No.Contains(model.Search.ToLower()) || x.PO_desc.ToLower().Contains(model.Search.ToLower()))).ToList();
                    }
                    model.ITEMPagedList = polist.AsEnumerable().ToPagedList(pageNo, pageSize);
                    if (polist != null && polist.Count > 0)
                    {
                        model.SelectedAccount = polist.FirstOrDefault().Po_No;
                    }
                    CacheProvider.Store(CacheKeys.WORK_PERMIT_PROJECT_LIST, new CacheItem<List<POITEM>>(polist, TimeSpan.FromMinutes(100)));
                }
                if (initial)
                {
                    return PartialView("~/Views/Feature/GatePass/WorkPermit/_POAccountSelector.cshtml", model);
                }
                return PartialView("~/Views/Feature/GatePass/WorkPermit/_POList.cshtml", model);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error--") + ex.Message);
                return PartialView("~/Views/Feature/GatePass/WorkPermit/_POAccountSelector.cshtml");
            }
        }

        /// <summary>
        /// The WorkPermitSubcontractorList.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult WorkPermitSubcontractorList()
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplier)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            subContractorResponse subContractorResponse = null;
            ServiceResponse<subContractorResponse> response = GetWPSubcontractorList(new GetWorkPermitSubContract
            {
                Process_Code = "SBG",
                Lang = RequestLanguage.ToString(),
                Session = new sessionDetails
                {
                    sessionid = CurrentPrincipal.SessionToken,
                    userid = CurrentPrincipal.UserId
                },
                Sub_Contract_Details = new subContractorDetails[] { new subContractorDetails() },
                Attachment = new attachmentDetails[] { new attachmentDetails() }
            });
            if (response != null && response.Succeeded && response.Payload != null)
            {
                subContractorResponse = response.Payload;
            }
            CacheProvider.Store(CacheKeys.WORK_PERMIT_SUBCONTRACTOR_LIST, new CacheItem<subContractorResponse>(subContractorResponse, TimeSpan.FromMinutes(40)));

            return PartialView("~/Views/Feature/GatePass/WorkPermit/SubcontractorList.cshtml");
        }

        /// <summary>
        /// The WorkPermitSubcontractorAjax.
        /// </summary>
        /// <param name="pagesize">The pagesize<see cref="int"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <param name="statustxt">The statustxt<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="namesort">The namesort<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WorkPermitSubcontractorAjax(int pagesize = 5, string keyword = "", string statustxt = "all", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_SUBCONTRACTOR_LIST, out subContractorResponse response))
            {
                SecurityPassFilterViewModel SecurityPassFilterViewModel = new SecurityPassFilterViewModel
                {
                    page = page
                };
                pagesize = pagesize > 30 ? 30 : pagesize;
                SecurityPassFilterViewModel.strdataindex = "0";
                if (response != null && response.subcontractordetails != null && response.subcontractordetails.Count() > 0)
                {
                    List<ePassSubContractor> lstePassSubContractors = response.subcontractordetails.ToList()
                            .Select(x => new ePassSubContractor()
                            {
                                displaysubcontractorid = x.work_permit_pass_number,
                                displaysubcontractorname = x.sub_contractor_name,
                                displaycountry = x.country_key,
                                displaytradelicensenumber = x.trade_license_number,
                                displaytradeenddate = !string.IsNullOrWhiteSpace(x.trade_license_end_date) && !x.trade_license_end_date.Equals("0000-00-00") ? DateTime.ParseExact(x.trade_license_end_date, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MMM dd, yyyy", Sitecorex.Context.Culture) : string.Empty,
                                displaytradeissuedate = !string.IsNullOrWhiteSpace(x.trade_license_issue_date) && !x.trade_license_end_date.Equals("0000-00-00") ? DateTime.ParseExact(x.trade_license_issue_date, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MMM dd, yyyy", Sitecorex.Context.Culture) : string.Empty,
                            })
                          .ToList();
                    List<subContractorDetails> LstsubContractorDetails = response.subcontractordetails.ToList();
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        lstePassSubContractors = lstePassSubContractors.Where(x => x.displaysubcontractorname.ToLower().Contains(keyword.ToLower()) || x.displaytradelicensenumber.ToLower().Contains(keyword.ToLower())).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(namesort))
                    {
                        if (namesort.ToLower().Equals("ascending"))
                        {
                            lstePassSubContractors = lstePassSubContractors.OrderBy(x => x.displaysubcontractorname).ToList();
                        }
                        else if (namesort.ToLower().Equals("descending"))
                        {
                            lstePassSubContractors = lstePassSubContractors.OrderByDescending(x => x.displaysubcontractorname).ToList();
                        }
                    }
                    SecurityPassFilterViewModel.namesort = namesort;
                    SecurityPassFilterViewModel.totalpage = Pager.CalculateTotalPages(lstePassSubContractors.Count(), pagesize);
                    SecurityPassFilterViewModel.pagination = SecurityPassFilterViewModel.totalpage > 1 ? true : false;
                    SecurityPassFilterViewModel.pagenumbers = SecurityPassFilterViewModel.totalpage > 1 ? Pager.GetPaginationRange(page, SecurityPassFilterViewModel.totalpage) : new List<int>();
                    lstePassSubContractors = lstePassSubContractors.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    SecurityPassFilterViewModel.lstsubcontractors = new JavaScriptSerializer().Serialize(lstePassSubContractors);
                    return Json(new { status = true, Message = SecurityPassFilterViewModel }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The WPAddSubContractor.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult WPAddSubContractor(string id)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            ePassSubContractor model = new ePassSubContractor();
            if (!CacheProvider.TryGet(CacheKeys.WORK_PERMIT_COUNTRYLIST, out IEnumerable<SelectListItem> countrylist))
            {
                countrylist = GetWPCountryList();
            }
            model.CountryList = countrylist;
            if (!string.IsNullOrWhiteSpace(id))
            {
                ServiceResponse<subContractorResponse> response = GetWPSubcontractorList(new GetWorkPermitSubContract
                {
                    Process_Code = "SBG",
                    Lang = RequestLanguage.ToString(),
                    Session = new sessionDetails
                    {
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId
                    },
                    Work_Permit_Request_No = id,
                    Sub_Contract_Details = new subContractorDetails[] { new subContractorDetails() },
                    Attachment = new attachmentDetails[] { new attachmentDetails() }
                });
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.subcontractordetails != null && response.Payload.subcontractordetails.Count() > 0)
                {
                    model.Name = response.Payload.subcontractordetails.FirstOrDefault().sub_contractor_name;
                    string country = response.Payload.subcontractordetails.FirstOrDefault().country_key;
                    model.CountryKey = response.Payload.subcontractordetails.FirstOrDefault().country_key;
                    model.Country = !string.IsNullOrWhiteSpace(country) && countrylist != null && countrylist.Count() > 0 && countrylist.Where(y => y.Value.Equals(country)).HasAny() ? countrylist.Where(y => y.Value.Equals(country)).FirstOrDefault().Text : country;
                    model.Trade_License_Number = response.Payload.subcontractordetails.FirstOrDefault().trade_license_number;
                    model.Trade_License_Issue_Date = DateHelper.getCultureDate(response.Payload.subcontractordetails.FirstOrDefault().trade_license_issue_date).ToString("dd MMMM yyyy");
                    model.Trade_License_Expiry_Date = DateHelper.getCultureDate(response.Payload.subcontractordetails.FirstOrDefault().trade_license_end_date).ToString("dd MMMM yyyy");
                    model.SubcontractorID = response.Payload.subcontractordetails.FirstOrDefault().work_permit_pass_number;
                }
            }
            return PartialView("~/Views/Feature/GatePass/WorkPermit/_WPSubContrator.cshtml", model);
        }

        /// <summary>
        /// The WPAddSubContractor.
        /// </summary>
        /// <param name="model">The model<see cref="ePassSubContractor"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WPAddSubContractor(ePassSubContractor model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            string error = string.Empty;
            attachmentDetails attachmentDetails = null;
            if (model.TradelicenseDoc != null)
            {
                if (!AttachmentIsValid(model.TradelicenseDoc, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        model.TradelicenseDoc.InputStream.CopyTo(memoryStream);
                        model.TradelicenseDoc_Bytes = memoryStream.ToArray();
                        model.TradelicenseDocFileName = model.TradelicenseDoc.FileName.GetFileNameWithoutPath();

                        attachmentDetails = new attachmentDetails
                        {
                            file_data = memoryStream.ToArray(),
                            file_name = model.TradelicenseDoc.FileName.GetFileNameWithoutPath(),
                            file_type = model.TradelicenseDoc.ContentType,
                            folder_name = AttachmentTypes.TradeLicense
                        };
                    }
                }
            }
            if (ModelState.IsValid)
            {
                ServiceResponse<subContractorResponse> response = GetWPSubcontractorList(new GetWorkPermitSubContract
                {
                    Process_Code = !string.IsNullOrWhiteSpace(model.SubcontractorID) ? "SBE" : "SBA",
                    Lang = RequestLanguage.ToString(),
                    Session = new sessionDetails
                    {
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId
                    },
                    Sub_Contract_Details = new subContractorDetails[] { new subContractorDetails {
                    country_key = model.Country,
                    gatepass_userid = CurrentPrincipal.UserId,
                    sub_contractor_name= model.Name,
                    trade_license_number= model.Trade_License_Number,
                    trade_license_attached = attachmentDetails != null ? "X":string.Empty,
                    trade_license_issue_date = model.DT_Trade_License_Issue_Date.HasValue ? model.DT_Trade_License_Issue_Date.Value.ToString("yyyyMMdd") : string.Empty,
                    trade_license_end_date = model.DT_Trade_License_Expiry_Date.HasValue ? model.DT_Trade_License_Expiry_Date.Value.ToString("yyyyMMdd") : string.Empty,
                    work_permit_pass_number = model.SubcontractorID
                } },
                    Attachment = new attachmentDetails[] { attachmentDetails }
                });

                if (response != null && response.Succeeded && response.Payload != null && response.Payload.subcontractreturn != null && response.Payload.subcontractreturn.Count() > 0)
                {
                    model.ReferenceNumber = response.Payload.subcontractreturn.FirstOrDefault().work_permit_pass;
                    model.Successmessage = !string.IsNullOrWhiteSpace(model.SubcontractorID) ? Translate.Text("WP.Editsuccessmessage") : Translate.Text("WP.Addsuccessmessage");
                    return PartialView("~/Views/Feature/GatePass/WorkPermit/_SubcontractorSuccess.cshtml", model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            if (!CacheProvider.TryGet(CacheKeys.WORK_PERMIT_COUNTRYLIST, out IEnumerable<SelectListItem> countrylist))
            {
                countrylist = GetWPCountryList();
            }
            model.CountryList = countrylist;
            return PartialView("~/Views/Feature/GatePass/WorkPermit/_WPSubContrator.cshtml", model);
        }

        /// <summary>
        /// The WPDeleteSubContractor.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WPDeleteSubContractor(string id)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (!string.IsNullOrWhiteSpace(id))
            {
                ServiceResponse<subContractorResponse> response = GetWPSubcontractorList(new GetWorkPermitSubContract
                {
                    Process_Code = "SBD",
                    Lang = RequestLanguage.ToString(),
                    Session = new sessionDetails
                    {
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId
                    },
                    Work_Permit_Request_No = id,
                    Sub_Contract_Details = new subContractorDetails[] { new subContractorDetails() },
                    Attachment = new attachmentDetails[] { new attachmentDetails() }
                });
                if (response != null && response.Succeeded)
                {
                    return Json(new { status = true });
                }
                return Json(new { status = false });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The WPCancelPass.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WPCancelPass(string id)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (!string.IsNullOrWhiteSpace(id))
            {
                //ServiceResponse<workPermitResponseDetails> response = VendorServiceClient.GetWorkPermitPass(
                //        new GetWorkPermitPass
                //        {
                //            Process_Code = "CNL",
                //            Lang = RequestLanguage.ToString(),
                //            Session = new sessionDetails
                //            {
                //                sessionid = CurrentPrincipal.SessionToken,
                //                userid = CurrentPrincipal.UserId
                //            },
                //            Work_Permit_Pass_Request_Details = new workPermitPassDetails(),
                //            Dewa_Project_Coordinator = new projectCoordinator(),
                //            Location_Details = new locPassDetails[] { new locPassDetails() },
                //            Attachment = new attachmentDetails[] { new attachmentDetails() },
                //            Work_Permit_Pass_No = id
                //        }, RequestLanguage, Request.Segment());
                var response = SmartVendorClient.SubmitWorkPermitPass(
                        new GroupPemitPassRequest
                        {
                            grouppassinput = new Grouppassinput
                            {
                                processcode = "CNL",
                                lang = RequestLanguage.ToString(),
                                sessionid = CurrentPrincipal.SessionToken,
                                userid = CurrentPrincipal.UserId,
                                groupid = id
                            }
                        }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded)
                {
                    return Json(new { status = true });
                }
                return Json(new { status = false });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The WPPassRenewal.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult WPPassRenewal(string passNumber)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (!string.IsNullOrWhiteSpace(passNumber) && (passNumber.ToLower().StartsWith("gp") || passNumber.ToLower().StartsWith("sp")))
            {
                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out string error))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                SecurityPassViewModel passDetails = null;
                List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstmodel))
                {
                    passDetails = lstmodel.Where(x => x.passNumber.ToLower().Equals(passNumber.ToLower())
                    || x.mainpassNumber.ToLower().Equals(passNumber.ToLower())
                    || x.grouppassid.ToLower().Equals(passNumber.ToLower())
                    ).FirstOrDefault();
                }

                List<WorkPermitPass> lstpassDetails = new List<WorkPermitPass>();
                BasePassViewModel relPassdetail = new BasePassViewModel();
                if (passDetails != null && passDetails.wppass)
                {
                    GroupPassPemitResponse lstpassess = GetWorkpermitPasses(passNumber);
                    if (!CacheProvider.TryGet(CacheKeys.WORK_PERMIT_COUNTRYLIST, out IEnumerable<SelectListItem> countrylist))
                    {
                        countrylist = GetWPCountryList();
                    }

                    if (lstpassess != null && lstpassess.Groupworkpermitpassbothlist != null)
                    {
                        lstpassDetails = lstpassess.Groupworkpermitpassbothlist.Where(y => !string.IsNullOrWhiteSpace(y.Passexpirydate) && !string.IsNullOrWhiteSpace(y.Passissuedate)
                            && !y.Passexpirydate.Equals("0000-00-00") && !y.Passissuedate.Equals("0000-00-00")).Select(x => new WorkPermitPass
                            {
                                FullName = x.Fullname,
                            PassNumber = x.Permitpass,
                            //mainpassNumber = x.Permitpass,
                            GroupPassid = x.Grouppassid,
                            //wppass = true,
                            //grouppass = !string.IsNullOrWhiteSpace(x.Grouppassid) && x.Grouppassid.StartsWith("GP") ? true : false,
                            //passType = Translate.Text("Work Permit"),
                            ProfessionLevel = x.Profession,
                            //CreatedDate = !string.IsNullOrWhiteSpace(x.createddate) && !x.createddate.Equals("0000-00-00") ? DateTime.Parse(x.createddate) : (DateTime?)null,
                            //ChangedDate = !string.IsNullOrWhiteSpace(x.createddate) && !x.createddate.Equals("0000-00-00") ? DateTime.Parse(x.createddate) : (DateTime?)null,
                            //PassExpiry = FormatKofaxDatewithCulture(x.Passexpirydate),
                            //PassIssue = FormatKofaxDatewithCulture(x.Passissuedate),
                            //status = assignWPStatus(x.Permitstatus, x.Passexpirydate),
                            Nationality = x.Countrykey,
                            EmiratesID = x.Emiratesid,
                            EmiratesIDExpiry = FormatKofaxDatewithCulture(x.Emiratesidenddate),
                            VisaNumber = x.Visanumber,
                            VisaExpiry = FormatKofaxDatewithCulture(x.Visaendate),
                            PassportNumber = x.Passportnumber,
                            PassportExpiry = FormatKofaxDatewithCulture(x.Passportenddate),
                            FromTime = !string.IsNullOrWhiteSpace(x.Fromtime) ? x.Fromtime.Replace(":", "") : string.Empty,
                            ToTime = !string.IsNullOrWhiteSpace(x.Totime) ? x.Totime.Replace(":", "") : string.Empty,
                            Mobilenumber = x.Mobile.RemoveMobileNumberZeroPrefix(),
                            Emailaddress = x.Emailid,
                            SelectedLocation = lstpassess.Grouppasslocationreturnlist != null && lstpassess.Grouppasslocationreturnlist.Count() > 0 && lstpassess.Grouppasslocationreturnlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                    lstpassess.Grouppasslocationreturnlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Select(z => z.Locationcode).ToList() : new List<string>(),
                            Remarks = x.Remarks,
                            SubContractorID = !string.IsNullOrWhiteSpace(x.Permitsubreference) && lstpassess.subcontractordetails != null && lstpassess.subcontractordetails.Count() > 0 && lstpassess.subcontractordetails.Where(y => y.Permitsubreference.Equals(x.Permitsubreference)).Any() ?
                                            lstpassess.subcontractordetails.Where(y => y.Permitsubreference.Equals(x.Permitsubreference)).FirstOrDefault().Subcontractorname : string.Empty,
                            //passAttachements = new List<SecurityPassAttachement>(),
                            CompanyName = x.Companyname,
                            POName = x.Projectname,
                            PurposeofVisit = x.Purposeofvisit,
                            projectStartDate = null,
                            projectEndDate = null,
                            projectId = x.Ponumber,
                            projectStatus = string.Empty,
                            departmentName = string.Empty,
                            enablerenewbutton = !string.IsNullOrWhiteSpace(x.Passexpirydate) && string.IsNullOrWhiteSpace(x.Renewalreference) ? DateTime.Parse(lstpassess.Groupworkpermitpassbothlist[0].Passexpirydate).AddDays(-10) <= DateTime.Now : false,
                            Projectcoordinatorname = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                            lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Fullname : string.Empty,
                            ProjectCoordinatorEmailaddress = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                            lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Emailid : string.Empty,
                            ProjectCoordinatorMobile = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                            lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Mobile.RemoveMobileNumberZeroPrefix() : string.Empty,
                        }).ToList();

                        CacheProvider.Store(CacheKeys.EPASS_RENEWAL_DETAILS, new CacheItem<List<WorkPermitPass>>(lstpassDetails, TimeSpan.FromMinutes(20)));

                        ViewBag.role = CurrentPrincipal.Role;
                        if (lstpassDetails != null && lstpassDetails.Count > 0 && !string.IsNullOrWhiteSpace(passNumber) && (passNumber.ToLower().StartsWith("gp") || passNumber.ToLower().StartsWith("sp")))
                        //&& lstpassDetails.FirstOrDefault().enablerenewbutton)
                        {
                            WPDefaultValues(lstpassDetails.FirstOrDefault());
                            return PartialView("~/Views/Feature/GatePass/WorkPermit/_WPPassRenewal.cshtml", lstpassDetails);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Not eligible");
                        }
                        return PartialView("~/Views/Feature/GatePass/WorkPermit/_WPPassRenewal.cshtml", null);
                    }
                }
            }
            ViewBag.failed = true;
            ModelState.AddModelError(string.Empty, "Not a valid pass");
            return PartialView("~/Views/Feature/GatePass/WorkPermit/_WPPassRenewal.cshtml", null);
        }

        /// <summary>
        /// The WPPassRenewal.
        /// </summary>
        /// <param name="model">The model<see cref="SecurityPassViewModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WPPassRenewal(WorkPermitPass model)
        {
            try
            {
                if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
                }
                if (model.PassSubmitType.Equals("single"))
                {
                    string error;
                    List<Attachmentlist> lstattachmentDetails = new List<Attachmentlist>();
                    List<Grouppasslocationlist> lstlocPassDetails = new List<Grouppasslocationlist>();
                    if (model.SinglePass_EmiratesID != null && model.SinglePass_EmiratesID.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_EmiratesID, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsEmiratesid = true;
                                model.SinglePass_EmiratesID.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_EmiratesID.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_EmiratesID.ContentType,
                                    folderid = AttachmentTypes.EmiratesID
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Visa != null && model.SinglePass_Visa.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Visa, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Isvisadocument = true;
                                model.SinglePass_Visa.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Visa.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Visa.ContentType,
                                    folderid = AttachmentTypes.VisaDocument
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Passport != null && model.SinglePass_Passport.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Passport, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Ispassportdocument = true;
                                model.SinglePass_Passport.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Passport.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Passport.ContentType,
                                    folderid = AttachmentTypes.PassportDocument
                                });
                            }
                        }
                    }
                    if (model.withcar && model.DrivingLicense != null && model.DrivingLicense.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.DrivingLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsDrivingLicense = true;
                                model.DrivingLicense.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.DrivingLicense.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.DrivingLicense.ContentType,
                                    folderid = AttachmentTypes.DrivingLicense
                                });
                            }
                        }
                    }
                    if (model.SelectedLocation != null && model.SelectedLocation.Count() > 0)
                    {
                        Array.ForEach(model.SelectedLocation.ToArray(), x => lstlocPassDetails.Add(new Grouppasslocationlist { locationcode = x.ToString() }));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>("Select Location", Times.Once));
                        ModelState.AddModelError("SelectedLocation", Translate.Text("wp.select the location"));
                    }
                    if (model.EmiratesIDExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.EmiratesIDExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"));
                    }

                    if (model.PassportExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.PassportExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Passport expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Passport expiry date"));
                    }
                    if (model.VisaExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.VisaExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Visa expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Visa expiry date"));
                    }
                    if (!model.PassExpiryDate.HasValue || !model.PassIssueDate.HasValue)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date and Pass Issue date should not be empty"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date and Pass Issue date should not be empty"));
                    }
                    if (ModelState.IsValid)
                    {
                        var response = SmartVendorClient.SubmitWorkPermitPass(
                            new GroupPemitPassRequest
                            {
                                grouppassinput = new Grouppassinput
                                {
                                    processcode = "SAV",
                                    save = "R",
                                    type = "S",
                                    lang = RequestLanguage.ToString(),
                                    sessionid = CurrentPrincipal.SessionToken,
                                    userid = CurrentPrincipal.UserId,
                                    attachmentlist = lstattachmentDetails.ToArray(),
                                    grouppasslocationlist = lstlocPassDetails.ToArray(),
                                    projectcoordinatordetails = new Projectcoordinatordetails
                                    {
                                        fullname = model.Projectcoordinatorname,
                                        emailid = model.ProjectCoordinatorEmailaddress,
                                        mobile = !string.IsNullOrWhiteSpace(model.ProjectCoordinatorMobile) ? model.ProjectCoordinatorMobile.AddMobileNumberZeroPrefix() : string.Empty
                                    },
                                    workpermitpassdetailsip = new Workpermitpassdetailsip
                                    {
                                        fullname = model.FullName,
                                        emailid = model.Emailaddress,
                                        profession = model.ProfessionLevel,
                                        emiratesid = model.EmiratesID,
                                        emiratesidenddate = model.EmiratesIDExpiryDate.HasValue ? model.EmiratesIDExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        countrykey = model.Nationality,
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                        passportnumber = model.PassportNumber,
                                        passportenddate = model.PassportExpiryDate.HasValue ? model.PassportExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        visanumber = model.VisaNumber,
                                        visaendate = model.VisaExpiryDate.HasValue ? model.VisaExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        remarks = model.Remarks,
                                        platenumber = model.withcar ? FormatVehiclePlateNumber(model.EmirateOrCountry, model.PlateCode, model.PlateNumber) : string.Empty,
                                        vehicleavailableflag = model.withcar ? "X" : string.Empty,
                                        photoflag = model.IsApplicantphoto ? "X" : string.Empty,
                                        emiratesidflag = model.IsEmiratesid ? "X" : string.Empty,
                                        passportflag = model.Ispassportdocument ? "X" : string.Empty,
                                        visaflag = model.Isvisadocument ? "X" : string.Empty,
                                        drivinglicenseflag = model.IsDrivingLicense ? "X" : string.Empty,
                                        permitpass = model.PassNumber
                                    },
                                    workpermitpassrequestip = new Workpermitpassrequestip
                                    {
                                        grouppassid = model.GroupPassid,
                                        userid = CurrentPrincipal.UserId,
                                        countrykey = model.Nationality,
                                        passissuedate = model.PassIssueDate.HasValue ? model.PassIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        passexpirydate = model.PassExpiryDate.HasValue ? model.PassExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        fromtime = model.FromTime,
                                        totime = model.ToTime,
                                        purposeofvisit = model.PurposeofVisit,
                                        remarks = model.Remarks,
                                        emailid = model.Emailaddress,
                                        lang = RequestLanguage.ToString(),
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                    },
                                }
                            }, RequestLanguage, Request.Segment());
                        if (response != null && response.Succeeded && response.Payload.Permitpassdetails != null)
                        {
                            model.ReferenceNumber = response.Payload.Permitpassdetails.Grouppassid.ToString();
                            return PartialView("~/Views/Feature/GatePass/WorkPermit/_Success.cshtml", model);
                        }
                        else
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(response.Message, Times.Once));
                        }
                    }
                }
                else
                {
                    List<WorkPermitPass> workPermitPasses = new List<WorkPermitPass>();
                    if (CacheProvider.TryGet(CacheKeys.EPASS_RENEWAL_DETAILS, out workPermitPasses))
                    {
                        CacheProvider.Store(CacheKeys.EPASS_RENEWAL_DETAILS, new CacheItem<List<WorkPermitPass>>(workPermitPasses, TimeSpan.FromMinutes(20)));
                        List<WorkPermitCSVfileformat> records = workPermitPasses.Select((x, i) => new WorkPermitCSVfileformat
                        {
                            CustomerName = x.FullName,
                            Emailid = x.Emailaddress,
                            Phonenumber = x.Mobilenumber,
                            EmiratesID = x.EmiratesID,
                            EidDate = x.EmiratesIDExpiryDate,
                            Passportnumber = x.PassportNumber,
                            PassportexpDate = x.PassportExpiryDate,
                            Profession = x.ProfessionLevel,
                            //Purpose = x.PurposeofVisit,
                            Visanumber = x.VisaNumber,
                            VisaexpDate = x.VisaExpiryDate,
                            grouppassid = x.GroupPassid,
                            ID = i
                        }).ToList();
                        if (ModelState.IsValid)
                        {
                            model.PageNo = 3;
                            if (records != null && records.Count > 0)
                            {
                                CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(model, TimeSpan.FromMinutes(40)));
                                CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, new CacheItem<List<WorkPermitCSVfileformat>>(records, TimeSpan.FromMinutes(40)));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.WP_RENEWAL_REVIEW_PAGE);
                            }
                        }
                        model.PageNo = 3;
                    }
                }
            }
            catch (System.Exception ex)
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>("Unable to process the request, Kindly resubmit the request again", Times.Once));
                LogService.Fatal(ex, this);
            }
            QueryString q1 = new QueryString(true);
            q1.With("passNumber", model.GroupPassid, true);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.WP_PASSRENEWAL, q1);
        }

        [HttpGet]
        public ActionResult WorkPermitRenewalReview()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<WorkPermitCSVfileformat> csvlist = new List<WorkPermitCSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out csvlist))
            {
                WorkPermitPass model = new WorkPermitPass();
                if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, out model))
                {
                    CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(model, TimeSpan.FromMinutes(40)));
                    WPDefaultValues(model);
                    model.allpasssubmitted = !csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                    return PartialView("~/Views/Feature/GatePass/WorkPermit/ApplyWorkPermitRenewalReview.cshtml", model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.WORKPERMIT_PAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RenewalMultipass(WorkPermitPass model)
        {
            try
            {
                WorkPermitPass passmodel = new WorkPermitPass();
                if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, out passmodel))
                {
                    CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(passmodel, TimeSpan.FromMinutes(40)));
                    string error;
                    List<Attachmentlist> lstattachmentDetails = new List<Attachmentlist>();
                    List<Grouppasslocationlist> lstlocPassDetails = new List<Grouppasslocationlist>();
                    if (model.SinglePass_EmiratesID != null && model.SinglePass_EmiratesID.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_EmiratesID, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsEmiratesid = true;
                                model.SinglePass_EmiratesID.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_EmiratesID.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_EmiratesID.ContentType,
                                    folderid = AttachmentTypes.EmiratesID
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Visa != null && model.SinglePass_Visa.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Visa, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Isvisadocument = true;
                                model.SinglePass_Visa.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Visa.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Visa.ContentType,
                                    folderid = AttachmentTypes.VisaDocument
                                });
                            }
                        }
                    }
                    if (model.SinglePass_Passport != null && model.SinglePass_Passport.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.SinglePass_Passport, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.Ispassportdocument = true;
                                model.SinglePass_Passport.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.SinglePass_Passport.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.SinglePass_Passport.ContentType,
                                    folderid = AttachmentTypes.PassportDocument
                                });
                            }
                        }
                    }
                    if (model.withcar && model.DrivingLicense != null && model.DrivingLicense.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.DrivingLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                model.IsDrivingLicense = true;
                                model.DrivingLicense.InputStream.CopyTo(memoryStream);
                                lstattachmentDetails.Add(new Attachmentlist
                                {
                                    filedata = Convert.ToBase64String(memoryStream.ToArray() ?? new byte[0]),
                                    filename = model.DrivingLicense.FileName.GetFileNameWithoutPath(),
                                    mimetype = model.DrivingLicense.ContentType,
                                    folderid = AttachmentTypes.DrivingLicense
                                });
                            }
                        }
                    }
                    if (passmodel.SelectedLocation != null && passmodel.SelectedLocation.Count() > 0)
                    {
                        Array.ForEach(passmodel.SelectedLocation.ToArray(), x => lstlocPassDetails.Add(new Grouppasslocationlist { locationcode = x.ToString() }));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>("Select Location", Times.Once));
                        ModelState.AddModelError("SelectedLocation", Translate.Text("wp.select the location"));
                    }

                    model.PassIssue = passmodel.PassIssue;
                    model.PassExpiry = passmodel.PassExpiry;
                    model.FromTime = passmodel.FromTime;
                    model.ToTime = passmodel.ToTime;
                    model.Projectcoordinatorname = passmodel.Projectcoordinatorname;
                    model.ProjectCoordinatorMobile = passmodel.ProjectCoordinatorMobile;
                    model.ProjectCoordinatorEmailaddress = passmodel.ProjectCoordinatorEmailaddress;
                    model.PassNumber = passmodel.PassNumber;
                    model.GroupPassid = passmodel.GroupPassid;
                    model.PurposeofVisit = passmodel.PurposeofVisit;
                    if (model.EmiratesIDExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.EmiratesIDExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date"));
                        return Json(new { status = false, Message = Translate.Text("Pass expiry date should not be greater than Emirates ID expiry date") }, JsonRequestBehavior.AllowGet);
                    }
                    if (model.PassportExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.PassportExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Passport expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Passport expiry date"));
                        return Json(new { status = false, Message = Translate.Text("Pass expiry date should not be greater than Passport expiry date") }, JsonRequestBehavior.AllowGet);
                    }
                    if (model.VisaExpiryDate.HasValue && model.PassExpiryDate.HasValue && model.PassExpiryDate.Value > model.VisaExpiryDate.Value)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date should not be greater than Visa expiry date"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date should not be greater than Visa expiry date"));
                        return Json(new { status = false, Message = Translate.Text("Pass expiry date should not be greater than Visa expiry date") }, JsonRequestBehavior.AllowGet);
                    }
                    if (!model.PassExpiryDate.HasValue || !model.PassIssueDate.HasValue)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Pass expiry date and Pass Issue date should not be empty"), Times.Once));
                        ModelState.AddModelError(string.Empty, Translate.Text("Pass expiry date and Pass Issue date should not be empty"));
                        return Json(new { status = false, Message = Translate.Text("Pass expiry date and Pass Issue date should not be empty") }, JsonRequestBehavior.AllowGet);
                    }
                    if (ModelState.IsValid)
                    {
                        List<WorkPermitCSVfileformat> csvlist = new List<WorkPermitCSVfileformat>();
                        if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out csvlist))
                        {
                            bool firstrecord = csvlist.Any(x => !string.IsNullOrWhiteSpace(x.registeredefolderid));
                            if (!firstrecord)
                            {
                                var grouppassresponse = SmartVendorClient.SubmitWorkPermitPass(
                                new GroupPemitPassRequest
                                {
                                    grouppassinput = new Grouppassinput
                                    {
                                        processcode = "SVD",
                                        save = "R",
                                        type = "H",
                                        groupid = passmodel.GroupPassid,
                                        lang = RequestLanguage.ToString(),
                                        sessionid = CurrentPrincipal.SessionToken,
                                        userid = CurrentPrincipal.UserId,
                                        grouppasslocationlist = lstlocPassDetails.ToArray(),
                                        projectcoordinatordetails = new Projectcoordinatordetails
                                        {
                                            fullname = model.Projectcoordinatorname,
                                            emailid = model.ProjectCoordinatorEmailaddress,
                                            mobile = !string.IsNullOrWhiteSpace(model.ProjectCoordinatorMobile) ? model.ProjectCoordinatorMobile.AddMobileNumberZeroPrefix() : string.Empty
                                        },
                                        workpermitpassrequestip = new Workpermitpassrequestip
                                        {
                                            grouppassid = model.GroupPassid,
                                            userid = CurrentPrincipal.UserId,
                                            passissuedate = model.PassIssueDate.HasValue ? model.PassIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                                            passexpirydate = model.PassExpiryDate.HasValue ? model.PassExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                            fromtime = model.FromTime,
                                            totime = model.ToTime,
                                            emailid = CurrentPrincipal.EmailAddress,
                                            lang = RequestLanguage.ToString(),
                                            purposeofvisit = model.PurposeofVisit
                                        }
                                    }
                                }, RequestLanguage, Request.Segment());
                                if (grouppassresponse != null && grouppassresponse.Succeeded && grouppassresponse.Payload.Permitpassdetails != null)
                                {
                                    var grouppassid = grouppassresponse.Payload.Permitpassdetails.Grouppassid;
                                    passmodel.GroupPassid = grouppassid;
                                    CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, new CacheItem<WorkPermitPass>(passmodel, TimeSpan.FromMinutes(40)));
                                }
                                else
                                {
                                    return Json(new { status = false, Message = grouppassresponse.Message }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            var response = SmartVendorClient.SubmitWorkPermitPass(
                            new GroupPemitPassRequest
                            {
                                grouppassinput = new Grouppassinput
                                {
                                    processcode = "SVD",
                                    save = "R",
                                    type = "I",
                                    groupid = passmodel.GroupPassid,
                                    lang = RequestLanguage.ToString(),
                                    sessionid = CurrentPrincipal.SessionToken,
                                    userid = CurrentPrincipal.UserId,
                                    attachmentlist = lstattachmentDetails.ToArray(),
                                    workpermitpassdetailsip = new Workpermitpassdetailsip
                                    {
                                        permitpass = model.PassNumber,
                                        grouppassid = passmodel.GroupPassid,
                                        fullname = model.FullName,
                                        emailid = model.Emailaddress,
                                        profession = model.ProfessionLevel,
                                        emiratesid = model.EmiratesID,
                                        emiratesidenddate = model.EmiratesIDExpiryDate.HasValue ? model.EmiratesIDExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        countrykey = model.Nationality,
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                        passportnumber = model.PassportNumber,
                                        passportenddate = model.PassportExpiryDate.HasValue ? model.PassportExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        visanumber = model.VisaNumber,
                                        visaendate = model.VisaExpiryDate.HasValue ? model.VisaExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        remarks = model.Remarks,
                                        platenumber = model.withcar ? FormatVehiclePlateNumber(model.EmirateOrCountry, model.PlateCode, model.PlateNumber) : string.Empty,
                                        vehicleavailableflag = model.withcar ? "X" : string.Empty,
                                        photoflag = model.IsApplicantphoto ? "X" : string.Empty,
                                        emiratesidflag = model.IsEmiratesid ? "X" : string.Empty,
                                        passportflag = model.Ispassportdocument ? "X" : string.Empty,
                                        visaflag = model.Isvisadocument ? "X" : string.Empty,
                                        drivinglicenseflag = model.IsDrivingLicense ? "X" : string.Empty,
                                    },
                                    workpermitpassrequestip = new Workpermitpassrequestip
                                    {
                                        grouppassid = passmodel.GroupPassid,
                                        userid = CurrentPrincipal.UserId,
                                        countrykey = model.Nationality,
                                        passissuedate = model.PassIssueDate.HasValue ? model.PassIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        passexpirydate = model.PassExpiryDate.HasValue ? model.PassExpiryDate.Value.ToString("yyyyMMdd") : string.Empty,
                                        fromtime = model.FromTime,
                                        totime = model.ToTime,
                                        purposeofvisit = model.PurposeofVisit,
                                        remarks = model.Remarks,
                                        emailid = model.Emailaddress,
                                        lang = RequestLanguage.ToString(),
                                        mobile = !string.IsNullOrWhiteSpace(model.Mobilenumber) ? model.Mobilenumber.AddMobileNumberZeroPrefix() : string.Empty,
                                    },
                                }
                            }, RequestLanguage, Request.Segment());

                            string strerror = string.Empty;
                            if (response != null && response.Succeeded && response.Payload.Permitpassdetails != null)
                            {
                                model.ReferenceNumber = response.Payload.Permitpassdetails.Permitpass;
                                var grouppassid = response.Payload.Permitpassdetails.Grouppassid;

                                //csvlist.Where(x => x.ID.ToString().Equals(model.Serialnumber)).ToList().ForEach(
                                //x =>
                                //{
                                //    x.registeredefolderid = model.ReferenceNumber;
                                //    x.CustomerName = model.FullName;
                                //    x.grouppassid = grouppassid;
                                //});
                                csvlist.Where(x => x.ID.ToString().Equals(model.Serialnumber)).ToList().ForEach(
                                x =>
                                {
                                    x.registeredefolderid = model.ReferenceNumber;
                                    x.CustomerName = model.FullName;
                                    x.grouppassid = grouppassid;
                                    x.Emailid = model.Emailaddress;
                                    x.Phonenumber = model.Mobilenumber;
                                    x.EmiratesID = model.EmiratesID;
                                    x.EidDate = model.EmiratesIDExpiryDate;
                                    x.Passportnumber = model.PassportNumber;
                                    x.PassportexpDate = model.PassportExpiryDate;
                                    x.Profession = model.ProfessionLevel;
                                    x.Nationality = model.Nationality;
                                    x.Visanumber = model.VisaNumber;
                                    x.VisaexpDate = model.VisaExpiryDate;
                                });
                                bool notcompleted = csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                                CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, new CacheItem<List<WorkPermitCSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                                WPDefaultValues(passmodel);
                                string viewContent = ConvertViewToString("/Views/Feature/GatePass/WorkPermit/_RenewalReviewForm.cshtml", passmodel);
                                if (!notcompleted)
                                {
                                    viewContent = ConvertViewToString("/Views/Feature/GatePass/WorkPermit/_ReviewFormAllSubmitted.cshtml", passmodel);
                                }

                                return Json(new { status = true, completed = !notcompleted, result = viewContent });
                            }
                            else
                            {
                                return Json(new { status = false, Message = response.Message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                return Json(new { status = false, Message = "Error while submitting the request" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RenewalMultipassSubmit()
        {
            try
            {
                WorkPermitPass passmodel = new WorkPermitPass();
                if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST, out passmodel))
                {
                    if (!string.IsNullOrWhiteSpace(passmodel.GroupPassid))
                    {
                        List<WorkPermitCSVfileformat> csvlist = new List<WorkPermitCSVfileformat>();
                        if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out csvlist))
                        {
                            bool notcompleted = csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                            CacheProvider.Store(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, new CacheItem<List<WorkPermitCSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                            if (!notcompleted)
                            {
                                var response = SmartVendorClient.SubmitWorkPermitPass(
                                new GroupPemitPassRequest
                                {
                                    grouppassinput = new Grouppassinput
                                    {
                                        processcode = "SAV",
                                        save = "R",
                                        type = "",
                                        groupid = passmodel.GroupPassid,
                                        lang = RequestLanguage.ToString(),
                                        sessionid = CurrentPrincipal.SessionToken,
                                        userid = CurrentPrincipal.UserId,
                                    }
                                }, RequestLanguage, Request.Segment());
                                if (response != null && response.Succeeded)
                                {
                                    CacheProvider.Remove(CacheKeys.WORK_PERMIT_MULTIPASS_LIST);
                                    CacheProvider.Remove(CacheKeys.WORK_PERMIT_MULTIPASS_REQUEST);
                                    passmodel.ReferenceNumber = passmodel.GroupPassid;
                                    string viewContent = ConvertViewToString("/Views/Feature/GatePass/WorkPermit/_Success.cshtml", passmodel);
                                    return Json(new { status = true, completed = !notcompleted, result = viewContent });
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, Message = "check the input" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditLocation(string[] locations, string grouppassid)
        {
            string message = ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE;
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (!string.IsNullOrWhiteSpace(grouppassid) || (locations != null && locations.Count() > 0))
            {
                List<Grouppasslocationlist> lstlocPassDetails = new List<Grouppasslocationlist>();
                Array.ForEach(locations, x => lstlocPassDetails.Add(new Grouppasslocationlist { locationcode = x.ToString() }));
                message = Translate.Text("Location edited successfully");
                var response = SmartVendorClient.SubmitWorkPermitPass(
                        new GroupPemitPassRequest
                        {
                            grouppassinput = new Grouppassinput
                            {
                                processcode = "LOC",
                                lang = RequestLanguage.ToString(),
                                sessionid = CurrentPrincipal.SessionToken,
                                userid = CurrentPrincipal.UserId,
                                groupid = grouppassid,
                                grouppasslocationlist = lstlocPassDetails.ToArray(),
                            },
                        }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded)
                {
                    return Json(new { status = true, Message = message });
                }
                return Json(new { status = false, Message = response.Message });
            }
            return Json(new { status = false, Message = message });
        }

        /// <summary>
        /// The WPAttachment.
        /// </summary>
        /// <param name="passnumber">The passnumber<see cref="string"/>.</param>
        /// <param name="objectid">The objectid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult WPAttachment(string passnumber, string objectid)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return null;
            }
            //workPermitResponseDetails lstpassess = null;
            GroupPassPemitResponse lstpassess = GetWorkpermitPasses(passnumber);
            if (lstpassess.attachmentdetails != null && lstpassess.attachmentdetails.Count() > 0 && lstpassess.attachmentdetails.Where(x => x.Folderid.Equals(objectid)).Any())
            {
                byte[] bytes = Convert.FromBase64String(lstpassess.attachmentdetails.Where(x => x.Folderid.Equals(objectid)).FirstOrDefault().Filedata);
                string type = lstpassess.attachmentdetails.Where(x => x.Folderid.Equals(objectid)).FirstOrDefault().Mimetype;
                return File(bytes, type);
            }
            return null;
        }

        [HttpGet]
        public ActionResult WPPassDownload(string passnumber, string grouppassnumber)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return null;
            }
            var response = SmartVendorClient.SubmitWorkPermitPass(
                        new GroupPemitPassRequest
                        {
                            grouppassinput = new Grouppassinput
                            {
                                processcode = "DWN",
                                lang = RequestLanguage.ToString(),
                                sessionid = CurrentPrincipal.SessionToken,
                                userid = CurrentPrincipal.UserId,
                                permitpass = passnumber,
                                groupid = grouppassnumber
                            }
                        }, RequestLanguage, Request.Segment());
            if (response != null && response.Succeeded && response.Payload != null)
            {
                var lstpassess = response.Payload;
                if (lstpassess.attachmentdetails != null && lstpassess.attachmentdetails.Count() > 0 && lstpassess.attachmentdetails.Any())
                {
                    byte[] bytes = Convert.FromBase64String(lstpassess.attachmentdetails.FirstOrDefault().Filedata);
                    string type = lstpassess.attachmentdetails.FirstOrDefault().Mimetype;
                    return File(bytes, type);
                }
            }
            return null;
        }

        private ServiceResponse<subContractorResponse> GetWPSubContractorList()
        {
            return GetWPSubcontractorList(new GetWorkPermitSubContract
            {
                Process_Code = "SBG",
                Lang = RequestLanguage.ToString(),
                Session = new sessionDetails
                {
                    sessionid = CurrentPrincipal.SessionToken,
                    userid = CurrentPrincipal.UserId
                },
                Sub_Contract_Details = new subContractorDetails[] { new subContractorDetails() },
                Attachment = new attachmentDetails[] { new attachmentDetails() }
            });
        }

        /// <summary>
        /// The GetWPSubcontractorList.
        /// </summary>
        /// <param name="getWorkPermitSubContract">The getWorkPermitSubContract<see cref="GetWorkPermitSubContract"/>.</param>
        /// <returns>The <see cref="ServiceResponse{subContractorResponse}"/>.</returns>
        public ServiceResponse<subContractorResponse> GetWPSubcontractorList(GetWorkPermitSubContract getWorkPermitSubContract)
        {
            ServiceResponse<subContractorResponse> response = VendorServiceClient.GetWorkPermitSubContract(
                        getWorkPermitSubContract, RequestLanguage, Request.Segment());
            return response;
        }
    }
}