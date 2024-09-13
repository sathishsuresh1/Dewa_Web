using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using static Sitecore.Configuration.State;
using DEWAXP.Foundation.Content.Filters.Http;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using Sitecore;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Sitecore.Web.UI.WebControls;
using System.Security.Cryptography.X509Certificates;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Impl;
using System.Web.Helpers;
using static System.Net.WebRequestMethods;
using System.Web.Http;
using System.Web.Mvc;

namespace DEWAXP.Feature.Payment.Controllers.Api
{
    public class ApplePayController : BaseApiController
    {
        private readonly IPaymentChannelClient _paymentChannelClient;
        protected IPaymentChannelClient PaymentChannelApiClient => _paymentChannelClient;
        public ApplePayController() : base()
        {
            _paymentChannelClient = System.Web.Mvc.DependencyResolver.Current.GetService<PaymentChannelClient>();
        }
        //[System.Web.Http.AcceptVerbs(Http.Get)]
        //public Task<string> Validate(string validateurl)
        //{
        //    LogService.Error(new Exception("Validate apple"), this);

        //    LogService.Error(new Exception(validateurl), this);
        //    if (!ModelState.IsValid ||
        //        string.IsNullOrWhiteSpace(validateurl) ||
        //        !Uri.TryCreate(validateurl, UriKind.Absolute, out Uri requestUri))
        //    {
        //        return null;
        //    }

        //    try
        //    {
        //        // Create the JSON payload to POST to the Apple Pay merchant validation URL.
        //        var request = new MerchantSessionRequest()
        //        {
        //            DisplayName = "Dewa",
        //            Initiative = "web",
        //            InitiativeContext = "stagingnew.dewa.gov.ae",
        //            MerchantIdentifier = "merchant.sandboxenbd.ae.gov.dewa",
        //        };

        //        var merchantSession = _paymentChannelClient.GetMerchantSessionAsync(JsonConvert.SerializeObject(request));

        //        LogService.Info(new Exception(JsonConvert.SerializeObject(merchantSession)));
        //        // Return the merchant session as-is to the JavaScript as JSON.
        //        return merchantSession;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Info(ex);
        //    }
        //    return null;
        //}
        
    }

    public class HomeModel
    {
        public string MerchantId { get; set; } = "merchant.sandboxenbd.ae.gov.dewa";
        public string StoreName { get; set; } = "DEWA";
    }

    public class MerchantSessionRequest
    {
        [JsonProperty("merchantIdentifier")]
        public string MerchantIdentifier { get; set; } = "merchant.sandboxenbd.ae.gov.dewa";

        [JsonProperty("displayName")]
        public string DisplayName { get; set; } = "DEWA";

        [JsonProperty("initiative")]
        public string Initiative { get; set; }

        [JsonProperty("initiativeContext")]
        public string InitiativeContext { get; set; }
    }

    public class ApplePayOptions
    {
        public string StoreName { get; set; } = "DEWA";

        public bool UseCertificateStore { get; set; }

        public string MerchantCertificate { get; set; }

        public string MerchantCertificateFileName { get; set; } = "MerchantIdentitySandbox.pem";

        public string MerchantCertificatePassword { get; set; } = "Dewa@1234";

        public string MerchantCertificateThumbprint { get; set; } = "bedfbe2b6a09940ec48ded751ed528988606737c";
    }

    public class ValidateMerchantSessionModel
    {
        [DataType(DataType.Url)]
        [Required]
        public string ValidationUrl { get; set; }
    }
}