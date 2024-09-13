using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Premise;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Common;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.Premise;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests.JoinOwnership;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.JoinOwnership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IPremiseClient
    {
        ServiceResponse<PremiseDetailsResponse> GetDetails(PremiseDetailsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<JointOwnerResponse> PostJointOwnershipRequest(JointOwnerRequest request, SupportedLanguage language = SupportedLanguage.English);

        ServiceResponse<CommonResponse> SaveJointOwnershipAttachment(JointOwnerAttachmentRequest request);
    }
}
