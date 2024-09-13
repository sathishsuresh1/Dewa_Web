using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.InternshipSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.DI;

namespace DEWAXP.Foundation.Integration.Impl.InternshipSvc
{
    [Service(typeof(IinternshipServiceClient), Lifetime = Lifetime.Transient)]
    public class InternshipServiceClient : BaseDewaGateway, IinternshipServiceClient
	{
		public ServiceResponse<SetInternshipRegistrationResponse> SetInternshipRegistration(SetInternshipRegistration request)
		{
			var client = CreateProxy();

			try
			{
				SetInternshipRegistrationRequest requestRegistration = new SetInternshipRegistrationRequest();
				requestRegistration.SetInternshipRegistration = request;

				var response = client.SetInternshipRegistration(requestRegistration);

				var typedResponse = response.SetInternshipRegistrationResponse;

				if (typedResponse.@return.errorcode == "0")
				{
					return new ServiceResponse<SetInternshipRegistrationResponse>(response.SetInternshipRegistrationResponse, true);

				}

				return new ServiceResponse<SetInternshipRegistrationResponse>(null, false, typedResponse.@return.errormessage);
			}
			catch (System.Exception ex)
			{
				LogService.Fatal(ex, this);
				return new ServiceResponse<SetInternshipRegistrationResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
			}
		}

		public ServiceResponse<GetInternshipHelpValuesResponse> GetHelpValues(GetInternshipHelpValues request, SupportedLanguage language = SupportedLanguage.English)
		{
            try
            {
                var client = CreateProxy();
                //GetInternshipHelpValues request = new GetInternshipHelpValues();

                //request.Relation = "X";
                //request.countrycodes = "X";
                //request.departmentdivision = "X";
                request.Language = language.Code();

                GetInternshipHelpValuesRequest requestHelpvalues = new GetInternshipHelpValuesRequest();
                requestHelpvalues.GetInternshipHelpValues = request;

                var response = client.GetInternshipHelpValues(requestHelpvalues);


                return new ServiceResponse<GetInternshipHelpValuesResponse>(response.GetInternshipHelpValuesResponse, true);
            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<GetInternshipHelpValuesResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }

        }


		#region Service Proxy methods
		private HCMV1 CreateProxy()
		{
			var client = new HCMV1Client(CreateBinding(), GetEndpointAddress("HRService1"));
			client.ChannelFactory.Endpoint.Behaviors.Remove<System.ServiceModel.Description.ClientCredentials>();
			client.ChannelFactory.Endpoint.Behaviors.Add(new Helpers.DewaApiCredentials());
			client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
			client.ClientCredentials.UserName.UserName = BbUsername;
			client.ClientCredentials.UserName.Password = BbPassword;

			return client;
		}
		#endregion

		#region Custom binding code

		private CustomBinding CreateBinding()
		{
			var binding = new CustomBinding()
			{
				ReceiveTimeout = TimeSpan.FromMinutes(2),
				SendTimeout = TimeSpan.FromMinutes(1)
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
			binding.Elements.Add(new Helpers.CustomMessageEncoder.CustomTextMessageBindingElement());
			binding.Elements.Add(transport);

			return binding;
		}
		#endregion


	}
}
