// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.SmartConsultant
{
    using DEWAXP.Foundation.DI;
    using DEWAXP.Foundation.Integration.Enums;
    using DEWAXP.Foundation.Integration.Helpers;
    using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
    using DEWAXP.Foundation.Integration.Requests;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.SmartConsultant;
    using DEWAXP.Foundation.Logger;
    using RestSharp;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;
    [Service(typeof(ISmartConsultantClient), Lifetime = Lifetime.Transient)]
    /// <summary>
    /// Defines the <see cref="SmartConsultantClient" />.
    /// </summary>
    public class SmartConsultantClient : BaseDewaGateway, ISmartConsultantClient
    {
        /// <summary>
        /// Gets or sets the DEWASMARTCONSULTANTURL.
        /// </summary>
        internal string DEWASMARTCONSULTANTURL { get; set; } = WebConfigurationManager.AppSettings["DEWASMARTCONSULTANT_URL"];

        /// <summary>
        /// The SmartConsultantSubmit.
        /// </summary>
        /// <param name="language">The language<see cref="SupportedLanguage"/>.</param>
        /// <returns>The <see cref="ServiceResponse{DisplayConsultantTrainings}"/>.</returns>
        public ServiceResponse<List<TrainingDetail>> DisplayPVCCTraining(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                Dictionary<string, string> querystringValue = new Dictionary<string, string>
                {
                    {SmartConsultantConstant.LANGUAGE, language.Code() },
                    {SmartConsultantConstant.VENDORID, GetVendorId(segment) }
                };
                IRestResponse  response = SmartConsultantSubmit(SmartConsultantConstant.DISPLAY_TRAINING, null, Method.GET, querystringValue);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DisplayConsultantTrainings _Response = CustomJsonConvertor.DeserializeObject<DisplayConsultantTrainings>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<List<TrainingDetail>>(_Response.TrainingDetails);
                    }
                    else
                    {
                        return new ServiceResponse<List<TrainingDetail>>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<List<TrainingDetail>>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<List<TrainingDetail>>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        #region BookingPVCCTraining

        public ServiceResponse<UpdateConsultantTrainingResponse> BookingPVCCTraining(UpdateConsultantTrainingRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                Dictionary<string, string> querystringValue = new Dictionary<string, string>
                {
                    { SmartConsultantConstant.LANGUAGE, language.Code() }
                };

                // booking request
                request.trainingdetails.appidentifier = segment.Identifier();
                request.trainingdetails.appver = AppVersion;
                request.trainingdetails.mobileosver = AppVersion;
                request.trainingdetails.lang = language.Code();
                request.trainingdetails.vendorid = GetVendorId(segment);

                IRestResponse  response = SmartConsultantSubmit(SmartConsultantConstant.UPDATE_TRAINING, request, Method.POST, querystringValue);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UpdateConsultantTrainingResponse _Response = CustomJsonConvertor.DeserializeObject<UpdateConsultantTrainingResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.Responsecode) && _Response.Responsecode.Equals("000"))
                    {
                        return new ServiceResponse<UpdateConsultantTrainingResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<UpdateConsultantTrainingResponse>(null, false, _Response.Description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<UpdateConsultantTrainingResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<UpdateConsultantTrainingResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        #endregion BookingPVCCTraining

        #region BookingPVCCTrainingAttach

        public ServiceResponse<UpdateConsultantTrainingAttachResponse> BookingPVCCTrainingAttach(UpdateConsultantTrainingAttachRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                Dictionary<string, string> querystringValue = new Dictionary<string, string>
                {
                    { SmartConsultantConstant.LANGUAGE, language.Code() }
                };

                // booking request
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.lang = language.Code();
                request.flag = "U";
                request.vendorid = GetVendorId(segment);
                var bookrequest = new
                {
                    trainingattachments = request
                };

                IRestResponse  response = SmartConsultantSubmit(SmartConsultantConstant.ATTACHMENT_TRAINING, bookrequest, Method.POST, querystringValue);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UpdateConsultantTrainingAttachResponse _Response = CustomJsonConvertor.DeserializeObject<UpdateConsultantTrainingAttachResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<UpdateConsultantTrainingAttachResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<UpdateConsultantTrainingAttachResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<UpdateConsultantTrainingAttachResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<UpdateConsultantTrainingAttachResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        #endregion BookingPVCCTrainingAttach

        #region TrainingBookingDetails

        public ServiceResponse<TrainingBookingDetailsResponse> TrainingBookingDetails(TrainingBookingDetailsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                Dictionary<string, string> querystringValue = new Dictionary<string, string>
                {
                    { SmartConsultantConstant.LANGUAGE, language.Code() }
                };
                request.vendorid = GetVendorId(segment);

                IRestResponse  response = SmartConsultantSubmit(SmartConsultantConstant.TRAINING_DETAILS, request, Method.POST, querystringValue);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TrainingBookingDetailsResponse _Response = CustomJsonConvertor.DeserializeObject<TrainingBookingDetailsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<TrainingBookingDetailsResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<TrainingBookingDetailsResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<TrainingBookingDetailsResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<TrainingBookingDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        #endregion TrainingBookingDetails

        #region PVCCCertificateDetails

        public ServiceResponse<PVCCCertificateDetailsResponse> PVCCCertificateDetails(PVCCCertificateDetailsRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                Dictionary<string, string> querystringValue = new Dictionary<string, string>
                {
                    { SmartConsultantConstant.LANGUAGE, language.Code() }
                };

                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);

                IRestResponse  response = SmartConsultantSubmit(SmartConsultantConstant.CERTIFICATE_DETAILS, request, Method.POST, querystringValue);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PVCCCertificateDetailsResponse _Response = CustomJsonConvertor.DeserializeObject<PVCCCertificateDetailsResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<PVCCCertificateDetailsResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<PVCCCertificateDetailsResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<PVCCCertificateDetailsResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<PVCCCertificateDetailsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        #endregion PVCCCertificateDetails

        #region PVCCEmiratesidVerify

        public ServiceResponse<PVCCEmiratesidVerifyResponse> PVCCEmiratesidVerify(PVCCEmiratesidVerifyRequest request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                Dictionary<string, string> querystringValue = new Dictionary<string, string>
                {
                    { SmartConsultantConstant.LANGUAGE, language.Code() }
                };

                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.mobileosversion = AppVersion;

                IRestResponse  response = SmartConsultantSubmit(SmartConsultantConstant.EMIRATESID_VERIFY, request, Method.POST, querystringValue);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PVCCEmiratesidVerifyResponse _Response = CustomJsonConvertor.DeserializeObject<PVCCEmiratesidVerifyResponse>(response.Content);
                    if (_Response != null && !string.IsNullOrWhiteSpace(_Response.responsecode) && _Response.responsecode.Equals("000"))
                    {
                        return new ServiceResponse<PVCCEmiratesidVerifyResponse>(_Response);
                    }
                    else
                    {
                        return new ServiceResponse<PVCCEmiratesidVerifyResponse>(null, false, _Response.description);
                    }
                }
                else
                {
                    LogService.Fatal(new System.Exception($"response value: ''Status : {response.StatusCode}' Content:{response.Content}'' Description: {response.StatusDescription}'"), this);
                    return new ServiceResponse<PVCCEmiratesidVerifyResponse>(null, false, $"response value: '{response}'");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<PVCCEmiratesidVerifyResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        #endregion PVCCEmiratesidVerify

        #region Submit, Create header

        /// <summary>
        /// The SmartConsultantSubmit.
        /// </summary>
        /// <param name="methodname">The methodname<see cref="string"/>.</param>
        /// <param name="requestbody">The requestbody<see cref="object"/>.</param>
        /// <param name="method">The method<see cref="Method"/>.</param>
        /// <param name="Querystring_Array">The Querystring_Array<see cref="Dictionary{string, string}"/>.</param>
        /// <returns>The <see cref="IRestResponse"/>.</returns>
        public IRestResponse  SmartConsultantSubmit(string methodname, object requestbody, Method method = Method.POST, Dictionary<string, string> Querystring_Array = null)
        {
            RestRequest request = null;
            RestClient client = CreateClient();
            request = new RestRequest(methodname, method);
            request = CreateHeader(request);
            if (Querystring_Array != null)
            {
                request = CreateQueryString(request, Querystring_Array);
            }
            if (requestbody != null)
            {
                request.AddBody(requestbody);
            }
            IRestResponse  response = client.Execute(request);
            return response;
        }

        /// <summary>
        /// The CreateClient.
        /// </summary>
        /// <returns>The <see cref="RestClient"/>.</returns>
        private RestClient CreateClient()
        {
            return new RestClient(DEWASMARTCONSULTANTURL);
        }

        /// <summary>
        /// The CreateHeader.
        /// </summary>
        /// <param name="request">The request<see cref="RestRequest"/>.</param>
        /// <returns>The <see cref="RestRequest"/>.</returns>
        private RestRequest CreateHeader(RestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + OAuthToken.GetAccessToken());
            request.RequestFormat = DataFormat.Json;
            return request;
        }

        /// <summary>
        /// The CreateQueryString.
        /// </summary>
        /// <param name="request">The request<see cref="RestRequest"/>.</param>
        /// <param name="Querystring_Array">The Querystring_Array<see cref="Dictionary{string, string}"/>.</param>
        /// <returns>The <see cref="RestRequest"/>.</returns>
        private RestRequest CreateQueryString(RestRequest request, Dictionary<string, string> Querystring_Array)
        {
            Querystring_Array.ToList().ForEach
            (
                pair =>
                {
                    request.AddQueryParameter(pair.Key, pair.Value);
                }
            );
            return request;
        }

        #endregion Submit, Create header
    }
}