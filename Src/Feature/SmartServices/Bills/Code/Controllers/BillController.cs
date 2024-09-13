using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.Bills;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Impl;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Payment;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.KhadamatechDEWASvc;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using _ApiBillDownloaders = DEWAXP.Feature.Bills.Controllers.Api.BillDownloadsController;

namespace DEWAXP.Feature.Bills.Controllers
{
    public class BillController : BaseController
    {
        private readonly IPaymentChannelClient _paymentChannelClient;
        protected IPaymentChannelClient PaymentChannelApiClient => _paymentChannelClient;
        public BillController() : base()
        {
            _paymentChannelClient = DependencyResolver.Current.GetService<PaymentChannelClient>();
        }
        #region Transaction History

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult TransactionHistory()
        {
            ViewBag.TransactionFilters = GetLstDataSource(DataSources.TRANSACTIONHISTORY_FILTERS).ToList();
            ViewBag.DownloadHistory = GetLstDataSource(DataSources.TRANSACTIONHISTORY_DOWNLOADHISTORY);
            return PartialView("~/Views/Feature/Bills/Bill/_TransactionHistoryV2.cshtml");
        }

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult Receipts(string id = "", string date = "", string type = "", string docnumber = "")
        {
            if (!string.IsNullOrWhiteSpace(docnumber))
            {
                AccountCache transactionaccount;
                if (CacheProvider.TryGet(CacheKeys.SELECTED_TRANSACTIONACCOUNT, out transactionaccount))
                {
                    var response =
                        SmartCustomerClient.GetBillPaymentHistory(
                        new BillHistoryRequest
                        {
                            contractaccountnumber = transactionaccount.accountnumber,
                            sessionid = CurrentPrincipal.SessionToken,
                            userid = CurrentPrincipal.UserId
                        }, RequestLanguage, Request.Segment());
                    //DewaApiClient.GetPaymentHistory(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, transactionaccount.accountnumber, RequestLanguage, Request.Segment());
                    //var response = DewaApiClient.GetTransactionHistory(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        if (response.Payload != null && response.Payload.Responsecode != "105")
                        {
                            if (response.Payload.paymentlist != null)
                            {
                                var docrecipt = response.Payload.paymentlist.Where(x => x.Documentnumber.Equals(docnumber));
                                if (docrecipt.Any())
                                {
                                    var model = PaymentReceiptDetailModel.From(docrecipt.FirstOrDefault());
                                    return PartialView("~/Views/Feature/Bills/Bill/_Receipts.cshtml", model);
                                }
                            }
                        }
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(type))
            {
                DateTime parseddate;
                if (DateTime.TryParseExact(date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parseddate))
                {
                    var onlinereceipt = DewaApiClient.GetOnlinePaymentReceipt(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, id, date, type, RequestLanguage, Request.Segment());
                    if (onlinereceipt.Succeeded && onlinereceipt.Payload != null)
                    {
                        var model = PaymentReceiptDetailModel.From(onlinereceipt.Payload);
                        CacheProvider.Store(CacheKeys.ReceiptAccount, new CacheItem<paymentReceiptDetails>(onlinereceipt.Payload, TimeSpan.FromMinutes(30)));
                        return PartialView("~/Views/Feature/Bills/Bill/_Receipts.cshtml", model);
                    }
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J1_VIEW_PAST_BILLS);
        }

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult ReceiptsLoadmore(int page)
        {
            paymentReceiptDetails model;
            if (!CacheProvider.TryGet(CacheKeys.ReceiptAccount, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J1_VIEW_PAST_BILLS);
            }
            var paymentmodel = PaymentReceiptDetailModel.From(model, page);
            return PartialView("~/Views/Feature/Bills/Bill/_Receiptspartial.cshtml", paymentmodel);
        }

        #endregion Transaction History

        #region J14: Pay bills

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ViewAndPayBills()
        {
            // This action can be hit in response to other activities/journey results.
            // Under certain circumstances, we need to maintain the state of the view
            // and therefore NOT flush the cache.
            string paymentError;
            if (!CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out paymentError))
            {
                CacheProvider.Remove(CacheKeys.SELECTED_BILL_LIST);
                CacheProvider.Remove(CacheKeys.SELECTED_BILLS_TOTAL);
            }
            else
            {
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                ModelState.AddModelError(string.Empty, paymentError);
            }

            CacheProvider.Remove(CacheKeys.PAYMENT_METADATA);

            return PartialView("~/Views/Feature/Bills/Bill/_ViewAndPayBills.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ViewAndPayBills(BillSelector model)
        {
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J14_PAYBILL_REVIEW);
        }

        [HttpPost]
        public JsonResult ViewAndPayBills_ajax(string[] model)
        {
            if (model == null) return Json(false);
            if (model.Length <= 0) return Json(false);

            var selectedAccounts = model.ToArray();
            var totalPayable = 0;

            CacheProvider.Store(CacheKeys.SELECTED_BILL_LIST, new CacheItem<string[]>(selectedAccounts, TimeSpan.FromMinutes(10)));
            CacheProvider.Store(CacheKeys.SELECTED_BILLS_TOTAL, new CacheItem<decimal>(totalPayable, TimeSpan.FromMinutes(10)));

            return Json(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J14_PAYBILL_REVIEW, false));
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ReviewAccountSelection()
        {
            string[] accountNumbers;
            if (!CacheProvider.TryGet(CacheKeys.SELECTED_BILL_LIST, out accountNumbers))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J14_PAY_BILLS);
            }

            decimal selectionTotal;
            if (!CacheProvider.TryGet(CacheKeys.SELECTED_BILLS_TOTAL, out selectionTotal))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J14_PAY_BILLS);
            }

            ViewBag.AccountNumbers = accountNumbers;
            ViewBag.SelectionTotal = selectionTotal;

            return PartialView("~/Views/Feature/Bills/Bill/_ReviewAccountSelection.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ReviewAccountSelection(BillSelector model)
        {
            LogService.Info(JsonConvert.SerializeObject(model.details));
            if (!model.Selection.Any())
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Please select at least one account"));
                return PartialView("~/Views/Feature/Bills/Bill/_ReviewAccountSelection.cshtml", model);
            }

            if (model.TotalElectedPaymentAmount <= 0.00m)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Payment amount invalid"));
                return PartialView("~/Views/Feature/Bills/Bill/_ReviewAccountSelection.cshtml", model);
            }

            string[] accounts;
            decimal[] amounts;
            ReviewAccountSelected(model, out accounts, out amounts);

            if (model.paymentMethod.Equals(PaymentMethod.PaythroughSamsungPay) || model.paymentMethod.Equals(PaymentMethod.PaythroughApplePay))
            {
                return SamsungPayApplePay(model, amounts);
            }

            #region [MIM Payment Implementation]

            var payRequest = new CipherPaymentModel();
            payRequest.PaymentData.amounts = string.Join(",", amounts);
            payRequest.PaymentData.contractaccounts = string.Join(",", accounts);
            payRequest.ServiceType = ServiceType.PayBill;
            payRequest.PaymentMethod = model.paymentMethod;
            payRequest.IsThirdPartytransaction = false;
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

            return PartialView("~/Views/Feature/Bills/Bill/_ReviewAccountSelection.cshtml", model);
        }

        private ActionResult SamsungPayApplePay(BillSelector model, decimal[] amounts)
        {
            string sdgref = string.Empty;
            string dewatransaction = string.Empty;
            try
            {
                string cardtype = string.Empty;
                string paymentdetails = string.Empty;
                try
                {
                    if (model.paymentMethod.Equals(PaymentMethod.PaythroughApplePay))
                    {
                        dynamic dynamicobj = Newtonsoft.Json.Linq.JObject.Parse(model.details);
                        paymentdetails = JsonConvert.SerializeObject(dynamicobj.token.paymentData);
                        cardtype = (string)dynamicobj.token.paymentMethod.network;
                    }
                    else if (model.paymentMethod.Equals(PaymentMethod.PaythroughSamsungPay))
                    {
                        dynamic dynamicobj = Newtonsoft.Json.Linq.JObject.Parse(model.details);
                        paymentdetails = JsonConvert.SerializeObject(dynamicobj.details.paymentCredential);
                        cardtype = (string)dynamicobj.details.paymentInfo.cardBrand;
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                }
                List<Foundation.Integration.APIHandler.Models.Request.Payment.ContractAccount> contractAccounts = new List<Foundation.Integration.APIHandler.Models.Request.Payment.ContractAccount>();
                model.Accounts.Where(acc => acc.ElectedPaymentAmount > 0.00m).ForEach(x => contractAccounts.Add(new Foundation.Integration.APIHandler.Models.Request.Payment.ContractAccount { accountnumber = x.AccountNumber, amount = string.Format("{0:#,###0.00}", x.ElectedPaymentAmount) }));
                var serviceResponse = PaymentChannelApiClient.PaymentChannel(new SamsungPayRequest
                {
                    walletin = new Walletin
                    {
                        sessionid = CurrentPrincipal.SessionToken,
                        cardtoken = paymentdetails,
                        contract_accounts = contractAccounts,
                        userid = CurrentPrincipal.UserId,
                        totalamount = amounts.Sum(acc => acc).ToString(),
                        servicetype = "BP",
                        cardtype = cardtype
                    }
                }, PaymentContextExtensions.GetPaymentChannelType(model.paymentMethod), RequestLanguage, Request.Segment());
                if (serviceResponse != null && serviceResponse.Payload != null)
                {
                    if (serviceResponse.Succeeded)
                    {
                        string dewatoken = Guid.NewGuid().ToString();
                        CacheProvider.Store(CacheKeys.LastestDewaToken, new CacheItem<string>(dewatoken, TimeSpan.FromDays(1)));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.Payment_Confirmation, new QueryString(false)
                                            .With("s", EPayResponse.Success.ToString())
                                            .With("t", serviceResponse.Payload.Dewatransactionid)
                                            .With("g", serviceResponse.Payload.Sdgreferencenumber)
                                            .With("p", ServiceType.PayBill)
                                            .With("dewatoken", dewatoken)
                                            );
                    }
                    else
                    {
                        sdgref = serviceResponse.Payload.Pgtransactionid;
                        dewatransaction = serviceResponse.Payload.Dewatransactionid;
                    }
                }

            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.Payment_Confirmation, new QueryString(false)
            .With("s", EPayResponse.Failure.ToString())
            .With("g", sdgref)
            .With("t", dewatransaction)
            .With("p", ServiceType.PayBill)
            ); ;
        }

        private void ReviewAccountSelected(BillSelector model, out string[] accounts, out decimal[] amounts)
        {
            accounts = model.Accounts.Where(acc => acc.ElectedPaymentAmount > 0.00m).Select(acc => acc.AccountNumber).ToArray();
            amounts = model.Accounts.Where(acc => acc.ElectedPaymentAmount > 0.00m).Select(m => m.ElectedPaymentAmount).ToArray();
            var businessPartners = model.Accounts.Where(acc => acc.ElectedPaymentAmount > 0.00m).Select(m => m.BusinessPartnerNumber).ToArray();

            CacheProvider.Store(CacheKeys.PAYMENT_METADATA, new CacheItem<PaymentMetaDataModel>(new PaymentMetaDataModel
            {
                BusinessPartnerNumbers = businessPartners,
                ContractAccountNumbers = accounts
            }));

            //var allContractAccounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, true, RequestLanguage, Request.Segment());
            var allContractAccounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, true, RequestLanguage, Request.Segment());

            if (allContractAccounts != null && allContractAccounts.Payload != null)
            {
                var selAccDetails = allContractAccounts.Payload.Where(x => model.Accounts.Any(y => y.AccountNumber == x.AccountNumber));
                List<Account> a = new List<Account>();
                foreach (var account in selAccDetails)
                {
                    a.Add(Account.From(account));
                }
                CacheProvider.Store(CacheKeys.PAYMENT_ACCOUNTS_METADATA, new CacheItem<System.Collections.Generic.IEnumerable<Account>>(a));
            }
        }

        #endregion J14: Pay bills

        #region J15: Pay a friend's bill(s)

        [HttpGet, AllowAnonymous]
        public ActionResult FindFriendsBill()
        {
            CacheProvider.Remove(CacheKeys.FRIENDS_BILL);
            CacheProvider.Remove(CacheKeys.FRIENDS_BILL_SEARCH_CRITERIA);
            CacheProvider.Remove(CacheKeys.PAYMENT_METADATA);
            return PartialView("~/Views/Feature/Bills/Bill/_FindFriendsBill.cshtml");
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult FindFriendsBill(FindBillModel model)
        {
            bool status = false;
            string recaptchaResponse = Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");
            bool _isRecaptchasettingAvailble = ReCaptchaHelper.Recaptchasetting();
            if (_isRecaptchasettingAvailble && !String.IsNullOrEmpty(recaptchaResponse))
            {
                status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
            }
            else if (!_isRecaptchasettingAvailble)
            {
                status = true;
            }
            if (status)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        ///Modified by Shujaat for future center
                        var _fc = FetchFutureCenterValues();
                        var response = DewaApiClient.GetBill(model.SearchCriteria, RequestLanguage, Request.Segment(), _fc.Branch, CurrentPrincipal.SessionToken);

                        if (response.Succeeded)
                        {
                            CacheProvider.Store(CacheKeys.FRIENDS_BILL, new CacheItem<BillEnquiryResponse>(response.Payload));
                            CacheProvider.Store(CacheKeys.FRIENDS_BILL_SEARCH_CRITERIA, new CacheItem<string>(model.SearchCriteria));

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J15_PAY_A_FRIENDS_BILL);
                        }
                        ModelState.AddModelError(string.Empty, response.Message);

                    }
                    catch (System.Exception ex)
                    {
                        LogService.Error(ex, this);
                        ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
            }
            return base.Index();
        }

        [HttpGet, AllowAnonymous]
        public ActionResult FindBill()
        {
            CacheProvider.Remove(CacheKeys.MY_BILL);
            CacheProvider.Remove(CacheKeys.MY_BILL_SEARCH_CRITERIA);
            CacheProvider.Remove(CacheKeys.PAYMENT_METADATA);
            return PartialView("~/Views/Feature/Bills/Bill/_FindBill.cshtml");
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult FindBill(FindBillModel model)
        {
            bool status = false;
            string recaptchaResponse = Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

            bool _isCaptchSettingAvaliable = ReCaptchaHelper.Recaptchasetting();

            if (_isCaptchSettingAvaliable && !String.IsNullOrEmpty(recaptchaResponse))
            {
                status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
            }
            else if (!_isCaptchSettingAvaliable)
            {
                status = true;
            }
            if (status)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        ///Modified by Shujaat for future center
                        var _fc = FetchFutureCenterValues();
                        var response = DewaApiClient.GetBill(model.SearchCriteria, RequestLanguage, Request.Segment(), _fc.Branch, CurrentPrincipal.SessionToken);

                        if (response.Succeeded)
                        {
                            CacheProvider.Store(CacheKeys.MY_BILL, new CacheItem<BillEnquiryResponse>(response.Payload));
                            CacheProvider.Store(CacheKeys.MY_BILL_SEARCH_CRITERIA, new CacheItem<string>(model.SearchCriteria));

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J15_PAY_A_BILL);
                        }
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                    catch (System.Exception)
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
            }
            return base.Index();
        }

        [HttpGet, AllowAnonymous]
        public ActionResult ViewPayFriendsBill()
        {
            BillEnquiryResponse bill;
            if (!CacheProvider.TryGet(CacheKeys.FRIENDS_BILL, out bill))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J15_PAYFRIENDBILL_START);
            }

            string searchCriteria;
            if (!CacheProvider.TryGet(CacheKeys.FRIENDS_BILL_SEARCH_CRITERIA, out searchCriteria))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J15_PAYFRIENDBILL_START);
            }

            string paymentError;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out paymentError))
            {
                ModelState.AddModelError(string.Empty, paymentError);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }

            var model = ViewFriendsBillsModel.From(searchCriteria, bill);

            ViewBag.LoginRedirect = new QueryString(false)
                .With("returnUrl", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J15_PAY_A_FRIENDS_BILL))
                .CombineWith(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE));

            if (model.MultipleBillsFound)
            {
                return PartialView("~/Views/Feature/Bills/Bill/_ViewPayFriendsBills.cshtml", model);
            }
            return PartialView("~/Views/Feature/Bills/Bill/_ViewPayFriendsBill.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ViewPayFriendsBill(ViewFriendsBillsModel model)
        {
            if (!model.Bills.Any())
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Please select at least one account"));
            }

            if (model.TotalElectedPayments < 0.01m)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Payment amount invalid"));
            }

            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Feature/Bills/Bill/_ViewPayFriendsBill.cshtml", model);
            }

            var accounts = model.Bills.Select(acc => acc.AccountNumber).ToArray();
            var businessPartners = model.Bills.Select(acc => acc.BusinessPartnerNumber).ToArray();
            var amounts = model.Bills.Select(m => m.ElectedPaymentAmount).ToArray();

            CacheProvider.Store(CacheKeys.PAYMENT_METADATA, new CacheItem<PaymentMetaDataModel>(new PaymentMetaDataModel
            {
                BusinessPartnerNumbers = businessPartners,
                ContractAccountNumbers = accounts
            }));

            CacheProvider.Store(CacheKeys.PAYMENT_ACCOUNTS_METADATA, new CacheItem<System.Collections.Generic.IEnumerable<Account>>(model.Bills));
            //return this.RedirectToAction("Redirect", "Payment", new BillPaymentRequestModel
            //{
            //    Amounts = string.Join(",", amounts),
            //    ContractAccounts = string.Join(",", accounts),
            //    ThirdPartyPayment = true
            //});

            #region [MIM Payment Implementation]

            var payRequest = new CipherPaymentModel();
            payRequest.PaymentData.amounts = string.Join(",", amounts);
            payRequest.PaymentData.contractaccounts = string.Join(",", accounts);
            payRequest.ServiceType = ServiceType.PayBill;
            payRequest.PaymentMethod = model.paymentMethod;
            payRequest.IsThirdPartytransaction = false;
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

            return PartialView("~/Views/Feature/Bills/Bill/_ViewPayFriendsBill.cshtml", model);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult ViewPayBill()
        {
            BillEnquiryResponse bill;
            if (!CacheProvider.TryGet(CacheKeys.MY_BILL, out bill))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J15_BILL_ENQUIRY);
            }

            string searchCriteria;
            if (!CacheProvider.TryGet(CacheKeys.MY_BILL_SEARCH_CRITERIA, out searchCriteria))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J15_BILL_ENQUIRY);
            }

            string paymentError;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out paymentError))
            {
                ModelState.AddModelError(string.Empty, paymentError);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }

            var model = ViewFriendsBillsModel.From(searchCriteria, bill);

            ViewBag.LoginRedirect = new QueryString(false)
                .With("returnUrl", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J15_PAY_A_BILL))
                .CombineWith(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE));

            if (model.MultipleBillsFound)
            {
                return PartialView("~/Views/Feature/Bills/Bill/_ViewPayMyBills.cshtml", model);
            }
            return PartialView("~/Views/Feature/Bills/Bill/_ViewPayMyBill.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ViewPayBill(ViewFriendsBillsModel model)
        {
            if (!model.Bills.Any())
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Please select at least one account"));
            }

            if (model.TotalElectedPayments < 0.01m)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Payment amount invalid"));
            }

            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Feature/Bills/Bill/_ViewPayMyBill.cshtml", model);
            }

            var accounts = model.Bills.Select(acc => acc.AccountNumber).ToArray();
            var businessPartners = model.Bills.Select(acc => acc.BusinessPartnerNumber).ToArray();
            var amounts = model.Bills.Select(m => m.ElectedPaymentAmount).ToArray();

            CacheProvider.Store(CacheKeys.PAYMENT_METADATA, new CacheItem<PaymentMetaDataModel>(new PaymentMetaDataModel
            {
                BusinessPartnerNumbers = businessPartners,
                ContractAccountNumbers = accounts
            }));

            //return this.RedirectToAction("Redirect", "Payment", new BillPaymentRequestModel
            //{
            //    Amounts = string.Join(",", amounts),
            //    ContractAccounts = string.Join(",", accounts),
            //    ThirdPartyPayment = true
            //});

            #region [MIM Payment Implementation]

            var payRequest = new CipherPaymentModel();
            payRequest.PaymentData.amounts = string.Join(",", amounts);
            payRequest.PaymentData.contractaccounts = string.Join(",", accounts);
            payRequest.ServiceType = ServiceType.PayBill;
            payRequest.PaymentMethod = model.paymentMethod;
            payRequest.IsThirdPartytransaction = false;
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

            return PartialView("~/Views/Feature/Bills/Bill/_ViewPayMyBill.cshtml", model);
        }

        #endregion J15: Pay a friend's bill(s)

        #region J11: Request collective account

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult RequestCollectiveAccount()
        {
            CacheProvider.Remove(CacheKeys.PAYMENT_METADATA);

            var model = new RequestCollectiveAccount()
            {
                Categories = GetDictionaryListByKey("Categories").ToDictionary(x => x.Name, x => x.Fields["Phrase"].Value)
            };
            return PartialView("~/Views/Feature/Bills/Bill/_RequestCollectiveAccount.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize]
        public ActionResult RequestCollectiveAccount(RequestCollectiveAccount model)
        {
            string error;
            if (!AttachmentIsValid(model.OfficialLetterUploader, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            //if (!AttachmentIsValid(model.TradeLicenceUploader, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
            //{
            //    ModelState.AddModelError(string.Empty, error);
            //}

            if (string.IsNullOrEmpty(model.BusinessPartnerNumber))
            {
                ModelState.AddModelError(string.Empty, Translate.Text("please select business partner validation message"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var @params = new RequestCollectiveAccountParameters
                    {
                        BusinessPartnerNumber = model.BusinessPartnerNumber,
                        Name = model.ContactName,
                        Email = model.EmailAddress,
                        Mobile = model.Mobile.AddMobileNumberZeroPrefix(),
                        Category = model.SelectedCategoryValue,
                        Attachment1 = model.OfficialLetterUploader.ToArray(),
                        FileType1 = model.OfficialLetterUploader.GetTrimmedFileExtension(),
                        Filename1 = model.OfficialLetterUploader.FileName.GetFileNameWithoutPath(),

                        UserId = CurrentPrincipal.UserId,
                        SessionId = CurrentPrincipal.SessionToken
                    };

                    if (model.TradeLicenceUploader != null)
                    {
                        @params.Attachment2 = model.TradeLicenceUploader.ToArray();
                        @params.FileType2 = model.TradeLicenceUploader.GetTrimmedFileExtension();
                        @params.Filename2 = model.TradeLicenceUploader.FileName.GetFileNameWithoutPath();
                    }

                    var response = DewaApiClient.RequestCollectiveAccount(@params, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        var sessionModel = new RequestCollectiveAccountWorkflowState
                        {
                            BusinessPartnerNumber = model.BusinessPartnerNumber,
                            ContactName = model.ContactName,
                            EmailAddress = model.EmailAddress,
                            Mobile = model.Mobile.AddMobileNumberZeroPrefix(),
                            SelectedCategoryValue = model.SelectedCategoryValue,
                            OfficialLetterUploader = @params.Attachment1,
                            OfficialLetterFileName = model.OfficialLetterUploader.FileName.GetFileNameWithoutPath(),

                            NotificationNumber = response.Payload.NotificationNumber.TrimStart('0')
                        };

                        if (@params.Attachment2 != null)
                        {
                            sessionModel.TradeLicenceUploader = @params.Attachment2;
                            sessionModel.TradeLicenceFileName = model.TradeLicenceUploader.FileName.GetFileNameWithoutPath();
                        }

                        CacheProvider.Store(CacheKeys.REQUEST_COLLECTIVE_ACCOUNT_STATE, new CacheItem<RequestCollectiveAccountWorkflowState>(sessionModel));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J11_REQUEST_COLLECTIVE_ACCOUNT_SUCCESS);
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            model.Categories = GetDictionaryListByKey("Categories").ToDictionary(x => x.Name, x => x.Fields["Phrase"].Value);

            return PartialView("~/Views/Feature/Bills/Bill/_RequestCollectiveAccount.cshtml", model);
        }

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult RequestCollectiveAccountSuccess()
        {
            RequestCollectiveAccountWorkflowState state;
            if (!CacheProvider.TryGet(CacheKeys.REQUEST_COLLECTIVE_ACCOUNT_STATE, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J11_REQUEST_COLLECTIVE_ACCOUNT);
            }
            return PartialView("~/Views/Feature/Bills/Bill/_RequestCollectiveAccountSuccess.cshtml", state);
        }

        #endregion J11: Request collective account

        #region J12: Add to collective billing

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult AddToCollectiveBilling()
        {
            CacheProvider.Remove(CacheKeys.ADD_TO_COLLECTIVE_BILLING_STATE);

            var model = new AddToCollectiveBilling();

            //var accounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
            var accounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

            if (accounts.Payload != null)
            {
                foreach (var account in accounts.Payload)
                {
                    if (!model.BusinessPartners.ContainsKey(account.BusinessPartnerNumber))
                    {
                        model.BusinessPartners.Add(account.BusinessPartnerNumber, string.Format("{0} ({1})", Translate.Text("business partner label"), account.BusinessPartnerNumber));
                    }
                }
            }
            return PartialView("~/Views/Feature/Bills/Bill/_AddToCollectiveBilling.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize]
        public ActionResult AddToCollectiveBilling(AddToCollectiveBilling model)
        {
            string error;
            if (!AttachmentIsValid(model.OfficialLetterUploader, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            //var accounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
            var accounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

            if (accounts.Payload != null)
            {
                foreach (var account in accounts.Payload)
                {
                    if (!model.BusinessPartners.ContainsKey(account.BusinessPartnerNumber))
                    {
                        model.BusinessPartners.Add(account.BusinessPartnerNumber, string.Format("{0} ({1})", Translate.Text("business partner label"), account.BusinessPartnerNumber));
                    }
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var @params = new RequestCollectiveAccountParameters
                    {
                        Name = model.ContactName,
                        Email = model.EmailAddress,
                        Mobile = model.Mobile.AddMobileNumberZeroPrefix(),
                        BusinessPartnerNumber = model.SelectedBusinessPartnerKey,
                        Attachment1 = model.OfficialLetterUploader.ToArray(),
                        FileType1 = model.OfficialLetterUploader.GetTrimmedFileExtension(),
                        Filename1 = model.OfficialLetterUploader.FileName.GetFileNameWithoutPath(),
                        UserId = CurrentPrincipal.UserId,
                        SessionId = CurrentPrincipal.SessionToken
                    };

                    var response = DewaApiClient.AddToCollectiveBilling(@params, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        var sessionModel = new RequestCollectiveAccountWorkflowState
                        {
                            ContactName = model.ContactName,
                            EmailAddress = model.EmailAddress,
                            Mobile = model.Mobile.AddMobileNumberZeroPrefix(),
                            BusinessPartnerNumber = model.SelectedBusinessPartnerKey,
                            OfficialLetterUploader = @params.Attachment1,
                            OfficialLetterFileName = model.OfficialLetterUploader.FileName.GetFileNameWithoutPath(),
                            NotificationNumber = response.Payload.NotificationNumber.TrimStart('0')
                        };

                        CacheProvider.Store(CacheKeys.ADD_TO_COLLECTIVE_BILLING_STATE, new CacheItem<RequestCollectiveAccountWorkflowState>(sessionModel));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J12_ADD_TO_COLLECTIVE_BILLING_SUCCESS);
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            return PartialView("~/Views/Feature/Bills/Bill/_AddToCollectiveBilling.cshtml", model);
        }

        [HttpGet]
        [TwoPhaseAuthorize]
        public ActionResult AddToCollectiveBillingSuccess()
        {
            RequestCollectiveAccountWorkflowState state;
            if (CacheProvider.TryGet(CacheKeys.ADD_TO_COLLECTIVE_BILLING_STATE, out state))
            {
                return PartialView("~/Views/Feature/Bills/Bill/_AddToCollectiveBillingSuccess.cshtml", state);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J12_ADD_TO_COLLECTIVE_BILLING);
        }

        #endregion J12: Add to collective billing

        #region Anonymous Bill download

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult AnonymousBilldownload(string q)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                return View("~/Views/Feature/Bills/Bill/AnonymousBillDownload.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AnonymousBilldownload(string q, string a)
        {
            bool status = false;
            bool _isCaptchSettingAvaliable = ReCaptchaHelper.Recaptchasetting();
            if (!string.IsNullOrWhiteSpace(q))
            {
                string recaptchaResponse = Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

                if (_isCaptchSettingAvaliable && !String.IsNullOrEmpty(recaptchaResponse))
                {
                    status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
                }
                else if (!_isCaptchSettingAvaliable)
                {
                    status = true;
                }
                if (status)
                {
                    var response = DewaApiClient.GetBillDetailsPDF(q, RequestLanguage, Request.Segment());

                    if (response.Succeeded && response.Payload.billData != null && response.Payload.billData.Length > 0)
                    {
                        CacheProvider.Store(CacheKeys.anonymous_bill_download, new AccessCountingCacheItem<byte[]>(response.Payload.billData, Times.Once));
                        return RedirectToAction("ActualbillDownload", "Bill");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
                return View("~/Views/Feature/Bills/Bill/AnonymousBillDownload.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        [HttpGet]
        public ActionResult ActualbillDownload()
        {
            byte[] bytes;
            if (CacheProvider.TryGet(CacheKeys.anonymous_bill_download, out bytes))
            {
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ERROR_404);
        }

        #endregion Anonymous Bill download

        #region [API]

        [HttpPost, ValidateAntiForgeryToken, TwoPhaseAuthorize]
        public ActionResult BillDownloads(string id, _ApiBillDownloaders.BillIdentifierType type = _ApiBillDownloaders.BillIdentifierType.InvoiceNumber)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var prefix = type == _ApiBillDownloaders.BillIdentifierType.InvoiceNumber ? "IN" : "CA";
                var formattedIdentifier = !id.StartsWith(prefix) ? string.Concat(prefix, id) : id;
                var response = DewaApiClient.GetBill(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, formattedIdentifier, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    if (response.Payload.Length > 0)
                    {
                        return File(response.Payload, "application/pdf", getClearstr(string.Format("{0}.pdf", formattedIdentifier)));
                    }
                }
            }

            return Redirect(GetSitecoreUrlByID(SitecoreItemIdentifiers.Bill_Download_Message));
        }

        #endregion [API]
    }
}