using System;
using RestSharp;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using ResponseModels = DEWAXP.Foundation.Integration.APIHandler.Models.Response.SelfEnergyAssessmentSurvey;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Responses;
using RequestModels = DEWAXP.Foundation.Integration.APIHandler.Models.Request.SelfEnergyAssessmentSurvey;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.APIHandler.Config;
using System.Collections.Generic;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.APIHandler.Impl
{
    [Service(typeof(ISelfEnergyAssessmentSurveyClient), Lifetime = Lifetime.Transient)]
    public class SelfEnergyAssessmentSurveyClient : BaseApiDewaGateway, ISelfEnergyAssessmentSurveyClient
    {
        private const string SUCCESS_RESPONSE_CODE = "000";

        public ServiceResponse<string> DownloadSurveyReport(string userSessionId, string userId, string contractAccount, string version, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            return DownloadOrEmailSurvey("DOWNLOAD", version, userSessionId, userId, contractAccount, segment, language);
        }

        public ServiceResponse<string> EmailSurveyReport(string userSessionId, string userId, string contractAccount, string version, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            return DownloadOrEmailSurvey("EMAIL", version, userSessionId, userId, contractAccount, segment, language);
        }

        public ServiceResponse<ResponseModels.QuestionsAndAnswersResponse> GetQuestionsAndAnswers(string userSessionId, string userId, string contractAccount, RequestSegment segment, SupportedLanguage language)
        {
            try
            {
                RequestModels.BaseRequest request = new RequestModels.BaseRequest()
                {
                    surveyinputs = new RequestModels.SurveyinputBase()
                    {
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,
                        sessionid = userSessionId,
                        userid = userId,
                        vendorid = GetVendorId(segment),
                        contractaccount = contractAccount
                    }
                };


#if DEBUG
                LogService.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(request));
#endif
                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "selfenergyquestions", request, Method.POST, null);
#if DEBUG
                LogService.Debug(response.Content);
#endif
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ResponseModels.QuestionsAndAnswersResponse model = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModels.QuestionsAndAnswersResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals(SUCCESS_RESPONSE_CODE))
                    {
                        return new ServiceResponse<ResponseModels.QuestionsAndAnswersResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(LogMessageTemplate.BuildException(model.Responsecode, response.Content, response.StatusDescription), this);
                        return new ServiceResponse<ResponseModels.QuestionsAndAnswersResponse>(null, false, model.Description.ToString());
                    }
                }
                else
                {
                    LogService.Fatal(LogMessageTemplate.BuildException(response.StatusCode.ToString(), response.Content, response.StatusDescription), this);
                    return new ServiceResponse<ResponseModels.QuestionsAndAnswersResponse>(null, false, response.StatusDescription.ToString());
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ResponseModels.QuestionsAndAnswersResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ResponseModels.SavedAnsersResponse> GetSavedAnswers(string userSessionId, string userId, string contractAccount, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                RequestModels.BaseRequest request = new RequestModels.BaseRequest()
                {
                    surveyinputs = new RequestModels.SurveyinputBase()
                    {
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,
                        sessionid = userSessionId,
                        userid = userId,
                        vendorid = GetVendorId(segment),
                        contractaccount = contractAccount
                    }
                };

                global::Sitecore.Diagnostics.Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(request), this);

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "selfenergyresult", request, Method.POST, null);

                //LogService.Debug(response.Content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ResponseModels.SavedAnsersResponse model = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModels.SavedAnsersResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals(SUCCESS_RESPONSE_CODE))
                    {
                        return new ServiceResponse<ResponseModels.SavedAnsersResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(LogMessageTemplate.BuildException(model.Responsecode, response.Content, response.StatusDescription), this);
                        return new ServiceResponse<ResponseModels.SavedAnsersResponse>(null, false, model.Description.ToString());
                    }
                }
                else
                {
                    LogService.Fatal(LogMessageTemplate.BuildException(response.StatusCode.ToString(), response.Content, response.StatusDescription), this);
                    return new ServiceResponse<ResponseModels.SavedAnsersResponse>(null, false, response.StatusDescription.ToString());
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ResponseModels.SavedAnsersResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ResponseModels.SubmittedSurveysResponse> GetSubmittedSurveys(string userSessionId, string userId, string contractAccount, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                RequestModels.BaseRequest request = new RequestModels.BaseRequest()
                {
                    surveyinputs = new RequestModels.SurveyinputBase()
                    {
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,
                        sessionid = userSessionId,
                        userid = userId,
                        vendorid = GetVendorId(segment),
                        contractaccount = contractAccount
                    },
                };

                //LogService.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(request));

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "selfenergyversiondate", request, Method.POST, null);

                //LogService.Debug(response.Content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ResponseModels.SubmittedSurveysResponse model = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModels.SubmittedSurveysResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals(SUCCESS_RESPONSE_CODE))
                    {
                        return new ServiceResponse<ResponseModels.SubmittedSurveysResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(LogMessageTemplate.BuildException(model.Responsecode, response.Content, response.StatusDescription), this);
                        return new ServiceResponse<ResponseModels.SubmittedSurveysResponse>(null, false, model.Description.ToString());
                    }
                }
                else
                {
                    LogService.Fatal(LogMessageTemplate.BuildException(response.StatusCode.ToString(), response.Content, response.StatusDescription), this);
                    return new ServiceResponse<ResponseModels.SubmittedSurveysResponse>(null, false, response.StatusDescription.ToString());
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return new ServiceResponse<ResponseModels.SubmittedSurveysResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ResponseModels.SaveSurveyResponse> SaveSurveyAnswers(string userSessionId, string userId, string contractAccount, List<RequestModels.Answerlist> answers, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                RequestModels.SubmitRequest request = new RequestModels.SubmitRequest()
                {
                    surveyinputs = new RequestModels.SurveyinputSubmit()
                    {
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,
                        sessionid = userSessionId,
                        userid = userId,
                        vendorid = GetVendorId(segment),
                        contractaccount = contractAccount,
                        action = "SAVE"
                    },
                    answerlist = answers
                };

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "selfenergyanswers", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ResponseModels.SaveSurveyResponse model = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModels.SaveSurveyResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals(SUCCESS_RESPONSE_CODE))
                    {
                        return new ServiceResponse<ResponseModels.SaveSurveyResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(LogMessageTemplate.BuildException(model.Responsecode, response.Content, response.StatusDescription), this);
                        return new ServiceResponse<ResponseModels.SaveSurveyResponse>(null, false, model.Description.ToString());
                    }
                }
                else
                {
                    LogService.Fatal(LogMessageTemplate.BuildException(response.StatusCode.ToString(), response.Content, response.StatusDescription), this);
                    return new ServiceResponse<ResponseModels.SaveSurveyResponse>(null, false, response.StatusDescription.ToString());
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ResponseModels.SaveSurveyResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        public ServiceResponse<ResponseModels.SubmitSurveyResponse> SubmitSurveyAnswers(string userSessionId, string userId, string contractAccount, List<RequestModels.Answerlist> answers, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                RequestModels.SubmitRequest request = new RequestModels.SubmitRequest()
                {
                    surveyinputs = new RequestModels.SurveyinputSubmit()
                    {
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,
                        sessionid = userSessionId,
                        userid = userId,
                        vendorid = GetVendorId(segment),
                        contractaccount = contractAccount,
                        action = "SUBMIT"
                    },
                    answerlist = answers
                };

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "selfenergyanswers", request, Method.POST, null);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ResponseModels.SubmitSurveyResponse model = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModels.SubmitSurveyResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals(SUCCESS_RESPONSE_CODE))
                    {
                        return new ServiceResponse<ResponseModels.SubmitSurveyResponse>(model);
                    }
                    else
                    {
                        LogService.Fatal(LogMessageTemplate.BuildException(model.Responsecode, response.Content, response.StatusDescription), this);
                        return new ServiceResponse<ResponseModels.SubmitSurveyResponse>(null, false, model.Description.ToString());
                    }
                }
                else
                {
                    LogService.Fatal(LogMessageTemplate.BuildException(response.StatusCode.ToString(), response.Content, response.StatusDescription), this);
                    return new ServiceResponse<ResponseModels.SubmitSurveyResponse>(null, false, response.StatusDescription.ToString());
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new ServiceResponse<ResponseModels.SubmitSurveyResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }

        private ServiceResponse<string> DownloadOrEmailSurvey(string action, string version, string userSessionId, string userId, string contractAccount, RequestSegment segment = RequestSegment.Desktop, SupportedLanguage language = SupportedLanguage.English)
        {
            try
            {
                RequestModels.SubmitSurveyRequest request = new RequestModels.SubmitSurveyRequest()
                {
                    surveyinputs = new RequestModels.DownloadSurveyInput()
                    {
                        appidentifier = segment.Identifier(),
                        appversion = AppVersion,
                        lang = language.Code(),
                        mobileosversion = AppVersion,
                        sessionid = userSessionId,
                        userid = userId,
                        vendorid = GetVendorId(segment),
                        contractaccount = contractAccount,
                        action = action,
                        version = version
                    },
                };                

                IRestResponse response = DewaApiExecute(ApiBaseConfig.SmartCustomerV3_ApiUrl, "selfenergyattachments", request, Method.POST, null);                

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ResponseModels.DownloadSurveyResponse model = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModels.DownloadSurveyResponse>(response.Content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Responsecode) && model.Responsecode.Equals(SUCCESS_RESPONSE_CODE))
                    {                        
                        return new ServiceResponse<string>(model.Binarydata);
                    }
                    else
                    {
                        LogService.Fatal(LogMessageTemplate.BuildException(model.Responsecode, response.Content, response.StatusDescription), this);
                        return new ServiceResponse<string>(null, false, model.Description.ToString());
                    }
                }
                else
                {
                    LogService.Fatal(LogMessageTemplate.BuildException(response.StatusCode.ToString(), response.Content, response.StatusDescription), this);
                    return new ServiceResponse<string>(null, false, response.StatusDescription.ToString());
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
        }
    }
}
