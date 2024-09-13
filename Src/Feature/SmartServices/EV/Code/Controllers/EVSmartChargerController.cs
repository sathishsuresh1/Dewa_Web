// <copyright file="EVSmartChargerController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Controllers
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Impl.EVLocationSvc;
    using DEWAXP.Foundation.Integration.InternshipSvc;
    using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVSmartCharger;
    using DEWAXP.Foundation.Integration.Responses.EVLocationSvc;
    using DEWAXP.Feature.EV.Models.EVSmartCharger;
    using global::Sitecore.Globalization;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;
    using Exception = System.Exception;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
    using DEWAXP.Foundation.Content.Models.Payment;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers;

    /// <summary>
    /// Defines the <see cref="EVSmartChargerController" />.
    /// </summary>
    public class EVSmartChargerController : BaseWorkflowController<EVSmartCharger>
    {
        /// <summary>
        /// Gets the Name.
        /// </summary>
        protected override string Name => CacheKeys.EV_SMART_WORKFLOW_STATE;

        protected virtual bool InProgressAfterPayment()
        {
            var isCached = CacheProvider.HasKey(Name);
            if (isCached)
            {
                if (CacheProvider.TryGet(Name, out EVSmartCharger eVSmartCharger))
                {
                    return eVSmartCharger.PaymentProcessed;
                }
            }
            return false;
        }

        /// <summary>
        /// The EVGreenChargerDetails.
        /// </summary>
        /// <param name="model">The model<see cref="EV_Green_Charger_Details_Param"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ChargerDetails(EV_Green_Charger_Details_Param model)
        {
            Clear();
            EV_Green_Charger_Details_ViewModel viewModel = new EV_Green_Charger_Details_ViewModel();
            if (ModelState.IsValid)
            {
                DEWAXP.Foundation.Integration.Responses.ServiceResponse<Devicelist> locations = EVCSClient.GetLocations(EVCSConstant.FULLDATA, model.evqr);
                if (locations != null && locations.Succeeded && locations.Payload != null && locations.Payload.Devices != null && locations.Payload.Devices.Length > 0 && locations.Payload.Devices.ToList().FirstOrDefault().Connectors != null
                    && locations.Payload.Devices.ToList().FirstOrDefault().TotalNbOfConnectors > 0 && locations.Payload.Devices.ToList().FirstOrDefault().Connectors.Where(x => x.LocalConnectorNumber.Equals(model.cc)).Any())
                {
                    Device device = locations.Payload.Devices.ToList().FirstOrDefault();
                    Connector connector = locations.Payload.Devices.ToList().FirstOrDefault().Connectors.Where(x => x.LocalConnectorNumber.Equals(model.cc)).FirstOrDefault();
                    viewModel = new EV_Green_Charger_Details_ViewModel
                    {
                        ChargerLocation = device.LocationName,
                        ChargerID = device.HubeleonID,
                        ChargerType = connector.ConnectorDisplay,
                        Connector = connector.ConnectorName,
                        ConnectorIcon = !string.IsNullOrWhiteSpace(connector.ConnectorDisplay) ? System.Text.RegularExpressions.Regex.Replace(connector.ConnectorDisplay, @"[^a-zA-Z]+", "").ToLower() : string.Empty,
                        ConnectorStatus = connector.ConnectorStatus,
                        ConnectorAvailable = connector.ConnectorStatus.Equals("Available"),
                        Status = true,
                    };
                    State.DeviceID = device.DeviceDBID;
                    State.ChargerID = device.HubeleonID;
                    State.ConnectorNumber = connector.LocalConnectorNumber.ToString();
                    State.ChargerLocation = viewModel.ChargerLocation;
                    State.ChargerType = viewModel.ChargerType;
                    State.Connector = viewModel.Connector;
                    State.ConnectorStatus = viewModel.ConnectorStatus;
                    Save();
                }
            }
            return View("~/Views/Feature/EV/EVSmartCharger/ChargerDetails.cshtml", viewModel);
        }

        /// <summary>
        /// The EVGreenChargerDetails.
        /// </summary>
        /// <param name="model">The model<see cref="EV_Green_Charger_Details_ViewModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChargerDetails(EV_Green_Charger_Details_ViewModel model)
        {
            if (!InProgress() && !State.ChargerID.Equals(model.ChargerID))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_GENERAL_DETAILS_PAGE);
        }

        [HttpGet]
        public ActionResult GeneralDetails()
        {
            if (!InProgress())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            var response = IntershipServiceClient.GetHelpValues(new GetInternshipHelpValues { countrycodes = "X" }, RequestLanguage);
            if (response != null && response.Succeeded && response.Payload != null && response.Payload.@return != null && response.Payload.@return.countrycodes != null)
            {
                State.CountryCallingCodesList = response.Payload.@return.countrycodes.Select(p => new SelectListItem()
                {
                    Text = "+" + p.countrytelephonecode,
                    Value = p.countrytelephonecode,
                }
            ).ToList();
                State.CountryCallingCodesList.Where(X => X.Value.Equals("971")).ToList().ForEach(y => y.Selected = true);
                ViewBag.CountryCallingCodesList = State.CountryCallingCodesList;
                return View("~/Views/Feature/EV/EVSmartCharger/GeneralDetails.cshtml", new EV_Green_Charger_GeneralDetails { EmailAddress = State.EmailAddress, CountryCode = State.CountryCode, FullName = State.FullName, MobileNumber = State.MobileNumber, PlateNumber = State.PlateNumber, VatNumber = State.VatNumber });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GeneralDetails(EV_Green_Charger_GeneralDetails model)
        {
            if (!InProgress() && !ModelState.IsValid)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
            {
                chargein = new Chargein
                {
                    fullname = model.FullName,
                    contactemail = model.EmailAddress,
                    contactmobile = model.CountryCode + model.MobileNumber,
                    vehicleidentification = model.PlateNumber,
                    vatnumber = model.VatNumber,
                    connectorid = State.Connector,
                    devicedbid = State.ChargerID,
                    devicetype = State.ChargerType,
                    locationaddress = State.ChargerLocation,
                    evmode = EVSmartChargerConstant.SendOTP,
                }
            }, RequestLanguage, Request.Segment());
            if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null && !string.IsNullOrWhiteSpace(EvSmartResponse.Payload.Requestid))
            {
                State.EmailAddress = model.EmailAddress;
                State.FullName = model.FullName;
                State.MobileNumber = model.MobileNumber;
                State.PlateNumber = model.PlateNumber;
                State.CountryCode = model.CountryCode;
                State.VatNumber = model.VatNumber;
                State.Maxotp = false;
                State.VerifiedEmail = false;
                State.RequestSubmitted = true;
                State.Requestid = EvSmartResponse.Payload.Requestid;
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_VERIFY_EMAIL_PAGE);
            }
            else
            {
                ModelState.AddModelError("", EvSmartResponse.Message);
            }
            ViewBag.CountryCallingCodesList = State.CountryCallingCodesList;
            return View("~/Views/Feature/EV/EVSmartCharger/GeneralDetails.cshtml", model);
        }

        [HttpGet]
        public ActionResult VerifyEmail()
        {
            if (!InProgress())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            if (State.Maxotp || !State.RequestSubmitted)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_GENERAL_DETAILS_PAGE);
            }
            ViewBag.Email = State.EmailAddress;
            return View("~/Views/Feature/EV/EVSmartCharger/VerifyEmail.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VerifyEmail(string otp)
        {
            if (!InProgress() && string.IsNullOrWhiteSpace(otp))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
            {
                chargein = new Chargein
                {
                    otp = otp,
                    requestid = State.Requestid,
                    evmode = EVSmartChargerConstant.VerifyOTP
                }
            }, RequestLanguage, Request.Segment());
            if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null)
            {
                State.Maxotp = EvSmartResponse.Payload.Maxotp;
                State.VerifiedEmail = true;
                State.RequestSubmitted = false;
                if (EvSmartResponse.Payload.Evstatus.Equals("40"))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_PAYMENT_CONFIRMATION_PAGE);
                }
                else if (EvSmartResponse.Payload.Otcpacks != null && EvSmartResponse.Payload.Otcpacks.Count > 0)
                {
                    State.Otcpacks = EvSmartResponse.Payload.Otcpacks;
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_CHARGING_TIMING_PAGE);
                }
            }
            else if (EvSmartResponse != null && !EvSmartResponse.Succeeded && EvSmartResponse.Payload != null && EvSmartResponse.Payload.Maxotp)
            {
                State.Maxotp = EvSmartResponse.Payload.Maxotp;
                ViewBag.Maxotp = EvSmartResponse.Payload.Maxotp;
                ModelState.AddModelError("", EvSmartResponse.Payload.Description);
            }
            else
            {
                ModelState.AddModelError("", EvSmartResponse.Message);
            }
            ViewBag.Email = State.EmailAddress;
            return View("~/Views/Feature/EV/EVSmartCharger/VerifyEmail.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ResendOTP()
        {
            string error = Translate.Text("Select the value");
            if (!InProgress())
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            if (!State.Maxotp)
            {
                var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
                {
                    chargein = new Chargein
                    {
                        requestid = State.Requestid,
                        evmode = EVSmartChargerConstant.SendOTP
                    }
                }, RequestLanguage, Request.Segment());
                if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null)
                {
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    error = EvSmartResponse.Message;
                }
            }
            return Json(new { status = false, Error = error }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ChargingTimingDetails()
        {
            if (!InProgress())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            if (State.Maxotp || !State.VerifiedEmail)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_GENERAL_DETAILS_PAGE);
            }
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                CacheProvider.Remove(CacheKeys.ERROR_MESSAGE);
            }
            if (State.Otcpacks != null && State.Otcpacks.Count > 0)
            {
                ViewBag.otcpacks = State.Otcpacks;
                return View("~/Views/Feature/EV/EVSmartCharger/ChargingTimingDetails.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChargingTimingDetails(EVChargerPayment model)
        {
            try
            {
                string error = Translate.Text("Select the value");
                if (!InProgress())
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
                }
                if (State.Otcpacks != null && State.Otcpacks.Count > 0 && State.Otcpayment != null && State.Otcpayment.Count > 0)
                {
                    #region [MIM Payment Implementation]

                    var payRequest = new CipherPaymentModel();
                    payRequest.PaymentData.amounts = string.Join(",", State.Otcpayment.Select(x => x.Amount));
                    payRequest.PaymentData.contractaccounts = string.Join(",", State.Otcpayment.Select(x => x.Contractaccount));
                    payRequest.PaymentMethod = model.paymentMethod;
                    payRequest.PaymentData.email = State.EmailAddress;
                    payRequest.PaymentData.mobile = FormatInternationalMobileNumber(State.CountryCode, State.MobileNumber);

                    payRequest.PaymentData.clearancetransaction = State.Requestid;
                    payRequest.ServiceType = IsLoggedIn ? ServiceType.EVCard : ServiceType.EVAnonymous;

                    payRequest.BankKey = model.bankkey;
                    payRequest.SuqiaValue = model.SuqiaDonation;
                    payRequest.SuqiaAmt = model.SuqiaDonationAmt;
                    var payResponse = ExecutePaymentGateway(payRequest);
                    if (Convert.ToInt32(payResponse.ErrorMessages?.Count) == 0)
                    {
                        CacheProvider.Store(CacheKeys.EV_SmartChargingSuqiaAmount, new CacheItem<decimal>(Foundation.Content.Utils.PaymentPopupHelper.GetSuqiaAmount(model.SuqiaDonation,model.SuqiaDonationAmt), TimeSpan.FromMinutes(40)));
                        CacheProvider.Store(CacheKeys.EV_SmartChargingPayment, new CacheItem<string>("EV_SmartChargingPayment"));
                        return View("~/Views/Feature/CommonComponents/Shared/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);
                    }
                    ModelState.AddModelError("", string.Join("\n", payResponse.ErrorMessages.Values.ToList()));
                    ViewBag.otcpacks = State.Otcpacks;
                    return View("~/Views/Feature/EV/EVSmartCharger/ChargingTimingDetails.cshtml");

                    #endregion [MIM Payment Implementation]
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ValidateChargingTiming(string package)
        {
            string error = Translate.Text("Select the value");
            if (!InProgress())
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            if (State.Otcpacks != null && State.Otcpacks.Count > 0)
            {
                var selectedindex = State.Otcpacks.Where(x => x.Packtype.Equals(package));
                if (selectedindex.HasAny())
                {
                    var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
                    {
                        chargein = new Chargein
                        {
                            packagetype = package,
                            requestid = State.Requestid,
                            evmode = EVSmartChargerConstant.FetchPayment
                        }
                    }, RequestLanguage, Request.Segment());
                    if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null && EvSmartResponse.Payload.Otcpayment != null && EvSmartResponse.Payload.Otcpayment.Count > 0)
                    {
                        State.Duration = selectedindex.FirstOrDefault().Packname;
                        State.Amount = selectedindex.FirstOrDefault().Packageamount;
                        State.Currency = selectedindex.FirstOrDefault().Currency;
                        State.Otcpayment = EvSmartResponse.Payload.Otcpayment;
                        return Json(new { status = true, Payload = State.Otcpayment }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        error = EvSmartResponse.Message;
                    }
                }
            }
            return Json(new { status = false, Error = error }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EVPaymentConfirmation(string p)
        {
            try
            {
                string requestid = string.Empty;
                if (string.IsNullOrWhiteSpace(p))
                {
                    if (InProgress())
                    {
                        requestid = State.Requestid;
                    }
                }
                if (!string.IsNullOrWhiteSpace(p) || !string.IsNullOrWhiteSpace(requestid))
                {
                    var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
                    {
                        chargein = new Chargein
                        {
                            requestid = requestid,
                            urlparameter = p,
                            evmode = EVSmartChargerConstant.GetDetails
                        }
                    }, RequestLanguage, Request.Segment());
                    if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null)
                    {

                        EVPaymentModel model = new EVPaymentModel
                        {
                            DegTransactionId = EvSmartResponse.Payload.Requestid,
                            DewaTransactionId = EvSmartResponse.Payload.Requestid,
                            Context = PaymentContext.EVAnonymous,
                            PaymentDate = !string.IsNullOrWhiteSpace(EvSmartResponse.Payload.Dateandtime) ? DateTime.ParseExact(EvSmartResponse.Payload.Dateandtime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now,
                            ReceiptIdentifiers = "",
                            Message = "",
                            Succeeded = true,
                            Total = Convert.ToDecimal(EvSmartResponse.Payload.Evamount),
                            Amount = EvSmartResponse.Payload.Evamount,
                            Currency = "",
                            Duration = EvSmartResponse.Payload.Packagename,
                        };
                        PaymentCompletionModel paymentState;
                        if (CacheProvider.TryGet(CacheKeys.EV_SmartChargingconfirmationPayment, out paymentState))
                        {
                            model.SuqiaAmount = paymentState.SuqiaAmount;
                        }

                        if (model.SuqiaAmount <= 0)
                        {
                            decimal SuqiaAmount = 0;
                            if (CacheProvider.TryGet(CacheKeys.EV_SmartChargingSuqiaAmount, out SuqiaAmount))
                            {
                                model.SuqiaAmount = SuqiaAmount;
                            }
                        }
                        State.PaymentProcessed = true;
                        State.PaymentDate = model.PaymentDate;
                        State.EVChargerStatus = EvSmartResponse.Payload.Evstatus;
                        State.Requestid = EvSmartResponse.Payload.Requestid;
                        State.ChargerType = EvSmartResponse.Payload.Devicetype;
                        State.ChargerID = EvSmartResponse.Payload.Devicedbid;
                        State.Connector = EvSmartResponse.Payload.Connectorid;
                        State.ChargerLocation = EvSmartResponse.Payload.Locationaddress;
                        Save();
                        var durationcount = Convert.ToInt32(EvSmartResponse.Payload.EVduraion);
                        switch (EvSmartResponse.Payload.Evstatus)
                        {
                            case "40":
                                model.Timer = GetEVTimer(model.PaymentDate, durationcount);
                                return View("~/Views/Feature/EV/EVSmartCharger/EVPaymentConfirmation.cshtml", model);

                            case "50":
                            case "60":
                            case "70": return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_EVCHARGING_PAGE);
                            case "80":
                                //ViewBag.RequestExpired = true;
                                //ModelState.AddModelError("", Translate.Text("One Time Charge request expired"));
                                return View("~/Views/Feature/EV/EVSmartCharger/EVExpiredStatus.cshtml", model);

                            default:
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EVPaymentConfirmation()
        {
            if (!InProgress() && !InProgressAfterPayment())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
            {
                chargein = new Chargein
                {
                    requestid = State.Requestid,
                    evmode = EVSmartChargerConstant.StartCharger
                }
            }, RequestLanguage, Request.Segment());
            if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null && EvSmartResponse.Payload.Evstatus.Equals("50"))
            {
                State.EVChargerStatus = EvSmartResponse.Payload.Evstatus;
                State.Durationcount = Convert.ToInt32(EvSmartResponse.Payload.EVduraion);
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_EVCHARGING_PAGE);
            }
            else
            {
                ModelState.AddModelError("", EvSmartResponse.Message);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_PAYMENT_CONFIRMATION_PAGE);
        }

        [HttpGet]
        public ActionResult EVCharging()
        {
            if (!InProgress() && !InProgressAfterPayment())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE);
            }
            if (!string.IsNullOrWhiteSpace(State.EVChargerStatus))
            {
                switch (State.EVChargerStatus)
                {
                    case "50":
                        ViewBag.ChargerType = State.Connector;
                        ViewBag.Durationcount = State.Durationcount;
                        return View("~/Views/Feature/EV/EVSmartCharger/EVCharging.cshtml");

                    case "60":
                        var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
                        {
                            chargein = new Chargein
                            {
                                requestid = State.Requestid,
                                evmode = EVSmartChargerConstant.GetDetails
                            }
                        }, RequestLanguage, Request.Segment());
                        if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null && EvSmartResponse.Payload.Evstatus.Equals("60"))
                        {
                            State.ChargerType = EvSmartResponse.Payload.Devicetype;
                            State.ChargerID = EvSmartResponse.Payload.Devicedbid;
                            State.Connector = EvSmartResponse.Payload.Connectorid;
                            State.ChargerLocation = EvSmartResponse.Payload.Locationaddress;
                            var durationcount = Convert.ToInt32(EvSmartResponse.Payload.EVduraion);
                            var startdate = DateTime.ParseExact(EvSmartResponse.Payload.Evchargestart, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                            State.Timer = GetEVTimer(startdate, durationcount);
                            State.EVChargerStatus = EvSmartResponse.Payload.Evstatus;
                            return PartialView("~/Views/Feature/EV/EVSmartCharger/ChargingStarted.cshtml", State); ;
                        }
                        break;

                    case "70":
                        var EvSmartResponse1 = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
                        {
                            chargein = new Chargein
                            {
                                requestid = State.Requestid,
                                evmode = EVSmartChargerConstant.GetDetails
                            }
                        }, RequestLanguage, Request.Segment());
                        if (EvSmartResponse1 != null && EvSmartResponse1.Succeeded && EvSmartResponse1.Payload != null && EvSmartResponse1.Payload.Evstatus.Equals("70"))
                        {
                            State.Duration = EvSmartResponse1.Payload.Packagename;
                            State.Amount = EvSmartResponse1.Payload.Evamount;
                            State.EVChargerStatus = EvSmartResponse1.Payload.Evstatus;
                            State.PaymentDate = DateTime.ParseExact(EvSmartResponse1.Payload.Dateandtime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                            return PartialView("~/Views/Feature/EV/EVSmartCharger/ChargingCompleted.cshtml", State); ;
                        }
                        break;
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_PAYMENT_CONFIRMATION_PAGE);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StartEVCharging()
        {
            try
            {
                if (!InProgress() && !InProgressAfterPayment())
                {
                    return Json(new { status = false, Redirect = true, RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EVGREEN_PAYMENT_CONFIRMATION_PAGE)}" }, JsonRequestBehavior.AllowGet);
                }
                var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
                {
                    chargein = new Chargein
                    {
                        requestid = State.Requestid,
                        evmode = EVSmartChargerConstant.GetDetails
                    }
                }, RequestLanguage, Request.Segment());
                if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null && EvSmartResponse.Payload.Evstatus.Equals("60"))
                {
                    State.ChargerType = EvSmartResponse.Payload.Devicetype;
                    State.ChargerID = EvSmartResponse.Payload.Devicedbid;
                    State.Connector = EvSmartResponse.Payload.Connectorid;
                    State.ChargerLocation = EvSmartResponse.Payload.Locationaddress;
                    var durationcount = Convert.ToInt32(EvSmartResponse.Payload.EVduraion);
                    var startdate = DateTime.ParseExact(EvSmartResponse.Payload.Evchargestart, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    State.Timer = GetEVTimer(startdate, durationcount);
                    State.EVChargerStatus = EvSmartResponse.Payload.Evstatus;
                    return PartialView("~/Views/Feature/EV/EVSmartCharger/ChargingStarted.cshtml", State); ;
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return Json(new { status = false, wait = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StopEVCharging()
        {
            var process = true;
            int count = 0; var total = 4;
            if (!InProgress() && !InProgressAfterPayment())
            {
                return Json(new { status = false, Redirect = true, RedirectUrl = $"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.EVGREEN_DETAILS_PAGE)}" }, JsonRequestBehavior.AllowGet);
            }

            var EvSmartResponse = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
            {
                chargein = new Chargein
                {
                    requestid = State.Requestid,
                    evmode = EVSmartChargerConstant.StopCharger
                }
            }, RequestLanguage, Request.Segment());
            if (EvSmartResponse != null && EvSmartResponse.Succeeded && EvSmartResponse.Payload != null)
            {
                do
                {
                    count++;
                    var EvSmartResponse1 = SmartCustomerClient.EVSmartCharger(new OneTimeChargeRequest
                    {
                        chargein = new Chargein
                        {
                            requestid = State.Requestid,
                            evmode = EVSmartChargerConstant.GetDetails
                        }
                    }, RequestLanguage, Request.Segment());
                    if (EvSmartResponse1 != null && EvSmartResponse1.Succeeded && EvSmartResponse1.Payload != null && EvSmartResponse1.Payload.Evstatus.Equals("70"))
                    {
                        process = false;
                        State.PaymentDate = DateTime.ParseExact(EvSmartResponse1.Payload.Dateandtime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                        State.EVChargerStatus = EvSmartResponse1.Payload.Evstatus;
                        State.Duration = EvSmartResponse1.Payload.Packagename;
                        State.Amount = EvSmartResponse1.Payload.Evamount;
                        var model = State;
                        Clear();
                        return PartialView("~/Views/Feature/EV/EVSmartCharger/ChargingCompleted.cshtml", model);
                    }
                    else
                    {
                        Thread.Sleep(10000);
                    }
                    if (count > total)
                    {
                        process = false;
                    }
                } while (process);
            }

            return Json(new { status = false, wait = true }, JsonRequestBehavior.AllowGet);
        }

        private string GetEVTimer(DateTime dateTime, int durationcount)
        {
            string timer = "00:00"; //defualt zero set

            if (dateTime != null)
            {
                DateTime updateDate = dateTime.AddSeconds(durationcount); // adding second in expe
                if (dateTime > DateTime.MinValue && updateDate.Subtract(DateTime.Now).Ticks > 0) //date should be greater  than now
                {
                    var _dateDiff = updateDate.Subtract(DateTime.Now);

                    int totalMinInHour = _dateDiff.Hours * 60;
                    int totalMin = _dateDiff.Minutes + totalMinInHour;

                    timer = $"{totalMin}:{_dateDiff.Seconds}"; //mm:ss
                }
            }
            return timer;
        }
    }
}