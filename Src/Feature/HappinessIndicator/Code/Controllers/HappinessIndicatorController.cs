using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Feature.HappinessIndicator.Models.HappinessIndicator;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Integration.DewaSvc;
using Sitecore.Mvc.Controllers;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.HappinessIndicator;

namespace DEWAXP.Feature.HappinessIndicator.Controllers
{
    public class HappinessIndicatorController : BaseController
    {
        //private IContentRepository _contentRepository;
        //private ICacheProvider CacheProvider;
        //private DewaProfile CurrentPrincipal;
        //private IDewaAuthStateService AuthStateService;

        //public HappinessIndicatorController(IContentRepository contentRepository,
        //    ICacheProvider cacheProvider,
        //    DewaProfile dewaProfile,
        //    IDewaAuthStateService dewaAuthStateService)
        //{
        //    _contentRepository = contentRepository;
        //    CacheProvider = cacheProvider;
        //    CurrentPrincipal = dewaProfile;
        //    AuthStateService = dewaAuthStateService;
        //}

        public PartialViewResult Render(string transactionId, string serviceCode, string type = IndicatorType.Application, bool islocal = false, string ServiceDescription = "")
        {
            GetEasyPayEnquiryResponse easypaymentpath;
            if (serviceCode != null && serviceCode == "X")
            {
                CacheProvider.TryGet(CacheKeys.Easy_Pay_Response, out easypaymentpath);
                if (easypaymentpath != null && easypaymentpath.@return != null)
                {
                    serviceCode = GetServiceCode("EasyPay" + easypaymentpath.@return?.transactiontype ?? "");
                    ServiceDescription = "EasyPay" + easypaymentpath.@return?.transactiontype ?? "";
                }
                CacheProvider.Remove(CacheKeys.Easy_Pay_Response);
            }
            return PartialView("~/Views/Feature/HappinessIndicator/Renderings/_Indicator.cshtml", new HappinessIndicatorViewModel(type, transactionId, ShouldOpenAutomatically(islocal), serviceCode, ServiceDescription, islocal));
        }

        public ActionResult PostData(string language, string type, string transactionId, string serviceCode, bool islocal = false, string ServiceDescription = "")
        {
            string json;
            HappinessIndicatorConfig happinessIndicatorConfig = new HappinessIndicatorConfig();

            var clientId = happinessIndicatorConfig.ClientIdentifier;
            var postUrl = happinessIndicatorConfig.PostUrl;
            var serviceProvider = happinessIndicatorConfig.ServiceProvider;
            var gessEnabled = happinessIndicatorConfig.GessEnabled;
            var channel = happinessIndicatorConfig.Channel;
            var applicationId = happinessIndicatorConfig.ApplicationIdentifier;
            var applicationType = happinessIndicatorConfig.ApplicationType;
            var version = happinessIndicatorConfig.Version;
            var secretKey = happinessIndicatorConfig.SecretKey;
            var themeColor = happinessIndicatorConfig.ThemeColor;
            //var serviceCode = HappinessIndicatorConfig.ServiceCode;
            //var serviceDescription = HappinessIndicatorConfig.ServiceDescription;

            var srvName = HttpUtility.UrlDecode(Request.UrlReferrer.AbsoluteUri).ToString();
            var sourceType = !User.Identity.IsAuthenticated ? "ANONYMOUS" : "LOCAL";
            var sessionNo = CurrentPrincipal.SessionToken;
            var isMyIdLogin = CurrentPrincipal.IsMyIdUser;
            var username = CurrentPrincipal.UserId;
            string serviceDescription = string.Empty, dsgServiceCode = string.Empty;

            //TODO: When EmiratesID is available, update
            var email = CurrentPrincipal.EmailAddress;
            var mobile = CurrentPrincipal.MobileNumber;
            var emiratesId = "";//"789-1998-87689898";

            if (String.IsNullOrEmpty(sessionNo))
                sourceType = "ANONYMOUS";
            else if (!String.IsNullOrEmpty(sessionNo) && !isMyIdLogin)
                sourceType = "LOCAL";
            else if (!String.IsNullOrEmpty(sessionNo) && isMyIdLogin)
                sourceType = "MYID";

            var dubai = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            var dateAndOffset = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, dubai);
            var date = dateAndOffset.DateTime;
            var jsonTimestamp = date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            if (type == IndicatorType.Transaction)
            {
                if (islocal)
                {
                    serviceDescription = ServiceDescription;
                    dsgServiceCode = serviceCode;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(serviceCode))
                    {
                        var servicedesc = HappinessIndicatorExtensions.Parse(serviceCode);
                        serviceDescription = servicedesc.ToString();
                        dsgServiceCode = GetServiceCode(servicedesc.ToString());
                    }
                }
                json = "{\"header\":{"
                       + "\"timestamp\" : \"" + jsonTimestamp + "\","
                       + "\"themeColor\" : \"" + themeColor + "\","
                       + "\"serviceProvider\" : \"" + serviceProvider + "\"},"

                       + "\"transaction\":{"
                       + "\"transactionID\" : \"" + transactionId + "\","
                       + "\"gessEnabled\" : \"" + gessEnabled + "\","
                       + "\"serviceCode\" : \"" + dsgServiceCode + "\","
                       + "\"serviceDescription\" : \"" + serviceDescription + "\","
                       + "\"channel\" : \"" + channel + "\","
                       + "\"result\" : \"\","
                       + "\"notes\" : \"" + srvName + "\"},"

                       + "\"user\":{"
                       + "\"source\" : \"" + sourceType + "\","
                    + "\"emiratesID\" : \"" + emiratesId + "\","
                    + "\"username\" :  \"" + username + "\","
                    + "\"email\" :  \"" + email + "\","
                    + "\"mobile\" :  \"" + mobile + "\"}}";
            }
            else
            {
                json = "{\"header\":{"
                       + "\"timestamp\" : \"" + jsonTimestamp + "\","
                       + "\"themeColor\" : \"" + themeColor + "\","
                       + "\"serviceProvider\" : \"" + serviceProvider + "\"},"

                       + "\"application\":{"

                       + "\"applicationID\" : \"" + applicationId + "\","
                       + "\"type\" : \"" + applicationType + "\","
                       + "\"platform\" : \"" + string.Empty + "\","
                       + "\"url\" : \"" + srvName + "\","
                       + "\"version\" : \"" + version + "\","
                       + "\"result\" : \"\","
                       + "\"notes\" :  \"\"},"

                       + "\"user\":{"
                       + "\"source\" : \"" + sourceType + "\","
                       + "\"emiratesID\" : \"" + "" + "\","
                       + "\"username\" :  \"" + username + "\","
                       + "\"email\" :  \"" + "" + "\","
                       + "\"mobile\" :  \"" + "" + "\"}}";
            }

            // Setup variables
            var signature = GetCrypt(json + "|" + secretKey);
            var jsonRequest = Server.UrlEncode(json);
            var localtimestamp = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var localrandom = GenerateRnd();
            var random = Server.UrlEncode(localrandom);
            var timestamp = Server.UrlEncode(localtimestamp);
            var nonce = (GetCrypt(localrandom + "|" + localtimestamp + "|" + secretKey));
            var lang = language == "en" ? "en" : "ar";

            // Build the model and render
            var viewModel = new HappinessIndicatorModel(postUrl, jsonRequest, clientId, signature, lang, timestamp, random, nonce);
            return View("~/Views/Feature/HappinessIndicator/Renderings/PostData.cshtml", viewModel);
        }

        #region Private

        private bool ShouldOpenAutomatically(bool islocal)
        {
            if (islocal)
            {
                return islocal;
            }
            else
            {
                if (!Request.QueryString.AllKeys.Contains("t")) return false;
                if (!Request.QueryString.AllKeys.Contains("s")) return false;

                return "success".Equals(Request.QueryString["s"].ToLower());
            }
        }

        private string GetCrypt(string text)
        {
            SHA512 alg = SHA512.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(Convert.ToString((result[i] & 0xff) + 0x100, 16).Substring(1));
            }
            return sb.ToString();
        }

        private string GenerateRnd()
        {
            int maxSize = 15;
            //int minSize = 15;
            char[] chars = new char[63];
            string a = null;
            a = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            chars = a.ToCharArray(0, 16);
            int size = maxSize;
            byte[] data = new byte[2];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size + 1];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }

        private string GetServiceCode(string serviceName)
        {
            string srvCode = string.Empty;
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                var happinessSrvCodes = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.HAPPINESS_SERVICE_CODES));
                if (happinessSrvCodes != null)
                {
                    var convertedItems = happinessSrvCodes.Items.Where(c => c.Text.ToLower().Equals(serviceName.ToLower())).FirstOrDefault();
                    srvCode = convertedItems?.Value ?? string.Empty;
                }
            }

            return srvCode;
        }

        #endregion Private
    }
}