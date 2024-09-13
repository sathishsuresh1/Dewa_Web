using DEWAXP.Foundation.Integration.Responses;
using System;

namespace DEWAXP.Foundation.Integration
{
    public interface IEmailServiceClient
    {
        //ServiceResponse<string> Send_Mail(string from, string to, string subject, string body);
        ServiceResponse<string> SendEmail(string from, string to, string subject, string body);
        ServiceResponse<string> SendEmail(string from, string to, string cc, string bcc, string subject, string body, System.Collections.Generic.List<Tuple<string, byte[]>> attachments);
        
    }
}
