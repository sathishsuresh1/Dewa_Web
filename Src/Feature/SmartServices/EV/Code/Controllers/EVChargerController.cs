using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Feature.EV.Models.EVCharger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Context = Sitecore.Context;
using System.Web.Mvc;
using Sitecore.Data;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.Responses;
using System.IO;
using DEWAXP.Foundation.Integration.DewaSvc;
using SitecoreX = global::Sitecore.Context;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
using X.PagedList;
using DEWAXP.Feature.EV.Models.EVDashboard;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Helpers;
using Sitecore.Globalization;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Utils;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.MoveOut;
using DEWAXP.Foundation.Content.Models.MoveOut.v3;
using DEWAXP.Foundation.Content.Models.EVCharger;
using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;
//using DEWAXP.Feature.EV.Models.ConnectionEnquiries;

namespace DEWAXP.Feature.EV.Controllers
{
    public class EVChargerController : BaseServiceActivationController
    {
        private const string VIEW_Registration_NonDewaIndividual = "~/Views/Feature/EV/EVCharger/Registration_NonDewaIndividual.cshtml";
        private const string VIEW_Registration_NonDewaBusiness = "~/Views/Feature/EV/EVCharger/Registration_NonDewaBusiness.cshtml";
        //private const string VIEW_ApplyEVCard_Details = "ApplyEVCard_Details";

        #region [variable]
        private string _personalCType = Convert.ToString(((int)AccountType.Personal));
        private string _businessCType = Convert.ToString(((int)AccountType.Business));
        private string _governmentCType = Convert.ToString(((int)AccountType.Government));


        #endregion

        #region EV Services
        public ActionResult EVServices()
        {
            //ViewBag.EVServices = GetDataSource(SitecoreItemIdentifiers.EV_Services).OrderBy(x => x.Value);
            return View("~/Views/Feature/EV/EVCharger/EVServicesLanding.cshtml", new Services() { Step = 1, IsUserLoggedIn = IsLoggedIn });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EVServices(Services model)
        {
            if (model.Service == "1" && IsLoggedIn && model.Step == 1)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ApplyCardLandingPage);
            }
            /*if (model.Service == "1" && IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ApplyCardLandingPage);
            }*/
            if (model.Service == "1" && model.SelectedMethod == UserType.DewaCustomer)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ApplyCardLandingPage);
            }
            else if (model.Service == "1" && model.SelectedMethod == UserType.NoDewaAccount)
            {
                if (IsLoggedIn)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ApplyCardLandingPage);
                }
                else
                {
                    CacheProvider.Store(CacheKeys.RegistrationReturnURL, new CacheItem<string>(SitecoreItemIdentifiers.EV_IndividualApplyCard));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J9_CUSTOMER_REGISTRATION);
                }
            }
            else if (model.Service == "1" && model.SelectedMethod == UserType.NoDewaCustomer)
            {
                if (IsLoggedIn)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ApplyCardLandingPage);
                }
                else
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_NonDewaCustomerLandingpage);
                }
            }
            else if (model.Service == "2")  // Deactivate EV Card
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateLandingPage);
            }
            else if (model.Service == "3") // Replace EV Card
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ReplaceLandingPage);
            }
            else if (model.Service == "4")
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_EnquiryPage);
            }

            else if (model.Service == "5")
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_CardStatusPage);
            }
            //ViewBag.EVServices = GetDataSource(SitecoreItemIdentifiers.EV_Services);
            model.Step = 2;
            return View("~/Views/Feature/EV/EVCharger/EVServicesLanding.cshtml", model);
        }
        #endregion

        #region Actions - EV Enquiry
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpGet]
        public ActionResult Enquiry()
        {
            var model = PopulateQueryTypes();

            return PartialView("~/Views/Feature/EV/EVCharger/Enquiry.cshtml", model);
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Enquiry(EVEnquiry model)
        {
            if (model.Attachment != null)
            {
                string error;
                if (!AttachmentIsValid(model.Attachment, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                //var accounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken).Payload.FirstOrDefault(x => x.AccountNumber == model.ContractAccountNo);
                //var accounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment()).Payload.FirstOrDefault(x => x.AccountNumber == model.ContractAccountNo);

                //var account = Account.CreateFrom(accounts);

                //if (account != null)
                //{
                //    //model.PremiseNumber = account.Premise;
                //    model.BusinessPartnerNo = account.BusinessPartner;
                //    model.ContractAccountNo = account.AccountNumber;
                //}

                var request = new RequestConnectionEnquiry()
                {
                    BusinessPartnerNo = model.BusinessPartnerNo,
                    ContractAccountNo = model.ContractAccountNo,
                    MobileNo = model.MobileNumber,
                    PremiseNo = model.CardNumber,
                    //todo send EV card number instead

                    QueryType = model.SelectedQueryType,
                    Remarks = model.Details,
                    Attachment = model.Attachment != null ? model.Attachment.ToArray() : new byte[0],
                    AttachmentType = model.Attachment != null ? model.Attachment.GetTrimmedFileExtension() : string.Empty,
                    UserId = CurrentPrincipal.UserId,
                    SessionNo = CurrentPrincipal.SessionToken
                };

                //var response = DewaApiClient.SubmitConnectionEnquiry(request, RequestLanguage, Request.Segment());
                var response = DewaApiClient.SubmitConnectionEnquiry(request, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    var successModel = new EnquirySuccess()
                    {
                        //Account = account,
                        MobileNumber = request.MobileNo,
                        Documentation = request.Attachment,
                        FurtherComments = request.Remarks,
                        Reference = response.Payload.Reference,
                        AttachmentType = request.AttachmentType,
                        FileName = model.Attachment != null ? model.Attachment.FileName.GetFileNameWithoutPath() : string.Empty,
                        ///Modified to display query type on the notification message
                        QueryType = PopulateQueryTypes().QueryTypes.Where(x => x.Value == model.SelectedQueryType).FirstOrDefault().Text,
                        SelectedQueryType = model.SelectedQueryType
                    };
                    CacheProvider.Store(CacheKeys.EV_ENQUIRY_SENT_SET, new CacheItem<EnquirySuccess>(successModel));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Enquiry_Sent);
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            var content = ContextRepository.GetCurrentItem<GenericPageWithIntro>();
            model.Intro = content.Intro;
            model = PopulateQueryTypes();

            return PartialView("~/Views/Feature/EV/EVCharger/Enquiry.cshtml", model);
        }

        public ActionResult EnquirySent()
        {
            EnquirySuccess model;
            if (!CacheProvider.TryGet(CacheKeys.EV_ENQUIRY_SENT_SET, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_EnquiryPage);
            }

            model.QueryType = PopulateQueryTypes().QueryTypes.Where(x => x.Value == model.SelectedQueryType).FirstOrDefault().Text;

            return PartialView("~/Views/Feature/EV/EVCharger/EnquirySent.cshtml", model);

        }

        #endregion

        #region EV Card Status

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult Notifications()
        {
            ViewBag.NotificationFilters = GetDataSource(SitecoreItemIdentifiers.EV_Notifications_Filter_V1);
            return PartialView("~/Views/Feature/EV/EVCharger/CardStatus.cshtml");
        }

        #endregion

        #region ApplyEVCard for Personal
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ApplyEVCard(string s = "", string at = null, string b = "0")
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            ViewBag.BusinessPartnerNum = GetPersonBusinessPartners();
            ApplyEVCard applyEVcard = new ApplyEVCard();
            applyEVcard.Emirates = GetCountryOREmirates();
            applyEVcard.EmirateOrCountry = "DXB";
            applyEVcard.AccountType = "1";
            //applyEVcard.CardIdType = "P";
            if (!string.IsNullOrWhiteSpace(at))
            {
                applyEVcard.AccountType = at;
                ViewBag.BusinessPartnerNum = GetBPListByCtype(applyEVcard.AccountType);
            }

            var d = GetDetailForCatOrCode("", false, applyEVcard.EmirateOrCountry);
            applyEVcard.CategoryCode = Convert.ToString(d.FirstOrDefault()?.Value);
            ViewBag.CategoryCode = d;
            ViewBag.PlateCode = GetDetailForCatOrCode(applyEVcard.CategoryCode, true, applyEVcard.EmirateOrCountry);
            applyEVcard.RegistrationStage = Step.STEP_ONE;
            ViewBag.backlink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EV_IndividualApplyCard) + "?s=1&b=1";
            string p;
            if (CacheProvider.TryGet(CacheKeys.EV_SDPaymentParam, out p))
            {
                ViewBag.backlink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EV_SDPayment) + "?p=" + p;
                CacheProvider.Store(CacheKeys.EV_SDPaymentParam, new AccessCountingCacheItem<string>(p, Times.Once));
            }


            if (!string.IsNullOrWhiteSpace(s))
            {
                ApplyEVCard cachedData = null;
                if (CacheProvider.TryGet(CacheKeys.EV_CustomerLoginDetails, out cachedData))
                {
                    applyEVcard = cachedData;
                    applyEVcard.Emirates = GetCountryOREmirates(applyEVcard.CarRegisteredIn);
                    Step step = (Step)Convert.ToInt32(s);
                    bool isBack = Convert.ToBoolean(b == "1");
                    switch (step)
                    {
                        case Step.NOT_DEFINED:
                            break;
                        case Step.STEP_ONE:
                            return DEWALogin_EVCard_Step1(applyEVcard, isBack);
                        case Step.STEP_TWO:
                            return DEWALogin_EVCard_Step2(applyEVcard, isBack);
                        case Step.STEP_THREE:
                            return DEWALogin_EVCard_Step3(applyEVcard, isBack);
                    }
                }
            }

            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", applyEVcard);
        }

        #region [login EV Card Steps]

        private ActionResult DEWALogin_EVCard_Step1(ApplyEVCard model, bool isBack = false)
        {

            if (isBack)
            {
                ApplyEVCard cachedData = null;
                if (!CacheProvider.TryGet(CacheKeys.EV_CustomerLoginDetails, out cachedData))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_IndividualApplyCard);
                }
                model = cachedData;
                model.RegistrationStage = Step.STEP_ONE;
                return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
            }

            model.Emirates = GetEmirates();
            model.BusinessPartnerNumber = !string.IsNullOrWhiteSpace(model.BusinessPartnerDetails) ? model.BusinessPartnerDetails?.Split('|')[0] : model.BusinessPartnerNumber;

            if (string.IsNullOrWhiteSpace(model.BusinessPartnerNumber))
            {
                ModelState.AddModelError("", Translate.Text("Ev_Bp_NumberError"));
                model.RegistrationStage = Step.STEP_ONE;
                return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
            }

            #region [Auto fill Login Customer Selected BP Detail]

            /*get RTA detail if it is UAE*/
            var response = EVServiceClient.SetNewEVGreenCardV3(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.SetNewEVGreenCardV3Request()
            {

                bpCategory = model.AccountType,
                trafficFileNumber = model.TCNumber?.Trim(),
                carRegistratedCountry = model.GetCarRegisteredCountry(),
                carRegistratedRegion = model.GetCarRegisteredRegion(),
                carIdNumber = model.SubmittedCarPlateNumbers,
                carCategory = model.SubmittedCarCategories,
                carPlateCode = model.SubmittedCarPlateCodes,
                carIdType = "P",
                bpNumber = model.BusinessPartnerNumber,
                sessionId = CurrentPrincipal.SessionToken,
                userId = CurrentPrincipal.UserId
            }, RequestLanguage, Request.Segment());

            if (!response.Succeeded)
            {
                ModelState.AddModelError("", response.Message);
                return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
            }

            var rtoDetails = response.Payload;
            /*atuo fill*/
            var _userdetails = GetPersonBusinessPartnersHandler(model.BusinessPartnerNumber);
            if (_userdetails != null && _userdetails.Count > 0)
            {
                model.EmailAddress = !string.IsNullOrWhiteSpace(rtoDetails.emailId) ? rtoDetails.emailId : _userdetails.FirstOrDefault().email;
                model.MobileNumber = !string.IsNullOrWhiteSpace(rtoDetails.mobileNumber) ? rtoDetails.mobileNumber : _userdetails.FirstOrDefault()?.mobilenumber.RemoveMobileNumberZeroPrefix();
                model.POBox = !string.IsNullOrWhiteSpace(rtoDetails.poBox) ? rtoDetails.poBox : _userdetails.FirstOrDefault().POBox;
                model.EmirateinDetail = !string.IsNullOrEmpty(rtoDetails.region) ? rtoDetails.region : "";
            }
            #endregion
            /*get RTA detail if it is UAE*/
            //if (model.EmirateOrCountry == "DXB")
            //{
            //   /*Detail fetech for all user*/
            //}

            if (model.EmirateOrCountry != "DXB" || model.AccountType == _governmentCType)/*if car is registered other than DXB, proceed through normal registration and attachement is required.*/
            {
                if (model.AccountType != _governmentCType)
                {
                    model.SubmittedCarPlateCodes = "";
                    model.SubmittedCarCategories = "";
                }
                #region [Non DUBAI Customer]

                //Adding attachment logic

                byte[] attachmentBytes = new byte[0];
                var attachmentType = string.Empty;
                string error = string.Empty;
                if (model.AttachedDocument != null && model.AttachedDocument.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
                    }
                    else
                    {
                        try
                        {
                            if (model.AttachedDocument != null)
                            {
                                attachmentBytes = model.AttachedDocument.ToArray();
                                attachmentType = model.AttachedDocument.GetFileNameWithoutExtension();
                                model.AttachmentFileBinary = attachmentBytes;
                                model.AttachmentFileType = attachmentType;
                            }
                        }
                        catch (System.Exception)
                        {
                            ModelState.AddModelError(string.Empty, "Unexpected error");
                            model.RegistrationStage = Step.STEP_ONE;
                            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
                        }
                    }
                }

                if (model.AttachedDocument2 != null && model.AttachedDocument2.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.AttachedDocument2, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
                    }
                    else
                    {
                        try
                        {
                            if (model.AttachedDocument2 != null)
                            {
                                model.AttachmentFileBinary2 = model.AttachedDocument2.ToArray();
                                model.AttachmentFileType2 = model.AttachedDocument2.GetFileNameWithoutExtension();
                            }
                        }
                        catch (System.Exception)
                        {
                            ModelState.AddModelError(string.Empty, "Unexpected error");
                            model.RegistrationStage = Step.STEP_ONE;
                            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
                        }
                    }
                }

                #endregion
            }

            model.RegistrationStage = Step.STEP_TWO;
            CacheProvider.Store(CacheKeys.EV_CustomerLoginDetails, new CacheItem<ApplyEVCard>(model));
            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_Details.cshtml", model);
        }

        private ActionResult DEWALogin_EVCard_Step2(ApplyEVCard model, bool isBack = false)
        {
            #region [STEP_TWO]

            ApplyEVCard cachedData = null;

            if (!CacheProvider.TryGet(CacheKeys.EV_CustomerLoginDetails, out cachedData))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_IndividualApplyCard);
            }

            if (isBack)
            {
                return DEWALogin_EVCard_Step1(cachedData);
            }

            #region [cachedata binding]
            cachedData.EmailAddress = model.EmailAddress;
            cachedData.MobileNumber = model.MobileNumber;
            cachedData.POBox = model.POBox;
            cachedData.EmirateinDetail = model.EmirateinDetail;
            model = cachedData;
            #endregion

            if (ModelState.IsValid)
            {

                try
                {
                    var response = EVServiceClient.SetNewEVGreenCardV3(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.SetNewEVGreenCardV3Request()
                    {
                        bpNumber = model.BusinessPartnerNumber,
                        bpCategory = model.AccountType,
                        bpRegion = model.EmirateinDetail,
                        trafficFileNumber = model.TCNumber?.Trim(),
                        carRegistratedCountry = model.GetCarRegisteredCountry(),
                        carRegistratedRegion = model.GetCarRegisteredRegion(),
                        carIdType = "P",
                        carIdNumber = model.SubmittedCarPlateNumbers,
                        carCategory = model.SubmittedCarCategories,
                        carPlateCode = model.SubmittedCarPlateCodes,
                        file1Name = model.AttachedDocument != null ? model.AttachmentFileType : string.Empty, // car Document
                        file1Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary != null ? model.AttachmentFileBinary.ToArray() : new byte[0])),// car Document data
                        file2Name = model.AttachedDocument2 != null ? model.AttachmentFileType2 : string.Empty, // car Document
                        file2Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary2 != null ? model.AttachmentFileBinary2.ToArray() : new byte[0])),// Gaurantee Document data
                        userId = CurrentPrincipal.UserId,
                        sessionId = CurrentPrincipal.SessionToken,
                        processFlag = "X",
                        emailId = model.EmailAddress,
                        mobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                        poBox = model.POBox,
                        noOfCars = Convert.ToString(Convert.ToString(model.SubmittedCarPlateNumbers ?? "").Split(',').Count())
                        //id document not required
                    }, RequestLanguage, Request.Segment());

                    if (response.Succeeded && response.Payload != null)
                    {

                        var paymentDetails = response.Payload.accountList.Where(x => !string.IsNullOrWhiteSpace(x.plateNumber));
                        if (paymentDetails != null)
                        {
                            model.EvCardPaymentDetail = new List<EvCardPaymentDetail>();
                            foreach (var item in paymentDetails)
                            {
                                model.EvCardPaymentDetail.Add(new EvCardPaymentDetail()
                                {
                                    accountNumber = item.accountNumber,
                                    amount1 = item.amount1,
                                    amount2 = item.amount2,
                                    courierCharge = item.courierCharge,
                                    courierVatAmount = item.courierVatAmount,
                                    evCardNumber = item.evCardNumber,
                                    plateNumber = item.plateNumber,
                                    sdAmount = item.sdAmount,
                                    totalAmount = item.totalAmount
                                });
                            }

                            model.PayingAmount = model.EvCardPaymentDetail.Select(x => Convert.ToDecimal(string.IsNullOrWhiteSpace(x.totalAmount) ? "0" : x.totalAmount)).Sum();
                        }

                        if (model.PayingAmount <= 0)
                        {
                            CacheProvider.Remove(CacheKeys.EV_CustomerLoginDetails);
                            CacheProvider.Store(CacheKeys.EV_RegistrationSuccess, new CacheItem<EV_Success>(new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = string.Format(Translate.Text("EV.Success_Message"), response.Payload.requestNumber) }));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_NonDewa_Success);
                        }

                        //model.PayingAmount = model.EvCardPaymentDetail.Select(x => Convert.ToDecimal(string.IsNullOrWhiteSpace(x.totalAmount) ? "0" : x.totalAmount)).Sum();
                        model.RegistrationStage = Step.STEP_THREE;
                        CacheProvider.Store(CacheKeys.EV_CustomerLoginDetails, new CacheItem<ApplyEVCard>(model));
                        return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_Reviews.cshtml", model);


                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Payload != null ? response.Payload.description : response.Message);
                    }
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(ex.Message, Translate.Text("Unexpected error"));
                }
            }

            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_Details.cshtml", model);
            #endregion

        }

        private ActionResult DEWALogin_EVCard_Step3(ApplyEVCard model, bool isBack = false)
        {

            #region [STEP_THREE]
            ApplyEVCard _cachedData;
            if (!CacheProvider.TryGet(CacheKeys.EV_CustomerLoginDetails, out _cachedData))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_IndividualApplyCard);
            }
            else
            {
                _cachedData.SuqiaDonation = model.SuqiaDonation;
                _cachedData.SuqiaDonationAmt = model.SuqiaDonationAmt;
                _cachedData.bankkey = model.bankkey;
                _cachedData.paymentMethod = model.paymentMethod;
            }
            model = _cachedData;

            if (isBack)
            {
                return DEWALogin_EVCard_Step2(_cachedData);
            }

            var response = EVServiceClient.SetNewEVGreenCardV3(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.SetNewEVGreenCardV3Request()
            {
                bpNumber = model.BusinessPartnerNumber,
                bpCategory = model.AccountType,
                bpRegion = model.EmirateinDetail,
                trafficFileNumber = model.TCNumber?.Trim(),
                carRegistratedCountry = model.GetCarRegisteredCountry(),
                carRegistratedRegion = model.GetCarRegisteredRegion(),
                carIdType = "P",
                carIdNumber = model.SubmittedCarPlateNumbers,
                carCategory = model.SubmittedCarCategories,
                carPlateCode = model.SubmittedCarPlateCodes,
                file1Name = string.IsNullOrWhiteSpace(model.AttachmentFileType) ? model.AttachmentFileType : string.Empty, // car Document
                file1Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary != null ? model.AttachmentFileBinary.ToArray() : new byte[0])),// car Document data
                file2Name = string.IsNullOrWhiteSpace(model.AttachmentFileType2) ? model.AttachmentFileType2 : string.Empty, // car Document
                file2Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary2 != null ? model.AttachmentFileBinary2.ToArray() : new byte[0])),// Gaurantee Document data
                userId = CurrentPrincipal.UserId,
                sessionId = CurrentPrincipal.SessionToken,
                processFlag = "P",
                emailId = model.EmailAddress,
                mobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                poBox = model.POBox,
                noOfCars = Convert.ToString(Convert.ToString(model.SubmittedCarPlateNumbers ?? "").Split(',').Count())
                //id document not required
            }, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null)
            {
                var rDetail = response.Payload;

                #region [MIM Payment Implementation]
                var payRequest = new CipherPaymentModel();
                payRequest.PaymentData.amounts = string.Join(",", rDetail.accountList.ToList().Select(x => x.totalAmount));
                payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.accountList.ToList().Select(x => x.accountNumber));
                payRequest.PaymentData.clearancetransaction = string.Join(",", response.Payload.accountList.ToList().Select(x => x.evCardNumber));
                payRequest.PaymentData.businesspartner = !string.IsNullOrWhiteSpace(model.BusinessPartnerNumber) ? model.BusinessPartnerNumber : rDetail.bpNumber;
                payRequest.PaymentData.email = model.EmailAddress ?? string.Empty;
                payRequest.PaymentData.mobile = model.MobileNumber?.AddMobileNumberZeroPrefix() ?? string.Empty;
                payRequest.ServiceType = ServiceType.EVCard;
                payRequest.PaymentMethod = model.paymentMethod;
                payRequest.BankKey = model.bankkey;
                payRequest.SuqiaValue = model.SuqiaDonation;
                payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                var payResponse = ExecutePaymentGateway(payRequest);
                if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                {
                    CacheProvider.Store(CacheKeys.EV_CustomerTypePayment, new CacheItem<string>("EV_CustomerTypePaymentLogin"));
                    CacheProvider.Store(CacheKeys.EV_CustomerTypePaymentRequestNumber, new CacheItem<string>(response.Payload.requestNumber));
                    CacheProvider.Store(CacheKeys.EV_CustomerTypePaymentdetails, new CacheItem<List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ContractAccount>>(response.Payload.accountList));
                    return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                }
                ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                #endregion
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Payload != null ? response.Payload.description : response.Message);
            }
            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_Reviews.cshtml", model);
            #endregion
        }
        #endregion

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyEVCard(ApplyEVCard model)
        {
            ViewBag.BusinessPartnerNum = GetBPListByCtype(model.AccountType);
            model.Emirates = GetCountryOREmirates(model.CarRegisteredIn);
            //Get Car Category and Plate Code list.
            var d = GetDetailForCatOrCode("", false, model.EmirateOrCountry);
            ViewBag.CategoryCode = d;
            ViewBag.PlateCode = (d != null && d.Count() > 0 ? GetDetailForCatOrCode(d.FirstOrDefault().Value, true, model.EmirateOrCountry) : new List<SelectListItem>());


            try
            {
                if (model.RegistrationStage == Step.NOT_DEFINED)
                {
                    model.RegistrationStage = Step.STEP_ONE;
                    model.AccountType = _personalCType;
                    return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
                }

                if (model.AccountType == _personalCType)
                {
                    model.SubmittedCarPlateNumbers = model.PlateNumber?.Trim();
                    model.SubmittedCarPlateCodes = model.PlateCode?.Trim();
                    model.SubmittedCarCategories = model.CategoryCode?.Trim();
                    model.IdType = (model.GetCarRegisteredCountry() == "AE" ? EVDocType.EidDocType : EVDocType.PassportDocType);
                }
                else if (model.AccountType != _personalCType)
                {
                    model.SubmittedCarPlateNumbers = model.CarPlateNumberList;
                    model.SubmittedCarPlateCodes = model.CarPlateCodeList;
                    model.SubmittedCarCategories = model.CarCategoryList;

                    model.IdType = EVDocType.TradLicenseDocType;

                    if (model.RegistrationStage == Step.STEP_ONE)
                    {
                        if (string.IsNullOrWhiteSpace(model.SubmittedCarPlateNumbers))
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Ev_MultiplePlateNoValidationMsg"));
                            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
                        }
                    }
                }


                if (model.RegistrationStage == Step.STEP_ONE)
                {
                    return DEWALogin_EVCard_Step1(model);
                }
                if (model.RegistrationStage == Step.STEP_TWO)
                {
                    return DEWALogin_EVCard_Step2(model);
                }
                if (model.RegistrationStage == Step.STEP_THREE)
                {
                    return DEWALogin_EVCard_Step3(model);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                LogService.Error(ex, this);
                ViewBag.BusinessPartnerNum = GetPersonBusinessPartners();
                model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
                model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
                model.EmirateOrCountry = model.Emirates.ToString();
                return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
            }

            ViewBag.BusinessPartnerNum = GetPersonBusinessPartners();
            model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
            model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
            model.EmirateOrCountry = model.Emirates.ToString();
            model.AccountType = "1";
            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard.cshtml", model);
        }
        #endregion

        #region ApplyEVCard for Organization

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult ApplyforOrganization()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
            ViewBag.BusinessPartnerNum = GetOrganizationBusinessPartners();
            return View("~/Views/Feature/EV/EVCharger/ApplyforOrganization.cshtml", new ApplyEVCard());
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyforOrganization(ApplyEVCard model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.CardIdNumber) && !string.IsNullOrEmpty(model.mulkiyanum))
                    {
                        var listofNumber = model.CardIdNumber.Split(',');
                        if (Convert.ToInt32(model.mulkiyanum) != listofNumber.Length)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("No of Cards and IDNumber Mismatch"));
                            ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                            ViewBag.BusinessPartnerNum = GetOrganizationBusinessPartners();
                            return View("~/Views/Feature/EV/EVCharger/ApplyforOrganization.cshtml", model);
                        }
                    }

                    string error;
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
                                attachmentType = model.AttachedDocument.GetFileNameWithoutExtension();
                            }

                            var response = EVServiceClient.SetNewEVGreenCard(model.BPCategoryType,
                               model.BusinessPartnerNumber,
                               model.EmailAddress,
                               model.MobileNumber.AddMobileNumberZeroPrefix(),
                               model.CardIdType, // P or M
                               model.CardIdNumber, // Mulkiya or Plate Number
                               attachmentType,
                               Server.UrlEncode(Convert.ToBase64String(attachmentBytes)),
                               CurrentPrincipal.UserId,
                               CurrentPrincipal.SessionToken,
                               RequestLanguage,
                               Request.Segment(),
                               "X", //Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               model.mulkiyanum, // No of cars
                               string.Empty, //User Creation flag "X" means create new user,if empty it will not create user
                               string.Empty,
                               string.Empty,
                               string.Empty, string.Empty);

                            if (response.Succeeded && response.Payload != null && response.Payload.responsecode == "000")
                            {
                                CacheProvider.Store(CacheKeys.EV_RegistrationSuccess, new CacheItem<EV_Success>(new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = string.Format(Translate.Text("EV.Success_Message"), response.Payload.requestNumber) }));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Success);
                            }
                            ModelState.AddModelError(string.Empty, response.Payload != null ? response.Payload.description : response.Message);
                        }
                        catch (System.Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, ex.ToString());
                        }
                    }
                }
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                ViewBag.BusinessPartnerNum = GetOrganizationBusinessPartners();
                return View("~/Views/Feature/EV/EVCharger/ApplyforOrganization.cshtml", model);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                ViewBag.BusinessPartnerNum = GetOrganizationBusinessPartners();
                return View("~/Views/Feature/EV/EVCharger/ApplyforOrganization.cshtml", model);
            }
        }

        #endregion

        #region ApplyEVCard for Government

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpGet]
        public ActionResult ApplyforGovernment()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
            ViewBag.BusinessPartnerNum = GetGovernmentBusinessPartners();
            return View("~/Views/Feature/EV/EVCharger/ApplyforGovernment.cshtml", new ApplyEVCard());
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyforGovernment(ApplyEVCard model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.CardIdNumber) && !string.IsNullOrEmpty(model.mulkiyanum))
                    {
                        var listofNumber = model.CardIdNumber.Split(',');
                        if (Convert.ToInt32(model.mulkiyanum) != listofNumber.Length)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("No of Cards and IDNumber Mismatch"));
                            ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                            ViewBag.BusinessPartnerNum = GetGovernmentBusinessPartners();
                            return View("~/Views/Feature/EV/EVCharger/ApplyforGovernment.cshtml", model);
                        }
                    }

                    string error;
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
                                attachmentType = model.AttachedDocument.GetFileNameWithoutExtension();
                            }

                            var response = EVServiceClient.SetNewEVGreenCard(model.BPCategoryType,
                               model.BusinessPartnerNumber,
                               model.EmailAddress,
                               model.MobileNumber.AddMobileNumberZeroPrefix(),
                               model.CardIdType, // P or M
                               model.CardIdNumber, // Mulkiya or Plate Number
                               attachmentType,
                               Server.UrlEncode(Convert.ToBase64String(attachmentBytes)),
                               CurrentPrincipal.UserId,
                               CurrentPrincipal.SessionToken,
                               RequestLanguage,
                               Request.Segment(),
                               "X", //Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               model.mulkiyanum, // No of cars
                               string.Empty, //User Creation flag "X" means create new user,if empty it will not create user
                               string.Empty,
                               string.Empty,
                               string.Empty, string.Empty);

                            if (response.Succeeded && response.Payload != null && response.Payload.responsecode == "000")
                            {
                                CacheProvider.Store(CacheKeys.EV_RegistrationSuccess, new CacheItem<EV_Success>(new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = string.Format(Translate.Text("EV.Success_Message"), response.Payload.requestNumber) }));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Success);
                            }
                            ModelState.AddModelError(string.Empty, response.Payload != null ? response.Payload.description : response.Message);
                        }
                        catch (System.Exception)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                        }
                    }
                }
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                ViewBag.BusinessPartnerNum = GetGovernmentBusinessPartners();
                return View("~/Views/Feature/EV/EVCharger/ApplyforGovernment.cshtml", model);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                ViewBag.BusinessPartnerNum = GetGovernmentBusinessPartners();
                return View("~/Views/Feature/EV/EVCharger/ApplyforGovernment.cshtml", model);
            }
        }

        #endregion

        #region Registration Using BPNumber
        public ActionResult Registration()
        {
            return View("~/Views/Feature/EV/EVCharger/Registration.cshtml", new EVBPAccountSetup() { BusinessPartnerNumber = "123456789", MobileNumber = "200352652", EmailAddress = "mitesh.kumar@dewa.gov.ae" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Registration(EVBPAccountSetup model)
        {
            return View("~/Views/Feature/EV/EVCharger/Registration.cshtml", new EVBPAccountSetup());
        }
        #endregion

        #region Regisrtration for NonDewa user
        public ActionResult PreRegistration_NonDewa(string q, int b = 1, string s = "")
        {

            EVAccountSetup model = null;


            try
            {
                if (model == null)
                {
                    model = new EVAccountSetup();
                }
                model.RegistrationStage = Stage.STAGE_ONE;
                model.Countries = FormExtensions.GetNationalities(DropDownTermValues);
                model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);

                model.EmirateOrCountry = "DXB";
                model.AccountType = "1";

                var d = GetDetailForCatOrCode("", false, model.EmirateOrCountry);
                model.CategoryCode = Convert.ToString(d.FirstOrDefault()?.Value);
                ViewBag.Categories = d;

                #region [old]
                //if (b == 0)
                //{
                //    CacheProvider.TryGet(CacheKeys.EV_RegistrationDetails, out model);
                //}
                //else
                //{

                //    if (model.processtFlag != "R")
                //    {
                //        CacheProvider.Remove(CacheKeys.EV_Personaldetails);
                //        CacheProvider.Remove(CacheKeys.EV_RegistrationDetails);
                //    }

                //    if (!string.IsNullOrWhiteSpace(q))
                //    {
                //        var response = EVServiceClient.GetEVParamCarDetails(q, RequestLanguage, Request.Segment());
                //        if (response.Succeeded && response.Payload != null && response.Payload?.Envelope?.Body != null)
                //        {
                //            var res = response.Payload?.Envelope?.Body?.GetEVParamCarDetailsResponse.@return;
                //            model.EmirateOrCountry = res?.carRegistratedCountry != "AE" ? res?.carRegistratedCountry : res?.carRegistratedRegion;

                //            model.AccountType = res?.customercategory;
                //            model.CarRegisteredIn = res?.carRegistratedCountry != "AE" ? "2" : "1";

                //            model.TrafficCodeNumber = res?.trafficFileNumber;
                //            if (res?.customercategory == "1")
                //            {
                //                model.CarPlateNumber = res?.carplatenumber;

                //            }
                //            else
                //            {
                //                model.CarPlateNumberList = res?.carplatenumber;
                //            }
                //            model.processtFlag = "R";
                //        }
                //    }
                //    else
                //    {
                //        //model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
                //        model.EmirateOrCountry = "DXB";
                //        model.AccountType = "1";
                //    }

                //}
                #endregion
                ViewBag.backlink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EV_Registration) + "?s=0";
                string p;
                if (CacheProvider.TryGet(CacheKeys.EV_SDPaymentParam, out p))
                {
                    ViewBag.backlink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EV_SDPayment) + "?p=" + p;
                    CacheProvider.Store(CacheKeys.EV_SDPaymentParam, new AccessCountingCacheItem<string>(p, Times.Once));
                }
                #region [Step redirect]
                try
                {
                    if (!string.IsNullOrWhiteSpace(s) && !string.IsNullOrWhiteSpace(s))
                    {
                        EVAccountSetup cachedData = null;
                        if (CacheProvider.TryGet(CacheKeys.EV_RegistrationDetails, out cachedData))
                        {
                            model = cachedData;
                            bool back = !model.Isbackfromsd;
                            model.RegistrationStage = (Stage)Convert.ToInt32(s);
                            #region [ddl datasource binding]
                            ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                            model.IssuingAuthorities = GetDataSource(SitecoreItemIdentifiers.EV_Issuing_Authorities).OrderBy(x => x.Value)?.ToList();
                            model.Emirates = GetEmirates();
                            ViewBag.Emirates = model.Emirates;
                            ViewBag.Nationalities = model.Countries;
                            ViewBag.Title = GetDataSource(SitecoreItemIdentifiers.EV_Title);
                            model.SupportingDocTypes = GetDataSource(SitecoreItemIdentifiers.EV_Supporting_doc);
                            #endregion
                            switch (model.RegistrationStage)
                            {
                                case Stage.NOT_DEFINED:
                                    model.RegistrationStage = Stage.STAGE_ONE;
                                    break;
                                case Stage.STAGE_ONE:

                                    return Execute_NonDEWA_EV_Step1(model, back);
                                case Stage.STAGE_TWO:
                                    return Execute_NonDEWA_EV_Step2(model, true);
                                case Stage.STAGE_THREE:
                                    return Execute_NonDEWA_EV_Step3(model, true);
                                case Stage.STAGE_FOUR:
                                    break;
                                case Stage.STAGE_FIVE:
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError(string.Empty, "Unexpected error");
                }

                ViewBag.PlateCodes = GetDetailForCatOrCode(model.CategoryCode, true, model.EmirateOrCountry);
                #endregion
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error");
            }



            return View("~/Views/Feature/EV/EVCharger/PreRegistration.cshtml", model);
        }

        #region [Utility STEPS]
        private ActionResult RedirectToLoginWithEV(DEWAXP.Foundation.Integration.APIHandler.Models.Response.SetNewEVGreenCardV3Response r,
            EVAccountSetup model, string step = "1", string isback = null)
        {

            //Add all the details.
            var loginEvCard = new ApplyEVCard()
            {
                BusinessPartnerNumber = r.bpNumber,
                UserId = r.userId,
                AccountType = model.AccountType,
                PlateNumber = model.CarPlateNumber,
                PlateCode = model.PlateCode,
                CategoryCode = model.CategoryCode,
                CarPlateCodeList = model.CarPlateCodeList,
                CarPlateNumberList = model.CarPlateNumberList,
                SubmittedCarCategories = model.SubmittedCarCategories,
                SubmittedCarPlateCodes = model.SubmittedCarPlateCodes,
                SubmittedCarPlateNumbers = model.SubmittedCarPlateNumbers,
                CarCategoryList = model.CarCategoryList,
                TCNumber = model.TrafficCodeNumber,
                EmailAddress = model.EmailAddress,
                MobileNumber = model.MobileNumber,
                EmirateinDetail = model.Emirate,
                EmirateOrCountry = model.EmirateOrCountry,
                POBox = model.POBox,
                CarRegisteredIn = model.CarRegisteredIn,
                Emirates = GetLstDataSource(DataSources.EmiratesList).ToList(),
            };
            CacheProvider.Store(CacheKeys.EV_CustomerLoginDetails, new CacheItem<ApplyEVCard>(loginEvCard));
            CacheProvider.Store(CacheKeys.MOVEIN_USERID, new CacheItem<string>(r.userId));
            var loginUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
            string redirectUrl = string.Format(loginUrl + "?returnUrl={0}", HttpUtility.UrlEncode(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EV_IndividualApplyCard) + "?s=" + step + "&b=" + isback));
            CacheProvider.Remove(CacheKeys.EV_RegistrationDetails);
            //TempData["Error"] = r.description;
            return Redirect(redirectUrl);
        }

        #endregion

        #region [NON DEWA EV Steps]

        private ActionResult Execute_NonDEWA_EV_Step1(EVAccountSetup model, bool isBack = false)
        {


            #region [STAGE_ONE]
            ModelState.RemoveFor<EVAccountSetup>(x => x.Password);
            ModelState.RemoveFor<EVAccountSetup>(x => x.ConfirmPassword);
            ModelState.RemoveFor<EVAccountSetup>(x => x.Username);

            string return_view = model.AccountType == _personalCType ? VIEW_Registration_NonDewaIndividual : VIEW_Registration_NonDewaBusiness;
            model.RegistrationStage = Stage.STAGE_TWO;


            if (isBack)
            {
                return View(return_view, model);
            }

            ViewBag.backlink = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EV_Registration) + "?s=0";

            if (model.EmirateOrCountry == "DXB")
            {
                #region [IN DUBAI Customer]
                /*get RTA detail if it is UAE*/
                var response = EVServiceClient.SetNewEVGreenCardV3(new DEWAXP.Foundation.Integration.APIHandler.Models.Request.SetNewEVGreenCardV3Request()
                {

                    bpCategory = model.AccountType,
                    bpNumber = model.bpNumber,
                    trafficFileNumber = model.TrafficCodeNumber?.Trim(),
                    carRegistratedCountry = model.GetCarRegisteredCountry(),
                    carRegistratedRegion = model.GetCarRegisteredRegion(),
                    carIdNumber = model.SubmittedCarPlateNumbers,
                    carCategory = model.SubmittedCarCategories,
                    carPlateCode = model.SubmittedCarPlateCodes,
                    carIdType = "P",
                }, RequestLanguage, Request.Segment());

                if (response.Succeeded)
                {

                    //var resObj = response.Payload.Envelope.Body.SetNewEVGreenCardV2Response.@return;
                    var resObj = response.Payload;
                    model.IdNumber = resObj?.idNumber;
                    model.EmailAddress = resObj.emailId;
                    model.FirstName = resObj.firstName;
                    model.LastName = resObj.lastName?.ToString();
                    model.MobileNumber = resObj.mobileNumber?.RemoveMobileNumberZeroPrefix();
                    model.IdType = resObj.idType;
                    model.Nationality = resObj.nationality?.ToString();
                    model.POBox = resObj.poBox?.ToString();
                    model.Emirate = resObj.region?.ToString();
                    model.Username = resObj?.userId?.ToString();
                    model.CompanyName = resObj?.firstName?.ToString();

                    ViewBag.datafrombackend = true;

                }
                else if (response != null && response.Payload != null &&
                        !string.IsNullOrWhiteSpace(response.Payload.userId) &&
                        !string.IsNullOrEmpty(response.Payload.userId))
                {
                    return RedirectToLoginWithEV(response.Payload, model, "1");
                }
                else
                {
                    /*if Not valid return to 1st screen*/
                    ModelState.AddModelError("", response.Message);
                    model.RegistrationStage = Stage.STAGE_ONE;
                    ViewBag.PlateCodes = GetDetailForCatOrCode(model.CategoryCode, true, model.EmirateOrCountry);
                    return_view = "~/Views/Feature/EV/EVCharger/PreRegistration.cshtml";
                }
                #endregion
            }
            else /*if car is registered other than DXB, proceed through normal registration and attachement is required.*/
            {
                #region [Non DUBAI Customer]
                model.RegistrationStage = Stage.STAGE_TWO;
                model.SubmittedCarPlateCodes = string.Empty;
                model.SubmittedCarCategories = string.Empty;
                //Adding attachment logic
                string error;
                byte[] attachmentBytes = new byte[0];
                var attachmentType = string.Empty;
                if (model.AttachedDocument != null && !AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                else
                {
                    try
                    {
                        if (model.AttachedDocument != null)
                        {
                            attachmentBytes = model.AttachedDocument.ToArray();
                            attachmentType = model.AttachedDocument.GetFileNameWithoutExtension();
                            model.AttachmentFileBinary = attachmentBytes;
                            model.AttachmentFileType = attachmentType;
                            CacheProvider.Store(CacheKeys.EV_RegistrationDetails, new CacheItem<EVAccountSetup>(model));
                        }
                    }
                    catch (System.Exception)
                    {
                        ModelState.AddModelError(string.Empty, "Unexpected error");
                        model.RegistrationStage = Stage.STAGE_ONE;
                        ViewBag.PlateCodes = GetDetailForCatOrCode(model.CategoryCode, true, model.EmirateOrCountry);
                        return_view = "~/Views/Feature/EV/EVCharger/PreRegistration.cshtml";
                        //return View("PreRegistration", model);
                    }
                }
                #endregion
            }

            CacheProvider.Store(CacheKeys.EV_RegistrationDetails, new CacheItem<EVAccountSetup>(model));
            return View(return_view, model);
            #endregion
        }

        private ActionResult Execute_NonDEWA_EV_Step2(EVAccountSetup model, bool isBack = false)
        {

            #region [STAGE_TWO]
            EVAccountSetup modelReg;
            if (!CacheProvider.TryGet(CacheKeys.EV_RegistrationDetails, out modelReg))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
            }

            if (isBack)
            {
                if (modelReg.IsUserExited)
                {
                    return Execute_NonDEWA_EV_Step3(modelReg);
                }
                modelReg.RegistrationStage = Stage.STAGE_THREE;
                return View("~/Views/Feature/EV/EVCharger/Registration_NonDewaUserCreate.cshtml", modelReg);
            }
            model.AttachmentFileBinary2 = new byte[0];
            model.AttachmentFileType2 = string.Empty;
            model.RegistrationStage = Stage.STAGE_THREE;

            //if (modelReg.EmirateOrCountry != "DXB")
            //{

            //    if (model != null && model.AttachedDocument != null)
            //    {
            //        attachmentBytesforIdType = model.AttachedDocument.ToArray();
            //        attachmentTypeforIdTypeName = model.AttachedDocument.GetFileNameWithoutExtension();
            //    }

            //    if (modelReg != null && modelReg.AttachedDocument != null)
            //    {
            //        attachmentBytes = modelReg.AttachmentFileBinary;
            //        attachmentType = modelReg.AttachedDocument.GetFileNameWithoutExtension();
            //    }
            //}

            if (model != null && model.AttachedDocument2 != null)
            {
                model.AttachmentFileBinary2 = model.AttachedDocument2.ToArray();
                model.AttachmentFileType2 = model.AttachedDocument2.GetFileNameWithoutExtension();
            }

            if (model.AccountType == _businessCType)
            {
                if (model.IssuingAuthority != null && model.IssuingAuthorities.Any(x => x.Value == model.IssuingAuthority) && model.IssuingAuthority != Authority.Other)
                {
                    model.SubmittedTradeLicenceAuthorityName = model.IssuingAuthorities.Where(x => x.Value == model.IssuingAuthority).FirstOrDefault()?.Text;
                    model.SubmittedTradeLicenceAuthorityCode = model.IssuingAuthority;
                }
                else
                {
                    model.SubmittedTradeLicenceAuthorityCode = model.IssuingAuthority;
                }

            }

            #region [Binding Prevalue From Cache ]

            if (!string.IsNullOrEmpty(model.ExpiryDate))
                model.ExpiryDate = model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");

            if (!string.IsNullOrEmpty(model.DateOfBirth))
                model.DateOfBirth = model.DateOfBirth.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");


            modelReg.IdType = model.IdType;
            modelReg.ExpiryDate = model.ExpiryDate;
            modelReg.DateOfBirth = model.DateOfBirth;
            modelReg.IdNumber = model.IdNumber;
            modelReg.FirstName = modelReg.AccountType == _personalCType ? model.FirstName : model.CompanyName;
            modelReg.LastName = model.LastName;
            modelReg.POBox = model.POBox;
            modelReg.MobileNumber = model.MobileNumber;
            modelReg.EmailAddress = model.EmailAddress;
            modelReg.POBox = model.POBox;
            modelReg.Nationality = model.Nationality;
            modelReg.Emirate = model.Emirate;
            modelReg.Title = (model.AccountType == _personalCType ? model.Title : Translate.Text("companyTitleCode"));
            modelReg.AttachedDocument2 = model.AttachedDocument2;
            modelReg.AttachmentFileBinary2 = model.AttachmentFileBinary2;
            modelReg.AttachmentFileType2 = model.AttachmentFileType2;
            modelReg.SubmittedTradeLicenceAuthorityName = model.SubmittedTradeLicenceAuthorityName;
            modelReg.SubmittedTradeLicenceAuthorityCode = model.SubmittedTradeLicenceAuthorityCode;
            model = modelReg;
            #endregion

            /*Check and User is already exist with this submitted EID*/
            #region [Check and User is already exist with this submitted Id Number]
            var serviceRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.SetNewEVGreenCardV3Request()
            {

                bpCategory = model.AccountType,
                trafficFileNumber = model.TrafficCodeNumber?.Trim(),
                carRegistratedCountry = model.GetCarRegisteredCountry(),
                carRegistratedRegion = model.GetCarRegisteredRegion(),
                carIdNumber = model.SubmittedCarPlateNumbers,
                carCategory = model.SubmittedCarCategories,
                carPlateCode = model.SubmittedCarPlateCodes,
                carIdType = "P",
                idType = model.IdType,
                idNumber = model.IdNumber,
                addressTitle = (model.AccountType == _personalCType ? model.Title : Translate.Text("companyTitleCode")),
                bpFirstName = model.FirstName,
                bpLastName = model.LastName,
                emailId = model.EmailAddress,
                mobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                poBox = model.POBox,
                nationality = model.Nationality,
                bpRegion = model.Emirate,
                file1Name = model.AttachedDocument != null ? model.AttachmentFileType : string.Empty, // car Document
                file1Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary != null ? model.AttachmentFileBinary : new byte[0])),// car Document data
                bpNumber = modelReg.bpNumber,
                tradelicenceauthorityname = model.SubmittedTradeLicenceAuthorityName,
                tradelicenceauthoritycode = model.SubmittedTradeLicenceAuthorityCode,
                file2Name = model.AttachmentFileType2 != null ? model.AttachmentFileType2 : string.Empty, // proof Document
                file2Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary2 != null ? model.AttachmentFileBinary2 : new byte[0])),// proof Document data
                idexpiry = !string.IsNullOrEmpty(model.ExpiryDate) ? Convert.ToDateTime(model.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                dateofbirth = !string.IsNullOrEmpty(model.DateOfBirth) ? Convert.ToDateTime(model.DateOfBirth).ToString("yyyyMMdd") : string.Empty

            };

            var userCheckResponse = EVServiceClient.SetNewEVGreenCardV3(serviceRequest, RequestLanguage, Request.Segment());

            if (!userCheckResponse.Succeeded)
            {
                /*if user already link or exit after searching by EID skip the create screen Redirect to Login then after Login redirect to Review Page*/
                if (userCheckResponse.Payload != null &&
                !string.IsNullOrEmpty(userCheckResponse.Payload.userId))
                {
                    return RedirectToLoginWithEV(userCheckResponse.Payload, model, "3", "1");
                }

                ModelState.AddModelError("", userCheckResponse.Message);
                return View(model.AccountType == _personalCType ? VIEW_Registration_NonDewaIndividual : VIEW_Registration_NonDewaBusiness, model);
            }
            #endregion

            //if (model.IdType == EVDocType.EidDocType)
            //{

            //}
            modelReg.RegistrationStage = Stage.STAGE_THREE;
            CacheProvider.Store(CacheKeys.EV_RegistrationDetails, new CacheItem<EVAccountSetup>(model));
            return View("~/Views/Feature/EV/EVCharger/Registration_NonDewaUserCreate.cshtml", model);
            #endregion

        }

        private ActionResult Execute_NonDEWA_EV_Step3(EVAccountSetup model, bool isBack = false)
        {

            EVAccountSetup modelReg;
            if (!CacheProvider.TryGet(CacheKeys.EV_RegistrationDetails, out modelReg))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
            }

            if (isBack)
            {
                if (modelReg.IsUserExited)
                {
                    return Execute_NonDEWA_EV_Step1(modelReg, true);
                }

                return Execute_NonDEWA_EV_Step2(modelReg, true);
            }

            #region [Bind value from cache]

            if (!model.IsUserExited)
            {
                modelReg.Username = model.Username;
                modelReg.Password = model.Password;
                modelReg.ConfirmPassword = model.ConfirmPassword;
            }

            model = modelReg;
            #endregion

            #region [STAGE THREE]
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.ExpiryDate))
                    model.ExpiryDate = model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");

                if (!string.IsNullOrEmpty(model.DateOfBirth))
                    model.DateOfBirth = model.DateOfBirth.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");


                //TODO: Need to implement logic for bussiness.
                var serviceRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.SetNewEVGreenCardV3Request()
                {

                    bpCategory = model.AccountType,
                    trafficFileNumber = model.TrafficCodeNumber?.Trim(),
                    carRegistratedCountry = model.GetCarRegisteredCountry(),
                    carRegistratedRegion = model.GetCarRegisteredRegion(),
                    carIdNumber = model.SubmittedCarPlateNumbers,
                    carCategory = model.SubmittedCarCategories,
                    carPlateCode = model.SubmittedCarPlateCodes,
                    carIdType = "P",
                    idType = model.IdType,
                    idNumber = model.IdNumber,
                    addressTitle = (model.AccountType == _personalCType ? model.Title : Translate.Text("companyTitleCode")),
                    bpFirstName = model.FirstName,
                    bpLastName = model.LastName,
                    emailId = model.EmailAddress,
                    mobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                    poBox = model.POBox,
                    nationality = model.Nationality,
                    bpRegion = model.Emirate,
                    processFlag = "X",
                    file1Name = model.AttachedDocument != null ? model.AttachmentFileType : string.Empty, // car Document
                    file1Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary != null ? model.AttachmentFileBinary : new byte[0])),// car Document data
                    bpNumber = modelReg.bpNumber,
                    tradelicenceauthorityname = model.SubmittedTradeLicenceAuthorityName,
                    tradelicenceauthoritycode = model.SubmittedTradeLicenceAuthorityCode,
                    file2Name = model.AttachmentFileType2 != null ? model.AttachmentFileType2 : string.Empty, // proof Document
                    file2Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary2 != null ? model.AttachmentFileBinary2 : new byte[0])),// proof Document data
                    idexpiry = !string.IsNullOrEmpty(model.ExpiryDate) ? Convert.ToDateTime(model.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                    dateofbirth = !string.IsNullOrEmpty(model.DateOfBirth) ? Convert.ToDateTime(model.DateOfBirth).ToString("yyyyMMdd") : string.Empty
                };



                if (!model.IsUserExited)
                {

                    var availability = DewaApiClient.VerifyUserIdentifierAvailable(model.Username, RequestLanguage, Request.Segment());
                    if (availability.Succeeded && availability.Payload.IsAvailableForUse)
                    {
                        serviceRequest.userId = model.Username;
                        serviceRequest.password = model.Password;
                        serviceRequest.userCreateFlag = "X";


                        var evProcessResponse = EVServiceClient.SetNewEVGreenCardV3(serviceRequest, RequestLanguage, Request.Segment());
                        var responseData = evProcessResponse.Payload;

                        if (!evProcessResponse.Succeeded && responseData != null &&
                            !string.IsNullOrWhiteSpace(responseData.userId) &&
                            !string.Equals(responseData.userId, serviceRequest.userId, StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToLoginWithEV(responseData, model, "3", "1");
                        }


                        if (evProcessResponse.Succeeded && responseData != null)
                        {
                            model.RegistrationStage = Stage.STAGE_FOUR;
                            var paymentList = responseData.accountList.Where(x => !string.IsNullOrWhiteSpace(x.plateNumber));
                            if (paymentList.Count() > 0)
                            {
                                model.EvCardPaymentDetail = new List<EvCardPaymentDetail>();
                                foreach (DEWAXP.Foundation.Integration.APIHandler.Models.Response.ContractAccount item in paymentList)
                                {
                                    model.EvCardPaymentDetail.Add(new EvCardPaymentDetail()
                                    {
                                        accountNumber = item.accountNumber,
                                        amount1 = item.amount1,
                                        amount2 = item.amount2,
                                        courierCharge = item.courierCharge,
                                        courierVatAmount = item.courierVatAmount,
                                        evCardNumber = item.evCardNumber,
                                        sdAmount = item.sdAmount,
                                        totalAmount = item.totalAmount,
                                        plateNumber = item.plateNumber,

                                    });
                                }
                                model.bpNumber = responseData.bpNumber;
                                model.Username = responseData.userId;
                                model.PayingAmount = model.EvCardPaymentDetail.Select(x => Convert.ToDecimal(string.IsNullOrWhiteSpace(x.totalAmount) ? "0" : x.totalAmount)).Sum();

                                //if (model.PayingAmount <= 0)
                                //{
                                //    CacheProvider.Remove(CacheKeys.EV_RegistrationDetails);
                                //    CacheProvider.Store(CacheKeys.EV_RegistrationSuccess, new CacheItem<EV_Success>(new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = Translate.Text("EV.Success_Message") }));
                                //    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_NonDewa_Success);
                                //}
                            }


                            if (model.PayingAmount <= 0)
                            {
                                CacheProvider.Remove(CacheKeys.EV_CustomerLoginDetails);
                                CacheProvider.Store(CacheKeys.EV_RegistrationSuccess, new CacheItem<EV_Success>(new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = string.Format(Translate.Text("EV.Success_Message"), evProcessResponse.Payload.requestNumber) }));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_NonDewa_Success);
                            }

                            CacheProvider.Store(CacheKeys.EV_RegistrationDetails, new CacheItem<EVAccountSetup>(model));
                            return View("~/Views/Feature/EV/EVCharger/Registration_NonDewaReview.cshtml", model);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, evProcessResponse.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, availability.Message);
                    }
                }
            }

            if (modelReg.IsUserExited)
            {
                return Execute_NonDEWA_EV_Step1(modelReg, true);
            }
            return View("~/Views/Feature/EV/EVCharger/Registration_NonDewaUserCreate.cshtml", model);
            #endregion

        }

        private ActionResult Execute_NonDEWA_EV_Step4(EVAccountSetup model, bool isBack = false)
        {
            EVAccountSetup modelReg;
            if (!CacheProvider.TryGet(CacheKeys.EV_RegistrationDetails, out modelReg))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
            }
            else
            {
                modelReg.bankkey = model.bankkey;
                modelReg.SuqiaDonation = model.SuqiaDonation;
                modelReg.SuqiaDonationAmt = model.SuqiaDonationAmt;
                modelReg.paymentMethod = model.paymentMethod;
            }
            model = modelReg;

            if (!string.IsNullOrEmpty(model.ExpiryDate))
                model.ExpiryDate = model.ExpiryDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");

            if (!string.IsNullOrEmpty(model.DateOfBirth))
                model.DateOfBirth = model.DateOfBirth.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");


            var serviceRequest = new DEWAXP.Foundation.Integration.APIHandler.Models.Request.SetNewEVGreenCardV3Request()
            {

                bpCategory = model.AccountType,
                trafficFileNumber = model.TrafficCodeNumber?.Trim(),
                carRegistratedCountry = model.GetCarRegisteredCountry(),
                carRegistratedRegion = model.GetCarRegisteredRegion(),
                carIdNumber = model.SubmittedCarPlateNumbers,
                carCategory = model.SubmittedCarCategories,
                carPlateCode = model.SubmittedCarPlateCodes,
                carIdType = "P",
                idType = model.IdType,
                idNumber = model.IdNumber,
                addressTitle = (model.AccountType == _personalCType ? model.Title : Translate.Text("companyTitleCode")),
                bpFirstName = model.FirstName,
                bpLastName = model.LastName,
                emailId = model.EmailAddress,
                mobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix(),
                poBox = model.POBox,
                nationality = model.Nationality,
                bpRegion = model.Emirate,
                processFlag = "P",
                bpNumber = modelReg.bpNumber,
                userId = modelReg.Username,
                file1Name = model.AttachedDocument != null ? model.AttachmentFileType : string.Empty, // car Document
                file1Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary != null ? model.AttachmentFileBinary : new byte[0])),// car Document data
                tradelicenceauthorityname = model.SubmittedTradeLicenceAuthorityName,
                tradelicenceauthoritycode = model.SubmittedTradeLicenceAuthorityCode,
                file2Name = model.AttachmentFileType2 != null ? model.AttachmentFileType2 : string.Empty, // proof Document
                file2Data = Server.UrlEncode(Convert.ToBase64String(model.AttachmentFileBinary2 != null ? model.AttachmentFileBinary2 : new byte[0])),// proof Document data
                idexpiry = !string.IsNullOrEmpty(model.ExpiryDate) ? Convert.ToDateTime(model.ExpiryDate).ToString("yyyyMMdd") : string.Empty,
                dateofbirth = !string.IsNullOrEmpty(model.DateOfBirth) ? Convert.ToDateTime(model.DateOfBirth).ToString("yyyyMMdd") : string.Empty

            };

            var response = EVServiceClient.SetNewEVGreenCardV3(serviceRequest, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null)
            {
                #region [MIM Payment Implementation]
                var payRequest = new CipherPaymentModel();
                payRequest.PaymentData.amounts = string.Join(",", response.Payload.accountList.ToList().Select(x => x.totalAmount));
                payRequest.PaymentData.contractaccounts = string.Join(",", response.Payload.accountList.ToList().Select(x => x.accountNumber)); //accountno(s)
                payRequest.PaymentData.clearancetransaction = string.Join(",", response.Payload.accountList.ToList().Select(x => x.evCardNumber)); //accountno(s)
                payRequest.PaymentData.businesspartner = !string.IsNullOrWhiteSpace(response.Payload.bpNumber) ? response.Payload.bpNumber : (modelReg.bpNumber ?? string.Empty); //bpnumber
                payRequest.PaymentData.email = model.EmailAddress ?? string.Empty;
                payRequest.PaymentData.mobile = model.MobileNumber?.AddMobileNumberZeroPrefix() ?? string.Empty;
                payRequest.PaymentData.userid = model.Username;
                payRequest.ServiceType = ServiceType.EVCard;
                payRequest.PaymentMethod = model.paymentMethod;
                payRequest.BankKey = model.bankkey;
                payRequest.SuqiaValue = model.SuqiaDonation;
                payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                var payResponse = ExecutePaymentGateway(payRequest);
                if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                {
                    CacheProvider.Store(CacheKeys.EV_CustomerTypePayment, new CacheItem<string>("EV_CustomerTypePaymentNonDEWA"));
                    CacheProvider.Store(CacheKeys.EV_CustomerTypePaymentRequestNumber, new CacheItem<string>(response.Payload.requestNumber));
                    CacheProvider.Store(CacheKeys.EV_CustomerTypePaymentdetails, new CacheItem<List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ContractAccount>>(response.Payload.accountList));
                    return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                }
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new CacheItem<string>(string.Join("\n", payResponse.ErrorMessages.Values.ToList())));
                #endregion
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
        }

        #endregion

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PreRegistration_NonDewa(EVAccountSetup model)
        {

            CacheProvider.Remove(CacheKeys.MOVEIN_USERID);
            //Get Car Category and Plate Code list.
            var d = GetDetailForCatOrCode("", false, model.EmirateOrCountry);
            ViewBag.Categories = d;
            ViewBag.PlateCodes = GetDetailForCatOrCode(model.CategoryCode, true, model.EmirateOrCountry);


            var return_view = "~/Views/Feature/EV/EVCharger/PreRegistration.cshtml";
            try
            {


                model.Countries = FormExtensions.GetNationalities(DropDownTermValues, true, true);
                #region [ddl datasource binding]
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                model.IssuingAuthorities = GetDataSource(SitecoreItemIdentifiers.EV_Issuing_Authorities).OrderBy(x => x.Value)?.ToList();
                model.Emirates = GetEmirates();
                ViewBag.Emirates = model.Emirates;
                ViewBag.Nationalities = model.Countries;
                ViewBag.Title = GetDataSource(SitecoreItemIdentifiers.EV_Title);
                model.SupportingDocTypes = GetDataSource(SitecoreItemIdentifiers.EV_Supporting_doc);
                #endregion


                if (model.RegistrationStage == Stage.NOT_DEFINED)
                {
                    model.RegistrationStage = Stage.STAGE_ONE;
                    //model.Countries = FormExtensions.GetNationalities(DropDownTermValues, true, true);
                    //model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
                    model.AccountType = _personalCType;
                    //ViewBag.Title = GetDataSource(SitecoreItemIdentifiers.EV_Title);
                    return View(return_view, model);
                }

                if (model.AccountType == _personalCType)
                {
                    model.SubmittedCarPlateNumbers = model.CarPlateNumber?.Trim();
                    model.SubmittedCarPlateCodes = model.PlateCode?.Trim();
                    model.SubmittedCarCategories = model.CategoryCode?.Trim();
                    model.IdType = (model.GetCarRegisteredCountry() == "AE" ? EVDocType.EidDocType : EVDocType.PassportDocType);
                }

                if (model.AccountType == _businessCType)
                {
                    model.SubmittedCarPlateNumbers = model.CarPlateNumberList;
                    model.SubmittedCarPlateCodes = model.CarPlateCodeList;
                    model.SubmittedCarCategories = model.CarCategoryList;
                    model.IdType = EVDocType.TradLicenseDocType;

                    if (model.RegistrationStage == Stage.STAGE_ONE)
                    {
                        if (string.IsNullOrWhiteSpace(model.SubmittedCarPlateNumbers))
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Ev_MultiplePlateNoValidationMsg"));
                            return View("~/Views/Feature/EV/EVCharger/PreRegistration.cshtml", model);
                        }
                    }
                }

                if (model.RegistrationStage == Stage.STAGE_ONE)
                {
                    return Execute_NonDEWA_EV_Step1(model);
                }

                if (model.RegistrationStage == Stage.STAGE_TWO)
                {
                    return Execute_NonDEWA_EV_Step2(model);
                }

                if (model.RegistrationStage == Stage.STAGE_THREE)
                {
                    return Execute_NonDEWA_EV_Step3(model);
                }

                if (model.RegistrationStage == Stage.STAGE_FOUR)
                {
                    #region [STAGE FOUR]
                    return Execute_NonDEWA_EV_Step4(model);
                    #endregion
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                LogService.Error(ex, this);
            }

            //model.RegistrationStage = Stage.STAGE_ONE;

            model.Countries = FormExtensions.GetNationalities(DropDownTermValues, true, true);
            ViewBag.Nationalities = model.Countries;
            model.Emirates = GetLstDataSource(DataSources.EmiratesList).ToList();
            model.Emirates.Find(x => x.Value.ToLower() == "dxb").Selected = true;
            ViewBag.Emirates = model.Emirates;
            model.AccountType = "1";
            ViewBag.Title = GetDataSource(SitecoreItemIdentifiers.EV_Title);

            return View(return_view, model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Registration_NonDewa(EVAccountSetup model)
        {

            try
            {
                EVAccountSetup personaldata;
                if (CacheProvider.TryGet(CacheKeys.EV_Personaldetails, out personaldata))
                {
                    if (personaldata != null)
                    {
                        if (!string.IsNullOrWhiteSpace(personaldata.FirstName))
                        {
                            if (model.AccountType == ((int)AccountType.Personal).ToString())
                            {
                                model.FirstName = personaldata.FirstName;
                            }
                            else
                            {
                                model.CompanyName = personaldata.FirstName;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(personaldata.LastName))
                        {
                            if (model.AccountType == ((int)AccountType.Personal).ToString())
                            {
                                model.LastName = personaldata.LastName;
                            }
                            else
                            {
                                model.LastName = string.Empty;
                            }
                        }

                    }
                }
                if (ModelState.IsValid)
                {
                    string error;
                    byte[] attachmentBytes = new byte[0];
                    var attachmentType = string.Empty;
                    if (model.AttachedDocument != null && !AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        try
                        {
                            if (model.AttachedDocument != null)
                            {
                                attachmentBytes = model.AttachedDocument.ToArray();
                                attachmentType = model.AttachedDocument.GetFileNameWithoutExtension();
                                model.AttachmentFileBinary = attachmentBytes;
                            }

                            var availability = DewaApiClient.VerifyUserIdentifierAvailable(model.Username, RequestLanguage, Request.Segment());
                            if (availability.Succeeded && availability.Payload.IsAvailableForUse)
                            {
                                var response = EVServiceClient.SetNewEVGreenCard(model.AccountType,
                                   string.Empty,
                                   model.EmailAddress.Contains("***") ? string.Empty : model.EmailAddress,
                                   model.MobileNumber.Contains("***") ? string.Empty : model.MobileNumber.AddMobileNumberZeroPrefix(),
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   string.Empty,
                                   model.Username,
                                   CurrentPrincipal.SessionToken,
                                   RequestLanguage, Request.Segment(),
                                   "", //Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                                   model.Password,
                                   model.AccountType == ((int)AccountType.Personal).ToString() ? model.Title : Translate.Text("companyTitleCode"), // 0003 for company 
                                   model.AccountType == ((int)AccountType.Personal).ToString() ? Regex.Replace(model.FirstName, @"[^0-9a-zA-Z]+", " ").Trim() : Regex.Replace(model.CompanyName, @"[^0-9a-zA-Z]+", " ").Trim(),  // First Name
                                   !string.IsNullOrEmpty(model.LastName) ? Regex.Replace(model.LastName, @"[^0-9a-zA-Z]+", " ").Trim() : string.Empty,
                                   model.Nationality,
                                   model.Emirate,
                                   model.POBox,
                                   string.Empty, //No of Cars 
                                   "", //User Creation flag "X" means create new user,if empty it will not create user
                                   model.IdType,
                                   model.IdNumber,
                                   attachmentType, Server.UrlEncode(Convert.ToBase64String(attachmentBytes)));

                                if (response.Succeeded && response.Payload != null && response.Payload.responsecode == "000")
                                {
                                    CacheProvider.Store(CacheKeys.EV_CustomerLoginDetails, new CacheItem<EVAccountSetup>(model));
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ApplyCard_NonDewa);
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, response.Payload != null ? response.Payload.description : response.Message);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, availability.Message);
                            }
                        }
                        catch (System.Exception)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                        }
                    }
                }
                var Nationalities = FormExtensions.GetNationalities(DropDownTermValues);
                ViewBag.Emirates = GetEmirates();
                ViewBag.Nationalities = Nationalities;
                ViewBag.Title = GetDataSource(SitecoreItemIdentifiers.EV_Title);
                return View("~/Views/Feature/EV/EVCharger/Registration_NonDewa.cshtml", model);
            }
            catch (System.Exception)
            {
                var Nationalities = FormExtensions.GetNationalities(DropDownTermValues);
                ViewBag.Emirates = GetEmirates();
                ViewBag.Nationalities = Nationalities;
                ViewBag.Title = GetDataSource(SitecoreItemIdentifiers.EV_Title);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return PartialView("~/Views/Feature/EV/EVCharger/Registration_NonDewa.cshtml", model);
            }
        }

        public ActionResult ApplyEVCard_NonDewa()
        {
            EVAccountSetup modelReg;
            if (!CacheProvider.TryGet(CacheKeys.EV_CustomerLoginDetails, out modelReg))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
            }
            var applymodel = new ApplyEVCard_NonDewa()
            {
                AccountType = modelReg.AccountType,
                EmailAddress = modelReg.EmailAddress,
                MobileNumber = modelReg.MobileNumber,
                Name = modelReg.AccountType == ((int)AccountType.Business).ToString() ? modelReg.CompanyName : modelReg.FirstName,
                CardIdNumber = modelReg.CarPlateNumber,
                CarPlateNumber = modelReg.CarPlateNumber,
                CarRegisteredIn = modelReg.CarRegisteredIn,
                EmirateOrCountry = modelReg.EmirateOrCountry,
                TrafficCodeNumber = modelReg.TrafficCodeNumber
            };
            /*
               public string CarRegisteredIn { get; set; }
        public string EmirateOrCountry { get; set; }
        public string TrafficCodeNumber { get; set; }
        public string CarPlateNumber { get; set; }
             */
            ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_NonDewa.cshtml", applymodel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApplyEVCard_NonDewa(ApplyEVCard_NonDewa model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string error;
                    string cardIdNumberList = model.AccountType == ((int)AccountType.Personal).ToString() ? model.mulkiya : model.CardIdNumber; // Mulkiya or Plate Number
                    if (!string.IsNullOrEmpty(model.mulkiyanum) && !string.IsNullOrEmpty(cardIdNumberList))
                    {
                        var listofNumber = cardIdNumberList.Split(',');
                        if (Convert.ToInt32(model.mulkiyanum) != listofNumber.Length)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("No of Cards and IDNumber Mismatch"));
                            ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                            return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_NonDewa.cshtml", model);
                        }
                    }

                    if (model.AttachedDocument != null && !AttachmentIsValid(model.AttachedDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    else
                    {
                        try
                        {
                            EVAccountSetup modelReg;
                            if (!CacheProvider.TryGet(CacheKeys.EV_CustomerLoginDetails, out modelReg))
                            {
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
                            }

                            byte[] attachmentBytes = new byte[0];
                            var attachmentType = string.Empty;
                            byte[] attachmentBytesforIdType = new byte[0];
                            var attachmentTypeforIdTypeName = string.Empty;

                            if (model != null && model.AttachedDocument != null)
                            {
                                attachmentBytes = model.AttachedDocument.ToArray();
                                attachmentType = model.AttachedDocument.GetFileNameWithoutExtension();
                            }

                            if (modelReg != null && modelReg.AttachedDocument != null)
                            {
                                attachmentBytesforIdType = modelReg.AttachedDocument.ToArray();
                                attachmentTypeforIdTypeName = modelReg.AttachedDocument.GetFileNameWithoutExtension();
                            }

                            var response = EVServiceClient.SetNewEVGreenCardFinal(modelReg.AccountType,
                                string.Empty,
                                model.EmailAddress.Contains("***") ? string.Empty : model.EmailAddress,
                                model.MobileNumber.Contains("***") ? string.Empty : model.MobileNumber.AddMobileNumberZeroPrefix(),
                                model.CardIdType, // P or M
                                model.AccountType == ((int)AccountType.Personal).ToString() ? model.mulkiya : model.CardIdNumber, // Mulkiya or Plate Number
                                attachmentType,
                                Server.UrlEncode(Convert.ToBase64String(attachmentBytes)),
                                modelReg.Username,
                                CurrentPrincipal.SessionToken,
                                model.TrafficCodeNumber, model.GetCarRegisteredRegion(), model.GetCarRegisteredCountry(),
                                RequestLanguage, Request.Segment(),
                                "X", //Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                                modelReg.Password,
                                model.AccountType == ((int)AccountType.Personal).ToString() ? modelReg.Title : Translate.Text("companyTitleCode"), // 0003 for company 
                                model.AccountType == ((int)AccountType.Personal).ToString() ? Regex.Replace(modelReg.FirstName, @"[^0-9a-zA-Z]+", " ").Trim() : Regex.Replace(modelReg.CompanyName, @"[^0-9a-zA-Z]+", " ").Trim(),  // First Name
                                !string.IsNullOrEmpty(modelReg.LastName) ? Regex.Replace(modelReg.LastName, @"[^0-9a-zA-Z]+", " ").Trim() : string.Empty,
                                modelReg.Nationality,
                                modelReg.Emirate,
                                modelReg.POBox,
                                model.mulkiyanum, // No of cars
                                "X", //User Creation flag "X" means create new user,if empty it will not create user
                                modelReg.IdType,
                                modelReg.IdNumber,
                                attachmentTypeforIdTypeName, Server.UrlEncode(Convert.ToBase64String(modelReg.AttachmentFileBinary)));
                            if (response.Succeeded)
                            {
                                CacheProvider.Remove(CacheKeys.EV_CustomerLoginDetails);
                                CacheProvider.Store(CacheKeys.EV_RegistrationSuccess, new CacheItem<EV_Success>(new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = string.Format(Translate.Text("EV.Success_Message"), response.Payload?.Envelope?.Body?.SetNewEVGreenCardV2Response?.@return?.requestNumber) }));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_NonDewa_Success);
                            }

                            ModelState.AddModelError(string.Empty, response.Message);

                        }
                        catch (System.Exception)
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                        }
                    }
                }
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_NonDewa.cshtml", model);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                ViewBag.NoofCards = GetDataSource(SitecoreItemIdentifiers.EV_NoOfCars);
                return View("~/Views/Feature/EV/EVCharger/ApplyEVCard_NonDewa.cshtml", model);
            }
        }
        #endregion

        #region DeactivateCard
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpGet]
        public ActionResult DeactivateCard_v1()
        {
            return View("~/Views/Feature/EV/EVCharger/DeactivateCard.cshtml", new EVDeactivate());
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeactivateCard_v1(EVDeactivate model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        CultureInfo culture;
                        DateTimeStyles styles;
                        culture = Context.Culture;
                        styles = DateTimeStyles.None;
                        if (culture.ToString().Equals("ar-AE"))
                        {
                            model.DeactivateDate = model.DeactivateDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                        }
                        DateTime dateResult;
                        if (DateTime.TryParse(model.DeactivateDate, culture, styles, out dateResult))
                            model.Deactivatedt = dateResult;
                        else
                            model.Deactivatedt = null;

                        var response = EVServiceClient.DeActivateEVGreenCard(model.BPNumber, model.AccountNumber, model.Deactivatedt.GetValueOrDefault().ToString("dd.MM.yyyy"), model.MobileNumber.AddMobileNumberZeroPrefix(), model.cardNumber, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                        if (response.Succeeded && response.Payload != null && response.Payload.responsecode == "000")
                        {
                            CacheProvider.Store(CacheKeys.EV_RegistrationSuccess, new CacheItem<EV_Success>(new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = string.Format(Translate.Text("EV.Success_DeactivateMessage"), response.Payload.notificationNumber, model.cardNumber) }));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Deactivate_Success);
                        }
                        ModelState.AddModelError(string.Empty, response.Payload != null ? response.Payload.description : response.Message);
                    }
                    catch (System.Exception)
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    }
                }
                model.DeactivateDate = null;
                return View("~/Views/Feature/EV/EVCharger/DeactivateCard.cshtml", model);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return View("~/Views/Feature/EV/EVCharger/DeactivateCard.cshtml", model);
            }
        }
        #endregion

        #region Deactivate Card V2
        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DeactivateCard()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            return View("~/Views/Feature/EV/EVCharger/DeactivateCard.cshtml", new EVDeactivate());
        }


        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeactivateCard(EVDeactivate model)
        {
            MoveOutState state;
            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.moveoutresult.proceed)
            {
                string PaymentPath = "evdeactivatePayment";
                CacheProvider.Store(CacheKeys.EVDEACTIVATE_PAYMENT_PATH, new CacheItem<string>(PaymentPath));

                #region [MIM Payment Implementation]
                var payRequest = new CipherPaymentModel();
                payRequest.PaymentData.amounts = state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).Select(x => x.amounttocollect).Aggregate((i, j) => i + "," + j);
                payRequest.PaymentData.contractaccounts = state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).Select(x => x.contractaccountnumber).Aggregate((i, j) => i + "," + j);
                payRequest.ServiceType = ServiceType.EVCard;
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
                #endregion
            }
            else
            {
                state.page = new List<string>
                {
                    evdeactivatestep.details.ToString()
                };
                CacheProvider.Store(CacheKeys.EV_DEACTIVATE_RESULT, new CacheItem<MoveOutState>(state, TimeSpan.FromMinutes(20)));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard_Details);
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DeactivateCard_Details()
        {
            MoveOutState state;
            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.page.Contains(evdeactivatestep.details.ToString()))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            else
            {
                state.page = new List<string>
                {
                    evdeactivatestep.details.ToString()
                };
                CacheProvider.Store(CacheKeys.EV_DEACTIVATE_RESULT, new CacheItem<MoveOutState>(state, TimeSpan.FromMinutes(20)));
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

            List<MoveoutDetailsv3> lstmoveoutdetails = new List<MoveoutDetailsv3>();
            Array.ForEach(state.moveoutdetails.accountslist.ToArray(), x => lstmoveoutdetails.Add(
                new MoveoutDetailsv3
                {
                    contractaccountnumber = x.contractaccountnumber,
                    contractaccountname = x.contractaccountname,
                    customerfirstname = x.customerfirstname,
                    customerlastname = x.customerlastname,
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

            return PartialView("~/Views/Feature/EV/EVCharger/DeactivateCardDetails.cshtml", new LstMoveoutAccount
            {
                lstdetails = lstmoveoutdetails.OrderBy(x => x.refund ? 0 : 1).ToList(),
                lsttranferaccount = lsttransferaccounts,
                IsAttachmentMandatory = attachmentMandatory,
                IsChequeRefundForAll = isChequeRefundForAll,
                IsIbanRefundForAll = isIbanRefundForAll,
                IsTransferRefundForAll = isTransferRefundForAll,
                IsWesternUnionForAll = isWesternRefundForAll,
                IsNoRefundForAll = isNoRefundForAll
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DeactivateCard_Details(LstMoveoutAccount model)
        {
            MoveOutState state;
            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.page.Contains(evdeactivatestep.details.ToString()))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!validateMoveout(model))
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            model.Attachment = new byte[0];
            string error = string.Empty;
            if (model.RefundDocument != null && model.RefundDocument.ContentLength > 0)
            {
                if (!AttachmentIsValid(model.RefundDocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                {
                    ModelState.AddModelError(string.Empty, error);
                    CacheProvider.Store(CacheKeys.ERROR_MODEL, new AccessCountingCacheItem<ModelStateDictionary>(ModelState, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard_Details);
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
            state.page = new List<string>
            {
                evdeactivatestep.details.ToString(),
                evdeactivatestep.review.ToString()
            };
            CacheProvider.Store(CacheKeys.EV_DEACTIVATE_RESULT, new CacheItem<MoveOutState>(state, TimeSpan.FromMinutes(20)));
            CacheProvider.Store(CacheKeys.EV_DEACTIVATE_DETAILS, new CacheItem<LstMoveoutAccount>(model));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard_Review);
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DeactivateCard_Review()
        {
            MoveOutState state;
            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.page.Contains(evdeactivatestep.review.ToString()))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
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
            LstMoveoutAccount model;
            if (CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_DETAILS, out model))
            {
                CultureInfo culture;
                DateTimeStyles styles;
                culture = SitecoreX.Culture;
                string reviewdisconnectdate = string.Empty;
                if (model.lstdetails != null && model.lstdetails.Count > 0 && !string.IsNullOrWhiteSpace(model.lstdetails[0].DisconnectDate))
                {
                    reviewdisconnectdate = model.lstdetails[0].DisconnectDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                }
                DateTime dateResult;
                styles = DateTimeStyles.None;
                DateTime.TryParse(reviewdisconnectdate, culture, styles, out dateResult);

                model.lstdetails.ToList().ForEach(x => x.SelectedRefundOption = (model.SameTransferIban || model.SameTransferAcccount || model.SameTransferCheque || model.SameTransferWestern) ?
                model.lstdetails[0].SelectedRefundOption : (x.SelectedRefundOption != null) ? x.SelectedRefundOption : string.Empty);
                List<MoveoutReviewV3> reviewlist = new List<MoveoutReviewV3>();
                Array.ForEach(model.lstdetails.ToArray(), x => reviewlist.Add(new MoveoutReviewV3
                {
                    CustomerAccountNumber = x.contractaccountnumber,
                    CustomerName = x.contractaccountname,
                    CustomerFirstName = x.customerfirstname,
                    CustomerLastName = x.customerlastname,
                    CardNumber = state.moveoutresult.evCardnumber,
                    RefundFlag = model.SameNoRefund ? model.lstdetails[0].SelectedRefundOption : x.SelectedRefundOption,
                    RefundThrough = !model.SameNoRefund ? (x.SelectedRefundOption.Equals("Q") ? Translate.Text("updateiban.westernunion") :
                                (x.SelectedRefundOption.Equals("T") ? Translate.Text("updateiban.anotheractive") : x.SelectedRefundOption.Equals("I") ? Translate.Text("updateiban.iban") : x.SelectedRefundOption.Equals("N") ? Translate.Text("updateiban.norefund") :
                                x.SelectedRefundOption.Equals("C") ? Translate.Text("updateiban.cheque") : string.Empty)) : (model.lstdetails[0].SelectedRefundOption.Equals("Q") ? Translate.Text("updateiban.westernunion") :
                                (model.lstdetails[0].SelectedRefundOption.Equals("T") ? Translate.Text("updateiban.anotheractive") : model.lstdetails[0].SelectedRefundOption.Equals("I") ? Translate.Text("updateiban.iban") : model.lstdetails[0].SelectedRefundOption.Equals("N") ? Translate.Text("updateiban.norefund") :
                                model.lstdetails[0].SelectedRefundOption.Equals("C") ? Translate.Text("updateiban.cheque") : string.Empty)),
                    //RefundThrough = x.SelectedRefundOption.Equals("Q") ? Translate.Text("updateiban.westernunion") :
                    //            (x.SelectedRefundOption.Equals("T") ? Translate.Text("updateiban.anotheractive") : x.SelectedRefundOption.Equals("I") ? Translate.Text("updateiban.iban") :
                    //            x.SelectedRefundOption.Equals("C") ? Translate.Text("updateiban.cheque") : string.Empty),
                    DisconnectionDate = dateResult != null ? dateResult.ToString("dd MMM yyyy", culture) : string.Empty,
                    IBANNumber = !model.SameTransferIban ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("I") && (x.ConfirmIbanAccountNumber.Equals(x.SelectedIban) || x.ConfirmIbanAccountNumber.Equals(x.IbanAccountNumber)) ? "AE" + x.ConfirmIbanAccountNumber : string.Empty) : (model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].SelectedIban) || model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].IbanAccountNumber)) ? "AE" + model.lstdetails[0].ConfirmIbanAccountNumber : string.Empty,
                    AccountNumber = !model.SameTransferAcccount ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("T") ? x.transferaccount : string.Empty) : model.lstdetails[0].transferaccount,
                    ReceivingCity = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCitylist : string.Empty) : model.lstdetails[0].IsWestenCitylist,
                    ReceivingCountry = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.CountrylistText : string.Empty) : model.lstdetails[0].CountrylistText,
                    ReceivingState = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.StatelistText : string.Empty) : model.lstdetails[0].StatelistText,
                    ReceivingCurrency = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCurrencylist : string.Empty) : model.lstdetails[0].IsWestenCurrencylist,
                }));
                return View("~/Views/Feature/EV/EVCharger/DeactivateCardReview.cshtml", reviewlist);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
        }
        [HttpPost, ValidateAntiForgeryToken]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DeactivateCard_Review(MoveoutReviewV3 reviewmodel)
        {
            MoveOutState state;
            LstMoveoutAccount model;
            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.moveoutresult.proceed)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (!state.page.Contains(evdeactivatestep.review.ToString()))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }
            if (CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_DETAILS, out model))
            {

                ServiceResponse<AccountDetails[]> response;

                response = GetBillingAccounts(false, true, "X", "X");
                List<accountsIn> accountlist = new List<accountsIn>();
                CultureInfo culture;
                DateTimeStyles styles;
                culture = SitecoreX.Culture;
                string reviewdisconnectdate = string.Empty;
                if (model.lstdetails != null && model.lstdetails.Count > 0 && !string.IsNullOrWhiteSpace(model.lstdetails[0].DisconnectDate))
                {
                    reviewdisconnectdate = model.lstdetails[0].DisconnectDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                }
                DateTime dateResult;
                styles = DateTimeStyles.None;
                DateTime.TryParse(reviewdisconnectdate, culture, styles, out dateResult);

                Array.ForEach(model.lstdetails.ToArray(), x => accountlist.Add(new accountsIn
                {
                    contractaccountnumber = x.contractaccountnumber,
                    additionalinput1 = state.moveoutresult.evCardnumber,
                    premise = response.Payload.Where(y => ("00" + y.AccountNumber).Equals(x.contractaccountnumber)).FirstOrDefault().PremiseNumber,
                    disconnectiondate = dateResult != null ? dateResult.ToString("yyyyMMdd") : string.Empty,
                    businesspartnernumber = x.businesspartnernumber,
                    refundmode = (model.SameTransferIban || model.SameTransferAcccount || model.SameTransferCheque || model.SameTransferWestern || model.SameNoRefund) ? model.lstdetails[0].SelectedRefundOption : x.SelectedRefundOption,
                    ibannumber = !model.SameTransferIban ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("I") && (x.ConfirmIbanAccountNumber.Equals(x.SelectedIban) || x.ConfirmIbanAccountNumber.Equals(x.IbanAccountNumber)) ? "AE" + x.ConfirmIbanAccountNumber : string.Empty) : (model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].SelectedIban) || model.lstdetails[0].ConfirmIbanAccountNumber.Equals(model.lstdetails[0].IbanAccountNumber)) ? "AE" + model.lstdetails[0].ConfirmIbanAccountNumber : string.Empty,
                    transferaccountnumber = !model.SameTransferAcccount ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("T") ? x.transferaccount : string.Empty) : model.lstdetails[0].transferaccount,
                    countrykey = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCountrylist : string.Empty) : model.lstdetails[0].IsWestenCountrylist,
                    region = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenStatelist : string.Empty) : model.lstdetails[0].IsWestenStatelist,
                    city = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCitylist : string.Empty) : model.lstdetails[0].IsWestenCitylist,
                    currencykey = !model.SameTransferWestern ? (x.SelectedRefundOption != null && x.SelectedRefundOption.Equals("Q") ? x.IsWestenCurrencylist : string.Empty) : model.lstdetails[0].IsWestenCurrencylist,
                }));


                moveOutParams moveoutParam = new moveOutParams
                {
                    accountlist = accountlist.ToArray(),
                    channel = "W",
                    notificationtype = "EV",
                    executionflag = "W",
                    applicationflag = "M",
                    disconnectiondate = dateResult != null ? dateResult.ToString("yyyyMMdd") : string.Empty,
                    mobile = model.MobileNumber.AddMobileNumberZeroPrefix(),
                    attachment = model.Attachment,
                    attachmenttype = model.RefundDocument != null && !string.IsNullOrWhiteSpace(model.RefundDocument.FileName) ? model.RefundDocument.FileName.Split('.')[1] : string.Empty
                };

                var responsemoveout = DewaApiClient.SetMoveOutRequest(moveoutParam, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());

                if (responsemoveout.Succeeded)
                {
                    var selectedAccounts = SharedAccount.CreateForMoveOut(response.Payload.ToList(), accountlist.Select(x => x.contractaccountnumber).ToList());

                    var moveOutConfirmModel = new MoveOutConfirm();
                    moveOutConfirmModel.SelectedAccounts = selectedAccounts;

                    CacheProvider.Store(CacheKeys.EV_DEACTIVATE_CONFIRM_ACCOUNTS, new CacheItem<MoveOutConfirm>(moveOutConfirmModel, TimeSpan.FromMinutes(20)));
                    CacheProvider.Store(CacheKeys.EV_DEACTIVATE_CONFIRM, new CacheItem<SetMoveOutRequestResponse>(responsemoveout.Payload, TimeSpan.FromMinutes(20)));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Deactivate_Success);
                }
                else
                {
                    if (responsemoveout.Payload != null && responsemoveout.Payload.@return != null && responsemoveout.Payload.@return.notificationlist != null)
                    {
                        var errorCount = 0;
                        foreach (var notification in responsemoveout.Payload.@return.notificationlist)
                        {
                            ModelState.AddModelError("error" + errorCount.ToString(), notification.message);
                            errorCount++;
                        }

                        CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                    }
                    else if (responsemoveout.Payload != null && responsemoveout.Payload.@return != null)
                    {
                        ModelState.AddModelError(string.Empty, responsemoveout.Payload.@return.description);
                        CacheProvider.Store(CacheKeys.ERROR_MODEL, new CacheItem<ModelStateDictionary>(ModelState));
                    }
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard_Review);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
        }

        [HttpGet]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        public ActionResult DeactivateCardConfirm()
        {
            SetMoveOutRequestResponse moveOutState;
            MoveOutState state;
            MoveOutConfirm moveOutConfirmModel;

            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out state))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }

            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_CONFIRM, out moveOutState))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }

            if (!CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_CONFIRM_ACCOUNTS, out moveOutConfirmModel))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
            }

            if (state.moveoutresult.issuccess)
            {
                CacheProvider.Remove(CacheKeys.EV_DEACTIVATE_CONFIRM);
                CacheProvider.Remove(CacheKeys.EV_DEACTIVATE_CONFIRM_ACCOUNTS);
                CacheProvider.Remove(CacheKeys.EV_DEACTIVATE_RESULT);
                CacheProvider.Remove(CacheKeys.EV_DEACTIVATE_SELECTEDACCOUNT);
            }

            return PartialView("~/Views/Feature/EV/EVCharger/DeactivateCardConfirm.cshtml", new ConfirmModel
            {
                Accounts = moveOutConfirmModel.SelectedAccounts,
                Notifications = moveOutState.@return.notificationlist.Select(x => new ConfirmNotificationModel { ContractAccountNumber = x.contractaccountnumber, Message = x.message, NotificationNumber = x.notificationnumber }).ToArray(),
                IsSuccess = state.moveoutresult.issuccess,
                Message = state.moveoutdetails.description,
                CardNumber = state.moveoutresult.evCardnumber,
                ErrorMessage = state.moveoutresult.errormessage
            });
        }
        [HttpGet]
        public ActionResult ClearEVDeactivateCache(string c)
        {
            if (CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out MoveOutState state))
            {
                CacheProvider.Remove(CacheKeys.EV_DEACTIVATE_RESULT);
                if (string.IsNullOrWhiteSpace(c))
                {
                    CacheProvider.Remove(CacheKeys.EV_DEACTIVATE_SELECTEDACCOUNT);
                }
            }
            if (!string.IsNullOrWhiteSpace(c))
            {
                c = StringExtensions.GetSanitizePlainText(c);
                CacheProvider.Store(CacheKeys.EV_SELECTEDCARD, new AccessCountingCacheItem<string>(c, Times.Once));
            }
            return new EmptyResult();
        }

        #endregion




        #region ReplaceCard

        [HttpGet]
        public ActionResult EVReplaceCache(string c)
        {
            if (!string.IsNullOrWhiteSpace(c))
            {
                c = StringExtensions.GetSanitizePlainText(c);
                CacheProvider.Store(CacheKeys.EV_SELECTEDCARD, new AccessCountingCacheItem<string>(c, Times.Once));
            }
            return new EmptyResult();
        }
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpGet]
        public ActionResult ReplaceCard()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            return View("~/Views/Feature/EV/EVCharger/ReplaceCard.cshtml", new EVReplaceCard());
        }

        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReplaceCard(EVReplaceCard model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var response = EVServiceClient.ReplaceEVGreenCard(model.AccountNumber, model.Reason, "X", model.CardFee, model.TaxAmount, model.TaxRate, model.TotalAmount, CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.CardNumber, RequestLanguage, Request.Segment());
                        if (response.Succeeded && response.Payload != null && response.Payload.responsecode == "000")
                        {
                            #region [MIM Payment Implementation]
                            var payRequest = new CipherPaymentModel();
                            payRequest.PaymentData.amounts = model.TotalAmount ?? string.Empty;
                            payRequest.PaymentData.contractaccounts = model.AccountNumber;
                            payRequest.PaymentData.clearancetransaction = response.Payload.requestNumber;
                            payRequest.PaymentData.movetoflag = "V";
                            payRequest.ServiceType = ServiceType.EVCard;
                            payRequest.PaymentMethod = model.paymentMethod;
                            model.RequestNumber = response.Payload.requestNumber;
                            payRequest.BankKey = model.bankkey;
                            payRequest.SuqiaValue = model.SuqiaDonation;
                            payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                            var payResponse = ExecutePaymentGateway(payRequest);
                            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                            {
                                CacheProvider.Store(CacheKeys.EV_ReplacePayment_AccountNumber, new CacheItem<EVReplaceCard>(model));
                                CacheProvider.Store(CacheKeys.EV_ReplacePayment, new CacheItem<string>("EV_ReplacePayment"));
                                return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                            }
                            ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                            #endregion
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Payload != null ? response.Payload.description : response.Message);
                        }
                    }
                    catch (System.Exception)
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    }
                }
                ViewBag.Reason = GetDataSource(SitecoreItemIdentifiers.EV_ReplaceCard_Reason);
                return View("~/Views/Feature/EV/EVCharger/ReplaceCard.cshtml", model);
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                ViewBag.Reason = GetDataSource(SitecoreItemIdentifiers.EV_ReplaceCard_Reason);
                return View("~/Views/Feature/EV/EVCharger/ReplaceCard.cshtml", model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult getEVReplaceDetails(string id, string cardno)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                CacheProvider.Store(CacheKeys.Dashboard_SELECTEDACCOUNT, new AccessCountingCacheItem<string>(id.TrimStart(new char[] { '0' }), Times.Once));
            }
            var response = EVServiceClient.ReplaceEVGreenCard(id, "", string.Empty, "0", "0", "0", "0", CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, cardno, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null && response.Payload.responsecode == "000")
            {
                return Json(new { status = true, Data = new JavaScriptSerializer().Serialize(response.Payload), Message = response.Payload != null ? response.Payload.description : response.Message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Data = "", Message = response.Payload != null ? response.Payload.description : response.Message }, JsonRequestBehavior.AllowGet);
            }
            // return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        //[Throttle(Name = "EV")]
        public ActionResult getEVPersonalDetails(string idnumber, string idtype, string category)
        {
            var response = EVServiceClient.SetNewEVGreenCard(category,
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              string.Empty, // P or M
                              string.Empty, // Mulkiya or Plate Number
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              RequestLanguage,
                              Request.Segment(),
                              string.Empty, //Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              string.Empty,
                              string.Empty, // No of cars
                              string.Empty, //User Creation flag "X" means create new user,if empty it will not create user
                              idtype,
                              idnumber,
                              string.Empty, string.Empty);
            if (response.Succeeded && response.Payload != null && response.Payload.responsecode == "000")
            {
                var responsedata = new EVAccountSetup()
                {
                    FirstName = response.Payload.firstName,
                    LastName = response.Payload.lastName,
                    EmailAddress = response.Payload.emailId,
                    MobileNumber = response.Payload.mobileNumber.RemoveMobileNumberZeroPrefix(),
                    POBox = response.Payload.poBox,
                    Nationality = response.Payload.nationaliy,
                    Emirate = response.Payload.region
                };
                CacheProvider.Store(CacheKeys.EV_Personaldetails, new CacheItem<EVAccountSetup>(responsedata));
                var customdata = new EVAccountSetup()
                {
                    FirstName = !string.IsNullOrWhiteSpace(responsedata.FirstName) ? "first name exist" : null,
                    LastName = !string.IsNullOrWhiteSpace(responsedata.LastName) ? "last name exist" : null,
                };
                return Json(new { status = true, Data = new JavaScriptSerializer().Serialize(customdata), Message = response.Payload != null ? response.Payload.description : response.Message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Data = "", Message = response.Payload != null ? response.Payload.description : response.Message }, JsonRequestBehavior.AllowGet);
            }
            // return null;
        }
        #endregion

        #region Success
        public ActionResult Success()
        {
            EV_Success model;
            if (!CacheProvider.TryGet(CacheKeys.EV_RegistrationSuccess, out model))
            {
                model = new EV_Success() { Header = Translate.Text("EV.Submission_Successful"), Description = Translate.Text("EV.Success_Message") };
            }
            return View("~/Views/Feature/EV/EVCharger/Success.cshtml", model);
        }
        #endregion

        #region DeepLink SD payment
        public ActionResult EVSDPayment(string p, int? page)
        {
            if (!string.IsNullOrWhiteSpace(p))
            {
                ServiceResponse<EVDeepLinkResponse> Response = EVDashboardClient.GetEVSDPayment(
                               new EVDeeplinkRequest
                               {
                                   param = p,
                                   userid = CurrentPrincipal.UserId,
                               }, RequestLanguage, Request.Segment());

                if (Response != null && Response.Succeeded && Response.Payload != null)
                {
                    ViewBag.status = true;
                    EVSDPaymentViewModel transactionresponse = EVSDPaymentViewModel.From(Response.Payload, page);
                    CacheProvider.Store(CacheKeys.EV_SDPayment, new CacheItem<EVDeepLinkResponse>(Response.Payload));
                    CacheProvider.Store(CacheKeys.EV_SDPaymentParam, new AccessCountingCacheItem<string>(p, Times.Once));
                    return View("~/Views/Feature/EV/EVCharger/EVSDPayment.cshtml", transactionresponse);
                }
                else
                {
                    ViewBag.ErrorMessage = Response.Message;
                }
            }
            else
            {
                ViewBag.ErrorMessage = Translate.Text("Invalid URL");
            }
            ViewBag.status = false;
            return View("~/Views/Feature/EV/EVCharger/EVSDPayment.cshtml");
        }
        public ActionResult EVSDPaymentPagingation(int? page)
        {
            EVDeepLinkResponse model;
            if (CacheProvider.TryGet(CacheKeys.EV_SDPayment, out model))
            {
                EVSDPaymentViewModel transactionresponse = EVSDPaymentViewModel.From(model, page);
                return View("~/Views/Feature/EV/EVCharger/EVSDPayment_SubList.cshtml", transactionresponse);
            }
            return View("~/Views/Feature/EV/EVCharger/EVSDPayment_SubList.cshtml");
        }
        [HttpPost]
        public ActionResult EVSDPayment(EVSDPaymentViewModel paymentViewModel)
        {
            EVDeepLinkResponse model;
            if (paymentViewModel != null && !string.IsNullOrWhiteSpace(paymentViewModel.accounts))
            {
                var modifiedacccounts = paymentViewModel.accounts.Split(',').Where(x => !string.IsNullOrWhiteSpace(x));
                if (modifiedacccounts != null && modifiedacccounts.Count() > 0)
                {
                    if (CacheProvider.TryGet(CacheKeys.EV_SDPayment, out model))
                    {
                        var selected = model.Vehiclelist.Where(i => modifiedacccounts.Contains(i.Platenumber));
                        if (selected != null && selected.Count() > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(model.Userid))
                            {
                                var loginEvCard = new ApplyEVCard()
                                {
                                    BusinessPartnerNumber = model.Businesspartnernumber,
                                    UserId = model.Userid,
                                    AccountType = model.Customercategory,
                                    PlateNumber = selected.Select(i => i.Platenumber).Aggregate((i, j) => i + "," + j),
                                    PlateCode = selected.Select(i => i.Platecode).Aggregate((i, j) => i + "," + j),
                                    CategoryCode = selected.Select(i => i.Platecategorycode).Aggregate((i, j) => i + "," + j),
                                    CarPlateCodeList = selected.Select(i => i.Platecode).Aggregate((i, j) => i + "," + j),
                                    CarPlateNumberList = selected.Select(i => i.Platenumber).Aggregate((i, j) => i + "," + j),
                                    SubmittedCarCategories = selected.Select(i => i.Platecategorycode).Aggregate((i, j) => i + "," + j),
                                    SubmittedCarPlateCodes = selected.Select(i => i.Platecode).Aggregate((i, j) => i + "," + j),
                                    SubmittedCarPlateNumbers = selected.Select(i => i.Platenumber).Aggregate((i, j) => i + "," + j),
                                    CarCategoryList = selected.Select(i => i.Platecategorycode).Aggregate((i, j) => i + "," + j),
                                    TCNumber = model.Trafficfileno,
                                    EmirateinDetail = model.Carregistrationregion,
                                    EmirateOrCountry = model.Carregistrationregion,
                                    CarRegisteredIn = !string.IsNullOrWhiteSpace(model.Carregistrationcountry) && model.Carregistrationcountry.Equals("AE") ? "1" : "2",
                                    Emirates = GetLstDataSource(DataSources.EmiratesList).ToList(),
                                };
                                CacheProvider.Store(CacheKeys.EV_CustomerLoginDetails, new CacheItem<ApplyEVCard>(loginEvCard));
                                if (!IsLoggedIn)
                                {
                                    CacheProvider.Store(CacheKeys.MOVEIN_USERID, new CacheItem<string>(model.Userid));
                                    var loginUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                                    string redirectUrl = string.Format(loginUrl + "?returnUrl={0}", HttpUtility.UrlEncode(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EV_IndividualApplyCard) + "?s=1"));
                                    return Redirect(redirectUrl);
                                }
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_IndividualApplyCard, new QueryString(false)
                                   .With("s", 1));
                            }
                            else
                            {
                                EVAccountSetup eVAccountSetup = new EVAccountSetup
                                {
                                    bpNumber = model.Businesspartnernumber,
                                    AccountType = model.Customercategory,
                                    CarPlateNumber = selected.Select(i => i.Platenumber).Aggregate((i, j) => i + "," + j),
                                    PlateCode = selected.Select(i => i.Platecode).Aggregate((i, j) => i + "," + j),
                                    CategoryCode = selected.Select(i => i.Platecategorycode).Aggregate((i, j) => i + "," + j),
                                    CarPlateCodeList = selected.Select(i => i.Platecode).Aggregate((i, j) => i + "," + j),
                                    CarPlateNumberList = selected.Select(i => i.Platenumber).Aggregate((i, j) => i + "," + j),
                                    SubmittedCarCategories = selected.Select(i => i.Platecategorycode).Aggregate((i, j) => i + "," + j),
                                    SubmittedCarPlateCodes = selected.Select(i => i.Platecode).Aggregate((i, j) => i + "," + j),
                                    SubmittedCarPlateNumbers = selected.Select(i => i.Platenumber).Aggregate((i, j) => i + "," + j),
                                    CarCategoryList = selected.Select(i => i.Platecategorycode).Aggregate((i, j) => i + "," + j),
                                    TrafficCodeNumber = model.Trafficfileno,
                                    Emirate = model.Carregistrationregion,
                                    EmirateOrCountry = model.Carregistrationregion,
                                    CarRegisteredIn = !string.IsNullOrWhiteSpace(model.Carregistrationcountry) && model.Carregistrationcountry.Equals("AE") ? "1" : "2",
                                    Emirates = GetLstDataSource(DataSources.EmiratesList).ToList(),
                                    Isbackfromsd = true
                                };
                                QueryString q1 = new QueryString(true);
                                q1.With("q", "", true);
                                q1.With("s", 1, true);
                                CacheProvider.Store(CacheKeys.EV_RegistrationDetails, new CacheItem<EVAccountSetup>(eVAccountSetup));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration, q1);
                            }

                        }
                    }
                }
            }
            string p;
            if (CacheProvider.TryGet(CacheKeys.EV_SDPaymentParam, out p))
            {
                CacheProvider.Store(CacheKeys.EV_SDPaymentParam, new AccessCountingCacheItem<string>(p, Times.Once));
            }
            QueryString q = new QueryString(true);
            q.With("p", p, true);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_SDPayment, q);

        }
        #endregion

        #region DataSource

        protected List<SelectListItem> GetDataSource(string sourceID)
        {
            var item = Context.Database.GetItem(new ID(sourceID));
            if (item != null)
            {
                return item.Children.Select(c => new SelectListItem
                {
                    Text = c.Fields["Text"].ToString(),
                    Value = c.Fields["Value"].ToString()
                }).ToList();
            }
            return new List<SelectListItem>();
        }

        private List<SelectListItem> GetPersonBusinessPartners()
        {
            List<SelectListItem> objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem()
            {
                Text = Translate.Text("EV.SelectBPNumber"),
                Value = "",
                Selected = true
            });
            try
            {
                var returnData = GetPersonBusinessPartnersHandler().Where(x => x.CustomerType == "Person");
                if (returnData != null)
                {

                    objSelectListItem.AddRange(returnData.Select(c => new SelectListItem
                    {
                        Text = c.businesspartnernumber + '-' + c.bpname,
                        Value = c.businesspartnernumber + '|' + c.mobilenumber.RemoveMobileNumberZeroPrefix() + '|' + c.email
                    }).ToList());

                }
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return objSelectListItem;
        }


        private List<BusinessPartner> GetPersonBusinessPartnersHandler(string bpNumer = null)
        {
            try
            {

                var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (UserDetails.Succeeded && UserDetails.Payload.BusinessPartners != null && UserDetails.Payload.BusinessPartners.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(bpNumer))
                    {
                        return UserDetails.Payload.BusinessPartners.Where(x => x.businesspartnernumber == bpNumer).ToList();
                    }

                    return UserDetails.Payload.BusinessPartners;
                }

                return null;

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        private List<SelectListItem> GetOrganizationBusinessPartners()
        {
            List<SelectListItem> objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem()
            {
                Text = Translate.Text("EV.SelectBPNumber"),
                Value = "",
                Selected = true
            });
            try
            {
                var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (UserDetails.Succeeded && UserDetails.Payload.BusinessPartners != null && UserDetails.Payload.BusinessPartners.Count > 0)
                {
                    objSelectListItem.AddRange(UserDetails.Payload.BusinessPartners.Where(x => x.CustomerType == "Organization" && x.BPType != "Z001").Select(c => new SelectListItem
                    {
                        Text = c.businesspartnernumber + '-' + c.bpname,
                        Value = c.businesspartnernumber + '|' + c.mobilenumber.RemoveMobileNumberZeroPrefix() + '|' + c.email
                    }).ToList());
                }

                return objSelectListItem;
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return objSelectListItem;
            }

        }

        private List<SelectListItem> GetGovernmentBusinessPartners()
        {
            List<SelectListItem> objSelectListItem = new List<SelectListItem>();
            objSelectListItem.Add(new SelectListItem()
            {
                Text = Translate.Text("EV.SelectBPNumber"),
                Value = "",
                Selected = true
            });
            try
            {
                var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
                if (UserDetails.Succeeded && UserDetails.Payload.BusinessPartners != null && UserDetails.Payload.BusinessPartners.Count > 0)
                {
                    objSelectListItem.AddRange(UserDetails.Payload.BusinessPartners.Where(x => x.CustomerType == "Organization" && x.BPType == "Z001").Select(c => new SelectListItem
                    {
                        Text = c.businesspartnernumber + '-' + c.bpname,
                        Value = c.businesspartnernumber + '|' + c.mobilenumber.RemoveMobileNumberZeroPrefix() + '|' + c.email
                    }).ToList());
                }

                return objSelectListItem;
            }
            catch (System.Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                return objSelectListItem;
            }

        }

        #endregion

        #region Helpers - Enquiries
        private ListDataSources GetQueryTypes()
        {
            //need to refactor this code as it is not yet desicded if we will store it in sitecore or make call to SAP webservice.

            return ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.EV_ENQUIRY_TYPES));
        }
        private EVEnquiry PopulateQueryTypes()
        {
            var queryTypes = GetQueryTypes().Items;

            var convertedItems = queryTypes.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
            var model = ContextRepository.GetCurrentItem<EVEnquiry>();
            model.QueryTypes = convertedItems;
            return model;
        }

        #endregion

        #region Json Lookup List
        /// <summary>
        /// return emirates when input parameter is 1, or countries when parameter is 2
        /// </summary>
        /// <param name="emid"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetCountries(string emid)
        {
            List<SelectListItem> rgns = new List<SelectListItem>();

            if (!string.IsNullOrWhiteSpace(emid))
            {
                rgns = GetCountryOREmirates(emid);
                return Json(rgns.Select(x => new { Value = x.Value, Text = x.Text }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetCatOrCodeDetail(string catCode = "", bool isPlateCode = true, string region = "DXB")
        {
            return Json(GetDetailForCatOrCode(catCode, isPlateCode, region), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Utility]

        private List<SelectListItem> GetDetailForCatOrCode(string catCode = "", bool isPlateCode = true, string region = "DXB")
        {
            List<SelectListItem> data = new List<SelectListItem>();
            DEWAXP.Foundation.Integration.APIHandler.Models.Response.EvPlateDetailsResponse d = null;

            if (!CacheProvider.TryGet("ev.plate.detail", out d))
            {
                var returnData = EVCardApiHandler.GetEvPlateDetails(new DEWAXP.Foundation.Integration.APIHandler.Models.ApiBaseRequest(), RequestLanguage, Request.Segment());
                if (returnData != null && returnData.Succeeded)
                {
                    d = returnData.Payload;
                    CacheProvider.Store("ev.plate.detail", new CacheItem<DEWAXP.Foundation.Integration.APIHandler.Models.Response.EvPlateDetailsResponse>(d, TimeSpan.FromHours(1)));
                }
            }

            if (d != null)
            {
                bool IsArabic = RequestLanguage == DEWAXP.Foundation.Integration.Enums.SupportedLanguage.Arabic;

                if (isPlateCode)
                {
                    data = d.EVPlateCodeList.Where(x => x.region == region && (x.categoryCode == catCode || string.IsNullOrWhiteSpace(catCode))).GroupBy(x => new { x.codeAR, x.codeEN, x.plateCode })?.Select(xx => new SelectListItem() { Text = (IsArabic ? xx.FirstOrDefault()?.codeAR : xx.FirstOrDefault()?.codeEN), Value = xx.FirstOrDefault()?.plateCode })?.Distinct()?.ToList();
                }
                else
                {
                    data = d.EVPlateCodeList.Where(x => x.region == region).GroupBy(x => new { x.categoryEN, x.categoryAR, x.categoryCode })?.Select(xx => new SelectListItem() { Text = (IsArabic ? xx.FirstOrDefault()?.categoryAR : xx.FirstOrDefault()?.categoryEN), Value = xx.FirstOrDefault()?.categoryCode })?.Distinct()?.ToList();
                }
            }

            return data;


        }

        private List<SelectListItem> GetCountryOREmirates(string emid = "1")
        {
            List<SelectListItem> rgns = new List<SelectListItem>();
            if (emid == "1")
            {
                rgns = GetLstDataSource(DataSources.EmiratesList).ToList();
            }
            else
            {
                rgns = FormExtensions.GetNationalities(DropDownTermValues);
                var ae = rgns.Find(x => x.Value == "AE");
                if (ae != null) { rgns.Remove(ae); }
            }
            return rgns;
        }

        private Tuple<IEnumerable<SelectListItem>, bool> PopulateMoveoutRefundOptions(string striban, string strcheque, string strtransfer, string strwestern, string stroknorefund)
        {
            try
            {
                bool iban = striban.Equals("Y") ? true : false;
                bool cheque = strcheque.Equals("Y") ? true : false;
                bool transfer = strtransfer.Equals("Y") ? true : false;
                bool western = strwestern.Equals("Y") ? true : false;
                bool oknorefund = stroknorefund.Equals("Y") ? true : false;
                var dataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.REFUND_OPTIONS));
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
                if (!model.SameTransferIban)
                {
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

        #endregion

        #region GetBusinessPartnerListByCType
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetBusinessPartnerListByCType(string ctype = "1")
        {
            return Json(GetBPListByCtype(ctype), JsonRequestBehavior.AllowGet);
        }


        private List<SelectListItem> GetBPListByCtype(string ctype = "1")
        {
            List<SelectListItem> bpList = null;
            switch (ctype)
            {
                case "1":
                    bpList = GetPersonBusinessPartners();
                    break;
                case "2":
                    bpList = GetOrganizationBusinessPartners();
                    break;
                case "3":
                    bpList = GetGovernmentBusinessPartners();
                    break;
                default:
                    bpList = new List<SelectListItem>();
                    break;
            }


            return bpList;
        }
        #endregion
    }
}