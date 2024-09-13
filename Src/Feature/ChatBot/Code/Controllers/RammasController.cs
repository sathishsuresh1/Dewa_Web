using DEWAXP.Feature.ChatBot.Filters;
using DEWAXP.Feature.ChatBot.Models.DirectLine;
using DEWAXP.Feature.ChatBot.Models.Rammas;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.AccountModel;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Models.RammasLogin;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.ForgotPassword;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.JobSeekerSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Roles = DEWAXP.Foundation.Content.Roles;
using SitecoreX = Sitecore;

namespace DEWAXP.Feature.ChatBot.Controllers
{
    public class RammasController : BaseController
    {
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            int magicNumber = RammasParameter.GenerateRandomNumber();

            if (Request.QueryString["conversationId"] != null && !string.IsNullOrEmpty(Request.QueryString["conversationId"]))
            {
                string error, rammassessionToken;
                if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
                {
                    if (string.Equals("authentication", Request.QueryString["loginType"], StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Log.Info("auth controller",this);
                        if (TryLoginWithSmartCustAuthentication(CurrentPrincipal.Username, CurrentPrincipal.SessionToken, out rammassessionToken))
                        {
                            CacheProvider.Store(CacheKeys.RAMMAS_LOGIN, new CacheItem<bool>(true));

                            //After login success please set session Id
                            if (RammasParameter.PostLoginCredentialsToBot(CurrentPrincipal.Username, rammassessionToken, Request.QueryString["channelId"], Request.QueryString["conversationId"], Request.QueryString["surl"], Request.QueryString["activityId"], Request.QueryString["fromId"], Request.QueryString["fromName"], Request.QueryString["toId"], Request.QueryString["toName"], Request.QueryString["languageCode"], out error, magicNumber, Request.QueryString["loginType"]))
                            {
                                Session["MagicNumber"] = magicNumber;
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.RammasLogin_SUCCESS);
                            }
                            else
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                            }
                        }
                    }
                }

                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                if (string.IsNullOrEmpty(returnUrl))
                {
                    if (Session[BotService._conversation_key] == null)
                    {
                        ClearSessionAndSignOut();
                    }
                }
                // ViewBag.loginattempt = 1;
            }
            else
            {
                if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                }

                string error;
                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                if (string.IsNullOrEmpty(returnUrl))
                {
                    ClearSessionAndSignOut();
                }

                ViewBag.ReturnUrl = returnUrl;
            }
            return PartialView("~/Views/Feature/Rammas/Rammas/_LoginFormRammas.cshtml");
        }

        [HttpPost, AntiForgeryHandleError, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string channelId, string conversationId, string surl, string activityId, string fromId, string fromName, string toId, string toName, string languageCode, string loginattempt, string loginType, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(conversationId))
                {
                    try
                    {
                        int magicNumber = RammasParameter.GenerateRandomNumber();

                        if (string.Equals("authentication", loginType, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ServiceResponse<LoginResponse> response;
                            string error, sessionId, rammasSessionId;

                            if (TryLogin(model, out response, out sessionId))
                            {
                                //if user login success please pass user name, password and remaining perameters through post call
                                // Log.Info("auth trylogindone", this);

                                if (TryLoginWithSmartCustAuthentication(model.Username, sessionId, out rammasSessionId))
                                {
                                    //Set Cache value for Rammas login. It is used for displaying back button rendering.
                                    CacheProvider.Store(CacheKeys.RAMMAS_LOGIN, new CacheItem<bool>(true));
                                    //After login succuess please set session id
                                    if (RammasParameter.PostLoginCredentialsToBot(model.Username, rammasSessionId, channelId, conversationId, surl, activityId, fromId, fromName, toId, toName, languageCode, out error, magicNumber, loginType))
                                    {
                                        Session["MagicNumber"] = magicNumber;
                                        //CacheProvider.Store(CacheKeys.RAMMAS_CONVERSATION_ID, new CacheItem<string>(conversationId));
                                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.RammasLogin_SUCCESS);
                                    }
                                    else
                                    {
                                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                                    }
                                }
                            }
                            //If user entered 6 times invalid login pass userName and sessionId is 'InvAtt' along with other parameters
                            if (response.Payload != null && response.Payload.ResponseCode.Equals("113"))
                            {
                                RammasParameter.PostLoginCredentialsToBot("InvAtt", "InvAtt", channelId, conversationId, surl, activityId, fromId, fromName, toId, toName, languageCode, out error, magicNumber, loginType);
                            }

                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                        else if (string.Equals("jobseeker", loginType, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string error = string.Empty, sessionId;
                            ServiceResponse<userLoginValidation> response;
                            if (TryLoginWithJobSeeker(model, out response, out sessionId))
                            {
                                //Log.Info("job seeker done", response);

                                //if user login success please pass user name, password and remaining perameters through post call
                                //If user entered 3 times invalid login pass userName and sessionId is 0  along with other parameters

                                //Set Cache value for Rammas login. It is used for displaying back button rendering.

                                if (Session[BotService._conversation_key] != null)
                                {
                                    //redirect back to Rammas chat page

                                    Session.Add("R01", sessionId);

                                    AuthStateService.Save(new DewaProfile(model.Username, response.Payload.credential, Roles.Jobseeker)
                                    {
                                        IsContactUpdated = true
                                    });

                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.RammasLandingPage);
                                }

                                CacheProvider.Store(CacheKeys.RAMMAS_LOGIN, new CacheItem<bool>(true));

                                //After login succuess please set session id
                                if (RammasParameter.PostLoginCredentialsToBot(model.Username, sessionId, channelId, conversationId, surl, activityId, fromId, fromName, toId, toName, languageCode, out error, magicNumber, loginType))
                                {
                                    Session["MagicNumber"] = magicNumber;
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.RammasLogin_SUCCESS);
                                }
                            }
                            //If user entered 6 times invalid login pass userName and sessionId is 'InvAtt' along with other parameters
                            if (response.Payload != null && response.Payload.errorcode == "10510")
                            {
                                RammasParameter.PostLoginCredentialsToBot("InvAtt", "InvAtt", channelId, conversationId, surl, activityId, fromId, fromName, toId, toName, languageCode, out error, magicNumber, loginType);
                                ModelState.AddModelError(string.Empty, response.Message);
                                ViewBag.ReturnUrl = returnUrl;
                                return PartialView("~/Views/Feature/Rammas/Rammas/_LoginFormRammas.cshtml", model);
                            }
                            ModelState.AddModelError(string.Empty, response.Message);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text(ex.InnerException.ToString()));
                    }
                }
                else
                {
                    try
                    {
                        ServiceResponse<LoginResponse> response;
                        string sessionId;
                        if (TryLogin(model, out response, out sessionId))
                        {
                            if (!string.IsNullOrWhiteSpace(returnUrl))
                            {
                                returnUrl = HttpUtility.UrlDecode(returnUrl);
                                return Redirect(returnUrl);
                            }
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                        }
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                    catch (System.Exception)
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    }
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("~/Views/Feature/Rammas/Rammas/_LoginFormRammas.cshtml", model);
        }

        private bool TryLogin(LoginModel model, out ServiceResponse<LoginResponse> response, out string sessionId)
        {
            //error = null;
            sessionId = null;

            //response = DewaApiClient.AuthenticateNew(model.Username, model.Password, RequestLanguage, Request.Segment());
            response = SmartCustomerClient.LoginUser(
                new LoginRequest
                {
                    getloginsessionrequest = new Getloginsessionrequest
                    {
                        userid = model.Username,
                        password = Base64Encode(model.Password),
                    }
                }, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                var primaryAccountNumber = string.Empty;
                var emailAddress = string.Empty;
                var mobileNumber = string.Empty;
                var fullName = string.Empty;
                var termsAndConditions = string.Empty;
                var isContactUpdated = false;

                if (response.Payload != null && !String.IsNullOrEmpty(response.Payload.AccountNumber))
                {
                    primaryAccountNumber = response.Payload.AccountNumber;
                    emailAddress = response.Payload.Email;
                    mobileNumber = response.Payload.Mobile;
                    fullName = response.Payload.FullName;
                }
                termsAndConditions = response.Payload.AcceptedTerms ? "X" : string.Empty;
                isContactUpdated = response.Payload.IsUpdateContact;
                AuthStateService.Save(new DewaProfile(model.Username, response.Payload.SessionNumber)
                {
                    PrimaryAccount = primaryAccountNumber,
                    EmailAddress = emailAddress,
                    MobileNumber = mobileNumber,
                    FullName = fullName,
                    HasActiveAccounts = true,
                    TermsAndConditions = termsAndConditions,
                    IsContactUpdated = isContactUpdated
                });

                //StoreProfilePhoto(model.Username, response.Payload);
                sessionId = response.Payload.SessionNumber;
                return true;
            }
            //error = response.Message;
            return false;
        }

        private bool TryLoginWithSmartCustAuthentication(string userId, string sessionId, out string rammasSessionId)
        {
            //Log.Info("auth session", this);

            rammasSessionId = null;

            var response = SmartCustAuthenticationServiceClient.GetLoginSessionCustomerAuthentication(userId, sessionId, RequestLanguage, Request.Segment());
            //Log.Info("auth response", response);

            if (response.Succeeded)
            {
                rammasSessionId = response.Payload.SessionNumber;
                return true;
            }
            //error = response.Message;
            return false;
        }

        private bool TryLoginWithJobSeeker(LoginModel model, out ServiceResponse<userLoginValidation> response, out string sessionId)
        {
            //Log.Info("TryLoginWithJobSeeker", "");

            sessionId = null;

            response = JobSeekerClient.GetValidateCandidateLogin(model.Username, Base64Encode(model.Password),true, RequestLanguage, Request.Segment());
            // Log.Info("TryLoginWithJobSeeker response", response);

            if (response.Succeeded)
            {
                sessionId = response.Payload.credential;

                return true;
            }
            //error = response.Message;
            return false;
        }

        public void ClearSessionAndSignOut()
        {
            DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
        }

        [HttpGet]
        public ActionResult Success()
        {
            return PartialView("~/Views/Feature/Rammas/Rammas/_RammasSuccess.cshtml");
        }

        public ActionResult BacktoRammas()
        {
            try
            {
                var rammasData = ContentRepository.GetItem<RammasModel>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.RammasConfigID)));
                bool rammasModel;
                string _rammasref = string.Empty;

                CacheProvider.TryGet(CacheKeys.REF_RAMMAS, out _rammasref);
                if (Request.QueryString["ref"] != null)
                {
                    CacheProvider.Store(CacheKeys.REF_RAMMAS, new CacheItem<string>(Request.QueryString["ref"]));
                    ViewBag.RammasParam = Request.QueryString["ref"];
                }
                else
                {
                    ViewBag.RammasParam = _rammasref;
                }

                if (CacheProvider.TryGet(CacheKeys.RAMMAS_LOGIN, out rammasModel))
                {
                    ViewBag.RammasLogin = rammasModel;
                }
                return PartialView("~/Views/Feature/Rammas/Rammas/BacktoRammas.cshtml", rammasData);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Info(ex.Message + "Back to Rammas -", this);
                return null;
            }
        }

        [HttpGet]
        public string RammasClose()
        {
            if (CacheKeys.RAMMAS_LOGIN != null)
            {
                CacheProvider.Remove(CacheKeys.RAMMAS_LOGIN);
            }
            if (CacheKeys.REF_RAMMAS != null)
            {
                CacheProvider.Remove(CacheKeys.REF_RAMMAS);
            }
            return "Success";
        }

        public ActionResult RammasEPayment(string conversationId, string channelId = "", string LanguageCode = "")
        {
            //Context = model.PaymentContext;
            var lang = RequestLanguage.Code().ToUpperInvariant();

            RammasParameter ramasparameter = new RammasParameter();
            RammasGateWayResponse rammasgatewayresponse = new RammasGateWayResponse();

            bool isRammasFacebookPayment = false;
            try
            {
                var data = ramasparameter.FetchBotState(conversationId).Result;

                if (data == null) // || string.IsNullOrEmpty(data.TransactionType))
                {
                    Log.Error("Rammas Easy Pay Type null or empty", this);

                    ModelState.AddModelError("Error", "Please ensure that amounts have been specified for all contract accounts");

                    goto JumpHere;
                }

                #region [MIM Payment Implementation]

                if (Config.IsSecuredMIMEnabled)
                {
                    var payReqt = new CipherPaymentModel();
                    payReqt.PaymentData.amounts = data.Amount;
                    payReqt.PaymentData.contractaccounts = data.ContractAccounts;
                    payReqt.PaymentMethod = PaymentContextExtensions.GetPaymentMethod(!string.IsNullOrWhiteSpace(channelId) ? channelId.ToLower().ToString() : string.Empty);
                    if (data.ChannelId == "facebook" || data.ChannelId == "instagram")
                    {
                        isRammasFacebookPayment = true;
                        payReqt.PaymentData.sessionid = data.SessionId;
                        payReqt.PaymentData.userid = data.UserId;
                    }

                    CacheProvider.Store(CacheKeys.RAMMAS_TRANSACTION, new CacheItem<string>(conversationId));
                    payReqt.PaymentData.transactiontype = "RammasPayment";

                    if (string.IsNullOrEmpty(data.EasyPayNumber))
                    {
                        payReqt.PaymentData.easypayflag = string.Empty;
                        payReqt.ServiceType = ServiceType.RammasPayment;

                        goto SkipEasyPay;
                    }

                    var respEasyPay = DewaApiClient.GetEasyPayEnquiry(CurrentPrincipal.UserId, (data.ChannelId == "facebook" || data.ChannelId == "instagram") ? data.SessionId : CurrentPrincipal.SessionToken, data.EasyPayNumber, RequestLanguage, Request.Segment());

                    if (respEasyPay.Succeeded && respEasyPay.Payload != null && respEasyPay.Payload.@return != null)
                    {
                        DEWAXP.Foundation.Integration.DewaSvc.GetEasyPayEnquiryResponse _response;
                        _response = respEasyPay.Payload;

                        CacheProvider.Store(CacheKeys.Easy_Pay_LoggedIn, new CacheItem<bool>(IsLoggedIn));
                        CacheProvider.Store(CacheKeys.Easy_Pay_Response, new CacheItem<DEWAXP.Foundation.Integration.DewaSvc.GetEasyPayEnquiryResponse>(_response));
                    }

                    switch (data.TransactionType.ToUpper())
                    {
                        //case "BILL":
                        case "COLBILL":
                        case "BILL":
                            payReqt.IsThirdPartytransaction = true;
                            payReqt.ServiceType = ServiceType.RammasPayment;
                            payReqt.PaymentData.easypaynumber = data.EasyPayNumber;
                            payReqt.PaymentData.easypayflag = "X";

                            break;

                        case "REFBILL":
                            /*a = model.TotalAmount.ToString(),
                                c = _easy.@return.easypaynumber,
                                ThirdPartyPayment = false,
                                mto = "R",
                                EPayUrl = fullUrl,
                                type = "PayBill",
                                epnum = _easy.@return.easypaynumber,
                                epf = "X",
                                paymode =pay.paymode = PaymentContextExtensions.GetPaymentMode(data.model.paymentMethod);*/
                            payReqt.PaymentData.contractaccounts = data.EasyPayNumber;
                            payReqt.PaymentData.userid = CurrentPrincipal.UserId;
                            payReqt.IsThirdPartytransaction = false;
                            payReqt.PaymentData.movetoflag = "R";
                            payReqt.ServiceType = ServiceType.RammasPayment;
                            payReqt.PaymentData.easypaynumber = data.EasyPayNumber;
                            payReqt.PaymentData.easypayflag = "X";

                            break;

                        case "EVPAY":
                            payReqt.PaymentData.userid = CurrentPrincipal.UserId;
                            //payRequest.PaymentData.transactiontype= pay.PaymentContext.EPayTransactionCode(Request.Segment());
                            payReqt.IsThirdPartytransaction = true;
                            payReqt.PaymentData.movetoflag = "V";
                            payReqt.ServiceType = ServiceType.RammasPayment;
                            payReqt.PaymentData.easypaynumber = data.EasyPayNumber;
                            payReqt.PaymentData.easypayflag = "X";
                            break;

                        case "PSALE":
                            payReqt.PaymentData.businesspartner = data.BusinessPartnerNumber;
                            payReqt.PaymentData.userid = CurrentPrincipal.UserId;
                            payReqt.ServiceType = ServiceType.RammasPayment;
                            payReqt.PaymentData.easypayflag = "X";
                            break;

                        case "SDPAY":
                            payReqt.PaymentData.businesspartner = data.BusinessPartnerNumber;
                            payReqt.PaymentData.email = respEasyPay?.Payload?.@return?.email;
                            payReqt.PaymentData.userid = CurrentPrincipal.UserId;
                            payReqt.PaymentData.mobile = respEasyPay?.Payload?.@return?.mobile;
                            payReqt.IsThirdPartytransaction = true;
                            payReqt.PaymentData.easypaynumber = data.EasyPayNumber;
                            payReqt.ServiceType = ServiceType.ServiceActivation; // RammasPayment;
                            payReqt.PaymentData.easypayflag = "X";
                            break;

                        case "ESTMNM":
                        case "KWP2":
                            CacheProvider.Store(CacheKeys.Easy_Pay_Estimate, new CacheItem<decimal>(Convert.ToDecimal(data.Amount)));
                            payReqt.IsThirdPartytransaction = true;
                            payReqt.PaymentData.userid = CurrentPrincipal.UserId;
                            payReqt.PaymentData.estimatenumber = data.EasyPayNumber;
                            payReqt.PaymentData.businesspartner = data.BusinessPartnerNumber;
                            payReqt.PaymentData.ownerbusinesspartnernumber = data.OwnerPartnerNumber;
                            payReqt.PaymentData.consultantbusinesspartnernumber = data.ConsultantpartnerNumber;
                            payReqt.PaymentData.email = respEasyPay?.Payload?.@return?.email;
                            payReqt.PaymentData.easypaynumber = data.EasyPayNumber;
                            payReqt.ServiceType = ServiceType.EstimatePayment; // RammasPayment;
                            payReqt.PaymentData.easypayflag = "X";
                            break;

                        case "MSLPAY":
                            if (string.IsNullOrEmpty(payReqt.PaymentData.contractaccounts))
                            {
                                payReqt.PaymentData.contractaccounts = data.EasyPayNumber;
                            }
                            CacheProvider.Store(CacheKeys.Easy_Pay_Estimate, new CacheItem<decimal>(Convert.ToDecimal(data.Amount)));
                            payReqt.PaymentData.businesspartner = data.BusinessPartnerNumber;
                            payReqt.PaymentData.easypayflag = "X";
                            payReqt.IsThirdPartytransaction = false;
                            payReqt.PaymentData.email = respEasyPay?.Payload?.@return?.email;
                            payReqt.PaymentData.userid = CurrentPrincipal.UserId;
                            payReqt.PaymentData.mobile = respEasyPay?.Payload?.@return?.mobile;
                            payReqt.ServiceType = ServiceType.Miscellaneous; //.RammasPayment;
                            payReqt.PaymentData.easypaynumber = data.EasyPayNumber;
                            payReqt.PaymentData.notificationnumber = data.EasyPayNumber;
                            break;

                        default:
                            payReqt.IsThirdPartytransaction = true;
                            payReqt.ServiceType = ServiceType.RammasPayment;
                            payReqt.PaymentData.easypaynumber = data.EasyPayNumber;
                            payReqt.PaymentData.easypayflag = "X";
                            break;
                    }

                SkipEasyPay:
                    Log.Info("Rammas Payment DNS:" + Request.Url.DnsSafeHost, this);

                        //payRequest.ServiceType = ServiceType.PayBill;
                        //payRequest.PaymentMethod = model.paymentMethod;
                        //payRequest.IsThirdPartytransaction = false;
                        payReqt.SuqiaValue = data.SuqiaValue;
                        payReqt.SuqiaAmt = data.Suqiaamt;
                        var payResponse = ExecutePaymentGateway(payReqt, isRammasFacebookPayment);
                        if (Convert.ToInt32(payResponse.ErrorMessages?.Count) > 0)
                        {
                            foreach (KeyValuePair<string, string> errorItem in payResponse.ErrorMessages)
                            {
                                ModelState.AddModelError(errorItem.Key, errorItem.Value);
                            }
                            return Redirect(Request.UrlReferrer.AbsoluteUri);
                        }

                    return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                }

                #endregion [MIM Payment Implementation]

                
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                ModelState.AddModelError("", ErrorMessages.UNEXPECTED_ERROR);
            }
        JumpHere:
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "BotStateInvalid");
            //return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        //[HttpPost]
        //public ActionResult ChatEmail(RammasExportChat model)
        //{
        //    var responseData = new
        //    {
        //        success = true,
        //        Code = "000",
        //        decodeChat = HttpUtility.UrlDecode(model.Chat),
        //        encodeChat = model.Chat,
        //        captcha = model.captcha,
        //    };
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            bool status = false;

        //            if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(model.captcha))
        //            {
        //                status = ReCaptchaHelper.RecaptchaResponse(model.captcha);
        //            }
        //            //else if (this.IsCaptchaValid("Captcha is not valid"))
        //            //{
        //            //    status = true;
        //            //}
        //            if (status)
        //            {
        //                var emailClient = DependencyResolver.Current.GetService<DEWAXP.Foundation.Integration.IEmailServiceClient>();// EmailServiceClient;
        //                SitecoreX.Data.Items.Item rammasItem = SitecoreX.Context.Database.GetItem(SitecoreX.Data.ID.Parse("{26DCE97E-5B85-4145-BB03-7997F2430F9A}"));
        //                var exportStatus = Task.Run(() => ExportChat(model.EmailAddress, model.Subject, responseData.decodeChat));
        //                exportStatus.Wait();
        //                if (exportStatus.Result == true)
        //                {
        //                    return Json(responseData, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    return Json(new { success = false, Code = "001", responseData.decodeChat, responseData.encodeChat, responseData.captcha }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                return Json(new { success = false, Code = "002", responseData.decodeChat, responseData.encodeChat, responseData.captcha }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        catch (System.Exception ex)
        //        {
        //            LogService.Error(ex, this);
        //            return Json(new { success = false, Code = "003", responseData.decodeChat, responseData.encodeChat, responseData.captcha }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = false, Code = "004", responseData.decodeChat, responseData.encodeChat, responseData.captcha }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //private bool ExportChat(string emailAddress, string subject, string chat)
        //{
        //    try
        //    {
        //        SitecoreX.Data.Items.Item rammasItem = SitecoreX.Context.Database.GetItem(SitecoreX.Data.ID.Parse("{26DCE97E-5B85-4145-BB03-7997F2430F9A}"));

        //        var from = rammasItem?.Fields["From Email"]?.Value ?? "no-reply@dewa.gov.ae";
        //        var fileName = (rammasItem?.Fields["Attachment Name"]?.Value ?? "RammasChat-") + DateTime.Now.ToFileTime() + ".pdf";
        //        string body = rammasItem?.Fields["Export Email Body"]?.Value ?? "[Content Item Missing]";

        //        if (SitecoreX.Context.Language.CultureInfo.TextInfo.IsRightToLeft) { body = string.Format("<div style=\"direction:rtl\">{0}</div>", body); }

        //        //StringReader sr = new StringReader(chat.ToString());

        //        IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();

        //        //PdfDocument pdf = PdfGenerator.GeneratePdf(chat, PageSize.A4);
        //        //pdf.Save("document.pdf");
        //        //MemoryStream stream = new MemoryStream();

        //        //pdf.Save(stream, false);
        //        byte[] bytes = Renderer.RenderHtmlAsPdf(chat).Stream.ToArray();
        //        //stream.Close();
        //        System.Collections.Generic.List<Tuple<string, byte[]>> attList = new System.Collections.Generic.List<Tuple<string, byte[]>>(); attList.Add(new Tuple<string, byte[]>(fileName, bytes));
        //        var response = EmailServiceClient.SendEmail(from, emailAddress, "", "", subject, body, attList);
        //        if (response.Succeeded)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogService.Error(ex, this);
        //    }
        //    return false;
        //}

        #region IsCheckedlogin

        [HttpGet]
        public string IsCheckedlogin(string conversationId)
        {
            try
            {
                int magicNumber = RammasParameter.GenerateRandomNumber();
                Session["MagicNumber"] = magicNumber;

                string languageCode = RequestLanguage.ToString();
                if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
                {
                    string rammassessionToken, error;
                    if (languageCode != null && languageCode == "English")
                        languageCode = "en-US";
                    else
                        languageCode = "ar-AE";
                    if (TryLoginWithSmartCustAuthentication(CurrentPrincipal.Username, CurrentPrincipal.SessionToken, out rammassessionToken))
                    {
                        //After login success please set session Id
                        if (RammasParameter.PostLoginCredentialsToBot(CurrentPrincipal.Username, rammassessionToken, string.Empty, conversationId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, languageCode, out error, magicNumber, "authentication"))
                        {
                            return "Success";
                        }
                        else
                        {
                            return "Faild";
                        }
                    }
                }
                return "Success";
            }
            catch (System.Exception ex)
            {
                Log.Info(ex.Message + "Login checked -", this);
                return null;
            }
        }

        #endregion IsCheckedlogin
    }
}