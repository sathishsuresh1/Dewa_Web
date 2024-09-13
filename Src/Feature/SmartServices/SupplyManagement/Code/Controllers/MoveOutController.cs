using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.MoveOut;
using DEWAXP.Foundation.Content.Models.MoveOut.v3;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.DataAnnotations;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Glass.Mapper.Sc;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MoveOutAccount = DEWAXP.Foundation.Content.Models.MoveOut.MoveOutAccount;
using SitecoreX = Sitecore.Context;
using Sitecore.Mvc.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.ForgotPassword;
using DEWAXP.Foundation.Content.Models.UpdateIBAN;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Content.Models.SupplyManagement.MoveOut;
using DEWAXP.Foundation.Integration.SmartVendorSvc;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class MoveOutController : BaseController
    {
        #region Enum MoveOutAnonymousOTP

        private enum OTPFlags
        {
            FV1 = 0,
            SOT = 1,
            VOT = 2
        }

        #endregion Enum MoveOutAnonymousOTP

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ConfirmV3()
        {
            SetMoveOutRequestResponse moveOutState;
            MoveOutState state;
            MoveOutConfirm moveOutConfirmModel;

            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }

            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_CONFIRM, out moveOutState))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }

            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_CONFIRM_ACCOUNTS, out moveOutConfirmModel))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }

            if (state.moveoutresult.issuccess)
            {
                CacheProvider.Remove(CacheKeys.MOVE_OUT_CONFIRM);
                CacheProvider.Remove(CacheKeys.MOVE_OUT_CONFIRM_ACCOUNTS);
                CacheProvider.Remove(CacheKeys.MOVE_OUT_SELECTEDACCOUNTS);
                CacheProvider.Remove(CacheKeys.MOVE_OUT_MOB_EML_LST);
                var context = ContentRepository.GetItem<DEWAXP.Foundation.Content.Models.AccountSelector>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{0E59E02C-CC36-4882-9FDD-CFBFC7ED2B5F}")));

                if (!string.IsNullOrEmpty(context.NotificationCode))
                {
                    CacheProvider.Remove(context.NotificationCode);
                }
                CacheProvider.Remove(CacheKeys.MOVE_OUT_RESULT);
            }

            CacheProvider.Store("MoveOutIsSuccess", new CacheItem<bool>(state.moveoutresult.issuccess));

            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutConfirm.cshtml", new ConfirmModel
            {
                Accounts = moveOutConfirmModel.SelectedAccounts,
                Notifications = moveOutState.@return.notificationlist.Select(x => new ConfirmNotificationModel { ContractAccountNumber = x.contractaccountnumber, Message = x.message, NotificationNumber = x.notificationnumber }).ToArray(),
                IsSuccess = state.moveoutresult.issuccess,
                Message = state.moveoutdetails.description,
                ErrorMessage = state.moveoutresult.errormessage,
                AdditionalInformation = moveOutState.@return.additionalinformation
            });
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DemolishConfirmV3()
        {
            SetMoveOutRequestResponse moveOutState;

            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_DEMOLISH_CONFIRM, out moveOutState))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_DEMOLISH_START_V3);
            }
            CacheProvider.Remove(CacheKeys.ACCOUNT_LIST);
            CacheProvider.Remove(CacheKeys.ACCOUNT_LIST_WITH_BILLING);
            CacheProvider.Remove(CacheKeys.MOVE_OUT_DEMOLISH_CONFIRM);
            var context = ContentRepository.GetItem<DEWAXP.Foundation.Content.Models.AccountSelector>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{5FF8F10F-A24F-4E1C-B426-934381432C66}")));

            if (!string.IsNullOrEmpty(context.NotificationCode))
            {
                CacheProvider.Remove(context.NotificationCode);
            }
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolishConfirm.cshtml", new ConfirmModel
            {
                Notifications = moveOutState.@return.notificationlist.Select(x => new ConfirmNotificationModel { ContractAccountNumber = x.contractaccountnumber, Message = x.message, NotificationNumber = x.notificationnumber }).ToArray(),
                AdditionalInformation = moveOutState.@return.additionalinformation,
                //ReferenceNumbers = moveOutState.@return.notificationlist.Select(x => x.referencenumber).ToArray(),
                //Message = moveOutState.@return.description,
            });
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DemolishConfirmGovV3()
        {
            SetMoveOutRequestResponse moveOutState;

            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_DEMOLISH_GOV_CONFIRM, out moveOutState))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_DEMOLISH_GOV_START_V3);
            }
            var context = ContentRepository.GetItem<DEWAXP.Foundation.Content.Models.AccountSelector>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{5FF8F10F-A24F-4E1C-B426-934381432C66}")));

            if (!string.IsNullOrEmpty(context.NotificationCode))
            {
                CacheProvider.Remove(context.NotificationCode);
            }

            //CacheProvider.Remove(CacheKeys.ACCOUNT_LIST_WITH_BILLING);
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolishGovConfirm.cshtml", new ConfirmModel
            {
                Notifications = moveOutState.@return.notificationlist.Select(x => new ConfirmNotificationModel { ContractAccountNumber = x.contractaccountnumber, Message = x.message, NotificationNumber = x.notificationnumber }).ToArray(),
                AdditionalInformation = moveOutState.@return.additionalinformation,
                //ReferenceNumbers = moveOutState.@return.notificationlist.Select(x => x.referencenumber).ToArray(),
                //Message = moveOutState.@return.description,
            });
        }

        #region Version 3

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = true)]
        public ActionResult MoveOut_v3()
        {
            string errorMessage;
            string selectedaccounts;
            var model = new MoveOutAccount();
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_SELECTEDACCOUNTS, out selectedaccounts))
            {
                model.dewaglobalmultiselect = selectedaccounts;
            }
            CacheProvider.Remove(CacheKeys.MOVE_OUT_MOB_EML_LST);
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutAccounts.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = true)]
        public ActionResult MoveOut_v3(MoveOutAccount model)
        {
            MoveOutState state;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }
            CacheProvider.Store(CacheKeys.MOVE_OUT_SELECTEDACCOUNTS, new CacheItem<string>(model.dewaglobalmultiselect, TimeSpan.FromMinutes(20)));
            if (!state.moveoutresult.proceed)
            {
                string PaymentPath = "MoveoutPayment";
                CacheProvider.Store(CacheKeys.MOVEOUT_PAYMENT_PATH, new CacheItem<string>(PaymentPath));

                #region [MIM Payment Implementation]

                var payRequest = new CipherPaymentModel();
                payRequest.PaymentData.amounts = state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).Any() ? state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).Select(x => x.amounttocollect).Aggregate((i, j) => i + "," + j) : "0";
                payRequest.PaymentData.contractaccounts = state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).Any() ? state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).Select(x => x.contractaccountnumber).Aggregate((i, j) => i + "," + j) : string.Empty;

                if (state.moveoutdetails.divisionlist != null && state.moveoutdetails.divisionlist.Count() > 0)
                {
                    if (!string.IsNullOrWhiteSpace(payRequest.PaymentData.amounts) && payRequest.PaymentData.amounts != "0")
                    {
                        payRequest.PaymentData.amounts = payRequest.PaymentData.amounts + "," + state.moveoutdetails.divisionlist.Select(x => x.totalamount).Aggregate((i, j) => i + "," + j);
                        payRequest.PaymentData.contractaccounts = payRequest.PaymentData.contractaccounts + "," + state.moveoutdetails.divisionlist.Select(x => x.contractaccountnumber).Aggregate((i, j) => i + "," + j);
                    }
                    else
                    {
                        payRequest.PaymentData.amounts = state.moveoutdetails.divisionlist.Select(x => x.totalamount).Aggregate((i, j) => i + "," + j);
                        payRequest.PaymentData.contractaccounts = state.moveoutdetails.divisionlist.Select(x => x.contractaccountnumber).Aggregate((i, j) => i + "," + j);
                    }
                }

                payRequest.ServiceType = ServiceType.MoveOut;
                payRequest.PaymentMethod = model.paymentMethod;
                payRequest.BankKey = model.bankkey;
                payRequest.SuqiaValue = model.SuqiaDonation;
                payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                var payResponse = ExecutePaymentGateway(payRequest);
                if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                {
                    return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                }
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(string.Join("\n", payResponse.ErrorMessages.Values.ToList())));

                #endregion [MIM Payment Implementation]
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_DETAILS_V3);
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = true)]
        public ActionResult MoveOutSetDetails_v3()
        {
            MoveOutState state;
            List<MoveoutDetailsv3> lstmoveoutdetails = new List<MoveoutDetailsv3>();

            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }

            ModelStateDictionary errorModel;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MODEL, out errorModel))
            {
                var errors = errorModel.Where(n => n.Value.Errors.Count > 0)
                    .SelectMany(a => a.Value.Errors)
                    .Select(error => error.ErrorMessage);

                if (errors != null && errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                CacheProvider.Remove(CacheKeys.ERROR_MODEL);
            }

            var ibanResponse = DewaApiClient.IBANList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, "", state.moveoutdetails.accountslist.FirstOrDefault().businesspartnernumber, RequestLanguage, Request.Segment());

            List<SelectListItem> ibanList = null;
            if (ibanResponse.Succeeded)
            {
                ibanList = new List<SelectListItem>();
                foreach (var iban in ibanResponse.Payload.IBAN)
                {
                    ibanList.Add(new SelectListItem { Text = iban.maskIban, Value = iban.iban.Replace("AE", "") });
                }

                if (ibanList.Any())
                    ibanList.Add(new SelectListItem { Text = Translate.Text("Others"), Value = "others" });
            }

            Array.ForEach(state.moveoutdetails.accountslist.ToArray(), x => lstmoveoutdetails.Add(
                new MoveoutDetailsv3
                {
                    contractaccountnumber = x.contractaccountnumber,
                    contractaccountname = x.contractaccountname,
                    customerfirstname = x.customerfirstname,
                    customerlastname = x.customerlastname,
                    customeremailid = x.maskedemail,
                    Mobile = x.mobile,
                    MaskedMobile = x.maskedmobile,
                    Email = x.email,
                    MaskedEmail = x.maskedemail,
                    businesspartnercategory = x.businesspartnercategory,
                    businesspartnernumber = x.businesspartnernumber,
                    RefundOptions = PopulateMoveoutRefundOptions(x.okiban, x.okcheque, x.okaccounttransfer, x.okwesternunion, x.oknorefund).Item1,
                    refund = PopulateMoveoutRefundOptions(x.okiban, x.okcheque, x.okaccounttransfer, x.okwesternunion, x.oknorefund).Item2,
                    transferaccount = state.moveoutdetails.trnsferlist != null && state.moveoutdetails.trnsferlist.FirstOrDefault() != null &&
                       !string.IsNullOrWhiteSpace(state.moveoutdetails.trnsferlist.FirstOrDefault().contractaccountnumber) ? state.moveoutdetails.trnsferlist.FirstOrDefault().contractaccountnumber : string.Empty,
                    IbanList = ibanList,
                    ValidBankCodes = (state.moveoutdetails.banklist != null && state.moveoutdetails.banklist.Any()) ? new Tuple<string, string>(string.Join(",", state.moveoutdetails.banklist.Where(y => y.bptype == x.businesspartnercategory && !y.bankkey.Equals("*")).Select(y => y.bankkey)), string.Join(", ", state.moveoutdetails.banklist.Where(y => y.bptype == x.businesspartnercategory && !y.bankkey.Equals("*")).Select(y => y.bankname))) : new Tuple<string, string>("", "")
                }));

            var isChequeRefundForAll = false;
            var isIbanRefundForAll = false;
            var isTransferRefundForAll = false;
            var isWesternRefundForAll = false;
            var isNoRefundForAll = false;
            var chequeRefundCount = 0;
            var ibanRefundCount = 0;
            var transferRefundCount = 0;
            var westernRefundCount = 0;
            var noRefundCount = 0;

            foreach (var account in state.moveoutdetails.accountslist.ToArray())
            {
                if (account.okcheque == "Y")
                    chequeRefundCount++;

                if (account.okiban == "Y")
                    ibanRefundCount++;

                if (account.okaccounttransfer == "Y")
                    transferRefundCount++;

                if (account.okwesternunion == "Y")
                    westernRefundCount++;

                if (account.oknorefund == "Y")
                    noRefundCount++;
            }

            if (state.moveoutdetails.accountslist.Count() == chequeRefundCount)
                isChequeRefundForAll = true;

            if (state.moveoutdetails.accountslist.Count() == ibanRefundCount)
                isIbanRefundForAll = true;

            if (state.moveoutdetails.accountslist.Count() == transferRefundCount)
                isTransferRefundForAll = true;

            if (state.moveoutdetails.accountslist.Count() == westernRefundCount)
                isWesternRefundForAll = true;

            if (state.moveoutdetails.accountslist.Count() == noRefundCount)
                isNoRefundForAll = true;

            //List<transferAccountsOut> lsttransferaccounts = new List<transferAccountsOut>();
            //lsttransferaccounts.Add(new transferAccountsOut() { contractaccountname = Translate.Text("moveout.selectaccount"), contractaccountnumber = "0" });
            //if (state.moveoutdetails.trnsferlist != null && state.moveoutdetails.trnsferlist.Count() > 0)
            //{
            //    Array.ForEach(state.moveoutdetails.trnsferlist, x => lsttransferaccounts.Add(x));
            //}
            List<MoveOutTransferAccountsResponse> lsttransferaccounts = null;
            if (state.moveoutdetails.trnsferlist != null && state.moveoutdetails.trnsferlist.Count() > 0)
            {
                lsttransferaccounts = state.moveoutdetails.trnsferlist.ToList();
                lsttransferaccounts.Insert(0, new MoveOutTransferAccountsResponse() { contractaccountname = Translate.Text("refund.selectaccount"), contractaccountnumber = "0" });
            }
            var attachmentMandatory = false;
            if (!string.IsNullOrWhiteSpace(state.moveoutdetails.attachmentmandatory)
                && state.moveoutdetails.attachmentmandatory.ToLower().Equals("x"))
                attachmentMandatory = true;

            var holidayResponse = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
            {
                lang = RequestLanguage.Code(),
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                holidayflag = "X",
                shiftflag = "X"
            }, Request.Segment());
            List<string> holidayList = new List<string>();
            var moveoutconfigitem = ContentRepository.GetItem<MoveoutDisconnectionTime>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.MOVEOUT_DISCONNECTIONTIME_CONFIG)));
            if (holidayResponse != null && holidayResponse.Payload != null)
            {
                holidayList = holidayResponse.Payload.holidayList.Select(x => x.holidaydate).ToList();
                if (holidayResponse.Payload.shiftstarttime != null && holidayResponse.Payload.shiftendtime != null)
                {
                    moveoutconfigitem.FromTime = Convert.ToDateTime(holidayResponse.Payload.shiftstarttime);
                    moveoutconfigitem.ToTime = Convert.ToDateTime(holidayResponse.Payload.shiftendtime);
                }
                CacheProvider.Store(CacheKeys.MOVE_OUT_HOLIDAYS, new CacheItem<List<string>>(holidayList));
            }

            List<SelectListItem> MobileList = new List<SelectListItem>();
            List<SelectListItem> EmailList = new List<SelectListItem>();
            state.moveoutdetails.accountslist.ForEach(x =>
            {
                if (MobileList.Count() == 0 || !MobileList.Any(m => m.Text == x.maskedmobile))
                    MobileList.Add(new SelectListItem { Text = x.maskedmobile, Value = x.contractaccountnumber });
            });
            state.moveoutdetails.accountslist.ForEach(x =>
            {
                if (EmailList.Count() == 0 || !EmailList.Any(m => m.Text == x.maskedemail))
                    EmailList.Add(new SelectListItem { Text = x.maskedemail, Value = x.contractaccountnumber });
            });
            MobileList = MobileList?.Distinct().ToList();
            EmailList = EmailList?.Distinct().ToList();
            MoveOutMobileEmailList moveOutMobileEmailList = new MoveOutMobileEmailList();
            moveOutMobileEmailList.MobileItems = MobileList;
            moveOutMobileEmailList.EMailItems = EmailList;
            CacheProvider.Store(CacheKeys.MOVE_OUT_MOB_EML_LST, new CacheItem<MoveOutMobileEmailList>(moveOutMobileEmailList));
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDetails.cshtml",
                new LstMoveoutAccount
                {
                    DisconnectionCurrentTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, false),
                    DisconnectionTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, true),
                    DisconnectionDateTimeNotes = moveoutconfigitem.Notes,
                    ShiftStartTime = moveoutconfigitem.FromTime,
                    ShiftEndTime = moveoutconfigitem.ToTime,
                    TimeFormat = moveoutconfigitem.TimeFormat,
                    TimeInterval = moveoutconfigitem.TimeInterval,
                    lstdetails = lstmoveoutdetails.OrderBy(x => x.refund ? 0 : 1).ToList(),
                    lsttranferaccount = lsttransferaccounts,
                    IsAttachmentMandatory = attachmentMandatory,
                    IsChequeRefundForAll = isChequeRefundForAll,
                    IsIbanRefundForAll = isIbanRefundForAll,
                    IsTransferRefundForAll = isTransferRefundForAll,
                    IsWesternUnionForAll = isWesternRefundForAll,
                    IsNoRefundForAll = isNoRefundForAll,
                    HolidayList = holidayList,
                    MobileList = MobileList,
                    EmailList = EmailList
                });
        }

        private List<SelectListItem> GetDisconnectionTime(DateTime startTime, DateTime endTime, TimeSpan interval, string timeformat, bool allOption)
        {
            List<SelectListItem> timeOption = new List<SelectListItem>();
            DateTime time = startTime;
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            while (time <= endTime)
            {
                if (!allOption)
                {
                    if (TimeSpan.Parse(time.ToString("HH:mm")) >= currentTime)
                        timeOption.Add(new SelectListItem { Text = time.ToString(timeformat, SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = time.ToString("HH:mm:ss") });
                }
                else
                {
                    timeOption.Add(new SelectListItem { Text = time.ToString(timeformat, SitecoreX.Culture).Replace("ص", "صباحاً").Replace("م", "مساءً"), Value = time.ToString("HH:mm:ss") });
                }
                time = time.Add(interval);
            }
            return timeOption;
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult MoveOutSetDetails_v3(LstMoveoutAccount model)
        {
            MoveOutState state;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }
            if (!validateMoveout(model))
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_DETAILS_V3);
                //return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDetails.cshtml", model);
            }

            model.Attachment = new byte[0];
            string error = string.Empty;
            if (model.RefundDocument != null && model.RefundDocument.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.RefundDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                    CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                    return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDetails.cshtml", model);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.RefundDocument.InputStream.CopyTo(memoryStream);
                        model.Attachment = memoryStream.ToArray();
                    }
                }
            }
            List<string> holidayList = new List<string>();
            if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_HOLIDAYS, out holidayList))
            {
                if (holidayList != null)
                {
                    model.HolidayList = holidayList;
                }
                if (model.ShiftStartTime != null && model.ShiftEndTime != null && model.TimeInterval != null && model.TimeFormat != null)
                {
                    model.DisconnectionCurrentTimeOptions = GetDisconnectionTime(model.ShiftStartTime, model.ShiftEndTime, new TimeSpan(0, model.TimeInterval != null ? Convert.ToInt32(model.TimeInterval) : 15, 0), model.TimeFormat, false);
                    model.DisconnectionTimeOptions = GetDisconnectionTime(model.ShiftStartTime, model.ShiftEndTime, new TimeSpan(0, model.TimeInterval != null ? Convert.ToInt32(model.TimeInterval) : 15, 0), model.TimeFormat, true);
                }
            }
            MoveOutMobileEmailList moveOutMobileEmailList = new MoveOutMobileEmailList();
            if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_MOB_EML_LST, out moveOutMobileEmailList))
            {
                moveOutMobileEmailList.SelectedEmail = model.lstdetails.Where(x => x.contractaccountnumber == model.SelectedEmail).Select(x => x.Email).FirstOrDefault();
                moveOutMobileEmailList.SelectedMobile = model.lstdetails.Where(x => x.contractaccountnumber == model.SelectedMobile).Select(x => x.Mobile).FirstOrDefault();
                moveOutMobileEmailList.SelectedBusinessPartnerNumber = model.lstdetails.Where(x => x.contractaccountnumber == model.SelectedMobile).Select(x => x.businesspartnernumber).FirstOrDefault();
            }
            CacheProvider.Store(CacheKeys.MOVE_OUT_DETAILS, new CacheItem<LstMoveoutAccount>(model));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_REVIEW_V3);
        }

        private bool validateMoveout(LstMoveoutAccount model)
        {
            foreach (var lstItem in model.lstdetails)
            {
                if (string.IsNullOrWhiteSpace(lstItem.contractaccountnumber))
                {
                    return false;
                }
                if (string.IsNullOrWhiteSpace(lstItem.businesspartnernumber))
                {
                    return false;
                }
                if (model.SameDisconnectDate)
                {
                    if (string.IsNullOrWhiteSpace(model.lstdetails[0].DisconnectDate))
                    {
                        return false;
                    }
                }
                else if (string.IsNullOrWhiteSpace(lstItem.DisconnectDate))
                {
                    return false;
                }
                if (model.SameTransferIban || model.SameTransferAcccount || model.SameTransferCheque || model.SameTransferWestern)
                {
                    if (string.IsNullOrWhiteSpace(model.lstdetails[0].SelectedRefundOption))
                    {
                        return false;
                    }
                }
                //else
                //{
                //    if (string.IsNullOrWhiteSpace(lstItem.SelectedRefundOption))
                //    {
                //        return false;
                //    }
                //}
                if (!model.SameTransferIban)
                {
                    //if (string.IsNullOrWhiteSpace(lstItem.SelectedRefundOption))
                    //{
                    //    return false;
                    //}
                    if (!string.IsNullOrWhiteSpace(lstItem.SelectedRefundOption) && lstItem.SelectedRefundOption.Equals("I"))
                    {
                        if (string.IsNullOrWhiteSpace(lstItem.ConfirmIbanAccountNumber))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(model.lstdetails[0].ConfirmIbanAccountNumber))
                    {
                        return false;
                    }
                }
                if (!model.SameTransferAcccount)
                {
                    if (!string.IsNullOrWhiteSpace(lstItem.SelectedRefundOption) && lstItem.SelectedRefundOption.Equals("T"))
                    {
                        if (string.IsNullOrWhiteSpace(lstItem.transferaccount))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(model.lstdetails[0].transferaccount))
                    {
                        return false;
                    }
                }
                if (!model.SameTransferWestern)
                {
                    if (!string.IsNullOrWhiteSpace(lstItem.SelectedRefundOption) && lstItem.SelectedRefundOption.Equals("Q"))
                    {
                        if (string.IsNullOrWhiteSpace(lstItem.IsWestenCountrylist) || string.IsNullOrWhiteSpace(lstItem.IsWestenCurrencylist))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(model.lstdetails[0].IsWestenCountrylist) || string.IsNullOrWhiteSpace(model.lstdetails[0].IsWestenCurrencylist))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult MoveOutReview_v3()
        {
            MoveOutState state;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }

            LstMoveoutAccount model;
            if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_DETAILS, out model))
            {
                CultureInfo culture;
                DateTimeStyles styles;
                culture = SitecoreX.Culture;
                if (culture.ToString().Equals("ar-AE"))
                {
                    Array.ForEach(model.lstdetails.ToArray(), x => model.lstdetails.Select(y => y.DisconnectDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December")));
                }
                styles = DateTimeStyles.None;
                Array.ForEach(model.lstdetails.ToArray(),
                    (y =>
                    {
                        DateTime dateResult;
                        if (DateTime.TryParse(y.DisconnectDate, culture, styles, out dateResult))
                            y.DisconnectDateTime = dateResult;
                        else
                            y.DisconnectDateTime = null;

                    }));
                model.lstdetails.ToList().ForEach(x => x.SelectedRefundOption = (model.SameTransferIban || model.SameTransferAcccount || model.SameTransferCheque || model.SameTransferWestern) ? model.lstdetails[0].SelectedRefundOption : (x.SelectedRefundOption != null) ? x.SelectedRefundOption : string.Empty);
                List<MoveoutReviewV3> reviewlist = new List<MoveoutReviewV3>();

                Array.ForEach(model.lstdetails.ToArray(), x => reviewlist.Add(new MoveoutReviewV3
                {
                    CustomerName = x.contractaccountname,
                    CustomerFirstName = x.customerfirstname,
                    CustomerLastName = x.customerlastname,
                    RefundFlag = model.SameNoRefund ? model.lstdetails[0].SelectedRefundOption : x.SelectedRefundOption,
                    RefundThrough = !model.SameNoRefund ? (x.SelectedRefundOption.Equals("Q") ? Translate.Text("updateiban.westernunion") :
                            (x.SelectedRefundOption.Equals("T") ? Translate.Text("updateiban.anotheractive") : x.SelectedRefundOption.Equals("I") ? Translate.Text("updateiban.iban") : x.SelectedRefundOption.Equals("N") ? Translate.Text("updateiban.norefund") :
                            x.SelectedRefundOption.Equals("C") ? Translate.Text("updateiban.cheque") : string.Empty)) : (model.lstdetails[0].SelectedRefundOption.Equals("Q") ? Translate.Text("updateiban.westernunion") :
                            (model.lstdetails[0].SelectedRefundOption.Equals("T") ? Translate.Text("updateiban.anotheractive") : model.lstdetails[0].SelectedRefundOption.Equals("I") ? Translate.Text("updateiban.iban") : model.lstdetails[0].SelectedRefundOption.Equals("N") ? Translate.Text("updateiban.norefund") :
                            model.lstdetails[0].SelectedRefundOption.Equals("C") ? Translate.Text("updateiban.cheque") : string.Empty)),
                    DisconnectionDate = model.SameDisconnectDate ? model.lstdetails[0].DisconnectDateTime.Value.ToString("dd MMM yyyy") : x.DisconnectDateTime.Value.ToString("dd MMM yyyy"),
                    DisconnectionTime = model.SameDisconnectDate ? model.lstdetails[0].DisconnectionTime : x.DisconnectionTime,
                    IBANNumber = !model.SameTransferIban ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("I") && (x.ConfirmIbanAccountNumber.Equals(x.SelectedIban) || x.ConfirmIbanAccountNumber.Equals(x.IbanAccountNumber)) ? "AE" + x.ConfirmIbanAccountNumber : string.Empty) : (model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].SelectedIban) || model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].IbanAccountNumber)) ? "AE" + model.lstdetails[0].ConfirmIbanAccountNumber : string.Empty,
                    AccountNumber = !model.SameTransferAcccount ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("T") ? x.transferaccount : string.Empty) : model.lstdetails[0].transferaccount,
                    ReceivingCity = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCitylist : string.Empty) : model.lstdetails[0].IsWestenCitylist,
                    ReceivingCountry = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.CountrylistText : string.Empty) : model.lstdetails[0].CountrylistText,
                    ReceivingState = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.StatelistText : string.Empty) : model.lstdetails[0].StatelistText,
                    ReceivingCurrency = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCurrencylist : string.Empty) : model.lstdetails[0].IsWestenCurrencylist,
                    MaskedEmail = model.lstdetails.Where(acc => acc.contractaccountnumber == model.SelectedEmail).Select(em => em.MaskedEmail).FirstOrDefault(),
                    MaskedMobile = model.lstdetails.Where(acc => acc.contractaccountnumber == model.SelectedMobile).Select(m => m.MaskedMobile).FirstOrDefault(),
                    Mobile = model.lstdetails.Where(acc => acc.contractaccountnumber == model.SelectedMobile).Select(m => m.Mobile).FirstOrDefault(),
                    Email = model.lstdetails.Where(acc => acc.contractaccountnumber == model.SelectedMobile).Select(m => m.Email).FirstOrDefault(),
                }));
                return View("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutReviews.cshtml", reviewlist);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult MoveOutReview_v3(string otprequestid)
        {
            MoveOutState state;
            LstMoveoutAccount model;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
            }
            if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_DETAILS, out model))
            {
                ServiceResponse<AccountDetails[]> response;
                var context = ContentRepository.GetItem<AccountSelector>(new GetItemByIdOptions(Guid.Parse("{0E59E02C-CC36-4882-9FDD-CFBFC7ED2B5F}")));
                response = GetBillingAccounts(false, true, context.NotificationCode, context.NotificationCode, context.ServiceFlag);
                List<accountsIn> accountlist = new List<accountsIn>();
                CultureInfo culture;
                DateTimeStyles styles;

                culture = SitecoreX.Culture;
                if (culture.ToString().Equals("ar-AE"))
                {
                    Array.ForEach(model.lstdetails.ToArray(), x => model.lstdetails.Select(y => y.DisconnectDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December")));
                }
                styles = DateTimeStyles.None;
                Array.ForEach(model.lstdetails.ToArray(),
                    (y =>
                    {
                        DateTime dateResult;
                        if (DateTime.TryParse(y.DisconnectDate, culture, styles, out dateResult))
                            y.DisconnectDateTime = dateResult;
                        else
                            y.DisconnectDateTime = null;
                    }));

                Array.ForEach(model.lstdetails.ToArray(), x => accountlist.Add(new accountsIn
                {
                    contractaccountnumber = x.contractaccountnumber,
                    premise = response.Payload.Where(y => ("00" + y.AccountNumber).Equals(x.contractaccountnumber)).FirstOrDefault().PremiseNumber,
                    disconnectiondate = model.SameDisconnectDate ? model.lstdetails[0].DisconnectDateTime.Value.ToString("yyyyMMdd") : x.DisconnectDateTime.Value.ToString("yyyyMMdd"),
                    businesspartnernumber = x.businesspartnernumber,
                    disconnectiontime = model.SameDisconnectDate ? model.lstdetails[0].DisconnectionTime.ToString().Replace(":", "") : x.DisconnectionTime.ToString().Replace(":", ""),
                    refundmode = (model.SameTransferIban || model.SameTransferAcccount || model.SameTransferCheque || model.SameTransferWestern || model.SameNoRefund) ? model.lstdetails[0].SelectedRefundOption : x.SelectedRefundOption,
                    ibannumber = !model.SameTransferIban ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("I") && (x.ConfirmIbanAccountNumber.Equals(x.SelectedIban) || x.ConfirmIbanAccountNumber.Equals(x.IbanAccountNumber)) ? "AE" + x.ConfirmIbanAccountNumber : string.Empty) : (model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].SelectedIban) || model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].IbanAccountNumber)) ? "AE" + model.lstdetails[0].ConfirmIbanAccountNumber : string.Empty,
                    transferaccountnumber = !model.SameTransferAcccount ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("T") ? x.transferaccount : string.Empty) : model.lstdetails[0].transferaccount,
                    countrykey = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCountrylist : string.Empty) : model.lstdetails[0].IsWestenCountrylist,
                    region = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenStatelist : string.Empty) : model.lstdetails[0].IsWestenStatelist,
                    city = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCitylist : string.Empty) : model.lstdetails[0].IsWestenCitylist,
                    currencykey = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCurrencylist : string.Empty) : model.lstdetails[0].IsWestenCurrencylist,
                    additionalinput2 = otprequestid
                }));

                moveOutParams moveoutParam = new moveOutParams
                {
                    accountlist = accountlist.ToArray(),
                    channel = "W",
                    notificationtype = "MO",
                    executionflag = "W",
                    applicationflag = "M",
                    disconnectiondate = model.lstdetails[0].DisconnectDateTime.Value.ToString("yyyyMMdd"),
                    disconnectiontime = model.lstdetails[0].DisconnectionTime.ToString().Replace(":", ""),
                    //mobile = model.MobileNumber.AddMobileNumberZeroPrefix(),
                    mobile = model.lstdetails[0].Mobile,
                    email = model.lstdetails[0].Email,
                    attachment = model.Attachment,
                    attachmenttype = model.RefundDocument != null && !string.IsNullOrWhiteSpace(model.RefundDocument.FileName) ? model.RefundDocument.FileName.Split('.')[1] : string.Empty

                };

                var responsemoveout = DewaApiClient.SetMoveOutRequest(moveoutParam, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (responsemoveout.Succeeded)
                {
                    var selectedAccounts = SharedAccount.CreateForMoveOut(response.Payload.ToList(), accountlist.Select(x => x.contractaccountnumber).ToList());

                    var moveOutConfirmModel = new MoveOutConfirm();
                    moveOutConfirmModel.SelectedAccounts = selectedAccounts;
                    CacheProvider.Store(CacheKeys.MOVE_OUT_CONFIRM_ACCOUNTS, new CacheItem<MoveOutConfirm>(moveOutConfirmModel, TimeSpan.FromMinutes(20)));
                    CacheProvider.Store(CacheKeys.MOVE_OUT_CONFIRM, new CacheItem<SetMoveOutRequestResponse>(responsemoveout.Payload, TimeSpan.FromMinutes(20)));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J18_MOVE_OUT_CONFIRM);
                }
                else
                {
                    if (responsemoveout.Payload != null && responsemoveout.Payload.@return != null && responsemoveout.Payload.@return.notificationlist != null)
                    {
                        var errorCount = 0;
                        foreach (var notification in responsemoveout.Payload.@return.notificationlist)
                        {
                            ModelState.AddModelError("error" + errorCount.ToString(), notification.message + Translate.Text("moveout.foraccount") + notification.contractaccountnumber);
                            errorCount++;
                        }

                        CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                    }
                    else if (responsemoveout.Payload != null && responsemoveout.Payload.@return != null)
                    {
                        ModelState.AddModelError(string.Empty, responsemoveout.Payload.@return.description);
                        CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                    }
                    //return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDetails.cshtml", model);;
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_DETAILS_V3);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
        }

        #region [Crud OTP]
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtp(SendOtpRequest request)
        {
            string errVerf = "";
            try
            {
                MoveOutMobileEmailList model;
                if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_MOB_EML_LST, out model))
                {
                    //Send or Verify otp
                    var response = SmartCommunicationClient.CustomerVerifyOtp(new SmartCommunicationVerifyOtpRequest()
                    {
                        mode = request.mode,
                        sessionid = CurrentPrincipal.SessionToken,
                        reference = !string.IsNullOrEmpty(request.reqId) ? request.reqId : string.Empty,
                        prtype = request.prtype,
                        email = (!string.IsNullOrEmpty(request.type) && request.type.Equals("e")) ? model.SelectedEmail : string.Empty,
                        mobile = (!string.IsNullOrEmpty(request.type) && request.type.Equals("m")) ? model.SelectedMobile : string.Empty,
                        contractaccountnumber = string.Empty,
                        businesspartner = model.SelectedBusinessPartnerNumber,
                        otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : null
                    }, Request.Segment(), RequestLanguage); ;
                    if (response != null && response.Succeeded)
                    {
                        if (request.mode == "S" && string.IsNullOrEmpty(request.reqId))
                            model.OtpRequestId = response.Payload.otprequestid;
                        return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        errVerf = response.Message;
                    }
                    return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = ex.Message;
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #endregion Version 3

        #region Moveout Demolish

        [HttpGet, TwoPhaseAuthorize]
        public ActionResult MoveOutDemolish()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            var model = new MoveOutDemolish();
            var moveoutconfigitem = ContentRepository.GetItem<MoveoutDisconnectionTime>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.MOVEOUT_DISCONNECTIONTIME_CONFIG)));
            var holidayResponse = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
            {
                lang = RequestLanguage.Code(),
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                holidayflag = "X",
                shiftflag = "X"
            }, Request.Segment());
            List<string> holidayList = new List<string>();
            if (holidayResponse != null && holidayResponse.Payload != null)
            {
                holidayList = holidayResponse.Payload.holidayList.Select(x => x.holidaydate).ToList();
                model.HolidayList = holidayList;
                if (holidayResponse.Payload.shiftstarttime != null && holidayResponse.Payload.shiftendtime != null)
                {
                    moveoutconfigitem.FromTime = Convert.ToDateTime(holidayResponse.Payload.shiftstarttime);
                    moveoutconfigitem.ToTime = Convert.ToDateTime(holidayResponse.Payload.shiftendtime);
                }
            }
            if (moveoutconfigitem != null)
            {
                model.DisconnectionDateTimeNotes = moveoutconfigitem.Notes;
                model.DisconnectionCurrentTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, false);
                model.DisconnectionTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, true);
            }
            model.isCustomer = !(CurrentPrincipal.Role == Roles.Government);
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolish.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorize]
        public ActionResult MoveOutDemolish(MoveOutDemolish model)
        {
            CultureInfo culture;
            DateTimeStyles styles;
            culture = SitecoreX.Culture;
            DateTime dateResult;
            if (culture.ToString().Equals("ar-AE"))
            {
                model.DisconnectDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }
            styles = DateTimeStyles.None;
            if (DateTime.TryParse(model.DisconnectDate, culture, styles, out dateResult))
                model.DisconnectDateTime = dateResult;
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("moveout.checkdisconnectiondate"));
            }
            if (ModelState.IsValid)
            {
                byte[] reqLetterAttachment = new byte[0];
                string error = string.Empty;
                if (model.RequestLetter != null && model.RequestLetter.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.RequestLetter, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        // CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                        return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolish.cshtml", model);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.RequestLetter.InputStream.CopyTo(memoryStream);
                            reqLetterAttachment = memoryStream.ToArray();
                        }
                    }
                }

                byte[] sitePlanAttachment = new byte[0];
                if (model.SitePlan != null && model.SitePlan.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.SitePlan, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                        return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolish.cshtml", model);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.SitePlan.InputStream.CopyTo(memoryStream);
                            sitePlanAttachment = memoryStream.ToArray();
                        }
                    }
                }

                byte[] emiratesAttachment = new byte[0];
                if (model.EmiratesID != null && model.EmiratesID.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.EmiratesID, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                        return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolish.cshtml", model);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.EmiratesID.InputStream.CopyTo(memoryStream);
                            emiratesAttachment = memoryStream.ToArray();
                        }
                    }
                }

                byte[] tradelicenseAttachment = new byte[0];
                if (model.TradeLicense != null && model.TradeLicense.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.TradeLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                        return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolish.cshtml", model);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.TradeLicense.InputStream.CopyTo(memoryStream);
                            tradelicenseAttachment = memoryStream.ToArray();
                        }
                    }
                }

                ServiceResponse<AccountDetails[]> response;
                //if (!CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST, out response) || response.Payload.Length < 1)
                //{
                //    response = DewaApiClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, "X", false, RequestLanguage, Request.Segment());
                //    if (response.Succeeded)
                //    {
                //        CacheProvider.Store(CacheKeys.ACCOUNT_LIST, new CacheItem<ServiceResponse<AccountDetails[]>>(response, TimeSpan.FromHours(1)));
                //    }
                //}
                var context = ContentRepository.GetItem<AccountSelector>(new GetItemByIdOptions(Guid.Parse("{5FF8F10F-A24F-4E1C-B426-934381432C66}")));
                response = GetBillingAccounts(false, true, context.NotificationCode, context.NotificationCode, context.ServiceFlag);

                List<accountsIn> accountlist = new List<accountsIn>();
                //if (model.isCustomer)
                //{
                accountlist.Add(new accountsIn
                {
                    contractaccountnumber = model.contractaccountnumber,
                    premise = response.Payload.Where(y => (y.AccountNumber).Equals(model.contractaccountnumber)).FirstOrDefault().PremiseNumber,
                    disconnectiondate = model.DisconnectDateTime.Value.ToString("yyyyMMdd"),
                    disconnectiontime = model.DisconnectionTime.ToString().Replace(":", ""),
                });
                //}
                moveOutParams moveoutParam = new moveOutParams
                {
                    accountlist = accountlist.ToArray(),
                    channel = "W",
                    notificationtype = "MO",
                    executionflag = "W",
                    applicationflag = "D",
                    email = model.Emailaddress,
                    mobile = model.MobileNumber.AddMobileNumberZeroPrefix(),
                    disconnectiondate = model.DisconnectDateTime.Value.ToString("yyyyMMdd"),
                    disconnectiontime = model.DisconnectionTime.ToString().Replace(":", ""),
                    plotnumber = model.plotnumber,
                    premisenumber = model.premisenumber,
                    attachment = reqLetterAttachment,
                    attachmenttype = model.RequestLetter != null && !string.IsNullOrWhiteSpace(model.RequestLetter.FileName) ? model.RequestLetter.FileName.Split('.')[1] : string.Empty,
                    attachment2 = sitePlanAttachment,
                    attachment2type = model.SitePlan != null && !string.IsNullOrWhiteSpace(model.SitePlan.FileName) ? model.SitePlan.FileName.Split('.')[1] : string.Empty,
                    attachment3 = (emiratesAttachment != null && !emiratesAttachment.Length.Equals(byte.MinValue)) ? emiratesAttachment : tradelicenseAttachment,
                    attachment3type = model.EmiratesID != null && !string.IsNullOrWhiteSpace(model.EmiratesID.FileName) ? model.EmiratesID.FileName.Split('.')[1] :
                           (model.TradeLicense != null && !string.IsNullOrWhiteSpace(model.TradeLicense.FileName) ? model.TradeLicense.FileName.Split('.')[1] : string.Empty)
                };

                var responsedemolish = DewaApiClient.SetMoveOutRequest(moveoutParam, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (responsedemolish.Succeeded)
                {
                    CacheProvider.Store(CacheKeys.MOVE_OUT_DEMOLISH_CONFIRM, new CacheItem<SetMoveOutRequestResponse>(responsedemolish.Payload, TimeSpan.FromMinutes(20)));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J18_MOVE_OUT_DEMOLISH_CONFIRM);
                }
                else
                {
                    var holidayResponse = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
                    {
                        lang = RequestLanguage.Code(),
                        userid = CurrentPrincipal.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        holidayflag = "X",
                        shiftflag = "X"
                    }, Request.Segment());
                    List<string> holidayList = new List<string>();
                    var moveoutconfigitem = ContentRepository.GetItem<MoveoutDisconnectionTime>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.MOVEOUT_DISCONNECTIONTIME_CONFIG)));
                    if (holidayResponse != null && holidayResponse.Payload != null)
                    {
                        holidayList = holidayResponse.Payload.holidayList.Select(x => x.holidaydate).ToList();
                        model.HolidayList = holidayList;
                        if (holidayResponse.Payload.shiftstarttime != null && holidayResponse.Payload.shiftendtime != null)
                        {
                            moveoutconfigitem.FromTime = Convert.ToDateTime(holidayResponse.Payload.shiftstarttime);
                            moveoutconfigitem.ToTime = Convert.ToDateTime(holidayResponse.Payload.shiftendtime);
                        }
                    }
                    if (moveoutconfigitem != null)
                    {
                        model.DisconnectionDateTimeNotes = moveoutconfigitem.Notes;
                        model.DisconnectionCurrentTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, false);
                        model.DisconnectionTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, true);
                    }
                    if (responsedemolish.Payload.@return.notificationlist == null)
                    {
                        ModelState.AddModelError(string.Empty, responsedemolish.Message);
                        //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, responsedemolish.Payload.@return.notificationlist[0].message);
                        // CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                    }
                }
            }
            //model.isCustomer = !(CurrentPrincipal.Role == Roles.Government);
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolish.cshtml", model);
        }

        [HttpGet, TwoPhaseAuthorize]
        public ActionResult MoveOutDemolishGov()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            var model = new MoveOutDemolish();
            var moveoutconfigitem = ContentRepository.GetItem<MoveoutDisconnectionTime>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.MOVEOUT_DISCONNECTIONTIME_CONFIG)));
            if (moveoutconfigitem != null)
            {
                model.DisconnectionDateTimeNotes = moveoutconfigitem.Notes;
                model.DisconnectionCurrentTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, false);
                model.DisconnectionTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, true);
            }
            var holidayResponse = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
            {
                lang = RequestLanguage.Code(),
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                holidayflag = "X",
                shiftflag = "X"
            }, Request.Segment());
            List<string> holidayList = new List<string>();
            if (holidayResponse != null && holidayResponse.Payload != null)
            {
                holidayList = holidayResponse.Payload.holidayList.Select(x => x.holidaydate).ToList();
                model.HolidayList = holidayList;
                if (holidayResponse.Payload.shiftstarttime != null && holidayResponse.Payload.shiftendtime != null)
                {
                    moveoutconfigitem.FromTime = Convert.ToDateTime(holidayResponse.Payload.shiftstarttime);
                    moveoutconfigitem.ToTime = Convert.ToDateTime(holidayResponse.Payload.shiftendtime);
                }
            }
            model.isCustomer = !(CurrentPrincipal.Role == Roles.Government);
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolishGov.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorize]
        public ActionResult MoveOutDemolishGov(MoveOutDemolish model)
        {
            CultureInfo culture;
            DateTimeStyles styles;
            culture = SitecoreX.Culture;
            DateTime dateResult;
            if (culture.ToString().Equals("ar-AE"))
            {
                model.DisconnectDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            }
            styles = DateTimeStyles.None;
            if (DateTime.TryParse(model.DisconnectDate, culture, styles, out dateResult))
                model.DisconnectDateTime = dateResult;
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("moveout.checkdisconnectiondate"));
            }
            if (ModelState.IsValid)
            {
                byte[] reqLetterAttachment = new byte[0];
                string error = string.Empty;
                if (model.RequestLetter != null && model.RequestLetter.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.RequestLetter, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                        return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolishGov.cshtml", model);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.RequestLetter.InputStream.CopyTo(memoryStream);
                            reqLetterAttachment = memoryStream.ToArray();
                        }
                    }
                }

                byte[] sitePlanAttachment = new byte[0];
                string attachmenterror = string.Empty;
                if (model.SitePlan != null && model.SitePlan.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.SitePlan, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                        return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolishGov.cshtml", model);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.SitePlan.InputStream.CopyTo(memoryStream);
                            sitePlanAttachment = memoryStream.ToArray();
                        }
                    }
                }

                List<accountsIn> accountlist = new List<accountsIn>();
                accountlist.Add(new accountsIn
                {
                    demolishplotnumber = model.plotnumber,
                    demolishpremise = model.premisenumber,
                    usertype = "G",
                    disconnectiondate = model.DisconnectDateTime.Value.ToString("yyyyMMdd"),
                    disconnectiontime = model.DisconnectionTime.ToString().Replace(":", ""),
                });

                moveOutParams moveoutParam = new moveOutParams
                {
                    accountlist = accountlist.ToArray(),
                    channel = "W",
                    notificationtype = "MO",
                    executionflag = "W",
                    applicationflag = "D",
                    email = model.Emailaddress,
                    mobile = model.MobileNumber.AddMobileNumberZeroPrefix(),
                    disconnectiondate = model.DisconnectDateTime.Value.ToString("yyyyMMdd"),
                    disconnectiontime = model.DisconnectionTime.ToString().Replace(":", ""),
                    plotnumber = model.plotnumber,
                    premisenumber = model.premisenumber,
                    attachment = reqLetterAttachment,
                    attachmenttype = model.RequestLetter != null && !string.IsNullOrWhiteSpace(model.RequestLetter.FileName) ? model.RequestLetter.FileName.Split('.')[1] : string.Empty,
                    attachment2 = sitePlanAttachment,
                    attachment2type = model.SitePlan != null && !string.IsNullOrWhiteSpace(model.SitePlan.FileName) ? model.SitePlan.FileName.Split('.')[1] : string.Empty
                };

                var responsedemolishgov = DewaApiClient.SetMoveOutRequest(moveoutParam, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (responsedemolishgov.Succeeded)
                {
                    CacheProvider.Store(CacheKeys.MOVE_OUT_DEMOLISH_GOV_CONFIRM, new CacheItem<SetMoveOutRequestResponse>(responsedemolishgov.Payload, TimeSpan.FromMinutes(20)));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J18_MOVE_OUT_DEMOLISH_CONFIRM_GOV);
                }
                else
                {
                    var holidayResponse = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
                    {
                        lang = RequestLanguage.Code(),
                        userid = CurrentPrincipal.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        holidayflag = "X",
                        shiftflag = "X"
                    }, Request.Segment());
                    List<string> holidayList = new List<string>();
                    var moveoutconfigitem = ContentRepository.GetItem<MoveoutDisconnectionTime>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.MOVEOUT_DISCONNECTIONTIME_CONFIG)));
                    if (holidayResponse != null && holidayResponse.Payload != null)
                    {
                        holidayList = holidayResponse.Payload.holidayList.Select(x => x.holidaydate).ToList();
                        model.HolidayList = holidayList;
                        if (holidayResponse.Payload.shiftstarttime != null && holidayResponse.Payload.shiftendtime != null)
                        {
                            moveoutconfigitem.FromTime = Convert.ToDateTime(holidayResponse.Payload.shiftstarttime);
                            moveoutconfigitem.ToTime = Convert.ToDateTime(holidayResponse.Payload.shiftendtime);
                        }
                    }
                    if (moveoutconfigitem != null)
                    {
                        model.DisconnectionDateTimeNotes = moveoutconfigitem.Notes;
                        model.DisconnectionCurrentTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, false);
                        model.DisconnectionTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, true);
                    }
                    if (responsedemolishgov.Payload.@return.notificationlist == null)
                    {
                        ModelState.AddModelError(string.Empty, responsedemolishgov.Message);
                        // CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, responsedemolishgov.Payload.@return.notificationlist[0].message);
                        //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                    }
                }
            }
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutDemolishGov.cshtml", model);
        }

        //[HttpGet]
        //public ActionResult DemolishAcoountGet(string accounts)
        //{
        //    if (!string.IsNullOrWhiteSpace(accounts))
        //    {
        //        string[] selectedaccount = null;

        //        if (accounts.Contains(','))
        //            selectedaccount = accounts.Split(',');
        //        else
        //            selectedaccount = new string[] { accounts };

        //        List<accountsIn> accountlist = new List<accountsIn>();
        //        Array.ForEach(selectedaccount, x => accountlist.Add(new accountsIn { contractaccountnumber = x.ToString() }));

        //        moveOutParams moveoutParam = new moveOutParams
        //        {
        //            accountlist = accountlist.ToArray(),
        //            channel = "W",
        //            executionflag = "R",
        //            applicationflag = "M"
        //        };

        //        var response = DewaApiClient.SetMoveOutRequest(moveoutParam, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
        //        if (response.Succeeded && response.Payload != null && response.Payload.@return != null && response.Payload.@return.accountslist != null)
        //        {
        //            return Json(new { status = true, Data = response.Payload.@return.attachmentmandatory }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return Json(new { status = false, Data = string.Empty }, JsonRequestBehavior.AllowGet);
        //}

        // Get Accounts Email and Mobile number.
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DemolishAcoountGetDetails(string accounts)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accounts))
                {
                    var response = DewaApiClient.GetContactDetails(CurrentPrincipal.SessionToken, accounts, RequestLanguage, Request.Segment());
                    var context = ContentRepository.GetItem<Foundation.Content.Models.AccountSelector>(new GetItemByIdOptions(Guid.Parse("{5FF8F10F-A24F-4E1C-B426-934381432C66}")));
                    ServiceResponse<AccountDetails[]> accresponse = null;
                    if (!string.IsNullOrEmpty(context.NotificationCode))
                    {
                        CacheProvider.TryGet(context.NotificationCode, out accresponse);
                    }
                    if (accresponse != null && accresponse.Payload != null)
                    {
                        var accountFromService = accresponse.Payload.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.AccountNumber) && x.AccountNumber.Equals(accounts));
                        if (accountFromService != null)
                        {
                            var result = new { detailsavailable = response.Succeeded, details = (response.Payload != null) ? response.Payload : null, category = accountFromService.AccountCategory };
                            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new { Data = new { detailsavailable = false, details = string.Empty, category = string.Empty } }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception)
            {
                return Json(new { Data = new { detailsavailable = false, details = string.Empty, category = string.Empty } }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Moveout Demolish

        #region Moveout Anonymous

        #region Landing Page

        [HttpGet, AllowAnonymous]
        public ActionResult MoveoutAnonymousLanding()
        {
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_RequestLanding.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MoveoutAnonymousLanding(FormCollection collection)
        {
            string redirectItem = string.Empty;

            if (collection != null)
            {
                int Selvalue = 0;

                if (collection.Keys.Count > 0)
                {
                    int.TryParse(collection.Get("radios_group1"), out Selvalue);
                }
                switch (Selvalue)
                {
                    case 1:
                        redirectItem = SitecoreItemIdentifiers.MOVE_OUT_START_V3;
                        break;

                    case 2:
                        redirectItem = SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE;
                        break;
                }
            }
            return RedirectToSitecoreItem(redirectItem);
        }

        #endregion Landing Page

        #region Step1 Search Premise and Contract Account

        [HttpGet, AllowAnonymous]
        public ActionResult MoveoutAnonymous()
        {
            MoveOutAnonymous _cacheModel = new MoveOutAnonymous();
            CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out _cacheModel);
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousOTP.cshtml", _cacheModel);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult MoveoutAnonymous(MoveOutAnonymous model)
        {
            try
            {
                // Future Center
                var _fc = FetchFutureCenterValues();

                var response = DewaApiClient.SetMoveOutwithOTP(model.AccountNumber, model.PremiseNumber, OTPFlags.FV1.ToString(), _fc.Branch, RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    SetMoveOutwithotpResponse _response;
                    _response = response.Payload;

                    model.EmailAddress = _response.@return.emailaddress;
                    model.MaskedEmailAddress = _response.@return.maskedemailaddress;
                    model.MobileNumber = _response.@return.mobilenumber;
                    model.MaskedMobileNumber = _response.@return.maskedmobilenumber;
                    model.BusinessPartnerName = _response.@return.businesspartnernickname;
                    model.BusinessPartnerNumber = _response.@return.businesspartnernumber;

                    CacheProvider.Store(CacheKeys.MOVEOUT_OTP_RESPONSE, new CacheItem<MoveOutAnonymous>(model, TimeSpan.FromMinutes(20)));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_DETAILS_OTP);
                }

                ModelState.AddModelError(string.Empty, response.Message);
                return View("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousOTP.cshtml", model);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return View("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousOTP.cshtml");
            }
        }

        #endregion Step1 Search Premise and Contract Account

        #region Step 2 Send OTP

        [HttpGet, AllowAnonymous]
        public ActionResult MoveoutAnonymousDetails()
        {
            try
            {
                MoveOutAnonymous model = new MoveOutAnonymous();

                CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out model);

                return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousDetailsOTP.cshtml", model);
            }
            catch (System.Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            }
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult MoveoutAnonymousDetails(MoveOutAnonymous model)
        {
            MoveOutAnonymous _cacheModel = new MoveOutAnonymous();
            CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out _cacheModel);
            try
            {
                // Future Center
                var _fc = FetchFutureCenterValues();
                string mobile = null;
                string email = null;

                var _emailAdressFormat = new EmailAddressAttribute();

                if (model.SelectedOptions == "email")
                {
                    email = _cacheModel.EmailAddress;
                    _cacheModel.SelectedOptions = email;
                }
                else
                {
                    mobile = _cacheModel.MobileNumber;
                    _cacheModel.SelectedOptions = mobile;
                }

                _cacheModel.PremiseNumber = model.PremiseNumber;
                _cacheModel.AccountNumber = model.AccountNumber;
                _cacheModel.OTPflag = OTPFlags.SOT.ToString();

                CacheProvider.Store(CacheKeys.MOVEOUT_OTP_RESPONSE, new CacheItem<MoveOutAnonymous>(_cacheModel, TimeSpan.FromMinutes(20)));

                var response = DewaApiClient.SetMoveOutwithOTP(model.AccountNumber, model.PremiseNumber, OTPFlags.SOT.ToString(), _fc.Branch, RequestLanguage, Request.Segment(), mobile, email);

                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_VERIFICATION_CODE_OTP);
                }

                ModelState.AddModelError(string.Empty, response.Message);
                return View("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousDetailsOTP.cshtml", _cacheModel);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return View("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousDetailsOTP.cshtml", _cacheModel);
            }
        }

        #endregion Step 2 Send OTP

        #region Step3 Enter Verification Code

        [HttpGet, AllowAnonymous]
        public ActionResult MoveoutAnonymousVerificationCode()
        {
            try
            {
                MoveOutAnonymous model = new MoveOutAnonymous();

                CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out model);

                var _emailAdressFormat = new EmailAddressAttribute();

                if (_emailAdressFormat.IsValid(model.SelectedOptions))
                {
                    model.Message = Translate.Text("Moveout.Anonymous.Message.Email");
                    model.MaskedDisplayValue = model.MaskedEmailAddress;
                }
                else
                {
                    model.Message = Translate.Text("Moveout.Anonymous.Message.Mobile");
                    model.MaskedDisplayValue = model.MaskedMobileNumber;
                }

                return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousVerificationCodeOTP.cshtml", model);
            }
            catch (System.Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            }
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult MoveoutAnonymousVerificationCode(MoveOutAnonymous model)
        {
            try
            {
                string mobile = null;
                string email = null;

                MoveOutAnonymous _cacheModel = new MoveOutAnonymous();
                CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out _cacheModel);
                model.AccountNumber = _cacheModel.AccountNumber;
                model.PremiseNumber = _cacheModel.PremiseNumber;
                model.SelectedOptions = _cacheModel.SelectedOptions;
                model.EmailAddress = _cacheModel.EmailAddress;

                var _emailAdressFormat = new EmailAddressAttribute();

                if (_emailAdressFormat.IsValid(model.SelectedOptions))
                {
                    email = model.SelectedOptions;
                    _cacheModel.SelectedOptions = email;
                }
                else
                {
                    mobile = model.SelectedOptions;
                    _cacheModel.SelectedOptions = mobile;
                }
                // Future Center
                var _fc = FetchFutureCenterValues();

                var response = DewaApiClient.SetMoveOutwithOTP(model.AccountNumber, model.PremiseNumber, OTPFlags.VOT.ToString(), _fc.Branch, RequestLanguage, Request.Segment(), mobile, email, model.OTPNumber);

                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    if (response.Payload.@return.recoverylist != null && response.Payload.@return.recoverylist.Count() > 0)
                    {
                        _cacheModel.recoverylist = response.Payload.@return.recoverylist;
                        _cacheModel.TotalPendingBalance = Convert.ToString(response.Payload.@return.recoverylist.Where(x => !string.IsNullOrWhiteSpace(x.total))?.Sum(x => Convert.ToDecimal(x.total)));
                        _cacheModel.PaymentAccountList = string.Join(",", _cacheModel.recoverylist.Select(x => x.contractaccount));
                        _cacheModel.PaymentAmountList = string.Join(",", _cacheModel.recoverylist.Select(x => x.total));
                        _cacheModel.PaymentBP_List = string.Join(",", _cacheModel.recoverylist.Select(x => x.businesspartner));
                    }
                    _cacheModel.PayAmount = response.Payload.@return.payamount;
                    _cacheModel.OTPNumber = model.OTPNumber;
                    _cacheModel.BusinessPartnerCatg = response.Payload.@return.businesspartnercategory;
                    _cacheModel.attachmentflag = !string.IsNullOrWhiteSpace(response.Payload.@return.ibanattachmentflag) && response.Payload.@return.ibanattachmentflag.Equals("X");

                    _cacheModel.okiban = response.Payload.@return.okiban;
                    _cacheModel.okcheque = response.Payload.@return.okcheque;
                    _cacheModel.oknorefund = response.Payload.@return.oknorefund;
                    List<SelectListItem> ibanList = null;
                    if (response.Payload.@return.ET_IBAN != null && response.Payload.@return.ET_IBAN.Any())
                    {
                        ibanList = new List<SelectListItem>();
                        foreach (var iban in response.Payload.@return.ET_IBAN)
                        {
                            ibanList.Add(new SelectListItem { Text = iban.maskIban, Value = iban.iban.Replace("AE", "") });
                        }

                        if (ibanList.Any())
                            ibanList.Add(new SelectListItem { Text = Translate.Text("Others"), Value = "others" });
                    }
                    _cacheModel.IbanList = ibanList;
                    _cacheModel.Ibanavailable = ibanList != null && ibanList.Any();
                    _cacheModel.ValidBankCodes = (response.Payload.@return.ET_Banks != null && response.Payload.@return.ET_Banks.Any()) ? new Tuple<string, string>(string.Join(",", response.Payload.@return.ET_Banks.Where(y => y.businesspartnercategory == response.Payload.@return.businesspartnercategory && !y.bankkeys.Equals("*")).Select(y => y.bankkeys)), string.Join(", ", response.Payload.@return.ET_Banks.Where(y => y.businesspartnercategory == response.Payload.@return.businesspartnercategory && !y.bankkeys.Equals("*")).Select(y => y.nameofbank))) : new Tuple<string, string>("", "");

                    CacheProvider.Store(CacheKeys.MOVEOUT_OTP_RESPONSE, new CacheItem<MoveOutAnonymous>(_cacheModel, TimeSpan.FromMinutes(20)));

                    if (response.Payload.@return.payamount != null && Convert.ToDecimal(response.Payload.@return.payamount) > 0)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_PAYMENT);
                    }
                    else
                    {
                        if (response.Payload.@return.recoverylist != null && response.Payload.@return.recoverylist.Count() > 0 && _cacheModel.TotalPendingBalance != null && _cacheModel.TotalPendingBalance != "0")
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_PAYMENT);
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_MOVEOUT_SUBMIT_REQUEST);
                    }
                }

                ModelState.AddModelError(string.Empty, response.Message);
                return View("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousVerificationCodeOTP.cshtml", model);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return View("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAnonymousVerificationCodeOTP.cshtml", model);
            }
        }

        #endregion Step3 Enter Verification Code

        /// <summary>
        /// If 0 amount then goto to step 4 submit request and create notification
        /// else
        /// goto payment process.
        /// </summary>
        /// <returns></returns>

        #region Payment Process

        [HttpGet]
        public ActionResult MoveOutAnonymousPayment()
        {
            MoveOutAnonymous model;
            CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out model);
            if (model != null)
            {
                return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAccounts.cshtml", model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult MoveOutAnonymousPayment(MoveOutAnonymous model)
        {
            MoveOutAnonymous cachemodel;
            CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out cachemodel);
            if (cachemodel == null)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            string PaymentPath = "MoveoutAnonymousPayment";
            CacheProvider.Store(CacheKeys.MOVEOUT_PAYMENT_ANONYMOUS_PATH, new CacheItem<string>(PaymentPath));
            if (Convert.ToDecimal(cachemodel.PayAmount) > 0 || Convert.ToDecimal(cachemodel.TotalPendingBalance) > 0)
            {
                #region [MIM Payment Implementation]

                var payRequest = new CipherPaymentModel();

                payRequest.PaymentData.amounts = Convert.ToString(cachemodel.PayAmount);
                payRequest.PaymentData.contractaccounts = cachemodel.AccountNumber;

                if (cachemodel.recoverylist != null && cachemodel.recoverylist.Count() > 0)
                {
                    if (!string.IsNullOrWhiteSpace(payRequest.PaymentData.amounts) && payRequest.PaymentData.amounts != "0")
                    {
                        payRequest.PaymentData.amounts = payRequest.PaymentData.amounts + "," + cachemodel.recoverylist.Select(x => x.total).Aggregate((i, j) => i + "," + j);
                        payRequest.PaymentData.contractaccounts = payRequest.PaymentData.contractaccounts + "," + cachemodel.recoverylist.Select(x => x.contractaccount).Aggregate((i, j) => i + "," + j);
                    }
                    else
                    {
                        payRequest.PaymentData.amounts = cachemodel.recoverylist.Select(x => x.total).Aggregate((i, j) => i + "," + j);
                        payRequest.PaymentData.contractaccounts = cachemodel.recoverylist.Select(x => x.contractaccount).Aggregate((i, j) => i + "," + j);
                    }
                }

                payRequest.PaymentData.businesspartner = cachemodel.BusinessPartnerNumber;
                payRequest.PaymentData.email = cachemodel.EmailAddress;
                payRequest.PaymentData.mobile = cachemodel.MobileNumber.AddMobileNumberZeroPrefix();
                payRequest.PaymentData.userid = cachemodel.BusinessPartnerNumber + "_" + cachemodel.OTPNumber;
                payRequest.IsThirdPartytransaction = false;
                payRequest.PaymentData.clearancetransaction = "";
                payRequest.ServiceType = ServiceType.PayBill;
                payRequest.PaymentMethod = model.paymentMethod;
                payRequest.BankKey = model.bankkey;
                payRequest.SuqiaValue = model.SuqiaDonation;
                payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                var payResponse = ExecutePaymentGateway(payRequest);
                if (Convert.ToInt32(payResponse.ErrorMessages?.Count) > 0)
                {
                    foreach (KeyValuePair<string, string> errorItem in payResponse.ErrorMessages)
                    {
                        ModelState.AddModelError(errorItem.Key, errorItem.Value);
                    }
                    return Redirect(Request.UrlReferrer.AbsoluteUri);
                }

                CacheProvider.Store(CacheKeys.MOVEOUT_ANONYMOUS_PAYMENT_MODEL, new CacheItem<CipherPaymentModel>(payResponse));
                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);

                #endregion [MIM Payment Implementation]
            }
            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAccounts.cshtml", model);
        }

        #endregion Payment Process

        #region Submit Request

        [HttpGet, AllowAnonymous]
        public ActionResult MoveoutAnonymousSubmitRequest()
        {
            try
            {
                MoveOutAnonymous model = new MoveOutAnonymous();

                CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out model);
                if (model != null)
                {
                    var holidayResponse = DewaApiClient.SetMoveInReadRequest(new moveInReadInput
                    {
                        lang = RequestLanguage.Code(),
                        userid = CurrentPrincipal.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        holidayflag = "X",
                        shiftflag = "X"
                    }, Request.Segment());
                    List<string> holidayList = new List<string>();
                    var moveoutconfigitem = ContentRepository.GetItem<MoveoutDisconnectionTime>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.MOVEOUT_DISCONNECTIONTIME_CONFIG)));
                    if (holidayResponse != null && holidayResponse.Payload != null)
                    {
                        holidayList = holidayResponse.Payload.holidayList.Select(x => x.holidaydate).ToList();
                        model.HolidayList = holidayList;
                        if (holidayResponse.Payload.shiftstarttime != null && holidayResponse.Payload.shiftendtime != null)
                        {
                            moveoutconfigitem.FromTime = Convert.ToDateTime(holidayResponse.Payload.shiftstarttime);
                            moveoutconfigitem.ToTime = Convert.ToDateTime(holidayResponse.Payload.shiftendtime);
                        }
                    }
                    if (moveoutconfigitem != null)
                    {
                        model.DisconnectionDateTimeNotes = moveoutconfigitem.Notes;
                        model.DisconnectionCurrentTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, false);
                        model.DisconnectionTimeOptions = GetDisconnectionTime(moveoutconfigitem.FromTime, moveoutconfigitem.ToTime, new TimeSpan(0, moveoutconfigitem.TimeInterval != null ? Convert.ToInt32(moveoutconfigitem.TimeInterval) : 15, 0), moveoutconfigitem.TimeFormat, true);
                    }
                    CacheProvider.Store(CacheKeys.MOVEOUT_OTP_RESPONSE, new CacheItem<MoveOutAnonymous>(model, TimeSpan.FromMinutes(20)));
                    model.MobileNumber = model.MobileNumber.RemoveMobileNumberZeroPrefix();
                    model.MaskedMobileNumber = model.MaskedMobileNumber.AddMobileNumberZeroPrefix();
                    model.DisconnectionDate = model.DiconnectionDateTime != null && model.DiconnectionDateTime.HasValue ? model.DiconnectionDateTime.Value.ToString("dd MMM yyyy", SitecoreX.Culture) : string.Empty;
                    model.DiconnectionDateTime = model.DiconnectionDateTime;
                    var refundoptions = PopulateMoveoutRefundOptions(model.okiban, model.okcheque, string.Empty, string.Empty, model.oknorefund);
                    model.RefundOptions = refundoptions.Item1;
                    model.refund = refundoptions.Item2;
                    return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutDetails.cshtml", model);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            }
            catch (System.Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            }
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult MoveoutAnonymousSubmitRequest(MoveOutAnonymous model)
        {
            MoveOutAnonymous cachemodel;
            CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out cachemodel);
            if (cachemodel == null)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            cachemodel.IsSuccess = false;
            cachemodel.SelectedRefundOption = model.SelectedRefundOption;
            var _fc = FetchFutureCenterValues();
            byte[] RefundAttachment = new byte[0];
            byte[] RequestAttachment = new byte[0];
            cachemodel.DisconnectionDate = model.DisconnectionDate;
            cachemodel.DiconnectionDateTime = model.DiconnectionDateTime;
            cachemodel.DisconnectionTime = model.DisconnectionTime;
            //cachemodel.MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix();
            cachemodel.MobileNumber = cachemodel.MobileNumber.AddMobileNumberZeroPrefix();
            cachemodel.Ibannumber = (model.SelectedRefundOption != null && model.SelectedRefundOption.Equals("I") && (model.ConfirmIbanAccountNumber.Equals(model.SelectedIban) || model.ConfirmIbanAccountNumber.Equals(model.IbanAccountNumber))) ? string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, model.ConfirmIbanAccountNumber) : string.Empty;
            string error;
            if (model.RefundDocument != null && model.RefundDocument.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.RefundDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                    CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                    return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutDetails.cshtml", model);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.RefundDocument.InputStream.CopyTo(memoryStream);
                        cachemodel.RefundDocumentbyte = memoryStream.ToArray();
                        cachemodel.RefundFilename = model.RefundDocument.FileName;
                    }
                }
            }
            if (model.RequestLetter != null && model.RequestLetter.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.RequestLetter, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                    CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                    return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutAccounts.cshtml", model);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.RequestLetter.InputStream.CopyTo(memoryStream);
                        cachemodel.RequestDocumentbyte = memoryStream.ToArray();
                        cachemodel.RequestFilename = model.RequestLetter.FileName;
                    }
                }
            }
            CacheProvider.Store(CacheKeys.MOVEOUT_OTP_RESPONSE, new CacheItem<MoveOutAnonymous>(cachemodel, TimeSpan.FromMinutes(20)));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_REVIEW);
        }

        #endregion Submit Request

        #region Review Page

        [HttpGet, AllowAnonymous]
        public ActionResult MoveoutAnonymousReview()
        {
            try
            {
                MoveOutAnonymous cachemodel = new MoveOutAnonymous();
                CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out cachemodel);
                if (cachemodel != null)
                {
                    MoveOutAnonymous model = new MoveOutAnonymous();
                    model.BusinessPartnerName = cachemodel.BusinessPartnerName;
                    CultureInfo culture;
                    DateTimeStyles styles;
                    culture = SitecoreX.Culture;
                    model.DisconnectionDate = cachemodel.DisconnectionDate;
                    model.DisconnectionDateReview = cachemodel.DisconnectionDate;
                    model.DisconnectionDateReview = model.DisconnectionDateReview.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                    DateTime dateResult;
                    styles = DateTimeStyles.None;
                    if (DateTime.TryParse(model.DisconnectionDateReview, culture, styles, out dateResult))
                        model.DiconnectionDateTime = dateResult;

                    model.DisconnectionDateReview = model.DiconnectionDateTime.Value.ToString("dd MMM yyyy", culture);
                    model.DisconnectionTime = cachemodel.DisconnectionTime;
                    model.Ibannumber = cachemodel.Ibannumber;
                    model.Refundthrough = string.Empty;
                    if (cachemodel.SelectedRefundOption != null)
                    {
                        model.Refundthrough = cachemodel.SelectedRefundOption.Equals("I") ? Translate.Text("updateiban.iban") : cachemodel.SelectedRefundOption.Equals("C") ? Translate.Text("updateiban.cheque") : cachemodel.SelectedRefundOption.Equals("N") ? Translate.Text("updateiban.norefund") : string.Empty;
                    }
                    return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutReviews.cshtml", model);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_MOVEOUT_SUBMIT_REQUEST);
            }
            catch (System.Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_MOVEOUT_SUBMIT_REQUEST);
            }
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult MoveoutAnonymousReview(MoveOutAnonymous model)
        {
            MoveOutAnonymous cachemodel;
            CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out cachemodel);
            if (cachemodel == null)
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            cachemodel.IsSuccess = false;
            var _fc = FetchFutureCenterValues();
            string appflag = "FNB";
            string disdate = cachemodel.DisconnectionDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            var response = DewaApiClient.SetMoveOutwithOTP(cachemodel.AccountNumber, cachemodel.PremiseNumber, appflag, _fc.Branch, RequestLanguage, Request.Segment(), cachemodel.MobileNumber, null, null, cachemodel.SelectedRefundOption, Convert.ToDateTime(disdate).ToString("yyyyMMdd"), cachemodel.DisconnectionTime.Replace(":", ""), cachemodel.RefundDocumentbyte, cachemodel.RefundFilename, cachemodel.RequestDocumentbyte, cachemodel.RequestFilename, cachemodel.Ibannumber);

            if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
            {
                cachemodel.Message = response.Payload.@return.notificationnumber;
                cachemodel.IsSuccess = true;
                cachemodel.AdditionalInformation = response.Payload.@return.additionalinformation;
                CacheProvider.Store(CacheKeys.MOVEOUT_OTP_RESPONSE, new CacheItem<MoveOutAnonymous>(cachemodel, TimeSpan.FromMinutes(20)));

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_ANONYMOUS_CONFIRM);
            }
            model.BusinessPartnerName = cachemodel.BusinessPartnerName;
            CultureInfo culture;
            DateTimeStyles styles;
            culture = SitecoreX.Culture;
            model.DisconnectionDate = cachemodel.DisconnectionDate;
            model.DisconnectionDateReview = cachemodel.DisconnectionDate;
            model.DisconnectionDateReview = model.DisconnectionDateReview.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
            DateTime dateResult;
            styles = DateTimeStyles.None;
            if (DateTime.TryParse(model.DisconnectionDateReview, culture, styles, out dateResult))
                model.DiconnectionDateTime = dateResult;

            model.DisconnectionDateReview = model.DiconnectionDateTime.Value.ToString("dd MMM yyyy", culture);
            model.DisconnectionTime = cachemodel.DisconnectionTime;
            model.Ibannumber = cachemodel.Ibannumber;
            model.Refundthrough = string.Empty;
            if (cachemodel.SelectedRefundOption != null)
            {
                model.Refundthrough = cachemodel.SelectedRefundOption.Equals("I") ? Translate.Text("updateiban.iban") : cachemodel.SelectedRefundOption.Equals("C") ? Translate.Text("updateiban.cheque") : cachemodel.SelectedRefundOption.Equals("N") ? Translate.Text("updateiban.norefund") : string.Empty;
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutReviews.cshtml", model);
        }

        #endregion Review Page

        #region Notification Page

        [HttpGet, AllowAnonymous]
        public ActionResult MoveOutAnonymous_Confirm()
        {
            try
            {
                MoveOutAnonymous model = new MoveOutAnonymous();

                CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out model);
                if (model == null)
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
                CacheProvider.Remove(CacheKeys.MOVEOUT_OTP_RESPONSE);
                return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_Confirm.cshtml", model);
            }
            catch (System.Exception)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVEOUTANONYMOUS_INITIATE);
            }
        }

        #endregion Notification Page

        #endregion Moveout Anonymous

        [HttpGet]
        public ActionResult ClearMoveOutCache()
        {
            string state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_SELECTEDACCOUNTS, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVE_OUT_SELECTEDACCOUNTS);
                CacheProvider.Remove(CacheKeys.MOVE_OUT_RESULT);
            }
            CacheProvider.Remove(CacheKeys.MOVEOUT_OTP_RESPONSE);
            return new EmptyResult();
        }

        #region Helpers

        private Tuple<IEnumerable<SelectListItem>, bool> PopulateMoveoutRefundOptions(string striban, string strcheque, string strtransfer, string strwestern, string stroknorefund)
        {
            try
            {
                bool iban = striban.Equals("Y") ? true : false;
                bool cheque = strcheque.Equals("Y") ? true : false;
                bool transfer = strtransfer.Equals("Y") ? true : false;
                bool western = strwestern.Equals("Y") ? true : false;
                bool oknorefund = stroknorefund.Equals("Y") ? true : false;
                var dataSource = ContentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.REFUND_OPTIONS));
                var refundOptions = dataSource.Items;
                var convertedItems = refundOptions.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                List<SelectListItem> selectedItems = new List<SelectListItem>();
                if (!iban && !cheque && !transfer && !western)
                {
                    return Tuple.Create<IEnumerable<SelectListItem>, bool>(null, false);
                }

                if (iban && cheque && transfer && western)
                    return Tuple.Create<IEnumerable<SelectListItem>, bool>(convertedItems, true);

                if (iban)
                    selectedItems.Add(convertedItems.Where(x => x.Value.Equals("I")).FirstOrDefault());

                if (cheque)
                    selectedItems.Add(convertedItems.Where(x => x.Value.Equals("C")).FirstOrDefault());
                if (transfer)
                    selectedItems.Add(convertedItems.Where(x => x.Value.Equals("T")).FirstOrDefault());
                if (western)
                    selectedItems.Add(convertedItems.Where(x => x.Value.Equals("Q")).FirstOrDefault());
                if (oknorefund)
                    selectedItems.Add(convertedItems.Where(x => x.Value.Equals("N")).FirstOrDefault());

                return Tuple.Create<IEnumerable<SelectListItem>, bool>(selectedItems.AsEnumerable(), true);
            }
            catch (System.Exception)
            {
                throw new System.Exception("TransferTo DataSource is Null");
            }
        }

        #endregion Helpers
    }

    [Serializable]
    public class MoveOutWorkflowState
    {
        public SharedAccount Account { get; set; }

        public string ContractAccount { get; set; }

        public bool Clearance { get; set; }

        public DateTime? DisconnectDate { get; set; }

        public string MobileNumber { get; set; }

        public string IbanAccountNumber { get; set; }

        public string Comments { get; set; }

        public Guid AttachmentId { get; set; }

        public string AttachmentFilename { get; set; }

        public string AttachmentExtension { get; set; }

        public bool TransferToIbanAccount { get; set; }

        public bool TransferToChequeAccount { get; set; }

        public bool Completed { get; set; }

        public string ResponseMessage { get; set; }

        public string ErrorMessage { get; set; }

        public bool Succeeded { get; set; }
        public decimal CashAmount { get; set; }
        public decimal ChequeAmount { get; set; }
        public decimal IBANAmount { get; set; }
        public decimal CashNonSDAmount { get; set; } //Cash Non Security Deposit Amount
        public decimal ChequeNonSDAmount { get; set; } //Cheque Non Security Deposit Amount
        public decimal IBANNonSDAmount { get; set; } //IBAN Non Security Deposit Amount
        public decimal CashSDAmount { get; set; } //Cash Security Deposit Amount
        public decimal ChequeSDAmount { get; set; } //Cheque Security Deposit Amount
        public decimal IBANSDAmount { get; set; } //IBAN Security Deposit Amount
        public decimal NetAmount { get; set; }
        public decimal DownPaymentAmount { get; set; }
        public decimal NonSDAmount { get; set; }
        public string CashAllowedCode { get; set; }
        public bool IsCashAllowed { get; set; }

        public string ChequeAllowedCode { get; set; }
        public bool IsChequeAllowed { get; set; }

        public string IBANAllowedCode { get; set; }
        public bool IsIBANAllowed { get; set; }

        public string AccountNumber { get; set; }

        public string SelectedBusinessPartnerNumber { get; set; }
        public string BusinessPartnerType { get; set; }

        public decimal OutStandingAmount { get; set; }
        public string PremiseNo { get; set; }
        public string PermiseType { get; set; }
        public decimal SDAmount { get; set; } //Security Deposit Amount
        public string WorkflowInProcess { get; set; }
        public bool CollectPaymentCode { get; set; }

        public string SelectedRefundMode { get; set; }
    }

    public class MoveOutValidator
    {
        private readonly MoveOutWorkflowState _moveOut;

        public MoveOutValidator(MoveOutWorkflowState moveOut)
        {
            _moveOut = moveOut;
        }

        public IEnumerable<Result> ValidateSelectedAccount(Account selectedAccount)
        {
            if (selectedAccount == null)
            {
                yield return new Result { Field = "SelectedAccount", Message = "Invalid Account" };
            }
            else if (!selectedAccount.PartialPaymentPermitted)
            {
                yield return new Result { Field = "SelectedAccount", Message = "Move out has been successfully applied for this account" };
            }
        }

        public IEnumerable<Result> ValidateDetails(DetailsView detailsModel)
        {
            if (detailsModel == null)
            {
                yield return new Result
                {
                    Field = string.Empty,
                    Message = "Invalid post."
                };
            }
            else
            {
                var today = Foundation.Helpers.DateHelper.Today();
                if (!detailsModel.DisconnectDateAsDateTime.HasValue)
                {
                    yield return new Result
                    {
                        Field = "DisconnectDate",
                        Message = "Disconnection date is required."
                    };
                }
                if (detailsModel.DisconnectDateAsDateTime.HasValue && detailsModel.DisconnectDateAsDateTime.Value.Date < today)
                {
                    yield return new Result
                    {
                        Field = "DisconnectDate",
                        Message = Translate.Text("Disconnection date must not be in the past.")
                    };
                }
                if (detailsModel.DisconnectDateAsDateTime.HasValue && detailsModel.DisconnectDateAsDateTime.Value.Date > today.AddMonths(3))
                {
                    yield return new Result
                    {
                        Field = "DisconnectDate",
                        Message = Translate.Text("Disconnection date can only be up to 3 months in advance.")
                    };
                }

                if (_moveOut.Account.Type.Equals("commercial", StringComparison.InvariantCulture) ||
                    _moveOut.Account.Type.Equals("industrial", StringComparison.InvariantCulture))
                {
                    if (string.IsNullOrWhiteSpace(detailsModel.IbanAccountNumber) ||
                        string.IsNullOrWhiteSpace(detailsModel.ConfirmIbanAccountNumber))
                    {
                        yield return new Result
                        {
                            Field = "IbanAccountNumber",
                            Message = Translate.Text("IBAN number is required.")
                        };
                    }

                    if (detailsModel.Attachment == null)
                    {
                        yield return new Result
                        {
                            Field = "Attachment",
                            Message = Translate.Text("Attachment is required.")
                        };
                    }
                    string errors;
                    if (!AttachmentHelper.AttachmentIsValid(detailsModel.Attachment, General.MaxAttachmentSize, out errors, General.AcceptedFileTypes))
                    {
                        yield return new Result
                        {
                            Field = "Attachment",
                            Message = errors
                        };
                    }
                }
            }
        }

        public class Result
        {
            public string Field { get; set; }

            public string Message { get; set; }
        }
    }

    //public class MoveOutResult
    //{
    //    public bool proceed { get; set; }
    //    public bool issuccess { get; set; }
    //    public string errormessage { get; set; }
    //    public List<string> duplicaterequests { get; set; }
    //    public double totalamounttopay { get; set; }
    //    public bool iscustomer { get; set; }

    //    public List<MoveOutaccountsDetailResponse> details { get; set; }
    //    public List<MoveOutDivisionWiseCAResponse> divisionlist { get; set; }
    //    public string TotalPendingBalance { get; set; }
    //    public string OutstandingBalance { get; set; }
    //    public string PaymentAmountList { get; set; }
    //    public string PaymentAccountList { get; set; }
    //    public string PaymentBP_List { get; set; }
    //    public string evCardnumber { get; set; }
    //    //public accountsDetailsOut[] details { get; set; }
    //}

    //public class MoveOutState
    //{
    //    public MoveOutResult moveoutresult { get; set; }
    //    public MoveOutResponse moveoutdetails { get; set; }
    //    public List<string> page { get; set; }
    //}

    //public enum evdeactivatestep
    //{
    //    accounts = 0,
    //    details = 1,
    //    review = 2,
    //    confirm = 3
    //}
}