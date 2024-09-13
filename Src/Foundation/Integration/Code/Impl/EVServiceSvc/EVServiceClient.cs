using System;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Integration.Requests;
using System.Text;
using System.Web;
using System.Configuration;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.Tr;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.RTA;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.NCR;
using DEWAXP.Foundation.Integration.Responses.EVGreenCard.RTAPARAM;
using Return = DEWAXP.Foundation.Integration.Responses.EVGreenCard.RTAPARAM.Return;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Impl.EVLocationSvc;

namespace DEWAXP.Foundation.Integration.Impl.EVServiceSvc
{
    [Service(typeof(IEVServiceClient), Lifetime = Lifetime.Transient)]
    public class EVServiceClient : BaseDewaGateway, IEVServiceClient
    {
        #region SetNewEVGreenCard Methods
        public ServiceResponse<EVCardResponse> SetNewEVGreenCard(string bpCategory, string bpNumber, string emailId, string mobileNumber, string cardIdType, string cardIdNumber, string file1Name, string file1Data, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string processFlag = "", string password = "", string title = "", string firstName = "", string lastName = "", string notionality = "", string Emirate = "", string poBox = "", string noOfCars = "", string userCreateFlag = "", string idType = "", string idNumber = "", string file2Name = "", string file2Data = "")
        {
            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    var request = GetRequest("ev/greencard/apply");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("userId", userId);
                    dictionaryParam.Add("password", password);
                    dictionaryParam.Add("vendorId", GetVendorId(segment));
                    dictionaryParam.Add("sessionId", sessionId);
                    dictionaryParam.Add("processtFlag", processFlag); ////Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                    dictionaryParam.Add("bpCategory", bpCategory);
                    dictionaryParam.Add("bpNumber", bpNumber);
                    dictionaryParam.Add("emailId", emailId);
                    dictionaryParam.Add("mobileNumber", mobileNumber);
                    dictionaryParam.Add("cardIdType", cardIdType); //Mulkiya = M or Car Plate Number = P
                    dictionaryParam.Add("cardIdNumber", cardIdNumber); //Either Mulkiya or Car Plate Number
                    dictionaryParam.Add("addressTitle", title); // Title
                    dictionaryParam.Add("bpFirstName", firstName); //If account type business at that time pass company name here
                    dictionaryParam.Add("bpLastName", lastName);
                    dictionaryParam.Add("nationality", notionality);
                    // dictionaryParam.Add("branchName", companyName); //Company Name
                    dictionaryParam.Add("bpRegion", Emirate);
                    dictionaryParam.Add("poBox", poBox);
                    dictionaryParam.Add("noOfCars", noOfCars);
                    dictionaryParam.Add("userCreateFlag", userCreateFlag); // //User Creation flag "X" means create new user,if empty it will not create user
                    dictionaryParam.Add("idType", idType); // Z00002 = Emirates ID, Z00005 = Trade License and Z00001 = Passport No
                    dictionaryParam.Add("idNumber", idNumber); // Emirate ID, Passport Number or Trade Licence number
                    dictionaryParam.Add("file1Name", file1Name);
                    dictionaryParam.Add("file1Data", file1Data);
                    dictionaryParam.Add("file2Name", file2Name);
                    dictionaryParam.Add("file2Data", file2Data);

                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<EVCardResponse>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<EVCardResponse>(ResultData);
                        if (result != null && !string.IsNullOrWhiteSpace(result.responsecode) && result.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<EVCardResponse>(result);
                        }
                        else
                        {
                            return new ServiceResponse<EVCardResponse>(result, false, "Error Response");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
                return new ServiceResponse<EVCardResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region DeActivateEVGreenCard Methods
        public ServiceResponse<RestBaseResponse> DeActivateEVGreenCard(string bpNumber, string contractAccount, string deactivateDate, string mobileNumber, string cardNumber, string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    var request = GetRequest("ev/greencard/deactivate");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("userId", userId);
                    dictionaryParam.Add("vendorId", GetVendorId(segment));
                    dictionaryParam.Add("sessionId", sessionId);
                    dictionaryParam.Add("businessPartner", bpNumber);
                    dictionaryParam.Add("contractAccount", contractAccount);
                    dictionaryParam.Add("mobileNumber", mobileNumber);
                    dictionaryParam.Add("cardNumber", cardNumber);
                    dictionaryParam.Add("deactivationDate", deactivateDate);
                    dictionaryParam.Add("requestType", "D1");

                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<RestBaseResponse>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<RestBaseResponse>(ResultData);
                        if (result != null && !string.IsNullOrWhiteSpace(result.responsecode) && result.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<RestBaseResponse>(result);
                        }
                        else
                        {
                            return new ServiceResponse<RestBaseResponse>(result, false, "Error Response");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
                return new ServiceResponse<RestBaseResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region ReplaceEVGreenCard Methods
        public ServiceResponse<ReplaceEVCard> ReplaceEVGreenCard(string accountNumber, string reason, string processtFlag, string cardFee, string taxAmount, string taxRate, string totalAmount, string userId, string sessionId,string cardnumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    var request = GetRequest("ev/greencard/replace");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("userId", userId);
                    dictionaryParam.Add("vendorId", GetVendorId(segment));
                    dictionaryParam.Add("sessionId", sessionId);
                    dictionaryParam.Add("contractAccount", accountNumber);
                    dictionaryParam.Add("processFlag", processtFlag);
                    dictionaryParam.Add("replaceReason", reason);
                    dictionaryParam.Add("cardNumber", cardnumber);
                    //dictionaryParam.Add("returnType", processtFlag);
                    //dictionaryParam.Add("taxAmount", taxAmount);
                    //dictionaryParam.Add("taxRate", taxRate);
                    //dictionaryParam.Add("totalAmount", totalAmount);
                    //dictionaryParam.Add("cardFee", cardFee);


                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<ReplaceEVCard>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<ReplaceEVCard>(ResultData);
                        if (result != null && !string.IsNullOrWhiteSpace(result.responsecode) && result.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<ReplaceEVCard>(result);
                        }
                        else
                        {
                            return new ServiceResponse<ReplaceEVCard>(result, false, "Error Response");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
                return new ServiceResponse<ReplaceEVCard>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        public HttpRequestMessage GetRequest(string method)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings[ConfigKeys.RestAPI_Smart_Customer] + method);
            request.Headers.Add("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            return request;
        }

        #region New Changes regarding RTA integration

        public ServiceResponse<NewCardResponse> SetNewEVGreenCardFinal(string bpCategory, string bpNumber, string emailId, string mobileNumber, string cardIdType, string cardIdNumber, string file1Name, string file1Data, string userId, string sessionId, string trafficFileNumber, string carRegistratedRegion, string carRegistratedCountry, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string processFlag = "", string password = "", string title = "", string firstName = "", string lastName = "", string notionality = "", string Emirate = "", string poBox = "", string noOfCars = "", string userCreateFlag = "", string idType = "", string idNumber = "", string file2Name = "", string file2Data = "")
        {
            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    var request = GetRequest("ev/greencard/apply/v2");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("userId", userId);
                    dictionaryParam.Add("password", password);
                    dictionaryParam.Add("vendorId", GetVendorId(segment));
                    dictionaryParam.Add("sessionId", sessionId);
                    dictionaryParam.Add("processtFlag", processFlag); ////Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                    dictionaryParam.Add("bpCategory", bpCategory);
                    dictionaryParam.Add("bpNumber", bpNumber);
                    dictionaryParam.Add("emailId", emailId);
                    dictionaryParam.Add("mobileNumber", mobileNumber);
                    dictionaryParam.Add("cardIdType", cardIdType); //Mulkiya = M or Car Plate Number = P
                    dictionaryParam.Add("cardIdNumber", cardIdNumber); //Either Mulkiya or Car Plate Number
                    dictionaryParam.Add("addressTitle", title); // Title
                    dictionaryParam.Add("bpFirstName", firstName); //If account type business at that time pass company name here
                    dictionaryParam.Add("bpLastName", lastName);
                    dictionaryParam.Add("nationality", notionality);
                    // dictionaryParam.Add("branchName", companyName); //Company Name
                    dictionaryParam.Add("bpRegion", Emirate);
                    dictionaryParam.Add("poBox", poBox);
                    dictionaryParam.Add("noOfCars", noOfCars);
                    dictionaryParam.Add("userCreateFlag", userCreateFlag); // //User Creation flag "X" means create new user,if empty it will not create user
                    dictionaryParam.Add("idType", idType); // Z00002 = Emirates ID, Z00005 = Trade License and Z00001 = Passport No
                    dictionaryParam.Add("idNumber", idNumber); // Emirate ID, Passport Number or Trade Licence number
                    dictionaryParam.Add("file1Name", file1Name);
                    dictionaryParam.Add("file1Data", file1Data);
                    dictionaryParam.Add("file2Name", file2Name);
                    dictionaryParam.Add("file2Data", file2Data);
                    //new V2 related params
                    dictionaryParam.Add("trafficFileNumber", trafficFileNumber);
                    dictionaryParam.Add("carRegistratedRegion", carRegistratedRegion);
                    dictionaryParam.Add("carRegistratedCountry", carRegistratedCountry);

                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        LogService.Error(new Exception(response.ToString()), this);
                        return new ServiceResponse<NewCardResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<NewCardResponse>(ResultData);
                        var resp = result?.Envelope?.Body?.SetNewEVGreenCardV2Response?.@return;

                        if (resp != null && resp.responseCode.Equals("000"))
                        {
                            return new ServiceResponse<NewCardResponse>(result);
                        }
                        else
                        {
                            LogService.Error(new Exception(ResultData), this);
                            return new ServiceResponse<NewCardResponse>(result, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<NewCardResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<NewCardResponse> SetNewEVGreenCard(string bpCategory, string bpNumber, string emailId, string mobileNumber, string cardIdType, string cardIdNumber, string file1Name, string file1Data, string userId, string sessionId, string trafficFileNumber, string carRegistratedRegion, string carRegistratedCountry,
          SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop, string processFlag = "", string password = "", string title = "", string firstName = "", string lastName = "", string notionality = "", string Emirate = "", string poBox = "", string noOfCars = "", string userCreateFlag = "", string idType = "", string idNumber = "", string file2Name = "", string file2Data = "", string tradelicenceauthorityname = "", string tradelicenceauthoritycode = "")
        {
            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    var request = GetRequest("ev/greencard/apply/v2");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("userId", userId);
                    dictionaryParam.Add("password", password);
                    dictionaryParam.Add("vendorId", GetVendorId(segment));
                    dictionaryParam.Add("sessionId", sessionId);
                    dictionaryParam.Add("processtFlag", processFlag); ////Process Flag user from RFC if empty then valiadte data if "X" then submit data.
                    dictionaryParam.Add("bpCategory", bpCategory);
                    dictionaryParam.Add("bpNumber", bpNumber);
                    dictionaryParam.Add("emailId", emailId);
                    dictionaryParam.Add("mobileNumber", mobileNumber);
                    dictionaryParam.Add("cardIdType", cardIdType); //Mulkiya = M or Car Plate Number = P
                    dictionaryParam.Add("cardIdNumber", cardIdNumber); //Either Mulkiya or Car Plate Number
                    dictionaryParam.Add("addressTitle", title); // Title
                    dictionaryParam.Add("bpFirstName", firstName); //If account type business at that time pass company name here
                    dictionaryParam.Add("bpLastName", lastName);
                    dictionaryParam.Add("nationality", notionality);
                    // dictionaryParam.Add("branchName", companyName); //Company Name
                    dictionaryParam.Add("bpRegion", Emirate);
                    dictionaryParam.Add("poBox", poBox);
                    dictionaryParam.Add("noOfCars", noOfCars);
                    dictionaryParam.Add("userCreateFlag", userCreateFlag); // //User Creation flag "X" means create new user,if empty it will not create user
                    dictionaryParam.Add("idType", idType); // Z00002 = Emirates ID, Z00005 = Trade License and Z00001 = Passport No
                    dictionaryParam.Add("idNumber", idNumber); // Emirate ID, Passport Number or Trade Licence number
                    dictionaryParam.Add("file1Name", file1Name);
                    dictionaryParam.Add("file1Data", file1Data);
                    dictionaryParam.Add("file2Name", file2Name);
                    dictionaryParam.Add("file2Data", file2Data);
                    //new V2 related params
                    dictionaryParam.Add("trafficFileNumber", trafficFileNumber);
                    dictionaryParam.Add("carRegistratedRegion", carRegistratedRegion);
                    dictionaryParam.Add("carRegistratedCountry", carRegistratedCountry);
                    if (!string.IsNullOrEmpty(tradelicenceauthoritycode))
                    {
                        dictionaryParam.Add("tradelicenceauthoritycode", tradelicenceauthoritycode);
                    }
                    if (!string.IsNullOrEmpty(tradelicenceauthorityname))
                    {
                        dictionaryParam.Add("tradelicenceauthorityname", tradelicenceauthorityname);
                    }


                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        LogService.Error(new Exception(response.ToString()), this);
                        return new ServiceResponse<NewCardResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<NewCardResponse>(ResultData);
                        var res = result?.Envelope?.Body?.SetNewEVGreenCardV2Response?.@return;

                        if (res != null && res.responseCode.Equals("000"))
                        {
                            return new ServiceResponse<NewCardResponse>(result);
                        }
                        else
                        {
                            LogService.Error(new Exception(ResultData), this);
                            return new ServiceResponse<NewCardResponse>(result, false, string.IsNullOrEmpty(res.description) ? ErrorMessages.FRONTEND_ERROR_MESSAGE : res?.description ?? "");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<NewCardResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<EVTrackResponse> GetNotifications(string bpNumber, string sessionId, string userId, string clientIPAddress = "", string contractAccNo = "", string serviceCode = "", SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var request = GetRequest("ev/greencard/evnotification/trackrequest");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("businesspartnernumber", bpNumber);
                    dictionaryParam.Add("clientipaddress", clientIPAddress);
                    dictionaryParam.Add("contractaccountnumber", contractAccNo);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("servicecode", serviceCode);
                    dictionaryParam.Add("sessionid", sessionId);
                    dictionaryParam.Add("userid", userId);
                    dictionaryParam.Add("vendorid", GetVendorId(segment));

                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<EVTrackResponse>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<EVTrackResponse>(ResultData);
                        if (result != null && !string.IsNullOrWhiteSpace(result.Envelope.Body.GetEVTrackRequestResponse.@return.responsecode) && result.Envelope.Body.GetEVTrackRequestResponse.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<EVTrackResponse>(result);
                        }
                        else
                        {
                            return new ServiceResponse<EVTrackResponse>(result, false, "Error Response");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<EVTrackResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<Responses.EVGreenCard.Trd.EVTrackResponseDetail> GetNotificationDetail(string bpNumber, string sessionId, string userId, string notificationNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var request = GetRequest("ev/greencard/evnotification/details");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("businesspartnernumber", bpNumber);
                    dictionaryParam.Add("notificationnumber", notificationNumber);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("sessionid", sessionId);
                    dictionaryParam.Add("vendorid", GetVendorId(segment));
                    dictionaryParam.Add("userid", userId);

                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<Responses.EVGreenCard.Trd.EVTrackResponseDetail>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<Responses.EVGreenCard.Trd.EVTrackResponseDetail>(ResultData);
                        if (result != null && !string.IsNullOrWhiteSpace(result.Envelope.Body.GetEVNotificationDetailsResponse.@return.responsecode) && result.Envelope.Body.GetEVNotificationDetailsResponse.@return.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<Responses.EVGreenCard.Trd.EVTrackResponseDetail>(result);
                        }
                        else
                        {
                            return new ServiceResponse<Responses.EVGreenCard.Trd.EVTrackResponseDetail>(result, false, "Error Response");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<Responses.EVGreenCard.Trd.EVTrackResponseDetail>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<EVUserRTADetailsResponse> GetEVUserRTADetails(string bpCategory, string trafficFileNumber, string carRegistrationCountry, string carRegistrationRegion, string carIdNumber, string carIdType = "P", SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                /*
                 trafficFileNumber:000000050167673
                 carRegistratedRegion:DXB
                 cardIdNumber:W16734
                 carRegistratedCountry:AE
                 bpCategory:1
                 cardIdType:P
                 */
                using (HttpClient httpClient = new HttpClient())
                {
                    var request = GetRequest("ev/greencard/apply/v2");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("trafficFileNumber", trafficFileNumber);
                    dictionaryParam.Add("carRegistratedRegion", carRegistrationRegion);
                    dictionaryParam.Add("vendorId", GetVendorId(segment));
                    dictionaryParam.Add("cardIdNumber", carIdNumber);
                    dictionaryParam.Add("carRegistratedCountry", carRegistrationCountry);
                    dictionaryParam.Add("bpCategory", bpCategory);
                    dictionaryParam.Add("cardIdType", carIdType);

                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<EVUserRTADetailsResponse>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<EVUserRTADetailsResponse>(ResultData);
                        var resp = result?.Envelope?.Body?.SetNewEVGreenCardV2Response?.@return;

                        if (resp != null && resp.responseCode.Equals("000"))
                        {
                            return new ServiceResponse<EVUserRTADetailsResponse>(result);
                        }
                        else
                        {
                            return new ServiceResponse<EVUserRTADetailsResponse>(result, false, resp?.description ?? "");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<EVUserRTADetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }


        public ServiceResponse<EVParamResponse> GetEVParamCarDetails(string urlParam, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {

                using (HttpClient httpClient = new HttpClient())
                {
                    var request = GetRequest("ev/greencard/evparamcardetails");

                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    dictionaryParam.Add("appidentifier", segment.Identifier());
                    dictionaryParam.Add("appversion", AppVersion);
                    dictionaryParam.Add("lang", language.Code());
                    dictionaryParam.Add("mobileosversion", AppVersion);
                    dictionaryParam.Add("urlParam", urlParam);
                    dictionaryParam.Add("vendorId", GetVendorId(segment));


                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    request.Content = paramContent;

                    var response = httpClient.SendAsync(request).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<EVParamResponse>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<EVParamResponse>(ResultData);
                        var resp = result?.Envelope?.Body?.GetEVParamCarDetailsResponse?.@return;

                        if (resp != null && resp.responsecode.Equals("000"))
                        {
                            return new ServiceResponse<EVParamResponse>(result);
                        }
                        else
                        {
                            return new ServiceResponse<EVParamResponse>(result, false, resp?.description ?? "");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<EVParamResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion

        #region SetNewEVGreenCard V3 Methods

        public ServiceResponse<SetNewEVGreenCardV3Response> SetNewEVGreenCardV3(SetNewEVGreenCardV3Request request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {

            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {

                    var apiRequest = GetRequest("/ev/greencard/apply/v3");
                    if (request != null)
                    {
                        request.lang = language.Code();
                        request.appidentifier = segment.Identifier();
                        request.vendorId = GetVendorId(segment);
                        request.appversion = AppVersion;
                    }
                    Dictionary<string, string> dictionaryParam = new Dictionary<string, string>();
                    foreach (var prop in request.GetType().GetProperties())
                    {
                        if (prop.GetIndexParameters().Length == 0)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(prop.GetValue(request))))
                            {
                                dictionaryParam.Add(prop.Name, Convert.ToString(prop.GetValue(request)));
                            }
                        }
                    }

                    StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");

                    apiRequest.Content = paramContent;

                    var response = httpClient.SendAsync(apiRequest).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResponse<SetNewEVGreenCardV3Response>(null, false, "Response Error");
                    }
                    else
                    {
                        var ResultData = response.Content.ReadAsStringAsync().Result.ToString();
                        var result = JsonConvert.DeserializeObject<SetNewEVGreenCardV3Response>(ResultData);
                        if (result != null && !string.IsNullOrWhiteSpace(result.responseCode) && result.responseCode.Equals("000"))
                        {
                            return new ServiceResponse<SetNewEVGreenCardV3Response>(result);
                        }
                        else
                        {
                            return new ServiceResponse<SetNewEVGreenCardV3Response>(result, false, result.description ?? "Error Response");
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
                return new ServiceResponse<SetNewEVGreenCardV3Response>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
        #endregion
    }
}
