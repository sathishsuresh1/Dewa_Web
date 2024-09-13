using DEWAXP.Feature.SupplyManagement.Models.MoveIn;
using DEWAXP.Feature.SupplyManagement.Models.RERA;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class ReraMoveInController : BaseController
    {
        //private readonly ISitecoreContext _sitecoreContext;

        private readonly string RegistrationCacheKey = "ReraRegistrationSessionStore";

        private readonly string ErrorKey = string.Empty;
        private readonly string RegisterReraUserViewPath = "~/Views/Feature/SupplyManagement/RERA/ReraSetUsernameAndPassword.cshtml";
        private readonly string PaymentReraUserViewPath = "~/Views/Feature/SupplyManagement/RERA/_ReraMoveInPaymentDetails.cshtml";
        private readonly string ReraErrorLandingViewPath = "~/Views/Feature/SupplyManagement/RERA/_ReraMoveIn.cshtml";

        //public ReraMoveInController(ISitecoreContext sitecoreContext)
        //{
        //    _sitecoreContext = sitecoreContext;
        //}

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult MoveInRedirect()
        {
            var _fc = FetchFutureCenterValues();

            var language = Request.QueryString["l"];
            if (!string.IsNullOrEmpty(language))
            {
                try
                {
                    if (language.ToLower().Equals("en-us") || language.ToLower().Equals("en"))
                    {
                        SitecoreX.Language = Language.Parse("en");
                    }
                    else if (language.ToLower().Equals("ar-ae"))
                    {
                        SitecoreX.Language = Language.Parse("ar-AE");
                    }
                }
                catch (System.Exception ex)
                {
                    global::Sitecore.Diagnostics.Log.Error(string.Format("Could not parse language: {0}", language), ex, new object());
                }
            }
            var customername = string.Empty;
            var accountNumbers = GetAccountValues(QueryStringKeys.ReraMovenInEncryptedNumbers);
            var ReraAccountDetailsViewModel = new ReraAccountDetailsViewModel();
            if (accountNumbers != null && !string.IsNullOrEmpty(accountNumbers.ContractAccount) &&
                !string.IsNullOrEmpty(accountNumbers.BusinessPartner))
            {
                var respose = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                {
                    moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           businesspartnernumber=accountNumbers.BusinessPartner,
                           reracontractaccountnumber=FormatContractAccount(accountNumbers.ContractAccount),
                           center =_fc.Branch
                       }
                    },
                    userid = string.Empty,
                    sessionid = string.Empty,
                    lang = RequestLanguage.Code(),
                    executionflag = "E",
                    channel = "R",
                    applicationflag = "M"
                }, Request.Segment());

                if (respose != null && respose.Succeeded && respose.Payload != null)
                {
                    ReraAccountDetailsViewModel.CustomerName = respose.Payload.moveinnotif.firtname + " " + respose.Payload.moveinnotif.lastname;
                    ReraAccountDetailsViewModel.BusinessPartner = accountNumbers.BusinessPartner;
                    ReraAccountDetailsViewModel.ContractAccount = accountNumbers.ContractAccount;
                    ReraAccountDetailsViewModel.Total = 0;
                    ReraAccountDetailsViewModel.ResponseCode = Convert.ToDecimal(respose.Payload.responsecode);

                    if (respose.Payload.responsecode != "399")
                    {
                        //user already exists, so redirect user to payment.
                        if (respose.Payload.moveinnotif.userid != "")
                        {
                            return RedirectToPaymentReviewPage(respose.Payload, FormatContractAccount(accountNumbers.ContractAccount), true);
                        }

                        //no user, redirect to user registration

                        var userPassModel = new ReraSetUsernamePasswordModel
                        {
                            BusinessPartnerNumber = respose.Payload.moveinnotif.businesspartnernumber,
                            ContractAccountNumber = FormatContractAccount(accountNumbers.ContractAccount),
                            CustomerName = respose.Payload.moveinnotif.firtname + " " + respose.Payload.moveinnotif.firtname,
                            Username = string.Empty,
                            Password = string.Empty
                        };

                        var registrationModel = new ReraUserRegistrationModel
                        {
                            PayLoad = respose.Payload,
                            UsernamePasswordModel = userPassModel
                        };

                        CacheProvider.Store(RegistrationCacheKey, new CacheItem<ReraUserRegistrationModel>(registrationModel));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J73_RERA_USER_REGISTRATION);
                    }
                    else
                    {
                        //todo: what to do when account is inactive
                        return View(ReraErrorLandingViewPath, ReraAccountDetailsViewModel);
                    }
                }
            }

            return View(ReraErrorLandingViewPath, ReraAccountDetailsViewModel);
        }

        [HttpGet]
        private ActionResult RedirectToPaymentReviewPage(moveInPostOutput paymentDetails, string accountnumber, bool loggedin = false)
        {
            if (paymentDetails != null)
            {
                var paymentModel = new ReraPaymentModel();
                paymentModel.SecurityDeposit = Convert.ToDouble(paymentDetails.moveinnotif.totalsecuritydepositamount);
                paymentModel.ReconnectionRegistrationFee = Convert.ToDouble(paymentDetails.moveinnotif.totalreconnectionchargeamount);
                paymentModel.AddressRegistrationFee = Convert.ToDouble(paymentDetails.moveinnotif.totaladdresschangeamount);
                paymentModel.ReconnectionVATrate = paymentDetails.moveinnotif.reconnectionvatpercentage;
                paymentModel.ReconnectionVATamt = Convert.ToDouble(paymentDetails.moveinnotif.reconnectionvatamount);
                paymentModel.AddressVATrate = paymentDetails.moveinnotif.addresschangevatpercentage;
                paymentModel.AddressVAtamt = Convert.ToDouble(paymentDetails.moveinnotif.addresschangevatamount);
                paymentModel.KnowledgeFee = Convert.ToDouble(paymentDetails.moveinnotif.totalknowledgefeesamount);
                paymentModel.InnovationFee = Convert.ToDouble(paymentDetails.moveinnotif.totalinnovationfeesamount);
                paymentModel.TermsLink = "";
                paymentModel.BusinessPartner = paymentDetails.moveinnotif.businesspartnernumber;
                paymentModel.ContractAccountNumber = accountnumber;
                paymentModel.Email = paymentDetails.moveinnotif.email;
                paymentModel.UserName = paymentDetails.moveinnotif.firtname;
                paymentModel.UserId = paymentDetails.moveinnotif.userid;
                paymentModel.PayLater = !string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.sdpaylaterflag) ? paymentDetails.moveinnotif.sdpaylaterflag.Equals("X") : false;
                paymentModel.PayOther = !string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.sdpayotherflag) ? paymentDetails.moveinnotif.sdpayotherflag.Equals("X") : false;
                paymentModel.Easypayflag = !string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.easypayflag) ? paymentDetails.moveinnotif.easypayflag.Equals("X") : false;
                paymentModel.payotherchannelflag = !string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.payotherchannelflag) ? paymentDetails.moveinnotif.payotherchannelflag.Equals("X") : false;
                paymentModel.DiscountApplied = !string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.movetotransactionid) ? true : false;
                paymentModel.messagewhatsnext = paymentDetails.moveinScreenMessageList != null && paymentDetails.moveinScreenMessageList.Count() > 0 && paymentDetails.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")) != null ? paymentDetails.moveinScreenMessageList.ToList().Where(x => x.category.Equals("WN")).Select(y => y.message).ToArray() : new string[] { };
                paymentModel.messagepaychannel = paymentDetails.moveinScreenMessageList != null && paymentDetails.moveinScreenMessageList.Count() > 0 && paymentDetails.moveinScreenMessageList.ToList().Where(x => x.category.Equals("PC")) != null ? paymentDetails.moveinScreenMessageList.ToList().Where(x => x.category.Equals("PC")).Select(y => y.message).ToArray() : new string[] { };
                paymentModel.MaiDubaiGift = !string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.maidubaigift) && paymentDetails.moveinnotif.maidubaigift.Equals("X") ? true : false;
                paymentModel.MaiDubaiMsg = paymentDetails.moveinnotif.maidubaimsgtext;
                paymentModel.MaiDubaiTitle = paymentDetails.moveinnotif.maidubaimsgtitle;
                paymentModel.MaiDubaiContribution = !string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.maidubaigift) && paymentDetails.moveinnotif.maidubaigift.Equals("X") ? true : false;

                CacheProvider.Store(CacheKeys.PaymentCacheKey, new CacheItem<ReraPaymentModel>(paymentModel));
                if (loggedin)
                {
                    if (!string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.skippayment))
                    {
                        paymentModel.MoveinNotificationnumber = paymentDetails.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                        CacheProvider.Store(CacheKeys.PaymentCacheKey, new CacheItem<ReraPaymentModel>(paymentModel));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.RERA_MOVEIN_CONFIRMATION_PAGE);
                    }
                    else
                    {
                        //ISitecoreContext isitecoreContext = new SitecoreContext();
                        var currentitem = ContextRepository.GetCurrentItem<Item>();
                        var lang = currentitem.Language.CultureInfo.TextInfo.IsRightToLeft ? "ar-AE" : "en";
                        CacheProvider.Store(CacheKeys.MOVEIN_USERID, new CacheItem<string>(paymentDetails.moveinnotif.userid));
                        QueryString a = new QueryString();
                        a.With("returnUrl", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J73_RERA_PAYMENT_DETAILS), true);
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J7_LOGIN_PAGE, a);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(paymentDetails.moveinnotif.skippayment))
                    {
                        paymentModel.MoveinNotificationnumber = paymentDetails.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                        CacheProvider.Store(CacheKeys.PaymentCacheKey, new CacheItem<ReraPaymentModel>(paymentModel));
                        ReraUserRegistrationModel register;
                        var password = string.Empty;
                        var createflag = string.Empty;
                        CacheProvider.TryGet("Reraregister", out register);
                        if (register != null && register.PayLoad != null)
                        {
                            password = register.UsernamePasswordModel.Password;
                            createflag = "X";
                        }
                        try
                        {
                            var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                            {
                                //premiseDetailsList = State.PremiseAccount.Where(x => x != "").Select(x => new premiseDetails { premise = x }).ToArray(),
                                moveInDetailsList = new moveInDetails[] {
                                   new moveInDetails {
                                       businesspartnernumber=paymentModel.BusinessPartner,
                                       reracontractaccountnumber=paymentModel.ContractAccountNumber,
                                       password=password,
                                       createuseraccount=createflag,
                                       sdpaylaterflag=string.Empty
                                   }
                                },
                                userid = paymentModel.UserId,
                                sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                                lang = RequestLanguage.Code(),
                                executionflag = "U",
                                channel = "R",
                                applicationflag = "M"
                            }, Request.Segment());
                            password = string.Empty;
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.RERA_MOVEIN_CONFIRMATION_PAGE);
                        }
                        catch (System.Exception ex)
                        {
                            return View(ReraErrorLandingViewPath);
                        }
                    }
                    else
                    {
                        //todo: check if account is active, otherwise what?
                        //redirect to payment
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J73_RERA_PAYMENT_DETAILS);
                    }
                }
            }

            return View(ReraErrorLandingViewPath);
        }

        [HttpGet]
        public ActionResult SetUsernameAndPassword()
        {
            var registrationSessionStore = new ReraUserRegistrationModel();
            CacheProvider.TryGet(RegistrationCacheKey, out registrationSessionStore);
            if (registrationSessionStore != null)
            {
                return View(RegisterReraUserViewPath, registrationSessionStore.UsernamePasswordModel);
            }

            //redirect to error page?
            return View(ReraErrorLandingViewPath);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetUsernameAndPassword(ReraSetUsernamePasswordModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                try
                {
                    var reraSessionStore = new ReraUserRegistrationModel();
                    var availability = DewaApiClient.VerifyUserIdentifierAvailable(model.Username, RequestLanguage, Request.Segment());
                    if (availability.Succeeded && availability.Payload.IsAvailableForUse)
                    {
                        CacheProvider.TryGet(RegistrationCacheKey, out reraSessionStore);
                        if (reraSessionStore != null && reraSessionStore.PayLoad != null)
                        {
                            reraSessionStore.PayLoad.moveinnotif.userid = model.Username;
                            reraSessionStore.UsernamePasswordModel.UserId = model.Username;
                            reraSessionStore.UsernamePasswordModel.Password = model.Password;
                            CacheProvider.Store("Reraregister", new CacheItem<ReraUserRegistrationModel>(reraSessionStore));
                            return RedirectToPaymentReviewPage(reraSessionStore.PayLoad, reraSessionStore.UsernamePasswordModel.ContractAccountNumber);   //redirect to payment screen
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, availability.Message);
                    }

                    ModelState.AddModelError(ErrorKey, reraSessionStore.PayLoad.description);
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError(ErrorKey, Translate.Text("Unexpected error"));
                }
            }
            else
            {
                if (model == null)
                {
                    ModelState.AddModelError(ErrorKey, Translate.Text("Unexpected error"));
                }
            }

            // Reset passwords
            if (model != null)
            {
                var emptyValue = new ValueProviderResult(string.Empty, string.Empty, CultureInfo.CurrentCulture);
                ModelState.SetModelValue("Password", emptyValue);
                model.Password = string.Empty;

                ModelState.SetModelValue("ConfirmPassword", emptyValue);
                model.ConfirmPassword = string.Empty;
            }

            return View(RegisterReraUserViewPath, model);
        }

        [HttpGet]
        public ActionResult PaymentDetails()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            ReraPaymentModel state;
            if (CacheProvider.TryGet(CacheKeys.PaymentCacheKey, out state))
            {
                CacheProvider.Remove(CacheKeys.MOVEIN_USERID);
                return View(PaymentReraUserViewPath, state);
            }
            return View(ReraErrorLandingViewPath);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PaymentDetails(ReraPaymentModel model)
        {
            if (model.IsPayLaterClicked)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.RERA_MOVEIN_PAYLATER_REVIEW_PAGEv3);
            }
            else
            {
                var password = string.Empty;
                var createflag = string.Empty;
                ReraUserRegistrationModel register;
                bool IsPayLaterClicked = model.IsPayLaterClicked;
                bool IsPayOtherClicked = model.IsPayotherSelected;
                CacheProvider.TryGet("Reraregister", out register);
                if (register != null && register.PayLoad != null)
                {
                    password = register.UsernamePasswordModel.Password;
                    createflag = "X";
                }
                bool processed = false;
                if (CacheProvider.TryGet(CacheKeys.MOVEIN_RERA_PROCESSED, out processed))
                {
                    processed = true;
                }
                var paymentMethod = model.paymentMethod;
                ReraPaymentModel paymentCacheKeystate;
                if (CacheProvider.TryGet(CacheKeys.PaymentCacheKey, out paymentCacheKeystate) && paymentCacheKeystate!=null)
                {
                    paymentCacheKeystate.SuqiaDonation = model.SuqiaDonation;
                    paymentCacheKeystate.SuqiaDonationAmt = model.SuqiaDonationAmt;
                    paymentCacheKeystate.bankkey = model.bankkey;
                    paymentCacheKeystate.paymentMethod = model.paymentMethod;

                    model = paymentCacheKeystate;

                    var amount = Convert.ToDecimal(model.Total.ToString() ?? "0.00");
                    try
                    {
                        var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                        {
                            //premiseDetailsList = State.PremiseAccount.Where(x => x != "").Select(x => new premiseDetails { premise = x }).ToArray(),
                            moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           businesspartnernumber=model.BusinessPartner,
                           reracontractaccountnumber=model.ContractAccountNumber,
                           password=password,
                           createuseraccount=processed?string.Empty:createflag,
                           sdpaylaterflag=IsPayLaterClicked?"X":string.Empty,
                           sdpayotherflag=IsPayOtherClicked?"X":string.Empty,
                           maidubaicond = model.MaiDubaiContribution?"X":string.Empty,

                       }
                    },
                            userid = model.UserId,
                            sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                            lang = RequestLanguage.Code(),
                            executionflag = "E",
                            channel = "R",
                            applicationflag = "M"
                        }, Request.Segment());
                        password = string.Empty;
                        if (response.Succeeded)
                        {
                            CacheProvider.Store(CacheKeys.MOVEIN_RERA_PROCESSED, new CacheItem<bool>(true));
                            if (IsPayLaterClicked || IsPayOtherClicked)
                            {
                                model.MoveinNotificationnumber = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                                CacheProvider.Store(CacheKeys.PaymentCacheKey, new CacheItem<ReraPaymentModel>(model));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.RERA_MOVEIN_CONFIRMATION_PAGE);
                            }
                            else
                            {
                                #region [MIM Payment Implementation]

                                var payRequest = new CipherPaymentModel();
                                payRequest.PaymentData.amounts = Convert.ToString(amount);
                                payRequest.PaymentData.contractaccounts = model.ContractAccountNumber ?? "";
                                payRequest.PaymentData.businesspartner = model.BusinessPartnerNumber;
                                payRequest.PaymentData.userid = model.UserId;
                                payRequest.PaymentData.email = model.Email;
                                payRequest.PaymentData.mobile = model.Mobile;
                                payRequest.IsThirdPartytransaction = false;
                                payRequest.ServiceType = ServiceType.ReraServiceActivation;
                                payRequest.PaymentMethod = paymentMethod;
                                payRequest.BankKey = model.bankkey;
                                payRequest.SuqiaValue = model.SuqiaDonation;
                                payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                                var payResponse = ExecutePaymentGateway(payRequest);
                                if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                                {
                                    CacheProvider.Store(CacheKeys.PaymentReraCacheKey, new CacheItem<string>("RERAMovein"));
                                    CacheProvider.Store(CacheKeys.MOVEIN_RERA_MIM_MODEL, new CacheItem<CipherPaymentModel>(payRequest));
                                    return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                                }
                                ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                                #endregion [MIM Payment Implementation]

                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogService.Error(ex, this);
                    }
                }
            }
            return View(ReraErrorLandingViewPath);
        }

        [HttpGet]
        public ActionResult PaylaterReview()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            ReraPaymentModel state;
            if (CacheProvider.TryGet(CacheKeys.PaymentCacheKey, out state))
            {
                return View("~/Views/Feature/SupplyManagement/MoveIn/ReraPaylaterReview.cshtml", state);
            }
            return View(ReraErrorLandingViewPath);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PaylaterReview(ReraPaymentModel model)
        {
            try
            {
                var password = string.Empty;
                var createflag = string.Empty;
                ReraUserRegistrationModel register;
                CacheProvider.TryGet("Reraregister", out register);
                if (register != null && register.PayLoad != null)
                {
                    password = register.UsernamePasswordModel.Password;
                    createflag = "X";
                }
                bool processed = false;
                if (CacheProvider.TryGet(CacheKeys.MOVEIN_RERA_PROCESSED, out processed))
                {
                    processed = true;
                }
                if (CacheProvider.TryGet(CacheKeys.PaymentCacheKey, out model))
                {
                    var amount = Convert.ToDecimal(model.Total.ToString() ?? "0.00");
                    var response = DewaApiClient.SetMoveInPostRequest(new moveInPostInput
                    {
                        moveInDetailsList = new moveInDetails[] {
                       new moveInDetails {
                           businesspartnernumber=model.BusinessPartner,
                           reracontractaccountnumber=model.ContractAccountNumber,
                           password=password,
                           createuseraccount=processed?string.Empty:createflag,
                           sdpaylaterflag="X",
                           sdpayotherflag=string.Empty
                       }
                    },
                        userid = model.UserId,
                        sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        lang = RequestLanguage.Code(),
                        executionflag = "E",
                        channel = "R",
                        applicationflag = "M"
                    }, Request.Segment());
                    password = string.Empty;
                    if (response.Succeeded)
                    {
                        CacheProvider.Store(CacheKeys.MOVEIN_RERA_PROCESSED, new CacheItem<bool>(true));
                        model.MoveinNotificationnumber = response.Payload.premiseAmountDetailsList.Select(x => x.transactionid).FirstOrDefault();
                        CacheProvider.Store(CacheKeys.PaymentCacheKey, new CacheItem<ReraPaymentModel>(model));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.RERA_MOVEIN_CONFIRMATION_PAGE);
                    }
                    else
                    {
                        return View(ReraErrorLandingViewPath);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(Translate.Text("Unexpected error")));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.RERA_MOVEIN_PAYLATER_REVIEW_PAGEv3);
        }

        public ActionResult ResendVerification()
        {
            var registrationSessionStore = new ReraUserRegistrationModel();

            if (!CacheProvider.TryGet(RegistrationCacheKey, out registrationSessionStore))
            {
                return View(ReraErrorLandingViewPath);
            }

            try
            {
                var response = DewaApiClient.SendVerificationCodeForRegistration
                    (registrationSessionStore.UsernamePasswordModel.BusinessPartnerNumber,
                    true, false, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J73_RERA_USER_REGISTRATION);
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View(ReraErrorLandingViewPath);
        }

        [HttpGet]
        public ActionResult PaylaterConfirmation()
        {
            ReraPaymentModel state;
            if (CacheProvider.TryGet(CacheKeys.PaymentCacheKey, out state))
            {
                return View("~/Views/Feature/SupplyManagement/MoveIn/ReraPaylaterConfirmation.cshtml", state);
            }
            return View(ReraErrorLandingViewPath);
        }
    }
}