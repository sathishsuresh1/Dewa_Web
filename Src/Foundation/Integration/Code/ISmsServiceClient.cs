using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Integration
{
    public interface ISmsServiceClient
    {
        ServiceResponse<bool> Send_DEWA_SMS(string Moblie_Number, string Text_Message, string Application_Name, string Sender_Name, string SMS_Priority);
        ServiceResponse<bool> Send_DEWA_SMSAr(string Moblie_Number, string Text_Message, string Application_Name, string Sender_Name, string SMS_Priority);
    }
}
