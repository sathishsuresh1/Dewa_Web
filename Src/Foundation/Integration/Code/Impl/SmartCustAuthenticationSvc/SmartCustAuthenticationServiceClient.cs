using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Responses;
using System.ServiceModel.Dispatcher;
using Sitecore.Diagnostics;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.SmartCustAuthenticationSvc;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.SmartCustAuthenticationSvc
{
    [Service(typeof(ISmartCustAuthenticationServiceClient), Lifetime = Lifetime.Transient)]
    public class SmartCustAuthenticationServiceClient : BaseDewaGateway, ISmartCustAuthenticationServiceClient
    {
        #region Methods

        public ServiceResponse<SmartCustAuthenticationResponse> GetLoginSessionCustomerAuthentication(string userId, string sessionId, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new GetLoginSessionCustomerAuthentication
                {
                    userid = userId,
                    issuersessionid = sessionId,
                    issuervendorid = GetVendorId(segment),
                    providermerchantid = WebConfigurationManager.AppSettings["SmartCustAuthen_MerchantId"],
                    providermerchantpassword = WebConfigurationManager.AppSettings["SmartCustAuthen_MerchantPassword"],
                };
                Log.Info("smart auth request", request);
                try
                {
                    var response = client.GetLoginSessionCustomerAuthentication(request);
                    Log.Info("smart auth res", response);

                    var typedResponse = response.@return.DeserializeAs<SmartCustAuthenticationResponse>();
                    Log.Info("smart auth res2", typedResponse);

                    if (typedResponse.ResponseCode != 0)
                    {
                        return new ServiceResponse<SmartCustAuthenticationResponse>(null, false, typedResponse.Description);
                    }
                    return new ServiceResponse<SmartCustAuthenticationResponse>(typedResponse);
                }
                catch (System.Exception ex)
                {
                    Log.Info(ex.Message + "---smartauth" , this);
                    return new ServiceResponse<SmartCustAuthenticationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #endregion

        #region Proxy configuration methods

        private SmartCustAuthenticationClient CreateProxy()
        {
            Log.Info("smart auth create proxy",true);
            var client = new SmartCustAuthenticationClient(CreateBinding(), GetEndpointAddress("SmartCustAuthenticationPort"));
            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());
            //client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["Jobseeker_apiKey"]));
            client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
            client.ClientCredentials.UserName.UserName = WebConfigurationManager.AppSettings["SmartCustAuthen_UserName"];
            client.ClientCredentials.UserName.Password = WebConfigurationManager.AppSettings["SmartCustAuthen_Password"];

            return client;
        }

        private CustomBinding CreateBinding()
        {
            Log.Info("smart auth binding1", true);

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
            //var transport = null;
            if (!string.IsNullOrWhiteSpace(Rammas_SSL_SETTING) && Rammas_SSL_SETTING.Equals("1"))
            {
                var transport = new HttpsTransportBindingElement();
                transport.MaxReceivedMessageSize = 20000000; // 20 megs

                binding.Elements.Add(security);
                binding.Elements.Add(new CustomTextMessageBindingElement());
                binding.Elements.Add(transport);
            }
            else
            {
                var transport = new HttpTransportBindingElement();
                transport.MaxReceivedMessageSize = 20000000; // 20 megs
                binding.Elements.Add(security);
                binding.Elements.Add(new CustomTextMessageBindingElement());
                binding.Elements.Add(transport);
            }
            Log.Info("smart auth binding2", true);

            return binding;
        }

        //private EndpointAddress GetEndpointAddress()
        //{
        //    var clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
        //    string address = string.Empty;
        //    for (int i = 0; i < clientSection.Endpoints.Count; i++)
        //    {
        //        if (clientSection.Endpoints[i].Name == "SmartCustAuthenticationPort")
        //            address = clientSection.Endpoints[i].Address.ToString();
        //    }
        //    return new EndpointAddress(address);
        //}

        #endregion
    }
    public class CustomAuthenticationBehavior : IEndpointBehavior
    {
        /// <summary>
        ///     The sap API key.
        /// </summary>
        private string sapApiKey;
        private string token;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthenticationBehavior"/> class. 
        /// Initializes a new instance of the
        ///     <see cref="CustomAuthenticationBehavior"/> class.
        /// </summary>
        /// <param name="sapKey">
        /// The authentication token.
        /// </param>
        public CustomAuthenticationBehavior(string sapKey, string token)
        {
            this.sapApiKey = sapKey;
            this.token = token;
        }

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// The apply client behavior.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="clientRuntime">
        /// The client runtime.
        /// </param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (clientRuntime != null)
            {
                clientRuntime.ClientMessageInspectors.Add(new CustomMessageInspector(this.sapApiKey,this.token));
            }
        }

        /// <summary>
        /// The apply dispatch behavior.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        /// <param name="endpointDispatcher">
        /// The <paramref name="endpoint"/> dispatcher.
        /// </param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint.
        /// </param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class CustomMessageInspector : IClientMessageInspector
    {
        /// <summary>
        ///     The sap API key.
        /// </summary>
        private readonly string sapApiKey;
        private readonly string token;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMessageInspector"/> class. 
        /// Initializes a new instance of the
        ///     <see cref="CustomMessageInspector"/> class.
        /// </summary>
        /// <param name="sapKey">
        /// The authentication token.
        /// </param>
        public CustomMessageInspector(string sapKey, string token)
        {
            this.sapApiKey = sapKey;
            this.token = token;
        }

        /// <summary>
        /// The after receive reply.
        /// </summary>
        /// <param name="reply">
        /// The reply.
        /// </param>
        /// <param name="correlationState">
        /// The correlation state.
        /// </param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <summary>
        /// The before send request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// The <see cref="System.Object"/> .
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var reqMsgProperty = new HttpRequestMessageProperty();
            reqMsgProperty.Headers.Add("apikey", this.sapApiKey);
            reqMsgProperty.Headers.Add("Authorization", this.token);
            if (request != null)
            {
                request.Properties[HttpRequestMessageProperty.Name] = reqMsgProperty;
            }

            return null;
        }
    }

}
