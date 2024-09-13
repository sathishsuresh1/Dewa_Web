using DEWAXP.Feature.SupplyManagement.Models.Track;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class TrackController : BaseController
    {
        #region Step-1 Search Notification Number and Contract Account

        // GET: Track
        [HttpGet, AllowAnonymous]
        public ActionResult TrackRequestAnonymous()
        {
            TrackRequestAnonymous _cacheModel = new TrackRequestAnonymous();
            // CacheProvider.TryGet(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, out _cacheModel);
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            return PartialView("~/Views/Feature/SupplyManagement/Track/_TrackRequestAnonymous.cshtml", _cacheModel);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult TrackRequestAnonymous(TrackRequestAnonymous model)
        {
            TrackRequestAnonymous _cacheModel = new TrackRequestAnonymous();
            bool IsSuccessful = false;
            try
            {
                //send otp
                var verifyRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.TrackVerifyOtpRequest()
                {
                    mode = "R",
                    lang = RequestLanguageCode,
                    sessionid = "",
                    reference = model.AccountOrRequestNumber,
                    prtype = "UCP",
                    contractaccountnumber = "",
                    businesspartner = "",
                };

                var returnData = TrackRequestAnonymousClient.VerifyOtp(verifyRequest, Request.Segment(), RequestLanguage);
                IsSuccessful = returnData.Succeeded &&
                        (Convert.ToInt32(returnData.Payload.emaillist?.Count ?? 0) > 0 || Convert.ToInt32(returnData.Payload.mobilelist?.Count ?? 0) > 0);
                if (IsSuccessful)
                {
                    var email = returnData.Payload.emaillist?.FirstOrDefault() ?? null;
                    var mobile = returnData.Payload.mobilelist?.FirstOrDefault() ?? null;
                    model.BusinessPartnerNumber = returnData.Payload.businesspartnernumber;
                    model.Flag = returnData.Payload.flag;
                    if (email != null && !string.IsNullOrWhiteSpace(email.unmaskedemail))
                    {
                        model.EmailAddress = email.unmaskedemail;
                        model.MaskedEmailAddress = email.maskedemail;
                    }
                    if (mobile != null && !string.IsNullOrWhiteSpace(mobile.unmaskedmobile))
                    {
                        model.MobileNumber = mobile.unmaskedmobile;
                        model.MaskedMobileNumber = mobile.maskedmobile.RemoveMobileNumberZeroPrefix();
                    }
                    model.ReferenceNumber = model.AccountOrRequestNumber;
                    if (!string.IsNullOrWhiteSpace(model.Flag) && model.Flag.ToLower().Equals("a"))
                        model.AccountNumber = model.AccountOrRequestNumber;

                    CacheProvider.Store(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, new CacheItem<TrackRequestAnonymous>(model, TimeSpan.FromMinutes(20)));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRACK_REQUEST_ANONYMOUS_DETAIL);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, returnData.Message);
                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/SupplyManagement/Track/_TrackRequestAnonymous.cshtml", model);
        }

        #endregion Step-1 Search Notification Number and Contract Account

        #region Step-2 Verify Mobile or Email

        [HttpGet, AllowAnonymous]
        public ActionResult TrackRequestAnonymousDetails()
        {
            TrackRequestAnonymous model;
            try
            {
                CacheProvider.TryGet(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, out model);
                if (model == null)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRACK_REQUEST_ANONYMOUS);
            }
            catch (Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRACK_REQUEST_ANONYMOUS);
            }
            return PartialView("~/Views/Feature/SupplyManagement/Track/_TrackRequestAnonymousDetails.cshtml", model);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult TrackRequestAnonymousDetails(TrackRequestAnonymous model)
        {
            TrackRequestAnonymous _cacheModel = new TrackRequestAnonymous();
            CacheProvider.TryGet(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, out _cacheModel);
            try
            {
                string mobile = null;
                string email = null;

                if (model.SelectedOptions == "email")
                {
                    email = _cacheModel.EmailAddress;
                    _cacheModel.SelectedOptions = "email";
                }
                else
                {
                    mobile = _cacheModel.MobileNumber;
                    _cacheModel.SelectedOptions = "mobile";
                }
                CacheProvider.Store(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, new CacheItem<TrackRequestAnonymous>(_cacheModel, TimeSpan.FromMinutes(20)));
                if (!string.IsNullOrWhiteSpace(_cacheModel.Flag) && _cacheModel.Flag.ToLower().Equals("n"))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRACK_REQUEST_VIEW_STATUS);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRACK_REQUEST_ANONYMOUS_LIST);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/SupplyManagement/Track/_TrackRequestAnonymousDetails.cshtml", model);
        }

        #endregion Step-2 Verify Mobile or Email

        #region Step-3 Notification list

        [HttpGet, AllowAnonymous]
        public ActionResult TrackRequestList()
        {
            TrackRequestAnonymous model = null;
            try
            {
                CacheProvider.TryGet(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, out model);
                if (model == null)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRACK_REQUEST_ANONYMOUS);
                var responseData = TrackRequestAnonymousClient.GetNotificationList(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.TrackListRequest
                {
                    complaintsIn = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.ComplaintsIn
                    {
                        businesspartner = model.BusinessPartnerNumber,
                        contractaccountnumber = model.AccountNumber,
                        email = model.SelectedOptions.Equals("email") ? model.EmailAddress : "",
                        mobilenumber = model.SelectedOptions.Equals("mobile") ? model.MobileNumber : "",
                        referencevalue = model.ReferenceNumber,
                        servicecode = "UC",
                    }
                }, Request.Segment(), RequestLanguage);

                if (responseData != null && responseData.Payload != null && responseData.Succeeded)
                {
                    model.TrackList = responseData.Payload.trackRequests;
                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return PartialView("~/Views/Feature/SupplyManagement/Track/_TrackRequestNotificationList.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TrackRequestStatus(TrackRequestAnonymous model)
        {
            try
            {
                bool IsSuccessful = false;
                TrackRequestAnonymous _cacheModel = new TrackRequestAnonymous();
                model.TrackStatusList = new List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack.TrackStatusList>();
                CacheProvider.TryGet(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, out _cacheModel);

                var verifyRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.TrackVerifyOtpRequest()
                {
                    mode = "R",
                    lang = RequestLanguageCode,
                    sessionid = "",
                    reference = model.NotificationNumber,
                    prtype = "UCP",
                    contractaccountnumber = "",
                    businesspartner = "",
                };

                var returnData = TrackRequestAnonymousClient.VerifyOtp(verifyRequest, Request.Segment(), RequestLanguage);
                IsSuccessful = returnData.Succeeded &&
                        (Convert.ToInt32(returnData.Payload.emaillist?.Count ?? 0) > 0 || Convert.ToInt32(returnData.Payload.mobilelist?.Count ?? 0) > 0);
                if (IsSuccessful)
                {
                    var email = returnData.Payload.emaillist?.FirstOrDefault() ?? null;
                    var mobile = returnData.Payload.mobilelist?.FirstOrDefault() ?? null;
                    string validateMob = _cacheModel.MobileNumber;
                    if (mobile != null && !mobile.unmaskedmobile.Equals(_cacheModel.MobileNumber))
                        validateMob = mobile.unmaskedmobile;

                    var responseData = TrackRequestAnonymousClient.GetGeneralTrackDetail(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.GeneralTrackRequest
                    {
                        trackrequestparams = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.TrackRequestParams
                        {
                            mobile = _cacheModel.SelectedOptions.Equals("mobile") ? validateMob : "",
                            email = _cacheModel.SelectedOptions.Equals("email") ? _cacheModel.EmailAddress : "",
                            notificationnumber = model.NotificationNumber
                        }
                    }, Request.Segment(), RequestLanguage);
                    if (responseData != null)
                    {
                        if (responseData.Succeeded)
                        {
                            model.TrackStatusList = responseData.Payload.tracklist;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView($"~/Views/Feature/SupplyManagement/Track/_TrackRequestStatus.cshtml", model);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult ViewStatus()
        {
            TrackRequestAnonymous model = null;
            try
            {
                CacheProvider.TryGet(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, out model);

                if (model == null)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRACK_REQUEST_ANONYMOUS);

                model.TrackStatusList = new List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.AnonymousTrack.TrackStatusList>();
                var responseData = TrackRequestAnonymousClient.GetGeneralTrackDetail(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.GeneralTrackRequest
                {
                    trackrequestparams = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.AnonymousTrack.TrackRequestParams
                    {
                        lang = RequestLanguageCode,
                        mobile = model.MobileNumber,
                        email = model.EmailAddress,
                        notificationnumber = model.ReferenceNumber
                    }
                }, Request.Segment(), RequestLanguage);
                if (responseData != null)
                {
                    if (responseData.Succeeded)
                    {
                        model.NotificationNumber = responseData.Payload.notificationnumber;
                        model.EvCardNo = responseData.Payload.evcardno;
                        model.EvNotification = responseData.Payload.evnotification;
                        model.EvStatus = responseData.Payload.evstatus;
                        model.EvStatusDescription = responseData.Payload.evstatusdescription;
                        model.NotificationType = responseData.Payload.notificationtypetext;
                        model.NotificationShortText = responseData.Payload.shorttext;
                        model.TrackStatusList = responseData.Payload.tracklist;
                    }
                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/SupplyManagement/Track/_TrackStatus.cshtml", model);
        }

        #endregion Step-3 Notification list

        #region Verified Email/Mobile

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VerifyEmailMobile(string ivType, string mobile, string email)
        {
            string responseMessage = string.Empty;
            string responseCode = "000";
            TrackRequestAnonymous model = new TrackRequestAnonymous();
            if (!string.IsNullOrWhiteSpace(ivType) && (!string.IsNullOrEmpty(mobile) || !string.IsNullOrEmpty(email)))
            {
                CacheProvider.TryGet(CacheKeys.TRACK_REQUEST_ANONYMOUS_RESPONSE, out model);
                if (ivType.Equals("E") && !model.EmailAddress.ToLower().Equals(email.ToLower()))
                {
                    responseMessage = Translate.Text("TRA_EmailValidation");
                    responseCode = "999";
                }

                if (ivType.Equals("M") && !model.MobileNumber.RemoveMobileNumberZeroPrefix().Equals(mobile))
                {
                    responseMessage = Translate.Text("TRA_MobileValidation");
                    responseCode = "999";
                }
            }
            else
            {
                if (ivType.Equals("M"))
                {
                    responseMessage = Translate.Text("TRA_MobileValidation");
                    responseCode = "999";
                }
                else
                {
                    responseMessage = Translate.Text("TRA_EmailValidation");
                    responseCode = "999";
                }
            }
            return Json(new { Message = responseMessage, errorCode = responseCode });
        }

        #endregion Verified Email/Mobile
    }
}