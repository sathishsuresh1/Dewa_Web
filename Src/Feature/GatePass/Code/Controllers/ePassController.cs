// <copyright file="ePassController.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Controllers
{
    using DEWAXP.Feature.GatePass.Models.ePass;
    using DEWAXP.Feature.GatePass.Models.ePass.SubPasses;
    using DEWAXP.Feature.GatePass.Utils;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Filters.Mvc;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Content.Services;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.GatePassSvc;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.KofaxRest;
    using DEWAXP.Foundation.Integration.Responses.SmartVendor.WorkPermit;
    using DEWAXP.Foundation.Logger;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Globalization;
    using Newtonsoft.Json;
    using Sitecore.Links.UrlBuilders;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.Protocols;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using X.PagedList;   
    using Attribute = Models.ePass.SubPasses.Attribute;
    using Sitecorex = global::Sitecore;

    /// <summary>
    /// Defines the <see cref="ePassController" />.
    /// </summary>
    public class ePassController : EpassBaseController
    {
        /// <summary>
        /// Defines the _lock.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Defines the epasscommonmodel.
        /// </summary>
        internal EpassCommon epasscommonmodel = new EpassCommon();

        /// <summary>
        /// Defines the TimeFormatPattern.
        /// </summary>
        private readonly string TimeFormatPattern = "hh:mm tt";

        /// <summary>
        /// Apply new pass form.
        /// </summary>
        /// <param name="passNo">.</param>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ApplyPermanentPass(string passNo = null)
        {
            PermanentPass model = new PermanentPass();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (passNo != null)
            {
                model = GetPermanentPass(passNo);
            }

            CacheProvider.TryGet(CacheKeys.EPASS_LOCMAIN, out string storedMainLoc);
            AssignDefaultvalues(model);
            return PartialView("~/Views/Feature/GatePass/ePass/_ApplyPermanentPass.cshtml", model);
        }

        /// <summary>
        /// Diplay my passes.
        /// </summary>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ePassMyPasses()
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplier)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<SecurityPassViewModel> lstpass = GetListofPasses();

            return PartialView("~/Views/Feature/GatePass/ePass/_MyPasses.cshtml");
        }

        /// <summary>
        /// Display all passes.
        /// </summary>
        /// <returns>.</returns>
        public ActionResult ePassAllPasses()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            //var lstpass = GetAllListofPasses();

            return PartialView("~/Views/Feature/GatePass/ePass/_AllPasses.cshtml");
        }

        /// <summary>
        /// Apply multiple pass form.
        /// </summary>
        /// <param name="model">.</param>
        /// <returns>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProcessMultipass(PermanentPass model)
        {
            try
            {
                PermanentPass passmodel = new PermanentPass();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_REQUEST, out passmodel))
                {
                    List<SecurityPassViewModel> lstuser = ExistingCheckUser(model, null);
                    if (lstuser != null && lstuser.Count > 0)
                    {
                        string errormessage = Translate.Text("Epass.alreadyapplied");
                        if (lstuser.Where(x => x.IsBlocked).HasAny())
                        {
                            errormessage = Translate.Text("Epass.userblockedtoapply");
                        }
                        ModelState.AddModelError(string.Empty, errormessage);
                        if (!ModelState.IsValid)
                        {
                            model.PageNo = 0;
                            return Json(new { status = false, Message = errormessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    model.PONumber = passmodel.PONumber;
                    model.PassType = EpassHelper.GetDisplayName(PassType.LongTerm);
                    List<poDetails> projectList = new List<poDetails>();
                    poDetails projectItem = null;
                    if (CacheProvider.TryGet(CacheKeys.EPASS_PROJECT_LIST, out projectList))
                    {
                        projectItem = projectList.Where(x => x.projectid.ToLower().Equals(model.PONumber.ToLower())).FirstOrDefault();
                    }
                    if (projectItem != null)
                    {
                        model.POName = projectItem.projectdescription;
                        model.Coor_eMail_IDs = projectItem.emailid.ToLower();
                        if (!string.IsNullOrWhiteSpace(projectItem.plannedstartdate))
                        {
                            DateTime.TryParse(projectItem.plannedstartdate, out DateTime pstartDate);
                            if (pstartDate.Ticks != 0)
                            {
                                model.projectStartDate = pstartDate;//FormatEpassDate(projectItem.plannedstartdate);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(projectItem.plannedenddate))
                        {
                            DateTime.TryParse(projectItem.plannedenddate, out DateTime pendDate);
                            if (pendDate.Ticks != 0)
                            {
                                model.projectEndDate = pendDate;//FormatEpassDate(projectItem.plannedenddate);
                            }
                        }
                        model.projectStatus = projectItem.status;
                        model.projectId = projectItem.projectid;
                        model.departmentName = projectItem.departmentname;
                        if (!string.IsNullOrWhiteSpace(projectItem.emailid))
                        {
                            string[] emailList = projectItem.emailid.Split(';');
                            char[] characters = new char[] { ';' };
                            string userList;
                            StringBuilder str = new StringBuilder();
                            foreach (string item in emailList)
                            {
                                str.Append(item.Substring(0, item.IndexOf('@')) + ";");
                            }
                            userList = str.ToString().TrimEnd(characters);
                            model.Coor_Username_List = userList.ToLower();
                        }
                    }
                    //model.CompanyName = passmodel.CompanyName;
                    string error = string.Empty;
                    ImageFileUploaderResponse _uploadPhotoResponse = new ImageFileUploaderResponse();
                    if (model.SinglePass_Photo != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Photo, SystemEnum.AttachmentType.Profile, "Profile", out _uploadPhotoResponse, out error);
                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            model.SinglePass_Photo_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_EmiratesID != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_EmiratesID, SystemEnum.AttachmentType.EID, "EID", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_EmiratesID", error);
                        }
                        else
                        {
                            model.SinglePass_EID_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_Passport != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Passport, SystemEnum.AttachmentType.Passport, "Passport", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Passport", error);
                        }
                        else
                        {
                            model.SinglePass_Passport_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_Visa != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Visa, SystemEnum.AttachmentType.VISA, "VISA", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Visa", error);
                        }
                        else
                        {
                            model.SinglePass_Visa_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (model.withcar && model.SinglePass_DrivingLicense != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_DrivingLicense, SystemEnum.AttachmentType.DrivingLicense, "DL", out _uploadPhotoResponse, out error);
                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_DrivingLicense", error);
                        }
                        else
                        {
                            model.SinglePass_DrivingLicense_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (model.withcar && model.SinglePass_VehicleRegistration != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_VehicleRegistration, SystemEnum.AttachmentType.Mulkiya, "VehicleRegistration", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_VehicleRegistration", error);
                        }
                        else
                        {
                            model.SinglePass_VehicleRegistration_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return Json(new { status = false, Message = "Error while submitting the request" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string UniquePassNumber = string.Format("{0}{1}{2}", "PP", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                        Tuple<bool, string, string> isSaved = SaveMainPass(UniquePassNumber, model);
                        string strerror = string.Empty;
                        if (isSaved.Item1 == true)
                        {
                            model.ReferenceNumber = UniquePassNumber;
                            List<CSVfileformat> csvlist = new List<CSVfileformat>();
                            if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_LIST, out csvlist))
                            {
                                csvlist.Where(x => x.ID.ToString().Equals(model.Serialnumber)).ToList().ForEach(
                                    x =>
                                    {
                                        x.registeredefolderid = UniquePassNumber;
                                        x.CustomerName = model.FullName;
                                    });
                                bool notcompleted = csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                                CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_LIST, new CacheItem<List<CSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                                model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
                                model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
                                string viewContent = ConvertViewToString("/Views/Feature/GatePass/ePass/Module/_ReviewForm.cshtml", new PermanentPass
                                {
                                    SubContractList = GetSubContractors(),
                                    Location = GetLocation(),
                                    Emirates = model.Emirates,
                                    PlateCategory = GetDetailForCatOrCode("", false)
                                });
                                return Json(new { status = true, completed = !notcompleted, result = viewContent });
                            }
                            return Json(new { status = true, Message = isSaved.Item2 }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            model.PageNo = 0;
                            ModelState.AddModelError(string.Empty, isSaved.Item2);
                            return Json(new { status = false, Message = isSaved.Item2 }, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// Create a new pass.
        /// </summary>
        /// <param name="model">.</param>
        /// <returns>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyPermanentPass(PermanentPass model)
        {
            try
            {
                if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
                }
                // epassCommon epasscommonmodel = new epassCommon();
                List<SecurityPassViewModel> PassModel = new List<SecurityPassViewModel>();
                if (model.PassSubmitType.Equals("single"))
                {
                    if ((string.IsNullOrWhiteSpace(model.PassNumber)))
                    {
                        List<SecurityPassViewModel> lstuser = ExistingCheckUser(model, null);
                        if (lstuser != null && lstuser.Count > 0)
                        {
                            string errormessage = Translate.Text("Epass.alreadyapplied");
                            if (lstuser.Where(x => x.IsBlocked).HasAny())
                            {
                                errormessage = Translate.Text("Epass.userblockedtoapply");
                            }
                            ModelState.AddModelError(string.Empty, errormessage);
                            if (!ModelState.IsValid)
                            {
                                model.PageNo = 2;
                                AssignDefaultvalues(model);
                                return PartialView("~/Views/Feature/GatePass/ePass/_ApplyPermanentPass.cshtml", model);
                            }
                        }
                    }
                    List<poDetails> projectList = new List<poDetails>();
                    poDetails projectItem = null;
                    if (CacheProvider.TryGet(CacheKeys.EPASS_PROJECT_LIST, out projectList))
                    {
                        projectItem = projectList.Where(x => x.projectid.ToLower().Equals(model.PONumber.ToLower())).FirstOrDefault();
                    }
                    model.PONumber = model.PONumber;

                    //model.PassType = "Permanent";
                    model.PassType = EpassHelper.GetDisplayName(PassType.LongTerm);

                    if (projectItem != null)
                    {
                        model.POName = projectItem.projectdescription;
                        model.Coor_eMail_IDs = projectItem.emailid.ToLower();
                        if (!string.IsNullOrWhiteSpace(projectItem.plannedstartdate))
                        {
                            DateTime.TryParse(projectItem.plannedstartdate, out DateTime pstartDate);
                            if (pstartDate.Ticks != 0)
                            {
                                model.projectStartDate = pstartDate;//FormatEpassDate(projectItem.plannedstartdate);
                            }
                        }
                        //model.Coor_eMail_IDs = "sivakumar.r@dewa.gov.ae";

                        if (!string.IsNullOrWhiteSpace(projectItem.plannedenddate))
                        {
                            DateTime.TryParse(projectItem.plannedenddate, out DateTime pendDate);
                            if (pendDate.Ticks != 0)
                            {
                                model.projectEndDate = pendDate;//FormatEpassDate(projectItem.plannedenddate);
                            }
                        }

                        model.projectStatus = projectItem.status;
                        model.projectId = projectItem.projectid;
                        model.departmentName = projectItem.departmentname;
                        if (!string.IsNullOrWhiteSpace(projectItem.emailid))
                        {
                            string[] emailList = projectItem.emailid.Split(';');
                            char[] characters = new char[] { ';' };
                            string userList;
                            StringBuilder str = new StringBuilder();
                            foreach (string item in emailList)
                            {
                                str.Append(item.Substring(0, item.IndexOf('@')) + ";");
                            }
                            userList = str.ToString().TrimEnd(characters);
                            model.Coor_Username_List = userList.ToLower();
                        }
                    }
                    model.CompanyName = model.CompanyName;
                    string error = string.Empty;
                    ImageFileUploaderResponse _uploadPhotoResponse = new ImageFileUploaderResponse();
                    if (model.SinglePass_Photo != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Photo, SystemEnum.AttachmentType.Profile, "Profile", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            model.SinglePass_Photo_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_EmiratesID != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_EmiratesID, SystemEnum.AttachmentType.EID, "EID", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_EmiratesID", error);
                        }
                        else
                        {
                            model.SinglePass_EID_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_Passport != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Passport, SystemEnum.AttachmentType.Passport, "Passport", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Passport", error);
                        }
                        else
                        {
                            model.SinglePass_Passport_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (!string.IsNullOrEmpty(model.VisaNumber))
                    {
                        if (model.SinglePass_Visa != null)
                        {
                            epasscommonmodel.SinglepassSubmit(model.SinglePass_Visa, SystemEnum.AttachmentType.VISA, "VISA", out _uploadPhotoResponse, out error);

                            if (!_uploadPhotoResponse.IsSucess)
                            {
                                ModelState.AddModelError("SinglePass_Visa", error);
                            }
                            else
                            {
                                model.SinglePass_Visa_Bytes = _uploadPhotoResponse.SingelFileBytes;
                            }
                        }
                    }
                    if (model.withcar && model.SinglePass_DrivingLicense != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_DrivingLicense, SystemEnum.AttachmentType.DrivingLicense, "DL", out _uploadPhotoResponse, out error);
                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_DrivingLicense", error);
                        }
                        else
                        {
                            model.SinglePass_DrivingLicense_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (model.withcar && model.SinglePass_VehicleRegistration != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_VehicleRegistration, SystemEnum.AttachmentType.Mulkiya, "VehicleRegistration", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_VehicleRegistration", error);
                        }
                        else
                        {
                            model.SinglePass_VehicleRegistration_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        model.PageNo = 2;
                        AssignDefaultvalues(model);
                        return PartialView("~/Views/Feature/GatePass/ePass/_ApplyPermanentPass.cshtml", model);
                    }
                    else
                    {
                        string UniquePassNumber = string.Empty;

                        if (!string.IsNullOrWhiteSpace(model.PassNumber))
                        {
                            UniquePassNumber = model.PassNumber;
                        }
                        else
                        {
                            UniquePassNumber = string.Format("{0}{1}{2}", "PP", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                        }

                        // If model is valid then post the Kofaxsss
                        Tuple<bool, string, string> isSaved = SaveMainPass(UniquePassNumber, model);
                        string strerror = string.Empty;
                        if (isSaved.Item1 == true)
                        {
                            model.ReferenceNumber = UniquePassNumber;
                            model.PassType = EpassHelper.GetDisplayName(PassType.LongTerm);
                            ViewBag.RedirectLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EPASS_PERMANENT_PASS);
                            return PartialView("~/Views/Feature/GatePass/ePass/_Success.cshtml", model);
                        }
                        else
                        {
                            model.PageNo = 0;
                            ModelState.AddModelError(string.Empty, isSaved.Item2);
                            AssignDefaultvalues(model);
                            return PartialView("~/Views/Feature/GatePass/ePass/_ApplyPermanentPass.cshtml", model);
                        }
                    }
                }
                else
                {
                    CSVFilepassType.expirydatecount = 29;
                    //model.PassType = "Permanent";
                    model.PassType = EpassHelper.GetDisplayName(PassType.LongTerm);
                    CSVfileimportwitherror result = CSVfileimport.fileread(model.GroupPass_Applicants);
                    if (result.errorlist != null && result.errorlist.Count > 0)
                    {
                        model.errorlist = result.errorlist;
                        ModelState.AddModelError("GroupPass_Applicants", Translate.Text("Epass.Grpinfomissing"));
                    }
                    else
                    {
                        model.errorlist = null;
                    }

                    List<CSVfileformat> records = result.passedrecords.ToList();
                    if (records != null && records.Count > 0)
                    {
                        List<SecurityPassViewModel> lstuser = ExistingCheckUser(null, records);
                        if (lstuser != null && lstuser.Count > 0)
                        {
                            List<ErrorDuplicateInfo> errorList = new List<ErrorDuplicateInfo>();
                            HashSet<string> diffeid = new HashSet<string>(lstuser.Select(s => s.emiratesId));
                            HashSet<string> diffpassport = new HashSet<string>(lstuser.Select(s => s.passportNumber));
                            HashSet<string> diffvisa = new HashSet<string>(lstuser.Select(s => s.visaNumber));
                            //You will have the difference here.w
                            List<CSVfileformat> results = records.Where(m => (!string.IsNullOrWhiteSpace(m.EmiratesID) && diffeid.Contains(m.EmiratesID)) || (!string.IsNullOrWhiteSpace(m.Passportnumber) && diffpassport.Contains(m.Passportnumber)) || (!string.IsNullOrWhiteSpace(m.Visanumber) && diffvisa.Contains(m.Visanumber))).ToList();

                            results.ForEach(x => errorList.Add(new ErrorDuplicateInfo
                            {
                                EmiratesID = x.EmiratesID,
                                Name = x.CustomerName,
                                PassportNumber = x.Passportnumber,
                                VisaNumber = x.Visanumber,
                                Errormessage = lstuser.Where(y => (y.emiratesId.Equals(x.EmiratesID) || y.passportNumber.Equals(x.Passportnumber)
                                || y.visaNumber.Equals(x.Visanumber))).HasAny() ? lstuser.Where(y => (y.emiratesId.Equals(x.EmiratesID) || y.passportNumber.Equals(x.Passportnumber)
                                || y.visaNumber.Equals(x.Visanumber))).FirstOrDefault().status.Equals(SecurityPassStatus.Blocked) ? Translate.Text("User is blocked.") : Translate.Text("Pass is already exists.") : string.Empty
                            }));
                            if (errorList.Count() > 0)
                            {
                                model.errorDuplicatelist = errorList;
                                ModelState.AddModelError("GroupPass_Applicants", Translate.Text("Epass.Grpinfomissing"));
                            }
                        }
                    }
                    if (!ModelState.IsValid)
                    {
                        model.PageNo = 3;
                        AssignDefaultvalues(model);
                        return PartialView("~/Views/Feature/GatePass/ePass/_ApplyPermanentPass.cshtml", model);
                    }

                    CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_REQUEST, new CacheItem<PermanentPass>(model, TimeSpan.FromMinutes(40)));
                    CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_LIST, new CacheItem<List<CSVfileformat>>(result.passedrecords, TimeSpan.FromMinutes(40)));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_MULTIPASS_REVIEW);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            AssignDefaultvalues(model);
            return PartialView("~/Views/Feature/GatePass/ePass/_ApplyPermanentPass.cshtml", model);
        }

        private void AssignDefaultvalues(PermanentPass model)
        {
            model.SubContractList = GetSubContractors();// GetSubcontractorList();
            model.Location = GetLocation();
            model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
            model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
            model.PlateCategory = GetDetailForCatOrCode("", false);
        }

        /// <summary>
        /// New pass new review form.
        /// </summary>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ApplyPermanentPassReview()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<CSVfileformat> csvlist = new List<CSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_LIST, out csvlist))
            {
                PermanentPass model = new PermanentPass();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_REQUEST, out model))
                {
                    AssignDefaultvalues(model);
                    return PartialView("~/Views/Feature/GatePass/ePass/ApplyPermanentPassReview.cshtml", model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_PERMANENT_PASS);
        }

        /// <summary>
        /// Apply new pass form.
        /// </summary>
        /// <param name="passNo">.</param>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ApplyShortTermPass(string passNo = null)
        {
            PermanentPass model = new PermanentPass();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (passNo != null)
            {
                model = GetPermanentPass(passNo);
            }
            AssignDefaultvalues(model);
            return PartialView("~/Views/Feature/GatePass/ePass/_ApplyShortTermPass.cshtml", model);
        }

        /// <summary>
        /// The ApplyShortTermPass.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyShortTermPass(PermanentPass model)
        {
            try
            {
                if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
                }
                // epassCommon epasscommonmodel = new epassCommon();
                List<SecurityPassViewModel> PassModel = new List<SecurityPassViewModel>();
                if (model.PassSubmitType.Equals("single"))
                {
                    if ((string.IsNullOrWhiteSpace(model.PassNumber)))
                    {
                        List<SecurityPassViewModel> lstuser = ExistingCheckUser(model, null);
                        if (lstuser != null && lstuser.Count > 0)
                        {
                            string errormessage = Translate.Text("Epass.alreadyapplied");
                            if (lstuser.Where(x => x.IsBlocked).HasAny())
                            {
                                errormessage = Translate.Text("Epass.userblockedtoapply");
                            }
                            ModelState.AddModelError(string.Empty, errormessage);
                            if (!ModelState.IsValid)
                            {
                                model.PageNo = 2;
                                AssignDefaultvalues(model);
                                return PartialView("~/Views/Feature/GatePass/ePass/_ApplyShortTermPass.cshtml", model);
                            }
                        }
                    }
                    List<poDetails> projectList = new List<poDetails>();
                    poDetails projectItem = null;
                    if (CacheProvider.TryGet(CacheKeys.EPASS_PROJECT_LIST, out projectList))
                    {
                        projectItem = projectList.Where(x => x.projectid.ToLower().Equals(model.PONumber.ToLower())).FirstOrDefault();
                    }
                    model.PONumber = model.PONumber;

                    model.PassType = EpassHelper.GetDisplayName(PassType.ShortTerm);
                    if (projectItem != null)
                    {
                        model.POName = projectItem.projectdescription;
                        model.Coor_eMail_IDs = projectItem.emailid.ToLower();
                        if (!string.IsNullOrWhiteSpace(projectItem.plannedstartdate))
                        {
                            DateTime.TryParse(projectItem.plannedstartdate, out DateTime pstartDate);
                            if (pstartDate.Ticks != 0)
                            {
                                model.projectStartDate = pstartDate;//FormatEpassDate(projectItem.plannedstartdate);
                            }
                        }
                        //model.Coor_eMail_IDs = "sivakumar.r@dewa.gov.ae";

                        if (!string.IsNullOrWhiteSpace(projectItem.plannedenddate))
                        {
                            DateTime.TryParse(projectItem.plannedenddate, out DateTime pendDate);
                            if (pendDate.Ticks != 0)
                            {
                                model.projectEndDate = pendDate;//FormatEpassDate(projectItem.plannedenddate);
                            }
                        }

                        model.projectStatus = projectItem.status;
                        model.projectId = projectItem.projectid;
                        model.departmentName = projectItem.departmentname;
                        if (!string.IsNullOrWhiteSpace(projectItem.emailid))
                        {
                            string[] emailList = projectItem.emailid.Split(';');
                            char[] characters = new char[] { ';' };
                            string userList;
                            StringBuilder str = new StringBuilder();
                            foreach (string item in emailList)
                            {
                                str.Append(item.Substring(0, item.IndexOf('@')) + ";");
                            }
                            userList = str.ToString().TrimEnd(characters);
                            model.Coor_Username_List = userList.ToLower();
                        }
                    }
                    model.CompanyName = model.CompanyName;

                    string error = string.Empty;
                    ImageFileUploaderResponse _uploadPhotoResponse = new ImageFileUploaderResponse();
                    if (model.SinglePass_Photo != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Photo, SystemEnum.AttachmentType.Profile, "Profile", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            model.SinglePass_Photo_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_EmiratesID != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_EmiratesID, SystemEnum.AttachmentType.EID, "EID", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_EmiratesID", error);
                        }
                        else
                        {
                            model.SinglePass_EID_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_Passport != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Passport, SystemEnum.AttachmentType.Passport, "Passport", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Passport", error);
                        }
                        else
                        {
                            model.SinglePass_Passport_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (!string.IsNullOrEmpty(model.VisaNumber))
                    {
                        if (model.SinglePass_Visa != null)
                        {
                            epasscommonmodel.SinglepassSubmit(model.SinglePass_Visa, SystemEnum.AttachmentType.VISA, "VISA", out _uploadPhotoResponse, out error);

                            if (!_uploadPhotoResponse.IsSucess)
                            {
                                ModelState.AddModelError("SinglePass_Visa", error);
                            }
                            else
                            {
                                model.SinglePass_Visa_Bytes = _uploadPhotoResponse.SingelFileBytes;
                            }
                        }
                    }

                    if (model.withcar && model.SinglePass_DrivingLicense != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_DrivingLicense, SystemEnum.AttachmentType.DrivingLicense, "DL", out _uploadPhotoResponse, out error);
                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_DrivingLicense", error);
                        }
                        else
                        {
                            model.SinglePass_DrivingLicense_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (model.withcar && model.SinglePass_VehicleRegistration != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_VehicleRegistration, SystemEnum.AttachmentType.Mulkiya, "VehicleRegistration", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_VehicleRegistration", error);
                        }
                        else
                        {
                            model.SinglePass_VehicleRegistration_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        model.PageNo = 2;
                        AssignDefaultvalues(model);
                        return PartialView("~/Views/Feature/GatePass/ePass/_ApplyShortTermPass.cshtml", model);
                    }
                    else
                    {
                        string UniquePassNumber = string.Empty;

                        if (!string.IsNullOrWhiteSpace(model.eFolderId))
                        {
                            UniquePassNumber = model.PassNumber;
                        }
                        else
                        {
                            UniquePassNumber = string.Format("{0}{1}{2}", "ST", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                        }

                        // If model is valid then post to Kofax
                        Tuple<bool, string, string> isSaved = SaveMainPass(UniquePassNumber, model);
                        string strerror = string.Empty;
                        if (isSaved.Item1 == true)
                        {
                            model.ReferenceNumber = UniquePassNumber;
                            model.PassType = EpassHelper.GetDisplayName(PassType.ShortTerm);
                            ViewBag.RedirectLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EPASS_SHORTTERM_PASS);
                            return PartialView("~/Views/Feature/GatePass/ePass/_Success.cshtml", model);
                        }
                        else
                        {
                            model.PageNo = 0;
                            ModelState.AddModelError(string.Empty, isSaved.Item2);
                            AssignDefaultvalues(model);
                            return PartialView("~/Views/Feature/GatePass/ePass/_ApplyShortTermPass.cshtml", model);
                        }
                    }
                }
                else
                {
                    CSVFilepassType.expirydatecount = 0;
                    //model.PassType = "Shortterm";
                    model.PassType = EpassHelper.GetDisplayName(PassType.ShortTerm);
                    CSVfileimportwitherror result = CSVfileimport.fileread(model.GroupPass_Applicants);
                    if (result.errorlist != null && result.errorlist.Count > 0)
                    {
                        model.errorlist = result.errorlist;
                        ModelState.AddModelError("GroupPass_Applicants", Translate.Text("Epass.Grpinfomissing"));
                    }
                    else
                    {
                        model.errorlist = null;
                    }

                    List<CSVfileformat> records = result.passedrecords.ToList();
                    if (records != null && records.Count > 0)
                    {
                        List<SecurityPassViewModel> lstuser = ExistingCheckUser(null, records);
                        if (lstuser != null && lstuser.Count > 0)
                        {
                            List<ErrorDuplicateInfo> errorList = new List<ErrorDuplicateInfo>();
                            HashSet<string> diffeid = new HashSet<string>(lstuser.Select(s => s.emiratesId));
                            HashSet<string> diffpassport = new HashSet<string>(lstuser.Select(s => s.passportNumber));
                            HashSet<string> diffvisa = new HashSet<string>(lstuser.Select(s => s.visaNumber));
                            //You will have the difference here.w
                            List<CSVfileformat> results = records.Where(m => (!string.IsNullOrWhiteSpace(m.EmiratesID) && diffeid.Contains(m.EmiratesID)) || (!string.IsNullOrWhiteSpace(m.Passportnumber) && diffpassport.Contains(m.Passportnumber)) || (!string.IsNullOrWhiteSpace(m.Visanumber) && diffvisa.Contains(m.Visanumber))).ToList();

                            results.ForEach(x => errorList.Add(new ErrorDuplicateInfo
                            {
                                EmiratesID = x.EmiratesID,
                                Name = x.CustomerName,
                                PassportNumber = x.Passportnumber,
                                VisaNumber = x.Visanumber,
                                Errormessage = lstuser.Where(y => (y.emiratesId.Equals(x.EmiratesID) || y.passportNumber.Equals(x.Passportnumber)
                                || y.visaNumber.Equals(x.Visanumber))).HasAny() ? lstuser.Where(y => (y.emiratesId.Equals(x.EmiratesID) || y.passportNumber.Equals(x.Passportnumber)
                                || y.visaNumber.Equals(x.Visanumber))).FirstOrDefault().status.Equals(SecurityPassStatus.Blocked) ? Translate.Text("User is blocked.") : Translate.Text("Pass is already exists.") : string.Empty
                            }));
                            if (errorList.Count() > 0)
                            {
                                model.errorDuplicatelist = errorList;
                                ModelState.AddModelError("GroupPass_Applicants", Translate.Text("Epass.Grpinfomissing"));
                            }
                        }
                    }
                    if (!ModelState.IsValid)
                    {
                        model.PageNo = 3;
                        AssignDefaultvalues(model);
                        return PartialView("~/Views/Feature/GatePass/ePass/_ApplyShortTermPass.cshtml", model);
                    }

                    CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_SHORTTERM_REQUEST, new CacheItem<PermanentPass>(model, TimeSpan.FromMinutes(40)));
                    CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_SHORTTERM_LIST, new CacheItem<List<CSVfileformat>>(result.passedrecords, TimeSpan.FromMinutes(40)));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_MULTIPASS_SHORTTERM_REVIEW);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            AssignDefaultvalues(model);
            return PartialView("~/Views/Feature/GatePass/ePass/_ApplyShortTermPass.cshtml", model);
        }

        /// <summary>
        /// New pass new review form.
        /// </summary>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ApplyShortTermPassReview()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<CSVfileformat> csvlist = new List<CSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_SHORTTERM_LIST, out csvlist))
            {
                PermanentPass model = new PermanentPass();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_SHORTTERM_REQUEST, out model))
                {
                    AssignDefaultvalues(model);
                    return PartialView("~/Views/Feature/GatePass/ePass/Module/ApplyShortTermPassReview.cshtml", model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_SHORTTERM_PASS);
        }

        /// <summary>
        /// The ProcessMultipassShortterm.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProcessMultipassShortterm(PermanentPass model)
        {
            try
            {
                PermanentPass passmodel = new PermanentPass();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_SHORTTERM_REQUEST, out passmodel))
                {
                    List<SecurityPassViewModel> lstuser = ExistingCheckUser(model, null);
                    if (lstuser != null && lstuser.Count > 0)
                    {
                        string errormessage = Translate.Text("Epass.alreadyapplied");
                        if (lstuser.Where(x => x.IsBlocked).HasAny())
                        {
                            errormessage = Translate.Text("Epass.userblockedtoapply");
                        }
                        ModelState.AddModelError(string.Empty, errormessage);
                        if (!ModelState.IsValid)
                        {
                            model.PageNo = 0;
                            return Json(new { status = false, Message = errormessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    model.PONumber = passmodel.PONumber;
                    //model.POName = passmodel.POName;
                    model.PassType = EpassHelper.GetDisplayName(PassType.ShortTerm);
                    List<poDetails> projectList = new List<poDetails>();
                    poDetails projectItem = null;
                    if (CacheProvider.TryGet(CacheKeys.EPASS_PROJECT_LIST, out projectList))
                    {
                        projectItem = projectList.Where(x => x.projectid.ToLower().Equals(model.PONumber.ToLower())).FirstOrDefault();
                    }
                    if (projectItem != null)
                    {
                        model.POName = projectItem.projectdescription;
                        model.Coor_eMail_IDs = projectItem.emailid.ToLower();
                        if (!string.IsNullOrWhiteSpace(projectItem.plannedstartdate))
                        {
                            DateTime.TryParse(projectItem.plannedstartdate, out DateTime pstartDate);
                            if (pstartDate.Ticks != 0)
                            {
                                model.projectStartDate = pstartDate;//FormatEpassDate(projectItem.plannedstartdate);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(projectItem.plannedenddate))
                        {
                            DateTime.TryParse(projectItem.plannedenddate, out DateTime pendDate);
                            if (pendDate.Ticks != 0)
                            {
                                model.projectEndDate = pendDate;//FormatEpassDate(projectItem.plannedenddate);
                            }
                        }
                        model.projectStatus = projectItem.status;
                        model.projectId = projectItem.projectid;
                        model.departmentName = projectItem.departmentname;
                        if (!string.IsNullOrWhiteSpace(projectItem.emailid))
                        {
                            string[] emailList = projectItem.emailid.Split(';');
                            char[] characters = new char[] { ';' };
                            string userList;
                            StringBuilder str = new StringBuilder();
                            foreach (string item in emailList)
                            {
                                str.Append(item.Substring(0, item.IndexOf('@')) + ";");
                            }
                            userList = str.ToString().TrimEnd(characters);
                            model.Coor_Username_List = userList.ToLower();
                        }
                    }
                    //model.CompanyName = passmodel.CompanyName;
                    string error = string.Empty;
                    ImageFileUploaderResponse _uploadPhotoResponse = new ImageFileUploaderResponse();
                    if (model.SinglePass_Photo != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Photo, SystemEnum.AttachmentType.Profile, "Profile", out _uploadPhotoResponse, out error);
                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Photo", error);
                        }
                        else
                        {
                            model.SinglePass_Photo_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_EmiratesID != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_EmiratesID, SystemEnum.AttachmentType.EID, "EID", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_EmiratesID", error);
                        }
                        else
                        {
                            model.SinglePass_EID_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_Passport != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Passport, SystemEnum.AttachmentType.Passport, "Passport", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Passport", error);
                        }
                        else
                        {
                            model.SinglePass_Passport_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }
                    if (model.SinglePass_Visa != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_Visa, SystemEnum.AttachmentType.VISA, "VISA", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_Visa", error);
                        }
                        else
                        {
                            model.SinglePass_Visa_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (model.withcar && model.SinglePass_DrivingLicense != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_DrivingLicense, SystemEnum.AttachmentType.DrivingLicense, "DL", out _uploadPhotoResponse, out error);
                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_DrivingLicense", error);
                        }
                        else
                        {
                            model.SinglePass_DrivingLicense_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (model.withcar && model.SinglePass_VehicleRegistration != null)
                    {
                        epasscommonmodel.SinglepassSubmit(model.SinglePass_VehicleRegistration, SystemEnum.AttachmentType.Mulkiya, "VehicleRegistration", out _uploadPhotoResponse, out error);

                        if (!_uploadPhotoResponse.IsSucess)
                        {
                            ModelState.AddModelError("SinglePass_VehicleRegistration", error);
                        }
                        else
                        {
                            model.SinglePass_VehicleRegistration_Bytes = _uploadPhotoResponse.SingelFileBytes;
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return Json(new { status = false, Message = "Error while submitting the request" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string UniquePassNumber = string.Format("{0}{1}{2}", "ST", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                        Tuple<bool, string, string> isSaved = SaveMainPass(UniquePassNumber, model);
                        string strerror = string.Empty;
                        if (isSaved.Item1 == true)
                        {
                            model.ReferenceNumber = UniquePassNumber;
                            List<CSVfileformat> csvlist = new List<CSVfileformat>();
                            if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_SHORTTERM_LIST, out csvlist))
                            {
                                csvlist.Where(x => x.ID.ToString().Equals(model.Serialnumber)).ToList().ForEach(
                                    x =>
                                    {
                                        x.registeredefolderid = UniquePassNumber;
                                        x.CustomerName = model.FullName;
                                    });
                                bool notcompleted = csvlist.Any(x => string.IsNullOrWhiteSpace(x.registeredefolderid));
                                CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_SHORTTERM_LIST, new CacheItem<List<CSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                                model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
                                model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
                                string viewContent = ConvertViewToString("/Views/Feature/GatePass/ePass/Module/_ShortTermReviewForm.cshtml", new PermanentPass
                                {
                                    SubContractList = GetSubContractors(),
                                    Location = GetLocation(),
                                    Emirates = model.Emirates,
                                    PlateCategory = GetDetailForCatOrCode("", false)
                                });
                                return Json(new { status = true, completed = !notcompleted, result = viewContent });
                            }
                            return Json(new { status = true, Message = isSaved.Item2 }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            model.PageNo = 0;
                            ModelState.AddModelError(string.Empty, isSaved.Item2);
                            return Json(new { status = false, Message = isSaved.Item2 }, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// Remove pass from multi pass list.
        /// </summary>
        /// <param name="id">.</param>
        /// <param name="eid">.</param>
        /// <returns>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RemoveEpassentry(string id, string eid)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<CSVfileformat> csvlist = new List<CSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_LIST, out csvlist))
            {
                CSVfileformat itemToRemove = csvlist.SingleOrDefault(r => r.ID.ToString().Equals(id) && r.EmiratesID.Equals(eid));
                if (itemToRemove != null)
                {
                    csvlist.Remove(itemToRemove);
                    CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_LIST, new CacheItem<List<CSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                }

                return Json(new { Message = true });
            }
            else if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_SHORTTERM_LIST, out csvlist))
            {
                CSVfileformat itemToRemove = csvlist.SingleOrDefault(r => r.ID.ToString().Equals(id) && r.EmiratesID.Equals(eid));
                if (itemToRemove != null)
                {
                    csvlist.Remove(itemToRemove);
                    CacheProvider.Store(CacheKeys.EPASS_MULTIPASS_SHORTTERM_LIST, new CacheItem<List<CSVfileformat>>(csvlist, TimeSpan.FromMinutes(40)));
                }

                return Json(new { Message = true });
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_PERMANENT_PASS);
        }

        /// <summary>
        /// Pass services page.
        /// </summary>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ePassServices()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            LocationModel lmodel = new LocationModel { MainLocation = "generation" };

            return View("~/Views/Feature/GatePass/ePass/Module/_SelectLocation.cshtml", lmodel);
        }

        /// <summary>
        /// The ePassServices.
        /// </summary>
        /// <param name="model">The model<see cref="LocationModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ePassServices(LocationModel model)
        {
            List<string> mLoc = new List<string>() { "generation", "other" };

            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }

            if (mLoc.Contains(model.MainLocation))
            {
                CacheProvider.Store(CacheKeys.EPASS_LOCMAIN, new CacheItem<string>(model.MainLocation));

                if (mLoc.IndexOf(model.MainLocation) == 0)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_GENERATION_LANDING_PAGE);
                }
                else
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_OTHERS_LANDING_PAGE);
                }
            }

            LocationModel lmodel = new LocationModel() /*{ OfficeLocations = GetDewaOfficeLocationFromSitecore() } */;

            return View("~/Views/Feature/GatePass/ePass/Module/_SelectLocation.cshtml", lmodel);
        }

        /// <summary>
        /// The ApplyNewPassForGenerations.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ApplyNewPassForGenerations()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// The ApplyNewPassForOtherOffice.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ApplyNewPassForOtherOffice()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Display Subcontractor Form.
        /// </summary>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ePassSubContractor()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }

            return PartialView("~/Views/Feature/GatePass/ePass/_SubContrator.cshtml");
        }

        /// <summary>
        /// Add Subcontractor Form to eForm.
        /// </summary>
        /// <param name="model">.</param>
        /// <returns>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ePassSubContractor(ePassSubContractor model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }

            //Add data to subcontractor
            Tuple<bool, string, string> isSaved = AddSubContractor(model);

            if (isSaved.Item1 == true)
            {
                return PartialView("~/Views/Feature/GatePass/ePass/_SubcontractorSuccess.cshtml", model);
            }
            else
            {
                ModelState.AddModelError("", isSaved.Item2);
            }

            return PartialView("~/Views/Feature/GatePass/ePass/_SubContrator.cshtml", model);
        }

        /// <summary>
        /// Pass dashboard page.
        /// </summary>
        /// <returns>.</returns>
        [HttpGet]
        public ActionResult ePassDashBoard()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            ePassLogin model = new ePassLogin
            {
                UserName = CurrentPrincipal.Name,
                lstpassess = GetListofPasses(),
                UserType = CurrentPrincipal.Role,
                Userid = CurrentPrincipal.Username
            };
            return PartialView("~/Views/Feature/GatePass/ePass/_DashBoard.cshtml", model);
        }

        /// <summary>
        /// The DisplayPOList.
        /// </summary>
        /// <param name="model">The model<see cref="AccountListModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult DisplayPOList(AccountListModel model)
        {
            try
            {
                int pageNo = model.PageNo <= 1 ? 1 : model.PageNo;
                int pageSize = 5;
                ServiceResponse<poDetailsOutput> response = GatePassServiceClient.ProjectList(new userDataInput
                {
                    supplierid = CurrentPrincipal.BusinessPartner,
                    key = CurrentPrincipal.SessionToken,
                    userid = CurrentPrincipal.Username,
                    sessionid = CurrentPrincipal.SessionToken
                }, RequestLanguage, Request.Segment());
                //var response = VendorServiceClient.GetPOList(CurrentPrincipal.BusinessPartner, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.podetailslist != null)
                {
                    IEnumerable<poDetails> polist = response.Payload.podetailslist.Where(x => !string.IsNullOrWhiteSpace(x.emailid));
                    if (!string.IsNullOrEmpty(model.Search))
                    {
                        polist = polist.ToList().Where(x => x != null && !string.IsNullOrEmpty(x.projectid) && !string.IsNullOrEmpty(x.projectname.ToLower()) && !string.IsNullOrEmpty(x.projectdescription.ToLower()) &&
                        (x.projectid.Contains(model.Search.ToLower()) || x.projectname.ToLower().Contains(model.Search.ToLower()) || x.projectdescription.ToLower().Contains(model.Search.ToLower()))).ToArray();
                    }
                    model.ITEMPagedList = polist.ToPagedList(pageNo, pageSize);
                    model.supplierid = response.Payload.supplierid.ToLower();
                    model.suppliername = response.Payload.suppliername.ToLower();
                    model.suppliertelephonenumber = response.Payload.suppliertelephonenumber.ToLower();

                    CacheProvider.Store(CacheKeys.EPASS_PROJECT_LIST, new CacheItem<List<poDetails>>(response.Payload.podetailslist.ToList(), TimeSpan.FromMinutes(100)));
                }
                return PartialView("~/Views/Feature/GatePass/ePass/Module/_POList.cshtml", model);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error--") + ex.Message);
                return PartialView("~/Views/Feature/GatePass/ePass/Module/_POList.cshtml");
            }
        }

        /// <summary>
        /// The ePassLogin.
        /// </summary>
        /// <param name="returnUrl">The returnUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AcceptVerbs("GET", "HEAD")]
        public ActionResult ePassLogin(string returnUrl)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                ViewBag.ReturnUrl = returnUrl;
                return PartialView("~/Views/Feature/GatePass/ePass/_Login.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// ePass Login to verify SRM user.
        /// </summary>
        /// <param name="model">.</param>
        /// <returns>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ePassLogin(ePassLogin model)
        {
            if (TryLogin(model, out string responseMessage))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
            }

            ModelState.AddModelError(string.Empty, responseMessage);

            return PartialView("~/Views/Feature/GatePass/ePass/_Login.cshtml");
        }

        /// <summary>
        /// The ePassLogout.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ePassLogout()
        {
            //CacheProvider.Remove(CacheKeys.EPASS_USER_MODEL);
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
            ServiceResponse<logoutDetailsOutput> response = GatePassServiceClient.UserLogout(new logInOutDataInput
            {
                userid = CurrentPrincipal.UserId,
                key = CurrentPrincipal.SessionToken
            }, RequestLanguage, Request.Segment());
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
        }

        /// <summary>
        /// The Createaccount.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AcceptVerbs("GET", "HEAD")]
        public ActionResult Createaccount()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return View("~/Views/Feature/GatePass/ePass/Createaccount.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The Createaccount.
        /// </summary>
        /// <param name="model">The model<see cref="VendorLoginModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Createaccount(VendorLoginModel model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                ServiceResponse<supplierDetailsOutput> response = GatePassServiceClient.GetGPSupplierDetails(new DEWAXP.Foundation.Integration.GatePassSvc.userDataInput { emailid = model.Emailaddress, supplierid = model.VendorId, mode = "S" }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    CacheProvider.Store(CacheKeys.EPASS_CREATEACCOUNT_SUCCESS, new AccessCountingCacheItem<VendorLoginModel>(model, Times.Max));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_CREATEACCOUNT_SUCCESS);
                }
                ModelState.AddModelError(string.Empty, response.Message);
                return View("~/Views/Feature/GatePass/ePass/Createaccount.cshtml",model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The CreateaccountSuccess.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult CreateaccountSuccess()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                if (CacheProvider.TryGet(CacheKeys.EPASS_CREATEACCOUNT_SUCCESS, out VendorLoginModel data))
                {
                    CacheProvider.Remove(CacheKeys.EPASS_CREATEACCOUNT_SUCCESS);
                    return View("~/Views/Feature/GatePass/ePass/CreateaccountSuccess.cshtml",data);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The UserRegistration.
        /// </summary>
        /// <param name="key">The key<see cref="string"/>.</param>
        /// <param name="vendorid">The vendorid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult UserRegistration(string key, string vendorid)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(vendorid))
                {
                    if (CacheProvider.TryGet(CacheKeys.EPASS_ERROR_MESSAGE, out string errorMessage))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                        CacheProvider.Remove(CacheKeys.EPASS_ERROR_MESSAGE);
                    }
                    ServiceResponse<userDetailsOutput> response = GatePassServiceClient.CreateGPUser(new createDataInput { key = key, supplierid = vendorid, mode = "V" }, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null && response.Payload.userdetails != null)
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
                        CacheProvider.Store(CacheKeys.EPASS_CREATEACCOUNT_USERREGISTER, new CacheItem<userDetails>(response.Payload.userdetails, TimeSpan.FromMinutes(20)));
                        return View("~/Views/Feature/GatePass/ePass/UserRegistration.cshtml",new UserRegistration
                        {
                            CompanyName = response.Payload.userdetails.suppliername,
                            VendorId = vendorid
                        });
                    }
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The UserRegistration.
        /// </summary>
        /// <param name="model">The model<see cref="UserRegistration"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserRegistration(UserRegistration model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                if (CacheProvider.TryGet(CacheKeys.EPASS_CREATEACCOUNT_USERREGISTER, out userDetails data))
                {
                    bool status = false;
                    string recaptchaResponse = Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

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
                        ServiceResponse<userDetailsOutput> response = GatePassServiceClient.CreateGPUser(new createDataInput
                        {
                            key = data.key,
                            supplierid = data.supplierid,
                            mode = "C",
                            contactemailid = model.Emailid,
                            contactmobilenumber = model.MobilePhone,
                            contactname = model.Fullname,
                            userid = model.Username?.ToLower(),
                            password = model.Password,
                            suppliername = data.suppliername
                        }, RequestLanguage, Request.Segment()
                    );
                        if (response != null && response.Succeeded && response.Payload != null)
                        {
                            CacheProvider.Store(CacheKeys.EPASS_USERREGISTER_SUCCESS, new AccessCountingCacheItem<userDetailsOutput>(response.Payload, Times.Max));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_USERREGISTRATION_SUCCESS);
                        }
                        CacheProvider.Store(CacheKeys.EPASS_ERROR_MESSAGE, new CacheItem<string>(response.Message));
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.EPASS_ERROR_MESSAGE, new CacheItem<string>(Translate.Text("unsubscribe-Captcha-Not-Valid")));
                    }

                    if (CacheProvider.TryGet(CacheKeys.EPASS_ERROR_MESSAGE, out string errorMessage))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                        CacheProvider.Remove(CacheKeys.EPASS_ERROR_MESSAGE);
                    }
                    ServiceResponse<userDetailsOutput> response1 = GatePassServiceClient.CreateGPUser(new createDataInput { key = data.key, supplierid = data.supplierid, mode = "V" }, RequestLanguage, Request.Segment());
                    if (response1 != null && response1.Succeeded && response1.Payload != null && response1.Payload.userdetails != null)
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
                        CacheProvider.Store(CacheKeys.EPASS_CREATEACCOUNT_USERREGISTER, new CacheItem<userDetails>(response1.Payload.userdetails, TimeSpan.FromMinutes(20)));
                    }
                    model.CompanyName = response1.Payload.userdetails.suppliername;
                    model.VendorId = data.supplierid;
                    model.Password = string.Empty; model.ConfirmationPassword = string.Empty;
                    return View("~/Views/Feature/GatePass/ePass/UserRegistration.cshtml",model);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The UserRegistrationSuccess.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult UserRegistrationSuccess()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                if (CacheProvider.TryGet(CacheKeys.EPASS_USERREGISTER_SUCCESS, out userDetailsOutput data))
                {
                    CacheProvider.Remove(CacheKeys.EPASS_USERREGISTER_SUCCESS);
                    return View("~/Views/Feature/GatePass/ePass/UserRegistrationSuccess.cshtml");
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The Useractivation.
        /// </summary>
        /// <param name="key">The key<see cref="string"/>.</param>
        /// <param name="userid">The userid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Useractivation(string key, string userid)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(userid))
                {
                    ServiceResponse<userDetailsOutput> response = GatePassServiceClient.UserActivation(new userDataInput { key = key, userid = userid }, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        return View("~/Views/Feature/GatePass/ePass/Useractivation.cshtml");
                    }
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The ForgotUserid.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ForgotUserid()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return View("~/Views/Feature/GatePass/ePass/ForgotUserid.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The ForgotUserid.
        /// </summary>
        /// <param name="model">The model<see cref="VendorLoginModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotUserid(VendorLoginModel model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                ServiceResponse<userDetailsOutput> response = GatePassServiceClient.ForgotUserid(new credentialDataInput { contactemailid = model.Emailaddress, supplierid = model.VendorId }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    ViewBag.success = true;
                    return View("~/Views/Feature/GatePass/ePass/ForgotUserid.cshtml",model);
                }
                ModelState.AddModelError(string.Empty, response.Message);
                return View("~/Views/Feature/GatePass/ePass/ForgotUserid.cshtml",model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The ForgotPassword.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return View("~/Views/Feature/GatePass/ePass/ForgotPassword.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The ForgotPassword.
        /// </summary>
        /// <param name="model">The model<see cref="VendorLoginModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(VendorLoginModel model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                ServiceResponse<userDetailsOutput> response = GatePassServiceClient.ForgotPassword(new credentialDataInput { contactemailid = model.Emailaddress, userid = model.VendorId }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    ViewBag.success = true;
                    return View("~/Views/Feature/GatePass/ePass/ForgotPassword.cshtml",model);
                }
                ModelState.AddModelError(string.Empty, response.Message);
                return View("~/Views/Feature/GatePass/ePass/ForgotPassword.cshtml",model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The SetNewPassword.
        /// </summary>
        /// <param name="key">The key<see cref="string"/>.</param>
        /// <param name="userid">The userid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult SetNewPassword(string key, string userid)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(userid))
                {
                    if (CacheProvider.TryGet(CacheKeys.EPASS_ERROR_MESSAGE, out string errorMessage))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                        CacheProvider.Remove(CacheKeys.EPASS_ERROR_MESSAGE);
                    }
                    ServiceResponse<userDetailsOutput> response = GatePassServiceClient.SetNewPassword(new credentialDataInput { key = key, userid = userid, mode = "V" }, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        return View("~/Views/Feature/GatePass/ePass/SetNewPassword.cshtml", new SetNewPassword { key = key, userid = userid });
                    }
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
        }

        /// <summary>
        /// The SetNewPassword.
        /// </summary>
        /// <param name="model">The model<see cref="SetNewPassword"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(SetNewPassword model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                ServiceResponse<userDetailsOutput> response = GatePassServiceClient.SetNewPassword(new credentialDataInput { key = model.key, userid = model.userid, password = model.ConfirmPassword, mode = "S" }, RequestLanguage, Request.Segment());
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    ViewBag.success = true;
                    return View("~/Views/Feature/GatePass/ePass/SetNewPassword.cshtml",new SetNewPassword());
                }
                else
                {
                    CacheProvider.Store(CacheKeys.EPASS_ERROR_MESSAGE, new CacheItem<string>(response.Message));
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_SETNEWPASSWORD, new QueryString(false)
                       .With("key", model.key)
                       .With("userid", model.userid));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The MypassesAjax.
        /// </summary>
        /// <param name="pagesize">The pagesize<see cref="int"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <param name="statustxt">The statustxt<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="namesort">The namesort<see cref="string"/>.</param>
        /// <param name="passfilter">The passfilter<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MypassesAjax(int pagesize = 5, string keyword = "", string statustxt = "all", int page = 1, string namesort = "", string passfilter = "")
        {
            keyword = keyword.Trim();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstmodel))
            {
                SecurityPassFilterViewModel SecurityPassFilterViewModel = new SecurityPassFilterViewModel
                {
                    page = page
                };
                pagesize = pagesize > 100 ? 100 : pagesize;
                SecurityPassFilterViewModel.strdataindex = "0";
                if (lstmodel != null && lstmodel.Count > 0)
                {
                    //lstmodel = lstmodel.Where(x => !x.Subpass).ToList();
                    if (!string.IsNullOrWhiteSpace(passfilter))
                    {
                        if (passfilter.ToLower().Equals("main"))
                        {
                            lstmodel = lstmodel.Where(x => !x.Subpass).ToList();
                            SecurityPassFilterViewModel.passtypefilter = "1";
                        }
                        else if (passfilter.ToLower().Equals("subpass"))
                        {
                            lstmodel = lstmodel.Where(x => x.Subpass).ToList();
                            SecurityPassFilterViewModel.passtypefilter = "2";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(statustxt))
                    {
                        if (statustxt.ToLower().Equals("expired"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Expired).ToList();
                        }
                        else if (statustxt.ToLower().Equals("soontoexpire"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.SoontoExpire).ToList();
                        }
                        else if (statustxt.ToLower().Equals("active"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire).ToList();
                        }
                        else if (statustxt.ToLower().Equals("pending"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.UnderApprovalinWorkPermit || x.status == SecurityPassStatus.PendingApprovalwithSecurity || x.status == SecurityPassStatus.PendingApprovalwithCoordinator).ToList();
                        }
                        else if (statustxt.ToLower().Equals("blocked"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Blocked).ToList();
                        }
                        else if (statustxt.ToLower().Equals("initiated"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Initiated).ToList();
                        }
                        else if (statustxt.ToLower().Equals("cancelled"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Cancelled).ToList();
                        }
                        else if (statustxt.ToLower().Equals("rejected"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Rejected).ToList();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        lstmodel = lstmodel.Where(x => (!string.IsNullOrWhiteSpace(x.name) && x.name.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.emiratesId) && x.emiratesId.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.passportNumber) && x.passportNumber.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.visaNumber) && x.visaNumber.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.passNumber) && x.passNumber.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.grouppassid) && x.grouppassid.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.mainpassNumber) && x.mainpassNumber.ToLower().Contains(keyword.ToLower()))
                        ).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(namesort))
                    {
                        if (namesort.ToLower().Equals("ascending"))
                        {
                            lstmodel = lstmodel.OrderBy(x => x.name).ToList();
                        }
                        else if (namesort.ToLower().Equals("descending"))
                        {
                            lstmodel = lstmodel.OrderByDescending(x => x.name).ToList();
                        }
                    }
                    SecurityPassFilterViewModel.namesort = namesort;
                    SecurityPassFilterViewModel.totalpage = Pager.CalculateTotalPages(lstmodel.Count(), pagesize);
                    SecurityPassFilterViewModel.pagination = SecurityPassFilterViewModel.totalpage > 1 ? true : false;
                    SecurityPassFilterViewModel.pagenumbers = SecurityPassFilterViewModel.totalpage > 1 ? Pager.GetPaginationRange(page, SecurityPassFilterViewModel.totalpage) : new List<int>();
                    lstmodel = lstmodel.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    SecurityPassFilterViewModel.lstpasses = new JavaScriptSerializer().Serialize(lstmodel);
                    return Json(new { status = true, Message = SecurityPassFilterViewModel }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The SharePassDetails.
        /// </summary>
        /// <param name="passNO">The passNO<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SharePassDetails(string passNO)
        {
            List<SecurityPassViewModel> lstRecent = new List<SecurityPassViewModel>();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstRecent))
            {
                SecurityPassViewModel firstDetail = lstRecent.Where(x => x.passNumber == passNO).FirstOrDefault();
                return PartialView("~/Views/Feature/GatePass/ePass/_ePassShareDetails.cshtml", firstDetail);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The ePassShareDetails.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <param name="shareType">The shareType<see cref="string"/>.</param>
        /// <param name="emailId">The emailId<see cref="string"/>.</param>
        /// <param name="mobileNo">The mobileNo<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ePassShareDetails(string passNumber, string shareType, string emailId, string mobileNo)
        {
            string successMessage = string.Empty;
            try
            {
                List<SecurityPassViewModel> lstRecent = new List<SecurityPassViewModel>();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstRecent))
                {
                    SecurityPassViewModel firstDetail = lstRecent.Where(x => x.passNumber.ToLower().Equals(passNumber.ToLower())).FirstOrDefault();
                    if (firstDetail != null)
                    {
                        bool chkMobile = (!string.IsNullOrWhiteSpace(shareType) && shareType.Equals("sms")) ? true : false; // Convert.ToBoolean(formData[0].Split('=')[1]);
                        bool chkEmail = (!string.IsNullOrWhiteSpace(shareType) && shareType.Equals("email")) ? true : false; //Convert.ToBoolean(formData[1].Split('=')[1]);

                        Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                        if (string.IsNullOrWhiteSpace(firstDetail.email))
                        {
                            firstDetail.email = emailId;
                        }
                        if (string.IsNullOrWhiteSpace(firstDetail.mobile))
                        {
                            firstDetail.mobile = mobileNo;
                        }
                        if (!string.IsNullOrWhiteSpace(firstDetail.email) && chkEmail)
                        {
                            // send email
                            int chkEmailLimit = Convert.ToInt32(ePassConfig["Email Limit"]);
                            if (firstDetail.emailLimit < chkEmailLimit)
                            {
                                string from = ePassConfig != null ? ePassConfig["From"].ToString() : "noreply@dewa.gov.ae";
                                string subject = ePassConfig != null ? ePassConfig["Subject"].ToString() : string.Empty;
                                string body = ePassConfig != null ? ePassConfig["ePass email"].ToString() : string.Empty;
                                SendEmailToUsers(firstDetail.email, subject, body.ToString(), from, firstDetail);
                                firstDetail.emailLimit++;
                                Tuple<bool, string, string> isSaved = UpdatePassStatus(passDetail: firstDetail);
                                if (isSaved.Item1)
                                {
                                    successMessage = Translate.Text("epassemail.success");
                                }
                                //updateData(firstDetail);
                            }
                            else
                            {
                                successMessage = Translate.Text("epassemail.limit");
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(firstDetail.mobile) && chkMobile)
                        {
                            //Send SMS from here
                            int chkSmsLimit = Convert.ToInt32(ePassConfig["SMS Limit"]);

                            if (firstDetail.smsLimit < chkSmsLimit)
                            {
                                string locations = "N/A";
                                if (firstDetail.Location.Count > 0)
                                {
                                    locations = string.Join(",", firstDetail.Location.ToArray()).Trim();
                                }

                                string smsBody = ePassConfig != null ? ePassConfig["sms"].ToString() : string.Empty;
                                DateTime.TryParse(firstDetail.passIssueDate.ToString(), out DateTime fromdateTime);
                                DateTime.TryParse(firstDetail.passExpiryDate.ToString(), out DateTime todatetime);
                                smsBody = string.Format(smsBody, firstDetail.name, firstDetail.passNumber, fromdateTime.ToString("MMM d, yyyy"), todatetime.ToString("MMM d, yyyy"), firstDetail.fromTime, firstDetail.toTime, locations);
                                SendSmsToUsers(smsBody, firstDetail.mobile, "Security_Pass", CurrentPrincipal.Name.ToString());
                                successMessage = Translate.Text("epasssms.success");
                                firstDetail.smsLimit++;
                                //updateData(firstDetail);
                                Tuple<bool, string, string> isSaved = UpdatePassStatus(passDetail: firstDetail);
                                if (isSaved.Item1)
                                {
                                    successMessage = Translate.Text("epassemail.success");
                                }
                            }
                            else
                            {
                                successMessage = Translate.Text("epasssms.limit");
                            }
                        }
                        else
                        {
                            successMessage = Translate.Text("Please enter valid value");
                        }
                    }
                    else
                    {
                        successMessage = Translate.Text("Please enter valid value");
                    }
                }
            }
            catch (System.Exception ex)
            {
                successMessage = Translate.Text("Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            return Json(successMessage);
        }

        /// <summary>
        /// The GetRecentPasses.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult GetRecentPasses()
        {
            List<SecurityPassViewModel> lstpasses = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstpasses))
            {
                lstpasses = lstpasses.Take(5).ToList();
            }
            ViewBag.role = CurrentPrincipal.Role;
            return PartialView("~/Views/Feature/GatePass/ePass/_RecentPasses.cshtml", lstpasses);
        }

        /// <summary>
        /// The ePassDetails.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ePassDetails(string passNumber)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }

            if (!string.IsNullOrWhiteSpace(passNumber))
            {
                SecurityPassViewModel passDetails = null;
                List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstmodel))
                {
                    passDetails = lstmodel.Where(x => (!string.IsNullOrWhiteSpace(x.passNumber) && x.passNumber.ToLower().Equals(passNumber.ToLower()))
                    || (!string.IsNullOrWhiteSpace(x.mainpassNumber) && x.mainpassNumber.ToLower().Equals(passNumber.ToLower()))
                    || (!string.IsNullOrWhiteSpace(x.grouppassid) && x.grouppassid.ToLower().Equals(passNumber.ToLower()))
                    ).FirstOrDefault();
                }

                List<SecurityPassViewModel> lstpassDetails = new List<SecurityPassViewModel>();
                BasePassViewModel relPassdetail = new BasePassViewModel();
                if (passDetails != null)
                {
                    if (!passDetails.wppass)
                    {
                        BasePassDetailModel lstrelPassdetail = GetPassByPassNumber(passNumber);
                        if (lstrelPassdetail != null && lstrelPassdetail.MainPass != null)
                        {
                            lstpassDetails = PassFormating(passDetails, lstpassDetails, relPassdetail, lstrelPassdetail);

                            ViewBag.role = CurrentPrincipal.Role;
                            return PartialView("~/Views/Feature/GatePass/ePass/_PassDetails.cshtml", lstpassDetails);
                        }
                    }
                    else
                    {
                        passDetails = new SecurityPassViewModel
                        {
                            wppass = true
                        };
                        if (passDetails.wppass)
                        {
                            GroupPassPemitResponse lstpassess = GetWorkpermitPasses(passNumber);
                            if (!CacheProvider.TryGet(CacheKeys.WORK_PERMIT_COUNTRYLIST, out IEnumerable<SelectListItem> countrylist))
                            {
                                countrylist = GetWPCountryList();
                            }
                            if (lstpassess != null && lstpassess.Groupworkpermitpassbothlist != null)
                            {
                                lstpassDetails = lstpassess.Groupworkpermitpassbothlist.Where(y => !string.IsNullOrWhiteSpace(y.Passexpirydate) && !string.IsNullOrWhiteSpace(y.Passissuedate) && !y.Passexpirydate.Equals("0000-00-00") && !y.Passissuedate.Equals("0000-00-00")).Select(x => new SecurityPassViewModel
                                {
                                    eFolderId = string.Empty,
                                    name = x.Fullname,
                                    passNumber = x.Permitpass,
                                    mainpassNumber = x.Permitpass,
                                    grouppassid = x.Grouppassid,
                                    wppass = true,
                                    grouppass = !string.IsNullOrWhiteSpace(x.Grouppassid) && x.Grouppassid.ToLower().StartsWith("gp") ? true : false,
                                    passType = Translate.Text("Work Permit"),
                                    profession = x.Profession,
                                    CreatedDate = !string.IsNullOrWhiteSpace(x.createddate) && !x.createddate.Equals("0000-00-00") ? DateTime.Parse(x.createddate) : (DateTime?)null,
                                    ChangedDate = !string.IsNullOrWhiteSpace(x.createddate) && !x.createddate.Equals("0000-00-00") ? DateTime.Parse(x.createddate) : (DateTime?)null,
                                    passExpiryDate = !string.IsNullOrWhiteSpace(x.Passexpirydate) && !x.Passexpirydate.Equals("0000-00-00") ? DateTime.Parse(x.Passexpirydate) : (DateTime?)null,
                                    passIssueDate = !string.IsNullOrWhiteSpace(x.Passissuedate) && !x.Passissuedate.Equals("0000-00-00") ? DateTime.Parse(x.Passissuedate) : (DateTime?)null,
                                    strpassExpiryDate = !string.IsNullOrWhiteSpace(x.Passexpirydate) && !x.Passexpirydate.Equals("0000-00-00") ? FormatEpassstrDate(DateTime.Parse(x.Passexpirydate).ToString("MMMM dd, yyyy")) : string.Empty,
                                    status = assignWPStatus(lstpassess.Grouppasslocationreturnlist, x.Grouppassid, x.Passexpirydate),
                                    strstatus = Translate.Text("epassstatus." + assignWPStatus(lstpassess.Grouppasslocationreturnlist, x.Grouppassid, x.Passexpirydate).ToString().ToLower()),
                                    strclass = assignWPStatus(lstpassess.Grouppasslocationreturnlist, x.Grouppassid, x.Passexpirydate).ToString(),
                                    nationality = x.Countrykey,
                                    emiratesId = x.Emiratesid,
                                    emiratesExpiryDate = !string.IsNullOrWhiteSpace(x.Emiratesidenddate) && !x.Emiratesidenddate.Equals("0000-00-00") ? DateTime.Parse(x.Emiratesidenddate) : (DateTime?)null,
                                    visaNumber = x.Visanumber,
                                    visaExpiryDate = !string.IsNullOrWhiteSpace(x.Visaendate) && !x.Visaendate.Equals("0000-00-00") ? DateTime.Parse(x.Visaendate) : (DateTime?)null,
                                    emailLimit = 0,
                                    smsLimit = 0,
                                    downloadLimit = 0,
                                    passportNumber = x.Passportnumber,
                                    passportExpiryDate = !string.IsNullOrWhiteSpace(x.Passportenddate) && !x.Passportenddate.Equals("0000-00-00") ? DateTime.Parse(x.Passportenddate) : (DateTime?)null,
                                    fromTime = x.Fromtime,
                                    toTime = x.Totime,
                                    mobile = x.Mobile.AddMobileNumberZeroPrefix(),
                                    email = x.Emailid,
                                    VisitorEmail = x.Emailid,
                                    enablerenewbutton = !string.IsNullOrWhiteSpace(x.Passexpirydate) && string.IsNullOrWhiteSpace(x.Renewalreference) ? DateTime.Parse(lstpassess.Groupworkpermitpassbothlist[0].Passexpirydate).AddDays(-10) <= DateTime.Now : false,
                                    Location = lstpassess.Grouppasslocationreturnlist != null && lstpassess.Grouppasslocationreturnlist.Count() > 0 && lstpassess.Grouppasslocationreturnlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                           lstpassess.Grouppasslocationreturnlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Select(z => z.Location).ToList() : new List<string>(),
                                    WpLocation = lstpassess.Grouppasslocationreturnlist,
                                    SeletecedLocation = lstpassess.Grouppasslocationreturnlist != null && lstpassess.Grouppasslocationreturnlist.Count() > 0 && lstpassess.Grouppasslocationreturnlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                           lstpassess.Grouppasslocationreturnlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Select(z => z.Locationcode).ToList() : new List<string>(),
                                    RejectRemarks = x.Rejectremarks,
                                    Purpose = x.Purposeofvisit,
                                    Remarks = x.Remarks,
                                    Subcontractor = !string.IsNullOrWhiteSpace(x.Permitsubreference) && lstpassess.subcontractordetails != null && lstpassess.subcontractordetails.Count() > 0 && lstpassess.subcontractordetails.Where(y => y.Permitsubreference.Equals(x.Permitsubreference)).Any() ?
                                                 lstpassess.subcontractordetails.Where(y => y.Permitsubreference.Equals(x.Permitsubreference)).FirstOrDefault().Subcontractorname : string.Empty,
                                    passAttachements = new List<SecurityPassAttachement>(),
                                    companyName = x.Companyname,
                                    projectName = x.Projectname,
                                    projectStartDate = null,
                                    projectEndDate = null,
                                    projectId = x.Ponumber,
                                    projectStatus = string.Empty,
                                    departmentName = string.Empty,
                                    IsBlocked = false,
                                    DEWAID = string.Empty,
                                    DEWAdesignation = string.Empty,
                                    VehicleRegNumber = x.Platenumber,
                                    strpassVehicleRegDate = string.Empty,
                                    pendingwith = string.Empty,
                                    wpprojectcoordinatorname = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                                    lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Fullname : string.Empty,
                                    wpprojectcoordinatoremail = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                                    lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Emailid : string.Empty,
                                    wpprojectcoordinatormobile = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                                    lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Mobile : string.Empty,
                                }).ToList();
                                if (lstpassDetails != null && lstpassDetails.Count > 0 && lstpassess.attachmentdetails != null && lstpassess.attachmentdetails.Count > 0)
                                {
                                    if (lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("4")).Any())
                                    {
                                        lstpassDetails.FirstOrDefault().profilepic = lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("4")).FirstOrDefault().Filedata;
                                    }
                                    if (lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("6")).Any())
                                    {
                                        lstpassDetails.FirstOrDefault().isEidattachment = !string.IsNullOrWhiteSpace(lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("6")).FirstOrDefault().Filedata);
                                    }
                                    if (lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("8")).Any())
                                    {
                                        lstpassDetails.FirstOrDefault().isPassportattachment = !string.IsNullOrWhiteSpace(lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("8")).FirstOrDefault().Filedata);
                                    }
                                    if (lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("10")).Any())
                                    {
                                        lstpassDetails.FirstOrDefault().isVisaattachment = !string.IsNullOrWhiteSpace(lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("10")).FirstOrDefault().Filedata);
                                    }
                                    if (lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("12")).Any())
                                    {
                                        lstpassDetails.FirstOrDefault().isTrafficfilecodeattachment = !string.IsNullOrWhiteSpace(lstpassess.attachmentdetails.Where(x => x.Folderid.Equals("12")).FirstOrDefault().Filedata);
                                    }
                                }
                                CacheProvider.Store(CacheKeys.EPASS_DETAILS, new CacheItem<List<SecurityPassViewModel>>(lstpassDetails, TimeSpan.FromMinutes(20)));

                                ViewBag.role = CurrentPrincipal.Role;
                                ViewBag.LocationList = GetLocationList();
                                if (lstpassDetails != null && lstpassDetails.Count > 0 && !string.IsNullOrWhiteSpace(passNumber) && passNumber.ToLower().StartsWith("gp"))
                                {
                                    return PartialView("~/Views/Feature/GatePass/ePass/_GroupPassDetails.cshtml", lstpassDetails);
                                }
                                return PartialView("~/Views/Feature/GatePass/ePass/_PassDetails.cshtml", lstpassDetails);
                            }
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
                        }
                    }
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        private List<SecurityPassViewModel> PassFormating(SecurityPassViewModel passDetails, List<SecurityPassViewModel> lstpassDetails, BasePassViewModel relPassdetail, BasePassDetailModel lstrelPassdetail)
        {
            BaseMainPassDetailModel mainpassitem = lstrelPassdetail.MainPass.FirstOrDefault();

            lstpassDetails.Add(new SecurityPassViewModel
            {
                name = mainpassitem.ePassVisitorName,
                mainpassNumber = mainpassitem.ePassPassNo,
                passNumber = mainpassitem.ePassPassNo,
                passType = mainpassitem.ePassPassType,
                passTypeText = mainpassitem.ePassPassType,
                profession = mainpassitem.ePassProfession,
                Designation = mainpassitem.ePassDesignation,
                passExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassExpiryDate) :
                     (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassDate(mainpassitem.ePassVisitingDate) : default(DateTime?)),
                passIssueDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassIssueDate)) ? FormatEpassDate(mainpassitem.ePassPassIssueDate) :
    (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassDate(mainpassitem.ePassVisitingDate) : default(DateTime?)),
                CreatedDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassCreatedOn)) ? FormatEpassDate(mainpassitem.ePassCreatedOn) : default(DateTime?),
                strpassExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassstrDate(mainpassitem.ePassPassExpiryDate) :
          (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassstrDate(mainpassitem.ePassVisitingDate) : string.Empty),
                status = assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)),
                strstatus = Translate.Text("epassstatus." + assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)).ToString().ToLower()),
                strclass = assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)).ToString(),
                nationality = mainpassitem.ePassNationality,
                emiratesId = mainpassitem.ePassEmiratesID,
                emiratesExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassEmiratesiDExpiry)) ? FormatEpassDate(mainpassitem.ePassEmiratesiDExpiry) : default(DateTime?),
                visaNumber = mainpassitem.ePassVisaNumber,
                visaExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassVisaExpiryDate)) ? FormatEpassDate(mainpassitem.ePassVisaExpiryDate) : default(DateTime?),
                emailLimit = (mainpassitem.ePassEmailLimit == null) ? 0 : Convert.ToInt16(mainpassitem.ePassEmailLimit),
                smsLimit = (mainpassitem.ePassSMSLimit == null) ? 0 : Convert.ToInt16(mainpassitem.ePassSMSLimit),
                downloadLimit = (mainpassitem.ePassDownloadLimit == null) ? 0 : Convert.ToInt16(mainpassitem.ePassDownloadLimit),
                passportNumber = mainpassitem.ePassPassportNumber,
                passportExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassportExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassportExpiryDate) : default(DateTime?),
                fromTime = mainpassitem.ePassFromDateTime,
                toTime = mainpassitem.ePassToDateTime,
                mobile = mainpassitem.ePassMobileNumber,
                email = mainpassitem.ePassEmailAddress,
                VisitorEmail = mainpassitem.ePassVisitorEmailID,
                SeniorManagerEmail = mainpassitem.ePassVisitorEmailID,
                Location = lstselectedlocations(mainpassitem.ePassLocation),
                Subcontractor = mainpassitem.ePassSubContractorID,
                companyName = mainpassitem.ePassCompanyName,
                projectName = mainpassitem.ePassProjectName,
                projectStartDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassProjectStartName)) ? FormatEpassDate(mainpassitem.ePassProjectStartName) : default(DateTime?),
                projectEndDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassProjectEndDate)) ? FormatEpassDate(mainpassitem.ePassProjectEndDate) : default(DateTime?),
                projectId = mainpassitem.ePassProjectID,
                projectStatus = mainpassitem.ePassProjectStatus,
                departmentName = mainpassitem.ePassDepartmentName,
                IsBlocked = (mainpassitem.ePassIsBlocked == "Yes" ? true : false),
                DEWAID = mainpassitem.ePassDEWAID,
                DEWAdesignation = mainpassitem.ePassDesignation,
                Subpass = false,
                wppass = false,
                VehicleRegNumber = mainpassitem.ePassVehicleNo,
                VehRegistrationDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassVehicleRegDate)) ? FormatEpassDate(mainpassitem.ePassVehicleRegDate) : default(DateTime?),
                strpassVehicleRegDate = !string.IsNullOrWhiteSpace(mainpassitem.ePassVehicleRegDate) ? FormatEpassDate(mainpassitem.ePassVehicleRegDate).ToString("MMMM dd, yyyy") : string.Empty,
                pendingwith = (mainpassitem.ePassSecurityApprovers != null && mainpassitem.ePassPassStatus != null && !string.IsNullOrWhiteSpace(mainpassitem.ePassSecurityApprovers.ToString()) && !string.IsNullOrWhiteSpace(mainpassitem.ePassPassStatus.ToString())) ? (mainpassitem.ePassPassStatus.ToString().ToLower().Equals("Dept Coordinator".ToLower()) ? mainpassitem.ePassSecurityApprovers.ToString() : (mainpassitem.ePassPassStatus.ToString().ToLower().Equals("Security Team".ToLower()) ? Translate.Text("Epass.SecurityAdmin") : string.Empty)) : string.Empty,
                passAttachements = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo)).Any() ?
                lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo)).Select(y => new SecurityPassAttachement
                {
                    docType = y.FileType,
                    fileCategory = y.FileCategory,
                    fileContent = y.SupportingFile,
                    fileName = y.FileName,
                    fileContentType = y.FileContentType
                }).ToList() : null,
                isEidattachment = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("EmiratesID")).Any(),
                isPassportattachment = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("Passport")).Any(),
                isVisaattachment = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("Visa")).Any(),
                isTrafficfilecodeattachment = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("Vehicle")).Any(),
                isDrivingLicenseattachment = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("DrivingLicense")).Any(),
                profilepic = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("Photo")).Any() ?
                (lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("Photo")).FirstOrDefault() != null) ?
                 Convert.FromBase64String(lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("Photo")).FirstOrDefault().SupportingFile) != null ?
                 Convert.ToBase64String(Convert.FromBase64String(lstrelPassdetail.Attachments.Where(y => y.ReqID.Equals(mainpassitem.ePassPassNo) && y.FileCategory.Equals("Photo")).FirstOrDefault().SupportingFile))
                 : string.Empty
                 : string.Empty
                : string.Empty,
            });

            if (!string.IsNullOrEmpty(relPassdetail.ePassLinkExpiry)) { passDetails.linkExpiryDate = DateTime.Parse(relPassdetail.ePassLinkExpiry); }

            if (lstrelPassdetail.SubPasses != null && lstrelPassdetail.SubPasses.Count > 0)
            {
                lstrelPassdetail.SubPasses.ToList().ForEach(sp =>
                {
                    lstpassDetails.Add(new SecurityPassViewModel()
                    {
                        name = lstpassDetails.FirstOrDefault().name,
                        passNumber = sp.subPassRequestID,
                        mainpassNumber = lstpassDetails.FirstOrDefault().passNumber,
                        Subpass = true,
                        passType = sp.subPassNewPassType,
                        passTypeText = Translate.Text("Epass." + sp.subPassNewPassType),
                        passExpiryDate = DateTime.Parse(sp.subPassValidTo),
                        strpassExpiryDate = FormatEpassstrDate(DateTime.Parse(sp.subPassValidTo).ToString("MMMM dd, yyyy")),
                        passIssueDate = DateTime.Parse(sp.subPassValidFrom),
                        CreatedDate = DateTime.Parse(sp.subPassCreatedOn),
                        status = assignStatus(sp.subPassStatus, sp.subPassValidTo, lstpassDetails.FirstOrDefault().IsBlocked),
                        strstatus = Translate.Text("epassstatus." + assignStatus(sp.subPassStatus, sp.subPassValidTo, lstpassDetails.FirstOrDefault().IsBlocked).ToString().ToLower()),
                        strclass = assignStatus(sp.subPassStatus, lstpassDetails.FirstOrDefault().passExpiryDate, lstpassDetails.FirstOrDefault().IsBlocked).ToString(),
                        SeniorManagerEmail = lstpassDetails.FirstOrDefault().SeniorManagerEmail,
                        fromTime = DateTime.Parse(sp.subPassValidFrom).TimeOfDay.ToString(),
                        toTime = DateTime.Parse(sp.subPassValidTo).TimeOfDay.ToString(),
                        emiratesId = lstpassDetails.FirstOrDefault().emiratesId,
                        visaNumber = lstpassDetails.FirstOrDefault().visaNumber,
                        passportNumber = lstpassDetails.FirstOrDefault().passportNumber,
                        pendingwith = sp.subPassStatus.ToLower().Equals("initiated") ? sp.subPassDepartmentApprover : (sp.subPassStatus.ToLower().Equals("dept approved") ? sp.subPassSecurityApprovers : (sp.subPassStatus.ToLower().Equals("security approved") ? "Completed" : "Unknown"))
                    });
                });
            }

            // GetSubPasses(lstpassDetails, lstrelPassdetail);

            lstpassDetails = lstpassDetails.OrderByDescending(x => x.CreatedDate).ToList();

            CacheProvider.Store(CacheKeys.EPASS_DETAILS, new CacheItem<List<SecurityPassViewModel>>(lstpassDetails, TimeSpan.FromMinutes(20)));
            return lstpassDetails;
        }

        /// <summary>
        /// The CancelEpassentry.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CancelEpassentry(string id)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            List<SecurityPassViewModel> lstpasses = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstpasses))
            {
                SecurityPassViewModel itemTocancel = lstpasses.SingleOrDefault(r => r.passNumber.ToString().Equals(id));
                if (itemTocancel != null)
                {
                    //bool cancelentry = CancelPass(itemTocancel.eFolderId);
                    //if (cancelentry)
                    //{
                    //    return Json(new { status = true });
                    //}
                    string message = string.Empty;
                    SecurityApproveRejectPassViewModel model = new SecurityApproveRejectPassViewModel();
                    if (itemTocancel != null)
                    {
                        if (itemTocancel.passTypeText.Equals(EpassHelper.GetDisplayName(PassType.LongTerm)) || itemTocancel.passTypeText.Equals(EpassHelper.GetDisplayName(PassType.ShortTerm)))
                        {
                            model.PassType = "Main";
                        }
                        else
                        {
                            model.PassType = "Sub";
                        }

                        model.PassNumber = itemTocancel.passNumber;
                        model.PassStatus = "Cancelled";
                    }

                    Tuple<bool, string, string> isSaved = UpdatePassStatus(model);
                    if (isSaved.Item1)
                    {
                        return Json(new { status = true });
                    }
                    else
                    {
                        return Json(new { status = false });
                    }
                }
                return Json(new { status = false });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The DownloadPDF.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult DownloadPDF(string passNumber)
        {
            if (!string.IsNullOrWhiteSpace(passNumber))
            {
                List<SecurityPassViewModel> model = new List<SecurityPassViewModel>();
                SecurityPassViewModel passDetails = null;
                List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstmodel))
                {
                    passDetails = lstmodel.Where(x => x.passNumber.ToLower().Equals(passNumber.ToLower()) || x.mainpassNumber.ToLower().Equals(passNumber.ToLower())).FirstOrDefault();
                }

                bool valid = false;
                if (passDetails != null)
                {
                    if (CacheProvider.TryGet(CacheKeys.EPASS_DETAILS, out model))
                    {
                        if (model != null && model.Count > 0 && model.Where(x => x.passNumber.Equals(passNumber)).Any())
                        {
                            valid = true;
                        }
                    }
                    if (!valid)
                    {
                        BasePassViewModel relPassdetail = new BasePassViewModel();
                        BasePassDetailModel lstrelPassdetail = GetPassByPassNumber(passNumber);
                        if (lstrelPassdetail != null && lstrelPassdetail.MainPass != null)
                        {
                            model = PassFormating(passDetails, model, relPassdetail, lstrelPassdetail);
                        }
                    }

                    if (model != null && model.Count > 0 && model.Where(x => x.passNumber.Equals(passNumber)).Any())
                    {
                        SecurityPassViewModel itemToDownload = model.Where(x => x.passNumber.Equals(passNumber)).FirstOrDefault();
                        Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                        int chkDownloadLimit = Convert.ToInt32(ePassConfig["Download Limit"]);
                        if (itemToDownload != null && itemToDownload.downloadLimit < chkDownloadLimit)
                        {
                            FileResult fileResult = new FileContentResult(ExportPdf(itemToDownload), "application/pdf")
                            {
                                FileDownloadName = itemToDownload.passNumber + "-" + DateTime.Now.Ticks + ".pdf"
                            };
                            itemToDownload.downloadLimit++;
                            Tuple<bool, string, string> isSaved = UpdatePassStatus(passDetail: itemToDownload);
                            if (isSaved.Item1)
                            {
                                return fileResult;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The DownloadAttachmentKofax.
        /// </summary>
        /// <param name="passnumber">The passnumber<see cref="string"/>.</param>
        /// <param name="cat">The cat<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult DownloadAttachmentKofax(string passnumber, string cat)
        {
            if (!string.IsNullOrEmpty(passnumber))
            {
                List<SecurityPassViewModel> model = new List<SecurityPassViewModel>();
                SecurityPassViewModel passDetails = null;
                List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstmodel))
                {
                    passDetails = lstmodel.Where(x => x.passNumber.ToLower().Equals(passnumber.ToLower()) || x.mainpassNumber.ToLower().Equals(passnumber.ToLower())).FirstOrDefault();
                }
                bool valid = false;
                if (passDetails != null)
                {
                    if (CacheProvider.TryGet(CacheKeys.EPASS_DETAILS, out model))
                    {
                        if (model != null && model.Count > 0 && model.Where(x => x.passNumber.Equals(passnumber)).Any())
                        {
                            valid = true;
                        }
                    }
                    if (!valid)
                    {
                        BasePassViewModel relPassdetail = new BasePassViewModel();
                        BasePassDetailModel lstrelPassdetail = GetPassByPassNumber(passnumber);
                        if (lstrelPassdetail != null && lstrelPassdetail.MainPass != null)
                        {
                            model = PassFormating(passDetails, model, relPassdetail, lstrelPassdetail);
                        }
                    }

                    SecurityPassAttachement passAttachement = null;
                    if (model != null && model.Count > 0 && model.Where(x => x.passNumber.Equals(passnumber)).Any())
                    {
                        SecurityPassViewModel passmodel = model.Where(x => x.passNumber.Equals(passnumber)).FirstOrDefault();
                        if (passmodel != null && passmodel.passAttachements != null)
                        {
                            passAttachement = passmodel.passAttachements.Where(x => x.fileCategory == cat).FirstOrDefault();
                            byte[] bytes = Convert.FromBase64String(passAttachement.fileContent);
                            string type = MimeExtensions.GetExtension(passAttachement.fileContentType);
                            return File(bytes, passAttachement.fileContentType);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The ePassProfileInfo.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ePassProfileInfo()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (CacheProvider.TryGet(CacheKeys.EPASS_PROFILEUPDATEERROR_MESSAGE, out string errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.EPASS_PROFILEUPDATEERROR_MESSAGE);
            }
            else if (CacheProvider.TryGet(CacheKeys.EPASS_PROFILEUPDATE_MESSAGE, out errorMessage))
            {
                ViewBag.success = true;
                CacheProvider.Remove(CacheKeys.EPASS_PROFILEUPDATE_MESSAGE);
            }
            ServiceResponse<userDetailsOutput> response = GatePassServiceClient.UserDetails(new userDataInput { userid = CurrentPrincipal.Username, supplierid = CurrentPrincipal.BusinessPartner, key = CurrentPrincipal.SessionToken, sessionid = CurrentPrincipal.SessionToken, mode = "D" }, RequestLanguage, Request.Segment());
            if (response != null && response.Succeeded && response.Payload != null && response.Payload.userdetails != null)
            {
                userDetails userdetails = response.Payload.userdetails;
                UserRegistration userreg = new UserRegistration
                {
                    CompanyName = userdetails.suppliername,
                    Fullname = userdetails.contactname,
                    Emailid = userdetails.contactemailid,
                    MobilePhone = userdetails.contactmobilenumber.RemoveMobileNumberZeroPrefix(),
                    VendorId = userdetails.supplierid,
                    Username = userdetails.userid
                };
                return PartialView("~/Views/Feature/GatePass/ePass/ProfileInfoForm.cshtml", userreg);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_DASHBOARD);
        }

        /// <summary>
        /// The ePassProfileInfo.
        /// </summary>
        /// <param name="model">The model<see cref="UserRegistration"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ePassProfileInfo(UserRegistration model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            ServiceResponse<userDetailsOutput> response = GatePassServiceClient.UserDetails(new userDataInput { userid = CurrentPrincipal.Username, emailid = CurrentPrincipal.EmailAddress, supplierid = CurrentPrincipal.BusinessPartner, key = CurrentPrincipal.SessionToken, sessionid = CurrentPrincipal.SessionToken, mode = "U", mobilenumber = model.MobilePhone.AddMobileNumberZeroPrefix(), contactname = model.Fullname }, RequestLanguage, Request.Segment());
            if (response != null && response.Succeeded && response.Payload != null && response.Payload.userdetails != null)
            {
                CacheProvider.Store(CacheKeys.EPASS_PROFILEUPDATE_MESSAGE, new CacheItem<string>(response.Message));
            }
            else
            {
                CacheProvider.Store(CacheKeys.EPASS_PROFILEUPDATEERROR_MESSAGE, new CacheItem<string>(response.Message));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_MYPROFILE);
        }

        /// <summary>
        /// The ChangePassword.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            if (CacheProvider.TryGet(CacheKeys.EPASS_CHANGEPASSWORDERROR_MESSAGE, out string errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                ViewBag.success = false;
                CacheProvider.Remove(CacheKeys.EPASS_CHANGEPASSWORDERROR_MESSAGE);
            }
            else if (CacheProvider.TryGet(CacheKeys.EPASS_CHANGEPASSWORD_MESSAGE, out errorMessage))
            {
                ViewBag.success = true;
                CacheProvider.Remove(CacheKeys.EPASS_CHANGEPASSWORD_MESSAGE);
            }
            return PartialView("~/Views/Feature/GatePass/ePass/ChangePassword.cshtml");
        }

        /// <summary>
        /// The ChangePassword.
        /// </summary>
        /// <param name="model">The model<see cref="SetNewPassword"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(SetNewPassword model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            ServiceResponse<userDetailsOutput> response = GatePassServiceClient.ChangePassword(new credentialDataInput { key = CurrentPrincipal.SessionToken, sessionid = CurrentPrincipal.SessionToken, userid = CurrentPrincipal.Username, oldpassword = model.OldPassword, password = model.ConfirmPassword, mode = "S" }, RequestLanguage, Request.Segment());
            if (response != null && response.Succeeded && response.Payload != null)
            {
                CacheProvider.Store(CacheKeys.EPASS_CHANGEPASSWORD_MESSAGE, new CacheItem<string>(response.Message));
            }
            else
            {
                CacheProvider.Store(CacheKeys.EPASS_CHANGEPASSWORDERROR_MESSAGE, new CacheItem<string>(response.Message));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_MYPROFILE);
        }

        /// <summary>
        /// The AllpassesAjax.
        /// </summary>
        /// <param name="pagesize">The pagesize<see cref="int"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <param name="statustxt">The statustxt<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="namesort">The namesort<see cref="string"/>.</param>
        /// <param name="passfilter">The passfilter<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AllpassesAjax(int pagesize = 5, string keyword = "", string statustxt = "all", int page = 1, string namesort = "", string passfilter = "")
        {
            keyword = keyword.Trim();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
            List<SecurityPassViewModel> lstpassDetails = new List<SecurityPassViewModel>();
            bool cachevalue = false;
            if (CacheProvider.TryGet(CacheKeys.EPASS_TRACKPASSKEYWORD, out string cachekeyword))
            {
                if (cachekeyword.Equals(keyword))
                {
                    cachevalue = true;
                }
            }

            if (cachevalue && CacheProvider.TryGet(CacheKeys.EPASS_TRACKPASS, out lstpassDetails))
            {
                lstmodel = lstpassDetails;
            }
            else
            {
                BasePassDetailModel lstrelPassdetail = GetPassByPassNumber(keyword: keyword);
                if (lstrelPassdetail != null && lstrelPassdetail.MainPass != null)
                {
                    //BaseMainPassDetailModel mainpassitem = lstrelPassdetail.MainPass.FirstOrDefault();

                    lstrelPassdetail.MainPass.ToList().ForEach(mainpassitem =>
                    {
                        lstpassDetails.Add(new SecurityPassViewModel
                        {
                            name = mainpassitem.ePassVisitorName,
                            mainpassNumber = mainpassitem.ePassPassNo,
                            passNumber = mainpassitem.ePassPassNo,
                            passType = mainpassitem.ePassPassType,
                            passTypeText = mainpassitem.ePassPassType,
                            profession = mainpassitem.ePassProfession,
                            Designation = mainpassitem.ePassDesignation,
                            //passExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassExpiryDate) : default(DateTime?),
                            //passIssueDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassIssueDate)) ? FormatEpassDate(mainpassitem.ePassPassIssueDate) : default(DateTime?),
                            //CreatedDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassCreatedOn)) ? FormatEpassDate(mainpassitem.ePassCreatedOn) : default(DateTime?),
                            //strpassExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassExpiryDate).ToString("MMMM dd, yyyy") : string.Empty,

                            passExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassExpiryDate) :
                                         (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassDate(mainpassitem.ePassVisitingDate) : default(DateTime?)),
                            passIssueDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassIssueDate)) ? FormatEpassDate(mainpassitem.ePassPassIssueDate) :
                        (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassDate(mainpassitem.ePassVisitingDate) : default(DateTime?)),
                            CreatedDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassCreatedOn)) ? FormatEpassDate(mainpassitem.ePassCreatedOn) : default(DateTime?),
                            strpassExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassstrDate(mainpassitem.ePassPassExpiryDate) :
                              (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassstrDate(mainpassitem.ePassVisitingDate) : string.Empty),

                            status = assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)),
                            strstatus = Translate.Text("epassstatus." + assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)).ToString().ToLower()),
                            strclass = assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)).ToString(),
                            nationality = mainpassitem.ePassNationality,
                            emiratesId = mainpassitem.ePassEmiratesID,
                            emiratesExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassEmiratesiDExpiry)) ? FormatEpassDate(mainpassitem.ePassEmiratesiDExpiry) : default(DateTime?),
                            visaNumber = mainpassitem.ePassVisaNumber,
                            visaExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassVisaExpiryDate)) ? FormatEpassDate(mainpassitem.ePassVisaExpiryDate) : default(DateTime?),
                            emailLimit = (!string.IsNullOrWhiteSpace(mainpassitem.ePassEmailLimit)) ? Convert.ToInt16(mainpassitem.ePassEmailLimit) : 0,
                            smsLimit = (!string.IsNullOrWhiteSpace(mainpassitem.ePassSMSLimit)) ? Convert.ToInt16(mainpassitem.ePassSMSLimit) : 0,
                            downloadLimit = (!string.IsNullOrWhiteSpace(mainpassitem.ePassDownloadLimit)) ? Convert.ToInt16(mainpassitem.ePassDownloadLimit) : 0,
                            passportNumber = mainpassitem.ePassPassportNumber,
                            passportExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassportExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassportExpiryDate) : default(DateTime?),
                            fromTime = mainpassitem.ePassFromDateTime,
                            toTime = mainpassitem.ePassToDateTime,
                            mobile = mainpassitem.ePassMobileNumber,
                            email = mainpassitem.ePassEmailAddress,
                            VisitorEmail = mainpassitem.ePassVisitorEmailID,
                            SeniorManagerEmail = mainpassitem.ePassVisitorEmailID,
                            Location = lstselectedlocations(mainpassitem.ePassLocation),
                            Subcontractor = mainpassitem.ePassSubContractorID,
                            companyName = mainpassitem.ePassCompanyName,
                            projectName = mainpassitem.ePassProjectName,
                            projectStartDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassProjectStartName)) ? FormatEpassDate(mainpassitem.ePassProjectStartName) : default(DateTime?),
                            projectEndDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassProjectEndDate)) ? FormatEpassDate(mainpassitem.ePassProjectEndDate) : default(DateTime?),
                            projectId = mainpassitem.ePassProjectID,
                            projectStatus = mainpassitem.ePassProjectStatus,
                            departmentName = mainpassitem.ePassDepartmentName,
                            IsBlocked = (mainpassitem.ePassIsBlocked == "Yes" ? true : false),
                            DEWAID = mainpassitem.ePassDEWAID,
                            DEWAdesignation = mainpassitem.ePassDesignation,
                            Subpass = false,
                            wppass = false,
                            VehicleRegNumber = mainpassitem.ePassVehicleNo,
                            VehRegistrationDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassVehicleRegDate)) ? FormatEpassDate(mainpassitem.ePassVehicleRegDate) : default(DateTime?),
                            strpassVehicleRegDate = !string.IsNullOrWhiteSpace(mainpassitem.ePassVehicleRegDate) ? FormatEpassDate(mainpassitem.ePassVehicleRegDate).ToString("MMMM dd, yyyy") : string.Empty,
                            pendingwith = (mainpassitem.ePassSecurityApprovers != null && mainpassitem.ePassPassStatus != null && !string.IsNullOrWhiteSpace(mainpassitem.ePassSecurityApprovers.ToString()) && !string.IsNullOrWhiteSpace(mainpassitem.ePassPassStatus.ToString())) ? (mainpassitem.ePassPassStatus.ToString().ToLower().Equals("Dept Coordinator".ToLower()) ? mainpassitem.ePassSecurityApprovers.ToString() : (mainpassitem.ePassPassStatus.ToString().ToLower().Equals("Security Team".ToLower()) ? Translate.Text("Epass.SecurityAdmin") : string.Empty)) : string.Empty,
                            passAttachements = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower())).Any() ?
    lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower())).Select(y => new SecurityPassAttachement
    {
        docType = y.FileType,
        fileCategory = y.FileCategory,
        fileContent = y.SupportingFile,
        fileName = y.FileName,
        fileContentType = y.FileContentType
    }).ToList() : null,

                            profilepic = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).Any() ?
    (lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).FirstOrDefault() != null) ?
    Convert.FromBase64String(lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).FirstOrDefault().SupportingFile) != null ?
    Convert.ToBase64String(Convert.FromBase64String(lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).FirstOrDefault().SupportingFile))
    : string.Empty
    : string.Empty
    : string.Empty,
                        });
                    });

                    if (lstrelPassdetail.SubPasses != null && lstrelPassdetail.SubPasses.Count > 0)
                    {
                        lstrelPassdetail.SubPasses.ToList().ForEach(sp =>
                        {
                            lstpassDetails.Add(new SecurityPassViewModel()
                            {
                                name = lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().name,
                                passNumber = sp.subPassRequestID,
                                mainpassNumber = lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().passNumber,
                                Subpass = true,
                                passType = sp.subPassNewPassType,
                                passTypeText = Translate.Text("Epass." + sp.subPassNewPassType),
                                passExpiryDate = DateTime.Parse(sp.subPassValidTo),
                                strpassExpiryDate = FormatEpassstrDate(DateTime.Parse(sp.subPassValidTo).ToString("MMMM dd, yyyy")),
                                passIssueDate = DateTime.Parse(sp.subPassValidFrom),
                                CreatedDate = DateTime.Parse(sp.subPassCreatedOn),
                                status = assignStatus(sp.subPassStatus, sp.subPassValidTo, lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().IsBlocked),
                                strstatus = Translate.Text("epassstatus." + assignStatus(sp.subPassStatus, sp.subPassValidTo, lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().IsBlocked).ToString().ToLower()),
                                strclass = assignStatus(sp.subPassStatus, lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().passExpiryDate, lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().IsBlocked).ToString(),
                                SeniorManagerEmail = lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().SeniorManagerEmail,
                                fromTime = DateTime.Parse(sp.subPassValidFrom).TimeOfDay.ToString(),
                                toTime = DateTime.Parse(sp.subPassValidTo).TimeOfDay.ToString(),
                                emiratesId = lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().emiratesId,
                                visaNumber = lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().visaNumber,
                                passportNumber = lstpassDetails.Where(x => x.passNumber.Equals(sp.subPassMainPassNo)).FirstOrDefault().passportNumber,
                                pendingwith = sp.subPassStatus.ToLower().Equals("initiated") ? sp.subPassDepartmentApprover : (sp.subPassStatus.ToLower().Equals("dept approved") ? sp.subPassSecurityApprovers : (sp.subPassStatus.ToLower().Equals("security approved") ? "Completed" : "Unknown"))
                            });
                        });
                    }

                    CacheProvider.Store(CacheKeys.EPASS_TRACKPASS, new CacheItem<List<SecurityPassViewModel>>(lstpassDetails, TimeSpan.FromMinutes(40)));
                    CacheProvider.Store(CacheKeys.EPASS_TRACKPASSKEYWORD, new CacheItem<string>(keyword, TimeSpan.FromMinutes(40)));
                    lstmodel = lstpassDetails;
                }
            }

            SecurityPassFilterViewModel SecurityPassFilterViewModel = new SecurityPassFilterViewModel
            {
                page = page
            };
            pagesize = pagesize > 30 ? 30 : pagesize;
            SecurityPassFilterViewModel.strdataindex = "0";
            if (lstmodel != null && lstmodel.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(passfilter))
                {
                    if (passfilter.ToLower().Equals("main"))
                    {
                        lstmodel = lstmodel.Where(x => !x.Subpass).ToList();
                        SecurityPassFilterViewModel.passtypefilter = "1";
                    }
                    else if (passfilter.ToLower().Equals("subpass"))
                    {
                        lstmodel = lstmodel.Where(x => x.Subpass).ToList();
                        SecurityPassFilterViewModel.passtypefilter = "2";
                    }
                }
                //if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                //{
                //    lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire) || x.status == SecurityPassStatus.PendingApprovalwithSecurity).ToList().ForEach(x => { x.strstatus = (Translate.Text("epassstatus.approved")); x.strclass = "approved"; });
                //}
                //else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
                //{
                //    lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire)).ToList().ForEach(x => { x.strstatus = (Translate.Text("epassstatus.approved")); x.strclass = "approved"; });
                //}
                if (!string.IsNullOrWhiteSpace(statustxt))
                {
                    if (statustxt.ToLower().Equals("rejected"))
                    {
                        SecurityPassFilterViewModel.strdataindex = "3";
                        lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Rejected).ToList();
                    }
                    else if (statustxt.ToLower().Equals("approved"))
                    {
                        SecurityPassFilterViewModel.strdataindex = "1";
                        if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                        {
                            lstmodel = lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire) || x.status == SecurityPassStatus.PendingApprovalwithSecurity).ToList();
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
                        {
                            lstmodel = lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire)).ToList();
                        }
                        // lstmodel.ForEach(x => { x.strclass = (Translate.Text("epassstatus.approved")); x.strstatus = "approved"; });
                    }
                    else if (statustxt.ToLower().Equals("pending"))
                    {
                        SecurityPassFilterViewModel.strdataindex = "2";
                        if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.PendingApprovalwithCoordinator).ToList();
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.PendingApprovalwithSecurity).ToList();
                        }
                    }
                }
                //if (!string.IsNullOrWhiteSpace(keyword))
                //{
                //    lstmodel = lstmodel.Where(x => x.name.ToLower().Contains(keyword.ToLower()) || x.emiratesId.ToLower().Contains(keyword.ToLower()) || x.passportNumber.ToLower().Contains(keyword.ToLower()) || x.visaNumber.ToLower().Contains(keyword.ToLower()) || x.passNumber.ToLower().Contains(keyword.ToLower())).ToList();
                //}
                if (!string.IsNullOrWhiteSpace(namesort))
                {
                    if (namesort.ToLower().Equals("ascending"))
                    {
                        lstmodel = lstmodel.OrderBy(x => x.name).ToList();
                    }
                    else if (namesort.ToLower().Equals("descending"))
                    {
                        lstmodel = lstmodel.OrderByDescending(x => x.name).ToList();
                    }
                }
                SecurityPassFilterViewModel.namesort = namesort;
                SecurityPassFilterViewModel.totalpage = Pager.CalculateTotalPages(lstmodel.Count(), pagesize);
                SecurityPassFilterViewModel.pagination = SecurityPassFilterViewModel.totalpage > 1 ? true : false;
                SecurityPassFilterViewModel.pagenumbers = SecurityPassFilterViewModel.totalpage > 1 ? Pager.GetPaginationRange(page, SecurityPassFilterViewModel.totalpage) : new List<int>();
                lstmodel = lstmodel.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                SecurityPassFilterViewModel.lstpasses = new JavaScriptSerializer().Serialize(lstmodel);
                return Json(new { status = true, Message = SecurityPassFilterViewModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// The ePassActivityLogs.
        ///// </summary>
        ///// <param name="eFolderId">The eFolderId<see cref="string"/>.</param>
        ///// <returns>The <see cref="ActionResult"/>.</returns>
        //[HttpGet]
        //public ActionResult ePassActivityLogs(string eFolderId)
        //{
        //    List<SecurityPassActivityLogModel> activityLogs = GetActivityLogs(eFolderId);
        //    return PartialView("~/Views/Feature/GatePass/ePass/_PassActivityLogs.cshtml", activityLogs);
        //}

        /// <summary>
        /// The ePassWorkLogs.
        /// </summary>
        /// <param name="passNo">The passNo<see cref="string"/></param>
        /// <param name="currentPage">The currentPage<see cref="int"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        //[HttpGet]
        //public ActionResult ePassWorkLogs(string passNo, int currentPage)
        //{
        //    string startDate = DateTime.Now.AddMonths(-1).ToShortDateString(), endDate = DateTime.Now.ToShortDateString();
        //    SecurityPassWorkLogFilterModel employeeTimeLogList = GetWorkLogs(passNo, startDate, endDate, currentPage);
        //    return PartialView("~/Views/Feature/GatePass/ePass/_PassWorkLogs.cshtml", employeeTimeLogList);
        //}

        /// <summary>
        /// The AjaxWorkLogs.
        /// </summary>
        /// <param name="currentPage">The currentPage<see cref="int"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        //[HttpPost]
        //public ActionResult AjaxWorkLogs(int currentPage)
        //{
        //    int maxRows = 1;
        //    SecurityPassWorkLogFilterModel filterWorkLog = new SecurityPassWorkLogFilterModel();
        //    List<SecurityPassWorkLogModel> employeeTimeLogList = new List<SecurityPassWorkLogModel>();
        //    if (CacheProvider.TryGet(CacheKeys.EPASS_WORK_LOGS, out employeeTimeLogList))
        //    {
        //        double pageCount = (double)(employeeTimeLogList.Count() / Convert.ToDecimal(maxRows));
        //        filterWorkLog.ListWorkLogs = employeeTimeLogList.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

        //        filterWorkLog.totalpage = Pager.CalculateTotalPages(employeeTimeLogList.Count, maxRows);
        //        filterWorkLog.pagination = filterWorkLog.totalpage > 1 ? true : false;
        //        filterWorkLog.pagenumbers = filterWorkLog.totalpage > 1 ? CommonUtility.GetPaginationRange(currentPage, filterWorkLog.totalpage) : new List<int>();
        //        filterWorkLog.page = currentPage;
        //    }
        //    return PartialView("~/Views/Feature/GatePass/ePass/_PassWorkLogs.cshtml", filterWorkLog);
        //}

        /// <summary>
        /// The ePassWorkLogs.
        /// </summary>
        /// <param name="passNo">The passNo<see cref="string"/></param>
        /// <param name="startDate">The startDate<see cref="string"/></param>
        /// <param name="endDate">The endDate<see cref="string"/></param>
        /// <param name="currentPage">The currentPage<see cref="int"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        //[HttpPost]
        //public ActionResult ePassWorkLogs(string passNo, string startDate = "", string endDate = "", int currentPage = 1)
        //{
        //    SecurityPassWorkLogFilterModel employeeTimeLogList = GetWorkLogs(passNo, startDate, endDate, currentPage);
        //    return PartialView("~/Views/Feature/GatePass/ePass/_PassWorkLogs.cshtml", employeeTimeLogList);
        //}

        /// <summary>
        /// The BlockUnBlockPassOrUser.
        /// </summary>
        /// <param name="Passnumber">The Passnumber<see cref="string"/>.</param>
        /// <param name="Comments">The Comments<see cref="string"/>.</param>
        /// <param name="blockUser">The blockUser<see cref="bool"/>.</param>
        /// <param name="BlockPass">The BlockPass<see cref="bool"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, OnlyCM, ValidateAntiForgeryToken]
        public ActionResult BlockUnBlockPassOrUser(string Passnumber, string Comments, bool blockUser, bool BlockPass)
        {
            string successMessage = string.Empty;
            try
            {
                List<SecurityPassViewModel> lstRecent = new List<SecurityPassViewModel>();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstRecent))
                {
                    SecurityPassViewModel firstDetail = lstRecent.Where(x => x.passNumber.ToLower().Equals(Passnumber.ToLower())).FirstOrDefault();
                    if (firstDetail != null)
                    {
                        firstDetail.Blockcomments = Comments;
                        if (blockUser)
                        {
                            Tuple<bool, string, string> isSaved = UpdatePassStatus(passDetail: firstDetail, isBlocked: true, isUserBlocked: blockUser);
                            if (isSaved.Item1)
                            {
                                successMessage = Translate.Text("Successfully blocked user / pass");
                            }
                        }
                        else if (BlockPass)
                        {
                            Tuple<bool, string, string> isSaved = UpdatePassStatus(passDetail: firstDetail, isBlocked: BlockPass);
                            if (isSaved.Item1)
                            {
                                successMessage = Translate.Text("Successfully blocked user / pass");
                            }
                        }
                        GetListofPasses();
                        return Json(new { status = true, Message = successMessage }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        successMessage = Translate.Text("Please enter valid value");
                    }
                }
            }
            catch (System.Exception ex)
            {
                successMessage = Translate.Text("Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            return Json(new { status = false, Message = successMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The UnblockPass.
        /// </summary>
        /// <param name="Passnumber">The Passnumber<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnblockPass(string Passnumber)
        {
            string successMessage = string.Empty;
            try
            {
                List<SecurityPassViewModel> lstRecent = new List<SecurityPassViewModel>();
                if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstRecent))
                {
                    SecurityPassViewModel firstDetail = lstRecent.Where(x => x.passNumber.ToLower().Equals(Passnumber.ToLower())).FirstOrDefault();
                    if (firstDetail != null)
                    {
                        Tuple<bool, string, string> isSaved = UpdatePassStatus(passDetail: firstDetail, isPassUnblocked: true);
                        if (isSaved.Item1)
                        {
                            successMessage = Translate.Text("Successfully unblock pass.");
                            GetListofPasses();
                        }
                        return Json(new { status = true, Message = successMessage }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        successMessage = Translate.Text("Please enter valid value");
                    }
                }
            }
            catch (System.Exception ex)
            {
                successMessage = Translate.Text("Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            return Json(new { status = false, Message = successMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The BlockedUsersAjax.
        /// </summary>
        /// <param name="pagesize">The pagesize<see cref="int"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="namesort">The namesort<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BlockedUsersAjax(int pagesize = 5, string keyword = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            List<SecurityBlockedUserViewModel> lstmodel = new List<SecurityBlockedUserViewModel>();
            if (!CacheProvider.TryGet(CacheKeys.EPASS_BLOCKED_USERS, out lstmodel))
            {
                if (lstmodel == null)
                {
                    lstmodel = GetBlockedUsers();
                }
            }
            SecurityPassFilterViewModel SecurityPassFilterViewModel = new SecurityPassFilterViewModel
            {
                page = page
            };
            pagesize = pagesize > 30 ? 30 : pagesize;
            SecurityPassFilterViewModel.strdataindex = "0";
            if (lstmodel != null && lstmodel.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    lstmodel = lstmodel.Where(x => x.passNo != null && (x.name.ToLower().Contains(keyword.ToLower()) || x.emiratesID.ToLower().Contains(keyword.ToLower()) || x.passportNumber.ToLower().Contains(keyword.ToLower()) || x.visaNumber.ToLower().Contains(keyword.ToLower()) || x.passNo.ToLower().Contains(keyword.ToLower()))).ToList();
                }
                if (!string.IsNullOrWhiteSpace(namesort))
                {
                    if (namesort.ToLower().Equals("ascending"))
                    {
                        lstmodel = lstmodel.OrderBy(x => x.name).ToList();
                    }
                    else if (namesort.ToLower().Equals("descending"))
                    {
                        lstmodel = lstmodel.OrderByDescending(x => x.name).ToList();
                    }
                }
                SecurityPassFilterViewModel.namesort = namesort;
                SecurityPassFilterViewModel.totalpage = Pager.CalculateTotalPages(lstmodel.Count(), pagesize);
                SecurityPassFilterViewModel.pagination = SecurityPassFilterViewModel.totalpage > 1 ? true : false;
                SecurityPassFilterViewModel.pagenumbers = SecurityPassFilterViewModel.totalpage > 1 ? Pager.GetPaginationRange(page, SecurityPassFilterViewModel.totalpage) : new List<int>();
                lstmodel = lstmodel.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                SecurityPassFilterViewModel.lstpasses = new JavaScriptSerializer().Serialize(lstmodel);
                return Json(new { status = true, Message = SecurityPassFilterViewModel }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The UnblockUserEntry.
        /// </summary>
        /// <param name="eFolderId">The eFolderId<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UnblockUserEntry(string eFolderId)
        {
            //if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
            //{
            //    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            //}
            //List<SecurityBlockedUserViewModel> lstpasses = new List<SecurityBlockedUserViewModel>();
            //if (CacheProvider.TryGet(CacheKeys.EPASS_BLOCKED_USERS, out lstpasses))
            //{
            //    SecurityBlockedUserViewModel itemTounblock = lstpasses.SingleOrDefault(r => r.eFolderId.ToString().Equals(eFolderId));
            //    if (itemTounblock != null)
            //    {
            //        bool unblockentry = UnblockUser(itemTounblock, false);
            //        if (unblockentry)
            //        {
            //            return Json(new { status = true });
            //        }
            //    }
            //    return Json(new { status = false });
            //}
            //return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_BLOCKEDUSERS);

            string successMessage = string.Empty;
            try
            {
                List<SecurityBlockedUserViewModel> lstRecent = new List<SecurityBlockedUserViewModel>();
                //CacheProvider.Store(CacheKeys.EPASS_BLOCKEDUSERS, new CacheItem<List<SecurityBlockedUserViewModel>>(LstBlockedUsers, TimeSpan.FromMinutes(40)));
                if (CacheProvider.TryGet(CacheKeys.EPASS_BLOCKEDUSERS, out lstRecent))
                {
                    SecurityBlockedUserViewModel firstDetail = lstRecent.Where(x => x.eFolderId.ToLower().Equals(eFolderId.ToLower())).FirstOrDefault();
                    if (firstDetail != null)
                    {
                        Tuple<bool, string, string> isSaved = UpdatePassStatus(blockeduser: firstDetail, unblockmethod: true);
                        if (isSaved.Item1)
                        {
                            successMessage = Translate.Text("Successfully unblocked user");
                            //GetListofPasses();
                            return Json(new { status = true, Message = successMessage }, JsonRequestBehavior.AllowGet);
                        }
                        successMessage = Translate.Text("Unable to process the request, Kindly resubmit the request again");
                    }
                    else
                    {
                        successMessage = Translate.Text("Please enter valid value");
                    }
                }
            }
            catch (System.Exception ex)
            {
                successMessage = Translate.Text("Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            return Json(new { status = false, Message = successMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The ePassBlockedUsers.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet, OnlyCM]
        public ActionResult ePassBlockedUsers()
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            return PartialView("~/Views/Feature/GatePass/ePass/_BlockedUsers.cshtml");
        }

        /// <summary>
        /// The PendingApprovalpassesAjax.
        /// </summary>
        /// <param name="pagesize">The pagesize<see cref="int"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <param name="statustxt">The statustxt<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <param name="namesort">The namesort<see cref="string"/>.</param>
        /// <param name="passfilter">The passfilter<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PendingApprovalpassesAjax(int pagesize = 5, string keyword = "", string statustxt = "all", int page = 1, string namesort = "", string passfilter = "")
        {
            keyword = keyword.Trim();
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) || CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstmodel))
            {
                SecurityPassFilterViewModel SecurityPassFilterViewModel = new SecurityPassFilterViewModel
                {
                    page = page
                };
                pagesize = pagesize > 30 ? 30 : pagesize;
                if (lstmodel != null && lstmodel.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(passfilter))
                    {
                        if (passfilter.ToLower().Equals("main"))
                        {
                            lstmodel = lstmodel.Where(x => !x.Subpass).ToList();
                            SecurityPassFilterViewModel.passtypefilter = "1";
                        }
                        else if (passfilter.ToLower().Equals("subpass"))
                        {
                            lstmodel = lstmodel.Where(x => x.Subpass).ToList();
                            SecurityPassFilterViewModel.passtypefilter = "2";
                        }
                    }
                    if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                    {
                        lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire) || x.status == SecurityPassStatus.PendingApprovalwithSecurity).ToList().ForEach(x => { x.strstatus = (Translate.Text("epassstatus.approved")); x.strclass = "approved"; });
                    }
                    else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator))
                    {
                        lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire)).ToList().ForEach(x => { x.strstatus = (Translate.Text("epassstatus.approved")); x.strclass = "approved"; });
                    }
                    if (!string.IsNullOrWhiteSpace(statustxt))
                    {
                        if (statustxt.ToLower().Equals("rejected"))
                        {
                            lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.Rejected).ToList();
                        }
                        else if (statustxt.ToLower().Equals("active"))
                        {
                            if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                            {
                                lstmodel = lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire) || x.status == SecurityPassStatus.PendingApprovalwithSecurity).ToList();
                            }
                            else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator))
                            {
                                lstmodel = lstmodel.Where(x => (x.status == SecurityPassStatus.Active || x.status == SecurityPassStatus.SoontoExpire)).ToList();
                            }
                            // lstmodel.ForEach(x => { x.strclass = (Translate.Text("epassstatus.approved")); x.strstatus = "approved"; });
                        }
                        else if (statustxt.ToLower().Equals("pending"))
                        {
                            if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                            {
                                lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.PendingApprovalwithCoordinator).ToList();
                            }
                            else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator))
                            {
                                lstmodel = lstmodel.Where(x => x.status == SecurityPassStatus.PendingApprovalwithSecurity).ToList();
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        lstmodel = lstmodel.Where(x => x.name.ToLower().Contains(keyword.ToLower()) || x.emiratesId.ToLower().Contains(keyword.ToLower()) || x.passportNumber.ToLower().Contains(keyword.ToLower()) || x.visaNumber.ToLower().Contains(keyword.ToLower()) || x.passNumber.ToLower().Contains(keyword.ToLower())).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(namesort))
                    {
                        if (namesort.ToLower().Equals("ascending"))
                        {
                            lstmodel = lstmodel.OrderBy(x => x.name).ToList();
                        }
                        else if (namesort.ToLower().Equals("descending"))
                        {
                            lstmodel = lstmodel.OrderByDescending(x => x.name).ToList();
                        }
                    }
                    SecurityPassFilterViewModel.namesort = namesort;
                    SecurityPassFilterViewModel.totalpage = Pager.CalculateTotalPages(lstmodel.Count(), pagesize);
                    SecurityPassFilterViewModel.pagination = SecurityPassFilterViewModel.totalpage > 1 ? true : false;
                    SecurityPassFilterViewModel.pagenumbers = SecurityPassFilterViewModel.totalpage > 1 ? Pager.GetPaginationRange(page, SecurityPassFilterViewModel.totalpage) : new List<int>();
                    lstmodel = lstmodel.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    SecurityPassFilterViewModel.lstpasses = new JavaScriptSerializer().Serialize(lstmodel);
                    return Json(new { status = true, Message = SecurityPassFilterViewModel }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The ePassAdminLogin.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ePassAdminLogin()
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) || CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator)))
            {
                return PartialView("~/Views/Feature/GatePass/ePass/Admin/_Login.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINDASHBOARD);
        }

        /// <summary>
        /// ePass Login to verify SRM user.
        /// </summary>
        /// <param name="model">.</param>
        /// <returns>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ePassAdminLogin(ePassLogin model)
        {
            if (fnValidateUser(model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINDASHBOARD);
            }

            ModelState.AddModelError(string.Empty, Translate.Text("Epass.InvalidUser"));

            return PartialView("~/Views/Feature/GatePass/ePass/Admin/_Login.cshtml");
        }

        /// <summary>
        /// The ePassManagement.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ePassManagement()
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin)
                || CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) ||
                CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            ePassLogin model = new ePassLogin
            {
                UserName = CurrentPrincipal.Name,
                lstpassess = GetListofPasses(),
                UserType = CurrentPrincipal.Role,
                Userid = CurrentPrincipal.BusinessPartner,
                onedayinitiate = CurrentPrincipal.HasActiveAccounts
            };
            return PartialView("~/Views/Feature/GatePass/ePass/_ePassManagement.cshtml", model);
        }

        /// <summary>
        /// The ePassPendingApprovalDetails.
        /// </summary>
        /// <param name="searchbyNumber">The searchbyNumber<see cref="string"/>.</param>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ePassPendingApprovalDetails(string searchbyNumber, string passNumber)
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) || CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }

            SecurityPassViewModel passDetails = null;
            List<SecurityPassViewModel> lstpassDetails = new List<SecurityPassViewModel>();
            BasePassViewModel relPassdetail = new BasePassViewModel();

            BasePassDetailModel lstrelPassdetail = GetPassByPassNumber(passNumber);
            if (lstrelPassdetail != null && lstrelPassdetail.MainPass != null)
            {
                BaseMainPassDetailModel mainpassitem = lstrelPassdetail.MainPass.FirstOrDefault();

                lstpassDetails.Add(new SecurityPassViewModel
                {
                    name = mainpassitem.ePassVisitorName,
                    mainpassNumber = mainpassitem.ePassPassNo,
                    passNumber = mainpassitem.ePassPassNo,
                    passType = mainpassitem.ePassPassType,
                    passTypeText = mainpassitem.ePassPassType,
                    profession = mainpassitem.ePassProfession,
                    Designation = mainpassitem.ePassDesignation,
                    passExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassExpiryDate) :
                                     (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassDate(mainpassitem.ePassVisitingDate) : default(DateTime?)),
                    passIssueDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassIssueDate)) ? FormatEpassDate(mainpassitem.ePassPassIssueDate) :
                    (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassDate(mainpassitem.ePassVisitingDate) : default(DateTime?)),
                    CreatedDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassCreatedOn)) ? FormatEpassDate(mainpassitem.ePassCreatedOn) : default(DateTime?),
                    strpassExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassExpiryDate)) ? FormatEpassstrDate(mainpassitem.ePassPassExpiryDate) :
                          (mainpassitem.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(mainpassitem.ePassVisitingDate) ? FormatEpassstrDate(mainpassitem.ePassVisitingDate) : string.Empty),
                    status = assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)),
                    strstatus = Translate.Text("epassstatus." + assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)).ToString().ToLower()),
                    strclass = assignStatus(mainpassitem.ePassPassStatus, mainpassitem.ePassPassExpiryDate, (mainpassitem.ePassIsBlocked == "Yes" ? true : false)).ToString(),
                    nationality = mainpassitem.ePassNationality,
                    emiratesId = mainpassitem.ePassEmiratesID,
                    emiratesExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassEmiratesiDExpiry)) ? FormatEpassDate(mainpassitem.ePassEmiratesiDExpiry) : default(DateTime?),
                    visaNumber = mainpassitem.ePassVisaNumber,
                    visaExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassVisaExpiryDate)) ? FormatEpassDate(mainpassitem.ePassVisaExpiryDate) : default(DateTime?),
                    emailLimit = (!string.IsNullOrWhiteSpace(mainpassitem.ePassEmailLimit)) ? Convert.ToInt16(mainpassitem.ePassEmailLimit) : 0,
                    smsLimit = (!string.IsNullOrWhiteSpace(mainpassitem.ePassSMSLimit)) ? Convert.ToInt16(mainpassitem.ePassSMSLimit) : 0,
                    downloadLimit = (!string.IsNullOrWhiteSpace(mainpassitem.ePassDownloadLimit)) ? Convert.ToInt16(mainpassitem.ePassDownloadLimit) : 0,
                    passportNumber = mainpassitem.ePassPassportNumber,
                    passportExpiryDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassPassportExpiryDate)) ? FormatEpassDate(mainpassitem.ePassPassportExpiryDate) : default(DateTime?),
                    fromTime = mainpassitem.ePassFromDateTime,
                    toTime = mainpassitem.ePassToDateTime,
                    mobile = mainpassitem.ePassMobileNumber,
                    email = mainpassitem.ePassEmailAddress,
                    VisitorEmail = mainpassitem.ePassVisitorEmailID,
                    SeniorManagerEmail = mainpassitem.ePassVisitorEmailID,
                    Location = lstselectedlocations(mainpassitem.ePassLocation),
                    Subcontractor = mainpassitem.ePassSubContractorID,
                    companyName = mainpassitem.ePassCompanyName,
                    projectName = mainpassitem.ePassProjectName,
                    projectStartDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassProjectStartName)) ? FormatEpassDate(mainpassitem.ePassProjectStartName) : default(DateTime?),
                    projectEndDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassProjectEndDate)) ? FormatEpassDate(mainpassitem.ePassProjectEndDate) : default(DateTime?),
                    projectId = mainpassitem.ePassProjectID,
                    projectStatus = mainpassitem.ePassProjectStatus,
                    departmentName = mainpassitem.ePassDepartmentName,
                    IsBlocked = (mainpassitem.ePassIsBlocked == "Yes" ? true : false),
                    DEWAID = mainpassitem.ePassDEWAID,
                    DEWAdesignation = mainpassitem.ePassDesignation,
                    Subpass = false,
                    wppass = false,
                    VehicleRegNumber = mainpassitem.ePassVehicleNo,
                    VehRegistrationDate = (!string.IsNullOrWhiteSpace(mainpassitem.ePassVehicleRegDate)) ? FormatEpassDate(mainpassitem.ePassVehicleRegDate) : default(DateTime?),
                    strpassVehicleRegDate = !string.IsNullOrWhiteSpace(mainpassitem.ePassVehicleRegDate) ? FormatEpassDate(mainpassitem.ePassVehicleRegDate).ToString("MMMM dd, yyyy") : string.Empty,
                    pendingwith = (mainpassitem.ePassSecurityApprovers != null && mainpassitem.ePassPassStatus != null && !string.IsNullOrWhiteSpace(mainpassitem.ePassSecurityApprovers.ToString()) && !string.IsNullOrWhiteSpace(mainpassitem.ePassPassStatus.ToString())) ? (mainpassitem.ePassPassStatus.ToString().ToLower().Equals("Dept Coordinator".ToLower()) ? mainpassitem.ePassSecurityApprovers.ToString() : (mainpassitem.ePassPassStatus.ToString().ToLower().Equals("Security Team".ToLower()) ? Translate.Text("Epass.SecurityAdmin") : string.Empty)) : string.Empty,
                    passAttachements = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower())).Any() ?
                    lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower())).Select(y => new SecurityPassAttachement
                    {
                        docType = y.FileType,
                        fileCategory = y.FileCategory,
                        fileContent = y.SupportingFile,
                        fileName = y.FileName,
                        fileContentType = y.FileContentType
                    }).ToList() : null,

                    profilepic = lstrelPassdetail != null && lstrelPassdetail.Attachments != null && lstrelPassdetail.Attachments.Count > 0 && lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).Any() ?
                    (lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).FirstOrDefault() != null) ?
                     Convert.FromBase64String(lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).FirstOrDefault().SupportingFile) != null ?
                     Convert.ToBase64String(Convert.FromBase64String(lstrelPassdetail.Attachments.Where(y => y.ReqID.ToLower().Equals(mainpassitem.ePassPassNo.ToLower()) && y.FileCategory.Equals("Photo")).FirstOrDefault().SupportingFile))
                     : string.Empty
                     : string.Empty
                    : string.Empty,
                });

                if (!string.IsNullOrEmpty(relPassdetail.ePassLinkExpiry)) { passDetails.linkExpiryDate = DateTime.Parse(relPassdetail.ePassLinkExpiry); }

                if (lstrelPassdetail.SubPasses != null && lstrelPassdetail.SubPasses.Count > 0)
                {
                    lstrelPassdetail.SubPasses.ToList().ForEach(sp =>
                    {
                        lstpassDetails.Add(new SecurityPassViewModel()
                        {
                            name = lstpassDetails.FirstOrDefault().name,
                            passNumber = sp.subPassRequestID,
                            mainpassNumber = lstpassDetails.FirstOrDefault().passNumber,
                            Subpass = true,
                            passType = sp.subPassNewPassType,
                            passTypeText = Translate.Text("Epass." + sp.subPassNewPassType),
                            passExpiryDate = DateTime.Parse(sp.subPassValidTo),
                            strpassExpiryDate = FormatEpassstrDate(DateTime.Parse(sp.subPassValidTo).ToString("MMMM dd, yyyy")),
                            passIssueDate = DateTime.Parse(sp.subPassValidFrom),
                            CreatedDate = DateTime.Parse(sp.subPassCreatedOn),
                            status = assignStatus(sp.subPassStatus, sp.subPassValidTo, lstpassDetails.FirstOrDefault().IsBlocked),
                            strstatus = Translate.Text("epassstatus." + assignStatus(sp.subPassStatus, sp.subPassValidTo, lstpassDetails.FirstOrDefault().IsBlocked).ToString().ToLower()),
                            strclass = assignStatus(sp.subPassStatus, lstpassDetails.FirstOrDefault().passExpiryDate, lstpassDetails.FirstOrDefault().IsBlocked).ToString(),
                            SeniorManagerEmail = lstpassDetails.FirstOrDefault().SeniorManagerEmail,
                            fromTime = DateTime.Parse(sp.subPassValidFrom).TimeOfDay.ToString(),
                            toTime = DateTime.Parse(sp.subPassValidTo).TimeOfDay.ToString(),
                            emiratesId = lstpassDetails.FirstOrDefault().emiratesId,
                            visaNumber = lstpassDetails.FirstOrDefault().visaNumber,
                            passportNumber = lstpassDetails.FirstOrDefault().passportNumber,
                            pendingwith = sp.subPassStatus.ToLower().Equals("initiated") ? sp.subPassDepartmentApprover : (sp.subPassStatus.ToLower().Equals("dept approved") ? sp.subPassSecurityApprovers : (sp.subPassStatus.ToLower().Equals("security approved") ? "Completed" : "Unknown"))
                        });
                    });
                }
                lstpassDetails = lstpassDetails.OrderByDescending(x => x.CreatedDate).ToList();

                CacheProvider.Store(CacheKeys.EPASS_DETAILS, new CacheItem<List<SecurityPassViewModel>>(lstpassDetails, TimeSpan.FromMinutes(20)));

                ViewBag.role = CurrentPrincipal.Role;
                ViewBag.usertype = CurrentPrincipal.Role;
                return PartialView("~/Views/Feature/GatePass/ePass/_PassApprovalDetails.cshtml", lstpassDetails);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINDASHBOARD);
            }
        }

        /// <summary>
        /// The ApprovePasses.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <param name="eFolderId">The eFolderId<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ApprovePasses(string passNumber, string eFolderId)
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) || CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            SecurityApproveRejectPassViewModel model = new SecurityApproveRejectPassViewModel();
            List<SecurityPassViewModel> passViewModels = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_DETAILS, out passViewModels))
            {
                if (passViewModels != null && passViewModels.Count > 0 && passViewModels.Where(x => x.passNumber.Equals(passNumber)).Any())
                {
                    SecurityPassViewModel ePassDetail = passViewModels.Where(x => x.passNumber.Equals(passNumber)).FirstOrDefault();
                    if (ePassDetail != null)
                    {
                        model.IsOneDayPass = ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass));

                        model.PassNumber = passNumber;
                        model.eFolderID = eFolderId;
                        model.visaExpiryDate = ePassDetail.visaExpiryDate;
                        model.PassIssueDate = (ePassDetail.passIssueDate != null && ePassDetail.passIssueDate.HasValue) ? ePassDetail.passIssueDate : DateTime.Now;
                        model.PassExpiryDate = (ePassDetail.passExpiryDate != null && ePassDetail.passExpiryDate.HasValue) ? ePassDetail.passExpiryDate : DateTime.Now.AddDays(1);
                        model.FromTime = ePassDetail.fromTime;
                        model.ToTime = ePassDetail.toTime;
                        model.PassType = ePassDetail.passType;
                        //model.SelectedLocation = ePassDetail.Location;
                        model.OfficeLocations = EpassHelper.GetLocations(DataSources.EpassgenerationLocations);
                        model.SelectedLocation = model.OfficeLocations.Where(x => ePassDetail.Location.Contains(x.Text)).Select(x => x.Value).ToList();
                        if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                        {
                            //model.Location = GetLocation();
                            //The below code is for oneday pass, we must update other bindings to use this logic as eform will be discarded soon.

                            ViewBag.Role = "SupplierAdmin";
                        }
                        else
                        {
                            ViewBag.Role = "SupplierSecurity";
                        }
                        //if (model.IsOneDayPass{ model.PassIssueDate=ePassDetail.vis })
                        return PartialView("~/Views/Feature/GatePass/ePass/_ApprovePasses.cshtml", model);
                    }
                }
            }

            return new EmptyResult();
        }

        /// <summary>
        /// The ApprovePasses.
        /// </summary>
        /// <param name="passnumber">The passnumber<see cref="string"/>.</param>
        /// <param name="eFolderId">The eFolderId<see cref="string"/>.</param>
        /// <param name="locations">The locations<see cref="string"/>.</param>
        /// <param name="fromdate">The fromdate<see cref="string"/>.</param>
        /// <param name="todate">The todate<see cref="string"/>.</param>
        /// <param name="fromtime">The fromtime<see cref="string"/>.</param>
        /// <param name="totime">The totime<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApprovePasses(string passnumber, string eFolderId, string locations = "", string fromdate = "", string todate = "", string fromtime = "", string totime = "")
        {
            string message = string.Empty;
            if (Sitecorex.Context.Culture.ToString().Equals("ar-AE"))
            {
                fromdate = fromdate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                todate = todate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }

            SecurityApproveRejectPassViewModel model = null;
            List<SecurityPassViewModel> passViewModels = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_DETAILS, out passViewModels))
            {
                if (passViewModels != null && passViewModels.Count > 0 && passViewModels.Where(x => x.passNumber.Equals(passnumber)).Any())
                {
                    SecurityPassViewModel ePassDetail = passViewModels.Where(x => x.passNumber.Equals(passnumber)).FirstOrDefault();
                    if (ePassDetail != null)
                    {
                        model = new SecurityApproveRejectPassViewModel
                        {
                            IsOneDayPass = ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass))
                        };

                        DateTime.TryParse(fromdate, out DateTime fromdateTime);
                        DateTime.TryParse(todate, out DateTime todatetime);

                        if (string.IsNullOrEmpty(fromdate)) { fromdateTime = ePassDetail.passIssueDate ?? DateTime.Now; }
                        if (string.IsNullOrEmpty(todate)) { todatetime = ePassDetail.passExpiryDate ?? fromdateTime.AddDays(1); }

                        if (ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.LongTerm)) || ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.ShortTerm)) || ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)))
                        {
                            model.PassType = "Main";
                        }
                        else
                        {
                            model.PassType = "Sub";
                        }

                        model.PassNumber = ePassDetail.passNumber;
                        List<SelectListItem> locs = EpassHelper.GetLocations(DataSources.EpassgenerationLocations);
                        if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                        {
                            model.PassStatus = "Dept Approved";

                            model.PassIssueDate = fromdateTime;
                            model.PassExpiryDate = todatetime;
                            model.FromTime = fromtime;
                            model.ToTime = totime;
                            model.StrLocation = locations;
                            model.ApprovalType = "Dept";
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
                        {
                            model.PassStatus = "Security Approved";

                            model.PassIssueDate = ePassDetail.passIssueDate;
                            model.PassExpiryDate = ePassDetail.passportExpiryDate;
                            model.FromTime = ePassDetail.fromTime;
                            model.ToTime = ePassDetail.toTime;

                            string[] selLocs = locs.Where(x => ePassDetail.Location != null && ePassDetail.Location.Contains(x.Text)).Select(x => x.Value).ToArray();
                            model.StrLocation = (selLocs != null && selLocs.Length > 0) ? string.Join(";", selLocs) : string.Empty;
                            model.ApprovalType = "Sec";
                        }

                        if (string.IsNullOrWhiteSpace(fromtime))
                        {
                            model.FromTime = ePassDetail.fromTime;
                        }
                        if (string.IsNullOrWhiteSpace(totime))
                        {
                            model.ToTime = ePassDetail.toTime;
                        }
                    }
                }
            }

            if (model != null)
            {
                Tuple<bool, string, string> isSaved = UpdatePassStatus(model);
                if (isSaved.Item1)
                {
                    message = Translate.Text("ePass.ApprovedPass.Message");
                    GetListofPasses();
                }
                else
                {
                    message = Translate.Text("ePass.ApprovedPass.ErrorMessage");
                }

                return Json(new { status = isSaved.Item1, Message = message }, JsonRequestBehavior.AllowGet);
            }
            message = Translate.Text("ePass.ApprovedPass.ErrorMessage");
            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The RejectPasses.
        /// </summary>
        /// <param name="passnumber">The passnumber<see cref="string"/>.</param>
        /// <param name="eFolderId">The eFolderId<see cref="string"/>.</param>
        /// <param name="comments">The comments<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RejectPasses(string passnumber, string eFolderId, string comments)
        {
            string message = string.Empty;
            SecurityApproveRejectPassViewModel model = null;
            List<SecurityPassViewModel> passViewModels = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_DETAILS, out passViewModels))
            {
                if (passViewModels != null && passViewModels.Count > 0 && passViewModels.Where(x => x.passNumber.Equals(passnumber)).Any())
                {
                    SecurityPassViewModel ePassDetail = passViewModels.Where(x => x.passNumber.Equals(passnumber)).FirstOrDefault();
                    if (ePassDetail != null)
                    {
                        model = new SecurityApproveRejectPassViewModel();
                        if (ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.LongTerm)) || ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.ShortTerm)) || ePassDetail.passType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)))
                        {
                            model.PassType = "Main";
                        }
                        else
                        {
                            model.PassType = "Sub";
                        }

                        model.PassNumber = ePassDetail.passNumber;
                        model.PassIssueDate = ePassDetail.passIssueDate;
                        model.PassExpiryDate = ePassDetail.passportExpiryDate;
                        model.FromTime = ePassDetail.fromTime;
                        model.ToTime = ePassDetail.toTime;
                        model.StrLocation = ePassDetail.Location != null && ePassDetail.Location.Count > 0 ? string.Join(";", ePassDetail.Location) : string.Empty;
                        model.Comments = comments;
                        if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
                        {
                            model.ApprovalType = "Dept";
                            model.PassStatus = "Dept Rejected";
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
                        {
                            model.ApprovalType = "Sec";
                            model.PassStatus = "Security Rejected";
                        }
                    }
                }
            }

            if (model != null)
            {
                Tuple<bool, string, string> isSaved = UpdatePassStatus(model);
                if (isSaved.Item1)
                {
                    message = Translate.Text("ePass.RejectedPass.Message");
                    GetListofPasses();
                }
                else
                {
                    message = Translate.Text("ePass.ApprovedPass.ErrorMessage");
                }

                return Json(new { status = isSaved.Item1, Message = message }, JsonRequestBehavior.AllowGet);
            }
            message = Translate.Text("ePass.ApprovedPass.ErrorMessage");
            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The GetPendingApprovalPasses.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult GetPendingApprovalPasses()
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) || CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            //var lstpass = GetListOfPendingApprovalPasses();
            return PartialView("~/Views/Feature/GatePass/ePass/_PendingApprovalPasses.cshtml");
        }

        /// <summary>
        /// The ePassSearch.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ePassSearch()
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) || CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            return PartialView("~/Views/Feature/GatePass/ePass/Admin/_Search.cshtml", new List<SecurityPassViewModel>());
        }

        /// <summary>
        /// The GatePassentry.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        //[HttpGet]
        //public ActionResult GatePassentry()
        //{
        //    ViewBag.headeragent = Request.Headers["User-Agent"];
        //    return PartialView("~/Views/Feature/GatePass/ePass/Admin/_IndividualPass.cshtml", null);
        //}

        /// <summary>
        /// The GatePassentry.
        /// </summary>
        /// <param name="radios_group1">The radios_group1<see cref="string"/></param>
        /// <param name="passnumber">The passnumber<see cref="string"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult GatePassentry(string radios_group1, string searchbyNumber, string passnumber)
        //{
        //    ViewBag.headeragent = Request.Headers["User-Agent"];
        //    if (!string.IsNullOrWhiteSpace(radios_group1))
        //    {
        //        ViewBag.checkedbutton = radios_group1;
        //    }
        //    if (!string.IsNullOrWhiteSpace(passnumber) && !string.IsNullOrWhiteSpace(searchbyNumber))
        //    {
        //        //change it atlast
        //        //SecurityPassViewModel pass = GetPassDetailsByPassNumber(searchbyNumber, passnumber);
        //        SecurityPassViewModel pass = null;
        //        if (pass != null && pass.status.Equals(SecurityPassStatus.Active))
        //        {
        //            try
        //            {
        //                using (Entities context = new Entities())
        //                {
        //                    ePass_Attendence attendence = new ePass_Attendence()
        //                    {
        //                        PassNo = passnumber,
        //                        Status = !string.IsNullOrWhiteSpace(radios_group1) && radios_group1.Equals("2") ? Enum.GetName(typeof(EntryStatus), EntryStatus.Checkout) : Enum.GetName(typeof(EntryStatus), EntryStatus.Checkin),
        //                        Created = DateTime.Now
        //                    };
        //                    context.ePass_Attendence.Add(attendence);
        //                    context.SaveChanges();
        //                }
        //            }
        //            catch (System.Exception ex)
        //            {
        //                LogService.Fatal(ex, this);
        //            }
        //            //change here also
        //            //byte[] profilepic = applicantphoto(pass.eFolderId);
        //            byte[] profilepic = null;
        //            if (profilepic != null)
        //            {
        //                pass.profilepic = Convert.ToBase64String(profilepic);
        //            }
        //            ViewBag.statustext = "Enty pass is valid";
        //            ViewBag.color = "green";
        //            return PartialView("~/Views/Feature/GatePass/ePass/Admin/_IndividualPass.cshtml", pass);
        //        }
        //        ViewBag.statustext = "Entry pass is invalid";
        //        ViewBag.color = "red";
        //        return PartialView("~/Views/Feature/GatePass/ePass/Admin/_IndividualPass.cshtml", pass);
        //    }
        //    return PartialView("~/Views/Feature/GatePass/ePass/Admin/_IndividualPass.cshtml");
        //}

        /// <summary>
        /// The ePassAdminLogout.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ePassAdminLogout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CheckidValidation(string emiratesid, string expirydate)
        {
            if (!string.IsNullOrWhiteSpace(emiratesid) && !string.IsNullOrWhiteSpace(expirydate))
            {
                var emiratesidrequest = new DEWAXP.Foundation.Integration.Responses.Emirates.EmiratesIDIntegrationRequest
                {
                    eiddetails = new DEWAXP.Foundation.Integration.Responses.Emirates.eiddetails { idnumber = emiratesid.Trim(), idexpirydate = FormatEpassDate(expirydate).ToString("dd.MM.yyyy") },
                    header = new DEWAXP.Foundation.Integration.Responses.Emirates.header { processid = "PICAHREPAS", username = "PICAPIHREPSQUsr", password = "kl98resrxwb26tso" }
                };

                var response = SmartCustomerClient.GetEmiratesIDdetails(emiratesidrequest, RequestLanguage, Request.Segment());
                return Json(new { status = true, result = response.Payload }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, Message = "Check the input" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The OnedayPassInitiator.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult OnedayPassInitiator()
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator) || (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) && CurrentPrincipal.HasActiveAccounts)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            return PartialView("~/Views/Feature/GatePass/ePass/Module/_OnedayPassInitiator.cshtml", new PermanentPass());
        }

        /// <summary>
        /// The OnedayPassInitiator.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OnedayPassInitiator(PermanentPass model)
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator) || (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) && CurrentPrincipal.HasActiveAccounts)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }
            Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);

            string errorMessage = string.Empty;
            if (CheckEmailAddress(model, ePassConfig, out errorMessage))
            {
                string UniquePassNumber = string.Format("{0}{1}{2}", "OP", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                //if Model is valid then post the eform
                model.PassType = "OnedayPass";
                Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.EPASS_ONEDAYVISITOR));

                string UniqueURL = HttpUtility.UrlEncode(GetEncryptedLinkExpiryURL(UniquePassNumber));
                UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always }) + "?code=" + UniqueURL;
                UniqueURL = UniqueURL.Replace(":443", "");
                //Request.Url.GetLeftPart(UriPartial.Path) + "?code=" + GetEncryptedLinkExpiryURL(UniquePassNumber);
                //PermanentPass permanentPass = null;
                //permanentPass = CheckExistingOnedayPassNumber(model.op_visitorEmailid, string.Empty, string.Empty);
                //if (permanentPass != null && string.IsNullOrWhiteSpace(permanentPass.eFolderId))
                if (model != null)
                {
                    //ServiceResponse<WebApiRestResponseEpass> response = SubmitDataOnedaypass(model, UniquePassNumber, UniqueURL);
                    Tuple<bool, string, string> result = SaveMainPass(UniquePassNumber, model, UniqueURL);
                    if (result.Item1)
                    //if (response.Succeeded && response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.eFolderId) && response.Payload.eFolderId.Length == 31)
                    {
                        model.ReferenceNumber = UniquePassNumber;
                        //string from = ePassConfig["From"] != null ? ePassConfig["From"].ToString() : "noreply@dewa.gov.ae";
                        //string subject = ePassConfig["Subject"] != null ? ePassConfig["Subject"].ToString() : string.Empty;
                        //string body = ePassConfig["Oneday invitation link"] != null ? ePassConfig["Oneday invitation link"].ToString() : string.Empty;
                        //this.EmailServiceClient.Send_Mail(from, model.op_visitorEmailid, subject, string.Format(body, HttpUtility.UrlEncode(UniqueURL)));
                        ViewBag.usertype = CurrentPrincipal.Role;
                        return PartialView("~/Views/Feature/GatePass/ePass/_OnedayPassSuccess.cshtml", model);
                    }
                    else
                    {
                        //ModelState.AddModelError(string.Empty, (response.Payload != null && response.Payload.ErrorMessage != null) ? response.Payload.ErrorMessage.ToString() : ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                        ModelState.AddModelError(string.Empty, string.IsNullOrEmpty(result.Item2) ? ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE : result.Item2);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Epass.Applied.Onedaypass"));
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            return PartialView("~/Views/Feature/GatePass/ePass/Module/_OnedayPassInitiator.cshtml", model);
        }

        /// <summary>
        /// The ReinitiateOnedaypass.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReinitiateOnedaypass(string id)
        {
            if (!IsEpassLoggedIn || !(CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) || CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator) || (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) && CurrentPrincipal.HasActiveAccounts)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINLOGIN);
            }

            List<SecurityPassViewModel> lstePassDetail = null;
            PermanentPass model = new PermanentPass();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.EPASS_DETAILS, out lstePassDetail))
                {
                    if (lstePassDetail != null && lstePassDetail.Count > 0)
                    {
                        SecurityPassViewModel ePassDetail = lstePassDetail.Where(x => !x.Subpass).FirstOrDefault();
                        //model.PassType = "Main";
                        model.PassNumber = ePassDetail.passNumber;

                        Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.EPASS_ONEDAYVISITOR));
                        string UniqueURL = HttpUtility.UrlEncode(GetEncryptedLinkExpiryURL(ePassDetail.passNumber));
                        UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always }) + "?code=" + UniqueURL;

                        Tuple<bool, string, string> isSaved = UpdateOneDayPass(model, UniqueURL, true);
                        if (isSaved.Item1)
                        {
                            goto jumpHere;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, string.IsNullOrEmpty(isSaved.Item2) ? ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE : isSaved.Item2);
                            QueryString a = new QueryString();
                            a.With("passNumber", model.PassNumber);
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_SHOWPENDINGAPPROVALDETAILS, a);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

        jumpHere:
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ADMINDASHBOARD);
        }

        /// <summary>
        /// The OnedayPassVisitor.
        /// </summary>
        /// <param name="code">The code<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult OnedayPassVisitor(string code)
        {
            ViewBag.success = false;
            PermanentPass permanentPass = new PermanentPass() { op_code = code };

            if (!string.IsNullOrWhiteSpace(code))
            {
                string passnumber = string.Empty;
                string errormessage = Translate.Text("oneday.check the URL");
                //code = HttpUtility.UrlDecode(code);
                code = code.Trim().Replace(" ", "+");

                Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.EPASS_ONEDAYVISITOR));
                Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                //string UniqueURL = HttpUtility.UrlEncode(GetEncryptedLinkExpiryURL(UniquePassNumber));
                string kofaxLinkURL = ePassConfig["Oneday Pass Visitor Link"].ToString() + "?code=" + HttpUtility.UrlEncode(code);
                kofaxLinkURL = kofaxLinkURL.Replace(":443", "");

                //string kofaxLinkURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always }) + "?code=" + HttpUtility.UrlEncode(permanentPass.op_code);
                //kofaxLinkURL = kofaxLinkURL.Replace(":443", "");
                bool isValidKofaxLink = ValidateOneDayPass(kofaxLinkURL);
                if (isValidKofaxLink == true && GetDecryptedValues(code, out passnumber, out errormessage))
                {
                    //permanentPass = CheckExistingOnedayPassNumber(string.Empty, passnumber, code);

                    BasePassDetailModel lstpassViewModel = GetPassByPassNumber(passnumber);
                    if (lstpassViewModel != null)
                    {
                        BaseMainPassDetailModel passViewModel = lstpassViewModel.MainPass.FirstOrDefault();
                        permanentPass.eFolderId = string.Empty;
                        permanentPass.op_projectName = passViewModel.ePassProjectName;
                        permanentPass.PassNumber = passViewModel.ePassPassNo;
                        permanentPass.op_projectID = passViewModel.ePassProjectID;
                        permanentPass.op_visitorEmailid = passViewModel.ePassVisitorEmailID;
                        permanentPass.op_seniormanagerEmailid = passViewModel.ePassProjectDeptApprovers;
                        //permanentPass.op_code = passViewModel??ePassLinkURL;
                        permanentPass.op_dewaemployee = string.IsNullOrEmpty(passViewModel.ePassVisitorEmailID) ? false : passViewModel.ePassVisitorEmailID.Trim().EndsWith("@dewa.gov.ae");
                        permanentPass.FullName = passViewModel.ePassVisitorName;
                        permanentPass.Mobilenumber = passViewModel.ePassMobileNumber;
                        permanentPass.CompanyName = passViewModel.ePassCompanyName;
                        permanentPass.DateOfVisit = passViewModel.ePassVisitingDate;
                        permanentPass.FromTime = passViewModel.ePassVistingTimeFrom;
                        permanentPass.ToTime = passViewModel.ePassVisitingTimeTo;

                        //permanentPass.Location = GetLocation();
                        permanentPass.OfficeLocations = EpassHelper.GetLocations(DataSources.EpassgenerationLocations);
                        ViewBag.success = true;
                        permanentPass.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
                        permanentPass.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("oneday.check the URL"));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errormessage);
                }
                if (CacheProvider.TryGet(CacheKeys.EPASS_ERROR_MESSAGE, out string errorMessage))
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    CacheProvider.Remove(CacheKeys.EPASS_ERROR_MESSAGE);
                    PermanentPass model = new PermanentPass();
                    if (CacheProvider.TryGet(CacheKeys.EPASS_ONEDAYPASS, out model))
                    {
                        permanentPass = model;
                        CacheProvider.Remove(CacheKeys.EPASS_ONEDAYPASS);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("oneday.check the URL"));
            }

            return PartialView("~/Views/Feature/GatePass/ePass/Module/_OnedayPassVisitor.cshtml", permanentPass);
        }

        /// <summary>
        /// The OnedayPassVisitor.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OnedayPassVisitor(PermanentPass model)
        {
            string qs = model.op_code;
            try
            {
                List<SecurityPassViewModel> PassModel = new List<SecurityPassViewModel>();

                string error = string.Empty;
                ImageFileUploaderResponse _uploadPhotoResponse = new ImageFileUploaderResponse();
                epasscommonmodel.SinglepassSubmit(model.SinglePass_Photo, SystemEnum.AttachmentType.Profile, "Profile", out _uploadPhotoResponse, out error);

                if (!_uploadPhotoResponse.IsSucess)
                {
                    ModelState.AddModelError("SinglePass_Photo", error);
                }
                else
                {
                    model.SinglePass_Photo_Bytes = _uploadPhotoResponse.SingelFileBytes;
                }

                if (model.SinglePass_EmiratesID != null)
                {
                    epasscommonmodel.SinglepassSubmit(model.SinglePass_EmiratesID, SystemEnum.AttachmentType.EID, "EID", out _uploadPhotoResponse, out error);

                    if (!_uploadPhotoResponse.IsSucess)
                    {
                        ModelState.AddModelError("SinglePass_EmiratesID", error);
                    }
                    else
                    {
                        model.SinglePass_EID_Bytes = _uploadPhotoResponse.SingelFileBytes;
                    }
                }
                /*if (!string.IsNullOrEmpty(model.PassportNumber) && !model.op_dewaemployee && model.SinglePass_Passport != null)
                {
                    epasscommonmodel.SinglepassSubmit(model.SinglePass_Passport, SystemEnum.AttachmentType.Passport, "Passport", out _uploadPhotoResponse, out error);

                    if (!_uploadPhotoResponse.IsSucess)
                    {
                        ModelState.AddModelError("SinglePass_Passport", error);
                    }
                    else
                    {
                        model.SinglePass_Passport_Bytes = _uploadPhotoResponse.SingelFileBytes;
                    }
                }*/
                if (!string.IsNullOrEmpty(model.VisaNumber) && !model.op_dewaemployee && model.SinglePass_Visa != null)
                {
                    epasscommonmodel.SinglepassSubmit(model.SinglePass_Visa, SystemEnum.AttachmentType.VISA, "VISA", out _uploadPhotoResponse, out error);

                    if (!_uploadPhotoResponse.IsSucess)
                    {
                        ModelState.AddModelError("SinglePass_Visa", error);
                    }
                    else
                    {
                        model.SinglePass_Visa_Bytes = _uploadPhotoResponse.SingelFileBytes;
                    }
                }
                if (!string.IsNullOrEmpty(model.op_dewantid) && model.op_dewaemployee && model.op_DEWAId != null)
                {
                    epasscommonmodel.SinglepassSubmit(model.op_DEWAId, SystemEnum.AttachmentType.DEWAID, "DEWAID", out _uploadPhotoResponse, out error);

                    if (!_uploadPhotoResponse.IsSucess)
                    {
                        ModelState.AddModelError("op_DEWAId", error);
                    }
                    else
                    {
                        model.SinglePass_DewaID_Bytes = _uploadPhotoResponse.SingelFileBytes;
                    }
                }
                if (model.withcar && model.SinglePass_DrivingLicense != null)
                {
                    epasscommonmodel.SinglepassSubmit(model.SinglePass_DrivingLicense, SystemEnum.AttachmentType.DrivingLicense, "DL", out _uploadPhotoResponse, out error);
                    if (!_uploadPhotoResponse.IsSucess)
                    {
                        ModelState.AddModelError("SinglePass_DrivingLicense", error);
                    }
                    else
                    {
                        model.SinglePass_DrivingLicense_Bytes = _uploadPhotoResponse.SingelFileBytes;
                    }
                }

                if (model.withcar && model.SinglePass_VehicleRegistration != null)
                {
                    epasscommonmodel.SinglepassSubmit(model.SinglePass_VehicleRegistration, SystemEnum.AttachmentType.Mulkiya, "VehicleRegistration", out _uploadPhotoResponse, out error);

                    if (!_uploadPhotoResponse.IsSucess)
                    {
                        ModelState.AddModelError("SinglePass_VehicleRegistration", error);
                    }
                    else
                    {
                        model.SinglePass_VehicleRegistration_Bytes = _uploadPhotoResponse.SingelFileBytes;
                    }
                }

                if (ModelState.IsValid)
                {
                    //string UniquePassNumber = model.PassNumber;
                    //PermanentPass permanentPass = CheckExistingOnedayPassNumber(model.op_visitorEmailid, model.PassNumber, model.op_code);
                    Tuple<bool, string, string> updatePass = UpdateOneDayPass(model);
                    if (updatePass.Item1 == true)
                    {
                        //Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                        //string from = ePassConfig["From"] != null ? ePassConfig["From"].ToString() : "noreply@dewa.gov.ae";
                        //string subject = ePassConfig["Subject"] != null ? ePassConfig["Subject"].ToString() : string.Empty;
                        //string body = ePassConfig["Oneday Manager approve link"] != null ? ePassConfig["Oneday Manager approve link"].ToString() : string.Empty;
                        //this.EmailServiceClient.Send_Mail(from, permanentPass.op_seniormanagerEmailid, subject, string.Format(body));
                        ViewBag.usertype = CurrentPrincipal.Role;
                        model.ReferenceNumber = model.PassNumber;
                        return PartialView("~/Views/Feature/GatePass/ePass/_OnedayPassSuccess.cshtml", model);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, string.IsNullOrEmpty(updatePass.Item2) ? ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE : updatePass.Item2);
                        CacheProvider.Store(CacheKeys.EPASS_ERROR_MESSAGE, new CacheItem<string>(updatePass.Item2));
                        CacheProvider.Store(CacheKeys.EPASS_ONEDAYPASS, new CacheItem<PermanentPass>(model));
                    }
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to process the request, Kindly resubmit the request again");
                LogService.Fatal(ex, this);
            }
            QueryString a = new QueryString();
            a.With("code", qs, true);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_ONEDAYVISITOR, a);
        }

        /// <summary>
        /// The VisitorPass.
        /// </summary>
        /// <param name="e">The e<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult VisitorPass(string e)
        {
            VisitorPassModel model = new VisitorPassModel();
            if (!string.IsNullOrWhiteSpace(e))
            {
                model.ReferenceNumber = CryptoUtility.Base64Decode(e);
            }
            model.LocationList = GetLocation(DataSources.EpassLocations);
            return PartialView("~/Views/Feature/GatePass/ePass/Module/_VisitorPass.cshtml", model);
        }

        /// <summary>
        /// The VisitorPass.
        /// </summary>
        /// <param name="model">The model<see cref="VisitorPassModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VisitorPass(VisitorPassModel model)
        {
            if (ModelState.IsValid)
            {
                model.ApplicatePhoto_attachmentData = new byte[0];
                if (model.ApplicatePhoto_attachment != null)
                {
                    model.ApplicatePhoto_attachmentName = model.ApplicatePhoto_attachment.FileName;
                    model.ApplicatePhoto_attachmentOrigSize = model.ApplicatePhoto_attachment.ContentLength;
                    model.ApplicatePhoto_attachmentData = model.ApplicatePhoto_attachment.ToArray();
                    model.ApplicatePhoto_fileExtension = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.ApplicatePhoto_filetype = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.ApplicatePhoto_contentType = model.ApplicatePhoto_attachment.ContentType;
                }

                model.EmiratesIDCard_attachmentData = new byte[0];
                if (model.EmiratesIDCard_attachment != null)
                {
                    model.EmiratesIDCard_attachmentName = model.EmiratesIDCard_attachment.FileName;
                    model.EmiratesIDCard_attachmentOrigSize = model.EmiratesIDCard_attachment.ContentLength;
                    model.EmiratesIDCard_attachmentData = model.EmiratesIDCard_attachment.ToArray();
                    model.EmiratesIDCard_fileExtension = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.EmiratesIDCard_filetype = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.EmiratesIDCard_contentType = model.ApplicatePhoto_attachment.ContentType;
                }

                model.PassportCard_attachmentData = new byte[0];
                if (model.PassportCard_attachment != null)
                {
                    model.PassportCard_attachmentName = model.PassportCard_attachment.FileName;
                    model.PassportCard_attachmentOrigSize = model.PassportCard_attachment.ContentLength;
                    model.PassportCard_attachmentData = model.PassportCard_attachment.ToArray();
                    model.PassportCard_fileExtension = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.PassportCard_filetype = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.PassportCard_contentType = model.ApplicatePhoto_attachment.ContentType;
                }

                model.VisaNumberCard_attachmentData = new byte[0];
                if (model.VisaNumberCard_attachment != null)
                {
                    model.VisaNumberCard_attachmentName = model.VisaNumberCard_attachment.FileName;
                    model.VisaNumberCard_attachmentData = model.VisaNumberCard_attachment.ToArray();
                    model.VisaNumberCard_attachmentOrigSize = model.VisaNumberCard_attachment.ContentLength;
                    model.VisaNumberCard_fileExtension = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.VisaNumberCard_filetype = model.ApplicatePhoto_attachment.GetTrimmedFileExtension();
                    model.VisaNumberCard_contentType = model.ApplicatePhoto_attachment.ContentType;
                }

                model.Flag = "Create";
                DateTime Entrydate = DateTime.Now;
                model.Entryintime = DateTime.Now.ToString(TimeFormatPattern);
                DEWAXP.Foundation.Integration.KhadamatechDEWASvc.CreateReqRequest requestVisitorPass = new DEWAXP.Foundation.Integration.KhadamatechDEWASvc.CreateReqRequest()
                {
                    VisitorEmailid = model.VisitorEmailid,
                    Visitorname = model.Visitorname,
                    Projectname = model.Projectname,
                    ProjectID = model.ProjectID,
                    Designation = model.Designation,
                    MobileNumber = model.MobileNumber,
                    Nationality = model.Nationality,
                    Location = string.Join(";", model.Location),
                    Entrydate = Entrydate,
                    Entryintime = model.Entryintime,
                    Entryouttime = model.Entryouttime,
                    EmiratesID = model.EmiratesID,
                    //EmiratesIDExpirydate = !string.IsNullOrWhiteSpace(model.EmiratesIDExpirydate) ? FormatEpassDate(model.EmiratesIDExpirydate) : DateTime.MinValue,
                    Passport = model.Passport,
                    //PassportExpirydate = !string.IsNullOrWhiteSpace(model.PassportExpirydate) ? FormatEpassDate(model.PassportExpirydate) : DateTime.MinValue,
                    VisaNumber = model.VisaNumber,
                    //VisaNumberExpirydate = !string.IsNullOrWhiteSpace(model.VisaNumberExpirydate) ? FormatEpassDate(model.VisaNumberExpirydate) : null,
                    EmiratesIDCard_attachmentName = model.EmiratesIDCard_attachmentName,
                    EID_Filecontenttype = model.EmiratesIDCard_contentType,
                    EID_Fileextension = model.EmiratesIDCard_fileExtension,
                    EID_Filetype = model.EmiratesIDCard_filetype,
                    EmiratesIDCard_attachmentData = model.EmiratesIDCard_attachmentData,
                    EmiratesIDCard_attachmentOrigSize = model.EmiratesIDCard_attachmentOrigSize,
                    PassportCard_attachmentName = model.PassportCard_attachmentName,
                    Passport_Filecontenttype = model.PassportCard_contentType,
                    Passport_Fileextension = model.PassportCard_fileExtension,
                    Passport_Filetype = model.PassportCard_filetype,
                    PassportCard_attachmentData = model.PassportCard_attachmentData,
                    PassportCard_attachmentOrigSize = model.PassportCard_attachmentOrigSize,
                    VisaNumberCard_attachmentName = model.VisaNumberCard_attachmentName,
                    VISA_Filecontenttype = model.VisaNumberCard_contentType,
                    VISA_Fileextension = model.VisaNumberCard_fileExtension,
                    VISA_Filetype = model.VisaNumberCard_filetype,
                    VisaNumberCard_attachmentData = model.VisaNumberCard_attachmentData,
                    VisaNumberCard_attachmentOrigSize = model.VisaNumberCard_attachmentOrigSize,
                    ApplicatePhoto_attachmentName = model.ApplicatePhoto_attachmentName,
                    Photo_Filecontenttype = model.ApplicatePhoto_contentType,
                    Photo_Fileextension = model.ApplicatePhoto_fileExtension,
                    Photo_Filetype = model.ApplicatePhoto_filetype,
                    ApplicatePhoto_attachmentData = model.ApplicatePhoto_attachmentData,
                    ApplicatePhoto_attachmentOrigSize = model.ApplicatePhoto_attachmentOrigSize,
                    //Entry_In =  DateTime.MinValue,
                    //Exit_Out =  DateTime.MinValue,
                    Comment = model.Comment,
                    CardNo = model.CardNo,
                    Flag = model.Flag
                };

                //dateformat
                if (!string.IsNullOrWhiteSpace(model.EmiratesIDExpirydate))
                {
                    requestVisitorPass.EmiratesIDExpirydate = FormatEpassDate(DateHelper.getCultureDate(model.EmiratesIDExpirydate).ToShortDateString()); //FormatEpassDate(model.EmiratesIDExpirydate);
                }

                if (!string.IsNullOrWhiteSpace(model.PassportExpirydate))
                {
                    requestVisitorPass.PassportExpirydate = FormatEpassDate(DateHelper.getCultureDate(model.PassportExpirydate).ToShortDateString());
                }

                if (!string.IsNullOrWhiteSpace(model.VisaNumberExpirydate))
                {
                    requestVisitorPass.VisaNumberExpirydate = FormatEpassDate(DateHelper.getCultureDate(model.VisaNumberExpirydate).ToShortDateString());
                }
                ServiceResponse<DEWAXP.Foundation.Integration.KhadamatechDEWASvc.CreateReqResponse> responseData = khadamatechDEWAServiceClient.CreateReq(requestVisitorPass);
                if (responseData != null && responseData.Succeeded
                    && responseData.Payload != null && !string.IsNullOrWhiteSpace(responseData.Payload.GatePassID))
                {
                    model.GID = responseData.Payload.GatePassID;
                    model.WID = responseData.Payload.WorkOrderID;
                    if (!string.IsNullOrWhiteSpace(model.GID))
                    {
                        QueryString p = new QueryString(true);
                        p.With("e", CryptoUtility.Base64Encode(model.GID), true);
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.Epass_VisitorPass, p);
                    }
                }
                else
                {
                    ModelState.AddModelError("", Translate.Text("VisitorCreationErrorMsg"));
                }
            }
            model.LocationList = GetLocation(DataSources.EpassLocations);
            return PartialView("~/Views/Feature/GatePass/ePass/Module/_VisitorPass.cshtml", model);
        }

        /// <summary>
        /// The TryLogin.
        /// </summary>
        /// <param name="model">The model<see cref="ePassLogin"/>.</param>
        /// <param name="responseMessage">The responseMessage<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool TryLogin(ePassLogin model, out string responseMessage)
        {
            responseMessage = string.Empty;
            ClearSessionAndSignOut();
            ServiceResponse<loginDetailsOutput> response = GatePassServiceClient.UserLogin(new logInOutDataInput
            {
                userid = model.UserName?.ToLower(),
                password = model.UserPassword
            }, RequestLanguage, Request.Segment());
            // var response = VendorServiceClient.SignInAsSupplier(model.UserName, model.UserPassword, RequestLanguage, Request.Segment());

            if (response.Succeeded && response.Payload != null && response.Payload.logindetails != null)
            {
                //CacheProvider.Store(CacheKeys.EPASS_USER_MODEL, new AccessCountingCacheItem<ePassLogin>(model, Times.Max));
                //ClearSessionAndSignOut();
                AuthStateService.Save(new DewaProfile(model.UserName?.ToLower(), response.Payload.logindetails.key, Roles.DewaSupplier)
                {
                    IsContactUpdated = true,
                    Name = response.Payload.logindetails.contactname,
                    EmailAddress = response.Payload.logindetails.contactemailid,
                    BusinessPartner = response.Payload.logindetails.supplierid  //store vendor id in Business partner field
                });
                return true;
            }
            else
            {
                responseMessage = response.Message;
                return false;
            }
        }

        /// <summary>
        /// The SendEmailToUsers.
        /// </summary>
        /// <param name="email">The <see cref="string"/>.</param>
        /// <param name="subject">The <see cref="string"/>.</param>
        /// <param name="body">The <see cref="string"/>.</param>
        /// <param name="from">The <see cref="string"/>.</param>
        /// <param name="model">The model<see cref="SecurityPassViewModel"/>.</param>
        private void SendEmailToUsers(string email, string subject, string body, string from, SecurityPassViewModel model)
        {
            try
            {
                byte[] filebytearray = ExportPdf(model);//Convert.FromBase64String(model.base64string);
                List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                attList.Add(new Tuple<string, byte[]>(model.passNumber + "-" + DateTime.Now.Ticks + ".pdf", filebytearray));

                ServiceResponse<string> response = EmailServiceClient.SendEmail(from, email, string.Empty, string.Empty, subject, body, attList);

                //this.EmailServiceClient.Send_Mail(from, email, subject, body);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
        }

        /// <summary>
        /// The SendSMSToUsers.
        /// </summary>
        /// <param name="message">The <see cref="string"/>.</param>
        /// <param name="mobile">The <see cref="string"/>.</param>
        /// <param name="applicationName">The <see cref="string"/>.</param>
        /// <param name="sender">The <see cref="string"/>.</param>
        private void SendSmsToUsers(string message, string mobile, string applicationName, string sender)
        {
            try
            {
                if (Translate.Text("lang") == "ar")
                {
                    SmsServiceClient.Send_DEWA_SMSAr(mobile, message, applicationName, sender, "1");
                }
                else
                {
                    SmsServiceClient.Send_DEWA_SMS(mobile, message, applicationName, sender, "1");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
        }

        /// <summary>
        /// The convertToEN.
        /// </summary>
        /// <param name="strDate">The strDate<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string convertToEN(string strDate)
        {
            CultureInfo culture;
            culture = Sitecorex.Context.Culture;
            if (culture.ToString().Equals("ar-AE"))
            {
                strDate = strDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }
            return strDate;
        }

        /// <summary>
        /// The AddSubContractor.
        /// </summary>
        /// <param name="model">The model<see cref="ePassSubContractor"/>.</param>
        /// <returns>The <see cref="Tuple{bool, string, string}"/>.</returns>
        private Tuple<bool, string, string> AddSubContractor(ePassSubContractor model = null)
        {
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.AddSubContractors) { Attribute = GetAddSubcontractorsAttributes(model) });
                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.AddSubContractorsURL, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    string newid = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Status") && x.Type.Equals("text")).FirstOrDefault().Value;
                    if (!string.IsNullOrWhiteSpace(newid) && newid.ToLower().Equals("failed"))
                    {
                        string message = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Message") && x.Type.Equals("text")).FirstOrDefault().Value;
                        retval = new Tuple<bool, string, string>(false, message, newid);
                    }
                    else if (!string.IsNullOrWhiteSpace(newid) && newid.ToLower().Equals("success"))
                    {
                        string message = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Message") && x.Type.Equals("text")).FirstOrDefault().Value;
                        retval = new Tuple<bool, string, string>(true, message, newid);
                    }
                }
                else
                {
                    retval = new Tuple<bool, string, string>(false, ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE, "");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this); retval = new Tuple<bool, string, string>(false, ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE, ex.Message);
            }

            return retval;
        }

        /// <summary>
        /// The GetSubContractors.
        /// </summary>
        /// <returns>The <see cref="List{ePassSubContractor}"/>.</returns>
        private List<ePassSubContractor> GetSubContractors()
        {
            List<ePassSubContractor> ePassSubContractors = new List<ePassSubContractor>();
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.GetSubContractors) { Attribute = GetSubcontractorsAttributes() });
                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.GetSubContractorsURL, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    Value json = res.Payload.Values.Where(x => x.TypeName.Equals("T000121_ePass_Subcontractors_OUT")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute atrrrequest = json?.Attribute.Where(x => x.Name.Equals("Details")).FirstOrDefault();
                    List<KofaxSubContractor> kofaxSubContractors = new List<KofaxSubContractor>();
                    if (atrrrequest != null)
                    {
                        kofaxSubContractors = JsonConvert.DeserializeObject<List<KofaxSubContractor>>(System.Text.RegularExpressions.Regex.Unescape(atrrrequest.Value.Replace(@"\""", "")));
                    }
                    if (kofaxSubContractors != null && kofaxSubContractors.Count > 0)
                    {
                        kofaxSubContractors.ForEach(x =>
                            ePassSubContractors.Add(new ePassSubContractor
                            {
                                SubcontractorID = x.SubContractID.ToString(),
                                Name = x.SubcontractName
                            }));
                    }
                }
                //else
                //{
                //    ePassSubContractors.Add(new ePassSubContractor
                //    {
                //        SubcontractorID = string.Empty,
                //        Name = ""
                //    });
                //}
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                //ePassSubContractors.Add(new ePassSubContractor
                //{
                //    SubcontractorID = string.Empty,
                //    Name = ""
                //});
            }

            return ePassSubContractors;
        }

        /// <summary>
        /// The GetAddSubcontractorsAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="ePassSubContractor"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetAddSubcontractorsAttributes(ePassSubContractor model)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "VendorID", Value = CurrentPrincipal.BusinessPartner.ToLower() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VendorName", Value = model.Vendor_Name },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmailAddress", Value = model.Email_Address },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Address", Value = model.Address },
                new Attribute { Type = EpassTypeEnum.Text, Name = "POBOX", Value = model.PO_Box },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SubcontractName", Value = model.Name },
                new Attribute { Type = EpassTypeEnum.Text, Name = "TelephoneNumber", Value = model.Telephone_Number },
                new Attribute { Type = EpassTypeEnum.Text, Name = "TradeLicenseNumber", Value = model.Trade_License_Number},
                new Attribute { Type = EpassTypeEnum.Text, Name = "TradeLicenseIssueDate", Value = FormatKofaxDate(model.Trade_License_Issue_Date)},
                new Attribute { Type = EpassTypeEnum.Text, Name = "TradeLicenseExpiryDate", Value = FormatKofaxDate(model.Trade_License_Expiry_Date)},
            };
        }

        /// <summary>
        /// The GetSubcontractorsAttributes.
        /// </summary>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetSubcontractorsAttributes()
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "VendorID", Value = CurrentPrincipal.BusinessPartner.ToLower() },
            };
        }

        /// <summary>
        /// Return a string of random hexadecimal values which is 6 characters long and relatively unique.
        /// </summary>
        /// <returns>.</returns>
        private string GetPassNumber()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return al.GetRandomUppercaseAlphaNumericValue(8);
            }
        }

        /// <summary>
        /// The GetEncryptedLinkExpiryURL.
        /// </summary>
        /// <param name="passnumber">The passnumber<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string GetEncryptedLinkExpiryURL(string passnumber)
        {
            string passphrase = "EPASSOneday";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
            {
                Key = TDESKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros
            };
            byte[] DataToEncrypt = UTF8.GetBytes(string.Concat(passnumber, "|", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            string newlinkurl = Convert.ToBase64String(Results);
            return newlinkurl;
        }

        /// <summary>
        /// The GetDecryptedValues.
        /// </summary>
        /// <param name="url">The url<see cref="string"/>.</param>
        /// <param name="passnumber">The passnumber<see cref="string"/>.</param>
        /// <param name="errormessage">The errormessage<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool GetDecryptedValues(string url, out string passnumber, out string errormessage)
        {
            bool valid = false;
            passnumber = string.Empty;
            errormessage = Translate.Text("oneday.check the URL");
            try
            {
                string passphrase = "EPASSOneday";
                byte[] Results;
                UTF8Encoding UTF8 = new UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.Zeros
                };
                byte[] DataToDecrypt = Convert.FromBase64String(url);
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                string resulttext = UTF8.GetString(Results);
                string[] result = resulttext.Split('|');
                if (result.Length > 1 && !string.IsNullOrWhiteSpace(result[1]))
                {
                    if (DateTime.TryParse(result[1], out DateTime parsetime))
                    {
                        if (DateTime.Now.CompareTo(parsetime.AddDays(3)) < 0)
                        {
                            valid = true;
                            passnumber = result[0];
                        }
                        else
                        {
                            errormessage = Translate.Text("Link has been expired");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return valid;
        }

        /// <summary>
        /// The GetPermanentPass.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <returns>The <see cref="PermanentPass"/>.</returns>
        private PermanentPass GetPermanentPass(string passNumber)
        {
            SecurityPassViewModel model = null;
            List<SecurityPassViewModel> lstmodel = new List<SecurityPassViewModel>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MYPASS_LIST, out lstmodel))
            {
                model = lstmodel.Where(x => x.passNumber == passNumber).FirstOrDefault();
            }
            PermanentPass passDetails = new PermanentPass();
            if (model != null)
            {
                passDetails = new PermanentPass
                {
                    eFolderId = model.eFolderId,
                    FullName = model.name,
                    Nationality = model.nationality,
                    ProfessionLevel = model.profession,
                    EmiratesID = model.emiratesId,
                    EmiratesIDExpiry = model.emiratesExpiryDate != null ? model.emiratesExpiryDate.ToString() : string.Empty,
                    VisaNumber = model.visaNumber,
                    VisaExpiry = model.visaExpiryDate != null ? model.visaExpiryDate.ToString() : string.Empty,
                    PassportNumber = model.passportNumber,
                    PassportExpiry = model.passportExpiryDate != null ? model.passportExpiryDate.ToString() : string.Empty,
                    PassNumber = model.passNumber,
                    PassType = model.passType,
                    PassExpiry = model.passExpiryDate != null ? model.passExpiryDate.ToString() : string.Empty,
                    Mobilenumber = model.mobile,
                    Emailaddress = model.email,
                    SubContractorID = model.Subcontractor,
                    FromTime = model.fromTime,
                    ToTime = model.toTime,
                    PassIssue = model.passIssueDate != null ? model.passIssueDate.ToString() : string.Empty,
                    CompanyName = model.companyName,
                    POName = model.projectName,
                    PONumber = model.projectId,
                    projectStartDate = model.projectStartDate,
                    projectEndDate = model.projectEndDate,
                    projectId = model.projectId,
                    projectStatus = model.projectStatus,
                    departmentName = model.departmentName,
                    SelectedLocation = model.Location != null ? model.Location.Select(s => s.ToString().Trim()) : null
                };
            }
            return passDetails;
        }

        /// <summary>
        /// The GetBlockedUsers.
        /// </summary>
        /// <returns>The <see cref="List{SecurityBlockedUserViewModel}"/>.</returns>
        private List<SecurityBlockedUserViewModel> GetBlockedUsers()
        {
            List<SecurityBlockedUserViewModel> LstBlockedUsers = new List<SecurityBlockedUserViewModel>();
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.GetBlockeduserURL, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    Value json = res.Payload.Values.Where(x => x.TypeName.Equals("T000121_ePass_BlockedUser_OUT")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute atrrrequest = json?.Attribute.Where(x => x.Name.Equals("Details")).FirstOrDefault();
                    List<KofaxePassBlockedUsers> kofaxSubContractors = new List<KofaxePassBlockedUsers>();
                    if (atrrrequest != null)
                    {
                        kofaxSubContractors = JsonConvert.DeserializeObject<List<KofaxePassBlockedUsers>>(System.Text.RegularExpressions.Regex.Unescape(atrrrequest.Value));
                    }
                    if (kofaxSubContractors != null && kofaxSubContractors.Count > 0)
                    {
                        kofaxSubContractors.ForEach(x =>
                            LstBlockedUsers.Add(new SecurityBlockedUserViewModel
                            {
                                eFolderId = x.ID.ToString(),
                                emiratesID = x.EmiratesID,
                                isBlocked = x.Status,
                                name = x.UserName,
                                passNo = x.PassNo,
                                passportNumber = x.PassportNo,
                                status = x.Status,
                                visaNumber = x.VisaNo,
                            }));
                        CacheProvider.Store(CacheKeys.EPASS_BLOCKEDUSERS, new CacheItem<List<SecurityBlockedUserViewModel>>(LstBlockedUsers, TimeSpan.FromMinutes(40)));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this); retval = new Tuple<bool, string, string>(false, ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE, ex.Message);
            }

            return LstBlockedUsers;
        }

        /// <summary>
        /// The GetListofPasses.
        /// </summary>
        /// <param name="dptmgr">The dptmgr<see cref="string"/>.</param>
        /// <returns>The <see cref="List{SecurityPassViewModel}"/>.</returns>
        private List<SecurityPassViewModel> GetListofPasses(string dptmgr = "")
        {
            List<SecurityPassViewModel> LstPassItems = new List<SecurityPassViewModel>();
            List<BasePassViewModel> allMainPasses = new List<BasePassViewModel>();
            allMainPasses = GetMainPasses(dptmgr);
            IEnumerable<IGrouping<string, BasePassViewModel>> grpAllpass = allMainPasses.GroupBy(x => x.ePassPassNo);
            grpAllpass.ToList().ForEach(x =>
            {
                IEnumerable<BasePassViewModel> lstpasses = allMainPasses.Where(y => y.ePassPassNo.Equals(x.Key));
                if (lstpasses.Any())
                {
                    BasePassViewModel p = lstpasses.FirstOrDefault();
                    SecurityPassStatus astatus = assignStatus(p.ePassPassStatus, string.IsNullOrEmpty(p.ePassPassExpiryDate) ? p.ePassVisitingDate : p.ePassPassExpiryDate, (p.ePassIsBlocked == "Yes" ? true : false));
                    LstPassItems.Add(new SecurityPassViewModel()
                    {
                        wppass = false,
                        Subpass = false,
                        name = p.ePassVisitorName,
                        passNumber = p.ePassPassNo,
                        mainpassNumber = x.Key,
                        passType = Translate.Text(p.ePassPassType),
                        passTypeText = p.ePassPassType,
                        profession = p.ePassProfession,
                        Designation = p.ePassDesignation,
                        passExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassPassExpiryDate)) ? FormatEpassDate(p.ePassPassExpiryDate) :
                                     (p.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(p.ePassVisitingDate) ? FormatEpassDate(p.ePassVisitingDate) : default(DateTime?)),
                        passIssueDate = (!string.IsNullOrWhiteSpace(p.ePassPassIssueDate)) ? FormatEpassDate(p.ePassPassIssueDate) :
                    (p.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(p.ePassVisitingDate) ? FormatEpassDate(p.ePassVisitingDate) : default(DateTime?)),
                        CreatedDate = (!string.IsNullOrWhiteSpace(p.ePassCreatedOn)) ? FormatEpassDate(p.ePassCreatedOn) : default(DateTime?),
                        strpassExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassPassExpiryDate)) ? FormatEpassstrDate(p.ePassPassExpiryDate) :
                          (p.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(p.ePassVisitingDate) ? FormatEpassstrDate(p.ePassVisitingDate) : string.Empty),
                        status = astatus,
                        strstatus = Translate.Text("epassstatus." + astatus.ToString().ToLower()),
                        strclass = astatus.ToString(),
                        nationality = p.ePassNationality,
                        emiratesId = p.ePassEmiratesID,
                        emiratesExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassEmiratesiDExpiry)) ? FormatEpassDate(p.ePassEmiratesiDExpiry) : default(DateTime?),
                        visaNumber = p.ePassVisaNumber,
                        visaExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassVisaExpiryDate)) ? FormatEpassDate(p.ePassVisaExpiryDate) : default(DateTime?),
                        emailLimit = (string.IsNullOrWhiteSpace(p.ePassEmailLimit)) ? 0 : Convert.ToInt16(p.ePassEmailLimit),
                        smsLimit = (string.IsNullOrWhiteSpace(p.ePassSMSLimit)) ? 0 : Convert.ToInt16(p.ePassSMSLimit),
                        downloadLimit = (string.IsNullOrWhiteSpace(p.ePassDownloadLimit)) ? 0 : Convert.ToInt16(p.ePassDownloadLimit),
                        passportNumber = p.ePassPassportNumber,
                        passportExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassPassportExpiryDate)) ? FormatEpassDate(p.ePassPassportExpiryDate) : default(DateTime?),
                        fromTime = p.ePassFromDateTime,
                        toTime = p.ePassToDateTime,
                        mobile = p.ePassMobileNumber,
                        email = p.ePassEmailAddress,
                        VisitorEmail = p.ePassVisitorEmailID,
                        SeniorManagerEmail = p.ePassVisitorEmailID,
                        //LinkURL = p.ePassVi,
                        //linkExpiryDate = p.ePassVi,
                        Location = lstselectedlocations(p.ePassLocation),
                        //RejectRemarks = p.epa,
                        Subcontractor = p.ePassSubContractorID,
                        //passAttachements = p.ep
                        companyName = p.ePassCompanyName,
                        projectName = p.ePassProjectName,
                        projectStartDate = (!string.IsNullOrWhiteSpace(p.ePassProjectStartName)) ? FormatEpassDate(p.ePassProjectStartName) : default(DateTime?),
                        projectEndDate = (!string.IsNullOrWhiteSpace(p.ePassProjectEndDate)) ? FormatEpassDate(p.ePassProjectEndDate) : default(DateTime?),
                        projectId = p.ePassProjectID,
                        projectStatus = p.ePassProjectStatus,
                        departmentName = p.ePassDepartmentName,
                        IsBlocked = (p.ePassIsBlocked == "Yes" ? true : false),
                        DEWAID = p.ePassDEWAID,
                        DEWAdesignation = p.ePassDesignation,
                        VehicleRegNumber = p.ePassVehicleNo,
                        VehRegistrationDate = (!string.IsNullOrWhiteSpace(p.ePassVehicleRegDate)) ? FormatEpassDate(p.ePassVehicleRegDate) : default(DateTime?),
                        strpassVehicleRegDate = !string.IsNullOrWhiteSpace(p.ePassVehicleRegDate) ? FormatEpassDate(p.ePassVehicleRegDate).ToString("MMMM dd, yyyy") : string.Empty,
                        pendingwith = (p.ePassSecurityApprovers != null && p.ePassPassStatus != null && !string.IsNullOrWhiteSpace(p.ePassSecurityApprovers.ToString()) && !string.IsNullOrWhiteSpace(p.ePassPassStatus.ToString())) ? (p.ePassPassStatus.ToString().ToLower().Equals("Dept Coordinator".ToLower()) ? p.ePassSecurityApprovers.ToString() : (p.ePassPassStatus.ToString().ToLower().Equals("Security Team".ToLower()) ? Translate.Text("Epass.SecurityAdmin") : string.Empty)) : string.Empty
                    });
                }
            });
            List<SecurityPassViewModel> LstWPPassItems = new List<SecurityPassViewModel>();
            GroupPassPemitResponse lstpassess = GetWorkpermitPasses();
            if (lstpassess != null && lstpassess.Groupworkpermitpassbothlist != null)
            {
                LstWPPassItems = lstpassess.Groupworkpermitpassbothlist.Where(y => !string.IsNullOrWhiteSpace(y.Passexpirydate) && !string.IsNullOrWhiteSpace(y.Passissuedate) && !y.Passexpirydate.Equals("0000-00-00") && !y.Passissuedate.Equals("0000-00-00")).Select(x => new SecurityPassViewModel
                {
                    eFolderId = string.Empty,
                    name = x.Fullname,
                    passNumber = x.Permitpass,
                    mainpassNumber = x.Permitpass,
                    grouppassid = x.Grouppassid,
                    wppass = true,
                    grouppass = !string.IsNullOrWhiteSpace(x.Grouppassid) && x.Grouppassid.StartsWith("GP") ? true : false,
                    passType = Translate.Text("Work Permit"),
                    profession = x.Profession,
                    CreatedDate = !string.IsNullOrWhiteSpace(x.createddate) && !x.createddate.Equals("0000-00-00") ? DateTime.Parse(x.createddate) : (DateTime?)null,
                    ChangedDate = !string.IsNullOrWhiteSpace(x.createddate) && !x.createddate.Equals("0000-00-00") ? DateTime.Parse(x.createddate) : (DateTime?)null,
                    passExpiryDate = !string.IsNullOrWhiteSpace(x.Passexpirydate) && !x.Passexpirydate.Equals("0000-00-00") ? DateTime.Parse(x.Passexpirydate) : (DateTime?)null,
                    passIssueDate = !string.IsNullOrWhiteSpace(x.Passissuedate) && !x.Passissuedate.Equals("0000-00-00") ? DateTime.Parse(x.Passissuedate) : (DateTime?)null,
                    strpassExpiryDate = !string.IsNullOrWhiteSpace(x.Passexpirydate) && !x.Passexpirydate.Equals("0000-00-00") ? FormatEpassstrDate(DateTime.Parse(x.Passexpirydate).ToString("MMMM dd, yyyy")) : string.Empty,
                    status = assignWPStatus(lstpassess.Grouppasslocationreturnlist, x.Grouppassid, x.Passexpirydate),
                    strstatus = Translate.Text("epassstatus." + assignWPStatus(lstpassess.Grouppasslocationreturnlist, x.Grouppassid, x.Passexpirydate).ToString().ToLower()),
                    strclass = assignWPStatus(lstpassess.Grouppasslocationreturnlist, x.Grouppassid, x.Passexpirydate).ToString(),
                    nationality = x.Countrykey,
                    emiratesId = x.Emiratesid,
                    emiratesExpiryDate = !string.IsNullOrWhiteSpace(x.Emiratesidenddate) && !x.Emiratesidenddate.Equals("0000-00-00") ? DateTime.Parse(x.Emiratesidenddate) : (DateTime?)null,
                    visaNumber = x.Visanumber,
                    visaExpiryDate = !string.IsNullOrWhiteSpace(x.Visaendate) && !x.Visaendate.Equals("0000-00-00") ? DateTime.Parse(x.Visaendate) : (DateTime?)null,
                    emailLimit = 0,
                    smsLimit = 0,
                    downloadLimit = 0,
                    passportNumber = x.Passportnumber,
                    passportExpiryDate = !string.IsNullOrWhiteSpace(x.Passportenddate) && !x.Passportenddate.Equals("0000-00-00") ? DateTime.Parse(x.Passportenddate) : (DateTime?)null,
                    fromTime = x.Fromtime,
                    toTime = x.Totime,
                    mobile = x.Mobile.AddMobileNumberZeroPrefix(),
                    email = x.Emailid,
                    VisitorEmail = x.Emailid,
                    Location = new List<string>(),
                    RejectRemarks = x.Remarks,
                    Subcontractor = string.Empty,
                    passAttachements = new List<SecurityPassAttachement>(),
                    companyName = x.Companyname,
                    projectName = x.Projectname,
                    projectStartDate = null,
                    projectEndDate = null,
                    projectId = x.Ponumber,
                    projectStatus = string.Empty,
                    departmentName = string.Empty,
                    IsBlocked = false,
                    DEWAID = string.Empty,
                    DEWAdesignation = string.Empty,
                    VehicleRegNumber = string.Empty,
                    strpassVehicleRegDate = string.Empty,
                    pendingwith = string.Empty,
                    wpprojectcoordinatorname = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                    lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Fullname : string.Empty,
                    wpprojectcoordinatoremail = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                    lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Emailid : string.Empty,
                    wpprojectcoordinatormobile = lstpassess.Projectcoordinatorlist != null && lstpassess.Projectcoordinatorlist.Count() > 0 && lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).Any() ?
                                    lstpassess.Projectcoordinatorlist.Where(y => y.Grouppassid.Equals(x.Grouppassid)).FirstOrDefault().Mobile : string.Empty,
                }).ToList();
            }
            LstPassItems.AddRange(LstWPPassItems);
            //subpasses
            //AppendSubPasses(LstPassItems);
            GetSubPasses(LstPassItems, allMainPasses);
            LstPassItems = LstPassItems.OrderByDescending(x => x.CreatedDate).ToList();
            CacheProvider.Store(CacheKeys.EPASS_MYPASS_LIST, new CacheItem<List<SecurityPassViewModel>>(LstPassItems, TimeSpan.FromMinutes(40)));
            return LstPassItems;
        }

        /// <summary>
        /// The Loggeduserpasses.
        /// </summary>
        /// <param name="emailaddress">The emailaddress<see cref="string"/>.</param>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The <see cref="List{SecurityPassViewModel}"/>.</returns>
        private List<SecurityPassViewModel> Loggeduserpasses(string emailaddress, string username)
        {
            return GetListofPasses(emailaddress);
        }

        /// <summary>
        /// The ExistingCheckUser.
        /// </summary>
        /// <param name="passmodel">The passmodel<see cref="PermanentPass"/>.</param>
        /// <param name="lstcsvformat">The lstcsvformat<see cref="List{CSVfileformat}"/>.</param>
        /// <returns>The <see cref="List{SecurityPassViewModel}"/>.</returns>
        private List<SecurityPassViewModel> ExistingCheckUser(PermanentPass passmodel, List<CSVfileformat> lstcsvformat)
        {
            List<SecurityPassViewModel> LstPassItems = new List<SecurityPassViewModel>();
            try
            {
                string lstemirates = string.Empty;
                string lstvisa = string.Empty;
                string lstpassport = string.Empty;
                if (passmodel != null && !string.IsNullOrWhiteSpace(passmodel.EmiratesID) && !string.IsNullOrWhiteSpace(passmodel.VisaNumber) && !string.IsNullOrWhiteSpace(passmodel.PassportNumber))
                {
                    lstemirates = passmodel.EmiratesID.ToLower();
                    lstvisa = passmodel.VisaNumber.ToLower();
                    lstpassport = passmodel.PassportNumber.ToLower();
                }
                else if (lstcsvformat != null)
                {
                    lstemirates = lstcsvformat.Where(x => !string.IsNullOrWhiteSpace(x.EmiratesID)).Select(i => i.EmiratesID.ToLower()).Aggregate((i, j) => i + "," + j);
                    lstvisa = lstcsvformat.Where(x => !string.IsNullOrWhiteSpace(x.Visanumber)).Select(i => i.Visanumber.ToLower()).Aggregate((i, j) => i + "," + j);
                    lstpassport = lstcsvformat.Where(x => !string.IsNullOrWhiteSpace(x.Passportnumber)).Select(i => i.Passportnumber.ToLower()).Aggregate((i, j) => i + "," + j);
                }
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.CheckExistingUser) { Attribute = CheckexistinguserAttributes(lstemirates, lstvisa, lstpassport) });
                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CheckExistingUserURL, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    Value json = res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute att = json?.Attribute.Where(x => x.Name.Equals("Requests")).FirstOrDefault();
                    List<BasePassViewModel> subPassViewModels = new List<BasePassViewModel>();
                    if (att != null)
                    {
                        subPassViewModels = JsonConvert.DeserializeObject<List<BasePassViewModel>>(System.Text.RegularExpressions.Regex.Unescape(att.Value));
                    }
                    if (subPassViewModels != null && subPassViewModels.Count > 0)
                    {
                        IEnumerable<IGrouping<string, BasePassViewModel>> grpAllpass = subPassViewModels.GroupBy(x => x.ePassPassNo);
                        grpAllpass.ToList().ForEach(x =>
                        {
                            IEnumerable<BasePassViewModel> lstpasses = subPassViewModels.Where(y => y.ePassPassNo.Equals(x.Key));
                            if (lstpasses.Any())
                            {
                                BasePassViewModel p = lstpasses.FirstOrDefault();
                                SecurityPassStatus astatus = assignStatus(p.ePassPassStatus, string.IsNullOrEmpty(p.ePassPassExpiryDate) ? p.ePassVisitingDate : p.ePassPassExpiryDate, (p.ePassIsBlocked == "Yes" ? true : false));
                                LstPassItems.Add(new SecurityPassViewModel()
                                {
                                    wppass = false,
                                    Subpass = false,
                                    name = p.ePassVisitorName,
                                    passNumber = p.ePassPassNo,
                                    mainpassNumber = x.Key,
                                    passType = p.ePassPassType,
                                    passTypeText = p.ePassPassType,
                                    profession = p.ePassProfession,
                                    Designation = p.ePassDesignation,
                                    passExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassPassExpiryDate)) ? FormatEpassDate(p.ePassPassExpiryDate) :
                                     (p.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(p.ePassVisitingDate) ? FormatEpassDate(p.ePassVisitingDate) : default(DateTime?)),
                                    passIssueDate = (!string.IsNullOrWhiteSpace(p.ePassPassIssueDate)) ? FormatEpassDate(p.ePassPassIssueDate) :
                    (p.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(p.ePassVisitingDate) ? FormatEpassDate(p.ePassVisitingDate) : default(DateTime?)),
                                    CreatedDate = (!string.IsNullOrWhiteSpace(p.ePassCreatedOn)) ? FormatEpassDate(p.ePassCreatedOn) : default(DateTime?),
                                    strpassExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassPassExpiryDate)) ? FormatEpassDate(p.ePassPassExpiryDate).ToString("MMMM dd, yyyy") :
                          (p.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(p.ePassVisitingDate) ? FormatEpassDate(p.ePassVisitingDate).ToString("MMMM dd, yyyy") : string.Empty),
                                    status = astatus,
                                    strstatus = Translate.Text("epassstatus." + astatus.ToString().ToLower()),
                                    strclass = astatus.ToString(),
                                    nationality = p.ePassNationality,
                                    emiratesId = p.ePassEmiratesID,
                                    emiratesExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassEmiratesiDExpiry)) ? FormatEpassDate(p.ePassEmiratesiDExpiry) : default(DateTime?),
                                    visaNumber = p.ePassVisaNumber,
                                    visaExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassVisaExpiryDate)) ? FormatEpassDate(p.ePassVisaExpiryDate) : default(DateTime?),
                                    emailLimit = (string.IsNullOrWhiteSpace(p.ePassEmailLimit)) ? 0 : Convert.ToInt16(p.ePassEmailLimit),
                                    smsLimit = (string.IsNullOrWhiteSpace(p.ePassSMSLimit)) ? 0 : Convert.ToInt16(p.ePassSMSLimit),
                                    downloadLimit = (string.IsNullOrWhiteSpace(p.ePassDownloadLimit)) ? 0 : Convert.ToInt16(p.ePassDownloadLimit),
                                    passportNumber = p.ePassPassportNumber,
                                    passportExpiryDate = (!string.IsNullOrWhiteSpace(p.ePassPassportExpiryDate)) ? FormatEpassDate(p.ePassPassportExpiryDate) : default(DateTime?),
                                    fromTime = p.ePassFromDateTime,
                                    toTime = p.ePassToDateTime,
                                    mobile = p.ePassMobileNumber,
                                    email = p.ePassEmailAddress,
                                    VisitorEmail = p.ePassVisitorEmailID,
                                    SeniorManagerEmail = p.ePassVisitorEmailID,
                                    //LinkURL = p.ePassVi,
                                    //linkExpiryDate = p.ePassVi,
                                    Location = lstselectedlocations(p.ePassLocation),
                                    //RejectRemarks = p.epa,
                                    Subcontractor = p.ePassSubContractorID,
                                    //passAttachements = p.ep
                                    companyName = p.ePassCompanyName,
                                    projectName = p.ePassProjectName,
                                    projectStartDate = (!string.IsNullOrWhiteSpace(p.ePassProjectStartName)) ? FormatEpassDate(p.ePassProjectStartName) : default(DateTime?),
                                    projectEndDate = (!string.IsNullOrWhiteSpace(p.ePassProjectEndDate)) ? FormatEpassDate(p.ePassProjectEndDate) : default(DateTime?),
                                    projectId = p.ePassProjectID,
                                    projectStatus = p.ePassProjectStatus,
                                    departmentName = p.ePassDepartmentName,
                                    IsBlocked = (p.ePassIsBlocked == "Yes" ? true : false),
                                    DEWAID = p.ePassDEWAID,
                                    DEWAdesignation = p.ePassDesignation,
                                    VehicleRegNumber = p.ePassVehicleNo,
                                    VehRegistrationDate = (!string.IsNullOrWhiteSpace(p.ePassVehicleRegDate)) ? FormatEpassDate(p.ePassVehicleRegDate) : default(DateTime?),
                                    strpassVehicleRegDate = !string.IsNullOrWhiteSpace(p.ePassVehicleRegDate) ? FormatEpassDate(p.ePassVehicleRegDate).ToString("MMMM dd, yyyy") : string.Empty,
                                    pendingwith = (p.ePassSecurityApprovers != null && p.ePassPassStatus != null && !string.IsNullOrWhiteSpace(p.ePassSecurityApprovers.ToString()) && !string.IsNullOrWhiteSpace(p.ePassPassStatus.ToString())) ? (p.ePassPassStatus.ToString().ToLower().Equals("Dept Coordinator".ToLower()) ? p.ePassSecurityApprovers.ToString() : (p.ePassPassStatus.ToString().ToLower().Equals("Security Team".ToLower()) ? Translate.Text("Epass.SecurityAdmin") : string.Empty)) : string.Empty
                                });
                            }
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return LstPassItems;
        }

        /// <summary>
        /// The CheckexistinguserAttributes.
        /// </summary>
        /// <param name="lstemirates">The lstemirates<see cref="string"/>.</param>
        /// <param name="lstvisa">The lstvisa<see cref="string"/>.</param>
        /// <param name="lstpassport">The lstpassport<see cref="string"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> CheckexistinguserAttributes(string lstemirates, string lstvisa, string lstpassport)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesID", Value = lstemirates },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisaNo", Value = lstvisa },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassportNo", Value = lstpassport },
            };
        }

        /// <summary>
        /// The assignStatus.
        /// </summary>
        /// <param name="dystatus">The dystatus<see cref="dynamic"/>.</param>
        /// <param name="dypassexpirydate">The dypassexpirydate<see cref="dynamic"/>.</param>
        /// <param name="isblocked">The isblocked<see cref="dynamic"/>.</param>
        /// <returns>The <see cref="SecurityPassStatus"/>.</returns>
        private SecurityPassStatus assignStatus(dynamic dystatus, dynamic dypassexpirydate, dynamic isblocked)
        {
            SecurityPassStatus assingedstatus = SecurityPassStatus.Notapplicable;
            string status = dystatus.ToString(); string passexpirydate = dypassexpirydate != null ? dypassexpirydate.ToString() : string.Empty;
            bool isblockedpass = (isblocked == null) ? false : (bool)isblocked;
            if (isblockedpass)
            {
                assingedstatus = SecurityPassStatus.Blocked;
            }
            else if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.ToLower().Equals("canceled") || status.ToLower().Equals("cancelled"))
                {
                    assingedstatus = SecurityPassStatus.Cancelled;
                }
                else if (status.ToLower().Equals("originator - resubmit") || status.ToLower().Equals("rejected") || status.ToLower().Equals("security rejected") || status.ToLower().Equals("dept rejected"))
                {
                    assingedstatus = SecurityPassStatus.Rejected;
                }
                else if (status.ToLower().Equals("completed") || status.ToLower().Equals("security approved"))
                {
                    DateTime expirydate = new DateTime();
                    if (!string.IsNullOrWhiteSpace(passexpirydate))
                    {
                        if (DateTime.TryParse(passexpirydate, out DateTime dateResult))
                        {
                            expirydate = dateResult;
                        }
                    }
                    if (expirydate.Ticks > 0)
                    {
                        if (expirydate.Date < DateTime.Now.Date)
                        {
                            assingedstatus = SecurityPassStatus.Expired;
                        }
                        else if (expirydate.Date < DateTime.Now.Date.AddDays(14))
                        {
                            assingedstatus = SecurityPassStatus.SoontoExpire;
                        }
                        else
                        {
                            assingedstatus = SecurityPassStatus.Active;
                        }
                    }
                }
                else if (status.ToLower().Equals("Security Approved".ToLower()))
                {
                    assingedstatus = SecurityPassStatus.Active;
                }
                else if (status.ToLower().Equals("Dept Approved".ToLower()))
                {
                    assingedstatus = SecurityPassStatus.PendingApprovalwithSecurity;
                }
                else if (status.ToLower().Equals("Dept Coordinator".ToLower()) || status.ToLower().Equals("Initiated".ToLower()))
                {
                    assingedstatus = SecurityPassStatus.PendingApprovalwithCoordinator;
                }
                else if (status.ToLower().Equals("Security Team".ToLower()))
                {
                    assingedstatus = SecurityPassStatus.PendingApprovalwithSecurity;
                }
                else if (status.ToLower().Equals("Sr Manager".ToLower()))
                {
                    assingedstatus = SecurityPassStatus.PendingApprovalwithCoordinator;
                }
                else if (status.ToLower().Equals("Admin Security".ToLower()))
                {
                    assingedstatus = SecurityPassStatus.PendingApprovalwithSecurity;
                }
                //else if (status.ToLower().Equals("Initiated".ToLower()))
                //{
                //    assingedstatus = SecurityPassStatus.Initiated;
                //}
                else
                {
                    assingedstatus = SecurityPassStatus.Notapplicable;
                }
            }

            if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin))
            {
                if ((assingedstatus == SecurityPassStatus.Active || assingedstatus == SecurityPassStatus.SoontoExpire) || assingedstatus == SecurityPassStatus.PendingApprovalwithSecurity)
                {
                    assingedstatus = SecurityPassStatus.Approved;
                }
            }
            else if (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity))
            {
                if (assingedstatus == SecurityPassStatus.Active || assingedstatus == SecurityPassStatus.SoontoExpire)
                {
                    assingedstatus = SecurityPassStatus.Approved;
                }
            }

            return assingedstatus;
        }

        /// <summary>
        /// The ExportPdf.
        /// </summary>
        /// <param name="model">The model<see cref="SecurityPassViewModel"/>.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public byte[] ExportPdf(SecurityPassViewModel model)
        {
            //string byteString = string.Empty;
            //if (model.passAttachements != null)
            //{
            //    SecurityPassAttachement strBytes = model.passAttachements.Where(x => x.fileCategory.Equals("Photo")).FirstOrDefault();
            //    if (strBytes != null)
            //    {
            //        byteString = model.passAttachements.Where(x => x.fileCategory.Equals("Photo")).FirstOrDefault().fileContent;
            //    }
            //}
            //byte[] byteArrayIn = byteString != null ? Convert.FromBase64String(byteString) : null; //applicantphoto(model.eFolderId);

            //string imgPath = Server.MapPath("/images/epassdownload.png");
            //string html = "<img src='" + imgPath + "' />"; //File.ReadAllText(Server.MapPath("/html/index2.html"));

            //StringReader sr = new StringReader("");
            //Document pdfDoc = new Document(PageSize.A6.Rotate());

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);

            //    writer.StrictImageSequence = true;

            //    pdfDoc.Open();

            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);

            //    PdfContentByte cb = writer.DirectContent;
            //    BaseFont bf = BaseFont.CreateFont(Server.MapPath("~/content/fonts/ARIALUNI.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //    Font font = new Font(bf, 10);
            //    //creating the table
            //    PdfPTable table = new PdfPTable(1);
            //    table.DefaultCell.NoWrap = false;
            //    table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            //    table.WidthPercentage = 100;

            //    table = new PdfPTable(1);
            //    PdfPCell text = new PdfPCell(new Phrase(model.name, font))
            //    {
            //        BorderWidth = 0,
            //        Padding = 0
            //    };
            //    table.AddCell(text);
            //    Rectangle rect = new Rectangle(60, 143, 395, 122);
            //    cb.Fill();
            //    ColumnText ct = new ColumnText(cb);
            //    ct.SetSimpleColumn(rect);
            //    ct.AddElement(table);
            //    ct.Go();

            //    table = new PdfPTable(1);
            //    text = new PdfPCell(new Phrase(model.passNumber, font))
            //    {
            //        BorderWidth = 0,
            //        Padding = 0
            //    };
            //    table.AddCell(text);
            //    rect = new Rectangle(60, 167, 395, 146);
            //    cb.Fill();
            //    ct = new ColumnText(cb);
            //    ct.SetSimpleColumn(rect);
            //    ct.AddElement(table);
            //    ct.Go();

            //    table = new PdfPTable(1);
            //    text = new PdfPCell(new Phrase(model.profession, font))
            //    {
            //        BorderWidth = 0,
            //        Padding = 0
            //    };
            //    table.AddCell(text);
            //    rect = new Rectangle(60, 119, 395, 99);
            //    cb.Fill();
            //    ct = new ColumnText(cb);
            //    ct.SetSimpleColumn(rect);
            //    ct.AddElement(table);
            //    ct.Go();

            //    table = new PdfPTable(1);
            //    text = new PdfPCell(new Phrase(model.companyName, font))
            //    {
            //        BorderWidth = 0,
            //        Padding = 0
            //    };
            //    table.AddCell(text);
            //    rect = new Rectangle(60, 95, 395, 74);
            //    cb.Fill();
            //    ct = new ColumnText(cb);
            //    ct.SetSimpleColumn(rect);
            //    ct.AddElement(table);
            //    ct.Go();

            //    table = new PdfPTable(1);
            //    text = new PdfPCell(new Phrase(model.projectName, font))
            //    {
            //        BorderWidth = 0,
            //        Padding = 0
            //    };
            //    table.AddCell(text);
            //    rect = new Rectangle(60, 71, 395, 50);
            //    cb.Fill();
            //    ct = new ColumnText(cb);
            //    ct.SetSimpleColumn(rect);
            //    ct.AddElement(table);
            //    ct.Go();

            //    if (model.passExpiryDate != null && model.passExpiryDate.Value != null)
            //    {
            //        table = new PdfPTable(1);
            //        text = new PdfPCell(new Phrase(model.passExpiryDate.Value.ToString("MMMM dd, yyyy"), font))
            //        {
            //            BorderWidth = 0,
            //            Padding = 0
            //        };
            //        table.AddCell(text);
            //        rect = new Rectangle(60, 47, 395, 26);
            //        cb.Fill();
            //        ct = new ColumnText(cb);
            //        ct.SetSimpleColumn(rect);
            //        ct.AddElement(table);
            //        ct.Go();
            //    }
            //    Image _certificate = iTextSharp.text.Image.GetInstance(imgPath);
            //    _certificate.ScaleAbsolute(PageSize.A6.Rotate().Width, PageSize.A6.Rotate().Height);
            //    _certificate.SetAbsolutePosition(0, 0);
            //    pdfDoc.Add(_certificate);

            //    QRCodeWriter qrcodewriter = new QRCodeWriter();

            //    // instantiate a writer object
            //    BarcodeWriter barcodeWriter = new BarcodeWriter
            //    {
            //        // set the barcode format
            //        Format = BarcodeFormat.QR_CODE
            //    };
            //    barcodeWriter.Options.Height = 300;
            //    barcodeWriter.Options.Width = 300;
            //    string qrcodetext = model.passNumber;
            //    //string qrcodetext = @"MECARD:N:" + model.passNumber + ";ORG:DEWA;URL:" + HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host + LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EPASS_ADMINLOGIN) + ";NOTE:" + model.passNumber + ";;";
            //    Image barcodeimg = iTextSharp.text.Image.GetInstance(barcodeWriter.Write(qrcodetext), System.Drawing.Imaging.ImageFormat.Jpeg);
            //    table = new PdfPTable(1);
            //    barcodeimg.SpacingAfter = 0f;
            //    barcodeimg.SpacingBefore = 0f;
            //    barcodeimg.IndentationLeft = 0f;
            //    barcodeimg.IndentationRight = 0f;
            //    barcodeimg.ScaleAbsolute(100f, 100f);
            //    text = new PdfPCell(barcodeimg)
            //    {
            //        BorderWidth = 0,
            //        Padding = 0
            //    };
            //    table.AddCell(text);
            //    rect = new Rectangle(334, 96, 420, 10);
            //    cb.Fill();
            //    ct = new ColumnText(cb);
            //    ct.SetSimpleColumn(rect);
            //    ct.AddElement(table);
            //    ct.Go();

            //    if (byteArrayIn != null && byteArrayIn.Length > 0)
            //    {
            //        Image profileimage = Image.GetInstance(byteArrayIn);
            //        table = new PdfPTable(1);
            //        barcodeimg.ScaleAbsolute(150f, 170f);
            //        text = new PdfPCell(profileimage)
            //        {
            //            BorderWidth = 0,
            //            Padding = 0
            //        };
            //        table.AddCell(text);
            //        rect = new Rectangle(332, 179, 422, 89);
            //        cb.Fill();
            //        ct = new ColumnText(cb);
            //        ct.SetSimpleColumn(rect);
            //        ct.AddElement(table);
            //        ct.Go();
            //    }
            //    else
            //    {
            //        Image profileimage = iTextSharp.text.Image.GetInstance(Server.MapPath("~/images/IdealHome/profile-icon.png"));
            //        table = new PdfPTable(1);
            //        barcodeimg.ScaleAbsolute(150f, 170f);
            //        text = new PdfPCell(profileimage)
            //        {
            //            BorderWidth = 0,
            //            Padding = 0
            //        };
            //        table.AddCell(text);
            //        rect = new Rectangle(332, 179, 422, 89);
            //        cb.Fill();
            //        ct = new ColumnText(cb);
            //        ct.SetSimpleColumn(rect);
            //        ct.AddElement(table);
            //        ct.Go();
            //    }
            //    pdfDoc.Close();
            //    byte[] bytes = ms.ToArray();
            //    ms.Close();
               //return bytes;
               return new byte[0];
            //}
        }

        /// <summary>
        /// The fnValidateUser.
        /// </summary>
        /// <param name="model">The model<see cref="ePassLogin"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool fnValidateUser(ePassLogin model)
        {
            bool validation;
            try
            {
                LdapConnection ldapConn = new LdapConnection
                        (new LdapDirectoryIdentifier((string)null, false, false));
                NetworkCredential nc = new NetworkCredential(model.UserName,
                                       model.UserPassword, "DEWA");
                ldapConn.Credential = nc;
                ldapConn.AuthType = AuthType.Negotiate;
                // user has authenticated at this point,
                // as the credentials were used to login to the dc.
                //start of dev/debug code only remove it in production
                //end of dev/debug code only
                ldapConn.Bind(nc);
                validation = true;
                if (validation)
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                    {
                        UserPrincipal usr = UserPrincipal.FindByIdentity(context, model.UserName);
                        if (usr != null)
                        {
                            ClearSessionAndSignOut();

                            Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                            // First Check Admin Security
                            string SecurityRole = ePassConfig["Security Role"]; //"syed.shujaat@dewa.gov.ae;mayur.prajapati@dewa.gov.ae"; //
                            List<string> lstsecurity = SecurityRole.Split(';').ToList();
                            bool group12 = lstsecurity.Any(a => a.ToLower().Equals(usr.EmailAddress.ToLower()));
                            if (group12)
                            {
                                AuthStateService.Save(new DewaProfile(usr.SamAccountName, usr.Sid.ToString(), Roles.DewaSupplierSecurity)
                                {
                                    IsContactUpdated = true,
                                    Name = usr.DisplayName,
                                    EmailAddress = usr.EmailAddress,
                                    BusinessPartner = usr.SamAccountName  //UserPrincipalName is email address
                                });
                                if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
                                {
                                    Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddMinutes(60);
                                }
                                System.Web.HttpContext.Current.Session.Timeout = 60;
                                return validation;
                            }

                            List<SecurityPassViewModel> listpasses = Loggeduserpasses(usr.EmailAddress, usr.SamAccountName);
                            string OnedayInitiator = ePassConfig["Oneday Initiator"]; //"syed.shujaat@dewa.gov.ae;mayur.prajapati@dewa.gov.ae"; //
                            List<string> lstinitiator = OnedayInitiator.Split(';').ToList();
                            bool group13 = lstinitiator.Any(a => a.ToLower().Equals(usr.EmailAddress.ToLower()));
                            if (listpasses != null && listpasses.Count > 0)
                            {
                                AuthStateService.Save(new DewaProfile(usr.SamAccountName, usr.Sid.ToString(), Roles.DewaSupplierAdmin)
                                {
                                    IsContactUpdated = true,
                                    Name = usr.DisplayName,
                                    EmailAddress = usr.EmailAddress,
                                    BusinessPartner = usr.SamAccountName, //UserPrincipalName is userid
                                    HasActiveAccounts = group13  //if its oneday initiator as well
                                });
                                if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
                                {
                                    Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddMinutes(60);
                                }
                                System.Web.HttpContext.Current.Session.Timeout = 60;
                                return validation;
                            }

                            if (group13)
                            {
                                AuthStateService.Save(new DewaProfile(usr.SamAccountName, usr.Sid.ToString(), Roles.DewaonedayInitiator)
                                {
                                    IsContactUpdated = true,
                                    Name = usr.DisplayName,
                                    EmailAddress = usr.EmailAddress,
                                    BusinessPartner = usr.SamAccountName  //UserPrincipalName is email address
                                });
                                if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
                                {
                                    Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddMinutes(60);
                                }
                                System.Web.HttpContext.Current.Session.Timeout = 60;
                                return validation;
                            }

                            validation = false;
                        }
                    }
                }
            }
            catch (LdapException ex)
            {
                LogService.Fatal(ex, this);
                validation = false;
            }
            return validation;
        }

        /// <summary>
        /// The CheckEmailAddress.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <param name="ePassConfig">The ePassConfig<see cref="Item"/>.</param>
        /// <param name="errormessage">The errormessage<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool CheckEmailAddress(PermanentPass model, Item ePassConfig, out string errormessage)
        {
            bool valid = true;
            errormessage = string.Empty;
            try
            {
                if (ePassConfig != null && !string.IsNullOrWhiteSpace(model.op_visitorEmailid) && !string.IsNullOrWhiteSpace(model.op_seniormanagerEmailid))
                {
                    if (!model.op_seniormanagerEmailid.Trim().ToLower().EndsWith("@dewa.gov.ae"))
                    {
                        errormessage = Translate.Text("Epass.entervalidemailid");
                        valid = false;
                    }
                    if (valid)
                    {
                        string[] SecurityEmails = ePassConfig["Security Role"] != null ? ePassConfig["Security Role"].Split(';') : null;
                        string[] InitiatorEmails = ePassConfig["Oneday Initiator"] != null ? ePassConfig["Oneday Initiator"].Split(';') : null;

                        foreach (string email in SecurityEmails)
                        {
                            if (!string.IsNullOrWhiteSpace(email))
                            {
                                if (email.Equals(model.op_visitorEmailid))
                                {
                                    errormessage = Translate.Text("Epass.VisitorSeniorEmailId.SecurityRole.Validation");
                                    valid = false;
                                    break;
                                }
                            }
                        }
                        //foreach (string email in InitiatorEmails)
                        //{
                        //    if (email.Equals(model.op_visitorEmailid) || email.Equals(model.op_seniormanagerEmailid))
                        //    {
                        //        errormessage = Translate.Text("Epass.VisitorSeniorEmailId.Initiator.Validation");
                        //        valid = false;
                        //        break;
                        //    }
                        //}
                        if (model.op_visitorEmailid.Equals(model.op_seniormanagerEmailid))
                        {
                            errormessage = Translate.Text("Epass.VisitorSeniorEmailId.Validation");
                            valid = false;
                        }
                        else if (model.op_seniormanagerEmailid.Equals(CurrentPrincipal.EmailAddress) || model.op_visitorEmailid.Equals(CurrentPrincipal.EmailAddress))
                        {
                            errormessage = Translate.Text("Epass.VisitorSeniorEmailId.CurrentUser.Validation");
                            valid = false;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return valid;
        }

        /// <summary>
        /// The ClearSessionAndSignOut.
        /// </summary>
        private void ClearSessionAndSignOut()
        {
            //DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
            System.Web.Security.FormsAuthentication.SignOut();
            //Session.Abandon();
            Session.Clear();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
        }

        /// <summary>
        /// The CreateNightPass.
        /// </summary>
        /// <param name="passNo">The passNo<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult CreateNightPass(string passNo = "")
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            BaseSubPass model = new BaseSubPass() { PassType = SubPassType.Night };
            if (!string.IsNullOrWhiteSpace(passNo))
            {
                BasePassDetailModel relpassDetail = GetPassByPassNumber(passNo);
                if (relpassDetail != null && relpassDetail.MainPass != null && relpassDetail.MainPass.Count > 0 && relpassDetail.SubPasses != null && relpassDetail.SubPasses.Count > 0)
                {
                    BaseSubPassDetailModel subpass = relpassDetail.SubPasses.FirstOrDefault();
                    if (subpass != null)
                    {
                        model.PassNumber = subpass.subPassMainPassNo;
                        model.SubPassNumber = subpass.subPassRequestID;
                        model.StartDate = FormatEpassDate(subpass.subPassValidFrom).ToString("dd MMMM yyyy");
                        model.EndDate = FormatEpassDate(subpass.subPassValidTo).ToString("dd MMMM yyyy");
                        model.FromTime = FormatEpassDate(subpass.subPassValidFrom).ToString("HH:mm");
                        model.ToTime = FormatEpassDate(subpass.subPassValidTo).ToString("HH:mm");
                    }
                }
            }
            return PartialView("~/Views/Feature/GatePass/ePass/Module/_CreateHolidayOrNightPass.cshtml", model);
        }

        /// <summary>
        /// The CreateNightPass.
        /// </summary>
        /// <param name="model">The model<see cref="BaseSubPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNightPass(BaseSubPass model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            Tuple<bool, string> isValidPass = ValidatePassForSubPass(model);
            try
            {
                if (isValidPass.Item1 == true)
                {
                    if (string.IsNullOrWhiteSpace(model.SubPassNumber))
                    {
                        model.SubPassNumber = string.Format("{0}{1}{2}", "NP", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                    }

                    Tuple<bool, string, string> isSaved = SaveSubPass(SubPassType.Night, basspassmodel: model);

                    if (isSaved.Item1 == true)
                    {
                        ViewBag.RedirectLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EPASS_NIGHT_PASS);

                        return PartialView("~/Views/Feature/GatePass/ePass/_Success.cshtml", new PermanentPass()
                        {
                            ReferenceNumber = model.SubPassNumber,
                            PassType = SubPassType.Night.ToString()
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("", isSaved.Item2);
                    }
                }
                else
                {
                    ModelState.AddModelError("", isValidPass.Item2);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return PartialView("~/Views/Feature/GatePass/ePass/Module/_CreateHolidayOrNightPass.cshtml", model);
        }

        /// <summary>
        /// The CreateHolidayPass.
        /// </summary>
        /// <param name="passNo">The passNo<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult CreateHolidayPass(string passNo = "")
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            BaseSubPass model = new BaseSubPass() { PassType = SubPassType.Holiday };
            if (!string.IsNullOrWhiteSpace(passNo))
            {
                BasePassDetailModel relpassDetail = GetPassByPassNumber(passNo);
                if (relpassDetail != null && relpassDetail.MainPass != null && relpassDetail.MainPass.Count > 0 && relpassDetail.SubPasses != null && relpassDetail.SubPasses.Count > 0)
                {
                    BaseSubPassDetailModel subpass = relpassDetail.SubPasses.FirstOrDefault();
                    if (subpass != null)
                    {
                        model.PassNumber = subpass.subPassMainPassNo;
                        model.SubPassNumber = subpass.subPassRequestID;
                        model.StartDate = FormatEpassDate(subpass.subPassValidFrom).ToString("dd MMMM yyyy");
                        model.EndDate = FormatEpassDate(subpass.subPassValidTo).ToString("dd MMMM yyyy");
                        model.FromTime = FormatEpassDate(subpass.subPassValidFrom).ToString("HH:mm");
                        model.ToTime = FormatEpassDate(subpass.subPassValidTo).ToString("HH:mm");
                    }
                }
            }
            return PartialView("~/Views/Feature/GatePass/ePass/Module/_CreateHolidayOrNightPass.cshtml", model);
        }

        /// <summary>
        /// The CreateHolidayPass.
        /// </summary>
        /// <param name="model">The model<see cref="BaseSubPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHolidayPass(BaseSubPass model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }

            Tuple<bool, string> isValidPass = ValidatePassForSubPass(model);
            try
            {
                if (isValidPass.Item1 == true)
                {
                    if (string.IsNullOrWhiteSpace(model.SubPassNumber))
                    {
                        model.SubPassNumber = string.Format("{0}{1}{2}", "HP", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                    }

                    Tuple<bool, string, string> isSaved = SaveSubPass(SubPassType.Holiday, basspassmodel: model);

                    if (isSaved.Item1 == true)
                    {
                        ViewBag.RedirectLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EPASS_HOLIDAY_PASS);

                        return PartialView("~/Views/Feature/GatePass/ePass/_Success.cshtml", new PermanentPass()
                        {
                            ReferenceNumber = model.SubPassNumber,
                            PassType = SubPassType.Holiday.ToString()
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("", isSaved.Item2);
                    }
                }
                else
                {
                    ModelState.AddModelError("", isValidPass.Item2);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return PartialView("~/Views/Feature/GatePass/ePass/Module/_CreateHolidayOrNightPass.cshtml", model);
        }

        /// <summary>
        /// The CreateElectronicPass.
        /// </summary>
        /// <param name="passNo">The passNo<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult CreateElectronicPass(string passNo = "")
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            ElectronicPass model = new ElectronicPass();
            if (!string.IsNullOrWhiteSpace(passNo))
            {
                BasePassDetailModel relpassDetail = GetPassByPassNumber(passNo);
                if (relpassDetail != null && relpassDetail.MainPass != null && relpassDetail.MainPass.Count > 0 && relpassDetail.SubPasses != null && relpassDetail.SubPasses.Count > 0)
                {
                    BaseSubPassDetailModel subpass = relpassDetail.SubPasses.FirstOrDefault();
                    if (subpass != null)
                    {
                        model.PassNumber = subpass.subPassMainPassNo;
                        model.SubPassNumber = subpass.subPassRequestID;
                        model.NameoftheDevice = subpass.subPassDeviceType;
                        model.ModelName = subpass.subPassDeviceModel;
                        model.StartDate = FormatEpassDate(subpass.subPassValidFrom).ToString("dd MMMM yyyy");
                        model.EndDate = FormatEpassDate(subpass.subPassValidTo).ToString("dd MMMM yyyy");
                        model.SerialNumber = subpass.subPassDeviceSerialNo;
                        model.Purpose = subpass.subPassJustification;
                        model.FromTime = FormatEpassDate(subpass.subPassValidFrom).ToString("HH:mm");
                        model.ToTime = FormatEpassDate(subpass.subPassValidTo).ToString("HH:mm");
                    }
                }
            }
            model.PassType = SubPassType.Device;
            return PartialView("~/Views/Feature/GatePass/ePass/Module/_ElectronicPass.cshtml", model);
        }

        /// <summary>
        /// The CreateElectronicPass.
        /// </summary>
        /// <param name="model">The model<see cref="ElectronicPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateElectronicPass(ElectronicPass model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            string error = string.Empty;
            if (model.DevicePic != null)
            {
                if (!AttachmentIsValid(model.DevicePic, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        model.DevicePic.InputStream.CopyTo(memoryStream);
                        model.DevicePicbytes = memoryStream.ToArray();
                    }
                }
            }
            try
            {
                if (ModelState.IsValid)
                {
                    Tuple<bool, string> isValidPass = ValidatePassForSubPass(model);

                    if (isValidPass.Item1 == true)
                    {
                        if (string.IsNullOrWhiteSpace(model.SubPassNumber))
                        {
                            model.SubPassNumber = string.Format("{0}{1}{2}", "DP", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                        }

                        Tuple<bool, string, string> isSaved = SaveSubPass(SubPassType.Device, electronicmodel: model);
                        //var isSaved = SaveSubElectronicPass(model);

                        if (isSaved.Item1 == true)
                        {
                            ViewBag.RedirectLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EPASS_DEVICE_PASS);

                            return PartialView("~/Views/Feature/GatePass/ePass/_Success.cshtml", new PermanentPass()
                            {
                                ReferenceNumber = model.SubPassNumber,
                                PassType = SubPassType.Device.ToString()
                            });
                        }
                        else
                        {
                            ModelState.AddModelError("", isSaved.Item2);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", isValidPass.Item2);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return PartialView("~/Views/Feature/GatePass/ePass/Module/_ElectronicPass.cshtml", model);
        }

        /// <summary>
        /// The CreateMaterialPass.
        /// </summary>
        /// <param name="passNo">The passNo<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult CreateMaterialPass(string passNo = "")
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            MaterialPass model = new MaterialPass();
            if (!string.IsNullOrWhiteSpace(passNo))
            {
                BasePassDetailModel relpassDetail = GetPassByPassNumber(passNo);
                if (relpassDetail != null && relpassDetail.MainPass != null && relpassDetail.MainPass.Count > 0 && relpassDetail.SubPasses != null && relpassDetail.SubPasses.Count > 0)
                {
                    BaseSubPassDetailModel subpass = relpassDetail.SubPasses.FirstOrDefault();
                    if (subpass != null)
                    {
                        model.PassNumber = subpass.subPassMainPassNo;
                        model.SubPassNumber = subpass.subPassRequestID;
                        model.DeliveryNoteNumber = subpass.subPassDeliveryNoteNo;
                        model.Lponumber = subpass.subPassLPONo;
                        model.StartDate = FormatEpassDate(subpass.subPassValidFrom).ToString("dd MMMM yyyy");
                        model.EndDate = FormatEpassDate(subpass.subPassValidTo).ToString("dd MMMM yyyy");
                        model.FromTime = FormatEpassDate(subpass.subPassValidFrom).ToString("HH:mm");
                        model.ToTime = FormatEpassDate(subpass.subPassValidTo).ToString("HH:mm");
                    }
                }
            }
            model.PassType = SubPassType.Material;
            return PartialView("~/Views/Feature/GatePass/ePass/Module/_MaterialinoutPass.cshtml", model);
        }

        /// <summary>
        /// The CreateMaterialPass.
        /// </summary>
        /// <param name="model">The model<see cref="MaterialPass"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMaterialPass(MaterialPass model)
        {
            if (!IsEpassLoggedIn || !CurrentPrincipal.Role.Equals(Roles.DewaSupplier))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EPASS_LOGIN);
            }
            string error = string.Empty;
            if (model.ContractorSiteAttachment != null)
            {
                if (!AttachmentIsValid(model.ContractorSiteAttachment, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        model.ContractorSiteAttachment.InputStream.CopyTo(memoryStream);
                        model.ContractorSiteAttachmentbytes = memoryStream.ToArray();
                    }
                }
            }
            if (model.DeliveryNoteattachment != null)
            {
                if (!AttachmentIsValid(model.DeliveryNoteattachment, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        model.DeliveryNoteattachment.InputStream.CopyTo(memoryStream);
                        model.DeliveryNoteattachmentbytes = memoryStream.ToArray();
                    }
                }
            }
            if (model.LPOAttachment != null)
            {
                if (!AttachmentIsValid(model.LPOAttachment, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        model.LPOAttachment.InputStream.CopyTo(memoryStream);
                        model.LPOAttachmentbytes = memoryStream.ToArray();
                    }
                }
            }
            try
            {
                if (ModelState.IsValid)
                {
                    Tuple<bool, string> isValidPass = ValidatePassForSubPass(model);

                    if (isValidPass.Item1 == true)
                    {
                        if (string.IsNullOrWhiteSpace(model.SubPassNumber))
                        {
                            model.SubPassNumber = string.Format("{0}{1}{2}", "MP", DateTime.Now.ToString("MMdd"), GetPassNumber().ToString());
                        }

                        Tuple<bool, string, string> isSaved = SaveSubPass(SubPassType.Material, materialmodel: model);
                        //var isSaved = SaveSubElectronicPass(model);

                        if (isSaved.Item1 == true)
                        {
                            ViewBag.RedirectLink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EPASS_MATERIAL_PASS);

                            return PartialView("~/Views/Feature/GatePass/ePass/_Success.cshtml", new PermanentPass()
                            {
                                ReferenceNumber = model.SubPassNumber,
                                PassType = SubPassType.Material.ToString()
                            });
                        }
                        else
                        {
                            ModelState.AddModelError("", isSaved.Item2);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", isValidPass.Item2);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return PartialView("~/Views/Feature/GatePass/ePass/Module/_MaterialinoutPass.cshtml", model);
        }

        /// <summary>
        /// The ValidatePassForSubPass.
        /// </summary>
        /// <param name="model">The model<see cref="BaseSubPass"/>.</param>
        /// <returns>The <see cref="Tuple{bool, string}"/>.</returns>
        private Tuple<bool, string> ValidatePassForSubPass(BaseSubPass model)
        {
            Tuple<bool, string> retval = new Tuple<bool, string>(false, Translate.Text("Epass.InvalidPassNoForSubPass"));
            try
            {
                //var mainPass = GetPassDetailsByPassNumber("", model.PassNumber);
                //BasePassViewModel relPassdetail = new BasePassViewModel();
                BasePassDetailModel Passdetail = GetPassByPassNumber(model.PassNumber);
                SecurityPassViewModel mainPass = null;
                if (Passdetail != null && Passdetail.MainPass != null && Passdetail.MainPass.Count > 0)
                {
                    BaseMainPassDetailModel relPassdetail = Passdetail.MainPass.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(relPassdetail.ePassVendorID) && relPassdetail.ePassVendorID.Equals(CurrentPrincipal.BusinessPartner))
                    {
                        mainPass = new SecurityPassViewModel
                        {
                            name = relPassdetail.ePassVisitorName,
                            passNumber = relPassdetail.ePassPassNo,
                            passType = relPassdetail.ePassPassType,
                            passTypeText = relPassdetail.ePassPassType,
                            profession = relPassdetail.ePassProfession,
                            Designation = relPassdetail.ePassDesignation,
                            passExpiryDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassPassExpiryDate)) ? FormatEpassDate(relPassdetail.ePassPassExpiryDate) :
                                         (relPassdetail.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(relPassdetail.ePassVisitingDate) ? FormatEpassDate(relPassdetail.ePassVisitingDate) : default(DateTime?)),
                            passIssueDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassPassIssueDate)) ? FormatEpassDate(relPassdetail.ePassPassIssueDate) :
                        (relPassdetail.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(relPassdetail.ePassVisitingDate) ? FormatEpassDate(relPassdetail.ePassVisitingDate) : default(DateTime?)),
                            CreatedDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassCreatedOn)) ? FormatEpassDate(relPassdetail.ePassCreatedOn) : default(DateTime?),
                            strpassExpiryDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassPassExpiryDate)) ? FormatEpassDate(relPassdetail.ePassPassExpiryDate).ToString("MMMM dd, yyyy") :
                              (relPassdetail.ePassPassType.Equals(EpassHelper.GetDisplayName(PassType.OnedayPass)) && !string.IsNullOrWhiteSpace(relPassdetail.ePassVisitingDate) ? FormatEpassDate(relPassdetail.ePassVisitingDate).ToString("MMMM dd, yyyy") : string.Empty),
                            status = assignStatus(relPassdetail.ePassPassStatus, relPassdetail.ePassPassExpiryDate, (relPassdetail.ePassIsBlocked == "Yes" ? true : false)),
                            strstatus = Translate.Text("epassstatus." + assignStatus(relPassdetail.ePassPassStatus, relPassdetail.ePassPassExpiryDate, (relPassdetail.ePassIsBlocked == "Yes" ? true : false)).ToString().ToLower()),
                            strclass = assignStatus(relPassdetail.ePassPassStatus, relPassdetail.ePassPassExpiryDate, (relPassdetail.ePassIsBlocked == "Yes" ? true : false)).ToString(),
                            nationality = relPassdetail.ePassNationality,
                            emiratesId = relPassdetail.ePassEmiratesID,
                            emiratesExpiryDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassEmiratesiDExpiry)) ? FormatEpassDate(relPassdetail.ePassEmiratesiDExpiry) : default(DateTime?),
                            visaNumber = relPassdetail.ePassVisaNumber,
                            visaExpiryDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassVisaExpiryDate)) ? FormatEpassDate(relPassdetail.ePassVisaExpiryDate) : default(DateTime?),
                            emailLimit = (relPassdetail.ePassEmailLimit == null) ? 0 : Convert.ToInt16(relPassdetail.ePassEmailLimit),
                            smsLimit = (relPassdetail.ePassSMSLimit == null) ? 0 : Convert.ToInt16(relPassdetail.ePassSMSLimit),
                            downloadLimit = (relPassdetail.ePassDownloadLimit == null) ? 0 : Convert.ToInt16(relPassdetail.ePassDownloadLimit),
                            passportNumber = relPassdetail.ePassPassportNumber,
                            passportExpiryDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassPassportExpiryDate)) ? FormatEpassDate(relPassdetail.ePassPassportExpiryDate) : default(DateTime?),
                            fromTime = relPassdetail.ePassFromDateTime,
                            toTime = relPassdetail.ePassToDateTime,
                            mobile = relPassdetail.ePassMobileNumber,
                            email = relPassdetail.ePassEmailAddress,
                            VisitorEmail = relPassdetail.ePassVisitorEmailID,
                            SeniorManagerEmail = relPassdetail.ePassProjectDeptApprovers,
                            //LinkURL = relPassdetail.ePassVi,
                            //linkExpiryDate = relPassdetail.ePassVi,
                            Location = lstselectedlocations(relPassdetail.ePassLocation),
                            //RejectRemarks = relPassdetail.epa,
                            Subcontractor = relPassdetail.ePassSubContractorID,
                            //passAttachements = relPassdetail.ep
                            companyName = relPassdetail.ePassCompanyName,
                            projectName = relPassdetail.ePassProjectName,
                            projectStartDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassProjectStartName)) ? FormatEpassDate(relPassdetail.ePassProjectStartName) : default(DateTime?),
                            projectEndDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassProjectEndDate)) ? FormatEpassDate(relPassdetail.ePassProjectEndDate) : default(DateTime?),
                            projectId = relPassdetail.ePassProjectID,
                            projectStatus = relPassdetail.ePassProjectStatus,
                            departmentName = relPassdetail.ePassDepartmentName,
                            IsBlocked = (relPassdetail.ePassIsBlocked == "Yes" ? true : false),
                            DEWAID = relPassdetail.ePassDEWAID,
                            DEWAdesignation = relPassdetail.ePassDesignation,
                            VehicleRegNumber = relPassdetail.ePassVehicleNo,
                            VehRegistrationDate = (!string.IsNullOrWhiteSpace(relPassdetail.ePassVehicleRegDate)) ? FormatEpassDate(relPassdetail.ePassVehicleRegDate) : default(DateTime?),
                            strpassVehicleRegDate = !string.IsNullOrWhiteSpace(relPassdetail.ePassVehicleRegDate) ? FormatEpassDate(relPassdetail.ePassVehicleRegDate).ToString("MMMM dd, yyyy") : string.Empty,
                            pendingwith = (relPassdetail.ePassSecurityApprovers != null && relPassdetail.ePassPassStatus != null && !string.IsNullOrWhiteSpace(relPassdetail.ePassSecurityApprovers.ToString()) && !string.IsNullOrWhiteSpace(relPassdetail.ePassPassStatus.ToString())) ? (relPassdetail.ePassPassStatus.ToString().ToLower().Equals("Dept Coordinator".ToLower()) ? relPassdetail.ePassSecurityApprovers.ToString() : (relPassdetail.ePassPassStatus.ToString().ToLower().Equals("Security Team".ToLower()) ? Translate.Text("Epass.SecurityAdmin") : string.Empty)) : string.Empty,
                        };
                    }
                }
                const string validpasstype1 = "short term";
                const string validpasstype2 = "long term";
                const string validpasstype3 = "OnedayPass";
                //if (mainPass != null && mainPass.status == SecurityPassStatus.Active && !string.IsNullOrEmpty(mainPass.passTypeText) && validpasstype.Contains(mainPass.passTypeText.ToLower()))
                if (mainPass != null &&
                    (mainPass.status.Equals(SecurityPassStatus.Active) || mainPass.status.Equals(SecurityPassStatus.SoontoExpire)) &&
                    !string.IsNullOrEmpty(mainPass.passTypeText) &&
                    (validpasstype1.Contains(mainPass.passTypeText.ToLower()) || validpasstype2.Contains(mainPass.passTypeText.ToLower()) || validpasstype3.Contains(mainPass.passTypeText.ToLower())))
                {
                    if (mainPass.passExpiryDate != null && mainPass.passExpiryDate.Value.Ticks >= FormatEpassDate(model.EndDate).Ticks)
                    {
                        //pass is valid so create sub pass
                        model.ManagerEmail = mainPass.SeniorManagerEmail;
                        model.Location = mainPass.Location != null && mainPass.Location.Count > 0 ? mainPass.Location.Aggregate((i, j) => i + "," + j) : string.Empty;
                        model.RequesterEmail = CurrentPrincipal.EmailAddress;
                        Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                        model.SecurityEmail = ePassConfig["Security Role"];

                        retval = new Tuple<bool, string>(true, "");
                    }
                    else
                    {
                        retval = new Tuple<bool, string>(false, Translate.Text("Epass.passexpirygreaterthanmainpassexpiry"));
                    }
                }
                else
                {
                    retval = new Tuple<bool, string>(false, Translate.Text("Epass.PassNotFoundorInvalid"));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, "ePassController.ValidatePassForSubPass");
            }

            return retval;
        }

        /// <summary>
        /// The SaveMainPass.
        /// </summary>
        /// <param name="uniquePassNumber">The uniquePassNumber<see cref="string"/>.</param>
        /// <param name="permanentPass">The permanentPass<see cref="PermanentPass"/>.</param>
        /// <param name="uniqueUrl">The uniqueUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="Tuple{bool, string, string}"/>.</returns>
        private Tuple<bool, string, string> SaveMainPass(string uniquePassNumber, PermanentPass permanentPass = null, string uniqueUrl = "")
        {
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                permanentPass.PassNumber = uniquePassNumber;
                Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                permanentPass.SecurityApproversEmail = ePassConfig["Security Role"];
                if (permanentPass.PassType == EpassHelper.GetDisplayName(PassType.OnedayPass))
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.Mainpassesname) { Attribute = GetOnedayPassAttributes(permanentPass, uniqueUrl, uniquePassNumber, true) });
                }
                else if (permanentPass.PassType == EpassHelper.GetDisplayName(PassType.LongTerm) || permanentPass.PassType == EpassHelper.GetDisplayName(PassType.ShortTerm))
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.Mainpassesname) { Attribute = GetMainPassAttributes(permanentPass) });
                }
                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CreateMainPass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    string newid = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Status") && x.Type.Equals("text")).FirstOrDefault().Value;
                    retval = new Tuple<bool, string, string>(true, string.Empty, newid);
                }
                else
                {
                    retval = new Tuple<bool, string, string>(false, res.Message, "");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return retval;
        }

        /// <summary>
        /// The UpdateOneDayPass.
        /// </summary>
        /// <param name="permanentPass">The permanentPass<see cref="PermanentPass"/>.</param>
        /// <param name="uniqueUrl">The uniqueUrl<see cref="string"/>.</param>
        /// <param name="isReInitiate">The isReInitiate<see cref="bool"/>.</param>
        /// <returns>The <see cref="Tuple{bool, string, string}"/>.</returns>
        private Tuple<bool, string, string> UpdateOneDayPass(PermanentPass permanentPass, string uniqueUrl = "", bool isReInitiate = false)
        {
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                //permanentPass.PassNumber = uniquePassNumber;
                Item ePassConfig = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.ePass_CONFIG);
                permanentPass.SecurityApproversEmail = ePassConfig["Security Role"];

                baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.OneDayPassRequestIn) { Attribute = GetOnedayPassAttributes(permanentPass, "", permanentPass.PassNumber, false, isReInitiate) });

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.OneDayPassUpdateMethod, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    //string newid = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Status") && x.Type.Equals("text")).FirstOrDefault().Value;
                    retval = new Tuple<bool, string, string>(true, string.Empty, string.Empty);
                }
                else
                {
                    retval = new Tuple<bool, string, string>(false, ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE, "");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                retval = new Tuple<bool, string, string>(false, ex.Message, "");
            }
            return retval;
        }

        /// <summary>
        /// The SaveSubPass.
        /// </summary>
        /// <param name="subPassType">The subPassType<see cref="SubPassType"/>.</param>
        /// <param name="basspassmodel">The basspassmodel<see cref="BaseSubPass"/>.</param>
        /// <param name="electronicmodel">The electronicmodel<see cref="ElectronicPass"/>.</param>
        /// <param name="materialmodel">The materialmodel<see cref="MaterialPass"/>.</param>
        /// <returns>The <see cref="Tuple{bool, string, string}"/>.</returns>
        private Tuple<bool, string, string> SaveSubPass(SubPassType subPassType, BaseSubPass basspassmodel = null, ElectronicPass electronicmodel = null, MaterialPass materialmodel = null)
        {
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                if (subPassType.Equals(SubPassType.Holiday) || subPassType.Equals(SubPassType.Night))
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.Subpassesname) { Attribute = GetHolidayorNightPassAttributes(basspassmodel, subPassType) });
                }
                else if (subPassType.Equals(SubPassType.Device))
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.Subpassesname) { Attribute = GetElectriconicPassAttributes(electronicmodel) });
                }
                else if (subPassType.Equals(SubPassType.Material))
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.Subpassesname) { Attribute = GetMaterialPassAttributes(materialmodel) });
                }

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CreateEpass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                //create subpass in kofax
                //model.Status = "Initiated";
                //var res = KofaxRESTService.SubmitKofax(KofaxConstants.CreateEpass, basspassmodel.ToKofaxRequestObject());
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    string newid = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Status") && x.Type.Equals("text")).FirstOrDefault().Value;
                    if (!string.IsNullOrWhiteSpace(newid) && newid.ToLower().Equals("failed"))
                    {
                        string message = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Message") && x.Type.Equals("text")).FirstOrDefault().Value;
                        retval = new Tuple<bool, string, string>(false, message, newid);
                    }
                    else if (!string.IsNullOrWhiteSpace(newid) && newid.ToLower().Equals("success"))
                    {
                        string message = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Message") && x.Type.Equals("text")).FirstOrDefault().Value;
                        retval = new Tuple<bool, string, string>(true, message, newid);
                    }
                }
                else
                {
                    retval = new Tuple<bool, string, string>(false, ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE, "");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this); retval = new Tuple<bool, string, string>(false, ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE, ex.Message);
            }

            return retval;
        }

        /// <summary>
        /// The UpdatePassStatus.
        /// </summary>
        /// <param name="model">The model<see cref="SecurityApproveRejectPassViewModel"/>.</param>
        /// <param name="passDetail">The passDetail<see cref="SecurityPassViewModel"/>.</param>
        /// <param name="isBlocked">The isBlocked<see cref="bool"/>.</param>
        /// <param name="isUserBlocked">The isUserBlocked<see cref="bool"/>.</param>
        /// <param name="isPassUnblocked">The isPassUnblocked<see cref="bool"/>.</param>
        /// <param name="isUserUnblocked">The isUserUnblocked<see cref="bool"/>.</param>
        /// <param name="unblockmethod">The unblockmethod<see cref="bool"/>.</param>
        /// <param name="blockeduser">The blockeduser<see cref="SecurityBlockedUserViewModel"/>.</param>
        /// <returns>The <see cref="Tuple{bool, string, string}"/>.</returns>
        private Tuple<bool, string, string> UpdatePassStatus(SecurityApproveRejectPassViewModel model = null, SecurityPassViewModel passDetail = null, bool isBlocked = false, bool isUserBlocked = false
            , bool isPassUnblocked = false, bool isUserUnblocked = false, bool unblockmethod = false, SecurityBlockedUserViewModel blockeduser = null)
        {
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                ServiceResponse<KofaxRestResponse> res = null;
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                if (model != null)
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.UpdateStatus) { Attribute = GetPassApprovalAttributes(model) });
                    res = KofaxRESTService.SubmitKofax(KofaxConstants.UpdateApprovalStatus, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                }
                if (passDetail != null)
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.UpdateShareStatus) { Attribute = GetPassShareAttributes(passDetail, isBlocked, isUserBlocked, isPassUnblocked, isUserUnblocked) });
                    res = KofaxRESTService.SubmitKofax(KofaxConstants.UpdateShareStatusURL, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                }
                if (unblockmethod)
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.UpdateShareStatus) { Attribute = GetUnblockAttributes(blockeduser) });
                    res = KofaxRESTService.SubmitKofax(KofaxConstants.UpdateShareStatusURL, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                }
#if DEBUG
                LogService.Info(new System.Exception("EPass Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });
#endif
                if (res != null && res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    string newid = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Status") && x.Type.Equals("text")).FirstOrDefault().Value;
                    if (!string.IsNullOrWhiteSpace(newid) && newid.ToLower().Equals("failed"))
                    {
                        string message = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Message") && x.Type.Equals("text")).FirstOrDefault().Value;
                        retval = new Tuple<bool, string, string>(false, message, newid);
                    }
                    else if (!string.IsNullOrWhiteSpace(newid) && newid.ToLower().Equals("success"))
                    {
                        string message = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("Message") && x.Type.Equals("text")).FirstOrDefault().Value;
                        retval = new Tuple<bool, string, string>(true, message, newid);
                    }
                }
                else
                {
                    retval = new Tuple<bool, string, string>(false, res.Message, "");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return retval;
        }

        /// <summary>
        /// The GetHolidayorNightPassAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="BaseSubPass"/>.</param>
        /// <param name="subPassType">The subPassType<see cref="SubPassType"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetHolidayorNightPassAttributes(BaseSubPass model, SubPassType subPassType)
        {
            // model.ManagerEmail = "sivakumar.r@dewa.gov.ae";
            //model.SecurityEmail = "hansrajsinh.rathva@dewa.gov.ae";
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = model.PassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ReqID", Value = model.SubPassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassValidity", Value = FormatKofaxDate(model.EndDate) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "NewPassType", Value = subPassType.ToString() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Justification", Value = string.Empty},
                new Attribute { Type = EpassTypeEnum.Text, Name = "ValidFrom", Value = FormatEpassDate(model.StartDate).ToString("MM-dd-yyyy") + " " + model.FromTime},
                new Attribute { Type = EpassTypeEnum.Text, Name = "ValidTo", Value = FormatEpassDate(model.EndDate).ToString("MM-dd-yyyy") + " " + model.ToTime },
                new Attribute { Type = EpassTypeEnum.Text, Name = "RequestorEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "DeptMgrEmail", Value = model.ManagerEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "SecurityEmail", Value = model.SecurityEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "CreatedByEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "Status", Value = SecurityPassStatus.Initiated.ToString()},
            };
        }

        /// <summary>
        /// The GetElectriconicPassAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="ElectronicPass"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetElectriconicPassAttributes(ElectronicPass model)
        {
            //model.ManagerEmail = "sivakumar.r@dewa.gov.ae";
            //model.SecurityEmail = "hansrajsinh.rathva@dewa.gov.ae";
            List<KofaxFileViewModel> fileViewModel = GetSubPassAttachment(model);
            string attachmentJSON = JsonConvert.SerializeObject(fileViewModel.ToArray(), Converter.Settings);
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = model.PassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ReqID", Value = model.SubPassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassValidity", Value = FormatKofaxDate(model.EndDate) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "NewPassType", Value = SubPassType.Device.ToString() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "DeviceType", Value = model.NameoftheDevice},
                new Attribute { Type = EpassTypeEnum.Text, Name = "DeviceModel", Value = model.ModelName},
                new Attribute { Type = EpassTypeEnum.Text, Name = "DeviceSerialNo", Value = model.SerialNumber},
                new Attribute { Type = EpassTypeEnum.Text, Name = "Location", Value = model.Location},
                new Attribute { Type = EpassTypeEnum.Text, Name = "Justification", Value = model.Purpose},
                new Attribute { Type = EpassTypeEnum.Text, Name = "ValidFrom", Value = FormatEpassDate(model.StartDate).ToString("MM-dd-yyyy")+ " " + model.FromTime},
                new Attribute { Type = EpassTypeEnum.Text, Name = "ValidTo", Value = FormatEpassDate(model.EndDate).ToString("MM-dd-yyyy") + " " + model.ToTime },
                new Attribute { Type = EpassTypeEnum.Text, Name = "RequestorEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "DeptMgrEmail", Value = model.ManagerEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "SecurityEmail", Value = model.SecurityEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "CreatedByEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "Status", Value = SecurityPassStatus.Initiated.ToString()},
                new Attribute { Type = EpassTypeEnum.Text, Name = "SupportingFiles", Value = attachmentJSON },
            };
        }

        /// <summary>
        /// The GetMaterialPassAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="MaterialPass"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetMaterialPassAttributes(MaterialPass model)
        {
            //model.ManagerEmail = "sivakumar.r@dewa.gov.ae";
            //model.SecurityEmail = "hansrajsinh.rathva@dewa.gov.ae";
            List<KofaxFileViewModel> fileViewModel = GetSubPassMaterialAttachment(model);
            string attachmentJSON = JsonConvert.SerializeObject(fileViewModel.ToArray(), Converter.Settings);
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = model.PassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ReqID", Value = model.SubPassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassValidity", Value = FormatKofaxDate(model.EndDate) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "NewPassType", Value = SubPassType.Material.ToString() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "MaterialPassType", Value = model.MaterialMode},
                new Attribute { Type = EpassTypeEnum.Text, Name = "MaterialList", Value = model.MaterialInformation},
                new Attribute { Type = EpassTypeEnum.Text, Name = "LocationType", Value = model.Storelocation},
                new Attribute { Type = EpassTypeEnum.Text, Name = "LPONo", Value = model.Lponumber},
                new Attribute { Type = EpassTypeEnum.Text, Name = "DeliveryNoteNo", Value = model.DeliveryNoteNumber},
                new Attribute { Type = EpassTypeEnum.Text, Name = "ValidFrom", Value = FormatEpassDate(model.StartDate).ToString("MM-dd-yyyy")+ " 00:00"},
                new Attribute { Type = EpassTypeEnum.Text, Name = "ValidTo", Value = FormatEpassDate(model.EndDate).ToString("MM-dd-yyyy") + " 00:00" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "RequestorEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "DeptMgrEmail", Value = model.ManagerEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "SecurityEmail", Value = model.SecurityEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "CreatedByEmail", Value = model.RequesterEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "Status", Value = SecurityPassStatus.Initiated.ToString()},
                new Attribute { Type = EpassTypeEnum.Text, Name = "SupportingFiles", Value = attachmentJSON },
            };
        }

        /// <summary>
        /// The GetmyPassAttributes.
        /// </summary>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetmyPassAttributes()
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "Email", Value = CurrentPrincipal.EmailAddress },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Name", Value = CurrentPrincipal.EmailAddress },
            };
        }

        /// <summary>
        /// The GetPassDetailAttributes.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetPassDetailAttributes(string passNumber)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = passNumber },
            };
        }

        /// <summary>
        /// The SearchPassDetailAttributes.
        /// </summary>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> SearchPassDetailAttributes(string keyword)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = keyword },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesID", Value = keyword },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisaNo", Value = keyword },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassportNo", Value = keyword },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorName", Value = keyword },
            };
        }

        /// <summary>
        /// The GetDeptmgrPassAttributes.
        /// </summary>
        /// <param name="dptmgr">The dptmgr<see cref="string"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetDeptmgrPassAttributes(string dptmgr)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "Status", Value = "Initiated" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Email", Value = !string.IsNullOrWhiteSpace(dptmgr)?dptmgr: CurrentPrincipal.EmailAddress },
            };
        }

        /// <summary>
        /// The GetSecurityAdminPassAttributes.
        /// </summary>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetSecurityAdminPassAttributes()
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "Status", Value = "Dept Approved" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Email", Value = CurrentPrincipal.EmailAddress },
            };
        }

        /// <summary>
        /// The GetMainPassAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetMainPassAttributes(PermanentPass model)
        {
            List<KofaxFileViewModel> fileViewModel = GetAttachment(model);
            model.Submitter_Email_ID = CurrentPrincipal.EmailAddress.ToLower();
            model.op_visitorEmailid = CurrentPrincipal.EmailAddress.ToLower();
            model.VendorID = CurrentPrincipal.BusinessPartner;
            //model.Coor_eMail_IDs = "mayur.prajapati@dewa.gov.ae";
            string attachmentJSON = JsonConvert.SerializeObject(fileViewModel.ToArray(), Converter.Settings);
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = model.PassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassType", Value = model.PassType },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassStatus", Value = SecurityPassStatus.Initiated.ToString() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorName", Value = (!string.IsNullOrWhiteSpace(model.FullName) ? model.FullName : model.op_visitorname) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Nationality", Value = model.Nationality },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Profession", Value = model.ProfessionLevel },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Designation", Value = model.Designation },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesID", Value = model.EmiratesID },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesiDExpiry", Value = FormatKofaxDate(model.EmiratesIDExpiry) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisaNumber", Value = model.VisaNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisaExpiryDate", Value = FormatKofaxDate(model.VisaExpiry) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassportNumber", Value = model.PassportNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassportExpiryDate", Value = FormatKofaxDate(model.PassportExpiry) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SubContractorID", Value = model.SubContractorID },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VendorID", Value = model.VendorID },
                new Attribute { Type = EpassTypeEnum.Text, Name = "CoordinatorNames", Value = model.Coor_Username_List },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectDeptApprovers", Value = model.Coor_eMail_IDs},
                new Attribute { Type = EpassTypeEnum.Text, Name = "MobileNumber", Value = model.Mobilenumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SecurityApprovers", Value = model.SecurityApproversEmail},
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmailAddress", Value = model.Emailaddress },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SMSLimit", Value = "0" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmailLimit", Value = "0" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "DownloadLimit", Value = "0" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PONumber", Value = model.PONumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectName", Value = !string.IsNullOrWhiteSpace(model.op_projectName)?model.op_projectName:model.POName },
                new Attribute { Type = EpassTypeEnum.Text, Name = "CompanyName", Value = model.CompanyName },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectStartDate", Value =FormatKofaxDate(model.projectStartDate.ToString()) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectEndDate", Value = FormatKofaxDate(model.projectEndDate.ToString()) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassExpiryDate", Value = FormatKofaxDate(model.PassExpiry.ToString()) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassIssueDate", Value = FormatKofaxDate(model.PassIssue.ToString())},
                new Attribute { Type = EpassTypeEnum.Text, Name = "FromDateTime", Value = model.FromTime }, // 24 hour format
                new Attribute { Type = EpassTypeEnum.Text, Name = "ToDateTime", Value = model.ToTime }, // 24 hour format
                new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectID", Value = model.projectId },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectStatus", Value = model.projectStatus },
                new Attribute { Type = EpassTypeEnum.Text, Name = "DepartmentName", Value = model.departmentName },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Location", Value = model.SelectedLocation.Count() > 0 ? string.Join("; ", model.SelectedLocation) : "" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "AdminSecurityEmails", Value = model.SecurityApproversEmail },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SubmitterEmail", Value = model.Submitter_Email_ID },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorEmailID", Value = model.op_visitorEmailid },
                new Attribute { Type = EpassTypeEnum.Text, Name = "DEWAID", Value = model.op_dewantid },
                new Attribute { Type = EpassTypeEnum.Text, Name = "JobTiltle", Value = model.Designation },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisitingDate", Value =FormatKofaxDate(model.DateOfVisit) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VistingTimeFrom", Value = model.FromTime },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VistingTimeTo", Value = model.ToTime },

            new Attribute { Type = EpassTypeEnum.Text, Name = "VehicleNo", Value = model.withcar? FormatVehiclePlateNumber(model.EmirateOrCountry, model.PlateCode,model.PlateNumber):string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VehicleRegistrationDate", Value = model.withcar? FormatKofaxDate(model.VehicleRegistrationDate):string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SupportingFiles", Value = attachmentJSON },
            };
        }

        /// <summary>
        /// The GetOnedayPassAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <param name="uniqueURL">The uniqueURL<see cref="string"/>.</param>
        /// <param name="uniquePassNumber">The uniquePassNumber<see cref="string"/>.</param>
        /// <param name="isInitial">The isInitial<see cref="bool"/>.</param>
        /// <param name="isReinitiate">The isReinitiate<see cref="bool"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetOnedayPassAttributes(PermanentPass model, string uniqueURL, string uniquePassNumber, bool isInitial = true, bool isReinitiate = false)
        {
            List<Attribute> retval = new List<Attribute>();
            model.Submitter_Email_ID = CurrentPrincipal.EmailAddress.ToLower();

            if (isInitial)
            {
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = uniquePassNumber });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassType", Value = model.PassType });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassStatus", Value = SecurityPassStatus.Initiated.ToString() });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorName", Value = model.FullName ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectName", Value = model.op_projectName ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "MobileNumber", Value = model.Mobilenumber ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "SubmitterEmail", Value = model.Submitter_Email_ID });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "CompanyName", Value = model.CompanyName });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectDeptApprovers", Value = model.op_seniormanagerEmailid ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorEmailID", Value = model.op_visitorEmailid ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassStatus", Value = SecurityPassStatus.Initiated.ToString() });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "SecurityApprovers", Value = model.SecurityApproversEmail });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "SMSLimit", Value = "0" });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "EmailLimit", Value = "0" });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "DownloadLimit", Value = "0" });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectID", Value = model.op_projectID ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Nationality", Value = model.Nationality ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Profession", Value = model.ProfessionLevel ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Designation", Value = model.Designation ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisitingDate", Value = FormatKofaxDate(model.DateOfVisit) });
                //new Attribute { Type = EpassTypeEnum.Text, Name = "VistingTimeFrom", Value = model.FromTime ??string.Empty},
                //new Attribute { Type = EpassTypeEnum.Text, Name = "VistingTimeTo", Value = model.ToTime ??string.Empty},

                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesID", Value = (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? model.EmiratesID : string.Empty }); //emiratesid or passport no
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesiDExpiry", Value = (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? FormatKofaxDate(model.EmiratesIDExpiry) : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassportNumber", Value = (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? string.Empty : model.EmiratesID });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassportExpiryDate", Value = (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? string.Empty : FormatKofaxDate(model.EmiratesIDExpiry) });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisaNumber", Value = model.VisaNumber ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisaExpiryDate", Value = string.IsNullOrEmpty(model.VisaExpiry) ? "" : FormatKofaxDate(model.VisaExpiry) });

                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Location", Value = model.SelectedLocation != null && model.SelectedLocation.Count() > 0 ? string.Join(";", model.SelectedLocation) : "" });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "CoordinatorNames", Value = model.Coor_Username_List ?? string.Empty });

                //new Attribute { Type = EpassTypeEnum.Text, Name = "EmailAddress", Value = model.Emailaddress },

                //new Attribute { Type = EpassTypeEnum.Text, Name = "AdminSecurityEmails", Value = model.SecurityApproversEmail },
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "DEWAID", Value = model.op_dewantid ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VehicleNo", Value = model.withcar ? FormatVehiclePlateNumber(model.EmirateOrCountry, model.PlateCode, model.PlateNumber) : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VehicleRegistrationDate", Value = model.withcar ? FormatKofaxDate(model.VehicleRegistrationDate) : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "LinkURL", Value = uniqueURL });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "LinkExpiry", Value = DateTime.Now.AddDays(3).ToString("MM/dd/yyyy HH:mm:ss") });
            }
            else
            {
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = uniquePassNumber });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorName", Value = isReinitiate ? string.Empty : model.FullName ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Nationality", Value = isReinitiate ? string.Empty : model.Nationality ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Profession", Value = isReinitiate ? string.Empty : model.ProfessionLevel ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Designation", Value = isReinitiate ? string.Empty : model.Designation ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesID", Value = isReinitiate ? string.Empty : (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? model.EmiratesID : string.Empty }); //emiratesid or passport no
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesiDExpiry", Value = isReinitiate ? string.Empty : (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? FormatKofaxDate(model.EmiratesIDExpiry) : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisaNumber", Value = isReinitiate ? string.Empty : model.VisaNumber ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisaExpiryDate", Value = isReinitiate ? string.Empty : string.IsNullOrEmpty(model.VisaExpiry) ? "" : FormatKofaxDate(model.VisaExpiry) });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassportNumber", Value = isReinitiate ? string.Empty : (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? string.Empty : model.EmiratesID });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassportExpiryDate", Value = isReinitiate ? string.Empty : (model.EmiratesID != null && model.EmiratesID.StartsWith("784")) ? string.Empty : FormatKofaxDate(model.EmiratesIDExpiry) });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "SubContractorID", Value = isReinitiate ? string.Empty : model.SubContractorID ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VendorID", Value = isReinitiate ? string.Empty : model.VendorID ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectDeptApprovers", Value = isReinitiate ? string.Empty : model.op_seniormanagerEmailid ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "MobileNumber", Value = isReinitiate ? string.Empty : model.Mobilenumber ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PONumber", Value = isReinitiate ? string.Empty : model.PONumber ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectName", Value = isReinitiate ? string.Empty : model.op_projectName ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "CompanyName", Value = isReinitiate ? string.Empty : model.CompanyName ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectStartDate", Value = isReinitiate ? string.Empty : model.projectStartDate.HasValue ? model.projectStartDate.ToString() : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectEndDate", Value = isReinitiate ? string.Empty : model.projectEndDate.HasValue ? model.projectEndDate.ToString() : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassIssueDate", Value = string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "PassExpiryDate", Value = string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "FromDateTime", Value = isReinitiate ? string.Empty : model.FromTime ?? string.Empty }); // 24 hour format
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ToDateTime", Value = isReinitiate ? string.Empty : model.ToTime ?? string.Empty }); // 24 hour format
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectID", Value = isReinitiate ? string.Empty : model.op_projectID ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "ProjectStatus", Value = string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "DepartmentName", Value = string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "Location", Value = isReinitiate ? string.Empty : model.SelectedLocation != null && model.SelectedLocation.Count() > 0 ? string.Join(";", model.SelectedLocation) : "" });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisitorEmailID", Value = isReinitiate ? string.Empty : model.op_visitorEmailid ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "JobTitle", Value = string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VisitingDate", Value = isReinitiate ? string.Empty : FormatKofaxDate(model.DateOfVisit) });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VistingTimeFrom", Value = isReinitiate ? string.Empty : model.FromTime ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VistingTimeTo", Value = isReinitiate ? string.Empty : model.ToTime ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "DEWAID", Value = model.op_dewantid ?? string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VehicleNo", Value = model.withcar ? FormatVehiclePlateNumber(model.EmirateOrCountry, model.PlateCode, model.PlateNumber) : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "VehicleRegistrationDate", Value = model.withcar ? FormatKofaxDate(model.VehicleRegistrationDate) : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "LinkURL", Value = isReinitiate ? uniqueURL : string.Empty });
                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "LinkExpiry", Value = isReinitiate ? DateTime.Now.AddDays(3).ToString("MM/dd/yyyy HH:mm:ss") : string.Empty });

                retval.Add(new Attribute { Type = EpassTypeEnum.Text, Name = "SupportingFiles", Value = isReinitiate ? string.Empty : JsonConvert.SerializeObject(GetAttachment(model).ToArray(), Converter.Settings) });
            }

            return retval;
        }

        /// <summary>
        /// The GetPassApprovalAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="SecurityApproveRejectPassViewModel"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetPassApprovalAttributes(SecurityApproveRejectPassViewModel model)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = model.PassNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassType", Value = model.PassType},
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassIssueDate", Value = FormatKofaxDate(model.PassIssueDate.ToString()) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassExpiryDate", Value = FormatKofaxDate(model.PassExpiryDate.ToString()) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "FromTime", Value = model.FromTime },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ToTime", Value = model.ToTime },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Location", Value = model.StrLocation },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Status", Value = model.PassStatus },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ApprovedByEmail", Value = CurrentPrincipal.EmailAddress },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ApproverType", Value =  model.ApprovalType },
                new Attribute { Type = EpassTypeEnum.Text, Name = "Remarks", Value = model.Comments },
            };
        }

        /// <summary>
        /// The GetPassShareAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="SecurityPassViewModel"/>.</param>
        /// <param name="isPassBlocked">The isPassBlocked<see cref="bool"/>.</param>
        /// <param name="isUserBlocked">The isUserBlocked<see cref="bool"/>.</param>
        /// <param name="isPassUnblocked">The isPassUnblocked<see cref="bool"/>.</param>
        /// <param name="isUserUnblocked">The isUserUnblocked<see cref="bool"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetPassShareAttributes(SecurityPassViewModel model, bool isPassBlocked, bool isUserBlocked, bool isPassUnblocked, bool isUserUnblocked)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = model.passNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "MobileNumber", Value = model.mobile},
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmailAddress", Value = model.email },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SMSLimit", Value = model.smsLimit.ToString() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmailLimit", Value = model.emailLimit.ToString() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "DownloadLimit", Value = model.downloadLimit.ToString() },
                new Attribute { Type = EpassTypeEnum.Text, Name = "IsPassBlocked", Value = isPassBlocked ? "Yes": (isPassUnblocked ?"No": string.Empty) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "IsUserBlocked", Value = isUserBlocked ? "Yes":(isUserUnblocked ?"No": string.Empty) },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesID", Value = isUserBlocked ? model.emiratesId:string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisaNo", Value = isUserBlocked ? model.visaNumber:string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassportNo", Value = isUserBlocked ? model.passportNumber:string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "UserName", Value = isUserBlocked ? CurrentPrincipal.Name:string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ActionBy", Value = CurrentPrincipal.EmailAddress },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ActionComments", Value = model.Blockcomments },
            };
        }

        /// <summary>
        /// The GetUnblockAttributes.
        /// </summary>
        /// <param name="model">The model<see cref="SecurityBlockedUserViewModel"/>.</param>
        /// <returns>The <see cref="List{Attribute}"/>.</returns>
        private List<Attribute> GetUnblockAttributes(SecurityBlockedUserViewModel model)
        {
            return new List<Attribute> {
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassNo", Value = string.Empty},
                new Attribute { Type = EpassTypeEnum.Text, Name = "MobileNumber", Value = string.Empty},
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmailAddress", Value = string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "SMSLimit", Value = string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmailLimit", Value = string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "DownloadLimit", Value = string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "IsPassBlocked", Value = string.Empty },
                new Attribute { Type = EpassTypeEnum.Text, Name = "IsUserBlocked", Value = "No" },
                new Attribute { Type = EpassTypeEnum.Text, Name = "EmiratesID", Value = model.emiratesID},
                new Attribute { Type = EpassTypeEnum.Text, Name = "VisaNo", Value = model.visaNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "PassportNo", Value = model.passportNumber },
                new Attribute { Type = EpassTypeEnum.Text, Name = "UserName", Value =  model.name },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ActionBy", Value = CurrentPrincipal.EmailAddress },
                new Attribute { Type = EpassTypeEnum.Text, Name = "ActionComments", Value = "Unblocked" },
            };
        }

        /// <summary>
        /// The GetMySubPasses.
        /// </summary>
        /// <returns>The <see cref="List{SubPassViewModel}"/>.</returns>
        private List<SubPassViewModel> GetMySubPasses()
        {
            List<SubPassViewModel> retval = new List<SubPassViewModel>();
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                ServiceResponse<KofaxRestResponse> res = null;
                switch (CurrentPrincipal.Role)
                {
                    case Roles.DewaSupplierAdmin:
                        baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.GetPassesname) { Attribute = GetDeptmgrPassAttributes(string.Empty) });
                        res = KofaxRESTService.SubmitKofax(KofaxConstants.GetPendingApprovalEpass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                        break;

                    case Roles.DewaSupplierSecurity:
                        baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.GetPassesname) { Attribute = GetSecurityAdminPassAttributes() });
                        res = KofaxRESTService.SubmitKofax(KofaxConstants.GetPendingApprovalEpass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                        break;

                    default:
                        baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.GetPassesname) { Attribute = GetmyPassAttributes() });
                        res = KofaxRESTService.SubmitKofax(KofaxConstants.GetmyEpass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                        break;
                }

                if (res != null && res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).Any())
                {
                    Value json = res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute att = json?.Attribute.Where(x => x.Name.Equals("Requests")).FirstOrDefault();
                    if (att != null)
                    {
                        retval = JsonConvert.DeserializeObject<List<SubPassViewModel>>(System.Text.RegularExpressions.Regex.Unescape(att.Value));

                        SetProperStatus(retval);
                    }
                }

                //var lstSubPasses = KofaxRESTService.GetMyPasses(CurrentPrincipal.EmailAddress);
                //var json = lstSubPasses.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).FirstOrDefault();
                //var att = json?.Attribute.Where(x => x.Name.Equals("Requests")).FirstOrDefault();
                //if (att != null)
                //{
                //    retval = JsonConvert.DeserializeObject<List<SubPassViewModel>>(System.Text.RegularExpressions.Regex.Unescape(att.Value));

                //    SetProperStatus(retval);
                //}
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return retval;
        }

        /// <summary>
        /// The GetPassByPassNumber.
        /// </summary>
        /// <param name="passNumber">The passNumber<see cref="string"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <returns>The <see cref="List{ePassSubContractor}"/>.</returns>
        private BasePassDetailModel GetPassByPassNumber(string passNumber = "", string keyword = "")
        {
            BasePassDetailModel retval = new BasePassDetailModel();
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                ServiceResponse<KofaxRestResponse> res = null;
                if (!string.IsNullOrWhiteSpace(passNumber))
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.GetPassRequest) { Attribute = GetPassDetailAttributes(passNumber) });
                    res = KofaxRESTService.SubmitKofax(KofaxConstants.GetPassDetailByPassNo, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                }
                else if (!string.IsNullOrWhiteSpace(keyword))
                {
                    baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.SearchPassRequest) { Attribute = SearchPassDetailAttributes(keyword) });
                    res = KofaxRESTService.SubmitKofax(KofaxConstants.SearchPassDetailByPassNo, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                }

                if (res != null && res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).Any())
                {
                    Value json = res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute atrrrequest = json?.Attribute.Where(x => x.Name.Equals("Requests")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute attrsubrequest = json?.Attribute.Where(x => x.Name.Equals("SubRequests")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute attrAttachments = json?.Attribute.Where(x => x.Name.Equals("Files")).FirstOrDefault();
                    if (atrrrequest != null)
                    {
                        retval.MainPass = JsonConvert.DeserializeObject<List<BaseMainPassDetailModel>>(System.Text.RegularExpressions.Regex.Unescape(atrrrequest.Value));
                    }
                    if (attrsubrequest != null)
                    {
                        retval.SubPasses = JsonConvert.DeserializeObject<List<BaseSubPassDetailModel>>(System.Text.RegularExpressions.Regex.Unescape(attrsubrequest.Value));
                    }
                    if (attrAttachments != null)
                    {
                        retval.Attachments = JsonConvert.DeserializeObject<List<BasePassDetailAttachment>>(System.Text.RegularExpressions.Regex.Unescape(attrAttachments.Value));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return retval;
        }

        /// <summary>
        /// The GetSubPassAttachment.
        /// </summary>
        /// <param name="model">The model<see cref="ElectronicPass"/>.</param>
        /// <returns>The <see cref="List{KofaxFileViewModel}"/>.</returns>
        private List<KofaxFileViewModel> GetSubPassAttachment(ElectronicPass model)
        {
            List<KofaxFileViewModel> fileViewModel = new List<KofaxFileViewModel>();
            if (model.DevicePicbytes != null && model.DevicePicbytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.DevicePicbytes),
                    FileContentType = model.DevicePic.ContentType.ToString(),
                    FileCategory = "Device",
                    FileExtension = model.DevicePic.GetTrimmedFileExtension(),
                    FileName = model.DevicePic.FileName.ToString(),
                    FileType = model.DevicePic.GetTrimmedFileExtension(),
                });
            }
            return fileViewModel;
        }

        /// <summary>
        /// The GetSubPassMaterialAttachment.
        /// </summary>
        /// <param name="model">The model<see cref="MaterialPass"/>.</param>
        /// <returns>The <see cref="List{KofaxFileViewModel}"/>.</returns>
        private List<KofaxFileViewModel> GetSubPassMaterialAttachment(MaterialPass model)
        {
            List<KofaxFileViewModel> fileViewModel = new List<KofaxFileViewModel>();
            if (model.LPOAttachmentbytes != null && model.LPOAttachmentbytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.LPOAttachmentbytes),
                    FileContentType = model.LPOAttachment.ContentType.ToString(),
                    FileCategory = "LPO",
                    FileExtension = model.LPOAttachment.GetTrimmedFileExtension(),
                    FileName = model.LPOAttachment.FileName.ToString(),
                    FileType = model.LPOAttachment.GetTrimmedFileExtension(),
                });
            }
            if (model.DeliveryNoteattachmentbytes != null && model.DeliveryNoteattachmentbytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.DeliveryNoteattachmentbytes),
                    FileContentType = model.DeliveryNoteattachment.ContentType.ToString(),
                    FileCategory = "DeliveryNote",
                    FileExtension = model.DeliveryNoteattachment.GetTrimmedFileExtension(),
                    FileName = model.DeliveryNoteattachment.FileName.ToString(),
                    FileType = model.DeliveryNoteattachment.GetTrimmedFileExtension(),
                });
            }
            if (model.ContractorSiteAttachmentbytes != null && model.ContractorSiteAttachmentbytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.ContractorSiteAttachmentbytes),
                    FileContentType = model.ContractorSiteAttachment.ContentType.ToString(),
                    FileCategory = "ContractorSite",
                    FileExtension = model.ContractorSiteAttachment.GetTrimmedFileExtension(),
                    FileName = model.ContractorSiteAttachment.FileName.ToString(),
                    FileType = model.ContractorSiteAttachment.GetTrimmedFileExtension(),
                });
            }
            return fileViewModel;
        }

        /// <summary>
        /// The GetAttachment.
        /// </summary>
        /// <param name="model">The model<see cref="PermanentPass"/>.</param>
        /// <returns>The <see cref="List{KofaxFileViewModel}"/>.</returns>
        private List<KofaxFileViewModel> GetAttachment(PermanentPass model)
        {
            List<KofaxFileViewModel> fileViewModel = new List<KofaxFileViewModel>();
            if (model.SinglePass_Visa_Bytes != null && model.SinglePass_Visa_Bytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.SinglePass_Visa_Bytes),
                    FileContentType = model.SinglePass_Visa.ContentType.ToString(),
                    FileCategory = "Visa",
                    FileExtension = model.SinglePass_Visa.GetTrimmedFileExtension(),
                    FileName = model.SinglePass_Visa.FileName.ToString(),
                    FileType = model.SinglePass_Visa.GetTrimmedFileExtension(),
                });
            }
            if (model.SinglePass_Photo_Bytes != null && model.SinglePass_Photo_Bytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.SinglePass_Photo_Bytes),
                    FileContentType = model.SinglePass_Photo.ContentType.ToString(),
                    FileCategory = "Photo",
                    FileExtension = model.SinglePass_Photo.GetTrimmedFileExtension(),
                    FileName = model.SinglePass_Photo.FileName.ToString(),
                    FileType = model.SinglePass_Photo.GetTrimmedFileExtension(),
                });
            }
            if (model.SinglePass_Passport_Bytes != null && model.SinglePass_Passport_Bytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.SinglePass_Passport_Bytes),
                    FileContentType = model.SinglePass_Passport.ContentType.ToString(),
                    FileCategory = "Passport",
                    FileExtension = model.SinglePass_Passport.GetTrimmedFileExtension(),
                    FileName = model.SinglePass_Passport.FileName.ToString(),
                    FileType = model.SinglePass_Passport.GetTrimmedFileExtension(),
                });
            }
            if (model.SinglePass_EID_Bytes != null && model.SinglePass_EID_Bytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.SinglePass_EID_Bytes),
                    FileContentType = model.SinglePass_EmiratesID.ContentType.ToString(),
                    FileCategory = "EmirateID",
                    FileExtension = model.SinglePass_EmiratesID.GetTrimmedFileExtension(),
                    FileName = model.SinglePass_EmiratesID.FileName.ToString(),
                    FileType = model.SinglePass_EmiratesID.GetTrimmedFileExtension(),
                });
            }
            if (model.SinglePass_VehicleRegistration_Bytes != null && model.SinglePass_VehicleRegistration_Bytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.SinglePass_VehicleRegistration_Bytes),
                    FileContentType = model.SinglePass_VehicleRegistration.ContentType.ToString(),
                    FileCategory = "Vehicle",
                    FileExtension = model.SinglePass_VehicleRegistration.GetTrimmedFileExtension(),
                    FileName = model.SinglePass_VehicleRegistration.FileName.ToString(),
                    FileType = model.SinglePass_VehicleRegistration.GetTrimmedFileExtension(),
                });
            }
            if (model.SinglePass_DrivingLicense_Bytes != null && model.SinglePass_DrivingLicense_Bytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.SinglePass_DrivingLicense_Bytes),
                    FileContentType = model.SinglePass_DrivingLicense.ContentType.ToString(),
                    FileCategory = "DrivingLicense",
                    FileExtension = model.SinglePass_DrivingLicense.GetTrimmedFileExtension(),
                    FileName = model.SinglePass_DrivingLicense.FileName.ToString(),
                    FileType = model.SinglePass_DrivingLicense.GetTrimmedFileExtension(),
                });
            }
            if (model.SinglePass_DewaID_Bytes != null && model.SinglePass_DewaID_Bytes.Count() > 0)
            {
                fileViewModel.Add(new KofaxFileViewModel
                {
                    FileContent = BitConverter.ToString(model.SinglePass_DewaID_Bytes),
                    FileContentType = model.op_DEWAId.ContentType.ToString(),
                    FileCategory = "DEWAID",
                    FileExtension = model.op_DEWAId.GetTrimmedFileExtension(),
                    FileName = model.op_DEWAId.FileName.ToString(),
                    FileType = model.op_DEWAId.GetTrimmedFileExtension(),
                });
            }

            return fileViewModel;
        }

        /// <summary>
        /// The SetProperStatus.
        /// </summary>
        /// <param name="lst">The lst<see cref="List{SubPassViewModel}"/>.</param>
        private void SetProperStatus(List<SubPassViewModel> lst)
        {
            foreach (SubPassViewModel sp in lst)
            {
                switch (sp.subPassStatus.ToLower())
                {
                    case "initiated":
                        sp.subPassStatus = "Dept Coordinator";
                        break;

                    case "dept approved":
                        sp.subPassStatus = "Security Team";
                        break;

                    case "security approved":
                        sp.subPassStatus = "Completed";
                        break;

                    default:
                        //keep whatever is set in kofax repository
                        break;
                }
            }
        }

        /// <summary>
        /// The GetMainPasses.
        /// </summary>
        /// <param name="dptmgr">The dptmgr<see cref="string"/>.</param>
        /// <returns>The <see cref="List{BasePassViewModel}"/>.</returns>
        private List<BasePassViewModel> GetMainPasses(string dptmgr)
        {
            List<BasePassViewModel> retval = new List<BasePassViewModel>();
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                ServiceResponse<KofaxRestResponse> res = null;
                PassRole passRole = string.IsNullOrWhiteSpace(dptmgr) ? (CurrentPrincipal.Role.Equals(Roles.DewaSupplierAdmin) ? PassRole.Dptmgr : (CurrentPrincipal.Role.Equals(Roles.DewaonedayInitiator) ? PassRole.Dptmgr : (CurrentPrincipal.Role.Equals(Roles.DewaSupplierSecurity) ? PassRole.SecurityAdmin : PassRole.Individual))) : PassRole.Dptmgr;

                switch (passRole)
                {
                    case PassRole.Dptmgr:
                        baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.Pendingapprovalname) { Attribute = GetDeptmgrPassAttributes(dptmgr) });
                        res = KofaxRESTService.SubmitKofax(KofaxConstants.GetPendingApprovalEpass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                        break;

                    case PassRole.SecurityAdmin:
                        baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.Pendingapprovalname) { Attribute = GetSecurityAdminPassAttributes() });
                        res = KofaxRESTService.SubmitKofax(KofaxConstants.GetPendingApprovalEpass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                        break;

                    default:
                        baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.GetPassesname) { Attribute = GetmyPassAttributes() });
                        res = KofaxRESTService.SubmitKofax(KofaxConstants.GetmyEpass, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
                        break;
                }

                if (res != null && res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).Any())
                {
                    Value json = res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute att = json?.Attribute.Where(x => x.Name.Equals("Requests")).FirstOrDefault();
                    if (att != null)
                    {
                        retval = JsonConvert.DeserializeObject<List<BasePassViewModel>>(System.Text.RegularExpressions.Regex.Unescape(att.Value));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return retval;
        }

        /// <summary>
        /// The GetSubPasses.
        /// </summary>
        /// <param name="lp">The lp<see cref="List{SecurityPassViewModel}"/>.</param>
        /// <param name="Alllp">The Alllp<see cref="List{BasePassViewModel}"/>.</param>
        private void GetSubPasses(List<SecurityPassViewModel> lp, List<BasePassViewModel> Alllp)
        {
            if (lp == null || lp.Count < 0)
            {
                return;
            }

            List<SecurityPassViewModel> spl = new List<SecurityPassViewModel>();
            IEnumerable<IGrouping<string, BasePassViewModel>> grppass = Alllp.GroupBy(x => x.subPassMainPassNo);
            grppass.ToList().ForEach(x =>
            {
                IEnumerable<SecurityPassViewModel> lstpasses = lp.Where(y => y.passNumber.Equals(x.Key));
                if (lstpasses.Any())
                {
                    SecurityPassViewModel p = lstpasses.FirstOrDefault();
                    p.MySubPasses = Alllp.Where(z => z.subPassMainPassNo.Equals(p.passNumber)).ToList() ?? new List<BasePassViewModel>();
                    p.SubPasses = Alllp.Where(z => z.subPassMainPassNo.Equals(p.passNumber)).ToList().Any() ? Alllp.Where(z => z.subPassMainPassNo.Equals(p.passNumber)).ToList().Select(y => GenericExtensions.MapProperties<SubpassDetails>(y)).ToList() : new List<SubpassDetails>();
                    x.ToList().ForEach(sp =>
                            spl.Add(new SecurityPassViewModel()
                            {
                                name = p.name,
                                passNumber = sp.subPassRequestID,
                                mainpassNumber = p.passNumber,
                                Subpass = true,
                                passType = Translate.Text("Epass." + sp.subPassNewPassType),
                                passTypeText = sp.subPassNewPassType,
                                passExpiryDate = DateTime.Parse(sp.subPassValidTo),
                                strpassExpiryDate = FormatEpassstrDate(DateTime.Parse(sp.subPassValidTo).ToString("MMMM dd, yyyy")),
                                passIssueDate = DateTime.Parse(sp.subPassValidFrom),
                                CreatedDate = DateTime.Parse(sp.subPassCreatedOn),
                                status = assignStatus(sp.subPassStatus, sp.subPassValidTo, p.IsBlocked),
                                strstatus = Translate.Text("epassstatus." + assignStatus(sp.subPassStatus, sp.subPassValidTo, p.IsBlocked).ToString().ToLower()),
                                strclass = assignStatus(sp.subPassStatus, p.passExpiryDate, p.IsBlocked).ToString(),
                                SeniorManagerEmail = p.SeniorManagerEmail,
                                fromTime = DateTime.Parse(sp.subPassValidFrom).TimeOfDay.ToString(),
                                toTime = DateTime.Parse(sp.subPassValidTo).TimeOfDay.ToString(),
                                emiratesId = p.emiratesId,
                                visaNumber = p.visaNumber,
                                passportNumber = p.passportNumber,
                                pendingwith = sp.subPassStatus.ToLower().Equals("initiated") ? sp.subPassDepartmentApprover : (sp.subPassStatus.ToLower().Equals("dept approved") ? sp.subPassSecurityApprovers : (sp.subPassStatus.ToLower().Equals("security approved") ? "Completed" : "Unknown"))
                            }));
                }
            });
            if (spl.Count > 0)
            {
                lp.AddRange(spl);
            }
        }

        /// <summary>
        /// The AppendSubPasses.
        /// </summary>
        /// <param name="lp">The lp<see cref="List{SecurityPassViewModel}"/>.</param>
        private void AppendSubPasses(List<SecurityPassViewModel> lp)
        {
            List<SubPassViewModel> lstSubpasses = GetMySubPasses();
            //switch (CurrentPrincipal.Role)
            //{
            //    case Roles.DewaSupplierAdmin:
            //        lstSubpasses = GetManagerPendingSubPasses();
            //        break;
            //    case Roles.DewaSupplierSecurity:
            //        lstSubpasses = GetSecurityPendingSubPasses();
            //        break;
            //    default:
            //        lstSubpasses = GetMySubPasses();
            //        break;
            //}
            if (lstSubpasses == null || lstSubpasses.Count < 0)
            {
                return;
            }

            List<SecurityPassViewModel> spl = new List<SecurityPassViewModel>();

            IEnumerable<IGrouping<string, SubPassViewModel>> grppass = lstSubpasses.GroupBy(x => x.subPassMainPassNo);

            grppass.ToList().ForEach(x =>
            {
                IEnumerable<SecurityPassViewModel> lstpasses = lp.Where(y => y.passNumber.Equals(x.Key));
                if (lstpasses.Any())
                {
                    SecurityPassViewModel p = lstpasses.FirstOrDefault();
                    p.SubPasses = lstSubpasses.Where(z => z.subPassMainPassNo.Equals(p.passNumber)).ToList().Any() ? lstSubpasses.Where(z => z.subPassMainPassNo.Equals(p.passNumber)).ToList().Select(y => GenericExtensions.MapProperties<SubpassDetails>(y)).ToList() : new List<SubpassDetails>();
                    x.ToList().ForEach(sp =>
                   spl.Add(new SecurityPassViewModel()
                   {
                       name = p.name,
                       passNumber = sp.subPassRequestID,
                       mainpassNumber = p.passNumber,
                       Subpass = true,
                       passType = Translate.Text("Epass." + sp.subPassNewPassType),
                       passTypeText = sp.subPassNewPassType,
                       passExpiryDate = DateTime.Parse(sp.subPassValidTo),
                       strpassExpiryDate = FormatEpassstrDate(DateTime.Parse(sp.subPassValidTo).ToString("MMMM dd, yyyy")),
                       passIssueDate = DateTime.Parse(sp.subPassValidFrom),
                       CreatedDate = p.CreatedDate,
                       status = assignStatus(sp.subPassStatus, sp.subPassValidTo, p.IsBlocked),
                       strstatus = Translate.Text("epassstatus." + assignStatus(sp.subPassStatus, sp.subPassValidTo, p.IsBlocked).ToString().ToLower()),
                       strclass = assignStatus(sp.subPassStatus, p.passExpiryDate, p.IsBlocked).ToString(),
                       SeniorManagerEmail = p.SeniorManagerEmail,
                       fromTime = DateTime.Parse(sp.subPassValidFrom).TimeOfDay.ToString(),
                       toTime = DateTime.Parse(sp.subPassValidTo).TimeOfDay.ToString(),
                       pendingwith = sp.subPassStatus.ToLower().Equals("initiated") ? sp.subPassDepartmentApprover : (sp.subPassStatus.ToLower().Equals("dept approved") ? sp.subPassSecurityApprovers : (sp.subPassStatus.ToLower().Equals("security approved") ? "Completed" : "Unknown"))
                   }));
                }
            }
            );
            if (spl.Count > 0)
            {
                lp.AddRange(spl);
            }
        }

        /// <summary>
        /// The ValidateOneDayPass.
        /// </summary>
        /// <param name="linkURL">The linkURL<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ValidateOneDayPass(string linkURL)
        {
            List<BasePassViewModel> retval = new List<BasePassViewModel>();
            try
            {
                BaseKofaxViewModel baseKofaxViewModel = new BaseKofaxViewModel();
                ServiceResponse<KofaxRestResponse> res = null;
                Parameter param = new Parameter(KofaxConstants.OneDayValidationIN)
                {
                    Attribute = new List<Attribute>()
                };
                param.Attribute.Add(new Attribute() { Name = "LinkURL", Type = EpassTypeEnum.Text, Value = linkURL });

                baseKofaxViewModel.Parameters.Add(param);
                res = KofaxRESTService.SubmitKofax(KofaxConstants.OneDayPassValidateMethod, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));

                if (res != null && res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null &&
                    res.Payload.Values.Length > 0 && res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).Any())
                {
                    Value json = res.Payload.Values.Where(x => x.TypeName.Equals("T000121_Requests_OUT")).FirstOrDefault();
                    DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute att = json?.Attribute.Where(x => x.Name.Equals("Status")).FirstOrDefault();
                    if (att != null)
                    {
                        if (att.Value.ToLower().Equals("not valid"))
                        {
                            return false;
                        }

                        DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute req = json?.Attribute.Where(x => x.Name.Equals("Requests")).FirstOrDefault();
                        if (req != null && !string.IsNullOrEmpty(req.Value))
                        {
                            BasePassViewModel passVM = JsonConvert.DeserializeObject<List<BasePassViewModel>>(req.Value).FirstOrDefault();
                            if (string.IsNullOrEmpty(passVM.ePassVisitingDate)) { return true; }
                        }
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return false;
        }
    }
}