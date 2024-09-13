using DEWAXP.Feature.GeneralServices.Models.VillaCostExemption;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Utils;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests.VillaCostExemption;
using DEWAXP.Foundation.Integration.Responses.VillaCostExemption;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Attachlist = DEWAXP.Feature.GeneralServices.Models.VillaCostExemption.Attachlist;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    [TwoPhaseAuthorize]
    public class VillaCostExemptionController : BaseController
    {
        #region Variables

        private const string ExistingApplications = "DD11";
        private const string ApplicationDetails = "AD11";
        private const string EditApplicationDetails = "EAD11";
        private const string SaveStage = "SS2233";
        private const string ViewPath = "~/Views/Feature/GeneralServices/VillaCostExemption/{0}.cshtml";
        private const string SUBMITTED = "SUBMITTED";
        private const string SAVED = "SAVED";
        private const string REJECTED = "REJECTED";
        private const string APPROVED = "APPROVED";
        private const string UNKNOWN = "UNKNOWN";
        private const string CountryList = "CountryList";
        private const string OwnerAttachment = "OwnerAttachment";
        private const string SearchOptions = "SearchOptions";
        private long FileSize = (5 * 1024 * 1024);
        private string[] FileType = { ".PDF", ".JPG", ".JPEG", ".PNG", ".BMP", };

        #endregion Variables

        #region Action

        #region Dashboard

        [HttpGet]
        public ActionResult Dashboard()
        {
            DashboardPageModel model = new DashboardPageModel();
            try
            {
                if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                }
                CacheProvider.Remove(ExistingApplications);
                CacheProvider.Remove(EditApplicationDetails);
                CacheProvider.Remove(ApplicationDetails);

                model = GetExistingApplications(true);
                ListDataSources searchDataSource = null;
                searchDataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.HH_VILLA_SEARCH_OPTIONS));
                if (searchDataSource != null)
                {
                    model.SearchOptions = searchDataSource.Items.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).Distinct().ToList();
                }
                if (model == null)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text(ErrorMessages.WEBSERVICE_ERROR));
                }
                CacheProvider.Store(SearchOptions, new CacheItem<ListDataSources>(searchDataSource, TimeSpan.FromMinutes(40)));
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", ex.Message);
            }

            return View(string.Format(ViewPath, "DashboardPage"), model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Dashboard(DashboardPageModel model)
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            ListDataSources searchDataSource = null;

            if (CacheProvider.TryGet(SearchOptions, out searchDataSource))
            {
                if (searchDataSource != null)
                {
                    model.SearchOptions = searchDataSource.Items.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();
                }
            }
            try
            {
                DashboardPageModel _cacheModel = new DashboardPageModel();
                _cacheModel = GetExistingApplications(false);
                model.OwnerTypeList = _cacheModel.OwnerTypeList;
                model.StatusTypeList = _cacheModel.StatusTypeList;
                model.CustomerDetails = _cacheModel.CustomerDetails;
                switch (model.selectedSearchType)
                {
                    case "1":
                        if (!string.IsNullOrWhiteSpace(model.SearchText) && !string.IsNullOrWhiteSpace(model.selectedSearchType))
                            model.CustomerDetails = _cacheModel.CustomerDetails.Where(x => x.Number.ToLower().Contains(model.SearchText.Trim().ToLower())).ToList();
                        break;

                    case "2":
                        if (!string.IsNullOrWhiteSpace(model.SearchText) && !string.IsNullOrWhiteSpace(model.selectedSearchType))
                            model.CustomerDetails = _cacheModel.CustomerDetails.Where(x => x.OwnerTypeDescription.ToLower().Contains(model.SearchText.ToLower())).ToList();
                        break;

                    case "3":
                        if (!string.IsNullOrWhiteSpace(model.SearchText) && !string.IsNullOrWhiteSpace(model.selectedSearchType))
                            model.CustomerDetails = _cacheModel.CustomerDetails.Where(x => x.Reference.ToLower().Contains(model.SearchText.Trim().ToLower())).ToList();
                        break;

                    default:
                        if (!string.IsNullOrWhiteSpace(model.SearchText) && !string.IsNullOrWhiteSpace(model.selectedSearchType))
                            model.CustomerDetails = _cacheModel.CustomerDetails.Where(x => x.StatusDesc.ToLower().Contains(model.SearchText.ToLower())).ToList();
                        break;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View(string.Format(ViewPath, "DashboardPage"), model);
        }

        #endregion Dashboard

        #region Step 1 (New)

        public ActionResult NewApplication()
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            string errorMessage;
            CacheProvider.Remove(CountryList);
            CacheProvider.Remove(OwnerAttachment);
            ApplicationViewModel model = new ApplicationViewModel() { Stage = 1 };
            try
            {
                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                }
                NewApplicationRequest applicationRequest = new NewApplicationRequest { villarequest = new villarequest() };
                model.Countrylists = GetCountryList();
                var applicationResponse = VillaCostExemptionClient.GetApplicationResources(applicationRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);

                if (applicationResponse.Succeeded && applicationResponse.Payload != null)
                {
                    model.BPList = applicationResponse.Payload.Bpdetails.GroupBy(a => a.Bpnumber).Select(x => x.First()).ToList().Select(c => new SelectListItem() { Text = c.Bpname + "-" + c.Bpnumber, Value = c.Bpnumber }).ToList();
                    model.PropertyOwnerTypeList = applicationResponse.Payload.Ownertypes.Select(x => new SelectListItem() { Text = x.Value, Value = x.Key }).ToList();

                    var bpdetails = applicationResponse.Payload.Bpdetails.GroupBy(a => a.Bpnumber).Select(x => x.First()).Select(p => new BPartnerDetail() { EID = p.Bpemiratesid ?? string.Empty, Email = p.Email, EN = p.Bpestimate, Mobile = p.Mobile, Name = p.Bpname, Number = p.Bpnumber, RN = p.Bpapplicationreferencenumber, Relationship = p.Relationship, Telephone = p.Telephone, Nationality = p.Nationality, Valid = p.Valid }).ToList();

                    model.BPDetails = Newtonsoft.Json.JsonConvert.SerializeObject(bpdetails, Newtonsoft.Json.Formatting.None);
                    foreach (var i in applicationResponse.Payload.Bpdetails)
                    {
                        if (!model.Applications.Where(x => x.Id.Equals(i.Bpnumber)).Any())
                        {
                            model.Applications.Add(new BPFrontEnd() { Id = i.Bpnumber, Values = new List<string>() });
                        }
                        if (!model.Estimates.Where(x => x.Id.Equals(i.Bpapplicationreferencenumber)).Any())
                        {
                            model.Estimates.Add(new BPFrontEnd() { Id = i.Bpapplicationreferencenumber, Values = new List<string>() });
                        }
                    }

                    var existingApps = GetExistingApplications();
                    if (existingApps != null && existingApps.CustomerDetails.Count > 0)
                    {
                        foreach (var d in existingApps.CustomerDetails)
                        {
                            var toRemove = model.Estimates.Where(x => x.Id.Equals(d.Number)).FirstOrDefault();
                            if (toRemove != null)
                            {
                                //model.Estimates.Remove(toRemove);
                                toRemove.IsSubmittedApp = true;
                            }
                        }
                    }

                    foreach (var a in model.Applications)
                    {
                        var itm = applicationResponse.Payload.Bpdetails.Where(x => x.Bpnumber.Equals(a.Id)).ToList();
                        if (itm != null && itm.Count > 0) { a.Values.AddRange(itm.Select(x => x.Bpapplicationreferencenumber).ToList()); }
                    }

                    foreach (var b in model.Estimates)
                    {
                        var itm = applicationResponse.Payload.Bpdetails.Where(x => x.Bpapplicationreferencenumber.Equals(b.Id)).ToList();
                        if (itm != null && itm.Count > 0) { b.Values.AddRange(itm.Select(x => x.Bpestimate).ToList()); }
                    }

                    CacheProvider.Store(ApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, applicationResponse.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", ex.Message);
            }

            return View(string.Format(ViewPath, "NewApplication"), model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NewApplication1(ApplicationViewModel model)
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            try
            {
                ApplicationViewModel _cacheModel = new ApplicationViewModel();
                if (CacheProvider.TryGet<ApplicationViewModel>(ApplicationDetails, out _cacheModel))
                {
                    model.BPList = _cacheModel.BPList;
                    model.PropertyOwnerTypeList = _cacheModel.PropertyOwnerTypeList;
                    model.BPDetails = _cacheModel.BPDetails;
                    model.Estimates = _cacheModel.Estimates;
                    model.Applications = _cacheModel.Applications;
                }
                switch (model.Stage)
                {
                    case 1:

                        #region Step 1

                        var owners = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OwnerDetail>>(model.OwnersJson);
                        if (owners != null && owners.Count > 0) { model.Owners = owners; }
                        NewApplicationRequest reqObjectStage1 = new NewApplicationRequest()
                        {
                            villarequest = new villarequest()
                            {
                                processcode = "03", //save application
                                applicationnumber = model.ApplicationNumber,
                                customernumber = model.BusinessPartner,
                                estimate = model.EstimateNumber,
                                ownertype = model.OwnerType,
                                //remarks = "INITIATING APPLICATION",
                                ownerdetails = model.Owners.Select(x => new ownerdetails() { email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = x.PassportExpiry, passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                            }
                        };
                        var applicationResponseStage1 = VillaCostExemptionClient.SaveApplication(reqObjectStage1, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                        if (applicationResponseStage1 != null && applicationResponseStage1.Succeeded && applicationResponseStage1.Payload != null && applicationResponseStage1.Payload.Responsecode.Equals("000"))
                        {
                            model.Stage = 2;
                            if (applicationResponseStage1.Payload.Applicationreference != null && !string.IsNullOrWhiteSpace(applicationResponseStage1.Payload.Applicationreference.ToString()))
                            {
                                model.ApplicationReferenceNumber = applicationResponseStage1.Payload.Applicationreference.ToString();
                            }
                            CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                            return View(string.Format(ViewPath, "NewApplicationStep2"), model);
                        }
                        else
                        {
                            ModelState.AddModelError("", applicationResponseStage1.Message);
                        }
                        return View(string.Format(ViewPath, "NewApplication"), model);

                    #endregion Step 1

                    case 2: //Save

                        #region Step 2

                        UploadFileRequest uploadFileRequest;
                        ApplicationViewModel savedModelStage2;
                        string error = null;
                        if (CacheProvider.TryGet<ApplicationViewModel>(SaveStage, out savedModelStage2))
                        {
                            if (model.AttachmentLists != null && model.AttachmentLists.Count > 0)
                            {
                                foreach (var itemAttach in model.AttachmentLists)
                                {
                                    #region Construction contract signed by (Owner, Contractor, Consultant)

                                    if (itemAttach.ConstructionContractSigned != null && itemAttach.ConstructionContractSigned.ContentLength > 0)
                                    {
                                        if (CustomeAttachmentIsValid(itemAttach.ConstructionContractSigned, FileSize, out error, FileType))
                                        {
                                            uploadFileRequest = new UploadFileRequest();
                                            using (var memoryStream_8 = new MemoryStream())
                                            {
                                                itemAttach.ConstructionContractSigned.InputStream.CopyTo(memoryStream_8);
                                                uploadFileRequest.attachrequest = new Attachrequest
                                                {
                                                    action = "C",
                                                    itemnumber = "0010",
                                                    applicationreferencenumber = savedModelStage2.ApplicationReferenceNumber,
                                                    content = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                                    mimetype = itemAttach.ConstructionContractSigned.ContentType,
                                                    filename = savedModelStage2.ApplicationReferenceNumber + "0010" + AttachmentType.ConstructionContractSigned
                                                };
                                                var responseFile1 = uploadAttachment(uploadFileRequest);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, Translate.Text("VE_File1RequiredField"));
                                    }

                                    #endregion Construction contract signed by (Owner, Contractor, Consultant)

                                    #region Construction 02 (optional)

                                    if (itemAttach.ConstructionContract_02 != null && itemAttach.ConstructionContract_02.ContentLength > 0)
                                    {
                                        if (CustomeAttachmentIsValid(itemAttach.ConstructionContract_02, FileSize, out error, FileType))
                                        {
                                            using (var memoryStream_8 = new MemoryStream())
                                            {
                                                uploadFileRequest = new UploadFileRequest();
                                                itemAttach.ConstructionContract_02.InputStream.CopyTo(memoryStream_8);
                                                uploadFileRequest.attachrequest = new Attachrequest
                                                {
                                                    action = "C",
                                                    itemnumber = "0010",
                                                    applicationreferencenumber = savedModelStage2.ApplicationReferenceNumber,
                                                    content = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                                    mimetype = itemAttach.ConstructionContract_02.ContentType,
                                                    filename = savedModelStage2.ApplicationReferenceNumber + "0010" + AttachmentType.ConstructionContract_02
                                                };
                                                var responseFile2 = uploadAttachment(uploadFileRequest);
                                            }
                                        }
                                    }

                                    #endregion Construction 02 (optional)

                                    #region Construction 03 (optional)

                                    if (itemAttach.ConstructionContract_03 != null && itemAttach.ConstructionContract_03.ContentLength > 0)
                                    {
                                        if (CustomeAttachmentIsValid(itemAttach.ConstructionContract_03, FileSize, out error, FileType))
                                        {
                                            using (var memoryStream_8 = new MemoryStream())
                                            {
                                                uploadFileRequest = new UploadFileRequest();
                                                itemAttach.ConstructionContract_03.InputStream.CopyTo(memoryStream_8);
                                                uploadFileRequest.attachrequest = new Attachrequest
                                                {
                                                    action = "C",
                                                    itemnumber = "0010",
                                                    applicationreferencenumber = savedModelStage2.ApplicationReferenceNumber,
                                                    content = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                                    mimetype = itemAttach.ConstructionContract_03.ContentType,
                                                    filename = savedModelStage2.ApplicationReferenceNumber + "0010" + AttachmentType.ConstructionContract_03
                                                };
                                                var responseFile3 = uploadAttachment(uploadFileRequest);
                                            }
                                        }
                                    }

                                    #endregion Construction 03 (optional)

                                    #region Construction 04 (optional)

                                    if (itemAttach.ConstructionContract_04 != null && itemAttach.ConstructionContract_04.ContentLength > 0)
                                    {
                                        if (CustomeAttachmentIsValid(itemAttach.ConstructionContract_04, FileSize, out error, FileType))
                                        {
                                            using (var memoryStream_8 = new MemoryStream())
                                            {
                                                uploadFileRequest = new UploadFileRequest();
                                                itemAttach.ConstructionContract_04.InputStream.CopyTo(memoryStream_8);
                                                uploadFileRequest.attachrequest = new Attachrequest
                                                {
                                                    action = "C",
                                                    itemnumber = "0010",
                                                    applicationreferencenumber = savedModelStage2.ApplicationReferenceNumber,
                                                    content = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                                    mimetype = itemAttach.ConstructionContract_04.ContentType,
                                                    filename = savedModelStage2.ApplicationReferenceNumber + "0010" + AttachmentType.ConstructionContract_04
                                                };
                                                var responseFile4 = uploadAttachment(uploadFileRequest);
                                            }
                                        }
                                    }

                                    #endregion Construction 04 (optional)

                                    #region Completion Letter (mandatory if more then one construction contract is uploaded)

                                    if (itemAttach.CompletionLetter != null && itemAttach.CompletionLetter.ContentLength > 0)
                                    {
                                        if (CustomeAttachmentIsValid(itemAttach.CompletionLetter, FileSize, out error, FileType))
                                        {
                                            using (var memoryStream_8 = new MemoryStream())
                                            {
                                                uploadFileRequest = new UploadFileRequest();
                                                itemAttach.CompletionLetter.InputStream.CopyTo(memoryStream_8);
                                                uploadFileRequest.attachrequest = new Attachrequest
                                                {
                                                    action = "C",
                                                    itemnumber = "0010",
                                                    applicationreferencenumber = savedModelStage2.ApplicationReferenceNumber,
                                                    content = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                                    mimetype = itemAttach.CompletionLetter.ContentType,
                                                    filename = savedModelStage2.ApplicationReferenceNumber + "0010" + AttachmentType.CompletionLetter
                                                };
                                                var responseFile5 = uploadAttachment(uploadFileRequest);
                                            }
                                        }
                                    }

                                    #endregion Completion Letter (mandatory if more then one construction contract is uploaded)
                                }
                            }
                        }
                        CacheProvider.Store<ApplicationViewModel>(SaveStage, new CacheItem<ApplicationViewModel>(model, TimeSpan.FromMinutes(5)));
                        return View(string.Format(ViewPath, "ApplicationStep3"), model);

                    #endregion Step 2

                    case 3: //Submit

                        #region Step 3

                        ApplicationViewModel savedModelStage3;
                        if (CacheProvider.TryGet<ApplicationViewModel>(SaveStage, out savedModelStage3))
                        {
                            savedModelStage3.CustomerComments = model.CustomerComments;
                            NewApplicationRequest reqObject = new NewApplicationRequest()
                            {
                                villarequest = new villarequest()
                                {
                                    processcode = "03",
                                    applicationnumber = savedModelStage3.ApplicationNumber,
                                    customernumber = savedModelStage3.BusinessPartner,
                                    estimate = savedModelStage3.EstimateNumber,
                                    ownertype = savedModelStage3.OwnerType,
                                    remarks = savedModelStage3.CustomerComments,
                                    ownerdetails = savedModelStage3.Owners.Select(x => new DEWAXP.Foundation.Integration.Requests.VillaCostExemption.ownerdetails() { email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = x.PassportExpiry, passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                                }
                            };
                            var applicationResponse = VillaCostExemptionClient.SaveApplication(reqObject, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                            if (applicationResponse != null && applicationResponse.Succeeded && applicationResponse.Payload != null && applicationResponse.Payload.Responsecode.Equals("000"))
                            {
                                if (!string.IsNullOrEmpty(applicationResponse.Payload.Applicationreference.ToString()))
                                {
                                    ViewBag.AppNo = applicationResponse.Payload.Applicationreference.ToString();
                                    ViewBag.Stage = "02";
                                    return View(string.Format(ViewPath, "RequestSubmitSuccessful"));
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", applicationResponse.Message);
                            }
                        }
                        break;

                    #endregion Step 3

                    default:
                        ModelState.AddModelError("", ErrorMessages.UNEXPECTED_ERROR);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", Translate.Text("Unexpected error"));
            }

            return View(string.Format(ViewPath, model.Stage == 1 ? "NewApplication" : "NewApplicationStep2"), model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NewApplication(ApplicationViewModel model)
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            try
            {
                ApplicationViewModel _cacheModel = new ApplicationViewModel();
                if (CacheProvider.TryGet<ApplicationViewModel>(ApplicationDetails, out _cacheModel))
                {
                    model.BPList = _cacheModel.BPList;
                    model.PropertyOwnerTypeList = _cacheModel.PropertyOwnerTypeList;
                    model.BPDetails = _cacheModel.BPDetails;
                    model.Estimates = _cacheModel.Estimates;
                    model.Applications = _cacheModel.Applications;
                }
                else
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);

                #region Step 1

                var owners = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OwnerDetail>>(model.OwnersJson);
                if (owners != null && owners.Count > 0) { model.Owners = owners; }
                NewApplicationRequest reqObjectStage1 = new NewApplicationRequest()
                {
                    villarequest = new villarequest()
                    {
                        processcode = "03", //save application
                        applicationnumber = model.ApplicationNumber,
                        customernumber = model.BusinessPartner,
                        estimate = model.EstimateNumber,
                        ownertype = model.OwnerType,
                        //remarks = "INITIATING APPLICATION",
                        ownerdetails = model.Owners.Select(x => new ownerdetails() { itemnumber = x.Itemnumber != null ? x.Itemnumber : " ", idtype = x.IdType, marsoom = x.Marsoom, email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = !string.IsNullOrWhiteSpace(x.PassportExpiry) ? CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(x.PassportExpiry), "dd MMMM yyyy").ToString("yyyyMMdd") : "", passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                    }
                };
                var applicationResponseStage1 = VillaCostExemptionClient.SaveApplication(reqObjectStage1, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                if (applicationResponseStage1 != null && applicationResponseStage1.Succeeded && applicationResponseStage1.Payload != null && applicationResponseStage1.Payload.Responsecode.Equals("000"))
                {
                    List<OwnerDetail> _cacheOwnerDetail = new List<OwnerDetail>();
                    if (CacheProvider.TryGet(OwnerAttachment, out _cacheOwnerDetail))
                    {
                        if (model.Owners != null)
                        {
                            foreach (var owner in model.Owners)
                            {
                                if (!string.IsNullOrWhiteSpace(owner.PassportNumber))
                                {
                                    var itemAttach = _cacheOwnerDetail.Where(x => x.PassportNumber.Equals(owner.PassportNumber)).FirstOrDefault();
                                    if (itemAttach != null && applicationResponseStage1.Payload.OwnerDetails != null)
                                    {
                                        foreach (var ownerItem in applicationResponseStage1.Payload.OwnerDetails)
                                        {
                                            if (ownerItem.Passport != null && ownerItem.Passport.Equals(itemAttach.PassportNumber))
                                            {
                                                //var itemNo = applicationResponseStage1.Payload.OwnerDetails.Where(x => x.Passport != null && x.Passport.Equals(itemAttach.PassportNumber)).Any() ? applicationResponseStage1.Payload.OwnerDetails.Where(x => x.Passport.Equals(itemAttach.PassportNumber)).FirstOrDefault() : null;
                                                if (!string.IsNullOrWhiteSpace(ownerItem.Itemnumber) && itemAttach.PassportCopy != null)
                                                {
                                                    HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(itemAttach.PassportCopy_Binary, itemAttach.PassportCopy.FileName, itemAttach.PassportCopy.ContentType);
                                                    var success = uploadAttachments(objFile, applicationResponseStage1.Payload.Applicationreference, "06", ownerItem.Itemnumber);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    model.Stage = 2;
                    if (applicationResponseStage1.Payload.Applicationreference != null && !string.IsNullOrWhiteSpace(applicationResponseStage1.Payload.Applicationreference.ToString()))
                    {
                        model.ApplicationReferenceNumber = applicationResponseStage1.Payload.Applicationreference.ToString();
                    }
                    CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                    GetExistingApplications(true);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_EDITAPPLICATION_STEP2);
                }
                else
                {
                    CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(applicationResponseStage1.Message, Times.Once));
                }

                #endregion Step 1
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_NEWAPPLICATION);
        }

        #endregion Step 1 (New)

        #region Step 1 (Edit)

        public ActionResult EditApplication(string n = "")
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            ApplicationViewModel model = new ApplicationViewModel();

            DashboardPageModel dashboardPageModel;
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out model))
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                    {
                        n = model.ApplicationReferenceNumber;
                        model.Stage = 1;
                    }
                }
                else
                {
                    model = new ApplicationViewModel { Stage = 1 };
                }

                if (CacheProvider.TryGet<DashboardPageModel>(ExistingApplications, out dashboardPageModel))
                {
                    CustomerDetail customerDetails = dashboardPageModel.CustomerDetails.Where(x => x.Reference.Equals(n)).FirstOrDefault();
                    model.BusinessPartner = customerDetails.CustomerNumber;
                    model.ApplicationNumber = customerDetails.Number;
                    model.EstimateNumber = customerDetails.EstimateNumber;
                    model.ApplicationReferenceNumber = customerDetails.Reference;
                    model.AapplicationSequenceNumber = customerDetails.SequenceNumber;
                    model.OwnerType = customerDetails.OwnerType;
                    model.Owners = customerDetails.OwnerDetails;
                    model.CustomerComments = customerDetails.Remarks;
                    model.OwnersJson = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails.OwnerDetails, Newtonsoft.Json.Formatting.None);
                    model.PropertyOwnerTypeList = dashboardPageModel.OwnerTypeList != null ? dashboardPageModel.OwnerTypeList.Select(x => new SelectListItem() { Text = x.Text, Value = x.Value }).ToList() : new List<SelectListItem>();
                    model.Countrylists = GetCountryList();
                    if (customerDetails.OwnerAttachments != null && customerDetails.OwnerAttachments.Count() > 0)
                    {
                        var OwnerAttachments = customerDetails.OwnerAttachments.Where(x => x.ownerapplicationitemnumber != null && !x.ownerapplicationitemnumber.Equals("0010")).ToList();
                        foreach (var itemAttach in OwnerAttachments)
                        {
                            if (itemAttach != null && itemAttach.documentnumber != null)
                            {
                                DownloadFileRequest downloadFileRequest = new DownloadFileRequest();
                                downloadFileRequest.attachrequest = new Attachrequest
                                {
                                    applicationreferencenumber = model.ApplicationReferenceNumber,
                                    itemnumber = itemAttach.ownerapplicationitemnumber,
                                    documentnumber = itemAttach.documentnumber,
                                    flag = "X"
                                };

                                var FileServiceResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                                if (FileServiceResponse != null && FileServiceResponse.Succeeded && FileServiceResponse.Payload != null && FileServiceResponse.Payload.responsecode.Equals("000"))
                                {
                                    var ownerItem = model.Owners.Where(x => !string.IsNullOrWhiteSpace(x.Itemnumber) && x.Itemnumber.Equals(itemAttach.ownerapplicationitemnumber)).FirstOrDefault();
                                    if (ownerItem != null && FileServiceResponse.Payload.attachlist != null)
                                    {
                                        ownerItem.PassportCopy_Binary = FileServiceResponse.Payload.attachlist[0].content;
                                        ownerItem.DocumentNumber = FileServiceResponse.Payload.attachlist[0].documentnumber;
                                        ownerItem.FileName = FileServiceResponse.Payload.attachlist[0].filename;
                                        ownerItem.FileSize = FileServiceResponse.Payload.attachlist[0].filesize;
                                        ownerItem.AttachmentType = "06";
                                    }
                                }
                            }
                        }
                    }
                    model.OwnersJson = Newtonsoft.Json.JsonConvert.SerializeObject(model.Owners, Newtonsoft.Json.Formatting.None);

                    CacheProvider.Store(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                    return View(string.Format(ViewPath, "EditApplication"), model);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditApplication(ApplicationViewModel model)
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }

            ApplicationViewModel _cacheModel = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out _cacheModel))
                {
                    model.Estimates = _cacheModel.Estimates;
                    model.Applications = _cacheModel.Applications;
                    model.BusinessPartner = _cacheModel.BusinessPartner;
                    model.BPList = _cacheModel.BPList;
                    model.PropertyOwnerTypeList = _cacheModel.PropertyOwnerTypeList;
                    model.BPDetails = _cacheModel.BPDetails;
                    //model.OwnerType = _cacheModel.OwnerType;
                    if (_cacheModel.AttachmentLists != null && model.AttachmentLists != null)
                    {
                        foreach (var x in _cacheModel.AttachmentLists)
                        {
                            var itemToChange = model.AttachmentLists.FirstOrDefault(d => d.AttachmentType == x.AttachmentType);
                            if (itemToChange != null)
                            {
                                itemToChange.ConstructionContractSigned_Binary = x.ConstructionContractSigned_Binary;
                                itemToChange.ConstructionContract_02_Binary = x.ConstructionContract_02_Binary;
                                itemToChange.ConstructionContract_03_Binary = x.ConstructionContract_03_Binary;
                                itemToChange.ConstructionContract_04_Binary = x.ConstructionContract_04_Binary;
                                itemToChange.CompletionLetter_Binary = x.CompletionLetter_Binary;
                                itemToChange.DocumentNumber = x.DocumentNumber;
                                itemToChange.FileName = x.FileName;
                                itemToChange.FileSize = x.FileSize;
                            }
                        }
                    }
                }
                switch (model.Stage)
                {
                    case 1:
                        var owners = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OwnerDetail>>(model.OwnersJson);
                        if (owners != null && owners.Count > 0) { model.Owners = owners; }

                        NewApplicationRequest reqObjectStage1 = new NewApplicationRequest()
                        {
                            villarequest = new villarequest()
                            {
                                processcode = "03",
                                applicationreferencenumber = model.ApplicationReferenceNumber,
                                applicationsequencenumber = model.AapplicationSequenceNumber,
                                applicationnumber = model.ApplicationNumber,
                                customernumber = model.BusinessPartner,
                                estimate = model.EstimateNumber,
                                applicationstatus = model.Status,
                                ownertype = model.OwnerType,
                                //remarks = "EDIT APPLICATION",
                                ownerdetails = model.Owners.Select(x => new ownerdetails() { itemnumber = x.Itemnumber != null ? x.Itemnumber : " ", marsoom = x.Marsoom, idtype = x.IdType, email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = x.PassportExpiry, passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                            }
                        };
                        var applicationResponseStage1 = VillaCostExemptionClient.SaveApplication(reqObjectStage1, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                        if (applicationResponseStage1 != null && applicationResponseStage1.Succeeded && applicationResponseStage1.Payload != null && applicationResponseStage1.Payload.Responsecode.Equals("000"))
                        {
                            List<OwnerDetail> _cacheOwnerDetail = new List<OwnerDetail>();
                            if (CacheProvider.TryGet(OwnerAttachment, out _cacheOwnerDetail))
                            {
                                if (model.Owners != null)
                                {
                                    foreach (var owner in model.Owners)
                                    {
                                        if (!string.IsNullOrWhiteSpace(owner.PassportNumber))
                                        {
                                            var itemAttach = _cacheOwnerDetail.Where(x => x.PassportNumber.Equals(owner.PassportNumber)).FirstOrDefault();
                                            if (itemAttach != null && applicationResponseStage1.Payload.OwnerDetails != null)
                                            {
                                                foreach (var ownerItem in applicationResponseStage1.Payload.OwnerDetails)
                                                {
                                                    if (ownerItem.Passport != null && ownerItem.Passport.Equals(itemAttach.PassportNumber))
                                                    {
                                                        //var itemNo = applicationResponseStage1.Payload.OwnerDetails.Where(x => x.Passport != null && x.Passport.Equals(itemAttach.PassportNumber)).Any() ? applicationResponseStage1.Payload.OwnerDetails.Where(x => x.Passport.Equals(itemAttach.PassportNumber)).FirstOrDefault() : null;
                                                        if (!string.IsNullOrWhiteSpace(ownerItem.Itemnumber) && itemAttach.PassportCopy != null)
                                                        {
                                                            var success = uploadAttachments(itemAttach.PassportCopy, applicationResponseStage1.Payload.Applicationreference, "06", ownerItem.Itemnumber);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            model.Stage = 2;
                            model.AttachmentLists = new List<Attachlist>();
                            model.AttachmentLists.Add(new Attachlist()
                            {
                                AttachmentType = "01",
                            });
                            model.AttachmentLists.Add(new Attachlist()
                            {
                                AttachmentType = "02",
                            });
                            model.AttachmentLists.Add(new Attachlist()
                            {
                                AttachmentType = "03",
                            });
                            model.AttachmentLists.Add(new Attachlist()
                            {
                                AttachmentType = "04",
                            });
                            model.AttachmentLists.Add(new Attachlist()
                            {
                                AttachmentType = "05",
                            });
                            if (applicationResponseStage1.Payload.Applicationreference != null && !string.IsNullOrWhiteSpace(applicationResponseStage1.Payload.Applicationreference.ToString()))
                            {
                                model.ApplicationReferenceNumber = applicationResponseStage1.Payload.Applicationreference.ToString();
                                DownloadFileRequest downloadFileRequest = new DownloadFileRequest();
                                downloadFileRequest.attachrequest = new Attachrequest
                                {
                                    applicationreferencenumber = model.ApplicationReferenceNumber,
                                    itemnumber = "0010"
                                };
                                var attachmentResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                                if (attachmentResponse.Succeeded && attachmentResponse.Payload != null && attachmentResponse.Payload.responsecode.Equals("000"))
                                {
                                    if (attachmentResponse.Payload.attachlist != null)
                                    {
                                        foreach (var item in attachmentResponse.Payload.attachlist)
                                        {
                                            downloadFileRequest.attachrequest.documentnumber = item.documentnumber;
                                            downloadFileRequest.attachrequest.flag = "X";
                                            var FileServiceResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                                            if (FileServiceResponse != null && FileServiceResponse.Payload != null && FileServiceResponse.Payload.attachlist != null && FileServiceResponse.Payload.attachlist.Count > 0)
                                            {
                                                foreach (var itemAttach in FileServiceResponse.Payload.attachlist)
                                                {
                                                    if (item.attachmenttype.Equals("01"))
                                                    {
                                                        model.AttachmentLists[0] = new Attachlist
                                                        {
                                                            AttachmentType = item.attachmenttype,
                                                            ConstructionContractSigned_Binary = itemAttach.content,
                                                            DocumentNumber = itemAttach.documentnumber,
                                                            FileName = itemAttach.filename,
                                                            FileSize = itemAttach.filesize,
                                                        };
                                                    }
                                                    else if (item.attachmenttype.Equals("02"))
                                                    {
                                                        model.AttachmentLists[1] = new Attachlist
                                                        {
                                                            AttachmentType = item.attachmenttype,
                                                            ConstructionContract_02_Binary = itemAttach.content,
                                                            DocumentNumber = itemAttach.documentnumber,
                                                            FileName = itemAttach.filename,
                                                            FileSize = itemAttach.filesize,
                                                        };
                                                    }
                                                    else if (item.attachmenttype.Equals("03"))
                                                    {
                                                        model.AttachmentLists[2] = new Attachlist
                                                        {
                                                            AttachmentType = item.attachmenttype,
                                                            ConstructionContract_03_Binary = itemAttach.content,
                                                            DocumentNumber = itemAttach.documentnumber,
                                                            FileName = itemAttach.filename,
                                                            FileSize = itemAttach.filesize,
                                                        };
                                                    }
                                                    else if (item.attachmenttype.Equals("04"))
                                                    {
                                                        model.AttachmentLists[3] = new Attachlist
                                                        {
                                                            AttachmentType = item.attachmenttype,
                                                            ConstructionContract_04_Binary = itemAttach.content,
                                                            DocumentNumber = itemAttach.documentnumber,
                                                            FileName = itemAttach.filename,
                                                            FileSize = itemAttach.filesize,
                                                        };
                                                    }
                                                    else
                                                    {
                                                        model.AttachmentLists[4] = new Attachlist
                                                        {
                                                            AttachmentType = item.attachmenttype,
                                                            CompletionLetter_Binary = itemAttach.content,
                                                            DocumentNumber = itemAttach.documentnumber,
                                                            FileName = itemAttach.filename,
                                                            FileSize = itemAttach.filesize,
                                                        };
                                                    }
                                                }
                                            }
                                        }
                                        if (attachmentResponse.Payload.attachlist.Count() < 5)
                                        {
                                        }
                                    }
                                }
                            }
                            CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                            if (ModelState.IsValid)
                                ModelState.Clear();
                            return View(string.Format(ViewPath, "EditApplicationStep2"), model);
                        }
                        else
                        {
                            ModelState.AddModelError("", applicationResponseStage1.Message);
                        }
                        return View(string.Format(ViewPath, "EditApplication"), model);

                    case 2: //Add/Update/Delete files
                        bool isSuccess = false;
                        var cnt = 0;
                        var ownersStage2 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OwnerDetail>>(model.OwnersJson);
                        if (ownersStage2 != null && ownersStage2.Count > 0) { model.Owners = ownersStage2; }
                        if (model != null && model.AttachmentLists != null && model.AttachmentLists.Count > 0)
                        {
                            foreach (var itemAttach in model.AttachmentLists)
                            {
                                #region Construction contract signed by (Owner, Contractor, Consultant)

                                if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("01"))
                                {
                                    if (itemAttach.ConstructionContractSigned != null && itemAttach.ConstructionContractSigned.ContentLength > 0)
                                    {
                                        isSuccess = uploadAttachments(itemAttach.ConstructionContractSigned, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                        cnt++;
                                    }
                                    else
                                    {
                                        if (itemAttach.ConstructionContractSigned_Binary == null)
                                            ModelState.AddModelError(string.Empty, Translate.Text("VE_File1RequiredField"));
                                    }

                                    if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContractSigned_AttachmentRemove1))
                                    {
                                        string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                        isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContractSigned_AttachmentRemove1, strFileExtension);
                                    }
                                }

                                #endregion Construction contract signed by (Owner, Contractor, Consultant)

                                #region Construction 02 (optional)

                                if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("02"))
                                {
                                    if (itemAttach.ConstructionContract_02 != null && itemAttach.ConstructionContract_02.ContentLength > 0)
                                    {
                                        isSuccess = uploadAttachments(itemAttach.ConstructionContract_02, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                        cnt++;
                                    }
                                    if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContract_02_AttachmentRemove2))
                                    {
                                        string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                        isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContract_02_AttachmentRemove2, strFileExtension);
                                    }
                                }

                                #endregion Construction 02 (optional)

                                #region Construction 03 (optional)

                                if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("03"))
                                {
                                    if (itemAttach.ConstructionContract_03 != null && itemAttach.ConstructionContract_03.ContentLength > 0)
                                    {
                                        isSuccess = uploadAttachments(itemAttach.ConstructionContract_03, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                    }
                                    if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContract_03_AttachmentRemove3))
                                    {
                                        string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                        isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContract_03_AttachmentRemove3, strFileExtension);
                                    }
                                }

                                #endregion Construction 03 (optional)

                                #region Construction 04 (optional)

                                if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("04"))
                                {
                                    if (itemAttach.ConstructionContract_04 != null && itemAttach.ConstructionContract_04.ContentLength > 0)
                                    {
                                        isSuccess = uploadAttachments(itemAttach.ConstructionContract_04, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                    }
                                    if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContract_04_AttachmentRemove4))
                                    {
                                        string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                        isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContract_04_AttachmentRemove4, strFileExtension);
                                    }
                                }

                                #endregion Construction 04 (optional)

                                #region Completion Letter (mandatory if more then one construction contract is uploaded)

                                if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("05"))
                                {
                                    if (itemAttach.CompletionLetter != null && itemAttach.CompletionLetter.ContentLength > 0)
                                    {
                                        isSuccess = uploadAttachments(itemAttach.CompletionLetter, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                    }
                                    else
                                    {
                                        if (cnt == 2)
                                        {
                                            if (itemAttach.CompletionLetter_Binary == null)
                                                ModelState.AddModelError(string.Empty, Translate.Text("VE_File1RequiredField"));
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(itemAttach.CompletionLetter_AttachmentRemove5))
                                    {
                                        string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                        isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.CompletionLetter_AttachmentRemove5, strFileExtension);
                                    }
                                }

                                #endregion Completion Letter (mandatory if more then one construction contract is uploaded)
                            }
                        }
                        if (!ModelState.IsValid)
                        {
                            model.Stage = 2;
                            CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                            return View(string.Format(ViewPath, "EditApplicationStep2"), model);
                        }
                        else
                        {
                            model.Stage = 3;
                        }
                        CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        if (ModelState.IsValid)
                            ModelState.Clear();
                        return View(string.Format(ViewPath, "ApplicationStep3"), model);

                    case 3:
                        var processCode = model.IsSubmit.Equals(1) ? "04" : "03";
                        NewApplicationRequest reqObject = new NewApplicationRequest()
                        {
                            villarequest = new villarequest()
                            {
                                processcode = processCode,
                                applicationnumber = _cacheModel.ApplicationNumber,
                                applicationreferencenumber = _cacheModel.ApplicationReferenceNumber,
                                customernumber = _cacheModel.BusinessPartner,
                                estimate = _cacheModel.EstimateNumber,
                                ownertype = _cacheModel.OwnerType,
                                remarks = model.CustomerComments,
                                ownerdetails = _cacheModel.Owners.Select(x => new DEWAXP.Foundation.Integration.Requests.VillaCostExemption.ownerdetails() { email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = x.PassportExpiry, passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                            }
                        };
                        var applicationResponse = VillaCostExemptionClient.SaveApplication(reqObject, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                        if (applicationResponse != null && applicationResponse.Succeeded && applicationResponse.Payload != null && applicationResponse.Payload.Responsecode.Equals("000"))
                        {
                            if (!string.IsNullOrEmpty(applicationResponse.Payload.Applicationreference.ToString()))
                            {
                                ViewBag.AppNo = applicationResponse.Payload.Applicationreference.ToString();
                                ViewBag.IsSubmit = model.IsSubmit.ToString();
                                CacheProvider.Remove(ExistingApplications);
                                CacheProvider.Remove(EditApplicationDetails);
                                CacheProvider.Remove(ApplicationDetails);
                                return View(string.Format(ViewPath, "RequestSubmitSuccessful"));
                            }
                        }
                        else
                        {
                            model.Stage = 3;
                            ModelState.AddModelError("", applicationResponse.Message);
                        }
                        CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        if (ModelState.IsValid)
                            ModelState.Clear();
                        return View(string.Format(ViewPath, "ApplicationStep3"), model);

                    default:
                        ModelState.AddModelError("", ErrorMessages.UNEXPECTED_ERROR);
                        break;
                }
                model.Stage = 1;
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", Translate.Text("Unexpected error"));
            }

            return View(string.Format(ViewPath, "EditApplication"), model);
        }

        [HttpGet]
        public ActionResult EditApplicationStep1(string n = "")
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }

            CacheProvider.Remove(EditApplicationDetails);
            CacheProvider.Remove(CountryList);

            DashboardPageModel dashboardPageModel;
            ApplicationViewModel model = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out model))
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                    {
                        model.Stage = 1;
                    }
                }
                else
                {
                    model = new ApplicationViewModel { Stage = 1 };
                    GetExistingApplications(true);
                    if (CacheProvider.TryGet<DashboardPageModel>(ExistingApplications, out dashboardPageModel))
                    {
                        CustomerDetail customerDetails = dashboardPageModel.CustomerDetails.Where(x => x.Reference.Equals(n)).FirstOrDefault();
                        model.BusinessPartner = customerDetails.CustomerNumber;
                        model.ApplicationNumber = customerDetails.Number;
                        model.EstimateNumber = customerDetails.EstimateNumber;
                        model.ApplicationReferenceNumber = customerDetails.Reference;
                        model.AapplicationSequenceNumber = customerDetails.SequenceNumber;
                        model.OwnerType = customerDetails.OwnerType;
                        model.Owners = customerDetails.OwnerDetails;
                        model.CustomerComments = customerDetails.Remarks;
                        model.OwnersJson = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails.OwnerDetails, Newtonsoft.Json.Formatting.None);
                        model.PropertyOwnerTypeList = dashboardPageModel.OwnerTypeList != null ? dashboardPageModel.OwnerTypeList.Select(x => new SelectListItem() { Text = x.Text, Value = x.Value }).ToList() : new List<SelectListItem>();
                        model.Countrylists = GetCountryList();
                        if (customerDetails.OwnerAttachments != null && customerDetails.OwnerAttachments.Count() > 0)
                        {
                            var OwnerAttachments = customerDetails.OwnerAttachments.Where(x => x.ownerapplicationitemnumber != null && !x.ownerapplicationitemnumber.Equals("0010")).ToList();
                            foreach (var itemAttach in OwnerAttachments)
                            {
                                if (itemAttach != null && itemAttach.documentnumber != null)
                                {
                                    DownloadFileRequest downloadFileRequest = new DownloadFileRequest();
                                    downloadFileRequest.attachrequest = new Attachrequest
                                    {
                                        applicationreferencenumber = model.ApplicationReferenceNumber,
                                        itemnumber = itemAttach.ownerapplicationitemnumber,
                                        documentnumber = itemAttach.documentnumber,
                                        flag = "X"
                                    };

                                    var FileServiceResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                                    if (FileServiceResponse != null && FileServiceResponse.Succeeded && FileServiceResponse.Payload != null && FileServiceResponse.Payload.responsecode.Equals("000"))
                                    {
                                        var ownerItem = model.Owners.Where(x => !string.IsNullOrWhiteSpace(x.Itemnumber) && x.Itemnumber.Equals(itemAttach.ownerapplicationitemnumber)).FirstOrDefault();
                                        if (ownerItem != null && FileServiceResponse.Payload.attachlist != null)
                                        {
                                            ownerItem.PassportCopy_Binary = FileServiceResponse.Payload.attachlist[0].content;
                                            ownerItem.DocumentNumber = FileServiceResponse.Payload.attachlist[0].documentnumber;
                                            ownerItem.FileName = FileServiceResponse.Payload.attachlist[0].filename;
                                            ownerItem.FileSize = FileServiceResponse.Payload.attachlist[0].filesize;
                                            ownerItem.AttachmentType = "06";
                                        }
                                    }
                                }
                            }
                        }
                        model.OwnersJson = Newtonsoft.Json.JsonConvert.SerializeObject(model.Owners, Newtonsoft.Json.Formatting.None);

                        CacheProvider.Store(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        return View(string.Format(ViewPath, "EditApplication"), model);
                    }
                    else
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View(string.Format(ViewPath, "EditApplication"), model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditApplicationStep1(ApplicationViewModel model)
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            ApplicationViewModel _cacheModel = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out _cacheModel))
                {
                    model.Estimates = _cacheModel.Estimates;
                    model.Applications = _cacheModel.Applications;
                    model.BusinessPartner = _cacheModel.BusinessPartner;
                    model.BPList = _cacheModel.BPList;
                    model.PropertyOwnerTypeList = _cacheModel.PropertyOwnerTypeList;
                    model.BPDetails = _cacheModel.BPDetails;
                    if (_cacheModel.AttachmentLists != null && model.AttachmentLists != null)
                    {
                        foreach (var x in _cacheModel.AttachmentLists)
                        {
                            var itemToChange = model.AttachmentLists.FirstOrDefault(d => d.AttachmentType == x.AttachmentType);
                            if (itemToChange != null)
                            {
                                itemToChange.ConstructionContractSigned_Binary = x.ConstructionContractSigned_Binary;
                                itemToChange.ConstructionContract_02_Binary = x.ConstructionContract_02_Binary;
                                itemToChange.ConstructionContract_03_Binary = x.ConstructionContract_03_Binary;
                                itemToChange.ConstructionContract_04_Binary = x.ConstructionContract_04_Binary;
                                itemToChange.CompletionLetter_Binary = x.CompletionLetter_Binary;
                                itemToChange.DocumentNumber = x.DocumentNumber;
                                itemToChange.FileName = x.FileName;
                                itemToChange.FileSize = x.FileSize;
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(model.OwnersJson))
                    {
                        var owners = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OwnerDetail>>(model.OwnersJson);
                        if (owners != null && owners.Count > 0) { model.Owners = owners; }
                    }
                    NewApplicationRequest reqObjectStage1 = new NewApplicationRequest()
                    {
                        villarequest = new villarequest()
                        {
                            processcode = "03",
                            applicationreferencenumber = model.ApplicationReferenceNumber,
                            applicationsequencenumber = model.AapplicationSequenceNumber,
                            applicationnumber = model.ApplicationNumber,
                            customernumber = model.BusinessPartner,
                            estimate = model.EstimateNumber,
                            applicationstatus = model.Status,
                            ownertype = model.OwnerType,
                            //remarks = "EDIT APPLICATION STEP 1",
                            ownerdetails = model.Owners.Select(x => new ownerdetails() { itemnumber = x.Itemnumber != null ? x.Itemnumber : " ", idtype = x.IdType, marsoom = x.Marsoom, email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = !string.IsNullOrWhiteSpace(x.PassportExpiry) ? CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(x.PassportExpiry), "dd MMMM yyyy").ToString("yyyyMMdd") : "", passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                        }
                    };
                    var applicationResponseStage1 = VillaCostExemptionClient.SaveApplication(reqObjectStage1, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                    if (applicationResponseStage1 != null && applicationResponseStage1.Succeeded && applicationResponseStage1.Payload != null && applicationResponseStage1.Payload.Responsecode.Equals("000"))
                    {
                        List<OwnerDetail> _cacheOwnerDetail = new List<OwnerDetail>();
                        if (CacheProvider.TryGet(OwnerAttachment, out _cacheOwnerDetail))
                        {
                            if (model.Owners != null)
                            {
                                foreach (var owner in model.Owners)
                                {
                                    if (!string.IsNullOrWhiteSpace(owner.PassportNumber))
                                    {
                                        var itemAttach = _cacheOwnerDetail.Where(x => x.PassportNumber.Equals(owner.PassportNumber)).FirstOrDefault();
                                        if (itemAttach != null && applicationResponseStage1.Payload.OwnerDetails != null)
                                        {
                                            foreach (var ownerItem in applicationResponseStage1.Payload.OwnerDetails)
                                            {
                                                if (ownerItem.Passport != null && ownerItem.Passport.Equals(itemAttach.PassportNumber))
                                                {
                                                    //var itemNo = applicationResponseStage1.Payload.OwnerDetails.Where(x => x.Passport != null && x.Passport.Equals(itemAttach.PassportNumber)).Any() ? applicationResponseStage1.Payload.OwnerDetails.Where(x => x.Passport.Equals(itemAttach.PassportNumber)).FirstOrDefault() : null;
                                                    if (!string.IsNullOrWhiteSpace(ownerItem.Itemnumber) && itemAttach.PassportCopy != null)
                                                    {
                                                        HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(itemAttach.PassportCopy_Binary, itemAttach.PassportCopy.FileName, itemAttach.PassportCopy.ContentType);
                                                        var success = uploadAttachments(objFile, applicationResponseStage1.Payload.Applicationreference, "06", ownerItem.Itemnumber);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        model.Stage = 2;
                        CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        GetExistingApplications(true);
                        if (ModelState.IsValid)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_EDITAPPLICATION_STEP2);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", applicationResponseStage1.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
        }

        #endregion Step 1 (Edit)

        #region Step 2 (Edit)

        [HttpGet]
        public ActionResult EditApplicationStep2()
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            ApplicationViewModel model = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out model))
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                    {
                        model.AttachmentLists = new List<Attachlist>();
                        model.AttachmentLists.Add(new Attachlist()
                        {
                            AttachmentType = "01",
                        });
                        model.AttachmentLists.Add(new Attachlist()
                        {
                            AttachmentType = "02",
                        });
                        model.AttachmentLists.Add(new Attachlist()
                        {
                            AttachmentType = "03",
                        });
                        model.AttachmentLists.Add(new Attachlist()
                        {
                            AttachmentType = "04",
                        });
                        model.AttachmentLists.Add(new Attachlist()
                        {
                            AttachmentType = "05",
                        });
                        if (!string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                        {
                            DownloadFileRequest downloadFileRequest = new DownloadFileRequest();
                            downloadFileRequest.attachrequest = new Attachrequest
                            {
                                applicationreferencenumber = model.ApplicationReferenceNumber,
                                itemnumber = "0010"
                            };
                            var attachmentResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                            if (attachmentResponse.Succeeded && attachmentResponse.Payload != null && attachmentResponse.Payload.responsecode.Equals("000"))
                            {
                                if (attachmentResponse.Payload.attachlist != null)
                                {
                                    foreach (var item in attachmentResponse.Payload.attachlist)
                                    {
                                        downloadFileRequest.attachrequest.documentnumber = item.documentnumber;
                                        downloadFileRequest.attachrequest.flag = "X";
                                        downloadFileRequest.attachrequest.applicationreferencenumber = model.ApplicationReferenceNumber;

                                        var FileServiceResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                                        if (FileServiceResponse != null && FileServiceResponse.Payload != null && FileServiceResponse.Payload.attachlist != null && FileServiceResponse.Payload.attachlist.Count > 0)
                                        {
                                            foreach (var itemAttach in FileServiceResponse.Payload.attachlist)
                                            {
                                                if (item.attachmenttype.Equals("01"))
                                                {
                                                    model.AttachmentLists[0] = new Attachlist
                                                    {
                                                        AttachmentType = item.attachmenttype,
                                                        ConstructionContractSigned_Binary = itemAttach.content,
                                                        DocumentNumber = itemAttach.documentnumber,
                                                        FileName = itemAttach.filename,
                                                        FileSize = itemAttach.filesize,
                                                    };
                                                }
                                                else if (item.attachmenttype.Equals("02"))
                                                {
                                                    model.AttachmentLists[1] = new Attachlist
                                                    {
                                                        AttachmentType = item.attachmenttype,
                                                        ConstructionContract_02_Binary = itemAttach.content,
                                                        DocumentNumber = itemAttach.documentnumber,
                                                        FileName = itemAttach.filename,
                                                        FileSize = itemAttach.filesize,
                                                    };
                                                }
                                                else if (item.attachmenttype.Equals("03"))
                                                {
                                                    model.AttachmentLists[2] = new Attachlist
                                                    {
                                                        AttachmentType = item.attachmenttype,
                                                        ConstructionContract_03_Binary = itemAttach.content,
                                                        DocumentNumber = itemAttach.documentnumber,
                                                        FileName = itemAttach.filename,
                                                        FileSize = itemAttach.filesize,
                                                    };
                                                }
                                                else if (item.attachmenttype.Equals("04"))
                                                {
                                                    model.AttachmentLists[3] = new Attachlist
                                                    {
                                                        AttachmentType = item.attachmenttype,
                                                        ConstructionContract_04_Binary = itemAttach.content,
                                                        DocumentNumber = itemAttach.documentnumber,
                                                        FileName = itemAttach.filename,
                                                        FileSize = itemAttach.filesize,
                                                    };
                                                }
                                                else
                                                {
                                                    model.AttachmentLists[4] = new Attachlist
                                                    {
                                                        AttachmentType = item.attachmenttype,
                                                        CompletionLetter_Binary = itemAttach.content,
                                                        DocumentNumber = itemAttach.documentnumber,
                                                        FileName = itemAttach.filename,
                                                        FileSize = itemAttach.filesize,
                                                    };
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return View(string.Format(ViewPath, "EditApplicationStep2"), model);
                    }
                }
                else
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditApplicationStep2(ApplicationViewModel model)
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            ApplicationViewModel _cacheModel = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out _cacheModel))
                {
                    model.Estimates = _cacheModel.Estimates;
                    model.Applications = _cacheModel.Applications;
                    model.BusinessPartner = _cacheModel.BusinessPartner;
                    model.BPList = _cacheModel.BPList;
                    model.PropertyOwnerTypeList = _cacheModel.PropertyOwnerTypeList;
                    model.BPDetails = _cacheModel.BPDetails;
                    //model.OwnerType = _cacheModel.OwnerType;
                    if (_cacheModel.AttachmentLists != null && model.AttachmentLists != null)
                    {
                        foreach (var x in _cacheModel.AttachmentLists)
                        {
                            var itemToChange = model.AttachmentLists.FirstOrDefault(d => d.AttachmentType == x.AttachmentType);
                            if (itemToChange != null)
                            {
                                itemToChange.ConstructionContractSigned_Binary = x.ConstructionContractSigned_Binary;
                                itemToChange.ConstructionContract_02_Binary = x.ConstructionContract_02_Binary;
                                itemToChange.ConstructionContract_03_Binary = x.ConstructionContract_03_Binary;
                                itemToChange.ConstructionContract_04_Binary = x.ConstructionContract_04_Binary;
                                itemToChange.CompletionLetter_Binary = x.CompletionLetter_Binary;
                                itemToChange.DocumentNumber = x.DocumentNumber;
                                itemToChange.FileName = x.FileName;
                                itemToChange.FileSize = x.FileSize;
                            }
                        }
                    }

                    bool isSuccess = false;
                    var cnt = 0;
                    var ownersStage2 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OwnerDetail>>(model.OwnersJson);
                    if (ownersStage2 != null && ownersStage2.Count > 0) { model.Owners = ownersStage2; }
                    if (model != null && model.AttachmentLists != null && model.AttachmentLists.Count > 0)
                    {
                        foreach (var itemAttach in model.AttachmentLists)
                        {
                            #region Construction contract signed by (Owner, Contractor, Consultant)

                            if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("01"))
                            {
                                if (itemAttach.ConstructionContractSigned != null && itemAttach.ConstructionContractSigned.ContentLength > 0)
                                {
                                    isSuccess = uploadAttachments(itemAttach.ConstructionContractSigned, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                    cnt++;
                                }
                                else
                                {
                                    if (itemAttach.ConstructionContractSigned_Binary == null)
                                        ModelState.AddModelError(string.Empty, Translate.Text("VE_Construction_Signed_File1RequiredField"));
                                }

                                if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContractSigned_AttachmentRemove1))
                                {
                                    string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                    isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContractSigned_AttachmentRemove1, strFileExtension);
                                }
                            }

                            #endregion Construction contract signed by (Owner, Contractor, Consultant)

                            #region Construction 02 (optional)

                            if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("02"))
                            {
                                if (itemAttach.ConstructionContract_02 != null && itemAttach.ConstructionContract_02.ContentLength > 0)
                                {
                                    isSuccess = uploadAttachments(itemAttach.ConstructionContract_02, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                    cnt++;
                                }
                                if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContract_02_AttachmentRemove2))
                                {
                                    string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                    isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContract_02_AttachmentRemove2, strFileExtension);
                                }
                            }

                            #endregion Construction 02 (optional)

                            #region Construction 03 (optional)

                            if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("03"))
                            {
                                if (itemAttach.ConstructionContract_03 != null && itemAttach.ConstructionContract_03.ContentLength > 0)
                                {
                                    isSuccess = uploadAttachments(itemAttach.ConstructionContract_03, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                }
                                if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContract_03_AttachmentRemove3))
                                {
                                    string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                    isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContract_03_AttachmentRemove3, strFileExtension);
                                }
                            }

                            #endregion Construction 03 (optional)

                            #region Construction 04 (optional)

                            if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("04"))
                            {
                                if (itemAttach.ConstructionContract_04 != null && itemAttach.ConstructionContract_04.ContentLength > 0)
                                {
                                    isSuccess = uploadAttachments(itemAttach.ConstructionContract_04, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                }
                                if (!string.IsNullOrWhiteSpace(itemAttach.ConstructionContract_04_AttachmentRemove4))
                                {
                                    string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                    isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.ConstructionContract_04_AttachmentRemove4, strFileExtension);
                                }
                            }

                            #endregion Construction 04 (optional)

                            #region Completion Letter (mandatory if more then one construction contract is uploaded)

                            if (itemAttach != null && itemAttach.AttachmentType != null && itemAttach.AttachmentType.Equals("05"))
                            {
                                if (itemAttach.CompletionLetter != null && itemAttach.CompletionLetter.ContentLength > 0)
                                {
                                    isSuccess = uploadAttachments(itemAttach.CompletionLetter, model.ApplicationReferenceNumber, itemAttach.AttachmentType, "0010");
                                }
                                else
                                {
                                    if (cnt == 2)
                                    {
                                        if (itemAttach.CompletionLetter_Binary == null)
                                            ModelState.AddModelError(string.Empty, Translate.Text("VE_Completion_File1RequiredField"));
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(itemAttach.CompletionLetter_AttachmentRemove5))
                                {
                                    string strFileExtension = System.IO.Path.GetExtension(itemAttach.FileName).Replace(".", "");
                                    isSuccess = DeleteAttachments(model.ApplicationReferenceNumber, itemAttach.AttachmentType, itemAttach.CompletionLetter_AttachmentRemove5, strFileExtension);
                                }
                            }

                            #endregion Completion Letter (mandatory if more then one construction contract is uploaded)
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        model.Stage = 3;
                        CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_EDITAPPLICATION_STEP3);
                    }
                }
                else
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", Translate.Text("Unexpected error"));
            }

            return View(string.Format(ViewPath, "EditApplicationStep2"), model);
        }

        #endregion Step 2 (Edit)

        #region Step 3 (Edit)

        [HttpGet]
        public ActionResult EditApplicationStep3(string n = "")
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            ApplicationViewModel model = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out model))
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                    {
                        return View(string.Format(ViewPath, "ApplicationStep3"), model);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(n))
                    {
                        DashboardPageModel dashboardPageModel;
                        model = new ApplicationViewModel();
                        if (CacheProvider.TryGet<DashboardPageModel>(ExistingApplications, out dashboardPageModel))
                        {
                            CustomerDetail customerDetails = dashboardPageModel.CustomerDetails.Where(x => x.Reference.Equals(n)).FirstOrDefault();
                            model.BusinessPartner = customerDetails.CustomerNumber;
                            model.ApplicationNumber = customerDetails.Number;
                            model.EstimateNumber = customerDetails.EstimateNumber;
                            model.ApplicationReferenceNumber = customerDetails.Reference;
                            model.AapplicationSequenceNumber = customerDetails.SequenceNumber;
                            model.OwnerType = customerDetails.OwnerType;
                            model.Owners = customerDetails.OwnerDetails;
                            model.OwnersJson = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails.OwnerDetails, Newtonsoft.Json.Formatting.None);
                            model.PropertyOwnerTypeList = dashboardPageModel.OwnerTypeList != null ? dashboardPageModel.OwnerTypeList.Select(x => new SelectListItem() { Text = x.Text, Value = x.Value }).ToList() : new List<SelectListItem>();
                            model.Countrylists = GetCountryList();
                        }
                        CacheProvider.Store(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        return View(string.Format(ViewPath, "ApplicationStep3"), model);
                    }
                    else
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditApplicationStep3(ApplicationViewModel model)
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            ApplicationViewModel _cacheModel = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out _cacheModel))
                {
                    model.Estimates = _cacheModel.Estimates;
                    model.Applications = _cacheModel.Applications;
                    model.BusinessPartner = _cacheModel.BusinessPartner;
                    model.BPList = _cacheModel.BPList;
                    model.PropertyOwnerTypeList = _cacheModel.PropertyOwnerTypeList;
                    model.BPDetails = _cacheModel.BPDetails;
                }
                var processCode = model.IsSubmit.Equals(1) ? "04" : "03";
                NewApplicationRequest reqObject = new NewApplicationRequest()
                {
                    villarequest = new villarequest()
                    {
                        processcode = processCode,
                        applicationsequencenumber = _cacheModel.AapplicationSequenceNumber,
                        applicationnumber = _cacheModel.ApplicationNumber,
                        applicationreferencenumber = _cacheModel.ApplicationReferenceNumber,
                        customernumber = _cacheModel.BusinessPartner,
                        estimate = _cacheModel.EstimateNumber,
                        ownertype = _cacheModel.OwnerType,
                        remarks = model.CustomerComments,
                        //ownerdetails = _cacheModel.Owners.Select(x => new DEWAXP.Foundation.Integration.Requests.VillaCostExemption.ownerdetails() { email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = x.PassportExpiry, passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                        ownerdetails = _cacheModel.Owners.Select(x => new ownerdetails() { itemnumber = x.Itemnumber != null ? x.Itemnumber : " ", idtype = x.IdType, marsoom = x.Marsoom, email = x.Email, emiratesid = x.EmiratesID, mobile = x.Mobile1, mobile2 = x.Mobile2, name = x.Name, passport = x.PassportNumber, passportexpiry = !string.IsNullOrWhiteSpace(x.PassportExpiry) ? CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(x.PassportExpiry), "dd MMMM yyyy").ToString("yyyyMMdd") : "", passportissue_authority = x.IssuingAuthority, relation = x.Relation }).ToList()
                    }
                };
                var applicationResponse = VillaCostExemptionClient.SaveApplication(reqObject, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                if (applicationResponse != null && applicationResponse.Succeeded && applicationResponse.Payload != null && applicationResponse.Payload.Responsecode.Equals("000"))
                {
                    if (!string.IsNullOrEmpty(applicationResponse.Payload.Applicationreference.ToString()))
                    {
                        CacheProvider.Store<ApplicationViewModel>(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        ViewBag.AppNo = _cacheModel.ApplicationNumber;
                        ViewBag.RefNo = applicationResponse.Payload.Applicationreference.ToString();
                        ViewBag.IsSubmit = model.IsSubmit.ToString();
                        CacheProvider.Remove(ExistingApplications);
                        CacheProvider.Remove(EditApplicationDetails);
                        CacheProvider.Remove(ApplicationDetails);
                        return View(string.Format(ViewPath, "RequestSubmitSuccessful"));
                    }
                }
                else
                {
                    ModelState.AddModelError("", applicationResponse.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError("", Translate.Text("Unexpected error"));
            }

            return View(string.Format(ViewPath, "ApplicationStep3"), model);
        }

        #endregion Step 3 (Edit)

        #region View Application

        [HttpGet]
        public ActionResult ViewApplication(string n = "")
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            }
            DashboardPageModel dashboardPageModel;
            ApplicationViewModel model = new ApplicationViewModel();
            try
            {
                if (CacheProvider.TryGet<ApplicationViewModel>(EditApplicationDetails, out model))
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                    {
                        model.Stage = 1;
                    }
                }
                else
                {
                    model = new ApplicationViewModel { Stage = 1 };
                    if (CacheProvider.TryGet<DashboardPageModel>(ExistingApplications, out dashboardPageModel))
                    {
                        CustomerDetail customerDetails = dashboardPageModel.CustomerDetails.Where(x => x.Reference.Equals(n)).FirstOrDefault();
                        model.BusinessPartner = customerDetails.CustomerNumber;
                        model.ApplicationNumber = customerDetails.Number;
                        model.EstimateNumber = customerDetails.EstimateNumber;
                        model.ApplicationReferenceNumber = customerDetails.Reference;
                        model.AapplicationSequenceNumber = customerDetails.SequenceNumber;
                        model.OwnerType = customerDetails.OwnerType;
                        model.Owners = customerDetails.OwnerDetails;
                        model.CustomerComments = customerDetails.Remarks;
                        model.OwnersJson = Newtonsoft.Json.JsonConvert.SerializeObject(customerDetails.OwnerDetails, Newtonsoft.Json.Formatting.None);
                        model.PropertyOwnerTypeList = dashboardPageModel.OwnerTypeList != null ? dashboardPageModel.OwnerTypeList.Select(x => new SelectListItem() { Text = x.Text, Value = x.Value }).ToList() : new List<SelectListItem>();
                        if (model.PropertyOwnerTypeList != null && model.PropertyOwnerTypeList.Count > 0)
                        {
                            var selectedOwner = model.PropertyOwnerTypeList.Where(x => x.Value.Equals(model.OwnerType)).Select(x => x.Text).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(selectedOwner))
                                model.OwnerTypeText = selectedOwner.ToString();
                        }
                        model.Countrylists = GetCountryList();
                        model.OwnersJson = Newtonsoft.Json.JsonConvert.SerializeObject(model.Owners, Newtonsoft.Json.Formatting.None);
                        //CacheProvider.Store(EditApplicationDetails, new CacheItem<ApplicationViewModel>(model));
                        if (model != null && !string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                        {
                            if (!string.IsNullOrWhiteSpace(model.ApplicationReferenceNumber))
                            {
                                DownloadFileRequest downloadFileRequest = new DownloadFileRequest();
                                downloadFileRequest.attachrequest = new Attachrequest
                                {
                                    applicationreferencenumber = model.ApplicationReferenceNumber,
                                    itemnumber = "0010"
                                };
                                var attachmentResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                                if (attachmentResponse.Succeeded && attachmentResponse.Payload != null && attachmentResponse.Payload.responsecode.Equals("000"))
                                {
                                    if (attachmentResponse.Payload.attachlist != null)
                                    {
                                        model.AttachmentLists = new List<Attachlist>();
                                        foreach (var item in attachmentResponse.Payload.attachlist)
                                        {
                                            model.AttachmentLists.Add(new Attachlist()
                                            {
                                                AttachmentType = item.attachmenttype,
                                                DocumentNumber = item.documentnumber,
                                                FileName = item.filename,
                                                FileSize = item.filesize
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.HH_VILLA_DASHBOARD);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View(string.Format(ViewPath, "ViewApplication"), model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DownloadViewAttachment(string docno, string apprefno, string filename)
        {
            DownloadFileRequest downloadFileRequest = new DownloadFileRequest();
            byte[] downloadFile = new byte[0];
            string fileMimeType = "";
            string strFileExtension = (!string.IsNullOrWhiteSpace(filename)) ? System.IO.Path.GetExtension(filename).Replace(".", "") : string.Empty;
            try
            {
                downloadFileRequest.attachrequest = new Attachrequest
                {
                    documentnumber = docno,
                    flag = "X",
                    applicationreferencenumber = apprefno,
                };
                var downloadResponse = VillaCostExemptionClient.DownloadFile(downloadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                if (downloadResponse.Succeeded && downloadResponse.Payload != null && downloadResponse.Payload.attachlist != null)
                {
                    var filedata = downloadResponse.Payload.attachlist.Where(x => x.documentnumber == docno).FirstOrDefault();
                    if (filedata != null)
                    {
                        downloadFile = filedata.content;
                        fileMimeType = filedata.attachmenttype;
                    }
                }
                else
                {
                    fileMimeType = "application/" + strFileExtension + "";
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return File(downloadFile, fileMimeType, filename);
        }

        #endregion View Application

        #endregion Action

        #region Private methods

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult IsSessionExpired()
        {
            if (!IsLoggedIn && !CurrentPrincipal.Role.Equals(Roles.User))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetICADetails(string idType, string dateofbirth, string gender, string idnumber, string documentnumber, string nationality)
        {
            var formattedDOB = string.Empty;
            OwnerDetail ownerDetail = new OwnerDetail();
            try
            {
                if (!string.IsNullOrWhiteSpace(dateofbirth))
                {
                    formattedDOB = FormatDate(dateofbirth).ToString("dd.MM.yyyy");
                }
                ICADetailsRequest iCADetailsRequest = new ICADetailsRequest();
                eiddetails eiddetails = new eiddetails();
                passportdetails passportdetails = new passportdetails();
                if (!string.IsNullOrWhiteSpace(idType) && idType.Equals("01"))
                {
                    eiddetails.dateofbirth = (idType != null && idType.Equals("01")) ? formattedDOB : "";
                    eiddetails.gender = (idType != null && idType.Equals("01")) ? gender : "";
                    eiddetails.idnumber = idnumber;
                }
                else
                {
                    passportdetails.dateofbirth = (idType != null && !idType.Equals("01")) ? formattedDOB : "";
                    passportdetails.gender = (idType != null && !idType.Equals("01")) ? gender : "";
                    passportdetails.nationality = nationality;
                    passportdetails.documentnumber = documentnumber;
                }
                iCADetailsRequest.eiddetails = eiddetails;
                iCADetailsRequest.passportdetails = passportdetails;

                var response = VillaCostExemptionClient.GetICADetails(iCADetailsRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                if (response != null && response.Succeeded && response.Payload != null)
                {
                    ownerDetail.DateOfBirth = dateofbirth;
                    ownerDetail.Email = response.Payload.email;
                    ownerDetail.EmiratesID = response.Payload.emiratesid;
                    ownerDetail.IdType = idType;
                    ownerDetail.Marsoom = response.Payload.marsoom;
                    ownerDetail.Mobile1 = response.Payload.mobile;
                    ownerDetail.Name = response.Payload.fullname;
                    ownerDetail.PassportNumber = response.Payload.passportnumber;
                    ownerDetail.PassportExpiry = DateToStringFormat(response.Payload.passportexpirydate); //!string.IsNullOrWhiteSpace(response.Payload.passportexpirydate) && response.Payload.passportexpirydate != "0000-00-00" ? DateTime.Parse(response.Payload.passportexpirydate).ToString("dd MMM yyyy", SitecoreX.Culture) : "";
                    ownerDetail.IssuingAuthority = response.Payload.passportissuingauthority;
                    ownerDetail.MaskedEmail = response.Payload.maskedemail;
                    ownerDetail.MaskedMobile = response.Payload.maskedmobile;
                    ownerDetail.Success = true;
                }
                else
                {
                    ownerDetail.Success = false;
                    ownerDetail.ErrorMsg = response.Payload.description;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return Json(ownerDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult uploadOwnerAttachment(OwnerDetail ownerDetail)
        {
            try
            {
                List<OwnerDetail> _cacheOwnerDetail = new List<OwnerDetail>();
                if (CacheProvider.TryGet(OwnerAttachment, out _cacheOwnerDetail))
                {
                    if (_cacheOwnerDetail != null)
                    {
                        var item = _cacheOwnerDetail.Where(x => x.PassportNumber.Equals(ownerDetail.PassportNumber)).FirstOrDefault();
                        if (item != null)
                        {
                            if (ownerDetail.PassportCopy != null)
                            {
                                item.PassportCopy = ownerDetail.PassportCopy;
                                item.PassportCopy_Binary = ownerDetail.PassportCopy.ToArray();
                            }
                        }
                        else
                        {
                            ownerDetail.PassportCopy_Binary = ownerDetail.PassportCopy.ToArray();
                            _cacheOwnerDetail.Add(ownerDetail);
                        }
                    }
                }
                else
                {
                    _cacheOwnerDetail = new List<OwnerDetail>();
                    ownerDetail.PassportCopy_Binary = ownerDetail.PassportCopy.ToArray();
                    _cacheOwnerDetail.Add(ownerDetail);
                }
                CacheProvider.Store(OwnerAttachment, new CacheItem<List<OwnerDetail>>(_cacheOwnerDetail));

                return Json(new { status = true, Message = "Saved" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult getOwnerAttachment(string PassportNumber)
        {
            byte[] buffer = null;
            try
            {
                List<OwnerDetail> _cacheOwnerDetail = new List<OwnerDetail>();
                if (CacheProvider.TryGet(OwnerAttachment, out _cacheOwnerDetail))
                {
                    if (_cacheOwnerDetail != null)
                    {
                        var item = _cacheOwnerDetail.Where(x => x.PassportNumber.Equals(PassportNumber)).FirstOrDefault();
                        if (item != null)
                        {
                            buffer = item.PassportCopy_Binary;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, content = buffer }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult removeOwnerAttachment(string PassportNumber)
        {
            try
            {
                List<OwnerDetail> _cacheOwnerDetail = new List<OwnerDetail>();
                if (CacheProvider.TryGet(OwnerAttachment, out _cacheOwnerDetail))
                {
                    if (_cacheOwnerDetail != null)
                    {
                        var item = _cacheOwnerDetail.Where(x => x.PassportNumber.Equals(PassportNumber)).FirstOrDefault();
                        _cacheOwnerDetail.Remove(item);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        private DateTime FormatDate(string datetime) => Convert.ToDateTime(datetime.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December"));

        private string DateToStringFormat(string dateVal)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(dateVal) && dateVal != "0000-00-00" ? DateTime.Parse(dateVal).ToString("dd MMMM yyyy", SitecoreX.Culture) : "";
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return string.Empty;
        }

        private string CheckEmptyString(string strVal)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(strVal) ? strVal.Trim() : "";
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return string.Empty;
        }

        private bool uploadAttachments(HttpPostedFileBase attachItem, string applicationRefNo, string attchmentType, string itemNumber)
        {
            UploadFileRequest uploadFileRequest;
            string error = string.Empty;
            try
            {
                if (attachItem != null)
                {
                    if (CustomeAttachmentIsValid(attachItem, FileSize, out error, FileType))
                    {
                        uploadFileRequest = new UploadFileRequest();
                        using (var memoryStream_8 = new MemoryStream())
                        {
                            attachItem.InputStream.CopyTo(memoryStream_8);
                            string strFileExtension = System.IO.Path.GetExtension(attachItem.FileName).Replace(".", "");
                            uploadFileRequest.attachrequest = new Attachrequest
                            {
                                action = "C",
                                itemnumber = itemNumber,
                                attachmenttype = attchmentType,
                                applicationreferencenumber = applicationRefNo,
                                content = Convert.ToBase64String(memoryStream_8.ToArray() ?? new byte[0]),
                                mimetype = strFileExtension,
                                filename = applicationRefNo + itemNumber + attchmentType
                            };
                            var response = VillaCostExemptionClient.UploadFile(uploadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);

                            if (response != null && response.Payload != null && response.Payload.responsecode != null && !response.Payload.responsecode.Equals("000"))
                            {
                                ModelState.AddModelError(string.Empty, response.Payload.description);
                                return false;
                            }
                            if (response != null && response.Payload == null && !string.IsNullOrWhiteSpace(response.Message))
                            {
                                ModelState.AddModelError(string.Empty, response.Message);
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                return false;
            }
            return true;
        }

        public bool DeleteAttachments(string applicationRefNo, string attchmentType, string documentNo, string mimetype)
        {
            try
            {
                UploadFileRequest uploadFileRequest = new UploadFileRequest(); ;
                string error = string.Empty;

                uploadFileRequest.attachrequest = new Attachrequest
                {
                    action = "D",
                    itemnumber = "0010",
                    mimetype = mimetype,
                    attachmenttype = attchmentType,
                    applicationreferencenumber = applicationRefNo,
                    content = "",
                    documentnumber = documentNo,
                    filename = applicationRefNo + "0010" + attchmentType
                };
                var response = VillaCostExemptionClient.UploadFile(uploadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);

                if (response != null && response.Payload != null && response.Payload.responsecode != null && !response.Payload.responsecode.Equals("000"))
                {
                    ModelState.AddModelError(string.Empty, response.Payload.description);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return true;
        }

        private DashboardPageModel GetExistingApplications(bool fetchFresh = false)
        {
            List<CustomerDetail> customerDetails;
            DashboardPageModel model = new DashboardPageModel();
            if (!fetchFresh && CacheProvider.TryGet<DashboardPageModel>(ExistingApplications, out model))
            {
                return model;
            }
            customerDetails = new List<CustomerDetail>();
            try
            {
                var dashboardResponse = VillaCostExemptionClient.GetDashboardResources(CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                //var res = dashboardResponse?.Payload?.Customerdetails?.FirstOrDefault();

                if (dashboardResponse.Succeeded && dashboardResponse?.Payload?.Customerdetails?.Count > 0)
                {
                    if (dashboardResponse.Payload.Ownertypes != null)
                        model.OwnerTypeList = dashboardResponse.Payload.Ownertypes.Select(x => new SelectListItem() { Text = x.Value, Value = x.Key }).ToList();
                    if (dashboardResponse.Payload.Statustypes != null)
                        model.StatusTypeList = dashboardResponse.Payload.Statustypes.Select(x => new SelectListItem() { Text = x.Value, Value = x.Key }).ToList();

                    foreach (var res in dashboardResponse.Payload.Customerdetails.OrderByDescending(x => Convert.ToDateTime(x.Dateofrecordcreation)).ThenByDescending(d => Convert.ToDateTime(d.Timeofrecordcreation)))
                    {
                        CustomerDetail d = new CustomerDetail();
                        d.CreatedOnDateTime = res.Dateofrecordcreation + " " + res.Timeofrecordcreation;
                        d.CustomerNumber = res.Customernumber ?? string.Empty;
                        d.EstimateNumber = res.Estimate ?? string.Empty;
                        d.SequenceNumber = res.ApplicationSequencenumber ?? string.Empty;
                        d.Number = res.Applicationnumber ?? string.Empty;
                        d.OwnerTypeDescription = res.Ownertypedescription ?? string.Empty;
                        d.OwnerType = res.Ownertype ?? string.Empty;
                        d.Reference = res.Applicationreferencenumber ?? string.Empty;
                        d.Remarks = res.Remarks ?? string.Empty;
                        if (res.Ownerdetails != null)
                        {
                            d.OwnerDetails = res.Ownerdetails.Select((o, key) => new OwnerDetail()
                            {
                                OwnerID = (key + 1).ToString(),
                                IdType = o.IdType ?? string.Empty,
                                Itemnumber = o.Itemnumber ?? string.Empty,
                                Marsoom = o.Marsoom ?? string.Empty,
                                Email = o.Email ?? string.Empty,
                                EmiratesID = o.Emiratesid ?? string.Empty,
                                PassportNumber = o.Passport ?? string.Empty,
                                Mobile1 = o.Mobile ?? string.Empty,
                                Name = o.Name ?? string.Empty,
                                Relation = o.Relation ?? string.Empty,
                                Mobile2 = o.Mobile2 ?? string.Empty,
                                PassportExpiry = DateToStringFormat(o.Passportexpiry), //!string.IsNullOrWhiteSpace(o.Passportexpiry) && o.Passportexpiry != "0000-00-00" && !o.Passportexpiry.Equals("--") ? o.Passportexpiry : "",
                                IssuingAuthority = o.PassportissueAuthority
                            }).ToList();
                        }
                        d.Status = res.Applicationstatus;
                        d.StatusDesc = res.Applicationstatusdesc;
                        if (res.Ownerattachments != null)
                        {
                            d.OwnerAttachments = res.Ownerattachments.Select(x => new DEWAXP.Feature.GeneralServices.Models.VillaCostExemption.Ownerattachment
                            {
                                dateofrecordcreation = x.Dateofrecordcreation,
                                documentnumber = x.Documentnumber,
                                filename = x.Filename,
                                ownerapplicationitemnumber = x.Ownerapplicationitemnumber,
                                ownerapplicationreferencenumber = x.Ownerapplicationreferencenumber,
                                requiredflag = x.Requiredflag,
                                timeofrecordcreation = x.Timeofrecordcreation,
                                uploadedflag = x.Uploadedflag
                            }).ToList();
                        }
                        if (res.History != null)
                        {
                            var history = res.History.Select(x => $"{{\"date\":\"{DateToStringFormat(x.Hisdateofrecordcreation)}\",\"status\":\"{x.Hisapplicationstatus}\",\"statusdesc\":\"{x.Hisapplicationstatusdesc}\",\"remarks\":\"{CheckEmptyString(x.Hisremarks)}\"}}").ToArray();
                            if (history != null && history.Count() > 0)
                            {
                                d.History = $"{{\"appno\":\"{d.Number}\", \"data\":[{string.Join(",", history)}]}}";
                            }
                            else
                            {
                                d.History = "[]";
                            }
                        }
                        /*d.History = res.History.Select(h => new BaseApplication()
                        {
                            CreatedOnDateTime = h.Hisdateofrecordcreation + " " + h.Histimeofrecordcreation,
                            CustomerNumber = h.Hiscustomernumber ?? string.Empty,
                            EstimateNumber = h.Hisestimate ?? string.Empty,
                            Number = h.Hisapplicationnumber ?? string.Empty,
                            OwnerType = h.Hisownertype ?? string.Empty,
                            OwnerTypeDescription = h.Hisownertypedescription ?? string.Empty,
                            Reference = h.Hisapplicationreferencenumber ?? string.Empty,
                            Remarks = h.Hisremarks ?? string.Empty,
                            Status = h.Hisapplicationstatus ?? string.Empty,
                            Version = h.Hisversion ?? string.Empty
                        }).ToList();*/

                        customerDetails.Add(d);
                    }

                    model.CustomerDetails = customerDetails;
                    CacheProvider.Store(ExistingApplications, new CacheItem<DashboardPageModel>(model));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return model;
        }

        private UploadFileResponse uploadAttachment(UploadFileRequest uploadFileRequest)
        {
            try
            {
                var response = VillaCostExemptionClient.UploadFile(uploadFileRequest, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.responsecode.Equals("000"))
                {
                    return response.Payload;
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return new UploadFileResponse();
        }

        private List<CountryList> GetCountryList()
        {
            List<CountryList> countrylist = new List<CountryList>();
            if (CacheProvider.TryGet<List<CountryList>>(CountryList, out countrylist))
            {
                return countrylist;
            }
            else
            {
                var response = VillaCostExemptionClient.GetCountryList(new CountryListRequest { }, CurrentPrincipal.SessionToken, CurrentPrincipal.Username, Request.Segment(), RequestLanguage);

                if (response != null && response.Succeeded && response.Payload != null && response.Payload.countryList != null && response.Payload.countryList.Count > 0)
                {
                    countrylist = response.Payload.countryList;
                }
                CacheProvider.Store(CountryList, new CacheItem<List<CountryList>>(countrylist));
            }
            return countrylist;
        }

        #endregion Private methods
    }

    public class MemoryPostedFile : HttpPostedFileBase
    {
        private readonly byte[] fileBytes;

        public MemoryPostedFile(byte[] fileBytes, string fileName = null, string contentType = null)
        {
            this.fileBytes = fileBytes;
            this.FileName = fileName;
            this.ContentType = contentType;
            this.InputStream = new MemoryStream(fileBytes);
        }

        public override int ContentLength => fileBytes.Length;
        public override string ContentType { get; }
        public override string FileName { get; }

        public override Stream InputStream { get; }
    }
}