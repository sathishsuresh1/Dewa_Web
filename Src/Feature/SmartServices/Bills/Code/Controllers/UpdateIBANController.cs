using DEWAXP.Feature.Bills.Models.UpdateIBAN;
using DEWAXP.Feature.Bills.UpdateIBAN;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.MoveOut;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Models.UpdateIBAN;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Utils;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    public class UpdateIBANController : BaseController
    {
        //
        // GET: /UpdateIBAN/

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), HttpGet]
        public ActionResult NewRequest()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            // ;
            //UpdateIBANModel Successmodel;
            //if (CacheProvider.TryGet(CacheKeys.UPDATEIBAN, out Successmodel))
            //{
            //    model.Successful = Successmodel.Successful;
            //    model.Successfulmessage = Successmodel.Successfulmessage;
            //    CacheProvider.Remove(CacheKeys.UPDATEIBAN);
            //}
            UpdateIBANModel model = new UpdateIBANModel();
            model.IBANSList = PopulateUpdateibanOptions();

            return PartialView("~/Views/Feature/Bills/UpdateIBAN/_NewRequest.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewRequest(UpdateIBANModel model)
        {
            string error = string.Empty;
            model.Attachment = new byte[0];
            if (model.SumbitType == "recoverypayment")
            {
                try
                {
                    UpdateIBANModel _cachedIBANData = null;
                    if (CacheProvider.TryGet($"IBANDetail_ac{model.AccountSelected}_bp{model.SelectedBusinessPartnerNumber}", out _cachedIBANData))
                    {
                        #region [MIM Payment Implementation]

                        var payRequest = new CipherPaymentModel();
                        payRequest.PaymentData.amounts = _cachedIBANData.PaymentAmountList;
                        payRequest.PaymentData.contractaccounts = _cachedIBANData.PaymentAccountList;
                        payRequest.PaymentData.easypayflag = DewaPaymentChannel.RR;
                        payRequest.ServiceType = ServiceType.PayBill;
                        payRequest.PaymentMethod = model.paymentMethod;
                        payRequest.BankKey = model.bankkey;
                        payRequest.SuqiaValue = model.SuqiaDonation;
                        payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                        var payResponse = ExecutePaymentGateway(payRequest);
                        if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                        {
                            CacheProvider.Store(CacheKeys.IBANDetail_PaymentData, new CacheItem<UpdateIBANModel>(_cachedIBANData, TimeSpan.FromHours(1)));
                            CacheProvider.Store(CacheKeys.PAYMENT_PATH, new CacheItem<string>(CacheKeys.IBANDetail_PaymentData, TimeSpan.FromHours(1)));

                            return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                        }
                        error = string.Join("\n", payResponse.ErrorMessages.Values.ToList());

                        #endregion [MIM Payment Implementation]
                    }
                    else
                    {
                        error = Translate.Text("IBANCacheError");
                    }
                }
                catch (System.Exception ex)
                {
                    error = ex.Message;
                }

                ModelState.AddModelError(string.Empty, error);
                model.IBANSList = PopulateUpdateibanOptions();
                return PartialView("~/Views/Feature/Bills/UpdateIBAN/_NewRequest.cshtml", model);
            }

            if (model.IbanRefundDocument != null && model.IbanRefundDocument.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.IbanRefundDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                    model.IBANSList = PopulateUpdateibanOptions();
                    return PartialView("~/Views/Feature/Bills/UpdateIBAN/_NewRequest.cshtml", model);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.IbanRefundDocument.InputStream.CopyTo(memoryStream);
                        model.Attachment = memoryStream.ToArray();
                    }
                }
            }
            CacheProvider.Store(CacheKeys.UPDATEIBAN, new CacheItem<UpdateIBANModel>(model));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_IBANPAGE_REVIEWS);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtp(SendOtpRequest request)
        {
            string errVerf = "";
            try
            {
                UpdateIBANModel model;
                if (CacheProvider.TryGet(CacheKeys.UPDATEIBAN, out model))
                {
                    //Send or Verify otp
                    var response = SmartCommunicationClient.CustomerVerifyOtp(new SmartCommunicationVerifyOtpRequest()
                    {
                        mode = request.mode,
                        sessionid = CurrentPrincipal.SessionToken,
                        reference = !string.IsNullOrEmpty(request.reqId) ? request.reqId : string.Empty,
                        prtype = request.prtype,
                        email = (!string.IsNullOrEmpty(request.type) && request.type.Equals("e")) ? model.Emailid : string.Empty,
                        mobile = (!string.IsNullOrEmpty(request.type) && request.type.Equals("m")) ? model.MobileNumber : string.Empty,
                        contractaccountnumber = string.Empty,
                        businesspartner = model.SelectedBusinessPartnerNumber,
                        otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : null
                    }, Request.Segment(), RequestLanguage); ;
                    if (response != null && response.Succeeded)
                    {
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
        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult Review()
        {
            UpdateIBANModel model;
            if (CacheProvider.TryGet(CacheKeys.UPDATEIBAN, out model))
            {
                switch (model.SelectedIBAN)
                {
                    case "w":
                        model.RefundThrough = Translate.Text("updateiban.westernunion");
                        break;
                    //case "upcq":
                    //    model.RefundThrough = Translate.Text("upcq");
                    //    break;
                    //case "upiban":
                    //    model.RefundThrough = Translate.Text("upiban");
                    //    break;
                    case "iban":
                        model.RefundThrough = Translate.Text("updateiban.iban");
                        string confirmnumber = !string.IsNullOrWhiteSpace(model.ConfirmIbanAccountNumber) ? model.ConfirmIbanAccountNumber : model.ConfirmIbanAccountNumber2;
                        model.IbanNumber = string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, confirmnumber);
                        break;

                    case "upiban":
                        model.RefundThrough = Translate.Text("updateiban.iban");
                        string confirmnumber1 = !string.IsNullOrWhiteSpace(model.ConfirmIbanAccountNumber) ? model.ConfirmIbanAccountNumber : model.ConfirmIbanAccountNumber2;
                        model.IbanNumber = string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, confirmnumber1);
                        break;

                    case "cq":
                        model.RefundThrough = Translate.Text("updateiban.cheque");
                        break;

                    case "t":
                        model.RefundThrough = Translate.Text("updateiban.anotheractive");
                        break;
                }
                return View("~/Views/Feature/Bills/UpdateIBAN/_RefundReviews.cshtml", model);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_IBANPAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Review(UpdateIBANModel model)
        {
            string OtpRequestId = model.OtpRequestId;
            if (CacheProvider.TryGet(CacheKeys.UPDATEIBAN, out model))
            {
                string error = string.Empty;
                model.OtpRequestId = OtpRequestId;
                string confirmnumber = !string.IsNullOrWhiteSpace(model.ConfirmIbanAccountNumber) ? model.ConfirmIbanAccountNumber : model.ConfirmIbanAccountNumber2;

                if (model.SelectedIBAN == "upcq")
                {
                    // var response = DewaApiClient.UpdateIBAN(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.AccountSelected, string.Empty, model.SelectedBusinessPartnerNumber, "CQ", model.Comments, RequestLanguage, Request.Segment());
                    var response = RefundHistoryClient.IbanNumberv2(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory.IBANNumberV2Request()
                    {
                        contractaccountnumber = model.AccountSelected,
                        businesspartner = model.SelectedBusinessPartnerNumber,
                        ibannumber = "",
                        chequeiban = "CQ",
                        address = model.Comments,
                        notificationnumber = "",
                        skipcaflag = "",
                        bankdetailsid = "",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,
                    }, Request.Segment(), RequestLanguage);

                    model.Successful = response.Succeeded;
                    if (!model.Successful)
                    {
                        model.Errormessage = response.Message;
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(response.Message));
                    }
                    else
                    {
                        model.Successfulmessage = Translate.Text("updateiban.updatecheckmessage");
                    }
                }
                else if (model.SelectedIBAN == "upiban")
                {
                    var IBANRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory.IBANNumberV2Request()
                    {
                        contractaccountnumber = model.AccountSelected,
                        businesspartner = model.SelectedBusinessPartnerNumber,
                        ibannumber = string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, confirmnumber),
                        chequeiban = "IBAN",
                        address = "",
                        notificationnumber = "",
                        skipcaflag = "",
                        bankdetailsid = "",
                        validateibanflag = "X",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,
                    };

                    #region [validate IBAN NO]

                    var responseIbanValidate = RefundHistoryClient.IbanNumberv2(IBANRequest, Request.Segment(), RequestLanguage);

                    model.Successful = responseIbanValidate.Succeeded;
                    model.Errormessage = responseIbanValidate.Message;

                    #endregion [validate IBAN NO]

                    if (model.Successful)
                    {
                        IBANRequest.validateibanflag = null;
                        //var response = DewaApiClient.UpdateIBAN(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.AccountSelected, string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, confirmnumber), model.SelectedBusinessPartnerNumber, "IBAN", string.Empty, RequestLanguage, Request.Segment());
                        var response = RefundHistoryClient.IbanNumberv2(IBANRequest, Request.Segment(), RequestLanguage);
                        model.Successful = response.Succeeded;
                        model.Errormessage = response.Message;
                    }

                    LogService.Debug("RefundHistoryClient.IbanNumberv2:" + model.Errormessage);
                    if (!model.Successful)
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(model.Errormessage));
                    }
                    else
                    {
                        model.Errormessage = null;
                        model.Successfulmessage = Translate.Text("updateiban.updateibanmessage");
                    }
                }
                else if (model.SelectedIBAN == "iban" || model.SelectedIBAN == "cq" || model.SelectedIBAN == "t" || model.SelectedIBAN == "w")
                {
                    string refund = "";
                    if (model.SelectedIBAN.Equals("cq"))
                    {
                        refund = "C";
                    }
                    else if (model.SelectedIBAN.Equals("iban"))
                    {
                        refund = "I";
                    }
                    else if (model.SelectedIBAN.Equals("w"))
                    {
                        refund = "Q";
                    }
                    else
                    {
                        refund = "T";
                    }
                    List<DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut.Moveout_AccountsIn_Request> accountlist = new List<DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut.Moveout_AccountsIn_Request>();
                    string[] selectedaccount = null;

                    selectedaccount = new string[] { model.AccountSelected };
                    Array.ForEach(selectedaccount, x => accountlist.Add(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut.Moveout_AccountsIn_Request
                    {
                        contractaccountnumber = x.ToString(),
                        premise = "",
                        disconnectiondate = System.DateTime.Now.ToString("yyyyMMdd"),
                        businesspartnernumber = model.SelectedBusinessPartnerNumber,
                        refundmode = refund,
                        ibannumber = model.SelectedIBAN.Equals("iban") ? string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, confirmnumber) : string.Empty,
                        transferaccountnumber = model.SelectedIBAN.Equals("t") ? model.transferaccount : string.Empty,
                        countrykey = model.SelectedCountry,
                        city = model.SelectedCity,
                        currencykey = model.SelectedCurrency,
                        region = model.SelectedState,
                        additionalinput2 = model.OtpRequestId
                    }));

                    ServiceResponse<AccountDetails[]> cacheresponse;
                    DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut.MoveoutRequest moveoutParam = null;
                    if (!CacheProvider.TryGet("RF", out cacheresponse) || cacheresponse.Payload.Length < 1)
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_IBANPAGE);
                    }
                    if (cacheresponse.Payload.Where(x => FormatContractAccount(x.AccountNumber).Equals(FormatContractAccount(model.AccountSelected))).FirstOrDefault().BillingClass.Equals(DEWAXP.Foundation.Integration.Enums.BillingClassification.ElectricVehicle))
                    {
                        moveoutParam = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut.MoveoutRequest()
                        {
                            accountlist = accountlist,
                            channel = "M",
                            executionflag = "W",
                            applicationflag = "R",
                            notificationtype = "EV",
                        };
                    }
                    else
                    {
                        moveoutParam = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.MoveOut.MoveoutRequest()
                        {
                            accountlist = accountlist,
                            channel = "M",
                            executionflag = "W",
                            applicationflag = "M",
                            notificationtype = "RF",
                        };
                    }
                    if (moveoutParam != null)
                    {
                        moveoutParam.disconnectiondate = DateTime.Now.ToString("yyyyMMdd");
                        moveoutParam.mobile = model.MobileNumber.AddMobileNumberZeroPrefix();
                        moveoutParam.attachment = Convert.ToBase64String(model.Attachment != null ? model.Attachment.ToArray() : new byte[0]);
                        moveoutParam.attachmenttype = model.IbanRefundDocument != null && !string.IsNullOrWhiteSpace(model.IbanRefundDocument.FileName) ? model.IbanRefundDocument.FileName.Split('.')[1] : string.Empty;
                        moveoutParam.sessionid = CurrentPrincipal.SessionToken;
                        moveoutParam.userid = CurrentPrincipal.UserId;
                    }
                    var responsemoveout = MoveOutClient.SetMoveOutRequestV2(moveoutParam, Request.Segment(), RequestLanguage);
                    model.Successful = responsemoveout.Succeeded;
                    if (!model.Successful)
                    {
                        model.Errormessage = responsemoveout.Message;
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(responsemoveout.Message));
                    }
                    else
                    {
                        model.Notificationnumber = responsemoveout.Payload.notificationlist[0].notificationnumber;
                        if (model.SelectedIBAN == "iban")
                        {
                            model.Successfulmessage = Translate.Text("updateiban.ibansuccessmessage");
                        }
                        else if (model.SelectedIBAN == "cq")
                        {
                            model.Successfulmessage = Translate.Text("updateiban.chequesuccessmessage");
                        }
                        else if (model.SelectedIBAN == "t")
                        {
                            model.Successfulmessage = Translate.Text("updateiban.transfersuccessmessage");
                        }
                        else if (model.SelectedIBAN == "w")
                        {
                            model.Successfulmessage = Translate.Text("updateiban.westernsuccessmessage");
                        }
                    }
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_IBANPAGE_CONFIRM);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_IBANPAGE);
            }
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult Confirm()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            UpdateIBANModel Successmodel;
            if (CacheProvider.TryGet(CacheKeys.UPDATEIBAN, out Successmodel))
            {
                CacheProvider.Remove(CacheKeys.UPDATEIBAN);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_IBANPAGE);
            }
            //var context = SitecoreContext.GetItem<Models.Renderings.AccountSelector>("{4458B6E2-7A59-4C6C-8CAB-B80205B42DEA}");
            //ServiceResponse<AccountDetails[]> accresponse = null;
            //if (!string.IsNullOrEmpty(context.ServiceFlag))
            //{
            //    CacheProvider.TryGet(context.ServiceFlag, out accresponse);
            //}
            //var accountFromService = accresponse.Payload.FirstOrDefault(x => x.AccountNumber == Successmodel.AccountSelected);
            ////var accountFromService = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment()).Payload
            ////        .FirstOrDefault(x => x.AccountNumber == Successmodel.AccountSelected);

            //var account = Account.CreateFrom(accountFromService);

            return PartialView("~/Views/Feature/Bills/UpdateIBAN/_Confirm.cshtml", new ConfirmModel
            {
                IsSuccess = Successmodel.Successful,
                Message = Successmodel.Successfulmessage,
                ErrorMessage = Successmodel.Errormessage,
                Notification = Successmodel.Notificationnumber
                //Notifications = Successmodel.@return.notificationlist.Select(x => new ConfirmNotificationModel { ContractAccountNumber = x.contractaccountnumber, Message = x.message, NotificationNumber = x.notificationnumber }).ToArray(),
                // Account = account
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartailIBANUpdateNewRequest(PartailUpdateIBANRequestModel model)
        {
            string _IbanKey = CacheKeys.UPDATEIBAN + model.SelectedRefernceNo;
            bool IsSuccessful = false;
            CacheProvider.Remove(_IbanKey);
            string Message = "";

            if (model.IBANNumber.Equals(model.IBANConfirmNumber))
            {
                bool IsValidIBAN = false;

                #region [validate IBAN NO]

                var responseIbanValidate = RefundHistoryClient.IbanNumberv2(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory.IBANNumberV2Request()
                {
                    contractaccountnumber = model.SelectedAccount,
                    businesspartner = model.SelectedBusinessPartnerNumber,
                    ibannumber = string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, model.IBANConfirmNumber),
                    chequeiban = "IBAN",
                    address = "",
                    notificationnumber = "",
                    skipcaflag = "",
                    bankdetailsid = "",
                    validateibanflag = "X",
                    sessionid = CurrentPrincipal.SessionToken,
                    userid = CurrentPrincipal.UserId,
                }, Request.Segment(), RequestLanguage);

                IsValidIBAN = responseIbanValidate.Succeeded;
                Message = responseIbanValidate.Message;

                #endregion [validate IBAN NO]

                if (IsValidIBAN)
                {
                    #region [Retrieve otp details]

                    //check IBAN Is properly Assigned to CA .
                    var verifyRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory.RefundHistoryVerifyOtpRequest()
                    {
                        mode = "R",
                        lang = RequestLanguageCode,
                        sessionid = CurrentPrincipal.SessionToken,
                        reference = model.SelectedRefernceNo,
                        prtype = "IBRF",
                        contractaccountnumber = model.SelectedAccount,
                        businesspartner = model.SelectedBusinessPartnerNumber,
                    };

                    var returnData = RefundHistoryClient.VerifyOtp(verifyRequest);

                    IsSuccessful = returnData.Succeeded &&
                        (Convert.ToInt32(returnData.Payload.emaillist?.Count ?? 0) > 0 || Convert.ToInt32(returnData.Payload.mobilelist?.Count ?? 0) > 0);
                    if (IsSuccessful)
                    {
                        var email = returnData.Payload.emaillist?.FirstOrDefault() ?? null;
                        var mobile = returnData.Payload.mobilelist?.FirstOrDefault() ?? null;

                        if (email != null && !string.IsNullOrWhiteSpace(email.unmaskedemail))
                        {
                            model.EmailAddess = email.unmaskedemail;
                            model.MaskedEmailAddess = email.maskedemail;
                        }
                        if (mobile != null && !string.IsNullOrWhiteSpace(mobile.unmaskedmobile))
                        {
                            model.Mobile = mobile.unmaskedmobile;
                            model.MaskedMobile = mobile.maskedmobile;
                        }

                        CacheProvider.Store(_IbanKey, new CacheItem<PartailUpdateIBANRequestModel>(model, TimeSpan.FromHours(1)));
                    }
                    else
                    {
                        Message = returnData.Message;
                    }

                    #endregion [Retrieve otp details]
                }
            }
            else
            {
                Message = Translate.Text("updateiban.ibanmissmatchmessage");
            }

            return Json(new
            {
                success = IsSuccessful,
                message = Message,
                data = new
                {
                    m = !string.IsNullOrWhiteSpace(model.MaskedMobile) ? model.MaskedMobile : GetMaskedMobileNo(model.Mobile),
                    e = !string.IsNullOrWhiteSpace(model.MaskedEmailAddess) ? model.MaskedEmailAddess : GetMaskedEmail(model.EmailAddess),
                    ib = GetMaskedIBAN(string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, model.IBANConfirmNumber))
                }
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartailIBANUpdateOtpRequest(PartailUpdateIBANRequestModel model)
        {
            string _IbanKey = CacheKeys.UPDATEIBAN + model.SelectedRefernceNo;
            PartailUpdateIBANRequestModel _saveData = null;
            bool IsSuccessful = false;
            string Message = string.Empty;
            string MaxAttempt = string.Empty;

            if (CacheProvider.TryGet(_IbanKey, out _saveData))
            {
                //send otp
                var r = RefundHistoryClient.VerifyOtp(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory.RefundHistoryVerifyOtpRequest()
                {
                    mode = "V",
                    lang = RequestLanguageCode,
                    sessionid = CurrentPrincipal.SessionToken,
                    reference = model.SelectedRefernceNo,
                    prtype = "IBRF",
                    mobile = _saveData.Mobile,
                    email = _saveData.EmailAddess,
                    contractaccountnumber = _saveData.SelectedAccount,
                    businesspartner = _saveData.SelectedBusinessPartnerNumber,
                    otp = model.OTP
                });
                MaxAttempt = r.Payload.maxattempts;
                IsSuccessful = r.Succeeded && MaxAttempt != "X";

                if (IsSuccessful)
                {
                    CacheProvider.Remove(CacheKeys.SELECTED_REFUND);

                    #region [IBAN Update & OTP Detail Retrieve]

                    var response = RefundHistoryClient.IbanNumberv2(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory.IBANNumberV2Request()
                    {
                        contractaccountnumber = _saveData.SelectedAccount,
                        businesspartner = _saveData.SelectedBusinessPartnerNumber,
                        ibannumber = string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, _saveData.IBANConfirmNumber),
                        chequeiban = "IBAN",
                        address = "",
                        notificationnumber = model.SelectedRefernceNo,
                        skipcaflag = "",
                        bankdetailsid = "",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,
                    }, Request.Segment(), RequestLanguage);

                    if (response.Succeeded)
                    {
                        CacheProvider.Remove(CacheKeys.SELECTED_REFUND);
                        Message = Translate.Text("updateiban.updateibanmessage");
                    }
                    else
                    {
                        Message = response.Message;
                    }

                    #endregion [IBAN Update & OTP Detail Retrieve]
                }
                else
                {
                    Message = r.Message;
                }
            }
            else
            {
                Message = Translate.Text("updateiban.Ibaninvalidrequest");
            }

            return Json(new
            {
                success = IsSuccessful,
                message = Message,
                data = new
                {
                    m = !string.IsNullOrWhiteSpace(_saveData.MaskedMobile) ? _saveData.MaskedMobile : GetMaskedMobileNo(_saveData.Mobile),
                    e = !string.IsNullOrWhiteSpace(_saveData.MaskedEmailAddess) ? _saveData.MaskedEmailAddess : GetMaskedEmail(_saveData.EmailAddess),
                    ib = GetMaskedIBAN(string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, _saveData?.IBANConfirmNumber)),
                    maxattempt = MaxAttempt
                }
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PartailIBANUpdateResendOtpRequest(PartailUpdateIBANRequestModel model)
        {
            string _IbanKey = CacheKeys.UPDATEIBAN + model.SelectedRefernceNo;
            PartailUpdateIBANRequestModel _saveData = null;
            bool IsSuccessful = false;
            string Message = string.Empty;

            if (CacheProvider.TryGet(_IbanKey, out _saveData))
            {
                //send otp
                var r = RefundHistoryClient.VerifyOtp(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory.RefundHistoryVerifyOtpRequest()
                {
                    mode = "S",
                    lang = RequestLanguageCode,
                    sessionid = CurrentPrincipal.SessionToken,
                    reference = model.SelectedRefernceNo,
                    prtype = "IBRF",
                    mobile = _saveData.Mobile,
                    email = _saveData.EmailAddess,
                    contractaccountnumber = _saveData.SelectedAccount,
                    businesspartner = _saveData.SelectedBusinessPartnerNumber,
                });

                IsSuccessful = r.Succeeded;
                if (IsSuccessful)
                {
                    Message = Translate.Text("updateiban_resendotpscuccess");
                }
                else
                {
                    Message = r.Message;
                }

                CacheProvider.Store(_IbanKey, new CacheItem<PartailUpdateIBANRequestModel>(_saveData, TimeSpan.FromHours(1)));
            }
            else
            {
                Message = Translate.Text("updateiban.Ibaninvalidrequest");
            }

            return Json(new
            {
                success = IsSuccessful,
                message = Message,
                data = new
                {
                    m = !string.IsNullOrWhiteSpace(_saveData.MaskedMobile) ? _saveData.MaskedMobile : GetMaskedMobileNo(_saveData.Mobile),
                    e = !string.IsNullOrWhiteSpace(_saveData.MaskedEmailAddess) ? _saveData.MaskedEmailAddess : GetMaskedEmail(_saveData.EmailAddess),
                    ib = GetMaskedIBAN(string.Format("{0}{1}", GenericConstants.MoveOutIbanPrefix, _saveData?.IBANConfirmNumber))
                }
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateRefundNameSubmit(UpdateRefundNameSubmitModel model)
        {
            var apiRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundNameChange.RefundNameChangeRequest()
            {
                notificationnumber = model.notificationnumber,
                fullnamenew = model.fullnamenew,
                fullnameold = model.fullnameold,
                mode = model.mode,
                userid = CurrentPrincipal.UserId,
                sessionid = CurrentPrincipal.SessionToken,
                lang = RequestLanguageCode
            };

            if (apiRequest.mode == "W")
            {
                if (model.isuae)
                {
                    apiRequest.emiratesid = model.emiratesid;
                }
                else
                {
                    apiRequest.passportnumber = model.passportnumber;

                    if (!string.IsNullOrWhiteSpace(model.dateofbirth))
                    {
                        apiRequest.dateofbirth = CommonUtility.DateTimeFormatParse(CommonUtility.ConvertDateArToEn(model.dateofbirth), "dd MMMM yyyy").ToString("yyyyMMdd");
                    }

                    apiRequest.nationality = model.nationality;
                }

                if (model.attachment != null)
                {
                    apiRequest.filename = model.attachment.FileName;
                    apiRequest.attachment = Convert.ToBase64String(model.attachment != null ? model.attachment.ToArray() : new byte[0]);
                }
            }
            CacheProvider.Remove(CacheKeys.SELECTED_REFUND);
            var r = RefundNameChangeClient.RefundNameChange(apiRequest, Request.Segment(), RequestLanguage);
            return Json(new
            {
                success = r.Succeeded,
                message = r.Message,
                data = r.Payload
            });
        }

        private string GetMaskedIBAN(string plainIban)
        {
            string maskIban = null;
            if (!string.IsNullOrWhiteSpace(plainIban) && plainIban.Length > 20)
            {
                maskIban = string.Format("{0}*****00*****{1}", plainIban.Substring(0, 5), plainIban.Substring(17, plainIban.Length - 17));
            }
            return maskIban;
        }

        private string GetMaskedEmail(string plainEmail)
        {
            string maskEmail = null;
            if (!string.IsNullOrWhiteSpace(plainEmail) && plainEmail.Contains("@"))
            {
                maskEmail = string.Format("{0}XXXX{1}", plainEmail[0], plainEmail.Substring(plainEmail.IndexOf('@') - 1));
            }

            return maskEmail;
        }

        private string GetMaskedMobileNo(string plainMobileNo)
        {
            string maskMobileNo = null;
            if (!string.IsNullOrWhiteSpace(plainMobileNo))
            {
                maskMobileNo = string.Format("{0}XXX{1}", plainMobileNo.Substring(0, 3), plainMobileNo.Substring(6, plainMobileNo.Length - 6));
            }

            return maskMobileNo;
        }

        #region Refund History

        [AcceptVerbs("GET", "HEAD")]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult RefundHistory()
        {
            ViewBag.RefundTypes = GetLstDataSource(DataSources.REFUNDHISTORY_FILTERS);
            return PartialView("~/Views/Feature/Bills/UpdateIBAN/_RefundHistory.cshtml");
        }

        #endregion Refund History

        private IEnumerable<SelectListItem> PopulateUpdateibanOptions()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.UPDATEIBAN_OPTIONS));
                var refundOptions = dataSource.Items;
                var convertedItems = refundOptions.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("TransferTo DataSource is Null");
            }
        }
    }
}