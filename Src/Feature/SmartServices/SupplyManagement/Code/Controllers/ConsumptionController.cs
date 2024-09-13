using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models.Consumption;
using DEWAXP.Foundation.Content.Models.ContactDetails;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Utils;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartResponseModel;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using _CC_CommonHelper = DEWAXP.Feature.SupplyManagement.Helpers.ConsumptionComplaint.ConsumptionHelper;
using _CC_Model = DEWAXP.Feature.SupplyManagement.Models.ConsumptionComplaint;
using _CC_SessionHelper = DEWAXP.Feature.SupplyManagement.Helpers.ConsumptionComplaint.ConsumptionSessionHelper;
using _commonUtility = DEWAXP.Foundation.Content.Utils.CommonUtility;
using apiModel = DEWAXP.Foundation.Integration.APIHandler.Models;
using ScContext = Sitecore.Context;
using ScData = Sitecore.Data;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class ConsumptionController : BaseController
    {
        private readonly IDropdownHelper _dropdownHelper;
        protected IDropdownHelper DropdownHelper => _dropdownHelper;
        public ConsumptionController() : base()
        {
            _dropdownHelper = DependencyResolver.Current.GetService<IDropdownHelper>();
        }
        #region [private varaible]

        private string _UploadedFile = "_UploadedFile";

        #endregion [private varaible]

        #region [Actions]

        // GET: Consumption
        [HttpGet, TwoPhaseAuthorize]
        public ActionResult ComplaintResponse(_CC_Model.SmPageRequestModel pgRqst)
        {
            if (Convert.ToBoolean(pgRqst.se == "0x00x"))
            {
                _CC_CommonHelper.Clear_CC_Session();
                CacheProvider.Remove(CacheKeys.SM_SC_TRANSLATION_ITEMS);
            }

            var lanType = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.LangType);

            #region [Set Language on SC lang]

            if (lanType == null || lanType == "")
            {
                if (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic)
                {
                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.LangType, SmlangCode.ar.ToString());
                }
                else if (RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic)
                {
                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.LangType, SmlangCode.en.ToString());
                }
            }
            lanType = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.LangType);

            #endregion [Set Language on SC lang]

            var _currentRequest = new _CC_Model.CommonRenderRequest()
            {
                IsPageRefesh = Convert.ToBoolean(pgRqst.isLang == 0),
                LangType = _CC_CommonHelper.GetSMLangType(lanType),
            };
            _CC_Model.CommonRender model = new _CC_Model.CommonRender()
            {
                CurrentRequest = _currentRequest,
            };

            bool AccountSeletor = Convert.ToBoolean(pgRqst.asc) && IsLoggedIn;

            ViewBag.ShowAccountSelector = AccountSeletor;
            if (AccountSeletor && _CC_SessionHelper.CC_CurrentUserAnswer != null)
            {
                model = _CC_SessionHelper.CC_CurrentUserAnswer.LastOrDefault().Value;
                if (model != null)
                {
                    if (model.CurrentRequest.LangType == null)
                    {
                        model.CurrentRequest.LangType = _CC_CommonHelper.GetSMLangType(_CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.LangType));
                    }
                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.LangType, model.CurrentRequest.LangType.ToString());
                    model.CurrentRequest.IsPageRefesh = true;
                }
            }
            //handled langauge on page redirect.
            if (pgRqst.l.HasValue)
            {
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.LangType, pgRqst.l.Value.ToString());
            }

            _CC_SessionHelper.ConsumptionComplaintJsonSetting = _CC_CommonHelper.LoadUpdatedConsumptionComplaintModelJson();
            //SmartResponse.SessionHelper.TempElectricityComplaintJsonSetting = new JsonMasterModel() { Questions = SmartResponse.SessionHelper.ElectricityComplaintJsonSetting.Questions };

            ///if directly wants to land on notification list page

            //if (!string.IsNullOrWhiteSpace(pgRqst.n))
            //{
            //    model = _CC_CommonHelper.GetGuestNotiicationListModel(pgRqst.n, Convert.ToInt32(pgRqst.t));
            //    ViewBag.ShowNotification = true;
            //}

            if (pgRqst.s != null && pgRqst.s.HasValue)
            {
                ViewBag.ShowNotification = true;
                switch (pgRqst.s.Value)
                {
                    case _CC_Model.SmScreenCode.ant:
                        model = _CC_CommonHelper.GetTrackOtherIncident(_CC_SessionHelper.ConsumptionComplaintJsonSetting);
                        break;

                    case _CC_Model.SmScreenCode.d:
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
            _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.IsUserLoggedIn, Convert.ToString(IsLoggedIn?1:0));

            if (!string.IsNullOrWhiteSpace(pgRqst.n) && !string.IsNullOrWhiteSpace(pgRqst.a))
            {
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.NotificationNumber, pgRqst.n);
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Account, pgRqst.a);
            }
            return PartialView("~/Views/Feature/SupplyManagement/Consumption/ComplaintResponse.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorize]
        public ActionResult ComplaintResponse(_CC_Model.CommonRenderRequest request)
        {
            bool isPreviousError = false;
            List<_CC_Model.SM_ErrorDetail> previousErrorDetails = null;
            _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.IsUserLoggedIn, Convert.ToString(IsLoggedIn ? 1 : 0));

            string notif = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.NotificationNumber);
            string acNO = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Account);
            bool IsTrackEnabled = (!string.IsNullOrWhiteSpace(notif) && !string.IsNullOrWhiteSpace(acNO) && request.IsStart);

            _CC_Model.CommonRender prevoiusAnswer = null;
            SmlangCode PrevLang = _CC_CommonHelper.GetSMLangType(_CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.LangType));

            if (!request.IsPageBack)
            {
                if (_CC_SessionHelper.CC_CurrentUserAnswer != null && !request.IsStart && request.TckId == 0)
                {
                    prevoiusAnswer = _CC_SessionHelper.CC_CurrentUserAnswer.LastOrDefault().Value;
                    //
                    if (prevoiusAnswer != null && prevoiusAnswer.Answer.Action == _CC_Model.SM_Action.Customerlogin)
                    {
                        prevoiusAnswer = _CC_SessionHelper.CC_CurrentUserAnswer?[_CC_SessionHelper.CC_CurrentUserAnswer.Count - 2];
                    }
                    int lastTrackingId = Convert.ToInt32(prevoiusAnswer?.Answer?.TrackingId);
                    request.TckId = lastTrackingId > 1 ? lastTrackingId : request.TckId;
                }
            }
            if (request.LangType == null)
            {
                request.LangType = PrevLang;
            }

            //force calling track request
            if (request.IsAnsAltered || IsTrackEnabled)
            {
                _CC_SessionHelper.ConsumptionComplaintJsonSetting = _CC_CommonHelper.LoadUpdatedConsumptionComplaintModelJson();

                request.IsAnsAltered = false;
            }

        INITIATE_CURRENT_ANS:
            var currentAns = _CC_CommonHelper.GetRenderModel(request, _CC_SessionHelper.ConsumptionComplaintJsonSetting);
            //var tempCurrentAns = _CC_CommonHelper.GetRenderModel(request, SmartResponse.SessionHelper.TempElectricityComplaintJsonSetting);

            if (currentAns.Answer.Action == _CC_Model.SM_Action.ReportTechnicalIncident)
            {
                var d = _CC_CommonHelper.GetReportTechnicalIncident(_CC_SessionHelper.ConsumptionComplaintJsonSetting);
                if (d != null && d.Answer != null)
                {
                    currentAns = d;
                }
            }

            //force calling track request
            if (IsTrackEnabled)
            {
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.NotificationNumber, notif);
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Account, acNO);
                currentAns.Answer = _CC_SessionHelper.ConsumptionComplaintJsonSetting.Questions.FirstOrDefault().Answers[1];
            }

            try
            {
                if (currentAns.Answer.Action == _CC_Model.SM_Action.TrackAnotherIncident)
                {
                    var d = _CC_CommonHelper.GetTrackOtherIncident(_CC_SessionHelper.ConsumptionComplaintJsonSetting);
                    if (d != null && d.Answer != null)
                    {
                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.NotificationNumber, "");
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
            TrackNotif:
                currentAns = SR_ActionHelper(currentAns);//

                if (currentAns.CurrentRequest.IsForceTrack)
                {
                    currentAns.CurrentRequest.IsForceTrack = false;
                    currentAns.CurrentRequest.IsStart = false;
                    goto TrackNotif;
                }
                currentAns = SR_TypeHelper(currentAns);
                currentAns = SR_IdHelper(currentAns);
            }

            #region [Error handling]

            if (!isPreviousError && (currentAns.IsError || currentAns.Answer.Action == _CC_Model.SM_Action.Resendotpmobile))
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

            _CC_Model.CustomerSubmittedData smDetail = _CC_CommonHelper.GetUserSubmittedData();
            //Add username in Required place
            //int i = 0;
            foreach (var item in currentAns.Answer.Questions.Where(x => x != null).ToList())
            {
                string username = IsLoggedIn && activeUser != null ? activeUser.Username : _CC_CommonHelper.GetSMTranslation("User");
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.UsernName, username);
                //item.Value = _CC_CommonHelper.GetSMTranslation(item.Value)?.Replace("{{username}}", _CC_CommonHelper.TextHighlight(username));

                if (item.Answers != null)
                {
                    if (item.Answers.FirstOrDefault()?.Type == _CC_Model.TypeEnum.Confirmation)
                    {
                        //review page logic
                        currentAns.RequestConfirmationDetail = new _CC_Model.RequestConfirmationDetail();
                        currentAns.RequestConfirmationDetail.ContactAccountNo = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Account);
                        currentAns.RequestConfirmationDetail.ContactPerson = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ContactPersonName);
                        currentAns.RequestConfirmationDetail.ContactNumber = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Mobile);
                        currentAns.RequestConfirmationDetail.ContactMonth = _CC_CommonHelper.GetConsumptionTextValue(_CC_Model.SM_Id.BillingMonth);
                        currentAns.RequestConfirmationDetail.ContactEmail = _CC_CommonHelper.GetConsumptionTextValue(_CC_Model.SM_Id.Email);
                        currentAns.RequestConfirmationDetail.ContactLocation = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.CA_Location) + " " + _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Street);
                    }
                    var filterAns = item.Answers.Where(x => x != null && x.Action == _CC_Model.SM_Action.GetNotificationList).FirstOrDefault();
                    if (filterAns != null)
                    {
                        //item.Value = item.Value?.Replace("{{notif}}", _CC_CommonHelper.TextHighlight(smDetail.NotificationNumber));
                        filterAns.Actiondata = smDetail.NotificationNumber;
                    }
                }
            }

            //var currentQus = SmartDubaiModel.CommonHelper.GetRenderModel(request.QusId, request.IsStart);
            currentAns.RedirectCount = Convert.ToInt32(prevoiusAnswer?.RedirectCount ?? currentAns.RedirectCount) + 1; //this to stop multiple page redirect

            if (!request.IsStart)
            {
                if (_CC_SessionHelper.CC_CurrentUserAnswer == null)
                {
                    _CC_SessionHelper.CC_CurrentUserAnswer = new Dictionary<int, _CC_Model.CommonRender>();
                }

                _CC_SessionHelper.CC_CurrentUserAnswer?.Add(_CC_SessionHelper.CC_CurrentUserAnswer?.Count ?? 0, currentAns);
            }
            else
            {
                _CC_SessionHelper.CC_CurrentUserAnswer = null;
            }

            try
            {
                if (request.IsPageBack || request.IsPageRefesh)
                {
                    #region [SetSelectedAns]

                    LogService.Debug("start SetSelectedAns");
                    if (_CC_SessionHelper.CC_CurrentUserAnswer != null && _CC_SessionHelper.CC_CurrentUserAnswer.Count > 0)
                    {
                        var r = _CC_SessionHelper.CC_CurrentUserAnswer.Where(x => x.Value != null && x.Value.Answer != null && x.Value.Question != null).Select(x => new
                        {
                            QuesId = x.Value.Question.TrackingId,
                            AnsId = x.Value.Answer.TrackingId,
                            SrNo = x.Key
                        }).OrderByDescending(x => x.SrNo);

                        foreach (var item in currentAns.Answer.Questions.Where(x => x != null & x.Answers.Count > 0))
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
                                            Where(x => x.Id == _CC_Model.SM_Id.Success && (x.Answers == null ||
                                            (x.Answers != null && x.Answers.Count == 0))).Count() > 0);
                byte[] image1Bytes = new byte[2];
                byte[] image2Bytes = new byte[2];
                var filePath = Server.MapPath(ConsumptionComplaintResponseCofig.CC_RESPONSE_UPLOADPATH);
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
                            image2Path = System.IO.Path.Combine(filePath, smDetail.Image2);
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
                            customertype = smDetail.CustomerType ?? "CO01",
                            customercategory = smDetail.CustomerCategory,
                            docstream = image1Bytes,
                            filename = smDetail.Image1,
                            docstream2 = image2Bytes,
                            filename2 = smDetail.Image2,
                            //housenumber = smDetail.CA_Location,
                            //lang = "",
                            makaninumber = "",
                            mobile = _CC_CommonHelper.MobileTenFormat(smDetail.Mobile),
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
                            businessPartner = smDetail.PartnerNo,
                            trcodegroup2 = smDetail.TrCodeGroup2,
                            trcode2 = smDetail.TrCode2,
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
                        _CC_Model.SM_Id idType = (_CC_Model.SM_Id)Enum.Parse(typeof(_CC_Model.SM_Id), item);
                        if (idType == _CC_Model.SM_Id.Image1 || idType == _CC_Model.SM_Id.Image2)
                        {
                            string imageFileName = _CC_CommonHelper.GetSelectedAnswerValue(idType);
                            if (!string.IsNullOrWhiteSpace(imageFileName))
                            {
                                string imageFilePath = Path.Combine(filePath, imageFileName);
                                DeleteFile(imageFilePath);
                            }
                        }
                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(idType, null);
                    }
                }

                #endregion [if page back clear the previous page value session ]
            }
            catch (Exception ex)
            {
                LogService.Debug("SetSelectedAns");
                LogService.Error(ex, this);
            }

            bool IsSuccessPage = false;//currentAns.Answer?.Questions.FirstOrDefault(x => x.Id == _CC_Model.SM_Id.Success) != null;

            if (currentAns != null && !string.IsNullOrEmpty(currentAns.RedirectUrl) && currentAns.RedirectCount <= 1 && !IsSuccessPage)
            {
                if (currentAns.Answer.Action == _CC_Model.SM_Action.Track ||
                    currentAns.Answer.Action == _CC_Model.SM_Action.GetNotificationList)
                {
                    if (currentAns.Answer.Type != _CC_Model.TypeEnum.Accountselection)
                        _CC_CommonHelper.Clear_CC_Session();
                }

                if (!string.IsNullOrWhiteSpace(smDetail.ContractAccountNo))
                {
                    if (Convert.ToBoolean(currentAns?.RedirectUrl.Contains("?")))
                    {
                        currentAns.RedirectUrl = currentAns.RedirectUrl + "&a=" + smDetail.ContractAccountNo;
                    }
                    else
                    {
                        currentAns.RedirectUrl = currentAns.RedirectUrl + "?a=" + smDetail.ContractAccountNo;
                    }
                }
                return Json(currentAns, JsonRequestBehavior.DenyGet);
            }

            currentAns.CurrentRequest.DdlList = GetCities();

            return PartialView($"/Views/Feature/SupplyManagement/Consumption/GenericComplaintResponse.cshtml", currentAns);
        }

        /// <summary>
        /// scd (surveyCode)= s1 ,s2
        /// n (surveyno)= rand no
        /// </summary>
        /// <param name="s"></param>
        /// <param name="n"></param>
        /// <returns></returns>

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SmartDubaiNotificationList(_CC_Model.TrackingRequest request)
        {
            _CC_Model.TrackingResponse model = new _CC_Model.TrackingResponse();
            model.TrackingRequest = request;
            try
            {
                //if (request.Id == _CC_Model.SM_Id.NotificationNumber)
                //{
                //    var r = DewaApiClient.GetGuestTrackComplaints(new DEWAXP.Foundation.Integration.DewaSvc.GetGuestTrackComplaints()
                //    {
                //        guesttrackinput = new DEWAXP.Foundation.Integration.DewaSvc.guestTrackInput()
                //        {
                //            notificationnumber = request.SearchText,
                //        }
                //    }, RequestLanguage, Request.Segment());

                //    if (r != null && r.Succeeded && r.Payload?.@return?.notificationlist != null)
                //    {
                //        model.TrackNotificationDetails = r.Payload?.@return?.notificationlist.ToList();

                //    }

                //}
                //else if (request.Id == _CC_Model.SM_Id.SearchTxt)
                //{
                //    var activeUser = AuthStateService.GetActiveProfile();
                //    var r = DewaApiClient.GetLoginTrackComplaints(new DEWAXP.Foundation.Integration.DewaSvc.GetLoginTrackComplaints()
                //    {
                //        logintrackinput = new DEWAXP.Foundation.Integration.DewaSvc.loginTrackInput()
                //        {
                //            userid = activeUser.UserId,

                //            sessionid = activeUser.SessionToken,
                //        }
                //    }, RequestLanguage, Request.Segment());

                //    if (r != null && r.Succeeded && r.Payload?.@return?.notificationlist != null)
                //    {
                //        model.TrackNotificationDetails = r.Payload?.@return?.notificationlist.ToList();

                //        if (!string.IsNullOrEmpty(request.SearchText))
                //        {
                //            model.TrackNotificationDetails = model.TrackNotificationDetails.Where(x => x != null && x.notificationnumber.Contains(request.SearchText)).ToList();
                //        }
                //    }
                //}

                if (string.IsNullOrWhiteSpace(request.SearchText))
                {
                    request.SearchText = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.NotificationNumber);
                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.NotificationNumber, null);
                }

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
                        model.TrackNotificationDetails = model.TrackNotificationDetails.Where(x => x != null && (x.notificationtype == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE || x.notificationtype == _CC_Model.CommonConst.ConsumptionType_WATERCODE) && x.notificationnumber.Contains(request.SearchText)).ToList();
                    }
                }

                if (model.TrackNotificationDetails != null)
                {
                    model.TrackNotificationDetails = model.TrackNotificationDetails.Where(x => (x.notificationtype == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE || x.notificationtype == _CC_Model.CommonConst.ConsumptionType_WATERCODE)).OrderByDescending(x => _commonUtility.DateTimeFormatParse(x.notificationdate + " " + x.notificationtime, _commonUtility.DF_yyyy_MM_dd_HHmmss).ToString(_commonUtility.DF_yyyyMMddHHmmss)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView($"~/Views/Feature/SupplyManagement/Consumption/NotificationList.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumptionNotificationList(_CC_Model.TrackingRequest request)
        {
            _CC_Model.TrackingResponse model = new _CC_Model.TrackingResponse();

            model.AccountNo = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Account);//"2001188552";
            model.TrackingRequest = request;
            try
            {
                model.ConsumptionTrackingDetailList = GetNotificationDataList(model.AccountNo, "CV");
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.NotificationNumber, null);
            CacheProvider.Store("GetTrackingData", new CacheItem<_CC_Model.TrackingResponse>(model));
            return PartialView($"~/Views/Feature/SupplyManagement/Consumption/ConsumptionNotificationList.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumptionTrackStatusList(_CC_Model.NotificationStatusModel model)
        {
            model.StatusList = new List<apiModel.Response.DTMCTracking.WorkDetail>();
            try
            {
                var activeUser = AuthStateService.GetActiveProfile();
                var repsonseData = DTMCTrackingClient.GetNotificationStatus(new apiModel.Request.DTMCTracking.NotificationStatusRequest()
                {
                    notificationdtmcinput = new apiModel.Request.DTMCTracking.notificationdtmcinput()
                    {
                        notificationnumber = model.N,
                        lang = RequestLanguageCode,
                        userid = activeUser.UserId,
                        sessionid = activeUser.SessionToken
                    }
                }, Request.Segment());
                if (repsonseData != null)
                {
                    if (repsonseData.Succeeded)
                    {
                        model.StatusList = repsonseData.Payload.worklist;
                        CacheProvider.Store("GetTrackingData" + model.N, new CacheItem<List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCTracking.WorkDetail>>(model.StatusList, TimeSpan.FromMinutes(40)));
                    }
                    else
                    {
                        model.Message = repsonseData.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView($"~/Views/Feature/SupplyManagement/Consumption/ConsumptionNotificationTrackList.cshtml", model);
        }

        [HttpGet, TwoPhaseAuthorize]
        public ActionResult ConsumptiontTrackingMap(string n)
        {
            string msg = null;
            _CC_Model.TrackingResponse _storeValue = null;
            var activeUser = AuthStateService.GetActiveProfile();
            _CC_Model.TrackingMapDetail model = new _CC_Model.TrackingMapDetail();
            try
            {
                if (CacheProvider.TryGet("GetTrackingData", out _storeValue))
                {
                    model.NotificationDetail = _storeValue?.ConsumptionTrackingDetailList?.Where(x => x.Reference == n).FirstOrDefault();

                    if (model.NotificationDetail != null)
                    {
                        #region [tracking Data binding]

                        List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCTracking.WorkDetail> workDetail = null;
                        if (!CacheProvider.TryGet("GetTrackingData" + n, out workDetail))
                        {
                            var repsonseStatusDataList = DTMCTrackingClient.GetNotificationStatus(new apiModel.Request.DTMCTracking.NotificationStatusRequest()
                            {
                                notificationdtmcinput = new apiModel.Request.DTMCTracking.notificationdtmcinput()
                                {
                                    notificationnumber = model.NotificationDetail.Reference,
                                    lang = RequestLanguageCode,
                                    userid = activeUser.UserId,
                                    sessionid = activeUser.SessionToken
                                }
                            }, Request.Segment());
                            if (repsonseStatusDataList != null)
                            {
                                if (repsonseStatusDataList.Succeeded)
                                {
                                    workDetail = repsonseStatusDataList.Payload.worklist;
                                    CacheProvider.Store("GetTrackingData" + n, new CacheItem<List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.DTMCTracking.WorkDetail>>(workDetail, TimeSpan.FromMinutes(40)));
                                }
                                else
                                {
                                    //model.Message = repsonseStatusDataList.Message;
                                }
                            }
                        }

                        if (workDetail != null && workDetail.FirstOrDefault(x => x.status == "S002" && x.map == "X") != null)
                        {
                            var trackingData = workDetail.FirstOrDefault(x => x.status == "S002" && x.map == "X");
                            model.FromLocation = new apiModel.Response.DTMCTracking.LocationDetail()
                            {
                                geolatitude = trackingData.fromxcord,
                                geolongitude = trackingData.fromycord,
                            };

                            model.ToLocation = new apiModel.Response.DTMCTracking.LocationDetail()
                            {
                                geolatitude = trackingData.toxcord,
                                geolongitude = trackingData.toycord
                            };

                            model.GuidText = trackingData.guid;
                        }
                        else
                        {
                            msg = Translate.Text("DTMC_TrackingDataIssue");
                        }
                        //else
                        //{
                        //    //CacheProvider.Store("GetTrackingData"+ model.N,

                        //    model.NotificationDetail = _storeValue?.ConsumptionTrackingDetailList?.Where(x => x.Reference == n).FirstOrDefault();
                        //    model.AccountNo = _storeValue.AccountNo;
                        //    //Get Client Start Geo Location
                        //    var repsonseData = DTMCTrackingClient.GetLocationStatus(new apiModel.Request.DTMCTracking.LocationStatusRequest()
                        //    {
                        //        notificationdtmcinput = new apiModel.Request.DTMCTracking.LocationStatusInput()
                        //        {
                        //            notificationnumber = n,
                        //            lang = RequestLanguageCode,
                        //            userid = activeUser.UserId,
                        //            sessionid = activeUser.SessionToken
                        //        }
                        //    }, Request.Segment());

                        //    if (repsonseData != null)
                        //    {
                        //        if (repsonseData.Succeeded)
                        //        {
                        //            model.FromLocation = repsonseData.Payload.notiflocation;
                        //        }
                        //        else
                        //        {
                        //            msg = repsonseData.Message;
                        //        }
                        //    }

                        //    //Get User Account - Home GeoLocation
                        //    var accountDetail = GetBillingAccounts(false, true, "")?.Payload.Where(x => x != null && x.AccountNumber == model.AccountNo).FirstOrDefault();
                        //    if (accountDetail != null)
                        //    {
                        //        model.ToLocation = new apiModel.Response.DTMCTracking.LocationDetail()
                        //        {
                        //            geolatitude = accountDetail.XCordinate,
                        //            geolongitude = accountDetail.YCordinate
                        //        };
                        //    }
                        //    else
                        //    {
                        //        msg = Translate.Text("DTMC_ErrorAccountDetail");
                        //    }
                        //}

                        #endregion [tracking Data binding]
                    }
                    else
                    {
                        msg = Translate.Text("DTMC_ErrorDataNoAvaliable");
                    }
                }
                else
                {
                    msg = Translate.Text("DTMC_ErrorDataNoAvaliable");
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogService.Fatal(ex, this);
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                model.ErrorMessages = msg;
            }
            return PartialView($"~/Views/Feature/SupplyManagement/Consumption/ConsumptiontTrackingMap.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetConsumptiontTrackingMapData(_CC_Model.NotificationStatusModel model)
        {
            var activeUser = AuthStateService.GetActiveProfile();
            var repsonseData = DTMCTrackingClient.GetLocationStatus(new apiModel.Request.DTMCTracking.LocationStatusRequest()
            {
                notificationdtmcinput = new apiModel.Request.DTMCTracking.LocationStatusInput()
                {
                    notificationnumber = model.N,
                    lang = RequestLanguageCode,
                    userid = activeUser.UserId,
                    sessionid = activeUser.SessionToken,
                    guid = model.g
                }
            }, Request.Segment());

            if (repsonseData != null)
            {
                if (repsonseData.Succeeded)
                {
                    var tdata = repsonseData.Payload.notiflocation;
                    if (Convert.ToDecimal(tdata.geolatitude) != Convert.ToDecimal(model.lat) &&
                        Convert.ToDecimal(tdata.geolongitude) != Convert.ToDecimal(model.lng))
                    {
                        return Json(new { success = true, data = tdata }, JsonRequestBehavior.DenyGet);
                    }
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UploadSMComplaintFile(_CC_Model.SM_FileUplploadRequest upRequest)
        {
            try
            {
                string error;
                string[] AcceptedImageFileTypes = { ".JPG", ".JPEG", ".PNG" };
                if (!AttachmentIsValid(upRequest.Uploadfile, General.MaxAttachmentSize, out error, AcceptedImageFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    CacheProvider.Remove(_UploadedFile + upRequest.ImageType.ToString());
                    CacheProvider.Remove(_UploadedFile + "bytes");
                    if (upRequest.Uploadfile != null && upRequest.Uploadfile.ContentLength > 0)
                    {
                        CacheProvider.Store(_UploadedFile + upRequest.ImageType.ToString(), new CacheItem<HttpPostedFileBase>(upRequest.Uploadfile, TimeSpan.FromHours(30)));
                        return Json(new { success = true, fName = upRequest.Uploadfile.FileName }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMarkerImage(string t)
        {
            string ImagePath = System.Web.HttpContext.Current.Server.MapPath(UtilAppConfig.DestMapMakerImage);

            Image _image = null;
            ImageDetail imageDetail = new ImageDetail()
            {
                FileName = "m.png",
                ImagePath = ImagePath,
                ImageTextDetails = new List<ImageTextDetail>(),
                ImageFormat = ImageFormat.Png
            };

            Color color = System.Drawing.ColorTranslator.FromHtml("#000");
            StringFormat centeredFromat = new StringFormat();
            centeredFromat.Alignment = StringAlignment.Center;
            centeredFromat.LineAlignment = StringAlignment.Center;
            imageDetail.ImageTextDetails.Add(new ImageTextDetail()
            {
                InputText = t?.Replace(" ", "\n"),
                brush = new SolidBrush(color),
                font = new Font("arial", 10, FontStyle.Regular),
                format = centeredFromat,
                isTextCentered = true,
            });

            try
            {
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    _image = ImageUtilityHelper.GetEditedImage(imageDetail);
                }
            }
            catch (Exception ex)
            {
                _image = (System.Drawing.Image)Bitmap.FromFile(ImagePath);
            }
            return File(ImageUtilityHelper.ImageToByteArray(_image, imageDetail.ImageFormat), "image/png");
        }

        public ActionResult GetConsumptionInsight()
        {
            _CC_Model.Question currentQuestion = null;
            _CC_Model.ConsumptionInsightData model = null;
            apiModel.Response.DTMCInsightsReport.GetWaterAIResponse getwaterai_response = null;
            _CC_Model.CustomerSubmittedData smDetail = _CC_CommonHelper.GetUserSubmittedData();

            if (CacheProvider.TryGet(CacheKeys.FILTER_AMI_STEP, out currentQuestion))
            {
                if (CacheProvider.TryGet(CacheKeys.CONSUMPTION_INSIGHT_DATAMODEL, out model))
                {
                    model.AlarmListSetting = currentQuestion.Answers.FirstOrDefault(x => x.Type == _CC_Model.TypeEnum.insights && x.Id == _CC_Model.SM_Id.meteralerts);
                    model.BehaviourSetting = currentQuestion.Answers.FirstOrDefault(x => x.Type == _CC_Model.TypeEnum.insights && x.Id == _CC_Model.SM_Id.behaviour);
                    model.ConsumptionGraphDataShowSetting = currentQuestion.Answers.FirstOrDefault(x => x.Type == _CC_Model.TypeEnum.insights && x.Id == _CC_Model.SM_Id.consumptiongraph);
                    model.ConsumptionSlabDataListSetting = currentQuestion.Answers.FirstOrDefault(x => x.Type == _CC_Model.TypeEnum.insights && x.Id == _CC_Model.SM_Id.consumptionslab);
                    model.ConsumptionUsageSetting = currentQuestion.Answers.FirstOrDefault(x => x.Type == _CC_Model.TypeEnum.insights && x.Id == _CC_Model.SM_Id.aireport);

                    if (CacheProvider.TryGet(CacheKeys.GETWATERAI_RESPONSE, out getwaterai_response))
                    {
                        if (getwaterai_response != null)
                        {
                            if (model.BehaviourSetting != null)
                            {
                                model.Behaviour = new List<_CC_Model.ConsumptionData>();

                                foreach (var item in getwaterai_response.ReturnData.Consolidated)
                                {
                                    model.Behaviour.Add(new _CC_Model.ConsumptionData()
                                    {
                                        Data = item.cotcode == 0 ? Translate.Text("SM_WaterNormal") : Translate.Text("SM_WaterAbnormal"),
                                        Data1 = item.cot,
                                        Data2 = Convert.ToString(item.cotcode),
                                        Data3 = Convert.ToString(item.reason)
                                    });
                                }
                            }

                            if (model.ConsumptionUsageSetting != null)
                            {
                                model.ConsumptionUsage = new List<_CC_Model.ConsumptionData>();

                                if (getwaterai_response.ReturnData.Aggs != null)
                                {
                                    foreach (SelectListItem CUsageitem in GetDataSource(SitecoreItemIdentifiers.DTMC_ConsumptionSlabData, null, ScData.Managers.LanguageManager.GetLanguage("en")))
                                    {
                                        var _data = new _CC_Model.ConsumptionData()
                                        {
                                            Data = _CC_CommonHelper.GetSMTranslation(CUsageitem.Text),
                                            Data1 = CUsageitem.Value?.ToUpper()?.Replace("AM", _CC_CommonHelper.GetSMTranslation("AM"))?.Replace("PM", _CC_CommonHelper.GetSMTranslation("PM")),
                                        };
                                        switch (CUsageitem.Text?.ToLower())
                                        {
                                            case "morning":
                                                _data.Data2 = getwaterai_response.ReturnData.Aggs.morning;
                                                break;

                                            case "afternoon":
                                                _data.Data2 = getwaterai_response.ReturnData.Aggs.midday;
                                                break;

                                            case "evening":
                                                _data.Data2 = getwaterai_response.ReturnData.Aggs.evening;
                                                break;

                                            case "night":
                                                _data.Data2 = getwaterai_response.ReturnData.Aggs.night;
                                                break;

                                            default:
                                                break;
                                        }
                                        model.ConsumptionUsage.Add(_data);
                                    }
                                    model.ConsumptionUsageSetting.BtnValue = GetJsonStringOfConsumptionUsage(model.ConsumptionUsage);
                                }
                            }

                            if (model.AlarmListSetting != null)
                            {
                                model.Alarms = getwaterai_response.ReturnData.Alarms;
                            }

                            if (model.ConsumptionSlabDataListSetting != null)
                            {
                                model.SlabCaps = GeSlabCapsData();
                                model.ConsumptionSlabDataList = GetConsumptionSlabData();
                            }

                            if (model.ConsumptionGraphDataShowSetting != null)
                            {
                                model.ConsumptionGraphData = GetConsumptionGraphDetail(model, getwaterai_response, smDetail);
                            }
                        }
                    }
                }
            }

            return PartialView($"~/Views/Feature/SupplyManagement/Consumption/WaterConsumptionInsight.cshtml", model);
        }

        #endregion [Actions]

        #region [Survey Actions]

        [HttpGet]
        public ActionResult Survey(_CC_Model.SurveyRequestModel request)
        {
            _CC_Model.SurveyInfoDetail model = new _CC_Model.SurveyInfoDetail();
            try
            {
                #region [get Survey Detail config]

                /*get Suvey Id and Suvery Appliction Id by Sitecore Item*/
                var surveyDetail = GetDataSource(SitecoreItemIdentifiers.DTMC_SURVEY_DATALIST_SCPATH, request.s).FirstOrDefault();
                model.ShowError = (surveyDetail == null);

                #endregion [get Survey Detail config]

                #region [validate survey Notification No]

                if (!model.ShowError && request.vd == "1")
                {
                    /*Survey Id validation*/
                    var surveyValidationRepsonse = ConsultantServiceClient.ValidateSurvey(new DEWAXP.Foundation.Integration.SmartConsultantSvc.ValidateSurvey()
                    {
                        transaction = request.n
                    }, RequestLanguage, Request.Segment());

                    if (string.IsNullOrWhiteSpace(request.s) || string.IsNullOrWhiteSpace(request.n) ||
                                      Convert.ToBoolean(surveyValidationRepsonse == null || (surveyValidationRepsonse != null && !surveyValidationRepsonse.Succeeded)))
                    {
                        model.ShowError = true;
                        model.ErrorMessage = surveyValidationRepsonse.Message;
                    }
                }

                #endregion [validate survey Notification No]

                if (!model.ShowError)
                {
                    model = GetQuestionAndAns(surveyDetail.Value, surveyDetail.Text);
                    model.SurveyType = request.s;
                    model.SurveyNo = request.n;
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return View("~/Views/Feature/SupplyManagement/Consumption/Survey.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Survey(_CC_Model.SurveyPostModel model)
        {
            if (!string.IsNullOrEmpty(model.n) && !string.IsNullOrEmpty(model.s))
            {
                var surveyDetail = GetDataSource(SitecoreItemIdentifiers.DTMC_SURVEY_DATALIST_SCPATH, model.s).FirstOrDefault();
                if (surveyDetail != null)
                {
                    var reponseData = ConsultantServiceClient.PostSurveyAnswers(new DEWAXP.Foundation.Integration.SmartConsultantSvc.PostSurveyAnswers()
                    {
                        surveyDataList = model?.SurveyDatas?.ToArray() ?? null,
                        surveyid = surveyDetail.Value,
                        surveytype = surveyDetail.Text,
                        transaction = model.n
                    }, RequestLanguage, Request.Segment());

                    if (reponseData == null || (reponseData != null && !reponseData.Succeeded))
                    {
                        model.ErrorMessage = reponseData?.Message;
                    }
                }
                else
                {
                    model.ErrorMessage = Translate.Text("DT_survey_InvalidSurvey");
                }
            }
            else
            {
                model.ErrorMessage = Translate.Text("DT_Survey_fieldsMiss", model.s, model.n, Convert.ToInt32(model.SurveyDatas?.Count() ?? 0));
            }

            return Json(model, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult Inquries(_CC_Model.SurveyRequestModel request)
        {
            _CC_Model.SurveyInfoDetail model = new _CC_Model.SurveyInfoDetail();
            try
            {
                #region [get Survey Detail config]

                /*get Suvey Id and Suvery Appliction Id by Sitecore Item*/
                var surveyDetail = GetDataSource(SitecoreItemIdentifiers.DTMC_SURVEY_DATALIST_SCPATH, request.s).FirstOrDefault();
                model.ShowError = (surveyDetail == null);

                #endregion [get Survey Detail config]

                #region [validate survey Notification No]

                if (!model.ShowError && request.vd == "1")
                {
                    /*Survey Id validation*/
                    var surveyValidationRepsonse = ConsultantServiceClient.ValidateSurvey(new DEWAXP.Foundation.Integration.SmartConsultantSvc.ValidateSurvey()
                    {
                        transaction = request.n
                    }, RequestLanguage, Request.Segment());

                    if (string.IsNullOrWhiteSpace(request.s) || string.IsNullOrWhiteSpace(request.n) ||
                                      Convert.ToBoolean(surveyValidationRepsonse == null || (surveyValidationRepsonse != null && !surveyValidationRepsonse.Succeeded)))
                    {
                        model.ShowError = true;
                        model.ErrorMessage = surveyValidationRepsonse.Message;
                    }
                }

                #endregion [validate survey Notification No]

                if (!model.ShowError)
                {
                    model = GetInquiriesQuestionAndAns(surveyDetail.Value, surveyDetail.Text);
                    model.SurveyType = request.s;
                    model.SurveyNo = request.n;
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return View("~/Views/Feature/SupplyManagement/Consumption/Inquries.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Inquries(_CC_Model.SurveyPostModel model)
        {
            if (!string.IsNullOrEmpty(model.n) && !string.IsNullOrEmpty(model.s))
            {
                var surveyDetail = GetDataSource(SitecoreItemIdentifiers.DTMC_SURVEY_DATALIST_SCPATH, model.s).FirstOrDefault();
                if (surveyDetail != null)
                {
                    var reponseData = ConsultantServiceClient.PostSurveyAnswers(new DEWAXP.Foundation.Integration.SmartConsultantSvc.PostSurveyAnswers()
                    {
                        surveyDataList = model?.SurveyDatas?.ToArray() ?? null,
                        surveyid = surveyDetail.Value,
                        surveytype = surveyDetail.Text,
                        transaction = model.n
                    }, RequestLanguage, Request.Segment());
                    model.IsError = (reponseData == null || (reponseData != null && !reponseData.Succeeded));
                    if (reponseData != null)
                    {
                        model.ErrorMessage = reponseData?.Message;
                    }
                }
                else
                {
                    model.ErrorMessage = Translate.Text("DT_survey_InvalidSurvey");
                }
            }
            else
            {
                model.ErrorMessage = Translate.Text("DT_Survey_fieldsMiss", model.s, model.n, Convert.ToInt32(model.SurveyDatas?.Count() ?? 0));
            }

            return Json(model, JsonRequestBehavior.DenyGet);
        }

        #endregion [Survey Actions]

        #region Functions

        public _CC_Model.CommonRender SR_ActionHelper(_CC_Model.CommonRender data)
        {
            var IsStopAction = Convert.ToBoolean(data != null && Convert.ToBoolean(data.CurrentRequest?.IsPageBack) || (data.CurrentRequest.IsPageRefesh));
            _CC_Model.CustomerSubmittedData smDetail = _CC_CommonHelper.GetUserSubmittedData();
            bool IsSessionExit = IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User);
            var activeUser = AuthStateService.GetActiveProfile();

            string _otpCount = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.OtpCount);
            int _Customer_OtpCount = Convert.ToInt32(!string.IsNullOrEmpty(_otpCount) ? _otpCount : "0");
            string _errorMessage = "";

            #region file variable

            byte[] image1Bytes = new byte[2];
            byte[] image2Bytes = new byte[2];
            string image1Path = "";
            string image2Path = "";
            var filePath = Server.MapPath(ConsumptionComplaintResponseCofig.CC_RESPONSE_UPLOADPATH);
            var consumptionTypeString = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);

            #endregion file variable

            _CC_Model.Question filteredQus = null;
            if (data != null && data.Answer != null)
            {
                switch (data.Answer.Action)
                {
                    case _CC_Model.SM_Action.Call://extra
                        break;

                    case _CC_Model.SM_Action.Checklogin:
                        //handle screen by session

                        #region [handle screen by session]

                        if (data.Answer.Questions.Where(x => !string.IsNullOrEmpty(x.Code.ToString())).Count() > 1)
                        {
                            filteredQus = null;
                            _CC_Model.SM_Code code = IsSessionExit ? _CC_Model.SM_Code.The000 : _CC_Model.SM_Code.The001;
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == code).FirstOrDefault());
                            if (filteredQus != null)
                            {
                                data.Answer.Questions = new System.Collections.Generic.List<_CC_Model.Question>();
                                data.Answer.Questions.Add(filteredQus);
                            }
                        }

                        if (IsLoggedIn && IsSessionExit)
                        {
                            if (string.IsNullOrEmpty(smDetail.ContactPersonName))
                            {
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ContactPersonName, GetContactNameByUserSession());
                            }

                            if (string.IsNullOrEmpty(smDetail.Mobile))
                            {
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Mobile, activeUser.MobileNumber);
                            }
                        }

                        #endregion [handle screen by session]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case _CC_Model.SM_Action.Empty:
                        break;

                    case _CC_Model.SM_Action.Verifyotp:

                        if (IsStopAction || ConsumptionComplaintResponseCofig.CC_DISBABLE_LOGIC)
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
                                    mobile = _CC_CommonHelper.MobileTenFormat(smDetail.Mobile),
                                    otp = smDetail.MobileOtp,
                                    servicetype = "Y7",
                                }
                            }, RequestLanguage, Request.Segment());
                            data.IsError = !(r1 != null && r1.Succeeded);

                            if (data.IsError)
                            {
                                data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail() { ControlId = _CC_Model.SM_Id.Otp });
                            }
                        }

                        #endregion [VerifyOTP]

                        break;

                    case _CC_Model.SM_Action.Getotpmobile:
                    case _CC_Model.SM_Action.Resendotpmobile:

                        try
                        {
                            if (IsStopAction || ConsumptionComplaintResponseCofig.CC_DISBABLE_LOGIC)
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
                            //            mobile = _CC_CommonHelper.MobileTenFormat(smDetail.Mobile), // masking
                            //            servicetype = "Y7",
                            //        }
                            //    }, RequestLanguage, Request.Segment());
                            //    OtpSuccess = (r != null && r.Succeeded);
                            //    _errorMessage = r.Message;
                            //    LogService.Debug(_errorMessage);
                            //    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.OtpCount, Convert.ToString(_Customer_OtpCount + 1));
                            //}
                            //data.IsError = !OtpSuccess;

                            //if (data.IsError)
                            //{
                            //    if (_Customer_OtpCount > 3)
                            //    {
                            //        data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                            //        {
                            //            ControlId = _CC_Model.SM_Id.OtpCount,
                            //            ErorrMessage = _errorMessage
                            //        });
                            //    }
                            //    else
                            //    {
                            //        data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                            //        {
                            //            ControlId = _CC_Model.SM_Id.Mobile,
                            //            ErorrMessage = _errorMessage
                            //        });
                            //    }
                            //}

                            #endregion [Disabled OTP count logic]

                            var r = DewaApiClient.SendNotificationOTP(new DEWAXP.Foundation.Integration.DewaSvc.SendNotificationOTP()
                            {
                                otpinput = new DEWAXP.Foundation.Integration.DewaSvc.notificationOTPInput()
                                {
                                    mobile = _CC_CommonHelper.MobileTenFormat(smDetail.Mobile), // masking
                                    servicetype = "Y7",
                                }
                            }, RequestLanguage, Request.Segment());
                            data.IsError = !(r != null && r.Succeeded);
                            LogService.Debug(r.Message);
                            if (data.IsError)
                            {
                                data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                                {
                                    ControlId = _CC_Model.SM_Id.Mobile,
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

                    case _CC_Model.SM_Action.Submit:
                        string complaintId = null;
                        if (IsStopAction)
                        {
                            break;
                        }

                        #region [Consumption Submit]

                        if (!string.IsNullOrWhiteSpace(consumptionTypeString))
                        {
                            if (!string.IsNullOrEmpty(smDetail.ContractAccountNo) && IsLoggedIn && IsSessionExit)
                            {
                                var acDetails = _CC_SessionHelper.CC_GetConsumptionDetail?.accountslist.FirstOrDefault();

                                var acRequest = new DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn()
                                {
                                    contractaccountnumber = smDetail.ContractAccountNo,
                                    billingMonth = smDetail.BillingMonth,
                                    email = smDetail.Email,
                                    dateofnotification = smDetail.ScheduleDate,
                                    timeofnotification = smDetail.ScheduleTime,
                                    mobile = smDetail.Mobile,
                                    highlowconsumption = smDetail.ComplaintType,
                                    businesspartnernumber = acDetails?.businesspartnernumber,
                                    calltype = smDetail.CallType
                                };
                                if (smDetail.ConsumptionType == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                                {
                                    acRequest.electricityContract = acDetails?.electricitycontract;
                                }
                                if (smDetail.ConsumptionType == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                                {
                                    acRequest.waterContract = acDetails?.watercontract;
                                }

                                List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn> acList = new List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn>();
                                acList.Add(acRequest);

                                var srequest = new DEWAXP.Foundation.Integration.DewaSvc.SetNotificationSubmit()
                                {
                                    NotificationInfoIn = new DEWAXP.Foundation.Integration.DewaSvc.notificationInfoIn()
                                    {
                                        accountlist = acList.ToArray(),
                                        executionflag = "W",
                                        journeystepnumber = "XX",
                                        notificationtype = smDetail.ConsumptionType,
                                        email = smDetail.Email,
                                        mobile = smDetail.Mobile,
                                        disconnectiondate = smDetail.ScheduleDate,
                                        disconnectiontime = smDetail.ScheduleTime,
                                        remarks = smDetail.Remark
                                    },
                                };

                                HttpPostedFileBase imageData = null;
                                byte[] fileBytes = null;

                                if (CacheProvider.TryGet<HttpPostedFileBase>(_UploadedFile + _CC_Model.SM_Id.Image1, out imageData) &&
                                    CacheProvider.TryGet<byte[]>(_UploadedFile + "bytes", out fileBytes))
                                {
                                    srequest.NotificationInfoIn.attachment = fileBytes;
                                    srequest.NotificationInfoIn.attachmenttype = imageData.FileName.GetFileExtensionTrimmed()?.ToLower();
                                }

                                //Get All bill Consumption testing
                                var ConsumptionSubmissionReponse = DewaApiClient.SetNotificationSubmit(srequest, "C", activeUser.UserId, activeUser.SessionToken, RequestLanguage, Request.Segment());

                                if (ConsumptionSubmissionReponse != null && ConsumptionSubmissionReponse.Succeeded &&
                                    ConsumptionSubmissionReponse.Payload?.@return != null && ConsumptionSubmissionReponse.Payload.@return.notificationlist != null)
                                {
                                    complaintId = ConsumptionSubmissionReponse.Payload.@return.notificationlist.FirstOrDefault()?.notificationnumber;
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ComplaintId, complaintId);
                                    //if (!string.IsNullOrWhiteSpace(complaintId))
                                    //{
                                    //    HttpPostedFileBase imageData = null;
                                    //    byte[] fileBytes = null;

                                    //    if (CacheProvider.TryGet<HttpPostedFileBase>(_UploadedFile + _CC_Model.SM_Id.Image1, out imageData) &&
                                    //        CacheProvider.TryGet<byte[]>(_UploadedFile + "bytes", out fileBytes))
                                    //    {
                                    //        var UploadResponse = DewaApiClient.SendMoveInAttachment(activeUser.UserId, activeUser.SessionToken, complaintId, null,
                                    //               imageData.FileName, imageData.ContentType, fileBytes, RequestLanguage, Request.Segment()
                                    //           );
                                    //    }

                                    //}
                                }
                                data.IsError = string.IsNullOrEmpty(complaintId);
                                if (!data.IsError)
                                {
                                    CacheProvider.Remove(_UploadedFile + _CC_Model.SM_Id.Image1);
                                    CacheProvider.Remove(_UploadedFile + "bytes");
                                }
                                if (data.IsError)
                                {
                                    data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                                    {
                                        ControlId = _CC_Model.SM_Id.ComplaintId,
                                        ErorrMessage = ConsumptionSubmissionReponse.Message
                                    });
                                }
                            }

                            break;
                        }

                        #endregion [Consumption Submit]

                        //TODO:SM-- need to complete Submission Flow. --DONE

                        #region [Submit]

                        _CC_Model.SM_SessionType _type = _CC_CommonHelper.Get_CC_SessionType();

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

                        #region [Submission Handling]

                        if (_type == _CC_Model.SM_SessionType.IsLoggedIn)
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
                                    mobile = _CC_CommonHelper.MobileTenFormat(smDetail.Mobile),
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
                            if (_setLgCmpResponse != null && _setLgCmpResponse.Succeeded && _setLgCmpResponse.Payload != null)
                            {
                                complaintId = _setLgCmpResponse.Payload.@return.notificationnumber;
                            }
                        }
                        else if (_type == _CC_Model.SM_SessionType.IsGuest)
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
                                    mobile = _CC_CommonHelper.MobileTenFormat(smDetail.Mobile),
                                    name = smDetail.ContactPersonName,
                                    sessionid = activeUser.SessionToken,
                                    text = smDetail.MoreDescription,
                                    trcode = smDetail.TrCode,
                                    trcodegroup = smDetail.TrCodeGroup,
                                    xgps = smDetail.Latitude,
                                    ygps = smDetail.Longitude,
                                },
                            }, RequestLanguage, Request.Segment());

                            if (_setGsCmpResponse != null && _setGsCmpResponse.Succeeded && _setGsCmpResponse.Payload != null)
                            {
                                complaintId = _setGsCmpResponse.Payload.@return.notificationnumber;
                            }
                        }

                        #endregion [Submission Handling]

                        data.IsError = string.IsNullOrEmpty(complaintId);
                        if (!data.IsError)
                        {
                            DeleteFile(image1Path);
                            DeleteFile(image2Path);
                            _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ComplaintId, complaintId);
                        }
                        if (data.IsError)
                        {
                            data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                            {
                                ControlId = _CC_Model.SM_Id.ComplaintId,
                            });
                        }

                        #endregion [Submit]

                        break;

                    case _CC_Model.SM_Action.Track:
                        //data.RedirectUrl = string.Format("{0}?a={1}&o=CV", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J22_TRACK_COMPLAINTS), smDetail.ContractAccountNo);
                        break;

                    case _CC_Model.SM_Action.UploadMedia:
                        if (!string.IsNullOrWhiteSpace(consumptionTypeString))
                        {
                            //HttpPostedFileBase imageData = null;
                            //HttpPostedFileBase tempImageData = null;

                            //if (CacheProvider.TryGet<HttpPostedFileBase>(_UploadedFile + data.Answer.Id, out imageData))
                            //{
                            //    tempImageData = imageData;
                            //    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(data.Answer.Id, imageData != null ? "data:image;base64," + Convert.ToBase64String(tempImageData.ToArray()) : "/images/preview@2x.png");
                            //    //CacheProvider.Store(_UploadedFile + data.Answer.Id, new CacheItem<HttpPostedFileBase>(tempImageData, TimeSpan.FromHours(30)));
                            //}
                            //else
                            //{
                            //    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(data.Answer.Id, "");
                            //}

                            HttpPostedFileBase imageData = null;
                            byte[] fileBytes = null;
                            if (CacheProvider.TryGet<HttpPostedFileBase>(_UploadedFile + data.Answer.Id, out imageData))
                            {
                                if (!CacheProvider.TryGet<byte[]>(_UploadedFile + "bytes", out fileBytes))
                                {
                                    fileBytes = imageData.ToArray();
                                    CacheProvider.Store(_UploadedFile + "bytes", new CacheItem<byte[]>(fileBytes, TimeSpan.FromHours(30)));
                                }

                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(data.Answer.Id, imageData != null ? "data:image;base64," + Convert.ToBase64String(fileBytes) : "/images/preview@2x.png");
                            }
                            else
                            {
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(data.Answer.Id, "");
                            }
                        }
                        break;

                    case _CC_Model.SM_Action.SubmitRequestFire://extra
                        break;

                    case _CC_Model.SM_Action.BillPayment://extra
                        break;

                    case _CC_Model.SM_Action.SubmitRequestSpark://extra
                        break;

                    case _CC_Model.SM_Action.SubmitRequestSmoke://extra
                        break;

                    case _CC_Model.SM_Action.SubmitRequestEfluctuation://extra
                        break;

                    case _CC_Model.SM_Action.CheckMeterStatus:

                        break;

                    case _CC_Model.SM_Action.SubMeterCheck:
                        //TODO:SM-- need to implement Sub Meter Check --DONE
                        bool IsSubMeter = smDetail.ElectricityMeterType == _CC_Model.CommonConst.IsSubMeter;

                        if (data.Answer.Questions.Where(x => !string.IsNullOrEmpty(x.Code.ToString())).Count() > 1)
                        {
                            filteredQus = null;
                            _CC_Model.SM_Code code = IsSessionExit && IsSubMeter ? _CC_Model.SM_Code.The000 : _CC_Model.SM_Code.The001;
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == code).FirstOrDefault());
                            if (filteredQus != null)
                            {
                                data.Answer.Questions = new System.Collections.Generic.List<_CC_Model.Question>();
                                data.Answer.Questions.Add(filteredQus);
                            }
                        }

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case _CC_Model.SM_Action.Paybill:
                        break;

                    case _CC_Model.SM_Action.Customerlogin:
                        //setting redirect url: --DONE
                        var loginUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                        data.RedirectUrl = string.Format(loginUrl + "?returnUrl={0}", HttpUtility.UrlEncode(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J24_BILLING_COMPLAINT) + "?l=" + smDetail.LangType.ToString()));

                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.SessionType, "1");//seting its customer Type login.
                        break;

                    case _CC_Model.SM_Action.Accountcountcheck:
                    case _CC_Model.SM_Action.ACCOUNTCOUNTCHECK_TRACK:
                        //TODO:SM-- Need to Add Account Count Service COntroller : DONE
                        bool isTrack = (data.Answer.Action == _CC_Model.SM_Action.ACCOUNTCOUNTCHECK_TRACK);

                        if (isTrack && !string.IsNullOrEmpty(smDetail.NotificationNumber) && !string.IsNullOrWhiteSpace(smDetail.ContractAccountNo))
                        {
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The001).FirstOrDefault());
                            data.Answer.Questions = new List<_CC_Model.Question>();
                            data.Answer.Questions.Add(filteredQus);

                            //setting to tracking page.
                            data.Question = data.Answer.Questions.FirstOrDefault();
                            data.Answer = data.Answer.Questions.FirstOrDefault().Answers.FirstOrDefault();
                            data.CurrentRequest.IsForceTrack = true;
                            data.CurrentRequest.IsAnsAltered = true;
                            break;
                        }

                        int AccountCount = 0;

                        var accountList = GetBillingAccounts(false, true, "");
                        AccountCount = Convert.ToInt32(accountList.Succeeded && accountList?.Payload != null ? accountList?.Payload.Count() : 0);

                        #region [handle screen by Account Count]

                        if (data.Answer.Questions.Where(x => !string.IsNullOrEmpty(x.Code.ToString())).Count() > 1)
                        {
                            filteredQus = null;
                            if (AccountCount == 1)
                            {
                                // next step
                                filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The000).FirstOrDefault());
                                var dAccount = accountList.Payload.FirstOrDefault();
                                string accNo = dAccount.AccountNumber;
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Account, activeUser.HasPrimaryAccount ? activeUser.PrimaryAccount : accNo);

                                string personName = GetContactNameByAccountDetails(dAccount);
                                if (dAccount != null && string.IsNullOrWhiteSpace(personName))
                                {
                                    personName = GetContactNameByUserSession();
                                }
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ContactPersonName, personName);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Mobile, activeUser.MobileNumber);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.CA_Location, dAccount.Location);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Street, dAccount.Street);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Location, dAccount.Street);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.BPNumber, dAccount.BusinessPartnerNumber);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.CustomerPremiseNo, dAccount.CustomerPremiseNumber);
                            }

                            if (AccountCount > 1)
                            {
                                filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The001).FirstOrDefault());
                                data.RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J24_BILLING_COMPLAINT)}?asc=1";
                            }

                            if (AccountCount < 1)
                            {
                                //data.Answer.Questions = new System.Collections.Generic.List<_CC_Model.Question>();
                                data.Answer.Questions.Add(new _CC_Model.Question()
                                {
                                    Value = _CC_CommonHelper.GetSMTranslation("No Account Exist"),
                                    Id = _CC_Model.SM_Id.Error
                                });
                            }

                            if (filteredQus != null)
                            {
                                data.Answer.Questions = new List<_CC_Model.Question>();
                                data.Answer.Questions.Add(filteredQus);

                                data.CurrentRequest.IsAnsAltered = true;
                            }
                        }

                        #endregion [handle screen by Account Count]

                        if (isTrack)
                        {
                            break;
                        }

                        #region [consumption step10 single account]

                        if (!string.IsNullOrEmpty(_CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Account)) && AccountCount == 1)
                        {
                            smDetail.ContractAccountNo = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Account);
                            List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn> acList = new List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn>();
                            acList.Add(new DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn()
                            {
                                contractaccountnumber = smDetail.ContractAccountNo,
                            });

                            var srequest = new DEWAXP.Foundation.Integration.DewaSvc.SetNotificationSubmit()
                            {
                                NotificationInfoIn = new DEWAXP.Foundation.Integration.DewaSvc.notificationInfoIn()
                                {
                                    accountlist = acList.ToArray(),
                                    executionflag = "R",
                                    journeystepnumber = "10",
                                },
                            };

                            //Get All bill Consumption testing
                            var ConsumptionReponse = DewaApiClient.SetNotificationSubmit(srequest, "C", activeUser.UserId, activeUser.SessionToken, RequestLanguage, Request.Segment());
                            bool isConsumptionAvaible = false;
                            if (ConsumptionReponse != null && ConsumptionReponse.Succeeded && ConsumptionReponse.Payload?.@return != null)
                            {
                                var consumpData = ConsumptionReponse.Payload.@return;
                                _CC_SessionHelper.CC_GetConsumptionDetail = consumpData;

                                var ac = ConsumptionReponse.Payload.@return.accountslist.FirstOrDefault();

                                if (ac != null)
                                {
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ElectricityConsumptionCA, ac.electricitycontract);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WaterConsumptionCA, ac.watercontract);

                                    //user Detail
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ContactPersonName, ac.firstname + " " + ac.lastname);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Email, ac.email);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Mobile, ac.mobile);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.IsNewCustomer, ac.newcustomer);

                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.amie, ac.amielectricity);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.amiw, ac.amiwater);

                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.EMeterNo, ac.electricitymeternumber);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.EMeterInstallDate, ac.electricitymeterinstallationdate);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WMeterNo, ac.watermeternumber);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WMeterInstallDate, ac.watermeterinstallationdate);

                                    data.Answer.Questions.FirstOrDefault().Answers[0].disabled = string.IsNullOrWhiteSpace(ac.electricitycontract);
                                    data.Answer.Questions.FirstOrDefault().Answers[1].disabled = string.IsNullOrWhiteSpace(ac.watercontract);

                                    isConsumptionAvaible = !(consumpData != null && ((string.IsNullOrWhiteSpace(ac.electricitycontract) && string.IsNullOrWhiteSpace(ac.watercontract)) || consumpData.consumptionlist == null));
                                    if (isConsumptionAvaible && !string.IsNullOrWhiteSpace(smDetail.ConsumptionType))
                                    {
                                        isConsumptionAvaible = GetBillingMonthAndAmount() != null;
                                    }
                                }
                                //List<_CC_Model.Answer> filteredAns = new List<_CC_Model.Answer>();
                            }
                            else
                            {
                                _CC_SessionHelper.CC_GetConsumptionDetail = null;
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ElectricityConsumptionCA, null);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WaterConsumptionCA, null);
                            }

                            if (!isConsumptionAvaible)
                            {
                                data.IsError = true;
                                data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                                {
                                    ControlId = _CC_Model.SM_Id.Empty,
                                    ErorrMessage = _CC_CommonHelper.GetSMTranslation(Translate.Text("DT_ConsumptionValidationMsg")).Replace("xxxxx", smDetail.ContractAccountNo)
                                });
                            }
                        }

                        #endregion [consumption step10 single account]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case _CC_Model.SM_Action.Checkpod:

                        #region [Checkpod]

                        _CC_Model.SM_Code checkpodCode = _CC_Model.SM_Code.The000;
                        if (data.Answer.Questions.Where(x => x.Code != _CC_Model.SM_Code.Empty).Count() > 1 && !string.IsNullOrEmpty(smDetail.ContractAccountNo))
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
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.customercategory, _CC_Model.CommonConst.CusSubTyp_POD);
                                }
                                else
                                  if (POD_repsonse.Payload.seniorCitizen)
                                {
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.customercategory, _CC_Model.CommonConst.CusSubTyp_ElderPeople);
                                }
                            }

                            checkpodCode = IsPod ? _CC_Model.SM_Code.Pod : _CC_Model.SM_Code.The000;
                        }
                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == checkpodCode).FirstOrDefault());
                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new System.Collections.Generic.List<_CC_Model.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }

                        #endregion [Checkpod]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case _CC_Model.SM_Action.NoPower:
                    case _CC_Model.SM_Action.NopowerPod:
                    case _CC_Model.SM_Action.NoWater:
                    case _CC_Model.SM_Action.NoWaterPOD:

                        #region [NoPower,NopowerPod]

                        _CC_Model.SM_Code _currentCOde = _CC_Model.SM_Code.The005;
                        _CC_Model.SM_Code _defaultCOde = _CC_Model.SM_Code.The005;

                        if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == _CC_Model.CommonConst.PARENT_INCIDENT_WATER))
                        {
                            _currentCOde = _CC_Model.SM_Code.The004;
                            _defaultCOde = _CC_Model.SM_Code.The004;
                        }

                        string easyPayUrl = "";
                        string _dueAmount = "0.00";
                        string _startDateText = "";
                        string _endDateText = "";
                        bool isDisconnection = false;
                        string DisconnectionReason = "";
                        if (data.Answer.Questions.Where(x => x.Code != _CC_Model.SM_Code.Empty).Count() > 1 && !string.IsNullOrEmpty(smDetail.ContractAccountNo))
                        {
                            //only POD
                            bool IsPod = false;

                            var PODIsExist = (data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.Pod).Count() > 0);
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
                                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.customercategory, _CC_Model.CommonConst.CusSubTyp_POD);
                                    }
                                    else
                                    if (POD_repsonse.Payload.seniorCitizen)
                                    {
                                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.customercategory, _CC_Model.CommonConst.CusSubTyp_ElderPeople);
                                    }
                                }
                            }

                            if (IsPod)
                            {
                                _currentCOde = _CC_Model.SM_Code.Pod;
                                //filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code ==_CC_Model.SM_Code.Pod).FirstOrDefault());
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
                                    }
                                }, RequestLanguage, Request.Segment());

                                if (_issueRepsonse.Succeeded && _issueRepsonse.Payload != null)
                                {
                                    var _responseData = _issueRepsonse.Payload;

                                    #region [Smarter & logic]

                                    //electricity
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ElectricityMeterType, _responseData.meter.electricitymeterType);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ElectricityIsSmartMeter, Convert.ToString(_responseData.meter.electricitySmartMeter));
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ElectricityMeterNo, Convert.ToString(_responseData.meter.electricityMeter));

                                    //water
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WaterMeterType, _responseData.meter.watermeterType);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WaterIsSmartMeter, Convert.ToString(_responseData.meter.waterSmartMeter));
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WaterMeterNo, Convert.ToString(_responseData.meter.waterMeter));

                                    #endregion [Smarter & logic]

                                    #region [Bussinesss Logic Info]

                                    // payement Screen :_CC_Model.SM_Code.The000
                                    //if Electricity && electicityActive == false && resone is non payment
                                    //000
                                    if (Convert.ToBoolean(_responseData.meter.disconnectionReasonCode == "NP-DC")) //TODO:SM-- clearity Needed non payment  reason code
                                    {
                                        _currentCOde = _CC_Model.SM_Code.The000;
                                        //need to create payment Url & redirect it on the same.
                                        easyPayUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EasyPay_Enquire)}?ac=" + smDetail.ContractAccountNo;
                                        _dueAmount = _responseData.meter.reconnectionDueamount;
                                    }

                                    if (Convert.ToBoolean(_responseData.dubaiMunicipality?.dmfine) || Convert.ToBoolean(_responseData.meter?.disconnectionReasonCode == "DM-DC") && _currentCOde == _defaultCOde)
                                    {
                                        _currentCOde = _CC_Model.SM_Code.The002;
                                    }

                                    if (!string.IsNullOrWhiteSpace(_responseData.meter?.disconnectionReasonCode) && _currentCOde == _defaultCOde)
                                    {
                                        //TODO:SM-- Need to show message to the User for Disconnection and user should not
                                        isDisconnection = true;
                                        _currentCOde = _CC_Model.SM_Code.Empty;
                                        DisconnectionReason = _responseData.meter?.disconnectionAlert;
                                    }

                                    #region [Electircity Case]

                                    if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == _CC_Model.CommonConst.PARENT_INCIDENT_ELECTRICITY))
                                    {
                                        #region [isOutage]

                                        if (_currentCOde == _defaultCOde)
                                        {
                                            var _outageinfo = _responseData.outageStatus;

                                            var plannedElectricityOutage = (_outageinfo.plannedElectricityOutage ?? "").ToUpper();
                                            if (plannedElectricityOutage != _CC_Model.CommonConst.NOOUTAGE)
                                            {
                                                if (plannedElectricityOutage == _CC_Model.CommonConst.PLANNED)
                                                {
                                                    DateTime _startDate = DateTime.MinValue;
                                                    DateTime _endDate = DateTime.MinValue;
                                                    if (DateTime.TryParseExact($"{_outageinfo.electricityStartDate} {_outageinfo.electricityStartTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _startDate) &&
                                                         DateTime.TryParseExact($"{_outageinfo.electricityEndDate} {_outageinfo.electricityEndTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _endDate)) ;
                                                    {
                                                        //check currrent date is in range outage date
                                                        if (DateHelper.InRange(DateTime.Now, _startDate, _endDate))
                                                        {
                                                            _currentCOde = _CC_Model.SM_Code.The001;
                                                            //TODO:SM-- Additional Logic to Trim and reset Question
                                                            _startDateText = _CC_CommonHelper.TextHighlight(_startDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                            _endDateText = _CC_CommonHelper.TextHighlight(_endDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                        }
                                                    }
                                                    //Outage planned Screen :_CC_Model.SM_Code.The001
                                                    // check outageType = planned & DateTime Range i.e current date range
                                                }
                                                else
                                                if (plannedElectricityOutage == _CC_Model.CommonConst.UNPLANNED)
                                                {
                                                    _currentCOde = _CC_Model.SM_Code.The003;
                                                    //Outage unplanned Screen :_CC_Model.SM_Code.The003
                                                    //check outageType = unplanned
                                                }
                                            }
                                        }

                                        #endregion [isOutage]
                                    }

                                    #endregion [Electircity Case]

                                    #region [Water case]

                                    if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == _CC_Model.CommonConst.PARENT_INCIDENT_WATER))
                                    {
                                        if (_currentCOde == _defaultCOde)
                                        {
                                            #region [isOutage]

                                            var _outageinfo = _responseData.outageStatus;

                                            var plannedwaterOutage = (_outageinfo.plannedwaterOutage ?? "").ToUpper();
                                            if (plannedwaterOutage != _CC_Model.CommonConst.NOOUTAGE)
                                            {
                                                if (plannedwaterOutage == _CC_Model.CommonConst.PLANNED)
                                                {
                                                    DateTime _startDate = DateTime.MinValue;
                                                    DateTime _endDate = DateTime.MinValue;
                                                    if (DateTime.TryParseExact($"{_outageinfo.waterStartDate} {_outageinfo.waterStartTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _startDate) &&
                                                         DateTime.TryParseExact($"{_outageinfo.waterEndDate} {_outageinfo.waterEndTime}", _commonUtility.DF_yyyy_MM_dd_HHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out _endDate))
                                                    {
                                                        //check currrent date is in range outage date
                                                        if (DateHelper.InRange(DateTime.Now, _startDate, _endDate))
                                                        {
                                                            _currentCOde = _CC_Model.SM_Code.The001;
                                                            //TODO:SM-- Additional Logic to Trim and reset Question
                                                            _startDateText = _CC_CommonHelper.TextHighlight(_startDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                            _endDateText = _CC_CommonHelper.TextHighlight(_endDate.ToString(_commonUtility.DF_dd_MM_yyyy_hhmmtt));
                                                        }
                                                    }
                                                    //Outage planned Screen :_CC_Model.SM_Code.The001
                                                    // check outageType = planned & DateTime Range i.e current date range
                                                }
                                                else
                                                if (plannedwaterOutage == _CC_Model.CommonConst.UNPLANNED)
                                                {
                                                    _currentCOde = _CC_Model.SM_Code.The003;
                                                    //Outage unplanned Screen :_CC_Model.SM_Code.The003
                                                    //check outageType = unplanned
                                                }
                                            }

                                            #endregion [isOutage]

                                            #region isSubmeter

                                            bool IsSubMeterC = smDetail.WaterMeterType == _CC_Model.CommonConst.IsSubMeter;

                                            if (IsSubMeterC)
                                            {
                                                _currentCOde = _CC_Model.SM_Code.The005;
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
                                //    smDetail = _CC_CommonHelper.GetUserSubmittedData();
                                //    //after checking Type(electricy || water )checking meter status service. :_CC_Model.SM_Code.The004
                                //    //TODO:SM-- Need to implement servicec - DONE
                                //    #region [ELECTRICITY]
                                //    if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() ==_CC_Model.CommonConst.PARENT_INCIDENT_ELECTRICITY))
                                //    {
                                //        if (Convert.ToBoolean(smDetail.ElectricityIsSmartMeter) &&
                                //        !string.IsNullOrEmpty(smDetail.ElectricityMeterNo) &&
                                //        ValidateSmartMeter(smDetail.ElectricityMeterNo, "1", activeUser.SessionToken, activeUser.UserId))
                                //        {
                                //            _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.CheckSmartMeter, "1");
                                //            _currentCOde =_CC_Model.SM_Code.The004;
                                //        }

                                //    }
                                //    #endregion

                                //}
                            }
                        }
                        filteredQus = data.Answer.Questions.Where(x => x != null && x.Code == _currentCOde).FirstOrDefault();
                        data.Answer.Questions = new System.Collections.Generic.List<_CC_Model.Question>();
                        if (filteredQus != null)
                        {
                            data.Answer.Questions.Add(filteredQus);

                            #region [Data Alteration]

                            if (filteredQus.Answers.FirstOrDefault().Action == _CC_Model.SM_Action.Paybill)
                            {
                                data.Answer.Questions.FirstOrDefault().Answers.FirstOrDefault().Actiondata = easyPayUrl;
                                data.Answer.Questions.FirstOrDefault().Value = _CC_CommonHelper.GetSMTranslation(filteredQus.Value)?.Replace("{{amount}}", _CC_CommonHelper.TextHighlight(_dueAmount));
                            }

                            if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == _CC_Model.CommonConst.PARENT_INCIDENT_ELECTRICITY))
                            {
                                //planned outage:
                                if (filteredQus.Code == _CC_Model.SM_Code.The001)
                                {
                                    data.Answer.Questions.FirstOrDefault().Value = _CC_CommonHelper.GetSMTranslation(filteredQus.Value)?.Replace("{{fromXX}}", _startDateText)?.Replace("{{toXX}}", _endDateText);
                                }
                            }

                            if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == _CC_Model.CommonConst.PARENT_INCIDENT_WATER))
                            {
                                //planned outage:
                                if (filteredQus.Code == _CC_Model.SM_Code.The001)
                                {
                                    data.Answer.Questions.FirstOrDefault().Value = _CC_CommonHelper.GetSMTranslation(filteredQus.Value)?.Replace("{{fromXX}}", _startDateText)?.Replace("{{toXX}}", _endDateText);
                                }
                            }

                            #endregion [Data Alteration]
                        }
                        else if (isDisconnection)
                        {
                            data.Answer.Questions.Add(new _CC_Model.Question()
                            {
                                Value = DisconnectionReason,
                                Id = _CC_Model.SM_Id.Error,
                                Infotext = ""
                            });
                        }
                        else
                        {
                            data.Answer.Questions.Add(new _CC_Model.Question()
                            {
                                Value = Translate.Text("SM_Error_UnableToProcess"),
                                Id = _CC_Model.SM_Id.Error,
                                Infotext = ""
                            });
                        }

                        #endregion [NoPower,NopowerPod]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case _CC_Model.SM_Action.SubmetercheckPod:
                        //TODO: need to implement service.
                        //get primise detail with pod+meter= true & no smart meter check

                        #region [SubmetercheckPod]

                        _CC_Model.SM_Code _code = _CC_Model.SM_Code.The001;
                        if (data.Answer.Questions.Where(x => x.Code != _CC_Model.SM_Code.Empty).Count() > 1 && !string.IsNullOrEmpty(smDetail.ContractAccountNo))
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
                                    sessionid = activeUser.SessionToken
                                }
                            }, RequestLanguage, Request.Segment());

                            if (POD_repsonse.Succeeded && POD_repsonse.Payload != null && POD_repsonse.Payload.responseCode == "000")
                            {
                                var d = POD_repsonse.Payload;
                                IsPod = Convert.ToBoolean(d.podCustomer || d.seniorCitizen);

                                if (POD_repsonse.Payload.podCustomer)
                                {
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.customercategory, _CC_Model.CommonConst.CusSubTyp_POD);
                                }
                                else
                                  if (POD_repsonse.Payload.seniorCitizen)
                                {
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.customercategory, _CC_Model.CommonConst.CusSubTyp_ElderPeople);
                                }

                                if (IsPod)
                                {
                                    _code = _CC_Model.SM_Code.Pod;
                                }
                                if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == _CC_Model.CommonConst.PARENT_INCIDENT_ELECTRICITY))
                                {
                                    if (d.meter.electricitymeterType != _CC_Model.CommonConst.IsSubMeter)
                                    {
                                        _code = _CC_Model.SM_Code.The000;
                                    }
                                }
                                if (Convert.ToBoolean(smDetail.CodeGroup?.ToUpper() == _CC_Model.CommonConst.PARENT_INCIDENT_WATER))
                                {
                                    if (d.meter.watermeterType != _CC_Model.CommonConst.IsSubMeter)
                                    {
                                        _code = _CC_Model.SM_Code.The000;
                                    }
                                }
                            }
                        }
                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _code).FirstOrDefault());
                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new System.Collections.Generic.List<_CC_Model.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }

                        #endregion [SubmetercheckPod]

                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case _CC_Model.SM_Action.GetLocation:

                        break;

                    case _CC_Model.SM_Action.UploadMedia_ssd:

                        if (string.IsNullOrWhiteSpace(consumptionTypeString))
                        {
                            filteredQus = null;
                            _CC_Model.SM_Code _umDefault = _CC_Model.SM_Code.The001;
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
                                        var response = SmartResponseClient.GetPredict(image1Path,true);
                                        if (response != null && response.Succeeded)
                                        {
                                            r = response.Payload;
                                        }
                                        r.prev = System.IO.Path.Combine(ConsumptionComplaintResponseCofig.CC_RESPONSE_UPLOADPATH, smDetail.Image1).TrimStart('~');
                                    }

                                    //_CC_Model.PredictState r = _CC_CommonHelper.GetPredict(image1Path, smDetail.Image1);
                                    data.IsError = (r == null || r != null && !r.fuseboxDetectionFlag);//checking the response is null OR response has fuse detection
                                    if (data.IsError)
                                    {
                                        data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail() { ControlId = _CC_Model.SM_Id.UploadSSD, ErorrMessage = _CC_CommonHelper.GetSMTranslation("Oops! this photo can’t be analyzed. Please point rightly to your electricity box and take/upload an adequate photo again.") });
                                        break;
                                    }

                                    if (r != null && Convert.ToBoolean(r.boxes?.Count > 0))
                                    {
                                        DrawIssueImage(filePath, image1Path, r.boxes);
                                        _umDefault = _CC_Model.SM_Code.The000;
                                    }
                                    CacheProvider.Store(CacheKeys.SM_AI_IMG_PREDICT, new CacheItem<PredictState>(r, TimeSpan.FromHours(1)));
                                }
                            }

                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _umDefault).FirstOrDefault());
                            if (filteredQus != null)
                            {
                                data.Answer.Questions = new System.Collections.Generic.List<_CC_Model.Question>();
                                data.Answer.Questions.Add(filteredQus);
                            }
                            data.CurrentRequest.IsAnsAltered = true;
                        }
                        else
                        {
                            HttpPostedFileBase imageData = null;
                            byte[] fileBytes = new byte[0];
                            if (CacheProvider.TryGet<HttpPostedFileBase>(_UploadedFile + data.Answer.Id, out imageData))
                            {
                                if (!CacheProvider.TryGet<byte[]>(_UploadedFile + "bytes", out fileBytes))
                                {
                                    fileBytes = imageData.ToArray();
                                    CacheProvider.Store(_UploadedFile + "bytes", new CacheItem<byte[]>(fileBytes, TimeSpan.FromHours(30)));
                                }

                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(data.Answer.Id, imageData != null ? "data:image;base64," + Convert.ToBase64String(fileBytes) : "/images/preview@2x.png");
                            }
                            else
                            {
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(data.Answer.Id, "");
                            }
                        }
                        break;

                    case _CC_Model.SM_Action.ShowInterruption:
                        var InterruptionUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.InterruptionService);
                        data.RedirectUrl = InterruptionUrl;

                        break;

                    case _CC_Model.SM_Action.GETMAINTENANCEPROVIDERS:

                        DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse cmpyDetails = null;
                        if (!CacheProvider.TryGet<DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse>($"Get_CC_CompanyDetailsResponse_{RequestLanguage}", out cmpyDetails))
                        {
                            var d = DewaApiClient.GetSRCompanyDetails(new DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetails() { CompaniesInput = new DEWAXP.Foundation.Integration.DewaSvc.companiesInput() }, RequestLanguage, Request.Segment());
                            if (d != null && d.Payload != null)
                            {
                                cmpyDetails = d.Payload;
                                CacheProvider.Store($"Get_CC_CompanyDetailsResponse_{RequestLanguage}", new CacheItem<DEWAXP.Foundation.Integration.DewaSvc.GetSRCompanyDetailsResponse>(d.Payload, TimeSpan.FromHours(1)));
                            }
                            else
                            {
                                ModelState.AddModelError("", d.Message);
                            }
                        }
                        data.SRCompanyDetails = cmpyDetails;

                        break;
                    //Removed by team.
                    //case _CC_Model.SM_Action.GETBILLINGMONTHS:
                    //TODO-27062020- Apply Dynaimc Logic for Water and Electircity
                    //break;

                    case _CC_Model.SM_Action.GETTIMESLOT:

                        #region [GETTIMESLOT]

                        List<string> shiftData = new List<string>();

                        string consumptionType = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);

                        if (!string.IsNullOrWhiteSpace(consumptionType))
                        {
                            List<DEWAXP.Foundation.Integration.DewaSvc.sInput> sInputs = new List<DEWAXP.Foundation.Integration.DewaSvc.sInput>();
                            if (consumptionType == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                            {
                                sInputs.Add(new DEWAXP.Foundation.Integration.DewaSvc.sInput()
                                {
                                    controlkey = smDetail.CallType == _CC_Model.CommonConst.CallType_CALLBACK ? "ZECSHIFT" : "ZESHIFT",
                                });
                                sInputs.Add(new DEWAXP.Foundation.Integration.DewaSvc.sInput()
                                {
                                    controlkey = "EHDLIST",
                                });
                                sInputs.Add(new DEWAXP.Foundation.Integration.DewaSvc.sInput()
                                {
                                    controlkey = "ZEDAYS",
                                });
                            }

                            if (consumptionType == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                            {
                                sInputs.Add(new DEWAXP.Foundation.Integration.DewaSvc.sInput()
                                {
                                    controlkey = smDetail.CallType == _CC_Model.CommonConst.CallType_CALLBACK ? "ZWCSHIFT" : "ZWSHIFT",
                                });
                                sInputs.Add(new DEWAXP.Foundation.Integration.DewaSvc.sInput()
                                {
                                    controlkey = "WHDLIST",
                                });
                                sInputs.Add(new DEWAXP.Foundation.Integration.DewaSvc.sInput()
                                {
                                    controlkey = "ZWDAYS",
                                });
                            }

                            var shiftHolidayRespone = DewaApiClient.GetShiftHolidayList(new DEWAXP.Foundation.Integration.DewaSvc.GetShiftHolidayList()
                            {
                                ShiftHolidayList = new DEWAXP.Foundation.Integration.DewaSvc.shiftHolidaysIn()
                                {
                                    additionalinput = "",
                                    sinputIns = sInputs.ToArray()
                                },
                            }, activeUser.UserId, activeUser.SessionToken, RequestLanguage, Request.Segment());

                            if (shiftHolidayRespone != null && shiftHolidayRespone.Succeeded && shiftHolidayRespone.Payload.@return.shiftOuts != null)
                            {
                                data.CurrentRequest.ShiftTimeList = GetShiftTimeList(shiftHolidayRespone.Payload.@return.shiftOuts, sInputs.ToArray());
                                data.CurrentRequest.ShiftTimeListJson = JsonConvert.SerializeObject(data.CurrentRequest.ShiftTimeList.Select(x => new { x.Text, x.Value })); ;
                                data.CurrentRequest.HolidayList = shiftHolidayRespone.Payload.@return.holidayOuts.Where(x => x != null)?.Select(x => Convert.ToDateTime(x.holidaydate))?.ToList();
                                data.CurrentRequest.HolidayList.Add(DateTime.Now);
                                data.CurrentRequest.HolidayListJson = string.Join(",", data.CurrentRequest.HolidayList.Select(x => x.ToString("[" + x.Year + "," + (x.Month - 1) + "," + x.Day + "]")));
                            }
                        }

                        #endregion [GETTIMESLOT]

                        break;

                    case _CC_Model.SM_Action.VALIDATETIMESLOT:

                        #region [validate time slot]

                        if (!string.IsNullOrWhiteSpace(smDetail.ConsumptionType))
                        {
                            if (!string.IsNullOrEmpty(smDetail.ContractAccountNo) && IsLoggedIn && IsSessionExit)
                            {
                                var acDetails = _CC_SessionHelper.CC_GetConsumptionDetail?.accountslist.FirstOrDefault();

                                var acRequest = new DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn()
                                {
                                    contractaccountnumber = smDetail.ContractAccountNo,
                                    billingMonth = smDetail.BillingMonth,
                                    //email = smDetail.Email,
                                    dateofnotification = smDetail.ScheduleDate,
                                    timeofnotification = smDetail.ScheduleTime,
                                    //mobile = smDetail.Mobile,
                                    highlowconsumption = smDetail.ComplaintType,
                                    businesspartnernumber = acDetails?.businesspartnernumber,
                                    calltype = smDetail.CallType
                                };
                                if (smDetail.ConsumptionType == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                                {
                                    acRequest.electricityContract = acDetails?.electricitycontract;
                                }
                                if (smDetail.ConsumptionType == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                                {
                                    acRequest.waterContract = acDetails?.watercontract;
                                }

                                List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn> acList = new List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn>();
                                acList.Add(acRequest);

                                var srequest = new DEWAXP.Foundation.Integration.DewaSvc.SetNotificationSubmit()
                                {
                                    NotificationInfoIn = new DEWAXP.Foundation.Integration.DewaSvc.notificationInfoIn()
                                    {
                                        accountlist = acList.ToArray(),
                                        executionflag = "R",
                                        journeystepnumber = "50",
                                        notificationtype = smDetail.ConsumptionType,
                                        //email = smDetail.Email,
                                        //mobile = smDetail.Mobile,
                                        disconnectiondate = smDetail.ScheduleDate,
                                        disconnectiontime = smDetail.ScheduleTime,
                                    },
                                };

                                //Get All bill Consumption testing
                                var ConsumptionSubmissionReponse = DewaApiClient.SetNotificationSubmit(srequest, "C", activeUser.UserId, activeUser.SessionToken, RequestLanguage, Request.Segment());

                                if (!(ConsumptionSubmissionReponse != null && ConsumptionSubmissionReponse.Succeeded))
                                {
                                    data.IsError = true;
                                    data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                                    {
                                        ControlId = _CC_Model.SM_Id.DateOfNotification,
                                        ErorrMessage = ConsumptionSubmissionReponse.Message ?? _CC_CommonHelper.GetSMTranslation("Choose different date time")
                                    });
                                }
                                else
                                {
                                    string chargeAmount = ConsumptionSubmissionReponse.Payload.@return.accountslist?.FirstOrDefault()?.chargeamount ?? "0";
                                    _CC_SessionHelper.CC_GetConsumptionDetail.accountslist.FirstOrDefault().chargeamount = chargeAmount;
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.chargeAmount, chargeAmount);
                                }
                            }

                            break;
                        }

                        #endregion [validate time slot]

                        break;

                    case _CC_Model.SM_Action.SETSMARTALERT:
                        data.RedirectUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SMARTRESPONSEPAGE);
                        break;

                    case _CC_Model.SM_Action.SHOWDEWASTORE:
                        data.RedirectUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DEWASTOREPAGE);
                        break;

                    case _CC_Model.SM_Action.SHOWSAVINGPLAN:
                        data.RedirectUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.MYSAVINGSTIPSPAGE);
                        break;

                    case _CC_Model.SM_Action.CHECKCONNECTIONNEW:

                        filteredQus = null;
                        if (smDetail.IsNewCustomer)
                        {
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The000).FirstOrDefault());
                        }
                        else
                        {
                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The001).FirstOrDefault());
                        }

                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new List<_CC_Model.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }
                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    case _CC_Model.SM_Action.GetNotificationList:

                        if (!string.IsNullOrWhiteSpace(smDetail.NotificationNumber))
                        {
                            data.Answer.ParentTrackingId = 0;
                        }
                        //data.RedirectUrl = string.Format("{0}?o=CV", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J22_TRACK_COMPLAINTS));
                        break;

                    case _CC_Model.SM_Action.E_REQ_EXISTS:
                    case _CC_Model.SM_Action.W_REQ_EXISTS:

                        #region [Check notfication Exited Already]

                        filteredQus = null;
                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ComplaintId, null);
                        if (_CC_SessionHelper.CC_GetConsumptionDetail != null)
                        {
                            var ac = _CC_SessionHelper.CC_GetConsumptionDetail.accountslist.FirstOrDefault();
                            string type = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);
                            if (ac != null && !string.IsNullOrWhiteSpace(type))
                            {
                                if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                                {
                                    if (string.IsNullOrWhiteSpace(ac.electricityDuplicateNotification))
                                    {
                                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The000).FirstOrDefault());
                                    }
                                    else
                                    {
                                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The001).FirstOrDefault());
                                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ComplaintId, ac.electricityDuplicateNotification);
                                    }
                                }

                                if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                                {
                                    if (string.IsNullOrWhiteSpace(ac.waterDuplicateNotification))
                                    {
                                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The000).FirstOrDefault());
                                    }
                                    else
                                    {
                                        filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The001).FirstOrDefault());
                                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ComplaintId, ac.waterDuplicateNotification);
                                    }
                                }
                            }
                        }

                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new List<_CC_Model.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }

                        data.CurrentRequest.IsAnsAltered = true;

                        #endregion [Check notfication Exited Already]

                        break;

                    case _CC_Model.SM_Action.CHECK_AMI_E:
                    case _CC_Model.SM_Action.CHECK_AMI_W:
                    case _CC_Model.SM_Action.CHECK_AMI_W_AI:

                        bool IsAMI = false;
                        filteredQus = null;

                        #region [CHECK_AMI_W_AI]

                        if (data.Answer.Action == _CC_Model.SM_Action.CHECK_AMI_W_AI ||
                            data.Answer.Action == _CC_Model.SM_Action.CHECK_AMI_E_AI)
                        {
                            var GetConsumptionRequestModel = GetConsumptionPeriod(smDetail.BillingMonth);
                            IsAMI = GetConsumptionRequestModel.IsAMI;
                            if (IsAMI && GetConsumptionRequestModel.IsValidRequest)
                            {
                                var d = DTMCInsightsReportClient.GetWaterAI(new apiModel.Request.DTMCInsightsReport.GetWaterAIRequest()
                                {
                                    dsn = GetConsumptionRequestModel.ConsumptionMeterNo,
                                    from = GetConsumptionRequestModel.ConsumptionFromDate,
                                    to = GetConsumptionRequestModel.ConsumptionEndDate,
                                    sessionid = activeUser.SessionToken
                                }, Request.Segment());

                                if (d.Succeeded && d.Payload != null)
                                {
                                    var returnData = d.Payload.ReturnData;
                                    var _d = returnData.Consolidated.FirstOrDefault();

                                    #region [Logic]

                                    //"cotcode": 1 – Text: "abnormal condition detected"
                                    //"cotcode": 2 – Text: "atypical condition detected"
                                    //"cotcode": 3 – Text: "leakage condition detected"
                                    //"cotcode": 4 – Text: "Meter Fault detected"
                                    //"cotcode": 0 – Text: "Found to be normal"
                                    //+++++++++++++++++++++++++++++
                                    //a.  000 - AMI meter, but selected bill is not in the latest 3
                                    //b.  000 - MF - AMI meter, selected bill falls in the latest 3 and Meter Faulty
                                    //c.  000 - WL - AMI meter, selected bill falls in the latest 3 and Water leak
                                    //d.  000 - OT - AMI meter, selected bill falls in the latest 3 and Other Reason
                                    //e.  001 – Non AMI meter

                                    #endregion [Logic]

                                    switch (_d.cotcode)
                                    {
                                        case 1:
                                        case 2:
                                            filteredQus = (_d.reasoncode == 4) ? QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.C000_MF).FirstOrDefault()) : //when reason code is 4 i.e meter faulty
                                                          (_d.reasoncode == 0 || _d.reasoncode == 2 || _d.reasoncode == 22) ? filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.C000_WL).FirstOrDefault()) ://when reason code is 0,2,22 i.e water leakage
                                                          filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.C000_OT).FirstOrDefault());//when by default other case.
                                            break;

                                        case 3:
                                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.C000_WL).FirstOrDefault());
                                            break;

                                        case 4:
                                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.C000_MF).FirstOrDefault());
                                            break;

                                        case 0:
                                        default:
                                            filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.C000_OT).FirstOrDefault());
                                            break;
                                    }

                                    CacheProvider.Store(CacheKeys.FILTER_AMI_STEP, new CacheItem<_CC_Model.Question>(filteredQus, TimeSpan.FromHours(1)));
                                    CacheProvider.Store(CacheKeys.GETWATERAI_RESPONSE, new CacheItem<apiModel.Response.DTMCInsightsReport.GetWaterAIResponse>(d.Payload, TimeSpan.FromHours(1)));
                                    CacheProvider.Store(CacheKeys.CONSUMPTION_INSIGHT_DATAMODEL, new CacheItem<_CC_Model.ConsumptionInsightData>(GetConsumptionRequestModel, TimeSpan.FromHours(1)));
                                }
                            }
                        }

                        #endregion [CHECK_AMI_W_AI]

                        #region [AMI CHECK for CALL Action Option]

                        if (data.Answer.Action == _CC_Model.SM_Action.CHECK_AMI_E)
                        {
                            IsAMI = smDetail.amie;
                        }

                        if (data.Answer.Action == _CC_Model.SM_Action.CHECK_AMI_W)
                        {
                            IsAMI = smDetail.amiw;
                        }

                        if (filteredQus == null)
                        {
                            if (IsAMI)
                            {
                                filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The000).FirstOrDefault());
                            }
                            else
                            {
                                filteredQus = QuestionValueBinder(data.Answer.Questions.Where(x => x.Code == _CC_Model.SM_Code.The001).FirstOrDefault());
                            }
                        }

                        #endregion [AMI CHECK for CALL Action Option]

                        if (filteredQus != null)
                        {
                            data.Answer.Questions = new List<_CC_Model.Question>();
                            data.Answer.Questions.Add(filteredQus);
                        }
                        data.CurrentRequest.IsAnsAltered = true;
                        break;

                    default:
                        break;
                }
            }

            return data;
        }

        public _CC_Model.CommonRender SR_TypeHelper(_CC_Model.CommonRender data)
        {
            var IsStopAction = Convert.ToBoolean(data != null && Convert.ToBoolean(data.CurrentRequest?.IsPageBack));
            _CC_Model.CustomerSubmittedData smDetail = _CC_CommonHelper.GetUserSubmittedData();
            if (data != null && data.Answer != null)
            {
                bool IsSessionExit = IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User);
                var activeUser = AuthStateService.GetActiveProfile();

                switch (data.Answer.Type)
                {
                    case _CC_Model.TypeEnum.Accountselection:

                        data.RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J24_BILLING_COMPLAINT)}";

                        //logic to get All AccountRelated  detail and set it.
                        string accountNo = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Account);

                        if (!string.IsNullOrEmpty(accountNo) && IsLoggedIn && IsSessionExit)
                        {
                            #region [AccountList handling logic]

                            var accountList = GetBillingAccounts(false, true, "");

                            var accountDetail = accountList?.Payload.FirstOrDefault(x => x.AccountNumber == accountNo);
                            if (accountDetail != null && !string.IsNullOrEmpty(accountDetail.AccountNumber))
                            {
                                string personName = GetContactNameByAccountDetails(accountDetail);
                                if (string.IsNullOrEmpty(personName))
                                {
                                    personName = GetContactNameByUserSession();
                                }
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ContactPersonName, personName);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Mobile, activeUser.MobileNumber);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.CA_Location, accountDetail.Location);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Street, accountDetail.Street);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Location, accountDetail.Street);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.BPNumber, accountDetail.BusinessPartnerNumber);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.CustomerPremiseNo, accountDetail.CustomerPremiseNumber);
                                //TODO:SM--need to add location logic - DONE
                            }

                            #endregion [AccountList handling logic]

                            #region [consumption step 10 logic]

                            List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn> acList = new List<DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn>();
                            acList.Add(new DEWAXP.Foundation.Integration.DewaSvc.accountsInfoIn()
                            {
                                contractaccountnumber = accountNo
                            });

                            var srequest = new DEWAXP.Foundation.Integration.DewaSvc.SetNotificationSubmit()
                            {
                                NotificationInfoIn = new DEWAXP.Foundation.Integration.DewaSvc.notificationInfoIn()
                                {
                                    accountlist = acList.ToArray(),
                                    executionflag = "R",
                                    journeystepnumber = "10",
                                },
                            };

                            if (data.Answer.Action == _CC_Model.SM_Action.GetNotificationList)
                            {
                                break;
                            }

                            //Get All bill Consumption testing
                            var ConsumptionReponse = DewaApiClient.SetNotificationSubmit(srequest, "C", activeUser.UserId, activeUser.SessionToken, RequestLanguage, Request.Segment());
                            bool isConsumptionAvaible = false;
                            if (ConsumptionReponse != null && ConsumptionReponse.Succeeded && ConsumptionReponse.Payload?.@return != null)
                            {
                                var consumpData = ConsumptionReponse.Payload.@return;
                                _CC_SessionHelper.CC_GetConsumptionDetail = consumpData;

                                var ac = ConsumptionReponse.Payload.@return.accountslist.FirstOrDefault();

                                if (ac != null)
                                {
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ElectricityConsumptionCA, ac.electricitycontract);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WaterConsumptionCA, ac.watercontract);

                                    //user Detail
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ContactPersonName, ac.firstname + " " + ac.lastname);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Email, ac.email);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Mobile, ac.mobile);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.IsNewCustomer, ac.newcustomer);

                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.amie, ac.amielectricity);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.amiw, ac.amiwater);

                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.EMeterNo, ac.electricitymeternumber);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.EMeterInstallDate, ac.electricitymeterinstallationdate);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WMeterNo, ac.watermeternumber);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WMeterInstallDate, ac.watermeterinstallationdate);

                                    data.Answer.Questions.FirstOrDefault().Answers[0].disabled = string.IsNullOrWhiteSpace(ac.electricitycontract);
                                    data.Answer.Questions.FirstOrDefault().Answers[1].disabled = string.IsNullOrWhiteSpace(ac.watercontract);
                                }
                                //List<_CC_Model.Answer> filteredAns = new List<_CC_Model.Answer>();

                                isConsumptionAvaible = !(consumpData != null && ((string.IsNullOrWhiteSpace(ac.electricitycontract) && string.IsNullOrWhiteSpace(ac.watercontract)) || consumpData.consumptionlist == null));

                                if (isConsumptionAvaible && !string.IsNullOrWhiteSpace(smDetail.ConsumptionType))
                                {
                                    isConsumptionAvaible = GetBillingMonthAndAmount() != null;
                                }
                            }
                            else
                            {
                                _CC_SessionHelper.CC_GetConsumptionDetail = null;
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ElectricityConsumptionCA, null);
                                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.WaterConsumptionCA, null);
                                isConsumptionAvaible = false;
                            }

                            if (!isConsumptionAvaible)
                            {
                                data.IsError = true;
                                data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                                {
                                    ControlId = _CC_Model.SM_Id.ConsumptionType,
                                    ErorrMessage = _CC_CommonHelper.GetSMTranslation(Translate.Text("DT_ConsumptionValidationMsg")).Replace("xxxxx", accountNo)
                                });
                            }

                            #endregion [consumption step 10 logic]
                        }

                        break;

                    case _CC_Model.TypeEnum.Button:
                        break;

                    case _CC_Model.TypeEnum.Confirmation:

                        break;

                    case _CC_Model.TypeEnum.Textinput:
                        break;

                    case _CC_Model.TypeEnum.Notes:
                        break;

                    case _CC_Model.TypeEnum.Loading:
                        break;

                    case _CC_Model.TypeEnum.Showlist:
                        break;

                    default:
                        break;
                }
            }

            return data;
        }

        public _CC_Model.CommonRender SR_IdHelper(_CC_Model.CommonRender data)
        {
            var IsStopAction = Convert.ToBoolean(data != null && Convert.ToBoolean(data.CurrentRequest?.IsPageBack));
            var smDetail = _CC_CommonHelper.GetUserSubmittedData();

            if (data != null && data.Answer != null)
            {
                bool IsSessionExit = IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User);
                var activeUser = AuthStateService.GetActiveProfile();
                switch (data.Answer.Id)
                {
                    case _CC_Model.SM_Id.Empty:
                        break;

                    case _CC_Model.SM_Id.Location:
                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Street, smDetail.Location);
                        break;

                    case _CC_Model.SM_Id.Mobile:
                        break;

                    case _CC_Model.SM_Id.Account:

                        #region [actionable code]

                        data.RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J24_BILLING_COMPLAINT)}";
                        //logic to get All AccountRelated  detail and set it.
                        string accountNo = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.Account);

                        bool isValidAccountNo = false;
                        if (!string.IsNullOrEmpty(accountNo))
                        {
                            if (IsLoggedIn && IsSessionExit)
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
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ContactPersonName, personName);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Mobile, activeUser.MobileNumber);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.BPNumber, accountDetail.BusinessPartnerNumber);
                                    _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.CustomerPremiseNo, accountDetail.CustomerPremiseNumber);
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

                        if (!isValidAccountNo)
                        {
                            data.IsError = true;
                            data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail() { ControlId = _CC_Model.SM_Id.Account });
                        }

                        #endregion [actionable code]

                        break;

                    case _CC_Model.SM_Id.Success:
                        break;

                    case _CC_Model.SM_Id.Media:
                        break;

                    case _CC_Model.SM_Id.Error:
                        break;

                    case _CC_Model.SM_Id.ContactPersonName:
                        break;

                    case _CC_Model.SM_Id.ConsumptionType:
                        //data.CurrentRequest.BillingMonthList = GetBillingMonthAndAmount();
                        data.CurrentRequest.BillingMonthConfig = GetBillingMonthConfig();

                        if (!(data.CurrentRequest.BillingMonthConfig != null && data.CurrentRequest.BillingMonthConfig.BillingMonthData.Count > 0))
                        {
                            data.IsError = true;
                            data.ErrorDetails.Add(new _CC_Model.SM_ErrorDetail()
                            {
                                ControlId = _CC_Model.SM_Id.BillingMonth,
                                ErorrMessage = _CC_CommonHelper.GetSMTranslation(Translate.Text("DT_ConsumptionValidationMsg")).Replace("xxxxx", smDetail.ContractAccountNo)
                            });
                        }
                        break;

                    case _CC_Model.SM_Id.BillingMonth:
                        SetBillingAmount();
                        break;

                    case _CC_Model.SM_Id.TimeOfNotification:

                        break;

                    default:
                        break;
                }
            }

            return data;
        }

        public _CC_Model.Question QuestionValueBinder(_CC_Model.Question data)
        {
            _CC_Model.Question returnData = null;
            if (data != null)
            {
                returnData = new _CC_Model.Question()
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

        public IEnumerable<System.Web.Mvc.SelectListItem> GetBillingMonthAndAmount()
        {
            IEnumerable<System.Web.Mvc.SelectListItem> data = null;
            var CC_StoreData = _CC_SessionHelper.CC_GetConsumptionDetail;
            string type = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);
            if (CC_StoreData != null && CC_StoreData.consumptionlist != null)
            {
                if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                {
                    data = CC_StoreData.consumptionlist?.OrderByDescending(x => x.billingmonth).Where(x => !string.IsNullOrWhiteSpace(x.electricityconsumption)).Select(x => new SelectListItem { Text = _CC_CommonHelper.GetSMTranslation(_CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "MMM")) + " " + _CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "yyyy"), Value = x.billingmonth });
                }

                if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                {
                    data = CC_StoreData.consumptionlist?.OrderByDescending(x => x.billingmonth).Where(x => !string.IsNullOrWhiteSpace(x.waterconsumption)).Select(x => new SelectListItem { Text = _CC_CommonHelper.GetSMTranslation(_CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "MMM")) + " " + _CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "yyyy"), Value = x.billingmonth });
                }
            }
            return data;
        }

        public _CC_Model.ConsumptionInsightData GetConsumptionPeriod(string billingMonth)
        {
            _CC_Model.ConsumptionInsightData data = null;
            _CC_Model.CustomerSubmittedData smDetail = _CC_CommonHelper.GetUserSubmittedData();
            try
            {
                var CC_StoreData = _CC_SessionHelper.CC_GetConsumptionDetail;
                string type = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);
                if (CC_StoreData != null && CC_StoreData.consumptionlist != null)
                {
                    if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                    {
                        if (smDetail.amie)
                        {
                            data = CC_StoreData.consumptionlist.Where(x => !string.IsNullOrWhiteSpace(x.electricityconsumption) && x.billingmonth == billingMonth).Select(x => new _CC_Model.ConsumptionInsightData() { ConsumptionFromDate = x.startbillingperiod, ConsumptionEndDate = x.endbillingperiod }).FirstOrDefault();
                            if (data != null)
                            {
                                data.ConsumptionMeterNo = smDetail.ElectricityMeterNo;
                                data.ConsumptionMeterInstalledDate = smDetail.EMeterInstallDate;
                                data.IsAMI = smDetail.amie;
                                data.CustomerPremiseNo = $"{smDetail.CustomerPremiseNo}_E";
                                data.usagetype = "ME";
                            }
                        }
                    }

                    if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                    {
                        if (smDetail.amiw)
                        {
                            data = CC_StoreData.consumptionlist.Where(x => !string.IsNullOrWhiteSpace(x.waterconsumption) && x.billingmonth == billingMonth).Select(x => new _CC_Model.ConsumptionInsightData() { ConsumptionFromDate = x.startbillingperiod, ConsumptionEndDate = x.endbillingperiod }).FirstOrDefault();
                            if (data != null)
                            {
                                data.ConsumptionMeterNo = smDetail.WaterMeterNo;
                                data.ConsumptionMeterInstalledDate = smDetail.WMeterInstallDate;
                                data.IsAMI = smDetail.amiw;
                                data.CustomerPremiseNo = $"{smDetail.CustomerPremiseNo}_W";
                                data.usagetype = "MW";
                            }
                        }
                    }

                    if (data != null &&
                        Convert.ToDateTime(data.ConsumptionFromDate).Subtract(Convert.ToDateTime(data.ConsumptionMeterInstalledDate)).Ticks >= 0)
                    {
                        data.IsValidRequest = true;
                    }
                    else
                    {
                        data.Description = Translate.Text("CC_InstallatedDateIssue");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }

        public _CC_Model.BillingMonthConfig GetBillingMonthConfig()
        {
            _CC_Model.BillingMonthConfig billingMonthConfig = new _CC_Model.BillingMonthConfig();

            //IEnumerable<System.Web.Mvc.SelectListItem> data = null;
            var CC_StoreData = _CC_SessionHelper.CC_GetConsumptionDetail;
            string type = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);
            if (CC_StoreData != null && CC_StoreData.consumptionlist != null)
            {
                var d = CC_StoreData.consumptionlist?.OrderByDescending(x => x.billingmonth);
                if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                {
                    billingMonthConfig.BillingMonth = d.Where(x => !string.IsNullOrWhiteSpace(x.electricityconsumption))
                                                       ?.Select(x => new _CC_Model.ListHelperItem
                                                       {
                                                           Text = _CC_CommonHelper.GetSMTranslation(_CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "MMM")),
                                                           MappingValue = _CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "yyyy"),
                                                           Value = x.billingmonth
                                                       })?.ToList() ?? null;
                }

                if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                {
                    billingMonthConfig.BillingMonth = d.Where(x => !string.IsNullOrWhiteSpace(x.waterconsumption))
                                                       ?.Select(x => new _CC_Model.ListHelperItem
                                                       {
                                                           Text = _CC_CommonHelper.GetSMTranslation(_CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "MMM")),
                                                           MappingValue = _CC_CommonHelper.CustomDateFormate(x.billingmonth, "yyyyMM", "yyyy"),
                                                           Value = x.billingmonth
                                                       })?.ToList() ?? null;
                }

                if (billingMonthConfig.BillingMonth != null && billingMonthConfig.BillingMonth.Count > 0)
                {
                    billingMonthConfig.BillingMonthData = billingMonthConfig.BillingMonth.Select(x => new _CC_Model.ListHelperItem { Text = $"{x.Text} {x.MappingValue}", Value = x.Value }).ToList();
                    billingMonthConfig.BillingYear = billingMonthConfig.BillingMonth.Select(x => x.MappingValue).Distinct().Select(x => new _CC_Model.ListHelperItem { Text = x, Value = x }).ToList();
                    billingMonthConfig.BillingMonthJson = JsonConvert.SerializeObject(billingMonthConfig.BillingMonth); ;
                }
            }
            return billingMonthConfig;
        }

        public void SetBillingAmount()
        {
            var CC_StoreData = _CC_SessionHelper.CC_GetConsumptionDetail;
            string date = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.BillingMonth);
            string type = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);
            if (CC_StoreData != null && CC_StoreData.consumptionlist != null)
            {
                var filteredDate = CC_StoreData.consumptionlist.FirstOrDefault(x => x.billingmonth == date);

                if (filteredDate != null)
                {
                    if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)//electricity
                    {
                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ConsumptionValue, filteredDate.electricityconsumption);
                    }

                    if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)//water
                    {
                        _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.ConsumptionValue, filteredDate.waterconsumption);
                    }
                }
            }
        }

        public IEnumerable<System.Web.Mvc.SelectListItem> GetShiftTimeList(DEWAXP.Foundation.Integration.DewaSvc.shift[] shiftDetail, DEWAXP.Foundation.Integration.DewaSvc.sInput[] shiftOrders)
        {
            SmlangCode _Curlang = _CC_CommonHelper.GetSMLangType(_CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.LangType));
            List<System.Web.Mvc.SelectListItem> data = new List<SelectListItem>();
            //string startTime = "000000";
            //string endTime = "000000";
            //string intervalTime = "000000";

            List<_CC_Model.TimeSlotsSlotDetail> timeShifts = new List<_CC_Model.TimeSlotsSlotDetail>();

            //string startTimeFormatted = "000000";
            //string endTimeFormatted = "000000";
            //string intervalTimeFormatted = "000000";
            if (shiftOrders != null && shiftOrders.Count() >= 3)
            {
                foreach (var item in shiftOrders)
                {
                    var filterData = shiftDetail.Where(x => x.controlkey == item.controlkey).FirstOrDefault();
                    switch (item.controlkey)
                    {
                        case "ZESHIFT":
                        case "ZECSHIFT":
                        case "ZWSHIFT":
                        case "ZWCSHIFT":
                            timeShifts.AddRange(GetShiftTimeDetailSlotList(shiftDetail, item.controlkey));
                            break;
                        //case "ZEETIME":
                        //case "ZWETIME":
                        //    endTime = item.controlFromvalue;
                        //    break;
                        //case "ZESLOTD":
                        //case "ZWSLOTD":
                        //    intervalTime = item.controlFromvalue;
                        //    break;
                        //case "EHDLIST":
                        //case "WHDLIST":
                        ///
                        //break;
                        case "ZEDAYS":
                        case "ZWDAYS":
                            _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.DNo, filterData?.controlFromvalue);
                            break;

                        default:
                            break;
                    }
                }
            }

            //DateTime Starttimeloop = DateTime.ParseExact(startTime, "HH:mm:ss", new CultureInfo("en-GB"));
            //DateTime endtimeloop = DateTime.ParseExact(endTime, "HH:mm:ss", new CultureInfo("en-GB"));
            //DateTime intervaltimeloop = DateTime.ParseExact(intervalTime, "HH:mm:ss", new CultureInfo("en-GB"));

            //if no value came from service
            if (string.IsNullOrWhiteSpace(_CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.DNo)))
            {
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.DNo, "90");
            }
            //DateTime __date = Starttimeloop;
            //data.Add(new SelectListItem { Text = Starttimeloop.ToString("HH:mm:ss"), Value = Starttimeloop.ToString("HHmmss") });

            //for (int i = 0; (__date.Ticks - endtimeloop.Ticks) < 0; i++)
            //{
            //    __date = __date.Add(new TimeSpan(intervaltimeloop.Hour, intervaltimeloop.Minute, intervaltimeloop.Second));
            //    if ((__date.Ticks - endtimeloop.Ticks) < 0)
            //    {
            //        data.Add(new SelectListItem { Text = __date.ToString("HH:mm:ss"), Value = __date.ToString("HHmmss") });
            //    }
            //}
            //data.Add(new SelectListItem { Text = endtimeloop.ToString("HH:mm:ss"), Value = endtimeloop.ToString("HHmmss") });
            string _pm = _CC_CommonHelper.GetSMTranslation("PM");
            string _am = _CC_CommonHelper.GetSMTranslation("AM");

            string timeFormat = "h:mm tt";
            if (_Curlang == SmlangCode.zh)
            {
                timeFormat = "tt h:mm";
            }
            data = timeShifts.OrderBy(x => x.WeekNo).ToList()?.Select(x => new SelectListItem { Value = $"{_CC_CommonHelper.CustomDateFormate(x.StartTime, "HH:mm:ss", "HHmmss")}|{x.WeekNo}", Text = (_CC_CommonHelper.CustomDateFormate(x.StartTime, "HH:mm:ss", timeFormat) + "-" + _CC_CommonHelper.CustomDateFormate(x.OutTime, "HH:mm:ss", timeFormat))?.Replace("PM", _pm)?.Replace("AM", _am) }).ToList();

            return data;
        }

        public List<_CC_Model.TimeSlotsSlotDetail> GetShiftTimeDetailSlotList(DEWAXP.Foundation.Integration.DewaSvc.shift[] shiftDetail, string key)
        {
            List<_CC_Model.TimeSlotsSlotDetail> data = new List<_CC_Model.TimeSlotsSlotDetail>();

            string _filterKey = "";
            switch (key)
            {
                case "ZESHIFT":
                    _filterKey = "ZE{0}SHIFT";
                    break;

                case "ZECSHIFT":
                    _filterKey = "ZEC{0}SHIFT";
                    break;

                case "ZWSHIFT":
                    _filterKey = "ZW{0}SHIFT";
                    break;

                case "ZWCSHIFT":
                    _filterKey = "ZWC{0}SHIFT";
                    break;

                default:
                    break;
            }
            if (!string.IsNullOrWhiteSpace(_filterKey))
            {
                for (int i = 1; i <= 5; i++)
                {
                    var filterShifts = shiftDetail.Where(x => x.controlkey == string.Format(_filterKey, i));

                    if (filterShifts.Any())
                    {
                        data.AddRange(filterShifts.Select(xx => new _CC_Model.TimeSlotsSlotDetail() { BackendKey = xx.controlkey, WeekNo = Convert.ToString(i), StartTime = xx.controlFromvalue, OutTime = xx.controlTovalue, }));
                    }
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

        private string GetContactNameByAccountDetails(DEWAXP.Foundation.Integration.Responses.AccountDetails accountDetail)
        {
            if (accountDetail != null)
            {
                return !string.IsNullOrWhiteSpace(accountDetail.AccountName) ? accountDetail.AccountName : accountDetail.NickName;
            }
            return "";
        }

        //Delete file
        private bool DeleteFile(string filePath)
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
                string outPutImage = $"{_CC_Model.SM_Id.Image1.ToString() + Guid.NewGuid()}.jpg";
                using (var image = Image.FromFile(imgPath))
                {
                    var image1Path = System.IO.Path.Combine(filePath, outPutImage);
                    using (Graphics thumbnailGraph = Graphics.FromImage(image))
                    {
                        try
                        {
                            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            using (SolidBrush brush = new SolidBrush(Color.Red))
                            {
                                using (Pen pen = new Pen(brush, 10))
                                {
                                    foreach (var item in boxes)
                                    {
                                        var rectCord = item;
                                        if (rectCord != null)
                                        {
                                            thumbnailGraph.DrawRectangle(pen, new Rectangle(Convert.ToInt32(rectCord[0]), Convert.ToInt32(rectCord[1]), Convert.ToInt32(rectCord[2]), Convert.ToInt32(rectCord[3])));
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.Error(ex, this);
                        }
                        finally
                        {
                            if (thumbnailGraph != null)
                            {
                                thumbnailGraph.Dispose();
                            }
                        }
                    }
                    image.Save(image1Path);
                }

                DeleteFile(imgPath);
                _CC_CommonHelper.SetUserSelectedAnsTypeAndValue(_CC_Model.SM_Id.Image1, outPutImage);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        public _CC_Model.SurveyInfoDetail GetQuestionAndAns(string surveyId, string applicationId)
        {
            _CC_Model.SurveyInfoDetail surveyInfoDetail = new _CC_Model.SurveyInfoDetail();
            System.Data.DataTable d = null;

            var data = ConsultantServiceClient.GetSurveyQuestions(new DEWAXP.Foundation.Integration.SmartConsultantSvc.GetSurveyQuestions() { surveyid = surveyId, applicationid = applicationId }, RequestLanguage, Request.Segment());

            if (data.Succeeded && data.Payload.@return.surveyQuestions != null && data.Payload.@return.surveyQuestions.Count() > 0)
            {
                var dataList = data.Payload.@return.surveyQuestions.ToList();

                //var title = dataList.Where(x => x.type == "Title").FirstOrDefault()?.text;
                //var section = dataList.Where(x => x.type == "Section").FirstOrDefault()?.text;

                d = ListtoDataTableConverter.ToDataTable(dataList);

                //surveyInfoDetail.IntroText = $"{title }{section}";

                var ansList = dataList.Where(x => x.type == "Answer");

                string[] subQusIdList = ansList?.GroupBy(x => x.subQuestion)?.Select(x => x.FirstOrDefault()?.subQuestion)?.ToArray() ?? null;

                var questionList = dataList.Where(x => x.type == "Question" && !Convert.ToBoolean(subQusIdList?.Contains(x.questionId))
                                                     && string.IsNullOrEmpty(x.subQuestion));

                var subQuestionList = dataList.Where(x => x.type == "Question" && Convert.ToBoolean(subQusIdList?.Contains(x.questionId)) && string.IsNullOrEmpty(x.subQuestion));

                #region [Emoji Question]

                if (questionList != null && questionList.Where(x => (x.category == "EMOJI")) != null)
                {
                    foreach (var item in questionList.Where(x => (x.category == "EMOJI") && string.IsNullOrEmpty(x.subQuestion)))
                    {
                        string qusId = item.questionId;
                        var qus = new _CC_Model.QuestionAndAnsItem()
                        {
                            Question = item.text,
                            Id = qusId,
                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                            ValueId = item.valueId,
                        };

                        var filteredAnsList = ansList.Where(x => x.answerId == (qusId + "_a") && x.category == item.category)
                            .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                        qus.AnsList.AddRange(filteredAnsList);

                        //get Sub Qus no
                        var filteredSubQus = filteredAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                        if (filteredSubQus != null && filteredSubQus.Count() > 0)
                        {
                            qus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>();
                            //Filtered Sub Ques list
                            foreach (var subQusItem in subQuestionList.Where(x => filteredSubQus.Contains(x.questionId)))
                            {
                                string subQusId = subQusItem.questionId;
                                var subQus = new _CC_Model.QuestionAndAnsItem()
                                {
                                    Question = subQusItem.text,
                                    Id = subQusId,
                                    AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                    ValueId = subQusItem.valueId,
                                };

                                var filteredSubAnsList = ansList.Where(x => x.answerId == (subQusId + "_a"))
                              .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                                subQus.AnsList.AddRange(filteredSubAnsList);

                                qus.SubQuestion.Add(subQus);
                            }
                        }

                        surveyInfoDetail.CustomerEmotion.Add(qus);
                    }
                }

                #endregion [Emoji Question]

                #region [final Question List]

                if (questionList != null && questionList.Where(x => (x.category == "RATING" || x.category == "YESNO")) != null)
                {
                    foreach (var item in questionList.Where(x => (x.category == "RATING" || x.category == "YESNO") && string.IsNullOrEmpty(x.subQuestion)))
                    {
                        string qusId = item.questionId;
                        var qus = new _CC_Model.QuestionAndAnsItem()
                        {
                            Question = item.text,
                            Id = qusId,
                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                            ValueId = item.valueId,
                            PreSectionTitle = GetQuestionSection(dataList, qusId)
                        };

                        var filteredAnsList = ansList.Where(x => x.answerId == (qusId + "_a") && x.category == item.category).Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();

                        if (filteredAnsList != null && filteredAnsList.Count > 4)
                        {
                            qus.AnsList.AddRange(filteredAnsList.OrderBy(x => x.Question));
                        }
                        else
                        {
                            qus.AnsList.AddRange(filteredAnsList);
                        }

                        //get Sub Qus no
                        var filteredSubQus = filteredAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                        if (filteredSubQus != null && filteredSubQus.Count() > 0)
                        {
                            qus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>();
                            //Filtered Sub Ques list
                            foreach (var subQusItem in subQuestionList.Where(x => filteredSubQus.Contains(x.questionId)))
                            {
                                string subQusId = subQusItem.questionId;
                                var subQus = new _CC_Model.QuestionAndAnsItem()
                                {
                                    Question = subQusItem.text,
                                    Id = subQusId,
                                    AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                    ValueId = subQusItem.valueId,
                                    PreSectionTitle = GetQuestionSection(dataList, subQusId)
                                };

                                var filteredSubAnsList = ansList.Where(x => x.answerId == (subQusId + "_a")).Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();

                                if (filteredSubAnsList != null && filteredSubAnsList.Count > 4)
                                {
                                    subQus.AnsList.AddRange(filteredSubAnsList.OrderBy(x => x.Question));
                                }
                                else
                                {
                                    subQus.AnsList.AddRange(filteredSubAnsList);
                                }

                                //get sub qus ans - qus

                                #region [sub qus ans - qus]

                                var filteredSubQusSubQus = filteredSubAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                                if (filteredSubQusSubQus != null && filteredSubQusSubQus.Count() > 0)
                                {
                                    subQus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>(); //initailizing subQuestion

                                    foreach (var sub_subQusItem in subQuestionList.Where(x => filteredSubQusSubQus.Contains(x.questionId)))
                                    {
                                        string subQusSubQusId = sub_subQusItem.questionId;
                                        var subsubQus = new _CC_Model.QuestionAndAnsItem()
                                        {
                                            Question = sub_subQusItem.text,
                                            Id = subQusSubQusId,
                                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                            ValueId = sub_subQusItem.valueId,
                                            PreSectionTitle = GetQuestionSection(dataList, subQusSubQusId)
                                        };

                                        var filteredSubQusSubAnsList = ansList.Where(x => x.answerId == (subQusSubQusId + "_a")).Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();

                                        if (filteredSubQusSubAnsList != null && filteredSubQusSubAnsList.Count > 4)
                                        {
                                            subsubQus.AnsList.AddRange(filteredSubQusSubAnsList.OrderBy(x => x.Question));
                                        }
                                        else
                                        {
                                            subsubQus.AnsList.AddRange(filteredSubQusSubAnsList);
                                        }
                                        subQus.SubQuestion.Add(subsubQus);
                                    }
                                }

                                #endregion [sub qus ans - qus]

                                qus.SubQuestion.Add(subQus);
                            }
                        }

                        surveyInfoDetail.FinalQuestionList.Add(qus);
                    }
                }

                #endregion [final Question List]

                #region [Bottom Question List]

                if (questionList != null && questionList.Where(x => (x.category != "RATING" && x.category != "YESNO" && x.category != "EMOJI")) != null)
                {
                    foreach (var item in questionList.Where(x => (x.category != "RATING" && x.category != "YESNO" && x.category != "EMOJI" && !subQusIdList.Contains(x.questionId)) && string.IsNullOrEmpty(x.subQuestion)))
                    {
                        string qusId = item.questionId;
                        var qus = new _CC_Model.QuestionAndAnsItem()
                        {
                            Question = item.text,
                            Id = qusId,
                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                            ValueId = item.valueId,
                            PreSectionTitle = GetQuestionSection(dataList, qusId)
                        };

                        var filteredAnsList = ansList.Where(x => x.answerId == (qusId + "_a") && x.category == item.category)
                            .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                        qus.AnsList.AddRange(filteredAnsList);

                        //get Sub Qus no
                        var filteredSubQus = filteredAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                        if (filteredSubQus != null && filteredSubQus.Count() > 0)
                        {
                            qus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>();
                            //Filtered Sub Ques list
                            foreach (var subQusItem in subQuestionList.Where(x => filteredSubQus.Contains(x.questionId)))
                            {
                                string subQusId = subQusItem.questionId;
                                var subQus = new _CC_Model.QuestionAndAnsItem()
                                {
                                    Question = subQusItem.text,
                                    Id = subQusId,
                                    AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                    ValueId = subQusItem.valueId,
                                    PreSectionTitle = GetQuestionSection(dataList, subQusId)
                                };

                                var filteredSubAnsList = ansList.Where(x => x.answerId == (subQusId + "_a"))
                              .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                                subQus.AnsList.AddRange(filteredSubAnsList);

                                qus.SubQuestion.Add(subQus);
                            }
                        }

                        surveyInfoDetail.BottomQuestionList.Add(qus);
                    }
                }

                #endregion [Bottom Question List]
            }
            else
            {
                surveyInfoDetail.ShowError = true;
                surveyInfoDetail.ErrorMessage = data.Message;
            }

            return surveyInfoDetail;
        }

        public _CC_Model.SurveyInfoDetail GetInquiriesQuestionAndAns(string surveyId, string applicationId)
        {
            _CC_Model.SurveyInfoDetail surveyInfoDetail = new _CC_Model.SurveyInfoDetail();
            System.Data.DataTable d = null;

            var data = ConsultantServiceClient.GetSurveyQuestions(new DEWAXP.Foundation.Integration.SmartConsultantSvc.GetSurveyQuestions() { surveyid = surveyId, applicationid = applicationId }, RequestLanguage, Request.Segment());

            if (data.Succeeded && data.Payload.@return.surveyQuestions != null && data.Payload.@return.surveyQuestions.Count() > 0)
            {
                var dataList = data.Payload.@return.surveyQuestions.ToList();

                //var title = dataList.Where(x => x.type == "Title").FirstOrDefault()?.text;
                //var section = dataList.Where(x => x.type == "Section").FirstOrDefault()?.text;

                d = ListtoDataTableConverter.ToDataTable(dataList);

                //surveyInfoDetail.IntroText = $"{title }{section}";

                var ansList = dataList.Where(x => x.type == "Answer");

                string[] subQusIdList = ansList?.GroupBy(x => x.subQuestion)?.Select(x => x.FirstOrDefault()?.subQuestion)?.ToArray() ?? null;

                var questionList = dataList.Where(x => x.type == "Question" && !Convert.ToBoolean(subQusIdList?.Contains(x.questionId))
                                                     && string.IsNullOrEmpty(x.subQuestion));

                var subQuestionList = dataList.Where(x => x.type == "Question" && Convert.ToBoolean(subQusIdList?.Contains(x.questionId)) && string.IsNullOrEmpty(x.subQuestion));

                #region [Emoji Question]

                if (questionList != null && questionList.Where(x => (x.category == "EMOJI")) != null)
                {
                    foreach (var item in questionList.Where(x => (x.category == "EMOJI") && string.IsNullOrEmpty(x.subQuestion)))
                    {
                        string qusId = item.questionId;
                        var qus = new _CC_Model.QuestionAndAnsItem()
                        {
                            Question = item.text,
                            Id = qusId,
                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                            ValueId = item.valueId,
                        };

                        var filteredAnsList = ansList.Where(x => x.answerId == (qusId + "_a") && x.category == item.category)
                            .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                        qus.AnsList.AddRange(filteredAnsList);

                        //get Sub Qus no
                        var filteredSubQus = filteredAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                        if (filteredSubQus != null && filteredSubQus.Count() > 0)
                        {
                            qus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>();
                            //Filtered Sub Ques list
                            foreach (var subQusItem in subQuestionList.Where(x => filteredSubQus.Contains(x.questionId)))
                            {
                                string subQusId = subQusItem.questionId;
                                var subQus = new _CC_Model.QuestionAndAnsItem()
                                {
                                    Question = subQusItem.text,
                                    Id = subQusId,
                                    AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                    ValueId = subQusItem.valueId,
                                };

                                var filteredSubAnsList = ansList.Where(x => x.answerId == (subQusId + "_a"))
                              .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                                subQus.AnsList.AddRange(filteredSubAnsList);

                                qus.SubQuestion.Add(subQus);
                            }
                        }

                        surveyInfoDetail.CustomerEmotion.Add(qus);
                    }
                }

                #endregion [Emoji Question]

                #region [final Question List]

                if (questionList != null && questionList.Where(x => (x.category == "RATING" || x.category == "YESNO")) != null)
                {
                    foreach (var item in questionList.Where(x => (x.category == "RATING" || x.category == "YESNO") && string.IsNullOrEmpty(x.subQuestion)))
                    {
                        string qusId = item.questionId;
                        var qus = new _CC_Model.QuestionAndAnsItem()
                        {
                            Question = item.text,
                            Id = qusId,
                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                            ValueId = item.valueId,
                            PreSectionTitle = GetQuestionSection(dataList, qusId)
                        };

                        var filteredAnsList = ansList.Where(x => x.answerId == (qusId + "_a") && x.category == item.category).Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();

                        //if (filteredAnsList != null && filteredAnsList.Count > 4)
                        //{
                        //    qus.AnsList.AddRange(filteredAnsList.OrderBy(x => x.Question));
                        //}
                        //else
                        //{
                        //    qus.AnsList.AddRange(filteredAnsList);
                        //}

                        qus.AnsList.AddRange(filteredAnsList);

                        //get Sub Qus no
                        var filteredSubQus = filteredAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                        if (filteredSubQus != null && filteredSubQus.Count() > 0)
                        {
                            qus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>();
                            //Filtered Sub Ques list
                            foreach (var subQusItem in subQuestionList.Where(x => filteredSubQus.Contains(x.questionId)))
                            {
                                string subQusId = subQusItem.questionId;
                                var subQus = new _CC_Model.QuestionAndAnsItem()
                                {
                                    Question = subQusItem.text,
                                    Id = subQusId,
                                    AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                    ValueId = subQusItem.valueId,
                                    PreSectionTitle = GetQuestionSection(dataList, subQusId)
                                };

                                var filteredSubAnsList = ansList.Where(x => x.answerId == (subQusId + "_a")).Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();

                                //if (filteredSubAnsList != null && filteredSubAnsList.Count > 4)
                                //{
                                //    subQus.AnsList.AddRange(filteredSubAnsList.OrderBy(x => x.Question));
                                //}
                                //else
                                //{
                                //    subQus.AnsList.AddRange(filteredSubAnsList);
                                //}

                                subQus.AnsList.AddRange(filteredSubAnsList);

                                //get sub qus ans - qus

                                #region [sub qus ans - qus]

                                var filteredSubQusSubQus = filteredSubAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                                if (filteredSubQusSubQus != null && filteredSubQusSubQus.Count() > 0)
                                {
                                    subQus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>(); //initailizing subQuestion

                                    foreach (var sub_subQusItem in subQuestionList.Where(x => filteredSubQusSubQus.Contains(x.questionId)))
                                    {
                                        string subQusSubQusId = sub_subQusItem.questionId;
                                        var subsubQus = new _CC_Model.QuestionAndAnsItem()
                                        {
                                            Question = sub_subQusItem.text,
                                            Id = subQusSubQusId,
                                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                            ValueId = sub_subQusItem.valueId,
                                            PreSectionTitle = GetQuestionSection(dataList, subQusSubQusId)
                                        };

                                        var filteredSubQusSubAnsList = ansList.Where(x => x.answerId == (subQusSubQusId + "_a")).Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();

                                        if (filteredSubQusSubAnsList != null && filteredSubQusSubAnsList.Count > 4)
                                        {
                                            subsubQus.AnsList.AddRange(filteredSubQusSubAnsList.OrderBy(x => x.Question));
                                        }
                                        else
                                        {
                                            subsubQus.AnsList.AddRange(filteredSubQusSubAnsList);
                                        }
                                        subQus.SubQuestion.Add(subsubQus);
                                    }
                                }

                                #endregion [sub qus ans - qus]

                                qus.SubQuestion.Add(subQus);
                            }
                        }

                        surveyInfoDetail.FinalQuestionList.Add(qus);
                    }
                }

                #endregion [final Question List]

                #region [Bottom Question List]

                if (questionList != null && questionList.Where(x => (x.category != "RATING" && x.category != "YESNO" && x.category != "EMOJI")) != null)
                {
                    foreach (var item in questionList.Where(x => (x.category != "RATING" && x.category != "YESNO" && x.category != "EMOJI" && !subQusIdList.Contains(x.questionId)) && string.IsNullOrEmpty(x.subQuestion)))
                    {
                        string qusId = item.questionId;
                        var qus = new _CC_Model.QuestionAndAnsItem()
                        {
                            Question = item.text,
                            Id = qusId,
                            AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                            ValueId = item.valueId,
                            PreSectionTitle = GetQuestionSection(dataList, qusId)
                        };

                        var filteredAnsList = ansList.Where(x => x.answerId == (qusId + "_a") && x.category == item.category)
                            .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                        qus.AnsList.AddRange(filteredAnsList);

                        //get Sub Qus no
                        var filteredSubQus = filteredAnsList.Where(x => !string.IsNullOrEmpty(x.SubQuestionId))?.GroupBy(u => u.SubQuestionId)?.Select(x => x.FirstOrDefault()?.SubQuestionId);
                        if (filteredSubQus != null && filteredSubQus.Count() > 0)
                        {
                            qus.SubQuestion = new List<_CC_Model.QuestionAndAnsItem>();
                            //Filtered Sub Ques list
                            foreach (var subQusItem in subQuestionList.Where(x => filteredSubQus.Contains(x.questionId)))
                            {
                                string subQusId = subQusItem.questionId;
                                var subQus = new _CC_Model.QuestionAndAnsItem()
                                {
                                    Question = subQusItem.text,
                                    Id = subQusId,
                                    AnsList = new List<_CC_Model.QuestionAndAnsItem>(),
                                    ValueId = subQusItem.valueId,
                                    PreSectionTitle = GetQuestionSection(dataList, subQusId)
                                };

                                var filteredSubAnsList = ansList.Where(x => x.answerId == (subQusId + "_a"))
                              .Select(x => new _CC_Model.QuestionAndAnsItem() { Question = x.text, ValueId = x.valueId, Id = x.answerId, SubQuestionId = x.subQuestion }).ToList();
                                subQus.AnsList.AddRange(filteredSubAnsList);

                                qus.SubQuestion.Add(subQus);
                            }
                        }

                        surveyInfoDetail.BottomQuestionList.Add(qus);
                    }
                }

                #endregion [Bottom Question List]
            }
            else
            {
                surveyInfoDetail.ShowError = true;
                surveyInfoDetail.ErrorMessage = data.Message;
            }

            return surveyInfoDetail;
        }

        private List<_CC_Model.ConsumptionTrackingDetail> GetNotificationDataList(string accNo, string o = null)
        {
            List<_CC_Model.ConsumptionTrackingDetail> returnData = new List<_CC_Model.ConsumptionTrackingDetail>();

            // Fetch contact details for the selected account
            var response = DewaApiClient.GetContactDetails(CurrentPrincipal.SessionToken, accNo, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                var contactDetails = ContactDetails.From(response.Payload);

                // Fetch the selected account details
                //var accountListResponse = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false,RequestLanguage, Request.Segment());
                var accountListResponse = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

                if (accountListResponse.Succeeded)
                {
                    var selectedAccount = accountListResponse.Payload.First(al => al.AccountNumber == accNo);

                    // Fetch and map the complaint track requests
                    var bpNumber = selectedAccount.BusinessPartnerNumber;
                    var accountNumber = selectedAccount.AccountNumber;
                    var enquiriesResponse = DewaApiClient.GetCustomerEnquiries(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, bpNumber, accountNumber, DEWAXP.Foundation.Integration.Enums.SupportedLanguage.English, Request.Segment(), o);

                    if (enquiriesResponse.Succeeded)
                    {
                        var enquiries = enquiriesResponse.Payload;

                        returnData = enquiries.Select(x => new _CC_Model.ConsumptionTrackingDetail()
                        {
                            RequestDate = x.RequestDate,
                            CompletedDate = x.CompletedDate,
                            Status = x.Status,
                            Reference = x.Reference,
                            RequestType = x.RequestType,
                            StatusDate = x.StatusDate,//!string.IsNullOrWhiteSpace(x.StatusDate) && x.StatusDate != "0000-00-00" ? DateTime.Parse(x.StatusDate).ToString("dd MMM yyyy", new CultureInfo("en-GB")) : "",
                            StatusTime = x.StatusTime,
                            StatusCode = x.StatusCode,
                            StatusDescription = x.StatusDescription,
                        })?.ToList();
                    }
                }
            }

            return returnData;
        }

        private string GetJsonStringOfConsumptionUsage(List<_CC_Model.ConsumptionData> d)
        {
            string jsonValue = "[]";

            List<_CC_Model.ConsumptionData> returnData = new List<_CC_Model.ConsumptionData>();
            if (d != null)
            {
                var SortedData = d.OrderBy(x => Convert.ToDecimal(x.Data2)).ToList();

                foreach (_CC_Model.ConsumptionData item in d)
                {
                    int indx = SortedData.IndexOf(item);
                    item.Data3 = Translate.Text(string.Format("sm_cslb_{0}", indx));
                    returnData.Add(item);
                }
                jsonValue = JsonConvert.SerializeObject(returnData.Select(x => new
                {
                    title = x.Data,
                    time = x.Data1,
                    color = x.Data3,
                    value = Convert.ToDecimal(x.Data2)
                }));
            }

            return jsonValue;
        }

        private string GeSlabCapsData()
        {
            string _data = "0,0,0,0";

            _CC_Model.CustomerSubmittedData smDetail = _CC_CommonHelper.GetUserSubmittedData();

            if (smDetail != null)
            {
                var SlabResponse = DewaApiClient.GetSlabCaps(new DEWAXP.Foundation.Integration.DewaSvc.GetSlabCaps()
                {
                    slabTarrifIn = new DEWAXP.Foundation.Integration.DewaSvc.slabTarrifIn()
                    {
                        contractAccount = smDetail.ContractAccountNo,
                        credential = CurrentPrincipal.SessionToken,
                        lang = RequestLanguageCode
                    }
                }, RequestLanguage, Request.Segment());

                if (SlabResponse != null && SlabResponse.Succeeded)
                {
                    if (smDetail.ConsumptionType == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)
                    {
                        _data = _CC_CommonHelper.slabList(SlabResponse.Payload, "01");
                    }

                    if (smDetail.ConsumptionType == _CC_Model.CommonConst.ConsumptionType_WATERCODE)
                    {
                        _data = _CC_CommonHelper.slabList(SlabResponse.Payload, "02");
                    }
                }
            }

            return _data;
        }

        private List<_CC_Model.ConsumptionData> GetConsumptionSlabData()
        {
            List<_CC_Model.ConsumptionData> consumptionSlabData = new List<_CC_Model.ConsumptionData>();

            var CC_StoreData = _CC_SessionHelper.CC_GetConsumptionDetail;
            string date = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.BillingMonth);
            string type = _CC_CommonHelper.GetSelectedAnswerValue(_CC_Model.SM_Id.ConsumptionType);

            if (CC_StoreData != null && CC_StoreData.consumptionlist != null)
            {
                //Current Selected month Consumption;

                #region [Current Selected month Consumption]

                var filteredDate = CC_StoreData.consumptionlist.FirstOrDefault(x => x.billingmonth == date);

                if (filteredDate != null)
                {
                    if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)//electricity
                    {
                        consumptionSlabData.Add(new _CC_Model.ConsumptionData()
                        {
                            Data = "SelectedMonth",
                            Data1 = date,
                            Data2 = filteredDate.electricityconsumption,
                        });
                    }

                    if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)//water
                    {
                        consumptionSlabData.Add(new _CC_Model.ConsumptionData()
                        {
                            Data = "SelectedMonth",
                            Data1 = date,
                            Data2 = filteredDate.waterconsumption,
                        });
                    }
                }

                #endregion [Current Selected month Consumption]

                //Prevoius month Consumption

                #region [Current Selected Pre month Consumption]

                try
                {
                    var filterPreMonthDate = Convert.ToDateTime(_CC_CommonHelper.CustomDateFormate(date, "yyyyMM", "yyyy-MM-dd")).AddMonths(-1).ToString("yyyyMM");
                    var filteredPreMonthDate = CC_StoreData.consumptionlist.FirstOrDefault(x => filterPreMonthDate == x.billingmonth);

                    if (filteredPreMonthDate != null)
                    {
                        if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)//electricity
                        {
                            consumptionSlabData.Add(new _CC_Model.ConsumptionData()
                            {
                                Data = "SelectedPreMonth",
                                Data1 = filterPreMonthDate,
                                Data2 = filteredPreMonthDate.electricityconsumption,
                            });
                        }

                        if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)//water
                        {
                            consumptionSlabData.Add(new _CC_Model.ConsumptionData()
                            {
                                Data = "SelectedPreMonth",
                                Data1 = filterPreMonthDate,
                                Data2 = filteredPreMonthDate.waterconsumption,
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                }

                #endregion [Current Selected Pre month Consumption]

                //Prevoius month Consumption

                #region [Current Selected Pre Year Consumption]

                try
                {
                    var filterPreYearDate = Convert.ToDateTime(_CC_CommonHelper.CustomDateFormate(date, "yyyyMM", "yyyy-MM-dd")).AddYears(-1).ToString("yyyyMM");
                    var filteredPreYearData = CC_StoreData.consumptionlist.FirstOrDefault(x => filterPreYearDate == x.billingmonth);

                    if (filteredPreYearData != null)
                    {
                        if (type == _CC_Model.CommonConst.ConsumptionType_ELECTRICITYCODE)//electricity
                        {
                            consumptionSlabData.Add(new _CC_Model.ConsumptionData()
                            {
                                Data = "SelectedPreYear",
                                Data1 = filterPreYearDate,
                                Data2 = filteredPreYearData.electricityconsumption,
                            });
                        }

                        if (type == _CC_Model.CommonConst.ConsumptionType_WATERCODE)//water
                        {
                            consumptionSlabData.Add(new _CC_Model.ConsumptionData()
                            {
                                Data = "SelectedPreYear",
                                Data1 = filterPreYearDate,
                                Data2 = filteredPreYearData.waterconsumption,
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                }

                #endregion [Current Selected Pre Year Consumption]
            }

            return consumptionSlabData;
        }

        private _CC_Model.ConsumptionGraphData GetConsumptionGraphDetail(_CC_Model.ConsumptionInsightData model,
                                                                            apiModel.Response.DTMCInsightsReport.GetWaterAIResponse getWaterAIResponse,
                                                                             _CC_Model.CustomerSubmittedData smDetail)
        {
            _CC_Model.ConsumptionGraphData data = new _CC_Model.ConsumptionGraphData();

            List<_CC_Model.ConsumptionData> consumptionDatas = new List<_CC_Model.ConsumptionData>();

            var activeUser = AuthStateService.GetActiveProfile();
            if (model != null && getWaterAIResponse.ReturnData.daily != null && smDetail != null)
            {
                var returnData = DTMCInsightsReportClient.GetWaterAIConsumption(new apiModel.Request.DTMCInsightsReport.GetWaterAIConsumptionRequest()
                {
                    premise = model.CustomerPremiseNo,
                    startdate = _CC_CommonHelper.CustomDateFormate(model.ConsumptionFromDate, "yyyy-MM-dd", "dd.MM.yyyy"),
                    enddate = _CC_CommonHelper.CustomDateFormate(model.ConsumptionEndDate, "yyyy-MM-dd", "dd.MM.yyyy"),
                    usagetype = model.usagetype,
                    sessionid = activeUser.SessionToken,
                }, Request.Segment());

                if (returnData != null && returnData.Succeeded &&
                    returnData.Payload.ReplyMessage.Payload != null && returnData.Payload.ReplyMessage.Payload.MeterReading != null && returnData.Payload.ReplyMessage.Payload.MeterReading.IntervalBlock != null)
                {
                    string _d_formate = "yyyy-MM-dd";

                    string _dis_formate = "MMdd";
                    foreach (apiModel.Response.DTMCInsightsReport.GWAICR_RM_P_MR_IB_IReading item in returnData.Payload.ReplyMessage.Payload.MeterReading.IntervalBlock.IReading)
                    {
                        var formmatedData = new _CC_Model.ConsumptionData()
                        {
                            Data = _CC_CommonHelper.CustomDateFormate(Convert.ToDateTime(item.startTime).ToString(_d_formate), _d_formate, _dis_formate),
                            Data1 = item.value,
                        };

                        var dt = getWaterAIResponse.ReturnData.daily.Where(x => _CC_CommonHelper.CustomDateFormate(Convert.ToDateTime(x.date).ToString(_d_formate), _d_formate, _dis_formate) == formmatedData.Data).FirstOrDefault();

                        if (dt != null)
                        {
                            formmatedData.Data2 = dt.leak;
                            formmatedData.Data3 = dt.status;
                        }
                        consumptionDatas.Add(formmatedData);
                    }

                    consumptionDatas = consumptionDatas.OrderBy(x => Convert.ToInt32(x.Data)).ToList();
                    data.x_axis_jsonString = string.Join(",", consumptionDatas.Select(x => $"{_CC_CommonHelper.GetSMTranslation(_CC_CommonHelper.CustomDateFormate(x.Data, _dis_formate, "MMM"))} {_CC_CommonHelper.CustomDateFormate(x.Data, _dis_formate, "dd")}"));
                    data.y_axis_jsonString = string.Join(",", consumptionDatas.Select(x => x.Data1));
                    data.AbnormalJsonString = string.Join(",", consumptionDatas.Select(x => x.Data3 == "abnormal" ? 1 : 0));
                    data.AbnormalCount = consumptionDatas.Where(x => x.Data3 == "abnormal").Count();
                }
            }

            return data;
        }

        #endregion Functions

        #region [Survey Function]

        protected List<SelectListItem> GetDataSource(string sourceID, string childItemName = "", Language lang = null)
        {
            if (lang == null)
            {
                lang = ScContext.Language;
            }
            var item = ScContext.Database.GetItem(new ScData.ID(sourceID), lang);
            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(childItemName))
                {
                    return item.Children.Where(x => x.Name == childItemName).Select(c => new SelectListItem
                    {
                        Text = c.Fields["Text"].ToString(),
                        Value = c.Fields["Value"].ToString()
                    })?.ToList();
                }
                else
                {
                    return item.Children.Select(c => new SelectListItem
                    {
                        Text = c.Fields["Text"].ToString(),
                        Value = c.Fields["Value"].ToString()
                    })?.ToList();
                }
            }
            return new List<SelectListItem>();
        }

        protected string GetQuestionSection(List<DEWAXP.Foundation.Integration.SmartConsultantSvc.surveyQuestion> dataList, string qid)
        {
            try
            {
                int indx = dataList.IndexOf(dataList.FirstOrDefault(x => x.questionId == qid));
                if (0 <= indx - 1 && dataList.Count >= indx - 1)
                {
                    var filterData = dataList[indx - 1];

                    if (filterData != null && filterData.type == "Section")
                    {
                        return filterData.text;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return null;
        }

        #endregion [Survey Function]
    }

    public class ListtoDataTableConverter

    {
        public static DataTable ToDataTable<T>(List<T> items)

        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)

            {
                //Setting column names as Property names

                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)

            {
                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)

                {
                    //inserting property values to datatable rows

                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            //put a breakpoint here and check datatable

            return dataTable;
        }
    }
}