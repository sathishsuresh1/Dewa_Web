using DEWAXP.Foundation.Integration.Helpers;
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using DEWAXP.Foundation.Integration.DubaiModelSvc;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.DewaSvc
{
    [Service(typeof(IDubaiModelServiceClient), Lifetime = Lifetime.Transient)]
    public class DubaiModelSoapClient : BaseDewaGateway, IDubaiModelServiceClient
	{
		private string ServiceUsername
		{
			get { return ConfigurationManager.AppSettings[ConfigKeys.DUBAI_MODEL_UN]; }
		}

		private string ServiceSecret
		{
			get { return ConfigurationManager.AppSettings[ConfigKeys.DUBAI_MODEL_PWD]; }
		}

		public ServiceResponse<ContractAccountClassificationResponse> GetAccountClassification(string contractAccountNumber, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
		{
			using (var proxy = CreateProxy())
			{
				var request = new getAccountTypeStatus()
				{
					appidentifier = segment.Identifier(),
					appversion = AppVersion,
					merchantid = GetMerchantId(segment),
					merchantpassword = GetMerchantPassword(segment),
					vendorid = GetVendorId(segment),
					contractaccountnumber = contractAccountNumber,
					lang = language.Code(),
					mobileosversion = "ds"
				};

				try
				{
					var response = proxy.getAccountTypeStatus(request);
					var typedResponse = ContractAccountClassificationResponse.From(response.@return);
					if (typedResponse.ResponseCode != 0)
					{
						return new ServiceResponse<ContractAccountClassificationResponse>(null, false, typedResponse.Description);
					}
					return new ServiceResponse<ContractAccountClassificationResponse>(typedResponse);
				}
				catch (Exception ex)
				{
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<ContractAccountClassificationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
				}

			}
		}

		#region Proxy configuration methods

		private ConfigClient CreateProxy()
		{
			var client = new ConfigClient(CreateBinding(), GetEndpointAddress("DubaiModelPort"));
			client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
			client.ChannelFactory.Endpoint.Behaviors.Add(new DubaiModelServiceCredentials());

			client.ClientCredentials.UserName.UserName = ServiceUsername;
			client.ClientCredentials.UserName.Password = ServiceSecret;

			return client;
		}
		
		private Binding CreateBinding()
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

            if (!string.IsNullOrWhiteSpace(DubaiModelSSLSettings) && DubaiModelSSLSettings.Equals("1"))
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

			return binding;
		}

		#endregion
	}
}
