using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Impl;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Integration.Responses;
using RestSharp;
using Sitecore.Diagnostics;
//using RestSharp;
//using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using _ApiBaseConfig = DEWAXP.Foundation.Integration.APIHandler.Config.ApiBaseConfig;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(IEVCardApiHandler), Lifetime = Lifetime.Transient)]
    public class EVCardApiHandler : BaseDewaGateway, IEVCardApiHandler
    {
        public ServiceResponse<EvPlateDetailsResponse> GetEvPlateDetails(ApiBaseRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                var client = new RestClient(_ApiBaseConfig.RESTAPI_SMART_CUSTOMER);
                var apiRequest = CreatRestRequestWithToken("/ev/greencard/evplatedetails", Method.GET);

                //Map request to query string by refelction
                if (request != null)
                {
                    request.lang = language.Code();
                    request.appidentifier = segment.Identifier();
                    request.vendorId = GetVendorId(segment);
                    request.appversion = AppVersion;

                    foreach (var prop in request.GetType().GetProperties())
                    {
                        if (prop.GetIndexParameters().Length == 0)
                        {
                            apiRequest.AddQueryParameter(prop.PropertyType.Name, Convert.ToString(prop.GetValue(request) ?? ""));
                        }
                    }
                }
                //Execute Api
                IRestResponse<EvPlateDetailsResponse> responseData = client.Execute<EvPlateDetailsResponse>(apiRequest);

                if (responseData != null && responseData.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (responseData.Data != null && responseData.Data.responseCode == "000")
                    {
                        return new ServiceResponse<EvPlateDetailsResponse>(responseData.Data);

                    }
                    else
                    {
                        return new ServiceResponse<EvPlateDetailsResponse>(responseData.Data, false, responseData.Data.description);
                    }
                }
                else
                {
                    return new ServiceResponse<EvPlateDetailsResponse>(null, false, "Response Error");
                }

            }
            catch (Exception ex)
            {
                Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
                return new ServiceResponse<EvPlateDetailsResponse>(null, false, ex.Message);
            }
        }

        //public ServiceResponse<SetNewEVGreenCardV3Response> SetNewEVGreenCardV3(SetNewEVGreenCardV3Request request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        //{
        //    try
        //    {
        //        var client = new RestClient(_ApiBaseConfig.APIURL_SMARTCUSTOMER);
        //        var apiRequest = CreatRestRequestWithToken("/ev/greencard/apply/v3", Method.POST);
        //        apiRequest.AddHeader("content-type", "application/x-www-form-urlencoded");


        //        if (request != null)
        //        {
        //            request.lang = language.Code();
        //            request.appidentifier = segment.Identifier();
        //            request.vendorId = GetVendorId(segment);
        //            request.appversion = AppVersion;
        //        }
        //        //adding Param in body
        //        //apiRequest.AddBody(request);

        //        //Execute Api
        //        List<ApiFormData> formData = new List<ApiFormData>();
        //        foreach (var prop in request.GetType().GetProperties())
        //        {
        //            if (prop.GetIndexParameters().Length == 0)
        //            {
        //                if(!string.IsNullOrEmpty( Convert.ToString(prop.GetValue(request))))
        //                {
        //                    formData.Add(new ApiFormData()
        //                    {
        //                        PropertyName = prop.Name,
        //                        PropertyValue = Convert.ToString(prop.GetValue(request))
        //                    });
        //                }
        //            }
        //        }
        //        string val = "";
        //        if (formData != null)
        //        {
        //            val = string.Join("&", formData.Select(x => Convert.ToString($"{x.PropertyName }={x.PropertyValue}")));
        //        }
        //        //StringContent paramContent = new StringContent(OAuthToken.ConvertParameterString(dictionaryParam), Encoding.UTF8, "application/x-www-form-urlencoded");
        //        //apiRequest.AddParameter("application/x-www-form-urlencoded", val, ParameterType.RequestBody);
        //        //apiRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //        //apiRequest.AddParameter("undefined",val , ParameterType.RequestBody);

        //        apiRequest.AddParameter("undefined", "appidentifier=d&vendorId=DWEBQMID&lang=EN&appversion=1.1&bpCategory=2&carIdNumber=52245&carPlateCode=G&carRegistratedCountry=AE&carRegistratedRegion=DXB&cardIdType=P&trafficFileNumber=50000418&carCategory=2", ParameterType.RequestBody);
        //        RestResponse<SetNewEVGreenCardV3Response> responseData = client.Execute<SetNewEVGreenCardV3Response>(apiRequest);

        //        if (responseData != null && responseData.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            if (responseData.Data != null && responseData.Data.responseCode == "000")
        //            {
        //                return new ServiceResponse<SetNewEVGreenCardV3Response>(responseData.Data);

        //            }
        //            else
        //            {
        //                return new ServiceResponse<SetNewEVGreenCardV3Response>(responseData.Data, false, responseData.Data.description);
        //            }
        //        }
        //        else
        //        {
        //            return new ServiceResponse<SetNewEVGreenCardV3Response>(null, false, "Response Error");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Info(ex.Message + "---" + ex.InnerException.ToString(), this);
        //        return new ServiceResponse<SetNewEVGreenCardV3Response>(null, false, ex.Message);
        //    }
        //}

        //SetNewEVGreenCardV3

        #region [Utility]

        private RestRequest CreatRestRequestWithToken(string url, Method method)
        {
            var apiRequest = new RestRequest(url, method);
            apiRequest.AddHeader("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            return apiRequest;
        }

        #endregion
    }
}
