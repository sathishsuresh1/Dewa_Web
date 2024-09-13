using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartSurvey;
using DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartSurvey;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    public class SmartSurveyClient : BaseApiDewaGateway, ISmartSurveyClient
    {
        /// <summary>
        /// Gets the BaseApiUrl.
        /// </summary>
        private string BaseApiUrl => $"{ApiBaseConfig.SmartSurvey_ApiUrl}";

        public ServiceResponse<Surveydataoutput> SurveyData(SurveyDataInput request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.surveyinput.lang = language.Code();
                request.surveyinput.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, "surveydata", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SurveyDataOutputModel _Response = CustomJsonConvertor.DeserializeObject<SurveyDataOutputModel>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000") && _Response.Surveydataoutput != null)
                    {
                        return new ServiceResponse<Surveydataoutput>(_Response.Surveydataoutput);
                    }
                    else
                    {
                        return new ServiceResponse<Surveydataoutput>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<Surveydataoutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<Surveydataoutput>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }

        public ServiceResponse<SurveyOTPOutputModel> SurveyOtp(SurveyOTPInput request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                request.surveyotpinput.lang = language.Code();
                request.surveyotpinput.vendorid = GetVendorId(segment);
                IRestResponse response = DewaApiExecute(BaseApiUrl, "surveyotp", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SurveyOTPOutputModel _Response = CustomJsonConvertor.DeserializeObject<SurveyOTPOutputModel>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<SurveyOTPOutputModel>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<SurveyOTPOutputModel>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<SurveyOTPOutputModel>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<SurveyOTPOutputModel>(null, false, ErrorMessages.PLEASETRYAGAIN_ERROR_MESSAGE);
        }
    }
}
