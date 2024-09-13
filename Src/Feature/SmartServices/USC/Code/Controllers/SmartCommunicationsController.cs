// <copyright file="SmartCommunicationsController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>
using Sitecore.Globalization;

namespace DEWAXP.Feature.USC.Controllers
{
    using DEWAXP.Feature.USC.Models.SmartCommunications;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Content.Services;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartCommunication;
    using DEWAXP.Foundation.Integration.Requests.QmsSvc;
    using DEWAXP.Foundation.Logger;
    using global::Sitecore.Data.Items;
    using global::Sitecore.Mvc.Presentation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Sitecorex = global::Sitecore;

    /// <summary>
    /// Defines the <see cref="SmartCommunicationsController" />.
    /// </summary>
    public class SmartCommunicationsController : BaseController
    {
        /// <summary>
        /// The CommunicationComponent.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult CommunicationComponent(string b = "")
        {
            bool _returnUrl = false;
            CacheProvider.TryGet(CacheKeys.UAEPASS_USC_returnUrl, out _returnUrl);
            string datasource = RenderingContext.CurrentOrNull.Rendering?.DataSource;
            Models.SmartCommunications.SmartCommunicationFolder header = null;
            try
            {
                if (IsLoggedIn && !_returnUrl)
                {
                    ClearSessionAndSignOut();
                }

                if (!RenderingRepository.HasDataSource || string.IsNullOrWhiteSpace(b))
                {
                    if (!_returnUrl)
                        return new EmptyResult();
                }
                if (QmsServiceClient != null)
                {
                    var responseBranchList = QmsServiceClient.GetBranchServiceStatusList(new BranchServiceStatusReq(), RequestLanguage, Request.Segment());
                    if (responseBranchList != null && responseBranchList.Payload != null && responseBranchList.Succeeded
                            && responseBranchList.Payload.BranchStatusList != null
                            && responseBranchList.Payload.BranchStatusList.BranchStatus != null)
                    {
                        if (responseBranchList.Payload.BranchStatusList.BranchStatus.Where(x => x.Branch.IndBranch.BranchCode == b).Count() > 0)
                        {
                            var branchCode = !string.IsNullOrWhiteSpace(b) ? responseBranchList.Payload.BranchStatusList.BranchStatus.Where(x => x.Branch.IndBranch.BranchCode == b).Select(y => y.Branch.IndBranch.BranchCode.ToString()).First() : string.Empty;
                            if (string.IsNullOrWhiteSpace(branchCode))
                                return new EmptyResult();

                            header = RenderingRepository.GetDataSourceItem<SmartCommunicationFolder>();
                            CacheProvider.Remove(CacheKeys.UAEPASS_USC_returnUrl);
                            return View("~/Views/Feature/USC/SmartCommunications/CommunicationComponent.cshtml",header);
                        }
                        else
                        {
                            return new EmptyResult();
                        }
                    }
                }
                else
                {
                    header = RenderingRepository.GetDataSourceItem<SmartCommunicationFolder>();
                    CacheProvider.Remove(CacheKeys.UAEPASS_USC_returnUrl);
                    return View("~/Views/Feature/USC/SmartCommunications/CommunicationComponent.cshtml",header);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            header = RenderingRepository.GetDataSourceItem<SmartCommunicationFolder>();
            CacheProvider.Remove(CacheKeys.UAEPASS_USC_returnUrl);
            return View("~/Views/Feature/USC/SmartCommunications/CommunicationComponent.cshtml",header);
        }

        #region Consumption Verification

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumptVerifyNxtSubmitForm(ConsumptionVerification consumptionVerification)
        {
            string errNxt = "";
            try
            {
                //send otp
                var response = SmartCommunicationClient.VerifyMobileOtp(new SmartCommunicationVerifyOtpRequest()
                {
                    mode = "R",
                    sessionid = string.Empty,
                    reference = consumptionVerification.AccountNumber,
                    prtype = "UCP",
                    mobile = string.Empty,
                    email = string.Empty,
                    contractaccountnumber = string.Empty,
                    businesspartner = string.Empty,
                    otp = string.Empty
                }, Request.Segment(), RequestLanguage);
                if (response != null && response.Succeeded)
                {
                    CacheProvider.Store(CacheKeys.USC_CONSUMPT_VERIFYOTP, new CacheItem<SmartCommunicationVerifyOtpResponse>(response.Payload, TimeSpan.FromMinutes(20)));
                    SmartCommunicationVerifyOtpResponse _responseData = new SmartCommunicationVerifyOtpResponse();
                    string msg = string.Empty;
                    if (response.Payload.mobilelist != null)
                    {
                        _responseData.mobilelist = new List<SmartCommunicationMobileResponse>();
                        _responseData.mobilelist.Add(new SmartCommunicationMobileResponse
                        {
                            maskedmobile = response.Payload.mobilelist.FirstOrDefault().maskedmobile ?? null
                        });
                    }

                    if (response.Payload.emaillist != null)
                    {
                        _responseData.emaillist = new List<SmartCommunicationEmailResponse>();
                        _responseData.emaillist.Add(new SmartCommunicationEmailResponse
                        {
                            maskedemail = response.Payload.emaillist.FirstOrDefault().maskedemail ?? null
                        });
                    }
                    return Json(new { status = true, desc = response.Message, data = _responseData }, JsonRequestBehavior.AllowGet);

                    //else
                    //{
                    //    return Json(new { status = false, desc = response.Message }, JsonRequestBehavior.AllowGet);
                    //}
                    //return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errNxt = response.Message;
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                errNxt = ex.Message;
            }
            return Json(new { status = false, desc = errNxt }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumptSendOtpSubmitForm(ConsumptionVerification consumptionVerification)
        {
            string errVerf = "";
            try
            {
                SmartCommunicationVerifyOtpResponse _cacheResponse;
                if (!string.IsNullOrWhiteSpace(consumptionVerification.SelectedOptions))
                {
                    //send otp
                    if (CacheProvider.TryGet(CacheKeys.USC_CONSUMPT_VERIFYOTP, out _cacheResponse))
                    {
                        //send otp
                        var response = SmartCommunicationClient.VerifyMobileOtp(new SmartCommunicationVerifyOtpRequest()
                        {
                            mode = "S",
                            sessionid = string.Empty,
                            reference = consumptionVerification.AccountNumber,
                            prtype = "UCP",
                            email = consumptionVerification.SelectedOptions.Equals("email") ? _cacheResponse.emaillist[0].unmaskedemail : string.Empty,
                            mobile = consumptionVerification.SelectedOptions.Equals("mobile") ? _cacheResponse.mobilelist[0].unmaskedmobile : string.Empty,
                            contractaccountnumber = consumptionVerification.AccountNumber,
                            businesspartner = string.Empty,
                            otp = string.Empty
                        }, Request.Segment(), RequestLanguage);
                        if (response != null && response.Succeeded)
                        {
                            return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            errVerf = response.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = ex.Message;
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumptSessionLogin(ConsumptionVerification consumptionVerification)
        {
            string errMsg = "";
            string maxAttempt = "";
            try
            {
                SmartCommunicationVerifyOtpResponse _cacheResponse;
                if (CacheProvider.TryGet(CacheKeys.USC_CONSUMPT_VERIFYOTP, out _cacheResponse))
                {
                    if (consumptionVerification.OTPKey.All(char.IsDigit))
                    {
                        var response = SmartCommunicationClient.SessionLogin(new SessionLoginRequest
                        {
                            sessionparams = new SessionParams
                            {
                                center = "X",
                                email = consumptionVerification.SelectedOptions.Equals("email") ? _cacheResponse.emaillist[0].unmaskedemail : string.Empty,
                                mobile = consumptionVerification.SelectedOptions.Equals("mobile") ? _cacheResponse.mobilelist[0].unmaskedmobile : string.Empty,
                                logintype = "UCP",
                                otpkey = consumptionVerification.OTPKey,
                                userid = consumptionVerification.AccountNumber,
                                referencenumber = consumptionVerification.AccountNumber,
                                xcoordinate = "10.10",
                                ycoordinate = "11.11",
                            }
                        }, Request.Segment(), RequestLanguage);
                        if (response != null && response.Succeeded)
                        {
                            AuthStateService.Save(new DewaProfile("UCP" + consumptionVerification.AccountNumber, response.Payload.sessionid)
                            {
                                BusinessPartner = response.Payload.businesspartnernumber,
                                PrimaryAccount = consumptionVerification.AccountNumber,
                                EmailAddress = consumptionVerification.Email,
                                FullName = response.Payload.fullname,
                                MobileNumber = consumptionVerification.Mobile,
                                TermsAndConditions = "X",
                                HasActiveAccounts = true,
                                IsContactUpdated = true,
                                PopupFlag = false,
                                IsUSC = true
                            });
                            return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            errMsg = response.Message;
                            if (response.Payload != null)
                                maxAttempt = response.Payload.maxattempt;
                        }
                    }
                    else
                    {
                        errMsg = Translate.Text("Sc.Invalid OTP");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                errMsg = ex.Message;
            }

            return Json(new { status = false, desc = errMsg, maxattempt = maxAttempt }, JsonRequestBehavior.AllowGet);
        }

        private void ClearSessionAndSignOut()
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

        #endregion Consumption Verification

        #region Phase-1

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumerInquiryNxtSubmitForm(ConsumerInquiryForm consumerInquiryForm)
        {
            string errNxt = "";
            try
            {
                if (consumerInquiryForm.Type.Equals(1))
                {
                    var validAccount = SmartCommunicationClient.VerifyMobileOtp(new SmartCommunicationVerifyOtpRequest()
                    {
                        mode = "C",
                        sessionid = string.Empty,
                        reference = consumerInquiryForm.AccountNumber,
                        prtype = "UCP",
                        mobile = string.Empty,
                        email = string.Empty,
                        contractaccountnumber = string.Empty,
                        businesspartner = string.Empty,
                        otp = string.Empty
                    }, Request.Segment(), RequestLanguage);
                    if (validAccount != null && validAccount.Succeeded)
                    {
                        //send otp
                        var response = SmartCommunicationClient.VerifyMobileOtp(new SmartCommunicationVerifyOtpRequest()
                        {
                            mode = "R",
                            sessionid = string.Empty,
                            reference = consumerInquiryForm.AccountNumber,
                            prtype = "UCP",
                            mobile = string.Empty,
                            email = string.Empty,
                            contractaccountnumber = string.Empty,
                            businesspartner = string.Empty,
                            otp = string.Empty
                        }, Request.Segment(), RequestLanguage);
                        if (response != null && response.Succeeded)
                        {
                            CacheProvider.Store(CacheKeys.USC_VERIFYOTP, new CacheItem<SmartCommunicationVerifyOtpResponse>(response.Payload, TimeSpan.FromMinutes(20)));
                            string _responseData = string.Empty;
                            string msg = string.Empty;
                            if (response.Payload.mobilelist != null)
                            {
                                _responseData = response.Payload.mobilelist.FirstOrDefault().maskedmobile ?? null;
                                return Json(new { status = true, desc = response.Message, data = _responseData }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { status = false, desc = "" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            errNxt = response.Message;
                        }
                    }
                    else
                    {
                        errNxt = !string.IsNullOrWhiteSpace(validAccount.Message) ? validAccount.Message : Translate.Text("MBAccountNumberErrorMsg");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                errNxt = ex.Message;
            }
            return Json(new { status = false, desc = errNxt }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumerInquiryVerifySubmitForm(ConsumerInquiryForm consumerInquiryForm)
        {
            string errVerf = "";
            try
            {
                SmartCommunicationVerifyOtpResponse response;
                if (consumerInquiryForm.Type.Equals(1))
                {
                    //send otp
                    if (CacheProvider.TryGet(CacheKeys.USC_VERIFYOTP, out response))
                    {
                        var mobile = response.mobilelist?.FirstOrDefault() ?? null;
                        if (consumerInquiryForm.vMobile.Equals(mobile.unmaskedmobile))
                        {
                            var unmaskedmobile = response.mobilelist != null ? response.mobilelist.FirstOrDefault().unmaskedmobile.RemoveMobileNumberZeroPrefix() : null;
                            var unmaskedemail = response.emaillist != null ? response.emaillist.FirstOrDefault().unmaskedemail : null;

                            return Json(new { status = true, desc = response.description, mobile = unmaskedmobile, email = unmaskedemail }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            errVerf = Translate.Text("TRA_MobileValidation");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = ex.Message;
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);
        }

        #endregion Phase-1

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumerInquiryForm(ConsumerInquiryForm consumerInquiryForm)
        {
            string errSub = "";
            var regexItem = new Regex("^[أ-يa-zA-Z ]*$");
            MatchCollection scriptBlocks = consumerInquiryForm.InquiryDetails != null ? Regex.Matches(consumerInquiryForm.InquiryDetails, "<*.*?<" + "*/>", RegexOptions.IgnoreCase | RegexOptions.Singleline) : null;

            try
            {
                bool status = false;
                string recaptchaResponse = Convert.ToString(consumerInquiryForm.recaptcha ?? "");

                if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
                {
                    status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
                }
                //else if (this.IsCaptchaValid("Captcha is not valid"))
                //{
                //    status = true;
                //}
                if (!status)
                {
                    errSub = Translate.Text("unsubscribe-Captcha-Not-Valid");
                }

                SmartCommunicationSettings settings = ContentRepository.GetItem<SmartCommunicationSettings>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.SMARTCOMMUNICATIONS_CONFIG)));
                if (consumerInquiryForm != null && status)
                {
                    if (scriptBlocks != null && scriptBlocks.Count == 0)
                    {
                        if (settings != null && consumerInquiryForm != null)
                        {
                            if (consumerInquiryForm.Type.Equals(1))
                            {
                                // submit action
                                var response = SmartCommunicationClient.SmartCommunicationSubmit(new SmartCommunicationRequest
                                {
                                    businesspartnernumber = string.Empty,
                                    contractaccountnumber = consumerInquiryForm.AccountNumber ?? string.Empty,
                                    mobilenumber = consumerInquiryForm.Mobile.AddMobileNumberZeroPrefix() ?? string.Empty,
                                    premisenumber = string.Empty,
                                    remarks = consumerInquiryForm.InquiryDetails ?? string.Empty,
                                    requesttype = consumerInquiryForm.InquiryType ?? string.Empty,
                                    userid = string.Empty
                                }, Request.Segment(), RequestLanguage);

                                if (response != null && response.Succeeded && response.Payload != null)
                                {
                                    return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    errSub = response.Message;
                                }
                            }
                            if (consumerInquiryForm.Type.Equals(2))
                            {
                                if (regexItem.IsMatch(consumerInquiryForm.Name))
                                {
                                    var builder = SendMail(new SendMailRequest
                                    {
                                        from = settings.FromEmail,
                                        to = settings.Builder_Inquiry,
                                        subject = settings.Inquiry_Form_Subject,
                                        bcc = settings.Builder_Inquiry,
                                        cc = settings.Builder_Inquiry,
                                        body = ConsumerEmailbody(settings.Consumer_Template, consumerInquiryForm)
                                    });
                                    if (builder)
                                    {
                                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    errSub = Translate.Text("Sc.Requester Name Error message");
                                }
                            }
                            if (consumerInquiryForm.Type.Equals(3))
                            {
                                if (regexItem.IsMatch(consumerInquiryForm.Name))
                                {
                                    string toEmail = settings.Visitor_Inquiry;
                                    List<SelectListItem> GeneralInquiryEmailList = GetLstDataSource(DataSources.SC_General_Information_Email).ToList();

                                    if (GeneralInquiryEmailList != null && GeneralInquiryEmailList.Count > 0)
                                    {
                                        foreach (var item in GeneralInquiryEmailList)
                                        {
                                            if (item.Text == consumerInquiryForm.SubType.ToString())
                                            {
                                                toEmail = item.Value;
                                                break;
                                            }
                                        }
                                    }
                                    var visitor = SendMail(new SendMailRequest
                                    {
                                        from = settings.FromEmail,
                                        to = toEmail, //settings.Visitor_Inquiry,
                                        subject = settings.Inquiry_Form_Subject,
                                        bcc = settings.Visitor_Inquiry,
                                        cc = settings.Visitor_Inquiry,
                                        body = VisitorEmailbody(settings.General_Inquiry_Template, consumerInquiryForm)
                                    });
                                    if (visitor)
                                    {
                                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    errSub = Translate.Text("Sc.Requester Name Error message");
                                }
                            }
                        }
                    }
                    else
                    {
                        errSub = Translate.Text("Sc.Inquiry Details Error Message");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                errSub = ex.Message;
            }
            return Json(new { status = false, desc = errSub }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BuilderTDInquiryForm(BuilderTDInquiryForm consumerInquiryForm)
        {
            string errSub = "";
            try
            {
                SmartCommunicationSettings settings = ContentRepository.GetItem<SmartCommunicationSettings>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.SMARTCOMMUNICATIONS_CONFIG)));
                bool status = false;
                string recaptchaResponse = Convert.ToString(consumerInquiryForm.recaptcha ?? "");

                if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
                {
                    status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
                }
                //else if (this.IsCaptchaValid("Captcha is not valid"))
                //{
                //    status = true;
                //}
                if (!status)
                {
                    errSub = Translate.Text("unsubscribe-Captcha-Not-Valid");
                }
                var regexItem = new Regex("^[أ-يa-zA-Z ]*$");
                MatchCollection scriptBlocks = Regex.Matches(consumerInquiryForm.InquiryDetails, "<*.*?<" + "*/>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (settings != null && consumerInquiryForm != null && status)
                {
                    if (regexItem.IsMatch(consumerInquiryForm.Name))
                    {
                        if (scriptBlocks.Count == 0)
                        {
                            var to = settings.Builder_Inquiry;

                            List<SelectListItem> DiscussionareaEmailList = GetLstDataSource(DataSources.SC_Discussion_Area_Email).ToList();
                            if (DiscussionareaEmailList.Count > 0 && DiscussionareaEmailList != null)
                            {
                                foreach (var item in DiscussionareaEmailList)
                                {
                                    if (item.Text == consumerInquiryForm.BuilderSubType)
                                    {
                                        to = item.Value;
                                        break;
                                    }
                                }
                            }
                            List<SelectListItem> NOCCategoryEmailList = GetLstDataSource(DataSources.SC_NOC_Category_Email).ToList();
                            if (NOCCategoryEmailList != null && NOCCategoryEmailList.Count > 0)
                            {
                                foreach (var item in NOCCategoryEmailList)
                                {
                                    if (item.Text == consumerInquiryForm.NOCCategory)
                                    {
                                        to = string.Join(";", to, item.Value);
                                        break;
                                    }
                                }
                            }
                            //switch (consumerInquiryForm.BuilderSubType)
                            //{
                            //    case "1": to = settings.Electricity_Service_Email; break;
                            //    case "2": to = settings.Water_Service_Email; break;
                            //    case "3": to = settings.Infrastructure_NOC_Email; break;
                            //    default: to = settings.Builder_Inquiry; break;
                            //}
                            if (!string.IsNullOrWhiteSpace(to))
                            {
                                var builder = SendMail(new SendMailRequest
                                {
                                    from = settings.FromEmail,
                                    to = to,
                                    subject = settings.Inquiry_Form_Subject,
                                    bcc = to,
                                    cc = to,
                                    body = BuilderEmailbody(settings.Builder_Template, consumerInquiryForm)
                                });
                                if (builder)
                                {
                                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            errSub = Translate.Text("Sc.Inquiry Details Error Message");
                        }
                    }
                    else
                    {
                        errSub = Translate.Text("Sc.Requester Name Error message");
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                errSub = ex.Message;
            }
            return Json(new { status = false, desc = errSub }, JsonRequestBehavior.AllowGet);
        }

        #region [Schdeule Call]

        [HttpGet]
        public ActionResult ScheduleCall()
        {
            return View("~/Views/Feature/USC/SmartCommunications/ScheduleCall.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ScheduleCallSubmit(Models.SmartCommunications.ScheduleCallSubmitModel model)
        {
            string desc = "";
            bool success = false;
            try
            {
                SmartCommunicationSettings settings = ContentRepository.GetItem<SmartCommunicationSettings>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.SMARTCOMMUNICATIONS_CONFIG)));
                if (settings != null && model != null)
                {
                    var to = settings.Schedule_a_call_Email;
                    if (!string.IsNullOrWhiteSpace(to))
                    {
                        success = SendMail(new SendMailRequest
                        {
                            from = settings.FromEmail,
                            to = to,
                            subject = settings.Inquiry_Form_Subject,
                            bcc = to,
                            cc = to,
                            body = ScheduleCallBody(settings.Schedule_a_call_Template, model)
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                success = false;
                desc = ex.Message;
            }
            return Json(new { Success = success, Messages = desc });
        }

        #endregion [Schdeule Call]

        #region [GenerateVideoCallToken]

        [HttpGet]
        public ActionResult VideoCallToken(string v = "", string b = "", string s = "")
        {
            string branchcode = string.Empty;
            string ticketNumber = string.Empty;
            string serviceNo = string.Empty;
            string waitingToken = string.Empty;
            var model = new Models.SmartCommunications.VideoCallTokenRequest();
            if (!string.IsNullOrWhiteSpace(b) && !string.IsNullOrWhiteSpace(s))
            {
                branchcode = b;
                serviceNo = s;
                if (QmsServiceClient != null)
                {
                    var responseWaitingTicket = QmsServiceClient.GetServiceStatusList(new ServiceStatusListReq
                    {
                        BranchCode = branchcode
                    }, RequestLanguage, Request.Segment());

                    var response = QmsServiceClient.IssueTicket(new IssueTicketReq
                    {
                        BranchCode = branchcode,
                        Cust = new Cust
                        {
                            IndCust = new IndCust
                            {
                                SyncId = System.Guid.NewGuid().ToString()
                            }
                        },
                        IndService = new IndService
                        {
                            ServiceNo = serviceNo
                        }
                    }, RequestLanguage, Request.Segment());

                    if (response != null && response.Payload != null && response.Succeeded && response.Payload.TicketWaiting != null && response.Payload.TicketWaiting.TicketNo != null)
                    {
                        model.NextToken = response.Payload.TicketWaiting.TicketNo;
                        model.Status = true;
                        model.WaitingToken = "0";
                        if (responseWaitingTicket != null && responseWaitingTicket.Payload != null && response.Succeeded && responseWaitingTicket.Payload.ServiceStatusList != null
                        && responseWaitingTicket.Payload.ServiceStatusList.ServiceStatus != null)
                        {
                            if (responseWaitingTicket.Payload.ServiceStatusList.ServiceStatus.Where(x => x.Service.IndService.ServiceNo == serviceNo).ToList().Count > 0)
                            {
                                model.WaitingToken = responseWaitingTicket.Payload.ServiceStatusList.ServiceStatus.Where(x => x.Service.IndService.ServiceNo == serviceNo).Select(y => y.TotalTicketWaiting).First();
                                model.WaitingTokenTxt = Translate.Text("scomWaitingToken").Replace("{{waitingToken}}", model.WaitingToken);
                                model.Status = true;
                            }
                        }
                        else
                        {
                            model.WaitingTokenTxt = responseWaitingTicket.Message;
                            model.Status = false;
                        }
                    }
                    else
                    {
                        if (response != null && response.Payload != null && response.Payload.Error != null && response.Payload.Error.ErrorCode != null)
                            model.ErrorCode = response.Payload.Error.ErrorCode;
                        model.NextToken = response.Message;
                        model.Status = false;
                    }
                }
            }
            else
            {
                ticketNumber = GetRandomNo(9999);
            }

            //var model = new Models.SmartCommunications.VideoCallTokenRequest()
            //{
            //    NextToken = ticketNumber,
            //    WaitingTokenTxt = WaitingTokenTxt
            //};
            if (!string.IsNullOrWhiteSpace(v))
            {
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            return View("~/Views/Feature/USC/SmartCommunications/VideoCallToken.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VideoCallTokenSubmit(Models.SmartCommunications.VideoCallTokenRequest model)
        {
            string desc = "";
            bool success = false;
            try
            {
                string _mobileNO = Convert.ToInt32(model.CustomerMobileNo).ToString("0000000000");
                string _body = Translate.Text("SCOM_SMSBody")?.Replace("{token}", model.NextToken)?.Replace("{waitingtoken}", model.WaitingToken);
                success = SendSMS(_mobileNO, _body);
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                success = false;
                desc = ex.Message;
            }
            return Json(new { Success = success, Messages = desc });
        }

        #endregion [GenerateVideoCallToken]


        [HttpGet]
        public ActionResult LogOut()
        {
            string redirectURI = string.Empty;
            string UniqueURL = string.Empty;
            if (IsLoggedIn)
            {
                bool myid = CurrentPrincipal.IsMyIdUser;
                string username = CurrentPrincipal.Username, samlSessionIndex = myid ? (Session["SamlSessionIndex"] != null ? Session["SamlSessionIndex"].ToString() : string.Empty) : string.Empty;
                DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
                FormsAuthentication.SignOut();
                Session.Abandon();
                Session.Clear();

                if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
                {
                    Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                    Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
                }

                if (CurrentPrincipal.IsMyIdUser)
                {
                    if (!string.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["UAEPASS_Logout"]))
                    {
                        if (Uri.IsWellFormedUriString(System.Web.Configuration.WebConfigurationManager.AppSettings["UAEPASS_Logout"].ToString(), UriKind.Absolute))
                        {
                            redirectURI = RemoveQueryStringByKey(System.Web.Configuration.WebConfigurationManager.AppSettings["UAEPASS_Logout"].ToString(), "redirect_uri");
                            Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.UNIVERSAL_SERVICE_CENTRE));
                            UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always });
                            UniqueURL = UniqueURL.Replace(":443", "");
                            redirectURI += "?redirect_uri=" + UniqueURL + "";
                        }
                    }
                }
            }
            return Json(new { Success = true, RedirectUrl = redirectURI }, JsonRequestBehavior.AllowGet);
        }

        #region [Function]

        public string RemoveQueryStringByKey(string url, string key)
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
                : pagePathWithoutQueryString;
        }

        private string ConsumerEmailbody(string body, ConsumerInquiryForm consumerInquiryForm)
        {
            body = body.Replace("{DewaAccPreNum}", consumerInquiryForm.AccountNumber);
            body = body.Replace("{Name}", consumerInquiryForm.Name);
            body = body.Replace("{Email}", consumerInquiryForm.EmailId);
            body = body.Replace("{Mobile}", consumerInquiryForm.Mobile);
            body = body.Replace("{InquiryType}", consumerInquiryForm.InquiryTypetxt);
            body = body.Replace("{InquiryDetails}", consumerInquiryForm.InquiryDetails);
            return body;
        }

        private string BuilderEmailbody(string body, BuilderTDInquiryForm builderTDInquiryForm)
        {
            body = body.Replace("{Email}", builderTDInquiryForm.EmailId);
            body = body.Replace("{Name}", builderTDInquiryForm.Name);
            body = body.Replace("{Mobile}", builderTDInquiryForm.Mobile);
            //body = body.Replace("{Channel}", builderTDInquiryForm.Channel);
            body = body.Replace("{DiscussionArea}", builderTDInquiryForm.Discussionarea);
            if (builderTDInquiryForm.BuilderSubType == "3")
            {
                body = body.Replace("{NOCCategory}", builderTDInquiryForm.NOCCategorytxt);
            }
            else
            {
                body = body.Replace("{NOCCategory}", Translate.Text("Sc.Not Applicable"));
            }
            body = body.Replace("{InquiryDetails}", builderTDInquiryForm.InquiryDetails);
            return body;
        }

        private string VisitorEmailbody(string body, ConsumerInquiryForm consumerInquiryForm)
        {
            body = body.Replace("{Name}", consumerInquiryForm.Name);
            body = body.Replace("{Email}", consumerInquiryForm.EmailId);
            body = body.Replace("{Mobile}", consumerInquiryForm.Mobile);
            body = body.Replace("{InquiryDetails}", consumerInquiryForm.InquiryDetails);
            body = body.Replace("{InquiryType}", consumerInquiryForm.Generalinquirytype);
            return body;
        }

        private string ScheduleCallBody(string body, ScheduleCallSubmitModel model)
        {
            body = body.Replace("{name}", model.ShCallName);
            body = body.Replace("{date}", model.ShCallDate);
            body = body.Replace("{time}", model.ShCallTime);
            body = body.Replace("{mobile}", model.ShCallMobileNO);
            body = body.Replace("{detail}", model.ShCallDescription);
            body = body.Replace("{emailaddress}", model.ShCallEmailAddress);
            return body;
        }

        private bool SendSMS(string mobileNo, string body)
        {
            bool success = false;
            if (Translate.Text("lang") == "ar")
            {
                success = SmsServiceClient.Send_DEWA_SMSAr(mobileNo, body, Translate.Text("SCOM_AppName"), Translate.Text("SCOM_SenderName"), "1").Payload;
            }
            else
            {
                success = SmsServiceClient.Send_DEWA_SMS(mobileNo, body, Translate.Text("SCOM_AppName"), Translate.Text("SCOM_SenderName"), "1").Payload;
            }
            return success;
        }

        private bool SendMail(SendMailRequest sendMailRequest)
        {
            try
            {
                if (sendMailRequest.fileattachment)
                {
                    if (!string.IsNullOrEmpty(sendMailRequest.cc) ||
                        !string.IsNullOrEmpty(sendMailRequest.bcc) ||
                        (!string.IsNullOrEmpty(sendMailRequest.filename) && sendMailRequest.filebytearray != null && sendMailRequest.filebytearray.Length > 0))
                    {
                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>(); attList.Add(new Tuple<string, byte[]>(sendMailRequest.filename, sendMailRequest.filebytearray));
                        var response = this.EmailServiceClient.SendEmail(sendMailRequest.from, sendMailRequest.to, sendMailRequest.cc, sendMailRequest.bcc, sendMailRequest.subject, string.Format(sendMailRequest.body), attList);
                    }
                }
                else
                {
                    var response = this.EmailServiceClient.SendEmail(sendMailRequest.from, sendMailRequest.to, sendMailRequest.subject, sendMailRequest.body);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return false;
            }
            return true;
        }

        private string GetRandomNo(int seed)
        {
            System.Random r = new System.Random();
            return System.Convert.ToString(r.Next(1000, seed));
        }

        #endregion [Function]
    }

    #region [Helper class]

    public class SendMailRequest
    {
        public string from { get; set; }
        public string to { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string filename { get; set; }
        public byte[] filebytearray { get; set; }
        public bool fileattachment { get; set; }
    }

    #endregion [Helper class]
}