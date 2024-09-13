using DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundHistory;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.RefundHistory;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IRefundHistoryClient
    {
        //verifyotp
        ServiceResponse<RefundHistoryResponse> VerifyOtp(RefundHistoryVerifyOtpRequest request, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<IBANNumberV2Response> IbanNumberv2(IBANNumberV2Request request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
