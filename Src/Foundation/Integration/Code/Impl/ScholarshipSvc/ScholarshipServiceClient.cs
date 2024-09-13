using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Impl.OauthClientCredentials;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.ScholarshipService;
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

namespace DEWAXP.Foundation.Integration.Impl.ScholarshipSvc
{
    [Service(typeof(IScholarshipServiceClient), Lifetime = Lifetime.Transient)]
    public class ScholarshipServiceClient : BaseDewaGateway, IScholarshipServiceClient
    {
        public ServiceResponse<LoginScholarshipResponse> SignIn(string userId, string password, SupportedLanguage language, RequestSegment segment, string credentials = "")
        {
            using (var client = CreateProxy())
            {
                var request = new LoginScholarship()
                {
                    username = userId,
                    password = password,
                    //merchantid = GetMerchantId(segment),
                    //merchantpassword = GetMerchantPassword(segment),
                    credential = credentials,
                    lang = language.Code(),
                    vendorid = GetVendorId(segment)
                };

                try
                {
                    var response = client.LoginScholarship(request);

                    var typedResponse = response.@return;

                    if (typedResponse.responseCode != "000")
                    {
                        string error = "";
                        if (response.@return.errorList != null)
                        {
                            error = GetErrorMessage(response.@return.errorList, response.@return.description);
                        }

                        return new ServiceResponse<LoginScholarshipResponse>(null, false, error);

                    }

                    return new ServiceResponse<LoginScholarshipResponse>(response, true);
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<LoginScholarshipResponse>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse SetNewPassword(string username, string currentPassword, string newPassword,
            string credentials, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new ResetPasswordScholarship()
                {
                    currentpassword = currentPassword,
                    newpassword = newPassword,
                    username = username,
                    vendorid = GetVendorId(segment),
                    lang = language.Code(),
                    credential = ""
                };

                try
                {
                    var response = client.ResetPasswordScholarship(request);

                    //var typedResponse = response.@return.DeserializeAs<SetNewPasswordResponse>();
                    if (response.@return.responseCode != "000")
                    {
                        string error = "";
                        if (response.@return.errorList != null)
                        {
                            error = GetErrorMessage(response.@return.errorList, response.@return.description);
                        }

                        return new ServiceResponse(false, error);
                    }
                    return new ServiceResponse();
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse<scholarshipHelpValues> GetHelpValuesInEnglish()
        {
            using (var client = CreateProxy())
            {
                try
                {

                    var vl = new GetScholarshipHelpValues() { lang = SupportedLanguage.English.ToString() };

                    return new ServiceResponse<scholarshipHelpValues>(client.GetScholarshipHelpValues(vl).@return);

                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<scholarshipHelpValues>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
            //var ojb = GetScholarshipHelpValuesResponse()
        }

        public ServiceResponse<scholarshipHelpValues> GetHelpValuesInArabic()
        {
            using (var client = CreateProxy())
            {
                try
                {

                    var vl = new GetScholarshipHelpValues() { lang = SupportedLanguage.Arabic.ToString() };

                    return new ServiceResponse<scholarshipHelpValues>(client.GetScholarshipHelpValues(vl).@return);

                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<scholarshipHelpValues>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }

        }


        public ServiceResponse<string> NewRegistration(
            string programID, string fName, string mName, string lName,
            string mobileNo, string username, string password, string password1,
            string email, string email1, string emiratesId, bool register,
            SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                try
                {
                    var model = new RegisterScholarship()
                    {
                        lang = language.ToString(),
                        registrationdata = new registrationData()
                        {
                            email = email,
                            emailRepeat = email1,
                            emiratesId = emiratesId,
                            firstName = fName,
                            lastName = lName,
                            middleName = mName,
                            passportNo = mobileNo,
                            password = password,
                            passwordRepeat = password1,
                            program = programID,
                            userId = username,
                            registerFlag = (register ? "Y" : string.Empty)
                        },
                        vendorid = GetVendorId(segment)
                    };

                    var response = client.RegisterScholarship(model);
                    if (response.@return.responseCode != "000")
                    {
                        string error = "";
                        if (response.@return.errorList != null)
                        {
                            error = GetErrorMessage(response.@return.errorList, response.@return.description);
                        }

                        return new ServiceResponse<string>(null, false, error);
                    }

                    return new ServiceResponse<string>(response.@return.description, true);


                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
            //var ojb = GetScholarshipHelpValuesResponse()
        }

        public ServiceResponse<candidateDetails> UpdateCandidateDetails(candidateDetails canDetails, string stage, string credentials, string username,
            SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                try
                {

                    var model = new UpdateCandidateScholarship()
                    {
                        candidatedetails = canDetails,
                        page = stage,
                        credential = credentials,
                        lang = language.ToString(),
                        username = username,
                        vendorid = GetVendorId(segment)
                    };

                    var response = client.UpdateCandidateScholarship(model);
                    if (response.@return.responseCode != "000")
                    {
                        string error = "";
                        if (response.@return.errorList != null)
                        {
                            error = GetErrorMessage(response.@return.errorList, response.@return.description);
                        }

                        return new ServiceResponse<candidateDetails>(null, false, error);
                    }

                    return new ServiceResponse<candidateDetails>(response.@return.candidateDetails);


                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<candidateDetails>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }

            }
            //var ojb = GetScholarshipHelpValuesResponse()
        }

        public ServiceResponse ForgotPassword(string userId, string email, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new ForgotPasswordScholarship()
                {
                    username = userId,
                    email = email,
                    vendorid = GetVendorId(segment),
                    lang = language.ToString(),
                };

                try
                {
                    var response = client.ForgotPasswordScholarship(request);

                    var typedResponse = response.@return;
                    if (typedResponse.responseCode != "000")
                    {
                        string error = "";
                        if (response.@return.errorList != null)
                        {
                            error = GetErrorMessage(response.@return.errorList, response.@return.description);
                        }

                        return new ServiceResponse(false, error);
                    }
                    return new ServiceResponse();
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse(false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        public ServiceResponse RequestUsername(string email, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            using (var client = CreateProxy())
            {
                var request = new ForgotUserNameScholarship()
                {
                    email = email,
                    lang = language.ToString(),
                    vendorid = GetVendorId(segment)
                };

                try
                {
                    var response = client.ForgotUserNameScholarship(request);

                    var typedResponse = response.@return;
                    if (typedResponse.responseCode != "000")
                    {
                        string error = "";
                        if (response.@return.errorList != null)
                        {
                            error = GetErrorMessage(response.@return.errorList, response.@return.description);
                        }
                        LogService.Debug(error);
                        return new ServiceResponse(false, error);
                    }
                    return new ServiceResponse();
                }
                catch (System.Exception ex)
                {
                    LogService.Fatal(ex, this);
                    return new ServiceResponse<string>(null, false, ErrorMessages.FRONTEND_ERROR_MESSAGE);
                }
            }
        }

        #region Service Proxy methods
        private HCMV1Client CreateProxy()
        {
            //var client = new SmartVendorSvc.SmartVendorClient(CreateBinding(), GetEndpointAddress("HRService"));
            var client = new HCMV1Client(CreateBinding(), GetEndpointAddress("HRService"));
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

        #region Private Methods

        private string GetErrorMessage(validationError[] errorList, string message)
        {
            if (errorList == null || errorList.Length == 0) return message;

            return string.Join(",", errorList.Select(e => string.Format("{0}:{1}", e.field, e.errorText)).ToList());


        }

        #endregion
    }
}
