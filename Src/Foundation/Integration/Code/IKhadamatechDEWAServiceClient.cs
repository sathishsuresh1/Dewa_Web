using DEWAXP.Foundation.Integration.KhadamatechDEWASvc;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface IKhadamatechDEWAServiceClient
    {

        ServiceResponse<CreateReqResponse> CreateReq(CreateReqRequest request);
    }
}
