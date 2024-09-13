using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.QmsCustSvc;
using DEWAXP.Foundation.Integration.Requests.QmsSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.QmsSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface IQmsServiceClient
    {
        /// <summary>
        /// Get issue new token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="language"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        ServiceResponse<IssueTicketResponse> IssueTicket(IssueTicketReq request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<ServiceStatusListResponse> GetServiceStatusList(ServiceStatusListReq request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<BranchServiceStatusResponse> GetBranchServiceStatusList(BranchServiceStatusReq request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

    }
}
