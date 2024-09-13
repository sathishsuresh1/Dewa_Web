using DEWAXP.Feature.Bills.ClearanceCertificate;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.ClearanceCertificate;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    public class ClearanceCertificateController : BaseController
    {
        

        /// <summary>
        /// Clearance Certificate v2
        /// Added by Syed Shujaat Ali
        /// </summary>
        /// <returns></returns>

        #region Clearance Certificate v2

        [HttpGet, TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult RequestDEWACustomerv2()
        {
            string errorMessage, contractAccountNumber;

            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            if (CacheProvider.TryGet(CacheKeys.CLEARANCE_CERTIFICATE_FinalFlow, out contractAccountNumber) && !string.IsNullOrEmpty(contractAccountNumber))
            {
                ViewBag.ContractAccountNumber = contractAccountNumber;
            }

            AuthenticatedClearanceCertificateModel model = new AuthenticatedClearanceCertificateModel();
            return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestDewaCustomer.cshtml", model);
        }

        [HttpPost, TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), ValidateAntiForgeryToken]
        public ActionResult RequestDEWACustomerv2(AuthenticatedClearanceCertificateModel model)
        {
            var _fc = FetchFutureCenterValues();

            if (model.Amount <= 0)
            {
                var response = DewaApiClient.ApplyClearanceCertificateDEWACustomer(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, _fc.Branch, model.ContractAccountNumber, model.Remarks, model.EmailAddress, model.FirstName, model.LastName, model.MobileNumber.AddMobileNumberZeroPrefix(), RequestLanguage, Request.Segment(), model.Languague);

                if (response.Succeeded)
                {
                    model.NotificationNumber = response.Payload.@return.notificationNumber;

                    CacheProvider.Remove(CacheKeys.CLEARANCE_CERTIFICATE_FinalFlow);

                    var context = ContentRepository.GetItem<AccountSelector>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{B5DCB7F5-E800-41EE-8423-3F95BF081929}")));
                    if (!string.IsNullOrEmpty(context.NotificationCode))
                    {
                        CacheProvider.Remove(context.NotificationCode);
                    }

                    return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestDewaCustomerSuccess.cshtml", model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            else
            {
                var paymentModel = new ClearanceCertificatePaymentModel
                {
                    ContractAccountNumber = model.ContractAccountNumber,
                    BusinessPartnerNumber = model.BusinessPartnerNumber,
                    Amount = Convert.ToDecimal(model.Amount),
                    //TransactionNumber = response.Payload.@return.notificationNumber
                };

                CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_DETAILS, new CacheItem<ClearanceCertificatePaymentModel>(paymentModel));
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("ClearanceCertificatePayment", SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_DEWACUSTOMER);

                CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_PATH, new CacheItem<Dictionary<string, string>>(dictionary));

                #region [MIM Payment Implementation]

                var payRequest = new CipherPaymentModel();
                payRequest.PaymentData.amounts = Convert.ToString(model.Amount);
                payRequest.PaymentData.contractaccounts = model.ContractAccountNumber;
                payRequest.PaymentData.businesspartner = model.BusinessPartnerNumber;
                payRequest.PaymentData.email = model.EmailAddress;
                payRequest.PaymentData.mobile = model.MobileNumber?.AddMobileNumberZeroPrefix();
                payRequest.PaymentData.clearancetransaction = "";
                payRequest.ServiceType = ServiceType.PayBill;
                payRequest.PaymentMethod = model.paymentMethod;
                payRequest.PaymentData.userid = CurrentPrincipal.UserId;
                payRequest.IsThirdPartytransaction = false;
                payRequest.BankKey = model.bankkey;
                payRequest.SuqiaValue = model.SuqiaDonation;
                payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                var payResponse = ExecutePaymentGateway(payRequest);
                if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                {
                    CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_MODEL, new CacheItem<CipherPaymentModel>(payRequest));
                    return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                }
                ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));

                #endregion [MIM Payment Implementation]
            }

            CacheProvider.Remove(CacheKeys.CLEARANCE_CERTIFICATE_FinalFlow);
            return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestDewaCustomer.cshtml", model);
        }

        /// <summary>
        /// Verify Clearance Certificate search, and display the clearance certificate data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VerifyCertificate(string r, string p)
        {
            string _refNo = string.Empty;
            string _pin = string.Empty;
            if (!string.IsNullOrEmpty(r))
                _refNo = LinkHelper.Base64Decode(r);
            if (!string.IsNullOrEmpty(p))
                _pin = LinkHelper.Base64Decode(p);

            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            VerifyCertificateModel _verifyModel = new VerifyCertificateModel();
            if (!string.IsNullOrWhiteSpace(_refNo) && !string.IsNullOrWhiteSpace(_pin))
            {
                _verifyModel.ReferenceNumber = _refNo;
                _verifyModel.PinNumber = _pin;
            }

            return View("~/Views/Feature/Bills/ClearanceCertificate/_VerifyCertificate.cshtml", _verifyModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VerifyCertificate(VerifyCertificateModel model)
        {
            bool status = false;
            string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

            if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
            {
                status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
            }
            else if (!ReCaptchaHelper.Recaptchasetting())
            {
                status = true;
            }
            if (status)
            {
                var verifiedresponse = DewaApiClient.GetVerifyClearanceCertificate(model.ReferenceNumber, model.PinNumber, string.Empty, RequestLanguage, Request.Segment());

                if (verifiedresponse != null)
                {
                    if (verifiedresponse.Succeeded && verifiedresponse.Payload != null && verifiedresponse.Payload.@return.responseCode.Equals("000"))
                    {
                        ViewBag.Message = "valid";
                        model.ReferenceNumber = verifiedresponse.Payload.@return.referenceNumber;
                        model.RequesterName = verifiedresponse.Payload.@return.name;
                        model.PinNumber = verifiedresponse.Payload.@return.pinNumber;
                        model.ContractAccountNumber = verifiedresponse.Payload.@return.contractAccount;
                        model.CertificateNote = verifiedresponse.Payload.@return.textMessage;
                        model.pdfData = verifiedresponse.Payload.@return.pdfData;
                    }
                    else if (verifiedresponse != null && verifiedresponse.Payload != null && !string.IsNullOrWhiteSpace(verifiedresponse.Payload.@return.responseCode) && verifiedresponse.Payload.@return.responseCode.Equals("398"))
                    {
                        ViewBag.Message = "expired";
                        ViewBag.MessageDesc = verifiedresponse.Payload.@return.description;
                    }
                    else
                    {
                        ViewBag.Message = "invalid";
                        ViewBag.MessageDesc = verifiedresponse.Payload.@return.description;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, verifiedresponse.Payload.@return.description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
            }
            return PartialView("~/Views/Feature/Bills/ClearanceCertificate/DocumentDetails.cshtml", model);
        }

        [HttpGet]
        public ActionResult ClearanceCertificateLanding()
        {
            ViewBag.IsLoggedIn = IsLoggedIn;
            return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestLanding.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ClearanceCertificateLanding(FormCollection collection)
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
                    case 1: //DEWA Customer
                            //ClearMoveInCache();
                            //ClearMoveToCache();
                        redirectItem = SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_DEWA_CUSTOMER;
                        break;

                    case 2: // Non-DEWA Customer
                            //ClearMoveInCache();
                            //ClearMoveToCache();
                        redirectItem = SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_NON_DEWA;
                        break;

                    case 3: //Property Seller
                        redirectItem = SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_PROPERTY_SELLER;
                        break;

                    case 4: // Move To
                            //ClearMoveToCache();
                            //ClearMoveInCache();
                        redirectItem = SitecoreItemIdentifiers.VERIFY_CLEARANCE_CERTIFICATE;
                        break;
                }
            }
            return RedirectToSitecoreItem(redirectItem);
        }

        [HttpGet, TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult RequestDEWACustomer()
        {
            CacheProvider.Remove(CacheKeys.CLEARANCE_PAYMENT_DETAILS);
            CacheProvider.Remove(CacheKeys.CLEARANCE_FAILED);
            CacheProvider.Remove(CacheKeys.CLEARANCE_SENT);

            var primaryAccountResponse = DewaApiClient.GetPrimaryAccount(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

            if (primaryAccountResponse.Succeeded)
            {
                var primaryAccount = primaryAccountResponse.Payload;
                var cert = DewaApiClient.GetClearanceCertificate(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,
                    primaryAccount.AccountNumber, RequestLanguage, Request.Segment());

                if (cert.Succeeded)
                {
                    var model = new AuthenticatedClearanceCertificateModel
                    {
                        FirstName = cert.Payload.FirstName ?? string.Empty,
                        LastName = cert.Payload.LastName ?? string.Empty,
                        EmailAddress = cert.Payload.Email,
                        MobileNumber = cert.Payload.Mobile.RemoveMobileNumberZeroPrefix(),
                        ContractAccountNumber = primaryAccount.AccountNumber,
                        BusinessPartnerNumber = primaryAccount.BusinessPartner,
                        //CertificateCost =
                        //    SitecoreContext.GetItem<CostsConfig>(SitecoreItemIdentifiers.COSTS_CONFIG)
                        //        .ClearanceCertificateCost,
                        CertificateCost = cert.Payload.ClearanceCharge,
                        OutstandingBill = cert.Payload.Amount > 0 ? cert.Payload.Amount : 0,
                        City = cert.Payload.Region,
                        PoBox = cert.Payload.PoBox,
                        TradeLicenseNumber = cert.Payload.TradeLicenseNumber,
                        Purposes = GetPurposes(),
                        VATamount = cert.Payload.ClearanceTax,
                        TotalClearanceCharges = cert.Payload.ClearanceTotal,
                        TaxRate = cert.Payload.TaxRate
                    };
                    string errorMessage;
                    AuthenticatedClearanceCertificateModel persistmodel;
                    if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                        CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                        CacheProvider.TryGet(CacheKeys.AUTHENTICATED_CLEARANCE_DETAILS, out persistmodel);
                        CacheProvider.Remove(CacheKeys.AUTHENTICATED_CLEARANCE_DETAILS);
                        if (persistmodel != null)
                        {
                            model.MobileNumber = persistmodel.MobileNumber;
                            model.Remarks = persistmodel.Remarks;
                            model.EmailAddress = persistmodel.EmailAddress;
                            model.Purpose = persistmodel.Purpose;
                        }
                        CacheProvider.Remove(CacheKeys.AUTHENTICATED_CLEARANCE_DETAILS);
                    }
                    return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestDewaCustomer.cshtml", model);
                }
                ModelState.AddModelError(string.Empty, cert.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text(DictionaryKeys.ClearanceCertificates.ActiveAccountNeeded));
            }

            CacheProvider.Store(CacheKeys.CLEARANCE_FAILED, new CacheItem<string>(ModelState.AsFormattedString()));

            return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestFailed.cshtml");
        }

        [HttpPost, TwoPhaseAuthorize, ValidateAntiForgeryToken]
        public ActionResult RequestDEWACustomer(AuthenticatedClearanceCertificateModel model)
        {
            List<HttpPostedFileBase> uploads = new List<HttpPostedFileBase>();
            CacheProvider.Store(CacheKeys.AUTHENTICATED_CLEARANCE_DETAILS, new CacheItem<AuthenticatedClearanceCertificateModel>(model));
            string error = "";
            for (var i = 0; i < uploads.Count; i++)
            {
                if (!AttachmentIsValid(uploads[i], General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                    ViewBag.Message += error;
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                    break;
                }
            }

            if (ModelState.IsValid)
            {
                var request = Transform(model, uploads);
                var response = DewaApiClient.RequestClearanceCertificate(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, request, RequestLanguage, Request.Segment());

                if (response.Succeeded)
                {
                    var paymentModel = new ClearanceCertificatePaymentModel
                    {
                        ContractAccountNumber = response.Payload.ContractAccountNumber,
                        BusinessPartnerNumber = model.BusinessPartnerNumber,
                        Amount = model.TotalPayable,
                        TransactionNumber = response.Payload.TransactionNumber
                    };

                    CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_DETAILS, new CacheItem<ClearanceCertificatePaymentModel>(paymentModel));

                    //return RedirectToAction("RedirectForClearance", "Payment", new ClearancePaymentRequestModel
                    //{
                    //    ContractAccount = model.ContractAccountNumber,
                    //    BusinessPartnerNumber = model.BusinessPartnerNumber,
                    //    Amount = model.TotalPayable,
                    //    EmailAddress = model.EmailAddress,
                    //    MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                    //    Username = CurrentPrincipal.UserId,
                    //    IsAnonymous = false,
                    //    TransactionNumber = response.Payload.TransactionNumber
                    //});

                    #region [MIM Payment Implementation]

                    if (Config.IsSecuredMIMEnabled)
                    {
                        var payRequest = new CipherPaymentModel();
                        payRequest.PaymentData.amounts = Convert.ToString(model.TotalPayable);
                        payRequest.PaymentData.contractaccounts = model.ContractAccountNumber;
                        payRequest.PaymentData.businesspartner = model.BusinessPartnerNumber;
                        payRequest.PaymentData.email = model.EmailAddress;
                        payRequest.PaymentData.mobile = model.MobileNumber?.AddMobileNumberZeroPrefix();
                        payRequest.PaymentData.clearancetransaction = response.Payload.TransactionNumber;
                        payRequest.ServiceType = ServiceType.Clearance;
                        payRequest.PaymentMethod = model.paymentMethod;
                        payRequest.PaymentData.userid = CurrentPrincipal.UserId;
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
                    }

                    #endregion [MIM Payment Implementation]
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J26_REQUEST_POST_LOGIN);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J26_REQUEST_POST_LOGIN);
            // below lines is not tested ever and not working.
            //model.Purposes = GetPurposes();
            //return PartialView("_RequestPostLogin", model);
        }

        //Added by Mitesh RequestProperty Seller Clerance Certificate
        [HttpGet, TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult RequestPropertySeller()
        {
            CacheProvider.Remove(CacheKeys.CLEARANCE_PAYMENT_DETAILS);
            CacheProvider.Remove(CacheKeys.CLEARANCE_FAILED);
            CacheProvider.Remove(CacheKeys.CLEARANCE_SENT);

            string errorMessage, contractAccountNumber;
            var persistmodel = new ClearanceCertificatePropertySeller();
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
                CacheProvider.TryGet(CacheKeys.PropertySeller_CLEARANCE_DETAILS, out persistmodel);
                CacheProvider.Remove(CacheKeys.PropertySeller_CLEARANCE_DETAILS);
            }
            if (CacheProvider.TryGet(CacheKeys.CLEARANCE_CERTIFICATE_FinalFlow, out contractAccountNumber) && !string.IsNullOrEmpty(contractAccountNumber))
            {
                ViewBag.ContractAccountNumber = contractAccountNumber;
            }
            return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestPropertySeller.cshtml", persistmodel);
        }

        [HttpPost, TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false), ValidateAntiForgeryToken]
        public ActionResult RequestPropertySeller(ClearanceCertificatePropertySeller model)
        {
            CacheProvider.Store(CacheKeys.PropertySeller_CLEARANCE_DETAILS, new CacheItem<ClearanceCertificatePropertySeller>(model));
            string error = "";
            if (ModelState.IsValid)
            {
                if (model.OutstandingBill > 0)
                {
                    var paymentModel = new ClearanceCertificatePaymentModel
                    {
                        ContractAccountNumber = model.ReceivedContractAccountNumber,
                        BusinessPartnerNumber = model.BusinessPartnerNumber,
                        Amount = model.OutstandingBill,
                        TransactionNumber = string.Empty
                    };

                    CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_DETAILS, new CacheItem<ClearanceCertificatePaymentModel>(paymentModel));

                    var dictionary = new Dictionary<string, string>();
                    dictionary.Add("ClearanceCertificatePayment", SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_PROPERTY_SELLER);
                    CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_PATH, new CacheItem<Dictionary<string, string>>(dictionary));

                    #region [MIM Payment Implementation]

                    if (Config.IsSecuredMIMEnabled)
                    {
                        var payRequest = new CipherPaymentModel();
                        payRequest.PaymentData.amounts = Convert.ToString(!string.IsNullOrWhiteSpace(model.Amounts) ? model.Amounts : model.OutstandingBill.ToString());
                        payRequest.PaymentData.contractaccounts = model.ReceivedContractAccountNumber;
                        payRequest.PaymentData.businesspartner = model.BusinessPartnerNumber;
                        payRequest.PaymentData.email = model.EmailAddress;
                        payRequest.PaymentData.mobile = model.MobileNumber?.AddMobileNumberZeroPrefix();
                        payRequest.ServiceType = ServiceType.PayBill;
                        payRequest.PaymentMethod = model.paymentMethod;
                        payRequest.PaymentData.userid = CurrentPrincipal.UserId;
                        payRequest.IsThirdPartytransaction = false;
                        payRequest.BankKey = model.bankkey;
                        payRequest.SuqiaValue = model.SuqiaDonation;
                        payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                        var payResponse = ExecutePaymentGateway(payRequest);
                        if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                        {
                            CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_MODEL, new CacheItem<CipherPaymentModel>(payResponse));
                            CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_Details_Propertyseller, new CacheItem<ClearanceCertificatePropertySeller>(model));
                            return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                        }
                        ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                    }

                    #endregion [MIM Payment Implementation]
                }
                else
                {
                    if (model.AttachedDocument != null && !AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        try
                        {
                            byte[] attachmentBytes = new byte[0];
                            var attachmentType = string.Empty;

                            if (model.AttachedDocument != null)
                            {
                                attachmentBytes = model.AttachedDocument.ToArray();
                                attachmentType = model.AttachedDocument.FileName.GetFileNameWithoutPath();
                            }
                            var request = new RequestClearanceCertificate
                            {
                                Branch = FetchFutureCenterValues().Branch,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                EmailAddress = model.EmailAddress,
                                MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                                Remarks = model.Remarks,
                                ContractAccountNumber = model.ContractAccountNumber,
                                Attachment1 = attachmentBytes,
                                Attachment1Extension = attachmentType,
                                Purpose = "3",
                                cclang = model.Languague
                            };

                            // var response = DewaApiClient.RequestClearanceCertificate(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, request, RequestLanguage, Request.Segment());

                            var response = DewaApiClient.ApplyClearanceCertificate(request, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                            if (response.Succeeded)
                            {
                                CacheProvider.Remove(CacheKeys.PropertySeller_CLEARANCE_DETAILS);
                                CacheProvider.Remove(CacheKeys.CLEARANCE_CERTIFICATE_FinalFlow);
                                CacheProvider.Remove(CacheKeys.CLEARANCE_CERTIFICATE_AccountList);

                                var context = ContentRepository.GetItem<AccountSelector>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{E348D8E5-45D8-4654-B81C-722F62AF7879}")));

                                if (!string.IsNullOrEmpty(context.NotificationCode))
                                {
                                    CacheProvider.Remove(context.NotificationCode);
                                }

                                return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestDewaCustomerSuccess.cshtml", new AuthenticatedClearanceCertificateModel() { NotificationNumber = response.Payload.@return.notificationNumber });
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, response.Message);
                            }
                        }
                        catch (System.Exception)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                        }
                    }
                }
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_PROPERTY_SELLER);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_PROPERTY_SELLER);
        }

        [HttpGet]
        public ActionResult RequestNonDEWA()
        {
            if (IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_DEWA_CUSTOMER);
            }

            CacheProvider.Remove(CacheKeys.CLEARANCE_PAYMENT_DETAILS);
            CacheProvider.Remove(CacheKeys.CLEARANCE_FAILED);
            CacheProvider.Remove(CacheKeys.CLEARANCE_SENT);
            AnonymousClearanceCertificateModel persistmodel;
            string errorMessage;

            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }

            if (!CacheProvider.TryGet(CacheKeys.ANONYMOUS_CLEARANCE_DETAILS, out persistmodel))
            {
                persistmodel = new AnonymousClearanceCertificateModel();
                CacheProvider.Remove(CacheKeys.ANONYMOUS_CLEARANCE_DETAILS);
            }

            //persistmodel.ContractAccountNumber = SitecoreContext.GetItem<CostsConfig>(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_ACCOUNT).ClearanceCertificateCost.ToString();

            //var cert = DewaApiClient.GetClearanceCertificate(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,
            //		persistmodel.ContractAccountNumber, RequestLanguage, Request.Segment());

            //if (cert != null)
            //{
            persistmodel.Emirates = GetEmirates();
            DEWAXP.Foundation.Integration.DewaSvc.clearanceMasterInput clearanceMasterInput = new DEWAXP.Foundation.Integration.DewaSvc.clearanceMasterInput()
            {
                selectall = "X",
                service = "01",
                scenario = "02"
            };
            var response = DewaApiClient.GetClearanceFieldMaster(clearanceMasterInput, RequestLanguage, Request.Segment());

            if (response.Succeeded && response.Payload != null &&
                response.Payload.clearancemasterlist != null && response.Payload.clearancemasterlist.Count() > 0)
            {
                persistmodel.NonDEWAPurposes = response.Payload.clearancemasterlist?.Where(x => x.fieldname == "PURPOSE").ToList();
                persistmodel.CA_Setting = response.Payload.clearancemasterlist?.Where(x => x.fieldname == "CONTRACT_ACC").FirstOrDefault();
            }
            persistmodel.CustomerTypes = GetCustomerTypes();
            persistmodel.IdentityTypes = GetIdTypes();
            //persistmodel.CertificateCost = cert.Payload.ClearanceCharge;
            //persistmodel.VATamount = cert.Payload.ClearanceTax;
            //persistmodel.TotalClearanceCharges = cert.Payload.ClearanceTotal;
            //persistmodel.TaxRate = cert.Payload.TaxRate;
            return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestNonDewa.cshtml", persistmodel);
            //}

            //return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestFailed.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RequestNonDEWA(AnonymousClearanceCertificateModel model, List<HttpPostedFileBase> uploads)
        {
            CacheProvider.Store(CacheKeys.ANONYMOUS_CLEARANCE_DETAILS, new CacheItem<AnonymousClearanceCertificateModel>(model));
            string error = "";

            var validUploads = uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).ToList();

            for (var i = 0; i < validUploads.Count; i++)
            {
                if (!AttachmentIsValid(validUploads[i], General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_NON_DEWA);
                }
            }

            ModelState["ContractAccountNumber"].Errors.Clear();
            ModelState["CertificateCost"].Errors.Clear();

            if (Convert.ToBoolean(model.IsAccountNO) && string.IsNullOrWhiteSpace(model.ContractAccountNumber))
            {
                ModelState.AddModelError("ContractAccountNumbers", Translate.Text("CC_AC_ErrorMsg"));
            }

            if (ModelState.IsValid)
            {
                model.Branch = FetchFutureCenterValues().Branch;

                var request = Transform(model, uploads);
                var response = DewaApiClient.ApplyClearanceCertificate(request, "", "", RequestLanguage, Request.Segment());

                if (response.Succeeded && response.Payload != null && response.Payload.@return != null)
                {
                    var successModel = new AnonymousClearanceCertificateSuccessModel
                    {
                        NotificationNumber = response.Payload.@return.notificationNumber
                    };
                    //	var paymentModel = new ClearanceCertificatePaymentModel
                    //	{
                    //		//ContractAccountNumber = response.Payload.ContractAccountNumber,
                    //		BusinessPartnerNumber = string.Empty,
                    //		Amount = model.CertificateCost + model.VATamount,
                    //		//TransactionNumber = response.Payload.TransactionNumber
                    //	};

                    //	CacheProvider.Store(CacheKeys.CLEARANCE_PAYMENT_DETAILS, new CacheItem<ClearanceCertificatePaymentModel>(paymentModel));

                    //	//return RedirectToAction("RedirectForClearance", "Payment", new ClearancePaymentRequestModel
                    //	//{
                    //	//    ContractAccount = model.ContractAccountNumber,
                    //	//    BusinessPartnerNumber = string.Empty,
                    //	//    Amount = model.CertificateCost + model.VATamount,
                    //	//    EmailAddress = model.EmailAddress,
                    //	//    MobileNumber = model.MobileNumber,
                    //	//    IdentityNumber = model.IdentityNumber,
                    //	//    Username = "anonymous",
                    //	//    IsAnonymous = true,
                    //	//    TransactionNumber = response.Payload.TransactionNumber
                    //	//});
                    //	decimal amount = model.CertificateCost + model.VATamount;
                    //	var fullUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.PaymentRedirect); //this.Url.Action("RedirectForClearance", "Payment");
                    //	PaymentRedirectModel pay = new PaymentRedirectModel()
                    //	{
                    //		c = model.ContractAccountNumber,
                    //		b = string.Empty,
                    //		a = amount.ToString() ?? string.Empty,
                    //		em = model.EmailAddress,
                    //		mb = model.MobileNumber,
                    //		emid = model.IdentityNumber,
                    //		u = "anonymous",
                    //		ThirdPartyPayment = true,
                    //		x = response.Payload.@return.transactionNumber,
                    //		EPayUrl = fullUrl,
                    //		type = "Clearance"
                    //	};
                    //	return View("~/Views/Payment/PaymentRedirect.cshtml", pay);
                    //}

                    //ModelState.AddModelError(string.Empty, response.Message);

                    //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                    CacheProvider.Remove(CacheKeys.ANONYMOUS_CLEARANCE_DETAILS);
                    return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_RequestNonDewaCustomerSuccess.cshtml", successModel);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);

                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_NON_DEWA);
                }
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_NON_DEWA);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult RedirectNonDewaToLogin(string message)
        {
            AntiForgery.Validate();
            ModelState.AddModelError(string.Empty, message);

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(ModelState.AsFormattedString()));

            var item = ContentRepository.GetItem<Item>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_NON_DEWA)));

            if (item != null)
                return Json(LinkManager.GetItemUrl(item));
            else
                return Json(string.Empty);

            //return RedirectToSitecoreItem(SitecoreItemIdentifiers.CLEARANCE_CERTIFICATE_DEWA_CUSTOMER);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VerifyDoc(VerifyCertificateModel model)
        {
            try
            {
                bool status = false;

                if (status)
                {
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View("VerifyDocumentation", model);
        }

        #endregion Clearance Certificate v2

        #region Helpers

        private IEnumerable<SelectListItem> GetPurposes()
        {
            var purposes = GetDictionaryListByKey(DictionaryKeys.ClearanceCertificates.Purposes, DictionaryKeys.ClearanceCertificates.PurposeList);

            return from itm in purposes.ToList()
                   select new SelectListItem()
                   {
                       Text = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownTermValues),
                       Value = itm["Key"]?.Substring(0, 1) ?? ""
                   };
        }

        private IEnumerable<SelectListItem> GetEmirates()
        {
            var emirates = GetDictionaryListByKey(DictionaryKeys.Global.Emirates);

            return from itm in emirates.ToList()
                   select new SelectListItem()
                   {
                       Text = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownTermValues),
                       Value = itm["Key"]
                   };
        }

        protected IEnumerable<SelectListItem> GetIdTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() {Text = Translate.Text("Emarites ID").AddToTermDictionary("ED", DropDownTermValues), Value = "ED"},
                new SelectListItem() {Text = Translate.Text("Passport").AddToTermDictionary("PP", DropDownTermValues), Value = "PP"},
                new SelectListItem() {Text = Translate.Text("Trade License").AddToTermDictionary("TN", DropDownTermValues), Value = "TN"},
                new SelectListItem() {Text = Translate.Text("Trade License - Others").AddToTermDictionary("TO", DropDownTermValues), Value = "TO"}
            };
        }

        private IEnumerable<SelectListItem> GetCustomerTypes()
        {
            var customerTypes = GetDictionaryListByKey(DictionaryKeys.ClearanceCertificates.CustomerTypesKey, DictionaryKeys.ClearanceCertificates.CustomerTypeDictionary);

            return from itm in customerTypes.ToList()
                   select new SelectListItem()
                   {
                       Text = itm["Phrase"].AddToTermDictionary(itm.DisplayName, DropDownTermValues),
                       Value = itm.DisplayName
                   };
        }

        public Dictionary<string, string> DropDownTermValues
        {
            get
            {
                Dictionary<string, string> dictionary;
                if (!CacheProvider.TryGet(CacheKeys.TERMS, out dictionary))
                {
                    dictionary = new Dictionary<string, string>();

                    CacheProvider.Store(CacheKeys.TERMS, new CacheItem<Dictionary<string, string>>(dictionary));
                }
                return dictionary;
            }
        }

        private static RequestClearanceCertificate Transform(AuthenticatedClearanceCertificateModel model, List<HttpPostedFileBase> uploads)
        {
            return new RequestClearanceCertificate
            {
                TotalPayable = model.TotalPayable,
                EmailAddress = model.EmailAddress,
                MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                ContractAccountNumber = model.ContractAccountNumber,
                Purpose = model.Purpose,
                FirstName = model.FirstName,
                LastName = model.LastName ?? string.Empty,
                Remarks = model.Remarks,
                City = model.City,
                PoBox = model.PoBox,
                TradeLicenseNumber = model.TradeLicenseNumber,
                Attachment1 = uploads.Count > 0 ? uploads[0].ToArray() : new byte[0],
                Attachment1Extension = uploads.Count > 0 ? uploads[0].FileName.GetFileNameWithoutPath() : null,
                Attachment2 = uploads.Count > 1 ? uploads[1].ToArray() : new byte[0],
                Attachment2Extension = uploads.Count > 0 ? uploads[1].FileName.GetFileNameWithoutPath() : null,
                Attachment3 = uploads.Count > 2 ? uploads[2].ToArray() : new byte[0],
                Attachment3Extension = uploads.Count > 0 ? uploads[2].FileName.GetFileNameWithoutPath() : null
            };
        }

        private static RequestClearanceCertificate Transform(AnonymousClearanceCertificateModel model, List<HttpPostedFileBase> uploads)
        {
            var uploadsCount = uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).Count();
            return new RequestClearanceCertificate
            {
                TotalPayable = model.CertificateCost,
                EmailAddress = model.EmailAddress.Contains("**") ? string.Empty : model.EmailAddress,
                MobileNumber = model.MobileNumber.Contains("*") ? string.Empty : model.MobileNumber.AddMobileNumberZeroPrefix(),
                ContractAccountNumber = model.ContractAccountNumber,
                Purpose = model.Purpose,
                CourtNumber = model.CourtReference,
                FirstName = model.CustomerType == "Individual" ? model.FirstName : model.Organization,
                LastName = model.LastName,
                Remarks = model.Emirate,
                Emirates = model.Emirate,
                PoBox = model.PoBox,
                TradeLicenseNumber = "TN".Equals(model.IdentityType) || "TO".Equals(model.IdentityType) ? model.IdentityNumber : null,
                TradeLicenseAuthority = model.IdentityType,
                IdentityNumber = "ED".Equals(model.IdentityType) ? model.IdentityNumber : null,
                PassportNumber = "PP".Equals(model.IdentityType) ? model.IdentityNumber : null,
                Branch = model.Branch,
                Attachment1 = uploadsCount > 0 ? uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).ToList()[0].ToArray() : new byte[0],
                Attachment1Extension = uploadsCount > 0 ? (uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).ToList()[0].FileName.GetFileNameWithoutPath()) : null,
                Attachment2 = uploadsCount > 1 ? uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).ToList()[1].ToArray() : new byte[0],
                Attachment2Extension = uploadsCount > 1 ? (uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).ToList()[1].FileName.GetFileNameWithoutPath()) : null,
                Attachment3 = uploadsCount > 2 ? uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).ToList()[2].ToArray() : new byte[0],
                Attachment3Extension = uploadsCount > 2 ? (uploads.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FileName)).ToList()[2].FileName.GetFileNameWithoutPath()) : null,
                cclang = model.Languague
            };
        }

        #endregion Helpers
    }
}