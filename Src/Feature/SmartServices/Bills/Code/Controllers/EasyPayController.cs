using DEWAXP.Feature.Bills.Models.EasyPay;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DEWAXP.Feature.Bills.Controllers
{
    public class EasyPayController : BaseController
    {
        // GET: EasyPay
        [AcceptVerbs("GET", "HEAD")]
        [AllowAnonymous]
        public ActionResult FindEasyPay()
        {
            ViewBag.IsLoggedIn = IsLoggedIn;
            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            //string ca = ;

            EasyPayModel model = new EasyPayModel() { EasyPayNumber = string.IsNullOrWhiteSpace(Request.QueryString["ac"]) ? "" : Request.QueryString["ac"] };
            ViewBag.BeneficaryJson = GetBeneficaryJson();
            return View("~/Views/Feature/Bills/EasyPay/FindEasyPay.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FindEasyPay(EasyPayModel model)
        {
            try
            {
                ViewBag.IsLoggedIn = IsLoggedIn;
                // Future Center
                var _fc = FetchFutureCenterValues();
                var response = DewaApiClient.GetEasyPayEnquiry(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.EasyPayNumber, RequestLanguage, Request.Segment(), _fc.Branch);

                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    GetEasyPayEnquiryResponse _response;
                    _response = response.Payload;
                    CacheProvider.Store(CacheKeys.Easy_Pay_Response, new CacheItem<GetEasyPayEnquiryResponse>(_response));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EasyPay_Details);
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            ViewBag.BeneficaryJson = GetBeneficaryJson();
            return View("~/Views/Feature/Bills/EasyPay/FindEasyPay.cshtml");
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult EasyPayDetails()
        {
            try
            {
                GetEasyPayEnquiryResponse _easy;
                if (!CacheProvider.TryGet(CacheKeys.Easy_Pay_Response, out _easy))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EasyPay_Enquire);
                }
                string error;
                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                var _fc = FetchFutureCenterValues();
                var response = DewaApiClient.GetEasyPayEnquiry(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, _easy.@return.easypaynumber, RequestLanguage, Request.Segment(), _fc.Branch);

                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    GetEasyPayEnquiryResponse _response;
                    _response = response.Payload;
                    EasyPayModel model = new EasyPayModel
                    {
                        EasyPayNumber = _response.@return.easypaynumber,
                        Name = _response.@return.name,
                        ServiceType = _response.@return.transactiondescription,
                        Email = _response.@return.maskedemail,
                        Mobile = _response.@return.maskedmobile,
                        TotalAmount = Convert.ToDecimal(_response.@return.amount),
                        PartialPayFlage = _response.@return.partialpayflag,
                        Transactiontype = _response.@return.transactiontype
                    };
                    CacheProvider.Store(CacheKeys.Easy_Pay_LoggedIn, new CacheItem<bool>(IsLoggedIn));
                    CacheProvider.Store(CacheKeys.Easy_Pay_Response, new CacheItem<GetEasyPayEnquiryResponse>(_response));
                    ViewBag.LoginRedirect = new QueryString(false)
               .With("returnUrl", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EasyPay_Details))
               .CombineWith(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE));
                    ViewBag.IsLoggedIn = IsLoggedIn;
                    return View("~/Views/Feature/Bills/EasyPay/EasyPayDetails.cshtml", model);
                }
            }
            catch (System.Exception ex)
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(ex.ToString(), Times.Once));
                //ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EasyPay_Enquire);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EasyPayDetails(EasyPayModel model)
        {
            GetEasyPayEnquiryResponse _easy;
            if (!CacheProvider.TryGet(CacheKeys.Easy_Pay_Response, out _easy))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EasyPay_Enquire);
            }
            try
            {
                //validating partail payment
                if (_easy.@return.partialpayflag != "X" && IsLoggedIn &&
                    model.TotalAmount != Convert.ToDecimal(_easy.@return.amount))
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Invalid Payment"));
                }

                if (ModelState.IsValid)
                {
                    var fullUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PaymentRedirect);//this.Url.Action("Redirect", "Payment");
                    #region [MIM Payment Implementation]
                    if (!string.IsNullOrWhiteSpace(_easy.@return.transactiontype))
                    {
                        var payRequest = new CipherPaymentModel();
                        #region [binding model on service type]
                        if ((_easy.@return.transactiontype == "ESTMNM" || _easy.@return.transactiontype == "KWP2"))
                        {
                            #region [Model binding]
                            CacheProvider.Store(CacheKeys.Easy_Pay_Estimate, new CacheItem<decimal>(model.TotalAmount));

                            payRequest.PaymentData.contractaccounts = _easy.@return.contractaccountnumber?.Trim();
                            payRequest.PaymentData.amounts = model.TotalAmount.ToString();
                            payRequest.PaymentData.estimatenumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.businesspartner = _easy.@return.businesspartnernumber;
                            payRequest.PaymentData.ownerbusinesspartnernumber = _easy.@return.ownerbp;
                            payRequest.PaymentData.consultantbusinesspartnernumber = _easy.@return.consultantbp;
                            payRequest.PaymentData.email = _easy.@return.email;
                            payRequest.IsThirdPartytransaction = true;
                            //payRequest.paymentData.EPayUrl = fullUrl,
                            payRequest.ServiceType = ServiceType.EstimatePayment;//"EstimatePayment",
                            payRequest.PaymentData.easypaynumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.easypayflag = "X";

                            if (!string.IsNullOrEmpty(model.paymentMethod.ToString()) && _easy.@return.transactiontype == "ESTMNM")
                            {
                                payRequest.PaymentMethod = model.paymentMethod;
                            }
                            #endregion [Model binding]
                        }
                        else if (_easy.@return.transactiontype == "SDPAY")
                        {
                            #region [Model Binding]
                            payRequest.PaymentData.contractaccounts = _easy.@return.contractaccountnumber?.Trim();
                            payRequest.PaymentData.amounts = Convert.ToString(model.TotalAmount);
                            payRequest.PaymentData.businesspartner = _easy.@return.businesspartnernumber?.Trim();
                            payRequest.PaymentData.email = _easy.@return.email;
                            payRequest.PaymentData.userid = CurrentPrincipal.UserId;
                            payRequest.PaymentData.mobile = _easy.@return.mobile;
                            payRequest.IsThirdPartytransaction = true;
                            payRequest.ServiceType = ServiceType.ServiceActivation;//"ServiceActivation",
                            payRequest.PaymentData.easypaynumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.easypayflag = "X";
                            payRequest.PaymentMethod = model.paymentMethod;
                            #endregion [Model Binding]
                        }
                        else if (_easy.@return.transactiontype == "EVPAY")
                        {
                            #region [Model Binding]
                            payRequest.PaymentData.contractaccounts = _easy.@return.contractaccountnumber?.Trim();
                            payRequest.PaymentData.amounts = Convert.ToString(model.TotalAmount);
                            payRequest.PaymentData.movetoflag = "V";
                            payRequest.IsThirdPartytransaction = true;
                            payRequest.ServiceType = ServiceType.PayBill; //"PayBill",
                            payRequest.PaymentData.easypaynumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.easypayflag = "X";
                            payRequest.PaymentMethod = model.paymentMethod;
                            #endregion [Model Binding]
                        }
                        else if (_easy.@return.transactiontype == "REFBILL")
                        {
                            #region [Model Binding]
                            payRequest.PaymentData.contractaccounts = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.amounts = Convert.ToString(model.TotalAmount);
                            payRequest.PaymentData.movetoflag = "R";
                            payRequest.IsThirdPartytransaction = false;
                            payRequest.ServiceType = ServiceType.PayBill; //"PayBill",
                            payRequest.PaymentData.easypaynumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.easypayflag = "X";
                            payRequest.PaymentMethod = model.paymentMethod;
                            #endregion [Model Binding]
                        }
                        else if (_easy.@return.transactiontype == "MSLPAY")
                        {
                            #region [Model Binding]
                            CacheProvider.Store(CacheKeys.Easy_Pay_Estimate, new CacheItem<decimal>(model.TotalAmount));

                            payRequest.PaymentData.contractaccounts = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.amounts = Convert.ToString(model.TotalAmount);
                            payRequest.PaymentData.businesspartner = _easy.@return.businesspartnernumber;
                            payRequest.PaymentData.email = _easy.@return.email;
                            payRequest.PaymentData.mobile = _easy.@return.mobile;
                            payRequest.IsThirdPartytransaction = false;
                            payRequest.ServiceType = ServiceType.Miscellaneous; //"Miscellaneous",
                            payRequest.PaymentData.easypaynumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.notificationnumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.easypayflag = "X";
                            payRequest.PaymentMethod = model.paymentMethod;
                            #endregion [Model Binding]
                        }
                        else
                        {
                            #region [Model Binding]
                            payRequest.PaymentData.contractaccounts = _easy.@return.contractaccountnumber?.Trim();
                            payRequest.PaymentData.amounts = Convert.ToString(model.TotalAmount);
                            payRequest.IsThirdPartytransaction = true;
                            payRequest.ServiceType = ServiceType.PayBill; //"PayBill",
                            payRequest.PaymentData.easypaynumber = _easy.@return.easypaynumber?.Trim();
                            payRequest.PaymentData.easypayflag = "X";
                            payRequest.PaymentMethod = model.paymentMethod;
                            #endregion [Model Binding]
                        }

                        #endregion [binding model on service type]
                        payRequest.BankKey = model.bankkey;
                        payRequest.SuqiaValue = model.SuqiaDonation;
                        payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                        var payResponse = ExecutePaymentGateway(payRequest);
                        if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                        {
                            return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                        }
                        ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                    }
                    #endregion [MIM Payment Implementation]
                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            if (_easy != null && _easy.@return != null)
            {
                model.EasyPayNumber = _easy.@return.easypaynumber;
                model.Name = _easy.@return.name;
                model.ServiceType = _easy.@return.transactiondescription;
                model.Email = _easy.@return.maskedemail;
                model.Mobile = _easy.@return.maskedmobile;
                model.TotalAmount = Convert.ToDecimal(_easy.@return.amount);
                model.PartialPayFlage = _easy.@return.partialpayflag;

                CacheProvider.Store(CacheKeys.Easy_Pay_LoggedIn, new CacheItem<bool>(IsLoggedIn));
                CacheProvider.Store(CacheKeys.Easy_Pay_Response, new CacheItem<GetEasyPayEnquiryResponse>(_easy));
                ViewBag.LoginRedirect = new QueryString(false)
           .With("returnUrl", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EasyPay_Details))
           .CombineWith(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE));
                ViewBag.IsLoggedIn = IsLoggedIn;
                return View("~/Views/Feature/Bills/EasyPay/EasyPayDetails.cshtml", model);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EasyPay_Enquire);
        }

        #region [Manage Beneficiary]

        [TwoPhaseAuthorize, HttpGet]
        public ActionResult ManageBeneficiariesV1(string ac = "")
        {
            ManageBeneficiaryDetails model = new ManageBeneficiaryDetails();
            try
            {
                string error;
                string BeneficiaryName;
                ManageBeneficiaryDetails cacheModel = new ManageBeneficiaryDetails();
                model.BeneficiaryDetailLists = new List<BeneficiaryDetail>();

                if (CacheProvider.TryGet(CacheKeys.ManageBeneficiary_Error, out error))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                if (CacheProvider.TryGet(CacheKeys.ManageBeneficiary_Response, out cacheModel))
                {
                    model.ContractAccount = cacheModel.ContractAccount;
                    model.Name = cacheModel.Name;
                }
                if (CacheProvider.TryGet(CacheKeys.ManageBeneficiary_Success, out BeneficiaryName))
                {
                    ViewBag.BeneficiaryName = BeneficiaryName;
                }
                var response = WebManageBeneficiary(null, BeneficiaryManagemode.G);
                if (response.Succeeded &&
                    response.Payload.beneficiaryList != null &&
                    response.Payload.beneficiaryList.Length > 0)
                {
                    foreach (var item in response.Payload.beneficiaryList)
                    {
                        model.BeneficiaryDetailLists.Add(new BeneficiaryDetail()
                        {
                            ContractAccount = item.contractAccount,
                            Name = item.name,
                        });
                    }
                }
                if (!string.IsNullOrWhiteSpace(ac))
                    model.ContractAccount = ac;

                CacheProvider.Remove(CacheKeys.ManageBeneficiary_Response);
                CacheProvider.Remove(CacheKeys.ManageBeneficiary_Error);
                CacheProvider.Remove(CacheKeys.ManageBeneficiary_Success);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return PartialView("~/Views/Feature/Bills/EasyPay/_Beneficiary/v1/ManageBeneficiaries.cshtml", model);
        }

        [TwoPhaseAuthorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageBeneficiariesV1(ManageBeneficiaryDetails model)
        {
            try
            {
                model.ContractAccount = model.ContractAccount?.Trim();
                model.Name = model.Name?.Trim();
                BeneficiaryDetail detail = new BeneficiaryDetail();
                detail.ContractAccount = model.ContractAccount;
                detail.Name = model.Name;
                var response = WebManageBeneficiary(detail, BeneficiaryManagemode.A);
                if (response.Succeeded)
                {
                    ViewBag.BeneficiaryName = model.ContractAccount;
                    CacheProvider.Store(CacheKeys.ManageBeneficiary_Success, new CacheItem<string>(model.ContractAccount));
                    model = new ManageBeneficiaryDetails();
                }
                else if (!response.Succeeded && response.Payload != null)
                {
                    ModelState.AddModelError(string.Empty, response.Payload.description);
                    CacheProvider.Store(CacheKeys.ManageBeneficiary_Error, new CacheItem<string>(response.Payload.description));
                    CacheProvider.Store(CacheKeys.ManageBeneficiary_Response, new CacheItem<ManageBeneficiaryDetails>(model));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.ADD_BENEFICIARY);
            // return PartialView("~/Views/Feature/Bills/EasyPay/_Beneficiary/Add.cshtml", model);
        }

        [TwoPhaseAuthorize, HttpGet]
        public ActionResult AddBeneficiary(string ac = "")
        {
            return PartialView("~/Views/Feature/Bills/EasyPay/_Beneficiary/Add.cshtml", new BeneficiaryDetail()
            {
                ContractAccount = ac,
            });
        }

        [TwoPhaseAuthorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddBeneficiary(BeneficiaryDetail model)
        {
            try
            {
                model.ContractAccount = model.ContractAccount?.Trim();
                model.Name = model.Name?.Trim();

                var response = WebManageBeneficiary(model, BeneficiaryManagemode.A);
                if (response.Succeeded)
                {
                    ViewBag.BeneficiaryName = model.ContractAccount;
                    model = new BeneficiaryDetail();
                }
                else if (!response.Succeeded && response.Payload != null)
                {
                    ModelState.AddModelError(string.Empty, response.Payload.description);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/Bills/EasyPay/_Beneficiary/Add.cshtml", model);
        }

        [TwoPhaseAuthorize, HttpGet]
        public ActionResult EditBeneficiary()
        {
            BeneficiaryDetailList model = new BeneficiaryDetailList();
            try
            {
                model.BeneficiaryDetails = new List<BeneficiaryDetail>();
                var response = WebManageBeneficiary(null, BeneficiaryManagemode.G);
                if (response.Succeeded &&
                    response.Payload.beneficiaryList != null &&
                    response.Payload.beneficiaryList.Length > 0)
                {
                    foreach (var item in response.Payload.beneficiaryList)
                    {
                        model.BeneficiaryDetails.Add(new BeneficiaryDetail()
                        {
                            ContractAccount = item.contractAccount,
                            Name = item.name,
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView("~/Views/Feature/Bills/EasyPay/_Beneficiary/Edit.cshtml", model);
        }

        [TwoPhaseAuthorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateBeneficiary(BeneficiaryDetail model)
        {
            try
            {
                var response = WebManageBeneficiary(model, BeneficiaryManagemode.E);
                return Json(new { success = response.Succeeded, description = response.Payload.description }, JsonRequestBehavior.DenyGet);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return Json(new { success = false, description = Translate.Text("Unexpected error") }, JsonRequestBehavior.DenyGet);
        }

        [TwoPhaseAuthorize, HttpDelete, ValidateAntiForgeryToken]
        public ActionResult DeleteBeneficiary(BeneficiaryDetail model)
        {
            try
            {
                var response = WebManageBeneficiary(model, BeneficiaryManagemode.D);
                return Json(new { success = response.Succeeded, description = response.Payload.description }, JsonRequestBehavior.DenyGet);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return Json(new { success = false, description = Translate.Text("Unexpected error") }, JsonRequestBehavior.DenyGet);
        }

        #region[function]

        private DEWAXP.Foundation.Integration.Responses.ServiceResponse<manageBeneficiaryResponse> WebManageBeneficiary(BeneficiaryDetail data, BeneficiaryManagemode beneficiaryManagemode = BeneficiaryManagemode.G)
        {
            try
            {
                List<DEWAXP.Foundation.Integration.DewaSvc.beneficiary> beneficiaries = new List<DEWAXP.Foundation.Integration.DewaSvc.beneficiary>();
                beneficiaries.Add(new DEWAXP.Foundation.Integration.DewaSvc.beneficiary()
                {
                    contractAccount = data?.ContractAccount ?? "",
                    name = data?.Name ?? ""
                });
                var input = new DEWAXP.Foundation.Integration.DewaSvc.ManageBeneficiary()
                {
                    beneficiarylist = beneficiaries.ToArray(),
                    managemode = beneficiaryManagemode.ToString(),
                    sessionid = CurrentPrincipal.SessionToken,
                };

                var result = DewaApiClient.ManageBeneficiary(input, RequestLanguage, Request.Segment());
                return result;
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return null;
        }

        private string GetBeneficaryJson()
        {
            if (IsLoggedIn)
            {
                var response = WebManageBeneficiary(null, BeneficiaryManagemode.G);
                if (response.Succeeded && response.Payload?.beneficiaryList != null)
                {
                    List<object> dataList = new List<object>();
                    foreach (var item in response.Payload?.beneficiaryList)
                    {
                        dataList.Add(new
                        {
                            key = item.name,
                            number = item.contractAccount,
                        });
                    }

                    return new JavaScriptSerializer().Serialize(dataList);
                }
            }
            return "[]";
        }

        #endregion [Manage Beneficiary]

        #endregion
    }
}