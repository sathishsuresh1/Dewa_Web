using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.NCR;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.RTA;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.RTAPARAM;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.Tr;

namespace DEWAXP.Foundation.Integration
{
    public interface IEVServiceClient
    {
        //ServiceResponse<QRCodeResponse> getQRCodeVerified(string certificatetype, string referencenumber, string pinnumber, SupportedLanguage language=SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<EVCardResponse> SetNewEVGreenCard(string bpCategory, string bpNumber,string emailId, string mobileNumber, string cardIdType, string cardIdNumber,string file1Name, string file1Data, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string processFlag = "", string password = "", string title = "", string firstName = "", string lastName = "", string notionality = "",  string Emirate = "", string poBox = "", string noOfCars = "", string userCreateFlag = "", string idType = "", string idNumber = "", string file2Name = "", string file2Data = "");
        ServiceResponse<RestBaseResponse> DeActivateEVGreenCard(string bpNumber,string contractAccount, string deactivateDate, string mobileNumber, string cardNumber, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<ReplaceEVCard> ReplaceEVGreenCard(string accountNumber, string Reason,  string processtFlag,string cardFee, string taxAmount, string taxRate, string totalAmount, string userId, string sessionId, string cardnumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        #region New Changes regarding RTA integration

        ServiceResponse<NewCardResponse> SetNewEVGreenCardFinal(string bpCategory, string bpNumber, string emailId, string mobileNumber, string cardIdType, string cardIdNumber, string file1Name, string file1Data, string userId, string sessionId, string trafficFileNumber, string carRegistratedRegion, string carRegistratedCountry, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string processFlag = "", string password = "", string title = "", string firstName = "", string lastName = "", string notionality = "", string Emirate = "", string poBox = "", string noOfCars = "", string userCreateFlag = "", string idType = "", string idNumber = "", string file2Name = "", string file2Data = "");

        ServiceResponse<NewCardResponse> SetNewEVGreenCard(string bpCategory, string bpNumber, string emailId, string mobileNumber, string cardIdType, string cardIdNumber, string file1Name, string file1Data, string userId, string sessionId, string trafficFileNumber, string carRegistratedRegion, string carRegistratedCountry, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string processFlag = "", string password = "", string title = "", string firstName = "", string lastName = "", string notionality = "", string Emirate = "", string poBox = "", string noOfCars = "", string userCreateFlag = "", string idType = "", string idNumber = "", string file2Name = "", string file2Data = "", string tradelicenceauthorityname = "", string tradelicenceauthoritycode = "");

        ServiceResponse<EVTrackResponse> GetNotifications(string bpNumber, string sessionId, string userId, string clientIPAddress = "", string contractAccNo = "", string serviceCode = "", SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<Responses.EVGreenCard.Trd.EVTrackResponseDetail> GetNotificationDetail(string bpNumber, string sessionId, string userid, string notificationNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<EVUserRTADetailsResponse> GetEVUserRTADetails(string bpCategory, string trafficFileNumber, string carRegistrationCountry, string carRegistrationRegion, string carIdNumber, string carIdType = "P", SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);

        ServiceResponse<EVParamResponse> GetEVParamCarDetails(string urlParam, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        ServiceResponse<APIHandler.Models.Response.SetNewEVGreenCardV3Response> SetNewEVGreenCardV3(APIHandler.Models.Request.SetNewEVGreenCardV3Request request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop);
        #endregion
    }
}
