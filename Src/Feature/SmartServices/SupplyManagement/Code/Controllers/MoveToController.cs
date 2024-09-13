using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Feature.SupplyManagement.Models.MoveIn;
using DEWAXP.Feature.SupplyManagement.Models.MoveTo;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using Glass.Mapper.Sc;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;
using DEWAXP.Foundation.Content.Models.SupplyManagement.MoveTo;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class MoveToController : BaseController
    {
        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = true)]
        public ActionResult MoveToLanding()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            var model = new MoveToAccount();
            MoveToWorkflowState state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out state))
            {
                model.SelectedAccount = state.Account;
                model.DisconnectDate = state.MoveOuttDate.HasValue ? state.MoveOuttDate.Value.ToString("dd MMMM, yyyy") : string.Empty;
                model.DisconnectDateAsDateTime = state.MoveOuttDate;
                model.MobileNumber = state.MobilePhone;
                model.AccountNumber = state.Account.AccountNumber;
            }
            else
            {
                state = new MoveToWorkflowState();
            }
            state.page = string.Empty;
            CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(state, TimeSpan.FromMinutes(20)));
            return PartialView("~/Views/Feature/SupplyManagement/MoveTo/_MoveToAccounts.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MoveToLanding(MoveToAccount account)
        {
            MoveToWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            state.MoveOuttDate = account.DisconnectDateAsDateTime;
            state.MobilePhone = account.MobileNumber;
            state.page = "details";
            CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(state, TimeSpan.FromMinutes(20)));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START_DETAILS);
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = true)]
        public ActionResult MoveToDetails()
        {
            MoveToWorkflowState State;
            bool moveinindicator = false;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out State))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            if (string.IsNullOrWhiteSpace(State.page) || !State.page.Equals("details"))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            if (CacheProvider.TryGet(CacheKeys.MOVEIN_INDICATOR, out moveinindicator))
            {
                State.moveinindicator = true;
                CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(State, TimeSpan.FromMinutes(20)));
                CacheProvider.Remove(CacheKeys.MOVEIN_INDICATOR);
            }
            BusinessPartner bp = new BusinessPartner();
            var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            bp = UserDetails.Payload.BusinessPartners.Where(x => x.businesspartnernumber == "00" + State.Account.BusinessPartner).FirstOrDefault();
            if (bp == null)
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(Translate.Text("moveto.businesspartnernull")));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            // bp.CustomerType.Substring(0,1)
            var model = new MoveToAccountDetailsViewModel
            {
                MovoutAccountnumber = State.Account.AccountNumber,
                BusinessPartner = State.Account.BusinessPartner,
                AccountType = State.Account.BillingClass,
                MoveoutAccountDate = State.MoveOuttDate,
                UserId = CurrentPrincipal.UserId,
                SessionToken = CurrentPrincipal.SessionToken,
                CustomerCategory = !string.IsNullOrWhiteSpace(State.CustomerCategory) ? State.CustomerCategory : bp.CustomerType.Substring(0, 1).Equals("P") ? "P" : "O",
                CustomerType = State.CustomerType,
                IdType = !string.IsNullOrWhiteSpace(State.IdType) ? State.IdType : (!string.IsNullOrWhiteSpace(State.Ejari) ? "EJ" : string.Empty),
                IdNumber = !string.IsNullOrWhiteSpace(State.IdNumber) ? State.IdNumber : (!string.IsNullOrWhiteSpace(State.Ejari) ? State.Ejari : string.Empty),
                DocumentExpiryDate = State.ExpiryDate,
                loggedinuser = this.IsLoggedIn ? true : false,
                NumberOfRooms = State.NumberOfRooms,
                owner = State.Owner,
                MoveinStartDate = State.MoveinStartDate,
                PremiseNo = State.PremiseAccount,
                SelectedIDType = State.IdType,
                Moveinindicator = State.moveinindicator,
                VatNumber = State.VatNumber
            };
            ViewBag.NumberOfRoomsBag = GetNumberOfRooms();
            if (model.CustomerCategory == "P")
            {
                ViewBag.IdTypes = GetResMoveinIdTypes();
                ViewBag.CustomerTypes = GetResMoveinCustomerTypes();
                return PartialView("~/Views/Feature/SupplyManagement/MoveTo/_MoveToResAccounts.cshtml", model);
            }
            else
            {
                ViewBag.IdTypes = GetNonResMoveinIdTypes();
                ViewBag.CustomerTypes = GetNonResMoveinCustomerTypes();
                return PartialView("~/Views/Feature/SupplyManagement/MoveTo/_MoveToNonResAccounts.cshtml", model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MoveToDetails(MoveToAccountDetailsViewModel account)
        {
            //Added for Future Center
            var _fc = FetchFutureCenterValues();

            MoveToWorkflowState State;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out State))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            if (string.IsNullOrWhiteSpace(State.page) || !State.page.Equals("details"))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            string idtype = account.SelectedIDType;
            string idnumber = account.IdNumber;
            string ejarinumber = string.Empty;
            string ejaritype = string.Empty;

            if (string.Equals(idtype, "EJ"))
            {
                ejaritype = idtype;
                idtype = string.Empty;
                ejarinumber = idnumber;
                idnumber = string.Empty;
            }
            string error = "";
            if (account.PassportDocument != null && account.PassportDocument.ContentLength > 0)
            {
                if (!AttachmentIsValid(account.PassportDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        account.PassportDocument.InputStream.CopyTo(memoryStream);
                        State.Attachment1 = memoryStream.ToArray();
                        State.Attachment1Filename = account.PassportDocument.FileName.GetFileNameWithoutPath();
                        State.Attachment1FileType = AttachmentTypeCodes.Passport;
                    }
                }
            }
            if (account.OwnerDocument != null && account.OwnerDocument.ContentLength > 0)
            {
                if (!AttachmentIsValid(account.OwnerDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        account.OwnerDocument.InputStream.CopyTo(memoryStream);
                        State.Attachment2 = memoryStream.ToArray();
                        State.Attachment2Filename = account.OwnerDocument.FileName.GetFileNameWithoutPath();
                        State.Attachment2FileType = AttachmentTypeCodes.PurchaseAgreement;
                    }
                }
            }
            if (account.PurchaseAgreement != null && account.PurchaseAgreement.ContentLength > 0)
            {
                if (!AttachmentIsValid(account.PurchaseAgreement, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        account.PurchaseAgreement.InputStream.CopyTo(memoryStream);
                        State.Attachment3 = memoryStream.ToArray();
                        State.Attachment3Filename = account.PurchaseAgreement.FileName.GetFileNameWithoutPath();
                        State.Attachment3FileType = AttachmentTypeCodes.PurchaseAgreement;
                    }
                }
            }
            if (account.TradeLicense != null && account.TradeLicense.ContentLength > 0)
            {
                if (!AttachmentIsValid(account.TradeLicense, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        account.TradeLicense.InputStream.CopyTo(memoryStream);
                        State.Attachment4 = memoryStream.ToArray();
                        State.Attachment4Filename = account.TradeLicense.FileName.GetFileNameWithoutPath();
                        State.Attachment4FileType = AttachmentTypeCodes.TradeLicense;
                    }
                }
            }
            if (account.VatDocument != null && account.VatDocument.ContentLength > 0)
            {
                if (!AttachmentIsValid(account.VatDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        account.VatDocument.InputStream.CopyTo(memoryStream);
                        State.Attachment5 = memoryStream.ToArray();
                        State.Attachment5Filename = account.VatDocument.FileName.GetFileNameWithoutPath();
                        State.Attachment5FileType = AttachmentTypeCodes.VatDocument;
                    }
                }
            }
            State.moveinindicator = account.Moveinindicator;
            State.VatNumber = account.VatNumber;
            State.PremiseAccount = account.PremiseNo;
            State.IdType = idtype;
            State.CustomerType = account.CustomerType;
            State.IdNumber = idnumber;
            State.ExpiryDate = account.DocumentExpiryDate ?? Convert.ToDateTime(account.DocumentExpiryDate);
            State.MoveinStartDate = Convert.ToDateTime(System.DateTime.Now);
            State.CustomerCategory = account.CustomerCategory;
            State.Ejari = ejarinumber;
            State.NumberOfRooms = account.NumberOfRooms;
            State.BusinessPartner = State.Account.BusinessPartner;
            CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(State, TimeSpan.FromMinutes(20)));
            var response = DewaApiClient.MoveInEjariRequest(
           CurrentPrincipal.UserId,
           string.Empty,
           string.Empty,
           string.Empty,
           CurrentPrincipal.SessionToken ?? string.Empty,
           State.CustomerType,
           State.IdType,
           State.IdNumber,
           string.Empty,
           string.Empty,
           string.Empty,
           string.Empty,
           State.MobilePhone.AddMobileNumberZeroPrefix(),
           string.Empty,
           State.MoveinStartDate,
           State.MoveOuttDate,
           State.Account.AccountNumber,
           null,
           null,
           State.BusinessPartner,
           State.ExpiryDate,
           string.Empty,
           State.CustomerCategory,
           null,
           string.Empty,
           this.IsLoggedIn ? "R" : "A",
           State.Ejari,
           new string[] { State.PremiseAccount },
           "X",
           State.VatNumber,
           RequestLanguage,
           Request.Segment(), _fc.Branch);
            if (response.Succeeded)
            {
                if (State.Attachment1 != null && State.Attachment1.Length > 0)
                {
                    DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, new string[] { response.Payload.moveinEjari.moveToTransactionID }, State.Attachment1Filename, State.Attachment1FileType, State.Attachment1);
                }
                if (State.Attachment2 != null && State.Attachment2.Length > 0)
                {
                    DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, new string[] { response.Payload.moveinEjari.moveToTransactionID }, State.Attachment2Filename, State.Attachment2FileType, State.Attachment2);
                }
                if (State.Attachment3 != null && State.Attachment3.Length > 0)
                {
                    DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, new string[] { response.Payload.moveinEjari.moveToTransactionID }, State.Attachment3Filename, State.Attachment3FileType, State.Attachment3);
                }
                if (State.Attachment4 != null && State.Attachment4.Length > 0)
                {
                    DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, new string[] { response.Payload.moveinEjari.moveToTransactionID }, State.Attachment4Filename, State.Attachment4FileType, State.Attachment4);
                }
                if (State.Attachment5 != null && State.Attachment5.Length > 0)
                {
                    DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, new string[] { response.Payload.moveinEjari.moveToTransactionID }, State.Attachment5Filename, State.Attachment5FileType, State.Attachment5);
                }
                if (State.CustomerType == "01" && !string.Equals(ejaritype, "EJ"))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVETO_TENANT_DETAILS);
                }
                CacheProvider.Store(CacheKeys.MOVETOPAYMENT, new CacheItem<string>("movetopaymentprocess"));
                decimal outstandingbalance;
                decimal.TryParse(response.Payload.moveinEjari.outStandingAmount, out outstandingbalance);
                decimal securitydeposit;
                decimal.TryParse(response.Payload.moveinEjari.securityDepositAmount, out securitydeposit);
                if (outstandingbalance > 0)
                {
                    #region [MIM Payment Implementation]

                    var payRequest = new CipherPaymentModel();
                    payRequest.PaymentData.amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount));
                    payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber)));
                    payRequest.ServiceType = ServiceType.PayBill;
                    payRequest.PaymentMethod = account.paymentMethod;
                    payRequest.BankKey = account.bankkey;
                    payRequest.SuqiaValue = account.SuqiaDonation;
                    payRequest.SuqiaAmt = account.SuqiaDonationAmt;
                    var payResponse = ExecutePaymentGateway(payRequest);
                    if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                    {
                        return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                    }
                    ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                    #endregion [MIM Payment Implementation]
                }
                else if (securitydeposit > 0)
                {
                    #region [MIM Payment Implementation]

                    var payRequest = new CipherPaymentModel();
                    payRequest.PaymentData.amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount));
                    payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber)));
                    payRequest.PaymentData.businesspartner = State.BusinessPartner ?? string.Empty;
                    payRequest.PaymentData.email = State.EmailAddress;
                    payRequest.PaymentData.mobile = State.MobilePhone.AddMobileNumberZeroPrefix();
                    payRequest.PaymentData.userid = CurrentPrincipal.UserId;
                    payRequest.IsThirdPartytransaction = true;
                    payRequest.ServiceType = ServiceType.ServiceActivation;
                    payRequest.PaymentMethod = account.paymentMethod;
                    payRequest.BankKey = account.bankkey;
                    payRequest.SuqiaValue = account.SuqiaDonation;
                    payRequest.SuqiaAmt = account.SuqiaDonationAmt;
                    var payResponse = ExecutePaymentGateway(payRequest);
                    if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                    {
                        return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                    }
                    ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                    #endregion [MIM Payment Implementation]
                }
                else
                {
                    State.ResponseMessage = response.Payload.moveinEjari.description;
                    State.ErrorMessage = response.Message;
                    State.Succeeded = response.Succeeded;
                    State.page = "confirm";
                    CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(State, TimeSpan.FromMinutes(20)));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_CONFIRM);
                }
            }
            else
            {
                if (response.Payload != null && response.Payload.moveinEjari != null && response.Payload.moveinEjari.moveInIndicator == "000010")
                {
                    CacheProvider.Store(CacheKeys.MOVEIN_INDICATOR, new CacheItem<bool>(true));
                }
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(response.Message));
            }
            State.page = "details";
            CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(State, TimeSpan.FromMinutes(20)));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START_DETAILS);
        }

        [HttpGet]
        public ActionResult MoveToTenantPage()
        {
            MoveToWorkflowState State;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out State))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            if (string.IsNullOrWhiteSpace(State.page) || !State.page.Equals("tenant"))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }

            ViewBag.NumberOfRoomsBag = GetNumberOfRooms();

            var model = new TenancyDetailsViewModel
            {
                ContractStartDate = State.ContractStartDate,
                ContractEndDate = State.ContractEndDate,
                NumberOfRooms = State.NumberOfRooms,
                ContractValue = State.ContractValue,
                CustomerType = State.CustomerType,
                ContractLabel1 = Translate.Text("Add Tenancy Contract")
            };
            return View("~/Views/Feature/SupplyManagement/MoveTo/TenancyDetails.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MoveToTenantPage(TenancyDetailsViewModel model)
        {
            //Added for Future Center
            var _fc = FetchFutureCenterValues();

            MoveToWorkflowState State;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out State))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            if (string.IsNullOrWhiteSpace(State.page) || !State.page.Equals("tenant"))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }

            if (!model.ContractStartDate.HasValue || !model.ContractEndDate.HasValue)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("activation.invalid.expat-contract-period"));
            }
            else
            {
                var delta = model.ContractEndDate.Value.Subtract(model.ContractStartDate.Value);
                if (delta.TotalDays < 30 || delta.TotalDays > 365)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("activation.invalid.expat-contract-period"));
                }
            }
            if (ModelState.IsValid)
            {
                State.ContractStartDate = model.ContractStartDate;
                State.ContractEndDate = model.ContractEndDate;
                State.NumberOfRooms = model.NumberOfRooms;
                State.ContractValue = model.ContractValue;
                string error = "";
                if (model.UploadContract != null && model.UploadContract.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.UploadContract, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.UploadContract.InputStream.CopyTo(memoryStream);

                            State.Attachment2 = memoryStream.ToArray();
                            State.Attachment2Filename = model.UploadContract.FileName.GetFileNameWithoutPath();
                            State.Attachment2FileType = AttachmentTypeCodes.TenancyContract;
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    var response = DewaApiClient.MoveInEjariRequest(
           CurrentPrincipal.UserId,
           string.Empty,
           string.Empty,
           string.Empty,
           CurrentPrincipal.SessionToken ?? string.Empty,
           State.CustomerType,
           State.IdType,
           State.IdNumber,
           string.Empty,
           string.Empty,
           string.Empty,
           string.Empty,
           State.MobilePhone.AddMobileNumberZeroPrefix(),
           string.Empty,
           State.MoveinStartDate,
           State.MoveOuttDate,
           State.Account.AccountNumber,
           State.ContractStartDate,
           State.ContractEndDate,
           State.BusinessPartner,
           State.ExpiryDate,
           State.ContractValue,
           State.CustomerCategory,
           State.NumberOfRooms,
           string.Empty,
           this.IsLoggedIn ? "R" : "A",
           State.Ejari,
           new string[] { State.PremiseAccount },
           "X",
           string.Empty,
           RequestLanguage,
           Request.Segment(), _fc.Branch);
                    if (!response.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                    else
                    {
                        if (State.Attachment2 != null && State.Attachment2.Length > 0)
                        {
                            DewaApiClient.SendMoveInAttachment(State.UserId, string.Empty, string.Empty, new string[] { response.Payload.moveinEjari.moveToTransactionID }, State.Attachment2Filename, State.Attachment2FileType, State.Attachment2);
                        }
                        CacheProvider.Store(CacheKeys.MOVETOPAYMENT, new CacheItem<string>("movetopaymentprocess"));
                        decimal outstandingbalance;
                        decimal.TryParse(response.Payload.moveinEjari.outStandingAmount, out outstandingbalance);
                        decimal securitydeposit;
                        decimal.TryParse(response.Payload.moveinEjari.securityDepositAmount, out securitydeposit);
                        if (outstandingbalance > 0)
                        {
                            //return this.RedirectToAction("Redirect", "Payment", new BillPaymentRequestModel
                            //{
                            //    Amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount)),
                            //    ContractAccounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber)))
                            //});

                            #region [MIM Payment Implementation]

                            var payRequest = new CipherPaymentModel();
                            payRequest.PaymentData.amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount));
                            payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber)));
                            payRequest.ServiceType = ServiceType.PayBill;
                            payRequest.PaymentMethod = model.paymentMethod;
                            payRequest.BankKey = model.bankkey;
                            payRequest.SuqiaValue = model.SuqiaDonation;
                            payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                            var payResponse = ExecutePaymentGateway(payRequest);
                            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                            {
                                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                            }
                            ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                            #endregion [MIM Payment Implementation]
                        }
                        else if (securitydeposit > 0)
                        {
                            //return RedirectToAction("RedirectForServiceActivation", "Payment", new ServiceActivationPaymentRequestModel()
                            //{
                            //    Amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount)),
                            //    BusinessPartnerNumber = State.BusinessPartner ?? string.Empty,
                            //    EmailAddress = State.EmailAddress,
                            //    PremiseNumber = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber))),
                            //    Username = CurrentPrincipal.UserId,
                            //    MobileNumber = State.MobilePhone.AddMobileNumberZeroPrefix(),
                            //    IsAnonymous = true
                            //});

                            #region [MIM Payment Implementation]

                            var payRequest = new CipherPaymentModel();
                            payRequest.PaymentData.amounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => x.amount));
                            payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.moveinEjari.premiseAmountList.ToList().Select(x => (!string.IsNullOrWhiteSpace(x.contractAccount) ? x.contractAccount : x.premiseNumber)));
                            payRequest.PaymentData.businesspartner = State.BusinessPartner ?? string.Empty;
                            payRequest.PaymentData.email = State.EmailAddress;
                            payRequest.PaymentData.mobile = State.MobilePhone.AddMobileNumberZeroPrefix();
                            payRequest.PaymentData.userid = CurrentPrincipal.UserId;
                            payRequest.IsThirdPartytransaction = true;
                            payRequest.ServiceType = ServiceType.ServiceActivation;
                            payRequest.PaymentMethod = model.paymentMethod;
                            payRequest.BankKey = model.bankkey;
                            payRequest.SuqiaValue = model.SuqiaDonation;
                            payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                            var payResponse = ExecutePaymentGateway(payRequest);
                            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                            {
                                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                            }
                            ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                            #endregion [MIM Payment Implementation]
                        }
                        else
                        {
                            State.ResponseMessage = response.Payload.moveinEjari.description;
                            State.ErrorMessage = response.Message;
                            State.Succeeded = response.Succeeded;
                            State.page = "confirm";
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_CONFIRM);
                        }
                    }
                }
            }

            ViewBag.NumberOfRoomsBag = GetNumberOfRooms();
            model.ContractLabel1 = Translate.Text("Add Tenancy Contract");
            //model.ContractLabel1 = Translate.Text("Add Purchase Agreement");
            //model.ContractLabel2 = Translate.Text("Add Purchase Agreement 2");
            model.ContractStartDate = State.ContractStartDate;
            model.ContractEndDate = State.ContractEndDate;
            model.NumberOfRooms = State.NumberOfRooms;
            model.ContractValue = State.ContractValue;
            model.CustomerType = State.CustomerType;
            return PartialView("~/Views/Feature/SupplyManagement/MoveTo/TenancyDetails.cshtml", model);
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult Confirm()
        {
            MoveToWorkflowState State;
            if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out State))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }
            if (string.IsNullOrWhiteSpace(State.page) || !State.page.Equals("confirm"))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
            }

            return PartialView("~/Views/Feature/SupplyManagement/MoveTo/_MoveToConfirm.cshtml", new MoveToConfirmModel
            {
                Account = State.Account,
                IsSuccess = State.Succeeded,
                Message = State.ResponseMessage,
                ErrorMessage = State.ErrorMessage
            });
        }

        [HttpGet]
        public ActionResult ClearMoveToCache()
        {
            MoveToWorkflowState state;
            if (CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVE_TO_WORKFLOW_STATE);
                CacheProvider.Remove(CacheKeys.MOVETOPAYMENT);
            }
            return new EmptyResult();
        }

        public List<SelectListItem> GetNumberOfRooms()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() {Text = Translate.Text(DictionaryKeys.MoveIn.Studio), Value = "1"},
                new SelectListItem() {Text = "1 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "2"},
                new SelectListItem() {Text = "2 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "3"},
                new SelectListItem() {Text = "3 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "4"},
                new SelectListItem() {Text = "4 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "5"},
                new SelectListItem() {Text = "5 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "6"},
                new SelectListItem() {Text = "6 " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "7"},
                new SelectListItem() {Text = "6+ " + Translate.Text(DictionaryKeys.MoveIn.Bedroom), Value = "8"}
            };
        }

        protected IEnumerable<SelectListItem> GetResMoveinIdTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.MOVE_IN_IDTYPES_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting ID Types");
            }
        }

        protected IEnumerable<SelectListItem> GetNonResMoveinIdTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.MOVE_IN_IDTYPES_NON_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting ID Types");
            }
        }

        public IEnumerable<SelectListItem> GetResMoveinCustomerTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.MOVE_IN_CUST_TYPES_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting ResMoveinCustomerTypes");
            }
        }

        public IEnumerable<SelectListItem> GetNonResMoveinCustomerTypes()
        {
            try
            {
                var dataSource = ContentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.MOVE_IN_CUST_TYPES_NON_RES));
                var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting NonResMoveinCustomerTypes");
            }
        }
    }

    

    public class MoveToValidator
    {
        private readonly MoveToWorkflowState _moveOut;

        public MoveToValidator(MoveToWorkflowState moveOut)
        {
            _moveOut = moveOut;
        }

        public IEnumerable<Result> ValidateSelectedAccount(SharedAccount selectedAccount)
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

        public class Result
        {
            public string Field { get; set; }

            public string Message { get; set; }
        }
    }
}