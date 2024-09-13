using System;
using DEWAXP.Foundation.Integration.CustomerSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Requests;
using DEWAXP.Foundation.Integration.Responses;
using System.ServiceModel.Description;
using System.Xml.Linq;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.Helpers;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.CustomerSvc
{
    [Service(typeof(IEServicesClient), Lifetime = Lifetime.Transient)]
    public class CustomerSoapClient : BaseDewaGateway, IEServicesClient
    {
        public ServiceResponse<string> LinkBusinessPartnerToMyId(LinkBusinessPartnerToMyId @params, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var proxy = CreateProxy())
            {
                var request = new addBP
                {
                    merchantid = GetMerchantId(segment),
                    merchantpassword = GetMerchantPassword(segment),
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    sessionid = string.Empty,
                    userid = @params.MyIdUsername,
                    updFlag = "I",
                    bp = @params.BusinessPartnerNumber,
                    eMail = @params.EmailAddress,
                    mobile = @params.MobileNumber,
                    poBox = @params.PoBox,
                    addtoMYID = @params.EmiratesIdentifier,
                    addtoUserID = string.Empty,
                    appidentifier = segment.Identifier(),
                    appver = AppVersion
                };
                try
                {
                    var response = proxy.addBP(request);
                    var typedResponse = response.@return.DeserializeAs<MyIdRegistrationResponse>();
                    if (typedResponse != null && typedResponse.ResponseCode.Equals(0))
                    {
                        return new ServiceResponse<string>(typedResponse.SessionToken);
                    }
                    return new ServiceResponse<string>(null, false, typedResponse.Description);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<ClearanceCertificateDetails> GetClearanceCertificate(string userId, string sessionId, string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var proxy = CreateProxy())
            {
                var request = new getClearanceCertificateDisplay
                {
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    tokenid = string.Empty,
                    userid = userId,
                    contractaccount = contractAccountNumber,
                    appidentifier = segment.Identifier(),
                    appver = AppVersion
                };
                try
                {
                    var response = proxy.getClearanceCertificateDisplay(request);
                    var typedResponse = response.@return.DeserializeAs<ClearanceCertificateDetails>();
                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<ClearanceCertificateDetails>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<ClearanceCertificateDetails>(typedResponse);
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ClearanceCertificateDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #region Factory methods

        private Config1Client CreateProxy()
        {
            var client = new Config1Client(CreateBinding(), GetEndpointAddress("SmartGovtPort"));

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

        #endregion
    }
}
