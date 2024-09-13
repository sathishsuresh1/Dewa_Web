using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartResponseModel;
using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface ISmartResponseClient
    {
        ServiceResponse<PredictState> GetPredict(string imagePath, bool consumption = false);
    }
}
