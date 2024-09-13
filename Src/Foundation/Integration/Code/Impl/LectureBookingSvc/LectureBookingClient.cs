using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Helpers;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Integration.LectureBookingSvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.LectureBookingSvc
{
    [Service(typeof(ILectureBookingClient), Lifetime = Lifetime.Transient)]
    public class LectureBookingClient : BaseDewaGateway, ILectureBookingClient
    {
        public ServiceResponse<PutLectureBookingAwardSubmissionResponse> PutLectureBookingAwardSubmission(PutLectureBookingAwardSubmission request,
            string userId,
            SupportedLanguage language = SupportedLanguage.English,
            RequestSegment segment = RequestSegment.Desktop)
        {


            try
            {
                using (var client = CreateProxy())
                {
                    var serviceRequest = request;
                    serviceRequest.merchantid = GetVendorId(segment);
                    serviceRequest.lang = language.Code();
                    serviceRequest.userid = userId;
                    serviceRequest.appversion = AppVersion;
                    serviceRequest.mobileosversion = AppVersion;

                    //vendorid = GetVendorId(segment),
                    //lang = language.Code(),
                    //tokenid = string.Empty,
                    //userid = userId,
                    //contractaccount = contractAccountNumber,
                    //appidentifier = segment.Identifier(),
                    //appver = c


                    var response = client.PutLectureBookingAwardSubmission(serviceRequest);


                    if (response != null && response.@return != null && response.@return.responseCode == "000")
                    {
                        var typedresponse = new ServiceResponse<PutLectureBookingAwardSubmissionResponse>(response);
                        return typedresponse;
                    }
                    else if (response != null && response.@return != null && response.@return.responseCode == "105")
                    {
                        var typedresponse = new ServiceResponse<PutLectureBookingAwardSubmissionResponse>(response, true, response.@return.description);
                        return typedresponse;
                    }
                    else
                    {
                        var typedresponse = new ServiceResponse<PutLectureBookingAwardSubmissionResponse>(response, false, response.@return.description);
                        return typedresponse;
                    }
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<PutLectureBookingAwardSubmissionResponse>(null, false, "timeout error message");
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<PutLectureBookingAwardSubmissionResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }
        }

        #region Factory methods

        private CustomerServicesClient CreateProxy()
        {
            var client = new CustomerServicesClient(CreateBinding(), GetEndpointAddress("CSPort"));

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
