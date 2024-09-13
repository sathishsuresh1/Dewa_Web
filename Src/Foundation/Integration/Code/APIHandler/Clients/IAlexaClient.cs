using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Alexa;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration.APIHandler.Clients
{
    public interface IAlexaClient
    {
        ServiceResponse<Login_Res> ValidateCredential(string username, string password, string lang);
    }
}