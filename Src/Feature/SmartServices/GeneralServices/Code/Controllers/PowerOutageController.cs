using Sitecore.Globalization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    using DEWAXP.Foundation.Content;
    #region [self assembly namespace]

    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Filters.Mvc;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Content.Utils;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Logger;
    using Models.PowerOutage;

    #endregion [self assembly namespace]

    [TwoPhaseAuthorize]
    public class PowerOutageController : BaseController
    {
        private long FileSize = (3 * 1024 * 1024);
        private string[] FileType = { ".PDF", ".JPG", ".JPEG", ".PNG", ".BMP", };

        // GET: PowerOutage

        #region [Actions]

        [HttpGet]
        public ActionResult PO_NewRequest()
        {
            PoweOutageNewRequest model = new PoweOutageNewRequest();
            var listData = GetOutageDropDetailsList();
            if (listData != null)
            {
                model.PowerInterruptionList = listData.Interruption.InterruptionItem.Select(x => new SelectListItem { Text = x.InterruptionText, Value = x.InterruptionCode }).ToList();
                model.PurposeOfWorkList = listData.Work.WorkItem?.ToList();
                model.TypeOfWorkList = listData.Outage.OutageItem.Select(x => new SelectListItem { Text = x.OutageText, Value = x.OutageCode }).ToList();
            }

            return View("~/Views/Feature/GeneralServices/PowerOutage/PO_NewRequest.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PO_NewRequest(PoweOutageNewRequest model)
        {
            if (ModelState.IsValid)
            {
                var OutageRequest = new DEWAXP.Foundation.Integration.DewaSvc.SetOutageRequest()
                {
                    contractaccount = model.AccountNo,
                    mobilenumber = model.MobileNumber?.AddMobileNumberZeroPrefix(),
                    startdate = CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(model.StartDate), "dd MMMM yyyy").ToString("yyyyMMdd"), //Convert.ToDateTime(model.StartDate).ToString("yyyyMMdd"),
                    starttime = DateTime.Parse(model.StartTime?.Replace("صباحاً", "AM").Replace("مساءً", "PM")).ToString("HH:mm:ss"),
                    enddate = CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(model.EndDate), "dd MMMM yyyy").ToString("yyyyMMdd"),
                    endtime = DateTime.Parse(model.EndTime?.Replace("صباحاً", "AM").Replace("مساءً", "PM")).ToString("HH:mm:ss"),
                    interruptiontype = model.PowerInterruption,
                    outagetype = model.TypeOfWork,
                    purposeofwork = model.PurposeOfWork,
                    workpermitholdername = model.CustomerAuthorizedPersonName,
                    substationnumber = model.DEWASubStationNumber,
                    isolationpoint = model.IsolationPoint,
                    companyname = model.CompanyName,
                    email = model.EmailID,
                };

                DateTime startDateTime = CommonUtility.DateTimeFormatParse($"{OutageRequest.startdate} {OutageRequest.starttime}", "yyyyMMdd HH:mm:ss");
                DateTime endDateTime = CommonUtility.DateTimeFormatParse($"{OutageRequest.enddate} {OutageRequest.endtime}", "yyyyMMdd HH:mm:ss");

                if (!Convert.ToBoolean(startDateTime.Subtract(Convert.ToDateTime(DateTime.Now.AddDays(4).ToShortDateString())).TotalDays >= 0))
                {
                    ModelState.AddModelError("", Translate.Text("pwo_datetime_note"));
                }
                if (ModelState.IsValid)
                {
                    if (endDateTime.Subtract(startDateTime).TotalHours < 1)
                    {
                        ModelState.AddModelError("", Translate.Text("pwo_DateDiffErrorMessage"));
                    }
                }
                if (ModelState.IsValid)
                {
                    string error = null;
                    if (model.CompanyLetterAttachement != null && model.CompanyLetterAttachement.ContentLength > 0)
                    {
                        if (CustomeAttachmentIsValid(model.CompanyLetterAttachement, FileSize, out error, FileType))
                        {
                            OutageRequest.file1name = model.CompanyLetterAttachement.FileName;
                            OutageRequest.file1content = model.CompanyLetterAttachement.ToArray();
                        }
                        else
                        {
                            ModelState.AddModelError("", error);
                            ModelState.AddModelError("CompanyLetterAttachement", error);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", Translate.Text("pwo_CompanyLetterAttachementRequired"));
                    }

                    if (string.IsNullOrWhiteSpace(error) && ModelState.IsValid)
                    {
                        if (model.MethodofStatementAttachment != null && model.MethodofStatementAttachment.ContentLength > 0)
                        {
                            if (CustomeAttachmentIsValid(model.MethodofStatementAttachment, FileSize, out error, FileType))
                            {
                                OutageRequest.file2name = model.MethodofStatementAttachment.FileName;
                                OutageRequest.file2content = model.MethodofStatementAttachment.ToArray();
                            }
                            else
                            {
                                ModelState.AddModelError("", error);
                                ModelState.AddModelError("MethodofStatementAttachment", error);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("pwo_MethodofStatementAttachmentRequired"));
                        }
                    }

                    if (string.IsNullOrWhiteSpace(error) && ModelState.IsValid)
                    {
                        if (model.RiskAssessmentAttachment != null && model.RiskAssessmentAttachment.ContentLength > 0)
                        {
                            if (CustomeAttachmentIsValid(model.RiskAssessmentAttachment, FileSize, out error, FileType))
                            {
                                OutageRequest.file3name = model.RiskAssessmentAttachment.FileName;
                                OutageRequest.file3content = model.RiskAssessmentAttachment.ToArray();
                            }
                            else
                            {
                                ModelState.AddModelError("", error);
                                ModelState.AddModelError("RiskAssessmentAttachment", error);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("pwo_RiskAssessmentAttachmentRequired"));
                        }
                    }

                    if (string.IsNullOrWhiteSpace(error) && ModelState.IsValid)
                    {
                        if (model.CustomerOutageRequestForm != null && model.CustomerOutageRequestForm.ContentLength > 0)
                        {
                            if (CustomeAttachmentIsValid(model.CustomerOutageRequestForm, FileSize, out error, FileType))
                            {
                                OutageRequest.file4name = model.CustomerOutageRequestForm.FileName;
                                OutageRequest.file4content = model.CustomerOutageRequestForm.ToArray();
                            }
                            else
                            {
                                ModelState.AddModelError("", error);
                                ModelState.AddModelError("CustomerOutageRequestForm", error);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", Translate.Text("pwo_CustomerOutageRequestFormRequired"));
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    var requestResponse = DewaApiClient.SetOutageRequest(OutageRequest, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                    if (requestResponse != null && requestResponse.Succeeded)
                    {
                        return Redirect(string.Format("{0}?n={1}",
                                        LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PowerOutageRequestConfirmationPage),
                                        requestResponse.Payload.@return.notificationNumber));
                    }
                    else
                    {
                        ModelState.AddModelError("", requestResponse.Message);
                    }
                }
            }
            var listData = GetOutageDropDetailsList();
            if (listData != null)
            {
                model.PowerInterruptionList = listData.Interruption.InterruptionItem.Select(x => new SelectListItem { Text = x.InterruptionText, Value = x.InterruptionCode }).ToList();
                model.PurposeOfWorkList = listData.Work.WorkItem?.ToList();
                model.TypeOfWorkList = listData.Outage.OutageItem.Select(x => new SelectListItem { Text = x.OutageText, Value = x.OutageCode }).ToList();
            }
            model.MobileNumber = model.MobileNumber.RemoveMobileNumberZeroPrefix();
            return View("~/Views/Feature/GeneralServices/PowerOutage/PO_NewRequest.cshtml",model);
        }

        [HttpGet]
        public ActionResult PO_TrackListingPage(string n)
        {
            PowerOutageTrackingModel model = new PowerOutageTrackingModel()
            {
                NotificationNo = n,
                IsSearched = string.IsNullOrEmpty(n)
            };

            if (!model.IsSearched)
            {
                var requestResponse = DewaApiClient.GetOutageTracker(new DEWAXP.Foundation.Integration.DewaSvc.GetOutageTracker()
                {
                    notificationnumber = model.NotificationNo
                }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (requestResponse != null && requestResponse.Succeeded)
                {
                    model.TrackingData = requestResponse.Payload.@return.trackList?.Select(x => new PowerOutageTackItem()
                    {
                        AccountNo = x.contractAccount,
                        EndDate = x.endDate,
                        EndTime = x.endTime,
                        StartDate = x.startDate,
                        StartTime = x.startTime,
                        Status = x.status,
                        CustomerAuthorizedPersonName = x.WPHNAME,
                        NotificationNo = x.notificationNumber,
                        DEWASubStationNumber = x.subStationNumber,
                        MobileNumber = x.mobileNumber,
                        PowerInterruption = x.interruptionType,
                        TypeOfWork = x.outageType,
                        PurposeOfWork = x.workDescription
                    }).ToList();
                }
                else
                {
                    ModelState.AddModelError("", requestResponse.Message);
                }
            }
            return View("~/Views/Feature/GeneralServices/PowerOutage/PO_TrackListingPage.cshtml",model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PO_TrackListingPage(PowerOutageTrackingModel model)
        {
            var requestResponse = DewaApiClient.GetOutageTracker(new DEWAXP.Foundation.Integration.DewaSvc.GetOutageTracker()
            {
                notificationnumber = model.NotificationNo
            }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            if (requestResponse != null && requestResponse.Succeeded)
            {
                model.TrackingData = requestResponse.Payload.@return.trackList?.Select(x => new PowerOutageTackItem()
                {
                    AccountNo = x.contractAccount,
                    EndDate = x.endDate,
                    EndTime = x.endTime,
                    StartDate = x.startDate,
                    StartTime = x.startTime,
                    Status = x.status,
                    CustomerAuthorizedPersonName = x.WPHNAME,
                    NotificationNo = x.notificationNumber,
                    DEWASubStationNumber = x.subStationNumber,
                    MobileNumber = x.mobileNumber,
                    PowerInterruption = x.interruptionType,
                    TypeOfWork = x.outageType,
                    PurposeOfWork = x.workDescription
                }).ToList();
            }
            else
            {
                ModelState.AddModelError("", requestResponse.Message);
            }
            model.IsSearched = true;
            return View("~/Views/Feature/GeneralServices/PowerOutage/PO_TrackListingPage.cshtml",model);
        }

        [HttpGet]
        public ActionResult PO_Confirmation(string n)
        {
            PowerOutageTrackingModel model = new PowerOutageTrackingModel()
            {
                NotificationNo = n,
            };

            var requestResponse = DewaApiClient.GetOutageTracker(new DEWAXP.Foundation.Integration.DewaSvc.GetOutageTracker()
            {
                notificationnumber = model.NotificationNo
            }, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            if (requestResponse != null && requestResponse.Succeeded)
            {
                model.TrackingData = requestResponse.Payload.@return.trackList?.Select(x => new PowerOutageTackItem()
                {
                    AccountNo = x.contractAccount,
                    EndDate = x.endDate,
                    EndTime = x.endTime,
                    StartDate = x.startDate,
                    StartTime = x.startTime,
                    Status = x.status,
                    CustomerAuthorizedPersonName = x.WPHNAME,
                    NotificationNo = x.notificationNumber,
                    DEWASubStationNumber = x.subStationNumber,
                    MobileNumber = x.mobileNumber,
                    PowerInterruption = x.interruptionType,
                    TypeOfWork = x.outageType,
                    PurposeOfWork = x.workDescription,
                    CompanyName = x.companyname
                }).ToList();
            }

            if (!(string.IsNullOrWhiteSpace(n) && model.TrackingData != null && model.TrackingData.Count > 0))
            {
                ModelState.AddModelError("", Translate.Text("pwo_successpagegenericerror"));
            }
            return View("~/Views/Feature/GeneralServices/PowerOutage/PO_Confirmation.cshtml",model);
        }

        [HttpGet]
        public ActionResult ValidateDateTime(PoweOutageNewRequest model)
        {
            bool IsValid = false;
            string Message = "";
            try
            {
                string startdate = CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(model.StartDate), "dd MMMM yyyy").ToString("yyyyMMdd");
                string starttime = DateTime.Parse(model.StartTime?.Replace("صباحاً", "AM").Replace("مساءً", "PM")).ToString("HH:mm:ss");
                DateTime startDateTime = CommonUtility.DateTimeFormatParse($"{startdate} {starttime}", "yyyyMMdd HH:mm:ss");
                if (!model.IsCheckStart)
                {
                    string enddate = CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(model.EndDate), "dd MMMM yyyy").ToString("yyyyMMdd");
                    string endtime = DateTime.Parse(model.EndTime?.Replace("صباحاً", "AM").Replace("مساءً", "PM")).ToString("HH:mm:ss");
                    DateTime endDateTime = CommonUtility.DateTimeFormatParse($"{enddate} {endtime}", "yyyyMMdd HH:mm:ss");
                    IsValid = !Convert.ToBoolean(endDateTime.Subtract(startDateTime).TotalHours < 1);
                    Message = !IsValid ? Translate.Text("pwo_DateDiffErrorMessage") : "";
                }
                else
                {
                    IsValid = startDateTime.Subtract(Convert.ToDateTime(DateTime.Now.AddDays(4).ToShortDateString())).TotalDays >= 0;
                    Message = !IsValid ? Translate.Text("pwo_datetime_note") : "";
                }
            }
            catch (Exception ex)
            {
                Message = model.IsCheckStart ? Translate.Text("pwo_datetime_note") : Translate.Text("pwo_DateDiffErrorMessage");
                LogService.Error(ex, this);
            }

            return Json(new { IsSuccess = IsValid, Description = !IsValid ? Message : "" }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Actions]

        #region [Methods]

        public DEWAXP.Foundation.Integration.Responses.PowerOutage.GetOutageDropDetailsXMLResponse GetOutageDropDetailsList()
        {
            DEWAXP.Foundation.Integration.Responses.PowerOutage.GetOutageDropDetailsXMLResponse returnDataList = null;
            if (!CacheProvider.TryGet("GetOutageDropDetailsList_" + Translate.Text("lang"), out returnDataList))
            {
                var returndata = DewaApiClient.GetOutageDropDetails(new DEWAXP.Foundation.Integration.DewaSvc.GetOutageDropDetails(), CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (returndata != null && returndata.Succeeded)
                {
                    returnDataList = returndata.Payload;
                    CacheProvider.Store("GetOutageDropDetailsList_" + Translate.Text("lang"), new CacheItem<DEWAXP.Foundation.Integration.Responses.PowerOutage.GetOutageDropDetailsXMLResponse>(returndata.Payload, TimeSpan.FromMinutes(20)));
                }
            }

            return returnDataList;
        }

        #endregion [Methods]
    }
}