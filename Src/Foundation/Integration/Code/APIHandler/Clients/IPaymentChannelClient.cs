using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Payment;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Payment;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IPaymentChannelClient
    {
        ServiceResponse<SamsungPayResponse> PaymentChannel(SamsungPayRequest Request, string channel, SupportedLanguage requestLanguage, RequestSegment segment = RequestSegment.Desktop);
        Task<string> GetMerchantSessionAsync(string request);
    }
}
