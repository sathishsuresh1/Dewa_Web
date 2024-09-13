using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartResponseModel;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(ISmartResponseClient), Lifetime = Lifetime.Transient)]
    public class SmartReponseClient :ISmartResponseClient
    {
        public ServiceResponse<PredictState> GetPredict(string imagePath,bool consumption=false)
        {
            {
                try
                {
                    string apiurl = SmartResponseConfig.SM_PREDICT_API;
                    string apikey = SmartResponseConfig.SM_PREDICT_APIKEY;
                    if (consumption)
                    {
                        apiurl = ConsumptionComplaintResponseCofig.CC_PREDICT_API;
                        apikey = ConsumptionComplaintResponseCofig.CC_PREDICT_APIKEY;
                    }
                    var client = new RestClient(apiurl);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("apikey", apikey);
                    request.AddHeader("Content-Type", "multipart/form-data");
                    request.AddFile("file", imagePath, "file"); ;
                    IRestResponse<PredictState> res = client.Execute<PredictState>(request);
                    if (res.StatusCode == System.Net.HttpStatusCode.OK && res.Data != null)
                    {
                        return new ServiceResponse<PredictState>(res.Data);
                    }
                    else
                    {
                        return new ServiceResponse<PredictState>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                }
                return new ServiceResponse<PredictState>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
    }
}
