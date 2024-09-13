using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Responses;
using Exception = System.Exception;
using DEWAXP.Foundation.Integration.HappinessSvc;
using DEWAXP.Foundation.Integration.Responses.HappinessSvc;
using System.Collections.Generic;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.HappinessSvc
{
    [Service(typeof(IHappinessServiceClient), Lifetime = Lifetime.Transient)]
    public class HappinessSoapClient : BaseDewaGateway, IHappinessServiceClient
    {
        #region Methods

        public ServiceResponse<HappinessSurveyResponse> GetHappinessSurvey(string surveyType)
        {
            using (var client = CreateProxy())
            {
                var request = new GetCustHappinessSurveyQuestions
                {
                    survey = surveyType
                };

                try
                {
                    var response = client.GetCustHappinessSurveyQuestions(request);
                    var typedResponse = response.@return.DeserializeAs<HappinessSurveyResponse>();

                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<HappinessSurveyResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<HappinessSurveyResponse>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<HappinessSurveyResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }
        public ServiceResponse SubmitHappinessSurvey(AnswersResponse[] arrAnswers, string referenceNo, string surveyType)
        {
            surveyAnswer[] arrSurveyAnswers = new surveyAnswer[arrAnswers.LongLength];

            for (int count = 0; count < arrAnswers.Length; count++)
            {
                arrSurveyAnswers[count] = new surveyAnswer();
                arrSurveyAnswers[count].answerNumber = arrAnswers[count].AnswerNumber;
                arrSurveyAnswers[count].arabic = arrAnswers[count].ArabicText;
                arrSurveyAnswers[count].english = arrAnswers[count].EnglishText;
                arrSurveyAnswers[count].questionNumber = arrAnswers[count].QuestionNumber;
                arrSurveyAnswers[count].survey = arrAnswers[count].SurveyName;
                arrSurveyAnswers[count].type = arrAnswers[count].ControlType;
            }
            using (var client = CreateProxy())
            {
                var request = new SetCustHappinessSurveyAnswers
                {
                    customerSurveyAnswers = arrSurveyAnswers,
                    referenceNumber = referenceNo,
                    survey = surveyType
                };

                try
                {
                    var response = client.SetCustHappinessSurveyAnswers(request);

                    var typedResponse = response.@return.DeserializeAs<HappinessSurveySubmitResponse>();

                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse(false, typedResponse.Description);
                    }
                    return new ServiceResponse(true, typedResponse.Description);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #endregion

        #region Proxy configuration methods

        private ConfigClient CreateProxy()
        {
            var client = new ConfigClient(CreateBinding(), GetEndpointAddress("EmployeeHappinessPort"));
            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());

            client.ClientCredentials.UserName.UserName = EHMUserName;
            client.ClientCredentials.UserName.Password = EHMPassword;

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
        //        if (clientSection.Endpoints[i].Name == "EmployeeHappinessPort")
        //            address = clientSection.Endpoints[i].Address.ToString();
        //    }
        //    return new EndpointAddress(address);
        //}

        #endregion
    }
}
