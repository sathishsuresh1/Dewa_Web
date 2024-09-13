using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.Content.Models.Common;
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
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Content.Controllers
{
    using _securePayRestModel = Integration.APIHandler.Models.Request.SecuredPayment;
    using _securePayWebModel = Models.Payment.SecurePay;

    public class FutureCenterValues
    {
        public string Branch { get; set; }
        public string ServiceCenter { get; set; }

        public string Message { get; set; }
    }

    public abstract class BaseController : SitecoreController
    {
        private IContentRepository _contentRepository;
        private IContextRepository _contextRepository;
        private IRenderingRepository _renderingRepository;
        private readonly DewaProfile _currentPrincipal;
        private readonly ISitecoreService _sitecoreService;
        private readonly IDewaServiceClient _dewaApiClient;
        private readonly IDewaAuthStateService _authStateService;
        private readonly ITenderServiceClient _tenderServiceClient;
        private readonly IVendorServiceClient _vendorServiceClient;
        private readonly IEServicesClient _customerServiceClient;
        private readonly ICacheProvider _cache;

        private readonly IProjectDocumentGenerationServiceClient _projectgenerationdocumentclient;
        private readonly IEmailServiceClient _emailserviceclient;
        private readonly IHappinessServiceClient _happinessServiceClient;
        private readonly IConsultantServiceClient _consulantServiceClient;
        private readonly IEFormServiceClient _eFormServiceClient;
        private readonly IDubaiModelServiceClient _dubaiModelServiceClient;
        private readonly IJobSeekerServiceClient _jobSeekerServiceClient;
        private readonly IVerificationcodeServiceClient _verificationCodeServiceClient;
        private readonly ISmartCustAuthenticationServiceClient _smartCustAuthenticationServiceClient;

        private readonly IEVServiceClient _EVServiceClient;
        private readonly IScholarshipServiceClient _scholarshipServiceClient;
        private readonly IinternshipServiceClient _internshipServiceClient;
        private readonly IWebsiteSurveyServiceClient _WebsiteSurveyServiceClient;
        private readonly IGraphServiceClient _iGraphServiceClient;
        private readonly ILectureBookingClient _lectureBookingClient;
        private readonly ICustomerSmartSaleClient _customerSmartSaleClient;
        private readonly IEFormRESTServiceClient _eFormRESTServiceClient;
        private readonly ICorportatePortalClient _corporateportalClient;
        private readonly IKhadamatechDEWAServiceClient _khadamatechDEWAServiceClient;
        private readonly IDewaStoreClient _dewaStoreClient;
        private readonly ISmartConsultantClient _smartConsultantClient;
        private readonly IQmsServiceClient _qmsServiceClient;
        private readonly ISmartCustomerClient _smartCustomerClient;
        private readonly IGatePassServiceClient _iGatePassServiceClient;
        private readonly ISmsServiceClient _iSmsServiceclient;
        private readonly IEstimateClient _EstimateRestClient;
        private readonly IEVCSClient _EVCSClient;
        private readonly IChipherPaymentClient _ChipherPaymentClient;
        private readonly IUAEPassServiceClient _UAEPassServiceClient;
        #region [Rest API Client]
        private readonly IEVCardApiHandler _EVCardApiHandler;
        private readonly IAwayModeClient _AwayModeClient;
        private readonly IDTMCTrackingClient _DTMCTrackingClient;
        private readonly IDTMCInsightsReportClient _DTMCInsightsReportClient;
        private readonly IKofaxRESTServiceClient _kofaxRESTClient;
        private readonly IPremiseNumberSearchClient _premiseNumberSearchClient;
        private readonly IRefundHistoryClient _refundHistoryClient;
        private readonly ISmartCommunicationClient _smartCommunicationClient;
        private readonly IAnonymousTrackingClient _trackRequestAnonymousClient;
        private readonly IRefundNameChangeClient _refundNameChangeClient;
        private readonly IMoveOutClient _MoveOutClient;
        private readonly IExpo2020Client _expo2020Client;
        private readonly ISmartVendorClient _smartVendorClient;
        private readonly IEVDashboardClient _EVDashboardClient;
        private readonly IDewaScholarshipRestClient _dewaScholarshipRestClient;
        private readonly IJobseekerRestClient _jobseekerRestClient;
        private readonly IVillaCostExemptionClient _villaCostExemptionClient;
        private readonly ISmartResponseClient _smartResponseClient;
        private readonly IPremiseClient _iPremiseClient;
        private readonly ITayseerClient _tayseerClient;
        private readonly ISmartSurveyClient _smartSurveyClient;
        private readonly IMasarClient _masarClient;
        private readonly IGeneralServicesClient _generalServicesClient;
        #endregion [Rest API Client]

        protected BaseController()
        {
            _contentRepository = new ContentRepository(new MvcContext(new SitecoreService(SitecoreX.Database)));
            _contextRepository = new ContextRepository(new MvcContext(new SitecoreService(SitecoreX.Database)));
            _renderingRepository = new RenderingRepository(new MvcContext(new SitecoreService(SitecoreX.Database)));
            _sitecoreService = DependencyResolver.Current.GetService<ISitecoreService>();
            _dewaApiClient = DependencyResolver.Current.GetService<IDewaServiceClient>();
            _authStateService = DependencyResolver.Current.GetService<IDewaAuthStateService>();
            _tenderServiceClient = DependencyResolver.Current.GetService<ITenderServiceClient>();
            _vendorServiceClient = DependencyResolver.Current.GetService<IVendorServiceClient>();
            _customerServiceClient = DependencyResolver.Current.GetService<IEServicesClient>();
            _cache = DependencyResolver.Current.GetService<ICacheProvider>();

            _currentPrincipal = _authStateService.GetActiveProfile();
            _projectgenerationdocumentclient = DependencyResolver.Current.GetService<IProjectDocumentGenerationServiceClient>();
            _emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();
            _happinessServiceClient = DependencyResolver.Current.GetService<IHappinessServiceClient>();
            _consulantServiceClient = DependencyResolver.Current.GetService<IConsultantServiceClient>();
            _eFormServiceClient = DependencyResolver.Current.GetService<IEFormServiceClient>();
            _dubaiModelServiceClient = DependencyResolver.Current.GetService<IDubaiModelServiceClient>();
            _jobSeekerServiceClient = DependencyResolver.Current.GetService<IJobSeekerServiceClient>();
            _smartCustAuthenticationServiceClient = DependencyResolver.Current.GetService<ISmartCustAuthenticationServiceClient>();
            _EVServiceClient = DependencyResolver.Current.GetService<IEVServiceClient>();
            _iGatePassServiceClient = DependencyResolver.Current.GetService<IGatePassServiceClient>();
            _scholarshipServiceClient = DependencyResolver.Current.GetService<IScholarshipServiceClient>();
            _internshipServiceClient = DependencyResolver.Current.GetService<IinternshipServiceClient>();
            _verificationCodeServiceClient = DependencyResolver.Current.GetService<IVerificationcodeServiceClient>();
            _WebsiteSurveyServiceClient = DependencyResolver.Current.GetService<IWebsiteSurveyServiceClient>();
            _EVCardApiHandler = DependencyResolver.Current.GetService<EVCardApiHandler>();
            _lectureBookingClient = DependencyResolver.Current.GetService<ILectureBookingClient>();
            _customerSmartSaleClient = DependencyResolver.Current.GetService<ICustomerSmartSaleClient>();
            _iGraphServiceClient = DependencyResolver.Current.GetService<IGraphServiceClient>();
            _khadamatechDEWAServiceClient = DependencyResolver.Current.GetService<IKhadamatechDEWAServiceClient>();
            _dewaStoreClient = DependencyResolver.Current.GetService<IDewaStoreClient>();
            _smartCustomerClient = DependencyResolver.Current.GetService<ISmartCustomerClient>();
            _AwayModeClient = DependencyResolver.Current.GetService<IAwayModeClient>();
            _DTMCTrackingClient = DependencyResolver.Current.GetService<IDTMCTrackingClient>();
            _kofaxRESTClient = DependencyResolver.Current.GetService<IKofaxRESTServiceClient>();
            _smartConsultantClient = DependencyResolver.Current.GetService<ISmartConsultantClient>();

            _iSmsServiceclient = DependencyResolver.Current.GetService<ISmsServiceClient>();
            _eFormRESTServiceClient = DependencyResolver.Current.GetService<IEFormRESTServiceClient>();

            _corporateportalClient = DependencyResolver.Current.GetService<ICorportatePortalClient>();
            _DTMCInsightsReportClient = DependencyResolver.Current.GetService<IDTMCInsightsReportClient>();
            _premiseNumberSearchClient = DependencyResolver.Current.GetService<IPremiseNumberSearchClient>();
            _smartCommunicationClient = DependencyResolver.Current.GetService<ISmartCommunicationClient>();
            _qmsServiceClient = DependencyResolver.Current.GetService<IQmsServiceClient>();
            _expo2020Client = DependencyResolver.Current.GetService<IExpo2020Client>();
            _EVCSClient = DependencyResolver.Current.GetService<IEVCSClient>();
            _iPremiseClient = DependencyResolver.Current.GetService<PremiseHandler>();
            _smartSurveyClient = DependencyResolver.Current.GetService<SmartSurveyClient>();

            #region [RestClient]
            _refundHistoryClient = DependencyResolver.Current.GetService<IRefundHistoryClient>();
            _trackRequestAnonymousClient = DependencyResolver.Current.GetService<IAnonymousTrackingClient>();
            _refundNameChangeClient = DependencyResolver.Current.GetService<IRefundNameChangeClient>();
            _MoveOutClient = DependencyResolver.Current.GetService<IMoveOutClient>();
            _ChipherPaymentClient = DependencyResolver.Current.GetService<IChipherPaymentClient>();
            _EVDashboardClient = DependencyResolver.Current.GetService<IEVDashboardClient>();
            _smartVendorClient = DependencyResolver.Current.GetService<ISmartVendorClient>();
            _UAEPassServiceClient = DependencyResolver.Current.GetService<IUAEPassServiceClient>();
            _dewaScholarshipRestClient = DependencyResolver.Current.GetService<IDewaScholarshipRestClient>();
            _jobseekerRestClient = DependencyResolver.Current.GetService<IJobseekerRestClient>();
            _villaCostExemptionClient = DependencyResolver.Current.GetService<IVillaCostExemptionClient>();
            _EstimateRestClient = DependencyResolver.Current.GetService<IEstimateClient>();
            _smartResponseClient = DependencyResolver.Current.GetService<ISmartResponseClient>();
            _tayseerClient = DependencyResolver.Current.GetService<TayseerClient>();

            _masarClient = DependencyResolver.Current.GetService<IMasarClient>();
            _generalServicesClient = DependencyResolver.Current.GetService<IGeneralServicesClient>();
            #endregion [RestClient]
        }

        #region Methods
        #region Dependencies

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

        protected DewaProfile CurrentPrincipal
        {
            get { return _currentPrincipal; }
        }

        protected IDewaServiceClient DewaApiClient
        {
            get { return _dewaApiClient; }
        }

        protected IDewaAuthStateService AuthStateService
        {
            get { return _authStateService; }
        }

        protected ITenderServiceClient TenderServiceClient
        {
            get { return _tenderServiceClient; }
        }

        protected IVendorServiceClient VendorServiceClient
        {
            get { return _vendorServiceClient; }
        }

        protected IEServicesClient CustomerServiceClient
        {
            get { return _customerServiceClient; }
        }

        protected ISmartCustAuthenticationServiceClient SmartCustAuthenticationServiceClient
        {
            get { return _smartCustAuthenticationServiceClient; }
        }

        protected IJobSeekerServiceClient JobSeekerClient
        {
            get { return _jobSeekerServiceClient; }
        }

        protected IVerificationcodeServiceClient VerificationCodeClient
        {
            get { return _verificationCodeServiceClient; }
        }

        protected ICacheProvider CacheProvider
        {
            get { return _cache; }
        }

        protected IProjectDocumentGenerationServiceClient ProjectGenerationClient
        {
            get { return this._projectgenerationdocumentclient; }
        }

        protected IEmailServiceClient EmailServiceClient
        {
            get { return this._emailserviceclient; }
        }

        protected IHappinessServiceClient HappinessServiceClient
        {
            get { return this._happinessServiceClient; }
        }

        protected IConsultantServiceClient ConsultantServiceClient
        {
            get { return this._consulantServiceClient; }
        }

        protected IEFormServiceClient eFormServiceClient
        {
            get { return this._eFormServiceClient; }
        }

        protected IDubaiModelServiceClient DubaiModelServiceClient
        {
            get { return this._dubaiModelServiceClient; }
        }

        protected IEVServiceClient EVServiceClient
        {
            get { return _EVServiceClient; }
        }

        protected IScholarshipServiceClient ScholarshipServiceClient
        {
            get { return _scholarshipServiceClient; }
        }

        protected IinternshipServiceClient IntershipServiceClient
        {
            get { return _internshipServiceClient; }
        }

        protected IWebsiteSurveyServiceClient WebsiteSurveyServiceClient
        {
            get { return _WebsiteSurveyServiceClient; }
        }

        protected ILectureBookingClient LectureBookingClient => _lectureBookingClient;
        protected ICustomerSmartSaleClient CustomerSmartSaleClient => _customerSmartSaleClient;
        protected IKhadamatechDEWAServiceClient khadamatechDEWAServiceClient => _khadamatechDEWAServiceClient;
        protected IDewaStoreClient DewaStoreClient => _dewaStoreClient;
        protected ISmartConsultantClient SmartConsultantClient => _smartConsultantClient;
        protected IKofaxRESTServiceClient KofaxRESTService => _kofaxRESTClient;
        protected IGatePassServiceClient GatePassServiceClient => _iGatePassServiceClient;
        protected ISmsServiceClient SmsServiceClient => _iSmsServiceclient;

        protected IEFormRESTServiceClient eFormRESTServiceClient => _eFormRESTServiceClient;

        protected ISmartVendorClient SmartVendorClient => _smartVendorClient;

        protected ICorportatePortalClient CorportatePortalClient => _corporateportalClient;

        protected IQmsServiceClient QmsServiceClient
        {
            get { return _qmsServiceClient; }
        }

        protected ISmartCustomerClient SmartCustomerClient => _smartCustomerClient;

        protected IEVCSClient EVCSClient => _EVCSClient;

        #region [Rest Client]

        protected IEVCardApiHandler EVCardApiHandler
        {
            get { return _EVCardApiHandler; }
        }

        protected IGraphServiceClient IGraphServiceClient
        { get { return _iGraphServiceClient; } }
        protected IAwayModeClient AwayModeClient => _AwayModeClient;
        protected IDTMCTrackingClient DTMCTrackingClient => _DTMCTrackingClient;
        protected IDTMCInsightsReportClient DTMCInsightsReportClient => _DTMCInsightsReportClient;

        protected IPremiseNumberSearchClient PremiseNumberSearchClient => _premiseNumberSearchClient;
        protected IRefundHistoryClient RefundHistoryClient => _refundHistoryClient;
        protected ISmartCommunicationClient SmartCommunicationClient => _smartCommunicationClient;

        protected IAnonymousTrackingClient TrackRequestAnonymousClient => _trackRequestAnonymousClient;
        protected IRefundNameChangeClient RefundNameChangeClient => _refundNameChangeClient;
        protected IMoveOutClient MoveOutClient => _MoveOutClient;
        protected IExpo2020Client Expo2020Client => _expo2020Client;
        protected IChipherPaymentClient ChipherPaymentClient => _ChipherPaymentClient;
        protected IEVDashboardClient EVDashboardClient => _EVDashboardClient;
        protected IUAEPassServiceClient UAEPassServiceClient => _UAEPassServiceClient;
        protected IDewaScholarshipRestClient DewaScholarshipRestClient => _dewaScholarshipRestClient;
        protected IJobseekerRestClient JobseekerRestClient => _jobseekerRestClient;
        protected IVillaCostExemptionClient VillaCostExemptionClient => _villaCostExemptionClient;
        protected IEstimateClient EstimateRestClient => _EstimateRestClient;
        protected ISmartResponseClient SmartResponseClient => _smartResponseClient;
        protected IPremiseClient PremiseHandler => _iPremiseClient;
        protected ITayseerClient TayseerClient => _tayseerClient;

        protected IMasarClient MasarClient => _masarClient;
        protected IGeneralServicesClient GeneralServicesClient => _generalServicesClient;
        protected ISmartSurveyClient SmartSurveyClient => _smartSurveyClient;

        #endregion [Rest Client]
        #endregion Dependencies

        /// <summary>
        /// Attempts to locate and redirect to the desired Sitecore item
        /// </summary>
        /// <param name="id">The unique identifier of the Sitecore item</param>
        /// <returns></returns>
        protected ActionResult RedirectToSitecoreItem(string id)
        {
            var item = SitecoreX.Database.GetItem(id);

            return RedirectToSitecoreItem(item);
        }

        protected ActionResult RedirectToSitecoreItem(string id, QueryString q)
        {
            var item = SitecoreX.Database.GetItem(id);

            return RedirectToSitecoreItem(item, q);
        }

        /// <summary>
        /// Added. This function was not available in Production and was not merged before.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        protected ActionResult RedirectToSitecoreItem(Item item, QueryString q)
        {
            if (item != null)
            {
                var url = LinkManager.GetItemUrl(item, LinkOptions.Url);
                url = q.CombineWith(url);
                return Redirect(url);
                //return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = url.TrimStart(new[] { '/' }) });
            }
            return HttpNotFound();
        }

        /// <summary>
        /// Attempts to locate and redirect to the desired Sitecore item
        /// </summary>
        /// <param name="item">The Sitecore item to redirect to</param>
        /// <returns></returns>
        protected ActionResult RedirectToSitecoreItem(Item item)
        {
            if (item != null)
            {
                var url = LinkManager.GetItemUrl(item, LinkOptions.Url);

                return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = url.TrimStart(new[] { '/' }) });
            }
            return HttpNotFound();
        }

        protected string GetSitecoreUrl(string path)
        {
            var item = SitecoreX.Database.GetItem(path);
            return LinkManager.GetItemUrl(item, LinkOptions.Url);
        }

        protected string GetSitecoreUrlByID(string itemId)
        {
            var item = Sitecore.Context.Database.GetItem(new ID(itemId));
            return LinkManager.GetItemUrl(item, LinkOptions.Url);
        }

        protected ChildList GetDictionaryListByKey(string key, string dictionaryPath = DictionaryKeys.DictionaryPath)
        {
            return _contentRepository.GetItem<Item>(new GetItemByPathOptions(dictionaryPath)).Children[key].GetChildren();
        }

        protected void StoreProfilePhoto(string username, string sessionTokenNumber)
        {
            var profilePhotoModel = new ProfilePhotoModel { HasProfilePhoto = false };
            CacheProvider.Store(CacheKeys.HAS_PROFILE_PHOTO, new CacheItem<bool>(false, TimeSpan.FromHours(10)));
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(sessionTokenNumber))
            {
                var profilePhotoResponse = DewaApiClient.GetProfilePhoto(username, sessionTokenNumber, RequestLanguage, Request.Segment());
                if (profilePhotoResponse.Succeeded && profilePhotoResponse.Payload != null)
                {
                    if (profilePhotoResponse.Payload.Length > 0)
                    {
                        profilePhotoModel.HasProfilePhoto = true;
                        profilePhotoModel.ProfilePhoto = profilePhotoResponse.Payload;
                        CacheProvider.Store(CacheKeys.HAS_PROFILE_PHOTO, new CacheItem<bool>(true, TimeSpan.FromHours(10)));
                    }
                }
            }

            CacheProvider.Store(CacheKeys.PROFILE_PHOTO, new CacheItem<ProfilePhotoModel>(profilePhotoModel, TimeSpan.FromHours(10)));
        }

        protected bool AttachmentIsValid(HttpPostedFileBase attachment, long maxBytes, out string error, params string[] allowedExtensions)
        {
            error = null;
            allowedExtensions = allowedExtensions ?? new string[0];

            //Get File Size Value from Sitecore if not set then take it from Constants.
            Item maxAttachmentSize = SitecoreX.Database.GetItem(General.Max_Attachment_Size);
            maxBytes = maxAttachmentSize != null && maxAttachmentSize.Fields["Text"] != null ? Convert.ToInt64(maxAttachmentSize.Fields["Text"].ToString()) : maxBytes;

            //Get File Extension Value from Sitecore if not set then take it from Constant.
            Item acceptedFileTypes = SitecoreX.Database.GetItem(General.Accepted_File_Types);
            string acceptedFileType = acceptedFileTypes != null && acceptedFileTypes.Fields["Text"] != null ? acceptedFileTypes.Fields["Text"].ToString() : string.Empty;
            allowedExtensions = !string.IsNullOrEmpty(acceptedFileType) ? acceptedFileType.Split(',').Select(extType => extType.Trim()).ToArray() : allowedExtensions;

            if (attachment != null && attachment.ContentLength > 0)
            {
                if (attachment.ContentLength > maxBytes)
                {
                    error = Translate.Text("file too large validation message");
                    return false;
                }

                var ext = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrWhiteSpace(ext) && allowedExtensions.Any())
                {
                    error = Translate.Text("invalid file type validation message");
                    return false;
                }

                if (allowedExtensions.Any() && !allowedExtensions.Any(e => e.Equals(ext, StringComparison.InvariantCultureIgnoreCase)))
                {
                    error = Translate.Text("invalid file type validation message");
                    return false;
                }
                if (!attachment.ContentType.Equals(MimeExtensions.GetMimeType(ext)))
                {
                    error = Translate.Text("invalid file type validation message");
                    return false;
                }
                using (var basememoryStream = new MemoryStream())
                {
                    attachment.InputStream.CopyTo(basememoryStream);
                    var invalidfile = IsExecutableFile(basememoryStream.ToArray());
                    attachment.InputStream.Position = 0;
                    if (invalidfile)
                    {
                        error = Translate.Text("invalid file type validation message");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                error = Translate.Text("no file attached");
                return false;
            }
        }

        /// <summary>
        /// https://filesignatures.net/index.php?page=all&order=EXT&alpha=P
        ///  https://www.aspsnippets.com/questions/291996/Validate-file-extension-with-magic-number-using-C-and-VBNet-in-ASPNet/
        ///  https://www.aspsnippets.com/questions/291996/Validate-file-extension-with-magic-number-using-C-and-VBNet-in-ASPNet/
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetFileTypeByBytes(byte[] bytes)
        {
            string contenttype = "";
            try
            {
                string data_as_hex = BitConverter.ToString(bytes);
                string magicCheck = data_as_hex.Substring(0, 11);

                switch (magicCheck)
                {
                    case "FF-D8-FF-E0"://JPEG IMAGE
                    case "FF D8 FF E8"://Still Picture Interchange File Format (SPIFF)
                    case "FF-D8-FF-E1"://Digital camera JPG using Exchangeable Image File Format(EXIF)
                    case "FF-D8-FF-E2"://CANNON EOS JPEG FILE
                    case "FF-D8-FF-E3"://SAMSUNG D500 JPEG FILE
                        contenttype = "image/jpg|image/jpeg|image/jfif|image/jpe";
                        break;

                    case "00-00-00-0C"://JPEG2000 image files
                        contenttype = "image/JP2";
                        break;

                    case "89-50-4E-47":// PNG file
                        contenttype = "image/png";
                        break;
                    //case "42-4D"://Bitmap image
                    //    contenttype = "image/bmp";
                    //    break;
                    case "25-50-44-46":
                        contenttype = "application/pdf|text/pdf";
                        break;
                }
                if (string.IsNullOrWhiteSpace(contenttype))
                {
                    #region [Bitmap image]
                    if (magicCheck.Substring(0, 5) == "42-4D")
                    {
                        contenttype = "image/bmp";
                    }
                    #endregion [Bitmap image]
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, null);
            }
            return contenttype;
        }

        public static bool IsExecutableFile(byte[] FileContent)
        {
            var twoBytes = SubByteArray(FileContent, 0, 2);
            return ((System.Text.Encoding.UTF8.GetString(twoBytes) == "MZ") || (System.Text.Encoding.UTF8.GetString(twoBytes) == "ZM"));
        }

        private static byte[] SubByteArray(byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        protected bool CustomeAttachmentIsValid(HttpPostedFileBase attachment, long maxBytes, out string error, params string[] allowedExtensions)
        {
            error = null;
            allowedExtensions = allowedExtensions ?? new string[0];

            //Get File Size Value from Sitecore if not set then take it from Constants.
            if (maxBytes < 0)
            {
                Item maxAttachmentSize = SitecoreX.Database.GetItem(General.Max_Attachment_Size);
                maxBytes = maxAttachmentSize != null && maxAttachmentSize.Fields["Text"] != null ? Convert.ToInt64(maxAttachmentSize.Fields["Text"].ToString()) : maxBytes;
            }

            //Get File Extension Value from Sitecore if not set then take it from Constant.
            if (allowedExtensions.Length < 0)
            {
                Item acceptedFileTypes = SitecoreX.Database.GetItem(General.Accepted_File_Types);
                string acceptedFileType = acceptedFileTypes != null && acceptedFileTypes.Fields["Text"] != null ? acceptedFileTypes.Fields["Text"].ToString() : string.Empty;
                allowedExtensions = !string.IsNullOrEmpty(acceptedFileType) ? acceptedFileType.Split(',').Select(extType => extType.Trim()).ToArray() : allowedExtensions;
            }

            if (attachment != null && attachment.ContentLength > 0)
            {
                if (attachment.ContentLength > maxBytes)
                {
                    error = Translate.Text("file too large validation message");
                    return false;
                }

                var ext = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrWhiteSpace(ext) && allowedExtensions.Any())
                {
                    error = Translate.Text("invalid file type validation message");
                    return false;
                }

                if (allowedExtensions.Any() && !allowedExtensions.Any(e => e.Equals(ext, StringComparison.InvariantCultureIgnoreCase)))
                {
                    error = Translate.Text("invalid file type validation message");
                    return false;
                }
                if (!attachment.ContentType.Equals(MimeExtensions.GetMimeType(ext)))
                {
                    error = Translate.Text("invalid file type validation message");
                    return false;
                }
                using (var basememoryStream = new MemoryStream())
                {
                    attachment.InputStream.CopyTo(basememoryStream);

                    bool IsValidExtensionByMemory = GetFileTypeByBytes(basememoryStream.ToArray()).Split('|').ToArray().Contains(attachment.ContentType);
                    var validfile = IsValidExtensionByMemory && !IsExecutableFile(basememoryStream.ToArray());
                    attachment.InputStream.Position = 0;
                    if (!validfile)
                    {
                        error = Translate.Text("invalid file type validation message");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                error = Translate.Text("no file attached");
                return false;
            }
        }

        public static string Base64Encode(string plainText) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));

        public static string Base64Decode(string base64EncodedData) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));

        #endregion Methods

        #region User values

        protected bool IsLoggedIn
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated && !SitecoreX.IsAdministrator)
                {
                    var authCookie = Request.Cookies.Get(GenericConstants.AntiHijackCookieName);
                    var storedToken = Session[GenericConstants.AntiHijackCookieName] != null ? Session[GenericConstants.AntiHijackCookieName].ToString() : null;

                    if (authCookie != null && string.Equals(storedToken, authCookie.Value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected bool IsEpassLoggedIn
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated && !SitecoreX.IsAdministrator)
                {
                    var authCookie = Request.Cookies.Get(GenericConstants.AntiHijackCookieName);
                    var storedToken = Session[GenericConstants.AntiHijackCookieName] != null ? Session[GenericConstants.AntiHijackCookieName].ToString() : null;

                    if (authCookie != null && string.Equals(storedToken, authCookie.Value))
                    {
                        authCookie.Expires = DateTime.Now.AddMinutes(20);
                        Session.Timeout = 20;
                        return true;
                    }
                }
                return false;
            }
        }

        protected ProfilePhotoModel CurrentUserProfilePhoto
        {
            get
            {
                ProfilePhotoModel photo;
                bool hasprofilephoto;
                if (!CacheProvider.TryGet(CacheKeys.HAS_PROFILE_PHOTO, out hasprofilephoto))
                {
                    if (!string.IsNullOrEmpty(CurrentPrincipal.UserId) &&
                        !string.IsNullOrEmpty(CurrentPrincipal.SessionToken))
                    {
                        this.StoreProfilePhoto(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
                    }
                }
                if (CacheProvider.TryGet(CacheKeys.PROFILE_PHOTO, out photo))
                {
                    return photo;
                }

                return new ProfilePhotoModel() { HasProfilePhoto = false, ProfilePhoto = new byte[0] }; ;
            }
        }

        protected SupportedLanguage RequestLanguage
        {
            get
            {
                return "en".Equals(SitecoreX.Language.CultureInfo.TwoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase) ?
                    SupportedLanguage.English : SupportedLanguage.Arabic;
            }
        }

        protected string RequestLanguageCode
        {
            get
            {
                return DEWAXP.Foundation.Integration.Enums.SupportedLanguageExtensions.Code(RequestLanguage);
            }
        }

        protected ReraQsValues GetAccountValues(string querystringvalue)
        {
            var qs = Request.QueryString[querystringvalue];
            if (!string.IsNullOrEmpty(qs))
            {
                var values = ReraHelper.GetReraValuesFromQuerystring(qs);
                if (!string.IsNullOrEmpty(values))
                {
                    var split = values.Split('|');
                    if (split.Length == 2)
                    {
                        return new ReraQsValues()
                        {
                            BusinessPartner = split[0],
                            ContractAccount = split[1]
                        };
                    }
                }
            }
            return null;
        }

        protected class ReraQsValues
        {
            public string BusinessPartner { get; set; }
            public string ContractAccount { get; set; }
        }

        public FutureCenterValues FetchFutureCenterValues()
        {
            FutureCenterValues values = new FutureCenterValues();

            if (Request.Cookies["fdc"] != null)
            {
                string[] _cookie = Request.Cookies["fdc"].Value.Split('|');
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

        public string FormatInternationalMobileNumber(string CountryCode, string MobileNumber)
        {
            if (!string.IsNullOrWhiteSpace(CountryCode) && !string.IsNullOrWhiteSpace(MobileNumber))
            {
                if (CountryCode.Length == 1)
                {
                    CountryCode = "+" + CountryCode;
                }
                else if (CountryCode.Equals("971") || CountryCode.Equals("+971"))
                {
                    MobileNumber = MobileNumber.RemoveMobileNumberZeroPrefix();
                }
                return CountryCode + MobileNumber;
            }
            return string.Empty;
        }

        #endregion User values

        #region Accounts List

        /// <summary>
        /// Added by mayank to get & cache the accounts list
        /// </summary>
        /// <param name="includeBillingDetails"></param>
        /// <returns></returns>
        public ServiceResponse<AccountDetails[]> GetAccounts(bool includeBillingDetails)
        {
            if (includeBillingDetails)
            {
                ServiceResponse<AccountDetails[]> response;
                if (!CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST_WITH_BILLING, out response) || response.Payload.Length < 1)
                {
                    //response = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, includeBillingDetails, RequestLanguage, Request.Segment());
                    response = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, includeBillingDetails, RequestLanguage, Request.Segment());

                    if (response.Succeeded)
                    {
                        CacheProvider.Store(CacheKeys.ACCOUNT_LIST_WITH_BILLING, new CacheItem<ServiceResponse<AccountDetails[]>>(response, TimeSpan.FromHours(1)));
                    }
                }
                return response;
            }
            else
            {
                ServiceResponse<AccountDetails[]> response;
                if (!CacheProvider.TryGet(CacheKeys.ACCOUNT_LIST, out response) || response.Payload.Length < 1)
                {
                    //response = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, includeBillingDetails, RequestLanguage, Request.Segment());
                    response = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, includeBillingDetails, RequestLanguage, Request.Segment());

                    if (response.Succeeded)
                    {
                        CacheProvider.Store(CacheKeys.ACCOUNT_LIST, new CacheItem<ServiceResponse<AccountDetails[]>>(response, TimeSpan.FromHours(1)));
                    }
                }
                return response;
            }
        }

        /// <summary>
        /// Added by mayank to get & cache the accounts list
        /// when cache flag is flase it will fetch the account list again
        /// includeBillingDetails when true gets the active
        /// </summary>
        /// <param name="includeBillingDetails"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public ServiceResponse<AccountDetails[]> GetBillingAccounts(bool includeBillingDetails, bool cache, string checkMoveOut, string cacheKey = "", string ServiceFlag = "")
        {
            if (string.IsNullOrWhiteSpace(cacheKey) && includeBillingDetails)
            {
                cacheKey = CacheKeys.ACCOUNT_LIST_WITH_BILLING;
            }
            else
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                cacheKey = CacheKeys.ACCOUNT_LIST;
            }

            ServiceResponse<AccountDetails[]> response;
            //To be changed
            if (!CacheProvider.TryGet(cacheKey, out response) || response.Payload.Length < 1)
            {
                response = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, checkMoveOut, ServiceFlag, includeBillingDetails, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    if (cache)
                        CacheProvider.Store(cacheKey, new CacheItem<ServiceResponse<AccountDetails[]>>(response, TimeSpan.FromHours(1)));
                }
            }
            return response;
        }

        #endregion Accounts List

        #region

        public IEnumerable<SelectListItem> GetLstDataSource(string datasource)
        {
            try
            {
                var dataSource = _contentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(datasource));
                if (dataSource != null && dataSource.Items != null)
                {
                    var convertedItems = dataSource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, new object());
                //throw new System.Exception("Error in getting Datasource");
            }
            return Enumerable.Empty<SelectListItem>();
        }

        #endregion

        #region getClearstr

        public string getClearstr(string input)
        {
            string cleanStr = "";
            try
            {
                Regex regex = new Regex(Translate.Text("SS_Regex_Pattern"));
                cleanStr = regex.Replace(input, "");
            }
            catch (System.Exception)
            {
                cleanStr = "";
            }
            return cleanStr;
        }

        #endregion

        #region [Cipher Payment Function]

        public _securePayWebModel.CipherPaymentModel ExecutePaymentGateway(_securePayWebModel.CipherPaymentModel model, bool isRammasFacebookPayment = false)
        {
            _securePayWebModel.CipherPaymentModel paymentReturnData = model;
            try
            {
                string _userId = Convert.ToString(model.PaymentData?.userid ?? CurrentPrincipal?.UserId);
                string _sessionId = isRammasFacebookPayment ? model.PaymentData.sessionid : CurrentPrincipal.SessionToken;

                paymentReturnData.ErrorMessages = new Dictionary<string, string>();
                paymentReturnData.PayPostModel = new _securePayWebModel.CipherPaymentPostModel()
                {
                    PaymentUrl = WebHandler.CipherPaymentHandler.GetSecuredPayUrl(model.PaymentMethod),
                    u = _userId,
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
                #endregion
                string ServiceTypeFlagKey;
                if (model.ErrorMessages.Count == 0)
                {
                    _securePayRestModel.CipherPaymentDetailInputs EpayTokenparams = model.PaymentData;
                    #region [binding default value]
                    EpayTokenparams.redirectflag = Config.SecuredPayurlRedirect;
                    EpayTokenparams.userid = _userId;
                    EpayTokenparams.sessionid = _sessionId;
                    EpayTokenparams.transactiontype = model.ContextType.EPayTransactionCode(Request.Segment());
                    EpayTokenparams.paymode = PaymentContextExtensions.GetPaymentMode(model.PaymentMethod);
                    EpayTokenparams.branch = FetchFutureCenterValues().Branch;
                    EpayTokenparams.tendertransactionid = string.IsNullOrWhiteSpace(model.PaymentData.tendertransactionid) ? "STATIC" : model.PaymentData.tendertransactionid;
                    EpayTokenparams.bk = Convert.ToString(!string.IsNullOrWhiteSpace(model.BankKey) && model.PaymentMethod == PaymentMethod.PaythroughNetbanking ? model.BankKey : null);
                    EpayTokenparams.suqiaamt = GetSuqiaAmount(model.SuqiaValue,model.SuqiaAmt);
                    #endregion

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
                            #endregion
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
                            #endregion
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
                            #endregion
                            break;

                        case ServiceType.EVAnonymous:
                            paymentReturnData.PayPostModel.u = string.Empty;  // this is anonymous so we are not sending the userid
                            EpayTokenparams.userid = string.Empty;
                            EpayTokenparams.sessionid = string.Empty;
                            break;

                        case ServiceType.EstimatePayment:
                            break;
                    }
                    #endregion

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
                    #endregion
                }
            }
            catch (Exception ex)
            {
                paymentReturnData.ErrorMessages.Add("error4", ErrorMessages.UNEXPECTED_ERROR);
                LogService.Error(ex, this);
            }
            return paymentReturnData;
        }

        #endregion
        private string GetSuqiaAmount(string suqiavalue, string suqiamt)
        {
            var suqiaList = Utils.PaymentPopupHelper.GetSuqiaDonationList();
            if (suqiaList!=null && suqiaList.Count >0 && !string.IsNullOrWhiteSpace(suqiamt)) 
            {
                try
                {
                    if (suqiaList.Any(x => x.Value.Equals(suqiavalue)))
                    {
                        if(!suqiavalue.Equals("other"))
                        {
                            suqiamt = suqiaList.FirstOrDefault(x => x.Value.Equals(suqiavalue)).Text.Replace(Translate.Text("AED"), "");
                        }
                        var decSuqiaAmt = Convert.ToDecimal(suqiamt);
                        return decSuqiaAmt.ToString();
                    }
                }
                catch(Exception ex)
                {
                    LogService.Error(ex, this);
                }
            }
            return string.Empty;
        }
    }
}