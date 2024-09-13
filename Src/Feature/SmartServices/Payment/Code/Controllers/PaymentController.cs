using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.Bills;
using DEWAXP.Foundation.Content.Models.ClearanceCertificate;
using DEWAXP.Foundation.Content.Models.EVCharger;
using DEWAXP.Foundation.Content.Models.HappinessIndicator;
using DEWAXP.Foundation.Content.Models.ScrapSale;
using DEWAXP.Foundation.Content.Models.MoveOut;
using DEWAXP.Foundation.Content.Models.MoveOut.v3;
using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Models.Payment.SecurePay;
using DEWAXP.Foundation.Content.Models.RammasLogin;
using DEWAXP.Foundation.Content.Models.RequestTempConnection;
using DEWAXP.Foundation.Content.Models.SupplyManagement.Movein;
using DEWAXP.Foundation.Content.Models.SupplyManagement.MoveTo;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DEWAXP.Foundation.Content.Models.UpdateIBAN;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Feature.Payment.Controllers
{
    public class PaymentController : BaseController
    {
        private PaymentContext? Context
        {
            get { return (PaymentContext?)Session["PaymentContext"]; }
            set { Session["PaymentContext"] = value; }
        }

        private string _branch
        {
            get
            {
                var _fc = FetchFutureCenterValues();
                return _fc.Branch;
            }
        }

        private string[] _loggedinPaytype = new string[] { "r1b1" };

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Gateway(PaymentRedirectModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            if (!model.IsValid())
            {
                ModelState.AddModelError("ContractAccounts", "Please ensure that amounts have been specified for all contract accounts");

                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            Context = model.PaymentContext;
            var lang = RequestLanguage.Code().ToUpperInvariant();
            if (string.IsNullOrEmpty(model.type))
            {
                Log.Error("Error:Payment Type null or empty", this);

                ModelState.AddModelError("Error", "Please ensure that amounts have been specified for all contract accounts");

                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            else if (model.type == "PayBill" || model.type == "RammasPayment")
            {
                //PayBill
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    c = model.c,
                    a = model.a,
                    u = CurrentPrincipal.UserId,
                    //sxid=CurrentPrincipal.SessionToken,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch,
                    epnum = model.epnum,
                    epf = model.epf
                };
                //For Tayseer Payment
                if (!string.IsNullOrEmpty(model.mto) && model.mto != "")
                {
                    _model.mto = model.mto;
                }
                string paymentpath = string.Empty;
                if (CacheProvider.TryGet(CacheKeys.PAYMENT_PATH, out paymentpath))
                {
                    if (!string.IsNullOrEmpty(paymentpath) && paymentpath.Equals("SmartWallet"))
                    {
                        _model.swal = "y";
                    }
                }
                string movetopaymentpath = string.Empty;
                if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out movetopaymentpath))
                {
                    if (!string.IsNullOrEmpty(movetopaymentpath) && movetopaymentpath.Equals("movetopaymentprocess"))
                    {
                        _model.mto = "A";
                    }
                }

                string evpaymentpath = string.Empty;
                if (CacheProvider.TryGet(CacheKeys.EV_ReplacePayment, out evpaymentpath))
                {
                    if (!string.IsNullOrEmpty(evpaymentpath))
                    {
                        _model.mto = "V";
                        _model.x = model.x;
                    }
                }

                if (CacheProvider.TryGet(CacheKeys.MOVEOUT_PAYMENT_ANONYMOUS_PATH, out paymentpath))
                {
                    if (paymentpath.Equals("MoveoutAnonymousPayment"))
                    {
                        _model.u = model.u;
                        _model.b = model.b;
                    }
                }
                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    //_model.EPayUrl = "https://portaltest.dewa.gov.ae/irj/servlet/prt/portal/prtroot/NoqodiPay.DewaNoqodiGateway?rd=webdojo";
                    _model.u = CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);
#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }

                Log.Info("Payment DNS:" + Request.Url.DnsSafeHost, this);
                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "MoveOut")
            {
                //MoveOut
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    c = model.c,
                    a = model.a,
                    u = CurrentPrincipal.UserId,
                    //sxid = CurrentPrincipal.SessionToken,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch
                };
                if (!string.IsNullOrEmpty(model.x))
                {
                    _model.x = model.x;
                }
                //_model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    _model.u = CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);
#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }

                Log.Info("Payment DNS:" + Request.Url.DnsSafeHost, this);

                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "ServiceActivation")
            {
                //ServiceActivation
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    c = model.c,
                    a = model.a,
                    u = model.u,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    em = model.em,
                    mb = model.mb,
                    b = model.b,
                    pnuser = model.pnuser,
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch,
                    epnum = model.epnum,
                    epf = model.epf
                };
                string movetopaymentpath = string.Empty;
                if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out movetopaymentpath))
                {
                    if (!string.IsNullOrEmpty(movetopaymentpath) && movetopaymentpath.Equals("movetopaymentprocess"))
                    {
                        _model.mto = "A";
                    }
                }
                // _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    _model.u = _model.u ?? CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);
#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }
                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "ReraServiceActivation")
            {
                //ReraServiceActivation
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    a = model.a,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    c = model.c,
                    em = model.em,
                    u = model.u,
                    mb = model.mb,
                    b = model.b,
                    pnuser = model.pnuser,
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch
                };
                _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "Clearance")
            {
                //Clearance
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    a = model.a,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    c = model.c,
                    em = model.em,
                    u = model.u,
                    mb = model.mb,
                    x = model.x,
                    b = model.b,
                    emid = model.emid,
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch
                };
                //_model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    _model.u = _model.u ?? CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);
#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }
                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "EstimatePayment")
            {
                //EstimatePayment
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    a = model.a,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    c = model.c,
                    u = CurrentPrincipal.UserId,
                    //sxid = CurrentPrincipal.SessionToken,
                    b = model.b,
                    em = model.em,
                    est = model.est,
                    cons = model.cons,
                    owner = model.owner,
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch,
                    epnum = model.epnum,
                    epf = model.epf
                };
                // _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    _model.u = CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);
#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }
                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "Miscellaneous")
            {
                //EstimatePayment
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    a = model.a,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    c = model.c,
                    u = CurrentPrincipal.UserId,
                    b = model.b,
                    em = model.em,
                    mb = model.mb,
                    ntf = model.ntf,
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch,
                    epnum = model.epnum,
                    epf = model.epf
                };
                //_model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    _model.u = CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);

#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }

                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "TemporaryConnection")
            {
                //TemporaryConnection
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    a = model.a.ToString() ?? string.Empty,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    c = model.c,
                    u = CurrentPrincipal.UserId,
                    //sxid = CurrentPrincipal.SessionToken,
                    b = model.b,
                    em = model.em,
                    ntf = model.ntf,
                    mb = model.mb.AddMobileNumberZeroPrefix(),
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch
                };
                //_model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    _model.u = CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);

#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }
                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            else if (model.type == "EVCard")
            {
                //EV card
                PaymentRedirectModel _model = new PaymentRedirectModel()
                {
                    c = model.c,
                    a = model.a,
                    u = !string.IsNullOrWhiteSpace(CurrentPrincipal.UserId) ? CurrentPrincipal.UserId : model.u,
                    //sxid=CurrentPrincipal.SessionToken,
                    t = model.PaymentContext.EPayTransactionCode(Request.Segment()),
                    d = "STATIC",
                    nn = DateTime.UtcNow.ToString("yyyyMMdd"),
                    l = lang,
                    branch = _branch,
                    epnum = model.epnum,
                    epf = model.epf
                };
                string evpaymentpath = string.Empty;
                if (CacheProvider.TryGet(CacheKeys.EV_ReplacePayment, out evpaymentpath))
                {
                    if (!string.IsNullOrEmpty(evpaymentpath))
                    {
                        _model.mto = "V";
                        _model.x = model.x;
                    }
                }
                // _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);

                // Payment Method i.e. epay, Noqodi, Apple pay
                if (!string.IsNullOrEmpty(model.paymode))
                {
                    _model.paymode = model.paymode;
                    _model.u = _model.u ?? CurrentPrincipal.UserId;
                    _model.sxid = CurrentPrincipal.SessionToken;

                    if (Request.Segment() == RequestSegment.Desktop)
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_DESKTOP"];
                    }
                    else
                    {
                        _model.vendorid = System.Configuration.ConfigurationManager.AppSettings["DEWA_VENDOR_ID_MOBILE"];
                    }
                    //TODO remove this line once test is done
                    _model.EPayUrl = GetNoqodiPayURL(Request.Url.DnsSafeHost);

#if DEBUG
                    Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(_model), this);
#endif
                }
                else
                {
                    _model.EPayUrl = GetEPayURL(Request.Url.DnsSafeHost);
                }
                return View("~/Views/Feature/Payment/Payment/PaymentRedirect.cshtml", _model);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RedirectForTemporaryConnection(TemporaryConnectionPaymentRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            Context = model.PaymentContext;

            #region [MIM Payment Implementation]

            var payRequest = new CipherPaymentModel();
            payRequest.PaymentData.amounts = Convert.ToString(model.Amount);
            payRequest.PaymentData.contractaccounts = model.ContractAccount ?? model.NotificationReference;
            payRequest.PaymentData.businesspartner = model.BusinessPartnerNumber;
            payRequest.PaymentData.email = model.EmailAddress;
            payRequest.PaymentData.mobile = model.MobileNumber?.AddMobileNumberZeroPrefix();
            payRequest.PaymentData.notificationnumber = model.NotificationReference;
            payRequest.ServiceType = ServiceType.TemporaryConnection;
            payRequest.PaymentMethod = model.paymentMethod;
            payRequest.IsThirdPartytransaction = false;
            payRequest.BankKey = model.bankkey;
            payRequest.SuqiaValue = model.SuqiaDonation;
            payRequest.SuqiaAmt = model.SuqiaDonationAmt;
            var payResponse = ExecutePaymentGateway(payRequest);
            if (Convert.ToInt32(payResponse.ErrorMessages?.Count) > 0)
            {
                foreach (KeyValuePair<string, string> errorItem in payResponse.ErrorMessages)
                {
                    ModelState.AddModelError(errorItem.Key, errorItem.Value);
                }
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }

            return View("~/Views/Feature/Payment/Payment/CipherPaymentSubmitform.cshtml", payResponse.PayPostModel);

            #endregion [MIM Payment Implementation]
        }

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult Confirmation(EPayResponseModel model)
        {
            string _lastDewaToken;
            bool IsValidPayment = Convert.ToBoolean(CacheProvider.TryGet(CacheKeys.LastestDewaToken, out _lastDewaToken) &&
                                                     !string.IsNullOrWhiteSpace(_lastDewaToken) && _lastDewaToken?.ToLower() == model.dewatoken?.ToLower());

            var context = Context ?? PaymentContextExtensions.Parse(model.ptype);
            var completionModel = new PaymentCompletionModel(context, model.s == EPayResponse.Success)
            {
                DegTransactionId = model.g,
                DewaTransactionId = model.t,
                PaymentDate = model.PaymentDate,
                Message = model.s == EPayResponse.Success ? GetSuccessMessage(context) : GetErrorMessage(model.g),
                Epnumber = model.epnum,
                Eptype = model.epf,
                pgspending = Convert.ToBoolean(model.pgspending == "X")
            };

            #region [Dewa payment]

            if (Convert.ToInt32(_loggedinPaytype.Where(x => x == model.ptype)?.Count() ?? 0) > 0 && !IsLoggedIn)
            {
                var currentUrl = HttpContext.Request.Url != null ?
                        HttpContext.Request.Url.PathAndQuery : string.Empty;
                return Redirect($"{LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE)}?returnUrl={System.Web.HttpUtility.UrlEncode(currentUrl)}");
            }

            if (completionModel.Succeeded && !IsValidPayment)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.TRANSACTION_HISTORY_PAGE);
            }

            #endregion [Dewa payment]

            #region [pgs Pending payment Logic]

            if (completionModel.pgspending && !string.IsNullOrWhiteSpace(completionModel.DegTransactionId))
            {

                CacheProvider.Store(CacheKeys.UAEPGS_PaymentPending + completionModel.DegTransactionId, new CacheItem<PaymentCompletionModel>(completionModel, TimeSpan.FromMinutes(10)));
                QueryString q = new QueryString();
                q.With("g", completionModel.DegTransactionId);
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.Payment_Pending, q);
            }
            #endregion
            // Payment through Rammas Facebook
            RammasGateWayResponse _rammaspara;
            if (CacheProvider.TryGet(CacheKeys.RAMMAS_LOGIN, out _rammaspara))
            {
                string error;
                if (_rammaspara != null)
                {
                    RammasGateWayResponse rammasGateWayResponse = new RammasGateWayResponse();
                    //rammasGateWayResponse.Message = completionModel.Message;
                    rammasGateWayResponse.Message = model.s.ToString().ToLower();
                    rammasGateWayResponse.TransactionId = completionModel.DewaTransactionId;
                    rammasGateWayResponse.ConversationId = _rammaspara.ConversationId;

                    if (!RammasParameter.EpayRedirect(rammasGateWayResponse, out error))
                    {
                        ViewBag.RammasErrorMessage = error;
                        //return RedirectToContextualConfirmationView(completionModel);
                    }
                }
            }
            // End

            // Payment through Rammas Directline
            string _rammas_conv_key;
            if (CacheProvider.TryGet(CacheKeys.RAMMAS_TRANSACTION, out _rammas_conv_key))
            {
                string error;
                if (_rammas_conv_key != null)
                {
                    RammasGateWayResponse rammasGateWayResponse = new RammasGateWayResponse();
                    rammasGateWayResponse.Message = model.s.ToString().ToLower();
                    rammasGateWayResponse.TransactionId = completionModel.DewaTransactionId;
                    rammasGateWayResponse.ConversationId = _rammas_conv_key;

                    if (!RammasParameter.EpayRedirect(rammasGateWayResponse, out error))
                    {
                        ViewBag.RammasErrorMessage = error;
                        //return RedirectToContextualConfirmationView(completionModel);
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.RAMMAS_TRANSACTION_SUCCESS, new CacheItem<string>(_rammas_conv_key));

                        ViewBag.RammasPaymentDone = true;
                    }
                    CacheProvider.Remove(CacheKeys.RAMMAS_TRANSACTION);
                }
            }
            // End
            CacheProvider.Remove(CacheKeys.SELECTED_TRANSACTION);
            if (IsLoggedIn && completionModel.Succeeded && !string.IsNullOrWhiteSpace(completionModel.DegTransactionId))
            {
                var receipts = DewaApiClient.GetReceipts(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.g, model.t, RequestLanguage, Request.Segment());
                if (receipts.Succeeded && receipts.Payload.Any())
                {
                    IEnumerable<Account> paymentAccounts = null;

                    CacheProvider.TryGet(CacheKeys.PAYMENT_ACCOUNTS_METADATA, out paymentAccounts);

                    if (paymentAccounts != null && string.IsNullOrWhiteSpace(model.epf))
                    {
                        var mapped = MapReceipts(receipts.Payload, paymentAccounts);

                        // Setup viewbag for happiness indicator
                        ViewBag.IndicatorType = IndicatorType.Transaction;
                        ViewBag.TransactionId = model.t;
                        completionModel.Receipts = mapped;
                        //completionModel = new PaymentCompletionModel(context, true)
                        //{
                        //	Receipts = mapped,
                        //	DegTransactionId = model.g,
                        //	DewaTransactionId = model.t,
                        //	PaymentDate = model.PaymentDate,
                        //	Message = GetSuccessMessage(context)
                        //};
                    }
                    else
                    {
                        if (receipts.Payload.Any())
                        {
                            var total = receipts.Payload.Select(x => x.Amount).Sum();
                            completionModel.Total = total;
                            completionModel.SuqiaAmount = receipts.Payload.Any(x => !string.IsNullOrWhiteSpace(x.PaymentGatewayReconStatus)) ? decimal.Parse(receipts.Payload.Select(x => x.PaymentGatewayReconStatus).FirstOrDefault()) : 0;
                            completionModel.ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.g);
                            string paymentpath = string.Empty;
                            if (CacheProvider.TryGet(CacheKeys.MOVEOUT_PAYMENT_PATH, out paymentpath))
                            {
                                if (paymentpath.Equals("MoveoutPayment"))
                                {
                                    MoveOutState state;
                                    CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state);

                                    completionModel.LstReceipts = new List<ReceiptModel>();
                                    receipts.Payload.ToList().ForEach(x => completionModel.LstReceipts.Add(new ReceiptModel()
                                    {
                                        ReceiptAccountName =
                                        (state.moveoutdetails.accountslist.Where(y => y.contractaccountnumber == "00" + x.AccountNumber).Any() ? state.moveoutdetails.accountslist.Where(y => y.contractaccountnumber == "00" + x.AccountNumber).FirstOrDefault().contractaccountname : state.moveoutdetails.divisionlist.Where(y => y.contractaccountnumber == "00" + x.AccountNumber).Any() ? state.moveoutdetails.divisionlist.Where(y => y.contractaccountnumber == "00" + x.AccountNumber).FirstOrDefault().contractaccountname : string.Empty),
                                        DewaTransactionReference = x.DewaTransactionReference,
                                        BusinessPartnerNumber = x.BusinessPartnerNumber,
                                        AccountNumber = x.AccountNumber,
                                        ReceiptId = x.ReceiptId,
                                        Amount = x.Amount
                                    }));
                                    completionModel.AffectedAccounts = string.Join(", ", receipts.Payload.Select(x => x.AccountNumber));
                                    completionModel.BusinessPartners = receipts.Payload.Select(x => x.BusinessPartnerNumber).FirstOrDefault();
                                }
                            }
                            string movetopaymentpath = string.Empty;
                            if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out movetopaymentpath))
                            {
                                if (!string.IsNullOrEmpty(movetopaymentpath) && movetopaymentpath.Equals("movetopaymentprocess"))
                                {
                                    completionModel.LstReceipts = new List<ReceiptModel>();
                                    receipts.Payload.ToList().ForEach(x => completionModel.LstReceipts.Add(new ReceiptModel() { DewaTransactionReference = x.DewaTransactionReference, BusinessPartnerNumber = x.BusinessPartnerNumber, AccountNumber = x.AccountNumber, ReceiptId = x.ReceiptId, Amount = x.Amount }));
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToContextualConfirmationView(completionModel);
        }

        [HttpGet]
        public ActionResult Pending(string g)
        {
            PaymentCompletionModel model;

            if (!CacheProvider.TryGet(CacheKeys.UAEPGS_PaymentPending + g, out model) || model == null)
            {
                ViewBag.PaymentSubmittedErorr = string.Format(Translate.Text("Payment failed"), g);
            }
            else
            {
                string _amount;
                try
                {
                    if (CacheProvider.TryGet(CacheKeys.UAEPGS_PaymentAmount, out _amount))
                    {
                        model.pgsTotal = _amount.Split(',').Sum(x => Convert.ToDecimal(x));
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                }
            }
            return PartialView("~/Views/Feature/Payment/Payment/_Pending.cshtml", model);
        }

        #region Helper methods

        private ActionResult RedirectToContextualConfirmationView(PaymentCompletionModel model)
        {
            if (!model.Succeeded)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Payment failed", model.DegTransactionId));
            }

            if (model.Eptype == "X")
            {
                return RenderEasyPayConfirmation(model);
            }

            if (model.Eptype == "T")
            {
                return RenderToTenderPayConfirmation(model);
            }
            if (model.Eptype == "BT")
            {
                return RenderToTenderBidPayConfirmation(model);
            }
            if (model.Eptype == "SO")
            {
                return RenderToSalesOrderPayConfirmation(model);
            }

            string paymentpath = null;
            if (model.Eptype == DewaPaymentChannel.RR || (CacheProvider.TryGet(CacheKeys.PAYMENT_PATH, out paymentpath) && paymentpath == CacheKeys.IBANDetail_PaymentData))
            {
                return RenderRefundRecoveryConfirmation(model);
            }
            switch (model.Context)
            {
                case PaymentContext.TemporaryConnection:
                    return RenderTemporaryConnectionConfirmation(model);

                case PaymentContext.PayFriendsBill:
                    return RenderFriendPaymentConfirmation(model);

                case PaymentContext.PayMyEstimate:
                    return RedirectToMyEstimatePaymentConfirmation(model);

                case PaymentContext.PayFriendsEstimate:
                    return RedirectToFriendEstimatePaymentConfirmation(model);

                case PaymentContext.ServiceActivation:
                case PaymentContext.AnonymousServiceActivation:
                    return RenderMoveInConfirmation(model);

                case PaymentContext.Clearance:
                case PaymentContext.AnonymousClearance:
                    return RenderClearanceCertificateConfirmation(model);

                case PaymentContext.ReraServiceActivation:
                case PaymentContext.AnonymousReraServiceActivation:
                    return ReraMoveInPaymentConfirmation(model);

                case PaymentContext.EVCard:
                    return RedirectToEVconfirmation(model);

                case PaymentContext.EVAnonymous:
                    return RedirectToEVAnonymousconfirmation(model);

                default:
                    return RenderBillPaymentConfirmation(model);
            }
        }

        private ActionResult RenderBillPaymentConfirmation(PaymentCompletionModel model)
        {
            PaymentMetaDataModel metadata;
            GetEasyPayEnquiryResponse easypaymentpath;
            if (CacheProvider.TryGet(CacheKeys.PAYMENT_METADATA, out metadata))
            {
                model.BusinessPartners = !string.IsNullOrWhiteSpace(model.BusinessPartners) ? model.BusinessPartners : string.Join(", ", metadata.BusinessPartnerNumbers);
                model.AffectedAccounts = !string.IsNullOrWhiteSpace(model.AffectedAccounts) ? model.AffectedAccounts : string.Join(", ", metadata.ContractAccountNumbers);
            }

            if (model.Succeeded)
            {
                ServiceResponse<AccountDetails[]> response;

                if (CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST_WITH_BILLING, out response))
                    CacheProvider.Remove(CacheKeys.ACCOUNT_LIST_WITH_BILLING);
                else if (CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST, out response))
                    CacheProvider.Remove(CacheKeys.ACCOUNT_LIST);

                string paymentpath = string.Empty;

                Dictionary<string, string> dictionary;
                if (CacheProvider.TryGet(CacheKeys.PAYMENT_PATH, out paymentpath))
                {
                    if (paymentpath.Equals("SmartWallet"))
                    {
                        List<subscribeDetails> lstAccounts = new List<subscribeDetails>();
                        foreach (string contractAccountNumber in metadata.ContractAccountNumbers)
                        {
                            subscribeDetails acc1 = new subscribeDetails
                            {
                                contractAccount = contractAccountNumber
                            };
                            lstAccounts.Add(acc1);
                        }
                        var response1 = DewaApiClient.SaveSmartWalletSubscription(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, "Y", lstAccounts.ToArray(), RequestLanguage, Request.Segment());
                    }
                }
                else if (CacheProvider.TryGet(CacheKeys.MOVEOUT_PAYMENT_PATH, out paymentpath))
                {
                    if (paymentpath.Equals("MoveoutPayment"))
                    {
                        MoveOutState state;
                        if (CacheProvider.TryGet(CacheKeys.MOVE_OUT_RESULT, out state))
                        {
                            state.moveoutresult.proceed = true;
                            CacheProvider.Store(CacheKeys.MOVE_OUT_RESULT, new CacheItem<MoveOutState>(state, TimeSpan.FromMinutes(20)));
                            CacheProvider.Remove(CacheKeys.MOVEOUT_PAYMENT_PATH);
                            return PartialView("~/Views/Feature/SupplyManagement/MoveOut/_MoveOutPayment.cshtml", model);
                        }
                    }
                }
                else if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out paymentpath))
                {
                    if (!string.IsNullOrEmpty(paymentpath) && paymentpath.Equals("movetopaymentprocess"))
                    {
                        MoveToWorkflowState movetostate;
                        if (CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out movetostate))
                        {
                            CacheProvider.Remove(CacheKeys.MOVETOPAYMENT);
                            return PartialView("~/Views/Feature/SupplyManagement/MoveTo/_MoveToPayment.cshtml", new ServiceActivationPaymentCompletionModel(model.Context, model.Succeeded)
                            {
                                DegTransactionId = model.DegTransactionId,
                                DewaTransactionId = model.DewaTransactionId,
                                PaymentDate = model.PaymentDate,
                                PaymentAmount = model.Total,
                                Message = model.Message,
                                Receipts = model.Receipts,
                                PremiseNo = movetostate.PremiseAccount,
                                BusinessPartner = movetostate.BusinessPartner,
                                LstReceipts = model.LstReceipts,
                                Total = model.Total,
                                SuqiaAmount = model.SuqiaAmount
                            });
                        }
                    }
                }
                else if (CacheProvider.TryGet(CacheKeys.CLEARANCE_PAYMENT_PATH, out dictionary))
                {
                    CipherPaymentModel _upmodel = new CipherPaymentModel();
                    CacheProvider.TryGet(CacheKeys.CLEARANCE_PAYMENT_MODEL, out _upmodel);
                    model.BusinessPartners = _upmodel.PaymentData.businesspartner;
                    model.ContractAccountNumber = _upmodel.PaymentData.contractaccounts;
                    ViewBag.returnurl = dictionary.FirstOrDefault().Value;
                    CacheProvider.Remove(CacheKeys.CLEARANCE_PAYMENT_PATH);
                    CacheProvider.TryGet(CacheKeys.CLEARANCE_PAYMENT_Details_Propertyseller, out ClearanceCertificatePropertySeller clearanceCertificatePropertySeller);
                    if (clearanceCertificatePropertySeller != null)
                    {
                        CacheProvider.Store(CacheKeys.CLEARANCE_CERTIFICATE_FinalFlow, new CacheItem<string>(clearanceCertificatePropertySeller.ContractAccountNumber));
                        CacheProvider.Remove(CacheKeys.CLEARANCE_PAYMENT_Details_Propertyseller);
                    }
                    else
                    {
                        CacheProvider.Store(CacheKeys.CLEARANCE_CERTIFICATE_FinalFlow, new CacheItem<string>(_upmodel.PaymentData.contractaccounts));
                    }
                    return PartialView("~/Views/Feature/Bills/ClearanceCertificate/_ClearanceCertificatePayment.cshtml", model);
                }
                ///Added by Shujaat ali for moveout anonymous
                else if (CacheProvider.TryGet(CacheKeys.MOVEOUT_PAYMENT_ANONYMOUS_PATH, out paymentpath))
                {
                    if (Config.IsSecuredMIMEnabled)
                    {
                        CipherPaymentModel _pyModel = new CipherPaymentModel();
                        CacheProvider.TryGet(CacheKeys.MOVEOUT_ANONYMOUS_PAYMENT_MODEL, out _pyModel);
                        MoveOutAnonymous anoymousmodel = new MoveOutAnonymous();
                        CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out anoymousmodel);
                        model.ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                        model.BusinessPartners = _pyModel.PaymentData.businesspartner;
                        model.ContractAccountNumber = _pyModel.PaymentData.contractaccounts;
                        model.Total = Convert.ToDecimal(_pyModel.PaymentData.amounts);
                        if (!string.IsNullOrWhiteSpace(_pyModel.PaymentData.suqiaamt))
                        {
                            model.SuqiaAmount = Convert.ToDecimal(_pyModel.PaymentData.suqiaamt);
                        }
                        model.LstReceipts = new List<ReceiptModel>
                    {
                        new ReceiptModel
                        {
                            ReceiptAccountName = anoymousmodel.BusinessPartnerName,
                            AccountNumber = _pyModel.PaymentData.contractaccounts,
                            Amount = Convert.ToDecimal(_pyModel.PaymentData.amounts)
                        }
                    };
                    }
                    else
                    {
                        PaymentRedirectModel _model = new PaymentRedirectModel();
                        CacheProvider.TryGet(CacheKeys.MOVEOUT_ANONYMOUS_PAYMENT_MODEL, out _model);
                        MoveOutAnonymous anoymousmodel = new MoveOutAnonymous();
                        CacheProvider.TryGet(CacheKeys.MOVEOUT_OTP_RESPONSE, out anoymousmodel);
                        model.ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                        model.BusinessPartners = _model.b;
                        model.ContractAccountNumber = _model.c;
                        model.Total = Convert.ToDecimal(_model.a);
                        model.LstReceipts = new List<ReceiptModel>
                    {
                        new ReceiptModel
                        {
                            ReceiptAccountName = anoymousmodel.BusinessPartnerName,
                            AccountNumber = _model.c,
                            Amount = Convert.ToDecimal(_model.a)
                        }
                    };
                    }
                    CacheProvider.Remove(CacheKeys.MOVEOUT_PAYMENT_ANONYMOUS_PATH);
                    return PartialView("~/Views/Feature/SupplyManagement/MoveOut/MoveOutAnonymous/_MoveOutPayment.cshtml", model);
                }

                return PartialView("~/Views/Feature/Payment/Payment/_Confirmation.cshtml", model);
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Exactly(2)));

            string paymentpatherror = string.Empty;
            var dict = new Dictionary<string, string>();

            if (CacheProvider.TryGet(CacheKeys.PAYMENT_PATH, out paymentpatherror))
            {
                if (paymentpatherror.Equals("SmartWallet"))
                {
                    CacheProvider.Remove(CacheKeys.PAYMENT_PATH);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.M51_SMARTWALLET_SELECTACCOUNT);
                }
            }
            else if (CacheProvider.TryGet(CacheKeys.MOVEOUT_PAYMENT_PATH, out paymentpatherror))
            {
                if (paymentpatherror.Equals("MoveoutPayment"))
                {
                    CacheProvider.Remove(CacheKeys.MOVEOUT_PAYMENT_PATH);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_OUT_START_V3);
                }
            }
            else if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out paymentpatherror))
            {
                if (!string.IsNullOrEmpty(paymentpatherror) && paymentpatherror.Equals("movetopaymentprocess"))
                {
                    CacheProvider.Remove(CacheKeys.MOVETOPAYMENT);
                    MoveToWorkflowState State;
                    if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out State))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
                    }
                    State.page = "details";
                    CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(State, TimeSpan.FromMinutes(20)));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START_DETAILS);
                }
            }
            else if (CacheProvider.TryGet(CacheKeys.CLEARANCE_PAYMENT_PATH, out dict))
            {
                if (!string.IsNullOrEmpty(dict.FirstOrDefault().Value))
                {
                    CacheProvider.Remove(CacheKeys.CLEARANCE_PAYMENT_PATH);
                    return RedirectToSitecoreItem(dict.FirstOrDefault().Value);
                }
            }


            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J14_PAY_BILLS);
        }

        private ActionResult RenderClearanceCertificateConfirmation(PaymentCompletionModel model)
        {
            if (model.Succeeded)
            {
                ClearanceCertificatePaymentModel payModel;
                if (CacheProvider.TryGet(CacheKeys.CLEARANCE_PAYMENT_DETAILS, out payModel))
                {
                    model.Total = payModel.Amount;
                    model.ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                    model.SuqiaAmount = model.SuqiaAmount;
                    model.BusinessPartners = payModel.BusinessPartnerNumber;
                    model.ContractAccountNumber = payModel.ContractAccountNumber;

                    model.ContractAccountNumber = this.IsLoggedIn ? payModel.ContractAccountNumber : string.Empty;
                    return PartialView("~/Views/Feature/Payment/Payment/_ClearanceCertificateConfirmation.cshtml", model);
                }
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));
            //CacheProvider.Store(CacheKeys.CLEARANCE_FAILED, new CacheItem<string>(model.Message));
            if (this.IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J26_REQUEST_POST_LOGIN);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J26_NEW_REQUEST);
            }
        }

        private ActionResult RenderMoveInConfirmation(PaymentCompletionModel model)
        {
            CipherPaymentModel cipherPaymentModel = new CipherPaymentModel();
            if (model.SuqiaAmount<=0 && CacheProvider.TryGet(CacheKeys.MOVEIN_MIM_MODEL, out cipherPaymentModel) && !string.IsNullOrWhiteSpace(cipherPaymentModel.SuqiaAmt))
            {
                model.SuqiaAmount = Convert.ToDecimal(cipherPaymentModel.SuqiaAmt);
            }
            string paymentrera;
            if (CacheProvider.TryGet(CacheKeys.PaymentReraCacheKey, out paymentrera))
            {
                if (!string.IsNullOrEmpty(paymentrera) && paymentrera.Equals("RERAMovein"))
                {
                    if (model.Succeeded)
                    {
                        ReraPaymentModel payModel;
                        if (CacheProvider.TryGet(CacheKeys.PaymentCacheKey, out payModel))
                        {
                            var confirmationModel = new ServiceActivationPaymentCompletionModel(model.Context, model.Succeeded)
                            {
                                Total = (decimal)payModel.Total,
                                DegTransactionId = model.DegTransactionId,
                                DewaTransactionId = model.DewaTransactionId,
                                PaymentDate = model.PaymentDate,
                                ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.DegTransactionId),
                                Message = model.Message,
                                Receipts = model.Receipts,
                                BusinessPartners = payModel.BusinessPartner,
                                ContractAccountNumber = payModel.ContractAccountNumber,
                                Whatsnexttext = payModel.messagewhatsnext,
                                SuqiaAmount = model.SuqiaAmount
                            };
                            CacheProvider.Remove(CacheKeys.PaymentReraCacheKey);
                            return PartialView("~/Views/Feature/Payment/Payment/_ReraMoveInPaymentConfirmation.cshtml", confirmationModel);
                        }
                    }

                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));

                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J73_RERA_PAYMENT_DETAILS);
                }
            }
            string movetopaymentpath = string.Empty;
            bool movein = true;
            if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out movetopaymentpath))
            {
                if (!string.IsNullOrEmpty(movetopaymentpath) && movetopaymentpath.Equals("movetopaymentprocess"))
                {
                    movein = false;
                }
            }
            MoveInViewModelv3 state;
            if (model.Succeeded && CacheProvider.TryGet(CacheKeys.MOVE_IN_3_WORKFLOW_STATE, out state) && movein)
            {
                var total = (decimal.Parse(state.SecurityDeposit ?? "0.00") + decimal.Parse(state.ReconnectionRegistrationFee ?? "0.00")
                            + decimal.Parse(state.KnowledgeFee ?? "0.00") + decimal.Parse(state.TotalOutstandingFee ?? "0.00") + decimal.Parse(state.InnovationFee ?? "0.00")
                            + decimal.Parse(state.AddressRegistrationFee ?? "0.00") + decimal.Parse(state.ReconnectionVATamt ?? "0.00")
                            + decimal.Parse(state.AddressVAtamt ?? "0.00"));

                var confirmationModel = new ServiceActivationPaymentCompletionModel(model.Context, model.Succeeded)
                {
                    DegTransactionId = model.DegTransactionId,
                    DewaTransactionId = model.DewaTransactionId,
                    PaymentDate = model.PaymentDate,
                    SuqiaAmount = model.SuqiaAmount,
                    PaymentAmount = total,
                    Message = model.Message,
                    Receipts = model.Receipts,
                    PremiseNo = string.Join(", ", state.PremiseAccount.ToList()),
                    BusinessPartner = state.BusinessPartner,
                    ContractAccountNumber = string.Join(", ", state.ContractAccountnumber.ToList()),
                    landloardmsg = false,
                    Whatsnexttext = state.messagewhatsnext,
                    MoveInNotificationNumber = state.MoveInNotificationNumber,
                    premiseAmountDetails = state.premiseAmountDetails.ToList(),
                    FullName = state.FirstName + " " + ((!string.IsNullOrWhiteSpace(state.LastName) && !state.LastName.Equals(".")) ? state.LastName : string.Empty)
                };
                ViewBag.IsLoggedIn = this.IsLoggedIn;
                if (state.Owner && !state.occupiedbyowner)
                {
                    confirmationModel.landloardmsg = true;
                }
                CacheProvider.Remove(CacheKeys.MOVE_IN_3_WORKFLOW_STATE);
                CacheProvider.Remove(CacheKeys.MOVEIN_JOURNEY);
                if (movein)
                {
                    return PartialView("~/Views/Feature/SupplyManagement/MoveIn/Confirmation.cshtml", confirmationModel);
                }
                else
                {
                    return PartialView("~/Views/Feature/SupplyManagement/MoveTo/Confirmation.cshtml", confirmationModel);
                }
            }
            MoveToWorkflowState movetostate;
            if (model.Succeeded && CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out movetostate) && !movein)
            {
                var confirmationModelmoveto = new ServiceActivationPaymentCompletionModel(model.Context, model.Succeeded)
                {
                    DegTransactionId = model.DegTransactionId,
                    DewaTransactionId = model.DewaTransactionId,
                    PaymentDate = model.PaymentDate,
                    SuqiaAmount = model.SuqiaAmount,
                    PaymentAmount = model.Total,
                    Message = model.Message,
                    Receipts = model.Receipts,
                    PremiseNo = string.Join(", ", movetostate.PremiseAccount.ToList()),
                    BusinessPartner = movetostate.BusinessPartner,
                    LstReceipts = model.LstReceipts
                };
                CacheProvider.Remove(CacheKeys.MOVE_TO_WORKFLOW_STATE);
                return PartialView("~/Views/Feature/SupplyManagement/MoveTo/_MoveToPayment.cshtml", confirmationModelmoveto);
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));
            if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out movetopaymentpath))
            {
                if (!string.IsNullOrEmpty(movetopaymentpath) && movetopaymentpath.Equals("movetopaymentprocess"))
                {
                    MoveToWorkflowState State;
                    if (!CacheProvider.TryGet(CacheKeys.MOVE_TO_WORKFLOW_STATE, out State))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START);
                    }
                    State.page = "details";
                    CacheProvider.Store(CacheKeys.MOVE_TO_WORKFLOW_STATE, new CacheItem<MoveToWorkflowState>(State, TimeSpan.FromMinutes(20)));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.MOVE_TO_START_DETAILS);
                }
            }
            if (IsLoggedIn)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.LOGIN_MOVEIN_PAYMENT_PAGEv3);
            }
        }

        private ActionResult RenderFriendPaymentConfirmation(PaymentCompletionModel model)
        {
            PaymentMetaDataModel metadata;
            if (CacheProvider.TryGet(CacheKeys.PAYMENT_METADATA, out metadata))
            {
                model.BusinessPartners = !string.IsNullOrWhiteSpace(model.BusinessPartners) ? model.BusinessPartners : string.Join(", ", metadata.BusinessPartnerNumbers);
                model.AffectedAccounts = !string.IsNullOrWhiteSpace(model.AffectedAccounts) ? model.AffectedAccounts : string.Join(", ", metadata.ContractAccountNumbers);
            }

            if (model.Succeeded)
            {
                return PartialView("~/Views/Feature/Payment/Payment/_FriendConfirmation.cshtml", model);
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J15_PAY_A_FRIENDS_BILL);
        }

        private ActionResult RenderEasyPayConfirmation(PaymentCompletionModel model)
        {
            GetEasyPayEnquiryResponse easypaymentpath;
            bool easyPayTransactionType = false;
            CacheProvider.TryGet(CacheKeys.Tayseer_EasyPay_Transaction_Type, out easyPayTransactionType);

            if (model.Succeeded)
            {
                if (CacheProvider.TryGet(CacheKeys.Easy_Pay_Response, out easypaymentpath))
                {
                    if (easypaymentpath.@return.transactiontype != null && (easypaymentpath.@return.transactiontype == "ESTMNM" || easypaymentpath.@return.transactiontype == "KWP2" || easypaymentpath.@return.transactiontype == "MSLPAY"))
                    {
                        model.Total = 0;
                        decimal estimateamount;
                        if (CacheProvider.TryGet(CacheKeys.Easy_Pay_Estimate, out estimateamount))
                        {
                            model.Total = estimateamount;
                        }
                        model.ReceiptIdentifiers = string.Format("{0:yyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                    }
                    model.Epnumber = easypaymentpath.@return.easypaynumber;
                    model.Eptype = easypaymentpath.@return.transactiondescription;
                    model.TayseerHistoryPayment = easyPayTransactionType;
                    //CacheProvider.Remove(CacheKeys.Easy_Pay_Response);
                    return PartialView("~/Views/Feature/Bills/EasyPay/EasyPayPayment.cshtml", model);
                }
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));

            string paymentpatherror;
            if (CacheProvider.TryGet(CacheKeys.TAYSEER_HISTORY_PAYMENT_PATH, out paymentpatherror))
            {
                if (!string.IsNullOrEmpty(paymentpatherror) && paymentpatherror.Equals("tayseerhistorypayment"))
                {
                    CacheProvider.Remove(CacheKeys.TAYSEER_HISTORY_PAYMENT_PATH);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.TAYSEER_REF_NO_HISTORY);
                }
            }
            else if (CacheProvider.TryGet(CacheKeys.TAYSEER_PAYMENT_PATH, out paymentpatherror))
            {
                if (!string.IsNullOrEmpty(paymentpatherror) && paymentpatherror.Equals("tayseerpaymentpath"))
                {
                    CacheProvider.Remove(CacheKeys.TAYSEER_PAYMENT_PATH);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.TAYSEER_GENERATE_REF_NO);
                }
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EasyPay_Details);
        }

        private ActionResult RenderTemporaryConnectionConfirmation(PaymentCompletionModel model)
        {
            if (!model.Succeeded)
            {
                CacheProvider.Store(CacheKeys.TEMP_CON_REQ_FAILED, new CacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId)));

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_TRACK_PAY_REQUEST);
            }

            string reference;
            if (!CacheProvider.TryGet(CacheKeys.TEMP_CONN_REQ_REF, out reference))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SEARCH_REQUEST);
            }

            var response = DewaApiClient.GetTemporaryConnectionDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, reference, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                ViewBag.TempConnectionDetails = TrackTempConnectionRequestItem.From(response.Payload);

                return PartialView("~/Views/Feature/Payment/Payment/_TemporaryConnectionConfirmation.cshtml", model);
            }

            CacheProvider.Store(CacheKeys.TEMP_CON_REQ_FAILED, new CacheItem<string>(response.Message));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SUBMISSION_FAILED);
        }

        private ActionResult RedirectToMyEstimatePaymentConfirmation(PaymentCompletionModel model)
        {
            if (model.Succeeded)
            {
                CacheProvider.Store(CacheKeys.MY_ESTIMATE_PAYMENT_STATE, new CacheItem<PaymentCompletionModel>(model));

                var confirmationUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J81_ESTIMATE_PAYMENT_CONFIRMATION);
                var transactionParam = model.DewaTransactionId;
                var url = string.Format("{0}?t={1}&s={2}", confirmationUrl, transactionParam, model.Succeeded ? "success" : "failure");
                if (Url.IsLocalUrl(url))
                {
                    return Redirect(url);
                }
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J81_ESTIMATES_LANDING);
        }

        private ActionResult RedirectToFriendEstimatePaymentConfirmation(PaymentCompletionModel model)
        {
            if (model.Succeeded)
            {
                CacheProvider.Store(CacheKeys.FRIENDS_ESTIMATE_PAYMENT_STATE, new CacheItem<PaymentCompletionModel>(model));

                var confirmationUrl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J82_ESTIMATE_PAYMENT_CONFIRMATION);
                var transactionParam = model.DewaTransactionId;
                var url = string.Format("{0}?t={1}&s={2}", confirmationUrl, transactionParam, model.Succeeded ? "success" : "failure");
                if (Url.IsLocalUrl(url))
                {
                    return Redirect(url);
                }
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J82_ESTIMATE_LANDING);
        }

        private ActionResult ReraMoveInPaymentConfirmation(PaymentCompletionModel model)
        {
            CipherPaymentModel cipherPaymentModel = new CipherPaymentModel();
            if (model.SuqiaAmount <= 0 && CacheProvider.TryGet(CacheKeys.MOVEIN_RERA_MIM_MODEL, out cipherPaymentModel) && !string.IsNullOrWhiteSpace(cipherPaymentModel.SuqiaAmt))
            {
                model.SuqiaAmount = Convert.ToDecimal(cipherPaymentModel.SuqiaAmt);
            }

            if (model.Succeeded)
            {
                ReraPaymentModel payModel;
                if (CacheProvider.TryGet(CacheKeys.PaymentCacheKey, out payModel))
                {
                    var confirmationModel = new ServiceActivationPaymentCompletionModel(model.Context, model.Succeeded)
                    {
                        Total = (decimal)payModel.Total,
                        DegTransactionId = model.DegTransactionId,
                        DewaTransactionId = model.DewaTransactionId,
                        PaymentDate = model.PaymentDate,
                        SuqiaAmount = model.SuqiaAmount,
                        ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.DegTransactionId),
                        Message = model.Message,
                        Receipts = model.Receipts,
                        BusinessPartners = payModel.BusinessPartner,
                        ContractAccountNumber = payModel.ContractAccountNumber,
                        Whatsnexttext = payModel.messagewhatsnext,
                    };
                    return PartialView("~/Views/Feature/Payment/Payment/_ReraMoveInPaymentConfirmation.cshtml", model);
                }
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J73_RERA_PAYMENT_DETAILS);
        }

        private ActionResult RedirectToEVconfirmation(PaymentCompletionModel model)
        {
            if (CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST_WITH_BILLING, out ServiceResponse<AccountDetails[]> response))
            {
                CacheProvider.Remove(CacheKeys.ACCOUNT_LIST_WITH_BILLING);
            }
            else if (CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST, out response))
            {
                CacheProvider.Remove(CacheKeys.ACCOUNT_LIST);
            }

            string paymentpath = string.Empty;
            if (model.Succeeded)
            {
                if (CacheProvider.TryGet(CacheKeys.EV_CustomerTypePayment, out paymentpath))
                {
                    if (paymentpath.Equals("EV_CustomerTypePaymentLogin") || paymentpath.Equals("EV_CustomerTypePaymentNonDEWA"))
                    {
                        if (CacheProvider.TryGet(CacheKeys.EV_CustomerTypePaymentdetails, out List<DEWAXP.Foundation.Integration.APIHandler.Models.Response.ContractAccount> contractAccounts))
                        {
                            string requestnumber = string.Empty;
                            if (CacheProvider.TryGet(CacheKeys.EV_CustomerTypePaymentRequestNumber, out requestnumber))
                            {
                            }
                            EVLoginPaymentCompletionModel evmodel = new EVLoginPaymentCompletionModel(model.Context, model.Succeeded)
                            {
                                AccountDetailsList = contractAccounts,
                                Total = contractAccounts.Select(x => Convert.ToDecimal(string.IsNullOrWhiteSpace(x.totalAmount) ? "0" : x.totalAmount)).Sum(),
                                DegTransactionId = model.DegTransactionId,
                                DewaTransactionId = model.DewaTransactionId,
                                PaymentDate = model.PaymentDate,
                                SuqiaAmount = model.SuqiaAmount,
                                Message = model.Message,
                                Receipts = model.Receipts,
                                ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.DegTransactionId),
                                RequestNumber = requestnumber
                            };
                            CacheProvider.Remove(CacheKeys.EV_CustomerTypePayment);
                            CacheProvider.Remove(CacheKeys.EV_CustomerTypePaymentRequestNumber);
                            //CacheProvider.Remove(CacheKeys.EV_CustomerTypePaymentdetails);
                            return PartialView("~/Views/Feature/EV/EVCharger/ApplyEVCard_Confirms.cshtml", evmodel);
                        }
                    }
                }
                else if (CacheProvider.TryGet(CacheKeys.EVDEACTIVATE_PAYMENT_PATH, out paymentpath))
                {
                    if (paymentpath.Equals("evdeactivatePayment"))
                    {
                        if (CacheProvider.TryGet(CacheKeys.EV_DEACTIVATE_RESULT, out MoveOutState state))
                        {
                            model.ReceiptIdentifiers = string.Format("{0:yyyyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                            model.Total = Convert.ToDecimal(state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).FirstOrDefault().amounttocollect);
                            model.BusinessPartners = state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).FirstOrDefault().businesspartnernumber;
                            model.LstReceipts = new List<ReceiptModel>{
                                new ReceiptModel
                                {
                                    ReceiptAccountName = state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).FirstOrDefault().contractaccountname,
                                    AccountNumber = state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).FirstOrDefault().contractaccountnumber,
                                    Amount = Convert.ToDecimal(state.moveoutdetails.accountslist.Where(y => y.okpaymenttocollect.Equals("Y")).FirstOrDefault().amounttocollect)
                                }
                            };
                            state.moveoutresult.proceed = true;
                            state.page = new List<string>
                            {
                                evdeactivatestep.details.ToString()
                            };
                            if (!string.IsNullOrWhiteSpace(state.moveoutresult.evCardnumber))
                            {
                                CacheProvider.Store(CacheKeys.EV_SELECTEDCARD, new AccessCountingCacheItem<string>(state.moveoutresult.evCardnumber, Times.Once));
                            }
                            CacheProvider.Store(CacheKeys.EV_DEACTIVATE_RESULT, new CacheItem<MoveOutState>(state, TimeSpan.FromMinutes(20)));
                            CacheProvider.Remove(CacheKeys.EVDEACTIVATE_PAYMENT_PATH);
                            return PartialView("~/Views/Feature/EV/EVCharger/DeactivateCardPayment.cshtml", model);
                        }
                    }
                }
                else if (CacheProvider.TryGet(CacheKeys.EV_ReplacePayment, out paymentpath))
                {
                    EVReplaceCard replaceCard;
                    if (paymentpath.Equals("EV_ReplacePayment"))
                    {
                        CacheProvider.TryGet(CacheKeys.EV_ReplacePayment_AccountNumber, out replaceCard);
                        model.ContractAccountNumber = replaceCard.AccountNumber;
                        model.CardNumber = replaceCard.CardNumber;
                        model.RequestNumber = replaceCard.RequestNumber;
                        CacheProvider.Remove(CacheKeys.EV_ReplacePayment);
                        CacheProvider.Remove(CacheKeys.EV_ReplacePayment_AccountNumber);
                        return PartialView("~/Views/Feature/EV/EVCharger/ReplaceCardPayment.cshtml", model);
                    }
                }
                else if (CacheProvider.TryGet(CacheKeys.EV_SmartChargingPayment, out paymentpath))
                {
                    if (paymentpath.Equals("EV_SmartChargingPayment"))
                    {
                        CacheProvider.Remove(CacheKeys.EV_SmartChargingPayment);
                        CacheProvider.Store(CacheKeys.EV_SmartChargingconfirmationPayment, new AccessCountingCacheItem<PaymentCompletionModel>(model, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_PAYMENT_CONFIRMATION_PAGE);
                    }
                }
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));
            if (CacheProvider.TryGet(CacheKeys.EVDEACTIVATE_PAYMENT_PATH, out paymentpath))
            {
                if (paymentpath.Equals("evdeactivatePayment"))
                {
                    CacheProvider.Remove(CacheKeys.EVDEACTIVATE_PAYMENT_PATH);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_DeActivateCard);
                }
            }
            else if (CacheProvider.TryGet(CacheKeys.EV_CustomerTypePayment, out paymentpath))
            {
                if (paymentpath.Equals("EV_CustomerTypePaymentLogin"))
                {
                    CacheProvider.Remove(CacheKeys.EV_CustomerTypePayment);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_IndividualApplyCard);
                }
                else if (paymentpath.Equals("EV_CustomerTypePaymentNonDEWA"))
                {
                    CacheProvider.Remove(CacheKeys.EV_CustomerTypePayment);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
                }
            }
            else if (CacheProvider.TryGet(CacheKeys.EV_ReplacePayment, out paymentpath))
            {
                if (!string.IsNullOrEmpty(paymentpath) && paymentpath.Equals("EV_ReplacePayment"))
                {
                    CacheProvider.Remove(CacheKeys.EV_ReplacePayment);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_ReplaceCard);
                }
            }
            else if (CacheProvider.TryGet(CacheKeys.EV_SmartChargingPayment, out paymentpath))
            {
                if (paymentpath.Equals("EV_SmartChargingPayment"))
                {
                    CacheProvider.Remove(CacheKeys.EV_SmartChargingPayment);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_CHARGING_TIMING_PAGE);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
        }

        private ActionResult RedirectToEVAnonymousconfirmation(PaymentCompletionModel model)
        {
            string paymentpath = string.Empty;
            if (model.Succeeded)
            {
                //if (CacheProvider.TryGet(CacheKeys.EV_SmartChargingPayment, out paymentpath))
                //{
                //    if (paymentpath.Equals("EV_SmartChargingPayment"))
                //    {
                //        CacheProvider.Remove(CacheKeys.EV_SmartChargingPayment);
                CacheProvider.Store(CacheKeys.EV_SmartChargingconfirmationPayment, new AccessCountingCacheItem<PaymentCompletionModel>(model, Times.Once));
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_PAYMENT_CONFIRMATION_PAGE);
                //    }
                //}
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));
            if (CacheProvider.TryGet(CacheKeys.EV_SmartChargingPayment, out paymentpath))
            {
                if (paymentpath.Equals("EV_SmartChargingPayment"))
                {
                    CacheProvider.Remove(CacheKeys.EV_SmartChargingPayment);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.EVGREEN_CHARGING_TIMING_PAGE);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.EV_Registration);
        }

        private ActionResult RenderToTenderPayConfirmation(PaymentCompletionModel model)
        {
            ScrapeTenderPaymentModel _scPaymentModel = null;
            try
            {
                if (!CacheProvider.TryGet("TenderOnlinePurchaseDetail", out _scPaymentModel))
                {
                    _scPaymentModel = new ScrapeTenderPaymentModel();
                }

                _scPaymentModel.DegTransactionId = model.DegTransactionId;
                _scPaymentModel.DewaTransactionId = model.DewaTransactionId;
                _scPaymentModel.PaymentDate = model.PaymentDate;
                _scPaymentModel.Message = model.Message;
                _scPaymentModel.Epnumber = model.Epnumber;
                _scPaymentModel.Eptype = model.Eptype;
                _scPaymentModel.IsSuccess = model.Succeeded;
                _scPaymentModel.ReceiptIdentifiers = string.Format("{0:yyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                //Added as default setting to initate online payment.
                _scPaymentModel.IsJustPaid = !string.IsNullOrWhiteSpace(_scPaymentModel.Epnumber);
                _scPaymentModel.IsOnline = !string.IsNullOrWhiteSpace(_scPaymentModel.Epnumber); ;
                _scPaymentModel.referencenumber = _scPaymentModel.Epnumber;
                _scPaymentModel.ReferenceNo = _scPaymentModel.Epnumber;
                _scPaymentModel.PaidSuqiaAmount = model.SuqiaAmount;
                _scPaymentModel.PaidAmount = model.Total;
            }
            catch (System.Exception ex)
            {
                if (_scPaymentModel != null)
                {
                    _scPaymentModel = new ScrapeTenderPaymentModel();
                    _scPaymentModel.Message = ex.Message;
                }
            }

            CacheProvider.Store("TenderOnlinePurchaseDetail", new CacheItem<ScrapeTenderPaymentModel>(_scPaymentModel, TimeSpan.FromHours(1)));
            string _url = string.Format("{0}?t={1}&r={2}", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SCRAPESALE_TENDER_PUCHASE), Convert.ToString(_scPaymentModel?.transactionNumber), model.DegTransactionId);
            return Redirect(_url);
        }

        private ActionResult RenderToTenderBidPayConfirmation(PaymentCompletionModel model)
        {
            TenderBiddingPaymentModel _scPaymentModel = null;
            try
            {
                if (!CacheProvider.TryGet("TenderBidOnlinePurchaseDetail", out _scPaymentModel))
                {
                    _scPaymentModel = new TenderBiddingPaymentModel();
                }

                _scPaymentModel.DegTransactionId = model.DegTransactionId;
                _scPaymentModel.DewaTransactionId = model.DewaTransactionId;
                _scPaymentModel.PaymentDate = model.PaymentDate;
                _scPaymentModel.Message = model.Message;
                _scPaymentModel.Epnumber = model.Epnumber;
                _scPaymentModel.Eptype = model.Eptype;
                _scPaymentModel.IsSuccess = model.Succeeded;
                _scPaymentModel.ReceiptIdentifiers = string.Format("{0:yyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                _scPaymentModel.IsJustPaid = !string.IsNullOrWhiteSpace(_scPaymentModel.Epnumber);
                _scPaymentModel.IsOnline = !string.IsNullOrWhiteSpace(_scPaymentModel.Epnumber);
                _scPaymentModel.TenderBidRefNumber = _scPaymentModel.Epnumber;
                _scPaymentModel.ReferenceNo = _scPaymentModel.Epnumber;
                _scPaymentModel.PaidSuqiaAmount = model.SuqiaAmount;
                _scPaymentModel.PaidAmount = model.Total;
            }
            catch (System.Exception ex)
            {
                if (_scPaymentModel != null)
                {
                    _scPaymentModel = new TenderBiddingPaymentModel();
                    _scPaymentModel.Message = ex.Message;
                }
            }

            CacheProvider.Store("TenderBidOnlinePurchaseDetail", new CacheItem<TenderBiddingPaymentModel>(_scPaymentModel, TimeSpan.FromHours(1)));
            string _url = string.Format("{0}?t={1}&r={2}", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SCRAPESALE_TENDER_BIDDING_PUCHASE), Convert.ToString(_scPaymentModel?.TenderBidRefNumber), model.DegTransactionId);
            return Redirect(_url);
        }

        private ActionResult RenderToSalesOrderPayConfirmation(PaymentCompletionModel model)
        {
            SalesOrderPaymentModel _scPaymentModel = null;
            try
            {
                if (!CacheProvider.TryGet("SalesOrderOnlinePurchaseDetail", out _scPaymentModel))
                {
                    _scPaymentModel = new SalesOrderPaymentModel();
                }

                _scPaymentModel.DegTransactionId = model.DegTransactionId;
                _scPaymentModel.DewaTransactionId = model.DewaTransactionId;
                _scPaymentModel.PaymentDate = model.PaymentDate;
                _scPaymentModel.Message = model.Message;
                _scPaymentModel.Epnumber = model.Epnumber;
                _scPaymentModel.Eptype = model.Eptype;
                _scPaymentModel.IsSuccess = model.Succeeded;
                _scPaymentModel.ReceiptIdentifiers = string.Format("{0:yyMMdd}{1}", model.PaymentDate, model.DegTransactionId);
                _scPaymentModel.PaidSuqiaAmount = model.SuqiaAmount;
                _scPaymentModel.PaidAmount = model.Total;
            }
            catch (System.Exception ex)
            {
                if (_scPaymentModel != null)
                {
                    _scPaymentModel = new SalesOrderPaymentModel();
                    _scPaymentModel.Message = ex.Message;
                }
            }

            CacheProvider.Store("SalesOrderOnlinePurchaseDetail", new CacheItem<SalesOrderPaymentModel>(_scPaymentModel, TimeSpan.FromHours(1)));
            string _url = string.Format("{0}?t={1}&r={2}", LinkHelper.GetItemUrl(SitecoreItemIdentifiers.SCRAPESALE_SALESORDER_PAYMENT), Convert.ToString(_scPaymentModel?.salesDocumentNo), model.DegTransactionId);
            return Redirect(_url);
        }

        private ActionResult RenderRefundRecoveryConfirmation(PaymentCompletionModel model)
        {
            UpdateIBANModel metadata;
            if (CacheProvider.TryGet(CacheKeys.IBANDetail_PaymentData, out metadata))
            {
                model.Receipts = metadata.divisionlist.Select(x => new ReceiptModel()
                {
                    AccountNumber = x.contractaccountnumber,
                    ReceiptAccountName = x.contractaccountname,
                    Amount = Convert.ToDecimal(x.totalamount),
                });
                model.ContractAccountNumber = metadata.AccountSelected;
            }

            if (model.Succeeded)
            {
                return PartialView("~/Views/Feature/Payment/Payment/_RefundRecoveryPaymentConfirmation.cshtml", model);
            }

            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Payment failed", model.DegTransactionId), Times.Once));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.UPDATE_IBANPAGE);
        }

        private IEnumerable<ReceiptModel> MapReceipts(IEnumerable<Receipt> receipts, IEnumerable<AccountDetails> accounts)
        {
            foreach (var receipt in receipts)
            {
                var accountNumber = receipt.AccountNumber;

                var match = accounts.FirstOrDefault(acc => acc.AccountNumber.Equals(accountNumber));
                if (match != null)
                {
                    yield return AccountReceiptModel.From(receipt, match);
                    continue;
                }
                //yield return ReceiptModel.From(receipt);
            }
        }

        private IEnumerable<ReceiptModel> MapReceipts(IEnumerable<Receipt> receipts, IEnumerable<Account> accounts)
        {
            foreach (var receipt in receipts)
            {
                var accountNumber = receipt.AccountNumber;

                var match = accounts.FirstOrDefault(acc => acc.AccountNumber.Equals(accountNumber));
                if (match != null)
                {
                    yield return AccountReceiptModel.From(receipt, match);
                    continue;
                }
                //yield return ReceiptModel.From(receipt);
            }
        }

        private static string GetSuccessMessage(PaymentContext context)
        {
            switch (context)
            {
                case PaymentContext.PayFriendsBill:
                    return Translate.Text("Friend payment confirmation message");

                default:
                    return Translate.Text("Payment confirmation message");
            }
        }

        private static string GetErrorMessage(string transactionIdentifier)
        {
            if (string.IsNullOrEmpty(transactionIdentifier))
            {
                return Translate.Text("Paymentfail");
            }
            return Translate.Text("Payment failed", transactionIdentifier);
        }

        private static string GetEPayURL(string dns)
        {
            string url = Config.EPayUrl;

            if (dns == Config.SecondaryDNS)
                url = Config.EPayUrlDEWA;

            return url;
        }

        private static string GetNoqodiPayURL(string dns)
        {
            string url = Config.NoqodiPayUrl;

            if (dns == Config.SecondaryDNS)
                url = Config.NoqodiPayUrlDEWA;

            return url;
        }

        #endregion Helper methods
    }
}