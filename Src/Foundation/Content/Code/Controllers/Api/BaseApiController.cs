using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Impl;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Impl.EVLocationSvc;
using DEWAXP.Foundation.Integration.Impl.SmartCustomerV3Svc;
using DEWAXP.Foundation.Integration.Impl.SmartVendor;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc.Web.Mvc;
using Glass.Mapper.Sc;
using Sitecore;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Content.Controllers.Api
{
    using _securePayRestModel = DEWAXP.Foundation.Integration.APIHandler.Models.Request.SecuredPayment;
    using _securePayWebModel = Models.Payment.SecurePay;

    public class FutureCenterValues
    {
        public string Branch { get; set; }
        public string ServiceCenter { get; set; }
        public string Message { get; set; }
    }

    public abstract class BaseApiController : ApiController
    {
        private IContentRepository _contentRepository;
        private IContextRepository _contextRepository;
        private IRenderingRepository _renderingRepository;
        private readonly DewaProfile _currentPrincipal;
        private readonly IDewaServiceClient _dewaApiClient;
        private readonly IEServicesClient _customerServiceClient;
        private readonly IVendorServiceClient _vendorServiceClient;
        private readonly ICacheProvider _cache;
        private readonly IDewaAuthStateService _authStateService;
        private readonly IConsultantServiceClient _consulantServiceClient;
        private readonly ISmartCustomerClient _smartCustomerClient;
        private readonly IDubaiModelServiceClient _dubaiModelServiceClient;
        private readonly IDTMCInsightsReportClient _DTMCInsightsReportClient;
        private readonly IMoveOutClient _MoveOutClient;
        private readonly IEVDashboardClient _EVDashboardClient;
        private readonly IEVCSClient _EVCSClient;
        private readonly ISmartVendorClient _smartVendorClient;
        private readonly IPremiseClient _IPremiseClient;
        private readonly ILocationDataPointsClient _locationDataPointsClient;

        protected BaseApiController()
        {
            _contentRepository = new ContentRepository(new MvcContext(new SitecoreService(SitecoreX.Database)));
            _contextRepository = new ContextRepository(new MvcContext(new SitecoreService(SitecoreX.Database)));
            _renderingRepository = new RenderingRepository(new MvcContext(new SitecoreService(SitecoreX.Database)));
            _dewaApiClient = DependencyResolver.Current.GetService<IDewaServiceClient>();
            _customerServiceClient = DependencyResolver.Current.GetService<IEServicesClient>();
            _vendorServiceClient = DependencyResolver.Current.GetService<IVendorServiceClient>();
            _cache = DependencyResolver.Current.GetService<ICacheProvider>();
            _authStateService = DependencyResolver.Current.GetService<IDewaAuthStateService>();
            _consulantServiceClient = DependencyResolver.Current.GetService<IConsultantServiceClient>();
            _dubaiModelServiceClient = DependencyResolver.Current.GetService<IDubaiModelServiceClient>();
            _smartCustomerClient = DependencyResolver.Current.GetService<ISmartCustomerClient>();
            _currentPrincipal = _authStateService.GetActiveProfile();
            _DTMCInsightsReportClient = DependencyResolver.Current.GetService<DTMCInsightsReportClient>();

            #region [Rest Client]

            _MoveOutClient = DependencyResolver.Current.GetService<MoveOutClient>();
            _EVDashboardClient = DependencyResolver.Current.GetService<EVDashboardClient>();
            ChipherPaymentClient = DependencyResolver.Current.GetService<ChipherPaymentClient>();

            #endregion [Rest Client]

            _EVCSClient = DependencyResolver.Current.GetService<EVCSClient>();
            _smartVendorClient = DependencyResolver.Current.GetService<ISmartVendorClient>();
            _IPremiseClient = DependencyResolver.Current.GetService<IPremiseClient>();
            _locationDataPointsClient = DependencyResolver.Current.GetService<LocationDataPointsClient>();
        }
        protected IContentRepository ContentRepository
        {
            get { return _contentRepository; }
        }
        protected IContextRepository ContextRepository
        {
            get { return _contextRepository; }
        }
        protected IRenderingRepository RenderingRepository
        {
            get { return _renderingRepository; }
        }
        protected IDTMCInsightsReportClient DTMCInsightsReportClient => _DTMCInsightsReportClient;

        protected IDewaServiceClient DewaApiClient
        {
            get { return _dewaApiClient; }
        }

        protected IVendorServiceClient VendorApiClient
        {
            get { return _vendorServiceClient; }
        }

        protected IEServicesClient CustomerServiceClient
        {
            get { return _customerServiceClient; }
        }

        protected IConsultantServiceClient ConsultantServiceClient
        {
            get { return this._consulantServiceClient; }
        }

        protected ISmartCustomerClient SmartCustomerClient
        {
            get { return this._smartCustomerClient; }
        }

        protected IDubaiModelServiceClient DubaiModelServiceClient
        {
            get { return this._dubaiModelServiceClient; }
        }

        protected IEVCSClient EVCSClient => _EVCSClient;
        protected IEVDashboardClient EVDashboardClient => _EVDashboardClient;
        protected ISmartVendorClient SmartVendorClient => _smartVendorClient;

        protected IPremiseClient PremiseClient
        {
            get { return this._IPremiseClient; }
        }

        protected ILocationDataPointsClient LocationDataPointsClient
        {
            get { return this._locationDataPointsClient; }
        }

        protected ICacheProvider CacheProvider
        {
            get { return _cache; }
        }

        protected bool IsAuthenticated
        {
            get { return System.Web.HttpContext.Current.Request.IsAuthenticated && !Context.IsAdministrator; }
        }

        protected DewaProfile CurrentPrincipal
        {
            get { return _currentPrincipal; }
        }

        protected SupportedLanguage RequestLanguage
        {
            get
            {
                var languageCookieKey = SitecoreX.Site.GetCookieKey("lang");
                var electedLanguage = WebUtil.GetCookieValue(languageCookieKey, "en");

                return "en".Equals(electedLanguage, StringComparison.InvariantCultureIgnoreCase) ?
                    SupportedLanguage.English : SupportedLanguage.Arabic;
            }
        }

        protected string RedirectUrl(string id)
        {
            var item = SitecoreX.Database.GetItem(id);
            return LinkManager.GetItemUrl(item, LinkOptions.Url);
        }

        public string FormatContractAccount(string account)
        {
            long i = 0;
            long.TryParse(account, out i);
            return i.ToString("D12");
        }

        public string FormatBusinessPartner(string account)
        {
            long i = 0;
            long.TryParse(account, out i);
            return i.ToString("D10");
        }

        protected FutureCenterValues FetchFutureCenterValues()
        {
            FutureCenterValues values = new FutureCenterValues();

            if (System.Web.HttpContext.Current.Request.Cookies["fdc"] != null)
            {
                string[] _cookie = System.Web.HttpContext.Current.Request.Cookies["fdc"].Value.Split('|');
                string _branch = _cookie[0];
                string _service = _cookie[1];

                values.Branch = _branch;
                values.ServiceCenter = _service;

                return values;
            }
            else
            {
                values.Message = "No cookie value set for Future Center";
                return values;
            }
        }

        public DateTime? converttodate(string strdate)
        {
            if (!string.IsNullOrWhiteSpace(strdate))
            {
                CultureInfo culture = SitecoreX.Culture;
                if (culture.ToString().Equals("ar-AE"))
                {
                    strdate = strdate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                }
                DateTime.TryParse(strdate, out DateTime fromdateTime);
                return fromdateTime;
            }
            return null;
        }

        #region [Rest Client Variable]

        protected IMoveOutClient MoveOutClient
        {
            get { return this._MoveOutClient; }
        }

        protected IChipherPaymentClient ChipherPaymentClient { get; }

        #endregion [Rest Client Variable]

        #region [Cipher Payment Function]

        protected _securePayWebModel.CipherPaymentModel ExecutePaymentGateway(_securePayWebModel.CipherPaymentModel model)
        {
            _securePayWebModel.CipherPaymentModel paymentReturnData = model;
            try
            {
                string _userId = System.Convert.ToString(model.PaymentData?.userid ?? CurrentPrincipal?.UserId);
                paymentReturnData.ErrorMessages = new Dictionary<string, string>();
                paymentReturnData.PayPostModel = new _securePayWebModel.CipherPaymentPostModel()
                {
                    PaymentUrl = WebHandler.CipherPaymentHandler.GetSecuredPayUrl(model.PaymentMethod),
                    u = _userId
                };

                #region [Validation]

                if (!model.IsValidAccountsAndAmountDetails())
                {
                    paymentReturnData.ErrorMessages.Add("error1", Translate.Text("AmountAndContractNoErrorMsg"));
                }
                if (!model.ServiceType.HasValue)
                {
                    LogService.Error(new Exception("Error:Payment Type null or empty"), this);
                    paymentReturnData.ErrorMessages.Add("error2", Translate.Text("AmountAndContractNoErrorMsg"));
                }

                #endregion [Validation]

                string ServiceTypeFlagKey;
                if (model.ErrorMessages.Count == 0)
                {
                    _securePayRestModel.CipherPaymentDetailInputs EpayTokenparams = model.PaymentData;

                    #region [binding default value]

                    EpayTokenparams.redirectflag = Config.SecuredPayurlRedirect;
                    EpayTokenparams.userid = _userId;
                    EpayTokenparams.sessionid = System.Convert.ToString(CurrentPrincipal?.SessionToken);
                    EpayTokenparams.transactiontype = model.ContextType.EPayTransactionCode(Request.Segment());
                    EpayTokenparams.paymode = PaymentContextExtensions.GetPaymentMode(model.PaymentMethod);
                    EpayTokenparams.branch = FetchFutureCenterValues().Branch;
                    EpayTokenparams.tendertransactionid = string.IsNullOrWhiteSpace(model.PaymentData.tendertransactionid) ? "STATIC" : model.PaymentData.tendertransactionid;
                    EpayTokenparams.bk = System.Convert.ToString(!string.IsNullOrWhiteSpace(model.BankKey) && model.PaymentMethod == PaymentMethod.PaythroughNetbanking ? model.BankKey : null);
                    #endregion [binding default value]

                    #region [ServiceType base logic]

                    switch (model.ServiceType)
                    {
                        case ServiceType.PayBill:

                            #region [paybill check & updates]

                            //For Tayseer Payment //Already map
                            //if (!string.IsNullOrEmpty(model.mto) && model.mto != "")
                            //{
                            //    _model.mto = model.mto;
                            //}

                            //SmartWallet not in used
                            //if (CacheProvider.TryGet(CacheKeys.PAYMENT_PATH, out ServiceTypeFlagKey) &&
                            //!string.IsNullOrEmpty(ServiceTypeFlagKey) && ServiceTypeFlagKey.Equals("SmartWallet"))
                            //{
                            //    //EpayTokenparams.swal = "y";
                            //}

                            if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out ServiceTypeFlagKey) &&
                                (!string.IsNullOrEmpty(ServiceTypeFlagKey) && ServiceTypeFlagKey.Equals("movetopaymentprocess")))
                            {
                                EpayTokenparams.movetoflag = "A";
                            }

                            if (CacheProvider.TryGet(CacheKeys.EV_ReplacePayment, out ServiceTypeFlagKey) &&
                                !string.IsNullOrEmpty(ServiceTypeFlagKey))
                            {
                                EpayTokenparams.movetoflag = "V";
                                //_model.x = model.x; // already mapped.
                            }

                            //Already Mapped
                            //if (CacheProvider.TryGet(CacheKeys.MOVEOUT_PAYMENT_ANONYMOUS_PATH, out ServiceTypeFlagKey)&&
                            //    (!string.IsNullOrEmpty(ServiceTypeFlagKey)&& ServiceTypeFlagKey.Equals("MoveoutAnonymousPayment")))
                            //{
                            //    EpayTokenparams.userid = model.u;
                            //    EpayTokenparams.businesspartner = model.b;
                            //}

                            #endregion [paybill check & updates]

                            break;

                        case ServiceType.MoveOut:
                            //Not Required as already been handled.
                            break;

                        case ServiceType.TemporaryConnection:
                            break;

                        case ServiceType.PayFriendsBill:
                            break;

                        case ServiceType.PayFriendsEstimate:
                            break;

                        case ServiceType.PayMyEstimate:
                            break;

                        case ServiceType.Clearance:
                            break;

                        case ServiceType.AnonymousClearance:
                            break;

                        case ServiceType.ServiceActivation:

                            #region [ServiceActivation Check Update]

                            if (CacheProvider.TryGet(CacheKeys.MOVETOPAYMENT, out ServiceTypeFlagKey) &&
                                (!string.IsNullOrEmpty(ServiceTypeFlagKey) && ServiceTypeFlagKey.Equals("movetopaymentprocess")))
                            {
                                EpayTokenparams.movetoflag = "A";
                            }

                            #endregion [ServiceActivation Check Update]

                            break;

                        case ServiceType.AnonymousServiceActivation:
                            break;

                        case ServiceType.ReraServiceActivation:
                            break;

                        case ServiceType.AnonymousReraServiceActivation:
                            break;

                        case ServiceType.MoveOutActivation:
                            break;

                        case ServiceType.Miscellaneous:
                            break;

                        case ServiceType.EVCard:

                            #region [EVCard check update]

                            if (CacheProvider.TryGet(CacheKeys.EV_ReplacePayment, out ServiceTypeFlagKey) &&
                                !string.IsNullOrEmpty(ServiceTypeFlagKey))
                            {
                                EpayTokenparams.movetoflag = "V";
                            }

                            #endregion [EVCard check update]

                            break;

                        case ServiceType.EstimatePayment:
                            break;
                    }

                    #endregion [ServiceType base logic]

                    #region [Payment Token Generation]

                    var returnData = ChipherPaymentClient.GenerateEpayToken(new _securePayRestModel.CipherPaymentDetailRequest()
                    {
                        paymentparams = EpayTokenparams,
                    }, Request.Segment(), RequestLanguage);
                    if (returnData.Succeeded)
                    {
                        paymentReturnData.PayPostModel.dewatoken = returnData.Payload.paymenttoken;
                        CacheProvider.Store(CacheKeys.LastestDewaToken, new CacheItem<string>(paymentReturnData.PayPostModel.dewatoken, TimeSpan.FromDays(1)));
                        CacheProvider.Store(CacheKeys.UAEPGS_PaymentAmount, new CacheItem<string>(paymentReturnData.PaymentData.amounts, TimeSpan.FromDays(1)));
                    }
                    else
                    {
                        paymentReturnData.ErrorMessages.Add("error3", returnData.Message);
                    }

                    #endregion [Payment Token Generation]
                }
            }
            catch (Exception ex)
            {
                paymentReturnData.ErrorMessages.Add("error4", ErrorMessages.UNEXPECTED_ERROR);
                LogService.Error(ex, this);
            }
            return paymentReturnData;
        }

        #endregion [Cipher Payment Function]
    }
}