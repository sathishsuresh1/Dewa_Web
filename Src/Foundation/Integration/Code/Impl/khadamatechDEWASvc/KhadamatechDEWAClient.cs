using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Helpers.CustomMessageEncoder;
using DEWAXP.Foundation.Integration.KhadamatechDEWASvc;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Integration.Impl.KhadamatechDEWASvc
{
    [Service(typeof(IKhadamatechDEWAServiceClient), Lifetime = Lifetime.Transient)]
    public class KhadamatechDEWAClient : BaseDewaGateway, IKhadamatechDEWAServiceClient
    {
        public ServiceResponse<CreateReqResponse> CreateReq(CreateReqRequest request)
        {
            CreateReqResponse response = new CreateReqResponse();
            try
            {
                using (BMCPortTypeClient client = new BMCPortTypeClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    var returnData = client.CreateReq(GetAuthenticationInfo(), request.VisitorEmailid, request.Visitorname, request.Projectname, request.ProjectID,
                           request.Designation, request.MobileNumber, request.Nationality, request.Location,
                           request.Entrydate, request.Entryintime, request.Entryouttime, request.EmiratesID, request.EmiratesIDExpirydate,
                           request.Passport, request.PassportExpirydate, request.VisaNumber, request.VisaNumberExpirydate,
                           request.EmiratesIDCard_attachmentName, request.EmiratesIDCard_attachmentData, request.EmiratesIDCard_attachmentOrigSize,
                           request.PassportCard_attachmentName, request.PassportCard_attachmentData, request.PassportCard_attachmentOrigSize,
                           request.VisaNumberCard_attachmentName, request.VisaNumberCard_attachmentData, request.VisaNumberCard_attachmentOrigSize,
                           request.ApplicatePhoto_attachmentName, request.ApplicatePhoto_attachmentData, request.ApplicatePhoto_attachmentOrigSize,
                           request.Entry_In, request.Exit_Out, request.Comment, request.CardNo, request.Flag,
                           request.EID_Filecontenttype,
                            request.EID_Fileextension,
                            request.EID_Filetype,
                            request.Photo_Filecontenttype,
                            request.Photo_Fileextension,
                            request.Photo_Filetype,
                            request.Passport_Filecontenttype,
                            request.Passport_Fileextension,
                            request.Passport_Filetype,
                            request.VISA_Filecontenttype,
                            request.VISA_Fileextension,
                            request.VISA_Filetype,
                           out response.Message, out response.WorkOrderID);
                    response.GatePassID = returnData;
                }

            }
            catch (TimeoutException)
            {
                return new ServiceResponse<CreateReqResponse>(null, false, "timeout error message");
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                return new ServiceResponse<CreateReqResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
            }

            return new ServiceResponse<CreateReqResponse>(response);
        }


        #region Factory methods

        //private BMCPortTypeClient CreateProxy()
        //{
        //    BMCPortTypeClient bMCPortTypeClient = new BMCPortTypeClient();
        //   /// var client = new BMCPortTypeClient(CreateBinding(), GetEndpointAddress("BMCSoap2"));

        //    //Comment out this code when a valid certificate is available
        //    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
        //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;



        //    //client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
        //    //client.ChannelFactory.Endpoint.Behaviors.Add(new DewaApiCredentials());
        //    //client.Endpoint.EndpointBehaviors.Add(new CustomAuthenticationBehavior(WebConfigurationManager.AppSettings["RestAPI_Client_Id"], "Bearer " + OAuthToken.GetAccessToken()));
        //    //client.ClientCredentials.UserName.UserName = BbUsername;
        //    //client.ClientCredentials.UserName.Password = BbPassword;
        //    return client;
        //}

        //private CustomBinding CreateBinding()
        //{
        //    var binding = new CustomBinding()
        //    {
        //        ReceiveTimeout = TimeSpan.FromMinutes(2),
        //        SendTimeout = TimeSpan.FromMinutes(2)
        //    };

        //    var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
        //    security.IncludeTimestamp = true;
        //    security.LocalClientSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
        //    security.LocalServiceSettings.MaxClockSkew = new TimeSpan(0, 0, 10, 0);
        //    security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
        //    security.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
        //    security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
        //    security.EnableUnsecuredResponse = true;
        //    security.AllowInsecureTransport = true;

        //    var encoding = new TextMessageEncodingBindingElement();
        //    encoding.MessageVersion = MessageVersion.Soap11;

        //    var transport = new HttpsTransportBindingElement();
        //    transport.MaxReceivedMessageSize = 20000000; // 20 megs

        //    binding.Elements.Add(security);
        //    binding.Elements.Add(new CustomTextMessageBindingElement());
        //    binding.Elements.Add(transport);


        //    return binding;
        //}

        private AuthenticationInfo GetAuthenticationInfo()
        {

            AuthenticationInfo authenticationInfo = new AuthenticationInfo
            {
                userName = Kadamtech_DEWA_Username,
                password = Kadamtech_DEWA_Password,
            };

            return authenticationInfo;
        }
        #endregion
    }
}
