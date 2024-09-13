using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Estimate;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IEstimateClient
    {
        ServiceResponse<NewConnectionRefundResponse> SmartCommunicationSubmit(NewConnectionRefundRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<EstimateDetailsResponse> EstimateDetails(EstimateDetailsRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<EstimatePaymentHistoryResponse> EstimateHistory(EstimatePaymentHistoryRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<EstimateCustomerListResponse> EstimateCustomerlist(EstimateCustomerListResquest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<EstimatePdfResponse> EstimatePDF(EstimatePdfRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<EstimateAmountDisplayResponse> EstimateAmountDisplay(EstimateAmountDisplayRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<NewConnectionTaxInvoicePdfResponse> NewConnectionTaxInvoicePdf(NewConnectionTaxInvoicePdfRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
        ServiceResponse<PaymentReceiptDetailsResponse> OnlinePaymentReceipt(PaymentReceiptDetailsRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);
    }
}
