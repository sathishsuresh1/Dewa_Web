using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Feature.SupplyManagement.Helpers.SmartResponse;
using DEWAXP.Feature.SupplyManagement.Models.Complaints;
using DEWAXP.Feature.SupplyManagement.Models.SmartResponseModel;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartResponseModel;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using DEWAXP.Foundation.Logger;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _commonUtility = DEWAXP.Foundation.Content.Utils.CommonUtility;
using _SM_CommonHelper = DEWAXP.Feature.SupplyManagement.Helpers.SmartResponse.SmartResponseHelper;
using DEWAXP.Foundation.Content.Models.SmartResponseModel;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;
using DEWAXP.Foundation.Content.Services;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class ComplaintsController : BaseController
    {
        private readonly IDropdownHelper _dropdownHelper;
        protected IDropdownHelper DropdownHelper => _dropdownHelper;
        public ComplaintsController() : base()
        {
            _dropdownHelper = DependencyResolver.Current.GetService<IDropdownHelper>();
        }
        public ActionResult ServiceComplaint()
        {
            var result = DewaApiClient.GetServiceComplaintCriteria(RequestLanguage, Request.Segment());

            if (result.Succeeded)
            {
                // Get the page data
                //Commented by Adeel
                //ViewBag.Page = SitecoreContext.GetCurrentItem<GenericPageWithIntro>();

                //Modified by Adeel
                string errorMessage = string.Empty;
                ServiceComplaint persistmodel = null;
                var content = ContextRepository.GetCurrentItem<GenericPageWithIntro>();

                CacheProvider.Store(CacheKeys.Support_kiosk, new CacheItem<string>(RenderingContext.Current.Rendering.Parameters["Kiosk_Service_Sesrvice"]));

                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    CacheProvider.TryGet(CacheKeys.SERVICECOMPLAINT_DETAILS, out persistmodel);
                    if (persistmodel == null) persistmodel = new ServiceComplaint();
                    CacheProvider.Remove(CacheKeys.SERVICECOMPLAINT_DETAILS);
                    CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                }
                else
                {
                    persistmodel = new ServiceComplaint();
                }
                persistmodel.Intro = content.Intro;
                persistmodel.Header = content.Header;

                if (IsLoggedIn)
                {
                    // Populate dropdown and the other complaints URL
                    ViewBag.Complaints = DropdownHelper.ComplaintsDropdown(result.Payload.ComplaintCodeList);
                    ViewBag.BillingUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J24_BILLING_COMPLAINT);
                    return PartialView("~/Views/Feature/SupplyManagement/Complaints/_ServiceComplaint.cshtml", persistmodel);
                }

                // Populate dropdowns
                ViewBag.Cities = DropdownHelper.CityDropdown(result.Payload.CityList);
                ViewBag.Complaints = DropdownHelper.ComplaintsDropdown(result.Payload.ComplaintCodeList);
                return PartialView("~/Views/Feature/SupplyManagement/Complaints/_ServiceComplaint_Prelogin.cshtml", persistmodel);
            }

            CacheProvider.Store(CacheKeys.COMPLAINT_FAILED, new CacheItem<string>(result.Message));
            //return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_COMPLAINT_FAILED);
            return PartialView("~/Views/Feature/SupplyManagement/Complaints/_ServiceComplaint_Prelogin.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MakeServiceComplaint(ServiceComplaint model)
        {
            string support_service_success;
            try
            {
                CacheProvider.Store(CacheKeys.SERVICECOMPLAINT_DETAILS, new CacheItem<ServiceComplaint>(model));
                string serror;
                if (model.File != null && model.File.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.File, General.MaxAttachmentSize, out serror, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, serror);
                    }
                }

                if (ModelState.IsValid)
                {
                    var complaintType = model.ComplaintType;

                    // Create the object from the model
                    var serviceComplaint = new LodgeServiceComplaint
                    {
                        City = model.City,
                        Code = complaintType.Split('|')[1],
                        CodeGroup = complaintType.Split('|')[0],
                        MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                        Name = model.ContactName,
                        Remarks = model.Description,
                        ContractAccountNumber = model.AccountNumber,
                    };

                    // Check if we must set GPS coordinates
                    if (model.LocationCoordinates)
                    {
                        serviceComplaint.GpsXCoordinates = model.xGPS;
                        serviceComplaint.GpsYCoordinates = model.yGPS;
                    }

                    // Read in attachment if it exists
                    if (model.File != null)
                    {
                        serviceComplaint.DocumentData = model.File.ToArray();
                    }

                    // Make the call
                    var result = DewaApiClient.LodgeServiceComplaint(
                        CurrentPrincipal.UserId,
                        CurrentPrincipal.SessionToken,
                        serviceComplaint,
                        RequestLanguage, Request.Segment());

                    // If successful, redirect to the success page with the reference number
                    if (result.Succeeded)
                    {
                        if (CacheProvider.TryGet(CacheKeys.Support_kiosk, out support_service_success) && !string.IsNullOrEmpty(support_service_success))
                        {
                            var viewModel = new ReferenceNumber(result.Payload, SitecoreItemIdentifiers.Kiosk_HomePage, DictionaryKeys.Global.Home);
                            CacheProvider.Store(CacheKeys.COMPLAINT_SENT, new CacheItem<ReferenceNumber>(viewModel));

                            CacheProvider.Remove(CacheKeys.Support_kiosk);
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.Kiosk_Service_Success);
                        }
                        else
                        {
                            var viewModel = new ReferenceNumber(result.Payload, SitecoreItemIdentifiers.HOME, DictionaryKeys.Global.Home);
                            CacheProvider.Store(CacheKeys.COMPLAINT_SENT, new CacheItem<ReferenceNumber>(viewModel));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_COMPLAINT_SENT);
                        }
                    }

                    CacheProvider.Store(CacheKeys.COMPLAINT_FAILED, new CacheItem<string>(result.Message));
                    if (CacheProvider.TryGet(CacheKeys.Support_kiosk, out support_service_success) && !string.IsNullOrEmpty(support_service_success))
                    {
                        CacheProvider.Remove(CacheKeys.Support_kiosk);
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.Kiosk_Service_Failed);
                    }
                    else
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_COMPLAINT_FAILED);
                    }
                }
                var error = ViewData.ModelState.AsFormattedString();
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(error));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J22_SERVICE_COMPLAINT);
            }
            catch (Exception ex)
            {
                Error.LogError(string.Format("Unable to lodge a service complaint. Message: {0}", ex.Message));
                CacheProvider.Store(CacheKeys.COMPLAINT_FAILED, new CacheItem<string>(ex.Message));
            }
            if (CacheProvider.TryGet(CacheKeys.Support_kiosk, out support_service_success) && !string.IsNullOrEmpty(support_service_success))
            {
                CacheProvider.Remove(CacheKeys.Support_kiosk);
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.Kiosk_Service_Failed);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_COMPLAINT_FAILED);
            }
        }

        [TwoPhaseAuthorize, HttpGet]
        public ActionResult BillingComplaint()
        {
            BillingComplaint persistmodel = new BillingComplaint();
            var result = DewaApiClient.GetServiceComplaintCriteria(RequestLanguage, Request.Segment());
            if (result.Succeeded)
            {
                string errorMessage;

                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    CacheProvider.TryGet(CacheKeys.BILLINGCOMPLAINT_DETAILS, out persistmodel);
                    if (persistmodel == null) persistmodel = new BillingComplaint();
                    CacheProvider.Remove(CacheKeys.BILLINGCOMPLAINT_DETAILS);
                    CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                }

                // Populate dropdown, page and the other complaints URL
                //ViewBag.Page = SitecoreContext.GetCurrentItem<GenericPageWithIntro>();
                var content = ContextRepository.GetCurrentItem<GenericPageWithIntro>();
                persistmodel.Intro = content.Intro;
                ViewBag.Complaints = DropdownHelper.ComplaintsDropdown(result.Payload.ComplaintCodeList);
            }

            CacheProvider.Store(CacheKeys.COMPLAINT_FAILED, new CacheItem<string>(result.Message));
            return PartialView("~/Views/Feature/SupplyManagement/Complaints/_BillingComplaint.cshtml", persistmodel);
        }

        [TwoPhaseAuthorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult MakeBillingComplaint(BillingComplaint model)
        {
            try
            {
                // Ensure the file isn't too big
                CacheProvider.Store(CacheKeys.BILLINGCOMPLAINT_DETAILS, new CacheItem<BillingComplaint>(model));
                string error;
                if (model.ComplaintAttachmentUploader != null && model.ComplaintAttachmentUploader.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.ComplaintAttachmentUploader, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }

                if (!model.AgreedToPayment)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("You must agree to pay a penalty if meter is found in working order"));
                }

                if (ModelState.IsValid)
                {
                    // Fetch the selected account
                    //var accounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
                    var accounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

                    var selectedAccount = accounts.Payload.First(c => c.AccountNumber == model.AccountNumber);
                    var bpNumber = selectedAccount.BusinessPartnerNumber;

                    // Create the request
                    var billingComplaint = new LodgeBillingComplaint
                    {
                        ContractAccountNumber = selectedAccount.AccountNumber,
                        MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                        Remarks = model.Description,
                        PremiseNumber = selectedAccount.PremiseNumber,
                        BusinessPartnerNumber = bpNumber,
                        Priority = model.RequestCategory,
                        AffectedService = model.ComplaintType,
                    };

                    // Read in attachment if it exists
                    if (model.ComplaintAttachmentUploader != null)
                    {
                        billingComplaint.Attachment = model.ComplaintAttachmentUploader.ToArray();
                        billingComplaint.AttachmentExtension = model.ComplaintAttachmentUploader.GetTrimmedFileExtension();
                    }
                    else
                    {
                        billingComplaint.Attachment = new byte[0];
                        billingComplaint.AttachmentExtension = string.Empty;
                    }

                    // Make the call
                    var result = DewaApiClient.LodgeBillingComplaint(
                        CurrentPrincipal.UserId,
                        CurrentPrincipal.SessionToken,
                        billingComplaint,
                        RequestLanguage, Request.Segment());

                    // If successful, redirect to the success page with the reference number
                    if (result.Succeeded)
                    {
                        var viewModel = new ReferenceNumber(result.Payload, SitecoreItemIdentifiers.J22_TRACK_COMPLAINTS, "Track your complaints");
                        CacheProvider.Store(CacheKeys.COMPLAINT_SENT, new CacheItem<ReferenceNumber>(viewModel));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_COMPLAINT_SENT);
                    }

                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(result.Message));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_BILLING_COMPLAINT);
                }

                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ViewData.ModelState.AsFormattedString()));

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_BILLING_COMPLAINT);
            }
            catch (Exception ex)
            {
                Error.LogError(string.Format("Unable to lodge a billing complaint. Message: {0}", ex.Message));
                CacheProvider.Store(CacheKeys.COMPLAINT_FAILED, new CacheItem<string>(ex.Message));
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J24_COMPLAINT_FAILED);
        }

        [TwoPhaseAuthorize, HttpGet]
        public PartialViewResult TrackComplaints()
        {
            var model = ContextRepository.GetCurrentItem<GenericPageWithIntro>();
            return PartialView("~/Views/Feature/SupplyManagement/Complaints/_TrackComplaints.cshtml", model);
        }

        [HttpGet]
        public PartialViewResult ComplaintFailed()
        {
            string message;
            CacheProvider.TryGet(CacheKeys.COMPLAINT_FAILED, out message);
            ViewBag.Message = message;
            return PartialView("~/Views/Feature/SupplyManagement/Complaints/_ComplaintFailed.cshtml");
        }

        [HttpGet]
        public ActionResult ComplaintSent()
        {
            ReferenceNumber viewModel;
            if (!CacheProvider.TryGet(CacheKeys.COMPLAINT_SENT, out viewModel))
                return RedirectToAction("ComplaintFailed");

            return PartialView("~/Views/Feature/SupplyManagement/Complaints/_ComplaintSent.cshtml", viewModel);
        }

        #region Private

        //private void ValidateServiceComplaint(ServiceComplaint model)
        //{
        //	// Ensure the file isn't too big
        //	ValidateAttachment(model.File);

        //	// Check if one is logged in and requirements are met
        //	if (IsLoggedIn)
        //	{
        //		if (string.IsNullOrEmpty(model.MobileNumber))
        //			ModelState.AddModelError("", Translate.Text("A mobile number is required"));
        //	}
        //}

        //private void ValidateAttachment(HttpPostedFileBase file)
        //{
        //	if (file == null) return;

        //	string[] acceptedFileTypes = { ".PDF", ".TXT", ".DOC", ".JPG", ".JPEG", ".PNG", ".BMP", ".TIFF", "GIF", ".ZIP", ".RAR", ".XLS", ".XLSX", ".DOCX" };
        //	var extension = Path.GetExtension(file.FileName);
        //	if (!string.IsNullOrWhiteSpace(extension))
        //	{
        //		// The web-services only accept 3 character extensions. .jpeg would thus be seen as "corrupt" in the SAP system.
        //		if (extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
        //		{
        //			extension = ".jpg";
        //		}
        //		if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
        //		{
        //			extension = ".doc";
        //		}
        //		if (extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //		{
        //			extension = ".xls";
        //		}
        //	}
        //	if (!string.IsNullOrEmpty(extension) && !acceptedFileTypes.Contains(extension.ToUpper()))
        //	{
        //		ModelState.AddModelError(string.Empty, Translate.Text("invalid file type validation message"));
        //	}

        //	var fileSize = file.ContentLength / 1024 / 1024;
        //	if (fileSize > 2)
        //	{
        //		ModelState.AddModelError("", Translate.Text("The file may not be bigger than 2MB"));
        //	}
        //}

        #endregion Private

        #region Smart Response

        //[TwoPhaseAuthorize]
        public ActionResult SmartDubaiComplaint(SmPageRequestModel pgRqst)
        {
            if (Convert.ToBoolean(pgRqst.se == "0x00x"))
            {
                _SM_CommonHelper.Clear_SM_Session();
            }

            var lanType = _SM_CommonHelper.GetSelectedAnswerValue(Models.SmartResponseModel.SM_Id.LangType);

            #region [Set Language on SC lang]

            if (lanType == null || lanType == "")
            {
                if (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic)
                {
                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.LangType, SmlangCode.ar.ToString());
                }
                else if (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic)
                {
                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(Models.SmartResponseModel.SM_Id.LangType, SmlangCode.en.ToString());
                }
            }
            lanType = _SM_CommonHelper.GetSelectedAnswerValue(Models.SmartResponseModel.SM_Id.LangType);

            #endregion [Set Language on SC lang]

            var _currentRequest = new CommonRenderRequest()
            {
                IsPageRefesh = Convert.ToBoolean(pgRqst.isLang == 0),
                LangType = _SM_CommonHelper.GetSMLangType(lanType),
            };
            CommonRender model = new CommonRender()
            {
                CurrentRequest = _currentRequest,
            };

            bool AccountSeletor = Convert.ToBoolean(pgRqst.asc) && IsLoggedIn;

            ViewBag.ShowAccountSelector = AccountSeletor;
            if (AccountSeletor && SmartResponseSessionHelper.CurrentUserAnswer != null)
            {
                model = SmartResponseSessionHelper.CurrentUserAnswer.LastOrDefault().Value;
                if (model != null)
                {
                    if (model.CurrentRequest.LangType == null)
                    {
                        model.CurrentRequest.LangType = _SM_CommonHelper.GetSMLangType(_SM_CommonHelper.GetSelectedAnswerValue(SM_Id.LangType));
                    }
                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.LangType, model.CurrentRequest.LangType.ToString());
                    model.CurrentRequest.IsPageRefesh = true;
                }
            }
            //handled langauge on page redirect.
            if (pgRqst.l.HasValue)
            {
                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.LangType, pgRqst.l.Value.ToString());
            }

            SmartResponseSessionHelper.ElectricityComplaintJsonSetting = _SM_CommonHelper.LoadUpdatedSmartDubaiModelJson();
            //SessionHelper.TempElectricityComplaintJsonSetting = new JsonMasterModel() { Questions = SessionHelper.ElectricityComplaintJsonSetting.Questions };

            ///if directly wants to land on notification list page

            if (!string.IsNullOrWhiteSpace(pgRqst.n))
            {
                model = _SM_CommonHelper.GetGuestNotiicationListModel(pgRqst.n, Convert.ToInt32(pgRqst.t));
                ViewBag.ShowNotification = true;
            }

            if (pgRqst.s != null && pgRqst.s.HasValue)
            {
                ViewBag.ShowNotification = true;
                switch (pgRqst.s.Value)
                {
                    case SmScreenCode.ant:
                        model = _SM_CommonHelper.GetTrackOtherIncident(SmartResponseSessionHelper.ElectricityComplaintJsonSetting);
                        break;

                    case SmScreenCode.d:
                    default:
                        ViewBag.ShowNotification = false;
                        break;
                }
            }

            if (model != null && model.CurrentRequest == null)
            {
                _currentRequest.IsPageRefesh = false;
                _currentRequest.TckId = 1;
                model.CurrentRequest = _currentRequest;
            }
            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.IsUserLoggedIn, Convert.ToString(IsLoggedIn ? 1 : 0));
            return PartialView("~/Views/Feature/SupplyManagement/Complaints/_ElectricityComplaint.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SmartDubaiGenericScreen(Models.SmartResponseModel.CommonRenderRequest request)
        {
            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.IsUserLoggedIn, Convert.ToString(IsLoggedIn ? 1 : 0));
            bool isPreviousError = false;
            List<SM_ErrorDetail> previousErrorDetails = null;

            CommonRender prevoiusAnswer = null;
            SmlangCode PrevLang = _SM_CommonHelper.GetSMLangType(_SM_CommonHelper.GetSelectedAnswerValue(SM_Id.LangType));

            if (!request.IsPageBack)
            {
                if (SmartResponseSessionHelper.CurrentUserAnswer != null && !request.IsStart && request.TckId == 0)
                {
                    prevoiusAnswer = SmartResponseSessionHelper.CurrentUserAnswer.LastOrDefault().Value;
                    //
                    if (prevoiusAnswer != null)
                    {
                        if (prevoiusAnswer.Answer.Action == Models.SmartResponseModel.SM_Action.Customerlogin)
                        {
                            prevoiusAnswer = SmartResponseSessionHelper.CurrentUserAnswer?[SmartResponseSessionHelper.CurrentUserAnswer.Count - 2];
                        }
                        if (prevoiusAnswer != null && prevoiusAnswer.Answer.Action == SM_Action.Accountcountcheck)
                        {
                            prevoiusAnswer.RedirectCount = 0;
                        }
                    }
                    int lastTrackingId = prevoiusAnswer.Answer.TrackingId;
                    request.TckId = lastTrackingId > 1 ? lastTrackingId : request.TckId;
                }
            }
            if (request.LangType == null)
            {
                request.LangType = PrevLang;
            }

            if (request.IsAnsAltered)
            {
                SmartResponseSessionHelper.ElectricityComplaintJsonSetting = _SM_CommonHelper.LoadUpdatedSmartDubaiModelJson();
                request.IsAnsAltered = false;
            }

        INITIATE_CURRENT_ANS:
            var currentAns = _SM_CommonHelper.GetRenderModel(request, SmartResponseSessionHelper.ElectricityComplaintJsonSetting);
            //var tempCurrentAns = _SM_CommonHelper.GetRenderModel(request, SessionHelper.TempElectricityComplaintJsonSetting);

            if (currentAns.Answer.Action == SM_Action.ReportTechnicalIncident)
            {
                var d = _SM_CommonHelper.GetReportTechnicalIncident(SmartResponseSessionHelper.ElectricityComplaintJsonSetting);
                if (d != null && d.Answer != null)
                {
                    currentAns = d;
                }
            }

            try
            {
                if (currentAns.Answer.Action == SM_Action.TrackAnotherIncident)
                {
                    var d = _SM_CommonHelper.GetTrackOtherIncident(SmartResponseSessionHelper.ElectricityComplaintJsonSetting);
                    if (d != null && d.Answer != null)
                    {
                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.NotificationNumber, "");
                        currentAns = d;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            currentAns.CurrentRequest = request;
            if (!Convert.ToBoolean(currentAns.Answer.disabled))
            {
                currentAns = SR_ActionHelper(currentAns);//
                currentAns = SR_TypeHelper(currentAns);
                currentAns = SR_IdHelper(currentAns);
            }

            #region [Error handling]

            if (!isPreviousError && (currentAns.IsError || currentAns.Answer.Action == SM_Action.Resendotpmobile))
            {
                isPreviousError = currentAns.IsError;
                previousErrorDetails = currentAns.ErrorDetails;
                request.TckId = currentAns.Answer.ParentTrackingId;
                request.IsPageBack = isPreviousError;
                goto INITIATE_CURRENT_ANS;
            }

            if (isPreviousError)
            {
                currentAns.IsError = isPreviousError;
                currentAns.ErrorDetails = previousErrorDetails;
                currentAns.CurrentRequest = request;
            }

            #endregion [Error handling]

            var activeUser = AuthStateService.GetActiveProfile();

            CustomerSubmittedData smDetail = _SM_CommonHelper.GetUserSubmittedData();
            //Add username in Required place
            //int i = 0;
            foreach (var item in currentAns.Answer.Questions.Where(x => x != null).ToList())
            {
                string username = IsLoggedIn && activeUser != null ? activeUser.Username : _SM_CommonHelper.GetSMTranslation("User");
                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.UsernName, username);
                //item.Value = _SM_CommonHelper.GetSMTranslation(item.Value)?.Replace("{{username}}", _SM_CommonHelper.TextHighlight(username));

                if (item.Answers != null)
                {
                    if (item.Answers.FirstOrDefault()?.Type == Models.SmartResponseModel.TypeEnum.Confirmation)
                    {
                        //review page logic
                        currentAns.RequestConfirmationDetail = new Models.SmartResponseModel.RequestConfirmationDetail();
                        currentAns.RequestConfirmationDetail.ContactAccountNo = _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.Account);
                        currentAns.RequestConfirmationDetail.ContactPerson = _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.ContactPersonName);
                        currentAns.RequestConfirmationDetail.ContactNumber = _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.Mobile);
                        currentAns.RequestConfirmationDetail.ContactLocation = _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.CA_Location) + " " + _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.Street);
                    }
                    var filterAns = item.Answers.Where(x => x != null && x.Action == SM_Action.GetNotificationList).FirstOrDefault();
                    if (filterAns != null)
                    {
                        //item.Value = item.Value?.Replace("{{notif}}", _SM_CommonHelper.TextHighlight(smDetail.NotificationNumber));
                        filterAns.Actiondata = smDetail.NotificationNumber;
                    }
                }
            }

            //var currentQus = Utils.SmartDubaiModel.CommonHelper.GetRenderModel(request.QusId, request.IsStart);
            currentAns.RedirectCount = Convert.ToInt32(prevoiusAnswer?.RedirectCount ?? currentAns.RedirectCount) + 1; //this to stop multiple page redirect

            if (!request.IsStart)
            {
                if (SmartResponseSessionHelper.CurrentUserAnswer == null)
                {
                    SmartResponseSessionHelper.CurrentUserAnswer = new System.Collections.Generic.Dictionary<int, Models.SmartResponseModel.CommonRender>();
                }

                SmartResponseSessionHelper.CurrentUserAnswer?.Add(SmartResponseSessionHelper.CurrentUserAnswer?.Count ?? 0, currentAns);
            }
            else
            {
                SmartResponseSessionHelper.CurrentUserAnswer = null;
            }

            try
            {
                if (request.IsPageBack || request.IsPageRefesh)
                {
                    #region [SetSelectedAns]

                    LogService.Debug("start SetSelectedAns");
                    if (SmartResponseSessionHelper.CurrentUserAnswer != null && SmartResponseSessionHelper.CurrentUserAnswer.Count > 0)
                    {
                        var r = SmartResponseSessionHelper.CurrentUserAnswer.Where(x => x.Value != null && x.Value.Answer != null && x.Value.Question != null).Select(x => new
                        {
                            QuesId = x.Value.Question.TrackingId,
                            AnsId = x.Value.Answer.TrackingId,
                            SrNo = x.Key
                        }).OrderByDescending(x => x.SrNo);

                        foreach (var item in currentAns.Answer.Questions.Where(x => x != null & x.Answers != null & x.Answers.Count > 0))
                        {
                            int LastSelectedAns = r.OrderByDescending(x => x.SrNo).FirstOrDefault(x => x.QuesId == item.TrackingId)?.AnsId ?? 0;

                            foreach (var ansItem in item.Answers.Where(o => o.TrackingId == LastSelectedAns))
                            {
                                ansItem.IsSelected = true;
                            }

                            foreach (var ansItem in item.Answers.Where(o => o.TrackingId != LastSelectedAns))
                            {
                                ansItem.IsSelected = false;
                            }
                        }
                    }

                    LogService.Debug("end SetSelectedAns");

                    #endregion [SetSelectedAns]
                }

                bool isCaseCompleteLast = currentAns.Answer != null && currentAns.Answer.Questions != null &&
                                            Convert.ToBoolean(currentAns.Answer.Questions.ToList().
                                            Where(x => x.Id == SM_Id.Success && (x.Answers == null ||
                                            (x.Answers != null && x.Answers.Count == 0))).Count() > 0);
                byte[] image1Bytes = new byte[2];
                byte[] image2Bytes = new byte[2];
                var filePath = Server.MapPath(SmartResponseCofig.SMART_RESPONSE_UPLOADPATH);
                string image1Path = "";
                string image2Path = "";

                if (isCaseCompleteLast)
                {
                    #region [FileDataHandling]

                    if (System.IO.Directory.Exists(filePath))
                    {
                        if (!string.IsNullOrWhiteSpace(smDetail.Image1))
                        {
                            image1Path = Path.Combine(filePath, smDetail.Image1);

                            if (System.IO.File.Exists(image1Path))
                            {
                                image1Bytes = System.IO.File.ReadAllBytes(image1Path);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(smDetail.Image2))
                        {
                            image2Path = Path.Combine(filePath, smDetail.Image2);
                            if (System.IO.File.Exists(image2Path))
                            {
                                image2Bytes = System.IO.File.ReadAllBytes(image2Path);
                            }
                        }
                    }

                    #endregion [FileDataHandling]

                    string currentSelectedQUS = currentAns.Question.Value;
                    if (string.IsNullOrEmpty(currentSelectedQUS))
                    {
                        currentSelectedQUS = currentAns.Question.Infotext;
                    }

                    string currentSelectedAns = currentAns.Answer.Btntitle;

                    var _setLgCmpResponse = DewaApiClient.SetCRMInteraction(new DEWAXP.Foundation.Integration.DewaSvc.SetCRMInteraction()
                    {
                        logincomplaints = new DEWAXP.Foundation.Integration.DewaSvc.loginComplaintsInput()
                        {
                            //appidentifier = "",
                            //appversion = "",
                            city = smDetail.Location,
                            code = smDetail.Code,
                            codegroup = smDetail.CodeGroup,
                            contractaccountnumber = smDetail.ContractAccountNo,
                            customertype = smDetail.CustomerType,
                            customercategory = smDetail.CustomerCategory,
                            docstream = image1Bytes,
                            filename = smDetail.Image1,
                            docstream2 = image2Bytes,
                            filename2 = smDetail.Image2,
                            housenumber = smDetail.CA_Location,
                            //lang = "",
                            makaninumber = "",
                            mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile),
                            //mobileosversion = "",
                            name = smDetail.ContactPersonName,
                            sessionid = Convert.ToString(activeUser?.SessionToken),
                            street = smDetail.Street,
                            text = $"{currentSelectedQUS}-{currentSelectedAns}",
                            trcode = smDetail.TrCode,
                            trcodegroup = smDetail.TrCodeGroup,
                            userid = Convert.ToString(activeUser?.UserId),
                            //vendorid = "",
                            xgps = smDetail.Latitude,
                            ygps = smDetail.Longitude,
                            businessPartner = smDetail.PartnerNo
                        }
                    }, RequestLanguage, Request.Segment());
                    if (_setLgCmpResponse != null && _setLgCmpResponse.Succeeded && _setLgCmpResponse.Payload != null)
                    {
                        DeleteFile(image1Path);
                        DeleteFile(image2Path);
                        string intractionId = _setLgCmpResponse.Payload.@return.notificationnumber;
                    }
                }

                ///if page back clear the previous page value session

                #region [if page back clear the previous page value session ]

                if (request.IsPageBack && !string.IsNullOrWhiteSpace(request.SubmittedIds))
                {
                    foreach (var item in request.SubmittedIds.Split(',').ToList().Distinct())
                    {
                        SM_Id idType = (SM_Id)Enum.Parse(typeof(SM_Id), item);
                        if (idType == SM_Id.Image1 || idType == SM_Id.Image2)
                        {
                            string imageFileName = _SM_CommonHelper.GetSelectedAnswerValue(idType);
                            if (!string.IsNullOrWhiteSpace(imageFileName))
                            {
                                string imageFilePath = Path.Combine(filePath, imageFileName);
                                DeleteFile(imageFilePath);
                            }
                        }
                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(idType, null);
                    }
                }

                #endregion [if page back clear the previous page value session ]
            }
            catch (Exception ex)
            {
                LogService.Debug("SetSelectedAns");
                LogService.Error(ex, this);
            }

            if (!string.IsNullOrEmpty(currentAns.RedirectUrl) && currentAns.RedirectCount <= 1)
            {
                return Json(currentAns, JsonRequestBehavior.DenyGet);
            }

            currentAns.CurrentRequest.DdlList = GetCities();
            return PartialView($"/Views/Feature/SupplyManagement/Complaints/_SmartDubaiGenericScreen.cshtml", currentAns);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SmartDubaiNotificationList(Models.SmartResponseModel.TrackingRequest request)
        {
            Models.SmartResponseModel.TrackingResponse model = new TrackingResponse();
            model.TrackingRequest = request;
            try
            {
                if (request.Id == SM_Id.NotificationNumber)
                {
                    var r = DewaApiClient.GetGuestTrackComplaints(new DEWAXP.Foundation.Integration.DewaSvc.GetGuestTrackComplaints()
                    {
                        guesttrackinput = new DEWAXP.Foundation.Integration.DewaSvc.guestTrackInput()
                        {
                            notificationnumber = request.SearchText,
                        }
                    }, RequestLanguage, Request.Segment());

                    if (r != null && r.Succeeded && r.Payload?.@return?.notificationlist != null)
                    {
                        model.TrackNotificationDetails = r.Payload?.@return?.notificationlist.ToList();
                    }
                }
                else if (request.Id == SM_Id.SearchTxt)
                {
                    var activeUser = AuthStateService.GetActiveProfile();
                    var r = DewaApiClient.GetLoginTrackComplaints(new DEWAXP.Foundation.Integration.DewaSvc.GetLoginTrackComplaints()
                    {
                        logintrackinput = new DEWAXP.Foundation.Integration.DewaSvc.loginTrackInput()
                        {
                            userid = activeUser.UserId,
                            sessionid = activeUser.SessionToken,
                        }
                    }, RequestLanguage, Request.Segment());

                    if (r != null && r.Succeeded && r.Payload?.@return?.notificationlist != null)
                    {
                        model.TrackNotificationDetails = r.Payload?.@return?.notificationlist.ToList();

                        if (!string.IsNullOrEmpty(request.SearchText))
                        {
                            model.TrackNotificationDetails = model.TrackNotificationDetails.Where(x => x != null && x.notificationnumber.Contains(request.SearchText)).ToList();
                        }
                    }
                }

                if (model.TrackNotificationDetails != null)
                {
                    model.TrackNotificationDetails = model.TrackNotificationDetails.OrderByDescending(x => _commonUtility.DateTimeFormatParse(x.notificationdate + " " + x.notificationtime, _commonUtility.DF_yyyy_MM_dd_HHmmss).ToString(_commonUtility.DF_yyyyMMddHHmmss)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView($"/Views/Feature/SupplyManagement/Complaints/_SmartDubaiNotificationList.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ValidateCurrentSmartMeter()
        {
            CustomerSubmittedData smDetail = _SM_CommonHelper.GetUserSubmittedData();
            var activeUser = AuthStateService.GetActiveProfile();

            #region [ELECTRICITY]

            if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_ELECTRICITY))
            {
                if (Convert.ToBoolean(smDetail.ElectricityIsSmartMeter) &&
                !string.IsNullOrEmpty(smDetail.ElectricityMeterNo) &&
                ValidateSmartMeter(smDetail.ElectricityMeterNo, "", activeUser.SessionToken, activeUser.UserId))
                {
                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.CheckSmartMeter, "000");
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }

            #endregion [ELECTRICITY]

            #region [WATER]

            if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_WATER))
            {
                if (Convert.ToBoolean(smDetail.WaterIsSmartMeter) &&
                !string.IsNullOrEmpty(smDetail.WaterMeterNo) &&
                ValidateSmartMeter(smDetail.WaterMeterNo, "", activeUser.SessionToken, activeUser.UserId))
                {
                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.CheckSmartMeter, "000");
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }

            #endregion [WATER]

            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.CheckSmartMeter, "1");
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CalculateLoaderPercentage(string voltagerequest)
        {
            CustomerSubmittedData smDetail = _SM_CommonHelper.GetUserSubmittedData();
            var activeUser = AuthStateService.GetActiveProfile();
            var smartmetercount = SmartCustomerClient.GetSmartMeterDetails(new SmartMeterRequest
            {
                smartmeterinputs = new Smartmeterinputs
                {
                    contractaccount = smDetail.ContractAccountNo,
                    sessionid = activeUser.SessionToken,
                    process = "02",
                    type = "02",
                    standardrequest = string.Empty,
                    voltagerequest = voltagerequest
                }
            }, RequestLanguage, Request.Segment());
            if (smartmetercount != null && smartmetercount.Succeeded && smartmetercount.Payload != null && smartmetercount.Payload.Voltagerequest != null)
            {
                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.SmartMeterLoader, smartmetercount.Payload.Voltagerequest.Status);
                return Json(new { success = true, status = smartmetercount.Payload.Voltagerequest.Status }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UploadSMComplaintFile(SM_FileUplploadRequest upRequest)
        {
            try
            {
                if (upRequest.Uploadfile != null && upRequest.Uploadfile.ContentLength > 0)
                {
                    string fileExtension = ".jpg";//Path.GetExtension(upRequest.Uploadfile.FileName);

                    //if (fileExtension.ToLower() == ".jpeg")
                    //{
                    //    fileExtension = ".jpg";
                    //}
                    var fileName = $"{upRequest.ImageType.ToString() + Guid.NewGuid()}{fileExtension}";
                    // store the file inside ~/App_Data/uploads folder
                    var filePath = Server.MapPath(SmartResponseCofig.SMART_RESPONSE_UPLOADPATH);
                    if (!System.IO.Directory.Exists(filePath))
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                    //var frompath = Path.Combine(filePath, "Temp" + Guid.NewGuid() + Path.GetExtension(uploadfile.FileName));
                    var topath = Path.Combine(filePath, fileName);

                    System.Drawing.Image sourceimage = System.Drawing.Image.FromStream(upRequest.Uploadfile.InputStream);
                    System.Drawing.Image fixo = _SM_CommonHelper.FixImageOrientation(sourceimage);
                    fixo.Save(topath, ImageFormat.Jpeg);

                    try
                    {
                        string image1Path = "";
                        string image2Path = "";
                        CustomerSubmittedData smDetail = _SM_CommonHelper.GetUserSubmittedData();
                        if (System.IO.Directory.Exists(filePath))
                        {
                            if (!string.IsNullOrWhiteSpace(smDetail.Image1) && upRequest.ImageType == SM_Id.Image1)
                            {
                                image1Path = Path.Combine(filePath, smDetail.Image1);
                                DeleteFile(image1Path);
                            }

                            if (!string.IsNullOrWhiteSpace(smDetail.Image2) && upRequest.ImageType == SM_Id.Image2)
                            {
                                image2Path = Path.Combine(filePath, smDetail.Image2);
                                DeleteFile(image2Path);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex, this);
                    }

                    return Json(new { success = true, fName = fileName }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SupportCompaines()
        {
            DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse cmpyDetails = null;
            if (!CacheProvider.TryGet<DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse>($"GetSRCompanyDetailsResponse_{RequestLanguage}", out cmpyDetails))
            {
                var d = DewaApiClient.GetSRCompanyDetails(new DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetails() { CompaniesInput = new DEWAXP.Foundation.Integration.DewaSvc.companiesInput() }, RequestLanguage, Request.Segment());
                if (d != null && d.Payload != null)
                {
                    cmpyDetails = d.Payload;
                    CacheProvider.Store($"GetSRCompanyDetailsResponse_{RequestLanguage}", new CacheItem<DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse>(d.Payload, TimeSpan.FromHours(1)));
                }
                else
                {
                    ModelState.AddModelError("", d.Message);
                }
            }

            var currentItem = ContextRepository.GetCurrentItem<Item>();
            ViewBag.IsRTL = currentItem != null && currentItem.Language.CultureInfo.TextInfo.IsRightToLeft;

            return PartialView("~/Views/Feature/SupplyManagement/Complaints/_SupportCompaines.cshtml", cmpyDetails);
        }

        public ActionResult Predict()
        {
            return PartialView($"/Views/Feature/SupplyManagement/Complaints/_Predict.cshtml");
        }

        //As per developer advice, commenting out the below method and this method is not in use
        //public ActionResult GetCurrentPredict()
        //{
        //    PredictState model = new PredictState();
        //    CacheProvider.TryGet(CacheKeys.SM_AI_IMG_PREDICT, out model);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        #region Functions

        public CommonRender SR_ActionHelper(CommonRender data)
        {
            var IsStopAction = Convert.ToBoolean(data != null && Convert.ToBoolean(data.CurrentRequest?.IsPageBack) || (data.CurrentRequest.IsPageRefesh));
            CustomerSubmittedData smDetail = _SM_CommonHelper.GetUserSubmittedData();
            bool IsSessionExit = IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User);
            var activeUser = AuthStateService.GetActiveProfile();

            string _otpCount = _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.OtpCount);
            int _Customer_OtpCount = Convert.ToInt32(!string.IsNullOrEmpty(_otpCount) ? _otpCount : "0");
            string _errorMessage = "";

            #region file variable

            byte[] image1Bytes = new byte[2];
            byte[] image2Bytes = new byte[2];
            string image1Path = "";
            string image2Path = "";
            var filePath = Server.MapPath(SmartResponseCofig.SMART_RESPONSE_UPLOADPATH);

            #endregion file variable

            Models.SmartResponseModel.Question filteredQus = null;
            if (data != null && data.Answer != null)
            {
                switch (data.Answer.Action)
                {
                    case Models.SmartResponseModel.SM_Action.Call://extra
                        break;

                    case Models.SmartResponseModel.SM_Action.Checklogin:
                        //handle screen by session

                        #region [handle screen by session]

                        if (data.Answer.Questions.Where(x => !string.IsNullOrEmpty(x.Code.ToString())).Count() > 1)
                        {
                            filteredQus = null;
                            Models.SmartResponseModel.SM_Code code = IsSessionExit ? SM_Code.The000 : SM_Code.The001;
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == code).FirstOrDefault());
                            if (filteredQus != null)
                            {
                                data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                                data.Answer.Questions.Add(filteredQus);
                            }
                        }

                        if (IsLoggedIn && IsSessionExit)
                        {
                            if (string.IsNullOrEmpty(smDetail.ContactPersonName))
                            {
                                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ContactPersonName, GetContactNameByUserSession());
                            }

                            if (string.IsNullOrEmpty(smDetail.Mobile))
                            {
                                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Mobile, activeUser.MobileNumber);
                            }
                        }

                        #endregion [handle screen by session]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case Models.SmartResponseModel.SM_Action.Empty:
                        break;

                    case Models.SmartResponseModel.SM_Action.Verifyotp:

                        if (IsStopAction || SmartResponseCofig.SM_DISBABLE_LOGIC)
                        {
                            break;
                        }

                        #region [VerifyOTP]

                        //TODO:SM-- need to implement Verifyotp. -- DONE
                        if (!string.IsNullOrEmpty(smDetail.Mobile) && !string.IsNullOrEmpty(smDetail.MobileOtp))
                        {
                            var r1 = DewaApiClient.VerifyNotificationOTP(new DEWAXP.Foundation.Integration.DewaSvc.VerifyNotificationOTP()
                            {
                                otpinput = new DEWAXP.Foundation.Integration.DewaSvc.notificationOTPInput()
                                {
                                    mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile),
                                    otp = smDetail.MobileOtp,
                                    servicetype = "Y7",
                                }
                            }, RequestLanguage, Request.Segment());
                            data.IsError = !(r1 != null && r1.Succeeded);

                            if (data.IsError)
                            {
                                data.ErrorDetails.Add(new SM_ErrorDetail() { ControlId = SM_Id.Otp });
                            }
                        }

                        #endregion [VerifyOTP]

                        break;

                    case Models.SmartResponseModel.SM_Action.Getotpmobile:
                    case Models.SmartResponseModel.SM_Action.Resendotpmobile:

                        try
                        {
                            if (IsStopAction || SmartResponseCofig.SM_DISBABLE_LOGIC)
                            {
                                break;
                            }

                            #region [Getotpmobile,Resendotpmobile]

                            //TODO:SM-- need to implement Getotpmobile. -- DONE
                            //TODO:SM-- need to implement Resendotpmobile. -- DONE

                            #region [Disabled OTP count logic]

                            //bool OtpSuccess = false;
                            //if (!string.IsNullOrEmpty(smDetail.Mobile) && _Customer_OtpCount <= 3)
                            //{
                            //    var r = DewaApiClient.SendNotificationOTP(new DEWAXP.Foundation.Integration.DewaSvc.SendNotificationOTP()
                            //    {
                            //        otpinput = new DEWAXP.Foundation.Integration.DewaSvc.notificationOTPInput()
                            //        {
                            //            mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile), // masking
                            //            servicetype = "Y7",
                            //        }
                            //    }, RequestLanguage, Request.Segment());
                            //    OtpSuccess = (r != null && r.Succeeded);
                            //    _errorMessage = r.Message;
                            //    LogService.Debug(_errorMessage);
                            //    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.OtpCount, Convert.ToString(_Customer_OtpCount + 1));
                            //}
                            //data.IsError = !OtpSuccess;

                            //if (data.IsError)
                            //{
                            //    if (_Customer_OtpCount > 3)
                            //    {
                            //        data.ErrorDetails.Add(new SM_ErrorDetail()
                            //        {
                            //            ControlId = SM_Id.OtpCount,
                            //            ErorrMessage = _errorMessage
                            //        });
                            //    }
                            //    else
                            //    {
                            //        data.ErrorDetails.Add(new SM_ErrorDetail()
                            //        {
                            //            ControlId = SM_Id.Mobile,
                            //            ErorrMessage = _errorMessage
                            //        });
                            //    }
                            //}

                            #endregion [Disabled OTP count logic]

                            var r = DewaApiClient.SendNotificationOTP(new DEWAXP.Foundation.Integration.DewaSvc.SendNotificationOTP()
                            {
                                otpinput = new DEWAXP.Foundation.Integration.DewaSvc.notificationOTPInput()
                                {
                                    mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile), // masking
                                    servicetype = "Y7",
                                }
                            }, RequestLanguage, Request.Segment());
                            data.IsError = !(r != null && r.Succeeded);
                            LogService.Debug(r.Message);
                            if (data.IsError)
                            {
                                data.ErrorDetails.Add(new SM_ErrorDetail()
                                {
                                    ControlId = SM_Id.Mobile,
                                    ErorrMessage = r.Message
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.Error(ex, this);
                        }

                        break;

                    #endregion [Getotpmobile,Resendotpmobile]

                    case Models.SmartResponseModel.SM_Action.Submit:
                        if (IsStopAction)
                        {
                            break;
                        }
                        //TODO:SM-- need to complete Submission Flow. --DONE

                        #region [Submit]

                        SM_SessionType _type = _SM_CommonHelper.Get_SM_SessionType();

                        #region [FileDataHandling]

                        if (System.IO.Directory.Exists(filePath))
                        {
                            if (!string.IsNullOrWhiteSpace(smDetail.Image1))
                            {
                                image1Path = Path.Combine(filePath, smDetail.Image1);
                                if (System.IO.File.Exists(image1Path))
                                {
                                    image1Bytes = System.IO.File.ReadAllBytes(image1Path);
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(smDetail.Image2))
                            {
                                image2Path = Path.Combine(filePath, smDetail.Image2);
                                if (System.IO.File.Exists(image2Path))
                                {
                                    image2Bytes = System.IO.File.ReadAllBytes(image2Path);
                                }
                            }
                        }

                        #endregion [FileDataHandling]

                        string complaintId = null;

                        #region [Submission Handling]

                        string notif = string.Empty;
                        if (_type == SM_SessionType.IsLoggedIn)
                        {
                            var _setLgCmpResponse = DewaApiClient.SetLoginComplaints(new DEWAXP.Foundation.Integration.DewaSvc.SetLoginComplaints()
                            {
                                logincomplaints = new DEWAXP.Foundation.Integration.DewaSvc.loginComplaintsInput()
                                {
                                    //appidentifier = "",
                                    //appversion = "",
                                    city = smDetail.Location,
                                    code = smDetail.Code,
                                    codegroup = smDetail.CodeGroup,
                                    contractaccountnumber = smDetail.ContractAccountNo,
                                    customertype = smDetail.CustomerType,
                                    customercategory = smDetail.CustomerCategory,
                                    docstream = image1Bytes,
                                    filename = smDetail.Image1,
                                    docstream2 = image2Bytes,
                                    filename2 = smDetail.Image2,
                                    housenumber = smDetail.CA_Location,
                                    //lang = "",
                                    makaninumber = "",
                                    mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile),
                                    //mobileosversion = "",
                                    name = smDetail.ContactPersonName,
                                    sessionid = Convert.ToString(activeUser?.SessionToken),
                                    street = smDetail.Street,
                                    text = smDetail.MoreDescription,
                                    trcode = smDetail.TrCode,
                                    trcodegroup = smDetail.TrCodeGroup,
                                    userid = Convert.ToString(activeUser?.UserId),
                                    //vendorid = "",
                                    xgps = smDetail.Latitude,
                                    ygps = smDetail.Longitude,
                                }
                            }, RequestLanguage, Request.Segment());
                            if (_setLgCmpResponse != null)
                            {
                                if (_setLgCmpResponse.Succeeded && _setLgCmpResponse.Payload != null)
                                {
                                    complaintId = _setLgCmpResponse.Payload.@return.notificationnumber;
                                }

                                if (_setLgCmpResponse.Payload.@return.responsecode == "791")
                                {
                                    notif = _setLgCmpResponse.Payload.@return.description;
                                }
                            }
                        }
                        else if (_type == SM_SessionType.IsGuest)
                        {
                            var _setGsCmpResponse = DewaApiClient.SetGuestComplaints(new DEWAXP.Foundation.Integration.DewaSvc.SetGuestComplaints()
                            {
                                guestcomplaints = new DEWAXP.Foundation.Integration.DewaSvc.guestComplaintsInput()
                                {
                                    city = smDetail.Location,
                                    code = smDetail.Code,
                                    codegroup = smDetail.CodeGroup,
                                    customertype = smDetail.CustomerType,
                                    customercategory = smDetail.CustomerCategory,
                                    contractaccountnumber = smDetail.ContractAccountNo,
                                    docstream = image1Bytes,
                                    filename = smDetail.Image1,
                                    docstream2 = image2Bytes,
                                    filename2 = smDetail.Image2,
                                    //lang = "",
                                    makaninumber = "",
                                    mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile),
                                    name = smDetail.ContactPersonName,
                                    sessionid = activeUser.SessionToken,
                                    text = smDetail.MoreDescription,
                                    trcode = smDetail.TrCode,
                                    trcodegroup = smDetail.TrCodeGroup,
                                    xgps = smDetail.Latitude,
                                    ygps = smDetail.Longitude,
                                },
                            }, RequestLanguage, Request.Segment());

                            if (_setGsCmpResponse != null)
                            {
                                if (_setGsCmpResponse.Succeeded && _setGsCmpResponse.Payload != null)
                                {
                                    complaintId = _setGsCmpResponse.Payload.@return.notificationnumber;
                                }
                                if (_setGsCmpResponse.Payload.@return.responsecode == "791")
                                {
                                    notif = _setGsCmpResponse.Payload.@return.description;
                                }
                            }
                        }

                        #endregion [Submission Handling]

                        data.IsError = string.IsNullOrEmpty(complaintId);
                        if (!data.IsError)
                        {
                            DeleteFile(image1Path);
                            DeleteFile(image2Path);
                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ComplaintId, complaintId);
                        }
                        if (data.IsError)
                        {
                            var errorData = new SM_ErrorDetail()
                            {
                                ControlId = SM_Id.ComplaintId,
                            };

                            if (!string.IsNullOrWhiteSpace(notif))
                            {
                                errorData.ErorrMessage = _SM_CommonHelper.GetSMTranslation(Translate.Text("SM_ ResolutionStageText")).Replace("{​{​notif}​}", notif);
                            }
                            data.ErrorDetails.Add(errorData);
                        }

                        #endregion [Submit]

                        break;

                    case Models.SmartResponseModel.SM_Action.Track:
                        break;

                    case Models.SmartResponseModel.SM_Action.UploadMedia:
                        break;

                    case Models.SmartResponseModel.SM_Action.SubmitRequestFire://extra
                        break;

                    case Models.SmartResponseModel.SM_Action.BillPayment://extra
                        break;

                    case Models.SmartResponseModel.SM_Action.SubmitRequestSpark://extra
                        break;

                    case Models.SmartResponseModel.SM_Action.SubmitRequestSmoke://extra
                        break;

                    case Models.SmartResponseModel.SM_Action.SubmitRequestEfluctuation://extra
                        break;

                    case Models.SmartResponseModel.SM_Action.CheckMeterStatus:
                        if (data.Answer.Questions.Where(x => !string.IsNullOrEmpty(x.Code.ToString())).Count() > 1)
                        {
                            filteredQus = null;
                            SM_Code code = smDetail.SmartMeterLoader.Equals("G") ? SM_Code.The000 : SM_Code.The001;
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == code).FirstOrDefault());
                            if (filteredQus != null)
                            {
                                data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                                data.Answer.Questions.Add(filteredQus);
                            }
                        }
                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case Models.SmartResponseModel.SM_Action.SubMeterCheck:
                        //TODO:SM-- need to implement Sub Meter Check --DONE
                        bool IsSubMeter = (smDetail.ElectricityMeterType == CommonConst.IsSubMeter || smDetail.ElectricityMeterType == CommonConst.IsStandAloneMeter);

                        if (data.Answer.Questions.Where(x => !string.IsNullOrEmpty(x.Code.ToString())).Count() > 1)
                        {
                            filteredQus = null;
                            Models.SmartResponseModel.SM_Code code = IsSessionExit && IsSubMeter ? SM_Code.The000 : SM_Code.The001;
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == code).FirstOrDefault());
                            if (filteredQus != null)
                            {
                                data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                                data.Answer.Questions.Add(filteredQus);
                            }
                        }

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case Models.SmartResponseModel.SM_Action.Paybill:
                        break;

                    case Models.SmartResponseModel.SM_Action.Customerlogin:
                        //setting redirect url: --DONE
                        var loginUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                        data.RedirectUrl = string.Format(loginUrl + "?returnUrl={0}", HttpUtility.UrlEncode(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CommonComplaint) + "?l=" + smDetail.LangType.ToString()));

                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.SessionType, "1");//seting its customer Type login.
                        break;

                    case Models.SmartResponseModel.SM_Action.Accountcountcheck:
                        //TODO:SM-- Need to Add Account Count Service COntroller : DONE
                        int AccountCount = 0;

                        var accountList = GetBillingAccounts(false, true, "");
                        AccountCount = Convert.ToInt32(accountList.Succeeded && accountList?.Payload != null ? accountList?.Payload.Count() : 0);

                        #region [handle screen by Account Count]

                        //if (data.Answer.Questions.Where(x => !string.IsNullOrEmpty(x.Code.ToString())).Count() > 1)
                        //{
                        filteredQus = null;
                        if (AccountCount == 1)
                        {
                            // next step
                            filteredQus = QuestionValueBinder(data.Answer.Questions.FirstOrDefault().Answers.FirstOrDefault().Questions.FirstOrDefault());
                            var dAccount = accountList.Payload.FirstOrDefault();
                            string accNo = dAccount.AccountNumber;
                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Account, activeUser.HasPrimaryAccount ? activeUser.PrimaryAccount : accNo);

                            string personName = GetContactNameByAccountDetails(dAccount);
                            if (dAccount != null && string.IsNullOrWhiteSpace(personName))
                            {
                                personName = GetContactNameByUserSession();
                            }
                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ContactPersonName, personName);
                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Mobile, activeUser.MobileNumber);
                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.CA_Location, dAccount.Location);
                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Street, dAccount.Street);
                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Location, dAccount.Street);

                            #region [Validate the data for Notitication Creation]
                            if (!data.IsError)
                            {
                                smDetail = _SM_CommonHelper.GetUserSubmittedData();

                                var _smdSubmitDetail = new CustomerSubmittedData()
                                {
                                    CustomerCategory = smDetail.CustomerCategory,
                                    CodeGroup = smDetail.CodeGroup,
                                    ContractAccountNo = smDetail.ContractAccountNo,
                                    CustomerType = smDetail.CustomerType,
                                    Mobile = smDetail.Mobile
                                };
                                var validateSubmitedComplaint = ValidateAndSubmitLoginComplaints(_smdSubmitDetail, activeUser, image1Bytes, image2Bytes);

                                data.IsError = validateSubmitedComplaint != null
                                        && validateSubmitedComplaint.code == "791"
                                        && !string.IsNullOrWhiteSpace(validateSubmitedComplaint.notif);

                                if (data.IsError)
                                {
                                    data.ErrorDetails.Add(new SM_ErrorDetail()
                                    {
                                        ErorrMessage = _SM_CommonHelper.GetSMTranslation(Translate.Text("SM_ ResolutionStageText")).Replace("{​{​notif}​}", validateSubmitedComplaint.notif),
                                        ControlId = SM_Id.Error
                                    });
                                }

                                if (!data.IsError)
                                {
                                    data.IsError = validateSubmitedComplaint != null && validateSubmitedComplaint.code == "error";
                                    if (data.IsError)
                                    {
                                        data.ErrorDetails.Add(new SM_ErrorDetail()
                                        {
                                            ErorrMessage = _SM_CommonHelper.GetSMTranslation("SM_Error_UnableToProcess"),
                                            ControlId = SM_Id.Error
                                        });
                                    }
                                }
                            }
                            #endregion

                        }

                        if (AccountCount > 1)
                        {
                            filteredQus = QuestionValueBinder(data.Answer.Questions.FirstOrDefault());
                            data.RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CommonComplaint)}?asc=1";
                        }

                        if (AccountCount < 1)
                        {
                            //data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                            data.Answer.Questions.Add(new Question()
                            {
                                Value = _SM_CommonHelper.GetSMTranslation("No Account Exist"),
                                Id = SM_Id.Error
                            });
                        }

                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }

                        //}

                        #endregion [handle screen by Account Count]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case Models.SmartResponseModel.SM_Action.Checkpod:

                        #region [Checkpod]

                        Models.SmartResponseModel.SM_Code checkpodCode = SM_Code.The000;
                        if (data.Answer.Questions.Where(x => x.Code != SM_Code.Empty).Count() > 1 && !string.IsNullOrEmpty(smDetail.ContractAccountNo))
                        {
                            bool IsPod = false;
                            filteredQus = null;

                            //TODO:SM-- add POD checking Service. :  DONE
                            //calling service to check pod.
                            var POD_repsonse = PremiseHandler.GetDetails(new PremiseDetailsRequest()
                            {
                                PremiseDetailsIN = new PremiseDetailsIN()
                                {
                                    contractaccount = smDetail.ContractAccountNo,
                                    dminfo = false,
                                    meterstatusinfo = false,
                                    outageinfo = false,
                                    podcustomer = true,
                                    seniorcitizen = true,
                                    userid = activeUser.Username,
                                    sessionid = activeUser.SessionToken,
                                },
                            }, RequestLanguage, Request.Segment());

                            if (POD_repsonse.Succeeded && POD_repsonse.Payload != null)
                            {
                                IsPod = Convert.ToBoolean(POD_repsonse.Payload.responseCode == "000" &&
                                                 (POD_repsonse.Payload.podCustomer || POD_repsonse.Payload.seniorCitizen));

                                if (POD_repsonse.Payload.podCustomer)
                                {
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.customercategory, CommonConst.CusSubTyp_POD);
                                }
                                else
                                  if (POD_repsonse.Payload.seniorCitizen)
                                {
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.customercategory, CommonConst.CusSubTyp_ElderPeople);
                                }
                            }

                            checkpodCode = IsPod ? SM_Code.Pod : SM_Code.The000;
                        }
                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == checkpodCode).FirstOrDefault());
                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }

                        #endregion [Checkpod]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case Models.SmartResponseModel.SM_Action.NoPower:
                    case Models.SmartResponseModel.SM_Action.NopowerPod:
                    case Models.SmartResponseModel.SM_Action.NoWater:
                    case Models.SmartResponseModel.SM_Action.NoWaterPOD:

                        #region [NoPower,NopowerPod]

                        SM_Code _currentCOde = SM_Code.The005;
                        SM_Code _defaultCOde = SM_Code.The005;

                        if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_WATER))
                        {
                            _currentCOde = SM_Code.The004;
                            _defaultCOde = SM_Code.The004;
                        }

                        string easyPayUrl = "";
                        string _dueAmount = "0.00";
                        string _startDateText = "";
                        string _endDateText = "";
                        bool isDisconnection = false;
                        string DisconnectionReason = "";
                        if (data.Answer.Questions.Where(x => x.Code != SM_Code.Empty).Count() > 1 && !string.IsNullOrEmpty(smDetail.ContractAccountNo))
                        {
                            //only POD
                            bool IsPod = false;

                            var PODIsExist = (data.Answer.Questions.Where(x => x.Code == SM_Code.Pod).Count() > 0);
                            if (PODIsExist)
                            {
                                //calling service to check pod.
                                var POD_repsonse = PremiseHandler.GetDetails(new PremiseDetailsRequest()
                                {
                                    PremiseDetailsIN = new PremiseDetailsIN()
                                    {
                                        contractaccount = smDetail.ContractAccountNo,
                                        dminfo = false,
                                        meterstatusinfo = false,
                                        outageinfo = false,
                                        podcustomer = true,
                                        seniorcitizen = true,
                                        userid = activeUser.Username,
                                        sessionid = activeUser.SessionToken,
                                    },
                                }, RequestLanguage, Request.Segment());

                                if (POD_repsonse.Succeeded && POD_repsonse.Payload != null)
                                {
                                    IsPod = Convert.ToBoolean(POD_repsonse.Payload.responseCode == "000" &&
                                                     (POD_repsonse.Payload.podCustomer || POD_repsonse.Payload.seniorCitizen));

                                    if (POD_repsonse.Payload.podCustomer)
                                    {
                                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.customercategory, CommonConst.CusSubTyp_POD);
                                    }
                                    else
                                    if (POD_repsonse.Payload.seniorCitizen)
                                    {
                                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.customercategory, CommonConst.CusSubTyp_ElderPeople);
                                    }
                                }
                            }

                            if (IsPod)
                            {
                                _currentCOde = SM_Code.Pod;
                                //filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == SM_Code.Pod).FirstOrDefault());
                            }
                            else
                            {
                                ////AccountNo = "002001212003";// "2001099576";
                                var _issueRepsonse = PremiseHandler.GetDetails(new PremiseDetailsRequest()
                                {
                                    PremiseDetailsIN = new PremiseDetailsIN()
                                    {
                                        contractaccount = smDetail.ContractAccountNo,
                                        dminfo = true,
                                        meterstatusinfo = true,
                                        outageinfo = true,
                                        podcustomer = false,
                                        seniorcitizen = false,
                                        userid = activeUser.Username,
                                        sessionid = activeUser.SessionToken,
                                    },
                                }, RequestLanguage, Request.Segment());

                                if (_issueRepsonse.Succeeded && _issueRepsonse.Payload != null)
                                {
                                    var _responseData = _issueRepsonse.Payload;

                                    #region [Smarter & logic]

                                    //electricity
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ElectricityMeterType, _responseData.meter.electricitymeterType);
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ElectricityIsSmartMeter, Convert.ToString(_responseData.meter.electricitySmartMeter));
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ElectricityMeterNo, Convert.ToString(_responseData.meter.electricityMeter));

                                    //water
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.WaterMeterType, _responseData.meter.watermeterType);
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.WaterIsSmartMeter, Convert.ToString(_responseData.meter.waterSmartMeter));
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.WaterMeterNo, Convert.ToString(_responseData.meter.waterMeter));

                                    #endregion [Smarter & logic]

                                    #region [Bussinesss Logic Info]

                                    // payement Screen : SM_Code.The000
                                    //if Electricity && electicityActive == false && resone is non payment
                                    //000
                                    if (Convert.ToBoolean(_responseData.meter.disconnectionReasonCode == "NP-DC")) //TODO:SM-- clearity Needed non payment  reason code
                                    {
                                        _currentCOde = SM_Code.The000;
                                        //need to create payment Url & redirect it on the same.
                                        easyPayUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EasyPay_Enquire)}?ac=" + smDetail.ContractAccountNo;
                                        _dueAmount = _responseData.meter.reconnectionDueamount;
                                    }

                                    if (Convert.ToBoolean(_responseData.dubaiMunicipality?.dmfine) || Convert.ToBoolean(_responseData.meter?.disconnectionReasonCode == "DM-DC") && _currentCOde == _defaultCOde)
                                    {
                                        _currentCOde = SM_Code.The002;
                                    }

                                    if (!string.IsNullOrWhiteSpace(_responseData.meter?.disconnectionReasonCode) && _currentCOde == _defaultCOde)
                                    {
                                        //TODO:SM-- Need to show message to the User for Disconnection and user should not
                                        isDisconnection = true;
                                        _currentCOde = SM_Code.Empty;
                                        DisconnectionReason = _responseData.meter?.disconnectionAlert;
                                    }

                                    #region [Electircity Case]

                                    if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_ELECTRICITY))
                                    {
                                        #region [isOutage]

                                        if (_currentCOde == _defaultCOde)
                                        {
                                            var _outageinfo = _responseData.outageStatus;

                                            var plannedElectricityOutage = (_outageinfo.plannedElectricityOutage ?? "").ToUpper();
                                            if (plannedElectricityOutage != CommonConst.NOOUTAGE)
                                            {
                                                if (plannedElectricityOutage == CommonConst.PLANNED)
                                                {
                                                    DateTime _startDate = DateTime.MinValue;
                                                    DateTime _endDate = DateTime.MinValue;
                                                    if (DateTime.TryParseExact($"{_outageinfo.electricityStartDate} {_outageinfo.electricityStartTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _startDate) &&
                                                         DateTime.TryParseExact($"{_outageinfo.electricityEndDate} {_outageinfo.electricityEndTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _endDate)) ;
                                                    {
                                                        //check currrent date is in range outage date
                                                        if (DateHelper.InRange(DateTime.Now, _startDate, _endDate))
                                                        {
                                                            _currentCOde = SM_Code.The001;
                                                            //TODO:SM-- Additional Logic to Trim and reset Question
                                                            _startDateText = _SM_CommonHelper.TextHighlight(_startDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                            _endDateText = _SM_CommonHelper.TextHighlight(_endDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                        }
                                                    }
                                                    //Outage planned Screen : SM_Code.The001
                                                    // check outageType = planned & DateTime Range i.e current date range
                                                }
                                                else
                                                if (plannedElectricityOutage == CommonConst.UNPLANNED)
                                                {
                                                    _currentCOde = SM_Code.The003;
                                                    //Outage unplanned Screen : SM_Code.The003
                                                    //check outageType = unplanned
                                                }
                                            }
                                            else
                                            {
                                                if (_responseData.meter.electricitySmartMeter)
                                                {
                                                    var smartmeterdetailsResponse = SmartCustomerClient.GetSmartMeterDetails(new SmartMeterRequest
                                                    {
                                                        smartmeterinputs = new Smartmeterinputs
                                                        {
                                                            contractaccount = smDetail.ContractAccountNo,
                                                            sessionid = activeUser.SessionToken,
                                                            process = "01",
                                                            type = "02",
                                                            standardrequest = string.Empty,
                                                            voltagerequest = string.Empty
                                                        }
                                                    }, RequestLanguage, Request.Segment());
                                                    if (smartmeterdetailsResponse != null && smartmeterdetailsResponse.Succeeded && smartmeterdetailsResponse.Payload != null && smartmeterdetailsResponse.Payload.Voltagerequest != null && !string.IsNullOrWhiteSpace(smartmeterdetailsResponse.Payload.Voltagerequest.Requestno))
                                                    {
                                                        data.LoaderDetails = new SM_LoaderDetails();
                                                        data.LoaderDetails.Voltagerequest = smartmeterdetailsResponse.Payload.Voltagerequest.Requestno;
                                                        data.LoaderDetails.strretry = smartmeterdetailsResponse.Payload.Retry;
                                                        var smartmetercount = SmartCustomerClient.GetSmartMeterDetails(new SmartMeterRequest
                                                        {
                                                            smartmeterinputs = new Smartmeterinputs
                                                            {
                                                                contractaccount = smDetail.ContractAccountNo,
                                                                sessionid = activeUser.SessionToken,
                                                                process = "02",
                                                                type = "02",
                                                                standardrequest = string.Empty,
                                                                voltagerequest = data.LoaderDetails.Voltagerequest
                                                            }
                                                        }, RequestLanguage, Request.Segment());
                                                        if (smartmetercount != null && smartmetercount.Succeeded && smartmetercount.Payload != null && smartmetercount.Payload.Voltagerequest != null)
                                                        {
                                                            data.LoaderDetails.Status = smartmetercount.Payload.Voltagerequest.Status;
                                                            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.SmartMeterLoader, smartmetercount.Payload.Voltagerequest.Status);
                                                            if (smartmetercount.Payload.Voltagerequest.Status.Equals("G"))
                                                            {
                                                                _currentCOde = SM_Code.The004;
                                                                data.LoaderDetails.RetryCount = data.LoaderDetails.TotalCount;
                                                            }
                                                            else if (smartmetercount.Payload.Voltagerequest.Status.Equals("I"))
                                                            {
                                                                _currentCOde = SM_Code.The004;
                                                                data.LoaderDetails.RetryCount = 1;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        #endregion [isOutage]
                                    }

                                    #endregion [Electircity Case]

                                    #region [Water case]

                                    if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_WATER))
                                    {
                                        if (_currentCOde == _defaultCOde)
                                        {
                                            #region [isOutage]

                                            var _outageinfo = _responseData.outageStatus;

                                            var plannedwaterOutage = (_outageinfo.plannedwaterOutage ?? "").ToUpper();
                                            if (plannedwaterOutage != CommonConst.NOOUTAGE)
                                            {
                                                if (plannedwaterOutage == CommonConst.PLANNED)
                                                {
                                                    DateTime _startDate = DateTime.MinValue;
                                                    DateTime _endDate = DateTime.MinValue;
                                                    if (DateTime.TryParseExact($"{_outageinfo.waterStartDate} {_outageinfo.waterStartTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _startDate) &&
                                                         DateTime.TryParseExact($"{_outageinfo.waterEndDate} {_outageinfo.waterEndTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _endDate))
                                                    {
                                                        //check currrent date is in range outage date
                                                        if (DateHelper.InRange(DateTime.Now, _startDate, _endDate))
                                                        {
                                                            _currentCOde = SM_Code.The001;
                                                            //TODO:SM-- Additional Logic to Trim and reset Question
                                                            _startDateText = _SM_CommonHelper.TextHighlight(_startDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                            _endDateText = _SM_CommonHelper.TextHighlight(_endDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                        }
                                                    }
                                                    //Outage planned Screen : SM_Code.The001
                                                    // check outageType = planned & DateTime Range i.e current date range
                                                }
                                                else
                                                if (plannedwaterOutage == CommonConst.UNPLANNED)
                                                {
                                                    _currentCOde = SM_Code.The003;
                                                    //Outage unplanned Screen : SM_Code.The003
                                                    //check outageType = unplanned
                                                }
                                            }

                                            #endregion [isOutage]

                                            #region isSubmeter

                                            bool IsSubMeterC = smDetail.WaterMeterType == CommonConst.IsSubMeter;

                                            if (IsSubMeterC)
                                            {
                                                _currentCOde = SM_Code.The005;
                                            }

                                            #endregion isSubmeter
                                        }
                                    }

                                    #endregion [Water case]

                                    #endregion [Bussinesss Logic Info]
                                }

                                //TODO: Never Remove.
                                //if (_currentCOde == _defaultCOde)
                                //{
                                //    smDetail = _SM_CommonHelper.GetUserSubmittedData();
                                //    //after checking Type(electricy || water )checking meter status service. : SM_Code.The004
                                //    //TODO:SM-- Need to implement servicec - DONE
                                //    #region [ELECTRICITY]
                                //    if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_ELECTRICITY))
                                //    {
                                //        if (Convert.ToBoolean(smDetail.ElectricityIsSmartMeter) &&
                                //        !string.IsNullOrEmpty(smDetail.ElectricityMeterNo) &&
                                //        ValidateSmartMeter(smDetail.ElectricityMeterNo, "1", activeUser.SessionToken, activeUser.UserId))
                                //        {
                                //            _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.CheckSmartMeter, "1");
                                //            _currentCOde = SM_Code.The004;
                                //        }

                                //    }
                                //    #endregion

                                //}
                            }
                        }
                        filteredQus = data.Answer.Questions.Where(x => x != null && x.Code == _currentCOde).FirstOrDefault();
                        data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                        if (filteredQus != null)
                        {
                            data.Answer.Questions.Add(filteredQus);

                            #region [Data Alteration]

                            if (filteredQus.Answers.FirstOrDefault().Action == SM_Action.Paybill)
                            {
                                data.Answer.Questions.FirstOrDefault().Answers.FirstOrDefault().Actiondata = easyPayUrl;
                                data.Answer.Questions.FirstOrDefault().Value = _SM_CommonHelper.GetSMTranslation(filteredQus.Value)?.Replace("{{amount}}", _SM_CommonHelper.TextHighlight(_dueAmount));
                            }

                            if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_ELECTRICITY))
                            {
                                //planned outage:
                                if (filteredQus.Code == SM_Code.The001)
                                {
                                    data.Answer.Questions.FirstOrDefault().Value = _SM_CommonHelper.GetSMTranslation(filteredQus.Value)?.Replace("{{fromXX}}", _startDateText)?.Replace("{{toXX}}", _endDateText);
                                }
                            }

                            if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_WATER))
                            {
                                //planned outage:
                                if (filteredQus.Code == SM_Code.The001)
                                {
                                    data.Answer.Questions.FirstOrDefault().Value = _SM_CommonHelper.GetSMTranslation(filteredQus.Value)?.Replace("{{fromXX}}", _startDateText)?.Replace("{{toXX}}", _endDateText);
                                }
                            }

                            #endregion [Data Alteration]
                        }
                        else if (isDisconnection)
                        {
                            data.Answer.Questions.Add(new Question()
                            {
                                Value = DisconnectionReason,
                                Id = SM_Id.Error,
                                Infotext = ""
                            });
                        }
                        else
                        {
                            data.Answer.Questions.Add(new Question()
                            {
                                Value = Translate.Text("SM_Error_UnableToProcess"),
                                Id = SM_Id.Error,
                                Infotext = ""
                            });
                        }

                        #endregion [NoPower,NopowerPod]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case Models.SmartResponseModel.SM_Action.SubmetercheckPod:
                        //TODO: need to implement service.
                        //get primise detail with pod+meter= true & no smart meter check

                        #region [SubmetercheckPod]

                        Models.SmartResponseModel.SM_Code _code = SM_Code.The001;
                        if (data.Answer.Questions.Where(x => x.Code != SM_Code.Empty).Count() > 1 && !string.IsNullOrEmpty(smDetail.ContractAccountNo))
                        {
                            bool IsPod = false;
                            filteredQus = null;

                            //TODO:SM-- add POD checking Service. :  DONE
                            //calling service to check pod.
                            var POD_repsonse = PremiseHandler.GetDetails(new PremiseDetailsRequest()
                            {
                                PremiseDetailsIN = new PremiseDetailsIN()
                                {
                                    contractaccount = smDetail.ContractAccountNo,
                                    dminfo = false,
                                    meterstatusinfo = true,
                                    outageinfo = false,
                                    podcustomer = true,
                                    seniorcitizen = true,
                                    userid = activeUser.Username,
                                    sessionid = activeUser.SessionToken,
                                },
                            }, RequestLanguage, Request.Segment());

                            if (POD_repsonse.Succeeded && POD_repsonse.Payload != null && POD_repsonse.Payload.responseCode == "000")
                            {
                                var d = POD_repsonse.Payload;
                                IsPod = Convert.ToBoolean(d.podCustomer || d.seniorCitizen);

                                if (POD_repsonse.Payload.podCustomer)
                                {
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.customercategory, CommonConst.CusSubTyp_POD);
                                }
                                else
                                  if (POD_repsonse.Payload.seniorCitizen)
                                {
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.customercategory, CommonConst.CusSubTyp_ElderPeople);
                                }

                                if (IsPod)
                                {
                                    _code = SM_Code.Pod;
                                }
                                if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_ELECTRICITY))
                                {
                                    if (d.meter.electricitymeterType != CommonConst.IsSubMeter)
                                    {
                                        _code = SM_Code.The000;
                                    }
                                }
                                if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == CommonConst.PARENT_INCIDENT_WATER))
                                {
                                    if (d.meter.watermeterType != CommonConst.IsSubMeter)
                                    {
                                        _code = SM_Code.The000;
                                    }
                                }
                            }
                        }
                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _code).FirstOrDefault());
                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }

                        #endregion [SubmetercheckPod]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case Models.SmartResponseModel.SM_Action.GetLocation:

                        break;

                    case Models.SmartResponseModel.SM_Action.UploadMedia_ssd:

                        filteredQus = null;
                        SM_Code _umDefault = SM_Code.The001;
                        if (System.IO.Directory.Exists(filePath))
                        {
                            if (!string.IsNullOrWhiteSpace(smDetail.Image1))
                            {
                                image1Path = System.IO.Path.Combine(filePath, smDetail.Image1);
                                CacheProvider.Remove(CacheKeys.SM_AI_IMG_PREDICT);
                                PredictState r = new PredictState()
                                {
                                    boxes = new List<List<double>>(),
                                };
                                if (System.IO.File.Exists(image1Path))
                                {
                                    byte[] imageArray = System.IO.File.ReadAllBytes(image1Path);
                                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                                    var response = SmartResponseClient.GetPredict(image1Path);
                                    if (response != null && response.Succeeded)
                                    {
                                        r = response.Payload;
                                    }
                                    //var client = new RestClient(SmartResponseCofig.SM_PREDICT_API);
                                    //var request = new RestRequest(Method.POST);
                                    //request.AddHeader("apikey", SmartResponseCofig.SM_PREDICT_APIKEY);
                                    //request.AddHeader("Content-Type", "multipart/form-data");
                                    //request.AddFile("file", imagePath, "file"); ;
                                    //IRestResponse<PredictState> res = client.Execute<PredictState>(request);
                                    //if (res.StatusCode == System.Net.HttpStatusCode.OK && res.Data != null)
                                    //{
                                    //    response = res.Data;
                                    //}
                                    //response.image = base64ImageRepresentation;
                                    r.prev = System.IO.Path.Combine(SmartResponseCofig.SMART_RESPONSE_UPLOADPATH, smDetail.Image1).TrimStart('~');
                                }
                                //PredictState r = _SM_CommonHelper.GetPredict(image1Path, smDetail.Image1);
                                data.IsError = (r == null || r != null && !r.fuseboxDetectionFlag);//checking the response is null OR response has fuse detection
                                if (data.IsError)
                                {
                                    data.ErrorDetails.Add(new SM_ErrorDetail() { ControlId = SM_Id.UploadSSD, ErorrMessage = _SM_CommonHelper.GetSMTranslation("Oops! this photo can’t be analyzed. Please point rightly to your electricity box and take/upload an adequate photo again.") });
                                    break;
                                }

                                if (r != null && Convert.ToBoolean(r.boxes?.Count > 0))
                                {
                                    DrawIssueImage(filePath, image1Path, r.boxes);
                                    _umDefault = SM_Code.The000;
                                }
                                CacheProvider.Store(CacheKeys.SM_AI_IMG_PREDICT, new CacheItem<PredictState>(r, TimeSpan.FromHours(1)));
                            }
                        }

                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _umDefault).FirstOrDefault());
                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new System.Collections.Generic.List<Models.SmartResponseModel.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }
                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case SM_Action.ShowInterruption:
                        var InterruptionUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.InterruptionService);
                        data.RedirectUrl = InterruptionUrl;

                        break;

                    case SM_Action.GETMAINTENANCEPROVIDERS:

                        DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse cmpyDetails = null;
                        if (!CacheProvider.TryGet<DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse>($"GetSRCompanyDetailsResponse_{RequestLanguage}", out cmpyDetails))
                        {
                            var d = DewaApiClient.GetSRCompanyDetails(new DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetails() { CompaniesInput = new DEWAXP.Foundation.Integration.DewaSvc.companiesInput() }, RequestLanguage, Request.Segment());
                            if (d != null && d.Payload != null)
                            {
                                cmpyDetails = d.Payload;
                                CacheProvider.Store($"GetSRCompanyDetailsResponse_{RequestLanguage}", new CacheItem<DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse>(d.Payload, TimeSpan.FromHours(1)));
                            }
                            else
                            {
                                ModelState.AddModelError("", d.Message);
                            }
                        }
                        data.SRCompanyDetails = cmpyDetails;

                        break;

                    default:
                        break;
                }
            }

            return data;
        }

        public CommonRender SR_TypeHelper(CommonRender data)
        {
            var IsStopAction = Convert.ToBoolean(data != null && Convert.ToBoolean(data.CurrentRequest?.IsPageBack));
            CustomerSubmittedData smDetail = _SM_CommonHelper.GetUserSubmittedData();
            if (data != null && data.Answer != null)
            {
                bool IsSessionExit = IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User);
                var activeUser = AuthStateService.GetActiveProfile();

                switch (data.Answer.Type)
                {
                    case TypeEnum.Accountselection:

                        data.RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CommonComplaint)}";

                        //logic to get All AccountRelated  detail and set it.
                        string accountNo = _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.Account);
                        if (!string.IsNullOrEmpty(accountNo) && IsLoggedIn && IsSessionExit)
                        {
                            var accountList = GetBillingAccounts(false, true, "");

                            var accountDetail = accountList?.Payload.FirstOrDefault(x => x.AccountNumber == accountNo);
                            if (accountDetail != null && !string.IsNullOrEmpty(accountDetail.AccountNumber))
                            {
                                string personName = GetContactNameByAccountDetails(accountDetail);
                                if (string.IsNullOrEmpty(personName))
                                {
                                    personName = GetContactNameByUserSession();
                                }
                                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ContactPersonName, personName);
                                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Mobile, activeUser.MobileNumber);
                                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.CA_Location, accountDetail.Location);
                                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Street, accountDetail.Street);
                                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Location, accountDetail.Street);
                                //TODO:SM--need to add location logic - DONE
                            }
                        }

                        break;

                    case TypeEnum.Button:
                        break;

                    case TypeEnum.Confirmation:

                        break;

                    case TypeEnum.Textinput:
                        break;

                    case TypeEnum.Notes:
                        break;

                    case TypeEnum.Loading:
                        break;

                    case TypeEnum.PercentageLoader:
                        var previousBack = _SM_CommonHelper.Percentageprevioustrackid(SmartResponseSessionHelper.ElectricityComplaintJsonSetting, data.Answer.ParentTrackingId);
                        data.f_bckid = previousBack;
                        break;

                    case TypeEnum.Showlist:
                        break;

                    default:
                        break;
                }
            }

            return data;
        }

        public CommonRender SR_IdHelper(CommonRender data)
        {
            var IsStopAction = Convert.ToBoolean(data != null && Convert.ToBoolean(data.CurrentRequest?.IsPageBack));
            var smDetail = _SM_CommonHelper.GetUserSubmittedData();
            SM_SessionType _type = _SM_CommonHelper.Get_SM_SessionType();
            if (data != null && data.Answer != null)
            {
                bool IsSessionExit = IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User);
                var activeUser = AuthStateService.GetActiveProfile();
                switch (data.Answer.Id)
                {
                    case SM_Id.Empty:
                        break;

                    case SM_Id.Location:
                        _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Street, smDetail.Location);
                        break;

                    case SM_Id.Mobile:
                        break;

                    case SM_Id.Account:
                        data.RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CommonComplaint)}";
                        //logic to get All AccountRelated  detail and set it.
                        string accountNo = _SM_CommonHelper.GetSelectedAnswerValue(SM_Id.Account);

                        bool isValidAccountNo = false;
                        if (!string.IsNullOrEmpty(accountNo))
                        {

                            if (IsLoggedIn && IsSessionExit && isValidAccountNo)
                            {
                                var accountList = GetBillingAccounts(false, true, "");

                                var accountDetail = accountList?.Payload.FirstOrDefault(x => x.AccountNumber == accountNo);
                                isValidAccountNo = accountDetail != null && !string.IsNullOrEmpty(accountDetail.AccountNumber);
                                if (isValidAccountNo)
                                {
                                    string personName = GetContactNameByAccountDetails(accountDetail);
                                    if (string.IsNullOrEmpty(personName))
                                    {
                                        personName = GetContactNameByUserSession();
                                    }
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.ContactPersonName, personName);
                                    _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Mobile, activeUser.MobileNumber);
                                    //TODO:SM--need to add laoction logic --DONE - its handle at (TypeEnum.Accountselection)
                                }
                            }
                            else
                            {
                                var acValidator = DewaApiClient.GetContractAccountStatus(new DEWAXP.Foundation.Integration.DewaSvc.GetContractAccountStatus()
                                {
                                    contractaccountinput = new DEWAXP.Foundation.Integration.DewaSvc.contractAccountStatusInput()
                                    {
                                        contractaccountnumber = accountNo,
                                    }
                                }, RequestLanguage, Request.Segment());

                                isValidAccountNo = acValidator.Succeeded;
                            }
                        }

                        data.IsError = !isValidAccountNo;

                        if (data.IsError)
                        {
                            data.ErrorDetails.Add(new SM_ErrorDetail() { ControlId = SM_Id.Account });
                        }

                        #region [Validate the data for Notitication Creation - Login multiple - single ac selection/Guest all flow]
                        if (!data.IsError)
                        {
                            var _smdSubmitDetail = new CustomerSubmittedData()
                            {
                                CustomerCategory = smDetail.CustomerCategory,
                                CodeGroup = smDetail.CodeGroup,
                                ContractAccountNo = accountNo,
                                CustomerType = smDetail.CustomerType,
                                Mobile = smDetail.Mobile
                            };
                            SubmitComplaintReponse validateSubmitedComplaint = null;
                            if (_type == SM_SessionType.IsLoggedIn)
                            {
                                validateSubmitedComplaint = ValidateAndSubmitLoginComplaints(_smdSubmitDetail, activeUser, new byte[2], new byte[2]);
                            }
                            else if (_type == SM_SessionType.IsGuest)
                            {
                                validateSubmitedComplaint = ValidateAndSubmitGuestComplaints(_smdSubmitDetail, activeUser, new byte[2], new byte[2]);


                            }

                            data.IsError = validateSubmitedComplaint != null
                                    && validateSubmitedComplaint.code == "791"
                                    && !string.IsNullOrWhiteSpace(validateSubmitedComplaint.notif);
                            if (data.IsError)
                            {
                                data.ErrorDetails.Add(new SM_ErrorDetail()
                                {
                                    ErorrMessage = _SM_CommonHelper.GetSMTranslation(Translate.Text("SM_ ResolutionStageText")).Replace("{​{​notif}​}", validateSubmitedComplaint.notif),
                                    ControlId = SM_Id.Error
                                });
                            }

                            if (!data.IsError)
                            {
                                data.IsError = validateSubmitedComplaint != null && validateSubmitedComplaint.code == "error";
                                if (data.IsError)
                                {
                                    data.ErrorDetails.Add(new SM_ErrorDetail()
                                    {
                                        ErorrMessage = _SM_CommonHelper.GetSMTranslation("SM_Error_UnableToProcess"),
                                        ControlId = SM_Id.Error
                                    });
                                }
                            }

                        }

                        #endregion
                        break;

                    case SM_Id.Success:
                        break;

                    case SM_Id.Media:
                        break;

                    case SM_Id.Error:
                        break;

                    case SM_Id.ContactPersonName:
                        break;

                    default:
                        break;
                }
            }

            return data;
        }

        public Models.SmartResponseModel.Question QuestionValueBinder(Models.SmartResponseModel.Question data)
        {
            Models.SmartResponseModel.Question returnData = null;
            if (data != null)
            {
                returnData = new Models.SmartResponseModel.Question()
                {
                    Answers = data.Answers,
                    Value = data.Value,
                    Infotext = data.Infotext,
                    Infotype = data.Infotype,
                    ParentTrackingId = data.ParentTrackingId,
                    TrackingId = data.TrackingId,
                    Code = data.Code,
                };
            }
            return returnData;
        }

        /// <summary>
        /// Validate Smart Meter
        /// </summary>
        /// <param name="meterNo"></param>
        /// <param name="modeOfCall"></param>
        /// <param name="session"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ValidateSmartMeter(string meterNo, string modeOfCall = "", string session = "", string userId = "")
        {
            var dResponse = DewaApiClient.GetValidSmartMeter(new DEWAXP.Foundation.Integration.DewaSvc.GetValidSmartMeter()
            {
                MeterIN = new DEWAXP.Foundation.Integration.DewaSvc.meterIN()
                {
                    meternumber = meterNo,
                    modeofcall = modeOfCall
                },
                UserCredentialIN = new DEWAXP.Foundation.Integration.DewaSvc.userCredentialIN()
                {
                    sessionid = session,
                    userid = userId,
                }
            }, RequestLanguage, Request.Segment());
            return (dResponse != null && dResponse.Succeeded);
        }

        public IEnumerable<System.Web.Mvc.SelectListItem> GetCities()
        {
            IEnumerable<System.Web.Mvc.SelectListItem> data = null;

            CacheProvider.TryGet(CacheKeys.SM_CITIES + RequestLanguage.ToString(), out data);
            if (data == null)
            {
                var result = DewaApiClient.GetServiceComplaintCriteria(RequestLanguage, Request.Segment());
                if (result.Succeeded)
                {
                    data = DropdownHelper.CityDropdown(result.Payload.CityList);
                    CacheProvider.Store(CacheKeys.SM_CITIES + RequestLanguage.ToString(), new CacheItem<IEnumerable<System.Web.Mvc.SelectListItem>>(data, TimeSpan.FromMinutes(20)));
                }
            }
            return data;
        }

        public string GetContactNameByUserSession()
        {
            var activeUser = AuthStateService.GetActiveProfile();
            if (activeUser != null)
            {
                return !string.IsNullOrWhiteSpace(activeUser.Name) ? activeUser.Name : activeUser.Username;
            }
            return "";
        }

        public string GetContactNameByAccountDetails(DEWAXP.Foundation.Integration.Responses.AccountDetails accountDetail)
        {
            if (accountDetail != null)
            {
                return !string.IsNullOrWhiteSpace(accountDetail.AccountName) ? accountDetail.AccountName : accountDetail.NickName;
            }
            return "";
        }

        //Delete file
        public bool DeleteFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return false;
        }

        public void DrawIssueImage(string filePath, string imgPath, List<List<double>> boxes)
        {
            try
            {
                string outPutImage = $"{SM_Id.Image1.ToString() + Guid.NewGuid()}.jpg";
                using (var image = Image.FromFile(imgPath))
                {
                    var image1Path = System.IO.Path.Combine(filePath, outPutImage);
                    //int thumbnailSize = 0;//img.Length;
                    //int newWidth, newHeight;

                    //if (image.Width > image.Height)
                    //{
                    //    newWidth = thumbnailSize;
                    //    newHeight = image.Height * thumbnailSize / image.Width;
                    //}
                    //else
                    //{
                    //    newWidth = image.Width * thumbnailSize / image.Height;
                    //    newHeight = thumbnailSize;
                    //}

                    //var thumbnailBitmap = new Bitmap(newWidth, newHeight);

                    using (var thumbnailGraph = Graphics.FromImage(image))
                    {
                        thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        //var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                        //thumbnailGraph.DrawImage(image, imageRectangle);

                        //Color color = Color.FromArgb(50, 255, 255, 255);
                        SolidBrush brush = new SolidBrush(Color.Red);
                        //Point atPoint = new Point(10, 10);
                        Pen pen = new Pen(brush, 10);
                        foreach (var item in boxes)
                        {
                            var rectCord = item;
                            if (rectCord != null)
                            {
                                thumbnailGraph.DrawRectangle(pen, new Rectangle(Convert.ToInt32(rectCord[0]), Convert.ToInt32(rectCord[1]), Convert.ToInt32(rectCord[2]), Convert.ToInt32(rectCord[3])));
                            }
                        }
                        thumbnailGraph.Dispose();
                    }
                    image.Save(image1Path);
                }

                DeleteFile(imgPath);
                _SM_CommonHelper.SetUserSelectedAnsTypeAndValue(SM_Id.Image1, outPutImage);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            // thumbnailBitmap.Dispose();
        }


        public SubmitComplaintReponse ValidateAndSubmitLoginComplaints(CustomerSubmittedData smDetail, DewaProfile activeUser, byte[] image1Bytes = null, byte[] image2Bytes = null)
        {
            SubmitComplaintReponse submitComplaintReponse = new SubmitComplaintReponse();

            var loginComplainData = new DEWAXP.Foundation.Integration.DewaSvc.loginComplaintsInput()
            {
                //appidentifier = "",
                //appversion = "",
                city = smDetail.Location,
                code = smDetail.Code,
                codegroup = smDetail.CodeGroup,
                contractaccountnumber = smDetail.ContractAccountNo,
                customertype = smDetail.CustomerType,
                customercategory = smDetail.CustomerCategory,
                docstream = image1Bytes,
                filename = smDetail.Image1,
                docstream2 = image2Bytes,
                filename2 = smDetail.Image2,
                housenumber = smDetail.CA_Location,
                //lang = "",
                makaninumber = "",
                mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile),
                //mobileosversion = "",
                name = smDetail.ContactPersonName,
                sessionid = Convert.ToString(activeUser?.SessionToken),
                street = smDetail.Street,
                text = smDetail.MoreDescription,
                trcode = smDetail.TrCode,
                trcodegroup = smDetail.TrCodeGroup,
                userid = Convert.ToString(activeUser?.UserId),
                //vendorid = "",
                xgps = smDetail.Latitude,
                ygps = smDetail.Longitude,
            };
            var _setLgCmpResponse = DewaApiClient.SetLoginComplaints(new DEWAXP.Foundation.Integration.DewaSvc.SetLoginComplaints()
            {
                logincomplaints = loginComplainData
            }, RequestLanguage, Request.Segment());
            if (_setLgCmpResponse != null &&
                _setLgCmpResponse.Payload != null &&
                _setLgCmpResponse.Payload.@return != null)
            {
                submitComplaintReponse.code = _setLgCmpResponse.Payload.@return.responsecode;
                submitComplaintReponse.description = _setLgCmpResponse.Payload.@return.description;

                if (_setLgCmpResponse.Succeeded && _setLgCmpResponse.Payload != null)
                {
                    submitComplaintReponse.complaintId = _setLgCmpResponse.Payload.@return.notificationnumber;
                }

                if (_setLgCmpResponse.Payload.@return.responsecode == "791")
                {
                    submitComplaintReponse.notif = _setLgCmpResponse.Payload.@return.description;
                }
            }
            else
            {
                submitComplaintReponse.code = "error";
                submitComplaintReponse.description = _setLgCmpResponse.Message;
            }

            return submitComplaintReponse;
        }
        public SubmitComplaintReponse ValidateAndSubmitGuestComplaints(CustomerSubmittedData smDetail, DewaProfile activeUser, byte[] image1Bytes = null, byte[] image2Bytes = null)
        {
            SubmitComplaintReponse submitComplaintReponse = new SubmitComplaintReponse();

            var guestComplaintData = new DEWAXP.Foundation.Integration.DewaSvc.guestComplaintsInput()
            {
                city = smDetail.Location,
                code = smDetail.Code,
                codegroup = smDetail.CodeGroup,
                customertype = smDetail.CustomerType,
                customercategory = smDetail.CustomerCategory,
                contractaccountnumber = smDetail.ContractAccountNo,
                docstream = image1Bytes,
                filename = smDetail.Image1,
                docstream2 = image2Bytes,
                filename2 = smDetail.Image2,
                //lang = "",
                makaninumber = "",
                mobile = _SM_CommonHelper.MobileTenFormat(smDetail.Mobile),
                name = smDetail.ContactPersonName,
                sessionid = activeUser.SessionToken,
                text = smDetail.MoreDescription,
                trcode = smDetail.TrCode,
                trcodegroup = smDetail.TrCodeGroup,
                xgps = smDetail.Latitude,
                ygps = smDetail.Longitude,
            };
            var _setGsCmpResponse = DewaApiClient.SetGuestComplaints(new DEWAXP.Foundation.Integration.DewaSvc.SetGuestComplaints()
            {
                guestcomplaints = guestComplaintData,
            }, RequestLanguage, Request.Segment());

            if (_setGsCmpResponse != null &&
                _setGsCmpResponse.Payload != null &&
                _setGsCmpResponse.Payload.@return != null)
            {
                submitComplaintReponse.code = _setGsCmpResponse.Payload.@return.responsecode;
                submitComplaintReponse.description = _setGsCmpResponse.Payload.@return.description;

                if (_setGsCmpResponse.Succeeded && _setGsCmpResponse.Payload != null)
                {
                    submitComplaintReponse.complaintId = _setGsCmpResponse.Payload.@return.notificationnumber;
                }
                if (_setGsCmpResponse.Payload.@return.responsecode == "791")
                {
                    submitComplaintReponse.notif = _setGsCmpResponse.Payload.@return.description;
                }
            }
            else
            {
                submitComplaintReponse.code = "error";
                submitComplaintReponse.description = _setGsCmpResponse.Message;
            }
            return submitComplaintReponse;
        }
        #endregion Functions

        #endregion Smart Response
    }
}