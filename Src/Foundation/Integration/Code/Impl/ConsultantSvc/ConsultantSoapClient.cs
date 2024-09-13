using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.ConsultantSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.SmartConsultantSvc;
using System.ServiceModel.Description;
using DEWAXP.Foundation.Integration.Helpers;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.ConsultantSvc
{
    [Service(typeof(IConsultantServiceClient), Lifetime = Lifetime.Transient)]
    public class ConsultantSoapClient : BaseDewaGateway, IConsultantServiceClient
    {

        #region Proxy configuration methods

        private SmartConsultantClient CreateProxy()
        {
            var client = new SmartConsultantClient(CreateBinding(), GetEndpointAddress("SmartConsultantPort"));
            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
            client.ClientCredentials.UserName.UserName = BbUsername;
            client.ClientCredentials.UserName.Password = BbPassword;

            return client;
        }

        private CustomBinding CreateBinding()
        {
            var binding = new CustomBinding()
            {
                ReceiveTimeout = TimeSpan.FromMinutes(2),
                SendTimeout = TimeSpan.FromMinutes(2)
            };

            var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = true;
            security.LocalClientSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.LocalServiceSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
            security.EnableUnsecuredResponse = true;
            security.AllowInsecureTransport = true;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap11;

            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 20000000; // 20 megs

            binding.Elements.Add(security);
            binding.Elements.Add(new CustomTextMessageBindingElement());
            binding.Elements.Add(transport);

            return binding;
        }

        //private EndpointAddress GetEndpointAddress()
        //{
        //    var clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
        //    string address = string.Empty;
        //    for (int i = 0; i < clientSection.Endpoints.Count; i++)
        //    {
        //        if (clientSection.Endpoints[i].Name == "SmartConsultantPort")
        //            address = clientSection.Endpoints[i].Address.ToString();
        //    }
        //    return new EndpointAddress(address);
        //}

        #endregion



        public ServiceResponse<ConnectionCalculatorResponse> GetConnectionCalCharges(string existingLoad, string proposeLoad, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetConnectionServChargesCalc()
                {
                    load = proposeLoad,
                    existingload = existingLoad,
                    vendorid = GetVendorId(segment),
                    appidentifier = segment.Identifier(),
                    appversion = AppVersion,
                    lang = language.Code()
                };

                try
                {
                    var response = client.GetConnectionServChargesCalc(request);

                    var typedResponse = response.@return.DeserializeAs<ConnectionCalculatorResponse>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<ConnectionCalculatorResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<ConnectionCalculatorResponse>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ConnectionCalculatorResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetSurveyQuestionsResponse> GetSurveyQuestions(GetSurveyQuestions request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.lang = language.Code();

                try
                {
                    var response = client.GetSurveyQuestions(request);


                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<GetSurveyQuestionsResponse>(response, true, response.@return.description);
                    }
                    return new ServiceResponse<GetSurveyQuestionsResponse>(response, false, response.@return.description);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<GetSurveyQuestionsResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<PostSurveyAnswersResponse> PostSurveyAnswers(PostSurveyAnswers request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.lang = language.Code();

                try
                {
                    var response = client.PostSurveyAnswers(request);
                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<PostSurveyAnswersResponse>(response, true, response.@return.description);
                    }
                    return new ServiceResponse<PostSurveyAnswersResponse>(response, false, response.@return.description);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<PostSurveyAnswersResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<ValidateSurveyResponse> ValidateSurvey(ValidateSurvey request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                request.mobileosversion = AppVersion;
                request.vendorid = GetVendorId(segment);
                request.appidentifier = segment.Identifier();
                request.appversion = AppVersion;
                request.lang = language.Code();

                try
                {
                    var response = client.ValidateSurvey(request);


                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        return new ServiceResponse<ValidateSurveyResponse>(response, true, response.@return.description);
                    }
                    return new ServiceResponse<ValidateSurveyResponse>(response, false, response.@return.description);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ValidateSurveyResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<GetLoginSessionConsultantResponse> GetLoginSessionOwnerConsultant(GetLoginSessionConsultant request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.appidentifier = segment.Identifier();
                    request.merchantid = GetMerchantId(segment);
                    request.merchantpassword = GetMerchantPassword(segment);
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;

                    var response = client.GetLoginSessionConsultant(request);

                    if (response != null && response.@return != null && response.@return == "000")
                    {
                        var typedresponse = new ServiceResponse<GetLoginSessionConsultantResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return == "105")
                    {
                        var typedresponse = new ServiceResponse<GetLoginSessionConsultantResponse>(response, true, response.@return);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<GetLoginSessionConsultantResponse>(response, false, response.@return);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<GetLoginSessionConsultantResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetLoginSessionConsultantResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<TrackOwnerResponse> GetTrackOwner(TrackOwner request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.TrackOwner(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "399")
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<TrackOwnerResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<TrackOwnerResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<TrackOwnerOrderResponse> GetTrackOwnerOrder(TrackOwnerOrder request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.TrackOwnerOrder(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerOrderResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "399")
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerOrderResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerOrderResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<TrackOwnerOrderResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<TrackOwnerOrderResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        public ServiceResponse<TrackOwnerStatusResponse> GetTrackOwnerStatus(TrackOwnerStatus request, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            try
            {
                using (var client = CreateProxy())
                {
                    request.appidentifier = segment.Identifier();
                    request.lang = language.Code();
                    request.appversion = AppVersion;
                    request.mobileosversion = AppVersion;
                    request.vendorid = GetVendorId(segment);

                    var response = client.TrackOwnerStatus(request);

                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerStatusResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "399")
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerStatusResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<TrackOwnerStatusResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<TrackOwnerStatusResponse>(null, false, "timeout error message");
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<TrackOwnerStatusResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }
    }
}
