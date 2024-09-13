using DEWAXP.Foundation.Integration.APIHandler.Models.Request.RefundNameChange;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.RefundNameChange;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IRefundNameChangeClient
    {
        ServiceResponse<RefundNameChangeResponse> RefundNameChange(RefundNameChangeRequest request, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English);

    }
}
